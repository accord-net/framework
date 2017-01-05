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

namespace Accord.Statistics.Analysis
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Accord.Collections;
    using Accord.Math;
    using Accord.Math.Comparers;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Testing;
    using System.Threading.Tasks;
    using System.Diagnostics.CodeAnalysis;
    using Accord.Statistics.Distributions.Fitting;

    /// <summary>
    ///   Distribution fitness analysis.
    /// </summary>
    /// 
    /// <remarks>
    ///   The distribution analysis class can be used to perform a battery
    ///   of distribution fitting tests in order to check from which distribution
    ///   a sample is more likely to have come from.
    /// </remarks>
    /// 
    /// <example>
    /// <code source="Unit Tests\Accord.Tests.Statistics\Analysis\DistributionAnalysisTest.cs" region="doc_learn" />
    /// </example>
    /// 
    /// <seealso cref="KolmogorovSmirnovTest"/>
    /// <seealso cref="ChiSquareTest"/>
    /// <seealso cref="AndersonDarlingTest"/>
    /// 
    [Serializable]
    public class DistributionAnalysis
    {
        private double[] data;

        /// <summary>
        ///   Gets the tested distribution names.
        /// </summary>
        /// 
        /// <value>
        ///   The distribution names.
        /// </value>
        /// 
        public string[] DistributionNames { get; private set; }

        /// <summary>
        ///   Gets the estimated distributions.
        /// </summary>
        /// 
        /// <value>
        ///   The estimated distributions.
        /// </value>
        /// 
        public List<IFittableDistribution<double>> Distributions { get; set; }

        /// <summary>
        ///   Gets or sets a mapping of fitting options that should be
        ///   used when attempting to estimate each of the distributions
        ///   in <see cref="Distributions"/>.
        /// </summary>
        /// 
        public Dictionary<IFittableDistribution<double>, IFittingOptions> Options { get; set; }

        /// <summary>
        ///   Gets the <see cref="KolmogorovSmirnovTest">Kolmogorov-Smirnov tests</see>
        ///   performed against each of the candidate distributions.
        /// </summary>
        /// 
        public KolmogorovSmirnovTest[] KolmogorovSmirnov { get; private set; }

        /// <summary>
        ///   Gets the <see cref="ChiSquareTest">Chi-Square tests</see>
        ///   performed against each of the candidate distributions.
        /// </summary>
        /// 
        public ChiSquareTest[] ChiSquare { get; private set; }

        /// <summary>
        ///   Gets the <see cref="AndersonDarlingTest">Anderson-Darling tests</see>
        ///   performed against each of the candidate distributions.
        /// </summary>
        /// 
        public AndersonDarlingTest[] AndersonDarling { get; private set; }

        /// <summary>
        ///   Gets the rank of each distribution according to the Kolmogorov-Smirnov
        ///   test statistic. A value of 0 means the distribution is the most likely.
        /// </summary>
        /// 
        public int[] KolmogorovSmirnovRank { get; private set; }

        /// <summary>
        ///   Gets the rank of each distribution according to the Chi-Square
        ///   test statistic. A value of 0 means the distribution is the most likely.
        /// </summary>
        /// 
        public int[] ChiSquareRank { get; private set; }

        /// <summary>
        ///   Gets the rank of each distribution according to the Anderson-Darling
        ///   test statistic. A value of 0 means the distribution is the most likely.
        /// </summary>
        /// 
        public int[] AndersonDarlingRank { get; private set; }

        /// <summary>
        ///   Gets the goodness of fit for each candidate distribution.
        /// </summary>
        /// 
        public GoodnessOfFitCollection GoodnessOfFit { get; private set; }


        /// <summary>
        ///   Initializes a new instance of the <see cref="DistributionAnalysis"/> class.
        /// </summary>
        /// 
        /// <param name="observations">The observations to be fitted against candidate distributions.</param>
        /// 
        [Obsolete("Please use the default parameterless constructor instead.")]
        public DistributionAnalysis(double[] observations)
            : this()
        {
            this.data = observations;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="DistributionAnalysis"/> class.
        /// </summary>
        /// 
        public DistributionAnalysis()
        {
            Distributions = new List<IFittableDistribution<double>>()
            {
                new NormalDistribution(),
                new UniformContinuousDistribution(),
                new GammaDistribution(),
                new GumbelDistribution(),
                new PoissonDistribution(),
            };

            Options = new Dictionary<IFittableDistribution<double>, IFittingOptions>();
        }


        /// <summary>
        ///   Obsolete. Please use the <see cref="Learn(double[], double[])"/> method instead.
        /// </summary>
        /// 
        [Obsolete("Please use the Learn(x) method instead.")]
        public void Compute()
        {
            compute(data, null);
        }

        /// <summary>
        ///   Learns a model that can map the given inputs to the desired outputs.
        /// </summary>
        /// 
        /// <param name="x">The model inputs.</param>
        /// <param name="weights">The weight of importance for each input sample.</param>
        /// 
        /// <returns>A model that has learned how to produce suitable outputs
        ///   given the input data <paramref name="x"/>.</returns>
        /// 
        public GoodnessOfFitCollection Learn(double[] x, double[] weights = null)
        {
            compute(x, weights);
            return GoodnessOfFit;
        }

        private void compute(double[] data, double[] weights)
        {
            bool[] fail = new bool[Distributions.Count];

            // Step 1. Fit all candidate distributions to the data.
            for (int i = 0; i < Distributions.Count; i++)
            {
                var distribution = Distributions[i];

                IFittingOptions options = null;
                Options.TryGetValue(distribution, out options);

                try
                {
                    distribution.Fit(data, weights, options);
                }
                catch
                {
                    // TODO: Maybe revisit the decision to swallow exceptions here.
                    fail[i] = true;
                }
            }

            // Step 2. Use statistical tests to see how well each
            //         distribution was able to model the data.

            KolmogorovSmirnov = new KolmogorovSmirnovTest[Distributions.Count];
            ChiSquare = new ChiSquareTest[Distributions.Count];
            AndersonDarling = new AndersonDarlingTest[Distributions.Count];
            DistributionNames = new string[Distributions.Count];

            double[] ks = new double[Distributions.Count];
            double[] cs = new double[Distributions.Count];
            double[] ad = new double[Distributions.Count];

            var measures = new List<GoodnessOfFit>();
            for (int i = 0; i < Distributions.Count; i++)
            {
                ks[i] = Double.NegativeInfinity;
                cs[i] = Double.NegativeInfinity;
                ad[i] = Double.NegativeInfinity;

                var d = this.Distributions[i] as IUnivariateDistribution<double>;

                if (d == null)
                    continue;

                this.DistributionNames[i] = GetName(d.GetType());

                if (fail[i])
                    continue;

                int ms = 5000;

                run(() =>
                {
                    this.KolmogorovSmirnov[i] = new KolmogorovSmirnovTest(data, d);
                    ks[i] = -KolmogorovSmirnov[i].Statistic;
                }, ms);

                run(() =>
                {
                    this.ChiSquare[i] = new ChiSquareTest(data, d);
                    cs[i] = -ChiSquare[i].Statistic;
                }, ms);

                run(() =>
                {
                    this.AndersonDarling[i] = new AndersonDarlingTest(data, d);
                    ad[i] = AndersonDarling[i].Statistic;
                }, ms);

                if (Double.IsNaN(ks[i]))
                    ks[i] = Double.NegativeInfinity;

                if (Double.IsNaN(cs[i]))
                    cs[i] = Double.NegativeInfinity;

                if (Double.IsNaN(ad[i]))
                    ad[i] = Double.NegativeInfinity;

                measures.Add(new GoodnessOfFit(this, i));
            }

            this.KolmogorovSmirnovRank = getRank(ks);
            this.ChiSquareRank = getRank(cs);
            this.AndersonDarlingRank = getRank(ad);

            measures.Sort();

            this.GoodnessOfFit = new GoodnessOfFitCollection(measures);
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static void run(Action a, int timeoutMilliseconds)
        {
#if NET35
            try
            {
                a();
            }
            catch
            {
            }
#else
            var task = Task.Factory.StartNew(() =>
            {
                try
                {
                    a();
                }
                catch
                {
                }
            });

            task.Wait(timeoutMilliseconds);
#endif
        }

        private int[] getRank(double[] ks)
        {
            int[] idx = Vector.Range(0, Distributions.Count);
            Array.Sort(ks, idx, new GeneralComparer(ComparerDirection.Descending));

            int[] rank = new int[idx.Length];
            for (int i = 0; i < rank.Length; i++)
                rank[i] = Array.IndexOf(idx, i);

            return rank;
        }



        /// <summary>
        ///   Gets all univariate distributions (types implementing
        ///   <see cref="IUnivariateDistribution"/>) loaded in the 
        ///   current domain.
        /// </summary>
        /// 
        public static Type[] GetUnivariateDistributions()
        {
            var distributions = getTypes(typeof(IUnivariateDistribution));
            return distributions;
        }

        /// <summary>
        ///   Gets all multivariate distributions (types implementing
        ///   <see cref="IMultivariateDistribution"/>) loaded in the 
        ///   current domain.
        /// </summary>
        /// 
        public static Type[] GetMultivariateDistributions()
        {
            var distributions = getTypes(typeof(IMultivariateDistribution));
            return distributions;
        }

        /// <summary>
        ///   Gets a distribution's name in a human-readable form.
        /// </summary>
        /// 
        /// <param name="distribution">The distribution whose name must be obtained.</param>
        /// 
        public static string GetName(Type distribution)
        {
            return distribution.Name.Replace("Distribution", "");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static Type[] getTypes(Type baseType)
        {
            var distributions = AppDomain.CurrentDomain.GetAssemblies()

               .SelectMany(s =>
               {
                   try { return s.GetTypes(); }
                   catch { return new Type[0]; }
               })
               .Where(p => baseType.IsAssignableFrom(p) && !p.IsAbstract && !p.IsInterface)
               .OrderBy(p => p.Name)
               .ToArray();

            return distributions;
        }

        /// <summary>
        ///   Gets the index of the first distribution with the given name.
        /// </summary>
        /// 
        public IFittableDistribution<double> GetFirstIndex(string name)
        {
            return Distributions.Find(d => d.GetType().Name == name);
        }
    }

    /// <summary>
    ///   Goodness-of-fit result for a given distribution.
    /// </summary>
    /// 
    [Serializable]
    public class GoodnessOfFit : IComparable<GoodnessOfFit>, IComparable,
        IFormattable
    {

        private DistributionAnalysis analysis;
        private int index;

        internal GoodnessOfFit(DistributionAnalysis analysis, int index)
        {
            this.analysis = analysis;
            this.index = index;
        }

        /// <summary>
        ///   Gets the analysis that has produced this measure.
        /// </summary>
        /// 
        [Browsable(false)]
        public DistributionAnalysis Analysis
        {
            get { return analysis; }
        }

        /// <summary>
        ///   Gets the variable's index.
        /// </summary>
        /// 
        public int Index
        {
            get { return index; }
        }

        /// <summary>
        ///   Gets the distribution name
        /// </summary>
        /// 
        public string Name
        {
            get { return analysis.DistributionNames[index]; }
        }

        /// <summary>
        ///   Gets (a clone of) the measured distribution.
        /// </summary>
        /// 
        /// <value>
        ///   The distribution associated with this good-of-fit measure.
        /// </value>
        /// 
        public IFittableDistribution<double> Distribution
        {
            get { return (IFittableDistribution<double>)analysis.Distributions[index].Clone(); }
        }

        /// <summary>
        ///   Gets the value of the Kolmogorov-Smirnov statistic.
        /// </summary>
        /// 
        /// <value>
        ///   The Kolmogorov-Smirnov for the <see cref="Distribution"/>.
        /// </value>
        /// 
        public double KolmogorovSmirnov
        {
            get
            {
                if (analysis.KolmogorovSmirnov[index] == null)
                    return Double.NaN;
                return analysis.KolmogorovSmirnov[index].Statistic;
            }
        }

        /// <summary>
        ///   Gets the rank of this distribution according to the
        ///   <see cref="KolmogorovSmirnovTest">Kolmogorov-Smirnov test</see>.
        /// </summary>
        /// 
        /// <value>
        ///   An integer value where 0 indicates most probable.
        /// </value>
        /// 
        public int KolmogorovSmirnovRank
        {
            get { return analysis.KolmogorovSmirnovRank[index]; }
        }

        /// <summary>
        ///   Gets the value of the Chi-Square statistic.
        /// </summary>
        /// 
        /// <value>
        ///   The Chi-Square for the <see cref="Distribution"/>.
        /// </value>
        /// 
        public double ChiSquare
        {
            get
            {
                if (analysis.ChiSquare[index] == null)
                    return Double.NaN;
                return analysis.ChiSquare[index].Statistic;
            }
        }

        /// <summary>
        ///   Gets the rank of this distribution according to the
        ///   <see cref="ChiSquareTest">Chi-Square test</see>.
        /// </summary>
        /// 
        /// <value>
        ///   An integer value where 0 indicates most probable.
        /// </value>
        /// 
        public int ChiSquareRank
        {
            get { return analysis.ChiSquareRank[index]; }
        }

        /// <summary>
        ///   Gets the value of the Anderson-Darling statistic.
        /// </summary>
        /// 
        /// <value>
        ///   The Anderson-Darling for the <see cref="Distribution"/>.
        /// </value>
        /// 
        public double AndersonDarling
        {
            get
            {
                if (analysis.AndersonDarling[index] == null)
                    return Double.NaN;
                return analysis.AndersonDarling[index].Statistic;
            }
        }

        /// <summary>
        ///   Gets the rank of this distribution according to the
        ///   <see cref="AndersonDarlingTest">Anderson-Darling test</see>.
        /// </summary>
        /// 
        /// <value>
        ///   An integer value where 0 indicates most probable.
        /// </value>
        /// 
        public int AndersonDarlingRank
        {
            get { return analysis.AndersonDarlingRank[index]; }
        }

        /// <summary>
        ///   Compares the current object with another object of the same type.
        /// </summary>
        /// 
        /// <param name="other">An object to compare with this object.</param>
        /// 
        /// <returns>
        ///   A value that indicates the relative order of the objects being compared. The return value
        ///   has the following meanings: Value Meaning Less than zero This object is less than the 
        ///   <paramref name="other" /> parameter.Zero This object is equal to <paramref name="other" />.
        ///   Greater than zero This object is greater than <paramref name="other" />.
        /// </returns>
        /// 
        public int CompareTo(GoodnessOfFit other)
        {
            return ChiSquareRank.CompareTo(other.ChiSquareRank);
        }

        /// <summary>
        ///   Compares the current instance with another object of the same type and returns an
        ///   integer that indicates whether the current instance precedes, follows, or occurs in
        ///   the same position in the sort order as the other object.
        /// </summary>
        /// 
        /// <param name="obj">An object to compare with this instance.</param>
        /// 
        /// <returns>
        ///   A value that indicates the relative order of the objects being compared. The return
        ///   value has these meanings: Value Meaning Less than zero This instance precedes <paramref name="obj" />
        ///   in the sort order. Zero This instance occurs in the same position in the sort order as
        ///   <paramref name="obj" />. Greater than zero This instance follows <paramref name="obj" /> 
        ///   in the sort order.
        /// </returns>
        /// 
        public int CompareTo(object obj)
        {
            return CompareTo(obj as GoodnessOfFit);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// 
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        /// 
        public override string ToString()
        {
            return analysis.Distributions[index].ToString();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format to use.-or- A null reference (Nothing in Visual Basic) to use the default format defined for the type of the <see cref="T:System.IFormattable" /> implementation.</param>
        /// <param name="formatProvider">The provider to use to format the value.-or- A null reference (Nothing in Visual Basic) to obtain the numeric format information from the current locale setting of the operating system.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            var dist = analysis.Distributions[index];
            var fmt = dist as IFormattable;
            if (fmt != null)
                return fmt.ToString(format, formatProvider);
            return dist.ToString();
        }
    }

    /// <summary>
    ///   Collection of goodness-of-fit measures.
    /// </summary>
    /// 
    /// <seealso cref="DistributionAnalysis"/>
    /// 
    [Serializable]
    public class GoodnessOfFitCollection : ReadOnlyKeyedCollection<string, GoodnessOfFit>
    {
        internal GoodnessOfFitCollection(IList<GoodnessOfFit> components)
            : base(components)
        {
        }

        /// <summary>
        ///   Gets the key for item.
        /// </summary>
        /// 
        protected override string GetKeyForItem(GoodnessOfFit item)
        {
            return item.Name;
        }
    }


}
