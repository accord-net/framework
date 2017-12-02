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
    using System;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    /// <summary>
    ///   Jagged array to BitmapSource converter.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This class can convert double and float arrays to either Grayscale
    ///   or color WPF images. Color images should be represented as an
    ///   array of pixel values for the final image. The actual dimensions
    ///   of the image should be specified in the class constructor.</para>
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
    /// // Create the converter to create a BitmapSource from the array
    /// ArrayToImageSource conv = new ArrayToImageSource(width: 4, height: 4);
    ///
    /// // Declare an image and store the pixels on it
    /// BitmapSource image; conv.Convert(pixels, out image);
    /// </code>
    /// 
    /// <para>
    ///   The resulting image is shown below.</para>
    ///   
    /// <img src="..\images\matrix-to-image.png" />
    /// </example>
    /// 
    public class ArrayToBitmapSource :
        IConverter<double[], BitmapSource>,
        IConverter<double[][], BitmapSource>,
        IConverter<float[], BitmapSource>,
        IConverter<float[][], BitmapSource>,
        IConverter<byte[], BitmapSource>,
        IConverter<byte[][], BitmapSource>,
        IConverter<Color[], BitmapSource>
    {

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
        ///   Gets or sets the horizontal DPI of the image stored in the double array. Default is 96.
        /// </summary>
        /// 
        public int DpiX { get; set; }

        /// <summary>
        ///   Gets or sets the vertical DPI of the image stored in the double array. Default is 96.
        /// </summary>
        /// 
        public int DpiY { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ArrayToBitmapSource"/> class.
        /// </summary>
        /// 
        /// <param name="width">The width of the image to be created.</param>
        /// <param name="height">The height of the image to be created.</param>
        /// 
        public ArrayToBitmapSource(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.DpiX = 96;
            this.DpiY = 96;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayToBitmapSource"/> class.
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
        public ArrayToBitmapSource(int width, int height, double min, double max)
        {
            this.Width = width;
            this.Height = height;
            this.DpiX = 96;
            this.DpiY = 96;
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(double[] input, out BitmapSource output)
        {
            Convert(input.ToSingle(), out output);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(double[][] input, out BitmapSource output)
        {
            Convert(input.ToSingle(), out output);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(float[] input, out BitmapSource output)
        {
            output = create(input, PixelFormats.Gray32Float, null);
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
        public void Convert(byte[] input, out BitmapSource output)
        {
            output = create(input, PixelFormats.Gray8, BitmapPalettes.Gray256);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(float[][] input, out BitmapSource output)
        {
            int channels = input[0].Length;

            PixelFormat format;
            BitmapPalette palette = null;
            switch (channels)
            {
                case 1:
                    format = PixelFormats.Gray32Float;
                    break;

                case 3:
                    format = PixelFormats.Rgb128Float;
                    break;

                case 4:
                    format = PixelFormats.Rgba128Float;
                    break;

                default:
                    throw new ArgumentException("Unsupported image pixel format.", "input");
            }

            output = create(input.ToMatrix(), format, palette);
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
        public void Convert(byte[][] input, out BitmapSource output)
        {
            int channels = input[0].Length;

            BitmapPalette palette = null;
            PixelFormat format;
            switch (channels)
            {
                case 1:
                    format = PixelFormats.Gray8;
                    palette = BitmapPalettes.Gray256;
                    break;

                case 3:
                    format = PixelFormats.Bgr24;
                    break;

                case 4:
                    format = PixelFormats.Bgra32;
                    break;

                default:
                    throw new ArgumentException("Unsupported image pixel format.", "input");
            }

            output = create(input.ToMatrix(), format, palette);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(Color[] input, out BitmapSource output)
        {
            var buffer = new Int32[input.Length];

            unsafe
            {
                fixed (int* ptr = buffer)
                {
                    int src = 0;
                    byte* dst = (byte*)ptr;

                    for (int y = 0; y < Height; y++)
                    {
                        for (int x = 0; x < Width; x++, src++, dst += 4)
                        {
                            dst[0] = input[src].R;
                            dst[1] = input[src].G;
                            dst[2] = input[src].B;
                            dst[3] = input[src].A;
                        }
                    }
                }
            }

            output = create(buffer, PixelFormats.Bgra32, null);
        }

        private BitmapSource create(Array input, System.Windows.Media.PixelFormat format, BitmapPalette palette)
        {
            int stride = input.GetNumberOfBytes() / Height;
            return BitmapSource.Create(Width, Height, DpiX, DpiY, format, palette, input, stride);
        }
    }
}
