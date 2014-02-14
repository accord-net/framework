// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
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
    using System.Drawing;
    using Accord.Imaging.Moments;
    using Accord.Tests.Imaging.Properties;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Imaging.Converters;
    using Accord.Math;
    using AForge.Imaging;
    using Accord.Controls.Imaging;
    using Accord.Imaging;
    using System.Collections.Generic;


    [TestClass()]
    public class HaralickTest
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



        [TestMethod()]
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
                0.999999999999998, 1.4432899320127E-15, 0.624999999999999, 
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
                Assert.AreEqual(actual[i], expected[i], System.Math.Abs(actual[i]) * 1e-10);
        }

        [TestMethod()]
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
                0.00393700787401572, 0.999999999999998, 353791227646.996, 0.999999999999998,
                0.499999999999999, 254, 83238.6962631393, 5.53733426701852, 5.53733426701852,
                0.003921568627451, 1.66533453693773E-15, -1, 0.999992249954468, 0.00393700787401572, 
                0.999999999999998, 353791227646.996, 0.999999999999998, 0.499999999999999, 254,
                83238.6962631393, 5.53733426701852, 5.53733426701852, 0.003921568627451, 
                1.66533453693773E-15, -1, 0.999992249954468, 0.00392156862745099, 0,
                1.45558373073433E+38, 0.999999999999997, 0.999999999999997, 254, 
                83406.410387403, 5.54126354515845, 5.54126354515845, 0.00392156862745098, 
                3.10862446895043E-15, -1, 0.999992310620187, 0.00393700787401572, 
                0.999999999999998, 353791227646.996, 0.999999999999998, 0.499999999999999,
                254, 83238.6962631393, 5.53733426701852, 5.53733426701852, 0.003921568627451, 
                1.66533453693773E-15, -1, 0.999992249954468 
            };

            // string str = actual.ToString(Math.Formats.CSharpArrayFormatProvider.InvariantCulture);

            Assert.AreEqual(52, actual.Length);
            for (int i = 0; i < actual.Length; i++)
                Assert.AreEqual(actual[i], expected[i], System.Math.Abs(actual[i]) * 1e-10);
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
