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
    using Accord.Statistics.Analysis;
    using Accord.Statistics.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class McNemarTestTest
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
        public void McNemarTestConstructorTest()
        {
            int[,] matrix = 
            {
                { 101, 121 },
                {  59,  33 },
            };

            ConfusionMatrix a = new ConfusionMatrix(matrix);

            McNemarTest target = new McNemarTest(a, true);

            Assert.AreEqual(21.0125, target.Statistic);
            Assert.AreEqual(1, target.DegreesOfFreedom);

            McNemarTest target2 = new McNemarTest(a, false);

            Assert.AreEqual(21.355555, target2.Statistic, 1e-5);
            Assert.AreEqual(1, target2.DegreesOfFreedom);
        }


    }
}
