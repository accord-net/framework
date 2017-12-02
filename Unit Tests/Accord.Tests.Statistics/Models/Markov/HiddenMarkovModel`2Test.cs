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
    using System.Collections.Generic;
    using System;
    using Accord.Math;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Models.Markov;
    using Accord.Statistics.Models.Markov.Learning;
    using Accord.Statistics.Models.Markov.Topology;
    using NUnit.Framework;

    [TestFixture]
    public class GenericHiddenMarkovModelTest2
    {

        [Test]
        public void ConstructorTest()
        {
            double[,] A;
            double[] pi;

            var hmm = HiddenMarkovModel.CreateDiscrete(2, 4);

            A = new double[,]
            {
                { 0.5, 0.5 },
                { 0.5, 0.5 }
            };

            pi = new double[] { 1, 0 };

            var logA = A.Log();
            var logPi = pi.Log();

            Assert.AreEqual(2, hmm.States);
            Assert.AreEqual(1, hmm.NumberOfInputs);
            Assert.IsTrue(logA.IsEqual(hmm.LogTransitions));
            Assert.IsTrue(logPi.IsEqual(hmm.LogInitial));


            hmm = HiddenMarkovModel.CreateDiscrete(new Forward(2), 4);

            A = new double[,]
            {
                { 0.5, 0.5 },
                { 0.0, 1.0 }
            };

            pi = new double[] { 1, 0 };

            logA = A.Log();
            logPi = pi.Log();

            Assert.AreEqual(2, hmm.States);
            Assert.AreEqual(1, hmm.NumberOfInputs);
            Assert.IsTrue(logA.IsEqual(hmm.LogTransitions));
            Assert.IsTrue(logPi.IsEqual(hmm.LogInitial));



            A = new double[,]
            {
                { 0.7, 0.3 },
                { 0.4, 0.6 }
            };

            GeneralDiscreteDistribution[] B =
            {
                new GeneralDiscreteDistribution(0.1, 0.4, 0.5),
                new GeneralDiscreteDistribution(0.6, 0.3, 0.1)
            };

            pi = new double[]
            {
                0.6, 0.4
            };

            hmm = new HiddenMarkovModel<GeneralDiscreteDistribution, int>(A, B, pi);

            logA = A.Log();
            logPi = pi.Log();

            Assert.AreEqual(2, hmm.States);
            Assert.AreEqual(1, hmm.NumberOfInputs);
            Assert.IsTrue(logA.IsEqual(hmm.LogTransitions));
            Assert.IsTrue(logPi.IsEqual(hmm.LogInitial));
            Assert.AreEqual(B, hmm.Emissions);
        }

        [Test]
        public void ConstructorTest2()
        {

            double[,] A = new double[,]
            {
                { 0.5, 0.5 },
                { 0.5, 0.5 }
            };

            double[] pi = new double[] { 1, 0 };

            var distribution = new MultivariateNormalDistribution(3);
            var hmm = new HiddenMarkovModel<MultivariateNormalDistribution, double[]>(2, distribution);

            for (int i = 0; i < hmm.Emissions.Length; i++)
            {
                IDistribution b = hmm.Emissions[i];

                Assert.AreNotSame(distribution, b);
                Assert.IsTrue(b is MultivariateNormalDistribution);

                MultivariateNormalDistribution n = b as MultivariateNormalDistribution;

                Assert.AreEqual(n.Dimension, hmm.NumberOfInputs);

                Assert.AreNotSame(n.Covariance, distribution.Covariance);
                Assert.IsTrue(n.Covariance.IsEqual(distribution.Covariance));

                Assert.AreNotSame(n.Mean, distribution.Mean);
                Assert.IsTrue(n.Mean.IsEqual(distribution.Mean));
            }

            Assert.AreEqual(2, hmm.States);
            Assert.AreEqual(3, hmm.NumberOfInputs);
            Assert.AreEqual(2, hmm.Emissions.Length);

            var logA = Matrix.Log(A);
            var logPi = Matrix.Log(pi);

            Assert.IsTrue(logA.IsEqual(hmm.LogTransitions));
            Assert.IsTrue(logPi.IsEqual(hmm.LogInitial));
        }

        [Test]
        public void ConstructorTest3()
        {
            double[,] A = new double[,]
            {
                { 0.5, 0.5 },
                { 0.5, 0.5 }
            };

            double[] pi = new double[] { 1, 0 };

            MultivariateNormalDistribution[] emissions =
            {
                new MultivariateNormalDistribution(new[] { 0.0, 0.1 }, new[,] { {1.0, 0.0}, {1.0, 5.1} }),
                new MultivariateNormalDistribution(new[] { 2.0, 0.0 }, new[,] { {1.1, 0.1}, {1.0, 6.0} }),
            };

            var hmm = new HiddenMarkovModel<MultivariateNormalDistribution, double[]>(A, emissions, pi);

            for (int i = 0; i < hmm.Emissions.Length; i++)
            {
                IDistribution b = hmm.Emissions[i];
                IDistribution e = emissions[i];

                Assert.AreEqual(b, e);
            }

            A = A.Log();
            pi = pi.Log();
            Assert.AreEqual(2, hmm.States);
            Assert.AreEqual(2, hmm.NumberOfInputs);
            Assert.AreEqual(2, hmm.Emissions.Length);
            Assert.IsTrue(A.IsEqual(hmm.LogTransitions));
            Assert.IsTrue(pi.IsEqual(hmm.LogInitial));
        }

        [Test]
        public void ConstructorTest_RandomEquals()
        {
            Accord.Math.Random.Generator.Seed = 0;
            var dhmm = new HiddenMarkovModel(10, 50, true);

            Accord.Math.Random.Generator.Seed = 0;
            var chmm = HiddenMarkovModel.CreateDiscrete(10, 50, true);

            for (int i = 0; i < dhmm.Probabilities.Length; i++)
                Assert.AreEqual(dhmm.LogInitial[i], chmm.LogInitial[i]);

            for (int i = 0; i < dhmm.States; i++)
                for (int j = 0; j < dhmm.States; j++)
                    Assert.AreEqual(dhmm.Transitions[i, j], chmm.LogTransitions[i][j]);

            for (int i = 0; i < dhmm.States; i++)
                for (int j = 0; j < dhmm.Symbols; j++)
                    Assert.AreEqual(dhmm.Emissions[i, j], chmm.Emissions[i][j], 1e-10);
        }

        [Test]
        public void DecodeTest()
        {
            #region doc_decode
            // Create the transition matrix A
            double[,] transitions =
            {
                { 0.7, 0.3 },
                { 0.4, 0.6 }
            };

            // Create the vector of emission densities B
            GeneralDiscreteDistribution[] emissions =
            {
                new GeneralDiscreteDistribution(0.1, 0.4, 0.5),
                new GeneralDiscreteDistribution(0.6, 0.3, 0.1)
            };

            // Create the initial probabilities pi
            double[] initial =
            {
                0.6, 0.4
            };

            // Create a new hidden Markov model with discrete probabilities
            var hmm = new HiddenMarkovModel<GeneralDiscreteDistribution, double>(transitions, emissions, initial);

            // After that, one could, for example, query the probability
            // of a sequence occurring. We will consider the sequence
            double[] sequence = new double[] { 0, 1, 2 };

            // And now we will evaluate its likelihood
            double logLikelihood = hmm.LogLikelihood(sequence);

            // At this point, the log-likelihood of the sequence
            // occurring within the model is -3.3928721329161653.

            // We can also get the Viterbi path of the sequence
            int[] path = hmm.Decide(sequence);

            // Or also its Viterbi likelihood alongside the path
            double viterbi = hmm.LogLikelihood(sequence, ref path);

            // At this point, the state path will be 1-0-0 and the
            // log-likelihood will be -4.3095199438871337
            #endregion

            Assert.AreEqual(logLikelihood, -3.3928721329161653, 1e-10);
            Assert.AreEqual(viterbi, -3.3928721329161653, 1e-10);
            Assert.AreEqual(path[0], 1);
            Assert.AreEqual(path[1], 0);
            Assert.AreEqual(path[2], 0);
        }

        [Test]
        public void DecodeIntegersTest()
        {
            double[,] transitions =
            {
                { 0.7, 0.3 },
                { 0.4, 0.6 }
            };

            GeneralDiscreteDistribution[] emissions =
            {
                new GeneralDiscreteDistribution(0.1, 0.4, 0.5),
                new GeneralDiscreteDistribution(0.6, 0.3, 0.1)
            };

            double[] initial =
            {
                0.6, 0.4
            };

            var hmm = new HiddenMarkovModel<GeneralDiscreteDistribution, int>(transitions, emissions, initial);

            int[] sequence = new int[] { 0, 1, 2 };

            double logLikelihood = hmm.Evaluate(sequence);
            int[] path = hmm.Decode(sequence, out logLikelihood);

            Assert.AreEqual(logLikelihood, Math.Log(0.01344), 1e-10);
            Assert.AreEqual(path[0], 1);
            Assert.AreEqual(path[1], 0);
            Assert.AreEqual(path[2], 0);
        }

        [Test]
        public void DecodeTest2()
        {
            double[,] transitions =
            {
                { 0.7, 0.3 },
                { 0.4, 0.6 }
            };

            double[,] emissions =
            {
                { 0.1, 0.4, 0.5 },
                { 0.6, 0.3, 0.1 }
            };

            double[] initial =
            {
                0.6, 0.4
            };

            var hmm = new HiddenMarkovModel<GeneralDiscreteDistribution, double>(transitions, GeneralDiscreteDistribution.FromMatrix(emissions), initial);

            double logLikelihood;
            double[] sequence = new double[] { 0, 1, 2 };
            int[] path = hmm.Decode(sequence, out logLikelihood);

            double expected = Math.Log(0.01344);

            Assert.AreEqual(logLikelihood, expected, 1e-10);
            Assert.AreEqual(path[0], 1);
            Assert.AreEqual(path[1], 0);
            Assert.AreEqual(path[2], 0);
        }

        [Test]
        public void DecodeTest5()
        {
            var density = new MultivariateNormalDistribution(3);

            var hmm = new HiddenMarkovModel<MultivariateNormalDistribution, double[]>(2, density);


            double logLikelihood;
            int[] path = hmm.Decode(new double[][]
                {
                    new double[] { 0, 1, 2 },
                    new double[] { 0, 1, 2 },
                }, out logLikelihood);

            Assert.AreEqual(-11.206778379787982, logLikelihood);
        }

        [Test]
        public void LearnTest3()
        {
            double[][] sequences = new double[][]
            {
                new double[] { 0,1,1,1,1,0,1,1,1,1 },
                new double[] { 0,1,1,1,0,1,1,1,1,1 },
                new double[] { 0,1,1,1,1,1,1,1,1,1 },
                new double[] { 0,1,1,1,1,1         },
                new double[] { 0,1,1,1,1,1,1       },
                new double[] { 0,1,1,1,1,1,1,1,1,1 },
                new double[] { 0,1,1,1,1,1,1,1,1,1 },
            };

            // Creates a new Hidden Markov Model with 3 states
            var hmm = HiddenMarkovModel.CreateDiscrete(3, 2);

            // Try to fit the model to the data until the difference in
            //  the average log-likelihood changes only by as little as 0.0001
            var teacher = new BaumWelchLearning<GeneralDiscreteDistribution, int>(hmm)
            {
                Topology = new Ergodic(3),
                Tolerance = 0.0001
            };

            Assert.AreEqual(0, teacher.CurrentIteration);
            var hmm2 = teacher.Learn(sequences.ToInt32());
            Assert.AreEqual(13, teacher.CurrentIteration);
            Assert.AreEqual(0, teacher.MaxIterations);

            double ll = teacher.LogLikelihood;

            // Calculate the probability that the given
            //  sequences originated from the model
            double l1; hmm.Decode(new int[] { 0, 1 }, out l1);        // 0.4999
            double l2; hmm.Decode(new int[] { 0, 1, 1, 1 }, out l2);  // 0.1145

            double l3; hmm.Decode(new int[] { 1, 1 }, out l3);        // 0.0000
            double l4; hmm.Decode(new int[] { 1, 0, 0, 0 }, out l4);  // 0.0000

            double l5; hmm.Decode(new int[] { 0, 1, 0, 1, 1, 1, 1, 1, 1 }, out l5); // 0.0002
            double l6; hmm.Decode(new int[] { 0, 1, 1, 1, 1, 1, 1, 0, 1 }, out l6); // 0.0002


            ll = System.Math.Exp(ll);
            l1 = System.Math.Exp(l1);
            l2 = System.Math.Exp(l2);
            l3 = System.Math.Exp(l3);
            l4 = System.Math.Exp(l4);
            l5 = System.Math.Exp(l5);
            l6 = System.Math.Exp(l6);

            Assert.AreEqual(0.30679264538040718, ll, 1e-4);
            Assert.AreEqual(0.49999423004041477, l1, 1e-4);
            Assert.AreEqual(0.114586850458029, l2, 1e-4);
            Assert.AreEqual(2.5713496109015777E-06, l3, 1e-4);
            Assert.AreEqual(2.2386106829023717E-18, l4, 1e-4);
            Assert.AreEqual(0.00026743534097025686, l5, 1e-4);
            Assert.AreEqual(0.00026743534097025686, l6, 1e-4);

            Assert.IsTrue(l1 > l3 && l1 > l4);
            Assert.IsTrue(l2 > l3 && l2 > l4);

            Assert.AreEqual(1, hmm.NumberOfInputs);
        }

        [Test]
        public void LearnTest6()
        {
            #region doc_learn
            // Continuous Markov Models can operate using any
            // probability distribution, including discrete ones. 

            // In the following example, we will try to create a
            // Continuous Hidden Markov Model using a discrete
            // distribution to detect if a given sequence starts
            // with a zero and has any number of ones after that.

            int[][] sequences = new double[][]
            {
                new double[] { 0,1,1,1,1,0,1,1,1,1 },
                new double[] { 0,1,1,1,0,1,1,1,1,1 },
                new double[] { 0,1,1,1,1,1,1,1,1,1 },
                new double[] { 0,1,1,1,1,1         },
                new double[] { 0,1,1,1,1,1,1       },
                new double[] { 0,1,1,1,1,1,1,1,1,1 },
                new double[] { 0,1,1,1,1,1,1,1,1,1 },
            }.ToInt32();

            // Create a new Hidden Markov Model with 3 states and
            //  a generic discrete distribution with two symbols
            var hmm = HiddenMarkovModel.CreateDiscrete(3, 2);

            // Try to fit the model to the data until the difference in
            //  the average log-likelihood changes only by as little as 0.0001
            var teacher = new BaumWelchLearning<GeneralDiscreteDistribution, int>(hmm)
            {
                Tolerance = 0.0001,
                Iterations = 0
            };

            // Learn the model
            teacher.Learn(sequences);

            double ll = Math.Exp(teacher.LogLikelihood);

            // Calculate the probability that the given
            //  sequences originated from the model
            double l1 = hmm.Probability(new int[] { 0, 1 });       // 0.999
            double l2 = hmm.Probability(new int[] { 0, 1, 1, 1 }); // 0.916

            // Sequences which do not start with zero have much lesser probability.
            double l3 = hmm.Probability(new int[] { 1, 1 });       // 0.000
            double l4 = hmm.Probability(new int[] { 1, 0, 0, 0 }); // 0.000

            // Sequences which contains few errors have higher probability
            //  than the ones which do not start with zero. This shows some
            //  of the temporal elasticity and error tolerance of the HMMs.
            double l5 = hmm.Probability(new int[] { 0, 1, 0, 1, 1, 1, 1, 1, 1 }); // 0.034
            double l6 = hmm.Probability(new int[] { 0, 1, 1, 1, 1, 1, 1, 0, 1 }); // 0.034
            #endregion

            //Assert.AreSame(hmm, same);

            Assert.AreEqual(0.30679264538040718, ll, 1e-4);
            Assert.AreEqual(0.99998846008082953, l1, 1e-4);
            Assert.AreEqual(0.91669523195813685, l2, 1e-4);
            Assert.AreEqual(5.1426992218031553E-06, l3, 1e-4);
            Assert.AreEqual(1.7529139078386114E-17, l4, 1e-4);
            Assert.AreEqual(0.034236482540284281, l5, 1e-4);
            Assert.AreEqual(0.03423647471730052, l6, 1e-4);

            Assert.IsTrue(l1 > l3 && l1 > l4);
            Assert.IsTrue(l2 > l3 && l2 > l4);
        }

        [Test]
        public void LearnTest7()
        {
            #region doc_learn2
            // Create continuous sequences. In the sequences below, there
            //  seems to be two states, one for values between 0 and 1 and
            //  another for values between 5 and 7. The states seems to be
            //  switched on every observation.
            double[][] sequences = new double[][]
            {
                new double[] { 0.1, 5.2, 0.3, 6.7, 0.1, 6.0 },
                new double[] { 0.2, 6.2, 0.3, 6.3, 0.1, 5.0 },
                new double[] { 0.1, 7.0, 0.1, 7.0, 0.2, 5.6 },
            };


            // Specify a initial normal distribution for the samples.
            var density = new NormalDistribution();

            // Creates a continuous hidden Markov Model with two states organized in a forward
            //  topology and an underlying univariate Normal distribution as probability density.
            var model = new HiddenMarkovModel<NormalDistribution, double>(new Ergodic(2), density);

            // Configure the learning algorithms to train the sequence classifier until the
            // difference in the average log-likelihood changes only by as little as 0.0001
            var teacher = new BaumWelchLearning<NormalDistribution, double>(model)
            {
                Tolerance = 0.0001,
                Iterations = 0,
            };

            // Fit the model
            teacher.Learn(sequences);

            double logLikelihood = teacher.LogLikelihood;

            // See the log-probability of the sequences learned
            double a1 = model.LogLikelihood(new[] { 0.1, 5.2, 0.3, 6.7, 0.1, 6.0 }); // -0.12799388666109757
            double a2 = model.LogLikelihood(new[] { 0.2, 6.2, 0.3, 6.3, 0.1, 5.0 }); // 0.01171157434400194

            // See the probability of an unrelated sequence
            double a3 = model.LogLikelihood(new[] { 1.1, 2.2, 1.3, 3.2, 4.2, 1.0 }); // -298.7465244473417

            double likelihood = Math.Exp(logLikelihood);
            a1 = Math.Exp(a1); // 0.879
            a2 = Math.Exp(a2); // 1.011
            a3 = Math.Exp(a3); // 0.000

            // We can also ask the model to decode one of the sequences. After
            // this step the resulting sequence will be: { 0, 1, 0, 1, 0, 1 }
            //
            int[] states = model.Decide(new[] { 0.1, 5.2, 0.3, 6.7, 0.1, 6.0 });
            #endregion

            Assert.AreEqual(7, teacher.CurrentIteration);
            Assert.IsTrue(states.IsEqual(new[] { 0, 1, 0, 1, 0, 1 }));

            Assert.AreEqual(1.091030568847944, likelihood, 1e-10);
            Assert.AreEqual(0.87985875800297753, a1, 1e-10);
            Assert.AreEqual(1.0117804233450221, a2, 1e-10);
            Assert.AreEqual(1.8031545195073828E-130, a3, 1e-10);

            Assert.AreEqual(2, model.Emissions.Length);
            var state1 = (model.Emissions[0] as NormalDistribution);
            var state2 = (model.Emissions[1] as NormalDistribution);
            Assert.AreEqual(0.16666666666666, state1.Mean, 1e-10);
            Assert.AreEqual(6.11111111111111, state2.Mean, 1e-10);
            Assert.IsFalse(Double.IsNaN(state1.Mean));
            Assert.IsFalse(Double.IsNaN(state2.Mean));

            Assert.AreEqual(0.007499999999999, state1.Variance, 1e-10);
            Assert.AreEqual(0.538611111111111, state2.Variance, 1e-10);
            Assert.IsFalse(Double.IsNaN(state1.Variance));
            Assert.IsFalse(Double.IsNaN(state2.Variance));

            Assert.AreEqual(2, model.LogTransitions.GetLength(0));
            Assert.AreEqual(2, model.LogTransitions.Columns());

            var A = model.LogTransitions.Exp();
            Assert.AreEqual(0, A[0][0], 1e-16);
            Assert.AreEqual(1, A[0][1], 1e-16);
            Assert.AreEqual(1, A[1][0], 1e-16);
            Assert.AreEqual(0, A[1][1], 1e-16);

            Assert.IsFalse(A.HasNaN());
        }

        [Test]
        public void LearnTest8()
        {
            #region doc_learn3
            // Create continuous sequences. In the sequence below, there
            // seems to be two states, one for values equal to 1 and another
            // for values equal to 2.
            double[][] sequences = new double[][]
            {
                new double[] { 1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2 }
            };

            // Specify a initial normal distribution for the samples.
            var density = new NormalDistribution();

            // Creates a continuous hidden Markov Model with two states organized in a forward
            //  topology and an underlying univariate Normal distribution as probability density.
            var model = new HiddenMarkovModel<NormalDistribution, double>(new Ergodic(2), density);

            // Configure the learning algorithms to train the sequence classifier until the
            // difference in the average log-likelihood changes only by as little as 0.0001
            var teacher = new BaumWelchLearning<NormalDistribution, double>(model)
            {
                Tolerance = 0.0001,
                Iterations = 0,

                // However, we will need to specify a regularization constant as the
                //  variance of each state will likely be zero (all values are equal)
                FittingOptions = new NormalOptions() { Regularization = double.Epsilon }
            };

            // Fit the model
            teacher.Learn(sequences);

            double likelihood = teacher.LogLikelihood;


            // See the likelihood of the sequences learned
            double a1 = model.LogLikelihood(new double[] { 1, 2, 1, 2, 1, 2, 1, 2, 1 }); // exp(a1) = infinity
            double a2 = model.LogLikelihood(new double[] { 1, 2, 1, 2, 1 });             // exp(a2) = infinity

            // See the likelihood of an unrelated sequence
            double a3 = model.LogLikelihood(new double[] { 1, 2, 3, 2, 1, 2, 1 });          // exp(a3) = 0
            double a4 = model.LogLikelihood(new double[] { 1.1, 2.2, 1.3, 3.2, 4.2, 1.0 }); // exp(a4) = 0
            #endregion

            Assert.AreEqual(double.PositiveInfinity, System.Math.Exp(likelihood));
            Assert.AreEqual(3341.7098768473734, a1);
            Assert.AreEqual(1856.5054871374298, a2);
            Assert.AreEqual(0.0, Math.Exp(a3));
            Assert.AreEqual(0.0, Math.Exp(a4));

            Assert.AreEqual(2, model.Emissions.Length);
            var state1 = (model.Emissions[0] as NormalDistribution);
            var state2 = (model.Emissions[1] as NormalDistribution);
            Assert.AreEqual(1.0, state1.Mean, 1e-10);
            Assert.AreEqual(2.0, state2.Mean, 1e-10);
            Assert.IsFalse(Double.IsNaN(state1.Mean));
            Assert.IsFalse(Double.IsNaN(state2.Mean));

            Assert.IsTrue(state1.Variance < 1e-30);
            Assert.IsTrue(state2.Variance < 1e-30);

            var A = model.LogTransitions.Exp();
            Assert.AreEqual(2, A.GetLength(0));
            Assert.AreEqual(2, A.Columns());
            Assert.AreEqual(0, A[0][0]);
            Assert.AreEqual(1, A[0][1]);
            Assert.AreEqual(1, A[1][0]);
            Assert.AreEqual(0, A[1][1]);
        }

        [Test]
        public void LearnTest9()
        {
            var observations = new double[][][]
            {
                #region example
                new double[][]
                {
                    new double[] {2.58825719356537, -6.10018078957452, -3.51826652951428,},
                    new double[] {1.5637531876564, -8.92844874836103, -9.09330631370717,},
                    new double[] {2.12242007255554, -14.8117769726059, -9.04211363915664,},
                    new double[] {0.39045587182045, -10.3548189544216, -7.69608701297759,},
                    new double[] {-0.553155690431595, -34.9185135663671, 14.6941023804174,},
                    new double[] {-0.923129916191101, -6.06337512248124, 8.28106954197084,},
                    new double[] {0.478342920541763, -4.93066650122859, 3.1120912556361,},
                },
                new double[][]
                {
                    new double[] {1.89824998378754, -8.21581113387553, -7.88790716806936,},
                    new double[] {2.24453508853912, -10.281886698766, -9.67846789539227,},
                    new double[] {0.946296751499176, -22.0276392511088, -6.52238763834787,},
                    new double[] {-0.251136720180511, -13.3010653290676, 8.47499524273859,},
                    new double[] {-2.35625505447388, -18.1542111199742, 6.25564428645639,},
                    new double[] {0.200483202934265, -5.48215328147925, 5.88811639894938,},
                },
                new double[][]
                {
                    new double[] {2.7240589261055, -3.71720542338046, -3.75092324997593,},
                    new double[] {2.19917744398117, -7.18434871865373, -4.92539999824263,},
                    new double[] {1.40723958611488, -11.5545592998714, -5.14780194932221,},
                    new double[] {1.61909088492393, -12.5262932665595, -6.34366687651826,},
                    new double[] {-2.54745036363602, -8.64924529565274, 4.15127988308386,},
                    new double[] {0.815489888191223, -33.8531051237431, 4.3954106953589,},
                    new double[] {-2.2090271115303, -7.17818258102413, 8.9117419130814,},
                    new double[] {-1.9000232219696, -2.4331659041997, 6.91224717766923,},
                },
                new double[][]
                {
                    new double[] {4.88746017217636, -4.36384651224969, -5.45526891285354,},
                    new double[] {1.07786506414413, -12.9399071692788, -5.88248026843442,},
                    new double[] {2.28888094425201, -15.4017823367163, -9.36490649113217,},
                    new double[] {-1.16468518972397, -35.4200913138333, 5.44735305966353,},
                    new double[] {-1.1483296751976, -13.5454911068913, 7.83577905727326,},
                    new double[] {-2.58188247680664, -1.10149600205281, 10.5928750605715,},
                    new double[] {-0.277529656887054, -6.96828661824016, 4.59381106840823,},
                },
                new double[][]
                {
                    new double[] {3.39118540287018, -2.9173207268871, -5.66795398530988,},
                    new double[] {1.44856870174408, -9.21319243840922, -5.74986260778932,},
                    new double[] {1.45215392112732, -10.3989582187704, -7.06932768129103,},
                    new double[] {0.640938431024551, -15.319525165245, -7.68866476960221,},
                    new double[] {-0.77500119805336, -20.8335910793105, -1.56702420087282,},
                    new double[] {-3.48337143659592, -18.0461677940976, 12.3393172987974,},
                    new double[] {-1.17014795541763, -5.59624373275155, 6.09176828712909,},
                },
                new double[][]
                {
                    new double[] {-3.984335064888, -6.2406475893692, -8.13815178201645,},
                    new double[] {-2.12110131978989, -5.60649378910647, -7.69551693188544,},
                    new double[] {-1.62762850522995, -24.1160212319193, -14.9683354815265,},
                    new double[] {-1.15231424570084, -17.1336790735458, -5.70731951079186,},
                    new double[] {0.00514835119247437, -35.4256585588532, 11.0357975880744,},
                    new double[] {0.247226655483246, -4.87705331087666, 8.47028869639136,},
                    new double[] {-1.28729045391083, -4.4684855254196, 4.45432778840328,},
                },
                new double[][]
                {
                    new double[] {-5.14926165342331, -14.4168633009146, -14.4808205022332,},
                    new double[] {-3.93681302666664, -13.6040611430423, -9.52852874304709,},
                    new double[] {-4.0200162678957, -17.9772444010218, -10.9145425003168,},
                    new double[] {2.99205146729946, -11.3995995445577, 10.0112700536762,},
                    new double[] {-1.80960297584534, -25.9626088707583, 3.84153700324761,},
                    new double[] {-0.47445073723793, -3.15995343875038, 3.81288679772555,},
                },
                new double[][]
                {
                    new double[] {-3.10730338096619, -4.90623566171983, -7.71155001801384,},
                    new double[] {-2.58265435695648, -12.8249488039327, -7.81701695282102,},
                    new double[] {-3.70455086231232, -10.9642675851383, -10.3474496036822,},
                    new double[] {2.34457105398178, -22.575668228196, -4.00681935468317,},
                    new double[] {-0.137023627758026, -22.8846781066673, 6.49448229892285,},
                    new double[] {-1.04487389326096, -10.8106353197974, 6.89123118904132,},
                    new double[] {-0.807777792215347, -6.72485967042486, 6.44026679233423,},
                    new double[] {-0.0864192843437195, -1.82784244477527, 5.21446167464657,},
                },
                new double[][]
                {
                    new double[] {-3.68375554680824, -8.91158395500054, -9.35894038244743,},
                    new double[] {-3.42774018645287, -8.90966793048099, -12.0502934183779,},
                    new double[] {-2.21796408295631, -20.1283824753482, -9.3404551995806,},
                    new double[] {0.275979936122894, -24.8898254667703, -1.95441472953041,},
                    new double[] {2.8757631778717, -25.5929744730134, 15.9213204397452,},
                    new double[] {-0.0532664358615875, -5.41014381829368, 7.0702071664098,},
                    new double[] {-0.523447245359421, -2.21351362388411, 5.47910029515575,},
                },
                new double[][]
                {
                    new double[] {-2.87790596485138, -4.67335526533981, -5.23215633615683,},
                    new double[] {-2.4156779050827, -3.99829080603495, -4.85576151355235,},
                    new double[] {-2.6987336575985, -7.76589206730162, -5.81054787011341,},
                    new double[] {-2.65482440590858, -10.5628263066491, -5.60468502395908,},
                    new double[] {-2.54620611667633, -13.0387387107748, -5.36223367466908,},
                    new double[] {-0.349991768598557, -6.54244110985515, -4.35843018634009,},
                    new double[] {1.43021196126938, -14.1423935327282, 11.3171592025544,},
                    new double[] {-0.248833745718002, -25.6880129237476, 3.6943247495434,},
                    new double[] {-0.191526114940643, -7.40986142342928, 5.01053017361167,},
                    new double[] {0.0262223184108734, -2.32355649224634, 5.02960958030255,},
                },
                new double[][]
                {
                    new double[] {-0.491838902235031, -6.14010393559236, 0.827477332024586,},
                    new double[] {-0.806065648794174, -7.15029676810841, -1.19623376104369,},
                    new double[] {-0.376655906438828, -8.79062775480082, -1.90518908829517,},
                    new double[] {0.0747844576835632, -8.78933441325732, -1.96265207353993,},
                    new double[] {-0.375023484230042, 3.89681155173501, 9.01643231817069,},
                    new double[] {-2.8106614947319, -11.460008093918, 2.27801912994775,},
                    new double[] {8.87353122234344, -36.8569805718597, 6.36432395690119,},
                    new double[] {2.17160433530808, -6.57312981892095, 6.99683358454453,},
                },
                new double[][]
                {
                    new double[] {-2.59969010949135, -3.67992698430228, 1.09594294144671,},
                    new double[] {-1.09673067927361, -5.84256216502719, -0.576662929456575,},
                    new double[] {-1.31642892956734, -7.75851355520771, -2.38379618379558,},
                    new double[] {-0.119869410991669, -8.5749576027529, -1.84393133510667,},
                    new double[] {1.6157403588295, -8.50491836461337, 1.75083250596366,},
                    new double[] {1.66225507855415, -26.4882911957686, 1.98153904369032,},
                    new double[] {2.55657434463501, -10.5098938623168, 11.632377227365,},
                    new double[] {1.91832333803177, -9.98753621777953, 7.38483383044985,},
                    new double[] {2.16058492660522, -2.7784029746222, 7.8378896386686,},
                },
#endregion
            };

            var density = new MultivariateNormalDistribution(3);
            var model = new HiddenMarkovModel<MultivariateNormalDistribution, double[]>(new Forward(5), density);

            var learning = new BaumWelchLearning<MultivariateNormalDistribution, double[]>(model)
            {
                Tolerance = 0.0001,
                Iterations = 0,
                FittingOptions = new NormalOptions() { Regularization = 0.0001 }
            };

            learning.Learn(observations);
            double logLikelihood = learning.LogLikelihood;

            Assert.IsFalse(Double.IsNaN(logLikelihood));

            foreach (double value in model.LogTransitions.ToMatrix())
                Assert.IsFalse(Double.IsNaN(value));

            foreach (double value in model.LogInitial)
                Assert.IsFalse(Double.IsNaN(value));
        }

        [Test]
        public void LearnTest10()
        {
            #region doc_learn_multivariate
            // Create sequences of vector-valued observations. In the
            // sequence below, a single observation is composed of two
            // coordinate values, such as (x, y). There seems to be two
            // states, one for (x,y) values less than (5,5) and another
            // for higher values. The states seems to be switched on
            // every observation.
            double[][][] sequences =
            {
                new double[][] // sequence 1
                {
                    new double[] { 1, 2 }, // observation 1 of sequence 1
                    new double[] { 6, 7 }, // observation 2 of sequence 1
                    new double[] { 2, 3 }, // observation 3 of sequence 1
                },
                new double[][] // sequence 2
                {
                    new double[] { 2, 2 }, // observation 1 of sequence 2
                    new double[] { 9, 8 }, // observation 2 of sequence 2
                    new double[] { 1, 0 }, // observation 3 of sequence 2
                },
                new double[][] // sequence 3
                {
                    new double[] { 1, 3 }, // observation 1 of sequence 3
                    new double[] { 8, 9 }, // observation 2 of sequence 3
                    new double[] { 3, 3 }, // observation 3 of sequence 3
                },
            };


            // Specify a initial normal distribution for the samples.
            var density = new MultivariateNormalDistribution(dimension: 2);

            // Creates a continuous hidden Markov Model with two states organized in a forward
            //  topology and an underlying univariate Normal distribution as probability density.
            var model = new HiddenMarkovModel<MultivariateNormalDistribution, double[]>(new Forward(2), density);

            // Configure the learning algorithms to train the sequence classifier until the
            // difference in the average log-likelihood changes only by as little as 0.0001
            var teacher = new BaumWelchLearning<MultivariateNormalDistribution, double[]>(model)
            {
                Tolerance = 0.0001,
                Iterations = 0,
            };

            // Fit the model
            teacher.Learn(sequences);

            double logLikelihood = teacher.LogLikelihood;

            // See the likelihood of the sequences learned
            double a1 = Math.Exp(model.LogLikelihood(new[] {
                new double[] { 1, 2 },
                new double[] { 6, 7 },
                new double[] { 2, 3 }})); // 0.000208

            double a2 = Math.Exp(model.LogLikelihood(new[] {
                new double[] { 2, 2 },
                new double[] { 9, 8  },
                new double[] { 1, 0 }})); // 0.0000376

            // See the likelihood of an unrelated sequence
            double a3 = Math.Exp(model.LogLikelihood(new[] {
                new double[] { 8, 7 },
                new double[] { 9, 8  },
                new double[] { 1, 0 }})); // 2.10 x 10^(-89)
            #endregion

            Assert.AreEqual(0.00020825319093038984, a1);
            Assert.AreEqual(0.000037671116792519834, a2, 1e-15);
            Assert.AreEqual(2.1031924118199194E-89, a3);
        }

        [Test]
        public void LearnTest10_Independent()
        {
            #region doc_learn_independent
            // Let's say we have 2 meteorological sensors gathering data
            // from different time periods of the day. Those periods are
            // represented below:

            double[][][] data =
            {
                new double[][] // first sequence (we just repeated the measurements 
                {              //  once, so there is only one observation sequence)

                    new double[] { 1, 2 }, // Day 1, 15:00 pm
                    new double[] { 6, 7 }, // Day 1, 16:00 pm
                    new double[] { 2, 3 }, // Day 1, 17:00 pm
                    new double[] { 2, 2 }, // Day 1, 18:00 pm
                    new double[] { 9, 8 }, // Day 1, 19:00 pm
                    new double[] { 1, 0 }, // Day 1, 20:00 pm
                    new double[] { 1, 3 }, // Day 1, 21:00 pm
                    new double[] { 8, 9 }, // Day 1, 22:00 pm
                    new double[] { 3, 3 }, // Day 1, 23:00 pm
                    new double[] { 1, 3 }, // Day 2, 00:00 am
                    new double[] { 1, 1 }, // Day 2, 01:00 am
                }
            };

            // Let's assume those sensors are unrelated (for simplicity). As
            // such, let's assume the data gathered from the sensors may reside
            // into circular centroids denoting each state the underlying system
            // might be in.
            NormalDistribution[] initial_components =
            {
                new NormalDistribution(), // initial value for the first variable's distribution
                new NormalDistribution()  // initial value for the second variable's distribution
            };

            // Specify a initial independent normal distribution for the samples.
            var density = new Independent<NormalDistribution, double>(initial_components);

            // Creates a continuous hidden Markov Model with two states organized in an Ergodic
            //  topology and an underlying independent Normal distribution as probability density.
            var model = new HiddenMarkovModel<Independent<NormalDistribution>, double[]>(new Ergodic(2), density);

            // Configure the learning algorithms to train the sequence classifier until the
            // difference in the average log-likelihood changes only by as little as 0.0001
            var teacher = new BaumWelchLearning<Independent<NormalDistribution>, double[]>(model)
            {
                Tolerance = 0.0001,
                Iterations = 0,
            };

            // Fit the model
            teacher.Learn(data);

            double error = teacher.LogLikelihood;

            // Get the hidden state associated with each observation
            //
            int[] hiddenStates = null; // log-likelihood of the Viterbi path
            double logLikelihood = model.LogLikelihood(data[0], ref hiddenStates);
            #endregion

            Assert.AreEqual(-33.978800850637882, error, 1e-6);
            Assert.AreEqual(-33.9788008509802, logLikelihood, 1e-6);
            Assert.AreEqual(11, hiddenStates.Length);
        }


        [Test]
        public void LearnTest11()
        {
            #region doc_learn_mixture
            // Suppose we have a set of six sequences and we would like to
            // fit a hidden Markov model with mixtures of Normal distributions
            // as the emission densities. 

            // First, let's consider a set of univariate sequences:
            double[][] sequences =
            {
                new double[] { 1, 1, 2, 2, 2, 3, 3, 3 },
                new double[] { 1, 2, 2, 2, 3, 3 },
                new double[] { 1, 2, 2, 3, 3, 5 },
                new double[] { 2, 2, 2, 2, 3, 3, 3, 4, 5, 5, 1 },
                new double[] { 1, 1, 1, 2, 2, 5 },
                new double[] { 1, 2, 2, 4, 4, 5 },
            };


            // Now we can begin specifying a initial Gaussian mixture distribution. It is
            // better to add some different initial parameters to the mixture components:
            var density = new Mixture<NormalDistribution>(
                new NormalDistribution(mean: 2, stdDev: 1.0), // 1st component in the mixture
                new NormalDistribution(mean: 0, stdDev: 0.6), // 2nd component in the mixture
                new NormalDistribution(mean: 4, stdDev: 0.4), // 3rd component in the mixture
                new NormalDistribution(mean: 6, stdDev: 1.1)  // 4th component in the mixture
            );

            // Let's then create a continuous hidden Markov Model with two states organized in a forward
            //  topology with the underlying univariate Normal mixture distribution as probability density.
            var model = new HiddenMarkovModel<Mixture<NormalDistribution>, double>(new Forward(2), density);

            // Now we should configure the learning algorithms to train the sequence classifier. We will
            // learn until the difference in the average log-likelihood changes only by as little as 0.0001
            var teacher = new BaumWelchLearning<Mixture<NormalDistribution>, double>(model)
            {
                Tolerance = 0.0001,
                Iterations = 0,

                // Note, however, that since this example is extremely simple and we have only a few
                // data points, a full-blown mixture wouldn't really be needed. Thus we will have a
                // great chance that the mixture would become degenerated quickly. We can avoid this
                // by specifying some regularization constants in the Normal distribution fitting:

                FittingOptions = new MixtureOptions()
                {
                    Iterations = 1, // limit the inner e-m to a single iteration

                    InnerOptions = new NormalOptions()
                    {
                        Regularization = 1e-5 // specify a regularization constant
                    }
                }
            };

            // Finally, we can fit the model
            teacher.Learn(sequences);

            double logLikelihood = teacher.LogLikelihood;

            // And now check the likelihood of some approximate sequences.
            double a1 = Math.Exp(model.LogLikelihood(new double[] { 1, 1, 2, 2, 3 })); // 2.3413833128741038E+45
            double a2 = Math.Exp(model.LogLikelihood(new double[] { 1, 1, 2, 5, 5 })); // 9.94607618459872E+19

            // We can see that the likelihood of an unrelated sequence is much smaller:
            double a3 = Math.Exp(model.LogLikelihood(new double[] { 8, 2, 6, 4, 1 })); // 1.5063654166181737E-44
            #endregion

            Assert.IsTrue(a1 > 1e+6);
            Assert.IsTrue(a2 > 1e+6);
            Assert.IsTrue(a3 < 1e-6);

            Assert.AreEqual(a1, 3.77323640691724E+20);
            Assert.AreEqual(a2, 23052174.733230453);
            Assert.AreEqual(a3, 1.808715235176357E-29);
        }

        [Test]
        public void LearnTest12()
        {
            #region doc_learn_mixture_regularization
            // Suppose we have a set of six sequences and we would like to
            // fit a hidden Markov model with mixtures of Normal distributions
            // as the emission densities. 

            // First, let's consider a set of univariate sequences:
            double[][] sequences =
            {
                new double[] { -0.223, -1.05, -0.574, 0.965, -0.448, 0.265, 0.087, 0.362, 0.717, -0.032 },
                new double[] { -1.05, -0.574, 0.965, -0.448, 0.265, 0.087, 0.362, 0.717, -0.032, -0.346 },
                new double[] { -0.574, 0.965, -0.448, 0.265, 0.087, 0.362, 0.717, -0.032, -0.346, -0.989 },
                new double[] { 0.965, -0.448, 0.265, 0.087, 0.362, 0.717, -0.032, -0.346, -0.989, -0.619 },
                new double[] { -0.448, 0.265, 0.087, 0.362, 0.717, -0.032, -0.346, -0.989, -0.619, 0.02 },
                new double[] { 0.265, 0.087, 0.362, 0.717, -0.032, -0.346, -0.989, -0.619, 0.02, -0.297 },
            };


            // Now we can begin specifying a initial Gaussian mixture distribution. It is
            // better to add some different initial parameters to the mixture components:
            var density = new Mixture<NormalDistribution>(
                new NormalDistribution(mean: 2, stdDev: 1.0), // 1st component in the mixture
                new NormalDistribution(mean: 0, stdDev: 0.6), // 2nd component in the mixture
                new NormalDistribution(mean: 4, stdDev: 0.4), // 3rd component in the mixture
                new NormalDistribution(mean: 6, stdDev: 1.1)  // 4th component in the mixture
            );

            // Let's then create a continuous hidden Markov Model with two states organized in a forward
            //  topology with the underlying univariate Normal mixture distribution as probability density.
            var model = new HiddenMarkovModel<Mixture<NormalDistribution>, double>(new Forward(2), density);

            // Now we should configure the learning algorithms to train the sequence classifier. We will
            // learn until the difference in the average log-likelihood changes only by as little as 0.0001
            var teacher = new BaumWelchLearning<Mixture<NormalDistribution>, double, MixtureOptions>(model)
            {
                Tolerance = 0.0001,
                Iterations = 0,

                // Note, however, that since this example is extremely simple and we have only a few
                // data points, a full-blown mixture wouldn't really be needed. Thus we will have a
                // great chance that the mixture would become degenerated quickly. We can avoid this
                // by specifying some regularization constants in the Normal distribution fitting:

                FittingOptions = new MixtureOptions()
                {
                    Iterations = 1, // limit the inner e-m to a single iteration

                    InnerOptions = new NormalOptions()
                    {
                        Regularization = 1e-5 // specify a regularization constant
                    }
                }
            };

            // Finally, we can fit the model
            teacher.Learn(sequences);

            double logLikelihood = teacher.LogLikelihood;

            // And now check the likelihood of some approximate sequences.
            double[] newSequence = { -0.223, -1.05, -0.574, 0.965, -0.448, 0.265, 0.087, 0.362, 0.717, -0.032 };
            double a1 = Math.Exp(model.LogLikelihood(newSequence)); // 11729312967893.566

            int[] path = model.Decide(newSequence);

            // We can see that the likelihood of an unrelated sequence is much smaller:
            double a3 = Math.Exp(model.LogLikelihood(new double[] { 8, 2, 6, 4, 1 })); // 0.0
            #endregion

            Assert.IsTrue(a1 > 1e+10);
            Assert.IsTrue(a3 < 1e+10);

            Assert.IsTrue(path.IsEqual(new int[] { 0, 0, 0, 0, 0, 0, 1, 1, 1, 1 }));
        }

        [Test]
        public void LearnTest_EmptySequence()
        {
            double[][] sequences =
            {
                new double[] { -0.223, -1.05, -0.574, 0.965, -0.448, 0.265, 0.087, 0.362, 0.717, -0.032 },
                new double[] { -1.05, -0.574, 0.965, -0.448, 0.265, 0.087, 0.362, 0.717, -0.032, -0.346 },
                new double[] {  },
                new double[] { 0.965, -0.448, 0.265, 0.087, 0.362, 0.717, -0.032, -0.346, -0.989, -0.619 },
                new double[] { -0.448, 0.265, 0.087, 0.362, 0.717, -0.032, -0.346, -0.989, -0.619, 0.02 },
                new double[] { 0.265, 0.087, 0.362, 0.717, -0.032, -0.346, -0.989, -0.619, 0.02, -0.297 },
            };

            var model = new HiddenMarkovModel<NormalDistribution, double>(new Ergodic(2), new NormalDistribution());

            var teacher = new BaumWelchLearning<NormalDistribution, double>(model)
            {
                Tolerance = 0.0001,
                Iterations = 0,
            };

            Assert.Throws<ArgumentException>(() => teacher.Learn(sequences), "");
        }

        [Test]
        public void LearnTest13()
        {
            var observations = new double[][][]
            {
                new double[][]
                {
                    new double[] { 2, 0 },
                    new double[] { 5, 0 },
                    new double[] { 10, 0 },
                },
                new double[][]
                {
                    new double[] { 2, 0 },
                    new double[] { 5, 0 },
                    new double[] { 10, 0 },
                },
                new double[][]
                {
                    new double[] { 2, 0 },
                    new double[] { 5, 0 },
                    new double[] { 10, 0 },
                },
                new double[][]
                {
                    new double[] { 2, 0 },
                    new double[] { 5, 0 },
                    new double[] { 10, 0 },
                },
            };

            checkDegenerate(observations, 10);

            observations = new double[][][]
            {
                new double[][]
                {
                    new double[] { 2, 1 },
                    new double[] { 5, 2 },
                    new double[] { 10, 3 },
                },
                new double[][]
                {
                    new double[] { 2, 1 },
                    new double[] { 5, 2 },
                    new double[] { 10, 3 },
                },
                new double[][]
                {
                    new double[] { 2, 1 },
                    new double[] { 5, 2 },
                    new double[] { 10, 3 },
                },
                new double[][]
                {
                    new double[] { 2, 1 },
                    new double[] { 5, 2 },
                    new double[] { 10, 3 },
                },
            };

            checkDegenerate(observations, 3);
        }

        private static void checkDegenerate(double[][][] observations, int states)
        {
            bool thrown = false;
            try
            {
                var density = new MultivariateNormalDistribution(2);
                var model = new HiddenMarkovModel<MultivariateNormalDistribution, double[]>(new Forward(states), density);

                var learning = new BaumWelchLearning<MultivariateNormalDistribution, double[]>(model)
                {
                    Tolerance = 0.0001,
                    Iterations = 0,
                };

                learning.Learn(observations);

                Assert.AreEqual(0, learning.LogLikelihood);
            }
            catch (NonPositiveDefiniteMatrixException)
            {
                thrown = true;
            }

            Assert.IsTrue(thrown);

            {
                var density = new MultivariateNormalDistribution(2);
                var model = new HiddenMarkovModel<MultivariateNormalDistribution, double[]>(new Forward(states), density);

                var learning = new BaumWelchLearning<MultivariateNormalDistribution, double[]>(model)
                {
                    Tolerance = 0.0001,
                    Iterations = 0,
                    FittingOptions = new NormalOptions() { Robust = true }
                };

                learning.Learn(observations);

                Assert.IsFalse(Double.IsNaN(learning.LogLikelihood));

                foreach (double value in model.LogTransitions.ToMatrix())
                    Assert.IsFalse(Double.IsNaN(value));

                foreach (double value in model.LogInitial)
                    Assert.IsFalse(Double.IsNaN(value));
            }
        }

        [Test]
        public void FittingOptionsTest()
        {
            // Create a degenerate problem
            double[][][] sequences = new double[][]
            {
                new double[] { 1,1,1,1,1,0,1,1,1,1 },
                new double[] { 1,1,1,1,0,1,1,1,1,1 },
                new double[] { 1,1,1,1,1,1,1,1,1,1 },
                new double[] { 1,1,1,1,1,1         },
                new double[] { 1,1,1,1,1,1,1       },
                new double[] { 1,1,1,1,1,1,1,1,1,1 },
                new double[] { 1,1,1,1,1,1,1,1,1,1 },
            }.Apply((xi, i, j) => new[] { xi });

            // Creates a continuous hidden Markov Model with two states organized in a ergodic
            //  topology and an underlying multivariate Normal distribution as density.
            var density = new MultivariateNormalDistribution(1);

            var model = new HiddenMarkovModel<MultivariateNormalDistribution, double[]>(new Ergodic(2), density);

            // Configure the learning algorithms to train the sequence classifier
            var teacher = new BaumWelchLearning<MultivariateNormalDistribution, double[]>(model)
            {
                Tolerance = 0.0001,
                Iterations = 0,

                // Configure options for fitting the normal distribution
                FittingOptions = new NormalOptions() { Regularization = 0.0001, }
            };

            // Fit the model. No exceptions will be thrown
            teacher.Learn(sequences);
            double logLikelihood = teacher.LogLikelihood;
            double likelihood = Math.Exp(logLikelihood);

            Assert.AreEqual(31.954060476891243, logLikelihood, 1e-10);

            Assert.AreEqual(0.0001, (teacher.FittingOptions as NormalOptions).Regularization);



            // Try without a regularization constant to get an exception
            bool thrown;

            thrown = false;
            density = new MultivariateNormalDistribution(1);
            model = new HiddenMarkovModel<MultivariateNormalDistribution, double[]>(new Ergodic(2), density);
            teacher = new BaumWelchLearning<MultivariateNormalDistribution, double[]>(model) { Tolerance = 0.0001, Iterations = 0, };
            Assert.IsNull(teacher.FittingOptions);
            try { teacher.Learn(sequences); }
            catch { thrown = true; }
            Assert.IsTrue(thrown);

            thrown = false;
            density = new Accord.Statistics.Distributions.Multivariate.MultivariateNormalDistribution(1);
            model = new HiddenMarkovModel<MultivariateNormalDistribution, double[]>(new Ergodic(2), density);
            teacher = new BaumWelchLearning<MultivariateNormalDistribution, double[]>(model)
            {
                Tolerance = 0.0001,
                Iterations = 0,
                FittingOptions = new NormalOptions() { Regularization = 0 }
            };
            Assert.IsNotNull(teacher.FittingOptions);
            try { teacher.Learn(sequences); }
            catch { thrown = true; }
            Assert.IsTrue(thrown);
        }

        [Test]
        public void GenerateTest()
        {
            double[,] A;
            double[] pi;

            A = new double[,]
            {
                { 0.7, 0.3 },
                { 0.4, 0.6 }
            };

            GeneralDiscreteDistribution[] B =
            {
                new GeneralDiscreteDistribution(0.1, 0.4, 0.5),
                new GeneralDiscreteDistribution(0.6, 0.3, 0.1)
            };

            pi = new double[]
            {
                0.6, 0.4
            };

            var hmm = new HiddenMarkovModel<GeneralDiscreteDistribution, double>(A, B, pi);


            double logLikelihood;
            int[] path;
            double[] samples = (double[])hmm.Generate(10, out path, out logLikelihood);

            double expected = hmm.Evaluate(samples, path);

            Assert.AreEqual(expected, logLikelihood);
        }

        [Test]
        public void PosteriorTest1()
        {
            // Example from http://ai.stanford.edu/~serafim/CS262_2007/notes/lecture5.pdf

            double[,] A =
            {
                { 0.95, 0.05 }, // fair dice state
                { 0.05, 0.95 }, // loaded dice state
            };

            double[,] B =
            {
                { 1 /  6.0, 1 /  6.0, 1 /  6.0, 1 /  6.0, 1 /  6.0, 1 / 6.0 }, // fair dice probabilities
                { 1 / 10.0, 1 / 10.0, 1 / 10.0, 1 / 10.0, 1 / 10.0, 1 / 2.0 }, // loaded probabilities
            };

            double[] pi = { 0.5, 0.5 };

            var hmm = new HiddenMarkovModel<GeneralDiscreteDistribution, int>(A, GeneralDiscreteDistribution.FromMatrix(B), pi);

            int[] x = new int[] { 1, 2, 1, 5, 6, 2, 1, 5, 2, 4 }.Subtract(1);
            int[] y = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            double py = Math.Exp(hmm.Evaluate(x, y));

            Assert.AreEqual(0.00000000521158647211, py, 1e-16);

            x = new int[] { 1, 2, 1, 5, 6, 2, 1, 5, 2, 4 }.Subtract(1);
            y = new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            py = Math.Exp(hmm.Evaluate(x, y));

            Assert.AreEqual(0.00000000015756235243, py, 1e-16);


            Accord.Math.Random.Generator.Seed = 0;
            var u = new UniformDiscreteDistribution(0, 6);

            int[] sequence = u.Generate(1000);
            int start = 120;
            int end = 150;
            for (int i = start; i < end; i += 2)
                sequence[i] = 5;


            // Predict the next observation in sequence
            int[] path;
            double[][] p = hmm.Posterior(sequence, out path);

            for (int i = 0; i < path.Length; i++)
                Assert.AreEqual(1, p[i][0] + p[i][1], 1e-10);


            int loaded = 0;
            for (int i = 0; i < start; i++)
                if (p[i][1] > 0.95)
                    loaded++;

            Assert.AreEqual(0, loaded);

            loaded = 0;
            for (int i = start; i < end; i++)
                if (p[i][1] > 0.95)
                    loaded++;

            Assert.IsTrue(loaded > 15);

            loaded = 0;
            for (int i = end; i < p.Length; i++)
                if (p[i][1] > 0.95)
                    loaded++;

            Assert.AreEqual(0, loaded);
        }

        [Test]
        public void learn_predict()
        {
            #region doc_predict
            // We will try to create a Hidden Markov Model which
            // can recognize (and predict) the following sequences:
            double[][] sequences =
            {
                new double[] { 1, 3, 5, 7, 9, 11, 13 },
                new double[] { 1, 3, 5, 7, 9, 11 },
                new double[] { 1, 3, 5, 7, 9, 11, 13 },
                new double[] { 1, 3, 3, 7, 7, 9, 11, 11, 13, 13 },
                new double[] { 1, 3, 7, 9, 11, 13 },
            };

            // Create a Baum-Welch HMM algorithm:
            var teacher = new BaumWelchLearning<NormalDistribution, double, NormalOptions>()
            {
                // Let's creates a left-to-right (forward)
                // Hidden Markov Model with 7 hidden states
                Topology = new Forward(7),

                FittingOptions = new NormalOptions()
                {
                    Regularization = 1e-8
                },

                // We'll try to fit the model to the data until the difference in
                // the average log-likelihood changes only by as little as 0.0001
                Tolerance = 0.0001,
                Iterations = 0 // do not impose a limit on the number of iterations
            };

            // Use the algorithm to learn a new Markov model:
            HiddenMarkovModel<NormalDistribution, double> hmm = teacher.Learn(sequences);

            // Now, we will try to predict the next 1 observation in a base symbol sequence
            double[] prediction = hmm.Predict(observations: new double[] { 1, 3, 5, 7, 9 }, next: 1);

            // At this point, prediction should be around double[] { 11.909090909090905 }
            double nextObservation = prediction[0]; // should be comparatively near 11.
            #endregion

            Assert.AreEqual(prediction.Length, 1);
            Assert.AreEqual(11.909090909090905, prediction[0], 1e-6);
        }
    }
}
