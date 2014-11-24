using System;
using AForge;
using AForge.Math;
using MbUnit.Framework;

namespace AForge.Math.Tests
{
    [TestFixture]
    public class Matrix3x3Test
    {
        private const float Epsilon = 0.000001f;

        private Matrix3x3 a1 = new Matrix3x3( );
        private Matrix3x3 a2 = new Matrix3x3( );

        public Matrix3x3Test( )
        {
            // prepare 1st argument
            a1.V00 = 1;
            a1.V01 = 2;
            a1.V02 = 3;

            a1.V10 = 3;
            a1.V11 = 2;
            a1.V12 = 1;

            a1.V20 = 1;
            a1.V21 = 3;
            a1.V22 = 2;

            // prepare 2nd argument
            a2.V00 = 2;
            a2.V01 = 1;
            a2.V02 = 3;

            a2.V10 = 1;
            a2.V11 = 3;
            a2.V12 = 2;

            a2.V20 = 3;
            a2.V21 = 2;
            a2.V22 = 1;
        }

        [Test]
        public void ToArrayTest( )
        {
            Matrix3x3 matrix = new Matrix3x3( );

            matrix.V00 = 1;
            matrix.V01 = 2;
            matrix.V02 = 3;

            matrix.V10 = 4;
            matrix.V11 = 5;
            matrix.V12 = 6;

            matrix.V20 = 7;
            matrix.V21 = 8;
            matrix.V22 = 9;

            float[] array = matrix.ToArray( );

            for ( int i = 0; i < 9; i++ )
            {
                Assert.AreEqual<float>( array[i], (float) ( i + 1 ) );
            }
        }

        [Test]
        public void CreateFromRowsTest( )
        {
            Vector3 row0 = new Vector3( 1, 2, 3 );
            Vector3 row1 = new Vector3( 4, 5, 6 );
            Vector3 row2 = new Vector3( 7, 8, 9 );
            Matrix3x3 matrix = Matrix3x3.CreateFromRows( row0, row1, row2 );

            float[] array = matrix.ToArray( );

            for ( int i = 0; i < 9; i++ )
            {
                Assert.AreEqual<float>( array[i], (float) ( i + 1 ) );
            }

            Assert.AreEqual<Vector3>( row0, matrix.GetRow( 0 ) );
            Assert.AreEqual<Vector3>( row1, matrix.GetRow( 1 ) );
            Assert.AreEqual<Vector3>( row2, matrix.GetRow( 2 ) );

            Assert.Throws<ArgumentException>( ( ) =>
            {
                matrix.GetRow( -1 );
            }
            );

            Assert.Throws<ArgumentException>( ( ) =>
            {
                matrix.GetRow( 3 );
            }
            );
        }

        [Test]
        public void CreateFromColumnsTest( )
        {
            Vector3 column0 = new Vector3( 1, 4, 7 );
            Vector3 column1 = new Vector3( 2, 5, 8 );
            Vector3 column2 = new Vector3( 3, 6, 9 );
            Matrix3x3 matrix = Matrix3x3.CreateFromColumns( column0, column1, column2 );

            float[] array = matrix.ToArray( );

            for ( int i = 0; i < 9; i++ )
            {
                Assert.AreEqual<float>( array[i], (float) ( i + 1 ) );
            }

            Assert.AreEqual<Vector3>( column0, matrix.GetColumn( 0 ) );
            Assert.AreEqual<Vector3>( column1, matrix.GetColumn( 1 ) );
            Assert.AreEqual<Vector3>( column2, matrix.GetColumn( 2 ) );

            Assert.Throws<ArgumentException>( ( ) =>
            {
                matrix.GetColumn( -1 );
            }
            );

            Assert.Throws<ArgumentException>( ( ) =>
            {
                matrix.GetColumn( 3 );
            }
            );
        }

        [Test]
        [Row( 0 )]
        [Row( 30 )]
        [Row( 45 )]
        [Row( 60 )]
        [Row( 90 )]
        [Row( -30 )]
        [Row( -90 )]
        [Row( -180 )]
        public void CreateRotationYTest( float angle )
        {
            float radians = (float) ( angle * System.Math.PI / 180 );
            Matrix3x3 matrix = Matrix3x3.CreateRotationY( radians );

            float sin = (float) System.Math.Sin( radians );
            float cos = (float) System.Math.Cos( radians );

            float[] expectedArray = new float[9]
            {
                cos, 0, sin, 0, 1, 0, -sin, 0, cos
            };

            CompareMatrixWithArray( matrix, expectedArray );
        }

