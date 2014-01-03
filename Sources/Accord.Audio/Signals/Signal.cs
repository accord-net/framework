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
    using AForge.Math;

    /// <summary>
    ///   Specifies the format of each sample in a signal.
    /// </summary>
    /// 
    public enum SampleFormat
    {
        /// <summary>
        ///   Specifies the format is 8 bit, unsigned.
        /// </summary>
        /// 
        Format8BitUnsigned,

        /// <summary>
        ///   Specifies the format is 8 bit, signed.
        /// </summary>
        /// 
        Format8Bit,

        /// <summary>
        ///   Specifies the format is 16 bit, signed.
        /// </summary>
        /// 
        Format16Bit,

        /// <summary>
        ///   Specifies the format is 32 bit, signed.
        /// </summary>
        /// 
        Format32Bit,

        /// <summary>
        ///   Specifies the format is 32 bit, represented by
        ///   single-precision IEEE floating-point numbers.
        /// </summary>
        /// 
        Format32BitIeeeFloat,

        /// <summary>
        ///   Specifies the format is 64 bit, represented by
        ///   double-precision IEEE floating-point numbers.
        /// </summary>
        /// 
        Format64BitIeeeFloat,

        /// <summary>
        ///   Specifies the format is 128 bit, represented by
        ///   complex numbers with real and imaginary parts as
        ///   double-precision IEEE floating-point numbers.
        /// </summary>
        /// 
        Format128BitComplex,
    }

    /// <summary>
    ///   Represents a PCM sound discrete signal (measured in time).
    /// </summary>
    /// 
    /// <remarks>
    ///  <para>
    ///   A real discrete-time signal is defined as any real-valued 
    ///   function of the integers.</para>
    ///  <para>
    ///  In signal processing, sampling is the reduction of a continuous
    ///  signal to a discrete signal. A common example is the conversion
    ///  of a sound wave (a continuous-time signal) to a sequence of samples
    ///  (a discrete-time signal).</para>
    ///  
    ///  <para>
    ///  A sample refers to a value or set of values at a point in time 
    ///  and/or space.</para>
    ///
    /// <para>Sample usage:</para>
    /// <code>
    /// // create an empty audio signal 
    /// Signal signal = new Signal( channels, length, sampleRate, format );
    /// </code>
    /// 
    /// <code>
    /// float[,] data = 
    /// {
    ///     {  0.00f, 0.2f  },
    ///     {  0.32f, 0.1f  },
    ///     {  0.22f, 0.2f  },
    ///     {  0.12f, 0.42f },
    ///     { -0.12f, 0.1f  },
    ///     { -0.22f, 0.2f  },
    /// };
    /// 
    /// // or create an audio signal from an array of audio frames
    /// Signal target = Signal.FromArray(data, sampleRate: 8000);
    /// </code>
    /// 
    /// <para>
    /// For an example on how to decode a signal from a Wave file, please
    /// take a look on the WaveDecoder and WaveFileAudioSource documentation.</para>
    /// </remarks>
    /// 
    /// <seealso cref="ComplexSignal"/>
    ///
    public class Signal : IDisposable
    {
        private byte[] rawData;
        private IntPtr ptrData;
        private GCHandle handle;

        private SampleFormat format;

        private int channels;
        private int sampleRate;
        private int length;



        #region Properties
        /// <summary>
        ///   Gets the sample format used by this signal.
        /// </summary>
        /// 
        /// <value>The signal's sample format.</value>
        /// 
        public SampleFormat SampleFormat
        {
            get { return format; }
        }

        /// <summary>
        ///   Gets the signal duration in milliseconds.
        /// </summary>
        /// 
        public int Duration
        {
            get { return DurationOfSamples(length, sampleRate); }
        }

        /// <summary>
        ///   Gets the number of samples in each channel of this signal,
        ///   as known as the number of frames in the signal.
        /// </summary>
        /// 
        public int Length
        {
            get { return length; }
        }

        /// <summary>
        ///   Gets the total number of samples in this signal.
        /// </summary>
        /// 
        public int Samples
        {
            get { return length * channels; }
        }

        /// <summary>
        ///   Gets the number of samples per second for this signal.
        /// </summary>
        /// 
        public int SampleRate
        {
            get { return sampleRate; }
        }

        /// <summary>
        ///   Gets the number of channels of this signal.
        /// </summary>
        /// 
        public int Channels
        {
            get { return channels; }
        }

        /// <summary>
        ///   Gets the raw binary data representing the signal.
        /// </summary>
        /// 
        public byte[] RawData
        {
            get { return rawData; }
            protected set { rawData = value; }
        }

        /// <summary>
        ///   Gets a pointer to the first sample of the signal.
        /// </summary>
        /// 
        public IntPtr Data
        {
            get { return ptrData; }
            protected set { ptrData = value; }
        }
        #endregion



        /// <summary>
        ///   Constructs a new signal.
        /// </summary>
        /// 
        /// <param name="data">The raw data for the signal.</param>
        /// <param name="channels">The number of channels for the signal.</param>
        /// <param name="length">The length of the signal.</param>
        /// <param name="format">The sample format for the signal.</param>
        /// <param name="sampleRate">The sample date of the signal.</param>
        /// 
        public Signal(byte[] data, int channels, int length, int sampleRate, SampleFormat format)
        {
            init(data, channels, length, sampleRate, format);
        }

        /// <summary>
        ///   Constructs a new Signal.
        /// </summary>
        /// 
        /// <param name="channels">The number of channels for the signal.</param>
        /// <param name="length">The length of the signal.</param>
        /// <param name="format">The sample format for the signal.</param>
        /// <param name="sampleRate">The sample date of the signal.</param>
        /// 
        public Signal(int channels, int length, int sampleRate, SampleFormat format)
        {
            int sampleSize = GetSampleSize(format) / 8;
            byte[] data = new byte[channels * length * sampleSize];

            init(data, channels, length, sampleRate, format);
        }

        private void init(byte[] data, int channels, int length, int sampleRate, SampleFormat format)
        {
            this.handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            this.ptrData = handle.AddrOfPinnedObject();

            this.rawData = data;
            this.sampleRate = sampleRate;
            this.format = format;
            this.length = length;
            this.channels = channels;
        }



        /// <summary>
        ///   Computes the signal energy.
        /// </summary>
        /// 
        public unsafe double GetEnergy()
        {
            double e = 0, v;

            if (format != SampleFormat.Format128BitComplex)
            {
                // Iterate over all samples and compute energy
                float* src = (float*)this.ptrData.ToPointer();
                for (int i = 0; i < this.Samples; i++, src++)
                {
                    v = (*src);
                    e += v * v;
                }
            }
            else
            {
                // Iterate over all samples and compute energy
                Complex* src = (Complex*)this.Data.ToPointer();
                for (int i = 0; i < this.Samples; i++, src++)
                    e += (*src).SquaredMagnitude;
            }

            return e;
        }

        /// <summary>
        ///   Gets the value of the specified sample in the Signal.
        /// </summary>
        /// 
        /// <param name="channel">The channel's index of the sample to set.</param>
        /// <param name="position">The position of the sample to set.</param>
        /// <returns>A floating-point value ranging from -1 to 1 representing
        ///   the retrieved value. Conversion is performed automatically from
        ///   the underlying signal sample format if supported.</returns>
        ///   
        public unsafe float GetSample(int channel, int position)
        {
            void* ptr = ptrData.ToPointer();
            int pos = position * Channels + channel;

            switch (format)
            {
                case SampleFormat.Format32BitIeeeFloat:
                    return ((float*)ptr)[pos];

                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        ///   Sets the value of the specified sample in the Signal.
        /// </summary>
        /// 
        /// <param name="channel">The channel's index of the sample to set.</param>
        /// <param name="position">The position of the sample to set.</param>
        /// <param name="value">A floating-point value ranging from -1 to 1
        ///   specifying the value to set. Conversion will be done automatically
        ///   to the underlying signal sample format if supported.</param>
        ///   
        public unsafe void SetSample(int channel, int position, float value)
        {
            void* ptr = ptrData.ToPointer();
            int pos = position * Channels + channel;

            switch (format)
            {
                case SampleFormat.Format32BitIeeeFloat:
                    ((float*)ptr)[pos] = value;
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        ///   Creates a new Signal from a float array.
        /// </summary>
        /// 
        public static Signal FromArray(Array signal, int sampleRate,
            SampleFormat format = SampleFormat.Format32BitIeeeFloat)
        {
            int channels = signal.Rank == 1 ? 1 : signal.GetLength(1);
            return FromArray(signal, channels, sampleRate, format);
        }

        /// <summary>
        ///   Converts this signal to a ComplexSignal object.
        /// </summary>
        /// 
        public ComplexSignal ToComplex()
        {
            if (format == SampleFormat.Format128BitComplex)
                return new ComplexSignal(rawData, channels, length, sampleRate);
            else return ComplexSignal.FromSignal(this);
        }

        /// <summary>
        ///   Creates a new Signal from a float array.
        /// </summary>
        /// 
        public static Signal FromArray(Array signal, int channels, int sampleRate,
            SampleFormat format = SampleFormat.Format32BitIeeeFloat)
        {
            return FromArray(signal, signal.Length, channels, sampleRate, format);
        }

        /// <summary>
        ///   Creates a new Signal from a float array.
        /// </summary>
        /// 
        public static Signal FromArray(Array signal, int size, int channels, int sampleRate,
            SampleFormat format = SampleFormat.Format32BitIeeeFloat)
        {
            int sampleSize = GetSampleSize(format) / 8;

            byte[] buffer = new byte[size * sampleSize];
            Buffer.BlockCopy(signal, 0, buffer, 0, buffer.Length);
            int samples = size / channels;

            return new Signal(buffer, channels, samples, sampleRate, format);
        }

        /// <summary>
        ///   Copies this signal to a given array.
        /// </summary>
        /// 
        public void CopyTo(Array array)
        {
            Buffer.BlockCopy(rawData, 0, array, 0, array.Length);
        }

        /// <summary>
        ///   Copies this signal to a given array.
        /// </summary>
        /// 
        public void CopyTo(float[] array)
        {
            if (format == Audio.SampleFormat.Format32BitIeeeFloat)
            {
                Buffer.BlockCopy(rawData, 0, array, 0, rawData.Length);
            }

            else if (format == Audio.SampleFormat.Format16Bit)
            {
                short[] source = new short[Samples];
                Buffer.BlockCopy(rawData, 0, source, 0, rawData.Length);
                SampleConverter.Convert(source, array);
            }

            else
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        ///   Copies this signal to a given array.
        /// </summary>
        /// 
        public void CopyTo(double[] array)
        {
            if (format == Audio.SampleFormat.Format64BitIeeeFloat)
            {
                Buffer.BlockCopy(rawData, 0, array, 0, rawData.Length);
            }
            else if (format == Audio.SampleFormat.Format32BitIeeeFloat)
            {
                float[] source = new float[Samples];
                Buffer.BlockCopy(rawData, 0, source, 0, rawData.Length);
                for (int i = 0; i < source.Length; i++)
                    array[i] = source[i];
            }
            else if (format == Audio.SampleFormat.Format16Bit)
            {
                short[] source = new short[Samples];
                Buffer.BlockCopy(rawData, 0, source, 0, rawData.Length);
                SampleConverter.Convert(source, array);
            }

            else
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        ///   Converts this signal into a array of floating-point samples.
        /// </summary>
        /// 
        /// <returns>An array of single-precision floating-point samples.</returns>
        /// 
        public float[] ToFloat()
        {
            float[] array = new float[Samples];
            CopyTo(array);
            return array;
        }

        /// <summary>
        ///   Converts this signal into a array of floating-point samples.
        /// </summary>
        /// 
        /// <returns>An array of single-precision floating-point samples.</returns>
        /// 
        public double[] ToDouble()
        {
            double[] array = new double[Samples];
            CopyTo(array);
            return array;
        }


        #region Static auxiliary methods

        /// <summary>
        ///   Gets the number of samples contained in a signal of given duration and sampling rate.
        /// </summary>
        /// 
        public static int NumberOfSamples(int duration, int samplingRate)
        {
            return (duration / 1000) * samplingRate;
        }

        /// <summary>
        ///   Gets the duration of each sample in a signal with the given number of samples and sampling rate.
        /// </summary>
        /// 
        public static int DurationOfSamples(int samples, int samplingRate)
        {
            return (int)(samples / (double)samplingRate * 1000.0);
        }

        /// <summary>
        ///   Gets the size (in bits) of a sample format.
        /// </summary>
        /// 
        public static int GetSampleSize(SampleFormat format)
        {
            switch (format)
            {
                case SampleFormat.Format128BitComplex:
                    return 128;

                case SampleFormat.Format32BitIeeeFloat:
                    return 32;

                case SampleFormat.Format64BitIeeeFloat:
                    return 64;

                case SampleFormat.Format16Bit:
                    return 16;

                case SampleFormat.Format32Bit:
                    return 32;

                case SampleFormat.Format8Bit:
                    return 8;

                case SampleFormat.Format8BitUnsigned:
                    return 8;

                default:
                    throw new NotSupportedException();
            }
        }
        #endregion


        #region IDisposable Members

        /// <summary>
        ///   Performs application-defined tasks associated with freeing, 
        ///   releasing, or resetting unmanaged resources.
        /// </summary>
        /// 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///   Releases unmanaged resources and performs other cleanup operations 
        ///   before the <see cref="Signal"/> is reclaimed by garbage collection.
        /// </summary>
        /// 
        ~Signal()
        {
            Dispose(false);
        }

        /// <summary>
        ///   Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// 
        /// <param name="disposing"><c>true</c> to release both managed
        /// and unmanaged resources; <c>false</c> to release only unmanaged
        /// resources.</param>
        ///
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
            }

            // free native resources
            if (handle.IsAllocated)
            {
                handle.Free();
                ptrData = IntPtr.Zero;
            }
        }

        #endregion


    }
}
