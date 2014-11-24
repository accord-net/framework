using System;
using System.Collections.Generic;
using AForge;
using MbUnit.Framework;

namespace AForge.Tests
{
    [TestFixture]
    public class IntPointTest
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
        public void EuclideanNormTest( int x, int y, double expectedNorm )
        {
            IntPoint point = new IntPoint( x, y );

            Assert.AreEqual( point.EuclideanNorm( ), expectedNorm );
        }

        [Test]
        [Row( 1, 2, 1, 2, true )]
        [Row( 1, 2, 3, 2, false )]
        [Row( 1, 2, 1, 4, false )]
        [Row( 1, 2, 3, 4, false )]
        public void EqualityOperatorTest( int x1, int y1, int x2, int y2, bool areEqual )
        {
            IntPoint point1 = new IntPoint( x1, y1 );
            IntPoint point2 = new IntPoint( x2, y2 );

            Assert.AreEqual( point1 == point2, areEqual );
        }

        [Test]
        [Row( 1, 2, 1, 2, false )]
        [Row( 1, 2, 3, 2, true )]
        [Row( 1, 2, 1, 4, true )]
        [Row( 1, 2, 3, 4, true )]
        public void InequalityOperatorTest( int x1, int y1, int x2, int y2, bool areNotEqual )
        {
            IntPoint point1 = new IntPoint( x1, y1 );
            IntPoint point2 = new IntPoint( x2, y2 );

            Assert.AreEqual( point1 != point2, areNotEqual );
        }
    }
}
