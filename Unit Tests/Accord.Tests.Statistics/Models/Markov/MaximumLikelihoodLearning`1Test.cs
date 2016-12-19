﻿// Accord Unit Tests
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
    using Accord.Statistics.Models.Markov.Learning;
    using NUnit.Framework;
    using System;
    using Accord.Statistics.Models.Markov;
    using Accord.Statistics.Filters;
    using Accord.Math;
    using Accord.Statistics.Distributions.Univariate;

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
            var A  = model.LogTransitions.Exp();
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
    }
}
