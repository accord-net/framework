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

    /// <summary>
    ///   Extension methods for WPF-format images (BitmapSource).
    /// </summary>
    /// 
    public static class WpfExtensionMethods
    {
        #region ToBitmapSource
        /// <summary>
        ///   Converts an image given as a matrix of pixel values into a <see cref="BitmapSource"/>.
        ///   For more options, please use the <see cref="MatrixToBitmapSouorce"/> class.
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
        ///   For more options, please use the <see cref="MatrixToBitmapSouorce"/> class.
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
        ///   For more options, please use the <see cref="MatrixToBitmapSouorce"/> class.
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
        ///   For more options, please use the <see cref="MatrixToBitmapSouorce"/> class.
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
        ///   For more options, please use the <see cref="MatrixToBitmapSouorce"/> class.
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

        public static Bitmap ToBitmap(this BitmapSource bitmapSource)
        {
            var format = bitmapSource.Format.ToImaging();

            Bitmap bmp = new Bitmap(bitmapSource.PixelWidth, bitmapSource.PixelHeight, format);

            BitmapData data = bmp.LockBits(ImageLockMode.WriteOnly);

            bitmapSource.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bmp.UnlockBits(data);
            return bmp;
        }

        public static UnmanagedImage ToUnmanagedImage(this BitmapSource bitmap)
        {
            return bitmap.ToBitmap().ToUnmanagedImage();
        }

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

        public static System.Drawing.Imaging.PixelFormat ToImaging(this System.Windows.Media.PixelFormat pixelFormat)
        {
            if (pixelFormat == PixelFormats.Gray8)
                return System.Drawing.Imaging.PixelFormat.Format8bppIndexed;
            if (pixelFormat == PixelFormats.Bgr24)
                return System.Drawing.Imaging.PixelFormat.Format24bppRgb;
            if (pixelFormat == PixelFormats.Bgr32)
                return System.Drawing.Imaging.PixelFormat.Format32bppRgb;
            if (pixelFormat == PixelFormats.Bgra32)
                return System.Drawing.Imaging.PixelFormat.Format32bppArgb;

            throw new NotImplementedException(String.Format("Conversion from pixel format {0} is not supported yet, please open a new ticket in Accord.NET's issue tracker.", pixelFormat));
        }


        public static BitmapSource Apply(this IFilter filter, BitmapSource image)
        {
            return filter.Apply(image.ToBitmap()).ToBitmapSource();
        }
    }
}