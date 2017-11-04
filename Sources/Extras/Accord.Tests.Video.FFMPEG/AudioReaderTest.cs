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

namespace Accord.Tests.Video
{
    using Accord.Vision.Detection;
    using NUnit.Framework;
    using System.Threading;
    using Accord.Vision.Detection.Cascades;
    using Accord.Video.FFMPEG;
    using Accord.Math;
    using Accord.Imaging.Converters;
    using System.Drawing;
    using System;
    using System.IO;
    using Accord.Audio.Generators;
    using Accord.Audio;
    using System.Collections.Generic;

    [TestFixture]
    public class AudioReaderTest
    {

        string fireplace_mp4 = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "fireplace.mp4");


        [Test]
        public void audio_read_test()
        {
            var fileInput = new FileInfo(fireplace_mp4);

            var audio = new List<byte>();

            using (var videoFileReader = new Accord.Video.FFMPEG.VideoFileReader())
            {
                videoFileReader.Open(fileInput.FullName);

                // Checked using G-Spot 2.70a
                Assert.AreEqual(699591, videoFileReader.BitRate); // approximate
                Assert.AreEqual("h264", videoFileReader.CodecName);
                Assert.AreEqual(513, videoFileReader.FrameCount);
                Assert.AreEqual(new Rational(2997,100), videoFileReader.FrameRate);
                Assert.AreEqual(360, videoFileReader.Height);
                Assert.AreEqual(true, videoFileReader.IsOpen);
                Assert.AreEqual(638, videoFileReader.Width);

                do
                {
                    using (var bitmap = videoFileReader.ReadVideoFrame(audio))
                    {
                        if (bitmap == null)
                            break;
                    }
                }
                while (true);

                videoFileReader.Close();

                Signal s = Signal.FromArray(audio.ToArray(), 44100, SampleFormat.Format32Bit);

                s.Save("fireplace_from_h264.wav");

                Assert.AreEqual(1, s.Channels);
                Assert.AreEqual(1378340000, s.Duration.Ticks);
                Assert.AreEqual(6078464, s.Length);
                Assert.AreEqual(1, s.NumberOfChannels);
                Assert.AreEqual(6078464, s.NumberOfFrames);
                Assert.AreEqual(6078464, s.NumberOfSamples);
                Assert.AreEqual(SampleFormat.Format32Bit, s.SampleFormat);
                Assert.AreEqual(44100, s.SampleRate);
                Assert.AreEqual(6078464, s.Samples);
                Assert.AreEqual(4, s.SampleSize);
            }
        }
    }
}
