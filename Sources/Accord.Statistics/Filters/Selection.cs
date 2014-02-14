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
    using System.Data;

    /// <summary>
    ///   Relational-algebra selection filter.
    /// </summary>
    /// 
    [Serializable]
    public class Selection : IFilter
    {
        /// <summary>
        ///   Gets or sets the eSQL filter expression for the filter.
        /// </summary>
        /// 
        public string Expression { get; set; }

        /// <summary>
        ///   Gets or sets the ordering to apply for the filter.
        /// </summary>
        /// 
        public string OrderBy { get; set; }


        /// <summary>
        ///   Constructs a new Selection Filter.
        /// </summary>
        /// 
        /// <param name="expression">The filtering criteria.</param>
        /// <param name="orderBy">The desired sort order.</param>
        /// 
        public Selection(string expression, string orderBy)
        {
            this.Expression = expression;
            this.OrderBy = orderBy;
        }

        /// <summary>
        ///   Constructs a new Selection Filter.
        /// </summary>
        /// 
        /// <param name="expression">The filtering criteria.</param>
        /// 
        public Selection(string expression)
            : this(expression, String.Empty) { }

        /// <summary>
        ///   Constructs a new Selection Filter.
        /// </summary>
        /// 
        public Selection()
            : this(String.Empty, String.Empty) { }

        /// <summary>
        ///   Applies the filter to the current data.
        /// </summary>
        /// 
        public DataTable Apply(DataTable data)
        {
            DataTable table = data.Clone();

            DataRow[] rows = data.Select(Expression, OrderBy);
            foreach (DataRow row in rows)
                table.ImportRow(row);

            return table;
        }

    }
}
