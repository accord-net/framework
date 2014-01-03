// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
// cesarsouza at gmail.com
//
// Copyright (c) 2011-2012 LTS2, EPFL
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of the CherryPy Team nor the names of its contributors 
//      may be used to endorse or promote products derived from this software 
//      without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE 
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//

namespace Accord.Imaging
{
    using Accord.Math;
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///   Fast Retina Keypoint (FREAK) descriptor.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   Based on original implementation by A. Alahi, R. Ortiz, and P. 
    ///   Vandergheynst, distributed under a BSD style license.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       A. Alahi, R. Ortiz, and P. Vandergheynst. FREAK: Fast Retina Keypoint. In IEEE Conference on 
    ///       Computer Vision and Pattern Recognition, CVPR 2012 Open Source Award Winner.</description></item>
    ///   </list>
    /// </para>
    /// </remarks>
    ///
    /// <seealso cref="FastRetinaKeypoint"/>
    /// <seealso cref="FastRetinaKeypointDetector"/>
    ///
    public class FastRetinaKeypointDescriptor
    {
        private FastRetinaKeypointPattern pattern;

        /// <summary>
        ///   Gets or sets whether the orientation is normalized.
        /// </summary>
        /// 
        public bool IsOrientationNormal { get; set; }

        /// <summary>
        ///   Gets or sets whether the scale is normalized.
        /// </summary>
        /// 
        public bool IsScaleNormal { get; set; }

        /// <summary>
        ///   Gets or sets whether to compute the standard 512-bit 
        ///   descriptors or extended 1024-bit 
        /// </summary>
        /// 
        public bool Extended { get; set; }

        /// <summary>
        ///   Gets the <see cref="UnmanagedImage"/> of
        ///   the original source's feature detector.
        /// </summary>
        /// 
        /// <value>The integral image from where the
        /// features have been detected.</value>
        /// 
        public UnmanagedImage Image { get; private set; }

        /// <summary>
        ///   Gets the <see cref="IntegralImage"/> of
        ///   the original source's feature detector.
        /// </summary>
        /// 
        /// <value>The integral image from where the
        /// features have been detected.</value>
        /// 
        public IntegralImage Integral { get; private set; }


        /// <summary>
        ///   Initializes a new instance of the <see cref="FastRetinaKeypointDescriptor"/> class.
        /// </summary>
        /// 
        internal FastRetinaKeypointDescriptor(UnmanagedImage image,
            IntegralImage integral, FastRetinaKeypointPattern pattern)
        {
            this.Extended = false;
            this.IsOrientationNormal = true;
            this.IsScaleNormal = true;
            this.Image = image;
            this.Integral = integral;

            this.pattern = pattern;
        }


