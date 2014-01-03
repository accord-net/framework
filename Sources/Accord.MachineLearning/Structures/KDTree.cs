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

namespace Accord.MachineLearning.Structures
{
    using System;

    /// <summary>
    ///   Convenience class for k-dimensional tree static methods. To 
    ///   create a new KDTree, specify the generic parameter as in
    ///   <see cref="KDTree{T}"/>.
    /// </summary>
    /// 
    /// <remarks>
    ///   Please check the documentation page for <see cref="KDTree{T}"/>
    ///   for examples, usage and actual remarks about kd-trees.
    /// </remarks>
    /// 
    /// <seealso cref="KDTree{T}"/>
    /// 
    public class KDTree : KDTree<Object>
    {

        /// <summary>
        ///   Creates a new <see cref="KDTree&lt;Object&gt;"/>.
        /// </summary>
        /// 
        /// <param name="dimensions">The number of dimensions in the tree.</param>
        /// 
        public KDTree(int dimensions)
            : base(dimensions)
        {
        }

        /// <summary>
        ///   Creates a new <see cref="KDTree&lt;Object&gt;"/>.
        /// </summary>
        /// 
        /// <param name="dimension">The number of dimensions in the tree.</param>
        /// <param name="root">The root node, if already existent.</param>
        /// 
        public KDTree(int dimension, KDTreeNode root) 
            : base(dimension, root)
        {
        }

        /// <summary>
        ///   Creates a new <see cref="KDTree&lt;Object&gt;"/>.
        /// </summary>
        /// 
        /// <param name="dimension">The number of dimensions in the tree.</param>
        /// <param name="root">The root node, if already existent.</param>
        /// 
        public KDTree(int dimension, KDTreeNode<Object> root)
            : base(dimension, root)
        {
        }

        /// <summary>
        ///   Creates a new <see cref="KDTree&lt;Object&gt;"/>.
        /// </summary>
        /// 
        /// <param name="dimension">The number of dimensions in the tree.</param>
        /// <param name="root">The root node, if already existent.</param>
        /// <param name="count">The number of elements in the root node.</param>
        /// <param name="leaves">The number of leaves linked through the root node.</param>
        /// 
        public KDTree(int dimension, KDTreeNode root, int count, int leaves)
            : base(dimension, root, count, leaves)
        {
        }

        /// <summary>
        ///   Creates a new <see cref="KDTree&lt;Object&gt;"/>.
        /// </summary>
        /// 
        /// <param name="dimension">The number of dimensions in the tree.</param>
        /// <param name="root">The root node, if already existent.</param>
        /// <param name="count">The number of elements in the root node.</param>
        /// <param name="leaves">The number of leaves linked through the root node.</param>
        /// 
        public KDTree(int dimension, KDTreeNode<Object> root, int count, int leaves)
            : base(dimension, root, count, leaves)
        {
        }


        /// <summary>
        ///   Creates a new k-dimensional tree from the given points.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the value to be stored.</typeparam>
        /// 
        /// <param name="points">The points to be added to the tree.</param>
        /// <param name="inPlace">Whether the given <paramref name="points"/> vector
        ///   can be ordered in place. Passing true will change the original order of
        ///   the vector. If set to false, all operations will be performed on an extra
        ///   copy of the vector.</param>
        /// 
        /// <returns>A <see cref="KDTree{T}"/> populated with the given data points.</returns>
        /// 
        public static KDTree<T> FromData<T>(double[][] points, bool inPlace = false)
        {
            if (points == null)
                throw new ArgumentNullException("points");

            if (points.Length == 0)
                throw new ArgumentException("Insufficient points for creating a tree.");

            int leaves;

            var root = KDTree<T>.CreateRoot(points, inPlace, out leaves);

            return new KDTree<T>(points[0].Length, root, points.Length, leaves);
        }

        /// <summary>
        ///   Creates a new k-dimensional tree from the given points.
        /// </summary>
        /// 
        /// <param name="points">The points to be added to the tree.</param>
        /// <param name="inPlace">Whether the given <paramref name="points"/> vector
        ///   can be ordered in place. Passing true will change the original order of
        ///   the vector. If set to false, all operations will be performed on an extra
        ///   copy of the vector.</param>
        /// 
        /// <returns>A <see cref="KDTree{T}"/> populated with the given data points.</returns>
        /// 
        public static KDTree FromData(double[][] points, bool inPlace = false)
        {
            if (points == null)
                throw new ArgumentNullException("points");

            if (points.Length == 0)
                throw new ArgumentException("Insufficient points for creating a tree.");

            int leaves;

            var root = CreateRoot(points, inPlace, out leaves);

            return new KDTree(points[0].Length, root, points.Length, leaves);
        }

