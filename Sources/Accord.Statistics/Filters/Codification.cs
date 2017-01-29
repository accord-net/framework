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

namespace Accord.Statistics.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.ComponentModel;
    using Accord.Math;
    using Accord.Collections;
    using MachineLearning;
    using System.Threading;

    /// <summary>
    ///   Codification type.
    /// </summary>
    /// 
    public enum CodificationVariable
    {
        /// <summary>
        ///   The variable should be codified as an ordinal variable,
        ///   meaning they will be translated to symbols 0, 1, 2, ... n,
        ///   where n is the total number of distinct symbols this variable
        ///   can assume.
        /// </summary>
        /// 
        Ordinal,

        /// <summary>
        ///   This variable should be codified as a 1-of-n vector by creating
        ///   one column for each symbol this variable can assume, and marking
        ///   the column corresponding to the current symbol as 1 and the rest
        ///   as zero.
        /// </summary>
        /// 
        Categorical,

        /// <summary>
        ///   This variable should be codified as a 1-of-(n-1) vector by creating
        ///   one column for each symbol this variable can assume, except the
        ///   first. This is the same as as <see cref="CodificationVariable.Categorical"/>,
        ///   but the first symbol is handled as a baseline (and should be indicated by
        ///   a zero in every column).
        /// </summary>
        /// 
        CategoricalWithBaseline
    }

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
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\Statistics\CodificationFilterSvmTest.cs" region="doc_learn_1" />
    /// 
    /// <para>
    ///   After we have created the codebook, we can use it to feed data with
    ///   categorical variables to method which would otherwise not know how
    ///   to handle text labels data. Continuing with our example, the next
    ///   code section shows how to convert an entire data table into a numerical
    ///   matrix. </para>
    /// 
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\Statistics\CodificationFilterSvmTest.cs" region="doc_learn_2" />
    /// 
    /// <para>
    ///   Finally, by expressing our data in terms of a simple numerical
    ///   matrix we will be able to feed it to any machine learning algorithm.
    ///   The following code section shows how to create a <see cref="Accord.Statistics.Kernels.Linear">
    ///   linear</see> multi-class Support Vector Machine to classify ages into any
    ///   of the previously considered text labels ("child", "adult" or "elder").</para>
    /// 
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\Statistics\CodificationFilterSvmTest.cs" region="doc_learn_3" />
    /// 
    /// <para>
    ///   Every Learn() method in the framework expects the class labels to be contiguous and zero-indexed,
    ///   meaning that if there is a classification problem with n classes, all class labels must be numbers
    ///   ranging from 0 to n-1. However, not every dataset might be in this format and sometimes we will
    ///   have to pre-process the data to be in this format. The example below shows how to use the 
    ///   Codification class to perform such pre-processing.</para>
    ///   
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\MulticlassSupportVectorLearningTest.cs" region="doc_learn_codification" />
    /// </example>
    /// 
    /// <seealso cref="Normalization"/>
    /// 
    [Serializable]
    public class Codification : Codification<string>, IAutoConfigurableFilter
    {
        // TODO: Mark redundant methods as obsolete

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
            : base(data)
        {
        }

        /// <summary>
        ///   Creates a new Codification Filter.
        /// </summary>
        /// 
        public Codification(DataTable data, params string[] columns)
            : base(data, columns)
        {
        }

        /// <summary>
        ///   Creates a new Codification Filter.
        /// </summary>
        /// 
        public Codification(string columnName, params string[] values)
            : base(columnName, values)
        {
        }

        /// <summary>
        ///   Creates a new Codification Filter.
        /// </summary>
        /// 
        public Codification(string[] columnNames, string[][] values)
            : base(columnNames, values)
        {
        }

        /// <summary>
        ///   Creates a new Codification Filter.
        /// </summary>
        /// 
        public Codification(string columnName, string[][] values)
            : base(columnName, values)
        {
        }

        /// <summary>
        ///   Transforms a matrix of key-value pairs (where the first column
        ///   denotes a key, and the second column a value) into their integer
        ///   vector representation.
        /// </summary>
        /// <param name="values">A 2D matrix with two columns, where the first column contains
        ///   the keys (i.e. "Date") and the second column the values (i.e. "14/05/1988").</param>
        ///   
        /// <returns>A vector of integers where each element contains the translation
        ///   of each respective row in the given <paramref name="values"/> matrix.</returns>
        /// 
        public int[] Transform(string[,] values)
        {
            int rows = values.Rows();
            int cols = values.Columns();
            if (cols != 2)
                throw new DimensionMismatchException("values", "The matrix should contain two columns. The first column should contain the key, and the second should contain the value.");

            int[] result = new int[rows];
            for (int i = 0; i < rows; i++)
                result[i] = Columns[values[i, 0]].Transform(values[i, 1]);
            return result;
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
        [Obsolete("Please use Transform(columnName, value) instead.")]
        public int Translate(string columnName, string value)
        {
            return Transform(columnName, value);
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
        [Obsolete("Please use Transform(data) instead.")]
        public int[] Translate(params string[] data)
        {
            return Transform(data);
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
        [Obsolete("Please use Transform(row, columnNames) instead.")]
        public int[] Translate(DataRow row, params string[] columnNames)
        {
            return Transform(row, columnNames);
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
        [Obsolete("Please use Transform(columnNames, values) instead.")]
        public int[] Translate(string[] columnNames, string[] values)
        {
            return Transform(columnNames, values);
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
        [Obsolete("Please use Transform(columnName, value) instead.")]
        public int[] Translate(string columnName, string[] values)
        {
            return Transform(columnName, values);
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
        [Obsolete("Please use Transform(columnName, values) instead.")]
        public int[][] Translate(string columnName, string[][] values)
        {
            return Transform(columnName, values);
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
        [Obsolete("Please use Revert(columnName, codeword) instead.")]
        public string Translate(string columnName, int codeword)
        {
            return Revert(columnName, codeword);
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
        [Obsolete("Please use Revert(columnName, codewords) instead.")]
        public string[] Translate(string columnName, int[] codewords)
        {
            return Revert(columnName, codewords);
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
        [Obsolete("Please use Revert(columnNames, codewords) instead.")]
        public string[] Translate(string[] columnNames, int[] codewords)
        {
            return Revert(columnNames, codewords);
        }

        /// <summary>
        ///   Auto detects the filter options by analyzing a given <see cref="System.Data.DataTable"/>.
        /// </summary> 
        ///  
        public void Detect(DataTable data, string[] columns)
        {
            for (int i = 0; i < columns.Length; i++)
                Columns.Add(new Options(columns[i]).Learn(data));
        }

        /// <summary>
        ///   Auto detects the filter options by analyzing a given <see cref="System.Data.DataTable"/>.
        /// </summary> 
        ///  
        public void Detect(DataTable data)
        {
            Learn(data);
        }

        /// <summary>
        ///   Auto detects the filter options by analyzing a set of string labels.
        /// </summary>
        /// 
        /// <param name="columnName">The variable name.</param>
        /// <param name="values">A set of values that this variable can assume.</param>
        /// 
        public void Detect(string columnName, string[] values)
        {
            Columns.Add(new Options(columnName).Learn(values));
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
            Detect(columnName, values.Reshape(0));
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
                Columns.Add(new Options(columnNames[i]).Learn(values.GetColumn(i)));
        }
    }

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
    ///   Every Learn() method in the framework expects the class labels to be contiguous and zero-indexed,
    ///   meaning that if there is a classification problem with n classes, all class labels must be numbers
    ///   ranging from 0 to n-1. However, not every dataset might be in this format and sometimes we will
    ///   have to pre-process the data to be in this format. The example below shows how to use the 
    ///   Codification class to perform such pre-processing.</para>
    ///   
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\MulticlassSupportVectorLearningTest.cs" region="doc_learn_codification" />
    /// </example>
    /// 
    /// <seealso cref="Normalization"/>
    /// <seealso cref="Codification"/>
    /// 
    [Serializable]
    public class Codification<T> : BaseFilter<Codification<T>.Options>,
        ITransform<T[], int[]>, IUnsupervisedLearning<Codification<T>, T[], int[]>
    {
        [NonSerialized]
        private CancellationToken token;

        /// <summary>
        /// Gets the number of inputs accepted by the model.
        /// </summary>
        /// <value>The number of inputs.</value>
        public int NumberOfInputs { get { return this.Columns.Count; } }

        /// <summary>
        /// Gets the number of outputs generated by the model.
        /// </summary>
        /// <value>The number of outputs.</value>
        public int NumberOfOutputs { get { return this.Columns.Count; } }

        /// <summary>
        /// Gets or sets a cancellation token that can be used to
        /// stop the learning algorithm while it is running.
        /// </summary>
        /// 
        /// <value>The token.</value>
        /// 
        public CancellationToken Token
        {
            get { return token; }
            set { token = value; }
        }

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
            : this()
        {
            this.Learn(data);
        }

        /// <summary>
        ///   Creates a new Codification Filter.
        /// </summary>
        /// 
        public Codification(DataTable data, params string[] columns)
            : this()
        {
            for (int i = 0; i < columns.Length; i++)
                Columns.Add(new Options(columns[i]).Learn(data));
        }

        /// <summary>
        ///   Creates a new Codification Filter.
        /// </summary>
        /// 
        public Codification(string columnName, params T[] values)
            : this()
        {
            Columns.Add(new Options(columnName).Learn(values));
        }

        /// <summary>
        ///   Creates a new Codification Filter.
        /// </summary>
        /// 
        public Codification(string[] columnNames, T[][] values)
            : this()
        {
            for (int i = 0; i < columnNames.Length; i++)
                Columns.Add(new Options(columnNames[i]).Learn(values.GetColumn(i)));
        }

        /// <summary>
        ///   Creates a new Codification Filter.
        /// </summary>
        /// 
        public Codification(string columnName, T[][] values)
            : this()
        {
            Columns.Add(new Options(columnName).Learn(values.Concatenate()));
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
        public int Transform(string columnName, T value)
        {
            return Columns[columnName].Transform(value);
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
        public int[] Transform(params T[] data)
        {
            if (this.Columns.Count == 1)
                return this.Columns[0].Transform(data);

            if (data.Length > this.Columns.Count)
            {
                throw new ArgumentException("The array contains more values"
                    + " than the number of known columns.", "data");
            }

            int[] result = new int[data.Length];
            for (int i = 0; i < data.Length; i++)
                result[i] = this.Columns[i].Transform(data[i]);
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
        public int[] Transform(DataRow row, params string[] columnNames)
        {
            var result = new int[columnNames.Length];
            for (int i = 0; i < columnNames.Length; i++)
                result[i] = this.Columns[columnNames[i]].Transform(row);
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
        public int[] Transform(string[] columnNames, T[] values)
        {
            if (columnNames.Length != values.Length)
            {
                throw new ArgumentException("The number of column names"
                    + " and the number of values must match.", "values");
            }

            var result = new int[values.Length];
            for (int i = 0; i < columnNames.Length; i++)
                result[i] = this.Columns[columnNames[i]].Transform(values[i]);
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
        public int[] Transform(string columnName, T[] values)
        {
            return this.Columns[columnName].Transform(values);
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
        public int[][] Transform(string columnName, T[][] values)
        {
            return values.Apply(x => this.Columns[columnName].Transform(x));
        }

        /// <summary>
        ///   Translates a value of the given variables
        ///   into their integer (codeword) representation.
        /// </summary>
        /// 
        /// <param name="input">The values to be translated.</param>
        /// 
        /// <returns>An array of integers in which each integer
        /// uniquely identifies the given value for the given 
        /// variables.</returns>
        /// 
        public int[][] Transform(T[][] input)
        {
            var result = new int[input.Length][];
            for (int i = 0; i < input.Length; i++)
                result[i] = Transform(input[i]);
            return result;
        }

        /// <summary>
        ///   Translates a value of the given variables
        ///   into their integer (codeword) representation.
        /// </summary>
        /// 
        /// <param name="input">The values to be translated.</param>
        /// <param name="result">The location to where to store the
        /// result of this transformation.</param>
        /// 
        /// <returns>An array of integers in which each integer
        /// uniquely identifies the given value for the given 
        /// variables.</returns>
        /// 
        public int[][] Transform(T[][] input, int[][] result)
        {
            for (int i = 0; i < input.Length; i++)
                result[i] = Transform(input[i]);
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
        public T Revert(string columnName, int codeword)
        {
            return this.Columns[columnName].Revert(codeword);
        }

        /// <summary>
        ///   Translates an integer (codeword) representation of
        ///   the value of a given variable into its original
        ///   value.
        /// </summary>
        /// 
        /// <param name="codewords">The codewords to be translated.</param>
        /// 
        /// <returns>The original meaning of the given codeword.</returns>
        /// 
        public T[] Revert(int[] codewords)
        {
            if (this.Columns.Count != 1)
                throw new InvalidOperationException("This method can only be called when there is a single output column.");

            return this.Columns[0].Revert(codewords);
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
        public T[] Revert(string columnName, int[] codewords)
        {
            return this.Columns[columnName].Revert(codewords);
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
        public T[] Revert(string[] columnNames, int[] codewords)
        {
            var result = new T[codewords.Length];
            for (int i = 0; i < columnNames.Length; i++)
                result[i] = Revert(columnNames[i], codewords[i]);
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
                if (!result.Columns.Contains(options.ColumnName))
                    continue;

                // If we are just converting strings to integer codes
                if (options.VariableType == CodificationVariable.Ordinal)
                {
                    // Change its type from string to integer
                    result.Columns[options.ColumnName].MaxLength = -1;
                    result.Columns[options.ColumnName].DataType = typeof(int);
                }

                // If we want to avoid implying an order relationship between them
                else if (options.VariableType == CodificationVariable.Categorical)
                {
                    // Create extra columns for each possible value
                    for (int i = 0; i < options.NumberOfSymbols; i++)
                    {
                        // Except for the first, that should be the baseline value
                        T symbolName = options.Mapping.Reverse[i];
                        string factorName = getFactorName(options, symbolName);

                        result.Columns.Add(new DataColumn(factorName, typeof(int))
                        {
                            DefaultValue = 0
                        });
                    }

                    // Remove the column from the schema
                    result.Columns.Remove(options.ColumnName);
                }

                // If we want to avoid implying an order relationship between them
                else if (options.VariableType == CodificationVariable.CategoricalWithBaseline)
                {
                    // Create extra columns for each possible value
                    for (int i = 1; i < options.NumberOfSymbols; i++)
                    {
                        // Except for the first, that should be the baseline value
                        T symbolName = options.Mapping.Reverse[i];
                        string factorName = getFactorName(options, symbolName);

                        result.Columns.Add(new DataColumn(factorName, typeof(int))
                        {
                            DefaultValue = 0
                        });
                    }

                    // Remove the column from the schema
                    result.Columns.Remove(options.ColumnName);
                }
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
                        var options = Columns[name];
                        var map = options.Mapping;

                        // Retrieve string value
                        T label = (T)inputRow[name];

                        if (options.VariableType == CodificationVariable.Ordinal)
                        {
                            // Get its corresponding integer
                            int value = 0;
                            try { value = map[label]; }
                            catch
                            {
                                value = map.Values.Count + 1;
                                map[label] = value;
                            }

                            // Set the row to the integer
                            resultRow[name] = value;
                        }
                        else if (options.VariableType == CodificationVariable.CategoricalWithBaseline)
                        {
                            if (options.Mapping[label] > 0)
                            {
                                // Find the corresponding column
                                var factorName = getFactorName(options, label);

                                try
                                {
                                    resultRow[factorName] = 1;
                                }
                                catch { }
                            }
                        }
                        else if (options.VariableType == CodificationVariable.Categorical)
                        {
                            // Find the corresponding column
                            var factorName = getFactorName(options, label);

                            try
                            {
                                resultRow[factorName] = 1;
                            }
                            catch { }
                        }
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

        private static string getFactorName(Options options, T name)
        {
            return options.ColumnName + ": " + name;
        }



        /// <summary>
        /// Learns a model that can map the given inputs to the desired outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="weights">The weight of importance for each input sample.</param>
        /// <returns>A model that has learned how to produce suitable outputs
        /// given the input data <paramref name="x" />.</returns>
        public Codification<T> Learn(T[] x, double[] weights = null)
        {
            Columns.Clear();
            Columns.Add(new Options(0.ToString()).Learn(x, weights));
            return this;
        }

        /// <summary>
        /// Learns a model that can map the given inputs to the desired outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="weights">The weight of importance for each input sample.</param>
        /// <returns>A model that has learned how to produce suitable outputs
        /// given the input data <paramref name="x" />.</returns>
        public Codification<T> Learn(T[][] x, double[] weights = null)
        {
            Columns.Clear();
            for (int i = 0; i < x.Columns(); i++)
                Columns.Add(new Options(i.ToString()).Learn(x.GetColumn(i), weights));
            return this;
        }

        /// <summary>
        /// Learns a model that can map the given inputs to the desired outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="weights">The weight of importance for each input sample.</param>
        /// <returns>A model that has learned how to produce suitable outputs
        /// given the input data <paramref name="x" />.</returns>
        public Codification<T> Learn(DataTable x, double[] weights = null)
        {
            Columns.Clear();
            foreach (DataColumn col in x.Columns)
            {
                if (col.DataType == typeof(T))
                    Columns.Add(new Options(col.ColumnName).Learn(x, weights));
            }

            return this;
        }

        /// <summary>
        ///   Options for processing a column.
        /// </summary>
        /// 
        [Serializable]
        public class Options : ColumnOptionsBase,
            ITransform<T, int>,
            IUnsupervisedLearning<Options, T, int>
        {
            [NonSerialized]
            private CancellationToken token;

            /// <summary>
            ///   Gets or sets the label mapping for translating
            ///   integer labels to the original string labels.
            /// </summary>
            /// 
            public TwoWayDictionary<T, int> Mapping { get; private set; }

            /// <summary>
            ///   Gets the number of symbols used to code this variable.
            /// </summary>
            /// 
            [Obsolete("Please use NumberOfSymbols instead.")]
            public int Symbols { get { return Mapping.Count; } }

            /// <summary>
            ///   Gets the number of symbols used to code this variable.
            /// </summary>
            /// 
            public int NumberOfSymbols { get { return Mapping.Count; } }

            /// <summary>
            ///   Gets the codification type that should be used for this variable.
            /// </summary>
            /// 
            public CodificationVariable VariableType { get; set; }

            /// <summary>
            /// Gets or sets a cancellation token that can be used to
            /// stop the learning algorithm while it is running.
            /// </summary>
            /// 
            /// <value>The token.</value>
            /// 
            public CancellationToken Token
            {
                get { return token; }
                set { token = value; }
            }

            /// <summary>
            ///   Gets the number of inputs accepted by the model (value will be 1).
            /// </summary>
            /// 
            /// <value>The number of inputs.</value>
            /// 
            public int NumberOfInputs { get; set; }

            /// <summary>
            /// Gets the number of outputs generated by the model (value will be 1).
            /// </summary>
            /// 
            /// <value>The number of outputs.</value>
            /// 
            public int NumberOfOutputs { get; set; }

            /// <summary>
            ///   Gets the values associated with each symbol, in the order of the symbols.
            /// </summary>
            /// 
            public T[] Values
            {
                get
                {
                    var values = new T[Mapping.Count];
                    for (int i = 0; i < values.Length; i++)
                        values[i] = Mapping.Reverse[i];
                    return values;
                }
            }


            /// <summary>
            ///   Forces the given key to have a specific symbol value.
            /// </summary>
            /// 
            /// <param name="key">The key.</param>
            /// <param name="value">The value that should be associated with this key.</param>
            /// 
            public void Remap(T key, int value)
            {
                int oldValue = Mapping[key];
                T oldKey = Mapping.Reverse[value];

                Mapping[key] = value;
                Mapping[oldKey] = oldValue;
            }

            /// <summary>
            /// Applies the transformation to an input, producing an associated output.
            /// </summary>
            /// <param name="input">The input data to which the transformation should be applied.</param>
            /// <returns>The output generated by applying this transformation to the given input.</returns>
            public int Transform(T input)
            {
                return Mapping[input];
            }

            /// <summary>
            /// Applies the transformation to a set of input vectors,
            /// producing an associated set of output vectors.
            /// </summary>
            /// <param name="input">The input data to which
            /// the transformation should be applied.</param>
            /// <returns>The output generated by applying this
            /// transformation to the given input.</returns>
            public int[] Transform(T[] input)
            {
                return Transform(input, new int[input.Length]);
            }


            /// <summary>
            /// Applies the transformation to a set of input vectors,
            /// producing an associated set of output vectors.
            /// </summary>
            /// <param name="input">The input data to which
            /// the transformation should be applied.</param>
            /// <param name="result">The location to where to store the
            /// result of this transformation.</param>
            /// <returns>The output generated by applying this
            /// transformation to the given input.</returns>
            public int[] Transform(T[] input, int[] result)
            {
                for (int i = 0; i < input.Length; i++)
                    result[i] = Transform(input[i]);
                return result;
            }

            /// <summary>
            /// Applies the transformation to an input, producing an associated output.
            /// </summary>
            /// <param name="input">The input data to which the transformation should be applied.</param>
            /// <returns>The output generated by applying this transformation to the given input.</returns>
            public int Transform(DataRow input)
            {
                return Transform((T)input[ColumnName]);
            }

            /// <summary>
            /// Applies the transformation to an input, producing an associated output.
            /// </summary>
            /// <param name="input">The input data to which the transformation should be applied.</param>
            /// <returns>The output generated by applying this transformation to the given input.</returns>
            public int[] Transform(DataRow[] input)
            {
                return Transform(input, new int[input.Length]);
            }

            /// <summary>
            /// Applies the transformation to a set of input vectors,
            /// producing an associated set of output vectors.
            /// </summary>
            /// <param name="input">The input data to which
            /// the transformation should be applied.</param>
            /// <param name="result">The location to where to store the
            /// result of this transformation.</param>
            /// <returns>The output generated by applying this
            /// transformation to the given input.</returns>
            public int[] Transform(DataRow[] input, int[] result)
            {
                for (int i = 0; i < input.Length; i++)
                    result[i] = Transform(input[i]);
                return result;
            }

            /// <summary>
            /// Reverts the transformation to a set of output vectors,
            /// producing an associated set of input vectors.
            /// </summary>
            /// <param name="output">The input data to which
            /// the transformation should be reverted.</param>
            public T Revert(int output)
            {
                return Mapping.Reverse[output];
            }

            /// <summary>
            /// Reverts the transformation to a set of output vectors,
            /// producing an associated set of input vectors.
            /// </summary>
            /// <param name="output">The input data to which
            /// the transformation should be reverted.</param>
            public T[] Revert(int[] output)
            {
                return Revert(output, new T[output.Length]);
            }

            /// <summary>
            /// Reverts the transformation to a set of output vectors,
            /// producing an associated set of input vectors.
            /// </summary>
            /// <param name="output">The input data to which
            /// the transformation should be reverted.</param>
            /// <param name="result">The location to where to store the
            /// result of this transformation.</param>
            /// <returns>The input generated by reverting this
            /// transformation to the given output.</returns>
            public T[] Revert(int[] output, T[] result)
            {
                for (int i = 0; i < output.Length; i++)
                    result[i] = Revert(output[i]);
                return result;
            }

            /// <summary>
            /// Learns a model that can map the given inputs to the desired outputs.
            /// </summary>
            /// <param name="x">The model inputs.</param>
            /// <param name="weights">The weight of importance for each input sample.</param>
            /// <returns>A model that has learned how to produce suitable outputs
            /// given the input data <paramref name="x" />.</returns>
            /// <exception cref="System.ArgumentException">Weights are not supported and should be null.</exception>
            public Options Learn(T[] x, double[] weights = null)
            {
                if (weights != null)
                    throw new ArgumentException("Weights are not supported and should be null.");

                this.NumberOfInputs = 1;
                this.NumberOfOutputs = 1;

                T[] unique = x.Distinct();

                for (int i = 0; i < unique.Length; i++)
                {
                    this.Mapping[unique[i]] = i;

                    if (this.Token.IsCancellationRequested)
                        return this;
                }

                return this;
            }

            /// <summary>
            /// Learns a model that can map the given inputs to the desired outputs.
            /// </summary>
            /// <param name="x">The model inputs.</param>
            /// <param name="weights">The weight of importance for each input sample.</param>
            /// <returns>A model that has learned how to produce suitable outputs
            /// given the input data <paramref name="x" />.</returns>
            /// <exception cref="System.ArgumentException">Weights are not supported and should be null.</exception>
            public Options Learn(DataTable x, double[] weights = null)
            {
                if (weights != null)
                    throw new ArgumentException("Weights are not supported and should be null.");

                this.NumberOfInputs = 1;
                this.NumberOfOutputs = 1;

                // Do a select distinct to get distinct values
                DataTable d = x.DefaultView.ToTable(true, ColumnName);

                // For each distinct value, create a corresponding integer
                for (int i = 0; i < d.Rows.Count; i++)
                    Mapping.Add((T)d.Rows[i][0], i); // And register the String->Integer mapping

                return this;
            }

            /// <summary>
            /// Learns a model that can map the given inputs to the desired outputs.
            /// </summary>
            /// <param name="x">The model inputs.</param>
            /// <param name="weights">The weight of importance for each input sample.</param>
            /// <returns>A model that has learned how to produce suitable outputs
            /// given the input data <paramref name="x" />.</returns>
            /// <exception cref="System.ArgumentException">Weights are not supported and should be null.</exception>
            public Options Learn(DataRow[] x, double[] weights = null)
            {
                return Learn(x.Apply(xx => (T)xx[ColumnName]), weights);
            }

            /// <summary>
            ///   Constructs a new Options object.
            /// </summary>
            /// 
            public Options()
                : this("New column")
            {
            }

            /// <summary>
            ///   Constructs a new Options object for the given column.
            /// </summary>
            /// 
            /// <param name="name">
            ///   The name of the column to create this options for.
            /// </param>
            /// 
            public Options(String name)
                : this(name, new Dictionary<T, int>())
            {
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
            public Options(String name, Dictionary<T, int> map)
                : base(name)
            {
                this.Mapping = new TwoWayDictionary<T, int>(map);
                this.NumberOfInputs = 1;
                this.NumberOfOutputs = 1;
            }

        }
    }
}
