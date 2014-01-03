// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Christopher Evans, 2009-2011
// http://www.chrisevansdev.com/
//
// Copyright © César Souza, 2009-2014
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
    using System.Collections.Generic;
    using AForge.Imaging;

    /// <summary>
    ///   Speeded-Up Robust Features (SURF) Descriptor.
    /// </summary>
    /// 
    /// <seealso cref="SpeededUpRobustFeaturesDetector"/>
    /// <seealso cref="SpeededUpRobustFeaturePoint"/>
    ///
    public class SpeededUpRobustFeaturesDescriptor : ICloneable
    {

        private bool invariant = true;
        private bool extended = false;
        private IntegralImage integral;

        /// <summary>
        ///   Gets or sets a value indicating whether the features
        ///   described by this <see cref="SpeededUpRobustFeaturesDescriptor"/> should
        ///   be invariant to rotation. Default is true.
        /// </summary>
        /// 
        /// <value><c>true</c> for rotation invariant features; <c>false</c> otherwise.</value>
        /// 
        public bool Invariant
        {
            get { return invariant; }
            set { invariant = value; }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether the features
        ///   described by this <see cref="SpeededUpRobustFeaturesDescriptor"/> should
        ///   be computed in extended form. Default is false.
        /// </summary>
        /// 
        /// <value><c>true</c> for extended features; <c>false</c> otherwise.</value>
        /// 
        public bool Extended
        {
            get { return extended; }
            set { extended = value; }
        }

        /// <summary>
        ///   Gets the <see cref="IntegralImage"/> of
        ///   the original source's feature detector.
        /// </summary>
        /// 
        /// <value>The integral image from where the
        /// features have been detected.</value>
        /// 
        public IntegralImage Image
        {
            get { return integral; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SpeededUpRobustFeaturesDescriptor"/> class.
        /// </summary>
        /// 
        /// <param name="integralImage">
        ///   The integral image which is the source of the feature points.
        /// </param>
        /// 
        public SpeededUpRobustFeaturesDescriptor(IntegralImage integralImage)
        {
            this.integral = integralImage;
        }


        /// <summary>
        ///   Describes the specified point (i.e. computes and
        ///   sets the orientation and descriptor vector fields
        ///   of the <see cref="SpeededUpRobustFeaturePoint"/>.
        /// </summary>
        /// 
        /// <param name="point">The point to be described.</param>
        /// 
        public void Compute(SpeededUpRobustFeaturePoint point)
        {
            // Get rounded feature point data
            int x = (int)System.Math.Round(point.X, 0);
            int y = (int)System.Math.Round(point.Y, 0);
            int s = (int)System.Math.Round(point.Scale, 0);

            if (this.invariant)
            {
                // Get the orientation (for rotation invariance)
                point.Orientation = this.GetOrientation(x, y, s);
            }

            // Extract SURF descriptor
            point.Descriptor = this.GetDescriptor(x, y, s, point.Orientation);
        }

        /// <summary>
        ///   Describes all specified points (i.e. computes and
        ///   sets the orientation and descriptor vector fields
        ///   of each <see cref="SpeededUpRobustFeaturePoint"/>.
        /// </summary>
        /// 
        /// <param name="points">The list of points to be described.</param>
        /// 
        public void Compute(IEnumerable<SpeededUpRobustFeaturePoint> points)
        {
            foreach (SpeededUpRobustFeaturePoint point in points)
            {
                Compute(point);
            }
        }

        /// <summary>
        ///   Determine dominant orientation for the feature point.
        /// </summary>
        /// 
        public double GetOrientation(SpeededUpRobustFeaturePoint point)
        {
            // Get rounded feature point data
            int x = (int)System.Math.Round(point.X, 0);
            int y = (int)System.Math.Round(point.Y, 0);
            int s = (int)System.Math.Round(point.Scale, 0);

            // Get the orientation (for rotation invariance)
            return this.GetOrientation(x, y, s);
        }

        const byte responses = 109;
        readonly double[] resX = new double[responses];
        readonly double[] resY = new double[responses];
        readonly double[] ang = new double[responses];
        static int[] id = { 6, 5, 4, 3, 2, 1, 0, 1, 2, 3, 4, 5, 6 };

        /// <summary>
        ///   Determine dominant orientation for feature point.
        /// </summary>
        /// 
        public double GetOrientation(int x, int y, int scale)
        {
            // Calculate Haar responses for points within radius of 6*scale
            for (int i = -6, idx = 0; i <= 6; i++)
            {
                for (int j = -6; j <= 6; j++)
                {
                    if (i * i + j * j < 36)
                    {
                        double g = gauss25[id[i + 6], id[j + 6]];
                        resX[idx] = g * haarX(y + j * scale, x + i * scale, 4 * scale);
                        resY[idx] = g * haarY(y + j * scale, x + i * scale, 4 * scale);
                        ang[idx] = Accord.Math.Tools.Angle(resX[idx], resY[idx]);
                        idx++;
                    }
                }
            }

            // Calculate the dominant direction 
            double orientation = 0, max = 0;

            // Loop slides pi/3 window around feature point
            for (double ang1 = 0; ang1 < 2.0 * Math.PI; ang1 += 0.15)
            {
                double ang2 = (ang1 + Math.PI / 3 > 2 * Math.PI ? ang1 - 5 * Math.PI / 3 : ang1 + Math.PI / 3);
                double sumX = 0;
                double sumY = 0;

                for (int k = 0; k < ang.Length; k++)
                {
                    // determine whether the point is within the window
                    if (ang1 < ang2 && ang1 < ang[k] && ang[k] < ang2)
                    {
                        sumX += resX[k];
                        sumY += resY[k];
                    }
                    else if (ang2 < ang1 && ((ang[k] > 0 && ang[k] < ang2) || (ang[k] > ang1 && ang[k] < Math.PI)))
                    {
                        sumX += resX[k];
                        sumY += resY[k];
                    }
                }

                // If the vector produced from this window is longer than all 
                // previous vectors then this forms the new dominant direction
                if (sumX * sumX + sumY * sumY > max)
                {
                    // store largest orientation
                    max = sumX * sumX + sumY * sumY;
                    orientation = Accord.Math.Tools.Angle(sumX, sumY);
                }
            }

            // Return orientation of the 
            // dominant response vector
            return orientation;
        }

        /// <summary>
        ///   Construct descriptor vector for this interest point
        /// </summary>
        /// 
        public double[] GetDescriptor(int x, int y, int scale, double orientation)
        {
            // Determine descriptor size
            double[] descriptor = (this.extended) ? new double[128] : new double[64];

            int count = 0;
            double cos, sin;
            double length = 0;

            double cx = -0.5; // Subregion centers for the
            double cy = +0.0; // 4x4 Gaussian weighting.

            if (!this.invariant)
            {
                cos = 1;
                sin = 0;
            }
            else
            {
                cos = System.Math.Cos(orientation);
                sin = System.Math.Sin(orientation);
            }

            // Calculate descriptor for this interest point
            int i = -8;
            while (i < 12)
            {
                int j = -8;
                i = i - 4;

                cx += 1f;
                cy = -0.5f;

                while (j < 12)
                {
                    cy += 1f;
                    j = j - 4;

                    int ix = i + 5;
                    int jx = j + 5;

                    int xs = (int)System.Math.Round(x + (-jx * scale * sin + ix * scale * cos), 0);
                    int ys = (int)System.Math.Round(y + (+jx * scale * cos + ix * scale * sin), 0);

                    // zero the responses
                    double dx = 0, dy = 0;
                    double mdx = 0, mdy = 0;
                    double dx_yn = 0, dy_xn = 0;
                    double mdx_yn = 0, mdy_xn = 0;

                    for (int k = i; k < i + 9; k++)
                    {
                        for (int l = j; l < j + 9; l++)
                        {
                            // Get coordinates of sample point on the rotated axis
                            int sample_x = (int)System.Math.Round(x + (-l * scale * sin + k * scale * cos), 0);
                            int sample_y = (int)System.Math.Round(y + (+l * scale * cos + k * scale * sin), 0);

                            // Get the Gaussian weighted x and y responses
                            double gauss_s1 = gaussian(xs - sample_x, ys - sample_y, 2.5f * scale);
                            double rx = haarX(sample_y, sample_x, 2 * scale);
                            double ry = haarY(sample_y, sample_x, 2 * scale);

                            // Get the Gaussian weighted x and y responses on rotated axis
                            double rrx = gauss_s1 * (-rx * sin + ry * cos);
                            double rry = gauss_s1 * (rx * cos + ry * sin);


                            if (this.extended)
                            {
                                // split x responses for different signs of y
                                if (rry >= 0)
                                {
                                    dx += rrx;
                                    mdx += System.Math.Abs(rrx);
                                }
                                else
                                {
                                    dx_yn += rrx;
                                    mdx_yn += System.Math.Abs(rrx);
                                }

                                // split y responses for different signs of x
                                if (rrx >= 0)
                                {
                                    dy += rry;
                                    mdy += System.Math.Abs(rry);
                                }
                                else
                                {
                                    dy_xn += rry;
                                    mdy_xn += System.Math.Abs(rry);
                                }
                            }
                            else
                            {
                                dx += rrx;
                                dy += rry;
                                mdx += System.Math.Abs(rrx);
                                mdy += System.Math.Abs(rry);
                            }
                        }
                    }

                    // Add the values to the descriptor vector
                    double gauss_s2 = gaussian(cx - 2.0, cy - 2.0, 1.5);

                    descriptor[count++] = dx * gauss_s2;
                    descriptor[count++] = dy * gauss_s2;
                    descriptor[count++] = mdx * gauss_s2;
                    descriptor[count++] = mdy * gauss_s2;

                    // Add the extended components
                    if (this.extended)
                    {
                        descriptor[count++] = dx_yn * gauss_s2;
                        descriptor[count++] = dy_xn * gauss_s2;
                        descriptor[count++] = mdx_yn * gauss_s2;
                        descriptor[count++] = mdy_xn * gauss_s2;
                    }

                    length += (dx * dx + dy * dy + mdx * mdx + mdy * mdy
                          + dx_yn + dy_xn + mdx_yn + mdy_xn) * gauss_s2 * gauss_s2;

                    j += 9;
                }
                i += 9;
            }

            // Normalize to obtain an unitary vector
            length = System.Math.Sqrt(length);

            if (length > 0)
            {
                for (int d = 0; d < descriptor.Length; ++d)
                    descriptor[d] /= length;
            }

            return descriptor;
        }

        private double haarX(int row, int column, int size)
        {
            double a = integral.GetRectangleSum(column, row - size / 2,
                column + size / 2 - 1, row - size / 2 + size - 1);

            double b = integral.GetRectangleSum(column - size / 2, row - size / 2,
                column - size / 2 + size / 2 - 1, row - size / 2 + size - 1);

            return (a - b) / 255.0;
        }

        private double haarY(int row, int column, int size)
        {
            double a = integral.GetRectangleSum(column - size / 2, row,
                column - size / 2 + size - 1, row + size / 2 - 1);

            double b = integral.GetRectangleSum(column - size / 2, row - size / 2,
                column - size / 2 + size - 1, row - size / 2 + size / 2 - 1);

            return (a - b) / 255.0;
        }



        #region Gaussian calculation

        /// <summary>
        ///   Get the value of the Gaussian with std dev sigma at the point (x,y)
        /// </summary>
        /// 
        private static double gaussian(int x, int y, double sigma)
        {
            return (1.0 / (2.0 * Math.PI * sigma * sigma)) * System.Math.Exp(-(x * x + y * y) / (2.0f * sigma * sigma));
        }

        /// <summary>
        ///   Get the value of the Gaussian with std dev sigma at the point (x,y)
        /// </summary>
        private static double gaussian(double x, double y, double sigma)
        {
            return 1.0 / (2.0 * Math.PI * sigma * sigma) * System.Math.Exp(-(x * x + y * y) / (2.0f * sigma * sigma));
        }

        /// <summary>
        ///   Gaussian look-up table for sigma = 2.5
        /// </summary>
        /// 
        private static readonly double[,] gauss25 = 
        {
            { 0.02350693969273, 0.01849121369071, 0.01239503121241, 0.00708015417522, 0.00344628101733, 0.00142945847484, 0.00050524879060 },
            { 0.02169964028389, 0.01706954162243, 0.01144205592615, 0.00653580605408, 0.00318131834134, 0.00131955648461, 0.00046640341759 },
            { 0.01706954162243, 0.01342737701584, 0.00900063997939, 0.00514124713667, 0.00250251364222, 0.00103799989504, 0.00036688592278 },
            { 0.01144205592615, 0.00900063997939, 0.00603330940534, 0.00344628101733, 0.00167748505986, 0.00069579213743, 0.00024593098864 },
            { 0.00653580605408, 0.00514124713667, 0.00344628101733, 0.00196854695367, 0.00095819467066, 0.00039744277546, 0.00014047800980 },
            { 0.00318131834134, 0.00250251364222, 0.00167748505986, 0.00095819467066, 0.00046640341759, 0.00019345616757, 0.00006837798818 },
            { 0.00131955648461, 0.00103799989504, 0.00069579213743, 0.00039744277546, 0.00019345616757, 0.00008024231247, 0.00002836202103 }
        };

        #endregion



        #region ICloneable Members

        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public object Clone()
        {
            var clone = new SpeededUpRobustFeaturesDescriptor(integral);
            clone.extended = extended;
            clone.invariant = invariant;

            return clone;
        }

        #endregion
    }
}
