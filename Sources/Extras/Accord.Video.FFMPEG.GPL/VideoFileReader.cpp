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

#include "StdAfx.h"
#include "VideoFileReader.h"
#include "Tools.h"

#include <string>

extern "C"
{
#include "libavformat\avformat.h"
#include "libavformat\avio.h"
#include "libavcodec\avcodec.h"
#include "libswscale\swscale.h"
}


namespace Accord {
    namespace Video {
        namespace FFMPEG
        {
            // A structure to encapsulate all FFMPEG related private variables
            ref struct ReaderPrivateData
            {
            public:
                AVFormatContext*	FormatContext;
                AVStream*			VideoStream;
                AVStream*			AudioStream;
                AVCodecContext*		VideoCodecContext;
                AVCodecContext*		AudioCodecContext;
                AVFrame*			VideoFrame;
                AVFrame*			AudioFrame;
                struct SwsContext*	ConvertContext;
                unsigned long int   nextFrameIndex;

                AVPacket* Packet;
                int uint8_tsRemaining;

                ReaderPrivateData()
                {
                    FormatContext = nullptr;

                    VideoStream = nullptr;
                    VideoCodecContext = nullptr;
                    VideoFrame = nullptr;

                    AudioStream = nullptr;
                    AudioCodecContext = nullptr;
                    AudioFrame = nullptr;

                    ConvertContext = nullptr;

                    Packet = nullptr;
                    uint8_tsRemaining = 0;
                    nextFrameIndex = 0;
                }
            };


            // Class constructor
            VideoFileReader::VideoFileReader()
                : data(nullptr), disposed(false)
            {
                av_register_all();
            }

#pragma managed(push, off)
            static AVFormatContext* open_file(const char* fileName)
            {
                AVFormatContext* formatContext = avformat_alloc_context();
                int success = avformat_open_input(&formatContext, fileName, nullptr, nullptr);

                if (success != 0)
                    return nullptr;
                return formatContext;
            }
#pragma managed(pop)

            // Opens the specified video file
            void VideoFileReader::Open(String^ fileName)
            {
                CheckIfDisposed();

                // close previous file if any was open
                Close();

                data = gcnew ReaderPrivateData();
                data->Packet = new AVPacket();
                data->Packet->data = nullptr;

                bool success = false;

                try
                {
                    // convert specified managed String to UTF8 unmanaged string
                    IntPtr ptr = System::Runtime::InteropServices::Marshal::StringToHGlobalUni(fileName);
                    wchar_t* nativeFileNameUnicode = (wchar_t*)ptr.ToPointer();
                    int utf8StringSize = WideCharToMultiByte(CP_UTF8, 0, nativeFileNameUnicode, -1, NULL, 0, NULL, NULL);
                    char* nativeFileName = new char[utf8StringSize];
                    WideCharToMultiByte(CP_UTF8, 0, nativeFileNameUnicode, -1, nativeFileName, utf8StringSize, NULL, NULL);

                    // open the specified video file
                    data->FormatContext = open_file(nativeFileName);
                    if (data->FormatContext == nullptr)
                        throw gcnew System::IO::IOException("Cannot open the video file.");

                    // retrieve stream information
                    if (avformat_find_stream_info(data->FormatContext, nullptr) < 0)
                        throw gcnew VideoException("Cannot find stream information.");

                    // search for the first video and audio streams
                    for (unsigned int i = 0; i < data->FormatContext->nb_streams; i++)
                    {
                        if (data->FormatContext->streams[i]->codec->codec_type == AVMEDIA_TYPE_VIDEO)
                        {
                            // get the pointer to the codec context for the video stream
                            data->VideoCodecContext = data->FormatContext->streams[i]->codec;
                            data->VideoStream = data->FormatContext->streams[i];
                            break;
                        }
                    }

                    for (unsigned int i = 0; i < data->FormatContext->nb_streams; i++)
                    {
                        if (data->FormatContext->streams[i]->codec->codec_type == AVMEDIA_TYPE_AUDIO)
                        {
                            // get the pointer to the codec context for the audio stream
                            data->AudioCodecContext = data->FormatContext->streams[i]->codec;
                            data->AudioStream = data->FormatContext->streams[i];
                            break;
                        }
                    }

                    if (data->VideoStream == nullptr)
                        throw gcnew VideoException("Cannot find video stream in the specified file.");

                    // find decoder for the video stream
                    AVCodec* videoCodec = avcodec_find_decoder(data->VideoCodecContext->codec_id);
                    if (videoCodec == nullptr)
                        throw gcnew VideoException("Cannot find codec to decode the video stream.");

                    // open the codec
                    if (avcodec_open2(data->VideoCodecContext, videoCodec, nullptr) < 0)
                        throw gcnew VideoException("Cannot open video codec.");

                    // allocate video frame
                    data->VideoFrame = av_frame_alloc();

                    // prepare scaling context to convert RGB image to video format
                    data->ConvertContext = sws_getContext(data->VideoCodecContext->width, data->VideoCodecContext->height, data->VideoCodecContext->pix_fmt,
                        data->VideoCodecContext->width, data->VideoCodecContext->height, AV_PIX_FMT_BGR24,
                        SWS_BICUBIC, nullptr, nullptr, nullptr);

                    if (data->ConvertContext == nullptr)
                        throw gcnew VideoException("Cannot initialize video frame conversion context.");

                    // get some properties of the video file
                    m_width = data->VideoCodecContext->width;
                    m_height = data->VideoCodecContext->height;

                    AVRational fps = av_stream_get_r_frame_rate(data->VideoStream);
                    m_videoFrameRate = Rational(fps.num, fps.den);
                    m_videoCodecName = gcnew String(data->VideoCodecContext->codec->name);
                    m_videoFramesCount = data->VideoStream->nb_frames;
                    m_videoBitRate = data->VideoCodecContext->bit_rate;

                    if (data->AudioStream != nullptr)
                    {
                        // find decoder for the audio stream
                        AVCodec* audioCodec = avcodec_find_decoder(data->AudioCodecContext->codec_id);
                        if (audioCodec == nullptr)
                            throw gcnew VideoException("Cannot find codec to decode the audio stream.");

                        // open the codec
                        if (avcodec_open2(data->AudioCodecContext, audioCodec, nullptr) < 0)
                            throw gcnew VideoException("Cannot open audio codec.");

                        // allocate audio frame
                        data->AudioFrame = av_frame_alloc();

                        AVRational fps = av_stream_get_r_frame_rate(data->AudioStream);
                        m_audioFrameRate = Rational(fps.num, fps.den);
                        m_audioCodecName = gcnew String(data->AudioCodecContext->codec->name);
                        m_audioFramesCount = data->AudioStream->nb_frames;
                        m_audioBitRate = data->AudioCodecContext->bit_rate;
                    }

                    success = true;
                }
                finally
                {
                    if (!success)
                        Close();
                }
            }

