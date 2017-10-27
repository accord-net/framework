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
// The code contained in this class was adapted from Laurens van der Maaten excellent 
// BH T-SNE code from https://github.com/lvdmaaten/bhtsne/blob/master/vptree.h. It was 
// originally adopted with minor modifications from Steve Hanov's great tutorial available 
// at http://stevehanov.ca/blog/index.php?id=130. The original license is listed below:
//  
//    Copyright (c) 2014, Laurens van der Maaten (Delft University of Technology)
//    All rights reserved.
//   
//    Redistribution and use in source and binary forms, with or without
//    modification, are permitted provided that the following conditions are met:
//    1. Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//    2. Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//    3. All advertising materials mentioning features or use of this software
//       must display the following acknowledgement:
//       This product includes software developed by the Delft University of Technology.
//    4. Neither the name of the Delft University of Technology nor the names of 
//       its contributors may be used to endorse or promote products derived from 
//       this software without specific prior written permission.
//   
//    THIS SOFTWARE IS PROVIDED BY LAURENS VAN DER MAATEN ''AS IS'' AND ANY EXPRESS
//    OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES 
//    OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO 
//    EVENT SHALL LAURENS VAN DER MAATEN BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
//    SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, 
//    PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR 
//    BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
//    CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING 
//    IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
//    OF SUCH DAMAGE.
//   

namespace Accord.Collections
{
    using System;
    using System.Text;
    using Accord.Math;
    using Accord;
    using Accord.Compat;

    /// <summary>
    ///   Node for a <see cref="SPTree">Space-Partitioning Tree</see>.
    /// </summary>
    /// 
    /// <seealso cref="SPTree"/>
    /// <seealso cref="VPTree"/>
    /// <seealso cref="KDTree"/>
    /// 
    [Serializable]
    public class SPTreeNode : TreeNode<SPTreeNode>
    {
        SPTree owner;
        double inp_data;
        int cum_size;
        SPCell boundary;
        double[] point;
        double[] center_of_mass;
        double[] buff;


        /// <summary>
        ///   Initializes a new instance of the <see cref="SPTreeNode"/> class.
        /// </summary>
        /// 
        /// <param name="owner">The tree that this node belongs to.</param>
        /// <param name="parent">The parent node for this node. Can be null if this node is the root.</param>
        /// <param name="corner">The starting point of the spatial cell.</param>
        /// <param name="width">The widths of the spatial cell.</param>
        /// <param name="index">The index of this node in the children collection of its parent node.</param>
        /// 
        public SPTreeNode(SPTree owner, SPTreeNode parent, int index, double[] corner, double[] width)
            : base(index)
        {
            int D = owner.Dimension;
            this.owner = owner;
            Parent = parent;
            this.Index = index;
            boundary = new SPCell(corner, width);
            center_of_mass = new double[D];
            buff = new double[D];
        }

        /// <summary>
        ///   Gets the position associated with this node.
        /// </summary>
        /// 
        public double[] Position
        {
            get { return point; }
        }

        /// <summary>
        ///   Gets the center of mass of this node.
        /// </summary>
        /// 
        public double[] CenterOfMass 
        {
            get { return center_of_mass; }
        }

        /// <summary>
        ///   Gets or sets the value associated with this node.
        /// </summary>
        /// 
        public double Value
        {
            get { return inp_data; }
            set { inp_data = value; }
        }

        /// <summary>
        ///   Gets or sets the space region delimited by this node.
        /// </summary>
        /// 
        public SPCell Region
        {
            get { return boundary; }
            set { boundary = value; }
        }

        /// <summary>
        ///   Gets whether this node is empty and does
        ///   not contain any points or children.
        /// </summary>
        /// 
        public bool IsEmpty
        {
            get { return cum_size == 0; }
        }

        /// <summary>
        ///   Inserts a point in the Space-Partitioning tree.
        /// </summary>
        /// 
        public bool Add(double[] point)
        {
            // Ignore objects which do not belong in this quad tree
            if (!boundary.Contains(point))
                return false;

            // On-line update of cumulative size and center-of-mass
            cum_size++;
            double mult1 = (double)(cum_size - 1) / (double)cum_size;
            double mult2 = 1.0 / (double)cum_size;
            for (int d = 0; d < center_of_mass.Length; d++)
            {
                center_of_mass[d] *= mult1;
                center_of_mass[d] += mult2 * point[d];
            }

            // If there is space in this quad tree and it is a leaf, add the object here
            if (IsLeaf && this.point == null)
            {
                this.point = point;
                return true;
            }

            // Don't add duplicates for now (this is not very nice)
            if (this.point.IsEqual(point, rtol: 1e-10))
                return true;

            // Otherwise, we need to subdivide the current cell
            if (IsLeaf)
                subdivide();

            // Find out where the point can be inserted
            for (int i = 0; i < Children.Length; i++)
                if (Children[i].Add(point))
                    return true;

            // Otherwise, the point cannot be inserted (this should never happen)
            return false;
        }

