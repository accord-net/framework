// Accord Unit Tests
// The Accord.NET Framework
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2013
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
    using Accord.Imaging;
    using Accord.Math;
    using AForge.Imaging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Drawing;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    [TestClass()]
    public class BagOfVisualWordsTest
    {


        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        // Load some test images
        static Bitmap[] images =
        {
            Properties.Resources.flower01,
            Properties.Resources.flower02,
            Properties.Resources.flower03,
            Properties.Resources.flower04,
            Properties.Resources.flower05,
            Properties.Resources.flower06,
        };

        [TestMethod()]
        public void BagOfVisualWordsConstructorTest()
        {
            BagOfVisualWords bow = new BagOfVisualWords(10);

            var points = bow.Compute(images, 1e-3);

            Assert.AreEqual(10, bow.NumberOfWords);
            Assert.AreEqual(6, points.Length);

            Assert.AreEqual(406, points[0].Count);
            Assert.AreEqual(727, points[1].Count);
            Assert.AreEqual(549, points[2].Count);
            Assert.AreEqual(458, points[3].Count);
            Assert.AreEqual(723, points[4].Count);
            Assert.AreEqual(1263, points[5].Count);

            Assert.AreEqual(388.043776954555, points[0][0].X);
            Assert.AreEqual(105.99327164889745, points[0][0].Y);

            Assert.AreEqual(335.64548481033881, points[3][7].X);
            Assert.AreEqual(152.14505651866821, points[2][3].Y);

            Assert.AreEqual(573.691355494602, points[2][52].X);
            Assert.AreEqual(153.6650841848263, points[1][11].Y);

            Assert.AreEqual(573.03087205188058, points[0][42].X);
            Assert.AreEqual(374.27580307739436, points[4][125].Y);
        }

        [TestMethod()]
        public void BagOfVisualWordsConstructorTest3()
        {
            MoravecCornersDetector moravec = new MoravecCornersDetector();
            CornerFeaturesDetector detector = new CornerFeaturesDetector(moravec);

            var bow = new BagOfVisualWords<CornerFeaturePoint>(detector, numberOfWords: 10);

            var points = bow.Compute(images, 1e-3);

            double[] vector = bow.GetFeatureVector(images[0]);

            Assert.AreEqual(10, bow.NumberOfWords);
            Assert.AreEqual(6, points.Length);
            Assert.AreEqual(10, vector.Length);

            Assert.AreEqual(2800, points[0].Count);
            Assert.AreEqual(4532, points[1].Count);
            Assert.AreEqual(2282, points[2].Count);
            Assert.AreEqual(1173, points[3].Count);
            Assert.AreEqual(4860, points[4].Count);
            Assert.AreEqual(5730, points[5].Count);

            Assert.AreEqual(596, points[0][0].Descriptor[0]);
            Assert.AreEqual(51, points[0][0].Descriptor[1]);
            Assert.AreEqual(points[0][0].Descriptor[0], points[0][0].X);
            Assert.AreEqual(points[0][0].Descriptor[1], points[0][0].Y);

            Assert.AreEqual(461, points[3][7].Descriptor[0]);
            Assert.AreEqual(8, points[2][3].Descriptor[1]);

            Assert.AreEqual(991, points[2][52].Descriptor[0]);
            Assert.AreEqual(82, points[1][11].Descriptor[1]);

            Assert.AreEqual(430, points[0][42].Descriptor[0]);
            Assert.AreEqual(135, points[4][125].Descriptor[1]);
        }


        [TestMethod()]
        public void GetFeatureVectorTest()
        {
            Accord.Math.Tools.SetupGenerator(0);
            Bitmap image = images[0];

            // The Bag-of-Visual-Words model converts arbitrary-size images 
            // into fixed-length feature vectors. In this example, we will
            // be setting the codebook size to 10. This means all generated
            // feature vectors will have the same length of 10.

            // Create a new Bag-of-Visual-Words (BoW) model
            BagOfVisualWords bow = new BagOfVisualWords(10);

            // Compute the model using
            // a set of training images
            bow.Compute(images);

            // After this point, we will be able to translate
            // images into double[] feature vectors using
            double[] feature = bow.GetFeatureVector(image);

            Assert.AreEqual(10, feature.Length);


            double[][] expected = 
            {
                new double[] { 26, 58, 102, 24, 53, 22, 47, 29, 42, 3 },
                new double[] { 61, 135, 90, 101, 28, 48, 26, 105, 71, 62 },
                new double[] { 47, 36, 138, 56, 61, 22, 71, 30, 55, 33 },
            };

            double[][] actual = new double[expected.Length][];
            for (int i = 0; i < actual.Length; i++)
                actual[i] = bow.GetFeatureVector(images[i]);

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [TestMethod()]
        public void SerializeTest()
        {
            Accord.Math.Tools.SetupGenerator(0);

            BagOfVisualWords bow = new BagOfVisualWords(10);

            bow.Compute(images);

            double[][] expected = new double[images.Length][];
            for (int i = 0; i < expected.Length; i++)
                expected[i] = bow.GetFeatureVector(images[i]);

            MemoryStream stream = new MemoryStream();
            BinaryFormatter fmt = new BinaryFormatter();
            fmt.Serialize(stream, bow);
            stream.Seek(0, SeekOrigin.Begin);
            bow = (BagOfVisualWords)fmt.Deserialize(stream);

            double[][] actual = new double[expected.Length][];
            for (int i = 0; i < actual.Length; i++)
                actual[i] = bow.GetFeatureVector(images[i]);


            Assert.IsTrue(expected.IsEqual(actual));
        }

    }
}
