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

namespace Accord.Statistics.Distributions.Univariate
{
    using System;
    using Accord.Math;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions;
    using AForge;
    using System.Globalization;

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
    public class GeneralDiscreteDistribution : UnivariateDiscreteDistribution,
        IFittableDistribution<double, GeneralDiscreteOptions>,
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
            if (probabilities == null) throw new ArgumentNullException("probabilities");

            initialize(start, probabilities);
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
        ///   
        public GeneralDiscreteDistribution(int start, int symbols)
        {
            initialize(start, symbols);
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
        ///   
        public GeneralDiscreteDistribution(int symbols)
            : this(0, symbols)
        {
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
                throw new ArgumentOutOfRangeException("b",
                    "The starting number a must be lower than b.");

            return new GeneralDiscreteDistribution(a, b - a + 1);
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
        ///   Gets the probabilities associated
        ///   with each discrete variable value.
        /// </summary>
        /// 
        public double[] Frequencies
        {
            get { return probabilities; }
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
                    mean = start;
                    for (int i = 0; i < probabilities.Length; i++)
                        mean += i * probabilities[i];
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
                    for (int i = 0; i < probabilities.Length; i++)
                    {
                        double d = i + start - m;
                        v += probabilities[i] * (d * d);
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
                {
                    double max = 0;
                    int imax = 0;
                    for (int i = 0; i < probabilities.Length; i++)
                    {
                        if (probabilities[i] >= max)
                        {
                            max = probabilities[i];
                            imax = i;
                        }
                    }

                    mode = imax;
                }

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
                    for (int i = 0; i < probabilities.Length; i++)
                        entropy -= probabilities[i] * System.Math.Log(probabilities[i]);
                }
                return entropy.Value;
            }
        }

        /// <summary>
        ///   Gets the support interval for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   A <see cref="AForge.DoubleRange" /> containing
        ///   the support interval for this distribution.
        /// </value>
        /// 
        public override DoubleRange Support
        {
            get { return new DoubleRange(start, start + probabilities.Length); }
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
        public override double DistributionFunction(int k)
        {
            int value = k - start;

            if (value < 0) 
                return 0;

            if (value >= probabilities.Length) 
                return 1.0;

            double sum = 0.0;
            for (int i = 0; i <= value; i++)
                sum += probabilities[i];

            return sum;
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
        public override double ProbabilityMassFunction(int k)
        {
            int value = k - start;

            if (value < 0 || value >= probabilities.Length)
                return 0;

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
        public override double LogProbabilityMassFunction(int k)
        {
            int value = k - start;

            if (value < 0 || value >= probabilities.Length)
                return double.NegativeInfinity;

            return Math.Log(probabilities[value]);
        }

        /// <summary>
        ///   Generates a random observation from the current distribution.
        /// </summary>
        /// 
        /// <returns>A random observations drawn from this distribution.</returns>
        /// 
        public int Generate()
        {
            return Random(probabilities);
        }

        /// <summary>
        ///   Generates a random vector of observations from the current distribution.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// 
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        /// 
        public int[] Generate(int samples)
        {
            return Random(probabilities, samples);
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
        public void Fit(double[] observations, double[] weights, GeneralDiscreteOptions options)
        {
            double[] p = new double[probabilities.Length];

            // Parse options
            double minimum = 0;
            bool useLaplace = false;

            if (options != null)
            {
                minimum = options.Minimum;
                useLaplace = options.UseLaplaceRule;
            }


            if (weights == null)
            {
                for (int i = 0; i < observations.Length; i++)
                    p[(int)observations[i]]++;
            }
            else
            {
                if (observations.Length != weights.Length)
                    throw new ArgumentException("The weight vector should have the same size as the observations", "weights");

                for (int i = 0; i < observations.Length; i++)
                    p[(int)observations[i]] += weights[i] * observations.Length;
            }

            if (useLaplace)
            {
                for (int i = 0; i < p.Length; i++)
                    p[i]++;

                for (int i = 0; i < p.Length; i++)
                    p[i] /= observations.Length + p.Length;
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

            System.Diagnostics.Debug.Assert(!p.HasNaN());

            initialize(0, p);
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
            GeneralDiscreteDistribution c = new GeneralDiscreteDistribution();

            c.probabilities = (double[])probabilities.Clone();
            c.start = start;
            c.mean = mean;
            c.entropy = entropy;
            c.variance = variance;

            return c;
        }


        private GeneralDiscreteDistribution()
        {
        }

        private void initialize(int s, double[] prob)
        {
            double sum = 0;
            for (int i = 0; i < prob.Length; i++)
                sum += prob[i];

            if (sum != 0 && sum != 1)
            {
                // assert that probabilities sum up to 1.
                for (int i = 0; i < prob.Length; i++)
                    prob[i] /= sum;
            }

            this.start = s;
            this.probabilities = prob;

            this.mean = null;
            this.variance = null;
            this.entropy = null;
        }

        private void initialize(int s, int symbols)
        {
            this.start = s;
            this.probabilities = new double[symbols];

            // Initialize with uniform distribution
            for (int i = 0; i < symbols; i++)
                probabilities[i] = 1.0 / symbols;

            this.mean = null;
            this.variance = null;
            this.entropy = null;
        }


        #region ISamplableDistribution<double> Members

        double[] ISampleableDistribution<double>.Generate(int samples)
        {
            return Generate(samples).ToDouble();
        }

        double ISampleableDistribution<double>.Generate()
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
            double[] uniform = new double[samples];
            for (int i = 0; i < uniform.Length; i++)
                uniform[i] = Accord.Math.Tools.Random.NextDouble();

            // Use the probabilities to partition the 0,1 interval
            double[] cumulative = probabilities.CumulativeSum();

            int[] result = new int[samples];

            for (int j = 0; j < result.Length; j++)
            {
                // Check in which range the values fall into
                for (int i = 0; i < cumulative.Length - 1; i++)
                {
                    if (uniform[j] <= cumulative[i] && uniform[j] > cumulative[i + 1])
                    {
                        result[j] = i; break;
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
        /// 
        /// <returns>A random symbol within the given probabilities.</returns>
        /// 
        public static int Random(double[] probabilities)
        {
            double uniform = Accord.Math.Tools.Random.NextDouble();

            double cumulativeSum = 0;

            // Use the probabilities to partition the [0,1] interval 
            //  and check inside which range the values fall into.

            for (int i = 0; i < probabilities.Length; i++)
            {
                cumulativeSum += probabilities[i];

                if (uniform < cumulativeSum)
                    return i;
            }

            throw new InvalidOperationException("Generated value is not between 0 and 1.");
        }



        /// <summary>
        ///   Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public override string ToString()
        {
            var provider = new CSharpArrayFormatProvider(CultureInfo.CurrentCulture,
                includeTypeName: false, includeSemicolon: false);

            return String.Format("Categorical(x; p = {0})",
                probabilities.ToString(provider));
        }

        /// <summary>
        ///   Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public string ToString(IFormatProvider formatProvider)
        {
            var provider = new CSharpArrayFormatProvider(formatProvider,
                includeTypeName: false, includeSemicolon: false);

            return String.Format("Categorical(x; p = {0})",
                probabilities.ToString(provider));
        }

        /// <summary>
        ///   Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public string ToString(string format, IFormatProvider formatProvider)
        {
            var provider = new CSharpArrayFormatProvider(formatProvider,
                includeTypeName: false, includeSemicolon: false);

            return String.Format("Categorical(x; p = {0})", probabilities
                .ToString(format, provider));
        }

        /// <summary>
        ///   Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public string ToString(string format)
        {
            var provider = new CSharpArrayFormatProvider(CultureInfo.CurrentCulture,
                includeTypeName: false, includeSemicolon: false);

            return String.Format("Categorical(x; p = {0})", probabilities
                .ToString(format, provider));
        }
    }
}
