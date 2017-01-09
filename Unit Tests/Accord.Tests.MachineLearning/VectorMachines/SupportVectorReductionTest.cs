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
    using Accord.Statistics.Kernels;
    using NUnit.Framework;
    using System.IO;
    using Math.Optimization.Losses;

    [TestFixture]
    public class SupportVectorReductionTest
    {

        [Test]
        public void ComputeTest()
        {
            // Example AND problem
            double[][] inputs =
            {
                new double[] { 0, 0 }, // 0 and 0: 0 (label -1)
                new double[] { 0, 1 }, // 0 and 1: 0 (label -1)
                new double[] { 1, 0 }, // 1 and 0: 0 (label -1)
                new double[] { 1, 1 }  // 1 and 1: 1 (label +1)
            };

            // Dichotomy SVM outputs should be given as [-1;+1]
            int[] labels =
            {
                // 0,  0,  0, 1
                  -1, -1, -1, 1
            };

            // Create a Support Vector Machine for the given inputs
            KernelSupportVectorMachine machine = new KernelSupportVectorMachine(new Linear(0), inputs[0].Length);

            // Instantiate a new learning algorithm for SVMs
            SequentialMinimalOptimization smo = new SequentialMinimalOptimization(machine, inputs, labels);

            // Set up the learning algorithm
            smo.Complexity = 100.0;

            // Run
            double error = smo.Run();

            Assert.AreEqual(0, error);
            Assert.AreEqual(-1, Math.Sign(machine.Compute(inputs[0])));
            Assert.AreEqual(-1, Math.Sign(machine.Compute(inputs[1])));
            Assert.AreEqual(-1, Math.Sign(machine.Compute(inputs[2])));
            Assert.AreEqual(+1, Math.Sign(machine.Compute(inputs[3])));

            // At this point we have the weighted support vectors
            //     w        sv        b
            //   (+4)  *  (1,1)      -3
            //   (-2)  *  (1,0)
            //   (-2)  *  (0,1)
            //
            // However, it can be seen that the last SV can be written
            // as a linear combination of the two first vectors:
            //
            //   (0,1) = (1,1) - (1,0)
            //
            // Since we have a linear space (we are using a linear kernel)
            // this vector could be removed from the support vector set.
            //
            // f(x) = sum(alpha_i * x * x_i) + b
            //      = 4*(1,1)*x - 2*(1,0)*x - 2*(0,1)*x             - 3
            //      = 4*(1,1)*x - 2*(1,0)*x - 2*((1,1) - (1,0))*x   - 3
            //      = 4*(1,1)*x - 2*(1,0)*x - 2*(1,1)*x + 2*(1,0)*x - 3
            //      = 4*(1,1)*x - 2*(1,0)*x - 2*(1,1)*x + 2*(1,0)*x - 3
            //      = 2*(1,1)*x - 3
            //      = 2*x1 + 2*x2 - 3
            //

            SupportVectorReduction svr = new SupportVectorReduction(machine);

            double error2 = svr.Run();


            Assert.AreEqual(-1, Math.Sign(machine.Compute(inputs[0])));
            Assert.AreEqual(-1, Math.Sign(machine.Compute(inputs[1])));
            Assert.AreEqual(-1, Math.Sign(machine.Compute(inputs[2])));
            Assert.AreEqual(+1, Math.Sign(machine.Compute(inputs[3])));
        }

        [Test]
        public void learn_test()
        {
            #region doc_learn
            // Example AND problem
            double[][] inputs =
            {
                new double[] { 0, 0 }, // 0 and 0: 0 (label -1)
                new double[] { 0, 1 }, // 0 and 1: 0 (label -1)
                new double[] { 1, 0 }, // 1 and 0: 0 (label -1)
                new double[] { 1, 1 }  // 1 and 1: 1 (label +1)
            };

            // Dichotomy SVM outputs should be given as [-1;+1]
            int[] labels =
            {
                // 0,  0,  0, 1
                  -1, -1, -1, 1
            };

            // Instantiate a new learning algorithm for SVMs
            var smo = new SequentialMinimalOptimization<Linear>()
            {

                // Set up the learning algorithm
                Complexity = 100.0
            };

            // Run
            var svm = smo.Learn(inputs, labels);

            int numberBefore = svm.SupportVectors.Length; // should be

            double errorBefore = new ZeroOneLoss(labels).Loss(svm.Decide(inputs)); // should be 0


            // At this point we have the weighted support vectors
            //     w        sv        b
            //   (+4)  *  (1,1)      -3
            //   (-2)  *  (1,0)
            //   (-2)  *  (0,1)
            //
            // However, it can be seen that the last SV can be written
            // as a linear combination of the two first vectors:
            //
            //   (0,1) = (1,1) - (1,0)
            //
            // Since we have a linear space (we are using a linear kernel)
            // this vector could be removed from the support vector set.
            //
            // f(x) = sum(alpha_i * x * x_i) + b
            //      = 4*(1,1)*x - 2*(1,0)*x - 2*(0,1)*x             - 3
            //      = 4*(1,1)*x - 2*(1,0)*x - 2*((1,1) - (1,0))*x   - 3
            //      = 4*(1,1)*x - 2*(1,0)*x - 2*(1,1)*x + 2*(1,0)*x - 3
            //      = 4*(1,1)*x - 2*(1,0)*x - 2*(1,1)*x + 2*(1,0)*x - 3
            //      = 2*(1,1)*x - 3
            //      = 2*x1 + 2*x2 - 3
            //

            // Clone the original machine so we don't change it
            var clone = (SupportVectorMachine<Linear>)svm.Clone();

            // Create a support vector reduction algorithm
            var svr = new SupportVectorReduction<Linear>(clone);

            // Reduce the number of support vectors
            var svm2 = svr.Learn();

            int numberAfter = svm2.SupportVectors.Length; // should be 1

            // Compute the new error
            double errorAfter = new ZeroOneLoss(labels).Loss(svm2.Decide(inputs)); // should still be 0
            #endregion

            Assert.AreEqual(3, numberBefore);
            Assert.AreEqual(1, numberAfter);

            Assert.AreEqual(4, svm.Weights[0], 1e-5);
            Assert.AreEqual(-2, svm.Weights[1], 1e-5);
            Assert.AreEqual(-2, svm.Weights[2], 1e-5);
            Assert.AreEqual(-3, svm.Threshold, 1e-5);

            Assert.AreNotSame(svm, svm2);
            Assert.AreSame(clone, svm2);
            Assert.IsTrue(numberAfter < numberBefore);

            Assert.AreEqual(0, errorBefore);
            Assert.AreEqual(0, errorAfter);
            Assert.AreEqual(false, svm.Decide(inputs[0]));
            Assert.AreEqual(false, svm.Decide(inputs[1]));
            Assert.AreEqual(false, svm.Decide(inputs[2]));
            Assert.AreEqual(true, svm.Decide(inputs[3]));

            Assert.AreEqual(false, svm2.Decide(inputs[0]));
            Assert.AreEqual(false, svm2.Decide(inputs[1]));
            Assert.AreEqual(false, svm2.Decide(inputs[2]));
            Assert.AreEqual(true, svm2.Decide(inputs[3]));
        }

    }
}
