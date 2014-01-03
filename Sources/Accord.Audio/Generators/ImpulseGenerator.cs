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
    using AForge.Math;

    /// <summary>
    ///   Impulse train signal generator.
    /// </summary>
    public class ImpulseGenerator : ISignalGenerator
    {

        private int interval;

        private const float ampMax = 1f;

        /// <summary>
        ///   Gets or sets the number of channels to generate.
        /// </summary>
        public int Channels { get; set; }

        /// <summary>
        ///   Gets or sets the sampling rate of channels to generate.
        /// </summary>
        public int SamplingRate { get; set; }

        /// <summary>
        ///   Gets or sets the number of pulses to generate in the signal.
        /// </summary>
        public int Pulses { get; set; }

        /// <summary>
        ///   Gets or sets the sample format for created signals.
        /// </summary>
        public SampleFormat Format { get; set; }


        /// <summary>
        ///   Gets or sets the beats per minute for the pulses.
        /// </summary>
        public int BeatsPerMinute
        {
            get { return (60 * SamplingRate) / interval; }
            set { this.interval = (60 * SamplingRate) / value; }
        }

        /// <summary>
        ///   Creates a new Impulse Signal Generator.
        /// </summary>
        public ImpulseGenerator()
            : this(32, 0, 44100, SampleFormat.Format32BitIeeeFloat)
        {
        }

        /// <summary>
        ///   Creates a new Impulse Signal Generator.
        /// </summary>
        public ImpulseGenerator(int bpm, int pulses, int sampleRate, SampleFormat format)
        {
            this.SamplingRate = sampleRate;
            this.BeatsPerMinute = bpm;
            this.Pulses = pulses;
            this.Channels = 1;
            this.Format = format;
        }

        /// <summary>
        ///   Generates the given number of samples.
        /// </summary>
        public Signal Generate(int samples)
        {
            Signal signal = new Signal(Channels, samples, SamplingRate, Format);

            int ti = interval * Channels;

            if (Format == SampleFormat.Format32BitIeeeFloat)
            {
                unsafe
                {
                    for (int c = 0; c < Channels; c++)
                    {
                        float* dst = (float*)signal.Data.ToPointer() + c;

                        for (int i = 0; i < samples; i += interval, dst += ti)
                        {
                            *dst = ampMax;

                            if (Pulses > 0 && i / interval >= Pulses)
                                break;
                        }
                    }
                }
            }
            else if (Format == SampleFormat.Format128BitComplex)
            {
                unsafe
                {
                    Complex campMax = new Complex(ampMax, 0);

                    for (int c = 0; c < Channels; c++)
                    {
                        Complex* dst = (Complex*)signal.Data.ToPointer() + c;

                        for (int i = 0; i < samples; i += interval, dst += ti)
                        {
                            *dst = campMax;

                            if (Pulses > 0 && i / interval >= Pulses)
                                break;
                        }
                    }
                }
            }

            return signal;
        }


    }
}
