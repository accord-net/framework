// AForge Math Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2011
// contacts@aforgenet.com
//

namespace AForge.Math.Geometry
{
    using System;
    using System.Collections.Generic;
    using AForge;

    /// <summary>
    /// Interface for shape optimizing algorithms.
    /// </summary>
    /// 
    /// <remarks><para>The interface defines set of methods, which should be implemented
    /// by shape optimizing algorithms. These algorithms take input shape, which is defined
    /// by a set of points (corners of convex hull, etc.), and remove some insignificant points from it,
    /// which has little influence on the final shape's look.</para>
    /// 
    /// <para>The shape optimizing algorithms can be useful in conjunction with such algorithms
    /// like convex hull searching, which usually may provide many hull points, where some
    /// of them are insignificant and could be removed.</para>
    ///
    /// <para>For additional details about shape optimizing algorithms, documentation of
    /// particular algorithm should be studied.</para>
    /// </remarks>
    /// 
    public interface IShapeOptimizer
    {
        /// <summary>
        /// Optimize specified shape.
        /// </summary>
        /// 
        /// <param name="shape">Shape to be optimized.</param>
        /// 
        /// <returns>Returns final optimized shape, which may have reduced amount of points.</returns>
        /// 
        List<IntPoint> OptimizeShape( List<IntPoint> shape );
    }
}
