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

namespace Accord.Statistics.Filters
{
    using System;
    using System.Data;
    using System.ComponentModel;
    using Accord.Math;
    using MachineLearning;

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
        CategoricalWithBaseline,

        /// <summary>
        ///   This variable is continuous and should be not be codified.
        /// </summary>
        /// 
        Continuous,

        /// <summary>
        ///   This variable is discrete and should be not be codified.
        /// </summary>
        /// 
        Discrete
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
    public class Codification : Codification<string>, IAutoConfigurableFilter, ITransform<string[], double[]>
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
}
