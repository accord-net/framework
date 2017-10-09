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

namespace Accord.Tests.Math
{
    using Accord.Math.Optimization;
    using NUnit.Framework;
    using System;
    using Accord.Math;


    [TestFixture]
    public class BrentSearchTest
    {

        [Test]
        public void ConstructorTest()
        {
            #region doc_example
            // Suppose we were given the function x³ + 2x² - 10x + 1 and 
            // we have to find its root, maximum and minimum inside 
            // the interval [-4, 2]. First, we express this function
            // as a lambda expression:
            Func<double, double> function = x => x * x * x + 2 * x * x - 10 * x + 1;

            // And now we can create the search algorithm:
            BrentSearch search = new BrentSearch(function, -4, 2);

            // Finally, we can query the information we need
            bool success1 = search.Maximize();  // should be true
            double max = search.Solution;       // occurs at -2.61

            bool success2 = search.Minimize();   // should be true  
            double min = search.Solution;       // occurs at  1.28

            bool success3 = search.FindRoot();  // should be true 
            double root = search.Solution;      // occurs at  0.10
            double value = search.Value;        // should be zero
            #endregion

            Assert.IsTrue(success1);
            Assert.IsTrue(success2);
            Assert.IsTrue(success3);
            Assert.AreEqual(-2.6103173073566239, max);
            Assert.AreEqual(1.2769839857480398, min);
            Assert.AreEqual(0.10219566016872624, root);
            Assert.AreEqual(0, value, 1e-5);
        }


        [Test]
        public void CreateSearchWithParametersSetPropertiesSetsCorrectly()
        {
            Func<double, double> f = x => 2 * x + 4;

            double lowerBound = -3;
            double upperBound = +5;
            double tolerance = 1e-9;
            int maxIterations = 20;

            BrentSearch sut = new BrentSearch(f, lowerBound, upperBound, tolerance, maxIterations);

            Assert.AreSame(f, sut.Function);
            Assert.AreEqual(lowerBound, sut.LowerBound);
            Assert.AreEqual(upperBound, sut.UpperBound);
            Assert.AreEqual(tolerance, sut.Tolerance);
            Assert.AreEqual(maxIterations, sut.MaxIterations);
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

            var search = new BrentSearch(f, a, b);
            bool isSuccess = search.FindRoot();

            Assert.True(isSuccess);
            Assert.AreEqual(BrentSearchStatus.Success, search.Status);
            Assert.AreEqual(expected, search.Solution, 1e-6);
            Assert.That(Math.Abs(search.Value), Is.LessThan(1e-5));
        }


        [Test]
        public void FindTest()
        {
            //  Example from http://en.wikipedia.org/wiki/Brent%27s_method
            double value = 10;

            Func<double, double> f = x => (x + 3) * Math.Pow((x - 1), 2) + value;
            double a = -4;
            double b = 4 / 3.0;

            double expected = -3;
            double actual = BrentSearch.Find(f, value, a, b);

            Assert.AreEqual(expected, actual, 1e-6);
            Assert.AreEqual(value, f(actual), 1e-5);

            var search = new BrentSearch(f, a, b);
            bool isSuccess = search.Find(value);

            Assert.True(isSuccess);
            Assert.AreEqual(BrentSearchStatus.Success, search.Status);
            Assert.AreEqual(expected, search.Solution, 1e-6);
            Assert.AreEqual(value, search.Value, 1e-5);
        }


        [Test]
        public void MaximizeTest()
        {
            Func<double, double> f = x => -2 * x * x - 3 * x + 5;

            double expected = -3 / 4d;
            double actual = BrentSearch.Maximize(f, -200, +200);

            Assert.AreEqual(expected, actual, 1e-10);


            var search = new BrentSearch(f, -200, 200);
            bool isSuccess = search.Maximize();

            Assert.True(isSuccess);
            Assert.AreEqual(BrentSearchStatus.Success, search.Status);
            Assert.AreEqual(expected, search.Solution, 1e-10);
            Assert.AreEqual(f(expected), search.Value, double.Epsilon);
        }


