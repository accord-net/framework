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
    using System;
    using Accord.MachineLearning.VectorMachines;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Math;
    using Accord.Statistics.Analysis;
    using Accord.Statistics.Kernels;
    using NUnit.Framework;
    using System.IO;
    using System.Text;
    using Accord.Tests.MachineLearning.Properties;
    using Accord.IO;
    using Accord.Statistics.Kernels.Sparse;

    [TestFixture]
    public class LinearCoordinateDescentTest
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
            var learn = new LinearCoordinateDescent(machine, augmented, xor);

            // Run the learning algorithm
            learn.Run();

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

            var kernel = new Polynomial(2, 1);

            Accord.Math.Tools.SetupGenerator(0);
            var projection = inputs.Apply(kernel.Transform);
            var machine = new SupportVectorMachine(projection[0].Length);
            var smo = new LinearCoordinateDescent(machine, projection, labels)
            {
                Complexity = 1000000,
                Tolerance = 1e-15
            };

            double error = smo.Run();

            Assert.AreEqual(1000000.0, smo.Complexity, 1e-15);

            int[] actual = new int[labels.Length];
            for (int i = 0; i < actual.Length; i++)
                actual[i] = Math.Sign(machine.Compute(projection[i]));

            ConfusionMatrix matrix = new ConfusionMatrix(actual, labels);
            Assert.AreEqual(6, matrix.FalseNegatives);
            Assert.AreEqual(7, matrix.FalsePositives);
            Assert.AreEqual(44, matrix.TruePositives);
            Assert.AreEqual(43, matrix.TrueNegatives);
        }

        // TODO: Re-enable this test
        //[Test]
        //public void SparseLinearTest()
        //{
        //    MemoryStream file = new MemoryStream(
        //        Encoding.Default.GetBytes(Resources.iris_scale));

        //    // Create a new Sparse Sample Reader to read any given file,
        //    //  passing the correct dense sample size in the constructor
        //    SparseReader reader = new SparseReader(file, Encoding.Default);

        //    var samples = reader.ReadSparseToEnd();

        //    Sparse<double>[] x = samples.Item1;
        //    int[] y = samples.Item2.ToInt32();

        //    var learner = new LinearCoordinateDescent<Linear, Sparse<double>>();
        //    SupportVectorMachine<Linear, Sparse<double>> svm = learner.Learn(x, y);

        //}
    }
}
