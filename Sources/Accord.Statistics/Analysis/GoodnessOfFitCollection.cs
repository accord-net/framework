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
    using System;
    using System.Collections.Generic;
    using Accord.Collections;
    using Accord.Compat;

    /// <summary>
    ///   Collection of goodness-of-fit measures.
    /// </summary>
    /// 
    /// <seealso cref="DistributionAnalysis"/>
    /// 
    [Serializable]
    public class GoodnessOfFitCollection : ReadOnlyKeyedCollection<string, GoodnessOfFit>
    {
        internal GoodnessOfFitCollection(IList<GoodnessOfFit> components)
            : base(components)
        {
        }

        /// <summary>
        ///   Gets the key for item.
        /// </summary>
        /// 
        protected override string GetKeyForItem(GoodnessOfFit item)
        {
            return item.Name;
        }
    }
}
