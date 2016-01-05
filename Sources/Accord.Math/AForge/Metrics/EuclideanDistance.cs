// AForge Math Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2010
// contacts@aforgenet.com
//

namespace Accord.Math.Metrics
{
    using Accord.Math.Distances;
    using System;

    /// <summary>
    /// Please use Accord.Math.Distances.Euclidean instead.
    /// </summary>
    [Obsolete("Please use Accord.Math.Distances.Euclidean instead.")]
    public sealed class EuclideanDistance : IDistance
    {
        /// <summary>
        /// Please use Accord.Math.Distances.Euclidean instead.
        /// </summary>
        public double GetDistance(double[] p, double[] q)
        {
            return new Euclidean().Distance(p, q);
        }
    }
}
