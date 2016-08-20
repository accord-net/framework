// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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
    using Accord.IO;
    using Accord.Math;
    using Accord.Statistics.Analysis;
    using Accord.Statistics.Filters;
    using NUnit.Framework;

    [TestFixture]
    public class MultinomialLogisticRegressionAnalysisTest
    {


        [Test]
        public void AnalyzeExample1()
        {
            // http://www.ats.ucla.edu/stat/stata/dae/mlogit.htm
            CsvReader reader = CsvReader.FromText(Properties.Resources.hsbdemo, hasHeaders: true);

            var table = reader.ToTable();

            var codification = new Codification(table);
            codification["ses"].VariableType = CodificationVariable.CategoricalWithBaseline;
            codification["prog"].VariableType = CodificationVariable.Categorical;
            codification["prog"].Remap("academic", 0);

            var inputs = codification.Apply(table, "ses", "write");
            var output = codification.Apply(table, "prog");


            // Get inputs
            string[] inputNames;
            var inputsData = inputs.ToArray(out inputNames);

            // Get outputs
            string[] outputNames;
            var outputData = output.ToArray(out outputNames);


            var analysis = new MultinomialLogisticRegressionAnalysis(inputsData, outputData, inputNames, outputNames);

            analysis.Compute();

            Assert.AreEqual(9, analysis.Coefficients.Count);

            int i = 0;

            Assert.AreEqual("(baseline)", analysis.Coefficients[i].Name);
            Assert.AreEqual("prog: academic", analysis.Coefficients[i].Class);
            Assert.AreEqual(0, analysis.Coefficients[i].Value);

            i++;
            Assert.AreEqual("Intercept", analysis.Coefficients[i].Name);
            Assert.AreEqual("prog: general", analysis.Coefficients[i].Class);
            Assert.AreEqual(1.0302662690579185, analysis.Coefficients[i].Value, 1e-10);

            i++;
            Assert.AreEqual("write", analysis.Coefficients[i].Name);
            Assert.AreEqual("prog: general", analysis.Coefficients[i].Class);
            Assert.AreEqual(-0.083689163424126883, analysis.Coefficients[i].Value, 1e-10);

            i++;
            Assert.AreEqual("ses: middle", analysis.Coefficients[i].Name);
            Assert.AreEqual("prog: general", analysis.Coefficients[i].Class);
            Assert.AreEqual(-0.58217998138556049, analysis.Coefficients[i].Value, 1e-10);

            i++;
            Assert.AreEqual("ses: high", analysis.Coefficients[i].Name);
            Assert.AreEqual("prog: general", analysis.Coefficients[i].Class);
            Assert.AreEqual(-1.1112048569892283, analysis.Coefficients[i].Value, 1e-10);

            i++;
            Assert.AreEqual("Intercept", analysis.Coefficients[i].Name);
            Assert.AreEqual("prog: vocation", analysis.Coefficients[i].Class);
            Assert.AreEqual(1.2715455854613191, analysis.Coefficients[i].Value, 1e-10);

            i++;
            Assert.AreEqual("write", analysis.Coefficients[i].Name);
            Assert.AreEqual("prog: vocation", analysis.Coefficients[i].Class);
            Assert.AreEqual(-0.13231057837059781, analysis.Coefficients[i].Value, 1e-10);

            i++;
            Assert.AreEqual("ses: middle", analysis.Coefficients[i].Name);
            Assert.AreEqual("prog: vocation", analysis.Coefficients[i].Class);
            Assert.AreEqual(0.20451187629162043, analysis.Coefficients[i].Value, 1e-10);

            i++;
            Assert.AreEqual("ses: high", analysis.Coefficients[i].Name);
            Assert.AreEqual("prog: vocation", analysis.Coefficients[i].Class);
            Assert.AreEqual(-0.93207938490449849, analysis.Coefficients[i].Value, 1e-10);
        }


        [Test]
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

        [Test]
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

        [Test]
        public void CoefficientsTest1()
        {
            double[][] inputs;
            int[] outputs;

            MultinomialLogisticRegressionTest.CreateInputOutputsExample2(out inputs, out outputs);

            var analysis = new MultinomialLogisticRegressionAnalysis(inputs, outputs);

            int inputCount = 5;
            int coeffCount = inputCount + 1;

            var mlr = analysis.regression;

            analysis.Iterations = 100;
            analysis.Tolerance = 1e-6;

            analysis.Compute();

            foreach (var coefficient in analysis.Coefficients)
            {
                Assert.IsNotNull(coefficient.Analysis);
                Assert.IsNotNull(coefficient.Confidence);
                Assert.IsNotNull(coefficient.ConfidenceLower);
                Assert.IsNotNull(coefficient.ConfidenceUpper);
                Assert.IsNotNull(coefficient.Name);
                Assert.IsNotNull(coefficient.Class);
                Assert.IsNotNull(coefficient.StandardError);
                Assert.IsNotNull(coefficient.Value);
            }


            Assert.AreEqual(13, analysis.Coefficients.Count);
            Assert.AreEqual(2, analysis.CoefficientValues.Length);
            
            var class1 = analysis.CoefficientValues[0];
            Assert.AreEqual(-11.774547061739975, class1[0], 1e-10);
            Assert.AreEqual(0.523813075806107, class1[1], 1e-10);
            Assert.AreEqual(0.36820307277024716, class1[2], 1e-10);
            Assert.AreEqual(0, class1[3], 1e-6);
            Assert.AreEqual(0, class1[4], 1e-6);
            Assert.AreEqual(0, class1[5], 1e-6);

            var class2 = analysis.CoefficientValues[1];
            Assert.AreEqual(-22.721272157115514, class2[0], 1e-10);
            Assert.AreEqual(0.46593949381162203, class2[1], 1e-10);
            Assert.AreEqual(0.68590438098052586, class2[2], 1e-10);
            Assert.AreEqual(0, class2[3], 1e-6);
            Assert.AreEqual(0, class2[4], 1e-6);
            Assert.AreEqual(0, class2[5], 1e-6);
        }

    }
}
