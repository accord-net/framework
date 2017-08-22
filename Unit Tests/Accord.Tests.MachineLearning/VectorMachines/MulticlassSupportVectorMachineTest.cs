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
    using System.IO;
    using System.Threading.Tasks;
    using Accord.MachineLearning.VectorMachines;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Statistics;
    using Accord.Math;
    using Accord.Statistics.Kernels;
    using NUnit.Framework;
    using System;
    using Accord.Math.Optimization.Losses;
    using System.Diagnostics;

    [TestFixture]
    public class MulticlassSupportVectorMachineTest
    {

        [Test]
        public void MulticlassSupportVectorMachineConstructorTest()
        {
            int inputs = 1;
            IKernel kernel = new Linear();
            int classes = 4;
            var target = new MulticlassSupportVectorMachine(inputs, kernel, classes);

            Assert.AreEqual(3, target.Machines.Length);
            Assert.AreEqual(classes * (classes - 1) / 2, target.Machines[0].Length + target.Machines[1].Length + target.Machines[2].Length);

            for (int i = 0; i < classes; i++)
            {
                for (int j = 0; j < classes; j++)
                {
                    if (i == j)
                        continue;

                    var machine = target[i, j];
                    Assert.IsNotNull(machine);
                }
            }
        }

        [Test]
        public void MulticlassSupportVectorMachineConstructorTest2()
        {
            int inputs = 1;
            int classes = 3;

            IKernel kernel = new Linear();

            var target = new MulticlassSupportVectorMachine(inputs, kernel, classes);

            target[0, 1].Kernel = new Gaussian(0.1);
            target[0, 2].Kernel = new Linear();
            target[1, 2].Kernel = new Polynomial(2);

            Assert.AreEqual(target[0, 0], target[0, 0]);
            Assert.AreEqual(target[1, 1], target[1, 1]);
            Assert.AreEqual(target[2, 2], target[2, 2]);
            Assert.AreEqual(target[0, 1], target[1, 0]);
            Assert.AreEqual(target[0, 2], target[0, 2]);
            Assert.AreEqual(target[1, 2], target[1, 2]);

            Assert.AreNotEqual(target[0, 1], target[0, 2]);
            Assert.AreNotEqual(target[1, 2], target[0, 2]);
            Assert.AreNotEqual(target[1, 2], target[0, 1]);
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
            var msvm = new MulticlassSupportVectorMachine(5, kernel, 4);
            var smo = new MulticlassSupportVectorLearning(msvm, inputs, outputs);
            smo.Algorithm = (svm, classInputs, classOutputs, i, j) =>
                new SequentialMinimalOptimization(svm, classInputs, classOutputs)
                {
                    Complexity = 1
                };

            msvm.ParallelOptions.MaxDegreeOfParallelism = 1;
            smo.ParallelOptions.MaxDegreeOfParallelism = 1;

            Assert.AreEqual(0, msvm.GetLastKernelEvaluations());

            double error = smo.Run();

            // Assert.AreEqual(6, msvm.GetLastKernelEvaluations());

            int[] evals = new int[inputs.Length];
            int[] evalexp = { 8, 8, 7, 7, 7, 7, 6, 6 };
            for (int i = 0; i < inputs.Length; i++)
            {
                double expected = outputs[i];
                double actual = msvm.Compute(inputs[i], MulticlassComputeMethod.Elimination);
                Assert.AreEqual(expected, actual);
                evals[i] = msvm.GetLastKernelEvaluations();
            }

            for (int i = 0; i < evals.Length; i++)
                Assert.AreEqual(evals[i], evalexp[i]);

            for (int i = 0; i < inputs.Length; i++)
            {
                double expected = outputs[i];
                double actual = msvm.Compute(inputs[i], MulticlassComputeMethod.Voting);
                Assert.AreEqual(expected, actual);
                evals[i] = msvm.GetLastKernelEvaluations();
            }

            for (int i = 0; i < evals.Length; i++)
                Assert.AreEqual(msvm.SupportVectorUniqueCount, evals[i], 1);
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


            var msvm = new MulticlassSupportVectorMachine(5, 4);
            var smo = new MulticlassSupportVectorLearning(msvm, inputs, outputs);
            smo.Algorithm = (svm, classInputs, classOutputs, i, j) =>
                new LinearCoordinateDescent(svm, classInputs, classOutputs)
                {
                    Complexity = 1
                };

            msvm.ParallelOptions.MaxDegreeOfParallelism = 1;
            smo.ParallelOptions.MaxDegreeOfParallelism = 1;

            Assert.AreEqual(0, msvm.GetLastKernelEvaluations());

            double error = smo.Run();

            Assert.AreEqual(0, error);

            // Linear machines in compact form do not require kernel evaluations
            Assert.AreEqual(0, msvm.GetLastKernelEvaluations());

            for (int i = 0; i < inputs.Length; i++)
            {
                double expected = outputs[i];
                double actual = msvm.Compute(inputs[i], MulticlassComputeMethod.Elimination);
                Assert.AreEqual(expected, actual);
                Assert.AreEqual(0, msvm.GetLastKernelEvaluations());
            }

            for (int i = 0; i < inputs.Length; i++)
            {
                double expected = outputs[i];
                double actual = msvm.Compute(inputs[i], MulticlassComputeMethod.Voting);
                Assert.AreEqual(expected, actual);
                Assert.AreEqual(0, msvm.GetLastKernelEvaluations());
            }
        }

        [Test]
        public void multiclass_new_usage_method_linear()
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

            var learner = new MulticlassSupportVectorLearning<Linear>();

            learner.Learner = (_) => new LinearCoordinateDescent()
            {
                Complexity = 1
            };

            learner.ParallelOptions.MaxDegreeOfParallelism = 1;

            MulticlassSupportVectorMachine<Linear> msvm = learner.Learn(inputs, outputs);

            // Linear machines in compact form do not require kernel evaluations
            Assert.AreEqual(0, msvm.GetLastKernelEvaluations());

            for (int i = 0; i < inputs.Length; i++)
            {
                double expected = outputs[i];
                msvm.Method = MulticlassComputeMethod.Elimination;
                double actual = msvm.Decide(inputs[i]);
                Assert.AreEqual(expected, actual);
                Assert.AreEqual(0, msvm.GetLastKernelEvaluations());
            }

            for (int i = 0; i < inputs.Length; i++)
            {
                double expected = outputs[i];
                msvm.Method = MulticlassComputeMethod.Voting;
                double actual = msvm.Decide(inputs[i]);
                Assert.AreEqual(expected, actual);
                Assert.AreEqual(0, msvm.GetLastKernelEvaluations());
            }
        }

        [Test]
        public void multiclass_new_usage_method_polynomial()
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

            var learner = new MulticlassSupportVectorLearning<Polynomial>()
            {
                Learner = (p) => new SequentialMinimalOptimization<Polynomial>()
                {
                    Model = p.Model,
                    Complexity = 1,
                    Kernel = new Polynomial(2)
                }
            };

            learner.ParallelOptions.MaxDegreeOfParallelism = 1;

            MulticlassSupportVectorMachine<Polynomial> msvm = learner.Learn(inputs, outputs);

            Assert.AreEqual(0, msvm.GetLastKernelEvaluations());

            int[] evals = { 8, 8, 7, 7, 7, 7, 6, 6 };

            for (int i = 0; i < inputs.Length; i++)
            {
                double expected = outputs[i];
                msvm.Method = MulticlassComputeMethod.Elimination;
                double actual = msvm.Decide(inputs[i]);
                Assert.AreEqual(expected, actual);
                Assert.AreEqual(evals[i], msvm.GetLastKernelEvaluations());
            }

            for (int i = 0; i < inputs.Length; i++)
            {
                double expected = outputs[i];
                msvm.Method = MulticlassComputeMethod.Voting;
                double actual = msvm.Decide(inputs[i]);
                Assert.AreEqual(expected, actual);
                Assert.AreEqual(8, msvm.GetLastKernelEvaluations());
            }
        }

        [Test]
        public void ComputeTest2()
        {
            double[][] input =
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

            int[] output =
            {
                0, 0,
                1, 1,
                2, 2,
                3, 3,
            };


            IKernel kernel = new Polynomial(2);
            int classes = 4;
            int inputs = 5;


            // Create the Multi-class Support Vector Machine using the selected Kernel
            var msvm = new MulticlassSupportVectorMachine(inputs, kernel, classes);
            msvm.SupportVectorCache = 0;

            // Create the learning algorithm using the machine and the training data
            var ml = new MulticlassSupportVectorLearning(msvm, input, output);

            // Configure the learning algorithm
            ml.Algorithm = (svm, classInputs, classOutputs, i, j) =>
            {
                var smo = new SequentialMinimalOptimization(svm, classInputs, classOutputs)
                {
                    Complexity = 1
                };
                return smo;
            };

            Assert.AreEqual(0, msvm.GetLastKernelEvaluations());

#if DEBUG
            msvm.ParallelOptions.MaxDegreeOfParallelism = 1;
            ml.ParallelOptions.MaxDegreeOfParallelism = 1;
#endif

            // Executes the training algorithm
            double error = ml.Run();
            Assert.AreEqual(error, 0);

            Assert.AreEqual(6, msvm.GetLastKernelEvaluations());

            int[] evals = new int[input.Length];
            int[] evalexp = { 8, 8, 7, 7, 7, 7, 6, 6 };

            Parallel.For(0, input.Length, i =>
            {
                double[] data = input[i];
                double[] responses;

                int num = msvm.Compute(data, MulticlassComputeMethod.Elimination, out responses);
                Assert.AreEqual(output[i], num);

                evals[i] = msvm.GetLastKernelEvaluations();
            });

            for (int i = 0; i < evals.Length; i++)
                Assert.AreEqual(evals[i], evalexp[i]);

            Parallel.For(0, input.Length, i =>
            {
                double[] data = input[i];
                double[] responses;

                int num = msvm.Compute(data, MulticlassComputeMethod.Voting, out responses);
                Assert.AreEqual(output[i], num);

                evals[i] = msvm.GetLastKernelEvaluations();
            });

            for (int i = 0; i < evals.Length; i++)
                Assert.AreEqual(msvm.SupportVectorUniqueCount, evals[i]);
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
            var msvm = new MulticlassSupportVectorMachine(5, kernel, 4);
            var smo = new MulticlassSupportVectorLearning(msvm, inputs, outputs);
            smo.Algorithm = (svm, classInputs, classOutputs, i, j) =>
                new SequentialMinimalOptimization(svm, classInputs, classOutputs);

            double expected = smo.Run();


            MemoryStream stream = new MemoryStream();

            // Save the machines
            msvm.Save(stream);

            // Rewind
            stream.Seek(0, SeekOrigin.Begin);

            // Reload the machines
            var target = MulticlassSupportVectorMachine.Load(stream);

            double actual;

            int count = 0; // Compute errors
            for (int i = 0; i < inputs.Length; i++)
            {
                double y = target.Compute(inputs[i]);
                if (y != outputs[i]) count++;
            }

            actual = (double)count / inputs.Length;


            Assert.AreEqual(expected, actual);

            Assert.AreEqual(msvm.Inputs, target.Inputs);
            Assert.AreEqual(msvm.Classes, target.Classes);
            for (int i = 0; i < msvm.Machines.Length; i++)
            {
                for (int j = 0; j < msvm.Machines.Length; j++)
                {
                    var a = msvm[i, j];
                    var b = target[i, j];

                    if (i != j)
                    {
                        Assert.IsTrue(a.SupportVectors.IsEqual(b.SupportVectors));
                    }
                    else
                    {
                        Assert.IsNull(a);
                        Assert.IsNull(b);
                    }
                }
            }
        }

        [Test]
