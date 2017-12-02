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

namespace Accord.Tests.Statistics
{
    using Accord.Statistics.TimeSeries;
    using NUnit.Framework;
    using Accord.Statistics;
    using System;
    using Accord.Math;

    [TestFixture]
    public class StatToolsTest
    {
        [Test]
        public void acfTest()
        {
            // expectedACF is ACF results from Python StatsModels ACF function applied to Sin(X) where X=0, 1, .., 99
            /*
             Python 3.5.2 |Anaconda 4.2.0 (64-bit)| (default, Jul  5 2016, 11:41:13) [MSC v.1900 64 bit (AMD64)] on win32
             Type "help", "copyright", "credits" or "license" for more information.
             >>> import statsmodels.api as sm
             >>> from statsmodels.tsa.stattools import acf
             >>> import numpy as np             
             >>> x = np.sin(np.linspace(0, 99, 100))
             >>> y = acf(x, nlags=9)
             >>> print(y)
             [ 1.          0.53515447 -0.4075514  -0.96025719 -0.62773328  0.2691908
               0.90248604  0.70133678 -0.13356576 -0.82902385 -0.75534937]
             >>>
             */

            double[] expectedACF = new double[] { 1.0, 0.53515447, -0.4075514, -0.96025719, -0.62773328, 0.2691908, 0.90248604, 0.70133678, -0.13356576, -0.82902385 };

            int windowSize = expectedACF.Length;

            double[] values = new double[100];
            for (int i = 0; i < values.Length; i++)
                values[i] = Math.Sin(i);

            double[] computedACF = TimeSeriesTools.AutoCorrelationFunction(values, windowSize);

            Assert.IsTrue(expectedACF.IsEqual(computedACF, 1e-6));
        }
    }
}