// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Diego Catalano, 2013
// diego.catalano at live.com
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

namespace Accord.Imaging
{
    using System;
    using Accord.Math;
    using Accord.Statistics;
    using System.Collections.Generic;
    using Accord.Math.Decompositions;
    using AForge;

    /// <summary>
    ///   Haralick's Texture Features.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   Haralick's texture features are based on measures derived from
    ///   <see cref="GrayLevelCooccurrenceMatrix">Gray-level Co-occurrence 
    ///   matrices (GLCM)</see>.</para>
    /// <para>
    ///   Whether considering the intensity or grayscale values of the image 
    ///   or various dimensions of color, the co-occurrence matrix can measure
    ///   the texture of the image. Because co-occurrence matrices are typically
    ///   large and sparse, various metrics of the matrix are often taken to get
    ///   a more useful set of features. Features generated using this technique
    ///   are usually called Haralick features, after R. M. Haralick, attributed to
    ///   his paper Textural features for image classification (1973).</para>
    ///   
    /// <para>
    ///   This class encompasses most of the features derived on Haralick's original
    ///   paper. All features are lazy-evaluated until needed; but may also be
    ///   combined in a single feature vector by calling <see cref="GetVector(int)"/>.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Wikipedia Contributors, "Co-occurrence matrix". Available at
    ///       http://en.wikipedia.org/wiki/Co-occurrence_matrix </description></item>
    ///     <item><description>
    ///       Robert M Haralick, K Shanmugam, Its'hak Dinstein; "Textural 
    ///       Features for Image Classification". IEEE Transactions on Systems, Man,
    ///       and Cybernetics. SMC-3 (6): 610–621, 1973. Available at:
    ///       <a href="http://www.makseq.com/materials/lib/Articles-Books/Filters/Texture/Co-occurrence/haralick73.pdf">
    ///       http://www.makseq.com/materials/lib/Articles-Books/Filters/Texture/Co-occurrence/haralick73.pdf </a>
    ///       </description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <seealso cref="GrayLevelCooccurrenceMatrix"/>
    /// <seealso cref="Haralick"/>
    /// 
    [Serializable]
    public class HaralickDescriptor
    {
        double[,] matrix;


        int size; // Ng, number of gray levels

        double[] px; // row marginal
        double[] py; // col marginal

        double? sum;
        double? mean;  // μ (mu), mean of matrix
        double? xmean; // μ_x (mu x), mean of px
        double? ymean; // μ_y (mu y), mean of py
        double? xdev;  // σ_x (sigma y), standard deviation of py
        double? ydev;  // σ_y (sigma x), standard deviation of px
        double? xentropy;    // H_x, entropy of px
        double? yentropy;    // H_y, entropy of py

        double[] xysum;  // p_x+y
        double[] xydiff; // p_x-y

        double? angular;       // f1: energy / angular second moment
        double? contrast;      // f2: contrast
        double? correlation;   // f3: correlation
        double? variance;      // f4: sum of squares: variance
        double? inverse;       // f5: inverse difference moment
        double? sumAverage;    // f6: sum average
        double? sumVariance;   // f7: sum variance
        double? sumEntropy;    // f8: sum entropy
        double? entropy;       // f9: matrix entropy
        double? diffVariance;  // f10: difference variance
        double? diffEntropy;   // f11: difference entropy
        double? information1;  // f12: first information measure of correlation
        double? information2;  // f13: second information measure of correlation
        double? maximal;       // f14: maximal correlation coefficient

        // bonus features
        double? laplace;  // contrast using absolute value instead of square
        double? textureHomogeneity;
        double? clusterShade;
        double? clusterProminence;

        const double epsilon = Constants.DoubleSmall;


