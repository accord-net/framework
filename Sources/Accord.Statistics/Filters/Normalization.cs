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

#if !NETSTANDARD1_4
namespace Accord.Statistics.Filters
{

    using System;
    using System.Data;
    using System.ComponentModel;
    using Accord.Math;

    /// <summary>
    ///   Data normalization preprocessing filter.
    /// </summary>
    /// 
    /// <para>
    ///   The normalization filter is able to transform numerical data into
    ///   Z-Scores, subtracting the mean for each variable and dividing by
    ///   their standard deviation. The filter is able to distinguish
    ///   numerical columns automatically, leaving other columns unaffected.
    ///   It is also possible to control which columns should be processed
    ///   by the filter.</para>
    /// 
    /// <example>
    /// <para>
    ///   Suppose we have a data table relating the age of a person and its 
    ///   categorical classification, as in "child", "adult" or "elder".
    ///   The normalization filter can be used to transform the "Age" column
    ///   into Z-scores, as shown below:</para>
    ///   
    /// <code>
    ///   // Create the aforementioned sample table
    ///   DataTable table = new DataTable("Sample data");
    ///   table.Columns.Add("Age", typeof(double));
    ///   table.Columns.Add("Label", typeof(string));
    ///   
    ///   //            age   label
    ///   table.Rows.Add(10, "child");
    ///   table.Rows.Add(07, "child");
    ///   table.Rows.Add(04, "child");
    ///   table.Rows.Add(21, "adult");
    ///   table.Rows.Add(27, "adult");
    ///   table.Rows.Add(12, "child");
    ///   table.Rows.Add(79, "elder");
    ///   table.Rows.Add(40, "adult");
    ///   table.Rows.Add(30, "adult");
    ///   
    ///   // The filter will ignore non-real (continuous) data
    ///   Normalization normalization = new Normalization(table);
    ///   
    ///   double mean = normalization["Age"].Mean;              // 25.55
    ///   double sdev = normalization["Age"].StandardDeviation; // 23.29
    ///   
    ///   // Now we can process another table at once:
    ///   DataTable result = normalization.Apply(table);
    ///   
    ///   // The result will be a table with the same columns, but
    ///   // in which any column named "Age" will have been normalized
    ///   // using the previously detected mean and standard deviation:
    ///   
    ///   DataGridBox.Show(result);
    /// </code>
    /// 
    /// <para>
    ///   The resulting data is shown below:</para>
    /// 
    ///   <img src="..\images\normalization-filter.png" />
    /// 
    /// </example>
    /// 
    /// <seealso cref="Codification"/>
    /// 
    [Serializable]
    public class Normalization : BaseFilter<Normalization.Options, Normalization>,
        IAutoConfigurableFilter
    {

        /// <summary>
        ///   Creates a new data normalization filter.
        /// </summary>
        /// 
        public Normalization()
        {
        }

        /// <summary>
        ///   Creates a new data normalization filter.
        /// </summary>
        /// 
        public Normalization(DataTable table)
        {
            Detect(table);
        }

        /// <summary>
        ///   Creates a new data normalization filter.
        /// </summary>
        /// 
        public Normalization(params string[] columns)
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
            DataTable result = data.Copy();

            // Scale each value from the original ranges to destination ranges
            foreach (Options column in this.Columns)
            {
                string name = column.ColumnName;

                foreach (DataRow row in result.Rows)
                {
                    double value = (double)row[name];

                    // Center
                    value -= column.Mean;

                    if (column.Standardize)
                    {
                        // Normalize
                        value /= column.StandardDeviation;
                    }

                    row[name] = value;
                }
            }

            return result;
        }

        /// <summary>
        ///   Applies the Filter to a <see cref="System.Data.DataTable"/>.
        /// </summary>
        /// 
        /// <param name="data">The source <see cref="System.Data.DataTable"/>.</param>
        /// 
        /// <returns>The processed <see cref="System.Data.DataTable"/>.</returns>
        /// 
        public double[][] Apply(double[][] data)
        {
            // Initial argument checking
            if (data == null)
                throw new ArgumentNullException("data");

            if (Active)
            {
                double[][] newData = data.MemberwiseClone();

                apply(newData);

                return newData;
            }

            return data;
        }

