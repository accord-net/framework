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

    /// <summary>
    ///   Custom function signal generator.
    /// </summary>
    /// 
    public class SignalGenerator : ISignalGenerator
    {

        /// <summary>
        ///   Gets or sets the windowing function to be
        ///   applied to each element in the window.
        /// </summary>
        /// 
        public Func<double, double> Function { get; set; }

        /// <summary>
        ///   Gets or sets the Sampling Rate of the generated signals.
        /// </summary>
        /// 
        public int SamplingRate { get; set; }

        /// <summary>
        ///   Gets or sets the number of channels for the generated signals.
        /// </summary>
        /// 
        public int Channels { get; set; }

        /// <summary>
        ///   Gets or sets the sample format for created signals.
        /// </summary>
        /// 
        public SampleFormat Format { get; set; }

        /// <summary>
        ///   Constructs a new signal generator.
        /// </summary>
        /// 
        public SignalGenerator(Func<double, double> func)
        {
            this.Function = func;
            this.Channels = 1;
            this.SamplingRate = 1;
            this.Format = SampleFormat.Format32BitIeeeFloat;
        }

        /// <summary>
        ///   Generates a signal.
        /// </summary>
        /// 
        public Signal Generate(int samples)
        {
            Signal signal = new Signal(Channels, samples, SamplingRate, Format);

            unsafe
            {
                if (Format == SampleFormat.Format32BitIeeeFloat)
                {
                    var dst = (float*)signal.Data.ToPointer();
                    for (int i = 0; i < signal.Samples; i++)
                        for (int c = 0; c < signal.Channels; c++, dst++)
                            *dst = (float)(Function(i));
                }
                else if (Format == SampleFormat.Format64BitIeeeFloat)
                {
                    var dst = (double*)signal.Data.ToPointer();
                    for (int i = 0; i < signal.Samples; i++)
                        for (int c = 0; c < signal.Channels; c++, dst++)
                            *dst = (double)(Function(i));
                }
                else
                {
                    throw new UnsupportedSampleFormatException("Sample format is not supported by the filter.");
                }
            }

            return signal;
        }

    }
}
