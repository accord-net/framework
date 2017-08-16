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
    using System;
    using Accord.Math;
    using Accord.Statistics;
    using Accord.Statistics.Analysis;
    using AForge;
    using NUnit.Framework;

    [TestFixture]
    public class CircularDescriptiveAnalysisTest
    {

        [Test]
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

        [Test]
        public void DescriptiveAnalysisConstructor_NoCompute()
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

            test(analysis);
        }


        private static void test(CircularDescriptiveAnalysis analysis)
        {
            double[] expectedMean;
            double[] expectedStdDev;
            double[] expectedVar;
            double[] expectedMed;
            double[] expectedError;
            DoubleRange[] expectedQuartiles;

            expectedValues(analysis,
                out expectedMean, out expectedStdDev,
                out expectedVar, out expectedMed,
                out expectedError, out expectedQuartiles);

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
            Assert.IsTrue(medians.IsEqual(expectedMed, 1e-14));
            Assert.IsTrue(error.IsEqual(expectedError, 1e-14));


            Assert.AreEqual(3, ranges.Length);
            Assert.AreEqual(0, ranges[0].Min);
            Assert.AreEqual(24, ranges[0].Max);
            Assert.AreEqual(0, ranges[1].Min);
            Assert.AreEqual(60, ranges[1].Max);
            Assert.AreEqual(0, ranges[2].Min);
            Assert.AreEqual(7, ranges[2].Max);

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
            Assert.AreEqual(0, quartiles[0].Min);
            Assert.AreEqual(7.75, quartiles[0].Max, 1e-8);
            Assert.AreEqual(9, quartiles[1].Min, 1e-8);
            Assert.AreEqual(20.5, quartiles[1].Max, 1e-8);
            Assert.AreEqual(5, quartiles[2].Min, 1e-8);
            Assert.AreEqual(6.75, quartiles[2].Max, 1e-8);

            Assert.AreEqual(12.375, inner[0].Min, 1e-8);
            Assert.AreEqual(19.375, inner[0].Max, 1e-8);

            Assert.AreEqual(51.75, inner[1].Min, 1e-8);
            Assert.AreEqual(37.75, inner[1].Max, 1e-8);

            Assert.AreEqual(0.75, outer[0].Min, 1e-8);
            Assert.AreEqual(7, outer[0].Max, 1e-8);

            Assert.AreEqual(34.5, outer[1].Min, 1e-8);
            Assert.AreEqual(55, outer[1].Max, 1e-8);

            Assert.AreEqual(5.0, quartiles[2].Min, 1e-8);
            Assert.AreEqual(6.75, quartiles[2].Max, 1e-8);
        }



        [Test]
        public void DescriptiveAnalysis_InPlaceRow()
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

            double[] lengths = { 24 };
            double[] row = data.GetColumn(0);

            double[] original = data.GetColumn(0);

            // Create the analysis
            var analysis = new CircularDescriptiveAnalysis(row, 24, "Hours", inPlace: true);

            // Compute
            analysis.Compute();

            testOne(analysis, original, 24);

            Assert.AreSame(row, analysis.Angles[0]);
        }

        private static void testOne(CircularDescriptiveAnalysis analysis, 
            double[] original, int originalLength)
        {
            double[] expectedMean;
            double[] expectedStdDev;
            double[] expectedVar;
            double[] expectedMed;
            double[] expectedError;
            DoubleRange[] expectedQuartiles;

            expectedValues(original, originalLength,
                out expectedMean, out expectedStdDev,
                out expectedVar, out expectedMed,
                out expectedError, out expectedQuartiles);

            expectedMean = expectedMean.First(1);
            expectedStdDev = expectedStdDev.First(1);
            expectedVar = expectedVar.First(1);
            expectedMed = expectedMed.First(1);
            expectedError = expectedError.First(1);

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
                "Hours",
            }));


            Assert.IsTrue(distinct.IsEqual(new int[] { 8 }));
            Assert.IsTrue(means.IsEqual(expectedMean));
            Assert.IsTrue(medians.IsEqual(expectedMed));
            Assert.IsTrue(error.IsEqual(expectedError, 1e-14));


            Assert.AreEqual(1, ranges.Length);
            Assert.AreEqual(0, ranges[0].Min);
            Assert.AreEqual(24, ranges[0].Max);

            Assert.IsTrue(samples == 8);

            // string str = angles.ToString(CSharpJaggedMatrixFormatProvider.InvariantCulture);

            var expectedAngles = new double[][] 
            {
                new double[] { -1.30899693899575, -2.35619449019234, 2.87979326579064, 2.35619449019234, -1.83259571459405, 0.785398163397448, -1.5707963267949, -1.0471975511966 },
            };

            Assert.IsTrue(angles.IsEqual(expectedAngles, 1e-10));

            Assert.IsTrue(standardDeviation.IsEqual(expectedStdDev, 1e-14));
            Assert.IsTrue(sums.IsEqual(new double[] { -2.0943951023931948 }));
            Assert.IsTrue(sin.IsEqual(new double[] { -2.831951230073507 }, 1e-14));
            Assert.IsTrue(cos.IsEqual(new double[] { -1.1730326074756157 }, 1e-14));
            Assert.IsTrue(variables == 1);
            Assert.IsTrue(variances.IsEqual(expectedVar, 1e-14));

            Assert.AreEqual(1, quartiles.Length);
            Assert.AreEqual(0, quartiles[0].Min);
            Assert.AreEqual(7.75, quartiles[0].Max, 1e-8);

            Assert.AreEqual(12.375, inner[0].Min, 1e-8);
            Assert.AreEqual(19.375, inner[0].Max, 1e-8);

            Assert.AreEqual(0.75, outer[0].Min, 1e-8);
            Assert.AreEqual(7.00, outer[0].Max, 1e-8);
        }



        private static void expectedValues(CircularDescriptiveAnalysis analysis, 
            out double[] expectedMean, out double[] expectedStdDev, 
            out double[] expectedVar, out double[] expectedMed,
            out double[] expectedErrors, out DoubleRange[] expectedQuartiles)
        {
            var data = analysis.Source.Transpose().ToJagged();

            double[] expectedLengths = { 24, 60, 7 };
            expectedMean = data.ApplyWithIndex((x, i) => Circular.Mean(x, expectedLengths[i]));
            expectedStdDev = data.ApplyWithIndex((x, i) => Circular.StandardDeviation(x, expectedLengths[i]));
            expectedErrors = data.ApplyWithIndex((x, i) => Circular.StandardError(x, expectedLengths[i], 0.05));
            expectedVar = data.ApplyWithIndex((x, i) => Circular.Variance(x, expectedLengths[i]));
            expectedMed = data.ApplyWithIndex((x, i) => Circular.Median(x, expectedLengths[i]));
            expectedQuartiles = data
                .ApplyWithIndex((x, i) =>
                {
                    DoubleRange q;
                    Circular.Quartiles(x, expectedLengths[i], out q);
                    return q;
                });
        }

        private static void expectedValues(double[] original, int originalLength,
           out double[] expectedMean, out double[] expectedStdDev,
           out double[] expectedVar, out double[] expectedMed,
           out double[] expectedErrors, out DoubleRange[] expectedQuartiles)
        {
            var data = original.ToJagged(asColumnVector: false);

            double[] expectedLengths = { originalLength };
            expectedMean = data.ApplyWithIndex((x, i) => Circular.Mean(x, expectedLengths[i]));
            expectedStdDev = data.ApplyWithIndex((x, i) => Circular.StandardDeviation(x, expectedLengths[i]));
            expectedErrors = data.ApplyWithIndex((x, i) => Circular.StandardError(x, expectedLengths[i], 0.05));
            expectedVar = data.ApplyWithIndex((x, i) => Circular.Variance(x, expectedLengths[i]));
            expectedMed = data.ApplyWithIndex((x, i) => Circular.Median(x, expectedLengths[i]));
            expectedQuartiles = data
                .ApplyWithIndex((x, i) =>
                {
                    DoubleRange q;
                    Circular.Quartiles(x, expectedLengths[i], out q);
                    return q;
                });
        }


        [Test]
        public void DataBindTest()
        {
            double[,] data =
            {
                // hours  minutes
                {    7,    52 },
                {    3,    12 },
                {   23,    12 },
                {   21,    42 },
                {    5,    13 },
                {   15,    16 },
                {    6,     8 },
                {    8,    22 },
            };

            double[] lengths = { 24, 60 };

            var analysis = new CircularDescriptiveAnalysis(data, lengths);

            analysis.Compute();

            var m0 = analysis.Measures[0];
            var m1 = analysis.Measures[1];

            Assert.AreEqual(0, m0.Index);
            Assert.AreEqual(1, m1.Index);

            Assert.AreEqual("Column 0", m0.Name);
            Assert.AreEqual("Column 1", m1.Name);

            Assert.AreEqual(m0, analysis.Measures["Column 0"]);
            Assert.AreEqual(m1, analysis.Measures["Column 1"]);

            // var box = Accord.Controls.DataGridBox.Show(analysis.Measures);
            // Assert.AreEqual(23, box.DataGridView.Columns.Count);
        }
    }
}
