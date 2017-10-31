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

extern "C"
{
#include "libavcodec\avcodec.h"
}


namespace Accord {
    namespace Video {
        namespace FFMPEG
        {
            /// <summary>
            ///   Enumeration of some video codecs from FFmpeg library, which are available for writing video files.
            /// </summary>
            ///
            public enum class VideoCodec
            {
                /// <summary>
                /// No video.
                /// </summary>
                ///
                None = AV_CODEC_ID_NONE,

                Default = AV_CODEC_ID_PROBE,

                /// <summary>
                ///   MPEG-1.
                /// </summary>
                MPEG1 = AV_CODEC_ID_MPEG1VIDEO,

                /// <summary>
                ///   MPEG-2. Preferred ID for MPEG-1/2 video decoding.
                /// </summary>
                MPEG2 = AV_CODEC_ID_MPEG2VIDEO,

                //MPEG2XvMC = AV_CODEC_ID_MPEG2VIDEO_XVMC,

                /// <summary>
                ///   H.261
                /// </summary>
                H261 = AV_CODEC_ID_H261,

                /// <summary>
                ///   H.263
                /// </summary>
                H263 = AV_CODEC_ID_H263,

                /// <summary>
                ///   RealVideo 1.0, based on H.263 (included with RealPlayer 5).
                /// </summary>
                RV10 = AV_CODEC_ID_RV10,

                /// <summary>
                ///    RealVideo G2 and RealVideo G2+SVT, also based on h.263 (included with RealPlayer 6).
                /// </summary>
                RV20 = AV_CODEC_ID_RV20,
                
                /// <summary>
                /// Motion JPEG (M-JPEG or MJPEG)
                /// </summary>
                MJPEG = AV_CODEC_ID_MJPEG,


                MJPEGB = AV_CODEC_ID_MJPEGB,
                LJPEG = AV_CODEC_ID_LJPEG,
                SP5X = AV_CODEC_ID_SP5X,
                JPEGLS = AV_CODEC_ID_JPEGLS,
                MPEG4 = AV_CODEC_ID_MPEG4,
                Raw = AV_CODEC_ID_RAWVIDEO,
                MSMPEG4V1 = AV_CODEC_ID_MSMPEG4V1,
                MSMPEG4V2 = AV_CODEC_ID_MSMPEG4V2,
                MSMPEG4v3 = AV_CODEC_ID_MSMPEG4V3,                

                /// <summary>
                /// Windows Media Video 7.
                /// </summary>
                WMV1 = AV_CODEC_ID_WMV1,

                /// <summary>
                /// Windows Media Video 8.
                /// </summary>
                WMV2 = AV_CODEC_ID_WMV2,

                /// <summary>
                /// H.263+ (H.263-1998)
                /// </summary>
                H263P = AV_CODEC_ID_H263P,
                
                /// <summary>
                /// H.263 Intel
                /// </summary>
                H263I = AV_CODEC_ID_H263I,

                
                /// <summary>
                /// Sorenson Spark (FLV1)
                /// </summary>
                FLV1 = AV_CODEC_ID_FLV1,

                /// <summary>
                /// Sorenson Video v1
                /// </summary>
                SVQ1 = AV_CODEC_ID_SVQ1,

                /// <summary>
                /// Sorenson Video v3
                /// </summary>
                SVQ3 = AV_CODEC_ID_SVQ3,
                
                /// <summary>
                /// DV.
                /// </summary>
                DV = AV_CODEC_ID_DVVIDEO,
                
                /// <summary>
                /// Huffyuv lossless codec.
                /// </summary>
                HUFFYUV = AV_CODEC_ID_HUFFYUV,
                CYUV = AV_CODEC_ID_CYUV,
                H264 = AV_CODEC_ID_H264,
                INDEO3 = AV_CODEC_ID_INDEO3,         

                /// <summary>
                /// FFmpeg VP3.
                /// </summary>
                VP3 = AV_CODEC_ID_VP3,
                
