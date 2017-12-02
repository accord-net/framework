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

namespace Accord.Statistics.Testing
{
    using System;
    using Accord.Compat;

    /// <summary>
    ///   Sources of variation in a <see cref="TwoWayAnova">two-way ANOVA experiment</see>.
    /// </summary>
    /// 
    /// <seealso cref="TwoWayAnova"/>
    /// <seealso cref="IAnova"/>
    /// 
    /// <example>
    ///   Please see <see cref="TwoWayAnova"/> for examples.
    /// </example>
    /// 
    [Serializable]
    public class TwoWayAnovaVariationSources
    {
        internal TwoWayAnovaVariationSources() { }

        /// <summary>
        ///   Gets information about the first factor (A).
        /// </summary>
        /// 
        public AnovaVariationSource FactorA { get; internal set; }

        /// <summary>
        ///   Gets information about the second factor (B) source.
        /// </summary>
        /// 
        public AnovaVariationSource FactorB { get; internal set; }

        /// <summary>
        ///   Gets information about the interaction factor (AxB) source.
        /// </summary>
        /// 
        public AnovaVariationSource Interaction { get; internal set; }

        /// <summary>
        ///   Gets information about the error (within-variance) source.
        /// </summary>
        /// 
        public AnovaVariationSource Error { get; internal set; }

        /// <summary>
        ///   Gets information about the grouped (cells) variance source.
        /// </summary>
        /// 
        public AnovaVariationSource Cells { get; internal set; }

        /// <summary>
        ///   Gets information about the total source of variance.
        /// </summary>
        /// 
        public AnovaVariationSource Total { get; internal set; }

    }
}
