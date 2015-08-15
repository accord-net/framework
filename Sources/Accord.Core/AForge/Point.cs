// AForge Core Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2011
// contacts@aforgenet.com
//

namespace AForge
{
    using System;

    /// <summary>
    /// Structure for representing a pair of coordinates of float type.
    /// </summary>
    /// 
    /// <remarks><para>The structure is used to store a pair of floating point
    /// coordinates with single precision.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // assigning coordinates in the constructor
    /// Point p1 = new Point( 10, 20 );
    /// // creating a point and assigning coordinates later
    /// Point p2;
    /// p2.X = 30;
    /// p2.Y = 40;
    /// // calculating distance between two points
    /// float distance = p1.DistanceTo( p2 );
    /// </code>
    /// </remarks>
    /// 
    [Serializable]
    public struct Point
    {
        /// <summary> 
        /// X coordinate.
        /// </summary> 
        /// 
        public float X;

        /// <summary> 
        /// Y coordinate.
        /// </summary> 
        /// 
        public float Y;

        /// <summary>
        /// Initializes a new instance of the <see cref="Point"/> structure.
        /// </summary>
        /// 
        /// <param name="x">X axis coordinate.</param>
        /// <param name="y">Y axis coordinate.</param>
        /// 
        public Point( float x, float y )
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Calculate Euclidean distance between two points.
        /// </summary>
        /// 
        /// <param name="anotherPoint">Point to calculate distance to.</param>
        /// 
        /// <returns>Returns Euclidean distance between this point and
        /// <paramref name="anotherPoint"/> points.</returns>
        /// 
        public float DistanceTo( Point anotherPoint )
        {
            float dx = X - anotherPoint.X;
            float dy = Y - anotherPoint.Y;

            return (float) System.Math.Sqrt( dx * dx + dy * dy );
        }

        /// <summary>
        /// Calculate squared Euclidean distance between two points.
        /// </summary>
        /// 
        /// <param name="anotherPoint">Point to calculate distance to.</param>
        /// 
        /// <returns>Returns squared Euclidean distance between this point and
        /// <paramref name="anotherPoint"/> points.</returns>
        /// 
        public float SquaredDistanceTo( Point anotherPoint )
        {
            float dx = X - anotherPoint.X;
            float dy = Y - anotherPoint.Y;

            return dx * dx + dy * dy;
        }

        /// <summary>
        /// Addition operator - adds values of two points.
        /// </summary>
        /// 
        /// <param name="point1">First point for addition.</param>
        /// <param name="point2">Second point for addition.</param>
        /// 
        /// <returns>Returns new point which coordinates equal to sum of corresponding
        /// coordinates of specified points.</returns>
        /// 
        public static Point operator +( Point point1, Point point2 )
        {
            return new Point( point1.X + point2.X, point1.Y + point2.Y );
        }

        /// <summary>
        /// Addition operator - adds values of two points.
        /// </summary>
        /// 
        /// <param name="point1">First point for addition.</param>
        /// <param name="point2">Second point for addition.</param>
        /// 
        /// <returns>Returns new point which coordinates equal to sum of corresponding
        /// coordinates of specified points.</returns>
        /// 
        public static Point Add( Point point1, Point point2 )
        {
            return new Point( point1.X + point2.X, point1.Y + point2.Y );
        }

        /// <summary>
        /// Subtraction operator - subtracts values of two points.
        /// </summary>
        /// 
        /// <param name="point1">Point to subtract from.</param>
        /// <param name="point2">Point to subtract.</param>
        /// 
        /// <returns>Returns new point which coordinates equal to difference of corresponding
        /// coordinates of specified points.</returns>
        ///
        public static Point operator -( Point point1, Point point2 )
        {
            return new Point( point1.X - point2.X, point1.Y - point2.Y );
        }

        /// <summary>
        /// Subtraction operator - subtracts values of two points.
        /// </summary>
        /// 
        /// <param name="point1">Point to subtract from.</param>
        /// <param name="point2">Point to subtract.</param>
        /// 
        /// <returns>Returns new point which coordinates equal to difference of corresponding
        /// coordinates of specified points.</returns>
        ///
        public static Point Subtract( Point point1, Point point2 )
        {
            return new Point( point1.X - point2.X, point1.Y - point2.Y );
        }

