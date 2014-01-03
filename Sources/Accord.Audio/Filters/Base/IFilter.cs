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

namespace Accord.Audio.Filters
{
    using System;
    using Accord;

    /// <summary>
    /// Audio processing filter interface.
    /// </summary>
    /// 
    /// <remarks>The interface defines the set of methods, which should be
    /// provided by all signal processing filters. Methods of this interface
    /// keep the source signal unchanged and returt the result of signal processing
    /// filter as new signal.</remarks>
    /// 
    public interface IFilter
    {
    	
        /// <summary>
        ///   Apply filter to an audio signal.
        /// </summary>
        /// 
        /// <param name="signal">Source signal to apply filter to.</param>
        /// 
        /// <returns>Returns filter's result obtained by applying the filter to
        /// the source sample.</returns>
        /// 
        /// <remarks>The method keeps the source sample unchanged and returns the
        /// the result of the signal processing filter as new sample.</remarks> 
        ///
        Signal Apply(Signal signal);

        /// <summary>
        ///   Apply filter to a windowed audio signal.
        /// </summary>
        /// 
        /// <param name="signal">Source signal to apply filter to.</param>
        /// 
        /// <returns>Returns filter's result obtained by applying the filter to
        /// the source sample.</returns>
        /// 
        /// <remarks>The method keeps the source sample unchanged and returns the
        /// the result of the signal processing filter as new sample.</remarks> 
        ///
        Signal[] Apply(params Signal[] signal);
        
    }
}
