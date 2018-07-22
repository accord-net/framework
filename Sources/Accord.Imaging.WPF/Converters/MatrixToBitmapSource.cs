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
    using Accord.Imaging;
    using Accord.Math;
    using System;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    /// <summary>
    ///   Multidimensional array to BitmapSource converter.
    /// </summary>
    /// 
    /// <remarks>
    ///   This class can convert double and float multidimensional arrays
    ///   (matrices) to Grayscale BitmapSource. The color representation of the
    ///   values contained in the matrices must be specified through the 
    ///   Min and Max properties of the class or class constructor.
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   This example converts a multidimensional array of double-precision
    ///   floating-point numbers with values from 0 to 1 into a grayscale image.</para>
    ///   
    /// <code>
    /// // Create a matrix representation 
    /// // of a 4x4 image with a inner 2x2
    /// // square drawn in the middle
    /// 
    /// double[,] pixels = 
    /// {
    ///      { 0, 0, 0, 0 },
    ///      { 0, 1, 1, 0 },
    ///      { 0, 1, 1, 0 },
    ///      { 0, 0, 0, 0 },
    /// };
    /// 
    /// // Create the converter to convert the matrix to a image
    /// var conv = new MatrixTobitmapSource(min: 0, max: 1);
    /// 
    /// // Declare a bitmap source and store the pixels on it
    /// BitmapSource image; conv.Convert(pixels, out image);
    /// </code>
    /// 
    /// <para>
    ///   The resulting image is shown below.</para>
    ///   
    /// <img src="..\images\matrix-to-image.png" />
    /// 
    /// </example>
    /// 
    public class MatrixToBitmapSource :
        IConverter<double[,], BitmapSource>,
        IConverter<float[,], BitmapSource>,
        IConverter<byte[,], BitmapSource>,
        IConverter<int[,], BitmapSource>,
        IConverter<short[,], BitmapSource>,
        IConverter<double[][], BitmapSource>,
        IConverter<float[][], BitmapSource>,
        IConverter<byte[][], BitmapSource>
    {

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
        ///   Gets or sets the desired output format of the image.
        /// </summary>
        /// 
        public PixelFormat Format { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MatrixToImage"/> class.
        /// </summary>
        /// 
        public MatrixToBitmapSource()
        {
            this.Format = PixelFormats.Gray8;
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
        public void Convert(double[,] input, out BitmapSource output)
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
            Convert(input.ToMatrix(), out output);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(float[,] input, out BitmapSource output)
        {
            output = create(input, PixelFormats.Gray32Float, null);
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
            Convert(input.ToMatrix(), out output);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(byte[,] input, out BitmapSource output)
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
        public void Convert(byte[][] input, out BitmapSource output)
        {
            Convert(input.ToMatrix(), out output);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(int[,] input, out BitmapSource output)
        {
            output = create(input, Format, null);
        }


        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(int[][] input, out BitmapSource output)
        {
            Convert(input.ToMatrix(), out output);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(short[,] input, out BitmapSource output)
        {
            output = create(input, Format, null);
        }


        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(short[][] input, out BitmapSource output)
        {
            Convert(input.ToMatrix(), out output);
        }

        private BitmapSource create(Array input, System.Windows.Media.PixelFormat format, BitmapPalette palette)
        {
            int width = input.GetLength(0);
            int height = input.GetLength(1);

            GCHandle handle = GCHandle.Alloc(input, GCHandleType.Pinned);
            IntPtr buffer = handle.AddrOfPinnedObject();
            int bufferSize = input.GetNumberOfBytes();
            int stride = bufferSize / height;

            BitmapSource bmp = BitmapSource.Create(width, height, DpiX, DpiY, format, palette, buffer, bufferSize, stride);

            handle.Free();

            return bmp;
        }
    }
}
