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
    using Accord.Math;
    using Accord.Statistics.Analysis;
    using Accord.Statistics.Kernels;
    using NUnit.Framework;
    using Accord;
    using Accord.Statistics.Kernels.Sparse;
    using Accord.Statistics;
    using System.IO;

    [TestFixture]
    public class SequentialMinimalOptimizationTest
    {

        [Test]
        public void LearnTest()
        {

            double[][] inputs =
            {
                new double[] { -1, -1 },
                new double[] { -1,  1 },
                new double[] {  1, -1 },
                new double[] {  1,  1 }
            };

            int[] xor =
            {
                -1,
                 1,
                 1,
                -1
            };

            // Create Kernel Support Vector Machine with a Polynomial Kernel of 2nd degree
            KernelSupportVectorMachine machine = new KernelSupportVectorMachine(new Polynomial(2), inputs[0].Length);

            // Create the sequential minimal optimization teacher
            SequentialMinimalOptimization learn = new SequentialMinimalOptimization(machine, inputs, xor);

            // Run the learning algorithm
            learn.Run();


            int[] output = inputs.Apply(p => Math.Sign(machine.Compute(p)));

            for (int i = 0; i < output.Length; i++)
                Assert.AreEqual(System.Math.Sign(xor[i]), System.Math.Sign(output[i]));
        }

        [Test]
        public void learn_linear()
        {
            #region doc_xor_linear
            // As an example, we will try to learn a linear machine  that can 
            // replicate the "exclusive-or" logical function. However, since we
            // will be using a linear SVM, we will not be able to solve this 
            // problem perfectly as the XOR is a non-linear classification problem:
            double[][] inputs =
            {
                new double[] { 0, 0 }, // the XOR function takes two booleans
                new double[] { 0, 1 }, // and computes their exclusive or: the
                new double[] { 1, 0 }, // output is true only if the two booleans
                new double[] { 1, 1 }  // are different
            };

            int[] xor = // this is the output of the xor function
            {
                0, // 0 xor 0 = 0 (inputs are equal)
                1, // 0 xor 1 = 1 (inputs are different)
                1, // 1 xor 0 = 1 (inputs are different)
                0, // 1 xor 1 = 0 (inputs are equal)
            };

            // Now, we can create the sequential minimal optimization teacher
            var learn = new SequentialMinimalOptimization()
            {
                UseComplexityHeuristic = true,
                UseKernelEstimation = false
            };

            // And then we can obtain a trained SVM by calling its Learn method
            SupportVectorMachine svm = learn.Learn(inputs, xor);

            // Finally, we can obtain the decisions predicted by the machine:
            bool[] prediction = svm.Decide(inputs);
            #endregion

            Assert.AreEqual(prediction[0], false);
            Assert.AreEqual(prediction[1], false);
            Assert.AreEqual(prediction[2], false);
            Assert.AreEqual(prediction[3], false);


            int[] or = // this is the output of the xor function
            {
                0, // 0 or 0 = 0 (inputs are equal)
                1, // 0 or 1 = 1 (inputs are different)
                1, // 1 or 0 = 1 (inputs are different)
                1, // 1 or 1 = 1 (inputs are equal)
            };


            learn = new SequentialMinimalOptimization()
            {
                Complexity = 1e+8,
                UseKernelEstimation = false
            };

            svm = learn.Learn(inputs, or);

            prediction = svm.Decide(inputs);

            Assert.AreEqual(prediction[0], false);
            Assert.AreEqual(prediction[1], true);
            Assert.AreEqual(prediction[2], true);
            Assert.AreEqual(prediction[3], true);
        }

        [Test]
        public void learn_new_method()
        {
            #region doc_xor_normal
            // As an example, we will try to learn a decision machine 
            // that can replicate the "exclusive-or" logical function:

            double[][] inputs =
            {
                new double[] { 0, 0 }, // the XOR function takes two booleans
                new double[] { 0, 1 }, // and computes their exclusive or: the
                new double[] { 1, 0 }, // output is true only if the two booleans
                new double[] { 1, 1 }  // are different
            };

            int[] xor = // this is the output of the xor function
            {
                0, // 0 xor 0 = 0 (inputs are equal)
                1, // 0 xor 1 = 1 (inputs are different)
                1, // 1 xor 0 = 1 (inputs are different)
                0, // 1 xor 1 = 0 (inputs are equal)
            };

            // Now, we can create the sequential minimal optimization teacher
            var learn = new SequentialMinimalOptimization<Gaussian>()
            {
                UseComplexityHeuristic = true,
                UseKernelEstimation = true
            };

            // And then we can obtain a trained SVM by calling its Learn method
            SupportVectorMachine<Gaussian> svm = learn.Learn(inputs, xor);

            // Finally, we can obtain the decisions predicted by the machine:
            bool[] prediction = svm.Decide(inputs);
            #endregion

            Assert.AreEqual(prediction, Classes.Decide(xor));
        }

        [Test]
        public void learn_precomputed()
        {
            #region doc_precomputed
            // As an example, we will try to learn a decision machine 
            // that can replicate the "exclusive-or" logical function:

            double[][] inputs =
            {
                new double[] { 0, 0 }, // the XOR function takes two booleans
                new double[] { 0, 1 }, // and computes their exclusive or: the
                new double[] { 1, 0 }, // output is true only if the two booleans
                new double[] { 1, 1 }  // are different
            };

            int[] xor = // this is the output of the xor function
            {
                0, // 0 xor 0 = 0 (inputs are equal)
                1, // 0 xor 1 = 1 (inputs are different)
                1, // 1 xor 0 = 1 (inputs are different)
                0, // 1 xor 1 = 0 (inputs are equal)
            };

            // Let's use a Gaussian kernel
            var kernel = new Gaussian(0.1);

            // Create a pre-computed Gaussian kernel matrix
            var precomputed = new Precomputed(kernel.ToJagged(inputs));

            // Now, we can create the sequential minimal optimization teacher
            var learn = new SequentialMinimalOptimization<Precomputed, int>()
            {
                Kernel = precomputed // set the precomputed kernel we created
            };

            // And then we can obtain the SVM by using Learn
            var svm = learn.Learn(precomputed.Indices, xor);

            // Finally, we can obtain the decisions predicted by the machine:
            bool[] prediction = svm.Decide(precomputed.Indices);

            // We can also compute the machine prediction to new samples
            double[][] sample =
            {
                new double[] { 0, 1 }
            };

            // Update the precomputed kernel with the new samples
            precomputed = new Precomputed(kernel.ToJagged2(inputs, sample));

            // Update the SVM kernel
            svm.Kernel = precomputed;

            // Compute the predictions to the new samples
            bool[] newPrediction = svm.Decide(precomputed.Indices);
            #endregion

            Assert.AreEqual(prediction, Classes.Decide(xor));
            Assert.AreEqual(newPrediction.Length, 1);
            Assert.AreEqual(newPrediction[0], true);
        }

#if !NETSTANDARD2_0
        [Test]
#if DEBUG
        [Ignore("Intensive")]
#endif
        public void learn_gaussian_sparse_kernel2()
        {
            string tmp = Path.Combine(TestContext.CurrentContext.WorkDirectory, "tmp");
            var news20 = new Accord.DataSets.News20(tmp);
            Sparse<double>[] inputs = news20.Training.Item1.Get(0, 2000);
            int[] outputs = news20.Training.Item2.ToMulticlass().Get(0, 2000);

            var learn = new MultilabelSupportVectorLearning<Gaussian, Sparse<double>>()
            {
                // using LIBLINEAR's SVC dual for each SVM
                Learner = (p) => new SequentialMinimalOptimization<Gaussian, Sparse<double>>()
                {
                    Strategy = SelectionStrategy.SecondOrder,
                    Complexity = 1.0,
                    Tolerance = 1e-4,
                    CacheSize = 1999,
                    UseKernelEstimation = true
                },
            };

            var svm = learn.Learn(inputs, outputs);
            Assert.AreEqual(20, svm.NumberOfClasses);

            int[] predicted = svm.ToMulticlass().Decide(inputs);

            var test = new GeneralConfusionMatrix(outputs, predicted);
            Assert.AreEqual(20, test.Classes);

            Assert.AreEqual(0.991, test.Accuracy, 1e-5);
        }

        [Test]
        [Category("Slow")]
        public void learn_linear_sparse_kernel()
        {
            Accord.Math.Random.Generator.Seed = 0;

            string tmp = Path.Combine(TestContext.CurrentContext.WorkDirectory, "tmp");
            var news20 = new Accord.DataSets.News20(tmp);

            Sparse<double>[] inputs = news20.Training.Item1.Get(0, 2000);
            int[] outputs = news20.Training.Item2.ToMulticlass().Get(0, 2000);

            var learn = new MultilabelSupportVectorLearning<Linear, Sparse<double>>()
            {
                // using LIBLINEAR's SVC dual for each SVM
                Learner = (p) => new LinearDualCoordinateDescent<Linear, Sparse<double>>()
                {
                },
            };

            var svm = learn.Learn(inputs, outputs);

            svm.Compress();

            Assert.AreEqual(20, svm.NumberOfClasses);

            int[] predicted = svm.ToMulticlass().Decide(inputs);

            var test = new GeneralConfusionMatrix(outputs, predicted);
            Assert.AreEqual(20, test.Classes);

            Assert.AreEqual(0.9715, test.Accuracy, 1e-5);
        }
#endif

        [Test]
        public void learn_sparse_kernel()
        {
            #region doc_xor_sparse
            // As an example, we will try to learn a decision machine 
            // that can replicate the "exclusive-or" logical function:

            Sparse<double>[] inputs =
            {
                Sparse.FromDense(new double[] { 0, 0 }), // the XOR function takes two booleans
                Sparse.FromDense(new double[] { 0, 1 }), // and computes their exclusive or: the
                Sparse.FromDense(new double[] { 1, 0 }), // output is true only if the two booleans
                Sparse.FromDense(new double[] { 1, 1 })  // are different
            };

            int[] xor = // this is the output of the xor function
            {
                0, // 0 xor 0 = 0 (inputs are equal)
                1, // 0 xor 1 = 1 (inputs are different)
                1, // 1 xor 0 = 1 (inputs are different)
                0, // 1 xor 1 = 0 (inputs are equal)
            };

            // Now, we can create the sequential minimal optimization teacher
            var learn = new SequentialMinimalOptimization<Gaussian, Sparse<double>>()
            {
                UseComplexityHeuristic = true,
                UseKernelEstimation = true
            };

            // And then we can obtain a trained SVM by calling its Learn method
            var svm = learn.Learn(inputs, xor);

            // Finally, we can obtain the decisions predicted by the machine:
            bool[] prediction = svm.Decide(inputs);
            #endregion

            Assert.AreEqual(prediction, Classes.Decide(xor));
        }

        [Test]
        public void LearnTest2()
        {

            double[][] inputs =
            {
                new double[] { -1, -1 },
                new double[] { -1,  1 },
                new double[] {  1, -1 },
                new double[] {  1,  1 }
            };

            int[] or =
            {
                -1,
                -1,
                -1,
                +1
            };

            // Create Kernel Support Vector Machine with a Polynomial Kernel of 2nd degree
            SupportVectorMachine machine = new SupportVectorMachine(inputs[0].Length);

            // Create the sequential minimal optimization teacher
            SequentialMinimalOptimization learn = new SequentialMinimalOptimization(machine, inputs, or);
            learn.Complexity = 1;

            // Run the learning algorithm
            learn.Run();


            int[] output = inputs.Apply(p => (int)machine.Compute(p));

            for (int i = 0; i < output.Length; i++)
            {
                bool sor = or[i] >= 0;
                bool sou = output[i] >= 0;
                Assert.AreEqual(sor, sou);
            }
        }

        [Test]
        public void LearnTest3()
        {
            double[][] inputs =
            {
                new double[] { -1, -1 },
                new double[] { -1,  1 },
                new double[] {  1, -1 },
                new double[] {  1,  1 }
            };

            int[] xor =
            {
                -1,
                 1,
                 1,
                -1
            };

            // Create Kernel Support Vector Machine with a Polynomial Kernel of 2nd degree
            KernelSupportVectorMachine machine = new KernelSupportVectorMachine(new Polynomial(2), inputs[0].Length);

            // Create the sequential minimal optimization teacher
            SequentialMinimalOptimization learn = new SequentialMinimalOptimization(machine, inputs, xor);

            // Run the learning algorithm
            learn.Run();


            int[] output = inputs.Apply(p => Math.Sign(machine.Compute(p)));

            for (int i = 0; i < output.Length; i++)
                Assert.AreEqual(System.Math.Sign(xor[i]), System.Math.Sign(output[i]));
        }

        [Test]
        public void linear_without_threshold_doesnt_solve_xor()
        {
            double[][] inputs =
            {
                new double[] { -1, -1 },
                new double[] { -1,  1 },
                new double[] {  1, -1 },
                new double[] {  1,  1 }
            };

            int[] xor =
            {
                -1,
                 1,
                 1,
                -1
            };

            // Create the sequential minimal optimization teacher
            var learn = new SequentialMinimalOptimization()
            {
                Complexity = 1e-5
            };

            // Run the learning algorithm
            SupportVectorMachine machine = learn.Learn(inputs, xor);

            bool[] output = machine.Decide(inputs);

            for (int i = 0; i < output.Length; i++)
                Assert.AreEqual(false, output[i]);
        }

        [Test]
        public void LearnTest4()
        {

            double[][] inputs =
            {
                new double[] { -1, -1 },
                new double[] { -1,  1 },
                new double[] {  1, -1 },
                new double[] {  1,  1 }
            };

            int[] negatives =
            {
                -1,
                -1,
                -1,
                -1
            };

            // Create Kernel Support Vector Machine with a Polynomial Kernel of 2nd degree
            SupportVectorMachine machine = new SupportVectorMachine(inputs[0].Length);

            // Create the sequential minimal optimization teacher
            SequentialMinimalOptimization learn = new SequentialMinimalOptimization(machine, inputs, negatives);
            learn.Complexity = 1;

            // Run the learning algorithm
            double error = learn.Run();

            Assert.AreEqual(0, error);


            int[] output = inputs.Apply(p => (int)machine.Compute(p));

            for (int i = 0; i < output.Length; i++)
            {
                bool sor = negatives[i] >= 0;
                bool sou = output[i] >= 0;
                Assert.AreEqual(sor, sou);
            }


        }

        [Test]
        public void LearnTest5()
        {

            double[][] inputs =
            {
                new double[] { -1, -1 },
                new double[] { -1,  1 },
                new double[] {  1, -1 },
                new double[] {  1,  1 }
            };

            int[] positives =
            {
                1,
                1,
                1,
                1
            };

            // Create Kernel Support Vector Machine with a Polynomial Kernel of 2nd degree
            SupportVectorMachine machine = new SupportVectorMachine(inputs[0].Length);

            // Create the sequential minimal optimization teacher
            SequentialMinimalOptimization learn = new SequentialMinimalOptimization(machine, inputs, positives);
            learn.Complexity = 1;

            // Run the learning algorithm
            double error = learn.Run();

            Assert.AreEqual(0, error);


            int[] output = inputs.Apply(p => (int)machine.Compute(p));

            for (int i = 0; i < output.Length; i++)
            {
                bool sor = positives[i] >= 0;
                bool sou = output[i] >= 0;
                Assert.AreEqual(sor, sou);
            }
        }

        [Test]
        public void Learn_UnspecifiedCacheSize_CacheSizeEqualsInputLength()
        {
            double[][] inputs =
            {
                new double[] { -1, -1 },
                new double[] { -1,  1 },
                new double[] {  1, -1 },
                new double[] {  1,  1 }
            };

            int[] xor =
            {
                -1,
                 1,
                 1,
                -1
            };

            KernelSupportVectorMachine svm = new KernelSupportVectorMachine(new Polynomial(2), inputs[0].Length);
            SequentialMinimalOptimization smo = new SequentialMinimalOptimization(svm, inputs, xor);

            smo.Run();

            Assert.AreEqual(smo.CacheSize, inputs.Length);
        }

        [Test]
        public void Learn_CacheSizeZero_CacheSizeShouldBeZero()
        {
            double[][] inputs =
            {
                new double[] { -1, -1 },
                new double[] { -1,  1 },
                new double[] {  1, -1 },
                new double[] {  1,  1 }
            };

            int[] xor =
            {
                -1,
                 1,
                 1,
                -1
            };

            KernelSupportVectorMachine svm = new KernelSupportVectorMachine(new Polynomial(2), inputs[0].Length);
            SequentialMinimalOptimization smo = new SequentialMinimalOptimization(svm, inputs, xor)
            {
                CacheSize = 0
            };

            smo.Run();

            Assert.AreEqual(smo.CacheSize, 0);
        }

        [Test]
        [Category("Slow")]
        public void LargeLearningTest1()
        {
            // Create large input vectors

            int rows = 1000;
            int dimension = 10000;

            double[][] inputs = new double[rows][];
            int[] outputs = new int[rows];

            Random rnd = new Random();

            for (int i = 0; i < inputs.Length; i++)
            {
                inputs[i] = new double[dimension];

                if (i > rows / 2)
                {
                    for (int j = 0; j < dimension; j++)
                        inputs[i][j] = rnd.NextDouble();
                    outputs[i] = -1;
                }
                else
                {
                    for (int j = 0; j < dimension; j++)
                        inputs[i][j] = rnd.NextDouble() * 4.21 + 5;
                    outputs[i] = +1;
                }
            }

            KernelSupportVectorMachine svm = new KernelSupportVectorMachine(new Polynomial(2), dimension);

            SequentialMinimalOptimization smo = new SequentialMinimalOptimization(svm, inputs, outputs)
            {
                UseComplexityHeuristic = true
            };


            double error = smo.Run();

            Assert.AreEqual(0, error);
        }

        [Test]
        public void SequentialMinimalOptimizationConstructorTest()
        {
            double[][] inputs =
            {
                new double[] { -1, -1 },
                new double[] { -1,  1 },
                new double[] {  1, -1 },
                new double[] {  1,  1 }
            };

            int[] or =
            {
                0,
                0,
                0,
                +1
            };

            // Create Kernel Support Vector Machine with a Polynomial Kernel of 2nd degree
            SupportVectorMachine machine = new SupportVectorMachine(inputs[0].Length);

            var learn = new SequentialMinimalOptimization(machine, inputs, or);
            learn.Run();

            for (int i = 0; i < inputs.Length; i++)
            {
                bool actual = machine.Decide(inputs[i]);
                Assert.AreEqual(or[i] > 0, actual);
            }
        }


        [Test]
        public void weight_test_inhomogeneous_linear_kernel()
        {
            var yinyang = GetYingYang();
            var dataset = yinyang;
            double[][] inputs = dataset.Submatrix(null, 0, 1).ToJagged();
            int[] labels = dataset.GetColumn(2).ToInt32();

            Accord.Math.Tools.SetupGenerator(0);

            var kernel = new Linear(1);
            Assert.AreEqual(kernel.Constant, 1);

            {
                var machine = new KernelSupportVectorMachine(kernel, inputs[0].Length);
                var smo = new SequentialMinimalOptimization(machine, inputs, labels);

                smo.Complexity = 1.0;
                smo.PositiveWeight = 1;
                smo.NegativeWeight = 1;
                smo.Tolerance = 0.001;

                double error = smo.Run();

                int[] actual = new int[labels.Length];
                for (int i = 0; i < actual.Length; i++)
                    actual[i] = Math.Sign(machine.Compute(inputs[i]));

                ConfusionMatrix matrix = new ConfusionMatrix(actual, labels);

                Assert.AreEqual(43, matrix.TruePositives); // both classes are
                Assert.AreEqual(43, matrix.TrueNegatives); // well equilibrated
                Assert.AreEqual(7, matrix.FalseNegatives);
                Assert.AreEqual(7, matrix.FalsePositives);

                Assert.AreEqual(1.0, smo.Complexity);
                Assert.AreEqual(1.0, smo.WeightRatio);
                Assert.AreEqual(1.0, smo.NegativeWeight);
                Assert.AreEqual(1.0, smo.PositiveWeight);
                Assert.AreEqual(0.14, error);
                Assert.AreEqual(0.001, smo.Tolerance);
                Assert.AreEqual(31, machine.SupportVectors.Length);

                machine.Compress();
                Assert.AreEqual(1, machine.Weights[0]);
                Assert.AreEqual(1, machine.SupportVectors.Length);
                Assert.AreEqual(-1.3107402300323954, machine.SupportVectors[0][0], 1e-3);
                Assert.AreEqual(-0.5779471529948812, machine.SupportVectors[0][1], 1e-3);
                Assert.AreEqual(-1.5338510320418068, machine.Threshold, 1e-3);
                for (int i = 0; i < actual.Length; i++)
                {
                    int expected = actual[i];
                    int y = Math.Sign(machine.Compute(inputs[i]));
                    Assert.AreEqual(expected, y);
                }
            }

            {
                var machine = new KernelSupportVectorMachine(kernel, inputs[0].Length);
                var smo = new SequentialMinimalOptimization(machine, inputs, labels);

                smo.Complexity = 1;
                smo.PositiveWeight = 100;
                smo.NegativeWeight = 1;
                smo.Tolerance = 0.001;

                double error = smo.Run();

                int[] actual = new int[labels.Length];
                for (int i = 0; i < actual.Length; i++)
                    actual[i] = Math.Sign(machine.Compute(inputs[i]));

                ConfusionMatrix matrix = new ConfusionMatrix(actual, labels);

                Assert.AreEqual(50, matrix.TruePositives); // has more importance
                Assert.AreEqual(23, matrix.TrueNegatives);
                Assert.AreEqual(0, matrix.FalseNegatives); // has more importance
                Assert.AreEqual(27, matrix.FalsePositives);

                Assert.AreEqual(1.0, smo.Complexity);
                Assert.AreEqual(100, smo.WeightRatio);
                Assert.AreEqual(1.0, smo.NegativeWeight);
                Assert.AreEqual(100, smo.PositiveWeight);
                Assert.AreEqual(0.001, smo.Tolerance);
                Assert.AreEqual(0.27, error);
                Assert.AreEqual(41, machine.SupportVectors.Length);
            }

            {
                var machine = new KernelSupportVectorMachine(kernel, inputs[0].Length);
                var smo = new SequentialMinimalOptimization(machine, inputs, labels);

                smo.Complexity = 1;
                smo.PositiveWeight = 1;
                smo.NegativeWeight = 100;
                smo.Tolerance = 0.001;

                double error = smo.Run();

                int[] actual = new int[labels.Length];
                for (int i = 0; i < actual.Length; i++)
                    actual[i] = Math.Sign(machine.Compute(inputs[i]));

                var matrix = new ConfusionMatrix(actual, labels);

                Assert.AreEqual(25, matrix.TruePositives);
                Assert.AreEqual(50, matrix.TrueNegatives); // has more importance
                Assert.AreEqual(25, matrix.FalseNegatives);
                Assert.AreEqual(0, matrix.FalsePositives);  // has more importance

                Assert.AreEqual(1.0, smo.Complexity);
                Assert.AreEqual(0.01, smo.WeightRatio);
                Assert.AreEqual(100, smo.NegativeWeight);
                Assert.AreEqual(1.0, smo.PositiveWeight);
                Assert.AreEqual(0.25, error);
                Assert.AreEqual(0.001, smo.Tolerance);
                Assert.AreEqual(40, machine.SupportVectors.Length);
            }
        }

        [Test]
        public void weight_test_homogeneous_linear_kernel()
        {
            var yinyang = GetYingYang();
            var dataset = yinyang;
            double[][] inputs = dataset.Submatrix(null, 0, 1).ToJagged();
            int[] labels = dataset.GetColumn(2).ToInt32();

            Accord.Math.Tools.SetupGenerator(0);

            var kernel = new Linear();
            Assert.AreEqual(kernel.Constant, 0);

            {
                var machine = new KernelSupportVectorMachine(kernel, inputs[0].Length);
                var smo = new SequentialMinimalOptimization(machine, inputs, labels);

                smo.Complexity = 1.0;
                smo.PositiveWeight = 1;
                smo.NegativeWeight = 1;
                smo.Tolerance = 0.001;

                double error = smo.Run();

                int[] actual = new int[labels.Length];
                for (int i = 0; i < actual.Length; i++)
                    actual[i] = machine.Decide(inputs[i]) ? 1 : 0;

                ConfusionMatrix matrix = new ConfusionMatrix(actual, labels);

                Assert.AreEqual(43, matrix.TruePositives); // both classes are
                Assert.AreEqual(43, matrix.TrueNegatives); // well equilibrated
                Assert.AreEqual(7, matrix.FalseNegatives);
                Assert.AreEqual(7, matrix.FalsePositives);

                Assert.AreEqual(1.0, smo.Complexity);
                Assert.AreEqual(1.0, smo.WeightRatio);
                Assert.AreEqual(1.0, smo.NegativeWeight);
                Assert.AreEqual(1.0, smo.PositiveWeight);
                Assert.AreEqual(0.14, error);
                Assert.AreEqual(0.001, smo.Tolerance);
                Assert.AreEqual(31, machine.SupportVectors.Length);

                machine.Compress();
                Assert.AreEqual(1, machine.Weights[0]);
                Assert.AreEqual(1, machine.SupportVectors.Length);
                Assert.AreEqual(-1.3107402300323954, machine.SupportVectors[0][0]);
                Assert.AreEqual(-0.5779471529948812, machine.SupportVectors[0][1]);
                Assert.AreEqual(-0.53366022455811646, machine.Threshold);
                for (int i = 0; i < actual.Length; i++)
                {
                    int expected = actual[i];
                    int y = machine.Decide(inputs[i]) ? 1 : 0;
                    Assert.AreEqual(expected, y);
                }
            }

            {
                var machine = new KernelSupportVectorMachine(kernel, inputs[0].Length);
                var smo = new SequentialMinimalOptimization(machine, inputs, labels);

                smo.Complexity = 1;
                smo.PositiveWeight = 100;
                smo.NegativeWeight = 1;
                smo.Tolerance = 0.001;

                double error = smo.Run();

                int[] actual = new int[labels.Length];
                for (int i = 0; i < actual.Length; i++)
                    actual[i] = machine.Decide(inputs[i]) ? 1 : 0;

                ConfusionMatrix matrix = new ConfusionMatrix(actual, labels);

                Assert.AreEqual(50, matrix.TruePositives); // has more importance
                Assert.AreEqual(23, matrix.TrueNegatives);
                Assert.AreEqual(0, matrix.FalseNegatives); // has more importance
                Assert.AreEqual(27, matrix.FalsePositives);

                Assert.AreEqual(1.0, smo.Complexity);
                Assert.AreEqual(100, smo.WeightRatio);
                Assert.AreEqual(1.0, smo.NegativeWeight);
                Assert.AreEqual(100, smo.PositiveWeight);
                Assert.AreEqual(0.001, smo.Tolerance);
                Assert.AreEqual(0.27, error);
                Assert.AreEqual(42, machine.SupportVectors.Length);
            }

            {
                var machine = new KernelSupportVectorMachine(kernel, inputs[0].Length);
                var smo = new SequentialMinimalOptimization(machine, inputs, labels);

                smo.Complexity = 1;
                smo.PositiveWeight = 1;
                smo.NegativeWeight = 100;
                smo.Tolerance = 0.001;

                double error = smo.Run();

                int[] actual = new int[labels.Length];
                for (int i = 0; i < actual.Length; i++)
                    actual[i] = machine.Decide(inputs[i]) ? 1 : 0;

                var matrix = new ConfusionMatrix(actual, labels);

                Assert.AreEqual(25, matrix.TruePositives);
                Assert.AreEqual(50, matrix.TrueNegatives); // has more importance
                Assert.AreEqual(25, matrix.FalseNegatives);
                Assert.AreEqual(0, matrix.FalsePositives);  // has more importance

                Assert.AreEqual(1.0, smo.Complexity);
                Assert.AreEqual(0.01, smo.WeightRatio);
                Assert.AreEqual(100, smo.NegativeWeight);
                Assert.AreEqual(1.0, smo.PositiveWeight);
                Assert.AreEqual(0.25, error);
                Assert.AreEqual(0.001, smo.Tolerance);
                Assert.AreEqual(40, machine.SupportVectors.Length);
            }
        }


        [Test]
        public void WeightsTest2()
        {
            var yinyang = GetYingYang();
            var dataset = yinyang;

            double[][] inputs = dataset.Submatrix(null, 0, 1).ToJagged();
            int[] labels = dataset.GetColumn(2).ToInt32();

            testWeights(inputs, labels, new Linear(0));
            testWeights(inputs, labels, new Linear(1));
            testWeights(inputs, labels, new Polynomial(2, 0));
            testWeights(inputs, labels, new Polynomial(2, 1));
            testWeights(inputs, labels, new Gaussian(1.0));
        }

        private static void testWeights(double[][] inputs, int[] labels, IKernel kernel)
        {
            {
                var machine = new KernelSupportVectorMachine(kernel, inputs[0].Length);
                var smo = new SequentialMinimalOptimization(machine, inputs, labels);

                smo.PositiveWeight = 100;
                smo.NegativeWeight = 1;

                double error = smo.Run();

                int[] actual = new int[labels.Length];
                for (int i = 0; i < actual.Length; i++)
                    actual[i] = Math.Sign(machine.Compute(inputs[i]));

                ConfusionMatrix matrix = new ConfusionMatrix(actual, labels);

                Assert.AreEqual(50, matrix.TruePositives); // has more importance
                Assert.AreEqual(0, matrix.FalseNegatives); // has more importance
            }

            {
                var machine = new KernelSupportVectorMachine(kernel, inputs[0].Length);
                var smo = new SequentialMinimalOptimization(machine, inputs, labels);

                smo.PositiveWeight = 1;
                smo.NegativeWeight = 100;

                double error = smo.Run();

                int[] actual = new int[labels.Length];
                for (int i = 0; i < actual.Length; i++)
                    actual[i] = Math.Sign(machine.Compute(inputs[i]));

                var matrix = new ConfusionMatrix(actual, labels);
                Assert.AreEqual(50, matrix.TrueNegatives); // has more importance
                Assert.AreEqual(0, matrix.FalsePositives);  // has more importance
            }
        }

        [Test]
        public void CompactTest()
        {

            double[][] inputs =
            {
                new double[] { -1, -1 },
                new double[] { -1,  1 },
                new double[] {  1, -1 },
                new double[] {  1,  1 }
            };

            int[] xor =
            {
                -1,
                 1,
                 1,
                -1
            };

            {
                var machine = new KernelSupportVectorMachine(new Polynomial(2), inputs[0].Length);
                var learn = new SequentialMinimalOptimization(machine, inputs, xor);

                learn.Compact = false;
                double error = learn.Run();
                Assert.AreEqual(0, error);
            }

            {
                var machine = new KernelSupportVectorMachine(new Polynomial(2), inputs[0].Length);
                var learn = new SequentialMinimalOptimization(machine, inputs, xor);

                bool thrown = false;
                try { learn.Compact = true; }
                catch { thrown = true; }
                Assert.IsTrue(thrown);
            }

            {
                var machine = new KernelSupportVectorMachine(new Linear(), inputs[0].Length);
                var learn = new SequentialMinimalOptimization(machine, inputs, xor);

                learn.Compact = false;
                double error = learn.Run();
                Assert.AreEqual(0.5, error);
            }

            {
                var machine = new KernelSupportVectorMachine(new Linear(), inputs[0].Length);
                var learn = new SequentialMinimalOptimization(machine, inputs, xor);

                learn.Compact = false;
                double error = learn.Run();
                Assert.AreEqual(0.5, error);
            }
        }

        [Test]
        public void new_method_null_test()
        {
            double[][] inputs =
            {
                new double[] { -1, -1 },
                null,
                new double[] {  1, -1 },
                new double[] {  1,  1 }
            };

            int[] or = { 0, 0, 0, +1 };

            var smo = new SequentialMinimalOptimization<Gaussian>();
            Assert.Throws<ArgumentException>(() => smo.Learn(inputs, or));
        }

        [Test]
        public void ComplexityHeuristicTest()
        {
            var yinyang = GetYingYang();
            var dataset = yinyang;

            double[][] inputs = dataset.Submatrix(null, 0, 1).ToJagged();
            int[] labels = dataset.GetColumn(2).ToInt32();

            var linear = new SupportVectorMachine(inputs[0].Length);

            Linear kernel = new Linear(0);
            var machine = new KernelSupportVectorMachine(kernel, inputs[0].Length);

            var smo1 = new SequentialMinimalOptimization(machine, inputs, labels);
            smo1.UseClassProportions = true;
            smo1.UseComplexityHeuristic = true;
            double e1 = smo1.Run();

            var smo2 = new SequentialMinimalOptimization(linear, inputs, labels);
            smo2.UseClassProportions = true;
            smo2.UseComplexityHeuristic = true;
            double e2 = smo2.Run();

            Assert.AreEqual(smo1.Complexity, smo2.Complexity);
            Assert.AreEqual(e1, e2);
        }

        [Test]
        public void ComplexityHeuristicTest_new_method()
        {
            var yinyang = GetYingYang();
            var dataset = yinyang;

            double[][] inputs = dataset.Submatrix(null, 0, 1).ToJagged();
            int[] labels = dataset.GetColumn(2).ToInt32();

            var smo = new SequentialMinimalOptimization<Gaussian>()
            {
                UseClassProportions = true,
                UseComplexityHeuristic = true
            };

            var svm = smo.Learn(inputs, labels);

            Assert.AreEqual(1, smo.Complexity);
        }

        [Test]
        public void UseClassProportionsTest()
        {
            var dataset = KernelSupportVectorMachineTest.training;
            var inputs = dataset.Submatrix(null, 0, 3);
            var labels = Accord.Math.Tools.Scale(0, 1, -1, 1, dataset.GetColumn(4)).ToInt32();

            Gaussian kernel = Gaussian.Estimate(inputs);
            var machine = new KernelSupportVectorMachine(kernel, inputs[0].Length);
            var smo = new SequentialMinimalOptimization(machine, inputs, labels);

            smo.Complexity = 1.0;
            smo.UseClassProportions = true;

            double error = smo.Run();

            Assert.AreEqual(1, smo.Complexity);
            Assert.AreEqual(0.4, smo.PositiveWeight);
            Assert.AreEqual(1.0, smo.NegativeWeight);
            Assert.AreEqual(0.4, smo.WeightRatio, 1e-10);
            Assert.AreEqual(0.2857142857142857, error);
            Assert.AreEqual(265.78327637381551, ((Gaussian)machine.Kernel).Sigma);
            Assert.AreEqual(26, machine.SupportVectors.Length);


            int[] actual = new int[labels.Length];
            for (int i = 0; i < actual.Length; i++)
                actual[i] = Math.Sign(machine.Compute(inputs[i]));

            ConfusionMatrix matrix = new ConfusionMatrix(actual, labels);
            Assert.AreEqual(12, matrix.FalseNegatives);
            Assert.AreEqual(0, matrix.FalsePositives);
            Assert.AreEqual(0, matrix.TruePositives);
            Assert.AreEqual(30, matrix.TrueNegatives);
        }

        [Test]
        public void WeightRatioTest()
        {
            var dataset = KernelSupportVectorMachineTest.training;
            var inputs = dataset.Submatrix(null, 0, 3);
            var labels = Accord.Math.Tools.Scale(0, 1, -1, 1, dataset.GetColumn(4)).ToInt32();

            Gaussian kernel = Gaussian.Estimate(inputs);

            {
                var machine = new KernelSupportVectorMachine(kernel, inputs[0].Length);
                var smo = new SequentialMinimalOptimization(machine, inputs, labels);

                smo.Complexity = 1.0;
                smo.WeightRatio = 10;

                double error = smo.Run();

                Assert.AreEqual(1.0, smo.PositiveWeight);
                Assert.AreEqual(0.1, smo.NegativeWeight);
                Assert.AreEqual(0.7142857142857143, error);
                Assert.AreEqual(265.78327637381551, ((Gaussian)machine.Kernel).Sigma);
                Assert.AreEqual(39, machine.SupportVectors.Length);


                int[] actual = new int[labels.Length];
                for (int i = 0; i < actual.Length; i++)
                    actual[i] = Math.Sign(machine.Compute(inputs[i]));

                ConfusionMatrix matrix = new ConfusionMatrix(actual, labels);

                Assert.AreEqual(12, matrix.TruePositives); // has more importance
                Assert.AreEqual(0, matrix.FalseNegatives); // has more importance
                Assert.AreEqual(30, matrix.FalsePositives);
                Assert.AreEqual(0, matrix.TrueNegatives);

                Assert.AreEqual(1.0, matrix.Sensitivity);
                Assert.AreEqual(0.0, matrix.Specificity);

                Assert.AreEqual(0.44444444444444448, matrix.FScore);
                Assert.AreEqual(0.0, matrix.MatthewsCorrelationCoefficient);
            }

            {
                var machine = new KernelSupportVectorMachine(kernel, inputs[0].Length);
                var smo = new SequentialMinimalOptimization(machine, inputs, labels);

                smo.Complexity = 1.0;
                smo.WeightRatio = 0.1;

                double error = smo.Run();

                Assert.AreEqual(0.1, smo.PositiveWeight);
                Assert.AreEqual(1.0, smo.NegativeWeight);
                Assert.AreEqual(0.21428571428571427, error);
                Assert.AreEqual(265.78327637381551, ((Gaussian)machine.Kernel).Sigma);
                Assert.AreEqual(18, machine.SupportVectors.Length);


                int[] actual = new int[labels.Length];
                for (int i = 0; i < actual.Length; i++)
                    actual[i] = Math.Sign(machine.Compute(inputs[i]));

                ConfusionMatrix matrix = new ConfusionMatrix(actual, labels);

                Assert.AreEqual(8, matrix.FalseNegatives);
                Assert.AreEqual(1, matrix.FalsePositives); // has more importance
                Assert.AreEqual(4, matrix.TruePositives);
                Assert.AreEqual(29, matrix.TrueNegatives); // has more importance

                Assert.AreEqual(0.33333333333333331, matrix.Sensitivity);
                Assert.AreEqual(0.96666666666666667, matrix.Specificity);

                Assert.AreEqual(0.47058823529411764, matrix.FScore);
                Assert.AreEqual(0.41849149947774944, matrix.MatthewsCorrelationCoefficient);
            }
        }

        [Test]
        public void FixedWeightsTest()
        {
            var dataset = KernelSupportVectorMachineTest.training;
            var inputs = dataset.Submatrix(null, 0, 3);
            var labels = Accord.Math.Tools.Scale(0, 1, -1, 1, dataset.GetColumn(4)).ToInt32();

            var machine = new KernelSupportVectorMachine(
                Gaussian.Estimate(inputs), inputs[0].Length);

            var smo = new SequentialMinimalOptimization(machine, inputs, labels);

            smo.Complexity = 10;

            double error = smo.Run();

            Assert.AreEqual(0.19047619047619047, error);
            Assert.AreEqual(265.78327637381551, ((Gaussian)machine.Kernel).Sigma);
            Assert.AreEqual(29, machine.SupportVectors.Length);

            double[] expectedWeights =
            {
                1.65717694716503, 1.20005456611466, -5.70824245415995, 10,
                10, -2.38755497916487, 10, -8.15723436363058, 10, -10, 10,
                10, -0.188634936781317, -5.4354281009458, -8.48341139483265,
                -5.91105702760141, -5.71489190049223, 10, -2.37289205235858,
                -3.33031262413522, -1.97545116517677, 10, -10, -9.563186799279,
                -3.917941544845, -0.532584110773336, 4.81951847548326, 0.343668292727091,
                -4.34159482731336
            };

            //Assert.IsTrue(expectedWeights.IsEqual(machine.Weights, 1e-5));

            int[] actual = new int[labels.Length];
            for (int i = 0; i < actual.Length; i++)
                actual[i] = Math.Sign(machine.Compute(inputs[i]));

            ConfusionMatrix matrix = new ConfusionMatrix(actual, labels);

            Assert.AreEqual(8, matrix.FalseNegatives);
            Assert.AreEqual(0, matrix.FalsePositives);
            Assert.AreEqual(4, matrix.TruePositives);
            Assert.AreEqual(30, matrix.TrueNegatives);

            Assert.AreEqual(1 / 3.0, matrix.Sensitivity);
            Assert.AreEqual(1, matrix.Specificity);

            Assert.AreEqual(0.5, matrix.FScore);
            Assert.AreEqual(0.5129891760425771, matrix.MatthewsCorrelationCoefficient);
        }


        [Test]
        public void TransformTest()
        {
            var yinyang = GetYingYang();
            var inputs = yinyang.Submatrix(null, 0, 1).ToJagged();
            var labels = yinyang.GetColumn(2).ToInt32();

            ConfusionMatrix actual, expected;
            SequentialMinimalOptimization a, b;

            var kernel = new Polynomial(2, 0);

            {
                var machine = new KernelSupportVectorMachine(kernel, inputs[0].Length);
                a = new SequentialMinimalOptimization(machine, inputs, labels);
                a.UseComplexityHeuristic = true;
                a.Run();

                int[] values = new int[labels.Length];
                for (int i = 0; i < values.Length; i++)
                    values[i] = Math.Sign(machine.Compute(inputs[i]));

                expected = new ConfusionMatrix(values, labels);
            }

            {
                var projection = inputs.Apply(kernel.Transform);
                var machine = new SupportVectorMachine(projection[0].Length);
                b = new SequentialMinimalOptimization(machine, projection, labels);
                b.UseComplexityHeuristic = true;
                b.Run();

                int[] values = new int[labels.Length];
                for (int i = 0; i < values.Length; i++)
                    values[i] = Math.Sign(machine.Compute(projection[i]));

                actual = new ConfusionMatrix(values, labels);
            }

            Assert.AreEqual(a.Complexity, b.Complexity, 1e-15);
            Assert.AreEqual(expected.TrueNegatives, actual.TrueNegatives);
            Assert.AreEqual(expected.TruePositives, actual.TruePositives);
            Assert.AreEqual(expected.FalseNegatives, actual.FalseNegatives);
            Assert.AreEqual(expected.FalsePositives, actual.FalsePositives);
        }

        public static double[,] GetYingYang()
        {
            double[,] yinyang =
            {
#region Yin Yang
            { -0.876847428, 1.996318824, -1 },
            { -0.748759325, 1.997248514, -1 },
            { -0.635574695, 1.978046579, -1 },
            { -0.513769071, 1.973224777, -1 },
            { -0.382577547, 1.955077224, -1 },
            { -0.275144211, 1.923813789, -1 },
            { -0.156802752, 1.949219695, -1 },
            { -0.046002059, 1.895342542, -1 },
            { 0.084152257, 1.873104082, -1 },
            { 0.192063131, 1.868157532, -1 },
            { 0.238547032, 1.811664165, -1 },
            { 0.381412694, 1.830869925, -1 },
            { 0.431182119, 1.755312479, -1 },
            { 0.562589082, 1.725444806, -1 },
            { 0.553294269, 1.689047886, -1 },
            { 0.730976261, 1.610522064, -1 },
            { 0.722164981, 1.633112952, -1 },
            { 0.861069302, 1.562450197, -1 },
            { 0.825107945, 1.435846225, -1 },
            { 0.825261132, 1.456391196, -1 },
            { 0.948721626, 1.393367552, -1 },
            { 1.001705278, 1.275768447, -1 },
            { 0.966788667, 1.321375233, -1 },
            { 1.030828944, 1.228437023, -1 },
            { 1.083195636, 1.143011589, -1 },
            { 0.920876422, 1.037854388, -1 },
            { 0.994518277, 1.064971023, -1 },
            { 0.954169422, 0.938084211, -1 },
            { 0.903586083, 0.985255341, -1 },
            { 0.877869854, 0.729143525, -1 },
            { 0.866594018, 0.75025734, -1 },
            { 0.757278389, 0.638917822, -1 },
            { 0.655489515, 0.670717406, -1 },
            { 0.687639626, 0.511655563, -1 },
            { 0.656365078, 0.638542346, -1 },
            { 0.491775914, 0.401874802, -1 },
            { 0.35504489, 0.38963967, -1 },
            { 0.275616568, 0.182958126, -1 },
            { 0.338471037, 0.102347682, -1 },
            { 0.103918095, 0.152960961, -1 },
            { 0.238473941, -0.070899965, -1 },
            { -0.00657754, 0.168107931, -1 },
            { -0.091307058, -0.032174399, -1 },
            { -0.290772034, -0.345025689, -1 },
            { -0.287555253, -0.397984323, -1 },
            { -0.363424618, -0.365636808, -1 },
            { -0.544071691, -0.512970644, -1 },
            { -0.7098968, -0.54654921, -1 },
            { -1.007857216, -0.811837224, -1 },
            { -0.932787122, -0.687973276, -1 },
            { -0.123987649, -1.547976483, 1 },
            { -0.247236701, -1.546629461, 1 },
            { -0.369357682, -1.533968755, 1 },
            { -0.497892178, -1.525597952, 1 },
            { -0.606998699, -1.518386229, 1 },
            { -0.751556976, -1.46427032, 1 },
            { -0.858848619, -1.464142289, 1 },
            { -0.957834238, -1.454165888, 1 },
            { -1.061602698, -1.444783216, 1 },
            { -1.169634343, -1.426033507, 1 },
            { -1.272115895, -1.408678817, 1 },
            { -1.380383293, -1.345651442, 1 },
            { -1.480866574, -1.279955202, 1 },
            { -1.548927664, -1.223262541, 1 },
            { -1.597886819, -1.227115936, 1 },
            { -1.686711497, -1.141898276, 1 },
            { -1.812689051, -1.14805053, 1 },
            { -1.809841336, -1.083347602, 1 },
            { -1.938850711, -1.019723742, 1 },
            { -1.974552679, -0.970515422, 1 },
            { -1.953184359, -0.88363121, 1 },
            { -1.98749965, -0.861879772, 1 },
            { -2.04215554, -0.797813815, 1 },
            { -1.984185734, -0.826986835, 1 },
            { -2.063307605, -0.749495213, 1 },
            { -1.964274134, -0.653639779, 1 },
            { -2.020258155, -0.530431615, 1 },
            { -1.946081996, -0.514425683, 1 },
            { -1.934356006, -0.435380423, 1 },
            { -1.827017658, -0.425058004, 1 },
            { -1.788385889, -0.312443513, 1 },
            { -1.800874033, -0.237312969, 1 },
            { -1.784225126, 0.013987951, 1 },
            { -1.682828321, -0.063911465, 1 },
            { -1.754042471, -0.075520653, 1 },
            { -1.5680733, 0.110795036, 1 },
            { -1.438333268, 0.170230561, 1 },
            { -1.356614661, 0.163613841, 1 },
            { -1.336362397, 0.334537756, 1 },
            { -1.296677607, 0.316006907, 1 },
            { -1.109908857, 0.474036646, 1 },
            { -0.845929174, 0.485303884, 1 },
            { -0.855794711, 0.395603118, 1 },
            { -0.68479255, 0.671166245, 1 },
            { -0.514222252, 0.652065554, 1 },
            { -0.387612557, 0.700858902, 1 },
            { -0.51939719, 1.025735335, 1 },
            { -0.228760025, 0.93490314, 1 },
            { -0.293782477, 1.008861678, 1 },
            { 0.013431012, 1.082021525, 1 },
#endregion
            };
            return yinyang;
        }
    }
}
