// Accord FFMPEG Library
// The Accord.NET Framework
// http://accord-framework.net
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

extern "C"
{
#include "libavcodec\avcodec.h"
}


using namespace System;

namespace Accord {
    namespace Video {
        namespace FFMPEG
        {
            /// <summary>
            ///   Enumeration of some pixel formats from FFmpeg library, which 
            ///   are available for writing video files.
            /// </summary>
            ///
            public enum class AVPixelFormat
            {
                /// <summary>
                ///   No pixel format.
                /// </summary>
                None = AV_PIX_FMT_NONE,

                /// <summary>
                ///   Planar YUV 4:2:0, 12bpp, (1 Cr and Cb sample per 2x2 Y samples).
                /// </summary>
                FormatYuv420P = AV_PIX_FMT_YUV420P,

                /// <summary>
                ///  packed YUV 4:2:2, 16bpp, Y0 Cb Y1 Cr.
                /// </summary>
                FormatYuyv422 = AV_PIX_FMT_YUYV422,

                /// <summary>
                ///  packed RGB 8:8:8, 24bpp, RGBRGB...
                /// </summary>
                FormatRgb24bpp = AV_PIX_FMT_RGB24,

                /// <summary>
                ///  packed RGB 8:8:8, 24bpp, BGRBGR...
                /// </summary>
                FormatBgr24bpp = AV_PIX_FMT_BGR24,

                /// <summary>
                ///  planar YUV 4:2:2, 16bpp, (1 Cr and Cb sample per 2x1 Y samples)
                /// </summary>
                FormatYuv422P = AV_PIX_FMT_YUV422P,

                /// <summary>
                ///  planar YUV 4:4:4, 24bpp, (1 Cr and Cb sample per 1x1 Y samples)
                /// </summary>
                FormatYuv444P = AV_PIX_FMT_YUV444P,

                /// <summary>
                ///  planar YUV 4:1:0,  9bpp, (1 Cr and Cb sample per 4x4 Y samples)
                /// </summary>
                FormatYuv410P = AV_PIX_FMT_YUV410P,

                /// <summary>
                ///  planar YUV 4:1:1, 12bpp, (1 Cr and Cb sample per 4x1 Y samples)
                /// </summary>
                FormatYuv411P = AV_PIX_FMT_YUV411P,

                /// <summary>
                ///         Y        ,  8bpp
                /// </summary>
                FormatGrayscale8bpp = AV_PIX_FMT_GRAY8,

                /// <summary>
                ///         Y        ,  1bpp, 0 is white, 1 is black, in each byte pixels are ordered from the msb to the lsb
                /// </summary>
                FormatMonoWhite = AV_PIX_FMT_MONOWHITE,

                /// <summary>
                ///         Y        ,  1bpp, 0 is black, 1 is white, in each byte pixels are ordered from the msb to the lsb
                /// </summary>
                FormatMonoBlack = AV_PIX_FMT_MONOBLACK,

                /// <summary>
                ///  8 bits with AV_PIX_FMT_RGB32 palette
                /// </summary>
                FormatPal8bpp = AV_PIX_FMT_PAL8,

                /// <summary>
                ///  planar YUV 4:2:0, 12bpp, full scale (JPEG), deprecated in favor of AV_PIX_FMT_YUV420P and setting color_range
                /// </summary>
                FormatYuvj420P = AV_PIX_FMT_YUVJ420P,

                /// <summary>
                ///  planar YUV 4:2:2, 16bpp, full scale (JPEG), deprecated in favor of AV_PIX_FMT_YUV422P and setting color_range
                /// </summary>
                FormatYuvj422P = AV_PIX_FMT_YUVJ422P,

                /// <summary>
                ///  planar YUV 4:4:4, 24bpp, full scale (JPEG), deprecated in favor of AV_PIX_FMT_YUV444P and setting color_range
                /// </summary>
                FormatYuvj444P = AV_PIX_FMT_YUVJ444P,

                /// <summary>
                ///  XVideo Motion Acceleration via common packet passing
                /// </summary>
                FormatXvmcMpeg2Mc = AV_PIX_FMT_XVMC_MPEG2_MC,

                /// <summary>                
                ///   XVideo Motion Acceleration via inverse discrete cosine transform.
                /// </summary>
                FormatXvmcMpeg2Idct = AV_PIX_FMT_XVMC_MPEG2_IDCT,

                /// <summary>                
                ///   XVideo Motion Acceleration 
                /// </summary>
                FormatXvmc = AV_PIX_FMT_XVMC,

                /// <summary>
                ///  packed YUV 4:2:2, 16bpp, Cb Y0 Cr Y1
                /// </summary>
                FormatUyvy422 = AV_PIX_FMT_UYVY422,

                /// <summary>
                ///  packed YUV 4:1:1, 12bpp, Cb Y0 Y1 Cr Y2 Y3
                /// </summary>
                FormatUyyvyy411 = AV_PIX_FMT_UYYVYY411,

                /// <summary>
                ///  packed RGB 3:3:2,  8bpp, (msb)2B 3G 3R(lsb)
                /// </summary>
                FormatBgr8bpp = AV_PIX_FMT_BGR8,

                /// <summary>
                ///  packed RGB 1:2:1 bitstream,  4bpp, (msb)1B 2G 1R(lsb), a byte contains two pixels, the first pixel in the byte is the one composed by the 4 msb bits
                /// </summary>
                FormatBgr4bpp = AV_PIX_FMT_BGR4,

