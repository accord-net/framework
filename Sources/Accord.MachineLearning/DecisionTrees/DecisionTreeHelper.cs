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

namespace Accord.MachineLearning.DecisionTrees
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using Accord.MachineLearning.DecisionTrees.Rules;
    using Accord.Math;


    internal static class DecisionTreeHelper
    {
        public static void CheckArgs(DecisionTree tree, int[][] inputs, int[] outputs, double[] weights = null)
        {
            checkArgs(tree, inputs, outputs, weights);

            for (int i = 0; i < inputs.Length; i++)
            {
                for (int j = 0; j < inputs[i].Length; j++)
                {
                    if (tree.Attributes[j].Nature != DecisionVariableKind.Discrete)
                        throw new Exception("The tree expects real-valued inputs.");

                    double min = tree.Attributes[j].Range.Min;
                    double max = tree.Attributes[j].Range.Max;

                    if (inputs[i][j] < min || inputs[i][j] > max)
                    {
                        throw new ArgumentOutOfRangeException("inputs",
                            String.Format("The input vector at index {0} contains an invalid entry ({1}) at column {2}. The tree expected this value to be between [{3}, {4}].",
                            i, inputs[i][j], j, min, max));
                    }
                }
            }
        }

        public static void CheckArgs(DecisionTree tree, double[][] inputs, int[] outputs, double[] weights = null)
        {
            checkArgs(tree, inputs, outputs, weights);

            for (int i = 0; i < inputs.Length; i++)
            {
                for (int j = 0; j < inputs[i].Length; j++)
                {
                    if (tree.Attributes[j].Nature != DecisionVariableKind.Discrete)
                        continue;

                    double min = tree.Attributes[j].Range.Min;
                    double max = tree.Attributes[j].Range.Max;

                    if (inputs[i][j] < min || inputs[i][j] > max)
                    {
                        throw new ArgumentOutOfRangeException("inputs",
                            String.Format("The input vector at index {0} contains an invalid entry ({1}) at column {2}. The tree expected this value to be between [{3}, {4}].",
                            i, inputs[i][j], j, min, max));
                    }
                }
            }
        }

        private static void checkArgs(DecisionTree tree, Array[] inputs, int[] outputs, double[] weights = null)
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

            if (weights != null)
                if (inputs.Length != weights.Length)
                    throw new ArgumentOutOfRangeException("weights",
                        "The number of input vectors and weights does not match.");

            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i] == null)
                {
                    throw new ArgumentNullException("inputs",
                        "The input vector at index " + i + " is null.");
                }

                if (inputs[i].Length != tree.NumberOfInputs)
                {
                    throw new DimensionMismatchException("inputs",
                        String.Format("The input vector at index {0} has {1} dimensions but the tree expects {2}.",
                        i, inputs[i].Length, tree.NumberOfInputs));
                }
            }

            for (int i = 0; i < outputs.Length; i++)
            {
                if (outputs[i] < 0 || outputs[i] >= tree.NumberOfOutputs)
                {
                    throw new ArgumentOutOfRangeException("outputs",
                      String.Format("The output label {0} at index {1} should be >= 0 and < {2} (the number of output classes expected by the tree).",
                        i, outputs[i], tree.NumberOfOutputs));
                }
            }
        }

        public static DecisionTree Create(double[][] x, int[] y, IList<DecisionVariable> attributes)
        {
            if (attributes == null || attributes.Count == 0)
                attributes = DecisionVariable.FromData(x);
            return Create(y, attributes);
        }

        public static DecisionTree Create(int[][] x, int[] y, IList<DecisionVariable> attributes)
        {
            if (attributes == null || attributes.Count == 0)
                attributes = DecisionVariable.FromData(x);
            return Create(y, attributes);
        }

        public static DecisionTree Create(int?[][] x, int[] y, IList<DecisionVariable> attributes)
        {
            if (attributes == null || attributes.Count == 0)
                attributes = DecisionVariable.FromData(x);
            return Create(y, attributes);
        }

        private static DecisionTree Create(int[] y, IList<DecisionVariable> attributes)
        {
            int classes = y.Max() + 1;
            var tree = new DecisionTree(attributes, classes);
            return tree;
        }
    }
}

