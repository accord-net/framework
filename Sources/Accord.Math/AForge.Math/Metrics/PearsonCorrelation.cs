// AForge Math Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2010
// contacts@aforgenet.com
//

namespace Accord.Math.Metrics
{
    using System;

    /// <summary>
    ///   Please use Accord.Math.Distances.PearsonCorrelation instead.
    /// </summary>
    [Obsolete("Please use Accord.Math.Distances.PearsonCorrelation instead.")]
    public sealed class PearsonCorrelation : ISimilarity
    {
        /// <summary>
        ///   Please use Accord.Math.Distances.PearsonCorrelation instead.
        /// </summary>
        public double GetSimilarityScore(double[] p, double[] q)
        {
            return new Accord.Math.Distances.PearsonCorrelation().Similarity(p, q);
        }
    }
}
