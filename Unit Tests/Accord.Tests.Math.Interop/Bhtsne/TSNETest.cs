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

#if !MONO
namespace Accord.Tests.Interop.Math
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Accord.Collections;
    using Accord.MachineLearning;
    using Accord.Math;
    using Accord.Math.Comparers;
    using Accord.Math.Distances;
    using NUnit.Framework;
    using AccordTestsMathCpp2;
    using Accord.MachineLearning.Clustering;
    using Accord.Statistics.Distributions.Univariate;

    [TestFixture]
    public class TSNETest
    {
        [Test]
        public void nth_element_test()
        {
            int[] random = 
            {
			    1823928530,
			    438567658,
			    419543343,
			    1143354873,
			    1106789249,
			    2061232594,
			    1361344170,
			    1844371025,
			    1583434746,
			    820082791,
			    815141475
            };

            var a = (int[])random.Clone();
            VPTreeWrapper.nth_element(a, 5);

            var keys = (int[])random.Clone();
            Sort.NthElement(keys, 5);

            Assert.AreEqual(keys[5], a[5]);
            Assert.IsTrue(a.IsEqual(keys));
        }

        [Test]
        [Category("Slow")]
        public void nth_element_test_2()
        {
            Accord.Math.Random.Generator.Seed = 0;

            for (int n = 0; n < 1000; n++)
            {
                int[] random = Vector.Random(n, 0, 1000);

                var a = (int[])random.Clone();
                a.Sort();

                for (int j = 0; j < n; j++)
                {
                    var b = (int[])random.Clone();
                    double actual = Sort.NthElement(b, j);
                    double expected = a[j];
                    Assert.AreEqual(actual, expected);        
                }
            }
        }


        [Test]
        public void FromDataTest2()
        {
            Accord.Math.Random.Generator.Seed = 0;

            int N = yinyang.GetLength(0);
            var init = Matrix.Random(N, 1, new NormalDistribution(0, 0.001));

            var X = yinyang.Submatrix(null, 0, 1);
            var Y = (double[,])init.Clone();
            var x = X.ToJagged();
            var y = Y.ToJagged();

            double perplexity = 20;
            double theta = 0.5;
            TSNEWrapper.run(X, Y, perplexity, theta);
            var expected = Y.Flatten();


            TSNE.run(x, y, perplexity, theta, true);
            var actual = y.Flatten();

            Assert.IsTrue(actual.IsEqual(expected));
        }




        [Test]
        public void compute_squared_distance_larger()
        {
            var points = yinyang.Submatrix(null, 0, 1).ToJagged();

            var X = points.ToMatrix();
            int N = X.Rows();
            int D = X.Columns();
            double[,] expected = new double[N, N];
            TSNEWrapper.computeSquaredEuclideanDistance(X, expected);

            double[][] actual = Jagged.Zeros(N, N);
            TSNE.computeSquaredEuclideanDistance(points, N, D, actual);

            Assert.IsTrue(actual.IsEqual(expected));
        }

        [Test]
        public void compute_squared_distance_1()
        {
            double[][] points =
            {
                new double[] { 2, 3, 2 },
                new double[] { 5, 4, 5 },
                new double[] { 9, 6, 4 },
                new double[] { 4, 7, 5 },
                new double[] { 8, 1, 1 },
                new double[] { 1, 2, 4 },
            };

            var X = points.ToMatrix();
            int N = X.Rows();
            int D = X.Columns();
            double[,] expected = new double[N, N];
            TSNEWrapper.computeSquaredEuclideanDistance(X, expected);

            double[][] actual = Jagged.Zeros(N, N);
            TSNE.computeSquaredEuclideanDistance(points, N, D, actual);

            Assert.IsTrue(actual.IsEqual(expected));
        }

        [Test]
        public void symmetrizeMatrix_1()
        {
            double perplexity = 0.5;
            int n = 6;
            int k = (int)(3 * perplexity);
            uint[] row_P = Vector.Create(n + 1, new uint[] { 0, 1, 2, 3, 4, 5, 6 });
            uint[] col_P = Vector.Create(n * k, new uint[] { 5, 3, 1, 1, 2, 1 });
            double[] val_P = Vector.Create(n * k, new double[] 
            { 
                0.99901046609114708, 
                0.99901047304189827,
                0.99901046869768451,
                0.99901047304189827,	
                0.99901046869768484,
                0.99901046869768451,
            });


            uint[] expected_row = Vector.Create(100, row_P);
            uint[] expected_col = Vector.Create(100, col_P);
            double[] expected_val = Vector.Create(100, val_P);
            TSNEWrapper.symmetrizeMatrix(expected_row, expected_col, expected_val, n);

            int[] actual_row = row_P.To<int[]>();
            int[] actual_col = col_P.To<int[]>();
            double[] actual_val = (double[])val_P.Clone();
            TSNE.symmetrizeMatrix(ref actual_row, ref actual_col, ref actual_val, n);

            expected_row = expected_row.First(7);
            expected_col = expected_col.First(10);
            expected_val = expected_val.First(10);
            Assert.IsTrue(actual_col.IsEqual(expected_col));
            Assert.IsTrue(actual_row.IsEqual(expected_row));
            Assert.IsTrue(actual_val.IsEqual(expected_val));
        }

        [Test]
        public void computeGaussianPerplexity_1()
        {
            double[][] points =
            {
                new double[] { 2, 3, 2 },
                new double[] { 5, 4, 5 },
                new double[] { 9, 6, 4 },
                new double[] { 4, 7, 5 },
                new double[] { 8, 1, 1 },
                new double[] { 1, 2, 4 },
            };

            double perplexity = 0.5;
            int N = points.Length;
            int D = 3;

            var X = points.ToMatrix();
            double[,] expected = new double[N, N];
            TSNEWrapper.computeGaussianPerplexity(X, N, D, expected, perplexity);

            double[][] actual = Jagged.Zeros(N, N);
            TSNE.computeGaussianPerplexity(points, N, D, ref actual, perplexity);

            Assert.IsTrue(actual.IsEqual(expected, rtol: 1e-5));
        }

        [Test]
        public void computeGaussianPerplexity_larger()
        {
            var points = yinyang.Submatrix(null, 0, 1).ToJagged();

            double perplexity = 0.5;
            int N = points.Rows();
            int D = points.Columns();

            var X = points.ToMatrix();
            double[,] expected = new double[N, N];
            TSNEWrapper.computeGaussianPerplexity(X, N, D, expected, perplexity);

            double[][] actual = Jagged.Zeros(N, N);
            TSNE.computeGaussianPerplexity(points, N, D, ref actual, perplexity);

            Assert.IsTrue(actual.IsEqual(expected, rtol: 1e-5));
        }

        [Test]
        public void evaluateError_1()
        {
            double perplexity = 0.5;
            double theta = 0.5;
            int N = 6;
            int K = (int)(3 * perplexity);
            int D = 2;
            uint[] row_P = Vector.Create(N + 1, new uint[] { 0, 1, 2, 3, 4, 5, 6 });
            uint[] col_P = Vector.Create(N * K, new uint[] { 5, 3, 1, 1, 2, 1 });
            double[] val_P = Vector.Create(N * K, new double[] 
            { 
                0.99901046609114708, 
                0.99901047304189827,
                0.99901046869768451,
                0.99901047304189827,	
                0.99901046869768484,
                0.99901046869768451,
            });

            double[,] Y = Matrix.Random(6, 2, new NormalDistribution());
            double[][] y = Y.ToJagged();

            uint[] expected_row = Vector.Create(row_P);
            uint[] expected_col = Vector.Create(col_P);
            double[] expected_val = Vector.Create(val_P);
            double expected = TSNEWrapper.evaluateError(expected_row, expected_col, expected_val, Y, N, D, theta);

            int[] actual_row = row_P.To<int[]>();
            int[] actual_col = col_P.To<int[]>();
            double[] actual_val = (double[])val_P.Clone();
            double actual = TSNE.evaluateError(actual_row, actual_col, actual_val, y, N, D, theta);

            Assert.AreEqual(expected, actual);
            Assert.IsTrue(actual_col.IsEqual(expected_col));
            Assert.IsTrue(actual_row.IsEqual(expected_row));
            Assert.IsTrue(actual_val.IsEqual(expected_val));
        }

        [Test]
        public void evaluateError_2()
        {
            int N = 6;
            int D = 2;

            double[,] P = Matrix.Random(6, 6, new NormalDistribution());
            double[][] p = P.ToJagged();

            double[,] Y = Matrix.Random(6, 2, new NormalDistribution());
            double[][] y = Y.ToJagged();

            double expected = TSNEWrapper.evaluateError(P, Y, N, D);

            double actual = TSNE.evaluateError(p, y, N, D);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void computeGaussianPerplexity_2()
        {
            double[][] points =
            {
                new double[] { 2, 3, 20 },
                new double[] { 5, 4, 5 },
                new double[] { 9, 6, 500 },
                new double[] { 4, 7, -100 },
                new double[] { 8, 1, 67 },
                new double[] { 1, 2, -888 },
            };

            double perplexity = 0.5;
            int N = points.Length;
            int D = 3;
            int K = (int)(3 * perplexity);
            double[,] X = points.ToMatrix();
            double[][] x = X.ToJagged();

            uint[] expected_row = Vector.Zeros<uint>(100);
            uint[] expected_col = Vector.Zeros<uint>(100);
            double[] expected_val = Vector.Zeros<double>(100);
            TSNEWrapper.computeGaussianPerplexity(X, N, D, expected_row, expected_col, expected_val, perplexity, K);

            int[] actual_row = null;
            int[] actual_col = null;
            double[] actual_val = null;
            TSNE.computeGaussianPerplexity(x, N, D, ref actual_row, ref actual_col, ref actual_val, perplexity, K);

            expected_row = expected_row.First(7);
            expected_col = expected_col.First(6);
            expected_val = expected_val.First(6);
            Assert.IsTrue(actual_row.IsEqual(expected_row));
            Assert.IsTrue(actual_col.IsEqual(expected_col));
            Assert.IsTrue(actual_val.IsEqual(expected_val, 1e-4));
        }

        [Test]
        public void computeGaussianPerplexity2_larger()
        {
            var points = yinyang.Submatrix(null, 0, 1).ToJagged();

            double perplexity = 0.5;
            int N = points.Rows();
            int D = points.Columns();
            int K = (int)(3 * perplexity);
            double[,] X = points.ToMatrix();
            double[][] x = X.ToJagged();

            uint[] expected_row = Vector.Zeros<uint>(200);
            uint[] expected_col = Vector.Zeros<uint>(200);
            double[] expected_val = Vector.Zeros<double>(200);
            TSNEWrapper.computeGaussianPerplexity(X, N, D, expected_row, expected_col, expected_val, perplexity, K);

            int[] actual_row = null;
            int[] actual_col = null;
            double[] actual_val = null;
            TSNE.computeGaussianPerplexity(x, N, D, ref actual_row, ref actual_col, ref actual_val, perplexity, K);

            expected_row = expected_row.First(101);
            expected_col = expected_col.First(100);
            expected_val = expected_val.First(100);
            Assert.IsTrue(actual_row.IsEqual(expected_row));
            Assert.IsTrue(actual_col.IsEqual(expected_col));
            Assert.IsTrue(actual_val.IsEqual(expected_val, 1e-4));
        }


        [Test]
        public void computeGradient_1()
        {
            Accord.Math.Random.Generator.Seed = 0;

            double perplexity = 0.5;
            double theta = 0.5;
            int N = 100;
            int K = (int)(3 * perplexity);
            int D = 3;
            uint[] row_P = Vector.Create(N + 1, new uint[] { 0, 1, 2, 3, 4, 5, 6 });
            uint[] col_P = Vector.Create(N * K, new uint[] { 5, 3, 1, 1, 2, 1 });
            double[] val_P = Vector.Create(N * K, new double[] 
            { 
                0.83901046609114708, 
                0.39701047304189827,
                0.19501046869768451,
                0.59401047304189827,	
                0.49301046869768484,
                0.59901046869768451,
            });

            double[,] P = Matrix.Random(N, N, new NormalDistribution());
            double[][] p = P.ToJagged();

            double[,] Y = Matrix.Random(N, D, new NormalDistribution());
            double[][] y = Y.ToJagged();



            uint[] expected_row = Vector.Create(row_P);
            uint[] expected_col = Vector.Create(col_P);
            double[] expected_val = Vector.Create(val_P);
            double[,] expected = Matrix.Zeros(N, D);
            TSNEWrapper.computeGradient(P, expected_row, expected_col, expected_val, Y, N, D, expected, theta);

            int[] actual_row = row_P.To<int[]>();
            int[] actual_col = col_P.To<int[]>();
            double[] actual_val = (double[])val_P.Clone();
            double[][] actual = Jagged.Zeros(N, D);
            TSNE.computeGradient(p, actual_row, actual_col, actual_val, y, N, D, actual, theta);

            Assert.IsTrue(actual.IsEqual(expected));
            Assert.IsTrue(actual_row.IsEqual(expected_row));
            Assert.IsTrue(actual_col.IsEqual(expected_col));
            Assert.IsTrue(actual_val.IsEqual(expected_val, 1e-4));
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
#endif
