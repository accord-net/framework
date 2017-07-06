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

namespace Accord.Statistics.Distributions.Reflection
{
    using System;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    ///   Discovers the parameters of a <see cref="IDistribution">probability distribution</see>
    ///   and helps determine their range and whether then can be constructed automatically from
    ///   their indicated parameter ranges.
    /// </summary>
    /// 
    public class DistributionConstructorInfo
    {
        private bool? isBuildable;
        private DistributionParameterInfo[] parameters;

        /// <summary>
        ///   Gets the reflection constructor information.
        /// </summary>
        /// 
        public ConstructorInfo ConstructorInfo { get; private set; }

        /// <summary>
        ///   Gets a value indicating whether it is possible to discover enough
        ///   information about this constructor such that the distribution can
        ///   be constructed using reflection.
        /// </summary>
        /// 
        /// <value><c>true</c> if this instance is buildable; otherwise, <c>false</c>.</value>
        /// 
        public bool IsBuildable
        {
            get
            {
                if (isBuildable == null)
                    isBuildable = GetParameters().All(x => x.IsBuildable);
                return isBuildable.Value;
            }
        }

        /// <summary>
        ///   Gets the parameters of the constructor.
        /// </summary>
        /// 
        public DistributionParameterInfo[] GetParameters()
        {
            if (parameters == null)
                parameters = ConstructorInfo.GetParameters().Select(p => new DistributionParameterInfo(p)).ToArray();
            return parameters;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributionConstructorInfo"/> class.
        /// </summary>
        /// 
        /// <param name="constructor">The distribution's constructor.</param>
        /// 
        public DistributionConstructorInfo(ConstructorInfo constructor)
        {
            this.ConstructorInfo = constructor;
        }

    }
}
