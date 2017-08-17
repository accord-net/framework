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

namespace Accord.Collections
{
    using System;
    using System.Collections.Generic;
    using Accord.Math;
    using Accord.Math.Comparers;
    using Accord.Math.Distances;
    using Accord.Compat;

    /// <summary>
    ///   K-dimensional tree.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the value being stored.</typeparam>
    /// 
    /// <remarks>
    /// <para>
    ///   A k-d tree (short for k-dimensional tree) is a space-partitioning data structure 
    ///   for organizing points in a k-dimensional space. k-d trees are a useful data structure
    ///   for several applications, such as searches involving a multidimensional search key 
    ///   (e.g. range searches and nearest neighbor searches). k-d trees are a special case 
    ///   of binary space partitioning trees.</para>
    ///   
    /// <para>
    ///   The k-d tree is a binary tree in which every node is a k-dimensional point. Every non-
    ///   leaf node can be thought of as implicitly generating a splitting hyperplane that divides
    ///   the space into two parts, known as half-spaces. Points to the left of this hyperplane 
    ///   represent the left subtree of that node and points right of the hyperplane are represented
    ///   by the right subtree. The hyperplane direction is chosen in the following way: every node 
    ///   in the tree is associated with one of the k-dimensions, with the hyperplane perpendicular 
    ///   to that dimension's axis. So, for example, if for a particular split the "x" axis is chosen,
    ///   all points in the subtree with a smaller "x" value than the node will appear in the left 
    ///   subtree and all points with larger "x" value will be in the right subtree. In such a case, 
    ///   the hyperplane would be set by the x-value of the point, and its normal would be the unit 
    ///   x-axis.</para>
    /// 
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Wikipedia, The Free Encyclopedia. K-d tree. Available on:
    ///       http://en.wikipedia.org/wiki/K-d_tree </description></item>
    ///     <item><description>
    ///       Moore, Andrew W. "An intoductory tutorial on kd-trees." (1991).
    ///       Available at: http://www.autonlab.org/autonweb/14665/version/2/part/5/data/moore-tutorial.pdf </description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <code lang="cs">
    /// // This is the same example found in Wikipedia page on
    /// // k-d trees: http://en.wikipedia.org/wiki/K-d_tree
    /// 
    /// // Suppose we have the following set of points:
    /// 
    /// double[][] points =
    /// {
    ///     new double[] { 2, 3 },
    ///     new double[] { 5, 4 },
    ///     new double[] { 9, 6 },
    ///     new double[] { 4, 7 },
    ///     new double[] { 8, 1 },
    ///     new double[] { 7, 2 },
    /// };
    /// 
    /// 
    /// // To create a tree from a set of points, we use
    /// KDTree&lt;int> tree = KDTree.FromData&lt;int>(points);
    /// 
    /// // Now we can manually navigate the tree
    /// KDTreeNode&lt;int> node = tree.Root.Left.Right;
    /// 
    /// // Or traverse it automatically
    /// foreach (KDTreeNode&lt;int> n in tree)
    /// {
    ///     double[] location = n.Position;
    ///     Assert.AreEqual(2, location.Length);
    /// }
    /// 
    /// // Given a query point, we can also query for other
    /// // points which are near this point within a radius
    /// 
    /// double[] query = new double[] { 5, 3 };
    /// 
    /// // Locate all nearby points within an euclidean distance of 1.5
    /// // (answer should be be a single point located at position (5,4))
    /// KDTreeNodeCollection&lt;int> result = tree.Nearest(query, radius: 1.5); 
    ///             
    /// // We can also use alternate distance functions
    /// tree.Distance = Accord.Math.Distance.Manhattan;
    /// 
    /// // And also query for a fixed number of neighbor points
    /// // (answer should be the points at (5,4), (7,2), (2,3))
    /// KDTreeNodeCollection&lt;int> neighbors = tree.Nearest(query, neighbors: 3);
    /// </code>
    /// <code lang="vb">
    /// ' This is the same example found in Wikipedia page on
    /// ' k-d trees: http://en.wikipedia.org/wiki/K-d_tree
    /// 
    /// ' Suppose we have the following set of points:
    /// 
    /// Dim points =
    /// {
    ///     New Double() { 2, 3 },
    ///     New Double() { 5, 4 },
    ///     New Double() { 9, 6 },
    ///     New Double() { 4, 7 },
    ///     New Double() { 8, 1 },
    ///     New Double() { 7, 2 }
    /// }
    /// 
    /// ' To create a tree from a set of points, we use
    /// Dim tree = KDTree.FromData(Of Integer)(points)
    /// 
    /// ' Now we can manually navigate the tree
    /// Dim node = tree.Root.Left.Right
    /// 
    /// ' Or traverse it automatically
    /// For Each n As KDTreeNode(Of Integer) In tree
    ///     Dim location = n.Position
    ///     Console.WriteLine(location.Length)
    /// Next
    /// 
    /// ' Given a query point, we can also query for other
    /// ' points which are near this point within a radius
    /// '
    /// Dim query = New Double() {5, 3}
    /// 
    /// ' Locate all nearby points within an Euclidean distance of 1.5
    /// ' (answer should be a single point located at position (5,4))
    /// '
    /// Dim result = tree.Nearest(query, radius:=1.5)
    /// 
    /// ' We can also use alternate distance functions
    /// tree.Distance = Function(a, b) Accord.Math.Distance.Manhattan(a, b)
    /// 
    /// ' And also query for a fixed number of neighbor points
    /// ' (answer should be the points at (5,4), (7,2), (2,3))
    /// '
    /// Dim neighbors = tree.Nearest(query, neighbors:=3)
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="Accord.MachineLearning.KNearestNeighbors"/>
    /// 
    [Serializable]
    public class KDTree<T> : KDTreeBase<KDTreeNode<T>>
    {
        /// <summary>
        ///   Creates a new <see cref="KDTree&lt;T&gt;"/>.
        /// </summary>
        /// 
        /// <param name="dimensions">The number of dimensions in the tree.</param>
        /// 
        public KDTree(int dimensions)
            : base(dimensions)
        {
        }

