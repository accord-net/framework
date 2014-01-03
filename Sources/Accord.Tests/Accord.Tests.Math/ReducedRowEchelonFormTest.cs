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
    using System;
    
    [TestClass()]
    public class ReducedRowEchelonFormTest
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
        public void ReducedRowEchelonFormConstructorTest()
        {
            double[,] matrix = 
            {
                { 1, 2, -3 },
                { 3, 5,  9 },
                { 5, 9,  3 },
            };

            ReducedRowEchelonForm target = new ReducedRowEchelonForm(matrix);

            var actual = target.Result;
            double[,] expected = 
            {
                { 1, 0,  33 },
                { 0, 1, -18 },
                { 0, 0,   0 },
            };


            Assert.IsTrue(expected.IsEqual(actual));
        }

        [TestMethod()]
        public void ReducedRowEchelonFormConstructorTest2()
        {
            double[,] matrix = 
            {
                {3,2,2,3,1},
                {6,4,4,6,2},
                {9,6,6,9,1},
            };

            ReducedRowEchelonForm target = new ReducedRowEchelonForm(matrix);

            var actual = target.Result;

            double[,] expected = 
            {
                { 1, 2/3.0,  2/3.0,   1,   0   },
                { 0,     0,      0,   0,   1   },
                { 0,     0,      0,   0,   0   },
            };


            Assert.IsTrue(expected.IsEqual(actual));
        }
   
    }
}
