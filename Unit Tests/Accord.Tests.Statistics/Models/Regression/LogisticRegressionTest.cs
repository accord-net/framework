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
    using Accord.IO;
    using Accord.Math;
    using Accord.Math.Optimization.Losses;
    using Accord.Statistics.Analysis;
    using Accord.Statistics.Links;
    using Accord.Statistics.Models.Regression;
    using Accord.Statistics.Models.Regression.Fitting;
    using Accord.Tests.Statistics.Properties;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;

    [TestFixture]
    public class LogisticRegressionTest
    {

        [Test]
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

            teacher.Regularization = 0;

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

            double[] actual = new double[output.Length];
            for (int i = 0; i < input.Length; i++)
                actual[i] = regression.Compute(input[i]);

            double[] expected =
            {
                0.21044171560168326,
                0.13242527535212373,
                0.65747803433771812,
                0.18122484822324372,
                0.74755661773156912,
                0.61450041841477232,
                0.33116705418194975,
                0.14474110902457912,
                0.43627109657399382,
                0.54419383282533118
            };

            for (int i = 0; i < actual.Length; i++)
                Assert.AreEqual(expected[i], actual[i], 1e-8);

            Assert.AreEqual(1.0208597028836701, ageOdds, 1e-10);
            Assert.AreEqual(5.8584748789881331, smokeOdds, 1e-8);

            Assert.AreEqual(-2.4577464307294092, regression.Intercept, 1e-8);
            Assert.AreEqual(-2.4577464307294092, regression.Coefficients[0], 1e-8);
            Assert.AreEqual(0.020645118265359252, regression.Coefficients[1], 1e-10);
            Assert.AreEqual(1.7678893101571855, regression.Coefficients[2], 1e-8);

            bool[] actualOutput = regression.Decide(input);
            Assert.IsFalse(actualOutput[0]);
            Assert.IsFalse(actualOutput[1]);
            Assert.IsTrue(actualOutput[2]);
            Assert.IsFalse(actualOutput[3]);
            Assert.IsTrue(actualOutput[4]);
            Assert.IsTrue(actualOutput[5]);
            Assert.IsFalse(actualOutput[6]);
            Assert.IsFalse(actualOutput[7]);
            Assert.IsFalse(actualOutput[8]);
            Assert.IsTrue(actualOutput[9]);
        }

        [Test]
        public void learn_new_mechanism()
        {
            Accord.Math.Random.Generator.Seed = 0;

            #region doc_log_reg_1
            // Suppose we have the following data about some patients.
            // The first variable is continuous and represent patient
            // age. The second variable is dichotomic and give whether
            // they smoke or not (This is completely fictional data).

            // We also know if they have had lung cancer or not, and 
            // we would like to know whether smoking has any connection
            // with lung cancer (This is completely fictional data).

            double[][] input =
            {              // age, smokes?, had cancer?
                new double[] { 55,    0  }, // false - no cancer
                new double[] { 28,    0  }, // false
                new double[] { 65,    1  }, // false
                new double[] { 46,    0  }, // true  - had cancer
                new double[] { 86,    1  }, // true
                new double[] { 56,    1  }, // true
                new double[] { 85,    0  }, // false
                new double[] { 33,    0  }, // false
                new double[] { 21,    1  }, // false
                new double[] { 42,    1  }, // true
            };

            bool[] output = // Whether each patient had lung cancer or not
            {
                false, false, false, true, true, true, false, false, false, true
            };


            // To verify this hypothesis, we are going to create a logistic
            // regression model for those two inputs (age and smoking), learned
            // using a method called "Iteratively Reweighted Least Squares":

            var learner = new IterativeReweightedLeastSquares<LogisticRegression>()
            {
                Tolerance = 1e-4,  // Let's set some convergence parameters
                Iterations = 100,  // maximum number of iterations to perform
                Regularization = 0
            };

            // Now, we can use the learner to finally estimate our model:
            LogisticRegression regression = learner.Learn(input, output);

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

            // We can also obtain confidence intervals for the odd ratios:
            DoubleRange ageRange = regression.GetConfidenceInterval(1);   // { 0.955442466180864, 1.09075592717851 }
            DoubleRange smokeRange = regression.GetConfidenceInterval(2); // { 0.326598216009923, 105.088535240304 }

            // If we would like to use the model to predict a probability for
            // each patient regarding whether they are at risk of cancer or not,
            // we can use the Probability function:

            double[] scores = regression.Probability(input);

            // Finally, if we would like to arrive at a conclusion regarding
            // each patient, we can use the Decide method, which will transform
            // the probabilities (from 0 to 1) into actual true/false values:

            bool[] actual = regression.Decide(input);
            #endregion

            double[] expected =
            {
                0.21044171509541, 0.132425274863516, 0.657478034489772, 0.181224847711481, 0.747556618035989, 0.614500418479497, 0.331167053803838, 0.144741108525755, 0.436271096256738, 0.544193832738005
            };

            string str = scores.ToCSharp();
            for (int i = 0; i < scores.Length; i++)
                Assert.AreEqual(expected[i], scores[i], 1e-8);

            double[] transform = regression.Transform(input, scores);
            for (int i = 0; i < scores.Length; i++)
                Assert.AreEqual(expected[i], transform[i], 1e-8);

            Assert.AreEqual(1.0208597028836701, ageOdds, 1e-10);
            Assert.AreEqual(5.8584748789881331, smokeOdds, 1e-6);

            Assert.AreEqual(-2.4577464307294092, regression.Intercept, 1e-8);
            Assert.AreEqual(-2.4577464307294092, regression.Coefficients[0], 1e-8);
            Assert.AreEqual(0.020645118265359252, regression.Coefficients[1], 1e-10);
            Assert.AreEqual(1.7678893101571855, regression.Coefficients[2], 1e-8);

            Assert.IsTrue(new[] { 0.955442466180864, 1.09075592717851 }.IsEqual(ageRange, atol: 1e-10));
            Assert.IsTrue(new[] { 0.326598216009923, 105.088535240304 }.IsEqual(smokeRange, atol: 1e-10));

            Assert.IsFalse(actual[0]);
            Assert.IsFalse(actual[1]);
            Assert.IsTrue(actual[2]);
            Assert.IsFalse(actual[3]);
            Assert.IsTrue(actual[4]);
            Assert.IsTrue(actual[5]);
            Assert.IsFalse(actual[6]);
            Assert.IsFalse(actual[7]);
            Assert.IsFalse(actual[8]);
            Assert.IsTrue(actual[9]);
        }

        [Test]
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
            double[][] inputs = Matrix.ColumnVector(inputExtended.GetColumn(0)).ToJagged();
            double[] outputs = inputExtended.GetColumn(1);
            LogisticRegression target = new LogisticRegression(1);
            IterativeReweightedLeastSquares irls = new IterativeReweightedLeastSquares(target);
            irls.Run(inputs, outputs);

            // Fit using grouped data
            double[][] inputs2 = Matrix.ColumnVector(inputGroupProb.GetColumn(0)).ToJagged();
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


            double[][] inputs3 = Matrix.ColumnVector(data.GetColumn(0)).ToJagged();
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

        [Test]
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

            teacher.Regularization = 0;


            double delta = 0;
            do
            {
                delta = teacher.Run(input, output, weights);

            } while (delta > 0.001);


            double ageOdds = regression.GetOddsRatio(1);
            double smokeOdds = regression.GetOddsRatio(2);

            Assert.AreEqual(1.0208597028836701, ageOdds, 1e-10);
            Assert.AreEqual(5.8584748789881331, smokeOdds, 1e-8);
            Assert.AreEqual(-2.4577464307294092, regression.Intercept, 1e-8);
            Assert.AreEqual(-2.4577464307294092, regression.Coefficients[0], 1e-8);
            Assert.AreEqual(0.020645118265359252, regression.Coefficients[1], 1e-8);
            Assert.AreEqual(1.7678893101571855, regression.Coefficients[2], 1e-8);
        }

        [Test]
        public void scores_probabilities_test()
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


            var teacher = new IterativeReweightedLeastSquares<LogisticRegression>()
            {
                Regularization = 0
            };

            var target = teacher.Learn(input, output, weights);

            LogitLinkFunction link = (LogitLinkFunction)target.Link;
            Assert.AreEqual(0, link.A);
            Assert.AreEqual(1, link.B);

            Assert.AreEqual(-2.4577464307294092, target.Intercept, 1e-8);
            Assert.AreEqual(-2.4577464307294092, target.Coefficients[0], 1e-8);
            Assert.AreEqual(0.020645118265359252, target.Coefficients[1], 1e-8);
            Assert.AreEqual(1.7678893101571855, target.Coefficients[2], 1e-8);

            // Test Scores, LogLikelihoods and Probability functions 
            // https://github.com/accord-net/framework/issues/570

            double[][] scoresAllSamples = target.Scores(input);
            double[][] logLikelihoodsAllSamples = target.LogLikelihoods(input);
            double[][] probabilitiesAllSamples = target.Probabilities(input);
            Assert.IsTrue(scoresAllSamples.IsEqual(Matrix.Apply(probabilitiesAllSamples, link.Function), rtol: 1e-5));

            Assert.IsTrue(probabilitiesAllSamples.IsEqual(logLikelihoodsAllSamples.Exp()));
            Assert.IsTrue(probabilitiesAllSamples.Sum(dimension: 1).IsEqual(Vector.Ones(11), rtol: 1e-6));


            bool[] decideAllSamples = target.Decide(input);
            double err = new ZeroOneLoss(output).Loss(decideAllSamples);
            Assert.AreEqual(0.18181818181818182, err, 1e-5);
            Assert.AreEqual(decideAllSamples, scoresAllSamples.ArgMax(dimension: 1).ToBoolean());
            Assert.AreEqual(decideAllSamples.ToInt32(), logLikelihoodsAllSamples.ArgMax(dimension: 1));
            Assert.AreEqual(decideAllSamples, probabilitiesAllSamples.ArgMax(dimension: 1).ToBoolean());

            double[] scoreAllSamples = target.Score(input);
            Assert.AreEqual(scoreAllSamples, scoresAllSamples.GetColumn(1));
            double[] logLikelihoodAllSamples = target.LogLikelihood(input);
            Assert.AreEqual(logLikelihoodAllSamples, logLikelihoodsAllSamples.GetColumn(1));
            double[] probabilityAllSamples = target.Probability(input);
            Assert.AreEqual(probabilityAllSamples, probabilitiesAllSamples.GetColumn(1));

            for (int i = 0; i < input.Length; i++)
            {
                double[] scoresOneSample = target.Scores(input[i]);
                Assert.AreEqual(scoresOneSample, scoresAllSamples[i]);

                double[] logLikelihoodsOneSample = target.LogLikelihoods(input[i]);
                Assert.AreEqual(logLikelihoodsOneSample, logLikelihoodsAllSamples[i]);

                double[] probabilitiesOneSample = target.Probabilities(input[i]);
                Assert.AreEqual(probabilitiesOneSample, probabilitiesAllSamples[i]);

                bool decideOneSample = target.Decide(input[i]);
                Assert.AreEqual(decideOneSample, decideAllSamples[i]);

                double scoreOneSample = target.Score(input[i]);
                Assert.AreEqual(scoreOneSample, scoreAllSamples[i]);

                double logLikelihoodOneSample = target.LogLikelihood(input[i]);
                Assert.AreEqual(logLikelihoodOneSample, logLikelihoodAllSamples[i]);

                double probabilityOneSample = target.Probability(input[i]);
                Assert.AreEqual(probabilityOneSample, probabilityAllSamples[i]);
            }

            bool[] decideScoresAllSamples = null; target.Scores(input, ref decideScoresAllSamples);
            bool[] decideLogLikelihoodsAllSamples = null; target.LogLikelihoods(input, ref decideLogLikelihoodsAllSamples);
            Assert.AreEqual(decideScoresAllSamples, decideLogLikelihoodsAllSamples);
            bool[] decideProbabilitiesAllSamples = null; target.Probabilities(input, ref decideProbabilitiesAllSamples);
            Assert.AreEqual(decideScoresAllSamples, decideProbabilitiesAllSamples);

            bool[] decideScoreAllSamples = null; target.Score(input, ref decideScoreAllSamples);
            Assert.AreEqual(decideScoreAllSamples, decideScoresAllSamples);
            bool[] decideLogLikelihoodAllSamples = null; target.LogLikelihood(input, ref decideLogLikelihoodAllSamples);
            Assert.AreEqual(decideScoreAllSamples, decideLogLikelihoodAllSamples);
            bool[] decideProbabilityAllSamples = null; target.Probability(input, ref decideProbabilityAllSamples);
            Assert.AreEqual(decideScoreAllSamples, decideProbabilityAllSamples);


            for (int i = 0; i < input.Length; i++)
            {
                bool decideScoresOneSample; target.Scores(input[i], out decideScoresOneSample);
                Assert.AreEqual(decideScoresOneSample, decideScoresAllSamples[i]);

                bool decideLogLikelihoodsOneSample; target.LogLikelihoods(input[i], out decideLogLikelihoodsOneSample);
                Assert.AreEqual(decideLogLikelihoodsOneSample, decideLogLikelihoodsAllSamples[i]);

                bool decideProbabilitiesOneSample; target.Probabilities(input[i], out decideProbabilitiesOneSample);
                Assert.AreEqual(decideProbabilitiesOneSample, decideProbabilitiesAllSamples[i]);

                bool decideScoreOneSample; target.Score(input[i], out decideScoreOneSample);
                Assert.AreEqual(decideScoreOneSample, decideScoreAllSamples[i]);

                bool decideLogLikelihoodOneSample; target.LogLikelihood(input[i], out decideLogLikelihoodOneSample);
                Assert.AreEqual(decideLogLikelihoodOneSample, decideLogLikelihoodAllSamples[i]);

                bool decideProbabilityOneSample; target.Probability(input: input[i], decision: out decideProbabilityOneSample);
                Assert.AreEqual(decideProbabilityOneSample, decideProbabilityAllSamples[i]);
            }


            //bool[][] decidesScoresAllSamples = null; target.Scores(input, ref decidesScoresAllSamples);
            //bool[][] decidesLogLikelihoodsAllSamples = null; target.LogLikelihoods(input, ref decidesLogLikelihoodsAllSamples);
            //bool[][] decidesProbabilitiesAllSamples = null; target.Probabilities(input, ref decidesProbabilitiesAllSamples);


            //bool[][] decidesScoreAllSamples = null; target.Score(input, ref decidesScoreAllSamples);
            //bool[][] decidesLogLikelihoodAllSamples = null; target.LogLikelihood(input, ref decidesLogLikelihoodAllSamples);
            //bool[][] decidesProbabilityAllSamples = null; target.Probability(input, ref decidesProbabilityAllSamples);
        }

        [Test]
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

            double[][] input = data.Submatrix(null, 0, 2).ToJagged();
            double[] output = data.GetColumn(3);

            var regression = new LogisticRegression(3);

            var teacher = new IterativeReweightedLeastSquares(regression);

            teacher.Regularization = 1e-10;

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
            Assert.AreEqual(-490.30977151704076, regression.Coefficients[0], 1e-7);
            Assert.AreEqual(1.7763049293456503, regression.Coefficients[1], 1e-7);
            Assert.AreEqual(-14.882619671822592, regression.Coefficients[2], 1e-7);
            Assert.AreEqual(60.5066623676452, regression.Coefficients[3], 1e-7);
        }

