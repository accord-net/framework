// Accord FFMPEG Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright � AForge.NET, 2009-2011
// contacts@aforgenet.com
//
// Copyright � C�sar Souza, 2009-2016
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

#include <string>
#include <msclr\marshal_cppstd.h>

namespace libffmpeg
{
	extern "C"
	{
#include "libavformat\avformat.h"
#include "libavformat\avio.h"
#include "libavcodec\avcodec.h"
#include "libswscale\swscale.h"
	}
}

namespace Accord {
	namespace Video {
		namespace FFMPEG
		{
			// A structure to encapsulate all FFMPEG related private variable
			ref struct ReaderPrivateData
			{
			public:
				libffmpeg::AVFormatContext*		FormatContext;
				libffmpeg::AVStream*			VideoStream;
				libffmpeg::AVCodecContext*		CodecContext;
				libffmpeg::AVFrame*				VideoFrame;
				struct libffmpeg::SwsContext*	ConvertContext;

				libffmpeg::AVPacket* Packet;
				int uint8_tsRemaining;

				ReaderPrivateData()
				{
					FormatContext = nullptr;
					VideoStream = nullptr;
					CodecContext = nullptr;
					VideoFrame = nullptr;
					ConvertContext = nullptr;

					Packet = nullptr;
					uint8_tsRemaining = 0;
				}
			};

			// Class constructor
			VideoFileReader::VideoFileReader() :
				data(nullptr), disposed(false)
			{
				libffmpeg::av_register_all();
			}

#pragma managed(push, off)
			static libffmpeg::AVFormatContext* open_file(const char* fileName)
			{
				libffmpeg::AVFormatContext* formatContext;
				return (libffmpeg::avformat_open_input(&formatContext, fileName, nullptr, nullptr) != 0) 
					? nullptr : formatContext;
			}
#pragma managed(pop)

