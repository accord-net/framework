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

namespace Accord.Audio.Windows
{
    using AForge.Math;

    /// <summary>
    ///   Base abstract class for signal windows.
    /// </summary>
    /// 
    public abstract class WindowBase : IWindow
    {
        private int sampleRate;
        private float[] window;

        /// <summary>
        ///   Gets the window length.
        /// </summary>
        /// 
        public int Length
        {
            get { return window.Length; }
        }

        /// <summary>
        ///   Gets the Window duration.
        /// </summary>
        /// 
        public double Duration
        {
            get { return sampleRate * window.Length; }
        }

        /// <summary>
        ///   Constructs a new Window.
        /// </summary>
        /// 
        protected WindowBase(double duration, int sampleRate)
            : this((int)duration * sampleRate, sampleRate)
        {

        }

        /// <summary>
        ///   Constructs a new Window.
        /// </summary>
        /// 
        protected WindowBase(int length)
            : this(length, 0)
        {
        }

        /// <summary>
        ///   Constructs a new Window.
        /// </summary>
        /// 
        protected WindowBase(int length, int sampleRate)
        {
            this.window = new float[length];
            this.sampleRate = sampleRate;
        }

        /// <summary>
        ///   Gets or sets values for the Window function.
        /// </summary>
        /// 
        public float this[int index]
        {
            get { return window[index]; }
            protected set { window[index] = value; }
        }

        /// <summary>
        ///   Splits a signal using the window.
        /// </summary>
        /// 
        public unsafe virtual Signal Apply(Signal signal, int sampleIndex)
        {
            int channels = signal.Channels;
            int samples = signal.Length;

            int minLength = System.Math.Min(signal.Length, Length);

            Signal result = new Signal(channels, Length, signal.SampleRate, signal.SampleFormat);

            if (signal.SampleFormat == SampleFormat.Format32BitIeeeFloat)
            {
                for (int c = 0; c < channels; c++)
                {
                    float* dst = (float*)result.Data.ToPointer() + c;
                    float* src = (float*)signal.Data.ToPointer() + c + c * sampleIndex;

                    for (int i = 0; i < minLength; i++, dst += channels, src += channels)
                    {
                        *dst = window[i] * (*src);
                    }
                }
            }
            else
            {
                throw new UnsupportedSampleFormatException("Sample format is not supported by the filter.");
            }

            return result;
        }

        /// <summary>
        ///   Splits a signal using the window.
        /// </summary>
        /// 
        public virtual ComplexSignal Apply(ComplexSignal complexSignal, int sampleIndex)
        {
            Complex[,] resultData = new Complex[Length, complexSignal.Channels];
            ComplexSignal result = ComplexSignal.FromArray(resultData, complexSignal.SampleRate);

            int channels = result.Channels;

            unsafe
            {
                for (int c = 0; c < complexSignal.Channels; c++)
                {
                    Complex* dst = (Complex*)result.Data.ToPointer() + c;
                    Complex* src = (Complex*)complexSignal.Data.ToPointer() + c + c * sampleIndex;

                    for (int i = 0; i < Length; i++, dst += channels, src += channels)
                    {
                        *dst = window[i] * (*src);
                    }
                }
            }

            return result;
        }

    }
}
