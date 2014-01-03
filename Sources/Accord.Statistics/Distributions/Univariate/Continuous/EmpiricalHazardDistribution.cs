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
    using AForge;

    /// <summary>
    ///   Estimators for Hazard distribution functions.
    /// </summary>
    /// 
    public enum HazardEstimator
    {
        /// <summary>
        ///   Breslow-Nelson-Aalen estimator.
        /// </summary>
        /// 
        BreslowNelsonAalen,

        /// <summary>
        ///   Kalbfleisch &amp; Prentice estimator.
        /// </summary>
        /// 
        KalbfleischPrentice
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
    /// <para>
    ///   This class can be instantiated using either hazards values through
    ///   its <see cref="EmpiricalHazardDistribution">constructor</see> and
    ///   <see cref="FromHazardValues"/>; or using survival values using
    ///   <see cref="FromSurvivalValues"/>.</para>
    ///   
    /// <para>
    ///   The most direct use for this class is in Survival Analysis, such as when
    ///   using or creating <see cref=" Accord.Statistics.Models.Regression.ProportionalHazards">
    ///   Cox's Proportional Hazards models</see>.</para>
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
    ///   double mean   = distribution.Mean;     // 0.93696461879063664
    ///   double median = distribution.Median;   // 3.9999999151458066
    ///   double var    = distribution.Variance; // 2.0441627748096289
    ///   
    ///   // Cumulative distribution functions
    ///   double cdf = distribution.DistributionFunction(x: 4.2);           // 0.7877520261732569
    ///   double ccdf = distribution.ComplementaryDistributionFunction(x: 4.2); // 0.21224797382674304
    ///   double icdf = distribution.InverseDistributionFunction(p: cdf);       // 4.3304819115496436
    ///   
    ///   // Probability density functions
    ///   double pdf = distribution.ProbabilityDensityFunction(x: 4.2);     // 0.046694554241883471
    ///   double lpdf = distribution.LogProbabilityDensityFunction(x: 4.2); // -3.0641277326297756
    ///   
    ///   // Hazard (failure rate) functions
    ///   double hf = distribution.HazardFunction(x: 4.2);            // 0.22
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
        IFittableDistribution<double, EmpiricalHazardOptions>
    {
        private double? mean;
        private double? variance;

        private double? maxTimes;


        /// <summary>
        ///   Gets the time steps of the hazard density values.
        /// </summary>
        /// 
        public double[] Times { get; private set; }

        /// <summary>
        ///   Gets the hazard values at each time step.
        /// </summary>
        /// 
        public double[] Hazards { get; private set; }


        /// <summary>
        ///   Initializes a new instance of the <see cref="EmpiricalHazardDistribution"/> class.
        /// </summary>
        /// 
        public EmpiricalHazardDistribution()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="EmpiricalHazardDistribution"/> class.
        /// </summary>
        /// 
        /// <param name="time">The time steps.</param>
        /// <param name="values">The hazard values at the time steps.</param>
        /// 
        public EmpiricalHazardDistribution(double[] time, double[] values)
        {
            this.Times = time;
            this.Hazards = values;
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
                    for (int i = 0; i < Times.Length; i++)
                        m += ComplementaryDistributionFunction(Times[i]);
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
                    double m = Mean;

                    for (int i = 0; i < Times.Length; i++)
                        v += Times[i] * ComplementaryDistributionFunction(Times[i]);
                    
                    this.variance = v - m * m;
                }

                return variance.Value;
            }
        }

        /// <summary>
        ///   Gets the entropy for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's entropy.
        /// </value>
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
        ///   A <see cref="AForge.DoubleRange" /> containing
        ///   the support interval for this distribution.
        /// </value>
        /// 
        public override DoubleRange Support
        {
            get
            {
                if (!maxTimes.HasValue)
                    maxTimes = Matrix.Max(Times);

                return new DoubleRange(0, maxTimes.Value);
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
            double sum = 0;

            for (int i = 0; i < Times.Length; i++)
            {
                if (Times[i] <= x)
                    sum += Hazards[i];
            }

            return sum;
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
        public override double DistributionFunction(double x)
        {
            if (!maxTimes.HasValue)
                maxTimes = Matrix.Max(Times);

            if (x > maxTimes)
                return 1.0;

            // H(x) = -ln(1-F(x))
            // F(x) = 1 - exp(-H(x))
            double chf = CumulativeHazardFunction(x);
            double exp = Math.Exp(-chf);
            return 1.0 - exp;
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
        ///   minus the CDF.
        /// </remarks>
        /// 
        public override double ComplementaryDistributionFunction(double x)
        {
            // H(x)   = -ln(1-F(x))
            // 1-F(x) = exp(-H(x))
            return Math.Exp(-CumulativeHazardFunction(x));
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
        /// <remarks>
        ///   In the Empirical Hazard Distribution, the PDF is defined
        ///   as the product of the hazard function h(x) and survival 
        ///   function CDF(x), as PDF(x) = h(x) * CDF(x).
        /// </remarks>
        /// 
        public override double ProbabilityDensityFunction(double x)
        {
            // f(x) = h(x)exp(-H(x))

            // Density function is the product of the hazard and survival functions
            return HazardFunction(x) * ComplementaryDistributionFunction(x);
        }

        /// <summary>
        ///   Gets the log-probability density function (pdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The logarithm of the probability of <c>x</c>
        ///   occurring in the current distribution.
        /// </returns>
        /// 
        public override double LogProbabilityDensityFunction(double x)
        {
            // f(x) = h(x)exp(-H(x))

            // Density function is the product of the hazard and survival functions
            return Math.Log(HazardFunction(x) * ComplementaryDistributionFunction(x));
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
            for (int i = 0; i < Times.Length; i++)
                if (Times[i] >= x) return Hazards[i];
            return 0;
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
            return new EmpiricalHazardDistribution(
                (double[])Times.Clone(),
                (double[])Hazards.Clone());
        }

        /// <summary>
        ///   Creates a new Empirical Hazard Distribution from the hazard values.
        /// </summary>
        /// 
        /// <param name="time">The time steps.</param>
        /// <param name="hazard">The hazard values at the time steps.</param>
        /// 
        /// <returns>A new <see cref="EmpiricalHazardDistribution"/> using the given hazard values.</returns>
        /// 
        public static EmpiricalHazardDistribution FromHazardValues(double[] time, double[] hazard)
        {
            return new EmpiricalHazardDistribution(time, hazard);
        }

        /// <summary>
        ///   Creates a new Empirical Hazard Distribution from the survival values.
        /// </summary>
        /// 
        /// <param name="time">The time steps.</param>
        /// <param name="survival">The survival values at the time steps.</param>
        /// 
        /// <returns>A new <see cref="EmpiricalHazardDistribution"/> using the given survival values.</returns>
        /// 
        public static EmpiricalHazardDistribution FromSurvivalValues(double[] time, double[] survival)
        {
            double[] hazard = new double[survival.Length];
            for (int i = 0; i < hazard.Length; i++)
                hazard[i] = 1.0 - survival[i];
            return new EmpiricalHazardDistribution(time, hazard);
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
            Fit(observations, weights, options as EmpiricalHazardOptions);
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

            if (options == null) throw new ArgumentNullException("options", "Options can't be null");

            if (weights != null) throw new ArgumentException("Weights are not supported.", "weights");


            double[] output = options.Output;
            int[] censor = options.Censor;
            double[] time = observations;

            double[] values = new double[time.Length];


            if (options.Estimator == HazardEstimator.BreslowNelsonAalen)
            {
                // Compute an estimate of the cumulative Hazard
                // function using the Nelson-Aalen estimator

                for (int i = 0; i < values.Length; i++)
                {
                    // Check if we should censor
                    if (censor[i] == 0) continue;

                    double t = time[i];

                    int numberOfTies = 0;

                    // Count the number of ties at t = time[i]
                    for (int j = 0; j < time.Length; j++)
                        if (time[j] == t) numberOfTies++;

                    double sum = 0;
                    for (int j = 0; j < output.Length && time[j] >= time[i]; j++)
                        sum += output[j];

                    values[i] = numberOfTies / sum;
                }
            }

            else if (options.Estimator == HazardEstimator.KalbfleischPrentice)
            {
                // Compute an estimate of the cumulative Survival
                // function using Kalbfleisch & Prentice's estimator
                // assuming there are no ties in event times

                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = 1;

                    // Check if we should censor
                    if (censor[i] == 0) continue;

                    double num = output[i];

                    double den = 0;
                    for (int j = 0; j < output.Length && time[j] >= time[i]; j++)
                        den += output[j];

                    values[i] = 1.0 - Math.Pow(1.0 - num / den, 1.0 / num);
                }


            }

            this.Times = (double[])time.Clone();
            this.Hazards = values;
            this.mean = null;
            this.variance = null;
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
            return "H(x; v, t)";
        }
    }
}
