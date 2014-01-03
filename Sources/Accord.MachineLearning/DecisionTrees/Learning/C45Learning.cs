// Accord Machine Learning Library
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

namespace Accord.MachineLearning.DecisionTrees.Learning
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Accord.Math;
    using System.Threading.Tasks;
    using AForge;
    using Parallel = System.Threading.Tasks.Parallel;

    /// <summary>
    ///   C4.5 Learning algorithm for <see cref="DecisionTree">Decision Trees</see>.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Quinlan, J. R. C4.5: Programs for Machine Learning. Morgan
    ///       Kaufmann Publishers, 1993.</description></item>
    ///     <item><description>
    ///       Quinlan, J. R. C4.5: Programs for Machine Learning. Morgan
    ///       Kaufmann Publishers, 1993.</description></item>
    ///     <item><description>
    ///       Quinlan, J. R. Improved use of continuous attributes in c4.5. Journal
    ///       of Artificial Intelligence Research, 4:77-90, 1996.</description></item>
    ///     <item><description>
    ///       Mitchell, T. M. Machine Learning. McGraw-Hill, 1997. pp. 55-58. </description></item>
    ///     <item><description><a href="http://en.wikipedia.org/wiki/ID3_algorithm">
    ///       Wikipedia, the free encyclopedia. ID3 algorithm. Available on 
    ///       http://en.wikipedia.org/wiki/ID3_algorithm </a></description></item>
    ///   </list>
    /// </para>   
    /// </remarks>
    ///
    /// <see cref="ID3Learning"/>
    /// 
    /// <example>
    /// <code>
    /// // This example uses the Nursery Database available from the University of
    /// // California Irvine repository of machine learning databases, available at
    /// //
    /// //   http://archive.ics.uci.edu/ml/machine-learning-databases/nursery/nursery.names
    /// //
    /// // The description paragraph is listed as follows.
    /// //
    /// //   Nursery Database was derived from a hierarchical decision model
    /// //   originally developed to rank applications for nursery schools. It
    /// //   was used during several years in 1980's when there was excessive
    /// //   enrollment to these schools in Ljubljana, Slovenia, and the
    /// //   rejected applications frequently needed an objective
    /// //   explanation. The final decision depended on three subproblems:
    /// //   occupation of parents and child's nursery, family structure and
    /// //   financial standing, and social and health picture of the family.
    /// //   The model was developed within expert system shell for decision
    /// //   making DEX (M. Bohanec, V. Rajkovic: Expert system for decision
    /// //   making. Sistemica 1(1), pp. 145-157, 1990.).
    /// //
    /// 
    /// // Let's begin by loading the raw data. This string variable contains
    /// // the contents of the nursery.data file as a single, continuous text.
    /// //
    /// string nurseryData = Resources.nursery;
    /// 
    /// // Those are the input columns available in the data
    /// //
    /// string[] inputColumns = 
    /// {
    ///     "parents", "has_nurs", "form", "children",
    ///     "housing", "finance", "social", "health"
    /// };
    /// 
    /// // And this is the output, the last column of the data.
    /// //
    /// string outputColumn = "output";
    ///             
    /// 
    /// // Let's populate a data table with this information.
    /// //
    /// DataTable table = new DataTable("Nursery");
    /// table.Columns.Add(inputColumns);
    /// table.Columns.Add(outputColumn);
    /// 
    /// string[] lines = nurseryData.Split(
    ///     new[] { Environment.NewLine }, StringSplitOptions.None);
    /// 
    /// foreach (var line in lines)
    ///     table.Rows.Add(line.Split(','));
    /// 
    /// 
    /// // Now, we have to convert the textual, categorical data found
    /// // in the table to a more manageable discrete representation.
    /// //
    /// // For this, we will create a codebook to translate text to
    /// // discrete integer symbols:
    /// //
    /// Codification codebook = new Codification(table);
    /// 
    /// // And then convert all data into symbols
    /// //
    /// DataTable symbols = codebook.Apply(table);
    /// double[][] inputs = symbols.ToArray(inputColumns);
    /// int[] outputs = symbols.ToArray&lt;int>(outputColumn);
    /// 
    /// // From now on, we can start creating the decision tree.
    /// //
    /// var attributes = DecisionVariable.FromCodebook(codebook, inputColumns);
    /// DecisionTree tree = new DecisionTree(attributes, outputClasses: 5);
    /// 
    /// 
    /// // Now, let's create the C4.5 algorithm
    /// C45Learning c45 = new C45Learning(tree);
    /// 
    /// // and learn a decision tree. The value of
    /// //   the error variable below should be 0.
    /// //
    /// double error = c45.Run(inputs, outputs);
    /// 
    /// 
    /// // To compute a decision for one of the input points,
    /// //   such as the 25-th example in the set, we can use
    /// //
    /// int y = tree.Compute(inputs[25]);
    /// 
    /// // Finally, we can also convert our tree to a native
    /// // function, improving efficiency considerably, with
    /// //
    /// Func&lt;double[], int> func = tree.ToExpression().Compile();
    /// 
    /// // Again, to compute a new decision, we can just use
    /// //
    /// int z = func(inputs[25]);
    /// </code>
    /// </example>
    ///
    [Serializable]
    public class C45Learning
    {


        private DecisionTree tree;

        private int maxHeight;
        private double[][] thresholds;
        private IntRange[] inputRanges;
        private int outputClasses;

        private bool[] attributes;


        /// <summary>
        ///   Gets or sets the maximum allowed 
        ///   height when learning a tree.
        /// </summary>
        /// 
        public int MaxHeight
        {
            get { return maxHeight; }
            set
            {
                if (maxHeight <= 0 || maxHeight > attributes.Length)
                    throw new ArgumentOutOfRangeException("value", 
                        "The height must be greater than zero and less than the number of variables in the tree.");
                maxHeight = value;
            }
        }


        /// <summary>
        ///   Creates a new C4.5 learning algorithm.
        /// </summary>
        /// 
        /// <param name="tree">The decision tree to be generated.</param>
        /// 
        public C45Learning(DecisionTree tree)
        {
            // Initial argument checking
            if (tree == null)
                throw new ArgumentNullException("tree");

            this.tree = tree;
            this.attributes = new bool[tree.InputCount];
            this.inputRanges = new IntRange[tree.InputCount];
            this.outputClasses = tree.OutputClasses;
            this.maxHeight = attributes.Length;

            for (int i = 0; i < inputRanges.Length; i++)
                inputRanges[i] = tree.Attributes[i].Range.ToIntRange(false);
        }

        /// <summary>
        ///   Runs the learning algorithm, creating a decision
        ///   tree modeling the given inputs and outputs.
        /// </summary>
        /// 
        /// <param name="inputs">The inputs.</param>
        /// <param name="outputs">The corresponding outputs.</param>
        /// 
        /// <returns>The error of the generated tree.</returns>
        /// 
        public double Run(double[][] inputs, int[] outputs)
        {
            // Initial argument check
            checkArgs(inputs, outputs);

            for (int i = 0; i < attributes.Length; i++)
                attributes[i] = false;

            thresholds = new double[tree.Attributes.Count][];

            List<double> candidates = new List<double>(inputs.Length);

            // 0. Create candidate split thresholds for each attribute
            for (int i = 0; i < tree.Attributes.Count; i++)
            {
                if (tree.Attributes[i].Nature == DecisionVariableKind.Continuous)
                {
                    double[] v = inputs.GetColumn(i);
                    int[] o = (int[])outputs.Clone();

                    Array.Sort(v, o);

                    for (int j = 0; j < v.Length - 1; j++)
                    {
                        // Add as candidate thresholds only adjacent values v[i] and v[i+1]
                        // belonging to different classes, following the results by Fayyad
                        // and Irani (1992). See footnote on Quinlan (1996).

                        if (o[j] != o[j + 1])
                            candidates.Add((v[j] + v[j + 1]) / 2.0);
                    }


                    thresholds[i] = candidates.ToArray();
                    candidates.Clear();
                }
            }


            // 1. Create a root node for the tree
            tree.Root = new DecisionNode(tree);

            split(tree.Root, inputs, outputs);

            return ComputeError(inputs, outputs);
        }

        /// <summary>
        ///   Computes the prediction error for the tree
        ///   over a given set of input and outputs.
        /// </summary>
        /// 
        /// <param name="inputs">The input points.</param>
        /// <param name="outputs">The corresponding output labels.</param>
        /// 
        /// <returns>The percentage error of the prediction.</returns>
        /// 
        public double ComputeError(double[][] inputs, int[] outputs)
        {
            int miss = 0;
            for (int i = 0; i < inputs.Length; i++)
            {
                if (tree.Compute(inputs[i]) != outputs[i])
                    miss++;
            }

            return (double)miss / inputs.Length;
        }

        private void split(DecisionNode root, double[][] input, int[] output)
        {

            // 2. If all examples are for the same class, return the single-node
            //    tree with the output label corresponding to this common class.
            double entropy = Statistics.Tools.Entropy(output, outputClasses);

            if (entropy == 0)
            {
                if (output.Length > 0)
                    root.Output = output[0];
                return;
            }

            // 3. If number of predicting attributes is empty, then return the single-node
            //    tree with the output label corresponding to the most common value of
            //    the target attributes in the examples.
            int predictors = attributes.Count(x => x == false);

            if (predictors <= attributes.Length - maxHeight)
            {
                root.Output = Statistics.Tools.Mode(output);
                return;
            }


            // 4. Otherwise, try to select the attribute which
            //    best explains the data sample subset.

            double[] scores = new double[predictors];
            double[] entropies = new double[predictors];
            double[] thresholds = new double[predictors];
            int[][][] partitions = new int[predictors][][];

            // Retrieve candidate attribute indices
            int[] candidates = new int[predictors];
            for (int i = 0, k = 0; i < attributes.Length; i++)
                if (!attributes[i]) candidates[k++] = i;


            // For each attribute in the data set
#if SERIAL
            for (int i = 0; i < scores.Length; i++)
#else
            Parallel.For(0, scores.Length, i =>
#endif
            {
                scores[i] = computeGainRatio(input, output, candidates[i],
                    entropy, out partitions[i], out thresholds[i]);
            }
#if !SERIAL
);
#endif

            // Select the attribute with maximum gain ratio
            int maxGainIndex; scores.Max(out maxGainIndex);
            var maxGainPartition = partitions[maxGainIndex];
            var maxGainEntropy = entropies[maxGainIndex];
            var maxGainAttribute = candidates[maxGainIndex];
            var maxGainRange = inputRanges[maxGainAttribute];
            var maxGainThreshold = thresholds[maxGainIndex];

            // Mark this attribute as already used
            attributes[maxGainAttribute] = true;

            double[][] inputSubset;
            int[] outputSubset;

            // Now, create next nodes and pass those partitions as their responsibilities. 
            if (tree.Attributes[maxGainAttribute].Nature == DecisionVariableKind.Discrete)
            {
                // This is a discrete nature attribute. We will branch at each
                // possible value for the discrete variable and call recursion.
                DecisionNode[] children = new DecisionNode[maxGainPartition.Length];

                // Create a branch for each possible value
                for (int i = 0; i < children.Length; i++)
                {
                    children[i] = new DecisionNode(tree)
                    {
                        Parent = root,
                        Value = i + maxGainRange.Min,
                        Comparison = ComparisonKind.Equal,
                    };

                    inputSubset = input.Submatrix(maxGainPartition[i]);
                    outputSubset = output.Submatrix(maxGainPartition[i]);
                    split(children[i], inputSubset, outputSubset); // recursion
                }

                root.Branches.AttributeIndex = maxGainAttribute;
                root.Branches.AddRange(children);
            }

            else if (maxGainPartition.Length > 1)
            {
                // This is a continuous nature attribute, and we achieved two partitions
                // using the partitioning scheme. We will branch on two possible settings:
                // either the value is higher than a currently detected optimal threshold 
                // or it is lesser.

                DecisionNode[] children = 
                {
                    new DecisionNode(tree) 
                    {
                        Parent = root, Value = maxGainThreshold,
                        Comparison = ComparisonKind.LessThanOrEqual 
                    },

                    new DecisionNode(tree)
                    {
                        Parent = root, Value = maxGainThreshold,
                        Comparison = ComparisonKind.GreaterThan
                    }
                };

                // Create a branch for lower values
                inputSubset = input.Submatrix(maxGainPartition[0]);
                outputSubset = output.Submatrix(maxGainPartition[0]);
                split(children[0], inputSubset, outputSubset);

                // Create a branch for higher values
                inputSubset = input.Submatrix(maxGainPartition[1]);
                outputSubset = output.Submatrix(maxGainPartition[1]);
                split(children[1], inputSubset, outputSubset);

                root.Branches.AttributeIndex = maxGainAttribute;
                root.Branches.AddRange(children);
            }
            else
            {
                // This is a continuous nature attribute, but all variables are equal
                // to a constant. If there is only a constant value as the predictor 
                // and there are multiple output labels associated with this constant
                // value, there isn't much we can do. This node will be a leaf.

                // We will set the class label for this node as the
                // majority of the currently selected output classes.

                outputSubset = output.Submatrix(maxGainPartition[0]);
                root.Output = Statistics.Tools.Mode(outputSubset);
            }

            attributes[maxGainAttribute] = false;
        }


        private double computeGainRatio(double[][] input, int[] output, int attributeIndex,
            double entropy, out int[][] partitions, out double threshold)
        {
            double infoGain = computeInfoGain(input, output, attributeIndex, entropy, out partitions, out threshold);
            double splitInfo = Measures.SplitInformation(output.Length, partitions);

            return infoGain == 0 || splitInfo == 0 ? 0 : infoGain / splitInfo;
        }

        private double computeInfoGain(double[][] input, int[] output, int attributeIndex,
            double entropy, out int[][] partitions, out double threshold)
        {
            threshold = 0;

            if (tree.Attributes[attributeIndex].Nature == DecisionVariableKind.Discrete)
                return entropy - computeInfoDiscrete(input, output, attributeIndex, out partitions);

            return entropy + computeInfoContinuous(input, output, attributeIndex, out partitions, out threshold);
        }

        private double computeInfoDiscrete(double[][] input, int[] output,
            int attributeIndex, out int[][] partitions)
        {
            // Compute the information gain obtained by using
            // this current attribute as the next decision node.
            double info = 0;

            IntRange valueRange = inputRanges[attributeIndex];
            partitions = new int[valueRange.Length + 1][];


            // For each possible value of the attribute
            for (int i = 0; i < partitions.Length; i++)
            {
                int value = valueRange.Min + i;

                // Partition the remaining data set
                // according to the attribute values
                partitions[i] = input.Find(x => x[attributeIndex] == value);

                // For each of the instances under responsibility
                // of this node, check which have the same value
                int[] outputSubset = output.Submatrix(partitions[i]);

                // Check the entropy gain originating from this partitioning
                double e = Statistics.Tools.Entropy(outputSubset, outputClasses);

                info += ((double)outputSubset.Length / output.Length) * e;
            }

            return info;
        }

        private double computeInfoContinuous(double[][] input, int[] output,
            int attributeIndex, out int[][] partitions, out double threshold)
        {
            // Compute the information gain obtained by using
            // this current attribute as the next decision node.
            double[] t = thresholds[attributeIndex];

            double bestGain = Double.NegativeInfinity;
            double bestThreshold = t[0];
            partitions = null;

            // For each possible splitting point of the attribute
            for (int i = 0; i < t.Length; i++)
            {
                // Partition the remaining data set
                // according to the threshold value
                double value = t[i];

                int[] idx1 = input.Find(x => x[attributeIndex] <= value);
                int[] idx2 = input.Find(x => x[attributeIndex] > value);

                int[] output1 = output.Submatrix(idx1);
                int[] output2 = output.Submatrix(idx2);

                double p1 = (double)idx1.Length / output.Length;
                double p2 = (double)idx2.Length / output.Length;

                double splitGain =
                    -p1 * Statistics.Tools.Entropy(output1, outputClasses) +
                    -p2 * Statistics.Tools.Entropy(output2, outputClasses);

                if (splitGain > bestGain)
                {
                    bestThreshold = value;
                    bestGain = splitGain;

                    if (idx1.Length > 0 && idx2.Length > 0)
                        partitions = new int[][] { idx1, idx2 };
                    else if (idx1.Length > 0)
                        partitions = new int[][] { idx1 };
                    else if (idx2.Length > 0)
                        partitions = new int[][] { idx2 };
                    else
                        partitions = new int[][] { };
                }
            }

            threshold = bestThreshold;
            return bestGain;
        }


        private void checkArgs(double[][] inputs, int[] outputs)
        {
            if (inputs == null)
                throw new ArgumentNullException("inputs");

            if (outputs == null)
                throw new ArgumentNullException("outputs");

            if (inputs.Length != outputs.Length)
                throw new DimensionMismatchException("outputs",
                                                     "The number of input vectors and output labels does not match.");

            if (inputs.Length == 0)
                throw new ArgumentOutOfRangeException("inputs",
                                                      "Training algorithm needs at least one training vector.");

            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i].Length != tree.InputCount)
                {
                    throw new DimensionMismatchException("inputs", "The size of the input vector at index "
                        + i + " does not match the expected number of inputs of the tree." 
                        + " All input vectors for this tree must have length " + tree.InputCount);
                }

                for (int j = 0; j < inputs[i].Length; j++)
                {
                    if (tree.Attributes[j].Nature != DecisionVariableKind.Discrete)
                        continue;

                    int min = (int)tree.Attributes[j].Range.Min;
                    int max = (int)tree.Attributes[j].Range.Max;

                    if (inputs[i][j] < min || inputs[i][j] > max)
                    {
                        throw new ArgumentOutOfRangeException("inputs", "The input vector at position "
                            + i + " contains an invalid entry at column " + j +
                            ". The value must be between the bounds specified by the decision tree " +
                            "attribute variables.");
                    }
                }
            }

            for (int i = 0; i < outputs.Length; i++)
            {
                if (outputs[i] < 0 || outputs[i] >= tree.OutputClasses)
                {
                    throw new ArgumentOutOfRangeException("outputs",
                      "The output label at index " + i + " should be equal to or higher than zero," +
                      "and should be lesser than the number of output classes expected by the tree.");
                }
            }
        }
    }
}
