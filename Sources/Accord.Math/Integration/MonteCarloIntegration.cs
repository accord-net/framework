// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
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
    ///   Monte Carlo method for multi-dimensional integration.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In mathematics, Monte Carlo integration is a technique for numerical 
    ///   integration using random numbers. It is a particular Monte Carlo method
    ///   that numerically computes a definite integral. While other algorithms 
    ///   usually evaluate the integrand at a regular grid, Monte Carlo randomly
    ///   choose points at which the integrand is evaluated. This method is 
    ///   particularly useful for higher-dimensional integrals. There are different
    ///   methods to perform a Monte Carlo integration, such as uniform sampling,
    ///   stratified sampling and importance sampling.
    /// </para>
    /// 
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Monte_Carlo_integration">
    ///       Wikipedia, The Free Encyclopedia. Monte Carlo integration. Available on: 
    ///       http://en.wikipedia.org/wiki/Monte_Carlo_integration </a></description></item>
    ///   </list>
    ///  </para>
    ///  </remarks>
    ///  
    /// <seealso cref="NonAdaptiveGaussKronrod"/>
    /// <seealso cref="InfiniteAdaptiveGaussKronrod"/>
    /// 
    public class MonteCarloIntegration : INumericalIntegration, IMultidimensionalIntegration
    {
        private ISingleValueConvergence convergence;


        /// <summary>
        ///   Gets the number of parameters expected by
        ///   the <see cref="Function"/> to be integrated.
        /// </summary>
        /// 
        public int NumberOfParameters { get; private set; }

        /// <summary>
        ///   Gets or sets the range of each input variable
        ///   under which the integral must be computed.
        /// </summary>
        /// 
        public DoubleRange[] Range { get; private set; }

        /// <summary>
        ///   Gets or sets the multidimensional function
        ///   whose integral should be computed.
        /// </summary>
        /// 
        public Func<double[], double> Function { get; set; }

        /// <summary>
        ///   Gets or sets the random generator algorithm to be used within
        ///   this <see cref="MonteCarloIntegration">Monte Carlo method</see>.
        /// </summary>
        /// 
        public Random Random { get; set; }

        /// <summary>
        ///   Gets the numerically computed result of the
        ///   definite integral for the specified function.
        /// </summary>
        /// 
        public double Area { get; set; }

        /// <summary>
        ///   Gets the integration error for the
        ///   computed <see cref="Area"/> value.
        /// </summary>
        /// 
        public double Error { get; set; }

        /// <summary>
        ///   Gets or sets the maximum relative change in the watched value
        ///   after an iteration of the algorithm used to detect convergence.
        /// </summary>
        /// 
        public double Tolerance
        {
            get { return convergence.Tolerance; }
            set { convergence.Tolerance = value; }
        }

        /// <summary>
        ///   Gets or sets the maximum number of iterations
        ///   performed by the iterative algorithm.
        /// </summary>
        /// 
        public int Iterations
        {
            get { return convergence.Iterations; }
            set { convergence.Iterations = value; }
        }

        /// <summary>
        ///   Constructs a new <see cref="MonteCarloIntegration">Monte Carlo integration method</see>.
        /// </summary>
        /// 
        /// <param name="function">The function to be integrated.</param>
        /// <param name="parameters">The number of parameters expected by the <paramref name="function"/>.</param>
        /// 
        public MonteCarloIntegration(int parameters, Func<double[], double> function)
            : this(parameters)
        {
            if (function == null)
                throw new ArgumentNullException("function");

            this.Function = function;
        }

        /// <summary>
        ///   Constructs a new <see cref="MonteCarloIntegration">Monte Carlo integration method</see>.
        /// </summary>
        /// 
        /// <param name="parameters">The number of parameters expected by the integrand.</param>
        /// 
        public MonteCarloIntegration(int parameters)
        {
            if (parameters <= 0)
            {
                throw new ArgumentOutOfRangeException("parameters",
                    "Number of parameters must be higher than zero.");
            }

            this.NumberOfParameters = parameters;
            this.Range = new DoubleRange[parameters];

            for (int i = 0; i < Range.Length; i++)
                Range[i].Max = 1;

            this.convergence = new RelativeConvergence();
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
            double volume = 1;
            for (int i = 0; i < Range.Length; i++)
                volume *= Range[i].Length;

            int count = 0;
            double sum = 0;
            double sum2 = 0;

            double[] sample = new double[NumberOfParameters];

            do
            {
                for (int i = 0; i < sample.Length; i++)
                    sample[i] = Random.NextDouble() * Range[i].Length + Range[i].Min;

                double f = Function(sample);

                count++;
                sum += f;
                sum2 += f * f;

                convergence.NewValue = sum;
            }
            while (!convergence.HasConverged);

            double avg = sum / count;
            double avg2 = sum2 / count;

            Area = volume * avg;
            Error = volume * Math.Sqrt((avg2 - avg * avg) / count);

            return true;
        }


        /// <summary>
        ///   Computes the area under the integral for the given function, in the 
        ///   given integration interval, using a Monte Carlo integration algorithm.
        /// </summary>
        /// 
        /// <param name="func">The unidimensional function whose integral should be computed.</param>
        /// <param name="a">The beginning of the integration interval.</param>
        /// <param name="b">The ending of the integration interval.</param>
        /// <param name="tol">The relative accuracy under which the solution has to be found.</param>
        /// <param name="samples">The number of points that should be sampled.</param>
        /// 
        /// <returns>The integral's value in the current interval.</returns>
        /// 
        public static double Integrate(Func<double, double> func, double a, double b,
            int samples, double tol)
        {
            double volume = (b - a);

            int count = 0;
            double sum = 0;
            double old = 0;

            var random = Accord.Math.Tools.Random;

            for (count = 0; count < samples; count++)
            {
                double u = random.Next() * (b - a) + a;

                double f = func(u);

                count++;
                sum += f;

                if ((f - old) < f * tol)
                    break;
            }

            double avg = sum / count;

            return volume * avg;
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
            var clone = new MonteCarloIntegration(this.NumberOfParameters, this.Function);

            clone.Iterations = Iterations;
            clone.Tolerance = Tolerance;

            clone.Range = (DoubleRange[])Range.Clone();

            return clone;
        }

    }
}
