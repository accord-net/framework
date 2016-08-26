// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2011
// contacts@aforgenet.com
//
// Matlab demo code for "Fast Guided Filter" (arXiv 2015)
// by Kaiming He(kahe @microsoft.com)
// If you use/adapt our code in your work(either as a stand-alone tool or as a component
// of any algorithm), you need to appropriately cite our work.
// This code is for academic purpose only.Not for commercial/industrial activities.
//
// Implemented FastGuidedFilter filter by HZ, March-2016
// Reference link: http://research.microsoft.com/en-us/um/people/kahe/eccv10/
// hashemzawary@gmail.com
// https://www.linkedin.com/in/hashem-zavvari-53b01457
//

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
    /// <summary>
    /// FastGuidedFilter filter.
    /// </summary>
    public class FastGuidedFilter : BaseInPlaceFilter2
    {
        // private format translation dictionary
        private readonly Dictionary<PixelFormat, PixelFormat> _formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => _formatTranslations;

        private byte _kernelSize = 9;
        /// <summary>
        /// KernelSize between 9 and 99. Default is 9.
        /// </summary>
        public byte KernelSize
        {
            get { return _kernelSize; }
            set
            {
                _kernelSize = System.Math.Max((byte)9, System.Math.Min((byte)99, value));
            }
        }

        private float _epsilon = 0.01f;
        /// <summary>
        /// Epsilon value between 0.0 and 1.0 . Default is 0.01 .
        /// </summary>
        public float Epsilon
        {
            get { return _epsilon; }
            set
            {
                _epsilon = System.Math.Max(0f, System.Math.Min(1f, value));
            }
        }

        private float _subSamplingRatio = 0.25f;
        /// <summary>
        /// SubSamplingRatio value between 0.25 and 1.0 . Default is 0.25 .
        /// </summary>
        public float SubSamplingRatio
        {
            get { return _subSamplingRatio; }
            set
            {
                _subSamplingRatio = System.Math.Max(0.25f, System.Math.Min(1f, value));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FastGuidedFilter"/> class.
        /// </summary>
        public FastGuidedFilter()
        {
            InitFormatTranslations();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FastGuidedFilter"/> class.
        /// </summary>
        /// 
        /// <param name="overlayImage">Overlay image.</param>
        /// 
        public FastGuidedFilter(Bitmap overlayImage)
            : base(overlayImage)
        {
            InitFormatTranslations();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FastGuidedFilter"/> class.
        /// </summary>
        /// 
        /// <param name="unmanagedOverlayImage">Unmanaged overlay image.</param>
        /// 
        public FastGuidedFilter(UnmanagedImage unmanagedOverlayImage)
            : base(unmanagedOverlayImage)
        {
            InitFormatTranslations();
        }

        // Initialize format translation dictionary
        private void InitFormatTranslations()
        {
            _formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            _formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            //_formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            //_formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
            _formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
            _formatTranslations[PixelFormat.Format48bppRgb] = PixelFormat.Format48bppRgb;
            //_formatTranslations[PixelFormat.Format64bppArgb] = PixelFormat.Format64bppArgb;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image data.</param>
        /// <param name="overlay">Overlay image data.</param>
        ///
        protected override void ProcessFilter(UnmanagedImage image, UnmanagedImage overlay)
        {
            BaseResizeFilter resizer = new ResizeNearestNeighbor(
                (int)(image.Width * _subSamplingRatio),
                (int)(image.Height * _subSamplingRatio));

            var imageSub = resizer.Apply(image);
            var overlaySub = resizer.Apply(overlay);
            var kernelSizeSub = (byte)(_kernelSize * _subSamplingRatio);

            var imageBorder = new FastBoxBlur(kernelSizeSub, kernelSizeSub).Apply(
                GetFilledImage(imageSub.Width, imageSub.Height, imageSub.PixelFormat, Color.White));

            var imageMean = new FastBoxBlur(kernelSizeSub, kernelSizeSub).Apply(imageSub);
            new Divide(imageBorder).ApplyInPlace(imageMean);

            var overlayMean = new FastBoxBlur(kernelSizeSub, kernelSizeSub).Apply(overlaySub);
            new Divide(imageBorder).ApplyInPlace(overlayMean);

            var mulMean = new Multiply(overlaySub).Apply(imageSub);
            overlaySub.Dispose();
            new FastBoxBlur(kernelSizeSub, kernelSizeSub).ApplyInPlace(mulMean);
            new Divide(imageBorder).ApplyInPlace(mulMean);

            //This is the covariance of (image, overlay) in each local patch.
            var mulCov = new Subtract(new Multiply(overlayMean).Apply(imageMean)).Apply(mulMean);
            mulMean.Dispose();

            var imageMean2 = new Multiply(imageSub).Apply(imageSub);
            imageSub.Dispose();
            new FastBoxBlur(kernelSizeSub, kernelSizeSub).ApplyInPlace(imageMean2);
            new Divide(imageBorder).ApplyInPlace(imageMean2);

            var imageVar = new Subtract(new Multiply(imageMean).Apply(imageMean)).Apply(imageMean2);
            imageMean2.Dispose();

            var cc = (byte)(255 * _epsilon);
            var imageEpsilon = GetFilledImage(
                imageVar.Width, imageVar.Height, imageVar.PixelFormat, Color.FromArgb(cc, cc, cc));

            new Add(imageEpsilon).ApplyInPlace(imageVar);
            imageEpsilon.Dispose();

            var a = new Divide(imageVar).Apply(mulCov);
            imageVar.Dispose();
            mulCov.Dispose();
            var b = new Subtract(new Multiply(imageMean).Apply(a)).Apply(overlayMean);
            imageMean.Dispose();
            overlayMean.Dispose();

            var aMean = new Divide(imageBorder).Apply(new FastBoxBlur(kernelSizeSub, kernelSizeSub).Apply(a));
            var bMean = new Divide(imageBorder).Apply(new FastBoxBlur(kernelSizeSub, kernelSizeSub).Apply(b));
            imageBorder.Dispose();
            a.Dispose();
            b.Dispose();

            resizer = new ResizeBilinear(image.Width, image.Height);

            aMean = resizer.Apply(aMean);
            bMean = resizer.Apply(bMean);

            new Multiply(aMean).ApplyInPlace(image);
            aMean.Dispose();
            new Add(bMean).ApplyInPlace(image);
            bMean.Dispose();
        }

        /// <summary>
        /// Get a filled image by a color.
        /// </summary>
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        /// <param name="pixelFormat">Image pixel format.</param>
        /// <param name="color">Image filled color.</param>
        /// <returns>An filled image with a color.</returns>
        public static UnmanagedImage GetFilledImage(int width, int height, PixelFormat pixelFormat, Color color)
        {
            UnmanagedImage filledImage;

            var grayImage = UnmanagedImage.Create(width, height, PixelFormat.Format8bppIndexed);

            if (pixelFormat == PixelFormat.Format8bppIndexed ||
                pixelFormat == PixelFormat.Format16bppGrayScale)
            {
                SystemTools.SetUnmanagedMemory(
                    grayImage.ImageData,
                    (color.R + color.G + color.B) / 3,
                    grayImage.Stride * grayImage.Height);

                filledImage = pixelFormat == PixelFormat.Format8bppIndexed ? grayImage.Clone() :
                    UnmanagedImage.FromManagedImage(
                        AForge.Imaging.Image.Convert8bppTo16bpp(grayImage.ToManagedImage(false)));
            }
            else
            {
                filledImage = UnmanagedImage.Create(width, height, pixelFormat);

                if (pixelFormat == PixelFormat.Format24bppRgb)
                {
                    SystemTools.SetUnmanagedMemory(grayImage.ImageData, color.R, grayImage.Stride * grayImage.Height);
                    var replaceChannel = new ReplaceChannel(RGB.R, grayImage);
                    replaceChannel.ApplyInPlace(filledImage);

                    SystemTools.SetUnmanagedMemory(grayImage.ImageData, color.G, grayImage.Stride * grayImage.Height);
                    replaceChannel.Channel = RGB.G;
                    grayImage.Copy(replaceChannel.UnmanagedChannelImage);
                    replaceChannel.ApplyInPlace(filledImage);

                    SystemTools.SetUnmanagedMemory(grayImage.ImageData, color.B, grayImage.Stride * grayImage.Height);
                    replaceChannel.Channel = RGB.B;
                    grayImage.Copy(replaceChannel.UnmanagedChannelImage);
                    replaceChannel.ApplyInPlace(filledImage);
                }
                else
                {
                    SystemTools.SetUnmanagedMemory(grayImage.ImageData, color.R, grayImage.Stride * grayImage.Height);

                    var grayImage16 = UnmanagedImage.FromManagedImage(
                        AForge.Imaging.Image.Convert8bppTo16bpp(grayImage.ToManagedImage(false)));

                    var replaceChannel = new ReplaceChannel(RGB.R, grayImage16);
                    replaceChannel.ApplyInPlace(filledImage);


                    SystemTools.SetUnmanagedMemory(grayImage.ImageData, color.G, grayImage.Stride * grayImage.Height);

                    replaceChannel.Channel = RGB.G;
                    replaceChannel.UnmanagedChannelImage.Dispose();

                    replaceChannel.UnmanagedChannelImage =
                        UnmanagedImage.FromManagedImage(AForge.Imaging.Image.Convert8bppTo16bpp(
                            grayImage.ToManagedImage(false)));

                    replaceChannel.ApplyInPlace(filledImage);


                    SystemTools.SetUnmanagedMemory(grayImage.ImageData, color.B, grayImage.Stride * grayImage.Height);

                    replaceChannel.Channel = RGB.B;
                    replaceChannel.UnmanagedChannelImage.Dispose();

                    replaceChannel.UnmanagedChannelImage =
                        UnmanagedImage.FromManagedImage(AForge.Imaging.Image.Convert8bppTo16bpp(
                            grayImage.ToManagedImage(false)));

                    replaceChannel.ApplyInPlace(filledImage);
                    replaceChannel.UnmanagedChannelImage.Dispose();

                    grayImage16.Dispose();
                }
            }

            grayImage.Dispose();

            return filledImage;
        }
    }
}