                /// <summary>
                ///  packed RGB 1:2:1,  8bpp, (msb)1B 2G 1R(lsb)
                /// </summary>
                FormatBgr4Byte = AV_PIX_FMT_BGR4_BYTE,

                /// <summary>
                ///  packed RGB 3:3:2,  8bpp, (msb)2R 3G 3B(lsb)
                /// </summary>
                FormatRgb8bpp = AV_PIX_FMT_RGB8,

                /// <summary>
                ///  packed RGB 1:2:1 bitstream,  4bpp, (msb)1R 2G 1B(lsb), a byte contains two pixels, the first pixel in the byte is the one composed by the 4 msb bits
                /// </summary>
                FormatRgb4bpp = AV_PIX_FMT_RGB4,

                /// <summary>
                ///  packed RGB 1:2:1,  8bpp, (msb)1R 2G 1B(lsb)
                /// </summary>
                FormatRgb4Byte = AV_PIX_FMT_RGB4_BYTE,

                /// <summary>
                ///  planar YUV 4:2:0, 12bpp, 1 plane for Y and 1 plane for the UV components, 
                ///  which are interleaved (first byte U and the following byte V).
                /// </summary>
                FormatNv12 = AV_PIX_FMT_NV12,

                /// <summary>
                ///  planar YUV 4:2:0, 12bpp, 1 plane for Y and 1 plane for the UV components, 
                ///  which are interleaved (first byte V and the following byte U).
                /// </summary>
                FormatNv21 = AV_PIX_FMT_NV21,

                /// <summary>
                ///  packed ARGB 8:8:8:8, 32bpp, ARGBARGB...
                /// </summary>
                FormatArgb32bpp = AV_PIX_FMT_ARGB,

                /// <summary>
                ///  packed RGBA 8:8:8:8, 32bpp, RGBARGBA...
                /// </summary>
                FormatRgba32bpp = AV_PIX_FMT_RGBA,

                /// <summary>
                ///  packed ABGR 8:8:8:8, 32bpp, ABGRABGR...
                /// </summary>
                FormatAbgr32bpp = AV_PIX_FMT_ABGR,

                /// <summary>
                ///  packed BGRA 8:8:8:8, 32bpp, BGRABGRA...
                /// </summary>
                FormatBgra32bpp = AV_PIX_FMT_BGRA,

                /// <summary>
                ///         Y        , 16bpp, big-endian
                /// </summary>
                FormatGrayscale16bppBigEndian = AV_PIX_FMT_GRAY16BE,

                /// <summary>
                ///         Y        , 16bpp, little-endian
                /// </summary>
                FormatGrayscale16bppLittleEndian = AV_PIX_FMT_GRAY16LE,

                /// <summary>
                ///  planar YUV 4:4:0 (1 Cr and Cb sample per 1x2 Y samples)
                /// </summary>
                FormatYuv440P = AV_PIX_FMT_YUV440P,

                /// <summary>
                ///  planar YUV 4:4:0 full scale (JPEG), deprecated in 
                ///  favor of AV_PIX_FMT_YUV440P and setting color_range
                /// </summary>
                FormatYuvj440P = AV_PIX_FMT_YUVJ440P,

                /// <summary>
                ///  planar YUV 4:2:0, 20bpp, (1 Cr and Cb sample per 2x2 Y and A samples)
                /// </summary>
                FormatYuva420P = AV_PIX_FMT_YUVA420P,

                /// <summary>
                ///  H.264 HW decoding with VDPAU, data[0] contains a vdpau_render_state struct which contains the bitstream of the slices as well as various fields extracted from headers
                /// </summary>
                FormatVdpauH264 = AV_PIX_FMT_VDPAU_H264,

                /// <summary>
                ///  MPEG-1 HW decoding with VDPAU, data[0] contains a vdpau_render_state struct which contains the bitstream of the slices as well as various fields extracted from headers
                /// </summary>
                FormatVdpauMpeg1 = AV_PIX_FMT_VDPAU_MPEG1,

                /// <summary>
                ///  MPEG-2 HW decoding with VDPAU, data[0] contains a vdpau_render_state struct which contains the bitstream of the slices as well as various fields extracted from headers
                /// </summary>
                FormatVdpauMpeg2 = AV_PIX_FMT_VDPAU_MPEG2,

                /// <summary>
                ///  WMV3 HW decoding with VDPAU, data[0] contains a vdpau_render_state struct which contains the bitstream of the slices as well as various fields extracted from headers
                /// </summary>
                FormatVdpauWmv3 = AV_PIX_FMT_VDPAU_WMV3,

                /// <summary>
                ///  VC-1 HW decoding with VDPAU, data[0] contains a vdpau_render_state struct which contains the bitstream of the slices as well as various fields extracted from headers
                /// </summary>
                FormatVdpauVc1 = AV_PIX_FMT_VDPAU_VC1,

                /// <summary>
                ///  packed RGB 16:16:16, 48bpp, 16R, 16G, 16B, the 2-byte value for each R/G/B component is stored as big-endian
                /// </summary>
                FormatRgb48bppBigEndian = AV_PIX_FMT_RGB48BE,

                /// <summary>
                ///  packed RGB 16:16:16, 48bpp, 16R, 16G, 16B, the 2-byte value for each R/G/B component is stored as little-endian
                /// </summary>
                FormatRgb48bppLittleEndian = AV_PIX_FMT_RGB48LE,

                /// <summary>
                ///  packed RGB 5:6:5, 16bpp, (msb)   5R 6G 5B(lsb), big-endian
                /// </summary>
                FormatRgb565BigEndian = AV_PIX_FMT_RGB565BE,