        /// <summary>
        ///   Initializes a new instance of the <see cref="HaralickDescriptor"/> class.
        /// </summary>
        /// 
        /// <param name="cooccurrenceMatrix">The co-occurrence matrix to compute features from.</param>
        /// 
        public HaralickDescriptor(double[,] cooccurrenceMatrix)
        {
            if (cooccurrenceMatrix == null)
                throw new ArgumentNullException("cooccurrenceMatrix");
            if (cooccurrenceMatrix.GetLength(0) != cooccurrenceMatrix.GetLength(1))
                throw new DimensionMismatchException("cooccurrenceMatrix", "Matrix must be square");

            this.matrix = cooccurrenceMatrix;
            this.size = cooccurrenceMatrix.GetLength(0);
        }


        #region Common calculations

        /// <summary>
        ///   Gets the number of gray levels in the 
        ///   original image. This is the number of
        ///   dimensions of the co-occurrence matrix.
        /// </summary>
        /// 
        public int GrayLevels { get { return size; } }

        /// <summary>
        ///   Gets the matrix sum.
        /// </summary>
        /// 
        public double Sum
        {
            get
            {
                if (sum == null)
                {
                    double s = 0;
                    foreach (double v in matrix)
                        s += v;
                    sum = s;
                }
                return sum.Value;
            }
        }

        /// <summary>
        ///   Gets the matrix mean μ.
        /// </summary>
        /// 
        public double Mean
        {
            get
            {
                if (mean == null)
                    mean = Sum / matrix.Length;
                return mean.Value;
            }
        }

        /// <summary>
        ///   Gets the marginal probability vector
        ///   obtained by summing the rows of p(i,j),
        ///   given as p<sub>x</sub>(i) = Σ<sub>j</sub> p(i,j).
        /// </summary>
        /// 
        public double[] RowMarginal
        {
            get
            {
                if (px == null)
                {
                    px = new double[size];
                    for (int i = 0; i < px.Length; i++)
                        for (int j = 0; j < size; j++)
                            px[i] += matrix[i, j];
                }
                return px;
            }
        }

        /// <summary>
        ///   Gets the marginal probability vector
        ///   obtained by summing the columns of p(i,j),
        ///   given as p<sub>y</sub>(j) = Σ<sub>i</sub> p(i,j).
        /// </summary>
        /// 
        public double[] ColumnMarginal
        {
            get
            {
                if (py == null)
                {
                    py = new double[size];
                    for (int j = 0; j < py.Length; j++)
                        for (int i = 0; i < size; i++)
                            py[j] += matrix[i, j];
                }
                return py;
            }
        }

        /// <summary>
        ///   Gets μ<sub>x</sub>, the mean value of the 
        ///   <see cref="RowMarginal"/> vector.
        /// </summary>
        /// 
        public double RowMean
        {
            get
            {
                if (xmean == null)
                    xmean = RowMarginal.Mean();
                return xmean.Value;
            }
        }

        /// <summary>
        ///   Gets μ_y, the mean value of the 
        ///   <see cref="ColumnMarginal"/> vector.
        /// </summary>
        /// 
        public double ColumnMean
        {
            get
            {
                if (ymean == null)
                    ymean = ColumnMarginal.Mean();
                return ymean.Value;
            }
        }

        /// <summary>
        ///   Gets σ<sub>x</sub>, the variance of the 
        ///   <see cref="RowMarginal"/> vector.
        /// </summary>
        /// 
        public double RowStandardDeviation
        {
            get
            {
                if (xdev == null)
                    xdev = RowMarginal.StandardDeviation(RowMean);
                return xdev.Value;
            }
        }

        /// <summary>
        ///   Gets σ<sub>y</sub>, the variance of the 
        ///   <see cref="ColumnMarginal"/> vector.
        /// </summary>
        /// 
        public double ColumnStandardDeviation
        {
            get
            {
                if (ydev == null)
                    ydev = ColumnMarginal.StandardDeviation(ColumnMean);
                return ydev.Value;
            }
        }

        /// <summary>
        ///   Gets H<sub>x</sub>, the entropy of the 
        ///   <see cref="RowMarginal"/> vector.
        /// </summary>
        /// 
        public double RowEntropy
        {
            get
            {
                if (xentropy == null)
                    xentropy = RowMarginal.Entropy(epsilon);
                return xentropy.Value;
            }
        }

