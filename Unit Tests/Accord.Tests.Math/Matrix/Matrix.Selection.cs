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
    using Accord.Math.Comparers;
    using Accord.Statistics.Distributions.Univariate;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using Accord.Statistics;

    public partial class MatrixTest
    {

        [Test]
        public void partition_success()
        {
            int[] a = { 2, 7, 3, 5, 4 };
            int pivot = Matrix.Partition(a, 0, a.Length);
            Assert.IsTrue(a.IsEqual(new[] { 2, 3, 4, 5, 7 }));
            // Assert.IsTrue(a.IsEqual(new[] { 2, 3, 7, 5, 4 }));

            a = new int[] { 7, 6, 5, 4, 3, 2, 1, 0 };
            pivot = Matrix.Partition(a, 0, a.Length);
            Assert.IsTrue(a.IsEqual(new[] { 0, 3, 2, 1, 4, 5, 7, 6 }));
            // Assert.IsTrue(a.IsEqual(new[] { 0, 2, 3, 1, 4, 6, 5, 7 }));

            a = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };
            Matrix.Partition(a, 0, a.Length);
            Assert.IsTrue(a.IsEqual(new[] { 0, 1, 2, 3, 4, 5, 6, 7 }));

            a = new int[] { 2, 7, 3, 4, 5, 6, 1, 8 };
            double expected = (int)Measures.Median(a.Submatrix(4, 7));
            int actual = Matrix.Partition(a, 4, 7);
            Assert.AreEqual(actual, expected);
            //Assert.IsTrue(a.IsEqual(new[] { 2, 7, 3, 4, 5, 1, 6, 8 }));
            Assert.IsTrue(a.IsEqual(new[] { 2, 7, 3, 4, 1, 5, 6, 8 }));
        }



        [Test]
        public void nth_element_0()
        {
            // http://www.codecogs.com/library/computing/stl/algorithms/sorting/nth_element.php
            int[] a = { 10, 2, 6, 11, 9, 3, 4, 12, 8, 7, 1, 5 };
            Sort.NthElement(a, 0, a.Length, 6);
            int[] expected = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            Assert.IsTrue(a.IsEqual(expected));
        }

        [Test]
        public void nth_element_1()
        {
            for (int size = 1; size < 1000; size += 10)
            {
                for (int nth = 1; nth < size; nth += 10)
                {
                    int[] random = UniformDiscreteDistribution.Random(size);
                    double[] a = random.ToDouble();
                    double[] b = random.ToDouble();
                    int[] bk = (int[])random.Clone();
                    double[] c = random.ToDouble();
                    double[] d = random.ToDouble();
                    int[] dk = (int[])random.Clone();
                    double[] e = random.ToDouble();
                    double[] f = random.ToDouble();
                    int[] fk = (int[])random.Clone();

                    Sort.NthElement(a, 0, a.Length - 1, nth);
                    Sort.NthElement(b, bk, 0, b.Length - 1, nth);
                    Assert.IsTrue(a.IsEqual(b));
                    Assert.IsTrue(a.IsEqual(bk));

                    Func<double, double, int> comparer = (x, y) => x.CompareTo(y);
                    Sort.NthElement(c, 0, a.Length - 1, nth, comparer);
                    Sort.NthElement(d, dk, 0, b.Length - 1, nth, comparer);
                    var str = random.ToString(CSharpArrayFormatProvider.InvariantCulture);
                    Assert.IsTrue(a.IsEqual(c));
                    Assert.IsTrue(c.IsEqual(d));
                    Assert.IsTrue(d.IsEqual(dk));

                    Func<double, double, int> comparer2 = (x, y) => -x.CompareTo(y);
                    Sort.NthElement(e, 0, a.Length - 1, nth, comparer2, asc: false);
                    Sort.NthElement(f, fk, 0, b.Length - 1, nth, comparer2, asc: false);
                    Assert.IsTrue(a.IsEqual(e));
                    Assert.IsTrue(e.IsEqual(f));
                    Assert.IsTrue(f.IsEqual(fk));

                    Assert.IsTrue(a.IsEqual(c));
                    Assert.IsTrue(a.IsEqual(d));
                    Assert.IsTrue(a.IsEqual(e));
                    Assert.IsTrue(a.IsEqual(f));
                    Assert.IsTrue(a.IsEqual(fk));
                }
            }
        }

        [Test]
        public void nth_element_with_repetition()
        {
            for (int size = 1; size < 1000; size += 10)
            {
                for (int nth = 1; nth < size; nth += 10)
                {
                    int[] random = UniformDiscreteDistribution.Random(0, 10, size);
                    double[] a = random.ToDouble();
                    double[] b = random.ToDouble();
                    int[] bk = (int[])random.Clone();
                    double[] c = random.ToDouble();
                    double[] d = random.ToDouble();
                    int[] dk = (int[])random.Clone();
                    double[] e = random.ToDouble();
                    double[] f = random.ToDouble();
                    int[] fk = (int[])random.Clone();

                    Sort.NthElement(a, 0, a.Length - 1, nth);
                    Sort.NthElement(b, bk, 0, b.Length - 1, nth);
                    Assert.IsTrue(a.IsEqual(b));
                    Assert.IsTrue(a.IsEqual(bk));

                    Func<double, double, int> comparer = (x, y) => x.CompareTo(y);
                    Sort.NthElement(c, 0, a.Length - 1, nth, comparer);
                    Sort.NthElement(d, dk, 0, b.Length - 1, nth, comparer);
                    var str = random.ToString(CSharpArrayFormatProvider.InvariantCulture);
                    Assert.IsTrue(a.IsEqual(c));
                    Assert.IsTrue(c.IsEqual(d));
                    Assert.IsTrue(d.IsEqual(dk));

                    Func<double, double, int> comparer2 = (x, y) => -x.CompareTo(y);
                    Sort.NthElement(e, 0, a.Length - 1, nth, comparer2, asc: false);
                    Sort.NthElement(f, fk, 0, b.Length - 1, nth, comparer2, asc: false);
                    Assert.IsTrue(a.IsEqual(e));
                    Assert.IsTrue(e.IsEqual(f));
                    Assert.IsTrue(f.IsEqual(fk));

                    Assert.IsTrue(a.IsEqual(c));
                    Assert.IsTrue(a.IsEqual(d));
                    Assert.IsTrue(a.IsEqual(e));
                    Assert.IsTrue(a.IsEqual(f));
                    Assert.IsTrue(a.IsEqual(fk));
                }
            }
        }


        [Test]
        public void nth_element_2()
        {
            // Example from http://en.cppreference.com/w/cpp/algorithm/nth_element

            double[] a = { 5, 6, 4, 3, 2, 6, 7, 9, 3 };

            Sort.NthElement(a, 0, a.Length, a.Length / 2);
            double median = a[a.Length / 2];
            Assert.AreEqual(5, median);

            double r = Sort.NthElement(a, 0, a.Length, 1, asc: false);
            Assert.AreEqual(a[1], 7);
            Assert.AreEqual(r, 7);
        }

        [Test]
        public void nth_element_3()
        {
            // Example from http://www.tenouk.com/cpluscodesnippet/cplusstlvectoralgorithmnth_element.html

            double[] a = { 0, 1, 2, 3, 4, 5, 10, 11, 12, 13, 14, 15, 20, 21, 22, 23, 24, 25 };
            Sort.NthElement(a, 0, a.Length, 3);
            double[] expected = { 0, 1, 2, 3, 4, 5, 10, 11, 12, 13, 14, 15, 20, 21, 22, 23, 24, 25 };
            Assert.IsTrue(a.IsEqual(expected));

            Func<double, double, int> greater = (double elem1, double elem2) =>
            {
                // Less than zero: This instance is less than value.
                // Zero: This instance is equal to value.
                // Greater than zero: This instance is greater than value.
                int ret = -elem1.CompareTo(elem2);
                return ret;
            };

            Sort.NthElement(a, 0, a.Length, 4, greater);
            expected = new double[] { 25, 24, 23, 22, 21, 20, 15, 14, 13, 12, 11, 10, 5, 4, 3, 2, 1, 0 };
            Assert.IsTrue(a.IsEqual(expected));

            Vector.Shuffle(a);
            Sort.NthElement(a, 0, a.Length, 4, greater);
            expected = new double[] { 25, 24, 23, 22, 21, 20, 15, 14, 13, 12, 11, 10, 5, 4, 3, 2, 1, 0 };
            Assert.IsTrue(a.IsEqual(expected));
        }

        [Test]
        public void partial_test_1()
        {
            // Example from https://github.com/accord-net/framework/issues/865

            double[][] data = {
                new double[] { 18, 31, 25, 2, 22, 13, 37, 1, 4, 7, 6, 45, 10, 24, 23, 49, 27, 9, 35,
                  14, 34, 33, 41, 42, 20, 43, 3, 48, 15, 39, 11, 38, 46, 17, 40, 16,
                  50, 29, 19, 47, 12, 28, 32, 8, 30, 26, 5, 44, 36, 21 },
                new double[] { 18,14,1,15,4,32,10,26,38,9,24,16,31,20,25,30,22,6,28,21,33,17,5,35,2,13,36,8,29,7 }
            };

            foreach (var v in data)
            {
                double[] sorted;

                sorted = v.Sorted();
                for (int i = 0; i < v.Length; i++)
                {
                    double[] copy = v.Copy();
                    Sort.Partial(copy, i);

                    for (int j = 0; j <= i; j++)
                        Assert.AreEqual(sorted[j], copy[j]);
                }

                sorted = v.Sorted();
                for (int i = 0; i < v.Length; i++)
                {
                    double[] copy = v.Copy();
                    int[] idx = Vector.Range(copy.Length);
                    Sort.Partial(copy, idx, i);

                    for (int j = 0; j <= i; j++)
                        Assert.AreEqual(sorted[j], copy[j]);

                    Assert.AreEqual(copy, v.Get(idx));
                }

                sorted = v.Sorted(asc: false);
                for (int i = 0; i < v.Length; i++)
                {
                    double[] copy = v.Copy();
                    Sort.Partial(copy, i, asc: false);

                    for (int j = 0; j <= i; j++)
                        Assert.AreEqual(sorted[j], copy[j]);
                }

                sorted = v.Sorted(asc: false);
                for (int i = 0; i < v.Length; i++)
                {
                    double[] copy = v.Copy();
                    int[] idx = Vector.Range(copy.Length);
                    Sort.Partial(copy, idx, i, asc: false);

                    for (int j = 0; j <= i; j++)
                        Assert.AreEqual(sorted[j], copy[j]);

                    Assert.AreEqual(copy, v.Get(idx));
                }
            }
        }

        [Test]
        public void bottom_1()
        {
            Accord.Math.Random.Generator.Seed = 0;

            for (int size = 1; size < 1000; size *= 10)
            {
                for (int n = 1; n < size; n += 10)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        int[] random = UniformDiscreteDistribution.Random(size);
                        int[] sorted = random.Sorted();

                        int[] expectedBottom = sorted.First(n);
                        int[] bottomIdx = random.Bottom(n);
                        int[] actualBottom = random.Submatrix(bottomIdx);
                        Assert.IsTrue(actualBottom.IsEqual(expectedBottom));

                        int[] expectedTop = sorted.Last(n).Reversed();
                        int[] topIdx = random.Top(n);
                        int[] actualTop = random.Submatrix(topIdx);
                        Assert.IsTrue(actualTop.IsEqual(expectedTop));
                    }
                }
            }
        }

        [Test]
        public void insertion_sort_success()
        {
            for (int n = 1; n < 100; n += 10)
            {
                for (int i = 0; i < 100; i++)
                {
                    int[] random = UniformDiscreteDistribution.Random(n);
                    double[] expected = random.ToDouble();
                    double[] a = random.ToDouble();
                    double[] b = random.ToDouble();
                    double[] c = random.ToDouble();
                    double[] d = random.ToDouble();
                    double[] e = random.ToDouble();

                    Array.Sort(expected);
                    Sort.Insertion(a);
                    Sort.Insertion(b, asc: false);
                    b = b.Reversed();
                    Sort.Insertion(c, asc: true);

                    Func<double, double, int> greater = (x, y) => -x.CompareTo(y);
                    Sort.Insertion(d, comparer: greater, asc: false);
                    Sort.Insertion(e, comparer: greater, asc: true);
                    e = e.Reversed();

                    Assert.IsTrue(expected.IsEqual(a));
                    Assert.IsTrue(expected.IsEqual(b));
                    Assert.IsTrue(expected.IsEqual(c));
                    Assert.IsTrue(expected.IsEqual(d));
                    Assert.IsTrue(expected.IsEqual(e));
                }
            }
        }

        [Test]
        public void insertion_sort_std()
        {
            int[] idx = { 5, 1, 2, 3, 4, 0 };
            double[][] points =
            {
                new double[] { 2, 3 },
                new double[] { 5, 4 },
                new double[] { 9, 6 },
                new double[] { 4, 7 },
                new double[] { 8, 1 },
                new double[] { 7, 2 },
            };

            var comparisons = new List<Tuple<double, double>>();
            double[] e = points[idx[0]];
            Func<double[], double[], int> comparer = (double[] x, double[] y) =>
            {
                var d1 = Distance.Euclidean(e, x);
                var d2 = Distance.Euclidean(e, y);
                comparisons.Add(Tuple.Create(d1, d2));
                return d1.CompareTo(d2);
            };

            Sort.NthElement(points, idx,
                  first: 1,
                  last: 5,
                  n: 2,
                  compare: comparer);

            int[] expected = { 5, 4, 1, 2, 3, 0 };
            Assert.IsTrue(idx.IsEqual(expected));
            Assert.AreEqual(5, comparisons.Count);

            Assert.AreEqual(4.47213595499958, comparisons[0].Item1);
            Assert.AreEqual(2.8284271247461903, comparisons[0].Item2);

            Assert.AreEqual(4.47213595499958, comparisons[1].Item1);
            Assert.AreEqual(2.8284271247461903, comparisons[1].Item2);

            Assert.AreEqual(5.8309518948453007, comparisons[2].Item1);
            Assert.AreEqual(2.8284271247461903, comparisons[2].Item2);

            Assert.AreEqual(5.8309518948453007, comparisons[3].Item1);
            Assert.AreEqual(4.47213595499958, comparisons[3].Item2);

            Assert.AreEqual(1.4142135623730951, comparisons[4].Item1);
            Assert.AreEqual(2.8284271247461903, comparisons[4].Item2);
        }

        [Test]
        public void insertion_sort_with_items_success()
        {
            for (int n = 1; n < 100; n += 10)
            {
                for (int i = 0; i < 100; i++)
                {
                    int[] random = UniformDiscreteDistribution.Random(n);
                    double[] expected = random.ToDouble();
                    double[] a = random.ToDouble();
                    long[] ai = random.ToInt64();
                    double[] b = random.ToDouble();
                    long[] bi = random.ToInt64();
                    double[] c = random.ToDouble();
                    long[] ci = random.ToInt64();
                    double[] d = random.ToDouble();
                    long[] di = random.ToInt64();
                    double[] e = random.ToDouble();
                    long[] ei = random.ToInt64();

                    Array.Sort(expected);
                    Sort.Insertion(a, ai);
                    Sort.Insertion(b, bi, asc: false);
                    b = b.Reversed();
                    bi = bi.Reversed();
                    Sort.Insertion(c, ci, asc: true);

                    Func<double, double, int> greater = (x, y) => -x.CompareTo(y);
                    Sort.Insertion(d, di, comparer: greater, asc: false);
                    Sort.Insertion(e, ei, comparer: greater, asc: true);
                    e = e.Reversed();
                    ei = ei.Reversed();

                    Assert.IsTrue(a.IsSorted());
                    Assert.IsTrue(a.IsSorted(ComparerDirection.Ascending));
                    Assert.IsTrue(expected.IsEqual(a));
                    Assert.IsTrue(expected.IsEqual(b));
                    Assert.IsTrue(expected.IsEqual(c));
                    Assert.IsTrue(expected.IsEqual(d));
                    Assert.IsTrue(expected.IsEqual(e));

                    Assert.IsTrue(expected.IsEqual(ai));
                    Assert.IsTrue(expected.IsEqual(bi));
                    Assert.IsTrue(expected.IsEqual(ci));
                    Assert.IsTrue(expected.IsEqual(di));
                    Assert.IsTrue(expected.IsEqual(ei));
                }
            }
        }

        [Test]
        public void insertion_sort_with_repetition()
        {
            for (int n = 1; n < 100; n += 10)
            {
                for (int i = 0; i < 100; i++)
                {
                    int[] random = UniformDiscreteDistribution.Random(0, 10, n);
                    long[] idx = Vector.Range((long)n);
                    double[] expected = random.ToDouble();
                    double[] a = random.ToDouble();
                    long[] ai = Vector.Range((long)n);
                    double[] b = random.ToDouble();
                    long[] bi = Vector.Range((long)n);
                    double[] c = random.ToDouble();
                    long[] ci = Vector.Range((long)n);
                    double[] d = random.ToDouble();
                    long[] di = Vector.Range((long)n);
                    double[] e = random.ToDouble();
                    long[] ei = Vector.Range((long)n);

                    Array.Sort(expected, idx);
                    Sort.Insertion(a, ai);
                    Sort.Insertion(b, bi, asc: false);
                    b = b.Reversed();
                    bi = bi.Reversed();
                    Sort.Insertion(c, ci, asc: true);

                    Func<double, double, int> greater = (x, y) => -x.CompareTo(y);
                    Sort.Insertion(d, di, comparer: greater, asc: false);
                    Sort.Insertion(e, ei, comparer: greater, asc: true);
                    e = e.Reversed();
                    ei = ei.Reversed();

                    Assert.IsTrue(a.IsSorted());
                    Assert.IsTrue(a.IsSorted(ComparerDirection.Ascending));
                    Assert.IsTrue(expected.IsEqual(a));
                    Assert.IsTrue(expected.IsEqual(b));
                    Assert.IsTrue(expected.IsEqual(c));
                    Assert.IsTrue(expected.IsEqual(d));
                    Assert.IsTrue(expected.IsEqual(e));

                    Assert.IsTrue(ai.IsEqual(ci));
                    Assert.IsTrue(ai.IsEqual(di));
                    Assert.IsTrue(bi.IsEqual(ei));
                }
            }
        }

        [Test]
        public void vector_get()
        {
            int[] a = { 2, 7, 3, 5, 4 };

            Assert.AreEqual(new[] { 7, 3 }, a.Get(1, 3));
            Assert.AreEqual(new[] { 4 }, a.Get(-1, 0));
            Assert.AreEqual(new[] { 3, 5, 4 }, a.Get(-3, 0));
            Assert.AreEqual(new[] { 2, 7, 3, 5 }, a.Get(0, -1));
        }

        [Test]
        public void matrix_get()
        {
            int[,] a =
            {
                { 2, 7, 3, 5, 4 },
                { 1, 2, 6, 8, 9 }
            };

            int[,] actual;

            actual = a.Get(0, 2, 1, 3);
            Assert.IsTrue(new[,] { { 7, 3 },
                                   { 2, 6 } }.IsEqual(actual));

            actual = a.Get(0, -1, -1, 0);
            Assert.IsTrue(new[,] { { 4 } }.IsEqual(actual));

            actual = a.Get(0, 1, -3, 0);
            Assert.IsTrue(new[,] { { 3, 5, 4 } }.IsEqual(actual));

            actual = a.Get(-1, 0, 0, -1);
            Assert.IsTrue(new[,] { { 1, 2, 6, 8 } }.IsEqual(actual));
        }

        [Test]
        public void jagged_get()
        {
            int[][] a =
            {
                new[] { 2, 7, 3, 5, 4 },
                new[] { 1, 2, 6, 8, 9 }
            };

            int[][] actual;

            actual = a.Get(0, 2, 1, 3);
            Assert.IsTrue(new[] { new[] { 7, 3 },
                                    new[] { 2, 6 } }.IsEqual(actual));

            actual = a.Get(0, -1, -1, 0);
            Assert.IsTrue(new[] { new[] { 4 } }.IsEqual(actual));

            actual = a.Get(0, 1, -3, 0);
            Assert.IsTrue(new[] { new[] { 3, 5, 4 } }.IsEqual(actual));

            actual = a.Get(-1, 0, 0, -1);
            Assert.IsTrue(new[] { new[] { 1, 2, 6, 8 } }.IsEqual(actual));
        }

        [Test]
        public void matrix_sort()
        {
            #region doc_matrix_sort
            // Let's say we are in a situation where you have a NxM matrix of values and 
            // a row vector 1xM where each value in the vector is associated with a column 
            // in the matrix (e.g. this is the case if we had a matrix of Eigenvectors and 
            // their associated eigenvalues):

            double[,] matrix =
            {
                { 1, 5, 3 },
                { 2, 2, 6 },
                { 1, 3, 4 },
                { 2, 4, 3 },
                { 1, 0, 1 },
            };

            double[] v = { -3, 1, 0.42 };

            // Let's say we would like to sort the columns of the matrix 
            // according to the value of the elements in the row vector:

            double[,] sortedMatrix1 = Matrix.Sort(keys: v, values: matrix);

            // The resulting matrix will be:
            double[,] expected1 =
            {
                { 1, 3, 5 },
                { 2, 6, 2 },
                { 1, 4, 3 },
                { 2, 3, 4 },
                { 1, 1, 0 },
            };

            // Now, let's say we would like to sort the columns of the matrix 
            // according to the absolute value of the elements in the row vector:

            double[,] sortedMatrix2 = Matrix.Sort(keys: v, values: matrix, 
                comparer: new GeneralComparer(ComparerDirection.Ascending, useAbsoluteValues: true));

            // The resulting matrix will be:
            double[,] expected2 =
            {
                { 3, 5, 1 },
                { 6, 2, 2 },
                { 4, 3, 1 },
                { 3, 4, 2 },
                { 1, 0, 1 },
            };
            #endregion

            Assert.AreEqual(expected1, sortedMatrix1);
            Assert.AreEqual(expected2, sortedMatrix2);
        }
    }
}
