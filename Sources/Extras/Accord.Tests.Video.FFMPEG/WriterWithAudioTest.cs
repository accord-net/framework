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
    public class WriterWithAudioTest
    {
        [Test]
        public void write_video_new_api()
        {
            string basePath = TestContext.CurrentContext.TestDirectory;

            #region doc_new_api
            // Let's say we would like to save file using a .mp4 media 
            // container, a H.265 video codec for the video stream, and 
            // AAC for the audio stream, into the file:
            string outputPath = Path.Combine(basePath, "output_audio.avi");

            // First, we create a new VideoFileWriter:
            var videoWriter = new VideoFileWriter()
            {
                // Our video will have the following characteristics:
                Width = 800,
                Height = 600,
                FrameRate = 24,
                BitRate = 1200 * 1000,
                VideoCodec = VideoCodec.H265,
                AudioCodec = AudioCodec.Aac,
                AudioBitRate = 44100,
                AudioLayout = AudioLayout.Stereo,
                FrameSize = 44100,
                PixelFormat = AVPixelFormat.FormatYuv420P
            };

            // We can open for it writing:
            videoWriter.Open(outputPath);

            // At this point, we can check the console of our application for useful 
            // information regarding the media streams created by FFMPEG. We can also
            // check those properties using the class itself, specially for properties
            // that we didn't set beforehand but that have been filled by FFMPEG:

            int width = videoWriter.Width;
            int height = videoWriter.Height;
            int frameRate = videoWriter.FrameRate.Numerator;
            int bitRate = videoWriter.BitRate;
            VideoCodec videoCodec = videoWriter.VideoCodec;
            AudioCodec audioCodec = videoWriter.AudioCodec;
            AudioLayout audioLayout = videoWriter.AudioLayout;
            int audioChannels = videoWriter.NumberOfChannels;

            // We haven't set those properties, but FFMPEG has filled them for us:
            int audioSampleRate = videoWriter.SampleRate;
            int audioSampleSize = videoWriter.FrameSize;

            // Now, let's say we would like to save dummy images of 
            // changing color, with a sine wave as the audio stream:

            var g = new SineGenerator()
            {
                Channels = 1, // we will generate only one channel, and the file writer will convert on-the-fly
                Format = SampleFormat.Format32BitIeeeFloat,
                Frequency = 10f,
                Amplitude = 0.9f,
                SamplingRate = 44100
            };

            var m2i = new MatrixToImage();
            Bitmap frame;

            for (byte i = 0; i < 255; i++)
            {
                // Create bitmap matrix from a matrix of RGB values:
                byte[,] matrix = Matrix.Create(height, width, i);
                m2i.Convert(matrix, out frame);

                // Write the frame to the stream. We can optionally specify
                // the moment when this frame should remain in the stream:
                videoWriter.WriteVideoFrame(frame, TimeSpan.FromSeconds(i));

                // We can also write the audio samples if we need to:
                Signal signal = g.Generate(TimeSpan.FromSeconds(1)); // generate 1 second of audio
                videoWriter.WriteAudioFrame(signal); // save it to the stream
            }

            // We can get how long our written video is:
            TimeSpan duration = videoWriter.Duration;

            // Close the stream
            videoWriter.Close();
            #endregion

            Assert.AreEqual(2540000000, duration.Ticks);

            Assert.AreEqual(800, width);
            Assert.AreEqual(600, height);
            Assert.AreEqual(24, frameRate);
            Assert.AreEqual(1200000, bitRate);
            Assert.AreEqual(VideoCodec.H265, videoCodec);

            Assert.AreEqual(AudioCodec.Aac, audioCodec);
            Assert.AreEqual(44100, audioSampleRate);
            Assert.AreEqual(AudioLayout.Stereo, audioLayout);
            Assert.AreEqual(2, audioChannels);
        }

    }
}
