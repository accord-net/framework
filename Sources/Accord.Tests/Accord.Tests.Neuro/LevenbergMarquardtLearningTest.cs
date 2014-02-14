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

namespace Accord.Tests.Neuro
{
    using Accord.Neuro.Learning;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using AForge.Neuro;
    using Accord.Math;
    using AForge.Neuro.Learning;
    using Accord.Neuro;
    using System;
    using AForge;

    [TestClass()]
    public class LevenbergMarquardtLearningTest
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
        public void RunEpochTest1()
        {
            Accord.Math.Tools.SetupGenerator(0);

            double[][] input = 
            {
                new double[] { -1, -1 },
                new double[] { -1,  1 },
                new double[] {  1, -1 },
                new double[] {  1,  1 }
            };

            double[][] output =
            {
                new double[] { -1 },
                new double[] {  1 },
                new double[] {  1 },
                new double[] { -1 }
            };

            Neuron.RandGenerator = new ThreadSafeRandom(0);

            ActivationNetwork network = new ActivationNetwork(
                   new BipolarSigmoidFunction(2), 2, 2, 1);

            var teacher = new LevenbergMarquardtLearning(network,
                false, JacobianMethod.ByFiniteDifferences);

            double error = 1.0;
            while (error > 1e-5)
                error = teacher.RunEpoch(input, output);

            for (int i = 0; i < input.Length; i++)
                Assert.AreEqual(network.Compute(input[i])[0], output[i][0], 0.1);
        }

        [TestMethod()]
        public void RunEpochTest2()
        {
            Accord.Math.Tools.SetupGenerator(0);

            double[][] input = 
            {
                new double[] { -1, -1 },
                new double[] { -1,  1 },
                new double[] {  1, -1 },
                new double[] {  1,  1 }
            };

            double[][] output =
            {
                new double[] { -1 },
                new double[] {  1 },
                new double[] {  1 },
                new double[] { -1 }
            };

            Neuron.RandGenerator = new ThreadSafeRandom(0);
            ActivationNetwork network = new ActivationNetwork(
                   new BipolarSigmoidFunction(2), 2, 2, 1);

            var teacher = new LevenbergMarquardtLearning(network,
                false, JacobianMethod.ByBackpropagation);

            double error = 1.0;
            while (error > 1e-5)
                error = teacher.RunEpoch(input, output);

            for (int i = 0; i < input.Length; i++)
                Assert.AreEqual(network.Compute(input[i])[0], output[i][0], 0.1);
        }

        [TestMethod()]
        public void ConstructorTest()
        {
            // Four training samples of the xor function

            // two inputs (x and y)
            double[][] input = 
            {
                new double[] { -1, -1 },
                new double[] { -1,  1 },
                new double[] {  1, -1 },
                new double[] {  1,  1 }
            };

            // one output (z = x ^ y)
            double[][] output = 
            {
                new double[] { -1 },
                new double[] {  1 },
                new double[] {  1 },
                new double[] { -1 }
            };


            // create multi-layer neural network
            ActivationNetwork network = new ActivationNetwork(
                   new BipolarSigmoidFunction(2), // use a bipolar sigmoid activation function
                   2, // two inputs
                   3, // three hidden neurons
                   1  // one output neuron
                   );

            // create teacher
            LevenbergMarquardtLearning teacher = new LevenbergMarquardtLearning(
                network, // the neural network
                false,   // whether or not to use Bayesian regularization
                JacobianMethod.ByBackpropagation // Jacobian calculation method
                );


            // set learning rate and momentum
            teacher.LearningRate = 0.1f;

            // start the supervisioned learning
            for (int i = 0; i < 1000; i++)
            {
                double error = teacher.RunEpoch(input, output);
            }

            // If we reached here, the constructor test has passed.
        }



