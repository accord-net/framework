// AForge Math Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2011
// contacts@aforgenet.com
//

namespace AForge.Math.Geometry
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
        public List<IntPoint> FindHull( List<IntPoint> points )
        {
            // do nothing if there 3 points or less
            if ( points.Count <= 3 )
            {
                return new List<IntPoint>( points );
            }

            List<PointToProcess> pointsToProcess = new List<PointToProcess>( );

            // convert input points to points we can process
            foreach ( IntPoint point in points )
            {
                pointsToProcess.Add( new PointToProcess( point ) );
            }

            // find a point, with lowest X and lowest Y
            int firstCornerIndex = 0;
            PointToProcess firstCorner = pointsToProcess[0];

            for ( int i = 1, n = pointsToProcess.Count; i < n; i++ )
            {
                if ( ( pointsToProcess[i].X < firstCorner.X ) ||
                     ( ( pointsToProcess[i].X == firstCorner.X ) && ( pointsToProcess[i].Y < firstCorner.Y ) ) )
                {
                    firstCorner = pointsToProcess[i];
                    firstCornerIndex = i;
                }
            }

            // remove the just found point
            pointsToProcess.RemoveAt( firstCornerIndex );

            // find K (tangent of line's angle) and distance to the first corner
            for ( int i = 0, n = pointsToProcess.Count; i < n; i++ )
            {
                int dx = pointsToProcess[i].X - firstCorner.X;
                int dy = pointsToProcess[i].Y - firstCorner.Y;

                // don't need square root, since it is not important in our case
                pointsToProcess[i].Distance = dx * dx + dy * dy;
                // tangent of lines angle
                pointsToProcess[i].K = ( dx == 0 ) ? float.PositiveInfinity : (float) dy / dx;
            }

            // sort points by angle and distance
            pointsToProcess.Sort( );

            List<PointToProcess> convexHullTemp = new List<PointToProcess>( );

            // add first corner, which is always on the hull
            convexHullTemp.Add( firstCorner );
            // add another point, which forms a line with lowest slope
            convexHullTemp.Add( pointsToProcess[0] );
            pointsToProcess.RemoveAt( 0 );

            PointToProcess lastPoint = convexHullTemp[1];
            PointToProcess prevPoint = convexHullTemp[0];

            while ( pointsToProcess.Count != 0 )
            {
                PointToProcess newPoint = pointsToProcess[0];

                // skip any point, which has the same slope as the last one or
                // has 0 distance to the first point
                if ( ( newPoint.K == lastPoint.K ) || ( newPoint.Distance == 0 ) )
                {
                    pointsToProcess.RemoveAt( 0 );
                    continue;
                }

                // check if current point is on the left side from two last points
                if ( ( newPoint.X - prevPoint.X ) * ( lastPoint.Y - newPoint.Y ) - ( lastPoint.X - newPoint.X ) * ( newPoint.Y - prevPoint.Y ) < 0 )
                {
                    // add the point to the hull
                    convexHullTemp.Add( newPoint );
                    // and remove it from the list of points to process
                    pointsToProcess.RemoveAt( 0 );

                    prevPoint = lastPoint;
                    lastPoint = newPoint;
                }
                else
                {
                    // remove the last point from the hull
                    convexHullTemp.RemoveAt( convexHullTemp.Count - 1 );

                    lastPoint = prevPoint;
                    prevPoint = convexHullTemp[convexHullTemp.Count - 2];
                }
            }

            // convert points back
            List<IntPoint> convexHull = new List<IntPoint>( );

            foreach ( PointToProcess pt in convexHullTemp )
            {
                convexHull.Add( pt.ToPoint( ) );
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

            public PointToProcess( IntPoint point )
            {
                X = point.X;
                Y = point.Y;

                K = 0;
                Distance = 0;
            }

            public int CompareTo( object obj )
            {
                PointToProcess another = (PointToProcess) obj;

                return ( K < another.K ) ? -1 : ( K > another.K ) ? 1 :
                    ( ( Distance > another.Distance ) ? -1 : ( Distance < another.Distance ) ? 1 : 0 );
            }

            public IntPoint ToPoint( )
            {
                return new IntPoint( X, Y );
            }
        }

    }
}
