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

        [Test]
        public void write_video_test()
        {
            var videoWriter = new VideoFileWriter();

            int width = 800;
            int height = 600;
            int framerate = 24;
            string path = Path.GetFullPath("output.avi");
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

    }
}
