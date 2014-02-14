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
    using System.Runtime.Serialization;
    using Accord.Math;

    /// <summary>
    ///   Imputation filter for filling missing values.
    /// </summary>
    /// 
    [Serializable]
    public class Imputation : BaseFilter<Imputation.Options>, IAutoConfigurableFilter
    {

        /// <summary>
        ///   Creates a new Imputation filter.
        /// </summary>
        /// 
        public Imputation()
            : base() { }

        /// <summary>
        ///   Creates a new Imputation filter.
        /// </summary>
        /// 
        public Imputation(params string[] columns)
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

            foreach (DataRow row in result.Rows)
            {
                foreach (Options options in Columns)
                {
                    double value;

                    if (!Double.TryParse(row[options.ColumnName].ToString(), out value))
                        value = Double.NaN;

                    if ((!Double.IsNaN(options.MissingValue) && value == options.MissingValue)
                      || (Double.IsNaN(options.MissingValue) && Double.IsNaN(value)))
                    {
                        row[options.ColumnName] = options.ReplaceWith;
                    }
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
                // Add the column to the processing options
                if (!Columns.Contains(column.ColumnName))
                    Columns.Add(new Options(column.ColumnName));
            }
        }

        /// <summary>
        ///   Strategies for missing value imputations.
        /// </summary>
        /// 
        public enum Strategy
        {
            /// <summary>
            ///   Uses a fixed-value to replace missing fields.
            /// </summary>
            /// 
            FixedValue,

            /// <summary>
            ///   Uses the mean value to replace missing fields.
            /// </summary>
            Mean,

            /// <summary>
            ///   Uses the mode value to replace missing fields.
            /// </summary>
            Mode,

            /// <summary>
            ///   Uses the median value to replace missing fields.
            /// </summary>
            Median
        };


        /// <summary>
        ///   Options for the imputation filter.
        /// </summary>
        /// 
        [Serializable]
        public class Options : ColumnOptionsBase, IAutoConfigurableColumn
        {

            [OptionalField]
            private Strategy strategy;

            /// <summary>
            ///   Gets or sets the imputation strategy
            ///   to use with this column.
            /// </summary>
            public Strategy Strategy
            {
                get { return strategy; }
                set { strategy = value; }
            }

            /// <summary>
            ///   Missing value indicator.
            /// </summary>
            /// 
            public double MissingValue { get; set; }

            /// <summary>
            ///   Value to replace missing values with.
            /// </summary>
            /// 
            public double ReplaceWith { get; set; }

            /// <summary>
            ///   Constructs a new column option
            ///   for the Imputation filter.
            /// </summary>
            /// 
            public Options(String name)
                : base(name)
            {
                this.MissingValue = Double.NaN;
                this.ReplaceWith = 0;
            }

            /// <summary>
            ///   Constructs a new column option
            ///   for the Imputation filter.
            /// </summary>
            /// 
            public Options()
                : this("New column") { }


            /// <summary>
            ///   Auto detects the column options by analyzing
            ///   a given <see cref="System.Data.DataColumn"/>.
            /// </summary>
            /// 
            /// <param name="column">The column to analyze.</param>
            /// 
            public void Detect(DataColumn column)
            {
                double[] row = column.ToArray();

                switch (Strategy)
                {
                    case Imputation.Strategy.FixedValue:
                        ReplaceWith = 0;
                        break;
                    case Imputation.Strategy.Mean:
                        ReplaceWith = row.Mean();
                        break;
                    case Imputation.Strategy.Median:
                        ReplaceWith = row.Median();
                        break;
                    case Imputation.Strategy.Mode:
                        ReplaceWith = row.Mode();
                        break;
                }
            }

        }
    }
}
