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
    using Accord.DataSets;
    using Accord.IO;
    using Accord.MachineLearning;
    using Accord.MachineLearning.VectorMachines;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Math;
    using Accord.Math.Optimization.Losses;
    using Accord.Statistics;
    using Accord.Statistics.Analysis;
    using Accord.Statistics.Kernels;
    using NUnit.Framework;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    [TestFixture]
    public class MultilabelSupportVectorLearningTest
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
                new double[] { 0, 0 },
                new double[] { 0, 1 },
                new double[] { 1, 0 },
                new double[] { 1, 1 },
            };

            // Outputs for each of the inputs
            int[][] outputs =
            { 
                //       and   or   nand   xor
                new[] {  -1,  -1,    +1,   +1 },
                new[] {  -1,  +1,    +1,   -1 },
                new[] {  -1,  +1,    +1,   -1 },
                new[] {  +1,  +1,    -1,   +1 },
            };

            // Create a new Linear kernel
            IKernel linear = new Linear();

            // Create a new Multi-class Support Vector Machine for one input,
            //  using the linear kernel and four disjoint classes.
            var machine = new MultilabelSupportVectorMachine(inputs: 2, kernel: linear, classes: 4);

            // Create the Multi-class learning algorithm for the machine
            var teacher = new MultilabelSupportVectorLearning(machine, inputs, outputs);

            // Configure the learning algorithm to use SMO to train the
            //  underlying SVMs in each of the binary class subproblems.
            teacher.Algorithm = (svm, classInputs, classOutputs, i, j) =>
                new SequentialMinimalOptimization(svm, classInputs, classOutputs)
                {
                    // Create a hard SVM
                    Complexity = 10000.0
                };

            // Run the learning algorithm
            double error = teacher.Run();

            // only xor is not learnable by
            // a hard-margin linear machine

            bool[][] pred = machine.Decide(inputs);
            bool[][] train = Classes.Decide(outputs);

            var and = pred.GetColumn(0);
            Assert.IsTrue(and.IsEqual(train.GetColumn(0)));

            var or = pred.GetColumn(1);
            Assert.IsTrue(or.IsEqual(train.GetColumn(1)));

            var nand = pred.GetColumn(2);
            Assert.IsTrue(nand.IsEqual(train.GetColumn(2)));

            var xor = pred.GetColumn(3);
            Assert.IsFalse(xor.IsEqual(train.GetColumn(3)));


            Assert.AreEqual(2 / 16.0, error);
        }


        [Test]
        public void ComputeTest1()
        {
            double[][] inputs =
            {
                new double[] { 1, 4, 2, 0, 1 },
                new double[] { 1, 3, 2, 0, 1 },
                new double[] { 3, 0, 1, 1, 1 },
                new double[] { 3, 0, 1, 0, 1 },
                new double[] { 0, 5, 5, 5, 5 },
                new double[] { 1, 5, 5, 5, 5 },
                new double[] { 1, 0, 0, 0, 0 },
                new double[] { 1, 0, 0, 0, 0 },
            };

            int[] outputs =
            {
                0, 0,
                1, 1,
                2, 2,
                3, 3,
            };


            IKernel kernel = new Polynomial(2);
            var msvm = new MultilabelSupportVectorMachine(5, kernel, 4);
            var smo = new MultilabelSupportVectorLearning(msvm, inputs, outputs);
            smo.Algorithm = (svm, classInputs, classOutputs, i, j) =>
                new SequentialMinimalOptimization(svm, classInputs, classOutputs)
                {
                    Complexity = 1
                };

            Assert.AreEqual(0, msvm.GetLastKernelEvaluations());

#if DEBUG
            smo.ParallelOptions.MaxDegreeOfParallelism = 1;

#endif
            msvm.ParallelOptions.MaxDegreeOfParallelism = 1;

            double error = smo.Run();
            Assert.AreEqual(0, error);


            int[] evals = new int[inputs.Length];
            int[] hits = new int[inputs.Length];
            for (int i = 0; i < inputs.Length; i++)
            {
                double expected = outputs[i];
                double[] responses; msvm.Compute(inputs[i], out responses);
                int actual; responses.Max(out actual);
                Assert.AreEqual(expected, actual);
                evals[i] = msvm.GetLastKernelEvaluations();
                hits[i] = msvm.GetLastKernelHits();
            }

            for (int i = 0; i < evals.Length; i++)
                Assert.AreEqual(msvm.SupportVectorUniqueCount, evals[i]);
        }

        [Test]
        public void LinearComputeTest1()
        {
            double[][] inputs =
            {
                new double[] { 1, 4, 2, 0, 1 },
                new double[] { 1, 3, 2, 0, 1 },
                new double[] { 3, 0, 1, 1, 1 },
                new double[] { 3, 0, 1, 0, 1 },
                new double[] { 0, 5, 5, 5, 5 },
                new double[] { 1, 5, 5, 5, 5 },
                new double[] { 1, 0, 0, 0, 0 },
                new double[] { 1, 0, 0, 0, 0 },
            };

            int[] outputs =
            {
                0, 0,
                1, 1,
                2, 2,
                3, 3,
            };


            var msvm = new MultilabelSupportVectorMachine(5, 4);
            var smo = new MultilabelSupportVectorLearning(msvm, inputs, outputs);
            smo.Algorithm = (svm, classInputs, classOutputs, i, j) =>
                new LinearNewtonMethod(svm, classInputs, classOutputs)
                {
                    Complexity = 1
                };

            Assert.AreEqual(0, msvm.GetLastKernelEvaluations());

#if DEBUG
            smo.ParallelOptions.MaxDegreeOfParallelism = 1;
            msvm.ParallelOptions.MaxDegreeOfParallelism = 1;
#endif

            double error = smo.Run();
            Assert.AreEqual(0.125, error);


            int[] evals = new int[inputs.Length];
            int[] y = new int[inputs.Length];
            for (int i = 0; i < inputs.Length; i++)
            {
                double expected = outputs[i];
                double[] responses;
                msvm.Compute(inputs[i], out responses);
                int actual;
                responses.Max(out actual);
                y[i] = actual;
                if (i < 6)
                {
                    Assert.AreEqual(expected, actual);
                    evals[i] = msvm.GetLastKernelEvaluations();
                }
                else
                {
                    Assert.AreNotEqual(expected, actual);
                    evals[i] = msvm.GetLastKernelEvaluations();
                }
            }

            for (int i = 0; i < evals.Length; i++)
                Assert.AreEqual(0, evals[i]);

            for (int i = 0; i < inputs.Length; i++)
            {
                int actual;
                msvm.Scores(inputs[i], out actual);
                Assert.AreEqual(y[i], actual);
            }
        }

