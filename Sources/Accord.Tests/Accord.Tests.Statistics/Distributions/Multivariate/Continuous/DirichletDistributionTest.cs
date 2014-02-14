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
    using Accord.Statistics.Distributions.Multivariate;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Math;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics;
    using System.Globalization;

    [TestClass()]
    public class DirichletDistributionTest
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
        public void ConstructorTest4()
        {
            // Create a Dirichlet with the following concentrations
            var dirich = new DirichletDistribution(0.42, 0.57, 1.2);

            // Common measures
            double[] mean = dirich.Mean;     // { 0.19, 0.26, 0.54 }
            double[] median = dirich.Median; // { 0.19, 0.26, 0.54 }
            double[] var = dirich.Variance;  // { 0.048, 0.060, 0.077 }
            double[,] cov = dirich.Covariance; // see below


            //       0.0115297440926238 0.0156475098399895 0.0329421259789253 
            // cov = 0.0156475098399895 0.0212359062114143 0.0447071709713986 
            //       0.0329421259789253 0.0447071709713986 0.0941203599397865

            // (the above matrix representation has been transcribed to text using)
            string str = cov.ToString(DefaultMatrixFormatProvider.InvariantCulture);
            

            // Probability mass functions
            double pdf1 = dirich.ProbabilityDensityFunction(new double[] { 2, 5 }); // 0.12121671541846207
            double pdf2 = dirich.ProbabilityDensityFunction(new double[] { 4, 2 }); // 0.12024840322466089
            double pdf3 = dirich.ProbabilityDensityFunction(new double[] { 3, 7 }); // 0.082907634905068528
            double lpdf = dirich.LogProbabilityDensityFunction(new double[] { 3, 7 }); // -2.4900281233124044


            Assert.AreEqual(0.19178082191780821, mean[0]);
            Assert.AreEqual(0.26027397260273971, mean[1]);
            Assert.AreEqual(0.547945205479452, median[2]);
            Assert.AreEqual(0.19178082191780821, median[0]);
            Assert.AreEqual(0.26027397260273971, median[1]);
            Assert.AreEqual(0.547945205479452, median[2]);
            Assert.AreEqual(0.048589635818914775, var[0]);
            Assert.AreEqual(0.060354680811388089, var[1]);
            Assert.AreEqual(0.077649296950323854, var[2]);
            Assert.AreEqual(0.011529744092623844, cov[0, 0]);
            Assert.AreEqual(0.015647509839989502, cov[0, 1]);
            Assert.AreEqual(0.015647509839989502, cov[1, 0]);
            Assert.AreEqual(0.021235906211414326, cov[1, 1]);
            Assert.AreEqual(0.12121671541846207, pdf1);
            Assert.AreEqual(0.12024840322466089, pdf2);
            Assert.AreEqual(0.082907634905068528, pdf3);
            Assert.AreEqual(-2.4900281233124044, lpdf);
        }

    }
}