        /// <summary>
        ///   Creates a new <see cref="KDTree&lt;T&gt;"/>.
        /// </summary>
        /// 
        /// <param name="dimension">The number of dimensions in the tree.</param>
        /// <param name="Root">The Root node, if already existent.</param>
        /// 
        public KDTree(int dimension, KDTreeNode<T> Root)
            : base(dimension, Root)
        {
        }

        /// <summary>
        ///   Creates a new <see cref="KDTree&lt;T&gt;"/>.
        /// </summary>
        /// 
        /// <param name="dimension">The number of dimensions in the tree.</param>
        /// <param name="Root">The Root node, if already existent.</param>
        /// <param name="count">The number of elements in the Root node.</param>
        /// <param name="leaves">The number of leaves linked through the Root node.</param>
        /// 
        public KDTree(int dimension, KDTreeNode<T> Root, int count, int leaves)
            : base(dimension, Root, count, leaves)
        {
        }

        /// <summary>
        ///   Inserts a value in the tree at the desired position.
        /// </summary>
        /// 
        /// <param name="position">A double-vector with the same number of elements as dimensions in the tree.</param>
        /// <param name="value">The value to be inserted.</param>
        /// 
        public void Add(double[] position, T value)
        {
            base.AddNode(position).Value = value;
        }

        /// <summary>
        ///   Creates the Root node for a new <see cref="KDTree{T}"/> given
        ///   a set of data points and their respective stored values.
        /// </summary>
        /// 
        /// <param name="points">The data points to be inserted in the tree.</param>
        /// <param name="values">The values associated with each point.</param>
        /// <param name="leaves">Return the number of leaves in the Root subtree.</param>
        /// <param name="inPlace">Whether the given <paramref name="points"/> vector
        ///   can be ordered in place. Passing true will change the original order of
        ///   the vector. If set to false, all operations will be performed on an extra
        ///   copy of the vector.</param>
        /// 
        /// <returns>The Root node for a new <see cref="KDTree{T}"/>
        ///   contained the given <paramref name="points"/>.</returns>
        /// 
        internal static KDTreeNode<T> CreateRoot(double[][] points, T[] values, bool inPlace, out int leaves)
        {
            // Initial argument checks for creating the tree
            if (points == null)
                throw new ArgumentNullException("points");

            if (values != null && points.Length != values.Length)
                throw new DimensionMismatchException("values");

            if (!inPlace)
            {
                points = (double[][])points.Clone();

                if (values != null)
                    values = (T[])values.Clone();
            }

            leaves = 0;

            int dimensions = points[0].Length;

            // Create a comparer to compare individual array
            // elements at specified positions when sorting
            ElementComparer comparer = new ElementComparer();

            // Call the recursive algorithm to operate on the whole array (from 0 to points.Length)
            KDTreeNode<T> Root = create(points, values, 0, dimensions, 0, points.Length, comparer, ref leaves);

            // Create and return the newly formed tree
            return Root;
        }

        private static KDTreeNode<T> create(double[][] points, T[] values,
         int depth, int k, int start, int length, ElementComparer comparer, ref int leaves)
        {
            if (length <= 0)
                return null;

            // We will be doing sorting in place
            int axis = comparer.Index = depth % k;
            Array.Sort(points, values, start, length, comparer);

            // Middle of the input section
            int half = start + length / 2;

            // Start and end of the left branch
            int leftStart = start;
            int leftLength = half - start;

            // Start and end of the right branch
            int rightStart = half + 1;
            int rightLength = length - length / 2 - 1;

            // The median will be located halfway in the sorted array
            var median = points[half];
            var value = values != null ? values[half] : default(T);

            depth++;

            // Continue with the recursion, passing the appropriate left and right array sections
            var left = create(points, values, depth, k, leftStart, leftLength, comparer, ref leaves);
            var right = create(points, values, depth, k, rightStart, rightLength, comparer, ref leaves);

            if (left == null && right == null)
                leaves++;

            // Backtrack and create
            return new KDTreeNode<T>()
            {
                Axis = axis,
                Position = median,
                Value = value,
                Left = left,
                Right = right,
            };
        }
    }




  
}
