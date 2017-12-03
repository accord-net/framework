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
    using System.Drawing;
    using System.Drawing.Imaging;
    using Accord.Imaging;
    using Accord.Math;
    using System;

    /// <summary>
    ///   Multidimensional array to Bitmap converter.
    /// </summary>
    /// 
    /// <remarks>
    ///   This class can convert double and float multidimensional arrays
    ///   (matrices) to Grayscale bitmaps. The color representation of the
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
    /// MatrixToImage conv = new MatrixToImage(min: 0, max: 1);
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
    public class MatrixToImage :
        IConverter<double[,], Bitmap>,
        IConverter<double[,], UnmanagedImage>,
        IConverter<float[,], Bitmap>,
        IConverter<float[,], UnmanagedImage>,
        IConverter<byte[,], Bitmap>,
        IConverter<byte[,], UnmanagedImage>,
        IConverter<int[,], Bitmap>,
        IConverter<int[,], UnmanagedImage>,
        IConverter<short[,], Bitmap>,
        IConverter<short[,], UnmanagedImage>,
        IConverter<double[][], Bitmap>,
        IConverter<double[][], UnmanagedImage>,
        IConverter<float[][], Bitmap>,
        IConverter<float[][], UnmanagedImage>,
        IConverter<byte[][], Bitmap>,
        IConverter<byte[][], UnmanagedImage>,
        IConverter<byte[][][], Bitmap>,
        IConverter<byte[][][], UnmanagedImage>,
        IConverter<byte[,,], Bitmap>,
        IConverter<byte[,,], UnmanagedImage>
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
        ///   Gets or sets the desired output format of the image.
        /// </summary>
        /// 
        public PixelFormat Format { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MatrixToImage"/> class.
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
        ///   
        public MatrixToImage(double min, double max)
        {
            this.Min = min;
            this.Max = max;
            this.Format = PixelFormat.Format8bppIndexed;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MatrixToImage"/> class.
        /// </summary>
        /// 
        public MatrixToImage()
            : this(0, 1)
        {
        }


        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(double[,] input, out UnmanagedImage output)
        {
            Bitmap image;

            Convert(input, out image);

            output = UnmanagedImage.FromManagedImage(image);
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
            Bitmap image;

            Convert(input, out image);

            output = UnmanagedImage.FromManagedImage(image);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(float[,] input, out UnmanagedImage output)
        {
            Bitmap image;

            Convert(input, out image);

            output = UnmanagedImage.FromManagedImage(image);
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
            Bitmap image;

            Convert(input, out image);

            output = UnmanagedImage.FromManagedImage(image);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(byte[,] input, out UnmanagedImage output)
        {
            Bitmap image;

            Convert(input, out image);

            output = UnmanagedImage.FromManagedImage(image);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(byte[][] input, out UnmanagedImage output)
        {
            Bitmap image;

            Convert(input, out image);

            output = UnmanagedImage.FromManagedImage(image);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(byte[,,] input, out UnmanagedImage output)
        {
            Bitmap image;

            Convert(input, out image);

            output = UnmanagedImage.FromManagedImage(image);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(byte[][][] input, out UnmanagedImage output)
        {
            Bitmap image;

            Convert(input, out image);

            output = UnmanagedImage.FromManagedImage(image);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(int[,] input, out UnmanagedImage output)
        {
            Bitmap image;

            Convert(input, out image);

            output = UnmanagedImage.FromManagedImage(image);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(int[][] input, out UnmanagedImage output)
        {
            Bitmap image;

            Convert(input, out image);

            output = UnmanagedImage.FromManagedImage(image);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(short[,] input, out UnmanagedImage output)
        {
            Bitmap image;

            Convert(input, out image);

            output = UnmanagedImage.FromManagedImage(image);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(short[][] input, out UnmanagedImage output)
        {
            Bitmap image;

            Convert(input, out image);

            output = UnmanagedImage.FromManagedImage(image);
        }

        /// <summary>
        ///   Converts an image from one representation to another.
        /// </summary>
        /// 
        /// <param name="input">The input image to be converted.</param>
        /// <param name="output">The converted image.</param>
        /// 
        public void Convert(double[,] input, out Bitmap output)
        {
            int width = input.GetLength(1);
            int height = input.GetLength(0);

            if (Format == PixelFormat.Format8bppIndexed)
                output = Accord.Imaging.Image.CreateGrayscaleImage(width, height);
            else output = new Bitmap(width, height, Format);

            BitmapData data = output.LockBits(ImageLockMode.WriteOnly);

            int pixelSize = System.Drawing.Image.GetPixelFormatSize(Format) / 8;

            int offset = data.Stride - width * pixelSize;

            unsafe
            {
                byte* dst = (byte*)data.Scan0.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        double v = 255 * (input[y, x] - Min) / (Max - Min);
                        byte value = unchecked((byte)v);

                        for (int c = 0; c < pixelSize; c++, dst++)
                            *dst = value;
                    }

                    dst += offset;
                }

                if (pixelSize == 4)
                {
                    dst = (byte*)data.Scan0.ToPointer();

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, dst += pixelSize)
                            dst[RGB.A] = 0;
                        dst += offset;
                    }
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
        public void Convert(double[][] input, out Bitmap output)
        {
            int width = input.Columns();
            int height = input.Rows();

            if (Format == PixelFormat.Format8bppIndexed)
                output = Accord.Imaging.Image.CreateGrayscaleImage(width, height);
            else output = new Bitmap(width, height, Format);

            BitmapData data = output.LockBits(ImageLockMode.WriteOnly);

            int pixelSize = System.Drawing.Image.GetPixelFormatSize(Format) / 8;

            int offset = data.Stride - width * pixelSize;

            unsafe
            {
                byte* dst = (byte*)data.Scan0.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        double v = 255 * (input[y][x] - Min) / (Max - Min);
                        byte value = unchecked((byte)v);

                        for (int c = 0; c < pixelSize; c++, dst++)
                            *dst = value;
                    }

                    dst += offset;
                }

                if (pixelSize == 4)
                {
                    dst = (byte*)data.Scan0.ToPointer();

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, dst += pixelSize)
                            dst[RGB.A] = 0;
                        dst += offset;
                    }
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
        public void Convert(float[,] input, out Bitmap output)
        {
            int width = input.GetLength(1);
            int height = input.GetLength(0);

            if (Format == PixelFormat.Format8bppIndexed)
                output = Accord.Imaging.Image.CreateGrayscaleImage(width, height);
            else output = new Bitmap(width, height, Format);

            BitmapData data = output.LockBits(ImageLockMode.WriteOnly);

            int pixelSize = System.Drawing.Image.GetPixelFormatSize(Format) / 8;

            int offset = data.Stride - width * pixelSize;

            float min = (float)Min;
            float max = (float)Max;

            unsafe
            {
                byte* dst = (byte*)data.Scan0.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte value = (byte)Vector.Scale(input[y, x], Min, Max, (byte)0, (byte)255);

                        for (int c = 0; c < pixelSize; c++, dst++)
                            *dst = value;
                    }

                    dst += offset;
                }

                if (pixelSize == 4)
                {
                    dst = (byte*)data.Scan0.ToPointer();

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, dst += pixelSize)
                            dst[RGB.A] = 0;
                        dst += offset;
                    }
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
            int width = input.Columns();
            int height = input.Rows();

            if (Format == PixelFormat.Format8bppIndexed)
                output = Accord.Imaging.Image.CreateGrayscaleImage(width, height);
            else output = new Bitmap(width, height, Format);

            BitmapData data = output.LockBits(ImageLockMode.WriteOnly);

            int pixelSize = System.Drawing.Image.GetPixelFormatSize(Format) / 8;

            int offset = data.Stride - width * pixelSize;

            float min = (float)Min;
            float max = (float)Max;

            unsafe
            {
                byte* dst = (byte*)data.Scan0.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte value = (byte)Vector.Scale(input[y][x], Min, Max, (byte)0, (byte)255);

                        for (int c = 0; c < pixelSize; c++, dst++)
                            *dst = value;
                    }

                    dst += offset;
                }

                if (pixelSize == 4)
                {
                    dst = (byte*)data.Scan0.ToPointer();

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, dst += pixelSize)
                            dst[RGB.A] = 0;
                        dst += offset;
                    }
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
        public void Convert(byte[,] input, out Bitmap output)
        {
            int width = input.GetLength(1);
            int height = input.GetLength(0);

            if (Format == PixelFormat.Format8bppIndexed)
                output = Accord.Imaging.Image.CreateGrayscaleImage(width, height);
            else output = new Bitmap(width, height, Format);

            int pixelSize = System.Drawing.Image.GetPixelFormatSize(Format) / 8;

            BitmapData data = output.LockBits(ImageLockMode.WriteOnly);

            int offset = data.Stride - width * pixelSize;

            unsafe
            {
                byte* dst = (byte*)data.Scan0.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte value = input[y, x];

                        for (int c = 0; c < pixelSize; c++, dst++)
                            *dst = value;
                    }

                    dst += offset;
                }

                if (pixelSize == 4)
                {
                    dst = (byte*)data.Scan0.ToPointer();

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, dst += pixelSize)
                            dst[RGB.A] = 0;
                        dst += offset;
                    }
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
        public void Convert(byte[,,] input, out Bitmap output)
        {
            int width = input.Columns();
            int height = input.Rows();
            int depth = input.Depth();

            if (depth == 1)
                output = Accord.Imaging.Image.CreateGrayscaleImage(width, height);
            else if (depth == 3)
                output = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            else if (depth == 4)
                output = new Bitmap(width, height, PixelFormat.Format32bppRgb);
            else throw new ArgumentException("input");

            int pixelSize = output.GetPixelFormatSizeInBytes();
            BitmapData data = output.LockBits(ImageLockMode.WriteOnly);
            int offset = data.Stride - width * pixelSize;

            unsafe
            {
                byte* dst = (byte*)data.Scan0.ToPointer();
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                        for (int c = 0; c < depth; c++, dst++)
                            *dst = input[y, x, c];
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
        public void Convert(byte[][] input, out Bitmap output)
        {
            int width = input.Columns();
            int height = input.Rows();

            if (Format == PixelFormat.Format8bppIndexed)
                output = Accord.Imaging.Image.CreateGrayscaleImage(width, height);
            else output = new Bitmap(width, height, Format);

            int pixelSize = System.Drawing.Image.GetPixelFormatSize(Format) / 8;

            BitmapData data = output.LockBits(ImageLockMode.WriteOnly);

            int offset = data.Stride - width * pixelSize;

            unsafe
            {
                byte* dst = (byte*)data.Scan0.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte value = input[y][x];

                        for (int c = 0; c < pixelSize; c++, dst++)
                            *dst = value;
                    }

                    dst += offset;
                }

                if (pixelSize == 4)
                {
                    dst = (byte*)data.Scan0.ToPointer();

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, dst += pixelSize)
                            dst[RGB.A] = 0;
                        dst += offset;
                    }
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
        public void Convert(byte[][][] input, out Bitmap output)
        {
            int width = input.Columns();
            int height = input.Rows();
            int depth = input.Depth();

            if (depth == 1)
                output = Accord.Imaging.Image.CreateGrayscaleImage(width, height);
            else if (depth == 3)
                output = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            else if (depth == 4)
                output = new Bitmap(width, height, PixelFormat.Format32bppRgb);
            else throw new ArgumentException("input");

            int pixelSize = output.GetPixelFormatSizeInBytes();
            BitmapData data = output.LockBits(ImageLockMode.WriteOnly);
            int offset = data.Stride - width * pixelSize;

            unsafe
            {
                byte* dst = (byte*)data.Scan0.ToPointer();
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                        for (int c = 0; c < depth; c++, dst++)
                            *dst = input[y][x][c];
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
        public void Convert(int[,] input, out Bitmap output)
        {
            int width = input.GetLength(1);
            int height = input.GetLength(0);

            if (Format == PixelFormat.Format8bppIndexed)
                output = Accord.Imaging.Image.CreateGrayscaleImage(width, height);
            else output = new Bitmap(width, height, Format);

            int pixelSize = System.Drawing.Image.GetPixelFormatSize(Format) / 8;

            BitmapData data = output.LockBits(ImageLockMode.WriteOnly);

            int offset = data.Stride - width * pixelSize;

            unsafe
            {
                byte* dst = (byte*)data.Scan0.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte value = (byte)input[y, x];

                        for (int c = 0; c < pixelSize; c++, dst++)
                            *dst = value;
                    }

                    dst += offset;
                }

                if (pixelSize == 4)
                {
                    dst = (byte*)data.Scan0.ToPointer();

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, dst += pixelSize)
                            dst[RGB.A] = 0;
                        dst += offset;
                    }
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
        public void Convert(int[][] input, out Bitmap output)
        {
            int width = input.Columns();
            int height = input.Rows();

            if (Format == PixelFormat.Format8bppIndexed)
                output = Accord.Imaging.Image.CreateGrayscaleImage(width, height);
            else output = new Bitmap(width, height, Format);

            int pixelSize = System.Drawing.Image.GetPixelFormatSize(Format) / 8;

            BitmapData data = output.LockBits(ImageLockMode.WriteOnly);

            int offset = data.Stride - width * pixelSize;

            unsafe
            {
                byte* dst = (byte*)data.Scan0.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte value = (byte)input[y][x];

                        for (int c = 0; c < pixelSize; c++, dst++)
                            *dst = value;
                    }

                    dst += offset;
                }

                if (pixelSize == 4)
                {
                    dst = (byte*)data.Scan0.ToPointer();

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, dst += pixelSize)
                            dst[RGB.A] = 0;
                        dst += offset;
                    }
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
        public void Convert(short[,] input, out Bitmap output)
        {
            int width = input.GetLength(1);
            int height = input.GetLength(0);

            if (Format == PixelFormat.Format8bppIndexed)
                output = Accord.Imaging.Image.CreateGrayscaleImage(width, height);
            else output = new Bitmap(width, height, Format);

            int pixelSize = System.Drawing.Image.GetPixelFormatSize(Format) / 8;

            BitmapData data = output.LockBits(ImageLockMode.WriteOnly);

            int offset = data.Stride - width * pixelSize;

            unsafe
            {
                byte* dst = (byte*)data.Scan0.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte value = (byte)input[y, x];

                        for (int c = 0; c < pixelSize; c++, dst++)
                            *dst = value;
                    }

                    dst += offset;
                }

                if (pixelSize == 4)
                {
                    dst = (byte*)data.Scan0.ToPointer();

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, dst += pixelSize)
                            dst[RGB.A] = 0;
                        dst += offset;
                    }
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
        public void Convert(short[][] input, out Bitmap output)
        {
            int width = input.Columns();
            int height = input.Rows();

            if (Format == PixelFormat.Format8bppIndexed)
                output = Accord.Imaging.Image.CreateGrayscaleImage(width, height);
            else output = new Bitmap(width, height, Format);

            int pixelSize = System.Drawing.Image.GetPixelFormatSize(Format) / 8;

            BitmapData data = output.LockBits(ImageLockMode.WriteOnly);

            int offset = data.Stride - width * pixelSize;

            unsafe
            {
                byte* dst = (byte*)data.Scan0.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte value = (byte)input[y][x];

                        for (int c = 0; c < pixelSize; c++, dst++)
                            *dst = value;
                    }

                    dst += offset;
                }

                if (pixelSize == 4)
                {
                    dst = (byte*)data.Scan0.ToPointer();

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, dst += pixelSize)
                            dst[RGB.A] = 0;
                        dst += offset;
                    }
                }
            }

            output.UnlockBits(data);
        }

    }
}
