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

namespace Accord.Audio.Formats
{
    using System;
    using System.IO;
    using Accord.Audio;
    using SharpDX.Multimedia;
    using System.Diagnostics;

    /// <summary>
    ///   Wave audio file decoder.
    /// </summary>
    /// 
    /// <example>
    /// <code source="Unit Tests\Accord.Tests.Audio\WaveEncoderTest.cs" region="doc_properties" />
    /// </example>
    /// 
    /// <seealso cref="WaveEncoder"/>
    /// <seealso cref="Signal"/>
    /// 
    [FormatDecoder("wav")]
    public class WaveDecoder : IAudioDecoder
    {
        private SoundStream waveStream;

        private int blockAlign;
        private int numberOfChannels;
        private int totalNumberOfFrames;
        private int totalNumberOfSamples;
        private int duration;
        private int sampleRate;
        private int bytesRead;
        private int bitsPerSample;
        private int averageBitsPerSecond;

        private int bufferSize;

        /// <summary>
        ///   Gets the current frame within the current decoder stream.
        /// </summary>
        /// 
        public int Position
        {
            get { return (int)(waveStream.Position / blockAlign); }
        }

        /// <summary>
        ///   Obsolete. Please use <see cref="NumberOfChannels"/> instead.
        /// </summary>
        /// 
        [Obsolete("Please use NumberOfChannels instead.")]
        public int Channels { get { return numberOfChannels; } }


        /// <summary>
        ///   Gets the number of channels of the underlying Wave stream.
        /// </summary>
        /// 
        /// <example>
        /// <code source="Unit Tests\Accord.Tests.Audio\WaveEncoderTest.cs" region="doc_properties" />
        /// </example>
        /// 
        public int NumberOfChannels { get { return numberOfChannels; } }

        /// <summary>
        ///   Obsolete. Please use <see cref="NumberOfFrames"/> instead.
        /// </summary>
        /// 
        [Obsolete("Please use NumberOfFrames instead.")]
        public int Frames { get { return totalNumberOfFrames; } }

        /// <summary>
        ///   Gets the number of frames of the underlying Wave stream.
        /// </summary>
        /// 
        /// <example>
        /// <code source="Unit Tests\Accord.Tests.Audio\WaveEncoderTest.cs" region="doc_properties" />
        /// </example>
        /// 
        public int NumberOfFrames { get { return totalNumberOfFrames; } }

        /// <summary>
        ///   Obsolete. Please use <see cref="NumberOfSamples"/> instead.
        /// </summary>
        /// 
        [Obsolete("Please use NumberOfSamples instead.")]
        public int Samples { get { return totalNumberOfSamples; } }

        /// <summary>
        ///   Gets the number of samples of the underlying Wave stream.
        /// </summary>
        /// 
        /// <example>
        /// <code source="Unit Tests\Accord.Tests.Audio\WaveEncoderTest.cs" region="doc_properties" />
        /// </example>
        /// 
        public int NumberOfSamples { get { return totalNumberOfSamples; } }

        /// <summary>
        ///   Gets the sample rate for the underlying Wave stream.
        /// </summary>
        /// 
        /// <example>
        /// <code source="Unit Tests\Accord.Tests.Audio\WaveEncoderTest.cs" region="doc_properties" />
        /// </example>
        /// 
        public int SampleRate { get { return sampleRate; } }

        /// <summary>
        ///   Gets the underlying Wave stream.
        /// </summary>
        /// 
        public Stream Stream
        {
            get { return waveStream; }
        }

        /// <summary>
        ///   Obsolete. Please use <see cref="NumberOfBytesRead"/> instead.
        /// </summary>
        /// 
        [Obsolete("Please use NumberOfBytesRead instead.")]
        public int Bytes
        {
            get { return bytesRead; }
        }

        /// <summary>
        ///   Gets the total number of bytes read by this Wave decoder.
        /// </summary>
        /// 
        /// <example>
        /// <code source="Unit Tests\Accord.Tests.Audio\WaveEncoderTest.cs" region="doc_properties" />
        /// </example>
        /// 
        public int NumberOfBytesRead
        {
            get { return bytesRead; }
        }

        /// <summary>
        ///   Gets the total time span duration (in milliseconds) read by this encoder.
        /// </summary>
        /// 
        /// <example>
        /// <code source="Unit Tests\Accord.Tests.Audio\WaveEncoderTest.cs" region="doc_properties" />
        /// </example>
        /// 
        public int Duration
        {
            get { return duration; }
        }