                /// <summary>
                ///  packed RGB 5:6:5, 16bpp, (msb)   5R 6G 5B(lsb), little-endian
                /// </summary>
                FormatRgb565LittleEndian = AV_PIX_FMT_RGB565LE,

                /// <summary>
                ///  packed RGB 5:5:5, 16bpp, (msb)1X 5R 5G 5B(lsb), big-endian   , X=unused/undefined
                /// </summary>
                FormatRgb555BigEndian = AV_PIX_FMT_RGB555BE,

                /// <summary>
                ///  packed RGB 5:5:5, 16bpp, (msb)1X 5R 5G 5B(lsb), little-endian, X=unused/undefined
                /// </summary>
                FormatRgb555LittleEndian = AV_PIX_FMT_RGB555LE,

                /// <summary>
                ///  packed BGR 5:6:5, 16bpp, (msb)   5B 6G 5R(lsb), big-endian
                /// </summary>
                FormatBgr565BigEndian = AV_PIX_FMT_BGR565BE,

                /// <summary>
                ///  packed BGR 5:6:5, 16bpp, (msb)   5B 6G 5R(lsb), little-endian
                /// </summary>
                FormatBgr565LittleEndian = AV_PIX_FMT_BGR565LE,

                /// <summary>
                ///  packed BGR 5:5:5, 16bpp, (msb)1X 5B 5G 5R(lsb), big-endian   , X=unused/undefined
                /// </summary>
                FormatBgr555BigEndian = AV_PIX_FMT_BGR555BE,

                /// <summary>
                ///  packed BGR 5:5:5, 16bpp, (msb)1X 5B 5G 5R(lsb), little-endian, X=unused/undefined
                /// </summary>
                FormatBgr555LittleEndian = AV_PIX_FMT_BGR555LE,

                /// <summary>
                ///  HW acceleration through VA API at motion compensation entry-point, Picture.data[3] contains a vaapi_render_state struct which contains macroblocks as well as various fields extracted from headers
                /// </summary>
                FormatVaApiMoco = AV_PIX_FMT_VAAPI_MOCO,

                /// <summary>
                ///  HW acceleration through VA API at IDCT entry-point, Picture.data[3] contains a vaapi_render_state struct which contains fields extracted from headers
                /// </summary>
                FormatVaApiIdct = AV_PIX_FMT_VAAPI_IDCT,

                /// <summary>
                ///  HW decoding through VA API, Picture.data[3] contains a VASurfaceID
                /// </summary>
                FormatVaApiVld,


                FormatVAAPI = AV_PIX_FMT_VAAPI_VLD,

                /// <summary>
                ///  planar YUV 4:2:0, 24bpp, (1 Cr and Cb sample per 2x2 Y samples), little-endian
                /// </summary>
                FormatYuv420P16LittleEndian = AV_PIX_FMT_YUV420P16LE,

                /// <summary>
                ///  planar YUV 4:2:0, 24bpp, (1 Cr and Cb sample per 2x2 Y samples), big-endian
                /// </summary>
                FormatYuv420P16BigEndian = AV_PIX_FMT_YUV420P16BE,

                /// <summary>
                ///  planar YUV 4:2:2, 32bpp, (1 Cr and Cb sample per 2x1 Y samples), little-endian
                /// </summary>
                FormatYuv422P16LittleEndian = AV_PIX_FMT_YUV422P16LE,

                /// <summary>
                ///  planar YUV 4:2:2, 32bpp, (1 Cr and Cb sample per 2x1 Y samples), big-endian
                /// </summary>
                FormatYuv422P16BigEndian = AV_PIX_FMT_YUV422P16BE,

                /// <summary>
                ///  planar YUV 4:4:4, 48bpp, (1 Cr and Cb sample per 1x1 Y samples), little-endian
                /// </summary>
                FormatYuv444P16LittleEndian = AV_PIX_FMT_YUV444P16LE,

                /// <summary>
                ///  planar YUV 4:4:4, 48bpp, (1 Cr and Cb sample per 1x1 Y samples), big-endian
                /// </summary>
                FormatYuv444P16BigEndian = AV_PIX_FMT_YUV444P16BE,

                /// <summary>
                ///  MPEG-4 HW decoding with VDPAU, data[0] contains a vdpau_render_state struct which contains the bitstream of the slices as well as various fields extracted from headers
                /// </summary>
                FormatVdpauMpeg4 = AV_PIX_FMT_VDPAU_MPEG4,

                /// <summary>
                ///  HW decoding through DXVA2, Picture.data[3] contains a LPDIRECT3DSURFACE9 pointer
                /// </summary>
                FormatDxva2Vld = AV_PIX_FMT_DXVA2_VLD,

                /// <summary>
                ///  packed RGB 4:4:4, 16bpp, (msb)4X 4R 4G 4B(lsb), little-endian, X=unused/undefined
                /// </summary>
                FormatRgb444LittleEndian = AV_PIX_FMT_RGB444LE,

                /// <summary>
                ///  packed RGB 4:4:4, 16bpp, (msb)4X 4R 4G 4B(lsb), big-endian,    X=unused/undefined
                /// </summary>
                FormatRgb444BigEndian = AV_PIX_FMT_RGB444BE,

                /// <summary>
                ///  packed BGR 4:4:4, 16bpp, (msb)4X 4B 4G 4R(lsb), little-endian, X=unused/undefined
                /// </summary>
                FormatBgr444LittleEndian = AV_PIX_FMT_BGR444LE,

