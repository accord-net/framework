// Accord FFMPEG Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
// Copyright © AForge.NET, 2009-2011
// contacts@aforgenet.com
//
// Copyright © MelvinGr, 2016-2017
// https://github.com/MelvinGr
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

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Drawing;
using namespace System::Drawing::Imaging;
using namespace Accord::Video;
using namespace Accord::Math;
using namespace Accord::Audio;

#include "VideoCodec.h"
#include "AudioCodec.h"
#include "AudioLayouts.h"
#include "PixelFormats.h"
#include "SampleFormats.h"

namespace Accord {
    namespace Video {
        namespace FFMPEG
        {
            ref class VideoFileReader;
            struct WriterPrivateData;

            /// <summary>
            ///   Class for writing video files utilizing FFmpeg library.
            /// </summary>
            ///
            /// <remarks>
            ///   <para>The class allows to write video files using <a href="http://www.ffmpeg.org/">FFmpeg</a> library.</para>
            ///
            ///   <para><note>Make sure you have <b>FFmpeg</b> binaries (DLLs) in the output folder of your application in order
            ///   to use this class successfully. <b>FFmpeg</b> binaries can be found in Externals folder provided with Accord.NET
            ///   framework's distribution.</note></para>
            /// </remarks>
            ///
            /// <example>
            /// <para>
            ///   After making sure FFMPEG's dlls are contained in the output folder of your application,
            ///   you can use the following code to open a video file and save frames in order to it:</para>
            ///   <code source="Sources\Extras\Accord.Tests.Video.FFMPEG\WriterTest.cs" region="doc_new_api"/>
            ///
            /// <para>
            ///   We can also record videos with audio, as shown below:</para>
            ///   <code source="Sources\Extras\Accord.Tests.Video.FFMPEG\WriterWithAudioTest.cs" region="doc_new_api"/>
            ///
            /// <para>
            ///   The following example shows how to read frames from a video file using <see cref="VideoFileReader"/>, 
            ///   how to process each frame using a face detector, and how to save those detections back to disk in the 
            ///   form of individual frames and as a .mp4 file.</para>
            ///   <code source="Sources\Extras\Accord.Tests.Video.FFMPEG\ObjectDetectorTest.cs" region="doc_video"/>
            ///   <img src="..\images\video\haar_frame_24.png" />
            /// <para>
            ///   The <a href="https://1drv.ms/v/s!AoiTwBxoR4OAoLJhPozzixD25XcbiQ">generated video file can be found here</a>.</para>
            /// </example>
            ///
            /// <seealso cref="VideoFileReader"/>
            ///
            public ref class VideoFileWriter : IDisposable
            {
                // private data of the class
                WriterPrivateData* data;
                bool disposed;

                Dictionary<String^, String^>^ audioOptions;
                Dictionary<String^, String^>^ videoOptions;

            protected:
                /// <summary>
                /// Disposes the object and frees its resources.
                /// </summary>
                /// 
                ~VideoFileWriter()
                {
                    this->!VideoFileWriter();
                }

                /// <summary>
                /// Disposes the object and frees its resources.
                /// </summary>
                /// 
                !VideoFileWriter();

            public:

                /// <summary>
                /// Gets the current video duration.
                /// </summary>
                ///
                /// <value>The duration.</value>
                ///
                property TimeSpan Duration
                {
                    TimeSpan get();
                }

                property Dictionary<String^, String^>^ AudioOptions
                {
                    Dictionary<String^, String^>^ get() { return this->audioOptions; };
                }

                property Dictionary<String^, String^>^ VideoOptions
                {
                    Dictionary<String^, String^>^ get() { return this->videoOptions; };
                }

                /// <summary>
                /// Gets or sets the picture frame width for the current file.
                /// </summary>
                ///
                property int Width { int get(); void set(int); }

                /// <summary>
                /// Gets or sets the picture frame height for the current file.
                /// </summary>
                ///
                property int Height { int get(); void set(int); }


                /// <summary>
                /// Gets or sets the picture frame rate for the current file.
                /// </summary>
                ///
                property Rational FrameRate { Rational get(); void set(Rational); }

                /// <summary>
                /// Gets or sets the audio sample rate for the current file.
                /// </summary>
                ///
                property int SampleRate { int get(); void set(int); }


                /// <summary>
                /// Gets or sets the bit rate of the video stream.
                /// </summary>
                ///
                property int BitRate { int get(); void set(int); }

                /// <summary>
                /// Gets or sets the bit rate of the audio stream.
                /// </summary>
                ///
                property int AudioBitRate { int get(); void set(int); }


                /// <summary>
                /// Gets or sets the audio frame size for the current file.
                /// </summary>
                ///
                property int FrameSize { int get(); void set(int); }

                /// <summary>
                /// Gets or sets the pixel format for the current file.
                /// </summary>
                ///
                property FFMPEG::AVPixelFormat PixelFormat { FFMPEG::AVPixelFormat get(); void set(FFMPEG::AVPixelFormat); }

                /// <summary>
                /// Gets or sets the audio sample format for the current file.
                /// </summary>
                ///
                property FFMPEG::AVSampleFormat SampleFormat { FFMPEG::AVSampleFormat get(); void set(FFMPEG::AVSampleFormat); }

                /// <summary>
                /// Gets or sets the current audio channel layout for the current file.
                /// </summary>
                ///
                property FFMPEG::AudioLayout AudioLayout { FFMPEG::AudioLayout get(); void set(FFMPEG::AudioLayout); }

                /// <summary>
                /// Gets the number of audio channels in the current <see cref="AudioLayout"/>.
                /// </summary>
                ///
                property int NumberOfChannels { int get(); }

                /// <summary>
                /// Codec to use for the video file.
                /// </summary>
                ///
                property VideoCodec VideoCodec { FFMPEG::VideoCodec get(); void set(FFMPEG::VideoCodec); }

                /// <summary>
                /// Audio to use for the video file.
                /// </summary>
                ///
                property FFMPEG::AudioCodec AudioCodec { FFMPEG::AudioCodec get(); void set(FFMPEG::AudioCodec); }

                /// <summary>
                /// Gets whether the video file is open. While the video is open, certain
                /// properties of this class cannot be changed and will become read-only.
                /// </summary>
                /// 
                property bool IsOpen { bool get(); }

                /// <summary>
                /// Initializes a new instance of the <see cref="VideoFileWriter"/> class.
                /// </summary>
                /// 
                VideoFileWriter();



                /// <summary>
                /// Create video file with the specified name and attributes.
                /// </summary>
                ///
                /// <param name="fileName">Video file name to create.</param>
                ///
                void Open(String^ fileName)
                {
                    Open(fileName, nullptr);
                }

                /// <summary>
                /// Create video file with the specified name and attributes.
                /// </summary>
                ///
                /// <param name="fileName">Video file name to create.</param>
                /// <param name="format">Container format to use (e.g. "avi", "mp4" or "mkv").</param>
                ///
                void Open(String^ fileName, String^ format);



                /// <summary>
                /// Obsolete. Please Please set the video properties in this class and pass only the filename to this constructor.
                /// </summary>
                ///
                [Obsolete("Please set the video properties in this class and pass only the filename to this constructor.")]
                void Open(String^ fileName, int width, int height)
                {
#pragma warning(push)
#pragma warning(disable: 612)
#pragma warning(disable: 618)
#pragma warning(disable: 4947)
                    Open(fileName, width, height, 25);
#pragma warning(pop)
                }

                /// <summary>
                /// Obsolete. Please Please set the video properties in this class and pass only the filename to this constructor.
                /// </summary>
                ///
                [Obsolete("Please set the video properties in this class and pass only the filename to this constructor.")]
                void Open(String^ fileName, int width, int height, Rational frameRate)
                {
#pragma warning(push)
#pragma warning(disable: 612)
#pragma warning(disable: 618)
#pragma warning(disable: 4947)
                    Open(fileName, width, height, frameRate, FFMPEG::VideoCodec::Default);
#pragma warning(pop)
                }

                /// <summary>
                /// Obsolete. Please Please set the video properties in this class and pass only the filename to this constructor.
                /// </summary>
                ///
                [Obsolete("Please set the video properties in this class and pass only the filename to this constructor.")]
                void Open(String^ fileName, int width, int height, Rational frameRate, FFMPEG::VideoCodec codec)
                {
#pragma warning(push)
#pragma warning(disable: 612)
#pragma warning(disable: 618)
#pragma warning(disable: 4947)
                    Open(fileName, width, height, frameRate, codec, 400000);
#pragma warning(pop)
                }

                /// <summary>
                /// Obsolete. Please Please set the video properties in this class and pass only the filename to this constructor.
                /// </summary>
                ///
                [Obsolete("Please set the video properties in this class and pass only the filename to this constructor.")]
                void Open(String^ fileName, int width, int height, Rational frameRate, FFMPEG::VideoCodec codec, int bitRate)
                {
#pragma warning(push)
#pragma warning(disable: 612)
#pragma warning(disable: 618)
#pragma warning(disable: 4947)
                    Open(fileName, width, height, frameRate, codec, bitRate, 0, FFMPEG::AudioLayout::Stereo, 0, FFMPEG::AudioCodec::None, 0);
#pragma warning(pop)
                }

                /// <summary>
                /// Obsolete. Please Please set the video properties in this class and pass only the filename to this constructor.
                /// </summary>
                ///
                [Obsolete("Please set the video properties in this class and pass only the filename to this constructor.")]
                void Open(String^ fileName, int width, int height, Rational frameRate,
                    FFMPEG::VideoCodec codec, int bitRate,
                    FFMPEG::AudioCodec audioCodec, int audioBitrate, int sampleRate, int channels)
                {
#pragma warning(push)
#pragma warning(disable: 612)
#pragma warning(disable: 618)
#pragma warning(disable: 4947)
                    FFMPEG::AudioLayout audioLayout;
                    if (channels == 1)
                        audioLayout = FFMPEG::AudioLayout::Mono;
                    else if (channels == 2)
                        audioLayout = FFMPEG::AudioLayout::Stereo;
                    else
                        throw gcnew VideoException("Unsupported number of channels.");

                    Open(fileName, width, height, frameRate, codec, bitRate, 1024, audioLayout, sampleRate, audioCodec, audioBitrate);
#pragma warning(pop)
                }

                /// <summary>
                /// Obsolete. Please Please set the video properties in this class and pass only the filename to this constructor.
                /// </summary>
                ///
                [Obsolete("Please set the video properties in this class and pass only the filename to this constructor.")]
                void Open(String^ fileName,
                    int imageWidth, int imageHeight, Rational videoFrameRate, FFMPEG::VideoCodec videoCodec, int videoBitRate,
                    int audioFrameSize, FFMPEG::AudioLayout audioChannels, int audioSampleRate, FFMPEG::AudioCodec audioCodec, int audioBitRate)
                {
                    this->Width = imageWidth;
                    this->Height = imageHeight;
                    this->FrameRate = videoFrameRate;
                    this->VideoCodec = videoCodec;
                    this->BitRate = videoBitRate;
                    this->FrameSize = audioFrameSize;
                    this->AudioLayout = audioChannels;
                    this->SampleRate = audioSampleRate;
                    this->AudioCodec = audioCodec;
                    this->AudioBitRate = audioBitRate;

                    this->Open(fileName);
                }

                /// <summary>
                /// Write new video frame into currently opened video file.
                /// </summary>
                ///
                /// <param name="frame">Bitmap to add as a new video frame.</param>
                ///
                /// <remarks><para>The specified bitmap must be either color 24 or 32 bpp image or grayscale 8 bpp (indexed) image.</para>
                /// </remarks>
                ///
                /// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
                /// <exception cref="ArgumentException">The provided bitmap must be 24 or 32 bpp color image or 8 bpp grayscale image.</exception>
                /// <exception cref="ArgumentException">Bitmap size must be of the same as video size, which was specified on opening video file.</exception>
                /// <exception cref="VideoException">A error occurred while writing new video frame. See exception message.</exception>
                /// 
                void WriteVideoFrame(Bitmap^ frame)
                {
                    WriteVideoFrame(frame, TimeSpan::MinValue, System::Drawing::Rectangle(0, 0, frame->Width, frame->Height));
                }

                /// <summary>
                /// Write new video frame into currently opened video file.
                /// </summary>
                ///
                /// <param name="frame">Bitmap to add as a new video frame.</param>
                ///
                /// <remarks><para>The specified bitmap must be either color 24 or 32 bpp image or grayscale 8 bpp (indexed) image.</para>
                /// </remarks>
                ///
                /// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
                /// <exception cref="ArgumentException">The provided bitmap must be 24 or 32 bpp color image or 8 bpp grayscale image.</exception>
                /// <exception cref="ArgumentException">Bitmap size must be of the same as video size, which was specified on opening video file.</exception>
                /// <exception cref="VideoException">A error occurred while writing new video frame. See exception message.</exception>
                /// 
                void WriteVideoFrame(BitmapData^ frame)
                {
                    WriteVideoFrame(frame, TimeSpan::MinValue);
                }

                /// <summary>
                /// Write new video frame into currently opened video file.
                /// </summary>
                ///
                /// <param name="frame">Bitmap to add as a new video frame.</param>
                /// <param name="region">The region of the bitmap that should be registered on video.</param>
                ///
                /// <remarks><para>The specified bitmap must be either color 24 or 32 bpp image or grayscale 8 bpp (indexed) image.</para>
                /// </remarks>
                ///
                /// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
                /// <exception cref="ArgumentException">The provided bitmap must be 24 or 32 bpp color image or 8 bpp grayscale image.</exception>
                /// <exception cref="ArgumentException">Bitmap size must be of the same as video size, which was specified on opening video file.</exception>
                /// <exception cref="VideoException">A error occurred while writing new video frame. See exception message.</exception>
                /// 
                void WriteVideoFrame(Bitmap^ frame, System::Drawing::Rectangle region)
                {
                    WriteVideoFrame(frame, TimeSpan::MinValue, region);
                }

                /// <summary>
                /// Write new video frame into currently opened video file.
                /// </summary>
                ///
                /// <param name="frame">Bitmap to add as a new video frame.</param>
                /// <param name="timestamp">Frame timestamp, total time since recording started.</param>
                ///
                /// <remarks><para>The specified bitmap must be either color 24 or 32 bpp image or grayscale 8 bpp (indexed) image.</para>
                /// </remarks>
                ///
                /// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
                /// <exception cref="ArgumentException">The provided bitmap must be 24 or 32 bpp color image or 8 bpp grayscale image.</exception>
                /// <exception cref="ArgumentException">Bitmap size must be of the same as video size, which was specified on opening video file.</exception>
                /// <exception cref="VideoException">A error occurred while writing new video frame. See exception message.</exception>
                /// 
                void WriteVideoFrame(Bitmap^ frame, TimeSpan timestamp)
                {
                    WriteVideoFrame(frame, timestamp, System::Drawing::Rectangle(0, 0, frame->Width, frame->Height));
                }


                /// <summary>
                /// Write new video frame into currently opened video file.
                /// </summary>
                ///
                /// <param name="frame">Bitmap to add as a new video frame.</param>
                /// <param name="timestamp">Frame timestamp, total time since recording started.</param>
                /// <param name="region">The region of the bitmap that should be registered on video.</param>
                ///
                /// <remarks><para>The specified bitmap must be either color 24 or 32 bpp image or grayscale 8 bpp (indexed) image.</para>
                /// </remarks>
                ///
                /// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
                /// <exception cref="ArgumentException">The provided bitmap must be 24 or 32 bpp color image or 8 bpp grayscale image.</exception>
                /// <exception cref="ArgumentException">Bitmap size must be of the same as video size, which was specified on opening video file.</exception>
                /// <exception cref="VideoException">A error occurred while writing new video frame. See exception message.</exception>
                /// 
                void WriteVideoFrame(Bitmap^ frame, TimeSpan timestamp, System::Drawing::Rectangle region);

                /// <summary>
                /// Write new video frame into currently opened video file.
                /// </summary>
                ///
                /// <param name="frame">Bitmap to add as a new video frame.</param>
                /// <param name="timestamp">Frame timestamp, total time since recording started.</param>
                ///
                /// <remarks><para>The specified bitmap must be either color 24 or 32 bpp image or grayscale 8 bpp (indexed) image.</para>
                /// </remarks>
                ///
                /// <exception cref="System::IO::IOException">Thrown if no video file was open.</exception>
                /// <exception cref="ArgumentException">The provided bitmap must be 24 or 32 bpp color image or 8 bpp grayscale image.</exception>
                /// <exception cref="ArgumentException">Bitmap size must be of the same as video size, which was specified on opening video file.</exception>
                /// <exception cref="VideoException">A error occurred while writing new video frame. See exception message.</exception>
                /// 
                void WriteVideoFrame(BitmapData^ frame, TimeSpan timestamp);

                /// <summary>
                /// Writes a new audio frame to the currently opened video file.
                /// </summary>
                /// 
                /// <param name="signal">Signal to add as a new audio frame.</param>
                ///
                void WriteAudioFrame(Accord::Audio::Signal^ signal)
                {
                    WriteAudioFrame(signal, TimeSpan::MinValue);
                }

                /// <summary>
                /// Writes a new audio frame to the currently opened video file.
                /// </summary>
                ///
                /// <param name="signal">Signal to add as a new audio frame.</param>
                /// <param name="timestamp">Frame timestamp, total time since recording started.</param>
                ///
                void WriteAudioFrame(Accord::Audio::Signal^ signal, TimeSpan timestamp);

                /// <summary>
                /// Flushes the current write buffer to disk.
                /// </summary>
                ///
                void Flush();

                /// <summary>
                /// Closes the currently opened video file if any.
                /// </summary>
                /// 
                void Close();
            };
        }
    }
}
