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
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Testing;
    using NUnit.Framework;

    [TestFixture]
    public class ShapiroWilkTestTest
    {


        [Test]
        public void ShapiroWilkTest()
        {
            // Tested against R's shapiro.test(x)

            #region doc_test
            // Let's say we would like to determine whether a set
            // of observations come from a normal distribution:
            double[] samples =
            {
                0.11, 7.87, 4.61, 10.14, 7.95, 3.14, 0.46, 4.43, 
                0.21, 4.75, 0.71, 1.52, 3.24, 0.93, 0.42, 4.97,
                9.53, 4.55, 0.47, 6.66 
            };

            // For this, we can use the Shapiro-Wilk test. This test tests the null hypothesis 
            // that samples come from a Normal distribution, vs. the alternative hypothesis that 
            // the samples do not come from such distribution. In other words, should this test
            // come out significant, it means our samples do not come from a Normal distribution.

            // Create a new Shapiro-Wilk test:
            var sw = new ShapiroWilkTest(samples);

            double W = sw.Statistic; // should be 0.90050
            double p = sw.PValue;    // should be 0.04209
            bool significant = sw.Significant; // should be true

            // The test is significant, therefore we should reject the null 
            // hypothesis that the samples come from a Normal distribution. 
            #endregion

            /* 
                samples <- c(0.11, 7.87, 4.61, 10.14, 7.95, 3.14, 0.46, 4.43, 0.21,
                  + 4.75, 0.71, 1.52, 3.24, 0.93, 0.42, 4.97, 9.53, 4.55, 0.47, 6.66)
             
                shapiro.test(samples)
             
                W = 0.90050, p-value = 0.04209
             */


            Assert.AreEqual(0.90050, W, 1e-4);
            Assert.AreEqual(0.04209, p, 1e-5);
            Assert.IsTrue(significant);
        }

        [Test]
        public void ShapiroWilkTest2()
        {
            // Tested against R's shapiro.test(x)

            double[] samples =
            {
                1.36, 1.14, 2.92, 2.55, 1.46, 1.06, 5.27, 1.11, 3.48,
                1.10, 0.88, 0.51, 1.46, 0.52, 6.20, 1.69, 0.08, 3.67,
                2.81, 3.49
            };

            /* 
                samples <- c(1.36, 1.14, 2.92, 2.55, 1.46, 1.06, 5.27, 1.11, 3.48, +
                  1.10, 0.88, 0.51, 1.46, 0.52, 6.20, 1.69, 0.08, 3.67, 2.81, 3.49)
             
                W = 0.89090000, p-value = 0.02798
             */

            var target = new ShapiroWilkTest(samples);

            Assert.AreEqual(0.89090, target.Statistic, 1e-4);
            Assert.AreEqual(0.02798, target.PValue, 1e-5);
        }
       
    }
}