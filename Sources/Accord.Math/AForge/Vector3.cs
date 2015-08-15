// AForge Math Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2011
// contacts@aforgenet.com
//

namespace AForge.Math
{
    using System;

    /// <summary>
    /// 3D Vector structure with X, Y and Z coordinates.
    /// </summary>
    /// 
    /// <remarks><para>The structure incapsulates X, Y and Z coordinates of a 3D vector and
    /// provides some operations with it.</para></remarks>
    /// 
    [Serializable]
    public struct Vector3
    {
        /// <summary>
        /// X coordinate of the vector.
        /// </summary>
        public float X;
        /// <summary>
        /// Y coordinate of the vector.
        /// </summary>
        public float Y;
        /// <summary>
        /// Z coordinate of the vector.
        /// </summary>
        public float Z;

        /// <summary>
        /// Returns maximum value of the vector.
        /// </summary>
        ///
        /// <remarks><para>Returns maximum value of all 3 vector's coordinates.</para></remarks>
        ///
        public float Max
        {
            get
            {
                return ( X > Y ) ? ( ( X > Z ) ? X : Z ) : ( ( Y > Z ) ? Y : Z );
            }
        }

        /// <summary>
        /// Returns minimum value of the vector.
        /// </summary>
        ///
        /// <remarks><para>Returns minimum value of all 3 vector's coordinates.</para></remarks>
        ///
        public float Min
        {
            get
            {
                return ( X < Y ) ? ( ( X < Z ) ? X : Z ) : ( ( Y < Z ) ? Y : Z );
            }
        }

        /// <summary>
        /// Returns index of the coordinate with maximum value.
        /// </summary>
        ///
        /// <remarks><para>Returns index of the coordinate, which has the maximum value - 0 for X,
        /// 1 for Y or 2 for Z.</para>
        /// 
        /// <para><note>If there are multiple coordinates which have the same maximum value, the
        /// property returns smallest index.</note></para>
        /// </remarks>
        ///
        public int MaxIndex
        {
            get
            {
                return ( X >= Y ) ? ( ( X >= Z ) ? 0 : 2 ) : ( ( Y >= Z ) ? 1 : 2 );
            }
        }

        /// <summary>
        /// Returns index of the coordinate with minimum value.
        /// </summary>
        ///
        /// <remarks><para>Returns index of the coordinate, which has the minimum value - 0 for X,
        /// 1 for Y or 2 for Z.</para>
        /// 
        /// <para><note>If there are multiple coordinates which have the same minimum value, the
        /// property returns smallest index.</note></para>
        /// </remarks>
        ///
        public int MinIndex
        {
            get
            {
                return ( X <= Y ) ? ( ( X <= Z ) ? 0 : 2 ) : ( ( Y <= Z ) ? 1 : 2 );
            }
        }

        /// <summary>
        /// Returns vector's norm.
        /// </summary>
        /// 
        /// <remarks><para>Returns Euclidean norm of the vector, which is a
        /// square root of the sum: X<sup>2</sup>+Y<sup>2</sup>+Z<sup>2</sup>.</para>
        /// </remarks>
        /// 
        public float Norm
        {
            get { return (float) System.Math.Sqrt( X * X + Y * Y + Z * Z ); }
        }

