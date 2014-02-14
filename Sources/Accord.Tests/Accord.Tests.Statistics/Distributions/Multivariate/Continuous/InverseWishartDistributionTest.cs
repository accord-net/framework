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
    public class InverseWishartDistributionTest
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
            // Create a Inverse Wishart with the parameters
            var invWishart = new InverseWishartDistribution(

                // Degrees of freedom
                degreesOfFreedom: 4,

                // Scale parameter
                inverseScale: new double[,] 
                {
                     {  1.7, -0.2 },
                     { -0.2,  5.3 },
                }
            );

            // Common measures
            double[] var = invWishart.Variance;  // { -3.4, -10.6 }
            double[,] cov = invWishart.Covariance;  // see below
            double[,] mmean = invWishart.MeanMatrix; // see below
            
            //        cov                mean
            //   -5.78   -4.56        1.7  -0.2 
            //   -4.56  -56.18       -0.2   5.3 

            // (the above matrix representations have been transcribed to text using)
            string scov = cov.ToString(DefaultMatrixFormatProvider.InvariantCulture);
            string smean = mmean.ToString(DefaultMatrixFormatProvider.InvariantCulture);

            // For compatibility reasons, .Mean stores a flattened mean matrix
            double[] mean = invWishart.Mean; // { 1.7, -0.2, -0.2, 5.3 }


            // Probability density functions
            double pdf = invWishart.ProbabilityDensityFunction(new double[,] 
            {
                {  5.2,  0.2 }, // 0.000029806281690351203
                {  0.2,  4.2 },
            });

            double lpdf = invWishart.LogProbabilityDensityFunction(new double[,] 
            {
                {  5.2,  0.2 }, // -10.420791391688828
                {  0.2,  4.2 },
            });


            Assert.AreEqual(pdf, System.Math.Exp(lpdf), 1e-6);
            Assert.AreEqual(1.7, mean[0]);
            Assert.AreEqual(-0.2, mean[1]);
            Assert.AreEqual(-0.2, mean[2]);
            Assert.AreEqual(5.3, mean[3]);
            Assert.AreEqual(-3.4, var[0]);
            Assert.AreEqual(-10.6, var[1]);
            Assert.AreEqual(-5.7799999999999994, cov[0, 0]);
            Assert.AreEqual(-4.5649999999999995, cov[0, 1]);
            Assert.AreEqual(-4.5649999999999995, cov[1, 0]);
            Assert.AreEqual(-56.18, cov[1, 1]);
            Assert.AreEqual(0.000029806281690351203, pdf);
            Assert.AreEqual(-10.420791391688828, lpdf);
        }

    }
}
