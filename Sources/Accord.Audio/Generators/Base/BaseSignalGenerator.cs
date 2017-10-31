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

namespace Accord.Audio.Generators
{
    using System;
    using Accord.Audio.Generators;

    /// <summary>
    ///   Base class for signal generators implementing the <see cref="ISignalGenerator"/> interface.
    /// </summary>
    /// 
    public abstract class BaseSignalGenerator
    {
        /// <summary>
        ///   Gets or sets the sampling rate used to create signals.
        /// </summary>
        /// 
        public int SamplingRate { get; set; }

        /// <summary>
        ///   Gets or sets the number of channels of the created signals.
        /// </summary>
        /// 
        public int Channels { get; set; }

        /// <summary>
        ///   Gets or sets the sample format for created signals.
        /// </summary>
        /// 
        public SampleFormat Format { get; set; }

        /// <summary>
        ///   Generates a signal with the given number of samples.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// 
        /// <returns>The generated signal</returns>
        /// 
        public abstract Signal Generate(int samples);

        /// <summary>
        ///   Generates a signal with the given duration.
        /// </summary>
        /// 
        /// <param name="duration">The duration of the signal to generate.</param>
        /// 
        /// <returns>The generated signal</returns>
        /// 
        public Signal Generate(TimeSpan duration)
        {
            return Generate(Signal.GetNumberOfSamples(duration, SamplingRate));
        }

    }
}
