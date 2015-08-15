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
    /// Interface for random numbers generators.
    /// </summary>
    /// 
    /// <remarks><para>The interface defines set of methods and properties, which should
    /// be implemented by different algorithms for random numbers generatation.</para>
    /// </remarks>
    /// 
    public interface IRandomNumberGenerator
    {
        /// <summary>
        /// Mean value of generator.
        /// </summary>
        /// 
        float Mean { get; }

        /// <summary>
        /// Variance value of generator.
        /// </summary>
        /// 
        float Variance { get; }

        /// <summary>
        /// Generate next random number.
        /// </summary>
        /// 
        /// <returns>Returns next random number.</returns>
        /// 
        float Next( );

        /// <summary>
        /// Set seed of the random numbers generator.
        /// </summary>
        /// 
        /// <param name="seed">Seed value.</param>
        /// 
        void SetSeed( int seed );
    }
}