        /// <summary>
        ///   Creates a new k-dimensional tree from the given points.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the value to be stored.</typeparam>
        /// 
        /// <param name="points">The points to be added to the tree.</param>
        /// <param name="values">The corresponding values at each data point.</param>
        /// <param name="inPlace">Whether the given <paramref name="points"/> vector
        ///   can be ordered in place. Passing true will change the original order of
        ///   the vector. If set to false, all operations will be performed on an extra
        ///   copy of the vector.</param>
        /// 
        /// <returns>A <see cref="KDTree{T}"/> populated with the given data points.</returns>
        /// 
        public static KDTree<T> FromData<T>(double[][] points, T[] values, bool inPlace = false)
        {
            if (points == null)
                throw new ArgumentNullException("points");

            if (points.Length == 0)
                throw new ArgumentException("Insufficient points for creating a tree.");

            int leaves;

            var root = KDTree<T>.CreateRoot(points, values, inPlace, out leaves);
            
            return new KDTree<T>(points[0].Length, root, points.Length, leaves);
        }

        /// <summary>
        ///   Creates a new k-dimensional tree from the given points.
        /// </summary>
        /// 
        /// <param name="points">The points to be added to the tree.</param>
        /// <param name="distance">The distance function to use.</param>
        /// <param name="inPlace">Whether the given <paramref name="points"/> vector
        ///   can be ordered in place. Passing true will change the original order of
        ///   the vector. If set to false, all operations will be performed on an extra
        ///   copy of the vector.</param>
        /// 
        /// <returns>A <see cref="KDTree{T}"/> populated with the given data points.</returns>
        /// 
        public static KDTree FromData(double[][] points, Func<double[], double[], double> distance,
            bool inPlace = false)
        {
            if (points == null)
                throw new ArgumentNullException("points");

            if (distance == null)
                throw new ArgumentNullException("distance");

            if (points.Length == 0)
                throw new ArgumentException("Insufficient points for creating a tree.");

            int leaves;

            var root = CreateRoot(points, inPlace, out leaves);

            return new KDTree(points[0].Length, root, points.Length, leaves)
            {
                Distance = distance,
            };
        }

        /// <summary>
        ///   Creates a new k-dimensional tree from the given points.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the value to be stored.</typeparam>
        /// 
        /// <param name="points">The points to be added to the tree.</param>
        /// <param name="values">The corresponding values at each data point.</param>
        /// <param name="distance">The distance function to use.</param>
        /// <param name="inPlace">Whether the given <paramref name="points"/> vector
        ///   can be ordered in place. Passing true will change the original order of
        ///   the vector. If set to false, all operations will be performed on an extra
        ///   copy of the vector.</param>
        /// 
        /// <returns>A <see cref="KDTree{T}"/> populated with the given data points.</returns>
        /// 
        public static KDTree<T> FromData<T>(double[][] points, T[] values, 
            Func<double[], double[], double> distance, bool inPlace = false)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            if (distance == null)
                throw new ArgumentNullException("distance");

            int leaves;

            var root = KDTree<T>.CreateRoot(points, values, inPlace, out leaves);

            return new KDTree<T>(points[0].Length, root, points.Length, leaves)
            {
                Distance = distance,
            };
        }

        /// <summary>
        ///   Creates a new k-dimensional tree from the given points.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the value to be stored.</typeparam>
        /// 
        /// <param name="points">The points to be added to the tree.</param>
        /// <param name="distance">The distance function to use.</param>
        /// <param name="inPlace">Whether the given <paramref name="points"/> vector
        ///   can be ordered in place. Passing true will change the original order of
        ///   the vector. If set to false, all operations will be performed on an extra
        ///   copy of the vector.</param>
        /// 
        /// <returns>A <see cref="KDTree{T}"/> populated with the given data points.</returns>
        /// 
        public static KDTree<T> FromData<T>(double[][] points, Func<double[], double[], double> distance, 
            bool inPlace = false)
        {
            if (distance == null)
                throw new ArgumentNullException("distance");

            int leaves;

            var root = KDTree<T>.CreateRoot(points, inPlace, out leaves);

            return new KDTree<T>(points[0].Length, root, points.Length, leaves)
            {
                Distance = distance
            };
        }

    }
}
