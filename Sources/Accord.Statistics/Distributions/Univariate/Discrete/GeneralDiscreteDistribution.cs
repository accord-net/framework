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
    using System.Globalization;
    using Accord.Math;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Math.Random;
    using System.Diagnostics;
    using Accord.Compat;

    /// <summary>
    ///   Univariate general discrete distribution, also referred as the
    ///   Categorical distribution.
    /// </summary>
    /// 
    /// <remarks>
    ///  <para>
    ///   An univariate categorical distribution is a statistical distribution
    ///   whose variables can take on only discrete values. Each discrete value
    ///   defined within the interval of the distribution has an associated 
    ///   probability value indicating its frequency of occurrence.</para>
    ///  <para>
    ///   The discrete uniform distribution is a special case of a generic
    ///   discrete distribution whose probability values are constant.</para>
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    ///   // Create a Categorical distribution for 3 symbols, in which
    ///   // the first and second symbol have 25% chance of appearing,
    ///   // and the third symbol has 50% chance of appearing.
    ///   
    ///   //                         1st   2nd   3rd
    ///   double[] probabilities = { 0.25, 0.25, 0.50 };
    ///   
    ///   // Create the categorical with the given probabilities
    ///   var dist = new GeneralDiscreteDistribution(probabilities);
    ///   
    ///   // Common measures
    ///   double mean = dist.Mean;     // 1.25
    ///   double median = dist.Median; // 1.00
    ///   double var = dist.Variance;  // 0.6875
    ///   
    ///   // Cumulative distribution functions
    ///   double cdf  = dist.DistributionFunction(k: 2);              // 1.0
    ///   double ccdf = dist.ComplementaryDistributionFunction(k: 2); // 0.0
    ///   
    ///   // Probability mass functions
    ///   double pdf1 = dist.ProbabilityMassFunction(k: 0); // 0.25
    ///   double pdf2 = dist.ProbabilityMassFunction(k: 1); // 0.25
    ///   double pdf3 = dist.ProbabilityMassFunction(k: 2); // 0.50
    ///   double lpdf = dist.LogProbabilityMassFunction(k: 2); // -0.69314718055994529
    ///   
    ///   // Quantile function
    ///   int icdf1 = dist.InverseDistributionFunction(p: 0.17); // 0
    ///   int icdf2 = dist.InverseDistributionFunction(p: 0.39); // 1
    ///   int icdf3 = dist.InverseDistributionFunction(p: 0.56); // 2
    ///   
    ///   // Hazard (failure rate) functions
    ///   double hf = dist.HazardFunction(x: 0); // 0.33333333333333331
    ///   double chf = dist.CumulativeHazardFunction(x: 0); // 0.2876820724517809
    ///   
    ///   // String representation
    ///   string str = dist.ToString(CultureInfo.InvariantCulture); // "Categorical(x; p = { 0.25, 0.25, 0.5 })"
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public sealed class GeneralDiscreteDistribution : UnivariateDiscreteDistribution,
        IUnivariateFittableDistribution,
        IFittableDistribution<double, GeneralDiscreteOptions>,
        IFittableDistribution<double[], GeneralDiscreteOptions>,
        IFittableDistribution<int, GeneralDiscreteOptions>,
        IFittable<double[], GeneralDiscreteOptions>,
        IFittable<double, GeneralDiscreteOptions>,
        IFittable<int, GeneralDiscreteOptions>,
        ISampleableDistribution<int>, ISampleableDistribution<double>
    {

        // distribution parameters
        private int start;
        private double[] probabilities;

        // distribution measures
        private double? mean;
        private double? variance;
        private double? entropy;
        private int? mode;

        private bool log = false;


        /// <summary>
        ///   Constructs a new generic discrete distribution.
        /// </summary>
        /// 
        /// <param name="probabilities">
        ///   The frequency of occurrence for each integer value in the
        ///   distribution. The distribution is assumed to begin in the
        ///   interval defined by start up to size of this vector.</param>
        /// <param name="logarithm">
        ///   True if the distribution should be represented using logarithms; false otherwise.
        /// </param>
        ///   
        public GeneralDiscreteDistribution(bool logarithm, params double[] probabilities)
        {
            if (probabilities == null)
                throw new ArgumentNullException("probabilities");

            if (probabilities.Length < 2)
                Trace.TraceWarning("Creating a discrete distribution that is actually constant.");

            initialize(0, probabilities, logarithm);
        }

        /// <summary>
        ///   Constructs a new generic discrete distribution.
        /// </summary>
        /// 
        /// <param name="start">
        ///   The integer value where the distribution starts, also
        ///   known as the offset value. Default value is 0.</param>
        /// <param name="probabilities">
        ///   The frequency of occurrence for each integer value in the
        ///   distribution. The distribution is assumed to begin in the
        ///   interval defined by start up to size of this vector.</param>
        ///   
        public GeneralDiscreteDistribution(int start, params double[] probabilities)
        {
            if (probabilities == null)
                throw new ArgumentNullException("probabilities");

            if (probabilities.Length < 2)
                Trace.TraceWarning("Creating a discrete distribution that is actually constant.");

            initialize(start, probabilities, false);
        }

        /// <summary>
        ///   Constructs a new uniform discrete distribution.
        /// </summary>
        /// 
        /// <param name="start">
        ///   The integer value where the distribution starts, also
        ///   known as the offset value. Default value is 0.</param>
        /// <param name="symbols">
        ///   The number of discrete values within the distribution.
        ///   The distribution is assumed to belong to the interval
        ///   [start, start + symbols].</param>
        /// <param name="logarithm">
        ///   True if the distribution should be represented using logarithms; false otherwise.
        /// </param>
        ///   
        public GeneralDiscreteDistribution(int start, int symbols, bool logarithm = false)
        {
            if (symbols < 2)
                Trace.TraceWarning("Creating a discrete distribution that is actually constant.");

            initialize(start, symbols, logarithm);
        }

        /// <summary>
        ///   Constructs a new generic discrete distribution.
        /// </summary>
        /// 
        /// <param name="probabilities">
        ///   The frequency of occurrence for each integer value in the
        ///   distribution. The distribution is assumed to begin in the
        ///   interval defined by start up to size of this vector.</param>
        ///   
        public GeneralDiscreteDistribution(params double[] probabilities)
            : this(0, probabilities)
        {
        }

        /// <summary>
        ///   Constructs a new uniform discrete distribution.
        /// </summary>
        /// 
        /// <param name="symbols">
        ///   The number of discrete values within the distribution.
        ///   The distribution is assumed to belong to the interval
        ///   [start, start + symbols].</param>
        /// <param name="logarithm">
        ///   True if the distribution should be represented using logarithms; false otherwise.
        /// </param>
        ///   
        public GeneralDiscreteDistribution(int symbols, bool logarithm = false)
            : this(0, symbols, logarithm)
        {
            this.LogProbabilityMassFunction(symbols);
        }

        /// <summary>
        ///   Constructs a new uniform discrete distribution.
        /// </summary>
        /// 
        /// <param name="a">
        ///   The integer value where the distribution starts, also
        ///   known as <c>a</c>. Default value is 0.</param>
        /// <param name="b">
        ///   The integer value where the distribution ends, also 
        ///   known as <c>b</c>.</param>
        ///   
        public static GeneralDiscreteDistribution Uniform(int a, int b)
        {
            if (a > b)
            {
                throw new ArgumentOutOfRangeException("b",
                    "The starting number a must be lower than b.");
            }

            return new GeneralDiscreteDistribution(a, b - a + 1);
        }

        /// <summary>
        ///   Gets the probability value associated with the symbol <paramref name="i"/>.
        /// </summary>
        /// 
        /// <param name="i">The symbol's index.</param>
        /// 
        /// <returns>The probability of the given symbol.</returns>
        /// 
        public double this[int i]
        {
            get { return probabilities[i]; }
            set { probabilities[i] = value; }
        }

        /// <summary>
        ///   Gets the integer value where the
        ///   discrete distribution starts.
        /// </summary>
        /// 
        public int Minimum
        {
            get { return start; }
        }

        /// <summary>
        ///   Gets the integer value where the
        ///   discrete distribution ends.
        /// </summary>
        /// 
        public int Maximum
        {
            get { return start + probabilities.Length; }
        }

        /// <summary>
        ///   Gets the number of symbols in the distribution.
        /// </summary>
        /// 
        public int Length
        {
            get { return probabilities.Length; }
        }

        /// <summary>
        ///   Gets the probabilities associated with each discrete variable value.
        ///   Note: if the frequencies in this property are manually changed, the
        ///   rest of the class properties (Mode, Mean, ...) will not be automatically
        ///   updated to reflect the actual inserted values.
        /// </summary>
        /// 
        public double[] Frequencies
        {
            get { return probabilities; }
            set
            {
                if (!probabilities.DimensionEquals(value))
                    throw new DimensionMismatchException("value");
                probabilities = value;
            }
        }

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        public override double Mean
        {
            get
            {
                if (!mean.HasValue)
                {
                    if (log)
                    {
                        mean = start;
                        for (int i = 0; i < probabilities.Length; i++)
                            mean += i * Math.Exp(probabilities[i]);
                    }
                    else
                    {
                        mean = start;
                        for (int i = 0; i < probabilities.Length; i++)
                            mean += i * probabilities[i];
                    }
                }
                return mean.Value;
            }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        public override double Variance
        {
            get
            {
                if (!variance.HasValue)
                {
                    double m = Mean;
                    double v = 0;

                    if (log)
                    {
                        for (int i = 0; i < probabilities.Length; i++)
                        {
                            double d = i + start - m;
                            v += Math.Exp(probabilities[i]) * (d * d);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < probabilities.Length; i++)
                        {
                            double d = i + start - m;
                            v += probabilities[i] * (d * d);
                        }
                    }
                    this.variance = v;
                }
                return variance.Value;
            }
        }

        /// <summary>
        ///   Gets the mode for this distribution.
        /// </summary>
        /// 
        public override double Mode
        {
            get
            {
                if (!mode.HasValue)
                    mode = probabilities.ArgMax();
                return mode.Value;
            }
        }


        /// <summary>
        ///   Gets the entropy for this distribution.
        /// </summary>
        /// 
        public override double Entropy
        {
            get
            {
                if (!entropy.HasValue)
                {
                    entropy = 0.0;
                    if (log)
                    {
                        for (int i = 0; i < probabilities.Length; i++)
                            entropy -= Math.Exp(probabilities[i]) * System.Math.Log(probabilities[i]);
                    }
                    else
                    {
                        for (int i = 0; i < probabilities.Length; i++)
                            entropy -= probabilities[i] * System.Math.Log(probabilities[i]);
                    }
                }
                return entropy.Value;
            }
        }

        /// <summary>
        ///   Gets the support interval for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   A <see cref="IntRange" /> containing
        ///   the support interval for this distribution.
        /// </value>
        /// 
        public override IntRange Support
        {
            get { return new IntRange(start, start + probabilities.Length - 1); }
        }

        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="k">
        ///   A single point in the distribution range. For a 
        ///   univariate distribution, this should be a single
        ///   double value. For a multivariate distribution,
        ///   this should be a double array.</param>
        ///   
        /// <remarks>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>k</c> will occur.
        /// </remarks>
        /// 
        /// <returns>
        ///   The probability of <c>k</c> occurring
        ///   in the current distribution.</returns>
        ///   
        protected internal override double InnerDistributionFunction(int k)
        {
            int value = k - start;

            if (log)
            {
                double sum = Double.NegativeInfinity;
                for (int i = 0; i <= value; i++)
                    sum = Special.LogSum(probabilities[i], sum);
                return sum;
            }
            else
            {
                double sum = 0.0;
                for (int i = 0; i <= value; i++)
                    sum += probabilities[i];
                return sum;
            }
        }


        /// <summary>
        ///   Gets the probability mass function (pmf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="k">
        ///   A single point in the distribution range.</param>
        ///   
        /// <remarks>
        ///   The Probability Mass Function (PMF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        /// <returns>
        ///   The probability of <c>x</c> occurring
        ///   in the current distribution.</returns>
        ///   
        protected internal override double InnerProbabilityMassFunction(int k)
        {
            int value = k - start;

            if (log)
                return Math.Exp(probabilities[value]);
            return probabilities[value];
        }

        /// <summary>
        ///   Gets the log-probability mass function (pmf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="k">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The logarithm of the probability of <c>k</c>
        ///   occurring in the current distribution.
        /// </returns>
        /// 
        /// <remarks>
        ///   The Probability Mass Function (PMF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        protected internal override double InnerLogProbabilityMassFunction(int k)
        {
            int value = k - start;

            if (log)
                return probabilities[value];
            return Math.Log(probabilities[value]);
        }

        /// <summary>
        ///   Generates a random observation from the current distribution.
        /// </summary>
        /// 
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        /// 
        /// <returns>A random observations drawn from this distribution.</returns>
        /// 
        public override int Generate(Random source)
        {
            return Random(probabilities, source, log: log);
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
        public override int[] Generate(int samples, int[] result, Random source)
        {
            return Random(probabilities, samples, result, source, log: log);
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).</param>
        /// <param name="weights">The weight vector containing the weight for each of the samples.</param>
        /// <param name="options">Optional arguments which may be used during fitting, such
        ///   as regularization constants and additional parameters.</param>
        ///   
        public override void Fit(double[] observations, double[] weights, IFittingOptions options)
        {
            GeneralDiscreteOptions discreteOptions = options as GeneralDiscreteOptions;
            if (options != null && discreteOptions == null)
                throw new ArgumentException("The specified options' type is invalid.", "options");

            Fit(observations, weights, discreteOptions);
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).</param>
        /// <param name="weights">The weight vector containing the weight for each of the samples.</param>
        /// <param name="options">Optional arguments which may be used during fitting, such
        ///   as regularization constants and additional parameters.</param>
        ///   
        public override void Fit(int[] observations, double[] weights, IFittingOptions options)
        {
            GeneralDiscreteOptions discreteOptions = options as GeneralDiscreteOptions;
            if (options != null && discreteOptions == null)
                throw new ArgumentException("The specified options' type is invalid.", "options");

            Fit(observations, weights, discreteOptions);
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).</param>
        /// <param name="weights">The weight vector containing the weight for each of the samples.</param>
        /// <param name="options">Optional arguments which may be used during fitting, such
        ///   as regularization constants and additional parameters.</param>
        ///   
        public void Fit(double[] observations, double[] weights, GeneralDiscreteOptions options)
        {
            Fit(observations.ToInt32(), weights, options);
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).</param>
        /// <param name="weights">The weight vector containing the weight for each of the samples.</param>
        ///   
        public void Fit(int[] observations, double[] weights)
        {
            Fit(observations, weights, null);
        }



        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public override object Clone()
        {
            var c = new GeneralDiscreteDistribution();
            c.probabilities = (double[])probabilities.Clone();
            c.start = start;
            c.mean = mean;
            c.entropy = entropy;
            c.variance = variance;
            c.log = log;
            return c;
        }


        private void initialize(int s, double[] prob, bool logarithm)
        {
            if (logarithm)
            {
                // assert that probabilities sum up to 1.
                double sum = prob.LogSumExp();
                if (sum != Double.NegativeInfinity && sum != 0)
                    prob.Subtract(sum, result: prob);
            }
            else
            {
                double sum = prob.Sum();
                if (sum != 0 && sum != 1)
                    prob.Divide(sum, result: prob);
            }

            this.start = s;
            this.probabilities = prob;

            this.log = logarithm;
            this.mean = null;
            this.variance = null;
            this.entropy = null;
        }

        private void initialize(int s, int symbols, bool logarithm)
        {
            this.start = s;
            this.probabilities = new double[symbols];

            // Initialize with uniform distribution
            for (int i = 0; i < symbols; i++)
                probabilities[i] = 1.0 / symbols;

            if (logarithm)
                probabilities.Log(result: probabilities);

            this.log = logarithm;
            this.mean = null;
            this.variance = null;
            this.entropy = null;
        }


        #region ISamplableDistribution<double> Members

        double[] IRandomNumberGenerator<double>.Generate(int samples)
        {
            return Generate(samples).ToDouble();
        }

        double IRandomNumberGenerator<double>.Generate()
        {
            return Generate();
        }

        #endregion

        /// <summary>
        ///   Returns a random sample within the given symbol probabilities.
        /// </summary>
        /// 
        /// <param name="probabilities">The probabilities for the discrete symbols.</param>
        /// <param name="samples">The number of samples to generate.</param>
        /// 
        /// <returns>A random sample within the given probabilities.</returns>
        /// 
        public static int[] Random(double[] probabilities, int samples)
        {
            return Random(probabilities, samples, Accord.Math.Random.Generator.Random);
        }

        /// <summary>
        ///   Returns a random sample within the given symbol probabilities.
        /// </summary>
        /// 
        /// <param name="probabilities">The probabilities for the discrete symbols.</param>
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        /// 
        /// <returns>A random sample within the given probabilities.</returns>
        /// 
        public static int[] Random(double[] probabilities, int samples, Random source)
        {
            return Random(probabilities, samples, new int[samples], source);
        }

        /// <summary>
        ///   Returns a random sample within the given symbol probabilities.
        /// </summary>
        /// 
        /// <param name="probabilities">The probabilities for the discrete symbols.</param>
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        /// <param name="log">Pass true if the <paramref name="probabilities"/> vector
        ///   contains log-probabilities instead of standard probabilities.</param>
        ///
        /// <returns>A random sample within the given probabilities.</returns>
        /// 
        public static int[] Random(double[] probabilities, int samples, int[] result, bool log = false)
        {
            return Random(probabilities, samples, result, Accord.Math.Random.Generator.Random, log);
        }

        /// <summary>
        ///   Returns a random sample within the given symbol probabilities.
        /// </summary>
        /// 
        /// <param name="probabilities">The probabilities for the discrete symbols.</param>
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        /// <param name="log">Pass true if the <paramref name="probabilities"/> vector
        ///   contains log-probabilities instead of standard probabilities.</param>
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        ///
        /// <returns>A random sample within the given probabilities.</returns>
        /// 
        public static int[] Random(double[] probabilities, int samples, int[] result, Random source, bool log = false)
        {
            if (log)
            {
                double[] cumulativeSum = probabilities.Exp();

                // Use the probabilities to partition the 0,1 interval
                probabilities.CumulativeSum(result: cumulativeSum);

                for (int j = 0; j < result.Length; j++)
                {
                    double u = source.NextDouble();

                    // Check in which range the values fall into
                    for (int i = 0; i < cumulativeSum.Length; i++)
                    {
                        if (u < cumulativeSum[i])
                        {
                            result[j] = i;
                            break;
                        }
                    }
                }
            }
            else
            {
                // Use the probabilities to partition the 0,1 interval
                double[] cumulativeSum = probabilities.CumulativeSum();

                for (int j = 0; j < result.Length; j++)
                {
                    double u = source.NextDouble();

                    // Check in which range the values fall into
                    for (int i = 0; i < cumulativeSum.Length; i++)
                    {
                        if (u < cumulativeSum[i])
                        {
                            result[j] = i;
                            break;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        ///   Returns a random symbol within the given symbol probabilities.
        /// </summary>
        /// 
        /// <param name="probabilities">The probabilities for the discrete symbols.</param>
        /// <param name="log">Pass true if the <paramref name="probabilities"/> vector
        ///   contains log-probabilities instead of standard probabilities.</param>
        /// 
        /// <returns>A random symbol within the given probabilities.</returns>
        /// 
        public static int Random(double[] probabilities, bool log = false)
        {
            return Random(probabilities, Accord.Math.Random.Generator.Random, log);
        }

        /// <summary>
        ///   Returns a random symbol within the given symbol probabilities.
        /// </summary>
        /// 
        /// <param name="probabilities">The probabilities for the discrete symbols.</param>
        /// <param name="log">Pass true if the <paramref name="probabilities"/> vector
        ///   contains log-probabilities instead of standard probabilities.</param>
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        /// 
        /// <returns>A random symbol within the given probabilities.</returns>
        /// 
        public static int Random(double[] probabilities, Random source, bool log = false)
        {
            double cumulativeSum = 0;
            double uniform = source.NextDouble();

            // Use the probabilities to partition the [0,1] interval 
            //  and check inside which range the values fall into.
            if (log)
            {
                for (int i = 0; i < probabilities.Length; i++)
                {
                    cumulativeSum += Math.Exp(probabilities[i]);
                    if (uniform < cumulativeSum)
                        return i;
                }
            }
            else
            {
                for (int i = 0; i < probabilities.Length; i++)
                {
                    cumulativeSum += probabilities[i];
                    if (uniform < cumulativeSum)
                        return i;
                }
            }

            if (cumulativeSum < 1e-100)
                throw new ArgumentException("probabilities", "All probabilities are zero.");

            throw new InvalidOperationException("The given probabilities do not sum up to one. Please normalize them by " +
                "dividing the probabilities by their sum. If the probabilities have already been normalized, this can be due " +
                "a numerical inaccuracy. If this is the case, try transforming the probabilities to logarithms and including " +
                "'log = true' in the arguments of the GeneralDiscreteDistribution.Random(double[] probabilities, bool log) function.");
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
            var provider = new CSharpArrayFormatProvider(formatProvider,
                includeTypeName: false, includeSemicolon: false);

            if (log)
                return String.Format("Categorical(x; p = log({0}))", probabilities.ToString(format, provider));
            return String.Format("Categorical(x; p = {0})", probabilities.ToString(format, provider));
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).</param>
        /// <param name="weights">The weight vector containing the weight for each of the samples.</param>
        /// <param name="options">Optional arguments which may be used during fitting, such
        ///   as regularization constants and additional parameters.</param>
        ///   
        public void Fit(int[] observations, double[] weights, GeneralDiscreteOptions options)
        {
            var p = new double[probabilities.Length];

            // Parse options
            double minimum = 0;
            bool useLaplace = false;
            double regularization = 0;

            if (options != null)
            {
                minimum = options.Minimum;
                useLaplace = options.UseLaplaceRule;
                regularization = options.Regularization;
            }


            if (weights == null)
            {
                for (int i = 0; i < observations.Length; i++)
                {
                    int j = observations[i] - start;
                    p[j]++;
                }
            }
            else
            {
                if (observations.Length != weights.Length)
                    throw new ArgumentException("The weight vector should have the same size as the observations",
                        "weights");

                for (int i = 0; i < observations.Length; i++)
                {
                    int j = observations[i] - start;
                    p[j] += weights[i] * observations.Length;
                }
            }

            if (regularization > 0)
            {
                for (int i = 0; i < p.Length; i++)
                {
                    double prev = log ? Math.Exp(this.probabilities[i]) : probabilities[i];
                    double num = p[i] + prev * regularization;
                    double den = p.Length + regularization;
                    p[i] = num / den;
                }
            }

            if (useLaplace)
            {
                for (int i = 0; i < p.Length; i++)
                    p[i] = (p[i] + 1) / (double)(observations.Length + p.Length);
            }
            else if (observations.Length != 0)
            {
                for (int i = 0; i < p.Length; i++)
                    p[i] /= observations.Length;
            }

            if (minimum != 0)
            {
                double sum = 0;
                for (int i = 0; i < p.Length; i++)
                {
                    if (p[i] == 0)
                        p[i] = options.Minimum;
                    sum += p[i];
                }

                for (int i = 0; i < p.Length; i++)
                    p[i] /= sum;
            }

            Accord.Diagnostics.Debug.Assert(!p.HasNaN());

            if (log)
                p.Log(result: p);

            initialize(0, p, log);
        }

        // TODO: unify both methods 
        // can only be done after HMM interface normalization is over

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).</param>
        /// <param name="weights">The weight vector containing the weight for each of the samples.</param>
        /// <param name="options">Optional arguments which may be used during fitting, such
        ///   as regularization constants and additional parameters.</param>
        ///   
        public void Fit(double[][] observations, double[] weights = null, GeneralDiscreteOptions options = null)
        {
            // TODO : Model the sum of weights
            var p = new double[probabilities.Length];

            // Parse options
            double minimum = 0;
            bool useLaplace = false;
            double regularization = 0;

            // TODO: Transfer the common parts to specialized functions
            if (options != null)
            {
                minimum = options.Minimum;
                useLaplace = options.UseLaplaceRule;
                regularization = options.Regularization;
            }


            if (weights == null)
            {
                for (int i = 0; i < observations.Length; i++)
                    for (int j = 0; j < observations[i].Length; j++)
                        p[j] += observations[i][j];
            }
            else
            {
                if (observations.Length != weights.Length)
                    throw new ArgumentException("The weight vector should have the same size as the observations",
                        "weights");

                for (int i = 0; i < observations.Length; i++)
                    for (int j = 0; j < observations[i].Length; j++)
                        p[j] += observations[i][j] * weights[i];
            }

            if (regularization > 0)
            {
                for (int i = 0; i < p.Length; i++)
                {
                    double prev = log ? Math.Exp(this.probabilities[i]) : probabilities[i];
                    double num = p[i] + prev * regularization;
                    double den = p.Length + regularization;
                    p[i] = num / den;
                }
            }

            if (useLaplace)
            {
                for (int i = 0; i < p.Length; i++)
                    p[i] = (p[i] + 1) / (double)(observations.Length + p.Length);
            }
            else
            {
                for (int i = 0; i < p.Length; i++)
                    p[i] /= observations.Length;
            }

            if (minimum != 0)
            {
                double sum = 0;
                for (int i = 0; i < p.Length; i++)
                {
                    if (p[i] == 0)
                        p[i] = options.Minimum;
                    sum += p[i];
                }

                for (int i = 0; i < p.Length; i++)
                    p[i] /= sum;
            }

            Accord.Diagnostics.Debug.Assert(!p.HasNaN());

            if (log)
                p.Log(result: p);

            initialize(0, p, log);
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).</param>
        ///   
        public void Fit(double[][] observations)
        {
            Fit(observations, null, null);
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).</param>
        /// <param name="weights">The weight vector containing the weight for each of the samples.</param>
        ///   
        public void Fit(double[][] observations, double[] weights)
        {
            Fit(observations, weights, null);
        }


        /// <summary>
        ///   Creates general discrete distributions given a matrix of symbol probabilities.
        /// </summary>
        /// 
        public static GeneralDiscreteDistribution[] FromMatrix(double[,] probabilities, bool logarithm = false)
        {
            var B = new GeneralDiscreteDistribution[probabilities.Rows()];
            for (int i = 0; i < B.Length; i++)
                B[i] = new GeneralDiscreteDistribution(logarithm, probabilities.GetRow(i));
            return B;
        }

        /// <summary>
        ///   Creates general discrete distributions given a matrix of symbol probabilities.
        /// </summary>
        /// 
        public static GeneralDiscreteDistribution[] FromMatrix(double[][] probabilities, bool logarithm = false)
        {
            var B = new GeneralDiscreteDistribution[probabilities.Rows()];
            for (int i = 0; i < B.Length; i++)
                B[i] = new GeneralDiscreteDistribution(logarithm, probabilities[i]);
            return B;
        }


    }
}
