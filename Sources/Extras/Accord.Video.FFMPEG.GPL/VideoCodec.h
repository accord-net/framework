// AForge FFMPEG Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2009-2011
// contacts@aforgenet.com
//

#pragma once

using namespace System;

extern int video_codecs[];
extern int pixel_formats[];

extern int CODECS_COUNT;

namespace AForge { namespace Video { namespace FFMPEG
{
	/// <summary>
	/// Enumeration of some video codecs from FFmpeg library, which are available for writing video files.
	/// </summary>
	public enum class VideoCodec
	{
		/// <summary>
		/// Default video codec, which FFmpeg library selects for the specified file format.
		/// </summary>
		Default = -1,
		/// <summary>
		/// MPEG-4 part 2.
		/// </summary>
		MPEG4 = 0,
		/// <summary>
		/// Windows Media Video 7.
		/// </summary>
		WMV1,
		/// <summary>
		/// Windows Media Video 8.
		/// </summary>
		WMV2,
		/// <summary>
		/// MPEG-4 part 2 Microsoft variant version 2.
		/// </summary>
		MSMPEG4v2,
		/// <summary>
		/// MPEG-4 part 2 Microsoft variant version 3.
		/// </summary>
		MSMPEG4v3,
		/// <summary>
		/// H.263+ / H.263-1998 / H.263 version 2.
		/// </summary>
		H263P,
		/// <summary>
		/// Flash Video (FLV) / Sorenson Spark / Sorenson H.263.
		/// </summary>
		FLV1,
		/// <summary>
		/// MPEG-2 part 2.
		/// </summary>
		MPEG2,
		/// <summary>
		/// Raw (uncompressed) video.
		/// </summary>
		Raw,
	};

} } }