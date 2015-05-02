// AForge Image Formats Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2008
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Formats
{
    using System;
    using System.IO;
    using System.Drawing;
    using System.ComponentModel;

    /// <summary>
    /// Image decoder interface, which specifies set of methods, which should be
    /// implemented by image decoders for different file formats.
    /// </summary>
    /// 
    /// <remarks><para>The interface specifies set of methods, which are suitable not
    /// only for simple one-frame image formats. The interface also defines methods
    /// to work with image formats designed to store multiple frames and image formats
    /// which provide different type of image description (like acquisition
    /// parameters, etc).
    /// </para></remarks>
    /// 
    public interface IImageDecoder
    {
        /// <summary>
        /// Decode first frame of image from the specified stream.
        /// </summary>
        /// 
        /// <param name="stream">Source stream, which contains encoded image.</param>
        /// 
        /// <returns>Returns decoded image frame.</returns>
        /// 
        /// <remarks>
        /// <para>For one-frame image formats the method is supposed to decode single
        /// available frame. For multi-frame image formats the first frame should be
        /// decoded.</para>
        /// 
        /// <para>Implementations of this method may throw
        /// <see cref="System.FormatException"/> exception to report about unrecognized image
        /// format, <see cref="System.ArgumentException"/> exception to report about incorrectly
        /// formatted image or <see cref="NotSupportedException"/> exception to report if
        /// certain formats are not supported.</para>
        /// </remarks>
        /// 
        Bitmap DecodeSingleFrame( Stream stream );

        /// <summary>
        /// Open specified stream.
        /// </summary>
        /// 
        /// <param name="stream">Stream to open.</param>
        /// 
        /// <returns>Returns number of images found in the specified stream.</returns>
        /// 
        /// <remarks><para>Implementation of this method is supposed to read image's header,
        /// checking for correct image format and reading its atributes.</para>
        /// 
        /// <para>Implementations of this method may throw
        /// <see cref="System.FormatException"/> exception to report about unrecognized image
        /// format, <see cref="System.ArgumentException"/> exception to report about incorrectly
        /// formatted image or <see cref="NotSupportedException"/> exception to report if
        /// certain formats are not supported.</para>
        /// </remarks>
        /// 
        int Open( Stream stream );

        /// <summary>
        /// Decode specified frame.
        /// </summary>
        /// 
        /// <param name="frameIndex">Image frame to decode.</param>
        /// <param name="imageInfo">Receives information about decoded frame.</param>
        /// 
        /// <returns>Returns decoded frame.</returns>
        /// 
        /// <remarks>Implementations of this method may throw
        /// <see cref="System.NullReferenceException"/> exception in the case if no image
        /// stream was opened previously, <see cref="System.ArgumentOutOfRangeException"/> in the
        /// case if stream does not contain frame with specified index or  <see cref="System.ArgumentException"/>
        /// exception to report about incorrectly formatted image.
        /// </remarks>
        /// 
        Bitmap DecodeFrame( int frameIndex, out ImageInfo imageInfo );

        /// <summary>
        /// Close decoding of previously opened stream.
        /// </summary>
        /// 
        /// <remarks><para>Implementations of this method don't close stream itself, but just close
        /// decoding cleaning all associated data with it.</para></remarks>
        /// 
        void Close( );
    }

    /// <summary>
    /// Information about image's frame.
    /// </summary>
    /// 
    /// <remarks><para>This is a base class, which keeps basic information about image, like its width,
    /// height, etc. Classes, which inherit from this, may define more properties describing certain
    /// image formats.</para></remarks>
    /// 
    public class ImageInfo : ICloneable
    {
        /// <summary>
        /// Image's width.
        /// </summary>
        protected int width;

        /// <summary>
        /// Image's height.
        /// </summary>
        protected int height;

        /// <summary>
        /// Number of bits per image's pixel.
        /// </summary>
        protected int bitsPerPixel;

        /// <summary>
        /// Frame's index.
        /// </summary>
        protected int frameIndex;

        /// <summary>
        ///  Total frames in the image.
        /// </summary>
        protected int totalFrames;

        /// <summary>
        /// Image's width.
        /// </summary>
        [Category( "General" )]
        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        /// <summary>
        /// Image's height.
        /// </summary>
        [Category( "General" )]
        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        /// <summary>
        /// Number of bits per image's pixel.
        /// </summary>
        [Category( "General" )]
        public int BitsPerPixel
        {
            get { return bitsPerPixel; }
            set { bitsPerPixel = value; }
        }

        /// <summary>
        /// Frame's index.
        /// </summary>
        /// 
        /// <remarks><para>Some image formats support storing multiple frames in one image file.
        /// The property specifies index of a particular frame.</para></remarks>
        /// 
        [Category( "General" )]
        public int FrameIndex
        {
            get { return frameIndex; }
            set { frameIndex = value; }
        }

        /// <summary>
        /// Total frames in the image.
        /// </summary>
        /// 
        /// <remarks><para>Some image formats support storing multiple frames in one image file.
        /// The property specifies total number of frames in image file.</para></remarks>
        /// 
        [Category( "General" )]
        public int TotalFrames
        {
            get { return totalFrames; }
            set { totalFrames = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageInfo"/> class.
        /// </summary>
        /// 
        public ImageInfo( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageInfo"/> class.
        /// </summary>
        /// 
        /// <param name="width">Image's width.</param>
        /// <param name="height">Image's height.</param>
        /// <param name="bitsPerPixel">Number of bits per image's pixel.</param>
        /// <param name="frameIndex">Frame's index.</param>
        /// <param name="totalFrames">Total frames in the image.</param>
        /// 
        public ImageInfo( int width, int height, int bitsPerPixel, int frameIndex, int totalFrames )
        {
            this.width  = width;
            this.height = height;
            this.bitsPerPixel = bitsPerPixel;
            this.frameIndex  = frameIndex;
            this.totalFrames = totalFrames;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance. 
        /// </summary>
        /// 
        /// <returns>A new object that is a copy of this instance.</returns>
        /// 
        public virtual object Clone( )
        {
            return new ImageInfo( width, height, bitsPerPixel, frameIndex, totalFrames );
        }
    }
}
