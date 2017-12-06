// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace Accord.Imaging.Converters
{
    using Accord.Math;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Numerics;

    /// <summary>
    ///   Bitmap to jagged array converter.
    /// </summary>
    /// 
    /// <remarks>
    ///   This class converts images to single or jagged arrays of
    ///   either double-precision or single-precision floating-point
    ///   values.
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   This example converts a 16x16 Bitmap image into
    ///   a double[] array with values between 0 and 1.</para>
    ///   
    /// <code>
    /// // Obtain a 16x16 bitmap image
    /// // Bitmap image = ...
    /// 
    /// // Show on screen
    /// ImageBox.Show(image, PictureBoxSizeMode.Zoom);
    /// 
    /// // Create the converter to convert the image to an
    /// //   array containing only values between 0 and 1 
    /// ImageToArray conv = new ImageToArray(min: 0, max: 1);
    /// 
    /// // Convert the image and store it in the array
    /// double[] array; conv.Convert(image, out array);
    /// 
    /// // Show the array on screen
    /// ImageBox.Show(array, 16, 16, PictureBoxSizeMode.Zoom);    /// </code>
    /// 
    /// <para>
    ///   The resulting image is shown below.</para>
    /// 
    /// <img src="..\images\image-to-matrix.png" />
    /// 
    /// </example>
    /// 
    public class ImageToArray :
        IConverter<Bitmap, double[]>,
        IConverter<UnmanagedImage, double[]>,
        IConverter<Bitmap, double[][]>,
        IConverter<UnmanagedImage, double[][]>,
        IConverter<Bitmap, float[]>,
        IConverter<UnmanagedImage, float[]>,
        IConverter<Bitmap, float[][]>,
        IConverter<UnmanagedImage, float[][]>,
        IConverter<Bitmap, Color[]>,
        IConverter<UnmanagedImage, Color[]>,
        IConverter<Bitmap, Complex[]>,
        IConverter<UnmanagedImage, Complex[]>
    {

        /// <summary>
        ///   Gets or sets the maximum double value in the
        ///   double array associated with the brightest color.
        /// </summary>
        /// 
        public double Max { get; set; }

        /// <summary>
        ///   Gets or sets the minimum double value in the
        ///   double array associated with the darkest color.
        /// </summary>
        /// 
        public double Min { get; set; }

        /// <summary>
        ///   Gets or sets the channel to be extracted.
        /// </summary>
        /// 
        public int Channel { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ImageToArray"/> class.
        /// </summary>
        /// 
        /// <param name="min">
        ///   The minimum double value in the double array
        ///   associated with the darkest color. Default is 0.
        /// </param>
        /// <param name="max">
        ///   The maximum double value in the double array
        ///   associated with the brightest color. Default is 1.
        /// </param>
        /// <param name="channel">The channel to extract. Default is 0.</param>
        ///   
        public ImageToArray(double min, double max, int channel)
        {
            this.Min = min;
            this.Max = max;
            this.Channel = channel;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ImageToArray"/> class.
        /// </summary>
        /// 
        public ImageToArray() 
            : this(0, 1) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ImageToArray"/> class.
        /// </summary>
        /// 
        /// <param name="min">
        ///   The minimum double value in the double array associated with the darkest color. Default is 0.
        /// </param>
        /// <param name="max">
        ///   The maximum double value in the double array associated with the brightest color. Default is 1.
        /// </param>
        ///   
        public ImageToArray(double min, double max) 
            : this(min, max, 0) { }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(Bitmap input, out double[][] output)
        {
            Accord.Imaging.Tools.CheckGrayscale(input);

            BitmapData bitmapData = input.LockBits(ImageLockMode.ReadOnly);

            Convert(new UnmanagedImage(bitmapData), out output);

            input.UnlockBits(bitmapData);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(Bitmap input, out float[][] output)
        {
            Accord.Imaging.Tools.CheckGrayscale(input);

            BitmapData bitmapData = input.LockBits(ImageLockMode.ReadOnly);

            Convert(new UnmanagedImage(bitmapData), out output);

            input.UnlockBits(bitmapData);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(Bitmap input, out double[] output)
        {
            Accord.Imaging.Tools.CheckGrayscale(input);

            BitmapData bitmapData = input.LockBits(ImageLockMode.ReadOnly);

            Convert(new UnmanagedImage(bitmapData), out output);

            input.UnlockBits(bitmapData);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(Bitmap input, out Complex[] output)
        {
            Accord.Imaging.Tools.CheckGrayscale(input);

            BitmapData bitmapData = input.LockBits(ImageLockMode.ReadOnly);

            Convert(new UnmanagedImage(bitmapData), out output);

            input.UnlockBits(bitmapData);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(Bitmap input, out float[] output)
        {
            Accord.Imaging.Tools.CheckGrayscale(input);

            BitmapData bitmapData = input.LockBits(ImageLockMode.ReadOnly);

            Convert(new UnmanagedImage(bitmapData), out output);

            input.UnlockBits(bitmapData);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(Bitmap input, out Color[] output)
        {
            Accord.Imaging.Tools.CheckGrayscale(input);

            BitmapData bitmapData = input.LockBits(ImageLockMode.ReadOnly);

            Convert(new UnmanagedImage(bitmapData), out output);

            input.UnlockBits(bitmapData);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(UnmanagedImage input, out double[][] output)
        {
            int width = input.Width;
            int height = input.Height;
            int pixelSize = input.PixelSize;
            int offset = input.Offset;

            output = new double[width * height][];

            unsafe
            {
                byte* src = (byte*)input.ImageData.ToPointer();
                int dst = 0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++, dst++)
                    {
                        double[] pixel = output[dst] = new double[pixelSize];
                        for (int i = pixel.Length - 1; i >= 0; i--, src++)
                            pixel[i] = Vector.Scale(*src, (byte)0, (byte)255, Min, Max);
                    }
                    src += offset;
                }
            }
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(UnmanagedImage input, out Complex[][] output)
        {
            int width = input.Width;
            int height = input.Height;
            int pixelSize = input.PixelSize;
            int offset = input.Offset;

            output = new Complex[width * height][];

            unsafe
            {
                byte* src = (byte*)input.ImageData.ToPointer();
                int dst = 0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++, dst++)
                    {
                        Complex[] pixel = output[dst] = new Complex[pixelSize];
                        for (int i = pixel.Length - 1; i >= 0; i--, src++)
                            pixel[i] = Vector.Scale(*src, (byte)0, (byte)255, Min, Max);
                    }
                    src += offset;
                }
            }
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(UnmanagedImage input, out float[][] output)
        {
            int width = input.Width;
            int height = input.Height;
            int pixelSize = input.PixelSize;
            int offset = input.Offset;

            output = new float[width * height][];

            float min = (float)Min;
            float max = (float)Max;

            unsafe
            {
                byte* src = (byte*)input.ImageData.ToPointer();
                int dst = 0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++, dst++)
                    {
                        float[] pixel = output[dst] = new float[pixelSize];
                        for (int i = pixel.Length - 1; i >= 0; i--, src++)
                            pixel[i] = Vector.Scale(*src, (byte)0, (byte)255, min, max);
                    }
                    src += offset;
                }
            }
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(UnmanagedImage input, out double[] output)
        {
            int width = input.Width;
            int height = input.Height;
            int pixelSize = input.PixelSize;
            int offset = input.Offset;

            output = new double[width * height];

            unsafe
            {
                if (input.PixelFormat == PixelFormat.Format16bppGrayScale)
                {
                    short* src = (short*)input.ImageData.ToPointer();
                    int dst = 0;

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, dst++, src++)
                            output[dst] = Vector.Scale(*src, 0, 65535, Min, Max);

                        src += offset;
                    }
                }
                else
                {
                    byte* src = (byte*)input.ImageData.ToPointer() + Channel;
                    int dst = 0;

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, dst++, src += pixelSize)
                            output[dst] = Vector.Scale(*src, (byte)0, (byte)255, Min, Max);

                        src += offset;
                    }
                }
            }
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(UnmanagedImage input, out Complex[] output)
        {
            int width = input.Width;
            int height = input.Height;
            int pixelSize = input.PixelSize;
            int offset = input.Offset;

            output = new Complex[width * height];

            unsafe
            {
                if (input.PixelFormat == PixelFormat.Format16bppGrayScale)
                {
                    short* src = (short*)input.ImageData.ToPointer();
                    int dst = 0;

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, dst++, src++)
                            output[dst] = Vector.Scale(*src, 0, 65535, Min, Max);

                        src += offset;
                    }
                }
                else
                {
                    byte* src = (byte*)input.ImageData.ToPointer() + Channel;
                    int dst = 0;

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, dst++, src += pixelSize)
                            output[dst] = Vector.Scale(*src, (byte)0, (byte)255, Min, Max);

                        src += offset;
                    }
                }
            }
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(UnmanagedImage input, out float[] output)
        {
            int width = input.Width;
            int height = input.Height;
            int pixelSize = input.PixelSize;
            int offset = input.Offset;

            output = new float[width * height];

            float min = (float)Min;
            float max = (float)Max;

            unsafe
            {
                if (input.PixelFormat == PixelFormat.Format16bppGrayScale)
                {
                    short* src = (short*)input.ImageData.ToPointer();
                    int dst = 0;

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, dst++, src++)
                            output[dst] = Vector.Scale(*src, 0, 65535, min, max);

                        src += offset;
                    }
                }
                else
                {
                    byte* src = (byte*)input.ImageData.ToPointer() + Channel;
                    int dst = 0;

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, dst++, src += pixelSize)
                            output[dst] = Vector.Scale(*src, (byte)0, (byte)255, min, max);

                        src += offset;
                    }
                }
            }
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(UnmanagedImage input, out Color[] output)
        {
            int width = input.Width;
            int height = input.Height;

            int pixelSize = input.PixelSize;
            int offset = input.Offset;

            output = new Color[input.Width * input.Height];
            int dst = 0;

            unsafe
            {
                if (input.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    byte* src = (byte*)input.ImageData.ToPointer();

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, src += pixelSize, dst++)
                            output[dst] = Color.FromArgb(*src, *src, *src);
                        src += offset;
                    }
                }
                else if (input.PixelFormat == PixelFormat.Format24bppRgb
                      || input.PixelFormat == PixelFormat.Format32bppRgb)
                {
                    byte* src = (byte*)input.ImageData.ToPointer();

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, src += pixelSize, dst++)
                            output[dst] = Color.FromArgb(src[RGB.R], src[RGB.G], src[RGB.B]);
                        src += offset;
                    }
                }
                else if (input.PixelFormat == PixelFormat.Format32bppArgb)
                {
                    byte* src = (byte*)input.ImageData.ToPointer();

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, src += pixelSize, dst++)
                            output[dst] = Color.FromArgb(src[RGB.A], src[RGB.R], src[RGB.G], src[RGB.B]);
                        src += offset;
                    }
                }
                else
                {
                    throw new UnsupportedImageFormatException("Pixel format is not supported.");
                }
            }
        }

    }
}
