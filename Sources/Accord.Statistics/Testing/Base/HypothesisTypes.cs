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
    /// <summary>
    ///   Hypothesis type
    /// </summary>
    /// 
    /// <remarks>
    ///   The type of the hypothesis being made expresses the way in
    ///   which a value of a parameter may deviate from that assumed
    ///   in the null hypothesis. It can either state that a value is
    ///   higher, lower or simply different than the one assumed under
    ///   the null hypothesis.
    /// </remarks>
    /// 
    public enum DistributionTail
    {
        /// <summary>
        ///   The test considers the two tails from a probability distribution.
        /// </summary>
        /// 
        /// <remarks>
        ///   The two-tailed test is a statistical test in which a given statistical
        ///   hypothesis, H0 (the null hypothesis), will be rejected when the value of
        ///   the test statistic is either sufficiently small or sufficiently large. 
        /// </remarks>
        /// 
        TwoTail = 0,

        /// <summary>
        ///   The test considers the upper tail from a probability distribution.
        /// </summary>
        /// 
        /// <remarks>
        ///   The one-tailed, upper tail test is a statistical test in which a given
        ///   statistical hypothesis, H0 (the null hypothesis), will be rejected when
        ///   the value of the test statistic is sufficiently large. 
        /// </remarks>
        /// 
        OneUpper = 1,

        /// <summary>
        ///   The test considers the lower tail from a probability distribution.
        /// </summary>
        /// 
        /// <remarks>
        ///   The one-tailed, lower tail test is a statistical test in which a given
        ///   statistical hypothesis, H0 (the null hypothesis), will be rejected when
        ///   the value of the test statistic is sufficiently small. 
        /// </remarks>
        /// 
        OneLower = 2,
    };


    /// <summary>
    ///   Common test Hypothesis for one sample tests, such
    ///   as <see cref="ZTest"/> and <see cref="TTest"/>.
    /// </summary>
    /// 
    public enum OneSampleHypothesis
    {
        /// <summary>
        ///   Tests if the mean (or the parameter under test)
        ///   is significantly different from the hypothesized
        ///   value, without considering the direction for this
        ///   difference.
        /// </summary>
        /// 
        ValueIsDifferentFromHypothesis = DistributionTail.TwoTail,

        /// <summary>
        ///   Tests if the mean (or the parameter under test)
        ///   is significantly greater (larger, bigger) than
        ///   the hypothesized value.
        /// </summary>
        /// 
        ValueIsGreaterThanHypothesis = DistributionTail.OneUpper,

        /// <summary>
        ///   Tests if the mean (or the parameter under test)
        ///   is significantly smaller (lesser) than the
        ///   hypothesized value.
        /// </summary>
        /// 
        ValueIsSmallerThanHypothesis = DistributionTail.OneLower,
    }

    /// <summary>
    ///   Common test Hypothesis for two sample tests, such as
    ///   <see cref="TwoSampleZTest"/> and <see cref="TwoSampleTTest"/>.
    /// </summary>
    /// 
    public enum TwoSampleHypothesis
    {
        /// <summary>
        ///   Tests if the mean (or the parameter under test) of
        ///   the first sample is different from the mean of the 
        ///   second sample, without considering any particular
        ///   direction for the difference.
        /// </summary>
        /// 
        ValuesAreDifferent = DistributionTail.TwoTail,

        /// <summary>
        ///   Tests if the mean (or the parameter under test) of
        ///   the first sample is greater (larger, bigger) than
        ///   the mean of the second sample.
        /// </summary>
        /// 
        FirstValueIsGreaterThanSecond = DistributionTail.OneUpper,

        /// <summary>
        ///   Tests if the mean (or the parameter under test) of
        ///   the first sample is smaller (lesser) than the mean 
        ///   of the second sample.
        /// </summary>
        /// 
        FirstValueIsSmallerThanSecond = DistributionTail.OneLower,
    }

    /// <summary>
    ///   Hypothesis for the one-sample Kolmogorov-Smirnov test.
    /// </summary>
    /// 
    public enum KolmogorovSmirnovTestHypothesis
    {
        /// <summary>
        ///   Tests whether the sample's distribution is
        ///   different from the reference distribution.
        /// </summary>
        /// 
        SampleIsDifferent = DistributionTail.TwoTail,

        /// <summary>
        ///   Tests whether the distribution of one sample is greater
        ///   than the reference distribution, in a statistical sense.
        /// </summary>
        /// 
        SampleIsGreater = DistributionTail.OneUpper,

        /// <summary>
        ///   Tests whether the distribution of one sample is smaller
        ///   than the reference distribution, in a statistical sense.
        /// </summary>
        /// 
        SampleIsSmaller = DistributionTail.OneLower,
    }

    /// <summary>
    ///   Test hypothesis for the two-sample Kolmogorov-Smirnov tests.
    /// </summary>
    /// 
    public enum TwoSampleKolmogorovSmirnovTestHypothesis
    {
        /// <summary>
        ///   Tests whether samples have been drawn 
        ///   from significantly unequal distributions.
        /// </summary>
        /// 
        SamplesDistributionsAreUnequal = DistributionTail.TwoTail,

        /// <summary>
        ///   Tests whether the distribution of one sample is
        ///   greater than the other, in a statistical sense.
        /// </summary>
        /// 
        FirstSampleIsLargerThanSecond = DistributionTail.OneUpper,

        /// <summary>
        ///   Tests whether the distribution of one sample is
        ///   smaller than the other, in a statistical sense.
        /// </summary>
        /// 
        FirstSampleIsSmallerThanSecond = DistributionTail.OneLower,
    }

}
