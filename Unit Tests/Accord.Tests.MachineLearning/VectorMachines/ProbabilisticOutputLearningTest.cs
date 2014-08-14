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

namespace Accord.Tests.MachineLearning
{
    using System;
    using Accord.MachineLearning.VectorMachines;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Statistics.Kernels;
    using Microsoft.VisualStudio.TestTools.UnitTesting;


    [TestClass()]
    public class ProbabilisticOutputLearningTest
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

            SequentialMinimalOptimization smo = new SequentialMinimalOptimization(svm, inputs, outputs);

            double error1 = smo.Run();

            Assert.AreEqual(0, error1);

            double[] distances = new double[outputs.Length];
            for (int i = 0; i < outputs.Length; i++)
            {
                int y = svm.Compute(inputs[i], out distances[i]);
                Assert.AreEqual(outputs[i], y);
            }


            ProbabilisticOutputLearning target = new ProbabilisticOutputLearning(svm, inputs, outputs);

            double ll0 = target.LogLikelihood(inputs, outputs);

            double ll1 = target.Run();

            double ll2 = target.LogLikelihood(inputs, outputs);

            Assert.AreEqual(3.4256203116918824, ll1);
            Assert.AreEqual(ll1, ll2);
            Assert.IsTrue(ll1 > ll0);

            double[] probs = new double[outputs.Length];
            for (int i = 0; i < outputs.Length; i++)
            {
                int y = svm.Compute(inputs[i], out probs[i]);
                Assert.AreEqual(outputs[i], y);
            }

            Assert.AreEqual(0.25, probs[0], 1e-5);
            Assert.AreEqual(0.75, probs[1], 1e-5);
            Assert.AreEqual(0.75, probs[2], 1e-5);
            Assert.AreEqual(0.25, probs[3], 1e-5);

            foreach (var p in probs)
                Assert.IsFalse(Double.IsNaN(p));

        }

        [TestMethod()]
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

            // Instantiate the probabilistic learning calibration
            ProbabilisticOutputLearning calibration = new ProbabilisticOutputLearning(svm, inputs, labels);

            // Run the calibration algorithm
            double loglikelihood = calibration.Run();


            // Compute the decision output for one of the input vectors,
            // while also retrieving the probability of the answer

            double probability;
            int decision = svm.Compute(inputs[0], out probability);

            // At this point, decision is +1 with a probability of 75%

            Assert.AreEqual(1, decision);
            Assert.AreEqual(0.74999975815069375, probability);
        }


        [TestMethod()]
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
            MulticlassSupportVectorMachine machine = new MulticlassSupportVectorMachine(4, kernel, 3);
            MulticlassSupportVectorLearning target = new MulticlassSupportVectorLearning(machine, inputs, outputs);

            target.Algorithm = (svm, classInputs, classOutputs, i, j) =>
                new SequentialMinimalOptimization(svm, classInputs, classOutputs);

            double error1 = target.Run();
            Assert.AreEqual(0, error1);

            target.Algorithm = (svm, classInputs, classOutputs, i, j) =>
                new ProbabilisticOutputLearning(svm, classInputs, classOutputs);

            double error2 = target.Run();
            Assert.AreEqual(0, error2);


        }


    }
}
