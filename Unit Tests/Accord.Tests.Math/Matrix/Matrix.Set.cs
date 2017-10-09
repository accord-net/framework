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

    public partial class MatrixTest
    {

        [Test]
        public void set_value_specific1()
        {
            int[,] m;
            int[,] expected;

            m = Vector.Interval(0, 12).Reshape(3, 4);
            m.Set(10, null, 2, -1); // tested against numpy
            expected = new int[,]
            {
                { 0, 1, 10, 3 },
                 { 4, 5, 10, 7 },
                 { 8, 9, 10, 11 },
            };
            Assert.AreEqual(expected, m);

            m = Vector.Interval(0, 12).Reshape(3, 4).Transpose();
            m.Set(10, 2, -1, null); // tested against numpy
            expected = new int[,]
            {
                { 0, 1, 10, 3 },
                 { 4, 5, 10, 7 },
                 { 8, 9, 10, 11 },
            }.Transpose();
            Assert.AreEqual(expected, m);
        }

        [Test]
        public void set_value()
        {
            int[,] m;
            int[,] expected;

            m = Vector.Interval(0, 12).Reshape(3, 4);
            m.Set(10);
            expected = new int[,]
            {
                 { 10, 10, 10, 10 },
                 { 10, 10, 10, 10 },
                 { 10, 10, 10, 10 },
            };
            Assert.AreEqual(expected, m);

            m = Vector.Interval(0, 12).Reshape(3, 4);
            m.Set(10, null, 2, 3);
            expected = new int[,]
            {
                 { 0, 1, 10, 3 },
                 { 4, 5, 10, 7 },
                 { 8, 9, 10, 11 },
            };
            Assert.AreEqual(expected, m);

            m = Vector.Interval(0, 12).Reshape(3, 4);
            m.Set(10, 2, 3, null);
            expected = new int[,]
            {
                { 0, 1, 2, 3 },
                { 4, 5, 6, 7 },
                { 10, 10, 10, 10 },
            };
            Assert.AreEqual(expected, m);

            m = Vector.Interval(0, 12).Reshape(3, 4);
            m.Set(10, 0, 1,
                      0, 2);
            expected = new int[,]
            {
                { 10, 10, 2, 3 },
                { 4, 5, 6, 7 },
                { 8, 9, 10, 11 },
            };
            Assert.AreEqual(expected, m);

            m.Set(42, new[] { 2 }, new[] { 3 });
            expected = new int[,]
            {
                 { 10, 10, 2, 3 },
                 { 4, 5, 6, 7 },
                 { 8, 9, 10, 42 },
            };
            Assert.AreEqual(expected, m);


            m = Vector.Interval(0, 12).Reshape(3, 4);
            m.Set(10, null, 2, -1);
            expected = new int[,]
            {
                { 0, 1, 10, 3 },
                 { 4, 5, 10, 7 },
                 { 8, 9, 10, 11 },
            };
            Assert.AreEqual(expected, m);

            m = Vector.Interval(0, 12).Reshape(3, 4).Transpose();
            m.Set(10, 2, -1, null);
            expected = new int[,]
            {
                { 0, 1, 10, 3 },
                 { 4, 5, 10, 7 },
                 { 8, 9, 10, 11 },
            }.Transpose();
            Assert.AreEqual(expected, m);

            m = Vector.Interval(0, 12).Reshape(3, 4);
            m.Set(10, 1, -1, null);
            expected = new int[,]
            {
                { 0, 1, 2, 3 },
                { 10, 10, 10, 10 },
                { 8, 9, 10, 11 },
            };
            Assert.AreEqual(expected, m);

            m.Set(10, 0, -1,
                      0, -2);
            expected = new int[,]
            {
                { 10, 10, 2, 3 },
                { 10, 10, 10, 10 },
                { 8, 9, 10, 11 },
            };
            Assert.AreEqual(expected, m);

            m = Vector.Interval(0, 12).Reshape(3, 4);
            m.Set(42, new[] { -1 }, new[] { -2 });
            expected = new int[,]
            {
                { 0, 1, 2, 3 },
                { 4, 5, 6, 7 },
                { 8, 9, 42, 11 },
            };
            Assert.AreEqual(expected, m);
        }

        //[Test]
        //public void set_matrix()
        //{
        //    int[,] m = Vector.Interval(0, 12).Reshape(3, 4);
        //    int[,] expected;

        //    m.Set(new[,] {
        //        { 10, 20 },
        //        { 30, 40 } });

        //    m.Set(new[,] {
        //        { 10, 20 },
        //        { 30, 40 } }, );

        //    m.Set(new[,] {
        //        { 10, 20 },
        //        { 30, 40 } });

        //    m.Set(new[,] {
        //        { 10, 20 },
        //        { 30, 40 } });
        //}

        [Test]
        public void set_vector()
        {
            int[] m = Vector.Range(0, 5);
            int[] expected;

            m.Set(42, -1);
            expected = new int[]
            {
                0, 1, 2, 3, 42
            };
            Assert.AreEqual(expected, m);

            m.Set(10, 0);
            expected = new int[]
            {
                10, 1, 2, 3, 42
            };
            Assert.AreEqual(expected, m);

            m.Set(42, new[] { 4, 2 });
            expected = new int[]
            {
                10, 1, 42, 3, 42
            };
            Assert.AreEqual(expected, m);
        }

    }
}
