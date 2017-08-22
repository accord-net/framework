// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Rémy Dispagne, 2015
// cramer at libertysurf.fr
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
    using Accord.Math;
    using Accord.Math.Decompositions;

    /// <summary>
    /// Class to perform a Procrustes Analysis
    /// </summary>
    ///
    /// <remarks>
    /// <para>
    ///   Procrustes analysis is a form of statistical shape analysis used to
    ///   analyze the distribution of a set of shapes. It allows to compare shapes (datasets) that have
    ///   different rotations, scales and positions. It defines a measure called Procrustes distance that
    ///   is an image of how different the shapes are.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Wikipedia contributors. "Procrustes analysis" Wikipedia, The Free Encyclopedia, 21 Sept. 2015.
    ///       Available at: https://en.wikipedia.org/wiki/Procrustes_analysis </description></item>
    ///     <item><description>
    ///      Amy Ross. "Procrustes Analysis"
    ///      Available at : <![CDATA[http://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.119.2686&rep=rep1&type=pdf]]>  </description></item>
    ///  </list></para>  
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    /// // This examples shows how to use the Procrustes Analysis on basic shapes
    /// // We're about to demonstrate that a diamond with four identical edges is also a square !
    /// 
    /// // Define a square
    /// double[,] square = { { 100, 100 },
    ///                      { 300, 100 },
    ///                      { 300, 300 },
    ///                      { 100, 300 }
    ///                    };
    /// // Define a diamond with different orientation and scale
    /// double[,] diamond = { { 170, 120 },
    ///                       { 220, 170 },
    ///                       { 270, 120 },
    ///                       { 220, 70 }
    ///                     };
    ///                     
    /// // Create the Procrustes analysis object
    /// ProcrustesAnalysis pa = new ProcrustesAnalysis(square, diamond);
    /// 
    /// // Compute the analysis on the square and the diamond
    /// pa.Compute();
    /// 
    /// // Assert that the diamond is a square
    /// Debug.Assert(pa.ProcrustesDistances[0, 1].IsEqual(0.0, 1E-11));
    /// 
    /// // Transform the diamond to a square
    /// double[,] diamond_to_a_square = pa.ProcrustedDatasets[1].Transform(pa.ProcrustedDatasets[0]);
    /// 
    /// </code>
    /// </example>
    /// 
#pragma warning disable 612, 618
    public class ProcrustesAnalysis : IAnalysis