        [Test]
        [Row( 0 )]
        [Row( 30 )]
        [Row( 45 )]
        [Row( 60 )]
        [Row( 90 )]
        [Row( -30 )]
        [Row( -90 )]
        [Row( -180 )]
        public void CreateRotationXTest( float angle )
        {
            float radians = (float) ( angle * System.Math.PI / 180 );
            Matrix3x3 matrix = Matrix3x3.CreateRotationX( radians );

            float sin = (float) System.Math.Sin( radians );
            float cos = (float) System.Math.Cos( radians );

            float[] expectedArray = new float[9]
            {
                1, 0, 0, 0, cos, -sin, 0, sin, cos
            };

            CompareMatrixWithArray( matrix, expectedArray );
        }

        [Test]
        [Row( 0 )]
        [Row( 30 )]
        [Row( 45 )]
        [Row( 60 )]
        [Row( 90 )]
        [Row( -30 )]
        [Row( -90 )]
        [Row( -180 )]
        public void CreateRotationZTest( float angle )
        {
            float radians = (float) ( angle * System.Math.PI / 180 );
            Matrix3x3 matrix = Matrix3x3.CreateRotationZ( radians );

            float sin = (float) System.Math.Sin( radians );
            float cos = (float) System.Math.Cos( radians );

            float[] expectedArray = new float[9]
            {
                cos, -sin, 0, sin, cos, 0, 0, 0, 1
            };

            CompareMatrixWithArray( matrix, expectedArray );
        }

        [Test]
        [Row( 0, 0, 0 )]
        [Row( 30, 45, 60 )]
        [Row( 45, 60, 30 )]
        [Row( 60, 30, 45 )]
        [Row( 90, 90, 90 )]
        [Row( -30, -60, -90 )]
        [Row( -90, -135, -180 )]
        [Row( -180, -30, -60 )]
        public void CreateFromYawPitchRollTest( float yaw, float pitch, float roll )
        {
            float radiansYaw   = (float) ( yaw   * System.Math.PI / 180 );
            float radiansPitch = (float) ( pitch * System.Math.PI / 180 );
            float radiansRoll  = (float) ( roll  * System.Math.PI / 180 );

            Matrix3x3 matrix = Matrix3x3.CreateFromYawPitchRoll( radiansYaw, radiansPitch, radiansRoll );

            Matrix3x3 xMatrix = Matrix3x3.CreateRotationX( radiansPitch );
            Matrix3x3 yMatrix = Matrix3x3.CreateRotationY( radiansYaw );
            Matrix3x3 zMatrix = Matrix3x3.CreateRotationZ( radiansRoll );

            Matrix3x3 rotationMatrix = ( yMatrix * xMatrix ) * zMatrix;

            CompareMatrixWithArray( matrix, rotationMatrix.ToArray( ) );
        }

        [Test]
        [Row( 0, 0, 0 )]
        [Row( 30, 45, 60 )]
        [Row( 45, 60, 30 )]
        [Row( 60, 30, 45 )]
        [Row( -30, -60, -90 )]
        public void ExtractYawPitchRollTest( float yaw, float pitch, float roll )
        {
            float radiansYaw   = (float) ( yaw   * System.Math.PI / 180 );
            float radiansPitch = (float) ( pitch * System.Math.PI / 180 );
            float radiansRoll  = (float) ( roll  * System.Math.PI / 180 );

            Matrix3x3 matrix = Matrix3x3.CreateFromYawPitchRoll( radiansYaw, radiansPitch, radiansRoll );

            float extractedYaw;
            float extractedPitch;
            float extractedRoll;

            matrix.ExtractYawPitchRoll( out extractedYaw, out extractedPitch, out extractedRoll );

            Assert.AreApproximatelyEqual<float, float>( radiansYaw,   extractedYaw,   Epsilon );
            Assert.AreApproximatelyEqual<float, float>( radiansPitch, extractedPitch, Epsilon );
            Assert.AreApproximatelyEqual<float, float>( radiansRoll,  extractedRoll,  Epsilon );
        }

        [Test]
        [Row( 1, 2, 3 )]
        [Row( -1, -2, -3 )]
        public void CreateDiagonalTest( float v00, float v11, float v22 )
        {
            Vector3 diagonal = new Vector3( v00, v11, v22 );
            Matrix3x3 matrix = Matrix3x3.CreateDiagonal( diagonal );

            float[] expectedArray = new float[9] { v00, 0, 0, 0, v11, 0, 0, 0, v22 };

            CompareMatrixWithArray( matrix, expectedArray );
        }

        [Test]
        [Row( 1, 1, 0, 0, 0, 1, 0, 0, 0, 1 )]
        [Row( 0, 1, 0, 0, 0, 1, 0, 1, 0, 0 )]
        [Row( 0, 1, 1, 1, 1, 1, 1, 1, 1, 1 )]
        [Row( -3, 1, 3, 2, 2, 2, 1, 2, 1, 1 )]
        public void DeterminantTest( float expectedDeterminant,
            float v00, float v01, float v02,
            float v10, float v11, float v12,
            float v20, float v21, float v22 )
        {
            Matrix3x3 matrix = new Matrix3x3( );

            matrix.V00 = v00;
            matrix.V01 = v01;
            matrix.V02 = v02;

            matrix.V10 = v10;
            matrix.V11 = v11;
            matrix.V12 = v12;

            matrix.V20 = v20;
            matrix.V21 = v21;
            matrix.V22 = v22;

            Assert.AreEqual<float>( expectedDeterminant, matrix.Determinant );
        }

