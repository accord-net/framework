// Accord Unit Tests
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

namespace Accord.Tests.Audio
{
    using Accord.DirectSound;
    using NUnit.Framework;
    using Accord.Audio;
    using Accord.Audio.Formats;
    using System.IO;
    using Accord.Math;
    using System;

    [TestFixture]
    public class WaveEncoderTest
    {

        [Test]
        public void WaveEncoderConstructorTest()
        {
            string basePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources");

            #region doc_properties
            // Let's say we would like to decode a wave file: 
            string fileName = Path.Combine(basePath, "a.wav");

            // File is in PCM 16bpp format:
            // Number of samples: 352.800
            // Number of frames:  176.400
            // Sample rate:        44.100 Hz
            // Block align:         4
            // Channels:            2
            // Duration:            4.0   s
            // Bytes:             705.644 
            // Bitrate:           1411kbps

            // First, create a decoder for the file stream:
            var sourceDecoder = new WaveDecoder(fileName);

            // Now we can use it to check some of the the stream properties:
            int numberOfChannels = sourceDecoder.NumberOfChannels; // 2
            int numberOfSamples = sourceDecoder.NumberOfSamples;   // 352800
            int numberOfFrames = sourceDecoder.NumberOfFrames;     // 176400
            int durationMilliseconds = sourceDecoder.Duration;     // 4000
            int sampleRate = sourceDecoder.SampleRate;             // 44100
            int bitsPerSample = sourceDecoder.BitsPerSample;       // 16
            int bps = sourceDecoder.AverageBitsPerSecond;          // 141200
            
            // Decode the signal in the source stream:
            Signal sourceSignal = sourceDecoder.Decode();

            // As we can see, all properties are kept in the signal:
            int signalChannels = sourceSignal.NumberOfChannels; // 2
            int signalSamples = sourceSignal.NumberOfSamples;   // 352800
            int signalFrames = sourceSignal.NumberOfFrames;     // 176400
            TimeSpan signalDuration = sourceSignal.Duration;    // {00:00:04}
            int signalLength = sourceSignal.Length;             // 176400
            int signalSampleRate = sourceSignal.SampleRate;     // 44100
            int signalBytes = sourceSignal.NumberOfBytes;       // 1411200

            // And this is the total number of bytes that have been read:
            int numberOfBytes = sourceDecoder.NumberOfBytesRead; // 705600
            #endregion


            Assert.AreEqual(2, numberOfChannels);
            Assert.AreEqual(352800, numberOfSamples);
            Assert.AreEqual(176400, numberOfFrames);
            Assert.AreEqual(4000, durationMilliseconds);
            Assert.AreEqual(44100, sampleRate);
            Assert.AreEqual(16, bitsPerSample);
            Assert.AreEqual(1411200, bps);
            Assert.AreEqual(sizeof(short) * 352800, numberOfBytes);

            Assert.AreEqual(352800, signalSamples);
            Assert.AreEqual(176400, signalLength);
            Assert.AreEqual(4000, signalDuration.TotalMilliseconds);
            Assert.AreEqual(2, signalChannels);
            Assert.AreEqual(44100, signalSampleRate);
            Assert.AreEqual(sizeof(float) * 352800, signalBytes);

            MemoryStream destinationStream = new MemoryStream();

            // Create a encoder for the destination stream
            WaveEncoder encoder = new WaveEncoder(destinationStream);

            // Encode the signal to the destination stream
            encoder.Encode(sourceSignal);

            Assert.AreEqual(2, encoder.Channels);
            Assert.AreEqual(352800, encoder.Samples);
            Assert.AreEqual(176400, encoder.Frames);
            Assert.AreEqual(4000, encoder.Duration);
            Assert.AreEqual(44100, encoder.SampleRate);
            Assert.AreEqual(32, encoder.BitsPerSample);
            Assert.AreEqual(sizeof(float) * 352800, encoder.Bytes);


            // Rewind both streams, them attempt to read the destination
            destinationStream.Seek(0, SeekOrigin.Begin);

            // Create a decoder to read the destination stream
            WaveDecoder destDecoder = new WaveDecoder(destinationStream);
            Assert.AreEqual(2, destDecoder.Channels);
            Assert.AreEqual(176400, destDecoder.Frames);
            Assert.AreEqual(352800, destDecoder.Samples);
            Assert.AreEqual(4000, destDecoder.Duration);
            Assert.AreEqual(44100, destDecoder.SampleRate);
            Assert.AreEqual(32, destDecoder.BitsPerSample);
            Assert.AreEqual(1411200, sourceDecoder.AverageBitsPerSecond);


            // Decode the destination stream
            Signal destSignal = destDecoder.Decode();

            // Assert that the signal which has been saved to the destination
            // stream and the signal which has just been read from this same
            // stream are identical
            Assert.AreEqual(sourceSignal.Length, destSignal.Length);
            Assert.AreEqual(sourceSignal.SampleFormat, destSignal.SampleFormat);
            Assert.AreEqual(sourceSignal.SampleRate, destSignal.SampleRate);
            Assert.AreEqual(sourceSignal.Samples, destSignal.Samples);
            Assert.AreEqual(sourceSignal.Duration, destSignal.Duration);

            byte[] actual = sourceSignal.ToByte();
            byte[] expected = destSignal.ToByte();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SignalLoadSave()
        {
            string input = Path.Combine(NUnit.Framework.TestContext.CurrentContext.TestDirectory, "Resources", "a.wav");

            Signal sourceSignal = Signal.FromFile(input);
            Assert.AreEqual(352800, sourceSignal.Samples);
            Assert.AreEqual(sourceSignal.Samples, sourceSignal.NumberOfSamples);
            Assert.AreEqual(176400, sourceSignal.Length);
            Assert.AreEqual(sourceSignal.Length, sourceSignal.NumberOfFrames);
            Assert.AreEqual(4000, sourceSignal.Duration.TotalMilliseconds);
            Assert.AreEqual(2, sourceSignal.Channels);
            Assert.AreEqual(44100, sourceSignal.SampleRate);
            Assert.AreEqual(sizeof(float) * 352800, sourceSignal.NumberOfBytes);

            string output = Path.Combine(NUnit.Framework.TestContext.CurrentContext.TestDirectory, "ab.wav");
            sourceSignal.Save(output);

            FileStream destinationStream = new FileStream(output, FileMode.Open, FileAccess.Read);

            // Create a decoder to read the destination stream
            WaveDecoder destDecoder = new WaveDecoder(destinationStream);
            Assert.AreEqual(2, destDecoder.Channels);
            Assert.AreEqual(176400, destDecoder.Frames);
            Assert.AreEqual(352800, destDecoder.Samples);
            Assert.AreEqual(4000, destDecoder.Duration);
            Assert.AreEqual(44100, destDecoder.SampleRate);
            Assert.AreEqual(32, destDecoder.BitsPerSample);
            Assert.AreEqual(1411200 * 2, destDecoder.AverageBitsPerSecond);

            // Decode the destination stream
            Signal destSignal = destDecoder.Decode();

            // Assert that the signal which has been saved to the destination
            // stream and the signal which has just been read from this same
            // stream are identical
            Assert.AreEqual(sourceSignal.Length, destSignal.Length);
            Assert.AreEqual(sourceSignal.SampleFormat, destSignal.SampleFormat);
            Assert.AreEqual(sourceSignal.SampleRate, destSignal.SampleRate);
            Assert.AreEqual(sourceSignal.Samples, destSignal.Samples);
            Assert.AreEqual(sourceSignal.Duration, destSignal.Duration);

            byte[] actual = sourceSignal.ToByte();
            byte[] expected = destSignal.ToByte();
            Assert.AreEqual(expected, actual);
        }

    }
}