        /// <summary>
        /// Addition operator - adds scalar to the specified point.
        /// </summary>
        /// 
        /// <param name="point">Point to increase coordinates of.</param>
        /// <param name="valueToAdd">Value to add to coordinates of the specified point.</param>
        /// 
        /// <returns>Returns new point which coordinates equal to coordinates of
        /// the specified point increased by specified value.</returns>
        /// 
        public static Point operator +( Point point, float valueToAdd )
        {
            return new Point( point.X + valueToAdd, point.Y + valueToAdd );
        }

        /// <summary>
        /// Addition operator - adds scalar to the specified point.
        /// </summary>
        /// 
        /// <param name="point">Point to increase coordinates of.</param>
        /// <param name="valueToAdd">Value to add to coordinates of the specified point.</param>
        /// 
        /// <returns>Returns new point which coordinates equal to coordinates of
        /// the specified point increased by specified value.</returns>
        /// 
        public static Point Add( Point point, float valueToAdd )
        {
            return new Point( point.X + valueToAdd, point.Y + valueToAdd );
        }

        /// <summary>
        /// Subtraction operator - subtracts scalar from the specified point.
        /// </summary>
        /// 
        /// <param name="point">Point to decrease coordinates of.</param>
        /// <param name="valueToSubtract">Value to subtract from coordinates of the specified point.</param>
        /// 
        /// <returns>Returns new point which coordinates equal to coordinates of
        /// the specified point decreased by specified value.</returns>
        /// 
        public static Point operator -( Point point, float valueToSubtract )
        {
            return new Point( point.X - valueToSubtract, point.Y - valueToSubtract );
        }

        /// <summary>
        /// Subtraction operator - subtracts scalar from the specified point.
        /// </summary>
        /// 
        /// <param name="point">Point to decrease coordinates of.</param>
        /// <param name="valueToSubtract">Value to subtract from coordinates of the specified point.</param>
        /// 
        /// <returns>Returns new point which coordinates equal to coordinates of
        /// the specified point decreased by specified value.</returns>
        /// 
        public static Point Subtract( Point point, float valueToSubtract )
        {
            return new Point( point.X - valueToSubtract, point.Y - valueToSubtract );
        }

        /// <summary>
        /// Multiplication operator - multiplies coordinates of the specified point by scalar value.
        /// </summary>
        /// 
        /// <param name="point">Point to multiply coordinates of.</param>
        /// <param name="factor">Multiplication factor.</param>
        /// 
        /// <returns>Returns new point which coordinates equal to coordinates of
        /// the specified point multiplied by specified value.</returns>
        ///
        public static Point operator *( Point point, float factor )
        {
            return new Point( point.X * factor, point.Y * factor );
        }

        /// <summary>
        /// Multiplication operator - multiplies coordinates of the specified point by scalar value.
        /// </summary>
        /// 
        /// <param name="point">Point to multiply coordinates of.</param>
        /// <param name="factor">Multiplication factor.</param>
        /// 
        /// <returns>Returns new point which coordinates equal to coordinates of
        /// the specified point multiplied by specified value.</returns>
        ///
        public static Point Multiply( Point point, float factor )
        {
            return new Point( point.X * factor, point.Y * factor );
        }

        /// <summary>
        /// Division operator - divides coordinates of the specified point by scalar value.
        /// </summary>
        /// 
        /// <param name="point">Point to divide coordinates of.</param>
        /// <param name="factor">Division factor.</param>
        /// 
        /// <returns>Returns new point which coordinates equal to coordinates of
        /// the specified point divided by specified value.</returns>
        /// 
        public static Point operator /( Point point, float factor )
        {
            return new Point( point.X / factor, point.Y / factor );
        }

