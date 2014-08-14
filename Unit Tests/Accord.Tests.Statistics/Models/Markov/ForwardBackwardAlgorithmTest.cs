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

namespace Accord.Tests.Statistics.Models.Markov
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Statistics.Models.Markov;
    using Accord.Math;
    using System;
    using Accord.Statistics.Distributions.Univariate;

    [TestClass()]
    public class ForwardBackwardAlgorithmTest
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



        public static HiddenMarkovModel CreateModel1()
        {
            // http://homepages.ulb.ac.be/~dgonze/TEACHING/viterbi2.pdf

            double[] initial = { 0.5, 0.5 };

            double[,] transitions = 
            {
                { 0.5, 0.5 },
                { 0.4, 0.6 },
            };

            double[,] emissions =
            {
                //         A    C    G    T
                /* H */ { 0.2, 0.3, 0.3, 0.2 },
                /* L */ { 0.3, 0.2, 0.2, 0.3 },
            };

            return new HiddenMarkovModel(transitions, emissions, initial);
        }

        public static HiddenMarkovModel CreateModel2()
        {
            // http://www.indiana.edu/~iulg/moss/hmmcalculations.pdf

            //                    s     t
            double[] initial = { 0.85, 0.15 };

            double[,] transitions = 
            {
                //         s    t
                /* s */ { 0.3, 0.7 },
                /* t */ { 0.1, 0.9 },
            };

            double[,] emissions =
            {
                //         A    B
                /* s */ { 0.4, 0.6 },
                /* t */ { 0.5, 0.5 },
            };

            return new HiddenMarkovModel(transitions, emissions, initial);
        }

        public static HiddenMarkovModel CreateModel3()
        {
            // Tested against R's HMM package
            // http://cran.r-project.org/web/packages/HMM/HMM.pdf

            // hmm = initHMM(c("A","B"), c("L","R"), 
            //          startProbs   = c(.9,.1), 
            //          transProbs   = matrix(c(.8,.3,.2,.7),2),
            //          emissionProbs= matrix(c(.75,.11,.25,.88),2))

            //                    A    B
            double[] initial = { 0.9, 0.1 };

            double[,] transitions = 
            {
                //      to:  A    B
                // from:
                /*  A  */ { 0.8, 0.2 },
                /*  B  */ { 0.3, 0.7 },
            };

            double[,] emissions =
            {
                //           L    R
                /*  A  */ { 0.75, 0.25 },
                /*  B  */ { 0.11, 0.88 },
            };

            return new HiddenMarkovModel(transitions, emissions, initial);
        }

        public static HiddenMarkovModel<GeneralDiscreteDistribution> CreateModel4()
        {
            double[] initial = { 0.5, 0.5 };

            double[,] transitions = 
            {
                { 0.5, 0.5 },
                { 0.4, 0.6 },
            };

            double[,] emissions =
            {
                //         A    C    G    T
                /* H */ { 0.2, 0.3, 0.3, 0.2 },
                /* L */ { 0.3, 0.2, 0.2, 0.3 },
            };

            return HiddenMarkovModel.CreateGeneric(transitions, emissions, initial);
        }


        [TestMethod()]
        public void BackwardTest()
        {
            HiddenMarkovModel hmm = CreateModel2();

            //                     A  B  B  A
            int[] observations = { 0, 1, 1, 0 };

            double logLikelihood;
            double[,] actual = ForwardBackwardAlgorithm.Backward(hmm, observations, out logLikelihood);

            var A = Matrix.Exp(hmm.Transitions);
            var B = Matrix.Exp(hmm.Emissions);
            var P = Matrix.Exp(hmm.Probabilities);

            double a30 = 1;
            double a31 = 1;

            double a20 = A[0, 0] * B[0, 0] * a30 + A[0, 1] * B[1, 0] * a31;
            double a21 = A[1, 0] * B[0, 0] * a30 + A[1, 1] * B[1, 0] * a31;

            double a10 = A[0, 0] * B[0, 1] * a20 + A[0, 1] * B[1, 1] * a21;
            double a11 = A[1, 0] * B[0, 1] * a20 + A[1, 1] * B[1, 1] * a21;

            double a00 = A[0, 0] * B[0, 1] * a10 + A[0, 1] * B[1, 1] * a11;
            double a01 = A[1, 0] * B[0, 1] * a10 + A[1, 1] * B[1, 1] * a11;


            Assert.AreEqual(actual[0, 0], a00);
            Assert.AreEqual(actual[0, 1], a01);

            Assert.AreEqual(actual[1, 0], a10);
            Assert.AreEqual(actual[1, 1], a11);

            Assert.AreEqual(actual[2, 0], a20);
            Assert.AreEqual(actual[2, 1], a21);

            Assert.AreEqual(actual[3, 0], a30);
            Assert.AreEqual(actual[3, 1], a31);


            double p = 0;
            for (int i = 0; i < hmm.States; i++)
                p += actual[0, i] * P[i] * B[i, observations[0]];

            Assert.AreEqual(0.054814695, p, 1e-8);
            Assert.IsFalse(double.IsNaN(p));

            p = System.Math.Exp(logLikelihood);
            Assert.AreEqual(0.054814695, p, 1e-8);
            Assert.IsFalse(double.IsNaN(p));
        }

        [TestMethod()]
        public void BackwardTest2()
        {
            HiddenMarkovModel hmm = CreateModel3();

            //                     L  L  R  R
            int[] observations = { 0, 0, 1, 1 };

            double[,] actual = ForwardBackwardAlgorithm.Backward(hmm, observations);

            // Backward matrices from R's HMM package are
            // transposed in relation to the framework's:

            Assert.AreEqual(4, actual.GetLength(0));
            Assert.AreEqual(2, actual.GetLength(1));

            Assert.AreEqual(0.128074432, actual[0, 0], 1e-10);
            Assert.AreEqual(0.07923051, actual[0, 1], 1e-8);

            Assert.AreEqual(0.196816, actual[1, 0]);
            Assert.AreEqual(0.453856, actual[1, 1]);

            Assert.AreEqual(0.376, actual[2, 0]);
            Assert.AreEqual(0.691, actual[2, 1]);

            Assert.AreEqual(1, actual[3, 0]);
            Assert.AreEqual(1, actual[3, 1]);

            foreach (double p in actual)
                Assert.IsFalse(double.IsNaN(p));
        }

        [TestMethod()]
        public void ForwardTest()
        {
            HiddenMarkovModel hmm = CreateModel1();

            //                     G  G  C  A
            int[] observations = { 2, 2, 1, 0 };

            double[,] actual = ForwardBackwardAlgorithm.Forward(hmm, observations);

            var P = Matrix.Exp(hmm.Probabilities);
            var A = Matrix.Exp(hmm.Transitions);
            var B = Matrix.Exp(hmm.Emissions);

            double a00 = P[0] * B[0, 2];
            double a01 = P[1] * B[1, 2];

            double a10 = (a00 * A[0, 0] + a01 * A[1, 0]) * B[0, 2];
            double a11 = (a01 * A[1, 1] + a00 * A[0, 1]) * B[1, 2];

            double a20 = (a10 * A[0, 0] + a11 * A[1, 0]) * B[0, 1];
            double a21 = (a11 * A[1, 1] + a10 * A[0, 1]) * B[1, 1];

            double a30 = (a20 * A[0, 0] + a21 * A[1, 0]) * B[0, 0];
            double a31 = (a21 * A[1, 1] + a20 * A[0, 1]) * B[1, 0];

            Assert.AreEqual(a00, actual[0, 0]);
            Assert.AreEqual(a01, actual[0, 1]);

            Assert.AreEqual(a10, actual[1, 0]);
            Assert.AreEqual(a11, actual[1, 1]);

            Assert.AreEqual(a20, actual[2, 0]);
            Assert.AreEqual(a21, actual[2, 1]);

            Assert.AreEqual(a30, actual[3, 0]);
            Assert.AreEqual(a31, actual[3, 1]);

            double p = 0;
            for (int i = 0; i < hmm.States; i++)
                p += actual[observations.Length - 1, i];

            Assert.AreEqual(0.00384315, p, 1e-8);
            Assert.IsFalse(double.IsNaN(p));
        }

        [TestMethod()]
        public void ForwardTest2()
        {
            HiddenMarkovModel hmm = CreateModel2();

            //                     A  B  B  A
            int[] observations = { 0, 1, 1, 0 };

            double[,] actual = ForwardBackwardAlgorithm.Forward(hmm, observations);

            var P = Matrix.Exp(hmm.Probabilities);
            var A = Matrix.Exp(hmm.Transitions);
            var B = Matrix.Exp(hmm.Emissions);

            double a00 = P[0] * B[0, 0];
            double a01 = P[1] * B[1, 0];

            //   a00 = System.Math.Round(a00, 2);
            //   a01 = System.Math.Round(a01, 2);

            double a10 = (a00 * A[0, 0] + a01 * A[1, 0]) * B[0, 1];
            double a11 = (a01 * A[1, 1] + a00 * A[0, 1]) * B[1, 1];

            double a20 = (a10 * A[0, 0] + a11 * A[1, 0]) * B[0, 1];
            double a21 = (a11 * A[1, 1] + a10 * A[0, 1]) * B[1, 1];

            double a30 = (a20 * A[0, 0] + a21 * A[1, 0]) * B[0, 0];
            double a31 = (a21 * A[1, 1] + a20 * A[0, 1]) * B[1, 0];


            Assert.AreEqual(a00, actual[0, 0]);
            Assert.AreEqual(a01, actual[0, 1]);

            Assert.AreEqual(a10, actual[1, 0]);
            Assert.AreEqual(a11, actual[1, 1]);

            Assert.AreEqual(a20, actual[2, 0]);
            Assert.AreEqual(a21, actual[2, 1]);

            Assert.AreEqual(a30, actual[3, 0]);
            Assert.AreEqual(a31, actual[3, 1]);

            double p = 0;
            for (int i = 0; i < hmm.States; i++)
                p += actual[observations.Length - 1, i];

            Assert.AreEqual(0.054814695, p, 1e-8);
            Assert.IsFalse(double.IsNaN(p));
        }

        [TestMethod()]
        public void ForwardTest3()
        {
            HiddenMarkovModel hmm = CreateModel3();

            //                     L  L  R  R
            int[] observations = { 0, 0, 1, 1 };

            double[,] actual = ForwardBackwardAlgorithm.Forward(hmm, observations);

            // Forward matrices from R's HMM package are
            // transposed in relation to the framework's:

            Assert.AreEqual(4, actual.GetLength(0));
            Assert.AreEqual(2, actual.GetLength(1));

            Assert.AreEqual(0.675, actual[0, 0], 1e-10);
            Assert.AreEqual(0.011, actual[0, 1], 1e-10);

            Assert.AreEqual(0.407475, actual[1, 0], 1e-10);
            Assert.AreEqual(0.015697, actual[1, 1], 1e-10);

            Assert.AreEqual(0.08267228, actual[2, 0], 1e-8);
            Assert.AreEqual(0.08138495, actual[2, 1], 1e-8);

            Assert.AreEqual(0.02263833, actual[3, 0], 1e-8);
            Assert.AreEqual(0.06468345, actual[3, 1], 1e-8);

            foreach (double p in actual)
                Assert.IsFalse(double.IsNaN(p));
        }

        [TestMethod()]
        public void ForwardScalingTest()
        {
            HiddenMarkovModel hmm = CreateModel1();

            var P = Matrix.Exp(hmm.Probabilities);
            var A = Matrix.Exp(hmm.Transitions);
            var B = Matrix.Exp(hmm.Emissions);

            //                     G  G  C  A
            int[] observations = { 2, 2, 1, 0 };

            double[] scaling;
            double logLikelihood;

            double[,] actual = ForwardBackwardAlgorithm.Forward(hmm, observations, out scaling, out logLikelihood);

            double a00 = P[0] * B[0, 2];
            double a01 = P[1] * B[1, 2];
            double t0 = a00 + a01;

            a00 /= t0;
            a01 /= t0;

            double a10 = (a00 * A[0, 0] + a01 * A[1, 0]) * B[0, 2];
            double a11 = (a01 * A[1, 1] + a00 * A[0, 1]) * B[1, 2];
            double t1 = a10 + a11;

            a10 /= t1;
            a11 /= t1;

            double a20 = (a10 * A[0, 0] + a11 * A[1, 0]) * B[0, 1];
            double a21 = (a11 * A[1, 1] + a10 * A[0, 1]) * B[1, 1];
            double t2 = a20 + a21;

            a20 /= t2;
            a21 /= t2;

            double a30 = (a20 * A[0, 0] + a21 * A[1, 0]) * B[0, 0];
            double a31 = (a21 * A[1, 1] + a20 * A[0, 1]) * B[1, 0];
            double t3 = a30 + a31;

            a30 /= t3;
            a31 /= t3;

            Assert.AreEqual(a00, actual[0, 0]);
            Assert.AreEqual(a01, actual[0, 1]);

            Assert.AreEqual(a10, actual[1, 0]);
            Assert.AreEqual(a11, actual[1, 1]);

            Assert.AreEqual(a20, actual[2, 0]);
            Assert.AreEqual(a21, actual[2, 1]);

            Assert.AreEqual(a30, actual[3, 0]);
            Assert.AreEqual(a31, actual[3, 1]);


            double p = System.Math.Exp(logLikelihood);
            Assert.AreEqual(0.00384315, p, 1e-8);
            Assert.IsFalse(double.IsNaN(p));
        }

        [TestMethod()]
        public void ForwardScalingTest2()
        {
            HiddenMarkovModel hmm = CreateModel2();

            //                     A  B  B  A
            int[] observations = { 0, 1, 1, 0 };

            double[] scaling;
            double logLikelihood;

            double[,] actual = ForwardBackwardAlgorithm.Forward(hmm, observations, out scaling, out logLikelihood);

            double p = System.Math.Exp(logLikelihood);
            Assert.AreEqual(0.054814695, p, 1e-8);
            Assert.IsFalse(double.IsNaN(p));
        }

        [TestMethod()]
        public void ForwardBackwardTest()
        {
            HiddenMarkovModel hmm = CreateModel1();

            //                     G  G  C  A
            int[] observations = { 2, 2, 1, 0 };

            double fwdLogLikelihood;
            double[,] fwd = ForwardBackwardAlgorithm.Forward(hmm, observations, out fwdLogLikelihood);

            double bwdLogLikelihood;
            double[,] bwd = ForwardBackwardAlgorithm.Backward(hmm, observations, out bwdLogLikelihood);

            Assert.AreEqual(fwdLogLikelihood, bwdLogLikelihood, 1e-10); // -5.5614629361549142
        }

        [TestMethod()]
        public void ForwardBackwardGenericTest()
        {
            var discreteModel = CreateModel1();
            var genericModel = CreateModel4();

            int[] discreteObservations = { 2, 2, 1, 0 };
            double[][] genericObservations = 
            {
                new double[] { 2 }, new double[] { 2 },
                new double[] { 1 }, new double[] { 0 }
            };

            double[] scaling = new double[3];

            double discreteFwdLogLikelihood;
            double[,] discreteFwd = ForwardBackwardAlgorithm.Forward(discreteModel,
                discreteObservations, out scaling, out discreteFwdLogLikelihood);

            double[,] discreteBwd = ForwardBackwardAlgorithm.Backward(discreteModel,
                discreteObservations, scaling);

            double genericFwdLogLikelihood;
            double[,] genericFwd = ForwardBackwardAlgorithm.Forward(genericModel,
                genericObservations, out scaling, out genericFwdLogLikelihood);

            double[,] genericBwd = ForwardBackwardAlgorithm.Backward(genericModel,
                genericObservations, scaling);

            Assert.AreEqual(discreteFwdLogLikelihood, genericFwdLogLikelihood);

            for (int i = 0; i < discreteFwd.GetLength(0); i++)
            {
                for (int j = 0; j < discreteFwd.GetLength(1); j++)
                {
                    Assert.AreEqual(discreteFwd[i, j], genericFwd[i, j]);
                    Assert.AreEqual(discreteBwd[i, j], genericBwd[i, j]);
                }
            }

        }


        [TestMethod()]
        public void LogBackwardTest()
        {
            HiddenMarkovModel hmm = Accord.Tests.Statistics.Models.Markov.
                ForwardBackwardAlgorithmTest.CreateModel2();


            int[] observations = { 0, 1, 1, 0 };

            double logLikelihood;
            double[,] expected = Matrix.Log(
              ForwardBackwardAlgorithm.Backward(hmm, observations));

            double[,] actual = 
                ForwardBackwardAlgorithm.LogBackward(hmm, observations, out logLikelihood);

            Assert.IsTrue(expected.IsEqual(actual, 1e-10));


            foreach (double e in actual)
                Assert.IsFalse(double.IsNaN(e));

            double p = System.Math.Exp(logLikelihood);
            Assert.AreEqual(0.054814695, p, 1e-8);
            Assert.IsFalse(double.IsNaN(p));
        }

        [TestMethod()]
        public void LogBackwardTest2()
        {
            HiddenMarkovModel hmm = Accord.Tests.Statistics.Models.Markov.
                ForwardBackwardAlgorithmTest.CreateModel3();

            int[] observations = { 0, 0, 1, 1 };

            double[,] expected = Matrix.Log(
              ForwardBackwardAlgorithm.Backward(hmm, observations));

            double[,] actual = 
                ForwardBackwardAlgorithm.LogBackward(hmm, observations);

            Assert.IsTrue(expected.IsEqual(actual, 1e-10));

            foreach (double p in actual)
                Assert.IsFalse(double.IsNaN(p));
        }

        [TestMethod()]
        public void LogForwardTest()
        {
            HiddenMarkovModel hmm = Accord.Tests.Statistics.Models.Markov.
                ForwardBackwardAlgorithmTest.CreateModel1();


            int[] observations = { 2, 2, 1, 0 };

            double[,] expected = Matrix.Log(
              ForwardBackwardAlgorithm.Forward(hmm, observations));

            double[,] actual = 
                ForwardBackwardAlgorithm.LogForward(hmm, observations);

            Assert.IsTrue(expected.IsEqual(actual, 1e-10));


            double p = 0;
            for (int i = 0; i < hmm.States; i++)
                p += Math.Exp(actual[observations.Length - 1, i]);

            Assert.AreEqual(0.00384315, p, 1e-8);
            Assert.IsFalse(double.IsNaN(p));
        }

        [TestMethod()]
        public void LogForwardTest2()
        {
            HiddenMarkovModel hmm = Accord.Tests.Statistics.Models.Markov.
                ForwardBackwardAlgorithmTest.CreateModel2();

            int[] observations = { 0, 1, 1, 0 };

            double[,] expected = Matrix.Log(
                ForwardBackwardAlgorithm.Forward(hmm, observations));

            double logLikelihood;
            double[,] actual = 
                ForwardBackwardAlgorithm.LogForward(hmm, observations, out logLikelihood);

            Assert.IsTrue(expected.IsEqual(actual, 1e-10));

            double p = 0;
            for (int i = 0; i < hmm.States; i++)
                p += Math.Exp(actual[observations.Length - 1, i]);

            Assert.AreEqual(0.054814695, p, 1e-8);
            Assert.IsFalse(double.IsNaN(p));
        }

        [TestMethod()]
        public void LogForwardBackwardGenericTest()
        {
            var discreteModel = CreateModel1();
            var genericModel = CreateModel4();

            int[] discreteObservations = { 2, 2, 1, 0 };
            double[][] genericObservations = 
            {
                new double[] { 2 }, new double[] { 2 },
                new double[] { 1 }, new double[] { 0 }
            };

            double discreteFwdLogLikelihood;
            double[,] discreteFwd = ForwardBackwardAlgorithm.LogForward(discreteModel,
                discreteObservations, out discreteFwdLogLikelihood);

            double discreteBwdLogLikelihood;
            double[,] discreteBwd = ForwardBackwardAlgorithm.LogBackward(discreteModel,
                discreteObservations, out discreteBwdLogLikelihood);

            double genericFwdLogLikelihood;
            double[,] genericFwd = ForwardBackwardAlgorithm.LogForward(genericModel,
                genericObservations, out genericFwdLogLikelihood);

            double genericBwdLogLikelihood;
            double[,] genericBwd = ForwardBackwardAlgorithm.LogBackward(genericModel,
                genericObservations, out genericBwdLogLikelihood);

            Assert.AreEqual(discreteFwdLogLikelihood, discreteBwdLogLikelihood);
            Assert.AreEqual(genericFwdLogLikelihood, genericBwdLogLikelihood);
            Assert.AreEqual(discreteFwdLogLikelihood, genericBwdLogLikelihood);

            for (int i = 0; i < discreteFwd.GetLength(0); i++)
            {
                for (int j = 0; j < discreteFwd.GetLength(1); j++)
                {
                    Assert.AreEqual(discreteFwd[i, j], genericFwd[i, j]);
                    Assert.AreEqual(discreteBwd[i, j], genericBwd[i, j]);
                }
            }

        }

    }
}
