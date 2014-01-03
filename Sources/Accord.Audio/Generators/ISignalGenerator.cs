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

namespace Accord.Audio.Generators
{
    using System;
    using Accord.Audio.Generators;

    /// <summary>
    ///   Interface for Signal generators.
    /// </summary>
    public interface ISignalGenerator
    {
        /// <summary>
        ///   Gets or sets the sampling rate used to create signals.
        /// </summary>
        int SamplingRate { get; set; }

        /// <summary>
        ///   Gets or sets the number of channels of the created signals.
        /// </summary>
        int Channels { get; set; }

        /// <summary>
        ///   Gets or sets the sample format for created signals.
        /// </summary>
        SampleFormat Format { get; set; }

        /// <summary>
        ///   Generates a signal with the given number of samples.
        /// </summary>
        /// <param name="samples">The number of samples to generate.</param>
        /// <returns>The generated signal</returns>
        Signal Generate(int samples);


        // TODO: Add support for Noise generators.

    }

}