        /// <summary>
        ///   Gets the average bits per second of the underlying Wave stream.
        /// </summary>
        /// 
        /// <example>
        /// <code source="Unit Tests\Accord.Tests.Audio\WaveEncoderTest.cs" region="doc_properties" />
        /// </example>
        /// 
        public int AverageBitsPerSecond
        {
            get { return averageBitsPerSecond; }
        }

        /// <summary>
        ///   Gets the bits per sample of the underlying Wave stream.
        /// </summary>
        /// 
        /// <example>
        /// <code source="Unit Tests\Accord.Tests.Audio\WaveEncoderTest.cs" region="doc_properties" />
        /// </example>
        /// 
        public int BitsPerSample
        {
            get { return bitsPerSample; }
        }



        #region Constructors

        /// <summary>
        ///   Constructs a new Wave decoder.
        /// </summary>
        /// 
        public WaveDecoder()
        {
        }

        /// <summary>
        ///   Constructs a new Wave decoder.
        /// </summary>
        /// 
        /// <param name="stream">Stream to open.</param>
        /// 
        public WaveDecoder(Stream stream)
        {
            Open(stream);
        }

        /// <summary>
        ///   Constructs a new Wave decoder.
        /// </summary>
        /// 
        /// <param name="path">Path of file to open as stream.</param>
        /// 
        public WaveDecoder(string path)
        {
            Open(path);
        }

        #endregion


        /// <summary>
        ///   Opens the specified stream.
        /// </summary>
        /// 
        /// <param name="stream">Stream to open.</param>
        /// 
        /// <returns>Returns number of frames found in the specified stream.</returns>
        /// 
        public int Open(SoundStream stream)
        {
            this.waveStream = stream;
            this.numberOfChannels = stream.Format.Channels;
            this.blockAlign = stream.Format.BlockAlign;
            this.totalNumberOfFrames = (int)stream.Length / blockAlign;
            this.sampleRate = stream.Format.SampleRate;
            this.totalNumberOfSamples = totalNumberOfFrames * this.numberOfChannels;
            this.duration = (int)(totalNumberOfFrames / (double)sampleRate * 1000.0);
            this.bitsPerSample = stream.Format.BitsPerSample;
            this.averageBitsPerSecond = stream.Format.AverageBytesPerSecond * 8;

            return totalNumberOfFrames;
        }

        /// <summary>
        ///   Open specified stream.
        /// </summary>
        /// 
        /// <param name="stream">Stream to open.</param>
        /// 
        /// <returns>Returns number of frames found in the specified stream.</returns>
        /// 
        public int Open(Stream stream)
        {
            return Open(new SoundStream(stream));
        }

        /// <summary>
        ///   Open specified stream.
        /// </summary>
        /// 
        /// <param name="path">Path of file to open as stream.</param>
        /// 
        /// <returns>Returns number of frames found in the specified stream.</returns>
        /// 
        public int Open(string path)
        {
            return Open(new SoundStream(new FileStream(path, FileMode.Open, FileAccess.Read)));
        }

        /// <summary>
        ///   Navigates to a position in this Wave stream.
        /// </summary>
        /// 
        /// <param name="frameIndex">The index of the sample to navigate to.</param>
        /// 
        public void Seek(int frameIndex)
        {
            waveStream.Position = frameIndex * blockAlign;
        }

        /// <summary>
        ///   Decodes the entire Wave stream into a new Signal object.
        /// </summary>
        /// 
        public Signal Decode()
        {
            // Reads the entire stream into a signal
            return Decode(index: 0, frames: totalNumberOfFrames);
        }

        /// <summary>
        ///   Decodes the entire Wave stream into a new Signal object.
        /// </summary>
        /// 
        public Signal Decode(int frames)
        {
            // Reads the entire stream into a signal
            return Decode(index: 0, frames: frames);
        }

