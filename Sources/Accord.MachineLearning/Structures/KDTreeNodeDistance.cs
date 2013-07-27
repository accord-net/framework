// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2013
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
    ///   K-d tree node-distance pair.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the value being stored, if any.</typeparam>
    /// 
    [Serializable]
    public class KDTreeNodeDistance<T>
    {
        private KDTreeNode<T> node;
        private double distance;

        /// <summary>
        ///   Gets the node in this pair.
        /// </summary>
        /// 
        public KDTreeNode<T> Node
        {
            get { return node; }
        }

        /// <summary>
        ///   Gets the distance of the node from the query point.
        /// </summary>
        /// 
        public double Distance
        {
            get { return distance; }
        }

        /// <summary>
        ///   Creates a new <see cref="KDTreeNodeDistance&lt;T&gt;"/>.
        /// </summary>
        /// 
        /// <param name="node">The node value.</param>
        /// <param name="distance">The distance value.</param>
        /// 
        public KDTreeNodeDistance(KDTreeNode<T> node, double distance)
        {
            this.node = node;
            this.distance = distance;
        }

        /// <summary>
        ///   Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// 
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// 
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// 
        public override bool Equals(object obj)
        {
            if (obj is KDTreeNodeDistance<T>)
            {
                var b = (KDTreeNodeDistance<T>)obj;
                return this.node == b.node && this.distance == b.distance;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        ///   Returns a hash code for this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        /// 
        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + node.GetHashCode();
            hash = hash * 23 + distance.GetHashCode();
            return hash;
        }

        /// <summary>
        ///   Implements the equality operator.
        /// </summary>
        /// 
        public static bool operator==(KDTreeNodeDistance<T> a, KDTreeNodeDistance<T> b)
        {
            return a.node == b.node && a.Distance == b.distance;
        }

        /// <summary>
        ///   Implements the inequality operator.
        /// </summary>
        /// 
        public static bool operator !=(KDTreeNodeDistance<T> a, KDTreeNodeDistance<T> b)
        {
            return !(a == b);
        }
        
    }
}
