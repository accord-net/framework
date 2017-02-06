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
    using Accord.MachineLearning;
    using Accord.MachineLearning.VectorMachines;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Math;
    using Accord.Math.Optimization.Losses;
    using Accord.Statistics.Kernels;
    using NUnit.Framework;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Accord.Statistics.Filters;

    [TestFixture]
    public class MulticlassSupportVectorLearningTest
    {

        [Test]
        public void RunTest()
        {
            Accord.Math.Random.Generator.Seed = 0;

            // Sample data
            //   The following is a simple auto association function
            //   in which each input correspond to its own class. This
            //   problem should be easily solved using a Linear kernel.

            // Sample input data
            double[][] inputs =
            {
                new double[] { 0 },
                new double[] { 3 },
                new double[] { 1 },
                new double[] { 2 },
            };

            // Output for each of the inputs
            int[] outputs = { 0, 3, 1, 2 };


            // Create a new Linear kernel
            IKernel kernel = new Linear();

            // Create a new Multi-class Support Vector Machine for one input,
            //  using the linear kernel and four disjoint classes.
            var machine = new MulticlassSupportVectorMachine(1, kernel, 4);

            // Create the Multi-class learning algorithm for the machine
            var teacher = new MulticlassSupportVectorLearning(machine, inputs, outputs);

            // Configure the learning algorithm to use SMO to train the
            //  underlying SVMs in each of the binary class subproblems.
            teacher.Algorithm = (svm, classInputs, classOutputs, i, j) =>
                new SequentialMinimalOptimization(svm, classInputs, classOutputs);

            // Run the learning algorithm
            double error = teacher.Run();

            Assert.AreEqual(0, error);
            Assert.AreEqual(0, machine.Compute(inputs[0]));
            Assert.AreEqual(3, machine.Compute(inputs[1]));
            Assert.AreEqual(1, machine.Compute(inputs[2]));
            Assert.AreEqual(2, machine.Compute(inputs[3]));
        }

        [Test]
        public void learn_test()
        {
            #region doc_learn
            // Generate always same random numbers
            Accord.Math.Random.Generator.Seed = 0;

            // The following is a simple auto association function in which 
            // the last column of each input correspond to its own class. This
            // problem should be easily solved using a Linear kernel.

            // Sample input data
            double[][] inputs =
            {
                new double[] { 1, 2, 0 },
                new double[] { 6, 2, 3 },
                new double[] { 1, 1, 1 },
                new double[] { 7, 6, 2 },
            };

            // Output for each of the inputs
            int[] outputs = { 0, 3, 1, 2 };


            // Create the multi-class learning algorithm for the machine
            var teacher = new MulticlassSupportVectorLearning<Linear>()
            {
                // Configure the learning algorithm to use SMO to train the
                //  underlying SVMs in each of the binary class subproblems.
                Learner = (param) => new SequentialMinimalOptimization<Linear>()
                {
                    // If you would like to use other kernels, simply replace
                    // the generic parameter to the desired kernel class, such
                    // as for example, Polynomial or Gaussian:

                    Kernel = new Linear() // use the Linear kernel
                }
            };

            // Estimate the multi-class support vector machine using one-vs-one method
            MulticlassSupportVectorMachine<Linear> ovo = teacher.Learn(inputs, outputs);

            // Obtain class predictions for each sample
            int[] predicted = ovo.Decide(inputs);

            // Compute classification error
            double error = new ZeroOneLoss(outputs).Loss(predicted);
            #endregion

            Assert.AreEqual(0, error);
            Assert.IsTrue(predicted.IsEqual(outputs));
            Assert.IsTrue(ovo.Scores(inputs[0]).IsEqual(new double[] { 0.62, -0.25, -0.59, -0.62 }, 1e-2));
            Assert.IsTrue(ovo.Scores(inputs[1]).IsEqual(new double[] { -0.62, -0.57, -0.13, 0.62 }, 1e-2));
            Assert.IsTrue(ovo.Scores(inputs[2]).IsEqual(new double[] { -0.25, 0.63, -0.63, -0.51 }, 1e-2));
        }

        [Test]
        public void learn_test_wavelet()
        {
            #region doc_learn_wavelet
            // Generate always same random numbers
            Accord.Math.Random.Generator.Seed = 0;

            // The following is a simple auto association function in which 
            // the last column of each input correspond to its own class. This
            // problem should be easily solved using a Linear kernel.

            // Sample input data
            int[][] inputs =
            {
                new int[] { 1, 2, 0 },
                new int[] { 6, 2, 3 },
                new int[] { 1, 1, 1 },
                new int[] { 7, 6, 2 },
            };

            // Output for each of the inputs
            int[] outputs = { 0, 3, 1, 2 };

            // Create the multi-class learning algorithm for the machine
            var teacher = new MulticlassSupportVectorLearning<Wavelet, int[]>()
            {
                // Configure the learning algorithm to use SMO to train the
                //  underlying SVMs in each of the binary class subproblems.
                Learner = (param) => new SequentialMinimalOptimization<Wavelet, int[]>()
                {
                    // If you would like to use other kernels, simply replace
                    // the generic parameter to the desired kernel class, such
                    // as for example, Wavelet:

                    Kernel = new Wavelet(invariant: true) // use the Wavelet kernel
                }
            };

            // Estimate the multi-class support vector machine using one-vs-one method
            var ovo = teacher.Learn(inputs, outputs);

            // Obtain class predictions for each sample
            int[] predicted = ovo.Decide(inputs);

            // Compute classification error
            double error = new ZeroOneLoss(outputs).Loss(predicted);
            #endregion

            Assert.AreEqual(0, error);
            Assert.IsTrue(predicted.IsEqual(outputs));
            var s0 = ovo.Scores(inputs[0]);
            var s1 = ovo.Scores(inputs[1]);
            var s2 = ovo.Scores(inputs[2]);
            Assert.IsTrue(s0.IsEqual(new double[] {  1, -1, -1, -1 }, 1e-2));
            Assert.IsTrue(s1.IsEqual(new double[] { -1, -1, -1,  1 }, 1e-2));
            Assert.IsTrue(s2.IsEqual(new double[] { -1.18, 1.18, -1, -1 }, 1e-2));
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


            double actual = target.Run();
            double expected = 0;

            Assert.AreEqual(expected, actual);

            for (int i = 0; i < inputs.Length; i++)
            {
                actual = machine.Compute(inputs[i]);
                expected = outputs[i];
                Assert.AreEqual(expected, actual);
            }

        }

        [Test]
        public void RunTest3()
        {

            double[][] inputs =
            {
                // Tickets with the following structure should be assigned to location 0
                new double[] { 1, 4, 2, 0, 1 }, // should be assigned to location 0
                new double[] { 1, 3, 2, 0, 1 }, // should be assigned to location 0

                // Tickets with the following structure should be assigned to location 1
                new double[] { 3, 0, 1, 1, 1 }, // should be assigned to location 1
                new double[] { 3, 0, 1, 0, 1 }, // should be assigned to location 1

                // Tickets with the following structure should be assigned to location 2
                new double[] { 0, 5, 5, 5, 5 }, // should be assigned to location 2
                new double[] { 1, 5, 5, 5, 5 }, // should be assigned to location 2

                // Tickets with the following structure should be assigned to location 3
                new double[] { 1, 0, 0, 0, 0 }, // should be assigned to location 3
                new double[] { 1, 0, 0, 0, 0 }, // should be assigned to location 3
            };

            int[] outputs =
            {
                0, 0, // Those are the locations for the first two vectors above
                1, 1, // Those are the locations for the next two vectors above
                2, 2, // Those are the locations for the next two vectors above
                3, 3, // Those are the locations for the last two vectors above
            };

            // Since this is a simplification, a linear machine will suffice:
            IKernel kernel = new Linear();

            // Create the machine for feature vectors of length 5, for 4 possible locations
            MulticlassSupportVectorMachine machine = new MulticlassSupportVectorMachine(5, kernel, 4);

            // Create a new learning algorithm to train the machine
            MulticlassSupportVectorLearning target = new MulticlassSupportVectorLearning(machine, inputs, outputs);

            // Use the standard SMO algorithm
            target.Algorithm = (svm, classInputs, classOutputs, i, j) =>
                new SequentialMinimalOptimization(svm, classInputs, classOutputs);

            // Train the machines
            double actual = target.Run();


            // Compute the answer for all training samples
            for (int i = 0; i < inputs.Length; i++)
            {
                double[] answersWeights;

                double answer = machine.Compute(inputs[i], MulticlassComputeMethod.Voting, out answersWeights);

                // Assert it has been classified correctly
                Assert.AreEqual(outputs[i], answer);

                // Assert the most probable answer is indeed the correct one
                int imax; Matrix.Max(answersWeights, out imax);
                Assert.AreEqual(answer, imax);
            }
        }

        [Test]
        public void LinearTest()
        {

            // Let's say we have the following data to be classified
            // into three possible classes. Those are the samples:
            //
            double[][] inputs =
            {
                //               input         output
                new double[] { 0, 1, 1, 0 }, //  0 
                new double[] { 0, 1, 0, 0 }, //  0
                new double[] { 0, 0, 1, 0 }, //  0
                new double[] { 0, 1, 1, 0 }, //  0
                new double[] { 0, 1, 0, 0 }, //  0
                new double[] { 1, 0, 0, 0 }, //  1
                new double[] { 1, 0, 0, 0 }, //  1
                new double[] { 1, 0, 0, 1 }, //  1
                new double[] { 0, 0, 0, 1 }, //  1
                new double[] { 0, 0, 0, 1 }, //  1
                new double[] { 1, 1, 1, 1 }, //  2
                new double[] { 1, 0, 1, 1 }, //  2
                new double[] { 1, 1, 0, 1 }, //  2
                new double[] { 0, 1, 1, 1 }, //  2
                new double[] { 1, 1, 1, 1 }, //  2
            };

            int[] outputs = // those are the class labels
            {
                0, 0, 0, 0, 0,
                1, 1, 1, 1, 1,
                2, 2, 2, 2, 2,
            };

            // Create a new multi-class linear support vector machine for 3 classes
            var machine = new MulticlassSupportVectorMachine(inputs: 4, classes: 3);

            // Create a one-vs-one learning algorithm using LIBLINEAR's L2-loss SVC dual
            var teacher = new MulticlassSupportVectorLearning(machine, inputs, outputs)
            {
                Algorithm = (svm, classInputs, classOutputs, i, j) =>
                    new LinearDualCoordinateDescent(svm, classInputs, classOutputs)
                    {
                        Loss = Loss.L2
                    }
            };

#if DEBUG
            teacher.ParallelOptions.MaxDegreeOfParallelism = 1;
#endif

            // Teach the machine
            double error = teacher.Run(); // should be 0.

            Assert.AreEqual(0, error);
            for (int i = 0; i < inputs.Length; i++)
            {
                error = machine.Compute(inputs[i]);
                double expected = outputs[i];
                Assert.AreEqual(expected, error);
            }
        }



        [Test]
        public void multiclass_linear_new_usage()
        {
            #region doc_learn_ldcd
            // Let's say we have the following data to be classified
            // into three possible classes. Those are the samples:
            //
            double[][] inputs =
            {
                //               input         output
                new double[] { 0, 1, 1, 0 }, //  0 
                new double[] { 0, 1, 0, 0 }, //  0
                new double[] { 0, 0, 1, 0 }, //  0
                new double[] { 0, 1, 1, 0 }, //  0
                new double[] { 0, 1, 0, 0 }, //  0
                new double[] { 1, 0, 0, 0 }, //  1
                new double[] { 1, 0, 0, 0 }, //  1
                new double[] { 1, 0, 0, 1 }, //  1
                new double[] { 0, 0, 0, 1 }, //  1
                new double[] { 0, 0, 0, 1 }, //  1
                new double[] { 1, 1, 1, 1 }, //  2
                new double[] { 1, 0, 1, 1 }, //  2
                new double[] { 1, 1, 0, 1 }, //  2
                new double[] { 0, 1, 1, 1 }, //  2
                new double[] { 1, 1, 1, 1 }, //  2
            };

            int[] outputs = // those are the class labels
            {
                0, 0, 0, 0, 0,
                1, 1, 1, 1, 1,
                2, 2, 2, 2, 2,
            };

            // Create a one-vs-one multi-class SVM learning algorithm 
            var teacher = new MulticlassSupportVectorLearning<Linear>()
            {
                // using LIBLINEAR's L2-loss SVC dual for each SVM
                Learner = (p) => new LinearDualCoordinateDescent()
                {
                    Loss = Loss.L2
                }
            };

            // Configure parallel execution options
            teacher.ParallelOptions.MaxDegreeOfParallelism = 1;

            // Learn a machine
            var machine = teacher.Learn(inputs, outputs);

            // Obtain class predictions for each sample
            int[] predicted = machine.Decide(inputs);

            // Compute classification error
            double error = new ZeroOneLoss(outputs).Loss(predicted);
            #endregion

            Assert.AreEqual(0, error);
            Assert.IsTrue(predicted.IsEqual(outputs));
        }

        [Test]
        public void multiclass_gaussian_new_usage()
        {
            #region doc_learn_gaussian
            // Let's say we have the following data to be classified
            // into three possible classes. Those are the samples:
            //
            double[][] inputs =
            {
                //               input         output
                new double[] { 0, 1, 1, 0 }, //  0 
                new double[] { 0, 1, 0, 0 }, //  0
                new double[] { 0, 0, 1, 0 }, //  0
                new double[] { 0, 1, 1, 0 }, //  0
                new double[] { 0, 1, 0, 0 }, //  0
                new double[] { 1, 0, 0, 0 }, //  1
                new double[] { 1, 0, 0, 0 }, //  1
                new double[] { 1, 0, 0, 1 }, //  1
                new double[] { 0, 0, 0, 1 }, //  1
                new double[] { 0, 0, 0, 1 }, //  1
                new double[] { 1, 1, 1, 1 }, //  2
                new double[] { 1, 0, 1, 1 }, //  2
                new double[] { 1, 1, 0, 1 }, //  2
                new double[] { 0, 1, 1, 1 }, //  2
                new double[] { 1, 1, 1, 1 }, //  2
            };

            int[] outputs = // those are the class labels
            {
                0, 0, 0, 0, 0,
                1, 1, 1, 1, 1,
                2, 2, 2, 2, 2,
            };

            // Create the multi-class learning algorithm for the machine
            var teacher = new MulticlassSupportVectorLearning<Gaussian>()
            {
                // Configure the learning algorithm to use SMO to train the
                //  underlying SVMs in each of the binary class subproblems.
                Learner = (param) => new SequentialMinimalOptimization<Gaussian>()
                {
                    // Estimate a suitable guess for the Gaussian kernel's parameters.
                    // This estimate can serve as a starting point for a grid search.
                    UseKernelEstimation = true
                }
            };

            // Configure parallel execution options
            teacher.ParallelOptions.MaxDegreeOfParallelism = 1;

            // Learn a machine
            var machine = teacher.Learn(inputs, outputs);

            // Obtain class predictions for each sample
            int[] predicted = machine.Decide(inputs);

            // Get class scores for each sample
            double[] scores = machine.Score(inputs);

            // Compute classification error
            double error = new ZeroOneLoss(outputs).Loss(predicted);
            #endregion

            // Get log-likelihoods (should be same as scores)
            double[][] logl = machine.LogLikelihoods(inputs);

            // Get probability for each sample
            double[][] prob = machine.Probabilities(inputs);

            // Compute classification error
            double loss = new CategoryCrossEntropyLoss(outputs).Loss(prob);

            string str = scores.ToCSharp();
 
            double[] expectedScores =
            { 
                1.00888999727541, 1.00303259868784, 1.00068403386636, 1.00888999727541,
                1.00303259868784, 1.00831890183328, 1.00831890183328, 0.843757409449037, 
                0.996768862332386, 0.996768862332386, 1.02627325826713, 1.00303259868784,
                0.996967401312164, 0.961947708617365, 1.02627325826713
            };

            double[][] expectedLogL =
            {
                new double[] { 1.00888999727541, -1.00888999727541, -1.00135670089335 },
                new double[] { 1.00303259868784, -0.991681098166717, -1.00303259868784 },
                new double[] { 1.00068403386636, -0.54983354268499, -1.00068403386636 },
                new double[] { 1.00888999727541, -1.00888999727541, -1.00135670089335 },
                new double[] { 1.00303259868784, -0.991681098166717, -1.00303259868784 },
                new double[] { -1.00831890183328, 1.00831890183328, -0.0542719287771535 },
                new double[] { -1.00831890183328, 1.00831890183328, -0.0542719287771535 },
                new double[] { -0.843757409449037, 0.843757409449037, -0.787899083913034 },
                new double[] { -0.178272229157676, 0.996768862332386, -0.996768862332386 },
                new double[] { -0.178272229157676, 0.996768862332386, -0.996768862332386 },
                new double[] { -1.02627325826713, -1.00323113766761, 1.02627325826713 },
                new double[] { -1.00303259868784, -0.38657999872922, 1.00303259868784 },
                new double[] { -0.996967401312164, -0.38657999872922, 0.996967401312164 },
                new double[] { -0.479189991343958, -0.961947708617365, 0.961947708617365 },
                new double[] { -1.02627325826713, -1.00323113766761, 1.02627325826713 } 
            };

            double[][] expectedProbs =
            {
                new double[] { 0.789324598208647, 0.104940932711551, 0.105734469079803 },
                new double[] { 0.78704862182644, 0.107080012017624, 0.105871366155937 },
                new double[] { 0.74223157627093, 0.157455631737191, 0.100312791991879 },
                new double[] { 0.789324598208647, 0.104940932711551, 0.105734469079803 },
                new double[] { 0.78704862182644, 0.107080012017624, 0.105871366155937 },
                new double[] { 0.0900153422818135, 0.676287261796794, 0.233697395921392 },
                new double[] { 0.0900153422818135, 0.676287261796794, 0.233697395921392 },
                new double[] { 0.133985810363445, 0.72433118122885, 0.141683008407705 },
                new double[] { 0.213703968297751, 0.692032433073136, 0.0942635986291124 },
                new double[] { 0.213703968297751, 0.692032433073136, 0.0942635986291124 },
                new double[] { 0.10192623206507, 0.104302095948601, 0.79377167198633 },
                new double[] { 0.0972161784678357, 0.180077937396817, 0.722705884135347 },
                new double[] { 0.0981785890979593, 0.180760971768703, 0.721060439133338 },
                new double[] { 0.171157270099157, 0.105617610634377, 0.723225119266465 },
                new double[] { 0.10192623206507, 0.104302095948601, 0.79377167198633 } 
            };

            Assert.AreEqual(0, error);
            Assert.AreEqual(4.5289447815997672, loss, 1e-10);
            Assert.IsTrue(predicted.IsEqual(outputs));
            Assert.IsTrue(expectedScores.IsEqual(scores, 1e-10));
            Assert.IsTrue(expectedLogL.IsEqual(logl, 1e-10));
            Assert.IsTrue(expectedProbs.IsEqual(prob, 1e-10));
        }

        [Test]
        public void multiclass_calibration()
        {
            #region doc_learn_calibration
            // Let's say we have the following data to be classified
            // into three possible classes. Those are the samples:
            //
            double[][] inputs =
            {
                //               input         output
                new double[] { 0, 1, 1, 0 }, //  0 
                new double[] { 0, 1, 0, 0 }, //  0
                new double[] { 0, 0, 1, 0 }, //  0
                new double[] { 0, 1, 1, 0 }, //  0
                new double[] { 0, 1, 0, 0 }, //  0
                new double[] { 1, 0, 0, 1 }, //  1
                new double[] { 0, 0, 0, 1 }, //  1
                new double[] { 0, 0, 0, 1 }, //  1
                new double[] { 1, 0, 1, 1 }, //  2
                new double[] { 1, 1, 0, 1 }, //  2
                new double[] { 0, 1, 1, 1 }, //  2
                new double[] { 1, 1, 1, 1 }, //  2
            };

            int[] outputs = // those are the class labels
            {
                0, 0, 0, 0, 0,
                1, 1, 1, 
                2, 2, 2, 2, 
            };

            // Create the multi-class learning algorithm for the machine
            var teacher = new MulticlassSupportVectorLearning<Gaussian>()
            {
                // Configure the learning algorithm to use SMO to train the
                //  underlying SVMs in each of the binary class subproblems.
                Learner = (param) => new SequentialMinimalOptimization<Gaussian>()
                {
                    // Estimate a suitable guess for the Gaussian kernel's parameters.
                    // This estimate can serve as a starting point for a grid search.
                    UseKernelEstimation = true
                }
            };

            // Learn a machine
            var machine = teacher.Learn(inputs, outputs);


            // Create the multi-class learning algorithm for the machine
            var calibration = new MulticlassSupportVectorLearning<Gaussian>()
            {
                Model = machine, // We will start with an existing machine

                // Configure the learning algorithm to use Platt's calibration
                Learner = (param) => new ProbabilisticOutputCalibration<Gaussian>()
                {
                    Model = param.Model // Start with an existing machine
                }
            };


            // Configure parallel execution options
            calibration.ParallelOptions.MaxDegreeOfParallelism = 1;

            // Learn a machine
            calibration.Learn(inputs, outputs);

            // Obtain class predictions for each sample
            int[] predicted = machine.Decide(inputs);

            // Get class scores for each sample
            double[] scores = machine.Score(inputs);

            // Get log-likelihoods (should be same as scores)
            double[][] logl = machine.LogLikelihoods(inputs);

            // Get probability for each sample
            double[][] prob = machine.Probabilities(inputs);

            // Compute classification error
            double error = new ZeroOneLoss(outputs).Loss(predicted);
            double loss = new CategoryCrossEntropyLoss(outputs).Loss(prob);
            #endregion

            //string str = logl.ToCSharp();

            double[] expectedScores =
            {
                1.87436400885238, 1.81168086449304, 1.74038320983522, 
                1.87436400885238, 1.81168086449304, 1.55446926953952, 
                1.67016543853596, 1.67016543853596, 1.83135194001403, 
                1.83135194001403, 1.59836868669125, 2.0618816310294 
            };

            double[][] expectedLogL =
            {
                new double[] { 1.87436400885238, -1.87436400885238, -1.7463646841257 },
                new double[] { 1.81168086449304, -1.81168086449304, -1.73142460658826 },
                new double[] { 1.74038320983522, -1.58848669816072, -1.74038320983522 },
                new double[] { 1.87436400885238, -1.87436400885238, -1.7463646841257 },
                new double[] { 1.81168086449304, -1.81168086449304, -1.73142460658826 },
                new double[] { -1.55446926953952, 1.55446926953952, -0.573599079216229 },
                new double[] { -0.368823000428743, 1.67016543853596, -1.67016543853596 },
                new double[] { -0.368823000428743, 1.67016543853596, -1.67016543853596 },
                new double[] { -1.83135194001403, -1.20039293330558, 1.83135194001403 },
                new double[] { -1.83135194001403, -1.20039293330558, 1.83135194001403 },
                new double[] { -0.894598978116595, -1.59836868669125, 1.59836868669125 },
                new double[] { -1.87336852014759, -2.0618816310294, 2.0618816310294 } 
            };

            double[][] expectedProbs =
            {
                new double[] { 0.95209908906855, 0.0224197237689656, 0.0254811871624848 },
                new double[] { 0.947314032745205, 0.0252864560196241, 0.0273995112351714 },
                new double[] { 0.937543314993345, 0.0335955309754816, 0.028861154031173 },
                new double[] { 0.95209908906855, 0.0224197237689656, 0.0254811871624848 },
                new double[] { 0.947314032745205, 0.0252864560196241, 0.0273995112351714 },
                new double[] { 0.0383670466237636, 0.859316640577158, 0.102316312799079 },
                new double[] { 0.111669460983068, 0.857937888238824, 0.0303926507781076 },
                new double[] { 0.111669460983068, 0.857937888238824, 0.0303926507781076 },
                new double[] { 0.0238971617859334, 0.0449126146360623, 0.931190223578004 },
                new double[] { 0.0238971617859334, 0.0449126146360623, 0.931190223578004 },
                new double[] { 0.0735735561383806, 0.0363980776342206, 0.890028366227399 },
                new double[] { 0.0188668069460003, 0.0156252941482294, 0.96550789890577 } 
            };

            Assert.AreEqual(0, error);
            Assert.AreEqual(3, machine.Count);
            Assert.AreEqual(0.5, machine[0].Value.Kernel.Gamma);
            Assert.AreEqual(0.5, machine[1].Value.Kernel.Gamma);
            Assert.AreEqual(0.5, machine[2].Value.Kernel.Gamma);
            Assert.AreEqual(1.0231652126930515, loss);
            Assert.IsTrue(predicted.IsEqual(outputs));
            Assert.IsTrue(expectedScores.IsEqual(scores, 1e-10));
            Assert.IsTrue(expectedLogL.IsEqual(logl, 1e-10));
            Assert.IsTrue(expectedProbs.IsEqual(prob, 1e-10));
        }

        [Test]
        public void multiclass_calibration_generic_kernel()
        {
            // Let's say we have the following data to be classified
            // into three possible classes. Those are the samples:
            //
            double[][] inputs =
            {
                //               input         output
                new double[] { 0, 1, 1, 0 }, //  0 
                new double[] { 0, 1, 0, 0 }, //  0
                new double[] { 0, 0, 1, 0 }, //  0
                new double[] { 0, 1, 1, 0 }, //  0
                new double[] { 0, 1, 0, 0 }, //  0
                new double[] { 1, 0, 0, 1 }, //  1
                new double[] { 0, 0, 0, 1 }, //  1
                new double[] { 0, 0, 0, 1 }, //  1
                new double[] { 1, 0, 1, 1 }, //  2
                new double[] { 1, 1, 0, 1 }, //  2
                new double[] { 0, 1, 1, 1 }, //  2
                new double[] { 1, 1, 1, 1 }, //  2
            };

            int[] outputs = // those are the class labels
            {
                0, 0, 0, 0, 0,
                1, 1, 1, 
                2, 2, 2, 2, 
            };

            // Create the multi-class learning algorithm for the machine
            var teacher = new MulticlassSupportVectorLearning<IKernel>()
            {
                // Configure the learning algorithm to use SMO to train the
                //  underlying SVMs in each of the binary class subproblems.
                Learner = (param) => new SequentialMinimalOptimization<IKernel>()
                {
                    UseKernelEstimation = false,
                    Kernel = Gaussian.FromGamma(0.5)
                }
            };

            // Learn a machine
            var machine = teacher.Learn(inputs, outputs);


            // Create the multi-class learning algorithm for the machine
            var calibration = new MulticlassSupportVectorLearning<IKernel>(machine)
            {
                // Configure the learning algorithm to use SMO to train the
                //  underlying SVMs in each of the binary class subproblems.
                Learner = (param) => new ProbabilisticOutputCalibration<IKernel>(param.Model)
            };


            // Configure parallel execution options
            calibration.ParallelOptions.MaxDegreeOfParallelism = 1;

            // Learn a machine
            calibration.Learn(inputs, outputs);

            // Obtain class predictions for each sample
            int[] predicted = machine.Decide(inputs);

            // Get class scores for each sample
            double[] scores = machine.Score(inputs);

            // Get log-likelihoods (should be same as scores)
            double[][] logl = machine.LogLikelihoods(inputs);

            // Get probability for each sample
            double[][] prob = machine.Probabilities(inputs);

            // Compute classification error
            double error = new ZeroOneLoss(outputs).Loss(predicted);
            double loss = new CategoryCrossEntropyLoss(outputs).Loss(prob);
            

            //string str = logl.ToCSharp();

            double[] expectedScores =
            {
                1.87436400885238, 1.81168086449304, 1.74038320983522, 
                1.87436400885238, 1.81168086449304, 1.55446926953952, 
                1.67016543853596, 1.67016543853596, 1.83135194001403, 
                1.83135194001403, 1.59836868669125, 2.0618816310294 
            };

            double[][] expectedLogL =
            {
                new double[] { 1.87436400885238, -1.87436400885238, -1.7463646841257 },
                new double[] { 1.81168086449304, -1.81168086449304, -1.73142460658826 },
                new double[] { 1.74038320983522, -1.58848669816072, -1.74038320983522 },
                new double[] { 1.87436400885238, -1.87436400885238, -1.7463646841257 },
                new double[] { 1.81168086449304, -1.81168086449304, -1.73142460658826 },
                new double[] { -1.55446926953952, 1.55446926953952, -0.573599079216229 },
                new double[] { -0.368823000428743, 1.67016543853596, -1.67016543853596 },
                new double[] { -0.368823000428743, 1.67016543853596, -1.67016543853596 },
                new double[] { -1.83135194001403, -1.20039293330558, 1.83135194001403 },
                new double[] { -1.83135194001403, -1.20039293330558, 1.83135194001403 },
                new double[] { -0.894598978116595, -1.59836868669125, 1.59836868669125 },
                new double[] { -1.87336852014759, -2.0618816310294, 2.0618816310294 } 
            };

            double[][] expectedProbs =
            {
                new double[] { 0.95209908906855, 0.0224197237689656, 0.0254811871624848 },
                new double[] { 0.947314032745205, 0.0252864560196241, 0.0273995112351714 },
                new double[] { 0.937543314993345, 0.0335955309754816, 0.028861154031173 },
                new double[] { 0.95209908906855, 0.0224197237689656, 0.0254811871624848 },
                new double[] { 0.947314032745205, 0.0252864560196241, 0.0273995112351714 },
                new double[] { 0.0383670466237636, 0.859316640577158, 0.102316312799079 },
                new double[] { 0.111669460983068, 0.857937888238824, 0.0303926507781076 },
                new double[] { 0.111669460983068, 0.857937888238824, 0.0303926507781076 },
                new double[] { 0.0238971617859334, 0.0449126146360623, 0.931190223578004 },
                new double[] { 0.0238971617859334, 0.0449126146360623, 0.931190223578004 },
                new double[] { 0.0735735561383806, 0.0363980776342206, 0.890028366227399 },
                new double[] { 0.0188668069460003, 0.0156252941482294, 0.96550789890577 } 
            };

            // Must be exactly the same as test above
            Assert.AreEqual(0, error);
            Assert.AreEqual(0.5, ((Gaussian)machine[0].Value.Kernel).Gamma);
            Assert.AreEqual(0.5, ((Gaussian)machine[1].Value.Kernel).Gamma);
            Assert.AreEqual(0.5, ((Gaussian)machine[2].Value.Kernel).Gamma);
            Assert.AreEqual(1.0231652126930515, loss);
            Assert.IsTrue(predicted.IsEqual(outputs));
            Assert.IsTrue(expectedScores.IsEqual(scores, 1e-10));
            Assert.IsTrue(expectedLogL.IsEqual(logl, 1e-10));
            Assert.IsTrue(expectedProbs.IsEqual(prob, 1e-10));
        }

        [Test]
        public void multiclass_linear_smo_new_usage()
        {

            // Let's say we have the following data to be classified
            // into three possible classes. Those are the samples:
            //
            double[][] inputs =
            {
                //               input         output
                new double[] { 0, 1, 1, 0 }, //  0 
                new double[] { 0, 1, 0, 0 }, //  0
                new double[] { 0, 0, 1, 0 }, //  0
                new double[] { 0, 1, 1, 0 }, //  0
                new double[] { 0, 1, 0, 0 }, //  0
                new double[] { 1, 0, 0, 0 }, //  1
                new double[] { 1, 0, 0, 0 }, //  1
                new double[] { 1, 0, 0, 1 }, //  1
                new double[] { 0, 0, 0, 1 }, //  1
                new double[] { 0, 0, 0, 1 }, //  1
                new double[] { 1, 1, 1, 1 }, //  2
                new double[] { 1, 0, 1, 1 }, //  2
                new double[] { 1, 1, 0, 1 }, //  2
                new double[] { 0, 1, 1, 1 }, //  2
                new double[] { 1, 1, 1, 1 }, //  2
            };

            int[] outputs = // those are the class labels
            {
                0, 0, 0, 0, 0,
                1, 1, 1, 1, 1,
                2, 2, 2, 2, 2,
            };

            // Create a one-vs-one learning algorithm using LIBLINEAR's L2-loss SVC dual
            var teacher = new MulticlassSupportVectorLearning<Linear>();
            teacher.Learner = (p) => new SequentialMinimalOptimization<Linear>()
            {
                UseComplexityHeuristic = true
            };

#if DEBUG
            teacher.ParallelOptions.MaxDegreeOfParallelism = 1;
#endif

            // Learn a machine
            var machine = teacher.Learn(inputs, outputs);

            for (int i = 0; i < inputs.Length; i++)
            {
                double actual = machine.Decide(inputs[i]);
                double expected = outputs[i];
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void multiclass_precomputed_matrix_smo()
        {
            #region doc_precomputed
            // Let's say we have the following data to be classified
            // into three possible classes. Those are the samples:
            //
            double[][] trainInputs =
            {
                //               input         output
                new double[] { 0, 1, 1, 0 }, //  0 
                new double[] { 0, 1, 0, 0 }, //  0
                new double[] { 0, 0, 1, 0 }, //  0
                new double[] { 0, 1, 1, 0 }, //  0
                new double[] { 0, 1, 0, 0 }, //  0
                new double[] { 1, 0, 0, 0 }, //  1
                new double[] { 1, 0, 0, 0 }, //  1
                new double[] { 1, 0, 0, 1 }, //  1
                new double[] { 0, 0, 0, 1 }, //  1
                new double[] { 0, 0, 0, 1 }, //  1
                new double[] { 1, 1, 1, 1 }, //  2
                new double[] { 1, 0, 1, 1 }, //  2
                new double[] { 1, 1, 0, 1 }, //  2
                new double[] { 0, 1, 1, 1 }, //  2
                new double[] { 1, 1, 1, 1 }, //  2
            };

            int[] trainOutputs = // those are the training set class labels
            {
                0, 0, 0, 0, 0,
                1, 1, 1, 1, 1,
                2, 2, 2, 2, 2,
            };

            // Let's chose a kernel function
            Polynomial kernel = new Polynomial(2);

            // Get the kernel matrix for the training set
            double[][] K = kernel.ToJagged(trainInputs);

            // Create a pre-computed kernel
            var pre  = new Precomputed(K);

            // Create a one-vs-one learning algorithm using SMO
            var teacher = new MulticlassSupportVectorLearning<Precomputed, int>()
            {
                Learner = (p) => new SequentialMinimalOptimization<Precomputed, int>()
                {
                    Kernel = pre
                }
            };

#if DEBUG
            teacher.ParallelOptions.MaxDegreeOfParallelism = 1;
#endif

            // Learn a machine
            var machine = teacher.Learn(pre.Indices, trainOutputs);

            // Compute the machine's prediction for the training set
            int[] trainPrediction = machine.Decide(pre.Indices);

            // Evaluate prediction error for the training set using mean accuracy (mAcc)
            double trainingError = new ZeroOneLoss(trainOutputs).Loss(trainPrediction);

            // Now let's compute the machine's prediction for a test set
            double[][] testInputs = // test-set inputs
            {
                //               input         output
                new double[] { 0, 1, 1, 0 }, //  0 
                new double[] { 0, 1, 0, 0 }, //  0
                new double[] { 0, 0, 0, 1 }, //  1
                new double[] { 1, 1, 1, 1 }, //  2
            };

            int[] testOutputs = // those are the test set class labels
            {
                0, 0,  1,  2, 
            };

            // Compute precomputed matrix between train and testing
            pre.Values = kernel.ToJagged2(trainInputs, testInputs);

            // Update the kernel
            machine.Kernel = pre;

            // Compute the machine's prediction for the test set
            int[] testPrediction = machine.Decide(pre.Indices);

            // Evaluate prediction error for the training set using mean accuracy (mAcc)
            double testError = new ZeroOneLoss(testOutputs).Loss(testPrediction);
            #endregion


            Assert.AreEqual(0, trainingError);
            Assert.AreEqual(0, testError);

            // Create a one-vs-one learning algorithm using SMO
            var teacher2 = new MulticlassSupportVectorLearning<Polynomial>()
            {
                Learner = (p) => new SequentialMinimalOptimization<Polynomial>()
                {
                    Kernel = kernel
                }
            };

#if DEBUG
            teacher.ParallelOptions.MaxDegreeOfParallelism = 1;
#endif

            // Learn a machine
            var expected = teacher2.Learn(trainInputs, trainOutputs);

            Assert.AreEqual(4, expected.NumberOfInputs);
            Assert.AreEqual(3, expected.NumberOfOutputs);
            Assert.AreEqual(0, machine.NumberOfInputs);
            Assert.AreEqual(3, machine.NumberOfOutputs);

            var machines = Enumerable.Zip(machine, expected, (a,b) => Tuple.Create(a.Value, b.Value));

            foreach (var pair in machines)
            {
                var a = pair.Item1;
                var e = pair.Item2;

                Assert.AreEqual(0, a.NumberOfInputs);
                Assert.AreEqual(2, a.NumberOfOutputs);

                Assert.AreEqual(4, e.NumberOfInputs);
                Assert.AreEqual(2, e.NumberOfOutputs);

                Assert.IsTrue(a.Weights.IsEqual(e.Weights));
            }
        }

        [Test]
        public void output_labels_test()
        {
            #region doc_learn_codification
            // Let's say we have the following data to be classified
            // into three possible classes. Those are the samples:
            //
            double[][] inputs =
            {
                //               input         output
                new double[] { 0, 1, 1, 0 }, //  0 
                new double[] { 0, 1, 0, 0 }, //  0
                new double[] { 0, 0, 1, 0 }, //  0
                new double[] { 0, 1, 1, 0 }, //  0
                new double[] { 0, 1, 0, 0 }, //  0
                new double[] { 1, 0, 0, 0 }, //  1
                new double[] { 1, 0, 0, 0 }, //  1
                new double[] { 1, 0, 0, 1 }, //  1
                new double[] { 0, 0, 0, 1 }, //  1
                new double[] { 0, 0, 0, 1 }, //  1
                new double[] { 1, 1, 1, 1 }, //  2
                new double[] { 1, 0, 1, 1 }, //  2
                new double[] { 1, 1, 0, 1 }, //  2
                new double[] { 0, 1, 1, 1 }, //  2
                new double[] { 1, 1, 1, 1 }, //  2
            };

            // Now, suppose that our class labels are not contiguous. We
            // have 3 classes, but they have the class labels 5, 1, and 8
            // respectively. In this case, we can use a Codification filter
            // to obtain a contiguous zero-indexed labeling before learning
            int[] output_labels = 
            {
                5, 5, 5, 5, 5,
                1, 1, 1, 1, 1,
                8, 8, 8, 8, 8,
            };

            // Create a codification object to obtain a output mapping
            var codebook = new Codification<int>().Learn(output_labels);

            // Transform the original labels using the codebook
            int[] outputs = codebook.Transform(output_labels);

            // Create the multi-class learning algorithm for the machine
            var teacher = new MulticlassSupportVectorLearning<Gaussian>()
            {
                // Configure the learning algorithm to use SMO to train the
                //  underlying SVMs in each of the binary class subproblems.
                Learner = (param) => new SequentialMinimalOptimization<Gaussian>()
                {
                    // Estimate a suitable guess for the Gaussian kernel's parameters.
                    // This estimate can serve as a starting point for a grid search.
                    UseKernelEstimation = true
                }
            };

            // Configure parallel execution options
            teacher.ParallelOptions.MaxDegreeOfParallelism = 1;

            // Learn a machine
            var machine = teacher.Learn(inputs, outputs);

            // Obtain class predictions for each sample
            int[] predicted = machine.Decide(inputs);

            // Translate the integers back to the original lagbels
            int[] predicted_labels = codebook.Revert(predicted);
            #endregion

            Assert.IsTrue(predicted_labels.IsEqual(output_labels));
        }

    }
}