                /// <summary>
                ///  packed BGR 4:4:4, 16bpp, (msb)4X 4B 4G 4R(lsb), big-endian,    X=unused/undefined
                /// </summary>
                FormatBgr444BigEndian = AV_PIX_FMT_BGR444BE,

                /// <summary>
                ///  8 bits gray, 8 bits alpha
                /// </summary>
                FormatYa8 = AV_PIX_FMT_YA8,

                /// <summary>
                ///  alias for AV_PIX_FMT_YA8
                /// </summary>
                FormatY400A = AV_PIX_FMT_YA8,

                /// <summary>
                ///  alias for AV_PIX_FMT_YA8
                /// </summary>
                FormatGrayscale8A = AV_PIX_FMT_YA8,

                /// <summary>
                ///  packed RGB 16:16:16, 48bpp, 16B, 16G, 16R, the 2-byte value for each R/G/B component is stored as big-endian
                /// </summary>
                FormatBgr48BigEndian = AV_PIX_FMT_BGR48BE,

                /// <summary>
                ///  packed RGB 16:16:16, 48bpp, 16B, 16G, 16R, the 2-byte value for each R/G/B component is stored as little-endian
                /// </summary>
                FormatBgr48LittleEndian = AV_PIX_FMT_BGR48LE,

                /**
                * The following 12 formats have the disadvantage of needing 1 format for each bit depth.
                * Notice that each 9/10 bits sample is stored in 16 bits with extra padding.
                * If you want to support multiple bit depths, then using AV_PIX_FMT_YUV420P16* with the bpp stored separately is better.
                */
                /// <summary>
                ///  planar YUV 4:2:0, 13.5bpp, (1 Cr and Cb sample per 2x2 Y samples), big-endian
                /// </summary>
                FormatYuv420P9BigEndian = AV_PIX_FMT_YUV420P9BE,

                /// <summary>
                ///  planar YUV 4:2:0, 13.5bpp, (1 Cr and Cb sample per 2x2 Y samples), little-endian
                /// </summary>
                FormatYuv420P9LittleEndian = AV_PIX_FMT_YUV420P9LE,

                /// <summary>
                ///  planar YUV 4:2:0, 15bpp, (1 Cr and Cb sample per 2x2 Y samples), big-endian
                /// </summary>
                FormatYuv420P10BigEndian = AV_PIX_FMT_YUV420P10BE,

                /// <summary>
                ///  planar YUV 4:2:0, 15bpp, (1 Cr and Cb sample per 2x2 Y samples), little-endian
                /// </summary>
                FormatYuv420P10LittleEndian = AV_PIX_FMT_YUV420P10LE,

                /// <summary>
                ///  planar YUV 4:2:2, 20bpp, (1 Cr and Cb sample per 2x1 Y samples), big-endian
                /// </summary>
                FormatYuv422P10BigEndian = AV_PIX_FMT_YUV422P10BE,

                /// <summary>
                ///  planar YUV 4:2:2, 20bpp, (1 Cr and Cb sample per 2x1 Y samples), little-endian
                /// </summary>
                FormatYuv422P10LittleEndian = AV_PIX_FMT_YUV422P10LE,

                /// <summary>
                ///  planar YUV 4:4:4, 27bpp, (1 Cr and Cb sample per 1x1 Y samples), big-endian
                /// </summary>
                FormatYuv444P9BigEndian = AV_PIX_FMT_YUV444P9BE,

                /// <summary>
                ///  planar YUV 4:4:4, 27bpp, (1 Cr and Cb sample per 1x1 Y samples), little-endian
                /// </summary>
                FormatYuv444P9LittleEndian = AV_PIX_FMT_YUV444P9LE,

                /// <summary>
                ///  planar YUV 4:4:4, 30bpp, (1 Cr and Cb sample per 1x1 Y samples), big-endian
                /// </summary>
                FormatYuv444P10BigEndian = AV_PIX_FMT_YUV444P10BE,

                /// <summary>
                ///  planar YUV 4:4:4, 30bpp, (1 Cr and Cb sample per 1x1 Y samples), little-endian
                /// </summary>
                FormatYuv444P10LittleEndian = AV_PIX_FMT_YUV444P10LE,

                /// <summary>
                ///  planar YUV 4:2:2, 18bpp, (1 Cr and Cb sample per 2x1 Y samples), big-endian
                /// </summary>
                FormatYuv422P9BigEndian = AV_PIX_FMT_YUV422P9BE,

                /// <summary>
                ///  planar YUV 4:2:2, 18bpp, (1 Cr and Cb sample per 2x1 Y samples), little-endian
                /// </summary>
                FormatYuv422P9LE,

                /// <summary>
                ///  hardware decoding through VDA
                /// </summary>
                FormatVdaVld,

                /// <summary>
                ///  planar GBR 4:4:4 24bpp
                /// </summary>
                FormatGbrP,

                /// <summary>
                ///   alias for #AV_PIX_FMT_GBRP
                /// </summary>
                FormatGbr24P = AV_PIX_FMT_GBRP,

                /// <summary>
                ///  planar GBR 4:4:4 27bpp, big-endian
                /// </summary>
                FormatGbrP9BigEndian = AV_PIX_FMT_GBRP9BE,

                /// <summary>
                ///  planar GBR 4:4:4 27bpp, little-endian
                /// </summary>
                FormatGbrP9LittleEndian = AV_PIX_FMT_GBRP9LE,

