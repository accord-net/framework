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

using Accord.Controls;
using Accord.Statistics.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accord.Math;
using Accord.Statistics;

namespace Accord.Tests.Statistics
{


    [TestClass()]
    public class OneWayAnovaTest
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
        public void OneWayAnovaConstructorTest()
        {
            // The following is the same example given in Wikipedia's page for the
            // F-Test [1]. Suppose one would like to test the effect of three levels
            // of a fertilizer on plant growth. 

            // To achieve this goal, an experimenter has divided a set of 18 plants on
            // three groups, 6 plants each. Each group has received different levels of
            // the fertilizer under question.

            // After some months, the experimenter registers the growth for each plant.

            double[][] samples =
            {
                new double[] {  6,  8,  4,  5,  3,  4 }, // records for the first group
                new double[] {  8, 12,  9, 11,  6,  8 }, // records for the second group
                new double[] { 13,  9, 11,  8,  7, 12 }, // records for the third group
            };

            // Now, he would like to test whether the different fertilizer levels has
            // indeed caused any effect in plant growth. In other words, he would like
            // to test if the three groups are indeed significantly different.

            // To do it, he runs an ANOVA test:

            OneWayAnova anova = new OneWayAnova(samples);

            // After the Anova object has been created, one can display its findings
            // in the form of a standard ANOVA table by binding anova.Table to a 
            // DataGridView or any other display object supporting data binding. To
            // illustrate, we could use Accord.NET's DataGridBox to inspect the
            // table's contents.

            // DataGridBox.Show(anova.Table);

            // The p-level for the analysis is about 0.002, meaning the test is
            // significant at the 5% significance level. The experimenter would
            // thus reject the null hypothesis, concluding there is a strong
            // evidence that the three groups are indeed different. Assuming the
            // experiment was correctly controlled, this would be an indication
            // that the fertilizer does indeed affect plant growth.

            // [1] http://en.wikipedia.org/wiki/F_test


            Assert.AreEqual(anova.Table.Count, 3);
            Assert.AreEqual("Between-Groups", anova.Table[0].Source);
            Assert.AreEqual(84, anova.Table[0].SumOfSquares); // Sb
            Assert.AreEqual(2, anova.Table[0].DegreesOfFreedom); // df
            Assert.AreEqual(42, anova.Table[0].MeanSquares); // MSb
            Assert.AreEqual(9.264705882352942, anova.Table[0].Statistic);
            Assert.AreEqual(0.0023987773293928649, anova.Table[0].Significance.PValue, 1e-16);
            Assert.IsFalse(double.IsNaN(anova.Table[0].Significance.PValue));

            Assert.AreEqual("Within-Groups", anova.Table[1].Source);
            Assert.AreEqual(68, anova.Table[1].SumOfSquares); // Sw
            Assert.AreEqual(15, anova.Table[1].DegreesOfFreedom); // df
            Assert.AreEqual(4.5333333333333332, anova.Table[1].MeanSquares); // MSw
            Assert.IsNull(anova.Table[1].Statistic);
            Assert.IsNull(anova.Table[1].Significance);

            Assert.AreEqual("Total", anova.Table[2].Source);
            Assert.AreEqual(152, anova.Table[2].SumOfSquares); // Sw
            Assert.AreEqual(17, anova.Table[2].DegreesOfFreedom); // df
            Assert.IsNull(anova.Table[2].Statistic);
            Assert.IsNull(anova.Table[2].Significance);

        }

        [TestMethod()]
        public void OneWayAnovaConstructorTest2()
        {
            // Test for unequal sample sizes

            double[][] samples =
            {
                new double[] {  6,  8,  4,  5 }, 
                new double[] {  8, 12,  9, 11,  6,  8 },
                new double[] { 13,  9, 11,  8, 12 }, 
            };


            double mean1 = samples[0].Mean();
            double dev1 = samples[0].StandardDeviation();

            double mean2 = samples[1].Mean();
            double dev2 = samples[1].StandardDeviation();

            double mean3 = samples[2].Mean();
            double dev3 = samples[2].StandardDeviation();


            OneWayAnova anova = new OneWayAnova(samples);



            Assert.AreEqual(anova.Table.Count, 3);
            Assert.AreEqual("Between-Groups", anova.Table[0].Source);
            Assert.AreEqual(53.383333333333326, anova.Table[0].SumOfSquares); // Sb
            Assert.AreEqual(2, anova.Table[0].DegreesOfFreedom); // df
            Assert.AreEqual(26.691666666666663, anova.Table[0].MeanSquares); // MSb
            Assert.AreEqual(6.4124124124124107, anova.Table[0].Statistic);
            Assert.AreEqual(0.012757639347451104, anova.Table[0].Significance.PValue);
            Assert.IsFalse(double.IsNaN(anova.Table[0].Significance.PValue));

            Assert.AreEqual("Within-Groups", anova.Table[1].Source);
            Assert.AreEqual(49.95000000000001, anova.Table[1].SumOfSquares); // Sw
            Assert.AreEqual(12, anova.Table[1].DegreesOfFreedom); // df
            Assert.AreEqual(4.1625000000000005, anova.Table[1].MeanSquares); // MSw
            Assert.IsNull(anova.Table[1].Statistic);
            Assert.IsNull(anova.Table[1].Significance);

            Assert.AreEqual("Total", anova.Table[2].Source);
            Assert.AreEqual(103.33333333333334, anova.Table[2].SumOfSquares); // Sw
            Assert.AreEqual(14, anova.Table[2].DegreesOfFreedom); // df
            Assert.IsNull(anova.Table[2].Statistic);
            Assert.IsNull(anova.Table[2].Significance);

        }

    }
}
