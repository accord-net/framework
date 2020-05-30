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

    [TestFixture]
    public class VideoFileWriterTest
    {

        string fireplace_mp4 = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "fireplace.mp4");

        [Test]
        public void framework_version_test()
        {
            string actual = typeof(VideoFileWriter).Assembly.ImageRuntimeVersion;
#if NET35
            Assert.AreEqual("v2.0.50727", actual);
#else
            Assert.IsTrue(actual.StartsWith("v4"));
#endif
        }

        [Test]
        public void write_video_new_api()
        {
            string basePath = TestContext.CurrentContext.TestDirectory;

            #region doc_new_api

            // Let's say we would like to save file using a .avi media 
            // container and a MPEG4 (DivX/XVid) codec, saving it into:
            string outputPath = Path.Combine(basePath, "output_video.avi");

            // First, we create a new VideoFileWriter:
            var videoWriter = new VideoFileWriter()
            {
                // Our video will have the following characteristics:
                Width = 800,
                Height = 600,
                FrameRate = 24,
                BitRate = 1200 * 1000,
                VideoCodec = VideoCodec.Mpeg4,
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

            // We haven't set those properties, but FFMPEG has filled them for us:
            AudioCodec audioCodec = videoWriter.AudioCodec;
            int audioSampleRate = videoWriter.SampleRate;
            AudioLayout audioLayout = videoWriter.AudioLayout;
            int audioChannels = videoWriter.NumberOfChannels;

            // Now, let's say we would like to save dummy images of changing color
            var m2i = new MatrixToImage();
            Bitmap frame;

            for (byte i = 0; i < 255; i++)
            {
                // Create bitmap matrix from a matrix of RGB values:
                byte[,] matrix = Matrix.Create(height, width, i);
                m2i.Convert(matrix, out frame);

                // Write the frame to the stream. We can optionally specify
                // the moment that this frame should remain in the stream:
                videoWriter.WriteVideoFrame(frame, TimeSpan.FromSeconds(i));
            }

            // We can get how long our written video is:
            TimeSpan duration = videoWriter.Duration;

            // Close the stream
            videoWriter.Close();
            videoWriter.Dispose();
            #endregion

            Assert.AreEqual(2540000000, duration.Ticks);

            Assert.AreEqual(800, width);
            Assert.AreEqual(600, height);
            Assert.AreEqual(24, frameRate);
            Assert.AreEqual(1200000, bitRate);
            Assert.AreEqual(VideoCodec.Mpeg4, videoCodec);

            Assert.IsTrue(AudioCodec.Default == audioCodec || AudioCodec.Mp3 == audioCodec);
            Assert.AreEqual(44100, audioSampleRate);
            Assert.AreEqual(audioLayout, audioLayout);
            Assert.AreEqual(2, audioChannels);
        }

        [Test]
        public void write_video_test()
        {
            var videoWriter = new VideoFileWriter();

            int width = 800;
            int height = 600;
            int framerate = 24;
            string path = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, "output.avi"));
            int videoBitRate = 1200 * 1000;

            videoWriter.Open(path, width, height, framerate, VideoCodec.H264, videoBitRate);

            Assert.AreEqual(videoBitRate, videoWriter.BitRate);

            var m2i = new MatrixToImage();
            Bitmap frame;

            for (byte i = 0; i < 255; i++)
            {
                byte[,] matrix = Matrix.Create(height, width, i);
                m2i.Convert(matrix, out frame);
                videoWriter.WriteVideoFrame(frame, TimeSpan.FromSeconds(i));
            }

            videoWriter.Close();

            Assert.IsTrue(File.Exists(path));
        }

        [Test]
        public void framerate_test()
        {
            write_and_open((Rational)30, 30, 1);
            write_and_open((Rational)29.97, 2997, 100);
        }

        [Test]
        public void reencode_vp8()
        {
            var fileInput = new FileInfo(fireplace_mp4);
            var fileOutput = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "fireplace_output.webm"));
            reencode(fileInput, fileOutput, VideoCodec.Vp8);
        }

        [Test]
        public void reencode_vp9()
        {
            var fileInput = new FileInfo(fireplace_mp4);
            var fileOutput = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "fireplace_output_webm.webm"));
            reencode(fileInput, fileOutput, VideoCodec.Vp9);
        }

        [Test]
        public void reencode_ogg()
        {
            var fileInput = new FileInfo(fireplace_mp4);
            var fileOutput = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "fireplace_output_ogg.ogg"));
            reencode(fileInput, fileOutput, VideoCodec.Theora);
        }

        [Test]
        public void reencode_h264_mp4()
        {
            var fileInput = new FileInfo(fireplace_mp4);
            var fileOutput = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "fireplace_output_mp4.mp4"));
            reencode(fileInput, fileOutput, VideoCodec.H264);
        }

        [Test]
        public void reencode_h264()
        {
            var fileInput = new FileInfo(fireplace_mp4);
            var fileOutput = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "fireplace_output_h264.avi"));
            reencode(fileInput, fileOutput, VideoCodec.H264);
        }

        private static void reencode(FileInfo fileInput, FileInfo fileOutput, VideoCodec outputCodec,
            AVPixelFormat format = AVPixelFormat.FormatYuv420P, double expectedFrameRate = 2997 / 100.0)
        {
            using (var videoFileReader = new Accord.Video.FFMPEG.VideoFileReader())
            {
                videoFileReader.Open(fileInput.FullName);

                using (var videoFileWriter = new Accord.Video.FFMPEG.VideoFileWriter())
                {
                    Assert.AreEqual(2997, videoFileReader.FrameRate.Numerator);
                    Assert.AreEqual(100, videoFileReader.FrameRate.Denominator);

                    videoFileWriter.Width = videoFileReader.Width;
                    videoFileWriter.Height = videoFileReader.Height;
                    videoFileWriter.FrameRate = videoFileReader.FrameRate;
                    videoFileWriter.VideoCodec = outputCodec;
                    Assert.AreEqual(AVPixelFormat.FormatYuv420P, videoFileWriter.PixelFormat);
                    videoFileWriter.PixelFormat = format;

                    videoFileWriter.Open(fileOutput.FullName);

                    do
                    {
                        using (var bitmap = videoFileReader.ReadVideoFrame())
                        {
                            if (bitmap == null)
                                break;

                            videoFileWriter.WriteVideoFrame(bitmap);
                        }
                    }
                    while (true);

                    videoFileWriter.Close();
                }

                videoFileReader.Close();
            }

            using (var videoFileReader = new Accord.Video.FFMPEG.VideoFileReader())
            {
                videoFileReader.Open(fileOutput.FullName);

                Assert.AreEqual(expectedFrameRate, videoFileReader.FrameRate.Value, 0.01);
            }
        }

        private static void write_and_open(Rational framerate, int num, int den)
        {
            int width = 800;
            int height = 600;
            string path = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, "output2.avi"));
            int videoBitRate = 1200 * 1000;

            {
                var videoWriter = new VideoFileWriter();

                videoWriter.Open(path, width, height, framerate, VideoCodec.FfvHuff, videoBitRate);

                Assert.AreEqual(width, videoWriter.Width);
                Assert.AreEqual(height, videoWriter.Height);
                Assert.AreEqual(videoBitRate, videoWriter.BitRate);
                Assert.AreEqual(num, videoWriter.FrameRate.Numerator);
                Assert.AreEqual(den, videoWriter.FrameRate.Denominator);



                var m2i = new MatrixToImage();
                Bitmap frame;

                for (byte i = 0; i < 255; i++)
                {
                    byte[,] matrix = Matrix.Create(height, width, i);
                    m2i.Convert(matrix, out frame);
                    videoWriter.WriteVideoFrame(frame, TimeSpan.FromSeconds(i));
                }

                videoWriter.Close();
            }

            Assert.IsTrue(File.Exists(path));


            {
                VideoFileReader reader = new VideoFileReader();

                reader.Open(path);

                Assert.AreEqual(width, reader.Width);
                Assert.AreEqual(height, reader.Height);
                //Assert.AreEqual(videoBitRate, reader.BitRate);
                Assert.AreEqual(num, reader.FrameRate.Numerator);
                Assert.AreEqual(den, reader.FrameRate.Denominator);
            }

        }
    }
}
