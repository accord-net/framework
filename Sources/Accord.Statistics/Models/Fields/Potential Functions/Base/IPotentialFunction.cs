// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
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

namespace Accord.Statistics.Models.Fields.Functions
{
    using System;
    using Accord.Statistics.Models.Fields.Features;

    /// <summary>
    ///   Common interface for CRF's Potential functions.
    /// </summary>
    /// 
    public interface IPotentialFunction<T> : ICloneable
    {

        /// <summary>
        ///   Gets the factor potentials (also known as clique potentials) 
        ///   functions composing this potential function.
        /// </summary>
        /// 
        FactorPotential<T>[] Factors { get; }

        /// <summary>
        ///   Gets the number of output classes assumed by this function.
        /// </summary>
        /// 
        int Outputs { get; }

        /// <summary>
        ///   Gets or sets the set of weights for each feature function.
        /// </summary>
        /// 
        /// <value>The weights for each of the feature functions.</value>
        /// 
        double[] Weights { get; set; }

        /// <summary>
        ///   Gets the feature functions composing this potential function.
        /// </summary>
        /// 
        IFeature<T>[] Features { get; }

        /// <summary>
        ///   Gets the feature vector for a given input and sequence of states.
        /// </summary>
        /// 
        double[] GetFeatureVector(int[] states, T[] observations, int output = 0);

    }
}
