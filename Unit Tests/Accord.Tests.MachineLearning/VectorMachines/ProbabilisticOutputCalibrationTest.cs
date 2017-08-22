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

namespace Accord.Tests.MachineLearning
{
    using System;
    using Accord.MachineLearning.VectorMachines;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Statistics.Kernels;
    using NUnit.Framework;
    using Accord.Math;
    using Accord.MachineLearning;
    using Accord.Math.Optimization.Losses;
    using System.IO;
    using Newtonsoft.Json;
    using System.Threading.Tasks;

    [TestFixture]
    public class ProbabilisticOutputLearningTest
    {


        [Test]
        public void RunTest1()
        {
            double[][] inputs =
            {
                new double[] { -1, -1 },
                new double[] { -1,  1 },
                new double[] {  1, -1 },
                new double[] {  1,  1 }
            };

            int[] outputs =
            {
                 -1,
                  1,
                  1,
                 -1
            };

            KernelSupportVectorMachine svm = new KernelSupportVectorMachine(new Gaussian(3.6), 2);

            var smo = new SequentialMinimalOptimization(svm, inputs, outputs);

            double error1 = smo.Run();

            Assert.AreEqual(0, error1);

            double[] distances = new double[outputs.Length];
            for (int i = 0; i < outputs.Length; i++)
            {
                int y = svm.Compute(inputs[i], out distances[i]);
                Assert.AreEqual(outputs[i], y);
            }


            var target = new ProbabilisticOutputCalibration(svm, inputs, outputs);

            double ll0 = target.LogLikelihood(inputs, outputs);

            double ll1 = target.Run();

            double ll2 = target.LogLikelihood(inputs, outputs);

            Assert.AreEqual(5.5451735748694571, ll1);
            Assert.AreEqual(ll1, ll2);
            Assert.IsTrue(ll1 > ll0);

            double[] newdistances = new double[outputs.Length];
            for (int i = 0; i < outputs.Length; i++)
            {
                int y = svm.Compute(inputs[i], out newdistances[i]);
                Assert.AreEqual(outputs[i], y);
            }

            double[] probs = new double[outputs.Length];
            for (int i = 0; i < outputs.Length; i++)
            {
                int y;
                probs[i] = svm.ToMulticlass().Probability(inputs[i], out y);
                Assert.AreEqual(outputs[i], y == 1 ? 1 : -1);
            }

            Assert.AreEqual(0.25, probs[0], 1e-5);
            Assert.AreEqual(0.75, probs[1], 1e-5);
            Assert.AreEqual(0.75, probs[2], 1e-5);
            Assert.AreEqual(0.25, probs[3], 1e-5);

            foreach (var p in probs)
                Assert.IsFalse(Double.IsNaN(p));

        }

        [Test]
        public void RunTest3()
        {
            // Example XOR problem
            double[][] inputs =
            {
                new double[] { 0, 0 }, // 0 xor 0: 1 (label +1)
                new double[] { 0, 1 }, // 0 xor 1: 0 (label -1)
                new double[] { 1, 0 }, // 1 xor 0: 0 (label -1)
                new double[] { 1, 1 }  // 1 xor 1: 1 (label +1)
            };

            // Dichotomy SVM outputs should be given as [-1;+1]
            int[] labels =
            {
                1, -1, -1, 1
            };

            // Create a Kernel Support Vector Machine for the given inputs
            KernelSupportVectorMachine svm = new KernelSupportVectorMachine(new Gaussian(0.1), inputs[0].Length);

            // Instantiate a new learning algorithm for SVMs
            SequentialMinimalOptimization smo = new SequentialMinimalOptimization(svm, inputs, labels);

            // Set up the learning algorithm
            smo.Complexity = 1.0;

            // Run the learning algorithm
            double error = smo.Run();

            Assert.IsFalse(svm.IsProbabilistic);
            Assert.AreEqual(0, error);
            Assert.AreEqual(-1, svm.Weights[0]);
            Assert.AreEqual(1, svm.Weights[1]);
            Assert.AreEqual(-1, svm.Weights[2]);
            Assert.AreEqual(1, svm.Weights[3]);

            // Instantiate the probabilistic learning calibration
            var calibration = new ProbabilisticOutputCalibration(svm, inputs, labels);

            // Run the calibration algorithm
            double loglikelihood = calibration.Run();

            Assert.IsTrue(svm.IsProbabilistic);
            Assert.AreEqual(-1.0986109988055595, svm.Weights[0]);
            Assert.AreEqual(1.0986109988055595, svm.Weights[1]);
            Assert.AreEqual(-1.0986109988055595, svm.Weights[2]);
            Assert.AreEqual(1.0986109988055595, svm.Weights[3]);

            // Compute the decision output for one of the input vectors,
            // while also retrieving the probability of the answer

            double probability;
            int decision = svm.Compute(inputs[0], out probability);

            // At this point, decision is +1 with a probability of 75%

            Assert.AreEqual(1, decision);
            Assert.AreEqual(0.74999975815069375, probability, 1e-10);
            Assert.AreEqual(0, error);
            Assert.AreEqual(5.5451735748925355, loglikelihood);
        }

