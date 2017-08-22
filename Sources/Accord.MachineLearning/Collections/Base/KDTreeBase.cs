// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
// Copyright © Hashem Zawary, 2017
// hashemzawary@gmail.com
// https://www.linkedin.com/in/hashem-zavvari-53b01457
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
    using System.Linq;
    using System.Collections.Generic;
    using Accord.Math;
    using Accord.Math.Comparers;
    using Accord.Math.Distances;
    using Accord.Collections;
    using Accord.Compat;

    /// <summary>
    ///   Base class for K-dimensional trees.
    /// </summary>
    /// 
    /// <typeparam name="TNode">The class type for the nodes of the tree.</typeparam>
    /// 
    [Serializable]
    public class KDTreeBase<TNode> : BinaryTree<TNode>, IEnumerable<TNode>
    where TNode : KDTreeNodeBase<TNode>, IComparable<TNode>, new()
    {

        private int count;
        private int dimensions;
        private int leaves;

        private IMetric<double[]> distance = new Accord.Math.Distances.Euclidean();


        /// <summary>
        ///   Gets the number of dimensions expected
        ///   by the input points of this tree.
        /// </summary>
        ///
        public int Dimensions
        {
            get { return dimensions; }
        }

        /// <summary>
        ///   Gets or set the distance function used to
        ///   measure distances amongst points on this tree
        /// </summary>
        ///
        public IMetric<double[]> Distance
        {
            get { return distance; }
            set { distance = value; }
        }

        /// <summary>
        ///   Gets the number of elements contained in this
        ///   tree. This is also the number of tree nodes.
        /// </summary>
        ///
        public int Count
        {
            get { return count; }
        }

        /// <summary>
        ///   Gets the number of leaves contained in this
        ///   tree. This can be used to calibrate approximate
        ///   nearest searchers.
        /// </summary>
        ///
        public int Leaves
        {
            get { return leaves; }
        }


        /// <summary>
        ///   Creates a new <see cref="KDTree&lt;T&gt;"/>.
        /// </summary>
        ///
        /// <param name="dimensions">The number of dimensions in the tree.</param>
        ///
        public KDTreeBase(int dimensions)
        {
            this.dimensions = dimensions;
        }

        /// <summary>
        ///   Creates a new <see cref="KDTree&lt;T&gt;"/>.
        /// </summary>
        ///
        /// <param name="dimension">The number of dimensions in the tree.</param>
        /// <param name="Root">The Root node, if already existent.</param>
        ///
        public KDTreeBase(int dimension, TNode Root)
            : this(dimension)
        {
            this.Root = Root;

            foreach (var node in this)
            {
                count++;

                if (node.IsLeaf)
                    leaves++;
            }
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
        public KDTreeBase(int dimension, TNode Root, int count, int leaves)
            : this(dimension)
        {
            this.Root = Root;
            this.count = count;
            this.leaves = leaves;
        }




        /// <summary>
        ///   Retrieves the nearest points to a given point within a given radius.
        /// </summary>
        ///
        /// <param name="position">The queried point.</param>
        /// <param name="radius">The search radius.</param>
        /// <param name="maximum">The maximum number of neighbors to retrieve.</param>
        ///
        /// <returns>A list of neighbor points, ordered by distance.</returns>
        ///
        public ICollection<NodeDistance<TNode>> Nearest(double[] position, double radius, int maximum)
        {
            if (maximum == 0)
            {
                var list = new List<NodeDistance<TNode>>();

                if (Root != null)
                    nearest(Root, position, radius, list);

                return list;
            }
            else
            {
                var list = new KDTreeNodeCollection<TNode>(maximum);

                if (Root != null)
                    nearest(Root, position, radius, list);

                return list;
            }
        }

        /// <summary>
        ///   Retrieves the nearest points to a given point within a given radius.
        /// </summary>
        ///
        /// <param name="position">The queried point.</param>
        /// <param name="radius">The search radius.</param>
        ///
        /// <returns>A list of neighbor points, ordered by distance.</returns>
        ///
        public List<NodeDistance<TNode>> Nearest(double[] position, double radius)
        {
            var list = new List<NodeDistance<TNode>>();

            if (Root != null)
                nearest(Root, position, radius, list);

            return list;
        }

        /// <summary>
        ///   Retrieves a fixed number of nearest points to a given point.
        /// </summary>
        ///
        /// <param name="position">The queried point.</param>
        /// <param name="neighbors">The number of neighbors to retrieve.</param>
        ///
        /// <returns>A list of neighbor points, ordered by distance.</returns>
        ///
        public KDTreeNodeCollection<TNode> Nearest(double[] position, int neighbors)
        {
            var list = new KDTreeNodeCollection<TNode>(size: neighbors);

            if (Root != null)
                nearest(Root, position, list);

            return list;
        }

        /// <summary>
        ///   Retrieves the nearest point to a given point.
        /// </summary>
        ///
        /// <param name="position">The queried point.</param>
        ///
        /// <returns>A list of neighbor points, ordered by distance.</returns>
        ///
        public TNode Nearest(double[] position)
        {
            double distance;
            return Nearest(position, out distance);
        }

        /// <summary>
        ///   Retrieves the nearest point to a given point.
        /// </summary>
        ///
        /// <param name="position">The queried point.</param>
        /// <param name="distance">The distance from the <paramref name="position"/>
        ///   to its nearest neighbor found in the tree.</param>
        ///
        /// <returns>A list of neighbor points, ordered by distance.</returns>
        ///
        public TNode Nearest(double[] position, out double distance)
        {
            TNode result = Root;
            distance = Distance.Distance(Root.Position, position);

            nearest(Root, position, ref result, ref distance);

            return result;
        }

        /// <summary>
        ///   Retrieves a fixed percentage of nearest points to a given point.
        /// </summary>
        ///
        /// <param name="position">The queried point.</param>
        /// <param name="neighbors">The number of neighbors to retrieve.</param>
        /// <param name="percentage">The maximum percentage of leaf nodes that
        /// can be visited before the search finishes with an approximate answer.</param>
        ///
        /// <returns>A list of neighbor points, ordered by distance.</returns>
        ///
        public KDTreeNodeCollection<TNode> ApproximateNearest(double[] position, int neighbors, double percentage)
        {
            int maxLeaves = (int)(leaves * percentage);

            var list = new KDTreeNodeCollection<TNode>(size: neighbors);

            if (Root != null)
            {
                int visited = 0;
                approximate(Root, position, list, maxLeaves, ref visited);
            }

            return list;
        }

        /// <summary>
        ///   Retrieves a percentage of nearest points to a given point.
        /// </summary>
        ///
        /// <param name="position">The queried point.</param>
        /// <param name="percentage">The maximum percentage of leaf nodes that
        /// can be visited before the search finishes with an approximate answer.</param>
        /// <param name="distance">The distance between the query point and its nearest neighbor.</param>
        ///
        /// <returns>A list of neighbor points, ordered by distance.</returns>
        ///
        public TNode ApproximateNearest(double[] position, double percentage, out double distance)
        {
            TNode result = Root;
            distance = Distance.Distance(Root.Position, position);

            int maxLeaves = (int)(leaves * percentage);

            int visited = 0;
            approximateNearest(Root, position, ref result, ref distance, maxLeaves, ref visited);

            return result;
        }

        /// <summary>
        ///   Retrieves a percentage of nearest points to a given point.
        /// </summary>
        ///
        /// <param name="position">The queried point.</param>
        /// <param name="percentage">The maximum percentage of leaf nodes that
        /// can be visited before the search finishes with an approximate answer.</param>
        ///
        /// <returns>A list of neighbor points, ordered by distance.</returns>
        ///
        public TNode ApproximateNearest(double[] position, double percentage)
        {
            var list = ApproximateNearest(position, neighbors: 1, percentage: percentage);

            return list.Nearest;
        }

        /// <summary>
        ///   Retrieves a fixed number of nearest points to a given point.
        /// </summary>
        ///
        /// <param name="position">The queried point.</param>
        /// <param name="neighbors">The number of neighbors to retrieve.</param>
        /// <param name="maxLeaves">The maximum number of leaf nodes that can
        /// be visited before the search finishes with an approximate answer.</param>
        ///
        /// <returns>A list of neighbor points, ordered by distance.</returns>
        ///
        public KDTreeNodeCollection<TNode> ApproximateNearest(double[] position, int neighbors, int maxLeaves)
        {
            var list = new KDTreeNodeCollection<TNode>(size: neighbors);

            if (Root != null)
            {
                int visited = 0;
                approximate(Root, position, list, maxLeaves, ref visited);
            }

            return list;
        }

        /// <summary>
        ///   Retrieves a fixed number of nearest points to a given point.
        /// </summary>
        ///
        /// <param name="position">The queried point.</param>
        /// <param name="maxLeaves">The maximum number of leaf nodes that can
        /// be visited before the search finishes with an approximate answer.</param>
        ///
        /// <returns>A list of neighbor points, ordered by distance.</returns>
        ///
        public TNode ApproximateNearest(double[] position, int maxLeaves)
        {
            var list = ApproximateNearest(position, neighbors: 1, maxLeaves: maxLeaves);

            return list.Nearest;
        }

        /// <summary>
        ///   Retrieves a list of all points inside a given region.
        /// </summary>
        /// 
        /// <param name="region">The region.</param>
        /// 
        /// <returns>A list of all nodes contained in the region.</returns>
        /// 
        public IList<TNode> GetNodesInsideRegion(Hyperrectangle region)
        {
            return getNodesInsideRegion(this.Root, region, region);
        }

        private IList<TNode> getNodesInsideRegion(TNode node, Hyperrectangle region, Hyperrectangle subRegion)
        {
            var result = new List<TNode>();

            if (node != null && region.IntersectsWith(subRegion))
            {
                if (region.Contains(node.Position))
                    result.Add(node);

                result.AddRange(getNodesInsideRegion(node.Left, region, leftRect(subRegion, node)));
                result.AddRange(getNodesInsideRegion(node.Right, region, rightRect(subRegion, node)));
            }

            return result;
        }


        // TODO: Optimize the two methods below. It shouldn't be necessary to make copies/clones of these arrays

        private static Hyperrectangle leftRect(Hyperrectangle hyperrect, TNode node)
        {
            //var rect = hyperrect.ToRectangle();
            //return (node.Axis != 0 ?
            //    Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, (int)node.Position[1]) :
            //    Rectangle.FromLTRB(rect.Left, rect.Top, (int)node.Position[0], rect.Bottom)).ToHyperrectangle();
            Hyperrectangle copy = new Hyperrectangle((double[])hyperrect.Min.Clone(), (double[])hyperrect.Max.Clone());
            copy.Max[node.Axis] = node.Position[node.Axis];
            return copy;
        }

        // helper: get the right rectangle of node inside parent's rect
        private static Hyperrectangle rightRect(Hyperrectangle hyperrect, TNode node)
        {
            //var rect = hyperrect.ToRectangle();
            //return (node.Axis != 0 ?
            //    Rectangle.FromLTRB(rect.Left, (int)node.Position[1], rect.Right, rect.Bottom) :
            //    Rectangle.FromLTRB((int)node.Position[0], rect.Top, rect.Right, rect.Bottom)).ToHyperrectangle();
            Hyperrectangle copy = new Hyperrectangle((double[])hyperrect.Min.Clone(), (double[])hyperrect.Max.Clone());
            copy.Min[node.Axis] = node.Position[node.Axis];
            return copy;
        }


        #region internal methods
        /// <summary>
        ///   Creates the Root node for a new <see cref="KDTree{T}"/> given
        ///   a set of data points and their respective stored values.
        /// </summary>
        ///
        /// <param name="points">The data points to be inserted in the tree.</param>
        /// <param name="leaves">Return the number of leaves in the Root subtree.</param>
        /// <param name="inPlace">Whether the given <paramref name="points"/> vector
        ///   can be ordered in place. Passing true will change the original order of
        ///   the vector. If set to false, all operations will be performed on an extra
        ///   copy of the vector.</param>
        ///
        /// <returns>The Root node for a new <see cref="KDTree{T}"/>
        ///   contained the given <paramref name="points"/>.</returns>
        ///
        protected static TNode CreateRoot(double[][] points, bool inPlace, out int leaves)
        {
            // Initial argument checks for creating the tree
            if (points == null)
                throw new ArgumentNullException("points");

            if (!inPlace)
                points = (double[][])points.Clone();

            leaves = 0;

            int dimensions = points[0].Length;

            // Create a comparer to compare individual array
            // elements at specified positions when sorting
            ElementComparer comparer = new ElementComparer();

            // Call the recursive algorithm to operate on the whole array (from 0 to points.Length)
            TNode Root = create(points, 0, dimensions, 0, points.Length, comparer, ref leaves);

            // Create and return the newly formed tree
            return Root;
        }

        private static TNode create(double[][] points,
        int depth, int k, int start, int length, ElementComparer comparer, ref int leaves)
        {
            if (length <= 0)
                return null;

            // We will be doing sorting in place
            int axis = comparer.Index = depth % k;
            Array.Sort(points, start, length, comparer);

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

            depth++;

            // Continue with the recursion, passing the appropriate left and right array sections
            var left = create(points, depth, k, leftStart, leftLength, comparer, ref leaves);
            var right = create(points, depth, k, rightStart, rightLength, comparer, ref leaves);

            if (left == null && right == null)
                leaves++;

            // Backtrack and create
            return new TNode()
            {
                Axis = axis,
                Position = median,
                Left = left,
                Right = right,
            };
        }
        #endregion


        #region Recursive methods

        /// <summary>
        ///   Radius search.
        /// </summary>
        ///
        private void nearest(TNode current, double[] position,
        double radius, ICollection<NodeDistance<TNode>> list)
        {
            // Check if the distance of the point from this
            // node is within the desired radius, and if it
            // is, add to the list of nearest nodes.

            double d = distance.Distance(position, current.Position);

            if (d <= radius)
                list.Add(new NodeDistance<TNode>(current, d));

            // Prepare for recursion. The following null checks
            // will be used to avoid function calls if possible

            double value = position[current.Axis];
            double median = current.Position[current.Axis];
            double u = value - median;

            if (u <= 0)
            {
                if (current.Left != null)
                    nearest(current.Left, position, radius, list);

                if (current.Right != null && Math.Abs(u) <= radius)
                    nearest(current.Right, position, radius, list);
            }
            else
            {
                if (current.Right != null)
                    nearest(current.Right, position, radius, list);

                if (current.Left != null && Math.Abs(u) <= radius)
                    nearest(current.Left, position, radius, list);
            }
        }

        /// <summary>
        ///   k-nearest neighbors search.
        /// </summary>
        ///
        private void nearest(TNode current, double[] position, KDTreeNodeCollection<TNode> list)
        {
            // Compute distance from this node to the point
            double d = distance.Distance(position, current.Position);


            list.Add(current, d);


            // Check for leafs on the opposite sides of
            // the subtrees to nearest possible neighbors.

            // Prepare for recursion. The following null checks
            // will be used to avoid function calls if possible

            double value = position[current.Axis];
            double median = current.Position[current.Axis];
            double u = value - median;

            if (u <= 0)
            {
                if (current.Left != null)
                    nearest(current.Left, position, list);

                if (current.Right != null && Math.Abs(u) <= list.Maximum)
                    nearest(current.Right, position, list);
            }
            else
            {
                if (current.Right != null)
                    nearest(current.Right, position, list);

                if (current.Left != null && Math.Abs(u) <= list.Maximum)
                    nearest(current.Left, position, list);
            }
        }

        private void nearest(TNode current, double[] position, ref TNode match, ref double minDistance)
        {
            // Compute distance from this node to the point
            double d = distance.Distance(position, current.Position);

            if (d < minDistance)
            {
                minDistance = d;
                match = current;
            }

            // Check for leafs on the opposite sides of
            // the subtrees to nearest possible neighbors.

            // Prepare for recursion. The following null checks
            // will be used to avoid function calls if possible

            double value = position[current.Axis];
            double median = current.Position[current.Axis];
            double u = value - median;

            if (u <= 0)
            {
                if (current.Left != null)
                    nearest(current.Left, position, ref match, ref minDistance);

                if (current.Right != null && u <= minDistance)
                    nearest(current.Right, position, ref match, ref minDistance);
            }
            else
            {
                if (current.Right != null)
                    nearest(current.Right, position, ref match, ref minDistance);

                if (current.Left != null && u <= minDistance)
                    nearest(current.Left, position, ref match, ref minDistance);
            }
        }


        private bool approximate(TNode current, double[] position,
        KDTreeNodeCollection<TNode> list, int maxLeaves, ref int visited)
        {
            // Compute distance from this node to the point
            double d = distance.Distance(position, current.Position);

            list.Add(current, d);

            if (++visited > maxLeaves)
                return true;


            // Check for leafs on the opposite sides of
            // the subtrees to nearest possible neighbors.

            // Prepare for recursion. The following null checks
            // will be used to avoid function calls if possible

            double value = position[current.Axis];
            double median = current.Position[current.Axis];
            double u = value - median;

            if (u <= 0)
            {
                if (current.Left != null)
                    if (approximate(current.Left, position, list, maxLeaves, ref visited))
                        return true;

                if (current.Right != null && Math.Abs(u) <= list.Maximum)
                    if (approximate(current.Right, position, list, maxLeaves, ref visited))
                        return true;
            }
            else
            {
                if (current.Right != null)
                    approximate(current.Right, position, list, maxLeaves, ref visited);

                if (current.Left != null && Math.Abs(u) <= list.Maximum)
                    if (approximate(current.Left, position, list, maxLeaves, ref visited))
                        return true;
            }

            return false;
        }

        private bool approximateNearest(TNode current, double[] position,
        ref TNode match, ref double minDistance, int maxLeaves, ref int visited)
        {
            // Compute distance from this node to the point
            double d = distance.Distance(position, current.Position);

            // Base: node is leaf
            if (d < minDistance)
            {
                minDistance = d;
                match = current;
            }

            if (++visited > maxLeaves)
                return true;


            // Check for leafs on the opposite sides of
            // the subtrees to nearest possible neighbors.

            // Prepare for recursion. The following null checks
            // will be used to avoid function calls if possible

            double value = position[current.Axis];
            double median = current.Position[current.Axis];
            double u = value - median;

            if (u <= 0)
            {
                if (current.Left != null)
                    if (approximateNearest(current.Left, position, ref match, ref minDistance, maxLeaves, ref visited))
                        return true;

                if (current.Right != null && Math.Abs(u) <= minDistance)
                    if (approximateNearest(current.Right, position, ref match, ref minDistance, maxLeaves, ref visited))
                        return true;
            }
            else
            {
                if (current.Right != null)
                    approximateNearest(current.Right, position,
                    ref match, ref minDistance, maxLeaves, ref visited);

                if (current.Left != null && Math.Abs(u) <= minDistance)
                    if (approximateNearest(current.Left, position, ref match, ref minDistance, maxLeaves, ref visited))
                        return true;
            }

            return false;
        }

        /// <summary>
        ///   Inserts a value into the tree at the desired position.
        /// </summary>
        ///
        /// <param name="position">A double-vector with the same number of elements as dimensions in the tree.</param>
        ///
        protected TNode AddNode(double[] position)
        {
            count++;
            var root = Root;
            TNode node = insert(ref root, position, 0);
            Root = root;
            return node;
        }

        private TNode insert(ref TNode node, double[] position, int depth)
        {
            if (node == null)
            {
                // Base case: node is null
                return node = new TNode()
                {
                    Axis = depth % dimensions,
                    Position = position,
                };
            }
            else
            {
                TNode newNode;

                // Recursive case: keep looking for a position to insert
                if (position[node.Axis] < node.Position[node.Axis])
                {
                    TNode child = node.Left;
                    newNode = insert(ref child, position, depth + 1);
                    node.Left = child;
                }
                else
                {
                    TNode child = node.Right;
                    newNode = insert(ref child, position, depth + 1);
                    node.Right = child;
                }

                return newNode;
            }
        }



        #endregion


        /// <summary>
        ///   Removes all nodes from this tree.
        /// </summary>
        ///
        public void Clear()
        {
            this.Root = null;
        }

        /// <summary>
        ///   Copies the entire tree to a compatible one-dimensional <see cref="System.Array"/>, starting
        ///   at the specified <paramref name="arrayIndex">index</paramref> of the <paramref name="array">
        ///   target array</paramref>.
        /// </summary>
        ///
        /// <param name="array">The one-dimensional <see cref="System.Array"/> that is the destination of the
        ///    elements copied from tree. The <see cref="System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        ///
        public void CopyTo(TNode[] array, int arrayIndex)
        {
            foreach (var node in this)
            {
                array[arrayIndex++] = node;
            }
        }


    }
}