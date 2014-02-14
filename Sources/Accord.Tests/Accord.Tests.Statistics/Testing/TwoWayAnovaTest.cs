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
    using System;

    [TestClass()]
    public class TwoWayAnovaTest
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
        public void TwoWayAnovaConstructorTest()
        {
            // Example by Aalborg Universitet
            // http://www.smi.hst.aau.dk/~cdahl/BiostatPhD/ANOVA.pdf

            double[][][] samples = 
            {
                
                new double[][] // No hormone treatment
                {
                    // Female
                    new double[] { 16.3, 20.4, 12.4, 15.8, 9.5 },

                    // Male
                    new double[] { 15.3, 17.4, 10.9, 10.3, 6.7 }
                },

                new double[][] // Hormone treatment
                {
                    // Female
                    new double[] { 38.1, 26.2, 32.3, 35.8, 30.2 },

                    // Male
                    new double[] { 34.0, 22.8, 27.8, 25.0, 29.3 }
                }
            };

            TwoWayAnova target = new TwoWayAnova(samples, TwoWayAnovaModel.Mixed);

            testSources(target);
        }

        [TestMethod()]
        public void TwoWayAnovaConstructorTest2()
        {
            // Example by Aalborg Universitet
            // http://www.smi.hst.aau.dk/~cdahl/BiostatPhD/ANOVA.pdf

            double[] samples = 
            {
                16.3, 20.4, 12.4, 15.8, 9.5, // female, no hormone
                15.3, 17.4, 10.9, 10.3, 6.7, // male, no hormone
                38.1, 26.2, 32.3, 35.8, 30.2, // female, hormone
                34.0, 22.8, 27.8, 25.0, 29.3, // male, hormone
            };

            int[] hormone = 
            {
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1
            };

            int[] sex =
            {
                0, 0, 0, 0, 0, 1, 1, 1, 1, 1,
                0, 0, 0, 0, 0, 1, 1, 1, 1, 1
            };

            TwoWayAnova target = new TwoWayAnova(samples, hormone, sex, TwoWayAnovaModel.Mixed);

            testSources(target);
        }

        private static void testSources(TwoWayAnova target)
        {
            AnovaVariationSource cells = target.Sources.Cells;
            Assert.AreEqual("Cells", cells.Source);
            Assert.AreEqual(1461.3254999999995, cells.SumOfSquares);
            Assert.AreEqual(3, cells.DegreesOfFreedom);
            Assert.IsTrue(target.Table.Contains(cells));

            AnovaVariationSource error = target.Sources.Error;
            Assert.AreEqual("Within-cells (error)", error.Source);
            Assert.AreEqual(301.39200000000005, error.SumOfSquares);
            Assert.AreEqual(16, error.DegreesOfFreedom);
            Assert.IsTrue(target.Table.Contains(error));

            AnovaVariationSource factorA = target.Sources.FactorA;
            Assert.AreEqual("Factor A", factorA.Source);
            Assert.AreEqual(1386.1125, factorA.SumOfSquares, 1e-4);
            Assert.AreEqual(1, factorA.DegreesOfFreedom);
            Assert.AreEqual(1386.1125, factorA.MeanSquares, 1e-4);
            Assert.AreEqual(73.58, factorA.Significance.Statistic, 1e-2);
            Assert.AreEqual(1, factorA.Significance.DegreesOfFreedom1);
            Assert.AreEqual(16, factorA.Significance.DegreesOfFreedom2);
            Assert.IsTrue(factorA.Significance.Significant);

            AnovaVariationSource factorB = target.Sources.FactorB;
            Assert.AreEqual("Factor B", factorB.Source);
            Assert.AreEqual(70.3125, factorB.SumOfSquares, 1e-4);
            Assert.AreEqual(1, factorB.DegreesOfFreedom);
            Assert.AreEqual(70.3125, factorB.MeanSquares, 1e-4);
            Assert.AreEqual(3.73, factorB.Significance.Statistic, 1e-2);
            Assert.AreEqual(1, factorB.Significance.DegreesOfFreedom1);
            Assert.AreEqual(16, factorB.Significance.DegreesOfFreedom2);
            Assert.IsFalse(factorB.Significance.Significant);

            AnovaVariationSource interaction = target.Sources.Interaction;
            Assert.AreEqual("Interaction AxB", interaction.Source);
            Assert.AreEqual(4.9005, interaction.SumOfSquares, 1e-4);
            Assert.AreEqual(1, interaction.DegreesOfFreedom);
            Assert.AreEqual(4.9005, interaction.MeanSquares, 1e-4);
            Assert.AreEqual(0.260, interaction.Significance.Statistic, 1e-3);
            Assert.AreEqual(1, interaction.Significance.DegreesOfFreedom1);
            Assert.AreEqual(16, interaction.Significance.DegreesOfFreedom2);
            Assert.IsFalse(interaction.Significance.Significant);
        }
    }
}
