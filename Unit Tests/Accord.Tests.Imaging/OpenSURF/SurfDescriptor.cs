using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenSURFcs
{
    public class SurfDescriptor
    {

        /// <summary>
        /// Gaussian distribution with sigma = 2.5.  Used as a fast lookup
        /// </summary>
        float[,] gauss25 = new float[7, 7] {
      {0.02350693969273f,0.01849121369071f,0.01239503121241f,0.00708015417522f,0.00344628101733f,0.00142945847484f,0.00050524879060f},
      {0.02169964028389f,0.01706954162243f,0.01144205592615f,0.00653580605408f,0.00318131834134f,0.00131955648461f,0.00046640341759f},
      {0.01706954162243f,0.01342737701584f,0.00900063997939f,0.00514124713667f,0.00250251364222f,0.00103799989504f,0.00036688592278f},
      {0.01144205592615f,0.00900063997939f,0.00603330940534f,0.00344628101733f,0.00167748505986f,0.00069579213743f,0.00024593098864f},
      {0.00653580605408f,0.00514124713667f,0.00344628101733f,0.00196854695367f,0.00095819467066f,0.00039744277546f,0.00014047800980f},
      {0.00318131834134f,0.00250251364222f,0.00167748505986f,0.00095819467066f,0.00046640341759f,0.00019345616757f,0.00006837798818f},
      {0.00131955648461f,0.00103799989504f,0.00069579213743f,0.00039744277546f,0.00019345616757f,0.00008024231247f,0.00002836202103f}
    };

        /// <summary>
        /// The integral image which is being used
        /// </summary>
        IntegralImage img;

        /// <summary>
        /// Static one-call do it all function
        /// </summary>
        /// <param name="img"></param>
        /// <param name="ipts"></param>
        /// <param name="extended"></param>
        /// <param name="upright"></param>
        public static void DecribeInterestPoints(List<IPoint> ipts, bool upright, bool extended, IntegralImage img)
        {
            SurfDescriptor des = new SurfDescriptor();
            des.DescribeInterestPoints(ipts, upright, extended, img);
        }


        /// <summary>
        /// Build descriptor vector for each interest point in the supplied list
        /// </summary>
        /// <param name="img"></param>
        /// <param name="ipts"></param>
        /// <param name="upright"></param>
        public void DescribeInterestPoints(List<IPoint> ipts, bool upright, bool extended, IntegralImage img)
        {
            if (ipts.Count == 0) return;
            this.img = img;

            foreach (IPoint ip in ipts)
            {
                // determine descriptor size
                if (extended) ip.descriptorLength = 128;
                else ip.descriptorLength = 64;

                // if we want rotation invariance get the orientation
                if (!upright) GetOrientation(ip);

                // Extract SURF descriptor
                GetDescriptor(ip, upright, extended);
            }
        }

        /// <summary>
        /// Determine dominant orientation for InterestPoint
        /// </summary>
        /// <param name="ip"></param>
        void GetOrientation(IPoint ip)
        {
            const byte Responses = 109;
            float[] resX = new float[Responses];
            float[] resY = new float[Responses];
            float[] Ang = new float[Responses];
            int idx = 0;
            int[] id = { 6, 5, 4, 3, 2, 1, 0, 1, 2, 3, 4, 5, 6 };

            // Get rounded InterestPoint data
            int X = (int)Math.Round(ip.x, 0);
            int Y = (int)Math.Round(ip.y, 0);
            int S = (int)Math.Round(ip.scale, 0);

            // calculate Haar responses for points within radius of 6*scale
            for (int i = -6; i <= 6; ++i)
            {
                for (int j = -6; j <= 6; ++j)
                {
                    if (i * i + j * j < 36)
                    {
                        float gauss = gauss25[id[i + 6], id[j + 6]];
                        resX[idx] = gauss * img.HaarX(Y + j * S, X + i * S, 4 * S);
                        resY[idx] = gauss * img.HaarY(Y + j * S, X + i * S, 4 * S);
                        Ang[idx] = (float)GetAngle(resX[idx], resY[idx]);
                        ++idx;
                    }
                }
            }

            // calculate the dominant direction 
            float sumX, sumY, max = 0, orientation = 0;
            float ang1, ang2;
            float pi = (float)Math.PI;

            // loop slides pi/3 window around feature point
            for (ang1 = 0; ang1 < 2 * pi; ang1 += 0.15f)
            {
                ang2 = (ang1 + pi / 3f > 2 * pi ? ang1 - 5 * pi / 3f : ang1 + pi / 3f);
                sumX = sumY = 0;

                for (int k = 0; k < Responses; ++k)
                {
                    // determine whether the point is within the window
                    if (ang1 < ang2 && ang1 < Ang[k] && Ang[k] < ang2)
                    {
                        sumX += resX[k];
                        sumY += resY[k];
                    }
                    else if (ang2 < ang1 &&
                      ((Ang[k] > 0 && Ang[k] < ang2) || (Ang[k] > ang1 && Ang[k] < pi)))
                    {
                        sumX += resX[k];
                        sumY += resY[k];
                    }
                }

                // if the vector produced from this window is longer than all 
                // previous vectors then this forms the new dominant direction
                if (sumX * sumX + sumY * sumY > max)
                {
                    // store largest orientation
                    max = sumX * sumX + sumY * sumY;
                    orientation = (float)GetAngle(sumX, sumY);
                }
            }

            // assign orientation of the dominant response vector
            ip.orientation = (float)orientation;
        }


        /// <summary>
        /// Construct descriptor vector for this interest point
        /// </summary>
        /// <param name="bUpright"></param>
        void GetDescriptor(IPoint ip, bool bUpright, bool bExtended)
        {
            int sample_x, sample_y, count = 0;
            int i = 0, ix = 0, j = 0, jx = 0, xs = 0, ys = 0;
            float dx, dy, mdx, mdy, co, si;
            float dx_yn, mdx_yn, dy_xn, mdy_xn;
            float gauss_s1 = 0f, gauss_s2 = 0f;
            float rx = 0f, ry = 0f, rrx = 0f, rry = 0f, len = 0f;
            float cx = -0.5f, cy = 0f; // Subregion centers for the 4x4 Gaussian weighting

            // Get rounded InterestPoint data
            int X = (int)Math.Round(ip.x, 0);
            int Y = (int)Math.Round(ip.y, 0);
            int S = (int)Math.Round(ip.scale, 0);

            // Allocate descriptor memory
            ip.SetDescriptorLength(bExtended ? 128 : 64);

            if (bUpright)
            {
                co = 1;
                si = 0;
            }
            else
            {
                co = (float)Math.Cos(ip.orientation);
                si = (float)Math.Sin(ip.orientation);
            }

            //Calculate descriptor for this interest point
            i = -8;
            while (i < 12)
            {
                j = -8;
                i = i - 4;

                cx += 1f;
                cy = -0.5f;

                while (j < 12)
                {
                    cy += 1f;

                    j = j - 4;

                    ix = i + 5;
                    jx = j + 5;

                    dx = dy = mdx = mdy = 0f;
                    dx_yn = mdx_yn = dy_xn = mdy_xn = 0f;

                    xs = (int)Math.Round(X + (-jx * S * si + ix * S * co), 0);
                    ys = (int)Math.Round(Y + (jx * S * co + ix * S * si), 0);

                    // zero the responses
                    dx = dy = mdx = mdy = 0f;
                    dx_yn = mdx_yn = dy_xn = mdy_xn = 0f;

                    for (int k = i; k < i + 9; ++k)
                    {
                        for (int l = j; l < j + 9; ++l)
                        {
                            //Get coords of sample point on the rotated axis
                            sample_x = (int)Math.Round(X + (-l * S * si + k * S * co), 0);
                            sample_y = (int)Math.Round(Y + (l * S * co + k * S * si), 0);

                            //Get the Gaussian weighted x and y responses
                            gauss_s1 = Gaussian(xs - sample_x, ys - sample_y, 2.5f * S);
                            rx = (float)img.HaarX(sample_y, sample_x, 2 * S);
                            ry = (float)img.HaarY(sample_y, sample_x, 2 * S);

                            //Get the Gaussian weighted x and y responses on rotated axis
                            rrx = gauss_s1 * (-rx * si + ry * co);
                            rry = gauss_s1 * (rx * co + ry * si);


                            if (bExtended)
                            {
                                // split x responses for different signs of y
                                if (rry >= 0)
                                {
                                    dx += rrx;
                                    mdx += Math.Abs(rrx);
                                }
                                else
                                {
                                    dx_yn += rrx;
                                    mdx_yn += Math.Abs(rrx);
                                }

                                // split y responses for different signs of x
                                if (rrx >= 0)
                                {
                                    dy += rry;
                                    mdy += Math.Abs(rry);
                                }
                                else
                                {
                                    dy_xn += rry;
                                    mdy_xn += Math.Abs(rry);
                                }
                            }
                            else
                            {
                                dx += rrx;
                                dy += rry;
                                mdx += Math.Abs(rrx);
                                mdy += Math.Abs(rry);
                            }
                        }
                    }

                    //Add the values to the descriptor vector
                    gauss_s2 = Gaussian(cx - 2f, cy - 2f, 1.5f);

                    ip.descriptor[count++] = dx * gauss_s2;
                    ip.descriptor[count++] = dy * gauss_s2;
                    ip.descriptor[count++] = mdx * gauss_s2;
                    ip.descriptor[count++] = mdy * gauss_s2;

                    // add the extended components
                    if (bExtended)
                    {
                        ip.descriptor[count++] = dx_yn * gauss_s2;
                        ip.descriptor[count++] = dy_xn * gauss_s2;
                        ip.descriptor[count++] = mdx_yn * gauss_s2;
                        ip.descriptor[count++] = mdy_xn * gauss_s2;
                    }

                    len += (dx * dx + dy * dy + mdx * mdx + mdy * mdy
                            + dx_yn + dy_xn + mdx_yn + mdy_xn) * gauss_s2 * gauss_s2;

                    j += 9;
                }
                i += 9;
            }

            //Convert to Unit Vector
            len = (float)Math.Sqrt((double)len);
            if (len > 0)
            {
                for (int d = 0; d < ip.descriptorLength; ++d)
                {
                    ip.descriptor[d] /= len;
                }
            }
        }


        /// <summary>
        /// Get the angle formed by the vector [x,y]
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        double GetAngle(float X, float Y)
        {
            if (X >= 0 && Y >= 0)
                return Math.Atan(Y / X);
            else if (X < 0 && Y >= 0)
                return Math.PI - Math.Atan(-Y / X);
            else if (X < 0 && Y < 0)
                return Math.PI + Math.Atan(Y / X);
            else if (X >= 0 && Y < 0)
                return 2 * Math.PI - Math.Atan(-Y / X);

            return 0;
        }


        /// <summary>
        /// Get the value of the Gaussian with std dev sigma
        /// at the point (x,y)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="sig"></param>
        /// <returns></returns>
        float Gaussian(int x, int y, float sig)
        {
            float pi = (float)Math.PI;
            return (1f / (2f * pi * sig * sig)) * (float)Math.Exp(-(x * x + y * y) / (2.0f * sig * sig));
        }


        /// <summary>
        /// Get the value of the Gaussian with std dev sigma
        /// at the point (x,y)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="sig"></param>
        /// <returns></returns>
        float Gaussian(float x, float y, float sig)
        {
            float pi = (float)Math.PI;
            return 1f / (2f * pi * sig * sig) * (float)Math.Exp(-(x * x + y * y) / (2.0f * sig * sig));
        }


    } // SurfDescriptor
} // OpenSURFcs
