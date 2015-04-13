// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
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
    public class MultinomialLogisticRegressionAnalysisTest
    {


        [TestMethod]
        public void ComputeTest1()
        {
            double[][] inputs;
            int[] outputs;

            MultinomialLogisticRegressionTest.CreateInputOutputsExample1(out inputs, out outputs);

            var analysis = new MultinomialLogisticRegressionAnalysis(inputs, outputs);

            int inputCount = 2;
            int outputCount = 3;
            int coeffCount = inputCount + 1;

            var mlr = analysis.regression;
            Assert.AreEqual(inputCount, mlr.Inputs);
            Assert.AreEqual(outputCount, mlr.Categories);

            Assert.AreEqual(inputCount, analysis.Inputs.Length);
            Assert.AreEqual(outputCount, analysis.OutputNames.Length);

            analysis.Iterations = 100;
            analysis.Tolerance = 1e-6;

            analysis.Compute();

            Assert.AreEqual(outputCount - 1, analysis.CoefficientValues.Length);
            Assert.AreEqual(outputCount - 1, analysis.StandardErrors.Length);

            Assert.AreEqual(outputCount - 1, analysis.WaldTests.Length);
            Assert.AreEqual(outputCount - 1, analysis.Confidences.Length);

            for (int i = 0; i < analysis.CoefficientValues.Length; i++)
            {
                Assert.AreEqual(coeffCount, analysis.CoefficientValues[i].Length);
                Assert.AreEqual(coeffCount, analysis.StandardErrors[i].Length);

                Assert.AreEqual(coeffCount, analysis.WaldTests[i].Length);
                Assert.AreEqual(coeffCount, analysis.Confidences[i].Length);

                for (int j = 0; j < analysis.CoefficientValues[i].Length; j++)
                {
                    Assert.IsFalse(double.IsNaN(analysis.CoefficientValues[i][j]));
                    Assert.IsFalse(double.IsNaN(analysis.StandardErrors[i][j]));
                }
            }

            var coefficients = analysis.CoefficientValues;

            // brand 2
            Assert.AreEqual(-11.774655, coefficients[0][0], 1e-4); // intercept
            Assert.AreEqual(0.523814, coefficients[0][1], 1e-4); // female
            Assert.AreEqual(0.368206, coefficients[0][2], 1e-4); // age

            // brand 3
            Assert.AreEqual(-22.721396, coefficients[1][0], 1e-4); // intercept
            Assert.AreEqual(0.465941, coefficients[1][1], 1e-4); // female
            Assert.AreEqual(0.685908, coefficients[1][2], 1e-4); // age

            var standard = analysis.StandardErrors;

            // Using the lower-bound approximation
            Assert.AreEqual(1.047378039787443, standard[0][0], 1e-6);
            Assert.AreEqual(0.153150051082552, standard[0][1], 1e-6);
            Assert.AreEqual(0.031640507386863, standard[0][2], 1e-6);

            Assert.AreEqual(1.047378039787443, standard[1][0], 1e-6);
            Assert.AreEqual(0.153150051082552, standard[1][1], 1e-6);
            Assert.AreEqual(0.031640507386863, standard[1][2], 1e-6);

            Assert.AreEqual(-702.97, analysis.LogLikelihood, 1e-2);
            Assert.AreEqual(185.85, analysis.ChiSquare.Statistic, 1e-2);
            Assert.AreEqual(1405.9414080469473, analysis.Deviance, 1e-10);

            var wald = analysis.WaldTests;
            Assert.AreEqual(-11.241995503283842, wald[0][0].Statistic, 1e-4);
            Assert.AreEqual(3.4202662152119889, wald[0][1].Statistic, 1e-4);
            Assert.AreEqual(11.637150673342207, wald[0][2].Statistic, 1e-4);

            Assert.AreEqual(-21.693553825772664, wald[1][0].Statistic, 1e-4);
            Assert.AreEqual(3.0423802097069097, wald[1][1].Statistic, 1e-4);
            Assert.AreEqual(21.678124991086548, wald[1][2].Statistic, 1e-4);
        }

        [TestMethod]
        public void ComputeTest2()
        {
            double[][] inputs;
            int[] outputs;

            MultinomialLogisticRegressionTest.CreateInputOutputsExample2(out inputs, out outputs);

            var analysis = new MultinomialLogisticRegressionAnalysis(inputs, outputs);

            int inputCount = 5;
            int outputCount = 3;
            int coeffCount = inputCount + 1;

            var mlr = analysis.regression;
            Assert.AreEqual(inputCount, mlr.Inputs);
            Assert.AreEqual(outputCount, mlr.Categories);

            Assert.AreEqual(inputCount, analysis.Inputs.Length);
            Assert.AreEqual(outputCount, analysis.OutputNames.Length);

            analysis.Iterations = 100;
            analysis.Tolerance = 1e-6;

            analysis.Compute();

            Assert.AreEqual(outputCount - 1, analysis.CoefficientValues.Length);
            Assert.AreEqual(outputCount - 1, analysis.StandardErrors.Length);

            Assert.AreEqual(outputCount - 1, analysis.WaldTests.Length);
            Assert.AreEqual(outputCount - 1, analysis.Confidences.Length);

            for (int i = 0; i < analysis.CoefficientValues.Length; i++)
            {
                Assert.AreEqual(coeffCount, analysis.CoefficientValues[i].Length);
                Assert.AreEqual(coeffCount, analysis.StandardErrors[i].Length);

                Assert.AreEqual(coeffCount, analysis.WaldTests[i].Length);
                Assert.AreEqual(coeffCount, analysis.Confidences[i].Length);

                for (int j = 0; j < analysis.CoefficientValues[i].Length; j++)
                {
                    Assert.IsFalse(double.IsNaN(analysis.CoefficientValues[i][j]));
                    Assert.IsFalse(double.IsNaN(analysis.StandardErrors[i][j]));
                }
            }

            var coefficients = analysis.CoefficientValues;

            // brand 2
            Assert.AreEqual(-11.774655, coefficients[0][0], 1e-3); // intercept
            Assert.AreEqual(0.523814, coefficients[0][1], 1e-3); // female
            Assert.AreEqual(0.368206, coefficients[0][2], 1e-3); // age

            // brand 3
            Assert.AreEqual(-22.721396, coefficients[1][0], 1e-3); // intercept
            Assert.AreEqual(0.465941, coefficients[1][1], 1e-3); // female
            Assert.AreEqual(0.685908, coefficients[1][2], 1e-3); // age

            var standard = analysis.StandardErrors;

            Assert.AreEqual(-702.97, analysis.LogLikelihood, 1e-2);
            Assert.AreEqual(185.85, analysis.ChiSquare.Statistic, 1e-2);
            Assert.AreEqual(1405.9414080469473, analysis.Deviance, 1e-5);
        }

    }
}
