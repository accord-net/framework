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
#if NET35
    extern alias core;
    using ParallelOptions = core::System.Threading.Tasks.ParallelOptions;
#endif

    using Accord;
    using Accord.IO;
    using Accord.MachineLearning;
    using Accord.Math;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Tests.MachineLearning.Properties;
    using NUnit.Framework;
    using System;
    using System.Data;
    using System.IO;
    using System.Threading.Tasks;

    [TestFixture]
    public class GaussianMixtureModelTest
    {


        [Test]
        public void GaussianMixtureModelConstructorTest()
        {
            Accord.Math.Tools.SetupGenerator(0);

            // Test Samples
            double[][] samples =
            {
                new double[] { 0, 1 },
                new double[] { 1, 2 }, 
                new double[] { 1, 1 },
                new double[] { 0, 7 },
                new double[] { 1, 1 },
                new double[] { 6, 2 },
                new double[] { 6, 5 },
                new double[] { 5, 1 },
                new double[] { 7, 1 },
                new double[] { 5, 1 }
            };

            double[] sample = samples[0];


            // Create a new Gaussian Mixture Model with 2 components
            GaussianMixtureModel gmm = new GaussianMixtureModel(2);

            // Compute the model (estimate)
            double ll = gmm.Compute(samples, 0.0001);
            Assert.AreEqual(-35.930732550698494, ll, 1e-10);

            Assert.AreEqual(2, gmm.Gaussians.Count);

            Assert.IsTrue(gmm.Gaussians.Means[0].IsEqual(new[] { 5.8, 2.0 }, 1e-3));
            Assert.IsTrue(gmm.Gaussians.Means[1].IsEqual(new[] { 0.6, 2.4 }, 1e-3));


            int[] c = samples.Apply(gmm.Clusters.Nearest);

            for (int i = 0; i < samples.Length; i++)
            {
                double[] responses;
                int e = gmm.Gaussians.Nearest(samples[i], out responses);
                int a = responses.ArgMax();

                Assert.AreEqual(a, e);
                Assert.AreEqual(c[i], (i < 5) ? 1 : 0);
            }
        }

        [Test]
        public void GaussianMixtureModelTest2()
        {
            Accord.Math.Random.Generator.Seed = 0;

            int height = 16;
            int width = 16;

            var gmm = new GaussianMixtureModel(3);
            // gmm.Regularization = 0;

            Assert.AreEqual(3, gmm.Gaussians.Count);
            Assert.IsNull(gmm.Gaussians[0].Covariance);
            Assert.IsNull(gmm.Gaussians[0].Mean);


            double[][][] A = new double[3][][];
            A[0] = new double[height][];
            A[1] = new double[height][];
            A[2] = new double[height][];

            for (int j = 0; j < height; j++)
            {
                A[0][j] = new double[width];
                A[1][j] = new double[width];
                A[2][j] = new double[width];

                for (int k = 0; k < width; k++)
                {
                    A[0][j][k] = 102;
                    A[1][j][k] = 57;
                    A[2][j][k] = 200;
                }
            }

            double[][] B = Matrix.Stack(A);

            bool thrown = false;
            try
            {
                var result = gmm.Compute(B);
            }
            catch (NonPositiveDefiniteMatrixException)
            {
                thrown = true;
            }

            Assert.IsTrue(thrown);
        }

        [Test]
        public void GaussianMixtureModelTest3()
        {
            Accord.Math.Tools.SetupGenerator(0);

            var gmm = new GaussianMixtureModel(3);
            Assert.AreEqual(3, gmm.Gaussians.Count);
            Assert.IsNull(gmm.Gaussians[0].Covariance);
            Assert.IsNull(gmm.Gaussians[0].Mean);


            double[][] B = Matrix.Random(56, 12).ToArray();

            Accord.Math.Tools.SetupGenerator(0);

            gmm.Options.Robust = true;
            var result = gmm.Compute(B, new GaussianMixtureModelOptions()
                {
                    NormalOptions = new NormalOptions
                    {
                        Robust = true
                    }
                });
        }

        [Test]
        public void GaussianMixtureModelTest7()
        {
            double[][] values =
            {
                new double[] {0},
                new double[] {1},
                new double[] {2},
                new double[] {3},
                new double[] {4},
                new double[] {5},
                new double[] {6},
                new double[] {7},
                new double[] {8},
                new double[] {9},
                new double[] {10},
                new double[] {11},
                new double[] {12}
            };

            double[] weights =
            {
                1, 3, 5, 4, 3, 5, 10, 17, 12, 6, 3, 1, 0
            };

            GaussianMixtureModel gmm = new GaussianMixtureModel(2);
            gmm.Compute(values, new GaussianMixtureModelOptions()
            {
                Weights = weights
            });

            int[] classifications = gmm.Gaussians.Nearest(values);
        }


        [Test]
        public void GaussianMixtureModelTest4()
        {

            // Suppose we have a weighted data set. Those are the input points:
            double[][] points =
            {
                new double[] { 0 }, new double[] { 3 }, new double[] {  1 }, 
                new double[] { 7 }, new double[] { 3 }, new double[] {  5 },
                new double[] { 1 }, new double[] { 2 }, new double[] { -1 },
                new double[] { 2 }, new double[] { 7 }, new double[] {  6 }, 
                new double[] { 8 }, new double[] { 6 } // (14 points)
            };

            // And those are their respective unnormalized weights:
            double[] weights = { 10, 1, 1, 2, 2, 1, 1, 1, 8, 1, 2, 5, 1, 1 }; // (14 weights)

            // with weights
            {
                Accord.Math.Tools.SetupGenerator(0);

                // If we need the GaussianMixtureModel functionality, we can
                // use the estimated mixture to initialize a new model:
                GaussianMixtureModel gmm = new GaussianMixtureModel(2);
                gmm.Initializations = 1;

                var options = new GaussianMixtureModelOptions();
                options.Weights = weights;
                options.ParallelOptions.MaxDegreeOfParallelism = 1;
                gmm.Compute(points, options);


                int a = 1;
                int b = 0;

                if (!gmm.Gaussians[a].Mean[0].IsRelativelyEqual(6.41922, 1e-4))
                {
                    var t = a;
                    a = b;
                    b = t;
                }

                Assert.AreEqual(6.4192285647145395, gmm.Gaussians[a].Mean[0], 1e-10);
                Assert.AreEqual(0.2888226129013588, gmm.Gaussians[b].Mean[0], 1e-10);

                Assert.AreEqual(0.32321638614859777, gmm.Gaussians[a].Proportion, 1e-6);
                Assert.AreEqual(0.67678361385140218, gmm.Gaussians[b].Proportion, 1e-6);
                Assert.AreEqual(1, gmm.Gaussians[0].Proportion + gmm.Gaussians[1].Proportion);
            }

            // with weights, new style
            {
                Accord.Math.Tools.SetupGenerator(0);

                // If we need the GaussianMixtureModel functionality, we can
                // use the estimated mixture to initialize a new model:
                GaussianMixtureModel gmm = new GaussianMixtureModel(2);
                gmm.Initializations = 1;

                gmm.UseLogarithm = false;
                gmm.ParallelOptions.MaxDegreeOfParallelism = 1;
                gmm.Compute(points, weights);


                int a = 1;
                int b = 0;

                if (!gmm.Gaussians[a].Mean[0].IsRelativelyEqual(6.41922, 1e-4))
                {
                    var t = a;
                    a = b;
                    b = t;
                }

                Assert.AreEqual(6.4192285647145395, gmm.Gaussians[a].Mean[0], 1e-10);
                Assert.AreEqual(0.2888226129013588, gmm.Gaussians[b].Mean[0], 1e-10);

                Assert.AreEqual(0.32321638614859777, gmm.Gaussians[a].Proportion, 1e-6);
                Assert.AreEqual(0.67678361385140218, gmm.Gaussians[b].Proportion, 1e-6);
                Assert.AreEqual(1, gmm.Gaussians[0].Proportion + gmm.Gaussians[1].Proportion);
            }

            // without weights
            {
                Accord.Math.Random.Generator.Seed = 0;

                // If we need the GaussianMixtureModel functionality, we can
                // use the estimated mixture to initialize a new model:
                GaussianMixtureModel gmm = new GaussianMixtureModel(2);
                gmm.Initializations = 1;

                var options = new GaussianMixtureModelOptions();
                options.ParallelOptions.MaxDegreeOfParallelism = 1;
                gmm.Compute(points, options);

                int a = 1;
                int b = 0;

                double tol = 1e-2;

                if (!6.5149525060859848.IsRelativelyEqual(gmm.Gaussians[a].Mean[0], tol))
                {
                    var t = a;
                    a = b;
                    b = t;
                }

                Assert.AreEqual(6.5149525060859848, gmm.Gaussians[a].Mean[0], tol);
                Assert.AreEqual(1.4191977895308987, gmm.Gaussians[b].Mean[0], tol);

                Assert.AreEqual(0.4195042394315267, gmm.Gaussians[a].Proportion, tol);
                Assert.AreEqual(0.58049576056847307, gmm.Gaussians[b].Proportion, tol);
                Assert.AreEqual(1, gmm.Gaussians[0].Proportion + gmm.Gaussians[1].Proportion, 1e-8);
            }
        }

        [Test]
        public void GaussianMixtureModelTest5()
        {
            Accord.Math.Tools.SetupGenerator(0);

            MemoryStream stream = new MemoryStream(Resources.CircleWithWeights);
            ExcelReader reader = new ExcelReader(stream, xlsx: false);

            DataTable table = reader.GetWorksheet("Sheet1");

            double[,] matrix = table.ToMatrix();

            double[][] points = matrix.Submatrix(null, 0, 1).ToArray();
            double[] weights = matrix.GetColumn(2);

            GaussianMixtureModel gmm = new GaussianMixtureModel(2);
            gmm.Initializations = 1;

            gmm.Compute(points, new GaussianMixtureModelOptions()
            {
                Weights = weights
            });

            int a = 0;
            int b = 1;

            if ((-0.407859903454185).IsRelativelyEqual(gmm.Gaussians[1].Mean[0], 1e-4))
            {
                a = 1;
                b = 0;
            }

            Assert.AreEqual(-0.407859903454185, gmm.Gaussians[a].Mean[0], 1e-4);
            Assert.AreEqual(-0.053911705279706859, gmm.Gaussians[a].Mean[1], 1e-3);

            Assert.AreEqual(0.39380877640250328, gmm.Gaussians[b].Mean[0], 1e-4);
            Assert.AreEqual(0.047186154880776772, gmm.Gaussians[b].Mean[1], 1e-4);

            Assert.AreEqual(1, gmm.Gaussians[0].Proportion + gmm.Gaussians[1].Proportion, 1e-15);

            Assert.IsFalse(gmm.Gaussians[0].Mean.HasNaN());
            Assert.IsFalse(gmm.Gaussians[1].Mean.HasNaN());
        }



        [Test]
        public void HighDimensionalTest()
        {
            for (int i = 0; i < 100; i++)
            {
                for (int j = 10; j < 25; j++)
                {
                    double[,] covariance = Accord.Statistics.Tools.RandomCovariance(j, -3.0, 3.0);

                    bool isPositiveDefinite = covariance.IsPositiveDefinite();

                    Assert.IsTrue(isPositiveDefinite);
                }
            }
        }

        [Test, Ignore]
        public void LargeSampleTest()
        {
            Accord.Math.Tools.SetupGenerator(0);

            Func<double> r = () => Tools.Random.NextDouble();
            Func<double> b = () => Tools.Random.NextDouble() > 0.3 ? 1 : -1;

            // Test Samples
            int thousand = 1000;
            int million = thousand * thousand;
            double[][] samples = new double[5 * million][];

            Func<int, double[]> expand = (int label) =>
                {
                    var d = new double[10];
                    d[label] = 1;
                    return d;
                };

            for (int j = 0; j < samples.Length; j++)
            {
                if (j % 10 > 8)
                    samples[j] = new double[] { r() }.Concatenate(expand(Tools.Random.Next() % 10));
                else samples[j] = new double[] { r() * j }.Concatenate(expand(j % 10));
            }


            // Create a new Gaussian Mixture Model with 2 components
            GaussianMixtureModel gmm = new GaussianMixtureModel(5);

            // Compute the model
            double result = gmm.Compute(samples, new GaussianMixtureModelOptions()
            {
                NormalOptions = new NormalOptions()
                {
                    Regularization = 1e-5,
                }
            });


            for (int i = 0; i < samples.Length; i++)
            {
                var sample = samples[i];
                int c = gmm.Gaussians.Nearest(sample);

                Assert.AreEqual(c, (i % 10) >= 5 ? 1 : 0);
            }
        }
    }
}
