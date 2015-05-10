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
    /// Shape optimizer, which merges points within close distance to each other.
    /// </summary>
    /// 
    /// <remarks><para>This shape optimizing algorithm checks all points of a shape
    /// and merges any two points which are within <see cref="MaxDistanceToMerge">specified distance</see>
    /// to each other. Two close points are replaced by a single point, which has
    /// mean coordinates of the removed points.</para>
    /// 
    /// <para><note>Because of the fact that the algorithm performs points merging
    /// while it goes through a shape, it may merge several points (more than 2) into a
    /// single point, where distance between extreme points may be bigger
    /// than the <see cref="MaxDistanceToMerge">specified limit</see>. For example, suppose
    /// a case with 3 points, where 1st and 2nd points are close enough to be merged, but the
    /// 3rd point is a little bit further. During merging of 1st and 2nd points, it may
    /// happen that the new point with mean coordinates will get closer to the 3rd point,
    /// so they will be merged also on next iteration of the algorithm.</note></para>
    /// 
    /// <para>
    /// For example, the below circle shape comprised of 65 points, can be optimized to 8 points
    /// by setting <see cref="MaxDistanceToMerge"/> to 28.<br />
    /// <img src="img/math/close_points_merging_optimizer.png" width="268" height="238" />
    /// </para>
    /// </remarks>
    /// 
    public class ClosePointsMergingOptimizer : IShapeOptimizer
    {
        private float maxDistanceToMerge = 10;

        /// <summary>
        /// Maximum allowed distance between points, which are merged during optimization, [0, ∞).
        /// </summary>
        /// 
        /// <remarks><para>The property sets maximum allowed distance between two points of
        /// a shape, which are replaced by single point with mean coordinates.</para>
        /// 
        /// <para>Default value is set to <b>10</b>.</para></remarks>
        /// 
        public float MaxDistanceToMerge
        {
            get { return maxDistanceToMerge; }
            set { maxDistanceToMerge = Math.Max( 0, value ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClosePointsMergingOptimizer"/> class.
        /// </summary>
        /// 
        public ClosePointsMergingOptimizer( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClosePointsMergingOptimizer"/> class.
        /// </summary>
        /// 
        /// <param name="maxDistanceToMerge">Maximum allowed distance between points, which are
        /// merged during optimization (see <see cref="MaxDistanceToMerge"/>).</param>
        ///
        public ClosePointsMergingOptimizer( float maxDistanceToMerge )
        {
            this.maxDistanceToMerge = maxDistanceToMerge;
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

            if ( shape.Count <= 3 )
            {
                // do nothing if shape has 3 points or less
                optimizedShape.AddRange( shape );
            }
            else
            {
                float distance = 0;

                // add first point to the new shape
                optimizedShape.Add( shape[0] );
                int pointsInOptimizedHull = 1;

                for ( int i = 1, n = shape.Count; i < n; i++ )
                {
                    distance = optimizedShape[pointsInOptimizedHull - 1].DistanceTo( shape[i] );

                    if ( ( distance <= maxDistanceToMerge ) &&
                         ( pointsInOptimizedHull + ( n - i ) > 3 ) )
                    {
                        // merge
                        optimizedShape[pointsInOptimizedHull - 1] = ( optimizedShape[pointsInOptimizedHull - 1] + shape[i] ) / 2;
                    }
                    else
                    {
                        optimizedShape.Add( shape[i] );
                        pointsInOptimizedHull++;
                    }
                }

                if ( pointsInOptimizedHull > 3 )
                {
                    // check the last and first points
                    distance = optimizedShape[pointsInOptimizedHull - 1].DistanceTo( optimizedShape[0] );

                    if ( distance <= maxDistanceToMerge )
                    {
                        // merge
                        optimizedShape[0] = ( optimizedShape[pointsInOptimizedHull - 1] + optimizedShape[0] ) / 2;
                        optimizedShape.RemoveAt( pointsInOptimizedHull - 1 );
                    }
                }
            }

            return optimizedShape;
        }
    }
}