        [Test]
        public void MinimizeTest()
        {
            Func<double, double> f = x => 2 * x * x - 3 * x + 5;

            double expected = 3 / 4d;
            double actual = BrentSearch.Minimize(f, -200, +200);

            Assert.AreEqual(expected, actual, 1e-10);


            var search = new BrentSearch(f, -200, 200);
            bool isSuccess = search.Minimize();

            Assert.True(isSuccess);
            Assert.AreEqual(BrentSearchStatus.Success, search.Status);
            Assert.AreEqual(expected, search.Solution, 1e-10);
            Assert.AreEqual(f(expected), search.Value, double.Epsilon);
        }


        [Test]
        public void FindRootNotBoundedThrowsConvergenceException()
        {
            Func<double, double> f = x => (x + 2) * (x - 2);
            double a = -1;
            double b = +1;

            Assert.Throws<ConvergenceException>(() =>
            {
                double actual = BrentSearch.FindRoot(f, a, b);
            });
        }


        [Test]
        public void FindRootNotBoundedFailsToSolve()
        {
            Func<double, double> f = x => (x + 2) * (x - 2);
            double a = -1;
            double b = +1;

            var search = new BrentSearch(f, a, b);
            var isSuccess = search.FindRoot();

            Assert.AreEqual(false, isSuccess);
            Assert.AreEqual(BrentSearchStatus.RootNotBracketed, search.Status);
        }


        [Test]
        public void FindRootBoundedTwiceThrowsConvergenceException()
        {
            Func<double, double> f = x => (x + 2) * (x - 2);
            double a = -3;
            double b = +3;

            Assert.Throws<ConvergenceException>(() =>
            {
                double actual = BrentSearch.FindRoot(f, a, b);
            });
        }


        [Test]
        public void FindRootBoundedTwiceFailsToSolve()
        {
            Func<double, double> f = x => (x + 2) * (x - 2);
            double a = -3;
            double b = +3;

            var search = new BrentSearch(f, a, b);
            var isSuccess = search.FindRoot();

            Assert.AreEqual(false, isSuccess);
            Assert.AreEqual(BrentSearchStatus.RootNotBracketed, search.Status);
        }


        [Test]
        public void FindRootWithLowMaxIterationsThrowsConvergenceException()
        {
            Func<double, double> f = x => (x + 2) * (x - 2);
            double a = -1;
            double b = +3000;

            Assert.Throws<ConvergenceException>(() =>
            {
                double actual = BrentSearch.FindRoot(f, a, b, maxIterations: 5);
            });
        }


        [Test]
        public void FindRootWithLowMaxIterationsFailsToSolveButGivesLastKnownSolution()
        {
            Func<double, double> f = x => (x + 2) * (x - 2);
            double a = -1;
            double b = +3000;

            var search = new BrentSearch(f, a, b, maxIterations: 5);
            var isSuccess = search.FindRoot();

            Assert.AreEqual(false, isSuccess);
            Assert.AreEqual(BrentSearchStatus.MaxIterationsReached, search.Status);

            Assert.That(search.Solution, Is.GreaterThan(a).And.LessThan(b));
        }


        [Test]
        public void MaximizeWithLowMaxIterationsThrowsConvergenceException()
        {
            Func<double, double> f = x => -2 * x * x - 3 * x + 5;
            double a = -200;
            double b = +200;

            Assert.Throws<ConvergenceException>(() =>
            {
                double actual = BrentSearch.Maximize(f, a, b, maxIterations: 10);
            });
        }


        [Test]
        public void MaximizeWithLowMaxIterationsFailsToSolveButGivesLastKnownSolution()
        {
            Func<double, double> f = x => -2 * x * x - 3 * x + 5;
            double a = -200;
            double b = +200;

            var search = new BrentSearch(f, a, b, maxIterations: 10);
            var isSuccess = search.Maximize();

            Assert.AreEqual(false, isSuccess);
            Assert.AreEqual(BrentSearchStatus.MaxIterationsReached, search.Status);

            Assert.That(search.Solution, Is.GreaterThan(a).And.LessThan(b));
        }
    }
}
