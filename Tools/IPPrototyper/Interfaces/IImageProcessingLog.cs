// Image Processing Prototyper
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2010-2011
// contacts@aforgenet.com
//

namespace AForge.Imaging.IPPrototyper
{
    using System;
    using System.Drawing;

    /// <summary>
    /// Interface for image processing loggers, which keep information about image
    /// processing steps.
    /// </summary>
    public interface IImageProcessingLog
    {
        /// <summary>
        /// Add new image to the log.
        /// </summary>
        /// 
        /// <param name="key">Key/name of the image (image processing step).</param>
        /// <param name="image">Image to add to the log.</param>
        /// 
        /// <remarks><para>Adds new image to the image processing log or replaces existing
        /// image if specified key already exists in the log.</para></remarks>
        /// 
        void AddImage( string key, Bitmap image );

        /// <summary>
        /// Add messafe to the log.
        /// </summary>
        /// 
        /// <param name="message">Message to add to the image processing log.</param>
        /// 
        void AddMessage( string message );

        /// <summary>
        /// Clear image processing log removing all images and messages from it.
        /// </summary>
        void Clear( );
    }
}
