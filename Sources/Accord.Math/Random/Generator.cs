// Accord Core Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
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
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;

    /// <summary>
    ///   Framework-wide random number generator. If you would like
    ///   to always generate the same results when using the framework,
    ///   set the <see cref="Seed"/> property of this class to a fixed
    ///   value.
    /// </summary>
    /// 
    public static class Generator
    {

        private static int? seed;


#if !NET35 && !NET40
        private static readonly ThreadLocal<Random> random;

        // This static constructor is being used to address an issue with the Mono runtime. 
        // The problem is that the runtime currently does not implement the "trackAllValues"
        // overload for ThreadLocal, even if the API offers such constructor. The following
        // CA suppression rule could be removed once Mono adds support for it.
        //
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static Generator()
        {
            try
            {
                random = new ThreadLocal<Random>(create, true);
            }
            catch (NotImplementedException)
            {
                // Deal with a temporary shortcoming when targeting Mono runtime.
                random = new ThreadLocal<Random>(create);
            }
        }
#else
        private static readonly ThreadLocal<Random> random = new ThreadLocal<Random>(create);
#endif

        private static Random create()
        {
            if (seed.HasValue)
                return new Random(seed.Value);
            return new Random();
        }

        /// <summary>
        ///   Gets a reference to the random number generator used
        ///   internally by the Accord.NET classes and methods.
        /// </summary>
        /// 
        public static Random Random { get { return random.Value; } }

        /// <summary>
        ///   Sets a random seed for the framework's main <see cref="Random">internal 
        ///   number generator</see>. Preferably, this method should be called <b>before</b>
        ///   other computations.
        /// </summary>
        /// 
        public static int? Seed
        {
            get { return seed; }
            set
            {
                Accord.Math.Random.Generator.seed = value;

#if !NET35 && !NET40
                lock (random)
                {
                    for (int i = 0; i < random.Values.Count; i++)
                        random.Values[i] = create();
                }
#endif
                Accord.Math.Random.Generator.random.Value = create();
            }
        }

    }
}
