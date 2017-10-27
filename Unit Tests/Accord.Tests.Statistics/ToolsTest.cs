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

namespace Accord.Tests.Statistics
{
    using System.Linq;
    using Accord.Math;
    using NUnit.Framework;
    using Tools = Accord.Statistics.Tools;
    using Accord.Statistics;
    using System;
    using System.Collections.Generic;

    [TestFixture]
    public class ToolsTest
    {

        [Test]
        public void CenteringTest()
        {

            double[,] C2 = Matrix.Centering(2);

            Assert.IsTrue(Matrix.IsEqual(C2, new double[,] {
                                                             {  0.5, -0.5 },
                                                             { -0.5,  0.5 }
                                                           }));

            double[,] X = {
                              { 1, 5, 2, 0   },
                              { 6, 2, 3, 100 },
                              { 2, 5, 8, 2   },
                          };



            double[,] CX = Matrix.Centering(3).Dot(X); // Remove means from rows
            double[,] XC = X.Dot(Matrix.Centering(4)); // Remove means from columns

            double[] colMean = Measures.Mean(X, 1);
            double[] rowMean = Measures.Mean(X, 0);

            Assert.IsTrue(rowMean.IsEqual(new double[] { 3.0, 4.0, 4.3333, 34.0 }, 0.001));
            Assert.IsTrue(colMean.IsEqual(new double[] { 2.0, 27.75, 4.25 }, 0.001));


            double[,] Xr = X.Subtract(rowMean, 0);          // Remove means from rows
            double[,] Xc = X.Subtract(colMean, 1);          // Remove means from columns

            Assert.IsTrue(Matrix.IsEqual(XC, Xc));
            Assert.IsTrue(Matrix.IsEqual(CX, Xr, 0.00001));

            double[,] S1 = XC.Dot(X.Transpose());
            double[,] S2 = Xc.Dot(Xc.Transpose());
            double[,] S3 = Measures.Scatter(X, colMean, 1);

            Assert.IsTrue(Matrix.IsEqual(S1, S2));
            Assert.IsTrue(Matrix.IsEqual(S2, S3));

            double[,] S4 = XC.DotWithTransposed(X);
            double[,] S5 = Xc.DotWithTransposed(Xc);

            Assert.IsTrue(Matrix.IsEqual(S1, S4));
            Assert.IsTrue(Matrix.IsEqual(S2, S5));
        }


        [Test]
        public void MahalanobisTest()
        {
            double[] x = { 1, 0 };
            double[,] y =
            {
                { 1, 0 },
                { 0, 8 },
                { 0, 5 }
            };

            // Computing the mean of y
            double[] meanY = Measures.Mean(y, dimension: 0);

            // Computing the covariance matrix of y
            double[,] covY = Measures.Covariance(y, meanY);

            // Inverting the covariance matrix
            double[,] precision = covY.Inverse();

            // Run actual test
            double expected = 1.33333;
            double actual = Distance.SquareMahalanobis(x, meanY, precision);

            Assert.AreEqual(expected, actual, 0.0001);
        }

        [Test]
        public void SubgroupTest1()
        {
            Accord.Math.Random.Generator.Seed = 0;

            double[] value = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };

            int[] idx = Tools.RandomGroups(value.Length, 4);

            double[][] groups = value.Subgroups(idx);

            int[] hist = idx.Histogram();

            Assert.AreEqual(4, groups.Length);
            Assert.AreEqual(3, groups[0].Length);
            Assert.AreEqual(4, groups[1].Length);
            Assert.AreEqual(3, groups[2].Length);
            Assert.AreEqual(3, groups[3].Length);
            Assert.AreEqual(new[] { 3, 4, 3, 3 }, hist);

            for (int i = 0; i < groups.Length; i++)
            {
                for (int j = 0; j < groups[i].Length; j++)
                {
                    double e = groups[i][j];

                    for (int k = 0; k < groups.Length; k++)
                    {
                        for (int l = 0; l < groups[k].Length; l++)
                        {
                            double a = groups[k][l];

                            if (k != i && l != j)
                                Assert.AreNotEqual(a, l);
                        }
                    }
                }
            }
        }

        [Test]
        public void StandardDeviationTest4()
        {
            double[][] matrix =
            {
                new double[] { 2, -1.0, 5 },
                new double[] { 7,  0.5, 9 },
            };

            double[] means = Measures.Mean(matrix, dimension: 0);
            Assert.IsTrue(means.IsEqual(new[] { 4.5000, -0.2500, 7.0000 }));

            double[] stdev = Measures.StandardDeviation(matrix, means);
            Assert.IsTrue(stdev.IsEqual(new[] { 3.5355339059327378, 1.0606601717798212, 2.8284271247461903 }));

            double[] stdev2 = Measures.StandardDeviation(matrix);
            Assert.IsTrue(stdev2.IsEqual(stdev));
        }

        [Test]
        public void StandardDeviationTest3()
        {
            double[,] matrix =
            {
                { 2, -1.0, 5 },
                { 7,  0.5, 9 },
            };

            double[] means = Measures.Mean(matrix, dimension: 0);
            Assert.IsTrue(means.IsEqual(new[] { 4.5000, -0.2500, 7.0000 }));

            double[] stdev = Measures.StandardDeviation(matrix, means);
            Assert.IsTrue(stdev.IsEqual(new[] { 3.5355339059327378, 1.0606601717798212, 2.8284271247461903 }));

            double[] stdev2 = Measures.StandardDeviation(matrix);
            Assert.IsTrue(stdev2.IsEqual(stdev));
        }

