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

namespace Accord.Tests.Math
{
    using Accord.Math;
    using NUnit.Framework;
    using System;
    using Accord.Math.Decompositions;
    using Accord.Math.Distances;


    [TestFixture]
    public class DistanceTest
    {

        [Test]
        public void MahalanobisTest2()
        {
            // Example from Statistical Distance Calculator
            // http://maplepark.com/~drf5n/cgi-bin/dist.cgi

            double[,] cov =
            {
                { 1.030303, 2.132728, 0.576716 },
                { 2.132728, 4.510515, 1.185771 },
                { 0.576716, 1.185771, 0.398922 }
            };

            double[] x, y;
            double actual, expected;

            x = new double[] { 2, 4, 1 };
            y = new double[] { 0, 0, 0 };

            expected = 2.07735368677415;
            actual = Distance.Mahalanobis(x, y, cov.Inverse());
            Assert.AreEqual(expected, actual, 1e-10);


            x = new double[] { 7, 5, 1 };
            y = new double[] { 1, 0.52, -79 };

            expected = 277.8828871106366;
            actual = Distance.Mahalanobis(x, y, cov.Inverse());
            Assert.AreEqual(expected, actual, 0.0000000000001);
        }

        [Test]
        public void MahalanobisTest3()
        {
            // Example from Statistical Distance Calculator
            // http://maplepark.com/~drf5n/cgi-bin/dist.cgi

            double[,] cov =
            {
                { 1.030303, 2.132728, 0.576716 },
                { 2.132728, 4.510515, 1.185771 },
                { 0.576716, 1.185771, 0.398922 }
            };



            double[] x, y;
            double actual, expected;

            var svd = new SingularValueDecomposition(cov, true, true, true);

            var inv = cov.Inverse();
            var pinv = svd.Inverse();
            Assert.IsTrue(inv.IsEqual(pinv, 1e-6));

            x = new double[] { 2, 4, 1 };
            y = new double[] { 0, 0, 0 };

            {
                var bla = cov.Solve(x);
                var blo = svd.Solve(x);
                var ble = inv.Multiply(x);
                var bli = pinv.Multiply(x);

                Assert.IsTrue(bla.IsEqual(blo, 1e-6));
                Assert.IsTrue(bla.IsEqual(ble, 1e-6));
                Assert.IsTrue(bla.IsEqual(bli, 1e-6));
            }

            expected = 2.0773536867741504;
            actual = Distance.Mahalanobis(x, y, inv);
            Assert.AreEqual(expected, actual, 1e-6);

            actual = Distance.Mahalanobis(x, y, svd);
            Assert.AreEqual(expected, actual, 1e-6);


            x = new double[] { 7, 5, 1 };
            y = new double[] { 1, 0.52, -79 };

            expected = 277.8828871106366;
            actual = Distance.Mahalanobis(x, y, inv);
            Assert.AreEqual(expected, actual, 1e-5);
            actual = Distance.Mahalanobis(x, y, svd);
            Assert.AreEqual(expected, actual, 1e-5);
        }

