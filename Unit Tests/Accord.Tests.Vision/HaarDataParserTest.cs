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
    using System.IO;
    using Accord.Vision.Detection;
    using Accord.Vision.Detection.Cascades;
    using NUnit.Framework;
#if NETSTANDARD2_0
    using Resources = Accord.Tests.Vision.Properties.Resources_Standard;
#endif

    [TestFixture]
    public class HaarDataParserTest
    {

        [Test]
        public void ParseTest()
        {
            string fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "haarcascade_frontalface_alt.xml");
            HaarCascade cascade1 = HaarCascade.FromXml(new StreamReader(fileName));

            Assert.AreEqual(22, cascade1.Stages.Length);
            Assert.AreEqual(3, cascade1.Stages[0].Trees.Length);
            Assert.AreEqual(1, cascade1.Stages[0].Trees[0].Length);

            Assert.AreEqual(false, cascade1.HasTiltedFeatures);

            // Load the hard coded version of the classifier
            HaarCascade cascade2 = new FaceHaarCascade();

            Assert.AreEqual(cascade1.Stages.Length, cascade2.Stages.Length);
            Assert.AreEqual(cascade1.Height, cascade2.Height);
            Assert.AreEqual(cascade1.Width, cascade2.Width);


            for (int i = 0; i < 3; i++)
            {
                HaarCascadeStage stage1 = cascade1.Stages[i];
                HaarCascadeStage stage2 = cascade2.Stages[i];

                //Assert.AreEqual(stage1.NextIndex, stage2.NextIndex);
                //Assert.AreEqual(stage1.ParentIndex, stage2.ParentIndex);

                Assert.AreEqual(stage1.Threshold, stage2.Threshold);

                Assert.AreEqual(stage1.Trees.Length, stage2.Trees.Length);

                for (int j = 0; j < stage1.Trees.Length && j < stage2.Trees.Length; j++)
                {
                    HaarFeatureNode[] tree1 = stage1.Trees[j];
                    HaarFeatureNode[] tree2 = stage2.Trees[j];

                    Assert.AreEqual(tree1.Length, tree2.Length);

                    for (int k = 0; k < tree1.Length; k++)
                    {
                        HaarFeatureNode node1 = tree1[k];
                        HaarFeatureNode node2 = tree2[k];

                        Assert.AreEqual(node1.LeftNodeIndex, node2.LeftNodeIndex);
                        Assert.AreEqual(node1.RightNodeIndex, node2.RightNodeIndex);

                        Assert.AreEqual(node1.LeftValue, node2.LeftValue);
                        Assert.AreEqual(node1.RightValue, node2.RightValue);

                        Assert.AreEqual(node1.Feature.Tilted, node2.Feature.Tilted);

                        Assert.AreEqual(node1.Threshold, node2.Threshold, 0.000000001);
                    }
                }
            }

        }

        [Test]
        public void ParseTest2()
        {
            string fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "haarcascade_frontalface_alt2.xml");
            StreamReader stringReader = new StreamReader(fileName);
            HaarCascade cascade = HaarCascade.FromXml(stringReader);

            Assert.AreEqual(20, cascade.Stages.Length);
            Assert.AreEqual(3, cascade.Stages[0].Trees.Length);
            Assert.AreEqual(2, cascade.Stages[0].Trees[0].Length);

            Assert.AreEqual(false, cascade.HasTiltedFeatures);
        }

        [Test]
        public void ParseTest3()
        {
            HaarCascade cascade = new FaceHaarCascade();

            Assert.AreEqual(22, cascade.Stages.Length);
            Assert.AreEqual(3, cascade.Stages[0].Trees.Length);
            Assert.AreEqual(1, cascade.Stages[0].Trees[0].Length);
        }

        [Test]
        public void ParseTest4()
        {
            string fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "haarcascade_eye_tree_eyeglasses.xml");
            StreamReader stringReader = new StreamReader(fileName);
            HaarCascade cascade = HaarCascade.FromXml(stringReader);

            Assert.AreEqual(30, cascade.Stages.Length);
            Assert.AreEqual(44, cascade.Stages[29].Trees.Length);
            Assert.AreEqual(3, cascade.Stages[29].Trees[43].Length);

            Assert.AreEqual(true, cascade.HasTiltedFeatures);
        }

        [Test]
        public void ParseTest5()
        {
            string fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "haarcascade_eye.xml");
            StreamReader stringReader = new StreamReader(fileName);
            HaarCascade cascade = HaarCascade.FromXml(stringReader);

            Assert.AreEqual(24, cascade.Stages.Length);
            Assert.AreEqual(93, cascade.Stages[23].Trees.Length);
            Assert.AreEqual(1, cascade.Stages[23].Trees[0].Length);

            Assert.AreEqual(false, cascade.HasTiltedFeatures);
        }

        [Test]
        public void ParseTest6()
        {
            string fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "haarcascade_mcs_nose.xml");
            StreamReader stringReader = new StreamReader(fileName);
            HaarCascade cascade = HaarCascade.FromXml(stringReader);

            Assert.AreEqual(20, cascade.Stages.Length);
            Assert.AreEqual(289, cascade.Stages[15].Trees.Length);

            for (int i = 0; i < cascade.Stages.Length; i++)
                Assert.AreEqual(1, cascade.Stages[i].Trees[0].Length);

            Assert.AreEqual(true, cascade.HasTiltedFeatures);

            //  StringWriter sw = new StringWriter();
            //  cascade.ToCode(sw, "NoseCascadeClassifier");
            //  string str = sw.ToString();
        }
    }
}
