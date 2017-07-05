﻿using System;
using NUnit.Framework;
using Accord.Math.Geometry;

namespace Accord.Tests.Math
{
    [TestFixture]
    public class LineSegmentTest
    {

        [TestCase(0, 0, 10, 0, 10)]
        [TestCase(0, 0, 0, 10, 10)]
        [TestCase(0, 0, 3, 4, 5)]
        [TestCase(0, 0, -3, 4, 5)]
        [TestCase(0, 0, -3, -4, 5)]
        public void LengthTest(float sx, float sy, float ex, float ey, float expectedResult)
        {
            LineSegment segment = new LineSegment(new Point(sx, sy), new Point(ex, ey));

            Assert.AreEqual(expectedResult, segment.Length);
        }


        [TestCase(0, 0, 5, 0, 8, 0, 5)]
        [TestCase(6f, 2.5f, 5f, 0f, 8f, 0f, 2.5f)]
        [TestCase(2.5f, 6f, 0f, 5f, 0f, 8f, 2.5f)]
        [TestCase(9, 0, 5, 0, 8, 0, 1)]
        [TestCase(3, 4, 0, 0, -10, 0, 5)]
        public void DistanceToPointTest(float x, float y, float x1, float y1, float x2, float y2, float expectedDistance)
        {
            Point pt = new Point(x, y);
            Point pt1 = new Point(x1, y1);
            Point pt2 = new Point(x2, y2);
            LineSegment segment = new LineSegment(pt1, pt2);

            Assert.AreEqual(expectedDistance, segment.DistanceToPoint(pt));
        }

        // Denotes which versions of the test are supposed to return non-null values:
        // SegmentA means that the segment A1-A2 intersects with the line B1-B2, but not
        // with the segment B1-B2.
        public enum IntersectionType { None, LinesOnly, SegmentA, SegmentB, AllFour };


        [TestCase(0, 0, 4, 4, 0, 4, 4, 0, 2, 2, IntersectionType.AllFour)]
        [TestCase(0, 0, 4, 0, 0, 0, 0, 4, 0, 0, IntersectionType.AllFour)]
        [TestCase(0, 0, 4, 4, 4, 8, 8, 4, 6, 6, IntersectionType.SegmentB)]
        [TestCase(-4, -4, 0, 0, 4, 0, 8, -4, 2, 2, IntersectionType.LinesOnly)]
        [TestCase(0, 0, 6, 0, 5, 1, 5, 5, 5, 0, IntersectionType.SegmentA)]
        [TestCase(0, 0, 0, 5, 1, 0, 1, 5, 0, 0, IntersectionType.None)]
        public void IntersectionPointTest(float ax1, float ay1, float ax2, float ay2, float bx1, float by1, float bx2, float by2, float ix, float iy, IntersectionType type)
        {
            LineSegment segA = new LineSegment(new Point(ax1, ay1), new Point(ax2, ay2));
            LineSegment segB = new LineSegment(new Point(bx1, by1), new Point(bx2, by2));
            Point expectedIntersection = new Point(ix, iy);

            Assert.DoesNotThrow(() =>
           {
               Point? segSeg = segA.GetIntersectionWith(segB);
               Point? segLine = segA.GetIntersectionWith((Line)segB);
               Point? lineSeg = ((Line)segA).GetIntersectionWith(segB);

               if (type == IntersectionType.AllFour)
               {
                   Assert.AreEqual(expectedIntersection, segSeg);
               }
               else
               {
                   Assert.AreEqual(null, segSeg);
               }

               if ((type == IntersectionType.AllFour) || (type == IntersectionType.SegmentA))
               {
                   Assert.AreEqual(expectedIntersection, segLine);
               }
               else
               {
                   Assert.AreEqual(null, segLine);
               }

               if ((type == IntersectionType.AllFour) || (type == IntersectionType.SegmentB))
               {
                   Assert.AreEqual(expectedIntersection, lineSeg);
               }
               else
               {
                   Assert.AreEqual(null, lineSeg);
               }
           });

            Point? lineLine = ((Line)segA).GetIntersectionWith((Line)segB);

            if (type != IntersectionType.None)
            {
                Assert.AreEqual(expectedIntersection, lineLine);
            }
            else
            {
                Assert.AreEqual(null, lineLine);
            }
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, IntersectionType.LinesOnly)]
        public void IntersectionPointTestException(float ax1, float ay1, float ax2, float ay2, float bx1, float by1, float bx2, float by2, float ix, float iy, IntersectionType type)
        {
            Assert.Throws<ArgumentException>(() => new LineSegment(new Point(ax1, ay1), new Point(ax2, ay2)),
                "Start point of the line cannot be the same as its end point.");
        }


        [TestCase(0, 0, 0, 1, 1, 1, 1, 2)]
        [TestCase(0, 0, 4, 4, 3, -1, 7, 3)]
        [TestCase(0, 0, 1, 0, 1, 1, 2, 1)]
        public void ParallelIntersectionPointTest(float ax1, float ay1, float ax2, float ay2, float bx1, float by1, float bx2, float by2)
        {
            LineSegment segA = new LineSegment(new Point(ax1, ay1), new Point(ax2, ay2));
            LineSegment segB = new LineSegment(new Point(bx1, by1), new Point(bx2, by2));

            // are we really parallel?
            Assert.AreEqual(null, ((Line)segA).GetIntersectionWith((Line)segB));

            Assert.AreEqual(null, segA.GetIntersectionWith((Line)segB));
            Assert.AreEqual(null, ((Line)segA).GetIntersectionWith(segB));
            Assert.AreEqual(null, segB.GetIntersectionWith((Line)segA));
            Assert.AreEqual(null, ((Line)segB).GetIntersectionWith(segA));
            Assert.AreEqual(null, segB.GetIntersectionWith(segA));
            Assert.AreEqual(null, segA.GetIntersectionWith(segB));
        }


