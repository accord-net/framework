﻿// Accord Machine Learning Library
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

namespace Accord.MachineLearning
{
    using System;
    using System.Linq;
    using Accord.Math;
    using Accord.Collections;
    using Accord.Math.Distances;

    /// <summary>
    ///   K-Nearest Neighbor (k-NN) algorithm.
    /// </summary>
    /// 
    /// <remarks>
    ///   For more detailed documentation, including examples and code snippets, 
    ///   please take a look on the <see cref="KNearestNeighbors{T}"/> documentation
    ///   page.
    /// </remarks>
    /// 
    /// <seealso cref="KNearestNeighbors{T}"/>
    /// 
    [Serializable]
    public class KNearestNeighbors : 
        BaseKNearestNeighbors<KNearestNeighbors, double[], IMetric<double[]>>
    {

        private KDTree<int> tree;

        /// <summary>
        ///   Creates a new <see cref="KNearestNeighbors"/>.
        /// </summary>
        /// 
        public KNearestNeighbors()
        {
        }

        /// <summary>
        ///   Creates a new <see cref="KNearestNeighbors"/>.
        /// </summary>
        /// 
        public KNearestNeighbors(int k)
        {
            this.K = k;
            this.Distance = new Euclidean();
        }

        /// <summary>
        ///   Creates a new <see cref="KNearestNeighbors"/>.
        /// </summary>
        /// 
        public KNearestNeighbors(int k, IMetric<double[]> distance)
        {
            this.K = k;
            this.Distance = distance;
        }


        /// <summary>
        /// Computes a numerical score measuring the association between
        /// the given <paramref name="input" /> vector and each class.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="result">An array where the result will be stored,
        /// avoiding unnecessary memory allocations.</param>
        /// <returns>System.Double[].</returns>
        public override double[] Scores(double[] input, double[] result)
        {
            KDTreeNodeCollection<KDTreeNode<int>> neighbors = tree.Nearest(input, this.K);

            foreach (NodeDistance<KDTreeNode<int>> point in neighbors)
            {
                int label = point.Node.Value;
                double d = point.Distance;

                // Convert to similarity measure
                result[label] += 1.0 / (1.0 + d);
            }

            return result;
        }




        /// <summary>
        ///   Gets the top <see cref="BaseKNearestNeighbors{TModel, TInput, TDistance}.K"/> points that are the closest
        ///   to a given <paramref name="input">reference point</paramref>.
        /// </summary>
        /// 
        /// <param name="input">The query point whose neighbors will be found.</param>
        /// <param name="labels">The label for each neighboring point.</param>
        /// 
        /// <returns>
        ///   An array containing the top <see cref="BaseKNearestNeighbors{TModel, TInput, TDistance}.K"/> points that are 
        ///   at the closest possible distance to <paramref name="input"/>.
        /// </returns>
        /// 
        public override double[][] GetNearestNeighbors(double[] input, out int[] labels)
        {
            var neighbors = tree.Nearest(input, this.K);

            double[][] points = new double[neighbors.Count][];
            labels = new int[neighbors.Count];

            int k = 0;
            foreach (var point in neighbors)
            {
                points[k] = point.Node.Position;
                labels[k] = point.Node.Value;
                k++;
            }

            return points;
        }

        /// <summary>
        ///   Creates a new <see cref="KNearestNeighbors"/> algorithm from an existing
        ///   <see cref="KDTree{T}"/>. The tree must have been created using the input
        ///   points and the point's class labels as the associated node information.
        /// </summary>
        /// 
        /// <param name="tree">The <see cref="KDTree{T}"/> containing the input points and their integer labels.</param>
        /// <param name="k">The number of nearest neighbors to be used in the decision.</param>
        /// <param name="classes">The number of classes in the classification problem.</param>
        /// <param name="inputs">The input data points.</param>
        /// <param name="outputs">The associated labels for the input points.</param>
        /// 
        /// <returns>A <see cref="KNearestNeighbors"/> algorithm initialized from the tree.</returns>
        /// 
        public static KNearestNeighbors FromTree(KDTree<int> tree, int k, int classes, double[][] inputs, int[] outputs)
        {
            var knn = new KNearestNeighbors();
            knn.K = k;
            knn.Inputs = inputs;
            knn.Outputs = outputs;
            knn.NumberOfInputs = inputs.Columns();
            knn.NumberOfOutputs = outputs.DistinctCount();
            knn.NumberOfClasses = knn.NumberOfOutputs;
            knn.tree = tree;

            return knn;
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
        public override KNearestNeighbors Learn(double[][] x, int[] y, double[] weights = null)
        {
            CheckArgs(K, x, y, Distance, weights);

            this.NumberOfInputs = GetNumberOfInputs(x);
            this.Inputs = x;
            this.Outputs = y;

            this.NumberOfOutputs = y.DistinctCount();
            this.NumberOfClasses = this.NumberOfOutputs;

            this.tree = KDTree.FromData(points: x, values: y, distance: Distance);

            return this;
        }





        #region Obsolete
        /// <summary>
        ///   Creates a new <see cref="KNearestNeighbors"/>.
        /// </summary>
        /// 
        /// <param name="k">The number of nearest neighbors to be used in the decision.</param>
        /// 
        /// <param name="inputs">The input data points.</param>
        /// <param name="outputs">The associated labels for the input points.</param>
        /// 
        [Obsolete("Please use KNearestNeighbors(int k) constructor instead.")]
        public KNearestNeighbors(int k, double[][] inputs, int[] outputs)
        {
            this.K = k;
            this.Distance = new Accord.Math.Distances.Euclidean();
            Learn(inputs, outputs);
        }

        /// <summary>
        ///   Creates a new <see cref="KNearestNeighbors"/>.
        /// </summary>
        /// 
        /// <param name="k">The number of nearest neighbors to be used in the decision.</param>
        /// <param name="classes">The number of classes in the classification problem.</param>
        /// 
        /// <param name="inputs">The input data points.</param>
        /// <param name="outputs">The associated labels for the input points.</param>
        /// 
        [Obsolete("Please use KNearestNeighbors(int k) constructor instead.")]
        public KNearestNeighbors(int k, int classes, double[][] inputs, int[] outputs)
        {
            this.K = k;
            this.Distance = new Accord.Math.Distances.Euclidean();
            Learn(inputs, outputs);
            if (classes != NumberOfOutputs)
                throw new ArgumentException("classes");
        }

        /// <summary>
        ///   Creates a new <see cref="KNearestNeighbors"/>.
        /// </summary>
        /// 
        /// <param name="k">The number of nearest neighbors to be used in the decision.</param>
        /// <param name="classes">The number of classes in the classification problem.</param>
        /// <param name="inputs">The input data points.</param>
        /// <param name="outputs">The associated labels for the input points.</param>
        /// <param name="distance">The distance measure to use.</param>
        /// 
        [Obsolete("Please use KNearestNeighbors(int k, IDistance<T> distance) constructor instead.")]
        public KNearestNeighbors(int k, int classes, double[][] inputs, int[] outputs, IMetric<double[]> distance)
        {
            this.K = k;
            this.Distance = distance;
            Learn(inputs, outputs);
            if (classes != NumberOfOutputs)
                throw new ArgumentException("classes");
        }

        /// <summary>
        ///   Gets the number of class labels
        ///   handled by this classifier.
        /// </summary>
        /// 
        [Obsolete("Please use NumberOfOutputs instead.")]
        public int ClassCount
        {
            get { return NumberOfOutputs; }
        }

        /// <summary>
        ///   Computes the most likely label of a new given point.
        /// </summary>
        /// 
        /// <param name="input">A point to be classified.</param>
        /// 
        /// <returns>The most likely label for the given point.</returns>
        /// 
        [Obsolete("Please use the Decide(input) method instead.")]
        public int Compute(double[] input)
        {
            return Decide(input);
        }

        /// <summary>
        ///   Computes the most likely label of a new given point.
        /// </summary>
        /// 
        /// <param name="input">A point to be classified.</param>
        /// <param name="response">A value between 0 and 1 giving 
        /// the strength of the classification in relation to the
        /// other classes.</param>
        /// 
        /// <returns>The most likely label for the given point.</returns>
        /// 
        [Obsolete("Please use the Score(input, out decision) method instead.")]
        public int Compute(double[] input, out double response)
        {
            int decision;
            response = Score(input, out decision);
            return decision;
        }

        /// <summary>
        ///   Computes the most likely label of a new given point.
        /// </summary>
        /// 
        /// <param name="input">A point to be classified.</param>
        /// <param name="scores">The distance score for each possible class.</param>
        /// 
        /// <returns>The most likely label for the given point.</returns>
        /// 
        [Obsolete("Please use the Scores(input, out decision) method instead.")]
        public virtual int Compute(double[] input, out double[] scores)
        {
            int decision;
            scores = Scores(input, out decision);
            return decision;
        }
        #endregion


    }
}
