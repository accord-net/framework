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
    using Accord.Audio;
    using Accord.Audio.Generators;
    using AForge.Math;

    /// <summary>
    ///   Comb filter.
    /// </summary>
    /// 
    public class CombFilter : BaseComplexFilter
    {

        private ImpulseGenerator impulseGenerator;
        private ComplexSignal combSignal;
        private int length;


        /// <summary>
        ///   Gets or sets the current BPM for the underlying impulse generator.
        /// </summary>
        /// 
        public int BeatsPerMinute
        {
            get { return impulseGenerator.BeatsPerMinute; }
            set
            {
                if (impulseGenerator.BeatsPerMinute != value)
                {
                    impulseGenerator.BeatsPerMinute = value;
                    generateBaseSignal();
                }
            }
        }

        /// <summary>
        ///   Gets or sets the length of the comb filter.
        /// </summary>
        /// 
        public int Length
        {
            get { return length; }
            set
            {
                if (length != value)
                {
                    length = value;
                    generateBaseSignal();
                }
            }
        }

        /// <summary>
        ///   Gets or sets the number of channels for the filter.
        /// </summary>
        /// 
        public int Channels
        {
            get { return impulseGenerator.Channels; }
            set
            {
                if (impulseGenerator.Channels != value)
                {
                    impulseGenerator.Channels = value;
                    generateBaseSignal();
                }
            }
        }

        /// <summary>
        ///   Creates a new Comb filter.
        /// </summary>
        /// 
        public CombFilter(int bpm, int pulses, int length, int samplingRate)
        {
            this.impulseGenerator = new ImpulseGenerator(bpm, pulses, samplingRate, SampleFormat.Format128BitComplex);
            this.length = length;
            generateBaseSignal();
        }


        private void generateBaseSignal()
        {
            combSignal = (ComplexSignal)impulseGenerator.Generate(this.length);
            combSignal.ForwardFourierTransform();
        }

        /// <summary>
        /// Processes the filter.
        /// </summary>
        /// 
        protected override void ProcessFilter(ComplexSignal sourceData, ComplexSignal destinationData)
        {
            SampleFormat format = sourceData.SampleFormat;
            int channels = sourceData.Channels;
            int length = sourceData.Length;
            int samples = sourceData.Samples;

            unsafe
            {
                Complex* src = (Complex*)sourceData.Data.ToPointer();
                Complex* dst = (Complex*)destinationData.Data.ToPointer();
                Complex* comb = (Complex*)combSignal.Data.ToPointer();

                Complex d = new Complex();

                for (int i = 0; i < samples; i++, src++, dst++, comb++)
                {
                    d.Re = (src[0] * comb[0]).Magnitude;
                    *dst = d;
                }
            }
        }


    }
}
