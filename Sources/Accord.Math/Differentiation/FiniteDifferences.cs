// Accord Math Library
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

namespace Accord.Math.Differentiation
{
    using System;
    using Accord.Math;

    /// <summary>
    ///   Derivative approximation by finite differences.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   Numerical differentiation is a technique of numerical analysis to produce an estimate
    ///   of the derivative of a mathematical function or function subroutine using values from 
    ///   the function and perhaps other knowledge about the function.</para>
    ///   
    /// <para>
    ///   A finite difference is a mathematical expression of the form f(x + b) − f(x + a). If a
    ///   finite difference is divided by b − a, one gets a difference quotient. The approximation
    ///   of derivatives by finite differences plays a central role in finite difference methods 
    ///   for the numerical solution of differential equations, especially boundary value problems.
    /// </para>
    /// 
    /// <para>
    ///   This class implements Newton's finite differences method for approximating the derivatives 
    ///   of a multivariate function. A simplified version of the class is also available for 
    ///   <see cref="Derivative(System.Func{double, double}, double, int)">univariate functions through
    ///   its Derivative static methods</see>.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Finite_difference">
    ///       Wikipedia, The Free Encyclopedia. Finite difference. Available on: 
    ///       http://en.wikipedia.org/wiki/Finite_difference </a></description></item>
    ///     <item><description>
    ///     Trent F. Guidry, Calculating derivatives of a function numerically. Available on:
    ///     http://www.trentfguidry.net/post/2009/07/12/Calculate-derivatives-function-numerically.aspx
    ///     </description></item>
    ///     </list>
    ///  </para>
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    /// // Create a simple function with two parameters: f(x, y) = x² + y
    /// Func &lt;double[], double> function = x => Math.Pow(x[0], 2) + x[1];
    /// 
    /// // The gradient function should be g(x,y) = &lt;2x, 1>
    /// 
    /// // Create a new finite differences calculator
    /// var calculator = new FiniteDifferences(2, function);
    /// 
    /// // Evaluate the gradient function at the point (2, -1)
    /// double[] result = calculator.Compute(2, -1); // answer is (4, 1)
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="Accord.Math.Integration"/>
    /// 
    public class FiniteDifferences
    {
        private int parameters;
        private int pointCount;

        private double[] stepSizes;
        private int[] orders;

        private double[][,] coefficients; // differential coefficients



        /// <summary>
        ///   Gets or sets the function to be differentiated.
        /// </summary>
        /// 
        public Func<double[], double> Function { get; set; }

        /// <summary>
        ///   Gets or sets the relative step size used to
        ///   approximate the derivatives. Default is 1e-2.
        /// </summary>
        /// 
        public double[] StepSizes
        {
            get { return stepSizes; }
        }

        /// <summary>
        ///   Gets or sets the order of the derivatives to be
        ///   obtained. Default is 1 (computes the first derivative).
        /// </summary>
        /// 
        public double[] Orders
        {
            get { return Orders; }
        }

