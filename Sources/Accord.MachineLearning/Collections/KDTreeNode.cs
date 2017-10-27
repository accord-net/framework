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
    using System.Text;
    using Accord.Compat;

    /// <summary>
    ///   K-dimensional tree node (for <see cref="KDTree"/>).
    /// </summary>
    /// 
    /// <seealso cref="SPTreeNode"/>
    /// <seealso cref="VPTreeNode{TPoint}"/>
    /// 
    [Serializable]
    public class KDTreeNode : KDTreeNodeBase<KDTreeNode>
    {
    }

    /// <summary>
    ///   K-dimensional tree node (for <see cref="KDTree{T}"/>).
    /// </summary>
    /// 
    /// <seealso cref="SPTreeNode"/>
    /// <seealso cref="VPTreeNode{TPoint}"/>
    /// 
    [Serializable]
    public class KDTreeNode<T> : KDTreeNodeBase<KDTreeNode<T>>
    {
        /// <summary>
        ///   Gets or sets the value being stored at this node.
        /// </summary>
        /// 
        public T Value { get; set; }
    }

    /// <summary>
    ///   Base class for K-dimensional tree nodes.
    /// </summary>
    /// 
    /// <typeparam name="TNode">The class type for the nodes of the tree.</typeparam>
    /// 
    /// <seealso cref="KDTreeNode"/>
    /// <seealso cref="KDTreeNode{T}"/>
    /// <seealso cref="BinaryNode{TNode}"/>
    /// 
    [Serializable]
    public class KDTreeNodeBase<TNode> : BinaryNode<TNode>, 
        IComparable<TNode>, IEquatable<TNode> // TODO: Try to remove IEquatable
        where TNode : KDTreeNodeBase<TNode>
    {
        /// <summary>
        ///   Gets or sets the position of 
        ///   the node in spatial coordinates.
        /// </summary>
        /// 
        public double[] Position { get; set; }

        /// <summary>
        ///   Gets or sets the dimension index of the split. This value is a
        ///   index of the <see cref="Position"/> vector and as such should
        ///   be higher than zero and less than the number of elements in <see cref="Position"/>.
        /// </summary>
        /// 
        public int Axis { get; set; }

        /// <summary>
        ///   Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public override string ToString()
        {
            if (Position == null)
                return "(null)";

            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            for (int i = 0; i < Position.Length; i++)
            {
                sb.Append(Position[i]);
                if (i < Position.Length - 1)
                    sb.Append(",");
            }
            sb.Append(")");

            return sb.ToString();
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This object is equal to <paramref name="other" />. Greater than zero This object is greater than <paramref name="other" />.
        /// </returns>
        public int CompareTo(TNode other)
        {
            return this.Position[this.Axis].CompareTo(other.Position[other.Axis]);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public new bool Equals(TNode other) // TODO: Try to remove IEquatable
        {
            return this == other;
        }

    }
}
