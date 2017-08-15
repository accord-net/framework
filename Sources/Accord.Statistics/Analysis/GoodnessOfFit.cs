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
    using System.ComponentModel;
    using Accord.Math;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Testing;
    using Accord.Compat;

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
}
