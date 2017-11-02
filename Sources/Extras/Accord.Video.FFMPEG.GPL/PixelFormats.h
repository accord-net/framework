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
            ///   Enumeration of some channel layouts codecs from FFmpeg library, which are available for writing audio files.
            /// </summary>
            ///
            public enum class PixelFormat
            {
                None = AV_PIX_FMT_NONE,
                ///< planar YUV 4:2:0, 12bpp, (1 Cr & Cb sample per 2x2 Y samples)                
                FormatYUV420P = AV_PIX_FMT_YUV420P,
                ///< packed YUV 4:2:2, 16bpp, Y0 Cb Y1 Cr
                FormatYUYV422 = AV_PIX_FMT_YUYV422,
                ///< packed RGB 8:8:8, 24bpp, RGBRGB...
                FormatRGB24 = AV_PIX_FMT_RGB24,
                ///< packed RGB 8:8:8, 24bpp, BGRBGR...
                FormatBGR24 = AV_PIX_FMT_BGR24,
                ///< planar YUV 4:2:2, 16bpp, (1 Cr & Cb sample per 2x1 Y samples)
                FormatYUV422P = AV_PIX_FMT_YUV422P,
                ///< planar YUV 4:4:4, 24bpp, (1 Cr & Cb sample per 1x1 Y samples)
                FormatYUV444P = AV_PIX_FMT_YUV444P,
                ///< planar YUV 4:1:0,  9bpp, (1 Cr & Cb sample per 4x4 Y samples)
                FormatYUV410P = AV_PIX_FMT_YUV410P,
                ///< planar YUV 4:1:1, 12bpp, (1 Cr & Cb sample per 4x1 Y samples)
                FormatYUV411P = AV_PIX_FMT_YUV411P,
                ///<        Y        ,  8bpp
                FormatGRAY8 = AV_PIX_FMT_GRAY8,
                ///<        Y        ,  1bpp, 0 is white, 1 is black, in each byte pixels are ordered from the msb to the lsb
                FormatMONOWHITE = AV_PIX_FMT_MONOWHITE,
                ///<        Y        ,  1bpp, 0 is black, 1 is white, in each byte pixels are ordered from the msb to the lsb
                FormatMONOBLACK = AV_PIX_FMT_MONOBLACK,
                ///< 8 bits with AV_PIX_FMT_RGB32 palette
                FormatAL8 = AV_PIX_FMT_PAL8,
                ///< planar YUV 4:2:0, 12bpp, full scale (JPEG), deprecated in favor of AV_PIX_FMT_YUV420P and setting color_range
                FormatYUVJ420P = AV_PIX_FMT_YUVJ420P,
                ///< planar YUV 4:2:2, 16bpp, full scale (JPEG), deprecated in favor of AV_PIX_FMT_YUV422P and setting color_range
                FormatYUVJ422P = AV_PIX_FMT_YUVJ422P,
                ///< planar YUV 4:4:4, 24bpp, full scale (JPEG), deprecated in favor of AV_PIX_FMT_YUV444P and setting color_range
                FormatYUVJ444P = AV_PIX_FMT_YUVJ444P,
                ///< XVideo Motion Acceleration via common packet passing
                FormatXVMC_MPEG2_MC = AV_PIX_FMT_XVMC_MPEG2_MC,
                FormatXVMC_MPEG2_IDCT = AV_PIX_FMT_XVMC_MPEG2_IDCT,
                FormatXVMC = AV_PIX_FMT_XVMC,
                ///< packed YUV 4:2:2, 16bpp, Cb Y0 Cr Y1
                FormatUYVY422 = AV_PIX_FMT_UYVY422,
                ///< packed YUV 4:1:1, 12bpp, Cb Y0 Y1 Cr Y2 Y3
                FormatUYYVYY411 = AV_PIX_FMT_UYYVYY411,
                ///< packed RGB 3:3:2,  8bpp, (msb)2B 3G 3R(lsb)
                FormatBGR8 = AV_PIX_FMT_BGR8,
                ///< packed RGB 1:2:1 bitstream,  4bpp, (msb)1B 2G 1R(lsb), a byte contains two pixels, the first pixel in the byte is the one composed by the 4 msb bits
                FormatBGR4 = AV_PIX_FMT_BGR4,
                ///< packed RGB 1:2:1,  8bpp, (msb)1B 2G 1R(lsb)
                FormatBGR4_BYTE = AV_PIX_FMT_BGR4_BYTE,
                ///< packed RGB 3:3:2,  8bpp, (msb)2R 3G 3B(lsb)
                FormatRGB8 = AV_PIX_FMT_RGB8,
                ///< packed RGB 1:2:1 bitstream,  4bpp, (msb)1R 2G 1B(lsb), a byte contains two pixels, the first pixel in the byte is the one composed by the 4 msb bits
                FormatRGB4 = AV_PIX_FMT_RGB4,
                ///< packed RGB 1:2:1,  8bpp, (msb)1R 2G 1B(lsb)
                FormatRGB4_BYTE = AV_PIX_FMT_RGB4_BYTE,
                ///< planar YUV 4:2:0, 12bpp, 1 plane for Y and 1 plane for the UV components, which are interleaved (first byte U and the following byte V)
                FormatFMT_NV12 = AV_PIX_FMT_NV12,
                ///< as above, but U and V bytes are swapped
                FormatFMT_NV21 = AV_PIX_FMT_NV21,
                ///< packed ARGB 8:8:8:8, 32bpp, ARGBARGB...
                FormatFMT_ARGB = AV_PIX_FMT_ARGB,
                ///< packed RGBA 8:8:8:8, 32bpp, RGBARGBA...
                FormatFMT_RGBA = AV_PIX_FMT_RGBA,
                ///< packed ABGR 8:8:8:8, 32bpp, ABGRABGR...
                FormatABGR = AV_PIX_FMT_ABGR,
                ///< packed BGRA 8:8:8:8, 32bpp, BGRABGRA...
                FormatBGRA = AV_PIX_FMT_BGRA,
                ///<        Y        , 16bpp, big-endian
                FormatGRAY16BE = AV_PIX_FMT_GRAY16BE,
                ///<        Y        , 16bpp, little-endian
                FormatGRAY16LE = AV_PIX_FMT_GRAY16LE,
                ///< planar YUV 4:4:0 (1 Cr & Cb sample per 1x2 Y samples)
                FormatYUV440P = AV_PIX_FMT_YUV440P,
                ///< planar YUV 4:4:0 full scale (JPEG), deprecated in favor of AV_PIX_FMT_YUV440P and setting color_range
                FormatYUVJ440P = AV_PIX_FMT_YUVJ440P,
                ///< planar YUV 4:2:0, 20bpp, (1 Cr & Cb sample per 2x2 Y & A samples)
                FormatYUVA420P = AV_PIX_FMT_YUVA420P,
                ///< H.264 HW decoding with VDPAU, data[0] contains a vdpau_render_state struct which contains the bitstream of the slices as well as various fields extracted from headers
                FormatVDPAU_H264 = AV_PIX_FMT_VDPAU_H264,
                ///< MPEG-1 HW decoding with VDPAU, data[0] contains a vdpau_render_state struct which contains the bitstream of the slices as well as various fields extracted from headers
                FormatVDPAU_MPEG1 = AV_PIX_FMT_VDPAU_MPEG1,
                ///< MPEG-2 HW decoding with VDPAU, data[0] contains a vdpau_render_state struct which contains the bitstream of the slices as well as various fields extracted from headers
                FormatVDPAU_MPEG2 = AV_PIX_FMT_VDPAU_MPEG2,
                ///< WMV3 HW decoding with VDPAU, data[0] contains a vdpau_render_state struct which contains the bitstream of the slices as well as various fields extracted from headers
                FormatVDPAU_WMV3 = AV_PIX_FMT_VDPAU_WMV3,
                ///< VC-1 HW decoding with VDPAU, data[0] contains a vdpau_render_state struct which contains the bitstream of the slices as well as various fields extracted from headers
                FormatVDPAU_VC1 = AV_PIX_FMT_VDPAU_VC1,
                ///< packed RGB 16:16:16, 48bpp, 16R, 16G, 16B, the 2-byte value for each R/G/B component is stored as big-endian
                FormatRGB48BE = AV_PIX_FMT_RGB48BE,
                ///< packed RGB 16:16:16, 48bpp, 16R, 16G, 16B, the 2-byte value for each R/G/B component is stored as little-endian
                FormatRGB48LE = AV_PIX_FMT_RGB48LE,
                ///< packed RGB 5:6:5, 16bpp, (msb)   5R 6G 5B(lsb), big-endian
                FormatRGB565BE = AV_PIX_FMT_RGB565BE,
                ///< packed RGB 5:6:5, 16bpp, (msb)   5R 6G 5B(lsb), little-endian
                FormatRGB565LE = AV_PIX_FMT_RGB565LE,
                ///< packed RGB 5:5:5, 16bpp, (msb)1X 5R 5G 5B(lsb), big-endian   , X=unused/undefined
                FormatRGB555BE = AV_PIX_FMT_RGB555BE,
                ///< packed RGB 5:5:5, 16bpp, (msb)1X 5R 5G 5B(lsb), little-endian, X=unused/undefined
                FormatRGB555LE = AV_PIX_FMT_RGB555LE,
                ///< packed BGR 5:6:5, 16bpp, (msb)   5B 6G 5R(lsb), big-endian
                FormatBGR565BE = AV_PIX_FMT_BGR565BE,
                ///< packed BGR 5:6:5, 16bpp, (msb)   5B 6G 5R(lsb), little-endian
                FormatBGR565LE = AV_PIX_FMT_BGR565LE,
                ///< packed BGR 5:5:5, 16bpp, (msb)1X 5B 5G 5R(lsb), big-endian   , X=unused/undefined
                FormatBGR555BE = AV_PIX_FMT_BGR555BE,
                ///< packed BGR 5:5:5, 16bpp, (msb)1X 5B 5G 5R(lsb), little-endian, X=unused/undefined
                FormatBGR555LE = AV_PIX_FMT_BGR555LE,
                ///< HW acceleration through VA API at motion compensation entry-point, Picture.data[3] contains a vaapi_render_state struct which contains macroblocks as well as various fields extracted from headers
                FormatVAAPI_MOCO = AV_PIX_FMT_VAAPI_MOCO,
                ///< HW acceleration through VA API at IDCT entry-point, Picture.data[3] contains a vaapi_render_state struct which contains fields extracted from headers
                FormatVAAPI_IDCT = AV_PIX_FMT_VAAPI_IDCT,
                ///< HW decoding through VA API, Picture.data[3] contains a VASurfaceID
                FormatVAAPI_VLD,
                FormatVAAPI = AV_PIX_FMT_VAAPI_VLD,
                ///< planar YUV 4:2:0, 24bpp, (1 Cr & Cb sample per 2x2 Y samples), little-endian
                FormatYUV420P16LE = AV_PIX_FMT_YUV420P16LE,
                ///< planar YUV 4:2:0, 24bpp, (1 Cr & Cb sample per 2x2 Y samples), big-endian
                FormatYUV420P16BE = AV_PIX_FMT_YUV420P16BE,
                ///< planar YUV 4:2:2, 32bpp, (1 Cr & Cb sample per 2x1 Y samples), little-endian
                FormatYUV422P16LE = AV_PIX_FMT_YUV422P16LE,
                ///< planar YUV 4:2:2, 32bpp, (1 Cr & Cb sample per 2x1 Y samples), big-endian
                FormatYUV422P16BE = AV_PIX_FMT_YUV422P16BE,
                ///< planar YUV 4:4:4, 48bpp, (1 Cr & Cb sample per 1x1 Y samples), little-endian
                FormatYUV444P16LE = AV_PIX_FMT_YUV444P16LE,
                ///< planar YUV 4:4:4, 48bpp, (1 Cr & Cb sample per 1x1 Y samples), big-endian
                FormatYUV444P16BE = AV_PIX_FMT_YUV444P16BE,
                ///< MPEG-4 HW decoding with VDPAU, data[0] contains a vdpau_render_state struct which contains the bitstream of the slices as well as various fields extracted from headers
                FormatVDPAU_MPEG4 = AV_PIX_FMT_VDPAU_MPEG4,
                ///< HW decoding through DXVA2, Picture.data[3] contains a LPDIRECT3DSURFACE9 pointer
                FormatDXVA2_VLD = AV_PIX_FMT_DXVA2_VLD,
                ///< packed RGB 4:4:4, 16bpp, (msb)4X 4R 4G 4B(lsb), little-endian, X=unused/undefined
                FormatRGB444LE = AV_PIX_FMT_RGB444LE,
                ///< packed RGB 4:4:4, 16bpp, (msb)4X 4R 4G 4B(lsb), big-endian,    X=unused/undefined
                FormatRGB444BE = AV_PIX_FMT_RGB444BE,
                ///< packed BGR 4:4:4, 16bpp, (msb)4X 4B 4G 4R(lsb), little-endian, X=unused/undefined
                FormatBGR444LE = AV_PIX_FMT_BGR444LE,
                ///< packed BGR 4:4:4, 16bpp, (msb)4X 4B 4G 4R(lsb), big-endian,    X=unused/undefined
                FormatBGR444BE = AV_PIX_FMT_BGR444BE,
                ///< 8 bits gray, 8 bits alpha
                FormatYA8 = AV_PIX_FMT_YA8,
                ///< alias for AV_PIX_FMT_YA8
                FormatY400A = AV_PIX_FMT_YA8,
                ///< alias for AV_PIX_FMT_YA8
                FormatGRAY8A = AV_PIX_FMT_YA8,
                ///< packed RGB 16:16:16, 48bpp, 16B, 16G, 16R, the 2-byte value for each R/G/B component is stored as big-endian
                FormatBGR48BE = AV_PIX_FMT_BGR48BE,
                ///< packed RGB 16:16:16, 48bpp, 16B, 16G, 16R, the 2-byte value for each R/G/B component is stored as little-endian
                FormatBGR48LE = AV_PIX_FMT_BGR48LE,

                /**
                * The following 12 formats have the disadvantage of needing 1 format for each bit depth.
                * Notice that each 9/10 bits sample is stored in 16 bits with extra padding.
                * If you want to support multiple bit depths, then using AV_PIX_FMT_YUV420P16* with the bpp stored separately is better.
                */
                ///< planar YUV 4:2:0, 13.5bpp, (1 Cr & Cb sample per 2x2 Y samples), big-endian
                FormatYUV420P9BE = AV_PIX_FMT_YUV420P9BE,
                ///< planar YUV 4:2:0, 13.5bpp, (1 Cr & Cb sample per 2x2 Y samples), little-endian
                FormatYUV420P9LE = AV_PIX_FMT_YUV420P9LE,
                ///< planar YUV 4:2:0, 15bpp, (1 Cr & Cb sample per 2x2 Y samples), big-endian
                FormatYUV420P10BE = AV_PIX_FMT_YUV420P10BE,
                ///< planar YUV 4:2:0, 15bpp, (1 Cr & Cb sample per 2x2 Y samples), little-endian
                FormatYUV420P10LE = AV_PIX_FMT_YUV420P10LE,
                ///< planar YUV 4:2:2, 20bpp, (1 Cr & Cb sample per 2x1 Y samples), big-endian
                FormatYUV422P10BE = AV_PIX_FMT_YUV422P10BE,
                ///< planar YUV 4:2:2, 20bpp, (1 Cr & Cb sample per 2x1 Y samples), little-endian
                FormatYUV422P10LE = AV_PIX_FMT_YUV422P10LE,
                ///< planar YUV 4:4:4, 27bpp, (1 Cr & Cb sample per 1x1 Y samples), big-endian
                FormatYUV444P9BE = AV_PIX_FMT_YUV444P9BE,
                ///< planar YUV 4:4:4, 27bpp, (1 Cr & Cb sample per 1x1 Y samples), little-endian
                FormatYUV444P9LE = AV_PIX_FMT_YUV444P9LE,
                ///< planar YUV 4:4:4, 30bpp, (1 Cr & Cb sample per 1x1 Y samples), big-endian
                FormatYUV444P10BE = AV_PIX_FMT_YUV444P10BE,
                ///< planar YUV 4:4:4, 30bpp, (1 Cr & Cb sample per 1x1 Y samples), little-endian
                FormatYUV444P10LE = AV_PIX_FMT_YUV444P10LE,
                ///< planar YUV 4:2:2, 18bpp, (1 Cr & Cb sample per 2x1 Y samples), big-endian
                FormatYUV422P9BE = AV_PIX_FMT_YUV422P9BE,
                ///< planar YUV 4:2:2, 18bpp, (1 Cr & Cb sample per 2x1 Y samples), little-endian
                FormatYUV422P9LE,
                ///< hardware decoding through VDA
                FormatVDA_VLD,
                ///< planar GBR 4:4:4 24bpp
                FormatGBRP,
                /// alias for #AV_PIX_FMT_GBRP
                FormatGBR24P = AV_PIX_FMT_GBRP,
                ///< planar GBR 4:4:4 27bpp, big-endian
                FormatGBRP9BE = AV_PIX_FMT_GBRP9BE,
                ///< planar GBR 4:4:4 27bpp, little-endian
                FormatGBRP9LE = AV_PIX_FMT_GBRP9LE,
                ///< planar GBR 4:4:4 30bpp, big-endian
                FormatGBRP10BE = AV_PIX_FMT_GBRP10BE,
                ///< planar GBR 4:4:4 30bpp, little-endian
                FormatGBRP10LE = AV_PIX_FMT_GBRP10LE,
                ///< planar GBR 4:4:4 48bpp, big-endian
                FormatGBRP16BE = AV_PIX_FMT_GBRP16BE,
                ///< planar GBR 4:4:4 48bpp, little-endian
                FormatGBRP16LE = AV_PIX_FMT_GBRP16LE,
                ///< planar YUV 4:2:2 24bpp, (1 Cr & Cb sample per 2x1 Y & A samples)
                FormatYUVA422P = AV_PIX_FMT_YUVA422P,
                ///< planar YUV 4:4:4 32bpp, (1 Cr & Cb sample per 1x1 Y & A samples)
                FormatYUVA444P = AV_PIX_FMT_YUVA444P,
                ///< planar YUV 4:2:0 22.5bpp, (1 Cr & Cb sample per 2x2 Y & A samples), big-endian
                FormatYUVA420P9BE = AV_PIX_FMT_YUVA420P9BE,
                ///< planar YUV 4:2:0 22.5bpp, (1 Cr & Cb sample per 2x2 Y & A samples), little-endian
                FormatYUVA420P9LE = AV_PIX_FMT_YUVA420P9LE,
                ///< planar YUV 4:2:2 27bpp, (1 Cr & Cb sample per 2x1 Y & A samples), big-endian
                FormatYUVA422P9BE = AV_PIX_FMT_YUVA422P9BE,
                ///< planar YUV 4:2:2 27bpp, (1 Cr & Cb sample per 2x1 Y & A samples), little-endian
                FormatYUVA422P9LE = AV_PIX_FMT_YUVA422P9LE,
                ///< planar YUV 4:4:4 36bpp, (1 Cr & Cb sample per 1x1 Y & A samples), big-endian
                FormatYUVA444P9BE = AV_PIX_FMT_YUVA444P9BE,
                ///< planar YUV 4:4:4 36bpp, (1 Cr & Cb sample per 1x1 Y & A samples), little-endian
                FormatYUVA444P9LE = AV_PIX_FMT_YUVA444P9LE,
                ///< planar YUV 4:2:0 25bpp, (1 Cr & Cb sample per 2x2 Y & A samples, big-endian)
                FormatYUVA420P10BE = AV_PIX_FMT_YUVA420P10BE,
                ///< planar YUV 4:2:0 25bpp, (1 Cr & Cb sample per 2x2 Y & A samples, little-endian)
                FormatYUVA420P10LE = AV_PIX_FMT_YUVA420P10LE,
                ///< planar YUV 4:2:2 30bpp, (1 Cr & Cb sample per 2x1 Y & A samples, big-endian)
                FormatYUVA422P10BE = AV_PIX_FMT_YUVA422P10BE,
                ///< planar YUV 4:2:2 30bpp, (1 Cr & Cb sample per 2x1 Y & A samples, little-endian)
                FormatYUVA422P10LE = AV_PIX_FMT_YUVA422P10LE,
                ///< planar YUV 4:4:4 40bpp, (1 Cr & Cb sample per 1x1 Y & A samples, big-endian)
                FormatYUVA444P10BE = AV_PIX_FMT_YUVA444P10BE,
                ///< planar YUV 4:4:4 40bpp, (1 Cr & Cb sample per 1x1 Y & A samples, little-endian)
                FormatYUVA444P10LE = AV_PIX_FMT_YUVA444P10LE,
                ///< planar YUV 4:2:0 40bpp, (1 Cr & Cb sample per 2x2 Y & A samples, big-endian)
                FormatYUVA420P16BE = AV_PIX_FMT_YUVA420P16BE,
                ///< planar YUV 4:2:0 40bpp, (1 Cr & Cb sample per 2x2 Y & A samples, little-endian)
                FormatYUVA420P16LE = AV_PIX_FMT_YUVA420P16LE,
                ///< planar YUV 4:2:2 48bpp, (1 Cr & Cb sample per 2x1 Y & A samples, big-endian)
                FormatYUVA422P16BE = AV_PIX_FMT_YUVA422P16BE,
                ///< planar YUV 4:2:2 48bpp, (1 Cr & Cb sample per 2x1 Y & A samples, little-endian)
                FormatYUVA422P16LE = AV_PIX_FMT_YUVA422P16LE,
                ///< planar YUV 4:4:4 64bpp, (1 Cr & Cb sample per 1x1 Y & A samples, big-endian)
                FormatYUVA444P16BE = AV_PIX_FMT_YUVA444P16BE,
                ///< planar YUV 4:4:4 64bpp, (1 Cr & Cb sample per 1x1 Y & A samples, little-endian)
                FormatYUVA444P16LE = AV_PIX_FMT_YUVA444P16LE,
                ///< HW acceleration through VDPAU, Picture.data[3] contains a VdpVideoSurface
                FormatVDPAU = AV_PIX_FMT_VDPAU,
                ///< packed XYZ 4:4:4, 36 bpp, (msb) 12X, 12Y, 12Z (lsb), the 2-byte value for each X/Y/Z is stored as little-endian, the 4 lower bits are set to 0
                FormatXYZ12LE = AV_PIX_FMT_XYZ12LE,
                ///< packed XYZ 4:4:4, 36 bpp, (msb) 12X, 12Y, 12Z (lsb), the 2-byte value for each X/Y/Z is stored as big-endian, the 4 lower bits are set to 0
                FormatXYZ12BE = AV_PIX_FMT_XYZ12BE,
                ///< interleaved chroma YUV 4:2:2, 16bpp, (1 Cr & Cb sample per 2x1 Y samples)
                FormatNV16 = AV_PIX_FMT_NV16,
                ///< interleaved chroma YUV 4:2:2, 20bpp, (1 Cr & Cb sample per 2x1 Y samples), little-endian
                FormatNV20LE = AV_PIX_FMT_NV20LE,
                ///< interleaved chroma YUV 4:2:2, 20bpp, (1 Cr & Cb sample per 2x1 Y samples), big-endian
                FormatNV20BE = AV_PIX_FMT_NV20BE,
                ///< packed RGBA 16:16:16:16, 64bpp, 16R, 16G, 16B, 16A, the 2-byte value for each R/G/B/A component is stored as big-endian
                FormatRGBA64BE = AV_PIX_FMT_RGBA64BE,
                ///< packed RGBA 16:16:16:16, 64bpp, 16R, 16G, 16B, 16A, the 2-byte value for each R/G/B/A component is stored as little-endian
                FormatRGBA64LE = AV_PIX_FMT_RGBA64LE,
                ///< packed RGBA 16:16:16:16, 64bpp, 16B, 16G, 16R, 16A, the 2-byte value for each R/G/B/A component is stored as big-endian
                FormatBGRA64BE = AV_PIX_FMT_BGRA64BE,
                ///< packed RGBA 16:16:16:16, 64bpp, 16B, 16G, 16R, 16A, the 2-byte value for each R/G/B/A component is stored as little-endian
                FormatBGRA64LE = AV_PIX_FMT_BGRA64LE,
                ///< packed YUV 4:2:2, 16bpp, Y0 Cr Y1 Cb
                FormatYVYU422 = AV_PIX_FMT_YVYU422,
                ///< HW acceleration through VDA, data[3] contains a CVPixelBufferRef
                FormatVDA = AV_PIX_FMT_VDA,
                ///< 16 bits gray, 16 bits alpha (big-endian)
                FormatYA16BE = AV_PIX_FMT_YA16BE,
                ///< 16 bits gray, 16 bits alpha (little-endian)
                FormatYA16LE = AV_PIX_FMT_YA16LE,
                ///< planar GBRA 4:4:4:4 32bpp
                FormatGBRAP = AV_PIX_FMT_GBRAP,
                ///< planar GBRA 4:4:4:4 64bpp, big-endian
                FormatGBRAP16BE = AV_PIX_FMT_GBRAP16BE,
                ///< planar GBRA 4:4:4:4 64bpp, little-endian
                FormatGBRAP16LE = AV_PIX_FMT_GBRAP16LE,

                /**
                *  HW acceleration through QSV, data[3] contains a pointer to the
                *  mfxFrameSurface1 structure.
                */
                FormatQSV = AV_PIX_FMT_QSV,

                /**
                * HW acceleration though MMAL, data[3] contains a pointer to the
                * MMAL_BUFFER_HEADER_T structure.
                */
                FormatMMAL = AV_PIX_FMT_MMAL,

                ///< HW decoding through Direct3D11, Picture.data[3] contains a ID3D11VideoDecoderOutputView pointer
                FormatD3D11VA_VLD,

                /**
                * HW acceleration through CUDA. data[i] contain CUdeviceptr pointers
                * exactly as for system memory frames.
                */
                FormatCUDA = AV_PIX_FMT_CUDA,

                ///< packed RGB 8:8:8, 32bpp, XRGBXRGB...   X=unused/undefined
                Format0RGB = 0x123 + 4,
                ///< packed RGB 8:8:8, 32bpp, RGBXRGBX...   X=unused/undefined
                FormatRGB0 = AV_PIX_FMT_RGB0,
                ///< packed BGR 8:8:8, 32bpp, XBGRXBGR...   X=unused/undefined
                Format0BGR = AV_PIX_FMT_0BGR,
                ///< packed BGR 8:8:8, 32bpp, BGRXBGRX...   X=unused/undefined
                FormatBGR0 = AV_PIX_FMT_BGR0,
                ///< planar YUV 4:2:0,18bpp, (1 Cr & Cb sample per 2x2 Y samples), big-endian
                FormatYUV420P12BE = AV_PIX_FMT_YUV420P12BE,
                ///< planar YUV 4:2:0,18bpp, (1 Cr & Cb sample per 2x2 Y samples), little-endian
                FormatYUV420P12LE = AV_PIX_FMT_YUV420P12LE,
                ///< planar YUV 4:2:0,21bpp, (1 Cr & Cb sample per 2x2 Y samples), big-endian
                FormatYUV420P14BE = AV_PIX_FMT_YUV420P14BE,
                ///< planar YUV 4:2:0,21bpp, (1 Cr & Cb sample per 2x2 Y samples), little-endian
                FormatYUV420P14LE = AV_PIX_FMT_YUV420P14LE,
                ///< planar YUV 4:2:2,24bpp, (1 Cr & Cb sample per 2x1 Y samples), big-endian
                FormatYUV422P12BE = AV_PIX_FMT_YUV422P12BE,
                ///< planar YUV 4:2:2,24bpp, (1 Cr & Cb sample per 2x1 Y samples), little-endian
                FormatYUV422P12LE = AV_PIX_FMT_YUV422P12LE,
                ///< planar YUV 4:2:2,28bpp, (1 Cr & Cb sample per 2x1 Y samples), big-endian
                FormatYUV422P14BE = AV_PIX_FMT_YUV422P14BE,
                ///< planar YUV 4:2:2,28bpp, (1 Cr & Cb sample per 2x1 Y samples), little-endian
                FormatYUV422P14LE = AV_PIX_FMT_YUV422P14LE,
                ///< planar YUV 4:4:4,36bpp, (1 Cr & Cb sample per 1x1 Y samples), big-endian
                FormatYUV444P12BE = AV_PIX_FMT_YUV444P12BE,
                ///< planar YUV 4:4:4,36bpp, (1 Cr & Cb sample per 1x1 Y samples), little-endian
                FormatYUV444P12LE = AV_PIX_FMT_YUV444P12LE,
                ///< planar YUV 4:4:4,42bpp, (1 Cr & Cb sample per 1x1 Y samples), big-endian
                FormatYUV444P14BE = AV_PIX_FMT_YUV444P14BE,
                ///< planar YUV 4:4:4,42bpp, (1 Cr & Cb sample per 1x1 Y samples), little-endian
                FormatYUV444P14LE = AV_PIX_FMT_YUV444P14LE,
                ///< planar GBR 4:4:4 36bpp, big-endian
                FormatGBRP12BE = AV_PIX_FMT_GBRP12BE,
                ///< planar GBR 4:4:4 36bpp, little-endian
                FormatGBRP12LE = AV_PIX_FMT_GBRP12LE,
                ///< planar GBR 4:4:4 42bpp, big-endian
                FormatGBRP14BE = AV_PIX_FMT_GBRP14BE,
                ///< planar GBR 4:4:4 42bpp, little-endian
                FormatGBRP14LE = AV_PIX_FMT_GBRP14LE,
                ///< planar YUV 4:1:1, 12bpp, (1 Cr & Cb sample per 4x1 Y samples) full scale (JPEG), deprecated in favor of AV_PIX_FMT_YUV411P and setting color_range
                FormatYUVJ411P = AV_PIX_FMT_YUVJ411P,
                ///< bayer, BGBG..(odd line), GRGR..(even line), 8-bit samples */
                FormatBAYER_BGGR8 = AV_PIX_FMT_BAYER_BGGR8,
                ///< bayer, RGRG..(odd line), GBGB..(even line), 8-bit samples */
                FormatBAYER_RGGB8 = AV_PIX_FMT_BAYER_RGGB8,
                ///< bayer, GBGB..(odd line), RGRG..(even line), 8-bit samples */
                FormatBAYER_GBRG8 = AV_PIX_FMT_BAYER_GBRG8,
                ///< bayer, GRGR..(odd line), BGBG..(even line), 8-bit samples */
                FormatBAYER_GRBG8 = AV_PIX_FMT_BAYER_GRBG8,
                ///< bayer, BGBG..(odd line), GRGR..(even line), 16-bit samples, little-endian */
                FormatBAYER_BGGR16LE = AV_PIX_FMT_BAYER_BGGR16LE,
                ///< bayer, BGBG..(odd line), GRGR..(even line), 16-bit samples, big-endian */
                FormatBAYER_BGGR16BE = AV_PIX_FMT_BAYER_BGGR16BE,
                ///< bayer, RGRG..(odd line), GBGB..(even line), 16-bit samples, little-endian */
                FormatBAYER_RGGB16LE = AV_PIX_FMT_BAYER_RGGB16LE,
                ///< bayer, RGRG..(odd line), GBGB..(even line), 16-bit samples, big-endian */
                FormatBAYER_RGGB16BE = AV_PIX_FMT_BAYER_RGGB16BE,
                ///< bayer, GBGB..(odd line), RGRG..(even line), 16-bit samples, little-endian */
                FormatBAYER_GBRG16LE = AV_PIX_FMT_BAYER_GBRG16LE,
                ///< bayer, GBGB..(odd line), RGRG..(even line), 16-bit samples, big-endian */
                FormatBAYER_GBRG16BE = AV_PIX_FMT_BAYER_GBRG16BE,
                ///< bayer, GRGR..(odd line), BGBG..(even line), 16-bit samples, little-endian */
                FormatBAYER_GRBG16LE = AV_PIX_FMT_BAYER_GRBG16LE,
                ///< bayer, GRGR..(odd line), BGBG..(even line), 16-bit samples, big-endian */
                FormatBAYER_GRBG16BEBAYER_GRBG16BE,
                ///< planar YUV 4:4:0,20bpp, (1 Cr & Cb sample per 1x2 Y samples), little-endian
                FormatYUV440P10LE = AV_PIX_FMT_YUV440P10LE,
                ///< planar YUV 4:4:0,20bpp, (1 Cr & Cb sample per 1x2 Y samples), big-endian
                FormatYUV440P10BE = AV_PIX_FMT_YUV440P10BE,
                ///< planar YUV 4:4:0,24bpp, (1 Cr & Cb sample per 1x2 Y samples), little-endian
                FormatYUV440P12LE = AV_PIX_FMT_YUV440P12LE,
                ///< planar YUV 4:4:0,24bpp, (1 Cr & Cb sample per 1x2 Y samples), big-endian
                FormatYUV440P12BE = AV_PIX_FMT_YUV440P12BE,
                ///< packed AYUV 4:4:4,64bpp (1 Cr & Cb sample per 1x1 Y & A samples), little-endian
                FormatAYUV64LE = AV_PIX_FMT_AYUV64LE,
                ///< packed AYUV 4:4:4,64bpp (1 Cr & Cb sample per 1x1 Y & A samples), big-endian
                FormatAYUV64BE = AV_PIX_FMT_AYUV64BE,
                ///< hardware decoding through Videotoolbox
                FormatVIDEOTOOLBOX = AV_PIX_FMT_VIDEOTOOLBOX,
                ///< like NV12, with 10bpp per component, data in the high bits, zeros in the low bits, little-endian
                FormatP010LE = AV_PIX_FMT_P010LE,
                ///< like NV12, with 10bpp per component, data in the high bits, zeros in the low bits, big-endian
                FormatP010BE = AV_PIX_FMT_P010BE,
                ///< planar GBR 4:4:4:4 48bpp, big-endian
                FormatGBRAP12BE = AV_PIX_FMT_GBRAP12BE,
                ///< planar GBR 4:4:4:4 48bpp, little-endian
                FormatGBRAP12LE = AV_PIX_FMT_GBRAP12LE,
                ///< planar GBR 4:4:4:4 40bpp, big-endian
                FormatGBRAP10BE = AV_PIX_FMT_GBRAP10BE,
                ///< planar GBR 4:4:4:4 40bpp, little-endian
                FormatGBRAP10LE = AV_PIX_FMT_GBRAP10LE,
                ///< hardware decoding through MediaCodec
                FormatMEDIACODEC = AV_PIX_FMT_MEDIACODEC,
                ///< number of pixel formats, DO NOT USE THIS if you want to link with shared libav* because the number of formats might differ between versions
                FormatNB = AV_PIX_FMT_NB
            };
        }
    }
}
