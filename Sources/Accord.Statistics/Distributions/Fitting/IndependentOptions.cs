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

namespace Accord.Statistics.Distributions.Fitting
{
    using System;

    /// <summary>
    ///   Estimation options for <see cref="Accord.Statistics.Distributions.Multivariate.Independent{T}">
    ///   multivariate independent distributions</see>.
    /// </summary>
    /// 
    [Serializable]
    public class IndependentOptions : IFittingOptions
    {

        /// <summary>
        ///   Gets or sets the fitting options for the inner
        ///   independent components in the joint distribution.
        /// </summary>
        /// 
        public IFittingOptions[] InnerOptions { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="IndependentOptions"/> class.
        /// </summary>
        /// 
        /// <param name="innerOptions">The fitting options for the inner
        ///   component distributions of the independent distributions.</param>
        ///   
        public IndependentOptions(IFittingOptions[] innerOptions)
        {
            InnerOptions = innerOptions;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="IndependentOptions"/> class.
        /// </summary>
        /// 
        public IndependentOptions()
        {
        }
    }
}
