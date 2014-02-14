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
    public class LocalBinaryPatternTest
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
            byte[,] gradient = new byte[255, 255];

            for (int i = 0; i < 255; i++)
                for (int j = 0; j < 255; j++)
                    gradient[i, j] = (byte)j;

            UnmanagedImage output;
            new MatrixToImage().Convert(gradient, out output);

            LocalBinaryPattern lbp = new LocalBinaryPattern();
            List<double[]> result = lbp.ProcessImage(output);

            int[,] actualPatterns = lbp.Patterns;
            Assert.AreEqual(255, actualPatterns.GetLength(0));
            Assert.AreEqual(255, actualPatterns.GetLength(1));

            for (int i = 0; i < 255; i++)
            {
                for (int j = 0; j < 255; j++)
                {
                    if (j == 0 || i == 0 || i == 254 || j == 254)
                    {
                        Assert.AreEqual(0, actualPatterns[i, j]);
                    }
                    else
                    {
                        Assert.AreEqual(7, actualPatterns[i, j]);
                    }
                }
            }

            Assert.AreEqual(196, result.Count);
        }

    }
}
