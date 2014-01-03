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

    /// <summary>
    ///   Square Signal Generator
    /// </summary>
    public class SquareGenerator : ISignalGenerator
    {

        /// <summary>
        ///   Gets or sets the Frequency of the squared signal.
        /// </summary>
        public double Frequency { get; set; }

        /// <summary>
        ///   Gets or sets the Amplitude of the squared signal.
        /// </summary>
        public double Amplitude { get; set; }

        /// <summary>
        ///   Gets or sets the Sampling Rate of the generated signals.
        /// </summary>
        public int SamplingRate { get; set; }

        /// <summary>
        ///   Gets or sets the number of channels for the generated signals.
        /// </summary>
        public int Channels { get; set; }

        /// <summary>
        ///   Gets or sets the sample format for created signals.
        /// </summary>
        public SampleFormat Format { get; set; }

        /// <summary>
        ///   Creates a new Square Signal Generator.
        /// </summary>
        public SquareGenerator()
        {
        }

        /// <summary>
        ///   Generates a signal.
        /// </summary>
        public Signal Generate(int samples)
        {
            Signal signal = new Signal(Channels, samples, SamplingRate, Format);



            if (Format == SampleFormat.Format32BitIeeeFloat)
            {
                unsafe
                {
                    float* dst = (float*)signal.Data.ToPointer();

                    float p = (float)(Frequency / SamplingRate);
                    float a = 2f * (float)Amplitude;

                    for (int i = 0; i < signal.Length; i++)
                    {
                        float q = i * p;
                        float t = a * (q - (float)Math.Round(q));

                        for (int c = 0; c < signal.Channels; c++, dst++)
                            *dst = t;
                    }
                }
            }

            return signal;
        }


    }
}
