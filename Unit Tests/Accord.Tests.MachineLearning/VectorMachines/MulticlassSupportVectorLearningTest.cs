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
    using System.IO;
    using Accord.Statistics.Analysis;
    using Accord.IO;
    using Accord.Tests.MachineLearning.Properties;
    using System.Data;
    using Accord.DataSets;

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
            Assert.AreEqual(1, machine.NumberOfInputs);
            Assert.AreEqual(4, machine.NumberOfOutputs);
            Assert.AreEqual(4, machine.NumberOfClasses);

            // Create the Multi-class learning algorithm for the machine
            var teacher = new MulticlassSupportVectorLearning(machine, inputs, outputs);

            // Configure the learning algorithm to use SMO to train the
            //  underlying SVMs in each of the binary class subproblems.
            teacher.Algorithm = (svm, classInputs, classOutputs, i, j) =>
                new SequentialMinimalOptimization(svm, classInputs, classOutputs);

            // Run the learning algorithm
            double error = teacher.Run();

            Assert.AreEqual(1, machine.NumberOfInputs);
            Assert.AreEqual(4, machine.NumberOfOutputs);
            Assert.AreEqual(4, machine.NumberOfClasses);
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

            Assert.AreEqual(3, ovo.NumberOfInputs);
            Assert.AreEqual(4, ovo.NumberOfOutputs);
            Assert.AreEqual(4, ovo.NumberOfClasses);

            Assert.AreEqual(0, error);
            Assert.IsTrue(predicted.IsEqual(outputs));
            Assert.IsTrue(ovo.Scores(inputs[0]).IsEqual(new double[] { 0.62, -0.25, -0.59, -0.62 }, 1e-2));
            Assert.IsTrue(ovo.Scores(inputs[1]).IsEqual(new double[] { -0.62, -0.57, -0.13, 0.62 }, 1e-2));
            Assert.IsTrue(ovo.Scores(inputs[2]).IsEqual(new double[] { -0.25, 0.63, -0.63, -0.51 }, 1e-2));

            #region doc_learn_decision_path
            // The decision process in a multi-class SVM is actually based on a series of
            // smaller, binary decisions combined together using a one-vs-one strategy. It
            // is possible to determine the results of each of those internal one-vs-one
            // decisions using:

            // First, call Decide, Scores, LogLikelihood or Probability methods:
            int y = ovo.Decide(new double[] { 6, 2, 3 }); // result should be 3

            // Now, call method GetLastDecisionPath():
            Decision[] path = ovo.GetLastDecisionPath(); // contains 3 decisions

            // The binary decisions obtained while computing the last decision
            // above (i.e. the last call to the Decide method), were:
            //
            //  - Class 0 vs. class 3, winner was 3
            //  - Class 1 vs. class 3, winner was 3
            //  - Class 2 vs. class 3, winner was 3

            // The GetLastDecisionPath() method is thread-safe and will always
            // return the last computed path in the current calling thread.
            #endregion

            Assert.AreEqual(3, y);
            Assert.AreEqual(0, path[0].Pair.Class1);
            Assert.AreEqual(1, path[1].Pair.Class1);
            Assert.AreEqual(2, path[2].Pair.Class1);
            Assert.AreEqual(3, path[0].Pair.Class2);
            Assert.AreEqual(3, path[1].Pair.Class2);
            Assert.AreEqual(3, path[2].Pair.Class2);
            Assert.AreEqual(3, path[0].Winner);
            Assert.AreEqual(3, path[1].Winner);
            Assert.AreEqual(3, path[2].Winner);
        }

        [Test]
        public void learn_test_iris()
        {
            #region doc_learn_iris_confusion_matrix
            // Generate always same random numbers
            Accord.Math.Random.Generator.Seed = 0;

            // Let's say we would like to learn a classifier for the famous Iris
            // dataset, and measure its performance using a GeneralConfusionMatrix

            // Download and load the Iris dataset
            var iris = new Iris();
            double[][] inputs = iris.Instances;
            int[] outputs = iris.ClassLabels;

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

            // Compute classification error
            GeneralConfusionMatrix cm = GeneralConfusionMatrix.Estimate(ovo, inputs, outputs);

            double error = cm.Error;         // should be 0.066666666666666652
            double accuracy = cm.Accuracy;   // should be 0.93333333333333335
            double kappa = cm.Kappa;         // should be 0.9
            double chiSquare = cm.ChiSquare; // should be 248.52216748768473
            #endregion

            Assert.AreEqual(0.066666666666666652, error);
            Assert.AreEqual(0.93333333333333335, accuracy);
            Assert.AreEqual(0.9, kappa);
            Assert.AreEqual(248.52216748768473, chiSquare);
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

            // You can set extra properties to configure the learning if you would like:
            teacher.AggregateExceptions = true;
            teacher.ParallelOptions.MaxDegreeOfParallelism = 1;

            // Estimate the multi-class support vector machine using one-vs-one method
            var ovo = teacher.Learn(inputs, outputs);

            // Obtain class predictions for each sample
            int[] predicted = ovo.Decide(inputs);

            // Compute classification error
            double error = new ZeroOneLoss(outputs).Loss(predicted);
            #endregion

            Assert.AreEqual(3, ovo.NumberOfInputs);
            Assert.AreEqual(4, ovo.NumberOfOutputs);
            Assert.AreEqual(4, ovo.NumberOfClasses);

            Assert.AreEqual(0, error);
            Assert.IsTrue(predicted.IsEqual(outputs));
            var s0 = ovo.Scores(inputs[0]);
            var s1 = ovo.Scores(inputs[1]);
            var s2 = ovo.Scores(inputs[2]);
            Assert.IsTrue(s0.IsEqual(new double[] { 1, -1, -1, -1 }, 1e-2));
            Assert.IsTrue(s1.IsEqual(new double[] { -1, -1, -1, 1 }, 1e-2));
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
            Assert.AreEqual(4, machine.NumberOfInputs);
            Assert.AreEqual(3, machine.NumberOfOutputs);
            Assert.AreEqual(3, machine.NumberOfClasses);

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

            Assert.AreEqual(5, machine.NumberOfInputs);
            Assert.AreEqual(4, machine.NumberOfOutputs);
            Assert.AreEqual(4, machine.NumberOfClasses);
        }

        [Test]
        public void LinearTest()
        {
            Accord.Math.Random.Generator.Seed = 0;

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

            Assert.IsTrue(Accord.Math.Random.Generator.HasBeenAccessed);
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

            // The following line is only needed to ensure reproducible results. Please remove it to enable full parallelization
            teacher.ParallelOptions.MaxDegreeOfParallelism = 1; // (Remove, comment, or change this line to enable full parallelism)

            // Learn a machine
            var machine = teacher.Learn(inputs, outputs);

            // Obtain class predictions for each sample
            int[] predicted = machine.Decide(inputs);

            // Compute classification error
            double error = new ZeroOneLoss(outputs).Loss(predicted);
            #endregion

            Assert.AreEqual(4, machine.NumberOfInputs);
            Assert.AreEqual(3, machine.NumberOfOutputs);
            Assert.AreEqual(3, machine.NumberOfClasses);

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

            // The following line is only needed to ensure reproducible results. Please remove it to enable full parallelization
            teacher.ParallelOptions.MaxDegreeOfParallelism = 1; // (Remove, comment, or change this line to enable full parallelism)

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
            Assert.AreEqual(1.0231652126930515, loss, 1e-8);
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

            teacher.ParallelOptions.MaxDegreeOfParallelism = 1;
            teacher.AggregateExceptions = false;

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
            Assert.AreEqual(1.0231652126930515, loss, 1e-8);
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
            var pre = new Precomputed(K);

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
            Assert.AreEqual(1, machine.NumberOfInputs);
            Assert.AreEqual(3, machine.NumberOfOutputs);

            var machines = Enumerable.Zip(machine, expected, (a, b) => Tuple.Create(a.Value, b.Value));

            foreach (var pair in machines)
            {
                var a = pair.Item1;
                var e = pair.Item2;

                Assert.AreEqual(1, a.NumberOfInputs);
                Assert.AreEqual(2, a.NumberOfClasses);
                Assert.AreEqual(1, a.NumberOfOutputs);

                Assert.AreEqual(4, e.NumberOfInputs);
                Assert.AreEqual(2, e.NumberOfClasses);
                Assert.AreEqual(1, e.NumberOfOutputs);

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

            // The following line is only needed to ensure reproducible results. Please remove it to enable full parallelization
            teacher.ParallelOptions.MaxDegreeOfParallelism = 1; // (Remove, comment, or change this line to enable full parallelism)

            // Learn a machine
            var machine = teacher.Learn(inputs, outputs);

            // Obtain class predictions for each sample
            int[] predicted = machine.Decide(inputs);

            // Translate the integers back to the original lagbels
            int[] predicted_labels = codebook.Revert(predicted);
            #endregion

            Assert.IsTrue(predicted_labels.IsEqual(output_labels));
        }



        [Test]
        public void index_out_range_test()
        {
            Accord.Math.Random.Generator.Seed = 1;

            var x = new Accord.IO.CsvReader(new StringReader(Properties.Resources.data16), hasHeaders: false).ToJagged<double>();
            var y = new Accord.IO.CsvReader(new StringReader(Properties.Resources.labels16), hasHeaders: false).ToJagged<int>().GetColumn(0);

            var msvl = new MulticlassSupportVectorLearning<Linear>()
            {
                Learner = (param) => new LinearDualCoordinateDescent<Linear>()
                {
                    Complexity = 10000000,
                }
            };

            msvl.ParallelOptions.MaxDegreeOfParallelism = 1;

            var svm = msvl.Learn(x, y);

            var actual = svm.Decide(x);

            var cm = new GeneralConfusionMatrix(actual, y);

#if MONO
            Assert.IsTrue(cm.Accuracy > 0.90);
#else
            Assert.AreEqual(0.95, cm.Accuracy, 0.02);
#endif
        }

        [Test]
        [Category("Slow")]
        public void dynamic_time_warp_issue_470()
        {
            var instances = CsvReader.FromText(Resources.Shapes, hasHeaders: false).Select(x => new
            {
                InstanceId = int.Parse(x[0]),
                ClassId = int.Parse(x[1]),
                X = Double.Parse(x[3]),
                Y = Double.Parse(x[4])
            });

            var Sequences = (from a in instances
                             group a by a.InstanceId into g
                             select new
                             {
                                 InstanceId = g.Key,
                                 ClassID = g.First().ClassId,
                                 Values = Enumerable.Zip(g.Select(x => x.X), g.Select(x => x.Y),
                                     (a, b) => new double[] { a, b }).ToArray()
                             }).ToList();



            double[][][] inputs = Sequences.Select(x => x.Values).ToArray(); // X,Y values sequences from attached file.
            int[] outputs = Sequences.Select(x => x.ClassID).ToArray();      // Class 0,1,2


            // Create the learning algorithm to teach the multiple class classifier
            var teacher = new MulticlassSupportVectorLearning<DynamicTimeWarping, double[][]>()
            {
                Learner = (param) => new SequentialMinimalOptimization<DynamicTimeWarping, double[][]>()
                {
                    Kernel = new DynamicTimeWarping(length: 2), // (x, y) pairs
                }
            };

            teacher.ParallelOptions.MaxDegreeOfParallelism = 1;
            teacher.AggregateExceptions = false;

            // Learn the SVM using the SMO algorithm
            var svm = teacher.Learn(inputs, outputs);

            // Compute predicted values (at once)
            int[] predicted = svm.Decide(inputs);

            // Compute individual values and compare
            for (int i = 0; i < inputs.Length; i++)
            {
                int outClass = svm.Decide(inputs[i]);
                Assert.AreEqual(predicted[i], outClass);
            }


            Assert.AreEqual(3, svm.NumberOfClasses);
            Assert.AreEqual(0, svm.NumberOfInputs);
            Assert.AreEqual(3, svm.NumberOfOutputs);
        }

        [Test]
        public void multilabel_probabilities()
        {
            double[][] inputs =
            {
                new double[] { 10.1, 0.1, 0.01 },
                new double[] { 10.1, 0.2, 0.01 },
                new double[] { 10.2, 0.3, 0.02 },
                new double[] { 10.1, 0.1, 0.01 },

                new double[] { 0.1, 10.1, 0.01 },
                new double[] { 0.2, 10.2, 0.01 },
                new double[] { 0.1, 10.1, 0.01 },

                new double[] { 0.01, 0.01, 10.1 },
                new double[] { 0.01, 0.01, 10.1 },
                new double[] { 0.01, 0.01, 10.2 },
            };

            int[] outputs = { 0, 0, 0, 0, 1, 1, 1, 2, 2, 2 };

            double[][] test = new[] { inputs[0], inputs[4], inputs[8] };

            double[][] a = new double[][] {
                new double[] { 1.00317837043652, -0.999133032432786, -1.00845270022214 },
                new double[] { -0.994915031332965, 0.996905863560959, -1.00252124694466 },
                new double[] { -0.996821629563478, -1.00309413643904, 1.00845270022214 }
            };

            double[][] b = new double[][] {
                new double[] { -0.312407884659696, -1.31262795734067, -1.31944812115402 },
                new double[] { -1.30954682143033, -0.314094770568381, -1.31510549138339 },
                new double[] { -1.31093910612127, -1.31552462320663, -0.310995420931884 }
            };

            double[][] c = new double[][] {
                new double[] { 0.577001726129664, 0.212220363519678, 0.210777910350659 },
                new double[] { 0.212747668321341, 0.575683976685405, 0.211568354993254 },
                new double[] { 0.212154153006434, 0.211183543586879, 0.576662303406687 }
            };

            double[][] d = new double[][] {
                new double[] { 0.731683025073335, 0.134615977120729, 0.133700997805936 },
                new double[] { 0.135149684317059, 0.730449799736373, 0.134400515946568 },
                new double[] { 0.133947791377159, 0.133334977599079, 0.732717231023761 }
            };

            double[][] i = new double[][] {
                new double[] { 0.731683025073335, 0.269111911681433, 0.267282768976239 },
                new double[] { 0.269942360762677, 0.730449799736373, 0.26844600300527 },
                new double[] { 0.269566785660751, 0.268333512318449, 0.732717231023761 }
            };

            {
                var teacher = new MultilabelSupportVectorLearning<Gaussian>()
                {
                    Learner = (param) => new SequentialMinimalOptimization<Gaussian>()
                    {
                        Complexity = 10000,
                    }
                };

                // Learn a machine
                var machine = teacher.Learn(inputs, outputs);

                Assert.AreEqual(MultilabelProbabilityMethod.SumsToOne, machine.Method);
                compare(test, machine, new double[][][] { a, b, c });

                machine.Method = MultilabelProbabilityMethod.SumsToOneWithEmphasisOnWinner;
                compare(test, machine, new double[][][] { a, b, d });
            }

            {
                var teacher = new MultilabelSupportVectorLearning<Gaussian>()
                {
                    Learner = (param) => new SequentialMinimalOptimization<Gaussian>()
                    {
                        Complexity = 10000,
                    },

                    IsMultilabel = true
                };

                // Learn a machine
                var machine = teacher.Learn(inputs, outputs);

                Assert.AreEqual(MultilabelProbabilityMethod.PerClass, machine.Method);
                compare(test, machine, new double[][][] { a, b, i });

                machine.Method = MultilabelProbabilityMethod.SumsToOneWithEmphasisOnWinner;
                compare(test, machine, new double[][][] { a, b, d });
            }


            double[][] e = new double[][] {
                new double[] { 1.61361630987002, -2.07900515334969, -2.08549355048741 },
                new double[] { -1.94610890503965, 1.38915297719873, -2.07527218562595 },
                new double[] { -1.94950562602869, -2.08588765202885, 1.39013460076266 }
            };

            double[][] f = new double[][] {
                new double[] { -0.181626368526726, -2.19683668600356, -2.2026059466164 },
                new double[] { -2.07961545532639, -0.222572481459575, -2.19351934244045 },
                new double[] { -2.08258829036651, -2.20295650174297, -0.222376682274817 }
            };

            double[][] g = new double[][] {
                new double[] { 0.790003018026837, 0.10530137197571, 0.104695609997453 },
                new double[] { 0.120523844040169, 0.771927460922736, 0.107548695037095 },
                new double[] { 0.120312473352932, 0.106668307692391, 0.773019218954677 }
            };

            double[][] h = new double[][] {
                new double[] { 0.833912858305498, 0.0832831201840334, 0.0828040215104686 },
                new double[] { 0.105447552576614, 0.800456986356854, 0.0940954610665313 },
                new double[] { 0.105685843190979, 0.0937004262821141, 0.800613730526906 }
            };

            double[][] j = new double[][] {
                new double[] { 0.833912858305498, 0.111154218508026, 0.110514787149899 },
                new double[] { 0.124978262684443, 0.800456986356854, 0.111523567529388 },
                new double[] { 0.124607274642233, 0.110476052414428, 0.800613730526906 }
            };

            {
                var teacher = new MultilabelSupportVectorLearning<Gaussian>()
                {
                    Learner = (param) => new SequentialMinimalOptimization<Gaussian>()
                    {
                        Complexity = 10000,
                    }
                };

                // Learn a machine
                var machine = teacher.Learn(inputs, outputs);

                var calibration = new MultilabelSupportVectorLearning<Gaussian>(machine)
                {
                    Learner = (param) => new ProbabilisticOutputCalibration<Gaussian>(param.Model)
                };

                machine = calibration.Learn(inputs, outputs);

                Assert.AreEqual(MultilabelProbabilityMethod.SumsToOne, machine.Method);
                compare(test, machine, new double[][][] { e, f, g });

                machine.Method = MultilabelProbabilityMethod.SumsToOneWithEmphasisOnWinner;
                compare(test, machine, new double[][][] { e, f, h });
            }

            {
                var teacher = new MultilabelSupportVectorLearning<Gaussian>()
                {
                    Learner = (param) => new SequentialMinimalOptimization<Gaussian>()
                    {
                        Complexity = 10000,
                    },

                    IsMultilabel = true
                };

                // Learn a machine
                var machine = teacher.Learn(inputs, outputs);

                var calibration = new MultilabelSupportVectorLearning<Gaussian>(machine)
                {
                    Learner = (param) => new ProbabilisticOutputCalibration<Gaussian>(param.Model)
                };

                machine = calibration.Learn(inputs, outputs);

                Assert.AreEqual(MultilabelProbabilityMethod.PerClass, machine.Method);
                compare(test, machine, new double[][][] { e, f, j });

                machine.Method = MultilabelProbabilityMethod.SumsToOneWithEmphasisOnWinner;
                compare(test, machine, new double[][][] { e, f, h });
            }
        }

        [Test]
        public void multiclass_probabilities()
        {
            Accord.Math.Random.Generator.Seed = 1;

            double[][] inputs =
            {
                new double[] { 10.1, 0.1, 0.01 },
                new double[] { 10.1, 0.2, 0.01 },
                new double[] { 10.2, 0.3, 0.02 },
                new double[] { 10.1, 0.1, 0.01 },

                new double[] { 0.1, 10.1, 0.01 },
                new double[] { 0.2, 10.2, 0.01 },
                new double[] { 0.1, 10.1, 0.01 },

                new double[] { 0.01, 0.01, 10.1 },
                new double[] { 0.01, 0.01, 10.1 },
                new double[] { 0.01, 0.01, 10.2 },
            };

            int[] outputs = { 0, 0, 0, 0, 1, 1, 1, 2, 2, 2 };

            double[][] test = new[] { inputs[0], inputs[4], inputs[8] };

            double[][] a = new double[][] {
                new double[] { 1.00990692633517, -1.00990692633517, -1.00595803613661 },
                new double[] { -1.00990692633517, 1.00990692633517, -0.00626027984045739 },
                new double[] { -1.00595803613661, -1.00741305990541, 1.00741305990541 }
            };

            double[][] b = new double[][] {
                new double[] { 2, 1, 0 },
                new double[] { 1, 2, 0 },
                new double[] { 1, 0, 2 }
            };

            double[][] c = new double[][] {
                new double[] { 0.789960988120308, 0.104812150960467, 0.105226860919225 },
                new double[] { 0.0887694693933983, 0.669048551282703, 0.242181979323899 },
                new double[] { 0.105406673002338, 0.105253415311559, 0.789339911686102 }
            };

            double[][] d = new double[][] {
                new double[] { 0.665240955774822, 0.244728471054798, 0.0900305731703805 },
                new double[] { 0.244728471054798, 0.665240955774822, 0.0900305731703805 },
                new double[] { 0.244728471054798, 0.0900305731703805, 0.665240955774822 }
            };


            {
                var teacher = new MulticlassSupportVectorLearning<Gaussian>()
                {
                    Learner = (param) => new SequentialMinimalOptimization<Gaussian>()
                    {
                        Complexity = 10000,
                    }
                };

                // Learn a machine
                var machine = teacher.Learn(inputs, outputs);

                Assert.AreEqual(MulticlassComputeMethod.Elimination, machine.Method);
                compare(test, machine, new double[][][] { a, a, c });

                machine.Method = MulticlassComputeMethod.Voting;
                compare(test, machine, new double[][][] { b, b, d });
            }

            {
                var teacher = new MulticlassSupportVectorLearning<Gaussian>()
                {
                    Learner = (param) => new SequentialMinimalOptimization<Gaussian>()
                    {
                        Complexity = 10000,
                    },
                };

                // Learn a machine
                var machine = teacher.Learn(inputs, outputs);

                Assert.AreEqual(MulticlassComputeMethod.Elimination, machine.Method);
                compare(test, machine, new double[][][] { a, a, c });

                machine.Method = MulticlassComputeMethod.Voting;
                compare(test, machine, new double[][][] { b, b, d });
            }



            double[][] e = new double[][] {
                new double[] { 1.61485251853836, -1.61485251853836, -1.60759667543465 },
                new double[] { -1.38916795967919, 1.38916795967919, -0.118723773337039 },
                new double[] { -1.38879628677469, -1.38855507690143, 1.38879628677469 }
            };

            double[][] f = new double[][] {
                new double[] { 0.926417880569892, 0.0366575852221866, 0.0369245342079215 },
                new double[] { 0.0484152373528162, 0.779108646139437, 0.172476116507747 },
                new double[] { 0.0553081942575627, 0.0553215367491952, 0.889370268993242 }
            };


            {
                var teacher = new MulticlassSupportVectorLearning<Gaussian>()
                {
                    Learner = (param) => new SequentialMinimalOptimization<Gaussian>()
                    {
                        Complexity = 10000,
                    }
                };

                // Learn a machine
                var machine = teacher.Learn(inputs, outputs);

                var calibration = new MulticlassSupportVectorLearning<Gaussian>(machine)
                {
                    Learner = (param) => new ProbabilisticOutputCalibration<Gaussian>(param.Model)
                };

                machine = calibration.Learn(inputs, outputs);

                Assert.AreEqual(MulticlassComputeMethod.Elimination, machine.Method);
                compare(test, machine, new double[][][] { e, e, f });

                machine.Method = MulticlassComputeMethod.Voting;
                compare(test, machine, new double[][][] { b, b, d });
            }

            {
                var teacher = new MulticlassSupportVectorLearning<Gaussian>()
                {
                    Learner = (param) => new SequentialMinimalOptimization<Gaussian>()
                    {
                        Complexity = 10000,
                    },
                };

                // Learn a machine
                var machine = teacher.Learn(inputs, outputs);

                var calibration = new MulticlassSupportVectorLearning<Gaussian>(machine)
                {
                    Learner = (param) => new ProbabilisticOutputCalibration<Gaussian>(param.Model)
                };

                machine = calibration.Learn(inputs, outputs);

                Assert.AreEqual(MulticlassComputeMethod.Elimination, machine.Method);
                compare(test, machine, new double[][][] { e, e, f });

                machine.Method = MulticlassComputeMethod.Voting;
                compare(test, machine, new double[][][] { b, b, d });
            }
        }

        private static void compare(double[][] test, IMultilabelLikelihoodClassifier<double[]> machine, double[][][] expected)
        {
            var scores = machine.Scores(test);
            var logLikelihoods = machine.LogLikelihoods(test);
            var probabilities = machine.Probabilities(test);

            string a = scores.ToCSharp();
            string b = logLikelihoods.ToCSharp();
            string c = probabilities.ToCSharp();

            Assert.IsTrue(expected[0].IsEqual(scores, 1e-5));
            Assert.IsTrue(expected[1].IsEqual(logLikelihoods, 1e-5));
            Assert.IsTrue(expected[2].IsEqual(probabilities, 1e-5));
        }

        [Test]
        public void no_samples_for_class()
        {
            double[][] inputs =
            {
                new double[] { 1, 1 }, // 0
                new double[] { 1, 1 }, // 0
                new double[] { 1, 1 }, // 2
            };

            int[] outputs =
            {
                0, 0, 2
            };

            var teacher = new MulticlassSupportVectorLearning<Gaussian>()
            {
                Learner = (param) => new SequentialMinimalOptimization<Gaussian>()
                {
                    UseKernelEstimation = true
                }
            };

            Assert.Throws<ArgumentException>(() => teacher.Learn(inputs, outputs),
                "There are no samples for class label {0}. Please make sure that class " +
                "labels are contiguous and there is at least one training sample for each label.", 1);
        }


        //[Test]
        //public void gh1047()
        //{
        //    // https://github.com/accord-net/framework/issues/1047#issuecomment-347990636

        //    double[][] x =
        //    {
        //        new double[] { 0.843946446600023, 0.590640737266467, 1.72137487731282, 1.57051220248599, 1.25787722046271},
        //        new double[] { 1.81795538387932, 1.1752993916574, 1.29604299866202, 1.14395278624226, 0.83041877565398},
        //        new double[] { 1.1867488818406, 1.41648938821298, 1.51987516068968, 1.30632678757385, 0.937799655212449},
        //        new double[] { 0.128888856980189, 1.27483840502442, 1.4553025510829, 1.29415393321166, 0.918761675903281},
        //        new double[] { 0.751579994633229, 1.72183855559824, 1.96500963073231, 0.969601765667682, 0.674641485676571} 
        //    };

        //    int[] y =
        //    {
        //        4,
        //        0,
        //        1,
        //        2,
        //        3
        //    };

        //    var teacher = new MulticlassSupportVectorLearning<Linear>()
        //    {
        //        // using LIBLINEAR's L2-loss SVC dual for each SVM
        //        Learner = (p) => new LinearDualCoordinateDescent()
        //        {
        //            Loss = Loss.L2
        //        }
        //    };

        //    teacher.Learn(x, y);
        //}
    }
}
