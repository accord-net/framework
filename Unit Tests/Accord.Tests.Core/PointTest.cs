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

using NUnit.Framework;

namespace Accord.Tests
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
