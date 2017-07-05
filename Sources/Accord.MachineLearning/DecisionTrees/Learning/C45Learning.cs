﻿// Accord Machine Learning Library
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

namespace Accord.MachineLearning.DecisionTrees.Learning
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Accord.Math;
    using AForge;
    using Parallel = System.Threading.Tasks.Parallel;
    using Accord.Statistics;
    using System.Threading.Tasks;
    using Accord.MachineLearning;
    using Accord.Math.Optimization.Losses;

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
    /// <example>
    /// <para>
    ///   This example shows the simplest way to induce a decision tree with continuous variables.</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\DecisionTrees\C45LearningTest.cs" region="doc_simplest" />
    /// 
    /// <para>
    ///   This is the same example as above, but the decision variables are specified manually.</para>
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\DecisionTrees\C45LearningTest.cs" region="doc_iris" />
    /// 
    /// <para>
    ///   The next example shows how to induce a decision tree for a more complicated example, again
    ///   using a <see cref="Accord.Statistics.Filters.Codification">codebook</see> to manage how input 
    ///   variables should be encoded. It also shows how to obtain a compiled version of the decision
    ///   tree for deciding the class labels for new samples with maximum performance.</para>
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\DecisionTrees\C45LearningTest.cs" region="doc_nursery" />
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\DecisionTrees\C45LearningTest.cs" region="doc_nursery_native" />
    /// </example>
    /// 
    /// <seealso cref="DecisionTree"/>
    /// <seealso cref="ID3Learning"/>
    /// <seealso cref="RandomForestLearning"/>
    ///
    [Serializable]
    public class C45Learning : ParallelLearningBase,
        ISupervisedLearning<DecisionTree, double[], int>
    {

        private DecisionTree tree;

        private int maxHeight;
        private int splitStep;

        private double[][] thresholds;
        private IntRange[] inputRanges;

        private int inputVariables;
        private int outputClasses;

        private int join = 1;
        private int[] attributeUsageCount;
        private IList<DecisionVariable> attributes;

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
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("value",
                        "The height must be greater than zero.");
                }

                maxHeight = value;
            }
        }

        /// <summary>
        ///   Gets or sets the collection of attributes to 
        ///   be processed by the induced decision tree.
        /// </summary>
        /// 
        public IList<DecisionVariable> Attributes
        {
            get { return attributes; }
            set { attributes = value; }
        }

        /// <summary>
        ///   Gets or sets the maximum number of variables that
        ///   can enter the tree. A value of zero indicates there
        ///   is no limit. Default is 0 (there is no limit on the
        ///   number of variables).
        /// </summary>
        /// 
        public int MaxVariables { get; set; }

        /// <summary>
        ///   Gets or sets the step at which the samples will
        ///   be divided when dividing continuous columns in
        ///   binary classes. Default is 1.
        /// </summary>
        /// 
        public int SplitStep
        {
            get { return splitStep; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("value",
                        "The split step must be greater than zero.");
                }

                splitStep = value;
            }
        }

        /// <summary>
        ///   Gets or sets how many times one single variable can be
        ///   integrated into the decision process. In the original
        ///   ID3 algorithm, a variable can join only one time per
        ///   decision path (path from the root to a leaf).
        /// </summary>
        /// 
        public int Join
        {
            get { return join; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("value",
                        "The number of times must be greater than zero.");
                }

                join = value;
            }
        }

        /// <summary>
        ///   Gets or sets the decision trees being learned.
        /// </summary>
        /// 
        public DecisionTree Model
        {
            get { return tree; }
            set { tree = value; }
        }

        /// <summary>
        ///   Creates a new C4.5 learning algorithm.
        /// </summary>
        /// 
        public C45Learning()
        {
            this.splitStep = 1;
            this.ParallelOptions = new ParallelOptions();
        }

        /// <summary>
        ///   Creates a new C4.5 learning algorithm.
        /// </summary>
        /// 
        /// <param name="tree">The decision tree to be generated.</param>
        /// 
        public C45Learning(DecisionTree tree)
            : this()
        {
            init(tree);
        }

        /// <summary>
        ///   Creates a new C4.5 learning algorithm.
        /// </summary>
        /// 
        /// <param name="attributes">The attributes to be processed by the induced tree.</param>
        //
        public C45Learning(DecisionVariable[] attributes)
            : this()
        {
            this.attributes = new List<DecisionVariable>(attributes);
        }

        private void init(DecisionTree tree)
        {
            // Initial argument checking
            if (tree == null)
                throw new ArgumentNullException("tree");

            this.tree = tree;
            this.inputVariables = tree.NumberOfInputs;
            this.outputClasses = tree.NumberOfOutputs;
            this.attributeUsageCount = new int[inputVariables];
            this.inputRanges = new IntRange[inputVariables];
            this.maxHeight = inputVariables;
            this.attributes = tree.Attributes;

            for (int i = 0; i < inputRanges.Length; i++)
                inputRanges[i] = tree.Attributes[i].Range.ToIntRange(false);
        }

        /// <summary>
        ///   Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// 
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weights">The weight of importance for each input-output pair (if supported by the learning algorithm).</param>
        /// 
        /// <returns>A model that has learned how to produce <paramref name="y"/> given <paramref name="x"/>.</returns>
        /// 
        public DecisionTree Learn(double[][] x, int[] y, double[] weights = null)
        {
            if (weights != null)
                throw new ArgumentException(Accord.Properties.Resources.NotSupportedWeights, "weights");

            if (tree == null)
            {
                if (this.attributes == null)
                    this.attributes = DecisionVariable.FromData(x);
                int classes = y.Max() + 1;
                init(new DecisionTree(this.attributes, classes));
            }

            this.run(x, y);
            return tree;
        }

        /// <summary>
        ///   Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// 
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weights">The weight of importance for each input-output pair (if supported by the learning algorithm).</param>
        /// 
        /// <returns>A model that has learned how to produce <paramref name="y"/> given <paramref name="x"/>.</returns>
        /// 
        public DecisionTree Learn(int[][] x, int[] y, double[] weights = null)
        {
            if (weights != null)
                throw new ArgumentException(Accord.Properties.Resources.NotSupportedWeights, "weights");

            if (tree == null)
            {
                var variables = DecisionVariable.FromData(x);
                int classes = y.Max() + 1;
                init(new DecisionTree(variables, classes));
            }

            this.run(x.ToDouble(), y);
            return tree;
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
        [Obsolete("Please use Learn(x, y) instead.")]
        public double Run(double[][] inputs, int[] outputs)
        {
            run(inputs, outputs);
            return new ZeroOneLoss(outputs)
            {
                Mean = true
            }.Loss(tree.Decide(inputs));
        }

        private void run(double[][] inputs, int[] outputs)
        {
            // Initial argument check
            DecisionTreeHelper.CheckArgs(tree, inputs, outputs);

            // Reset the usage of all attributes
            for (int i = 0; i < attributeUsageCount.Length; i++)
            {
                // a[i] has never been used
                attributeUsageCount[i] = 0;
            }

            thresholds = new double[tree.Attributes.Count][];

            var candidates = new List<double>(inputs.Length);

            // 0. Create candidate split thresholds for each attribute
            for (int i = 0; i < tree.Attributes.Count; i++)
            {
                if (tree.Attributes[i].Nature == DecisionVariableKind.Continuous)
                {
                    double[] v = inputs.GetColumn(i);
                    int[] o = (int[])outputs.Clone();

                    IGrouping<double, int>[] sortedValueToClassesMapping =
                        v.
                            Select((value, index) => new KeyValuePair<double, int>(value, o[index])).
                            GroupBy(keyValuePair => keyValuePair.Key, keyValuePair => keyValuePair.Value).
                            OrderBy(keyValuePair => keyValuePair.Key).
                            ToArray();

                    for (int j = 0; j < sortedValueToClassesMapping.Length - 1; j++)
                    {
                        // Following the results by Fayyad and Irani (1992) (see footnote on Quinlan (1996)):
                        // "If all cases of adjacent values V[i] and V[i+1] belong to the same class, 
                        // a threshold between them cannot lead to a partition that has the maximum value of
                        // the criterion." i.e no reason the add the threshold as a candidate

                        IGrouping<double, int> currentValueToClasses = sortedValueToClassesMapping[j];
                        IGrouping<double, int> nextValueToClasses = sortedValueToClassesMapping[j + 1];
                        double a = nextValueToClasses.Key;
                        double b = currentValueToClasses.Key;
                        if (a - b > Constants.DoubleEpsilon && currentValueToClasses.Union(nextValueToClasses).Count() > 1)
                            candidates.Add((currentValueToClasses.Key + nextValueToClasses.Key) / 2.0);
                    }

                    thresholds[i] = candidates.ToArray();
                    candidates.Clear();
                }
            }


            // 1. Create a root node for the tree
            tree.Root = new DecisionNode(tree);

            // Recursively split the tree nodes
            split(tree.Root, inputs, outputs, 0);
        }

        private void split(DecisionNode root, double[][] input, int[] output, int height)
        {
            // 2. If all examples are for the same class, return the single-node
            //    tree with the output label corresponding to this common class.
            double entropy = Measures.Entropy(output, outputClasses);

            if (entropy == 0)
            {
                if (output.Length > 0)
                    root.Output = output[0];
                return;
            }

            // 3. If number of predicting attributes is empty, then return the single-node
            //    tree with the output label corresponding to the most common value of
            //    the target attributes in the examples.

            // how many variables have been used less than the limit
            int[] candidates = Matrix.Find(attributeUsageCount, x => x < join);

            if (candidates.Length == 0 || (maxHeight > 0 && height == maxHeight))
            {
                root.Output = Measures.Mode(output);
                return;
            }


            // 4. Otherwise, try to select the attribute which
            //    best explains the data sample subset. If the tree
            //    is part of a random forest, only consider a percentage
            //    of the candidate attributes at each split point

            if (MaxVariables > 0)
                candidates = Vector.Sample(candidates, MaxVariables);

            var scores = new double[candidates.Length];
            var thresholds = new double[candidates.Length];
            var partitions = new int[candidates.Length][][];

            // For each attribute in the data set
            Parallel.For(0, scores.Length, ParallelOptions, i =>
            {
                scores[i] = computeGainRatio(input, output, candidates[i],
                    entropy, out partitions[i], out thresholds[i]);
            });

            // Select the attribute with maximum gain ratio
            int maxGainIndex; scores.Max(out maxGainIndex);
            var maxGainPartition = partitions[maxGainIndex];
            var maxGainAttribute = candidates[maxGainIndex];
            var maxGainRange = inputRanges[maxGainAttribute];
            var maxGainThreshold = thresholds[maxGainIndex];

            // Mark this attribute as already used
            attributeUsageCount[maxGainAttribute]++;

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

                    inputSubset = input.Get(maxGainPartition[i]);
                    outputSubset = output.Get(maxGainPartition[i]);
                    split(children[i], inputSubset, outputSubset, height + 1); // recursion
                }

                root.Branches.AttributeIndex = maxGainAttribute;
                root.Branches.AddRange(children);
            }

            else if (maxGainPartition.Length > 1)
            {
                // This is a continuous nature attribute, and we achieved two partitions
                // using the partitioning scheme. We will branch on two possible settings:
                // either the value is greater than a currently detected optimal threshold 
                // or it is less.

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
                inputSubset = input.Get(maxGainPartition[0]);
                outputSubset = output.Get(maxGainPartition[0]);
                split(children[0], inputSubset, outputSubset, height + 1);

                // Create a branch for higher values
                inputSubset = input.Get(maxGainPartition[1]);
                outputSubset = output.Get(maxGainPartition[1]);
                split(children[1], inputSubset, outputSubset, height + 1);

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

                outputSubset = output.Get(maxGainPartition[0]);
                root.Output = Measures.Mode(outputSubset);
            }

            attributeUsageCount[maxGainAttribute]--;
        }


        private double computeGainRatio(double[][] input, int[] output, int attributeIndex,
            double entropy, out int[][] partitions, out double threshold)
        {
            double infoGain = computeInfoGain(input, output, attributeIndex, entropy, out partitions, out threshold);
            double splitInfo = Statistics.Tools.SplitInformation(output.Length, partitions);

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
                int[] outputSubset = output.Get(partitions[i]);

                // Check the entropy gain originating from this partitioning
                double e = Measures.Entropy(outputSubset, outputClasses);

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

            // If there are no possible thresholds that we can use
            // to split the data (i.e. if all values are the same)
            if (t.Length == 0)
            {
                // Then they all belong to the same partition
                partitions = new int[][] { Vector.Range(input.Length) };
                threshold = Double.NegativeInfinity;
                return bestGain;
            }

            double bestThreshold = t[0];
            partitions = null;

            var idx1 = new List<int>(input.Length);
            var idx2 = new List<int>(input.Length);

            var output1 = new List<int>(input.Length);
            var output2 = new List<int>(input.Length);

            double[] values = new double[input.Length];
            for (int i = 0; i < values.Length; i++)
                values[i] = input[i][attributeIndex];

            // For each possible splitting point of the attribute
            for (int i = 0; i < t.Length; i += splitStep)
            {
                // Partition the remaining data set
                // according to the threshold value
                double value = t[i];

                idx1.Clear();
                idx2.Clear();

                output1.Clear();
                output2.Clear();

                for (int j = 0; j < values.Length; j++)
                {
                    double x = values[j];

                    if (x <= value)
                    {
                        idx1.Add(j);
                        output1.Add(output[j]);
                    }
                    else if (x > value)
                    {
                        idx2.Add(j);
                        output2.Add(output[j]);
                    }
                }

                double p1 = (double)output1.Count / output.Length;
                double p2 = (double)output2.Count / output.Length;

                double splitGain =
                    -p1 * Measures.Entropy(output1, outputClasses) +
                    -p2 * Measures.Entropy(output2, outputClasses);

                if (splitGain > bestGain)
                {
                    bestThreshold = value;
                    bestGain = splitGain;

                    if (idx1.Count > 0 && idx2.Count > 0)
                        partitions = new int[][] { idx1.ToArray(), idx2.ToArray() };
                    else if (idx1.Count > 0)
                        partitions = new int[][] { idx1.ToArray() };
                    else if (idx2.Count > 0)
                        partitions = new int[][] { idx2.ToArray() };
                    else
                        partitions = new int[][] { };
                }
            }

            threshold = bestThreshold;
            return bestGain;
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
        [Obsolete("Please use the ZeroOneLoss class instead.")]
        public double ComputeError(double[][] inputs, int[] outputs)
        {
            return new ZeroOneLoss(outputs)
            {
                Mean = true
            }.Loss(tree.Decide(inputs));
        }

    }
}
