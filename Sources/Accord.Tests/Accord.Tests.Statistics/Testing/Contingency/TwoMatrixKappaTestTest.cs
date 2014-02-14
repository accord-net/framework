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
    public class TwoMatrixKappaTestTest
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
        public void KappaTestConstructorTest1()
        {
            // Example from Ientilucci, Emmett (2006). "On Using and Computing the Kappa Statistic".
            // Available on: http://www.cis.rit.edu/~ejipci/Reports/On_Using_and_Computing_the_Kappa_Statistic.pdf 

            // This paper uses the delta method approximation
            // for computing the kappa variance.

            int[,] matrix1 =
            {
                { 317,  23,  0,  0 },
                {  61, 120,  0,  0 },
                {   2,   4, 60,  0 },
                {  35,  29,  0,  8 },
            };

            int[,] matrix2 =
            {
                { 377,  79,  0,  0 },
                {   2,  72,  0,  0 },
                {  33,   5, 60,  0 },
                {   3,  20,  0,  8 },
            };

            GeneralConfusionMatrix a = new GeneralConfusionMatrix(matrix1);
            GeneralConfusionMatrix b = new GeneralConfusionMatrix(matrix2);


            Assert.AreEqual(0.7663, a.OverallAgreement, 1e-4);
            Assert.AreEqual(0.7845, b.OverallAgreement, 1e-4);

            Assert.AreEqual(0.4087, a.ChanceAgreement, 1e-4);
            Assert.AreEqual(0.47986, b.ChanceAgreement, 1e-4);

            double kA = a.Kappa;
            double kB = b.Kappa;

            double varA = KappaTest.DeltaMethodKappaVariance(a);
            double varB = KappaTest.DeltaMethodKappaVariance(b);

            TwoMatrixKappaTest target = new TwoMatrixKappaTest(kA, varA, kB, varB);

            Assert.AreEqual(TwoSampleHypothesis.ValuesAreDifferent, target.Hypothesis);
            Assert.AreEqual(DistributionTail.TwoTail, target.Tail);

            // Compare Kappas
            Assert.AreEqual(0.605, target.EstimatedValue1, 1e-3);
            Assert.IsFalse(double.IsNaN(a.Kappa));

            Assert.AreEqual(0.586, target.EstimatedValue2, 1e-3);
            Assert.IsFalse(double.IsNaN(b.Kappa));


            // Compare variances: 
            Assert.AreEqual(0.00073735, target.Variance1, 1e-7);
            Assert.IsFalse(double.IsNaN(a.Variance));

            Assert.AreEqual(0.00087457, target.Variance2, 1e-7);
            Assert.IsFalse(double.IsNaN(b.Variance));


            Assert.AreEqual(0.475, target.Statistic, 1e-3);
            Assert.IsFalse(double.IsNaN(target.Statistic));

            Assert.IsFalse(target.Significant);
        }

        [TestMethod()]
        public void KappaTestConstructorTest2()
        {
            // Example from Congalton

            int[,] matrix1 = // pg 108
            {
                { 65,  4,  22,  24 },
                {  6, 81,   5,   8 },
                {  0, 11,  85,  19 },
                {  4,  7,   3,  90 },
            };

            GeneralConfusionMatrix a = new GeneralConfusionMatrix(matrix1);

            Assert.AreEqual(115, a.RowTotals[0]);
            Assert.AreEqual(100, a.RowTotals[1]);
            Assert.AreEqual(115, a.RowTotals[2]);
            Assert.AreEqual(104, a.RowTotals[3]);

            Assert.AreEqual(75, a.ColumnTotals[0]);
            Assert.AreEqual(103, a.ColumnTotals[1]);
            Assert.AreEqual(115, a.ColumnTotals[2]);
            Assert.AreEqual(141, a.ColumnTotals[3]);



            int[,] matrix2 = // pg 109
            {
                { 45,  4, 12, 24 },
                {  6, 91,  5,  8 },
                {  0,  8, 55,  9 },
                {  4,  7,  3, 55 },
            };

            
            GeneralConfusionMatrix b = new GeneralConfusionMatrix(matrix2);

            Assert.AreEqual(85, b.RowTotals[0]);
            Assert.AreEqual(110, b.RowTotals[1]);
            Assert.AreEqual(72, b.RowTotals[2]);
            Assert.AreEqual(69, b.RowTotals[3]);

            Assert.AreEqual(55, b.ColumnTotals[0]);
            Assert.AreEqual(110, b.ColumnTotals[1]);
            Assert.AreEqual(75, b.ColumnTotals[2]);
            Assert.AreEqual(96, b.ColumnTotals[3]);

            // Check overall accuracy
            Assert.AreEqual(0.74, a.OverallAgreement, 0.005);
            Assert.AreEqual(0.73, b.OverallAgreement, 0.005);


            double kA = a.Kappa;
            double kB = b.Kappa;

            double varA = KappaTest.DeltaMethodKappaVariance(a);
            double varB = KappaTest.DeltaMethodKappaVariance(b);

            // Create the test
            TwoMatrixKappaTest target = new TwoMatrixKappaTest(kA, varA, kB, varB);

            Assert.AreEqual(TwoSampleHypothesis.ValuesAreDifferent, target.Hypothesis);
            Assert.AreEqual(DistributionTail.TwoTail, target.Tail);

            // Compare Kappas (pg 109)
            Assert.AreEqual(0.65, target.EstimatedValue1, 0.05);
            Assert.IsFalse(double.IsNaN(a.Kappa));

            Assert.AreEqual(0.64, target.EstimatedValue2, 0.05);
            Assert.IsFalse(double.IsNaN(b.Kappa));


            // Compare variances: 
            Assert.AreEqual(0.0007778, target.Variance1, 1e-7);
            Assert.IsFalse(double.IsNaN(a.Variance));

            Assert.AreEqual(0.0010233, target.Variance2, 1e-7);
            Assert.IsFalse(double.IsNaN(b.Variance));


            Assert.AreEqual(0.3087, target.Statistic, 1e-5);
            Assert.IsFalse(double.IsNaN(target.Statistic));

            Assert.IsFalse(target.Significant);
        }

        [TestMethod()]
        public void KappaTestConstructorTest3()
        {
            double k1 = 0.95;
            double v1 = 6.10e-6;

            double k2 = 0.9241;
            double v2 = 9.02e-6;

            TwoMatrixKappaTest target = new TwoMatrixKappaTest(k1, v1, k2, v2);

            Assert.AreEqual(TwoSampleHypothesis.ValuesAreDifferent, target.Hypothesis);
            Assert.AreEqual(DistributionTail.TwoTail, target.Tail);
            Assert.AreEqual(v1 + v2, target.OverallVariance);

            Assert.AreEqual(6.6607612733636143, target.Statistic);
            Assert.IsTrue(target.Significant);
        }

        [TestMethod()]
        public void KappaTestConstructorTest4()
        {
            {
                double k1 = 0.819223955119253;
                double v1 = 0.00296025931609249;

                double k2 = 0.833170126346748;
                double v2 = 0.00278659995785188;

                TwoMatrixKappaTest target = new TwoMatrixKappaTest(k1, v1, k2, v2);

                Assert.AreEqual(TwoSampleHypothesis.ValuesAreDifferent, target.Hypothesis);
                Assert.AreEqual(DistributionTail.TwoTail, target.Tail);
                Assert.AreEqual(v1 + v2, target.OverallVariance);

                Assert.AreEqual(0.1839669091631167, target.Statistic, 1e-16);
                Assert.IsFalse(double.IsNaN(target.Statistic));
                Assert.IsFalse(target.Significant);
            }

            {
                double k1 = 0.946859215964404;
                double v1 = 0.000111244462937448;

                double k2 = 0.98368298182233;
                double v2 = 0.0000353910186138505;

                TwoMatrixKappaTest target = new TwoMatrixKappaTest(k1, v1, k2, v2);

                Assert.AreEqual(TwoSampleHypothesis.ValuesAreDifferent, target.Hypothesis);
                Assert.AreEqual(DistributionTail.TwoTail, target.Tail);
                Assert.AreEqual(v1 + v2, target.OverallVariance);

                Assert.AreEqual(3.0409457018033272, target.Statistic);
                Assert.IsTrue(target.Significant);
            }
        }


    }
}
