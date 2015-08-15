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
    /// Interface defining methods for algorithms, which search for convex hull of the specified points' set.
    /// </summary>
    /// 
    /// <remarks><para>The interface defines a method, which should be implemented by different classes
    /// performing convex hull search for specified set of points.</para>
    /// 
    /// <para><note>All algorithms, implementing this interface, should follow two rules for the found convex hull:
    /// <list type="bullet">
    /// <item>the first point in the returned list is the point with lowest X coordinate (and with lowest Y if
    /// there are several points with the same X value);</item>
    /// <item>points in the returned list are given in counter clockwise order
    /// (<a href="http://en.wikipedia.org/wiki/Cartesian_coordinate_system">Cartesian
    /// coordinate system</a>).</item>
    /// </list>
    /// </note></para>
    /// </remarks>
    /// 
    public interface IConvexHullAlgorithm
    {
        /// <summary>
        /// Find convex hull for the given set of points.
        /// </summary>
        /// 
        /// <param name="points">Set of points to search convex hull for.</param>
        /// 
        /// <returns>Returns set of points, which form a convex hull for the given <paramref name="points"/>.</returns>
        /// 
        List<IntPoint> FindHull( List<IntPoint> points );
    }
}
