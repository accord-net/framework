// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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
    using Accord;
    using Accord.Imaging;
    using Accord.MachineLearning;
    using Accord.Math;
    using Accord.Math.Distances;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;

    [TestFixture]
    public class BagOfVisualWordsTest
    {

        // Load some test images
        public static Bitmap[] GetImages()
        {
            Bitmap[] images = 
            { 
                Accord.Imaging.Image.Clone(Properties.Resources.flower01),
                Accord.Imaging.Image.Clone(Properties.Resources.flower02),
                Accord.Imaging.Image.Clone(Properties.Resources.flower03),
                Accord.Imaging.Image.Clone(Properties.Resources.flower04),
                Accord.Imaging.Image.Clone(Properties.Resources.flower05),
                Accord.Imaging.Image.Clone(Properties.Resources.flower06),
            };

            return images;
        }

        [Test]
        public void BagOfVisualWordsConstructorTest()
        {
            var images = GetImages();

            BagOfVisualWords bow = new BagOfVisualWords(10);
            bow.ParallelOptions.MaxDegreeOfParallelism = 1;
#pragma warning disable 612, 618
            var points = bow.Compute(images, 1e-3);
#pragma warning restore 612, 618
            Assert.AreEqual(10, bow.NumberOfWords);
            Assert.AreEqual(6, points.Length);

            Assert.AreEqual(409, points[0].Count);
            Assert.AreEqual(727, points[1].Count);
            Assert.AreEqual(552, points[2].Count);
            Assert.AreEqual(460, points[3].Count);
            Assert.AreEqual(719, points[4].Count);
            Assert.AreEqual(1265, points[5].Count);

            double tol = 1e-7;
            Assert.AreEqual(388.04225639880224, points[0][0].X, tol);
            Assert.AreEqual(105.9954439039073, points[0][0].Y, tol);

            Assert.AreEqual(335.62395561144433, points[3][7].X, tol);
            Assert.AreEqual(152.14505651866821, points[2][3].Y, tol);

            Assert.AreEqual(573.691355494602, points[2][52].X, tol);
            Assert.AreEqual(153.6650841848263, points[1][11].Y, tol);

            Assert.AreEqual(289.54728415724327, points[0][42].X, tol);
            Assert.AreEqual(373.99402540151056, points[4][125].Y, tol);

            foreach (var point in points)
            {
                foreach (var p in point)
                {
                    Assert.IsFalse(double.IsNaN(p.X));
                    Assert.IsFalse(double.IsNaN(p.Y));
                }
            }
        }

        [Test]
        public void BagOfVisualWordsConstructorTest3()
        {
            var images = GetImages();

            MoravecCornersDetector moravec = new MoravecCornersDetector();
            CornerFeaturesDetector detector = new CornerFeaturesDetector(moravec);

            var bow = new BagOfVisualWords<CornerFeaturePoint>(detector, numberOfWords: 10);
            bow.ParallelOptions.MaxDegreeOfParallelism = 1;

#pragma warning disable 612, 618
            var points = bow.Compute(images, 1e-3);
#pragma warning restore 612, 618
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

        [Test]
        public void GetFeatureVectorTest()
        {
            var images = GetImages();

            Accord.Math.Random.Generator.Seed = 0;
            Bitmap image = new Bitmap(images[0]);

            // The Bag-of-Visual-Words model converts arbitrary-size images 
            // into fixed-length feature vectors. In this example, we will
            // be setting the codebook size to 10. This means all generated
            // feature vectors will have the same length of 10.

            // Create a new Bag-of-Visual-Words (BoW) model
            BagOfVisualWords bow = new BagOfVisualWords(10);

            bow.ParallelOptions.MaxDegreeOfParallelism = 1;

            // Compute the model using
            // a set of training images
            bow.Compute(images);

            // After this point, we will be able to translate
            // images into double[] feature vectors using
            double[] feature = bow.GetFeatureVector(image);

            Assert.AreEqual(10, feature.Length);


            double[][] expected =
            {
                new double[] { 47, 44, 42, 4, 23, 22, 28, 53, 50, 96 },
                new double[] { 26, 91, 71, 49, 99, 70, 59, 28, 155, 79 },
                new double[] { 71, 34, 51, 33, 53, 25, 44, 64, 32, 145 } 
            };

            double[][] actual = new double[expected.Length][];
            for (int i = 0; i < actual.Length; i++)
                actual[i] = bow.GetFeatureVector(images[i]);

            //string str = actual.ToString(CSharpJaggedMatrixFormatProvider.InvariantCulture);
            //Assert.IsNullOrEmpty(str);

            for (int i = 0; i < actual.Length; i++)
                for (int j = 0; j < actual[i].Length; j++)
                    Assert.IsTrue(expected[i].Contains(actual[i][j]));
        }

        [Test, Timeout(600 * 1000)]
        public void SerializeTest()
        {
            var images = GetImages();

            Accord.Math.Random.Generator.Seed = 0;

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

        [Test]
        public void SerializeTest2()
        {
            var images = GetImages();

            Accord.Math.Random.Generator.Seed = 0;

            FastCornersDetector fast = new FastCornersDetector();

            FastRetinaKeypointDetector freak = new FastRetinaKeypointDetector(fast);

            var kmodes = new KModes<byte>(5, new Hamming());

            var bow = new BagOfVisualWords<FastRetinaKeypoint, byte[]>(freak, kmodes);

            bow.Compute(images);


            double[][] expected = new double[images.Length][];
            for (int i = 0; i < expected.Length; i++)
                expected[i] = bow.GetFeatureVector(images[i]);

            MemoryStream stream = new MemoryStream();
            BinaryFormatter fmt = new BinaryFormatter();
            fmt.Serialize(stream, bow);
            stream.Seek(0, SeekOrigin.Begin);
            bow = (BagOfVisualWords<FastRetinaKeypoint, byte[]>)fmt.Deserialize(stream);

            double[][] actual = new double[expected.Length][];
            for (int i = 0; i < actual.Length; i++)
                actual[i] = bow.GetFeatureVector(images[i]);


            Assert.IsTrue(expected.IsEqual(actual));
        }


        [Test, Ignore]
        public void LargeTest()
        {
            // Requires data from the National Data Science bowl
            // https://github.com/accord-net/framework/issues/58

            var trainingDirectory = @"C:\Users\CésarRoberto\Downloads\train\train\";

            var images = LabeledImages.FromDirectory(trainingDirectory).ToArray();

            var binarySplit = new BinarySplit(32);

            var bow = new BagOfVisualWords(binarySplit);

            bow.Compute(images.Select(i => i.Item).Take(50).ToArray());

        }

        public interface ILabeledItem<T>
        {
            string Category { get; }

            T Item { get; }
        }

        public class LabeledImages : ILabeledItem<Bitmap>
        {
            public LabeledImages(string category, string filename)
            {
                Category = category;
                Filename = filename;
            }

            public string Category { get; private set; }

            public string Filename { get; private set; }

            public Bitmap Item
            {
                get
                {
                    using (var sourceImage = System.Drawing.Image.FromFile(Filename))
                    {
                        var targetImage = new Bitmap(sourceImage.Width, sourceImage.Height, PixelFormat.Format32bppArgb);
                        using (var canvas = Graphics.FromImage(targetImage))
                        {
                            canvas.DrawImageUnscaled(sourceImage, 0, 0);
                        }
                        return targetImage;
                    }
                }
            }

            public static IEnumerable<LabeledImages> FromDirectory(string path)
            {
                return Directory.EnumerateDirectories(path)
                    .SelectMany(dir => Directory.EnumerateFiles(dir)
                        .Select(file => new LabeledImages(Path.GetFileName(dir), file)));
            }
        }

    }
}
