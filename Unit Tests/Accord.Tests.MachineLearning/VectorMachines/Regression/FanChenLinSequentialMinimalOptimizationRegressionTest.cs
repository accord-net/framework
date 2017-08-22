// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
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
    using Accord.Math.Optimization.Losses;
    using Accord.Statistics.Kernels;
    using Math;
    using NUnit.Framework;

    [TestFixture]
    public class FanChenLinSequentialMinimalOptimizationRegressionTest
    {

        [Test]
        public void learn_test()
        {
            #region doc_learn
            Accord.Math.Random.Generator.Seed = 0;

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

            // Create the sequential minimal optimization teacher
            var learn = new FanChenLinSupportVectorRegression()
            {
                Complexity = 100
            };

            // Run the learning algorithm
            SupportVectorMachine svm = learn.Learn(inputs, outputs);

            // Compute the predicted scores
            double[] predicted = svm.Score(inputs);

            // Compute the error between the expected and predicted
            double error = new SquareLoss(outputs).Loss(predicted);

            // Compute the answer for one particular example
            double fxy = svm.Score(inputs[0]); // 1.000776033448912
            #endregion

            Assert.AreEqual(1.0, fxy, 1e-3);
            for (int i = 0; i < outputs.Length; i++)
                Assert.AreEqual(outputs[i], predicted[i], 2e-3);
        }

        [Test]
        public void learn_test_polynomial()
        {
            Accord.Math.Random.Generator.Seed = 0;

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

            // Create the sequential minimal optimization teacher
            var learn = new FanChenLinSupportVectorRegression<Polynomial>()
            {
                Kernel = new Polynomial(degree: 1, constant: 0),
                Complexity = 100
            };

            // Run the learning algorithm
            SupportVectorMachine<Polynomial> svm = learn.Learn(inputs, outputs);

            // Compute the predicted scores
            double[] predicted = svm.Score(inputs);

            // Compute the error between the expected and predicted
            double error = new SquareLoss(outputs).Loss(predicted);

            // Compute the answer for one particular example
            double fxy = svm.Score(inputs[0]); // 1.000776033448912

            Assert.AreEqual(1.0, fxy, 1e-3);
            for (int i = 0; i < outputs.Length; i++)
                Assert.AreEqual(outputs[i], predicted[i], 2e-3);
        }

        [Test]
        public void learn_test_square_polynomial()
        {
            Accord.Math.Random.Generator.Seed = 0;

            // Example regression problem. Suppose we are trying
            // to model the following equation: f(x) = x * x

            double[][] inputs = // (x)
            {
                new double[] { -1 },
                new double[] { 4 },
                new double[] { 8 },
                new double[] { 2 },
                new double[] { 6 },
                new double[] { 5 },
                new double[] { 9 },
                new double[] { 1 },
                new double[] { 6 },
                new double[] { -5 },
                new double[] { -2 },
                new double[] { -3 },
                new double[] { 5 },
                new double[] { 4 },
                new double[] { 1 },
                new double[] { 2 },
                new double[] { 0 },
                new double[] { 4 },
                new double[] { 8 },
                new double[] { 2 },
                new double[] { 6 },
                new double[] { 52 },
                new double[] { 95 },
                new double[] { 1 },
                new double[] { 6 },
                new double[] { 5 },
                new double[] { -1 },
                new double[] { 2 },
                new double[] { 5 },
                new double[] { 4 },
                new double[] { -4 },
                new double[] { -50 },
            };

            double[] outputs = inputs.GetColumn(0).Pow(2);

            // Create the sequential minimal optimization teacher
            var learn = new FanChenLinSupportVectorRegression<Polynomial>()
            {
                Kernel = new Polynomial(degree: 2, constant: 0),
                Complexity = 100
            };

            // Run the learning algorithm
            SupportVectorMachine<Polynomial> svm = learn.Learn(inputs, outputs);

            // Compute the predicted scores
            double[] predicted = svm.Score(inputs);

            // Compute the error between the expected and predicted
            double error = new SquareLoss(outputs).Loss(predicted);

            // Compute the answer for one particular example
            double fxy = svm.Score(inputs[0]); // 1.000776033448912

            Assert.AreEqual(1.0, fxy, 1e-3);
            for (int i = 0; i < outputs.Length; i++)
                Assert.AreEqual(outputs[i], predicted[i], 2e-3);
        }

    }
}
