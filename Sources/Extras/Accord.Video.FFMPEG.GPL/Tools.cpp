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

#include "StdAfx.h"
#include "Tools.h"

#include <string>

using namespace System;
using namespace System::IO;
using namespace Accord::Audio;
using namespace Accord::Video;
using namespace Accord::Compat;

namespace Accord {
    namespace Video {
        namespace FFMPEG
        {
            int64_t frameToPTS(AVStream* stream, int64_t frame)
            {
                return (int64_t(frame) * stream->r_frame_rate.den * stream->time_base.den)
                    / (int64_t(stream->r_frame_rate.num) * stream->time_base.num);
            }

            int64_t ptsToFrame(AVStream* stream, int64_t pts)
            {
                return (int64_t(pts) * stream->time_base.num *  stream->codec->time_base.num) /
                    (int64_t(stream->time_base.den) * stream->codec->time_base.den);
            }

            /* check that a given sample format is supported by the encoder */
            static int check_sample_fmt(AVCodec* codec, enum ::AVSampleFormat& sample_fmt)
            {
                const enum ::AVSampleFormat* p = codec->sample_fmts;
                while (*p != AV_SAMPLE_FMT_NONE)
                {
                    if (*p == sample_fmt)
                        return 1;
                    p++;
                }
                return 0;
            }

            /* check that a given sample rate is supported by the encoder */
            static int check_sample_rate(AVCodec *codec, int sampleRate)
            {
                const int* p = codec->supported_samplerates;

                if (p == 0)
                    return 1;

                while (*p) {
                    if (*p == sampleRate)
                        return 1;
                    p++;
                }
                return 0;
            }

            String^ str(const char* str)
            {
                return gcnew System::String(str);
            }

            String^ str(int64_t ts, AVRational* tb)
            {
                if (ts == AV_NOPTS_VALUE)
                    return "NOPTS";
                return String::Format("{0}", av_q2d(*tb) * ts);
            }

            void CHECK(int errorCode, String^ message)
            {
                if (errorCode < 0)
                {
                    // TODO: Use av_err2str
                    char buff[AV_ERROR_MAX_STRING_SIZE];
                    av_make_error_string(&buff[0], AV_ERROR_MAX_STRING_SIZE, errorCode);
                    System::String^ ffmpegErrorMessage = System::Runtime::InteropServices::Marshal::PtrToStringAnsi((IntPtr)&buff[0]);
                    throw gcnew VideoException(String::Format("{0}. Error {1}: {2}. See console output for more details.",
                        message, errorCode, ffmpegErrorMessage));
                }
            }

            const char* str2native(String^ str, char* native)
            {
                if (str == nullptr)
                    return nullptr;

                IntPtr ptr = System::Runtime::InteropServices::Marshal::StringToHGlobalUni(str);
                wchar_t* nativeFileNameUnicode = (wchar_t*)ptr.ToPointer();
                int utf8StringSize = WideCharToMultiByte(CP_UTF8, 0, nativeFileNameUnicode, -1, nullptr, 0, nullptr, nullptr);
                WideCharToMultiByte(CP_UTF8, 0, nativeFileNameUnicode, -1, native, utf8StringSize, nullptr, nullptr);
                return native;
            }


            int64_t TimeSpanToPTS(TimeSpan& duration, AVStream* stream, AVCodecContext* codec)
            {
                uint64_t hundredNanoseconds = duration.Ticks;
                AVRational tickRate = { 1, 10000000 };
                AVRational streamRatio = stream->time_base;
                AVRational codecRatio = codec->time_base;
                int64_t streamPos = av_rescale_q_rnd(hundredNanoseconds, tickRate, streamRatio, AVRounding::AV_ROUND_UP);
                int64_t result = av_rescale_q_rnd(streamPos, streamRatio, codecRatio, AVRounding::AV_ROUND_UP);
                return result;
            }

