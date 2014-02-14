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
    using Accord.Math;

    [TestClass()]
    public class KappaTestTest
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
        public void KappaTestConstructorTest()
        {
            // Example from http://vassarstats.net/kappa.html

            // Checked against http://graphpad.com/quickcalcs/Kappa2.cfm     (OK)

            int[,] matrix =
            {
                { 44,  5,  1 },
                {  7, 20,  3 },
                {  9,  5,  6 },
            };

            GeneralConfusionMatrix a = new GeneralConfusionMatrix(matrix);

            Assert.AreEqual(a.RowTotals[0], 50);
            Assert.AreEqual(a.RowTotals[1], 30);
            Assert.AreEqual(a.RowTotals[2], 20);

            Assert.AreEqual(a.ColumnTotals[0], 60);
            Assert.AreEqual(a.ColumnTotals[1], 30);
            Assert.AreEqual(a.ColumnTotals[2], 10);


            Assert.AreEqual(0.4915, a.Kappa, 1e-4);
            Assert.IsFalse(double.IsNaN(a.Kappa));

            double var = a.Variance;
            double var0 = a.VarianceUnderNull;
            double varD = Accord.Statistics.Testing.KappaTest.DeltaMethodKappaVariance(a);

            double se = System.Math.Sqrt(var);
            double se0 = System.Math.Sqrt(var0);
            double seD = System.Math.Sqrt(varD);

            Assert.AreEqual(0.072, a.StandardError, 0.0005);

            // Create a test of the null hypothesis (actual k = 0)
            KappaTest target = new KappaTest(a, hypothesizedKappa: 0);

            // Std. Error is computed differently under the null hypothesis:
            Assert.AreEqual(0.073509316753225237, target.StandardError, 1e-5);
            Assert.IsFalse(double.IsNaN(target.StandardError));
        }

        [TestMethod()]
        public void KappaTestConstructorTest2()
        {
            // Example from: M. Reichenheim (2004). Confidence intervals for the
            // kappa statistic. The Stata Journal (2004) 4, Number 4, pp. 421–428.
            // http://www.stata-journal.com/sjpdf.html?articlenum=st0076

            int[,] matrix = // (pg 599)
            {
                { 48,  12 },
                { 16, 160 },
            };

            GeneralConfusionMatrix a = new GeneralConfusionMatrix(matrix);

            // Reichenheim's paper shows:
            // Agreement | Expected Agreement | Kappa  | Std. Error |   Z
            //   88.14%  |       61.25%       | 0.6938 |   0.0650   | 10.67

            Assert.AreEqual(88.14, a.OverallAgreement * 100, 1e-2);
            Assert.AreEqual(61.25, a.ChanceAgreement * 100, 1e-2);
            Assert.AreEqual(0.6938, a.Kappa, 1e-4);
            Assert.AreEqual(0.0650, a.StandardErrorUnderNull, 1e-4);

            KappaTest target = new KappaTest(a);

            Assert.AreEqual(OneSampleHypothesis.ValueIsGreaterThanHypothesis, target.Hypothesis);

            Assert.AreEqual(10.67, target.Statistic, 1e-3);
            Assert.AreEqual(0, target.PValue);
        }

        [TestMethod()]
        public void KappaTestConstructorTest4()
        {
            // Example from Statistical Methods for Rates and Proportions

            // Checked against http://graphpad.com/quickcalcs/Kappa2.cfm     (OK)
            // Checked against Statistical Methods for Rates and Proportions (OK)
            // Checked against http://vassarstats.net/kappa.html           (FAIL)

            int[,] matrix = // (pg 599)
            {
                { 75,  1, 4  },
                {  5,  4, 1  },
                {  0,  0, 10 },
            };

            GeneralConfusionMatrix a = new GeneralConfusionMatrix(matrix);

            Assert.AreEqual(100, a.Samples);

            Assert.AreEqual(80, a.RowTotals[0]);
            Assert.AreEqual(10, a.RowTotals[1]);
            Assert.AreEqual(10, a.RowTotals[2]);

            Assert.AreEqual(80, a.ColumnTotals[0]);
            Assert.AreEqual(5, a.ColumnTotals[1]);
            Assert.AreEqual(15, a.ColumnTotals[2]);

            double[,] proportions = // (pg 599)
            {
                { 0.75, 0.01, 0.04 },
                { 0.05, 0.04, 0.01 },
                { 0.00, 0.00, 0.10 },
            };

            Assert.IsTrue(proportions.IsEqual(a.ProportionMatrix));


            double expectedVar = a.Variance;

            // Test against non-null hypothesis
            KappaTest target = new KappaTest(a, hypothesizedKappa: 0.8);

            // Fleiss   reports 0.68  (page 606)
            // Graphpad reports 0.676
            Assert.AreEqual(0.68, target.EstimatedValue, 0.01);
            Assert.AreEqual(a.Kappa, target.EstimatedValue);
            Assert.IsFalse(double.IsNaN(target.EstimatedValue));

            // Fleiss   reports 0.087 (page 607)
            // Graphpad reports 0.088
            Assert.AreEqual(0.087, target.StandardError, 0.001);
            Assert.IsFalse(double.IsNaN(target.StandardError));

            Assert.AreEqual(-1.38, target.Statistic, 0.029);
            Assert.AreEqual(0.1589, target.PValue, 0.0001);

            Assert.IsFalse(target.Significant);
        }

        [TestMethod()]
        public void KappaTestConstructorTest5()
        {
            // Example from Statistical Methods for Rates and Proportions
            // for kappa variance under the null hypothesis

            // Checked against Statistical Methods for Rates and Proportions (OK)


            int[,] matrix = // (pg 599)
            {
                { 75,  1, 4  },
                {  5,  4, 1  },
                {  0,  0, 10 },
            };

            GeneralConfusionMatrix a = new GeneralConfusionMatrix(matrix);

            Assert.AreEqual(100, a.Samples);

            Assert.AreEqual(80, a.RowTotals[0]);
            Assert.AreEqual(10, a.RowTotals[1]);
            Assert.AreEqual(10, a.RowTotals[2]);

            Assert.AreEqual(80, a.ColumnTotals[0]);
            Assert.AreEqual(5, a.ColumnTotals[1]);
            Assert.AreEqual(15, a.ColumnTotals[2]);

            double[,] proportions = // (pg 599)
            {
                { 0.75, 0.01, 0.04 },
                { 0.05, 0.04, 0.01 },
                { 0.00, 0.00, 0.10 },
            };

            Assert.IsTrue(proportions.IsEqual(a.ProportionMatrix));

            // Test under null hypothesis
            KappaTest target = new KappaTest(a, hypothesizedKappa: 0,
                alternate: OneSampleHypothesis.ValueIsGreaterThanHypothesis);

            Assert.AreEqual(0.68, target.EstimatedValue, 0.01); // pg 605
            Assert.AreEqual(a.Kappa, target.EstimatedValue);
            Assert.IsFalse(double.IsNaN(target.EstimatedValue));

            Assert.AreEqual(0.076, target.StandardError, 0.001);
            Assert.IsFalse(double.IsNaN(target.StandardError));

            Assert.AreEqual(8.95, target.Statistic, 0.08);

            Assert.IsTrue(target.Significant);
        }

    }
}
