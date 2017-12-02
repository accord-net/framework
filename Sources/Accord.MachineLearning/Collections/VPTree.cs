// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
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

namespace Accord.Collections
{
#if !MONO

    using Accord;
    using Accord.Collections;
    using Accord.Math;
    using Accord.Math.Distances;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections;
    using Accord.Compat;

    /// <summary>
    ///   Vantage-Point Tree.
    /// </summary>
    /// 
    /// <seealso cref="SPTree"/>
    /// <seealso cref="VPTree"/>
    /// <seealso cref="KDTree"/>
    /// 
    [Serializable]
    public class VPTree : VPTree<double[]>
    {

        /// <summary>
        ///   Creates a new vantage-point tree from the given points.
        /// </summary>
        /// 
        /// <param name="points">The points to be added to the tree.</param>
        /// <param name="inPlace">Whether to perform operations in place, altering the
        ///   original array of points instead of creating an extra copy.</param>
        /// 
        /// <returns>A <see cref="VPTree"/> populated with the given data points.</returns>
        /// 
        public static VPTree FromData(double[][] points, bool inPlace = false)
        {
            var tree = new VPTree();
            tree.Root = tree.buildFromPoints(points, 0, points.Length, inPlace);
            return tree;
        }

        /// <summary>
        ///   Creates a new vantage-point tree from the given points.
        /// </summary>
        /// 
        /// <param name="points">The points to be added to the tree.</param>
        /// <param name="inPlace">Whether to perform operations in place, altering the
        ///   original array of points instead of creating an extra copy.</param>
        /// 
        /// <returns>A <see cref="VPTree"/> populated with the given data points.</returns>
        /// 
        public static VPTree<double> FromData(double[] points, bool inPlace = false)
        {
            return VPTree<double>.FromData(points, new Euclidean(), inPlace);
        }

        /// <summary>
        ///   Creates a new vantage-point tree from the given points.
        /// </summary>
        /// 
        /// <param name="points">The points to be added to the tree.</param>
        /// <param name="distance">The distance function to use.</param>
        /// <param name="inPlace">Whether to perform operations in place, altering the
        ///   original array of points instead of creating an extra copy.</param>
        /// 
        /// <returns>A <see cref="VPTree"/> populated with the given data points.</returns>
        /// 
        public static VPTree FromData(double[][] points, IDistance distance, bool inPlace = false)
        {
            var tree = new VPTree(distance);
            tree.Root = tree.buildFromPoints(points, 0, points.Length, inPlace);
            return tree;
        }

        /// <summary>
        ///   Creates a new vantage-point tree from the given points.
        /// </summary>
        /// 
        /// <typeparam name="TPoint">The type for the position vectors.</typeparam>
        /// 
        /// <param name="points">The points to be added to the tree.</param>
        /// <param name="distance">The distance function to use.</param>
        /// <param name="inPlace">Whether to perform operations in place, altering the
        ///   original array of points instead of creating an extra copy.</param>
        /// 
        /// <returns>A <see cref="VPTree{TPoint}"/> populated with the given data points.</returns>
        /// 
        public static VPTree<TPoint> FromData<TPoint>(TPoint[] points, IDistance<TPoint> distance, bool inPlace = false)
        {
            return VPTree<TPoint>.FromData(points, distance, inPlace);
        }

        /// <summary>
        ///   Creates a new vantage-point tree from the given points.
        /// </summary>
        /// 
        /// <typeparam name="TData">The type of the value to be stored.</typeparam>
        /// 
        /// <param name="points">The points to be added to the tree.</param>
        /// <param name="values">The corresponding values at each data point.</param>
        /// <param name="inPlace">Whether to perform operations in place, altering the
        ///   original array of points instead of creating an extra copy.</param>
        /// 
        /// <returns>A <see cref="VPTree{TPoint, TData}"/> populated with the given data points.</returns>
        /// 
        public static VPTree<double[], TData> FromData<TData>(double[][] points, TData[] values, bool inPlace = false)
        {
            return VPTree<double[], TData>.FromData(points, values, new Euclidean(), inPlace);
        }

        /// <summary>
        ///   Creates a new vantage-point tree from the given points.
        /// </summary>
        /// 
        /// <typeparam name="TPoint">The type for the position vectors.</typeparam>
        /// <typeparam name="TData">The type of the value to be stored.</typeparam>
        /// 
        /// <param name="points">The points to be added to the tree.</param>
        /// <param name="values">The corresponding values at each data point.</param>
        /// <param name="distance">The distance function to use.</param>
        /// <param name="inPlace">Whether to perform operations in place, altering the
        ///   original array of points instead of creating an extra copy.</param>
        /// 
        /// <returns>A <see cref="VPTree{TPoint, TData}"/> populated with the given data points.</returns>
        /// 
        public static VPTree<TPoint, TData> FromData<TPoint, TData>(TPoint[] points, TData[] values, IDistance<TPoint> distance, bool inPlace = false)
        {
            return VPTree<TPoint, TData>.FromData(points, values, distance, inPlace);
        }


        /// <summary>
        ///   Initializes a new instance of the <see cref="VPTree"/> class.
        /// </summary>
        /// 
        /// <param name="distance">The distance to use when comparing points. Default is 
        ///   <see cref="Accord.Math.Distance.Euclidean(double[], double[])"/>.</param>
        /// 
        public VPTree(IDistance distance)
            : base(distance)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="VPTree"/> class.
        /// </summary>
        /// 
        public VPTree()
            : base(new Euclidean())
        {
        }
    }
#endif
}

