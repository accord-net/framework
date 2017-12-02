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

namespace Accord.Tests.Neuro
{
    using Accord.IO;
    using Accord.Math;
    using Accord.Neuro;
    using Accord.Neuro.Learning;
    using AForge;
    using NUnit.Framework;
    using System;
    using System.IO;

    [TestFixture]
    public class LevenbergMarquardtLearningTest
    {

        [Test]
        public void MulticlassTest1()
        {
            Accord.Math.Tools.SetupGenerator(0);
            // Neuron.RandGenerator = new ThreadSafeRandom(0);


            int numberOfInputs = 3;
            int numberOfClasses = 4;
            int hiddenNeurons = 5;

            double[][] input = 
            {
                new double[] { -1, -1, -1 }, // 0
                new double[] { -1,  1, -1 }, // 1
                new double[] {  1, -1, -1 }, // 1
                new double[] {  1,  1, -1 }, // 0
                new double[] { -1, -1,  1 }, // 2
                new double[] { -1,  1,  1 }, // 3
                new double[] {  1, -1,  1 }, // 3
                new double[] {  1,  1,  1 }  // 2
            };

            int[] labels =
            {
                0,
                1,
                1,
                0,
                2,
                3,
                3,
                2,
            };

            double[][] outputs = Accord.Statistics.Tools
                .Expand(labels, numberOfClasses, -1, 1);

            var function = new BipolarSigmoidFunction(2);
            var network = new ActivationNetwork(function,
                numberOfInputs, hiddenNeurons, numberOfClasses);

            new NguyenWidrow(network).Randomize();

            var teacher = new LevenbergMarquardtLearning(network);

            double error = Double.PositiveInfinity;
            for (int i = 0; i < 10; i++)
                error = teacher.RunEpoch(input, outputs);

            for (int i = 0; i < input.Length; i++)
            {
                int answer;
                double[] output = network.Compute(input[i]);
                double response = output.Max(out answer);

                int expected = labels[i];
                Assert.AreEqual(expected, answer);
            }
        }

        [Test]
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

            // Neuron.RandGenerator = new ThreadSafeRandom(0);

            ActivationNetwork network = new ActivationNetwork(
                   new BipolarSigmoidFunction(2), 2, 2, 1);

            var teacher = new LevenbergMarquardtLearning(network,
                false, JacobianMethod.ByFiniteDifferences);

            double error = 1.0;
            while (error > 2.6e-3)
                error = teacher.RunEpoch(input, output);

            for (int i = 0; i < input.Length; i++)
                Assert.AreEqual(network.Compute(input[i])[0], output[i][0], 0.1);
        }

        [Test]
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

            // Neuron.RandGenerator = new ThreadSafeRandom(0);
            ActivationNetwork network = new ActivationNetwork(
                   new BipolarSigmoidFunction(2), 2, 2, 1);

            var teacher = new LevenbergMarquardtLearning(network,
                false, JacobianMethod.ByBackpropagation);

            double error = 1.0;
            while (error > 6e-3)
                error = teacher.RunEpoch(input, output);

