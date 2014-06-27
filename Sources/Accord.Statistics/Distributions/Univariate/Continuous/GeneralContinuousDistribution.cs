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
    using Accord.Math.Differentiation;
    using Accord.Math.Integration;
    using Accord.Math.Optimization;
    using AForge;

    [Serializable]
    public class GeneralContinuousDistribution : UnivariateContinuousDistribution
    {

        // distribution parameters
        private IUnivariateIntegration method;
        private Func<double, double> pdf;
        private Func<double, double> cdf;
        private DoubleRange support;

        private double? mean;
        private double? variance;
        private double? entropy;
        private double? mode;


        public GeneralContinuousDistribution(DoubleRange support,
            Func<double, double> density, Func<double, double> distribution)
        {
            if (density == null)
                throw new ArgumentNullException("density");

            if (distribution == null)
                throw new ArgumentNullException("distribution");

            this.pdf = density;
            this.cdf = distribution;
        }

        public static GeneralContinuousDistribution FromDistribution(UnivariateContinuousDistribution distribution)
        {
            GeneralContinuousDistribution dist = new GeneralContinuousDistribution();
            dist.support = distribution.Support;
            dist.pdf = distribution.ProbabilityDensityFunction;
            dist.cdf = distribution.DistributionFunction;
            return dist;
        }

        public static GeneralContinuousDistribution FromDensityFunction(
    DoubleRange support, Func<double, double> pdf)
        {
            var method = new InfiniteAdaptiveGaussKronrod(100);
            return FromDistributionFunction(support, pdf, method);
        }

        public static GeneralContinuousDistribution FromDistributionFunction(
            DoubleRange support, Func<double, double> cdf)
        {
            var method = new InfiniteAdaptiveGaussKronrod(100);
            return FromDistributionFunction(support, cdf, method);
        }

        public static GeneralContinuousDistribution FromDensityFunction(
            DoubleRange support, Func<double, double> pdf, IUnivariateIntegration method)
        {
            GeneralContinuousDistribution dist = new GeneralContinuousDistribution();
            dist.support = support;
            dist.pdf = pdf;
            dist.method = method;
            method.Range = support;
            return dist;
        }

        public static GeneralContinuousDistribution FromDistributionFunction(
            DoubleRange support, Func<double, double> cdf, IUnivariateIntegration method)
        {
            GeneralContinuousDistribution dist = new GeneralContinuousDistribution();
            dist.support = support;
            dist.cdf = cdf;
            dist.method = method;
            method.Range = support;
            return dist;
        }

        public override DoubleRange Support
        {
            get { return support; }
        }


        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's mean value.</value>
        /// 
        public override double Mean
        {
            get
            {
                if (mean == null)
                {
                    method.Function = (x) => x * ProbabilityDensityFunction(x);
                    method.Compute();
                    mean = method.Area;
                }

                return mean.Value;
            }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's variance.</value>
        /// 
        public override double Variance
        {
            get
            {
                if (variance == null)
                {
                    double u = Mean;
                    method.Function = (x) => (x - u) * ProbabilityDensityFunction(x);
                    method.Compute();
                    variance = method.Area;
                }

                return variance.Value;
            }
        }

        /// <summary>
        ///   Gets the entropy for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's entropy.</value>
        /// 
        public override double Entropy
        {
            get
            {
                if (entropy == null)
                {
                    method.Function = (x) => ProbabilityDensityFunction(x) * LogProbabilityDensityFunction(x);
                    method.Compute();
                    entropy = method.Area;
                }

                return entropy.Value;
            }
        }

        public override double Mode
        {
            get
            {
                if (mode == null)
                {
                    double lowerBound = Double.IsInfinity(Support.Min) ? 1e-300 : Support.Min;
                    double upperBound = Double.IsInfinity(Support.Max) ? 1e+300 : Support.Max;
                    mode = BrentSearch.Maximize(ProbabilityDensityFunction, lowerBound, upperBound);
                }

                return mode.Value;
            }
        }

        public override double DistributionFunction(double x)
        {
            if (cdf != null)
                return cdf(x);

            method.Function = pdf;
            method.Compute();
            return method.Area;
        }

        public override double ProbabilityDensityFunction(double x)
        {
            if (pdf != null)
                return pdf(x);

            return FiniteDifferences.Derivative(cdf, x);
        }

        public override object Clone()
        {
            GeneralContinuousDistribution c = new GeneralContinuousDistribution();

            c.pdf = pdf;
            c.cdf = cdf;
            c.method = (IUnivariateIntegration)method.Clone();

            return c;
        }


        private GeneralContinuousDistribution()
        {
        }

    }
}
