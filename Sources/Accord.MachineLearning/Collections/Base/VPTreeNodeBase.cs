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
#if !MONO
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Collections;
    using System.Text;
    using Accord.Compat;

    /// <summary>
    ///   Base class for <see cref="VPTree"/> nodes.
    /// </summary>
    /// 
    /// <typeparam name="TPoint">The type for the position vector (e.g. double[]).</typeparam>
    /// <typeparam name="TNode">The class type for the nodes of the tree.</typeparam>
    /// 
    [Serializable]
    public class VPTreeNodeBase<TPoint, TNode> : BinaryNode<TNode>, IEquatable<TNode> // TODO: Try to remove IEquatable
        where TNode : VPTreeNodeBase<TPoint, TNode>
    {
        private TPoint position;
        private double threshold;  // radius(?)

        /// <summary>
        ///   Gets or sets the current position for this Vantage-Point Tree Node.
        /// </summary>
        /// 
        public TPoint Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        ///   Gets or sets the threshold radius for this node.
        /// </summary>
        /// 
        public double Threshold
        {
            get { return threshold; }
            set { threshold = value; }
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
            return Object.ReferenceEquals(Position, other.Position);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[(");
            var list = Position as IList;
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    sb.Append(list[i]);
                    if (i < list.Count - 1)
                        sb.Append(",");
                }
            }
            else
            {
                sb.Append(Position);
            }

            sb.AppendFormat("), {0}]", threshold);
            return sb.ToString();
        }
    }
#endif
}