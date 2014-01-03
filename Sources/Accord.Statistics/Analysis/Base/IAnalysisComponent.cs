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

namespace Accord.Statistics.Analysis
{
    /// <summary>
    ///   Common interface for information components. Those are
    ///   present in multivariate analysis, such as <see cref="PrincipalComponentAnalysis"/>
    ///   and <see cref="LinearDiscriminantAnalysis"/>.
    /// </summary>
    /// 
    public interface IAnalysisComponent
    {
        /// <summary>
        ///   Gets the index for this component.
        /// </summary>
        /// 
        int Index { get; }

        /// <summary>
        ///   Gets the proportion, or amount of information explained by this component.
        /// </summary>
        /// 
        double Proportion { get; }

        /// <summary>
        ///   Gets the cumulative proportion of all discriminants until this component.
        /// </summary>
        /// 
        double CumulativeProportion { get; }

    }
}
