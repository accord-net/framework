using NUnit.Framework;

namespace AForge.Tests
{
    [TestFixture]
    public class IntRangeTest
    {
        [TestCase( 0, 1, 1, 2, true )]
        [TestCase( 0, 1, 2, 3, false )]
        [TestCase( 0, 10, 2, 4, true )]
        [TestCase( 0, 10, 5, 15, true )]
        [TestCase( 0, 10, -5, 5, true )]
        [TestCase( 2, 4, 0, 10, true )]
        [TestCase( 5, 15, 0, 10, true )]
        [TestCase( -5, 5, 0, 10, true )]
        public void IsOverlappingTest( int min1, int max1, int min2, int max2, bool expectedResult )
        {
            IntRange range1 = new IntRange( min1, max1 );
            IntRange range2 = new IntRange( min2, max2 );

            Assert.AreEqual( expectedResult, range1.IsOverlapping( range2 ) );
        }

        [TestCase( 0, 1, 0, 1 )]
        [TestCase( -1, 0, -1, 0 )]
        public void ToRangeTest( int iMin, int iMax, float fMin, float fMax )
        {
            IntRange iRange = new IntRange( iMin, iMax );
            Range range = iRange;

            Assert.AreEqual( fMin, range.Min );
            Assert.AreEqual( fMax, range.Max );
        }

        [TestCase( 1, 2, 1, 2, true )]
        [TestCase( -2, -1, -2, -1, true )]
        [TestCase( 1, 2, 2, 3, false )]
        [TestCase( 1, 2, 1, 4, false )]
        [TestCase( 1, 2, 3, 4, false )]
        public void EqualityOperatorTest( int min1, int max1, int min2, int max2, bool areEqual )
        {
            IntRange range1 = new IntRange( min1, max1 );
            IntRange range2 = new IntRange( min2, max2 );

            Assert.AreEqual( range1.Equals( range2 ), areEqual );
            Assert.AreEqual( range1 == range2, areEqual );
            Assert.AreEqual( range1 != range2, !areEqual );
        }
    }
}
