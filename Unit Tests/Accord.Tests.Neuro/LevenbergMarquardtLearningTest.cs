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
        public void RunEpochTest3()
        {
            Accord.Math.Tools.SetupGenerator(0);

            double[,] dataset = yinyang;

            double[][] input = dataset.GetColumns(0, 1).ToArray();
            double[][] output = dataset.GetColumn(2).ToArray();

            Neuron.RandGenerator = new ThreadSafeRandom(0);

            ActivationNetwork network = new ActivationNetwork(
                   new BipolarSigmoidFunction(2), 2, 5, 1);

            var teacher = new LevenbergMarquardtLearning(network,
                true, JacobianMethod.ByBackpropagation);

            Assert.IsTrue(teacher.UseRegularization);

            double error = 1.0;
            for (int i = 0; i < 500; i++)
                error = teacher.RunEpoch(input, output);

            double[][] actual = new double[output.Length][];

            for (int i = 0; i < input.Length; i++)
                actual[i] = network.Compute(input[i]);

            for (int i = 0; i < input.Length; i++)
                Assert.AreEqual(Math.Sign(output[i][0]), Math.Sign(actual[i][0]));
        }

        [TestMethod()]
        public void RunEpochTest4()
        {
            Accord.Math.Tools.SetupGenerator(0);

            double[][] input = 
            {
                new double[] { 0, 0 },
            };

            double[][] output =
            {
                new double[] { 0 },
            };

            Neuron.RandGenerator = new ThreadSafeRandom(0);
            ActivationNetwork network = new ActivationNetwork(
                   new BipolarSigmoidFunction(2), 2, 1);

            var teacher = new LevenbergMarquardtLearning(network,
                true, JacobianMethod.ByBackpropagation);

            double error = 1.0;
            for (int i = 0; i < 1000; i++)
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




        public static double[,] yinyang =
        {
            #region Yin Yang
            { -0.876847428, 1.996318824, -1 },
            { -0.748759325, 1.997248514, -1 },
            { -0.635574695, 1.978046579, -1 },
            { -0.513769071, 1.973224777, -1 },
            { -0.382577547, 1.955077224, -1 },
            { -0.275144211, 1.923813789, -1 },
            { -0.156802752, 1.949219695, -1 },
            { -0.046002059, 1.895342542, -1 },
            { 0.084152257, 1.873104082, -1 },
            { 0.192063131, 1.868157532, -1 },
            { 0.238547032, 1.811664165, -1 },
            { 0.381412694, 1.830869925, -1 },
            { 0.431182119, 1.755312479, -1 },
            { 0.562589082, 1.725444806, -1 },
            { 0.553294269, 1.689047886, -1 },
            { 0.730976261, 1.610522064, -1 },
            { 0.722164981, 1.633112952, -1 },
            { 0.861069302, 1.562450197, -1 },
            { 0.825107945, 1.435846225, -1 },
            { 0.825261132, 1.456391196, -1 },
            { 0.948721626, 1.393367552, -1 },
            { 1.001705278, 1.275768447, -1 },
            { 0.966788667, 1.321375233, -1 },
            { 1.030828944, 1.228437023, -1 },
            { 1.083195636, 1.143011589, -1 },
            { 0.920876422, 1.037854388, -1 },
            { 0.994518277, 1.064971023, -1 },
            { 0.954169422, 0.938084211, -1 },
            { 0.903586083, 0.985255341, -1 },
            { 0.877869854, 0.729143525, -1 },
            { 0.866594018, 0.75025734, -1 },
            { 0.757278389, 0.638917822, -1 },
            { 0.655489515, 0.670717406, -1 },
            { 0.687639626, 0.511655563, -1 },
            { 0.656365078, 0.638542346, -1 },
            { 0.491775914, 0.401874802, -1 },
            { 0.35504489, 0.38963967, -1 },
            { 0.275616568, 0.182958126, -1 },
            { 0.338471037, 0.102347682, -1 },
            { 0.103918095, 0.152960961, -1 },
            { 0.238473941, -0.070899965, -1 },
            { -0.00657754, 0.168107931, -1 },
            { -0.091307058, -0.032174399, -1 },
            { -0.290772034, -0.345025689, -1 },
            { -0.287555253, -0.397984323, -1 },
            { -0.363424618, -0.365636808, -1 },
            { -0.544071691, -0.512970644, -1 },
            { -0.7098968, -0.54654921, -1 },
            { -1.007857216, -0.811837224, -1 },
            { -0.932787122, -0.687973276, -1 },
            { -0.123987649, -1.547976483, 1 },
            { -0.247236701, -1.546629461, 1 },
            { -0.369357682, -1.533968755, 1 },
            { -0.497892178, -1.525597952, 1 },
            { -0.606998699, -1.518386229, 1 },
            { -0.751556976, -1.46427032, 1 },
            { -0.858848619, -1.464142289, 1 },
            { -0.957834238, -1.454165888, 1 },
            { -1.061602698, -1.444783216, 1 },
            { -1.169634343, -1.426033507, 1 },
            { -1.272115895, -1.408678817, 1 },
            { -1.380383293, -1.345651442, 1 },
            { -1.480866574, -1.279955202, 1 },
            { -1.548927664, -1.223262541, 1 },
            { -1.597886819, -1.227115936, 1 },
            { -1.686711497, -1.141898276, 1 },
            { -1.812689051, -1.14805053, 1 },
            { -1.809841336, -1.083347602, 1 },
            { -1.938850711, -1.019723742, 1 },
            { -1.974552679, -0.970515422, 1 },
            { -1.953184359, -0.88363121, 1 },
            { -1.98749965, -0.861879772, 1 },
            { -2.04215554, -0.797813815, 1 },
            { -1.984185734, -0.826986835, 1 },
            { -2.063307605, -0.749495213, 1 },
            { -1.964274134, -0.653639779, 1 },
            { -2.020258155, -0.530431615, 1 },
            { -1.946081996, -0.514425683, 1 },
            { -1.934356006, -0.435380423, 1 },
            { -1.827017658, -0.425058004, 1 },
            { -1.788385889, -0.312443513, 1 },
            { -1.800874033, -0.237312969, 1 },
            { -1.784225126, 0.013987951, 1 },
            { -1.682828321, -0.063911465, 1 },
            { -1.754042471, -0.075520653, 1 },
            { -1.5680733, 0.110795036, 1 },
            { -1.438333268, 0.170230561, 1 },
            { -1.356614661, 0.163613841, 1 },
            { -1.336362397, 0.334537756, 1 },
            { -1.296677607, 0.316006907, 1 },
            { -1.109908857, 0.474036646, 1 },
            { -0.845929174, 0.485303884, 1 },
            { -0.855794711, 0.395603118, 1 },
            { -0.68479255, 0.671166245, 1 },
            { -0.514222252, 0.652065554, 1 },
            { -0.387612557, 0.700858902, 1 },
            { -0.51939719, 1.025735335, 1 },
            { -0.228760025, 0.93490314, 1 },
            { -0.293782477, 1.008861678, 1 },
            { 0.013431012, 1.082021525, 1 },
            #endregion
        };
    }
}
