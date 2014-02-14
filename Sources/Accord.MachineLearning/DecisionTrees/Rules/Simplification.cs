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

namespace Accord.MachineLearning.DecisionTrees.Rules
{
    using System.Collections.Generic;
    using System.Linq;
    using Accord.Math;
    using Accord.Statistics.Analysis;
    using Accord.Statistics.Testing;

    /// <summary>
    ///   Decision rule simplification algorithm.
    /// </summary>
    /// 
    public class Simplification
    {
        DecisionSet decisionList;
        double alpha = 0.05;

        /// <summary>
        ///   Gets or sets the underlying hypothesis test
        ///   size parameter used to reject hypothesis.
        /// </summary>
        /// 
        public double Alpha
        {
            get { return alpha; }
            set { alpha = value; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Simplification"/> class.
        /// </summary>
        /// 
        /// <param name="list">The decision set to be simplified.</param>
        /// 
        public Simplification(DecisionSet list)
        {
            this.decisionList = list;
        }



        /// <summary>
        ///   Computes the reduction algorithm.
        /// </summary>
        /// 
        /// <param name="inputs">A set of training inputs.</param>
        /// <param name="outputs">The outputs corresponding to each of the inputs.</param>
        /// 
        /// <returns>The average error after the reduction.</returns>
        /// 
        public double Compute(double[][] inputs, int[] outputs)
        {
            int samples = outputs.Length;

            bool[] actual = new bool[samples];
            bool[] expected = new bool[samples];

            DecisionRule[] list = decisionList.ToArray();

            var antecedents = new HashSet<Antecedent>();


            foreach (DecisionRule rule in list)
                foreach (Antecedent antecedent in rule)
                    antecedents.Add(antecedent);



            // 1. Eliminate unnecessary antecedents
            for (int y = 0; y < decisionList.OutputClasses; y++)
            {
                for (int i = 0; i < outputs.Length; i++)
                    expected[i] = outputs[i] == y;

                var unnecessary = new HashSet<Antecedent>();

                /*foreach (var rule in list)
                {
                    if (rule.Output != y)
                        continue;
                */
                    foreach (var antecedent in antecedents)
                    {
                        for (int i = 0; i < inputs.Length; i++)
                            actual[i] = antecedent.Match(inputs[i]);

                        if (CanEliminate(actual, expected))
                            unnecessary.Add(antecedent);
                    }
                //}

                foreach (var antecedent in unnecessary)
                {
                    foreach (var rule in list)
                    {
                        if (rule.Output == y)
                            rule.Antecedents.Remove(antecedent);
                    }
                }
            }

            bool[][] matches = new bool[list.Length][];
            int[] counts = new int[list.Length];
            for (int i = 0; i < matches.Length; i++)
            {
                DecisionRule rule = list[i];

                matches[i] = new bool[outputs.Length];
                for (int j = 0; j < inputs.Length; j++)
                {
                    matches[i][j] = rule.Match(inputs[j]);

                    if (matches[i][j])
                        counts[i]++;
                }
            }

            double start = computeError(inputs, outputs, list);

            for (int i = 0; i < list.Length; i++)
            {
                if (list[i] == null)
                    continue;

                for (int j = 0; j < list.Length; j++)
                {
                    if (list[i] == null)
                        break;
                    if (list[j] == null)
                        continue;

                    if (list[i].IsInconsistentWith(list[j]))
                    {
                        if (counts[i] > counts[j])
                        {
                            list[j] = null;
                            counts[j] = 0;
                        }
                        else
                        {
                            list[i] = null;
                            counts[i] = 0;
                        }
                    }
                }
            }

            list = list.Distinct(allowNulls: false);

            List<DecisionRule> newList = new List<DecisionRule>(list);

            double middle = computeError(inputs, outputs, list);

            // 2. Eliminate redundant rules from the set

            for (int y = 0; y < decisionList.OutputClasses; y++)
            {
                for (int i = 0; i < outputs.Length; i++)
                    expected[i] = outputs[i] == y;

                var unnecessary = new HashSet<DecisionRule>();

                foreach (var rule in newList)
                {
                    if (rule.Output != y)
                        continue;

                    for (int i = 0; i < inputs.Length; i++)
                        actual[i] = rule.Match(inputs[i]);

                    if (CanEliminate(actual, expected))
                        unnecessary.Add(rule);
                }

                foreach (var rule in unnecessary)
                {
                    newList.Remove(rule);
                }
            }


            double final = computeError(inputs, outputs, newList);

            decisionList.Clear();
            decisionList.AddRange(newList);

            // Compute new decision error
            double newError = ComputeError(inputs, outputs);

            return newError;
        }

        /// <summary>
        ///   Computes the average decision error.
        /// </summary>
        /// 
        /// <param name="inputs">A set of input vectors.</param>
        /// <param name="outputs">A set of corresponding output vectors.</param>
        /// 
        /// <returns>The average misclassification rate.</returns>
        /// 
        public double ComputeError(double[][] inputs, int[] outputs)
        {
            return computeError(inputs, outputs, decisionList);
        }

        private static double computeError(double[][] inputs, int[] outputs, IEnumerable<DecisionRule> rules)
        {
            int errors = 0;
            for (int i = 0; i < outputs.Length; i++)
            {
                double expected = outputs[i];
                double? actual = match(rules, inputs[i]);

                if (expected != actual)
                    errors++;
            }

            return errors / (double)inputs.Length;
        }

        /// <summary>
        ///   Checks if two variables can be eliminated.
        /// </summary>
        /// 
        public bool CanEliminate(bool[] actual, bool[] expected)
        {
            return CanEliminate(actual, expected, alpha);
        }

        /// <summary>
        ///   Checks if two variables can be eliminated.
        /// </summary>
        /// 
        public static bool CanEliminate(bool[] actual, bool[] expected, double alpha)
        {
            var matrix = new ConfusionMatrix(actual, expected);

            double maxExpectedFrequency = matrix.ExpectedValues.Max();

            IHypothesisTest test;

            if (maxExpectedFrequency > 10)
                test = new ChiSquareTest(matrix, yatesCorrection: false) { Size = alpha };

            else if (maxExpectedFrequency >= 5)
                test = new ChiSquareTest(matrix, yatesCorrection: true) { Size = alpha };

            else
                test = new FisherExactTest(matrix) { Size = alpha };

            return !test.Significant;
        }


        private static double? match(IEnumerable<DecisionRule> rules, double[] input)
        {
            foreach (var rule in rules)
            {
                if (rule.Match(input))
                    return rule.Output;
            }

            return null;
        }

    }
}
