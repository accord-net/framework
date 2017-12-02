﻿// Accord Unit Tests
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

    [TestFixture]
    public class WaveEncoderTest
    {

        [Test]
        public void WaveEncoderConstructorTest()
        {
            // Load a file in PCM 16bpp format
            // Number of samples: 352.800
            // Number of frames:  176.400
            // Sample rate:        44.100 Hz
            // Block align:         4
            // Channels:            2
            // Duration:            4.0   s
            // Bytes:             705.644 
            // Bitrate:           1411kbps

            // sizeof(float) = 4
            // sizeof(int)   = 4
            // sizeof(short) = 2

            var sourceStream = SignalTest.GetSignal("a.wav");
            MemoryStream destinationStream = new MemoryStream();

            // Create a decoder for the source stream
            WaveDecoder sourceDecoder = new WaveDecoder(sourceStream);
            Assert.AreEqual(2, sourceDecoder.Channels);
            Assert.AreEqual(352800, sourceDecoder.Samples);
            Assert.AreEqual(176400, sourceDecoder.Frames);
            Assert.AreEqual(4000, sourceDecoder.Duration);
            Assert.AreEqual(44100, sourceDecoder.SampleRate);
            Assert.AreEqual(16, sourceDecoder.BitsPerSample);
            Assert.AreEqual(1411200, sourceDecoder.AverageBitsPerSecond);

            // Decode the signal in the source stream
            Signal sourceSignal = sourceDecoder.Decode();
            Assert.AreEqual(352800, sourceSignal.Samples);
            Assert.AreEqual(176400, sourceSignal.Length);
            Assert.AreEqual(4000, sourceSignal.Duration.TotalMilliseconds);
            Assert.AreEqual(2, sourceSignal.Channels);
            Assert.AreEqual(44100, sourceSignal.SampleRate);
            Assert.AreEqual(sizeof(float) * 352800, sourceSignal.NumberOfBytes);
            Assert.AreEqual(sizeof(short) * 352800, sourceDecoder.Bytes);


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
            sourceStream.Seek(0, SeekOrigin.Begin);
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