			// Opens the specified video file
			void VideoFileReader::Open(String^ fileName)
			{
				CheckIfDisposed();

				// close previous file if any was open
				Close();

				data = gcnew ReaderPrivateData();
				data->Packet = new libffmpeg::AVPacket();
				data->Packet->data = nullptr;

				bool success = false;

				try
				{					
					// convert specified managed String to UTF8 unmanaged string
					auto nativeFileName = msclr::interop::marshal_as<std::string>(fileName);

					// open the specified video file
					data->FormatContext = open_file(nativeFileName.c_str());
					if (data->FormatContext == nullptr)
						throw gcnew System::IO::IOException("Cannot open the video file.");

					// retrieve stream information
					if (libffmpeg::avformat_find_stream_info(data->FormatContext, nullptr) < 0)
						throw gcnew VideoException("Cannot find stream information.");

					// search for the first video stream
					for (unsigned int i = 0; i < data->FormatContext->nb_streams; i++)
					{
						if (data->FormatContext->streams[i]->codec->codec_type == libffmpeg::AVMEDIA_TYPE_VIDEO)
						{
							// get the pointer to the codec context for the video stream
							data->CodecContext = data->FormatContext->streams[i]->codec;
							data->VideoStream = data->FormatContext->streams[i];
							break;
						}
					}
					if (data->VideoStream == nullptr)
						throw gcnew VideoException("Cannot find video stream in the specified file.");

					// find decoder for the video stream
					libffmpeg::AVCodec* codec = libffmpeg::avcodec_find_decoder(data->CodecContext->codec_id);
					if (codec == nullptr)
						throw gcnew VideoException("Cannot find codec to decode the video stream.");

					// open the codec
					if (libffmpeg::avcodec_open2(data->CodecContext, codec, nullptr) < 0)
						throw gcnew VideoException("Cannot open video codec.");

					// allocate video frame
					data->VideoFrame = libffmpeg::av_frame_alloc();

					// prepare scaling context to convert RGB image to video format
					data->ConvertContext = libffmpeg::sws_getContext(data->CodecContext->width, data->CodecContext->height, data->CodecContext->pix_fmt,
						data->CodecContext->width, data->CodecContext->height, libffmpeg::AV_PIX_FMT_BGR24,
						SWS_BICUBIC, nullptr, nullptr, nullptr);

					if (data->ConvertContext == nullptr)
						throw gcnew VideoException("Cannot initialize frames conversion context.");

					// get some properties of the video file
					m_width = data->CodecContext->width;
					m_height = data->CodecContext->height;
					m_frameRate = data->VideoStream->r_frame_rate.num / data->VideoStream->r_frame_rate.den;
					m_codecName = gcnew String(data->CodecContext->codec->name);
					m_framesCount = data->VideoStream->nb_frames;

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
					libffmpeg::av_free(data->VideoFrame);

				if (data->CodecContext != nullptr)
					libffmpeg::avcodec_close(data->CodecContext);

				if (data->FormatContext != nullptr)
				{
					auto c = data->FormatContext;
					libffmpeg::avformat_close_input(&c);
				}

				if (data->ConvertContext != nullptr)
					libffmpeg::sws_freeContext(data->ConvertContext);

				if (data->Packet->data != nullptr)
					libffmpeg::av_free_packet(data->Packet);

				data = nullptr;
			}

			// Read next video frame of the current video file
			Bitmap^ VideoFileReader::ReadVideoFrame()
			{
				CheckIfDisposed();

				if (data == nullptr)
					throw gcnew System::IO::IOException("Cannot read video frames since video file is not open.");

				int frameFinished;
				Bitmap^ bitmap = nullptr;
				int uint8_tsDecoded;
				bool exit = false;

				while (true)
				{
					// work on the current packet until we have decoded all of it
					while (data->uint8_tsRemaining > 0)
					{
						// decode the next chunk of data
						uint8_tsDecoded = libffmpeg::avcodec_decode_video2(data->CodecContext, data->VideoFrame, &frameFinished, data->Packet);

						// was there an error?
						if (uint8_tsDecoded < 0)
							throw gcnew VideoException("Error while decoding frame.");

						data->uint8_tsRemaining -= uint8_tsDecoded;

						// did we finish the current frame? Then we can return
						if (frameFinished)
							return DecodeVideoFrame();
					}

					// read the next packet, skipping all packets that aren't for this stream
					do
					{
						// free old packet if any
						if (data->Packet->data != nullptr)
						{
							libffmpeg::av_free_packet(data->Packet);
							data->Packet->data = nullptr;
						}

						// read new packet
						if (libffmpeg::av_read_frame(data->FormatContext, data->Packet) < 0)
						{
							exit = true;
							break;
						}
					} while (data->Packet->stream_index != data->VideoStream->index);

					// exit ?
					if (exit)
						break;

					data->uint8_tsRemaining = data->Packet->size;
				}

				// decode the rest of the last frame
				uint8_tsDecoded = libffmpeg::avcodec_decode_video2(
					data->CodecContext, data->VideoFrame, &frameFinished, data->Packet);

				// free last packet
				if (data->Packet->data != nullptr)
				{
					libffmpeg::av_free_packet(data->Packet);
					data->Packet->data = nullptr;
				}

				// is there a frame
				if (frameFinished)
					bitmap = DecodeVideoFrame();

				return bitmap;
			}

			// Decodes video frame into managed Bitmap
			Bitmap^ VideoFileReader::DecodeVideoFrame()
			{
				Bitmap^ bitmap = gcnew Bitmap(data->CodecContext->width, data->CodecContext->height, PixelFormat::Format24bppRgb);

				// lock the bitmap
				BitmapData^ bitmapData = bitmap->LockBits(System::Drawing::Rectangle(0, 0, data->CodecContext->width, data->CodecContext->height),
					ImageLockMode::ReadOnly, PixelFormat::Format24bppRgb);
				
				uint8_t* srcData[4] = { static_cast<uint8_t*>(static_cast<void*>(bitmapData->Scan0)), 
					nullptr, nullptr, nullptr };
				int srcLinesize[4] = { bitmapData->Stride, 0, 0, 0 };

				// convert video frame to the RGB bitmap
				libffmpeg::sws_scale(data->ConvertContext, data->VideoFrame->data, data->VideoFrame->linesize, 0,
					data->CodecContext->height, srcData, srcLinesize);

				bitmap->UnlockBits(bitmapData);
				return bitmap;
			}
		}
	}
}
