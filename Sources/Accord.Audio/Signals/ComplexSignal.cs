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

namespace Accord.Audio
{
    using System;
    using System.Runtime.InteropServices;
    using Accord.Math;
    using AForge.Math;

    /// <summary>
    ///   Complex signal status.
    /// </summary>
    /// 
    public enum ComplexSignalStatus
    {
        /// <summary>
        ///   Normal state.
        /// </summary>
        /// 
        Normal,

        /// <summary>
        ///  Analytic form (Hilbert Transformed)
        /// </summary>
        /// 
        Analytic,

        /// <summary>
        ///  Frequency form (Fourier transformed)
        /// </summary>
        /// 
        FourierTransformed
    }

    /// <summary>
    ///   Complex audio signal.
    /// </summary>
    /// 
    /// <remarks>
    ///  <para>
    ///   A complex discrete-time signal is any complex-valued function
    ///   of integers. This class is used to keep audio signals represented 
    ///   in complex numbers so they are suitable to be converted to and
    ///   from the frequency domain in either analytic or Fourier transformed
    ///   forms.</para>
    /// 
    /// <para>    
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Analytic_signal">
    ///       Wikipedia, The Free Encyclopedia. Analytics Signal. Available on:
    ///       http://en.wikipedia.org/wiki/Analytic_signal </a></description></item>
    ///   </list></para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create complex audio signal
    /// ComplexSignal complexSignal = ComplexSignal.FromSignal( signal );
    /// // do forward Fourier transformation
    /// complexSignal.ForwardFourierTransform( );
    /// // generate spectrogram
    /// complexSignal.ToBitmap(512,512);
    /// </code>
    /// </remarks>
    /// 
    public class ComplexSignal : Signal
    {

        private ComplexSignalStatus status = ComplexSignalStatus.Normal;


        /// <summary>
        ///  Gets the status of the signal - Fourier transformed,
        ///  Hilbert transformed (analytic) or real.
        /// </summary>
        /// 
        public ComplexSignalStatus Status
        {
            get { return status; }
        }



        /// <summary>
        ///   Constructs a new Complex Signal
        /// </summary>
        /// 
        public ComplexSignal(byte[] data, int channels, int length, int sampleRate)
            : this(data, channels, length, sampleRate, ComplexSignalStatus.Normal)
        {
        }

        /// <summary>
        ///   Constructs a new Complex Signal
        /// </summary>
        /// 
        public ComplexSignal(byte[] data, int channels, int length, int sampleRate, ComplexSignalStatus status)
            : base(data, channels, length, sampleRate, SampleFormat.Format128BitComplex)
        {
            this.status = status;
        }

        /// <summary>
        ///   Constructs a new Complex Signal
        /// </summary>
        /// 
        public ComplexSignal(int channels, int length, int sampleRate)
            : base(channels, length, sampleRate, SampleFormat.Format128BitComplex)
        {
        }



        /// <summary>
        ///   Converts the complex signal to a float array.
        /// </summary>
        /// 
        public Complex[,] ToArray()
        {
            Complex[,] array = new Complex[Length, Channels];
            Buffer.BlockCopy(RawData, 0, array, 0, array.Length * Marshal.SizeOf(typeof(Complex)));
            return array;
        }

        /// <summary>
        ///   Extracts a channel from the signal.
        /// </summary>
        /// 
        public Complex[] GetChannel(int channel)
        {
            Complex[] array = new Complex[Length];
            int channels = Channels;
            int length = Length;

            unsafe
            {
                fixed (Complex* ptrArray = array)
                {
                    var src = (Complex*)Data + channel;
                    var dst = ptrArray;

                    for (int i = 0; i < length; i++, src += channels, dst++)
                        *dst = *src;
                }
            }

            return array;
        }

        /// <summary>
        ///   Copies an array of samples to a signal's channel.
        /// </summary>
        /// 
        private void SetChannel(int channel, Complex[] samples)
        {
            int channels = Channels;
            int length = Length;

            unsafe
            {
                fixed (Complex* ptrArray = samples)
                {
                    var dst = (Complex*)Data + channel;
                    var src = ptrArray;

                    for (int i = 0; i < length; i++, src++, dst += channels)
                        *dst = *src;
                }
            }
        }


        #region Transforms
        /// <summary>
        /// Applies forward fast Fourier transformation to the complex signal.
        /// </summary>
        /// 
        public void ForwardFourierTransform()
        {
            if (status == ComplexSignalStatus.Normal ||
                status == ComplexSignalStatus.Analytic)
            {
                for (int i = 0; i < Channels; i++)
                {
                    Complex[] channel = GetChannel(i);
                    FourierTransform.FFT(channel, FourierTransform.Direction.Forward);
                    SetChannel(i, channel);
                }
                status = ComplexSignalStatus.FourierTransformed;
            }
        }

        /// <summary>
        /// Applies backward fast Fourier transformation to the complex signal.
        /// </summary>
        /// 
        public void BackwardFourierTransform()
        {
            if (status == ComplexSignalStatus.FourierTransformed)
            {
                for (int i = 0; i < Channels; i++)
                {
                    Complex[] channel = GetChannel(i);
                    FourierTransform.FFT(channel, FourierTransform.Direction.Backward);
                    SetChannel(i, channel);
                }
                status = ComplexSignalStatus.Normal;
            }
        }

