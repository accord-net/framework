// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Diego Catalano, 2015
// diego.catalano at live.com
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
    using Accord.Compat;
    using System.Threading.Tasks;

    /// <summary>
    ///   Objective Fidelity Criteria.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///         H.T. Yalazan, J.D. Yucel. "A new objective fidelity criterion
    ///         for image processing." Proceedings of the 16th International 
    ///         Conference on Pattern Recognition, 2002.</description></item>
    ///   </list></para>  
    /// </remarks>
    ///   
    /// <example>
    /// <code>
    /// Bitmap ori = ... // Original picture
    /// Bitmap recon = ... // Reconstructed picture
    /// 
    /// // Create a new Objective fidelity comparer:
    /// var of = new ObjectiveFidelity(ori, recon);
    /// 
    /// // Get the results
    /// long errorTotal = of.ErrorTotal;
    /// double msr = of.MeanSquareError;
    /// double snr = of.SignalToNoiseRatio;
    /// double psnr = of.PeakSignalToNoiseRatio;
    /// double dsnr = of.DerivativeSignalNoiseRatio;
    /// </code>
    /// </example>
    /// 
    public class ObjectiveFidelity
    {
        private long totalError;
        private double meanError;
        private double mse;
        private double signalNoiseRatio;
        private double peakSignalNoiseRatio;
        private double derivativeSignalNoiseRatio;

        private int level = 256;

        /// <summary>
        ///  Gets the total error between the two images.
        /// </summary>
        /// 
        public long AbsoluteError
        {
            get { return totalError; }
        }

        /// <summary>
        ///   Gets the average error between the two images.
        /// </summary>
        /// 
        public double MeanError
        {
            get { return meanError; }
        }

        /// <summary>
        ///   Gets the root mean square error between the two images.
        /// </summary>
        /// 
        public double MeanSquareError
        {
            get { return mse; }
        }

        /// <summary>
        ///   Gets the signal to noise ratio.
        /// </summary>
        /// 
        public double SignalToNoiseRatio
        {
            get { return signalNoiseRatio; }
        }

        /// <summary>
        ///   Gets the peak signal to noise ratio.
        /// </summary>
        /// 
        public double PeakSignalToNoiseRatio
        {
            get { return peakSignalNoiseRatio; }
        }

        /// <summary>
        ///   Gets the derivative signal to noise ratio.
        /// </summary>
        /// 
        public double DerivativeSignalNoiseRatio
        {
            get { return derivativeSignalNoiseRatio; }
        }

        /// <summary>
        ///   Gets the level used in peak signal to noise ratio.
        /// </summary>
        /// 
        public int Level
        {
            get { return level; }
            set
            {
                if (value < 0 || value > 255)
                    throw new ArgumentOutOfRangeException("value", "Value must be between 0 and 255.");
                level = value;
            }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ObjectiveFidelity"/> class.
        /// </summary>
        /// 
        public ObjectiveFidelity()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ObjectiveFidelity"/> class.
        /// </summary>
        /// 
        /// <param name="a">The first image to be compared.</param>
        /// <param name="b">The second image that will be compared.</param>
        /// 
        public ObjectiveFidelity(Bitmap a, Bitmap b)
        {
            Compute(a, b);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ObjectiveFidelity"/> class.
        /// </summary>
        /// 
        /// <param name="a">The first image to be compared.</param>
        /// <param name="b">The second image that will be compared.</param>
        /// 
        public ObjectiveFidelity(BitmapData a, BitmapData b)
        {
            Compute(a, b);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ObjectiveFidelity"/> class.
        /// </summary>
        /// 
        /// <param name="a">The first image to be compared.</param>
        /// <param name="b">The second image that will be compared.</param>
        /// 
        public ObjectiveFidelity(UnmanagedImage a, UnmanagedImage b)
        {
            Compute(a, b);
        }

        /// <summary>
        ///   Compute objective fidelity metrics.
        /// </summary>
        /// 
        /// <param name="a">The first image to be compared.</param>
        /// <param name="b">The second image that will be compared.</param>
        /// 
        public void Compute(Bitmap a, Bitmap b)
        {
            // lock source image
            BitmapData dataOriginal = a.LockBits(ImageLockMode.ReadOnly);
            BitmapData dataReconstructed = b.LockBits(ImageLockMode.ReadOnly);

            Compute(new UnmanagedImage(dataOriginal), new UnmanagedImage(dataReconstructed));

            a.UnlockBits(dataOriginal);
            b.UnlockBits(dataReconstructed);
        }

        /// <summary>
        ///   Compute objective fidelity metrics.
        /// </summary>
        /// 
        /// <param name="a">The first image to be compared.</param>
        /// <param name="b">The second image that will be compared.</param>
        /// 
        public void Compute(BitmapData a, BitmapData b)
        {
            Compute(new UnmanagedImage(a), new UnmanagedImage(b));
        }

        /// <summary>
        ///   Compute objective fidelity metrics.
        /// </summary>
        /// 
        /// <param name="a">The first image to be compared.</param>
        /// <param name="b">The second image that will be compared.</param>
        /// 
        public void Compute(UnmanagedImage a, UnmanagedImage b)
        {
            // check image format
            if (!(a.PixelFormat == PixelFormat.Format8bppIndexed ||
                b.PixelFormat == PixelFormat.Format8bppIndexed))
            {
                throw new UnsupportedImageFormatException("Only grayscale images are supported.");
            }

            // check image format
            if (!(a.PixelFormat == PixelFormat.Format8bppIndexed &&
                b.PixelFormat == PixelFormat.Format8bppIndexed))
            {
                throw new UnsupportedImageFormatException("The both images must be in the same pixel format.");
            }

            // check image size
            if ((a.Width != b.Width) || (a.Height != b.Height))
            {
                throw new ArgumentException("The both images must be in the same size.");
            }


            int width = a.Width;
            int height = a.Height;

            int pixelSize = System.Drawing.Image.GetPixelFormatSize(a.PixelFormat) / 8;
            int stride = a.Stride;
            int offset = stride - a.Width * pixelSize;

            // Total error
            long sum = 0;
            double sumOfSquares = 0;
            double imageSumOfSquares = 0;

            unsafe
            {
                byte* ptrA = (byte*)a.ImageData.ToPointer();
                byte* ptrB = (byte*)b.ImageData.ToPointer();


                // Compute all metrics above
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++, ptrA++, ptrB++)
                    {
                        long pA = *ptrA;
                        long pB = *ptrB;

                        long error = pB - pA;
                        long square = error * error;

                        // total error 
                        sum += error;

                        // root mean square
                        sumOfSquares += square;

                        // signal to noise ratio
                        imageSumOfSquares += pB * pB;
                    }

                    ptrA += offset;
                    ptrB += offset;
                }
            }


            double size = height * width;


            // total error
            this.totalError = sum;

            // mean error
            this.meanError = sumOfSquares / size;

            // root mean square
            this.mse = System.Math.Sqrt(meanError);


            // signal to noise ratio
            if (sumOfSquares != 0)
            {
                // Signal to noise ratio
                this.signalNoiseRatio = System.Math.Sqrt(imageSumOfSquares / sumOfSquares);

                // peak signal to noise ratio
                this.peakSignalNoiseRatio = 10 * Math.Log10((level * level) / meanError);
            }
            else
            {
                this.signalNoiseRatio = 0;
                this.peakSignalNoiseRatio = 0;
            }

            // derivative signal noise ratio
            this.derivativeSignalNoiseRatio = derivativeSNR(a, b);
        }


        private static unsafe double derivativeSNR(UnmanagedImage a, UnmanagedImage b)
        {
            int width = a.Width;
            int height = a.Height;

            int pixelSize = System.Drawing.Image.GetPixelFormatSize(a.PixelFormat) / 8;
            int stride = a.Stride;
            int offset = stride - a.Width * pixelSize;

            byte* ptrA = (byte*)a.ImageData.ToPointer();
            byte* ptrB = (byte*)b.ImageData.ToPointer();

            double sum1 = 0;
            double sum2 = 0;

            for (int y = 0; y < height - 1; y++)
            {
                for (int x = 0; x < width - 1; x++, ptrA++, ptrB++)
                {
                    // original image
                    int gradO = System.Math.Abs(*ptrA - ptrA[+stride]) + System.Math.Abs(*ptrA - ptrA[+1]);
                    sum1 += gradO * gradO;

                    // reconstructed image
                    int gradR = System.Math.Abs(*ptrB - ptrB[+stride]) + System.Math.Abs(*ptrB - ptrB[+1]);
                    sum2 += gradR * gradR;
                }

                ptrA += offset + 1;
                ptrB += offset + 1;
            }

            if (sum2 == 0)
                return 0;

            return 10 * System.Math.Log10(sum1 / sum2);
        }
    }
}