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

namespace Accord.Statistics.Testing
{
    using Accord.Statistics.Analysis;
    using Accord.Statistics.Testing.Power;
    using System.Runtime.CompilerServices;

    /// <summary>
    ///  Contains 34+ statistical hypothesis tests, including <see cref="OneWayAnova">one way</see>
    ///  and <see cref="TwoWayAnova">two-way ANOVA tests</see>, non-parametric tests such as the 
    ///  <see cref="KolmogorovSmirnovTest">Kolmogorov-Smirnov test</see> and the <see cref="SignTest">
    ///  Sign Test for the Median</see>, <see cref="GeneralConfusionMatrix">contingency table</see>
    ///  tests such as the <see cref="KappaTest">Kappa test</see>, including variations for 
    ///  <see cref="AverageKappaTest">multiple tables</see>, as well as the <see cref="BhapkarTest">
    ///  Bhapkar</see> and <see cref="BowkerTest">Bowker</see> tests; and the more traditional
    ///  <see cref="ChiSquareTest">Chi-Square</see>, <see cref="ZTest">Z</see>, <see cref="FTest">F
    ///  </see>, <see cref="TTest">T</see> and <see cref="WaldTest">Wald tests</see>.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This namespace contains a suite of parametric and non-parametric hypothesis tests. Every
    ///   test in this library implements the <see cref="IHypothesisTest"/> interface, which defines
    ///   a few <see cref="IHypothesisTest.Significant">key methods and properties to assert whether
    ///   an statistical hypothesis can be supported or not</see>. Every hypothesis test is associated
    ///   with an <see cref="HypothesisTest{T}.StatisticDistribution">statistic distribution</see>
    ///   which can in turn be queried, inspected and computed as any other distribution in the 
    ///   <see cref="Accord.Statistics.Distributions"/>namespace.</para>
    ///   
    /// <para>
    ///   By default, tests are created using a 0.05 <see cref="HypothesisTest{T}.Size">significance level
    ///   </see>, which in the framework is referred as the test's size. P-Values are also ready to be
    ///   inspected by checking a test's <see cref="HypothesisTest{T}.PValue">P-Value</see> property.</para>
    ///   
    /// <para>
    ///   Furthermore, several tests in this namespace also support <see cref="Accord.Statistics.Testing.Power">
    ///   power analysis</see>. The power analysis of a test can be used to suggest an optimal number of samples
    ///   which have to be obtained in order to achieve a more interpretable or useful result while doing hypothesis
    ///   testing. Power analyses implement the <see cref="IPowerAnalysis"/> interface, and analyses are available 
    ///   for the one sample <see cref="ZTestPowerAnalysis">Z</see>, and <see cref="TTestPowerAnalysis">T</see> tests,
    ///   as well as their two sample versions.</para>
    ///   
    /// <para>
    ///   Some useful parametric tests are the <see cref="BinomialTest"/>, <see cref="ChiSquareTest"/>,
    ///   <see cref="FTest"/>, <see cref="MultinomialTest"/>, <see cref="TTest"/>, <see cref="WaldTest"/>
    ///   and <see cref="ZTest"/>. Useful non-parametric tests include the <see cref="KolmogorovSmirnovTest"/>,
    ///   <see cref="SignTest"/>, <see cref="WilcoxonSignedRankTest"/> and the <see cref="WilcoxonTest"/>.</para>
    ///   
    /// <para>
    ///   Tests are also available for two or more samples. In this case, we can find two sample variants for the
    ///   <see cref="PairedTTest"/>, <see cref="TwoProportionZTest"/>, <see cref="TwoSampleKolmogorovSmirnovTest"/>, 
    ///   <see cref="TwoSampleSignTest"/>, <see cref="TwoSampleTTest"/>, <see cref="TwoSampleWilcoxonSignedRankTest"/>,
    ///   <see cref="TwoSampleZTest"/>, as well as the <see cref="MannWhitneyWilcoxonTest"/> for unpaired samples. For
    ///   multiple samples we can find the <see cref="OneWayAnova"/> and <see cref="TwoWayAnova"/>, as well as the
    ///   <see cref="LeveneTest"/> and <see cref="BartlettTest"/>.</para>
    ///   
    /// <para>
    ///   Finally, the namespace also includes several tests for <see cref="ConfusionMatrix">contingency tables</see>.
    ///   Those tests include <see cref="KappaTest">Kappa test for inter-rater agreement</see> and its variants, such
    ///   as the <see cref="AverageKappaTest"/>, <see cref="TwoAverageKappaTest"/> and <see cref="TwoMatrixKappaTest"/>.
    ///   Other tests include <see cref="BhapkarTest"/>, <see cref="McNemarTest"/>, <see cref="ReceiverOperatingCurveTest"/>,
    ///   <see cref="StuartMaxwellTest"/>, and the <see cref="TwoReceiverOperatingCurveTest"/>.</para>
    ///   
    /// <para>
    ///   The namespace class diagram is shown below. </para>
    ///   <img src="..\diagrams\classes\Accord.Statistics.Testing.png" />
    ///   
    /// <para>
    ///   Please note that class diagrams for each of the inner namespaces are 
    ///   also available within their own documentation pages.</para>
    /// </remarks>
    /// 
    /// <seealso cref="Accord.Statistics.Distributions"/>
    /// <seealso cref="Accord.Statistics.Testing.Power"/>
    /// 
    [CompilerGenerated]
    class NamespaceDoc
    {
    }
}
