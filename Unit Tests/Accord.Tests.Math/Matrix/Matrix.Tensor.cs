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
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Accord.Math;
    using Accord.Math.Decompositions;
    using AForge;
    using NUnit.Framework;
    using Accord.IO;

    [TestFixture]
    public partial class MatrixTensor
    {

        [Test]
        public void squeeze_by_conversion()
        {
            double[,,,] a =
            {
                { { { 1 } }, { { 2 } }, { { 3 } } },
                { { { 4 } }, { { 5 } }, { { 6 } } },
            };

            double[,] expected =
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
            };

            object actual = a.To<double[,]>();
            Assert.AreEqual(expected, actual);

            actual = a.To<double[,,]>();
            Assert.AreNotEqual(expected, actual);

            actual = a.To<double[,,]>().To<double[,]>();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void jagged_to_multidimensional()
        {
            double[][] a = Jagged.Magic(3);

            double[,] expected = Matrix.Magic(3);

            object actual = a.To<double[,]>();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void string_to_multidimensional()
        {
            string[][] a = Jagged.Magic(3).Apply((x, i, j) => x.ToString());

            double[,] expected = Matrix.Magic(3);

            object actual = a.To<double[,]>();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void string_to_jagged()
        {
            string[][] a = Jagged.Magic(3).Apply((x, i, j) => x.ToString());

            double[][] expected = Jagged.Magic(3);

            object actual = a.To<double[][]>();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void expand_and_squeeze()
        {
            double[,] a =
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
            };


            double[,,] actual0 = a.ExpandDimensions(0).To<double[,,]>();
            double[,,] actual1 = a.ExpandDimensions(1).To<double[,,]>();
            double[,,] actual2 = a.ExpandDimensions(2).To<double[,,]>();

            double[,,] expected0 =
            {
                { { 1, 2, 3 },
                  { 4, 5, 6 } },
            };

            double[,,] expected1 =
            {
                { { 1, 2, 3 } },
                { { 4, 5, 6 } },
            };

            double[,,] expected2 =
            {
                  { { 1 }, { 2 }, { 3 } },
                  { { 4 }, { 5 }, { 6 } },
            };


            Assert.AreEqual(actual0, expected0);
            Assert.AreEqual(actual1, expected1);
            Assert.AreEqual(actual2, expected2);

            // test squeeze
            Assert.AreEqual(a, expected0.Squeeze());
            Assert.AreEqual(a, expected1.Squeeze());
            Assert.AreEqual(a, expected2.Squeeze());
        }

        [Test]
        public void flatten_and_reshape()
        {
            double[] expected, actual;
            double[,] a =
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
            };

            expected = a.Flatten(MatrixOrder.CRowMajor);
            actual = (double[])((Array)a).Flatten(MatrixOrder.CRowMajor);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(a, actual.Reshape(a.GetLength(), MatrixOrder.CRowMajor));
            Assert.AreEqual(a, ((Array)actual).Reshape(a.GetLength(), MatrixOrder.CRowMajor));

            expected = a.Flatten(MatrixOrder.FortranColumnMajor);
            actual = (double[])((Array)a).Flatten(MatrixOrder.FortranColumnMajor);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(a, actual.Reshape(a.GetLength(), MatrixOrder.FortranColumnMajor));
            Assert.AreEqual(a, ((Array)actual).Reshape(a.GetLength(), MatrixOrder.FortranColumnMajor));


            double[,,,] b =
            {
                { { { 1 }, { 2 }, { 3 } } },
                { { { 4 }, { 5 }, { 6 } } },
            };

            expected = a.Flatten(MatrixOrder.CRowMajor);
            actual = (double[])((Array)b).Flatten(MatrixOrder.CRowMajor);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(a, actual.Reshape(a.GetLength(), MatrixOrder.CRowMajor));
            Assert.AreEqual(a, ((Array)actual).Reshape(a.GetLength(), MatrixOrder.CRowMajor));

            expected = a.Flatten(MatrixOrder.FortranColumnMajor);
            actual = (double[])((Array)b).Flatten(MatrixOrder.FortranColumnMajor);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(a, actual.Reshape(a.GetLength(), MatrixOrder.FortranColumnMajor));
            Assert.AreEqual(a, ((Array)actual).Reshape(a.GetLength(), MatrixOrder.FortranColumnMajor));
        }

        [Test]
        public void transpose()
        {
            double[,,,] target =
            {
                { { { 1 }, { 2 }, { 3 } } },
                { { { 4 }, { 5 }, { 6 } } },
            };

            double[,,,] expected =
            {
                { { { 1, 4 } } ,
                  { { 2, 5 } } ,
                  { { 3, 6 } } },
            };

            Array actual = target.Transpose();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void convert_with_squeeze()
        {
            double[,,,] a =
            {
                { { { 1 } }, { { 2 } }, { { 3 } } },
                { { { 4 } }, { { 5 } }, { { 6 } } },
            };

            int[,] expected =
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
            };

            int[,] actual = a.To<int[,]>();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void convert_to_scalar()
        {
            double[,,,] a =
            {
                { { { 1 } } },
            };

            int expected = 1;
            int actual = a.To<int>();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void convert_to_bool_true()
        {
            double[,,,] a =
            {
                { { { 1 } } },
            };

            bool expected = true;
            bool actual = a.To<bool>();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void convert_to_bool_false()
        {
            double[,,,] a =
            {
                { { { 0 } } },
            };

            bool expected = false;
            bool actual = a.To<bool>();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void slice_dimension()
        {
            double[,] x =
            {
                { 0, 1 },
                { 1, 2 },
                { 2, 3 },
                { 3, 4 },
                { 4, 5 },
                { 5, 6 },
                { 6, 7 },
                { 7, 8 },
                { 8, 9 },
                { 9, 0 },
            };


            {
                double[,] r = (double[,])Matrix.Get(x, dimension: 0, indices: new[] { 0, 1 });

                double[,] expected =
                {
                    { 0, 1 },
                    { 1, 2 },
                };

                Assert.AreEqual(expected, r);
            }

            {
                double[,] r = (double[,])Matrix.Get(x, dimension: 0, indices: new[] { 1, 2, 3 });

                double[,] expected =
                {
                    { 1, 2 },
                    { 2, 3 },
                    { 3, 4 },
                };

                Assert.AreEqual(expected, r);
            }

            {
                double[,] r = (double[,])Matrix.Get(x, dimension: 0, indices: new[] { 9, 6, 3, 2 });

                double[,] expected =
                {
                    { 9, 0 },
                    { 6, 7 },
                    { 3, 4 },
                    { 2, 3 },
                };

                Assert.AreEqual(expected, r);
            }
        }

        [Test]
        public void create_as()
        {
            double[,] a =
            {
                {1, 2, 3 },
                {3, 4, 5 }
            };

            Array actual = Jagged.CreateAs(a, typeof(int));
            Assert.AreEqual(new[] { 2, 3 }, actual.GetLength());

            actual = Jagged.CreateAs(a.ToJagged(), typeof(int));
            Assert.AreEqual(new[] { 2, 3 }, actual.GetLength());

            actual = Matrix.CreateAs(a, typeof(int));
            Assert.AreEqual(new[] { 2, 3 }, actual.GetLength());

            actual = Matrix.CreateAs(a.ToJagged(), typeof(int));
            Assert.AreEqual(new[] { 2, 3 }, actual.GetLength());
        }

    }
}
