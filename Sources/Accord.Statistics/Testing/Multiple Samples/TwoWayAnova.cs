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
    ///   Two-way ANOVA model types.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Analysis_of_variance">
    ///       Wikipedia, The Free Encyclopedia. Analysis of variance. </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    public enum TwoWayAnovaModel
    {
        /// <summary>
        ///   Fixed-effects model (Model 1).
        /// </summary>
        /// <remarks>
        ///  <para>
        ///   The fixed-effects model of analysis of variance, as known as model 1, applies
        ///   to situations in which the experimenter applies one or more treatments to the
        ///   subjects of the experiment to see if the response variable values change.</para>
        ///  <para>
        ///   This allows the experimenter to estimate the ranges of response variable values
        ///   that the treatment would generate in the population as a whole.</para>  
        ///   
        ///  <para>
        ///   References:
        ///   <list type="bullet">
        ///     <item><description><a href="http://en.wikipedia.org/wiki/Analysis_of_variance">
        ///       Wikipedia, The Free Encyclopedia. Analysis of variance. </a></description></item>
        ///   </list></para>
        /// </remarks>
        /// 
        Fixed = 1,

        /// <summary>
        ///   Random-effects model (Model 2).
        /// </summary>
        /// <remarks>
        ///  <para>
        ///   Random effects models are used when the treatments are not fixed. This occurs when
        ///   the various factor levels are sampled from a larger population. Because the levels 
        ///   themselves are random variables, some assumptions and the method of contrasting the
        ///   treatments differ from ANOVA model 1.</para>
        ///   
        ///  <para>
        ///   References:
        ///   <list type="bullet">
        ///     <item><description><a href="http://en.wikipedia.org/wiki/Analysis_of_variance">
        ///       Wikipedia, The Free Encyclopedia. Analysis of variance. </a></description></item>
        ///   </list></para>
        /// </remarks>
        /// 
        Random = 2,

        /// <summary>
        ///   Mixed-effects models (Model 3).
        /// </summary>
        /// <remarks>
        ///  <para>
        ///   A mixed-effects model contains experimental factors of both fixed and random-effects 
        ///   types, with appropriately different interpretations and analysis for the two types.</para>
        ///   
        ///  <para>
        ///   References:
        ///   <list type="bullet">
        ///     <item><description><a href="http://en.wikipedia.org/wiki/Analysis_of_variance">
        ///       Wikipedia, The Free Encyclopedia. Analysis of variance. </a></description></item>
        ///   </list></para>
        /// </remarks>
        /// 
        Mixed = 3,
    }


    /// <summary>
    ///   Two-way Analysis of Variance.
    /// </summary>
    /// 
    /// <remarks>
    ///  <para>
    ///   The two-way ANOVA is an extension of the one-way ANOVA for two independent
    ///   variables. There are three classes of models which can also be used in the
    ///   analysis, each of which determining the interpretation of the independent
    ///   variables in the analysis.</para>
    /// 
    ///  <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Analysis_of_variance">
    ///       Wikipedia, The Free Encyclopedia. Analysis of variance. </a></description></item>
    ///     <item><description><a href="http://www.smi.hst.aau.dk/~cdahl/BiostatPhD/ANOVA.pdf">
    ///       Carsten Dahl Mørch, ANOVA. Aalborg Universitet. Available on:
    ///       http://www.smi.hst.aau.dk/~cdahl/BiostatPhD/ANOVA.pdf </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <see cref="TwoWayAnovaModel"/>
    /// 
    [Serializable]
    public class TwoWayAnova : IAnova
    {

        private double totalMean;
        private double[,] cellMeans;
        private double[] aMean;
        private double[] bMean;

        /// <summary>
        ///   Gets the number of observations in the sample.
        /// </summary>
        /// 
        public int Observations { get; private set; }

        /// <summary>
        ///   Gets the number of samples presenting the first factor.
        /// </summary>
        /// 
        public int FirstFactorSamples { get; private set; }

        /// <summary>
        ///   Gets the number of samples presenting the second factor.
        /// </summary>
        public int SecondFactorSamples { get; private set; }

        /// <summary>
        ///   Gets the number of replications of each factor.
        /// </summary>
        /// 
        public int Replications { get; private set; }



        /// <summary>
        ///   Gets or sets the variation sources obtained in the analysis.
        /// </summary>
        /// <value>The variation sources for the data.</value>
        /// 
        public TwoWayAnovaVariationSources Sources { get; private set; }

        /// <summary>
        ///   Gets the ANOVA results in the form of a table.
        /// </summary>
        /// 
        public AnovaSourceCollection Table { get; private set; }

        /// <summary>
        ///   Gets or sets the type of the model.
        /// </summary>
        /// <value>The type of the model.</value>
        /// 
        public TwoWayAnovaModel ModelType { get; private set; }


        /// <summary>
        ///   Constructs a new <see cref="TwoWayAnova"/>.
        /// </summary>
        /// 
        /// <param name="samples">The samples.</param>
        /// <param name="firstFactorLabels">The first factor labels.</param>
        /// <param name="secondFactorLabels">The second factor labels.</param>
        /// <param name="type">The type of the analysis.</param>
        /// 
        public TwoWayAnova(double[] samples, int[] firstFactorLabels, int[] secondFactorLabels,
            TwoWayAnovaModel type = TwoWayAnovaModel.Mixed)
        {
            FirstFactorSamples = firstFactorLabels.Max() + 1;
            SecondFactorSamples = secondFactorLabels.Max() + 1;

            List<double>[][] groups = new List<double>[FirstFactorSamples][];
            for (int i = 0; i < groups.Length; i++)
            {
                groups[i] = new List<double>[SecondFactorSamples];
                for (int j = 0; j < groups[i].Length; j++)
                    groups[i][j] = new List<double>();
            }

            for (int i = 0; i < samples.Length; i++)
            {
                int f = firstFactorLabels[i];
                int s = secondFactorLabels[i];

                groups[f][s].Add(samples[i]);
            }

            // Transform into array
            double[][][] sets = new double[FirstFactorSamples][][];
            for (int i = 0; i < sets.Length; i++)
            {
                sets[i] = new double[SecondFactorSamples][];
                for (int j = 0; j < sets[i].Length; j++)
                    sets[i][j] = groups[i][j].ToArray();
            }

            Replications = sets[0][0].Length;

            // Assert equal number of replicates
            for (int i = 0; i < sets.Length; i++)
                for (int j = 0; j < sets[i].Length; j++)
                    if (sets[i][j].Length != Replications)
                        throw new ArgumentException("Samples do not have the same number of replicates.", "samples");

            initialize(sets, type);
        }

        /// <summary>
        ///   Constructs a new <see cref="TwoWayAnova"/>.
        /// </summary>
        /// 
        /// <param name="samples">The samples in grouped form.</param>
        /// <param name="type">The type of the analysis.</param>
        /// 
        public TwoWayAnova(double[][][] samples, TwoWayAnovaModel type = TwoWayAnovaModel.Mixed)
        {
            FirstFactorSamples = samples.Length;
            SecondFactorSamples = samples[0].Length;
            Replications = samples[0][0].Length;
            Observations = FirstFactorSamples * SecondFactorSamples * Replications;

            // Assert equal number of replicates
            for (int i = 0; i < samples.Length; i++)
                for (int j = 0; j < samples[i].Length; j++)
                    if (samples[i][j].Length != Replications)
                        throw new ArgumentException("Samples do not have the same number of replicates.", "samples");

            initialize(samples, type);
        }


        private void initialize(double[][][] samples, TwoWayAnovaModel type)
        {
            // References:
            // -  http://www.smi.hst.aau.dk/~cdahl/BiostatPhD/ANOVA.pdf

            ModelType = type;
            Observations = FirstFactorSamples * SecondFactorSamples * Replications;


            // Step 1. Initialize all degrees of freedom
            int cellDegreesOfFreedom = FirstFactorSamples * SecondFactorSamples - 1;
            int aDegreesOfFreedom = FirstFactorSamples - 1;
            int bDegreesOfFreedom = SecondFactorSamples - 1;
            int abDegreesOfFreedom = cellDegreesOfFreedom - aDegreesOfFreedom - bDegreesOfFreedom;
            int errorDegreesOfFreedom = FirstFactorSamples * SecondFactorSamples * (Replications - 1);
            int totalDegreesOfFreedom = Observations - 1;


            // Step 1. Calculate cell means
            cellMeans = new double[FirstFactorSamples, SecondFactorSamples];

            double sum = 0;
            for (int i = 0; i < samples.Length; i++)
                for (int j = 0; j < samples[i].Length; j++)
                    sum += cellMeans[i, j] = Statistics.Tools.Mean(samples[i][j]);


            // Step 2. Calculate the total mean (grand mean)
            totalMean = sum / (FirstFactorSamples * SecondFactorSamples);


            // Step 3. Calculate factor means
            aMean = new double[FirstFactorSamples];
            for (int i = 0; i < samples.Length; i++)
            {
                sum = 0;
                for (int j = 0; j < samples[i].Length; j++)
                    for (int k = 0; k < samples[i][j].Length; k++)
                        sum += samples[i][j][k];

                aMean[i] = sum / (SecondFactorSamples * Replications);
            }

            bMean = new double[SecondFactorSamples];
            for (int j = 0; j < samples[0].Length; j++)
            {
                sum = 0;
                for (int i = 0; i < samples.Length; i++)
                    for (int k = 0; k < samples[i][j].Length; k++)
                        sum += samples[i][j][k];

                bMean[j] = sum / (FirstFactorSamples * Replications);
            }


            // Step 4. Calculate total sum of squares
            double ssum = 0;
            for (int i = 0; i < samples.Length; i++)
            {
                for (int j = 0; j < samples[i].Length; j++)
                {
                    for (int k = 0; k < samples[i][j].Length; k++)
                    {
                        double u = samples[i][j][k] - totalMean;
                        ssum += u * u;
                    }
                }
            }
            double totalSumOfSquares = ssum;


            // Step 5. Calculate the cell sum of squares
            ssum = 0;
            for (int i = 0; i < FirstFactorSamples; i++)
            {
                for (int j = 0; j < SecondFactorSamples; j++)
                {
                    double u = cellMeans[i, j] - totalMean;
                    ssum += u * u;
                }
            }
            double cellSumOfSquares = ssum * Replications;


            // Step 6. Compute within-cells error sum of squares
            ssum = 0;
            for (int i = 0; i < samples.Length; i++)
            {
                for (int j = 0; j < samples[i].Length; j++)
                {
                    for (int k = 0; k < samples[i][j].Length; k++)
                    {
                        double u = samples[i][j][k] - cellMeans[i, j];
                        ssum += u * u;
                    }
                }
            }
            double errorSumOfSquares = ssum;


            // Step 7. Compute factors sum of squares
            ssum = 0;
            for (int i = 0; i < aMean.Length; i++)
            {
                double u = aMean[i] - totalMean;
                ssum += u * u;
            }
            double aSumOfSquares = ssum * SecondFactorSamples * Replications;

            ssum = 0;
            for (int i = 0; i < bMean.Length; i++)
            {
                double u = bMean[i] - totalMean;
                ssum += u * u;
            }
            double bSumOfSquares = ssum * FirstFactorSamples * Replications;


            // Step 9. Compute interaction sum of squares
            double abSumOfSquares = cellSumOfSquares - aSumOfSquares - bSumOfSquares;

            // Step 10. Compute mean squares
            double aMeanSquares = aSumOfSquares / aDegreesOfFreedom;
            double bMeanSquares = bSumOfSquares / bDegreesOfFreedom;
            double abMeanSquares = abSumOfSquares / abDegreesOfFreedom;
            double errorMeanSquares = errorSumOfSquares / errorDegreesOfFreedom;

            // Step 10. Create the F-Statistics
            FTest aSignificance, bSignificance, abSignificance;

            if (type == TwoWayAnovaModel.Fixed)
            {
                // Model 1: Factors A and B fixed
                aSignificance = new FTest(aMeanSquares / abMeanSquares, aDegreesOfFreedom, abDegreesOfFreedom);
                bSignificance = new FTest(bMeanSquares / abMeanSquares, bDegreesOfFreedom, abDegreesOfFreedom);
                abSignificance = new FTest(abMeanSquares / errorMeanSquares, abDegreesOfFreedom, errorDegreesOfFreedom);
            }
            else if (type == TwoWayAnovaModel.Mixed)
            {
                // Model 2: Factors A and B random
                aSignificance = new FTest(aMeanSquares / errorMeanSquares, aDegreesOfFreedom, errorDegreesOfFreedom);
                bSignificance = new FTest(bMeanSquares / errorMeanSquares, bDegreesOfFreedom, errorDegreesOfFreedom);
                abSignificance = new FTest(abMeanSquares / errorMeanSquares, abDegreesOfFreedom, errorDegreesOfFreedom);
            }
            else if (type == TwoWayAnovaModel.Random)
            {
                // Model 3: Factor A fixed, factor B random
                aSignificance = new FTest(aMeanSquares / abMeanSquares, aDegreesOfFreedom, abDegreesOfFreedom);
                bSignificance = new FTest(bMeanSquares / errorMeanSquares, bDegreesOfFreedom, errorDegreesOfFreedom);
                abSignificance = new FTest(abMeanSquares / errorMeanSquares, abDegreesOfFreedom, errorDegreesOfFreedom);
            }
            else throw new ArgumentException("Unhandled analysis type.","type");


            // Step 11. Create the ANOVA table and sources
            AnovaVariationSource cell  = new AnovaVariationSource(this, "Cells", cellSumOfSquares, cellDegreesOfFreedom);
            AnovaVariationSource a     = new AnovaVariationSource(this, "Factor A", aSumOfSquares, aDegreesOfFreedom, aMeanSquares, aSignificance);
            AnovaVariationSource b     = new AnovaVariationSource(this, "Factor B", bSumOfSquares, bDegreesOfFreedom, bMeanSquares, bSignificance);
            AnovaVariationSource ab    = new AnovaVariationSource(this, "Interaction AxB", abSumOfSquares, abDegreesOfFreedom, abMeanSquares, abSignificance);
            AnovaVariationSource error = new AnovaVariationSource(this, "Within-cells (error)", errorSumOfSquares, errorDegreesOfFreedom, errorMeanSquares);
            AnovaVariationSource total = new AnovaVariationSource(this, "Total", totalSumOfSquares, totalDegreesOfFreedom);

            this.Sources = new TwoWayAnovaVariationSources()
            {
                Cells = cell,
                FactorA = a,
                FactorB = b,
                Interaction = ab,
                Error = error,
                Total = total
            };

            this.Table = new AnovaSourceCollection(cell, a, b, ab, error, total);
        }
    }

    /// <summary>
    ///   Variation sources associated with two-way ANOVA.
    /// </summary>
    /// 
    public class TwoWayAnovaVariationSources
    {
        internal TwoWayAnovaVariationSources() { }

        /// <summary>
        ///   Gets information about the first factor (A).
        /// </summary>
        /// 
        public AnovaVariationSource FactorA { get; internal set; }

        /// <summary>
        ///   Gets information about the second factor (B) source.
        /// </summary>
        /// 
        public AnovaVariationSource FactorB { get; internal set; }

        /// <summary>
        ///   Gets information about the interaction factor (AxB) source.
        /// </summary>
        /// 
        public AnovaVariationSource Interaction { get; internal set; }

        /// <summary>
        ///   Gets information about the error (within-variance) source.
        /// </summary>
        /// 
        public AnovaVariationSource Error { get; internal set; }

        /// <summary>
        ///   Gets information about the grouped (cells) variance source.
        /// </summary>
        /// 
        public AnovaVariationSource Cells { get; internal set; }

        /// <summary>
        ///   Gets information about the total source of variance.
        /// </summary>
        /// 
        public AnovaVariationSource Total { get; internal set; }
    }
}
