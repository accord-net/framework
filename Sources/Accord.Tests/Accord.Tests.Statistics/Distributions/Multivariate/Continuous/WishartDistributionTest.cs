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
    public class WishartDistributionTest
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
            // Create a Wishart distribution with the parameters:
            WishartDistribution wishart = new WishartDistribution(

                // Degrees of freedom
                degreesOfFreedom: 7,

                // Scale parameter
                scale: new double[,] 
                {
                    { 4, 1, 1 },  
                    { 1, 2, 2 },  // (must be symmetric and positive definite)
                    { 1, 2, 6 },
                }
            );

            // Common measures
            double[] var = wishart.Variance;  // { 224, 56, 504 }
            double[,] cov = wishart.Covariance;   // see below
            double[,] meanm = wishart.MeanMatrix; // see below
            
            //         224  63  175             28  7   7 
            //   cov =  63  56  112     mean =   7  14  14
            //         175 112  504              7  14  42

            // (the above matrix representations have been transcribed to text using)
            string scov = cov.ToString(DefaultMatrixFormatProvider.InvariantCulture);
            string smean = meanm.ToString(DefaultMatrixFormatProvider.InvariantCulture);

            // For compatibility reasons, .Mean stores a flattened mean matrix
            double[] mean = wishart.Mean; // { 28, 7, 7, 7, 14, 14, 7, 14, 42 }


            // Probability density functions
            double pdf = wishart.ProbabilityDensityFunction(new double[,] 
            {
                { 8, 3, 1 },
                { 3, 7, 1 },   //   0.000000011082455043473361
                { 1, 1, 8 },
            });

            double lpdf = wishart.LogProbabilityDensityFunction(new double[,] 
            {
                { 8, 3, 1 },
                { 3, 7, 1 },   // -18.317902605850534
                { 1, 1, 8 },
            });


            Assert.AreEqual(28.0, mean[0]);
            Assert.AreEqual(7.0, mean[1]);
            Assert.AreEqual(7.0, mean[3]);
            Assert.AreEqual(14.0, mean[4]);
            Assert.AreEqual(224.0, var[0]);
            Assert.AreEqual(56.0, var[1]);
            Assert.AreEqual(504.0, var[2]);
            Assert.AreEqual(224.0, cov[0, 0]);
            Assert.AreEqual(63.0, cov[0, 1]);
            Assert.AreEqual(63.0, cov[1, 0]);
            Assert.AreEqual(56.0, cov[1, 1]);
            Assert.AreEqual(0.00000001108245504347336, pdf);
            Assert.AreEqual(-18.317902605850534, lpdf);
        }

    }
}