        [Test]
        public void learn_test()
        {
            #region doc_learn
            double[][] inputs = // Example XOR problem
            {
                new double[] { 0, 0 }, // 0 xor 0: 1 (label +1)
                new double[] { 0, 1 }, // 0 xor 1: 0 (label -1)
                new double[] { 1, 0 }, // 1 xor 0: 0 (label -1)
                new double[] { 1, 1 }  // 1 xor 1: 1 (label +1)
            };

            int[] outputs = // XOR outputs
            {
                1, 0, 0, 1
            };

            // Instantiate a new SMO learning algorithm for SVMs
            var smo = new SequentialMinimalOptimization<Gaussian>()
            {
                Kernel = new Gaussian(0.1),
                Complexity = 1.0
            };

            // Learn a SVM using the algorithm
            var svm = smo.Learn(inputs, outputs);

            // Predict labels for each input sample
            bool[] predicted = svm.Decide(inputs);

            // Compute classification error
            double error = new ZeroOneLoss(outputs).Loss(predicted);

            // Instantiate the probabilistic calibration (using Platt's scaling)
            var calibration = new ProbabilisticOutputCalibration<Gaussian>(svm);

            // Run the calibration algorithm
            calibration.Learn(inputs, outputs); // returns the same machine

            // Predict probabilities of each input sample
            double[] probabilities = svm.Probability(inputs);

            // Compute the error based on a hard decision
            double loss = new BinaryCrossEntropyLoss(outputs).Loss(probabilities);

            // Compute the decision output for one of the input vectors,
            // while also retrieving the probability of the answer

            bool decision;
            double probability = svm.Probability(inputs[0], out decision);
            #endregion

            // At this point, decision is +1 with a probability of 75%

            Assert.AreEqual(true, decision);
            Assert.AreEqual(0, error);
            Assert.AreEqual(5.5451735748925355, loss);
            Assert.AreEqual(0.74999975815069375, probability, 1e-10);
            Assert.IsTrue(svm.IsProbabilistic);
            Assert.AreEqual(-1.0986109988055595, svm.Weights[0]);
            Assert.AreEqual(1.0986109988055595, svm.Weights[1]);
            Assert.AreEqual(-1.0986109988055595, svm.Weights[2]);
            Assert.AreEqual(1.0986109988055595, svm.Weights[3]);
        }