#if !NO_BINARY_SERIALIZATION
        [Test]
        [Category("Serialization")]
        public void SerializeTest1()
        {
            double[][] inputs =
            {
                new double[] { 1, 4, 2, 0, 1 },
                new double[] { 1, 3, 2, 0, 1 },
                new double[] { 3, 0, 1, 1, 1 },
                new double[] { 3, 0, 1, 0, 1 },
                new double[] { 0, 5, 5, 5, 5 },
                new double[] { 1, 5, 5, 5, 5 },
                new double[] { 1, 0, 0, 0, 0 },
                new double[] { 1, 0, 0, 0, 0 },
            };

            int[] outputs =
            {
                0, 0,
                1, 1,
                2, 2,
                3, 3,
            };

            IKernel kernel = new Linear();
            var msvm = new MultilabelSupportVectorMachine(5, kernel, 4);
            var smo = new MultilabelSupportVectorLearning(msvm, inputs, outputs);
            smo.Algorithm = (svm, classInputs, classOutputs, i, j) =>
                new SequentialMinimalOptimization(svm, classInputs, classOutputs)
                {
                    Complexity = 1
                };

            double error = smo.Run();
            Assert.AreEqual(0, error);

            int count = 0; // Compute errors
            for (int i = 0; i < inputs.Length; i++)
            {
                double[] responses;
                msvm.Compute(inputs[i], out responses);
                int y; responses.Max(out y);
                if (y != outputs[i]) count++;
            }

            double expected = (double)count / inputs.Length;

            Assert.AreEqual(msvm.Inputs, 5);
            Assert.AreEqual(msvm.Classes, 4);
            Assert.AreEqual(4, msvm.Machines.Length);


            MemoryStream stream = new MemoryStream();

            // Save the machines
            msvm.Save(stream);

            // Rewind
            stream.Seek(0, SeekOrigin.Begin);

            // Reload the machines
            var target = MultilabelSupportVectorMachine.Load(stream);

            double actual;

            count = 0; // Compute errors
            for (int i = 0; i < inputs.Length; i++)
            {
                double[] responses;
                target.Compute(inputs[i], out responses);
                int y; responses.Max(out y);
                if (y != outputs[i]) count++;
            }

            actual = (double)count / inputs.Length;


            Assert.AreEqual(expected, actual);

            Assert.AreEqual(msvm.Inputs, target.Inputs);
            Assert.AreEqual(msvm.Classes, target.Classes);
            for (int i = 0; i < msvm.Machines.Length; i++)
            {
                var a = msvm[i];
                var b = target[i];

                Assert.IsTrue(a.SupportVectors.IsEqual(b.SupportVectors));
            }
        }

        [Test]
        [Category("Serialization")]
        public void SerializeTest2()
        {
            double[][] inputs =
            {
                new double[] { 1, 4, 2, 0, 1 },
                new double[] { 1, 3, 2, 0, 1 },
                new double[] { 3, 0, 1, 1, 1 },
                new double[] { 3, 0, 1, 0, 1 },
                new double[] { 0, 5, 5, 5, 5 },
                new double[] { 1, 5, 5, 5, 5 },
                new double[] { 1, 0, 0, 0, 0 },
                new double[] { 1, 0, 0, 0, 0 },
            };

            int[] outputs =
            {
                0, 0,
                1, 1,
                2, 2,
                3, 3,
            };

            // Reload the machines
            string fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "ml-svm.bin");
            var target = Serializer.Load<MultilabelSupportVectorMachine>(fileName);

            double actual;

            int count = 0; // Compute errors
            for (int i = 0; i < inputs.Length; i++)
            {
                double[] responses;
                target.Compute(inputs[i], out responses);
                int y; responses.Max(out y);
                if (y != outputs[i]) count++;
            }

            actual = (double)count / inputs.Length;


            Assert.AreEqual(0, actual);

            Assert.AreEqual(5, target.Inputs);
            Assert.AreEqual(4, target.Classes);
            double[] thresholds = target.Machines.Apply(x => x.Threshold);
            double[][][] svs = target.Machines.Apply(x => x.SupportVectors);
            double[][] weights = target.Machines.Apply(x => x.Weights);

            string str = weights.ToString(CSharpJaggedMatrixFormatProvider.InvariantCulture);
            var expectedThresholds = new double[] { -1.02283345107303, -1.84854331146833, -1.54770710059171, 1.58508471778734 };
            var expectedSVs = new double[][][]
            {
                new double[][]
                {
                    new double[] { 1, 5, 5, 5, 5 },
                    new double[] { 1, 3, 2, 0, 1 },
                    new double[] { 3, 0, 1, 0, 1 },
                    new double[] { 0, 5, 5, 5, 5 },
                    new double[] { 1, 0, 0, 0, 0 },
                    new double[] { 1, 0, 0, 0, 0 }
                },
                new double[][]
                {
                    new double[] { 3, 0, 1, 1, 1 },
                    new double[] { 1, 0, 0, 0, 0 },
                    new double[] { 3, 0, 1, 0, 1 },
                    new double[] { 1, 0, 0, 0, 0 },
                    new double[] { 1, 5, 5, 5, 5 }
                },
                new double[][]
                {
                    new double[] { 1, 5, 5, 5, 5 },
                    new double[] { 1, 4, 2, 0, 1 },
                    new double[] { 0, 5, 5, 5, 5 }
                },
                new double[][]
                {
                    new double[] { 1, 0, 0, 0, 0 },
                    new double[] { 3, 0, 1, 1, 1 },
                    new double[] { 1, 3, 2, 0, 1 },
                    new double[] { 3, 0, 1, 0, 1 }
                }
             };

            double[][] expectedWeights = new double[][] {
                new double[] { -0.0234960254651262, 0.39850600072147, -0.0378635563194698, -0.0921056339066905, -0.15657076610795, -0.0884700189222341 },
                new double[] { 0.274739982272114, -0.161382051864791, 0.152130797418313, -0.209064650742987, -0.0564240770826484 },
                new double[] { 0.0384615384615383, -0.0392011834319528, 0.000739644970414358 },
                new double[] { 0.373020681418016, -0.00340179246301696, -0.079374696169365, -0.290244192785634 }
            };

            //Matrix.IsEqual(
            Assert.IsTrue(thresholds.IsEqual(expectedThresholds, rtol: 1e-8));
            Assert.IsTrue(svs.IsEqual(expectedSVs, rtol: 1e-8));
            Assert.IsTrue(weights.IsEqual(expectedWeights, rtol: 1e-8));
        }

        [Test]
        [Category("Serialization")]
        public void serialize_reload_new_version()
        {
            double[][] inputs =
            {
                new double[] { 1, 4, 2, 0, 1 },
                new double[] { 1, 3, 2, 0, 1 },
                new double[] { 3, 0, 1, 1, 1 },
                new double[] { 3, 0, 1, 0, 1 },
                new double[] { 0, 5, 5, 5, 5 },
                new double[] { 1, 5, 5, 5, 5 },
                new double[] { 1, 0, 0, 0, 0 },
                new double[] { 1, 0, 0, 0, 0 },
            };

            int[] outputs =
            {
                0, 0,
                1, 1,
                2, 2,
                3, 3,
            };

            IKernel kernel = new Linear();
            var msvm = new MultilabelSupportVectorMachine(5, kernel, 4);
            var smo = new MultilabelSupportVectorLearning(msvm, inputs, outputs);
            smo.Algorithm = (svm, classInputs, classOutputs, i, j) =>
                new SequentialMinimalOptimization(svm, classInputs, classOutputs)
                {
                    Complexity = 1
                };

            double expected = smo.Run();


            // Save the machines

            var bytes = msvm.Save();

            // Reload the machines
            var target = Serializer.Load<MultilabelSupportVectorMachine>(bytes);

            double actual;

            int count = 0; // Compute errors
            for (int i = 0; i < inputs.Length; i++)
            {
                double[] responses;
                target.Compute(inputs[i], out responses);
                int y; responses.Max(out y);
                if (y != outputs[i]) count++;
            }

            actual = (double)count / inputs.Length;


            Assert.AreEqual(expected, actual);

            Assert.AreEqual(msvm.Inputs, target.Inputs);
            Assert.AreEqual(msvm.Classes, target.Classes);
            for (int i = 0; i < msvm.Machines.Length; i++)
            {
                var a = msvm[i];
                var b = target[i];

                Assert.AreEqual(a.Threshold, b.Threshold);
                Assert.AreEqual(a.NumberOfInputs, b.NumberOfInputs);
                Assert.AreEqual(a.NumberOfOutputs, b.NumberOfOutputs);
                Assert.IsTrue(a.Weights.IsEqual(b.Weights));

                Assert.IsTrue(a.SupportVectors.IsEqual(b.SupportVectors));
            }
        }
