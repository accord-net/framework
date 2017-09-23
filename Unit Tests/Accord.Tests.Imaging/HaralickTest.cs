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
    using Accord.Imaging;
    using Accord.Imaging.Converters;
    using Accord.Imaging.Textures;
    using Accord.Math;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    [TestFixture]
    public class HaralickTest
    {
        [Test]
        public void apply_test()
        {
            #region doc_apply
            // Ensure results can be reproduced:
            Accord.Math.Random.Generator.Seed = 0;

            // Let's generate a square image containing a wooden texture:
            Bitmap wood = new WoodTexture().Generate(512, 512).ToBitmap();

            // Show the image or save it to disk so we can visualize it later:
            // ImageBox.Show(wood);   // visualizes it
            // wood.Save("wood.png"); // saves to disk

            // Create a new Haralick extractor:
            Haralick haralick = new Haralick()
            {
                Mode = HaralickMode.AverageWithRange,
                CellSize = 0 // use the entire image
            };

            // Extract the descriptors from the texture image
            List<double[]> descriptors = haralick.ProcessImage(wood);

            // If desired, we can obtain the GLCM for the image:
            GrayLevelCooccurrenceMatrix glcm = haralick.Matrix;

            // Since we specified CellSize = 0 when we created the Haralick object 
            // above, the Haralick extractor should have been computed considering 
            // the entire image and therefore it will return just one descriptor:

            double[] descriptor = descriptors[0]; // should be similar to
            // { 
            //      0.000452798780435224, 0.000176452252541844, 2185.7835571369, 1055.78890910489, 
            //      1819136356.0869, 31272466.1144943, 162.811942113822, 0.0294747289650559, 
            //      0.0600645989906271, 0.014234218481406, 325.617817549069, 0.0288571168872522,
            //      124520.583178736, 1051.39892825528, 6.04466262360143, 0.0166538281740083, 
            //      9.9387182389777, 0.188948901162149, 4.21006097507467E-05, 1.50197212765552E-05, 
            //      4.51859838204172, 0.250994550822876, -0.127589342042208, 0.0356360581349288, 
            //      0.858214424241827, 0.0572031826003976
            // }
            #endregion

            string str = descriptor.ToCSharp();
            double[] expected = new double[] { 0.000452798780435224, 0.000176452252541844, 2185.7835571369, 1055.78890910489, 1819136356.0869, 31272466.1144943, 162.811942113822, 0.0294747289650559, 0.0600645989906271, 0.014234218481406, 325.617817549069, 0.0288571168872522, 124520.583178736, 1051.39892825528, 6.04466262360143, 0.0166538281740083, 9.9387182389777, 0.188948901162149, 4.21006097507467E-05, 1.50197212765552E-05, 4.51859838204172, 0.250994550822876, -0.127589342042208, 0.0356360581349288, 0.858214424241827, 0.0572031826003976 };

            Assert.IsTrue(expected.IsEqual(descriptor, rtol: 1e-6));
            Assert.AreEqual(1, descriptors.Count);
            Assert.AreEqual(new HashSet<CooccurrenceDegree>
            {
                CooccurrenceDegree.Degree0,
                CooccurrenceDegree.Degree45,
                CooccurrenceDegree.Degree90,
                CooccurrenceDegree.Degree135
            }, haralick.Degrees);
        }

        [Test]
        public void ComputeTest()
        {
            int size = 255;
            UnmanagedImage output = createGradient(size);

            Haralick haralick = new Haralick()
            {
                Mode = HaralickMode.AverageWithRange
            };

            Assert.AreEqual(13, haralick.Features);
            Assert.AreEqual(4, haralick.Degrees.Count);

            List<double[]> result = haralick.ProcessImage(output);

            GrayLevelCooccurrenceMatrix glcm = haralick.Matrix;
            HaralickDescriptorDictionary[,] features = haralick.Descriptors;

            Assert.AreEqual(1, features.GetLength(0));
            Assert.AreEqual(1, features.GetLength(1));


            Assert.AreEqual(1, result.Count);
            double[] actual = result[0];
            double[] expected =
            {
                0.00393314806237454, 1.54392465647286E-05, 0.749999999999999,
                0.999999999999998, 3.63895932683582E+37, 1.45558373073433E+38,
                126.87498462129943, 1, 0.624999999999999,
                0.499999999999998, 254, 3.12638803734444E-13, 83280.6247942052,
                167.714124263744, 5.5383165865535, 0.00392927813992738,
                5.5383165865535, 0.00392927813992738, 0.00392156862745099,
                1.12757025938492E-17, 2.02615701994091E-15,
                1.4432899320127E-15, -1, 0, 0.999992265120898,
                6.06657187818271E-08
            };

            // string str = actual.ToString(Math.Formats.CSharpArrayFormatProvider.InvariantCulture);

            Assert.AreEqual(26, actual.Length);
            for (int i = 0; i < actual.Length; i++)
                Assert.AreEqual(expected[i], actual[i], System.Math.Abs(expected[i]) * 1e-10);
        }

        [Test]
        public void ComputeTest2()
        {
            int size = 255;
            UnmanagedImage output = createGradient(size);

            Haralick haralick = new Haralick()
            {
                Mode = HaralickMode.Combine
            };

            Assert.AreEqual(13, haralick.Features);
            Assert.AreEqual(4, haralick.Degrees.Count);

            List<double[]> result = haralick.ProcessImage(output);

            GrayLevelCooccurrenceMatrix glcm = haralick.Matrix;
            HaralickDescriptorDictionary[,] features = haralick.Descriptors;

            Assert.AreEqual(1, features.GetLength(0));
            Assert.AreEqual(1, features.GetLength(1));


            Assert.AreEqual(1, result.Count);
            double[] actual = result[0];
            double[] expected =
            {
                0.00393700787401572, 0.999999999999998, 353791227646.996,
                126.499984621299, 0.499999999999999, 254, 83238.6962631393,
                5.53733426701852, 5.53733426701852, 0.003921568627451,
                1.66533453693773E-15, -1, 0.999992249954468, 0.00393700787401572,
                0.999999999999998, 353791227646.996, 126.499984621299,
                0.499999999999999, 254, 83238.6962631393, 5.53733426701852,
                5.53733426701852, 0.003921568627451, 1.66533453693773E-15,
                -1, 0.999992249954468, 0.00392156862745099, 0, 1.45558373073433E+38,
                126.999984621299, 0.999999999999997, 254, 83406.410387403,
                5.54126354515845, 5.54126354515845, 0.00392156862745098,
                3.10862446895043E-15, -1, 0.999992310620187, 0.00393700787401572,
                0.999999999999998, 353791227646.996, 127.499984621299,
                0.499999999999999, 254, 83238.6962631393, 5.53733426701852,
                5.53733426701852, 0.003921568627451, 1.66533453693773E-15,
                -1, 0.999992249954468
            };

            string str = actual.ToString(Accord.Math.CSharpArrayFormatProvider.InvariantCulture);
            Assert.IsNotNull(str);

            Assert.AreEqual(52, actual.Length);
            for (int i = 0; i < actual.Length; i++)
                Assert.AreEqual(expected[i], actual[i], System.Math.Abs(expected[i]) * 1e-10);
        }

        private static UnmanagedImage createGradient(int size)
        {
            byte[,] gradient = new byte[size, size];

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    gradient[i, j] = (byte)j;

            UnmanagedImage output;
            new MatrixToImage().Convert(gradient, out output);
            return output;
        }

    }
}