                /// <summary>
                /// FFmpeg Theora
                /// </summary>
                Theora = AV_CODEC_ID_THEORA,
                ASV1 = AV_CODEC_ID_ASV1,
                ASV2 = AV_CODEC_ID_ASV2,
                FFV1 = AV_CODEC_ID_FFV1,
                FFMpeg4XM = AV_CODEC_ID_4XM,
                VCR1 = AV_CODEC_ID_VCR1,
                CLJR = AV_CODEC_ID_CLJR,
                MDEC = AV_CODEC_ID_MDEC,
                ROQ = AV_CODEC_ID_ROQ,
                Interplay = AV_CODEC_ID_INTERPLAY_VIDEO,
                XAN_WC3 = AV_CODEC_ID_XAN_WC3,
                XAN_WC4 = AV_CODEC_ID_XAN_WC4,
                RPZA = AV_CODEC_ID_RPZA,
                Cinepak = AV_CODEC_ID_CINEPAK,
                WS_VQA = AV_CODEC_ID_WS_VQA,
                MSRLE = AV_CODEC_ID_MSRLE,
                MSVIDEO1 = AV_CODEC_ID_MSVIDEO1,
                IDCIN = AV_CODEC_ID_IDCIN,
                PlanarRGB8BPS = AV_CODEC_ID_8BPS,
                SMC = AV_CODEC_ID_SMC,
                FLIC = AV_CODEC_ID_FLIC,
                TRUEMOTION1 = AV_CODEC_ID_TRUEMOTION1,
                VMDVIDEO = AV_CODEC_ID_VMDVIDEO,
                MSZH = AV_CODEC_ID_MSZH,
                ZLIB = AV_CODEC_ID_ZLIB,
                QTRLE = AV_CODEC_ID_QTRLE,
                TSCC = AV_CODEC_ID_TSCC,
                ULTI = AV_CODEC_ID_ULTI,
                QDRAW = AV_CODEC_ID_QDRAW,
                VIXL = AV_CODEC_ID_VIXL,
                QPEG = AV_CODEC_ID_QPEG,
                PNG = AV_CODEC_ID_PNG,
                PPM = AV_CODEC_ID_PPM,
                PBM = AV_CODEC_ID_PBM,
                PGM = AV_CODEC_ID_PGM,
                PGMYUV = AV_CODEC_ID_PGMYUV,
                PAM = AV_CODEC_ID_PAM,
                FFVHUFF = AV_CODEC_ID_FFVHUFF,                    
                /// <summary>
                /// RealVideo 8, suspected to based largely on an early draft of H.264 (included with RealPlayer 8).
                /// </summary>
                RV30 = AV_CODEC_ID_RV30,
                    
                /// <summary>
                /// RealVideo 9/10 (RV9 EHQ, included with RealPlayer 10)
                /// </summary>
                RV40 = AV_CODEC_ID_RV40,
                    
                /// <summary>
                /// SMPTE 421M, informally known as VC-1, was initially developed as the proprietary video 
                /// format Windows Media Video 9 by Microsoft in 2003 and officially approved as a SMPTE 
                /// video codec standard on April 3, 2006. It is today a supported standard found in Blu-ray
                /// Discs, Microsoft's Silverlight framework, and the now-discontinued HD-DVD.
                /// </summary>
                VC1 = AV_CODEC_ID_VC1,
                    
