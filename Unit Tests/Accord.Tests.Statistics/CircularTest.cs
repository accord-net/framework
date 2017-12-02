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
    using Accord;
    using Accord.Math;
    using Accord.Statistics;
    using AForge;
    using NUnit.Framework;
    using System;
    using System.Data;

    [TestFixture]
    public class CircularTest
    {

        private double[] angles = 
        {
            0.003898633, 5.956808760, 0.318487983,
            5.887227832, 0.641802182, 4.640345741,
            0.931996171, 0.426819547, 0.624824460,
            0.247553652, 6.282827901, 0.313780766,
            0.093206440, 0.392279489, 0.601228848
        };

        // Tests performed against the Circular Statistics Toolbox (Directional Statistics)
        // http://fr.mathworks.com/matlabcentral/fileexchange/10676-circular-statistics-toolbox--directional-statistics-

#if !NO_DATA_TABLE
        [Test]
        public void WeightedKappaTest()
        {
            DataTable table = new DataTable();

            // Add multiple columns at once:
            table.Columns.Add("columnName1", "columnName2");

            double[] angles = { 0.1242, 1.2425, 0.6712 };
            double[] weights = { 3, 1, 1 };

            weights = weights.Divide(weights.Sum());

            double expectedMean = 0.4436528;
            double expectedKappa = 5.497313;

            double actualMean = Circular.WeightedMean(angles, weights);
            Assert.AreEqual(expectedMean, actualMean, 1e-6);

            double actualKappa = Circular.WeightedConcentration(angles, weights);
            Assert.AreEqual(expectedKappa, actualKappa, 1e-6);

            actualKappa = Circular.WeightedConcentration(angles, weights, actualMean);
            Assert.AreEqual(expectedKappa, actualKappa, 1e-6);
        }
#endif

        [Test]
        public void KappaTest1()
        {
            double expected = 3.721646;
            double actual = Circular.Concentration(angles);

            Assert.AreEqual(expected, actual, 1e-6);
        }

        [Test]
        public void KappaTest()
        {
            double mean = Circular.Mean(angles);

            double expected = 3.721646;
            double actual = Circular.Concentration(angles, mean);

            Assert.AreEqual(expected, actual, 1e-6);
        }

        [Test]
        public void MeanTest()
        {
            double expected = 2.0519609734450655614e-01;
            double actual = Circular.Mean(angles);

            Assert.AreEqual(expected, actual, 1e-15);
        }

        [Test]
        public void VarianceTest()
        {
            double expected = 1.4668557528213066465e-01;
            double actual = Circular.Variance(angles);

            Assert.AreEqual(expected, actual, 1e-15);
        }

        [Test]
        public void CircularDeviationTest()
        {
            /* angles = [ 0.003898633 5.956808760 0.318487983 5.887227832 0.641802182, 4.640345741, 0.931996171, 0.426819547, 0.624824460, 0.247553652, 6.282827901, 0.313780766,  0.093206440, 0.392279489, 0.601228848 ]
            */
            
            // Compared against R's circular package
            /* angles <- c(0.003898633, 5.956808760, 0.318487983, 5.887227832, 0.641802182, 4.640345741, 0.931996171, 0.426819547, 0.624824460, 0.247553652, 6.282827901, 0.313780766,  0.093206440, 0.392279489, 0.601228848)
               angular.deviation(angles)
               0.541637471528937930465
             */ 

            double expected = 5.4163747152893815251e-01;
            double actual = Circular.AngularDeviation(angles);

            Assert.IsFalse(Double.IsNaN(actual));
            Assert.AreEqual(expected, actual, 1e-15);
        }

        [Test]
        public void StandardDeviationTest()
        {
            double expected = 5.6325338695100524156e-01;
            double actual = Circular.StandardDeviation(angles);

            Assert.AreEqual(expected, actual, 1e-15);
        }

        [Test]
        public void DistanceTest()
        {
            double x = 0.15;
            double y = -1.22;

            double expected = 1.37;
            double actual = Circular.Distance(x, y);

            Assert.AreEqual(expected, actual, 1e-15);
        }

        [Test]
        public void SkewnessTest()
        {
            double expected = 1.4938580058598621703e-01;
            double actual = Circular.Skewness(angles);

            Assert.AreEqual(expected, actual, 1e-15);
        }

        [Test]
        public void KurtosisTest()
        {
            double expected = 6.4794232277921437468e-01;
            double actual = Circular.Kurtosis(angles);

            Assert.AreEqual(expected, actual, 1e-15);
        }

        [Test]
        public void MedianTest()
        {
            double expected = 3.1378076599999993324e-01;
            double actual = Circular.Median(angles);

            Assert.AreEqual(expected, actual, 1e-15);
        }

        [Test]
        public void MedianTest_Odd()
        {
            double expected = 3.1613437449999998163e-01;
            double actual = Circular.Median(angles.RemoveAt(0));

            Assert.AreEqual(expected, actual, 1e-15);
        }

        [Test]
        public void QuartilesTest()
        {
            DoubleRange range;
            double median = Circular.Quartiles(angles, out range);

            Assert.AreEqual(6.2828279010000001, range.Min);
            Assert.AreEqual(0.60122884799999998, range.Max);
        }

        [Test]
        public void StandardErrorTest()
        {
            double actual = Circular.StandardError(angles, 0.05);

            Assert.AreEqual(3.5294262881192373094e-01, actual, 1e-10);
        }

        [Test]
        public void ClockTest()
        {
            double[] hours =
            {
                1, 23, 1, 23, 1, 23, 1, 23, 1, 23, 1, 23, 1, 23, 1, 23, 1, 23, 1, 23
            };

            double mean = Circular.Mean(hours, 24);
            double stdDev = Circular.StandardDeviation(hours, 24);
            double var = Circular.Variance(hours, 24);
            double med = Circular.Median(hours, 24);

            DoubleRange quartiles;
            double median = Circular.Quartiles(hours, 24, out quartiles);

            double d = Circular.Distance(1, 23, 24);


            Assert.AreEqual(0, mean);
            Assert.AreEqual(1.0058013608769885, stdDev, 1e-11);
            Assert.AreEqual(0.4971507281317768, var, 1e-11);
            Assert.AreEqual(0, med);
            Assert.AreEqual(0, median);
            Assert.AreEqual(23, quartiles.Min, 1e-13);
            Assert.AreEqual(1, quartiles.Max, 1e-13);
            Assert.AreEqual(2, d, 1e-15);
        }

        [Test]
        public void ClockTest2()
        {
            double[] hours =
            {
                0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12
            };

            double mean = Circular.Mean(hours, 24);
            double stdDev = Circular.StandardDeviation(hours, 24);
            double var = Circular.Variance(hours, 24);
            double med = Circular.Median(hours, 24);

            DoubleRange quartiles;
            double median = Circular.Quartiles(hours, 24, out quartiles);

            double d1 = Circular.Distance(23, 0, 24);
            double d2 = Circular.Distance(0, 23, 24);


            Assert.AreEqual(6, mean, 1e-15);
            Assert.AreEqual(3.9598525326427247, stdDev, 1e-14);
            Assert.AreEqual(6.0653308429985406, var, 1e-14);
            Assert.AreEqual(6, med);
            Assert.AreEqual(6, median);
            Assert.AreEqual(2.5, quartiles.Min);
            Assert.AreEqual(9.5, quartiles.Max);
            Assert.AreEqual(-1, d1, 1e-15);
            Assert.AreEqual(+1, d2, 1e-15);
        }

        [Test]
        public void constant_test()
        {
            double[] hours =
            {
                0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1
            };

            double mean = Circular.Mean(hours, 0.1);
            double stdDev = Circular.StandardDeviation(hours, 0);
            double var = Circular.Variance(hours, 0);
            double med = Circular.Median(hours, 0.1);

            Assert.AreEqual(0, mean, 1e-15);
            Assert.AreEqual(Double.NaN, stdDev, 1e-14);
            Assert.AreEqual(Double.NaN, var, 1e-14);
            Assert.AreEqual(0, med);
        }

    }
}
