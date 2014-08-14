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
    public class BernoulliFunctionTest
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
            // Create a Bernoulli function with sigmoid's alpha = 1
            BernoulliFunction function = new BernoulliFunction();

            // Computes the function output (sigmoid function)
            double y = function.Function(x: 0.4); // 0.5986876

            // Draws a sample from a Bernoulli distribution with
            // mean given by the function output y (given as before)
            double z = function.Generate(x: 0.4); // (random, 0 or 1)

            // Here, z can be either 0 or 1. Since it follows a Bernoulli
            // distribution with mean 0.59, it is expected to be 1 about 
            // 0.59 of the time.

            // Now, please note that the above is completely equivalent 
            // to computing the line below (remember, 0.5986876 == y)
            double w = function.Generate2(y: 0.5986876); // (random, 0 or 1)


            // We can also compute the derivative of the sigmoid function
            double d = function.Derivative(x: 0.4); // 0.240260

            // Or compute the derivative given the functions' output y
            double e = function.Derivative2(y: 0.5986876); // 0.240260

            Assert.AreEqual(0.5986876, y, 1e-7);
            Assert.AreEqual(0.2402607, d, 1e-7);
            Assert.AreEqual(0.2402607, e, 1e-7);
        }

    }
}
