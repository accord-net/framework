// Accord Imaging Library (non-commercial only)
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Kaiming He, Jian Sun, and Xiaoou Tang
// http://kaiminghe.com/eccv10/index.html 
//
// Copyright © Hashem Zawary, 2016
// hashemzawary@gmail.com
// https://www.linkedin.com/in/hashem-zavvari-53b01457
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
// This code has been contributed by Hashem Zawary based on the original from 
//   Kaiming He, Jian Sun, and Xiaoou Tang's Fast Guided Filter. Please note 
//   that this code is only available under a special license that specifically
//   *denies* the use for commercial applications and is thus *not compatible 
//   with the LGPL and the GPL*. Use at your own risk.
//

namespace Accord.Imaging.Filters
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    ///   Fast Guided Filter (non-commercial).
    /// </summary>
    /// 
    /// <remarks>
    ///   <para>This class implements the Fast Guided Filter as described in 
    ///   "Kaiming He, Jian Sun, and Xiaoou Tang; Guided Image Filtering, ECCV 2010" and
    ///   in TPAMI 2013. If this code is used or adapted, either as a stand-alone tool or
    ///   as a component of any algorithm, it is necessary to appropriately cite Kaiming He,
    ///   Jian Sun, and Xiaoou Tangyou's work.</para>
    ///   
    ///   <para>This code is intended for academic purposes only, and <b>cannot be used for 
    ///   commercial or industrial activities</b>. For this reason, this code is not under 
    ///   the LGPL license and is only available in the framework's Accord.Imaging.Noncommercial 
    ///   assembly.</para>
    ///   
    ///   <para>The original work can be found in http://kaiminghe.com/eccv10/index.html </para>
    /// </remarks>
    /// 
    public class FastGuidedFilter : BaseInPlaceFilter2
    {
        private byte _kernelSize = 9;
        private float _epsilon = 0.01f;
        private float _subSamplingRatio = 0.25f;


        // private format translation dictionary
        private readonly Dictionary<PixelFormat, PixelFormat> _formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return _formatTranslations; }
        }


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
            _formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
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
            // TODO: Refactor, add "using" clauses, manipulate pixel/pointers directly
            BaseResizeFilter resizer = new ResizeNearestNeighbor(
                (int)(image.Width * _subSamplingRatio),
                (int)(image.Height * _subSamplingRatio));

            UnmanagedImage imageSub = resizer.Apply(image);
            UnmanagedImage overlaySub = resizer.Apply(overlay);

            byte kernelSizeSub = (byte)(_kernelSize * _subSamplingRatio);
            FastBoxBlur blur = new FastBoxBlur(kernelSizeSub, kernelSizeSub);

            UnmanagedImage imageBorder = blur.Apply(GetFilledImage(imageSub.Width, imageSub.Height, imageSub.PixelFormat, Color.White));

            UnmanagedImage imageMean = blur.Apply(imageSub);
            new Divide(imageBorder).ApplyInPlace(imageMean);

            UnmanagedImage overlayMean = blur.Apply(overlaySub);
            new Divide(imageBorder).ApplyInPlace(overlayMean);

            UnmanagedImage mulMean = new Multiply(overlaySub).Apply(imageSub);
            overlaySub.Dispose();
            blur.ApplyInPlace(mulMean);
            new Divide(imageBorder).ApplyInPlace(mulMean);

            // This is the covariance of (image, overlay) in each local patch.
            UnmanagedImage mulCov = new Subtract(new Multiply(overlayMean).Apply(imageMean)).Apply(mulMean);
            mulMean.Dispose();

            UnmanagedImage imageMean2 = new Multiply(imageSub).Apply(imageSub);
            imageSub.Dispose();
            blur.ApplyInPlace(imageMean2);
            new Divide(imageBorder).ApplyInPlace(imageMean2);

            UnmanagedImage imageVar = new Subtract(new Multiply(imageMean).Apply(imageMean)).Apply(imageMean2);
            imageMean2.Dispose();

            byte cc = (byte)(255 * _epsilon);
            var imageEpsilon = GetFilledImage(
                imageVar.Width, imageVar.Height, imageVar.PixelFormat, Color.FromArgb(cc, cc, cc));

            new Add(imageEpsilon).ApplyInPlace(imageVar);
            imageEpsilon.Dispose();

            UnmanagedImage a = new Divide(imageVar).Apply(mulCov);
            imageVar.Dispose();
            mulCov.Dispose();

            UnmanagedImage b = new Subtract(new Multiply(imageMean).Apply(a)).Apply(overlayMean);
            imageMean.Dispose();
            overlayMean.Dispose();

            UnmanagedImage aMean = new Divide(imageBorder).Apply(blur.Apply(a));
            UnmanagedImage bMean = new Divide(imageBorder).Apply(blur.Apply(b));
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
                        Accord.Imaging.Image.Convert8bppTo16bpp(grayImage.ToManagedImage(false)));
            }
            else
            {
                filledImage = UnmanagedImage.Create(width, height, pixelFormat);

                if (pixelFormat == PixelFormat.Format24bppRgb || pixelFormat == PixelFormat.Format32bppArgb || pixelFormat == PixelFormat.Format32bppRgb)
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

                    if (pixelFormat == PixelFormat.Format32bppArgb)
                    {
                        SystemTools.SetUnmanagedMemory(grayImage.ImageData, color.A, grayImage.Stride * grayImage.Height);
                        replaceChannel.Channel = RGB.A;
                        grayImage.Copy(replaceChannel.UnmanagedChannelImage);
                        replaceChannel.ApplyInPlace(filledImage);
                    }
                }
                else
                {
                    SystemTools.SetUnmanagedMemory(grayImage.ImageData, color.R, grayImage.Stride * grayImage.Height);

                    UnmanagedImage grayImage16 = UnmanagedImage.FromManagedImage(
                        Accord.Imaging.Image.Convert8bppTo16bpp(grayImage.ToManagedImage(false)));

                    ReplaceChannel replaceChannel = new ReplaceChannel(RGB.R, grayImage16);
                    replaceChannel.ApplyInPlace(filledImage);


                    SystemTools.SetUnmanagedMemory(grayImage.ImageData, color.G, grayImage.Stride * grayImage.Height);

                    replaceChannel.Channel = RGB.G;
                    replaceChannel.UnmanagedChannelImage.Dispose();

                    replaceChannel.UnmanagedChannelImage =
                        UnmanagedImage.FromManagedImage(Accord.Imaging.Image.Convert8bppTo16bpp(
                            grayImage.ToManagedImage(false)));

                    replaceChannel.ApplyInPlace(filledImage);


                    SystemTools.SetUnmanagedMemory(grayImage.ImageData, color.B, grayImage.Stride * grayImage.Height);

                    replaceChannel.Channel = RGB.B;
                    replaceChannel.UnmanagedChannelImage.Dispose();

                    replaceChannel.UnmanagedChannelImage =
                        UnmanagedImage.FromManagedImage(Accord.Imaging.Image.Convert8bppTo16bpp(
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