                /// <summary>
                ///  planar GBR 4:4:4 30bpp, big-endian
                /// </summary>
                FormatGbrP10BigEndian = AV_PIX_FMT_GBRP10BE,

                /// <summary>
                ///  planar GBR 4:4:4 30bpp, little-endian
                /// </summary>
                FormatGbrP10LittleEndian = AV_PIX_FMT_GBRP10LE,

                /// <summary>
                ///  planar GBR 4:4:4 48bpp, big-endian
                /// </summary>
                FormatGbrP16BigEndian = AV_PIX_FMT_GBRP16BE,

                /// <summary>
                ///  planar GBR 4:4:4 48bpp, little-endian
                /// </summary>
                FormatGbrP16LittleEndian = AV_PIX_FMT_GBRP16LE,

                /// <summary>
                ///  planar YUV 4:2:2 24bpp, (1 Cr and Cb sample per 2x1 Y and A samples)
                /// </summary>
                FormatYuva422P = AV_PIX_FMT_YUVA422P,

                /// <summary>
                ///  planar YUV 4:4:4 32bpp, (1 Cr and Cb sample per 1x1 Y and A samples)
                /// </summary>
                FormatYuva444P = AV_PIX_FMT_YUVA444P,

                /// <summary>
                ///  planar YUV 4:2:0 22.5bpp, (1 Cr and Cb sample per 2x2 Y and A samples), big-endian
                /// </summary>
                FormatYuva420P9BigEndian = AV_PIX_FMT_YUVA420P9BE,

                /// <summary>
                ///  planar YUV 4:2:0 22.5bpp, (1 Cr and Cb sample per 2x2 Y and A samples), little-endian
                /// </summary>
                FormatYuva420P9LittleEndian = AV_PIX_FMT_YUVA420P9LE,

                /// <summary>
                ///  planar YUV 4:2:2 27bpp, (1 Cr and Cb sample per 2x1 Y and A samples), big-endian
                /// </summary>
                FormatYuva422P9BigEndian = AV_PIX_FMT_YUVA422P9BE,

                /// <summary>
                ///  planar YUV 4:2:2 27bpp, (1 Cr and Cb sample per 2x1 Y and A samples), little-endian
                /// </summary>
                FormatYuva422P9LittleEndian = AV_PIX_FMT_YUVA422P9LE,

                /// <summary>
                ///  planar YUV 4:4:4 36bpp, (1 Cr and Cb sample per 1x1 Y and A samples), big-endian
                /// </summary>
                FormatYuva444P9BigEndian = AV_PIX_FMT_YUVA444P9BE,

                /// <summary>
                ///  planar YUV 4:4:4 36bpp, (1 Cr and Cb sample per 1x1 Y and A samples), little-endian
                /// </summary>
                FormatYuva444P9LittleEndian = AV_PIX_FMT_YUVA444P9LE,

                /// <summary>
                ///  planar YUV 4:2:0 25bpp, (1 Cr and Cb sample per 2x2 Y and A samples, big-endian)
                /// </summary>
                FormatYuva420P10BigEndian = AV_PIX_FMT_YUVA420P10BE,

                /// <summary>
                ///  planar YUV 4:2:0 25bpp, (1 Cr and Cb sample per 2x2 Y and A samples, little-endian)
                /// </summary>
                FormatYuva420P10LittleEndian = AV_PIX_FMT_YUVA420P10LE,

                /// <summary>
                ///  planar YUV 4:2:2 30bpp, (1 Cr and Cb sample per 2x1 Y and A samples, big-endian)
                /// </summary>
                FormatYuva422P10BigEndian = AV_PIX_FMT_YUVA422P10BE,

                /// <summary>
                ///  planar YUV 4:2:2 30bpp, (1 Cr and Cb sample per 2x1 Y and A samples, little-endian)
                /// </summary>
                FormatYuva422P10LittleEndian = AV_PIX_FMT_YUVA422P10LE,

                /// <summary>
                ///  planar YUV 4:4:4 40bpp, (1 Cr and Cb sample per 1x1 Y and A samples, big-endian)
                /// </summary>
                FormatYuva444P10BigEndian = AV_PIX_FMT_YUVA444P10BE,

                /// <summary>
                ///  planar YUV 4:4:4 40bpp, (1 Cr and Cb sample per 1x1 Y and A samples, little-endian)
                /// </summary>
                FormatYuva444P10LittleEndian = AV_PIX_FMT_YUVA444P10LE,

                /// <summary>
                ///  planar YUV 4:2:0 40bpp, (1 Cr and Cb sample per 2x2 Y and A samples, big-endian)
                /// </summary>
                FormatYuva420P16BigEndian = AV_PIX_FMT_YUVA420P16BE,

                /// <summary>
                ///  planar YUV 4:2:0 40bpp, (1 Cr and Cb sample per 2x2 Y and A samples, little-endian)
                /// </summary>
                FormatYuva420P16LittleEndian = AV_PIX_FMT_YUVA420P16LE,

                /// <summary>
                ///  planar YUV 4:2:2 48bpp, (1 Cr and Cb sample per 2x1 Y and A samples, big-endian)
                /// </summary>
                FormatYuva422P16BigEndian = AV_PIX_FMT_YUVA422P16BE,

                /// <summary>
                ///  planar YUV 4:2:2 48bpp, (1 Cr and Cb sample per 2x1 Y and A samples, little-endian)
                /// </summary>
                FormatYuva422P16LittleEndian = AV_PIX_FMT_YUVA422P16LE,

