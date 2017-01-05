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

namespace Accord.Statistics.Analysis
{
    using AForge;

    /// <summary>
    ///   Common interface for descriptive measures, such as
    ///   <see cref="DescriptiveMeasures"/> and 
    ///   <see cref="CircularDescriptiveMeasures"/>.
    /// </summary>
    /// 
    /// <seealso cref="DescriptiveAnalysis"/>
    /// <seealso cref="CircularDescriptiveAnalysis"/>
    /// 
    public interface IDescriptiveMeasures
    {
        /// <summary>
        ///   Gets the variable's index.
        /// </summary>
        /// 
        int Index { get; }

        /// <summary>
        ///   Gets the variable's name
        /// </summary>
        /// 
        string Name { get; }

        /// <summary>
        ///   Gets the variable's total sum.
        /// </summary>
        /// 
        double Sum { get; }

        /// <summary>
        ///   Gets the variable's mean.
        /// </summary>
        /// 
        double Mean { get; }

        /// <summary>
        ///   Gets the variable's standard deviation.
        /// </summary>
        /// 
        double StandardDeviation { get; }

        /// <summary>
        ///   Gets the variable's median.
        /// </summary>
        /// 
        double Median { get; }

        /// <summary>
        ///   Gets the variable's outer fences range.
        /// </summary>
        /// 
        DoubleRange OuterFence { get; }

        /// <summary>
        ///   Gets the variable's inner fence range.
        /// </summary>
        /// 
        DoubleRange InnerFence { get; }

        /// <summary>
        ///   Gets the variable's interquartile range.
        /// </summary>
        /// 
        DoubleRange Quartiles { get; }

        /// <summary>
        ///   Gets the variable's mode.
        /// </summary>
        /// 
        double Mode { get; }

        /// <summary>
        ///   Gets the variable's variance.
        /// </summary>
        /// 
        double Variance { get; }

        /// <summary>
        ///   Gets the variable's skewness.
        /// </summary>
        /// 
        double Skewness { get; }

        /// <summary>
        ///   Gets the variable's kurtosis.
        /// </summary>
        /// 
        double Kurtosis { get; }

        /// <summary>
        ///   Gets the variable's standard error of the mean.
        /// </summary>
        /// 
        double StandardError { get; }

        /// <summary>
        ///   Gets the variable's maximum value.
        /// </summary>
        /// 
        double Max { get; }

        /// <summary>
        ///   Gets the variable's minimum value.
        /// </summary>
        /// 
        double Min { get; }

        /// <summary>
        ///   Gets the variable's length.
        /// </summary>
        /// 
        double Length { get; }

        /// <summary>
        ///   Gets the number of distinct values for the variable.
        /// </summary>
        /// 
        int Distinct { get; }

        /// <summary>
        ///   Gets the number of samples for the variable.
        /// </summary>
        /// 
        int Count { get; }

        /// <summary>
        ///   Gets the 95% confidence interval around the <see cref="Mean"/>.
        /// </summary>
        /// 
        DoubleRange Confidence { get; }

        /// <summary>
        ///   Gets the 95% deviance interval around the <see cref="Mean"/>.
        /// </summary>
        /// 
        DoubleRange Deviance { get; }

        /// <summary>
        ///   Gets the variable's observations.
        /// </summary>
        /// 
        double[] Samples { get; }

        /// <summary>
        ///   Gets a confidence interval for the <see cref="Mean"/>
        ///   within the given confidence level percentage.
        /// </summary>
        /// 
        /// <param name="percent">The confidence level. Default is 0.95.</param>
        /// 
        /// <returns>A confidence interval for the estimated value.</returns>
        /// 
        DoubleRange GetConfidenceInterval(double percent = 0.95);

        /// <summary>
        ///   Gets a deviance interval for the <see cref="Mean"/>
        ///   within the given confidence level percentage (i.e. uses
        ///   the standard deviation rather than the standard error to
        ///   compute the range interval for the variable).
        /// </summary>
        /// 
        /// <param name="percent">The confidence level. Default is 0.95.</param>
        /// 
        /// <returns>A confidence interval for the estimated value.</returns>
        /// 
        DoubleRange GetDevianceInterval(double percent = 0.95);
    }

}
