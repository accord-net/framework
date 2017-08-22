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

namespace Accord.Statistics.Models.Regression.Fitting
{
    using Accord.Statistics.Distributions.Univariate;
    using System;

    /// <summary>
    ///   Common interface for regression fitting methods.
    /// </summary>
    /// 
#pragma warning disable 612, 618
    [Obsolete("Please use ISupervisedLearning instead.")]
    interface ISurvivalFitting : IRegressionFitting
#pragma warning restore 612, 618
    {

        /// <summary>
        ///   Runs the fitting algorithm.
        /// </summary>
        /// 
        /// <param name="inputs">The input training data.</param>
        /// <param name="time">The time until the output happened.</param>
        /// <param name="censor">The indication variables used to signal
        ///   if the event occurred or if it was censored.</param>
        /// 
        /// <returns>The error.</returns>
        /// 
        double Run(double[][] inputs, double[] time, SurvivalOutcome[] censor);

        /// <summary>
        ///   Runs the fitting algorithm.
        /// </summary>
        /// 
        /// <param name="inputs">The input training data.</param>
        /// <param name="time">The time until the output happened.</param>
        /// <param name="censor">The indication variables used to signal
        ///   if the event occurred or if it was censored.</param>
        /// 
        /// <returns>The error.</returns>
        /// 
        double Run(double[][] inputs, double[] time, int[] censor);

    }
}