                /// <summary>
                ///  planar YUV 4:4:4 64bpp, (1 Cr and Cb sample per 1x1 Y and A samples, big-endian)
                /// </summary>
                FormatYuva444P16BigEndian = AV_PIX_FMT_YUVA444P16BE,

                /// <summary>
                ///  planar YUV 4:4:4 64bpp, (1 Cr and Cb sample per 1x1 Y and A samples, little-endian)
                /// </summary>
                FormatYuva444P16LittleEndian = AV_PIX_FMT_YUVA444P16LE,

                /// <summary>
                ///  HW acceleration through VDPAU, Picture.data[3] contains a VdpVideoSurface
                /// </summary>
                FormatVdpau = AV_PIX_FMT_VDPAU,

                /// <summary>
                ///  packed XYZ 4:4:4, 36 bpp, (msb) 12X, 12Y, 12Z (lsb), the 2-byte value for each X/Y/Z is stored as little-endian, the 4 lower bits are set to 0
                /// </summary>
                FormatXyz12LittleEndian = AV_PIX_FMT_XYZ12LE,

                /// <summary>
                ///  packed XYZ 4:4:4, 36 bpp, (msb) 12X, 12Y, 12Z (lsb), the 2-byte value for each X/Y/Z is stored as big-endian, the 4 lower bits are set to 0
                /// </summary>
                FormatXyz12BigEndian = AV_PIX_FMT_XYZ12BE,

                /// <summary>
                ///  interleaved chroma YUV 4:2:2, 16bpp, (1 Cr and Cb sample per 2x1 Y samples)
                /// </summary>
                FormatNv16 = AV_PIX_FMT_NV16,

                /// <summary>
                ///  interleaved chroma YUV 4:2:2, 20bpp, (1 Cr and Cb sample per 2x1 Y samples), little-endian
                /// </summary>
                FormatNv20LittleEndian = AV_PIX_FMT_NV20LE,

                /// <summary>
                ///  interleaved chroma YUV 4:2:2, 20bpp, (1 Cr and Cb sample per 2x1 Y samples), big-endian
                /// </summary>
                FormatNv20BigEndian = AV_PIX_FMT_NV20BE,

                /// <summary>
                ///  packed RGBA 16:16:16:16, 64bpp, 16R, 16G, 16B, 16A, the 2-byte value for each R/G/B/A component is stored as big-endian
                /// </summary>
                FormatRgba64BigEndian = AV_PIX_FMT_RGBA64BE,

                /// <summary>
                ///  packed RGBA 16:16:16:16, 64bpp, 16R, 16G, 16B, 16A, the 2-byte value for each R/G/B/A component is stored as little-endian
                /// </summary>
                FormatRgba64LittleEndian = AV_PIX_FMT_RGBA64LE,

                /// <summary>
                ///  packed RGBA 16:16:16:16, 64bpp, 16B, 16G, 16R, 16A, the 2-byte value for each R/G/B/A component is stored as big-endian
                /// </summary>
                FormatBgra64BigEndian = AV_PIX_FMT_BGRA64BE,

                /// <summary>
                ///  packed RGBA 16:16:16:16, 64bpp, 16B, 16G, 16R, 16A, the 2-byte value for each R/G/B/A component is stored as little-endian
                /// </summary>
                FormatBgra64LittleEndian = AV_PIX_FMT_BGRA64LE,

                /// <summary>
                ///  packed YUV 4:2:2, 16bpp, Y0 Cr Y1 Cb
                /// </summary>
                FormatYvyu422 = AV_PIX_FMT_YVYU422,

                /// <summary>
                ///  HW acceleration through VDA, data[3] contains a CVPixelBufferRef
                /// </summary>
                FormatVda = AV_PIX_FMT_VDA,

                /// <summary>
                ///  16 bits gray, 16 bits alpha (big-endian)
                /// </summary>
                FormatYa16BigEndian = AV_PIX_FMT_YA16BE,

                /// <summary>
                ///  16 bits gray, 16 bits alpha (little-endian)
                /// </summary>
                FormatYa16LittleEndian = AV_PIX_FMT_YA16LE,

                /// <summary>
                ///  planar GBRA 4:4:4:4 32bpp
                /// </summary>
                FormatGbraP = AV_PIX_FMT_GBRAP,

                /// <summary>
                ///  planar GBRA 4:4:4:4 64bpp, big-endian
                /// </summary>
                FormatGbraP16BigEndian = AV_PIX_FMT_GBRAP16BE,

                /// <summary>
                ///  planar GBRA 4:4:4:4 64bpp, little-endian
                /// </summary>
                FormatGbraP16LittleEndian = AV_PIX_FMT_GBRAP16LE,

                /**
                *  HW acceleration through QSV, data[3] contains a pointer to the
                *  mfxFrameSurface1 structure.
                */
                FormatQsv = AV_PIX_FMT_QSV,

                /**
                * HW acceleration though MMAL, data[3] contains a pointer to the
                * MMAL_BUFFER_HEADER_T structure.
                */
                FormatMmal = AV_PIX_FMT_MMAL,

                /// <summary>
                ///  HW decoding through Direct3D11, Picture.data[3] contains a ID3D11VideoDecoderOutputView pointer
                /// </summary>
                FormatD3d11VaVld,

                /**
                * HW acceleration through CUDA. data[i] contain CUdeviceptr pointers
                * exactly as for system memory frames.
                */
                FormatCuda = AV_PIX_FMT_CUDA,