        [TestCase(0, 0, 1, 1, 2, 2, 3, 3)]
        [TestCase(0, 1, 0, 2, 0, 3, 0, 4)]
        [TestCase(0, 0, -1, 1, -2, 2, -3, 3)]
        [TestCase(1, 0, 2, 0, 3, 0, 4, 0)]
        [TestCase(0, 0, 0, 1, 0, 2, 0, 3)]
        public void CollinearIntersectionPointTest(float ax1, float ay1, float ax2, float ay2, float bx1, float by1, float bx2, float by2)
        {
            LineSegment segA = new LineSegment(new Point(ax1, ay1), new Point(ax2, ay2));
            LineSegment segB = new LineSegment(new Point(bx1, by1), new Point(bx2, by2));

            // are we really collinear?
            Assert.Throws<InvalidOperationException>(() => ((Line)segA).GetIntersectionWith((Line)segB));

            Assert.Throws<InvalidOperationException>(() => segA.GetIntersectionWith((Line)segB));
            Assert.Throws<InvalidOperationException>(() => ((Line)segA).GetIntersectionWith(segB));
            Assert.Throws<InvalidOperationException>(() => segB.GetIntersectionWith((Line)segA));
            Assert.Throws<InvalidOperationException>(() => ((Line)segB).GetIntersectionWith(segA));
            Assert.AreEqual(null, segB.GetIntersectionWith(segA));
            Assert.AreEqual(null, segA.GetIntersectionWith(segB));
        }


        [TestCase(0, 0, 1, 1, 1, 1, 3, 3, 1, 1)]
        [TestCase(0, 0, 1, 1, 3, 3, 1, 1, 1, 1)]
        [TestCase(0, 0, 1, 1, 0, 0, -3, -3, 0, 0)]
        [TestCase(0, 0, 1, 1, -1, -1, 0, 0, 0, 0)]
        [TestCase(0, 1, 0, 2, 0, 1, 0, 0, 0, 1)]
        [TestCase(0, 1, 0, 2, 0, 2, 0, 4, 0, 2)]
        [TestCase(0, 1, 0, 2, 0, 0, 0, 1, 0, 1)]
        [TestCase(0, 1, 0, 2, 0, 3, 0, 2, 0, 2)]
        public void CommonIntersectionPointTest(float ax1, float ay1, float ax2, float ay2, float bx1, float by1, float bx2, float by2, float ix, float iy)
        {
            LineSegment segA = new LineSegment(new Point(ax1, ay1), new Point(ax2, ay2));
            LineSegment segB = new LineSegment(new Point(bx1, by1), new Point(bx2, by2));
            Point expectedIntersection = new Point(ix, iy);

            // are we really collinear?
            Assert.Throws<InvalidOperationException>(() => ((Line)segA).GetIntersectionWith((Line)segB));

            Assert.Throws<InvalidOperationException>(() => segA.GetIntersectionWith((Line)segB));
            Assert.Throws<InvalidOperationException>(() => ((Line)segA).GetIntersectionWith(segB));
            Assert.Throws<InvalidOperationException>(() => segB.GetIntersectionWith((Line)segA));
            Assert.Throws<InvalidOperationException>(() => ((Line)segB).GetIntersectionWith(segA));
            Assert.AreEqual(expectedIntersection, segB.GetIntersectionWith(segA));
            Assert.AreEqual(expectedIntersection, segA.GetIntersectionWith(segB));
        }


        [TestCase(0, 0, 0, 2, 0, 1, 0, 3)]
        [TestCase(1, 2, 3, 4, 2, 3, 4, 5)]
        [TestCase(0, 0, 2, 0, 3, 0, 1, 0)]
        public void OverlappingSegmentIntersectionPointTest(float ax1, float ay1, float ax2, float ay2, float bx1, float by1, float bx2, float by2)
        {
            LineSegment segA = new LineSegment(new Point(ax1, ay1), new Point(ax2, ay2));
            LineSegment segB = new LineSegment(new Point(bx1, by1), new Point(bx2, by2));

            // are we really collinear?
            Assert.Throws<InvalidOperationException>(() => ((Line)segA).GetIntersectionWith((Line)segB));

            Assert.Throws<InvalidOperationException>(() => segA.GetIntersectionWith((Line)segB));
            Assert.Throws<InvalidOperationException>(() => ((Line)segA).GetIntersectionWith(segB));
            Assert.Throws<InvalidOperationException>(() => segB.GetIntersectionWith((Line)segA));
            Assert.Throws<InvalidOperationException>(() => ((Line)segB).GetIntersectionWith(segA));
            Assert.Throws<InvalidOperationException>(() => segB.GetIntersectionWith(segA));
            Assert.Throws<InvalidOperationException>(() => segA.GetIntersectionWith(segB));
        }

        [Test]
        public void issue_359()
        {
            LineSegment segA = new LineSegment(new Point(32.0951f, 36.0828f), new Point(32.0863f, 36.1038f));
            LineSegment segB = new LineSegment(new Point(32.0861f, 36.2358f), new Point(32.0859f, 36.0889f));

            Point? result = segA.GetIntersectionWith(segB);
            Assert.IsNull(result);
        }
    }
}