            for (int i = 0; i < input.Length; i++)
                Assert.AreEqual(network.Compute(input[i])[0], output[i][0], 0.1);
        }

        [Test]
        public void RunEpochTest3()
        {
            double[,] dataset = yinyang;

            double[][] input = dataset.GetColumns(new[] { 0, 1 }).ToJagged();
            double[][] output = dataset.GetColumn(2).ToJagged();

            // Neuron.RandGenerator = new ThreadSafeRandom(0);
            Accord.Math.Random.Generator.Seed = 0;

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

        [Test]
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

            // Neuron.RandGenerator = new ThreadSafeRandom(0);
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

        [Test]
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



        [Test]
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

            // Neuron.RandGenerator = new ThreadSafeRandom(0);

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

            var jacobian1 = teacher1.Jacobian;
            var jacobian2 = teacher2.Jacobian;



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

        [Test]
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

            // Neuron.RandGenerator = new ThreadSafeRandom(0);

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

            var jacobian1 = teacher1.Jacobian;
            var jacobian2 = teacher2.Jacobian;


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

        [Test]
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

            // Neuron.RandGenerator = new ThreadSafeRandom(0);

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

            var jacobian1 = teacher1.Jacobian;
            var jacobian2 = teacher2.Jacobian;


            for (int i = 0; i < jacobian1.Length; i++)
            {
                for (int j = 0; j < jacobian1[i].Length; j++)
                {
                    double j1 = jacobian1[i][j];
                    double j2 = jacobian2[i][j];

                    Assert.AreEqual(j1, j2, 1e-3);

                    Assert.IsFalse(Double.IsNaN(j1));
                    Assert.IsFalse(Double.IsNaN(j2));
                }
            }

        }

        [Test]
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

            // Neuron.RandGenerator = new ThreadSafeRandom(0);
            Accord.Math.Random.Generator.Seed = 0;

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

            var jacobian1 = teacher1.Jacobian;
            var jacobian2 = teacher2.Jacobian;


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

        [Test]
        public void JacobianByChainRuleTest_MultipleOutput()
        {
            // Network with no hidden layers: 3-4

            int numberOfInputs = 3;
            int numberOfClasses = 4;

            double[][] input = 
            {
                new double[] { -1, -1, -1 }, // 0
                new double[] { -1,  1, -1 }, // 1
                new double[] {  1, -1, -1 }, // 1
                new double[] {  1,  1, -1 }, // 0
                new double[] { -1, -1,  1 }, // 2
                new double[] { -1,  1,  1 }, // 3
                new double[] {  1, -1,  1 }, // 3
                new double[] {  1,  1,  1 }  // 2
            };

            int[] labels =
            {
                0,
                1,
                1,
                0,
                2,
                3,
                3,
                2,
            };

            double[][] output = Accord.Statistics.Tools
                .Expand(labels, numberOfClasses, -1, 1);

            // Neuron.RandGenerator = new ThreadSafeRandom(0);
            Accord.Math.Random.Generator.Seed = 0;

            ActivationNetwork network = new ActivationNetwork(
                   new BipolarSigmoidFunction(2), numberOfInputs, numberOfClasses);

            var teacher1 = new LevenbergMarquardtLearning(network,
                false, JacobianMethod.ByFiniteDifferences);

            var teacher2 = new LevenbergMarquardtLearning(network,
                false, JacobianMethod.ByBackpropagation);

            // Set lambda to lambda max so no iterations are performed
            teacher1.LearningRate = 1e30f;
            teacher2.LearningRate = 1e30f;

            teacher1.RunEpoch(input, output);
            teacher2.RunEpoch(input, output);

            var jacobian1 = teacher1.Jacobian;
            var jacobian2 = teacher2.Jacobian;


            for (int i = 0; i < jacobian1.Length; i++)
            {
                for (int j = 0; j < jacobian1[i].Length; j++)
                {
                    double j1 = jacobian1[i][j];
                    double j2 = jacobian2[i][j];

                    Assert.AreEqual(j1, j2, 1e-3);

                    Assert.IsFalse(Double.IsNaN(j1));
                    Assert.IsFalse(Double.IsNaN(j2));
                }
            }
        }


        [Test]
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


        [Test]
        public void ZeroLambdaTest()
        {
            Accord.Math.Random.Generator.Seed = 0;
            double[,] data = null;

            // open selected file
            using (TextReader stream = new StringReader(Properties.Resources.ZeroLambda))
            using (CsvReader reader = new CsvReader(stream, false))
            {
                data = reader.ToTable().ToMatrix(System.Globalization.CultureInfo.InvariantCulture);
            }

            // number of learning samples
            int samples = data.GetLength(0);

            var ranges = data.GetRange(dimension: 0);

            Assert.AreEqual(2, ranges.Length);

            var rangeX = ranges[0];
            var rangeY = ranges[1];

            // data transformation factor
            double yFactor = 1.7 / rangeY.Length;
            double yMin = rangeY.Min;
            double xFactor = 2.0 / rangeX.Length;
            double xMin = rangeX.Min;

            // prepare learning data
            double[][] input = new double[samples][];
            double[][] output = new double[samples][];

            for (int i = 0; i < samples; i++)
            {
                input[i] = new double[1];
                output[i] = new double[1];

                input[i][0] = (data[i, 0] - xMin) * xFactor - 1.0; // set input
                output[i][0] = (data[i, 1] - yMin) * yFactor - 0.85; // set output
            }

            // Neuron.RandGenerator = new ThreadSafeRandom(0);
            Accord.Math.Random.Generator.Seed = 0;

            // create multi-layer neural network
            var network = new ActivationNetwork(
                new BipolarSigmoidFunction(5),
                1, 12, 1);

            // create teacher
            var teacher = new LevenbergMarquardtLearning(network, true);
#if MONO
            teacher.ParallelOptions.MaxDegreeOfParallelism = 1;
#endif
            teacher.LearningRate = 1;

            // iterations
            int iteration = 1;
            int iterations = 2000;

            // solution array
            double[,] solution = new double[samples, 2];
            double[] networkInput = new double[1];

            bool needToStop = false;

            double learningError = 0;

            // loop
            while (!needToStop)
            {
                Assert.AreNotEqual(0, teacher.LearningRate);

                // run epoch of learning procedure
                double error = teacher.RunEpoch(input, output) / samples;

                // calculate solution
                for (int j = 0; j < samples; j++)
                {
                    networkInput[0] = (solution[j, 0] - xMin) * xFactor - 1.0;
                    solution[j, 1] = (network.Compute(networkInput)[0] + 0.85) / yFactor + yMin;
                }


                // calculate error
                learningError = 0.0;
                for (int j = 0; j < samples; j++)
                {
                    networkInput[0] = input[j][0];
                    learningError += Math.Abs(data[j, 1] - ((network.Compute(networkInput)[0] + 0.85) / yFactor + yMin));
                }

                // increase current iteration
                iteration++;

                // check if we need to stop
                if ((iterations != 0) && (iteration > iterations))
                    break;
            }

            Assert.IsTrue(learningError < 0.13);
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
