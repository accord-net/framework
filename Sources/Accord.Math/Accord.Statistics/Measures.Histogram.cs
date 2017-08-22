// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// AForge Math Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2011
// contacts@aforgenet.com
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

namespace Accord.Statistics
{
    using Accord;
    using Accord.Math;
    using System;


    public static partial class Measures
    {
        /// <summary>
        ///   Gets the minimum value in the histogram.
        /// </summary>
        /// 
        /// <param name="values">Histogram array.</param>
        /// 
        /// <returns>The minimum value in the histogram.</returns>
        /// 
        public static int HistogramMin(this int[] values)
        {
            int min = values.Length;

            for (int i = 0; i < values.Length; i++)
                if (i < min)
                    min = i;

            return min;
        }

        /// <summary>
        ///   Gets the maximum value in the histogram.
        /// </summary>
        /// 
        /// <param name="values">Histogram array.</param>
        /// 
        /// <returns>The maximum value in the histogram.</returns>
        /// 
        public static int HistogramMax(this int[] values)
        {
            int max = 0;

            for (int i = 0; i < values.Length; i++)
                if (i > max)
                    max = i;

            return max;
        }

        /// <summary>
        ///   Calculates the total number of samples in a histogram.
        /// </summary>
        /// 
        /// <param name="values">The histogram array.</param>
        /// 
        /// <returns>The total number of samples in the histogram.</returns>
        /// 
        public static long HistogramSum(this int[] values)
        {
            long sum = 0;
            for (int i = 0; i < values.Length; i++)
                sum += values[i];
            return sum;
        }


        /// <summary>
        /// Calculate mean value of an histogram.
        /// </summary>
        /// 
        /// <param name="values">Histogram array.</param>
        /// 
        /// <returns>Returns mean value.</returns>
        /// 
        /// <remarks><para>The input array is treated as histogram, i.e. its
        /// indexes are treated as values of stochastic function, but
        /// array values are treated as "probabilities" (total amount of
        /// hits).</para>
        /// </remarks>
        /// 
        public static double HistogramMean(this int[] values)
        {
            double total = 0;
            double mean = 0;

            for (int i = 0; i < values.Length; i++)
            {
                int hits = values[i];
                mean += (double)i * hits;
                total += hits;
            }

            return (total == 0) ? 0 : mean / total;
        }

        /// <summary>
        /// Calculate standard deviation of an histogram.
        /// </summary>
        /// 
        /// <param name="values">Histogram array.</param>
        /// 
        /// <returns>Returns value of standard deviation.</returns>
        /// 
        /// <remarks><para>The input array is treated as histogram, i.e. its
        /// indexes are treated as values of stochastic function, but
        /// array values are treated as "probabilities" (total amount of
        /// hits).</para>
        /// </remarks>
        /// 
        public static double HistogramStandardDeviation(this int[] values)
        {
            return StandardDeviation(values, Mean(values));
        }

        /// <summary>
        /// Calculate standard deviation of an histogram.
        /// </summary>
        /// 
        /// <param name="values">Histogram array.</param>
        /// <param name="mean">Mean value of the histogram.</param>
        /// 
        /// <returns>Returns value of standard deviation.</returns>
        /// 
        /// <remarks><para>The input array is treated as histogram, i.e. its
        /// indexes are treated as values of stochastic function, but
        /// array values are treated as "probabilities" (total amount of
        /// hits).</para>
        /// 
        /// <para>The method is an equivalent to the <see cref="HistogramStandardDeviation(int[])"/> method,
        /// but it relies on the passed mean value, which is previously calculated
        /// using <see cref="HistogramMean"/> method.</para>
        /// </remarks>
        /// 
        public static double HistogramStandardDeviation(this int[] values, double mean)
        {
            double stddev = 0;
            int total = 0;

            // for all values
            for (int i = 0; i < values.Length; i++)
            {
                int hits = values[i];
                double diff = (double)i - mean;
                stddev += diff * diff * hits;
                total += hits;
            }

            return (total == 0) ? 0 : Math.Sqrt(stddev / total);
        }