#if !NO_DATA_TABLE
        [Test]
        public void RegularizationTest2()
        {
            CsvReader reader = CsvReader.FromText(Properties.Resources.regression, true);

            double[][] data = reader.ToTable().ToArray(System.Globalization.CultureInfo.InvariantCulture);

            double[][] inputs = data.GetColumns(new[] { 0, 1 });

            double[] output = data.GetColumn(2);

            var regression = new LogisticRegression(2);
            var irls = new IterativeReweightedLeastSquares(regression);

            double error = irls.Run(inputs, output);
            double newError = 0;

            for (int i = 0; i < 50; i++)
                newError = irls.Run(inputs, output);

            double actual = irls.ComputeError(inputs, output);
            Assert.AreEqual(30.507262964894068, actual, 1e-8);

            Assert.AreEqual(3, regression.Coefficients.Length);
            Assert.AreEqual(-0.38409721299838279, regression.Coefficients[0], 1e-7);
            Assert.AreEqual(0.1065137931017601, regression.Coefficients[1], 1e-7);
            Assert.AreEqual(22.010378526331344, regression.Coefficients[2], 1e-7);

            for (int i = 0; i < 50; i++)
                newError = irls.Run(inputs, output);

            Assert.AreEqual(-0.38409721299838279, regression.Coefficients[0], 1e-7);
            Assert.AreEqual(0.1065137931017601, regression.Coefficients[1], 1e-8);
            Assert.AreEqual(22.010378588337979, regression.Coefficients[2], 1e-8);
        }