#endif

        [Test]
        public void multilabel_linear_new_usage()
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
            var teacher = new MultilabelSupportVectorLearning<Linear>()
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
            bool[][] predicted = machine.Decide(inputs);

            // Compute classification error using mean accuracy (mAcc)
            double error = new HammingLoss(outputs).Loss(predicted);
            #endregion

            Assert.AreEqual(0, error);
            Assert.IsTrue(predicted.ArgMax(dimension: 1).IsEqual(outputs));
        }


        [Test]
        public void multilabel_linear_mnist()
        {
            string pathToTemporaryDir = Path.Combine(NUnit.Framework.TestContext.CurrentContext.WorkDirectory, "mnist");

            #region doc_learn_mnist
            // Download the MNIST dataset to a temporary dir:
            var mnist = new MNIST(path: pathToTemporaryDir);

            // Get the training inputs and expected outputs:
            Sparse<double>[] xTrain = mnist.Training.Item1;
            int[] yTrain = mnist.Training.Item2.ToInt32();

            // Create a one-vs-one multi-class SVM learning algorithm 
            var teacher = new MultilabelSupportVectorLearning<Linear, Sparse<double>>()
            {
                // using LIBLINEAR's L2-loss SVC dual for each SVM
                Learner = (p) => new LinearDualCoordinateDescent<Linear, Sparse<double>>()
                {
                    Loss = Loss.L2
                }
            };

            // Learn a linear machine on the training set
            var machine = teacher.Learn(xTrain, yTrain);

            // Compute classification error using mean accuracy (mAcc) for the training set:
            double trainError = GeneralConfusionMatrix.Estimate(machine, xTrain, yTrain).Error; // 0.084

            // Get the testing inputs and expected outputs
            Sparse<double>[] xTest = mnist.Testing.Item1;
            int[] yTest = mnist.Testing.Item2.ToInt32();

            // Compute classification error using mean accuracy (mAcc) for the testing set:
            double testError = GeneralConfusionMatrix.Estimate(machine, xTest, yTest).Error; // 0.0849
            #endregion

            Assert.AreEqual(60000, xTrain.Length);
            Assert.AreEqual(60000, yTrain.Length);
            Assert.AreEqual(10000, xTest.Length);
            Assert.AreEqual(10000, yTest.Length);
            Assert.AreEqual(0, machine.NumberOfInputs);
            Assert.AreEqual(10, machine.NumberOfOutputs);
            Assert.AreEqual(10, machine.NumberOfClasses);
            Assert.AreEqual(0.084016666666666628, trainError, 1e-4);
            Assert.AreEqual(0.084899999999999975, testError, 1e-3);
        }

        [Test]
        public void multilabel_gaussian_new_usage()
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
        public void multilabel_calibration()
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
            var teacher = new MultilabelSupportVectorLearning<Gaussian>()
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
            var calibration = new MultilabelSupportVectorLearning<Gaussian>()
            {
                Model = machine, // We will start with an existing machine

                // Configure the learning algorithm to use SMO to train the
                //  underlying SVMs in each of the binary class subproblems.
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
            bool[][] predicted = machine.Decide(inputs);

            // Get class scores for each sample
            double[][] scores = machine.Scores(inputs);

            // Get log-likelihoods (should be same as scores)
            double[][] logl = machine.LogLikelihoods(inputs);

            // Get probability for each sample
            double[][] prob = machine.Probabilities(inputs);

            // Compute classification error using mean accuracy (mAcc)
            double error = new HammingLoss(outputs).Loss(predicted);
            double loss = new CategoryCrossEntropyLoss(outputs).Loss(prob);
            #endregion

            string a = scores.ToCSharp();
            string b = logl.ToCSharp();
            string c = prob.ToCSharp();

            double[][] expectedScores =
            {
                new double[] { 1.85316017783605, -2.59688389729331, -2.32170102153988 },
                new double[] { 1.84933597524124, -1.99399145231446, -2.2920299547693 },
                new double[] { 1.44477953581274, -1.98592298465108, -2.27356092239125 },
                new double[] { 1.85316017783605, -2.59688389729331, -2.32170102153988 },
                new double[] { 1.84933597524124, -1.99399145231446, -2.2920299547693 },
                new double[] { -2.40815576360914, 0.328362962196791, -0.932721757919691 },
                new double[] { -2.13111157264226, 1.809192096031, -2.2920299547693 },
                new double[] { -2.13111157264226, 1.809192096031, -2.2920299547693 },
                new double[] { -2.14888646926108, -1.99399145231447, 1.33101148524982 },
                new double[] { -2.12915064678299, -1.98592298465108, 1.3242171079396 },
                new double[] { -1.47197826667149, -1.96368715704762, 0.843414180834243 },
                new double[] { -2.14221021749314, -2.83117892529093, 2.61354519154994 }
            };

            double[][] expectedLogL =
            {
                new double[] { -0.145606614365135, -2.66874434442222, -2.41528841111469 },
                new double[] { -0.146125659911391, -2.12163759796483, -2.3883043096263 },
                new double[] { -0.211716960454159, -2.11453945718522, -2.37154474995633 },
                new double[] { -0.145606614365135, -2.66874434442222, -2.41528841111469 },
                new double[] { -0.146125659911391, -2.12163759796483, -2.3883043096263 },
                new double[] { -2.4943161092787, -0.542383360363463, -1.26452689970624 },
                new double[] { -2.24328358118314, -0.151678833375872, -2.3883043096263 },
                new double[] { -2.24328358118314, -0.151678833375872, -2.3883043096263 },
                new double[] { -2.25918730624753, -2.12163759796483, -0.234447327588685 },
                new double[] { -2.24153091066541, -2.11453945718522, -0.2358711195715 },
                new double[] { -1.67856232802554, -2.0950136294762, -0.357841632335707 },
                new double[] { -2.25321037906455, -2.88845047104229, -0.0707140798850236 }
            };

            double[][] expectedProbs =
            {
                new double[] { 0.844913862516144, 0.0677684640174953, 0.0873176734663607 },
                new double[] { 0.803266328757473, 0.111405242674824, 0.0853284285677024 },
                new double[] { 0.790831391595502, 0.117950175028754, 0.0912184333757438 },
                new double[] { 0.844913862516144, 0.0677684640174953, 0.0873176734663607 },
                new double[] { 0.803266328757473, 0.111405242674824, 0.0853284285677024 },
                new double[] { 0.0872387667998771, 0.614360294206236, 0.298400938993887 },
                new double[] { 0.100372339295793, 0.812805149315815, 0.0868225113883914 },
                new double[] { 0.100372339295793, 0.812805149315815, 0.0868225113883914 },
                new double[] { 0.102863726210119, 0.11803188195247, 0.779104391837411 },
                new double[] { 0.104532503226998, 0.118686968710368, 0.776780528062634 },
                new double[] { 0.184996665350572, 0.121983586443407, 0.693019748206021 },
                new double[] { 0.0961702585148881, 0.0509517983210315, 0.85287794316408 }
            };

            int[] actual = predicted.ArgMax(dimension: 1);
            Assert.IsTrue(actual.IsEqual(outputs));
            Assert.AreEqual(0, error);
            Assert.AreEqual(3, machine.Count);
            Assert.AreEqual(0.5, machine[0].Kernel.Gamma);
            Assert.AreEqual(0.5, machine[1].Kernel.Gamma);
            Assert.AreEqual(0.5, machine[2].Kernel.Gamma);
            Assert.AreEqual(2.9395943260892361, loss, 1e-8);
            Assert.IsTrue(expectedScores.IsEqual(scores, 1e-10));
            Assert.IsTrue(expectedLogL.IsEqual(logl, 1e-10));
            Assert.IsTrue(expectedProbs.IsEqual(prob, 1e-10));
            double[] rowSums = expectedProbs.Sum(1);
            Assert.IsTrue(rowSums.IsEqual(Vector.Ones(expectedProbs.Length), 1e-10));

            {
                bool[][] predicted2 = null;
                double[][] scores2 = machine.Scores(inputs, ref predicted2);
                Assert.IsTrue(scores2.IsEqual(scores));
                Assert.IsTrue(predicted2.IsEqual(predicted));

                double[][] logl2 = machine.LogLikelihoods(inputs, ref predicted2);
                Assert.IsTrue(logl2.IsEqual(logl));
                Assert.IsTrue(predicted2.IsEqual(predicted));

                double[][] prob2 = machine.Probabilities(inputs, ref predicted2);
                Assert.IsTrue(prob2.IsEqual(prob));
                Assert.IsTrue(predicted2.IsEqual(predicted));

                bool[][] predicted3 = new bool[predicted2.Length][];
                double[][] scores3 = inputs.ApplyWithIndex((x, i) => machine.Scores(x, ref predicted3[i]));
                Assert.IsTrue(scores3.IsEqual(scores));
                Assert.IsTrue(predicted3.IsEqual(predicted));

                double[][] logl3 = inputs.ApplyWithIndex((x, i) => machine.LogLikelihoods(x, ref predicted3[i]));
                Assert.IsTrue(logl3.IsEqual(logl));
                Assert.IsTrue(predicted3.IsEqual(predicted));

                double[][] prob3 = inputs.ApplyWithIndex((x, i) => machine.Probabilities(x, ref predicted3[i]));
                Assert.IsTrue(prob3.IsEqual(prob));
                Assert.IsTrue(predicted3.IsEqual(predicted));
            }

            {
                double[] ed = new double[scores.Length];
                double[] es = new double[scores.Length];
                double[] el = new double[scores.Length];
                double[] ep = new double[scores.Length];
                for (int i = 0; i < expectedScores.Length; i++)
                {
                    int j = scores[i].ArgMax();
                    ed[i] = j;
                    es[i] = scores[i][j];
                    el[i] = logl[i][j];
                    ep[i] = prob[i][j];
                }

                int[] predicted2 = null;
                double[] scores2 = machine.ToMulticlass().Score(inputs, ref predicted2);
                Assert.IsTrue(scores2.IsEqual(es));
                Assert.IsTrue(predicted2.IsEqual(ed));

                double[] logl2 = machine.ToMulticlass().LogLikelihood(inputs, ref predicted2);
                Assert.IsTrue(logl2.IsEqual(el));
                Assert.IsTrue(predicted2.IsEqual(ed));

                double[] prob2 = machine.ToMulticlass().Probability(inputs, ref predicted2);
                Assert.IsTrue(prob2.IsEqual(ep));
                Assert.IsTrue(predicted2.IsEqual(ed));

                int[] predicted3 = new int[predicted2.Length];
                double[] scores3 = inputs.ApplyWithIndex((x, i) => machine.ToMulticlass().Score(x, out predicted3[i]));
                Assert.IsTrue(scores3.IsEqual(es));
                Assert.IsTrue(predicted3.IsEqual(ed));

                double[] logl3 = inputs.ApplyWithIndex((x, i) => machine.ToMulticlass().LogLikelihood(x, out predicted3[i]));
                Assert.IsTrue(logl3.IsEqual(el));
                Assert.IsTrue(predicted3.IsEqual(ed));

                double[] prob3 = inputs.ApplyWithIndex((x, i) => machine.ToMulticlass().Probability(x, out predicted3[i]));
                Assert.IsTrue(prob3.IsEqual(ep));
                Assert.IsTrue(predicted3.IsEqual(ed));
            }
        }

        [Test]
        public void multilabel_calibration_generic_kernel()
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
            var teacher = new MultilabelSupportVectorLearning<IKernel>()
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
            var calibration = new MultilabelSupportVectorLearning<IKernel>(machine)
            {
                // Configure the learning algorithm to use SMO to train the
                //  underlying SVMs in each of the binary class subproblems.
                Learner = (p) => new ProbabilisticOutputCalibration<IKernel>(p.Model)
            };


            // Configure parallel execution options
            calibration.ParallelOptions.MaxDegreeOfParallelism = 1;

            // Learn a machine
            calibration.Learn(inputs, outputs);

            // Obtain class predictions for each sample
            bool[][] predicted = machine.Decide(inputs);

            // Get class scores for each sample
            double[][] scores = machine.Scores(inputs);

            // Get log-likelihoods (should be same as scores)
            double[][] logl = machine.LogLikelihoods(inputs);

            // Get probability for each sample
            double[][] prob = machine.Probabilities(inputs);

            // Compute classification error using mean accuracy (mAcc)
            double error = new HammingLoss(outputs).Loss(predicted);
            double loss = new CategoryCrossEntropyLoss(outputs).Loss(prob);

            string a = scores.ToCSharp();
            string b = logl.ToCSharp();
            string c = prob.ToCSharp();

            double[][] expectedScores =
            {
                new double[] { 1.85316017783605, -2.59688389729331, -2.32170102153988 },
                new double[] { 1.84933597524124, -1.99399145231446, -2.2920299547693 },
                new double[] { 1.44477953581274, -1.98592298465108, -2.27356092239125 },
                new double[] { 1.85316017783605, -2.59688389729331, -2.32170102153988 },
                new double[] { 1.84933597524124, -1.99399145231446, -2.2920299547693 },
                new double[] { -2.40815576360914, 0.328362962196791, -0.932721757919691 },
                new double[] { -2.13111157264226, 1.809192096031, -2.2920299547693 },
                new double[] { -2.13111157264226, 1.809192096031, -2.2920299547693 },
                new double[] { -2.14888646926108, -1.99399145231447, 1.33101148524982 },
                new double[] { -2.12915064678299, -1.98592298465108, 1.3242171079396 },
                new double[] { -1.47197826667149, -1.96368715704762, 0.843414180834243 },
                new double[] { -2.14221021749314, -2.83117892529093, 2.61354519154994 }
            };

            double[][] expectedLogL =
            {
                new double[] { -0.145606614365135, -2.66874434442222, -2.41528841111469 },
                new double[] { -0.146125659911391, -2.12163759796483, -2.3883043096263 },
                new double[] { -0.211716960454159, -2.11453945718522, -2.37154474995633 },
                new double[] { -0.145606614365135, -2.66874434442222, -2.41528841111469 },
                new double[] { -0.146125659911391, -2.12163759796483, -2.3883043096263 },
                new double[] { -2.4943161092787, -0.542383360363463, -1.26452689970624 },
                new double[] { -2.24328358118314, -0.151678833375872, -2.3883043096263 },
                new double[] { -2.24328358118314, -0.151678833375872, -2.3883043096263 },
                new double[] { -2.25918730624753, -2.12163759796483, -0.234447327588685 },
                new double[] { -2.24153091066541, -2.11453945718522, -0.2358711195715 },
                new double[] { -1.67856232802554, -2.0950136294762, -0.357841632335707 },
                new double[] { -2.25321037906455, -2.88845047104229, -0.0707140798850236 }
            };

            double[][] expectedProbs =
            {
                new double[] { 0.844913862516144, 0.0677684640174953, 0.0873176734663607 },
                new double[] { 0.803266328757473, 0.111405242674824, 0.0853284285677024 },
                new double[] { 0.790831391595502, 0.117950175028754, 0.0912184333757438 },
                new double[] { 0.844913862516144, 0.0677684640174953, 0.0873176734663607 },
                new double[] { 0.803266328757473, 0.111405242674824, 0.0853284285677024 },
                new double[] { 0.0872387667998771, 0.614360294206236, 0.298400938993887 },
                new double[] { 0.100372339295793, 0.812805149315815, 0.0868225113883914 },
                new double[] { 0.100372339295793, 0.812805149315815, 0.0868225113883914 },
                new double[] { 0.102863726210119, 0.11803188195247, 0.779104391837411 },
                new double[] { 0.104532503226998, 0.118686968710368, 0.776780528062634 },
                new double[] { 0.184996665350572, 0.121983586443407, 0.693019748206021 },
                new double[] { 0.0961702585148881, 0.0509517983210315, 0.85287794316408 }
            };

            int[] actual = predicted.ArgMax(dimension: 1);
            Assert.IsTrue(actual.IsEqual(outputs));

            // Must be exactly the same as test above
            Assert.AreEqual(0, error);
            Assert.AreEqual(0.5, ((Gaussian)machine[0].Kernel).Gamma);
            Assert.AreEqual(0.5, ((Gaussian)machine[1].Kernel).Gamma);
            Assert.AreEqual(0.5, ((Gaussian)machine[2].Kernel).Gamma);
            Assert.AreEqual(2.9395943260892361, loss, 1e-8);
            Assert.IsTrue(expectedScores.IsEqual(scores, 1e-10));
            Assert.IsTrue(expectedLogL.IsEqual(logl, 1e-10));
            Assert.IsTrue(expectedProbs.IsEqual(prob, 1e-10));

            double[] probabilities = CorrectProbabilities(machine, inputs[0]);
            double[] actualProb = machine.Probabilities(inputs[0]);
            Assert.IsTrue(probabilities.IsEqual(actualProb, 1e-8));
        }

        public static double[] CorrectProbabilities(IMultilabelLikelihoodClassifier<double[]> machine, double[] input)
        {
            return Special.Softmax(machine.Scores(input).Apply(x => -Special.Log1pexp(-x)));
        }

        [Test]
        public void multilabel_linear_smo_new_usage()
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
            var teacher = new MultilabelSupportVectorLearning<Linear>();
            teacher.Learner = (p) => new SequentialMinimalOptimization<Linear>()
            {
                UseComplexityHeuristic = true
            };

#if DEBUG
            teacher.ParallelOptions.MaxDegreeOfParallelism = 1;
#endif

            // Learn a machine
            var machine = teacher.Learn(inputs, outputs);

            int[] actual = machine.Decide(inputs).ArgMax(dimension: 1);
            outputs[13] = 0;
            Assert.IsTrue(actual.IsEqual(outputs));
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

            var teacher = new MultilabelSupportVectorLearning<Gaussian>()
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
    }
}
