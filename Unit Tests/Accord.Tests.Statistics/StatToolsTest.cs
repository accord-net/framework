// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Liang Xie 2017
// xie1978 at hotmail dot com
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

namespace Accord.Tests.Statistics.TimeSeries
{
    using Accord.Statistics.TimeSeries;
    using NUnit.Framework;
    using Accord.Statistics;
    using System;

    [TestFixture]
    public class StatToolsTest
    {
        [Test]
        public void acfTest()
        {
            int SeriesSize = 100;
            double[] values = new double[SeriesSize];
            for (int i = 0; i < SeriesSize; i++)
                values[i] = Math.Sin(i);

            double[] expectedACF = new double[] 
            {
                1.0, 0.53515447, -0.4075514, -0.96025719, -0.62773328,
                0.2691908, 0.90248604, 0.70133678, -0.13356576, -0.82902385
            };

            int windowSize = values.Length;

            double[] computedACF = TimeSeriesTools.AutoCorrelationFunction(values, 10);

            Assert.AreEqual(expectedACF, computedACF);
        }
    }
}