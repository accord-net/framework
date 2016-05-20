// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
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
    using Accord.IO;
    using Accord.MachineLearning.VectorMachines;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Math;
    using Accord.Statistics;
    using Accord.Statistics.Analysis;
    using Accord.Statistics.Kernels;
    using Accord.Tests.MachineLearning.Properties;
    using NUnit.Framework;
    using System;
    using System.IO;
    using System.Text;

    [TestFixture]
    public class LinearDualCoordinateDescentTest
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
            var learn = new LinearDualCoordinateDescent(machine, augmented, xor);

            // Run the learning algorithm
            double error = learn.Run();

            Assert.AreEqual(0, error);

            int[] output = augmented.Apply(p => Math.Sign(machine.Compute(p)));
            for (int i = 0; i < output.Length; i++)
                Assert.AreEqual(System.Math.Sign(xor[i]), System.Math.Sign(output[i]));
        }

        [Test]
        public void ComputeTest5()
        {
            var dataset = SequentialMinimalOptimizationTest.yinyang;

            double[][] inputs = dataset.Submatrix(null, 0, 1).ToJagged();
            int[] labels = dataset.GetColumn(2).ToInt32();

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
                var smo = new LinearDualCoordinateDescent(machine, projection, labels);
                smo.UseComplexityHeuristic = true;
                smo.Tolerance = 0.01;

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

            {
                Accord.Math.Random.Generator.Seed = 0;
                var projection = inputs.Apply(kernel.Transform);
                var machine = new SupportVectorMachine(projection[0].Length);
                var smo = new LinearDualCoordinateDescent(machine, projection, labels);
                smo.UseComplexityHeuristic = true;
                smo.Loss = Loss.L1;

                double error = smo.Run();

                Assert.AreEqual(0.2, error);

                Assert.AreEqual(0.11714451552090821, smo.Complexity, 1e-15);

                int[] actual = new int[labels.Length];
                for (int i = 0; i < actual.Length; i++)
                    actual[i] = Math.Sign(machine.Compute(kernel.Transform(inputs[i])));

                ConfusionMatrix matrix = new ConfusionMatrix(actual, labels);
                Assert.AreEqual(20, matrix.FalseNegatives);
                Assert.AreEqual(0, matrix.FalsePositives);
                Assert.AreEqual(30, matrix.TruePositives);
                Assert.AreEqual(50, matrix.TrueNegatives);
            }
        }


        [Test]
        public void SparseLinearTest()
        {

            MulticlassSupportVectorMachine<Linear> svm1;
            MulticlassSupportVectorMachine<Linear, Sparse<double>> svm2;

            {
                Accord.Math.Random.Generator.Seed = 0;
                MemoryStream file = new MemoryStream(
                    Encoding.Default.GetBytes(Resources.iris_scale));
                var reader = new SparseReader(file, Encoding.Default);

                var samples = reader.ReadDenseToEnd();
                double[][] x = samples.Item1;
                int[] y = samples.Item2.ToMulticlass();

                var learner = new MulticlassSupportVectorLearning<Linear>()
                {
                    Learner = (p) => new LinearDualCoordinateDescent<Linear>()
                };

                svm1 = learner.Learn(x, y);
            }

            {
                Accord.Math.Random.Generator.Seed = 0;
                MemoryStream file = new MemoryStream(
                    Encoding.Default.GetBytes(Resources.iris_scale));

                // Create a new Sparse Sample Reader to read any given file,
                //  passing the correct dense sample size in the constructor
                var reader = new SparseReader(file, Encoding.Default);

                var samples = reader.ReadSparseToEnd();
                Sparse<double>[] x = samples.Item1;
                int[] y = samples.Item2.ToMulticlass();

                var learner = new MulticlassSupportVectorLearning<Linear, Sparse<double>>()
                {
                    Learner = (p) => new LinearDualCoordinateDescent<Linear, Sparse<double>>()
                };

                svm2 = learner.Learn(x, y);
            }

            Assert.AreEqual(svm1.Models.Length, svm2.Models.Length);
            for (int i = 0; i < svm1.Models.Length; i++)
            {
                var ma = svm1[i].Value;
                var mb = svm2[i].Value;

                Assert.IsTrue(ma.Weights.IsEqual(mb.Weights));
                Assert.AreEqual(ma.SupportVectors.Length, mb.SupportVectors.Length);
                for (int j = 0; j < ma.SupportVectors.Length; j++)
                {
                    double[] expected = ma.SupportVectors[j];
                    double[] actual = mb.SupportVectors[j].ToDense(4);
                    Assert.IsTrue(expected.IsEqual(actual));
                }
            }
        }
    }
}