        /// <summary>
        /// Calculate median value of an histogram.
        /// </summary>
        /// 
        /// <param name="values">Histogram array.</param>
        /// 
        /// <returns>Returns value of median.</returns>
        /// 
        /// <remarks>
        /// <para>The input array is treated as histogram, i.e. its
        /// indexes are treated as values of stochastic function, but
        /// array values are treated as "probabilities" (total amount of
        /// hits).</para>
        /// 
        /// <para><note>The median value is calculated accumulating histogram's
        /// values starting from the <b>left</b> point until the sum reaches 50% of
        /// histogram's sum.</note></para>
        /// </remarks>
        /// 
        public static int HistogramMedian(this int[] values)
        {
            int total = 0;

            // for all values
            for (int i = 0; i < values.Length; i++)
            {
                // accumulate total
                total += values[i];
            }

            int halfTotal = total / 2;
            int median = 0;
            int v = 0;

            // find median value
            for (; median < values.Length; median++)
            {
                v += values[median];
                if (v >= halfTotal)
                    break;
            }

            return median;
        }

        /// <summary>
        /// Get range around median of an histogram containing specified percentage of values.
        /// </summary>
        /// 
        /// <param name="values">Histogram array.</param>
        /// <param name="percent">Values percentage around median.</param>
        /// 
        /// <returns>Returns the range which contains specified percentage
        /// of values.</returns>
        /// 
        /// <remarks>
        /// <para>The input array is treated as histogram, i.e. its
        /// indexes are treated as values of stochastic function, but
        /// array values are treated as "probabilities" (total amount of
        /// hits).</para>
        /// 
        /// <para>The method calculates range of stochastic variable, which summary probability
        /// comprises the specified percentage of histogram's hits.</para>
        /// </remarks>
        /// 
        public static IntRange GetHistogramRange(this int[] values, double percent)
        {
            int total = 0;

            // for all values
            for (int i = 0; i < values.Length; i++)
            {
                // accumulate total
                total += values[i];
            }

            int min, max, hits;
            int h = (int)(total * (percent + (1 - percent) / 2));

            // get range min value
            for (min = 0, hits = total; min < values.Length; min++)
            {
                hits -= values[min];
                if (hits < h)
                    break;
            }

            // get range max value
            for (max = values.Length - 1, hits = total; max >= 0; max--)
            {
                hits -= values[max];
                if (hits < h)
                    break;
            }

            return new IntRange(min, max);
        }

        /// <summary>
        /// Calculate entropy value of an histogram.
        /// </summary>
        /// 
        /// <param name="values">Histogram array.</param>
        /// 
        /// <returns>Returns entropy value of the specified histogram array.</returns>
        /// 
        /// <remarks><para>The input array is treated as histogram, i.e. its
        /// indexes are treated as values of stochastic function, but
        /// array values are treated as "probabilities" (total amount of
        /// hits).</para>
        /// </remarks>
        /// 
        public static double HistogramEntropy(this int[] values)
        {
            int total = 0;
            double entropy = 0;
            double p;

            // calculate total amount of hits
            for (int i = 0; i < values.Length; i++)
                total += values[i];

            if (total != 0)
            {
                // for all values
                for (int i = 0; i < values.Length; i++)
                {
                    // get item's probability
                    p = (double)values[i] / total;
                    // calculate entropy
                    if (p != 0)
                        entropy += (-p * Math.Log(p, 2));
                }
            }

            return entropy;
        }

        /// <summary>
        /// Calculate mode value of an histogram.
        /// </summary>
        /// 
        /// <param name="values">Histogram array.</param>
        /// 
        /// <returns>Returns mode value of the histogram array.</returns>
        /// 
        /// <remarks>
        /// <para>The input array is treated as histogram, i.e. its
        /// indexes are treated as values of stochastic function, but
        /// array values are treated as "probabilities" (total amount of
        /// hits).</para>
        /// 
        /// <para><note>Returns the minimum mode value if the specified histogram is multimodal.</note></para>
        /// </remarks>
        /// 
        public static int HistogramMode(this int[] values)
        {
            int mode = 0, curMax = 0;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] > curMax)
                {
                    curMax = values[i];
                    mode = i;
                }
            }

            return mode;
        }
    }
}
