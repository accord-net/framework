using System;
using System.Diagnostics;
using Accord.Math;
using Accord.Statistics.Models.Regression.Nonlinear.Fitting;
using NUnit.Framework;

namespace Accord.Tests.Statistics.Models.Regression
{
    [TestFixture]
    public class NonNegativeLeastSquaresTests
    {
        [Test]
        public void ExampleTest()
        {
            // Suppose we would like to map the continuous values in the
            // second column to the integer values in the first column.
            var inputs = new[]
            {
                new[] { 1.0, 1.0 },
                new[] { 2.0, 4.0 },
                new[] { 3.0, 9.0 },
                new[] { 4.0, 16.0 },
            };

            var ouputs = new[] { 0.6, 2.2, 4.8, 8.4 };

            var nnls = new NonNegativeLeastSquares(inputs, ouputs);

            nnls.Fit(100);
            Assert.AreEqual(0.1, nnls.Coefficients[0], 1e-3);
            Assert.AreEqual(0.5, nnls.Coefficients[1], 1e-3);
        }

        [Test]
        public void ExampleTest2()
        {
            // Suppose we would like to map the continuous values in the
            // second column to the integer values in the first column.
            var inputs = new[]
            {
                new[] { 1.0, 1.0, 1.0 },
                new[] { 2.0, 4.0, 8.0 },
                new[] { 3.0, 9.0, 27.0 },
                new[] { 4.0, 16.0, 64.0 },
            };

            var ouputs = new[] { 0.73, 3.24, 8.31, 16.72 };

            var nnls = new NonNegativeLeastSquares(inputs, ouputs);

            nnls.Fit(100);
            Assert.AreEqual(0.1, nnls.Coefficients[0], 1e-3);
            Assert.AreEqual(0.5, nnls.Coefficients[1], 1e-3);
            Assert.AreEqual(0.13, nnls.Coefficients[2], 1e-3);
        }

        [Test]
        public void ExampleTest3()
        {
            // Suppose we would like to map the continuous values in the
            // second column to the integer values in the first column.
            var inputs = new[]
            {
                new[] { 1.0, 1.0, 1.0, 1.0 },
                new[] { 2.0, 4.0, 8.0, 16.0 },
                new[] { 3.0, 9.0, 27.0, 81.0 },
                new[] { 4.0, 16.0, 64.0, 256.0 },
            };

            var ouputs = new[] { 0.73, 3.24, 8.31, 16.72 };

            var nnls = new NonNegativeLeastSquares(inputs, ouputs);

            nnls.Fit(100);
            Assert.AreEqual(0.1, nnls.Coefficients[0], 1e-3);
            Assert.AreEqual(0.5, nnls.Coefficients[1], 1e-3);
            Assert.AreEqual(0.13, nnls.Coefficients[2], 1e-3);
            Assert.AreEqual(0, nnls.Coefficients[3], 1e-3);
        }

        [Test]
        public void ExampleTest4()
        {
            // Suppose we would like to map the continuous values in the
            // second column to the integer values in the first column.
            var inputs = new[]
            {
                new[] { 1.0, 1.0, 1.0 },
                new[] { 2.0, 4.0, 8.0 },
                new[] { 3.0, 9.0, 27.0 },
                new[] { 4.0, 16.0, 64.0 },
            };

            var ouputs = new[] { 0.23, 1.24, 3.81, 8.72 };

            var nnls = new NonNegativeLeastSquares(inputs, ouputs);

            nnls.Fit(100);
            Assert.AreEqual(0.1, nnls.Coefficients[0], 1e-3);
            Assert.AreEqual(0, nnls.Coefficients[1], 1e-3);
            Assert.AreEqual(0.13, nnls.Coefficients[2], 1e-3);
        }
    }
}