                /// <summary>
                /// Windows Media Video 9.
                /// </summary>
                WMV3 = AV_CODEC_ID_WMV3,
                LOCO = AV_CODEC_ID_LOCO,
                WNV1 = AV_CODEC_ID_WNV1,
                AASC = AV_CODEC_ID_AASC,
                INDEO2 = AV_CODEC_ID_INDEO2,
                FRAPS = AV_CODEC_ID_FRAPS,
                TRUEMOTION2 = AV_CODEC_ID_TRUEMOTION2,
                BMP = AV_CODEC_ID_BMP,
                CSCD = AV_CODEC_ID_CSCD,
                MMVIDEO = AV_CODEC_ID_MMVIDEO,
                ZMBV = AV_CODEC_ID_ZMBV,
                AVS = AV_CODEC_ID_AVS,
                SMACKVIDEO = AV_CODEC_ID_SMACKVIDEO,
                NUV = AV_CODEC_ID_NUV,
                KMVC = AV_CODEC_ID_KMVC,
                FLASHSV = AV_CODEC_ID_FLASHSV,
                CAVS = AV_CODEC_ID_CAVS,
                JPEG2000 = AV_CODEC_ID_JPEG2000,
                VMNC = AV_CODEC_ID_VMNC,
                VP5 = AV_CODEC_ID_VP5,
                VP6 = AV_CODEC_ID_VP6,
                VP6F = AV_CODEC_ID_VP6F,
                TARGA = AV_CODEC_ID_TARGA,
                DSICINVIDEO = AV_CODEC_ID_DSICINVIDEO,
                TIERTEXSEQVIDEO = AV_CODEC_ID_TIERTEXSEQVIDEO,
                TIFF = AV_CODEC_ID_TIFF,
                GIF = AV_CODEC_ID_GIF,
                DXA = AV_CODEC_ID_DXA,
                DNXHD = AV_CODEC_ID_DNXHD,
                THP = AV_CODEC_ID_THP,
                SGI = AV_CODEC_ID_SGI,
                C93 = AV_CODEC_ID_C93,
                BETHSOFTVID = AV_CODEC_ID_BETHSOFTVID,
                PTX = AV_CODEC_ID_PTX,
                TXD = AV_CODEC_ID_TXD,
                VP6A = AV_CODEC_ID_VP6A,
                AMV = AV_CODEC_ID_AMV,
                VB = AV_CODEC_ID_VB,
                PCX = AV_CODEC_ID_PCX,
                SUNRAST = AV_CODEC_ID_SUNRAST,
                INDEO4 = AV_CODEC_ID_INDEO4,
                INDEO5 = AV_CODEC_ID_INDEO5,
                MIMIC = AV_CODEC_ID_MIMIC,
                RL2 = AV_CODEC_ID_RL2,
                ESCAPE124 = AV_CODEC_ID_ESCAPE124,
                DIRAC = AV_CODEC_ID_DIRAC,
                BFI = AV_CODEC_ID_BFI,
                CMV = AV_CODEC_ID_CMV,
                MOTIONPIXELS = AV_CODEC_ID_MOTIONPIXELS,
                TGV = AV_CODEC_ID_TGV,
                TGQ = AV_CODEC_ID_TGQ,
                TQI = AV_CODEC_ID_TQI,
                AURA = AV_CODEC_ID_AURA,
                AURA2 = AV_CODEC_ID_AURA2,
                V210X = AV_CODEC_ID_V210X,
                TMV = AV_CODEC_ID_TMV,
                V210 = AV_CODEC_ID_V210,
                DPX = AV_CODEC_ID_DPX,
                MAD = AV_CODEC_ID_MAD,
                FRWU = AV_CODEC_ID_FRWU,
                FLASHSV2 = AV_CODEC_ID_FLASHSV2,
                CDGRAPHICS = AV_CODEC_ID_CDGRAPHICS,
                R210 = AV_CODEC_ID_R210,
                ANM = AV_CODEC_ID_ANM,
                BINKVIDEO = AV_CODEC_ID_BINKVIDEO,
                IFF_ILBM = AV_CODEC_ID_IFF_ILBM,
                IFF_BYTERUN1 = AV_CODEC_ID_IFF_BYTERUN1,
                KGV1 = AV_CODEC_ID_KGV1,
                YOP = AV_CODEC_ID_YOP,
                VP8 = AV_CODEC_ID_VP8,
                PICTOR = AV_CODEC_ID_PICTOR,
                ANSI = AV_CODEC_ID_ANSI,
                A64_MULTI = AV_CODEC_ID_A64_MULTI,
                A64_MULTI5 = AV_CODEC_ID_A64_MULTI5,
                R10K = AV_CODEC_ID_R10K,
                MXPEG = AV_CODEC_ID_MXPEG,
                LAGARITH = AV_CODEC_ID_LAGARITH,
                PRORES = AV_CODEC_ID_PRORES,
                JV = AV_CODEC_ID_JV,
                DFA = AV_CODEC_ID_DFA,
                WMV3IMAGE = AV_CODEC_ID_WMV3IMAGE,
                VC1IMAGE = AV_CODEC_ID_VC1IMAGE,
                UTVIDEO = AV_CODEC_ID_UTVIDEO,
                BMV_VIDEO = AV_CODEC_ID_BMV_VIDEO,
                VBLE = AV_CODEC_ID_VBLE,
                DXTORY = AV_CODEC_ID_DXTORY,
                V410 = AV_CODEC_ID_V410,
                XWD = AV_CODEC_ID_XWD,
                CDXL = AV_CODEC_ID_CDXL,
                XBM = AV_CODEC_ID_XBM,
                ZEROCODEC = AV_CODEC_ID_ZEROCODEC,
                MSS1 = AV_CODEC_ID_MSS1,
                MSA1 = AV_CODEC_ID_MSA1,
                TSCC2 = AV_CODEC_ID_TSCC2,
                MTS2 = AV_CODEC_ID_MTS2,
                CLLC = AV_CODEC_ID_CLLC,
                MSS2 = AV_CODEC_ID_MSS2,
                VP9 = AV_CODEC_ID_VP9,
                AIC = AV_CODEC_ID_AIC,
                ESCAPE130 = AV_CODEC_ID_ESCAPE130,
                G2M = AV_CODEC_ID_G2M,
                WEBP = AV_CODEC_ID_WEBP,
                HNM4_VIDEO = AV_CODEC_ID_HNM4_VIDEO,
                HEVC = AV_CODEC_ID_HEVC,
                H265 = HEVC,
                FIC = AV_CODEC_ID_FIC,
                AliasPIX = AV_CODEC_ID_ALIAS_PIX,
                BrenderPIX = AV_CODEC_ID_BRENDER_PIX,
                PAFVideo = AV_CODEC_ID_PAF_VIDEO,
                EXR = AV_CODEC_ID_EXR,

