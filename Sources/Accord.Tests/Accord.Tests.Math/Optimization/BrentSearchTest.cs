

namespace Accord.Tests.Math
{
    using Accord.Math.Optimization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Accord.Math;


    [TestClass()]
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


        [TestMethod()]
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
            double max = search.Maximize();  // occurs at -2.61
            double min = search.Minimize();  // occurs at  1.27
            double root = search.FindRoot(); // occurs at  0.50

            Assert.AreEqual(-2.6103173042172645, max);
            Assert.AreEqual(1.2769840667540548, min);
            Assert.AreEqual(-0.5, root);
        }


        [TestMethod()]
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


        [TestMethod()]
        public void MaximizeTest()
        {
            Func<double, double> f = x => -2 * x * x - 3 * x + 5;

            double expected = -3 / 4.0;
            double actual = BrentSearch.Maximize(f, -200, +200);

            Assert.AreEqual(expected, actual, 1e-10);
        }


        [TestMethod()]
        public void MinimizeTest()
        {
            Func<double, double> f = x => 2 * x * x - 3 * x + 5;

            double expected = 3 / 4.0;
            double actual = BrentSearch.Minimize(f, -200, +200);

            Assert.AreEqual(expected, actual, 1e-10);
        }
    }
}
