// Accord Audio Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
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

namespace Accord.Audio.ComplexFilters
{
    using System;
    using AForge;
    using AForge.Math;

    /// <summary>
    ///   Audio processing filter, which operates with Fourier transformed
    ///   complex audio signal.
    /// </summary>
    /// 
    /// <remarks>The interface defines the set of methods, which should be
    /// provided by all signal processing filter, which operate with Fourier
    /// transformed complex image.</remarks>
    /// 
    public interface IComplexFilter
    {
        /// <summary>
        ///   Apply filter to complex signal.
        /// </summary>
        /// 
        /// <param name="complexSignal">Complex signal to apply filter to.</param>
        /// 
        ComplexSignal Apply(ComplexSignal complexSignal);

        /// <summary>
        ///   Apply filter to a windowed complex signal.
        /// </summary>
        /// 
        /// <param name="complexSignal">Complex signal to apply filter to.</param>
        /// 
        ComplexSignal[] Apply(params ComplexSignal[] complexSignal);
    }
}