        [TestMethod()]
        public void JacobianByChainRuleTest()
        {
            // Network with one hidden layer: 2-2-1
            Accord.Math.Tools.SetupGenerator(0);

            double[][] input = 
            {
                new double[] { -1, -1 },
                new double[] { -1,  1 },
                new double[] {  1, -1 },
                new double[] {  1,  1 }
            };

            double[][] output =
            {
                new double[] { -1 },
                new double[] {  1 },
                new double[] {  1 },
                new double[] { -1 }
            };

            Neuron.RandGenerator = new ThreadSafeRandom(0);

            ActivationNetwork network = new ActivationNetwork(
                   new BipolarSigmoidFunction(2), 2, 2, 1);

            var teacher1 = new LevenbergMarquardtLearning(network,
                false, JacobianMethod.ByFiniteDifferences);

            var teacher2 = new LevenbergMarquardtLearning(network,
                false, JacobianMethod.ByBackpropagation);

            // Set lambda to lambda max so no iterations are performed
            teacher1.LearningRate = 1e30f;
            teacher2.LearningRate = 1e30f;

            teacher1.RunEpoch(input, output);
            teacher2.RunEpoch(input, output);

            PrivateObject privateTeacher1 = new PrivateObject(teacher1);
            PrivateObject privateTeacher2 = new PrivateObject(teacher2);

            var jacobian1 = (float[][])privateTeacher1.GetField("jacobian");
            var jacobian2 = (float[][])privateTeacher2.GetField("jacobian");

            Assert.AreEqual(jacobian1[0][0], -0.47895513745387097, 1e-6);
            Assert.AreEqual(jacobian1[0][1], -0.05863886707282373, 1e-6);
            Assert.AreEqual(jacobian1[0][2], 0.057751100929897485, 1e-6);
            Assert.AreEqual(jacobian1[0][3], 0.0015185010717608583, 1e-6);

            Assert.AreEqual(jacobian1[7][0], -0.185400783651892, 1e-6);
            Assert.AreEqual(jacobian1[7][1], 0.025575161626462877, 1e-6);
            Assert.AreEqual(jacobian1[7][2], 0.070494677797224889, 1e-6);
            Assert.AreEqual(jacobian1[7][3], 0.037740463822781616, 1e-6);


            Assert.AreEqual(jacobian2[0][0], -0.4789595904719437, 1e-6);
            Assert.AreEqual(jacobian2[0][1], -0.058636153936941729, 1e-6);
            Assert.AreEqual(jacobian2[0][2], 0.057748435491340212, 1e-6);
            Assert.AreEqual(jacobian2[0][3], 0.0015184453425611988, 1e-6);

            Assert.AreEqual(jacobian2[7][0], -0.1854008206574258, 1e-6);
            Assert.AreEqual(jacobian2[7][1], 0.025575150379247645, 1e-6);
            Assert.AreEqual(jacobian2[7][2], 0.070494269423259301, 1e-6);
            Assert.AreEqual(jacobian2[7][3], 0.037740117733922635, 1e-6);


            for (int i = 0; i < jacobian1.Length; i++)
            {
                for (int j = 0; j < jacobian1[i].Length; j++)
                {
                    double j1 = jacobian1[i][j];
                    double j2 = jacobian2[i][j];

                    Assert.AreEqual(j1, j2, 1e-4);

                    Assert.IsFalse(Double.IsNaN(j1));
                    Assert.IsFalse(Double.IsNaN(j2));
                }
            }

        }

        [TestMethod()]
        public void JacobianByChainRuleTest2()
        {
            // Network with two hidden layers: 2-4-3-1
            Accord.Math.Tools.SetupGenerator(0);

            double[][] input = 
            {
                new double[] {-1, -1 },
                new double[] {-1,  1 },
                new double[] { 1, -1 },
                new double[] { 1,  1 }
            };

            double[][] output =
            {
                new double[] {-1 },
                new double[] { 1 },
                new double[] { 1 },
                new double[] {-1 }
            };

            Neuron.RandGenerator = new ThreadSafeRandom(0);

            ActivationNetwork network = new ActivationNetwork(
                   new BipolarSigmoidFunction(2), 2, 4, 3, 1);

            var teacher1 = new LevenbergMarquardtLearning(network,
                false, JacobianMethod.ByFiniteDifferences);

            var teacher2 = new LevenbergMarquardtLearning(network,
                false, JacobianMethod.ByBackpropagation);

            // Set lambda to lambda max so no iterations are performed
            teacher1.LearningRate = 1e30f;
            teacher2.LearningRate = 1e30f;

            teacher1.RunEpoch(input, output);
            teacher2.RunEpoch(input, output);

            PrivateObject privateTeacher1 = new PrivateObject(teacher1);
            PrivateObject privateTeacher2 = new PrivateObject(teacher2);

            var jacobian1 = (float[][])privateTeacher1.GetField("jacobian");
            var jacobian2 = (float[][])privateTeacher2.GetField("jacobian");


            for (int i = 0; i < jacobian1.Length; i++)
            {
                for (int j = 0; j < jacobian1[i].Length; j++)
                {
                    double j1 = jacobian1[i][j];
                    double j2 = jacobian2[i][j];

                    Assert.AreEqual(j1, j2, 1e-4);

                    Assert.IsFalse(Double.IsNaN(j1));
                    Assert.IsFalse(Double.IsNaN(j2));
                }
            }

        }

        [TestMethod()]
        public void JacobianByChainRuleTest3()
        {
            // Network with 3 hidden layers: 2-4-3-4-1

            Accord.Math.Tools.SetupGenerator(0);

            double[][] input = 
            {
                new double[] {-1, -1 },
                new double[] {-1,  1 },
                new double[] { 1, -1 },
                new double[] { 1,  1 }
            };

            double[][] output =
            {
                new double[] {-1 },
                new double[] { 1 },
                new double[] { 1 },
                new double[] {-1 }
            };

            Neuron.RandGenerator = new ThreadSafeRandom(0);

            ActivationNetwork network = new ActivationNetwork(
                   new BipolarSigmoidFunction(2), 2, 4, 3, 4, 1);

            var teacher1 = new LevenbergMarquardtLearning(network,
                false, JacobianMethod.ByFiniteDifferences);

            var teacher2 = new LevenbergMarquardtLearning(network,
                false, JacobianMethod.ByBackpropagation);

            // Set lambda to lambda max so no iterations are performed
            teacher1.LearningRate = 1e30f;
            teacher2.LearningRate = 1e30f;

            teacher1.RunEpoch(input, output);
            teacher2.RunEpoch(input, output);

            PrivateObject privateTeacher1 = new PrivateObject(teacher1);
            PrivateObject privateTeacher2 = new PrivateObject(teacher2);

            var jacobian1 = (float[][])privateTeacher1.GetField("jacobian");
            var jacobian2 = (float[][])privateTeacher2.GetField("jacobian");


            for (int i = 0; i < jacobian1.Length; i++)
            {
                for (int j = 0; j < jacobian1[i].Length; j++)
                {
                    double j1 = jacobian1[i][j];
                    double j2 = jacobian2[i][j];

                    Assert.AreEqual(j1, j2, 1e-4);

                    Assert.IsFalse(Double.IsNaN(j1));
                    Assert.IsFalse(Double.IsNaN(j2));
                }
            }

        }

