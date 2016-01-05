// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
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

namespace Accord.Imaging
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;
    using System.Security;

    /// <summary>
    ///   Joint representation of both Integral Image and Squared Integral Image.
    /// </summary>
    /// 
    /// <remarks>
    ///   Using this representation, both structures can be created in a single pass
    ///   over the data. This is interesting for real time applications. This class
    ///   also accepts a channel parameter indicating the Integral Image should be
    ///   computed using a specified color channel. This avoids costly conversions.
    /// </remarks>
    /// 
    [SecurityCritical]
    public unsafe class IntegralImage2 : IDisposable
    {

        private long[,] nSumImage; // normal  integral image
        private long[,] sSumImage; // squared integral image
        private long[,] tSumImage; // tilted  integral image

        private long* nSum; // normal  integral image
        private long* sSum; // squared integral image
        private long* tSum; // tilted  integral image

        private GCHandle nSumHandle;
        private GCHandle sSumHandle;
        private GCHandle tSumHandle;

        private int width;
        private int height;

        private int nWidth;
        private int nHeight;

        private int tWidth;
        private int tHeight;


        /// <summary>
        ///   Gets the image's width.
        /// </summary>
        /// 
        public int Width
        {
            get { return width; }
        }

        /// <summary>
        ///   Gets the image's height.
        /// </summary>
        /// 
        public int Height
        {
            get { return height; }
        }

        /// <summary>
        ///   Gets the Integral Image for values' sum.
        /// </summary>
        /// 
        public long[,] Image
        {
            get { return nSumImage; }
        }

        /// <summary>
        ///   Gets the Integral Image for values' squared sum.
        /// </summary>
        /// 
        public long[,] Squared
        {
            get { return sSumImage; }
        }

        /// <summary>
        ///   Gets the Integral Image for tilted values' sum.
        /// </summary>
        /// 
        public long[,] Rotated
        {
            get { return tSumImage; }
        }

        /// <summary>
        ///   Constructs a new Integral image of the given size.
        /// </summary>
        /// 
        protected IntegralImage2(int width, int height, bool computeTilted)
        {
            this.width = width;
            this.height = height;

            this.nWidth = width + 1;
            this.nHeight = height + 1;

            this.tWidth = width + 2;
            this.tHeight = height + 2;

            this.nSumImage = new long[nHeight, nWidth];
            this.nSumHandle = GCHandle.Alloc(nSumImage, GCHandleType.Pinned);
            this.nSum = (long*)nSumHandle.AddrOfPinnedObject().ToPointer();

            this.sSumImage = new long[nHeight, nWidth];
            this.sSumHandle = GCHandle.Alloc(sSumImage, GCHandleType.Pinned);
            this.sSum = (long*)sSumHandle.AddrOfPinnedObject().ToPointer();

            if (computeTilted)
            {
                this.tSumImage = new long[tHeight, tWidth];
                this.tSumHandle = GCHandle.Alloc(tSumImage, GCHandleType.Pinned);
                this.tSum = (long*)tSumHandle.AddrOfPinnedObject().ToPointer();
            }
        }

        /// <summary>
        ///   Constructs a new Integral image from a Bitmap image.
        /// </summary>
        /// 
        /// <param name="image">The source image from where the integral image should be computed.</param>
        /// 
        /// <returns>
        ///   The <see cref="IntegralImage2"/> representation of 
        ///   the <paramref name="image">source image</paramref>.</returns>
        /// 
        public static IntegralImage2 FromBitmap(Bitmap image)
        {
            return FromBitmap(image, 0, false);
        }

        /// <summary>
        ///   Constructs a new Integral image from a Bitmap image.
        /// </summary>
        /// 
        /// <param name="image">The source image from where the integral image should be computed.</param>
        /// <param name="channel">The image channel to consider in the computations. Default is 0.</param>
        /// 
        /// <returns>
        ///   The <see cref="IntegralImage2"/> representation of 
        ///   the <paramref name="image">source image</paramref>.</returns>
        /// 
        public static IntegralImage2 FromBitmap(Bitmap image, int channel)
        {
            return FromBitmap(image, channel, false);
        }

        /// <summary>
        ///   Constructs a new Integral image from a Bitmap image.
        /// </summary>
        /// 
        /// <param name="image">The source image from where the integral image should be computed.</param>
        /// <param name="computeTilted"><c>True</c> to compute the tilted version of the integral image,
        ///   <c>false</c> otherwise. Default is false.</param>
        /// 
        /// <returns>
        ///   The <see cref="IntegralImage2"/> representation of 
        ///   the <paramref name="image">source image</paramref>.</returns>
        /// 
        public static IntegralImage2 FromBitmap(Bitmap image, bool computeTilted)
        {
            return FromBitmap(image, 0, computeTilted);
        }

        /// <summary>
        ///   Constructs a new Integral image from a Bitmap image.
        /// </summary>
        /// 
        /// <param name="image">The source image from where the integral image should be computed.</param>
        /// <param name="channel">The image channel to consider in the computations. Default is 0.</param>
        /// <param name="computeTilted"><c>True</c> to compute the tilted version of the integral image,
        ///   <c>false</c> otherwise. Default is false.</param>
        ///   
        /// <returns>
        ///   The <see cref="IntegralImage2"/> representation of 
        ///   the <paramref name="image">source image</paramref>.</returns>
        /// 
        public static IntegralImage2 FromBitmap(Bitmap image, int channel, bool computeTilted)
        {
            // check image format
            if (!(image.PixelFormat == PixelFormat.Format8bppIndexed ||
                image.PixelFormat == PixelFormat.Format24bppRgb ||
                image.PixelFormat == PixelFormat.Format32bppArgb ||
                image.PixelFormat == PixelFormat.Format32bppRgb))
            {
                throw new UnsupportedImageFormatException("Only grayscale, 24 and 32 bpp RGB images are supported.");
            }


            // lock source image
            BitmapData imageData = image.LockBits(
                new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, image.PixelFormat);

            // process the image
            IntegralImage2 im = FromBitmap(imageData, channel, computeTilted);

            // unlock image
            image.UnlockBits(imageData);

            return im;
        }

        /// <summary>
        ///   Constructs a new Integral image from a BitmapData image.
        /// </summary>
        /// 
        /// <param name="imageData">The source image from where the integral image should be computed.</param>
        /// 
        /// <returns>
        ///   The <see cref="IntegralImage2"/> representation of 
        ///   the <paramref name="imageData">source image</paramref>.</returns>
        /// 
        public static IntegralImage2 FromBitmap(BitmapData imageData)
        {
            return FromBitmap(imageData, 0);
        }

        /// <summary>
        ///   Constructs a new Integral image from a BitmapData image.
        /// </summary>
        /// 
        /// <param name="imageData">The source image from where the integral image should be computed.</param>
        /// <param name="channel">The image channel to consider in the computations. Default is 0.</param>
        /// 
        /// <returns>
        ///   The <see cref="IntegralImage2"/> representation of 
        ///   the <paramref name="imageData">source image</paramref>.</returns>
        /// 
        public static IntegralImage2 FromBitmap(BitmapData imageData, int channel)
        {
            return FromBitmap(new UnmanagedImage(imageData), channel);
        }

        /// <summary>
        ///   Constructs a new Integral image from a BitmapData image.
        /// </summary>
        /// 
        /// <param name="imageData">The source image from where the integral image should be computed.</param>
        /// <param name="channel">The image channel to consider in the computations. Default is 0.</param>
        /// <param name="computeTilted"><c>True</c> to compute the tilted version of the integral image,
        ///   <c>false</c> otherwise. Default is false.</param>
        ///   
        /// <returns>
        ///   The <see cref="IntegralImage2"/> representation of 
        ///   the <paramref name="imageData">source image</paramref>.</returns>
        /// 
        public static IntegralImage2 FromBitmap(BitmapData imageData, int channel, bool computeTilted)
        {
            return FromBitmap(new UnmanagedImage(imageData), channel, computeTilted);
        }

        /// <summary>
        ///   Constructs a new Integral image from a BitmapData image.
        /// </summary>
        /// 
        /// <param name="imageData">The source image from where the integral image should be computed.</param>
        /// <param name="computeTilted"><c>True</c> to compute the tilted version of the integral image,
        ///   <c>false</c> otherwise. Default is false.</param>
        /// 
        /// <returns>
        ///   The <see cref="IntegralImage2"/> representation of 
        ///   the <paramref name="imageData">source image</paramref>.</returns>
        /// 
        public static IntegralImage2 FromBitmap(BitmapData imageData, bool computeTilted)
        {
            return FromBitmap(new UnmanagedImage(imageData), 0, computeTilted);
        }

        /// <summary>
        ///   Constructs a new Integral image from an unmanaged image.
        /// </summary>
        /// 
        /// <param name="image">The source image from where the integral image should be computed.</param>
        /// <param name="channel">The image channel to consider in the computations. Default is 0.</param>
        /// 
        /// <returns>
        ///   The <see cref="IntegralImage2"/> representation of 
        ///   the <paramref name="image">source image</paramref>.</returns>
        /// 
        public static IntegralImage2 FromBitmap(UnmanagedImage image, int channel)
        {
            return FromBitmap(image, channel, false);
        }

        /// <summary>
        ///   Constructs a new Integral image from an unmanaged image.
        /// </summary>
        /// 
        /// <param name="image">The source image from where the integral image should be computed.</param>
        /// 
        /// <returns>
        ///   The <see cref="IntegralImage2"/> representation of 
        ///   the <paramref name="image">source image</paramref>.</returns>
        /// 
        public static IntegralImage2 FromBitmap(UnmanagedImage image)
        {
            return FromBitmap(image, 0, false);
        }

        /// <summary>
        ///   Constructs a new Integral image from an unmanaged image.
        /// </summary>
        /// 
        /// <param name="image">The source image from where the integral image should be computed.</param>
        /// <param name="computeTilted"><c>True</c> to compute the tilted version of the integral image,
        ///   <c>false</c> otherwise. Default is false.</param>
        ///   
        /// <returns>
        ///   The <see cref="IntegralImage2"/> representation of 
        ///   the <paramref name="image">source image</paramref>.</returns>
        /// 
        public static IntegralImage2 FromBitmap(UnmanagedImage image, bool computeTilted)
        {
            return FromBitmap(image, 0, computeTilted);
        }

        /// <summary>
        ///   Constructs a new Integral image from an unmanaged image.
        /// </summary>
        /// 
        /// <param name="image">The source image from where the integral image should be computed.</param>
        /// <param name="channel">The image channel to consider in the computations. Default is 0.</param>
        /// <param name="computeTilted"><c>True</c> to compute the tilted version of the integral image,
        ///   <c>false</c> otherwise. Default is false.</param>
        /// 
        /// <returns>
        ///   The <see cref="IntegralImage2"/> representation of 
        ///   the <paramref name="image">source image</paramref>.</returns>
        /// 
        public static IntegralImage2 FromBitmap(UnmanagedImage image, int channel, bool computeTilted)
        {

            // check image format
            if (!(image.PixelFormat == PixelFormat.Format8bppIndexed ||
                image.PixelFormat == PixelFormat.Format24bppRgb ||
                image.PixelFormat == PixelFormat.Format32bppArgb ||
                image.PixelFormat == PixelFormat.Format32bppRgb))
            {
                throw new UnsupportedImageFormatException("Only grayscale, 24 and 32 bpp RGB images are supported.");
            }

            int pixelSize = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;

            // get source image size
            int width = image.Width;
            int height = image.Height;
            int stride = image.Stride;
            int offset = stride - width * pixelSize;

            // create integral image
            IntegralImage2 im = new IntegralImage2(width, height, computeTilted);
            long* nSum = im.nSum;
            long* sSum = im.sSum;
            long* tSum = im.tSum;

            int nWidth = im.nWidth;
            int tWidth = im.tWidth;

            if (image.PixelFormat == PixelFormat.Format8bppIndexed && channel != 0)
                throw new ArgumentException("Only the first channel is available for 8 bpp images.", "channel");

            byte* srcStart = (byte*)image.ImageData.ToPointer() + channel;

            // do the job
            byte* src = srcStart;

            // for each line
            for (int y = 1; y <= height; y++)
            {
                int yy = nWidth * (y);
                int y1 = nWidth * (y - 1);

                // for each pixel
                for (int x = 1; x <= width; x++, src += pixelSize)
                {
                    long p1 = *src;
                    long p2 = p1 * p1;

                    int r = yy + (x);
                    int a = yy + (x - 1);
                    int b = y1 + (x);
                    int c = y1 + (x - 1);

                    nSum[r] = p1 + nSum[a] + nSum[b] - nSum[c];
                    sSum[r] = p2 + sSum[a] + sSum[b] - sSum[c];
                }
                src += offset;
            }


            if (computeTilted)
            {
                src = srcStart;

                // Left-to-right, top-to-bottom pass
                for (int y = 1; y <= height; y++, src += offset)
                {
                    int yy = tWidth * (y);
                    int y1 = tWidth * (y - 1);
                    
                    for (int x = 2; x < width + 2; x++, src += pixelSize)
                    {
                        int a = y1 + (x - 1);
                        int b = yy + (x - 1);
                        int c = y1 + (x - 2);
                        int r = yy + (x);

                        tSum[r] = *src + tSum[a] + tSum[b] - tSum[c];
                    }
                }

                {
                    int yy = tWidth * (height);
                    int y1 = tWidth * (height + 1);

                    for (int x = 2; x < width + 2; x++, src += pixelSize)
                    {
                        int a = yy + (x - 1);
                        int c = yy + (x - 2);
                        int b = y1 + (x - 1);
                        int r = y1 + (x);

                        tSum[r] = tSum[a] + tSum[b] - tSum[c];
                    }
                }


                // Right-to-left, bottom-to-top pass
                for (int y = height; y >= 0; y--)
                {
                    int yy = tWidth * (y);
                    int y1 = tWidth * (y + 1);

                    for (int x = width + 1; x >= 1; x--)
                    {
                        int r = yy + (x);
                        int b = y1 + (x - 1);

                        tSum[r] += tSum[b];
                    }
                }

                for (int y = height + 1; y >= 0; y--)
                {
                    int yy = tWidth * (y);

                    for (int x = width + 1; x >= 2; x--)
                    {
                        int r = yy + (x);
                        int b = yy + (x - 2);

                        tSum[r] -= tSum[b];
                    }
                }
            }


            return im;
        }

        /// <summary>
        ///   Gets the sum of the pixels in a rectangle of the Integral image.
        /// </summary>
        /// 
        /// <param name="x">The horizontal position of the rectangle <c>x</c>.</param>
        /// <param name="y">The vertical position of the rectangle <c>y</c>.</param>
        /// <param name="height">The rectangle's height <c>h</c>.</param>
        /// <param name="width">The rectangle's width <c>w</c>.</param>
        /// 
        /// <returns>The sum of all pixels contained in the rectangle, computed
        ///   as I[y, x] + I[y + h, x + w] - I[y + h, x] - I[y, x + w].</returns>
        /// 
        public long GetSum(int x, int y, int width, int height)
        {
            int a = nWidth * (y) + (x);
            int b = nWidth * (y + height) + (x + width);
            int c = nWidth * (y + height) + (x);
            int d = nWidth * (y) + (x + width);

            return nSum[a] + nSum[b] - nSum[c] - nSum[d];
        }

        /// <summary>
        ///   Gets the sum of the squared pixels in a rectangle of the Integral image.
        /// </summary>
        /// 
        /// <param name="x">The horizontal position of the rectangle <c>x</c>.</param>
        /// <param name="y">The vertical position of the rectangle <c>y</c>.</param>
        /// <param name="height">The rectangle's height <c>h</c>.</param>
        /// <param name="width">The rectangle's width <c>w</c>.</param>
        /// 
        /// <returns>The sum of all pixels contained in the rectangle, computed
        ///   as I²[y, x] + I²[y + h, x + w] - I²[y + h, x] - I²[y, x + w].</returns>
        /// 
        public long GetSum2(int x, int y, int width, int height)
        {
            int a = nWidth * (y) + (x);
            int b = nWidth * (y + height) + (x + width);
            int c = nWidth * (y + height) + (x);
            int d = nWidth * (y) + (x + width);

            return sSum[a] + sSum[b] - sSum[c] - sSum[d];
        }


        /// <summary>
        ///   Gets the sum of the pixels in a tilted rectangle of the Integral image.
        /// </summary>
        /// 
        /// <param name="x">The horizontal position of the rectangle <c>x</c>.</param>
        /// <param name="y">The vertical position of the rectangle <c>y</c>.</param>
        /// <param name="height">The rectangle's height <c>h</c>.</param>
        /// <param name="width">The rectangle's width <c>w</c>.</param>
        /// 
        /// <returns>The sum of all pixels contained in the rectangle, computed
        ///   as T[y + w, x + w + 1] + T[y + h, x - h + 1] - T[y, x + 1] - T[y + w + h, x + w - h + 1].</returns>
        /// 
        public long GetSumT(int x, int y, int width, int height)
        {
            int a = tWidth * (y + width) + (x + width + 1);
            int b = tWidth * (y + height) + (x - height + 1);
            int c = tWidth * (y) + (x + 1);
            int d = tWidth * (y + width + height) + (x + width - height + 1);

            return tSum[a] + tSum[b] - tSum[c] - tSum[d];
        }



        #region IDisposable Members

        /// <summary>
        ///   Performs application-defined tasks associated with freeing,
        ///   releasing, or resetting unmanaged resources.
        /// </summary>
        /// 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///   Releases unmanaged resources and performs other cleanup operations 
        ///   before the <see cref="IntegralImage2"/> is reclaimed by garbage collection.
        /// </summary>
        /// 
        ~IntegralImage2()
        {
            Dispose(false);
        }

        /// <summary>
        ///   Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// 
        /// <param name="disposing"><c>true</c> to release both managed 
        /// and unmanaged resources; <c>false</c> to release only unmanaged
        /// resources.</param>
        /// 
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                // (i.e. IDisposable objects)
            }

            // free native resources
            if (nSumHandle.IsAllocated)
            {
                nSumHandle.Free();
                nSum = null;
            }
            if (sSumHandle.IsAllocated)
            {
                sSumHandle.Free();
                sSum = null;
            }
            if (tSumHandle.IsAllocated)
            {
                tSumHandle.Free();
                tSum = null;
            }
        }

        #endregion

    }
}
