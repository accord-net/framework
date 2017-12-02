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

namespace Accord.Tests.MachineLearning
{
    using System.Collections.Generic;
    using System.Drawing;
    using Accord.MachineLearning.Geometry;
    using AForge;
    using Accord.Tests.Imaging.Properties;
    using NUnit.Framework;
    using Point = Accord.Point;
    using Accord.Imaging;
    using Accord.Imaging.Filters;
    using Accord.Math.Geometry;
    using System.Diagnostics;
    using System.Drawing.Imaging;
    using Accord.Collections;
    using Accord.Math;
#if NO_BITMAP
    using Resources = Accord.Tests.Imaging.Properties.Resources_Standard;
#endif

    [TestFixture]
    public class KDTree2DTest
    {

        [Test]
        public void gh_784()
        {
            Accord.Math.Random.Generator.Seed = 0;
            var rnd = Accord.Math.Random.Generator.Random;

            // This is the same example found in Wikipedia page on
            // k-d trees: http://en.wikipedia.org/wiki/K-d_tree

            var image = UnmanagedImage.Create(800, 600, PixelFormat.Format24bppRgb);

            // Suppose we have the following set of points:
            var points = new double[300000][];
            var listPoints = new List<IntPoint>(points.Length);
            for (int i = 0; i < points.Length; i++)
            {
                var point = new IntPoint(rnd.Next(0, image.Width), rnd.Next(0, image.Height));

                points[i] = new double[] { point.X, point.Y };

                listPoints.Add(point);
            }

            var region = new Rectangle(676, 441, 70, 55);

            var sw1 = new Stopwatch();
            sw1.Restart();
            var query1 = listPoints.FindAll((obj) =>
            {
                return obj.X > region.Left && obj.X < region.Right && obj.Y > region.Top && obj.Y < region.Bottom;
            });

            sw1.Stop();

            listPoints.Clear();

            var sw2 = new Stopwatch();
            sw2.Restart();
            // To create a tree from a set of points, we can use
            var tree = KDTree.FromData<int>(points, new Accord.Math.Distances.Manhattan(), inPlace: true);
            sw2.Stop();

            var sw3 = new Stopwatch();
            sw3.Restart();
            var actual = tree.GetNodesInsideRegion(region.ToHyperrectangle()).Apply(x => new IntPoint((int)x.Position[0], (int)x.Position[1]));
            sw3.Stop();


            var sw4 = new Stopwatch();
            sw4.Restart();
            var expected = QueryKDTree2D(tree.Root, region);
            sw4.Stop();

            Assert.AreEqual(actual, expected);
        }


        public static IList<IntPoint> QueryKDTree2D(KDTreeNode<int> node, Rectangle region)
        {
            return QueryKDTree2D(node, region, region);
        }

        private static IList<IntPoint> QueryKDTree2D(KDTreeNode<int> node, Rectangle region, Rectangle subRegion)
        {
            var result = new List<IntPoint>();

            if (node != null && region.IntersectsWith(subRegion))
            {
                //if (node.Position.Length != 2)
                //    new DimensionMismatchException("node", "The node should have a 2D vector.");

                var point = new IntPoint((int)node.Position[0], (int)node.Position[1]);

                if (region.Contains(point.X, point.Y))
                    result.Add(point);

                result.AddRange(QueryKDTree2D(node.Left, region, LeftRect(subRegion, node)));
                result.AddRange(QueryKDTree2D(node.Right, region, RightRect(subRegion, node)));
            }

            return result;
        }

        // helper: get the left rectangle of node inside parent's rect
        private static Rectangle LeftRect(Rectangle rect, KDTreeNode<int> node)
        {
            return node.Axis != 0 ?
                Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, (int)node.Position[1]) :
                Rectangle.FromLTRB(rect.Left, rect.Top, (int)node.Position[0], rect.Bottom);
        }
        // helper: get the right rectangle of node inside parent's rect
        private static Rectangle RightRect(Rectangle rect, KDTreeNode<int> node)
        {
            return node.Axis != 0 ?
                Rectangle.FromLTRB(rect.Left, (int)node.Position[1], rect.Right, rect.Bottom) :
                Rectangle.FromLTRB((int)node.Position[0], rect.Top, rect.Right, rect.Bottom);
        }
    }
}
