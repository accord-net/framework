

namespace Accord.Tests.Statistics
{
    using Accord.Statistics.Running;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Models.Markov;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Models.Markov.Learning;
    using Accord.Math;
    using Accord.Statistics.Models.Markov.Topology;


    [TestClass()]
    public class RunningMarkovModelTest
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

        [TestMethod()]
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

                    double[][] window = sequence.Submatrix(2);
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
