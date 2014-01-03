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

namespace Accord.Tests.Statistics.Models.Fields
{
    using Accord.Statistics.Models.Fields;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Statistics.Models.Fields.Functions;
    using Accord.Statistics.Models.Markov;
    using Accord.Math;
    using System;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Models.Markov.Topology;

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



        [TestMethod()]
        public void BackwardTest()
        {
            HiddenMarkovModel hmm = Accord.Tests.Statistics.Models.Markov.
                ForwardBackwardAlgorithmTest.CreateModel2();

            var function = new MarkovDiscreteFunction(hmm);


            //                     A  B  B  A
            int[] observations = { 0, 1, 1, 0 };

            double logLikelihood;
            double[,] actual = Accord.Statistics.Models.Fields.
                ForwardBackwardAlgorithm.Backward(function.Factors[0], observations, 0, out logLikelihood);

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


            Assert.AreEqual(actual[0, 0], a00, 1e-10);
            Assert.AreEqual(actual[0, 1], a01, 1e-10);

            Assert.AreEqual(actual[1, 0], a10, 1e-10);
            Assert.AreEqual(actual[1, 1], a11, 1e-10);

            Assert.AreEqual(actual[2, 0], a20, 1e-10);
            Assert.AreEqual(actual[2, 1], a21, 1e-10);

            Assert.AreEqual(actual[3, 0], a30, 1e-10);
            Assert.AreEqual(actual[3, 1], a31, 1e-10);

            foreach (double e in actual)
                Assert.IsFalse(double.IsNaN(e));

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
            HiddenMarkovModel hmm = Accord.Tests.Statistics.Models.Markov.
                ForwardBackwardAlgorithmTest.CreateModel3();

            MarkovDiscreteFunction function = new MarkovDiscreteFunction(hmm);

            //                     L  L  R  R
            int[] observations = { 0, 0, 1, 1 };

            double[,] actual = Accord.Statistics.Models.Fields.
                ForwardBackwardAlgorithm.Backward(function.Factors[0], observations);

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
            HiddenMarkovModel hmm = Accord.Tests.Statistics.Models.Markov.
                ForwardBackwardAlgorithmTest.CreateModel1();

            MarkovDiscreteFunction function = new MarkovDiscreteFunction(hmm);


            //                     G  G  C  A
            int[] observations = { 2, 2, 1, 0 };

            double[,] actual = Accord.Statistics.Models.Fields.
                ForwardBackwardAlgorithm.Forward(function.Factors[0], observations);

            var A = Matrix.Exp(hmm.Transitions);
            var B = Matrix.Exp(hmm.Emissions);
            var P = Matrix.Exp(hmm.Probabilities);

            double a00 = P[0] * B[0, 2];
            double a01 = P[1] * B[1, 2];

            double a10 = (a00 * A[0, 0] + a01 * A[1, 0]) * B[0, 2];
            double a11 = (a01 * A[1, 1] + a00 * A[0, 1]) * B[1, 2];

            double a20 = (a10 * A[0, 0] + a11 * A[1, 0]) * B[0, 1];
            double a21 = (a11 * A[1, 1] + a10 * A[0, 1]) * B[1, 1];

            double a30 = (a20 * A[0, 0] + a21 * A[1, 0]) * B[0, 0];
            double a31 = (a21 * A[1, 1] + a20 * A[0, 1]) * B[1, 0];

            Assert.AreEqual(a00, actual[0, 0], 1e-10);
            Assert.AreEqual(a01, actual[0, 1], 1e-10);

            Assert.AreEqual(a10, actual[1, 0], 1e-10);
            Assert.AreEqual(a11, actual[1, 1], 1e-10);

            Assert.AreEqual(a20, actual[2, 0], 1e-10);
            Assert.AreEqual(a21, actual[2, 1], 1e-10);

            Assert.AreEqual(a30, actual[3, 0], 1e-10);
            Assert.AreEqual(a31, actual[3, 1], 1e-10);

            foreach (double e in actual)
                Assert.IsFalse(double.IsNaN(e));

            double p = 0;
            for (int i = 0; i < hmm.States; i++)
                p += actual[observations.Length - 1, i];

            Assert.AreEqual(0.00384315, p, 1e-8);
            Assert.IsFalse(double.IsNaN(p));
        }

