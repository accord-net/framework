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

namespace Accord.Math.Integration
{
    using System;
    using AForge;

    /// <summary>
    ///   Trapezoidal rule for numerical integration.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In numerical analysis, the trapezoidal rule (also known as the trapezoid rule 
    ///   or trapezium rule) is a technique for approximating the definite integral 
    ///   <c>∫_a^b(x) dx</c>. The trapezoidal rule works by approximating the region 
    ///   under the graph of the function f(x) as a trapezoid and calculating its area.
    ///   It follows that <c>∫_a^b(x) dx ~ (b - a) [f(a) - f(b)] / 2</c>.
    /// </para>
    /// 
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Trapezoidal_rule">
    ///       Wikipedia, The Free Encyclopedia. Trapezoidal rule. Available on: 
    ///       http://en.wikipedia.org/wiki/Trapezoidal_rule </a></description></item>
    ///   </list>
    ///  </para>
    ///  </remarks>
    ///  
    /// <example>
    /// <para>
    ///   Let's say we would like to compute the definite integral of the function 
    ///   <c>f(x) = cos(x)</c> in the interval -1 to +1 using a variety of integration 
    ///   methods, including the <see cref="TrapezoidalRule"/>, <see cref="RombergMethod"/>
    ///   and <see cref="NonAdaptiveGaussKronrod"/>. Those methods can compute definite
    ///   integrals where the integration interval is finite:
    /// </para>
    /// 
    /// <code>
    /// // Declare the function we want to integrate
    /// Func&lt;double, double> f = (x) => Math.Cos(x);
    ///
    /// // We would like to know its integral from -1 to +1
    /// double a = -1, b = +1;
    ///
    /// // Integrate!
    /// double trapez  = TrapezoidalRule.Integrate(f, a, b, steps: 1000); // 1.6829414
    /// double romberg = RombergMethod.Integrate(f, a, b);                // 1.6829419
    /// double nagk    = NonAdaptiveGaussKronrod.Integrate(f, a, b);      // 1.6829419
    /// </code>
    /// 
    /// <para>
    ///   Moreover, it is also possible to calculate the value of improper integrals
    ///   (it is, integrals with infinite bounds) using <see cref="InfiniteAdaptiveGaussKronrod"/>,
    ///   as shown below. Let's say we would like to compute the area under the Gaussian
    ///   curve from -infinite to +infinite. While this function has infinite bounds, this
    ///   function is known to integrate to 1.</para>
    ///   
    /// <code>
    /// // Declare the Normal distribution's density function (which is the Gaussian's bell curve)
    /// Func&lt;double, double> g = (x) => (1 / Math.Sqrt(2 * Math.PI)) * Math.Exp(-(x * x) / 2);
    ///
    /// // Integrate!
    /// double iagk = InfiniteAdaptiveGaussKronrod.Integrate(g,
    ///     Double.NegativeInfinity, Double.PositiveInfinity);   // Result should be 0.99999...
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="RombergMethod"/>
    /// <seealso cref="NonAdaptiveGaussKronrod"/>
    /// <seealso cref="InfiniteAdaptiveGaussKronrod"/>
    /// <seealso cref="MonteCarloIntegration"/>
    /// 
    public class TrapezoidalRule : INumericalIntegration, IUnivariateIntegration
    {
        private DoubleRange range;

        /// <summary>
        ///   Gets or sets the unidimensional function
        ///   whose integral should be computed.
        /// </summary>
        /// 
        public Func<double, double> Function { get; set; }

        /// <summary>
        ///   Gets the numerically computed result of the
        ///   definite integral for the specified function.
        /// </summary>
        /// 
        public double Area { get; private set; }

        /// <summary>
        ///   Gets or sets the number of steps into which the
        ///   <see cref="Range">integration interval</see> will 
        ///   be divided. Default is 6.
        /// </summary>
        /// 
        public int Steps { get; set; }

        /// <summary>
        ///   Gets or sets the input range under
        ///   which the integral must be computed.
        /// </summary>
        /// 
        public DoubleRange Range
        {
            get { return range; }
            set
            {
                if (Double.IsInfinity(range.Min) || Double.IsNaN(range.Min))
                    throw new ArgumentOutOfRangeException("value", "Minimum is out of range.");

                if (Double.IsInfinity(range.Max) || Double.IsNaN(range.Max))
                    throw new ArgumentOutOfRangeException("value", "Maximum is out of range.");

                range = value;
            }
        }

