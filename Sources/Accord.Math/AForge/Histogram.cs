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

    /// <summary>
    /// Histogram for discrete random values.
    /// </summary>
    /// 
    /// <remarks><para>The class wraps histogram for discrete stochastic function, which is represented
    /// by integer array, where indexes of the array are treated as values of the stochastic function,
    /// but array values are treated as "probabilities" (total amount of hits).
    /// </para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create histogram
    /// Histogram histogram = new Histogram( new int[10] { 0, 0, 1, 3, 6, 8, 11, 0, 0, 0 } );
    /// // get mean and standard deviation values
    /// Console.WriteLine( "mean = " + histogram.Mean + ", std.dev = " + histogram.StdDev );
    /// </code>
    /// </remarks>
    ///
    [Serializable]
    public class Histogram
    {
        private int[]   values;
        private double  mean = 0;
        private double  stdDev = 0;
        private int     median = 0;
        private int     min;
        private int     max;
        private long    total;

        /// <summary>
        /// Values of the histogram.
        /// </summary>
        /// 
        /// <remarks><para>Indexes of this array are treated as values of stochastic function,
        /// but array values are treated as "probabilities" (total amount of hits).
        /// </para></remarks>
        /// 
        public int[] Values
        {
            get { return values; }
        }

        /// <summary>
        /// Mean value.
        /// </summary>
        /// 
        /// <remarks><para>The property allows to retrieve mean value of the histogram.</para>
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // create histogram
        /// Histogram histogram = new Histogram( new int[10] { 0, 0, 1, 3, 6, 8, 11, 0, 0, 0 } );
        /// // get mean value (= 4.862)
        /// Console.WriteLine( "mean = " + histogram.Mean.ToString( "F3" ) );
        /// </code>
        /// </remarks>
        /// 
        public double Mean
        {
            get { return mean; }
        }

        /// <summary>
        /// Standard deviation.
        /// </summary>
        /// 
        /// <remarks><para>The property allows to retrieve standard deviation value of the histogram.</para>
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // create histogram
        /// Histogram histogram = new Histogram( new int[10] { 0, 0, 1, 3, 6, 8, 11, 0, 0, 0 } );
        /// // get std.dev. value (= 1.136)
        /// Console.WriteLine( "std.dev. = " + histogram.StdDev.ToString( "F3" ) );
        /// </code>
        /// </remarks>
        /// 
        public double StdDev
        {
            get { return stdDev; }
        }

        /// <summary>
        /// Median value.
        /// </summary>
        /// 
        /// <remarks><para>The property allows to retrieve median value of the histogram.</para>
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // create histogram
        /// Histogram histogram = new Histogram( new int[10] { 0, 0, 1, 3, 6, 8, 11, 0, 0, 0 } );
        /// // get median value (= 5)
        /// Console.WriteLine( "median = " + histogram.Median );
        /// </code>
        /// </remarks>
        /// 
        public int Median
        {
            get { return median; }
        }

        /// <summary>
        /// Minimum value.
        /// </summary>
        /// 
        /// <remarks><para>The property allows to retrieve minimum value of the histogram with non zero
        /// hits count.</para>
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // create histogram
        /// Histogram histogram = new Histogram( new int[10] { 0, 0, 1, 3, 6, 8, 11, 0, 0, 0 } );
        /// // get min value (= 2)
        /// Console.WriteLine( "min = " + histogram.Min );
        /// </code>
        /// </remarks>
        /// 
        public int Min
        {
            get { return min; }
        }

        /// <summary>
        /// Maximum value.
        /// </summary>
        /// 
        /// <remarks><para>The property allows to retrieve maximum value of the histogram with non zero
        /// hits count.</para>
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // create histogram
        /// Histogram histogram = new Histogram( new int[10] { 0, 0, 1, 3, 6, 8, 11, 0, 0, 0 } );
        /// // get max value (= 6)
        /// Console.WriteLine( "max = " + histogram.Max );
        /// </code>
        /// </remarks>
        /// 
        public int Max
        {
            get { return max; }
        }

        /// <summary>
        /// Total count of values.
        /// </summary>
        /// 
        /// <remarks><para>The property represents total count of values contributed to the histogram, which is
        /// essentially sum of the <see cref="Values"/> array.</para>
        ///
        /// <para>Sample usage:</para>
        /// <code>
        /// // create histogram
        /// Histogram histogram = new Histogram( new int[10] { 0, 0, 1, 3, 6, 8, 11, 0, 0, 0 } );
        /// // get total value (= 29)
        /// Console.WriteLine( "total = " + histogram.TotalCount );
        /// </code>
        /// </remarks>
        /// 
        public long TotalCount
        {
            get { return total; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Histogram"/> class.
        /// </summary>
        /// 
        /// <param name="values">Values of the histogram.</param>
        /// 
        /// <remarks><para>Indexes of the input array are treated as values of stochastic function,
        /// but array values are treated as "probabilities" (total amount of hits).
        /// </para></remarks>
        /// 
        public Histogram( int[] values )
        {
            this.values = values;
            Update( );
        }

        /// <summary>
        /// Get range around median containing specified percentage of values.
        /// </summary>
        /// 
        /// <param name="percent">Values percentage around median.</param>
        /// 
        /// <returns>Returns the range which containes specifies percentage of values.</returns>
        /// 
        /// <remarks><para>The method calculates range of stochastic variable, which summary probability
        /// comprises the specified percentage of histogram's hits.</para>
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // create histogram
        /// Histogram histogram = new Histogram( new int[10] { 0, 0, 1, 3, 6, 8, 11, 0, 0, 0 } );
        /// // get 50% range
        /// IntRange range = histogram.GetRange( 0.5 );
        /// // show the range ([4, 6])
        /// Console.WriteLine( "50% range = [" + range.Min + ", " + range.Max + "]" );
        /// </code>
        /// </remarks>
        /// 
        public IntRange GetRange( double percent )
        {
            return Statistics.GetRange( values, percent );
        }

        /// <summary>
        /// Update statistical value of the histogram.
        /// </summary>
        /// 
        /// <remarks>The method recalculates statistical values of the histogram, like mean,
        /// standard deviation, etc., in the case if histogram's values were changed directly.
        /// The method should be called only in the case if histogram's values were retrieved
        /// through <see cref="Values"/> property and updated after that.
        /// </remarks>
        /// 
        public void Update( )
        {
            int i, n = values.Length;

            max = 0;
            min = n;
            total = 0;

            // calculate min and max
            for ( i = 0; i < n; i++ )
            {
                if ( values[i] != 0 )
                {
                    // max
                    if ( i > max )
                        max = i;
                    // min
                    if ( i < min )
                        min = i;

                    total += values[i];
                }
            }

            mean   = Statistics.Mean( values );
            stdDev = Statistics.StdDev( values, mean );
            median = Statistics.Median( values );
        }
    }
}
