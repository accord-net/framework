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
    ///   Base implementation for <see cref="IPotentialFunction{T}">potential functions</see>.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the observations modeled.</typeparam>
    /// 
    [Serializable]
    public abstract class PotentialFunctionBase<T> 
    {

        /// <summary>
        ///   Gets the factor potentials (also known as clique potentials) 
        ///   functions composing this potential function.
        /// </summary>
        /// 
        public FactorPotential<T>[] Factors { get; protected set; }

        /// <summary>
        ///   Gets the number of output classes assumed by this function.
        /// </summary>
        /// 
        public int Outputs { get; protected set; }

        /// <summary>
        ///   Gets or sets the set of weights for each feature function.
        /// </summary>
        /// 
        /// <value>The weights for each of the feature functions.</value>
        /// 
        public double[] Weights { get; set; }

        /// <summary>
        /// Gets the feature functions composing this potential function.
        /// </summary>
        /// 
        public IFeature<T>[] Features { get; protected set; }

        /// <summary>
        ///   Computes the factor potential function for the given parameters.
        /// </summary>
        /// 
        /// <param name="states">A state sequence.</param>
        /// <param name="observations">A sequence of observations.</param>
        /// <param name="output">The output class label for the sequence.</param>
        /// <returns>The value of the factor potential function evaluated for the given parameters.</returns>
        /// 
        public virtual double[] GetFeatureVector(int[] states, T[] observations, int output = 0)
        {
            double[] featureVector = new double[Weights.Length];

            for (int i = 0; i < featureVector.Length; i++)
                featureVector[i] = Features[i].Compute(states, observations, output);

            return featureVector;
        }

    }
}
