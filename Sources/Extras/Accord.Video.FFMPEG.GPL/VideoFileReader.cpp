// AForge FFMPEG Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2009-2012
// contacts@aforgenet.com
//

#include "StdAfx.h"
#include "VideoFileReader.h"

namespace libffmpeg
{
	extern "C"
	{
		// disable warnings about badly formed documentation from FFmpeg, which we don't need at all
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

// A structure to encapsulate all FFMPEG related private variable
ref struct ReaderPrivateData
{
public:
	libffmpeg::AVFormatContext*		FormatContext;
	libffmpeg::AVStream*			VideoStream;
	libffmpeg::AVCodecContext*		CodecContext;
	libffmpeg::AVFrame*				VideoFrame;
	struct libffmpeg::SwsContext*	ConvertContext;

	libffmpeg::AVPacket* Packet;
	int BytesRemaining;

	ReaderPrivateData( )
	{
		FormatContext     = NULL;
		VideoStream       = NULL;
		CodecContext      = NULL;
		VideoFrame        = NULL;
		ConvertContext	  = NULL;

		Packet  = NULL;
		BytesRemaining = 0;
	}
};
#pragma endregion

// Class constructor
VideoFileReader::VideoFileReader( void ) :
    data( nullptr ), disposed( false )
{	
	libffmpeg::av_register_all( );
}

#pragma managed(push, off)
static libffmpeg::AVFormatContext* open_file( char* fileName )
{
	libffmpeg::AVFormatContext* formatContext;

	if ( libffmpeg::av_open_input_file( &formatContext, fileName, NULL, 0, NULL ) !=0 )
	{
		return NULL;
	}
	return formatContext;
}
#pragma managed(pop)

// Opens the specified video file
void VideoFileReader::Open( String^ fileName )
{
    CheckIfDisposed( );

	// close previous file if any was open
	Close( );

	data = gcnew ReaderPrivateData( );
	data->Packet = new libffmpeg::AVPacket( );
	data->Packet->data = NULL;

	bool success = false;

	// convert specified managed String to UTF8 unmanaged string
	IntPtr ptr = System::Runtime::InteropServices::Marshal::StringToHGlobalUni( fileName );
    wchar_t* nativeFileNameUnicode = (wchar_t*) ptr.ToPointer( );
    int utf8StringSize = WideCharToMultiByte( CP_UTF8, 0, nativeFileNameUnicode, -1, NULL, 0, NULL, NULL );
    char* nativeFileName = new char[utf8StringSize];
    WideCharToMultiByte( CP_UTF8, 0, nativeFileNameUnicode, -1, nativeFileName, utf8StringSize, NULL, NULL );

	try
	{
		// open the specified video file
		data->FormatContext = open_file( nativeFileName );
		if ( data->FormatContext == NULL )
		{
			throw gcnew System::IO::IOException( "Cannot open the video file." );
		}

		// retrieve stream information
		if ( libffmpeg::av_find_stream_info( data->FormatContext ) < 0 )
		{
			throw gcnew VideoException( "Cannot find stream information." );
		}

		// search for the first video stream
		for ( unsigned int i = 0; i < data->FormatContext->nb_streams; i++ )
		{
			if( data->FormatContext->streams[i]->codec->codec_type == libffmpeg::AVMEDIA_TYPE_VIDEO )
			{
				// get the pointer to the codec context for the video stream
				data->CodecContext = data->FormatContext->streams[i]->codec;
				data->VideoStream  = data->FormatContext->streams[i];
				break;
			}
		}
		if ( data->VideoStream == NULL )
		{
			throw gcnew VideoException( "Cannot find video stream in the specified file." );
		}

		// find decoder for the video stream
		libffmpeg::AVCodec* codec = libffmpeg::avcodec_find_decoder( data->CodecContext->codec_id );
		if ( codec == NULL )
		{
			throw gcnew VideoException( "Cannot find codec to decode the video stream." );
		}

		// open the codec
		if ( libffmpeg::avcodec_open( data->CodecContext, codec ) < 0 )
		{
			throw gcnew VideoException( "Cannot open video codec." );
		}

		// allocate video frame
		data->VideoFrame = libffmpeg::avcodec_alloc_frame( );

		// prepare scaling context to convert RGB image to video format
		data->ConvertContext = libffmpeg::sws_getContext( data->CodecContext->width, data->CodecContext->height, data->CodecContext->pix_fmt,
				data->CodecContext->width, data->CodecContext->height, libffmpeg::PIX_FMT_BGR24,
				SWS_BICUBIC, NULL, NULL, NULL );

		if ( data->ConvertContext == NULL )
		{
			throw gcnew VideoException( "Cannot initialize frames conversion context." );
		}

		// get some properties of the video file
		m_width  = data->CodecContext->width;
		m_height = data->CodecContext->height;
		m_frameRate = data->VideoStream->r_frame_rate.num / data->VideoStream->r_frame_rate.den;
		m_codecName = gcnew String( data->CodecContext->codec->name );
		m_framesCount = data->VideoStream->nb_frames;

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
void VideoFileReader::Close(  )
{
	if ( data != nullptr )
	{
		if ( data->VideoFrame != NULL )
		{
			libffmpeg::av_free( data->VideoFrame );
		}

		if ( data->CodecContext != NULL )
		{
			libffmpeg::avcodec_close( data->CodecContext );
		}

		if ( data->FormatContext != NULL )
		{
			libffmpeg::av_close_input_file( data->FormatContext );
		}

		if ( data->ConvertContext != NULL )
		{
			libffmpeg::sws_freeContext( data->ConvertContext );
		}

		if ( data->Packet->data != NULL )
		{
			libffmpeg::av_free_packet( data->Packet );
		}

		data = nullptr;
	}
}

// Read next video frame of the current video file
Bitmap^ VideoFileReader::ReadVideoFrame(  )
{
    CheckIfDisposed( );

	if ( data == nullptr )
	{
		throw gcnew System::IO::IOException( "Cannot read video frames since video file is not open." );
	}

	int frameFinished;
	Bitmap^ bitmap = nullptr;

	int bytesDecoded;
	bool exit = false;

	while ( true )
	{
		// work on the current packet until we have decoded all of it
		while ( data->BytesRemaining > 0 )
		{
			// decode the next chunk of data
			bytesDecoded = libffmpeg::avcodec_decode_video2( data->CodecContext, data->VideoFrame, &frameFinished, data->Packet );

			// was there an error?
			if ( bytesDecoded < 0 )
			{
				throw gcnew VideoException( "Error while decoding frame." );
			}

			data->BytesRemaining -= bytesDecoded;
					 
			// did we finish the current frame? Then we can return
			if ( frameFinished )
			{
				return DecodeVideoFrame( );
			}
		}

		// read the next packet, skipping all packets that aren't
		// for this stream
		do
		{
			// free old packet if any
			if ( data->Packet->data != NULL )
			{
				libffmpeg::av_free_packet( data->Packet );
				data->Packet->data = NULL;
			}

			// read new packet
			if ( libffmpeg::av_read_frame( data->FormatContext, data->Packet ) < 0)
			{
				exit = true;
				break;
			}
		}
		while ( data->Packet->stream_index != data->VideoStream->index );

		// exit ?
		if ( exit )
			break;

		data->BytesRemaining = data->Packet->size;
	}

	// decode the rest of the last frame
	bytesDecoded = libffmpeg::avcodec_decode_video2(
		data->CodecContext, data->VideoFrame, &frameFinished, data->Packet );

	// free last packet
	if ( data->Packet->data != NULL )
	{
		libffmpeg::av_free_packet( data->Packet );
		data->Packet->data = NULL;
	}

	// is there a frame
	if ( frameFinished )
	{
		bitmap = DecodeVideoFrame( );
	}

	return bitmap;
}

// Decodes video frame into managed Bitmap
Bitmap^ VideoFileReader::DecodeVideoFrame( )
{
	Bitmap^ bitmap = gcnew Bitmap( data->CodecContext->width, data->CodecContext->height, PixelFormat::Format24bppRgb );
	
	// lock the bitmap
	BitmapData^ bitmapData = bitmap->LockBits( System::Drawing::Rectangle( 0, 0, data->CodecContext->width, data->CodecContext->height ),
		ImageLockMode::ReadOnly, PixelFormat::Format24bppRgb );

	libffmpeg::uint8_t* ptr = reinterpret_cast<libffmpeg::uint8_t*>( static_cast<void*>( bitmapData->Scan0 ) );

	libffmpeg::uint8_t* srcData[4] = { ptr, NULL, NULL, NULL };
	int srcLinesize[4] = { bitmapData->Stride, 0, 0, 0 };

	// convert video frame to the RGB bitmap
	libffmpeg::sws_scale( data->ConvertContext, data->VideoFrame->data, data->VideoFrame->linesize, 0,
		data->CodecContext->height, srcData, srcLinesize );

	bitmap->UnlockBits( bitmapData );

	return bitmap;
}

} } }