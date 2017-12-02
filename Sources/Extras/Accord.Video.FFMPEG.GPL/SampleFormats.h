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
            ///   Enumeration of some audio sample formats from FFmpeg library, which are 
            ///   available for writing audio in video or audio files.
            /// </summary>
            ///
            public enum class AVSampleFormat
            {
                /// <summary>
                ///   No sample format.
                /// </summary>
                None = AV_SAMPLE_FMT_NONE,

                /// <summary>
                ///  Unsigned 8 bits.
                /// </summary>
                Format8bitUnsigned = AV_SAMPLE_FMT_U8,

                /// <summary>
                ///  Signed 16 bits.
                /// </summary>
                Format16bitSigned = AV_SAMPLE_FMT_S16,

                /// <summary>
                ///  Signed 32 bits.
                /// </summary>
                Format32bitSigned = AV_SAMPLE_FMT_S32,

                /// <summary>
                ///  Signed 64 bits.
                /// </summary>
                Format64bitSigned = AV_SAMPLE_FMT_S64,

                /// <summary>
                ///  Float.
                /// </summary>
                Format32bitFloat = AV_SAMPLE_FMT_FLT,

                /// <summary>
                ///  Double.
                /// </summary>
                Format64bitDouble = AV_SAMPLE_FMT_DBL,

                /// <summary>
                ///  Unsigned 8 bits, planar.
                /// </summary>
                Format8bitUnsignedPlanar = AV_SAMPLE_FMT_U8P,

                /// <summary>
                ///  Signed 16 bits, planar.
                /// </summary>
                Format8bitSignedPlanar = AV_SAMPLE_FMT_S16P,

                /// <summary>
                ///  Signed 32 bits, planar.
                /// </summary>
                Format32bitSignedPlanar = AV_SAMPLE_FMT_S32P,

                /// <summary>
                ///  Signed 64 bits, planar.
                /// </summary>
                Format64bitSignedPlanar = AV_SAMPLE_FMT_S64P,

                /// <summary>
                ///  Float, planar.
                /// </summary>
                Format32bitFloatPlanar = AV_SAMPLE_FMT_FLTP,

                /// <summary>
                ///  Double, planar.
                /// </summary>
                Format64bitDoublePlanar = AV_SAMPLE_FMT_DBLP,
            };
        }
    }
}
