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
    using Accord.Math;
    using Accord.Statistics.Analysis;
    using AForge;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Accord.Statistics;

    [TestClass()]
    public class CircularDescriptiveAnalysisTest
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
        public void DescriptiveAnalysisConstructorTest1()
        {
            double[,] data =
            {
                // hours  minutes        weekedays
                {    7,    52,     (int)DayOfWeek.Monday   },
                {    3,    12,     (int)DayOfWeek.Friday   },
                {   23,    12,     (int)DayOfWeek.Thursday },
                {   21,    42,     (int)DayOfWeek.Saturday },
                {    5,    13,     (int)DayOfWeek.Friday   },
                {   15,    16,     (int)DayOfWeek.Saturday },
                {    6,     8,     (int)DayOfWeek.Sunday   },
                {    8,    22,     (int)DayOfWeek.Friday   },
            };

            double[] lengths = { 24, 60, 7 };

            // Create the analysis
            var analysis = new CircularDescriptiveAnalysis(data, lengths);

            // Compute
            analysis.Compute();

            test(analysis);
        }


        private static void test(CircularDescriptiveAnalysis analysis)
        {
            double[] expectedMean;
            double[] expectedStdDev;
            double[] expectedVar;
            double[] expectedMed;
            double[] expectedError;

            expectedValues(analysis, 
                out expectedMean, out expectedStdDev,
                out expectedVar, out expectedMed,
                out expectedError);

            var angles = analysis.Angles;
            var cos = analysis.CosineSum;
            var sin = analysis.SineSum;
            var columnNames = analysis.ColumnNames;
            var distinct = analysis.Distinct;
            var means = analysis.Means;
            var medians = analysis.Medians;
            var ranges = analysis.Ranges;
            var samples = analysis.Samples;
            var standardDeviation = analysis.StandardDeviations;
            var sums = analysis.Sums;
            var variables = analysis.Variables;
            var variances = analysis.Variances;
            var quartiles = analysis.Quartiles;
            var inner = analysis.InnerFences;
            var outer = analysis.OuterFences;
            var error = analysis.StandardErrors;

            Assert.IsTrue(columnNames.IsEqual(new string[] 
            {
                "Column 0",
                "Column 1",
                "Column 2",
            }));


            Assert.IsTrue(distinct.IsEqual(new int[] { 8, 7, 5 }));
            Assert.IsTrue(means.IsEqual(expectedMean));
            Assert.IsTrue(medians.IsEqual(expectedMed));
            Assert.IsTrue(error.IsEqual(expectedError, 1e-14));


            Assert.AreEqual(3, ranges.Length);
            Assert.AreEqual(3, ranges[0].Min);
            Assert.AreEqual(23, ranges[0].Max);
            Assert.AreEqual(8, ranges[1].Min);
            Assert.AreEqual(52, ranges[1].Max);
            Assert.AreEqual(0, ranges[2].Min);
            Assert.AreEqual(6, ranges[2].Max);

            Assert.IsTrue(samples == 8);

            // string str = angles.ToString(CSharpJaggedMatrixFormatProvider.InvariantCulture);

            var expectedAngles = new double[][] 
            {
                new double[] { -1.30899693899575, -2.35619449019234, 2.87979326579064, 2.35619449019234, -1.83259571459405, 0.785398163397448, -1.5707963267949, -1.0471975511966 },
                new double[] { 2.30383461263252, -1.88495559215388, -1.88495559215388, 1.25663706143592, -1.78023583703422, -1.46607657167524, -2.30383461263252, -0.837758040957278 },
                new double[] { -2.24399475256414, 1.34639685153848, 0.448798950512828, 2.24399475256414, 1.34639685153848, 2.24399475256414, -3.14159265358979, 1.34639685153848 } 
            };

            Assert.IsTrue(angles.IsEqual(expectedAngles, 1e-10));

            Assert.IsTrue(standardDeviation.IsEqual(expectedStdDev, 1e-14));
            Assert.IsTrue(sums.IsEqual(new double[] { 88.0, 177.0, 32.0 }));
            Assert.IsTrue(sin.IsEqual(new double[] { -2.831951230073507, -3.6668708378746273, 4.1404989581310589 }, 1e-14));
            Assert.IsTrue(cos.IsEqual(new double[] { -1.1730326074756157, -1.0815308282839116, -1.3019377358048381 }, 1e-14));
            Assert.IsTrue(variables == 3);
            Assert.IsTrue(variances.IsEqual(expectedVar, 1e-14));

            Assert.AreEqual(3, quartiles.Length);
            Assert.AreEqual(23, quartiles[0].Min);
            Assert.AreEqual(7.5, quartiles[0].Max);
            Assert.AreEqual(8, quartiles[1].Min);
            Assert.AreEqual(19, quartiles[1].Max);
            Assert.AreEqual(5, quartiles[2].Min);
            Assert.AreEqual(6.5, quartiles[2].Max);

            Assert.AreEqual(22.25, inner[0].Min);
            Assert.AreEqual(8.25, inner[0].Max);

            Assert.AreEqual(51.5, inner[1].Min);
            Assert.AreEqual(35.5, inner[1].Max);

            Assert.AreEqual(21.5, outer[0].Min);
            Assert.AreEqual(9, outer[0].Max);

            Assert.AreEqual(35, outer[1].Min);
            Assert.AreEqual(52, outer[1].Max);

            Assert.AreEqual(5.0, quartiles[2].Min);
            Assert.AreEqual(6.5, quartiles[2].Max);
        }

        private static void expectedValues(CircularDescriptiveAnalysis analysis, 
            out double[] expectedMean, out double[] expectedStdDev, 
            out double[] expectedVar, out double[] expectedMed,
            out double[] expectedErrors)
        {
            var data = analysis.Source.Transpose().ToArray();

            double[] expectedLengths = { 24, 60, 7 };
            expectedMean = data.ApplyWithIndex((x, i) => Circular.Mean(x, expectedLengths[i]));
            expectedStdDev = data.ApplyWithIndex((x, i) => Circular.StandardDeviation(x, expectedLengths[i]));
            expectedErrors = data.ApplyWithIndex((x, i) => Circular.StandardError(x, expectedLengths[i], 0.05));
            expectedVar = data.ApplyWithIndex((x, i) => Circular.Variance(x, expectedLengths[i]));
            expectedMed = data.ApplyWithIndex((x, i) => Circular.Median(x, expectedLengths[i]));
            DoubleRange[] expectedQuartiles = data
                .ApplyWithIndex((x, i) =>
                {
                    DoubleRange q;
                    Circular.Quartiles(x, expectedLengths[i], out q);
                    return q;
                });
        }


    }
}
