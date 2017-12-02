// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
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
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class TwoWayAnovaTest
    {

        [Test]
        public void doc_test()
        {
            #region doc_test
            // Example by J. Jones, Professor of Mathematics in Richland Community 
            // College: https://people.richland.edu/james/lecture/m170/ch13-2wy.html

            // In this example, we collected the number of successfully grown corn plants 
            // given the use of 4 types of fertilizer, and 3 different types of seed.

            double[,,] samples =
            {
            //                    Fert I        Fert II       Fert III        Fert IV        Fert V
            /* Seed A-402 */ { { 106, 110 },  { 95, 100 },  { 94,  107 },  { 103, 104 },  { 100, 102 } },
            /* Seed B-894 */ { { 110, 112 },  { 98,  99 },  { 100, 101 },  { 108, 112 },  { 105, 107 } },
            /* Seed C-952 */ { { 94,  97  },  { 86,  87 },  { 98,   99 },  {  99, 101 },  {  94,  98 } },
            };

            // There are always three hypothesis in a 2-way ANOVA:
            // - There is no difference between population means for the first factor;
            // - There is no difference between population means for the second factor;
            // - There is no interaction between the two different factors

            // Let's create a mixed-effect effects ANOVA model (ANOVA model 3)
            var twoWayAnova = new TwoWayAnova(samples, TwoWayAnovaModel.Mixed);

            // Now, we can obtain the test summary table by using:
            TwoWayAnovaVariationSources sources = twoWayAnova.Sources;

            // Seed source of variation
            double seedSS = sources.FactorA.SumOfSquares;                               // 512.86666666666713
            double seedDF = sources.FactorA.DegreesOfFreedom;                           // 2
            double seedMS = sources.FactorA.MeanSquares;                                // 256.43333333333356
            double seedF = sources.FactorA.Statistic.Value;                             // 28.283088235294144
            double seedPValue = sources.FactorA.Significance.PValue;                    // 8.1353422904297046E-06
            double seedCrit = sources.FactorA.Significance.CriticalValue;               // 3.6823203436732341
            bool seedSignificant = sources.FactorA.Significance.Significant;            // true
                                                                                        
            // Fertilizer source of variation                                           
            double fertilizerSS = sources.FactorB.SumOfSquares;                         // 449.46666666666607
            double fertilizerDF = sources.FactorB.DegreesOfFreedom;                     // 4
            double fertilizerMS = sources.FactorB.MeanSquares;                          // 112.36666666666652
            double fertilizerF = sources.FactorB.Statistic.Value;                       // 12.39338235294116
            double fertilizerPValue = sources.FactorB.Significance.PValue;              // 0.00011887234971294213
            double fertilizerCrit = sources.FactorB.Significance.CriticalValue;         // 3.0555682759065936
            bool fertilizerSignificant = sources.FactorB.Significance.Significant;      // true

            // Interaction source of variation
            double interactionSS = sources.Interaction.SumOfSquares;                    // 143.1333333333335
            double interactionDF = sources.Interaction.DegreesOfFreedom;                // 8
            double interactionMS = sources.Interaction.MeanSquares;                     // 17.891666666666687
            double interactionF = sources.Interaction.Statistic.Value;                  // 1.9733455882352964
            double interactionPValue = sources.Interaction.Significance.PValue;         // 0.12208995001760085
            double interactionCrit = sources.Interaction.Significance.CriticalValue;    // 2.6407968829069017
            bool interactionSignificant = sources.Interaction.Significance.Significant; // false

            // Within-variance source of variation
            double withinSS = sources.Error.SumOfSquares;                               // 136
            double withinDF = sources.Error.DegreesOfFreedom;                           // 15
            double withinMS = sources.Error.MeanSquares;                                // 9.0666666666666664
            #endregion

            Assert.AreEqual(512.86666666666713, seedSS, 1e-8);
            Assert.AreEqual(2, seedDF, 1e-8);
            Assert.AreEqual(256.43333333333356, seedMS, 1e-8);
            Assert.AreEqual(28.283088235294144, seedF, 1e-8);
            Assert.AreEqual(8.1353422904297046E-06, seedPValue, 1e-8);
            Assert.AreEqual(3.6823203436732341, seedCrit, 1e-8);
            Assert.AreEqual(true, seedSignificant);

            Assert.AreEqual(449.46666666666607, fertilizerSS, 1e-8);
            Assert.AreEqual(4, fertilizerDF, 1e-8);
            Assert.AreEqual(112.36666666666652, fertilizerMS, 1e-8);
            Assert.AreEqual(12.39338235294116, fertilizerF, 1e-8);
            Assert.AreEqual(0.00011887234971294213, fertilizerPValue, 1e-8);
            Assert.AreEqual(3.0555682759065936, fertilizerCrit, 1e-8);
            Assert.AreEqual(true, fertilizerSignificant);

            Assert.AreEqual(143.1333333333335, interactionSS, 1e-8);
            Assert.AreEqual(8, interactionDF, 1e-8);
            Assert.AreEqual(17.891666666666687, interactionMS, 1e-8);
            Assert.AreEqual(1.9733455882352964, interactionF, 1e-8);
            Assert.AreEqual(0.12208995001760085, interactionPValue, 1e-8);
            Assert.AreEqual(2.6407968829069017, interactionCrit, 1e-8);
            Assert.AreEqual(false, interactionSignificant);

            Assert.AreEqual(136, withinSS, 1e-8);
            Assert.AreEqual(15, withinDF, 1e-8);
            Assert.AreEqual(9.0666666666666664, withinMS, 1e-8);
        }

        [Test]
        public void TwoWayAnovaConstructorTest()
        {
            // Example by Aalborg Universitet, slide 22:
            // http://www.smi.hst.aau.dk/~cdahl/BiostatPhD/ANOVA.pdf

            // There are three set of hypothesis in atwo-way ANOVA. In the example above, they are:
            //  - H0: There is no effect of hormone treatment on the mean plasma concentration
            //  - H0: There is no difference in mean plasma concentration between sexes
            //  - H0: There is no interaction of sex and hormone treatment on the mean plasma concentration

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

            // Let's create a mixed-effect effects ANOVA model (ANOVA model 3)
            var twoWayAnova = new TwoWayAnova(samples, TwoWayAnovaModel.Mixed);

            testSources(twoWayAnova);
        }

        [Test]
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
