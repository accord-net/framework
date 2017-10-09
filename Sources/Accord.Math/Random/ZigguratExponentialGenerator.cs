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
    using System.Text;
    using Accord.Compat;
    using System.Threading.Tasks;

    /// <summary>
    ///   Exponential random number generator using the Ziggurat method.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>    
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://people.sc.fsu.edu/~jburkardt/c_src/ziggurat/ziggurat.html">
    ///       John Burkard, Ziggurat Random Number Generator (RNG). Available on:
    ///       http://people.sc.fsu.edu/~jburkardt/c_src/ziggurat/ziggurat.c (LGPL) </a>
    ///       </description></item>
    ///     <item><description>
    ///       Philip Leong, Guanglie Zhang, Dong-U Lee, Wayne Luk, John Villasenor,
    ///       A comment on the implementation of the ziggurat method,
    ///       Journal of Statistical Software, Volume 12, Number 7, February 2005.
    ///       </description></item>
    ///     <item><description>  
    ///       George Marsaglia, Wai Wan Tsang, The Ziggurat Method for Generating Random Variables,
    ///       Journal of Statistical Software, Volume 5, Number 8, October 2000, seven pages. 
    ///       </description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    public class ZigguratExponentialGenerator : IRandomNumberGenerator<double>
    {
        private ZigguratUniformOneGenerator u;
        private uint[] ke;
        private double[] fe;
        private double[] we;

        /// <summary>
        ///   Initializes a new instance of the <see cref="ZigguratExponentialGenerator"/> class.
        /// </summary>
        /// 
        /// <param name="seed">The random seed to use. Default is to use the next value from
        ///   the <see cref="Generator">the framework-wide random generator</see>.</param>
        /// 
        public ZigguratExponentialGenerator(int seed)
        {
            u = new ZigguratUniformOneGenerator(seed);

            ke = new uint[256];
            fe = new double[256];
            we = new double[256];
            setup();
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ZigguratExponentialGenerator"/> class.
        /// </summary>
        /// 
        public ZigguratExponentialGenerator()
            : this(Generator.Random.Next())
        {
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
        /// <returns>
        ///   A random vector of observations drawn from this distribution.
        /// </returns>
        /// 
        public double Generate()
        {
            uint jz = u.Next();
            uint iz = (jz & 255);

            if (jz < ke[iz])
                return jz * we[iz];

            for (; ; )
            {
                if (iz == 0)
                    return 7.69711 - Math.Log(u.Generate());

                double x = jz * we[iz];

                if (fe[iz] + u.Generate() * (fe[iz - 1] - fe[iz]) < Math.Exp(-x))
                    return x;

                jz = u.Next();
                iz = jz & 255;

                if (jz < ke[iz])
                    return jz * we[iz];
            }

            //throw new InvalidOperationException("Execution should not reach here.");
        }


        void setup()
        {
            double de = 7.697117470131487;
            const double m2 = 2147483648.0;
            double te = 7.697117470131487;
            const double ve = 3.949659822581572E-03;

            double q = ve / Math.Exp(-de);

            ke[0] = (uint)((de / q) * m2);
            ke[1] = 0;

            we[0] = q / m2;
            we[255] = de / m2;

            fe[0] = 1.0f;
            fe[255] = Math.Exp(-de);

            for (int i = 254; 1 <= i; i--)
            {
                de = -Math.Log(ve / de + Math.Exp(-de));
                ke[i + 1] = (uint)((de / te) * m2);
                te = de;
                fe[i] = Math.Exp(-de);
                we[i] = de / m2;
            }
        }

    }
}
