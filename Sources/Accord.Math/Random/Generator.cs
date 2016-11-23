// Accord Core Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;

    /// <summary>
    ///   Framework-wide random number generator. If you would like to always generate 
    ///   the same results when using the framework, set the <see cref="Seed"/> property 
    ///   of this class to a fixed value.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   By setting <see cref="Seed"/> to a given value, it is possible to adjust how
    ///   random numbers are generated within the framework. Preferably, this property
    ///   should be adjusted <b>before</b> other computations. </para>
    ///   
    /// <para>
    ///   If the <see cref="Seed"/> is set to a value that is less than or equal to zero, all
    ///   generators will start with the same fixed seed, <b>even among multiple threads</b>. 
    ///   If set to any other value, the generators in other threads will start with fixed, but 
    ///   different, seeds.</para>
    /// </remarks>
    /// 
    /// <seealso cref="IRandomNumberGenerator"/>
    /// 
    public static class Generator
    {

        // Random generator used to seed other generators. It is used
        // to prevent generators that have been created in short spans
        // to be initialized with the same seed.
        private static Random source = new Random();
        private static readonly object sourceLock = new Object();

        private static int? seed;
        private static readonly object seedLock = new Object();


        private static readonly ThreadLocal<Random> random = new ThreadLocal<Random>(create);


        private static Random create()
        {
            // We initialize new Random objects using the next value from a global 
            // static shared random generator in order to avoid creating many random 
            // objects with the random seed. This guarantees reproducibility but does
            // not compromise the effectiveness of parallel methods that depends on 
            // the generation of true random sequences with different values.

            lock (sourceLock)
            {
                if (source == null)
                    return new Random(Generator.seed.GetValueOrDefault());
                return new Random(source.Next());
            }
        }

        /// <summary>
        ///   Gets a reference to the random number generator used
        ///   internally by the Accord.NET classes and methods.
        /// </summary>
        /// 
        public static Random Random { get { return random.Value; } }

        /// <summary>
        ///   Sets a random seed for the framework's main <see cref="Random">internal number 
        ///   generator</see>. Preferably, this method should be called <b>before</b> other 
        ///   computations. If set to a value less than or equal to zero, all generators will 
        ///   start with the same fixed seed, <b>even among multiple threads</b>. If set to any 
        ///   other value, the generators in other threads will start with fixed, but different, 
        ///   seeds.
        /// </summary>
        /// 
        public static int? Seed
        {
            get { return Generator.seed; }
            set
            {
                lock (seedLock)
                {
                    Generator.seed = value;

                    lock (sourceLock)
                    {
                        if (Generator.seed.HasValue)
                        {
                            if (Generator.seed.Value <= 0)
                            {
                                Trace.WriteLine("All threads will be initialized with the same seed: " + value);
                                Generator.source = null;
                            }
                            else
                            {
                                Trace.WriteLine("All threads will be initialized with predictable, but random seeds.");
                                Generator.source = new Random(Generator.seed.Value);
                            }
                        }
                        else
                        {
                            Trace.WriteLine("All threads will be initialized with unpredictable random seeds.");
                            Generator.source = new Random();
                        }
                    }

                    Generator.random.Value = create();
                }
            }
        }

    }
}
