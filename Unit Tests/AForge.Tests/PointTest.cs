using System;
using System.Collections.Generic;
using AForge;
using MbUnit.Framework;

namespace AForge.Tests
{
    [TestFixture]
    public class PointTest
    {
        [Test]
        [Row( 0, 0, 0 )]
        [Row( 0, 1, 1 )]
        [Row( 0, 10, 10 )]
        [Row( 10, 0, 10 )]
        [Row( 3, 4, 5 )]
        [Row( -3, 4, 5 )]
        [Row( 3, -4, 5 )]
        [Row( -3, -4, 5 )]
        [Row( 0.3, 0.4, 0.5 )]
        public void EuclideanNormTest( float x, float y, float expectedNorm )
        {
            Point point = new Point( x, y );

            Assert.AreEqual( point.EuclideanNorm( ), expectedNorm );
        }

        [Test]
        [Row( 0, 0, 0, 0 )]
        [Row( 1, 2, 1, 2 )]
        [Row( -1, -2, -1, -2 )]
        [Row( 1.4, 3.3, 1, 3 )]
        [Row( 1.6, 3.7, 2, 4 )]
        [Row( -1.6, -3.3, -2, -3 )]
        [Row( -1.5, 1.5, -2, 2 )]
        [Row( -2.5, 2.5, -2, 2 )]
        public void RoundTest( float x, float y, int expectedX, int expectedY )
        {
            Point point = new Point( x, y );
            IntPoint iPoint = new IntPoint( expectedX, expectedY );

            Assert.AreEqual( iPoint, point.Round( ) );
        }

        [Test]
        [Row( 1.1, 2.2, 1.1, 2.2, true )]
        [Row( 1.1, 2.2, 3.3, 2.2, false )]
        [Row( 1.1, 2.2, 1.1, 4.4, false )]
        [Row( 1.1, 2.2, 3.3, 4.4, false )]
        public void EqualityOperatorTest( float x1, float y1, float x2, float y2, bool areEqual )
        {
            Point point1 = new Point( x1, y1 );
            Point point2 = new Point( x2, y2 );

            Assert.AreEqual( point1 == point2, areEqual );
        }

        [Test]
        [Row( 1.1, 2.2, 1.1, 2.2, false )]
        [Row( 1.1, 2.2, 3.3, 2.2, true )]
        [Row( 1.1, 2.2, 1.1, 4.4, true )]
        [Row( 1.1, 2.2, 3.3, 4.4, true )]
        public void InequalityOperatorTest( float x1, float y1, float x2, float y2, bool areNotEqual )
        {
            Point point1 = new Point( x1, y1 );
            Point point2 = new Point( x2, y2 );

            Assert.AreEqual( point1 != point2, areNotEqual );
        }
    }
}
