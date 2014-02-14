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
    using System.Linq;
    using System.Text;
    using System.Data;

    /// <summary>
    ///   Elimination filter.
    /// </summary>
    /// 
    [Serializable]
    public class Elimination : BaseFilter<Elimination.Options>, IAutoConfigurableFilter
    {

        /// <summary>
        ///   Creates a elimination filter to remove
        ///   rows containing missing values.
        /// </summary>
        /// 
        public Elimination()
            : base() { }

        /// <summary>
        ///   Creates a elimination filter to remove
        ///   rows containing missing values in the
        ///   specified columns.
        /// </summary>
        /// 
        public Elimination(params string[] columns)
        {
            foreach (String col in columns)
                Columns.Add(new Options(col));
        }

        /// <summary>
        ///   Processes the current filter.
        /// </summary>
        /// 
        protected override DataTable ProcessFilter(DataTable data)
        {
            // Copy the DataTable
            DataTable result = data.Copy();

            List<DataRow> deleted = new List<DataRow>();

            foreach (DataRow row in result.Rows)
            {
                foreach (Options options in Columns)
                {
                    double value;

                    if (!Double.TryParse(row[options.ColumnName].ToString(), out value))
                        value = Double.NaN;

                    if ((!Double.IsNaN(options.Value) && value == options.Value)
                      || (Double.IsNaN(options.Value) && Double.IsNaN(value)))
                    {
                        deleted.Add(row); break;
                    }
                }
            }

            foreach (DataRow row in deleted)
                result.Rows.Remove(row);

            return result;
        }

        /// <summary>
        ///   Auto detects the filter options by analyzing a given <see cref="System.Data.DataTable"/>.
        /// </summary> 
        /// 
        public void Detect(DataTable data)
        {
            foreach (DataColumn column in data.Columns)
            {
                // Add the column to the processing options
                if (!Columns.Contains(column.ColumnName))
                    Columns.Add(new Options(column.ColumnName));
            }
        }

        /// <summary>
        ///   Options for the discretization filter.
        /// </summary>
        /// 
        [Serializable]
        public class Options : ColumnOptionsBase
        {

            /// <summary>
            ///   Gets the value indicator of a missing field.
            ///   Default is <see cref="Double.NaN"/>.
            /// </summary>
            /// 
            public double Value { get; set; }

            /// <summary>
            ///   Constructs a new column option
            ///   for the Elimination filter.
            /// </summary>
            /// 
            public Options(String name)
                : base(name)
            {
                this.Value = Double.NaN;
            }

            /// <summary>
            ///   Constructs a new column option
            ///   for the Elimination filter.
            /// </summary>
            /// 
            public Options()
                : this("New column") { }

        }
    }
}
