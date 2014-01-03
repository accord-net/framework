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
    using Accord.Math.Decompositions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Math;

    [TestClass()]
    public class GramSchmidtTest
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
        public void ConstructorTest1()
        {
            double[,] value = 
            {
               {  4, -2 },
               {  3,  1 },
            };

            var svd = new SingularValueDecomposition(value);
            Assert.AreEqual(2, svd.Rank);


            var target = new GramSchmidtOrthogonalization(value);

            var Q = target.OrthogonalFactor;
            var R = target.UpperTriangularFactor;

            double[,] inv = Q.Inverse();
            double[,] transpose = Q.Transpose();
            double[,] m = Q.Multiply(transpose);

            Assert.IsTrue(Matrix.IsEqual(m, Matrix.Identity(2), 1e-10));
            Assert.IsTrue(Matrix.IsEqual(inv, transpose, 1e-10));
        }

    }
}
