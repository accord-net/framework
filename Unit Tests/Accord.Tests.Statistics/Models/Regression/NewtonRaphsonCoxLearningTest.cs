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
    using System;
    using Accord.Math;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Models.Regression;
    using Accord.Statistics.Models.Regression.Fitting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Statistics.Distributions.Fitting;

    [TestClass()]
    public class NewtonRaphsonCoxLearningTest
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
        public void RunTest()
        {
            // Data from: http://www.sph.emory.edu/~cdckms/CoxPH/prophaz2.html

            double[,] data =
            {
                { 50,  1, 0 },
                { 70,  2, 1 },
                { 45,  3, 0 },
                { 35,  5, 0 },
                { 62,  7, 1 },
                { 50, 11, 0 },
                { 45,  4, 0 },
                { 57,  6, 0 },
                { 32,  8, 0 },
                { 57,  9, 1 },
                { 60, 10, 1 },
            };

            ProportionalHazards regression = new ProportionalHazards(1);

            double[][] inputs = data.GetColumn(0).ToArray();
            double[] time = data.GetColumn(1);
            int[] output = data.GetColumn(2).ToInt32();

            ProportionalHazardsNewtonRaphson target = new ProportionalHazardsNewtonRaphson(regression);

            double error = target.Run(inputs, time, output);

            double log = -2 * regression.GetPartialLogLikelihood(inputs, time, output);


            Assert.AreEqual(0.3770, regression.Coefficients[0], 1e-4);
            Assert.IsFalse(Double.IsNaN(regression.Coefficients[0]));

            Assert.AreEqual(0.2542, regression.StandardErrors[0], 1e-4);
            Assert.IsFalse(Double.IsNaN(regression.StandardErrors[0]));


            double[] actual = new double[inputs.Length];
            for (int i = 0; i < actual.Length; i++)
                actual[i] = regression.Compute(inputs[i]);

            double[] expected = 
            {
                // Computed using R's predict(fit,type="risk")
                 0.640442743,  1206.226657448,   0.097217211,  0.002240107,
                59.081223025,     0.640442743,   0.097217211,  8.968345353,
                 0.000722814,     8.968345353,  27.794227993
            };

            for (int i = 0; i < actual.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i], 1e-3);
                Assert.IsFalse(Double.IsNaN(actual[i]));
            }
        }


        [TestMethod()]
        public void RunTest2()
        {
            // Data from: http://www.sph.emory.edu/~cdckms/CoxPH/prophaz2.html

            double[,] data =
            {
                { 50, 30,  1, 0 },
                { 70, 22,  2, 1 },
                { 45, 12,  3, 0 },
                { 35, 22,  5, 0 },
                { 62, 54,  7, 1 },
                { 50, 12, 11, 0 },
                { 45, 11,  4, 0 },
                { 57, 62,  6, 0 },
                { 32, 16,  8, 0 },
                { 57, 14,  9, 1 },
                { 60, 12, 10, 1 },
            };

            ProportionalHazards regression = new ProportionalHazards(2);

            double[][] inputs = data.Submatrix(null, 0, 1).ToArray();
            double[] time = data.GetColumn(2);
            int[] output = data.GetColumn(3).ToInt32();


            ProportionalHazardsNewtonRaphson target = new ProportionalHazardsNewtonRaphson(regression);


            double error = target.Run(inputs, time, output);

            double log = -2 * regression.GetPartialLogLikelihood(inputs, time, output);
            Assert.AreEqual(3.4261, log, 1e-4);
            Assert.IsFalse(Double.IsNaN(log));

            double actual = regression.Coefficients[0];

            Assert.AreEqual(0.3909, regression.Coefficients[0], 1e-4);
            Assert.IsFalse(Double.IsNaN(regression.Coefficients[0]));

            Assert.AreEqual(0.0424, regression.Coefficients[1], 1e-4);
            Assert.IsFalse(Double.IsNaN(regression.Coefficients[1]));

            Assert.AreEqual(0.2536, regression.StandardErrors[0], 1e-4);
            Assert.IsFalse(Double.IsNaN(regression.StandardErrors[0]));

            Assert.AreEqual(0.0624, regression.StandardErrors[1], 1e-4);
            Assert.IsFalse(Double.IsNaN(regression.StandardErrors[1]));
        }


        [TestMethod()]
        public void RunTest3()
        {
            // Data from: http://www.sph.emory.edu/~cdckms/CoxPH/prophaz2.html
            // with added tied times

            double[,] data =
            {
                { 50,  1, 0 },
                { 60,  1, 0 },
                { 40,  1, 0 },
                { 51,  1, 0 },
                { 70,  2, 1 },
                { 45,  3, 0 },
                { 35,  5, 0 },
                { 62,  7, 1 },
                { 50, 11, 0 },
                { 45,  4, 0 },
                { 57,  6, 0 },
                { 32,  8, 0 },
                { 57,  9, 1 },
                { 60, 10, 1 },
            };

            ProportionalHazards regression = new ProportionalHazards(1);

            double[][] inputs = data.GetColumn(0).ToArray();
            double[] time = data.GetColumn(1);
            int[] output = data.GetColumn(2).ToInt32();

            ProportionalHazardsNewtonRaphson target = new ProportionalHazardsNewtonRaphson(regression);

            double error = target.Run(inputs, time, output);

            double log = -2 * regression.GetPartialLogLikelihood(inputs, time, output);


            Assert.AreEqual(0.3770, regression.Coefficients[0], 1e-4);
            Assert.IsFalse(Double.IsNaN(regression.Coefficients[0]));

            Assert.AreEqual(0.2542, regression.StandardErrors[0], 1e-4);
            Assert.IsFalse(Double.IsNaN(regression.StandardErrors[0]));
        }

        [TestMethod()]
        public void RunTest4()
        {
            // Data from: http://www.sph.emory.edu/~cdckms/CoxPH/prophaz2.html
            // with added tied times

            double[,] data =
            {
                { 50,  1, 1 },
                { 60,  1, 1 },
                { 40,  1, 1 },
                { 51,  1, 1 },
                { 70,  2, 1 },
                { 45,  3, 0 },
                { 35,  5, 0 },
                { 62,  7, 1 },
                { 50, 11, 0 },
                { 45,  4, 0 },
                { 57,  6, 0 },
                { 32,  8, 0 },
                { 57,  9, 1 },
                { 60, 10, 1 },
            };

            ProportionalHazards regression = new ProportionalHazards(1);

            double[][] inputs = data.GetColumn(0).ToArray();
            double[] time = data.GetColumn(1);
            int[] output = data.GetColumn(2).ToInt32();

            ProportionalHazardsNewtonRaphson target = new ProportionalHazardsNewtonRaphson(regression);

            double error = target.Run(inputs, time, output);

            double log = -2 * regression.GetPartialLogLikelihood(inputs, time, output);


            Assert.AreEqual(0.04863, regression.Coefficients[0], 1e-4);
            Assert.IsFalse(Double.IsNaN(regression.Coefficients[0]));

            Assert.AreEqual(0.04186, regression.StandardErrors[0], 1e-4);
            Assert.IsFalse(Double.IsNaN(regression.StandardErrors[0]));
        }

        [TestMethod()]
        public void RunTest5()
        {
            double[,] inputs =
            {
                { 1, 1, 1, 1, 1, 0, 1, 1, 0, 0, 1, 0, 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 0, 0, 1, 0, 1, 0, 0, 0, 1, 1, 1, 0, 1, 0, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0, 1, 0, 1, 1, 1, 1, 0, 1, 0, 1, 1, 1, 1, 1, 0, 0, 0, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 0, 0, 1, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 0, 1, 0, 1, 1, 1, 1, 1, 0, 1, 0, 1, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 1, 1, 1, 1, 1, 0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0, 1, 1, 0, 1, 0, 1, 0, 1, 1, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 0, 0, 1, 0, 0, 1, 1, 1, 1, 1, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 0, 1, 1, 1, 1, 1, 0, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 1, 0, 1, 0, 1, 1, 0, 1, 1, 0, 1, 0, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 0, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 0, 1, 1, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 0, 1, 0, 0, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 0, 1, 0, 1, 1, 0, 1, 1, 0, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0, 1, 1, 1, 0, 1, 0, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 1, 0, 1, 1, 1, 1, 0, 0, 1, 1, 0, 1, 1, 1, 0, 1, 0, 1, 1, 1, 0, 1, 1, 0, 0, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 0, 1, 1, 1, 0, 0, 1, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 0, 0, 0, 1, 0, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 1, 1, 1, 1, 1, 0, 1, 0, 1, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 0, 0, 1, 1, 1, 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 0, 1, 0, 0, 1, 1, 1, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1 },
                { 0.9, 1.3, 1.5, 1, 2.4, 1.1, 1.3, 1.1, 0.8, 0.7, 1.1, 0.8, 1, 1, 1.4, 1.4, 1.2, 1.1, 1, 1.4, 2.5, 1.6, 1.2, 0.7, 0.8, 0.7, 0.7, 1.3, 1, 1.7, 1, 1.1, 1.3, 0.9, 1, 1.1, 0.7, 0.9, 1.3, 1.3, 0.9, 1.3, 0.9, 0.6, 1.2, 1.1, 1.1, 0.9, 1.1, 1.1, 1.9, 1.1, 0.9, 1.3, 1.1, 1.2, 0.8, 0.9, 1.7, 1.2, 0.7, 1.2, 1.6, 2.9, 1, 0.9, 0.8, 1, 1.5, 0.8, 0.7, 1.1, 1.2, 0.5, 0.9, 0.9, 0.9, 1.2, 1, 0.9, 1.3, 0.6, 1.2, 0.6, 1.7, 0.8, 1.1, 1, 1.1, 1.2, 0.6, 1.3, 1.7, 1.2, 2.7, 0.8, 1, 1, 1, 1.6, 1.2, 1.2, 2.4, 0.7, 0.8, 0.7, 1.3, 0.9, 1.1, 1.2, 1.2, 1.7, 1, 1.1, 2.1, 1.8, 2.2, 0.5, 1, 1.6, 1.2, 1.5, 1.1, 1, 1, 1.4, 1.2, 1.2, 1.1, 1.5, 0.9, 0.9, 1.2, 1.4, 1.2, 1, 0.9, 1.9, 1.7, 1.3, 1.2, 1.4, 1.7, 0.9, 1.6, 0.7, 0.9, 1, 1.1, 1.4, 1, 1.1, 1.4, 1.3, 1.1, 1.8, 1.6, 1.2, 1, 0.8, 1.1, 0.7, 1.4, 1.1, 1.8, 0.7, 1.7, 1, 1.2, 1.5, 1.1, 1.6, 1.7, 1.3, 0.8, 1.7, 1.5, 0.8, 1.4, 0.9, 4.7, 1, 0.7, 0.9, 0.9, 1, 2.6, 1, 1.1, 1.4, 1, 0.9, 1.3, 1.6, 1.8, 0.9, 1.6, 0.9, 1.2, 1, 1.2, 0.7, 1.4, 1.6, 1.2, 1.1, 1.3, 0.9, 1.3, 0.7, 1.2, 1.1, 1, 1, 0.9, 0.8, 1, 1, 1.5, 1.3, 1.2, 1, 1.2, 1.1, 1.6, 1.7, 1.1, 0.9, 1, 0.9, 1.3, 0.8, 0.8, 1, 0.9, 1, 1.2, 1.4, 0.8, 1, 1, 1.2, 1.3, 1.1, 1, 1.6, 1.2, 0.8, 2.1, 1, 1.5, 1, 1.5, 1.2, 1.1, 1.3, 0.8, 1, 1.1, 1.5, 0.8, 1.2, 1.1, 0.8, 0.9, 1.4, 1.5, 1, 1.3, 1.1, 1, 1.2, 2.2, 1.2, 1.3, 1.3, 0.7, 1, 1.1, 1.1, 1.7, 1.3, 1, 1.1, 1, 1.8, 1.2, 1.2, 1.3, 1.1, 1.6, 4.1, 1, 0.9, 1.5, 1.5, 1.4, 1, 0.5, 0.9, 0.8, 0.6, 1.3, 0.7, 0.7, 1.1, 1.4, 0.9, 1, 4.2, 1, 0.9, 1.2, 2.1, 2.4, 1.2, 2.5, 1, 1.2, 1.3, 0.9, 1.1, 1.6, 1, 1.5, 1.1, 1, 2.6, 0.9, 1.3, 1.4, 0.8, 1.4, 2.7, 1.1, 0.8, 1.2, 1.4, 1.2, 1, 1.1, 1.2, 1.2, 1.3, 1.2, 1.2, 1.2, 1.1, 1.3, 0.9, 1, 1.9, 1.4, 1.2, 1.1, 0.7, 0.9, 1.1, 1.1, 1.4, 1, 1.2, 1.4, 1, 2.8, 1.1, 0.7, 0.7, 1.2, 0.7, 1, 1.5, 1.5, 1.7, 0.9, 1.6, 0.9, 1.7, 0.8, 1, 0.8, 1.1, 1.3, 1.2, 0.9, 1.1, 1.1, 0.9, 0.9, 0.7, 1.1, 1.8, 0.8, 0.9, 1.4, 0.9, 1.2, 1.4, 1, 0.8, 1.4, 1.4, 1, 0.7, 0.6, 0.8, 1.4, 1.4, 0.8, 1.9, 1.3, 1, 1.3, 0.9, 0.9, 1.5, 1.5, 0.8, 1.4, 1.1, 0.8, 0.8, 1.4, 1.3, 1.1, 1, 2.2, 1.6, 1.7, 1.2, 0.9, 1.1, 1, 2.6, 1.6, 0.9, 0.9, 0.6, 1.6, 1.1, 1.3, 1.7, 1.2, 1.2, 1.6, 0.9, 0.9, 1.3, 2.2, 1.3, 1, 0.8, 1.5, 1.3, 1, 1.2, 1.4, 1.3, 1.3, 1.4, 1.3, 1.4, 1.2, 1, 1.2, 0.8, 1.1, 1.2, 1.1, 1.2, 1.8, 1.1, 1.6, 1.1, 0.8, 1.2, 1, 1.2, 0.7, 1.9, 1.6, 1.5, 0.9, 1.1, 0.8, 1.3, 1.1, 0.7, 1, 2, 1.2, 1.1, 1.2, 0.9, 0.9, 1, 1.2, 1, 1.2, 1.1, 1.1, 1, 1.4, 0.8, 1.3, 0.8, 1, 1.4, 1.2, 1.1, 1.3, 0.7, 1.2, 0.6, 1.2, 1.4, 1.5, 1.7, 0.9, 1, 0.6, 0.9, 1.4, 1.5, 1.1, 1.3, 0.9, 1.3, 2.1, 1.2, 1.3, 1.1, 1.4, 0.9, 1.4, 1.5, 1, 1.5, 1, 1.3, 0.9, 1.2, 1.1, 0.9, 1.1, 1.4, 1.4, 0.8, 1, 1.2, 1.1, 0.8, 1.2, 4.1, 0.7, 2, 0.8, 1.2, 3.8, 1.2, 1.4, 1, 1, 1.7, 1, 1, 0.9, 1.6, 0.7, 1.2, 1.1, 1, 1, 1, 0.7, 1.5, 1.1, 1, 0.9, 0.8, 0.9, 1.9, 1, 1.4, 1.3, 1.5, 0.9, 1.1, 1.2, 0.8, 1, 1.4, 4.2, 2.5, 1.2, 1.6, 0.9, 1.1, 1, 0.9, 0.6, 1, 1.1, 1.2, 1.2, 1.2, 0.9, 1.2, 1.1, 1.2, 0.8, 1, 1.6, 0.9, 1.2, 1.4, 0.7, 1.6, 1.1, 1.5, 0.8, 1.4, 0.9, 0.9, 1.5, 1.1, 9.1, 1.6, 1.3, 0.8, 4, 0.7, 1.4, 0.8, 3.6, 1.3, 1.3, 1, 1.1, 0.8, 1.2, 0.8, 0.9, 1.5, 1, 0.7, 6.6, 1.2, 1.8, 1, 1.3, 1, 1.3, 1.1, 1.1, 1, 0.9, 0.9, 1.1, 1.1, 0.9, 1.5, 4.5, 1.1, 0.8, 0.7, 0.9, 0.8, 1.6, 0.8, 0.8, 2, 2, 1.5, 1.2, 1.2, 0.9, 1.7, 1.4, 0.8, 1.5, 1, 1.1, 1.1, 0.7, 1.1, 1.2, 0.9, 1.5, 0.7, 1.5, 1.3, 0.8, 0.7, 1.1, 1, 1, 1.2, 0.8, 0.9, 0.8, 1.6, 2.4, 1.1, 2, 0.9, 1.4, 1.2, 1.1, 1.6, 0.9, 0.9, 1.3, 2, 1, 1.4, 1.3, 1.3, 0.7, 0.9, 1.1, 1.7, 1.1, 1.2, 0.9, 0.9, 1.1, 0.9, 5.2, 1.3, 0.7, 1.4, 1.4, 0.8, 1.2, 1.4, 0.9, 1.1, 1.1, 1.3, 1.7, 0.8, 1, 1.2, 1.9, 1.1, 1.3, 1.8, 0.6, 0.8, 1.4, 0.7, 0.9, 0.9, 1.2, 1.5, 0.7, 4.2, 1.1, 1.1, 1, 1.3, 0.8, 1.3, 1.1, 1.1, 0.8, 1.1, 1.5, 1.2, 1.2, 1.1, 1.7, 1, 1, 0.9, 0.9, 0.8, 1.2, 1.1, 0.7, 0.8, 1.1, 0.9, 1.4, 1, 1.1, 1.4, 0.9, 1, 1.7, 0.9, 1.3, 1.3, 0.8, 2.1, 1, 0.9, 1.2, 0.9, 1.1, 1.1, 1.2, 1.2, 0.9, 1.4, 1.2, 0.8, 1.1, 1.3, 1.1, 1.1, 0.8, 0.9, 0.9, 1.2, 1.1, 1.7, 1.3, 1.1, 1.7, 0.8, 0.9, 1.5, 1.1, 1.4, 1.4, 1.5, 1.1, 1.3, 0.9, 1.1, 1.1, 0.7, 1, 1.1, 0.6, 1, 1.2, 1.4, 1.1, 0.8, 1, 1.3, 0.9, 1, 1, 0.9, 1.5, 1.1, 1.5, 6.3, 1.4, 1.1, 5.2, 1.6, 1, 1.2, 1.3, 0.6, 1.1, 1.2, 1.1, 1.2, 1, 1.1, 1.9, 1.1, 1.2, 1.1, 0.7, 1.4, 2.2, 1.1, 1.5, 0.9, 1.2, 1.1, 0.8, 1.3, 1.1, 1.3, 1.8, 1.1, 1.1, 1.1, 1, 0.8, 1.7, 1.2, 1.4, 1.1, 1.4, 1.1, 0.8, 1, 1.1, 1.2, 1.4, 1, 1.3, 1.1, 2.3, 0.7, 1.3, 0.7, 0.9, 0.9, 1.2, 2, 0.7, 1.2, 1.6, 1.3, 1.4, 2.7, 1.5, 1, 1, 1.5, 1 }
            };

            double[,] outputs = 
            {
                { 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 9, 30, 30, 30, 30, 30, 2, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 2, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 13, 30, 30, 30, 30, 30, 30, 0, 30, 13, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 1, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 16, 30, 30, 30, 30, 30, 30, 0, 30, 30, 30, 30, 30, 30, 30, 30, 5, 30, 30, 30, 30, 30, 30, 30, 30, 1, 30, 21, 30, 30, 30, 30, 30, 30, 30, 30, 1, 30, 30, 30, 16, 30, 30, 30, 30, 30, 30, 30, 30, 2, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 16, 30, 30, 30, 30, 30, 3, 30, 18, 30, 30, 30, 30, 30, 28, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 0, 30, 30, 30, 30, 30, 1, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 17, 30, 1, 30, 30, 1, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 9, 30, 30, 30, 30, 1, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 9, 30, 30, 30, 27, 30, 30, 30, 30, 30, 30, 30, 30, 30, 0, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 16, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 20, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 1, 30, 30, 4, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 26, 30, 30, 30, 30, 12, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 21, 30, 1, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 9, 30, 30, 30, 30, 30, 30, 30, 1, 30, 1, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 0, 30, 25, 30, 30, 12, 30, 30, 30, 30, 30, 30, 10, 30, 30, 30, 30, 3, 30, 11, 30, 30, 30, 30, 30, 11, 30, 30, 30, 30, 30, 30, 4, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 1, 30, 30, 30, 30, 30, 1, 0, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 11, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 2, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 1, 30, 10, 30, 30, 30, 0, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 3, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 19, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 29, 30, 30, 30, 30, 30, 30, 2, 30, 30, 30, 30, 15, 30, 30, 30, 30, 30, 30, 30, 30, 3, 30, 30, 0, 30, 30, 30, 2, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 9, 30, 30, 30, 2, 30, 30, 30, 2, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 8, 30, 30, 30, 30, 30, 30, 0, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 14, 30, 30, 30, 9, 30, 30, 30, 30, 30, 13, 30, 30, 30, 4, 30, 30, 30, 30, 1, 30, 30, 30, 30, 30, 30, 10, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 1, 30, 30, 2, 30, 30, 30, 30, 30 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0 }
            };


            double[][] covariates = inputs.Transpose().ToArray();
            double[] time = outputs.GetRow(0);
            int[] censor = outputs.GetRow(1).ToInt32();


            string inputStr = inputs.Transpose().ToString(Accord.Math.DefaultMatrixFormatProvider.InvariantCulture);
            string outputStr = outputs.Transpose().ToString(Accord.Math.DefaultMatrixFormatProvider.InvariantCulture);

            ProportionalHazards regression = new ProportionalHazards(2);
            ProportionalHazardsNewtonRaphson target = new ProportionalHazardsNewtonRaphson(regression);

            double error = target.Run(covariates, time, censor);

            double log = -2 * regression.GetPartialLogLikelihood(covariates, time, censor);

            Assert.AreEqual(-0.270, regression.Coefficients[0], 1e-4);
            Assert.AreEqual(0.463, regression.Coefficients[1], 1e-2);
            Assert.IsFalse(Double.IsNaN(regression.Coefficients[0]));
            Assert.IsFalse(Double.IsNaN(regression.Coefficients[1]));

            Assert.AreEqual(0.2454, regression.StandardErrors[0], 1e-4);
            Assert.AreEqual(0.0671, regression.StandardErrors[1], 1e-4);
            Assert.IsFalse(Double.IsNaN(regression.StandardErrors[0]));
            Assert.IsFalse(Double.IsNaN(regression.StandardErrors[1]));
        }


        [TestMethod()]
        public void PredictTest1()
        {
            // Data from: http://www.sph.emory.edu/~cdckms/CoxPH/prophaz2.html

            double[,] data =
            {
                { 50,  1, 0 },
                { 70,  2, 1 },
                { 45,  3, 0 },
                { 35,  5, 0 },
                { 62,  7, 1 },
                { 50, 11, 0 },
                { 45,  4, 0 },
                { 57,  6, 0 },
                { 32,  8, 0 },
                { 57,  9, 1 },
                { 60, 10, 1 },
            };

            ProportionalHazards regression = new ProportionalHazards(1);

            double[][] inputs = data.GetColumn(0).ToArray();
            double[] time = data.GetColumn(1);
            int[] output = data.GetColumn(2).ToInt32();


            ProportionalHazardsNewtonRaphson target = new ProportionalHazardsNewtonRaphson(regression);

            double error = target.Run(inputs, time, output);


            double[] expected = 
            {
                0.000000000000, 0.919466527073, 0.000074105451, 0.000001707560,
                0.657371730925, 0.046771996036, 0.000074105451, 0.006836271860,
                0.000008042445, 0.339562971888, 2.029832541310 
            };

            double[] actual = new double[inputs.Length];
            for (int i = 0; i < inputs.Length; i++)
                actual[i] = regression.Compute(inputs[i], time[i]);

            for (int i = 0; i < actual.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i], 1e-6);
                Assert.IsFalse(Double.IsNaN(actual[i]));
            }
        }

        [TestMethod()]
        public void BaselineHazardTest()
        {
            double[,] data = 
            {
               // t   c  in
                { 8,  0, 13 },
                { 4,  1, 56 },
                { 12, 0, 25 },
                { 6,  0, 64 },
                { 10, 0, 38 },
                { 8,  1, 80 },
                { 5,  0, 0 },
                { 5,  0, 81 },
                { 3,  1, 81 },
                { 14, 1, 38 },
                { 8,  0, 23 },
                { 11, 0, 99 },
                { 7,  0, 12 },
                { 7,  1, 36 },
                { 12, 0, 63 },
                { 8,  0, 92 },
                { 7,  0, 38 },
            };

            double[] time = data.GetColumn(0);
            int[] censor = data.GetColumn(1).ToInt32();
            double[][] inputs = data.GetColumn(2).ToArray();

            ProportionalHazards regression = new ProportionalHazards(1);

            ProportionalHazardsNewtonRaphson target = new ProportionalHazardsNewtonRaphson(regression);
            target.Normalize = false;

            double error = target.Run(inputs, time, censor);
            double log = -2 * regression.GetPartialLogLikelihood(inputs, time, censor);

            EmpiricalHazardDistribution baseline = regression.BaselineHazard as EmpiricalHazardDistribution;
         
            double[] actual = new double[(int)baseline.Support.Max];
            for (int i = (int)baseline.Support.Min; i < baseline.Support.Max; i++)
                actual[i] = baseline.CumulativeHazardFunction(i);

            Assert.AreEqual(14, actual.Length);

            double[] expected = 
            {
                0,0,0,
                0.025000345517572315,0.052363663484639708,0.052363663484639708,0.052363663484639708,
                0.16317880290786446,
                0.34217461190678861,0.34217461190678861,0.34217461190678861,
                0.34217461190678861,0.34217461190678861,0.34217461190678861
            };

            for (int i = 0; i < actual.Length; i++)
                Assert.AreEqual(expected[i], actual[i], 0.025);
        }
    }
}
