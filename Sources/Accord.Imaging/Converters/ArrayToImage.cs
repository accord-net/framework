// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
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
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using AForge.Imaging;

    /// <summary>
    ///   Jagged array to Bitmap converter.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This class can convert double and float arrays to either Grayscale
    ///   or color Bitmap images. Color images should be represented as an
    ///   array of pixel values for the final image. The actual dimensions
    ///   of the image should be specified in the class constructor.</para>
    ///   
    /// <para>
    ///   When this class is converting from <see cref="T:byte[]"/> or
    ///   <see cref="T:System.Drawing.Color[]"/>, the values of the <see cref="Max"/>
    ///   and <see cref="Min"/> properties are ignored and no scaling operation
    ///   is performed.</para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   This example converts a single array of double-precision floating-
    ///   point numbers with values from 0 to 1 into a grayscale image.</para>
    ///   
    /// <code>
    /// // Create an array representation 
    /// // of a 4x4 image with a inner 2x2
    /// // square drawn in the middle
    ///
    /// double[] pixels = 
    /// {
    ///     0, 0, 0, 0, 
    ///     0, 1, 1, 0, 
    ///     0, 1, 1, 0, 
    ///     0, 0, 0, 0, 
    /// };
    ///
    /// // Create the converter to create a Bitmap from the array
    /// ArrayToImage conv = new ArrayToImage(width: 4, height: 4);
    ///
    /// // Declare an image and store the pixels on it
    /// Bitmap image; conv.Convert(pixels, out image);
    ///
    /// // Show the image on screen
    /// image = new ResizeNearestNeighbor(320, 320).Apply(image);
    /// ImageBox.Show(image, PictureBoxSizeMode.Zoom);
    /// </code>
    /// 
    /// <para>
    ///   The resulting image is shown below.</para>
    ///   
    /// <img src="..\images\matrix-to-image.png" />
    /// 
    /// </example>
    /// 
    public class ArrayToImage :
        IConverter<double[], Bitmap>,
        IConverter<double[], UnmanagedImage>,
        IConverter<double[][], Bitmap>,
        IConverter<double[][], UnmanagedImage>,
        IConverter<float[], Bitmap>,
        IConverter<float[], UnmanagedImage>,
        IConverter<float[][], Bitmap>,
        IConverter<float[][], UnmanagedImage>,
        IConverter<byte[], Bitmap>,
        IConverter<byte[], UnmanagedImage>,
        IConverter<byte[][], Bitmap>,
        IConverter<byte[][], UnmanagedImage>,
        IConverter<Color[], Bitmap>,
        IConverter<Color[], UnmanagedImage>
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
        ///   Gets or sets the height of the image
        ///   stored in the double array.
        /// </summary>
        /// 
        public int Height { get; set; }

        /// <summary>
        ///   Gets or sets the width of the image
        ///   stored in the double array.
        /// </summary>
        /// 
        public int Width { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ArrayToImage"/> class.
        /// </summary>
        /// 
        /// <param name="width">The width of the image to be created.</param>
        /// <param name="height">The height of the image to be created.</param>
        /// 
        public ArrayToImage(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.Min = 0;
            this.Max = 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayToImage"/> class.
        /// </summary>
        /// 
        /// <param name="width">The width of the image to be created.</param>
        /// <param name="height">The height of the image to be created.</param>
        /// <param name="min">
        ///   The minimum double value in the double array
        ///   associated with the darkest color. Default is 0.
        /// </param>
        /// <param name="max">
        ///   The maximum double value in the double array
        ///   associated with the brightest color. Default is 1.
        /// </param>
        /// 
        public ArrayToImage(int width, int height, double min, double max)
        {
            this.Width = width;
            this.Height = height;
            this.Min = min;
            this.Max = max;
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(double[] input, out Bitmap output)
        {
            output = AForge.Imaging.Image.CreateGrayscaleImage(Width, Height);

            BitmapData data = output.LockBits(new Rectangle(0, 0, Width, Height),
                ImageLockMode.WriteOnly, output.PixelFormat);

            int offset = data.Stride - Width;
            int src = 0;

            unsafe
            {
                byte* dst = (byte*)data.Scan0.ToPointer();

                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++, src++, dst++)
                    {
                        *dst = (byte)Accord.Math.Tools.Scale(Min, Max, 0, 255, input[src]);
                    }
                    dst += offset;
                }
            }

            output.UnlockBits(data);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(float[] input, out Bitmap output)
        {
            output = AForge.Imaging.Image.CreateGrayscaleImage(Width, Height);

            BitmapData data = output.LockBits(new Rectangle(0, 0, Width, Height),
                ImageLockMode.WriteOnly, output.PixelFormat);

            int offset = data.Stride - Width;
            int src = 0;

            float min = (float)Min;
            float max = (float)Max;

            unsafe
            {
                byte* dst = (byte*)data.Scan0.ToPointer();

                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++, src++, dst++)
                    {
                        *dst = (byte)Accord.Math.Tools.Scale(min, max, 0, 255, input[src]);
                    }
                    dst += offset;
                }
            }

            output.UnlockBits(data);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        ///   For byte transformations, the Min and Max properties
        ///   are ignored.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(byte[] input, out Bitmap output)
        {
            output = AForge.Imaging.Image.CreateGrayscaleImage(Width, Height);

            BitmapData data = output.LockBits(new Rectangle(0, 0, Width, Height),
                ImageLockMode.WriteOnly, output.PixelFormat);

            int offset = data.Stride - Width;
            int src = 0;

            unsafe
            {
                byte* dst = (byte*)data.Scan0.ToPointer();

                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++, src++, dst++)
                    {
                        *dst = input[src];
                    }
                    dst += offset;
                }
            }

            output.UnlockBits(data);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(double[] input, out UnmanagedImage output)
        {
            Bitmap bitmap;
            Convert(input, out bitmap);
            output = UnmanagedImage.FromManagedImage(bitmap);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(float[] input, out UnmanagedImage output)
        {
            Bitmap bitmap;
            Convert(input, out bitmap);
            output = UnmanagedImage.FromManagedImage(bitmap);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        ///   For byte transformations, the Min and Max properties
        ///   are ignored.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(byte[] input, out UnmanagedImage output)
        {
            Bitmap bitmap;
            Convert(input, out bitmap);
            output = UnmanagedImage.FromManagedImage(bitmap);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(double[][] input, out Bitmap output)
        {
            PixelFormat format;
            int channels = input[0].Length;

            switch (channels)
            {
                case 1:
                    format = PixelFormat.Format8bppIndexed;
                    break;

                case 3:
                    format = PixelFormat.Format24bppRgb;
                    break;

                case 4:
                    format = PixelFormat.Format32bppArgb;
                    break;

                default:
                    throw new ArgumentException("Unsupported image pixel format.", "input");
            }


            output = new Bitmap(Width, Height, format);

            BitmapData data = output.LockBits(new Rectangle(0, 0, Width, Height),
                ImageLockMode.WriteOnly, format);

            int pixelSize = System.Drawing.Image.GetPixelFormatSize(format) / 8;
            int offset = data.Stride - Width * pixelSize;
            int src = 0;

            unsafe
            {
                byte* dst = (byte*)data.Scan0.ToPointer();

                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++, src++)
                    {
                        for (int c = channels - 1; c >= 0; c--, dst++)
                        {
                            *dst = (byte)Accord.Math.Tools.Scale(Min, Max, 0, 255, input[src][c]);
                        }
                    }
                    dst += offset;
                }
            }

            output.UnlockBits(data);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(float[][] input, out Bitmap output)
        {
            PixelFormat format;
            int channels = input[0].Length;

            switch (channels)
            {
                case 1:
                    format = PixelFormat.Format8bppIndexed;
                    break;

                case 3:
                    format = PixelFormat.Format24bppRgb;
                    break;

                case 4:
                    format = PixelFormat.Format32bppArgb;
                    break;

                default:
                    throw new ArgumentException("Unsupported image pixel format.", "input");
            }


            output = new Bitmap(Width, Height, format);

            BitmapData data = output.LockBits(new Rectangle(0, 0, Width, Height),
                ImageLockMode.WriteOnly, format);

            int pixelSize = System.Drawing.Image.GetPixelFormatSize(format) / 8;
            int offset = data.Stride - Width * pixelSize;
            int src = 0;

            float min = (float)Min;
            float max = (float)Max;

            unsafe
            {
                byte* dst = (byte*)data.Scan0.ToPointer();

                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++, src++)
                    {
                        for (int c = channels - 1; c >= 0; c--, dst++)
                        {
                            *dst = (byte)Accord.Math.Tools.Scale(min, max, 0, 255, input[src][c]);
                        }
                    }
                    dst += offset;
                }
            }

            output.UnlockBits(data);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        ///   For byte transformations, the Min and Max properties
        ///   are ignored.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(byte[][] input, out Bitmap output)
        {
            PixelFormat format;
            int channels = input[0].Length;

            switch (channels)
            {
                case 1:
                    format = PixelFormat.Format8bppIndexed;
                    break;

                case 3:
                    format = PixelFormat.Format24bppRgb;
                    break;

                case 4:
                    format = PixelFormat.Format32bppArgb;
                    break;

                default:
                    throw new ArgumentException("Unsupported image pixel format.", "input");
            }


            output = new Bitmap(Width, Height, format);

            BitmapData data = output.LockBits(new Rectangle(0, 0, Width, Height),
                ImageLockMode.WriteOnly, format);

            int pixelSize = System.Drawing.Image.GetPixelFormatSize(format) / 8;
            int offset = data.Stride - Width * pixelSize;
            int src = 0;

            unsafe
            {
                byte* dst = (byte*)data.Scan0.ToPointer();

                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++, src++)
                    {
                        for (int c = channels - 1; c >= 0; c--, dst++)
                        {
                            *dst = input[src][c];
                        }
                    }
                    dst += offset;
                }
            }

            output.UnlockBits(data);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        ///   For byte transformations, the Min and Max properties are ignored. The 
        ///   resulting image from upon calling this method will always be <see cref=
        ///   "PixelFormat.Format32bppArgb">32-bit ARGB</see>.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(Color[] input, out Bitmap output)
        {
            PixelFormat format = PixelFormat.Format32bppArgb;
            output = new Bitmap(Width, Height, format);

            BitmapData data = output.LockBits(new Rectangle(0, 0, Width, Height),
                ImageLockMode.WriteOnly, format);

            int pixelSize = System.Drawing.Image.GetPixelFormatSize(format) / 8;
            int offset = data.Stride - Width * pixelSize;
            int src = 0;

            unsafe
            {
                byte* dst = (byte*)data.Scan0.ToPointer();

                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++, src++, dst += pixelSize)
                    {
                        dst[RGB.A] = input[src].A;
                        dst[RGB.R] = input[src].R;
                        dst[RGB.G] = input[src].G;
                        dst[RGB.B] = input[src].B;
                    }

                    dst += offset;
                }
            }

            output.UnlockBits(data);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(double[][] input, out UnmanagedImage output)
        {
            Bitmap bitmap;
            Convert(input, out bitmap);
            output = UnmanagedImage.FromManagedImage(bitmap);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(float[][] input, out UnmanagedImage output)
        {
            Bitmap bitmap;
            Convert(input, out bitmap);
            output = UnmanagedImage.FromManagedImage(bitmap);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        ///   For byte transformations, the Min and Max properties
        ///   are ignored.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(byte[][] input, out UnmanagedImage output)
        {
            Bitmap bitmap;
            Convert(input, out bitmap);
            output = UnmanagedImage.FromManagedImage(bitmap);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(Color[] input, out UnmanagedImage output)
        {
            Bitmap bitmap;
            Convert(input, out bitmap);
            output = UnmanagedImage.FromManagedImage(bitmap);
        }

    }
}
