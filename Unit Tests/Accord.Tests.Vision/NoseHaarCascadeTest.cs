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

namespace Accord.Tests.Vision
{
    using Accord.Vision.Detection.Cascades;
    using NUnit.Framework;
    using Accord.Vision.Detection;
    using System.IO;
#if NETSTANDARD2_0
    using Resources = Accord.Tests.Vision.Properties.Resources_Standard;
#endif

    [TestFixture]
    public class NoseHaarCascadeTest
    {

        [Test]
        public void NoseHaarCascadeConstructorTest()
        {
            NoseHaarCascade actual = new NoseHaarCascade();

            string fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "haarcascade_mcs_nose.xml");
            HaarCascade expected = HaarCascade.FromXml(new StreamReader(fileName));

            Assert.AreNotEqual(expected, actual);
            Assert.AreEqual(expected.HasTiltedFeatures, actual.HasTiltedFeatures);
            Assert.AreEqual(expected.Width, actual.Width);
            Assert.AreEqual(expected.Height, actual.Height);
            Assert.AreEqual(expected.Stages.Length, actual.Stages.Length);

            for (int i = 0; i < actual.Stages.Length; i++)
            {
                var aStage = actual.Stages[i];
                var eStage = expected.Stages[i];

                Assert.AreNotEqual(eStage, aStage);

                Assert.AreEqual(aStage.NextIndex, eStage.NextIndex);
                Assert.AreEqual(aStage.ParentIndex, eStage.ParentIndex);
                Assert.AreEqual(aStage.Threshold, eStage.Threshold);

                Assert.AreEqual(aStage.Trees.Length, eStage.Trees.Length);

                for (int j = 0; j < aStage.Trees.Length; j++)
                {
                    var aTree = aStage.Trees[j];
                    var eTree = eStage.Trees[j];

                    Assert.AreNotEqual(eTree, aTree);
                    Assert.AreEqual(eTree.Length, aTree.Length);

                    for (int k = 0; k < aTree.Length; k++)
                    {
                        var aNode = aTree[k];
                        var eNode = eTree[k];

                        Assert.AreNotEqual(eNode, aNode);
                        Assert.AreEqual(eNode.LeftNodeIndex, aNode.LeftNodeIndex);
                        Assert.AreEqual(eNode.LeftValue, aNode.LeftValue, 1e-10);
                        Assert.AreEqual(eNode.RightNodeIndex, aNode.RightNodeIndex, 1e-16);
                        Assert.AreEqual(eNode.RightValue, aNode.RightValue, 1e-16);
                        Assert.AreEqual(eNode.Threshold, aNode.Threshold, 1e-16);
                        Assert.IsFalse(double.IsNaN(aNode.Threshold));

                        Assert.AreEqual(eNode.Feature.Tilted, aNode.Feature.Tilted);
                        Assert.AreEqual(eNode.Feature.Rectangles.Length, aNode.Feature.Rectangles.Length);

                        for (int l = 0; l < eNode.Feature.Rectangles.Length; l++)
                        {
                            var aRect = aNode.Feature.Rectangles[l];
                            var eRect = eNode.Feature.Rectangles[l];

                            Assert.AreNotEqual(eRect, aRect);
                            Assert.AreEqual(eRect.Area, aRect.Area, 1e-16);
                            Assert.AreEqual(eRect.Height, aRect.Height, 1e-16);
                            Assert.AreEqual(eRect.Weight, aRect.Weight, 1e-16);
                            Assert.AreEqual(eRect.Width, aRect.Width, 1e-16);
                            Assert.AreEqual(eRect.X, aRect.X);
                            Assert.AreEqual(eRect.Y, aRect.Y);

                            Assert.IsFalse(double.IsNaN(aRect.Height));
                            Assert.IsFalse(double.IsNaN(aRect.Width));
                            Assert.IsFalse(double.IsNaN(aRect.Area));
                        }
                    }
                }
            }
        }
    }
}
