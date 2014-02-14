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

namespace Accord.MachineLearning.DecisionTrees.Pruning
{
    using System;
    using System.Collections.Generic;
    using Accord.Math;

    /// <summary>
    ///   Error-based pruning.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Lior Rokach, Oded Maimon. The Data Mining and Knowledge Discovery Handbook,
    ///       Chapter 9, Decision Trees. Springer, 2nd ed. 2010, XX, 1285 p. 40 illus. 
    ///       Available at: http://www.ise.bgu.ac.il/faculty/liorr/hbchap9.pdf .</description></item>
    ///   </list>
    /// </para>  
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    /// // Suppose you have the following input and output data
    /// // and would like to learn the relationship between the
    /// // inputs and outputs by using a Decision Tree:
    /// 
    /// double[][] inputs = ...
    /// int[] output = ...
    /// 
    /// // To prune a decision tree, we need to split your data into
    /// // training and pruning groups. Let's say we have 100 samples,
    /// // and would like to reserve 50 samples for training, and 50
    /// // for pruning:
    /// 
    /// // Gather the first half for the training set
    /// var trainingInputs = inputs.Submatrix(0, 49);
    /// var trainingOutput = output.Submatrix(0, 49);
    /// 
    /// // Gather the second hand data for pruning
    /// var pruningInputs = inputs.Submatrix(50, 99);
    /// var pruningOutput = output.Submatrix(50, 99);
    /// 
    /// 
    /// // Create the decision tree
    /// DecisionTree tree = new DecisionTree( ... );
    /// 
    /// // Learn our tree using the training data
    /// C45Learning c45 = new C45Learning(tree);
    /// double error = c45.Run(trainingInputs, trainingOutput);
    /// 
    ///             
    /// // Now we can attempt to prune the tree using the pruning groups
    /// ErrorBasedPruning prune = new ErrorBasedPruning(tree, pruningInputs, pruningOutput);
    /// 
    /// // Gain threshold
    /// prune.Threshold = 0.1;
    /// 
    /// double lastError;
    /// double error = Double.PositiveInfinity;
    /// 
    /// do
    /// {
    ///     // Now we can start pruning the tree as 
    ///     // long as the error doesn't increase
    /// 
    ///     lastError = error;
    ///     error = prune.Run();
    /// 
    /// } while (error &lt; lastError);
    /// </code>
    /// </example>
    /// 
    public class ErrorBasedPruning
    {

        DecisionTree tree;
        double[][] inputs;
        int[] outputs;
        int[] actual;
        double limit = 0.01;

        Dictionary<DecisionNode, List<int>> subsets;

        /// <summary>
        ///   Initializes a new instance of the <see cref="ErrorBasedPruning"/> class.
        /// </summary>
        /// 
        /// <param name="tree">The tree to be pruned.</param>
        /// <param name="inputs">The pruning set inputs.</param>
        /// <param name="outputs">The pruning set outputs.</param>
        /// 
        public ErrorBasedPruning(DecisionTree tree, double[][] inputs, int[] outputs)
        {
            this.tree = tree;
            this.inputs = inputs;
            this.outputs = outputs;
            this.subsets = new Dictionary<DecisionNode, List<int>>();
            this.actual = new int[outputs.Length];

            // Create the cache to store how many times a sample
            // passes through each decision node of the tree.
            //
            createCache(tree.Root);

            // Compute the entire pruning set and track the path
            // taken by each observation during the reasoning.
            //
            trackDecisions(tree.Root, inputs);
        }

        /// <summary>
        ///   Gets or sets the minimum allowed 
        ///   gain threshold to prune the tree.
        /// </summary>
        /// 
        public double Threshold
        {
            get { return limit; }
            set { limit = value; }
        }

        /// <summary>
        ///   Computes one pass of the pruning algorithm.
        /// </summary>
        /// 
        public double Run()
        {
            // Traverse the tree in bottom-up (post-order) order
            foreach (DecisionNode node in tree.Traverse(DecisionTreeTraversal.PostOrder))
            {
                if (node.IsLeaf)
                    continue;

                if (compute(node))
                    break;
            }

            return computeError();
        }



