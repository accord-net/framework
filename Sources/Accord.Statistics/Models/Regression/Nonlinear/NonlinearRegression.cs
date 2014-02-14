// Accord Statistics Library
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

namespace Accord.Statistics.Models.Regression
{
    using System;

    /// <summary>
    ///   Regression function delegate.
    /// </summary>
    /// 
    /// <remarks>
    ///   This delegate represents a parameterized function that, given a set of
    ///   <paramref name="coefficients">model coefficients</paramref> and an input
    ///   vector, produces an associated output value.
    /// </remarks>
    /// 
    /// <param name="coefficients">The model coefficients, also known as parameters or coefficients.</param>
    /// <param name="input">An input vector.</param>
    /// 
    /// <returns>The output value produced given the
    /// <paramref name="coefficients"/> and <paramref name="input"/> vector.</returns>
    /// 
    public delegate double RegressionFunction(double[] coefficients, double[] input);

    /// <summary>
    ///   Gradient function delegate.
    /// </summary>
    /// 
    /// <remarks>
    ///   This delegate represents the gradient of <see cref="RegressionFunction">regression 
    ///   function</see>. A regression function is a parameterized function that, given a set
    ///   of <paramref name="coefficients">model coefficients</paramref> and an input vector,
    ///   produces an associated output value. This function should compute the gradient vector
    ///   in respect to the function <paramref name="coefficients"/>.
    /// </remarks>
    /// 
    /// <param name="coefficients">The model coefficients, also known as parameters or coefficients.</param>
    /// <param name="input">An input vector.</param>
    /// <param name="result">The resulting gradient vector (w.r.t to the coefficients).</param>
    /// 
    public delegate void RegressionGradientFunction(double[] coefficients, double[] input, double[] result);

    /// <summary>
    ///   Nonlinear Regression.
    /// </summary>
    /// 
    [Serializable]
    public class NonlinearRegression : ICloneable
    {
        double[] coefficients;
        double[] standardErrors;

        // TODO: implement a way to serialize the function and gradient
        // functions, most likely using serializable expression trees.

        [NonSerialized]
        RegressionFunction function;

        [NonSerialized]
        RegressionGradientFunction gradient;


        /// <summary>
        ///   Gets the regression coefficients.
        /// </summary>
        /// 
        public double[] Coefficients
        {
            get { return coefficients; }
        }

        /// <summary>
        ///   Gets the standard errors for the regression coefficients.
        /// </summary>
        /// 
        public double[] StandardErrors
        {
            get { return standardErrors; }
        }



        /// <summary>
        ///   Gets the model function, mapping inputs to 
        ///   outputs given a suitable parameter vector.
        /// </summary>
        /// 
        public RegressionFunction Function
        {
            get { return function; }
        }

        /// <summary>
        ///   Gets or sets a function that computes the gradient of the
        ///   <see cref="Function"/> in respect to the <see cref="Coefficients"/>.
        /// </summary>
        /// 
        public RegressionGradientFunction Gradient
        {
            get { return gradient; }
            set { gradient = value; }
        }




        /// <summary>
        ///   Initializes a new instance of the <see cref="NonlinearRegression"/> class.
        /// </summary>
        /// 
        /// <param name="parameters">The number of variables (free parameters) in the model.</param>
        /// <param name="function">The regression function implementing the regression model.</param>
        /// 
        public NonlinearRegression(int parameters, RegressionFunction function)
        {
            this.coefficients = new double[parameters];
            this.standardErrors = new double[parameters];
            this.function = function;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="NonlinearRegression"/> class.
        /// </summary>
        /// 
        /// <param name="parameters">The number of variables (free parameters) in the model.</param>
        /// <param name="function">The regression function implementing the regression model.</param>
        /// <param name="gradient">The function that computes the gradient for <paramref name="function"/>.</param>
        /// 
        public NonlinearRegression(int parameters, RegressionFunction function, RegressionGradientFunction gradient)
            : this(parameters, function)
        {
            this.gradient = gradient;
        }



        /// <summary>
        ///   Computes the model output for the given input vector.
        /// </summary>
        /// 
        /// <param name="inputs">The input vector.</param>
        /// 
        /// <returns>The output value.</returns>
        /// 
        public double Compute(double[] inputs)
        {
            return function(coefficients, inputs);
        }



        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public object Clone()
        {
            var clone = new NonlinearRegression(Coefficients.Length, function, gradient);
            clone.coefficients = (double[])this.coefficients.Clone();
            clone.standardErrors = (double[])this.standardErrors.Clone();
            return clone;
        }
    }
}
