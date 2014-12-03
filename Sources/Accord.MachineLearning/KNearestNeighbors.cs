// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
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
    using Accord.MachineLearning.Structures;

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
    public class KNearestNeighbors : KNearestNeighbors<double[]>
    {

        private KDTree<int> tree;

        /// <summary>
        ///   Creates a new <see cref="KNearestNeighbors"/>.
        /// </summary>
        /// 
        /// <param name="k">The number of nearest neighbors to be used in the decision.</param>
        /// 
        /// <param name="inputs">The input data points.</param>
        /// <param name="outputs">The associated labels for the input points.</param>
        /// 
        public KNearestNeighbors(int k, double[][] inputs, int[] outputs)
            : base(k, inputs, outputs, Accord.Math.Distance.Euclidean)
        {
            this.tree = KDTree.FromData(inputs, outputs, Accord.Math.Distance.Euclidean);
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
        public KNearestNeighbors(int k, int classes, double[][] inputs, int[] outputs)
            : base(k, classes, inputs, outputs, Accord.Math.Distance.Euclidean)
        {
            this.tree = KDTree.FromData(inputs, outputs, Accord.Math.Distance.Euclidean);
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
        public KNearestNeighbors(int k, int classes, double[][] inputs, int[] outputs, Func<double[], double[], double> distance)
            : base(k, classes, inputs, outputs, distance)
        {
            this.tree = KDTree.FromData(inputs, outputs, distance);
        }


        private KNearestNeighbors(KDTree<int> tree, int k, int classes, 
            double[][] inputs, int[] outputs, Func<double[], double[], double> distance)
            : base(k, classes, inputs, outputs, distance)
        {
            this.tree = tree;
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
        public override int Compute(double[] input, out double[] scores)
        {
            KDTreeNodeCollection<int> neighbors = tree.Nearest(input, this.K);

            scores = new double[ClassCount];

            foreach (KDTreeNodeDistance<int> point in neighbors)
            {
                int label = point.Node.Value;
                double d = point.Distance;

                // Convert to similarity measure
                scores[label] += 1.0 / (1.0 + d);
            }

            // Get the maximum weighted score
            int result; scores.Max(out result);

            return result;
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
            var knn = new KNearestNeighbors(tree, k, classes, inputs, outputs, tree.Distance);

            return knn;
        }

    }
}