        /// <summary>
        ///   Gets H<sub>y</sub>, the entropy of the 
        ///   <see cref="ColumnMarginal"/> vector.
        /// </summary>
        /// 
        public double ColumnEntropy
        {
            get
            {
                if (yentropy == null)
                    yentropy = ColumnMarginal.Entropy(epsilon);
                return yentropy.Value;
            }
        }

        /// <summary>
        ///   Gets p<sub>(x+y)</sub>(k), the sum 
        ///   of elements whose indices sum to k.
        /// </summary>
        /// 
        public double[] Sums
        {
            get
            {
                if (sum == null)
                {
                    xysum = new double[2 * size];
                    for (int i = 0; i < size; i++)
                        for (int j = 0; j < size; j++)
                            xysum[i + j] += matrix[i, j];
                }
                return xysum;
            }
        }

        /// <summary>
        ///   Gets p<sub>(x-y)</sub> (k), the sum of elements 
        ///   whose absolute indices diferences equals to k.
        /// </summary>
        /// 
        public double[] Differences
        {
            get
            {
                if (xydiff == null)
                {
                    xydiff = new double[size];
                    for (int i = 0; i < size; i++)
                        for (int j = 0; j < size; j++)
                            xydiff[Math.Abs(i - j)] += matrix[i, j];
                }
                return xydiff;
            }
        }
        #endregion


        #region Feature Numbers

        /// <summary>
        ///   Gets Haralick's first textural feature,
        ///   the Angular Second Momentum.
        /// </summary>
        /// 
        public double F01 { get { return AngularSecondMomentum; } }

        /// <summary>
        ///   Gets Haralick's second textural feature,
        ///   the Contrast.
        /// </summary>
        /// 
        public double F02 { get { return Contrast; } }

        /// <summary>
        ///   Gets Haralick's third textural feature,
        ///   the Correlation.
        /// </summary>
        /// 
        public double F03 { get { return Correlation; } }

        /// <summary>
        ///   Gets Haralick's fourth textural feature,
        ///   the Sum of Squares: Variance.
        /// </summary>
        /// 
        public double F04 { get { return SumOfSquares; } }

        /// <summary>
        ///   Gets Haralick's fifth textural feature,
        ///   the Inverse Difference Moment.
        /// </summary>
        ///
        public double F05 { get { return InverseDifferenceMoment; } }

        /// <summary>
        ///   Gets Haralick's sixth textural feature,
        ///   the Sum Average.
        /// </summary>
        /// 
        public double F06 { get { return SumAverage; } }

        /// <summary>
        ///   Gets Haralick's seventh textural feature,
        ///   the Sum Variance.
        /// </summary>
        /// 
        public double F07 { get { return SumVariance; } }

        /// <summary>
        ///   Gets Haralick's eighth textural feature,
        ///   the Sum Entropy.
        /// </summary>
        /// 
        public double F08 { get { return SumEntropy; } }

        /// <summary>
        ///   Gets Haralick's ninth textural feature,
        ///   the Entropy.
        /// </summary>
        /// 
        public double F09 { get { return Entropy; } }

        /// <summary>
        ///   Gets Haralick's tenth textural feature,
        ///   the Difference Variance.
        /// </summary>
        /// 
        public double F10 { get { return DifferenceVariance; } }

        /// <summary>
        ///   Gets Haralick's eleventh textural feature,
        ///   the Difference Entropy.
        /// </summary>
        /// 
        public double F11 { get { return DifferenceEntropy; } }

        /// <summary>
        ///   Gets Haralick's twelfth textural feature,
        ///   the First Information Measure.
        /// </summary>
        /// 
        public double F12 { get { return FirstInformationMeasure; } }

        /// <summary>
        ///   Gets Haralick's thirteenth textural feature,
        ///   the Second Information Measure.
        /// </summary>
        /// 
        public double F13 { get { return SecondInformationMeasure; } }

