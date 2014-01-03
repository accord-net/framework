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

namespace Accord.Tests.Statistics
{
    using Accord.Statistics.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    
    [TestClass()]
    public class ChiSquareTestTest
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
        public void ConstructorTest()
        {
            double[] observed = { 639, 241 };
            double[] expected = { 660, 220 };

            int degreesOfFreedom = 1;
            var chi = new ChiSquareTest(expected, observed, degreesOfFreedom);

            Assert.AreEqual(2.668, chi.Statistic, 0.015);
            Assert.AreEqual(1, chi.DegreesOfFreedom);
            Assert.AreEqual(0.1020, chi.PValue, 1e-4);
        }

        [TestMethod()]
        public void ConstructorTest2()
        {
            double[] observed = { 6,    6,    16,    15,    4,    3};
            double[] expected = { 6.24, 5.76, 16.12, 14.88, 3.64, 3.36};

            int degreesOfFreedom = 2;
            var chi = new ChiSquareTest(expected, observed, degreesOfFreedom);

            Assert.AreEqual(0.0952, chi.Statistic, 0.001);
            Assert.AreEqual(2, chi.DegreesOfFreedom);
            Assert.AreEqual(false, chi.Significant);
        }
    }
}