#endif

#if !NO_BINARY_SERIALIZATION
        [Test]
        [Category("Serialization")]
#if NETCORE
        [Ignore("Models created in .NET desktop cannot be de-serialized in .NET Core/Standard (yet)")]
#endif
        public void serialization_test()
        {
            //CsvReader reader = CsvReader.FromText(Properties.Resources.regression, true);
            //double[][] data = reader.ToTable().ToArray(System.Globalization.CultureInfo.InvariantCulture);
            //double[][] inputs = data.GetColumns(new[] { 0, 1 });
            //double[] output = data.GetColumn(2);

            //var regression = new LogisticRegression(2);
            //var irls = new IterativeReweightedLeastSquares(regression);

            //double error = irls.Run(inputs, output);
            //double newError = 0;
            //for (int i = 0; i < 50; i++)
            //    newError = irls.Run(inputs, output);

            //double actual = irls.ComputeError(inputs, output);
            //Assert.AreEqual(30.507262964894068, actual, 1e-8);
            string fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "lr_3.2.3.bin");
            var regression = Serializer.Load<LogisticRegression>(fileName);

            Assert.AreEqual(3, regression.Coefficients.Length);
            Assert.AreEqual(-0.38409721299838279, regression.Coefficients[0], 1e-7);
            Assert.AreEqual(0.1065137931017601, regression.Coefficients[1], 1e-7);
            Assert.AreEqual(22.010378526331344, regression.Coefficients[2], 1e-7);

            Assert.AreEqual(3, regression.StandardErrors.Length);
            Assert.AreEqual(0.44978816773158686, regression.StandardErrors[0], 1e-7);
            Assert.AreEqual(0.051033708973742355, regression.StandardErrors[1], 1e-7);
            Assert.AreEqual(20846.736738575739, regression.StandardErrors[2], 1e-7);

            // regression.Save(@"C:\Users\CésarRoberto\Desktop\lr_3.2.3.bin");
        }
