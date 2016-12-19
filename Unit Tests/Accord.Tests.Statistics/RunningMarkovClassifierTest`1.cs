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
    using Accord.Statistics.Running;
    using NUnit.Framework;
    using System;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Models.Markov;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Models.Markov.Learning;
    using Accord.Math;
    using Accord.Statistics.Models.Markov.Topology;

    [TestFixture]
    public class RunningMarkovClassifierTest
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




        [Test]
        public void PushTest()
        {
            double[][] sequences;

            var classifier = createClassifier(out sequences);
            var running = new RunningMarkovClassifier<NormalDistribution>(classifier);

            for (int i = 0; i < sequences.Length; i++)
            {
                double[] sequence = sequences[i];

                running.Clear();
                for (int j = 0; j < sequence.Length; j++)
                    running.Push(sequence[j]);

                double actualLikelihood;
                int actual = running.Classification;
                actualLikelihood = Math.Exp(running.Responses[actual]) / running.Responses.Exp().Sum();

                double expectedLikelihood;
                int expected = classifier.Compute(sequence, out expectedLikelihood);

                Assert.AreEqual(expected, actual);
                Assert.AreEqual(expectedLikelihood, actualLikelihood, 1e-8);
            }
        }

        [Test]
        public void PushTest2()
        {
            double[][] sequences;

            var classifier = createClassifier(out sequences, rejection: true);
            var running = new RunningMarkovClassifier<NormalDistribution>(classifier);

            for (int i = 0; i < sequences.Length; i++)
            {
                double[] sequence = sequences[i];

                running.Clear();
                for (int j = 0; j < sequence.Length; j++)
                    running.Push(sequence[j]);

                double actualLikelihood;
                int actual = running.Classification;

                if (actual > -1)
                {
                    double sum = running.Responses.Exp().Sum() + Math.Exp(running.Threshold);
                    actualLikelihood = Math.Exp(running.Responses[actual]) / sum;
                }
                else
                {
                    double sum = running.Responses.Exp().Sum();
                    actualLikelihood = Math.Exp(running.Threshold) /
                        (sum + Math.Exp(running.Threshold));
                }

                double expectedLikelihood;
                int expected = classifier.Compute(sequence, out expectedLikelihood);

                Assert.AreEqual(expected, actual);
                Assert.AreEqual(expectedLikelihood, actualLikelihood, 1e-8);
            }
        }

        private static HiddenMarkovClassifier<NormalDistribution> createClassifier(
            out double[][] sequences, bool rejection = false)
        {
            sequences = new double[][] 
            {
                new double[] { 0,1,2,3,4 }, 
                new double[] { 4,3,2,1,0 }, 
            };

            int[] labels = { 0, 1 };

            NormalDistribution density = new NormalDistribution();
            HiddenMarkovClassifier<NormalDistribution> classifier =
                new HiddenMarkovClassifier<NormalDistribution>(2, new Ergodic(2), density);

            var teacher = new HiddenMarkovClassifierLearning<NormalDistribution>(classifier,

                modelIndex => new BaumWelchLearning<NormalDistribution>(classifier.Models[modelIndex])
                {
                    Tolerance = 0.0001,
                    Iterations = 0
                }
            );

            teacher.Rejection = rejection;
            teacher.Run(sequences, labels);

            return classifier;
        }

    }
}