        /// <summary>
        ///   Gets Haralick's fourteenth textural feature,
        ///   the Maximal Correlation Coefficient.
        /// </summary>
        /// 
        public double F14 { get { return MaximalCorrelationCoefficient; } }

        #endregion


        #region Features

        /// <summary>
        ///   Gets Haralick's first textural feature, the
        ///   Angular Second Momentum, also known as Energy
        ///   or Homogeneity.
        /// </summary>
        /// 
        public double AngularSecondMomentum
        {
            get
            {
                if (angular == null)
                {
                    double s = 0;
                    foreach (double v in matrix)
                        s += v * v;
                    angular = s;
                }
                return angular.Value;
            }
        }

        /// <summary>
        ///   Gets a variation of Haralick's second textural feature,
        ///   the Contrast with Absolute values (instead of squares).
        /// </summary>
        /// 
        public double LaplaceContrast
        {
            get
            {
                if (laplace == null)
                {
                    double[] p_xmy = Differences;

                    double s = 0;
                    for (int n = 0; n < p_xmy.Length; n++)
                        s += Math.Abs(n) * p_xmy[n];

                    laplace = s;
                }
                return laplace.Value;
            }
        }

        /// <summary>
        ///   Gets Haralick's second textural feature,
        ///   the Contrast.
        /// </summary>
        /// 
        public double Contrast
        {
            get
            {
                if (contrast == null)
                {
                    double[] p_xmy = Differences;

                    double s = 0;
                    for (int n = 0; n < p_xmy.Length; n++)
                        s += (n * n) * p_xmy[n];

                    contrast = s;
                    System.Diagnostics.Debug.Assert(!Double.IsNaN(contrast.Value));
                }
                return contrast.Value;
            }
        }

        /// <summary>
        ///   Gets Haralick's third textural feature,
        ///   the Correlation.
        /// </summary>
        /// 
        public double Correlation
        {
            get
            {
                if (correlation == null)
                {
                    double mx = RowMean;
                    double my = ColumnMean;
                    double sx = RowStandardDeviation;
                    double sy = ColumnStandardDeviation;
                    correlation = 0;

                    if (sx * sy > 0)
                    {
                        double s = 0;
                        for (int i = 0; i < size; i++)
                            for (int j = 0; j < size; j++)
                                s += (i * j) * matrix[i, j];

                        correlation = (s - mx * my) / (sx * sy);
                    }

                    System.Diagnostics.Debug.Assert(!Double.IsNaN(correlation.Value));
                }

                return correlation.Value;
            }
        }

        /// <summary>
        ///   Gets Haralick's fourth textural feature,
        ///   the Sum of Squares: Variance.
        /// </summary>
        /// 
        public double SumOfSquares
        {
            get
            {
                if (variance == null)
                {
                    double s = 0, mu = Mean;
                    for (int i = 0; i < size; i++)
                        for (int j = 0; j < size; j++)
                            s += (i - mu) * matrix[i, j];

                    variance = sum;
                }
                return variance.Value;
            }
        }

        /// <summary>
        ///   Gets Haralick's fifth textural feature, the Inverse
        ///   Difference Moment, also known as Local Homogeneity.
        ///   Can be regarded as a complement to <see cref="Contrast"/>.
        /// </summary>
        ///
        public double InverseDifferenceMoment
        {
            get
            {
                if (inverse == null)
                {
                    double s = 0;
                    for (int i = 0; i < size; i++)
                        for (int j = 0; j < size; j++)
                            s += matrix[i, j] / (double)(1 + (i - j) * (i - j));
                    inverse = s;
                }
                return inverse.Value;
            }
        }

        /// <summary>
        ///   Gets a variation of Haralick's fifth textural feature,
        ///   the Texture Homogeneity. Can be regarded as a complement
        ///   to <see cref="LaplaceContrast"/>.
        /// </summary>
        /// 
        public double TextureHomogeneity
        {
            get
            {
                if (textureHomogeneity == null)
                {
                    double s = 0;
                    for (int i = 0; i < size; i++)
                        for (int j = 0; j < size; j++)
                            s += matrix[i, j] / (double)(1 + Math.Abs(i - j));
                    textureHomogeneity = s;
                }
                return textureHomogeneity.Value;
            }
        }