        /// <summary>
        ///   Constructs a new <see cref="TrapezoidalRule"/> integration method.
        /// </summary>
        /// 
        public TrapezoidalRule()
            : this(6)
        {
        }

        /// <summary>
        ///   Constructs a new <see cref="TrapezoidalRule"/> integration method.
        /// </summary>
        /// 
        /// <param name="function">The unidimensional function whose integral should be computed.</param>
        /// 
        public TrapezoidalRule(Func<double, double> function)
            : this(6, function)
        {
        }

        /// <summary>
        ///   Constructs a new <see cref="TrapezoidalRule"/> integration method.
        /// </summary>
        /// 
        /// <param name="function">The unidimensional function whose integral should be computed.</param>
        /// <param name="a">The beginning of the integration interval.</param>
        /// <param name="b">The ending of the integration interval.</param>
        /// 
        public TrapezoidalRule(Func<double, double> function, double a, double b)
            : this(6, function, a, b)
        {
        }

        /// <summary>
        ///   Constructs a new <see cref="TrapezoidalRule"/> integration method.
        /// </summary>
        /// 
        /// <param name="steps">The number of steps into which the integration 
        ///   interval will be divided.</param>
        /// 
        public TrapezoidalRule(int steps)
        {
            Steps = steps;
            Range = new DoubleRange(0, 1);
        }

        /// <summary>
        ///   Constructs a new <see cref="TrapezoidalRule"/> integration method.
        /// </summary>
        /// 
        /// <param name="steps">The number of steps into which the integration 
        ///   interval will be divided.</param>
        /// <param name="function">The unidimensional function 
        ///   whose integral should be computed.</param>
        /// 
        public TrapezoidalRule(int steps, Func<double, double> function)
        {
            if (function == null)
                throw new ArgumentNullException("function");

            Range = new DoubleRange(0, 1);
            Function = function;
            Steps = steps;
        }

        /// <summary>
        ///   Constructs a new <see cref="TrapezoidalRule"/> integration method.
        /// </summary>
        /// 
        /// <param name="steps">The number of steps into which the integration 
        ///   interval will be divided.</param>
        /// <param name="function">The unidimensional function 
        ///   whose integral should be computed.</param>
        /// <param name="a">The beginning of the integration interval.</param>
        /// <param name="b">The ending of the integration interval.</param>
        /// 
        public TrapezoidalRule(int steps, Func<double, double> function, double a, double b)
        {
            if (Double.IsInfinity(a) || Double.IsNaN(a))
                throw new ArgumentOutOfRangeException("a");

            if (Double.IsInfinity(b) || Double.IsNaN(b))
                throw new ArgumentOutOfRangeException("b");

            Function = function;
            Range = new DoubleRange(a, b);
            Steps = steps;
        }

        /// <summary>
        ///   Computes the area of the function under the selected <see cref="Range"/>.
        ///   The computed value will be available at this object's <see cref="Area"/>.
        /// </summary>
        /// 
        /// <returns>
        ///   True if the integration method succeeds, false otherwise.
        /// </returns>
        /// 
        public bool Compute()
        {
            Area = Integrate(Function, range.Min, range.Max, Steps);

            return true;
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
            TrapezoidalRule clone = new TrapezoidalRule(
                this.Steps, this.Function,
                this.Range.Min, this.Range.Max);

            return clone;
        }


        /// <summary>
        ///   Computes the area under the integral for the given function, 
        ///   in the given integration interval, using the Trapezoidal rule.
        /// </summary>
        /// 
        /// <param name="steps">The number of steps into which the integration interval will be divided.</param>
        /// <param name="func">The unidimensional function whose integral should be computed.</param>
        /// <param name="a">The beginning of the integration interval.</param>
        /// <param name="b">The ending of the integration interval.</param>
        /// 
        /// <returns>The integral's value in the current interval.</returns>
        /// 
        public static double Integrate(Func<double, double> func, double a, double b, int steps)
        {
            double h = (b - a) / steps;

            double sum = 0.5 * (func(a) + func(b));

            for (int i = 1; i < steps; i++)
                sum += func(a + i * h);

            return h * sum;
        }
    }
}
