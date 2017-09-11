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
    using System.Threading;
    using Accord.Compat;

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
    ///   <code source="Unit Tests\Accord.Tests.Math\Differentiation\FiniteDifferencesTest.cs" region="doc_gradient" />
    ///   <code source="Unit Tests\Accord.Tests.Math\Differentiation\FiniteDifferencesTest.cs" region="doc_hessian" />
    /// </example>
    /// 
    /// <seealso cref="Accord.Math.Integration"/>
    /// 
    public class FiniteDifferences
    {
        const double DEFAULT_STEPSIZE = 1e-2;
        const int DEFAULT_NPOINTS = 3;
        const int DEFAULT_ORDER = 1;

        private int numberOfVariables;
        private int derivativeOrder; // whether to compute first, second, ... derivatives

        private double[] stepSizes;

        private double[][,] coefficients; // differential coefficients
        private ThreadLocal<double[][]> points; // cache for interpolation points
        private int center;

        /// <summary>
        ///   Gets or sets the function to be differentiated.
        /// </summary>
        /// 
        public Func<double[], double> Function { get; set; }

        /// <summary>
        ///   Gets or sets the relative step size used to approximate the derivatives. Default is 1e-2.
        ///   Setting this property updates the step size for all parameters at once. To adjust only a
        ///   single parameter, please refer to <see cref="StepSizes"/> instead.
        /// </summary>
        /// 
        public double StepSize
        {
            get { return stepSizes.Max(); }
            set
            {
                if (stepSizes == null)
                    stepSizes = new double[numberOfVariables];
                for (int i = 0; i < stepSizes.Length; i++)
                    stepSizes[i] = value;
            }
        }

        /// <summary>
        ///   Gets or sets the relative step sizes used to approximate the derivatives. Default is 1e-2.
        /// </summary>
        /// 
        public double[] StepSizes
        {
            get { return stepSizes; }
        }

        /// <summary>
        ///   Gets or sets the order of the partial derivatives to be
        ///   obtained. Default is 1 (computes the first derivative).
        /// </summary>
        /// 
        public int Order
        {
            get { return derivativeOrder; }
            set
            {
                if (value >= NumberOfPoints)
                {
                    throw new ArgumentException("The order must be less than the number of points being used for interpolation." +
                        " In order to use a higher order, please increase the number of interpolation points first.", "value");
                }

                derivativeOrder = value;
            }
        }


        /// <summary>
        ///   Gets or sets the number of points to be used when 
        ///   computing the approximation. Default is 3.
        /// </summary>
        /// 
        public int NumberOfPoints
        {
            get { return coefficients.Length; }
            set
            {
                if (value % 2 != 1)
                    throw new ArgumentException("The number of points must be odd.", "value");

                if (derivativeOrder >= value)
                {
                    throw new ArgumentException("The number of points must be higher than the desired differentiation order." +
                        " In order to use less interpolation points, please decrease the differentiation order first.", "value");
                }

                this.coefficients = CreateCoefficients(value);
                this.points = new ThreadLocal<double[][]>(() => Jagged.Zeros(2, coefficients.Length));
                this.center = (coefficients.Length - 1) / 2;
            }
        }

        /// <summary>
        ///   Obsolete. Please use <see cref="NumberOfPoints"/> instead.
        /// </summary>
        /// 
        [Obsolete("Please use NumberOfPoints instead.")]
        public int Points
        {
            get { return NumberOfPoints; }
            set { NumberOfPoints = value; }
        }

        /// <summary>
        ///   Gets the number of parameters expected by the <see cref="Function"/> to be differentiated.
        /// </summary>
        /// 
        public int NumberOfVariables
        {
            get { return numberOfVariables; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="FiniteDifferences"/> class.
        /// </summary>
        /// 
        /// <param name="variables">The number of free parameters in the function.</param>
        /// 
        public FiniteDifferences(int variables)
        {
            init(variables: variables);
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
            init(variables: variables, order: order);
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
            init(variables: variables, order: order, stepSize: stepSize);
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
            init(variables, function: function);
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
            init(variables: variables, function: function, order: order);
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
            init(variables: variables, function: function, order: order, stepSize: stepSize);
        }

        private void init(int variables, Func<double[], double> function = null, int? order = null, int? points = null, double? stepSize = null)
        {
            this.numberOfVariables = variables;
            this.Function = function;
            this.StepSize = stepSize.GetValueOrDefault(DEFAULT_STEPSIZE);
            this.NumberOfPoints = points.GetValueOrDefault(DEFAULT_NPOINTS);
            this.Order = order.GetValueOrDefault(DEFAULT_ORDER);
        }


        /// <summary>
        ///   Obsolete. Please use <see cref="Gradient(double[])"/> instead.
        /// </summary>
        /// 
        [Obsolete("Please use Gradient(double[]) instead.")]
        public double[] Compute(params double[] x)
        {
            return Gradient(x);
        }

        /// <summary>
        ///   Obsolete. Please use <see cref="Gradient(double[], double[])"/> instead.
        /// </summary>
        /// 
        [Obsolete("Please use Gradient(double[], double[]) instead.")]
        public void Compute(double[] x, double[] gradient)
        {
            Gradient(x, gradient);
        }

        /// <summary>
        ///   Computes the gradient at the given point <c>x</c>.
        /// </summary>
        /// <param name="x">The point where to compute the gradient.</param>
        /// <returns>The gradient of the function evaluated at point <c>x</c>.</returns>
        /// 
        public double[] Gradient(double[] x)
        {
            var gradient = new double[x.Length];
            Gradient(x, gradient);
            return gradient;
        }

        /// <summary>
        ///   Computes the gradient at the given point <paramref name="x"/>, 
        ///   storing the result at <paramref name="result"/>.
        /// </summary>
        /// 
        /// <param name="x">The point where to compute the gradient.</param>
        /// <param name="result">The gradient of the function evaluated at point <c>x</c>.</param>
        /// 
        public double[] Gradient(double[] x, double[] result)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            if (x.Length != numberOfVariables)
                throw new ArgumentException("The number of dimensions does not match.", "x");

            if (Function == null)
                throw new InvalidOperationException("The Function has not been defined.");

            if (x.Length < result.Length)
            {
                throw new DimensionMismatchException("gradient",
                    "Gradient vector must have at least the same size as x.");
            }

            if (Function == null)
                throw new InvalidOperationException("The Function has not been defined.");

            double[] pointCache = this.points.Value[0];
            double centerValue = Function(x);

            for (int i = 0; i < result.Length; i++)
                result[i] = derivative(Function, x, i, pointCache, centerValue);
            return result;
        }

        /// <summary>
        ///   Computes the Hessian matrix at given point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">The point where to compute the gradient.</param>
        /// 
        /// <returns>The Hessian of the function evaluated at point <c>x</c>.</returns>
        /// 
        public double[][] Hessian(double[] x)
        {
            return Hessian(x, Jagged.Zeros(x.Length, x.Length));
        }

        /// <summary>
        ///   Computes the Hessian matrix at given point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">The point where to compute the gradient.</param>
        /// <param name="result">The matrix where the Hessian should be stored.</param>
        /// 
        /// <returns>The Hessian of the function evaluated at point <c>x</c>.</returns>
        /// 
        public double[][] Hessian(double[] x, double[][] result)
        {
            double[] pointCache1 = this.points.Value[0];
            double[] pointCache2 = this.points.Value[1];
            double centerValue = Function(x);

            for (int i = 0; i < x.Length; i++)
            {
                Func<double[], double> f = (double[] x0) => derivative(Function, x0, i, pointCache2, Function(x0));

                // Process elements at the diagonal
                result[i][i] = derivative(f, x, i, pointCache1, centerValue);

                // Process off-diagonal elements
                for (int j = 0; j < i; j++)
                {
                    double d = derivative(f, x, j, pointCache1, centerValue);
                    result[j][i] = d; // Hessian is symmetric, no need
                    result[i][j] = d; // to compute the derivative twice
                }
            }

            return result;
        }

        /// <summary>
        ///   Computes the derivative at point <c>x_i</c>.
        /// </summary>
        /// 
        private double derivative(Func<double[], double> func, double[] x, int index, double[] points, double centerValue)
        {
            //if (order >= coefficients.Length)
            //{
            //    throw new ArgumentOutOfRangeException("The derivative order needs to be less than the number of " +
            //        "interpolation points. To use a higher order, please adjust the NumberOfPoints property first.");
            //}

            double step = GetUniformlySampledPoints(func, x, index, centerValue, points);

            return Interpolate(this.coefficients, points, derivativeOrder, this.center, step);
        }

        private double GetUniformlySampledPoints(Func<double[], double> func, double[] x, int index, double centerValue, double[] points)
        {
            // Saves the original parameter value
            double original = x[index];
            double step = this.stepSizes[index];

            // Create the interpolation points
            for (int i = 0; i < points.Length; i++)
            {
                if (i != center)
                {
                    // Make a small perturbation in the original parameter
                    x[index] = original + (i - center) * step;

                    // Recompute the function to measure its importance
                    points[i] = func(x);
                }
                else
                {
                    // The center point is the original function output, so 
                    points[i] = centerValue; // we can save one function call
                }
            }

            // Reverts the modified value
            x[index] = original;

            return step;
        }

        /// <summary>
        ///   Interpolates the points to obtain an estimative of the <paramref name="order"/> derivative.
        /// </summary>
        /// 
        private static double Interpolate(double[][,] coefficients, double[] points, int order, int center, double step)
        {
            double sum = 0.0;

            for (int i = 0; i < points.Length; i++)
            {
                double c = coefficients[center][order, i];

                if (c != 0)
                    sum += c * points[i];
            }

            double r = sum / Math.Pow(step, order);
            return r;
        }



        /// <summary>
        ///   Creates the interpolation coefficient table for interpolated numerical differentation.
        /// </summary>
        /// 
        /// <param name="numberOfPoints">The number of points in the tableau.</param>
        /// 
        public static double[][,] CreateCoefficients(int numberOfPoints)
        {
            if (numberOfPoints % 2 != 1)
                throw new ArgumentException("The number of points must be odd.", "numberOfPoints");

            // Compute difference coefficient table
            double[][,] c = new double[numberOfPoints][,]; // [center][order, i];

            double fac = Special.Factorial(numberOfPoints);

            for (int i = 0; i < numberOfPoints; i++)
            {
                var deltas = new double[numberOfPoints, numberOfPoints];

                for (int j = 0; j < numberOfPoints; j++)
                {
                    double h = 1.0;
                    double delta = j - i;

                    for (int k = 0; k < numberOfPoints; k++)
                    {
                        deltas[j, k] = h / Special.Factorial(k);
                        h *= delta;
                    }
                }

                c[i] = Matrix.Inverse(deltas);

                for (int j = 0; j < numberOfPoints; j++)
                    for (int k = 0; k < numberOfPoints; k++)
                        c[i][j, k] = (Math.Round(c[i][j, k] * fac, MidpointRounding.AwayFromZero)) / fac;
            }

            return c;
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
            return Derivative(function, value, order, stepSize: DEFAULT_STEPSIZE);
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
            return Derivative(function, value, order: DEFAULT_ORDER);
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
            // This method is specific for univariate functions.
            // TODO: Separate FiniteDifferences into classes for univariate, multivariate and vector-valued functions

            double output = function(value);
            double original = value;

            if (Math.Abs(original) > 1e-10)
                stepSize *= System.Math.Abs(original);
            else stepSize = 1e-10;


            // Create the interpolation points
            var outputs = new double[coefficientCache.Length];

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

            return Interpolate(coefficientCache, outputs, order, center, stepSize);
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
        public static Func<double[], double[]> Gradient(Func<double[], double> function, int variables, int order = 1)
        {
            return new FiniteDifferences(variables, function, order: order).Gradient;
        }

        /// <summary>
        ///   Obtains the Hessian function for a multidimensional function.
        /// </summary>
        /// 
        /// <param name="function">The function to be differentiated.</param>
        /// <param name="variables">The number of parameters for the function.</param>
        /// 
        /// <returns>The gradient function of the given <paramref name="function"/>.</returns>
        /// 
        public static Func<double[], double[][]> Hessian(Func<double[], double> function, int variables)
        {
            return new FiniteDifferences(variables, function).Hessian;
        }

    }
}