                /// <summary>
                ///  packed RGB 8:8:8, 32bpp, XRGBXRGB...   X=unused/undefined
                /// </summary>
                Format0Rgb = 0x123 + 4,

                /// <summary>
                ///  packed RGB 8:8:8, 32bpp, RGBXRGBX...   X=unused/undefined
                /// </summary>
                FormatRgb0 = AV_PIX_FMT_RGB0,

                /// <summary>
                ///  packed BGR 8:8:8, 32bpp, XBGRXBGR...   X=unused/undefined
                /// </summary>
                Format0Bgr = AV_PIX_FMT_0BGR,

                /// <summary>
                ///  packed BGR 8:8:8, 32bpp, BGRXBGRX...   X=unused/undefined
                /// </summary>
                FormatBgr0 = AV_PIX_FMT_BGR0,

                /// <summary>
                ///  planar YUV 4:2:0,18bpp, (1 Cr and Cb sample per 2x2 Y samples), big-endian
                /// </summary>
                FormatYuv420P12BigEndian = AV_PIX_FMT_YUV420P12BE,

                /// <summary>
                ///  planar YUV 4:2:0,18bpp, (1 Cr and Cb sample per 2x2 Y samples), little-endian
                /// </summary>
                FormatYuv420P12LittleEndian = AV_PIX_FMT_YUV420P12LE,

                /// <summary>
                ///  planar YUV 4:2:0,21bpp, (1 Cr and Cb sample per 2x2 Y samples), big-endian
                /// </summary>
                FormatYuv420P14BigEndian = AV_PIX_FMT_YUV420P14BE,

                /// <summary>
                ///  planar YUV 4:2:0,21bpp, (1 Cr and Cb sample per 2x2 Y samples), little-endian
                /// </summary>
                FormatYuv420P14LittleEndian = AV_PIX_FMT_YUV420P14LE,

                /// <summary>
                ///  planar YUV 4:2:2,24bpp, (1 Cr and Cb sample per 2x1 Y samples), big-endian
                /// </summary>
                FormatYuv422P12BigEndian = AV_PIX_FMT_YUV422P12BE,

                /// <summary>
                ///  planar YUV 4:2:2,24bpp, (1 Cr and Cb sample per 2x1 Y samples), little-endian
                /// </summary>
                FormatYuv422P12LittleEndian = AV_PIX_FMT_YUV422P12LE,

                /// <summary>
                ///  planar YUV 4:2:2,28bpp, (1 Cr and Cb sample per 2x1 Y samples), big-endian
                /// </summary>
                FormatYuv422P14BigEndian = AV_PIX_FMT_YUV422P14BE,

                /// <summary>
                ///  planar YUV 4:2:2,28bpp, (1 Cr and Cb sample per 2x1 Y samples), little-endian
                /// </summary>
                FormatYuv422P14LittleEndian = AV_PIX_FMT_YUV422P14LE,

                /// <summary>
                ///  planar YUV 4:4:4,36bpp, (1 Cr and Cb sample per 1x1 Y samples), big-endian
                /// </summary>
                FormatYuv444P12BigEndian = AV_PIX_FMT_YUV444P12BE,

                /// <summary>
                ///  planar YUV 4:4:4,36bpp, (1 Cr and Cb sample per 1x1 Y samples), little-endian
                /// </summary>
                FormatYuv444P12LittleEndian = AV_PIX_FMT_YUV444P12LE,

                /// <summary>
                ///  planar YUV 4:4:4,42bpp, (1 Cr and Cb sample per 1x1 Y samples), big-endian
                /// </summary>
                FormatYuv444P14BigEndian = AV_PIX_FMT_YUV444P14BE,

                /// <summary>
                ///  planar YUV 4:4:4,42bpp, (1 Cr and Cb sample per 1x1 Y samples), little-endian
                /// </summary>
                FormatYuv444P14LittleEndian = AV_PIX_FMT_YUV444P14LE,

                /// <summary>
                ///  planar GBR 4:4:4 36bpp, big-endian
                /// </summary>
                FormatGbrP12BigEndian = AV_PIX_FMT_GBRP12BE,

                /// <summary>
                ///  planar GBR 4:4:4 36bpp, little-endian
                /// </summary>
                FormatGbrP12LittleEndian = AV_PIX_FMT_GBRP12LE,

                /// <summary>
                ///  planar GBR 4:4:4 42bpp, big-endian
                /// </summary>
                FormatGbrP14BigEndian = AV_PIX_FMT_GBRP14BE,

                /// <summary>
                ///  planar GBR 4:4:4 42bpp, little-endian
                /// </summary>
                FormatGbrP14LittleEndian = AV_PIX_FMT_GBRP14LE,

                /// <summary>
                ///  planar YUV 4:1:1, 12bpp, (1 Cr and Cb sample per 4x1 Y samples) full scale (JPEG), deprecated in favor of AV_PIX_FMT_YUV411P and setting color_range
                /// </summary>
                FormatYuvj411P = AV_PIX_FMT_YUVJ411P,

                /// <summary>
                ///  bayer, BGBG..(odd line), GRGR..(even line), 8-bit samples */
                /// </summary>
                FormatBayerBggr8 = AV_PIX_FMT_BAYER_BGGR8,

                /// <summary>
                ///  bayer, RGRG..(odd line), GBGB..(even line), 8-bit samples */
                /// </summary>
                FormatBayerRggb8 = AV_PIX_FMT_BAYER_RGGB8,