        [TestMethod()]
        public void ForwardTest2()
        {
            HiddenMarkovModel hmm = Accord.Tests.Statistics.Models.Markov.
                ForwardBackwardAlgorithmTest.CreateModel2();

            MarkovDiscreteFunction function = new MarkovDiscreteFunction(hmm);


            //                     A  B  B  A
            int[] observations = { 0, 1, 1, 0 };

            double[,] actual = Accord.Statistics.Models.Fields.
                ForwardBackwardAlgorithm.Forward(function.Factors[0], observations);

            var A = Matrix.Exp(hmm.Transitions);
            var B = Matrix.Exp(hmm.Emissions);
            var P = Matrix.Exp(hmm.Probabilities);

            double a00 = P[0] * B[0, 0];
            double a01 = P[1] * B[1, 0];

            double a10 = (a00 * A[0, 0] + a01 * A[1, 0]) * B[0, 1];
            double a11 = (a01 * A[1, 1] + a00 * A[0, 1]) * B[1, 1];

            double a20 = (a10 * A[0, 0] + a11 * A[1, 0]) * B[0, 1];
            double a21 = (a11 * A[1, 1] + a10 * A[0, 1]) * B[1, 1];

            double a30 = (a20 * A[0, 0] + a21 * A[1, 0]) * B[0, 0];
            double a31 = (a21 * A[1, 1] + a20 * A[0, 1]) * B[1, 0];


            Assert.AreEqual(a00, actual[0, 0], 1e-10);
            Assert.AreEqual(a01, actual[0, 1], 1e-10);

            Assert.AreEqual(a10, actual[1, 0], 1e-10);
            Assert.AreEqual(a11, actual[1, 1], 1e-10);

            Assert.AreEqual(a20, actual[2, 0], 1e-10);
            Assert.AreEqual(a21, actual[2, 1], 1e-10);

            Assert.AreEqual(a30, actual[3, 0], 1e-10);
            Assert.AreEqual(a31, actual[3, 1], 1e-10);

            foreach (double e in actual)
                Assert.IsFalse(double.IsNaN(e));

            double p = 0;
            for (int i = 0; i < hmm.States; i++)
                p += actual[observations.Length - 1, i];

            Assert.AreEqual(0.054814695, p, 1e-8);
            Assert.IsFalse(double.IsNaN(p));
        }

        [TestMethod()]
        public void ForwardTest3()
        {
            HiddenMarkovModel hmm = Accord.Tests.Statistics.Models.Markov.
                ForwardBackwardAlgorithmTest.CreateModel3();

            MarkovDiscreteFunction function = new MarkovDiscreteFunction(hmm);

            //                     L  L  R  R
            int[] observations = { 0, 0, 1, 1 };

            double[,] actual = Accord.Statistics.Models.Fields.
                ForwardBackwardAlgorithm.Forward(function.Factors[0], observations);

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
            HiddenMarkovModel hmm = Accord.Tests.Statistics.Models.Markov.
                ForwardBackwardAlgorithmTest.CreateModel1();

            MarkovDiscreteFunction function = new MarkovDiscreteFunction(hmm);


            //                     G  G  C  A
            int[] observations = { 2, 2, 1, 0 };

            double[] scaling;
            double logLikelihood;

            double[,] actual = Accord.Statistics.Models.Fields.
                ForwardBackwardAlgorithm.Forward(function.Factors[0], observations, 0, out scaling, out logLikelihood);

            double[] P = Matrix.Exp(hmm.Probabilities);
            double[,] B = Matrix.Exp(hmm.Emissions);
            double[,] A = Matrix.Exp(hmm.Transitions);

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

            Assert.AreEqual(a00, actual[0, 0], 1e-10);
            Assert.AreEqual(a01, actual[0, 1], 1e-10);

            Assert.AreEqual(a10, actual[1, 0], 1e-10);
            Assert.AreEqual(a11, actual[1, 1], 1e-10);

            Assert.AreEqual(a20, actual[2, 0], 1e-10);
            Assert.AreEqual(a21, actual[2, 1], 1e-10);

            Assert.AreEqual(a30, actual[3, 0], 1e-10);
            Assert.AreEqual(a31, actual[3, 1], 1e-10);

            foreach (double e in actual)
                Assert.IsFalse(double.IsNaN(e));


            double p = System.Math.Exp(logLikelihood);
            Assert.AreEqual(0.00384315, p, 1e-8);
            Assert.IsFalse(double.IsNaN(p));
        }

        [TestMethod()]
        public void ForwardScalingTest2()
        {
            HiddenMarkovModel hmm = Accord.Tests.Statistics.Models.Markov.
                ForwardBackwardAlgorithmTest.CreateModel2();

            MarkovDiscreteFunction function = new MarkovDiscreteFunction(hmm);

            //                     A  B  B  A
            int[] observations = { 0, 1, 1, 0 };

            double[] scaling;
            double logLikelihood;

            double[,] actual = Accord.Statistics.Models.Fields.
                ForwardBackwardAlgorithm.Forward(function.Factors[0], observations, 0, out scaling, out logLikelihood);

            double p = System.Math.Exp(logLikelihood);
            Assert.AreEqual(0.054814695, p, 1e-7);
            Assert.IsFalse(double.IsNaN(p));
        }