        /// <summary>
        ///   Gets Haralick's sixth textural feature,
        ///   the Sum Average.
        /// </summary>
        /// 
        public double SumAverage
        {
            get
            {
                if (sumAverage == null)
                {
                    double[] sums = Sums;
                    double s = 0;
                    for (int i = 0; i < sums.Length; i++)
                        s += i * sums[i];
                    sumAverage = s;

                    System.Diagnostics.Debug.Assert(!Double.IsNaN(sumAverage.Value));
                }
                return sumAverage.Value;
            }
        }

        /// <summary>
        ///   Gets Haralick's seventh textural feature,
        ///   the Sum Variance.
        /// </summary>
        /// 
        public double SumVariance
        {
            get
            {
                if (sumVariance == null)
                {
                    double[] sums = Sums;
                    double f8 = F08;
                    double s = 0;
                    for (int i = 0; i < sums.Length; i++)
                        s += (i - f8) * (i - f8) * sums[i];
                    sumVariance = s;

                    System.Diagnostics.Debug.Assert(!Double.IsNaN(sumVariance.Value));
                }
                return sumVariance.Value;
            }
        }

        /// <summary>
        ///   Gets Haralick's eighth textural feature,
        ///   the Sum Entropy.
        /// </summary>
        /// 
        public double SumEntropy
        {
            get
            {
                if (sumEntropy == null)
                    sumEntropy = Sums.Entropy(epsilon);
                System.Diagnostics.Debug.Assert(!Double.IsNaN(sumEntropy.Value));
                return sumEntropy.Value;
            }
        }

        /// <summary>
        ///   Gets Haralick's ninth textural feature,
        ///   the Entropy.
        /// </summary>
        /// 
        public double Entropy
        {
            get
            {
                if (entropy == null)
                    entropy = matrix.Entropy(epsilon);
                System.Diagnostics.Debug.Assert(!Double.IsNaN(entropy.Value));
                return entropy.Value;
            }
        }

        /// <summary>
        ///   Gets Haralick's tenth textural feature,
        ///   the Difference Variance.
        /// </summary>
        /// 
        public double DifferenceVariance
        {
            get
            {
                if (diffVariance == null)
                    diffVariance = Differences.Variance();
                System.Diagnostics.Debug.Assert(!Double.IsNaN(diffVariance.Value));
                return diffVariance.Value;
            }
        }

        /// <summary>
        ///   Gets Haralick's eleventh textural feature,
        ///   the Difference Entropy.
        /// </summary>
        /// 
        public double DifferenceEntropy
        {
            get
            {
                if (diffEntropy == null)
                    diffEntropy = Differences.Entropy(epsilon);
                System.Diagnostics.Debug.Assert(!Double.IsNaN(diffEntropy.Value));
                return diffEntropy.Value;
            }
        }

        /// <summary>
        ///   Gets Haralick's twelfth textural feature,
        ///   the First Information Measure.
        /// </summary>
        /// 
        public double FirstInformationMeasure
        {
            get
            {
                if (information1 == null)
                {
                    double hxy = Entropy;
                    double hxy1 = 0;

                    double[] px = RowMarginal;
                    double[] py = ColumnMarginal;

                    double hx = RowEntropy;
                    double hy = ColumnEntropy;

                    information1 = 0;

                    if (hx + hy > 0)
                    {
                        for (int i = 0; i < size; i++)
                            for (int j = 0; j < size; j++)
                                hxy1 -= matrix[i, j] * Math.Log(px[i] * py[j] + epsilon);

                        information1 = (hxy - hxy1) / Math.Max(hx, hy);
                    }

                    System.Diagnostics.Debug.Assert(!Double.IsNaN(information1.Value));
                }

                return information1.Value;
            }
        }

