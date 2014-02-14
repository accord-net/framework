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

    using System.IO;
    using System.Threading.Tasks;
    using Accord.MachineLearning.VectorMachines;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Math;
    using Accord.Statistics.Kernels;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class MulticlassSupportVectorMachineTest
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
                    if (i == j) continue;

                    var machine = target[i, j];
                    Assert.IsNotNull(machine);
                }
            }
        }

        [TestMethod()]
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

        [TestMethod()]
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
                new SequentialMinimalOptimization(svm, classInputs, classOutputs);

            Assert.AreEqual(0, msvm.GetLastKernelEvaluations());

            double error = smo.Run();

            Assert.AreEqual(6, msvm.GetLastKernelEvaluations());

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

        [TestMethod()]
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
            MulticlassSupportVectorMachine msvm = new MulticlassSupportVectorMachine(inputs, kernel, classes);

            // Create the learning algorithm using the machine and the training data
            MulticlassSupportVectorLearning ml = new MulticlassSupportVectorLearning(msvm, input, output);

            // Configure the learning algorithm
            ml.Algorithm = (svm, classInputs, classOutputs, i, j) =>
            {
                var smo = new SequentialMinimalOptimization(svm, classInputs, classOutputs);
                return smo;
            };

            Assert.AreEqual(0, msvm.GetLastKernelEvaluations());

            // Executes the training algorithm
            double error = ml.Run();

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

        [TestMethod()]
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

        [TestMethod()]
        public void LoadTest1()
        {
            MemoryStream stream = new MemoryStream(Properties.Resources.svm1);
            var svm = MulticlassSupportVectorMachine.Load(stream);

            Assert.IsNotNull(svm.Machines);
            Assert.IsFalse(svm.IsProbabilistic);
            Assert.AreEqual(351, svm.MachinesCount);
        }

        [TestMethod()]
        public void LoadTest2()
        {
            byte[] blob = Properties.Resources.svm2;
            MemoryStream stream = new MemoryStream(blob);

            var ksvm = MulticlassSupportVectorMachine.Load(stream);

            Assert.AreEqual(3, ksvm.Classes);
            Assert.AreEqual(21, ksvm.Inputs);
            Assert.AreEqual(2334, ksvm.SupportVectorCount);
            Assert.AreEqual(1542, ksvm.SupportVectorSharedCount);
            Assert.AreEqual(1542, ksvm.SupportVectorUniqueCount);
            Assert.AreEqual(false, ksvm.IsProbabilistic);
        }

    }
}
