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
    ///   Value discretization preprocessing filter.
    /// </summary>
    /// 
    /// <remarks>
    ///   This filter converts double or decimal values with an fractional
    ///   part to the nearest possible integer according to a given threshold
    ///   and a rounding rule.</remarks>
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
    /// var filter = new Discretization("Cost (M)");
    /// 
    /// // Apply the filter and get the result
    /// DataTable result = filter.Apply(table);
    /// 
    /// // Show it
    /// DataGridBox.Show(result);
    /// </code>
    /// 
    /// <img src="..\images\filters\output-discretization.png" /> 
    /// </example>
    /// 
    [Serializable]
    public class Discretization : BaseFilter<Discretization.Options>, IAutoConfigurableFilter
    {

        /// <summary>
        ///   Creates a new Discretization filter.
        /// </summary>
        /// 
        public Discretization()
            : base()
        {
        }

        /// <summary>
        ///   Creates a new Discretization filter.
        /// </summary>
        /// 
        public Discretization(params string[] columns)
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

            foreach (Options options in Columns)
            {
                foreach (DataRow row in result.Rows)
                {
                    object obj = row[options.ColumnName];

                    double value;

                    String strValue = obj as String;
                    if (strValue != null)
                        value = Double.Parse(strValue);
                    else value = (double)obj;

                    double x = options.Symmetric ? System.Math.Abs(value) : value;

                    double floor = System.Math.Floor(x);

                    x = (x >= (floor + options.Threshold)) ?
                        System.Math.Ceiling(x) : floor;


                    value = (options.Symmetric && value < 0) ? -x : x;

                    row[options.ColumnName] = value;
                }
            }

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
                // If the column has a continuous numeric type
                if (column.DataType == typeof(Double) ||
                    column.DataType == typeof(Decimal))
                {
                    // Add the column to the processing options
                    if (!Columns.Contains(column.ColumnName))
                        Columns.Add(new Options(column.ColumnName));
                }
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
            ///   Gets or sets the threshold for the discretization filter.
            /// </summary>
            /// 
            public double Threshold { get; set; }

            /// <summary>
            ///   Gets or sets whether the discretization threshold is symmetric.
            /// </summary>
            /// 
            /// <remarks>
            /// <para>
            ///   If a symmetric threshold of 0.4 is used, for example, a real value of
            ///   0.5 will be rounded to 1.0 and a real value of -0.5 will be rounded to
            ///   -1.0. </para>
            /// <para>
            ///   If a non-symmetric threshold of 0.4 is used, a real value of 0.5
            ///   will be rounded towards 1.0, but a real value of -0.5 will be rounded
            ///   to 0.0 (because |-0.5| is higher than the threshold of 0.4).</para>
            /// </remarks>
            /// 
            public bool Symmetric { get; set; }

            /// <summary>
            ///   Constructs a new Options class for the discretization filter.
            /// </summary>
            /// 
            public Options(String name)
                : base(name)
            {
                this.Threshold = 0.5;
                this.Symmetric = false;
            }

            /// <summary>
            ///   Constructs a new Options object.
            /// </summary>
            /// 
            public Options()
                : this("New column")
            {
            }
        }
    }
}
