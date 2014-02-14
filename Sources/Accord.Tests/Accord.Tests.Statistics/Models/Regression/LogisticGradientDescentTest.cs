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
    using Accord.Statistics.Models.Regression.Fitting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Accord.Statistics.Models.Regression;
    using Accord.Math.Differentiation;

    [TestClass()]
    public class LogisticGradientDescentTest
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
        public void GradientTest()
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



            double[] expected = finiteDifferences(input, output, false);
            double[] actual = gradient(input, output, false);


            Assert.AreEqual(expected.Length, actual.Length);

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i], 1e-10);
                Assert.IsFalse(Double.IsNaN(actual[i]));
                Assert.IsFalse(Double.IsNaN(expected[i]));
            }
        }

        [TestMethod()]
        public void StochasticGradientTest()
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


            for (int j = 0; j < input.Length; j++)
            {

                double[] expected = finiteDifferences(new[] { input[j] }, new[] { output[j] }, true);
                double[] actual = gradient(new[] { input[j] }, new[] { output[j] }, true);


                Assert.AreEqual(expected.Length, actual.Length);

                for (int i = 0; i < expected.Length; i++)
                {
                    Assert.AreEqual(expected[i], actual[i], 1e-10);
                    Assert.IsFalse(Double.IsNaN(actual[i]));
                    Assert.IsFalse(Double.IsNaN(expected[i]));
                }
            }
        }


        private static double[] finiteDifferences(double[][] input, double[] output, bool stochastic)
        {
            LogisticRegression regression;
            LogisticGradientDescent teacher;

            regression = new LogisticRegression(inputs: 2);

            teacher = new LogisticGradientDescent(regression)
            {
                Stochastic = stochastic,
                LearningRate = 1e-4,
            };

            FiniteDifferences diff = new FiniteDifferences(3);

            diff.Function = (x) =>
            {
                for (int i = 0; i < x.Length; i++)
                    regression.Coefficients[i] = x[i];

                return regression.GetLogLikelihood(input, output);
            };

            return diff.Compute(regression.Coefficients);
        }

        private static double[] gradient(double[][] input, double[] output, bool stochastic)
        {
            LogisticRegression regression;
            LogisticGradientDescent teacher;

            regression = new LogisticRegression(inputs: 2);

            teacher = new LogisticGradientDescent(regression)
            {
                Stochastic = stochastic,
                LearningRate = 1e-4,
            };

            teacher.Run(input, output);
            return teacher.Gradient;
        }


        [TestMethod()]
        public void RunTest()
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
            // will use the Stochastic Gradient Descent algorithm.
            var teacher = new LogisticGradientDescent(regression)
            {
                Stochastic = false,
                LearningRate = 1e-4,
            };

            // Now, we will iteratively estimate our model. The Run method returns
            // the maximum relative change in the model parameters and we will use
            // it as the convergence criteria.

            double delta = 0;
            do
            {
                // Perform an iteration
                delta = teacher.Run(input, output);

            } while (delta > 1e-10);

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


            Assert.AreEqual(1.0208597028836701, ageOdds, 1e-4);
            Assert.AreEqual(5.8584748789881331, smokeOdds, 1e-4);

            Assert.IsFalse(Double.IsNaN(ageOdds));
            Assert.IsFalse(Double.IsNaN(smokeOdds));
        }

        [TestMethod()]
        public void RunTest1()
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
            // will use the Stochastic Gradient Descent algorithm.
            var teacher = new LogisticGradientDescent(regression)
            {
                Stochastic = true,
                LearningRate = 1e-5,
            };

            // Now, we will iteratively estimate our model. The Run method returns
            // the maximum relative change in the model parameters and we will use
            // it as the convergence criteria.

            double delta = 0;
            int iterations = 1;

            do
            {
                // Perform an iteration
                delta = teacher.Run(input, output);

               // teacher.LearningRate *= Math.Exp(-iterations / (double)input.Length);
                iterations++;

            } while (delta > 1e-10);

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


            Assert.AreEqual(1.0208597028836701, ageOdds, 1e-4);
            Assert.AreEqual(5.8584748789881331, smokeOdds, 0.05);

            Assert.IsFalse(Double.IsNaN(ageOdds));
            Assert.IsFalse(Double.IsNaN(smokeOdds));
        }

    }
}
