// Accord Math Library
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

namespace Accord.Math.Distances
{

    /// <summary>
    ///   Common interface for distance functions (not necessarily metrics).
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The framework distinguishes between metrics and distances by using different
    ///   types for them. This makes it possible to let the compiler figure out logic
    ///   problems such as the specification of a non-metric for a method that requires
    ///   a proper metric (i.e. that respects the triangle inequality).</para>
    ///   
    /// <para>
    ///   The objective of this technique is to make it harder to make some mistakes.
    ///   However, it is generally possible to bypass this mechanism by using named constructors
    ///   available at each of the classes, such as Minkowski's <see cref="Minkowski.Nonmetric"/> 
    ///   method, to create distances implementing the <see cref="IMetric{T}"/> interface that are not
    ///   really metrics. Use at your own risk.</para>
    /// </remarks>
    /// 
    /// <typeparam name="T">The type of the first element to be compared.</typeparam>
    /// <typeparam name="U">The type of the second element to be compared.</typeparam>
    /// 
    /// <seealso cref="IMetric{T}"/>
    /// 
    public interface IDistance<in T, in U>
    {
        /// <summary>
        ///   Computes the distance <c>d(x,y)</c> between points
        ///   <paramref name="x"/> and <paramref name="y"/>.
        /// </summary>
        /// 
        /// <param name="x">The first point <c>x</c>.</param>
        /// <param name="y">The second point <c>y</c>.</param>
        /// 
        /// <returns>
        ///   A double-precision value representing the distance <c>d(x,y)</c>
        ///   between <paramref name="x"/> and <paramref name="y"/> according 
        ///   to the distance function implemented by this class.
        /// </returns>
        /// 
        double Distance(T x, U y);
    }

    /// <summary>
    ///   Common interface for distance functions (not necessarily metrics).
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The framework distinguishes between metrics and distances by using different
    ///   types for them. This makes it possible to let the compiler figure out logic
    ///   problems such as the specification of a non-metric for a method that requires
    ///   a proper metric (i.e. that respects the triangle inequality).</para>
    ///   
    /// <para>
    ///   The objective of this technique is to make it harder to make some mistakes.
    ///   However, it is generally possible to bypass this mechanism by using named constructors
    ///   available at each of the classes, such as Minkowski's <see cref="Minkowski.Nonmetric"/> 
    ///   method, to create distances implementing the <see cref="IMetric{T}"/> interface that are not
    ///   really metrics. Use at your own risk.</para>
    /// </remarks>
    /// 
    /// <typeparam name="T">The type of the elements to be compared.</typeparam>
    /// 
    /// <seealso cref="IDistance{T}"/>
    /// <seealso cref="IDistance{T, U}"/>
    /// <seealso cref="IMetric{T}"/>
    /// 
    public interface IDistance<in T> : IDistance<T, T>
    {
    }

    /// <summary>
    ///   Common interface for distance functions (not necessarily metrics).
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The framework distinguishes between metrics and distances by using different
    ///   types for them. This makes it possible to let the compiler figure out logic
    ///   problems such as the specification of a non-metric for a method that requires
    ///   a proper metric (i.e. that respects the triangle inequality).</para>
    ///   
    /// <para>
    ///   The objective of this technique is to make it harder to make some mistakes.
    ///   However, it is generally possible to bypass this mechanism by using named constructors
    ///   available at each of the classes, such as Minkowski's <see cref="Minkowski.Nonmetric"/> 
    ///   method, to create distances implementing the <see cref="IMetric{T}"/> interface that are not
    ///   really metrics. Use at your own risk.</para>
    /// </remarks>
    /// 
    /// <seealso cref="IDistance{T}"/>
    /// <seealso cref="IMetric{T}"/>
    /// 
    public interface IDistance : IDistance<double[]>
    {
    }
}
