﻿// Accord Audio Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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

namespace Accord.Audio.Filters
{
    using System;
    using Accord;

    /// <summary>
    ///   In-place audio processing filter interface.
    /// </summary>
    /// 
    /// <remarks>The interface defines the set of methods, which should be
    /// provided by all signal processing filters. Methods of this interface
    /// operate in-place and alter the original source signal.</remarks>
    /// 
    public interface IInPlaceFilter : IFilter
    {

        /// <summary>
        ///   Apply filter to an audio signal.
        /// </summary>
        /// 
        /// <param name="signal">Source signal to apply filter to.</param>
        /// 
        /// <remarks>The method alters the original signal to store 
        /// the result of this signal processing filter.</remarks> 
        ///
        void ApplyInPlace(Signal signal);

        /// <summary>
        ///   Apply filter to a windowed audio signal.
        /// </summary>
        /// 
        /// <param name="signal">Source signal to apply filter to.</param>
        /// 
        /// <remarks>The method alters the original signal to store 
        /// the result of this signal processing filter.</remarks> 
        ///
        void ApplyInPlace(params Signal[] signal);

    }
}
