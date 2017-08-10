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

namespace Accord.Statistics.Models.Fields
{
    using Accord.MachineLearning;
    using Accord.Math;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Models.Fields.Functions;
    using Accord.Statistics.Models.Fields.Learning;
    using Accord.Statistics.Models.Markov;
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Accord.Compat;
    using System.Threading.Tasks;

    /// <summary>
    ///   Utility methods to assist in the creating of <see cref="HiddenConditionalRandomField"/>s.
    /// </summary>
    /// 
    public static class HiddenConditionalRandomField
    {

        /// <summary>
        ///   Creates a new <see cref="HiddenConditionalRandomField{T}"/> from the given <paramref name="classifier"/>.
        /// </summary>
        /// 
        /// <param name="classifier">The classifier.</param>
        /// 
        /// <returns>
        ///   A <see cref="HiddenConditionalRandomField{T}"/> that implements 
        ///   exactly the same model as the given <paramref name="classifier"/>.
        /// </returns>
        /// 
        public static HiddenConditionalRandomField<int> FromHiddenMarkov(HiddenMarkovClassifier classifier)
        {
            return new HiddenConditionalRandomField<int>(new MarkovDiscreteFunction(classifier));
        }

        /// <summary>
        ///   Creates a new <see cref="HiddenConditionalRandomField{T}"/> from the given <paramref name="classifier"/>.
        /// </summary>
        /// 
        /// <param name="classifier">The classifier.</param>
        /// 
        /// <returns>
        ///   A <see cref="HiddenConditionalRandomField{T}"/> that implements 
        ///   exactly the same model as the given <paramref name="classifier"/>.
        /// </returns>
        /// 
        public static HiddenConditionalRandomField<int> FromHiddenMarkov(HiddenMarkovClassifier<GeneralDiscreteDistribution, int> classifier)
        {
            return new HiddenConditionalRandomField<int>(new MarkovDiscreteFunction(classifier));
        }

        /// <summary>
        ///   Creates a new <see cref="HiddenConditionalRandomField{T}"/> from the given <paramref name="classifier"/>.
        /// </summary>
        /// 
        /// <param name="classifier">The classifier.</param>
        /// 
        /// <returns>
        ///   A <see cref="HiddenConditionalRandomField{T}"/> that implements 
        ///   exactly the same model as the given <paramref name="classifier"/>.
        /// </returns>
        /// 
        public static HiddenConditionalRandomField<double> FromHiddenMarkov(HiddenMarkovClassifier<NormalDistribution, double> classifier)
        {
            return new HiddenConditionalRandomField<double>(new MarkovContinuousFunction(classifier));
        }

        /// <summary>
        ///   Creates a new <see cref="HiddenConditionalRandomField{T}"/> from the given <paramref name="classifier"/>.
        /// </summary>
        /// 
        /// <param name="classifier">The classifier.</param>
        /// 
        /// <returns>
        ///   A <see cref="HiddenConditionalRandomField{T}"/> that implements 
        ///   exactly the same model as the given <paramref name="classifier"/>.
        /// </returns>
        /// 
        public static HiddenConditionalRandomField<double[]> FromHiddenMarkov(HiddenMarkovClassifier<Independent<NormalDistribution>, double[]> classifier)
        {
            return new HiddenConditionalRandomField<double[]>(new MarkovMultivariateFunction(classifier));
        }

        /// <summary>
        ///   Creates a new <see cref="HiddenConditionalRandomField{T}"/> from the given <paramref name="classifier"/>.
        /// </summary>
        /// 
        /// <param name="classifier">The classifier.</param>
        /// 
        /// <returns>
        ///   A <see cref="HiddenConditionalRandomField{T}"/> that implements 
        ///   exactly the same model as the given <paramref name="classifier"/>.
        /// </returns>
        /// 
        public static HiddenConditionalRandomField<double[]> FromHiddenMarkov(HiddenMarkovClassifier<Independent<NormalDistribution, double>, double[]> classifier)
        {
            return new HiddenConditionalRandomField<double[]>(new MarkovMultivariateFunction(classifier));
        }

        /// <summary>
        ///   Creates a new <see cref="HiddenConditionalRandomField{T}"/> from the given <paramref name="classifier"/>.
        /// </summary>
        /// 
        /// <param name="classifier">The classifier.</param>
        /// 
        /// <returns>
        ///   A <see cref="HiddenConditionalRandomField{T}"/> that implements 
        ///   exactly the same model as the given <paramref name="classifier"/>.
        /// </returns>
        /// 
        public static HiddenConditionalRandomField<double[]> FromHiddenMarkov(HiddenMarkovClassifier<MultivariateNormalDistribution, double[]> classifier)
        {
            return new HiddenConditionalRandomField<double[]>(new MarkovMultivariateFunction(classifier));
        }

    }
}
