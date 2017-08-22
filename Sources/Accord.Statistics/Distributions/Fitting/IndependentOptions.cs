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
    using Accord.Compat;

    /// <summary>
    ///   Estimation options for <see cref="Accord.Statistics.Distributions.Multivariate.Independent{T}">
    ///   multivariate independent distributions</see>.
    /// </summary>
    /// 
    public class IndependentOptions : IFittingOptions
    {

        /// <summary>
        ///   Gets or sets the fitting options for the inner
        ///   independent components in the joint distribution.
        /// </summary>
        /// 
        public IFittingOptions InnerOption { get; set; }

        /// <summary>
        ///   Gets or sets the fitting options for specific inner
        ///   independent components in the joint distribution.
        /// </summary>
        /// 
        public IFittingOptions[] InnerOptions { get; set; }

        /// <summary>
        ///   Gets or sets whether the data to be fitted has already been transposed.
        /// </summary>
        /// 
        public bool Transposed { get; set; }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public virtual object Clone()
        {
            return MemberwiseClone();
        }
    }

    /// <summary>
    ///   Estimation options for <see cref="Accord.Statistics.Distributions.Multivariate.Independent{T}">
    ///   multivariate independent distributions</see>.
    /// </summary>
    /// 
    [Serializable]
    public class IndependentOptions<TOptions> : IndependentOptions
        where TOptions : class, IFittingOptions, new()
    {

        /// <summary>
        ///   Gets or sets the fitting options for the inner
        ///   independent components in the joint distribution.
        /// </summary>
        /// 
        public new TOptions InnerOption
        {
            get { return base.InnerOption as TOptions; }
            set { base.InnerOption = value; }
        }

        /// <summary>
        ///   Gets or sets the fitting options for specific inner
        ///   independent components in the joint distribution.
        /// </summary>
        /// 
        public new TOptions[] InnerOptions
        {
            get { return base.InnerOptions as TOptions[]; }
            set { base.InnerOptions = value; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="IndependentOptions{TOptions}"/> class.
        /// </summary>
        /// 
        public IndependentOptions()
        {
            InnerOption = new TOptions();
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            return MemberwiseClone();
        }
    }
}
