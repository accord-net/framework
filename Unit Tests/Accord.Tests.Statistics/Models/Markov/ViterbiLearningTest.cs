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
    using Accord.Statistics.Models.Markov.Topology;

    [TestFixture]
    public class ViterbiLearningTest
    {


        [Test]
        public void LearnTest2()
        {
            Accord.Math.Tools.SetupGenerator(0);
            int[][] sequences = new int[500][];
            for (int i = 0; i < sequences.Length; i++)
            {
                sequences[i] = new int[Accord.Math.Tools.Random.Next(20, 80)];

                int start = Accord.Math.Tools.Random.Next();

                for (int j = 0; j < sequences[i].Length; j++)
                {
                    double s = Math.Sin(j + start);
                    double u = ((s + 1) / 2.0);
                    sequences[i][j] = (int)(u * 10);
                }
            }

            HiddenMarkovModel hmm1;
            double ll1;

            {
                Accord.Math.Tools.SetupGenerator(0);
                hmm1 = new HiddenMarkovModel(10, 10, true);
                var teacher = new ViterbiLearning(hmm1)
                {
                    Iterations = 1,
                    Tolerance = 1e-15,
                    Batches = 1,
                    UseLaplaceRule = true
                };
                ll1 = teacher.Run(sequences);
            }

            HiddenMarkovModel hmm10;
            double ll10;

            {
                Accord.Math.Tools.SetupGenerator(0);
                hmm10 = new HiddenMarkovModel(10, 10, true);

                var teacher = new ViterbiLearning(hmm10)
                {
                    Iterations = 100,
                    Tolerance = 1e-15,
                    Batches = 1,
                    UseLaplaceRule = true
                };

                ll10 = teacher.Run(sequences);
            }

            Assert.IsTrue(ll10 > ll1);
            Assert.IsTrue(Math.Abs(ll1 - ll10) > 10);

            // Those results must match the ones in ViterbiLearningTest`1.
            Assert.AreEqual(-33.834836461044411, ll1);
            Assert.AreEqual(-23.362967205628703, ll10);

            Assert.IsFalse(AreEqual(hmm1, hmm10));
        }

        private static bool AreEqual(HiddenMarkovModel hmm1, HiddenMarkovModel hmm10)
        {
            for (int i = 0; i < hmm1.States; i++)
            {
                if (hmm1.Probabilities[i] != hmm10.Probabilities[i])
                    return false;

                for (int j = 0; j < hmm1.States; j++)
                    if (hmm1.Transitions[i, j] != hmm10.Transitions[i, j])
                        return false;

                for (int j = 0; j < hmm1.Symbols; j++)
                    if (hmm1.Emissions[i, j] != hmm10.Emissions[i, j])
                        return false;
            }

            return true;
        }

        [Test]
        public void LearnTest4()
        {
            Accord.Math.Tools.SetupGenerator(0);

            int[][] sequences = new int[][] 
            {
                new int[] { 0, 3, 1 },
                new int[] { 0, 2 },
                new int[] { 1, 0, 3 },
                new int[] { 3, 4 },
                new int[] { 0, 1, 3, 5 },
                new int[] { 0, 3, 4 },
                new int[] { 0, 1, 3, 5 },
                new int[] { 0, 1, 3, 5 },
                new int[] { 0, 1, 3, 4, 5 },
            };

            HiddenMarkovModel hmm = new HiddenMarkovModel(new Ergodic(3, random: true), 6);

            var teacher = new ViterbiLearning(hmm) { Iterations = 100, Tolerance = 0 };

            double ll = teacher.Run(sequences);

            double l0; hmm.Decode(sequences[0], out l0);
            double l1; hmm.Decode(sequences[1], out l1);
            double l2; hmm.Decode(sequences[2], out l2);

            double pl = System.Math.Exp(ll);
            double p0 = System.Math.Exp(l0);
            double p1 = System.Math.Exp(l1);
            double p2 = System.Math.Exp(l2);

            Assert.AreEqual(0.078050218613091762, pl, 1e-10);
            Assert.AreEqual(0.008509757587448558, p0, 1e-10);
            Assert.AreEqual(0.010609567901234561, p1, 1e-10);
            Assert.AreEqual(0.008509757587448558, p2, 1e-10);
        }

        [Test]
        public void LearnTest()
        {
            Accord.Math.Tools.SetupGenerator(0);

            HiddenMarkovModel hmm = new HiddenMarkovModel(new Ergodic(2), 3);

            int[] observation = new int[]
            { 
                0,1,1,2,2,1,1,1,0,0,0,0,0,0,0,0,2,2,0,0,1,1,1,2,0,0,
                0,0,0,0,1,2,1,1,1,0,2,0,1,0,2,2,2,0,0,2,0,1,2,2,0,1,
                1,2,2,2,0,0,1,1,2,2,0,0,2,2,0,0,1,0,1,2,0,0,0,0,2,0,
                2,0,1,1,0,1,0,0,0,1,2,1,1,2,0,2,0,2,2,0,0,1
            };

            int[] observation2 = new int[]
            {
                0,1,0,0,2,1,1,0,0,2,1,0,1,1,2,0,1,1,1,0,0,2,0,0,2,1,
                1,1,2,0,2,2,1,0,1,2,0,2,1,0,2,1,1,2,0,1,0,1,1,0,1,2,
                1,0,2,0,1,0,1,2,0,0,2,0,2,0,0,1,0,0,0,0,1,1,2,2,1,2,
                0,1,1,1,2,2,1,1,1,2,2,0,2,1,1,2,0,0,1,1,1,1,1,1,1,0,
                0,1,0,1,0,1,0,0,2,0,1,0,2,0,0,0,0,1,1,1,1,1,1,0,2,0,
                2,2,1,2,1,2,1,0,2,1,1,2,1,2,1,0,0,2,0,0,2,2,2,0,0,1,
                0,1,0,1,0,1,0,0,0,0,0,1,1,1,2,0,0,0,0,0,0,2,2,0,0,0,
                0,0,1,0,2,2,2,2,2,1,2,0,1,0,1,2,2,1,0,1,1,2,1,1,1,2,
                2,2,0,1,1,1,1,2,1,0,1,0,1,1,0,2,2,2,1,1,1,1,0,2,1,0,
                2,1,1,1,2,0,0,1,1,1,1,2,1,1,2,0,0,0,0,0,2,2,2,0,1,1,
                1,0,1,0,0,0,0,2,2,2,2,0,1,1,0,1,2,1,2,1,1,0,0,0,0,2,
                2,1,1,0,1,0,0,0,0,1,0,0,0,2,0,0,0,2,1,2,2,0,0,0,0,0,
                0,2,0,0,2,0,0,0,2,0,1,1,2,2,1,2,1,2,0,0,0,0,2,0,2,0,
                1,0,0,2,2,1,2,1,2,2,0,1,1,1,0,0,1,1,1,2,1,0,0,2,0,0,
                0,0,1,2,0,0,1,2,0,0,0,2,1,1,1,1,1,2,2,0,0,1,1,1,0,0,
                2,0,1,1,0,2,2,0,0,0,1,1,1,1,1,1,2,1,1,0,2,0,0,0,1,1,
                1,2,1,0,0,0,1,1,0,1,1,1,0,0,0,1,1,1,2,2,2,0,2,0,2,1,
                2,1,0,2,1,2,1,0,0,2,1,1,1,1,0,0,0,1,2,0,2,2,1,2,1,1,
                1,0,1,0,0,0,0,2,0,1,1,1,0,2,0,1,0,2,1,2,2,0,2,1,0,0,
                2,1,2,2,0,2,1,2,1,2,0,0,0,1,2,1,2,2,1,0,0,0,1,1,2,0,
                2,1,0,0,0,1,0,0,1,2,0,0,1,2,2,2,0,1,2,0,1,0,1,0,2,2,
                0,2,0,1,1,0,1,1,1,2,2,0,0,0,0,0,1,1,0,0,2,0,0,1,0,0,
                1,0,2,1,1,1,1,1,2,0,0,2,0,1,2,0,1,1,1,2,0,0,0,1,2,0,
                0,0,2,2,1,1,1,0,1,1,0,2,2,0,1,2,2,1,1,1,2,1,0,2,0,0,
                1,1,1,1,1,1,2,1,2,1,0,1,0,2,2,0,1,2,1,1,2,1,0,1,2,1
            };


            var teacher = new ViterbiLearning(hmm)
            {
                Iterations = 650,
                Tolerance = 0
            };

            double ll = teacher.Run(observation);


            double[] pi = { 0.66, 0.33 };

            double[,] A =
            {
                { 0.99, 0.01 },
                { 0.50, 0.50 }
            };

            double[,] B =
            {
                { 0.44, 0.27, 0.28 },
                { 0.33, 0.33, 0.33 }
            };


            var hmmA = Matrix.Exp(hmm.Transitions);
            var hmmB = Matrix.Exp(hmm.Emissions);
            var hmmP = Matrix.Exp(hmm.Probabilities);


            Assert.IsTrue(Matrix.IsEqual(A, hmmA, 0.1));
            Assert.IsTrue(Matrix.IsEqual(B, hmmB, 0.1));
            Assert.IsTrue(Matrix.IsEqual(pi, hmmP, 0.1));
        }

        [Test]
        public void LearnTest3()
        {
            Accord.Math.Tools.SetupGenerator(0);

            // We will try to create a Hidden Markov Model which
            //  can detect if a given sequence starts with a zero
            //  and has any number of ones after that.
            //
            int[][] sequences = new int[][] 
            {
                new int[] { 0,1,1,1,1,0,1,1,1,1 },
                new int[] { 0,1,1,1,0,1,1,1,1,1 },
                new int[] { 0,1,1,1,1,1,1,1,1,1 },
                new int[] { 0,1,1,1,1,1         },
                new int[] { 0,1,1,1,1,1,1       },
                new int[] { 0,1,1,1,1,1,1,1,1,1 },
                new int[] { 0,1,1,1,1,1,1,1,1,1 },
            };

            // Creates a new Hidden Markov Model with 3 states for
            //  an output alphabet of two characters (zero and one)
            //
            HiddenMarkovModel hmm = new HiddenMarkovModel(new Forward(3), 2);

            // Try to fit the model to the data until the difference in
            //  the average log-likelihood changes only by as little as 0.0001
            //
            var teacher = new ViterbiLearning(hmm) { Tolerance = 0.0001, Iterations = 0 };

            //
            double ll = teacher.Run(sequences);

            // Calculate the probability that the given
            //  sequences originated from the model
            //
            double l1; hmm.Decode(new int[] { 0, 1 }, out l1);        // 0.5394
            double l2; hmm.Decode(new int[] { 0, 1, 1, 1 }, out l2);  // 0.4485

            // Sequences which do not start with zero have much lesser probability.
            double l3; hmm.Decode(new int[] { 1, 1 }, out l3);        // 0.0864
            double l4; hmm.Decode(new int[] { 1, 0, 0, 0 }, out l4);  // 0.0004


            // Sequences which contains few errors have higher probability
            //  than the ones which do not start with zero. This shows some
            //  of the temporal elasticity and error tolerance of the HMMs.
            //
            double l5; hmm.Decode(new int[] { 0, 1, 0, 1, 1, 1, 1, 1, 1 }, out l5); // 0.0154
            double l6; hmm.Decode(new int[] { 0, 1, 1, 1, 1, 1, 1, 0, 1 }, out l6); // 0.0154


            ll = System.Math.Exp(ll);
            l1 = System.Math.Exp(l1);
            l2 = System.Math.Exp(l2);
            l3 = System.Math.Exp(l3);
            l4 = System.Math.Exp(l4);
            l5 = System.Math.Exp(l5);
            l6 = System.Math.Exp(l6);

            Assert.AreEqual(1.754393540912413, ll, 1e-6);
            Assert.AreEqual(0.53946360153256712, l1, 1e-6);
            Assert.AreEqual(0.44850249229903377, l2, 1e-6);
            Assert.AreEqual(0.08646414524833077, l3, 1e-6);
            Assert.AreEqual(0.00041152263374485, l4, 1e-6);
            Assert.AreEqual(0.01541807695931400, l5, 1e-6);
            Assert.AreEqual(0.01541807695931400, l6, 1e-6);

            Assert.IsTrue(l1 > l3 && l1 > l4);
            Assert.IsTrue(l2 > l3 && l2 > l4);
        }

        [Test]
        public void learn_test()
        {
            #region doc_learn
            Accord.Math.Random.Generator.Seed = 0;

            // We will try to create a Hidden Markov Model which
            //  can detect if a given sequence starts with a zero
            //  and has any number of ones after that.
            //
            int[][] sequences = new int[][] 
            {
                new int[] { 0,1,1,1,1,0,1,1,1,1 },
                new int[] { 0,1,1,1,0,1,1,1,1,1 },
                new int[] { 0,1,1,1,1,1,1,1,1,1 },
                new int[] { 0,1,1,1,1,1         },
                new int[] { 0,1,1,1,1,1,1       },
                new int[] { 0,1,1,1,1,1,1,1,1,1 },
                new int[] { 0,1,1,1,1,1,1,1,1,1 },
            };

            // Creates a new Hidden Markov Model with 3 states for
            //  an output alphabet of two characters (zero and one)
            //
            HiddenMarkovModel hmm = new HiddenMarkovModel(new Forward(3), 2);

            // Try to fit the model to the data until the difference in
            //  the average log-likelihood changes only by as little as 0.0001
            //
            var teacher = new ViterbiLearning(hmm)
            {
                Tolerance = 0.0001,
                Iterations = 0
            };

            // Learn the model
            teacher.Learn(sequences);

            // Calculate the probability that the given
            //  sequences originated from the model
            //
            double l1; hmm.Decode(new int[] { 0, 1 }, out l1);        // 0.5394
            double l2; hmm.Decode(new int[] { 0, 1, 1, 1 }, out l2);  // 0.4485

            // Sequences which do not start with zero have much lesser probability.
            double l3; hmm.Decode(new int[] { 1, 1 }, out l3);        // 0.0864
            double l4; hmm.Decode(new int[] { 1, 0, 0, 0 }, out l4);  // 0.0004

            // Sequences which contains few errors have higher probability
            //  than the ones which do not start with zero. This shows some
            //  of the temporal elasticity and error tolerance of the HMMs.
            //
            double l5; hmm.Decode(new int[] { 0, 1, 0, 1, 1, 1, 1, 1, 1 }, out l5); // 0.0154
            double l6; hmm.Decode(new int[] { 0, 1, 1, 1, 1, 1, 1, 0, 1 }, out l6); // 0.0154
            #endregion


            l1 = System.Math.Exp(l1);
            l2 = System.Math.Exp(l2);
            l3 = System.Math.Exp(l3);
            l4 = System.Math.Exp(l4);
            l5 = System.Math.Exp(l5);
            l6 = System.Math.Exp(l6);

            Assert.AreEqual(0.53946360153256712, l1, 1e-6);
            Assert.AreEqual(0.44850249229903377, l2, 1e-6);
            Assert.AreEqual(0.08646414524833077, l3, 1e-6);
            Assert.AreEqual(0.00041152263374485, l4, 1e-6);
            Assert.AreEqual(0.01541807695931400, l5, 1e-6);
            Assert.AreEqual(0.01541807695931400, l6, 1e-6);

            Assert.IsTrue(l1 > l3 && l1 > l4);
            Assert.IsTrue(l2 > l3 && l2 > l4);
        }


        [Test]
        public void LearnTest6()
        {
            Accord.Math.Tools.SetupGenerator(0);

            // We will try to create a Hidden Markov Model which
            //  can detect if a given sequence starts with a zero
            //  and has any number of ones after that.
            //
            int[][] sequences = new int[][] 
            {
                new int[] { 0,1,1,1,1,0,1,1,1,1 },
                new int[] { 0,1,1,1,0,1,1,1,1,1 },
                new int[] { 0,1,1,1,1,1,1,1,1,1 },
                new int[] { 0,1,1,1,1,1         },
                new int[] { 0,1,1,1,1,1,1       },
                new int[] { 0,1,1,1,1,1,1,1,1,1 },
                new int[] { 0,1,1,1,1,1,1,1,1,1 },
            };

            // Creates a new Hidden Markov Model with 3 states for
            //  an output alphabet of two characters (zero and one)
            //
            HiddenMarkovModel hmm = new HiddenMarkovModel(new Forward(3), 2);

            // Try to fit the model to the data until the difference in
            //  the average log-likelihood changes only by as little as 0.0001
            //
            var teacher = new ViterbiLearning(hmm) { Tolerance = 0.0001, Iterations = 0 };

            double ll = teacher.Run(sequences);


            // Calculate the probability that the given
            //  sequences originated from the model
            double l1 = hmm.Evaluate(new int[] { 0, 1 });       // 0.613
            double l2 = hmm.Evaluate(new int[] { 0, 1, 1, 1 }); // 0.500


            // Sequences which do not start with zero have much lesser probability.
            double l3 = hmm.Evaluate(new int[] { 1, 1 });       // 0.186
            double l4 = hmm.Evaluate(new int[] { 1, 0, 0, 0 }); // 0.003

            // Sequences which contains few errors have higher probability
            //  than the ones which do not start with zero. This shows some
            //  of the temporal elasticity and error tolerance of the HMMs.
            //
            double l5 = hmm.Evaluate(new int[] { 0, 1, 0, 1, 1, 1, 1, 1, 1 }); // 0.033
            double l6 = hmm.Evaluate(new int[] { 0, 1, 1, 1, 1, 1, 1, 0, 1 }); // 0.026


            double pl = System.Math.Exp(ll);
            double p1 = System.Math.Exp(l1);
            double p2 = System.Math.Exp(l2);
            double p3 = System.Math.Exp(l3);
            double p4 = System.Math.Exp(l4);
            double p5 = System.Math.Exp(l5);
            double p6 = System.Math.Exp(l6);

            Assert.AreEqual(1.754393540912413, pl, 1e-6);
            Assert.AreEqual(0.61368718756104801, p1, 1e-6);
            Assert.AreEqual(0.50049466955818356, p2, 1e-6);
            Assert.AreEqual(0.18643340385264684, p3, 1e-6);
            Assert.AreEqual(0.00300262431355424, p4, 1e-6);
            Assert.AreEqual(0.03338686211012481, p5, 1e-6);
            Assert.AreEqual(0.02659161933179825, p6, 1e-6);

            Assert.IsTrue(l1 > l3 && l1 > l4);
            Assert.IsTrue(l2 > l3 && l2 > l4);
        }

    }
}