        [Test]
        public void StandardDeviationTest2()
        {
            double[] values = { -5, 0.2, 2 };
            double expected = 3.6350149013908224;
            double actual = Measures.StandardDeviation(values);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WeightedVarianceTest1()
        {
            double[] original = { 5, 5, 1, 4, 1, 2, 2, 3, 3, 3, 4, 3, 3, 3, 4, 3, 2, 3 };
            double expected = Measures.Variance(original, unbiased: false);

            double[] weights = { 2, 1, 1, 1, 2, 3, 1, 3, 1, 1, 1, 1 };
            double[] samples = { 5, 1, 4, 1, 2, 3, 4, 3, 4, 3, 2, 3 };

            weights = weights.Divide(weights.Sum());
            double actual = Measures.WeightedVariance(samples, weights, unbiased: false);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WeightedVarianceTest3()
        {
            double[] weights = { 2, 1, 1, 1, 2, 3, 1, 3, 1, 1, 1, 1 };
            double[] samples = { 5, 1, 4, 1, 2, 3, 4, 3, 4, 3, 2, 3 };

            weights = weights.Divide(weights.Sum());
            double actual = Measures.WeightedVariance(samples, weights);

            Assert.AreEqual(1.3655172413793104, actual);
        }

        [Test]
        public void WeightedVarianceTest2()
        {
            double[] original = { 5, 5, 1, 4, 1, 2, 2, 3, 3, 3, 4, 3, 3, 3, 4, 3, 2, 3 };
            double expected = Measures.Variance(original);

            var repeats = new[] { 2, 1, 1, 1, 2, 3, 1, 3, 1, 1, 1, 1 };
            var samples = new[] { 5, 1, 4, 1, 2, 3, 4, 3, 4, 3, 2, 3.0 };
            double actual = Measures.WeightedVariance(samples, repeats);

            Assert.AreEqual(expected, actual);
        }


        [Test]
        public void MeanTest4()
        {
            double[,] matrix =
            {
                { 2, -1.0, 5 },
                { 7,  0.5, 9 },
            };

            double[] sums = Matrix.Sum(matrix, 0);
            Assert.IsTrue(sums.IsEqual(new[] { 9.0, -0.5, 14.0 }));

            double[] expected = { 4.5000, -0.2500, 7.0000 };
            double[] actual = Measures.Mean(matrix, sums);

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void MeanTest3()
        {
            double[] values = { -5, 0.2, 2 };
            double expected = -0.93333333333333324;
            double actual = Measures.Mean(values);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MeanTest()
        {
            double[,] matrix =
            {
                { 2, -1.0, 5 },
                { 7,  0.5, 9 },
            };

            double[] rowMean = Measures.Mean(matrix, 0);
            Assert.IsTrue(rowMean.IsEqual(new[] { 4.5000, -0.2500, 7.0000 }));

            double[] colMean = Measures.Mean(matrix, 1);
            Assert.IsTrue(colMean.IsEqual(2, 5.5));
        }


        [Test]
        public void MedianTest1()
        {
            double[] values;
            double expected;
            double actual;

            values = new double[] { -5, 0.2, 2, 5, -0.7 };
            expected = 0.2;
            actual = Measures.Median(values, false);
            Assert.AreEqual(expected, actual);

            values = new double[] { -5, 0.2, 2, 5 };
            expected = 1.1;
            actual = Measures.Median(values, false);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MedianTest2()
        {
            double[][] matrix =
            {
                new double[] {   2, -1.0,  5 },
                new double[] {   7,  1.7,  9 },
                new double[] {  -4,  2.5,  6 },
                new double[] { 0.2,  0.5, -2 },
            };

            double[] mean = Measures.Mean(matrix, dimension: 0);
            Assert.IsTrue(mean.IsEqual(1.3000, 0.9250, 4.5000));

            double[] median = Measures.Median(matrix);
            Assert.IsTrue(median.IsEqual(1.1000, 1.1000, 5.5000));


            matrix = matrix.Transpose();
            mean = Measures.Mean(matrix, dimension: 0);
            Assert.IsTrue(mean.IsEqual(new[] { 2.0000, 5.8999999999999995, 1.5000, -0.43333333333333335 }));

            median = Measures.Median(matrix);
            Assert.IsTrue(median.IsEqual(new[] { 2.0000, 7.0000, 2.5000, 0.2000 }));
        }

        [Test]
        public void MedianTest()
        {
            double[,] matrix =
            {
                {   2, -1.0,  5 },
                {   7,  1.7,  9 },
                {  -4,  2.5,  6 },
                { 0.2,  0.5, -2 },
            };

            double[] mean = Measures.Mean(matrix, dimension: 0);
            Assert.IsTrue(mean.IsEqual(1.3000, 0.9250, 4.5000));

            double[] median = Measures.Median(matrix);
            Assert.IsTrue(median.IsEqual(1.1000, 1.1000, 5.5000));


            matrix = matrix.Transpose();
            mean = Measures.Mean(matrix, dimension: 0);
            Assert.IsTrue(mean.IsEqual(new[] { 2.0000, 5.8999999999999995, 1.5000, -0.43333333333333335 }));

            median = Measures.Median(matrix);
            Assert.IsTrue(median.IsEqual(new[] { 2.0000, 7.0000, 2.5000, 0.2000 }));
        }

        [Test]
        public void QuartileTest1()
        {
            double[] values = new double[] { 3, 4, 8 };
            double q1, q3, actual;

            actual = Measures.Quartiles(values, out q1, out q3, false);
            Assert.AreEqual(3, q1);
            Assert.AreEqual(4, actual);
            Assert.AreEqual(8, q3);
        }

        [Test]
        public void QuartileTest2()
        {
            double[] values = new double[] { 1, 3, 3, 4, 5, 6, 6, 7, 8, 8 };
            double q1, q3, actual;

            //quantile(c(1, 3, 3, 4, 5, 6, 6, 7, 8, 8), type = 6)
            //
            //    0 % 25 % 50 % 75 % 100 %
            //  1.00 3.00 5.50 7.25 8.00

            actual = Measures.Quartiles(values, out q1, out q3, false);
            Assert.AreEqual(3, q1);
            Assert.AreEqual(5.5, actual);
            Assert.AreEqual(7.25, q3);
        }

        [Test]
        public void QuartileTest3()
        {
            double[] values = new double[] { 102, 104, 105, 107, 108, 109, 110, 112, 115, 116, 118 };
            double q1, q3, actual;

            actual = Measures.Quartiles(values, out q1, out q3, false);
            Assert.AreEqual(105, q1);
            Assert.AreEqual(109, actual);
            Assert.AreEqual(115, q3);
        }


        public static IEnumerable<object[]> QuartilesTestValues = new List<object[]>
        {
            new object[] {new double[] {0.0}, 0.0, 0.0}, // correct
            new object[] {new double[] {0.0, 1.0}, 0.25, 0.75}, // correct
            new object[] {new double[] {1.0, 0.0}, 0.25, 0.75}, // correct
            new object[] {new double[] {1.0, 3.0}, 1.5, 2.5}, // correct
            new object[] {new double[] {3.0, 1.0}, 1.5, 2.5}, // correct
            new object[] {new double[] {0.0, 1.0, 2.0}, 0.5, 1.5}, // failing
            new object[] {new double[] {0.0, 1.0, 2.0, 4.0, 5.4, 3.5, 7.8, 8.9}, 1.75, 6.0}, // failing
            new object[] // failing
            {
                new double[] {0.0, 1.0, 2.0, 4.0, 5.4, 3.5, 7.8, 8.9, 17.0, 23.78, 98.9, 2.3, 4.5, 6.7, 9.34, 42.42}, 3.2, 11.255
            },
            new object[] // failing
            {
                new double[] {0.0, 5.4, 2.0, 4.0, 1.0, 3.5, 7.8, 17.0, 8.9, 98.9, 23.78, 2.3, 4.5, 6.7, 42.42, 9.34}, 3.2, 11.255
            }
        };

        [Test]
        [TestCaseSource("QuartilesTestValues")]
        public void return_correct_q1_value_for_vector(double[] values, double expectedQ1, double expectedQ3)
        {
            var q1 = values.LowerQuartile(type: QuantileMethod.R);
            var q3 = values.UpperQuartile(type: QuantileMethod.R);

            Assert.AreEqual(expectedQ1, q1, 1e-6);
            Assert.AreEqual(expectedQ3, q3, 1e-6);
        }

        [Test]
        public void QuartileTest5()
        {
            double[] values = new double[] { 3, 4, 8 };
            double q1, q3, actual;

            actual = Measures.Quartiles(values, out q1, out q3, false);
            Assert.AreEqual(3, q1);
            Assert.AreEqual(4, actual);
            Assert.AreEqual(8, q3);
        }

        [Test]
        public void QuartileTest6()
        {
            double[] values;
            double q1, q3, actual;

            values = new double[] { 3, 4 };
            actual = Measures.Quartiles(values, out q1, out q3, false);
            Assert.AreEqual(3, q1);
            Assert.AreEqual(3.5, actual);
            Assert.AreEqual(4, q3);

            values = new double[] { 4, 3 };
            actual = Measures.Quartiles(values, out q1, out q3, false);
            Assert.AreEqual(3, q1);
            Assert.AreEqual(3.5, actual);
            Assert.AreEqual(4, q3);
        }

        [Test]
        public void QuartileMatrixTest()
        {
            double[][] values =
            {
                new [] { 52.0 },
                new [] { 42.0 }
            };

            double[] q1, q3, actual;
            actual = Measures.Quartiles(values, out q1, out q3);

            // quantile(c(52, 42), type = 6)
            //   0 % 25 % 50 % 75 % 100 %
            //   42   42   47   52   52

            Assert.AreEqual(47, actual[0]);
            Assert.AreEqual(42, q1[0]);
            Assert.AreEqual(52, q3[0]);
        }

        [Test]
        public void QuartileTest7()
        {
            double[] values = new double[] { 0, 1, 2 };
            double q1, q3, actual;

            actual = Measures.Quartiles(values, out q1, out q3, false);
            Assert.AreEqual(0, q1);
            Assert.AreEqual(1, actual);
            Assert.AreEqual(2, q3);
        }

        [Test]
        [TestCase(new double[] { 0.0, 1.0, 2.0, 4.0, 5.4, 3.5, 7.8, 8.9, 17.0, 23.78, 98.9, 2.3, 4.5, 6.7, 9.34, 42.42 }, 3.2, 11.255)]
        [TestCase(new double[] { 0.0, 5.4, 2.0, 4.0, 1.0, 3.5, 7.8, 17.0, 8.9, 98.9, 23.78, 2.3, 4.5, 6.7, 42.42, 9.34 }, 3.2, 11.255)]
        [TestCase(new double[] { 0.0, 1.0, 2.0, 4.0, 5.4, 3.5, 7.8, 8.9, 17.0, 23.78, 98.9, 2.3, 4.5, 6.7, 9.34, 42.42, 23, 17.87, 18.54, 16.23, 15.34, 19.8723, 23, 24.32 }, 4.375, 20.65423)]
        [TestCase(new double[] { 18, 31, 25, 2, 22, 13, 37, 1, 4, 7, 6, 45, 10, 24, 23, 49, 27, 9, 35, 14, 34, 33, 41, 42, 20, 43, 3, 48, 15, 39, 11, 38, 46, 17, 40, 16, 50, 29, 19, 47, 12, 28, 32, 8, 30, 26, 5, 44, 36, 21 }, 13.25, 37.75)]
        [TestCase(new double[] { 18, 14, 1, 15, 4, 32, 10, 26, 38, 9, 24, 16, 31, 20, 25, 30, 22, 6, 28, 21, 33, 17, 5, 35, 2, 13, 36, 8, 29, 7 }, 9.25, 28.75)]
        [TestCase(new double[] { 112, 718, 320, 576, 547, 658, 253, 560, 408, 314, 681, 303, 236, 753, 122, 239, 222, 797, 593, 274, 338, 604, 52, 245, 389 }, 245, 593)]
        [TestCase(new double[] { 209, 556, 317, 571, 219, 599, 568, 516, 582, 279, 298, 319, 614, 290, 458, 262, 281, 606, 513, 519, 356, 338, 525, 576, 180 }, 290, 568)]
        public void quartile_test_gh865(double[] values, double expectedQ1, double expectedQ3)
        {
            // https://github.com/accord-net/framework/issues/865

            double q1, q3;
            Measures.Quartiles(values, out q1, out q3, alreadySorted: false, type: QuantileMethod.R);
            Assert.AreEqual(expectedQ1, q1, 1e-6);
            Assert.AreEqual(expectedQ3, q3, 1e-5);

            Measures.Quartiles(values.Sorted(), out q1, out q3, alreadySorted: true, type: QuantileMethod.R);
            Assert.AreEqual(expectedQ1, q1, 1e-6);
            Assert.AreEqual(expectedQ3, q3, 1e-5);

            Measures.Quartiles(values.Sorted(), out q1, out q3, alreadySorted: false, type: QuantileMethod.R);
            Assert.AreEqual(expectedQ1, q1, 1e-6);
            Assert.AreEqual(expectedQ3, q3, 1e-5);

            Measures.Quartiles(values, out q1, out q3, alreadySorted: true, type: QuantileMethod.R);
            Assert.IsFalse(expectedQ1.IsEqual(q1, 1e-3) && expectedQ3.IsEqual(q3, 1e-3));
        }

        [Test]
        public void ModeTest1()
        {
            double[] values = { 3, 3, 1, 4 };
            double expected = 3;
            double actual = Measures.Mode(values);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ModeTest()
        {
            double[,] matrix =
            {
                { 3, 3, 1, 4 },
                { 0, 1, 1, 1 },
                { 0, 1, 2, 4 },
            };


            double[] expected = { 0, 1, 1, 4 };
            double[] actual = Measures.Mode(matrix);
            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void ModeTest2()
        {
            double[] values = { 0, 1, 1, 2 };

            double expected = 1;
            double actual = Measures.Mode(values);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ModeTest3()
        {
            double[] values = { 0, 1, 2, 2 };

            double expected = 2;
            double actual = Measures.Mode(values);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ModeTest_NonComparable()
        {
            object a = new object();
            object b = new object();
            object c = new object();

            object[] values;
            object actual;

            values = new[] { a, a, b, c };
            actual = Measures.Mode(values);
            Assert.AreEqual(a, actual);

            values = new[] { a, b, b, c };
            actual = Measures.Mode(values);
            Assert.AreEqual(b, actual);

            values = new[] { a, b, c, c };
            actual = Measures.Mode(values);
            Assert.AreEqual(c, actual);

            int count;
            values = new[] { a, b, c, a };
            actual = Measures.Mode(values, out count);
            Assert.AreEqual(a, actual);
            Assert.AreEqual(2, count);

            values = new[] { b, a, b, b, c };
            actual = Measures.Mode(values, out count);
            Assert.AreEqual(b, actual);
            Assert.AreEqual(3, count);

            values = new[] { c, c, a, b, c, c };
            actual = Measures.Mode(values, out count);
            Assert.AreEqual(c, actual);
            Assert.AreEqual(4, count);
        }

        [Test]
        public void WeightedModeTest_NonComparable()
        {
            object a = new object();
            object b = new object();
            object c = new object();

            object[] values;
            int[] weight;
            object actual;

            values = new[] { a, a, b, c };
            weight = new[] { 1, 1, 5, 1 };
            actual = Measures.WeightedMode(values, weight);
            Assert.AreEqual(b, actual);

            values = new[] { a, b, b, c };
            weight = new[] { 5, 1, 1, 1 };
            actual = Measures.WeightedMode(values, weight);
            Assert.AreEqual(a, actual);

            values = new[] { a, b, b, c };
            weight = new[] { 2, 1, 1, 5 };
            actual = Measures.WeightedMode(values, weight);
            Assert.AreEqual(c, actual);

            values = new[] { a, b, c, c };
            weight = new[] { 1, 1, 5, 5 };
            actual = Measures.WeightedMode(values, weight);
            Assert.AreEqual(c, actual);

            values = new[] { a, a, b, c };
            weight = new[] { 1, 1, 1, 1 };
            actual = Measures.WeightedMode(values, weight);
            Assert.AreEqual(a, actual);

            values = new[] { a, b, b, c };
            weight = new[] { 1, 1, 1, 1 };
            actual = Measures.WeightedMode(values, weight);
            Assert.AreEqual(b, actual);

            values = new[] { a, b, b, c };
            weight = new[] { 1, 1, 1, 1 };
            actual = Measures.WeightedMode(values, weight);
            Assert.AreEqual(b, actual);

            values = new[] { a, b, c, c };
            weight = new[] { 1, 1, 1, 1 };
            actual = Measures.WeightedMode(values, weight);
            Assert.AreEqual(c, actual);
        }

        [Test]
        public void ModeTest_Comparable()
        {
            int a = 1;
            int b = 10;
            int c = 100;

            int[] values;
            int actual;

            values = new[] { a, a, b, c };
            actual = Measures.Mode(values);
            Assert.AreEqual(a, actual);

            values = new[] { a, b, b, c };
            actual = Measures.Mode(values);
            Assert.AreEqual(b, actual);

            values = new[] { a, b, c, c };
            actual = Measures.Mode(values);
            Assert.AreEqual(c, actual);

            int count;
            values = new[] { a, b, c, a };
            actual = Measures.Mode(values, out count);
            Assert.AreEqual(a, actual);
            Assert.AreEqual(2, count);

            values = new[] { b, a, b, b, c };
            actual = Measures.Mode(values, out count);
            Assert.AreEqual(b, actual);
            Assert.AreEqual(3, count);

            values = new[] { c, c, a, b, c, c };
            actual = Measures.Mode(values, out count);
            Assert.AreEqual(c, actual);
            Assert.AreEqual(4, count);
        }

        [Test]
        public void WeightedModeTest1()
        {
            double[] values = { 0, 1, 1, 2 };
            double[] weights = { 50, 1, 1, 2 };

            double expected = 0;
            double actual = Measures.WeightedMode(values, weights);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WeightedModeTest2()
        {
            double[] values = { 0, 1, 1, 2 };
            double[] weights = { 0, 1, 1, 50 };

            double expected = 2;
            double actual = Measures.WeightedMode(values, weights);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WeightedCountsModeTest1()
        {
            double[] values = { 0, 1, 1, 2 };
            int[] weights = { 50, 1, 1, 2 };

            double expected = 0;
            double actual = Measures.WeightedMode(values, weights);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WeightedCountsModeTest2()
        {
            double[] values = { 0, 1, 1, 2 };
            int[] weights = { 0, 1, 1, 50 };

            double expected = 2;
            double actual = Measures.WeightedMode(values, weights);
            Assert.AreEqual(expected, actual);
        }


        [Test]
        public void CovarianceTest()
        {
            double[,] matrix = new double[,]
            {
                { 4.0, 2.0, 0.60 },
                { 4.2, 2.1, 0.59 },
                { 3.9, 2.0, 0.58 },
                { 4.3, 2.1, 0.62 },
                { 4.1, 2.2, 0.63 }
            };


            double[,] expected = new double[,]
            {
                { 0.02500, 0.00750, 0.00175 },
                { 0.00750, 0.00700, 0.00135 },
                { 0.00175, 0.00135, 0.00043 },
            };


            double[,] actual = Measures.Covariance(matrix);
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.0001));

            actual = Measures.Covariance(matrix, 0);
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.0001));


            matrix = matrix.Transpose();

            actual = Measures.Covariance(matrix, 1);
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.0001));
        }

        [Test]
        public void CovarianceTest2()
        {
            double[][] matrix = new double[,]
            {
                { 4.0, 2.0, 0.60 },
                { 4.2, 2.1, 0.59 },
                { 3.9, 2.0, 0.58 },
                { 4.3, 2.1, 0.62 },
                { 4.1, 2.2, 0.63 }
            }.ToJagged();


            double[,] expected = new double[,]
            {
                { 0.02500, 0.00750, 0.00175 },
                { 0.00750, 0.00700, 0.00135 },
                { 0.00175, 0.00135, 0.00043 },
            };


            double[][] actual = Measures.Covariance(matrix);
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.0001));

            actual = Measures.Covariance(matrix, 0);
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.0001));


            matrix = matrix.Transpose();

            actual = Measures.Covariance(matrix, 1);
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.0001));
        }

