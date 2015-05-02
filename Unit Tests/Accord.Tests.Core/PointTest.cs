using NUnit.Framework;

namespace AForge.Tests
{
    [TestFixture]
    public class PointTest
    {
        [TestCase( 0, 0, 0 )]
        [TestCase( 0, 1, 1 )]
        [TestCase( 0, 10, 10 )]
        [TestCase( 10, 0, 10 )]
        [TestCase( 3, 4, 5 )]
        [TestCase( -3, 4, 5 )]
        [TestCase( 3, -4, 5 )]
        [TestCase( -3, -4, 5 )]
        [TestCase( 0.3f, 0.4f, 0.5f )]
        public void EuclideanNormTest( float x, float y, float expectedNorm )
        {
            Point point = new Point( x, y );

            Assert.AreEqual( point.EuclideanNorm( ), expectedNorm );
        }

        [TestCase( 0, 0, 0, 0 )]
        [TestCase( 1, 2, 1, 2 )]
        [TestCase( -1, -2, -1, -2 )]
        [TestCase( 1.4f, 3.3f, 1, 3 )]
        [TestCase( 1.6f, 3.7f, 2, 4 )]
        [TestCase( -1.6f, -3.3f, -2, -3 )]
        [TestCase( -1.5f, 1.5f, -2, 2 )]
        [TestCase( -2.5f, 2.5f, -2, 2 )]
        public void RoundTest( float x, float y, int expectedX, int expectedY )
        {
            Point point = new Point( x, y );
            IntPoint iPoint = new IntPoint( expectedX, expectedY );

            Assert.AreEqual( iPoint, point.Round( ) );
        }

        [TestCase( 1.1f, 2.2f, 1.1f, 2.2f, true )]
        [TestCase( 1.1f, 2.2f, 3.3f, 2.2f, false )]
        [TestCase( 1.1f, 2.2f, 1.1f, 4.4f, false )]
        [TestCase( 1.1f, 2.2f, 3.3f, 4.4f, false )]
        public void EqualityOperatorTest( float x1, float y1, float x2, float y2, bool areEqual )
        {
            Point point1 = new Point( x1, y1 );
            Point point2 = new Point( x2, y2 );

            Assert.AreEqual( point1 == point2, areEqual );
        }

        [TestCase( 1.1f, 2.2f, 1.1f, 2.2f, false )]
        [TestCase( 1.1f, 2.2f, 3.3f, 2.2f, true )]
        [TestCase( 1.1f, 2.2f, 1.1f, 4.4f, true )]
        [TestCase( 1.1f, 2.2f, 3.3f, 4.4f, true )]
        public void InequalityOperatorTest( float x1, float y1, float x2, float y2, bool areNotEqual )
        {
            Point point1 = new Point( x1, y1 );
            Point point2 = new Point( x2, y2 );

            Assert.AreEqual( point1 != point2, areNotEqual );
        }
    }
}