        // Create four children which fully divide this cell into four quads of equal area
        void subdivide()
        {
            // Create new children
            int dimension = owner.Dimension;
            int no_children = 2;
            for (int d = 1; d < dimension; d++)
                no_children *= 2;
            Children = new SPTreeNode[no_children];
            var new_corner = new double[dimension];
            var new_width = new double[dimension];
            for (int i = 0; i < Children.Length; i++)
            {
                int div = 1;
                for (int d = 0; d < dimension; d++)
                {
                    new_width[d] = 0.5 * boundary.Width[d];

                    if ((i / div) % 2 == 1)
                        new_corner[d] = boundary.Corner[d] - .5 * boundary.Width[d];
                    else
                        new_corner[d] = boundary.Corner[d] + .5 * boundary.Width[d];
                    div *= 2;
                }

                Children[i] = new SPTreeNode(owner, this, i, new_corner, new_width);
            }

            // Move existing points to correct children
            bool success = false;
            for (int j = 0; j < Children.Length; j++)
            {
                if (!success)
                    success = Children[j].Add(this.point);
            }

            this.point = null;
        }


        /// <summary>
        ///   Checks whether the current tree is correct.
        /// </summary>
        /// 
        public bool IsCorrect()
        {
            if (IsLeaf)
            {
                if (cum_size > 0)
                    if (!boundary.Contains(this.point))
                        return false;
            }
            else
            {
                if (point != null)
                    return false;
                bool correct = true;
                for (int i = 0; i < Children.Length; i++)
                    correct = correct && Children[i].IsCorrect();
                return correct;
            }

            return true;
        }


        /// <summary>
        ///   Gets the current depth of this node (distance from the root).
        /// </summary>
        /// 
        public int GetDepth()
        {
            if (IsLeaf)
                return 1;
            int depth = 0;
            for (int i = 0; i < Children.Length; i++)
                depth = System.Math.Max(depth, Children[i].GetDepth());
            return 1 + depth;
        }

        /// <summary>
        ///   Compute non-edge forces using Barnes-Hut algorithm.
        /// </summary>
        /// 
        public void ComputeNonEdgeForces(double[] point, double theta, double[] neg_f, ref double sum_Q)
        {
            // Make sure that we spend no time on empty nodes or self-interactions
            if (cum_size == 0 || (IsLeaf && this.point == point))
                return;

            // Compute distance between point and center-of-mass
            int dimension = owner.Dimension;
            double D = 0.0;
            for (int d = 0; d < dimension; d++)
                buff[d] = point[d] - center_of_mass[d];
            for (int d = 0; d < dimension; d++)
                D += buff[d] * buff[d];

            // Check whether we can use this node as a "summary"
            double max_width = 0.0;
            double cur_width;
            for (int d = 0; d < dimension; d++)
            {
                cur_width = boundary.Width[d];
                max_width = (max_width > cur_width) ? max_width : cur_width;
            }

            if (IsLeaf || max_width / System.Math.Sqrt(D) < theta)
            {
                // Compute and add t-SNE force between point and current node
                D = 1.0 / (1.0 + D);
                double mult = cum_size * D;
                sum_Q += mult;
                mult *= D;
                for (int d = 0; d < dimension; d++)
                    neg_f[d] += mult * buff[d];
            }
            else
            {
                // Recursively apply Barnes-Hut to children
                for (int i = 0; i < Children.Length; i++)
                    Children[i].ComputeNonEdgeForces(point, theta, neg_f, ref sum_Q);
            }
        }



        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (cum_size == 0)
                return "Empty node";

            var sb = new StringBuilder();

            if (IsLeaf)
            {
                sb.AppendLine(String.Format("Leaf node; data: {0}",
                    point.ToString(OctaveArrayFormatProvider.InvariantCulture)));
            }
            else
            {
                sb.AppendFormat("Center-of-mass: {0}, children: \n",
                    center_of_mass.ToString(OctaveArrayFormatProvider.InvariantCulture));

                for (int i = 0; i < Children.Length; i++)
                    sb.AppendLine(Children[i].ToString());
            }

            return sb.ToString();
        }
    }
}