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

#include <string>
#include "PixelFormats.h"
#include "SampleFormats.h"


struct AVStream;
struct AVCodec;
struct AVRational;
struct AVCodecContext;

using namespace System;

namespace Accord {
    namespace Video {
        namespace FFMPEG
        {
            int64_t frameToPTS(AVStream* stream, int64_t frame);

            int64_t ptsToFrame(AVStream* stream, int64_t pts);

            int check_sample_fmt(AVCodec* codec, enum ::AVSampleFormat& sample_fmt);

            int check_sample_rate(AVCodec* codec, int sampleRate);

            void CHECK(int errorCode, String^ message);

            template<class T>
            T* CHECK(T* obj, String^ message)
            {
                if (obj == nullptr)
                    throw gcnew VideoException(String::Format("{0}. See console output for more details.", message));
                return obj;
            }

            String^ str(const char* str);

            String^ str(int64_t ts, AVRational* tb);

            const char* str2native(System::String^ str, char* native);


            int64_t TimeSpanToPTS(TimeSpan& duration, AVStream* stream, AVCodecContext* codec);

            TimeSpan PTSToTimeSpan(uint64_t pts, AVStream* stream, AVCodecContext* codec);

            enum ::AVSampleFormat s2f(const Accord::Audio::SampleFormat& format);

            enum ::AVPixelFormat p2f(const System::Drawing::Imaging::PixelFormat& format);

            void check_redistributable();
        }
    }
}
