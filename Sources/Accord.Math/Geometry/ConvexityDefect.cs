// Accord Math Library
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

namespace Accord.Math.Geometry
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    ///   Convexity defect.
    /// </summary>
    /// 
    public class ConvexityDefect
    {

        /// <summary>
        ///   Initializes a new instance of the <see cref="ConvexityDefect"/> class.
        /// </summary>
        /// 
        /// <param name="point">The most distant point from the hull.</param>
        /// <param name="start">The starting index of the defect in the contour.</param>
        /// <param name="end">The ending index of the defect in the contour.</param>
        /// <param name="depth">The depth of the defect (highest distance from the hull to
        /// any of the contour points).</param>
        /// 
        public ConvexityDefect(int point, int start, int end, double depth)
        {
            this.Point = point;
            this.Start = start;
            this.End = end;
            this.Depth = depth;
        }

        /// <summary>
        ///   Gets or sets the starting index of the defect in the contour.
        /// </summary>
        /// 
        public int Start { get; set; }

        /// <summary>
        ///   Gets or sets the ending index of the defect in the contour.
        /// </summary>
        /// 
        public int End { get; set; }

        /// <summary>
        ///   Gets or sets the most distant point from the hull characterizing the defect.
        /// </summary>
        /// 
        /// <value>The point.</value>
        /// 
        public int Point { get; set; }

        /// <summary>
        ///   Gets or sets the depth of the defect (highest distance
        ///   from the hull to any of the points in the contour).
        /// </summary>
        /// 
        public double Depth { get; set; }

    }
}
