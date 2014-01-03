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
	
	/// <summary>
	///   Filter banks segregates signals into different parts to be further processed.
    ///   The most common filter bank is the band-pass filter bank.
	/// </summary>
    /// 
	public interface IComplexFilterBank
	{

        /// <summary>
        ///   Number of filters in the bank.
        /// </summary>
        int Length { get; }

		/// <summary>
		///   Apply filter to complex signal.
		/// </summary>
		/// 
		/// <param name="complexSignal">Complex signal to apply filter to.</param>
		ComplexSignal[] Apply(ComplexSignal complexSignal);

        /// <summary>
        ///   Apply filter to a windowed complex signal.
        /// </summary>
        /// 
        /// <param name="complexSignal">Windowed complex signal to apply filter to.</param>

        ComplexSignal[][] Apply(ComplexSignal[] complexSignal);
	}
}