        [Test]
        [Row( 1, 0, 0, 0, 1, 0, 0, 0, 1)]
        [Row( 1, 0, 0, 0, 1, 0, 1, 0, 0, ExpectedException = typeof( ArgumentException ) )]
        [Row( 2, 0, 0, 0, 4, 0, 0, 0, 3 )]
        [Row( 1, 4, 2, 2, 2, 1, 2, 1, 1 )]
        public void InverseTest( float v00, float v01, float v02, float v10, float v11, float v12, float v20, float v21, float v22 )
        {
            Matrix3x3 matrix = new Matrix3x3( );

            matrix.V00 = v00;
            matrix.V01 = v01;
            matrix.V02 = v02;

            matrix.V10 = v10;
            matrix.V11 = v11;
            matrix.V12 = v12;

            matrix.V20 = v20;
            matrix.V21 = v21;
            matrix.V22 = v22;

            Matrix3x3 inverse = matrix.Inverse( );
            Matrix3x3 identity = matrix * inverse;

            Assert.AreEqual<bool>( true, ApproximateEquals( identity, Matrix3x3.Identity ) );
        }

        [Test]
        public void AddMatricesTest( )
        {
            Matrix3x3 expectedResult = new Matrix3x3( );

            expectedResult.V00 = 3;
            expectedResult.V01 = 3;
            expectedResult.V02 = 6;

            expectedResult.V10 = 4;
            expectedResult.V11 = 5;
            expectedResult.V12 = 3;

            expectedResult.V20 = 4;
            expectedResult.V21 = 5;
            expectedResult.V22 = 3;

            Matrix3x3 result = a1 + a2;

            Assert.AreEqual<bool>( true, ApproximateEquals( result, expectedResult ) );
        }

        [Test]
        public void SubtractMatricesTest( )
        {
            Matrix3x3 expectedResult = new Matrix3x3( );

            expectedResult.V00 = -1;
            expectedResult.V01 = 1;
            expectedResult.V02 = 0;

            expectedResult.V10 = 2;
            expectedResult.V11 = -1;
            expectedResult.V12 = -1;

            expectedResult.V20 = -2;
            expectedResult.V21 = 1;
            expectedResult.V22 = 1;

            Matrix3x3 result = a1 - a2;

            Assert.AreEqual<bool>( true, ApproximateEquals( result, expectedResult ) );
        }

        [Test]
        public void MultiplyMatricesTest( )
        {
            Matrix3x3 expectedResult = new Matrix3x3( );

            expectedResult.V00 = 13;
            expectedResult.V01 = 13;
            expectedResult.V02 = 10;

            expectedResult.V10 = 11;
            expectedResult.V11 = 11;
            expectedResult.V12 = 14;

            expectedResult.V20 = 11;
            expectedResult.V21 = 14;
            expectedResult.V22 = 11;

            Matrix3x3 result = a1 * a2;

            Assert.AreEqual<bool>( true, ApproximateEquals( result, expectedResult ) );
        }

        private void CompareMatrixWithArray( Matrix3x3 matrix, float[] array )
        {
            float[] matrixArray = matrix.ToArray( );

            for ( int i = 0; i < 9; i++ )
            {
                Assert.AreEqual<float>( matrixArray[i], array[i] );
            }
        }

        private bool ApproximateEquals( Matrix3x3 matrix1, Matrix3x3 matrix2 )
        {
            // TODO: better algorithm should be put into the framework actually
            return (
                ( System.Math.Abs( matrix1.V00 - matrix2.V00 ) <= Epsilon ) &&
                ( System.Math.Abs( matrix1.V01 - matrix2.V01 ) <= Epsilon ) &&
                ( System.Math.Abs( matrix1.V02 - matrix2.V02 ) <= Epsilon ) &&

                ( System.Math.Abs( matrix1.V10 - matrix2.V10 ) <= Epsilon ) &&
                ( System.Math.Abs( matrix1.V11 - matrix2.V11 ) <= Epsilon ) &&
                ( System.Math.Abs( matrix1.V12 - matrix2.V12 ) <= Epsilon ) &&

                ( System.Math.Abs( matrix1.V20 - matrix2.V20 ) <= Epsilon ) &&
                ( System.Math.Abs( matrix1.V21 - matrix2.V21 ) <= Epsilon ) &&
                ( System.Math.Abs( matrix1.V22 - matrix2.V22 ) <= Epsilon )
            );
        }
    }
}