#endif

#if !NO_DATA_TABLE
        [Test]
        public void prediction_interval()
        {
            CsvReader reader = CsvReader.FromText(Properties.Resources.logreg, true);
            DataTable data = reader.ToTable();
            double[][] inputs = data.ToArray("AGE");
            double[] output = data.Columns["CHD"].ToArray();

            var learner = new IterativeReweightedLeastSquares<LogisticRegression>();

            var lr = learner.Learn(inputs, output);

            Assert.AreEqual(0.111, lr.Weights[0], 5e-4);
            Assert.AreEqual(-5.309, lr.Intercept, 5e-4);

            Assert.AreEqual(1.1337, lr.StandardErrors[0], 5e-5);
            Assert.AreEqual(0.0241, lr.StandardErrors[1], 5e-5);

            double ll = lr.GetLogLikelihood(inputs, output);
            Assert.AreEqual(-53.6765, ll, 1e-4);

            double[] point = new double[] { 50 };
            double y = lr.Score(point);

            double[][] im = learner.GetInformationMatrix();
            //double se = lr.GetStandardError(inputs, im);
            var ci = lr.GetConfidenceInterval(point, inputs.Length, im);
            Assert.AreEqual(0.435, ci.Min, 5e-3);
            Assert.AreEqual(0.677, ci.Max, 5e-3);

            var pi = lr.GetPredictionInterval(point, inputs.Length, im);
            Assert.AreEqual(0.1405, pi.Min, 5e-3);
            Assert.AreEqual(0.9075, pi.Max, 5e-3);
        }
#endif
    }
}
