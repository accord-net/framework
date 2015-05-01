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
    /// Uniform random numbers generator.
    /// </summary>
    /// 
    /// <remarks><para>The random numbers generator generates uniformly
    /// distributed numbers in the <see cref="Range">specified range</see> - values
    /// are greater or equal to minimum range's value and less than maximum range's
    /// value.</para>
    /// 
    /// <para>The generator uses <see cref="UniformOneGenerator"/> generator
    /// to generate random numbers.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create instance of random generator
    /// IRandomNumberGenerator generator = new UniformGenerator( new Range( 50, 100 ) );
    /// // generate random number
    /// float randomNumber = generator.Next( );
    /// </code>
    /// </remarks>
    /// 
    public class UniformGenerator : IRandomNumberGenerator
    {
        private UniformOneGenerator rand = null;

        // generator's range
        private float min;
        private float length;

        /// <summary>
        /// Mean value of the generator.
        /// </summary>
        ///
        public float Mean
        {
            get { return ( min + min + length ) / 2; }
        }

        /// <summary>
        /// Variance value of the generator.
        /// </summary>
        ///
        public float Variance
        {
            get { return length * length / 12; }
        }

        /// <summary>
        /// Random numbers range.
        /// </summary>
        /// 
        /// <remarks><para>Range of random numbers to generate. Generated numbers are
        /// greater or equal to minimum range's value and less than maximum range's
        /// value.</para>
        /// </remarks>
        /// 
        public Range Range
        {
            get { return new Range( min, min + length ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniformGenerator"/> class.
        /// </summary>
        /// 
        /// <param name="range">Random numbers range.</param>
        /// 
        /// <remarks>Initializes random numbers generator with zero seed.</remarks>
        /// 
        public UniformGenerator( Range range ) :
            this( range, 0 )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniformGenerator"/> class.
        /// </summary>
        /// 
        /// <param name="range">Random numbers range.</param>
        /// <param name="seed">Seed value to initialize random numbers generator.</param>
        /// 
        public UniformGenerator( Range range, int seed )
        {
            rand = new UniformOneGenerator( seed );

            min     = range.Min;
            length  = range.Length;
        }

        /// <summary>
        /// Generate next random number.
        /// </summary>
        /// 
        /// <returns>Returns next random number.</returns>
        /// 
        public float Next( )
        {
            return (float) rand.Next( ) * length + min;
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
            rand = new UniformOneGenerator( seed );
        }
    }
}
