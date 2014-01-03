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
    using System.Collections.Generic;
    using Accord.Math;

    /// <summary>
    ///   One-way Analysis of Variance (ANOVA).
    /// </summary>
    /// <remarks>
    /// <para>
    ///   The one-way ANOVA is a way to test for the equality of three or more means at the same
    ///   time by using variances. In its simplest form ANOVA provides a statistical test of whether
    ///   or not the means of several groups are all equal, and therefore generalizes t-test to more 
    ///   than two groups.</para>
    /// 
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Analysis_of_variance">
    ///       Wikipedia, The Free Encyclopedia. Analysis of variance. </a></description></item>
    ///     <item><description><a href="http://en.wikipedia.org/wiki/F_test">
    ///       Wikipedia, The Free Encyclopedia. F-Test. </a></description></item>
    ///     <item><description><a href="http://en.wikipedia.org/wiki/One-way_ANOVA">
    ///       Wikipedia, The Free Encyclopedia. One-way ANOVA. </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    /// The following is the same example given in Wikipedia's page for the
    /// F-Test [1]. Suppose one would like to test the effect of three levels
    /// of a fertilizer on plant growth. </para>
    /// 
    /// <para>
    /// To achieve this goal, an experimenter has divided a set of 18 plants on
    /// three groups, 6 plants each. Each group has received different levels of
    /// the fertilizer under question.</para>
    /// 
    /// <para>
    /// After some months, the experimenter registers the growth for each plant: </para>
    /// 
    /// <code>
    /// double[][] samples =
    /// {
    ///     new double[] {  6,  8,  4,  5,  3,  4 }, // records for the first group
    ///     new double[] {  8, 12,  9, 11,  6,  8 }, // records for the second group
    ///     new double[] { 13,  9, 11,  8,  7, 12 }, // records for the third group
    /// };
    /// </code>
    /// 
    /// <para>
    /// Now, he would like to test whether the different fertilizer levels has
    /// indeed caused any effect in plant growth. In other words, he would like
    /// to test if the three groups are indeed significantly different.</para>
    /// 
    /// <code>
    /// // To do it, he runs an ANOVA test:
    /// OneWayAnova anova = new OneWayAnova(samples);
    /// </code>
    /// 
    /// <para>
    /// After the Anova object has been created, one can display its findings
    /// in the form of a standard ANOVA table by binding anova.Table to a 
    /// DataGridView or any other display object supporting data binding. To
    /// illustrate, we could use Accord.NET's DataGridBox to inspect the
    /// table's contents.</para>
    /// 
    /// <code>
    ///   DataGridBox.Show(anova.Table);
    /// </code>
    /// 
    /// <para>Result in:</para>
    /// 
    /// <img src="../images/one-way-anova.png"/>
    /// 
    /// <para>
    ///  The p-level for the analysis is about 0.002, meaning the test is
    ///  significant at the 5% significance level. The experimenter would
    ///  thus reject the null hypothesis, concluding there is a strong
    ///  evidence that the three groups are indeed different. Assuming the
    ///  experiment was correctly controlled, this would be an indication
    ///  that the fertilizer does indeed affect plant growth.</para>
    /// 
    /// <para>
    ///   [1] http://en.wikipedia.org/wiki/F_test </para>
    ///   
    /// </example>
    /// 
    [Serializable]
    public class OneWayAnova : IAnova
    {

        private int groupCount;

        private int[] sizes;
        private int totalSize;

        private double[] means;
        private double totalMean;

        private double SSb; // Between-group sum of squares
        private double SSw; // Within-group sum of squares
        private double SSt; // Total sum of squares

        private double MSb; // Between-group mean square
        private double MSw; // Within-group mean square

        private int DFb; // Between-group degrees-of-freedom
        private int DFw; // Within-group degrees-of-freedom
        private int DFt; // Total degrees-of-freedom

        /// <summary>
        ///   Gets the F-Test produced by this one-way ANOVA.
        /// </summary>
        /// 
        public FTest FTest { get; private set; }

        /// <summary>
        ///   Gets the ANOVA results in the form of a table.
        /// </summary>
        /// 
        public AnovaSourceCollection Table { get; private set; }


        /// <summary>
        ///   Creates a new one-way ANOVA test.
        /// </summary>
        /// 
        /// <param name="samples">The sampled values.</param>
        /// <param name="labels">The independent, nominal variables.</param>
        /// 
        public OneWayAnova(double[] samples, int[] labels)
        {
            totalSize = samples.Length;
            groupCount = labels.Max();

            sizes = new int[groupCount];

            double[][] groups = new double[groupCount][];
            for (int i = 0; i < groups.Length; i++)
            {
                int[] idx = labels.Find(label => label == i);
                double[] group = samples.Submatrix(idx);

                groups[i] = group;
                sizes[i] = group.Length;
            }

            initialize(groups);
        }

        /// <summary>
        ///   Creates a new one-way ANOVA test.
        /// </summary>
        /// 
        /// <param name="samples">The grouped sampled values.</param>
        ///
        public OneWayAnova(params double[][] samples)
        {
            sizes = new int[samples.Length];

            groupCount = samples.Length;
            for (int i = 0; i < samples.Length; i++)
                totalSize += sizes[i] = samples[i].Length;

            initialize(samples);
        }

        private void initialize(double[][] samples)
        {
            DFb = groupCount - 1;
            DFw = totalSize - groupCount;
            DFt = totalSize - 1;

            // Step 1. Calculate the mean within each group
            means = Statistics.Tools.Mean(samples, 1);

            // Step 2. Calculate the overall mean
            totalMean = Statistics.Tools.GrandMean(means, sizes);


            // Step 3. Calculate the "between-group" sum of squares
            for (int i = 0; i < samples.Length; i++)
            {
                //  between-group sum of squares
                double u = (means[i] - totalMean);
                SSb += sizes[i] * u * u;
            }


            // Step 4. Calculate the "within-group" sum of squares
            for (int i = 0; i < samples.Length; i++)
            {
                for (int j = 0; j < samples[i].Length; j++)
                {
                    double u = samples[i][j] - means[i];
                    SSw += u * u;
                }
            }

            SSt = SSb + SSw; // total sum of squares


            // Step 5. Calculate the F statistic
            MSb = SSb / DFb; // between-group mean square
            MSw = SSw / DFw; // within-group mean square
            FTest = new FTest(MSb / MSw, DFb, DFw);


            // Step 6. Create the ANOVA table
            List<AnovaVariationSource> table = new List<AnovaVariationSource>();
            table.Add(new AnovaVariationSource(this, "Between-Groups", SSb, DFb, FTest));
            table.Add(new AnovaVariationSource(this, "Within-Groups", SSw, DFw, null));
            table.Add(new AnovaVariationSource(this, "Total", SSt, DFt, null));
            this.Table = new AnovaSourceCollection(table);
        }

    }
}
