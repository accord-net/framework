// Accord FFMPEG Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © AForge.NET, 2009-2011
// contacts@aforgenet.com
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
#include "VideoCodec.h"

namespace libffmpeg
{
    extern "C"
    {
#pragma warning(disable:4635) 
#pragma warning(disable:4244) 
#include "libavcodec\avcodec.h"
    }
}

int video_codecs[] =
{
    libffmpeg::CODEC_ID_MPEG4,
    libffmpeg::CODEC_ID_WMV1,
    libffmpeg::CODEC_ID_WMV2,
    libffmpeg::CODEC_ID_MSMPEG4V2,
    libffmpeg::CODEC_ID_MSMPEG4V3,
    libffmpeg::CODEC_ID_H263P,
    libffmpeg::CODEC_ID_FLV1,
    libffmpeg::CODEC_ID_MPEG2VIDEO,
    libffmpeg::CODEC_ID_RAWVIDEO,
    libffmpeg::CODEC_ID_FFV1,
    libffmpeg::CODEC_ID_FFVHUFF,
    libffmpeg::CODEC_ID_H264,
    libffmpeg::CODEC_ID_THEORA,
};

int pixel_formats[] =
{
    libffmpeg::PIX_FMT_YUV420P,
    libffmpeg::PIX_FMT_YUV420P,
    libffmpeg::PIX_FMT_YUV420P,
    libffmpeg::PIX_FMT_YUV420P,
    libffmpeg::PIX_FMT_YUV420P,
    libffmpeg::PIX_FMT_YUV420P,
    libffmpeg::PIX_FMT_YUV420P,
    libffmpeg::PIX_FMT_YUV420P,
    libffmpeg::PIX_FMT_BGR24,
    libffmpeg::PIX_FMT_YUV420P,
    libffmpeg::PIX_FMT_RGB32,
    libffmpeg::PIX_FMT_YUVJ420P,
    libffmpeg::PIX_FMT_YUV420P,
};

int CODECS_COUNT(sizeof(video_codecs) / sizeof(libffmpeg::CodecID));