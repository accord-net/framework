// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Alex Risman, 2016
// https://github.com/mthmn20
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
    using System.Linq;
    using System.Text;
    using System.Data;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.IO;
    using Accord.Statistics.Filters;
    using Accord.Math;
    using AForge;
    using Accord.Statistics;
    using Accord.MachineLearning.DecisionTrees.Learning;
    using Accord.Compat;
    using System.Threading.Tasks;
    using Accord.Collections;
    using Accord.Math.Optimization.Losses;

    /// <summary>
    ///   Random Forest learning algorithm.
    /// </summary>
    /// 
    /// <example>
    /// <para>
    ///   This example shows the simplest way to induce a decision tree with continuous variables.</para>
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\DecisionTrees\RandomForestTest.cs" region="doc_iris" />
    /// <para>
    ///   The next example shows how to induce a decision tree with continuous variables using a 
    ///   <see cref="Accord.Statistics.Filters.Codification">codebook</see> to manage how input 
    ///   variables should be encoded.</para>
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\DecisionTrees\RandomForestTest.cs" region="doc_nursery" />
    /// </example>
    /// 
    /// <seealso cref="RandomForest"/>
    /// <seealso cref="C45Learning"/>
    /// 
    [Serializable]
    public class RandomForestLearning : ParallelLearningBase,
        ISupervisedLearning<RandomForest, double[], int>
    {
        private RandomForest forest;
        private IList<DecisionVariable> attributes;

        /// <summary>
        ///   Gets or sets the number of trees in the random forest.
        /// </summary>
        /// 
        [Obsolete("Please use NumberOfTrees instead.")]
        public int Trees
        {
            get { return NumberOfTrees; }
            set { NumberOfTrees = value; }
        }

        /// <summary>
        ///   Gets or sets the number of trees in the random forest.
        /// </summary>
        /// 
        public int NumberOfTrees { get; set; }

        /// <summary>
        ///   Gets or sets how many times the same variable can 
        ///   enter a tree's decision path. Default is 100.
        /// </summary>
        /// 
        public int Join { get; set; }

        /// <summary>
        ///   Gets or sets the maximum allowed height when learning a tree. If 
        ///   set to zero, the tree can have an arbitrary length. Default is 0.
        /// </summary>
        /// 
        public int MaxHeight { get; set; }

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
        ///   Gets the proportion of samples used to train each
        ///   of the trees in the decision forest. Default is 0.632.
        /// </summary>
        /// 
        public double SampleRatio { get; set; }

        /// <summary>
        ///   Gets or sets the proportion of variables that
        ///   can be used at maximum by each tree in the decision
        ///   forest. Default is 1 (always use all variables).
        /// </summary>
        /// 
        public double CoverageRatio { get; set; }

        /// <summary>
        ///   Creates a new decision forest learning algorithm.
        /// </summary>
        /// 
        public RandomForestLearning()
        {
            this.SampleRatio = 0.632;
            this.CoverageRatio = 1;
            this.NumberOfTrees = 100;
            this.Join = 100;
        }

        /// <summary>
        ///   Creates a new decision forest learning algorithm.
        /// </summary>
        /// 
        public RandomForestLearning(RandomForest forest)
            : this()
        {
            this.NumberOfTrees = forest.Trees.Length;
            this.forest = forest;
        }

        /// <summary>
        ///   Creates a new decision forest learning algorithm.
        /// </summary>
        /// 
        /// <param name="attributes">The attributes to be processed by the induced tree.</param>
        ///
        public RandomForestLearning(OrderedDictionary<string, string[]> attributes)
            : this()
        {
            this.attributes = DecisionVariable.FromDictionary(attributes);
        }

        /// <summary>
        ///   Creates a new decision forest learning algorithm.
        /// </summary>
        /// 
        /// <param name="attributes">The attributes to be processed by the induced tree.</param>
        ///
        public RandomForestLearning(DecisionVariable[] attributes)
            : this()
        {
            this.attributes = attributes;
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
        public RandomForest Learn(int[][] x, int[] y, double[] weights = null)
        {
            if (weights != null)
                throw new ArgumentException(Accord.Properties.Resources.NotSupportedWeights, "weights");

            if (forest == null)
            {
                if (this.attributes == null)
                    this.attributes = DecisionVariable.FromData(x);
                this.forest = CreateTree(y);
            }

            run(x, y);
            return this.forest;
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
        public RandomForest Learn(double[][] x, int[] y, double[] weights = null)
        {
            if (weights != null)
                throw new ArgumentException(Accord.Properties.Resources.NotSupportedWeights, "weights");

            if (forest == null)
            {
                if (this.attributes == null)
                    this.attributes = DecisionVariable.FromData(x);
                this.forest = CreateTree(y);
            }

            run(x, y);
            return this.forest;
        }

        private RandomForest CreateTree(int[] y)
        {
            return new RandomForest(NumberOfTrees, this.attributes, y.Max() + 1);
        }


        /// <summary>
        ///   Runs the learning algorithm with the given data.
        /// </summary>
        /// 
        /// <param name="inputs">The input points.</param>
        /// <param name="output">The class label for each point.</param>
        /// 
        [Obsolete("Please use the Learn(x, y) method instead.")]
        public double Run(double[][] inputs, int[] output)
        {
            run(inputs, output);
            return new ZeroOneLoss(output).Loss(this.forest.Decide(inputs));
        }

        private void run(double[][] inputs, int[] output)
        {
            Parallel.For(0, forest.Trees.Length, ParallelOptions, i =>
            {
                learn<double, C45Learning, DecisionTree>(inputs, output, new C45Learning(forest.Trees[i]));
            });
        }

        private void run(int[][] inputs, int[] output)
        {
            Parallel.For(0, forest.Trees.Length, ParallelOptions, i =>
            {
                learn<int, ID3Learning, DecisionTree>(inputs, output, new ID3Learning(forest.Trees[i]));
            });
        }

        private void learn<TData, TLearning, TModel>(TData[][] inputs, int[] output, TLearning teacher)
            where TLearning : DecisionTreeLearningBase, ISupervisedLearning<TModel, TData[], int>
            where TModel : DecisionTree, ITransform<TData[], int>
        {
            teacher.MaxVariables = getColsPerTree(inputs);
            teacher.MaxHeight = this.MaxHeight;
            teacher.Join = this.Join;
            int[] idx = Vector.Sample(SampleRatio, output.Length);
            teacher.Learn(inputs.Get(idx), output.Get(idx));
        }

        private int getColsPerTree<T>(T[][] inputs)
        {
            int rows = inputs.Length;
            int cols = inputs[0].Length;

            int colsPerTree = 0;
            if (CoverageRatio == 0)
            {
                colsPerTree = (int)(System.Math.Sqrt(cols));
            }
            else
            {
                colsPerTree = (int)(cols * CoverageRatio);
            }

            return colsPerTree;
        }

    }
}
