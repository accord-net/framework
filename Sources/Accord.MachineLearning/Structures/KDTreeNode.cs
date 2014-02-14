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
    using System.Text;

    /// <summary>
    ///   K-dimensional tree node.
    /// </summary>
    /// 
    /// <remarks>
    ///   This class provides a shorthand notation for 
    ///   the actual <see cref="KDTreeNode{T}"/> type.
    /// </remarks>
    /// 
    [Serializable]
    public class KDTreeNode : KDTreeNode<Object>
    {
      
    }

    /// <summary>
    ///   K-dimensional tree node.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the value being stored.</typeparam>
    /// 
    [Serializable]
    public class KDTreeNode<T>
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
        ///   Gets or sets the left subtree of this node.
        /// </summary>
        /// 
        public KDTreeNode<T> Left { get; set; }

        /// <summary>
        ///   Gets or sets the right subtree of this node.
        /// </summary>
        /// 
        public KDTreeNode<T> Right { get; set; }

        /// <summary>
        ///   Gets or sets the value being stored at this node.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        ///   Gets whether this node is a leaf (has no children).
        /// </summary>
        /// 
        public bool IsLeaf
        {
            get { return Left == null && Right == null; }
        }

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
    }
}
