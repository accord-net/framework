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
    using AForge;

    /// <summary>
    ///   Wald's Test using the Normal distribution.
    /// </summary>
    /// 
    /// <remarks>
    ///  <para>
    ///   The Wald test is a parametric statistical test named after Abraham Wald
    ///   with a great variety of uses. Whenever a relationship within or between
    ///   data items can be expressed as a statistical model with parameters to be
    ///   estimated from a sample, the Wald test can be used to test the true value
    ///   of the parameter based on the sample estimate.</para>
    ///   
    /// <para>
    ///   Under the Wald statistical test, the maximum likelihood estimate of the
    ///   parameter(s) of interest θ is compared with the proposed value θ', with
    ///   the assumption that the difference between the two will be approximately
    ///   normal.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Wald_test">
    ///        Wikipedia, The Free Encyclopedia. Wald Test. Available on:
    ///        http://en.wikipedia.org/wiki/Wald_test </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <seealso cref="ZTest"/>
    /// 
    [Serializable]
    public class WaldTest : ZTest
    {

        /// <summary>
        ///   Constructs a Wald's test.
        /// </summary>
        /// 
        /// <param name="statistic">The test statistic, as given by (θ-θ')/SE.</param>
        /// 
        public WaldTest(double statistic)
            : base(statistic, OneSampleHypothesis.ValueIsDifferentFromHypothesis) { }

        /// <summary>
        ///   Constructs a Wald's test.
        /// </summary>
        /// 
        /// <param name="estimatedValue">The estimated value (θ).</param>
        /// <param name="hypothesizedValue">The hypothesized value (θ').</param>
        /// <param name="standardError">The standard error of the estimation (SE).</param>
        /// 
        public WaldTest(double estimatedValue, double hypothesizedValue, double standardError)
            : base((estimatedValue - hypothesizedValue) / standardError, OneSampleHypothesis.ValueIsDifferentFromHypothesis)
        {
            this.HypothesizedValue = hypothesizedValue;
            this.EstimatedValue = estimatedValue;
            this.StandardError = standardError;
        }
        
    }
}
