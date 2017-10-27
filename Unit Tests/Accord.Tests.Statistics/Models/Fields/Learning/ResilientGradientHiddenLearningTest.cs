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
    public class ResilientGradientHiddenLearningTest
    {

        [Test]
        public void RunTest()
        {
            var inputs = QuasiNewtonHiddenLearningTest.inputs;
            var outputs = QuasiNewtonHiddenLearningTest.outputs;

            HiddenMarkovClassifier hmm = DiscreteHiddenMarkovClassifierPotentialFunctionTest.CreateModel1();
            var function = new MarkovDiscreteFunction(hmm);

            var model = new HiddenConditionalRandomField<int>(function);
            var target = new HiddenResilientGradientLearning<int>(model);

            double[] actual = new double[inputs.Length];
            double[] expected = new double[inputs.Length];

            for (int i = 0; i < inputs.Length; i++)
            {
                actual[i] = model.Compute(inputs[i]);
                expected[i] = outputs[i];
            }

            for (int i = 0; i < inputs.Length; i++)
                Assert.AreEqual(expected[i], actual[i]);

            double ll0 = model.LogLikelihood(inputs, outputs);

            double error = Double.NegativeInfinity;
            for (int i = 0; i < 50; i++)
                error = target.RunEpoch(inputs, outputs);

            double ll1 = model.LogLikelihood(inputs, outputs);

            for (int i = 0; i < inputs.Length; i++)
            {
                actual[i] = model.Compute(inputs[i]);
                expected[i] = outputs[i];
            }

            Assert.AreEqual(-0.00046872579975998363, ll0, 1e-10);
            Assert.AreEqual(0, error, 1e-10);
            Assert.AreEqual(error, ll1);

            for (int i = 0; i < inputs.Length; i++)
                Assert.AreEqual(expected[i], actual[i]);

            Assert.IsTrue(ll1 > ll0);
        }

        [Test]
        public void RunTest3()
        {
            var inputs = QuasiNewtonHiddenLearningTest.inputs;
            var outputs = QuasiNewtonHiddenLearningTest.outputs;

            HiddenMarkovClassifier hmm = DiscreteHiddenMarkovClassifierPotentialFunctionTest.CreateModel1();
            var function = new MarkovDiscreteFunction(hmm);

            var model = new HiddenConditionalRandomField<int>(function);
            var target = new HiddenResilientGradientLearning<int>(model);

            double[] actual = new double[inputs.Length];
            double[] expected = new double[inputs.Length];

            for (int i = 0; i < inputs.Length; i++)
            {
                actual[i] = model.Compute(inputs[i]);
                expected[i] = outputs[i];
            }

            for (int i = 0; i < inputs.Length; i++)
                Assert.AreEqual(expected[i], actual[i]);

            double ll0 = model.LogLikelihood(inputs, outputs);

            double error = Double.NegativeInfinity;
            for (int i = 0; i < 50; i++)
                error = target.RunEpoch(inputs, outputs);

            double ll1 = model.LogLikelihood(inputs, outputs);

            for (int i = 0; i < inputs.Length; i++)
            {
                actual[i] = model.Compute(inputs[i]);
                expected[i] = outputs[i];
            }

            Assert.AreEqual(-0.00046872579975998363, ll0, 1e-10);
            Assert.AreEqual(0, error, 1e-10);
            Assert.AreEqual(error, ll1);

            for (int i = 0; i < inputs.Length; i++)
                Assert.AreEqual(expected[i], actual[i]);

            Assert.IsTrue(ll1 > ll0);
        }


        [Test]
        public void RunTest2()
        {
            var inputs = QuasiNewtonHiddenLearningTest.inputs;
            var outputs = QuasiNewtonHiddenLearningTest.outputs;


            Accord.Math.Tools.SetupGenerator(0);

            var function = new MarkovDiscreteFunction(2, 2, 2);

            var model = new HiddenConditionalRandomField<int>(function);
            var target = new HiddenResilientGradientLearning<int>(model);

            double[] actual = new double[inputs.Length];
            double[] expected = new double[inputs.Length];

            for (int i = 0; i < inputs.Length; i++)
            {
                actual[i] = model.Compute(inputs[i]);
                expected[i] = outputs[i];
            }


            double ll0 = model.LogLikelihood(inputs, outputs);

            double error = Double.PositiveInfinity;
            for (int i = 0; i < 50; i++)
            {
                error = target.RunEpoch(inputs, outputs);
            }

            double ll1 = model.LogLikelihood(inputs, outputs);

            for (int i = 0; i < inputs.Length; i++)
            {
                actual[i] = model.Compute(inputs[i]);
                expected[i] = outputs[i];
            }


            Assert.AreEqual(-5.5451774444795623, ll0, 1e-10);
            Assert.AreEqual(0, error, 1e-10);
            Assert.IsFalse(double.IsNaN(error));

            for (int i = 0; i < inputs.Length; i++)
                Assert.AreEqual(expected[i], actual[i]);

            Assert.IsTrue(ll1 > ll0);
        }

        [Test]
        public void ComputeTest2()
        {
            // Suppose we would like to learn how to classify the
            // following set of sequences among three class labels: 

            int[][] inputSequences =
            {
                // First class of sequences: starts and
                // ends with zeros, ones in the middle:
                new[] { 0, 1, 1, 1, 0 },        
                new[] { 0, 0, 1, 1, 0, 0 },     
                new[] { 0, 1, 1, 1, 1, 0 },     
 
                // Second class of sequences: starts with
                // twos and switches to ones until the end.
                new[] { 2, 2, 2, 2, 1, 1, 1, 1, 1 },
                new[] { 2, 2, 1, 2, 1, 1, 1, 1, 1 },
                new[] { 2, 2, 2, 2, 2, 1, 1, 1, 1 },
 
                // Third class of sequences: can start
                // with any symbols, but ends with three.
                new[] { 0, 0, 1, 1, 3, 3, 3, 3 },
                new[] { 0, 0, 0, 3, 3, 3, 3 },
                new[] { 1, 0, 1, 2, 2, 2, 3, 3 },
                new[] { 1, 1, 2, 3, 3, 3, 3 },
                new[] { 0, 0, 1, 1, 3, 3, 3, 3 },
                new[] { 2, 2, 0, 3, 3, 3, 3 },
                new[] { 1, 0, 1, 2, 3, 3, 3, 3 },
                new[] { 1, 1, 2, 3, 3, 3, 3 },
            };

            // Now consider their respective class labels
            int[] outputLabels =
            {
                /* Sequences  1-3 are from class 0: */ 0, 0, 0,
                /* Sequences  4-6 are from class 1: */ 1, 1, 1,
                /* Sequences 7-14 are from class 2: */ 2, 2, 2, 2, 2, 2, 2, 2
            };


            // Create the Hidden Conditional Random Field using a set of discrete features
            var function = new MarkovDiscreteFunction(states: 3, symbols: 4, outputClasses: 3);
            var classifier = new HiddenConditionalRandomField<int>(function);

            // Create a learning algorithm
            var teacher = new HiddenResilientGradientLearning<int>(classifier)
            {
                Iterations = 50
            };

            // Run the algorithm and learn the models
            teacher.Run(inputSequences, outputLabels);


            // After training has finished, we can check the 
            // output classification label for some sequences. 

            int y1 = classifier.Compute(new[] { 0, 1, 1, 1, 0 });    // output is y1 = 0
            int y2 = classifier.Compute(new[] { 0, 0, 1, 1, 0, 0 }); // output is y1 = 0

            int y3 = classifier.Compute(new[] { 2, 2, 2, 2, 1, 1 }); // output is y2 = 1
            int y4 = classifier.Compute(new[] { 2, 2, 1, 1 });       // output is y2 = 1

            int y5 = classifier.Compute(new[] { 0, 0, 1, 3, 3, 3 }); // output is y3 = 2
            int y6 = classifier.Compute(new[] { 2, 0, 2, 2, 3, 3 }); // output is y3 = 2

            Assert.AreEqual(0, y1);
            Assert.AreEqual(0, y2);
            Assert.AreEqual(1, y3);
            Assert.AreEqual(1, y4);
            Assert.AreEqual(2, y5);
            Assert.AreEqual(2, y6);
        }



        [Test, Category("Intensive")]
        [Ignore("Intensive")] // reproducible parallelization of this test requires #870
        public void learn_pendigits_normalization()
        {
            Console.WriteLine("Starting ResilientGradientHiddenLearningTest.learn_pendigits_normalization");
            string localDownloadPath = Path.Combine(NUnit.Framework.TestContext.CurrentContext.TestDirectory, "pendigits3");

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

                //// The following line is only needed to ensure reproducible results. Please remove it to enable full parallelization
                //teacher1.ParallelOptions.MaxDegreeOfParallelism = 1; // (Remove, comment, or change this line to enable full parallelism)

                // Use the learning algorithm to create a classifier
                var hmmc = teacher1.Learn(trainInputs, trainOutputs);

                // Create a new learning algorithm for creating HCRFs
                var teacher2 = new HiddenResilientGradientLearning<double[]>()
                {
                    Function = new MarkovMultivariateFunction(hmmc),

                    MaxIterations = 10
                };

                //// The following line is only needed to ensure reproducible results. Please remove it to enable full parallelization
                //teacher2.ParallelOptions.MaxDegreeOfParallelism = 1; // (Remove, comment, or change this line to enable full parallelism)

                // Use the learning algorithm to create a classifier
                var hcrf = teacher2.Learn(trainInputs, trainOutputs);

                // Compute predictions for the training set
                int[] trainPredicted = hcrf.Decide(trainInputs);

                // Check the performance of the classifier by comparing with the ground-truth:
                var m1 = new GeneralConfusionMatrix(predicted: trainPredicted, expected: trainOutputs);
                double trainAcc = m1.Accuracy; // should be 0.81532304173813608


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
                double testAcc = m2.Accuracy; // should be 0.77061649319455561
                #endregion

                var loss = new Accord.Math.Optimization.Losses.ZeroOneLoss(testOutputs).Loss(testPredicted);
                Assert.AreEqual(1.0 - loss, m2.Accuracy);

                Assert.AreEqual(10, m1.Classes);
                Assert.AreEqual(10, m2.Classes);

#if NET35
            Assert.AreEqual(0.89594053744997137d, trainAcc, 1e-5);
            Assert.AreEqual(0.89605017347211102d, testAcc, 1e-5);
#else
                Assert.IsTrue(trainAcc.IsEqual(0.81532304173813608, 1e-5) || trainAcc.IsEqual(0.81532304173813608, 1e-5));
                Assert.IsTrue(testAcc.IsEqual(0.77061649319455561, 1e-5) || testAcc.IsEqual(0.77061649319455561, 1e-5));
#endif
            }
        }
    }
}