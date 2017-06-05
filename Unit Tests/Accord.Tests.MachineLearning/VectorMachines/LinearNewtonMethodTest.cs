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
    using Accord.MachineLearning.VectorMachines;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Math;
    using Accord.Statistics.Analysis;
    using Accord.Statistics.Kernels;
    using NUnit.Framework;
    using System;
    using Accord.Statistics.Models.Regression.Linear;
    using Math.Optimization.Losses;

    [TestFixture]
    public class LinearNewtonMethodTest
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

            var kernel = new Polynomial(2, 0.0);

            double[][] augmented = new double[inputs.Length][];
            for (int i = 0; i < inputs.Length; i++)
                augmented[i] = kernel.Transform(inputs[i]);

            SupportVectorMachine machine = new SupportVectorMachine(augmented[0].Length);

            // Create the Least Squares Support Vector Machine teacher
            var learn = new LinearNewtonMethod(machine, augmented, xor);

            // Run the learning algorithm
            double error = learn.Run();
            Assert.AreEqual(0, error);

            int[] output = augmented.Apply(p => Math.Sign(machine.Compute(p)));
            for (int i = 0; i < output.Length; i++)
                Assert.AreEqual(System.Math.Sign(xor[i]), System.Math.Sign(output[i]));
        }

        [Test]
        public void linear_regression_test()
        {
            #region doc_linreg
            // Declare some training data. This is exactly the same
            // data used in the MultipleLinearRegression documentation page

            // We will try to model a plane as an equation in the form
            // "ax + by + c = z". We have two input variables (x and y)
            // and we will be trying to find two parameters a and b and 
            // an intercept term c.

            // Create a linear-SVM learning method
            var teacher = new LinearNewtonMethod()
            {
                Tolerance = 1e-10,
                Complexity = 1e+10, // learn a hard-margin model
            };

            // Now suppose you have some points
            double[][] inputs =
            {
                new double[] { 1, 1 },
                new double[] { 0, 1 },
                new double[] { 1, 0 },
                new double[] { 0, 0 },
            };

            // located in the same Z (z = 1)
            double[] outputs = { 1, 1, 1, 1 };

            // Learn the support vector machine
            var svm = teacher.Learn(inputs, outputs);

            // Convert the svm to logistic regression
            var regression = (MultipleLinearRegression)svm;

            // As result, we will be given the following:
            double a = regression.Weights[0]; // a = 0
            double b = regression.Weights[1]; // b = 0
            double c = regression.Intercept;  // c = 1

            // This is the plane described by the equation
            // ax + by + c = z => 0x + 0y + 1 = z => 1 = z.

            // We can compute the predicted points using
            double[] predicted = regression.Transform(inputs);

            // And the squared error loss using 
            double error = new SquareLoss(outputs).Loss(predicted);
            #endregion

            Assert.AreEqual(2, regression.NumberOfInputs);
            Assert.AreEqual(1, regression.NumberOfOutputs);


            Assert.AreEqual(0.0, a, 1e-6);
            Assert.AreEqual(0.0, b, 1e-6);
            Assert.AreEqual(1.0, c, 1e-6);
            Assert.AreEqual(0.0, error, 1e-6);

            double[] expected = regression.Compute(inputs);
            double[] actual = regression.Transform(inputs);
            Assert.IsTrue(expected.IsEqual(actual, 1e-10));

            double r = regression.CoefficientOfDetermination(inputs, outputs);
            Assert.AreEqual(1.0, r);
        }

        [Test]
        public void linear_regression_sparse_test()
        {
            #region doc_linreg_sparse
            // Declare some training data. This is exactly the same
            // data used in the MultipleLinearRegression documentation page

            // We will try to model a plane as an equation in the form
            // "ax + by + c = z". We have two input variables (x and y)
            // and we will be trying to find two parameters a and b and 
            // an intercept term c.

            // Create a linear-SVM learning method
            var teacher = new LinearNewtonMethod<Linear, Sparse<double>>()
            {
                Tolerance = 1e-10,
                Complexity = 1e+10, // learn a hard-margin model
            };

            // Now suppose you have some points
            Sparse<double>[] inputs = Sparse.FromDense(new[] 
            {
                new double[] { 1, 1 },
                new double[] { 0, 1 },
                new double[] { 1, 0 },
                new double[] { 0, 0 },
            });

            // located in the same Z (z = 1)
            double[] outputs = { 1, 1, 1, 1 };

            // Learn the support vector machine
            var svm = teacher.Learn(inputs, outputs);

            // Convert the svm to logistic regression
            var regression = (MultipleLinearRegression)svm;

            // As result, we will be given the following:
            double a = regression.Weights[0]; // a = 0
            double b = regression.Weights[1]; // b = 0
            double c = regression.Intercept;  // c = 1

            // This is the plane described by the equation
            // ax + by + c = z => 0x + 0y + 1 = z => 1 = z.

            // We can compute the predicted points using
            double[] predicted = regression.Transform(inputs.ToDense());

            // And the squared error loss using 
            double error = new SquareLoss(outputs).Loss(predicted);
            #endregion

            Assert.AreEqual(2, regression.NumberOfInputs);
            Assert.AreEqual(1, regression.NumberOfOutputs);


            Assert.AreEqual(0.0, a, 1e-6);
            Assert.AreEqual(0.0, b, 1e-6);
            Assert.AreEqual(1.0, c, 1e-6);
            Assert.AreEqual(0.0, error, 1e-6);

            double[] expected = regression.Compute(inputs.ToDense());
            double[] actual = regression.Transform(inputs.ToDense());
            Assert.IsTrue(expected.IsEqual(actual, 1e-10));

            double r = regression.CoefficientOfDetermination(inputs.ToDense(), outputs);
            Assert.AreEqual(1.0, r);
        }

        [Test]
        public void sparse_zero_vector_test()
        {
            // Create a linear-SVM learning method
            var teacher = new LinearNewtonMethod<Linear, Sparse<double>>()
            {
                Tolerance = 1e-10,
                Complexity = 1e+10, // learn a hard-margin model
            };

            // Now suppose you have some points
            Sparse<double>[] inputs = Sparse.FromDense(new[]
            {
                new double[] { 1, 1, 2 },
                new double[] { 0, 1, 6 },
                new double[] { 1, 0, 8 },
                new double[] { 0, 0, 0 },
            });

            int[] outputs = { 1, -1, 1, -1 };

            // Learn the support vector machine
            var svm = teacher.Learn(inputs, outputs);

            // Compute the predicted points 
            bool[] predicted = svm.Decide(inputs);

            // And the squared error loss using 
            double error = new ZeroOneLoss(outputs).Loss(predicted);

            Assert.AreEqual(3, svm.NumberOfInputs);
            Assert.AreEqual(1, svm.NumberOfOutputs);
            Assert.AreEqual(2, svm.NumberOfClasses);

            Assert.AreEqual(1, svm.Weights.Length);
            Assert.AreEqual(1, svm.SupportVectors.Length);

            Assert.AreEqual(1.0, svm.Weights[0], 1e-6);
            Assert.AreEqual(2.0056922148257597, svm.SupportVectors[0][0], 1e-6);
            Assert.AreEqual(-0.0085361347231909836, svm.SupportVectors[0][1], 1e-6);
            Assert.AreEqual(0.0014225721169379331, svm.SupportVectors[0][2], 1e-6);
            Assert.AreEqual(0.0, error);
        }



        [Test]
        public void ComputeTest5()
        {
            var dataset = SequentialMinimalOptimizationTest.GetYingYang();
            var inputs = dataset.Submatrix(null, 0, 1).ToJagged();
            var labels = dataset.GetColumn(2).ToInt32();

            var kernel = new Polynomial(2, 0);

            {
                var machine = new KernelSupportVectorMachine(kernel, inputs[0].Length);
                var smo = new SequentialMinimalOptimization(machine, inputs, labels);
                smo.UseComplexityHeuristic = true;

                double error = smo.Run();
                Assert.AreEqual(0.2, error);

                Assert.AreEqual(0.11714451552090824, smo.Complexity);

                int[] actual = new int[labels.Length];
                for (int i = 0; i < actual.Length; i++)
                    actual[i] = Math.Sign(machine.Compute(inputs[i]));

                ConfusionMatrix matrix = new ConfusionMatrix(actual, labels);
                Assert.AreEqual(20, matrix.FalseNegatives);
                Assert.AreEqual(0, matrix.FalsePositives);
                Assert.AreEqual(30, matrix.TruePositives);
                Assert.AreEqual(50, matrix.TrueNegatives);
            }

            {
                Accord.Math.Tools.SetupGenerator(0);

                var projection = inputs.Apply(kernel.Transform);
                var machine = new SupportVectorMachine(projection[0].Length);
                var smo = new LinearNewtonMethod(machine, projection, labels);
                smo.UseComplexityHeuristic = true;

                double error = smo.Run();
                Assert.AreEqual(0.18, error);

                Assert.AreEqual(0.11714451552090821, smo.Complexity, 1e-15);

                int[] actual = new int[labels.Length];
                for (int i = 0; i < actual.Length; i++)
                    actual[i] = Math.Sign(machine.Compute(projection[i]));

                ConfusionMatrix matrix = new ConfusionMatrix(actual, labels);
                Assert.AreEqual(17, matrix.FalseNegatives);
                Assert.AreEqual(1, matrix.FalsePositives);
                Assert.AreEqual(33, matrix.TruePositives);
                Assert.AreEqual(49, matrix.TrueNegatives);
            }

        }


        [Test]
        public void learn_linear_sparse()
        {
            #region doc_xor_sparse
            // As an example, we will try to learn a linear machine  that can 
            // replicate the "exclusive-or" logical function. However, since we
            // will be using a linear SVM, we will not be able to solve this 
            // problem perfectly as the XOR is a non-linear classification problem:
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
            var learn = new LinearNewtonMethod<Linear, Sparse<double>>()
            {
                UseComplexityHeuristic = true,
                UseKernelEstimation = false
            };

            // And then we can obtain a trained SVM by calling its Learn method
            var svm = learn.Learn(inputs, xor);

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


            learn = new LinearNewtonMethod<Linear, Sparse<double>>()
            {
                Complexity = 1e+8,
                UseKernelEstimation = false
            };

            svm = learn.Learn(inputs, or);

            prediction = svm.Decide(inputs);

            Assert.AreEqual(0, inputs[0].Indices.Length);
            Assert.AreEqual(1, inputs[1].Indices.Length);
            Assert.AreEqual(1, inputs[2].Indices.Length);
            Assert.AreEqual(2, inputs[3].Indices.Length);

            Assert.AreEqual(prediction[0], false);
            Assert.AreEqual(prediction[1], true);
            Assert.AreEqual(prediction[2], true);
            Assert.AreEqual(prediction[3], true);
        }
    }
}
