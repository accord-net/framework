﻿// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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
    using System.Data;

    /// <summary>
    ///   Data processing interface for in-place filters.
    /// </summary>
    /// 
    public interface IInPlaceFilter
    {

        /// <summary>
        ///   Applies the filter to a <see cref="System.Data.DataTable"/>,
        ///   modifying the table in place.
        /// </summary>
        /// 
        /// <param name="data">Source table to apply filter to.</param>
        /// 
        /// <remarks>The method modifies the source table in place.</remarks> 
        ///
        void ApplyInPlace(DataTable data);

    }

}