        private double computeError()
        {
            int error = 0;
            for (int i = 0; i < inputs.Length; i++)
            {
                int actual = tree.Compute(inputs[i]);
                int expected = outputs[i];
                if (actual != expected) error++;
            }

            return error / (double)inputs.Length;
        }

        private bool compute(DecisionNode node)
        {
            int[] indices = subsets[node].ToArray();
            int[] subset = outputs.Submatrix(indices);

            int size = indices.Length;
            int mostCommon = Statistics.Tools.Mode(subset);
            DecisionNode maxChild = getMaxChild(node);

            double replace = Double.PositiveInfinity;
            if (maxChild != null)
            {
                replace = computeErrorReplacingSubtrees(node, maxChild);
                replace = upperBound(replace, size);
            }


            double baseline = computeErrorSubtree(indices);
            double prune = computeErrorWithoutSubtree(node, mostCommon);


            baseline = upperBound(baseline, size);
            prune = upperBound(prune, size);


            bool changed = false;

            if (Math.Abs(prune - baseline) < limit ||
                Math.Abs(replace - baseline) < limit)
            {
                if (replace < prune)
                {
                    // We should replace the subtree with its maximum child
                    node.Branches = maxChild.Branches;
                    node.Output = maxChild.Output;
                    foreach (var child in node.Branches)
                        child.Parent = node;
                }
                else
                {
                    // We should prune the subtree
                    node.Branches = null;
                    node.Output = mostCommon;
                }

                changed = true;
                clearCache(node);
                trackDecisions(node, inputs);
            }

            return changed;
        }

        private static double upperBound(double error, int size)
        {
            return error + 1.96 * Math.Sqrt((error * (1 - error)) / size);
        }


        private double computeErrorWithoutSubtree(DecisionNode tree, int mostCommon)
        {
            var branches = tree.Branches;
            var output = tree.Output;

            tree.Branches = null;
            tree.Output = mostCommon;

            double error = computeError();

            tree.Branches = branches;
            tree.Output = output;

            return error;
        }

        private double computeErrorReplacingSubtrees(DecisionNode tree, DecisionNode child)
        {
            var branches = tree.Branches;
            var output = tree.Output;

            tree.Branches = child.Branches;
            tree.Output = child.Output;

            double error = computeError();

            tree.Branches = branches;
            tree.Output = output;

            return error;
        }

        private double computeErrorSubtree(int[] indices)
        {
            int error = 0;
            foreach (int i in indices)
                if (outputs[i] != actual[i]) error++;

            return error / (double)indices.Length;
        }

        private DecisionNode getMaxChild(DecisionNode tree)
        {
            DecisionNode max = null;
            int maxCount = 0;

            foreach (var child in tree.Branches)
            {
                if (child.Branches != null)
                {
                    foreach (var node in child.Branches)
                    {
                        var list = subsets[node];
                        if (list.Count > maxCount)
                        {
                            max = node;
                            maxCount = list.Count;
                        }
                    }
                }
            }

            return max;
        }



        private void createCache(DecisionNode current)
        {
            foreach (var node in tree.Traverse(DecisionTreeTraversal.BreadthFirst, current))
                subsets[node] = new List<int>();
        }

        private void clearCache(DecisionNode current)
        {
            foreach (var node in tree.Traverse(DecisionTreeTraversal.BreadthFirst, current))
                subsets[node].Clear();
        }

        private void trackDecisions(DecisionNode current, double[][] input)
        {
            for (int i = 0; i < input.Length; i++)
                trackDecisions(current, input[i], i);
        }

        private void trackDecisions(DecisionNode root, double[] input, int index)
        {
            DecisionNode current = root;

            while (current != null)
            {
                subsets[current].Add(index);

                if (current.IsLeaf)
                {
                    actual[index] = current.Output.HasValue ? current.Output.Value : -1;
                    return;
                }

                int attribute = current.Branches.AttributeIndex;

                DecisionNode nextNode = null;

                foreach (DecisionNode branch in current.Branches)
                {
                    if (branch.Compute(input[attribute]))
                    {
                        nextNode = branch; break;
                    }
                }

                current = nextNode;
            }

            // Normal execution should not reach here.
            throw new InvalidOperationException("The tree is degenerated.");
        }
    }
}
