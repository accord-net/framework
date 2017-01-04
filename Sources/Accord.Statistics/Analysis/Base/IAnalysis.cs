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

using System;

namespace Accord.Statistics.Analysis
{
    /// <summary>
    ///   Determines the method to be used in a statistical analysis.
    /// </summary>
    /// 
    public enum AnalysisMethod
    {

        /// <summary>
        ///   By choosing Center, the method will be run on the mean-centered data.
        /// </summary>
        /// 
        /// <remarks>
        ///   In Principal Component Analysis this means the method will operate
        ///   on the Covariance matrix of the given data.
        /// </remarks>
        ///  
        Center = 0,

        /// <summary>
        ///    By choosing Standardize, the method will be run on the mean-centered and
        ///    standardized data.
        /// </summary>
        /// 
        /// <remarks>
        ///    In Principal Component Analysis this means the method
        ///    will operate on the Correlation matrix of the given data. One should always
        ///    choose to standardize when dealing with different units of variables.
        /// </remarks>
        /// 
        Standardize = 1,
    };

    /// <summary>
    ///   Determines the method to be used in a statistical analysis.
    /// </summary>
    /// 
    public enum PrincipalComponentMethod
    {

        /// <summary>
        ///   By choosing Center, the method will be run on the mean-centered data.
        /// </summary>
        /// 
        /// <remarks>
        ///   In Principal Component Analysis this means the method will operate
        ///   on the Covariance matrix of the given data.
        /// </remarks>
        ///  
        Center = 0,

        /// <summary>
        ///    By choosing Standardize, the method will be run on the mean-centered and
        ///    standardized data.
        /// </summary>
        /// 
        /// <remarks>
        ///    In Principal Component Analysis this means the method
        ///    will operate on the Correlation matrix of the given data. One should always
        ///    choose to standardize when dealing with different units of variables.
        /// </remarks>
        /// 
        Standardize = 1,

        /// <summary>
        ///   By choosing CorrelationMatrix, the method will interpret the given data
        ///   as a correlation matrix.
        /// </summary>
        /// 
        CorrelationMatrix = 2,

        /// <summary>
        ///   By choosing CovarianceMatrix, the method will interpret the given data
        ///   as a correlation matrix.
        /// </summary>
        /// 
        CovarianceMatrix = 3,

        /// <summary>
        ///   By choosing KernelMatrix, the method will interpret the given data
        ///   as a Kernel (Gram) matrix.
        /// </summary>
        /// 
        KernelMatrix = 4,
    };

    /// <summary>
    ///   Common interface for statistical analysis.
    /// </summary>
    /// 
    [Obsolete]
    public interface IAnalysis
    {

        /// <summary>
        ///   Computes the analysis using given source data and parameters.
        /// </summary>
        /// 
        void Compute();

    }
}
