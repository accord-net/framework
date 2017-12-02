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
    using Accord.Math;
    using Accord.Math.Comparers;
    using Accord.Statistics.Distributions.Fitting;
    using System;
    using Accord.Compat;

    /// <summary>
    ///   Outcome status for survival methods. A sample can 
    ///   enter the experiment, exit the experiment while still
    ///   alive or exit the experiment due to failure.
    /// </summary>
    /// 
    public enum SurvivalOutcome
    {
        /// <summary>
        ///   Observation started. The observation was left censored before
        ///   the current time and has now entered the experiment. This is 
        ///   equivalent to R's censoring code -1.
        /// </summary>
        /// 
        Started = -1,

        /// <summary>
        ///   Failure happened. This is equivalent to R's censoring code 1.
        /// </summary>
        /// 
        Failed = 1,

        /// <summary>
        ///   The sample was right-censored. This is equivalent to R's censoring code 0.
        /// </summary>
        /// 
        Censored = 0,
    }

    /// <summary>
    ///   Estimators for estimating parameters of Hazard distributions.
    /// </summary>
    /// 
    public enum HazardEstimator
    {
        /// <summary>
        ///   Breslow-Nelson-Aalen estimator (default).
        /// </summary>
        /// 
        BreslowNelsonAalen = 0,

        /// <summary>
        ///   Kaplan-Meier estimator.
        /// </summary>
        /// 
        KaplanMeier
    }

    /// <summary>
    ///   Methods for handling ties in hazard/survival estimation algorithms.
    /// </summary>
    /// 
    public enum HazardTiesMethod
    {
        /// <summary>
        ///   Efron's method for ties (default).
        /// </summary>
        /// 
        Efron = 0,

        /// <summary>
        ///   Breslow's method for ties.
        /// </summary>
        /// 
        Breslow,
    }

    /// <summary>
    ///   Estimators for Survival distribution functions.
    /// </summary>
    /// 
    public enum SurvivalEstimator
    {
        /// <summary>
        ///   Fleming-Harrington estimator (default).
        /// </summary>
        /// 
        FlemingHarrington = 0,

        /// <summary>
        ///   Kaplan-Meier estimator.
        /// </summary>
        /// 
        KaplanMeier,
    }

    /// <summary>
    ///   Empirical Hazard Distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Empirical Hazard (or Survival) Distribution can be used as an
    ///   estimative of the true Survival function for a dataset which does
    ///   not relies on distribution or model assumptions about the data.</para>
    ///   
    /// <para>
    ///   The most direct use for this class is in Survival Analysis, such as when
    ///   using or creating <see cref=" Accord.Statistics.Models.Regression.ProportionalHazards">
    ///   Cox's Proportional Hazards models</see>.</para>
    ///   
    /// // references
    /// http://www.statsdirect.com/help/default.htm#survival_analysis/kaplan_meier.htm
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   The following example shows how to construct an empirical hazards 
    ///   function from a set of hazard values at the given time instants.</para>
    /// <code>
    /// 
    ///   // Consider the following observations, occurring at the given time steps
    ///   double[] times = { 11, 10, 9, 8, 6, 5, 4, 2 };
    ///   double[] values = { 0.22, 0.67, 1.00, 0.18, 1.00, 1.00, 1.00, 0.55 };
    ///   
    ///   // Create a new empirical distribution function given the observations and event times
    ///   EmpiricalHazardDistribution distribution = new EmpiricalHazardDistribution(times, values);
    ///   
    ///   // Common measures
    ///   double mean   = distribution.Mean;     // 2.1994135014183138
    ///   double median = distribution.Median;   // 3.9999999151458066
    ///   double var    = distribution.Variance; // 4.2044065839577112
    ///   
    ///   // Cumulative distribution functions
    ///   double cdf = distribution.DistributionFunction(x: 4.2);               // 0.7877520261732569
    ///   double ccdf = distribution.ComplementaryDistributionFunction(x: 4.2); // 0.21224797382674304
    ///   double icdf = distribution.InverseDistributionFunction(p: cdf);       // 4.3304819115496436
    ///   
    ///   // Probability density functions
    ///   double pdf = distribution.ProbabilityDensityFunction(x: 4.2);     // 0.21224797382674304
    ///   double lpdf = distribution.LogProbabilityDensityFunction(x: 4.2); // -1.55
    ///   
    ///   // Hazard (failure rate) functions
    ///   double hf = distribution.HazardFunction(x: 4.2);            // 1.0
    ///   double chf = distribution.CumulativeHazardFunction(x: 4.2); // 1.55
    ///   
    ///   // String representation
    ///   string str = distribution.ToString(); // H(x; v, t)
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="Accord.Statistics.Models.Regression.ProportionalHazards"/>
    /// 
    [Serializable]
    public class EmpiricalHazardDistribution : UnivariateContinuousDistribution,
        IFittableDistribution<double, SurvivalOptions>,
        IFittableDistribution<double, EmpiricalHazardOptions>
    {
        private double? mean;
        private double? variance;

        private double[] times;

        private double[] hazards;
        private double[] survivals;

        private DoubleRange range;

        private SurvivalEstimator estimator;


        /// <summary>
        ///   Gets the time steps of the hazard density values.
        /// </summary>
        /// 
        public double[] Times { get { return times; } }

        /// <summary>
        ///   Gets the hazard rate values at each time step.
        /// </summary>
        /// 
        public double[] Hazards { get { return hazards; } }

        /// <summary>
        ///   Gets the survival values at each time step.
        /// </summary>
        /// 
        public double[] Survivals { get { return survivals; } }

        /// <summary>
        ///   Gets the survival function estimator being used in this distribution.
        /// </summary>
        /// 
        public SurvivalEstimator Estimator
        {
            get { return estimator; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="EmpiricalHazardDistribution"/> class.
        /// </summary>
        /// 
        public EmpiricalHazardDistribution()
        {
            init(null, null, SurvivalOptions.DefaultSurvival);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="EmpiricalHazardDistribution"/> class.
        /// </summary>
        /// 
        /// <param name="time">The time steps.</param>
        /// <param name="lambdas">The hazard rates at the time steps.</param>
        /// 
        public EmpiricalHazardDistribution(double[] time, double[] lambdas)
        {
            init(time, lambdas, SurvivalOptions.DefaultSurvival);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="EmpiricalHazardDistribution"/> class.
        /// </summary>
        /// 
        /// <param name="times">The time steps.</param>
        /// <param name="lambdas">The hazard rates at the time steps.</param>
        /// <param name="estimator">The survival function estimator to be used. Default is 
        ///   <see cref="SurvivalEstimator.FlemingHarrington"/></param>
        /// 
        public EmpiricalHazardDistribution(double[] times, double[] lambdas, SurvivalEstimator estimator)
        {
            if (times == null)
                throw new ArgumentNullException("time");

            if (lambdas == null)
                throw new ArgumentNullException("values");

            if (times.Length != lambdas.Length)
            {
                throw new DimensionMismatchException("time",
                    "The time steps and value vectors must have the same length.");
            }

            init(times, lambdas, estimator);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="EmpiricalHazardDistribution"/> class.
        /// </summary>
        /// 
        /// <param name="estimator">The survival function estimator to be used. Default is 
        ///   <see cref="SurvivalEstimator.FlemingHarrington"/></param>
        /// 
        public EmpiricalHazardDistribution(SurvivalEstimator estimator)
        {
            init(null, null, estimator);
        }

        void init(double[] times, double[] hazards, SurvivalEstimator estimator)
        {
            if (times == null)
                times = new double[0];

            if (hazards == null)
                hazards = new double[0];

            times = (double[])times.Clone();
            hazards = (double[])hazards.Clone();

            Array.Sort(times, hazards);

            for (int i = 0; i < times.Length - 1; i++)
            {
                if (times[i] > times[i + 1])
                    throw new Exception();
            }

            if (times.Length > 0)
            {
                this.range = new DoubleRange(0, times[times.Length - 1] + 1);

                this.survivals = new double[times.Length];
                this.survivals[0] = 1;
                for (int i = 1; i < survivals.Length; i++)
                    survivals[i] = survivals[i - 1] * (1.0 - hazards[i - 1]);
            }

            this.times = times;
            this.hazards = hazards;
            this.estimator = estimator;

            this.mean = null;
            this.variance = null;
        }

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's mean value.
        /// </value>
        /// 
        public override double Mean
        {
            get
            {
                if (!mean.HasValue)
                {
                    // http://www.stat.nuk.edu.tw/wongkf_html/survival02.pdf
                    double m = 0;
                    foreach (var t in times.Distinct())
                        m += t * ProbabilityDensityFunction(t);
                    mean = m;
                }

                return mean.Value;
            }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's variance.
        /// </value>
        /// 
        public override double Variance
        {
            get
            {
                if (!variance.HasValue)
                {
                    // http://www.stat.nuk.edu.tw/wongkf_html/survival02.pdf

                    double v = 0;
                    foreach (var t in times.Distinct())
                        v += t * t * ProbabilityDensityFunction(t);

                    double m = Mean;
                    this.variance = v - m * m;
                }

                return variance.Value;
            }
        }

        /// <summary>
        ///   This method is not supported.
        /// </summary>
        /// 
        public override double Mode
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        ///   This method is not supported.
        /// </summary>
        /// 
        public override double Entropy
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        ///   Gets the support interval for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   A <see cref="DoubleRange" /> containing
        ///   the support interval for this distribution.
        /// </value>
        /// 
        public override DoubleRange Support
        {
            get { return range; }
        }




        /// <summary>
        ///   Gets the complementary cumulative distribution function
        ///   (ccdf) for this distribution evaluated at point <c>x</c>.
        ///   This function is also known as the Survival function.
        /// </summary>
        /// 
        /// <param name="x">
        ///   A single point in the distribution range.</param>
        ///   
        /// <remarks>
        ///   The Complementary Cumulative Distribution Function (CCDF) is
        ///   the complement of the Cumulative Distribution Function, or 1
        ///   minus the CDF. In the Empirical Hazard Distribution, this function
        ///   is computed using the Fleming-Harrington estimator.
        /// </remarks>
        /// 
        protected internal override double InnerComplementaryDistributionFunction(double x)
        {
            if (estimator == SurvivalEstimator.KaplanMeier)
            {
                // Kaplan-Meier estimator
                for (int i = 0; i < survivals.Length; i++)
                {
                    if (times[i] > x)
                        return survivals[i];
                }

                return survivals[survivals.Length - 1];
            }

            else // (estimator == SurvivalEstimator.FlemingHarrington)
            {
                // Use the Fleming-Harrington estimator. In general, this estimator
                // will be close to the Kaplan-Meier estimator, but not equal (page 98)
                // http://www.amstat.org/chapters/northeasternillinois/pastevents/presentations/summer05_Ibrahim_J.pdf

                // H(x)   = -ln(1-F(x))
                // 1-F(x) = exp(-H(x))
                double chf = CumulativeHazardFunction(x);
                double s = Math.Exp(-chf);
                return s;
            }
        }

        /// <summary>
        ///   Gets the cumulative hazard function for this
        ///   distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The cumulative hazard function <c>H(x)</c>
        ///   evaluated at <c>x</c> in the current distribution.
        /// </returns>
        /// 
        public override double CumulativeHazardFunction(double x)
        {
            if (estimator == SurvivalEstimator.KaplanMeier)
            {
                // Compute the cumulative hazard function using the Kaplan-Meier estimator (page 98)
                // http://www.amstat.org/chapters/northeasternillinois/pastevents/presentations/summer05_Ibrahim_J.pdf

                // This is also known as the method of Peterson (1977):
                // Peterson AV Jr.. Expressing the Kaplan-Meier estimator as a function of empirical 
                // subsurvival functions. Journal of the American Statistical Association 1977;72:854-858.
                return -Math.Log(ComplementaryDistributionFunction(x));
            }
            else // (estimator == SurvivalEstimator.FlemingHarrington)
            {
                // Fleming-Harrington estimator
                double sum = 0;
                for (int i = 0; i < times.Length; i++)
                {
                    if (times[i] <= x)
                        sum += hazards[i];
                }

                return sum;
            }
        }

        /// <summary>
        ///   Gets the hazard function, also known as the failure rate or
        ///   the conditional failure density function for this distribution
        ///   evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The conditional failure density function <c>h(x)</c>
        ///   evaluated at <c>x</c> in the current distribution.
        /// </returns>
        /// 
        public override double HazardFunction(double x)
        {
            if (x < range.Min || x > range.Max)
                return 0;

            double sum = 0;
            for (int i = 0; i < times.Length; i++)
            {
                if (times[i] == x)
                    sum += hazards[i];
            }

            return sum;
        }


        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The probability of <c>x</c> occurring in the current distribution.
        /// </returns>
        /// 
        /// <remarks>
        ///   In the Empirical Hazard Distribution, the PDF is defined
        ///   as the product of the hazard function h(x) and survival 
        ///   function S(x), as PDF(x) = h(x) * S(x).
        /// </remarks>
        /// 
        protected internal override double InnerProbabilityDensityFunction(double x)
        {
            return HazardFunction(x) * ComplementaryDistributionFunction(x);
        }

        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <remarks>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.
        /// </remarks>
        /// 
        protected internal override double InnerDistributionFunction(double x)
        {
            return 1.0 - ComplementaryDistributionFunction(x);
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
            return new EmpiricalHazardDistribution((double[])Times.Clone(), (double[])Hazards.Clone(), estimator);
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">The array of observations to fit the model against. The array
        /// elements can be either of type double (for univariate data) or
        /// type double[] (for multivariate data).</param>
        /// <param name="weights">The weight vector containing the weight for each of the samples.</param>
        /// <param name="options">Optional arguments which may be used during fitting, such
        /// as regularization constants and additional parameters.</param>
        /// 
        public override void Fit(double[] observations, double[] weights, IFittingOptions options)
        {
            Fit(observations, weights, options as SurvivalOptions);
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">The array of observations to fit the model against. The array
        /// elements can be either of type double (for univariate data) or
        /// type double[] (for multivariate data).</param>
        /// <param name="weights">The weight vector containing the weight for each of the samples.</param>
        /// <param name="options">Optional arguments which may be used during fitting, such
        /// as regularization constants and additional parameters.</param>
        /// 
        public void Fit(double[] observations, double[] weights, EmpiricalHazardOptions options)
        {
            Fit(observations, weights, options as SurvivalOptions);
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">The array of observations to fit the model against. The array
        /// elements can be either of type double (for univariate data) or
        /// type double[] (for multivariate data).</param>
        /// <param name="weights">The weight vector containing the weight for each of the samples.</param>
        /// <param name="options">Optional arguments which may be used during fitting, such
        /// as regularization constants and additional parameters.</param>
        /// 
        public void Fit(double[] observations, double[] weights, SurvivalOptions options)
        {
            SurvivalOutcome[] outcome = null;
            double[] output = weights;
            HazardEstimator hazardEstimator = HazardEstimator.BreslowNelsonAalen;
            HazardTiesMethod ties = HazardTiesMethod.Breslow;

            if (options != null)
            {
                outcome = options.Outcome;
                var hazardOps = options as EmpiricalHazardOptions;
                if (hazardOps != null)
                {
                    hazardEstimator = hazardOps.Estimator;
                    ties = hazardOps.Ties;
                }
            }

            if (outcome == null)
                outcome = Vector.Create(observations.Length, SurvivalOutcome.Failed);

            if (output == null)
                output = Vector.Create(observations.Length, 1.0);



            if (hazardEstimator == HazardEstimator.KaplanMeier)
            {
                double[] times = observations.Distinct();
                double[] hazards = new double[times.Length];
                hazards[hazards.Length - 1] = 1;

                Array.Sort(times);

                // Compute an estimate of the hazard function using
                // the Kaplan-Meier estimator for the survival function

                double r = observations.Length;
                double d = 0; // Number of deaths
                double c = 0; // Number of censor

                for (int j = 0; j < times.Length; j++)
                {
                    double t = times[j];

                    // Survivals until time t
                    r = r - d - c;
                    d = 0;
                    c = 0;

                    for (int i = 0; i < observations.Length; i++)
                    {
                        if (observations[i] == t)
                        {
                            if (outcome[i] == SurvivalOutcome.Failed)
                                d++; // Failure at time t 
                            else
                                c++; // Censored at time t
                        }
                    }

                    hazards[j] = d / r;
                }

                this.init(times, hazards, this.estimator);
                return;
            }
            else if (hazardEstimator == HazardEstimator.BreslowNelsonAalen)
            {
                double[] survivals = new double[observations.Length];

                Sort(ref observations, ref outcome, ref output);

                // Compute an estimate of the cumulative Hazard
                // function using the Breslow-Nelson-Aalen estimator

                double sum = 0;
                double d = 0;
                for (int i = 0; i < observations.Length; i++)
                {
                    survivals[i] = 1;
                    double t = observations[i];
                    double v = output[i];
                    var o = outcome[i];

                    sum += v;

                    if (o == SurvivalOutcome.Censored)
                    {
                        d = 0;
                        continue;
                    }

                    if (i > 0 && t != observations[i - 1])
                        d = 0;

                    // Count the number of deaths at t
                    if (o == SurvivalOutcome.Failed)
                        d++; // Deaths at time t

                    if (i < observations.Length - 1 && t == observations[i + 1] && outcome[i + 1] == SurvivalOutcome.Failed)
                        continue;

                    if (ties == HazardTiesMethod.Breslow)
                    {
                        survivals[i] = Math.Exp(-d / sum);
                    }
                    else
                    {
                        if (d == 1)
                        {
                            survivals[i] = Math.Pow(1 - v / sum, 1 / v);
                        }
                        else
                        {
                            survivals[i] = Math.Exp(-d / sum);
                        }
                    }
                }

                // Transform to hazards
                double[] hazards = new double[observations.Length];
                for (int i = 0; i < hazards.Length; i++)
                    hazards[i] = -Math.Log(survivals[i]);

                Array.Sort(observations, hazards);

                this.init(observations, hazards, this.estimator);
                return;
            }

            throw new ArgumentException();
        }

        /// <summary>
        ///   Sorts time-censored events considering their time of occurrence and the type of event.
        ///   Events are first sorted in decreased order of occurrence, and then with failures coming
        ///   before censoring.
        /// </summary>
        /// 
        /// <param name="time">The time of occurrence for the event.</param>
        /// <param name="output">The outcome at the time of event (failure or censored).</param>
        /// 
        /// <returns>The indices of the new sorting.</returns>
        /// 
        public static int[] Sort(ref double[] time, ref SurvivalOutcome[] output)
        {
            double[] keys = new double[time.Length];
            for (int i = 0; i < keys.Length; i++)
                keys[i] = time[i] - 1e-5 * (int)output[i];

            int[] idx;
            Vector.Sort(keys, out idx, stable: true, direction: ComparerDirection.Descending);

            time = time.Get(idx);
            output = output.Get(idx);

            return idx;
        }

        /// <summary>
        ///   Sorts time-censored events considering their time of occurrence and the type of event.
        ///   Events are first sorted in decreased order of occurrence, and then with failures coming
        ///   before censoring.
        /// </summary>
        /// 
        /// <param name="time">The time of occurrence for the event.</param>
        /// <param name="output">The outcome at the time of event (failure or censored).</param>
        /// <param name="inputs">The input vector associated with the event.</param>
        /// 
        /// <returns>The indices of the new sorting.</returns>
        /// 
        public static int[] Sort(ref double[] time, ref SurvivalOutcome[] output, ref double[][] inputs)
        {
            int[] idx = Sort(ref time, ref output);
            inputs = inputs.Get(idx);
            return idx;
        }

        /// <summary>
        ///   Sorts time-censored events considering their time of occurrence and the type of event.
        ///   Events are first sorted in decreased order of occurrence, and then with failures coming
        ///   before censoring.
        /// </summary>
        /// 
        /// <param name="time">The time of occurrence for the event.</param>
        /// <param name="output">The outcome at the time of event (failure or censored).</param>
        /// <param name="weights">The weights associated with each event.</param>
        /// 
        /// <returns>The indices of the new sorting.</returns>
        /// 
        public static int[] Sort(ref double[] time, ref SurvivalOutcome[] output, ref double[] weights)
        {
            int[] idx = Sort(ref time, ref output);
            weights = weights.Get(idx);
            return idx;
        }

        /// <summary>
        ///   Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// 
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// 
        /// <returns>
        ///   A <see cref="System.String" /> that represents this instance.
        /// </returns>
        /// 
        public override string ToString(string format, IFormatProvider formatProvider)
        {
            return String.Format(formatProvider, "H(x; v, t)");
        }



        /// <summary>
        ///   Estimates an Empirical Hazards distribution considering event times and the outcome of the
        ///   observed sample at the time of event, plus additional parameters for the hazard estimation.
        /// </summary>
        /// 
        /// <param name="time">The time of occurrence for the event.</param>
        /// <param name="weights">The weights associated with each event.</param>
        /// <param name="hazard">The hazard estimator to use. Default is <see cref="HazardEstimator.BreslowNelsonAalen"/>.</param>
        /// <param name="survival">The survival estimator to use. Default is <see cref="SurvivalEstimator.FlemingHarrington"/>.</param>
        /// <param name="ties">The method for handling event ties. Default is <see cref="HazardTiesMethod.Efron"/>.</param>
        /// 
        /// <returns>The <see cref="EmpiricalHazardDistribution"/> estimated from the given data.</returns>
        /// 
        public static EmpiricalHazardDistribution Estimate(double[] time, double[] weights = null,
            SurvivalEstimator survival = EmpiricalHazardOptions.DefaultSurvival,
            HazardEstimator hazard = EmpiricalHazardOptions.DefaultEstimator, HazardTiesMethod ties = EmpiricalHazardOptions.DefaultTies)
        {
            return Estimate(time, (SurvivalOutcome[])null, weights, survival, hazard, ties);
        }

        /// <summary>
        ///   Estimates an Empirical Hazards distribution considering event times and the outcome of the
        ///   observed sample at the time of event, plus additional parameters for the hazard estimation.
        /// </summary>
        /// 
        /// <param name="time">The time of occurrence for the event.</param>
        /// <param name="outcome">The outcome at the time of event (failure or censored).</param>
        /// <param name="weights">The weights associated with each event.</param>
        /// <param name="hazard">The hazard estimator to use. Default is <see cref="HazardEstimator.BreslowNelsonAalen"/>.</param>
        /// <param name="survival">The survival estimator to use. Default is <see cref="SurvivalEstimator.FlemingHarrington"/>.</param>
        /// <param name="ties">The method for handling event ties. Default is <see cref="HazardTiesMethod.Efron"/>.</param>
        /// 
        /// <returns>The <see cref="EmpiricalHazardDistribution"/> estimated from the given data.</returns>
        /// 
        public static EmpiricalHazardDistribution Estimate(double[] time, int[] outcome, double[] weights = null,
            SurvivalEstimator survival = EmpiricalHazardOptions.DefaultSurvival,
            HazardEstimator hazard = EmpiricalHazardOptions.DefaultEstimator, HazardTiesMethod ties = EmpiricalHazardOptions.DefaultTies)
        {
            return Estimate(time, outcome.To<SurvivalOutcome[]>(), weights, survival, hazard, ties);
        }

        /// <summary>
        ///   Estimates an Empirical Hazards distribution considering event times and the outcome of the
        ///   observed sample at the time of event, plus additional parameters for the hazard estimation.
        /// </summary>
        /// 
        /// <param name="time">The time of occurrence for the event.</param>
        /// <param name="outcome">The outcome at the time of event (failure or censored).</param>
        /// <param name="weights">The weights associated with each event.</param>
        /// <param name="hazard">The hazard estimator to use. Default is <see cref="HazardEstimator.BreslowNelsonAalen"/>.</param>
        /// <param name="survival">The survival estimator to use. Default is <see cref="SurvivalEstimator.FlemingHarrington"/>.</param>
        /// <param name="ties">The method for handling event ties. Default is <see cref="HazardTiesMethod.Efron"/>.</param>
        /// 
        /// <returns>The <see cref="EmpiricalHazardDistribution"/> estimated from the given data.</returns>
        /// 
        public static EmpiricalHazardDistribution Estimate(double[] time, SurvivalOutcome[] outcome, double[] weights = null,
            SurvivalEstimator survival = EmpiricalHazardOptions.DefaultSurvival,
            HazardEstimator hazard = EmpiricalHazardOptions.DefaultEstimator, HazardTiesMethod ties = EmpiricalHazardOptions.DefaultTies)
        {
            var dist = new EmpiricalHazardDistribution(survival);
            dist.Fit(time, weights, new EmpiricalHazardOptions
            {
                Outcome = outcome,
                Estimator = hazard,
                Ties = ties
            });

            return dist;
        }

    }
}
