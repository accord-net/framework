// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
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
    using Accord.Statistics.Models.Regression.Linear;
    using Accord.Math;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Globalization;

    [TestClass()]
    public class LogarithmRegressionTest
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
        public void LogarithmRegressionRegressTest()
        {
            // This is the same data from the example available at
            // http://mathbits.com/MathBits/TISection/Statistics2/logarithmic.htm

            // Declare your inputs and output data
            double[] inputs = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
            double[] outputs = { 6, 9.5, 13, 15, 16.5, 17.5, 18.5, 19, 19.5, 19.7, 19.8 };

            // Transform inputs to logarithms
            double[] logx = Matrix.Log(inputs);

            // Compute a simple linear regression
            var lr = new SimpleLinearRegression();

            // Compute with the log-transformed data
            double error = lr.Regress(logx, outputs);

            // Get an expression representing the learned regression model
            // We just have to remember that 'x' will actually mean 'log(x)'
            string result = lr.ToString("N4", CultureInfo.InvariantCulture);

            // Result will be "y(x) = 6.1082x + 6.0993"
            

            Assert.AreEqual(6.1081800414945704, lr.Slope);
            Assert.AreEqual(6.0993411396126653, lr.Intercept);
            Assert.AreEqual("y(x) = 6.1082x + 6.0993", result);
        }
    }

}
