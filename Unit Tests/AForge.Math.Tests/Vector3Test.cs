using System;
using AForge;
using AForge.Math;
using MbUnit.Framework;

namespace AForge.Math.Tests
{
    [TestFixture]
    public class Vector3Test
    {
        [Test]
        public void ConstructorTest( )
        {
            Vector3 v1 = new Vector3( 1, 2, 3 );
            Vector3 v2 = new Vector3( -1, -2, -3 );
            Vector3 v3 = new Vector3( 7 );

            Assert.AreEqual<float>( v1.X, 1 );
            Assert.AreEqual<float>( v1.Y, 2 );
            Assert.AreEqual<float>( v1.Z, 3 );

            Assert.AreEqual<float>( v2.X, -1 );
            Assert.AreEqual<float>( v2.Y, -2 );
            Assert.AreEqual<float>( v2.Z, -3 );

            Assert.AreEqual<float>( v3.X, 7 );
            Assert.AreEqual<float>( v3.Y, 7 );
            Assert.AreEqual<float>( v3.Z, 7 );
        }

        [Test]
        [Row( 0, 0, 0, 0, 0, 0, 0 )]
        [Row( 0, 7, 7, 0, 7, 0, 1 )]
        [Row( 0, 0, 7, 0, 7, 0, 2 )]
        [Row( 0, 7, 0, 0, 7, 0, 1 )]
        [Row( 7, 0, 7, 0, 7, 1, 0 )]
        [Row( 5, 7, 9, 5, 9, 0, 2 )]
        [Row( 5, 9, 7, 5, 9, 0, 1 )]
        [Row( 7, 5, 9, 5, 9, 1, 2 )]
        [Row( 7, 9, 5, 5, 9, 2, 1 )]
        [Row( 9, 5, 7, 5, 9, 1, 0 )]
        [Row( 9, 7, 5, 5, 9, 2, 0 )]
        public void MinMaxTest( float x, float y, float z, float expectedMin, float expectedMax,
            int expectedMinIndex, int expectedMaxIndex )
        {
            Vector3 vector = new Vector3( x, y, z );

            Assert.AreEqual<float>( vector.Min, expectedMin );
            Assert.AreEqual<float>( vector.Max, expectedMax );

            Assert.AreEqual<int>( vector.MinIndex, expectedMinIndex );
            Assert.AreEqual<int>( vector.MaxIndex, expectedMaxIndex );
        }

        [Test]
        [Row( 0, 0, 0, 0 )]
        [Row( 1, 0, 0, 1 )]
        [Row( 0, 2, 0, 2 )]
        [Row( 0, 0, 3, 3 )]
        [Row( 3, 4, 0, 5 )]
        [Row( -3, -4, 0, 5 )]
        public void NormTest( float x, float y, float z, float expectedNorm )
        {
            Vector3 vector = new Vector3( x, y, z );

            float norm = vector.Norm;

            Assert.AreEqual<float>( norm, expectedNorm );
            Assert.AreEqual<float>( norm * norm, vector.Square );
        }

        [Test]
        [Row( 1, 2, 3, 1, 2, 3, true )]
        [Row( -1, -2, -3, -1, -2, -3, true )]
        [Row( -1, -2, -3, -1, -2, 3, false )]
        public void EqualityTest( float x1, float y1, float z1, float x2, float y2, float z2, bool expected )
        {
            Vector3 vector1 = new Vector3( x1, y1, z1 );
            Vector3 vector2 = new Vector3( x2, y2, z2 );

            Assert.AreEqual<bool>( vector1 == vector2, expected );
            Assert.AreEqual<bool>( vector1 != vector2, !expected );

            Assert.AreEqual<bool>( vector1.Equals( vector2 ), expected );
            Assert.AreEqual<bool>( vector1.Equals( (object) vector2 ), expected );
        }

        [Test]
        [Row( 1, 2, 3, 4, 5, 6, 5, 7, 9 )]
        [Row( 1, 2, 3, -4, -5, -6, -3, -3, -3 )]
        public void AdditionTest( float x1, float y1, float z1, float x2, float y2, float z2,
            float expectedX, float expectedY, float expectedZ )
        {
            Vector3 vector1 = new Vector3( x1, y1, z1 );
            Vector3 vector2 = new Vector3( x2, y2, z2 );
            Vector3 expectedResult = new Vector3( expectedX, expectedY, expectedZ );

            Vector3 result1 = vector1 + vector2;
            Vector3 result2 = Vector3.Add( vector1, vector2 );

            Assert.AreEqual<bool>( expectedResult == result1, true );
            Assert.AreEqual<bool>( expectedResult == result2, true );
        }