        [Test]
        public void RunTest2()
        {
            double[][] inputs =
            {
                new double[] { 0, 1, 1, 0 }, // 0
                new double[] { 0, 1, 0, 0 }, // 0
                new double[] { 0, 0, 1, 0 }, // 0
                new double[] { 0, 1, 1, 0 }, // 0
                new double[] { 0, 1, 0, 0 }, // 0
                new double[] { 1, 0, 0, 0 }, // 1
                new double[] { 1, 0, 0, 0 }, // 1
                new double[] { 1, 0, 0, 1 }, // 1
                new double[] { 0, 0, 0, 1 }, // 1
                new double[] { 0, 0, 0, 1 }, // 1
                new double[] { 1, 1, 1, 1 }, // 2
                new double[] { 1, 0, 1, 1 }, // 2
                new double[] { 1, 1, 0, 1 }, // 2
                new double[] { 0, 1, 1, 1 }, // 2
                new double[] { 1, 1, 1, 1 }, // 2
            };

            int[] outputs =
            {
                0, 0, 0, 0, 0,
                1, 1, 1, 1, 1,
                2, 2, 2, 2, 2,
            };

            IKernel kernel = new Linear();
            var machine = new MulticlassSupportVectorMachine(4, kernel, 3);
            var target = new MulticlassSupportVectorLearning(machine, inputs, outputs);

            target.Algorithm = (svm, classInputs, classOutputs, i, j) =>
                new SequentialMinimalOptimization(svm, classInputs, classOutputs);

            double error1 = target.Run();
            Assert.AreEqual(0, error1);

            int[] actual = new int[outputs.Length];
            var paths = new Decision[outputs.Length][];
            for (int i = 0; i < actual.Length; i++)
            {
                actual[i] = machine.Decide(inputs[i]);
                paths[i] = machine.GetLastDecisionPath();
                Assert.AreEqual(outputs[i], actual[i]);
            }

            var original = (MulticlassSupportVectorMachine)machine.Clone();

            target.Algorithm = (svm, classInputs, classOutputs, i, j) =>
                new ProbabilisticOutputCalibration(svm, classInputs, classOutputs);

            double error2 = target.Run();
            Assert.AreEqual(0, error2);

            int[] actual2 = new int[outputs.Length];
            var paths2 = new Decision[outputs.Length][];
            for (int i = 0; i < actual.Length; i++)
            {
                actual2[i] = machine.Decide(inputs[i]);
                paths2[i] = machine.GetLastDecisionPath();
                Assert.AreEqual(outputs[i], actual[i]);
            }

            var svm21 = machine[2, 1];
            var org21 = original[2, 1];
            var probe = inputs[12];
            var w21 = svm21.Weights;
            var o21 = org21.Weights;
            Assert.IsFalse(w21.IsEqual(o21, rtol: 1e-2));
            bool b = svm21.Decide(probe);
            bool a = org21.Decide(probe);
            Assert.AreEqual(a, b);

            double[][] probabilities = machine.Probabilities(inputs);

            //string str = probabilities.ToString(CSharpJaggedMatrixFormatProvider.InvariantCulture);

            double[][] expected = new double[][]
            {
                new double[] { 0.978013252309678, 0.00665988562670578, 0.015326862063616 },
                new double[] { 0.923373734751393, 0.0433240974867644, 0.033302167761843 },
                new double[] { 0.902265207121918, 0.0651939200306017, 0.0325408728474804 },
                new double[] { 0.978013252309678, 0.00665988562670578, 0.015326862063616 },
                new double[] { 0.923373734751393, 0.0433240974867644, 0.033302167761843 },
                new double[] { 0.0437508203303804, 0.79994737664453, 0.156301803025089 },
                new double[] { 0.0437508203303804, 0.79994737664453, 0.156301803025089 },
                new double[] { 0.0147601290467641, 0.948443224264852, 0.0367966466883842 },
                new double[] { 0.0920231845129213, 0.875878175972548, 0.0320986395145312 },
                new double[] { 0.0920231845129213, 0.875878175972548, 0.0320986395145312 },
                new double[] { 0.00868243281954335, 0.00491075178001821, 0.986406815400439 },
                new double[] { 0.0144769600209954, 0.0552754387307989, 0.930247601248206 },
                new double[] { 0.0144769600209954, 0.0552754387307989, 0.930247601248206 },
                new double[] { 0.0584631682316073, 0.0122104663095354, 0.929326365458857 },
                new double[] { 0.00868243281954335, 0.00491075178001821, 0.986406815400439 }
            };

            Assert.IsTrue(probabilities.IsEqual(expected, rtol: 1e-8));
        }


    }
}
