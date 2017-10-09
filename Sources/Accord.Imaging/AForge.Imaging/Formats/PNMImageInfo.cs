// AForge Image Formats Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2008
// andrew.kirillov@gmail.com
//

namespace Accord.Imaging.Formats
{
    using System.ComponentModel;
    /// <summary>
    /// Information about PNM image's frame.
    /// </summary>
    public sealed class PNMImageInfo : ImageInfo
    {
        // PNM file version (1-6)
        private int version;
        // maximum data value
        private int maxDataValue;

        /// <summary>
        /// PNM file version (format), [1, 6].
        /// </summary>
        [Category("PNM Info")]
        public int Version
        {
            get { return version; }
            set { version = value; }
        }

        /// <summary>
        /// Maximum pixel's value in source PNM image.
        /// </summary>
        /// 
        /// <remarks><para>The value is used to scale image's data converting them
        /// from original data range to the range of
        /// <see cref="ImageInfo.BitsPerPixel">supported bits per pixel</see> format.</para></remarks>
        /// 
        [Category("PNM Info")]
        public int MaxDataValue
        {
            get { return maxDataValue; }
            set { maxDataValue = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PNMImageInfo"/> class.
        /// </summary>
        /// 
        public PNMImageInfo() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PNMImageInfo"/> class.
        /// </summary>
        /// 
        /// <param name="width">Image's width.</param>
        /// <param name="height">Image's height.</param>
        /// <param name="bitsPerPixel">Number of bits per image's pixel.</param>
        /// <param name="frameIndex">Frame's index.</param>
        /// <param name="totalFrames">Total frames in the image.</param>
        /// 
        public PNMImageInfo(int width, int height, int bitsPerPixel, int frameIndex, int totalFrames) :
            base(width, height, bitsPerPixel, frameIndex, totalFrames) { }

        /// <summary>
        /// Creates a new object that is a copy of the current instance. 
        /// </summary>
        /// 
        /// <returns>A new object that is a copy of this instance.</returns>
        /// 
        public override object Clone()
        {
            PNMImageInfo clone = new PNMImageInfo(width, height, bitsPerPixel, frameIndex, totalFrames);

            clone.version = version;
            clone.maxDataValue = maxDataValue;

            return clone;
        }
    }
}
