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

using Accord.Math.Decompositions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accord.Math;

namespace Accord.Tests.Math
{


    /// <summary>
    ///This is a test class for CholeskyDecompositionTest and is intended
    ///to contain all CholeskyDecompositionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class NonnegativeMatrixFactorizationTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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
        public void NonNegativeMatrixFactorizationConstructorTest()
        {

            Accord.Math.Tools.SetupGenerator(0);

            double[,] X =
            {
                { 1,     0,     5 },
                { 1,     2,     1 },
                { 0,     6,     1 },
                { 2,     6,     5 },
                { 2,     1,     1 },
                { 5,     1,     1 }
            };


            var nmf = new NonnegativeMatrixFactorization(X, 3);

            var H = nmf.RightNonnegativeFactors;
            var W = nmf.LeftNonnegativeFactors;

            var R = W.Multiply(H).Transpose();
            Assert.IsTrue(R.IsEqual(X, 0.001));
        }

    }
}
