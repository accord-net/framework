// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2013
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

namespace Accord.MachineLearning.Structures
{
    using System;

    /// <summary>
    ///   Convenience class for k-dimensional tree static
    ///   methods. Please specify a generic parameter to
    ///   create a new tree from scratch.
    /// </summary>
    /// 
    /// <seealso cref="KDTree{T}"/>
    /// 
    public static class KDTree
    {

        /// <summary>
        ///   Creates a new k-dimensional tree from the given points.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the value to be stored.</typeparam>
        /// 
        /// <param name="points">The points to be added to the tree.</param>
        /// 
        /// <returns>A <see cref="KDTree{T}"/> populated with the given data points.</returns>
        /// 
        public static KDTree<T> FromData<T>(double[][] points)
        {
            return KDTree<T>.FromData(points);
        }

        /// <summary>
        ///   Creates a new k-dimensional tree from the given points.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the value to be stored.</typeparam>
        /// 
        /// <param name="points">The points to be added to the tree.</param>
        /// <param name="values">The corresponding values at each data point.</param>
        /// 
        /// <returns>A <see cref="KDTree{T}"/> populated with the given data points.</returns>
        /// 
        public static KDTree<T> FromData<T>(double[][] points, T[] values)
        {
            return KDTree<T>.FromData(points, values);
        }

        /// <summary>
        ///   Creates a new k-dimensional tree from the given points.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the value to be stored.</typeparam>
        /// 
        /// <param name="points">The points to be added to the tree.</param>
        /// <param name="distance">The distance function to use.</param>
        /// 
        /// <returns>A <see cref="KDTree{T}"/> populated with the given data points.</returns>
        /// 
        public static KDTree<T> FromData<T>(double[][] points, Func<double[], double[], double> distance)
        {
            return KDTree<T>.FromData(points, distance);
        }

        /// <summary>
        ///   Creates a new k-dimensional tree from the given points.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the value to be stored.</typeparam>
        /// 
        /// <param name="points">The points to be added to the tree.</param>
        /// <param name="values">The corresponding values at each data point.</param>
        /// <param name="distance">The distance function to use.</param>
        /// 
        /// <returns>A <see cref="KDTree{T}"/> populated with the given data points.</returns>
        /// 
        public static KDTree<T> FromData<T>(double[][] points, T[] values, Func<double[], double[], double> distance)
        {
            return KDTree<T>.FromData(points, values, distance);
        }

    }
}
