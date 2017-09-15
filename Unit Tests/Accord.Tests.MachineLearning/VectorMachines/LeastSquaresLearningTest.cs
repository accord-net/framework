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
    using Accord.Statistics.Kernels;
    using NUnit.Framework;
    using Accord.Statistics.Analysis;
    using Accord.DataSets;

    [TestFixture]
    public class LeastSquaresLearningTest
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

            // Create the Least Squares Support Vector Machine teacher
            LeastSquaresLearning learn = new LeastSquaresLearning(machine, inputs, xor);
            learn.Complexity = 10;

            // Run the learning algorithm
            learn.Run();


            int[] output = inputs.Apply(p => Math.Sign(machine.Compute(p)));

            for (int i = 0; i < output.Length; i++)
                Assert.AreEqual(System.Math.Sign(xor[i]), System.Math.Sign(output[i]));
        }

        [Test]
        public void LearnTest2()
        {
            var dataset = new YinYang();
            double[][] inputs = dataset.Instances;
            int[] outputs = dataset.ClassLabels.Apply(x => x ? 1 : -1);

            // Create Kernel Support Vector Machine with a Polynomial Kernel of 2nd degree
            KernelSupportVectorMachine machine = new KernelSupportVectorMachine(new Polynomial(3), inputs[0].Length);

            // Create the Least Squares Support Vector Machine teacher
            LeastSquaresLearning learn = new LeastSquaresLearning(machine, inputs, outputs);
            learn.Complexity = 1 / 0.1;

            // Run the learning algorithm
            learn.Run();


            int[] output = inputs.Apply(p => Math.Sign(machine.Compute(p)));

            for (int i = 0; i < output.Length; i++)
                Assert.AreEqual(System.Math.Sign(outputs[i]), System.Math.Sign(output[i]));
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

            LeastSquaresLearning smo = new LeastSquaresLearning(svm, inputs, outputs);


            double error = smo.Run();

            Assert.AreEqual(0, error);
        }

        [Test]
        public void LeastSquaresConstructorTest()
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
            var machine = new SupportVectorMachine(inputs[0].Length);

            var learn = new LeastSquaresLearning(machine, inputs, or);

            double error = learn.Run();
            Assert.AreEqual(0, error);

            {
                int[] iout = new int[inputs.Length];
                machine.ToMulticlass().Decide(inputs, iout);
                for (int i = 0; i < iout.Length; i++)
                    Assert.AreEqual(or[i], iout[i]);
            }
            {
                double[] dout = new double[inputs.Length];
                machine.ToMulticlass().Decide(inputs, dout);
                for (int i = 0; i < dout.Length; i++)
                    Assert.AreEqual(or[i], dout[i]);
            }
            {
                bool[] bout = new bool[inputs.Length];
                machine.Decide(inputs, bout);
                Assert.IsFalse(bout[0]);
                Assert.IsFalse(bout[1]);
                Assert.IsFalse(bout[2]);
                Assert.IsTrue(bout[3]);
            }
            {
                int[][] iiout = Jagged.Create<int>(inputs.Length, 2);
                machine.ToMulticlass().Decide(inputs, iiout);
                for (int i = 0; i < iiout.Length; i++)
                {
                    Assert.AreEqual(or[i], iiout[i][0]);
                    Assert.AreEqual(or[i], iiout[i][1] == 1 ? 0 : 1);
                }
            }
            {
                bool[][] bbout = Jagged.Create<bool>(inputs.Length, 2);
                machine.ToMulticlass().Decide(inputs, bbout);
                for (int i = 0; i < bbout.Length; i++)
                {
                    Assert.AreEqual(or[i], bbout[i][0] ? 1 : 0);
                    Assert.AreEqual(or[i], bbout[i][1] ? 0 : 1);
                }
            }
        }

    }
}
