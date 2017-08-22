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

namespace Accord.Tests.Imaging
{
    using System.Collections.Generic;
    using Accord.Math;
    using AForge;
    using System.Linq;
    using NUnit.Framework;
    using Accord.Math.Geometry;
    using Accord.Imaging.Filters;

    [TestFixture]
    public class ConvexHullDefectsTest
    {


        [Test]
        public void FindTest()
        {
            List<IntPoint> contour = new List<IntPoint>();

            int max = 100;

            for (int i = 0; i < max; i++)
                add(contour, i, max);

            for (int i = 0; i < max; i++)
                add(contour, max, i);

            for (int i = 0; i < max; i++)
                add(contour, 0, i);

            for (int i = 0; i < max / 2; i++)
                add(contour, i, i);

            for (int i = 0; i < max / 2; i++)
                add(contour, i + max / 2, max / 2 - i);

            PointsMarker marker = new PointsMarker(contour);
            var bitmap = Accord.Imaging.Image.CreateGrayscaleImage(max + 1, max + 1);
            bitmap = marker.Apply(bitmap);
            // Accord.Controls.ImageBox.Show(bitmap);

            GrahamConvexHull graham = new GrahamConvexHull();
            List<IntPoint> hull = graham.FindHull(contour);
            ConvexHullDefects hullDefects = new ConvexHullDefects(10);
            List<ConvexityDefect> defects = hullDefects.FindDefects(contour, hull);

            Assert.AreEqual(1, defects.Count);
            Assert.AreEqual(99, defects[0].Depth);
        }

        private static void add(List<IntPoint> contour, int x, int y)
        {
            var p = new IntPoint(x, y);
            if (!contour.Contains(p))
                contour.Add(p);
        }
    }
}
