// Accord Core Library
// The Accord.NET Framework
// http://accord-framework.net
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

namespace Accord.Math.Random
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using Accord.Compat;
    using System.Threading.Tasks;

    /// <summary>
    ///   Uniform random number generator using the Ziggurat method.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>    
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://people.sc.fsu.edu/~jburkardt/c_src/ziggurat/ziggurat.html">
    ///       John Burkard, Ziggurat Random Number Generator (RNG). Available on:
    ///       http://people.sc.fsu.edu/~jburkardt/c_src/ziggurat/ziggurat.c (LGPL) </a></description></item>
    ///     <item><description>
    ///       Philip Leong, Guanglie Zhang, Dong-U Lee, Wayne Luk, John Villasenor,
    ///       A comment on the implementation of the ziggurat method,
    ///       Journal of Statistical Software, Volume 12, Number 7, February 2005.
    ///     </description></item>
    ///     <item><description>  
    ///       George Marsaglia, Wai Wan Tsang, The Ziggurat Method for Generating Random Variables,
    ///       Journal of Statistical Software, Volume 5, Number 8, October 2000, seven pages. </description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    public sealed class ZigguratUniformOneGenerator :
        IRandomNumberGenerator<double>,
        IRandomNumberGenerator<int>,
        IRandomNumberGenerator<uint>
    {
        private uint jsr;

        /// <summary>
        ///   Initializes a new instance of the <see cref="ZigguratExponentialGenerator"/> class.
        /// </summary>
        /// 
        public ZigguratUniformOneGenerator()
            : this(Generator.Random.Next())
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ZigguratUniformOneGenerator"/> class.
        /// </summary>
        /// 
        /// <param name="seed">The random seed to use. Default is to use the next value from
        ///   the <see cref="Generator">the framework-wide random generator</see>.</param>
        /// 
        public ZigguratUniformOneGenerator(int seed)
        {
            jsr = (uint)seed;
        }

        /// <summary>
        ///   Generates a random vector of observations from the current distribution.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// 
        /// <returns>
        ///   A random vector of observations drawn from this distribution.
        /// </returns>
        /// 
        public double[] Generate(int samples)
        {
            return Generate(samples, new double[samples]);
        }

        /// <summary>
        ///   Generates a random vector of observations from the current distribution.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        /// 
        /// <returns>
        ///   A random vector of observations drawn from this distribution.
        /// </returns>
        /// 
        public double[] Generate(int samples, double[] result)
        {
            for (int i = 0; i < samples; i++)
                result[i] = Generate();
            return result;
        }

        /// <summary>
        ///   Generates a random vector of observations from the current distribution.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        /// 
        /// <returns>
        ///   A random vector of observations drawn from this distribution.
        /// </returns>
        /// 
        public int[] Generate(int samples, int[] result)
        {
            for (int i = 0; i < samples; i++)
                result[i] = (int)Next();
            return result;
        }

        /// <summary>
        ///   Generates a random vector of observations from the current distribution.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        /// 
        /// <returns>
        ///   A random vector of observations drawn from this distribution.
        /// </returns>
        /// 
        public uint[] Generate(int samples, uint[] result)
        {
            for (int i = 0; i < samples; i++)
                result[i] = Next();
            return result;
        }

        /// <summary>
        ///   Generates a random vector of observations from the current distribution.
        /// </summary>
        /// 
        /// <returns>
        ///   A random vector of observations drawn from this distribution.
        /// </returns>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public double Generate()
        {
            unchecked
            {
                uint value = jsr;
                jsr = (jsr ^ (jsr << 13));
                jsr = (jsr ^ (jsr >> 17));
                jsr = (jsr ^ (jsr << 5));
                // https://core.ac.uk/download/files/153/6287927.pdf
                // return 0.5 + (value + jsr) * 0.2328306e-9;
                return (0.5 + (value + jsr) / 65536.0 / 65536.0) % 1.0;
            }
        }

        /// <summary>
        ///   Generates a new non-negative integer random number.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public uint Next()
        {
            unchecked
            {
                uint value = jsr;
                jsr = (jsr ^ (jsr << 13));
                jsr = (jsr ^ (jsr >> 17));
                jsr = (jsr ^ (jsr << 5));
                return value + jsr;
            }
        }


        int[] IRandomNumberGenerator<int>.Generate(int samples)
        {
            return Generate(samples, new int[samples]);
        }

        int IRandomNumberGenerator<int>.Generate()
        {
            return (int)Next();
        }

        uint[] IRandomNumberGenerator<uint>.Generate(int samples)
        {
            return Generate(samples, new uint[samples]);
        }

        uint IRandomNumberGenerator<uint>.Generate()
        {
            return Next();
        }
    }
}
