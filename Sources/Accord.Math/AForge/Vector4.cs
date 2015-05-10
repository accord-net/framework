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
    /// 4D Vector structure with X, Y, Z and W coordinates.
    /// </summary>
    /// 
    /// <remarks><para>The structure incapsulates X, Y, Z and W coordinates of a 4D vector and
    /// provides some operations with it.</para></remarks>
    /// 
    [Serializable]
    public struct Vector4
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
        /// W coordinate of the vector.
        /// </summary>
        public float W;

        /// <summary>
        /// Returns maximum value of the vector.
        /// </summary>
        ///
        /// <remarks><para>Returns maximum value of all 4 vector's coordinates.</para></remarks>
        ///
        public float Max
        {
            get
            {
                float v1 = ( X > Y ) ? X : Y;
                float v2 = ( Z > W ) ? Z : W;

                return ( v1 > v2 ) ? v1 : v2;
            }
        }

        /// <summary>
        /// Returns minimum value of the vector.
        /// </summary>
        ///
        /// <remarks><para>Returns minimum value of all 4 vector's coordinates.</para></remarks>
        ///
        public float Min
        {
            get
            {
                float v1 = ( X < Y ) ? X : Y;
                float v2 = ( Z < W ) ? Z : W;

                return ( v1 < v2 ) ? v1 : v2;
            }
        }

        /// <summary>
        /// Returns index of the coordinate with maximum value.
        /// </summary>
        ///
        /// <remarks><para>Returns index of the coordinate, which has the maximum value - 0 for X,
        /// 1 for Y, 2 for Z or 3 for W.</para>
        /// 
        /// <para><note>If there are multiple coordinates which have the same maximum value, the
        /// property returns smallest index.</note></para>
        /// </remarks>
        ///
        public int MaxIndex
        {
            get
            {
                float v1 = 0;
                float v2 = 0;
                int i1 = 0;
                int i2 = 0;

                if ( X >= Y )
                {
                    v1 = X;
                    i1 = 0;
                }
                else
                {
                    v1 = Y;
                    i1 = 1;
                }

                if ( Z >= W )
                {
                    v2 = Z;
                    i2 = 2;
                }
                else
                {
                    v2 = W;
                    i2 = 3;
                }

                return ( v1 >= v2 ) ? i1 : i2;
            }
        }

        /// <summary>
        /// Returns index of the coordinate with minimum value.
        /// </summary>
        ///
        /// <remarks><para>Returns index of the coordinate, which has the minimum value - 0 for X,
        /// 1 for Y, 2 for Z or 3 for W.</para>
        /// 
        /// <para><note>If there are multiple coordinates which have the same minimum value, the
        /// property returns smallest index.</note></para>
        /// </remarks>
        ///
        public int MinIndex
        {
            get
            {
                float v1 = 0;
                float v2 = 0;
                int i1 = 0;
                int i2 = 0;

                if ( X <= Y )
                {
                    v1 = X;
                    i1 = 0;
                }
                else
                {
                    v1 = Y;
                    i1 = 1;
                }

                if ( Z <= W )
                {
                    v2 = Z;
                    i2 = 2;
                }
                else
                {
                    v2 = W;
                    i2 = 3;
                }

                return ( v1 <= v2 ) ? i1 : i2;
            }
        }

        /// <summary>
        /// Returns vector's norm.
        /// </summary>
        /// 
        /// <remarks><para>Returns Euclidean norm of the vector, which is a
        /// square root of the sum: X<sup>2</sup>+Y<sup>2</sup>+Z<sup>2</sup>+W<sup>2</sup>.</para>
        /// </remarks>
        /// 
        public float Norm
        {
            get { return (float) System.Math.Sqrt( X * X + Y * Y + Z * Z + W * W ); }
        }

        /// <summary>
        /// Returns square of the vector's norm.
        /// </summary>
        /// 
        /// <remarks><para>Return X<sup>2</sup>+Y<sup>2</sup>+Z<sup>2</sup>+W<sup>2</sup>, which is
        /// a square of <see cref="Norm">vector's norm</see> or a <see cref="Dot">dot product</see> of this vector
        /// with itself.</para></remarks>
        /// 
        public float Square
        {
            get { return X * X + Y * Y + Z * Z + W * W; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector4"/> structure.
        /// </summary>
        /// 
        /// <param name="x">X coordinate of the vector.</param>
        /// <param name="y">Y coordinate of the vector.</param>
        /// <param name="z">Z coordinate of the vector.</param>
        /// <param name="w">W coordinate of the vector.</param>
        /// 
        public Vector4( float x, float y, float z, float w )
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector4"/> structure.
        /// </summary>
        /// 
        /// <param name="value">Value, which is set to all 4 coordinates of the vector.</param>
        /// 
        public Vector4( float value )
        {
            X = Y = Z = W = value;
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
                "{0}, {1}, {2}, {3}", X, Y, Z, W );
        }

        /// <summary>
        /// Returns array representation of the vector.
        /// </summary>
        /// 
        /// <returns>Array with 4 values containing X/Y/Z/W coordinates.</returns>
        /// 
        public float[] ToArray( )
        {
            return new float[4] { X, Y, Z, W };
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
        public static Vector4 operator +( Vector4 vector1, Vector4 vector2 )
        {
            return new Vector4( vector1.X + vector2.X, vector1.Y + vector2.Y,
                                vector1.Z + vector2.Z, vector1.W + vector2.W );
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
        public static Vector4 Add( Vector4 vector1, Vector4 vector2 )
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
        public static Vector4 operator +( Vector4 vector, float value )
        {
            return new Vector4( vector.X + value, vector.Y + value, vector.Z + value, vector.W + value );
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
        public static Vector4 Add( Vector4 vector, float value )
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
        public static Vector4 operator -( Vector4 vector1, Vector4 vector2 )
        {
            return new Vector4( vector1.X - vector2.X, vector1.Y - vector2.Y,
                                vector1.Z - vector2.Z, vector1.W - vector2.W );
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
        public static Vector4 Subtract( Vector4 vector1, Vector4 vector2 )
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
        public static Vector4 operator -( Vector4 vector, float value )
        {
            return new Vector4( vector.X - value, vector.Y - value, vector.Z - value, vector.W - value );
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
        public static Vector4 Subtract( Vector4 vector, float value )
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
        public static Vector4 operator *( Vector4 vector1, Vector4 vector2 )
        {
            return new Vector4( vector1.X * vector2.X, vector1.Y * vector2.Y,
                                vector1.Z * vector2.Z, vector1.W * vector2.W );
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
        public static Vector4 Multiply( Vector4 vector1, Vector4 vector2 )
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
        public static Vector4 operator *( Vector4 vector, float factor )
        {
            return new Vector4( vector.X * factor, vector.Y * factor, vector.Z * factor, vector.W * factor );
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
        public static Vector4 Multiply( Vector4 vector, float factor )
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
        public static Vector4 operator /( Vector4 vector1, Vector4 vector2 )
        {
            return new Vector4( vector1.X / vector2.X, vector1.Y / vector2.Y,
                                vector1.Z / vector2.Z, vector1.W / vector2.W );
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
        public static Vector4 Divide( Vector4 vector1, Vector4 vector2 )
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
        public static Vector4 operator /( Vector4 vector, float factor )
        {
            return new Vector4( vector.X / factor, vector.Y / factor, vector.Z / factor, vector.W / factor );
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
        public static Vector4 Divide( Vector4 vector, float factor )
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
        public static bool operator ==( Vector4 vector1, Vector4 vector2 )
        {
            return ( ( vector1.X == vector2.X ) && ( vector1.Y == vector2.Y ) &&
                     ( vector1.Z == vector2.Z ) && ( vector1.W == vector2.W ) );
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
        public static bool operator !=( Vector4 vector1, Vector4 vector2 )
        {
            return ( ( vector1.X != vector2.X ) || ( vector1.Y != vector2.Y ) ||
                     ( vector1.Z != vector2.Z ) || ( vector1.W != vector2.W ) );
        }

        /// <summary>
        /// Tests whether the vector equals to the specified one.
        /// </summary>
        /// 
        /// <param name="vector">The vector to test equality with.</param>
        /// 
        /// <returns>Returns <see langword="true"/> if the two vectors are equal or <see langword="false"/> otherwise.</returns>
        /// 
        public bool Equals( Vector4 vector )
        {
            return ( ( vector.X == X ) && ( vector.Y == Y ) && ( vector.Z == Z ) && ( vector.W == W ) );
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
            if ( obj is Vector4 )
            {
                return Equals( (Vector4) obj );
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
            return X.GetHashCode( ) + Y.GetHashCode( ) + Z.GetHashCode( ) + W.GetHashCode( );
        }

        /// <summary>
        /// Normalizes the vector by dividing it’s all coordinates with the vector's norm.
        /// </summary>
        /// 
        /// <returns>Returns the value of vectors’ norm before normalization.</returns>
        ///
        public float Normalize( )
        {
            float norm = (float) System.Math.Sqrt( X * X + Y * Y + Z * Z + W * W );
            float invNorm = 1.0f / norm;

            X *= invNorm;
            Y *= invNorm;
            Z *= invNorm;
            W *= invNorm;

            return norm;
        }

        /// <summary>
        /// Inverse the vector.
        /// </summary>
        /// 
        /// <returns>Returns a vector with all coordinates equal to 1.0 divided by the value of corresponding coordinate
        /// in this vector (or equal to 0.0 if this vector has corresponding coordinate also set to 0.0).</returns>
        ///
        public Vector4 Inverse( )
        {
            return new Vector4(
                ( X == 0 ) ? 0 : 1.0f / X,
                ( Y == 0 ) ? 0 : 1.0f / Y,
                ( Z == 0 ) ? 0 : 1.0f / Z,
                ( W == 0 ) ? 0 : 1.0f / W );
        }

        /// <summary>
        /// Calculate absolute values of the vector.
        /// </summary>
        /// 
        /// <returns>Returns a vector with all coordinates equal to absolute values of this vector's coordinates.</returns>
        /// 
        public Vector4 Abs( )
        {
            return new Vector4( System.Math.Abs( X ), System.Math.Abs( Y ), System.Math.Abs( Z ), System.Math.Abs( W ) );
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
        public static float Dot( Vector4 vector1, Vector4 vector2 )
        {
            return vector1.X * vector2.X + vector1.Y * vector2.Y +
                   vector1.Z * vector2.Z + vector1.W * vector2.W;
        }

        /// <summary>
        /// Converts the vector to a 3D vector.
        /// </summary>
        /// 
        /// <returns>Returns 3D vector which has X/Y/Z coordinates equal to X/Y/Z coordinates
        /// of this vector divided by <see cref="W"/>.</returns>
        /// 
        public Vector3 ToVector3( )
        {
            return new Vector3( X / W, Y / W, Z / W );
        }
    }
}
