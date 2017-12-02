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
    using Accord.MachineLearning;
    using Accord.Math;
    using Accord.Math.Distances;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Tests.MachineLearning.Structures;
    using NUnit.Framework;
    using System.Diagnostics;

    [TestFixture]
    public class KnnPerformanceTest
    {


        [Test]
        public void PerformanceTest1()
        {
            Accord.Math.Random.Generator.Seed = 0;

            int n1 = 1000;
            int n2 = 2000;
            int k = 5;

            double[][] inputs;
            NaiveKNearestNeighbors naive;
            KNearestNeighbors<double[]> normal;
            KNearestNeighbors target;
            Create(n1, n2, k, out inputs, out naive, out normal, out target);

            double[] expected = new double[inputs.Length];
            double[] actual1 = new double[inputs.Length];
            double[] actual2 = new double[inputs.Length];

            Stopwatch sw = new Stopwatch();

            sw.Start();
            for (int i = 0; i < inputs.Length; i++)
                expected[i] = naive.Compute(inputs[i]);
            sw.Stop();
            var t1 = sw.Elapsed;

            sw.Restart();
            for (int i = 0; i < inputs.Length; i++)
                actual1[i] = normal.Compute(inputs[i]);
            sw.Stop();
            var t2 = sw.Elapsed;

            sw.Restart();
            for (int i = 0; i < inputs.Length; i++)
                actual2[i] = target.Compute(inputs[i]);
            sw.Stop();
            var t3 = sw.Elapsed;

            //Assert.IsTrue(t1 > t2);
            Assert.IsTrue(t2 > t3);

            //Assert.IsTrue(t2.Ticks > t3.Ticks * 10);


            for (int i = 0; i < inputs.Length; i++)
            {
                Assert.AreEqual(expected[i], actual1[i]);
                Assert.AreEqual(expected[i], actual2[i]);
            }
        }

        [Test]
        [Category("Slow")]
        public void AccuracyTest1()
        {
            int n1 = 5000;
            int n2 = 3000;
            int k = 15;

            double[][] inputs;
            NaiveKNearestNeighbors naive;
            KNearestNeighbors<double[]> normal;
            KNearestNeighbors target;
            Create(n1, n2, k, out inputs, out naive, out normal, out target);


            for (int i = 0; i < inputs.Length; i++)
            {
                double[][] expectedNN;
                int[] expectedLabels;

                expectedNN = naive.GetNearestNeighbors(inputs[i], out expectedLabels);

                double[][] normalNN;
                int[] normalLabels;

                normalNN = normal.GetNearestNeighbors(inputs[i], out normalLabels);

                double[][] targetNN;
                int[] targetLabels;

                targetNN = target.GetNearestNeighbors(inputs[i], out targetLabels);

                Assert.AreEqual(expectedLabels.Length, normalNN.Length);
                Assert.AreEqual(expectedNN.Length, normalNN.Length);
                Assert.AreEqual(expectedNN.Length, targetNN.Length);

                for (int j = 0; j < expectedNN.Length; j++)
                {
                    int ni = normalNN.IndexOf(expectedNN[j]);
                    Assert.AreEqual(expectedNN[j], normalNN[ni]);
                    Assert.AreEqual(expectedLabels[j], normalLabels[ni]);

                    int ti = targetNN.IndexOf(expectedNN[j]);
                    Assert.AreEqual(expectedNN[j], targetNN[ti]);
                    Assert.AreEqual(expectedLabels[j], targetLabels[ti]);
                }
            }
        }

        private static void Create(int n1, int n2, int k, out double[][] inputs, out NaiveKNearestNeighbors naive, out KNearestNeighbors<double[]> normal, out KNearestNeighbors target)
        {
            int n = n1 + n2;

            double[][] gauss1 = MultivariateNormalDistribution.Generate(n1,
                mean: new double[] { 2, 1 },
                covariance: new double[,] 
                {
                    { 1, 0 },
                    { 0, 1 },
                });

            double[][] gauss2 = MultivariateNormalDistribution.Generate(n2,
                mean: new double[] { -1, 4 },
                covariance: new double[,] 
                {
                    { 2, 1 },
                    { 0, 3 },
                });

            inputs = gauss1.Stack(gauss2);
            int[] outputs = Matrix.Vector(n1, 0).Concatenate(Matrix.Vector(n2, +1));

            var idx = Vector.Sample(n1 + n2);
            inputs = inputs.Submatrix(idx);
            outputs = outputs.Submatrix(idx);

            naive = new NaiveKNearestNeighbors(k, inputs, outputs);
            normal = new KNearestNeighbors<double[]>(k, inputs, outputs, new Euclidean());
            Assert.AreEqual(2, normal.NumberOfInputs);
            Assert.AreEqual(2, normal.NumberOfOutputs);
            Assert.AreEqual(2, normal.NumberOfClasses);

            target = new KNearestNeighbors(k, inputs, outputs);
            Assert.AreEqual(2, target.NumberOfInputs);
            Assert.AreEqual(2, target.NumberOfOutputs);
            Assert.AreEqual(2, target.NumberOfClasses);
        }


    }
}
