using System;
using AForge;
using AForge.Math.Geometry;
using MbUnit.Framework;

namespace AForge.Math.Geometry.Tests
{
    [TestFixture]
    public class GeometryToolsTest
    {
        [Test]
        [Row( 0, 0, 10, 0, 100, 0, 0 )]
        [Row( 0, 0, 10, 10, 100, 100, 0 )]
        [Row( 0, 0, 10, 0, 0, 100, 90 )]
        [Row( 0, 0, 10, 0, 100, 100, 45 )]
        [Row( 0, 0, 10, 10, -100, 100, 90 )]
        [Row( 0, 0, 10, 0, -100, 100, 135 )]
        [Row( 0, 0, 10, 0, -100, 0, 180 )]
        [Row( 0, 0, 10, 0, -100, -100, 135 )]
        public void GetAngleBetweenVectorsTest( int sx, int sy, int ex1, int ey1, int ex2, int ey2, float expectedAngle )
        {
            IntPoint startPoint = new IntPoint( sx, sy );
            IntPoint vector1end = new IntPoint( ex1, ey1 );
            IntPoint vector2end = new IntPoint( ex2, ey2 );

            float angle = GeometryTools.GetAngleBetweenVectors( startPoint, vector1end, vector2end );

            Assert.AreApproximatelyEqual<float, float>( expectedAngle,  angle, 0.00001f );
        }

        [Test]
        [Row( 0, 0, 10, 0, 0, 10, 10, 10, 0 )]
        [Row( 0, 0, 10, 0, 0, 10, 0, 20, 90 )]
        [Row( 0, 0, 10, 0, 1, 1, 10, 10, 45 )]
        [Row( 0, 0, 10, 0, 1, 1, -8, 10, 45 )]
        [Row( 0, 0, 10, 10, 0, 0, -100, 100, 90 )]
        public void GetAngleBetweenLinesTest( int sx1, int sy1, int ex1, int ey1, int sx2, int sy2, int ex2, int ey2, float expectedAngle )
        {
            IntPoint line1start = new IntPoint( sx1, sy1 );
            IntPoint line1end   = new IntPoint( ex1, ey1 );
            IntPoint line2start = new IntPoint( sx2, sy2 );
            IntPoint line2end   = new IntPoint( ex2, ey2 );

            float angle = GeometryTools.GetAngleBetweenLines( line1start, line1end, line2start, line2end );

            Assert.AreApproximatelyEqual<float, float>( expectedAngle, angle, 0.00001f );
        }
    }
}
