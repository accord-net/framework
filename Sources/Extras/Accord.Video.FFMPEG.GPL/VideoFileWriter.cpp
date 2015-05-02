// AForge FFMPEG Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2009-2012
// contacts@aforgenet.com
//

#include "StdAfx.h"
#include "VideoFileWriter.h"

namespace libffmpeg
{
	extern "C"
	{
		// disable warnings about badly formed documentation from FFmpeg, which don't need at all
		#pragma warning(disable:4635) 
		// disable warning about conversion int64 to int32
		#pragma warning(disable:4244) 

		#include "libavformat\avformat.h"
		#include "libavformat\avio.h"
		#include "libavcodec\avcodec.h"
		#include "libswscale\swscale.h"
	}
}

namespace AForge { namespace Video { namespace FFMPEG
{
#pragma region Some private FFmpeg related stuff hidden out of header file

static void write_video_frame( WriterPrivateData^ data );
static void open_video( WriterPrivateData^ data );
static void add_video_stream( WriterPrivateData^ data, int width, int height, int frameRate, int bitRate,
							  enum libffmpeg::CodecID codec_id, enum libffmpeg::PixelFormat pixelFormat );

// A structure to encapsulate all FFMPEG related private variable
ref struct WriterPrivateData
{
public:
	libffmpeg::AVFormatContext*		FormatContext;
	libffmpeg::AVStream*			VideoStream;
	libffmpeg::AVFrame*				VideoFrame;
	struct libffmpeg::SwsContext*	ConvertContext;
	struct libffmpeg::SwsContext*	ConvertContextGrayscale;

	libffmpeg::uint8_t*	VideoOutputBuffer;
	int VideoOutputBufferSize;

	WriterPrivateData( )
	{
		FormatContext     = NULL;
		VideoStream       = NULL;
		VideoFrame        = NULL;
		ConvertContext	  = NULL;
		ConvertContextGrayscale = NULL;
		VideoOutputBuffer = NULL;
	}
};
#pragma endregion

// Class constructor
VideoFileWriter::VideoFileWriter( void ) :
    data( nullptr ), disposed( false )
{
	libffmpeg::av_register_all( );
}

void VideoFileWriter::Open( String^ fileName, int width, int height )
{
	Open( fileName, width, height, 25 );
}

void VideoFileWriter::Open( String^ fileName, int width, int height, int frameRate )
{
	Open( fileName, width, height, frameRate, VideoCodec::Default );
}

void VideoFileWriter::Open( String^ fileName, int width, int height, int frameRate, VideoCodec codec )
{
	Open( fileName, width, height, frameRate, codec, 400000 );
}

// Creates a video file with the specified name and properties
void VideoFileWriter::Open( String^ fileName, int width, int height, int frameRate, VideoCodec codec, int bitRate )
{
    CheckIfDisposed( );

	// close previous file if any open
	Close( );

	data = gcnew WriterPrivateData( );
	bool success = false;

	// check width and height
	if ( ( ( width & 1 ) != 0 ) || ( ( height & 1 ) != 0 ) )
	{
		throw gcnew ArgumentException( "Video file resolution must be a multiple of two." );
	}

	// check video codec
	if ( ( (int) codec < -1 ) || ( (int) codec >= CODECS_COUNT ) )
	{
		throw gcnew ArgumentException( "Invalid video codec is specified." );
	}

	m_width  = width;
	m_height = height;
	m_codec  = codec;
	m_frameRate = frameRate;
	m_bitRate = bitRate;
	
	// convert specified managed String to unmanaged string
	IntPtr ptr = System::Runtime::InteropServices::Marshal::StringToHGlobalUni( fileName );
    wchar_t* nativeFileNameUnicode = (wchar_t*) ptr.ToPointer( );
    int utf8StringSize = WideCharToMultiByte( CP_UTF8, 0, nativeFileNameUnicode, -1, NULL, 0, NULL, NULL );
    char* nativeFileName = new char[utf8StringSize];
    WideCharToMultiByte( CP_UTF8, 0, nativeFileNameUnicode, -1, nativeFileName, utf8StringSize, NULL, NULL );

	try
	{
		// gues about destination file format from its file name
		libffmpeg::AVOutputFormat* outputFormat = libffmpeg::av_guess_format( NULL, nativeFileName, NULL );

		if ( !outputFormat )
		{
			// gues about destination file format from its short name
			outputFormat = libffmpeg::av_guess_format( "mpeg", NULL, NULL );

			if ( !outputFormat )
			{
				throw gcnew VideoException( "Cannot find suitable output format." );
			}
		}

		// prepare format context
		data->FormatContext = libffmpeg::avformat_alloc_context( );

		if ( !data->FormatContext )
		{
			throw gcnew VideoException( "Cannot allocate format context." );
		}
		data->FormatContext->oformat = outputFormat;

		// add video stream using the specified video codec
		add_video_stream( data, width, height, frameRate, bitRate,
			( codec == VideoCodec::Default ) ? outputFormat->video_codec : (libffmpeg::CodecID) video_codecs[(int) codec],
			( codec == VideoCodec::Default ) ? libffmpeg::PIX_FMT_YUV420P : (libffmpeg::PixelFormat) pixel_formats[(int) codec] );

		// set the output parameters (must be done even if no parameters)
		if ( libffmpeg::av_set_parameters( data->FormatContext, NULL ) < 0 )
		{
			throw gcnew VideoException( "Failed configuring format context." );
		}

		open_video( data );

		// open output file
		if ( !( outputFormat->flags & AVFMT_NOFILE ) )
		{
			if ( libffmpeg::avio_open( &data->FormatContext->pb, nativeFileName, AVIO_FLAG_WRITE ) < 0 )
			{
				throw gcnew System::IO::IOException( "Cannot open the video file." );
			}
		}

		libffmpeg::av_write_header( data->FormatContext );

		success = true;
	}
	finally
	{
		System::Runtime::InteropServices::Marshal::FreeHGlobal( ptr );
        delete [] nativeFileName;

		if ( !success )
		{
			Close( );
		}
	}
}

// Close current video file
void VideoFileWriter::Close( )
{
	if ( data != nullptr )
	{
		if ( data->FormatContext )
		{
			if ( data->FormatContext->pb != NULL )
			{
				libffmpeg::av_write_trailer( data->FormatContext );
			}

			if ( data->VideoStream )
			{
				libffmpeg::avcodec_close( data->VideoStream->codec );
			}

			if ( data->VideoFrame )
			{
				libffmpeg::av_free( data->VideoFrame->data[0] );
				libffmpeg::av_free( data->VideoFrame );
			}

			if ( data->VideoOutputBuffer )
			{
				libffmpeg::av_free( data->VideoOutputBuffer );
			}

			for ( unsigned int i = 0; i < data->FormatContext->nb_streams; i++ )
			{
				libffmpeg::av_freep( &data->FormatContext->streams[i]->codec );
				libffmpeg::av_freep( &data->FormatContext->streams[i] );
			}

			if ( data->FormatContext->pb != NULL )
			{
				libffmpeg::avio_close( data->FormatContext->pb );
			}
			
			libffmpeg::av_free( data->FormatContext );
		}

		if ( data->ConvertContext != NULL )
		{
			libffmpeg::sws_freeContext( data->ConvertContext );
		}

		if ( data->ConvertContextGrayscale != NULL )
		{
			libffmpeg::sws_freeContext( data->ConvertContextGrayscale );
		}

		data = nullptr;
	}

	m_width  = 0;
	m_height = 0;
}

// Writes new video frame to the opened video file
void VideoFileWriter::WriteVideoFrame( Bitmap^ frame )
{
	WriteVideoFrame( frame, TimeSpan::MinValue );
}

// Writes new video frame to the opened video file
void VideoFileWriter::WriteVideoFrame( Bitmap^ frame, TimeSpan timestamp )
{
    CheckIfDisposed( );

	if ( data == nullptr )
	{
		throw gcnew System::IO::IOException( "A video file was not opened yet." );
	}

	if ( ( frame->PixelFormat != PixelFormat::Format24bppRgb ) &&
	     ( frame->PixelFormat != PixelFormat::Format32bppArgb ) &&
		 ( frame->PixelFormat != PixelFormat::Format32bppPArgb ) &&
	 	 ( frame->PixelFormat != PixelFormat::Format32bppRgb ) &&
		 ( frame->PixelFormat != PixelFormat::Format8bppIndexed ) )
	{
		throw gcnew ArgumentException( "The provided bitmap must be 24 or 32 bpp color image or 8 bpp grayscale image." );
	}

	if ( ( frame->Width != m_width ) || ( frame->Height != m_height ) )
	{
		throw gcnew ArgumentException( "Bitmap size must be of the same as video size, which was specified on opening video file." );
	}

	// lock the bitmap
	BitmapData^ bitmapData = frame->LockBits( System::Drawing::Rectangle( 0, 0, m_width, m_height ),
		ImageLockMode::ReadOnly,
		( frame->PixelFormat == PixelFormat::Format8bppIndexed ) ? PixelFormat::Format8bppIndexed : PixelFormat::Format24bppRgb );

	libffmpeg::uint8_t* ptr = reinterpret_cast<libffmpeg::uint8_t*>( static_cast<void*>( bitmapData->Scan0 ) );

	libffmpeg::uint8_t* srcData[4] = { ptr, NULL, NULL, NULL };
	int srcLinesize[4] = { bitmapData->Stride, 0, 0, 0 };

	// convert source image to the format of the video file
	if ( frame->PixelFormat == PixelFormat::Format8bppIndexed )
	{
		libffmpeg::sws_scale( data->ConvertContextGrayscale, srcData, srcLinesize, 0, m_height, data->VideoFrame->data, data->VideoFrame->linesize );
	}
	else
	{
		libffmpeg::sws_scale( data->ConvertContext, srcData, srcLinesize, 0, m_height, data->VideoFrame->data, data->VideoFrame->linesize );
	}

	frame->UnlockBits( bitmapData );

	if ( timestamp.Ticks >= 0 )
	{
		const double frameNumber = timestamp.TotalSeconds * m_frameRate;
		data->VideoFrame->pts = static_cast<libffmpeg::int64_t>( frameNumber );
	}

	// write the converted frame to the video file
	write_video_frame( data );
}

#pragma region Private methods
// Writes video frame to opened video file
void write_video_frame( WriterPrivateData^ data )
{
	libffmpeg::AVCodecContext* codecContext = data->VideoStream->codec;
	int out_size, ret = 0;

	if ( data->FormatContext->oformat->flags & AVFMT_RAWPICTURE )
	{
		Console::WriteLine( "raw picture must be written" );
	}
	else
	{
		// encode the image
		out_size = libffmpeg::avcodec_encode_video( codecContext, data->VideoOutputBuffer,
			data->VideoOutputBufferSize, data->VideoFrame );

		// if zero size, it means the image was buffered
		if ( out_size > 0 )
		{
			libffmpeg::AVPacket packet;
			libffmpeg::av_init_packet( &packet );

			if ( codecContext->coded_frame->pts != AV_NOPTS_VALUE )
			{
				packet.pts = libffmpeg::av_rescale_q( codecContext->coded_frame->pts, codecContext->time_base, data->VideoStream->time_base );
			}

			if ( codecContext->coded_frame->key_frame )
			{
				packet.flags |= AV_PKT_FLAG_KEY;
			}

			packet.stream_index = data->VideoStream->index;
			packet.data = data->VideoOutputBuffer;
			packet.size = out_size;

			// write the compressed frame to the media file
			ret = libffmpeg::av_interleaved_write_frame( data->FormatContext, &packet );
		}
		else
		{
			// image was buffered
		}
	}

	if ( ret != 0 )
	{
		throw gcnew VideoException( "Error while writing video frame." );
	}
}

// Allocate picture of the specified format and size
static libffmpeg::AVFrame* alloc_picture( enum libffmpeg::PixelFormat pix_fmt, int width, int height )
{
	libffmpeg::AVFrame* picture;
	void* picture_buf;
	int size;

	picture = libffmpeg::avcodec_alloc_frame( );
	if ( !picture )
	{
		return NULL;
	}

	size = libffmpeg::avpicture_get_size( pix_fmt, width, height );
	picture_buf = libffmpeg::av_malloc( size );
	if ( !picture_buf )
	{
		libffmpeg::av_free( picture );
		return NULL;
	}

	libffmpeg::avpicture_fill( (libffmpeg::AVPicture *) picture, (libffmpeg::uint8_t *) picture_buf, pix_fmt, width, height );

	return picture;
}

// Create new video stream and configure it
void add_video_stream( WriterPrivateData^ data,  int width, int height, int frameRate, int bitRate,
					  enum libffmpeg::CodecID codecId, enum libffmpeg::PixelFormat pixelFormat )
{
	libffmpeg::AVCodecContext* codecContex;

