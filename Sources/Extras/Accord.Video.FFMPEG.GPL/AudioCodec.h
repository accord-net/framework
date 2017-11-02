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
            ///   Enumeration of some audio codecs from FFmpeg library, which are available for writing audio files.
            /// </summary>
            ///
            public enum class AudioCodec
            {
                /// <summary>
                /// No audio.
                /// </summary>
                ///
                None = AV_CODEC_ID_NONE,

                Default = AV_CODEC_ID_PROBE,
                
                /// <summary>
                /// Signed 16-bit PCM (little endian).
                /// </summary>
                PCM16 = AV_CODEC_ID_PCM_S16LE,
                /*PCM16BigEndian = AV_CODEC_ID_PCM_S16BE,
                AV_CODEC_ID_PCM_U16LE,
                AV_CODEC_ID_PCM_U16BE,
                AV_CODEC_ID_PCM_S8,
                AV_CODEC_ID_PCM_U8,
                AV_CODEC_ID_PCM_MULAW,
                AV_CODEC_ID_PCM_ALAW,
                AV_CODEC_ID_PCM_S32LE,
                AV_CODEC_ID_PCM_S32BE,
                AV_CODEC_ID_PCM_U32LE,
                AV_CODEC_ID_PCM_U32BE,
                AV_CODEC_ID_PCM_S24LE,
                AV_CODEC_ID_PCM_S24BE,
                AV_CODEC_ID_PCM_U24LE,
                AV_CODEC_ID_PCM_U24BE,
                AV_CODEC_ID_PCM_S24DAUD,
                AV_CODEC_ID_PCM_ZORK,
                AV_CODEC_ID_PCM_S16LE_PLANAR,
                AV_CODEC_ID_PCM_DVD,
                AV_CODEC_ID_PCM_F32BE,
                AV_CODEC_ID_PCM_F32LE,
                AV_CODEC_ID_PCM_F64BE,
                AV_CODEC_ID_PCM_F64LE,
                AV_CODEC_ID_PCM_BLURAY,
                AV_CODEC_ID_PCM_LXF,
                AV_CODEC_ID_S302M,
                AV_CODEC_ID_PCM_S8_PLANAR,
                AV_CODEC_ID_PCM_S24LE_PLANAR,
                AV_CODEC_ID_PCM_S32LE_PLANAR,
                AV_CODEC_ID_PCM_S16BE_PLANAR,

                AV_CODEC_ID_PCM_S64LE = 0x10800,
                AV_CODEC_ID_PCM_S64BE,*/

                /* various ADPCM codecs */
               /* AV_CODEC_ID_ADPCM_IMA_QT = 0x11000,
                AV_CODEC_ID_ADPCM_IMA_WAV,
                AV_CODEC_ID_ADPCM_IMA_DK3,
                AV_CODEC_ID_ADPCM_IMA_DK4,
                AV_CODEC_ID_ADPCM_IMA_WS,
                AV_CODEC_ID_ADPCM_IMA_SMJPEG,
                AV_CODEC_ID_ADPCM_MS,
                AV_CODEC_ID_ADPCM_4XM,
                AV_CODEC_ID_ADPCM_XA,
                AV_CODEC_ID_ADPCM_ADX,
                AV_CODEC_ID_ADPCM_EA,
                AV_CODEC_ID_ADPCM_G726,
                AV_CODEC_ID_ADPCM_CT,
                AV_CODEC_ID_ADPCM_SWF,
                AV_CODEC_ID_ADPCM_YAMAHA,
                AV_CODEC_ID_ADPCM_SBPRO_4,
                AV_CODEC_ID_ADPCM_SBPRO_3,
                AV_CODEC_ID_ADPCM_SBPRO_2,
                AV_CODEC_ID_ADPCM_THP,
                AV_CODEC_ID_ADPCM_IMA_AMV,
                AV_CODEC_ID_ADPCM_EA_R1,
                AV_CODEC_ID_ADPCM_EA_R3,
                AV_CODEC_ID_ADPCM_EA_R2,
                AV_CODEC_ID_ADPCM_IMA_EA_SEAD,
                AV_CODEC_ID_ADPCM_IMA_EA_EACS,
                AV_CODEC_ID_ADPCM_EA_XAS,
                AV_CODEC_ID_ADPCM_EA_MAXIS_XA,
                AV_CODEC_ID_ADPCM_IMA_ISS,
                AV_CODEC_ID_ADPCM_G722,
                AV_CODEC_ID_ADPCM_IMA_APC,
                AV_CODEC_ID_ADPCM_VIMA,
                AV_CODEC_ID_VIMA = AV_CODEC_ID_ADPCM_VIMA,*/

            /*    AV_CODEC_ID_ADPCM_AFC = 0x11800,
                AV_CODEC_ID_ADPCM_IMA_OKI,
                AV_CODEC_ID_ADPCM_DTK,
                AV_CODEC_ID_ADPCM_IMA_RAD,
                AV_CODEC_ID_ADPCM_G726LE,
                AV_CODEC_ID_ADPCM_THP_LE,
                AV_CODEC_ID_ADPCM_PSX,
                AV_CODEC_ID_ADPCM_AICA,
                AV_CODEC_ID_ADPCM_IMA_DAT4,
                AV_CODEC_ID_ADPCM_MTAF,*/

                /* AMR */
                /*AV_CODEC_ID_AMR_NB = 0x12000,
                AV_CODEC_ID_AMR_WB,*/

                /* RealAudio codecs*/                
                /// <summary>
                ///   Real Audio lpcJ, 14_4: IS-54 VSELP (RealAudio 1)
                /// </summary>
                RA144 = AV_CODEC_ID_RA_144,

                /// <summary>
                ///   Real Audio 28_8: G.728 LD-CELP (RealAudio 2)
                /// </summary>
                RA288 = AV_CODEC_ID_RA_288,

                /* various DPCM codecs */
               /* AV_CODEC_ID_ROQ_DPCM = 0x14000,
                AV_CODEC_ID_INTERPLAY_DPCM,
                AV_CODEC_ID_XAN_DPCM,
                AV_CODEC_ID_SOL_DPCM,

                AV_CODEC_ID_SDX2_DPCM = 0x14800,*/


                MP2 = AV_CODEC_ID_MP2,
                MP3 = AV_CODEC_ID_MP3,  // preferred ID for decoding MPEG audio layer 1, 2 or 3
                AAC = AV_CODEC_ID_AAC,
                AC3 = AV_CODEC_ID_AC3,
                DTS = AV_CODEC_ID_DTS,
                    
                /// <summary>
                /// Vorbis is a free and open-source software project headed by the Xiph.Org Foundation. The project produces
                /// an audio coding format and software reference encoder/decoder (codec) for lossy audio compression. Vorbis
                /// is most commonly used in conjunction with the Ogg container format[7] and it is therefore often referred 
                /// to as Ogg Vorbis.
                /// </summary>
                Vorbis = AV_CODEC_ID_VORBIS,

                DVAUDIO = AV_CODEC_ID_DVAUDIO,
                WMAV1 = AV_CODEC_ID_WMAV1,
                WMAV2 = AV_CODEC_ID_WMAV2,
                MACE3 = AV_CODEC_ID_MACE3,
                MACE6 = AV_CODEC_ID_MACE6,
                VMDAUDIO = AV_CODEC_ID_VMDAUDIO,
                FLAC = AV_CODEC_ID_FLAC,
                MP3ADU = AV_CODEC_ID_MP3ADU,
                MP3ON4 = AV_CODEC_ID_MP3ON4,
                SHORTEN = AV_CODEC_ID_SHORTEN,
                ALAC = AV_CODEC_ID_ALAC,
                WESTWOOD_SND1 = AV_CODEC_ID_WESTWOOD_SND1,
                GSM = AV_CODEC_ID_GSM, // as in Berlin toast format
                QDM2 = AV_CODEC_ID_QDM2,
                COOK = AV_CODEC_ID_COOK,
                TRUESPEECH = AV_CODEC_ID_TRUESPEECH,
                TTA = AV_CODEC_ID_TTA,
                SMACKAUDIO = AV_CODEC_ID_SMACKAUDIO,
                QCELP = AV_CODEC_ID_QCELP,
                WAVPACK = AV_CODEC_ID_WAVPACK,
                DSICINAUDIO = AV_CODEC_ID_DSICINAUDIO,
                IMC = AV_CODEC_ID_IMC,
                MUSEPACK7 = AV_CODEC_ID_MUSEPACK7,
                MLP = AV_CODEC_ID_MLP,
                GSM_MS = AV_CODEC_ID_GSM_MS, // as found in WAV
                ATRAC3 = AV_CODEC_ID_ATRAC3,
                    
                /// <summary>
                /// VoxWare MetaSound Audio
                /// </summary>
                VoxWare = AV_CODEC_ID_VOXWARE,

                APE = AV_CODEC_ID_APE,
                NELLYMOSER = AV_CODEC_ID_NELLYMOSER,
                MUSEPACK8 = AV_CODEC_ID_MUSEPACK8,
                SPEEX = AV_CODEC_ID_SPEEX,
                WMAVOICE = AV_CODEC_ID_WMAVOICE,
                WMAPRO = AV_CODEC_ID_WMAPRO,
                WMALOSSLESS = AV_CODEC_ID_WMALOSSLESS,
                ATRAC3P = AV_CODEC_ID_ATRAC3P,
                EAC3 = AV_CODEC_ID_EAC3,
                SIPR = AV_CODEC_ID_SIPR,
                MP1 = AV_CODEC_ID_MP1,
                TWINVQ = AV_CODEC_ID_TWINVQ,
                TRUEHD = AV_CODEC_ID_TRUEHD,
                MP4ALS = AV_CODEC_ID_MP4ALS,
                ATRAC1 = AV_CODEC_ID_ATRAC1,
                BINKAUDIO_RDFT = AV_CODEC_ID_BINKAUDIO_RDFT,
                BINKAUDIO_DCT = AV_CODEC_ID_BINKAUDIO_DCT,
                AAC_LATM = AV_CODEC_ID_AAC_LATM,
                QDMC = AV_CODEC_ID_QDMC,
                CELT = AV_CODEC_ID_CELT,
                G723_1 = AV_CODEC_ID_G723_1,
                G729 = AV_CODEC_ID_G729,

                /// <summary>
                /// 8-Bit Sampled Voice (8SVX) in Exponential mode
                /// </summary>
                EightSVXExp = AV_CODEC_ID_8SVX_EXP,

                /// <summary>
                /// 8-Bit Sampled Voice (8SVX) in Fibonacci mode
                /// </summary>
                EightSVXFib = AV_CODEC_ID_8SVX_FIB,
                
                /// <summary>
                /// BMV Audio.
                /// </summary>

                BMVAudio = AV_CODEC_ID_BMV_AUDIO,
                RALF = AV_CODEC_ID_RALF,
                IAC = AV_CODEC_ID_IAC,
                ILBC = AV_CODEC_ID_ILBC,
                OPUS = AV_CODEC_ID_OPUS,
                COMFORT_NOISE = AV_CODEC_ID_COMFORT_NOISE,
                TAK = AV_CODEC_ID_TAK,
                METASOUND = AV_CODEC_ID_METASOUND,
                PAF_AUDIO = AV_CODEC_ID_PAF_AUDIO,
                ON2AVC = AV_CODEC_ID_ON2AVC,
                DSS_SP = AV_CODEC_ID_DSS_SP,
                FFWAVESYNTH = AV_CODEC_ID_FFWAVESYNTH,
                SONIC = AV_CODEC_ID_SONIC,
                SONIC_LS = AV_CODEC_ID_SONIC_LS,
                EVRC = AV_CODEC_ID_EVRC,
                SMV = AV_CODEC_ID_SMV,
                DSD_LSBF = AV_CODEC_ID_DSD_LSBF,
                DSD_MSBF = AV_CODEC_ID_DSD_MSBF,
                DSD_LSBF_PLANAR = AV_CODEC_ID_DSD_LSBF_PLANAR,
                DSD_MSBF_PLANAR = AV_CODEC_ID_DSD_MSBF_PLANAR,                
                /// <summary>
                /// Qualcomm's 4G voice speech codecs used by CDMA networks.
                /// </summary>
                Qualcomm4GV = AV_CODEC_ID_4GV,
                INTERPLAY_ACM = AV_CODEC_ID_INTERPLAY_ACM,
                XMA1 = AV_CODEC_ID_XMA1,
                XMA2 = AV_CODEC_ID_XMA2,
                DST = AV_CODEC_ID_DST,
                
            };
        }
    }
}
