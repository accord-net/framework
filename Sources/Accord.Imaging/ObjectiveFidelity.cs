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

using AForge.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Imaging
{
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
    /// // Create a new Wolf-Joulion threshold:
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
        private long errorTotal;
        private double mse;
        private double snr;
        private double psnr;
        private double dsnr;

        private int level = 256;

        /// <summary>
        ///   Error total.
        /// </summary>
        public long ErrorTotal { get { return errorTotal; } set { errorTotal = value; } }

        /// <summary>
        ///   Root mean square error.
        /// </summary>
        /// 
        public double MeanSquareError { get { return mse; } set { mse = value; } }

        /// <summary>
        ///   Signal to noise ratio.
        /// </summary>
        /// 
        public double SignalToNoiseRatio { get { return snr; } set { snr = value; } }

        /// <summary>
        ///   PEAK signal to noise ratio.
        /// </summary>
        /// 
        public double PeakSignalToNoiseRatio { get { return psnr; } set { psnr = value; } }

        /// <summary>
        ///   Derivative signal to noise ratio.
        /// </summary>
        /// 
        public double DerivativeSignalNoiseRatio { get { return dsnr; } set { dsnr = value; } }

        /// <summary>
        ///   Level used in PEAK signal to noise ratio.
        /// </summary>
        /// 
        public int Level { get { return level; } set { psnr = System.Math.Max(System.Math.Min(value, 256), 1); } }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ObjectiveFidelity"/> class.
        /// </summary>
        /// 
        /// <param name="original">Original image.</param>
        /// <param name="reconstructed">Reconstructed image.</param>
        /// 
        public ObjectiveFidelity(Bitmap original, Bitmap reconstructed)
        {
            // check image format
            if (!(original.PixelFormat == PixelFormat.Format8bppIndexed ||
                reconstructed.PixelFormat == PixelFormat.Format8bppIndexed))
            {
                throw new UnsupportedImageFormatException("Only grayscale images are supported.");
            }

            // check image format
            if (!(original.PixelFormat == PixelFormat.Format8bppIndexed &&
                reconstructed.PixelFormat == PixelFormat.Format8bppIndexed))
            {
                throw new UnsupportedImageFormatException("The both images must be in the same pixel format.");
            }

            // check image size
            if ((original.Width != reconstructed.Width) || (original.Height != reconstructed.Height))
            {
                throw new ArgumentException("The both images must be in the same size.");
            }

            // lock source image
            BitmapData dataOriginal = original.LockBits(
                new Rectangle(0, 0, original.Width, original.Height),
                ImageLockMode.ReadOnly, original.PixelFormat);

            BitmapData dataReconstructed = reconstructed.LockBits(
                new Rectangle(0, 0, reconstructed.Width, reconstructed.Height),
                ImageLockMode.ReadOnly, reconstructed.PixelFormat);

            Compute(new UnmanagedImage(dataOriginal), new UnmanagedImage(dataReconstructed));
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ObjectiveFidelity"/> class.
        /// </summary>
        /// 
        /// <param name="original">Original bitmap data.</param>
        /// <param name="reconstructed">Reconstructed bitmap data.</param>
        /// 
        public ObjectiveFidelity(BitmapData original, BitmapData reconstructed)
        {
            // check image format
            if (!(original.PixelFormat == PixelFormat.Format8bppIndexed ||
                reconstructed.PixelFormat == PixelFormat.Format8bppIndexed))
            {
                throw new UnsupportedImageFormatException("Only grayscale images are supported.");
            }

            // check image format
            if (!(original.PixelFormat == PixelFormat.Format8bppIndexed &&
                reconstructed.PixelFormat == PixelFormat.Format8bppIndexed))
            {
                throw new UnsupportedImageFormatException("The both images must be in the same pixel format.");
            }

            // check image size
            if ((original.Width != reconstructed.Width) || (original.Height != reconstructed.Height))
            {
                throw new ArgumentException("The both images must be in the same size.");
            }

            Compute(new UnmanagedImage(original), new UnmanagedImage(reconstructed));

        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ObjectiveFidelity"/> class.
        /// </summary>
        /// 
        /// <param name="original">Original unmanaged image.</param>
        /// <param name="reconstructed">Reconstructed unmanaged image.</param>
        /// 
        public ObjectiveFidelity(UnmanagedImage original, UnmanagedImage reconstructed)
        {
            Compute(original, reconstructed);
        }

        /// <summary>
        ///   Compute objective fidelity metrics.
        /// </summary>
        /// 
        /// <param name="original">Original image.</param>
        /// <param name="reconstructed">Reconstructed image.</param>
        /// 
        public unsafe void Compute(UnmanagedImage original, UnmanagedImage reconstructed)
        {
            int width = original.Width;
            int height = original.Height;

            int pixelSize = System.Drawing.Image.GetPixelFormatSize(original.PixelFormat) / 8;
            int stride = original.Stride;
            int offset = stride - original.Width * pixelSize;

            byte* o = (byte*)original.ImageData.ToPointer();
            byte* r = (byte*)reconstructed.ImageData.ToPointer();

            //Compute all metrics bellow
            // Error total
            long errorT = 0;

            //Root mean square
            double sumError = 0;

            //Signal to noise ratio
            double squareImg = 0;
            double squareRecon = 0;

            //PEAK signal to noise ratio
            double sum = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++, o++, r++)
                {
                    //Error total
                    int error = *r - *o;
                    errorT += error;

                    //Root mean square
                    double squareDiff = System.Math.Pow(error, 2);
                    sumError += squareDiff;

                    //Signal to noise ratio
                    squareRecon += *r * *r;
                    squareImg += System.Math.Pow(*r - *o, 2);

                    //PEAK signal to noise ratio
                    sum += System.Math.Pow(*r - *o, 2);

                }
                o += offset;
                r += offset;
            }
            
            //Error total
            errorTotal = errorT;

            //Root mean square
            double size = 1D/(double)(height * width);
            mse = System.Math.Sqrt(size * sumError);

            //Signal to noise ratio
            if (squareImg == 0)
                snr = 0;
            else
                snr = System.Math.Sqrt(squareRecon / squareImg);

            //PEAK signal to noise ratio
            size = original.Width * original.Height;
            if (sum == 0)
            {
                psnr = 0;
            }
            else
            {
                sum = (1D / (double)size) * sum;
                sum = level * level / sum;
                psnr = 10D * System.Math.Log10(sum);
            }

            //Derivative signal noise ratio
            dsnr = DerivativeSNR(original, reconstructed);
        }

        private static unsafe double DerivativeSNR(UnmanagedImage original, UnmanagedImage reconstructed)
        {
            int width = original.Width;
            int height = original.Height;

            int pixelSize = System.Drawing.Image.GetPixelFormatSize(original.PixelFormat) / 8;
            int stride = original.Stride;
            int offset = stride - original.Width * pixelSize;

            byte* o = (byte*)original.ImageData.ToPointer();
            byte* r = (byte*)reconstructed.ImageData.ToPointer();

            double sumGradO = 0;
            double sumGradDiff = 0;
            for (int y = 0; y < height - 1; y++)
            {
                for (int x = 0; x < width - 1; x++, o++, r++)
                {
                    int a = *o;
                    int b = o[+stride];
                    int c = o[+1];

                    //Original image
                    int gradO = System.Math.Abs(*o - o[+stride]) + System.Math.Abs(*o - o[+1]);
                    sumGradO += gradO * gradO;

                    //Reconstructed image
                    int gradR = System.Math.Abs(*r - r[+stride]) + System.Math.Abs(*r - r[+1]);
                    sumGradDiff += System.Math.Pow(gradO - gradR, 2);
                }
                o += offset + 1;
                r += offset + 1;
            }

            if (sumGradDiff == 0) return 0;
            double result = sumGradO / sumGradDiff;
            return 10 * System.Math.Log10(result);
        }
    }
}