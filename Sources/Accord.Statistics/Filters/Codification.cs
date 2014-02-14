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
    using System.Data;
    using System.ComponentModel;
    using Accord.Math;

    /// <summary>
    ///   Codification Filter class.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The codification filter performs an integer codification of classes in
    ///   given in a string form. An unique integer identifier will be assigned
    ///   for each of the string classes.</para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   When handling data tables, often there will be cases in which a single
    ///   table contains both numerical variables and categorical data in the form
    ///   of text labels. Since most machine learning and statistics algorithms
    ///   expect their data to be numeric, the codification filter can be used
    ///   to create mappings between text labels and discrete symbols.</para>
    ///   
    /// <code>
    /// // Show the start data
    /// DataGridBox.Show(table);
    /// </code>
    /// 
    /// <img src="..\images\filters\input-table.png" /> 
    /// 
    /// <code>
    /// // Create a new data projection (column) filter
    /// var filter = new Codification(table, "Category");
    /// 
    /// // Apply the filter and get the result
    /// DataTable result = filter.Apply(table);
    /// 
    /// // Show it
    /// DataGridBox.Show(result);
    /// </code>
    /// 
    /// <img src="..\images\filters\output-codification.png" /> 
    /// 
    /// 
    /// <para>
    ///   The following more elaborated examples show how to
    ///   use the <see cref="Codification"/> filter without
    ///   necessarily handling <see cref="System.Data.DataTable">
    ///   DataTable</see>s.</para>
    ///   
    /// <code>
    ///   // Suppose we have a data table relating the age of
    ///   // a person and its categorical classification, as 
    ///   // in "child", "adult" or "elder".
    ///   
    ///   // The Codification filter is able to extract those
    ///   // string labels and transform them into discrete
    ///   // symbols, assigning integer labels to each of them
    ///   // such as "child" = 0, "adult" = 1, and "elder" = 3.
    ///   
    ///   // Create the aforementioned sample table
    ///   DataTable table = new DataTable("Sample data");
    ///   table.Columns.Add("Age", typeof(int));
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
    ///   
    ///   // Now, let's say we need to translate those text labels
    ///   // into integer symbols. Let's use a Codification filter:
    ///   
    ///   Codification codebook = new Codification(table);
    ///   
    ///   
    ///   // After that, we can use the codebook to "translate"
    ///   // the text labels into discrete symbols, such as:
    ///   
    ///   int a = codebook.Translate("Label", "child"); // returns 0
    ///   int b = codebook.Translate("Label", "adult"); // returns 1
    ///   int c = codebook.Translate("Label", "elder"); // returns 2
    ///   
    ///   // We can also do the reverse:
    ///   string labela = codebook.Translate("Label", 0); // returns "child"
    ///   string labelb = codebook.Translate("Label", 1); // returns "adult"
    ///   string labelc = codebook.Translate("Label", 2); // returns "elder"
    /// </code>
    /// 
    /// <para>
    ///   After we have created the codebook, we can use it to feed data with
    ///   categorical variables to method which would otherwise not know how
    ///   to handle text labels data. Continuing with our example, the next
    ///   code section shows how to convert an entire data table into a numerical
    ///   matrix. </para>
    /// 
    /// <code>
    ///   // We can process an entire data table at once:
    ///   DataTable result = codebook.Apply(table);
    ///   
    ///   // The resulting table can be transformed to jagged array:
    ///   double[][] matrix = Matrix.ToArray(result);
    ///   
    ///   // and the resulting matrix will be given by
    ///   // new double[][] 
    ///   // {
    ///   //     new double[] { 10, 0 },
    ///   //     new double[] {  7, 0 },
    ///   //     new double[] {  4, 0 },
    ///   //     new double[] { 21, 1 },
    ///   //     new double[] { 27, 1 },
    ///   //     new double[] { 12, 0 },
    ///   //     new double[] { 79, 2 },
    ///   //     new double[] { 40, 1 },
    ///   //     new double[] { 30, 1 } 
    ///   // };
    ///   
    ///   // PS: the string representation for the matrix above can be obtained by calling
    ///   string str = matrix.ToString(CSharpJaggedMatrixFormatProvider.InvariantCulture);
    /// </code>
    /// 
    /// <para>
    ///   Finally, by expressing our data in terms of a simple numerical
    ///   matrix we will be able to feed it to any machine learning algorithm.
    ///   The following code section shows how to create a <see cref="Accord.Statistics.Kernels.Linear">
    ///   linear</see> multi-class Support Vector Machine to classify ages into any
    ///   of the previously considered text labels ("child", "adult" or "elder").</para>
    /// 
    /// <code>
    ///   // Now we will be able to feed this matrix to any machine learning
    ///   // algorithm without having to worry about text labels in our data:
    ///   
    ///   // Use the first column as input and the second column a output:
    ///   
    ///   double[][] inputs = matrix.GetColumns(0);      // Age column
    ///   int[] outputs = matrix.GetColumn(1).ToInt32(); // Label column
    ///   
    ///   
    ///   // Create a multi-class SVM for one input (Age) and three classes (Label)
    ///   var machine = new MulticlassSupportVectorMachine(inputs: 1, classes: 3);
    ///   
    ///   // Create a Multi-class learning algorithm for the machine
    ///   var teacher = new MulticlassSupportVectorLearning(machine, inputs, outputs);
    ///   
    ///   // Configure the learning algorithm to use SMO to train the
    ///   //  underlying SVMs in each of the binary class subproblems.
    ///   teacher.Algorithm = (svm, classInputs, classOutputs, i, j) =>
    ///       new SequentialMinimalOptimization(svm, classInputs, classOutputs);
    ///   
    ///   // Run the learning algorithm
    ///   double error = teacher.Run(); // error will be zero
    ///   
    ///   
    ///   // After we have learned the machine, we can use it to classify
    ///   // new data points, and use the codebook to translate the machine
    ///   // outputs to the original text labels:
    ///   
    ///   string result1 = codebook.Translate("Label", machine.Compute(10)); // child
    ///   string result2 = codebook.Translate("Label", machine.Compute(40)); // adult
    ///   string result3 = codebook.Translate("Label", machine.Compute(70)); // elder
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="Normalization"/>
    /// 
    [Serializable]
    public class Codification : BaseFilter<Codification.Options>, IAutoConfigurableFilter
    {

        /// <summary>
        ///   Creates a new Codification Filter.
        /// </summary>
        /// 
        public Codification()
        {
        }

        /// <summary>
        ///   Creates a new Codification Filter.
        /// </summary>
        /// 
        public Codification(DataTable data)
        {
            this.Detect(data);
        }

        /// <summary>
        ///   Creates a new Codification Filter.
        /// </summary>
        /// 
        public Codification(DataTable data, params string[] columns)
        {
            this.Detect(data, columns);
        }

        /// <summary>
        ///   Creates a new Codification Filter.
        /// </summary>
        /// 
        public Codification(string columnName, params string[] values)
        {
            parseColumn(columnName, values);
        }

        /// <summary>
        ///   Creates a new Codification Filter.
        /// </summary>
        /// 
        public Codification(string[] columnNames, string[][] values)
        {
            Detect(columnNames, values);
        }

        /// <summary>
        ///   Creates a new Codification Filter.
        /// </summary>
        /// 
        public Codification(string columnName, string[][] values)
        {
            Detect(columnName, values);
        }


        /// <summary>
        ///   Translates a value of a given variable
        ///   into its integer (codeword) representation.
        /// </summary>
        /// 
        /// <param name="columnName">The name of the variable's data column.</param>
        /// <param name="value">The value to be translated.</param>
        /// 
        /// <returns>An integer which uniquely identifies the given value
        /// for the given variable.</returns>
        /// 
        public int Translate(string columnName, string value)
        {
            return Columns[columnName].Mapping[value];
        }

        /// <summary>
        ///   Translates an array of values into their
        ///   integer representation, assuming values
        ///   are given in original order of columns.
        /// </summary>
        /// 
        /// <param name="data">The values to be translated.</param>
        /// 
        /// <returns>An array of integers in which each value
        /// uniquely identifies the given value for each of
        /// the variables.</returns>
        /// 
        public int[] Translate(params string[] data)
        {
            int[] result = new int[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                for (int j = 0; j < Columns.Count; j++)
                {
                    Options options = this.Columns[j];
                    if (options.Mapping.TryGetValue(data[i], out result[i]))
                        break;
                }
            }

            return result;
        }

        /// <summary>
        ///   Translates an array of values into their
        ///   integer representation, assuming values
        ///   are given in original order of columns.
        /// </summary>
        /// 
        /// <param name="row">A <see cref="DataRow"/> containing the values to be translated.</param>
        /// <param name="columnNames">The columns of the <paramref name="row"/> containing the
        /// values to be translated.</param>
        /// 
        /// <returns>An array of integers in which each value
        /// uniquely identifies the given value for each of
        /// the variables.</returns>
        /// 
        public int[] Translate(DataRow row, params string[] columnNames)
        {
            int[] result = new int[columnNames.Length];

            for (int i = 0; i < columnNames.Length; i++)
            {
                string name = columnNames[i];
                string value = row[name] as string;

                Options options = this.Columns[name];
                result[i] = options.Mapping[value];
            }

            return result;
        }

        /// <summary>
        ///   Translates a value of the given variables
        ///   into their integer (codeword) representation.
        /// </summary>
        /// 
        /// <param name="columnNames">The names of the variable's data column.</param>
        /// <param name="values">The values to be translated.</param>
        /// 
        /// <returns>An array of integers in which each integer
        /// uniquely identifies the given value for the given 
        /// variables.</returns>
        /// 
        public int[] Translate(string[] columnNames, string[] values)
        {
            int[] result = new int[values.Length];

            for (int i = 0; i < columnNames.Length; i++)
            {
                Options options = this.Columns[columnNames[i]];
                result[i] = options.Mapping[values[i]];
            }

            return result;
        }

        /// <summary>
        ///   Translates a value of the given variables
        ///   into their integer (codeword) representation.
        /// </summary>
        /// 
        /// <param name="columnName">The variable name.</param>
        /// <param name="values">The values to be translated.</param>
        /// 
        /// <returns>An array of integers in which each integer
        /// uniquely identifies the given value for the given 
        /// variables.</returns>
        /// 
        public int[] Translate(string columnName, string[] values)
        {
            int[] result = new int[values.Length];

            Options options = this.Columns[columnName];
            for (int i = 0; i < result.Length; i++)
                result[i] = options.Mapping[values[i]];

            return result;
        }

        /// <summary>
        ///   Translates a value of the given variables
        ///   into their integer (codeword) representation.
        /// </summary>
        /// 
        /// <param name="columnName">The variable name.</param>
        /// <param name="values">The values to be translated.</param>
        /// 
        /// <returns>An array of integers in which each integer
        /// uniquely identifies the given value for the given 
        /// variables.</returns>
        /// 
        public int[][] Translate(string columnName, string[][] values)
        {
            int[][] result = new int[values.Length][];

            Options options = this.Columns[columnName];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new int[values[i].Length];
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = options.Mapping[values[i][j]];
            }

            return result;
        }

        /// <summary>
        ///   Translates an integer (codeword) representation of
        ///   the value of a given variable into its original
        ///   value.
        /// </summary>
        /// 
        /// <param name="columnName">The variable name.</param>
        /// <param name="codeword">The codeword to be translated.</param>
        /// 
        /// <returns>The original meaning of the given codeword.</returns>
        /// 
        public string Translate(string columnName, int codeword)
        {
            Options options = this.Columns[columnName];
            foreach (var pair in options.Mapping)
            {
                if (pair.Value == codeword)
                    return pair.Key;
            }

            return null;
        }

        /// <summary>
        ///   Translates an integer (codeword) representation of
        ///   the value of a given variable into its original
        ///   value.
        /// </summary>
        /// 
        /// <param name="columnName">The name of the variable's data column.</param>
        /// <param name="codewords">The codewords to be translated.</param>
        /// 
        /// <returns>The original meaning of the given codeword.</returns>
        /// 
        public string[] Translate(string columnName, int[] codewords)
        {
            string[] result = new string[codewords.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = Translate(columnName, codewords[i]);

            return result;
        }

        /// <summary>
        ///   Translates the integer (codeword) representations of
        ///   the values of the given variables into their original
        ///   values.
        /// </summary>
        /// 
        /// <param name="columnNames">The name of the variables' columns.</param>
        /// <param name="codewords">The codewords to be translated.</param>
        /// 
        /// <returns>The original meaning of the given codewords.</returns>
        /// 
        public string[] Translate(string[] columnNames, int[] codewords)
        {
            string[] result = new string[codewords.Length];

            for (int i = 0; i < columnNames.Length; i++)
            {
                Options options = this.Columns[columnNames[i]];
                foreach (var pair in options.Mapping)
                {
                    if (pair.Value == codewords[i])
                    {
                        result[i] = pair.Key;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        ///   Processes the current filter.
        /// </summary>
        /// 
        protected override DataTable ProcessFilter(DataTable data)
        {
            // Copy only the schema (Clone)
            DataTable result = data.Clone();

            // For each column having a mapping
            foreach (Options options in Columns)
            {
                // Change its type from string to integer
                result.Columns[options.ColumnName].DataType = typeof(int);
            }


            // Now for each row on the original table
            foreach (DataRow inputRow in data.Rows)
            {
                // We'll import to the result table
                DataRow resultRow = result.NewRow();

                // For each column in original table
                foreach (DataColumn column in data.Columns)
                {
                    string name = column.ColumnName;

                    // If the column has a mapping
                    if (Columns.Contains(name))
                    {
                        var map = Columns[name].Mapping;

                        // Retrieve string value
                        string label = inputRow[name] as string;

                        // Get its corresponding integer
                        int value = map[label];

                        // Set the row to the integer
                        resultRow[name] = value;
                    }
                    else
                    {
                        // The column does not have a mapping
                        //  so we'll just copy the value over
                        resultRow[name] = inputRow[name];
                    }
                }

                // Finally, add the row into the result table
                result.Rows.Add(resultRow);
            }

            return result;
        }

        /// <summary>
        ///   Auto detects the filter options by analyzing a given <see cref="System.Data.DataTable"/>.
        /// </summary> 
        ///  
        public void Detect(DataTable data, string[] columns)
        {
            foreach (string column in columns)
                parseColumn(data, data.Columns[column]);
        }

        /// <summary>
        ///   Auto detects the filter options by analyzing a given <see cref="System.Data.DataTable"/>.
        /// </summary> 
        ///  
        public void Detect(DataTable data)
        {
            foreach (DataColumn column in data.Columns)
                parseColumn(data, column);
        }

        /// <summary>
        ///   Auto detects the filter options by analyzing a set of string labels.
        /// </summary>
        /// 
        /// <param name="columnName">The variable name.</param>
        /// <param name="values">A set of values that this variable can assume.</param>
        /// 
        public void Detect(string columnName, string[][] values)
        {
            parseColumn(columnName, values.Reshape(0));
        }

        /// <summary>
        ///   Auto detects the filter options by analyzing a set of string labels.
        /// </summary>
        /// 
        /// <param name="columnNames">The variable names.</param>
        /// <param name="values">A set of values that those variable can assume.
        ///   The first element of the array is assumed to be related to the first
        ///   <paramref name="columnNames">column name</paramref> parameter.</param>
        /// 
        public void Detect(string[] columnNames, string[][] values)
        {
            for (int i = 0; i < columnNames.Length; i++)
                parseColumn(columnNames[i], values[i]);
        }




        private void parseColumn(string name, string[] values)
        {
            string[] distinct = values.Distinct();

            var map = new Dictionary<string, int>();
            Columns.Add(new Options(name, map));

            for (int j = 0; j < distinct.Length; j++)
                map.Add(distinct[j], j);
        }

        private void parseColumn(DataTable data, DataColumn column)
        {
            // If the column has string type
            if (column.DataType == typeof(String))
            {
                // We'll create a mapping
                string name = column.ColumnName;
                var map = new Dictionary<string, int>();
                Columns.Add(new Options(name, map));

                // Do a select distinct to get distinct values
                DataTable d = data.DefaultView.ToTable(true, name);

                // For each distinct value, create a corresponding integer
                for (int i = 0; i < d.Rows.Count; i++)
                {
                    // And register the String->Integer mapping
                    map.Add(d.Rows[i][0] as string, i);
                }
            }
        }

        /// <summary>
        ///   Options for processing a column.
        /// </summary>
        /// 
        [Serializable]
        public class Options : ColumnOptionsBase
        {
            /// <summary>
            ///   Gets or sets the label mapping for translating
            ///   integer labels to the original string labels.
            /// </summary>
            /// 
            public Dictionary<string, int> Mapping { get; private set; }

            /// <summary>
            ///   Gets the number of symbols used to code this variable.
            /// </summary>
            /// 
            public int Symbols { get { return Mapping.Count; } }

            /// <summary>
            ///   Constructs a new Options object for the given column.
            /// </summary>
            /// 
            /// <param name="name">
            ///   The name of the column to create this options for.
            /// </param>
            /// 
            public Options(String name)
                : base(name)
            {
                this.Mapping = new Dictionary<string, int>();
            }

            /// <summary>
            ///   Constructs a new Options object for the given column.
            /// </summary>
            /// 
            /// <param name="name">
            ///   The name of the column to create this options for.
            /// </param>
            /// 
            /// <param name="map">The initial mapping for this column.</param>
            /// 
            public Options(String name, Dictionary<string, int> map)
                : base(name)
            {
                this.Mapping = map;
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
