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
    using System;
    using Accord.Math;
    using Accord.Statistics.Models.Markov;
    using Accord.Statistics.Models.Markov.Learning;
    using Accord.Statistics.Models.Markov.Topology;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Statistics.Filters;

    [TestClass()]
    public class HiddenMarkovModelTest
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
        public void ConstructorTest()
        {
            HiddenMarkovModel hmm;
            double[,] A, B;
            double[] pi;

            // Create a HMM with 2 states and 4 symbols
            hmm = new HiddenMarkovModel(2, 4);

            A = new double[,]
            {
                { 0.5, 0.5 },
                { 0.5, 0.5 }
            };

            B = new double[,]
            {
                { 0.25, 0.25, 0.25, 0.25 },
                { 0.25, 0.25, 0.25, 0.25 },
            };

            pi = new double[] { 1, 0 };

            Assert.AreEqual(2, hmm.States);
            Assert.AreEqual(4, hmm.Symbols);
            Assert.IsTrue(Matrix.Log(A).IsEqual(hmm.Transitions));
            Assert.IsTrue(Matrix.Log(B).IsEqual(hmm.Emissions));
            Assert.IsTrue(Matrix.Log(pi).IsEqual(hmm.Probabilities));



            hmm = new HiddenMarkovModel(new Forward(2), 4);

            A = new double[,]
            {
                { 0.5, 0.5 },
                { 0.0, 1.0 }
            };

            B = new double[,]
            {
                { 0.25, 0.25, 0.25, 0.25 },
                { 0.25, 0.25, 0.25, 0.25 },
            };

            pi = new double[] { 1, 0 };

            Assert.AreEqual(2, hmm.States);
            Assert.AreEqual(4, hmm.Symbols);
            Assert.IsTrue(Matrix.Log(A).IsEqual(hmm.Transitions));
            Assert.IsTrue(Matrix.Log(B).IsEqual(hmm.Emissions));
            Assert.IsTrue(Matrix.Log(pi).IsEqual(hmm.Probabilities));


            hmm = new HiddenMarkovModel(A, B, pi);
            Assert.AreEqual(2, hmm.States);
            Assert.AreEqual(4, hmm.Symbols);
            Assert.IsTrue(Matrix.Log(A).IsEqual(hmm.Transitions));
            Assert.IsTrue(Matrix.Log(B).IsEqual(hmm.Emissions));
            Assert.IsTrue(Matrix.Log(pi).IsEqual(hmm.Probabilities));
        }

        [TestMethod()]
        public void DecodeTest()
        {
            // Example taken from http://en.wikipedia.org/wiki/Viterbi_algorithm

            // Create the transition matrix A
            double[,] transition = 
            {  
                { 0.7, 0.3 },
                { 0.4, 0.6 }
            };

            // Create the emission matrix B
            double[,] emission = 
            {  
                { 0.1, 0.4, 0.5 },
                { 0.6, 0.3, 0.1 }
            };

            // Create the initial probabilities pi
            double[] initial =
            {
                0.6, 0.4
            };

            // Create a new hidden Markov model
            HiddenMarkovModel hmm = new HiddenMarkovModel(transition, emission, initial);

            // After that, one could, for example, query the probability
            // of a sequence occurring. We will consider the sequence
            int[] sequence = new int[] { 0, 1, 2 };

            // And now we will evaluate its likelihood
            double logLikelihood = hmm.Evaluate(sequence);

            // At this point, the log-likelihood of the sequence
            // occurring within the model is -3.3928721329161653.

            // We can also get the Viterbi path of the sequence
            int[] path = hmm.Decode(sequence, out logLikelihood);

            // At this point, the state path will be 1-0-0 and the
            // log-likelihood will be -4.3095199438871337


            Assert.AreEqual(logLikelihood, Math.Log(0.01344), 1e-10);
            Assert.AreEqual(path[0], 1);
            Assert.AreEqual(path[1], 0);
            Assert.AreEqual(path[2], 0);
        }


        [TestMethod()]
        public void LearnTest4()
        {

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

            HiddenMarkovModel hmm = new HiddenMarkovModel(3, 6);

            var teacher = new BaumWelchLearning(hmm) { Iterations = 100, Tolerance = 0 };

            double ll = teacher.Run(sequences);

            double l0; hmm.Decode(sequences[0], out l0);
            double l1; hmm.Decode(sequences[1], out l1);
            double l2; hmm.Decode(sequences[2], out l2);

            double pl = System.Math.Exp(ll);
            double p0 = System.Math.Exp(l0);
            double p1 = System.Math.Exp(l1);
            double p2 = System.Math.Exp(l2);

            Assert.AreEqual(0.49788370872923726, pl, 1e-10);
            Assert.AreEqual(0.014012065043262294, p0, 1e-10);
            Assert.AreEqual(0.016930905415294094, p1, 1e-10);
            Assert.AreEqual(0.001936595918966074, p2, 1e-10);
        }

        [TestMethod()]
        public void LearnTest()
        {
            HiddenMarkovModel hmm = new HiddenMarkovModel(2, 3);

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


            var teacher = new BaumWelchLearning(hmm)
            {
                Iterations = 650,
                Tolerance = 0
            };

            double ll = teacher.Run(observation);


            double[] pi = { 1.0, 0.0 };

            double[,] A =
            {
                { 0.7, 0.3 },
                { 0.5, 0.5 }
            };

            double[,] B =
            {
                { 0.6, 0.1, 0.3 },
                { 0.1, 0.7, 0.2 }
            };


            var hmmA = Matrix.Exp(hmm.Transitions);
            var hmmB = Matrix.Exp(hmm.Emissions);
            var hmmP = Matrix.Exp(hmm.Probabilities);


            Assert.IsTrue(Matrix.IsEqual(A, hmmA, 0.1));
            Assert.IsTrue(Matrix.IsEqual(B, hmmB, 0.1));
            Assert.IsTrue(Matrix.IsEqual(pi, hmmP));
        }

        [TestMethod()]
        public void LearnTest3()
        {
            // We will try to create a Hidden Markov Model which
            //  can detect if a given sequence starts with a zero
            //  and has any number of ones after that.
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
            HiddenMarkovModel hmm = new HiddenMarkovModel(3, 2);

            // Try to fit the model to the data until the difference in
            //  the average log-likelihood changes only by as little as 0.0001
            var teacher = new BaumWelchLearning(hmm) { Tolerance = 0.0001, Iterations = 0 };
            double ll = teacher.Run(sequences);

            // Calculate the probability that the given
            //  sequences originated from the model
            double l1; hmm.Decode(new int[] { 0, 1 }, out l1);  // 0.4999
            double l2; hmm.Decode(new int[] { 0, 1, 1, 1 }, out l2);  // 0.1145

            // Sequences which do not start with zero have much lesser probability.
            double l3; hmm.Decode(new int[] { 1, 1 }, out l3);  // 0.0000
            double l4; hmm.Decode(new int[] { 1, 0, 0, 0 }, out l4);  // 0.0000

            // Sequences which contains few errors have higher probability
            //  than the ones which do not start with zero. This shows some
            //  of the temporal elasticity and error tolerance of the HMMs.
            double l5; hmm.Decode(new int[] { 0, 1, 0, 1, 1, 1, 1, 1, 1 }, out l5); // 0.0002
            double l6; hmm.Decode(new int[] { 0, 1, 1, 1, 1, 1, 1, 0, 1 }, out l6); // 0.0002

            ll = System.Math.Exp(ll);
            l1 = System.Math.Exp(l1);
            l2 = System.Math.Exp(l2);
            l3 = System.Math.Exp(l3);
            l4 = System.Math.Exp(l4);
            l5 = System.Math.Exp(l5);
            l6 = System.Math.Exp(l6);

            Assert.AreEqual(0.95151126952069587, ll, 1e-4);
            Assert.AreEqual(0.4999419764097881, l1, 1e-4);
            Assert.AreEqual(0.1145702973735144, l2, 1e-4);
            Assert.AreEqual(0.0000529972606821, l3, 1e-4);
            Assert.AreEqual(0.0000000000000001, l4, 1e-4);
            Assert.AreEqual(0.0002674509390361, l5, 1e-4);
            Assert.AreEqual(0.0002674509390361, l6, 1e-4);

            Assert.IsTrue(l1 > l3 && l1 > l4);
            Assert.IsTrue(l2 > l3 && l2 > l4);
        }

        [TestMethod()]
        public void LearnTest6()
        {
            // We will try to create a Hidden Markov Model which
            //  can detect if a given sequence starts with a zero
            //  and has any number of ones after that.
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
            HiddenMarkovModel hmm = new HiddenMarkovModel(3, 2);

            // Try to fit the model to the data until the difference in
            //  the average log-likelihood changes only by as little as 0.0001
            var teacher = new BaumWelchLearning(hmm) { Tolerance = 0.0001, Iterations = 0 };
            double ll = teacher.Run(sequences);

            // Calculate the probability that the given
            //  sequences originated from the model
            double l1 = hmm.Evaluate(new int[] { 0, 1 });       // 0.999
            double l2 = hmm.Evaluate(new int[] { 0, 1, 1, 1 }); // 0.916

            // Sequences which do not start with zero have much lesser probability.
            double l3 = hmm.Evaluate(new int[] { 1, 1 });       // 0.000
            double l4 = hmm.Evaluate(new int[] { 1, 0, 0, 0 }); // 0.000

            // Sequences which contains few errors have higher probability
            //  than the ones which do not start with zero. This shows some
            //  of the temporal elasticity and error tolerance of the HMMs.
            double l5 = hmm.Evaluate(new int[] { 0, 1, 0, 1, 1, 1, 1, 1, 1 }); // 0.034
            double l6 = hmm.Evaluate(new int[] { 0, 1, 1, 1, 1, 1, 1, 0, 1 }); // 0.034

            double pl = System.Math.Exp(ll);
            double p1 = System.Math.Exp(l1);
            double p2 = System.Math.Exp(l2);
            double p3 = System.Math.Exp(l3);
            double p4 = System.Math.Exp(l4);
            double p5 = System.Math.Exp(l5);
            double p6 = System.Math.Exp(l6);

            Assert.AreEqual(0.95151126952069587, pl, 1e-6);
            Assert.AreEqual(0.99996863060890995, p1, 1e-6);
            Assert.AreEqual(0.91667240076011669, p2, 1e-6);
            Assert.AreEqual(0.00002335133758386, p3, 1e-6);
            Assert.AreEqual(0.00000000000000012, p4, 1e-6);
            Assert.AreEqual(0.034237231443226858, p5, 1e-6);
            Assert.AreEqual(0.034237195920532461, p6, 1e-6);

            Assert.IsTrue(l1 > l3 && l1 > l4);
            Assert.IsTrue(l2 > l3 && l2 > l4);
        }


        [TestMethod()]
        public void PredictTest()
        {
            int[][] sequences = new int[][] 
            {
                new int[] { 0, 3, 1, 2 },
            };


            HiddenMarkovModel hmm = new HiddenMarkovModel(new Forward(4), 4);

            var teacher = new BaumWelchLearning(hmm) { Tolerance = 1e-10, Iterations = 0 };
            double ll = teacher.Run(sequences);

            double l11, l12, l13, l14;

            int p1 = hmm.Predict(new int[] { 0 }, 1, out l11)[0];
            int p2 = hmm.Predict(new int[] { 0, 3 }, 1, out l12)[0];
            int p3 = hmm.Predict(new int[] { 0, 3, 1 }, 1, out l13)[0];
            int p4 = hmm.Predict(new int[] { 0, 3, 1, 2 }, 1, out l14)[0];

            Assert.AreEqual(3, p1);
            Assert.AreEqual(1, p2);
            Assert.AreEqual(2, p3);
            Assert.AreEqual(2, p4);

            double l21 = hmm.Evaluate(new int[] { 0, 3 });
            double l22 = hmm.Evaluate(new int[] { 0, 3, 1 });
            double l23 = hmm.Evaluate(new int[] { 0, 3, 1, 2 });
            double l24 = hmm.Evaluate(new int[] { 0, 3, 1, 2, 2 });

            Assert.AreEqual(l11, l21, 1e-10);
            Assert.AreEqual(l12, l22, 1e-10);
            Assert.AreEqual(l13, l23, 1e-10);
            Assert.AreEqual(l14, l24, 1e-10);

            Assert.IsFalse(double.IsNaN(l11));
            Assert.IsFalse(double.IsNaN(l12));
            Assert.IsFalse(double.IsNaN(l13));
            Assert.IsFalse(double.IsNaN(l14));

            Assert.IsFalse(double.IsNaN(l21));
            Assert.IsFalse(double.IsNaN(l22));
            Assert.IsFalse(double.IsNaN(l23));
            Assert.IsFalse(double.IsNaN(l24));

            double ln1;
            int[] pn = hmm.Predict(new int[] { 0 }, 4, out ln1);

            Assert.AreEqual(4, pn.Length);
            Assert.AreEqual(3, pn[0]);
            Assert.AreEqual(1, pn[1]);
            Assert.AreEqual(2, pn[2]);
            Assert.AreEqual(2, pn[3]);

            double ln2 = hmm.Evaluate(new int[] { 0, 3, 1, 2, 2 });

            Assert.AreEqual(ln1, ln2, 1e-10);
        }

        [TestMethod()]
        public void PredictTest2()
        {
            // We will try to create a Hidden Markov Model which
            // can recognize (and predict) the following sequences:
            int[][] sequences = 
            {
                new[] { 1, 2, 3, 4, 5 },
                new[] { 1, 2, 3, 3, 5 },
                new[] { 1, 2, 3 },
            };

            // Creates a new left-to-right (forward) Hidden Markov Model
            //  with 4 states for an output alphabet of six characters.
            HiddenMarkovModel hmm = new HiddenMarkovModel(new Forward(4), 6);

            // Try to fit the model to the data until the difference in
            //  the average log-likelihood changes only by as little as 0.0001
            BaumWelchLearning teacher = new BaumWelchLearning(hmm)
            {
                Tolerance = 0.0001,
                Iterations = 0
            };

            // Run the learning algorithm on the model
            double logLikelihood = teacher.Run(sequences);

            // Now, we will try to predict the next
            //   observations after a base sequence

            int length = 1; // number of observations to predict
            int[] input = { 1, 2 }; // base sequence for prediction

            // Predict the next 1 observation in sequence
            int[] prediction = hmm.Predict(input, length);

            // At this point, prediction should be int[] { 3 }
            Assert.AreEqual(prediction.Length, 1);
            Assert.AreEqual(prediction[0], 3);
        }

        [TestMethod()]
        public void PredictTest3()
        {
            // We will try to create a Hidden Markov Model which
            // can recognize (and predict) the following sequences:
            int[][] sequences = 
            {
                new[] { 1, 2, 3, 4, 5 },
                new[] { 1, 2, 4, 3, 5 },
                new[] { 1, 2, 5 },
            };

            // Creates a new left-to-right (forward) Hidden Markov Model
            //  with 4 states for an output alphabet of six characters.
            HiddenMarkovModel hmm = new HiddenMarkovModel(new Forward(4), 6);

            // Try to fit the model to the data until the difference in
            //  the average log-likelihood changes only by as little as 0.0001
            BaumWelchLearning teacher = new BaumWelchLearning(hmm)
            {
                Tolerance = 0.0001,
                Iterations = 0
            };

            // Run the learning algorithm on the model
            double logLikelihood = teacher.Run(sequences);

            // Now, we will try to predict the next
            //   observations after a base sequence

            int[] input = { 1, 2 }; // base sequence for prediction

            double[] logLikelihoods;

            // Predict the next observation in sequence
            int prediction = hmm.Predict(input, out logLikelihoods);

            var probs = Matrix.Exp(logLikelihoods);

            // At this point, prediction probabilities
            // should be equilibrated around 3, 4 and 5
            Assert.AreEqual(probs.Length, 6);
            Assert.AreEqual(probs[0], 0.00, 0.01);
            Assert.AreEqual(probs[1], 0.00, 0.01);
            Assert.AreEqual(probs[2], 0.00, 0.01);
            Assert.AreEqual(probs[3], 0.33, 0.05);
            Assert.AreEqual(probs[4], 0.33, 0.05);
            Assert.AreEqual(probs[5], 0.33, 0.05);


            double[][] probabilities2;

            // Predict the next 2 observation2 in sequence
            int[] prediction2 = hmm.Predict(input, 2, out probabilities2);

            Assert.AreEqual(probabilities2.Length, 2);
            Assert.AreEqual(probabilities2[0].Length, 6);
            Assert.AreEqual(probabilities2[1].Length, 6);

            Assert.IsTrue(probabilities2[0].IsEqual(logLikelihoods));
        }


        [TestMethod()]
        public void GenerateTest()
        {
            // Example taken from http://en.wikipedia.org/wiki/Viterbi_algorithm

            double[,] transition = 
            {  
                { 0.7, 0.3 },
                { 0.4, 0.6 }
            };

            double[,] emission = 
            {  
                { 0.1, 0.4, 0.5 },
                { 0.6, 0.3, 0.1 }
            };

            double[] initial =
            {
                0.6, 0.4
            };

            HiddenMarkovModel hmm = new HiddenMarkovModel(transition, emission, initial);

            double logLikelihood;
            int[] path;
            int[] samples = hmm.Generate(10, out path, out logLikelihood);

            double expected = hmm.Evaluate(samples, path);

            Assert.AreEqual(expected, logLikelihood);
        }

        [TestMethod()]
        public void GenerateTest2()
        {
            Accord.Math.Tools.SetupGenerator(42);

            // Consider some phrases:
            //
            string[][] phrases =
            {
                new[] { "those", "are", "sample", "words", "from", "a", "dictionary" },
                new[] { "those", "are", "sample", "words" },
                new[] { "sample", "words", "are", "words" },
                new[] { "those", "words" },
                new[] { "those", "are", "words" },
                new[] { "words", "from", "a", "dictionary" },
                new[] { "those", "are", "words", "from", "a", "dictionary" }
            };

            // Let's begin by transforming them to sequence of
            // integer labels using a codification codebook:
            var codebook = new Codification("Words", phrases);

            // Now we can create the training data for the models:
            int[][] sequence = codebook.Translate("Words", phrases);

            // To create the models, we will specify a forward topology,
            // as the sequences have definite start and ending points.
            //
            var topology = new Forward(states: 4);
            int symbols = codebook["Words"].Symbols; // We have 7 different words

            // Create the hidden Markov model
            HiddenMarkovModel hmm = new HiddenMarkovModel(topology, symbols);

            // Create the learning algorithm
            BaumWelchLearning teacher = new BaumWelchLearning(hmm);

            // Teach the model about the phrases
            double error = teacher.Run(sequence);

            // Now, we can ask the model to generate new samples
            // from the word distributions it has just learned:
            //
            int[] sample = hmm.Generate(3);

            // And the result will be: "those", "are", "words".
            string[] result = codebook.Translate("Words", sample);

            Assert.AreEqual("those", result[0]);
            Assert.AreEqual("are", result[1]);
            Assert.AreEqual("words", result[2]);
        }

    }
}