#if NETCORE
        [Ignore("Models created in .NET desktop cannot be de-serialized in .NET Core/Standard (yet)")]
#endif
        public void LoadTest1()
        {
            string fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "svm1.svm");

            var svm = MulticlassSupportVectorMachine.Load(fileName);

            Assert.IsNotNull(svm.Machines);
            Assert.IsFalse(svm.IsProbabilistic);
            Assert.AreEqual(351, svm.MachinesCount);
        }

        [Test]
#if NETCORE
        [Ignore("Models created in .NET desktop cannot be de-serialized in .NET Core/Standard (yet)")]
#endif
        public void LoadTest2()
        {
            string fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "svm2.svm");

            var ksvm = MulticlassSupportVectorMachine.Load(fileName);

            Assert.AreEqual(3, ksvm.Classes);
            Assert.AreEqual(21, ksvm.Inputs);
            Assert.AreEqual(2334, ksvm.SupportVectorCount);
            Assert.AreEqual(1584, ksvm.SupportVectorSharedCount);
            Assert.AreEqual(1542, ksvm.SupportVectorUniqueCount);
            Assert.AreEqual(false, ksvm.IsProbabilistic);
            Assert.AreEqual(0, ksvm.Machines[0][0].Weights.Sum());
            Assert.AreEqual(1.2115858453473118E-08d, ksvm.Machines[0][0].Weights.Variance());
            Assert.AreEqual(-0.00049625205093878355d, ksvm.Machines[0][0].Threshold);
            Assert.AreEqual(764, ksvm.Machines[0][0].SupportVectors.Length);

            Assert.AreEqual(0, ksvm.Machines[1][1].Weights.Sum());
            Assert.AreEqual(1.2115031055900578E-08d, ksvm.Machines[1][1].Weights.Variance());
            Assert.AreEqual(0.00010847163737093268, ksvm.Machines[1][1].Threshold);
            Assert.AreEqual(806, ksvm.Machines[1][1].SupportVectors.Length);
        }
