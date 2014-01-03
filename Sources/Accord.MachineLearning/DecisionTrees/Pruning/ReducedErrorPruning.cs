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
    using System.Linq;
    using System.Text;
    using Accord.Math;
    using Accord.Statistics;
    using Accord.MachineLearning.Structures;

    /// <summary>
    ///   Reduced error pruning.
    /// </summary>
    /// 
    public class ReducedErrorPruning
    {

        DecisionTree tree;

        double[][] inputs;
        int[] outputs;
        int[] actual;

        Dictionary<DecisionNode, NodeInfo> info;

        private class NodeInfo
        {
            public List<int> subset;
            public double error;
            public double gain;

            public NodeInfo()
            {
                subset = new List<int>();
            }
        }
        

        /// <summary>
        ///   Initializes a new instance of the <see cref="ReducedErrorPruning"/> class.
        /// </summary>
        /// 
        /// <param name="tree">The tree to be pruned.</param>
        /// <param name="inputs">The pruning set inputs.</param>
        /// <param name="outputs">The pruning set outputs.</param>
        /// 
        public ReducedErrorPruning(DecisionTree tree, double[][] inputs, int[] outputs)
        {
            this.tree = tree;
            this.inputs = inputs;
            this.outputs = outputs;
            this.info = new Dictionary<DecisionNode, NodeInfo>();
            this.actual = new int[outputs.Length];

            foreach (var node in tree)
                info[node] = new NodeInfo();

            for (int i = 0; i < inputs.Length; i++)
                trackDecisions(tree.Root, inputs[i], i);
        }


        /// <summary>
        ///   Computes one pass of the pruning algorithm.
        /// </summary>
        /// 
        public double Run()
        {
            // Compute misclassifications at each node
            foreach (var node in tree)
                info[node].error = computeError(node);

            // Compute the gain at each node
            foreach (var node in tree)
                info[node].gain = computeGain(node);

            // Get maximum violating node
            double maxGain = Double.NegativeInfinity;
            DecisionNode maxNode = null;
            foreach (var node in tree)
            {
                double gain = info[node].gain;

                if (gain > maxGain)
                {
                    maxGain = gain;
                    maxNode = node;
                }
            }

            if (maxGain >= 0 && maxNode != null)
            {
                int[] o = outputs.Submatrix(info[maxNode].subset.ToArray());

                // prune the maximum gain node
                int common = Accord.Statistics.Tools.Mode(o);

                maxNode.Branches = null;
                maxNode.Output = common;
            }

            return computeError();
        }



        private double computeError(DecisionNode node)
        {
            List<int> indices = info[node].subset;

            int error = 0;
            foreach (int i in indices)
                if (outputs[i] != actual[i]) error++;
            return error / (double)indices.Count;
        }

        private double computeGain(DecisionNode node)
        {
            if (node.IsLeaf) return Double.NegativeInfinity;

            // Compute the sum of misclassifications at the children
            double sum = 0;
            foreach (var child in node.Branches)
                sum += info[child].error;

            // Get the misclassifications at the current node
            double current = info[node].error;

            // Compute the expected gain at the current node:
            return sum - current;
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



        private void trackDecisions(DecisionNode root, double[] input, int index)
        {
            DecisionNode current = root;

            while (current != null)
            {
                info[current].subset.Add(index);

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