        [Test]
        [Row( 1, 2, 3, 4, 5, 6, 7 )]
        [Row( 1, 2, 3, -4, -3, -2, -1 )]
        public void AdditionWithConstTest( float x, float y, float z, float value,
            float expectedX, float expectedY, float expectedZ )
        {
            Vector3 vector = new Vector3( x, y, z );
            Vector3 expectedResult = new Vector3( expectedX, expectedY, expectedZ );

            Vector3 result1 = vector + value;
            Vector3 result2 = Vector3.Add( vector, value );

            Assert.AreEqual<bool>( expectedResult == result1, true );
            Assert.AreEqual<bool>( expectedResult == result2, true );
        }

        [Test]
        [Row( 1, 2, 3, 4, 5, 6, -3, -3, -3 )]
        [Row( 1, 2, 3, -4, -5, -6, 5, 7, 9 )]
        public void SubtractionTest( float x1, float y1, float z1, float x2, float y2, float z2,
            float expectedX, float expectedY, float expectedZ )
        {
            Vector3 vector1 = new Vector3( x1, y1, z1 );
            Vector3 vector2 = new Vector3( x2, y2, z2 );
            Vector3 expectedResult = new Vector3( expectedX, expectedY, expectedZ );

            Vector3 result1 = vector1 - vector2;
            Vector3 result2 = Vector3.Subtract( vector1, vector2 );

            Assert.AreEqual<bool>( expectedResult == result1, true );
            Assert.AreEqual<bool>( expectedResult == result2, true );
        }

        [Test]
        [Row( 1, 2, 3, 4, -3, -2, -1 )]
        [Row( 1, 2, 3, -4, 5, 6, 7 )]
        public void SubtractionWithConstTest( float x, float y, float z, float value,
            float expectedX, float expectedY, float expectedZ )
        {
            Vector3 vector = new Vector3( x, y, z );
            Vector3 expectedResult = new Vector3( expectedX, expectedY, expectedZ );

            Vector3 result1 = vector - value;
            Vector3 result2 = Vector3.Subtract( vector, value );

            Assert.AreEqual<bool>( expectedResult == result1, true );
            Assert.AreEqual<bool>( expectedResult == result2, true );
        }

        [Test]
        [Row( 1, 2, 3, 4, 5, 6, 4, 10, 18 )]
        [Row( 1, 2, 3, -4, -5, -6, -4, -10, -18 )]
        public void MultiplicationTest( float x1, float y1, float z1, float x2, float y2, float z2,
            float expectedX, float expectedY, float expectedZ )
        {
            Vector3 vector1 = new Vector3( x1, y1, z1 );
            Vector3 vector2 = new Vector3( x2, y2, z2 );
            Vector3 expectedResult = new Vector3( expectedX, expectedY, expectedZ );

            Vector3 result1 = vector1 * vector2;
            Vector3 result2 = Vector3.Multiply( vector1, vector2 );

            Assert.AreEqual<bool>( expectedResult == result1, true );
            Assert.AreEqual<bool>( expectedResult == result2, true );
        }

        [Test]
        [Row( 1, 2, 3, 4, 4, 8, 12 )]
        [Row( 1, 2, 3, -4, -4, -8, -12 )]
        public void MultiplicationWithConstTest( float x, float y, float z, float value,
            float expectedX, float expectedY, float expectedZ )
        {
            Vector3 vector = new Vector3( x, y, z );
            Vector3 expectedResult = new Vector3( expectedX, expectedY, expectedZ );

            Vector3 result1 = vector * value;
            Vector3 result2 = Vector3.Multiply( vector, value );

            Assert.AreEqual<bool>( expectedResult == result1, true );
            Assert.AreEqual<bool>( expectedResult == result2, true );
        }

