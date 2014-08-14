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

using Accord.Math.Wavelets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accord.Math;
namespace Accord.Tests.Wavelets
{


    /// <summary>
    ///This is a test class for CDF97Test and is intended
    ///to contain all CDF97Test Unit Tests
    ///</summary>
    [TestClass()]
    public class CDF97Test
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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
        public void FWT97Test()
        {
            double[] x = new double[32];

            // Makes a fancy cubic signal
            for (int i = 0; i < 32; i++)
                x[i] = 5 + i + 0.4 * i * i - 0.02 * i * i * i;

            double[] original = 
            {
                5.000000,    6.380000,    8.440000,   11.060000,
                14.120000,   17.500000,   21.080000,  24.740000,
                28.360000,   31.820000,   35.000000,  37.780000,
                40.040000,   41.660000,   42.520000,  42.500000,
                41.480000,   39.340000,   35.960000,  31.220000,
                25.000000,   17.180000,   7.640000,   -3.740000, 
               -17.080000,  -32.500000,  -50.120000,  -70.060000, 
               -92.440000, -117.380000, -145.000000, -175.420000 
            };

            double[] wc =
            {
                 7.752539,   12.210715,   20.072860,   29.837663,
                40.055014,   49.367269,   56.416781,   59.845907,
                58.297001,   50.412419,   34.834515,   10.205643,
               -24.831840, -71.635579,  -129.151279, -208.800687,
                 0.157752,   0.000000,     0.000000,    0.000000,
                 0.000000,   0.000000,     0.000000,    0.000000,
                 0.000000,   0.000000,     0.000000,   -0.000000,
                -0.000000,  -0.000000,     4.114999,  -19.449089
            };

            Assert.IsTrue(Matrix.IsEqual(x, original, 0.0001));

            // Do the forward 9/7 transform
            CDF97.FWT97(x);

            Assert.IsTrue(Matrix.IsEqual(x, wc, 0.0001));

            // Do the inverse 9/7 transform
            CDF97.IWT97(x);

            Assert.IsTrue(Matrix.IsEqual(x, original, 0.0001));

        }

        [TestMethod()]
        public void FWT2DTest()
        {
            double[,] x = 
            {
                { 0,  1,  2,  3 },
                { 4,  5,  6,  7 }, 
                { 8,  9, 10, 11 }, 
                {12, 13, 14, 15 }
            };

            double[,] expected = 
            {
   		        {  1.297637585669648,  3.484779276810484, 0.033728238667008574, 0.43254352928579837 },
                { 10.046204350232994, 12.233346041373833, 0.033728245672272984, 0.43254353629106429 },
                {  0.134912951550852,  0.134912953302168, 0.000000000135036058, 0.00000000045438059	},
                {  1.730174114026011,  1.730174115777326, 0.000000001412414811,	0.00000000173175807 },
            };

            int levels = 1;

            double[,] actual = CDF97.FWT97(x, levels);

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 1e-5));

            actual = CDF97.IWT97(x, levels);

            Assert.IsTrue(Matrix.IsEqual(x, actual, 0.001));

        }
    }
}
