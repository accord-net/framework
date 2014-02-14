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
    using AForge;
    using AForge.Math.Geometry;

    /// <summary>
    ///   Convex Hull Defects Extractor.
    /// </summary>
    /// 
    public class ConvexHullDefects
    {

        /// <summary>
        /// Gets or sets the minimum depth which characterizes a convexity defect.
        /// </summary>
        /// 
        /// <value>The minimum depth.</value>
        /// 
        public double MinimumDepth { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ConvexHullDefects"/> class.
        /// </summary>
        /// 
        /// <param name="minDepth">The minimum depth which characterizes a convexity defect.</param>
        /// 
        public ConvexHullDefects(double minDepth)
        {
            this.MinimumDepth = minDepth;
        }

        /// <summary>
        ///   Finds the convexity defects in a contour given a convex hull.
        /// </summary>
        /// 
        /// <param name="contour">The contour.</param>
        /// <param name="convexHull">The convex hull of the contour.</param>
        /// <returns>A list of <see cref="ConvexityDefect"/>s containing each of the
        /// defects found considering the convex hull of the contour.</returns>
        /// 
        public List<ConvexityDefect> FindDefects(List<IntPoint> contour, List<IntPoint> convexHull)
        {
            if (contour.Count < 4)
                throw new ArgumentException("Point sequence size should have at least 4 points.");

            if (convexHull.Count < 3)
                throw new ArgumentException("Convex hull must have at least 3 points.");


            // Find all convex hull points in the contour
            int[] indexes = new int[convexHull.Count];
            for (int i = 0, j = 0; i < contour.Count; i++)
            {
                if (convexHull.Contains(contour[i]))
                {
                    indexes[j++] = i;
                }
            }


            List<ConvexityDefect> defects = new List<ConvexityDefect>();

            // For each two consecutive points in the convex hull
            for (int i = 0; i < indexes.Length - 1; i++)
            {
                ConvexityDefect current = extractDefect(contour, indexes[i], indexes[i + 1]);

                if (current.Depth > MinimumDepth)
                {
                    defects.Add(current);
                }
            }


            return defects;
        }

        private static ConvexityDefect extractDefect(List<IntPoint> contour, int startIndex, int endIndex)
        {
            // Navigate the contour until the next point of the convex hull,
            //  taking note of the distance between the current contour point
            //  and the line connecting the two consecutive convex hull points

            IntPoint start = contour[startIndex];
            IntPoint end = contour[endIndex];
            Line line = Line.FromPoints(start, end);

            double maxDepth = 0;
            int maxIndex = 0;

            for (int i = startIndex; i < endIndex; i++)
            {
                double d = line.DistanceToPoint(contour[i]);

                if (d > maxDepth)
                {
                    maxDepth = d;
                    maxIndex = i;
                }
            }

            return new ConvexityDefect(maxIndex, startIndex, endIndex, maxDepth);
        }

    }

 

}