        /// <summary>
        /// Division operator - divides coordinates of the specified point by scalar value.
        /// </summary>
        /// 
        /// <param name="point">Point to divide coordinates of.</param>
        /// <param name="factor">Division factor.</param>
        /// 
        /// <returns>Returns new point which coordinates equal to coordinates of
        /// the specified point divided by specified value.</returns>
        /// 
        public static Point Divide( Point point, float factor )
        {
            return new Point( point.X / factor, point.Y / factor );
        }

        /// <summary>
        /// Equality operator - checks if two points have equal coordinates.
        /// </summary>
        /// 
        /// <param name="point1">First point to check.</param>
        /// <param name="point2">Second point to check.</param>
        /// 
        /// <returns>Returns <see langword="true"/> if coordinates of specified
        /// points are equal.</returns>
        ///
        public static bool operator ==( Point point1, Point point2 )
        {
            return ( ( point1.X == point2.X ) && ( point1.Y == point2.Y ) );
        }

        /// <summary>
        /// Inequality operator - checks if two points have different coordinates.
        /// </summary>
        /// 
        /// <param name="point1">First point to check.</param>
        /// <param name="point2">Second point to check.</param>
        /// 
        /// <returns>Returns <see langword="true"/> if coordinates of specified
        /// points are not equal.</returns>
        ///
        public static bool operator !=( Point point1, Point point2 )
        {
            return ( ( point1.X != point2.X ) || ( point1.Y != point2.Y ) );
        }

        /// <summary>
        /// Check if this instance of <see cref="Point"/> equal to the specified one.
        /// </summary>
        /// 
        /// <param name="obj">Another point to check equalty to.</param>
        /// 
        /// <returns>Return <see langword="true"/> if objects are equal.</returns>
        /// 
        public override bool Equals( object obj )
        {
            return ( obj is Point ) ? ( this == (Point) obj ) : false;
        }

        /// <summary>
        /// Get hash code for this instance.
        /// </summary>
        /// 
        /// <returns>Returns the hash code for this instance.</returns>
        /// 
        public override int GetHashCode( )
        {
            return X.GetHashCode( ) + Y.GetHashCode( );
        }

        /// <summary>
        /// Explicit conversion to <see cref="IntPoint"/>.
        /// </summary>
        /// 
        /// <param name="point">Single precision point to convert to integer point.</param>
        /// 
        /// <returns>Returns new integer point which coordinates are explicitly converted
        /// to integers from coordinates of the specified single precision point by
        /// casting float values to integers value.</returns>
        /// 
        public static explicit operator IntPoint( Point point )
        {
            return new IntPoint( (int) point.X, (int) point.Y );
        }

        /// <summary>
        /// Implicit conversion to <see cref="DoublePoint"/>.
        /// </summary>
        /// 
        /// <param name="point">Single precision point to convert to double precision point.</param>
        /// 
        /// <returns>Returns new double precision point which coordinates are implicitly converted
        /// to doubles from coordinates of the specified single precision point.</returns>
        /// 
        public static implicit operator DoublePoint( Point point )
        {
            return new DoublePoint( point.X, point.Y );
        } 

        /// <summary>
        /// Rounds the single precision point.
        /// </summary>
        /// 
        /// <returns>Returns new integer point, which coordinates equal to whole numbers
        /// nearest to the corresponding coordinates of the single precision point.</returns>
        /// 
        public IntPoint Round( )
        {
            return new IntPoint( (int) Math.Round( X ), (int) Math.Round( Y ) );
        }

        /// <summary>
        /// Get string representation of the class.
        /// </summary>
        /// 
        /// <returns>Returns string, which contains values of the point in readable form.</returns>
        ///
        public override string ToString( )
        {
            return string.Format( System.Globalization.CultureInfo.InvariantCulture, "{0}, {1}", X, Y );
        }

        /// <summary>
        /// Calculate Euclidean norm of the vector comprised of the point's 
        /// coordinates - distance from (0, 0) in other words.
        /// </summary>
        /// 
        /// <returns>Returns point's distance from (0, 0) point.</returns>
        /// 
        public float EuclideanNorm( )
        {
            return (float) System.Math.Sqrt( X * X + Y * Y );
        }
    }
}
