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

namespace Accord.Statistics.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Linq;

    /// <summary>
    ///   Relational-algebra projection filter.
    /// </summary>
    /// 
    /// <remarks>
    ///   This filter is able to selectively remove columns from tables, and keep
    ///   only the columns of interest.
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    /// // Show the start data
    /// DataGridBox.Show(table);
    /// </code>
    /// 
    /// <img src="..\images\filters\input-table.png" /> 
    /// 
    /// <code>
    /// // Create a new data projection (column) filter
    /// var filter = new Projection("Floors", "Finished");
    /// 
    /// // Apply the filter and get the result
    /// DataTable result = filter.Apply(table);
    /// 
    /// // Show it
    /// DataGridBox.Show(result);
    /// </code>
    /// 
    /// <img src="..\images\filters\output-projection.png" /> 
    /// 
    /// </example>
    /// 
    [Serializable]
    public class Projection : IFilter
    {
        /// <summary>
        ///   List of columns to keep in the projection.
        /// </summary>
        /// 
        public Collection<String> Columns { get; private set; }

        /// <summary>
        ///   Creates a new projection filter.
        /// </summary>
        /// 
        public Projection(params string[] columns)
        {
            this.Columns = new Collection<string>(columns.ToList());
        }

        /// <summary>
        ///   Creates a new projection filter.
        /// </summary>
        /// 
        public Projection(IEnumerable<string> columns)
        {
            this.Columns = new Collection<string>(columns.ToList());
        }

        /// <summary>
        ///   Creates a new projection filter.
        /// </summary>
        /// 
        public Projection()
        {
            this.Columns = new Collection<string>();
        }

        /// <summary>
        ///   Applies the filter to the DataTable.
        /// </summary>
        /// 
        public DataTable Apply(DataTable data)
        {
            List<String> cols = new List<String>();

            foreach (var col in Columns)
            {
                if (data.Columns.Contains(col))
                    cols.Add(col);
            }

            return data.DefaultView.ToTable(false, Columns.ToArray());
        }

    }
}
