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
    using Accord.Math;
    using Accord.Statistics.Testing;
    using System;
    using Accord.Controls;

    [TestClass()]
    public class StepwiseLogisticRegressionAnalysisTest
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

            // Suppose we have the following data about some patients.
            // The first variable is continuous and represent patient
            // age. The second variable is dichotomic and give whether
            // they smoke or not (this is completely fictional data).

            double[][] inputs =
            {
                //            Age  Smoking
                new double[] { 55,    0   },  // 1
                new double[] { 28,    0   },  // 2
                new double[] { 65,    1   },  // 3
                new double[] { 46,    0   },  // 4
                new double[] { 86,    1   },  // 5
                new double[] { 56,    1   },  // 6
                new double[] { 85,    0   },  // 7
                new double[] { 33,    0   },  // 8
                new double[] { 21,    1   },  // 9
                new double[] { 42,    1   },  // 10
                new double[] { 33,    0   },  // 11
                new double[] { 20,    1   },  // 12
                new double[] { 43,    1   },  // 13
                new double[] { 31,    1   },  // 14
                new double[] { 22,    1   },  // 15
                new double[] { 43,    1   },  // 16
                new double[] { 46,    0   },  // 17
                new double[] { 86,    1   },  // 18
                new double[] { 56,    1   },  // 19
                new double[] { 55,    0   },  // 20
            };

            // Additionally, we also have information about whether
            // or not they those patients had lung cancer. The array
            // below gives 0 for those who did not, and 1 for those
            // who did.

            double[] output =
            {
                0, 0, 0, 1, 1, 1, 0, 0, 0, 1,
                0, 1, 1, 1, 1, 1, 0, 1, 1, 0
            };


            // Create a Stepwise Logistic Regression analysis
            var regression = new StepwiseLogisticRegressionAnalysis(inputs, output,
                new[] { "Age", "Smoking" }, "Cancer");

            regression.Compute(); // compute the analysis.

            // The full model will be stored in the complete property:
            StepwiseLogisticRegressionModel full = regression.Complete;

            // The best model will be stored in the current property:
            StepwiseLogisticRegressionModel best = regression.Current;

            // Let's check the full model results
            // DataGridBox.Show(full.Coefficients); 

            // We can see only the Smoking variable is statistically significant.
            // This is an indication the Age variable could be discarded from
            // the model.

            // And check the best inner model result
            // DataGridBox.Show(best.Coefficients);

            // This is the best nested model found. This model only has the 
            // Smoking variable, which is still significant. Since no other
            // variables can be dropped, this is the best final model.

            // The variables used in the current best model are
            string[] inputVariableNames = best.Inputs; // Smoking

            // The best model likelihood ratio p-value is
            ChiSquareTest test = best.ChiSquare; // {0.816990081334823}

            // so the model is distinguishable from a null model. We can also
            // query the other nested models by checking the Nested property:

            // DataGridBox.Show(regression.Nested);

            // Finally, we can also use the analysis to classify a new patient
            double y = regression.Current.Regression.Compute(new double[] { 1 });

            // For a smoking person, the answer probability is approximately 83%.

            Assert.AreEqual(3, full.Coefficients.Count);
            Assert.AreEqual("Intercept", full.Coefficients[0].Name);
            Assert.AreEqual("Age", full.Coefficients[1].Name);
            Assert.AreEqual("Smoking", full.Coefficients[2].Name);

            Assert.AreEqual(0.10115178966846869, full.Coefficients[0].OddsRatio, 1e-10);
            Assert.AreEqual(1.0071560349008841, full.Coefficients[1].OddsRatio, 1e-10);
            Assert.AreEqual(35.498643454320685, full.Coefficients[2].OddsRatio, 1e-10);
            Assert.IsFalse(full.Coefficients.Apply(p => p.OddsRatio).HasNaN());

            Assert.AreEqual(1.8621025559858235, full.Coefficients[0].StandardError, 1e-10);
            Assert.AreEqual(0.030965622111482096, full.Coefficients[1].StandardError, 1e-10);
            Assert.AreEqual(1.3272612173685281, full.Coefficients[2].StandardError, 1e-10);
            Assert.IsFalse(full.Coefficients.Apply(p => p.StandardError).HasNaN());

            Assert.AreEqual(2, best.Coefficients.Count);
            Assert.AreEqual("Intercept", best.Coefficients[0].Name);
            Assert.AreEqual("Smoking", best.Coefficients[1].Name);

            Assert.AreEqual(0.14285724083908749, best.Coefficients[0].OddsRatio);
            Assert.AreEqual(34.999975694637072, best.Coefficients[1].OddsRatio);

            Assert.AreEqual(1.0685028815195794, best.Coefficients[0].StandardError, 1e-10);
            Assert.AreEqual(1.3197099261438616, best.Coefficients[1].StandardError, 1e-10);
            Assert.IsFalse(best.Coefficients.Apply(p => p.StandardError).HasNaN());

            Assert.AreEqual(2, regression.Nested.Count);
            Assert.AreEqual(best, regression.Nested[0]);
            Assert.AreEqual("Age", regression.Nested[1].Names);

            Assert.AreEqual(0.83333333214363825, y);

            int[] finalVars = regression.Current.Variables;
            double[][] finalData = inputs.Submatrix(null, finalVars);
            double[] expectedOutput = regression.Current.Regression.Compute(finalData);

            Assert.IsTrue(regression.Result.IsEqual(expectedOutput));
        }


        [TestMethod()]
        public void DoStepTest()
        {

            double[][] inputs = Matrix.Expand(
                new double[][] {
                    new double[] {0, 0, 0},
                    new double[] {1, 0, 0},
                    new double[] {0, 1, 0},
                    new double[] {1, 1, 0},
                    new double[] {0, 0, 1},
                    new double[] {1, 0, 1},
                    new double[] {0, 1, 1},
                    new double[] {1, 1, 1},
                }, new int[] { 60, 17, 8, 2, 187, 85, 51, 23 });

            double[] outputs = Matrix.Expand(
                new double[] { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0 },
                new int[] { 5, 60 - 5, 2, 17 - 2, 1, 8 - 1, 0, 2 - 0, 35, 187 - 35, 13, 85 - 13, 15, 51 - 15, 8, 23 - 8 });



            var target2 = new LogisticRegressionAnalysis(inputs, outputs);
            target2.Compute();

            Assert.AreEqual(target2.CoefficientValues[0], -2.377661, 0.0001);
            Assert.AreEqual(target2.CoefficientValues[1], -0.067775, 0.0001);
            Assert.AreEqual(target2.CoefficientValues[2], 0.69531, 0.0001);
            Assert.AreEqual(target2.CoefficientValues[3], 0.871939, 0.0001);


            var target = new StepwiseLogisticRegressionAnalysis(
                inputs, outputs,
                new string[] { "x1", "x2", "x3" }, "Y"
            );

            target.Threshold = 0.15;

            int actual;
            actual = target.DoStep();
            Assert.AreEqual(0, actual);

            actual = target.DoStep();
            Assert.AreEqual(-1, actual);


        }
    }
}
