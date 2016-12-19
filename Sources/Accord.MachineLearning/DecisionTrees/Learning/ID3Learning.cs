﻿// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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
    ///   In this example, we will be using the famous Play Tennis example by Tom Mitchell (1998).
    ///   In Mitchell's example, one would like to infer if a person would play tennis or not
    ///   based solely on four input variables. Those variables are all categorical, meaning that
    ///   there is no order between the possible values for the variable (i.e. there is no order
    ///   relationship between Sunny and Rain, one is not bigger nor smaller than the other, but are 
    ///   just distinct). Moreover, the rows, or instances presented above represent days on which the
    ///   behavior of the person has been registered and annotated, pretty much building our set of 
    ///   observation instances for learning:</para>
    /// 
    /// <code>
    ///   DataTable data = new DataTable("Mitchell's Tennis Example");
    ///   
    ///   data.Columns.Add("Day", "Outlook", "Temperature", "Humidity", "Wind", "PlayTennis");
    ///   
    ///   data.Rows.Add(   "D1",   "Sunny",      "Hot",       "High",   "Weak",    "No"  );
    ///   data.Rows.Add(   "D2",   "Sunny",      "Hot",       "High",  "Strong",   "No"  ); 
    ///   data.Rows.Add(   "D3",  "Overcast",    "Hot",       "High",   "Weak",    "Yes" );
    ///   data.Rows.Add(   "D4",   "Rain",       "Mild",      "High",   "Weak",    "Yes" ); 
    ///   data.Rows.Add(   "D5",   "Rain",       "Cool",     "Normal",  "Weak",    "Yes" ); 
    ///   data.Rows.Add(   "D6",   "Rain",       "Cool",     "Normal", "Strong",   "No"  ); 
    ///   data.Rows.Add(   "D7",  "Overcast",    "Cool",     "Normal", "Strong",   "Yes" );
    ///   data.Rows.Add(   "D8",   "Sunny",      "Mild",      "High",   "Weak",    "No"  );  
    ///   data.Rows.Add(   "D9",   "Sunny",      "Cool",     "Normal",  "Weak",    "Yes" ); 
    ///   data.Rows.Add(   "D10", "Rain",        "Mild",     "Normal",  "Weak",    "Yes" ); 
    ///   data.Rows.Add(   "D11",  "Sunny",      "Mild",     "Normal", "Strong",   "Yes" );
    ///   data.Rows.Add(   "D12", "Overcast",    "Mild",      "High",  "Strong",   "Yes" ); 
    ///   data.Rows.Add(   "D13", "Overcast",    "Hot",      "Normal",  "Weak",    "Yes" ); 
    ///   data.Rows.Add(   "D14",  "Rain",       "Mild",      "High",  "Strong",   "No"  );
    /// </code>
    /// 
    /// <para>
    ///   In order to try to learn a decision tree, we will first convert this problem to a more simpler
    ///   representation. Since all variables are categories, it does not matter if they are represented
    ///   as strings, or numbers, since both are just symbols for the event they represent. Since numbers
    ///   are more easily representable than text string, we will convert the problem to use a discrete 
    ///   alphabet through the use of a <see cref="Accord.Statistics.Filters.Codification">codebook</see>.</para>
    /// 
    /// <para>
    ///   A codebook effectively transforms any distinct possible value for a variable into an integer 
    ///   symbol. For example, “Sunny” could as well be represented by the integer label 0, “Overcast” 
    ///   by “1”, Rain by “2”, and the same goes by for the other variables. So:</para>
    /// 
    /// <code>
    ///   // Create a new codification codebook to 
    ///   // convert strings into integer symbols
    ///   Codification codebook = new Codification(data,
    ///     "Outlook", "Temperature", "Humidity", "Wind", "PlayTennis");
    ///   
    ///   // Translate our training data into integer symbols using our codebook:
    ///   DataTable symbols = codebook.Apply(data); 
    ///   int[][] inputs  = symbols.ToArray&lt;int>("Outlook", "Temperature", "Humidity", "Wind"); 
    ///   int[]   outputs = symbols.ToArray&lt;int>("PlayTennis");
    /// </code>
    /// 
    /// <para>
    ///   Now that we already have our learning input/ouput pairs, we should specify our
    ///   decision tree. We will be trying to build a tree to predict the last column, entitled
    ///   “PlayTennis”. For this, we will be using the “Outlook”, “Temperature”, “Humidity” and
    ///   “Wind” as predictors (variables which will we will use for our decision). Since those
    ///   are categorical, we must specify, at the moment of creation of our tree, the
    ///   characteristics of each of those variables. So:
    /// </para>
    /// 
    /// <code>
    ///   // Gather information about decision variables
    ///   DecisionVariable[] attributes =
    ///   {
    ///     new DecisionVariable("Outlook",     3), // 3 possible values (Sunny, overcast, rain)
    ///     new DecisionVariable("Temperature", 3), // 3 possible values (Hot, mild, cool)  
    ///     new DecisionVariable("Humidity",    2), // 2 possible values (High, normal)    
    ///     new DecisionVariable("Wind",        2)  // 2 possible values (Weak, strong) 
    ///   };
    ///   
    ///   int classCount = 2; // 2 possible output values for playing tennis: yes or no
    ///
    ///   //Create the decision tree using the attributes and classes
    ///   DecisionTree tree = new DecisionTree(attributes, classCount); 
    /// </code>
    /// 
    /// <para>Now we have created our decision tree. Unfortunately, it is not really very useful,
    /// since we haven't taught it the problem we are trying to predict. So now we must instantiate
    /// a learning algorithm to make it useful. For this task, in which we have only categorical 
    /// variables, the simplest choice is to use the ID3 algorithm by Quinlan. Let’s do it:</para>
    /// 
    /// <code>
    ///   // Create a new instance of the ID3 algorithm
    ///   ID3Learning id3learning = new ID3Learning(tree);
    ///
    ///   // Learn the training instances!
    ///   id3learning.Run(inputs, outputs); 
    /// </code>
    /// 
    /// <para>The tree can now be queried for new examples through its <see cref="DecisionTree.Compute(double[])"/>
    /// method. For example, we can use: </para>
    /// 
    /// <code>
    ///   string answer = codebook.Translate("PlayTennis",
    ///     tree.Compute(codebook.Translate("Sunny", "Hot", "High", "Strong")));
    /// </code>
    /// 
    /// <para>In the above example, answer will be "No".</para>
    /// 
    /// </example>
    /// 
    ///
    /// <see cref="C45Learning"/>
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
