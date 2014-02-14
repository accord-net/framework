// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
// cesarsouza at gmail.com
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
// 

namespace Accord.Tests.MachineLearning.GPL
{
    using Accord.MachineLearning.VectorMachines;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Statistics.Kernels;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SequentialMinimalOptimizationRegressionTest
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
        public void TrainTest()
        {

            // Example regression problem. Suppose we are trying
            // to model the following equation: f(x, y) = 2x + y

            double[][] inputs = // (x, y)
            {
                new double[] { 0,  1 }, // 2*0 + 1 =  1
                new double[] { 4,  3 }, // 2*4 + 3 = 11
                new double[] { 8, -8 }, // 2*8 - 8 =  8
                new double[] { 2,  2 }, // 2*2 + 2 =  6
                new double[] { 6,  1 }, // 2*6 + 1 = 13
                new double[] { 5,  4 }, // 2*5 + 4 = 14
                new double[] { 9,  1 }, // 2*9 + 1 = 20
                new double[] { 1,  6 }, // 2*1 + 6 =  5
            };

            double[] outputs = // f(x, y)
            {
                    1, 11, 8, 6, 13, 14, 20, 5
            };

            // Create Kernel Support Vector Machine with a Polynomial Kernel of 2nd degree
            var machine = new KernelSupportVectorMachine(new Polynomial(2), inputs: 2);

            // Create the sequential minimal optimization teacher
            var learn = new SequentialMinimalOptimizationRegression(machine, inputs, outputs);

            // Run the learning algorithm
            double error = learn.Run();

            // Compute the answer for one particular example
            double fxy = machine.Compute(inputs[0]); // 1.0003849827673186

            // Check for correct answers
            double[] answers = new double[inputs.Length];
            for (int i = 0; i < answers.Length; i++)
                answers[i] = machine.Compute(inputs[i]);

            Assert.AreEqual(1.0003849827673186, fxy);
            for (int i = 0; i < outputs.Length; i++)
                Assert.AreEqual(outputs[i], answers[i], 1.0);
        }
    }
}