#endif

        [Test]
#if DEBUG
        [Ignore("Disabled on Debug")]
#endif
        public void kaggle_digits_old_style()
        {
            string root = TestContext.CurrentContext.TestDirectory;
            var training = Properties.Resources.trainingsample;
            var validation = Properties.Resources.validationsample;

            var tset = readData(training);
            var observations = tset.Item1;
            var labels = tset.Item2;

            var teacher = new MulticlassSupportVectorLearning();
#if MONO
            teacher.ParallelOptions.MaxDegreeOfParallelism = 1;
#endif
            var svm = teacher.Learn(observations, labels);

            {
                var trainingLoss = new ZeroOneLoss(labels)
                {
                    Mean = true
                };

                double error = trainingLoss.Loss(svm.Decide(observations));
                Assert.AreEqual(0.054, error);
            }

            {
                var vset = readData(validation);
                var validationData = vset.Item1;
                var validationLabels = vset.Item2;

                var validationLoss = new ZeroOneLoss(validationLabels)
                {
                    Mean = true
                };

                double val = validationLoss.Loss(svm.Decide(validationData));
                Assert.AreEqual(0.082, val);
            }
        }

        [Test]
#if DEBUG
        [Ignore("Disabled on Debug")]
#endif
        public void kaggle_digits()
        {
            string root = TestContext.CurrentContext.TestDirectory;
            var training = Properties.Resources.trainingsample;
            var validation = Properties.Resources.validationsample;

            var tset = readData(training);
            var observations = tset.Item1;
            var labels = tset.Item2;

            var teacher = new MulticlassSupportVectorLearning<Linear>();

#if MONO
            teacher.ParallelOptions.MaxDegreeOfParallelism = 1;
#endif

            var svm = teacher.Learn(observations, labels);

            {
                var trainingLoss = new ZeroOneLoss(labels)
                {
                    Mean = true
                };

                double error = trainingLoss.Loss(svm.Decide(observations));
                Assert.AreEqual(0.054, error);
            }

            {
                var vset = readData(validation);
                var validationData = vset.Item1;
                var validationLabels = vset.Item2;

                var validationLoss = new ZeroOneLoss(validationLabels)
                {
                    Mean = true
                };

                double val = validationLoss.Loss(svm.Decide(validationData));
                Assert.AreEqual(0.082, val);
            }
        }

        [Test]