            // Close current video file
            void VideoFileReader::Close()
            {
                if (data == nullptr)
                    return;

                if (data->VideoFrame != nullptr)
                    av_free(data->VideoFrame);

                if (data->VideoCodecContext != nullptr)
                    avcodec_close(data->VideoCodecContext);

                if (data->AudioFrame != nullptr)
                    av_free(data->AudioFrame);

                if (data->AudioCodecContext != nullptr)
                    avcodec_close(data->AudioCodecContext);

                if (data->FormatContext != nullptr)
                {
                    AVFormatContext* c = data->FormatContext;
                    avformat_close_input(&c);
                }

                if (data->ConvertContext != nullptr)
                    sws_freeContext(data->ConvertContext);

                if (data->Packet->data != nullptr)
                    av_free_packet(data->Packet);

                data = nullptr;
            }


            Bitmap^ VideoFileReader::readVideoFrame(int frameIndex, BitmapData^ image, System::Collections::Generic::IList<byte>^ audio)
            {
                if (frameIndex == -1)
                    frameIndex = data->nextFrameIndex;

                CheckIfDisposed();

                if (data == nullptr)
                    throw gcnew System::IO::IOException("Cannot read video frames since video file is not open.");

                int frameFinished;
                Bitmap^ bitmap = nullptr;
                int uint8_tsDecoded;
                bool streamHasFinished = false;
                bool needsToSeek = false;

                if (frameIndex != this->data->nextFrameIndex)
                {
                    needsToSeek = true;
                    avcodec_flush_buffers(data->VideoStream->codec);
                    int error = av_seek_frame(data->FormatContext, data->VideoStream->index, frameIndex, AVSEEK_FLAG_FRAME | AVSEEK_FLAG_BACKWARD);
                    if (error < 0)
                        throw gcnew VideoException("Error while seeking video frame.");
                }

                while (true)
                {
                    // work on the current packet until we have decoded all of it
                    while (data->uint8_tsRemaining > 0 || streamHasFinished)
                    {
                        if (data->VideoStream != nullptr && data->Packet->stream_index == data->VideoStream->index)
                        {
                            // decode the next chunk of data
                            uint8_tsDecoded = avcodec_decode_video2(data->VideoCodecContext, data->VideoFrame, &frameFinished, data->Packet);

                            // was there an error?
                            if (uint8_tsDecoded < 0)
                                throw gcnew VideoException("Error while decoding video frame.");

                            data->uint8_tsRemaining -= uint8_tsDecoded;

                            // did we finish the current frame? Then we can return
                            if (frameFinished || streamHasFinished)
                            {
                                if (!needsToSeek || data->VideoFrame->pts >= frameToPTS(data->VideoStream, frameIndex))
                                {
                                    data->nextFrameIndex = frameIndex + 1;
                                    return DecodeVideoFrame(image);
                                }
                            }
                        }
                        else if (data->AudioStream != nullptr && (data->Packet->stream_index == data->AudioStream->index))
                        {
                            // decode the next chunk of data
                            uint8_tsDecoded = avcodec_decode_audio4(data->AudioCodecContext, data->AudioFrame, &frameFinished, data->Packet);

                            // was there an error?
                            if (uint8_tsDecoded < 0)
                                throw gcnew VideoException("Error while decoding audio frame.");

                            data->uint8_tsRemaining -= uint8_tsDecoded;

                            // did we finish the current frame? Then we add it to the list of decoded audio 
                            // frames, but we don't return until the video frame has finished being decoded
                            if (audio != nullptr && frameFinished || streamHasFinished)
                            {
                                DecodeAudioFrame(audio);
                            }
                        }

                        if (streamHasFinished)
                            return nullptr;
                    }

                    // read the next packet, skipping all packets that aren't for 
                    // the streams that we are interested into (video and/or audio).
                    do
                    {
                        // free old packet if any
                        if (data->Packet->data != nullptr)
                        {
                            av_free_packet(data->Packet);
                            data->Packet->data = nullptr;
                        }

                        // read new packet
                        if (av_read_frame(data->FormatContext, data->Packet) < 0)
                        {
                            streamHasFinished = true;
                            return nullptr;
                        }

                        // Are we interested in this packet?
                        if (data->Packet->stream_index == data->VideoStream->index)
                            break; // yes we are interested in all video packets
                        if (data->AudioStream != nullptr && data->Packet->stream_index == data->AudioStream->index)
                            break; // yes, the video we has audio stream, so we are interested in audio packets too
                    } while (true);

                    data->uint8_tsRemaining = data->Packet->size;
                }

                throw gcnew System::InvalidOperationException("Execution should never reach here.");
            }

