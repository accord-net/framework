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
// BH T-SNE code from https://github.com/lvdmaaten/bhtsne/blob/master/sptree.h The 
// original license text is listed below:
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
    using Accord.Math;

    /// <summary>
    ///   Space-Partitioning Tree.
    /// </summary>
    /// 
    [Serializable]
    public class SPTree : Tree<SPTreeNode>
    {
        private int dimension;

        /// <summary>
        ///   Gets the dimension of the space covered by this tree.
        /// </summary>
        /// 
        public int Dimension
        {
            get { return dimension; }
        }

        

        /// <summary>
        /// Initializes a new instance of the <see cref="SPTree"/> class.
        /// </summary>
        /// 
        /// <param name="dimensions">The dimensions of the space partitioned by the tree.</param>
        /// 
        public SPTree(int dimensions)
        {
            this.dimension = dimensions;
        }


        /// <summary>
        ///   Creates a new space-partitioning tree from the given points.
        /// </summary>
        /// 
        /// <param name="points">The points to be added to the tree.</param>
        /// 
        /// <returns>A <see cref="SPTree"/> populated with the given data points.</returns>
        /// 
        public static SPTree FromData(double[][] points)
        {
            int D = points.Columns();
            int N = points.Rows();
            var tree = new SPTree(D);

            // Compute mean, width, and height of current map (boundaries of SPTree)
            var mean_Y = new double[D];
            var min_Y = new double[D];
            var max_Y = new double[D];
            for (int d = 0; d < D; d++)
            {
                min_Y[d] = Double.MaxValue;
                max_Y[d] = Double.MinValue;
            }

            for (int n = 0; n < N; n++)
            {
                for (int d = 0; d < D; d++)
                {
                    mean_Y[d] += points[n][d];
                    if (points[n][d] < min_Y[d])
                        min_Y[d] = points[n][d];
                    if (points[n][d] > max_Y[d])
                        max_Y[d] = points[n][d];
                }
            }

            for (int d = 0; d < D; d++)
                mean_Y[d] /= (double)N;

            // Construct SPTree
            var width = new double[D];
            for (int d = 0; d < D; d++)
                width[d] = System.Math.Max(max_Y[d] - mean_Y[d], mean_Y[d] - min_Y[d]) + 1e-5;

            tree.Root = new SPTreeNode(tree, null, 0, mean_Y, width);
            for (int i = 0; i < points.Length; i++)
                tree.Root.Add(points[i]);

            return tree;
        }

        /// <summary>
        ///   Inserts a point in the Space-Partitioning tree.
        /// </summary>
        /// 
        public bool Add(double[] point)
        {
            return Root.Add(point);
        }

        /// <summary>
        ///  Computes non-edge forces using Barnes-Hut algorithm.
        /// </summary>
        /// 
        public void ComputeNonEdgeForces(double[] point, double theta, double[] neg_f, ref double sum_Q)
        {
            Root.ComputeNonEdgeForces(point, theta, neg_f, ref sum_Q);
        }

        /// <summary>
        ///   Computes edge forces.
        /// </summary>
        /// 
        public void ComputeEdgeForces(double[][] points, int[] row_P, int[] col_P, double[] val_P, double[][] pos_f)
        {
            // Loop over all edges in the graph
            double D;
            double[] buff = new double[dimension];
            for (int n = 0; n < points.Length; n++)
            {
                for (int i = row_P[n]; i < row_P[n + 1]; i++)
                {
                    // Compute pairwise distance and Q-value
                    D = 1.0;
                    int j = col_P[i];

                    for (int d = 0; d < dimension; d++)
                        buff[d] = points[n][d] - points[j][d];

                    for (int d = 0; d < dimension; d++)
                        D += buff[d] * buff[d];
                    D = val_P[i] / D;

                    // Sum positive force
                    for (int d = 0; d < dimension; d++)
                        pos_f[n][d] += D * buff[d];
                }
            }
        }

    }
}