        [TestMethod()]
        public void JacobianByChainRuleTest4()
        {
            // Network with no hidden layers: 3-1

            double[][] input = 
            {
                new double[] {-1, -1 },
                new double[] {-1,  1 },
                new double[] { 1, -1 },
                new double[] { 1,  1 }
            };

            double[][] output =
            {
                new double[] {-1 },
                new double[] { 1 },
                new double[] { 1 },
                new double[] {-1 }
            };

            Neuron.RandGenerator = new ThreadSafeRandom(0);

            ActivationNetwork network = new ActivationNetwork(
                   new BipolarSigmoidFunction(2), 2, 1);

            var teacher1 = new LevenbergMarquardtLearning(network,
                false, JacobianMethod.ByFiniteDifferences);

            var teacher2 = new LevenbergMarquardtLearning(network,
                false, JacobianMethod.ByBackpropagation);

            // Set lambda to lambda max so no iterations are performed
            teacher1.LearningRate = 1e30f;
            teacher2.LearningRate = 1e30f;

            teacher1.RunEpoch(input, output);
            teacher2.RunEpoch(input, output);

            PrivateObject privateTeacher1 = new PrivateObject(teacher1);
            PrivateObject privateTeacher2 = new PrivateObject(teacher2);

            var jacobian1 = (float[][])privateTeacher1.GetField("jacobian");
            var jacobian2 = (float[][])privateTeacher2.GetField("jacobian");


            for (int i = 0; i < jacobian1.Length; i++)
            {
                for (int j = 0; j < jacobian1[i].Length; j++)
                {
                    double j1 = jacobian1[i][j];
                    double j2 = jacobian2[i][j];

                    Assert.AreEqual(j1, j2, 1e-5);

                    Assert.IsFalse(Double.IsNaN(j1));
                    Assert.IsFalse(Double.IsNaN(j2));
                }
            }
        }


        [TestMethod()]
        public void BlockHessianTest1()
        {
            // Network with no hidden layers: 3-1

            Accord.Math.Tools.SetupGenerator(0);

            double[][] input = 
            {
                new double[] {-1, -1 },
                new double[] {-1,  1 },
                new double[] { 1, -1 },
                new double[] { 1,  1 }
            };

            double[][] output =
            {
                new double[] {-1 },
                new double[] { 1 },
                new double[] { 1 },
                new double[] {-1 }
            };

            Neuron.RandGenerator = new ThreadSafeRandom(0);


            ActivationNetwork network = new ActivationNetwork(
                   new BipolarSigmoidFunction(2), 2, 1);

            var teacher1 = new LevenbergMarquardtLearning(network,
                false, JacobianMethod.ByFiniteDifferences);

            var teacher2 = new LevenbergMarquardtLearning(network,
                false, JacobianMethod.ByBackpropagation);
            teacher2.Blocks = 2;

            // Set lambda to lambda max so no iterations are performed
            teacher1.LearningRate = 1e30f;
            teacher2.LearningRate = 1e30f;

            teacher1.RunEpoch(input, output);
            teacher2.RunEpoch(input, output);

            var hessian1 = teacher1.Hessian;
            var hessian2 = teacher1.Hessian;

            for (int i = 0; i < hessian1.Length; i++)
            {
                for (int j = 0; j < hessian1[i].Length; j++)
                {
                    double j1 = hessian1[i][j];
                    double j2 = hessian2[i][j];

                    Assert.AreEqual(j1, j2, 1e-4);

                    Assert.IsFalse(Double.IsNaN(j1));
                    Assert.IsFalse(Double.IsNaN(j2));
                }
            }

            Assert.IsTrue(hessian1.IsUpperTriangular());
            Assert.IsTrue(hessian2.IsUpperTriangular());

            var gradient1 = teacher1.Gradient;
            var gradient2 = teacher2.Gradient;

            for (int i = 0; i < gradient1.Length; i++)
            {
                    double j1 = gradient1[i];
                    double j2 = gradient2[i];

                    Assert.AreEqual(j1, j2, 1e-5);

                    Assert.IsFalse(Double.IsNaN(j1));
                    Assert.IsFalse(Double.IsNaN(j2));
            }
        }

    }
}