        /// <summary>
        ///   Gets or sets the number of points to be used when 
        ///   computing the approximation. Default is 3.
        /// </summary>
        /// 
        public int Points
        {
            get { return pointCount; }
            set
            {
                pointCount = value;
                this.coefficients = CreateCoefficients(value);
            }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="FiniteDifferences"/> class.
        /// </summary>
        /// 
        /// <param name="variables">The number of free parameters in the function.</param>
        /// 
        public FiniteDifferences(int variables)
        {
            init(null, variables, 1, 1e-2);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="FiniteDifferences"/> class.
        /// </summary>
        /// 
        /// <param name="variables">The number of free parameters in the function.</param>
        /// <param name="order">The derivative order that should be obtained. Default is 1.</param>
        /// 
        public FiniteDifferences(int variables, int order)
        {
            init(null, variables, order, 1e-2);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="FiniteDifferences"/> class.
        /// </summary>
        /// 
        /// <param name="variables">The number of free parameters in the function.</param>
        /// <param name="order">The derivative order that should be obtained. Default is 1.</param>
        /// <param name="stepSize">The relative step size used to approximate the derivatives. Default is 0.01.</param>
        /// 
        public FiniteDifferences(int variables, int order, double stepSize)
        {
            init(null, variables, order, stepSize);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="FiniteDifferences"/> class.
        /// </summary>
        /// 
        /// <param name="variables">The number of free parameters in the function.</param>
        /// <param name="function">The function to be differentiated.</param>
        /// 
        public FiniteDifferences(int variables, Func<double[], double> function)
        {
            init(function, variables, 1, 1e-2);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="FiniteDifferences"/> class.
        /// </summary>
        /// 
        /// <param name="variables">The number of free parameters in the function.</param>
        /// <param name="order">The derivative order that should be obtained. Default is 1.</param>
        /// <param name="function">The function to be differentiated.</param>
        /// 
        public FiniteDifferences(int variables, Func<double[], double> function, int order)
        {
            init(function, variables, order, 1e-2);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="FiniteDifferences"/> class.
        /// </summary>
        /// 
        /// <param name="variables">The number of free parameters in the function.</param>
        /// <param name="order">The derivative order that should be obtained. Default is 1.</param>
        /// <param name="stepSize">The relative step size used to approximate the derivatives. Default is 0.01.</param>
        /// <param name="function">The function to be differentiated.</param>
        /// 
        public FiniteDifferences(int variables, Func<double[], double> function, int order, double stepSize)
        {
            init(function, variables, order, stepSize);
        }

        private void init(Func<double[], double> func, int variables,
            int order, double stepSize)
        {
            this.Function = func;
            this.parameters = variables;
            this.stepSizes = new double[variables];
            this.orders = new int[variables];
            this.pointCount = 3;

            this.coefficients = CreateCoefficients(3);

            for (int i = 0; i < stepSizes.Length; i++)
                stepSizes[i] = stepSize;

            for (int i = 0; i < orders.Length; i++)
                orders[i] = order;
        }


        /// <summary>
        ///   Computes the gradient at the given point <c>x</c>.
        /// </summary>
        /// <param name="x">The point where to compute the gradient.</param>
        /// <returns>The gradient of the function evaluated at point <c>x</c>.</returns>
        /// 
        public double[] Compute(params double[] x)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            if (x.Length != parameters)
                throw new ArgumentException("The number of dimensions does not match.", "x");

            if (Function == null)
                throw new InvalidOperationException("The Function has not been defined.");

            double output = Function(x);

            double[] gradient = new double[x.Length];
            for (int i = 0; i < gradient.Length; i++)
                gradient[i] = derivative(x, i, output);

            return gradient;
        }

        /// <summary>
        ///   Computes the gradient at the given point <paramref name="x"/>, 
        ///   storing the result at <paramref name="gradient"/>.
        /// </summary>
        /// 
        /// <param name="x">The point where to compute the gradient.</param>
        /// <param name="gradient">The gradient of the function evaluated at point <c>x</c>.</param>
        /// 
        public void Compute(double[] x, double[] gradient)
        {
            if (x.Length < gradient.Length)
            {
                throw new DimensionMismatchException("gradient",
                    "Gradient vector must have at least the same size as x.");
            }

            if (Function == null)
                throw new InvalidOperationException("The Function has not been defined.");

            double output = Function(x);

            for (int i = 0; i < gradient.Length; i++)
                gradient[i] = derivative(x, i, output);
        }

        /// <summary>
        ///   Computes the derivative at point <c>x_i</c>.
        /// </summary>
        /// 
        private double derivative(double[] x, int index, double output)
        {
            // Saves the original parameter value

            double original = x[index];
            int order = orders[index];
            double step = stepSizes[index];

            if (original != 0.0)
                step *= System.Math.Abs(original);

            // Create the interpolation points
            var points = new double[pointCount];
            int center = (points.Length - 1) / 2;

            for (int i = 0; i < points.Length; i++)
            {
                if (i != center)
                {
                    // Make a small perturbation in the original parameter
                    x[index] = original + (i - center) * step;

                    // Recompute the function to measure its importance
                    points[i] = Function(x);
                }
                else
                {
                    // The center point is the original function
                    points[i] = output;
                }
            }

            // Reverts the modified value
            x[index] = original;

            return Interpolate(coefficients, points, order, center, step);
        }



        /// <summary>
        ///   Creates the interpolation coefficients.
        /// </summary>
        /// 
        /// <param name="points">The number of points in the tableau.</param>
        /// 
        public static double[][,] CreateCoefficients(int points)
        {
            // Compute difference coefficient table
            double[][,] c = new double[points][,];

            double fac = Special.Factorial(points);

            for (int i = 0; i < points; i++)
            {
                double[,] deltas = new double[points, points];

                for (int j = 0; j < points; j++)
                {
                    double h = 1.0;
                    double delta = j - i;

                    for (int k = 0; k < points; k++)
                    {
                        deltas[j, k] = h / Special.Factorial(k);
                        h *= delta;
                    }
                }

                c[i] = Matrix.Inverse(deltas);

                for (int j = 0; j < points; j++)
                    for (int k = 0; k < points; k++)
                        c[i][j, k] = (Math.Round(c[i][j, k] * fac, MidpointRounding.AwayFromZero)) / fac;
            }

            return c;
        }

        /// <summary>
        ///   Interpolates the points to obtain an estimative of the derivative at x.
        /// </summary>
        /// 
        private static double Interpolate(double[][,] coefficients, double[] points,
            int order, int center, double step)
        {
            double sum = 0.0;

            for (int i = 0; i < points.Length; i++)
            {
                double c = coefficients[center][order, i];

                if (c != 0)
                    sum += c * points[i];
            }

            return sum / Math.Pow(step, order);
        }






        private static double[][,] coefficientCache = FiniteDifferences.CreateCoefficients(3);


        /// <summary>
        ///   Computes the derivative for a simpler unidimensional function.
        /// </summary>
        /// 
        /// <param name="function">The function to be differentiated.</param>
        /// <param name="value">The value <c>x</c> at which the derivative should be evaluated.</param>
        /// <param name="order">The derivative order that should be obtained. Default is 1.</param>
        /// 
        /// <returns>The derivative of the function at the point <paramref name="value">x</paramref>.</returns>
        /// 
        public static double Derivative(Func<double, double> function, double value, int order)
        {
            return Derivative(function, value, order, 0.01);
        }

        /// <summary>
        ///   Computes the derivative for a simpler unidimensional function.
        /// </summary>
        /// 
        /// <param name="function">The function to be differentiated.</param>
        /// <param name="value">The value <c>x</c> at which the derivative should be evaluated.</param>
        /// 
        /// <returns>The derivative of the function at the point <paramref name="value">x</paramref>.</returns>
        /// 
        public static double Derivative(Func<double, double> function, double value)
        {
            return Derivative(function, value, 1);
        }

        /// <summary>
        ///   Computes the derivative for a simpler unidimensional function.
        /// </summary>
        /// 
        /// <param name="function">The function to be differentiated.</param>
        /// <param name="order">The derivative order that should be obtained. Default is 1.</param>
        /// <param name="stepSize">The relative step size used to approximate the derivatives. Default is 0.01.</param>
        /// <param name="value">The value <c>x</c> at which the derivative should be evaluated.</param>
        /// 
        /// <returns>The derivative of the function at the point <paramref name="value">x</paramref>.</returns>
        /// 
        public static double Derivative(Func<double, double> function, double value, int order, double stepSize)
        {
            double output = function(value);
            double original = value;

            if (Math.Abs(original) > 1e-10)
                stepSize *= System.Math.Abs(original);
            else stepSize = 1e-10;


            // Create the interpolation points
            double[] outputs = new double[coefficientCache.Length];

            int center = (outputs.Length - 1) / 2;
            for (int i = 0; i < outputs.Length; i++)
            {
                if (i != center)
                {
                    // Recompute the function to measure its importance
                    outputs[i] = function(original + (i - center) * stepSize);
                }
                else
                {
                    // The center point is the original function
                    outputs[i] = output;
                }
            }

            return FiniteDifferences.Interpolate(coefficientCache,
                outputs, order, center, stepSize);
        }

        /// <summary>
        ///   Obtains the gradient function for a multidimensional function.
        /// </summary>
        /// 
        /// <param name="function">The function to be differentiated.</param>
        /// <param name="variables">The number of parameters for the function.</param>
        /// <param name="order">The derivative order that should be obtained. Default is 1.</param>
        /// 
        /// <returns>The gradient function of the given <paramref name="function"/>.</returns>
        /// 
        public static Func<double[], double[]> Gradient(Func<double[], double> function, int variables, int order= 1)
        {
            return new FiniteDifferences(variables, function).Compute;
        }

    }
}
