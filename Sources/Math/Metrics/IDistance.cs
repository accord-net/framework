// AForge Math Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2010
// contacts@aforgenet.com
//

namespace AForge.Math.Metrics
{
    using System;

    /// <summary>
    /// Interface for distance metric algorithms.
    /// </summary>
    /// 
    /// <remarks><para>The interface defines a set of methods implemented
    /// by distance metric algorithms. These algorithms typically take a set of points and return a 
    /// distance measure of the x and y coordinates. In this case, the points are represented by two vectors.</para>
    /// 
    /// <para>Distance metric algorithms are used in many machine learning algorithms e.g K-nearest neighbor
    /// and K-means clustering.</para>
    ///
    /// <para>For additional details about distance metrics, documentation of the
    /// particular algorithms should be studied.</para>
    /// </remarks>
    /// 
    public interface IDistance
    {
        /// <summary>
        /// Returns distance between two N-dimensional double vectors.
        /// </summary>
        /// 
        /// <param name="p">1st point vector.</param>
        /// <param name="q">2nd point vector.</param>
        /// 
        /// <returns>Returns distance measurement determined by the given algorithm.</returns>
        /// 
        double GetDistance( double[] p, double[] q );
    }
}
