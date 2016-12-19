// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Accord.Video.FFMPEG;
using NUnit.Framework;

namespace Accord.Tests.Video.FFMPEG
{
    [TestFixture]
    public class VideoFileWriterTest
    {
        [Test]
        public void write_video_test()
        {
            var path = "text.mkv";
            var seconds = 15;
            var framerate = 30;
            var width = 1024;
            var height = 1024;

            // create large font
            var font = new Font(SystemFonts.DefaultFont.Name, width / 2f,
                SystemFonts.DefaultFont.Style, SystemFonts.DefaultFont.Unit);

            var brushes = new[]
            {
                new SolidBrush(Color.Red),
                new SolidBrush(Color.Green),
                new SolidBrush(Color.Blue)
            };

            // create instance of video writer
            var writer = new VideoFileWriter();

            // create new video file, (Other codecs need work)
            writer.Open(path, width, height, framerate, VideoCodec.MPEG4);

            // create a bitmap to save into the video file
            var bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            using (var g = Graphics.FromImage(bitmap))
            {
                for (var i = 0; i < seconds; i++)
                    for (var f = 0; f < framerate; f++)
                    {

                        var txt = i.ToString();
                        var size = g.MeasureString(txt, font);
                        var point = new PointF(width / 2f - size.Width / 2,
                            height / 2f - size.Height / 2);

                        g.FillRectangle(brushes[i % brushes.Length], 0, 0, width, height);
                        g.DrawString(txt, font, brushes[(i + 1) % brushes.Length], point);


                        writer.WriteVideoFrame(bitmap);
                    }
            }
            writer.Close();
            Assert.IsTrue(File.Exists(path));
        }
    }
}