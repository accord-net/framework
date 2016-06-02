﻿// Accord Unit Tests
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
    using Accord.MachineLearning.VectorMachines;
    using Accord.MachineLearning.VectorMachines.Learning;
    using NUnit.Framework;

    [TestFixture]
    public class LinearRegressionNewtonMethodTest
    {

        [Test]
        public void RunTest()
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
                new double[] { 9,  1 }, // 2*9 + 1 = 19
                new double[] { 1,  6 }, // 2*1 + 6 =  8
            };

            double[] outputs = // f(x, y)
            {
                    1, 11, 8, 6, 13, 14, 19, 8
            };

            // Create a new linear Support Vector Machine 
            var machine = new SupportVectorMachine(inputs: 2);

            // Create the linear regression coordinate descent teacher
            var learn = new LinearRegressionNewtonMethod(machine, inputs, outputs)
            {
                Complexity = 100000000,
                Epsilon = 1e-15,
                Tolerance = 1e-15,
            };

            // Run the learning algorithm
            double error = learn.Run();
            Assert.AreEqual(0.0000000000000000030183002120114053, error);

            // Compute the answer for one particular example
            double fxy = machine.Compute(inputs[0]); // 1.0003849827673186

            // Check for correct answers
            double[] answers = new double[inputs.Length];
            for (int i = 0; i < answers.Length; i++)
                answers[i] = machine.Compute(inputs[i]);

            Assert.AreEqual(1.0, fxy, 1e-5);
            for (int i = 0; i < outputs.Length; i++)
                Assert.AreEqual(outputs[i], answers[i], 1e-5);
        }

    }
}