	// create new stream
	data->VideoStream = libffmpeg::av_new_stream( data->FormatContext, 0 );
	if ( !data->VideoStream )
	{
		throw gcnew VideoException( "Failed creating new video stream." );
	}

	codecContex = data->VideoStream->codec;
	codecContex->codec_id   = codecId;
	codecContex->codec_type = libffmpeg::AVMEDIA_TYPE_VIDEO;

	// put sample parameters
	codecContex->bit_rate = bitRate;
	codecContex->width    = width;
	codecContex->height   = height;

	// time base: this is the fundamental unit of time (in seconds) in terms
	// of which frame timestamps are represented. for fixed-fps content,
	// timebase should be 1/framerate and timestamp increments should be
	// identically 1.
	codecContex->time_base.den = frameRate;
	codecContex->time_base.num = 1;

	codecContex->gop_size = 12; // emit one intra frame every twelve frames at most
	codecContex->pix_fmt  = pixelFormat;

	if ( codecContex->codec_id == libffmpeg::CODEC_ID_MPEG1VIDEO )
	{
		// Needed to avoid using macroblocks in which some coeffs overflow.
		// This does not happen with normal video, it just happens here as
		// the motion of the chroma plane does not match the luma plane.
		codecContex->mb_decision = 2;
	}

	// some formats want stream headers to be separate
	if( data->FormatContext->oformat->flags & AVFMT_GLOBALHEADER )
	{
		codecContex->flags |= CODEC_FLAG_GLOBAL_HEADER;
	}
}

// Open video codec and prepare out buffer and picture
void open_video( WriterPrivateData^ data )
{
	libffmpeg::AVCodecContext* codecContext = data->VideoStream->codec;
	libffmpeg::AVCodec* codec = avcodec_find_encoder( codecContext->codec_id );

	if ( !codec )
	{
		throw gcnew VideoException( "Cannot find video codec." );
	}

	// open the codec 
	if ( avcodec_open( codecContext, codec ) < 0 )
	{
		throw gcnew VideoException( "Cannot open video codec." );
	}

	data->VideoOutputBuffer = NULL;
	if ( !( data->FormatContext->oformat->flags & AVFMT_RAWPICTURE ) )
	{
         // allocate output buffer 
         data->VideoOutputBufferSize = 6 * codecContext->width * codecContext->height; // more than enough even for raw video
		 data->VideoOutputBuffer = (libffmpeg::uint8_t*) libffmpeg::av_malloc( data->VideoOutputBufferSize );
	}

	// allocate the encoded raw picture
	data->VideoFrame = alloc_picture( codecContext->pix_fmt, codecContext->width, codecContext->height );

	if ( !data->VideoFrame )
	{
		throw gcnew VideoException( "Cannot allocate video picture." );
	}

	// prepare scaling context to convert RGB image to video format
	data->ConvertContext = libffmpeg::sws_getContext( codecContext->width, codecContext->height, libffmpeg::PIX_FMT_BGR24,
			codecContext->width, codecContext->height, codecContext->pix_fmt,
			SWS_BICUBIC, NULL, NULL, NULL );
	// prepare scaling context to convert grayscale image to video format
	data->ConvertContextGrayscale = libffmpeg::sws_getContext( codecContext->width, codecContext->height, libffmpeg::PIX_FMT_GRAY8,
			codecContext->width, codecContext->height, codecContext->pix_fmt,
			SWS_BICUBIC, NULL, NULL, NULL );

	if ( ( data->ConvertContext == NULL ) || ( data->ConvertContextGrayscale == NULL ) )
	{
		throw gcnew VideoException( "Cannot initialize frames conversion context." );
	}
}
#pragma endregion
		
} } }

