// Image Processing Prototyper
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2010-2011
// contacts@aforgenet.com
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using AForge.Imaging.IPPrototyper;

namespace AForge.Imaging.IPPrototyper
{
    /// <summary>
    /// Default implementation of <see cref="IImageProcessingLog"/> interface.
    /// </summary>
    public class ImageProcessingLog : IImageProcessingLog
    {
        private Dictionary<string, Bitmap> images = new Dictionary<string,Bitmap>( );
        private List<string> messages = new List<string>( );

        /// <summary>
        ///  Collection of images representing image processing steps.
        /// </summary>
        public Dictionary<string, Bitmap> Images
        {
            get { return images; }
        }

        /// <summary>
        /// List of messages stored in the log.
        /// </summary>
        public List<string> Messages
        {
            get { return messages; }
        }

        /// <summary>
        /// Clear image processing log removing all images and messages from it.
        /// </summary>
        public void Clear( )
        {
            foreach ( KeyValuePair<string, Bitmap> kvp in images )
            {
                kvp.Value.Dispose( );
            }
            images.Clear( );
            messages.Clear( );
        }

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
        public void AddImage( string key, Bitmap image )
        {
            Bitmap imageToStore = (Bitmap) image.Clone( );

            if ( images.ContainsKey( key ) )
            {
                images[key].Dispose( );
                images[key] = imageToStore;
            }
            else
            {
                images.Add( key, imageToStore );
            }
        }

        /// <summary>
        /// Add messafe to the log.
        /// </summary>
        /// 
        /// <param name="message">Message to add to the image processing log.</param>
        ///
        public void AddMessage( string message )
        {
            messages.Add( message );
        }
    }
}
