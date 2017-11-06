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
            ///   Enumeration of some audio codecs from FFmpeg library, which are available for 
            ///   writing audio and video files. See remarks for more details regarding patents.
            /// </summary>
            /// 
            /// <remarks>
            ///   Some audio codecs made available by the FFmpeg library are under one or more
            ///   software patents. Before using any codec in your application, please check the
            ///   FFmpeg documentation (and not this documentation page) to determine who is the
            ///   provider or owner of the technology behind the codec and whether it should be
            ///   necessary to acquire a license to use it.
            /// </remarks>
            ///
            public enum class AudioCodec
            {
                /// <summary>
                ///   No audio.
                /// </summary>
                ///
                None = AV_CODEC_ID_NONE,

                /// <summary>
                ///   Special codec identifier meaning that the 
                ///   audio codec should be chosen automatically.
                /// </summary>
                Default = AV_CODEC_ID_PROBE,

                /// <summary>
                /// Signed 16-bit PCM (little endian).
                /// </summary>
                Pcm16 = AV_CODEC_ID_PCM_S16LE,

                /*
                PCM16BigEndian = AV_CODEC_ID_PCM_S16BE,
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
                AV_CODEC_ID_PCM_S64BE,

                // various ADPCM codecs
                AV_CODEC_ID_ADPCM_IMA_QT = 0x11000,
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
                AV_CODEC_ID_VIMA = AV_CODEC_ID_ADPCM_VIMA,

                AV_CODEC_ID_ADPCM_AFC = 0x11800,
                AV_CODEC_ID_ADPCM_IMA_OKI,
                AV_CODEC_ID_ADPCM_DTK,
                AV_CODEC_ID_ADPCM_IMA_RAD,
                AV_CODEC_ID_ADPCM_G726LE,
                AV_CODEC_ID_ADPCM_THP_LE,
                AV_CODEC_ID_ADPCM_PSX,
                AV_CODEC_ID_ADPCM_AICA,
                AV_CODEC_ID_ADPCM_IMA_DAT4,
                AV_CODEC_ID_ADPCM_MTAF,
                */

                // AMR
                /// <summary>
                /// Adaptive Multi-Rate (AMR or AMR-NB or GSM-AMR).
                /// </summary>
                /// <remarks>                    
                ///   This is a patented algorithm, and a license needs to be acquired
                ///   from VoiceAge Corporation to use this codec in any applications.
                /// </remarks>                    
                AmrNarrowBand = AV_CODEC_ID_AMR_NB,

                /// <summary>
                ///   Adaptive Multi-Rate Wideband (AMR-WB).
                /// </summary>
                /// <remarks>                    
                ///   This is a patented algorithm, and a license needs to be acquired
                ///   from VoiceAge Corporation to use this codec in any applications.
                /// </remarks>                    
                AmrWideBand = AV_CODEC_ID_AMR_WB,

                // RealAudio codecs
                /// <summary>
                ///   Real Audio lpcJ, 14_4: IS-54 VSELP (RealAudio 1)
                /// </summary>
                Ra144 = AV_CODEC_ID_RA_144,

                /// <summary>
                ///   Real Audio 28_8: G.728 LD-CELP (RealAudio 2)
                /// </summary>
                Ra288 = AV_CODEC_ID_RA_288,

                // various DPCM codecs 
               /*
                AV_CODEC_ID_ROQ_DPCM = 0x14000,
                AV_CODEC_ID_INTERPLAY_DPCM,
                AV_CODEC_ID_XAN_DPCM,
                AV_CODEC_ID_SOL_DPCM,

                AV_CODEC_ID_SDX2_DPCM = 0x14800,*/

                /// <summary>
                ///  MPEG Audio Layer II (ISO/IEC 11172-3).
                /// </summary>
                Mp2 = AV_CODEC_ID_MP2,

                /// <summary>
                ///   MPEG Audio Layer III (mp3). This should be the preferred
                /// codec when decoding MPEG audio layer 1, 2 or 3.
                /// </summary>
                Mp3 = AV_CODEC_ID_MP3,

                /// <summary>
                ///   Advanced Audio Coding (AAC).
                /// </summary>
                Aac = AV_CODEC_ID_AAC,

                /// <summary>
                ///   Dolby Digita's AC3 codec (AC-3 (Audio Codec 3, Advanced Codec 3, Acoustic Coder 3).
                /// </summary>
                /// <remarks>
                ///   Audio codec AC3 is covered by patents (though these are now expired).
                ///   The last patent covering AC - 3 expired March 20, 2017, so nowadays this 
                ///   audio codec is free to use.
                /// </remarks>
                Ac3 = AV_CODEC_ID_AC3,

                /// <summary>
                ///   DTS Coherent Acoustics (DCA).
                /// </summary>
                Dts = AV_CODEC_ID_DTS,

                /// <summary>
                ///   Vorbis audio codec. Vorbis is a free and open-source software project 
                ///   headed by the Xiph.Org Foundation. The project produces an audio coding 
                ///   format and software reference encoder/decoder (codec) for lossy audio 
                ///   compression. Vorbis is most commonly used in conjunction with the Ogg 
                ///   container format and it is therefore often referred to as Ogg Vorbis.
                /// </summary>
                Vorbis = AV_CODEC_ID_VORBIS,

                /// <summary>
                ///   DV audio.
                /// </summary>
                DvAudio = AV_CODEC_ID_DVAUDIO,

                /// <summary>
                ///   Windows Media Audio 1.
                /// </summary>
                WmaV1 = AV_CODEC_ID_WMAV1,

                /// <summary>
                ///   Windows Media Audio 2.
                /// </summary>
                WmaV2 = AV_CODEC_ID_WMAV2,

                /// <summary>
                ///   MACE 3:1 codec.
                /// </summary>
                Mace3 = AV_CODEC_ID_MACE3,

                /// <summary>
                ///   MACE 6:1 codec.
                /// </summary>
                Mace6 = AV_CODEC_ID_MACE6,

                /// <summary>
                ///   Sierra VMD audio
                /// </summary>
                VmdAudio = AV_CODEC_ID_VMDAUDIO,

                /// <summary>
                /// FLAC (Free Lossless Audio Codec).
                /// </summary>
                Flac = AV_CODEC_ID_FLAC,

                /// <summary>
                /// MP3-ADU (Application Data Unit).
                /// </summary>
                Mp3Adu = AV_CODEC_ID_MP3ADU,

                /// <summary>
                ///   MP3 on MP4 codec.
                /// </summary>
                Mp3On4 = AV_CODEC_ID_MP3ON4,

                /// <summary>
                /// Shorten codec.
                /// </summary>
                Shorten = AV_CODEC_ID_SHORTEN,

                /// <summary>
                ///   Apple Lossless Audio Codec (ALAC)
                /// </summary>
                Alac = AV_CODEC_ID_ALAC,

                /// <summary>
                ///   Westwood Audio (SND1).
                /// </summary>
                WestwoodSnd1 = AV_CODEC_ID_WESTWOOD_SND1,

                /// <summary>
                /// GSM codec (as in <a href="http://www.quut.com/berlin/toast.html">TU Berlin's toast</a> tool.
                /// </summary>
                Gsm = AV_CODEC_ID_GSM,

                /// <summary>
                /// QDesign Music Codec.
                /// </summary>
                Qdm2 = AV_CODEC_ID_QDM2,

                /// <summary>
                /// RealNetworks's cook codec (also known as Cooker, Gecko, RealAudio G2, and RealAudio 8 low bitrate - RA8LBR).
                /// </summary>
                Cook = AV_CODEC_ID_COOK,

                /// <summary>
                ///   DSP Group's Truespeech. 
                /// </summary>
                Truespeech = AV_CODEC_ID_TRUESPEECH,

                /// <summary>
                ///   True Audio lossless audio codec.
                /// </summary>
                Tta = AV_CODEC_ID_TTA,

                /// <summary>
                /// RAD Game Tools' Smacker audio.
                /// </summary>
                SmackAudio = AV_CODEC_ID_SMACKAUDIO,

                /// <summary>
                ///   Qualcomm PureVoice.
                /// </summary>
                Qcelp = AV_CODEC_ID_QCELP,

                /// <summary>
                ///   WavPack lossless audio codec.
                /// </summary>
                WavPack = AV_CODEC_ID_WAVPACK,

                /// <summary>
                ///   Delphine Software International CIN audio.
                /// </summary>
                DsiCinAudio = AV_CODEC_ID_DSICINAUDIO,

                /// <summary>
                ///    Intel Music Coder (IMC).
                /// </summary>
                Imc = AV_CODEC_ID_IMC,

                /// <summary>
                ///   Musepack (MPC) v7.
                /// </summary>
                Musepack7 = AV_CODEC_ID_MUSEPACK7,

                /// <summary>
                ///   Meridian Lossless Packing (Packed PCM - PPCM).
                /// </summary>
                Mlp = AV_CODEC_ID_MLP,

                /// <summary>
                ///   GSM (Microsoft variant).
                /// </summary>
                GsmMs = AV_CODEC_ID_GSM_MS,

                /// <summary>
                ///   Sony's ATRAC3 codec.
                /// </summary>
                Atrac3 = AV_CODEC_ID_ATRAC3,

                /// <summary>
                ///   VoxWare MetaSound Audio.
                /// </summary>
                VoxWare = AV_CODEC_ID_VOXWARE,

                /// <summary>
                ///   Monkey's Audio lossless audio codec.
                /// </summary>
                Ape = AV_CODEC_ID_APE,

                /// <summary>
                ///   Nellymoser's codec.
                /// </summary>
                Nellymoser = AV_CODEC_ID_NELLYMOSER,

                /// <summary>
                ///   Musepack (MPC) v8.
                /// </summary>
                Musepack8 = AV_CODEC_ID_MUSEPACK8,

                /// <summary>
                ///   Speex Free Codec For Free Speech.
                /// </summary>
                Speex = AV_CODEC_ID_SPEEX,

                /// <summary>
                ///   Windows Media Audio Voice
                /// </summary>
                WmaVoice = AV_CODEC_ID_WMAVOICE,

                /// <summary>
                ///   Windows Media Audio Professional.
                /// </summary>
                WmaPro = AV_CODEC_ID_WMAPRO,

                /// <summary>
                ///   Windows Media Audio Lossless.
                /// </summary>
                WmaLossless = AV_CODEC_ID_WMALOSSLESS,

                /// <summary>
                ///   ATRAC3+ (Adaptive TRansform Acoustic Coding 3+).
                /// </summary>
                Atrac3P = AV_CODEC_ID_ATRAC3P,

                /// <summary>
                ///   Dolby Digital Plus (Enhanced AC-3).
                /// </summary>
                Eac3 = AV_CODEC_ID_EAC3,

                /// <summary>
                ///   RealNetworks' RealAudio sipr.
                /// </summary>
                Sipr = AV_CODEC_ID_SIPR,

                /// <summary>
                ///   MPEG-1 Audio Layer I.
                /// </summary>
                Mp1 = AV_CODEC_ID_MP1,

                /// <summary>
                ///   TwinVQ (transform-domain weighted interleave vector quantization).
                /// </summary>
                TwinVQ = AV_CODEC_ID_TWINVQ,

                /// <summary>
                ///   Dolby TrueHD lossless codec.
                /// </summary>
                TrueHD = AV_CODEC_ID_TRUEHD,

                /// <summary>
                ///   MPEG-4 Audio Lossless Coding (MPEG-4 ALS)
                /// </summary>
                Mp4Als = AV_CODEC_ID_MP4ALS,

                /// <summary>
                ///   Adaptive Transform Acoustic Coding.
                /// </summary>
                Atrac1 = AV_CODEC_ID_ATRAC1,

                /// <summary>
                ///   Bink Audio (RDFT)
                /// </summary>
                BinkAudioRdft = AV_CODEC_ID_BINKAUDIO_RDFT,

                /// <summary>
                ///   Bink Audio (DCT)
                /// </summary>
                BinkAudioDct = AV_CODEC_ID_BINKAUDIO_DCT,

                /// <summary>
                ///   AAC (Advanced Audio Coding) LATM (Low Overhead Audio Transport Multiplex).
                /// </summary>
                AacLatm = AV_CODEC_ID_AAC_LATM,

                /// <summary>
                ///   QDesign Music Codec.
                /// </summary>
                Qdmc = AV_CODEC_ID_QDMC,

                /// <summary>
                ///   Constrained Energy Lapped Transform (CELT).
                /// </summary>
                Celt = AV_CODEC_ID_CELT,

                /// <summary>
                ///   G.723.1 codec.
                /// </summary>
                G7231 = AV_CODEC_ID_G723_1,

                /// <summary>
                ///   G.729 codec.
                /// </summary>
                G729 = AV_CODEC_ID_G729,

                /// <summary>
                ///   8-Bit Sampled Voice (8SVX) in Exponential mode.
                /// </summary>
                EightSvxExp = AV_CODEC_ID_8SVX_EXP,

                /// <summary>
                ///   8-Bit Sampled Voice (8SVX) in Fibonacci mode.
                /// </summary>
                EightSvxFib = AV_CODEC_ID_8SVX_FIB,

                /// <summary>
                ///   BMV Audio.
                /// </summary>
                BmvAudio = AV_CODEC_ID_BMV_AUDIO,

                /// <summary>
                /// RealNetworks' Real Lossless Codec.
                /// </summary>
                Ralf = AV_CODEC_ID_RALF,

                /// <summary>
                ///   Indeo Audio Codec.
                /// </summary>
                Iac = AV_CODEC_ID_IAC,

                /// <summary>
                ///   Internet Low Bitrate Codec.
                /// </summary>
                Ilbc = AV_CODEC_ID_ILBC,

                /// <summary>
                ///   Opus lossy audio codec.
                /// </summary>
                Opus = AV_CODEC_ID_OPUS,

                /// <summary>
                ///   RFC 3389 comfort noise codec.
                /// </summary>
                ComfortNoise = AV_CODEC_ID_COMFORT_NOISE,

                /// <summary>
                ///   Tom's lossless Audio Kompressor.
                /// </summary>
                Tak = AV_CODEC_ID_TAK,

                /// <summary>
                ///   VoxWare MetaSound Audio.
                /// </summary>
                MetaSound = AV_CODEC_ID_METASOUND,

                /// <summary>
                ///   Amazing Studio Packed Animation File Audio.
                /// </summary>
                PafAudio = AV_CODEC_ID_PAF_AUDIO,

                /// <summary>
                ///   On2's Audio for Video Codec (AVC).
                /// </summary>
                On2Avc = AV_CODEC_ID_ON2AVC,

                /// <summary>
                ///   Digital Speech Standard / Standard Play (DSS-SP).
                /// </summary>
                DsSsp = AV_CODEC_ID_DSS_SP,

                /// <summary>
                ///   Wave synthesis pseudo-codec.
                /// </summary>
                FfWaveSynth = AV_CODEC_ID_FFWAVESYNTH,

                /// <summary>
                /// Sonic.
                /// </summary>
                Sonic = AV_CODEC_ID_SONIC,

                /// <summary>
                ///   Sonic lossless.
                /// </summary>
                SonicLossless = AV_CODEC_ID_SONIC_LS,

                /// <summary>
                ///   Enhanced Variable Rate Codec
                /// </summary>
                Evrc = AV_CODEC_ID_EVRC,

                /// <summary>
                ///   Selectable Mode Vocoder.
                /// </summary>
                Smv = AV_CODEC_ID_SMV,

                /// <summary>
                ///   DSD (Direct Stream Digital), least significant bit first.
                /// </summary>
                DsdLsbf = AV_CODEC_ID_DSD_LSBF,

                /// <summary>
                ///   DSD (Direct Stream Digital), most significant bit first.
                /// </summary>
                DsdMsbf = AV_CODEC_ID_DSD_MSBF,

                /// <summary>
                ///   DSD (Direct Stream Digital), least significant bit first, planar.
                /// </summary>
                DsdLsbfPlanar = AV_CODEC_ID_DSD_LSBF_PLANAR,

                /// <summary>
                ///   DSD (Direct Stream Digital), most significant bit first, planar.
                /// </summary>
                DsdMsbfPlanar = AV_CODEC_ID_DSD_MSBF_PLANAR,

                /// <summary>
                ///   Qualcomm's 4G voice speech codecs used by CDMA networks.
                /// </summary>
                FourGV = AV_CODEC_ID_4GV,

                /// <summary>
                /// Interplay's ACM codec.
                /// </summary>
                InterplayAcm = AV_CODEC_ID_INTERPLAY_ACM,

                /// <summary>
                ///   Xbox Media Audio 1.
                /// </summary>
                Xma1 = AV_CODEC_ID_XMA1,

                /// <summary>
                ///   Xbox Media Audio 2.
                /// </summary>
                Xma2 = AV_CODEC_ID_XMA2,

                /// <summary>
                ///   Direct Stream Transfer (DST).
                /// </summary>
                Dst = AV_CODEC_ID_DST,




#pragma region backward compatibility
                /// <summary>
                ///   Obsolete. Please use <see cref="Mp3" /> instead.
                /// </summary>
                ///
                [Obsolete("Please use Mp3 instead.")]
                MP3 = 0,

                /// <summary>
                ///   Obsolete. Please use <see cref="Aac" /> instead.
                /// </summary>
                ///
                [Obsolete("Please use Aac instead.")]
                AAC = 1,

                /// <summary>
                ///   Obsolete. Please use <see cref="Mp4Als" /> instead.
                /// </summary>
                ///
                [Obsolete("Please use Mp4Als instead.")]
                M4A = 2,

                /// <summary>
                ///   Obsolete. Please use <see cref="Mp4Als" /> instead.
                /// </summary>
                ///
                [Obsolete("Please use Mp4Als instead.")]
                MP4ALS = 2,
#pragma endregion
            };
        }
    }
}
