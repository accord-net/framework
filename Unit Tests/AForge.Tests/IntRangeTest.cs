using System;
using AForge;
using MbUnit.Framework;

namespace AForge.Tests
{
    [TestFixture]
    public class IntRangeTest
    {
        [Test]
        [Row( 0, 1, 1, 2, true )]
        [Row( 0, 1, 2, 3, false )]
        [Row( 0, 10, 2, 4, true )]
        [Row( 0, 10, 5, 15, true )]
        [Row( 0, 10, -5, 5, true )]
        [Row( 2, 4, 0, 10, true )]
        [Row( 5, 15, 0, 10, true )]
        [Row( -5, 5, 0, 10, true )]
        public void IsOverlappingTest( int min1, int max1, int min2, int max2, bool expectedResult )
        {
            IntRange range1 = new IntRange( min1, max1 );
            IntRange range2 = new IntRange( min2, max2 );

            Assert.AreEqual<bool>( expectedResult, range1.IsOverlapping( range2 ) );
        }

        [Test]
        [Row( 0, 1, 0, 1 )]
        [Row( -1, 0, -1, 0 )]
        public void ToRangeTest( int iMin, int iMax, float fMin, float fMax )
        {
            IntRange iRange = new IntRange( iMin, iMax );
            Range range = iRange;

            Assert.AreEqual<float>( fMin, range.Min );
            Assert.AreEqual<float>( fMax, range.Max );
        }

        [Test]
        [Row( 1, 2, 1, 2, true )]
        [Row( -2, -1, -2, -1, true )]
        [Row( 1, 2, 2, 3, false )]
        [Row( 1, 2, 1, 4, false )]
        [Row( 1, 2, 3, 4, false )]
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