        [Test]
        public void MahalanobisTest4()
        {
            double[] x, y;
            double expected, actual;

            x = new double[] { 2, 4, 1 };
            y = new double[] { 0, 0, 0 };
            expected = Distance.Euclidean(x, y);
            actual = Distance.Mahalanobis(x, y, Matrix.Identity(3));
            Assert.AreEqual(expected, actual);

            x = new double[] { 0.1, 0.12, -1 };
            y = new double[] { 195, 0, 2912 };
            expected = Distance.Euclidean(x, y);
            actual = Distance.Mahalanobis(x, y, Matrix.Identity(3));
            Assert.AreEqual(expected, actual);

            x = new double[] { -2, -4, -1 };
            y = new double[] { -2, -4, -1 };
            expected = Distance.Euclidean(x, y);
            actual = Distance.Mahalanobis(x, y, Matrix.Identity(3));
            Assert.AreEqual(expected, actual);

            x = new double[] { 2, 4, 1 };
            y = new double[] { 0, -7.2, 4.6 };
            expected = Distance.Euclidean(x, y);
            actual = Distance.Mahalanobis(x, y, Matrix.Identity(3));
            Assert.AreEqual(expected, actual);

            x = new double[] { -2, 4, 1 };
            y = new double[] { 0, -0.1, 4.2 };
            expected = Distance.Euclidean(x, y);
            actual = Distance.Mahalanobis(x, y, Matrix.Identity(3));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MahalanobisTest5()
        {
            double[] x, y;
            double expected, actual;

            x = new double[] { 2, 4, 1 };
            y = new double[] { 0, 0, 0 };
            expected = Distance.Euclidean(x, y);
            actual = Distance.Mahalanobis(x, y, new SingularValueDecomposition(Matrix.Identity(3)));
            Assert.AreEqual(expected, actual);

            x = new double[] { 0.1, 0.12, -1 };
            y = new double[] { 195, 0, 2912 };
            expected = Distance.Euclidean(x, y);
            actual = Distance.Mahalanobis(x, y, new SingularValueDecomposition(Matrix.Identity(3)));
            Assert.AreEqual(expected, actual);

            x = new double[] { -2, -4, -1 };
            y = new double[] { -2, -4, -1 };
            expected = Distance.Euclidean(x, y);
            actual = Distance.Mahalanobis(x, y, new SingularValueDecomposition(Matrix.Identity(3)));
            Assert.AreEqual(expected, actual);

            x = new double[] { 2, 4, 1 };
            y = new double[] { 0, -7.2, 4.6 };
            expected = Distance.Euclidean(x, y);
            actual = Distance.Mahalanobis(x, y, new SingularValueDecomposition(Matrix.Identity(3)));
            Assert.AreEqual(expected, actual);

            x = new double[] { -2, 4, 1 };
            y = new double[] { 0, -0.1, 4.2 };
            expected = Distance.Euclidean(x, y);
            actual = Distance.Mahalanobis(x, y, new SingularValueDecomposition(Matrix.Identity(3)));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MahalanobisTest6()
        {
            double[] x = { -1, 0, 0 };
            double[] y = { 0, 0, 0 };

            double[,] covX =
            {
                { 2, 3, 0 },
                { 3, 1, 0 },
                { 0, 0, 0 }
            };

            var pinv = covX.PseudoInverse();

            // Run actual test
            double expected = 0.14285714285714282;
            double actual = Distance.SquareMahalanobis(x, y, pinv);

            Assert.AreEqual(expected, actual, 1e-6);
            Assert.IsFalse(Double.IsNaN(actual));
        }

        [Test]
        public void doc_mahalanobis()
        {
            #region doc_mahalanobis_3
            // Let's say we would like to compute the Mahalanobis
            // distance between the two vectors x and y below:
            double[] x = { 2, 5, 1 };
            double[] y = { 4, 2, 2 };

            // Using the covariance
            double[,] covariance =
            {
                { 4, 3, 0 },
                { 3, 5, 2 },
                { 0, 2, 6 }
            };

            // There are multiple ways to create a Mahalanobis 
            // distance. The easiest method by far is by using:
            var mahalanobis = Mahalanobis.FromCovarianceMatrix(covariance);

            // Then, you can compute the distance using:
            double distance = mahalanobis.Distance(x, y);

            // However, if you need more control over how the covariance matrix
            // should be inverted, or if you have the precision matrix instead of
            // the covariance, you can use any of the alternative methods:

            var fromCholesky = new Mahalanobis(new CholeskyDecomposition(covariance));
            var fromSVD = new Mahalanobis(new SingularValueDecomposition(covariance));
            var fromPrecision1 = new Mahalanobis(covariance.Inverse());
            var fromPrecision2 = Mahalanobis.FromPrecisionMatrix(covariance.Inverse());

            // They all should produce equivalent results:
            double a = fromCholesky.Distance(x, y);
            double b = fromSVD.Distance(x, y);
            double c = fromPrecision1.Distance(x, y);
            double d = fromPrecision2.Distance(x, y);
            #endregion

            double expected = Distance.Mahalanobis(x, y, new CholeskyDecomposition(covariance));
            Assert.AreEqual(3.5185224171518357, expected, 1e-10);
            Assert.AreEqual(expected, distance, 1e-10);
            Assert.AreEqual(distance, a, 1e-10);
            Assert.AreEqual(distance, b, 1e-10);
            Assert.AreEqual(distance, c, 1e-10);
            Assert.AreEqual(distance, d, 1e-10);
        }

        [Test]
        public void doc_square_mahalanobis()
        {
            #region doc_square_mahalanobis_3
            // Let's say we would like to compute the Squared 
            // Mahalanobis distance between the vectors below:
            double[] x = { 2, 5, 1 };
            double[] y = { 4, 2, 2 };

            // Using the covariance
            double[,] covariance =
            {
                { 4, 3, 0 },
                { 3, 5, 2 },
                { 0, 2, 6 }
            };

            // There are multiple ways to create a Mahalanobis 
            // distance. The easiest method by far is by using:
            var mahalanobis = SquareMahalanobis.FromCovarianceMatrix(covariance);

            // Then, you can compute the distance using:
            double distance = mahalanobis.Distance(x, y);

            // However, if you need more control over how the covariance matrix
            // should be inverted, or if you have the precision matrix instead of
            // the covariance, you can use any of the alternative methods:

            var fromCholesky = new SquareMahalanobis(new CholeskyDecomposition(covariance));
            var fromSVD = new SquareMahalanobis(new SingularValueDecomposition(covariance));
            var fromPrecision1 = new SquareMahalanobis(covariance.Inverse());
            var fromPrecision2 = SquareMahalanobis.FromPrecisionMatrix(covariance.Inverse());

            // They all should produce equivalent results:
            double a = fromCholesky.Distance(x, y);
            double b = fromSVD.Distance(x, y);
            double c = fromPrecision1.Distance(x, y);
            double d = fromPrecision2.Distance(x, y);
            #endregion

            double expected = Math.Pow(Distance.SquareMahalanobis(x, y, new CholeskyDecomposition(covariance)), 2);
            Assert.AreEqual(12.379999999999997, distance, 1e-10);
            Assert.AreEqual(3.5185224171518357 * 3.5185224171518357, distance, 1e-10);
            Assert.AreEqual(distance, a, 1e-10);
            Assert.AreEqual(distance, b, 1e-10);
            Assert.AreEqual(distance, c, 1e-10);
            Assert.AreEqual(distance, d, 1e-10);
        }

        [Test]
        public void ManhattanTest()
        {
            double[] x = { 3, 6 };
            double[] y = { 0, 0 };
            double expected = 9;
            double actual = Distance.Manhattan(x, y);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void EuclideanTest()
        {
            double[] x = new double[] { 2, 4, 1 };
            double[] y = new double[] { 0, 0, 0 };
            double expected = 4.58257569495584;
            double actual = Distance.Euclidean(x, y);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void EuclideanTest2()
        {
            Assert.AreEqual(Distance.Euclidean(2, 4, 0, 1),
                Distance.Euclidean(new double[] { 2, 4 }, new double[] { 0, 1 }));

            Assert.AreEqual(Distance.SquareEuclidean(2, 4, 0, 1),
                Distance.SquareEuclidean(new double[] { 2, 4 }, new double[] { 0, 1 }));
        }

        [Test]
        public void ModularTest()
        {
            int a = 1;
            int b = 359;
            int modulo = 360;
            int expected = 2;

            int actual = (int)Distance.Modular(a, b, modulo);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void EuclideanTest1()
        {
            #region doc_euclidean_4
            // Let's say the coordinates of the first 2D vector are
            double x1 = 1.5;
            double y1 = -2.1;

            // And then the coordinates of the second 2D vector are:
            double x2 = 4;
            double y2 = 1;

            // The euclidean distance between (x1, y1) and (x2, y2) are:
            double a = Distance.Euclidean(x1, y1, x2, y2); // should be ~3.9824615503479754

            // This is equivalent to 
            double b = Distance.Euclidean(new[] { x1, y1 }, new[] { x2, y2 });
            #endregion

            Assert.AreEqual(3.9824615503479754, a, 1e-10);
            Assert.AreEqual(a, b);
            Assert.IsFalse(double.IsNaN(a));
            Assert.IsFalse(double.IsNaN(b));
        }

        [Test]
        public void CosineTest1()
        {
            #region doc_cosine_2
            // The Cosine distance between (0, 2, 4) and (2, 5, 1) can be directly computed as:
            double a = Distance.Cosine(new[] { 0.0, 2.0, 4.0 }, new[] { 2.0, 5.0, 1.0 }); // ~0.42845239335059182d

            // Or could also be computed by instantiating the Cosine class beforehand as:
            Cosine cos = new Cosine();
            double b = cos.Distance(new[] { 0.0, 2.0, 4.0 }, new[] { 2.0, 5.0, 1.0 }); // ~0.42845239335059182d
            #endregion

            Assert.AreEqual(0.42845239335059182d, a, 1e-10);
            Assert.AreEqual(a, b);
        }



        [Test]
        public void LevenshteinTest1()
        {
            Assert.AreEqual(0, Distance.Levenshtein("", ""));
            Assert.AreEqual(1, Distance.Levenshtein("", "a"));
            Assert.AreEqual(1, Distance.Levenshtein("a", ""));
            Assert.AreEqual(0, Distance.Levenshtein("a", "a"));
            Assert.AreEqual(0, Distance.Levenshtein(null, null));
            Assert.AreEqual(1, Distance.Levenshtein(null, "a"));
            Assert.AreEqual(1, Distance.Levenshtein("a", null));
            Assert.AreEqual(0, Distance.Levenshtein(null, ""));
            Assert.AreEqual(5, Distance.Levenshtein("apple", "banana"));

            Assert.AreEqual(0, Distance.Levenshtein(new int[] { }, new int[] { }));
            Assert.AreEqual(1, Distance.Levenshtein(new int[] { }, new int[] { 1 }));
            Assert.AreEqual(1, Distance.Levenshtein(new int[] { 1 }, new int[] { }));
            Assert.AreEqual(0, Distance.Levenshtein(new int[] { 1 }, new int[] { 1 }));
            Assert.AreEqual(0, Distance.Levenshtein(null, null));
            Assert.AreEqual(1, Distance.Levenshtein(null, new int[] { 1 }));
            Assert.AreEqual(1, Distance.Levenshtein(new int[] { 1 }, null));
            Assert.AreEqual(0, Distance.Levenshtein(new int[] { }, null));
            Assert.AreEqual(0, Distance.Levenshtein(null, new int[] { }));
            Assert.AreEqual(5, Distance.Levenshtein(new int[] { 1, 2, 2, 3, 4 }, new int[] { 5, 1, 6, 1, 6, 1 }));
        }

        [Test]
        public void IsMetricTest()
        {
            Assert.IsTrue(Distance.IsMetric(Distance.Euclidean));
            Assert.IsTrue(Distance.IsMetric((double[] a, double[] b) => Distance.Manhattan(a, b)));
            Assert.IsTrue(Distance.IsMetric((int[] a, int[] b) => Distance.Manhattan(a, b)));
            Assert.IsFalse(Distance.IsMetric(Distance.Hamming));
            Assert.IsTrue(Distance.IsMetric((double[] a, double[] b) => new Minkowski(1).Distance(a, b)));
            Assert.IsTrue(Distance.IsMetric((double[] a, double[] b) => new Levenshtein<double>().Distance(a, b)));
            Assert.IsTrue(Distance.IsMetric(Distance.Chebyshev));
            Assert.IsTrue(Distance.IsMetric(Distance.Hellinger));

            Assert.IsFalse(Distance.IsMetric(Distance.Cosine));
            Assert.IsFalse(Distance.IsMetric(Distance.SquareEuclidean));
            Assert.IsFalse(Distance.IsMetric((double[] a, double[] b) => Math.Pow(Distance.Manhattan(a, b), 2)));
            Assert.IsFalse(Distance.IsMetric(Distance.BrayCurtis));
            // Assert.IsFalse(Distance.IsMetric((double[] a, double[] b) => new Minkowski(2).Distance(a, b)));
            // Assert.IsFalse(Distance.IsMetric((double[] a, double[] b) => new Minkowski(3).Distance(a, b)));

            Assert.IsFalse(Distance.IsMetric((double[] a, double[] b) => Distance.Kulczynski(a, b)));
            Assert.IsTrue(Distance.IsMetric((double[] a, double[] b) => Distance.Jaccard(a, b)));
            // Assert.IsFalse(Distance.IsMetric((double[] a, double[] b) => Distance.RogersTanimoto(a, b)));
            // Assert.IsFalse(Distance.IsMetric((double[] a, double[] b) => Distance.SokalMichener(a, b)));
            // Assert.IsFalse(Distance.IsMetric((double[] a, double[] b) => Distance.SokalSneath(a, b)));
            // Assert.IsFalse(Distance.IsMetric((double[] a, double[] b) => Distance.Yule(a, b)));
            // Assert.IsFalse(Distance.IsMetric((double[] a, double[] b) => Distance.Dice(a, b)));



            Assert.IsTrue(Distance.IsMetric<double[]>(new Euclidean()));
            Assert.IsTrue(Distance.IsMetric<double[]>(new Manhattan()));
            Assert.IsFalse(Distance.IsMetric<double[]>(new Hamming()));
            Assert.IsTrue(Distance.IsMetric<double[]>(new Minkowski(1)));
            Assert.IsTrue(Distance.IsMetric(new Levenshtein()));
            Assert.IsTrue(Distance.IsMetric(new Chebyshev()));
            Assert.IsFalse(Distance.IsMetric(new Cosine()));
            Assert.IsTrue(Distance.IsMetric(new Hellinger()));
            Assert.IsFalse(Distance.IsMetric<double[]>(new SquareEuclidean()));
            Assert.IsFalse(Distance.IsMetric(new BrayCurtis()));
            // Assert.IsFalse(Distance.IsMetric<double[]>(new Minkowski(2)));
            // Assert.IsFalse(Distance.IsMetric<double[]>(new Minkowski(3)));

            Assert.IsFalse(Distance.IsMetric<double[]>(new Kulczynski()));
            Assert.IsTrue(Distance.IsMetric<double[]>(new Jaccard<double>()));
            Assert.IsFalse(Distance.IsMetric<double[]>(new RogersTanimoto()));
            Assert.IsFalse(Distance.IsMetric<double[]>(new SokalMichener()));
            Assert.IsFalse(Distance.IsMetric<double[]>(new SokalSneath()));
            Assert.IsFalse(Distance.IsMetric<double[]>(new Yule()));
            Assert.IsFalse(Distance.IsMetric<double[]>(new Dice()));

            // Assert.IsFalse(Distance.IsMetric(Dissimilarity.RusselRao));
        }

        [Test]
        public void getdistance_test()
        {
            #region doc_getdistance
            // Let's say you have been using the static Distance.Euclidean() method in 
            // your code, and now you would like to obtain a reference to a class that 
            // implements the IDistance interface for this same distance, such that you 
            // could pass it to some other method in the framework:

            double[] x = new double[] { 2, 4, 1 };
            double[] y = new double[] { 0, 0, 0 };

            double a = Distance.Euclidean(x, y); // should be 4.58257569495584

            // Use the GetDistance method to obtain an IDistance that implements it:
            IDistance<double[]> obj = Distance.GetDistance<double[]>(Distance.Euclidean);

            // We can continue computing the same distances as before using:
            double b = obj.Distance(x, y); // should be 4.58257569495584
            #endregion

            double expected = 4.58257569495584;
            Assert.AreEqual(a, expected);
            Assert.AreEqual(b, expected);
        }
    }
}
