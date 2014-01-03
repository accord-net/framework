// Accord Machine Learning Library
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

namespace Accord.MachineLearning
{
    using System;

    /// <summary>
    ///   Summary statistics for a cross-validation trial.
    /// </summary>
    /// 
    [Serializable]
    public class CrossValidationStatistics
    {

        /// <summary>
        ///   Gets the values acquired during the cross-validation.
        ///   Most often those will be the errors for each folding.
        /// </summary>
        /// 
        public double[] Values { get; private set; }

        /// <summary>
        ///   Gets the variance for each value acquired during the cross-validation.
        ///   Most often those will be the error variance for each folding.
        /// </summary>
        /// 
        public double[] Variances { get; private set; }

        /// <summary>
        ///   Gets the number of samples used to compute the variance
        ///   of the values acquired during the cross-validation.
        /// </summary>
        /// 
        public int[] Sizes { get; private set; }


        /// <summary>
        ///   Gets the mean of the performance statistics.
        /// </summary>
        /// 
        public double Mean { get; private set; }

        /// <summary>
        ///   Gets the variance of the performance statistics.
        /// </summary>
        /// 
        public double Variance { get; private set; }

        /// <summary>
        ///   Gets the standard deviation of the performance statistics.
        /// </summary>
        /// 
        public double StandardDeviation { get { return Math.Sqrt(Variance); } }

        /// <summary>
        ///   Gets the pooled variance of the performance statistics.
        /// </summary>
        /// 
        public double PooledVariance { get; private set; }

        /// <summary>
        ///   Gets the pooled standard deviation of the performance statistics.
        /// </summary>
        /// 
        public double PooledStandardDeviation { get { return Math.Sqrt(PooledVariance); } }


        /// <summary>
        ///   Gets or sets a tag for user-defined information.
        /// </summary>
        /// 
        public object Tag { get; set; }

        /// <summary>
        ///   Create a new cross-validation statistics class.
        /// </summary>
        /// 
        /// <param name="sizes">The number of samples used to compute the statistics.</param>
        /// <param name="statistics">The performance statistics gathered during the run.</param>
        /// 
        public CrossValidationStatistics(int[] sizes, double[] statistics)
            : this(sizes, statistics, null) { }

        /// <summary>
        ///   Create a new cross-validation statistics class.
        /// </summary>
        /// 
        /// <param name="sizes">The number of samples used to compute the statistics.</param>
        /// <param name="statistics">The performance statistics gathered during the run.</param>
        /// <param name="variances">The variance of the statistics gathered during the run, if available.</param>
        /// 
        public CrossValidationStatistics(int[] sizes, double[] statistics, double[] variances)
        {
            this.Sizes = sizes;
            this.Values = statistics;
            this.Variances = variances;

            this.Mean = Statistics.Tools.Mean(statistics);
            this.Variance = Statistics.Tools.Variance(statistics, Mean);

            // Compute the pooled variance if the individual variances
            //  for each cross-validation run are available

            if (variances != null)
                this.PooledVariance = Statistics.Tools.PooledVariance(sizes, variances);
        }

    }
}