#if DEBUG
        [Ignore("Disabled on Debug")]
#endif
        public void kaggle_digits_with_compress()
        {
            string root = TestContext.CurrentContext.TestDirectory;
            var training = Properties.Resources.trainingsample;
            var validation = Properties.Resources.validationsample;

            var tset = readData(training);
            var observations = tset.Item1;
            var labels = tset.Item2;

            var teacher = new MulticlassSupportVectorLearning<Linear>();
#if MONO
            teacher.ParallelOptions.MaxDegreeOfParallelism = 1;
#endif
            var svm = teacher.Learn(observations, labels);

            Assert.AreEqual(50, svm.Models[0][0].SupportVectors.Length);
            Assert.AreEqual(127, svm.Models[1][0].SupportVectors.Length);
            svm.Compress();
            Assert.AreEqual(1, svm.Models[0][0].SupportVectors.Length);
            Assert.AreEqual(1, svm.Models[1][0].SupportVectors.Length);

            {
                var trainingLoss = new ZeroOneLoss(labels)
                {
                    Mean = true
                };

                double error = trainingLoss.Loss(svm.Decide(observations));
                Assert.AreEqual(0.054, error);
            }

            {
                var vset = readData(validation);
                var validationData = vset.Item1;
                var validationLabels = vset.Item2;

                var validationLoss = new ZeroOneLoss(validationLabels)
                {
                    Mean = true
                };

                double val = validationLoss.Loss(svm.Decide(validationData));
                Assert.AreEqual(0.082, val);
            }
        }


        private static Tuple<double[][], int[]> readData(string filePath)
        {
            var lines = filePath.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var output = new int[lines.Length - 1];
            var inputs = new double[lines.Length - 1][];
            for (int i = 0; i < lines.Length - 1; i++)
            {
                string[] parts = lines[i + 1].Split(',');
                output[i] = int.Parse(parts[0]);
                inputs[i] = new double[parts.Length - 1];
                for (int j = 0; j < parts.Length - 1; j++)
                {
                    inputs[i][j] = double.Parse(parts[1 + j]);
                }
            }

            return Tuple.Create(inputs, output);
        }
    }
}
