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
    using Accord.Statistics.Models.Markov.Learning;
    using NUnit.Framework;
    using System;
    using Accord.Statistics.Models.Markov;
    using Accord.Statistics.Filters;
    using Accord.Math;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Distributions.Fitting;
    using System.Collections.Generic;

    [TestFixture]
    public class GenericMaximumLikelihoodLearningTest
    {


        [Test]
        public void RunTest()
        {
            // Example from
            // http://www.cs.columbia.edu/4761/notes07/chapter4.3-HMM.pdf



            double[][] observations =
            {
                new double[] { 0,0,0,1,0,0 },
                new double[] { 1,0,0,1,0,0 },
                new double[] { 0,0,1,0,0,0 },
                new double[] { 0,0,0,0,1,0 },
                new double[] { 1,0,0,0,1,0 },
                new double[] { 0,0,0,1,1,0 },
                new double[] { 1,0,0,0,0,0 },
                new double[] { 1,0,1,0,0,0 },
            };

            int[][] paths =
            {
                new int[] { 0,0,1,0,1,0 },
                new int[] { 1,0,1,0,1,0 },
                new int[] { 1,0,0,1,1,0 },
                new int[] { 1,0,1,1,1,0 },
                new int[] { 1,0,0,1,0,1 },
                new int[] { 0,0,1,0,0,1 },
                new int[] { 0,0,1,1,0,1 },
                new int[] { 0,1,1,1,0,0 },
            };


            GeneralDiscreteDistribution initial = new GeneralDiscreteDistribution(symbols: 2);

            var model = new HiddenMarkovModel<GeneralDiscreteDistribution>(states: 2, emissions: initial);

            var target = new MaximumLikelihoodLearning<GeneralDiscreteDistribution>(model);
            target.UseLaplaceRule = false;

            double logLikelihood = target.Run(observations, paths);

            var pi = Matrix.Exp(model.Probabilities);
            var A = Matrix.Exp(model.Transitions);
            var B = model.Emissions;

            Assert.AreEqual(0.5, pi[0]);
            Assert.AreEqual(0.5, pi[1]);

            Assert.AreEqual(7 / 20.0, A[0, 0], 1e-5);
            Assert.AreEqual(13 / 20.0, A[0, 1], 1e-5);
            Assert.AreEqual(14 / 20.0, A[1, 0], 1e-5);
            Assert.AreEqual(6 / 20.0, A[1, 1], 1e-5);

            Assert.AreEqual(17 / 25.0, B[0].ProbabilityMassFunction(0));
            Assert.AreEqual(8 / 25.0, B[0].ProbabilityMassFunction(1));
            Assert.AreEqual(19 / 23.0, B[1].ProbabilityMassFunction(0));
            Assert.AreEqual(4 / 23.0, B[1].ProbabilityMassFunction(1));

            Assert.AreEqual(-1.1472359046136624, logLikelihood);
        }

        [Test]
        public void learnTest()
        {
            #region doc_learn
            // Example from
            // http://www.cs.columbia.edu/4761/notes07/chapter4.3-HMM.pdf

            // Inputs
            int[][] observations =
            {
                new int[] { 0,0,0,1,0,0 },
                new int[] { 1,0,0,1,0,0 },
                new int[] { 0,0,1,0,0,0 },
                new int[] { 0,0,0,0,1,0 },
                new int[] { 1,0,0,0,1,0 },
                new int[] { 0,0,0,1,1,0 },
                new int[] { 1,0,0,0,0,0 },
                new int[] { 1,0,1,0,0,0 },
            };

            // Outputs
            int[][] paths =
            {
                new int[] { 0,0,1,0,1,0 },
                new int[] { 1,0,1,0,1,0 },
                new int[] { 1,0,0,1,1,0 },
                new int[] { 1,0,1,1,1,0 },
                new int[] { 1,0,0,1,0,1 },
                new int[] { 0,0,1,0,0,1 },
                new int[] { 0,0,1,1,0,1 },
                new int[] { 0,1,1,1,0,0 },
            };

            // Create the initial discrete distributions for 2 symbols
            var initial = new GeneralDiscreteDistribution(symbols: 2);

            // Create a generic hidden Markov model based on the initial distribution with 2 states
            var model = new HiddenMarkovModel<GeneralDiscreteDistribution, int>(states: 2, emissions: initial);

            // Create a new (fully supervised) Maximum Likelihood learning algorithm for HMMs
            var target = new MaximumLikelihoodLearning<GeneralDiscreteDistribution, int>(model)
            {
                UseLaplaceRule = false // don't use Laplace smoothing (to reproduce the example)
            };

            // Learn the Markov model
            target.Learn(observations, paths);

            // Recover the learned parameters
            var pi = model.LogInitial.Exp();
            var A = model.LogTransitions.Exp();
            var B = model.Emissions;
            #endregion

            Assert.AreEqual(0.5, pi[0]);
            Assert.AreEqual(0.5, pi[1]);

            Assert.AreEqual(7 / 20.0, A[0][0], 1e-5);
            Assert.AreEqual(13 / 20.0, A[0][1], 1e-5);
            Assert.AreEqual(14 / 20.0, A[1][0], 1e-5);
            Assert.AreEqual(6 / 20.0, A[1][1], 1e-5);

            Assert.AreEqual(17 / 25.0, B[0].ProbabilityMassFunction(0));
            Assert.AreEqual(8 / 25.0, B[0].ProbabilityMassFunction(1));
            Assert.AreEqual(19 / 23.0, B[1].ProbabilityMassFunction(0));
            Assert.AreEqual(4 / 23.0, B[1].ProbabilityMassFunction(1));
        }



        [Test]
        public void sequence_parsing_test()
        {
            #region doc_learn_fraud_analysis

            // Ensure results are reproducible
            Accord.Math.Random.Generator.Seed = 0;

            // Let's say we have the following data about credit card transactions,
            // where the data is organized in order of transaction, per credit card 
            // holder. Everytime the "Time" column starts at zero, denotes that the
            // sequence of observations follow will correspond to transactions of the
            // same person:

            double[,] data =
            {
                // "Time", "V1",   "V2",  "V3", "V4", "V5", "Amount",  "Fraud"
                {      0,   0.521, 0.124, 0.622, 15.2, 25.6,   2.70,      0 }, // first person, ok
                {      1,   0.121, 0.124, 0.822, 12.2, 25.6,   42.0,      0 }, // first person, ok
                
                {      0,   0.551, 0.124, 0.422, 17.5, 25.6,   20.0,      0 }, // second person, ok
                {      1,   0.136, 0.154, 0.322, 15.3, 25.6,   50.0,      0 }, // second person, ok
                {      2,   0.721, 0.240, 0.422, 12.2, 25.6,   100.0,     1 }, // second person, fraud!
                {      3,   0.222, 0.126, 0.722, 18.1, 25.8,   10.0,      0 }, // second person, ok
            };

            // Transform the above data into a jagged matrix
            double[][][] input;
            int[][] states;
            transform(data, out input, out states);

            // Determine here the number of dimensions in the observations (in this case, 6)
            int observationDimensions = 6; // 6 columns: "V1", "V2", "V3", "V4", "V5", "Amount"

            // Create some prior distributions to help initialize our parameters
            var priorC = new WishartDistribution(dimension: observationDimensions, degreesOfFreedom: 10); // this 10 is just some random number, you might have to tune as if it was a hyperparameter
            var priorM = new MultivariateNormalDistribution(dimension: observationDimensions);

            // Configure the learning algorithms to train the sequence classifier
            var teacher = new MaximumLikelihoodLearning<MultivariateNormalDistribution, double[]>()
            {
                // Their emissions will be multivariate Normal distributions initialized using the prior distributions
                Emissions = (j) => new MultivariateNormalDistribution(mean: priorM.Generate(), covariance: priorC.Generate()),

                // We will prevent our covariance matrices from becoming degenerate by adding a small 
                // regularization value to their diagonal until they become positive-definite again:
                FittingOptions = new NormalOptions()
                {
                    Regularization = 1e-6
                },
            };

            // Use the teacher to learn a new HMM 
            var hmm = teacher.Learn(input, states);

            // Use the HMM to predict whether the transations were fradulent or not:
            int[] firstPerson = hmm.Decide(input[0]); // predict the first person, output should be: 0, 0

            int[] secondPerson = hmm.Decide(input[1]); // predict the second person, output should be: 0, 0, 1, 0
            #endregion


            Assert.AreEqual(new[] { 0, 0 }, firstPerson);
            Assert.AreEqual(new[] { 0, 0, 1, 0}, secondPerson);
        }

        #region doc_learn_fraud_transform
        private static void transform(double[,] data, out double[][][] input, out int[][] states)
        {
            var sequences = new List<double[][]>();
            var classLabels = new List<int[]>();

            List<double[]> currentSequence = null;
            List<int> currentLabels = null;
            for (int i = 0; i < data.Rows(); i++)
            {
                // Check if the first column contains a zero, this would be an indication
                // that a new sequence (for a different person) is beginning:
                if (data[i, 0] == 0)
                {
                    // Yes, this is a new sequence. Check if we were building
                    // a sequence before, and if yes, save it to the list:
                    if (currentSequence != null)
                    {
                        // Save the sequence we had so far 
                        sequences.Add(currentSequence.ToArray());
                        classLabels.Add(currentLabels.ToArray());

                        currentSequence = null;
                        currentLabels = null;
                    }

                    // We will be starting a new sequence
                    currentSequence = new List<double[]>();
                    currentLabels = new List<int>();
                }

                double[] features = data.GetRow(i).Get(1, 7); // Get values in columns from 1 (inclusive) to 7 (exclusive), meaning "V1", "V2", "V3", "V4", "V5", and "Amount"
                int classLabel = (int)data[i, 7]; // The seventh index corresponds to the class label column ("Class")

                // Save this information:
                currentSequence.Add(features);
                currentLabels.Add(classLabel);
            }

            // Check if there are any sequences and labels that we haven't saved yet:
            if (currentSequence != null)
            {
                // Yes there are: save them
                sequences.Add(currentSequence.ToArray());
                classLabels.Add(currentLabels.ToArray());
            }

            input = sequences.ToArray();
            states = classLabels.ToArray();
        }
        #endregion
    }
}