        /// <summary>
        ///   Applies forward Hilbert transformation to the complex signal.
        /// </summary>
        public void ForwardHilbertTransform()
        {
            if (status == ComplexSignalStatus.Normal)
            {
                for (int c = 0; c < Channels; c++)
                {
                    Complex[] channel = GetChannel(c);
                    HilbertTransform.FHT(channel, FourierTransform.Direction.Forward);
                    SetChannel(c, channel);
                }
                status = ComplexSignalStatus.Analytic;
            }
        }

        /// <summary>
        ///  Applies backward Hilbert transformation to the complex signal.
        /// </summary>
        public void BackwardHilbertTransform()
        {
            if (status == ComplexSignalStatus.Analytic)
            {
                for (int c = 0; c < Channels; c++)
                {
                    Complex[] channel = GetChannel(c);
                    HilbertTransform.FHT(channel, FourierTransform.Direction.Backward);
                    SetChannel(c, channel);
                }
                status = ComplexSignalStatus.Normal;
            }
        }
        #endregion



        #region Named constructors
        /// <summary>
        ///   Create multichannel complex signal from floating-point matrix.
        /// </summary>
        /// 
        /// <param name="signal">Source multichannel float array (matrix).</param>
        /// 
        /// <returns>Returns an instance of complex signal.</returns>
        /// 
        public static ComplexSignal FromSignal(Signal signal)
        {
            if (signal.SampleFormat == SampleFormat.Format32BitIeeeFloat)
            {
                float[] buffer = new float[signal.Samples];
                Marshal.Copy(signal.Data, buffer, 0, buffer.Length);

                float[,] data = new float[signal.Length, signal.Channels];
                Buffer.BlockCopy(buffer, 0, data, 0, signal.Samples * sizeof(float));

                return FromArray(data, signal.SampleRate);
            }
            else if (signal.SampleFormat == SampleFormat.Format128BitComplex)
            {
                return new ComplexSignal(signal.RawData, signal.Channels,
                    signal.Length, signal.SampleRate);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        ///   Create multichannel complex signal from floating-point matrix.
        /// </summary>
        /// 
        /// <param name="array">Source multichannel float array (matrix).</param>
        /// <param name="sampleRate">Sampling rate for the signal.</param>
        /// 
        /// <returns>Returns an instance of complex signal.</returns>
        /// 
        public static ComplexSignal FromArray(float[,] array, int sampleRate)
        {
            int samples = array.GetLength(0);
            int channels = array.GetLength(1);

            // check signal size
            if (!AForge.Math.Tools.IsPowerOf2(samples))
            {
                throw new InvalidSignalPropertiesException("Signals length should be a power of 2.");
            }

            Complex[,] data = new Complex[samples, channels];
            for (int i = 0; i < samples; i++)
                for (int j = 0; j < channels; j++)
                    data[i, j] = new Complex(array[i, j], 0);

            byte[] buffer = new byte[data.Length * Marshal.SizeOf(typeof(Complex))];

            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            Marshal.Copy(handle.AddrOfPinnedObject(), buffer, 0, buffer.Length);
            handle.Free();

            return new ComplexSignal(buffer, channels, samples, sampleRate);
        }


        /// <summary>
        ///   Create complex signal from complex array.
        /// </summary>
        /// 
        /// <param name="signal">Source complex array.</param>
        /// <param name="sampleRate">Sample rate of the signal.</param>
        /// 
        /// <returns>Returns an instance of complex signal.</returns>
        /// 
        public static ComplexSignal FromArray(Complex[,] signal, int sampleRate)
        {
            return ComplexSignal.FromArray(signal, sampleRate, ComplexSignalStatus.Normal);
        }

        /// <summary>
        ///   Create complex signal from complex array.
        /// </summary>
        /// 
        /// <param name="array">Source complex array.</param>
        /// <param name="sampleRate">Sample rate of the signal.</param>
        /// <param name="status">Status of the signal.</param>
        /// 
        /// <returns>Returns an instance of complex signal.</returns>
        /// 
        public static ComplexSignal FromArray(Complex[,] array, int sampleRate, ComplexSignalStatus status)
        {
            int samples = array.GetLength(0);
            int channels = array.GetLength(1);

            // check signal size
            if (!AForge.Math.Tools.IsPowerOf2(samples))
            {
                throw new InvalidSignalPropertiesException("Signals length should be a power of 2.");
            }


            byte[] buffer = new byte[array.Length * Marshal.SizeOf(typeof(Complex))];

            GCHandle handle = GCHandle.Alloc(array, GCHandleType.Pinned);
            Marshal.Copy(handle.AddrOfPinnedObject(), buffer, 0, buffer.Length);
            handle.Free();

            return new ComplexSignal(buffer, channels, samples, sampleRate, status);
        }

        #endregion


        #region Static methods

        /// <summary>
        ///   Combines a set of windows into one full signal.
        /// </summary>
        public static ComplexSignal Combine(params ComplexSignal[] signals)
        {
            // Compute common data
            int length = 0;
            int nchannels = signals[0].Channels;
            int sampleRate = signals[0].SampleRate;

            // Compute final length
            for (int i = 0; i < signals.Length; i++)
            {
                length += signals[i].Length;
            }

            // create channels
            ComplexSignal result = new ComplexSignal(nchannels, length, sampleRate);

            int pos = 0;
            foreach (ComplexSignal signal in signals)
            {
                Buffer.BlockCopy(signal.RawData, 0, result.RawData, pos, result.RawData.Length);
                pos += signal.RawData.Length;
            }

            return result;
        }

        #endregion



    }
}

