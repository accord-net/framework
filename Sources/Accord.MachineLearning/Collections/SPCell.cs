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
    using Accord.Math;
    using System;
    using Accord.Compat;

    /// <summary>
    ///   Region of space in a Space-Partitioning Tree. Represents an axis-aligned 
    ///   bounding box stored as a center with half-dimensions to represent the boundaries 
    ///   of this quad tree.
    /// </summary>
    /// 
    [Serializable]
    public class SPCell
    {
        private double[] corner;
        private double[] width;

        /// <summary>
        ///   Gets the dimensions of the space delimited
        ///   by this spatial cell.
        /// </summary>
        /// 
        public int Dimension
        {
            get { return corner.Length; }
        }

        /// <summary>
        ///   Gets or sets the starting point of this spatial cell.
        /// </summary>
        /// 
        public double[] Corner
        {
            get { return corner; }
            set { corner = value; }
        }

        /// <summary>
        ///   Gets or sets the width of this spatial cell.
        /// </summary>
        /// 
        public double[] Width
        {
            get { return width; }
            set { width = value; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SPCell"/> class.
        /// </summary>
        /// 
        /// <param name="dimension">The number of dimensions of the space.</param>
        /// 
        public SPCell(int dimension)
        {
            corner = new double[dimension];
            width = new double[dimension];
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SPCell"/> class.
        /// </summary>
        /// 
        /// <param name="corner">The starting point of this spatial cell.</param>
        /// <param name="width">The widths of this spatial cell.</param>
        /// 
        public SPCell(double[] corner, double[] width)
        {
            this.corner = corner.MemberwiseClone();
            this.width = width.MemberwiseClone();
        }

        /// <summary>
        ///   Determines whether a point lies inside this cell.
        /// </summary>
        /// 
        /// <param name="point">The point.</param>
        /// 
        /// <returns>True if the point is contained inside this cell; otherwise, false.</returns>
        /// 
        public bool Contains(double[] point)
        {
            for (int d = 0; d < corner.Length; d++)
            {
                if (corner[d] - width[d] > point[d])
                    return false;

                if (corner[d] + width[d] < point[d])
                    return false;
            }

            return true;
        }
    }
}