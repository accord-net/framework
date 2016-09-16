// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Alex Risman, 2016
// https://github.com/mthmn20
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

namespace Accord.MachineLearning.DecisionTrees
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Data;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.IO;
    using Accord.Statistics.Filters;
    using Accord.Math;
    using AForge;
    using Accord.Statistics;
    using Accord.MachineLearning.DecisionTrees.Learning;


    /// <summary>
    ///   Random Forest learning algorithm.
    /// </summary>
    /// 
    [Serializable]
    public class RandomForestLearning
    {
        private RandomForest forest;

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
        }

        /// <summary>
        ///   Creates a new decision forest learning algorithm.
        /// </summary>
        /// 
        public RandomForestLearning(RandomForest forest)
        {
            this.SampleRatio = 0.632;
            this.NumberOfTrees = forest.Trees.Length;
            this.forest = forest;
        }

        /// <summary>
        ///   Learns a new Random Forest with the given data.
        /// </summary>
        /// 
        /// <param name="inputs">The input points.</param>
        /// <param name="output">The class label for each point.</param>
        /// 
        /// <returns>A <see cref="RandomForest"/> object that learned
        ///   how to assign class labels to input points.</returns>
        /// 
        public RandomForest Learn(double[][] inputs, int[] output)
        {
            if (forest == null)
            {
                int classes = output.Max() + 1;
                this.forest = new RandomForest(NumberOfTrees, classes);
                var variables = DecisionVariable.FromData(inputs);
                for (int i = 0; i < forest.Trees.Length; i++)
                    forest.Trees[i] = new DecisionTree(variables, classes);
            }

            run(inputs, output);
            return this.forest;
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
            return run(inputs, output);
        }

        private double run(double[][] inputs, int[] output)
        {
            int rows = inputs.Length;
            int cols = inputs[0].Length;
            int classes = output.DistinctCount();

            int colsPerTree;

            if (CoverageRatio == 0)
            {
                colsPerTree = (int)(System.Math.Sqrt(cols));
            }
            else
            {
                colsPerTree = (int)(cols * CoverageRatio);
            }

            var trees = forest.Trees;
            int[] idx = Classes.Random(output, classes, trees.Length);

            Parallel.For(0, trees.Length, i =>
            {
                var x = inputs.Get(idx);
                var y = output.Get(idx);

                var c45 = new C45Learning(forest.Trees[i])
                {
                    MaxVariables = colsPerTree,
                    Join = 100
                };

                c45.Learn(x, y);
            });

            return 0;
        }

    }
}
