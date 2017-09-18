// Accord Machine Learning Library
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
    using Accord.Statistics;
    using Accord.MachineLearning;
    using Accord.Math.Optimization.Losses;
    using Accord.Compat;
    using System.Threading.Tasks;

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
    ///   This example shows how to handle missing values in the training data.</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\DecisionTrees\C45LearningTest.cs" region="doc_missing" />
    /// 
    /// <para>
    ///   The next example shows how to induce a decision tree for a more complicated example, again
    ///   using a <see cref="Accord.Statistics.Filters.Codification">codebook</see> to manage how input 
    ///   variables should be encoded. It also shows how to obtain a compiled version of the decision
    ///   tree for deciding the class labels for new samples with maximum performance.</para>
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\DecisionTrees\C45LearningTest.cs" region="doc_nursery" />
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\DecisionTrees\C45LearningTest.cs" region="doc_nursery_native" />
    /// 
    /// <para>
    ///   The next example shows how to estimate the true performance of a decision tree model using cross-validation:</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\DecisionTrees\DecisionTreeTest.cs" region="doc_cross_validation" />
    ///   
    /// <para>
    ///   The next example shows how to find the best parameters for a decision tree using grid-search cross-validation:</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\GridSearchTest.cs" region="doc_learn_tree_cv" />
    /// </example>
    /// 
    /// <seealso cref="DecisionTree"/>
    /// <seealso cref="ID3Learning"/>
    /// <seealso cref="RandomForestLearning"/>
    ///
    [Serializable]
    public class C45Learning : DecisionTreeLearningBase, ISupervisedLearning<DecisionTree, double[], int>
    {

        private double[][] thresholds;
        private IntRange[] inputRanges;

        private int splitStep = 1;


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
        ///   Creates a new C4.5 learning algorithm.
        /// </summary>
        /// 
        public C45Learning()
        {
        }

        /// <summary>
        ///   Creates a new C4.5 learning algorithm.
        /// </summary>
        /// 
        /// <param name="attributes">The attributes to be processed by the induced tree.</param>
        /// 
        public C45Learning(DecisionVariable[] attributes)
            : base(attributes)
        {
        }

        /// <summary>
        ///   Creates a new C4.5 learning algorithm.
        /// </summary>
        /// 
        /// <param name="tree">The decision tree to be generated.</param>
        /// 
        public C45Learning(DecisionTree tree)
        {
            init(tree);
        }

        private void init(DecisionTree tree)
        {
            // Initial argument checking
            if (tree == null)
                throw new ArgumentNullException("tree");

            this.Model = tree;
            this.AttributeUsageCount = new int[tree.NumberOfInputs];
            this.inputRanges = new IntRange[tree.NumberOfInputs];
            this.Attributes = tree.Attributes;

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
            if (Model == null)
                init(DecisionTreeHelper.Create(x, y, this.Attributes));

            this.run(x, y, weights);
            return Model;
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
        public DecisionTree Learn(int?[][] x, int[] y, double[] weights = null)
        {
            if (Model == null)
                init(DecisionTreeHelper.Create(x, y, this.Attributes));

            this.run(x.Apply((xi, i, j) => xi.HasValue ? (double)xi : Double.NaN), y, weights);
            return Model;
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

            if (Model == null)
                init(DecisionTreeHelper.Create(x, y, this.Attributes));

            this.run(x.ToDouble(), y, weights);
            return Model;
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
            run(inputs, outputs, null);
            return new ZeroOneLoss(outputs)
            {
                Mean = true
            }.Loss(Model.Decide(inputs));
        }

        private void run(double[][] inputs, int[] outputs, double[] weights)
        {
            if (weights == null)
                weights = Vector.Ones(inputs.Length);

            // Initial argument check
            DecisionTreeHelper.CheckArgs(Model, inputs, outputs, weights);

            // Reset the usage of all attributes
            for (int i = 0; i < AttributeUsageCount.Length; i++)
            {
                // a[i] has never been used
                AttributeUsageCount[i] = 0;
            }

            thresholds = new double[Model.Attributes.Count][];

            var candidates = new List<double>(inputs.Length);

            // 0. Create candidate split thresholds for each attribute
            for (int i = 0; i < Model.Attributes.Count; i++)
            {
                if (Model.Attributes[i].Nature == DecisionVariableKind.Continuous)
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
            Model.Root = new DecisionNode(Model);

            // Recursively split the tree nodes
            split(Model.Root, inputs, outputs, weights, height: 0);
        }

        private void split(DecisionNode root, double[][] inputs, int[] outputs, double[] weights, int height)
        {
            // 2. If all examples are for the same class, return the single-node
            //    tree with the output label corresponding to this common class.
            double entropy = Measures.WeightedEntropy(outputs, weights, Model.NumberOfClasses);

            if (entropy == 0)
            {
                if (outputs.Length > 0)
                    root.Output = outputs[0];
                return;
            }

            // 3. If number of predicting attributes is empty, then return the single-node
            //    tree with the output label corresponding to the most common value of
            //    the target attributes in the examples.

            // how many variables have been used less than the limit (if there is a limit)
            int[] candidates = Matrix.Find(AttributeUsageCount, x => Join == 0 ? true : x < Join);

            if (candidates.Length == 0 || (MaxHeight > 0 && height == MaxHeight))
            {
                root.Output = Measures.WeightedMode(outputs, weights);
                return;
            }


            // 4. Otherwise, try to select the attribute which
            //    best explains the data sample subset. If the tree
            //    is part of a random forest, only consider a percentage
            //    of the candidate attributes at each split point

            if (MaxVariables > 0 && candidates.Length > MaxVariables)
                candidates = Vector.Sample(candidates, MaxVariables);

            var scores = new double[candidates.Length];
            var thresholds = new double[candidates.Length];
            var partitions = new List<int>[candidates.Length][];

            if (ParallelOptions.MaxDegreeOfParallelism == 1)
            {
                // For each attribute in the data set
                for (int i = 0; i < scores.Length; i++)
                {
                    scores[i] = computeGainRatio(inputs, outputs, weights, candidates[i],
                        entropy, out partitions[i], out thresholds[i]);
                }
            }
            else
            {
                // For each attribute in the data set
                Parallel.For(0, scores.Length, ParallelOptions, i =>
                {
                    scores[i] = computeGainRatio(inputs, outputs, weights, candidates[i],
                        entropy, out partitions[i], out thresholds[i]);
                });
            }

            // Select the attribute with maximum gain ratio
            int maxGainIndex; scores.Max(out maxGainIndex);
            var maxGainPartition = partitions[maxGainIndex];
            var maxGainAttribute = candidates[maxGainIndex];
            var maxGainRange = inputRanges[maxGainAttribute];
            var maxGainThreshold = thresholds[maxGainIndex];

            // Mark this attribute as already used
            AttributeUsageCount[maxGainAttribute]++;

            double[][] inputSubset;
            int[] outputSubset;
            double[] weightSubset;

            // Now, create next nodes and pass those partitions as their responsibilities. 
            if (Model.Attributes[maxGainAttribute].Nature == DecisionVariableKind.Discrete)
            {
                // This is a discrete nature attribute. We will branch at each
                // possible value for the discrete variable and call recursion.
                var children = new DecisionNode[maxGainPartition.Length];

                // Create a branch for each possible value
                for (int i = 0; i < children.Length; i++)
                {
                    children[i] = new DecisionNode(Model)
                    {
                        Parent = root,
                        Value = i + maxGainRange.Min,
                        Comparison = ComparisonKind.Equal,
                    };

                    inputSubset = inputs.Get(maxGainPartition[i]);
                    outputSubset = outputs.Get(maxGainPartition[i]);
                    weightSubset = weights.Get(maxGainPartition[i]);
                    split(children[i], inputSubset, outputSubset, weightSubset, height + 1); // recursion
                }

                root.Branches.AttributeIndex = maxGainAttribute;
                root.Branches.AddRange(children);
            }
            else if (Model.Attributes[maxGainAttribute].Nature == DecisionVariableKind.Continuous)
            {
                List<int> partitionBelowThreshold = maxGainPartition[0];
                List<int> partitionAboveThreshold = maxGainPartition[1];

                if (partitionBelowThreshold != null && partitionAboveThreshold != null)
                {
                    // This is a continuous nature attribute, and we achieved two partitions
                    // using the partitioning scheme. We will branch on two possible settings:
                    // either the value is greater than a currently detected optimal threshold 
                    // or it is less.

                    DecisionNode[] children =
                    {
                        new DecisionNode(Model)
                        {
                            Parent = root, Value = maxGainThreshold,
                            Comparison = ComparisonKind.LessThanOrEqual
                        },

                        new DecisionNode(Model)
                        {
                            Parent = root, Value = maxGainThreshold,
                            Comparison = ComparisonKind.GreaterThan
                        }
                    };

                    // Create a branch for lower values
                    inputSubset = inputs.Get(partitionBelowThreshold);
                    outputSubset = outputs.Get(partitionBelowThreshold);
                    weightSubset = weights.Get(partitionBelowThreshold);
                    split(children[0], inputSubset, outputSubset, weightSubset, height + 1);

                    // Create a branch for higher values
                    inputSubset = inputs.Get(partitionAboveThreshold);
                    outputSubset = outputs.Get(partitionAboveThreshold);
                    weightSubset = weights.Get(partitionAboveThreshold);
                    split(children[1], inputSubset, outputSubset, weightSubset, height + 1);

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

                    var outputIndices = partitionBelowThreshold ?? partitionAboveThreshold;
                    outputSubset = outputs.Get(outputIndices);
                    root.Output = Measures.Mode(outputSubset);
                }
            }

            AttributeUsageCount[maxGainAttribute]--;
        }


        private double computeGainRatio(double[][] input, int[] output, double[] weight, int attributeIndex,
            double entropy, out List<int>[] partitions, out double threshold)
        {
            List<int> missing;
            double infoGain = computeInfoGain(input, output, weight, attributeIndex, entropy, out partitions, out missing, out threshold);
            double splitInfo = SplitInformation(output.Length, partitions, missing);

            return infoGain == 0 || splitInfo == 0 ? 0 : infoGain / splitInfo;
        }

        private double computeInfoGain(double[][] input, int[] output, double[] weight, int attributeIndex,
            double entropy, out List<int>[] partitions, out List<int> missing, out double threshold)
        {
            threshold = 0;

            if (Model.Attributes[attributeIndex].Nature == DecisionVariableKind.Discrete)
                return entropy - computeInfoDiscrete(input, output, weight, attributeIndex, out partitions, out missing);

            return entropy + computeInfoContinuous(input, output, weight, attributeIndex, out partitions, out missing, out threshold);
        }

        private double computeInfoDiscrete(double[][] input, int[] output, double[] weight,
            int attributeIndex, out List<int>[] partitions, out List<int> missingValues)
        {
            // Compute the information gain obtained by using
            // this current attribute as the next decision node.
            double info = 0;

            IntRange valueRange = inputRanges[attributeIndex];
            int numberOfDistinctValues = valueRange.Length + 1;
            partitions = new List<int>[numberOfDistinctValues];

            missingValues = new List<int>();
            for (int j = 0; j < input.Length; j++)
            {
                if (Double.IsNaN(input[j][attributeIndex]))
                    missingValues.Add(j);
            }

            // For each possible value of the attribute
            for (int i = 0; i < numberOfDistinctValues; i++)
            {
                int value = valueRange.Min + i;

                // Partition the remaining data set
                // according to the attribute values
                var indicesInPartition = new List<int>();

                double weightTotalSum = 0;
                double weightSubsetSum = 0;

                for (int j = 0; j < input.Length; j++)
                {
                    double x = input[j][attributeIndex];
                    if (!Double.IsNaN(x) && x == value)
                    {
                        indicesInPartition.Add(j);
                        weightSubsetSum += weight[j];
                    }
                    weightTotalSum += weight[j];
                }

                // For each of the instances under responsibility
                // of this node, check which have the same value
                int[] outputSubset = output.Get(indicesInPartition);
                double[] weightSubset = weight.Get(indicesInPartition);

                // Check the entropy gain originating from this partitioning
                double e = Measures.WeightedEntropy(outputSubset, weightSubset, Model.NumberOfClasses);
                info += (weightSubsetSum / weightTotalSum) * e;

                partitions[i] = indicesInPartition;
            }

            return info;
        }

        private double computeInfoContinuous(double[][] input, int[] output, double[] weight,
            int attributeIndex, out List<int>[] partitions, out List<int> missingValues, out double threshold)
        {
            // Compute the information gain obtained by using
            // this current attribute as the next decision node.
            double[] t = thresholds[attributeIndex];
            double bestGain = Double.NegativeInfinity;

            missingValues = new List<int>();
            for (int j = 0; j < input.Length; j++)
            {
                if (Double.IsNaN(input[j][attributeIndex]))
                    missingValues.Add(j);
            }

            // If there are no possible thresholds that we can use
            // to split the data (i.e. if all values are the same)
            if (t.Length == 0)
            {
                // Then they all belong to the same partition
                partitions = new[] { new List<int>(Vector.Range(input.Length)), null };
                threshold = Double.NegativeInfinity;
                return bestGain;
            }

            partitions = null;

            double bestThreshold = t[0];

            var indicesBelowThreshold = new List<int>(input.Length);
            var indicesAboveThreshold = new List<int>(input.Length);

            var output1 = new List<int>(input.Length);
            var output2 = new List<int>(input.Length);

            var weights1 = new List<double>(input.Length);
            var weights2 = new List<double>(input.Length);

            // For each possible splitting point of the attribute
            for (int i = 0; i < t.Length; i += splitStep)
            {
                // Partition the remaining data set
                // according to the threshold value
                double value = t[i];

                for (int j = 0; j < input.Length; j++)
                {
                    double x = input[j][attributeIndex];

                    if (Double.IsNaN(x))
                    {
                        continue;
                    }
                    else if (x <= value)
                    {
                        indicesBelowThreshold.Add(j);
                        output1.Add(output[j]);
                        weights1.Add(weight[j]);
                    }
                    else if (x > value)
                    {
                        indicesAboveThreshold.Add(j);
                        output2.Add(output[j]);
                        weights2.Add(weight[j]);
                    }
                }

                double weightSum = weight.Sum();
                double p1 = weights1.Sum() / weightSum;
                double p2 = weights2.Sum() / weightSum;

                double splitGain =
                    -p1 * Measures.WeightedEntropy(output1, weights1, Model.NumberOfClasses) +
                    -p2 * Measures.WeightedEntropy(output2, weights2, Model.NumberOfClasses);

                if (splitGain > bestGain)
                {
                    bestThreshold = value;
                    bestGain = splitGain;

                    if (indicesBelowThreshold.Count == 0)
                        indicesBelowThreshold = null;
                    if (indicesAboveThreshold.Count == 0)
                        indicesAboveThreshold = null;
                    partitions = new[] { indicesBelowThreshold, indicesAboveThreshold };

                    indicesBelowThreshold = new List<int>(input.Length);
                    indicesAboveThreshold = new List<int>(input.Length);
                }
                else
                {
                    indicesBelowThreshold.Clear();
                    indicesAboveThreshold.Clear();
                }

                output1.Clear();
                output2.Clear();
                weights1.Clear();
                weights2.Clear();
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
            }.Loss(Model.Decide(inputs));
        }

    }
}
