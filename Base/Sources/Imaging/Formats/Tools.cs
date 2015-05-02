// AForge Image Formats Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2008
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Formats
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    /// <summary>
    /// Set of tools used internally in AForge.Imaging.Formats library.
    /// </summary>
    internal class Tools
    {
        /// <summary>
        /// Create and initialize new grayscale image.
        /// </summary>
        /// 
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        /// 
        /// <returns>Returns new created grayscale image.</returns>
        /// 
        /// <remarks><para>AForge.Imaging.Image.CreateGrayscaleImage() function
        /// could be used instead, which does the some. But it was not used to get
        /// rid of dependency on AForge.Imaing library.</para></remarks>
        /// 
        public static Bitmap CreateGrayscaleImage( int width, int height )
        {
            // create new image
            Bitmap image = new Bitmap( width, height, PixelFormat.Format8bppIndexed );
            // get palette
            ColorPalette cp = image.Palette;
            // init palette with grayscale colors
            for ( int i = 0; i < 256; i++ )
            {
                cp.Entries[i] = Color.FromArgb( i, i, i );
            }
            // set palette back
            image.Palette = cp;
            // return new image
            return image;
        }

        /// <summary>
        /// Read specified amount of bytes from the specified stream.
        /// </summary>
        /// 
        /// <param name="stream">Source sream to read data from.</param>
        /// <param name="buffer">Buffer to read data into.</param>
        /// <param name="offset">Offset in buffer to put data into.</param>
        /// <param name="count">Number of bytes to read.</param>
        /// 
        /// <returns>Returns total number of bytes read. It may be smaller than requested amount only
        /// in the case if end of stream was reached.</returns>
        /// 
        /// <remarks><para>This tool function guarantees that requested number of bytes
        /// was read from the source stream (.NET streams don't guarantee this and may return less bytes
        /// than it was requested). Only in the case if end of stream was reached, the function
        /// may return with less bytes read.</para>
        /// </remarks>
        /// 
        public static int ReadStream( Stream stream, byte[] buffer, int offset, int count )
        {
            int totalBytesRead = 0, bytesRead = 0;

            while ( totalBytesRead != count )
            {
                bytesRead = stream.Read( buffer, offset + totalBytesRead, count - totalBytesRead );

                if ( bytesRead == 0 )
                {
                    break;
                }

                totalBytesRead += bytesRead;
            }

            return totalBytesRead;
        }
    }
}
