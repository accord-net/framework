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
    public class IntPointTest
    {
        [TestCase( 0, 0, 0 )]
        [TestCase( 0, 1, 1 )]
        [TestCase( 0, 10, 10 )]
        [TestCase( 10, 0, 10 )]
        [TestCase( 3, 4, 5 )]
        [TestCase( -3, 4, 5 )]
        [TestCase( 3, -4, 5 )]
        [TestCase( -3, -4, 5 )]
        public void EuclideanNormTest( int x, int y, double expectedNorm )
        {
            IntPoint point = new IntPoint( x, y );

            Assert.AreEqual( point.EuclideanNorm( ), expectedNorm );
        }

        [TestCase( 1, 2, 1, 2, true )]
        [TestCase( 1, 2, 3, 2, false )]
        [TestCase( 1, 2, 1, 4, false )]
        [TestCase( 1, 2, 3, 4, false )]
        public void EqualityOperatorTest( int x1, int y1, int x2, int y2, bool areEqual )
        {
            IntPoint point1 = new IntPoint( x1, y1 );
            IntPoint point2 = new IntPoint( x2, y2 );

            Assert.AreEqual( point1 == point2, areEqual );
        }

        [TestCase( 1, 2, 1, 2, false )]
        [TestCase( 1, 2, 3, 2, true )]
        [TestCase( 1, 2, 1, 4, true )]
        [TestCase( 1, 2, 3, 4, true )]
        public void InequalityOperatorTest( int x1, int y1, int x2, int y2, bool areNotEqual )
        {
            IntPoint point1 = new IntPoint( x1, y1 );
            IntPoint point2 = new IntPoint( x2, y2 );

            Assert.AreEqual( point1 != point2, areNotEqual );
        }
    }
}