        /// <summary>
        ///   Decodes a number of frames into a Signal object, reusing memory from an
        ///   existing signal if possible (that may be overwritten with the new data).
        /// </summary>
        /// 
        /// <param name="signal">The existing audio signal to be overwritten. If set to 
        ///   <c>null</c>, a new <see cref="Signal"/> will be created instead.</param>
        /// 
        /// <returns>Returns the decoded signal.</returns>
        /// 
        /// <remarks>Implementations of this method may throw
        /// <see cref="System.NullReferenceException"/> exception in the case if no audio
        /// stream was opened previously, <see cref="System.ArgumentOutOfRangeException"/> in the
        /// case if stream does not contain frame with specified index or  <see cref="System.ArgumentException"/>
        /// exception to report about incorrectly formatted audio.
        /// </remarks>
        /// 
        public Signal Decode(Signal signal)
        {
            return Decode(frames: totalNumberOfFrames, signal: signal);
        }

        /// <summary>
        ///   Decodes a number of frames into a Signal object, reusing memory from an
        ///   existing signal if possible (that may be overwritten with the new data).
        /// </summary>
        /// 
        /// <param name="frames">The number of frames to decode.</param>
        /// <param name="signal">The existing audio signal to be overwritten. If set to 
        ///   <c>null</c>, a new <see cref="Signal"/> will be created instead.</param>
        /// 
        /// <returns>Returns the decoded signal.</returns>
        /// 
        /// <remarks>Implementations of this method may throw
        /// <see cref="System.NullReferenceException"/> exception in the case if no audio
        /// stream was opened previously, <see cref="System.ArgumentOutOfRangeException"/> in the
        /// case if stream does not contain frame with specified index or  <see cref="System.ArgumentException"/>
        /// exception to report about incorrectly formatted audio.
        /// </remarks>
        /// 
        public Signal Decode(int frames, Signal signal)
        {
            if (waveStream.Position == waveStream.Length)
                return null;

            float[] buffer;
            bufferSize = numberOfChannels * frames;

            if (signal != null)
            {
                // Check if we have enough room to store the samples
                if (signal.NumberOfFrames != frames || signal.NumberOfChannels != numberOfChannels || signal.SampleFormat != SampleFormat.Format32BitIeeeFloat)
                    throw new ArgumentException("The signal does not have the correct number of channels, frames or pixel format.");

                buffer = (float[])signal.InnerData;
                Debug.Assert(buffer.Length == bufferSize);
            }
            else
            {
                buffer = new float[bufferSize];

                // This WaveDecoder class always decodes samples as 32-bit IEEE Float.
                signal = Signal.FromArray(buffer, bufferSize,
                    numberOfChannels, sampleRate, SampleFormat.Format32BitIeeeFloat);
            }

            bytesRead = readAsFloat(buffer, signal.NumberOfFrames);

            return signal;
        }

        /// <summary>
        /// Decodes the Wave stream into a Signal object.
        /// </summary>
        /// 
        /// <param name="index">Audio frame index to start decoding.</param>
        /// <param name="frames">The number of frames to decode.</param>
        /// 
        /// <returns>Returns the decoded signal.</returns>
        /// 
        public Signal Decode(int index, int frames)
        {
            return Decode(index: index, frames: frames, signal: null);
        }

        /// <summary>
        ///   Decodes a number of frames into a Signal object, reusing memory from an
        ///   existing signal if possible (that may be overwritten with the new data).
        /// </summary>
        /// 
        /// 
        /// <param name="index">Audio frame index to start decoding.</param>
        /// <param name="frames">The number of frames to decode.</param>
        /// <param name="signal">The existing audio signal to be overwritten. If set to 
        ///   <c>null</c>, a new <see cref="Signal"/> will be created instead.</param>
        /// 
        /// <returns>Returns the decoded signal.</returns>
        /// 
        /// <remarks>Implementations of this method may throw
        /// <see cref="System.NullReferenceException"/> exception in the case if no audio
        /// stream was opened previously, <see cref="System.ArgumentOutOfRangeException"/> in the
        /// case if stream does not contain frame with specified index or  <see cref="System.ArgumentException"/>
        /// exception to report about incorrectly formatted audio.
        /// </remarks>
        /// 
        public Signal Decode(int index, int frames, Signal signal)
        {
            waveStream.Seek(index * numberOfChannels, SeekOrigin.Begin);
            return Decode(frames: frames, signal: null);
        }

        /// <summary>
        ///   Closes the underlying stream.
        /// </summary>
        /// 
        public void Close()
        {
#if !NETSTANDARD1_4
            waveStream.Close();
#endif
        }