        /// <summary>
        ///   Gets Haralick's thirteenth textural feature,
        ///   the Second Information Measure.
        /// </summary>
        /// 
        public double SecondInformationMeasure
        {
            get
            {
                if (information2 == null)
                {
                    double hxy = Entropy;

                    double hxy2 = 0;

                    double[] px = RowMarginal;
                    double[] py = ColumnMarginal;

                    double hx = RowEntropy;
                    double hy = ColumnEntropy;

                    for (int i = 0; i < size; i++)
                        for (int j = 0; j < size; j++)
                            hxy2 -= px[i] * py[j] * Math.Log(px[i] * py[j] + epsilon);

                    information2 = Math.Sqrt(1.0 - Math.Exp(-2 * (hxy2 - hxy)));
                    System.Diagnostics.Debug.Assert(!Double.IsNaN(information2.Value));
                }

                return information2.Value;
            }
        }

        /// <summary>
        ///   Gets Haralick's fourteenth textural feature,
        ///   the Maximal Correlation Coefficient.
        /// </summary>
        /// 
        public double MaximalCorrelationCoefficient
        {
            get
            {
                if (maximal == null)
                {
                    double[] px = RowMarginal;
                    double[] py = ColumnMarginal;

                    double[,] Q = new double[size, size];
                    for (int i = 0; i < size; i++)
                    {
                        for (int j = 0; j < size; j++)
                        {
                            for (int k = 0; k < size; k++)
                            {
                                double num = matrix[i, k] * matrix[j, k];
                                double den = px[i] * py[i];
                                Q[i, j] += num / den;
                            }
                        }
                    }

                    var evd = new EigenvalueDecomposition(Q,
                        assumeSymmetric: false, inPlace: true);

                    maximal = Math.Sqrt(evd.RealEigenvalues[1]);
                }

                return maximal.Value;
            }
        }

        /// <summary>
        ///   Gets the Cluster Shade textural feature.
        /// </summary>
        /// 
        public double ClusterShade
        {
            get
            {
                if (clusterShade == null)
                {
                    double mx = RowMean;
                    double my = ColumnMean;

                    double s = 0;
                    for (int i = 0; i < size; i++)
                    {
                        for (int j = 0; j < size; j++)
                        {
                            double v = (i + j - mx - my);
                            s += (v * v * v) * matrix[i, j];
                        }
                    }

                    clusterShade = s;
                }

                return clusterShade.Value;
            }
        }

        /// <summary>
        ///   Gets the Cluster Prominence textural feature.
        /// </summary>
        /// 
        public double ClusterProminence
        {
            get
            {
                if (clusterProminence == null)
                {
                    double mx = RowMean;
                    double my = ColumnMean;

                    double s = 0;
                    for (int i = 0; i < size; i++)
                    {
                        for (int j = 0; j < size; j++)
                        {
                            double v = (i + j - mx - my);
                            s += (v * v * v * v) * matrix[i, j];
                        }
                    }

                    clusterProminence = s;
                }

                return clusterProminence.Value;
            }
        }

        #endregion


        /// <summary>
        ///   Creates a feature vector with 
        ///   the chosen feature functions.
        /// </summary>
        /// 
        /// <param name="features">How many features to include in the vector. Default is 13.</param>
        /// 
        /// <returns>A vector with Haralick's features up 
        /// to the given number passed as input.</returns>
        /// 
        public double[] GetVector(int features = 13)
        {
            double[] vector = new double[features];

            int i = features;

            switch (features)
            {
                case 01: vector[--i] = F01; break;
                case 02: vector[--i] = F02; goto case 1;
                case 03: vector[--i] = F03; goto case 2;
                case 04: vector[--i] = F04; goto case 3;
                case 05: vector[--i] = F05; goto case 4;
                case 06: vector[--i] = F06; goto case 5;
                case 07: vector[--i] = F07; goto case 6;
                case 08: vector[--i] = F08; goto case 7;
                case 09: vector[--i] = F09; goto case 8;
                case 10: vector[--i] = F10; goto case 9;
                case 11: vector[--i] = F11; goto case 10;
                case 12: vector[--i] = F12; goto case 11;
                case 13: vector[--i] = F13; goto case 12;
                case 14: vector[--i] = F14; goto case 13;

                default:
                    throw new ArgumentOutOfRangeException("features");
            }

            return vector;
        }
    }