                /// <summary>
                /// FFmpeg VP7.
                /// </summary>
                VP7 = AV_CODEC_ID_VP7,

                SANM = AV_CODEC_ID_SANM,
                SGIRLE = AV_CODEC_ID_SGIRLE,
                MVC1 = AV_CODEC_ID_MVC1,
                MVC2 = AV_CODEC_ID_MVC2,
                HQX = AV_CODEC_ID_HQX,
                TDSC = AV_CODEC_ID_TDSC,
                HQ_HQA = AV_CODEC_ID_HQ_HQA,
                HAP = AV_CODEC_ID_HAP,
                DDS = AV_CODEC_ID_DDS,
                DXV = AV_CODEC_ID_DXV,
                Screenpresso = AV_CODEC_ID_SCREENPRESSO,
                RSCC = AV_CODEC_ID_RSCC,

                Y41P = AV_CODEC_ID_Y41P,
                AVRP = AV_CODEC_ID_AVRP,
                FFMpeg012V = AV_CODEC_ID_012V,
                AVUI = AV_CODEC_ID_AVUI,
                AYUV = AV_CODEC_ID_AYUV,
                TARGA216 = AV_CODEC_ID_TARGA_Y216,
                V308 = AV_CODEC_ID_V308,
                V408 = AV_CODEC_ID_V408,
                YUV4 = AV_CODEC_ID_YUV4,
                AVRN = AV_CODEC_ID_AVRN,
                CPIA = AV_CODEC_ID_CPIA,
                XFACE = AV_CODEC_ID_XFACE,
                SNOW = AV_CODEC_ID_SNOW,
                SMVJPEG = AV_CODEC_ID_SMVJPEG,
                APNG = AV_CODEC_ID_APNG,
                DAALA = AV_CODEC_ID_DAALA,
                CFHD = AV_CODEC_ID_CFHD,
                TrueMotion2RT = AV_CODEC_ID_TRUEMOTION2RT,
                M101 = AV_CODEC_ID_M101,
                MagicYUV = AV_CODEC_ID_MAGICYUV,
                SheerVideo = AV_CODEC_ID_SHEERVIDEO,
                YLC = AV_CODEC_ID_YLC,
            };

        }
    }
}
