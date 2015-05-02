// AForge Core Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2011
// contacts@aforgenet.com
//

namespace AForge
{
    using System;

    /// <summary>
    /// Thread safe version of the <see cref="System.Random"/> class.
    /// </summary>
    /// 
    /// <remarks><para>The class inherits the <see cref="System.Random"/> and overrides
    /// its random numbers generation methods providing thread safety by guarding call
    /// to the base class with a lock. See documentation to <see cref="System.Random"/> for
    /// additional information about the base class.</para></remarks>
    /// 
    public sealed class ThreadSafeRandom : Random
    {
        private object sync = new object( );

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadSafeRandom"/> class.
        /// </summary>
        /// 
        /// <remarks>See <see cref="Random.Next()"/> for more information.</remarks>
        /// 
        public ThreadSafeRandom( )
            : base( )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadSafeRandom"/> class.
        /// </summary>
        /// 
        /// <remarks>A number used to calculate a starting value for the pseudo-random number sequence.
        /// If a negative number is specified, the absolute value of the number is used.</remarks>
        /// 
        /// 
        /// <remarks>See <see cref="Random.Next()"/> for more information.</remarks>
        /// 
        public ThreadSafeRandom( int seed )
            : base( seed )
        {
        }

        /// <summary>
        /// Returns a nonnegative random number.
        /// </summary>
        /// 
        /// <returns>Returns a 32-bit signed integer greater than or equal to zero and less than
        /// <see cref="int.MaxValue"/>.</returns>
        /// 
        /// <remarks>See <see cref="Random.Next()"/> for more information.</remarks>
        /// 
        public override int Next( )
        {
            lock ( sync ) return base.Next( );
        }

        /// <summary>
        /// Returns a nonnegative random number less than the specified maximum.
        /// </summary>
        /// 
        /// <param name="maxValue">The exclusive upper bound of the random number to be generated.
        /// <paramref name="maxValue"/> must be greater than or equal to zero.</param>
        /// 
        /// <returns>Returns a 32-bit signed integer greater than or equal to zero, and less than <paramref name="maxValue"/>;
        /// that is, the range of return values ordinarily includes zero but not <paramref name="maxValue"/>.</returns>
        /// 
        /// <remarks>See <see cref="Random.Next(int)"/> for more information.</remarks>
        /// 
        public override int Next( int maxValue )
        {
            lock ( sync ) return base.Next( maxValue );
        }

        /// <summary>
        /// Returns a random number within a specified range.
        /// </summary>
        /// 
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned.
        /// <paramref name="maxValue"/> must be greater than or equal to <paramref name="minValue"/>.</param>
        /// 
        /// <returns>Returns a 32-bit signed integer greater than or equal to <paramref name="minValue"/> and less
        /// than <paramref name="maxValue"/>; that is, the range of return values includes
        /// <paramref name="minValue"/> but not <paramref name="maxValue"/>.</returns>
        /// 
        /// <remarks>See <see cref="Random.Next(int,int)"/> for more information.</remarks>
        ///
        public override int Next( int minValue, int maxValue )
        {
            lock ( sync ) return base.Next( minValue, maxValue );
        }

        /// <summary>
        /// Fills the elements of a specified array of bytes with random numbers.
        /// </summary>
        /// 
        /// <param name="buffer">An array of bytes to contain random numbers.</param>
        /// 
        /// <remarks>See <see cref="Random.NextBytes(byte[])"/> for more information.</remarks>
        ///
        public override void NextBytes( byte[] buffer )
        {
            lock ( sync ) base.NextBytes( buffer );
        }

        /// <summary>
        /// Returns a random number between 0.0 and 1.0.
        /// </summary>
        /// 
        /// <returns>Returns a double-precision floating point number greater than or equal to 0.0, and less than 1.0.</returns>
        /// 
        /// <remarks>See <see cref="Random.NextDouble()"/> for more information.</remarks>
        ///
        public override double NextDouble( )
        {
            lock ( sync ) return base.NextDouble( );
        }
    }
}
