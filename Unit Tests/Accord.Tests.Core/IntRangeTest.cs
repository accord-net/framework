// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

using Accord;
using NUnit.Framework;

namespace Accord.Tests
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
