// Accord Statistics Library
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

namespace Accord.Statistics.Distributions
{
    using Accord.Math.Random;
    using System;
    using System.Threading;
    using Accord.Compat;

    /// <summary>
    ///   Base class for statistical distribution implementations.
    /// </summary>
    /// 
    [Serializable]
    public abstract class DistributionBase : IFormattable
    {
        /*
        public DistributionBase()
        {
            RandomSource = () => Accord.Math.Random.Generator.Random;
        }

        /// <summary>
        ///   Gets or sets a factory method to create random number generators used
        ///   within the distribution.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        ///   As a recommendation, any provided factory method should create a new generator
        ///   on each call instead of reusing the same one. The reason is that not all
        ///   generators can be used across multiple threads, and some distributions will
        ///   call this method from different threads expecting different objects. Ignore
        ///   this remark only if your generators are thread safe or thread local.</para>
        ///   
        /// <para>
        ///   By default, this method will return instances of the framework's main random generator.
        ///   The property that gives access to the framework's main random generator 
        ///   (<see cref="Accord.Math.Random.Generator.Random"/>) provides a different generator for 
        ///   each thread (as it uses ThreadLocal).</para>
        /// </remarks>
        /// 
        public Func<Random> RandomSource { get; set; }
        */

        /// <summary>
        ///   Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public override string ToString()
        {
            return ToString(null, null);
        }

        /// <summary>
        ///   Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public string ToString(IFormatProvider formatProvider)
        {
            return ToString(null, formatProvider);
        }

        /// <summary>
        ///   Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public string ToString(string format)
        {
            return ToString(format, null);
        }

        /// <summary>
        ///   Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// 
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// 
        /// <returns>
        ///   A <see cref="System.String" /> that represents this instance.
        /// </returns>
        /// 
        public abstract string ToString(string format, IFormatProvider formatProvider);

        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public abstract object Clone();

    }
}
