using System;
using NUnit.Framework;
using Accord.Math.Geometry;

namespace Accord.Tests.Math
{
    [TestFixture]
    public class LineTest
    {
        private const float Error = 0.00001f;


        [TestCase(1, 1, 45, 1.41421356f, -1, 2)]
        [TestCase(-2, 2, 135, 2 * 1.41421356f, 1, 4)]
        [TestCase(-0.5f, -1.73205081f / 2, 240, 1, -1 / 1.73205081f, -2 / 1.73205081f)]
        [TestCase(1, 0, 0, 1, float.NegativeInfinity, 1)]
        [TestCase(0, -1, 270, 1, 0, -1)]
        public void RThetaTest(float x, float y, float theta, float expectedRadius, float expectedSlope, float expectedIntercept)
        {
            Point pt = new Point(x, y);

            // test Point-Theta factory
            Line line = Line.FromPointTheta(pt, theta);
            Assert.AreEqual(expectedSlope, line.Slope, Error);
            Assert.AreEqual(expectedIntercept, line.Intercept, Error);

            // calculate radius
            float radius = pt.EuclideanNorm();
            Assert.AreEqual(expectedRadius, radius, Error);

            // test R-Theta factory
            line = Line.FromRTheta(radius, theta);
            Assert.AreEqual(expectedSlope, line.Slope, Error);
            Assert.AreEqual(expectedIntercept, line.Intercept, Error);
        }


        [TestCase(0, 0, 0, 10, true, float.PositiveInfinity, 0)]
        [TestCase(0, 0, 0, -10, true, float.NegativeInfinity, 0)]
        [TestCase(0, 0, 10, 10, false, 1, 0)]
        [TestCase(0, 0, 10, 0, false, 0, 0)]
        public void IsVerticalTest(float sx, float sy, float ex, float ey, bool expectedResult, float expectedSlope, float expectedIntercept)
        {
            Line line = Line.FromPoints(new Point(sx, sy), new Point(ex, ey));

            Assert.AreEqual(expectedResult, line.IsVertical);
            Assert.AreEqual(expectedSlope, line.Slope);
            Assert.AreEqual(expectedIntercept, line.Intercept);
        }


        [TestCase(0, 0, 10, 0, true, 0, 0)]
        [TestCase(0, 0, -10, 0, true, 0, 0)]
        [TestCase(0, 0, 10, 10, false, 1, 0)]
        [TestCase(0, 0, 0, 10, false, float.PositiveInfinity, 0)]
        public void IsHorizontalTest(float sx, float sy, float ex, float ey, bool expectedResult, float expectedSlope, float expectedIntercept)
        {
            Line line = Line.FromPoints(new Point(sx, sy), new Point(ex, ey));

            Assert.AreEqual(expectedResult, line.IsHorizontal);
            Assert.AreEqual(expectedSlope, line.Slope);
            Assert.AreEqual(expectedIntercept, line.Intercept);
        }


        [TestCase(0, 0, 10, 0, 0, 10, 10, 10, 0)]
        [TestCase(0, 0, 10, 0, 0, 10, 0, 20, 90)]
        [TestCase(0, 0, 10, 0, 1, 1, 10, 10, 45)]
        [TestCase(0, 0, 10, 0, 1, 1, -8, 10, 45)]
        [TestCase(0, 0, 10, 10, 0, 0, -100, 100, 90)]
        public void GetAngleBetweenLinesTest(float sx1, float sy1, float ex1, float ey1, float sx2, float sy2, float ex2, float ey2, float expectedAngle)
        {
            Line line1 = Line.FromPoints(new Point(sx1, sy1), new Point(ex1, ey1));
            Line line2 = Line.FromPoints(new Point(sx2, sy2), new Point(ex2, ey2));

            float angle = line1.GetAngleBetweenLines(line2);

            Assert.AreEqual(expectedAngle, angle, Error);
        }


        [TestCase(0, 0, 1, 0, 0, 1, 1, 1, 0, 0, false)]
        [TestCase(0, 0, 0, 1, 1, 0, 1, 1, 0, 0, false)]
        [TestCase(0, 0, 1, 1, 0, 1, 1, 2, 0, 0, false)]
        [TestCase(0, 0, 1, 0, 0, 0, 1, 1, 0, 0, true)]
        [TestCase(0, 0, 1, 0, 0, 1, 1, 2, -1, 0, true)]
        [TestCase(0, 0, 1, 0, 1, 1, 1, 2, 1, 0, true)]
        [TestCase(0, 0, 0, 1, 0, 1, -1, 1, 0, 1, true)]
        [TestCase(-1, -1, 1, 1, 1, -1, -1, 1, 0, 0, true)]
        public void GetIntersectionPointTest(float sx1, float sy1, float ex1, float ey1,
            float sx2, float sy2, float ex2, float ey2, float xRet, float yRet, bool hasResult)
        {
            Line line1 = Line.FromPoints(new Point(sx1, sy1), new Point(ex1, ey1));
            Line line2 = Line.FromPoints(new Point(sx2, sy2), new Point(ex2, ey2));

            Point? result = line1.GetIntersectionWith(line2);

            if (hasResult)
            {
                Assert.IsTrue(result == new Point(xRet, yRet));
            }
            else
            {
                Assert.AreEqual(null, result);
            }
        }

        [TestCase(0, 0, 1, 1, 0, 0, 1, 1, 0, 0, false)]
        public void GetIntersectionPointTestException(float sx1, float sy1, float ex1, float ey1,
           float sx2, float sy2, float ex2, float ey2, float xRet, float yRet, bool hasResult)
        {
            Line line1 = Line.FromPoints(new Point(sx1, sy1), new Point(ex1, ey1));
            Line line2 = Line.FromPoints(new Point(sx2, sy2), new Point(ex2, ey2));

            Assert.Throws<InvalidOperationException>(() => line1.GetIntersectionWith(line2), "");
        }


        [TestCase(0, 0, 5, 0, 8, 0, 0)]
        [TestCase(6, 2, 5, 0, 8, 0, 2)]
        [TestCase(2, 6, 0, 5, 0, 8, 2)]
        [TestCase(9, 0, 5, 0, 8, 0, 0)]
        [TestCase(3, 0, 0, 0, 3, 4, 2.4f)]
        public void DistanceToPointTest(float x, float y, float x1, float y1, float x2, float y2, float expectedDistance)
        {
            Point pt = new Point(x, y);
            Point pt1 = new Point(x1, y1);
            Point pt2 = new Point(x2, y2);
            Line line = Line.FromPoints(pt1, pt2);

            Assert.AreEqual(expectedDistance, line.DistanceToPoint(pt), Error);
        }
    }
}
