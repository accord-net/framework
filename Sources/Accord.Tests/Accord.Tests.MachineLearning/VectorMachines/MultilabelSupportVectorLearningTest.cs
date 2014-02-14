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
    using Accord.MachineLearning.VectorMachines;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Math;
    using Accord.Statistics.Kernels;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.IO;
    using System.Threading.Tasks;

    [TestClass()]
    public class MultilabelSupportVectorLearningTest
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
        public void RunTest()
        {
            Accord.Math.Tools.SetupGenerator(0);

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
            Assert.AreEqual(2 / 16.0, error);
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
            var msvm = new MultilabelSupportVectorMachine(5, kernel, 4);
            var smo = new MultilabelSupportVectorLearning(msvm, inputs, outputs);
            smo.Algorithm = (svm, classInputs, classOutputs, i, j) =>
                new SequentialMinimalOptimization(svm, classInputs, classOutputs);

            Assert.AreEqual(0, msvm.GetLastKernelEvaluations());

            double error = smo.Run();
            Assert.AreEqual(0, error);


            int[] evals = new int[inputs.Length];
            for (int i = 0; i < inputs.Length; i++)
            {
                double expected = outputs[i];
                double[] responses; msvm.Compute(inputs[i], out responses);
                int actual; responses.Max(out actual);
                Assert.AreEqual(expected, actual);
                evals[i] = msvm.GetLastKernelEvaluations();
            }

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
            var msvm = new MultilabelSupportVectorMachine(5, kernel, 4);
            var smo = new MultilabelSupportVectorLearning(msvm, inputs, outputs);
            smo.Algorithm = (svm, classInputs, classOutputs, i, j) =>
                new SequentialMinimalOptimization(svm, classInputs, classOutputs);

            double expected = smo.Run();


            MemoryStream stream = new MemoryStream();

            // Save the machines
            msvm.Save(stream);

            // Rewind
            stream.Seek(0, SeekOrigin.Begin);

            // Reload the machines
            var target = MultilabelSupportVectorMachine.Load(stream);

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

                Assert.IsTrue(a.SupportVectors.IsEqual(b.SupportVectors));
            }
        }

    }
}
