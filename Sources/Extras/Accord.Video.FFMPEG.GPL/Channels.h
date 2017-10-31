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

namespace Accord {
    namespace Video {
        namespace FFMPEG
        {
            /// <summary>
            ///   Enumeration of some channel layouts codecs from FFmpeg library, which are available for writing audio files.
            /// </summary>
            ///
            public enum class Channels
            {
                FrontLeft = 0x00000001,
                FrontRight = 0x00000002,
                FrontCenter = 0x00000004,
                /*LOW_FREQUENCY = 0x00000008,
                BACK_LEFT = 0x00000010,
                BACK_RIGHT = 0x00000020,
                FRONT_LEFT_OF_CENTER = 0x00000040,
                FRONT_RIGHT_OF_CENTER = 0x00000080,
                BACK_CENTER = 0x00000100,
                SIDE_LEFT = 0x00000200,
                SIDE_RIGHT = 0x00000400,
                TOP_CENTER = 0x00000800,
                TOP_FRONT_LEFT = 0x00001000,
                TOP_FRONT_CENTER = 0x00002000,
                TOP_FRONT_RIGHT = 0x00004000,
                TOP_BACK_LEFT = 0x00008000,
                TOP_BACK_CENTER = 0x00010000,
                TOP_BACK_RIGHT = 0x00020000,
                STEREO_LEFT = 0x20000000,  ///< Stereo downmix.
                STEREO_RIGHT = 0x40000000,  ///< See STEREO_LEFT.
                WIDE_LEFT = 0x0000000080000000ULL,
                WIDE_RIGHT = 0x0000000100000000ULL,
                SURROUND_DIRECT_LEFT = 0x0000000200000000ULL,
                SURROUND_DIRECT_RIGHT = 0x0000000400000000ULL,
                LOW_FREQUENCY_2 = 0x0000000800000000ULL,*/
                Mono = (FrontCenter),
                Stereo = (FrontLeft | FrontRight),
                /*LAYOUT_2POINT1 = (Stereo | LOW_FREQUENCY),
                LAYOUT_2_1 = (Stereo | BACK_CENTER),
                LAYOUT_SURROUND = (Stereo | FrontCenter),
                LAYOUT_3POINT1 = (LAYOUT_SURROUND | LOW_FREQUENCY),
                LAYOUT_4POINT0 = (LAYOUT_SURROUND | BACK_CENTER),
                LAYOUT_4POINT1 = (LAYOUT_4POINT0 | LOW_FREQUENCY),
                LAYOUT_2_2 = (Stereo | SIDE_LEFT | SIDE_RIGHT),
                LAYOUT_QUAD = (Stereo | BACK_LEFT | BACK_RIGHT),
                LAYOUT_5POINT0 = (LAYOUT_SURROUND | SIDE_LEFT | SIDE_RIGHT),
                LAYOUT_5POINT1 = (LAYOUT_5POINT0 | LOW_FREQUENCY),
                LAYOUT_5POINT0_BACK = (LAYOUT_SURROUND | BACK_LEFT | BACK_RIGHT),
                LAYOUT_5POINT1_BACK = (LAYOUT_5POINT0_BACK | LOW_FREQUENCY),
                LAYOUT_6POINT0 = (LAYOUT_5POINT0 | BACK_CENTER),
                LAYOUT_6POINT0_FRONT = (LAYOUT_2_2 | FRONT_LEFT_OF_CENTER | FRONT_RIGHT_OF_CENTER),
                LAYOUT_HEXAGONAL = (LAYOUT_5POINT0_BACK | BACK_CENTER),
                LAYOUT_6POINT1 = (LAYOUT_5POINT1 | BACK_CENTER),
                LAYOUT_6POINT1_BACK = (LAYOUT_5POINT1_BACK | BACK_CENTER),
                LAYOUT_6POINT1_FRONT = (LAYOUT_6POINT0_FRONT | LOW_FREQUENCY),
                LAYOUT_7POINT0 = (LAYOUT_5POINT0 | BACK_LEFT | BACK_RIGHT),
                LAYOUT_7POINT0_FRONT = (LAYOUT_5POINT0 | FRONT_LEFT_OF_CENTER | FRONT_RIGHT_OF_CENTER),
                LAYOUT_7POINT1 = (LAYOUT_5POINT1 | BACK_LEFT | BACK_RIGHT),
                LAYOUT_7POINT1_WIDE = (LAYOUT_5POINT1 | FRONT_LEFT_OF_CENTER | FRONT_RIGHT_OF_CENTER),
                LAYOUT_7POINT1_WIDE_BACK = (LAYOUT_5POINT1_BACK | FRONT_LEFT_OF_CENTER | FRONT_RIGHT_OF_CENTER),
                LAYOUT_OCTAGONAL = (LAYOUT_5POINT0 | BACK_LEFT | BACK_CENTER | BACK_RIGHT),
                LAYOUT_HEXADECAGONAL = (LAYOUT_OCTAGONAL | WIDE_LEFT | WIDE_RIGHT | TOP_BACK_LEFT | TOP_BACK_RIGHT | TOP_BACK_CENTER | TOP_FRONT_CENTER | TOP_FRONT_LEFT | TOP_FRONT_RIGHT),
                LAYOUT_STEREO_DOWNMIX = (STEREO_LEFT | STEREO_RIGHT),*/
            };
        }
    }
}
