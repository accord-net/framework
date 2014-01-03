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

namespace Accord.Statistics.Distributions
{
    using System;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Multivariate;

    /// <summary>
    ///   Common interface for multivariate probability distributions.
    /// </summary>
    /// 
    /// <remarks>
    ///   <para>
    ///   This interface is implemented by both multivariate <see cref="MultivariateDiscreteDistribution">
    ///   Discrete Distributions</see> and <see cref="MultivariateContinuousDistribution">Continuous 
    ///   Distributions</see>.</para>
    ///   
    ///   <para>
    ///   For Univariate distributions, see <see cref="IUnivariateDistribution"/>.</para>
    /// </remarks>
    /// 
    /// <seealso cref="MultivariateNormalDistribution"/>
    /// <seealso cref="DirichletDistribution"/>
    /// <seealso cref="MultivariateEmpiricalDistribution"/>
    /// <seealso cref="WishartDistribution"/>
    /// 
    public interface IMultivariateDistribution : IDistribution
    {

        /// <summary>
        ///   Gets the number of variables for the distribution.
        /// </summary>
        /// 
        int Dimension { get; }

        /// <summary>
        ///   Gets the Mean vector for the distribution.
        /// </summary>
        /// 
        /// <value>An array of double-precision values containing
        /// the mean values for this distribution.</value>
        /// 
        double[] Mean { get; }

        /// <summary>
        ///   Gets the Median vector for the distribution.
        /// </summary>
        ///
        /// <value>An array of double-precision values containing
        /// the median values for this distribution.</value>
        /// 
        double[] Median { get; }

        /// <summary>
        ///   Gets the Mode vector for the distribution.
        /// </summary>
        /// 
        /// <value>An array of double-precision values containing
        /// the mode values for this distribution.</value>
        /// 
        double[] Mode { get; }

        /// <summary>
        ///   Gets the Variance vector for the distribution.
        /// </summary>
        /// 
        /// <value>An array of double-precision values containing
        /// the variance values for this distribution.</value>
        /// 
        double[] Variance { get; }

        /// <summary>
        ///   Gets the Variance-Covariance matrix for the distribution.
        /// </summary>
        /// 
        /// <value>An multidimensional array of double-precision values
        /// containing the covariance values for this distribution.</value>
        /// 
        double[,] Covariance { get; }

    }

    /// <summary>
    ///   Common interface for multivariate probability distributions.
    /// </summary>
    /// 
    /// <remarks>
    ///   <para>
    ///   This interface is implemented by both multivariate <see cref="MultivariateDiscreteDistribution">
    ///   Discrete Distributions</see> and <see cref="MultivariateContinuousDistribution">Continuous 
    ///   Distributions</see>.  However, unlike <see cref="IMultivariateDistribution"/>, this interface
    ///   has a generic parameter that allows to define the type of the distribution values (i.e.
    ///   <see cref="T:double"/>).</para>
    ///   
    ///   <para>
    ///   For Univariate distributions, see <see cref="IUnivariateDistribution"/>.</para>
    /// </remarks>
    /// 
    /// <seealso cref="MultivariateNormalDistribution"/>
    /// <seealso cref="DirichletDistribution"/>
    /// <seealso cref="MultivariateEmpiricalDistribution"/>
    /// <seealso cref="WishartDistribution"/>
    /// 
    public interface IMultivariateDistribution<in TObservation> : IDistribution<TObservation>
    {

        /// <summary>
        ///   Gets the number of variables for the distribution.
        /// </summary>
        /// 
        int Dimension { get; }

        /// <summary>
        ///   Gets the Mean vector for the distribution.
        /// </summary>
        /// 
        /// <value>An array of double-precision values containing
        /// the mean values for this distribution.</value>
        /// 
        double[] Mean { get; }

        /// <summary>
        ///   Gets the Median vector for the distribution.
        /// </summary>
        ///
        /// <value>An array of double-precision values containing
        /// the median values for this distribution.</value>
        /// 
        double[] Median { get; }

        /// <summary>
        ///   Gets the Mode vector for the distribution.
        /// </summary>
        /// 
        /// <value>An array of double-precision values containing
        /// the mode values for this distribution.</value>
        /// 
        double[] Mode { get; }

        /// <summary>
        ///   Gets the Variance vector for the distribution.
        /// </summary>
        /// 
        /// <value>An array of double-precision values containing
        /// the variance values for this distribution.</value>
        /// 
        double[] Variance { get; }

        /// <summary>
        ///   Gets the Variance-Covariance matrix for the distribution.
        /// </summary>
        /// 
        /// <value>An multidimensional array of double-precision values
        /// containing the covariance values for this distribution.</value>
        /// 
        double[,] Covariance { get; }

    }

}
