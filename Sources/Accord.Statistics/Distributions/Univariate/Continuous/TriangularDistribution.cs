// Accord Statistics Library
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

namespace Accord.Statistics.Distributions.Univariate
{
    using System;
    using Accord.Math;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Fitting;
    using System.ComponentModel;
    using Accord.Compat;

    /// <summary>
    ///   Triangular distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In probability theory and statistics, the triangular distribution is a continuous
    ///   probability distribution with lower limit a, upper limit b and mode c, where a &lt; 
    ///   b and a ≤ c ≤ b.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="https://en.wikipedia.org/wiki/Triangular_distribution">
    ///       Wikipedia, The Free Encyclopedia. Triangular distribution. Available on: 
    ///       https://en.wikipedia.org/wiki/Triangular_distribution </a></description></item>
    ///   </list></para> 
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   This example shows how to create a Triangular distribution
    ///   with minimum 1, maximum 6, and most common value 3.</para>
    ///   
    /// <code>
    /// // Create a new Triangular distribution (1, 3, 6).
    /// var trig = new TriangularDistribution(a: 1, b: 6, c: 3);
    /// 
    /// double mean = trig.Mean;     // 3.3333333333333335
    /// double median = trig.Median; // 3.2613872124741694
    /// double mode = trig.Mode;     // 3.0
    /// double var = trig.Variance;  // 1.0555555555555556
    /// 
    /// double cdf = trig.DistributionFunction(x: 2); // 0.10000000000000001
    /// double pdf = trig.ProbabilityDensityFunction(x: 2); // 0.20000000000000001
    /// double lpdf = trig.LogProbabilityDensityFunction(x: 2); // -1.6094379124341003
    /// 
    /// double ccdf = trig.ComplementaryDistributionFunction(x: 2); // 0.90000000000000002
    /// double icdf = trig.InverseDistributionFunction(p: cdf); // 2.0000000655718773
    /// 
    /// double hf = trig.HazardFunction(x: 2); // 0.22222222222222224
    /// double chf = trig.CumulativeHazardFunction(x: 2); // 0.10536051565782628
    /// 
    /// string str = trig.ToString(CultureInfo.InvariantCulture); // Triangular(x; a = 1, b = 6, c = 3)
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class TriangularDistribution : UnivariateContinuousDistribution,
        ISampleableDistribution<double>, IFittableDistribution<double, TriangularOptions>
    {

        // Distribution parameters
        private double a;
        private double b;
        private double c;

        /// <summary>
        ///   Constructs a Triangular distribution
        ///   with the given parameters a, b and c.
        /// </summary>
        /// 
        /// <param name="min">The minimum possible value in the distribution (a).</param>
        /// <param name="max">The maximum possible value in the distribution (b).</param>
        /// <param name="mode">The most common value in the distribution (c).</param>
        /// 
        public TriangularDistribution([Real, DefaultValue(0)] double min, [Real(minimum: 1e-300), DefaultValue(1)] double max, [Real, DefaultValue(0.5)] double mode)
        {
            if (min >= max)
            {
                throw new ArgumentOutOfRangeException("max",
                    "The maximum value 'max' must be greater than the minimum value 'min'.");
            }

            if (mode < min || mode > max)
            {
                throw new ArgumentOutOfRangeException("mode",
                    "The most common value 'mode' must be between 'min' and 'max'.");
            }

            initialize(min, max, mode);
        }


        /// <summary>
        ///   Gets the triangular parameter A (the minimum value).
        /// </summary>
        /// 
        public double Min { get { return a; } }

        /// <summary>
        ///   Gets the triangular parameter B (the maximum value).
        /// </summary>
        /// 
        public double Max { get { return b; } }


        /// <summary>
        ///   Gets the mean for this distribution, 
        ///   defined as (a + b + c) / 3.
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's mean value.
        /// </value>
        /// 
        public override double Mean
        {
            get { return (a + b + c) / 3.0; }
        }


        /// <summary>
        ///   Gets the median for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's median value.
        /// </value>
        /// 
        public override double Median
        {
            get
            {
                double median;
                if (c >= (a + b) / 2.0)
                {
                    median = a + Math.Sqrt((b - a) * (c - a)) / Constants.Sqrt2;
                }
                else
                {
                    median = b - Math.Sqrt((b - a) * (b - c)) / Constants.Sqrt2;
                }

                Accord.Diagnostics.Debug.Assert(median.IsEqual(base.Median, 1e-5));

                return median;
            }
        }


        /// <summary>
        ///   Gets the variance for this distribution, defined
        ///   as (a² + b² + c² - ab - ac - bc) / 18.
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's variance.
        /// </value>
        /// 
        public override double Variance
        {
            get { return (a * a + b * b + c * c - a * b - a * c - b * c) / 18; }
        }

        /// <summary>
        ///   Gets the mode for this distribution,
        ///   also known as the triangular's <c>c</c>.
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's mode value.
        /// </value>
        /// 
        public override double Mode
        {
            get { return c; }
        }

        /// <summary>
        ///   Gets the distribution support, defined as (<see cref="Min"/>, <see cref="Max"/>).
        /// </summary>
        /// 
        public override DoubleRange Support
        {
            get { return new DoubleRange(a, b); }
        }


        /// <summary>
        ///   Gets the entropy for this distribution, 
        ///   defined as 0.5 + log((max-min)/2)).
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's entropy.
        /// </value>
        /// 
        public override double Entropy
        {
            get { return 0.5 + Math.Log((b - a) / 2); }
        }


        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        protected internal override double InnerDistributionFunction(double x)
        {
            if (x >= a && x <= c)
                return ((x - a) * (x - a)) / ((b - a) * (c - a));
            return 1 - ((b - x) * (b - x)) / ((b - a) * (b - c));
        }


        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The probability of <c>x</c> occurring
        ///   in the current distribution.
        /// </returns>
        /// 
        protected internal override double InnerProbabilityDensityFunction(double x)
        {
            if (x >= a && x <= c)
                return (2 * (x - a)) / ((b - a) * (c - a));
            return (2 * (b - x)) / ((b - a) * (b - c));
        }


        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public override object Clone()
        {
            return new TriangularDistribution(a, b, c);
        }

        /// <summary>
        ///   Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public override string ToString(string format, IFormatProvider formatProvider)
        {
            return String.Format(formatProvider, "Triangular(x; a = {0}, b = {1}, c = {2})",
                a.ToString(format, formatProvider),
                b.ToString(format, formatProvider),
                c.ToString(format, formatProvider));
        }


        private void initialize(double min, double max, double mode)
        {
            this.a = min;
            this.b = max;
            this.c = mode;
        }

        /// <summary>
        ///   Generates a random vector of observations from the current distribution.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        ///
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        /// 
        public override double[] Generate(int samples, double[] result, Random source)
        {
            double Fc = DistributionFunction(c);

            UniformContinuousDistribution.Random(samples, result, source);

            for (int i = 0; i < samples; i++)
            {
                double u = result[i];

                if (u < Fc)
                    result[i] = a + Math.Sqrt(u * (b - a) * (c - a));
                else
                    result[i] = b - Math.Sqrt((1 - u) * (b - a) * (b - c));
            }

            return result;
        }

        /// <summary>
        ///   Generates a random observation from the current distribution.
        /// </summary>
        /// 
        /// <returns>A random observations drawn from this distribution.</returns>
        /// 
        public override double Generate(Random source)
        {
            double u = UniformContinuousDistribution.Random(source);
            double Fc = DistributionFunction(c);

            if (u < Fc)
                return a + Math.Sqrt(u * (b - a) * (c - a));
            return b - Math.Sqrt((1 - u) * (b - a) * (b - c));
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">
        ///   The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).</param>
        /// <param name="weights">The weight vector containing the weight for each of the samples.</param>
        /// <param name="options">Optional arguments which may be used during fitting,
        ///   such as regularization constants and additional parameters.</param>
        /// 
        public override void Fit(double[] observations, double[] weights, IFittingOptions options)
        {
            Fit(observations, weights, options as TriangularOptions);
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">
        ///   The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).</param>
        /// <param name="weights">The weight vector containing the weight for each of the samples.</param>
        /// <param name="options">Optional arguments which may be used during fitting,
        ///   such as regularization constants and additional parameters.</param>
        /// 
        public override void Fit(double[] observations, int[] weights, IFittingOptions options)
        {
            Fit(observations, weights, options as TriangularOptions);
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">The array of observations to fit the model against.
        ///   The array elements can be either of type double (for univariate data) or type
        ///   double[] (for multivariate data).
        /// </param>
        /// <param name="weights">The weight vector containing the weight for each of the samples.</param>
        /// <param name="options">Optional arguments which may be used during fitting, 
        ///   such as regularization constants and additional parameters.</param>
        ///   
        public void Fit(double[] observations, double[] weights, TriangularOptions options)
        {
            bool fixMax = true;
            bool fixMin = true;
            bool sorted = false;
            int imax = -1;
            int imin = -1;
            TriangularEstimationMethod method = TriangularEstimationMethod.MeanMaxMin;

            if (options != null)
            {
                fixMax = options.FixMax;
                fixMin = options.FixMin;
                method = options.Method;
                sorted = options.IsSorted;
                imin = options.MinIndex;
                imax = options.MaxIndex;
            }

            double min = this.Min;
            double max = this.Max;
            double mode = this.Mode;


            if (!sorted)
                Array.Sort(observations, weights);

            if (!fixMin)
                min = GetMin(observations, weights, out imin);

            if (!fixMax)
                max = GetMax(observations, weights, out imax);

            if (imin == -1)
                imin = FindMin(observations, min);

            if (imax == -1)
                imax = FindMax(observations, max);


            mode = GetMode(observations, weights, method, min, max, imax, imin);

            initialize(min, max, mode);
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">The array of observations to fit the model against.
        ///   The array elements can be either of type double (for univariate data) or type
        ///   double[] (for multivariate data).
        /// </param>
        /// <param name="weights">The weight vector containing the weight for each of the samples.</param>
        /// <param name="options">Optional arguments which may be used during fitting, 
        ///   such as regularization constants and additional parameters.</param>
        ///   
        public void Fit(double[] observations, int[] weights, TriangularOptions options)
        {
            bool fixMax = true;
            bool fixMin = true;
            bool sorted = false;
            int imax = -1;
            int imin = -1;
            TriangularEstimationMethod method = TriangularEstimationMethod.MeanMaxMin;

            if (options != null)
            {
                fixMax = options.FixMax;
                fixMin = options.FixMin;
                method = options.Method;
                sorted = options.IsSorted;
                imin = options.MinIndex;
                imax = options.MaxIndex;
            }

            double min = this.Min;
            double max = this.Max;
            double mode = this.Mode;


            if (!sorted)
                Array.Sort(observations, weights);

            if (!fixMin)
                min = GetMin(observations, weights, out imin);

            if (!fixMax)
                max = GetMax(observations, weights, out imax);

            if (imin == -1)
                imin = FindMin(observations, min);

            if (imax == -1)
                imax = FindMax(observations, max);


            mode = GetMode(observations, weights, method, min, max, imax, imin);

            initialize(min, max, mode);
        }

        /// <summary>
        ///   Gets the minimum value in a set of weighted observations.
        /// </summary>
        /// 
        public static double GetMin(double[] observations, double[] weights, out int imin)
        {
            imin = 0;
            if (weights == null)
                return observations[imin];

            return observations.WeightedMin(weights, out imin, alreadySorted: true);
        }

        /// <summary>
        ///   Gets the maximum value in a set of weighted observations.
        /// </summary>
        /// 
        public static double GetMax(double[] observations, double[] weights, out int imax)
        {
            imax = observations.Length - 1;
            if (weights == null)
                return observations[imax];

            return observations.WeightedMax(weights, out imax, alreadySorted: true);
        }

        /// <summary>
        ///   Finds the index of the last largest value in a set of observations.
        /// </summary>
        /// 
        public static int FindMax(double[] observations, double max)
        {
            int imax = observations.Length - 1;
            while (observations[imax] > max)
                imax++;
            return imax;
        }

        /// <summary>
        ///   Finds the index of the first smallest value in a set of observations.
        /// </summary>
        /// 
        public static int FindMin(double[] observations, double min)
        {
            int imin = 0;
            while (observations[imin] < min)
                imin++;
            return imin;
        }

        /// <summary>
        ///   Finds the index of the first smallest value in a set of weighted observations.
        /// </summary>
        /// 
        public static double GetMin(double[] observations, int[] weights, out int imin)
        {
            imin = 0;
            if (weights == null)
                return observations[imin];

            return observations.WeightedMin(weights, out imin, alreadySorted: true);
        }

        /// <summary>
        ///   Finds the index of the last largest value in a set of weighted observations.
        /// </summary>
        /// 
        public static double GetMax(double[] observations, int[] weights, out int imax)
        {
            imax = observations.Length - 1;
            if (weights == null)
                return observations[imax];

            return observations.WeightedMax(weights, out imax, alreadySorted: true);
        }




        private static double GetMode(double[] observations, double[] weights, TriangularEstimationMethod method,
            double min, double max, int imax, int imin)
        {
            switch (method)
            {
                case TriangularEstimationMethod.MeanMaxMin:
                    return MeanMaxMin(observations, weights, min, max, imax, imin);
                case TriangularEstimationMethod.Standard:
                    return WeightedMode(observations, weights, imax, imin);
            }

            throw new NotSupportedException("Unsupported estimation method: " + method);
        }


        private static double GetMode(double[] observations, int[] weights, TriangularEstimationMethod method,
            double min, double max, int imax, int imin)
        {
            switch (method)
            {
                case TriangularEstimationMethod.MeanMaxMin:
                    return MeanMaxMin(observations, weights, min, max, imax, imin);
                case TriangularEstimationMethod.Standard:
                    return WeightedMode(observations, weights, imax, imin);
                case TriangularEstimationMethod.Bisection:
                    break;
                default:
                    break;
            }

            throw new NotSupportedException("Unsupported estimation method: " + method);
        }

        private static double WeightedMode(double[] observations, double[] weights, int imax, int imin)
        {
            double currentValue = observations[imin];
            double currentCount = weights[imin];

            double bestValue = currentValue;
            double bestCount = currentCount;


            for (int i = imin + 1; i <= imax; i++)
            {
                if (currentValue == observations[i])
                {
                    currentCount += weights[i];
                }
                else
                {
                    currentValue = observations[i];
                    currentCount = weights[i];
                }

                if (currentCount > bestCount)
                {
                    bestCount = currentCount;
                    bestValue = currentValue;
                }
            }

            return bestValue;
        }

        private static double WeightedMode(double[] observations, int[] weights, int imax, int imin)
        {
            double currentValue = observations[imin];
            double currentCount = weights[imin];

            double bestValue = currentValue;
            double bestCount = currentCount;


            for (int i = imin + 1; i <= imax; i++)
            {
                if (currentValue.Equals(observations[i]))
                {
                    currentCount += weights[i];
                }
                else
                {
                    currentValue = observations[i];
                    currentCount = weights[i];
                }

                if (currentCount > bestCount)
                {
                    bestCount = currentCount;
                    bestValue = currentValue;
                }
            }

            return bestValue;
        }

        private static double MeanMaxMin(double[] observations, double[] weights, double min, double max, int imax, int imin)
        {
            double mean;

            if (weights == null)
            {
                double sum = 0;
                for (int i = imin; i <= imax; i++)
                    sum += observations[i];
                mean = sum / (imax - imin);
            }
            else
            {
                double sum = 0;
                double weightSum = 0;
                for (int i = imin; i <= imax; i++)
                {
                    sum += weights[i] * observations[i];
                    weightSum += weights[i];
                }

                mean = sum / weightSum;
            }

            return 3 * mean - max - min;
        }

        private static double MeanMaxMin(double[] observations, int[] weights, double min, double max, int imax, int imin)
        {
            double mean;

            if (weights == null)
            {
                double sum = 0;
                for (int i = imin; i <= imax; i++)
                    sum += observations[i];
                mean = sum / (imax - imin);
            }
            else
            {
                double sum = 0;
                double weightSum = 0;
                for (int i = imin; i <= imax; i++)
                {
                    sum += weights[i] * observations[i];
                    weightSum += weights[i];
                }

                mean = sum / weightSum;
            }

            return 3 * mean - max - min;
        }
    }
}
