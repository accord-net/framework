// Accord Audio Library
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

namespace Accord.Audio
{
    using System;
    using System.Runtime.InteropServices;
    using Accord.Math;
    using System.Collections.Generic;
    using Accord.Compat;
    using System.Numerics;

    /// <summary>
    ///   Tool functions for audio processing.
    /// </summary>
    /// 
    internal static class UnsafeTools
    {
        /// <summary>
        ///   Gets the min, max and length in the each channel of a given signal.
        /// </summary>
        /// 
        public static unsafe float[] GetRange(float* data, int channels, int length, out float[] min, out float[] max)
        {
            min = new float[channels];
            max = new float[channels];
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < channels; j++, data++)
                {
                    float v = *data;
                    if (v < min[j])
                        min[j] = v;
                    if (v > max[j])
                        max[j] = v;
                }
            }

            return max.Subtract(min);
        }
    }
}
