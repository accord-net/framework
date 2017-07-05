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
    using Accord.Math;
    using Accord.Statistics.Models.Regression;
    using Accord.Statistics.Models.Regression.Fitting;
    using Accord.Statistics.Testing;
    using Math.Differentiation;
    using Math.Optimization;
    using Math.Optimization.Losses;
    using NUnit.Framework;
    using System.Linq;

    [TestFixture]
    public class MultinomialLogisticGradientDescentTest
    {

        [Test]
        public void LearnTest1()
        {
            #region doc_learn_0
            // Declare a simple classification/regression
            // problem with 5 input variables (a,b,c,d,e):
            double[][] inputs =
            {
                new double[] { 1, 4, 2, 0, 1 },
                new double[] { 1, 3, 2, 0, 1 },
                new double[] { 3, 0, 1, 1, 1 },
                new double[] { 3, 0, 1, 0, 1 },
                new double[] { 0, 5, 5, 5, 5 },
                new double[] { 1, 5, 5, 5, 5 },
                new double[] { 1, 0, 0, 0, 0 },
                new double[] { 1, 0, 0, 0, 0 },
                new double[] { 2, 4, 2, 0, 1 },
                new double[] { 2, 4, 2, 0, 1 },
                new double[] { 2, 6, 2, 0, 1 },
                new double[] { 2, 7, 5, 0, 1 },
            };

            // Class labels for each of the inputs
            int[] outputs =
            {
                0, 0, 1, 1, 2, 2, 3, 3, 0, 0, 0, 0
            };
            #endregion

            {
                #region doc_learn_cg
                // Create a Conjugate Gradient algorithm to estimate the regression
                var mcg = new MultinomialLogisticLearning<ConjugateGradient>();

                // Now, we can estimate our model using Conjugate Gradient
                MultinomialLogisticRegression mlr = mcg.Learn(inputs, outputs);

                // We can compute the model answers
                int[] answers = mlr.Decide(inputs);

                // And also the probability of each of the answers
                double[][] probabilities = mlr.Probabilities(inputs);

                // Now we can check how good our model is at predicting
                double error = new ZeroOneLoss(outputs).Loss(answers);
                #endregion

                Assert.AreEqual(0, error, 1e-5);
            }

            {
                #region doc_learn_gd
                // Create a Conjugate Gradient algorithm to estimate the regression
                var mgd = new MultinomialLogisticLearning<GradientDescent>();

                // Now, we can estimate our model using Gradient Descent
                MultinomialLogisticRegression mlr = mgd.Learn(inputs, outputs);

                // We can compute the model answers
                int[] answers = mlr.Decide(inputs);

                // And also the probability of each of the answers
                double[][] probabilities = mlr.Probabilities(inputs);

                // Now we can check how good our model is at predicting
                double error = new ZeroOneLoss(outputs).Loss(answers);
                #endregion

                Assert.AreEqual(0, error, 1e-5);
            }

            {
                #region doc_learn_bfgs
                // Create a Conjugate Gradient algorithm to estimate the regression
                var mlbfgs = new MultinomialLogisticLearning<BroydenFletcherGoldfarbShanno>();

                // Now, we can estimate our model using BFGS
                MultinomialLogisticRegression mlr = mlbfgs.Learn(inputs, outputs);

                // We can compute the model answers
                int[] answers = mlr.Decide(inputs);

                // And also the probability of each of the answers
                double[][] probabilities = mlr.Probabilities(inputs);

                // Now we can check how good our model is at predicting
                double error = new ZeroOneLoss(outputs).Loss(answers);
                #endregion

                Assert.AreEqual(0, error, 1e-5);
            }
        }

        [Test]
        public void RegressTest2()
        {
            Accord.Math.Random.Generator.Seed = 0;

            double[][] inputs;
            int[] outputs;

            MultinomialLogisticRegressionTest.CreateInputOutputsExample1(out inputs, out outputs);

            // Create an algorithm to estimate the regression
            var msgd = new MultinomialLogisticLearning<ConjugateGradient>();

            // Now, we can iteratively estimate our model
            MultinomialLogisticRegression mlr = msgd.Learn(inputs, outputs);

            int[] predicted = mlr.Decide(inputs);

            double acc = new ZeroOneLoss(outputs).Loss(predicted);

            Assert.AreEqual(0.61088435374149663, acc, 1e-8);
        }

        [Test]
        public void GradientTest()
        {
            double[][] inputs;
            int[] outputs;

            MultinomialLogisticRegressionTest.CreateInputOutputsExample1(out inputs, out outputs);

            // Create an algorithm to estimate the regression
            var msgd = new MultinomialLogisticLearning<ConjugateGradient>();

            msgd.Method.MaxIterations = 1;

            msgd.Learn(inputs, outputs);

            int variables = inputs.Columns() * outputs.DistinctCount();
            var fd = new FiniteDifferences(variables, msgd.crossEntropy);

            double[] probe = { 0.1, 0.2, 0.5, 0.6, 0.2, 0.1 };
            double[] expected = fd.Compute(probe);
            double[] actual = msgd.crossEntropyGradient(probe);

            Assert.IsTrue(expected.IsEqual(actual, 1e-5));
        }
    }
}