    /// <summary>
    ///   Feature dictionary. Associates a set of Haralick features to a given degree
    ///   used to compute the originating <see cref="GrayLevelCooccurrenceMatrix">GLCM</see>.
    /// </summary>
    /// 
    /// <seealso cref="Haralick"/>
    /// <seealso cref="HaralickDescriptor"/>
    /// 
    [Serializable]
    public class HaralickDescriptorDictionary : Dictionary<CooccurrenceDegree, HaralickDescriptor>
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="HaralickDescriptorDictionary"/> class.
        /// </summary>
        /// 
        public HaralickDescriptorDictionary()
        {
        }

        /// <summary>
        ///    Initializes a new instance of the <see cref="HaralickDescriptorDictionary"/>
        ///    class with serialized data.
        /// </summary>
        /// 
        /// <param name="info">A <see cref="System.Runtime.Serialization.SerializationInfo"/>
        ///   object containing the information required to serialize this 
        ///   <see cref="HaralickDescriptorDictionary"/>.</param>
        /// <param name="context">A <see cref="System.Runtime.Serialization.StreamingContext"/>
        ///    structure containing the source and destination of the serialized stream
        ///    associated with this <see cref="HaralickDescriptorDictionary"/>.</param>
        /// 
        protected HaralickDescriptorDictionary(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        ///   Combines features generated from different <see cref="GrayLevelCooccurrenceMatrix">
        ///   GLCMs</see> computed using different <see cref="CooccurrenceDegree">angulations</see>
        ///   by concatenating them into a single vector.
        /// </summary>
        /// 
        /// <param name="features">The number of Haralick's original features to compute.</param>
        /// 
        /// <returns>A single vector containing all values computed from
        /// the different <see cref="HaralickDescriptor"/>s.</returns>
        /// 
        /// <remarks>
        ///   If there are <c>d</c> <see cref="CooccurrenceDegree">degrees</see> in this
        ///   collection, and <c>n</c> <paramref name="features"/> given to compute, the
        ///   generated vector will have size <c>d * n</c>. All features from different
        ///   degrees will be concatenated into this single result vector.
        /// </remarks>
        /// 
        public double[] Combine(int features)
        {
            int count = this.Count;
            double[] haralick = new double[features * count];

            int c = 0;
            foreach (KeyValuePair<CooccurrenceDegree, HaralickDescriptor> pair in this)
            {
                HaralickDescriptor descriptor = pair.Value;
                double[] vector = descriptor.GetVector(features);

                for (int i = 0; i < vector.Length; i++)
                    haralick[c++] = vector[i];
            }

            return haralick;
        }

        /// <summary>
        ///   Combines features generated from different <see cref="GrayLevelCooccurrenceMatrix">
        ///   GLCMs</see> computed using different <see cref="CooccurrenceDegree">angulations</see>
        ///   by averaging them into a single vector.
        /// </summary>
        /// 
        /// <param name="features">The number of Haralick's original features to compute.</param>
        /// 
        /// <returns>A single vector containing the average of the values
        ///   computed from the different <see cref="HaralickDescriptor"/>s.</returns>
        ///   
        /// <remarks>
        ///   If there are <c>d</c> <see cref="CooccurrenceDegree">degrees</see> in this
        ///   collection, and <c>n</c> <paramref name="features"/> given to compute, the
        ///   generated vector will have size <c>n</c>. All features from different
        ///   degrees will be averaged into this single result vector.
        /// </remarks>
        /// 
        public double[] Average(int features)
        {
            double[] haralick = new double[features];

            foreach (KeyValuePair<CooccurrenceDegree, HaralickDescriptor> pair in this)
            {
                HaralickDescriptor descriptor = pair.Value;
                double[] vector = descriptor.GetVector(features);

                for (int i = 0; i < vector.Length; i++)
                    haralick[i] += vector[i];
            }

            int count = this.Count;
            for (int i = 0; i < haralick.Length; i++)
                haralick[i] /= count;

            return haralick;
        }

        /// <summary>
        ///   Combines features generated from different <see cref="GrayLevelCooccurrenceMatrix">
        ///   GLCMs</see> computed using different <see cref="CooccurrenceDegree">angulations</see>
        ///   by averaging them into a single vector.
        /// </summary>
        /// 
        /// <param name="features">The number of Haralick's original features to compute.</param>
        /// 
        /// <returns>A single vector containing the average of the values
        ///   computed from the different <see cref="HaralickDescriptor"/>s.</returns>
        /// 
        /// <remarks>
        ///   If there are <c>d</c> <see cref="CooccurrenceDegree">degrees</see> in this
        ///   collection, and <c>n</c> <paramref name="features"/> given to compute, the
        ///   generated vector will have size <c>2*n*d</c>. Each even index will have
        ///   the average of a given feature, and the subsequent odd index will contain
        ///   the range of this feature.
        /// </remarks>
        /// 
        public double[] AverageWithRange(int features)
        {
            int degrees = this.Count;

            double[][] vectors = new double[features][];
            for (int i = 0; i < vectors.Length; i++)
                vectors[i] = new double[degrees];

            int c = 0;
            foreach (KeyValuePair<CooccurrenceDegree, HaralickDescriptor> pair in this)
            {
                HaralickDescriptor descriptor = pair.Value;
                double[] vector = descriptor.GetVector(features);

                for (int i = 0; i < vector.Length; i++)
                    vectors[i][c] = vector[i];
                c++;
            }

            double[] haralick = new double[features * 2];

            int j = 0;
            for (int i = 0; i < vectors.Length; i++)
            {
                haralick[j++] = vectors[i].Mean();
                haralick[j++] = vectors[i].Range().Length;
            }

            return haralick;
        }

        /// <summary>
        ///   Combines features generated from different <see cref="GrayLevelCooccurrenceMatrix">
        ///   GLCMs</see> computed using different <see cref="CooccurrenceDegree">angulations</see>
        ///   by averaging them into a single vector, normalizing them to be between -1 and 1.
        /// </summary>
        /// 
        /// <param name="features">The number of Haralick's original features to compute.</param>
        /// 
        /// <returns>A single vector containing the averaged and normalized values
        ///   computed from the different <see cref="HaralickDescriptor"/>s.</returns>
        /// 
        /// <remarks>
        ///   If there are <c>d</c> <see cref="CooccurrenceDegree">degrees</see> in this
        ///   collection, and <c>n</c> <paramref name="features"/> given to compute, the
        ///   generated vector will have size <c>n</c>. All features will be averaged, and
        ///   the mean will be scaled to be in a [-1,1] interval.
        /// </remarks>
        /// 
        public double[] Normalize(int features)
        {
            int degrees = this.Count;

            double[][] vectors = new double[features][];
            for (int i = 0; i < vectors.Length; i++)
                vectors[i] = new double[degrees];

            int c = 0;
            foreach (KeyValuePair<CooccurrenceDegree, HaralickDescriptor> pair in this)
            {
                HaralickDescriptor descriptor = pair.Value;
                double[] vector = descriptor.GetVector(features);

                for (int i = 0; i < vector.Length; i++)
                    vectors[i][c] = vector[i];
                c++;
            }

            double[] haralick = new double[features];

            for (int i = 0; i < vectors.Length; i++)
            {
                DoubleRange range = vectors[i].Range();
                double mean = vectors[i].Mean();

                haralick[i] = Accord.Math.Tools.Scale(range.Min, range.Max, -1, 1, mean);
            }

            return haralick;
        }
    }
}