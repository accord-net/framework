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
    using Accord.Math;
    using Accord.Statistics.Analysis;
    using Accord.Statistics.Links;
    using Accord.Statistics.Models.Regression;
    using Accord.Statistics.Models.Regression.Fitting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class ProbitRegressionTest
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
            // Example from http://bayes.bgsu.edu/bcwr/vignettes/probit_regression.pdf

            double[][] input =
            {
                new double[] { 525 },
                new double[] { 533 },
                new double[] { 545 },
                new double[] { 582 },
                new double[] { 581 },
                new double[] { 576 },
                new double[] { 572 },
                new double[] { 609 },
                new double[] { 559 },
                new double[] { 543 },
                new double[] { 576 },
                new double[] { 525 }, 
                new double[] { 574 }, 
                new double[] { 582 }, 
                new double[] { 574 }, 
                new double[] { 471 }, 
                new double[] { 595 }, 
                new double[] { 557 }, 
                new double[] { 557 }, 
                new double[] { 584 }, 
                new double[] { 599 }, 
                new double[] { 517 }, 
                new double[] { 649 },
                new double[] { 584 }, 
                new double[] { 463 }, 
                new double[] { 591 }, 
                new double[] { 488 }, 
                new double[] { 563 }, 
                new double[] { 553 }, 
                new double[] { 549 }
            };

            double[] output =
            {
                0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0, 0, 1, 1, 0, 1, 1, 0, 1, 0, 1, 1, 1
            };


            var regression = new GeneralizedLinearRegression(new ProbitLinkFunction(), inputs: 1);

            var teacher = new IterativeReweightedLeastSquares(regression);


            double delta = 0;
            do
            {
                // Perform an iteration
                delta = teacher.Run(input, output);

            } while (delta > 1e-6);

            

            Assert.AreEqual(2, regression.Coefficients.Length);
            Assert.AreEqual(-17.6984, regression.Coefficients[0], 1e-4);
            Assert.AreEqual(0.03293, regression.Coefficients[1], 1e-4);

            Assert.AreEqual(2, regression.StandardErrors.Length);
            Assert.AreEqual(9.2731983954911374, regression.StandardErrors[0], 1e-6);
            Assert.AreEqual(0.016768779446085, regression.StandardErrors[1], 1e-6);
        }

        [TestMethod()]
        public void ComputeTest2()
        {
            double[][] input =
            {
                new double[] { 55, 0 }, // 0 - no cancer
                new double[] { 28, 0 }, // 0
                new double[] { 65, 1 }, // 0
                new double[] { 46, 0 }, // 1 - have cancer
                new double[] { 86, 1 }, // 1
                new double[] { 56, 1 }, // 1
                new double[] { 85, 0 }, // 0
                new double[] { 33, 0 }, // 0
                new double[] { 21, 1 }, // 0
                new double[] { 42, 1 }, // 1
            };

            double[] output =
            {
                0, 0, 0, 1, 1, 1, 0, 0, 0, 1
            };


            var regression = new GeneralizedLinearRegression(new ProbitLinkFunction(), inputs: 2);

            var teacher = new IterativeReweightedLeastSquares(regression);

            double delta = 0;
            do
            {
                // Perform an iteration
                delta = teacher.Run(input, output);

            } while (delta > 0.001);


            Assert.AreEqual(3, regression.Coefficients.Length);
            Assert.AreEqual(-1.4807594445304693, regression.Coefficients[0],1e-10);
            Assert.AreEqual(0.012417175632016827, regression.Coefficients[1], 1e-10);
            Assert.AreEqual(1.072665379969842, regression.Coefficients[2], 1e-10);
            Assert.IsFalse(regression.Coefficients.HasNaN());

            Assert.AreEqual(3, regression.StandardErrors.Length);
            Assert.AreEqual(1.6402037052797314, regression.StandardErrors[0], 1e-10);
            Assert.AreEqual(0.026119425452145524, regression.StandardErrors[1], 1e-10);
            Assert.AreEqual(1.1297252500874606, regression.StandardErrors[2], 1e-10);
            Assert.IsFalse(regression.StandardErrors.HasNaN());
        }

    }
}