        [Test]
        [Row( 1, 2, 3, 1, 4, 2, 1, 0.5, 1.5 )]
        [Row( 1, 2, 3, -1, -4, -2, -1, -0.5, -1.5 )]
        public void DivisionTest( float x1, float y1, float z1, float x2, float y2, float z2,
            float expectedX, float expectedY, float expectedZ )
        {
            Vector3 vector1 = new Vector3( x1, y1, z1 );
            Vector3 vector2 = new Vector3( x2, y2, z2 );
            Vector3 expectedResult = new Vector3( expectedX, expectedY, expectedZ );

            Vector3 result1 = vector1 / vector2;
            Vector3 result2 = Vector3.Divide( vector1, vector2 );

            Assert.AreEqual<bool>( expectedResult == result1, true );
            Assert.AreEqual<bool>( expectedResult == result2, true );
        }

        [Test]
        [Row( 1, 2, 3, 2, 0.5, 1, 1.5 )]
        [Row( 1, 2, 3, -2, -0.5, -1, -1.5 )]
        public void DivisionWithConstTest( float x, float y, float z, float value,
            float expectedX, float expectedY, float expectedZ )
        {
            Vector3 vector = new Vector3( x, y, z );
            Vector3 expectedResult = new Vector3( expectedX, expectedY, expectedZ );

            Vector3 result1 = vector / value;
            Vector3 result2 = Vector3.Divide( vector, value );

            Assert.AreEqual<bool>( expectedResult == result1, true );
            Assert.AreEqual<bool>( expectedResult == result2, true );
        }

        [Test]
        [Row( 1, 0, 0, 1, 0, 0 )]
        [Row( 0, 1, 0, 0, 1, 0 )]
        [Row( 0, 0, 1, 0, 0, 1 )]
        [Row( 3, 4, 0, 0.6, 0.8, 0 )]
        [Row( 3, 0, 4, 0.6, 0, 0.8 )]
        public void NormalizeTest( float x, float y, float z, float expectedX, float expectedY, float expectedZ )
        {
            Vector3 vector = new Vector3( x, y, z );
            Vector3 expectedResult = new Vector3( expectedX, expectedY, expectedZ );

            float norm1 = vector.Norm;
            float norm2 = vector.Normalize( );

            Assert.AreEqual<bool>( expectedResult == vector, true );
            Assert.AreEqual<float>( norm1, norm2 );
        }

        [Test]
        [Row( 1, 0, 0, 1, 0, 0 )]
        [Row( 0, 0, 0, 0, 0, 0 )]
        [Row( 2, 4, 8, 0.5, 0.25, 0.125 )]
        [Row( -2, -4, -8, -0.5, -0.25, -0.125 )]
        [Row( 0.5, 0.25, 0.125, 2, 4, 8 )]
        public void InverseTest( float x, float y, float z, float expectedX, float expectedY, float expectedZ )
        {
            Vector3 vector = new Vector3( x, y, z );
            Vector3 expectedResult = new Vector3( expectedX, expectedY, expectedZ );

            Vector3 result = vector.Inverse( );

            Assert.AreEqual<bool>( expectedResult == result, true );
        }

        [Test]
        [Row( 1, 2, 3, 0, 0, 0, 0 )]
        [Row( 1, 2, 3, 1, 1, 1, 6 )]
        [Row( 1, 2, 3, 3, 2, 1, 10 )]
        [Row( 1, 2, 3, -3, -2, -1, -10 )]
        public void DotTest( float x1, float y1, float z1, float x2, float y2, float z2, float expectedResult )
        {
            Vector3 vector1 = new Vector3( x1, y1, z1 );
            Vector3 vector2 = new Vector3( x2, y2, z2 );

            Assert.AreEqual<float>( Vector3.Dot( vector1, vector2 ), expectedResult );
        }

        [Test]
        [Row( 1, 0, 0, 0, 1, 0, 0, 0, 1 )]
        [Row( 1, 1, 1, 1, 1, 1, 0, 0, 0 )]
        [Row( 1, 2, 3, 4, 5, 6, -3, 6, -3 )]
        public void CrossTest( float x1, float y1, float z1, float x2, float y2, float z2,
            float expectedX, float expectedY, float expectedZ )
        {
            Vector3 vector1 = new Vector3( x1, y1, z1 );
            Vector3 vector2 = new Vector3( x2, y2, z2 );
            Vector3 expectedResult = new Vector3( expectedX, expectedY, expectedZ );

            Assert.AreEqual<bool>( Vector3.Cross( vector1, vector2 ) == expectedResult, true );
        }
    }
}
