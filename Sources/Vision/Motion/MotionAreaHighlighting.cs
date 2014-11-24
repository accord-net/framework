// AForge Vision Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2011
// contacts@aforgenet.com
//

namespace AForge.Vision.Motion
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    using AForge.Imaging;

    /// <summary>
    /// Motion processing algorithm, which highlights motion areas.
    /// </summary>
    /// 
    /// <remarks><para>The aim of this motion processing algorithm is to highlight
    /// motion areas with grid pattern of the <see cref="HighlightColor">specified color</see>.
    /// </para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create motion detector
    /// MotionDetector detector = new MotionDetector(
    ///     /* motion detection algorithm */,
    ///     new MotionAreaHighlighting( ) );
    /// 
    /// // continuously feed video frames to motion detector
    /// while ( ... )
    /// {
    ///     // process new video frame
    ///     detector.ProcessFrame( videoFrame );
    /// }
    /// </code>
    /// </remarks>
    /// 
    /// <seealso cref="MotionDetector"/>
    /// <seealso cref="IMotionDetector"/>
    /// 
    public class MotionAreaHighlighting : IMotionProcessing
    {
        private Color highlightColor = Color.Red;

        /// <summary>
        /// Color used to highlight motion regions.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>Default value is set to <b>red</b> color.</para>
        /// </remarks>
        /// 
        public Color HighlightColor
        {
            get { return highlightColor; }
            set { highlightColor = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MotionAreaHighlighting"/> class.
        /// </summary>
        /// 
        public MotionAreaHighlighting( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MotionAreaHighlighting"/> class.
        /// </summary>
        /// 
        /// <param name="highlightColor">Color used to highlight motion regions.</param>
        /// 
        public MotionAreaHighlighting( Color highlightColor )
        {
            this.highlightColor = highlightColor;
        }

        /// <summary>
        /// Process video and motion frames doing further post processing after
        /// performed motion detection.
        /// </summary>
        /// 
        /// <param name="videoFrame">Original video frame.</param>
        /// <param name="motionFrame">Motion frame provided by motion detection
        /// algorithm (see <see cref="IMotionDetector"/>).</param>
        /// 
        /// <remarks><para>Processes provided motion frame and highlights motion areas
        /// on the original video frame with <see cref="HighlightColor">specified color</see>.</para>
        /// </remarks>
        /// 
        /// <exception cref="InvalidImagePropertiesException">Motion frame is not 8 bpp image, but it must be so.</exception>
        /// <exception cref="UnsupportedImageFormatException">Video frame must be 8 bpp grayscale image or 24/32 bpp color image.</exception>
        ///
        public unsafe void ProcessFrame( UnmanagedImage videoFrame, UnmanagedImage motionFrame )
        {
            if ( motionFrame.PixelFormat != PixelFormat.Format8bppIndexed )
            {
                throw new InvalidImagePropertiesException( "Motion frame must be 8 bpp image." );
            }

            if ( ( videoFrame.PixelFormat != PixelFormat.Format8bppIndexed ) &&
                 ( videoFrame.PixelFormat != PixelFormat.Format24bppRgb ) &&
                 ( videoFrame.PixelFormat != PixelFormat.Format32bppRgb ) &&
                 ( videoFrame.PixelFormat != PixelFormat.Format32bppArgb ) )
            {
                throw new UnsupportedImageFormatException( "Video frame must be 8 bpp grayscale image or 24/32 bpp color image." );
            }

            int width  = videoFrame.Width;
            int height = videoFrame.Height;
            int pixelSize = Bitmap.GetPixelFormatSize( videoFrame.PixelFormat ) / 8; 

            if ( ( motionFrame.Width != width ) || ( motionFrame.Height != height ) )
                return;

            byte* src = (byte*) videoFrame.ImageData.ToPointer( );
            byte* motion = (byte*) motionFrame.ImageData.ToPointer( );

            int srcOffset = videoFrame.Stride - width * pixelSize;
            int motionOffset = motionFrame.Stride - width;

            if ( pixelSize == 1 )
            {
                // grayscale case
                byte fillG = (byte) ( 0.2125 * highlightColor.R +
                                      0.7154 * highlightColor.G +
                                      0.0721 * highlightColor.B );

                for ( int y = 0; y < height; y++ )
                {
                    for ( int x = 0; x < width; x++, motion++, src++ )
                    {
                        if ( ( *motion != 0 ) && ( ( ( x + y ) & 1 ) == 0 ) )
                        {
                            *src = fillG;
                        }
                    }
                    src += srcOffset;
                    motion += motionOffset;
                }
            }
            else
            {
                // color case
                byte fillR = highlightColor.R;
                byte fillG = highlightColor.G;
                byte fillB = highlightColor.B;

                for ( int y = 0; y < height; y++ )
                {
                    for ( int x = 0; x < width; x++, motion++, src += pixelSize )
                    {
                        if ( ( *motion != 0 ) && ( ( ( x + y ) & 1 ) == 0 ) )
                        {
                            src[RGB.R] = fillR;
                            src[RGB.G] = fillG;
                            src[RGB.B] = fillB;
                        }
                    }
                    src += srcOffset;
                    motion += motionOffset;
                }
            }
        }

        /// <summary>
        /// Reset internal state of motion processing algorithm.
        /// </summary>
        /// 
        /// <remarks><para>The method allows to reset internal state of motion processing
        /// algorithm and prepare it for processing of next video stream or to restart
        /// the algorithm.</para></remarks>
        ///
        public void Reset( )
        {
        }
    }
}