        [TestMethod()]
        public void ForwardBackwardTest()
        {
            HiddenMarkovModel hmm = Accord.Tests.Statistics.Models.Markov.
                ForwardBackwardAlgorithmTest.CreateModel1();

            MarkovDiscreteFunction function = new MarkovDiscreteFunction(hmm);

            //                     G  G  C  A
            int[] observations = { 2, 2, 1, 0 };

            double fwdLogLikelihood;
            double[,] fwd = Accord.Statistics.Models.Fields.
                ForwardBackwardAlgorithm.Forward(function.Factors[0], observations, 0, out fwdLogLikelihood);

            double bwdLogLikelihood;
            double[,] bwd = Accord.Statistics.Models.Fields.
                ForwardBackwardAlgorithm.Backward(function.Factors[0], observations, 0, out bwdLogLikelihood);

            Assert.AreEqual(fwdLogLikelihood, bwdLogLikelihood, 1e-10); // -5.5614629361549142
            Assert.AreEqual(-5.5614629361549142, fwdLogLikelihood, 1e-10);
            Assert.IsFalse(double.IsNaN(fwdLogLikelihood));
        }



        [TestMethod()]
        public void LogBackwardTest()
        {
            HiddenMarkovModel hmm = Accord.Tests.Statistics.Models.Markov.
                ForwardBackwardAlgorithmTest.CreateModel2();

            MarkovDiscreteFunction function = new MarkovDiscreteFunction(hmm);


            int[] observations = { 0, 1, 1, 0 };

            double logLikelihood;
            double[,] expected = Matrix.Log(Accord.Statistics.Models.Fields.
              ForwardBackwardAlgorithm.Backward(function.Factors[0], observations, 0));

            double[,] actual = Accord.Statistics.Models.Fields.
                ForwardBackwardAlgorithm.LogBackward(function.Factors[0], observations, 0, out logLikelihood);

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

            MarkovDiscreteFunction function = new MarkovDiscreteFunction(hmm);

            int[] observations = { 0, 0, 1, 1 };

            double[,] expected = Matrix.Log(Accord.Statistics.Models.Fields.
              ForwardBackwardAlgorithm.Backward(function.Factors[0], observations, 0));

            double[,] actual = Accord.Statistics.Models.Fields.
                ForwardBackwardAlgorithm.LogBackward(function.Factors[0], observations, 0);

            Assert.IsTrue(expected.IsEqual(actual, 1e-10));

            foreach (double p in actual)
                Assert.IsFalse(double.IsNaN(p));
        }

        [TestMethod()]
        public void LogForwardTest()
        {
            HiddenMarkovModel hmm = Accord.Tests.Statistics.Models.Markov.
                ForwardBackwardAlgorithmTest.CreateModel1();

            MarkovDiscreteFunction function = new MarkovDiscreteFunction(hmm);


            int[] observations = { 2, 2, 1, 0 };

            double[,] expected = Matrix.Log(Accord.Statistics.Models.Fields.
              ForwardBackwardAlgorithm.Forward(function.Factors[0], observations, 0));

            double[,] actual = Accord.Statistics.Models.Fields.
                ForwardBackwardAlgorithm.LogForward(function.Factors[0], observations, 0);

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

            MarkovDiscreteFunction function = new MarkovDiscreteFunction(hmm);

            int[] observations = { 0, 1, 1, 0 };

            double[,] expected = Matrix.Log(Accord.Statistics.Models.Fields.
                ForwardBackwardAlgorithm.Forward(function.Factors[0], observations, 0));

            double logLikelihood;
            double[,] actual = Accord.Statistics.Models.Fields.
                ForwardBackwardAlgorithm.LogForward(function.Factors[0], observations, 0, out logLikelihood);

            Assert.IsTrue(expected.IsEqual(actual, 1e-10));

            double p = 0;
            for (int i = 0; i < hmm.States; i++)
                p += Math.Exp(actual[observations.Length - 1, i]);

            Assert.AreEqual(0.054814695, p, 1e-8);
            Assert.AreEqual(0.054814695, Math.Exp(logLikelihood), 1e-8);
            Assert.IsFalse(double.IsNaN(p));
        }

