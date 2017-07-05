﻿// Accord Unit Tests
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
    using System.Collections.Generic;
    using System;
    using Accord;
    using System.IO;

    [TestFixture]
    public class MatrixFormatTest
    {

        [Test]
        public void ParseTest1()
        {
            // Parsing a matrix from Octave format
            double[,] a = Matrix.Parse("[1 2; 3 4]",
                OctaveMatrixFormatProvider.InvariantCulture);

            // Creating a 2 x 2 identity matrix
            double[,] I = Matrix.Identity(size: 2);

            // Matrix multiplication
            double[,] b = Matrix.Multiply(a, I);

            Assert.AreEqual(1, b[0, 0]);
            Assert.AreEqual(2, b[0, 1]);
            Assert.AreEqual(3, b[1, 0]);
            Assert.AreEqual(4, b[1, 1]);
        }


        [Test]
        public void ParseTest()
        {
            string str;

            double[,] expected, actual;

            expected = new double[,] 
            {
                { 1, 2 },
                { 3, 4 },
            };


            str = "[1 2; 3 4]";
            actual = Matrix.Parse(str, OctaveMatrixFormatProvider.InvariantCulture);
            Assert.IsTrue(actual.IsEqual(expected));

            str = "1 2\r\n3 4";
            actual = Matrix.Parse(str, DefaultMatrixFormatProvider.InvariantCulture);
            Assert.IsTrue(actual.IsEqual(expected));

            str = @"1 2
                    3 4";
            actual = Matrix.Parse(str, DefaultMatrixFormatProvider.InvariantCulture);
            Assert.IsTrue(actual.IsEqual(expected));

            str = @"double[,]
                   {
                      { 1, 2 },
                      { 3, 4 }
                   };";
            actual = Matrix.Parse(str, CSharpMatrixFormatProvider.InvariantCulture);
            Assert.IsTrue(actual.IsEqual(expected));

            str = @"double[][]
                   {
                      new double[] { 1, 2 },
                      new double[] { 3, 4 }
                   };";
            actual = Matrix.Parse(str, CSharpJaggedMatrixFormatProvider.InvariantCulture);
            Assert.IsTrue(actual.IsEqual(expected));
        }

        [Test]
        public void ParseJaggedTest()
        {
            string str;

            double[][] expected, actual;

            expected = new double[][] 
            {
                new double[] { 1, 2 },
                new double[] { 3, 4 },
            };


            str = "[1 2; 3 4]";
            actual = Matrix.ParseJagged(str, OctaveMatrixFormatProvider.InvariantCulture);
            Assert.IsTrue(actual.IsEqual(expected));

            str = "1 2\r\n3 4";
            actual = Matrix.ParseJagged(str, DefaultMatrixFormatProvider.InvariantCulture);
            Assert.IsTrue(actual.IsEqual(expected));

            str = @"1 2
                    3 4";
            actual = Matrix.ParseJagged(str, DefaultMatrixFormatProvider.InvariantCulture);
            Assert.IsTrue(actual.IsEqual(expected));

            str = @"double[,]
                   {
                      { 1, 2 },
                      { 3, 4 }
                   };";
            actual = Matrix.ParseJagged(str, CSharpMatrixFormatProvider.InvariantCulture);
            Assert.IsTrue(actual.IsEqual(expected));

            str = @"double[][]
                   {
                      new double[] { 1, 2 },
                      new double[] { 3, 4 }
                   };";
            actual = Matrix.ParseJagged(str, CSharpJaggedMatrixFormatProvider.InvariantCulture);
            Assert.IsTrue(actual.IsEqual(expected));
        }

        [Test]
        public void ToStringTest()
        {
            double[,] matrix = 
            {
                { 1, 2 },
                { 3, 4 },
            };

            string expected, actual;

            expected = "[1 2; 3 4]";
            actual = Matrix.ToString(matrix, OctaveMatrixFormatProvider.InvariantCulture);
            Assert.AreEqual(expected, actual);


            expected = String.Format("1 2 {0}3 4", Environment.NewLine);
            actual = Matrix.ToString(matrix, DefaultMatrixFormatProvider.InvariantCulture);
            Assert.AreEqual(expected, actual);


            expected = String.Format("new double[][] {{{0}" +
                       "    new double[] {{ 1, 2 }},{0}" +
                       "    new double[] {{ 3, 4 }} {0}" +
                       "}};", Environment.NewLine);
            actual = Matrix.ToString(matrix, CSharpJaggedMatrixFormatProvider.InvariantCulture);
            Assert.AreEqual(expected, actual);


            expected = String.Format("new double[,] {{{0}" +
                       "    {{ 1, 2 }},{0}" +
                       "    {{ 3, 4 }} {0}" +
                       "}};", Environment.NewLine);
            actual = Matrix.ToString(matrix, CSharpMatrixFormatProvider.InvariantCulture);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToStringTest2()
        {
            double[][] matrix = 
            {
                new double[] { 1, 2 },
                new double[] { 3, 4 },
            };

            string expected, actual;

            expected = "[1 2; 3 4]";
            actual = Matrix.ToString(matrix, OctaveMatrixFormatProvider.InvariantCulture);
            Assert.AreEqual(expected, actual);


            expected = "1 2 " + Environment.NewLine + "3 4";
            actual = Matrix.ToString(matrix, DefaultMatrixFormatProvider.InvariantCulture);
            Assert.AreEqual(expected, actual);


            expected = "new double[][] {" + Environment.NewLine +
                       "    new double[] { 1, 2 }," + Environment.NewLine +
                       "    new double[] { 3, 4 } " + Environment.NewLine +
                       "};";
            actual = Matrix.ToString(matrix, CSharpJaggedMatrixFormatProvider.InvariantCulture);
            Assert.AreEqual(expected, actual);


            expected = "new double[,] {" + Environment.NewLine +
                       "    { 1, 2 }," + Environment.NewLine +
                       "    { 3, 4 } " + Environment.NewLine +
                       "};";
            actual = Matrix.ToString(matrix, CSharpMatrixFormatProvider.InvariantCulture);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void StringFormat()
        {
            double[,] matrix = 
            {
                { 1, 2 },
                { 3, 4 },
            };

            string expected, actual;

            expected = "[1 2; 3 4]";
            actual = String.Format(OctaveMatrixFormatProvider.InvariantCulture, "{0:Ms}", matrix);
            Assert.AreEqual(expected, actual);


            expected = "1 2 3 4";
            actual = String.Format(DefaultMatrixFormatProvider.InvariantCulture, "{0:Ms}", matrix);
            Assert.AreEqual(expected, actual);


            expected = "new double[][] { new double[] { 1, 2 }, new double[] { 3, 4 } };";
            actual = String.Format(CSharpJaggedMatrixFormatProvider.InvariantCulture, "{0:Ms}", matrix);
            Assert.AreEqual(expected, actual);


            expected = "new double[,] { { 1, 2 }, { 3, 4 } };";
            actual = String.Format(CSharpMatrixFormatProvider.InvariantCulture, "{0:Ms}", matrix);
            Assert.AreEqual(expected, actual);



            expected = "[1.00 2.00; 3.00 4.00]";
            actual = String.Format(OctaveMatrixFormatProvider.InvariantCulture, "{0:Ms,N2}", matrix);
            Assert.AreEqual(expected, actual);


            expected = "1.00 2.00 3.00 4.00";
            actual = String.Format(DefaultMatrixFormatProvider.InvariantCulture, "{0:Ms,N2}", matrix);
            Assert.AreEqual(expected, actual);


            expected = "new double[][] { new double[] { 1.00, 2.00 }, new double[] { 3.00, 4.00 } };";
            actual = String.Format(CSharpJaggedMatrixFormatProvider.InvariantCulture, "{0:Ms,N2}", matrix);
            Assert.AreEqual(expected, actual);


            expected = "new double[,] { { 1.00, 2.00 }, { 3.00, 4.00 } };";
            actual = String.Format(CSharpMatrixFormatProvider.InvariantCulture, "{0:Ms,N2}", matrix);
            Assert.AreEqual(expected, actual);

        }

        [Test]
        public void StringFormat2()
        {
            double[][] matrix = 
            {
                new double[] { 1, 2 },
                new double[] { 3, 4 },
            };

            string expected, actual;


            expected = "[1.00 2.00; 3.00 4.00]";
            actual = String.Format(OctaveMatrixFormatProvider.InvariantCulture, "{0:Ms,N2}", matrix as Array);
            Assert.AreEqual(expected, actual);

            expected = "1.00 2.00 3.00 4.00";
            actual = String.Format(DefaultMatrixFormatProvider.InvariantCulture, "{0:Ms,N2}", matrix as Array);
            Assert.AreEqual(expected, actual);

            expected = "new double[][] { new double[] { 1.00, 2.00 }, new double[] { 3.00, 4.00 } };";
            actual = String.Format(CSharpJaggedMatrixFormatProvider.InvariantCulture, "{0:Ms,N2}", matrix as Array);
            Assert.AreEqual(expected, actual);

            expected = "new double[,] { { 1.00, 2.00 }, { 3.00, 4.00 } };";
            actual = String.Format(CSharpMatrixFormatProvider.InvariantCulture, "{0:Ms,N2}", matrix as Array);
            Assert.AreEqual(expected, actual);

        }

        [Test]
        public void ToStringTest3()
        {
            double[] x = { 1, 2, 3 };

            String str;

            str = x.ToString(DefaultArrayFormatProvider.CurrentCulture);

            Assert.AreEqual("1 2 3", str);

            str = x.ToString(OctaveArrayFormatProvider.CurrentCulture);

            Assert.AreEqual("[1 2 3]", str);

            str = x.ToString(CSharpArrayFormatProvider.CurrentCulture);

            Assert.AreEqual("new double[] { 1, 2, 3 };", str);
        }

        [Test]
        public void vector_parse_test()
        {
            double[][] ex = new Accord.IO.CsvReader(new StringReader(Properties.Resources.data16), hasHeaders: false).ToJagged<double>();
            int[] ey = new Accord.IO.CsvReader(new StringReader(Properties.Resources.labels16), hasHeaders: false).ToJagged<int>().GetColumn(0);

            double[][] ax = Jagged.Parse(Properties.Resources.data16);
            double[] ay = Vector.Parse(Properties.Resources.labels16);

            Assert.IsTrue(ex.IsEqual(ax));
            Assert.IsTrue(ey.IsEqual(ay));
        }
    }
}
