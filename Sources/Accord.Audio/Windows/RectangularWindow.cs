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

namespace Accord.Audio.Windows
{
    using Accord.Compat;
    using System.Numerics;

    /// <summary>
    ///   Rectangular Window.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The rectangular window (sometimes known as the boxcar or Dirichlet window) 
    ///   is the simplest window, equivalent to replacing all but N values of a data 
    ///   sequence by zeros, making it appear as though the waveform suddenly turns 
    ///   on and off.</para>
    ///   
    /// <para>    
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Window_function">
    ///       Wikipedia, The Free Encyclopedia. Window function. Available on:
    ///       http://en.wikipedia.org/wiki/Window_function </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    public class RectangularWindow : IWindow
    {
        private int length;
        private int sampleRate;

        /// <summary>
        ///   Gets the Window's length.
        /// </summary>
        /// 
        public int Length
        {
            get { return length; }
        }

        /// <summary>
        ///   Gets the Window's duration.
        /// </summary>
        /// 
        public double Duration
        {
            get { return sampleRate * length; }
        }

        /// <summary>
        ///   Constructs a new Rectangular Window.
        /// </summary>
        /// 
        public RectangularWindow(int length)
        {
            this.length = length;
        }

        /// <summary>
        ///   Constructs a new Rectangular Window.
        /// </summary>
        /// 
        public RectangularWindow(int length, int sampleRate)
        {
            this.length = length;
            this.sampleRate = sampleRate;
        }


        /// <summary>
        ///   Splits a signal using the current window.
        /// </summary>
        /// 
        public Signal Apply(Signal signal, int sampleIndex)
        {
            int channels = signal.Channels;
            int samples = signal.Length;

            int minLength = System.Math.Min(samples - sampleIndex, Length);

            Signal result = new Signal(channels, Length, signal.SampleRate, signal.SampleFormat);

            if (signal.SampleFormat == SampleFormat.Format32BitIeeeFloat)
            {
                for (int c = 0; c < channels; c++)
                {
                    unsafe
                    {
                        float* dst = (float*)result.Data.ToPointer() + c;
                        float* src = (float*)signal.Data.ToPointer() + c + channels * sampleIndex;

                        for (int i = 0; i < minLength; i++, dst += channels, src += channels)
                        {
                            *dst = *src;
                        }
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
        ///   Splits a complex signal using the current window.
        /// </summary>
        /// 
        public ComplexSignal Apply(ComplexSignal complexSignal, int sampleIndex)
        {
            Complex[,] resultData = new Complex[Length, complexSignal.Channels];
            ComplexSignal result = ComplexSignal.FromArray(resultData, complexSignal.SampleRate);

            int channels = result.Channels;
            int minLength = System.Math.Min(complexSignal.Length - sampleIndex, Length);

            unsafe
            {
                for (int c = 0; c < complexSignal.Channels; c++)
                {
                    Complex* dst = (Complex*)result.Data.ToPointer() + c;
                    Complex* src = (Complex*)complexSignal.Data.ToPointer() + c + channels * sampleIndex;

                    for (int i = 0; i < minLength; i++, dst += channels, src += channels)
                    {
                        *dst = *src;
                    }
                }
            }

            return result;
        }

        /// <summary>
        ///   Splits a signal using the window.
        /// </summary>
        /// 
        public virtual double[] Apply(double[] signal, int sampleIndex)
        {
            int minLength = System.Math.Min(signal.Length - sampleIndex, Length);

            double[] result = new double[Length];

            unsafe
            {
                fixed (double* R = result)
                fixed (double* S = signal)
                {
                    double* dst = R;
                    double* src = S + sampleIndex;

                    for (int i = 0; i < minLength; i++, dst++, src++)
                        *dst = *src;
                }
            }

            return result;
        }

        /// <summary>
        ///   Splits a signal using the window.
        /// </summary>
        /// 
        public virtual double[][] Apply(double[][] signal, int sampleIndex)
        {
            int channels = signal[0].Length;

            int minLength = System.Math.Min(signal.Length - sampleIndex, Length);

            double[][] result = new double[signal.Length][];
            for (int i = 0; i < result.Length; i++)
                result[i] = new double[signal[i].Length];

            for (int c = 0; c < channels; c++)
                for (int i = 0; i < minLength; i++)
                    result[i][c] = signal[i + sampleIndex][c];

            return result;
        }
    }
}
