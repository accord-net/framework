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
    using Accord.Audio;
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    /// <summary>
    ///   Wave audio file encoder.
    /// </summary>
    /// 
    /// <example>
    /// <code>
    /// // Create a stream to hold our encoded audio
    /// MemoryStream destinationStream = new MemoryStream();
    /// 
    /// // Create a encoder for the destination stream
    /// WaveEncoder encoder = new WaveEncoder(destinationStream);
    /// 
    /// // Encode the signal to the destination stream
    /// encoder.Encode(sourceSignal);
    /// </code>
    /// </example>
    /// 
    public class WaveEncoder : IAudioEncoder
    {
        private Stream waveStream;

        private int bytes;
        private int blockAlign;
        private int channels;
        private int numberOfFrames;
        private int numberOfSamples;
        private int duration;
        private int sampleRate;
        private int bitsPerSample;
        private int averageBitsPerSecond;
        private SampleFormat sampleFormat;


        // The following fields are set when the encoder
        // receives the first signal to be written.

        bool initialized = false;
        DataChunk header = new DataChunk();
        FormatHeader format = new FormatHeader();
        RIFFChunk riff = new RIFFChunk();
        byte[] waveFormat;


        /// <summary>
        ///   Gets the underlying Wave stream.
        /// </summary>
        /// 
        public Stream Stream { get { return waveStream; } }

        /// <summary>
        ///   Gets the number of channels
        ///   of the active Wave stream.
        /// </summary>
        /// 
        public int Channels
        {
            get { return channels; }
        }

        /// <summary>
        ///   Gets the total number of frames
        ///   written by this Wave encoder.
        /// </summary>
        /// 
        public int Frames
        {
            get { return numberOfFrames; }
        }

        /// <summary>
        ///   Gets the total number of samples
        ///   written by this Wave encoder.
        /// </summary>
        /// 
        public int Samples
        {
            get { return numberOfSamples; }
        }

        /// <summary>
        ///   Gets the sample rate of
        ///   the underlying Wave stream.
        /// </summary>
        /// 
        public int SampleRate
        {
            get { return sampleRate; }
        }

        /// <summary>
        ///   Gets the total number of bytes
        ///   written by this Wave encoder.
        /// </summary>
        /// 
        public int Bytes
        {
            get { return bytes; }
        }

        /// <summary>
        ///   Gets the total time span duration (in
        ///   milliseconds) written by this encoder.
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

        /// <summary>
        ///   Gets the sample format used by the encoder.
        /// </summary>
        /// 
        public SampleFormat Format
        {
            get { return sampleFormat; }
        }

        #region Constructors

        /// <summary>
        ///   Constructs a new Wave encoder.
        /// </summary>
        /// 
        public WaveEncoder()
        {
        }

        /// <summary>
        ///   Constructs a new Wave encoder.
        /// </summary>
        /// 
        /// <param name="stream">A file stream to store the encoded data.</param>
        /// 
        public WaveEncoder(FileStream stream)
        {
            Open(stream);
        }

        /// <summary>
        ///   Constructs a new Wave encoder.
        /// </summary>
        /// 
        /// <param name="stream">A stream to store the encoded data.</param>
        ///
        public WaveEncoder(Stream stream)
        {
            Open(stream);
        }

        /// <summary>
        ///   Constructs a new Wave encoder.
        /// </summary>
        /// 
        /// <param name="path">The path to a file to store the encoded data.</param>
        /// 
        public WaveEncoder(string path)
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
        public void Open(FileStream stream)
        {
            this.waveStream = stream;
        }

        /// <summary>
        ///   Open specified stream.
        /// </summary>
        /// 
        /// <param name="stream">Stream to open.</param>
        /// 
        /// <returns>Returns number of frames found in the specified stream.</returns>
        /// 
        public void Open(Stream stream)
        {
            this.waveStream = stream;
        }

        /// <summary>
        ///   Open specified stream.
        /// </summary>
        /// 
        /// <param name="path">Path of file to open as stream.</param>
        /// 
        /// <returns>Returns number of frames found in the specified stream.</returns>
        /// 
        public void Open(string path)
        {
            Open(new FileStream(path, FileMode.OpenOrCreate));
        }

        /// <summary>
        ///   Closes the underlying stream.
        /// </summary>
        /// 
        public void Close()
        {
            waveStream.Close();
        }

        /// <summary>
        ///   Encodes the Wave stream into a Signal object.
        /// </summary>
        /// 
        public void Encode(Signal signal)
        {
            if (!initialized)
            {
                initialize(signal);
                firstWriteHeaders();
            }

            // Update counters
            numberOfSamples += signal.Samples;
            numberOfFrames += signal.Length;
            bytes += signal.RawData.Length;
            duration += signal.Duration;

            // Navigate to start position
            waveStream.Seek(0, SeekOrigin.Begin);

            // Update headers
            updateHeaders();

            // Go back to previous position
            waveStream.Seek(0, SeekOrigin.End);

            // Write the current signal data
            waveStream.Write(signal.RawData, 0, signal.RawData.Length);
        }


        private void updateHeaders()
        {
            header.Length = this.bytes;
            byte[] dataHeader = header.GetBytes();

            riff.Length = this.bytes + dataHeader.Length + waveFormat.Length;
            byte[] riffHeader = riff.GetBytes();

            waveStream.Write(riffHeader, 0, riffHeader.Length);
            waveStream.Write(waveFormat, 0, waveFormat.Length);
            waveStream.Write(dataHeader, 0, dataHeader.Length);
        }


        private void firstWriteHeaders()
        {
            // Create data header
            header.Header = new char[] { 'd', 'a', 't', 'a' };
            header.Length = this.bytes;
            byte[] dataHeader = header.GetBytes();

            // Create Wave format header
            format.FmtHeader = new char[] { 'f', 'm', 't', ' ' };
            format.Length = 16;
            format.Channels = (short)this.channels;
            format.FormatTag = (short)sampleFormat.ToWaveFormat();
            format.SamplesPerSecond = sampleRate;
            format.BitsPerSample = (short)this.bitsPerSample;
            format.BlockAlignment = (short)this.blockAlign;
            format.AverageBytesPerSecond = this.averageBitsPerSecond;
            waveFormat = format.GetBytes();

            // Create RIFF header
            riff.RiffHeader = new char[] { 'R', 'I', 'F', 'F' };
            riff.WaveHeader = new char[] { 'W', 'A', 'V', 'E' };
            riff.Length = this.bytes + dataHeader.Length + waveFormat.Length;
            byte[] riffHeader = riff.GetBytes();

            // Write headers to allocate space
            waveStream.Write(riffHeader, 0, riffHeader.Length);
            waveStream.Write(waveFormat, 0, waveFormat.Length);
            waveStream.Write(dataHeader, 0, dataHeader.Length);
        }

        private void initialize(Signal signal)
        {
            this.channels = signal.Channels;
            this.sampleRate = signal.SampleRate;
            this.sampleFormat = signal.SampleFormat;
            this.bitsPerSample = Signal.GetSampleSize(signal.SampleFormat);
            this.blockAlign = (bitsPerSample / 8) * channels;
            this.averageBitsPerSecond = sampleRate * blockAlign;

            this.initialized = true;
        }

    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    internal struct FormatHeader
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public char[] FmtHeader;

        public int Length;

        public short FormatTag;

        public short Channels;

        public int SamplesPerSecond;

        public int AverageBytesPerSecond;

        public short BlockAlignment;

        public short BitsPerSample;

        public byte[] GetBytes()
        {
            int rawsize = Marshal.SizeOf(this);
            byte[] rawdata = new byte[rawsize];
            GCHandle handle = GCHandle.Alloc(rawdata, GCHandleType.Pinned);
            IntPtr buffer = handle.AddrOfPinnedObject();
            Marshal.StructureToPtr(this, buffer, false);
            handle.Free();
            return rawdata;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    internal struct RIFFChunk
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public char[] RiffHeader;

        public int Length;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public char[] WaveHeader;

        public byte[] GetBytes()
        {
            int rawsize = Marshal.SizeOf(this);
            byte[] rawdata = new byte[rawsize];
            GCHandle handle = GCHandle.Alloc(rawdata, GCHandleType.Pinned);
            IntPtr buffer = handle.AddrOfPinnedObject();
            Marshal.StructureToPtr(this, buffer, false);
            handle.Free();
            return rawdata;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    internal struct DataChunk
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]

        public char[] Header;

        public int Length;

        public byte[] GetBytes()
        {
            int rawsize = Marshal.SizeOf(this);
            byte[] rawdata = new byte[rawsize];
            GCHandle handle = GCHandle.Alloc(rawdata, GCHandleType.Pinned);
            IntPtr buffer = handle.AddrOfPinnedObject();
            Marshal.StructureToPtr(this, buffer, false);
            handle.Free();
            return rawdata;
        }
    }
}
