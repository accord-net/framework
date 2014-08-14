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


    [TestClass]
    public class GaborTest
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



        [TestMethod]
        public void KernelTest1()
        {
            double[,] expected =
            {
                { 2.1693514414478006E-34,	   2.4546349229561677E-22, -1.5279500064746295E-21    },
                { -0.00000069069240704724283,  0.9210609940028851,     -0.00000012799017494227224 },
		        { -7.7578659683414284E-22,    -4.1079449607342235E-22,  3.2663608516186217E-34    }
            };

            double[,] actual = Gabor.Kernel2D(sigma: 0.1, theta: 0.2, lambda: 0.3, psi: 0.4, gamma: 0.5);

            Assert.AreEqual(actual.GetLength(0), expected.GetLength(0));
            Assert.AreEqual(actual.GetLength(1), expected.GetLength(1));

            for (int i = 0; i < actual.GetLength(0); i++)
            {
                for (int j = 0; j < actual.GetLength(1); j++)
                {
                    double e = expected[i, j];
                    double a = actual[i, j];

                    Assert.AreEqual(e, a, System.Math.Abs(e * 1e-10));
                }
            }
        }

        [TestMethod]
        public void KernelTest2()
        {
            double[,] expected = 
            {
                { 1.42405394857622E-52, -1.55301965707389E-39, -5.91718738982049E-28, 6.74066784260594E-19, 9.45212732580885E-13, -1.54728187127383E-07, 1.35216035740648E-05 },
                { 2.44396261647309E-27, 6.22350869845861E-18, -2.53179283832159E-10, 4.71970195306204E-06, 0.01750857512698, -0.0562293027375883, -0.000676655483164679 },
                { -1.18923149745881E-11, 3.26086352470611E-05, -0.0380349460494781, -0.653643620863612, 0.0705347070196069, 1.49826805190377E-06, -8.02838283000017E-11 },
                { -0.00143882468734695, 0.0594224852404454, 0.00675479754684435, -2.97718145336781E-05, 8.09821092370491E-11, 1.01520928946922E-17, -3.06243455885214E-27 },
                { -2.89102012691001E-05, 2.93060843736615E-09, 4.32869511491687E-12, -4.97862860483868E-19, -4.04134720949385E-28, 4.26508052356691E-39, -1.65106988580208E-53 } 
            };

            double[,] actual = Gabor.Kernel2D(sigma: 1, theta: 2, lambda: 3, psi: 4, gamma: 5);

            Assert.AreEqual(actual.GetLength(0), expected.GetLength(0));
            Assert.AreEqual(actual.GetLength(1), expected.GetLength(1));

            for (int i = 0; i < actual.GetLength(0); i++)
            {
                for (int j = 0; j < actual.GetLength(1); j++)
                {
                    double e = expected[i, j];
                    double a = actual[i, j];

                    Assert.AreEqual(e, a, System.Math.Abs(e * 1e-10));
                }
            }
        }
    }
}
