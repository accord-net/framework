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
            ///   Enumeration of some channel layouts codecs from FFmpeg library, 
            ///   which are available for writing video and audio files.
            /// </summary>
            ///
            [System::Flags]
            public enum class AudioLayout
            {                
                /// <summary>
                ///   Front left speaker.
                /// </summary>
                FrontLeft = 0x00000001,

                /// <summary>
                ///   Front right speaker.
                /// </summary>
                FrontRight = 0x00000002,

                /// <summary>
                ///   Front center speaker.
                /// </summary>
                FrontCenter = 0x00000004,
                
                /// <summary>
                ///   Low-frequency speaker (i.e. sub-woofer).
                /// </summary>
                LowFrequency = 0x00000008,

                /// <summary>
                ///   Back left speaker.
                /// </summary>
                BackLeft = 0x00000010,

                /// <summary>
                ///   Back right speaker.
                /// </summary>
                BackRight = 0x00000020,

                /// <summary>
                ///   Front-left of center speaker.
                /// </summary>
                FrontLeftOfCenter = 0x00000040,
                
                /// <summary>
                ///   Front right of center speaker.
                /// </summary>
                FrontRightOfCenter = 0x00000080,

                /// <summary>
                ///   Back center speaker.
                /// </summary>
                BackCenter = 0x00000100,

                /// <summary>
                ///   Side left speaker.
                /// </summary>
                SideLeft = 0x00000200,

                /// <summary>
                ///   Side right speaker.
                /// </summary>
                SideRight = 0x00000400,

                /// <summary>
                ///   Top center speaker.
                /// </summary>
                TopCenter = 0x00000800,

                /// <summary>
                ///   Top front left speaker.
                /// </summary>
                TopFrontLeft = 0x00001000,

                /// <summary>
                ///   Top front center speaker.
                /// </summary>
                TopFrontCenter = 0x00002000,

                /// <summary>
                ///   Top front right speaker.
                /// </summary>
                TopFrontRight = 0x00004000,

                /// <summary>
                ///   Top back left speaker.
                /// </summary>
                TopBackLeft = 0x00008000,

                /// <summary>
                ///   Top back center speaker.
                /// </summary>
                TopBackCenter = 0x00010000,

                /// <summary>
                ///   Top back right speaker.
                /// </summary>
                TopBackRight = 0x00020000,

                /// <summary>
                ///   Stereo left downmix (for when reducing more than two channels into simple stereo).
                /// </summary>
                StereoLeft = 0x20000000,  

                /// <summary>
                ///   Stereo right downmix (for when reducing more than two channels into simple stereo).
                /// </summary>
                StereoRight = 0x40000000, 
                    
                /// <summary>
                ///   Wide left.
                /// </summary>
                WideLeft = 0x0000000080000000ULL,

                /// <summary>
                ///   Wide right.
                /// </summary>
                WideRight = 0x0000000100000000ULL,

                /// <summary>
                ///   Surround direct left.
                /// </summary>
                Surround_DIRECT_Left = 0x0000000200000000ULL,

                /// <summary>
                ///   Surround direct right.
                /// </summary>
                Surround_DIRECT_Right = 0x0000000400000000ULL,

                /// <summary>
                ///   Low-frequency speaker 2 (i.e. sub-woofer).
                /// </summary>
                LowFrequency_2 = 0x0000000800000000ULL,

                /// <summary>
                ///   One speaker will be used (<see cref="FrontCenter"/> speaker).
                /// </summary>
                Mono = (FrontCenter),

                /// <summary>
                ///   Two speakers will be used (<see cref="FrontLeft"/> and <see cref="FrontRight"/> speakers).
                /// </summary>
                Stereo = (FrontLeft | FrontRight),
                    
                /// <summary>
                ///   Predefined 2.1 layout (<see cref="FrontLeft"/>, <see cref="FrontRight"/>, 
                ///   and <see cref="LowFrequency"/> (sub-woofer)).
                /// </summary>
                Layout2Point1 = (Stereo | LowFrequency),

                /// <summary>
                ///   Predefined stereo layout (<see cref="FrontLeft"/>, <see cref="FrontRight"/>)
                ///   using also <see cref="BackCenter"/>.
                /// </summary>
                Layout2_1 = (Stereo | BackCenter),

                /// <summary>
                ///   Predefined surround layout (<see cref="FrontLeft"/>, <see cref="FrontRight"/>,
                ///   and <see cref="FrontCenter"/>).
                /// </summary>
                LayoutSurround = (Stereo | FrontCenter),

                /// <summary>
                ///   Predefined 3.1 layout (<see cref="FrontLeft"/>, <see cref="FrontRight"/>,
                ///   <see cref="FrontCenter"/>, and <see cref="LowFrequency"/> (sub-woofer)).
                /// </summary>
                Layout3Point1 = (LayoutSurround | LowFrequency),

                /// <summary>
                ///   Predefined 3.0 layout (<see cref="FrontLeft"/>, <see cref="FrontRight"/>, 
                ///   <see cref="FrontCenter"/>, and <see cref="BackCenter"/>).
                /// </summary>
                Layout4Point0 = (LayoutSurround | BackCenter),

                /// <summary>
                ///   Predefined 4.1 layout (<see cref="FrontLeft"/>, <see cref="FrontRight"/>, 
                ///   <see cref="FrontCenter"/>, <see cref="BackCenter"/>, and <see cref="LowFrequency"/> (sub-woofer)).
                /// </summary>
                Layout4Point1 = (Layout4Point0 | LowFrequency),
                    
                /// <summary>
                ///   Predefined stereo layout (using also <see cref="SideLeft"/> and <see cref="SideRight"/>).
                /// </summary>
                Layout2_2 = (Stereo | SideLeft | SideRight),

                /// <summary>
                ///   Predefined stereo layout (using also <see cref="SideLeft"/>, <see cref="SideRight"/>,
                ///   <see cref="BackLeft"/> and <see cref="BackRight"/>).
                /// </summary>
                LayoutQuad = (Stereo | BackLeft | BackRight),

                /// <summary>
                ///   Predefined 5.0 layout.
                /// </summary>
                Layout5Point0 = (LayoutSurround | SideLeft | SideRight),

                /// <summary>
                ///   Predefined 5.1 layout.
                /// </summary>
                Layout5Point1 = (Layout5Point0 | LowFrequency),

                /// <summary>
                ///   Predefined 5.0 layout with left and right back speakers.
                /// </summary>
                Layout5Point0Back = (LayoutSurround | BackLeft | BackRight),

                /// <summary>
                ///   Predefined 5.1 layout with back speakers.
                /// </summary>
                Layout5Point1Back = (Layout5Point0Back | LowFrequency),

                /// <summary>
                ///   Predefined 6.0 layout.
                /// </summary>
                Layout6Point0 = (Layout5Point0 | BackCenter),

                /// <summary>
                ///   Predefined 6.0 layout with front speakers.
                /// </summary>
                Layout6Point0Front = (Layout2_2 | FrontLeftOfCenter | FrontRightOfCenter),

                /// <summary>
                ///   Predefined 5.0 layout with left, right, and center back speakers.
                /// </summary>
                LayoutHexagonal = (Layout5Point0Back | BackCenter),

                /// <summary>
                ///   Predefined 6.1 layout.
                /// </summary>
                Layout6Point1 = (Layout5Point1 | BackCenter),

                /// <summary>
                ///   Predefined 6.1 layout using back center speaker.
                /// </summary>
                Layout6Point1Back = (Layout5Point1Back | BackCenter),

                /// <summary>
                ///   Predefined 6.1 layout using left, right, and center front speakers.
                /// </summary>
                Layout6Point1Front = (Layout6Point0Front | LowFrequency),

                /// <summary>
                ///   Predefined 7.0 layout using left and right back speakers.
                /// </summary>
                Layout7Point0 = (Layout5Point0 | BackLeft | BackRight),

                /// <summary>
                ///   Predefined 7.0 layout using left and right front center speakers.
                /// </summary>
                Layout7Point0Front = (Layout5Point0 | FrontLeftOfCenter | FrontRightOfCenter),

                /// <summary>
                ///   Predefined 7.1 layout using left and right back speakers.
                /// </summary>
                Layout7Point1 = (Layout5Point1 | BackLeft | BackRight),

                /// <summary>
                ///   Predefined 7.1 layout using left and right front center speakers.
                /// </summary>
                Layout7Point1Wide = (Layout5Point1 | FrontLeftOfCenter | FrontRightOfCenter),

                /// <summary>
                ///   Predefined 7.1 layout using left and right front center speakers, and side speakers.
                /// </summary>
                Layout7Point1WideBack = (Layout5Point1Back | FrontLeftOfCenter | FrontRightOfCenter),
                    
                /// <summary>
                ///   Predefined octagonal layout.
                /// </summary>
                LayoutOctagonal = (Layout5Point0 | BackLeft | BackCenter | BackRight),

                /// <summary>
                ///   Predefined hexagonal layout.
                /// </summary>
                LayoutHexadecagonal = (LayoutOctagonal | WideLeft | WideRight | TopBackLeft | TopBackRight | TopBackCenter | TopFrontCenter | TopFrontLeft | TopFrontRight),

                /// <summary>
                ///   Predefined stereo downmix layout.
                /// </summary>
                LayoutStereoDownMix = (StereoLeft | StereoRight)
            };
        }
    }
}
