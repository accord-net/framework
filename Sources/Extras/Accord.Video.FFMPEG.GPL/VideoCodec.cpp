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
#include "VideoCodec.h"

namespace libffmpeg
{
    extern "C"
    {
#include "libavcodec\avcodec.h"
    }
}

int video_codecs[] =
{
    libffmpeg::AV_CODEC_ID_MPEG4,
    libffmpeg::AV_CODEC_ID_WMV1,
    libffmpeg::AV_CODEC_ID_WMV2,
    libffmpeg::AV_CODEC_ID_MSMPEG4V2,
    libffmpeg::AV_CODEC_ID_MSMPEG4V3,
    libffmpeg::AV_CODEC_ID_H263P,
    libffmpeg::AV_CODEC_ID_FLV1,
    libffmpeg::AV_CODEC_ID_MPEG2VIDEO,
    libffmpeg::AV_CODEC_ID_RAWVIDEO,
    libffmpeg::AV_CODEC_ID_FFV1,
    libffmpeg::AV_CODEC_ID_FFVHUFF,
    libffmpeg::AV_CODEC_ID_H264,
    libffmpeg::AV_CODEC_ID_H265,
    libffmpeg::AV_CODEC_ID_THEORA,
    libffmpeg::AV_CODEC_ID_VP8,
    libffmpeg::AV_CODEC_ID_VP9,
};

int pixel_formats[] =
{
    libffmpeg::AV_PIX_FMT_YUV420P,  // libffmpeg::AV_CODEC_ID_MPEG4,
    libffmpeg::AV_PIX_FMT_YUV420P,  // libffmpeg::AV_CODEC_ID_WMV1,
    libffmpeg::AV_PIX_FMT_YUV420P,  // libffmpeg::AV_CODEC_ID_WMV2,
    libffmpeg::AV_PIX_FMT_YUV420P,  // libffmpeg::AV_CODEC_ID_MSMPEG4V2,
    libffmpeg::AV_PIX_FMT_YUV420P,  // libffmpeg::AV_CODEC_ID_MSMPEG4V3,
    libffmpeg::AV_PIX_FMT_YUV420P,  // libffmpeg::AV_CODEC_ID_H263P,
    libffmpeg::AV_PIX_FMT_YUV420P,  // libffmpeg::AV_CODEC_ID_FLV1,
    libffmpeg::AV_PIX_FMT_YUV420P,  // libffmpeg::AV_CODEC_ID_MPEG2VIDEO,
    libffmpeg::AV_PIX_FMT_BGR24,    // libffmpeg::AV_CODEC_ID_RAWVIDEO,
    libffmpeg::AV_PIX_FMT_YUV420P,  // libffmpeg::AV_CODEC_ID_FFV1,
    libffmpeg::AV_PIX_FMT_RGB32,    // libffmpeg::AV_CODEC_ID_FFVHUFF,
    libffmpeg::AV_PIX_FMT_YUVJ420P, // libffmpeg::AV_CODEC_ID_H264,
    libffmpeg::AV_PIX_FMT_YUVJ420P, // libffmpeg::AV_CODEC_ID_H265,
    libffmpeg::AV_PIX_FMT_YUV420P,  // libffmpeg::AV_CODEC_ID_THEORA,
    libffmpeg::AV_PIX_FMT_YUV420P,  // libffmpeg::AV_CODEC_ID_VP8,
    libffmpeg::AV_PIX_FMT_YUV420P,  // libffmpeg::AV_CODEC_ID_VP9,
};                                  
                                    

int CODECS_COUNT(sizeof(video_codecs) / sizeof(libffmpeg::AVCodecID));
