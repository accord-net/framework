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


namespace Accord.Tests.Statistics.Models.Fields
{
    using Accord.DataSets;
    using Accord.Math;
    using Accord.Statistics.Analysis;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Models.Fields;
    using Accord.Statistics.Models.Fields.Functions;
    using Accord.Statistics.Models.Fields.Learning;
    using Accord.Statistics.Models.Markov;
    using Accord.Statistics.Models.Markov.Learning;
    using Accord.Statistics.Models.Markov.Topology;
    using NUnit.Framework;
    using System;
    using System.IO;

    [TestFixture]
    public partial class NormalQuasiNewtonHiddenLearningTest
    {

        public static double[][] inputs = new double[][]
        {
            new double[] { 0,1,2,3,4 }, // This is the first  sequence with label = 0
            new double[] { 4,3,2,1,0 }, // This is the second sequence with label = 1
        };

        // Labels for the sequences
        public static int[] outputs = { 0, 1 };



        [Test]
        public void RunTest()
        {
            var hmm = MarkovContinuousFunctionTest.CreateModel1();
            var function = new MarkovContinuousFunction(hmm);

            var model = new HiddenConditionalRandomField<double>(function);
            var target = new HiddenQuasiNewtonLearning<double>(model);

            double[] actual = new double[inputs.Length];
            double[] expected = new double[inputs.Length];

            for (int i = 0; i < inputs.Length; i++)
            {
                actual[i] = model.Compute(inputs[i]);
                expected[i] = outputs[i];
            }

            for (int i = 0; i < inputs.Length; i++)
                Assert.AreEqual(expected[i], actual[i]);

            double llm = hmm.LogLikelihood(inputs, outputs);
            double ll0 = model.LogLikelihood(inputs, outputs);
            Assert.AreEqual(llm, ll0, 1e-10);

            double error = target.Run(inputs, outputs);
            double ll1 = model.LogLikelihood(inputs, outputs);
            Assert.AreEqual(-ll1, error, 1e-10);

            for (int i = 0; i < inputs.Length; i++)
            {
                actual[i] = model.Compute(inputs[i]);
                expected[i] = outputs[i];
            }

            Assert.AreEqual(-0.0000041736023099758768, ll0, 1e-10);

            for (int i = 0; i < inputs.Length; i++)
                Assert.AreEqual(expected[i], actual[i]);

            Assert.IsTrue(ll1 > ll0);
        }

        [Test, Category("Intensive")]
        [Ignore("Intensive")] // reproducible parallelization of this test requires #870
        public void learn_pendigits_normalization()
        {
            Console.WriteLine("Starting NormalQuasiNewtonHiddenLearningTest.learn_pendigits_normalization");
            string localDownloadPath = Path.Combine(NUnit.Framework.TestContext.CurrentContext.TestDirectory, "pendigits2");

            using (var travis = new KeepTravisAlive())
            {
                #region doc_learn_pendigits
                // Ensure we get reproducible results
                Accord.Math.Random.Generator.Seed = 0;

                // Download the PENDIGITS dataset from UCI ML repository
                var pendigits = new Pendigits(path: localDownloadPath);

                // Get and pre-process the training set
                double[][][] trainInputs = pendigits.Training.Item1;
                int[] trainOutputs = pendigits.Training.Item2;

                // Pre-process the digits so each of them is centered and scaled
                trainInputs = trainInputs.Apply(Accord.Statistics.Tools.ZScores);
                trainInputs = trainInputs.Apply((x) => x.Subtract(x.Min())); // make them positive

                // Create some prior distributions to help initialize our parameters
                var priorC = new WishartDistribution(dimension: 2, degreesOfFreedom: 5);
                var priorM = new MultivariateNormalDistribution(dimension: 2);

                // Create a new learning algorithm for creating continuous hidden Markov model classifiers
                var teacher1 = new HiddenMarkovClassifierLearning<MultivariateNormalDistribution, double[]>()
                {
                    // This tells the generative algorithm how to train each of the component models. Note: The learning
                    // algorithm is more efficient if all generic parameters are specified, including the fitting options
                    Learner = (i) => new BaumWelchLearning<MultivariateNormalDistribution, double[], NormalOptions>()
                    {
                        Topology = new Forward(5), // Each model will have a forward topology with 5 states

                        // Their emissions will be multivariate Normal distributions initialized using the prior distributions
                        Emissions = (j) => new MultivariateNormalDistribution(mean: priorM.Generate(), covariance: priorC.Generate()),

                        // We will train until the relative change in the average log-likelihood is less than 1e-6 between iterations
                        Tolerance = 1e-6,
                        MaxIterations = 1000, // or until we perform 1000 iterations (which is unlikely for this dataset)

                        // We will prevent our covariance matrices from becoming degenerate by adding a small 
                        // regularization value to their diagonal until they become positive-definite again:
                        FittingOptions = new NormalOptions()
                        {
                            Regularization = 1e-6
                        }
                    }
                };

                // The following line is only needed to ensure reproducible results. Please remove it to enable full parallelization
                teacher1.ParallelOptions.MaxDegreeOfParallelism = 1; // (Remove, comment, or change this line to enable full parallelism)

                // Use the learning algorithm to create a classifier
                var hmmc = teacher1.Learn(trainInputs, trainOutputs);


                // Create a new learning algorithm for creating HCRFs
                var teacher2 = new HiddenQuasiNewtonLearning<double[]>()
                {
                    Function = new MarkovMultivariateFunction(hmmc),

                    MaxIterations = 10
                };

                // The following line is only needed to ensure reproducible results. Please remove it to enable full parallelization
                teacher2.ParallelOptions.MaxDegreeOfParallelism = 1; // (Remove, comment, or change this line to enable full parallelism)

                // Use the learning algorithm to create a classifier
                var hcrf = teacher2.Learn(trainInputs, trainOutputs);

                // Compute predictions for the training set
                int[] trainPredicted = hcrf.Decide(trainInputs);

                // Check the performance of the classifier by comparing with the ground-truth:
                var m1 = new GeneralConfusionMatrix(predicted: trainPredicted, expected: trainOutputs);
                double trainAcc = m1.Accuracy; // should be 0.66523727844482561


                // Prepare the testing set
                double[][][] testInputs = pendigits.Testing.Item1;
                int[] testOutputs = pendigits.Testing.Item2;

                // Apply the same normalizations
                testInputs = testInputs.Apply(Accord.Statistics.Tools.ZScores);
                testInputs = testInputs.Apply((x) => x.Subtract(x.Min())); // make them positive

                // Compute predictions for the test set
                int[] testPredicted = hcrf.Decide(testInputs);

                // Check the performance of the classifier by comparing with the ground-truth:
                var m2 = new GeneralConfusionMatrix(predicted: testPredicted, expected: testOutputs);
                double testAcc = m2.Accuracy; // should be 0.66506538564184681
                #endregion

                Assert.AreEqual(0.66523727844482561, trainAcc, 1e-10);
                Assert.AreEqual(0.66506538564184681, testAcc, 1e-10);
            }
        }
    }
}
