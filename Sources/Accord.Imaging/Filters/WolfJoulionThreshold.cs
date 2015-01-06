// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Diego Catalano, 2015
// diego.catalano at live.com
//
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing.Imaging;
    using AForge.Imaging;
    using AForge.Imaging.Filters;

    /// <summary>
    ///   WolfJoulion Threshold.
    /// </summary>
    /// 
    /// <remarks>
    ///   <para>
    ///   The WolfJoulion filter is a variation of the <see cref="SauvolaThreshold"/>
    ///   thresholding filter.</para>
    ///   
    /// <para>
    ///  This filter implementation has been contributed by Diego Catalano.</para>
    ///  
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///         Sauvola, Jaakko, and Matti Pietikäinen. "Adaptive document image binarization."
    ///         Pattern Recognition 33.2 (2000): 225-236.</description></item>
    ///   </list></para>   
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    /// Bitmap image = ... // Lena's picture
    /// 
    /// // Create a new Sauvola threshold:
    /// var sauvola = new SauvolaThreshold();
    /// 
    /// // Compute the filter
    /// Bitmap result = sauvola.Apply(image);
    /// 
    /// // Show on the screen
    /// ImageBox.Show(result);
    /// </code>
    /// 
    /// <para>
    ///   The resulting image is shown below:</para>
    /// 
    ///   <img src="..\images\sauvola.png" />
    /// 
    /// </example>
    /// 
    /// <seealso cref="NiblackThreshold"/>
    /// <seealso cref="SauvolaThreshold"/>
    /// 
    public class WolfJoulionThreshold : BaseFilter
    {
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;

        private int radius = 15;
        private double k = 0.5D;
        private double r = 128;

        /// <summary>
        ///   Gets or sets the filter convolution
        ///   radius. Default is 15.
        /// </summary>
        /// 
        public int Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        /// <summary>
        ///   Gets or sets the user-defined 
        ///   parameter k. Default is 0.5.
        /// </summary>
        /// 
        public double K
        {
            get { return k; }
            set { k = value; }
        }

        /// <summary>
        ///   Gets or sets the dynamic range of the 
        ///   standard deviation, R. Default is 128.
        /// </summary>
        /// 
        public double R
        {
            get { return r; }
            set { r = value; }
        }

        /// <summary>
        ///   Format translations dictionary.
        /// </summary>
        /// 
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="WolfJoulionThreshold"/> class.
        /// </summary>
        /// 
        public WolfJoulionThreshold()
        {
            formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
        }


        /// <summary>
        ///   Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// <param name="destinationData">Destination image data.</param>
        /// 
        protected override unsafe void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
        {
            int width = sourceData.Width;
            int height = sourceData.Height;

            int size = width * height;

            int pixelSize = System.Drawing.Image.GetPixelFormatSize(sourceData.PixelFormat) / 8;

            int srcStride = sourceData.Stride;
            int dstStride = destinationData.Stride;

            int srcOffset = srcStride - width * pixelSize;
            int dstOffset = dstStride - width * pixelSize;

            byte* src = (byte*)sourceData.ImageData.ToPointer();
            byte* dst = (byte*)destinationData.ImageData.ToPointer();

            //Mean filter
            int[,] kernel = Accord.Math.Matrix.Create(radius*2+1, radius*2+1,1);

            Convolution conv = new Convolution(kernel);
            UnmanagedImage mean = conv.Apply(sourceData);

            //Variance filter
            FastVariance fv = new FastVariance(radius);
            UnmanagedImage var = fv.Apply(sourceData);

            // do the processing job
            if (sourceData.PixelFormat == PixelFormat.Format8bppIndexed)
            {

                //Store maximum value from variance.
                byte* srcVar = (byte*)var.ImageData.ToPointer();
                byte* srcMean = (byte*)mean.ImageData.ToPointer();

                int maxV = 0;
                for (int i = 0; i < size; i++)
                    if (srcVar[i] > maxV)
                        maxV = srcVar[i];


                //Store minimum value from original image.
                int minG = 255;
                for (int i = 0; i < size; i++)
                    if (src[i] < (byte)minG)
                        minG = src[i];

                //Do the job
                for (int i = 0; i < size; i++)
                {
                    double p = src[i];
                    double mP = srcMean[i];
                    double vP = srcVar[i];

                    int g = (p > (mP + k * ((Math.Sqrt(vP) / (double)maxV - 1.0) * (mP - (double)minG)))) ? 255 : 0;

                    dst[i] = (byte)g;
                }

            }
            else
            {

                //Store maximum value from variance.
                byte* srcVar = (byte*)var.ImageData.ToPointer();
                byte* srcMean = (byte*)mean.ImageData.ToPointer();

                int maxVR = 0;
                int maxVG = 0;
                int maxVB = 0;
                for (int i = 0; i < size; i++)
                {
                    byte* p = &srcVar[i];
                    if (p[RGB.R] > maxVR)
                        maxVR = p[RGB.R];

                    if (p[RGB.G] > maxVG)
                        maxVG = p[RGB.G];

                    if (p[RGB.B] > maxVB)
                        maxVB = p[RGB.B];
                }


                //Store minimum value from original image.
                int minR = 255;
                int minG = 255;
                int minB = 255;
                for (int i = 0; i < size; i++)
                {
                    byte* p = &src[i];
                    if (p[RGB.R] < minR)
                        minR = p[RGB.R];

                    if (p[RGB.G] < minG)
                        minG = p[RGB.G];

                    if (p[RGB.B] < minB)
                        minB = p[RGB.B];
                }

                //Pointers
                for (int i = 0; i < size; i++)
                {
                    byte* p = &src[i];
                    byte* mP = &srcMean[i];
                    byte* vP = &srcVar[i];

                    double rP = p[RGB.R];
                    double gP = p[RGB.G];
                    double bP = p[RGB.B];

                    double rmP = mP[RGB.R];
                    double gmP = mP[RGB.G];
                    double bmP = mP[RGB.B];

                    double rvP = vP[RGB.R];
                    double gvP = vP[RGB.G];
                    double bvP = vP[RGB.B];

                    int r = (rP > (rmP + k * ((Math.Sqrt(rvP) / (double)maxVR - 1.0) * (rmP - (double)minR)))) ? 255 : 0;
                    int g = (gP > (gmP + k * ((Math.Sqrt(gvP) / (double)maxVG - 1.0) * (gmP - (double)minG)))) ? 255 : 0;
                    int b = (bP > (bmP + k * ((Math.Sqrt(bvP) / (double)maxVB - 1.0) * (bmP - (double)minB)))) ? 255 : 0;

                    dst[RGB.R] = (byte)r;
                    dst[RGB.G] = (byte)g;
                    dst[RGB.B] = (byte)b;

                    //TODO: Need to move dst pointer.

                }
            }
        }
    }
}