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
            ///   Enumeration of some video codecs from FFmpeg library, which are available 
            ///   for writing video files. See remarks for more details regarding patents.
            /// </summary>
            /// 
            /// <remarks>
            ///   Some video codecs made available by the FFmpeg library are under one or more
            ///   software patents. Before using any codec in your application, please check the
            ///   FFmpeg documentation (and not this documentation page) to determine who is the
            ///   provider or owner of the technology behind the codec and whether it should be
            ///   necessary to acquire a license to use it.
            /// </remarks>
            ///
            public enum class VideoCodec
            {
                /// <summary>
                /// No video.
                /// </summary>
                ///
                None = AV_CODEC_ID_NONE,

                /// <summary>
                ///   Special codec identifier meaning that the 
                ///   audio codec should be chosen automatically.
                /// </summary>
                Default = AV_CODEC_ID_PROBE,

                /// <summary>
                ///   MPEG-1.
                /// </summary>
                Mpeg1 = AV_CODEC_ID_MPEG1VIDEO,

                /// <summary>
                ///   MPEG-2. Preferred ID for MPEG-1/2 video decoding.
                /// </summary>
                Mpeg2 = AV_CODEC_ID_MPEG2VIDEO,

                /// <summary>
                ///   MPEG-2 XVMC.
                /// </summary>
                Mpeg2Xvmc = AV_CODEC_ID_MPEG2VIDEO_XVMC,

                /// <summary>
                ///   H.261.
                /// </summary>
                H261 = AV_CODEC_ID_H261,

                /// <summary>
                ///   H.263.
                /// </summary>
                H263 = AV_CODEC_ID_H263,

                /// <summary>
                ///   RealVideo 1.0, based on H.263 (included with RealPlayer 5).
                /// </summary>
                Rv10 = AV_CODEC_ID_RV10,

                /// <summary>
                ///    RealVideo G2 and RealVideo G2+SVT, also based on h.263 (included with RealPlayer 6).
                /// </summary>
                Rv20 = AV_CODEC_ID_RV20,

                /// <summary>
                ///   Motion JPEG (M-JPEG or MJPEG).
                /// </summary>
                Mjpeg = AV_CODEC_ID_MJPEG,

                /// <summary>
                ///   QuickTime's MJPEG-B.
                /// </summary>
                MjpegB = AV_CODEC_ID_MJPEGB,

                /// <summary>
                ///   Lossless JPEG.
                /// </summary>
                Ljpeg = AV_CODEC_ID_LJPEG,

                /// <summary>
                ///   Sunplus SP5X codec.
                /// </summary>
                Sp5X = AV_CODEC_ID_SP5X,

                /// <summary>
                ///   JPEG-LS lossless/near-lossless compression standard for continuous-tone images (ISO-14495-1/ITU-T.87).
                /// </summary>
                JpegLs = AV_CODEC_ID_JPEGLS,

                /// <summary>
                ///   MPEG-4 file format version 2.
                /// </summary>
                Mpeg4 = AV_CODEC_ID_MPEG4,

                /// <summary>
                ///   Raw video.
                /// </summary>
                Raw = AV_CODEC_ID_RAWVIDEO,

                /// <summary>
                ///   Windows Media MPEG4 V1.
                /// </summary>
                MsMpeg4v1 = AV_CODEC_ID_MSMPEG4V1,

                /// <summary>
                ///   Windows Media MPEG4 V2.
                /// </summary>
                MsMpeg4v2 = AV_CODEC_ID_MSMPEG4V2,

                /// <summary>
                ///   Windows Media MPEG-4 V3.
                /// </summary>
                MsMpeg4v3 = AV_CODEC_ID_MSMPEG4V3,

                /// <summary>
                ///   Windows Media Video 7.
                /// </summary>
                Wmv1 = AV_CODEC_ID_WMV1,

                /// <summary>
                ///   Windows Media Video 8.
                /// </summary>
                Wmv2 = AV_CODEC_ID_WMV2,

                /// <summary>
                ///   H.263+ (H.263-1998).
                /// </summary>
                H263P = AV_CODEC_ID_H263P,

                /// <summary>
                ///   H.263I (Intel).
                /// </summary>
                H263I = AV_CODEC_ID_H263I,

                /// <summary>
                ///   Sorenson Spark (FLV1).
                /// </summary>
                Flv1 = AV_CODEC_ID_FLV1,

                /// <summary>
                ///   Sorenson Video v1.
                /// </summary>
                Svq1 = AV_CODEC_ID_SVQ1,

                /// <summary>
                ///   Sorenson Video v3.
                /// </summary>
                Svq3 = AV_CODEC_ID_SVQ3,

                /// <summary>
                ///   DV Video.
                /// </summary>
                DvVideo = AV_CODEC_ID_DVVIDEO,

                /// <summary>
                ///   Huffyuv lossless codec.
                /// </summary>
                HuffYuv = AV_CODEC_ID_HUFFYUV,

                /// <summary>
                ///   Creative YUV (CYUV).
                /// </summary>
                CYuv = AV_CODEC_ID_CYUV,

                /// <summary>
                ///   H.264 or MPEG-4 Part 10, Advanced Video Coding (MPEG-4 AVC).
                /// </summary>
                H264 = AV_CODEC_ID_H264,

                /// <summary>
                ///   Intel's Indeo Video 3.
                /// </summary>
                Indeo3 = AV_CODEC_ID_INDEO3,

                /// <summary>
                ///   VP3.
                /// </summary>
                Vp3 = AV_CODEC_ID_VP3,

                /// <summary>
                ///   Theora.
                /// </summary>
                Theora = AV_CODEC_ID_THEORA,

                /// <summary>
                ///   ASUS ASV1.
                /// </summary>
                Asv1 = AV_CODEC_ID_ASV1,

                /// <summary>
                ///   ASUS ASV2.
                /// </summary>
                Asv2 = AV_CODEC_ID_ASV2,

                /// <summary>
                ///   FF video codec 1, lossless intra-frame video codec.
                /// </summary>
                Ffv1 = AV_CODEC_ID_FFV1,

                /// <summary>
                ///   4X Technologies codec.
                /// </summary>
                FourXm = AV_CODEC_ID_4XM,

                /// <summary>
                ///   ATI VCR1.
                /// </summary>
                Vcr1 = AV_CODEC_ID_VCR1,

                /// <summary>
                ///   Cirrus Logic AccuPak.
                /// </summary>
                ClJr = AV_CODEC_ID_CLJR,

                /// <summary>
                ///   Sony PlayStation MDEC (Motion DECoder).
                /// </summary>
                Mdec = AV_CODEC_ID_MDEC,

                /// <summary>
                ///   RoQ.
                /// </summary>
                RoQ = AV_CODEC_ID_ROQ,

                /// <summary>
                ///   Interplay Video.
                /// </summary>
                InterplayVideo = AV_CODEC_ID_INTERPLAY_VIDEO,

                /// <summary>
                ///   Wing Commander III / Xan
                /// </summary>
                XanWc3 = AV_CODEC_ID_XAN_WC3,

                /// <summary>
                ///   Wing Commander IV / Xxan
                /// </summary>
                XanWc4 = AV_CODEC_ID_XAN_WC4,

                /// <summary>
                ///   Apple Video (rpza) Codec.
                /// </summary>
                Rpza = AV_CODEC_ID_RPZA,

                /// <summary>
                ///   Cinepak codec.
                /// </summary>
                Cinepak = AV_CODEC_ID_CINEPAK,

                /// <summary>
                ///   Westwood Studios VQA.
                /// </summary>
                WsVqa = AV_CODEC_ID_WS_VQA,

                /// <summary>
                /// Microsoft RLE.
                /// </summary>
                MsRle = AV_CODEC_ID_MSRLE,

                /// <summary>
                ///   Microsoft Video 1 (MS-CRAM).
                /// </summary>
                MsVideo1 = AV_CODEC_ID_MSVIDEO1,

                /// <summary>
                ///   id Cinematic codec
                /// </summary>
                IdCin = AV_CODEC_ID_IDCIN,

                /// <summary>
                ///   Planar RGB (8BPS) Codec.
                /// </summary>
                EightBps = AV_CODEC_ID_8BPS,

                /// <summary>
                ///   Apple Graphics (SMC) Codec.
                /// </summary>
                Smc = AV_CODEC_ID_SMC,

                /// <summary>
                ///   FLI/FLC/FLX animation.
                /// </summary>
                Flic = AV_CODEC_ID_FLIC,

                /// <summary>
                ///   Duck TrueMotion 1
                /// </summary>
                TrueMotion1 = AV_CODEC_ID_TRUEMOTION1,

                /// <summary>
                ///   Sierra VMD video
                /// </summary>
                VmdVideo = AV_CODEC_ID_VMDVIDEO,

                /// <summary>
                ///   MSZH Lossless codec.
                /// </summary>
                Mszh = AV_CODEC_ID_MSZH,

                /// <summary>
                ///   Zlib Lossless codec.
                /// </summary>
                Zlib = AV_CODEC_ID_ZLIB,

                /// <summary>
                ///   Apple QuickTime RLE.
                /// </summary>
                QtRle = AV_CODEC_ID_QTRLE,

                /// <summary>
                ///   TechSmith Screen Capture Codec (TSCC).
                /// </summary>
                Tscc = AV_CODEC_ID_TSCC,

                /// <summary>
                ///   IBM UltiMotion (ULTI) Video Codec.
                /// </summary>
                Ulti = AV_CODEC_ID_ULTI,

                /// <summary>
                ///   QuickTime QDraw.
                /// </summary>
                QDraw = AV_CODEC_ID_QDRAW,

                /// <summary>
                /// Miro Video XL codec.
                /// </summary>
                ViXl = AV_CODEC_ID_VIXL,

                /// <summary>
                ///   Q-team QPEG.
                /// </summary>
                Qpeg = AV_CODEC_ID_QPEG,

                /// <summary>
                ///   PNG image format.
                /// </summary>
                Png = AV_CODEC_ID_PNG,

                /// <summary>
                ///   Portable PixelMap image.
                /// </summary>
                Ppm = AV_CODEC_ID_PPM,

                /// <summary>
                ///   Portable BitMap image.
                /// </summary>
                Pbm = AV_CODEC_ID_PBM,

                /// <summary>
                ///   Portable GrayMap image.
                /// </summary>
                Pgm = AV_CODEC_ID_PGM,

                /// <summary>
                ///   PGM with U and V components in YUV 4:2:0.
                /// </summary>
                PgmYuv = AV_CODEC_ID_PGMYUV,

                /// <summary>
                ///   Pulse-amplitude modulation (PAM).
                /// </summary>
                Pam = AV_CODEC_ID_PAM,

                /// <summary>
                ///   Huffyuv FFmpeg variant.
                /// </summary>
                FfvHuff = AV_CODEC_ID_FFVHUFF,

                /// <summary>
                /// RealVideo 8, suspected to based largely on an early draft of H.264 (included with RealPlayer 8).
                /// </summary>
                Rv30 = AV_CODEC_ID_RV30,

                /// <summary>
                /// RealVideo 9/10 (RV9 EHQ, included with RealPlayer 10)
                /// </summary>
                Rv40 = AV_CODEC_ID_RV40,

                /// <summary>
                /// SMPTE 421M, informally known as VC-1, was initially developed as the proprietary video 
                /// format Windows Media Video 9 by Microsoft in 2003 and officially approved as a SMPTE 
                /// video codec standard on April 3, 2006. It is today a supported standard found in Blu-ray
                /// Discs, Microsoft's Silverlight framework, and the now-discontinued HD-DVD.
                /// </summary>
                Vc1 = AV_CODEC_ID_VC1,

                /// <summary>
                /// Windows Media Video 9.
                /// </summary>
                Wmv3 = AV_CODEC_ID_WMV3,

                /// <summary>
                ///   LOCO-I codec.
                /// </summary>
                Loco = AV_CODEC_ID_LOCO,

                /// <summary>
                ///   Winnov WNV1.
                /// </summary>
                Wnv1 = AV_CODEC_ID_WNV1,

                /// <summary>
                ///   Autodesk Animator Codec (AASC).
                /// </summary>
                Aasc = AV_CODEC_ID_AASC,

                /// <summary>
                ///   Intel's Indeo Video 2.
                /// </summary>
                Indeo2 = AV_CODEC_ID_INDEO2,

                /// <summary>
                ///   Fraps codec.
                /// </summary>
                Fraps = AV_CODEC_ID_FRAPS,

                /// <summary>
                ///   TrueMotion 2 codec.
                /// </summary>
                TrueMotion2 = AV_CODEC_ID_TRUEMOTION2,

                /// <summary>
                ///   Windows Bitmap (BMP).
                /// </summary>
                Bmp = AV_CODEC_ID_BMP,

                /// <summary>
                ///    CamStudio Codec (CSCD) 
                /// </summary>
                Cscd = AV_CODEC_ID_CSCD,

                /// <summary>
                ///   American Laser Games MM Video.
                /// </summary>
                MmVideo = AV_CODEC_ID_MMVIDEO,

                /// <summary>
                ///    Zip Motion Blocks Video (ZMBV).
                /// </summary>
                Zmbv = AV_CODEC_ID_ZMBV,

                /// <summary>
                ///   Audio Video Coding Standard (AVS).
                /// </summary>
                Avs = AV_CODEC_ID_AVS,

                /// <summary>
                /// RAD Game Tools' Smacker video.
                /// </summary>
                SmackVideo = AV_CODEC_ID_SMACKVIDEO,

                /// <summary>
                ///   NuppelVideo/RTJPEG.
                /// </summary>
                Nuv = AV_CODEC_ID_NUV,

                /// <summary>
                ///   Karl Morton's video codec.
                /// </summary>
                Kmvc = AV_CODEC_ID_KMVC,

                /// <summary>
                ///   Flash Screen Video.
                /// </summary>
                FlashSV = AV_CODEC_ID_FLASHSV,

                /// <summary>
                ///   Chinese AVS video.
                /// </summary>
                Cavs = AV_CODEC_ID_CAVS,

                /// <summary>
                ///   JPEG 2000 (JP2).
                /// </summary>
                Jpeg2000 = AV_CODEC_ID_JPEG2000,

                /// <summary>
                ///   VMware Movie Decoder VMnc Codec.
                /// </summary>
                Vmnc = AV_CODEC_ID_VMNC,

                /// <summary>
                ///   On2's TrueMotion VP5.
                /// </summary>
                Vp5 = AV_CODEC_ID_VP5,

                /// <summary>
                ///   On2's TrueMotion VP6.
                /// </summary>
                Vp6 = AV_CODEC_ID_VP6,

                /// <summary>
                ///   On2's TrueMotion VP6 (Flash 8).
                /// </summary>
                Vp6F = AV_CODEC_ID_VP6F,

                /// <summary>
                ///   Truevision TGA (Targa).
                /// </summary>
                Targa = AV_CODEC_ID_TARGA,

                /// <summary>
                ///   Delphine Software International CIN video.
                /// </summary>
                DsiCinVideo = AV_CODEC_ID_DSICINVIDEO,

                /// <summary>
                ///   Tiertex Seq Video.
                /// </summary>
                TiertexSeqVideo = AV_CODEC_ID_TIERTEXSEQVIDEO,

                /// <summary>
                ///   Tagged Image File Format (TIFF).
                /// </summary>
                Tiff = AV_CODEC_ID_TIFF,

                /// <summary>
                ///   Graphics Interchange Format 89a (GIF).
                /// </summary>
                Gif = AV_CODEC_ID_GIF,

                /// <summary>
                ///   Runesoft GmbH's DXA format.
                /// </summary>
                Dxa = AV_CODEC_ID_DXA,

                /// <summary>
                /// Avid's Digital Nonlinear Extensible High Definition (DNxHD) codec (SMPTE VC-3).
                /// </summary>
                DnxHd = AV_CODEC_ID_DNXHD,

                /// <summary>
                ///   THP format.
                /// </summary>
                Thp = AV_CODEC_ID_THP,

                /// <summary>
                ///   Silicon Graphics Image.
                /// </summary>
                Sgi = AV_CODEC_ID_SGI,

                /// <summary>
                ///   Interplay C93.
                /// </summary>
                C93 = AV_CODEC_ID_C93,

                /// <summary>
                ///   Bethesda Softworks VID format.
                /// </summary>
                BethSoftVid = AV_CODEC_ID_BETHSOFTVID,

                /// <summary>
                ///   V.Flash PTX image.
                /// </summary>
                Ptx = AV_CODEC_ID_PTX,

                /// <summary>
                ///   Renderware TeXture Dictionary.
                /// </summary>
                Txd = AV_CODEC_ID_TXD,

                /// <summary>
                ///   On2's TrueMotion VP6 (with alpha transparency).
                /// </summary>
                Vp6A = AV_CODEC_ID_VP6A,

                /// <summary>
                ///   AMV codec.
                /// </summary>
                Amv = AV_CODEC_ID_AMV,

                /// <summary>
                ///   Beam Software VB.
                /// </summary>
                Vb = AV_CODEC_ID_VB,

                /// <summary>
                ///   PC Paintbrush PCX image.
                /// </summary>
                Pcx = AV_CODEC_ID_PCX,

                /// <summary>
                ///   Sun Rasterfile Image Format
                /// </summary>
                Sunrast = AV_CODEC_ID_SUNRAST,

                /// <summary>
                ///   Intel's Indeo Video 4.
                /// </summary>
                Indeo4 = AV_CODEC_ID_INDEO4,

                /// <summary>
                ///   Intel's Indeo Video 5.
                /// </summary>
                Indeo5 = AV_CODEC_ID_INDEO5,

                /// <summary>
                ///   Mimic codec used by MSN Messenger for webcam conversations.
                /// </summary>
                Mimic = AV_CODEC_ID_MIMIC,

                /// <summary>
                ///   RL2 video.
                /// </summary>
                Rl2 = AV_CODEC_ID_RL2,

                /// <summary>
                ///   Escape 124.
                /// </summary>
                Escape124 = AV_CODEC_ID_ESCAPE124,

                /// <summary>
                ///   Raw Dirac format.
                /// </summary>
                Dirac = AV_CODEC_ID_DIRAC,

                /// <summary>
                ///   Brute Force and Ignorance.
                /// </summary>
                Bfi = AV_CODEC_ID_BFI,

                /// <summary>
                ///   Electronic Arts CMV video.
                /// </summary>
                Cmv = AV_CODEC_ID_CMV,

                /// <summary>
                ///   MotionPixels (MP codec, used in MovieCD).
                /// </summary>
                MotionPixels = AV_CODEC_ID_MOTIONPIXELS,

                /// <summary>
                ///   Electronic Arts TGV video.
                /// </summary>
                Tgv = AV_CODEC_ID_TGV,

                /// <summary>
                ///   Electronic Arts TGQ video.
                /// </summary>
                Tgq = AV_CODEC_ID_TGQ,

                /// <summary>
                ///   Electronic Arts TQI video.
                /// </summary>
                Tqi = AV_CODEC_ID_TQI,

                /// <summary>
                ///   AURA AuraVision - Aura 1 Video Codec.
                /// </summary>
                Aura = AV_CODEC_ID_AURA,

                /// <summary>
                ///   AURA AuraVision - Aura 2 Video Codec.
                /// </summary>
                Aura2 = AV_CODEC_ID_AURA2,

                /// <summary>
                ///   Uncompressed 4:2:2 10-bit.
                /// </summary>
                V210X = AV_CODEC_ID_V210X,

                /// <summary>
                ///   8088flex TMV.
                /// </summary>
                Tmv = AV_CODEC_ID_TMV,

                /// <summary>
                ///   Uncompressed 4:2:2 10-bit.
                /// </summary>
                V210 = AV_CODEC_ID_V210,

                /// <summary>
                ///   Digital Picture Exchange (DPX).
                /// </summary>
                Dpx = AV_CODEC_ID_DPX,

                /// <summary>
                ///   Electronic Arts MAD.
                /// </summary>
                Mad = AV_CODEC_ID_MAD,

                /// <summary>
                ///   Forward Uncompressed.
                /// </summary>
                Frwu = AV_CODEC_ID_FRWU,

                /// <summary>
                ///   Flash Screen Video v2.
                /// </summary>
                FlashSV2 = AV_CODEC_ID_FLASHSV2,

                /// <summary>
                ///   CD Graphics video
                /// </summary>
                CdGraphics = AV_CODEC_ID_CDGRAPHICS,

                /// <summary>
                ///   R210 Quicktime Uncompressed RGB 10-bit.
                /// </summary>
                R210 = AV_CODEC_ID_R210,

                /// <summary>
                ///   Deluxe Paint Animation.
                /// </summary>
                Anm = AV_CODEC_ID_ANM,

                /// <summary>
                ///   RAD Game Tools' Bink Video.
                /// </summary>
                BinkVideo = AV_CODEC_ID_BINKVIDEO,

                /// <summary>
                ///   Interchange File Format (IFF)'s Interleaved Bitmap (ILBM).
                /// </summary>
                IffIlbm = AV_CODEC_ID_IFF_ILBM,

                /// <summary>
                ///   Interchange File Format (IFF)'s ByteRun1.
                /// </summary>
                IffByteRun1 = AV_CODEC_ID_IFF_BYTERUN1,

                /// <summary>
                ///   Kega Game Video.
                /// </summary>
                Kgv1 = AV_CODEC_ID_KGV1,

                /// <summary>
                ///   Psygnosis YOP Video.
                /// </summary>
                Yop = AV_CODEC_ID_YOP,

                /// <summary>
                ///   Google's VP8.
                /// </summary>
                Vp8 = AV_CODEC_ID_VP8,

                /// <summary>
                ///   Pictor/PC Paint.
                /// </summary>
                Pictor = AV_CODEC_ID_PICTOR,

                /// <summary>
                ///   ASCII/ANSI art.
                /// </summary>
                Ansi = AV_CODEC_ID_ANSI,

                /// <summary>
                ///   Multicolor charset for Commodore 64
                /// </summary>
                A64Multi = AV_CODEC_ID_A64_MULTI,

                /// <summary>
                ///   Multicolor charset for Commodore 64, extended with 5th color (colram)
                /// </summary>
                A64Multi5 = AV_CODEC_ID_A64_MULTI5,

                /// <summary>
                ///   AJA Kona 10-bit RGB Codec.
                /// </summary>
                R10K = AV_CODEC_ID_R10K,

                /// <summary>
                ///   Mobotix's MxPEG codec.
                /// </summary>
                Mxpeg = AV_CODEC_ID_MXPEG,

                /// <summary>
                ///   Lagarith lossless
                /// </summary>
                Lagarith = AV_CODEC_ID_LAGARITH,

                /// <summary>
                ///   ProRes
                /// </summary>
                ProRes = AV_CODEC_ID_PRORES,

                /// <summary>
                ///   Bitmap Brothers JV.
                /// </summary>
                Jv = AV_CODEC_ID_JV,

                /// <summary>
                ///   Chronomaster DFA.
                /// </summary>
                Dfa = AV_CODEC_ID_DFA,

                /// <summary>
                ///   Windows Media Video 9 Image
                /// </summary>
                Wmv3Image = AV_CODEC_ID_WMV3IMAGE,

                /// <summary>
                ///   Windows Media Video 9 Image v2.
                /// </summary>
                Vc1Image = AV_CODEC_ID_VC1IMAGE,

                /// <summary>
                ///   UT Video Codec Suite
                /// </summary>
                UtVideo = AV_CODEC_ID_UTVIDEO,

                /// <summary>
                ///   Discworld II BMV video.
                /// </summary>
                BmvVideo = AV_CODEC_ID_BMV_VIDEO,

                /// <summary>
                ///   VBLE Lossless Codec.
                /// </summary>
                Vble = AV_CODEC_ID_VBLE,

                /// <summary>
                ///    Dxtory.
                /// </summary>
                Dxtory = AV_CODEC_ID_DXTORY,

                /// <summary>
                ///    Uncompressed 4:4:4 10-bit.
                /// </summary>
                V410 = AV_CODEC_ID_V410,

                /// <summary>
                ///   XWD (X Window Dump) image.
                /// </summary>
                Xwd = AV_CODEC_ID_XWD,

                /// <summary>
                ///   Commodore CDXL video.
                /// </summary>
                Cdxl = AV_CODEC_ID_CDXL,

                /// <summary>
                ///   XBM (X BitMap) image.
                /// </summary>
                Xbm = AV_CODEC_ID_XBM,

                /// <summary>
                ///   ZeroCodec Lossless Video.
                /// </summary>
                ZeroCodec = AV_CODEC_ID_ZEROCODEC,

                /// <summary>
                ///   MS Screen 1.
                /// </summary>
                MsS1 = AV_CODEC_ID_MSS1,

                /// <summary>
                ///   MS ATC Screen.
                /// </summary>
                MsA1 = AV_CODEC_ID_MSA1,

                /// <summary>
                ///   TechSmith Screen Codec 2.
                /// </summary>
                Tscc2 = AV_CODEC_ID_TSCC2,

                /// <summary>
                ///   MS Expression Encoder Screen.
                /// </summary>
                Mts2 = AV_CODEC_ID_MTS2,

                /// <summary>
                ///   Canopus Lossless Codec.
                /// </summary>
                Cllc = AV_CODEC_ID_CLLC,

                /// <summary>
                ///   MS Screen 2.
                /// </summary>
                MsS2 = AV_CODEC_ID_MSS2,

                /// <summary>
                ///   Google's VP9.
                /// </summary>
                Vp9 = AV_CODEC_ID_VP9,

                /// <summary>
                ///   Apple Intermediate Codec.
                /// </summary>
                Aic = AV_CODEC_ID_AIC,

                /// <summary>
                ///    Escape 130.
                /// </summary>
                Escape130 = AV_CODEC_ID_ESCAPE130,

                /// <summary>
                ///   Go2Meeting.
                /// </summary>
                G2M = AV_CODEC_ID_G2M,

                /// <summary>
                ///   WebP image format.
                /// </summary>
                WebP = AV_CODEC_ID_WEBP,

                /// <summary>
                ///   HNM 4 video
                /// </summary>
                Hnm4Video = AV_CODEC_ID_HNM4_VIDEO,

                /// <summary>
                ///   High Efficiency Video Coding (HEVC), also known as H.265 and MPEG-H Part 2.
                /// </summary>
                Hevc = AV_CODEC_ID_HEVC,

                /// <summary>
                ///   High Efficiency Video Coding (HEVC), also known as H.265 and MPEG-H Part 2.
                ///   This is an alias for <see cref="Hevc"/>.
                /// </summary>
                H265 = Hevc,

                /// <summary>
                ///   Mirillis FIC.
                /// </summary>
                Fic = AV_CODEC_ID_FIC,

                /// <summary>
                ///   Alias/Wavefront PIX image.
                /// </summary>
                AliasPix = AV_CODEC_ID_ALIAS_PIX,

                /// <summary>
                ///   BRender PIX image.
                /// </summary>
                BrenderPIX = AV_CODEC_ID_BRENDER_PIX,

                /// <summary>
                ///   Amazing Studio Packed Animation File Video.
                /// </summary>
                PafVideo = AV_CODEC_ID_PAF_VIDEO,

                /// <summary>
                ///   OpenEXR image.
                /// </summary>
                Exr = AV_CODEC_ID_EXR,

                /// <summary>
                /// FFmpeg VP7.
                /// </summary>
                Vp7 = AV_CODEC_ID_VP7,

                /// <summary>
                ///   LucasArts SANM/SMUSH video.
                /// </summary>
                Sanm = AV_CODEC_ID_SANM,

                /// <summary>
                ///   SGI RLE 8-bit.
                /// </summary>
                SgiRle = AV_CODEC_ID_SGIRLE,

                /// <summary>
                ///   Silicon Graphics Motion Video Compressor 1.
                /// </summary>
                Mvc1 = AV_CODEC_ID_MVC1,

                /// <summary>
                ///   Silicon Graphics Motion Video Compressor 2.
                /// </summary>
                Mvc2 = AV_CODEC_ID_MVC2,

                /// <summary>
                ///   Canopus HQX.
                /// </summary>
                Hqx = AV_CODEC_ID_HQX,

                /// <summary>
                ///   TDSC.
                /// </summary>
                Tdsc = AV_CODEC_ID_TDSC,

                /// <summary>
                ///   Canopus HQ/HQA decoder
                /// </summary>
                HqHqa = AV_CODEC_ID_HQ_HQA,

                /// <summary>
                ///   Vidvox Hap decoder.
                /// </summary>
                Hap = AV_CODEC_ID_HAP,

                /// <summary>
                ///   DirectDraw Surface image decoder.
                /// </summary>
                Dds = AV_CODEC_ID_DDS,

                /// <summary>
                ///   Resolume DXV.
                /// </summary>
                Dxv = AV_CODEC_ID_DXV,

                /// <summary>
                ///   Screenpresso SPV1.
                /// </summary>
                Screenpresso = AV_CODEC_ID_SCREENPRESSO,

                /// <summary>
                ///   innoHeim/Rsupport Screen Capture Codec.
                /// </summary>
                Rscc = AV_CODEC_ID_RSCC,

                /// <summary>
                ///   Uncompressed YUV 4:1:1 12-bit.
                /// </summary>
                Y41P = AV_CODEC_ID_Y41P,

                /// <summary>
                ///   Avid 1:1 10-bit RGB Packer.
                /// </summary>
                AvRp = AV_CODEC_ID_AVRP,

                /// <summary>
                ///   Uncompressed 4:2:2 10-bit.
                /// </summary>
                Zero12V = AV_CODEC_ID_012V,

                /// <summary>
                ///   Avid Meridien Uncompressed.
                /// </summary>
                AvUi = AV_CODEC_ID_AVUI,

                /// <summary>
                ///   Uncompressed packed MS 4:4:4:4.
                /// </summary>
                AYUV = AV_CODEC_ID_AYUV,

                /// <summary>
                ///   Pinnacle TARGA CineWave YUV16.
                /// </summary>
                TargaY216 = AV_CODEC_ID_TARGA_Y216,

                /// <summary>
                ///   Uncompressed packed 4:4:4.
                /// </summary>
                V308 = AV_CODEC_ID_V308,

                /// <summary>
                ///   Uncompressed packed QT 4:4:4:4.
                /// </summary>
                V408 = AV_CODEC_ID_V408,

                /// <summary>
                ///   Uncompressed packed 4:2:0.
                /// </summary>
                Yuv4 = AV_CODEC_ID_YUV4,

                /// <summary>
                ///   Avid AVI Codec.
                /// </summary>
                AvRn = AV_CODEC_ID_AVRN,

                /// <summary>
                ///   CPiA video format.
                /// </summary>
                CPiA = AV_CODEC_ID_CPIA,

                /// <summary>
                ///   X-face image.
                /// </summary>
                Xface = AV_CODEC_ID_XFACE,

                /// <summary>
                ///   Snow.
                /// </summary>
                Snow = AV_CODEC_ID_SNOW,

                /// <summary>
                ///   Sigmatel Motion Video.
                /// </summary>
                SmvJpeg = AV_CODEC_ID_SMVJPEG,

                /// <summary>
                ///   APNG (Animated Portable Network Graphics) image.
                /// </summary>
                Apng = AV_CODEC_ID_APNG,

                /// <summary>
                ///   Daala, upcoming sucessor to Theora.
                /// </summary>
                Daala = AV_CODEC_ID_DAALA,

                /// <summary>
                ///   Cineform HD.
                /// </summary>
                CfHD = AV_CODEC_ID_CFHD,

                /// <summary>
                ///   Duck TrueMotion 2.0 Real Time.
                /// </summary>
                TrueMotion2RT = AV_CODEC_ID_TRUEMOTION2RT,

                /// <summary>
                ///   Matrox Uncompressed SD.
                /// </summary>
                M101 = AV_CODEC_ID_M101,

                /// <summary>
                ///   MagicYUV Lossless Video Codec.
                /// </summary>
                MagicYuv = AV_CODEC_ID_MAGICYUV,

                /// <summary>
                ///   BitJazz's SheerVideo lossless codec.
                /// </summary>
                SheerVideo = AV_CODEC_ID_SHEERVIDEO,

                /// <summary>
                ///   YUY2 Lossless Codec.
                /// </summary>
                Ylc = AV_CODEC_ID_YLC,







#pragma region backward compatibility
                /// <summary>
                ///   Obsolete. Please use <see cref="Mpeg4"/> instead.
                /// </summary>
                ///
                [ObsoleteAttribute("Obsolete. Please use VideoCodec.Mpeg4 instead.")]
                MPEG4 = Mpeg4,

                /// <summary>
                ///   Obsolete. Please use <see cref="Wmv1"/> instead.
                /// </summary>
                ///
                [ObsoleteAttribute("Obsolete. Please use VideoCodec.Wmv1 instead.")]
                WMV1 = Wmv1,

                /// <summary>
                ///   Obsolete. Please use <see cref="Wmv2"/> instead.
                /// </summary>
                ///
                [ObsoleteAttribute("Obsolete. Please use VideoCodec.Wmv2 instead.")]
                WMV2 = Wmv2,

                /// <summary>
                ///   Obsolete. Please use <see cref="MsMpeg4v2"/> instead.
                /// </summary>
                ///
                [ObsoleteAttribute("Obsolete. Please use VideoCodec.MsMpeg4v2 instead.")]
                MSMPEG4v2 = MsMpeg4v2,

                /// <summary>
                ///   Obsolete. Please use <see cref="MsMpeg4v3"/> instead.
                /// </summary>
                ///
                [ObsoleteAttribute("Obsolete. Please use VideoCodec.MsMpeg4v3 instead.")]
                MSMPEG4v3 = MsMpeg4v3,

                /// <summary>
                ///   Obsolete. Please use <see cref="Flv1"/> instead.
                /// </summary>
                ///
                [ObsoleteAttribute("Obsolete. Please use VideoCodec.Flv1 instead.")]
                FLV1 = Flv1,

                /// <summary>
                ///   Obsolete. Please use <see cref="Mpeg2"/> instead.
                /// </summary>
                ///
                [ObsoleteAttribute("Obsolete. Please use VideoCodec.Mpeg2 instead.")]
                MPEG2 = Mpeg2,

                /// <summary>
                ///   Obsolete. Please use <see cref="Ffv1"/> instead.
                /// </summary>
                ///
                [ObsoleteAttribute("Obsolete. Please use VideoCodec.Ffv1 instead.")]
                FFV1 = Ffv1,

                /// <summary>
                ///   Obsolete. Please use <see cref="FfvHuff"/> instead.
                /// </summary>
                ///
                [ObsoleteAttribute("Obsolete. Please use VideoCodec.FfvHuff instead.")]
                FFVHUFF = FfvHuff,

                /// <summary>
                ///   Obsolete. Please use <see cref="Vp8"/> instead.
                /// </summary>
                ///
                [ObsoleteAttribute("Obsolete. Please use VideoCodec.Vp8 instead.")]
                VP8 = Vp8,

                /// <summary>
                ///   Obsolete. Please use <see cref="Vp9"/> instead.
                /// </summary>
                ///
                [ObsoleteAttribute("Obsolete. Please use VideoCodec.Vp9 instead.")]
                VP9 = Vp9,
#pragma endregion
            };

        }
    }
}