            TimeSpan PTSToTimeSpan(uint64_t pts, AVStream* stream, AVCodecContext* codec)
            {
                if (pts == AV_NOPTS_VALUE)
                    pts = 0;

                AVRational codecRatio = codec->time_base;
                AVRational streamRatio = stream->time_base;
                AVRational tickRate = { 1, 10000000 };

                int64_t codecPos = av_rescale_q_rnd(pts, codecRatio, streamRatio, AVRounding::AV_ROUND_DOWN);
                uint64_t  hundredNanoseconds = av_rescale_q_rnd(codecPos, streamRatio, tickRate, AVRounding::AV_ROUND_DOWN);

                return TimeSpan::FromTicks(hundredNanoseconds);
            }


            enum ::AVSampleFormat s2f(const Accord::Audio::SampleFormat& format)
            {
                switch (format)
                {
                case Accord::Audio::SampleFormat::Format16Bit:
                    return AV_SAMPLE_FMT_S16;
                case Accord::Audio::SampleFormat::Format32Bit:
                    return AV_SAMPLE_FMT_S32;
                case Accord::Audio::SampleFormat::Format32BitIeeeFloat:
                    return AV_SAMPLE_FMT_FLT;
                }

                throw gcnew ArgumentOutOfRangeException("format", "Invalid audio format.");
            }

            enum ::AVPixelFormat p2f(const System::Drawing::Imaging::PixelFormat& format)
            {
                switch (format)
                {
                case System::Drawing::Imaging::PixelFormat::Format8bppIndexed:
                    return AV_PIX_FMT_GRAY8;
                case System::Drawing::Imaging::PixelFormat::Format24bppRgb:
                    return AV_PIX_FMT_BGR24;
                case System::Drawing::Imaging::PixelFormat::Format32bppArgb:
                    return AV_PIX_FMT_BGRA;
                }

                throw gcnew ArgumentOutOfRangeException("format", "Invalid image format.");
            }


            void check_redistributable()
            {
                static bool checked = false;

                if (checked)
                    return;
#if NET35
                bool is64bits = Accord::Compat::EnvironmentEx::Is64BitProcess;
                DirectoryInfo^ windowPath = Directory::GetParent(Environment::GetFolderPath(Environment::SpecialFolder::System));

                if (is64bits)
                {
                    throw gcnew InvalidOperationException("This application cannot be run in 64-bits.");
                }
                else
                {
                    bool success = windowPath->GetDirectories("WinSxS\\x86_Microsoft.VC90.CRT*")->Length > 0;

                    if (!success)
                    {
                        throw gcnew InvalidOperationException("This application requires the x86 version of " +
                            "the Microsoft Visual C++ 2008 SP1 Redistributable Package to be installed on this computer. Please " +
                            "download and install it from https://www.microsoft.com/en-us/download/details.aspx?id=5582");
                    }
                }
#else
                String^ system32Path = Environment::GetFolderPath(Environment::SpecialFolder::SystemX86);
                String^ system64Path = Environment::GetFolderPath(Environment::SpecialFolder::System);
                bool is64bits = Environment::Is64BitProcess;

                String^ msvcDllName = "msvcp140.dll";

                bool success = File::Exists(Path::Combine(system32Path, msvcDllName));

                if (is64bits)
                {
                    success &= File::Exists(Path::Combine(system64Path, msvcDllName));
                }

                if (!success)
                {
                    if (is64bits)
                    {
                        throw gcnew InvalidOperationException("This application requires both the x64 and x86 versions of " +
                            "the Visual C++ Redistributable for Visual Studio 2015 to be installed on this computer. Please " +
                            "download and install them from https://www.microsoft.com/en-us/download/details.aspx?id=48145");
                    }
                    else
                    {
                        throw gcnew InvalidOperationException("This application requires the x86 version of " +
                            "the Visual C++ Redistributable for Visual Studio 2015 to be installed on this computer. Please " +
                            "download and install it from https://www.microsoft.com/en-us/download/details.aspx?id=48145");
                    }
                }
#endif

                checked = true;
            }

        }


    }
}
