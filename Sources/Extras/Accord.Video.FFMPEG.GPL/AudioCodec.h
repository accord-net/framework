// Accord FFMPEG Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © AForge.NET, 2009-2011
// contacts@aforgenet.com
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

extern int audio_codecs[];
extern int AUDIO_CODECS_COUNT;

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
                /// No audio
                /// </summary>
                ///
                None = -1,

                /// <summary>
                /// MPEG-3
                /// </summary>
                ///
                MP3 = 0,

                /// <summary>
                /// AAC
                /// </summary>
                ///
                AAC = 1,

                /// <summary>
                ///   Please use <see href="MP4ALS" /> instead.
                /// </summary>
                ///
                [Obsolete("Please use MP4ALS instead.")]
                M4A = 2,

                /// <summary>
                /// MPEG-4 Audio Lossless Coding, also known as MPEG-4 ALS.
                /// </summary>
                ///
                MP4ALS = 2,
            };
        }
    }
}
