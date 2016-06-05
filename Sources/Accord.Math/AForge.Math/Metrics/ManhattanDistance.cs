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
    ///   Please use Accord.Math.Distances.Manhattan instead.
    /// </summary>
    [Obsolete("Please use Accord.Math.Distances.Manhattan instead.")]
    public sealed class ManhattanDistance : IDistance
    {
        /// <summary>
        ///   Please use Accord.Math.Distances.Manhattan instead.
        /// </summary>
        public double GetDistance(double[] p, double[] q)
        {
            return new Manhattan().Distance(p, q);
        }
    }
}