                /// <summary>
                ///  bayer, GBGB..(odd line), RGRG..(even line), 8-bit samples */
                /// </summary>
                FormatBayerGbrg8 = AV_PIX_FMT_BAYER_GBRG8,

                /// <summary>
                ///  bayer, GRGR..(odd line), BGBG..(even line), 8-bit samples */
                /// </summary>
                FormatBayerGrbg8 = AV_PIX_FMT_BAYER_GRBG8,

                /// <summary>
                ///  bayer, BGBG..(odd line), GRGR..(even line), 16-bit samples, little-endian */
                /// </summary>
                FormatBayerBggr16LittleEndian = AV_PIX_FMT_BAYER_BGGR16LE,

                /// <summary>
                ///  bayer, BGBG..(odd line), GRGR..(even line), 16-bit samples, big-endian */
                /// </summary>
                FormatBayerBggr16BigEndian = AV_PIX_FMT_BAYER_BGGR16BE,

                /// <summary>
                ///  bayer, RGRG..(odd line), GBGB..(even line), 16-bit samples, little-endian */
                /// </summary>
                FormatBayerRggb16LittleEndian = AV_PIX_FMT_BAYER_RGGB16LE,

                /// <summary>
                ///  bayer, RGRG..(odd line), GBGB..(even line), 16-bit samples, big-endian */
                /// </summary>
                FormatBayerRggb16BigEndian = AV_PIX_FMT_BAYER_RGGB16BE,

                /// <summary>
                ///  bayer, GBGB..(odd line), RGRG..(even line), 16-bit samples, little-endian */
                /// </summary>
                FormatBayerGbrg16LittleEndian = AV_PIX_FMT_BAYER_GBRG16LE,

                /// <summary>
                ///  bayer, GBGB..(odd line), RGRG..(even line), 16-bit samples, big-endian */
                /// </summary>
                FormatBayerGbrg16BigEndian = AV_PIX_FMT_BAYER_GBRG16BE,

                /// <summary>
                ///  bayer, GRGR..(odd line), BGBG..(even line), 16-bit samples, little-endian */
                /// </summary>
                FormatBayerGrbg16LittleEndian = AV_PIX_FMT_BAYER_GRBG16LE,

                /// <summary>
                ///  bayer, GRGR..(odd line), BGBG..(even line), 16-bit samples, big-endian */
                /// </summary>
                FormatBayerGrbg16BigEndian = AV_PIX_FMT_BAYER_GRBG16BE,

                /// <summary>
                ///  planar YUV 4:4:0,20bpp, (1 Cr and Cb sample per 1x2 Y samples), little-endian
                /// </summary>
                FormatYuv440P10LittleEndian = AV_PIX_FMT_YUV440P10LE,

                /// <summary>
                ///  planar YUV 4:4:0,20bpp, (1 Cr and Cb sample per 1x2 Y samples), big-endian
                /// </summary>
                FormatYuv440P10BigEndian = AV_PIX_FMT_YUV440P10BE,

                /// <summary>
                ///  planar YUV 4:4:0,24bpp, (1 Cr and Cb sample per 1x2 Y samples), little-endian
                /// </summary>
                FormatYuv440P12LittleEndian = AV_PIX_FMT_YUV440P12LE,

                /// <summary>
                ///  planar YUV 4:4:0,24bpp, (1 Cr and Cb sample per 1x2 Y samples), big-endian
                /// </summary>
                FormatYuv440P12BigEndian = AV_PIX_FMT_YUV440P12BE,

                /// <summary>
                ///  packed AYUV 4:4:4,64bpp (1 Cr and Cb sample per 1x1 Y and A samples), little-endian
                /// </summary>
                FormatAyuv64LittleEndian = AV_PIX_FMT_AYUV64LE,

                /// <summary>
                ///  packed AYUV 4:4:4,64bpp (1 Cr and Cb sample per 1x1 Y and A samples), big-endian
                /// </summary>
                FormatAyuv64BigEndian = AV_PIX_FMT_AYUV64BE,

                /// <summary>
                ///  hardware decoding through Videotoolbox
                /// </summary>
                FormatvideoToolbox = AV_PIX_FMT_VIDEOTOOLBOX,

                /// <summary>
                ///  like NV12, with 10bpp per component, data in the high bits, zeros in the low bits, little-endian
                /// </summary>
                FormatP010LittleEndian = AV_PIX_FMT_P010LE,

                /// <summary>
                ///  like NV12, with 10bpp per component, data in the high bits, zeros in the low bits, big-endian
                /// </summary>
                FormatP010BigEndian = AV_PIX_FMT_P010BE,

                /// <summary>
                ///  planar GBR 4:4:4:4 48bpp, big-endian
                /// </summary>
                FormatGbraP12BigEndian = AV_PIX_FMT_GBRAP12BE,

                /// <summary>
                ///  planar GBR 4:4:4:4 48bpp, little-endian
                /// </summary>
                FormatGbraP12LittleEndian = AV_PIX_FMT_GBRAP12LE,

                /// <summary>
                ///  planar GBR 4:4:4:4 40bpp, big-endian
                /// </summary>
                FormatGbraP10BigEndian = AV_PIX_FMT_GBRAP10BE,

                /// <summary>
                ///  planar GBR 4:4:4:4 40bpp, little-endian
                /// </summary>
                FormatGbraP10LittleEndian = AV_PIX_FMT_GBRAP10LE,

                /// <summary>
                ///  hardware decoding through MediaCodec
                /// </summary>
                FormatMediaCodec = AV_PIX_FMT_MEDIACODEC,
            };
        }
    }
}