        private int readAsFloat(float[] buffer, int count)
        {
            int reads = 0;

            // Detect the underlying stream format.
            if (waveStream.Format.Encoding == WaveFormatEncoding.Pcm)
            {
                // The wave is in standard PCM format. We'll need
                //  to convert it to IeeeFloat.
                switch (waveStream.Format.BitsPerSample)
                {
                    case 8: // Stream is 8 bits
                        {
                            byte[] block = new byte[bufferSize];
                            reads = read(block, count);
                            SampleConverter.Convert(block, buffer);
                        }
                        break;

                    case 16: // Stream is 16 bits
                        {
                            short[] block = new short[bufferSize];
                            reads = read(block, count);
                            SampleConverter.Convert(block, buffer);
                        }
                        break;

                    case 32: // Stream is 32 bits
                        {
                            int[] block = new int[bufferSize];
                            reads = read(block, count);
                            SampleConverter.Convert(block, buffer);
                        }
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }
            else if (waveStream.Format.Encoding == WaveFormatEncoding.IeeeFloat)
            {
                // Format is 16-bit IEEE float,
                // just copy to the buffer.
                reads = read(buffer, count);
            }
            else
            {
                throw new NotSupportedException("The wave format isn't supported");
            }

            return reads; // return the number of bytes read
        }


        /// <summary>
        ///   Reads a maximum of count samples from the current stream, and writes the data to buffer, beginning at index.
        /// </summary>
        /// <param name="buffer">
        ///    When this method returns, this parameter contains the specified byte array with the values between index and (index + count -1) replaced by the 8 bit frames read from the current source.
        /// </param>
        /// <param name="count">The amount of frames to read.</param>
        /// <returns>The number of reads performed on the stream.</returns>
        private int read(float[] buffer, int count)
        {
            int reads;

            int blockSize = sizeof(float) * count * numberOfChannels;
            byte[] block = new byte[blockSize];
            reads = waveStream.Read(block, 0, blockSize);

            // Convert from byte to float
            for (int j = 0; j < bufferSize; j++)
                buffer[j] = BitConverter.ToSingle(block, j * sizeof(float));

            return reads;
        }

        /// <summary>
        ///   Reads a maximum of count frames from the current stream, and writes the data to buffer, beginning at index.
        /// </summary>
        /// <param name="buffer">
        ///    When this method returns, this parameter contains the specified byte array with the values between index and (index + count -1) replaced by the 8 bit frames read from the current source.
        /// </param>
        /// <param name="count">The amount of frames to read.</param>
        /// <returns>The number of reads performed on the stream.</returns>
        private int read(short[] buffer, int count)
        {
            int reads;

            int blockSize = sizeof(short) * count * numberOfChannels;
            byte[] block = new byte[blockSize];
            reads = waveStream.Read(block, 0, blockSize);

            // Convert from byte to short
            for (int j = 0; j < bufferSize; j++)
                buffer[j] = BitConverter.ToInt16(block, j * sizeof(short));

            return reads;
        }

        /// <summary>
        ///   Reads a maximum of count frames from the current stream, and writes the data to buffer, beginning at index.
        /// </summary>
        /// <param name="buffer">
        ///    When this method returns, this parameter contains the specified byte array with the values between index and (index + count -1) replaced by the 8 bit frames read from the current source.
        /// </param>
        /// <param name="count">The amount of frames to read.</param>
        /// <returns>The number of reads performed on the stream.</returns>
        private int read(int[] buffer, int count)
        {
            int reads;

            int blockSize = sizeof(int) * count * numberOfChannels;
            byte[] block = new byte[blockSize];
            reads = waveStream.Read(block, 0, blockSize);

            // Convert from byte to int
            for (int j = 0; j < bufferSize; j++)
                buffer[j] = BitConverter.ToInt32(block, j * sizeof(int));

            return reads;
        }

        /// <summary>
        ///   Reads a maximum of count frames from the current stream, and writes the data to buffer, beginning at index.
        /// </summary>
        /// <param name="buffer">
        ///    When this method returns, this parameter contains the specified byte array with the values between index and (index + count -1) replaced by the 8 bit frames read from the current source.
        /// </param>
        /// <param name="count">The amount of frames to read.</param>
        /// <returns>The number of reads performed on the stream.</returns>
        private int read(byte[] buffer, int count)
        {
            return waveStream.Read(buffer, 0, count);
        }

    }
}
