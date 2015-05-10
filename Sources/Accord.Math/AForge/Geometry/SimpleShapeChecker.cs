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
    /// A class for checking simple geometrical shapes.
    /// </summary>
    /// 
    /// <remarks><para>The class performs checking/detection of some simple geometrical
    /// shapes for provided set of points (shape's edge points). During the check
    /// the class goes through the list of all provided points and checks how accurately
    /// they fit into assumed shape.</para>
    /// 
    /// <para>All the shape checks allow some deviation of
    /// points from the shape with assumed parameters. In other words it is allowed
    /// that specified set of points may form a little bit distorted shape, which may be
    /// still recognized. The allowed amount of distortion is controlled by two
    /// properties (<see cref="MinAcceptableDistortion"/> and <see cref="RelativeDistortionLimit"/>),
    /// which allow higher distortion level for bigger shapes and smaller amount of
    /// distortion for smaller shapes. Checking specified set of points, the class
    /// calculates mean distance between specified set of points and edge of the assumed
    /// shape. If the mean distance is equal to or less than maximum allowed distance,
    /// then a shape is recognized. The maximum allowed distance is calculated as:
    /// <code lang="none">
    /// maxDistance = max( minAcceptableDistortion, relativeDistortionLimit * ( width + height ) / 2 )
    /// </code>
    /// , where <b>width</b> and <b>height</b> is the size of bounding rectangle for the
    /// specified points.
    /// </para>
    /// 
    /// <para>See also <see cref="AngleError"/> and <see cref="LengthError"/> properties,
    /// which set acceptable errors for polygon sub type checking done by
    /// <see cref="CheckPolygonSubType"/> method.</para>
    /// 
    /// <para><note>See the next article for details about the implemented algorithms:
    /// <a href="http://www.aforgenet.com/articles/shape_checker/">Detecting some simple shapes in images</a>.
    /// </note></para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// private List&lt;IntPoint&gt; idealCicle = new List&lt;IntPoint&gt;( );
    /// private List&lt;IntPoint&gt; distorredCircle = new List&lt;IntPoint&gt;( );
    /// System.Random rand = new System.Random( );
    ///
    /// // generate sample circles
    /// float radius = 100;
    ///
    /// for ( int i = 0; i &lt; 360; i += 10 )
    /// {
    ///     float angle = (float) ( (float) i / 180 * System.Math.PI );
    /// 
    ///     // add point to ideal circle
    ///     idealCicle.Add( new IntPoint(
    ///         (int) ( radius * System.Math.Cos( angle ) ),
    ///         (int) ( radius * System.Math.Sin( angle ) ) ) );
    /// 
    ///     // add a bit distortion for distorred cirlce
    ///     float distorredRadius = radius + rand.Next( 7 ) - 3;
    /// 
    ///     distorredCircle.Add( new IntPoint(
    ///         (int) ( distorredRadius * System.Math.Cos( angle ) ),
    ///         (int) ( distorredRadius * System.Math.Sin( angle ) ) ) );
    /// }
    /// 
    /// // check shape
    /// SimpleShapeChecker shapeChecker = new SimpleShapeChecker( );
    /// 
    /// if ( shapeChecker.IsCircle( idealCicle ) )
    /// {
    ///     // ...
    /// }
    /// 
    /// if ( shapeChecker.CheckShapeType( distorredCircle ) == ShapeType.Circle )
    /// {
    ///     // ...
    /// }
    /// </code>
    /// </remarks>
    /// 
    public class SimpleShapeChecker
    {
        private FlatAnglesOptimizer shapeOptimizer = new FlatAnglesOptimizer( 160 );

        private float minAcceptableDistortion = 0.5f;
        private float relativeDistortionLimit = 0.03f;

        private float angleError = 7;
        private float lengthError = 0.1f;

        /// <summary>
        /// Minimum value of allowed shapes' distortion.
        /// </summary>
        /// 
        /// <remarks><para>The property sets minimum value for allowed shapes'
        /// distortion (in pixels). See documentation to <see cref="SimpleShapeChecker"/>
        /// class for more details about this property.</para>
        /// 
        /// <para>Default value is set to <b>0.5</b>.</para>
        /// </remarks>
        /// 
        public float MinAcceptableDistortion
        {
            get { return minAcceptableDistortion; }
            set { minAcceptableDistortion = Math.Max( 0, value ); }
        }

        /// <summary>
        /// Maximum value of allowed shapes' distortion, [0, 1].
        /// </summary>
        /// 
        /// <remarks><para>The property sets maximum value for allowed shapes'
        /// distortion. The value is measured in [0, 1] range, which corresponds
        /// to [0%, 100%] range, which means that maximum allowed shapes'
        /// distortion is calculated relatively to shape's size. This results to
        /// higher allowed distortion level for bigger shapes and smaller allowed
        /// distortion for smaller shapers. See documentation to <see cref="SimpleShapeChecker"/>
        /// class for more details about this property.</para>
        /// 
        /// <para>Default value is set to <b>0.03</b> (3%).</para>
        /// </remarks>
        /// 
        public float RelativeDistortionLimit
        {
            get { return relativeDistortionLimit; }
            set { relativeDistortionLimit = Math.Max( 0, Math.Min( 1, value ) ); }
        }

        /// <summary>
        /// Maximum allowed angle error in degrees, [0, 20].
        /// </summary>
        /// 
        /// <remarks><para>The value sets maximum allowed difference between two angles to
        /// treat them as equal. It is used by <see cref="CheckPolygonSubType"/> method to
        /// check for parallel lines and angles of triangles and quadrilaterals.
        /// For example, if angle between two lines equals 5 degrees and this properties value
        /// is set to 7, then two compared lines are treated as parallel.</para>
        /// 
        /// <para>Default value is set to <b>7</b>.</para>
        /// </remarks>
        /// 
        public float AngleError
        {
            get { return angleError; }
            set { angleError = Math.Max( 0, Math.Min( 20, value ) ); }
        }

        /// <summary>
        /// Maximum allowed difference in sides' length (relative to shapes' size), [0, 1].
        /// </summary>
        ///
        /// <remarks><para>The values sets maximum allowed difference between two sides' length
        /// to treat them as equal. The error value is set relative to shapes size and measured
        /// in [0, 1] range, which corresponds to [0%, 100%] range. Absolute length error in pixels
        /// is calculated as:
        /// <code lang="none">
        /// LengthError * ( width + height ) / 2
        /// </code>
        /// , where <b>width</b> and <b>height</b> is the size of bounding rectangle for the
        /// specified shape.
        /// </para>
        /// 
        /// <para>Default value is set to <b>0.1</b> (10%).</para>
        /// </remarks>
        ///
        public float LengthError
        {
            get { return lengthError; }
            set { lengthError = Math.Max( 0, Math.Min( 1, value ) ); }
        }

        /// <summary>
        /// Check type of the shape formed by specified points.
        /// </summary>
        /// 
        /// <param name="edgePoints">Shape's points to check.</param>
        /// 
        /// <returns>Returns type of the detected shape.</returns>
        /// 
        public ShapeType CheckShapeType( List<IntPoint> edgePoints )
        {
            if ( IsCircle( edgePoints ) )
            {
                return ShapeType.Circle;
            }

            // check for convex polygon
            List<IntPoint> corners;

            if ( IsConvexPolygon( edgePoints, out corners ) )
            {
                return ( corners.Count == 4 ) ? ShapeType.Quadrilateral : ShapeType.Triangle;
            }

            return ShapeType.Unknown;
        }

        /// <summary>
        /// Check if the specified set of points form a circle shape.
        /// </summary>
        /// 
        /// <param name="edgePoints">Shape's points to check.</param>
        /// 
        /// <returns>Returns <see langword="true"/> if the specified set of points form a
        /// circle shape or <see langword="false"/> otherwise.</returns>
        /// 
        /// <remarks><para><note>Circle shape must contain at least 8 points to be recognized.
        /// The method returns <see langword="false"/> always, of number of points in the specified
        /// shape is less than 8.</note></para></remarks>
        /// 
        public bool IsCircle( List<IntPoint> edgePoints )
        {
            Point center;
            float radius;

            return IsCircle( edgePoints, out center, out radius );
        }

        /// <summary>
        /// Check if the specified set of points form a circle shape.
        /// </summary>
        /// 
        /// <param name="edgePoints">Shape's points to check.</param>
        /// <param name="center">Receives circle's center on successful return.</param>
        /// <param name="radius">Receives circle's radius on successful return.</param>
        /// 
        /// <returns>Returns <see langword="true"/> if the specified set of points form a
        /// circle shape or <see langword="false"/> otherwise.</returns>
        /// 
        /// <remarks><para><note>Circle shape must contain at least 8 points to be recognized.
        /// The method returns <see langword="false"/> always, of number of points in the specified
        /// shape is less than 8.</note></para></remarks>
        /// 
        public bool IsCircle( List<IntPoint> edgePoints, out Point center, out float radius )
        {
            // make sure we have at least 8 points for curcle shape
            if ( edgePoints.Count < 8 )
            {
                center = new Point( 0, 0 );
                radius = 0;
                return false;
            }

            // get bounding rectangle of the points list
            IntPoint minXY, maxXY;
            PointsCloud.GetBoundingRectangle( edgePoints, out minXY, out maxXY );
            // get cloud's size
            IntPoint cloudSize = maxXY - minXY;
            // calculate center point
            center = minXY + (Point) cloudSize / 2;

            radius = ( (float) cloudSize.X + cloudSize.Y ) / 4;

            // calculate mean distance between provided edge points and estimated circle’s edge
            float meanDistance = 0;

            for ( int i = 0, n = edgePoints.Count; i < n; i++ )
            {
                meanDistance += (float) Math.Abs( center.DistanceTo( edgePoints[i] ) - radius );
            }
            meanDistance /= edgePoints.Count;

            float maxDitance = Math.Max( minAcceptableDistortion,
                ( (float) cloudSize.X + cloudSize.Y ) / 2 * relativeDistortionLimit );

            return ( meanDistance <= maxDitance );
        }

        /// <summary>
        /// Check if the specified set of points form a quadrilateral shape.
        /// </summary>
        /// 
        /// <param name="edgePoints">Shape's points to check.</param>
        /// 
        /// <returns>Returns <see langword="true"/> if the specified set of points form a
        /// quadrilateral shape or <see langword="false"/> otherwise.</returns>
        /// 
        public bool IsQuadrilateral( List<IntPoint> edgePoints )
        {
            List<IntPoint> corners;
            return IsQuadrilateral( edgePoints, out corners );
        }

        /// <summary>
        /// Check if the specified set of points form a quadrilateral shape.
        /// </summary>
        /// 
        /// <param name="edgePoints">Shape's points to check.</param>
        /// <param name="corners">List of quadrilateral corners on successful return.</param>
        /// 
        /// <returns>Returns <see langword="true"/> if the specified set of points form a
        /// quadrilateral shape or <see langword="false"/> otherwise.</returns>
        /// 
        public bool IsQuadrilateral( List<IntPoint> edgePoints, out List<IntPoint> corners )
        {
            corners = GetShapeCorners( edgePoints );

            if ( corners.Count != 4 )
                return false;

            return CheckIfPointsFitShape( edgePoints, corners );
        }

        /// <summary>
        /// Check if the specified set of points form a triangle shape.
        /// </summary>
        /// 
        /// <param name="edgePoints">Shape's points to check.</param>
        /// 
        /// <returns>Returns <see langword="true"/> if the specified set of points form a
        /// triangle shape or <see langword="false"/> otherwise.</returns>
        /// 
        public bool IsTriangle( List<IntPoint> edgePoints )
        {
            List<IntPoint> corners;
            return IsTriangle( edgePoints, out corners );
        }

        /// <summary>
        /// Check if the specified set of points form a triangle shape.
        /// </summary>
        /// 
        /// <param name="edgePoints">Shape's points to check.</param>
        /// <param name="corners">List of triangle corners on successful return.</param>
        /// 
        /// <returns>Returns <see langword="true"/> if the specified set of points form a
        /// triangle shape or <see langword="false"/> otherwise.</returns>
        /// 
        public bool IsTriangle( List<IntPoint> edgePoints, out List<IntPoint> corners )
        {
            corners = GetShapeCorners( edgePoints );

            if ( corners.Count != 3 )
                return false;

            return CheckIfPointsFitShape( edgePoints, corners );
        }

        /// <summary>
        /// Check if the specified set of points form a convex polygon shape.
        /// </summary>
        /// 
        /// <param name="edgePoints">Shape's points to check.</param>
        /// <param name="corners">List of polygon corners on successful return.</param>
        /// 
        /// <returns>Returns <see langword="true"/> if the specified set of points form a
        /// convex polygon shape or <see langword="false"/> otherwise.</returns>
        /// 
        /// <remarks><para><note>The method is able to detect only triangles and quadrilaterals
        /// for now. Check number of detected corners to resolve type of the detected polygon.
        /// </note></para></remarks>
        /// 
        public bool IsConvexPolygon( List<IntPoint> edgePoints, out List<IntPoint> corners )
        {
            corners = GetShapeCorners( edgePoints );
            return CheckIfPointsFitShape( edgePoints, corners );
        }

        /// <summary>
        /// Check sub type of a convex polygon.
        /// </summary>
        /// 
        /// <param name="corners">Corners of the convex polygon to check.</param>
        /// 
        /// <returns>Return detected sub type of the specified shape.</returns>
        /// 
        /// <remarks><para>The method check corners of a convex polygon detecting
        /// its subtype. Polygon's corners are usually retrieved using <see cref="IsConvexPolygon"/>
        /// method, but can be any list of 3-4 points (only sub types of triangles and
        /// quadrilateral are checked).</para>
        /// 
        /// <para>See <see cref="AngleError"/> and <see cref="LengthError"/> properties,
        /// which set acceptable errors for polygon sub type checking.</para>
        /// </remarks>
        /// 
        public PolygonSubType CheckPolygonSubType( List<IntPoint> corners )
        {
            PolygonSubType subType = PolygonSubType.Unknown;

            // get bounding rectangle of the points list
            IntPoint minXY, maxXY;
            PointsCloud.GetBoundingRectangle( corners, out minXY, out maxXY );
            // get cloud's size
            IntPoint cloudSize = maxXY - minXY;

            float maxLengthDiff = lengthError * ( cloudSize.X + cloudSize.Y ) / 2;

            if ( corners.Count == 3 )
            {
                // get angles of the triangle
                float angle1 = GeometryTools.GetAngleBetweenVectors( corners[0], corners[1], corners[2] );
                float angle2 = GeometryTools.GetAngleBetweenVectors( corners[1], corners[2], corners[0] );
                float angle3 = GeometryTools.GetAngleBetweenVectors( corners[2], corners[0], corners[1] );

                // check for equilateral triangle
                if ( ( Math.Abs( angle1 - 60 ) <= angleError ) &&
                     ( Math.Abs( angle2 - 60 ) <= angleError ) &&
                     ( Math.Abs( angle3 - 60 ) <= angleError ) )
                {
                    subType = PolygonSubType.EquilateralTriangle;
                }
                else
                {
                    // check for isosceles triangle
                    if ( ( Math.Abs( angle1 - angle2 ) <= angleError ) ||
                         ( Math.Abs( angle2 - angle3 ) <= angleError ) ||
                         ( Math.Abs( angle3 - angle1 ) <= angleError ) )
                    {
                        subType = PolygonSubType.IsoscelesTriangle;
                    }

                    // check for rectangled triangle
                    if ( ( Math.Abs( angle1 - 90 ) <= angleError ) ||
                         ( Math.Abs( angle2 - 90 ) <= angleError ) ||
                         ( Math.Abs( angle3 - 90 ) <= angleError ) )
                    {
                        subType = ( subType == PolygonSubType.IsoscelesTriangle ) ?
                            PolygonSubType.RectangledIsoscelesTriangle : PolygonSubType.RectangledTriangle;
                    }
                }
            }
            else if ( corners.Count == 4 )
            {
                // get angles between 2 pairs of opposite sides
                float angleBetween1stPair = GeometryTools.GetAngleBetweenLines( corners[0], corners[1], corners[2], corners[3] );
                float angleBetween2ndPair = GeometryTools.GetAngleBetweenLines( corners[1], corners[2], corners[3], corners[0] );

                // check 1st pair for parallelism
                if ( angleBetween1stPair <= angleError )
                {
                    subType = PolygonSubType.Trapezoid;

                    // check 2nd pair for parallelism
                    if ( angleBetween2ndPair <= angleError )
                    {
                        subType = PolygonSubType.Parallelogram;

                        // check angle between adjacent sides
                        if ( Math.Abs( GeometryTools.GetAngleBetweenVectors( corners[1], corners[0], corners[2] ) - 90 ) <= angleError )
                        {
                            subType = PolygonSubType.Rectangle;
                        }

                        // get length of 2 adjacent sides
                        float side1Length = (float) corners[0].DistanceTo( corners[1] );
                        float side2Length = (float) corners[0].DistanceTo( corners[3] );

                        if ( Math.Abs( side1Length - side2Length ) <= maxLengthDiff )
                        {
                            subType = ( subType == PolygonSubType.Parallelogram ) ?
                                PolygonSubType.Rhombus : PolygonSubType.Square;
                        }
                    }
                }
                else
                {
                    // check 2nd pair for parallelism - last chence to detect trapezoid
                    if ( angleBetween2ndPair <= angleError )
                    {
                        subType = PolygonSubType.Trapezoid;
                    }
                }
            }

            return subType;
        }

        /// <summary>
        /// Check if a shape specified by the set of points fits a convex polygon
        /// specified by the set of corners.
        /// </summary>
        /// 
        /// <param name="edgePoints">Shape's points to check.</param>
        /// <param name="corners">Corners of convex polygon to check fitting into.</param>
        ///
        /// <returns>Returns <see langword="true"/> if the specified shape fits
        /// the specified convex polygon or <see langword="false"/> otherwise.</returns>
        /// 
        /// <remarks><para>The method checks if the set of specified points form the same shape
        /// as the set of provided corners.</para></remarks>
        /// 
        public bool CheckIfPointsFitShape( List<IntPoint> edgePoints, List<IntPoint> corners )
        {
            int cornersCount = corners.Count;

            // lines coefficients (for representation as y(x)=k*x+b)
            float[] k = new float[cornersCount];
            float[] b = new float[cornersCount];
            float[] div = new float[cornersCount]; // precalculated divisor
            bool[] isVert = new bool[cornersCount];

            for ( int i = 0; i < cornersCount; i++ )
            {
                IntPoint currentPoint = corners[i];
                IntPoint nextPoint = ( i + 1 == cornersCount ) ? corners[0] : corners[i + 1];

                if ( !( isVert[i] = nextPoint.X == currentPoint.X ) )
                {
                    k[i] = (float) ( nextPoint.Y - currentPoint.Y ) / ( nextPoint.X - currentPoint.X );
                    b[i] = currentPoint.Y - k[i] * currentPoint.X;
                    div[i] = (float) Math.Sqrt( k[i] * k[i] + 1 );
                }
            }

            // calculate distances between edge points and polygon sides
            float meanDistance = 0;

            for ( int i = 0, n = edgePoints.Count; i < n; i++ )
            {
                float minDistance = float.MaxValue;

                for ( int j = 0; j < cornersCount; j++ )
                {
                    float distance = 0;

                    if ( !isVert[j] )
                    {
                        distance = (float) Math.Abs( ( k[j] * edgePoints[i].X + b[j] - edgePoints[i].Y ) / div[j] );
                    }
                    else
                    {
                        distance = Math.Abs( edgePoints[i].X - corners[j].X );
                    }

                    if ( distance < minDistance )
                        minDistance = distance;
                }

                meanDistance += minDistance;
            }
            meanDistance /= edgePoints.Count;

            // get bounding rectangle of the corners list
            IntPoint minXY, maxXY;
            PointsCloud.GetBoundingRectangle( corners, out minXY, out maxXY );
            IntPoint rectSize = maxXY - minXY;

            float maxDitance = Math.Max( minAcceptableDistortion,
                ( (float) rectSize.X + rectSize.Y ) / 2 * relativeDistortionLimit );

            return ( meanDistance <= maxDitance );
        }

        // Get optimized quadrilateral area
        private List<IntPoint> GetShapeCorners( List<IntPoint> edgePoints )
        {
            return shapeOptimizer.OptimizeShape( PointsCloud.FindQuadrilateralCorners( edgePoints ) );
        }
    }
}
