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

namespace Accord.Tests.Statistics
{
    using Accord.Statistics.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    using Accord.Math;

    [TestClass()]
    public class BartlettTestTest
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
        public void BartlettTestConstructorTest()
        {
            // Example from NIST/SEMATECH e-Handbook of Statistical Methods,
            // http://www.itl.nist.gov/div898/handbook/eda/section3/eda357.htm

            BartlettTest target = new BartlettTest(samples);

            Assert.AreEqual(9, target.DegreesOfFreedom);
            Assert.AreEqual(20.785873428064864, target.Statistic, 1e-10);
            Assert.IsFalse(double.IsNaN(target.Statistic));
        }


        public static double[][] samples =
        {
            // Data Used for Chi-Square Test for the Variance,
            // NIST/SEMATECH e-Handbook of Statistical Methods,
            // http://www.itl.nist.gov/div898/handbook/, 2012

            #region 
            new double[] 
            {
                1.006, 0.996, 0.998, 1.000, 0.992,
                0.993, 1.002, 0.999, 0.994, 1.000,
            },
            new double[] 
            {
                0.998, 1.006, 1.000, 1.002, 0.997, 
                0.998, 0.996, 1.000, 1.006, 0.988,
            },
            new double[] 
            {
                0.991, 0.987, 0.997, 0.999, 0.995,
                0.994, 1.000, 0.999, 0.996, 0.996, 
            },
            new double[] 
            {
                1.005, 1.002, 0.994, 1.000, 0.995, 
                0.994, 0.998, 0.996, 1.002, 0.996, 
            },
            new double[] 
            {
                0.998, 0.998, 0.982, 0.990, 1.002, 
                0.984, 0.996, 0.993, 0.980, 0.996, 
            },
            new double[] 
            {
                1.009, 1.013, 1.009, 0.997, 0.988, 
                1.002, 0.995, 0.998, 0.981, 0.996, 
            },
            new double[] 
            {
                0.990,1.004,  0.996, 1.001, 0.998, 
                1.000, 1.018, 1.010, 0.996, 1.002, 

            },
            new double[] 
            {
                0.998, 1.000, 1.006, 1.000, 1.002,
                0.996, 0.998, 0.996, 1.002, 1.006,
            },
            new double[] 
            {
                1.002, 0.998, 0.996, 0.995, 0.996,
                1.004, 1.004, 0.998, 0.999, 0.991,
            },
            new double[] 
            {
                0.991, 0.995, 0.984, 0.994, 0.997,
                0.997, 0.991,  0.998, 1.004, 0.997,
            }
            #endregion
        };

    }
}
