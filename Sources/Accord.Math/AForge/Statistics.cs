// AForge Math Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2011
// contacts@aforgenet.com
//

namespace AForge.Math
{
    using System;
    using AForge;

    /// <summary>
    /// Set of statistics functions.
    /// </summary>
    /// 
    /// <remarks>The class represents collection of simple functions used
    /// in statistics.</remarks>
    /// 
    public static class Statistics
    {
        /// <summary>
        /// Calculate mean value.
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
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // create histogram array
        /// int[] histogram = new int[] { 1, 1, 2, 3, 6, 8, 11, 12, 7, 3 };
        /// // calculate mean value
        /// double mean = Statistics.Mean( histogram );
        /// // output it (5.759)
        /// Console.WriteLine( "mean = " + mean.ToString( "F3" ) );
        /// </code>
        /// </remarks>
        /// 
        public static double Mean( int[] values )
        {
            int     hits;
            double  total = 0;
            double  mean = 0;

            // for all values
            for ( int i = 0, n = values.Length; i < n; i++ )
            {
                hits = values[i];
                // accumulate mean
                mean += (double) i * hits;
                // accumalate total
                total += hits;
            }
            return ( total == 0 ) ? 0 : mean / total;
        }

        /// <summary>
        /// Calculate standard deviation.
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
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // create histogram array
        /// int[] histogram = new int[] { 1, 1, 2, 3, 6, 8, 11, 12, 7, 3 };
        /// // calculate standard deviation value
        /// double stdDev = Statistics.StdDev( histogram );
        /// // output it (1.999)
        /// Console.WriteLine( "std.dev. = " + stdDev.ToString( "F3" ) );
        /// </code>
        /// </remarks>
        /// 
        public static double StdDev( int[] values )
        {
            return StdDev( values, Mean( values ) );
        }

        /// <summary>
        /// Calculate standard deviation.
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
        /// <para>The method is an equevalent to the <see cref="StdDev(int[])"/> method,
        /// but it relieas on the passed mean value, which is previously calculated
        /// using <see cref="Mean"/> method.</para>
        /// </remarks>
        /// 
        public static double StdDev( int[] values, double mean )
        {
            double  stddev = 0;
            double  diff;
            int     hits;
            int     total = 0;

            // for all values
            for ( int i = 0, n = values.Length; i < n; i++ )
            {
                hits = values[i];
                diff = (double) i - mean;
                // accumulate std.dev.
                stddev += diff * diff * hits;
                // accumalate total
                total += hits;
            }

            return ( total == 0 ) ? 0 : Math.Sqrt( stddev / total );
        }

        /// <summary>
        /// Calculate median value.
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
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // create histogram array
        /// int[] histogram = new int[] { 1, 1, 2, 3, 6, 8, 11, 12, 7, 3 };
        /// // calculate median value
        /// int median = Statistics.Median( histogram );
        /// // output it (6)
        /// Console.WriteLine( "median = " + median );
        /// </code>
        /// </remarks>
        /// 
        public static int Median( int[] values )
        {
            int total = 0, n = values.Length;

            // for all values
            for ( int i = 0; i < n; i++ )
            {
                // accumalate total
                total += values[i];
            }

            int halfTotal = total / 2;
            int median = 0, v = 0;

            // find median value
            for ( ; median < n; median++ )
            {
                v += values[median];
                if ( v >= halfTotal )
                    break;
            }

            return median;
        }

        /// <summary>
        /// Get range around median containing specified percentage of values.
        /// </summary>
        /// 
        /// <param name="values">Histogram array.</param>
        /// <param name="percent">Values percentage around median.</param>
        /// 
        /// <returns>Returns the range which containes specifies percentage
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
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // create histogram array
        /// int[] histogram = new int[] { 1, 1, 2, 3, 6, 8, 11, 12, 7, 3 };
        /// // get 75% range around median
        /// IntRange range = Statistics.GetRange( histogram, 0.75 );
        /// // output it ([4, 8])
        /// Console.WriteLine( "range = [" + range.Min + ", " + range.Max + "]" );
        /// </code>
        /// </remarks>
        /// 
        public static IntRange GetRange( int[] values, double percent )
        {
            int total = 0, n = values.Length;

            // for all values
            for ( int i = 0; i < n; i++ )
            {
                // accumalate total
                total += values[i];
            }

            int min, max, hits;
            int h = (int) ( total * ( percent + ( 1 - percent ) / 2 ) );

            // get range min value
            for ( min = 0, hits = total; min < n; min++ )
            {
                hits -= values[min];
                if ( hits < h )
                    break;
            }
            // get range max value
            for ( max = n - 1, hits = total; max >= 0; max-- )
            {
                hits -= values[max];
                if ( hits < h )
                    break;
            }
            return new IntRange( min, max );
        }

        /// <summary>
        /// Calculate entropy value.
        /// </summary>
        /// 
        /// <param name="values">Histogram array.</param>
        /// 
        /// <returns>Returns entropy value of the specified histagram array.</returns>
        /// 
        /// <remarks><para>The input array is treated as histogram, i.e. its
        /// indexes are treated as values of stochastic function, but
        /// array values are treated as "probabilities" (total amount of
        /// hits).</para>
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // create histogram array with 2 values of equal probabilities
        /// int[] histogram1 = new int[2] { 3, 3 };
        /// // calculate entropy
        /// double entropy1 = Statistics.Entropy( histogram1 );
        /// // output it (1.000)
        /// Console.WriteLine( "entropy1 = " + entropy1.ToString( "F3" ) );
        /// 
        /// // create histogram array with 4 values of equal probabilities
        /// int[] histogram2 = new int[4] { 1, 1, 1, 1 };
        /// // calculate entropy
        /// double entropy2 = Statistics.Entropy( histogram2 );
        /// // output it (2.000)
        /// Console.WriteLine( "entropy2 = " + entropy2.ToString( "F3" ) );
        /// 
        /// // create histogram array with 4 values of different probabilities
        /// int[] histogram3 = new int[4] { 1, 2, 3, 4 };
        /// // calculate entropy
        /// double entropy3 = Statistics.Entropy( histogram3 );
        /// // output it (1.846)
        /// Console.WriteLine( "entropy3 = " + entropy3.ToString( "F3" ) );
        /// </code>
        /// </remarks>
        /// 
        public static double Entropy( int[] values )
        {
            int     n = values.Length;
            int     total = 0;
            double  entropy = 0;
            double  p;

            // calculate total amount of hits
            for ( int i = 0; i < n; i++ )
            {
                total += values[i];
            }

            if ( total != 0 )
            {
                // for all values
                for ( int i = 0; i < n; i++ )
                {
                    // get item's probability
                    p = (double) values[i] / total;
                    // calculate entropy
                    if ( p != 0 )
                        entropy += ( -p * Math.Log( p, 2 ) );
                }
            }
            return entropy;
        }

        /// <summary>
        /// Calculate mode value.
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
        ///
        /// <para>Sample usage:</para>
        /// <code>
        /// // create array
        /// int[] values = new int[] { 1, 1, 2, 3, 6, 8, 11, 12, 7, 3 };
        /// // calculate mode value
        /// int mode = Statistics.Mode( values );
        /// // output it (7)
        /// Console.WriteLine( "mode = " + mode );
        /// </code>
        /// </remarks>
        /// 
        public static int Mode( int[] values )
        {
            int mode = 0, curMax = 0;

            for ( int i = 0, length = values.Length; i < length; i++ )
            {
                if ( values[i] > curMax )
                {
                    curMax = values[i];
                    mode = i;
                }
            }

            return mode;
        }
    }
}
