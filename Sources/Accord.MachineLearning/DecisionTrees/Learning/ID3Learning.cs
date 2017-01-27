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
    using Accord.Math;
    using System.Threading.Tasks;
    using Accord.Statistics;
    using Accord.Math.Optimization.Losses;
    using Accord.MachineLearning;
    using System.Linq;
    using System.Collections.Generic;

    /// <summary>
    ///   ID3 (Iterative Dichotomizer 3) learning algorithm
    ///   for <see cref="DecisionTree">Decision Trees</see>.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Quinlan, J. R 1986. Induction of Decision Trees.
    ///       Mach. Learn. 1, 1 (Mar. 1986), 81-106.</description></item>
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
    ///   This example shows the simplest way to induce a decision tree with discrete variables.</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\DecisionTrees\ID3LearningTest.cs" region="doc_learn_simplest" />
    ///   
    ///<para>
    ///   This example shows a common textbook example, and how to induce a decision tree using a 
    ///   <see cref="Codebook"/> to convert string (text) variables into discrete symbols.</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\DecisionTrees\ID3LearningTest.cs" region="doc_learn_mitchell" />
    /// </example>
    ///
    /// <see cref="DecisionTree"/> 
    /// <see cref="C45Learning"/>
    /// <see cref="RandomForestLearning"/>
    /// 
    [Serializable]
    public class ID3Learning : ParallelLearningBase, ISupervisedLearning<DecisionTree, int[], int>
    {

        private DecisionTree tree;

        private int maxHeight;
        private IntRange[] inputRanges;
        private int outputClasses;

        private int join = 1;

        private int[] attributeUsageCount;
        private IList<DecisionVariable> attributes;


        /// <summary>
        ///   Gets or sets the maximum allowed height when
        ///   learning a tree. If set to zero, no limit will
        ///   be applied. Default is zero.
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
        ///   Gets or sets the decision trees being learned.
        /// </summary>
        /// 
        public DecisionTree Model
        {
            get { return tree; }
            set { tree = value; }
        }

        /// <summary>
        ///   Gets or sets whether all nodes are obligated to provide 
        ///   a true decision value. If set to false, some leaf nodes
        ///   may contain <c>null</c>. Default is false.
        /// </summary>
        /// 
        public bool Rejection { get; set; }


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
        ///   Creates a new ID3 learning algorithm.
        /// </summary>
        /// 
        public ID3Learning()
        {
            this.Rejection = true;
        }

        /// <summary>
        ///   Creates a new ID3 learning algorithm.
        /// </summary>
        /// 
        /// <param name="tree">The decision tree to be generated.</param>
        /// 
        public ID3Learning(DecisionTree tree)
            : this()
        {
            init(tree);
        }

        /// <summary>
        ///   Creates a new ID3 learning algorithm.
        /// </summary>
        /// 
        /// <param name="attributes">The attributes to be processed by the induced tree.</param>
        /// 
        public ID3Learning(DecisionVariable[] attributes)
            : this()
        {
            this.attributes = new List<DecisionVariable>(attributes);
        }

        private void init(DecisionTree tree)
        {
            if (tree == null)
                throw new ArgumentNullException("tree");

            this.tree = tree;
            this.inputRanges = new IntRange[tree.NumberOfInputs];
            this.outputClasses = tree.NumberOfOutputs;
            this.attributeUsageCount = new int[tree.NumberOfInputs];
            this.attributes = tree.Attributes;

            for (int i = 0; i < tree.Attributes.Count; i++)
            {
                if (tree.Attributes[i].Nature != DecisionVariableKind.Discrete)
                    throw new ArgumentException("The ID3 learning algorithm can only handle discrete inputs.");
            }

            for (int i = 0; i < inputRanges.Length; i++)
                inputRanges[i] = tree.Attributes[i].Range.ToIntRange(provideInnerRange: false);
        }

        /// <summary>
        ///   Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// 
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weights">The weight of importance for each input-output pair.</param>
        /// 
        /// <returns>A model that has learned how to produce <paramref name="y"/> given <paramref name="x"/>.</returns>
        /// 
        public DecisionTree Learn(int[][] x, int[] y, double[] weights = null)
        {
            if (tree == null)
            {
                if (this.attributes == null)
                    this.attributes = DecisionVariable.FromData(x);
                int classes = y.Max() + 1;
                init(new DecisionTree(this.attributes, classes));
            }

            run(x, y);

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
        public double Run(int[][] inputs, int[] outputs)
        {
            run(inputs, outputs);

            // Return the classification error
            return new ZeroOneLoss(outputs)
            {
                Mean = true
            }.Loss(tree.Decide(inputs));
        }

        private void run(int[][] inputs, int[] outputs)
        {
            // Initial argument check
            DecisionTreeHelper.CheckArgs(tree, inputs, outputs);

            // Reset the usage of all attributes
            for (int i = 0; i < attributeUsageCount.Length; i++)
            {
                // a[i] has never been used
                attributeUsageCount[i] = 0;
            }

            // 1. Create a root node for the tree
            this.tree.Root = new DecisionNode(tree);

            // Recursively split the tree nodes
            split(tree.Root, inputs, outputs, 0);
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
        public double ComputeError(int[][] inputs, int[] outputs)
        {
            return new ZeroOneLoss(outputs)
            {
                Mean = true
            }.Loss(tree.Decide(inputs));
        }

        private void split(DecisionNode root, int[][] input, int[] output, int height)
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
            //

            // how many variables have been used less than the limit
            int candidateCount = attributeUsageCount.Count(x => x < join);

            if (candidateCount == 0 || (maxHeight > 0 && height == maxHeight))
            {
                root.Output = Measures.Mode(output);
                return;
            }


            // 4. Otherwise, try to select the attribute which
            //    best explains the data sample subset.

            double[] scores = new double[candidateCount];
            int[][][] partitions = new int[candidateCount][][];
            int[][][] outputSubs = new int[candidateCount][][];

            // Retrieve candidate attribute indices
            int[] candidates = new int[candidateCount];
            for (int i = 0, k = 0; i < attributeUsageCount.Length; i++)
            {
                if (attributeUsageCount[i] < join)
                    candidates[k++] = i;
            }


            // For each attribute in the data set
            Parallel.For(0, scores.Length, ParallelOptions, i =>
            {
                scores[i] = computeGainRatio(input, output, candidates[i],
                    entropy, out partitions[i], out outputSubs[i]);
            });

            // Select the attribute with maximum gain ratio
            int maxGainIndex; scores.Max(out maxGainIndex);
            var maxGainPartition = partitions[maxGainIndex];
            var maxGainOutputs = outputSubs[maxGainIndex];
            var maxGainAttribute = candidates[maxGainIndex];
            var maxGainRange = inputRanges[maxGainAttribute];

            attributeUsageCount[maxGainAttribute]++;

            // Now, create next nodes and pass those partitions as their responsibilities.
            DecisionNode[] children = new DecisionNode[maxGainPartition.Length];

            for (int i = 0; i < children.Length; i++)
            {
                children[i] = new DecisionNode(tree)
                {
                    Parent = root,
                    Comparison = ComparisonKind.Equal,
                    Value = i + maxGainRange.Min
                };


                int[][] inputSubset = input.Get(maxGainPartition[i]);
                int[] outputSubset = maxGainOutputs[i];

                split(children[i], inputSubset, outputSubset, height + 1); // recursion

                if (children[i].IsLeaf)
                {
                    // If the resulting node is a leaf, and it has not
                    // been assigned a value because there were no available
                    // output samples in this category, we will be assigning
                    // the most common label for the current node to it.
                    if (!Rejection && !children[i].Output.HasValue)
                        children[i].Output = Measures.Mode(output);
                }
            }


            attributeUsageCount[maxGainAttribute]--;

            root.Branches.AttributeIndex = maxGainAttribute;
            root.Branches.AddRange(children);
        }


        private double computeInfo(int[][] input, int[] output, int attributeIndex,
            out int[][] partitions, out int[][] outputSubset)
        {
            // Compute the information gain obtained by using
            // this current attribute as the next decision node.
            double info = 0;

            IntRange valueRange = inputRanges[attributeIndex];

            partitions = new int[valueRange.Length + 1][];
            outputSubset = new int[valueRange.Length + 1][];

            // For each possible value of the attribute
            for (int i = 0; i < partitions.Length; i++)
            {
                int value = valueRange.Min + i;

                // Partition the remaining data set
                // according to the attribute values
                partitions[i] = input.Find(x => x[attributeIndex] == value);

                // For each of the instances under responsibility
                // of this node, check which have the same value
                outputSubset[i] = output.Get(partitions[i]);

                // Check the entropy gain originating from this partitioning
                double e = Measures.Entropy(outputSubset[i], outputClasses);

                info += (outputSubset[i].Length / (double)output.Length) * e;
            }

            return info;
        }

        private double computeInfoGain(int[][] input, int[] output, int attributeIndex,
            double entropy, out int[][] partitions, out int[][] outputSubset)
        {
            return entropy - computeInfo(input, output, attributeIndex, out partitions, out outputSubset);
        }

        private double computeGainRatio(int[][] input, int[] output, int attributeIndex,
            double entropy, out int[][] partitions, out int[][] outputSubset)
        {
            double infoGain = computeInfoGain(input, output, attributeIndex,
                entropy, out partitions, out outputSubset);

            double splitInfo = Statistics.Tools.SplitInformation(output.Length, partitions);

            return infoGain == 0 ? 0 : infoGain / splitInfo;
        }

    }
}