        /// <summary>
        ///   Describes the specified point (i.e. computes and
        ///   sets the orientation and descriptor vector fields
        ///   of the <see cref="FastRetinaKeypoint"/>.
        /// </summary>
        /// 
        /// <param name="points">The point to be described.</param>
        /// 
        public void Compute(IList<FastRetinaKeypoint> points)
        {
            const int CV_FREAK_SMALLEST_KP_SIZE = FastRetinaKeypointPattern.Size;
            const int CV_FREAK_NB_SCALES = FastRetinaKeypointPattern.Scales;
            const int CV_FREAK_NB_ORIENTATION = FastRetinaKeypointPattern.Orientations;

            var patternSizes = pattern.patternSizes;
            var pointsValues = pattern.pointsValues;
            var orientationPairs = pattern.orientationPairs;
            var descriptionPairs = pattern.descriptionPairs;
            var step = pattern.step;


            // used to save pattern scale index corresponding to each keypoints
            var scaleIndex = new List<int>(points.Count);
            for (int i = 0; i < points.Count; i++)
                scaleIndex.Add(0);


            // 1. Compute the scale index corresponding to the keypoint
            //  size and remove keypoints which are close to the border
            //
            if (IsScaleNormal)
            {
                for (int k = points.Count - 1; k >= 0; k--)
                {
                    // Is k non-zero? If so, decrement it and continue.
                    double ratio = points[k].Scale / CV_FREAK_SMALLEST_KP_SIZE;
                    scaleIndex[k] = Math.Max((int)(Math.Log(ratio) * step + 0.5), 0);

                    if (scaleIndex[k] >= CV_FREAK_NB_SCALES)
                        scaleIndex[k] = CV_FREAK_NB_SCALES - 1;

                    // Check if the description at this position and scale fits inside the image
                    if (points[k].X <= patternSizes[scaleIndex[k]] ||
                         points[k].Y <= patternSizes[scaleIndex[k]] ||
                         points[k].X >= Image.Width - patternSizes[scaleIndex[k]] ||
                         points[k].Y >= Image.Height - patternSizes[scaleIndex[k]])
                    {
                        points.RemoveAt(k);  // No, it doesn't. Remove the point.
                        scaleIndex.RemoveAt(k);
                    }
                }
            }

            else // if (!IsScaleNormal)
            {
                int scale = Math.Max((int)(Constants.Log3 * step + 0.5), 0);

                for (int k = points.Count - 1; k >= 0; k--)
                {
                    // equivalent to the formulae when the scale is normalized with 
                    // a constant size of keypoints[k].size = 3 * SMALLEST_KP_SIZE

                    scaleIndex[k] = scale;
                    if (scaleIndex[k] >= CV_FREAK_NB_SCALES)
                        scaleIndex[k] = CV_FREAK_NB_SCALES - 1;

                    if (points[k].X <= patternSizes[scaleIndex[k]] ||
                        points[k].Y <= patternSizes[scaleIndex[k]] ||
                        points[k].X >= Image.Width - patternSizes[scaleIndex[k]] ||
                        points[k].Y >= Image.Height - patternSizes[scaleIndex[k]])
                    {
                        points.RemoveAt(k);
                        scaleIndex.RemoveAt(k);
                    }
                }
            }


            // 2. Allocate descriptor memory, estimate 
            //    orientations, and extract descriptors
            //

            // For each interest (key/corners) point
            for (int k = 0; k < points.Count; k++)
            {
                int thetaIndex = 0;

                // Estimate orientation
                if (!IsOrientationNormal)
                {
                    // Orientation is not normalized, assign 0.
                    points[k].Orientation = thetaIndex = 0;
                }

                else // if (IsOrientationNormal)
                {
                    // Get intensity values in the unrotated patch
                    for (int i = 0; i < pointsValues.Length; i++)
                        pointsValues[i] = mean(points[k].X, points[k].Y, scaleIndex[k], 0, i);

                    int a = 0, b = 0;
                    for (int m = 0; m < orientationPairs.Length; m++)
                    {
                        var p = orientationPairs[m];
                        int delta = (pointsValues[p.i] - pointsValues[p.j]);
                        a += delta * (p.weight_dx) / 2048;
                        b += delta * (p.weight_dy) / 2048;
                    }

                    points[k].Orientation = Math.Atan2(b, a) * (180.0 / Math.PI);
                    thetaIndex = (int)(CV_FREAK_NB_ORIENTATION * points[k].Orientation * (1 / 360.0) + 0.5);

                    if (thetaIndex < 0) // bound in interval
                        thetaIndex += CV_FREAK_NB_ORIENTATION;
                    if (thetaIndex >= CV_FREAK_NB_ORIENTATION)
                        thetaIndex -= CV_FREAK_NB_ORIENTATION;
                }

                // Extract descriptor at the computed orientation
                for (int i = 0; i < pointsValues.Length; i++)
                    pointsValues[i] = mean(points[k].X, points[k].Y, scaleIndex[k], thetaIndex, i);


                // Extract either the standard descriptors of 512-bits (64 bytes)
                //   or the extended descriptors of 1024-bits (128 bytes) length.
                //
                if (!Extended)
                {
                    points[k].Descriptor = new byte[64];
                    for (int m = 0; m < descriptionPairs.Length; m++)
                    {
                        var p = descriptionPairs[m];
                        byte[] descriptor = points[k].Descriptor;

                        if (pointsValues[p.i] > pointsValues[p.j])
                            descriptor[m / 8] |= (byte)(1 << m % 8);
                        else descriptor[m / 8] &= (byte)~(1 << m % 8);

                    }
                }

                else // if (Extended)
                {
                    points[k].Descriptor = new byte[128];
                    for (int i = 1, m = 0; i < pointsValues.Length; i++)
                    {
                        for (int j = 0; j < i; j++, m++)
                        {
                            byte[] descriptor = points[k].Descriptor;

                            if (pointsValues[i] > pointsValues[j])
                                descriptor[m / 8] |= (byte)(1 << m % 8);
                            else descriptor[m / 8] &= (byte)~(1 << m % 8);
                        }
                    }
                }
            }
        }


        private unsafe int mean(double kx, double ky, int scale, int orientation, int pointIndex)
        {
            const int CV_FREAK_NB_ORIENTATION = FastRetinaKeypointPattern.Orientations;
            const int CV_FREAK_NB_POINTS = FastRetinaKeypointPattern.Points;

            // get point position in image
            var freak = pattern.lookupTable[
                scale * CV_FREAK_NB_ORIENTATION * CV_FREAK_NB_POINTS 
                + orientation * CV_FREAK_NB_POINTS + pointIndex];

            double xf = freak.x + kx;
            double yf = freak.y + ky;
            int x = (int)(xf);
            int y = (int)(yf);
            int imagecols = Image.Width;
            int ret_val;

            // get the sigma:
            float radius = freak.sigma;

            // calculate output:
            if (radius < 0.5)
            {
                // interpolation multipliers:
                int r_x = (int)((xf - x) * 1024);
                int r_y = (int)((yf - y) * 1024);
                int r_x_1 = (1024 - r_x);
                int r_y_1 = (1024 - r_y);
                byte* ptr = (byte*)Image.ImageData.ToPointer() + x + y * imagecols;

                // linear interpolation:
                ret_val = (r_x_1 * r_y_1 * (int)(*ptr));
                ptr++;
                ret_val += (r_x * r_y_1 * (int)(*ptr));
                ptr += imagecols;
                ret_val += (r_x * r_y * (int)(*ptr));
                ptr--;
                ret_val += (r_x_1 * r_y * (int)(*ptr));
                return ((ret_val + 512) / 1024);
            }


            // calculate borders
            int x_left = (int)(xf - radius + 0.5);
            int y_top = (int)(yf - radius + 0.5);
            int x_right = (int)(xf + radius + 1.5);  //integral image is 1px wider
            int y_bottom = (int)(yf + radius + 1.5); //integral image is 1px higher

            ret_val = (int)Integral.InternalData[y_bottom, x_right]; //bottom right corner
            ret_val -= (int)Integral.InternalData[y_bottom, x_left];
            ret_val += (int)Integral.InternalData[y_top, x_left];
            ret_val -= (int)Integral.InternalData[y_top, x_right];
            ret_val = ret_val / ((x_right - x_left) * (y_bottom - y_top));
            return ret_val;
        }
       
    }
}
