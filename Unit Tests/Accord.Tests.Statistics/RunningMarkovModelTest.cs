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
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Models.Markov;
    using Accord.Statistics.Models.Markov.Learning;
    using Accord.Statistics.Models.Markov.Topology;
    using Accord.Statistics.Running;
    using NUnit.Framework;


    [TestFixture]
    public class RunningMarkovModelTest
    {

        [Test]
        public void PushTest()
        {
            var hmm = createModel();

            var running = new RunningMarkovStatistics<MultivariateNormalDistribution>(hmm);

            double[][][] sequences =            
            {
                new[] 
                { 
                    new double[] { 1, 2 }, 
                    new double[] { 6, 7 },
                    new double[] { 2, 3 },
                },
                new[] 
                { 
                    new double[] { 2, 2 }, 
                    new double[] { 9, 8 },
                    new double[] { 1, 0 },
                },
                new[] 
                { 
                    new double[] { 8, 7 }, 
                    new double[] { 9, 8  },
                    new double[] { 1, 0 }
                }
            };

            for (int i = 0; i < sequences.Length; i++)
            {
                running.Clear();

                double[][] sequence = sequences[i];
                for (int j = 0; j < sequence.Length; j++)
                    running.Push(sequence[j]);

                double actual = running.LogForward;
                double expected = hmm.Evaluate(sequence);

                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void PeekTest()
        {
            var hmm = createModel();

            var running = new RunningMarkovStatistics<MultivariateNormalDistribution>(hmm);

            double[][][] sequences =            
            {
                new[] 
                { 
                    new double[] { 1, 2 }, 
                    new double[] { 6, 7 },
                    new double[] { 2, 3 },
                },
                new[] 
                { 
                    new double[] { 2, 2 }, 
                    new double[] { 9, 8 },
                    new double[] { 1, 0 },
                },
                new[] 
                { 
                    new double[] { 8, 7 }, 
                    new double[] { 9, 8  },
                    new double[] { 1, 0 }
                }
            };

            for (int i = 0; i < sequences.Length; i++)
            {
                running.Clear();

                double[][] sequence = sequences[i];
                for (int j = 0; j < sequence.Length - 1; j++)
                    running.Push(sequence[j]);

                for (int j = 0; j < sequence.Length; j++)
                {
                    double actual = running.Peek(sequence[j]);

                    double[][] window = sequence.First(2);
                    double expected = hmm.Evaluate(window.Concatenate(sequence[j]));

                    Assert.AreEqual(expected, actual);
                }
            }
        }

        private static HiddenMarkovModel<MultivariateNormalDistribution> createModel()
        {
            double[][][] sequences =
            {
                new double[][] 
                {
                    new double[] { 1, 2 }, 
                    new double[] { 6, 7 }, 
                    new double[] { 2, 3 }, 
                },
                new double[][] 
                {
                    new double[] { 2, 2 }, 
                    new double[] { 9, 8 }, 
                    new double[] { 1, 0 }, 
                },
                new double[][] 
                {
                    new double[] { 1, 3 }, 
                    new double[] { 8, 9 }, 
                    new double[] { 3, 3 }, 
                },
            };

            var density = new MultivariateNormalDistribution(dimension: 2);

            var model = new HiddenMarkovModel<MultivariateNormalDistribution>(new Forward(2), density);

            var teacher = new BaumWelchLearning<MultivariateNormalDistribution>(model)
            {
                Tolerance = 0.0001,
                Iterations = 0,
            };

            double logLikelihood = teacher.Run(sequences);
            return model;
        }
    }
}