#pragma warning restore 612, 618
    {

        /// <summary>
        /// Creates a Procrustes Analysis object using the given sample data
        /// </summary>
        /// <param name="samples">Data containing multiple dataset to run the analysis on</param>
        public ProcrustesAnalysis(params double[][,] samples)
        {
            CheckSampleDataArgument(samples);

            Source = samples;
        }

        /// <summary>
        /// Creates an empty Procrustes Analysis object
        /// </summary>
        public ProcrustesAnalysis()
        {
        }

        void CheckSampleDataArgument(params double[][,] samples)
        {
            if (samples == null)
                throw new ArgumentNullException("samples");

            if (samples.Length <= 1)
                throw new ArgumentException("Samples matrix must contain at least two datasets.", "samples");

            for (int i = 1; i < samples.Length; i++)
            {
                if (samples[i].GetLength(1) != samples[0].GetLength(1))
                {
                    throw new ArgumentException("All datasets within the samples matrix must be the same dimension.", "samples");
                }
            }
        }

        /// <summary>
        /// Source data given to run the analysis
        /// </summary>
        /// 
        private double[][,] Source { get; set; }

        /// <summary>
        /// Applies the translation operator to translate the dataset to the zero coordinate
        /// </summary>
        /// 
        /// <param name="samples">The dataset to translate</param>
        /// 
        /// <returns>The translated dataset</returns>
        /// 
        double[,] Translate(double[,] samples)
        {
            return Translate(samples, new double[samples.GetLength(1)]);
        }

        /// <summary>
        /// Applies the translation operator to translate the dataset to the given coordinate
        /// </summary>
        /// 
        /// <param name="samples">Dataset to translate</param>
        /// <param name="centroid">New center of the dataset</param>
        /// 
        /// <returns>The translated dataset</returns>
        /// 
        double[,] Translate(double[,] samples, double[] centroid)
        {
            if (centroid.Length != samples.GetLength(1))
                throw new ArgumentException("Centroid length should be the same length of the samples columns !");

            double[,] res = new double[samples.GetLength(0), samples.GetLength(1)];

            // Calculate the matrix mean and translate it according to the required centroid
            double[] translated_avg = samples.Mean(0).Subtract(centroid);

            // Translate the sample data to the new centroid
            res = samples.Center(translated_avg);

            return res;
        }

        /// <summary>
        /// Applies the scale operator to scale the data to the unitary scale
        /// </summary>
        /// <param name="samples">Dataset to scale</param>
        /// <returns>The scaled dataset</returns>
        double[,] Scale(double[,] samples)
        {
            return Scale(samples, 1.0);
        }

        /// <summary>
        /// Calculates the scale of the given dataset
        /// </summary>
        /// 
        /// <param name="samples">Dataset to find the scale</param>
        /// 
        /// <returns>The scale of the dataset</returns>
        /// 
        double GetDatasetScale(double[,] samples)
        {
            double sqr = 0.0;
            double[] norm = samples.SquareEuclidean(dimension: 0);
            for (int i = 0; i < norm.Length; i++)
                sqr += norm[i];
            sqr = sqr / (double)samples.Length;
            sqr = System.Math.Sqrt(sqr);

            return sqr;
        }

        /// <summary>
        /// Applies the scale operator to scale the data to the given scale
        /// </summary>
        /// <param name="samples">Dataset to scale</param>
        /// <param name="scale">Final scale of the output dataset</param>
        /// <returns>Scaled dataset</returns>
        double[,] Scale(double[,] samples, double scale)
        {
            double[,] res = new double[samples.GetLength(0), samples.GetLength(1)];
            double sqr = GetDatasetScale(samples);

            res = samples.Divide(sqr / scale);

            return res;
        }

        /// <summary>
        /// Applies the rotation operator to the given dataset according to the reference dataset
        /// </summary>
        /// <param name="p">Procrusted dataset to rotate</param>
        /// <param name="p_reference">Reference procrusted dataset</param>
        /// <returns>The rotated dataset</returns>
        double[,] Rotate(ProcrustedDataset p, ProcrustedDataset p_reference)
        {
            // Rotation calculus per Amy Ross, Procrustes Analysis : http://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.119.2686&rep=rep1&type=pdf

            SingularValueDecomposition svd = new SingularValueDecomposition(p_reference.Dataset.TransposeAndDot(p.Dataset));

            double[,] Q = svd.RightSingularVectors.DotWithTransposed(svd.LeftSingularVectors);

            p.RotationMatrix = Q;

            return p.Dataset.Dot(Q);
        }

        /// <summary>
        /// Updates the Procrustes Distances according to a set of Procrusted samples
        /// </summary>
        /// <param name="p">Procrusted samples</param>
        void UpdateProcrustesDistances(params ProcrustedDataset[] p)
        {
            // Create the procrustes distance matrix
            ProcrustesDistances = new double[p.Length, p.Length];
            for (int i = 0; i < ProcrustesDistances.GetLength(0); i++)
            {
                ProcrustesDistances[i, i] = 0;
            }

            // Compute the distances for one corner of the matrix (this is a symmetric matrix)
            for (int i = 0; i < ProcrustesDistances.GetLength(0); i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    if (i != j)
                    {
                        // Calculate the Procrusted distance
                        ProcrustesDistances[i, j] = ProcrustesDistance(p[i].Dataset, p[j].Dataset);

                        // The other corner of the matrix is the same as the other corner
                        ProcrustesDistances[j, i] = ProcrustesDistances[i, j];
                    }
                }
            }
        }

        /// <summary>
        /// Calculate the Procrustes Distance between two sets of data
        /// </summary>
        /// <param name="samples1">First data set</param>
        /// <param name="samples2">Second data set</param>
        /// <returns>The Procrustes distance</returns>
        double ProcrustesDistance(double[,] samples1, double[,] samples2)
        {
            double sum = 0;
            // Only calculate the distance for the maximal common number of points (i.e. the Min between samples1 and samples2 number of points)
            for (int i = 0; i < System.Math.Min(samples1.GetLength(0), samples2.GetLength(0)); i++)
            {
                sum += Norm.SquareEuclidean(samples1.GetRow(i).Subtract(samples2.GetRow(i)));
            }

            return System.Math.Sqrt(sum);
        }


        /// <summary>
        /// Procrustes Distances of the computed samples
        /// </summary>
        public double[,] ProcrustesDistances { get; protected set; }

        /// <summary>
        /// Procrusted models produced from the computed sample data
        /// </summary>
        public ProcrustedDataset[] ProcrustedDatasets { get; protected set; }



        /// <summary>
        /// Apply Procrustes translation and scale to the given dataset
        /// </summary>
        /// 
        /// <param name="p">Procrusted dataset to process and store the results to</param>
        /// 
        /// <param name="samples">The dataset itself</param>
        /// 
        private void ApplyTranslateScale(ProcrustedDataset p, double[,] samples)
        {
            // Save the original data
            p.Source = samples;

            // Save the original data center (i.e. mean)
            p.Center = p.Source.Mean(dimension: 0);
            // Translate the samples to zero
            p.Dataset = Translate(samples);

            // Save the original scale of the dataset
            p.Scale = GetDatasetScale(p.Dataset);
            // Scale the dataset to 1
            p.Dataset = Scale(p.Dataset);
        }

        /// <summary>
        ///   Compute the Procrustes analysis to extract Procrustes distances and models using the constructor parameters
        /// </summary>
        /// 
        public void Compute()
        {
            Compute(-1, Source);
        }

        /// <summary>
        ///   Compute the Procrustes analysis to extract Procrustes distances and models
        /// </summary>
        /// 
        /// <param name="samples">List of sample data sets to analyze</param>
        /// 
        /// <returns>Procrustes distances of the analyzed samples</returns>
        /// 
        public double[,] Compute(params double[][,] samples)
        {
            return Compute(-1, samples);
        }

        /// <summary>
        ///   Compute the Procrustes analysis to extract Procrustes distances and models by specifying the reference dataset
        /// </summary>
        /// 
        /// <param name="reference_sample_index">Index of the reference dataset. If out of bounds of the sample array, the first dataset is used.</param>
        /// <param name="samples">List of sample data sets to analyze</param>
        /// 
        /// <returns>Procrustes distances of the analyzed samples</returns>
        /// 
        public double[,] Compute(int reference_sample_index, params double[][,] samples)
        {
            // Check arguments in case of mistakes
            CheckSampleDataArgument(samples);

            // If reference index out of bounds...
            if (reference_sample_index < 0 || reference_sample_index >= samples.Length)
            {
                // Use the first element of the array
                reference_sample_index = 0;
            }

            // Allocate space for the computed results
            ProcrustedDatasets = new ProcrustedDataset[samples.Length];

            // Start with the reference dataset
            ProcrustedDatasets[reference_sample_index] = new ProcrustedDataset();
            // The rotation martrix of the reference is the identity matrix (i.e. no rotation since it's the reference itself)
            ProcrustedDatasets[reference_sample_index].RotationMatrix = Matrix.Identity(samples[reference_sample_index].GetLength(1));
            // Translate then scale the dataset to set it with scale = 1 and center position at zero
            ApplyTranslateScale(ProcrustedDatasets[reference_sample_index], samples[reference_sample_index]);

            // For each data set
            for (int i = 0; i < samples.Length; i++)
            {
                // Except for the reference
                if (i != reference_sample_index)
                {
                    ProcrustedDatasets[i] = new ProcrustedDataset();
                    // Translate then scale the dataset
                    ApplyTranslateScale(ProcrustedDatasets[i], samples[i]);

                    // Finally, rotate the sample to fit the reference data rotation
                    ProcrustedDatasets[i].Dataset = Rotate(ProcrustedDatasets[i], ProcrustedDatasets[reference_sample_index]);
                }
            }

            // Update the Procrustes distance matrix
            UpdateProcrustesDistances(ProcrustedDatasets);

            return ProcrustesDistances;
        }
    }

    #region Support Classes
    /// <summary>
    /// Class to represent an original dataset, its Procrustes form and all necessary data (i.e. rotation, center, scale...)
    /// </summary>
    public class ProcrustedDataset
    {
        /// <summary>
        /// Original dataset
        /// </summary>
        public double[,] Source { get; set; }

        /// <summary>
        /// Procrustes dataset (i.e. original dataset after Procrustes analysis)
        /// </summary>
        public double[,] Dataset { get; set; }

        /// <summary>
        /// Original dataset center
        /// </summary>
        public double[] Center { get; set; }

        /// <summary>
        /// Original dataset scale
        /// </summary>
        public double Scale { get; set; }

        /// <summary>
        /// Original dataset rotation matrix
        /// </summary>
        public double[,] RotationMatrix { get; set; }

        /// <summary>
        /// Transforms the dataset to match the given reference original dataset
        /// </summary>
        /// <param name="p_reference">Dataset to match</param>
        /// <returns>The transformed dataset matched to the reference</returns>
        public double[,] Transform(ProcrustedDataset p_reference)
        {
            // Make a copy of the current Procrustes dataset
            double[,] tData = (double[,])Dataset.Clone();

            // Rotate the dataset to match the reference dataset rotation
            tData = tData.Dot(p_reference.RotationMatrix.Transpose());

            // Scale the dataset to match the reference scale
            tData = tData.Multiply(p_reference.Scale);

            // Prepare a negative translation vector to...
            double[] refCenter = p_reference.Center.Multiply(-1);
            // ... move the dataset to the same center as the reference
            tData = tData.Center(refCenter);

            return tData;
        }
    }
    #endregion
}
