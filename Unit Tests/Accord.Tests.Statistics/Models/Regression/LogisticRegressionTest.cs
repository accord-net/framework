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
    using Accord.Statistics.Models.Regression;
    using Accord.Statistics.Models.Regression.Fitting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;

    [TestClass()]
    public class LogisticRegressionTest
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
            // they smoke or not (This is completely fictional data).
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

            // We also know if they have had lung cancer or not, and 
            // we would like to know whether smoking has any connection
            // with lung cancer (This is completely fictional data).
            double[] output =
            {
                0, 0, 0, 1, 1, 1, 0, 0, 0, 1
            };


            // To verify this hypothesis, we are going to create a logistic
            // regression model for those two inputs (age and smoking).
            LogisticRegression regression = new LogisticRegression(inputs: 2);

            // Next, we are going to estimate this model. For this, we
            // will use the Iteratively Reweighted Least Squares method.
            var teacher = new IterativeReweightedLeastSquares(regression);

            // Now, we will iteratively estimate our model. The Run method returns
            // the maximum relative change in the model parameters and we will use
            // it as the convergence criteria.

            double delta = 0;
            do
            {
                // Perform an iteration
                delta = teacher.Run(input, output);

            } while (delta > 0.001);

            // At this point, we can compute the odds ratio of our variables.
            // In the model, the variable at 0 is always the intercept term, 
            // with the other following in the sequence. Index 1 is the age
            // and index 2 is whether the patient smokes or not.

            // For the age variable, we have that individuals with
            //   higher age have 1.021 greater odds of getting lung
            //   cancer controlling for cigarette smoking.
            double ageOdds = regression.GetOddsRatio(1); // 1.0208597028836701

            // For the smoking/non smoking category variable, however, we
            //   have that individuals who smoke have 5.858 greater odds
            //   of developing lung cancer compared to those who do not 
            //   smoke, controlling for age (remember, this is completely
            //   fictional and for demonstration purposes only).
            double smokeOdds = regression.GetOddsRatio(2); // 5.8584748789881331


            Assert.AreEqual(1.0208597028836701, ageOdds, 1e-10);
            Assert.AreEqual(5.8584748789881331, smokeOdds, 1e-10);
            Assert.IsFalse(double.IsNaN(ageOdds));
            Assert.IsFalse(double.IsNaN(smokeOdds));

            Assert.AreEqual(-2.4577464307294092, regression.Intercept);
            Assert.AreEqual(-2.4577464307294092, regression.Coefficients[0]);
            Assert.AreEqual(0.020645118265359252, regression.Coefficients[1], 1e-15);
            Assert.AreEqual(1.7678893101571855, regression.Coefficients[2], 1e-15);
        }

        [TestMethod()]
        public void RegressTest()
        {

            double[,] inputGrouped =
            {
                { 1, 4, 5 }, // product 1 has four occurrences of class 1 and five  of class 0
                { 2, 1, 3 }, // product 2 has one  occurrence  of class 1 and three of class 0
            };

            double[,] inputGroupProb =
            {
                { 1, 4.0 / (4 + 5) }, // product 1 has 0.44 probability of belonging to class 1
                { 2, 1.0 / (1 + 3) }, // product 2 has 0.25 probability of belonging to class 1
            };


            double[,] inputExtended =
            {
                { 1, 1 }, // observation of product 1 in class 1
                { 1, 1 }, // observation of product 1 in class 1
                { 1, 1 }, // observation of product 1 in class 1
                { 1, 1 }, // observation of product 1 in class 1
                { 1, 0 }, // observation of product 1 in class 0
                { 1, 0 }, // observation of product 1 in class 0
                { 1, 0 }, // observation of product 1 in class 0
                { 1, 0 }, // observation of product 1 in class 0
                { 1, 0 }, // observation of product 1 in class 0
                { 2, 1 }, // observation of product 2 in class 1
                { 2, 0 }, // observation of product 2 in class 0
                { 2, 0 }, // observation of product 2 in class 0
                { 2, 0 }, // observation of product 2 in class 0
            };


            // Fit using extended data
            double[][] inputs = Matrix.ColumnVector(inputExtended.GetColumn(0)).ToArray();
            double[] outputs = inputExtended.GetColumn(1);
            LogisticRegression target = new LogisticRegression(1);
            IterativeReweightedLeastSquares irls = new IterativeReweightedLeastSquares(target);
            irls.Run(inputs, outputs);

            // Fit using grouped data
            double[][] inputs2 = Matrix.ColumnVector(inputGroupProb.GetColumn(0)).ToArray();
            double[] outputs2 = inputGroupProb.GetColumn(1);
            LogisticRegression target2 = new LogisticRegression(1);
            IterativeReweightedLeastSquares irls2 = new IterativeReweightedLeastSquares(target2);
            irls2.Run(inputs2, outputs2);


            Assert.IsTrue(Matrix.IsEqual(target.Coefficients, target2.Coefficients, 0.000001));



            double[,] data = new double[,]
            {
                {  1, 0 },
                {  2, 0 },
                {  3, 0 },
                {  4, 0 },
                {  5, 1 },
                {  6, 0 },
                {  7, 1 },
                {  8, 0 },
                {  9, 1 },
                { 10, 1 }
            };


            double[][] inputs3 = Matrix.ColumnVector(data.GetColumn(0)).ToArray();
            double[] outputs3 = data.GetColumn(1);
            LogisticRegressionAnalysis analysis = new LogisticRegressionAnalysis(inputs3, outputs3);

            analysis.Compute();

            Assert.IsFalse(double.IsNaN(analysis.Deviance));
            Assert.IsFalse(double.IsNaN(analysis.ChiSquare.PValue));

            Assert.AreEqual(analysis.Deviance, 8.6202, 0.0005);
            Assert.AreEqual(analysis.ChiSquare.PValue, 0.0278, 0.0005);

            // Check intercept
            Assert.IsFalse(double.IsNaN(analysis.Coefficients[0].Value));
            Assert.AreEqual(analysis.Coefficients[0].Value, -4.3578, 0.0005);

            // Check coefficients
            Assert.IsFalse(double.IsNaN(analysis.Coefficients[1].Value));
            Assert.AreEqual(analysis.Coefficients[1].Value, 0.6622, 0.0005);

            // Check statistics
            Assert.AreEqual(analysis.Coefficients[1].StandardError, 0.4001, 0.0005);
            Assert.AreEqual(analysis.Coefficients[1].Wald.PValue, 0.0979, 0.0005);

            Assert.AreEqual(analysis.Coefficients[1].OddsRatio, 1.9391, 0.0005);

            Assert.AreEqual(analysis.Coefficients[1].ConfidenceLower, 0.8852, 0.0005);
            Assert.AreEqual(analysis.Coefficients[1].ConfidenceUpper, 4.2478, 0.0005);


            Assert.IsFalse(double.IsNaN(analysis.Coefficients[1].Wald.PValue));
            Assert.IsFalse(double.IsNaN(analysis.Coefficients[1].StandardError));
            Assert.IsFalse(double.IsNaN(analysis.Coefficients[1].OddsRatio));
            Assert.IsFalse(double.IsNaN(analysis.Coefficients[1].ConfidenceLower));
            Assert.IsFalse(double.IsNaN(analysis.Coefficients[1].ConfidenceUpper));
        }

        [TestMethod()]
        public void ComputeTest3()
        {
            double[][] input =
            {
                new double[] { 55, 0 }, // 0 - no cancer
                new double[] { 28, 0 }, // 0
                new double[] { 65, 1 }, // 0
                new double[] { 46, 0 }, // 1 - have cancer

                new double[] { 86, 1 }, // 1
                new double[] { 86, 1 }, // 1
                new double[] { 56, 1 }, // 1
                new double[] { 85, 0 }, // 0

                new double[] { 33, 0 }, // 0
                new double[] { 21, 1 }, // 0
                new double[] { 42, 1 }, // 1
            };

            double[] output =
            {
                0, 0, 0, 1,
                1, 1, 1, 0, 
                0, 0, 1
            };

            double[] weights =
            {
                1.0, 1.0, 1.0, 1.0, 
                0.5, 0.5, 1.0, 1.0,
                1.0, 1.0, 1.0
            };


            LogisticRegression regression = new LogisticRegression(inputs: 2);

            var teacher = new IterativeReweightedLeastSquares(regression);


            double delta = 0;
            do
            {
                delta = teacher.Run(input, output, weights);

            } while (delta > 0.001);


            double ageOdds = regression.GetOddsRatio(1);
            double smokeOdds = regression.GetOddsRatio(2);

            Assert.AreEqual(1.0208597028836701, ageOdds, 1e-10);
            Assert.AreEqual(5.8584748789881331, smokeOdds, 1e-10);
            Assert.IsFalse(double.IsNaN(ageOdds));
            Assert.IsFalse(double.IsNaN(smokeOdds));


            Assert.AreEqual(-2.4577464307294092, regression.Intercept);
            Assert.AreEqual(-2.4577464307294092, regression.Coefficients[0]);
            Assert.AreEqual(0.020645118265359252, regression.Coefficients[1]);
            Assert.AreEqual(1.7678893101571855, regression.Coefficients[2]);
        }

        [TestMethod()]
        public void LargeCoefficientsTest()
        {
            double[,] data =
            {
                { 48, 1, 4.40, 0 },
                { 60, 0, 7.89, 1 },
                { 51, 0, 3.48, 0 },
                { 66, 0, 8.41, 1 },
                { 40, 1, 3.05, 0 },
                { 44, 1, 4.56, 0 },
                { 80, 0, 6.91, 1 },
                { 52, 0, 5.69, 0 },
                { 58, 0, 4.01, 0 },
                { 58, 0, 4.48, 0 },
                { 72, 1, 5.97, 0 },
                { 57, 0, 6.71, 1 },
                { 55, 1, 5.36, 0 },
                { 71, 0, 5.68, 0 },
                { 44, 1, 4.61, 0 },
                { 65, 1, 4.80, 0 },
                { 38, 0, 5.06, 0 },
                { 50, 0, 6.40, 0 },
                { 80, 0, 6.67, 1 },
                { 69, 1, 5.79, 0 },
                { 39, 0, 5.42, 0 },
                { 68, 0, 7.61, 1 },
                { 47, 1, 3.24, 0 },
                { 45, 1, 4.29, 0 },
                { 79, 1, 7.44, 1 },
                { 41, 1, 4.60, 0 },
                { 45, 0, 5.91, 0 },
                { 54, 0, 4.77, 0 },
                { 43, 1, 5.62, 0 },
                { 62, 1, 7.92, 1 },
                { 72, 1, 7.92, 1 },
                { 57, 1, 6.19, 0 },
                { 39, 1, 2.37, 0 },
                { 51, 0, 5.84, 0 },
                { 73, 1, 5.94, 0 },
                { 41, 1, 3.82, 0 },
                { 35, 0, 2.35, 0 },
                { 69, 0, 6.57, 1 },
                { 75, 1, 7.96, 1 },
                { 51, 1, 3.96, 0 },
                { 61, 1, 4.36, 0 },
                { 55, 0, 3.84, 0 },
                { 45, 1, 3.02, 0 },
                { 48, 0, 4.65, 0 },
                { 77, 0, 7.93, 1 },
                { 40, 1, 2.46, 0 },
                { 37, 1, 2.32, 0 },
                { 78, 0, 7.88, 1 },
                { 39, 1, 4.55, 0 },
                { 41, 0, 2.45, 0 },
                { 54, 1, 5.62, 0 },
                { 59, 1, 5.03, 0 },
                { 78, 0, 8.08, 1 },
                { 56, 1, 6.96, 1 },
                { 49, 1, 3.07, 0 },
                { 48, 0, 4.75, 0 },
                { 63, 1, 5.64, 0 },
                { 50, 0, 3.35, 0 },
                { 59, 1, 5.08, 0 },
                { 60, 0, 6.58, 1 },
                { 64, 0, 5.19, 0 },
                { 76, 1, 6.69, 1 },
                { 58, 0, 5.18, 0 },
                { 48, 1, 4.47, 0 },
                { 72, 0, 8.70, 1 },
                { 40, 1, 5.14, 0 },
                { 53, 0, 3.40, 0 },
                { 79, 0, 9.77, 1 },
                { 61, 1, 7.79, 1 },
                { 59, 0, 7.42, 1 },
                { 44, 0, 2.55, 0 },
                { 52, 1, 3.71, 0 },
                { 80, 1, 7.56, 1 },
                { 76, 0, 7.80, 1 },
                { 51, 0, 5.94, 0 },
                { 46, 1, 5.52, 0 },
                { 48, 0, 3.25, 0 },
                { 58, 1, 4.71, 0 },
                { 44, 1, 2.52, 0 }, 
                { 68, 0, 8.38, 1 },
            };

            double[][] input = data.Submatrix(null, 0, 2).ToArray();
            double[] output = data.GetColumn(3);

            LogisticRegression regression = new LogisticRegression(3);

            var teacher = new IterativeReweightedLeastSquares(regression);

            teacher.Regularization = 1e-5;

            var errors = new List<double>();
            for (int i = 0; i < 1000; i++)
                errors.Add(teacher.Run(input, output));

            double error = 0;
            for (int i = 0; i < output.Length; i++)
            {
                double expected = output[i];
                double actual = System.Math.Round(regression.Compute(input[i]));

                if (expected != actual)
                    error++;
            }

            error /= output.Length;

            Assert.AreEqual(error, 0);
            Assert.AreEqual(-58.817944701474687, regression.Coefficients[0]);
            Assert.AreEqual(0.13783960821658245, regression.Coefficients[1]);
            Assert.AreEqual(-1.532885090757945, regression.Coefficients[2]);
            Assert.AreEqual(7.9460105648631973, regression.Coefficients[3]);
        }
    }
}