        /// <summary>
        /// Returns square of the vector's norm.
        /// </summary>
        /// 
        /// <remarks><para>Return X<sup>2</sup>+Y<sup>2</sup>+Z<sup>2</sup>, which is
        /// a square of <see cref="Norm">vector's norm</see> or a <see cref="Dot">dot product</see> of this vector
        /// with itself.</para></remarks>
        /// 
        public float Square
        {
            get { return X * X + Y * Y + Z * Z; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3"/> structure.
        /// </summary>
        /// 
        /// <param name="x">X coordinate of the vector.</param>
        /// <param name="y">Y coordinate of the vector.</param>
        /// <param name="z">Z coordinate of the vector.</param>
        /// 
        public Vector3( float x, float y, float z )
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3"/> structure.
        /// </summary>
        /// 
        /// <param name="value">Value, which is set to all 3 coordinates of the vector.</param>
        /// 
        public Vector3( float value )
        {
            X = Y = Z = value;
        }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// 
        /// <returns>A string representation of this object.</returns>
        /// 
        public override string ToString( )
        {
            return string.Format( System.Globalization.CultureInfo.InvariantCulture,
                "{0}, {1}, {2}", X, Y, Z );
        }

        /// <summary>
        /// Returns array representation of the vector.
        /// </summary>
        /// 
        /// <returns>Array with 3 values containing X/Y/Z coordinates.</returns>
        /// 
        public float[] ToArray( )
        {
            return new float[3] { X, Y, Z };
        }

        /// <summary>
        /// Adds corresponding coordinates of two vectors.
        /// </summary>
        /// 
        /// <param name="vector1">The vector to add to.</param>
        /// <param name="vector2">The vector to add to the first vector.</param>
        /// 
        /// <returns>Returns a vector which coordinates are equal to sum of corresponding
        /// coordinates of the two specified vectors.</returns>
        ///
        public static Vector3 operator +( Vector3 vector1, Vector3 vector2 )
        {
            return new Vector3( vector1.X + vector2.X, vector1.Y + vector2.Y, vector1.Z + vector2.Z );
        }

        /// <summary>
        /// Adds corresponding coordinates of two vectors.
        /// </summary>
        /// 
        /// <param name="vector1">The vector to add to.</param>
        /// <param name="vector2">The vector to add to the first vector.</param>
        /// 
        /// <returns>Returns a vector which coordinates are equal to sum of corresponding
        /// coordinates of the two specified vectors.</returns>
        ///
        public static Vector3 Add( Vector3 vector1, Vector3 vector2 )
        {
            return vector1 + vector2;
        }

        /// <summary>
        /// Adds a value to all coordinates of the specified vector.
        /// </summary>
        /// 
        /// <param name="vector">Vector to add the specified value to.</param>
        /// <param name="value">Value to add to all coordinates of the vector.</param>
        /// 
        /// <returns>Returns new vector with all coordinates increased by the specified value.</returns>
        /// 
        public static Vector3 operator +( Vector3 vector, float value )
        {
            return new Vector3( vector.X + value, vector.Y + value, vector.Z + value );
        }

        /// <summary>
        /// Adds a value to all coordinates of the specified vector.
        /// </summary>
        /// 
        /// <param name="vector">Vector to add the specified value to.</param>
        /// <param name="value">Value to add to all coordinates of the vector.</param>
        /// 
        /// <returns>Returns new vector with all coordinates increased by the specified value.</returns>
        /// 
        public static Vector3 Add( Vector3 vector, float value )
        {
            return vector + value;
        }
       
        /// <summary>
        /// Subtracts corresponding coordinates of two vectors.
        /// </summary>
        /// 
        /// <param name="vector1">The vector to subtract from.</param>
        /// <param name="vector2">The vector to subtract from the first vector.</param>
        /// 
        /// <returns>Returns a vector which coordinates are equal to difference of corresponding
        /// coordinates of the two specified vectors.</returns>
        ///
        public static Vector3 operator -( Vector3 vector1, Vector3 vector2 )
        {
            return new Vector3( vector1.X - vector2.X, vector1.Y - vector2.Y, vector1.Z - vector2.Z );
        }

        /// <summary>
        /// Subtracts corresponding coordinates of two vectors.
        /// </summary>
        /// 
        /// <param name="vector1">The vector to subtract from.</param>
        /// <param name="vector2">The vector to subtract from the first vector.</param>
        /// 
        /// <returns>Returns a vector which coordinates are equal to difference of corresponding
        /// coordinates of the two specified vectors.</returns>
        ///
        public static Vector3 Subtract( Vector3 vector1, Vector3 vector2 )
        {
            return vector1 - vector2;
        }

        /// <summary>
        /// Subtracts a value from all coordinates of the specified vector.
        /// </summary>
        /// 
        /// <param name="vector">Vector to subtract the specified value from.</param>
        /// <param name="value">Value to subtract from all coordinates of the vector.</param>
        /// 
        /// <returns>Returns new vector with all coordinates decreased by the specified value.</returns>
        /// 
        public static Vector3 operator -( Vector3 vector, float value )
        {
            return new Vector3( vector.X - value, vector.Y - value, vector.Z - value );
        }

        /// <summary>
        /// Subtracts a value from all coordinates of the specified vector.
        /// </summary>
        /// 
        /// <param name="vector">Vector to subtract the specified value from.</param>
        /// <param name="value">Value to subtract from all coordinates of the vector.</param>
        /// 
        /// <returns>Returns new vector with all coordinates decreased by the specified value.</returns>
        /// 
        public static Vector3 Subtract( Vector3 vector, float value )
        {
            return vector - value;
        }

        /// <summary>
        /// Multiplies corresponding coordinates of two vectors.
        /// </summary>
        /// 
        /// <param name="vector1">The first vector to multiply.</param>
        /// <param name="vector2">The second vector to multiply.</param>
        /// 
        /// <returns>Returns a vector which coordinates are equal to multiplication of corresponding
        /// coordinates of the two specified vectors.</returns>
        ///
        public static Vector3 operator *( Vector3 vector1, Vector3 vector2 )
        {
            return new Vector3( vector1.X * vector2.X, vector1.Y * vector2.Y, vector1.Z * vector2.Z );
        }

        /// <summary>
        /// Multiplies corresponding coordinates of two vectors.
        /// </summary>
        /// 
        /// <param name="vector1">The first vector to multiply.</param>
        /// <param name="vector2">The second vector to multiply.</param>
        /// 
        /// <returns>Returns a vector which coordinates are equal to multiplication of corresponding
        /// coordinates of the two specified vectors.</returns>
        ///
        public static Vector3 Multiply( Vector3 vector1, Vector3 vector2 )
        {
            return vector1 * vector2;
        }

        /// <summary>
        /// Multiplies coordinates of the specified vector by the specified factor.
        /// </summary>
        /// 
        /// <param name="vector">Vector to multiply coordinates of.</param>
        /// <param name="factor">Factor to multiple coordinates of the specified vector by.</param>
        /// 
        /// <returns>Returns new vector with all coordinates multiplied by the specified factor.</returns>
        ///
        public static Vector3 operator *( Vector3 vector, float factor )
        {
            return new Vector3( vector.X * factor, vector.Y * factor, vector.Z * factor );
        }

        /// <summary>
        /// Multiplies coordinates of the specified vector by the specified factor.
        /// </summary>
        /// 
        /// <param name="vector">Vector to multiply coordinates of.</param>
        /// <param name="factor">Factor to multiple coordinates of the specified vector by.</param>
        /// 
        /// <returns>Returns new vector with all coordinates multiplied by the specified factor.</returns>
        ///
        public static Vector3 Multiply( Vector3 vector, float factor )
        {
            return vector * factor;
        }

        /// <summary>
        /// Divides corresponding coordinates of two vectors.
        /// </summary>
        /// 
        /// <param name="vector1">The first vector to divide.</param>
        /// <param name="vector2">The second vector to devide.</param>
        /// 
        /// <returns>Returns a vector which coordinates are equal to coordinates of the first vector divided by
        /// corresponding coordinates of the second vector.</returns>
        ///
        public static Vector3 operator /( Vector3 vector1, Vector3 vector2 )
        {
            return new Vector3( vector1.X / vector2.X, vector1.Y / vector2.Y, vector1.Z / vector2.Z );
        }

        /// <summary>
        /// Divides corresponding coordinates of two vectors.
        /// </summary>
        /// 
        /// <param name="vector1">The first vector to divide.</param>
        /// <param name="vector2">The second vector to devide.</param>
        /// 
        /// <returns>Returns a vector which coordinates are equal to coordinates of the first vector divided by
        /// corresponding coordinates of the second vector.</returns>
        ///
        public static Vector3 Divide( Vector3 vector1, Vector3 vector2 )
        {
            return vector1 / vector2;
        }

        /// <summary>
        /// Divides coordinates of the specified vector by the specified factor.
        /// </summary>
        /// 
        /// <param name="vector">Vector to divide coordinates of.</param>
        /// <param name="factor">Factor to divide coordinates of the specified vector by.</param>
        /// 
        /// <returns>Returns new vector with all coordinates divided by the specified factor.</returns>
        ///
        public static Vector3 operator /( Vector3 vector, float factor )
        {
            return new Vector3( vector.X / factor, vector.Y / factor, vector.Z / factor );
        }

        /// <summary>
        /// Divides coordinates of the specified vector by the specified factor.
        /// </summary>
        /// 
        /// <param name="vector">Vector to divide coordinates of.</param>
        /// <param name="factor">Factor to divide coordinates of the specified vector by.</param>
        /// 
        /// <returns>Returns new vector with all coordinates divided by the specified factor.</returns>
        ///
        public static Vector3 Divide( Vector3 vector, float factor )
        {
            return vector / factor;
        }

        /// <summary>
        /// Tests whether two specified vectors are equal.
        /// </summary>
        /// 
        /// <param name="vector1">The left-hand vector.</param>
        /// <param name="vector2">The right-hand vector.</param>
        /// 
        /// <returns>Returns <see langword="true"/> if the two vectors are equal or <see langword="false"/> otherwise.</returns>
        /// 
        public static bool operator ==( Vector3 vector1, Vector3 vector2 )
        {
            return ( ( vector1.X == vector2.X ) && ( vector1.Y == vector2.Y ) && ( vector1.Z == vector2.Z ) );
        }

        /// <summary>
        /// Tests whether two specified vectors are not equal.
        /// </summary>
        /// 
        /// <param name="vector1">The left-hand vector.</param>
        /// <param name="vector2">The right-hand vector.</param>
        /// 
        /// <returns>Returns <see langword="true"/> if the two vectors are not equal or <see langword="false"/> otherwise.</returns>
        /// 
        public static bool operator !=( Vector3 vector1, Vector3 vector2 )
        {
            return ( ( vector1.X != vector2.X ) || ( vector1.Y != vector2.Y ) || ( vector1.Z != vector2.Z ) );
        }

        /// <summary>
        /// Tests whether the vector equals to the specified one.
        /// </summary>
        /// 
        /// <param name="vector">The vector to test equality with.</param>
        /// 
        /// <returns>Returns <see langword="true"/> if the two vectors are equal or <see langword="false"/> otherwise.</returns>
        /// 
        public bool Equals( Vector3 vector )
        {
            return ( ( vector.X == X ) && ( vector.Y == Y ) && ( vector.Z == Z ) );
        }

        /// <summary>
        /// Tests whether the vector equals to the specified object.
        /// </summary>
        /// 
        /// <param name="obj">The object to test equality with.</param>
        /// 
        /// <returns>Returns <see langword="true"/> if the vector equals to the specified object or <see langword="false"/> otherwise.</returns>
        /// 
        public override bool Equals( Object obj )
        {
            if ( obj is Vector3 )
            {
                return Equals( (Vector3) obj );
            }
            return false;
        }

        /// <summary>
        /// Returns the hashcode for this instance.
        /// </summary>
        /// 
        /// <returns>A 32-bit signed integer hash code.</returns>
        /// 
        public override int GetHashCode( )
        {
            return X.GetHashCode( ) + Y.GetHashCode( ) + Z.GetHashCode( );
        }

        /// <summary>
        /// Normalizes the vector by dividing it’s all coordinates with the vector's norm.
        /// </summary>
        /// 
        /// <returns>Returns the value of vectors’ norm before normalization.</returns>
        ///
        public float Normalize( )
        {
            float norm = (float) System.Math.Sqrt( X * X + Y * Y + Z * Z );
            float invNorm = 1.0f / norm;

            X *= invNorm;
            Y *= invNorm;
            Z *= invNorm;

            return norm;
        }

        /// <summary>
        /// Inverse the vector.
        /// </summary>
        /// 
        /// <returns>Returns a vector with all coordinates equal to 1.0 divided by the value of corresponding coordinate
        /// in this vector (or equal to 0.0 if this vector has corresponding coordinate also set to 0.0).</returns>
        ///
        public Vector3 Inverse( )
        {
            return new Vector3(
                ( X == 0 ) ? 0 : 1.0f / X,
                ( Y == 0 ) ? 0 : 1.0f / Y,
                ( Z == 0 ) ? 0 : 1.0f / Z );
        }

        /// <summary>
        /// Calculate absolute values of the vector.
        /// </summary>
        /// 
        /// <returns>Returns a vector with all coordinates equal to absolute values of this vector's coordinates.</returns>
        /// 
        public Vector3 Abs( )
        {
            return new Vector3( System.Math.Abs( X ), System.Math.Abs( Y ), System.Math.Abs( Z ) );
        }
        
        /// <summary>
        /// Calculates cross product of two vectors.
        /// </summary>
        /// 
        /// <param name="vector1">First vector to use for cross product calculation.</param>
        /// <param name="vector2">Second vector to use for cross product calculation.</param>
        /// 
        /// <returns>Returns cross product of the two specified vectors.</returns>
        /// 
        public static Vector3 Cross( Vector3 vector1, Vector3 vector2 )
        {
            return new Vector3(
                vector1.Y * vector2.Z - vector1.Z * vector2.Y,
                vector1.Z * vector2.X - vector1.X * vector2.Z,
                vector1.X * vector2.Y - vector1.Y * vector2.X );
        }

        /// <summary>
        /// Calculates dot product of two vectors.
        /// </summary>
        /// 
        /// <param name="vector1">First vector to use for dot product calculation.</param>
        /// <param name="vector2">Second vector to use for dot product calculation.</param>
        /// 
        /// <returns>Returns dot product of the two specified vectors.</returns>
        /// 
        public static float Dot( Vector3 vector1, Vector3 vector2 )
        {
            return vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z;
        }

        /// <summary>
        /// Converts the vector to a 4D vector.
        /// </summary>
        /// 
        /// <returns>Returns 4D vector which is an extension of the 3D vector.</returns>
        /// 
        /// <remarks><para>The method returns a 4D vector which has X, Y and Z coordinates equal to the
        /// coordinates of this 3D vector and <see cref="Vector4.W">W</see> coordinate set to 1.0.</para>
        /// </remarks>
        /// 
        public Vector4 ToVector4( )
        {
            return new Vector4( X, Y, Z, 1 );
        }
    }
}
