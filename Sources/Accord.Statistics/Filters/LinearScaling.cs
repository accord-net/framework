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
    using AForge;
    using Accord.Math;

    /// <summary>
    ///   Linear Scaling Filter
    /// </summary>
    /// 
    [Serializable]
    public class LinearScaling : BaseFilter<LinearScaling.Options>, IAutoConfigurableFilter
    {


        /// <summary>
        ///   Creates a new Linear Scaling Filter.
        /// </summary>
        /// 
        public LinearScaling() { }

        /// <summary>
        ///   Creates a new Linear Scaling Filter.
        /// </summary>
        /// 
        public LinearScaling(params string[] columns)
        {
            foreach (String col in columns)
                Columns.Add(new Options(col));
        }

        /// <summary>
        ///   Creates a new Linear Scaling Filter.
        /// </summary>
        /// 
        public LinearScaling(DataTable table)
        {
            foreach (DataColumn col in table.Columns)
                Columns.Add(new Options(col.ColumnName));

            Detect(table);
        }

        /// <summary>
        ///   Creates a new Linear Scaling Filter.
        /// </summary>
        /// 
        public LinearScaling(DataTable table, params string[] columns)
        {
            foreach (String col in columns)
                Columns.Add(new Options(col));

            Detect(table, columns);
        }

        /// <summary>
        ///   Applies the filter to the DataTable.
        /// </summary>
        /// 
        protected override DataTable ProcessFilter(DataTable data)
        {
            DataTable result = data.Copy();

            // Scale each value from the original ranges to destination ranges
            foreach (DataColumn column in result.Columns)
            {
                string name = column.ColumnName;
                if (Columns.Contains(name))
                {
                    foreach (DataRow row in result.Rows)
                    {
                        try
                        {
                            double value = System.Convert.ToDouble(row[column]);

                            Options options = Columns[name];
                            row[column] = Accord.Math.Tools.Scale(
                                options.SourceRange,
                                options.OutputRange,
                                value);
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidCastException("Error in row #" + result.Rows.IndexOf(row), ex);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        ///   Auto detects the filter options by analyzing a given <see cref="System.Data.DataTable"/>.
        /// </summary>  
        /// 
        public void Detect(DataTable data, string[] columns)
        {
            // For each column
            foreach (string column in columns)
                parseColumn(data, data.Columns[column]);
        }

        /// <summary>
        ///   Auto detects the filter options by analyzing a given <see cref="System.Data.DataTable"/>.
        /// </summary>  
        /// 
        public void Detect(DataTable data)
        {
            // For each column
            foreach (DataColumn column in data.Columns)
                parseColumn(data, column);
        }

        private void parseColumn(DataTable data, DataColumn column)
        {
            string name = column.ColumnName;
            double max = 0, min = 0;

            // If the column has a continuous numeric type
            if (column.DataType == typeof(Double) ||
                column.DataType == typeof(Decimal))
            {
                object objMax = data.Compute("MAX([" + name + "])", String.Empty);
                object objMin = data.Compute("MIN([" + name + "])", String.Empty);

                if (objMax != DBNull.Value)
                    max = (double)objMax;
                if (objMin != DBNull.Value)
                    min = (double)objMin;
            }
            else if (column.DataType == typeof(String))
            {
                double[] values = column.ToArray();
                max = values.Max();
                min = values.Min();
            }

            if (!Columns.Contains(name))
                Columns.Add(new Options(name));

            Columns[name].SourceRange = new DoubleRange(min, max);
            // Columns[name].OutputRange = new DoubleRange(-1, +1);
        }

        /// <summary>
        ///   Options for the Linear Scaling filter.
        /// </summary>
        /// 
        [Serializable]
        public class Options : ColumnOptionsBase
        {
            /// <summary>
            ///   Range of the input values
            /// </summary>
            /// 
            public DoubleRange SourceRange { get; set; }

            /// <summary>
            ///   Target range of the output values after scaling.
            /// </summary>
            /// 
            public DoubleRange OutputRange { get; set; }

            /// <summary>
            ///   Creates a new column options.
            /// </summary>
            /// 
            public Options(String name)
                : base(name)
            {
                this.SourceRange = new DoubleRange(0, 1);
                this.OutputRange = new DoubleRange(-1, 1);
            }

            /// <summary>
            ///   Constructs a new Options object.
            /// </summary>
            /// 
            public Options()
                : this("New column") { }
        }
    }
}