        [TestMethod()]
        public void LogForwardTest3()
        {
            MultivariateNormalDistribution density = new MultivariateNormalDistribution(3);
            var hmm = new HiddenMarkovClassifier<MultivariateNormalDistribution>(2, new Ergodic(2), density);

            double[][][] inputs =
            {
                new [] { new double[] { 0, 1, 0 }, new double[] { 0, 1, 0 }, new double[] { 0, 1, 0 } },
                new [] { new double[] { 1, 6, 2 }, new double[] { 2, 1, 6 }, new double[] { 1, 1, 0 } },
                new [] { new double[] { 9, 1, 0 }, new double[] { 0, 1, 5 }, new double[] { 0, 0, 0 } },
            };

            int[] outputs = 
            {
                0, 0, 1
            };

            var function = new MarkovMultivariateFunction(hmm);

            var observations = inputs[0];

            double[,] expected = Matrix.Log(Accord.Statistics.Models.Fields.
                ForwardBackwardAlgorithm.Forward(function.Factors[0], observations, 0));

            double logLikelihood;
            double[,] actual = Accord.Statistics.Models.Fields.
                ForwardBackwardAlgorithm.LogForward(function.Factors[0], observations, 0, out logLikelihood);

            Assert.IsTrue(expected.IsEqual(actual, 1e-10));

            double p = 0;
            for (int i = 0; i < hmm[0].States; i++)
                p += Math.Exp(actual[observations.Length - 1, i]);

            Assert.AreEqual(Math.Exp(logLikelihood), p, 1e-8);
            Assert.IsFalse(double.IsNaN(p));
        }

        [TestMethod()]
        public void ForwardTest4()
        {
            var hmmc = Accord.Tests.Statistics.Models.Fields.
                MultivariateNormalHiddenMarkovClassifierPotentialFunctionTest.CreateModel3();

            var hmm = hmmc[0];

            var function = new MarkovMultivariateFunction(hmm);

            var observations = Accord.Tests.Statistics.Models.Fields.
                MultivariateNormalHiddenMarkovClassifierPotentialFunctionTest.inputTest[3];

            double expectedLogLikelihood;
            double[,] expected = Accord.Statistics.Models.Markov
                .ForwardBackwardAlgorithm.Forward(hmm, observations, out expectedLogLikelihood);

            double actualLogLikelihood;
            double[,] actual = Accord.Statistics.Models.Fields.
                ForwardBackwardAlgorithm.Forward(function.Factors[0], observations, 0, out actualLogLikelihood);


            Assert.IsTrue(expected.IsEqual(actual, 1e-10));


            Assert.AreEqual(expectedLogLikelihood, actualLogLikelihood, 1e-6);
            Assert.IsFalse(Double.IsNaN(actualLogLikelihood));

        }

        [TestMethod()]
        public void LogForwardTest4()
        {
            var hmmc = Accord.Tests.Statistics.Models.Fields.
                MultivariateNormalHiddenMarkovClassifierPotentialFunctionTest.CreateModel3();

            for (int c = 0; c < hmmc.Classes; c++)
            {
                var hmm = hmmc[c];

                var function = new MarkovMultivariateFunction(hmm);

                var sequences = Accord.Tests.Statistics.Models.Fields.
                    MultivariateNormalHiddenMarkovClassifierPotentialFunctionTest.inputTest;

                for (int i = 0; i < sequences.Length; i++)
                {
                    var observations = sequences[i];

                    double expectedLogLikelihood;
                    double[,] expected = Accord.Statistics.Models.Markov
                        .ForwardBackwardAlgorithm.LogForward(hmm, observations, out expectedLogLikelihood);

                    double actualLogLikelihood;
                    double[,] actual = Accord.Statistics.Models.Fields.
                        ForwardBackwardAlgorithm.LogForward(function.Factors[0], observations, 0, out actualLogLikelihood);


                    Assert.IsTrue(expected.IsEqual(actual, 1e-10));

                    Assert.AreEqual(expectedLogLikelihood, actualLogLikelihood, 1e-6);
                    Assert.IsFalse(Double.IsNaN(actualLogLikelihood));
                }
            }
        }

        [TestMethod()]
        public void LogForwardBackwardTest()
        {
            HiddenMarkovModel hmm = Accord.Tests.Statistics.Models.Markov.
                ForwardBackwardAlgorithmTest.CreateModel1();

            MarkovDiscreteFunction function = new MarkovDiscreteFunction(hmm);

            //                     G  G  C  A
            int[] observations = { 2, 2, 1, 0 };

            double fwdLogLikelihood;
            double[,] fwd = Accord.Statistics.Models.Fields.
                ForwardBackwardAlgorithm.LogForward(function.Factors[0], observations, 0, out fwdLogLikelihood);

            double bwdLogLikelihood;
            double[,] bwd = Accord.Statistics.Models.Fields.
                ForwardBackwardAlgorithm.LogBackward(function.Factors[0], observations, 0, out bwdLogLikelihood);

            Assert.AreEqual(fwdLogLikelihood, bwdLogLikelihood, 1e-10); // -5.5614629361549142
            Assert.AreEqual(-5.5614629361549142, fwdLogLikelihood, 1e-10);
            Assert.IsFalse(double.IsNaN(fwdLogLikelihood));
        }


    }
}
