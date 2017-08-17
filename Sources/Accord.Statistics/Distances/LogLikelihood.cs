﻿// Accord Math Library
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

namespace Accord.Math.Distances
{
    using Accord.Math.Decompositions;
    using Accord.Statistics.Distributions.Multivariate;
    using System;
    using Accord.Compat;

    /// <summary>
    ///   Log-likelihood distance between a sample and a statistical distribution.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the distribution.</typeparam>
    /// 
    [Serializable]
    public sealed class LogLikelihood<T> :
        IDistance<double[], T>,
        IDistance<double[], IMixtureComponent<T>>
        where T : MultivariateContinuousDistribution
    {

        /// <summary>
        ///   Initializes a new instance of the <see cref="LogLikelihood{T}"/> class.
        /// </summary>
        /// 
        public LogLikelihood()
        {
        }

        /// <summary>
        ///   Computes the distance <c>d(x,y)</c> between points
        ///   <paramref name="x"/> and <paramref name="y"/>.
        /// </summary>
        /// 
        /// <param name="x">The first point <c>x</c>.</param>
        /// <param name="y">The second point <c>y</c>.</param>
        /// 
        /// <returns>
        ///   A double-precision value representing the distance <c>d(x,y)</c>
        ///   between <paramref name="x"/> and <paramref name="y"/> according 
        ///   to the distance function implemented by this class.
        /// </returns>
        /// 
        public double Distance(double[] x, T y)
        {
            return -y.LogProbabilityDensityFunction(x);
        }

        /// <summary>
        ///   Computes the distance <c>d(x,y)</c> between points
        ///   <paramref name="x"/> and <paramref name="y"/>.
        /// </summary>
        /// 
        /// <param name="x">The first point <c>x</c>.</param>
        /// <param name="y">The second point <c>y</c>.</param>
        /// 
        /// <returns>
        ///   A double-precision value representing the distance <c>d(x,y)</c>
        ///   between <paramref name="x"/> and <paramref name="y"/> according 
        ///   to the distance function implemented by this class.
        /// </returns>
        /// 
        public double Distance(double[] x, IMixtureComponent<T> y)
        {
            return -Math.Log(y.Coefficient) - y.Component.LogProbabilityDensityFunction(x);
        }
    }
}
