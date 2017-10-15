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

// TODO: Uncomment the following line
// #define USE_SORTED_ORDER

namespace Accord.Statistics.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Accord.Math;
    using Accord.Collections;
    using MachineLearning;
    using Accord.Compat;
    using System.Threading;
    using System.Runtime.Serialization;

    public partial class Codification<T>
    {
        /// <summary>
        ///   Options for processing a column.
        /// </summary>
        /// 
        [Serializable]
#if NETSTANDARD2_0
        [SurrogateSelector(typeof(Codification.Selector))]
#endif
        public class Options : ColumnOptionsBase<Codification<T>>,
            ITransform<T, int>, IClassifier<T, int>,
            ITransform<T, double[]>,
            IUnsupervisedLearning<Options, T, int>,
            IUnsupervisedLearning<Options, T, double[]>
        {

            bool hasMissingValue;
            T missingValue;

            object missingValueReplacement;

            /// <summary>
            ///   Gets or sets the label mapping for translating
            ///   integer labels to the original string labels.
            /// </summary>
            /// 
            public TwoWayDictionary<T, int> Mapping { get; private set; }

            /// <summary>
            ///   Gets or sets whether the column can contain missing values.
            /// </summary>
            /// 
            public bool HasMissingValue
            {
                get { return hasMissingValue; }
                set { hasMissingValue = value; }
            }

            /// <summary>
            ///   Gets or sets how missing values are represented in this column.
            /// </summary>
            /// 
            public T MissingValue
            {
                get { return missingValue; }
                set { missingValue = value; }
            }

            /// <summary>
            ///   Gets or sets a value to be used to replace missing values. Default
            ///   is to replace missing values using <c>System.DBNull.Value</c>.
            /// </summary>
            /// 
            public object MissingValueReplacement
            {
                get
                {
                    if (missingValueReplacement == null)
                        return Owner.DefaultMissingValueReplacement;
                    return missingValueReplacement;
                }
                set { missingValueReplacement = value; }
            }

            /// <summary>
            ///   Gets the number of symbols used to code this variable.
            /// </summary>
            /// 
            [Obsolete("Please use NumberOfSymbols instead.")]
            public int Symbols { get { return NumberOfSymbols; } }

            /// <summary>
            ///   Gets the number of symbols used to code this variable. See remarks for details.
            /// </summary>
            /// 
            /// <remarks>
            ///   This method returns the following table of values:
            ///   <list type="table">  
            ///     <listheader>  
            ///         <term><see cref="CodificationVariable.Categorical"/></term>  
            ///         <description>Number of elements in <see cref="Mapping"/> (number of distinct elements).</description>  
            ///     </listheader>  
            ///     <item>  
            ///         <term><see cref="CodificationVariable.CategoricalWithBaseline"/></term>  
            ///         <description>Number of elements in <see cref="Mapping"/> (number of distinct elements).</description>  
            ///     </item>  
            ///     <item>  
            ///         <term><see cref="CodificationVariable.Ordinal"/></term>  
            ///         <description>Number of elements in <see cref="Mapping"/> (number of distinct elements).</description>  
            ///     </item>  
            ///     <item>  
            ///         <term><see cref="CodificationVariable.Continuous"/></term>  
            ///         <description>1 (there are no symbols to be encoded).</description>  
            ///     </item>  
            ///     <item>  
            ///         <term><see cref="CodificationVariable.Discrete"/></term>  
            ///         <description>1 (there are no symbols to be encoded).</description>  
            ///     </item>  
            /// </list>  
            /// </remarks>
            /// 
            public int NumberOfSymbols
            {
                get
                {
                    switch (VariableType)
                    {
                        case CodificationVariable.Categorical:
                        case CodificationVariable.Ordinal:
                        case CodificationVariable.CategoricalWithBaseline:
                            return Mapping.Count;
                        case CodificationVariable.Continuous:
                        case CodificationVariable.Discrete:
                            return 1;
                    }

                    throw new InvalidOperationException(String.Format("Unexpected VariableType {0}." +
                        " Execution should never have reached here.", VariableType));
                }
            }

            /// <summary>
            ///   Gets the codification type that should be used for this variable.
            /// </summary>
            /// 
            public CodificationVariable VariableType { get; set; }

            /// <summary>
            ///   Gets the number of inputs accepted by the model (value will be 1).
            /// </summary>
            /// 
            /// <value>The number of inputs.</value>
            /// 
            public int NumberOfInputs
            {
                get { return 1; }
            }

            /// <summary>
            /// Gets the number of outputs generated by the model. See remarks for details.
            /// </summary>
            /// 
            /// <value>The number of outputs.</value>
            /// 
            /// <remarks>
            ///   This method returns the following table of values:
            ///   <list type="table">  
            ///     <listheader>  
            ///         <term><see cref="CodificationVariable.Categorical"/></term>  
            ///         <description>Number of elements in <see cref="Mapping"/> (number of distinct elements).</description>  
            ///     </listheader>  
            ///     <item>  
            ///         <term><see cref="CodificationVariable.CategoricalWithBaseline"/></term>  
            ///         <description>Number of elements in <see cref="Mapping"/> minus 1 (number of distinct elements, except the baseline, which is encoded as the absence of other values).</description>  
            ///     </item>  
            ///     <item>  
            ///         <term><see cref="CodificationVariable.Ordinal"/></term>  
            ///         <description>1 (there is just one single ordinal variable).</description>  
            ///     </item>  
            ///     <item>  
            ///         <term><see cref="CodificationVariable.Continuous"/></term>  
            ///         <description>1 (there is just one single continuous variable).</description>  
            ///     </item>  
            ///     <item>  
            ///         <term><see cref="CodificationVariable.Discrete"/></term>  
            ///         <description>1 (there is just one single discrete variable).</description>  
            ///     </item>  
            /// </list>  
            /// </remarks>
            /// 
            public int NumberOfOutputs
            {
                get
                {
                    switch (VariableType)
                    {
                        case CodificationVariable.Categorical:
                            return Mapping.Count;
                        case CodificationVariable.CategoricalWithBaseline:
                            return Mapping.Count - 1;
                        case CodificationVariable.Ordinal:
                        case CodificationVariable.Continuous:
                        case CodificationVariable.Discrete:
                            return 1;
                    }

                    throw new InvalidOperationException(String.Format("Unexpected VariableType {0}." +
                        " Execution should never have reached here.", VariableType));
                }
            }

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
            /// Gets or sets the number of classes expected and recognized by the classifier.
            /// </summary>
            /// <value>The number of classes.</value>
            /// <exception cref="System.NotImplementedException"></exception>
            public int NumberOfClasses
            {
                get { return NumberOfSymbols; }
                set { throw new NotImplementedException(); }
            }


            /// <summary>
            ///   Determines whether the given object denotes a missing value.
            /// </summary>
            /// 
            public bool IsMissingValue(object value)
            {
#if !NETSTANDARD1_4
                return this.HasMissingValue && (value is DBNull || value == null || Object.Equals(this.MissingValue, value));
#else
                return this.HasMissingValue && (value == null || Object.Equals(this.MissingValue, value));
#endif
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
                if (hasMissingValue && input.IsEqual(missingValue))
                    return -1;
#if !NETSTANDARD1_4
                Check();
#endif
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

#if !NETSTANDARD1_4
            /// <summary>
            /// Applies the transformation to an input, producing an associated output.
            /// </summary>
            /// <param name="input">The input data to which the transformation should be applied.</param>
            /// <returns>The output generated by applying this transformation to the given input.</returns>
            public int Transform(DataRow input)
            {
                Check();
                return Transform((T)input[ColumnName]);
            }

            private void Check()
            {
                if (VariableType == CodificationVariable.Continuous)
                    throw new InvalidOperationException("The variable is continuous and cannot be codified as an integer.");
                if (VariableType == CodificationVariable.Discrete)
                    throw new InvalidOperationException("The variable is discrete and cannot be codified as an integer.");
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
#endif

            /// <summary>
            /// Reverts the transformation to a set of output vectors,
            /// producing an associated set of input vectors.
            /// </summary>
            /// <param name="output">The input data to which
            /// the transformation should be reverted.</param>
            public T Revert(int output)
            {
                if (hasMissingValue && output == -1)
                    return missingValue;
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

            private bool CanBeCodified
            {
                get
                {
                    return VariableType == CodificationVariable.Categorical ||
                           VariableType == CodificationVariable.CategoricalWithBaseline ||
                           VariableType == CodificationVariable.Ordinal;
                }
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
                return learn(weights, () =>
                {
                    // Do a select distinct to get distinct values
                    T[] unique = x.Distinct();
#if USE_SORTED_ORDER
                    Array.Sort(unique);
#endif
                    return unique;
                });
            }

#if !NETSTANDARD1_4
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
                return learn(weights, () =>
                {
                    // Do a select distinct to get distinct values
                    DataTable d = x.DefaultView.ToTable(true, ColumnName);
                    object[] unique = new object[d.Rows.Count];
                    for (int i = 0; i < d.Rows.Count; i++)
                        unique[i] = d.Rows[i][ColumnName];
#if USE_SORTED_ORDER
                    Array.Sort(unique);
#endif
                    return unique;
                });
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
                return learn(weights, () =>
                {
                    // Do a select distinct to get distinct values
                    object[] unique = x.Apply(xi => xi[ColumnName]).Distinct();
#if USE_SORTED_ORDER
                    Array.Sort(unique);
#endif
                    return unique;
                });
            }
#endif
            private Options learn<TObject>(double[] weights, Func<TObject[]> uniqueFunction)
            {
                if (weights != null)
                    throw new ArgumentException(Accord.Properties.Resources.NotSupportedWeights, "weights");

                if (CanBeCodified)
                {
                    TObject[] unique = uniqueFunction();

                    // For each distinct value, create a corresponding integer
                    for (int i = 0; i < unique.Length; i++)
                    {
                        TryAddValue(unique[i]);

                        if (this.Token.IsCancellationRequested)
                            return this;
                    }
                }

                return this;
            }

            private void TryAddValue(object value)
            {
#if NETSTANDARD1_4
                if (value == null)
#else
                if (value == null || value is DBNull)
#endif
                {
                    if (!hasMissingValue)
                    {
                        hasMissingValue = true;
                        missingValue = (T)System.Convert.ChangeType(null, typeof(T));
                    }
                }
                else
                {
                    T key = (T)value;
                    if (!Mapping.ContainsKey(key))
                        Mapping.Add(key, Mapping.Count); // And register the String->Integer mapping
                }
            }

            /// <summary>
            /// Computes a class-label decision for a given <paramref name="input" />.
            /// </summary>
            /// <param name="input">The input vector that should be classified into
            /// one of the <see cref="P:Accord.MachineLearning.ITransform.NumberOfOutputs" /> possible classes.</param>
            /// <returns>A class-label that best described <paramref name="input" /> according
            /// to this classifier.</returns>
            public int Decide(T input)
            {
                return Transform(input);
            }

            /// <summary>
            /// Computes class-label decisions for each vector in the given <paramref name="input" />.
            /// </summary>
            /// <param name="input">The input vectors that should be classified into
            /// one of the <see cref="P:Accord.MachineLearning.ITransform.NumberOfOutputs" /> possible classes.</param>
            /// <returns>The class-labels that best describe each <paramref name="input" />
            /// vectors according to this classifier.</returns>
            public int[] Decide(T[] input)
            {
                return Transform(input);
            }

            /// <summary>
            /// Computes class-label decisions for each vector in the given <paramref name="input" />.
            /// </summary>
            /// <param name="input">The input vectors that should be classified into
            /// one of the <see cref="P:Accord.MachineLearning.ITransform.NumberOfOutputs" /> possible classes.</param>
            /// <param name="result">The location where to store the class-labels.</param>
            /// <returns>The class-labels that best describe each <paramref name="input" />
            /// vectors according to this classifier.</returns>
            public int[] Decide(T[] input, int[] result)
            {
                return Transform(input, result);
            }

            double[] ICovariantTransform<T, double[]>.Transform(T input)
            {
                return Vector.OneHot(Transform(input), columns: NumberOfClasses);
            }

            double[][] ICovariantTransform<T, double[]>.Transform(T[] input)
            {
                return Transform(input, Jagged.Zeros(input.Length, NumberOfClasses));
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
            public double[][] Transform(T[] input, double[][] result)
            {
                return Jagged.OneHot(Transform(input), result: result);
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
            /// <param name="name">The name of the column to create this options for.</param>
            /// 
            public Options(String name)
                : this(name, new Dictionary<T, int>())
            {
            }

            /// <summary>
            ///   Constructs a new Options object for the given column.
            /// </summary>
            /// 
            /// <param name="name">The name of the column to create this options for.</param>
            /// <param name="variableType">The type of the variable in the column.</param>
            /// 
            public Options(String name, CodificationVariable variableType)
                : this(name)
            {
                this.VariableType = variableType;
            }

            /// <summary>
            ///   Constructs a new Options object for the given column.
            /// </summary>
            /// 
            /// <param name="name">The name of the column to create this options for.</param>
            /// <param name="variableType">The type of the variable in the column.</param>
            /// <param name="order">The order of the variables in the mapping. The first variable
            ///   will be assigned to position (symbol) 1, the second to position 2, and so on.</param>
            /// 
            public Options(String name, CodificationVariable variableType, params T[] order)
                : this(name, variableType)
            {
                if (variableType != CodificationVariable.Categorical && variableType != CodificationVariable.CategoricalWithBaseline && variableType != CodificationVariable.Ordinal)
                    throw new ArgumentException("An 'order' can only be specified when 'variableType' is ordinal, categorical, or categorical with baseline.");

                if (variableType == CodificationVariable.Discrete || variableType == CodificationVariable.Continuous)
                    throw new Exception("Invalid variable type: " + variableType);

                for (int i = 0; i < order.Length; i++)
                    this.Mapping[order[i]] = i;
            }

            /// <summary>
            ///   Constructs a new Options object for the given column.
            /// </summary>
            /// 
            /// <param name="name">The name of the column to create this options for.</param>
            /// <param name="variableType">The type of the variable in the column.</param>
            /// <param name="baseline">The baseline value to be used in conjunction with 
            ///   <see cref="CodificationVariable.CategoricalWithBaseline"/>. The baseline
            ///   value will be treated as absolute zero in a otherwise one-hot representation.</param>
            /// 
            /// 
            public Options(String name, CodificationVariable variableType, T baseline)
                : this(name, variableType)
            {
                if (variableType != CodificationVariable.CategoricalWithBaseline)
                    throw new ArgumentException("A 'baseline' can only be specified when 'variableType' is CodificationVariable.CategoricalWithBaseline.");

                this.Mapping[baseline] = 0;
            }

            /// <summary>
            ///   Constructs a new Options object for the given column.
            /// </summary>
            /// 
            /// <param name="name">The name of the column to create this options for.</param>
            /// 
            /// <param name="map">The initial mapping for this column.</param>
            /// 
            public Options(String name, Dictionary<T, int> map)
                : base(name)
            {
                this.Mapping = new TwoWayDictionary<T, int>(map);
            }



            int ITransform.NumberOfInputs
            {
                get { return NumberOfInputs; }
                set { throw new InvalidOperationException("This property is read-only."); }
            }

            int ITransform.NumberOfOutputs
            {
                get { return NumberOfOutputs; }
                set { throw new InvalidOperationException("This property is read-only."); }
            }
        }
    }
}
