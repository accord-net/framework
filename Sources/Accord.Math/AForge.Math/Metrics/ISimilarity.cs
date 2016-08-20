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
    ///   Please use Accord.Math.Distances.ISimilarity instead.
    /// </summary>
    [Obsolete("Please use Accord.Math.Distances.ISimilarity instead.")]
    public interface ISimilarity
    {
        /// <summary>
        ///   Please use Accord.Math.Distances.ISimilarity instead.
        /// </summary>
        double GetSimilarityScore(double[] p, double[] q);
    }
}
