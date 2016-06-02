﻿// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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

namespace Accord.Tests.Math
{
    using Accord.Math.Optimization;
    using NUnit.Framework;
    using System;
    using Accord.Math;


    [TestFixture]
    public class BrentSearchTest
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


        [Test]
        public void ConstructorTest()
        {

            // Suppose we were given the function x³ + 2x² - 10x and 
            // we have to find its root, maximum and minimum inside 
            // the interval [-4,3]. First, we express this function
            // as a lambda expression:
            Func<double, double> function = x => x * x * x + 2 * x * x - 10 * x;

            // And now we can create the search algorithm:
            BrentSearch search = new BrentSearch(function, -4, 3);

            // Finally, we can query the information we need
            Assert.IsTrue(search.Maximize());  // occurs at -2.61
            double max = search.Solution;
            Assert.IsTrue(search.Minimize());  // occurs at  1.27
            double min = search.Solution;
            Assert.IsTrue(search.FindRoot()); // occurs at  0.50
            double root = search.Solution;

            Assert.AreEqual(-2.6103173042172645, max);
            Assert.AreEqual(1.2769840667540548, min);
            Assert.AreEqual(-0.5, root);
        }


        [Test]
        public void FindRootTest()
        {
            //  Example from http://en.wikipedia.org/wiki/Brent%27s_method

            Func<double, double> f = x => (x + 3) * Math.Pow((x - 1), 2);
            double a = -4;
            double b = 4 / 3.0;

            double expected = -3;
            double actual = BrentSearch.FindRoot(f, a, b);

            Assert.AreEqual(expected, actual, 1e-6);
            Assert.IsFalse(Double.IsNaN(actual));
        }


        [Test]
        public void MaximizeTest()
        {
            Func<double, double> f = x => -2 * x * x - 3 * x + 5;

            double expected = -3 / 4.0;
            double actual = BrentSearch.Maximize(f, -200, +200);

            Assert.AreEqual(expected, actual, 1e-10);
        }


        [Test]
        public void MinimizeTest()
        {
            Func<double, double> f = x => 2 * x * x - 3 * x + 5;

            double expected = 3 / 4.0;
            double actual = BrentSearch.Minimize(f, -200, +200);

            Assert.AreEqual(expected, actual, 1e-10);
        }
    }
}