        [Test]
        public void CovarianceTest3()
        {
            double[][] matrix = new double[,]
            {
                { 4.0, 2.0, 0.60 },
                { 4.2, 2.1, 0.59 },
                { 3.9, 2.0, 0.58 },
                { 4.3, 2.1, 0.62 },
                { 4.1, 2.2, 0.63 }
            }.ToJagged();


            double[,] expected = new double[,]
            {
                { 0.02500, 0.00750, 0.00175 },
                { 0.00750, 0.00700, 0.00135 },
                { 0.00175, 0.00135, 0.00043 },
            };

            double[] weights = { 0.2, 0.2, 0.2, 0.2, 0.2 };

            double[,] actual = Measures.WeightedCovariance(matrix, weights, 0);
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.0001));

            matrix = matrix.Transpose();

            actual = Measures.WeightedCovariance(matrix, weights, 1);
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.0001));
        }

        [Test]
        public void CovarianceTest5()
        {
            double[][] matrix = new double[,]
            {
                { 4.0, 2.0, 0.60 },
                { 4.2, 2.1, 0.59 },
                { 3.9, 2.0, 0.58 },
                { 4.3, 2.1, 0.62 },
                { 4.1, 2.2, 0.63 }
            }.ToJagged();


            double[,] expected = new double[,]
            {
                { 0.02500, 0.00750, 0.00175 },
                { 0.00750, 0.00700, 0.00135 },
                { 0.00175, 0.00135, 0.00043 },
            };

            double[] weights = { 1, 1, 1, 1, 1 };

            double[] mean = Measures.WeightedMean(matrix, weights, 0);
            double[,] actual = Measures.WeightedCovariance(matrix, weights, mean);
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.0001));

            matrix = matrix.Transpose();

            mean = Measures.WeightedMean(matrix, weights, 1);
            actual = Measures.WeightedCovariance(matrix, weights, mean, 1);
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.0001));
        }

        [Test]
        public void CovarianceTest6()
        {
            double[][] matrix = new double[,]
            {
                { 4.0, 2.0, 0.60 },
                { 4.2, 2.1, 0.59 },
                { 3.9, 2.0, 0.58 },
                { 4.3, 2.1, 0.62 },
                { 4.1, 2.2, 0.63 }
            }.ToJagged();


            double[,] expected = new double[,]
            {
                { 0.02500, 0.00750, 0.00175 },
                { 0.00750, 0.00700, 0.00135 },
                { 0.00175, 0.00135, 0.00043 },
            };

            double[] weights = { 1, 1, 1, 1, 1 };

            double[,] actual = Measures.WeightedCovariance(matrix, weights, 0);
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.0001));

            matrix = matrix.Transpose();

            actual = Measures.WeightedCovariance(matrix, weights, 1);
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.0001));
        }


        [Test]
        public void CovarianceTest4()
        {
            double[] u = { -2, 1, 5 };
            double[] v = { 7, 1, 6 };

            double expected = -0.8333;

            double actual = Measures.Covariance(u, v);

            Assert.AreEqual(expected, actual, 0.0001);
        }

        [Test]
        public void VarianceTest7()
        {
            double[][] matrix = new double[,]
            {
                { 4.0, 2.0, 0.60 },
                { 4.2, 2.1, 0.59 },
                { 3.9, 2.0, 0.58 },
                { 4.3, 2.1, 0.62 },
                { 4.1, 2.2, 0.63 }
            }.ToJagged();

            double[] weights = { 0.9, 0.9, 0.9, 0.9, 0.9 };

            double[] expected =
            {
                Measures.WeightedVariance(matrix.GetColumn(0), weights),
                Measures.WeightedVariance(matrix.GetColumn(1), weights),
                Measures.WeightedVariance(matrix.GetColumn(2), weights),
            };


            double[] actual = Measures.WeightedVariance(matrix, weights);

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 1e-10));
        }

        [Test]
        public void VarianceTest8()
        {
            double[,] matrix =
            {
                { 4.0, 2.0, 0.60 },
                { 4.2, 2.1, 0.59 },
                { 3.9, 2.0, 0.58 },
                { 4.3, 2.1, 0.62 },
                { 4.1, 2.2, 0.63 }
            };

            double[] weights = { 0.9, 0.9, 0.9, 0.9, 0.9 };

            double[] expected =
            {
                Measures.WeightedVariance(matrix.GetColumn(0), weights, WeightType.Automatic),
                Measures.WeightedVariance(matrix.GetColumn(1), weights, WeightType.Automatic),
                Measures.WeightedVariance(matrix.GetColumn(2), weights, WeightType.Automatic),
            };


            double[] actual = Measures.WeightedVariance(matrix, weights, WeightType.Automatic);

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 1e-10));
        }

        [Test]
        public void CorrelationTest()
        {
            // http://www.solvemymath.com/online_math_calculator/statistics/descriptive/correlation.php

            #region doc_correlation
            // Let's say we have a matrix containing 5
            // samples (rows) of 3 dimensions (columns):
            double[,] matrix = new double[,]
            {
                { 4.0, 2.0, 0.60 },
                { 4.2, 2.1, 0.59 },
                { 3.9, 2.0, 0.58 },
                { 4.3, 2.1, 0.62 },
                { 4.1, 2.2, 0.63 }
            };

            // We can compute their correlation matrix using
            double[,] corr1 = Measures.Correlation(matrix);

            // The matrix should be equal to:
            double[,] expected = new double[,]
            {
                { 1.000000, 0.5669467, 0.533745 },
                { 0.566946, 1.0000000, 0.778127 },
                { 0.533745, 0.7781271, 1.000000 }
            };


            // The same could be repeated with a jagged matrix instead:
            double[][] jagged = new double[][]
            {
                new double[] { 4.0, 2.0, 0.60 },
                new double[] { 4.2, 2.1, 0.59 },
                new double[] { 3.9, 2.0, 0.58 },
                new double[] { 4.3, 2.1, 0.62 },
                new double[] { 4.1, 2.2, 0.63 }
            };

            // And the value would be the same:
            double[][] corr2 = Measures.Correlation(jagged);
            #endregion

            Assert.IsTrue(Matrix.IsEqual(expected, corr1, 1e-5));
            Assert.IsTrue(Matrix.IsEqual(expected, corr2, 1e-5));
        }




        [Test]
        public void ProportionsTest()
        {
            int[,] summary =
            {
                { 1, 4, 5 },
                { 2, 1, 3 },
            };

            double[,] probabilities =
            {
                { 1, 4.0/(4+5) },
                { 2, 1.0/(1+3) },
            };

            int[] positives = summary.GetColumn(1);
            int[] negatives = summary.GetColumn(2);
            double[] expected = probabilities.GetColumn(1);
            double[] actual;

            actual = Tools.Proportions(positives, negatives);
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.000000001));
        }

        [Test]
        public void GroupTest()
        {

            int[][] data = {
                 new int[] { 1, 1 },
                 new int[] { 1, 1 },
                 new int[] { 1, 1 },
                 new int[] { 1, 1 },
                 new int[] { 1, 0 },
                 new int[] { 1, 0 },
                 new int[] { 1, 0 },
                 new int[] { 1, 0 },
                 new int[] { 1, 0 },
                 new int[] { 2, 1 },
                 new int[] { 2, 0 },
                 new int[] { 2, 0 },
                 new int[] { 2, 0 },
            };

            int[][] expected = {
                 new int[] { 1, 4, 5 },
                 new int[] { 2, 1, 3 },
            };


            int[][] actual;
            actual = Tools.Group(data, 0, 1);

            Assert.IsTrue(Matrix.IsEqual(expected, actual));
        }

        [Test]
        public void ExtendTest()
        {
            int[,] summary =
            {
                { 1, 4, 5 },
                { 2, 1, 3 },
            };

            int[] group = summary.GetColumn(0);
            int[] positives = summary.GetColumn(1);
            int[] negatives = summary.GetColumn(2);

            int[][] expected =
            {
                 new int[] { 1, 1 },
                 new int[] { 1, 1 },
                 new int[] { 1, 1 },
                 new int[] { 1, 1 },
                 new int[] { 1, 0 },
                 new int[] { 1, 0 },
                 new int[] { 1, 0 },
                 new int[] { 1, 0 },
                 new int[] { 1, 0 },
                 new int[] { 2, 1 },
                 new int[] { 2, 0 },
                 new int[] { 2, 0 },
                 new int[] { 2, 0 },
            };

            int[][] actual;
            actual = Accord.Statistics.Tools.Expand(group, positives, negatives);
            Assert.IsTrue(Matrix.IsEqual(expected, actual));
        }

        [Test]
        public void ShuffleTest()
        {
            int[] array = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };

            Vector.Shuffle(array);

            for (int i = 0; i < 10; i++)
                Assert.IsTrue(array.Contains(i));
        }



        [Test]
        public void ExtendTest2()
        {
            int[][] data =
            {
                new int[] { 0, 2, 4 },
                new int[] { 1, 1, 2 },
            };

            int labelColumn = 0;
            int positiveColumn = 1;
            int negativeColumn = 2;

            int[][] expected =
            {
                // For label 0
                new int[] { 0, 1 }, // Two positive cases
                new int[] { 0, 1 },

                new int[] { 0, 0 }, // Four negative cases
                new int[] { 0, 0 },
                new int[] { 0, 0 },
                new int[] { 0, 0 },
                
                // For label 1
                new int[] { 1, 1 }, // One positive case
                new int[] { 1, 0 }, // Three negative cases
                new int[] { 1, 0 },
            };

            int[][] actual = Accord.Statistics.Tools.Expand(data, labelColumn, positiveColumn, negativeColumn);

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void KurtosisTest()
        {
            double[] values = { 3.4, 7.1, 1.5, 8.6, 4.9 };
            double expected = 1.6572340828415202; // definition used by STATA
            double actual = Measures.Kurtosis(values, false) + 3;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SkewnessTest()
        {
            double[] values = { 3.4, 7.1, 1.5, 8.6, 4.9 };
            double expected = -0.00861496080010664;
            double actual = Measures.Skewness(values, false);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void KurtosisTest2()
        {
            Accord.Math.Random.Generator.Seed = 0;
            double[] values = Accord.Math.Vector.Random(10000000);
            double actual = Measures.Kurtosis(values, true);
            Assert.AreEqual(-1.1997826610738631, actual);
        }

        [Test]
        public void SkewnessTest2()
        {
            Accord.Math.Random.Generator.Seed = 0;
            double[] values = Accord.Math.Vector.Random(10000000);
            double actual = Measures.Skewness(values, true);
            Assert.AreEqual(0.00046129386638486271, actual);
        }


        [Test]
        public void WhiteningTest()
        {
            double[,] value =
            {
                { 0.4218,    0.6557,    0.6787,    0.6555 },
                { 0.9157,    0.0357,    0.7577,    0.1712 },
                { 0.7922,    0.8491,    0.7431,    0.7060 },
                { 0.9595,    0.9340,    0.3922,    0.0318 },
            };

            double[,] I = Matrix.Identity(3);
            double[,] T = null; // transformation matrix

            // Perform the whitening transform
            double[,] actual = Tools.Whitening(value, out T);

            // Check if covariance matrix has the identity form
            double[,] cov = Measures.Covariance(actual).Submatrix(0, 2, 0, 2);
            Assert.IsTrue(cov.IsEqual(I, 1e-10));

            // Check if we can transform the data
            double[,] result = value.Dot(T);
            Assert.IsTrue(result.IsEqual(actual));
        }


        [Test]
        public void PooledVarianceTest()
        {
            double[][] samples =
            {
                new double[] { 31, 30, 29 },
                new double[] { 42, 41, 40, 39 },
                new double[] { 31, 28 },
                new double[] { 23, 22, 21, 19, 18 },
                new double[] { 21, 20, 19, 18,17 },
            };

            double expected = 2.7642857142857147;
            double actual = Measures.PooledVariance(samples);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PooledStandardDeviationTest()
        {
            double[][] samples =
            {
                new double[] { 31, 30, 29 },
                new double[] { 42, 41, 40, 39 },
                new double[] { 31, 28 },
                new double[] { 23, 22, 21, 19, 18 },
                new double[] { 21, 20, 19, 18,17 },
            };

            double expected = System.Math.Sqrt(2.7642857142857147);
            double actual = Measures.PooledStandardDeviation(samples);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void EntropyTest()
        {
            {
                int[] values = { 0, 1 };
                int classes = 2;

                double expected = 1;
                double actual = Measures.Entropy(values, classes);

                Assert.AreEqual(expected, actual);
            }
            {
                int[] values = { 0, 0 };
                int classes = 2;

                double expected = 0;
                double actual = Measures.Entropy(values, classes);

                Assert.AreEqual(expected, actual);
            }
            {
                int[] values = { 1, 1 };
                int classes = 2;

                double expected = 0;
                double actual = Measures.Entropy(values, classes);

                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void RankTest1()
        {
            double[] values = { 2, 3, 4, 4, 5, 6, 8, 10, 10, 14, 16, 20, 32, 40 };

            double[] expected = { 1, 2, 3.5, 3.5, 5, 6, 7, 8.5, 8.5, 10, 11, 12, 13, 14 };
            double[] actual = values.Rank();

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void RankTest2()
        {
            double[] values = { 7, 1, 2, 1, 7, 8, 1, 1, 2, 0, 10, 27 };
            double[] copy = (double[])values.Clone();

            double[] expected = { 8.5, 3.5, 6.5, 3.5, 8.5, 10.0, 3.5, 3.5, 6.5, 1.0, 11.0, 12.0 };
            double[] actual = values.Rank();

            Assert.IsTrue(expected.IsEqual(actual));
            Assert.IsTrue(copy.IsEqual(values));
        }

        [Test]
        public void RankTest3()
        {
            var sample1 = new double[] { 45, 45, 15, 50, 30, 15, 30, 35, 25 };
            var sample2 = new double[] { 45, 55, 20, 55, 20, 25, 35, 45, 20 };

            double[] samples = sample1.Concatenate(sample2);
            bool hasTies;
            double[] actual = samples.Rank(hasTies: out hasTies, adjustForTies: true);
            Assert.IsTrue(hasTies);

            double[] expected = { 13.5, 13.5, 1.5, 16.0, 8.5, 1.5, 8.5, 10.5, 6.5, 13.5, 17.5, 4.0, 17.5, 4.0, 6.5, 10.5, 13.5, 4.0 };
            Array.Sort(expected);
            Array.Sort(actual);

            Assert.IsTrue(expected.IsEqual(actual));
        }


        [Test]
        public void RankTest4()
        {
            double[] sample1 = new double[] { 250, 200, 450, 400, 250, 250, 350, 0, 200, 400, 300, 600, 200, 200,
                550, 100, 300, 250, 350, 200, 550, 200, 450, 400, 200, 400, 450,
                200, 400, 400, 500, 450, 300, 250, 200 };
            double[] sample2 = new double[] { 250, 200, 450, 400, 250, 250, 350, 0, 200, 400, 300, 600, 200, 200,
               550, 100, 300, 250, 350, 200, 550, 200, 450, 400, 200, 400, 450,
               200, 400, 400, 500, 450, 300, 250, 200 };

            double[] samples = sample1.Concatenate(sample2);

            bool hasTies;
            double[] actual = samples.Rank(hasTies: out hasTies, adjustForTies: true);
            Assert.IsTrue(hasTies);

            double[] expected =
            {
                27.5, 13.5, 58.5, 48.5, 27.5, 27.5, 40.5, 1.5, 13.5, 48.5, 35.5, 69.5, 13.5, 13.5, 66.5, 3.5, 35.5, 27.5, 40.5,
                13.5, 66.5, 13.5, 58.5, 48.5, 13.5, 48.5, 58.5, 13.5, 48.5, 48.5, 63.5, 58.5, 35.5, 27.5, 13.5, 27.5, 13.5, 58.5, 48.5,
                27.5, 27.5, 40.5, 1.5, 13.5, 48.5, 35.5, 69.5, 13.5, 13.5, 66.5, 3.5, 35.5, 27.5, 40.5, 13.5, 66.5, 13.5, 58.5, 48.5,
                13.5, 48.5, 58.5, 13.5, 48.5, 48.5, 63.5, 58.5, 35.5, 27.5, 13.5
            };

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void RankTest5()
        {
            double[] sample = new double[] { 250, 200, 450, 400, 250, 250, 350, 0, 200, 400, 300, 600, 200, 200,
                550, 100, 300, 250, 350, 200, 550, 200, 450, 400, 200, 400, 450,
                200, 400, 400, 500, 450, 300, 250, 200 };

            bool hasTies;
            double[] actual = sample.Rank(hasTies: out hasTies, adjustForTies: true);
            Assert.IsTrue(hasTies);

            double[] expected =
            {
                14.0, 7.0, 29.5, 24.5, 14.0, 14.0, 20.5, 1.0, 7.0, 24.5, 18.0, 35.0, 7.0, 7.0, 33.5,
                2.0, 18.0, 14.0, 20.5, 7.0, 33.5, 7.0, 29.5, 24.5, 7.0, 24.5, 29.5, 7.0, 24.5, 24.5,
                32.0, 29.5, 18.0, 14.0, 7.0
            };

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void test_rank_all_ties_odd()
        {
            double[] samples = new double[] { 0, 0, 0 };
            bool hasTies;
            double[] actual = samples.Rank(hasTies: out hasTies, adjustForTies: true);
            Assert.IsTrue(hasTies);
            double[] expected = { 2, 2, 2 };
            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void test_rank_all_ties_even()
        {
            double[] samples = new double[] { 0, 0, 0, 0 };
            bool hasTies;
            double[] actual = samples.Rank(hasTies: out hasTies, adjustForTies: true);
            Assert.IsTrue(hasTies);
            double[] expected = { 2.5, 2.5, 2.5, 2.5 };
            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void TiesTest1()
        {
            double[] rank = { 1, 2, 3.5, 3.5, 5, 6, 7, 8.5, 8.5, 10, 11, 12, 13, 14 };
            int[] actual = rank.Ties();
            double[] expected = { 1, 1, 2, 1, 1, 1, 2, 1, 1, 1, 1, 1 };
            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void MatrixModeTest2()
        {
            int[][] matrix =
            {
                new[] { 6, 4, 9 },
                new[] { 3, 1, 3 },
                new[] { 1, 3, 8 },
                new[] { 1, 5, 4 },
                new[] { 7, 4, 1 },
                new[] { 4, 4, 3 },
            };

            int[] expected = { 1, 4, 3 };
            int[] actual = Measures.Mode(matrix);

            Assert.AreEqual(expected[0], actual[0]);
            Assert.AreEqual(expected[1], actual[1]);
            Assert.AreEqual(expected[2], actual[2]);
        }

        [Test]
        public void MatrixModeTest3()
        {
            double[][] matrix =
            {
                new double[] { 6, 4, 9 },
                new double[] { 3, 1, 3 },
                new double[] { 1, 3, 8 },
                new double[] { 1, 5, 4 },
                new double[] { 7, 4, 1 },
                new double[] { 4, 4, 3 },
            };

            double[] expected = { 1, 4, 3 };
            double[] actual = Measures.Mode(matrix);

            Assert.AreEqual(expected[0], actual[0]);
            Assert.AreEqual(expected[1], actual[1]);
            Assert.AreEqual(expected[2], actual[2]);
        }

        [Test]
        public void ModeTest4()
        {
            int[] values = { 2, 5, 1, 6, 4, 1, 2, 6, 2, 6, 8, 2, 6, 2, 2 };
            int expected = 2;
            int actual = Measures.Mode(values);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void KurtosisMatrixTest1()
        {
            double[,] matrix =
            {
                { 1987 },
                { 1987 },
                { 1991 },
                { 1992 },
                { 1992 },
                { 1992 },
                { 1992 },
                { 1993 },
                { 1994 },
                { 1994 },
                { 1995 },
            };

            double expected = 0.4466489;
            {
                double[] actual = Measures.Kurtosis(matrix);
                Assert.AreEqual(expected, actual[0], 1e-7);
            }
            {
                double actual = Measures.Kurtosis(matrix.GetColumn(0));
                Assert.AreEqual(expected, actual, 1e-7);
            }
        }

        [Test]
        public void KurtosisMatrixTest2()
        {
            double[][] matrix =
            {
                new double[] { 1987 },
                new double[] { 1987 },
                new double[] { 1991 },
                new double[] { 1992 },
                new double[] { 1992 },
                new double[] { 1992 },
                new double[] { 1992 },
                new double[] { 1993 },
                new double[] { 1994 },
                new double[] { 1994 },
                new double[] { 1995 },
            };

            double expected = 0.4466489;
            {
                double[] actual = Measures.Kurtosis(matrix);
                Assert.AreEqual(expected, actual[0], 1e-7);
            }
            {
                double actual = Measures.Kurtosis(matrix.GetColumn(0));
                Assert.AreEqual(expected, actual, 1e-7);
            }
        }

        [Test]
        public void SkewnessMatrixTest1()
        {
            double[,] matrix =
            {
                { 180 },
                { 182 },
                { 169 },
                { 175 },
                { 178 },
                { 189 },
                { 174 },
                { 174 },
                { 171 },
                { 168 }
            };

            double expected = 0.77771377;
            {
                double[] actual = Measures.Skewness(matrix);
                Assert.AreEqual(expected, actual[0], 1e-7);
            }
            {
                double actual = Measures.Skewness(matrix.GetColumn(0));
                Assert.AreEqual(expected, actual, 1e-7);
            }
        }

        [Test]
        public void SkewnessMatrixTest()
        {
            double[][] matrix =
            {
                new double[] { 180 },
                new double[] { 182 },
                new double[] { 169 },
                new double[] { 175 },
                new double[] { 178 },
                new double[] { 189 },
                new double[] { 174 },
                new double[] { 174 },
                new double[] { 171 },
                new double[] { 168 }
            };

            double expected = 0.77771377;
            {
                double[] actual = Measures.Skewness(matrix);
                Assert.AreEqual(expected, actual[0], 1e-7);
            }
            {
                double actual = Measures.Skewness(matrix.GetColumn(0));
                Assert.AreEqual(expected, actual, 1e-7);
            }
        }
    }
}