            // Decodes video frame into managed Bitmap
            Bitmap^ VideoFileReader::DecodeVideoFrame(BitmapData^ bitmapData)
            {
                Bitmap^ bitmap = nullptr;

                if (bitmapData == nullptr)
                {
                    // create a new Bitmap with format 24-bpp RGB
                    bitmap = gcnew Bitmap(data->VideoCodecContext->width, data->VideoCodecContext->height, PixelFormat::Format24bppRgb);

                    // lock the bitmap
                    bitmapData = bitmap->LockBits(
                        System::Drawing::Rectangle(0, 0, data->VideoCodecContext->width, data->VideoCodecContext->height),
                        ImageLockMode::WriteOnly, PixelFormat::Format24bppRgb);
                }

                uint8_t* srcData[4] = { static_cast<uint8_t*>(static_cast<void*>(bitmapData->Scan0)),
                    nullptr, nullptr, nullptr };
                int srcLinesize[4] = { bitmapData->Stride, 0, 0, 0 };

                // convert video frame to the RGB bitmap
                sws_scale(data->ConvertContext, data->VideoFrame->data, data->VideoFrame->linesize, 0,
                    data->VideoCodecContext->height, srcData, srcLinesize);

                if (bitmap != nullptr)
                    bitmap->UnlockBits(bitmapData); // unlock only if we have created the bitmap ourselves
                return bitmap;
            }

            // Decodes video frame into managed Bitmap
            System::Collections::Generic::IList<byte>^ VideoFileReader::DecodeAudioFrame(System::Collections::Generic::IList<byte>^ audio)
            {
                // convert audio frame to the 16-bit PCM signal
                int data_size = av_samples_get_buffer_size(
                    NULL,
                    data->AudioStream->codec->channels,
                    data->AudioFrame->nb_samples,
                    data->AudioStream->codec->sample_fmt,
                    1
                );

                for (size_t i = 0; i < data_size; i++)
                    audio->Add(*data->AudioFrame->data[i]);

                return audio;
            }


        }
    }
}
