// Accord FFMPEG Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © AForge.NET, 2009-2011
// contacts@aforgenet.com
//
// Copyright © MelvinGr, 2016-2017
// https://github.com/MelvinGr
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//

#pragma once

#include "AudioCodec.h"
#include "VideoCodec.h"
#include "SampleFormats.h"
#include "PixelFormats.h"
#include "VideoFileWriter.h"

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Drawing;
using namespace System::Drawing::Imaging;
using namespace System::IO;
using namespace Accord::Video;
using namespace Accord::Math;

namespace Accord {
    namespace Video {
        namespace FFMPEG
        {
            ref class VideoFileWriter;
            ref struct ReaderPrivateData;

            /// <summary>
            ///   Class for reading video files utilizing FFmpeg library.
            /// </summary>
            /// 
            /// <remarks>
            ///   <para>This class allows to read video files using <a href="http://www.ffmpeg.org/">FFmpeg</a> library.</para>
            /// 
            ///   <para><note>Make sure you have <b>FFmpeg</b> binaries (DLLs) in the output folder of your application in order
            ///   to use this class successfully. <b>FFmpeg</b> binaries can be found in Externals folder provided with Accord.NET
            ///   framework's distribution.</note></para>
            /// </remarks>
            /// 
            /// <example>
            /// <para>
            ///   After making sure FFMPEG's dlls are contained in the output folder of your application,
            ///   you can use the following code to open a video file and read frames in order from it:</para>
            /// <code>
            /// // create instance of video reader
            /// VideoFileReader reader = new VideoFileReader();
            ///
            /// // open video file
            /// reader.Open("test.avi");
            /// 
            /// // check some of its attributes
            /// Console.WriteLine("width:  " + reader.Width);
            /// Console.WriteLine("height: " + reader.Height);
            /// Console.WriteLine("fps:    " + reader.FrameRate);
            /// Console.WriteLine("codec:  " + reader.CodecName);
            ///
            /// // read 100 video frames out of it
            /// for (int i = 0; i &lt; 100; i++)
            /// {
            ///     using (Bitmap videoFrame = reader.ReadVideoFrame())
            ///     {
            ///         // process the frame somehow
            ///         // ...
            ///     }
            /// }
            ///
            /// reader.Close();
            /// </code>
            ///
            /// <para>
            ///   Creating new Bitmaps for every frame can be quite expensive. The following example shows how 
            ///   to read frames into a pre-allocated Bitmap and reuse this same memory location for each frame.
            ///   It also shows how to process each frame using a face detector, and how to save those detections
            ///   back to disk in the form of individual frames and as a .mp4 file using <see cref="VideoFileWriter"/>.</para>
            ///   <code source="Sources\Extras\Accord.Tests.Video.FFMPEG\ObjectDetectorTest.cs" region="doc_video"/>
            ///   <img src="..\images\video\haar_frame_24.png" />
            /// <para>
            ///   The <a href="https://1drv.ms/v/s!AoiTwBxoR4OAoLJhPozzixD25XcbiQ">generated video file can be found here</a>.</para>
            ///
            /// <para>
            ///   The next example shows how to feed the frames returned by the VideoFileReader into an object
            ///   tracker, how to mark the tracked object positions using RectanglesMarker, and 
            ///   save those frames as individual files to the disk.</para>
            ///   <code source="Sources\Extras\Accord.Tests.Video.FFMPEG\MatchingTrackerTest.cs" region="doc_track" />
            ///   <img src="..\images\video\matching_frame_223.png" />
            /// </example>
            ///
            /// <seealso cref="VideoFileWriter"/>
            ///
            public ref class VideoFileReader : IDisposable
            {
                int                    m_width;
                int                    m_height;
                Rational               m_videoFrameRate;
                String^                m_videoCodecName;
                Int64                  m_videoFramesCount;
                int                    m_videoBitRate;
                FFMPEG::VideoCodec     m_videoCodec;

                int                    m_audioSampleRate;
                FFMPEG::AVSampleFormat m_audioSampleFormat;
                String^                m_audioCodecName;
                Int64                  m_audioFramesCount;
                int                    m_audioBitRate;
                FFMPEG::AudioCodec     m_audioCodec;

                // private data of the class
                ReaderPrivateData^ data;
                bool disposed;

                Bitmap^ DecodeVideoFrame(BitmapData^ bitmapData);
                IList<byte>^ DecodeAudioFrame(IList<byte>^ audio);

                Bitmap^ readVideoFrame(int frameIndex, BitmapData^ image, IList<byte>^ audio);

                // Checks if video file was opened
                void CheckIfVideoFileIsOpen()
                {
                    if (data == nullptr)
                        throw gcnew IOException("Video file is not open, so can not access its properties.");
                }

                // Check if the object was already disposed
                void CheckIfDisposed()
                {
                    if (disposed)
                        throw gcnew ObjectDisposedException("The object was already disposed.");
                }

            protected:
                /// <summary>
                /// Object's finalizer.
                /// </summary>
                /// 
                !VideoFileReader()
                {
                    Close();
                }

            public:
                /// <summary>
                /// Frame width of the opened video file.
                /// </summary>
                ///
                /// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
                ///
                property int Width
                {
                    int get()
                    {
                        CheckIfVideoFileIsOpen();
                        return m_width;
                    }
                }

                /// <summary>
                /// Frame height of the opened video file.
                /// </summary>
                ///
                /// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
                ///
                property int Height
                {
                    int get()
                    {
                        CheckIfVideoFileIsOpen();
                        return m_height;
                    }
                }

                /// <summary>
                /// Frame rate of the opened video file.
                /// </summary>
                ///
                /// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
                ///
                property Rational FrameRate
                {
                    Rational get()
                    {
                        CheckIfVideoFileIsOpen();
                        return m_videoFrameRate;
                    }
                }

                /// <summary>
                /// Audio frame rate of the opened video file.
                /// </summary>
                ///
                /// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
                ///
                property int SampleRate
                {
                    int get()
                    {
                        CheckIfVideoFileIsOpen();
                        return m_audioSampleRate;
                    }
                }

                /// <summary>
                /// Number of video frames in the opened video file.
                /// </summary>
                ///
                /// <remarks><para><note><b>Warning</b>: some video file formats may report different value
                /// from the actual number of video frames in the file (subject to fix/investigate).</note></para>
                /// </remarks>
                ///
                /// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
                ///
                property Int64 FrameCount
                {
                    Int64 get()
                    {
                        CheckIfVideoFileIsOpen();
                        return m_videoFramesCount;
                    }
                }

                /// <summary>
                /// Bit rate of the video stream.
                /// </summary>
                ///
                /// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
                ///
                property int BitRate
                {
                    int get()
                    {
                        CheckIfVideoFileIsOpen();
                        return m_videoBitRate;
                    }
                }
                
                /// <summary>
                /// Gets the codec that has been used to encode the opened video file.
                /// </summary>
                ///
                /// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
                ///
                property FFMPEG::VideoCodec VideoCodec
                {
                    FFMPEG::VideoCodec get()
                    {
                        CheckIfVideoFileIsOpen();
                        return m_videoCodec;
                    }
                }

                /// <summary>
                /// Gets the audio that has been used to encode audio in the opened video file.
                /// </summary>
                ///
                /// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
                ///
                property FFMPEG::AudioCodec AudioCodec
                {
                    FFMPEG::AudioCodec get()
                    {
                        CheckIfVideoFileIsOpen();
                        return m_audioCodec;
                    }
                }



                /// <summary>
                /// Name of codec used for encoding the opened video file.
                /// </summary>
                ///
                /// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
                ///
                property String^ CodecName
                {
                    String^ get()
                    {
                        CheckIfVideoFileIsOpen();
                        return m_videoCodecName;
                    }
                }

                /// <summary>
                /// The property specifies if a video file is opened or not by this instance of the class.
                /// </summary>
                property bool IsOpen
                {
                    bool get()
                    {
                        return (data != nullptr);
                    }
                }

                /// <summary>
                /// Initializes a new instance of the <see cref="VideoFileReader"/> class.
                /// </summary>
                /// 
                VideoFileReader();

                /// <summary>
                /// Disposes the object and frees its resources.
                /// </summary>
                /// 
                ~VideoFileReader()
                {
                    this->!VideoFileReader();
                    disposed = true;
                }

                /// <summary>
                /// Open video file with the specified name.
                /// </summary>
                ///
                /// <param name="fileName">Video file name to open.</param>
                ///
                /// <exception cref="System::IO::IOException">Cannot open video file with the specified name.</exception>
                /// <exception cref="VideoException">A error occurred while opening the video file. See exception message.</exception>
                ///
                void Open(String^ fileName);

                /// <summary>
                /// Read next video frame of the currently opened video file.
                /// </summary>
                /// 
                /// <returns>Returns next video frame of the opened file or <see langword="null"/> if end of
                /// file was reached. The returned video frame has 24 bpp color format.</returns>
                /// 
                /// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
                /// <exception cref="VideoException">A error occurred while reading next video frame. See exception message.</exception>
                /// 
                Bitmap^ ReadVideoFrame()
                {
                    return ReadVideoFrame(-1);
                }

                /// <summary>
                /// Read the given video frame of the currently opened video file.
                /// </summary>
                /// 
                /// <returns>Returns the desired frame of the opened file or <see langword="null"/> if end of
                /// file was reached. The returned video frame has 24 bpp color format.</returns>
                /// 
                /// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
                /// <exception cref="VideoException">A error occurred while reading next video frame. See exception message.</exception>
                /// 
                Bitmap^ ReadVideoFrame(int frameIndex)
                {
                    return readVideoFrame(frameIndex, nullptr, nullptr);
                }

                /// <summary>
                /// Read next video frame of the currently opened video file.
                /// </summary>
                /// 
                /// <returns>Returns next video frame of the opened file or <see langword="null"/> if end of
                /// file was reached. The returned video frame has 24 bpp color format.</returns>
                /// 
                /// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
                /// <exception cref="VideoException">A error occurred while reading next video frame. See exception message.</exception>
                /// 
                void ReadVideoFrame(BitmapData^ output)
                {
                    readVideoFrame(-1, output, nullptr);
                }

                /// <summary>
                /// Read the given video frame of the currently opened video file.
                /// </summary>
                /// 
                /// <returns>Returns the desired frame of the opened file or <see langword="null"/> if end of
                /// file was reached. The returned video frame has 24 bpp color format.</returns>
                /// 
                /// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
                /// <exception cref="VideoException">A error occurred while reading next video frame. See exception message.</exception>
                /// 
                void ReadVideoFrame(int frameIndex, BitmapData^ output)
                {
                    readVideoFrame(frameIndex, output, nullptr);
                }



                /// <summary>
                /// Read next video frame of the currently opened video file.
                /// </summary>
                /// 
                /// <returns>Returns next video frame of the opened file or <see langword="null"/> if end of
                /// file was reached. The returned video frame has 24 bpp color format.</returns>
                /// 
                /// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
                /// <exception cref="VideoException">A error occurred while reading next video frame. See exception message.</exception>
                /// 
                Bitmap^ ReadVideoFrame(System::Collections::Generic::IList<byte>^ audio)
                {
                    return readVideoFrame(-1, nullptr, audio);
                }

                /// <summary>
                /// Read the given video frame of the currently opened video file.
                /// </summary>
                /// 
                /// <returns>Returns the desired frame of the opened file or <see langword="null"/> if end of
                /// file was reached. The returned video frame has 24 bpp color format.</returns>
                /// 
                /// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
                /// <exception cref="VideoException">A error occurred while reading next video frame. See exception message.</exception>
                /// 
                Bitmap^ ReadVideoFrame(int frameIndex, System::Collections::Generic::IList<byte>^ audio)
                {
                    return readVideoFrame(frameIndex, nullptr, audio);
                }

                /// <summary>
                /// Read next video frame of the currently opened video file.
                /// </summary>
                /// 
                /// <returns>Returns next video frame of the opened file or <see langword="null"/> if end of
                /// file was reached. The returned video frame has 24 bpp color format.</returns>
                /// 
                /// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
                /// <exception cref="VideoException">A error occurred while reading next video frame. See exception message.</exception>
                /// 
                void ReadVideoFrame(BitmapData^ output, System::Collections::Generic::IList<byte>^ audio)
                {
                    readVideoFrame(-1, output, audio);
                }

                /// <summary>
                /// Read the given video frame of the currently opened video file.
                /// </summary>
                /// 
                /// <returns>Returns the desired frame of the opened file or <see langword="null"/> if end of
                /// file was reached. The returned video frame has 24 bpp color format.</returns>
                /// 
                /// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
                /// <exception cref="VideoException">A error occurred while reading next video frame. See exception message.</exception>
                /// 
                void ReadVideoFrame(int frameIndex, BitmapData^ output, System::Collections::Generic::IList<byte>^ audio)
                {
                    readVideoFrame(frameIndex, output, audio);
                }



                /// <summary>
                /// Close currently opened video file if any.
                /// </summary>
                /// 
                void Close();
            };
        }
    }
}
