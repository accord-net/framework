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

namespace Accord.Statistics.Distributions.Fitting
{
    using System;
    using Multivariate;
    using Univariate;
    using Accord.Compat;
    using Accord.Math.Decompositions;

    /// <summary>
    ///   Estimation options for <see cref="Accord.Statistics.Distributions.Univariate.NormalDistribution">
    ///   Normal distributions</see>.
    /// </summary>
    /// 
    [Serializable]
    public class NormalOptions : IFittingOptions, IComponentOptions
    {
        /// <summary>
        ///   Gets or sets the regularization step to avoid singular or non-positive definite 
        ///   covariance matrices. Default is 0. Setting this property to a small constant like
        ///   1e-6 is more efficient than setting <see cref="Robust"/> to true.
        /// </summary>
        /// 
        /// <value>The regularization step.</value>
        /// 
        public double Regularization { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating whether the covariance
        ///   matrix to be estimated should be assumed to be diagonal.
        /// </summary>
        /// 
        /// <value><c>true</c> to estimate a diagonal covariance matrix; otherwise, <c>false</c>.</value>
        /// 
        public bool Diagonal { get; set; }

        /// <summary>
        ///   Gets or sets whether the estimation function should allow non-positive definite covariance 
        ///   matrices by using the <see cref="SingularValueDecomposition">Singular Value Decomposition</see>
        ///   instead of the default <see cref="CholeskyDecomposition">Cholesky</see>. Enabling this property 
        ///   can significantly increase learning times.
        /// </summary>
        /// 
        public bool Robust { get; set; }

        /// <summary>
        ///   Gets or sets whether the normal distributions should have only a single, shared 
        ///   covariance matrix among all components in a mixture. Setting this property only 
        ///   has effect if the distributions are part of a <see cref="Mixture{T}"/> or 
        ///   <see cref="MultivariateMixture{T}"/>
        /// </summary>
        /// 
        public bool Shared { get; set; }

        /// <summary>
        ///   Gets or sets a post processing step can be called after all component
        ///   distributions have been fitted (or their .Fit() method has been called).
        /// </summary>
        /// 
        public Action<IDistribution[], double[]> Postprocessing { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="NormalOptions"/> class.
        /// </summary>
        /// 
        public NormalOptions()
        {
            Regularization = 0;
            Diagonal = false;
        }


        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
