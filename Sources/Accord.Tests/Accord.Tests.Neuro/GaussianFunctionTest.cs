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

namespace Accord.Tests.Neuro
{
    using Accord.Neuro.Networks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Accord.Neuro.ActivationFunctions;
    using Accord.Neuro.Learning;
    using Accord.Neuro.Layers;

    [TestClass()]
    public class GaussianFunctionTest
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
        public void ConstructorTest()
        {
            // Create a Gaussian function with slope alpha = 4.2
            GaussianFunction function = new GaussianFunction(4.2);

            // Computes the function output (linear, y = alpha * x)
            double y = function.Function(x: 0.2); // 4.2 * 2 = 0.48

            // Draws a sample from a Gaussian distribution with
            // mean given by the function output y (previously 0.48)
            double z = function.Generate(x: 0.4); // (random, between 0 and 1)

            // Please note that the above is completely equivalent 
            // to computing the line below (remember, 0.48 == y)
            double w = function.Generate2(y: 0.48); // (random, between 0 and 1)


            // We can also compute the derivative of the sigmoid function
            double d = function.Derivative(x: 0.2); // 4.2 (the slope)

            // Or compute the derivative given the functions' output y
            double e = function.Derivative2(y: 0.2); // 4.2 (the slope)

            Assert.AreEqual(0.84, y, 1e-7);
            Assert.AreEqual(4.2, d, 1e-7);
            Assert.AreEqual(4.2, e, 1e-7);
        }

    }
}
