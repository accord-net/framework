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

namespace Accord.Statistics.Models.Markov
{
    using Accord.Math;
    using System;

    /// <summary>
    ///   Internal methods for validation and other shared functions.
    /// </summary>
    /// 
    internal static class MarkovHelperMethods
    {
        /// <summary>
        ///   Converts a univariate or multivariate array
        ///   of observations into a two-dimensional jagged array.
        /// </summary>
        /// 
        internal static double[][] convertNoCheck(Array array, int dimension)
        {
            double[][] multivariate = array as double[][];
            if (multivariate != null) return multivariate;

            double[] univariate = array as double[];
            if (univariate != null) return Accord.Math.Matrix.Split(univariate, dimension);

            throw new ArgumentException("Invalid array argument type.", "array");
        }

        /// <summary>
        ///   Converts a univariate or multivariate array
        ///   of observations into a two-dimensional jagged array.
        /// </summary>
        /// 
        internal static double[][] checkAndConvert(Array observations, int dimension)
        {
            if (observations == null)
                throw new ArgumentNullException("observations");

            // Test if the observations are multivariate
            {
                double[][] multivariate = observations as double[][];
                if (multivariate != null)
                {
                    for (int i = 0; i < multivariate.Length; i++)
                        if (multivariate[i].Length != dimension)
                            throw new DimensionMismatchException("observations",
                                "This model expects observations of length " + dimension);
                    return multivariate;
                }

                // Test if the observations are univariate
                double[] univariate = observations as double[];
                if (univariate != null)
                {
                    if (dimension != 1)
                        throw new DimensionMismatchException("observations", 
                            "This model expects univariate observations");
                    return Accord.Math.Matrix.Split(univariate, dimension);
                }
            }

            {
                // Test if the observations are multivariate integers
                int[][] multivariate = observations as int[][];
                if (multivariate != null)
                {
                    for (int i = 0; i < multivariate.Length; i++)
                        if (multivariate[i].Length != dimension)
                            throw new DimensionMismatchException("observations",
                                "This model expects observations of length " + dimension);
                    return multivariate.ToDouble();
                }

                // Test if the observations are univariate
                int[] univariate = observations as int[];
                if (univariate != null)
                {
                    if (dimension != 1)
                        throw new DimensionMismatchException("observations",
                            "This model expects univariate observations");
                    return Accord.Math.Matrix.Split(univariate, dimension).ToDouble();
                }
            }

            // else
            throw new ArgumentException("Argument should be either of type " +
                    "double[] (for univariate observation) or double[][] (for " +
                    "multivariate observation).", "observations");
        }

        internal static void checkArgs(int[][] observations, int symbols)
        {
            if (observations == null)
                throw new ArgumentNullException("observations");

            for (int i = 0; i < observations.Length; i++)
            {
                for (int j = 0; j < observations[i].Length; j++)
                {
                    int symbol = observations[i][j];

                    if (symbol < 0 || symbol >= symbols)
                    {
                        string message = "Observation sequences should only contain symbols that are " +
                        "greater than or equal to 0, and lesser than the number of symbols passed to " +
                        "the HiddenMarkovModel. This model is expecting at most {0} symbols.";

                        throw new ArgumentOutOfRangeException("observations", String.Format(message, symbols));
                    }
                }
            }
        }
    }
}
