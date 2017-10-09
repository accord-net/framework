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
    using Accord.Statistics.Kernels;
    using Accord.Math;
    using NUnit.Framework;
    using System.Diagnostics;

    [TestFixture]
    public class GaussianDtwTest
    {

        [Test]
        public void GaussianFunctionTest()
        {
            var x = new double[] { 0, 4, 2, 1 };
            var y = new double[] { 3, 2, };

            var dtw = new DynamicTimeWarping(1);
            IKernel gaussian = new Gaussian<DynamicTimeWarping>(dtw, 1);

            double actual = gaussian.Function(x, y);

            Assert.AreEqual(0.3407192298459587, actual, 1e-10);


            gaussian = new Gaussian<DynamicTimeWarping>(dtw, 11.5);

            x = new double[] { 0.2, 5 };
            y = new double[] { 3, 0.7 };

            actual = gaussian.Function(x, y);

            Assert.AreEqual(0.99065918303292089, actual, 1e-10);
        }

        [Test]
        public void GammaSigmaTest()
        {
            var dtw = new DynamicTimeWarping(1);
            var gaussian = new Gaussian<DynamicTimeWarping>(dtw, 1);

            double expected, actual, gamma, sigma;

            expected = 0.01;
            gaussian.Sigma = expected;
            gamma = gaussian.Gamma;

            gaussian.Gamma = gamma;
            actual = gaussian.Sigma;

            Assert.AreEqual(expected, actual);


            expected = 0.01;
            gaussian.Gamma = expected;
            sigma = gaussian.Sigma;

            gaussian.Sigma = sigma;
            actual = gaussian.Gamma;

            Assert.AreEqual(expected, actual, 1e-12);
        }

        [Test]
        public void GammaSigmaSquaredTest()
        {
            var dtw = new DynamicTimeWarping(1);
            var gaussian = new Gaussian<DynamicTimeWarping>(dtw, 3.6);

            Assert.AreEqual(3.6 * 3.6, gaussian.SigmaSquared);
            Assert.AreEqual(3.6, gaussian.Sigma);
            Assert.AreEqual(1.0 / (2 * 3.6 * 3.6), gaussian.Gamma);

            gaussian.SigmaSquared = 81;
            Assert.AreEqual(81, gaussian.SigmaSquared);
            Assert.AreEqual(9, gaussian.Sigma);
            Assert.AreEqual(1.0 / (2 * 81), gaussian.Gamma);

            gaussian.Sigma = 6;
            Assert.AreEqual(36, gaussian.SigmaSquared);
            Assert.AreEqual(6, gaussian.Sigma);
            Assert.AreEqual(1.0 / (2 * 36), gaussian.Gamma);

            gaussian.Gamma = 1.0 / (2 * 49);
            Assert.AreEqual(49, gaussian.SigmaSquared, 1e-10);
            Assert.AreEqual(7, gaussian.Sigma, 1e-10);
            Assert.AreEqual(1.0 / (2 * 49), gaussian.Gamma);
        }

        [Test]
        public void GaussianEstimateTest()
        {
            // Suppose we have the following data 
            // 
            double[][] data =  
            { 
                new double[] { 5.1, 3.5, 1.4, 0.2 }, 
                new double[] { 5.0, 3.6, 1.4, 0.2 }, 
                new double[] { 4.9, 3.0, 1.4, 0.2 }, 
                new double[] { 5.8, 4.0, 1.2, 0.2 }, 
                new double[] { 4.7, 3.2, 1.3, 0.2 }, 
            };

            // Estimate an appropriate sigma from data 
            var dtw = new DynamicTimeWarping(1);
            var kernel = Gaussian.Estimate(dtw, data);
            double sigma = kernel.Sigma; 
            double sigma2 = kernel.SigmaSquared;
            Assert.AreEqual(0.044282096049367413, sigma);
            Assert.AreEqual(sigma * sigma, sigma2);
        }

        [Test]
        public void FunctionTest_EqualInputs()
        {
            var x = new double[] { 1, 2, 5, 1 };
            var y = new double[] { 1, 2, 5, 1 };

            var target = new Gaussian<DynamicTimeWarping>(new DynamicTimeWarping(1), 4.2);

            double expected = target.Function(x, y);
            double actual = target.Function(x, x);

            Assert.AreEqual(expected, actual, 0.000001);
        }

    }
}
