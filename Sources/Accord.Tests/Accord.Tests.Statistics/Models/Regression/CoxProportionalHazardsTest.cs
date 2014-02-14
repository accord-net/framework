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
    using System;
    using Accord.Math;
    using Accord.Statistics.Models.Regression;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Statistics.Distributions.Univariate;

    [TestClass()]
    public class CoxProportionalHazardsTest
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
        public void RunTest()
        {
            // Data from: http://www.sph.emory.edu/~cdckms/CoxPH/prophaz2.html

            double[,] data =
            {
                { 50,  1, 0 },
                { 70,  2, 1 },
                { 45,  3, 0 },
                { 35,  5, 0 },
                { 62,  7, 1 },
                { 50, 11, 0 },
                { 45,  4, 0 },
                { 57,  6, 0 },
                { 32,  8, 0 },
                { 57,  9, 1 },
                { 60, 10, 1 },
            };

            ProportionalHazards regression = new ProportionalHazards(1);

            regression.Coefficients[0] = 0.37704239281494084;
            regression.StandardErrors[0] = 0.25415755113043753;

            double[][] inputs = data.GetColumn(0).ToArray();
            double[] time = data.GetColumn(1);
            int[] output = data.GetColumn(2).ToInt32();


            {
                double actual = -2 * regression.GetPartialLogLikelihood(inputs, time, output);
                double expected = 4.0505;
                Assert.AreEqual(expected, actual, 1e-4);
                Assert.IsFalse(Double.IsNaN(actual));
            }

            {
                var test = regression.GetWaldTest(0);
                Assert.AreEqual(0.1379, test.PValue, 1e-4);
            }

            {
                var ci = regression.GetConfidenceInterval(0);
                Assert.AreEqual(0.8859, ci.Min, 1e-4);
                Assert.AreEqual(2.3993, ci.Max, 1e-4);
            }


            {
                double actual = regression.GetHazardRatio(0);
                double expected = 1.4580;
                Assert.AreEqual(expected, actual, 1e-4);
            }

            {
                var chi = regression.ChiSquare(inputs, time, output);
                Assert.AreEqual(7.3570, chi.Statistic, 1e-4);
                Assert.AreEqual(1, chi.DegreesOfFreedom);
                Assert.AreEqual(0.0067, chi.PValue, 1e-3);
            }

        }


        [TestMethod()]
        public void PredictTest1()
        {
            // Data from: http://www.sph.emory.edu/~cdckms/CoxPH/prophaz2.html

            double[,] data =
            {
                { 50,  1, 0 },
                { 70,  2, 1 },
                { 45,  3, 0 },
                { 35,  5, 0 },
                { 62,  7, 1 },
                { 50, 11, 0 },
                { 45,  4, 0 },
                { 57,  6, 0 },
                { 32,  8, 0 },
                { 57,  9, 1 },
                { 60, 10, 1 },
            };

            double[] distHazards = 
            {
               0, 0.0351683340828711, 0.0267358118285064, 0, 
               0.0103643094219679, 0, 0, 0, 0, 0.000762266794052363, 0
            };

            double[] distTimes =
            {
                11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1
            };

            ProportionalHazards regression = new ProportionalHazards(1,
                new EmpiricalHazardDistribution(distTimes, distHazards));

            regression.Coefficients[0] = 0.37704239281494084;
            regression.StandardErrors[0] = 0.25415755113043753;
            regression.Offsets[0] = 51.181818;

            double[][] inputs = data.GetColumn(0).ToArray();
            double[] time = data.GetColumn(1);
            int[] output = data.GetColumn(2).ToInt32();


            double[] expected = 
            {
                0.000000000000, 0.919466527073, 0.000074105451, 0.000001707560,
                0.657371730925, 0.046771996036, 0.000074105451, 0.006836271860,
                0.000008042445, 0.339562971888, 2.029832541310 
            };

            double[] actual = new double[inputs.Length];
            for (int i = 0; i < inputs.Length; i++)
                actual[i] = regression.Compute(inputs[i], time[i]);

            for (int i = 0; i < actual.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i], 1e-6);
                Assert.IsFalse(Double.IsNaN(actual[i]));
            }
        }

    }
}
