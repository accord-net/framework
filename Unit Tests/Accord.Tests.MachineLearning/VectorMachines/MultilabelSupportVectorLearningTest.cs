// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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
    using Accord.IO;
    using Accord.MachineLearning.VectorMachines;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Math;
    using Accord.Statistics.Kernels;
    using NUnit.Framework;
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
            bool[][] train = Special.Decide(outputs);

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
                msvm.Distances(inputs[i], out actual);
                Assert.AreEqual(y[i], actual);
            }
        }

        [Test]
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
            var target = Serializer.Load<MultilabelSupportVectorMachine>(Properties.Resources.ml_svm);

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
    }
}
