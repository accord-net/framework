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
    using AForge;

    /// <summary>
    /// Uniform random numbers generator in the range of [0, 1).
    /// </summary>
    /// 
    /// <remarks><para>The random number generator generates uniformly
    /// distributed numbers in the range of [0, 1) - greater or equal to 0.0
    /// and less than 1.0.</para>
    /// 
    /// <para><note>At this point the generator is based on the
    /// internal .NET generator, but may be rewritten to
    /// use faster generation algorithm.</note></para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create instance of random generator
    /// IRandomNumberGenerator generator = new UniformOneGenerator( );
    /// // generate random number
    /// float randomNumber = generator.Next( );
    /// </code>
    /// </remarks>
    /// 
    public class UniformOneGenerator : IRandomNumberGenerator
    {
        // .NET random generator as a base
        private ThreadSafeRandom rand = null;

        /// <summary>
        /// Mean value of the generator.
        /// </summary>
        ///
        public float Mean
        {
            get { return 0.5f; }
        }

        /// <summary>
        /// Variance value of the generator.
        /// </summary>
        ///
        public float Variance
        {
            get { return 1f / 12; }
        }

        /// <summary>
		/// Initializes a new instance of the <see cref="UniformOneGenerator"/> class.
        /// </summary>
        /// 
        /// <remarks>Initializes random numbers generator with zero seed.</remarks>
        /// 
        public UniformOneGenerator( )
        {
            rand = new ThreadSafeRandom( 0 );
        }

        /// <summary>
		/// Initializes a new instance of the <see cref="UniformOneGenerator"/> class.
        /// </summary>
        /// 
        /// <param name="seed">Seed value to initialize random numbers generator.</param>
        /// 
        public UniformOneGenerator( int seed )
        {
            rand = new ThreadSafeRandom( seed );
        }

        /// <summary>
        /// Generate next random number.
        /// </summary>
        /// 
        /// <returns>Returns next random number.</returns>
        /// 
        public float Next( )
        {
            return (float) rand.NextDouble( );
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
            rand = new ThreadSafeRandom( seed );
        }
    }
}
