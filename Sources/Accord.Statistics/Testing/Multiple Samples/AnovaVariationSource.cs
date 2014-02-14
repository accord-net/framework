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

namespace Accord.Statistics.Testing
{
    using System;
    using System.ComponentModel;

    /// <summary>
    ///   Source of variation in an ANOVA experiment.
    /// </summary>
    /// 
    [Serializable]
    public class AnovaVariationSource
    {
        private IAnova anova;

        /// <summary>
        ///   Creates a new object representation of a variation source in an ANOVA experiment.
        /// </summary>
        /// 
        /// <param name="anova">The associated ANOVA analysis.</param>
        /// <param name="source">The name of the variation source.</param>
        /// 
        public AnovaVariationSource(IAnova anova, string source)
        {
            this.anova = anova;
            this.Source = source;
        }

        /// <summary>
        ///   Creates a new object representation of a variation source in an ANOVA experiment.
        /// </summary>
        /// 
        /// <param name="anova">The associated ANOVA analysis.</param>
        /// <param name="source">The name of the variation source.</param>
        /// <param name="degreesOfFreedom">The degrees of freedom for the source.</param>
        /// <param name="sumOfSquares">The sum of squares of the source.</param>
        /// 
        public AnovaVariationSource(IAnova anova, string source, double sumOfSquares,
            int degreesOfFreedom)
            : this(anova, source, sumOfSquares, degreesOfFreedom, null) { }

        /// <summary>
        ///   Creates a new object representation of a variation source in an ANOVA experiment.
        /// </summary>
        /// 
        /// <param name="anova">The associated ANOVA analysis.</param>
        /// <param name="source">The name of the variation source.</param>
        /// <param name="degreesOfFreedom">The degrees of freedom for the source.</param>
        /// <param name="meanSquares">The mean sum of squares of the source.</param>
        /// <param name="sumOfSquares">The sum of squares of the source.</param>
        /// 
        public AnovaVariationSource(IAnova anova, string source, double sumOfSquares,
            int degreesOfFreedom, double meanSquares)
            : this(anova, source, sumOfSquares, degreesOfFreedom, meanSquares, null) { }

        /// <summary>
        ///   Creates a new object representation of a variation source in an ANOVA experiment.
        /// </summary>
        /// 
        /// <param name="anova">The associated ANOVA analysis.</param>
        /// <param name="source">The name of the variation source.</param>
        /// <param name="degreesOfFreedom">The degrees of freedom for the source.</param>
        /// <param name="sumOfSquares">The sum of squares of the source.</param>
        /// <param name="test">The F-Test containing the F-Statistic for the source.</param>
        /// 
        public AnovaVariationSource(IAnova anova, string source, double sumOfSquares,
            int degreesOfFreedom, FTest test)
            : this(anova, source, sumOfSquares, degreesOfFreedom, sumOfSquares / degreesOfFreedom, test) { }

        /// <summary>
        ///   Creates a new object representation of a variation source in an ANOVA experiment.
        /// </summary>
        /// 
        /// <param name="anova">The associated ANOVA analysis.</param>
        /// <param name="source">The name of the variation source.</param>
        /// <param name="degreesOfFreedom">The degrees of freedom for the source.</param>
        /// <param name="sumOfSquares">The sum of squares of the source.</param>
        /// <param name="meanSquares">The mean sum of squares of the source.</param>
        /// <param name="test">The F-Test containing the F-Statistic for the source.</param>
        /// 
        public AnovaVariationSource(IAnova anova, string source, double sumOfSquares,
            int degreesOfFreedom, double meanSquares, FTest test)
        {
            this.anova = anova;
            this.Source = source;
            this.SumOfSquares = sumOfSquares;
            this.DegreesOfFreedom = degreesOfFreedom;
            this.Significance = test;
            this.MeanSquares = meanSquares;
        }

        /// <summary>
        ///   Gets the ANOVA associated with this source.
        /// </summary>
        /// 
        [Browsable(false)] 
        public IAnova Anova { get { return anova; } }

        /// <summary>
        ///   Gets the name of the variation source.
        /// </summary>
        /// 
        [DisplayName("Source")]
        public string Source { get; private set; }

        /// <summary>
        ///   Gets the sum of squares associated with the variation source.
        /// </summary>
        /// 
        [DisplayName("Sum of Squares")]
        public double SumOfSquares { get; private set; }

        /// <summary>
        ///   Gets the degrees of freedom associated with the variation source.
        /// </summary>
        /// 
        [DisplayName("Degrees of Freedom")]
        public int DegreesOfFreedom { get; private set; }

        /// <summary>
        ///   Get the mean squares, or the variance, associated with the source.
        /// </summary>
        /// 
        [DisplayName("Mean Squares")]
        public double MeanSquares { get; private set; }

        /// <summary>
        ///   Gets the significance of the source.
        /// </summary>
        /// 
        [DisplayName("P-Value")]
        public FTest Significance { get; private set; }

        /// <summary>
        ///   Gets the F-Statistic associated with the source's significance.
        /// </summary>
        /// 
        [DisplayName("F-Statistic")]
        public double? Statistic
        {
            get
            {
                if (Significance != null)
                    return Significance.Statistic;
                else return null;
            }
        }

    }
}
