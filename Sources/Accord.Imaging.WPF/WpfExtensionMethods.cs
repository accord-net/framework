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
// Some functions adapted from the original work of Peter Kovesi,
// shared under a permissive MIT license. Details are given below:
//
//   Copyright (c) 1995-2010 Peter Kovesi
//   Centre for Exploration Targeting
//   School of Earth and Environment
//   The University of Western Australia
//
//   Permission is hereby granted, free of charge, to any person obtaining a copy
//   of this software and associated documentation files (the "Software"), to deal
//   in the Software without restriction, including without limitation the rights 
//   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
//   of the Software, and to permit persons to whom the Software is furnished to do
//   so, subject to the following conditions:
//   
//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.
//   
//   The software is provided "as is", without warranty of any kind, express or
//   implied, including but not limited to the warranties of merchantability, 
//   fitness for a particular purpose and noninfringement. In no event shall the
//   authors or copyright holders be liable for any claim, damages or other liability,
//   whether in an action of contract, tort or otherwise, arising from, out of or in
//   connection with the software or the use or other dealings in the software.
//   

namespace Accord.Imaging
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Windows.Media.Imaging;
    using System.Windows.Media;
    using System.Windows;
    using Accord.Imaging.Converters;
    using Accord.Imaging.Filters;
    using System.Runtime.InteropServices;
    using Accord.Math;

    /// <summary>
    ///   Extension methods for WPF-format images (BitmapSource).
    /// </summary>
    /// 
    public static class WpfExtensionMethods
    {
        #region ToBitmapSource
        /// <summary>
        ///   Converts an image given as a matrix of pixel values into a <see cref="BitmapSource"/>.
        ///   For more options, please use the <see cref="MatrixToBitmapSource"/> class.
        /// </summary>
        /// 
        /// <param name="pixels">A matrix containing the grayscale pixel
        /// values as <see cref="System.Double">bytes</see>.</param>
        /// <returns>A <see cref="BitmapSource"/> of the same width and height as the pixel matrix containing the given pixel values.</returns>
        /// 
        /// <seealso cref="MatrixToBitmapSource"/>
        /// 
        public static BitmapSource ToBitmapSource(this byte[,] pixels)
        {
            BitmapSource bitmap;
            new MatrixToBitmapSource().Convert(pixels, out bitmap);
            return bitmap;
        }

        /// <summary>
        ///   Converts an image given as a matrix of pixel values into a <see cref="BitmapSource"/>.
        ///   For more options, please use the <see cref="MatrixToBitmapSource"/> class.
        /// </summary>
        /// 
        /// <param name="pixels">A matrix containing the grayscale pixel
        /// values as <see cref="System.Double">bytes</see>.</param>
        /// <returns>A <see cref="BitmapSource"/> of the same width and height as the pixel matrix containing the given pixel values.</returns>
        /// 
        /// <seealso cref="MatrixToBitmapSource"/>
        /// 
        public static BitmapSource ToBitmapSource(this short[,] pixels)
        {
            BitmapSource bitmap;
            new MatrixToBitmapSource().Convert(pixels, out bitmap);
            return bitmap;
        }

        /// <summary>
        ///   Converts an image given as a matrix of pixel values into a <see cref="BitmapSource"/>.
        ///   For more options, please use the <see cref="MatrixToBitmapSource"/> class.
        /// </summary>
        /// 
        /// <param name="pixels">A matrix containing the grayscale pixel
        /// values as <see cref="System.Double">bytes</see>.</param>
        /// <returns>A <see cref="BitmapSource"/> of the same width and height as the pixel matrix containing the given pixel values.</returns>
        /// 
        /// <seealso cref="MatrixToBitmapSource"/>
        /// 
        public static BitmapSource ToBitmapSource(this double[,] pixels)
        {
            BitmapSource bitmap;
            new MatrixToBitmapSource().Convert(pixels, out bitmap);
            return bitmap;
        }

        /// <summary>
        ///   Converts an image given as a matrix of pixel values into a <see cref="BitmapSource"/>.
        ///   For more options, please use the <see cref="MatrixToBitmapSource"/> class.
        /// </summary>
        /// 
        /// <param name="pixels">A matrix containing the grayscale pixel
        /// values as <see cref="System.Double">bytes</see>.</param>
        /// <returns>A <see cref="BitmapSource"/> of the same width and height as the pixel matrix containing the given pixel values.</returns>
        /// 
        /// <seealso cref="MatrixToBitmapSource"/>
        /// 
        public static BitmapSource ToBitmapSource(this int[,] pixels)
        {
            BitmapSource bitmap;
            new MatrixToBitmapSource().Convert(pixels, out bitmap);
            return bitmap;
        }

        /// <summary>
        ///   Converts an image given as a matrix of pixel values into a <see cref="BitmapSource"/>.
        ///   For more options, please use the <see cref="MatrixToBitmapSource"/> class.
        /// </summary>
        /// 
        /// <param name="pixels">A matrix containing the grayscale pixel
        /// values as <see cref="System.Double">bytes</see>.</param>
        /// <returns>A <see cref="BitmapSource"/> of the same width and height as the pixel matrix containing the given pixel values.</returns>
        /// 
        /// <seealso cref="MatrixToBitmapSource"/>
        /// 
        public static BitmapSource ToBitmapSource(this float[,] pixels)
        {
            BitmapSource bitmap;
            new MatrixToBitmapSource().Convert(pixels, out bitmap);
            return bitmap;
        }





        /// <summary>
        ///   Converts an image given as a matrix of pixel values into a <see cref="BitmapSource"/>.
        ///   For more options, please use the <see cref="MatrixToBitmapSource"/> class.
        /// </summary>
        /// 
        /// <param name="pixels">A matrix containing the grayscale pixel
        /// values as <see cref="System.Double">bytes</see>.</param>
        /// <param name="width">The width of the image to be created.</param>
        /// <param name="height">The height of the image to be created.</param>
        /// 
        /// <returns>A <see cref="BitmapSource"/> of the same width and height as the pixel matrix containing the given pixel values.</returns>
        /// 
        /// <seealso cref="MatrixToBitmapSource"/>
        /// 
        public static BitmapSource ToBitmapSource(this byte[][] pixels, int width, int height)
        {
            BitmapSource bitmap;
            new ArrayToBitmapSource(width, height).Convert(pixels, out bitmap);
            return bitmap;
        }

        /// <summary>
        ///   Converts an image given as a matrix of pixel values into a <see cref="BitmapSource"/>.
        ///   For more options, please use the <see cref="MatrixToBitmapSource"/> class.
        /// </summary>
        /// 
        /// <param name="pixels">A matrix containing the grayscale pixel
        /// values as <see cref="System.Double">bytes</see>.</param>
        /// <param name="width">The width of the image to be created.</param>
        /// <param name="height">The height of the image to be created.</param>
        /// 
        /// <returns>A <see cref="BitmapSource"/> of the same width and height as the pixel matrix containing the given pixel values.</returns>
        /// 
        /// <seealso cref="MatrixToBitmapSource"/>
        /// 
        public static BitmapSource ToBitmapSource(this double[][] pixels, int width, int height)
        {
            BitmapSource bitmap;
            new ArrayToBitmapSource(width, height).Convert(pixels, out bitmap);
            return bitmap;
        }

        /// <summary>
        ///   Converts an image given as a matrix of pixel values into a <see cref="BitmapSource"/>.
        ///   For more options, please use the <see cref="MatrixToBitmapSource"/> class.
        /// </summary>
        /// 
        /// <param name="pixels">A matrix containing the grayscale pixel
        /// values as <see cref="System.Double">bytes</see>.</param>
        /// <param name="width">The width of the image to be created.</param>
        /// <param name="height">The height of the image to be created.</param>
        /// 
        /// <returns>A <see cref="BitmapSource"/> of the same width and height as the pixel matrix containing the given pixel values.</returns>
        /// 
        /// <seealso cref="MatrixToBitmapSource"/>
        /// 
        public static BitmapSource ToBitmapSource(this float[][] pixels, int width, int height)
        {
            BitmapSource bitmap;
            new ArrayToBitmapSource(width, height).Convert(pixels, out bitmap);
            return bitmap;
        }



        /// <summary>
        ///   Converts an image given as a matrix of pixel values into a <see cref="BitmapSource"/>.
        ///   For more options, please use the <see cref="MatrixToBitmapSource"/> class.
        /// </summary>
        /// 
        /// <param name="pixels">A matrix containing the grayscale pixel
        /// values as <see cref="System.Double">bytes</see>.</param>
        /// <param name="width">The width of the image to be created.</param>
        /// <param name="height">The height of the image to be created.</param>
        /// 
        /// <returns>A <see cref="BitmapSource"/> of the same width and height as the pixel matrix containing the given pixel values.</returns>
        /// 
        /// <seealso cref="MatrixToBitmapSource"/>
        /// 
        public static BitmapSource ToBitmapSource(this byte[] pixels, int width, int height)
        {
            BitmapSource bitmap;
            new ArrayToBitmapSource(width, height).Convert(pixels, out bitmap);
            return bitmap;
        }

        /// <summary>
        ///   Converts an image given as a matrix of pixel values into a <see cref="BitmapSource"/>.
        ///   For more options, please use the <see cref="MatrixToBitmapSource"/> class.
        /// </summary>
        /// 
        /// <param name="pixels">A matrix containing the grayscale pixel
        /// values as <see cref="System.Double">bytes</see>.</param>
        /// <param name="width">The width of the image to be created.</param>
        /// <param name="height">The height of the image to be created.</param>
        /// 
        /// <returns>A <see cref="BitmapSource"/> of the same width and height as the pixel matrix containing the given pixel values.</returns>
        /// 
        /// <seealso cref="MatrixToBitmapSource"/>
        /// 
        public static BitmapSource ToBitmapSource(this double[] pixels, int width, int height)
        {
            BitmapSource bitmap;
            new ArrayToBitmapSource(width, height).Convert(pixels, out bitmap);
            return bitmap;
        }

        /// <summary>
        ///   Converts an image given as a matrix of pixel values into a <see cref="BitmapSource"/>.
        ///   For more options, please use the <see cref="MatrixToBitmapSource"/> class.
        /// </summary>
        /// 
        /// <param name="pixels">A matrix containing the grayscale pixel
        /// values as <see cref="System.Double">bytes</see>.</param>
        /// <param name="width">The width of the image to be created.</param>
        /// <param name="height">The height of the image to be created.</param>
        /// 
        /// <returns>A <see cref="BitmapSource"/> of the same width and height as the pixel matrix containing the given pixel values.</returns>
        /// 
        /// <seealso cref="MatrixToBitmapSource"/>
        /// 
        public static BitmapSource ToBitmapSource(this float[] pixels, int width, int height)
        {
            BitmapSource bitmap;
            new ArrayToBitmapSource(width, height).Convert(pixels, out bitmap);
            return bitmap;
        }
        #endregion




        #region ToMatrix
        /// <summary>
        ///   Converts an image given as a <see cref="System.Drawing.Bitmap"/> into a matrix of 
        ///   pixel values.For more options, please use the <see cref="MatrixToImage"/> class.
        /// </summary>
        /// 
        /// <param name="bitmap">A image represented as a bitmap.</param>
        /// 
        /// <returns>A matrix containing the values of each pixel in the bitmap.</returns>
        /// 
        /// <seealso cref="ImageToMatrix"/>
        /// 
        public static double[,,] ToMatrix(this BitmapSource bitmap)
        {
            double[,,] matrix;
            new BitmapSourceToMatrix().Convert(bitmap, out matrix);
            return matrix;
        }

        /// <summary>
        ///   Converts an image given as a <see cref="System.Drawing.Bitmap"/> into a matrix of 
        ///   pixel values.For more options, please use the <see cref="MatrixToImage"/> class.
        /// </summary>
        /// 
        /// <param name="bitmap">A image represented as a bitmap.</param>
        /// 
        /// <returns>A matrix containing the values of each pixel in the bitmap.</returns>
        /// 
        /// <seealso cref="ImageToMatrix"/>
        /// 
        public static double[][][] ToJagged(this BitmapSource bitmap)
        {
            double[][][] matrix;
            new BitmapSourceToMatrix().Convert(bitmap, out matrix);
            return matrix;
        }

        /// <summary>
        ///   Converts an image given as a <see cref="System.Drawing.Bitmap"/> into a matrix of 
        ///   pixel values.For more options, please use the <see cref="MatrixToImage"/> class.
        /// </summary>
        /// 
        /// <param name="bitmap">A image represented as a bitmap.</param>
        /// <param name="channel">The color channel to be extracted.</param>
        /// 
        /// <returns>A matrix containing the values of each pixel in the bitmap.</returns>
        /// 
        /// <seealso cref="ImageToMatrix"/>
        /// 
        public static double[,] ToMatrix(this BitmapSource bitmap, int channel)
        {
            double[,] matrix;
            new BitmapSourceToMatrix()
            {
                Channel = channel
            }.Convert(bitmap, out matrix);
            return matrix;
        }

        /// <summary>
        ///   Converts an image given as a <see cref="System.Drawing.Bitmap"/> into a matrix of 
        ///   pixel values.For more options, please use the <see cref="MatrixToImage"/> class.
        /// </summary>
        /// 
        /// <param name="bitmap">A image represented as a bitmap.</param>
        /// <param name="channel">The color channel to be extracted.</param>
        /// 
        /// <returns>A matrix containing the values of each pixel in the bitmap.</returns>
        /// 
        /// <seealso cref="ImageToMatrix"/>
        /// 
        public static double[][] ToJagged(this BitmapSource bitmap, int channel)
        {
            double[][] matrix;
            new BitmapSourceToMatrix()
            {
                Channel = channel
            }.Convert(bitmap, out matrix);
            return matrix;
        }
        #endregion



        /// <summary>
        ///   Converts a System.Drawing.Bitmap into a WPF's BitmapSource image.
        /// </summary>
        /// 
        /// <param name="bitmap">The bitmap to be converted.</param>
        /// 
        public static BitmapSource ToBitmapSource(this Bitmap bitmap)
        {
            var format = bitmap.PixelFormat.ToWPF();

            var bitmapData = bitmap.LockBits(ImageLockMode.ReadOnly);

            var bitmapSource = BitmapSource.Create(bitmapData.Width, bitmapData.Height,
                bitmap.HorizontalResolution, bitmap.VerticalResolution, format, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);
            return bitmapSource;
        }

        /// <summary>
        /// Check if specified palette is grayscale.
        /// </summary>
        /// 
        /// <param name="palette">Palette to check.</param>
        /// 
        public static bool IsGrayscale(this BitmapPalette palette)
        {
            int colors = palette.Colors.Count;
            for (int i = 0; i < palette.Colors.Count; i++)
            {
                System.Windows.Media.Color c = palette.Colors[i];

                int expectedGray = i % 256;

                if ((c.R != c.B) || (c.R != c.G))
                    return false;

                if (c.G != expectedGray)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Check if specified image is grayscale.
        /// </summary>
        /// 
        /// <param name="bitmapSource">Bitmap source to check.</param>
        /// 
        public static bool IsGrayscale(this BitmapSource bitmapSource)
        {
            if (bitmapSource.Format == PixelFormats.Gray16 ||
                bitmapSource.Format == PixelFormats.Gray2 ||
                bitmapSource.Format == PixelFormats.Gray32Float ||
                bitmapSource.Format == PixelFormats.Gray4 ||
                bitmapSource.Format == PixelFormats.Gray8)
                return true;
            if (bitmapSource.Format == PixelFormats.Indexed1 ||
                bitmapSource.Format == PixelFormats.Indexed2 ||
                bitmapSource.Format == PixelFormats.Indexed4 ||
                bitmapSource.Format == PixelFormats.Indexed8)
            {
                if (bitmapSource.Palette == BitmapPalettes.BlackAndWhite ||
                    bitmapSource.Palette == BitmapPalettes.BlackAndWhiteTransparent ||
                    bitmapSource.Palette == BitmapPalettes.Gray16 ||
                    bitmapSource.Palette == BitmapPalettes.Gray16Transparent ||
                    bitmapSource.Palette == BitmapPalettes.Gray256 ||
                    bitmapSource.Palette == BitmapPalettes.Gray256Transparent ||
                    bitmapSource.Palette == BitmapPalettes.Gray4 ||
                    bitmapSource.Palette == BitmapPalettes.Gray4Transparent)
                    return true;
                if (bitmapSource.Palette.IsGrayscale())
                    return true;
            }
            return false;
        }


        /// <summary>
        ///   Converts a WPF's BitmapSource into a System.Drawing.Bitmap image.
        /// </summary>
        /// 
        /// <param name="bitmapSource">The bitmap source to be converted.</param>
        /// 
        public static Bitmap ToBitmap(this BitmapSource bitmapSource)
        {
            var format = bitmapSource.Format.ToImaging();

            int width = bitmapSource.PixelWidth;
            int height = bitmapSource.PixelHeight;

            Bitmap bmp;
            if (bitmapSource.IsGrayscale())
            {
                if (bitmapSource.Format != PixelFormats.Gray8)
                    return new FormatConvertedBitmap(bitmapSource, PixelFormats.Gray8, BitmapPalettes.Gray256, 1.0).ToBitmap();

                bmp = Accord.Imaging.Image.CreateGrayscaleImage(width, height);
            }
            else
            {
                bmp = new Bitmap(width, height, format);
            }

            BitmapData data = bmp.LockBits(ImageLockMode.WriteOnly);

            bitmapSource.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bmp.UnlockBits(data);
            return bmp;
        }

        /// <summary>
        /// Creates unmanaged image from the specified managed image.
        /// </summary>
        /// 
        /// <param name="bitmap">Source managed image.</param>
        /// 
        /// <returns>Returns new unmanaged image, which is a copy of source managed image.</returns>
        /// 
        /// <remarks><para>The method creates an exact copy of specified managed image, but allocated
        /// in unmanaged memory.</para></remarks>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of source image.</exception>
        /// 
        public static UnmanagedImage ToUnmanagedImage(this BitmapSource bitmap)
        {
            var format = bitmap.Format;
            return bitmap.ToBitmap().ToUnmanagedImage();
        }

        /// <summary>
        ///   Converts the given System.Drawing.Imaging.PixelFormat to its WPF (System.Windows.Media) equivalent.
        /// </summary>
        /// 
        public static System.Windows.Media.PixelFormat ToWPF(this System.Drawing.Imaging.PixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case System.Drawing.Imaging.PixelFormat.Indexed:
                    break;
                case System.Drawing.Imaging.PixelFormat.Gdi:
                    break;
                case System.Drawing.Imaging.PixelFormat.Alpha:
                    break;
                case System.Drawing.Imaging.PixelFormat.PAlpha:
                    break;
                case System.Drawing.Imaging.PixelFormat.Extended:
                    break;
                case System.Drawing.Imaging.PixelFormat.Canonical:
                    break;
                case System.Drawing.Imaging.PixelFormat.Undefined:
                    break;
                case System.Drawing.Imaging.PixelFormat.Format1bppIndexed:
                    return PixelFormats.BlackWhite;
                case System.Drawing.Imaging.PixelFormat.Format4bppIndexed:
                    return PixelFormats.Gray4;
                case System.Drawing.Imaging.PixelFormat.Format8bppIndexed:
                    return PixelFormats.Gray8;
                case System.Drawing.Imaging.PixelFormat.Format16bppGrayScale:
                    break;
                case System.Drawing.Imaging.PixelFormat.Format16bppRgb555:
                    break;
                case System.Drawing.Imaging.PixelFormat.Format16bppRgb565:
                    break;
                case System.Drawing.Imaging.PixelFormat.Format16bppArgb1555:
                    break;
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    return PixelFormats.Bgr24;
                case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                    return PixelFormats.Bgr32;
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    return PixelFormats.Bgra32;
                case System.Drawing.Imaging.PixelFormat.Format32bppPArgb:
                    break;
                case System.Drawing.Imaging.PixelFormat.Format48bppRgb:
                    break;
                case System.Drawing.Imaging.PixelFormat.Format64bppArgb:
                    break;
                case System.Drawing.Imaging.PixelFormat.Format64bppPArgb:
                    break;
                case System.Drawing.Imaging.PixelFormat.Max:
                    break;
                default:
                    break;
            }

            throw new NotImplementedException(String.Format("Conversion from pixel format {0} is not supported yet, please open a new ticket in Accord.NET's issue tracker.", pixelFormat));
        }

        /// <summary>
        ///   Converts the given (System.Windows.Media.PixelFormat to its WPF (System.Drawing.Imaging) equivalent.
        /// </summary>
        /// 
        public static System.Drawing.Imaging.PixelFormat ToImaging(this System.Windows.Media.PixelFormat pixelFormat)
        {
            if (pixelFormat == PixelFormats.Indexed8)
                return System.Drawing.Imaging.PixelFormat.Format8bppIndexed;
            if (pixelFormat == PixelFormats.Gray8)
                return System.Drawing.Imaging.PixelFormat.Format8bppIndexed;
            if (pixelFormat == PixelFormats.Bgr24)
                return System.Drawing.Imaging.PixelFormat.Format24bppRgb;
            if (pixelFormat == PixelFormats.Bgr32)
                return System.Drawing.Imaging.PixelFormat.Format32bppRgb;
            if (pixelFormat == PixelFormats.Bgra32)
                return System.Drawing.Imaging.PixelFormat.Format32bppArgb;

            if (pixelFormat == PixelFormats.Gray32Float)
                return System.Drawing.Imaging.PixelFormat.Extended;

            throw new NotImplementedException(String.Format("Conversion from pixel format {0} is not supported yet, please open a new ticket in Accord.NET's issue tracker.", pixelFormat));
        }

        /// <summary>
        /// Apply filter to an image.
        /// </summary>
        /// 
        /// <param name="filter">The image filter.</param>
        /// <param name="image">Source image to apply filter to.</param>
        /// 
        /// <returns>Returns filter's result obtained by applying the filter to
        /// the source image.</returns>
        /// 
        /// <remarks>The method keeps the source image unchanged and returns
        /// the result of image processing filter as new image.</remarks> 
        ///
        public static BitmapSource Apply(this IFilter filter, BitmapSource image)
        {
            return filter.Apply(image.ToBitmap()).ToBitmapSource();
        }

        /// <summary>
        ///   Gets the stride (number of bytes that define a row) of the given image.
        /// </summary>
        /// 
        public static int GetStride(this BitmapSource image)
        {
            int bytesPerPixel = image.GetPixelFormatSize();
            return image.PixelWidth * bytesPerPixel;
        }

        /// <summary>
        ///   Gets the number of channels in a given image.
        /// </summary>
        /// 
        public static int GetNumberOfChannels(this BitmapSource bitmapSource)
        {
            return bitmapSource.Format.GetNumberOfChannels();
        }

        /// <summary>
        ///   Gets the number of channels in a given pixel format.
        /// </summary>
        /// 
        public static int GetNumberOfChannels(this System.Windows.Media.PixelFormat pixelFormat)
        {
            if (pixelFormat == PixelFormats.Indexed8)
                return 1;
            if (pixelFormat == PixelFormats.Bgr24)
                return 3;
            if (pixelFormat == PixelFormats.Bgr32)
                return 4;
            if (pixelFormat == PixelFormats.Bgra32)
                return 4;
            if (pixelFormat == PixelFormats.BlackWhite)
                return 1;
            if (pixelFormat == PixelFormats.Gray32Float)
                return 1;
            if (pixelFormat == PixelFormats.Pbgra32)
                return 4;
            if (pixelFormat == PixelFormats.Prgba128Float)
                return 4;
            if (pixelFormat == PixelFormats.Rgb128Float)
                return 3;
            if (pixelFormat == PixelFormats.Rgba128Float)
                return 4;

            throw new NotImplementedException(String.Format("Retrieving the number of channels in pixel format {0} is not supported yet, please open a new ticket in Accord.NET's issue tracker.", pixelFormat));
        }

        /// <summary>
        ///    Returns the color depth, in number of bits per pixel, of the specified pixel format.
        /// </summary>
        /// 
        public static int GetPixelFormatSize(this BitmapSource image)
        {
            return image.Format.BitsPerPixel / 8;
        }

        /// <summary>
        ///    Copies the bitmap pixel data to the given array.
        /// </summary>
        /// 
        public static void CopyPixels(this BitmapSource image, Array buffer)
        {
            int stride = image.GetStride();
            int numberOfBytes = buffer.GetNumberOfBytes();

            Int32Rect r = new Int32Rect(0, 0, image.PixelWidth, image.PixelHeight);

            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            IntPtr ptr = handle.AddrOfPinnedObject();

            image.CopyPixels(r, buffer: ptr, bufferSize: numberOfBytes, stride: stride);

            handle.Free();
        }
    }
}