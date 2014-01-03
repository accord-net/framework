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
    ///   Summary statistics for a Split-set validation trial.
    /// </summary>
    /// 
    [Serializable]
    public class SplitSetStatistics<TModel> where TModel : class
    {

        /// <summary>
        ///   Gets the model created with the 
        /// </summary>
        public TModel Model { get; private set; }

        /// <summary>
        ///   Gets the values acquired during the cross-validation.
        ///   Most often those will be the errors for each folding.
        /// </summary>
        /// 
        public double Value { get; private set; }

        /// <summary>
        ///   Gets the variance for each value acquired during the cross-validation.
        ///   Most often those will be the error variance for each folding.
        /// </summary>
        /// 
        public double Variance { get; private set; }

        /// <summary>
        ///   Gets the number of samples used to compute the variance
        ///   of the values acquired during the validation.
        /// </summary>
        /// 
        public int Size { get; private set; }

        /// <summary>
        ///   Gets the standard deviation of the performance statistic.
        /// </summary>
        /// 
        public double StandardDeviation { get { return Math.Sqrt(Variance); } }

        /// <summary>
        ///   Gets or sets a tag for user-defined information.
        /// </summary>
        /// 
        public object Tag { get; set; }

        /// <summary>
        ///   Create a new split-set statistics class.
        /// </summary>
        /// 
        /// <param name="model">The generated model.</param>
        /// <param name="size">The number of samples used to compute the statistic.</param>
        /// <param name="value">The performance statistic gathered during the run.</param>
        /// <param name="variance">The variance of the performance statistic during the run.</param>
        /// 
        public SplitSetStatistics(TModel model, int size, double value, double variance)
        {
            this.Size = size;
            this.Model = model;
            this.Value = value;
            this.Variance = variance;
        }

    }

    /// <summary>
    ///   Summary statistics for a Split-set validation trial.
    /// </summary>
    /// 
    public class SplitSetStatistics : SplitSetStatistics<object>
    {
        /// <summary>
        ///   Create a new split-set statistics class.
        /// </summary>
        /// 
        /// <param name="model">The generated model.</param>
        /// <param name="size">The number of samples used to compute the statistic.</param>
        /// <param name="value">The performance statistic gathered during the run.</param>
        /// <param name="variance">The variance of the performance statistic during the run.</param>
        /// 
        public SplitSetStatistics(object model, int size, double value, double variance)
            : base(model, size, value, variance) { }

        /// <summary>
        ///   Create a new split-set statistics class.
        /// </summary>
        /// 
        /// <param name="model">The generated model.</param>
        /// <param name="size">The number of samples used to compute the statistic.</param>
        /// <param name="value">The performance statistic gathered during the run.</param>
        /// <param name="variance">The variance of the performance statistic during the run.</param>
        /// 
        public static SplitSetStatistics<TModel> Create<TModel>(TModel model, int size,
            double value, double variance) where TModel : class
        {
            return new SplitSetStatistics<TModel>(model, size, value, variance);
        }
    }
}
