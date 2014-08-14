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

using Accord.Statistics.Kernels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Accord.Tests.Statistics
{
    
    
    /// <summary>
    ///This is a test class for SplineTest and is intended
    ///to contain all SplineTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SplineTest
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
        public void KernelTest()
        {
            Spline target = new Spline();

            double[] x = { 1, 2, 3 };
            double[] y = { 4, 5, 6 };
            double expected = 5.577138888888889e+03;
            double actual;

            actual = target.Function(x, y);
            Assert.AreEqual(expected, actual);


            double[] x2 = { 5, 11, 2 };
            double[] y2 = { 4, 5,  1 };
            expected = 3.331507407407407e+04;

            actual = target.Function(x2, y2);
            Assert.AreEqual(System.Math.Round(expected, 5),
                            System.Math.Round(actual, 5));
        }

    }
}
