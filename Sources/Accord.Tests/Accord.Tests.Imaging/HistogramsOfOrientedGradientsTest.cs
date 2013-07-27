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
    using System.Drawing;
    using Accord.Imaging.Moments;
    using Accord.Tests.Imaging.Properties;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Imaging.Converters;
    using Accord.Math;
    using AForge.Imaging;
    using Accord.Controls.Imaging;
    using Accord.Imaging;


    [TestClass()]
    public class HistogramsOfOrientedGradientsTest
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


        [TestMethod()]
        public void MagnitudeDirectionTest()
        {
            byte[,] gradient = new byte[255, 255];

            for (int i = 0; i < 255; i++)
                for (int j = 0; j < 255; j++)
                    gradient[i, j] = (byte)j;

            UnmanagedImage output;
            new MatrixToImage().Convert(gradient, out output);

            //ImageBox.Show(output);
            HistogramsOfOrientedGradients hog = new HistogramsOfOrientedGradients();
            var result = hog.ProcessImage(output);

            float[,] actualDir = hog.Direction;
            Assert.AreEqual(255, actualDir.GetLength(0));
            Assert.AreEqual(255, actualDir.GetLength(1));

            for (int i = 0; i < 255; i++)
                for (int j = 0; j < 255; j++)
                    Assert.AreEqual(0, actualDir[i, j]);

            float[,] actualMag = hog.Magnitude;
            Assert.AreEqual(255, actualMag.GetLength(0));
            Assert.AreEqual(255, actualMag.GetLength(1));

            for (int i = 0; i < 255; i++)
            {
                for (int j = 0; j < 255; j++)
                {
                    if (i == 0 || j == 0 || i == 254 || j == 254)
                        Assert.AreEqual(0, actualMag[i, j]);
                    else
                        Assert.AreEqual(1, actualMag[i, j]);
                }
            }
        }
    }
}
