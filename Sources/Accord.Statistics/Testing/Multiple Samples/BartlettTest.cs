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
    using System;
    using Accord.Compat;

    /// <summary>
    ///   Bartlett's test for equality of variances.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In statistics, Bartlett's test is used to test if k samples are from populations
    ///   with equal variances. Equal variances across samples is called homoscedasticity 
    ///   or homogeneity of variances. Some statistical tests, for example the <see cref="IAnova">
    ///   analysis of variance</see>, assume that variances are equal across groups or samples. 
    ///   The Bartlett test can be used to verify that assumption.</para>
    /// <para>
    ///   Bartlett's test is sensitive to departures from normality. That is, if the samples 
    ///   come from non-normal distributions, then Bartlett's test may simply be testing for 
    ///   non-normality. <see cref="LeveneTest">Levene's test</see> and the Brown–Forsythe test
    ///   are alternatives to the Bartlett test that are less sensitive to departures from 
    ///   normality. </para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Wikipedia, The Free Encyclopedia. Bartlett's test. Available on:
    ///       http://en.wikipedia.org/wiki/Bartlett's_test </description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <seealso cref="LeveneTest"/>
    /// 
    /// <seealso cref="ChiSquareTest"/>
    /// <seealso cref="Accord.Statistics.Distributions.Univariate.ChiSquareDistribution"/>
    /// 
    [Serializable]
    public class BartlettTest : ChiSquareTest
    {

        /// <summary>
        ///   Tests the null hypothesis that all group variances are equal.
        /// </summary>
        /// 
        /// <param name="samples">The grouped samples.</param>
        /// 
        public BartlettTest(params double[][] samples)
        {
            int N = 0, k = samples.Length;
            double Sp = Measures.PooledVariance(samples);

            double sum1 = 0; // Numerator sum
            for (int i = 0; i < samples.Length; i++)
            {
                int n = samples[i].Length;
                double s = samples[i].Variance();
                double logs = Math.Log(s);

                sum1 += (n - 1.0) * logs;
                N += n;
            }

            double sum2 = 0; // Denominator sum
            for (int i = 0; i < samples.Length; i++)
            {
                int n = samples[i].Length;
                sum2 += 1.0 / (n - 1) ;
            }


            double num = (N - k) * Math.Log(Sp) - sum1;
            double den = 1 + (1 / (3.0 * (k - 1))) * (sum2 - 1.0 / (N - k));

            double W = num / den;
            int degrees = k - 1;

            this.Compute(W, degrees);
        }

    }
}
