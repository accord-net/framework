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

namespace Accord.MachineLearning.Boosting.Learners
{
    using System;
    using Accord.Math.Comparers;
    using Accord.Math;
    using Accord.MachineLearning.DecisionTrees;
    using Accord.Statistics;

    /// <summary>
    ///   Learning algorithm for <see cref="DecisionStump"/>s.
    /// </summary>
    /// 
    /// <seealso cref="AdaBoost{TModel}" />
    /// <seealso cref="DecisionStump" />
    /// 
    /// <example>
    /// <para>
    ///   The <see cref="ThresholdLearning"/> algorithm is mostly intended to be used to create 
    ///   <see cref="DecisionStump"/> weak classifiers in the context of an <see cref="AdaBoost{TModel}"/> 
    ///   learning algorithm. Please refer to the <see cref="AdaBoost{TModel}"/> class for more examples
    ///   on using the classifier in this scenario. A simple example is shown below:</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\Boosting\AdaBoostTest.cs" region="doc_learn" />
    ///   
    /// <para>
    ///   It is also possible to use the <see cref="ThresholdLearning"/> as a standalone learning algorithm. 
    ///   An example is given below:</para>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\Boosting\DecisionStumpTest.cs" region="doc_learn" />
    /// </example>
    /// 
    public class ThresholdLearning : ParallelLearningBase,
        ISupervisedBinaryLearning<DecisionStump, double[]>
    {
        /// <summary>
        ///   Gets or sets the model being trained.
        /// </summary>
        /// 
        public DecisionStump Model { get; set; }

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
        public DecisionStump Learn(double[][] x, int[][] y, double[] weights = null)
        {
            return Learn(x, y.ArgMax(dimension: 0), weights);
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
        public DecisionStump Learn(double[][] x, bool[][] y, double[] weights = null)
        {
            return Learn(x, y.ArgMax(dimension: 0), weights);
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
        public DecisionStump Learn(double[][] x, int[] y, double[] weights = null)
        {
            return Learn(x, Classes.Decide(y), weights);
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
        public DecisionStump Learn(double[][] x, bool[] y, double[] weights = null)
        {
            if (weights == null)
                weights = Vector.Create(x.Length, 1.0 / x.Length);

            if (Model == null)
                Model = new DecisionStump();

            var comparer = new ElementComparer();
            double errorMinimum = Double.MaxValue;
            int numberOfVariables = x.Columns();

            for (int i = 0; i < numberOfVariables; i++)
            {
                comparer.Index = i;
                int[] indices = Vector.Range(0, x.Length);
                Array.Sort(indices, (a, b) => x[a][i].CompareTo(x[b][i]));

                double error = 0.0;
                for (int j = 0; j < y.Length; j++)
                {
                    int idx = indices[j];
                    if (y[idx])
                        error += weights[idx];
                }

                for (int j = 0; j < y.Length - 1; j++)
                {
                    int idx = indices[j];
                    int nidx = indices[j + 1];

                    if (y[idx])
                        error -= weights[idx];
                    else
                        error += weights[idx];

                    double midpoint = (x[idx][i] + x[nidx][i]) / 2.0;

                    // Compare to current best
                    if (error < errorMinimum)
                    {
                        errorMinimum = error;
                        Model.Index = i;
                        Model.Threshold = midpoint;
                        Model.Comparison = ComparisonKind.LessThan;
                    }

                    if ((1.0 - error) < errorMinimum)
                    {
                        errorMinimum = (1.0 - error);
                        Model.Index = i;
                        Model.Threshold = midpoint;
                        Model.Comparison = ComparisonKind.GreaterThan;
                    }
                }
            }

            return Model;
        }
    }
}
