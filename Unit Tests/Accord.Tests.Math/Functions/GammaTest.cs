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
    public class GammaTest
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
        public void FunctionTest()
        {
            double[] x = 
            {
                1.0, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 1.8, 1.9, 2.0,
                3.1, 5.7, 56.2, 53.8, 5.1, 6.5, 8.8, 114.2, 1024.6271,
               -2, -52.1252, -0.10817480950786047, -0.11961291417237133,
                -0.12078223763524518, 0, 281982742.12985912, 0.5212392
            };

            double[] expected =    
            {
                  1.000000000000000e+00, 0.9513507698668732, 9.181687423997607e-01,
                  8.974706963062772e-01, 8.872638175030753e-01, 8.862269254527581e-01,
                  8.935153492876903e-01, 9.086387328532904e-01, 9.313837709802427e-01, 
                  9.617658319073874e-01, 1.000000000000000e+00, 2.197620278392477e+00,
                  7.252763452022295e+01, 2.835938400359957e+73, 1.929366760161528e+69,
                  2.793175373836837e+01, 2.878852778150444e+02, 2.633998635450860e+04,
                  5.749274244634086e+184, Double.PositiveInfinity, Double.NaN,
                 -6.188338737526232e-68, -9.940515795403039e+00,  -9.070713053754153e+00,
                 -8.991245623853780e+00, Double.NaN, Double.PositiveInfinity, 1.701905559094028e+00

            };

            for (int i = 0; i < x.Length; i++)
            {
                double xi = x[i];
                double expectedi = expected[i];

                if (Double.IsNaN(expectedi))
                {
                    bool thrown = false;
                    try { Gamma.Function(xi); }
                    catch { thrown = true; }
                    Assert.IsTrue(thrown);
                }
                else
                {
                    double actual = Gamma.Function(xi);

                    Assert.AreEqual(expectedi, actual, System.Math.Abs(expectedi) * 1e-12);
                }
            }
        }


        [TestMethod()]
        public void LogTest()
        {
            double[] x = 
            {
                1.0, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 1.8, 1.9, 2.0,
                3.1, 5.7, 56.2, 53.8, 5.1, 6.5, 8.8, 114.2, 1024.6271,
               -2, -52.1252, -0.10817480950786047, -0.11961291417237133,
                -0.12078223763524518, 0, 281982742.12985912, 0.5212392
            };

            double[] expected =    
            {
                 0.000000000000000e+00,  -0.04987244125983974, -8.537409000331581e-02,
                -1.081748095078605e-01, -1.196129141723713e-01, -1.207822376352452e-01,
                -1.125917656967558e-01, -9.580769740706588e-02, -7.108387291437214e-02, 
                -3.898427592308334e-02,  0.000000000000000e+00,  7.873750832738625e-01,
                 4.283967655031580e+00,  1.691310846763928e+02,  1.595355632621249e+02,
                 3.329764168475224e+00,  5.662562059857142e+00,  1.017884345724507e+01,
                 4.254247307394230e+02,  6.075627024736053e+03,  Double.PositiveInfinity,
                 -1.547531196513472e+02,  2.296618910207815e+00,  2.205050877768351e+00,
                 2.196251395487650e+00,  Double.PositiveInfinity,  5.204655969482103e+09,
                 5.317485404177827e-01
            };

            for (int i = 0; i < x.Length; i++)
            {
                double xi = x[i];
                double expectedi = expected[i];

                if (Double.IsNaN(expectedi) || Double.IsInfinity(expectedi))
                {
                    bool thrown = false;
                    try { Gamma.Function(xi); }
                    catch { thrown = true; }
                    Assert.IsTrue(thrown);
                }
                else
                {
                    double actual = Gamma.Log(xi);

                    Assert.AreEqual(expectedi, actual, System.Math.Abs(expectedi) * 1e-14);
                }
            }
        }


        [TestMethod()]
        public void DigammaTest()
        {
            double[] x = { 1, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 1.8, 1.9, 2.0 };

            double[] expected =    
            {
                -0.5772156649015329,
                -0.4237549404110768, 
                -0.2890398965921883,  
                -0.1691908888667997,
                -0.06138454458511615,
                 0.03648997397857652,
                 0.1260474527734763,
                 0.208547874873494,
                 0.2849914332938615,
                 0.3561841611640597,
                 0.4227843350984671,
            };

            for (int i = 0; i < x.Length; i++)
            {
                double xi = x[i];
                double expectedi = expected[i];
                double actual = Gamma.Digamma(xi);

                Assert.AreEqual(expectedi, actual, 1e-10);
            }
        }

        [TestMethod()]
        public void GammaFunctionsTest()
        {
            double x = 0.17;
            double gamma = Gamma.Function(x); // 5.4511741801042106
            double gammap = Gamma.Multivariate(x, p: 2); // -39.473585841300675
            double log = Gamma.Log(x);        // 1.6958310313607003
            double logp = Gamma.Log(x, p: 2); // 3.6756317353404273
            double stir = Gamma.Stirling(x);  // 24.040352622960743
            double psi = Gamma.Digamma(x);    // -6.2100942259248626
            double tri = Gamma.Trigamma(x);   // 35.915302055854525

            double a = 4.2;
            double lower = Gamma.LowerIncomplete(a, x); // 0.000015685073063633753
            double upper = Gamma.UpperIncomplete(a, x); // 0.9999843149269364


            Assert.AreEqual(0.9999843149269364, upper);
            Assert.AreEqual(0.000015685073063633753, lower);

            Assert.AreEqual(5.4511741801042106, gamma);
            Assert.AreEqual(-39.473585841300675, gammap);
            Assert.AreEqual(1.6958310313607003, log);
            Assert.AreEqual(3.6756317353404273, logp);
            Assert.AreEqual(24.040352622960743, stir);
            Assert.AreEqual(-6.2100942259248626, psi);
            Assert.AreEqual(35.915302055854525, tri);
        }

        [TestMethod()]
        public void GammaTest2()
        {
            double x = 171;
            double expected = 7.257415615308056e+306;
            double actual = Gamma.Function(x);
            Assert.AreEqual(expected, actual, 1e+293);
        }

        [TestMethod()]
        public void LgammaTest()
        {
            double x = 57;
            double expected = 172.35279713916282;

            double actual = Gamma.Log(x);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void DigammaTest2()
        {
            double x = 42;
            double expected = 3.7257176179372822;
            double actual = Gamma.Digamma(x);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void MultivariateGammaTest()
        {
            double expected = 35.342917352885181;
            double actual = Gamma.Multivariate(4, 2);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GammaUpperRTest()
        {
            // Example values from
            // http://opensource.zyba.com/code/maths/special/gamma/gamma_upper_reg.php

            double expected, actual;

            actual = Gamma.UpperIncomplete(0.000000, 2);
            expected = 1.000000;
            Assert.AreEqual(expected, actual);
            Assert.IsFalse(double.IsNaN(actual));

            actual = Gamma.UpperIncomplete(0.250000, 2);
            expected = 0.017286;
            Assert.AreEqual(expected, actual, 1e-6);
            Assert.IsFalse(double.IsNaN(actual));

            actual = Gamma.UpperIncomplete(0.500000, 2);
            expected = 0.045500;
            Assert.AreEqual(expected, actual, 1e-6);
            Assert.IsFalse(double.IsNaN(actual));

            actual = Gamma.UpperIncomplete(0.750000, 2);
            expected = 0.085056;
            Assert.AreEqual(expected, actual, 1e-6);
            Assert.IsFalse(double.IsNaN(actual));

            actual = Gamma.UpperIncomplete(1.000000, 2);
            expected = 0.135335;
            Assert.AreEqual(expected, actual, 1e-6);
            Assert.IsFalse(double.IsNaN(actual));

            actual = Gamma.UpperIncomplete(1.250000, 2);
            expected = 0.194847;
            Assert.AreEqual(expected, actual, 1e-6);
            Assert.IsFalse(double.IsNaN(actual));

            actual = Gamma.UpperIncomplete(1.500000, 2);
            expected = 0.261464;
            Assert.AreEqual(expected, actual, 1e-6);
            Assert.IsFalse(double.IsNaN(actual));

            actual = Gamma.UpperIncomplete(1.750000, 2);
            expected = 0.332706;
            Assert.AreEqual(expected, actual, 1e-6);
            Assert.IsFalse(double.IsNaN(actual));

            actual = Gamma.UpperIncomplete(2.000000, 2);
            expected = 0.406006;
            Assert.AreEqual(expected, actual, 1e-6);
            Assert.IsFalse(double.IsNaN(actual));

            actual = Gamma.UpperIncomplete(2.250000, 2);
            expected = 0.478944;
            Assert.AreEqual(expected, actual, 1e-6);
            Assert.IsFalse(double.IsNaN(actual));

            actual = Gamma.UpperIncomplete(2.500000, 2);
            expected = 0.549416;
            Assert.AreEqual(expected, actual, 1e-6);
            Assert.IsFalse(double.IsNaN(actual));

            actual = Gamma.UpperIncomplete(2.750000, 2);
            expected = 0.615734;
            Assert.AreEqual(expected, actual, 1e-6);
            Assert.IsFalse(double.IsNaN(actual));
        }
    }
}
