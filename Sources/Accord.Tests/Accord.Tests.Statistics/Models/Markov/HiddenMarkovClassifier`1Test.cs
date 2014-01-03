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
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Models.Markov;
    using Accord.Statistics.Models.Markov.Learning;
    using Accord.Statistics.Models.Markov.Topology;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Math;
    using System;


    [TestClass()]
    public class GenericSequenceClassifierTest
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
        public void LearnTest1()
        {
            // Create a Continuous density Hidden Markov Model Sequence Classifier
            // to detect a univariate sequence and the same sequence backwards.
            double[][] sequences = new double[][] 
            {
                new double[] { 0,1,2,3,4 }, // This is the first  sequence with label = 0
                new double[] { 4,3,2,1,0 }, // This is the second sequence with label = 1
            };

            // Labels for the sequences
            int[] labels = { 0, 1 };

            // Creates a sequence classifier containing 2 hidden Markov Models
            //  with 2 states and an underlying Normal distribution as density.
            NormalDistribution density = new NormalDistribution();
            var classifier = new HiddenMarkovClassifier<NormalDistribution>(2, new Ergodic(2), density);

            // Configure the learning algorithms to train the sequence classifier
            var teacher = new HiddenMarkovClassifierLearning<NormalDistribution>(classifier,

                // Train each model until the log-likelihood changes less than 0.001
                modelIndex => new BaumWelchLearning<NormalDistribution>(classifier.Models[modelIndex])
                {
                    Tolerance = 0.0001,
                    Iterations = 0
                }
            );

            // Train the sequence classifier using the algorithm
            double logLikelihood = teacher.Run(sequences, labels);


            // Calculate the probability that the given
            //  sequences originated from the model
            double likelihood1, likelihood2;

            // Try to classify the first sequence (output should be 0)
            int c1 = classifier.Compute(sequences[0], out likelihood1);

            // Try to classify the second sequence (output should be 1)
            int c2 = classifier.Compute(sequences[1], out likelihood2);

            Assert.AreEqual(0, c1);
            Assert.AreEqual(1, c2);


            Assert.AreEqual(-13.271981026832929, logLikelihood, 1e-10);
            Assert.AreEqual(0.99999791320102149, likelihood1, 1e-10);
            Assert.AreEqual(0.99999791320102149, likelihood2, 1e-10);
            Assert.IsFalse(double.IsNaN(logLikelihood));
            Assert.IsFalse(double.IsNaN(likelihood1));
            Assert.IsFalse(double.IsNaN(likelihood2));
        }

        [TestMethod()]
        public void LearnTest2()
        {
            // Creates a new Hidden Markov Model with 10 states
            var initial = new MultivariateNormalDistribution(3);
            var classifier = new HiddenMarkovClassifier<MultivariateNormalDistribution>(2, new Ergodic(10), initial);

            Assert.AreEqual(3, classifier.Models[0].Dimension);
            Assert.AreEqual(3, classifier.Models[1].Dimension);

            Assert.AreEqual(2, classifier.Models.Length);

            for (int i = 0; i < 2; i++)
            {
                var model = classifier.Models[i];
                Assert.AreEqual(3, model.Dimension);
                Assert.AreEqual(10, model.States);
                Assert.AreEqual(10, model.Emissions.Length);

                for (int j = 0; j < model.Emissions.Length; j++)
                {
                    var distribution = model.Emissions[j];
                    Assert.IsNotNull(distribution);
                    Assert.AreEqual(distribution.Dimension, 3);
                }
            }
        }

        [TestMethod()]
        public void LearnTest3()
        {
            // Create a Continuous density Hidden Markov Model Sequence Classifier
            // to detect a multivariate sequence and the same sequence backwards.
            double[][][] sequences = new double[][][]
            {
                new double[][] 
                { 
                    // This is the first  sequence with label = 0
                    new double[] { 0 },
                    new double[] { 1 },
                    new double[] { 2 },
                    new double[] { 3 },
                    new double[] { 4 },
                }, 

                new double[][]
                {
                     // This is the second sequence with label = 1
                    new double[] { 4 },
                    new double[] { 3 },
                    new double[] { 2 },
                    new double[] { 1 },
                    new double[] { 0 },
                }
            };

            // Labels for the sequences
            int[] labels = { 0, 1 };

            // Creates a sequence classifier containing 2 hidden Markov Models
            //  with 2 states and an underlying Normal distribution as density.
            MultivariateNormalDistribution density = new MultivariateNormalDistribution(1);
            var classifier = new HiddenMarkovClassifier<MultivariateNormalDistribution>(2, new Ergodic(2), density);

            // Configure the learning algorithms to train the sequence classifier
            var teacher = new HiddenMarkovClassifierLearning<MultivariateNormalDistribution>(classifier,

                // Train each model until the log-likelihood changes less than 0.001
                modelIndex => new BaumWelchLearning<MultivariateNormalDistribution>(classifier.Models[modelIndex])
                {
                    Tolerance = 0.0001,
                    Iterations = 0
                }
            );

            // Train the sequence classifier using the algorithm
            double logLikelihood = teacher.Run(sequences, labels);


            // Calculate the probability that the given
            //  sequences originated from the model
            double likelihood1, likelihood2;

            // Try to classify the first sequence (output should be 0)
            int c1 = classifier.Compute(sequences[0], out likelihood1);

            // Try to classify the second sequence (output should be 1)
            int c2 = classifier.Compute(sequences[1], out likelihood2);

            Assert.AreEqual(0, c1);
            Assert.AreEqual(1, c2);

            Assert.AreEqual(-13.271981026832929, logLikelihood, 1e-14);
            Assert.AreEqual(0.99999791320102149, likelihood1, 1e-15);
            Assert.AreEqual(0.99999791320102149, likelihood2, 1e-15);

            Assert.IsFalse(double.IsNaN(logLikelihood));
            Assert.IsFalse(double.IsNaN(likelihood1));
            Assert.IsFalse(double.IsNaN(likelihood2));
        }

        [TestMethod()]
        public void LearnTest4()
        {
            // Create a Continuous density Hidden Markov Model Sequence Classifier
            // to detect a multivariate sequence and the same sequence backwards.
            double[][][] sequences = new double[][][]
            {
                new double[][] 
                { 
                    // This is the first  sequence with label = 0
                    new double[] { 0 },
                    new double[] { 1 },
                    new double[] { 2 },
                    new double[] { 3 },
                    new double[] { 4 },
                }, 

                new double[][]
                {
                        // This is the second sequence with label = 1
                    new double[] { 4 },
                    new double[] { 3 },
                    new double[] { 2 },
                    new double[] { 1 },
                    new double[] { 0 },
                }
            };

            // Labels for the sequences
            int[] labels = { 0, 1 };


            // Create a mixture of two 1-dimensional normal distributions (by default,
            // initialized with zero mean and unit covariance matrices).
            var density = new MultivariateMixture<MultivariateNormalDistribution>(
                new MultivariateNormalDistribution(1),
                new MultivariateNormalDistribution(1));

            // Creates a sequence classifier containing 2 hidden Markov Models with 2 states
            // and an underlying multivariate mixture of Normal distributions as density.
            var classifier = new HiddenMarkovClassifier<MultivariateMixture<MultivariateNormalDistribution>>(
                2, new Ergodic(2), density);

            // Configure the learning algorithms to train the sequence classifier
            var teacher = new HiddenMarkovClassifierLearning<MultivariateMixture<MultivariateNormalDistribution>>(
                classifier,

                // Train each model until the log-likelihood changes less than 0.0001
                modelIndex => new BaumWelchLearning<MultivariateMixture<MultivariateNormalDistribution>>(
                    classifier.Models[modelIndex])
                {
                    Tolerance = 0.0001,
                    Iterations = 0,
                }
            );

            // Train the sequence classifier using the algorithm
            double logLikelihood = teacher.Run(sequences, labels);


            // Calculate the probability that the given
            //  sequences originated from the model
            double likelihood1, likelihood2;

            // Try to classify the 1st sequence (output should be 0)
            int c1 = classifier.Compute(sequences[0], out likelihood1);

            // Try to classify the 2nd sequence (output should be 1)
            int c2 = classifier.Compute(sequences[1], out likelihood2);


            Assert.AreEqual(0, c1);
            Assert.AreEqual(1, c2);

            Assert.AreEqual(-13.271981026832933, logLikelihood, 1e-10);
            Assert.AreEqual(0.99999791320102149, likelihood1, 1e-10);
            Assert.AreEqual(0.99999791320102149, likelihood2, 1e-10);

            Assert.IsFalse(double.IsNaN(logLikelihood));
            Assert.IsFalse(double.IsNaN(likelihood1));
            Assert.IsFalse(double.IsNaN(likelihood2));
        }

        [TestMethod()]
        public void LearnTest5()
        {
            // Create a Continuous density Hidden Markov Model Sequence Classifier
            // to detect a multivariate sequence and the same sequence backwards.
            double[][][] sequences = new double[][][]
            {
                new double[][] 
                { 
                    // This is the first  sequence with label = 0
                    new double[] { 0, 1 },
                    new double[] { 1, 2 },
                    new double[] { 2, 3 },
                    new double[] { 3, 4 },
                    new double[] { 4, 5 },
                }, 

                new double[][]
                {
                        // This is the second sequence with label = 1
                    new double[] { 4,  3 },
                    new double[] { 3,  2 },
                    new double[] { 2,  1 },
                    new double[] { 1,  0 },
                    new double[] { 0, -1 },
                }
            };

            // Labels for the sequences
            int[] labels = { 0, 1 };


            var density = new MultivariateNormalDistribution(2);

            // Creates a sequence classifier containing 2 hidden Markov Models with 2 states
            // and an underlying multivariate mixture of Normal distributions as density.
            var classifier = new HiddenMarkovClassifier<MultivariateNormalDistribution>(
                2, new Ergodic(2), density);

            // Configure the learning algorithms to train the sequence classifier
            var teacher = new HiddenMarkovClassifierLearning<MultivariateNormalDistribution>(
                classifier,

                // Train each model until the log-likelihood changes less than 0.0001
                modelIndex => new BaumWelchLearning<MultivariateNormalDistribution>(
                    classifier.Models[modelIndex])
                {
                    Tolerance = 0.0001,
                    Iterations = 0,

                    FittingOptions = new NormalOptions() { Diagonal = true }
                }
            );

            // Train the sequence classifier using the algorithm
            double logLikelihood = teacher.Run(sequences, labels);


            // Calculate the probability that the given
            //  sequences originated from the model
            double logLikelihood1, logLikelihood2;

            // Try to classify the 1st sequence (output should be 0)
            int c1 = classifier.Compute(sequences[0], out logLikelihood1);

            // Try to classify the 2nd sequence (output should be 1)
            int c2 = classifier.Compute(sequences[1], out logLikelihood2);


            Assert.AreEqual(0, c1);
            Assert.AreEqual(1, c2);

            Assert.AreEqual(-24.560599651649841, logLikelihood, 1e-10);
            Assert.AreEqual(0.99999999998806466, logLikelihood1, 1e-10);
            Assert.AreEqual(0.99999999998806466, logLikelihood2, 1e-10);

            Assert.IsFalse(double.IsNaN(logLikelihood));
            Assert.IsFalse(double.IsNaN(logLikelihood1));
            Assert.IsFalse(double.IsNaN(logLikelihood2));
        }

        [TestMethod()]
        public void LearnTest6()
        {
            // Create a Continuous density Hidden Markov Model Sequence Classifier
            // to detect a multivariate sequence and the same sequence backwards.
            double[][][] sequences = new double[][][]
            {
                new double[][] 
                { 
                    // This is the first  sequence with label = 0
                    new double[] { 0, 1 },
                    new double[] { 1, 2 },
                    new double[] { 2, 3 },
                    new double[] { 3, 4 },
                    new double[] { 4, 5 },
                }, 

                new double[][]
                {
                        // This is the second sequence with label = 1
                    new double[] { 4,  3 },
                    new double[] { 3,  2 },
                    new double[] { 2,  1 },
                    new double[] { 1,  0 },
                    new double[] { 0, -1 },
                }
            };

            // Labels for the sequences
            int[] labels = { 0, 1 };


            var density = new MultivariateNormalDistribution(2);

            // Creates a sequence classifier containing 2 hidden Markov Models with 2 states
            // and an underlying multivariate mixture of Normal distributions as density.
            var classifier = new HiddenMarkovClassifier<MultivariateNormalDistribution>(
                2, new Custom(new double[2, 2], new double[2]), density);

            // Configure the learning algorithms to train the sequence classifier
            var teacher = new HiddenMarkovClassifierLearning<MultivariateNormalDistribution>(
                classifier,

                // Train each model until the log-likelihood changes less than 0.0001
                modelIndex => new BaumWelchLearning<MultivariateNormalDistribution>(
                    classifier.Models[modelIndex])
                {
                    Tolerance = 0.0001,
                    Iterations = 0,

                    FittingOptions = new NormalOptions() { Diagonal = true }
                }
            );

            // Train the sequence classifier using the algorithm
            double logLikelihood = teacher.Run(sequences, labels);


            // Calculate the probability that the given
            //  sequences originated from the model
            double response1, response2;

            // Try to classify the 1st sequence (output should be 0)
            int c1 = classifier.Compute(sequences[0], out response1);

            // Try to classify the 2nd sequence (output should be 1)
            int c2 = classifier.Compute(sequences[1], out response2);

            Assert.AreEqual(double.NegativeInfinity, logLikelihood);
            Assert.AreEqual(0, response1);
            Assert.AreEqual(0, response2);

            Assert.IsFalse(double.IsNaN(logLikelihood));
            Assert.IsFalse(double.IsNaN(response1));
            Assert.IsFalse(double.IsNaN(response2));
        }

        [TestMethod()]
        public void LearnTest7()
        {
            // Create a Continuous density Hidden Markov Model Sequence Classifier
            // to detect a multivariate sequence and the same sequence backwards.

            double[][][] sequences = new double[][][]
            {
                new double[][] 
                { 
                    // This is the first  sequence with label = 0
                    new double[] { 0, 1 },
                    new double[] { 1, 2 },
                    new double[] { 2, 3 },
                    new double[] { 3, 4 },
                    new double[] { 4, 5 },
                }, 

                new double[][]
                {
                        // This is the second sequence with label = 1
                    new double[] { 4,  3 },
                    new double[] { 3,  2 },
                    new double[] { 2,  1 },
                    new double[] { 1,  0 },
                    new double[] { 0, -1 },
                }
            };

            // Labels for the sequences
            int[] labels = { 0, 1 };


            var initialDensity = new MultivariateNormalDistribution(2);

            // Creates a sequence classifier containing 2 hidden Markov Models with 2 states
            // and an underlying multivariate mixture of Normal distributions as density.
            var classifier = new HiddenMarkovClassifier<MultivariateNormalDistribution>(
                classes: 2, topology: new Forward(2), initial: initialDensity);

            // Configure the learning algorithms to train the sequence classifier
            var teacher = new HiddenMarkovClassifierLearning<MultivariateNormalDistribution>(
                classifier,

                // Train each model until the log-likelihood changes less than 0.0001
                modelIndex => new BaumWelchLearning<MultivariateNormalDistribution>(
                    classifier.Models[modelIndex])
                {
                    Tolerance = 0.0001,
                    Iterations = 0,

                    FittingOptions = new NormalOptions()
                    {
                        Diagonal = true,      // only diagonal covariance matrices
                        Regularization = 1e-5 // avoid non-positive definite errors
                    }
                }
            );

            // Train the sequence classifier using the algorithm
            double logLikelihood = teacher.Run(sequences, labels);


            // Calculate the probability that the given
            //  sequences originated from the model
            double likelihood, likelihood2;

            // Try to classify the 1st sequence (output should be 0)
            int c1 = classifier.Compute(sequences[0], out likelihood);

            // Try to classify the 2nd sequence (output should be 1)
            int c2 = classifier.Compute(sequences[1], out likelihood2);


            Assert.AreEqual(0, c1);
            Assert.AreEqual(1, c2);

            Assert.AreEqual(-24.560663315259973, logLikelihood, 1e-10);
            Assert.AreEqual(0.99999999998805045, likelihood, 1e-10);
            Assert.AreEqual(0.99999999998805045, likelihood2, 1e-10);

            Assert.IsFalse(double.IsNaN(logLikelihood));
            Assert.IsFalse(double.IsNaN(likelihood));
            Assert.IsFalse(double.IsNaN(likelihood2));
        }

        [TestMethod()]
        public void LearnTest8()
        {
            // Declare some testing data
            double[][] inputs = new double[][]
            {
                new double[] { 0,0,1,2 },     // Class 0
                new double[] { 0,1,1,2 },     // Class 0
                new double[] { 0,0,0,1,2 },   // Class 0
                new double[] { 0,1,2,2,2 },   // Class 0

                new double[] { 2,2,1,0 },     // Class 1
                new double[] { 2,2,2,1,0 },   // Class 1
                new double[] { 2,2,2,1,0 },   // Class 1
                new double[] { 2,2,2,2,1 },   // Class 1
            };

            int[] outputs = new int[]
            {
                0,0,0,0, // First four sequences are of class 0
                1,1,1,1, // Last four sequences are of class 1
            };


            // We are trying to predict two different classes
            int classes = 2;

            // Each sequence may have up to 3 symbols (0,1,2)
            int symbols = 3;

            // Nested models will have 3 states each
            int[] states = new int[] { 3, 3 };

            // Creates a new Hidden Markov Model Classifier with the given parameters
            var classifier = HiddenMarkovClassifier.CreateGeneric(classes, states, symbols);


            // Create a new learning algorithm to train the sequence classifier
            var teacher = new HiddenMarkovClassifierLearning<GeneralDiscreteDistribution>(classifier,

                // Train each model until the log-likelihood changes less than 0.001
                modelIndex => new BaumWelchLearning<GeneralDiscreteDistribution>(classifier.Models[modelIndex])
                {
                    Tolerance = 0.001,
                    Iterations = 0
                }
            );

            // Enable support for sequence rejection
            teacher.Rejection = true;

            // Train the sequence classifier using the algorithm
            double likelihood = teacher.Run(inputs, outputs);


            var threshold = classifier.Threshold;

            Assert.AreEqual(classifier.Models[0].Transitions[0, 0], threshold.Transitions[0, 0], 1e-10);
            Assert.AreEqual(classifier.Models[0].Transitions[1, 1], threshold.Transitions[1, 1], 1e-10);
            Assert.AreEqual(classifier.Models[0].Transitions[2, 2], threshold.Transitions[2, 2], 1e-10);

            Assert.AreEqual(classifier.Models[1].Transitions[0, 0], threshold.Transitions[3, 3], 1e-10);
            Assert.AreEqual(classifier.Models[1].Transitions[1, 1], threshold.Transitions[4, 4], 1e-10);
            Assert.AreEqual(classifier.Models[1].Transitions[2, 2], threshold.Transitions[5, 5], 1e-10);

            for (int i = 0; i < 3; i++)
                for (int j = 3; j < 6; j++)
                    Assert.AreEqual(Double.NegativeInfinity, threshold.Transitions[i, j]);

            for (int i = 3; i < 6; i++)
                for (int j = 0; j < 3; j++)
                    Assert.AreEqual(Double.NegativeInfinity, threshold.Transitions[i, j]);

            Assert.IsFalse(Matrix.HasNaN(threshold.Transitions));


            classifier.Sensitivity = 0.5;

            // Will assert the models have learned the sequences correctly.
            for (int i = 0; i < inputs.Length; i++)
            {
                int expected = outputs[i];
                int actual = classifier.Compute(inputs[i], out likelihood);
                Assert.AreEqual(expected, actual);
            }


            double[] r0 = new double[] { 1, 1, 0, 0, 2 };


            double logRejection;
            int c = classifier.Compute(r0, out logRejection);

            Assert.AreEqual(-1, c);
            Assert.AreEqual(0.99893048690086783, logRejection);
            Assert.IsFalse(double.IsNaN(logRejection));

            logRejection = threshold.Evaluate(r0);
            Assert.AreEqual(-4.7048235516322334, logRejection, 1e-10);
            Assert.IsFalse(double.IsNaN(logRejection));

            threshold.Decode(r0, out logRejection);
            Assert.AreEqual(-7.0705785431547579, logRejection, 1e-10);
            Assert.IsFalse(double.IsNaN(logRejection));

            foreach (var model in classifier.Models)
            {
                double[,] A = model.Transitions;

                for (int i = 0; i < A.GetLength(0); i++)
                {
                    double[] row = A.Exp().GetRow(i);
                    double sum = row.Sum();
                    Assert.AreEqual(1, sum, 1e-10);
                }
            }
            {
                double[,] A = classifier.Threshold.Transitions;

                for (int i = 0; i < A.GetLength(0); i++)
                {
                    double[] row = A.GetRow(i);
                    double sum = row.Exp().Sum();
                    Assert.AreEqual(1, sum, 1e-6);
                }
            }
        }



        [TestMethod()]
        public void LearnTest9()
        {
            double[][][] inputs = large_gestures;
            int[] outputs = large_outputs;

            int states = 5;
            int iterations = 100;
            double tolerance = 0.01;
            bool rejection = true;
            double sensitivity = 1E-85;

            int dimension = inputs[0][0].Length;

            var hmm = new HiddenMarkovClassifier<MultivariateNormalDistribution>(2,
                new Forward(states), new MultivariateNormalDistribution(dimension));

            // Create the learning algorithm for the ensemble classifier
            var teacher = new HiddenMarkovClassifierLearning<MultivariateNormalDistribution>(hmm,

                // Train each model using the selected convergence criteria
                i => new BaumWelchLearning<MultivariateNormalDistribution>(hmm.Models[i])
                {
                    Tolerance = tolerance,
                    Iterations = iterations,

                    FittingOptions = new NormalOptions()
                    {
                        Regularization = 1e-5
                    }
                }
            );

            teacher.Empirical = true;
            teacher.Rejection = rejection;

            // Run the learning algorithm
            double logLikelihood = teacher.Run(inputs, outputs);

            hmm.Sensitivity = sensitivity;

            for (int i = 0; i < large_gestures.Length; i++)
            {
                int actual = hmm.Compute(large_gestures[i]);
                int expected = large_outputs[i];
                Assert.AreEqual(expected,actual);
            }
        }



        int[] large_outputs = { 0, 0, 0, 0, 1, 1, 1 };

        double[][][] large_gestures =
        {
            new double[][] // gesture 0
            {
                #region sequence
                new double[] {   1.70607484670038, -0.418406015828554, -0.658357827182728, 0.910629137808571, -0.469253024784096, -0.257835129470835   },
                new double[] {   1.653881619034, -0.411162210682249, -0.775015524625083, 0.887731091115406, -0.469241198076328, -0.30895182927735   },
                new double[] {   1.58637419136102, -0.391336921856269, -0.892424225752619, 0.849212911279741, -0.476227938464957, -0.389638208576432   },
                new double[] {   1.51190288303488, -0.368328135257406, -1.03150239372554, 0.825130872562798, -0.477928863448287, -0.437341443738996   },
                new double[] {   1.39493969590095, -0.338676862356608, -1.17108542595808, 0.835006138766608, -0.419011395107274, -0.500165479031312   },
                new double[] {   1.27756668502921, -0.329140165352181, -1.31433677255895, 0.686535957441046, -0.459347136954787, -0.474609101836646   },
                new double[] {   0.413607364068208, -0.374555812073178, -1.62331623260606, 0.746377062791572, -0.494824002336889, -0.374774817858446   },
                new double[] {   0.305577015845048, -0.416623773917374, -1.38819126673346, 0.687705299530157, -0.458453965880553, -0.566279516255832   },
                new double[] {   0.00727625639756377, -0.401459546928455, -1.57953172464798, 0.559723275040049, -0.427914286843641, -0.689856839588558   },
                new double[] {   -0.22736964254191, -0.385086424151595, -1.55797407797098, 0.342166206322953, -0.300438141754156, -0.817532674144229   },
                new double[] {   -0.515170645407173, -0.403443552332983, -1.52710100369715, 0.320280257450252, -0.331639867455041, -0.83596341272077   },
                new double[] {   -0.684886707894193, -0.429082441572365, -1.38407627075346, 0.120453896458449, -0.332479617271534, -0.926628334195836   },
                new double[] {   -0.779521733777469, -0.464496274809447, -1.23777607617706, 0.106980252498934, -0.473385833593927, -0.67010289612375   },
                new double[] {   -0.974541163812474, -0.500872889365927, -0.927134859663545, -0.178944537872772, -0.43334837922459, -0.792824163994845   },
                new double[] {   -1.01682611855643, -0.518803061959224, -0.777824940466598, -0.210957773205119, -0.437201047833955, -0.753882660477477   },
                new double[] {   -1.0287266835527, -0.531479058924604, -0.660599612528683, -0.188434429978736, -0.452315198293264, -0.702972847749736   },
                new double[] {   -1.16248162658606, -0.58507008866902, -0.385722100815111, -0.313229512718333, -0.451092246483268, -0.588238496394115   },
                new double[] {   -0.958158452521033, -0.538115092899625, -0.38656216800097, -0.320020808162766, -0.457642889204085, -0.561764022221845   },
                new double[] {   -0.937272933101714, -0.560105265326725, -0.367423537332055, -0.324346207442505, -0.471984305228936, -0.534811708297922   },
                new double[] {   -0.910515980221341, -0.577823813797214, -0.356880806779959, -0.25342174692765, -0.492178568040908, -0.568020192286868   },
                new double[] {   -0.905570108523314, -0.566416607085832, -0.335261221026923, -0.255841434961772, -0.490325641493159, -0.564498098229217   },
                new double[] {   -0.904484362573821, -0.562730275399609, -0.326993162536716, -0.259596941639102, -0.494940365894767, -0.561830912741041   },
                new double[] {   -0.904503057252629, -0.548005549666813, -0.322175501948946, -0.255017670151389, -0.49440295285902, -0.565249386830728   },
                new double[] {   -0.903390234623301, -0.536355227598672, -0.320014138420945, -0.254283351252598, -0.493359877552782, -0.566136905686653   },
                new double[] {   -0.903757243008495, -0.533546756809274, -0.330600277212686, -0.247029159377969, -0.489329448094885, -0.571019912745714   },
                new double[] {   -0.906773193947014, -0.525855092671131, -0.336381526325636, -0.245279758259657, -0.489225222614202, -0.57422564584894   },
                new double[] {   -0.906852380696446, -0.530115011915916, -0.343210900540386, -0.245073497526835, -0.488508716932256, -0.577498793365395   },
                new double[] {   -0.902293541582962, -0.551749033120671, -0.363332612598281, -0.241967073691029, -0.48539196890024, -0.580395690063313   },
                new double[] {   -0.905880270111255, -0.54149083334007, -0.36687646188595, -0.240617423595973, -0.485213303456588, -0.582515822485501   },
                new double[] {   -0.906363579834058, -0.541561983978285, -0.367635972000968, -0.241312238131419, -0.485253793960694, -0.584193184440223   },
                new double[] {   -0.910702871536059, -0.539962110200158, -0.372942538995135, -0.241914067335765, -0.484148185468245, -0.589409315934428   },
                new double[] {   -0.910392529349647, -0.535217745814562, -0.388736377449023, -0.238775379290036, -0.480875525255205, -0.597666067440344   },
                new double[] {   -0.910330119395369, -0.536068434265457, -0.392643194950081, -0.239665695810985, -0.481175840105967, -0.599496681874508   },
                new double[] {   -0.903795352646719, -0.527784897380306, -0.39733495492408, -0.23910365365467, -0.480830212508504, -0.600700394166697   },
                new double[] {   -0.904784083939127, -0.52819672463037, -0.398137206679087, -0.239657950735831, -0.481663088791306, -0.602069725105439   },
                new double[] {   -0.906602894871905, -0.522725255813343, -0.399091122036865, -0.238217962601914, -0.481068238944123, -0.602462807622399   },
                new double[] {   -0.90552959671665, -0.520774511611745, -0.400695388879424, -0.237915184071582, -0.481001004992434, -0.603894474899885   },
                new double[] {   -0.903859149459035, -0.513767588201878, -0.40535412463515, -0.236965220461712, -0.480629084867433, -0.605755112565108   },
                new double[] {   -0.905148030199818, -0.513078914344105, -0.407492308363211, -0.237596812328161, -0.480891610484911, -0.606980775021889   },
                new double[] {   -1.0255926606909, -0.52171838743699, -0.362797164330491, -0.236859086827211, -0.479464554892558, -0.60876981880407   },
                new double[] {   -1.02686519172086, -0.52301958481552, -0.365784221716988, -0.23832043658083, -0.480156854431685, -0.610549244789236   },
                new double[] {   -1.02850526053723, -0.523405018615953, -0.367205793977022, -0.238932005696913, -0.48052060817329, -0.611634374904799   },
                new double[] {   -1.02935774503933, -0.522312199688553, -0.368534802261151, -0.239239118118912, -0.479796191982481, -0.612208783548573   },
                new double[] {   -1.02900454966656, -0.519548199609028, -0.372414394996566, -0.239947834337799, -0.475808796806167, -0.616549417556724   },
                new double[] {   -1.02886863844496, -0.51185771950698, -0.373043938043999, -0.245253652138224, -0.468363150677802, -0.616342487973042   },
                new double[] {   -1.02807548713829, -0.510043390007642, -0.374405068977444, -0.244874097029197, -0.467041561479762, -0.617335732227057   },
                new double[] {   -1.02688489688082, -0.509563425351445, -0.37713341834058, -0.242868216008101, -0.465912331198715, -0.618404977000026   },
                new double[] {   -1.02697660118859, -0.507341376079278, -0.378924241845202, -0.241440379078116, -0.466054434038443, -0.618946378170859   },
                new double[] {   -1.02808309841969, -0.506156532461147, -0.380835890854297, -0.24069140971121, -0.466015245848762, -0.619276513939016   },
                new double[] {   -1.02788328924312, -0.506079372417712, -0.382020730640115, -0.239944049718113, -0.465501282288041, -0.620178748536478   },
                new double[] {   -0.954855122267709, -0.503048799884092, -0.409076081849721, -0.245938722640227, -0.462421524338516, -0.619026836279003   },
                new double[] {   -0.933489402343861, -0.496502162044043, -0.424503433935457, -0.243853315904578, -0.461619546453055, -0.62076012991229   },
                new double[] {   -0.940757898475425, -0.499110133276087, -0.424115323900967, -0.24522256506418, -0.461772366223403, -0.621258972997566   },
                new double[] {   -0.941242806490049, -0.498738668528723, -0.424167974742123, -0.254689430690639, -0.464314930826952, -0.619772437954092   },
                new double[] {   -0.927776233886118, -0.495746778613493, -0.434067282449283, -0.251043797892951, -0.463044532969395, -0.621087349205987   },
#endregion
            },
            new double[][] // gesture 0
            {
                #region sequence
                new double[] {   1.83693249912303, -0.472608854432551, -0.361217031210459, 1.00843830742803, -0.447539851591357, -0.0521939050571813   },
                new double[] {   1.83716244351518, -0.47191974175817, -0.361071883732304, 1.00702227932266, -0.44678898450176, -0.0523100221038502   },
                new double[] {   1.8368131940214, -0.47241080908223, -0.360998504698197, 1.00887413954735, -0.445025806880815, -0.051695576926882   },
                new double[] {   1.83665885213551, -0.472448658491809, -0.36098254523635, 1.01038424771949, -0.444806946064462, -0.0518170645559464   },
                new double[] {   1.83555494395497, -0.472465233995507, -0.360902946052826, 1.00878371270746, -0.444575787178805, -0.0516899298344765   },
                new double[] {   1.83442313417456, -0.471889650816275, -0.361057566472531, 1.00915467284418, -0.442401247899697, -0.05224813932313   },
                new double[] {   1.83429474927218, -0.47123640935268, -0.365234278203742, 1.00879505722207, -0.443071844963396, -0.0542583072935921   },
                new double[] {   1.83297154237546, -0.471563698323051, -0.375778896423167, 1.00666144024136, -0.442826618767116, -0.0577505023485119   },
                new double[] {   1.82721145113657, -0.47130858996857, -0.407527026568549, 1.00359737009187, -0.439883730665643, -0.0676345474387445   },
                new double[] {   1.79608844017695, -0.450867864758149, -0.520333489190697, 0.996470626129841, -0.456547668928721, -0.116956604239414   },
                new double[] {   1.64069156902363, -0.415299288171011, -0.838064757159058, 0.933709739719477, -0.465375871100074, -0.241449913486909   },
                new double[] {   1.56704404362927, -0.394855072869398, -0.985548156248783, 0.899735407757183, -0.466783683991956, -0.326568895882804   },
                new double[] {   1.44351103046906, -0.37986410475332, -1.10178012714839, 0.847555100977272, -0.465624728081512, -0.380295823851738   },
                new double[] {   1.32931221541112, -0.364306504282257, -1.23094213758429, 0.788706344352215, -0.447846573749007, -0.416569569598153   },
                new double[] {   1.12358612230451, -0.341071174717691, -1.32157020674319, 0.760623353776633, -0.381449236122338, -0.563769951653079   },
                new double[] {   0.89939546641321, -0.322878538794563, -1.38956761151985, 0.72951403532235, -0.394136854415578, -0.775614192341608   },
                new double[] {   0.487775547845134, -0.350863829382161, -1.5070584364007, 0.65035097573189, -0.386671896564982, -0.62751695162814   },
                new double[] {   -0.215595665058761, -0.465343632117765, -1.55914207895624, 0.582218523262674, -0.328120320117002, -0.725588105813138   },
                new double[] {   -0.346276726687656, -0.467205225954852, -1.34376498587729, 0.351203475829538, -0.362495636207876, -0.797417225760677   },
                new double[] {   -0.605237178641826, -0.488691006365441, -1.24567907021941, 0.337075535986332, -0.359891752768797, -0.776445535788431   },
                new double[] {   -0.680431724859787, -0.509976662319833, -1.06396472088337, 0.309128609892378, -0.340200203087222, -0.740221980265351   },
                new double[] {   -0.884522594694474, -0.538804938267575, -0.664598583500497, 0.160468545775572, -0.371029885089445, -0.7188728308991   },
                new double[] {   -0.987505935395927, -0.555330977019373, -0.547860910971536, -0.0258374115169233, -0.45343953436702, -0.63556361623654   },
                new double[] {   -0.990045224262891, -0.55348177969015, -0.497937840691875, -0.0172074186286423, -0.459610870916301, -0.636438638148844   },
                new double[] {   -0.9857761746336, -0.557882287821778, -0.428907214366951, 0.107350136232095, -0.441098019644679, -0.630533601201039   },
                new double[] {   -0.980286011298116, -0.554055544556578, -0.423726534730789, 0.023218571718253, -0.450515810986719, -0.61853836820341   },
                new double[] {   -0.974330921411984, -0.550494843136668, -0.423709022372932, 0.0243053004170652, -0.449553667487048, -0.618083849094623   },
                new double[] {   -0.971232662789862, -0.548751873781336, -0.425300216174771, 0.0222502460322902, -0.450297977856979, -0.620692559288013   },
                new double[] {   -0.970065754323753, -0.546039310527534, -0.433260812647497, 0.0992255087767205, -0.406396804928127, -0.680301347402688   },
                new double[] {   -0.964485751944626, -0.543557450179018, -0.458957767177026, 0.0300944993383009, -0.457910272108831, -0.627116382707517   },
                new double[] {   -0.965182782326058, -0.542658393114308, -0.469937777564589, 0.0328679756042436, -0.455731911792743, -0.63058667566141   },
                new double[] {   -0.964331100219875, -0.543777944402283, -0.482384402603088, 0.033661731546584, -0.454837275012114, -0.631239603654917   },
                new double[] {   -0.963828613450293, -0.54387866277154, -0.49161714467317, 0.0609864251010074, -0.443264162978819, -0.63503770956204   },
                new double[] {   -0.928071184782701, -0.531461808934474, -0.529126313512955, 0.0564098975160823, -0.447444421934618, -0.636881772792905   },
                new double[] {   -0.927782444148225, -0.53393643293953, -0.534800442993008, 0.0501861202297594, -0.449985073980687, -0.63750637354753   },
                new double[] {   -0.933826228124211, -0.538777407049314, -0.540015267262065, 0.0791794799385945, -0.4160148166136, -0.646222811067455   },
                new double[] {   -0.927602878539924, -0.537078470507607, -0.560021602527319, 0.0514259966320764, -0.422516314182621, -0.659393245309332   },
                new double[] {   -0.904064634808152, -0.526918801909392, -0.575597385793559, 0.134346507079363, -0.390884912220126, -0.695175696612832   },
                new double[] {   -0.921146819563946, -0.526919675068527, -0.57304969793865, 0.132055363727719, -0.371630145020532, -0.708959956268348   },
                new double[] {   -0.92238628618226, -0.52109685518019, -0.580265958742416, 0.0518626526418991, -0.442263947169796, -0.656961906148189   },
                new double[] {   -0.925489340788311, -0.521571090701561, -0.587290797683535, 0.0291393556214983, -0.443454167161101, -0.66422044342435   },
                new double[] {   -0.940174291284657, -0.519059045441939, -0.592118320561516, 0.134188163780645, -0.359204713276485, -0.718141594861081   },
                new double[] {   -0.9522287132405, -0.518650314010239, -0.594840219062798, 0.0532409813009252, -0.447993117296391, -0.643075110348652   },
                new double[] {   -0.953746173472662, -0.518061246163143, -0.595668625798318, 0.154772138380501, -0.374521863798402, -0.718360293330311   },
                new double[] {   -0.955375813540822, -0.517362134448066, -0.597165452267288, 0.153301394508779, -0.373437989383605, -0.717693880498429   },
                new double[] {   -0.958049614904263, -0.516893979980473, -0.596825286911689, 0.150865487174001, -0.372073080345712, -0.715827044072708   },
                new double[] {   -0.96120613818445, -0.514330866503226, -0.597057494914068, 0.0486478313971428, -0.391830823106398, -0.657225338417496   },
                new double[] {   -0.962415899716692, -0.51297339699731, -0.597570945793402, 0.145071024010732, -0.372325084241105, -0.712456159045153   },
                new double[] {   -0.963493929406079, -0.511513215654961, -0.598966063239008, 0.154442631542815, -0.417105661750009, -0.66447157992031   },
                new double[] {   -0.968522224586578, -0.510482064533691, -0.602439787062632, 0.169739231735444, -0.38401663464456, -0.714998262437004   },
                new double[] {   -0.969361869388467, -0.508786929897984, -0.604156695934613, 0.167989429992085, -0.381326308011233, -0.71312980297181   },
                new double[] {   -0.968857628178037, -0.507841724531413, -0.605451160745135, 0.17628221750501, -0.381750088457408, -0.717683458745096   },
                new double[] {   -0.86960680130818, -0.487316185239472, -0.627897188969653, 0.176220692488876, -0.383835549204944, -0.717681229893216   },
                new double[] {   -0.871140036617996, -0.487663867845089, -0.630237901517305, 0.176628210180136, -0.385415923182474, -0.718334502245987   },
                new double[] {   -0.93336175600382, -0.498104211302941, -0.625551814370846, 0.172793322112157, -0.385501032810667, -0.715397387858403   },
#endregion
            },
            new double[][] // gesture 0
            {
                #region gesture
                new double[] {   1.89455683626509, -0.418753558655544, -0.410361427342023, 1.05139072610869, -0.417631338315134, -0.17028735510749   },
                new double[] {   1.90193378851809, -0.420074410052927, -0.420222980980532, 1.05172718830226, -0.417741631682848, -0.170773700087548   },
                new double[] {   1.90026938624568, -0.41694355085257, -0.456154315554865, 1.05045287954932, -0.415661390746359, -0.175461627607773   },
                new double[] {   1.80648035740344, -0.367674460009284, -0.742971970862091, 1.02056994162404, -0.394355580403405, -0.250870158932536   },
                new double[] {   1.71668936754118, -0.342120670954845, -0.895391882513275, 0.941475102882541, -0.383543376099638, -0.288049240874515   },
                new double[] {   1.62932490863542, -0.320446640789756, -1.0372945486486, 0.91868734592827, -0.37239620035925, -0.334245709319387   },
                new double[] {   1.52563810080375, -0.307424453580771, -1.1741715861557, 0.894228851267585, -0.375943552673663, -0.37450618225726   },
                new double[] {   1.3883707110936, -0.291230785138021, -1.30397813344113, 0.86412086242786, -0.370836972005937, -0.469557997713548   },
                new double[] {   1.22220508591983, -0.273345463911353, -1.39700420874574, 0.824570308314802, -0.377155055103815, -0.462379162681511   },
                new double[] {   0.418515662685165, -0.275237659098948, -1.60329218213574, 0.53721088787122, -0.469778354047239, -0.874491664795876   },
                new double[] {   0.0925210191787482, -0.277238916871387, -1.93463453938844, 0.547843828995278, -0.435260748290424, -0.892243292806033   },
                new double[] {   0.0246469863557982, -0.476466801023412, -1.51753530379255, 0.50409382729638, -0.312378406351647, -0.807415532228311   },
                new double[] {   -0.1424722791752, -0.353114891782043, -1.54109457546963, 0.456325564838798, -0.299444597196726, -0.767231895688783   },
                new double[] {   -0.31094773484354, -0.373650593637625, -1.45787305319423, 0.409413163094992, -0.309958461693771, -0.758493208059621   },
                new double[] {   -0.469680656857728, -0.375214761608655, -1.39479549871458, 0.303616970572645, -0.352349247732243, -0.789054598179063   },
                new double[] {   -0.598163924112781, -0.402509878974138, -1.3060997309773, 0.26151995101408, -0.396224292705507, -0.770447652797584   },
                new double[] {   -0.690715299104666, -0.432059091611156, -1.2145455030753, 0.258025274448002, -0.372024995444207, -0.74622120083919   },
                new double[] {   -0.710300178280161, -0.460744728668067, -1.07172520950883, 0.23909379629884, -0.361321505618302, -0.73488526669869   },
                new double[] {   -0.769345049985565, -0.45997284650877, -0.983677582003735, 0.228237167619772, -0.354177394949352, -0.719447030079939   },
                new double[] {   -0.792177080570086, -0.467451467443402, -0.888461340199305, 0.215096383615529, -0.355828199952667, -0.712981400790469   },
                new double[] {   -1.00214300058299, -0.491862283964432, -0.674707028952127, 0.051939936848828, -0.372025722868765, -0.654883067982522   },
                new double[] {   -1.00144686448377, -0.493029394656674, -0.660701345519758, 0.0830655019502978, -0.346952987917151, -0.651594409058008   },
                new double[] {   -1.00097769139644, -0.489083278811363, -0.648099721362419, 0.083023788015324, -0.352579153094426, -0.65745986214186   },
                new double[] {   -0.974806498406826, -0.489079266506256, -0.64509181938141, 0.115516517026128, -0.375243630478671, -0.673638813282271   },
                new double[] {   -0.964956928783193, -0.487190026078187, -0.645456411229327, 0.113665795892624, -0.388667959094118, -0.65593544019296   },
                new double[] {   -0.919925518393967, -0.48385458480248, -0.652619638697809, 0.124849748931869, -0.384117399866297, -0.672348662989241   },
                new double[] {   -0.946986830658933, -0.485719928362335, -0.668010747319594, 0.132577730143711, -0.379618964060522, -0.681747257941197   },
                new double[] {   -0.938717345050546, -0.483659207735279, -0.673755354972792, 0.120011367815471, -0.390760051740061, -0.65728452519312   },
                new double[] {   -0.935984517546255, -0.482586700438365, -0.676814191862139, 0.120079079802516, -0.394832904140794, -0.652120114033769   },
                new double[] {   -0.952958846148683, -0.481611653406225, -0.680037886849736, 0.118947888093851, -0.394322807296626, -0.652064735018251   },
                new double[] {   -0.946686607097515, -0.480335975563041, -0.682173402955088, 0.117799220501175, -0.396143253574054, -0.648411927016145   },
                new double[] {   -0.943285022549379, -0.480233843101331, -0.683149316932587, 0.111661797091595, -0.379660557774939, -0.662208759499102   },
                new double[] {   -0.941760520382705, -0.479371226301683, -0.683775710620384, 0.1153316291997, -0.3857428066977, -0.660858045952742   },
                new double[] {   -0.948160655108299, -0.474589878904018, -0.691104498265311, -0.0496286625621961, -0.359631040985771, -0.697764736576365   },
                new double[] {   -0.941172713059567, -0.470453924461423, -0.694858655186672, 0.147080067729176, -0.371761130683661, -0.702210336894401   },
                new double[] {   -0.955850652864031, -0.466862351029269, -0.696363136402233, -0.0565359400776716, -0.355886910982209, -0.704901549944897   },
                new double[] {   -0.950453838421678, -0.465483654710509, -0.699225066980537, -0.0566737014434251, -0.356500356878563, -0.704918933239278   },
                new double[] {   -0.94720274986498, -0.464744129455941, -0.700233451412116, -0.0558688712395958, -0.356372786841427, -0.70476597366545   },
                new double[] {   -0.960534467895102, -0.466419911481758, -0.702883891289517, -0.0565645642988323, -0.357188635112458, -0.705363651476062   },
                new double[] {   -0.952013350482399, -0.464345345430103, -0.705929477025851, -0.0549372091494358, -0.362398507312576, -0.704586576465677   },
                new double[] {   -0.946429469799482, -0.462009673823556, -0.705744307760694, 0.114425699669972, -0.356345783280439, -0.68876700823126   },
                new double[] {   -0.956488761942495, -0.458886684351511, -0.705368458560783, 0.000575056773888229, -0.410221607327182, -0.672137181458938   },
                new double[] {   -0.96622666117929, -0.454641477452084, -0.703023598033086, 0.104371650586298, -0.393261880713357, -0.666340687486207   },
                new double[] {   -0.949194314362968, -0.453700920641736, -0.70611415524476, -0.0156166675172164, -0.37223496686808, -0.683589742861608   },
                new double[] {   -0.94239407303153, -0.453739217306082, -0.706589436822577, -0.0294199752012577, -0.376676363008826, -0.685440919157965   },
                new double[] {   -0.93491523990071, -0.452160281698528, -0.706574199437884, -0.030596633267181, -0.375104377101595, -0.686009788822754   },
                new double[] {   -0.934987582144981, -0.452531048076525, -0.707181094671732, -0.0348958515921956, -0.374487715693462, -0.687191394690241   },
                new double[] {   -0.934581737875745, -0.452111532093404, -0.707290952283328, 0.0652424781980971, -0.406204688464211, -0.661870929992292   },
                new double[] {   -0.861933440484331, -0.455423266514914, -0.716886762189883, 0.0309513140437829, -0.398090231005882, -0.674793896831915   },
                new double[] {   -0.962001952943325, -0.448890288243682, -0.704158706188084, 0.0517758219885637, -0.399490349430017, -0.673765490185799   },
                new double[] {   -0.94838400066881, -0.448123292765554, -0.707144084824051, 0.0799070631540134, -0.400228314452365, -0.665124646240487   },
                new double[] {   -0.933440170528267, -0.447827017396187, -0.711234259594165, 0.0813716513288882, -0.399911739442851, -0.66612326649928   },
                new double[] {   -0.942765064631763, -0.44498621798774, -0.711130374098011, 0.122564076188503, -0.39003088703369, -0.664197133725458   },
                new double[] {   -0.930501799028152, -0.444068452563294, -0.71217027201062, -0.0466965465765701, -0.367636291151374, -0.690894440813067   },
                new double[] {   -0.951387575708864, -0.443075596767206, -0.708104277987458, -0.0511456095153162, -0.345085740369975, -0.69154130688982   },
#endregion
            },
            new double[][] // gesture 0
            {
                #region sequence
                new double[] {   1.81039746638912, -0.329993263438475, -0.478202228974957, 0.985513326660156, -0.398607588378203, -0.14218226056278   },
                new double[] {   1.76019127762543, -0.33062339939884, -0.561142146137987, 0.972538156506926, -0.397368026821941, -0.170688777426528   },
                new double[] {   1.65465817428832, -0.302325056730812, -0.787810196583759, 0.915559840893151, -0.424788463437848, -0.292594804755651   },
                new double[] {   1.56864698904641, -0.299889666055381, -0.902280245909527, 0.876615065745461, -0.422112008600472, -0.331907009048492   },
                new double[] {   1.48875425951138, -0.272456244022748, -1.0260148226825, 0.838622800547463, -0.427496431867428, -0.381039924329519   },
                new double[] {   1.38753940463716, -0.270823116147052, -1.15654076607346, 0.767970866245096, -0.422964807184901, -0.461945232967202   },
                new double[] {   1.04966106323252, -0.249372150085184, -1.35704490806424, 0.768245172304526, -0.368760673783894, -0.766695394394875   },
                new double[] {   0.852351360480833, -0.247706926718312, -1.41910434053969, 0.664381473265511, -0.360260776642751, -0.846503073349196   },
                new double[] {   0.685940687645199, -0.210389827309349, -1.7211288712141, 0.575306027769461, -0.367722281371065, -0.881403110866596   },
                new double[] {   0.49042050299064, -0.236842240928827, -1.52862592219976, 0.533539223801297, -0.365780867151083, -0.905819769823583   },
                new double[] {   -0.149782235523028, -0.315285850659095, -1.59355137583924, 0.350030173494973, -0.403044986230809, -0.878767214367829   },
                new double[] {   -0.376792488364135, -0.416495117942311, -1.41140947409903, 0.300205349152219, -0.397452520992553, -0.866076996592593   },
                new double[] {   -0.376658447008217, -0.472609612369745, -1.26258690624468, 0.297815606654603, -0.341505198485938, -0.741182894016732   },
                new double[] {   -0.543927543129837, -0.451039314392606, -1.15946245015059, 0.270151863331771, -0.346262044261479, -0.738410257114763   },
                new double[] {   -0.762452983646129, -0.468445494728813, -1.06699486263023, 0.24155512970831, -0.352741402129332, -0.729955585077136   },
                new double[] {   -0.933459962609862, -0.489183465800257, -0.95215685374777, 0.162691444071592, -0.344581167415752, -0.717303511687405   },
                new double[] {   -1.03985375810754, -0.48343572681944, -0.829906700615471, 0.147880063101928, -0.360618715995892, -0.724886047684413   },
                new double[] {   -1.05661537807442, -0.516108200028089, -0.595772076028504, 0.070740339202642, -0.312876270321103, -0.713611385741961   },
                new double[] {   -1.04733429586695, -0.517876095808304, -0.567424448766704, 0.0710801705808297, -0.31606599547986, -0.718285164524205   },
                new double[] {   -1.03679170936321, -0.503405240439483, -0.542780170378779, 0.0827657695474092, -0.326298852013148, -0.709736049749699   },
                new double[] {   -1.0517622958724, -0.471183506660116, -0.558798776293505, 0.120431031204365, -0.430775925342275, -0.63996844071837   },
                new double[] {   -1.03092209441232, -0.51451048382893, -0.528394642630866, 0.063256774169547, -0.295755098788545, -0.718323495970731   },
                new double[] {   -1.03666044701613, -0.505685300345208, -0.523990688250571, 0.0848948607513546, -0.324319528901618, -0.730721550883562   },
                new double[] {   -1.03387696335086, -0.489434210420209, -0.553200403353329, 0.113362563644969, -0.361379106431109, -0.704692028702259   },
                new double[] {   -1.03137813773012, -0.484603377730078, -0.560945331519837, 0.117738260788613, -0.363127260369693, -0.704765578178295   },
                new double[] {   -1.02822614915527, -0.472570445705764, -0.573075806396293, 0.134300155703843, -0.386175217287915, -0.69024043903213   },
                new double[] {   -1.02780764496789, -0.47004441638919, -0.579868825366108, 0.13427297746091, -0.388719511702022, -0.682539754575502   },
                new double[] {   -1.02587517757991, -0.474443814765937, -0.586274704282371, 0.128244974905803, -0.377423864522074, -0.688053339625063   },
                new double[] {   -1.02595421910257, -0.475817353740666, -0.594798909243656, 0.13560307627125, -0.378537128976002, -0.692123486833324   },
                new double[] {   -0.991758440997551, -0.48588782412764, -0.616817278749097, 0.129465275951911, -0.376563505828528, -0.691882006422893   },
                new double[] {   -1.00198465769423, -0.479259830731894, -0.625690964434839, 0.129648519840828, -0.382458484095359, -0.683030137680593   },
                new double[] {   -1.00052310227285, -0.479908964474454, -0.62539386062556, 0.127053080155167, -0.376048387304453, -0.686745190635571   },
                new double[] {   -0.999839931032392, -0.479137768178696, -0.625906301151506, 0.125590982776221, -0.368864948003283, -0.68965403163628   },
                new double[] {   -1.00131195859275, -0.480060487437313, -0.628402595785863, 0.123977518436431, -0.361915119481247, -0.691688945645548   },
                new double[] {   -0.986705448120446, -0.47168445734222, -0.628103600285529, 0.171720959987281, -0.375600626048208, -0.711968917433952   },
                new double[] {   -0.985180459450819, -0.470135067047787, -0.631290558802254, 0.17325488703814, -0.375231967034803, -0.713170504478024   },
                new double[] {   -1.00640476908435, -0.474593720084946, -0.6351120147601, 0.152698664242254, -0.369412047738043, -0.703196064318024   },
                new double[] {   -1.00823898506461, -0.474466843964183, -0.638779380968259, 0.14353646241826, -0.36868251623201, -0.697000498969252   },
                new double[] {   -1.00994495614105, -0.473661867050909, -0.642428642392034, 0.150928469820912, -0.370011235982928, -0.700213590955044   },
                new double[] {   -0.98684869194021, -0.467128669809874, -0.645513294888318, 0.174311386247693, -0.379051361164221, -0.706858174136523   },
                new double[] {   -0.979507243595792, -0.471350687889677, -0.642583409439783, 0.177496583062163, -0.361855402199924, -0.723949750649723   },
                new double[] {   -0.996531601436393, -0.476748400989022, -0.641915216792797, 0.161416538466906, -0.352468962254835, -0.724214209811448   },
                new double[] {   -0.997154472755842, -0.476530725442069, -0.643414503528077, 0.162000453404009, -0.354690099392019, -0.722262826959322   },
                new double[] {   -0.986741565438, -0.475329896121946, -0.642738441042417, 0.170926921767929, -0.352113280307902, -0.730626644195694   },
                new double[] {   -1.00656583374722, -0.473902539316773, -0.655598495286054, 0.12630013570929, -0.363832234473607, -0.68495057000387   },
                new double[] {   -1.0037489190822, -0.478202486746738, -0.65508893314498, 0.117485309525988, -0.352170600707507, -0.689015765083749   },
                new double[] {   -1.00553990674799, -0.479429202219119, -0.65236324171744, 0.119488828219287, -0.340666713617778, -0.696811974613894   },
                new double[] {   -1.00679232170519, -0.478188209560773, -0.654381472897022, 0.12089434627031, -0.343443332091958, -0.69553130562478   },
                new double[] {   -0.997161157977448, -0.468828205158612, -0.653623254851453, 0.160825302847773, -0.363535047475043, -0.706717761203364   },
                new double[] {   -1.01244651811234, -0.476211854861748, -0.654903361486754, 0.130776254855693, -0.34625828193111, -0.699528868332007   },
                new double[] {   -1.01378470936778, -0.459628472660414, -0.664164910849322, 0.132789039283516, -0.39069407606588, -0.679742066283898   },
                new double[] {   -1.01744678753277, -0.458474514725064, -0.661233295283402, 0.146422933167196, -0.387033879037338, -0.692194397360916   },
                new double[] {   -0.985862703595253, -0.454024946839723, -0.661155000355173, 0.182664760757853, -0.390655107482723, -0.71267065221645   },
                new double[] {   -1.00875500845051, -0.469055332165193, -0.663198491953554, 0.128260790212471, -0.349294788620521, -0.709690661244778   },
                new double[] {   -0.990063570186365, -0.474565377857891, -0.676177897094039, 0.0728452429641779, -0.330787059313887, -0.675803837496233   },
#endregion
            },

            new double[][] // gesture 1
            {
                #region sequence
                new double[] {   -0.457141558961931, -0.40632533154858, -0.735627349411788, 0.535630266102316, -0.346827950566122, -0.650236844528936   },
                new double[] {   -0.456071961790413, -0.409714955952653, -0.726524609508806, 0.535891032024648, -0.346643275206004, -0.648562468729008   },
                new double[] {   -0.456603392782934, -0.40612856011685, -0.718937097397445, 0.535278649216686, -0.346096775156327, -0.646807416293433   },
                new double[] {   -0.456195818726435, -0.405733234396067, -0.716619341645088, 0.537414102686565, -0.345148724938313, -0.644083712970447   },
                new double[] {   -0.456108219846857, -0.405055328145683, -0.716893982157323, 0.538993106624869, -0.343352950949183, -0.641020954278633   },
                new double[] {   -0.456117172115349, -0.404966150793507, -0.713597542331052, 0.539094945650424, -0.343580052414518, -0.641870410198698   },
                new double[] {   -0.456256366681632, -0.403942809658007, -0.712183562120758, 0.539117142669744, -0.343600182125899, -0.640827187777538   },
                new double[] {   -0.456318450962927, -0.404059470017929, -0.712023385265101, 0.539335744216375, -0.343330010954969, -0.640557928227373   },
                new double[] {   -0.496415742710127, -0.435930827089642, -0.842628571333189, 0.54012431482377, -0.340727424375059, -0.656223943674453   },
                new double[] {   -0.47046969206073, -0.445080332345827, -0.940379909545808, 0.54242263674365, -0.341026423628752, -0.665346327913693   },
                new double[] {   -0.404677316088423, -0.407212289430084, -1.09978927457519, 0.548926996500703, -0.336123408877271, -0.715567799729486   },
                new double[] {   -0.270133624688208, -0.398971108822503, -1.18186612923659, 0.581462608554204, -0.339939033451459, -0.728289697185392   },
                new double[] {   0.00177229092348332, -0.36195823936821, -1.40259786149438, 0.713928674521608, -0.299378131481296, -0.723163602209742   },
                new double[] {   0.438549670704743, -0.306664268663078, -1.66590607972288, 0.826072529708719, -0.288848429899672, -0.541535669916923   },
                new double[] {   0.80570292397565, -0.264655719834869, -1.33467732081479, 0.870692075895112, -0.379356031621558, -0.36093124767606   },
                new double[] {   1.00877757565257, -0.241624437507684, -1.38072303107421, 0.886187578931378, -0.343860756328378, -0.557513388235001   },
                new double[] {   1.22170213343089, -0.237897053738726, -1.3709597901228, 0.89171690778897, -0.341013798585059, -0.537198441407618   },
                new double[] {   1.41000647027091, -0.249498172756101, -1.30725259066176, 0.958269109598335, -0.344897675114607, -0.510737106517556   },
                new double[] {   1.58531963858677, -0.27108936164505, -1.19668906913541, 1.0419386196075, -0.365080545365751, -0.454926878844911   },
                new double[] {   1.82074585803987, -0.317679828600331, -0.923186049144346, 1.09551478573526, -0.352005201663378, -0.32215085120108   },
                new double[] {   1.88436571532342, -0.36047803896289, -0.71584920275508, 1.11315299509139, -0.375259870705983, -0.214146263190284   },
                new double[] {   1.90751637379707, -0.386683312954617, -0.611329783603259, 1.11419483684668, -0.38536894339752, -0.158490756238556   },
                new double[] {   1.8914779913182, -0.398852223648961, -0.567831637251282, 1.1034542991384, -0.387261155238522, -0.135129898287414   },
                new double[] {   1.88115214579454, -0.400094465827132, -0.558818760492937, 1.09966964508344, -0.386406296731806, -0.127826994277555   },
                new double[] {   1.86916635160405, -0.395459585597675, -0.552673536845899, 1.09564266866442, -0.385055028312389, -0.120456890618612   },
                new double[] {   1.86630368559614, -0.395863684336918, -0.552388621544454, 1.09389704858306, -0.385822929693176, -0.11906391001509   },
                new double[] {   1.86411827201658, -0.394502256605027, -0.552299449570839, 1.09325255762299, -0.385586891345116, -0.117995023494199   },
                new double[] {   1.86161897116387, -0.392970419240885, -0.551818418420905, 1.09117828614238, -0.385044438614618, -0.116734875513523   },
                new double[] {   1.85739170018871, -0.390768017847194, -0.551607170232083, 1.08992494303136, -0.383908191517361, -0.11635609786498   },
                new double[] {   1.85598066713726, -0.390734593125353, -0.552315023203081, 1.08931283928082, -0.38469236980134, -0.116485875560717   },
                new double[] {   1.85449550898546, -0.388844365123725, -0.553045443300943, 1.08962267014303, -0.384326431212267, -0.115848666680189   },
                new double[] {   1.85338663129397, -0.388161290828275, -0.554051284583098, 1.08890250036731, -0.384809002179466, -0.115824594001016   },
                new double[] {   1.8520347839392, -0.38868336663798, -0.558981491068113, 1.08912145615182, -0.385863702220597, -0.117162657659841   },
                new double[] {   1.84693871953876, -0.389953556575679, -0.562915233273519, 1.08476198266489, -0.387286565429475, -0.118803768593796   },
                new double[] {   1.84197393528843, -0.391036739298336, -0.566121785915048, 1.07775556567245, -0.397774316734383, -0.119706310865984   },
                new double[] {   1.83997626692841, -0.390549168449589, -0.56966121735625, 1.07765204762472, -0.397417143971034, -0.120719284876998   },
                new double[] {   1.83889376609092, -0.390030744714671, -0.57231957502666, 1.0769316198447, -0.397633716568192, -0.121731087763915   },
                new double[] {   1.83846202320877, -0.389913689646706, -0.575200605209555, 1.07676104640471, -0.397422636341243, -0.122185706990529   },
                new double[] {   1.83364910153588, -0.390516219906003, -0.579661292534311, 1.07350441858398, -0.396716379246846, -0.125166893603728   },
                new double[] {   1.83321717295502, -0.390446457313877, -0.5809408774325, 1.07326432120588, -0.396444059722583, -0.125559348466435   },
                new double[] {   1.83483906184715, -0.391162418408862, -0.582147415446547, 1.07407968093878, -0.397132314951963, -0.126171791677912   },
                new double[] {   1.83373009268665, -0.391287914680032, -0.583315908367262, 1.07368980110028, -0.397273611643872, -0.126946535639465   },
                new double[] {   1.83357078390028, -0.391514554703468, -0.583750760998393, 1.07406210800118, -0.397305431985301, -0.12764475819407   },
                new double[] {   1.83347056065606, -0.391759484642659, -0.584672541027804, 1.0735637920197, -0.398078091722596, -0.12800179406953   },
                new double[] {   1.83256454537436, -0.391832661379448, -0.58570711622771, 1.0730913970304, -0.398383751272588, -0.128589258650933   },
                new double[] {   1.83177457614257, -0.391683460074262, -0.585704532007366, 1.07191646849035, -0.398385700138894, -0.128971933898329   },
                new double[] {   1.83115618580524, -0.391457727878081, -0.586438190198238, 1.07118579519366, -0.398490122592621, -0.129517607336715   },
                new double[] {   1.83083050397421, -0.390417438003364, -0.587821562939945, 1.070662331901, -0.398305006643411, -0.130054637772058   },
                new double[] {   1.83073801533248, -0.387394904557115, -0.588709205075223, 1.07097140811707, -0.398577853518581, -0.131020010679511   },
                new double[] {   1.82958247462732, -0.385925935069183, -0.589062134159373, 1.0710557585688, -0.398042953218518, -0.13099375765123   },
                new double[] {   1.82686754843031, -0.383647186985985, -0.588609774125495, 1.07069163345956, -0.397716617323994, -0.131204395950237   },
                new double[] {   1.82682584500103, -0.382671091657007, -0.589092237632434, 1.07076777070932, -0.397723533155631, -0.131291372181642   },
                new double[] {   1.82554942165576, -0.381361211540481, -0.589291073333941, 1.07001640411405, -0.397008357978967, -0.131018806166192   },
                new double[] {   1.82543327677441, -0.381036349659362, -0.590189983728293, 1.07014560532559, -0.396809101167699, -0.131255796139038   },
                new double[] {   1.82538375558199, -0.380439362667942, -0.590867710924926, 1.0703911840899, -0.39671413611577, -0.131519871512601   },
#endregion
            },
            new double[][] // gesture 1
            {
                #region sequence
            new double[] {   -0.489801149161806, -0.435182232082444, -0.69958300997751, 0.544282523803138, -0.350114357080994, -0.664027770276546   },
            new double[] {   -0.490925548298537, -0.43220227275968, -0.700816453327263, 0.544024945795574, -0.348261323819772, -0.662829366433597   },
            new double[] {   -0.322301663994229, -0.407675475433039, -0.763017292054366, 0.545150343764277, -0.34967680502357, -0.661825969078828   },
            new double[] {   -0.481003256780778, -0.423237682260104, -0.752539207272433, 0.545379556048623, -0.349340951970279, -0.662147996250578   },
            new double[] {   -0.329122381507093, -0.419589568771028, -0.837154293497481, 0.531117490242325, -0.345260238824062, -0.676303093444909   },
            new double[] {   -0.300490024519226, -0.387872498109404, -0.916894328015263, 0.550778290832483, -0.352605893043472, -0.663981257126488   },
            new double[] {   -0.424921130280732, -0.35549386675959, -1.01802774298489, 0.554034929236072, -0.34298660115811, -0.686709712866159   },
            new double[] {   -0.314413626698373, -0.347093765345432, -1.14560757816325, 0.542581128732051, -0.34639916221197, -0.739934824927726   },
            new double[] {   -0.108383464770433, -0.294220516648368, -1.3921748386204, 0.554205472706091, -0.369285927026099, -0.749540275709648   },
            new double[] {   0.2706324419238, -0.25749035200793, -1.52884998195574, 0.816759402528711, -0.354805530259797, -0.760487804464397   },
            new double[] {   0.534534069923105, -0.223360984508078, -1.49059755380105, 0.811562440149598, -0.337976115912154, -0.756597615659226   },
            new double[] {   0.713148090272881, -0.258790781004112, -1.40575001765786, 0.820451972711219, -0.305219806836246, -0.762226137900717   },
            new double[] {   0.902802517314591, -0.192087879812596, -1.39436230029754, 0.860325129367102, -0.31143804420044, -0.601624949251728   },
            new double[] {   1.15495073685015, -0.171988806153135, -1.39025800094205, 0.881199487025633, -0.326837329685032, -0.524494338003884   },
            new double[] {   1.41050485401999, -0.177110761634213, -1.33578314106953, 0.973250769915801, -0.324184986109746, -0.516957668077631   },
            new double[] {   1.60992125343556, -0.179222710031664, -1.1933412254846, 1.06046434835688, -0.328399924806808, -0.473878248975515   },
            new double[] {   1.77768796176505, -0.197981556124187, -1.01376410374604, 1.09090125121382, -0.324310437439087, -0.36270964203242   },
            new double[] {   1.88668640440291, -0.247271722251506, -0.817693620258559, 1.11138193093905, -0.327504693226654, -0.302935208343556   },
            new double[] {   1.92474136596357, -0.286718154413123, -0.645328025801845, 1.12788717555507, -0.338844528567872, -0.227484557070479   },
            new double[] {   1.97717396944805, -0.33071146422337, -0.489225547078566, 1.13261299873562, -0.344866783778165, -0.15466445101311   },
            new double[] {   1.95143601532216, -0.351826663461255, -0.435765487191031, 1.12953988681325, -0.348948986191872, -0.120013914089056   },
            new double[] {   1.93455503740166, -0.353854716964098, -0.430773836434618, 1.12135822867619, -0.350651667226568, -0.112593335809211   },
            new double[] {   1.92455761685161, -0.353307646903339, -0.431227548729918, 1.11735156431028, -0.351614382174506, -0.108730780545278   },
            new double[] {   1.91728235204661, -0.350499157346214, -0.433838619569045, 1.11044376747197, -0.350908295137689, -0.108049919431556   },
            new double[] {   1.91572892209477, -0.349269375645993, -0.438803293510545, 1.10842272093237, -0.352703172940995, -0.107229503880901   },
            new double[] {   1.91046385742085, -0.348589311350062, -0.444168855647911, 1.10885983462811, -0.354490374905328, -0.106417716531827   },
            new double[] {   1.90188279731983, -0.343816581369453, -0.451471746729866, 1.10376878005962, -0.352924517101407, -0.10717312319307   },
            new double[] {   1.89689387894297, -0.342986075462353, -0.457711266385815, 1.10010254976143, -0.357366975406021, -0.107394437822759   },
            new double[] {   1.89267150280091, -0.342609624072512, -0.4624188817312, 1.09698487378906, -0.361060336748603, -0.107452803682314   },
            new double[] {   1.88019601022517, -0.33830769055876, -0.473267926643933, 1.08778398099801, -0.367488758430119, -0.10914454960924   },
            new double[] {   1.87789981266227, -0.336982139440278, -0.47512910633156, 1.08581387317738, -0.368662203642789, -0.11006593291624   },
            new double[] {   1.87343822892477, -0.334478080102878, -0.482302947463474, 1.08356342357864, -0.370030066039359, -0.110519407138547   },
            new double[] {   1.87582404154971, -0.335118458275004, -0.485694324122023, 1.08536517551115, -0.3688305884859, -0.111852305454756   },
            new double[] {   1.87811314231628, -0.335786490265598, -0.489093517819354, 1.08672936787591, -0.371770925107116, -0.111954239991606   },
            new double[] {   1.87674035204692, -0.334908032968335, -0.49055748408565, 1.0850886701376, -0.372604193251155, -0.113281539010851   },
            new double[] {   1.87165944969706, -0.333554702686491, -0.492650737236797, 1.08228994271406, -0.373306941198374, -0.113384140229457   },
            new double[] {   1.86784595017503, -0.331337100315497, -0.493471852181188, 1.07962398214774, -0.376205630478149, -0.110720673373595   },
            new double[] {   1.86554863929289, -0.329734337711775, -0.493214372253281, 1.07715071312796, -0.3772644355269, -0.108342847034826   },
            new double[] {   1.86253663370994, -0.328461119081429, -0.494393499481165, 1.07583060247471, -0.378913705803596, -0.107784464179537   },
            new double[] {   1.85983649267991, -0.326651927459992, -0.495634974821925, 1.07441687996992, -0.380030644403494, -0.106788889831527   },
            new double[] {   1.8591200927998, -0.326326817561859, -0.497326719572548, 1.07412766095891, -0.380111671371787, -0.107270682466607   },
            new double[] {   1.86010526695314, -0.324765208301082, -0.500727522536593, 1.07602651102221, -0.381379012787859, -0.109800811834504   },
            new double[] {   1.85824362363659, -0.32366453733985, -0.501572917967254, 1.07548863083419, -0.381093436932352, -0.1083236512991   },
            new double[] {   1.85693305484916, -0.322708201766606, -0.503070944789193, 1.07507295191517, -0.380963624783903, -0.109928908334175   },
            new double[] {   1.85688555011961, -0.32188126412221, -0.504152619862991, 1.07548455180966, -0.380539195253966, -0.111764549835625   },
            new double[] {   1.85520026565926, -0.320978374713013, -0.505052840656038, 1.07571896331439, -0.375226214970183, -0.114764484846522   },
            new double[] {   1.8523043909503, -0.320192685800293, -0.506385441214446, 1.07396748348163, -0.376369722559288, -0.114158622187013   },
            new double[] {   1.85221228959717, -0.319741360176631, -0.508529909714418, 1.07434244018574, -0.376845033042973, -0.11512604177611   },
            new double[] {   1.85098332877362, -0.31920669036825, -0.510332549608925, 1.074997236208, -0.374190509881891, -0.116913817200841   },
            new double[] {   1.84812488679266, -0.317725365664887, -0.515443605774018, 1.07503873479496, -0.371506710495313, -0.119243403438253   },
            new double[] {   1.84630077275393, -0.315505847067416, -0.525476809838314, 1.07855005492644, -0.365738335286963, -0.12371683137939   },
            new double[] {   1.84686689192806, -0.314193963081406, -0.528734677744107, 1.07910071803825, -0.364140853337704, -0.12530087577759   },
            new double[] {   1.84467769137288, -0.313897813309732, -0.531472527453272, 1.07838910327824, -0.364953686101647, -0.126090934245215   },
            new double[] {   1.84269421953036, -0.313063923589047, -0.532789229838071, 1.07832688613465, -0.362829063476232, -0.127267778784536   },
            new double[] {   1.84275069415317, -0.312174367121606, -0.534246528411556, 1.07944228516685, -0.361655549147256, -0.128250264923513   },
#endregion
            },
            new double[][] // gesture 1
            { 
                #region sequence
            new double[] {   -0.576631881789471, -0.393931720024426, -0.855029516776733, 0.379653432691902, -0.303807643252035, -0.843460633929   },
            new double[] {   -0.575852205635844, -0.389129131950642, -0.849660518741581, 0.380709083643687, -0.300654266662638, -0.8379317146046   },
            new double[] {   -0.573684468934919, -0.385923094478572, -0.847773956806995, 0.380834373262598, -0.297599183972112, -0.835363076753102   },
            new double[] {   -0.573556842780767, -0.389333426350308, -0.849229577903538, 0.382530452845256, -0.300819787055132, -0.83689167813096   },
            new double[] {   -0.573854543602696, -0.388499648185118, -0.84980649644167, 0.382180508518151, -0.29965037899999, -0.83676161203556   },
            new double[] {   -0.576715308548419, -0.384662687379599, -0.860484287093286, 0.378985079426265, -0.29640496524419, -0.838902123645525   },
            new double[] {   -0.588900711470768, -0.385266885532297, -0.923726583976011, 0.379267313726872, -0.30268573220621, -0.846573995477996   },
            new double[] {   -0.579526886275638, -0.385065718460342, -1.02515332828764, 0.363286872995528, -0.307475864538918, -0.873472018529689   },
            new double[] {   -0.555707868262369, -0.391825804002964, -1.16473916795364, 0.383813430940845, -0.3134839788396, -0.883441214345015   },
            new double[] {   -0.18071019488585, -0.27201590686453, -1.3855310035729, 0.697537878184434, -0.213372266138134, -0.777728595441673   },
            new double[] {   0.558902927867495, -0.143734536791717, -1.6185916206565, 0.862882546080203, -0.385508612812491, -0.367132147716302   },
            new double[] {   0.833143833635818, -0.189742179629685, -1.26443215210358, 0.880282682024249, -0.408365621171906, -0.195831125739087   },
            new double[] {   1.04263263988733, -0.121574619081287, -1.37072798921971, 0.911546565256996, -0.269708930532971, -0.469439374853857   },
            new double[] {   1.31980981378162, -0.123892588600308, -1.35633761399808, 0.944213110642746, -0.25104504001932, -0.48059193879092   },
            new double[] {   1.57943578415441, -0.110575468529706, -1.31365803069772, 1.11262168069149, -0.256695512020526, -0.5349739106025   },
            new double[] {   1.78011331549031, -0.135167791737735, -1.13022004737373, 1.12037380784346, -0.254315642129768, -0.408705729218384   },
            new double[] {   1.90551972300119, -0.169237096605296, -0.899631234679595, 1.14450795451353, -0.257523470897432, -0.325732741955414   },
            new double[] {   1.98945204974713, -0.192916570528212, -0.705692024462944, 1.18561431620876, -0.262737801209285, -0.252701141091082   },
            new double[] {   2.02431823927712, -0.254318808607701, -0.354140027011933, 1.17760375807929, -0.272327101635009, -0.0927504664968162   },
            new double[] {   2.0070438230488, -0.264867311791823, -0.321502000295648, 1.16833992785035, -0.275021840628271, -0.0703752564722967   },
            new double[] {   2.00193069285507, -0.273215620388046, -0.306458103428713, 1.17351080911579, -0.276874978851821, -0.0567655933840491   },
            new double[] {   1.97298234448375, -0.281235690157977, -0.301955685450132, 1.15773820128454, -0.276877445854644, -0.0541260508076733   },
            new double[] {   1.94235567284447, -0.292726506749575, -0.315378255148418, 1.14714721120784, -0.283450027936232, -0.0493111329984909   },
            new double[] {   1.93696009316083, -0.294898974541785, -0.325370642776291, 1.14496938261187, -0.283260158513866, -0.05009833017979   },
            new double[] {   1.9287464018115, -0.301312535916633, -0.333123577488315, 1.14253633344829, -0.285890419115648, -0.0510679345341711   },
            new double[] {   1.92428102896004, -0.308506347407225, -0.338960200091768, 1.13998516130791, -0.289751027086574, -0.0525552764623512   },
            new double[] {   1.92464393057879, -0.310726873436372, -0.342700679426658, 1.13559820760706, -0.290255640015453, -0.0534689886629568   },
            new double[] {   1.92439928345032, -0.314527845381607, -0.34641491367418, 1.13335532777156, -0.292890850955067, -0.0540399842299806   },
            new double[] {   1.92429209001418, -0.317653230705445, -0.349042818820129, 1.13111607725924, -0.295212707533746, -0.0543382490747962   },
            new double[] {   1.92269098965687, -0.319908221562351, -0.353187221994653, 1.13148161443365, -0.296563011082157, -0.0547187606966963   },
            new double[] {   1.92192681012931, -0.320209759024923, -0.354376792761962, 1.13101206983756, -0.296344379268291, -0.0552019282058071   },
            new double[] {   1.92259868423872, -0.319881224897635, -0.355464812432544, 1.131870513592, -0.296097199273674, -0.0555361985630884   },
            new double[] {   1.92250782332634, -0.319587330623615, -0.357238187611451, 1.13203723626197, -0.295973631284389, -0.0563333546173585   },
            new double[] {   1.91586463051412, -0.318817780808652, -0.359579774403466, 1.13257043926832, -0.296462286057923, -0.056512644387931   },
            new double[] {   1.91492491108248, -0.318900427277931, -0.360397692370786, 1.13227539981703, -0.296942378421001, -0.0565231963539769   },
            new double[] {   1.91599408906475, -0.318060308544714, -0.362453618260234, 1.13378441197217, -0.297148930774961, -0.0570810015285631   },
            new double[] {   1.91689745959972, -0.317531164319526, -0.363890289953727, 1.13424754281175, -0.29746514780975, -0.0577511144273151   },
            new double[] {   1.91952455989602, -0.31252954557783, -0.37300638572958, 1.13614800038862, -0.297626034169287, -0.0597076197738709   },
            new double[] {   1.92108657677648, -0.309884124986592, -0.379489614106564, 1.13769757027752, -0.298008866678724, -0.0616745485496382   },
            new double[] {   1.92085359173726, -0.30886114924721, -0.382294637632894, 1.13808761131541, -0.297440881719447, -0.0628719307188009   },
            new double[] {   1.91797228794823, -0.306777706532849, -0.390551974595011, 1.13847191246989, -0.296905045860188, -0.0652710530604795   },
            new double[] {   1.91282754810856, -0.306971558041769, -0.39544492792409, 1.13406968416693, -0.297751190093657, -0.0667874714334393   },
            new double[] {   1.91219116767175, -0.306040510829695, -0.398438971713883, 1.13410365021949, -0.297791660922468, -0.0679367243042832   },
            new double[] {   1.91170405443773, -0.305560345277244, -0.401483659245705, 1.13506039849532, -0.297990870346254, -0.0691909503701207   },
            new double[] {   1.91030846264415, -0.305220133518465, -0.404001188606841, 1.1262351449169, -0.301312292675794, -0.0690062186203101   },
            new double[] {   1.91008470802349, -0.304738286935335, -0.407574759065513, 1.12150714015463, -0.306582805177899, -0.0708400391231407   },
            new double[] {   1.90965863235153, -0.304871285886655, -0.41154329679234, 1.11982973212795, -0.308438867164482, -0.0724037937122973   },
            new double[] {   1.90991166082743, -0.304871213028084, -0.416035896002362, 1.1190136707529, -0.309635823322883, -0.0735614039513215   },
            new double[] {   1.91013472839841, -0.305054573275303, -0.418958828531858, 1.11738400714048, -0.31205731843624, -0.0728262447749829   },
            new double[] {   1.89945197574636, -0.306774286509657, -0.423839800782672, 1.10327782900639, -0.317218461280696, -0.0689001005492088   },
            new double[] {   1.8998206350073, -0.306282999457817, -0.424998320837192, 1.10322190294796, -0.317242490130102, -0.069449678739133   },
            new double[] {   1.90044014372886, -0.306170272247114, -0.42621526988945, 1.10353712070717, -0.317473096835792, -0.0697049148940084   },
            new double[] {   1.89955438817155, -0.306027649466928, -0.427873838900551, 1.10275557654103, -0.318348249245661, -0.0701315561268955   },
            new double[] {   1.89931347250055, -0.306067017143357, -0.429309184370141, 1.10288380793263, -0.318277631112229, -0.0701944608862133   },
            new double[] {   1.89865188784385, -0.305965868166295, -0.430338004776462, 1.10228213114224, -0.317877982522218, -0.0704518812188525   },
#endregion
            }
        };
    }
}
