// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
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
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class NormTest
    {


        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }



        [TestMethod()]
        public void EuclideanTest()
        {
            double[,] a = 
            {
                { 15.4457, 0.4187, 15.6093 },
                {  0.0000, 2.5708,  0.6534 }
            };


            double[] expected = 
            {
                21.9634, 2.6525
            };

            double[] actual = Norm.Euclidean(a, 1);
            Assert.IsTrue(expected.IsEqual(actual, 0.001));


            double[] expected2 = 
            {
                15.4457, 2.6047, 15.6229
            };

            double[] actual2 = Norm.Euclidean(a, 0);
            Assert.IsTrue(expected2.IsEqual(actual2, 0.001));

            double actual3 = Norm.Euclidean(a.GetRow(0));
            Assert.AreEqual(21.9634, actual3, 0.001);
        }

        [TestMethod()]
        public void Norm2Test()
        {
            double[,] a = 
            {
                { 2,     1,     5 },
                { 2,     2,     2 },
                { 1,     6,     4 }
            };

            double expected = 9.071111071571606;
            double actual = Norm.Norm2(a);
            Assert.AreEqual(expected, actual, 1e-12);
        }

        [TestMethod()]
        public void Norm1Test()
        {
            double[,] a = 
            {
                { 2,     1,     5 },
                { 2,     2,     2 },
                { 1,     6,     4 }
            };

            double expected = 11;
            double actual = Norm.Norm1(a);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void FrobeniusTest()
        {
            double[,] a = Matrix.Magic(5);

            double expected = 74.330343736592520;
            double actual = Norm.Frobenius(a);

            Assert.AreEqual(expected, actual, 1e-12);

        }

        [TestMethod()]
        public void EuclideanTest1()
        {
            float[,] a = 
            {
                { 15.4457f, 0.4187f, 15.6093f },
                {  0.0000f, 2.5708f,  0.6534f }
            };


            float[] expected = 
            {
                21.9634f, 2.6525f
            };

            float[] actual = Norm.Euclidean(a, 1);
            Assert.IsTrue(expected.IsEqual(actual, 0.001f));


            float[] expected2 = 
            {
                15.4457f, 2.6047f, 15.6229f
            };

            float[] actual2 = Norm.Euclidean(a, 0);
            Assert.IsTrue(expected2.IsEqual(actual2, 0.001f));

            float actual3 = Norm.Euclidean(a.GetRow(0));
            Assert.AreEqual(21.9634f, actual3, 0.001);
        }

    }
}
