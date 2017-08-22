// AForge Math Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2011
// contacts@aforgenet.com
//

namespace Accord.Math.Geometry
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Graham scan algorithm for finding convex hull.
    /// </summary>
    /// 
    /// <remarks><para>The class implements
    /// <a href="http://en.wikipedia.org/wiki/Graham_scan">Graham scan</a> algorithm for finding convex hull
    /// of a given set of points.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // generate some random points
    /// Random rand = new Random( );
    /// List&lt;IntPoint&gt; points = new List&lt;IntPoint&gt;( );
    /// 
    /// for ( int i = 0; i &lt; 10; i++ )
    /// {
    ///     points.Add( new IntPoint(
    ///            rand.Next( 200 ) - 100,
    ///            rand.Next( 200 ) - 100 ) );
    /// }
    /// 
    /// // find the convex hull
    /// IConvexHullAlgorithm hullFinder = new GrahamConvexHull( );
    /// List&lt;IntPoint&gt; hull = hullFinder.FindHull( points );
    /// </code>
    /// </remarks>
    /// 
    public class GrahamConvexHull : IConvexHullAlgorithm
    {
        /// <summary>
        /// Find convex hull for the given set of points.
        /// </summary>
        /// 
        /// <param name="points">Set of points to search convex hull for.</param>
        /// 
        /// <returns>Returns set of points, which form a convex hull for the given <paramref name="points"/>.
        /// The first point in the list is the point with lowest X coordinate (and with lowest Y if there are
        /// several points with the same X value). Points are provided in counter clockwise order
        /// (<a href="http://en.wikipedia.org/wiki/Cartesian_coordinate_system">Cartesian
        /// coordinate system</a>).</returns>
        /// 
        public List<IntPoint> FindHull(List<IntPoint> points)
        {
            // do nothing if there 3 points or less
            if (points.Count <= 3)
            {
                return new List<IntPoint>(points);
            }

            // find a point, with lowest X and lowest Y
            int firstCornerIndex = 0;
            IntPoint pointFirstCorner = points[0];

            for (int i = 1, n = points.Count; i < n; i++)
            {
                if ((points[i].X < pointFirstCorner.X) ||
                    ((points[i].X == pointFirstCorner.X) && (points[i].Y < pointFirstCorner.Y)))
                {
                    pointFirstCorner = points[i];
                    firstCornerIndex = i;
                }
            }

            // convert input points to points we can process
            PointToProcess firstCorner = new PointToProcess(pointFirstCorner);
            // Points to process must exclude the first corner that we've already found
            PointToProcess[] arrPointsToProcess = new PointToProcess[points.Count - 1];
            for (int i = 0; i < points.Count - 1; i++)
            {
                IntPoint point = points[i >= firstCornerIndex ? i + 1 : i];
                arrPointsToProcess[i] = new PointToProcess(point);
            }

            // find K (tangent of line's angle) and distance to the first corner
            for (int i = 0, n = arrPointsToProcess.Length; i < n; i++)
            {
                int dx = arrPointsToProcess[i].X - firstCorner.X;
                int dy = arrPointsToProcess[i].Y - firstCorner.Y;

                // don't need square root, since it is not important in our case
                arrPointsToProcess[i].Distance = dx * dx + dy * dy;
                // tangent of lines angle
                arrPointsToProcess[i].K = (dx == 0) ? float.PositiveInfinity : (float) dy / dx;
            }

            // sort points by angle and distance
            Array.Sort(arrPointsToProcess);

            // Convert points to process to a queue. Continually removing the first item of an array list
            //  is highly inefficient
            Queue<PointToProcess> queuePointsToProcess = new Queue<PointToProcess>(arrPointsToProcess);

            LinkedList<PointToProcess> convexHullTemp = new LinkedList<PointToProcess>();

            // add first corner, which is always on the hull
            PointToProcess prevPoint = convexHullTemp.AddLast(firstCorner).Value;
            // add another point, which forms a line with lowest slope
            PointToProcess lastPoint = convexHullTemp.AddLast(queuePointsToProcess.Dequeue()).Value;

            while (queuePointsToProcess.Count != 0)
            {
                PointToProcess newPoint = queuePointsToProcess.Peek();

                // skip any point, which has the same slope as the last one or
                // has 0 distance to the first point
                if ((newPoint.K == lastPoint.K) || (newPoint.Distance == 0))
                {
                    queuePointsToProcess.Dequeue();
                    continue;
                }

                // check if current point is on the left side from two last points
                if ((newPoint.X - prevPoint.X) * (lastPoint.Y - newPoint.Y) - (lastPoint.X - newPoint.X) * (newPoint.Y - prevPoint.Y) < 0)
                {
                    // add the point to the hull
                    convexHullTemp.AddLast(newPoint);
                    // and remove it from the list of points to process
                    queuePointsToProcess.Dequeue();

                    prevPoint = lastPoint;
                    lastPoint = newPoint;
                }
                else
                {
                    // remove the last point from the hull
                    convexHullTemp.RemoveLast();

                    lastPoint = prevPoint;
                    prevPoint = convexHullTemp.Last.Previous.Value;
                }
            }

            // convert points back
            List<IntPoint> convexHull = new List<IntPoint>();

            foreach (PointToProcess pt in convexHullTemp)
            {
                convexHull.Add(pt.ToPoint());
            }

            return convexHull;
        }

        // Internal comparer for sorting points
        private class PointToProcess : IComparable
        {
            public int X;
            public int Y;
            public float K;
            public float Distance;

            public PointToProcess(IntPoint point)
            {
                X = point.X;
                Y = point.Y;

                K = 0;
                Distance = 0;
            }

            public int CompareTo(object obj)
            {
                PointToProcess another = (PointToProcess)obj;

                return (K < another.K) ? -1 : (K > another.K) ? 1 :
                    ((Distance > another.Distance) ? -1 : (Distance < another.Distance) ? 1 : 0);
            }

            public IntPoint ToPoint()
            {
                return new IntPoint(X, Y);
            }
        }

    }
}
