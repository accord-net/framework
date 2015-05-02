// AForge Fuzzy Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2011
// contacts@aforgenet.com
//
namespace AForge.Fuzzy
{
    using System;
    using AForge;

    /// <summary>
    /// Membership function composed by several connected linear functions.
    /// </summary>
    /// 
    /// <remarks><para>The piecewise linear is a generic function used by many specific fuzzy membership 
    /// functions, like the <see cref="TrapezoidalFunction">trappezoidal function</see>. This class must
    /// be instantiated with a sequence of points representing the edges of each one of the lines composing the 
    /// piecewise function.</para>
    /// 
    /// <para><note>The x-axis points must be ordered (crescent), so the <see cref="GetMembership"/> function will use each X value 
    /// as an ending point for one line and starting point of the next.</note></para>
    /// 
    /// <para>While trapezoidal and half trapezoidal are classic functions used in fuzzy functions, this class supports any function
    /// or approximation that can be represented as a sequence of lines.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // creating an array of points representing a typical trapezoidal function /-\
    /// Point [] points = new Point[4];
    /// // point where membership starts to rise
    /// points[0] = new Point( 10, 0 );
    /// // maximum membership (1) reached at the second point 
    /// points[1] = new Point( 20, 1 );
    /// // membership starts to fall at the third point 
    /// points[2] = new Point( 30, 1 );
    /// // membership gets to zero at the last point 
    /// points[3] = new Point( 40, 0 );
    /// // creating the instance
    /// PiecewiseLinearFunction membershipFunction = new PiecewiseLinearFunction( points );
    /// // getting membership for several points
    /// for ( int i = 5; i &lt; 45; i++ )
    ///     Console.WriteLine( membershipFunction.GetMembership( i ) );
    /// </code>
    /// </remarks>
    /// 
    public class PiecewiseLinearFunction : IMembershipFunction
    {
        /// <summary>
        /// Vector of (X,Y) coordinates for end/start of each line.
        /// </summary>
        protected Point[] points;

        /// <summary>
        /// The leftmost x value of the membership function, given by the first (X,Y) coordinate.
        /// </summary>
        /// 
        /// <exception cref="NullReferenceException">Points of the membership function are not initialized.</exception>
        ///
        public float LeftLimit
        {
            get
            {
                return points[0].X;
            }
        }

        /// <summary>
        /// The rightmost x value of the membership function, given by the last (X,Y) coordinate.
        /// </summary>
        /// 
        /// <exception cref="NullReferenceException">Points of the membership function are not initialized.</exception>
        ///
        public float RightLimit
        {
            get
            {
                return points[points.Length - 1].X;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PiecewiseLinearFunction"/> class. 
        /// </summary>
        /// 
        /// <remarks><para>This constructor must be used only by inherited classes to create the
        /// points vector after the instantiation.</para></remarks>
        /// 
        protected PiecewiseLinearFunction( )
        {
            points = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PiecewiseLinearFunction"/> class.
        /// </summary>
        /// 
        /// <param name="points">Array of (X,Y) coordinates of each start/end of the lines.</param>
        /// 
        /// <remarks><para>Specified point must be in crescent order on X axis and their Y value
        /// must be in the range of [0, 1].</para></remarks>
        /// 
        /// <exception cref="ArgumentException">Points must be in crescent order on X axis.</exception>
        /// <exception cref="ArgumentException">Y value of points must be in the range of [0, 1].</exception>
        /// 
        public PiecewiseLinearFunction( Point[] points )
        {
            this.points = points;

            // check if X points are in a sequence and if Y values are in [0..1] range
            for ( int i = 0, n = points.Length; i < n; i++ )
            {
                if ( ( points[i].Y < 0 ) || ( points[i].Y > 1 ) )
                    throw new ArgumentException( "Y value of points must be in the range of [0, 1]." );

                if ( i == 0 )
                    continue;

                if ( points[i - 1].X > points[i].X )
                    throw new ArgumentException( "Points must be in crescent order on X axis." );
            }
        }

        /// <summary>
        /// Calculate membership of a given value to the piecewise function.
        /// </summary>
        /// 
        /// <param name="x">Value which membership will to be calculated.</param>
        /// 
        /// <returns>Degree of membership [0..1] of the value to the fuzzy set.</returns>
        /// 
        /// <exception cref="NullReferenceException">Points of the membership function are not initialized.</exception>
        ///
        public float GetMembership( float x )
        {
            // no values belong to the fuzzy set, if there are no points in the piecewise function
            if ( points.Length == 0 )
                return 0.0f;

            // if X value is less than the first point, so first point's Y will be returned as membership
            if ( x < points[0].X )
                return points[0].Y;

            // looking for the line that contais the X value
            for ( int i = 1, n = points.Length; i < n; i++ )
            {
                // the line with X value starts in points[i-1].X and ends at points[i].X
                if ( x < points[i].X )
                {
                    // points to calculate line's equation
                    float y1 = points[i].Y;
                    float y0 = points[i - 1].Y;
                    float x1 = points[i].X;
                    float x0 = points[i - 1].X;
                    // angular coefficient
                    float m = ( y1 - y0 ) / ( x1 - x0 );
                    // returning the membership - the Y value for this X
                    return m * ( x - x0 ) + y0;
                }
            }

            // X value is more than last point, so last point Y will be returned as membership
            return points[points.Length - 1].Y;
        }
    }
}
