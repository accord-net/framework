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

    /// <summary>
    /// The class encapsulates 2D line segment and provides some tool methods related to lines.
    /// </summary>
    /// 
    /// <remarks><para>The class provides some methods which are related to line segments:
    /// distance to point, finding intersection point, etc.
    /// </para>
    /// 
    /// <para>A line segment may be converted to a <see cref="Line"/>.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create a segment
    /// LineSegment segment = new LineSegment( new Point( 0, 0 ), new Point( 3, 4 ) );
    /// // get segment's length
    /// float length = segment.Length;
    /// 
    /// // get intersection point with a line
    /// Point? intersection = segment.GetIntersectionWith(
    ///     new Line( new Point( -3, 8 ), new Point( 0, 4 ) ) );
    /// </code>
    /// </remarks>
    /// 
    public sealed class LineSegment
    {
        // segment's start/end point
        private readonly Point start;
        private readonly Point end;

        private readonly Line line;

        /// <summary>
        /// Start point of the line segment.
        /// </summary>
        public Point Start
        {
            get { return start; }
        }

        /// <summary>
        /// End point of the line segment.
        /// </summary>
        public Point End
        {
            get { return end; }
        }

        /// <summary>
        /// Get segment's length - Euclidean distance between its <see cref="Start"/> and <see cref="End"/> points.
        /// </summary>
        public float Length
        {
            get { return start.DistanceTo( end ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineSegment"/> class.
        /// </summary>
        /// 
        /// <param name="start">Segment's start point.</param>
        /// <param name="end">Segment's end point.</param>
        /// 
        /// <exception cref="ArgumentException">Thrown if the two points are the same.</exception>
        /// 
        public LineSegment( Point start, Point end )
        {
            line = Line.FromPoints( start, end );
            this.start = start;
            this.end = end;
        }

        /// <summary>
        /// Converts this <see cref="LineSegment"/> to a <see cref="Line"/> by discarding
        /// its endpoints and extending it infinitely in both directions.
        /// </summary>
        /// 
        /// <param name="segment">The segment to convert to a <see cref="Line"/>.</param>
        /// 
        /// <returns>Returns a <see cref="Line"/> that contains this <paramref name="segment"/>.</returns>
        /// 
        public static explicit operator Line( LineSegment segment )
        {
            return segment.line;
        }

        /// <summary>
        /// Calculate Euclidean distance between a point and a finite line segment.
        /// </summary>
        /// 
        /// <param name="point">The point to calculate the distance to.</param>
        /// 
        /// <returns>Returns the Euclidean distance between this line segment and the specified point. Unlike
        /// <see cref="Line.DistanceToPoint"/>, this returns the distance from the finite segment. (0,0) is 5 units
        /// from the segment (0,5)-(0,8), but is 0 units from the line through those points.</returns>
        /// 
        public float DistanceToPoint( Point point )
        {
            float segmentDistance;

            switch ( LocateProjection( point ) )
            {
                case ProjectionLocation.RayA:
                    segmentDistance = point.DistanceTo( start );
                    break;
                case ProjectionLocation.RayB:
                    segmentDistance = point.DistanceTo( end );
                    break;
                default:
                    segmentDistance = line.DistanceToPoint( point );
                    break;
            };

            return segmentDistance;
        }

        /// <summary>
        /// Finds, provided it exists, the intersection point with the specified <see cref="LineSegment"/>.
        /// </summary>
        /// 
        /// <param name="other"><see cref="LineSegment"/> to find intersection with.</param>
        /// 
        /// <returns>Returns intersection point with the specified <see cref="LineSegment"/>, or <see langword="null"/>, if
        /// the two segments do not intersect.</returns>
        /// 
        /// <remarks><para>If the two segments do not intersect, the method returns <see langword="null"/>. If the two
        /// segments share multiple points, this throws an <see cref="InvalidOperationException"/>.
        /// </para></remarks>
        /// 
        /// <exception cref="InvalidOperationException">Thrown if the segments overlap - if they have
        /// multiple points in common.</exception>
        /// 
        public Point? GetIntersectionWith( LineSegment other )
        {
            Point? result = null;

            if ( ( line.Slope == other.line.Slope ) || ( line.IsVertical && other.line.IsVertical ) )
            {
                if ( line.Intercept == other.line.Intercept )
                {
                    // Collinear segments. Inspect and handle.
                    // Consider this segment AB and other as CD. (start/end in both cases)
                    // There are three cases:
                    // 0 shared points: C and D both project onto the same ray of AB
                    // 1 shared point: One of A or B equals one of C or D, and the other of C/D 
                    //      projects on the correct ray.
                    // Many shared points.

                    ProjectionLocation projC = LocateProjection( other.start ), projD = LocateProjection( other.end );

                    if ( ( projC != ProjectionLocation.SegmentAB ) && ( projC == projD ) )
                    {
                        // no shared points
                        result = null;
                    }
                    else if ( ( ( start == other.start ) && ( projD == ProjectionLocation.RayA ) ) ||
                              ( ( start == other.end ) && ( projC == ProjectionLocation.RayA ) ) )
                    {
                        // shared start point
                        result = start;
                    }
                    else if ( ( ( end == other.start ) && ( projD == ProjectionLocation.RayB ) ) ||
                              ( ( end == other.end ) && ( projC == ProjectionLocation.RayB ) ) )
                    {
                        // shared end point
                        result = end;
                    }
                    else
                    {
                        // overlapping
                        throw new InvalidOperationException( "Overlapping segments do not have a single intersection point." );
                    }
                }
            }
            else
            {
                result = GetIntersectionWith( other.line );

                if ( ( result.HasValue ) && ( other.LocateProjection( result.Value ) != ProjectionLocation.SegmentAB ) )
                {
                    // the intersection is on the extended line of this segment
                    result = null;
                }
            }

            return result;
        }

        /// <summary>
        /// Finds, provided it exists, the intersection point with the specified <see cref="Line"/>.
        /// </summary>
        /// 
        /// <param name="other"><see cref="Line"/> to find intersection with.</param>
        /// 
        /// <returns>Returns intersection point with the specified <see cref="Line"/>, or <see langword="null"/>, if
        /// the line does not intersect with this segment.</returns>
        /// 
        /// <remarks><para>If the line and the segment do not intersect, the method returns <see langword="null"/>. If the line
        /// and the segment share multiple points, the method throws an <see cref="InvalidOperationException"/>.
        /// </para></remarks>
        /// 
        /// <exception cref="InvalidOperationException">Thrown if this segment is a portion of
        /// <paramref name="other"/> line.</exception>
        /// 
        public Point? GetIntersectionWith( Line other )
        {
            Point? result;

            if ( ( line.Slope == other.Slope ) || ( line.IsVertical && other.IsVertical ) )
            {
                if ( line.Intercept == other.Intercept ) throw new InvalidOperationException( "Segment is a portion of the specified line." );

                // unlike Line.GetIntersectionWith(Line), this does not throw on parallel distinct lines
                result = null;
            }
            else
            {
                result = line.GetIntersectionWith( other );
            }

            if ( ( result.HasValue ) && ( LocateProjection( result.Value ) != ProjectionLocation.SegmentAB ) )
            {
                // the intersection is on this segment's extended line, but not on the segment itself
                result = null;
            }

            return result;
        }

        // Represents the location of a projection of a point on the line that contains this segment.
        // If the point projects to A,B, or anything between them, it is SegmentAB.
        // If it projects beyond A, it's RayA; if it projects beyond B, it's RayB.
        private enum ProjectionLocation { RayA, SegmentAB, RayB }

        // Get type of point's projections to this line segment
        private ProjectionLocation LocateProjection( Point point )
        {
            // Modified from http://www.codeguru.com/forum/showthread.php?t=194400

            /*  How do I find the distance from a point to a line segment?

                Let the point be C (Cx,Cy) and the line be AB (Ax,Ay) to (Bx,By).
                Let P be the point of perpendicular projection of C on AB.  The parameter
                r, which indicates P's position along AB, is computed by the dot product 
                of AC and AB divided by the square of the length of AB:
                
                (1)     AC dot AB
                    r = ---------  
                        ||AB||^2
                
                r has the following meaning:
                
                    r=0      P = A
                    r=1      P = B
                    r<0      P is on the backward extension of AB (and distance C-AB is distance C-A)
                    r>1      P is on the forward extension of AB (and distance C-AB is distance C-B)
                    0<r<1    P is interior to AB (and distance C-AB(segment) is distance C-AB(line))
                
                The length of the line segment AB is computed by:
                
                    L = sqrt( (Bx-Ax)^2 + (By-Ay)^2 )
                
                and the dot product of two 2D vectors, U dot V, is computed:
                
                    D = (Ux * Vx) + (Uy * Vy) 
                
                So (1) expands to:
                
                        (Cx-Ax)(Bx-Ax) + (Cy-Ay)(By-Ay)
                    r = -------------------------------
                             (Bx-Ax)^2 + (By-Ay)^2
            */

            // the above is modified here to compare the numerator and denominator, rather than doing the division
            Point abDelta = end - start;
            Point acDelta = point - start;

            float numerator   = acDelta.X * abDelta.X + acDelta.Y * abDelta.Y;
            float denomenator = abDelta.X * abDelta.X + abDelta.Y * abDelta.Y;

            ProjectionLocation result = ( numerator < 0 ) ? ProjectionLocation.RayA : ( numerator > denomenator ) ? ProjectionLocation.RayB : ProjectionLocation.SegmentAB;

            return result;
        }

        /// <summary>
        /// Equality operator - checks if two line segments have equal parameters.
        /// </summary>
        /// 
        /// <param name="line1">First line segment to check.</param>
        /// <param name="line2">Second line segment to check.</param>
        /// 
        /// <returns>Returns <see langword="true"/> if parameters of specified
        /// line segments are equal.</returns>
        ///
        public static bool operator ==( LineSegment line1, LineSegment line2 )
        {
            if ( System.Object.ReferenceEquals( line1, line2 ) )
            {
                return true;
            }

            if ( ( (object) line1 == null ) || ( (object) line2 == null ) )
            {
                return false;
            }

            return ( ( line1.start == line2.start ) && ( line1.end == line2.end ) );
        }

        /// <summary>
        /// Inequality operator - checks if two lines have different parameters.
        /// </summary>
        /// 
        /// <param name="line1">First line segment to check.</param>
        /// <param name="line2">Second line segment to check.</param>
        /// 
        /// <returns>Returns <see langword="true"/> if parameters of specified
        /// line segments are not equal.</returns>
        ///
        public static bool operator !=( LineSegment line1, LineSegment line2 )
        {
            return !( line1 == line2 );
        }

        /// <summary>
        /// Check if this instance of <see cref="LineSegment"/> equals to the specified one.
        /// </summary>
        /// 
        /// <param name="obj">Another line segment to check equalty to.</param>
        /// 
        /// <returns>Return <see langword="true"/> if objects are equal.</returns>
        /// 
        public override bool Equals( object obj )
        {
            return ( obj is LineSegment ) ? ( this == (LineSegment) obj ) : false;
        }

        /// <summary>
        /// Get hash code for this instance.
        /// </summary>
        /// 
        /// <returns>Returns the hash code for this instance.</returns>
        /// 
        public override int GetHashCode( )
        {
            return start.GetHashCode( ) + end.GetHashCode( );
        }

        /// <summary>
        /// Get string representation of the class.
        /// </summary>
        /// 
        /// <returns>Returns string, which contains values of the like in readable form.</returns>
        ///
        public override string ToString( )
        {
            return string.Format( System.Globalization.CultureInfo.InvariantCulture, "({0}) -> ({1})", start, end );
        }
    }
}
