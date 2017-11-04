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
            public enum class AudioLayout : unsigned long long
            {                
                /// <summary>
                ///   Front left speaker.
                /// </summary>
                FrontLeft = AV_CH_FRONT_LEFT,

                /// <summary>
                ///   Front right speaker.
                /// </summary>
                FrontRight = AV_CH_FRONT_RIGHT,

                /// <summary>
                ///   Front center speaker.
                /// </summary>
                FrontCenter = AV_CH_FRONT_CENTER,
                
                /// <summary>
                ///   Low-frequency speaker (i.e. sub-woofer).
                /// </summary>
                LowFrequency = AV_CH_LOW_FREQUENCY,

                /// <summary>
                ///   Back left speaker.
                /// </summary>
                BackLeft = AV_CH_BACK_LEFT,

                /// <summary>
                ///   Back right speaker.
                /// </summary>
                BackRight = AV_CH_BACK_RIGHT,

                /// <summary>
                ///   Front-left of center speaker.
                /// </summary>
                FrontLeftOfCenter = AV_CH_FRONT_LEFT_OF_CENTER,
                
                /// <summary>
                ///   Front right of center speaker.
                /// </summary>
                FrontRightOfCenter = AV_CH_FRONT_RIGHT_OF_CENTER,

                /// <summary>
                ///   Back center speaker.
                /// </summary>
                BackCenter = AV_CH_BACK_CENTER,

                /// <summary>
                ///   Side left speaker.
                /// </summary>
                SideLeft = AV_CH_SIDE_LEFT,

                /// <summary>
                ///   Side right speaker.
                /// </summary>
                SideRight = AV_CH_SIDE_RIGHT,

                /// <summary>
                ///   Top center speaker.
                /// </summary>
                TopCenter = AV_CH_TOP_CENTER,

                /// <summary>
                ///   Top front left speaker.
                /// </summary>
                TopFrontLeft = AV_CH_TOP_FRONT_LEFT,

                /// <summary>
                ///   Top front center speaker.
                /// </summary>
                TopFrontCenter = AV_CH_TOP_FRONT_CENTER,

                /// <summary>
                ///   Top front right speaker.
                /// </summary>
                TopFrontRight = AV_CH_TOP_FRONT_RIGHT,

                /// <summary>
                ///   Top back left speaker.
                /// </summary>
                TopBackLeft = AV_CH_TOP_BACK_LEFT,

                /// <summary>
                ///   Top back center speaker.
                /// </summary>
                TopBackCenter = AV_CH_TOP_BACK_CENTER,

                /// <summary>
                ///   Top back right speaker.
                /// </summary>
                TopBackRight = AV_CH_TOP_BACK_RIGHT,

                /// <summary>
                ///   Stereo left downmix (for when reducing more than two channels into simple stereo).
                /// </summary>
                StereoLeft = AV_CH_STEREO_LEFT,  

                /// <summary>
                ///   Stereo right downmix (for when reducing more than two channels into simple stereo).
                /// </summary>
                StereoRight = AV_CH_STEREO_RIGHT,
                    
                /// <summary>
                ///   Wide left.
                /// </summary>
                WideLeft = AV_CH_WIDE_LEFT,

                /// <summary>
                ///   Wide right.
                /// </summary>
                WideRight = AV_CH_WIDE_RIGHT,

                /// <summary>
                ///   Surround direct left.
                /// </summary>
                SurroundDirectLeft = AV_CH_SURROUND_DIRECT_LEFT,

                /// <summary>
                ///   Surround direct right.
                /// </summary>
                SurroundDirectRight = AV_CH_SURROUND_DIRECT_RIGHT,

                /// <summary>
                ///   Low-frequency speaker 2 (i.e. sub-woofer).
                /// </summary>
                LowFrequency2 = AV_CH_LOW_FREQUENCY_2,

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
                Layout21 = (Stereo | BackCenter),

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
                Layout22 = (Stereo | SideLeft | SideRight),

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
                Layout6Point0Front = (Layout22 | FrontLeftOfCenter | FrontRightOfCenter),

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
