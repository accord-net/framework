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
    using Accord.Statistics.Analysis;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Accord.Math;

    [TestClass()]
    public class WeightedConfusionMatrixTest
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
        public void WeightedConfusionMatrixConstructorTest()
        {
            // Sample data from Fleiss, Cohen and Everitt (1968), Large sample standard errors
            // of kappa and weighted kappa. Psychological Bulletin, Vol. 72, No. 5, 323-327

            double[,] matrix =
            {
                { 0.53, 0.05, 0.02 },
                { 0.11, 0.14, 0.05 },
                { 0.01, 0.06, 0.03 },
            };

            double[,] weights =
            {
                { 1.0000, 0.0000, 0.4444 },
                { 0.0000, 1.0000, 0.6667 },
                { 0.4444, 0.6667, 1.0000 },
            };

            WeightedConfusionMatrix target = new WeightedConfusionMatrix(matrix, weights, samples: 200);


            Assert.AreEqual(0.787, target.WeightedOverallAgreement, 1e-3);
            Assert.AreEqual(0.567, target.WeightedChanceAgreement, 1e-3);
            Assert.AreEqual(0.508, target.WeightedKappa, 1e-3);

            Assert.AreEqual(0.00324823, target.WeightedVariance, 1e-5);
            Assert.AreEqual(0.004270, target.WeightedVarianceUnderNull, 1e-3);
        }

        [TestMethod()]
        public void WeightedConfusionMatrixConstructorTest2()
        {
            double[,] matrix =
            {
                { 0.53, 0.05, 0.02 },
                { 0.11, 0.14, 0.05 },
                { 0.01, 0.06, 0.03 },
            };

            double[,] weights =
            {
                { 1.00, 0.50, 0.44 },
                { 0.90, 1.00, 0.56 },
                { 0.75, 0.66, 1.00 },
            };

            WeightedConfusionMatrix target = new WeightedConfusionMatrix(matrix, 
                weights, samples: 200);


            Assert.AreEqual(0.4453478, target.WeightedKappa, 1e-5);
            Assert.AreEqual(0.005535633, target.WeightedVariance, 1e-5);
        }
    }
}
