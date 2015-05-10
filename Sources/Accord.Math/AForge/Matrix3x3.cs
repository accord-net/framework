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
    /// A structure representing 3x3 matrix.
    /// </summary>
    /// 
    /// <remarks><para>The structure incapsulates elements of a 3x3 matrix and
    /// provides some operations with it.</para></remarks>
    /// 
    [Serializable]
    public struct Matrix3x3
    {
        /// <summary>
        /// Row 0 column 0 element of the matrix.
        /// </summary>
        public float V00;
        /// <summary>
        /// Row 0 column 1 element of the matrix.
        /// </summary>
        public float V01;
        /// <summary>
        /// Row 0 column 2 element of the matrix.
        /// </summary>
        public float V02;

        /// <summary>
        /// Row 1 column 0 element of the matrix.
        /// </summary>
        public float V10;
        /// <summary>
        /// Row 1 column 1 element of the matrix.
        /// </summary>
        public float V11;
        /// <summary>
        /// Row 1 column 2 element of the matrix.
        /// </summary>
        public float V12;

        /// <summary>
        /// Row 2 column 0 element of the matrix.
        /// </summary>
        public float V20;
        /// <summary>
        /// Row 2 column 1 element of the matrix.
        /// </summary>
        public float V21;
        /// <summary>
        /// Row 2 column 2 element of the matrix.
        /// </summary>
        public float V22;

        /// <summary>
        /// Provides an identity matrix with all diagonal elements set to 1.
        /// </summary>
        public static Matrix3x3 Identity
        {
            get
            {
                Matrix3x3 m = new Matrix3x3( );
                m.V00 = m.V11 = m.V22 = 1;
                return m;
            }
        }

        /// <summary>
        /// Calculates determinant of the matrix.
        /// </summary>
        public float Determinant
        {
            get
            {
                return
                    V00 * V11 * V22 + V01 * V12 * V20 + V02 * V10 * V21 -
                    V00 * V12 * V21 - V01 * V10 * V22 - V02 * V11 * V20;
            }
        }

        /// <summary>
        /// Returns array representation of the matrix.
        /// </summary>
        /// 
        /// <returns>Returns array which contains all elements of the matrix in the row-major order.</returns>
        /// 
        public float[] ToArray( )
        {
            return new float[]
            {
                V00, V01, V02, V10, V11, V12, V20, V21, V22
            };
        }

        /// <summary>
        /// Creates rotation matrix around Y axis.
        /// </summary>
        /// 
        /// <param name="radians">Rotation angle around Y axis in radians.</param>
        /// 
        /// <returns>Returns rotation matrix to rotate an object around Y axis.</returns>
        /// 
        public static Matrix3x3 CreateRotationY( float radians )
        {
            Matrix3x3 m = new Matrix3x3( );

            float cos = (float) System.Math.Cos( radians );
            float sin = (float) System.Math.Sin( radians );

            m.V00 = m.V22 = cos;
            m.V02 = sin;
            m.V20 = -sin;
            m.V11 = 1;

            return m;
        }

        /// <summary>
        /// Creates rotation matrix around X axis.
        /// </summary>
        /// 
        /// <param name="radians">Rotation angle around X axis in radians.</param>
        /// 
        /// <returns>Returns rotation matrix to rotate an object around X axis.</returns>
        /// 
        public static Matrix3x3 CreateRotationX( float radians )
        {
            Matrix3x3 m = new Matrix3x3( );

            float cos = (float) System.Math.Cos( radians );
            float sin = (float) System.Math.Sin( radians );

            m.V11 = m.V22 = cos;
            m.V12 = -sin;
            m.V21 = sin;
            m.V00 = 1;

            return m;
        }

        /// <summary>
        /// Creates rotation matrix around Z axis.
        /// </summary>
        /// 
        /// <param name="radians">Rotation angle around Z axis in radians.</param>
        /// 
        /// <returns>Returns rotation matrix to rotate an object around Z axis.</returns>
        /// 
        public static Matrix3x3 CreateRotationZ( float radians )
        {
            Matrix3x3 m = new Matrix3x3( );

            float cos = (float) System.Math.Cos( radians );
            float sin = (float) System.Math.Sin( radians );

            m.V00 = m.V11 = cos;
            m.V01 = -sin;
            m.V10 = sin;
            m.V22 = 1;

            return m;
        }

        /// <summary>
        /// Creates rotation matrix to rotate an object around X, Y and Z axes.
        /// </summary>
        /// 
        /// <param name="yaw">Rotation angle around Y axis in radians.</param>
        /// <param name="pitch">Rotation angle around X axis in radians.</param>
        /// <param name="roll">Rotation angle around Z axis in radians.</param>
        /// 
        /// <returns>Returns rotation matrix to rotate an object around all 3 axes.</returns>
        /// 
        /// <remarks>
        /// <para><note>The routine assumes roll-pitch-yaw rotation order, when creating rotation
        /// matrix, i.e. an object is first rotated around Z axis, then around X axis and finally around
        /// Y axis.</note></para>
        /// </remarks>
        /// 
        public static Matrix3x3 CreateFromYawPitchRoll( float yaw, float pitch, float roll )
        {
            return ( CreateRotationY( yaw ) * CreateRotationX( pitch ) ) * CreateRotationZ( roll );
        }

        /// <summary>
        /// Extract rotation angles from the rotation matrix.
        /// </summary>
        /// 
        /// <param name="yaw">Extracted rotation angle around Y axis in radians.</param>
        /// <param name="pitch">Extracted rotation angle around X axis in radians.</param>
        /// <param name="roll">Extracted rotation angle around Z axis in radians.</param>
        /// 
        /// <remarks><para><note>The routine assumes roll-pitch-yaw rotation order when extracting rotation angle.
        /// Using extracted angles with the <see cref="CreateFromYawPitchRoll"/> should provide same rotation matrix.
        /// </note></para>
        /// 
        /// <para><note>The method assumes the provided matrix represent valid rotation matrix.</note></para>
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // assume we have a rotation matrix created like this
        /// float yaw   = 10.0f / 180 * Math.PI;
        /// float pitch = 30.0f / 180 * Math.PI;
        /// float roll  = 45.0f / 180 * Math.PI;
        /// 
        /// Matrix3x3 rotationMatrix = Matrix3x3.CreateFromYawPitchRoll( yaw, pitch, roll );
        /// // ...
        /// 
        /// // now somewhere in the code you may want to get rotation
        /// // angles back from a matrix assuming same rotation order
        /// float extractedYaw;
        /// float extractedPitch;
        /// float extractedRoll;
        /// 
        /// rotation.ExtractYawPitchRoll( out extractedYaw, out extractedPitch, out extractedRoll );
        /// </code>
        /// </remarks>
        /// 
        public void ExtractYawPitchRoll( out float yaw, out float pitch, out float roll )
        {
            yaw   = (float) Math.Atan2( V02, V22 );
            pitch = (float) Math.Asin( -V12 );
            roll  = (float) Math.Atan2( V10, V11 );
        }

        /// <summary>
        /// Creates a matrix from 3 rows specified as vectors.
        /// </summary>
        /// 
        /// <param name="row0">First row of the matrix to create.</param>
        /// <param name="row1">Second row of the matrix to create.</param>
        /// <param name="row2">Third row of the matrix to create.</param>
        /// 
        /// <returns>Returns a matrix from specified rows.</returns>
        /// 
        public static Matrix3x3 CreateFromRows( Vector3 row0, Vector3 row1, Vector3 row2 )
        {
            Matrix3x3 m = new Matrix3x3( );

            m.V00 = row0.X;
            m.V01 = row0.Y;
            m.V02 = row0.Z;

            m.V10 = row1.X;
            m.V11 = row1.Y;
            m.V12 = row1.Z;

            m.V20 = row2.X;
            m.V21 = row2.Y;
            m.V22 = row2.Z;

            return m;
        }

        /// <summary>
        /// Creates a matrix from 3 columns specified as vectors.
        /// </summary>
        /// 
        /// <param name="column0">First column of the matrix to create.</param>
        /// <param name="column1">Second column of the matrix to create.</param>
        /// <param name="column2">Third column of the matrix to create.</param>
        /// 
        /// <returns>Returns a matrix from specified columns.</returns>
        /// 
        public static Matrix3x3 CreateFromColumns( Vector3 column0, Vector3 column1, Vector3 column2 )
        {
            Matrix3x3 m = new Matrix3x3( );

            m.V00 = column0.X;
            m.V10 = column0.Y;
            m.V20 = column0.Z;

            m.V01 = column1.X;
            m.V11 = column1.Y;
            m.V21 = column1.Z;

            m.V02 = column2.X;
            m.V12 = column2.Y;
            m.V22 = column2.Z;

            return m;
        }

        /// <summary>
        /// Creates a diagonal matrix using the specified vector as diagonal elements.
        /// </summary>
        /// 
        /// <param name="vector">Vector to use for diagonal elements of the matrix.</param>
        /// 
        /// <returns>Returns a diagonal matrix.</returns>
        /// 
        public static Matrix3x3 CreateDiagonal( Vector3 vector )
        {
            Matrix3x3 m = new Matrix3x3( );

            m.V00 = vector.X;
            m.V11 = vector.Y;
            m.V22 = vector.Z;

            return m;
        }

        /// <summary>
        /// Get row of the matrix.
        /// </summary>
        /// 
        /// <param name="index">Row index to get, [0, 2].</param>
        /// 
        /// <returns>Returns specified row of the matrix as a vector.</returns>
        /// 
        /// <exception cref="ArgumentException">Invalid row index was specified.</exception>
        /// 
        public Vector3 GetRow( int index )
        {
            if ( ( index < 0 ) || ( index > 2 ) )
                throw new ArgumentException( "Invalid row index was specified.", "index" );

            return ( index == 0 ) ? new Vector3( V00, V01, V02 ) :
                   ( index == 1 ) ? new Vector3( V10, V11, V12 ) : new Vector3( V20, V21, V22 );
        }

        /// <summary>
        /// Get column of the matrix.
        /// </summary>
        /// 
        /// <param name="index">Column index to get, [0, 2].</param>
        /// 
        /// <returns>Returns specified column of the matrix as a vector.</returns>
        /// 
        /// <exception cref="ArgumentException">Invalid column index was specified.</exception>
        /// 
        public Vector3 GetColumn( int index )
        {
            if ( ( index < 0 ) || ( index > 2 ) )
                throw new ArgumentException( "Invalid column index was specified.", "index" );

            return ( index == 0 ) ? new Vector3( V00, V10, V20 ) :
                   ( index == 1 ) ? new Vector3( V01, V11, V21 ) : new Vector3( V02, V12, V22 );
        }

        /// <summary>
        /// Multiplies two specified matrices.
        /// </summary>
        /// 
        /// <param name="matrix1">Matrix to multiply.</param>
        /// <param name="matrix2">Matrix to multiply by.</param>
        /// 
        /// <returns>Return new matrix, which the result of multiplication of the two specified matrices.</returns>
        /// 
        public static Matrix3x3 operator *( Matrix3x3 matrix1, Matrix3x3 matrix2 )
        {
            Matrix3x3 m = new Matrix3x3( );

            m.V00 = matrix1.V00 * matrix2.V00 + matrix1.V01 * matrix2.V10 + matrix1.V02 * matrix2.V20;
            m.V01 = matrix1.V00 * matrix2.V01 + matrix1.V01 * matrix2.V11 + matrix1.V02 * matrix2.V21;
            m.V02 = matrix1.V00 * matrix2.V02 + matrix1.V01 * matrix2.V12 + matrix1.V02 * matrix2.V22;

            m.V10 = matrix1.V10 * matrix2.V00 + matrix1.V11 * matrix2.V10 + matrix1.V12 * matrix2.V20;
            m.V11 = matrix1.V10 * matrix2.V01 + matrix1.V11 * matrix2.V11 + matrix1.V12 * matrix2.V21;
            m.V12 = matrix1.V10 * matrix2.V02 + matrix1.V11 * matrix2.V12 + matrix1.V12 * matrix2.V22;

            m.V20 = matrix1.V20 * matrix2.V00 + matrix1.V21 * matrix2.V10 + matrix1.V22 * matrix2.V20;
            m.V21 = matrix1.V20 * matrix2.V01 + matrix1.V21 * matrix2.V11 + matrix1.V22 * matrix2.V21;
            m.V22 = matrix1.V20 * matrix2.V02 + matrix1.V21 * matrix2.V12 + matrix1.V22 * matrix2.V22;

            return m;
        }

        /// <summary>
        /// Multiplies two specified matrices.
        /// </summary>
        /// 
        /// <param name="matrix1">Matrix to multiply.</param>
        /// <param name="matrix2">Matrix to multiply by.</param>
        /// 
        /// <returns>Return new matrix, which the result of multiplication of the two specified matrices.</returns>
        /// 
        public static Matrix3x3 Multiply( Matrix3x3 matrix1, Matrix3x3 matrix2 )
        {
            return matrix1 * matrix2;
        }

        /// <summary>
        /// Adds corresponding components of two matrices.
        /// </summary>
        /// 
        /// <param name="matrix1">The matrix to add to.</param>
        /// <param name="matrix2">The matrix to add to the first matrix.</param>
        /// 
        /// <returns>Returns a matrix which components are equal to sum of corresponding
        /// components of the two specified matrices.</returns>
        ///
        public static Matrix3x3 operator +( Matrix3x3 matrix1, Matrix3x3 matrix2 )
        {
            Matrix3x3 m = new Matrix3x3( );

            m.V00 = matrix1.V00 + matrix2.V00;
            m.V01 = matrix1.V01 + matrix2.V01;
            m.V02 = matrix1.V02 + matrix2.V02;

            m.V10 = matrix1.V10 + matrix2.V10;
            m.V11 = matrix1.V11 + matrix2.V11;
            m.V12 = matrix1.V12 + matrix2.V12;

            m.V20 = matrix1.V20 + matrix2.V20;
            m.V21 = matrix1.V21 + matrix2.V21;
            m.V22 = matrix1.V22 + matrix2.V22;

            return m;
        }

        /// <summary>
        /// Adds corresponding components of two matrices.
        /// </summary>
        /// 
        /// <param name="matrix1">The matrix to add to.</param>
        /// <param name="matrix2">The matrix to add to the first matrix.</param>
        /// 
        /// <returns>Returns a matrix which components are equal to sum of corresponding
        /// components of the two specified matrices.</returns>
        ///
        public static Matrix3x3 Add( Matrix3x3 matrix1, Matrix3x3 matrix2 )
        {
            return matrix1 + matrix2;
        }

        /// <summary>
        /// Subtracts corresponding components of two matrices.
        /// </summary>
        /// 
        /// <param name="matrix1">The matrix to subtract from.</param>
        /// <param name="matrix2">The matrix to subtract from the first matrix.</param>
        /// 
        /// <returns>Returns a matrix which components are equal to difference of corresponding
        /// components of the two specified matrices.</returns>
        ///
        public static Matrix3x3 operator -( Matrix3x3 matrix1, Matrix3x3 matrix2 )
        {
            Matrix3x3 m = new Matrix3x3( );

            m.V00 = matrix1.V00 - matrix2.V00;
            m.V01 = matrix1.V01 - matrix2.V01;
            m.V02 = matrix1.V02 - matrix2.V02;

            m.V10 = matrix1.V10 - matrix2.V10;
            m.V11 = matrix1.V11 - matrix2.V11;
            m.V12 = matrix1.V12 - matrix2.V12;

            m.V20 = matrix1.V20 - matrix2.V20;
            m.V21 = matrix1.V21 - matrix2.V21;
            m.V22 = matrix1.V22 - matrix2.V22;

            return m;
        }

        /// <summary>
        /// Subtracts corresponding components of two matrices.
        /// </summary>
        /// 
        /// <param name="matrix1">The matrix to subtract from.</param>
        /// <param name="matrix2">The matrix to subtract from the first matrix.</param>
        /// 
        /// <returns>Returns a matrix which components are equal to difference of corresponding
        /// components of the two specified matrices.</returns>
        ///
        public static Matrix3x3 Subtract( Matrix3x3 matrix1, Matrix3x3 matrix2 )
        {
            return matrix1 - matrix2;
        }

        /// <summary>
        /// Multiplies specified matrix by the specified vector.
        /// </summary>
        /// 
        /// <param name="matrix">Matrix to multiply by vector.</param>
        /// <param name="vector">Vector to multiply matrix by.</param>
        /// 
        /// <returns>Returns new vector which is the result of multiplication of the specified matrix
        /// by the specified vector.</returns>
        ///
        public static Vector3 operator *( Matrix3x3 matrix, Vector3 vector )
        {
            return new Vector3(
                matrix.V00 * vector.X + matrix.V01 * vector.Y + matrix.V02 * vector.Z,
                matrix.V10 * vector.X + matrix.V11 * vector.Y + matrix.V12 * vector.Z,
                matrix.V20 * vector.X + matrix.V21 * vector.Y + matrix.V22 * vector.Z );
        }

        /// <summary>
        /// Multiplies specified matrix by the specified vector.
        /// </summary>
        /// 
        /// <param name="matrix">Matrix to multiply by vector.</param>
        /// <param name="vector">Vector to multiply matrix by.</param>
        /// 
        /// <returns>Returns new vector which is the result of multiplication of the specified matrix
        /// by the specified vector.</returns>
        ///
        public static Vector3 Multiply( Matrix3x3 matrix, Vector3 vector )
        {
            return matrix * vector;
        }

        /// <summary>
        /// Multiplies matrix by the specified factor.
        /// </summary>
        /// 
        /// <param name="matrix">Matrix to multiply.</param>
        /// <param name="factor">Factor to multiple the specified matrix by.</param>
        /// 
        /// <returns>Returns new matrix with all components equal to corresponding components of the
        /// specified matrix multiples by the specified factor.</returns>
        /// 
        public static Matrix3x3 operator *( Matrix3x3 matrix, float factor )
        {
            Matrix3x3 m = new Matrix3x3( );

            m.V00 = matrix.V00 * factor;
            m.V01 = matrix.V01 * factor;
            m.V02 = matrix.V02 * factor;

            m.V10 = matrix.V10 * factor;
            m.V11 = matrix.V11 * factor;
            m.V12 = matrix.V12 * factor;

            m.V20 = matrix.V20 * factor;
            m.V21 = matrix.V21 * factor;
            m.V22 = matrix.V22 * factor;

            return m;
        }

        /// <summary>
        /// Multiplies matrix by the specified factor.
        /// </summary>
        /// 
        /// <param name="matrix">Matrix to multiply.</param>
        /// <param name="factor">Factor to multiple the specified matrix by.</param>
        /// 
        /// <returns>Returns new matrix with all components equal to corresponding components of the
        /// specified matrix multiples by the specified factor.</returns>
        /// 
        public static Matrix3x3 Multiply( Matrix3x3 matrix, float factor )
        {
            return matrix * factor;
        }

        /// <summary>
        /// Adds specified value to all components of the specified matrix.
        /// </summary>
        /// 
        /// <param name="matrix">Matrix to add value to.</param>
        /// <param name="value">Value to add to all components of the specified matrix.</param>
        /// 
        /// <returns>Returns new matrix with all components equal to corresponding components of the
        /// specified matrix increased by the specified value.</returns>
        /// 
        public static Matrix3x3 operator +( Matrix3x3 matrix, float value )
        {
            Matrix3x3 m = new Matrix3x3( );

            m.V00 = matrix.V00 + value;
            m.V01 = matrix.V01 + value;
            m.V02 = matrix.V02 + value;

            m.V10 = matrix.V10 + value;
            m.V11 = matrix.V11 + value;
            m.V12 = matrix.V12 + value;

            m.V20 = matrix.V20 + value;
            m.V21 = matrix.V21 + value;
            m.V22 = matrix.V22 + value;

            return m;
        }

        /// <summary>
        /// Adds specified value to all components of the specified matrix.
        /// </summary>
        /// 
        /// <param name="matrix">Matrix to add value to.</param>
        /// <param name="value">Value to add to all components of the specified matrix.</param>
        /// 
        /// <returns>Returns new matrix with all components equal to corresponding components of the
        /// specified matrix increased by the specified value.</returns>
        /// 
        public static Matrix3x3 Add( Matrix3x3 matrix, float value )
        {
            return matrix + value;
        }

        /// <summary>
        /// Tests whether two specified matrices are equal.
        /// </summary>
        /// 
        /// <param name="matrix1">The left-hand matrix.</param>
        /// <param name="matrix2">The right-hand matrix.</param>
        /// 
        /// <returns>Returns <see langword="true"/> if the two matrices are equal or <see langword="false"/> otherwise.</returns>
        /// 
        public static bool operator ==( Matrix3x3 matrix1, Matrix3x3 matrix2 )
        {
            return (
                ( matrix1.V00 == matrix2.V00 ) &&
                ( matrix1.V01 == matrix2.V01 ) &&
                ( matrix1.V02 == matrix2.V02 ) &&

                ( matrix1.V10 == matrix2.V10 ) &&
                ( matrix1.V11 == matrix2.V11 ) &&
                ( matrix1.V12 == matrix2.V12 ) &&

                ( matrix1.V20 == matrix2.V20 ) &&
                ( matrix1.V21 == matrix2.V21 ) &&
                ( matrix1.V22 == matrix2.V22 )
            );
        }

        /// <summary>
        /// Tests whether two specified matrices are not equal.
        /// </summary>
        /// 
        /// <param name="matrix1">The left-hand matrix.</param>
        /// <param name="matrix2">The right-hand matrix.</param>
        /// 
        /// <returns>Returns <see langword="true"/> if the two matrices are not equal or <see langword="false"/> otherwise.</returns>
        /// 
        public static bool operator !=( Matrix3x3 matrix1, Matrix3x3 matrix2 )
        {
            return (
                ( matrix1.V00 != matrix2.V00 ) ||
                ( matrix1.V01 != matrix2.V01 ) ||
                ( matrix1.V02 != matrix2.V02 ) ||

                ( matrix1.V10 != matrix2.V10 ) ||
                ( matrix1.V11 != matrix2.V11 ) ||
                ( matrix1.V12 != matrix2.V12 ) ||

                ( matrix1.V20 != matrix2.V20 ) ||
                ( matrix1.V21 != matrix2.V21 ) ||
                ( matrix1.V22 != matrix2.V22 )
            );
        }

        /// <summary>
        /// Tests whether the matrix equals to the specified one.
        /// </summary>
        /// 
        /// <param name="matrix">The matrix to test equality with.</param>
        /// 
        /// <returns>Returns <see langword="true"/> if the two matrices are equal or <see langword="false"/> otherwise.</returns>
        /// 
        public bool Equals( Matrix3x3 matrix )
        {
            return ( this == matrix );
        }

        /// <summary>
        /// Tests whether the matrix equals to the specified object.
        /// </summary>
        /// 
        /// <param name="obj">The object to test equality with.</param>
        /// 
        /// <returns>Returns <see langword="true"/> if the matrix equals to the specified object or <see langword="false"/> otherwise.</returns>
        /// 
        public override bool Equals( Object obj )
        {
            if ( obj is Matrix3x3 )
            {
                return Equals( (Matrix3x3) obj );
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
            return
                V00.GetHashCode( ) + V01.GetHashCode( ) + V02.GetHashCode( ) +
                V10.GetHashCode( ) + V11.GetHashCode( ) + V12.GetHashCode( ) +
                V20.GetHashCode( ) + V21.GetHashCode( ) + V22.GetHashCode( );
        }

        /// <summary>
        /// Transpose the matrix, A<sup>T</sup>.
        /// </summary>
        /// 
        /// <returns>Return a matrix which equals to transposition of this matrix.</returns>
        /// 
        public Matrix3x3 Transpose( )
        {
            Matrix3x3 m = new Matrix3x3( );

            m.V00 = V00;
            m.V01 = V10;
            m.V02 = V20;

            m.V10 = V01;
            m.V11 = V11;
            m.V12 = V21;

            m.V20 = V02;
            m.V21 = V12;
            m.V22 = V22;

            return m;
        }

        /// <summary>
        /// Multiply the matrix by its transposition, A*A<sup>T</sup>.
        /// </summary>
        /// 
        /// <returns>Returns a matrix which is the result of multiplying this matrix by its transposition.</returns>
        ///
        public Matrix3x3 MultiplySelfByTranspose( )
        {
            Matrix3x3 m = new Matrix3x3( );

            m.V00 = V00 * V00 + V01 * V01 + V02 * V02;
            m.V10 = m.V01 = V00 * V10 + V01 * V11 + V02 * V12;
            m.V20 = m.V02 = V00 * V20 + V01 * V21 + V02 * V22;

            m.V11 = V10 * V10 + V11 * V11 + V12 * V12;
            m.V21 = m.V12 = V10 * V20 + V11 * V21 + V12 * V22;

            m.V22 = V20 * V20 + V21 * V21 + V22 * V22;

            return m;
        }

        /// <summary>
        /// Multiply transposition of this matrix by itself, A<sup>T</sup>*A.
        /// </summary>
        /// 
        /// <returns>Returns a matrix which is the result of multiplying this matrix's transposition by itself.</returns>
        ///
        public Matrix3x3 MultiplyTransposeBySelf( )
        {
            Matrix3x3 m = new Matrix3x3( );

            m.V00 = V00 * V00 + V10 * V10 + V20 * V20;
            m.V10 = m.V01 = V00 * V01 + V10 * V11 + V20 * V21;
            m.V20 = m.V02 = V00 * V02 + V10 * V12 + V20 * V22;

            m.V11 = V01 * V01 + V11 * V11 + V21 * V21;
            m.V21 = m.V12 = V01 * V02 + V11 * V12 + V21 * V22;

            m.V22 = V02 * V02 + V12 * V12 + V22 * V22;

            return m;
        }

        /// <summary>
        /// Calculate adjugate of the matrix, adj(A).
        /// </summary>
        /// 
        /// <returns>Returns adjugate of the matrix.</returns>
        /// 
        public Matrix3x3 Adjugate( )
        {
            Matrix3x3 m = new Matrix3x3( );

            m.V00 = V11 * V22 - V12 * V21;
            m.V01 = -( V01 * V22 - V02 * V21 );
            m.V02 = V01 * V12 - V02 * V11;

            m.V10 = -( V10 * V22 - V12 * V20 );
            m.V11 = V00 * V22 - V02 * V20;
            m.V12 = -( V00 * V12 - V02 * V10 );

            m.V20 = V10 * V21 - V11 * V20;
            m.V21 = -( V00 * V21 - V01 * V20 );
            m.V22 = V00 * V11 - V01 * V10;

            return m;
        }

        /// <summary>
        /// Calculate inverse of the matrix, A<sup>-1</sup>.
        /// </summary>
        /// 
        /// <returns>Returns inverse of the matrix.</returns>
        /// 
        /// <exception cref="ArgumentException">Cannot calculate inverse of the matrix since it is singular.</exception>
        /// 
        public Matrix3x3 Inverse( )
        {
            float det = Determinant;

            if ( det == 0 )
            {
                throw new ArgumentException( "Cannot calculate inverse of the matrix since it is singular." );
            }

            float detInv = 1 / det;
            Matrix3x3 m = Adjugate( );

            m.V00 *= detInv;
            m.V01 *= detInv;
            m.V02 *= detInv;

            m.V10 *= detInv;
            m.V11 *= detInv;
            m.V12 *= detInv;

            m.V20 *= detInv;
            m.V21 *= detInv;
            m.V22 *= detInv;

            return m;
        }

        /// <summary>
        /// Calculate pseudo inverse of the matrix, A<sup>+</sup>.
        /// </summary>
        /// 
        /// <returns>Returns pseudo inverse of the matrix.</returns>
        /// 
        /// <remarks><para>The pseudo inverse of the matrix is calculate through its <see cref="SVD"/>
        /// as V*E<sup>+</sup>*U<sup>T</sup>.</para></remarks>
        /// 
        public Matrix3x3 PseudoInverse( )
        {
            Matrix3x3 u, v;
            Vector3 e;

            SVD( out u, out e, out v );

            return v * CreateDiagonal( e.Inverse( ) ) * u.Transpose( );
        }

        /// <summary>
        /// Calculate Singular Value Decomposition (SVD) of the matrix, such as A=U*E*V<sup>T</sup>.
        /// </summary>
        /// 
        /// <param name="u">Output parameter which gets 3x3 U matrix.</param>
        /// <param name="e">Output parameter which gets diagonal elements of the E matrix.</param>
        /// <param name="v">Output parameter which gets 3x3 V matrix.</param>
        /// 
        /// <remarks><para>Having components U, E and V the source matrix can be reproduced using below code:
        /// <code>
        /// Matrix3x3 source = u * Matrix3x3.Diagonal( e ) * v.Transpose( );
        /// </code>
        /// </para></remarks>
        /// 
        public void SVD( out Matrix3x3 u, out Vector3 e, out Matrix3x3 v )
        {
            double[,] uArray = new double[3, 3]
            {
                { V00, V01, V02 },
                { V10, V11, V12 },
                { V20, V21, V22 }
            };
            double[,] vArray;
            double[] eArray;

            svd.svdcmp( uArray, out eArray, out vArray );

            // build U matrix
            u = new Matrix3x3( );
            u.V00 = (float) uArray[0, 0];
            u.V01 = (float) uArray[0, 1];
            u.V02 = (float) uArray[0, 2];
            u.V10 = (float) uArray[1, 0];
            u.V11 = (float) uArray[1, 1];
            u.V12 = (float) uArray[1, 2];
            u.V20 = (float) uArray[2, 0];
            u.V21 = (float) uArray[2, 1];
            u.V22 = (float) uArray[2, 2];

            // build V matrix
            v = new Matrix3x3( );
            v.V00 = (float) vArray[0, 0];
            v.V01 = (float) vArray[0, 1];
            v.V02 = (float) vArray[0, 2];
            v.V10 = (float) vArray[1, 0];
            v.V11 = (float) vArray[1, 1];
            v.V12 = (float) vArray[1, 2];
            v.V20 = (float) vArray[2, 0];
            v.V21 = (float) vArray[2, 1];
            v.V22 = (float) vArray[2, 2];

            // build E Vector3
            e = new Vector3( );
            e.X = (float) eArray[0];
            e.Y = (float) eArray[1];
            e.Z = (float) eArray[2];
        }
    }
}
