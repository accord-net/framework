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
    ///   Cosine signal generator.
    /// </summary>
    /// 
    public class CosineGenerator : ISignalGenerator
    {
        private double theta;


        /// <summary>
        ///   Gets or sets the Frequency of the cosine signal.
        /// </summary>
        /// 
        public double Frequency { get; set; }

        /// <summary>
        ///   Gets or sets the Amplitude of the cosine signal.
        /// </summary>
        /// 
        public double Amplitude { get; set; }

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
        ///   Constructs a new Cosine Signal Generator.
        /// </summary>
        /// 
        public CosineGenerator(double frequency, double amplitude, int samplingRate)
        {
            this.Frequency = frequency;
            this.Amplitude = amplitude;

            this.theta = 2.0 * Math.PI * frequency / samplingRate;
        }

        /// <summary>
        ///   Generates a signal.
        /// </summary>
        /// 
        public Signal Generate(int samples)
        {
            Signal signal = new Signal(Channels, samples, SamplingRate, Format);

            if (Format == SampleFormat.Format32BitIeeeFloat)
            {
                unsafe
                {
                    float* dst = (float*)signal.Data.ToPointer();

                    for (int i = 0; i < signal.Samples; i++)
                    {
                        float t = (float)(Amplitude * Math.Cos(i * theta));

                        for (int c = 0; c < signal.Channels; c++, dst++)
                            *dst = t;
                    }
                }
            }

            return signal;
        }


    }
}
