// AForge Math Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2011
// contacts@aforgenet.com
//

namespace AForge.Math.Random
{
    using System;

    /// <summary>
    /// Gaussian random numbers generator.
    /// </summary>
    /// 
    /// <remarks><para>The random number generator generates gaussian
    /// random numbers with specified mean and standard deviation values.</para>
    /// 
    /// <para>The generator uses <see cref="StandardGenerator"/> generator as base
    /// to generate random numbers.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create instance of random generator
    /// IRandomNumberGenerator generator = new GaussianGenerator( 5.0, 1.5 );
    /// // generate random number
    /// float randomNumber = generator.Next( );
    /// </code>
    /// </remarks>
    /// 
    public class GaussianGenerator : IRandomNumberGenerator
    {
        // standard numbers generator
        private StandardGenerator rand = null;
        // mean value
        private float mean;
        // standard deviation value
        private float stdDev;

        /// <summary>
        /// Mean value of the generator.
        /// </summary>
        ///
        public float Mean
        {
            get { return mean;  }
        }

        /// <summary>
        /// Variance value of the generator.
        /// </summary>
        ///
        public float Variance
        {
            get { return stdDev * stdDev; }
        }

        /// <summary>
        /// Standard deviation value.
        /// </summary>
        ///
        public float StdDev
        {
            get { return stdDev; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GaussianGenerator"/> class.
        /// </summary>
        /// 
        /// <param name="mean">Mean value.</param>
        /// <param name="stdDev">Standard deviation value.</param>
        /// 
        public GaussianGenerator( float mean, float stdDev ) :
            this( mean, stdDev, 0 )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GaussianGenerator"/> class.
        /// </summary>
        /// 
        /// <param name="mean">Mean value.</param>
        /// <param name="stdDev">Standard deviation value.</param>
        /// <param name="seed">Seed value to initialize random numbers generator.</param>
        /// 
        public GaussianGenerator( float mean, float stdDev, int seed )
        {
            this.mean   = mean;
            this.stdDev = stdDev;

            rand = new StandardGenerator( seed );
        }

        /// <summary>
        /// Generate next random number.
        /// </summary>
        /// 
        /// <returns>Returns next random number.</returns>
        /// 
        public float Next( )
        {
            return (float) rand.Next( ) * stdDev + mean;
        }

        /// <summary>
        /// Set seed of the random numbers generator.
        /// </summary>
        /// 
        /// <param name="seed">Seed value.</param>
        /// 
        /// <remarks>Resets random numbers generator initializing it with
        /// specified seed value.</remarks>
        /// 
        public void SetSeed( int seed )
        {
            rand = new StandardGenerator( seed );
        }
    }
}
