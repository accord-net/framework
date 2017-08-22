// Accord FFMPEG Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © AForge.NET, 2009-2011
// contacts@aforgenet.com
//
// Copyright © MelvinGr, 2016-2017
// https://github.com/MelvinGr
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

extern int video_codecs[];
extern int pixel_formats[];
extern int CODECS_COUNT;

namespace Accord {
    namespace Video {
        namespace FFMPEG
        {
            /// <summary>
            ///   Enumeration of some video codecs from FFmpeg library, which are available for writing video files.
            /// </summary>
            ///
            public enum class VideoCodec
            {
                /// <summary>
                ///   Default video codec, which FFmpeg library selects for the specified file format.
                /// </summary>
                ///
                Default = -1,

                /// <summary>
                ///   MPEG-4 part 2.
                /// </summary>
                ///
                MPEG4 = 0,

                /// <summary>
                ///   Windows Media Video 7.
                /// </summary>
                ///
                WMV1,

                /// <summary>
                ///   Windows Media Video 8.
                /// </summary>
                ///
                WMV2,

                /// <summary>
                /// MPEG-4 part 2 Microsoft variant version 2.
                /// </summary>
                ///
                MSMPEG4v2,

                /// <summary>
                /// MPEG-4 part 2 Microsoft variant version 3.
                /// </summary>
                ///
                MSMPEG4v3,

                /// <summary>
                /// H.263+ / H.263-1998 / H.263 version 2.
                /// </summary>
                ///
                H263P,

                /// <summary>
                /// Flash Video (FLV) / Sorenson Spark / Sorenson H.263.
                /// </summary>
                FLV1,

                /// <summary>
                /// MPEG-2 part 2.
                /// </summary>
                ///
                MPEG2,

                /// <summary>
                /// Raw (uncompressed) video.
                /// </summary>
                ///
                Raw,

                /// <summary>
                /// FF video codec 1 lossless codec.
                /// </summary>
                ///
                FFV1,

                /// <summary>
                /// FFmpeg's HuffYUV lossless codec.
                /// </summary>
                ///
                FFVHUFF,

                /// <summary>
                /// H.264/MPEG-4 Part 10.
                /// </summary>
                ///
                H264,

                /// <summary>
                /// H.265
                /// </summary>
                ///
                H265,

                /// <summary>
                /// H.264/MPEG-4 Part 10.
                /// </summary>
                ///
                Theora,

				/// <summary>
				/// VP-8.
				/// </summary>
				///
                VP8,

				/// <summary>
				/// VP-9.
				/// </summary>
				///
                VP9,
            };

        }
    }
}
