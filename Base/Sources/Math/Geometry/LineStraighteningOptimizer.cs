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
    using AForge;

    /// <summary>
    /// Shape optimizer, which removes points within close range to shapes' body.
    /// </summary>
    /// 
    /// <remarks><para>This shape optimizing algorithm checks all points of the shape and
    /// removes those of them, which are in a certain distance to a line connecting previous and
    /// the next points. In other words, it goes through all adjacent edges of a shape and checks
    /// what is the distance between the corner formed by these two edges and a possible edge, which
    /// could be used as substitution of these edges. If the distance is equal or smaller than
    /// the <see cref="MaxDistanceToRemove">specified value</see>, then the point is removed,
    /// so the two edges are substituted by a single one. When optimization process is done,
    /// the new shape has reduced amount of points and none of the removed points are further away
    /// from the new shape than the specified limit.</para>
    /// 
    /// <para>The shape optimizer does not optimize shapes to less than 3 points, so optimized
    /// shape always will have at least 3 points.</para>
    ///
    /// <para>
    /// For example, the below circle shape comprised of 65 points, can be optimized to 8 points
    /// by setting <see cref="MaxDistanceToRemove"/> to 10.<br />
    /// <img src="img/math/line_straightening_optimizer.png" width="268" height="238" />
    /// </para>
    /// </remarks>
    /// 
    public class LineStraighteningOptimizer : IShapeOptimizer
    {
        private float maxDistanceToRemove = 5;

        /// <summary>
        /// Maximum allowed distance between removed points and optimized shape, [0, ∞).
        /// </summary>
        /// 
        /// <remarks><para>The property sets maximum allowed distance between points removed from original
        /// shape and optimized shape - none of the removed points are further away
        /// from the new shape than the specified limit.
        /// </para>
        /// 
        /// <para>Default value is set to <b>5</b>.</para></remarks>
        /// 
        public float MaxDistanceToRemove
        {
            get { return maxDistanceToRemove; }
            set { maxDistanceToRemove = Math.Max( 0, value ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineStraighteningOptimizer"/> class.
        /// </summary>
        /// 
        public LineStraighteningOptimizer( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineStraighteningOptimizer"/> class.
        /// </summary>
        /// 
        /// <param name="maxDistanceToRemove">Maximum allowed distance between removed points
        /// and optimized shape (see <see cref="MaxDistanceToRemove"/>).</param>
        /// 
        public LineStraighteningOptimizer( float maxDistanceToRemove )
        {
            this.maxDistanceToRemove = maxDistanceToRemove;
        }

        /// <summary>
        /// Optimize specified shape.
        /// </summary>
        /// 
        /// <param name="shape">Shape to be optimized.</param>
        /// 
        /// <returns>Returns final optimized shape, which may have reduced amount of points.</returns>
        /// 
        public List<IntPoint> OptimizeShape( List<IntPoint> shape )
        {
            // optimized shape
            List<IntPoint> optimizedShape = new List<IntPoint>( );
            // list of recently removed points
            List<IntPoint> recentlyRemovedPoints = new List<IntPoint>( );

            if ( shape.Count <= 3 )
            {
                // do nothing if shape has 3 points or less
                optimizedShape.AddRange( shape );
            }
            else
            {
                float distance = 0;

                // add first 2 points to the new shape
                optimizedShape.Add( shape[0] );
                optimizedShape.Add( shape[1] );
                int pointsInOptimizedHull = 2;

                for ( int i = 2, n = shape.Count; i < n; i++ )
                {
                    // add new point
                    optimizedShape.Add( shape[i] );
                    pointsInOptimizedHull++;

                    // add new candidate for removing to the list
                    recentlyRemovedPoints.Add( optimizedShape[pointsInOptimizedHull - 2] );

                    // calculate maximum distance between new candidate line and recently removed point
                    PointsCloud.GetFurthestPointFromLine( recentlyRemovedPoints,
                        optimizedShape[pointsInOptimizedHull - 3], optimizedShape[pointsInOptimizedHull - 1],
                        out distance );

                    if ( ( distance <= maxDistanceToRemove ) &&
                         ( ( pointsInOptimizedHull > 3 ) || ( i < n - 1 ) ) )
                    {
                        optimizedShape.RemoveAt( pointsInOptimizedHull - 2 );
                        pointsInOptimizedHull--;
                    }
                    else
                    {
                        // don't need to remove the last candidate point
                        recentlyRemovedPoints.Clear( );
                    }
                }

                if ( pointsInOptimizedHull > 3 )
                {
                    // check the last point
                    recentlyRemovedPoints.Add( optimizedShape[pointsInOptimizedHull - 1] );

                    PointsCloud.GetFurthestPointFromLine( recentlyRemovedPoints,
                        optimizedShape[pointsInOptimizedHull - 2], optimizedShape[0],
                        out distance );

                    if ( distance <= maxDistanceToRemove )
                    {
                        optimizedShape.RemoveAt( pointsInOptimizedHull - 1 );
                        pointsInOptimizedHull--;
                    }
                    else
                    {
                        recentlyRemovedPoints.Clear( );
                    }

                    if ( pointsInOptimizedHull > 3 )
                    {
                        // check the first point
                        recentlyRemovedPoints.Add( optimizedShape[0] );

                        PointsCloud.GetFurthestPointFromLine( recentlyRemovedPoints,
                            optimizedShape[pointsInOptimizedHull - 1], optimizedShape[1],
                            out distance );

                        if ( distance <= maxDistanceToRemove )
                        {
                            optimizedShape.RemoveAt( 0 );
                        }
                    }
                }
            }

            return optimizedShape;
        }
    }
}
