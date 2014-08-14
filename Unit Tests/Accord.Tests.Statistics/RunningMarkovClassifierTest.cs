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
    using Accord.Statistics.Running;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Models.Markov;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Models.Markov.Learning;
    using Accord.Math;
    using Accord.Statistics.Models.Markov.Topology;
    
    [TestClass()]
    public class GenericRunningMarkovClassifierTest
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
        public void PushTest()
        {
            int[][] sequences;
            
            var classifier = createClassifier(out sequences);
            var running = new RunningMarkovClassifier(classifier);

            for (int i = 0; i < sequences.Length; i++)
            {
                int[] sequence = sequences[i];

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

        [TestMethod()]
        public void PushTest2()
        {
            int[][] sequences;

            var classifier = createClassifier(out sequences, rejection: true);
            var running = new RunningMarkovClassifier(classifier);

            for (int i = 0; i < sequences.Length; i++)
            {
                int[] sequence = sequences[i];

                running.Clear();
                for (int j = 0; j < sequence.Length; j++)
                    running.Push(sequence[j]);

                double actualLikelihood;
                int actual = running.Classification;

                if (actual > -1)
                    actualLikelihood = Math.Exp(running.Responses[actual]) /
                        (running.Responses.Exp().Sum() + Math.Exp(running.Threshold));
                else
                    actualLikelihood = Math.Exp(running.Threshold) /
                        (running.Responses.Exp().Sum() + Math.Exp(running.Threshold));

                double expectedLikelihood;
                int expected = classifier.Compute(sequence, out expectedLikelihood);

                Assert.AreEqual(expected, actual);
                Assert.AreEqual(expectedLikelihood, actualLikelihood, 1e-8);
            }
        }

        private static HiddenMarkovClassifier createClassifier(
            out int[][] sequences, bool rejection = false)
        {
            sequences = new int[][] 
            {
                new int[] { 0,1,2,3,4 }, 
                new int[] { 4,3,2,1,0 }, 
            };

            int[] labels = { 0, 1 };

            HiddenMarkovClassifier classifier =
                new HiddenMarkovClassifier(2, new Ergodic(2), symbols: 5);

            var teacher = new HiddenMarkovClassifierLearning(classifier,

                modelIndex => new BaumWelchLearning(classifier.Models[modelIndex])
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
