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

namespace Accord.Statistics.Filters
{
    using System;
    using System.Collections.ObjectModel;
#if !NETSTANDARD1_4
    using System.Data;
#endif

    /// <summary>
    ///   Sample processing filter interface.
    /// </summary>
    /// 
    /// <remarks>The interface defines the set of methods which should be
    /// provided by all table processing filters. Methods of this interface should
    /// keep the source table unchanged and return the result of data processing
    /// filter as new data table.</remarks>
    /// 
    public interface IFilter
    {
#if !NETSTANDARD1_4
        /// <summary>
        ///   Applies the filter to a <see cref="System.Data.DataTable"/>.
        /// </summary>
        /// 
        /// <param name="data">Source table to apply filter to.</param>
        /// 
        /// <returns>Returns filter's result obtained by applying the filter to
        /// the source table.</returns>
        /// 
        /// <remarks>The method keeps the source table unchanged and returns the
        /// the result of the table processing filter as new data table.</remarks> 
        ///
        DataTable Apply(DataTable data);
#endif
    }
}