        /// <summary>
        ///   Applies the Filter to a <see cref="System.Data.DataTable"/>.
        /// </summary>
        /// 
        /// <param name="data">The source <see cref="System.Data.DataTable"/>.</param>
        /// 
        /// <returns>The processed <see cref="System.Data.DataTable"/>.</returns>
        /// 
        public double[][] ApplyInPlace(double[][] data)
        {
            // Initial argument checking
            if (data == null)
                throw new ArgumentNullException("data");

            if (Active)
                apply(data);

            return data;
        }

        private void apply(double[][] data)
        {
            // Scale each value from the original ranges to destination ranges
            foreach (Options column in this.Columns)
            {
                string name = column.ColumnName;

                int index = (column.Tag != null) ?
                    (int)column.Tag : int.Parse(name);

                double mean = column.Mean;
                double stdDev = column.StandardDeviation;

                if (column.Standardize)
                {
                    for (int i = 0; i < data.Length; i++)
                        data[i][index] = (data[i][index] - mean) / stdDev;
                }
                else
                {
                    for (int i = 0; i < data.Length; i++)
                        data[i][index] = (data[i][index] - mean);
                }
            }
        }

        /// <summary>
        ///   Auto detects the filter options by analyzing a given <see cref="System.Data.DataTable"/>.
        /// </summary>     
        /// 
        public void Detect(DataTable data)
        {
            // For each column
            foreach (DataColumn column in data.Columns)
            {
                if (column.DataType == typeof(Double) || column.DataType == typeof(Decimal))
                {
                    string name = column.ColumnName;

                    string safeName = name
                        .Replace("]", @"\]");

                    double sdev = (double)data.Compute("STDEV([" + safeName + "])", String.Empty);
                    double mean = (double)data.Compute("AVG([" + safeName + "])", String.Empty);

                    if (!Columns.Contains(name))
                        Columns.Add(new Options(name));

                    Columns[name].Mean = mean;
                    Columns[name].StandardDeviation = sdev;
                }
            }
        }

        /// <summary>
        ///   Auto detects the filter options by analyzing a given matrix.
        /// </summary>     
        /// 
        public void Detect(double[][] data)
        {
            int rows = data.Length;
            int cols = data[0].Length;

            double[] means = data.Mean(dimension: 0);
            double[] stdDev = data.StandardDeviation(means);

            for (int i = 0; i < cols; i++)
            {
                string name = i.ToString();

                if (!Columns.Contains(name))
                    Columns.Add(new Options(name));

                Columns[name].Tag = i;
                Columns[name].Mean = means[i];
                Columns[name].StandardDeviation = stdDev[i];
            }
        }


        /// <summary>
        ///   Options for normalizing a column.
        /// </summary>
        ///
        [Serializable]
        public class Options : ColumnOptionsBase<Normalization>
        {
            /// <summary>
            ///   Gets or sets the mean of the data contained in the column.
            /// </summary>
            public double Mean { get; set; }

            /// <summary>
            ///   Gets or sets the standard deviation of the data contained in the column.
            /// </summary>
            public double StandardDeviation { get; set; }

            /// <summary>
            ///   Gets or sets if the column's data should be standardized to Z-Scores.
            /// </summary>
            public bool Standardize { get; set; }

            /// <summary>
            ///   Constructs a new Options object.
            /// </summary>
            public Options()
                : this("New column") { }

            /// <summary>
            ///   Constructs a new Options object for the given column.
            /// </summary>
            /// 
            /// <param name="name">
            ///   The name of the column to create this options for.
            /// </param>
            /// 
            public Options(String name)
                : this(name, 0, 1) { }

            /// <summary>
            ///   Constructs a new Options object for the given column.
            /// </summary>
            /// 
            /// <param name="name">
            ///   The name of the column to create this options for.
            /// </param>
            /// 
            /// <param name="mean">The mean value for normalization.</param>
            /// <param name="standardDeviation">The standard deviation value for standardization.</param>
            /// 
            public Options(String name, double mean, double standardDeviation)
                : base(name)
            {
                this.Mean = mean;
                this.StandardDeviation = standardDeviation;
                this.Standardize = true;
            }

        }

    }
}
#endif