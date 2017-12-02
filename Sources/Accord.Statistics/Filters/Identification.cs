﻿// Accord Statistics Library
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

#if !NETSTANDARD1_4
namespace Accord.Statistics.Filters
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Linq;
    using System.Collections.Generic;

    /// <summary>
    ///   Identification filter.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The identification filter adds a new column to the data containing an
    ///   unique id for each of the samples (rows) in the data table (or matrix).</para>
    /// </remarks>
    /// 
    [Serializable]
    public class Identification : IFilter
    {
        
        /// <summary>
        ///   Gets or sets the name of the column used
        ///   to store row indices.
        /// </summary>
        /// 
        public String ColumnName { get; set; }

        /// <summary>
        ///   Creates a new identification filter.
        /// </summary>
        /// 
        public Identification()
        {
            ColumnName = "Id";
        }

        /// <summary>
        ///   Creates a new identification filter.
        /// </summary>
        /// 
        public Identification(String columnName)
        {
            ColumnName = columnName;
        }


        /// <summary>
        ///   Applies the filter to the DataTable.
        /// </summary>
        /// 
        public DataTable Apply(DataTable data)
        {
            DataTable result = data.Copy();

            result.Columns.Add(ColumnName, typeof(int));

            for (int i = 0; i < result.Rows.Count; i++)
                result.Rows[i][ColumnName] = i;

            return result;
        }

    }
}
#endif