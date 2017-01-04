// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
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
    using NUnit.Framework;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics;

    [TestFixture]
    public class NewtonRaphsonCoxLearningTest
    {

        [Test]
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

            var regression = new ProportionalHazards(1);

            double[][] inputs = data.GetColumn(0).ToJagged();
            double[] time = data.GetColumn(1);
            SurvivalOutcome[] output = data.GetColumn(2).To<SurvivalOutcome[]>();

            var target = new ProportionalHazardsNewtonRaphson(regression);

            double error = target.Run(inputs, time, output);

            double log = -2 * regression.GetPartialLogLikelihood(inputs, time, output);

            // Tested against http://www.sph.emory.edu/~cdckms/CoxPH/prophaz2.html
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


        [Test]
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

            var regression = new ProportionalHazards(2);

            double[][] inputs = data.Submatrix(null, 0, 1).ToJagged();
            double[] time = data.GetColumn(2);
            SurvivalOutcome[] output = data.GetColumn(3).To<SurvivalOutcome[]>();


            var target = new ProportionalHazardsNewtonRaphson(regression);


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


        [Test]
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

            var regression = new ProportionalHazards(1);

            double[][] inputs = data.GetColumn(0).ToJagged();
            double[] time = data.GetColumn(1);
            SurvivalOutcome[] output = data.GetColumn(2).To<SurvivalOutcome[]>();

            var target = new ProportionalHazardsNewtonRaphson(regression);

            double error = target.Run(inputs, time, output);

            double log = -2 * regression.GetPartialLogLikelihood(inputs, time, output);


            Assert.AreEqual(0.3770, regression.Coefficients[0], 1e-4);
            Assert.IsFalse(Double.IsNaN(regression.Coefficients[0]));

            Assert.AreEqual(0.2542, regression.StandardErrors[0], 1e-4);
            Assert.IsFalse(Double.IsNaN(regression.StandardErrors[0]));
        }

        [Test]
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

            var regression = new ProportionalHazards(1);

            double[][] inputs = data.GetColumn(0).ToJagged();
            double[] time = data.GetColumn(1);
            int[] output = data.GetColumn(2).ToInt32();

            var target = new ProportionalHazardsNewtonRaphson(regression);

            double error = target.Run(inputs, time, output);

            double log = -2 * regression.GetPartialLogLikelihood(inputs, time, output);


            Assert.AreEqual(0.04863, regression.Coefficients[0], 1e-4);
            Assert.IsFalse(Double.IsNaN(regression.Coefficients[0]));

            Assert.AreEqual(0.04186, regression.StandardErrors[0], 1e-4);
            Assert.IsFalse(Double.IsNaN(regression.StandardErrors[0]));
        }

        [Test]
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


            double[][] covariates = inputs.Transpose().ToJagged();
            double[] time = outputs.GetRow(0);
            int[] censor = outputs.GetRow(1).ToInt32();


            // string inputStr = inputs.Transpose().ToString(Accord.Math.DefaultMatrixFormatProvider.InvariantCulture);
            // string outputStr = outputs.Transpose().ToString(Accord.Math.DefaultMatrixFormatProvider.InvariantCulture);

            var regression = new ProportionalHazards(2);
            var target = new ProportionalHazardsNewtonRaphson(regression);

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


        [Test]
        public void PredictTest1()
        {
            // Data from: http://statpages.org/prophaz2.html

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

            var regression = new ProportionalHazards(1);

            double[][] inputs = data.GetColumn(0).ToJagged();
            double[] time = data.GetColumn(1);
            int[] censor = data.GetColumn(2).ToInt32();


            var target = new ProportionalHazardsNewtonRaphson(regression);

            double error = target.Run(inputs, time, censor);

            // Tested against http://statpages.org/prophaz2.html
            Assert.AreEqual(0.3770, regression.Coefficients[0], 1e-4);
            Assert.AreEqual(0.2542, regression.StandardErrors[0], 1e-4);
            Assert.AreEqual(51.18181818181818, regression.Offsets[0]);

            double mean = regression.Offsets[0];

            // Baseline survivor function at predictor means
            double[] baseline =
            {
                regression.Survival(2),
                regression.Survival(7),
                regression.Survival(9),
                regression.Survival(10),
            };

            // Tested against http://statpages.org/prophaz2.html
            Assert.AreEqual(0.9979, baseline[0], 1e-4);
            Assert.AreEqual(0.9820, baseline[1], 1e-4);
            Assert.AreEqual(0.9525, baseline[2], 1e-4);
            Assert.AreEqual(0.8310, baseline[3], 1e-4);

            double[] expected = 
            { 
                0, 2.51908236823927, 0.000203028311170645, 4.67823782106946E-06, 1.07100164957025,
                0.118590728553659, 0.000203028311170645, 0.0187294821517496, 1.31028937819308E-05,
                0.436716853556834, 5.14665484304978
            };

            double[] actual = new double[inputs.Length];
            for (int i = 0; i < inputs.Length; i++)
            {
                double a = actual[i] = regression.Compute(inputs[i], time[i]);
                double e = expected[i];

                Assert.AreEqual(e, a, 1e-3);
            }
            // string exStr = actual.ToString(CSharpArrayFormatProvider.InvariantCulture);
        }


        [Test]
        public void BaselineHazardTestR()
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
            SurvivalOutcome[] censor = data.GetColumn(1).To<SurvivalOutcome[]>();
            double[][] inputs = data.GetColumn(2).ToJagged();

            var regression = new ProportionalHazards(1);

            var target = new ProportionalHazardsNewtonRaphson(regression);

            double error = target.Run(inputs, time, censor);
            // Assert.AreEqual(-10.257417973830666, error, 1e-8);

            /* 
             library('survival')
             options(digits=17)
             time <- c(8, 4, 12, 6, 10, 8, 5, 5, 3, 14, 8, 11, 7, 7, 12, 8, 7)
             x <- c(13, 56, 25, 64, 38, 80, 0, 81, 81, 38, 23, 99, 12, 36, 63, 92, 38)
             c <- c(0, 1, 0, 0, 0, 1, 0, 0, 1, 1, 0, 0, 0, 1, 0, 0, 0)
             
             fit <- coxph(Surv(time, c) ~ x, ties="breslow")
              
             predict(fit,type="risk")
              
             fit$loglik
              
                    coef           exp(coef)          se(coef)               z              p
            x 0.01633097532122  1.016465054586   0.01711960930183    0.9539338797573   0.340117112635

            Likelihood ratio test=0.94  on 1 df, p=0.332836850925  n= 17, number of events= 5  
             */

            // Tested against GNU R
            Assert.AreEqual(49.352941176470587, regression.Offsets[0]);
            Assert.AreEqual(0.01633097532122, regression.Coefficients[0], 1e-10);
            Assert.AreEqual(0.01711960930183, regression.StandardErrors[0], 1e-10);
            Assert.AreEqual(0.340117112635, regression.GetWaldTest(0).PValue, 1e-5);
            Assert.AreEqual(-10.2879332934202168, regression.GetPartialLogLikelihood(time, censor));
            Assert.AreEqual(-9.8190189050165948, regression.GetPartialLogLikelihood(inputs, time, censor));

            double[] actual = inputs.Apply(x => regression.Compute(x));

            /*
             predict(r,type="risk")
                [1] 0.55229166964915244 1.11466393245000361 0.67185866444081555 1.27023351821156782 0.83076808526813917 1.64953983529334769 0.44664925161695829 1.67669959872327912
                [9] 1.67669959872327912 0.83076808526813917 0.65026895029003673 2.24967304521214029 0.54334545703992021 0.80407192663266613 1.24965783376477391 2.00665280971219540
                [17] 0.83076808526813917    
            */

            double[] expected = 
            {
                0.55229166964915244, 1.11466393245000361, 0.67185866444081555, 1.27023351821156782,
                0.83076808526813917, 1.64953983529334769, 0.44664925161695829, 1.67669959872327912,
                1.67669959872327912, 0.83076808526813917, 0.65026895029003673, 2.24967304521214029,
                0.54334545703992021, 0.80407192663266613, 1.24965783376477391, 2.00665280971219540,
                0.83076808526813917
            };

            for (int i = 0; i < actual.Length; i++)
                Assert.AreEqual(expected[i], actual[i], 0.025);

        }

        [Test]
        public void BaselineHazardTest()
        {
            double[,] data = 
            {
               // t   c  in
                { 8,  0, -1.2372626521865966    },
                { 4,  1,  0.22623087329625477   },
                { 12, 0, -0.8288458543774289    },
                { 6,  0,  0.49850873850236665   },
                { 10, 0, -0.38639432341749696   },
                { 8,  1,  1.0430644689145904    },
                { 5,  0, -1.6797141831465285    },
                { 5,  0,  1.0770992020653544    },
                { 3,  1,  1.0770992020653544    },
                { 14, 1, -0.38639432341749696   },
                { 8,  0, -0.8969153206789568    },
                { 11, 0,  1.6897243987791061    },
                { 7,  0, -1.2712973853373605    },
                { 7,  0, -0.38639432341749696   },
                { 7,  1, -0.45446378971902495   },
                { 12, 0,  0.4644740053516027    },
                { 8,  0,  1.4514812667237584    },
            };

            double[] time = data.GetColumn(0);
            SurvivalOutcome[] censor = data.GetColumn(1).To<SurvivalOutcome[]>();
            double[][] inputs = data.GetColumn(2).ToJagged();

            var regression = new ProportionalHazards(1);

            var target = new ProportionalHazardsNewtonRaphson(regression);

            target.Normalize = false;
            target.Lambda = 0;
            regression.Coefficients[0] = 0.47983261821350764;

            double error = target.Run(inputs, time, censor);

            /* Tested against http://statpages.org/prophaz2.html
                13, 8,  0
                56, 4,  1
                25, 12, 0
                64, 6,  0
                38, 10, 0
                80, 8,  1
                0 , 5,  0
                81, 5,  0
                81, 3,  1
                38, 14, 1
                23, 8,  0
                99, 11, 0
                12, 7,  0
                38, 7,  0
                36, 7,  1
                63, 12, 0
                92, 8,  0
             */

            double[] baseline =
            {
                regression.Survival(3),  // 0.9465           
                regression.Survival(4),  // 0.8919
                regression.Survival(7),  // 0.8231
                regression.Survival(8),  // 0.7436
                regression.Survival(12), // 0.7436
                regression.Survival(14), // 0.0000
            };

            Assert.AreEqual(0.9465, baseline[0], 1e-4);
            Assert.AreEqual(0.8919, baseline[1], 1e-4);
            Assert.AreEqual(0.8231, baseline[2], 1e-4);
            Assert.AreEqual(0.7436, baseline[3], 1e-4);
            Assert.AreEqual(0.7436, baseline[4], 1e-4);
            Assert.AreEqual(0.0000, baseline[5], 1e-4);

            // The value of the baseline must be exact the same if it was computed
            // after the Newton-Raphson or in a standalone EmpiricalHazard computation
            double[] outputs = inputs.Apply(x => regression.Compute(x));
            var empirical = EmpiricalHazardDistribution.Estimate(time, censor, outputs);

            baseline = new[]
            {
                empirical.ComplementaryDistributionFunction(3),  // 0.9465           
                empirical.ComplementaryDistributionFunction(4),  // 0.8919
                empirical.ComplementaryDistributionFunction(7),  // 0.8231
                empirical.ComplementaryDistributionFunction(8),  // 0.7436
                empirical.ComplementaryDistributionFunction(12), // 0.7436
                empirical.ComplementaryDistributionFunction(14), // 0.0000
            };

            Assert.AreEqual(0.9465, baseline[0], 1e-4);
            Assert.AreEqual(0.8919, baseline[1], 1e-4);
            Assert.AreEqual(0.8231, baseline[2], 1e-4);
            Assert.AreEqual(0.7436, baseline[3], 1e-4);
            Assert.AreEqual(0.7436, baseline[4], 1e-4);
            Assert.AreEqual(0.0000, baseline[5], 1e-4);
        }


        [Test]
        public void KaplanMeierTest2()
        {
            int[][] data = Classes.Expand(
                new[] { 1, 2, 3, 4, 5, 6 },     // years
                new[] { 3, 3, 3, 3, 3, 0 },     // died
                new[] { 5, 10, 15, 20, 25, 10 } // censored
            );

            double[] time = data.GetColumn(0).ToDouble();
            int[] censor = data.GetColumn(1);

            ProportionalHazards kp;
            ProportionalHazards km;

            double errkm, errp;

            {
                km = new ProportionalHazards(inputs: 0);

                var target = new ProportionalHazardsNewtonRaphson(km)
                {
                    Estimator = HazardEstimator.KaplanMeier
                };

                errkm = target.Run(time, censor);
                Assert.AreEqual(-63.734599918211551, errkm);
            }

            {
                kp = new ProportionalHazards(inputs: 0);

                var target = new ProportionalHazardsNewtonRaphson(kp)
                {
                    Estimator = HazardEstimator.BreslowNelsonAalen
                };

                errp = target.Run(time, censor);
                Assert.AreEqual(errkm, errp);
            }
        }

        [Test]
        public void KaplanMeierTest()
        {
            double[,] data =
            {
                //  time censor
                {    1,    0   }, // died at time 1
                {    2,    1   }, // lost at time 2
                {    3,    0   }, // died at time 3
                {    5,    0   }, // died at time 5
                {    7,    1   }, // lost at time 7
                {   11,    0   }, // ...
                {    4,    0   },
                {    6,    0   },
                {    8,    0   },
                {    9,    1   },
                {    10,   1   },
            };

            double[] time = data.GetColumn(0);
            int[] censor = data.GetColumn(1).ToInt32();

            var regression = new ProportionalHazards(inputs: 0);

            var target = new ProportionalHazardsNewtonRaphson(regression)
            {
                Estimator = HazardEstimator.KaplanMeier
            };

            double error = target.Run(time, censor);

            Assert.AreEqual(-5.7037824746562009, error);
        }
    }
}
