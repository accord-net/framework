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
    /// A structure representing 4x4 matrix.
    /// </summary>
    /// 
    /// <remarks><para>The structure incapsulates elements of a 4x4 matrix and
    /// provides some operations with it.</para></remarks>
    /// 
    [Serializable]
    public struct Matrix4x4
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
        /// Row 0 column 3 element of the matrix.
        /// </summary>
        public float V03;

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
        /// Row 1 column 3 element of the matrix.
        /// </summary>
        public float V13;

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
        /// Row 2 column 3 element of the matrix.
        /// </summary>
        public float V23;

        /// <summary>
        /// Row 3 column 0 element of the matrix.
        /// </summary>
        public float V30;
        /// <summary>
        /// Row 3 column 1 element of the matrix.
        /// </summary>
        public float V31;
        /// <summary>
        /// Row 3 column 2 element of the matrix.
        /// </summary>
        public float V32;
        /// <summary>
        /// Row 3 column 3 element of the matrix.
        /// </summary>
        public float V33;

        /// <summary>
        /// Provides an identity matrix with all diagonal elements set to 1.
        /// </summary>
        public static Matrix4x4 Identity
        {
            get
            {
                Matrix4x4 m = new Matrix4x4( );
                m.V00 = m.V11 = m.V22 = m.V33 = 1;
                return m;
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
                V00, V01, V02, V03, V10, V11, V12, V13, V20, V21, V22, V23, V30, V31, V32, V33
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
        public static Matrix4x4 CreateRotationY( float radians )
        {
            Matrix4x4 m = Matrix4x4.Identity;

            float cos = (float) System.Math.Cos( radians );
            float sin = (float) System.Math.Sin( radians );

            m.V00 = m.V22 = cos;
            m.V02 = sin;
            m.V20 = -sin;
            
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
        public static Matrix4x4 CreateRotationX( float radians )
        {
            Matrix4x4 m = Matrix4x4.Identity;

            float cos = (float) System.Math.Cos( radians );
            float sin = (float) System.Math.Sin( radians );

            m.V11 = m.V22 = cos;
            m.V12 = -sin;
            m.V21 = sin;

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
        public static Matrix4x4 CreateRotationZ( float radians )
        {
            Matrix4x4 m = Matrix4x4.Identity;

            float cos = (float) System.Math.Cos( radians );
            float sin = (float) System.Math.Sin( radians );

            m.V00 = m.V11 = cos;
            m.V01 = -sin;
            m.V10 = sin;

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
        public static Matrix4x4 CreateFromYawPitchRoll( float yaw, float pitch, float roll )
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
        /// Matrix4x4 rotationMatrix = Matrix3x3.CreateFromYawPitchRoll( yaw, pitch, roll );
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
        /// Creates 4x4 tranformation matrix from 3x3 rotation matrix.
        /// </summary>
        /// 
        /// <param name="rotationMatrix">Source 3x3 rotation matrix.</param>
        /// 
        /// <returns>Returns 4x4 rotation matrix.</returns>
        /// 
        /// <remarks><para>The source 3x3 rotation matrix is copied into the top left corner of the result 4x4 matrix,
        /// i.e. it represents 0th, 1st and 2nd row/column. The <see cref="V33"/> element is set to 1 and the rest
        /// elements of 3rd row and 3rd column are set to zeros.</para></remarks>
        /// 
        public static Matrix4x4 CreateFromRotation( Matrix3x3 rotationMatrix )
        {
            Matrix4x4 m = Matrix4x4.Identity;

            m.V00 = rotationMatrix.V00;
            m.V01 = rotationMatrix.V01;
            m.V02 = rotationMatrix.V02;

            m.V10 = rotationMatrix.V10;
            m.V11 = rotationMatrix.V11;
            m.V12 = rotationMatrix.V12;

            m.V20 = rotationMatrix.V20;
            m.V21 = rotationMatrix.V21;
            m.V22 = rotationMatrix.V22;

            return m;
        }

        /// <summary>
        /// Creates translation matrix for the specified movement amount.
        /// </summary>
        /// 
        /// <param name="position">Vector which set direction and amount of movement.</param>
        /// 
        /// <returns>Returns translation matrix.</returns>
        /// 
        /// <remarks><para>The specified vector is copied to the 3rd column of the result matrix.
        /// All diagonal elements are set to 1. The rest of matrix is initialized with zeros.</para></remarks>
        /// 
        public static Matrix4x4 CreateTranslation( Vector3 position )
        {
            Matrix4x4 m = Matrix4x4.Identity;

            m.V03 = position.X;
            m.V13 = position.Y;
            m.V23 = position.Z;

            return m;
        }

        /// <summary>
        /// Creates a view matrix for the specified camera position and target point.
        /// </summary>
        /// 
        /// <param name="cameraPosition">Position of camera.</param>
        /// <param name="cameraTarget">Target point towards which camera is pointing.</param>
        /// 
        /// <returns>Returns a view matrix.</returns>
        /// 
        /// <remarks><para>Camera's "up" vector is supposed to be (0, 1, 0).</para></remarks>
        /// 
        public static Matrix4x4 CreateLookAt( Vector3 cameraPosition, Vector3 cameraTarget )
        {
            Matrix4x4 m = new Matrix4x4( );

            Vector3 vector = cameraPosition - cameraTarget;
            vector.Normalize( );

            Vector3 vector2 = Vector3.Cross( new Vector3( 0, 1, 0 ), vector );
            vector2.Normalize( );

            Vector3 vector3 = Vector3.Cross( vector, vector2 );

            m.V00 = vector2.X;
            m.V01 = vector2.Y;
            m.V02 = vector2.Z;

            m.V10 = vector3.X;
            m.V11 = vector3.Y;
            m.V12 = vector3.Z;

            m.V20 = vector.X;
            m.V21 = vector.Y;
            m.V22 = vector.Z;

            m.V03 = -Vector3.Dot( cameraPosition, vector2 );
            m.V13 = -Vector3.Dot( cameraPosition, vector3 );
            m.V23 = -Vector3.Dot( cameraPosition, vector );
            m.V33 = 1;

            return m;
        }

        /// <summary>
        /// Creates a perspective projection matrix.
        /// </summary>
        /// 
        /// <param name="width">Width of the view volume at the near view plane.</param>
        /// <param name="height">Height of the view volume at the near view plane.</param>
        /// <param name="nearPlaneDistance">Distance to the near view plane.</param>
        /// <param name="farPlaneDistance">Distance to the far view plane.</param>
        /// 
        /// <returns>Return a perspective projection matrix.</returns>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">Both near and far view planes' distances must be greater than zero.</exception>
        /// <exception cref="ArgumentException">Near plane must be closer than the far plane.</exception>
        /// 
        public static Matrix4x4 CreatePerspective( float width, float height, float nearPlaneDistance, float farPlaneDistance )
        {
            if ( ( nearPlaneDistance <= 0 ) || ( farPlaneDistance <= 0 ) )
            {
                throw new ArgumentOutOfRangeException( "Both near and far view planes' distances must be greater than zero." );
            }

            if ( nearPlaneDistance >= farPlaneDistance )
            {
                throw new ArgumentException( "Near plane must be closer than the far plane." );
            }

            Matrix4x4 m = new Matrix4x4( );

            m.V00 = 2.0f * nearPlaneDistance / width;
            m.V11 = 2.0f * nearPlaneDistance / height;
            m.V22 = farPlaneDistance / ( nearPlaneDistance - farPlaneDistance );

            m.V32 = -1;
            m.V23 = ( nearPlaneDistance * farPlaneDistance ) / ( nearPlaneDistance - farPlaneDistance );

            return m;
        }

        /// <summary>
        /// Creates a matrix from 4 rows specified as vectors.
        /// </summary>
        /// 
        /// <param name="row0">First row of the matrix to create.</param>
        /// <param name="row1">Second row of the matrix to create.</param>
        /// <param name="row2">Third row of the matrix to create.</param>
        /// <param name="row3">Fourth row of the matrix to create.</param>
        /// 
        /// <returns>Returns a matrix from specified rows.</returns>
        /// 
        public static Matrix4x4 CreateFromRows( Vector4 row0, Vector4 row1, Vector4 row2, Vector4 row3 )
        {
            Matrix4x4 m = new Matrix4x4( );

            m.V00 = row0.X;
            m.V01 = row0.Y;
            m.V02 = row0.Z;
            m.V03 = row0.W;

            m.V10 = row1.X;
            m.V11 = row1.Y;
            m.V12 = row1.Z;
            m.V13 = row1.W;

            m.V20 = row2.X;
            m.V21 = row2.Y;
            m.V22 = row2.Z;
            m.V23 = row2.W;

            m.V30 = row3.X;
            m.V31 = row3.Y;
            m.V32 = row3.Z;
            m.V33 = row3.W;

            return m;
        }

        /// <summary>
        /// Creates a matrix from 4 columns specified as vectors.
        /// </summary>
        /// 
        /// <param name="column0">First column of the matrix to create.</param>
        /// <param name="column1">Second column of the matrix to create.</param>
        /// <param name="column2">Third column of the matrix to create.</param>
        /// <param name="column3">Fourth column of the matrix to create.</param>
        /// 
        /// <returns>Returns a matrix from specified columns.</returns>
        /// 
        public static Matrix4x4 CreateFromColumns( Vector4 column0, Vector4 column1, Vector4 column2, Vector4 column3 )
        {
            Matrix4x4 m = new Matrix4x4( );

            m.V00 = column0.X;
            m.V10 = column0.Y;
            m.V20 = column0.Z;
            m.V30 = column0.W;

            m.V01 = column1.X;
            m.V11 = column1.Y;
            m.V21 = column1.Z;
            m.V31 = column1.W;

            m.V02 = column2.X;
            m.V12 = column2.Y;
            m.V22 = column2.Z;
            m.V32 = column2.W;

            m.V03 = column3.X;
            m.V13 = column3.Y;
            m.V23 = column3.Z;
            m.V33 = column3.W;

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
        public static Matrix4x4 CreateDiagonal( Vector4 vector )
        {
            Matrix4x4 m = new Matrix4x4( );

            m.V00 = vector.X;
            m.V11 = vector.Y;
            m.V22 = vector.Z;
            m.V33 = vector.W;

            return m;
        }

        /// <summary>
        /// Get row of the matrix.
        /// </summary>
        /// 
        /// <param name="index">Row index to get, [0, 3].</param>
        /// 
        /// <returns>Returns specified row of the matrix as a vector.</returns>
        /// 
        /// <exception cref="ArgumentException">Invalid row index was specified.</exception>
        /// 
        public Vector4 GetRow( int index )
        {
            if ( ( index < 0 ) || ( index > 3 ) )
                throw new ArgumentException( "Invalid row index was specified.", "index" );

            return ( index == 0 ) ? new Vector4( V00, V01, V02, V03 ) :
                   ( index == 1 ) ? new Vector4( V10, V11, V12, V13 ) :
                   ( index == 2 ) ? new Vector4( V20, V21, V22, V23 ) : new Vector4( V30, V31, V32, V33 );
        }

        /// <summary>
        /// Get column of the matrix.
        /// </summary>
        /// 
        /// <param name="index">Column index to get, [0, 3].</param>
        /// 
        /// <returns>Returns specified column of the matrix as a vector.</returns>
        /// 
        /// <exception cref="ArgumentException">Invalid column index was specified.</exception>
        /// 
        public Vector4 GetColumn( int index )
        {
            if ( ( index < 0 ) || ( index > 3 ) )
                throw new ArgumentException( "Invalid column index was specified.", "index" );

            return ( index == 0 ) ? new Vector4( V00, V10, V20, V30 ) :
                   ( index == 1 ) ? new Vector4( V01, V11, V21, V31 ) :
                   ( index == 2 ) ? new Vector4( V02, V12, V22, V32 ) : new Vector4( V03, V13, V23, V33 );
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
        public static Matrix4x4 operator *( Matrix4x4 matrix1, Matrix4x4 matrix2 )
        {
            Matrix4x4 m = new Matrix4x4( );

            m.V00 = matrix1.V00 * matrix2.V00 + matrix1.V01 * matrix2.V10 + matrix1.V02 * matrix2.V20 + matrix1.V03 * matrix2.V30;
            m.V01 = matrix1.V00 * matrix2.V01 + matrix1.V01 * matrix2.V11 + matrix1.V02 * matrix2.V21 + matrix1.V03 * matrix2.V31;
            m.V02 = matrix1.V00 * matrix2.V02 + matrix1.V01 * matrix2.V12 + matrix1.V02 * matrix2.V22 + matrix1.V03 * matrix2.V32;
            m.V03 = matrix1.V00 * matrix2.V03 + matrix1.V01 * matrix2.V13 + matrix1.V02 * matrix2.V23 + matrix1.V03 * matrix2.V33;

            m.V10 = matrix1.V10 * matrix2.V00 + matrix1.V11 * matrix2.V10 + matrix1.V12 * matrix2.V20 + matrix1.V13 * matrix2.V30;
            m.V11 = matrix1.V10 * matrix2.V01 + matrix1.V11 * matrix2.V11 + matrix1.V12 * matrix2.V21 + matrix1.V13 * matrix2.V31;
            m.V12 = matrix1.V10 * matrix2.V02 + matrix1.V11 * matrix2.V12 + matrix1.V12 * matrix2.V22 + matrix1.V13 * matrix2.V32;
            m.V13 = matrix1.V10 * matrix2.V03 + matrix1.V11 * matrix2.V13 + matrix1.V12 * matrix2.V23 + matrix1.V13 * matrix2.V33;

            m.V20 = matrix1.V20 * matrix2.V00 + matrix1.V21 * matrix2.V10 + matrix1.V22 * matrix2.V20 + matrix1.V23 * matrix2.V30;
            m.V21 = matrix1.V20 * matrix2.V01 + matrix1.V21 * matrix2.V11 + matrix1.V22 * matrix2.V21 + matrix1.V23 * matrix2.V31;
            m.V22 = matrix1.V20 * matrix2.V02 + matrix1.V21 * matrix2.V12 + matrix1.V22 * matrix2.V22 + matrix1.V23 * matrix2.V32;
            m.V23 = matrix1.V20 * matrix2.V03 + matrix1.V21 * matrix2.V13 + matrix1.V22 * matrix2.V23 + matrix1.V23 * matrix2.V33;

            m.V30 = matrix1.V30 * matrix2.V00 + matrix1.V31 * matrix2.V10 + matrix1.V32 * matrix2.V20 + matrix1.V33 * matrix2.V30;
            m.V31 = matrix1.V30 * matrix2.V01 + matrix1.V31 * matrix2.V11 + matrix1.V32 * matrix2.V21 + matrix1.V33 * matrix2.V31;
            m.V32 = matrix1.V30 * matrix2.V02 + matrix1.V31 * matrix2.V12 + matrix1.V32 * matrix2.V22 + matrix1.V33 * matrix2.V32;
            m.V33 = matrix1.V30 * matrix2.V03 + matrix1.V31 * matrix2.V13 + matrix1.V32 * matrix2.V23 + matrix1.V33 * matrix2.V33;

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
        public static Matrix4x4 Multiply( Matrix4x4 matrix1, Matrix4x4 matrix2 )
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
        public static Matrix4x4 operator +( Matrix4x4 matrix1, Matrix4x4 matrix2 )
        {
            Matrix4x4 m = new Matrix4x4( );

            m.V00 = matrix1.V00 + matrix2.V00;
            m.V01 = matrix1.V01 + matrix2.V01;
            m.V02 = matrix1.V02 + matrix2.V02;
            m.V03 = matrix1.V03 + matrix2.V03;

            m.V10 = matrix1.V10 + matrix2.V10;
            m.V11 = matrix1.V11 + matrix2.V11;
            m.V12 = matrix1.V12 + matrix2.V12;
            m.V13 = matrix1.V13 + matrix2.V13;

            m.V20 = matrix1.V20 + matrix2.V20;
            m.V21 = matrix1.V21 + matrix2.V21;
            m.V22 = matrix1.V22 + matrix2.V22;
            m.V23 = matrix1.V23 + matrix2.V23;

            m.V30 = matrix1.V30 + matrix2.V30;
            m.V31 = matrix1.V31 + matrix2.V31;
            m.V32 = matrix1.V32 + matrix2.V32;
            m.V33 = matrix1.V33 + matrix2.V33;

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
        public static Matrix4x4 Add( Matrix4x4 matrix1, Matrix4x4 matrix2 )
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
        public static Matrix4x4 operator -( Matrix4x4 matrix1, Matrix4x4 matrix2 )
        {
            Matrix4x4 m = new Matrix4x4( );

            m.V00 = matrix1.V00 - matrix2.V00;
            m.V01 = matrix1.V01 - matrix2.V01;
            m.V02 = matrix1.V02 - matrix2.V02;
            m.V03 = matrix1.V03 - matrix2.V03;

            m.V10 = matrix1.V10 - matrix2.V10;
            m.V11 = matrix1.V11 - matrix2.V11;
            m.V12 = matrix1.V12 - matrix2.V12;
            m.V13 = matrix1.V13 - matrix2.V13;

            m.V20 = matrix1.V20 - matrix2.V20;
            m.V21 = matrix1.V21 - matrix2.V21;
            m.V22 = matrix1.V22 - matrix2.V22;
            m.V23 = matrix1.V23 - matrix2.V23;

            m.V30 = matrix1.V30 - matrix2.V30;
            m.V31 = matrix1.V31 - matrix2.V31;
            m.V32 = matrix1.V32 - matrix2.V32;
            m.V33 = matrix1.V33 - matrix2.V33;

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
        public static Matrix4x4 Subtract( Matrix4x4 matrix1, Matrix4x4 matrix2 )
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
        public static Vector4 operator *( Matrix4x4 matrix, Vector4 vector )
        {
            return new Vector4(
                matrix.V00 * vector.X + matrix.V01 * vector.Y + matrix.V02 * vector.Z + matrix.V03 * vector.W,
                matrix.V10 * vector.X + matrix.V11 * vector.Y + matrix.V12 * vector.Z + matrix.V13 * vector.W,
                matrix.V20 * vector.X + matrix.V21 * vector.Y + matrix.V22 * vector.Z + matrix.V23 * vector.W,
                matrix.V30 * vector.X + matrix.V31 * vector.Y + matrix.V32 * vector.Z + matrix.V33 * vector.W
                );
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
        public static Vector4 Multiply( Matrix4x4 matrix, Vector4 vector )
        {
            return matrix * vector;
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
        public static bool operator ==( Matrix4x4 matrix1, Matrix4x4 matrix2 )
        {
            return (
                ( matrix1.V00 == matrix2.V00 ) &&
                ( matrix1.V01 == matrix2.V01 ) &&
                ( matrix1.V02 == matrix2.V02 ) &&
                ( matrix1.V03 == matrix2.V03 ) &&

                ( matrix1.V10 == matrix2.V10 ) &&
                ( matrix1.V11 == matrix2.V11 ) &&
                ( matrix1.V12 == matrix2.V12 ) &&
                ( matrix1.V13 == matrix2.V13 ) &&

                ( matrix1.V20 == matrix2.V20 ) &&
                ( matrix1.V21 == matrix2.V21 ) &&
                ( matrix1.V22 == matrix2.V22 ) &&
                ( matrix1.V23 == matrix2.V23 ) &&

                ( matrix1.V30 == matrix2.V30 ) &&
                ( matrix1.V31 == matrix2.V31 ) &&
                ( matrix1.V32 == matrix2.V32 ) &&
                ( matrix1.V33 == matrix2.V33 )
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
        public static bool operator !=( Matrix4x4 matrix1, Matrix4x4 matrix2 )
        {
            return (
                ( matrix1.V00 != matrix2.V00 ) ||
                ( matrix1.V01 != matrix2.V01 ) ||
                ( matrix1.V02 != matrix2.V02 ) ||
                ( matrix1.V03 != matrix2.V03 ) ||

                ( matrix1.V10 != matrix2.V10 ) ||
                ( matrix1.V11 != matrix2.V11 ) ||
                ( matrix1.V12 != matrix2.V12 ) ||
                ( matrix1.V13 != matrix2.V13 ) ||

                ( matrix1.V20 != matrix2.V20 ) ||
                ( matrix1.V21 != matrix2.V21 ) ||
                ( matrix1.V22 != matrix2.V22 ) ||
                ( matrix1.V23 != matrix2.V23 ) ||

                ( matrix1.V30 != matrix2.V30 ) ||
                ( matrix1.V31 != matrix2.V31 ) ||
                ( matrix1.V32 != matrix2.V32 ) ||
                ( matrix1.V33 != matrix2.V33 )
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
        public bool Equals( Matrix4x4 matrix )
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
            if ( obj is Matrix4x4 )
            {
                return Equals( (Matrix4x4) obj );
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
                V00.GetHashCode( ) + V01.GetHashCode( ) + V02.GetHashCode( ) + V03.GetHashCode( ) +
                V10.GetHashCode( ) + V11.GetHashCode( ) + V12.GetHashCode( ) + V13.GetHashCode( ) +
                V20.GetHashCode( ) + V21.GetHashCode( ) + V22.GetHashCode( ) + V23.GetHashCode( ) +
                V30.GetHashCode( ) + V31.GetHashCode( ) + V32.GetHashCode( ) + V33.GetHashCode( );
        }
    }
}
