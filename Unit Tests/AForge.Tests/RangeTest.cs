using System;
using AForge;
using MbUnit.Framework;

namespace AForge.Tests
{
    [TestFixture]
    public class RangeTest
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
        public void IsOverlappingTest( float min1, float max1, float min2, float max2, bool expectedResult )
        {
            Range range1 = new Range( min1, max1 );
            Range range2 = new Range( min2, max2 );

            Assert.AreEqual<bool>( expectedResult, range1.IsOverlapping( range2 ) );
        }

        [Test]
        [Row( 0.4, 7.3, 1, 7, true )]
        [Row( -6.6, -0.1, -6, -1, true )]
        [Row( 0.4, 7.3, 0, 8, false )]
        [Row( -6.6, -0.1, -7, 0, false )]
        public void ToRangeTest( float fMin, float fMax, int iMin, int iMax, bool innerRange )
        {
            Range range = new Range( fMin, fMax );
            IntRange iRange = range.ToIntRange( innerRange );

            Assert.AreEqual<int>( iMin, iRange.Min );
            Assert.AreEqual<int>( iMax, iRange.Max );
        }

        [Test]
        [Row( 1.1, 2.2, 1.1, 2.2, true )]
        [Row( -2.2, -1.1, -2.2, -1.1, true )]
        [Row( 1.1, 2.2, 2.2, 3.3, false )]
        [Row( 1.1, 2.2, 1.1, 4.4, false )]
        [Row( 1.1, 2.2, 3.3, 4.4, false )]
        public void EqualityOperatorTest( float min1, float max1, float min2, float max2, bool areEqual )
        {
            Range range1 = new Range( min1, max1 );
            Range range2 = new Range( min2, max2 );

            Assert.AreEqual( range1.Equals( range2 ), areEqual );
            Assert.AreEqual( range1 == range2, areEqual );
            Assert.AreEqual( range1 != range2, !areEqual );
        }
    }
}
