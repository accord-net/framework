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

namespace Accord.Audio.Formats
{
    using System;
    using System.IO;
    using Accord.Audio;
    using SlimDX.Multimedia;

    /// <summary>
    ///   Wave audio file decoder.
    /// </summary>
    /// 
    /// <example>
    /// <code>
    /// // Let's decode an Wave audio file
    /// UnmanagedMemoryStream sourceStream = ...
    /// 
    /// // Create a decoder for the source stream
    /// WaveDecoder sourceDecoder = new WaveDecoder(sourceStream);
    /// 
    /// // At this point, we can query some properties of the audio file:
    /// int channels =  sourceDecoder.Channels;
    /// int samples  =  sourceDecoder.Samples;
    /// int frames   =  sourceDecoder.Frames;
    /// int duration =  sourceDecoder.Duration;
    /// int rate     =  sourceDecoder.SampleRate;
    /// int bps      =  sourceDecoder.BitsPerSample;
    /// 
    /// // Decode the signal in the source stream
    /// Signal sourceSignal = sourceDecoder.Decode();
    /// </code>
    /// </example>
    /// 
    public class WaveDecoder : IAudioDecoder
    {
        private WaveStream waveStream;

        private int blockAlign;
        private int channels;
        private int numberOfFrames;
        private int numberOfSamples;
        private int duration;
        private int sampleRate;
        private int bytes;
        private int bitsPerSample;
        private int averageBitsPerSecond;

        private float[] buffer;
        private int bufferSize;

        /// <summary>
        ///   Gets the current frame within
        ///   the current decoder stream.
        /// </summary>
        /// 
        public int Position
        {
            get { return (int)(waveStream.Position / blockAlign); }
        }

        /// <summary>
        ///   Gets the number of channels of
        ///   the underlying Wave stream.
        /// </summary>
        /// 
        public int Channels
        {
            get { return channels; }
        }

        /// <summary>
        ///   Gets the number of frames of
        ///   the underlying Wave stream.
        /// </summary>
        /// 
        public int Frames
        {
            get { return numberOfFrames; }
        }

        /// <summary>
        ///   Gets the number of samples of
        ///   the underlying Wave stream.
        /// </summary>
        /// 
        public int Samples
        {
            get { return numberOfSamples; }
        }

        /// <summary>
        ///   Gets the sample rate for
        ///   the underlying Wave stream.
        /// </summary>
        /// 
        public int SampleRate
        {
            get { return sampleRate; }
        }

        /// <summary>
        ///   Gets the underlying Wave stream.
        /// </summary>
        /// 
        public Stream Stream
        {
            get { return waveStream; }
        }

        /// <summary>
        ///   Gets the total number of bytes
        ///   read by this Wave encoder.
        /// </summary>
        /// 
        public int Bytes
        {
            get { return bytes; }
        }

        /// <summary>
        ///   Gets the total time span duration (in
        ///   milliseconds) read by this encoder.
        /// </summary>
        /// 
        public int Duration
        {
            get { return duration; }
        }

        /// <summary>
        ///   Gets the average bits per second
        ///   of the underlying Wave stream.
        /// </summary>
        /// 
        public int AverageBitsPerSecond
        {
            get { return averageBitsPerSecond; }
        }

        /// <summary>
        ///   Gets the bits per sample of
        ///   the underlying Wave stream.
        /// </summary>
        /// 
        public int BitsPerSample
        {
            get { return bitsPerSample; }
        }



        #region Constructors

        /// <summary>
        ///   Constructs a new Wave decoder.
        /// </summary>
        public WaveDecoder()
        {
        }

        /// <summary>
        ///   Constructs a new Wave decoder.
        /// </summary>
        public WaveDecoder(Stream stream)
        {
            Open(stream);
        }

        /// <summary>
        ///   Constructs a new Wave decoder.
        /// </summary>
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
        public int Open(WaveStream stream)
        {
            this.waveStream = stream;
            this.channels = stream.Format.Channels;
            this.blockAlign = stream.Format.BlockAlignment;
            this.numberOfFrames = (int)stream.Length / blockAlign;
            this.sampleRate = stream.Format.SamplesPerSecond;
            this.numberOfSamples = numberOfFrames * Channels;
            this.duration = (int)(numberOfFrames / (double)sampleRate * 1000.0);
            this.bitsPerSample = stream.Format.BitsPerSample;
            this.averageBitsPerSecond = stream.Format.AverageBytesPerSecond / 8;

            return numberOfFrames;
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
            return Open(new WaveStream(stream));
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
            return Open(new WaveStream(path));
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
        ///   Decodes the Wave stream into a Signal object.
        /// </summary>
        /// 
        public Signal Decode()
        {
            // Reads the entire stream into a signal
            return Decode(0, Frames);
        }



        /// <summary>
        ///   Decodes the Wave stream into a Signal object.
        /// </summary>
        /// 
        /// <param name="frames">The number of frames to decode.</param>
        /// 
        public Signal Decode(int frames)
        {
            if (waveStream.Position == waveStream.Length)
                return null;

            bufferSize = Channels * frames;

            // Create room to store the samples.
            if (buffer == null || buffer.Length < bufferSize)
                buffer = new float[bufferSize];

            bytes = readAsFloat(buffer, frames);

            // The decoder always decodes as 32-bit IEEE Float.
            return Signal.FromArray(buffer, bufferSize, channels, sampleRate, SampleFormat.Format32BitIeeeFloat);
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
            waveStream.Seek(index * channels, SeekOrigin.Begin);
            return Decode(frames);
        }

        /// <summary>
        ///   Closes the underlying stream.
        /// </summary>
        /// 
        public void Close()
        {
            waveStream.Close();
        }



        private int readAsFloat(float[] buffer, int count)
        {
            int reads = 0;

            // Detect the underlying stream format.
            if (waveStream.Format.FormatTag == WaveFormatTag.Pcm)
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
            else if (waveStream.Format.FormatTag == WaveFormatTag.IeeeFloat)
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

            int blockSize = sizeof(float) * count * channels;
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

            int blockSize = sizeof(short) * count * channels;
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

            int blockSize = sizeof(int) * count * channels;
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
