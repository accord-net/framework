// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
//

namespace AForge.Imaging
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Collections.Generic;

    /// <summary>
    /// Exhaustive template matching.
    /// </summary>
    /// 
    /// <remarks><para>The class implements exhaustive template matching algorithm,
    /// which performs complete scan of source image, comparing each pixel with corresponding
    /// pixel of template.</para>
    /// 
    /// <para>The class processes only grayscale 8 bpp and color 24 bpp images.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create template matching algorithm's instance
    /// ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching( 0.9f );
    /// // find all matchings with specified above similarity
    /// TemplateMatch[] matchings = tm.ProcessImage( sourceImage, templateImage );
    /// // highlight found matchings
    /// BitmapData data = sourceImage.LockBits(
    ///     new Rectangle( 0, 0, sourceImage.Width, sourceImage.Height ),
    ///     ImageLockMode.ReadWrite, sourceImage.PixelFormat );
    /// foreach ( TemplateMatch m in matchings )
    /// {
    ///     Drawing.Rectangle( data, m.Rectangle, Color.White );
    ///     // do something else with matching
    /// }
    /// sourceImage.UnlockBits( data );
    /// </code>
    /// 
    /// <para>The class also can be used to get similarity level between two image of the same
    /// size, which can be useful to get information about how different/similar are images:</para>
    /// <code>
    /// // create template matching algorithm's instance
    /// // use zero similarity to make sure algorithm will provide anything
    /// ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching( 0 );
    /// // compare two images
    /// TemplateMatch[] matchings = tm.ProcessImage( image1, image2 );
    /// // check similarity level
    /// if ( matchings[0].Similarity > 0.95f )
    /// {
    ///     // do something with quite similar images
    /// }
    /// </code>
    /// 
    /// </remarks>
    /// 
    public class ExhaustiveTemplateMatching : ITemplateMatching
    {
        private float similarityThreshold = 0.9f;

        /// <summary>
        /// Similarity threshold, [0..1].
        /// </summary>
        /// 
        /// <remarks><para>The property sets the minimal acceptable similarity between template
        /// and potential found candidate. If similarity is lower than this value,
        /// then object is not treated as matching with template.
        /// </para>
        /// 
        /// <para>Default value is set to <b>0.9</b>.</para>
        /// </remarks>
        /// 
        public float SimilarityThreshold
        {
            get { return similarityThreshold; }
            set { similarityThreshold = Math.Min( 1, Math.Max( 0, value ) ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExhaustiveTemplateMatching"/> class.
        /// </summary>
        /// 
        public ExhaustiveTemplateMatching( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExhaustiveTemplateMatching"/> class.
        /// </summary>
        /// 
        /// <param name="similarityThreshold">Similarity threshold.</param>
        /// 
        public ExhaustiveTemplateMatching( float similarityThreshold )
        {
            this.similarityThreshold = similarityThreshold;
        }

        /// <summary>
        /// Process image looking for matchings with specified template.
        /// </summary>
        /// 
        /// <param name="image">Source image to process.</param>
        /// <param name="template">Template image to search for.</param>
        /// 
        /// <returns>Returns array of found template matches. The array is sorted by similarity
        /// of found matches in descending order.</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">The source image has incorrect pixel format.</exception>
        /// <exception cref="InvalidImagePropertiesException">Template image is bigger than source image.</exception>
        /// 
        public TemplateMatch[] ProcessImage( Bitmap image, Bitmap template )
        {
            return ProcessImage( image, template, new Rectangle( 0, 0, image.Width, image.Height ) );
        }

        /// <summary>
        /// Process image looking for matchings with specified template.
        /// </summary>
        /// 
        /// <param name="image">Source image to process.</param>
        /// <param name="template">Template image to search for.</param>
        /// <param name="searchZone">Rectangle in source image to search template for.</param>
        /// 
        /// <returns>Returns array of found template matches. The array is sorted by similarity
        /// of found matches in descending order.</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">The source image has incorrect pixel format.</exception>
        /// <exception cref="InvalidImagePropertiesException">Template image is bigger than source image.</exception>
        /// 
        public TemplateMatch[] ProcessImage( Bitmap image, Bitmap template, Rectangle searchZone )
        {
            // check image format
            if (
                ( ( image.PixelFormat != PixelFormat.Format8bppIndexed ) &&
                  ( image.PixelFormat != PixelFormat.Format24bppRgb ) ) ||
                ( image.PixelFormat != template.PixelFormat ) )
            {
                throw new UnsupportedImageFormatException( "Unsupported pixel format of the source or template image." );
            }

            // check template's size
            if ( ( template.Width > image.Width ) || ( template.Height > image.Height ) )
            {
                throw new InvalidImagePropertiesException( "Template's size should be smaller or equal to source image's size." );
            }

            // lock source and template images
            BitmapData imageData = image.LockBits(
                new Rectangle( 0, 0, image.Width, image.Height ),
                ImageLockMode.ReadOnly, image.PixelFormat );
            BitmapData templateData = template.LockBits(
                new Rectangle( 0, 0, template.Width, template.Height ),
                ImageLockMode.ReadOnly, template.PixelFormat );

            TemplateMatch[] matchings;

            try
            {
                // process the image
                matchings = ProcessImage(
                    new UnmanagedImage( imageData ),
                    new UnmanagedImage( templateData ),
                    searchZone );
            }
            finally
            {
                // unlock images
                image.UnlockBits( imageData );
                template.UnlockBits( templateData );
            }

            return matchings;
        }

        /// <summary>
        /// Process image looking for matchings with specified template.
        /// </summary>
        /// 
        /// <param name="imageData">Source image data to process.</param>
        /// <param name="templateData">Template image to search for.</param>
        /// 
        /// <returns>Returns array of found template matches. The array is sorted by similarity
        /// of found matches in descending order.</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">The source image has incorrect pixel format.</exception>
        /// <exception cref="InvalidImagePropertiesException">Template image is bigger than source image.</exception>
        /// 
        public TemplateMatch[] ProcessImage( BitmapData imageData, BitmapData templateData )
        {
            return ProcessImage( new UnmanagedImage( imageData ), new UnmanagedImage( templateData ),
                new Rectangle( 0, 0, imageData.Width, imageData.Height ) );
        }

        /// <summary>
        /// Process image looking for matchings with specified template.
        /// </summary>
        /// 
        /// <param name="imageData">Source image data to process.</param>
        /// <param name="templateData">Template image to search for.</param>
        /// <param name="searchZone">Rectangle in source image to search template for.</param>
        /// 
        /// <returns>Returns array of found template matches. The array is sorted by similarity
        /// of found matches in descending order.</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">The source image has incorrect pixel format.</exception>
        /// <exception cref="InvalidImagePropertiesException">Template image is bigger than source image.</exception>
        /// 
        public TemplateMatch[] ProcessImage( BitmapData imageData, BitmapData templateData, Rectangle searchZone )
        {
            return ProcessImage( new UnmanagedImage( imageData ), new UnmanagedImage( templateData ), searchZone );
        }

        /// <summary>
        /// Process image looking for matchings with specified template.
        /// </summary>
        /// 
        /// <param name="image">Unmanaged source image to process.</param>
        /// <param name="template">Unmanaged template image to search for.</param>
        /// 
        /// <returns>Returns array of found template matches. The array is sorted by similarity
        /// of found matches in descending order.</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">The source image has incorrect pixel format.</exception>
        /// <exception cref="InvalidImagePropertiesException">Template image is bigger than source image.</exception>
        ///
        public TemplateMatch[] ProcessImage( UnmanagedImage image, UnmanagedImage template )
        {
            return ProcessImage( image, template, new Rectangle( 0, 0, image.Width, image.Height ) );
        }

        /// <summary>
        /// Process image looking for matchings with specified template.
        /// </summary>
        /// 
        /// <param name="image">Unmanaged source image to process.</param>
        /// <param name="template">Unmanaged template image to search for.</param>
        /// <param name="searchZone">Rectangle in source image to search template for.</param>
        /// 
        /// <returns>Returns array of found template matches. The array is sorted by similarity
        /// of found matches in descending order.</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">The source image has incorrect pixel format.</exception>
        /// <exception cref="InvalidImagePropertiesException">Template image is bigger than search zone.</exception>
        ///
        public TemplateMatch[] ProcessImage( UnmanagedImage image, UnmanagedImage template, Rectangle searchZone )
        {
            // check image format
            if (
                ( ( image.PixelFormat != PixelFormat.Format8bppIndexed ) &&
                  ( image.PixelFormat != PixelFormat.Format24bppRgb ) ) ||
                ( image.PixelFormat != template.PixelFormat ) )
            {
                throw new UnsupportedImageFormatException( "Unsupported pixel format of the source or template image." );
            }

            // clip search zone
            Rectangle zone = searchZone;
            zone.Intersect( new Rectangle( 0, 0, image.Width, image.Height ) );

            // search zone's starting point
            int startX = zone.X;
            int startY = zone.Y;

            // get source and template image size
            int sourceWidth    = zone.Width;
            int sourceHeight   = zone.Height;
            int templateWidth  = template.Width;
            int templateHeight = template.Height;

            // check template's size
            if ( ( templateWidth > sourceWidth ) || ( templateHeight > sourceHeight ) )
            {
                throw new InvalidImagePropertiesException( "Template's size should be smaller or equal to search zone." );
            }

            int pixelSize = ( image.PixelFormat == PixelFormat.Format8bppIndexed ) ? 1 : 3;
            int sourceStride = image.Stride;

            // similarity map. its size is increased by 4 from each side to increase
            // performance of non-maximum suppresion
            int mapWidth  = sourceWidth - templateWidth + 1;
            int mapHeight = sourceHeight - templateHeight + 1;
            int[,] map = new int[mapHeight + 4, mapWidth + 4];

            // maximum possible difference with template
            int maxDiff = templateWidth * templateHeight * pixelSize * 255;

            // integer similarity threshold
            int threshold = (int) ( similarityThreshold * maxDiff );

            // width of template in bytes
            int templateWidthInBytes = templateWidth * pixelSize;

            // do the job
            unsafe
            {
                byte* baseSrc = (byte*) image.ImageData.ToPointer( );
                byte* baseTpl = (byte*) template.ImageData.ToPointer( );

                int sourceOffset = image.Stride - templateWidth * pixelSize;
                int templateOffset = template.Stride - templateWidth * pixelSize;

                // for each row of the source image
                for ( int y = 0; y < mapHeight; y++ )
                {
                    // for each pixel of the source image
                    for ( int x = 0; x < mapWidth; x++ )
                    {
                        byte* src = baseSrc + sourceStride * ( y + startY ) + pixelSize * ( x + startX );
                        byte* tpl = baseTpl;

                        // compare template with source image starting from current X,Y
                        int dif = 0;

                        // for each row of the template
                        for ( int i = 0; i < templateHeight; i++ )
                        {
                            // for each pixel of the template
                            for ( int j = 0; j < templateWidthInBytes; j++, src++, tpl++ )
                            {
                                int d = *src - *tpl;
                                if ( d > 0 )
                                {
                                    dif += d;
                                }
                                else
                                {
                                    dif -= d;
                                }
                            }
                            src += sourceOffset;
                            tpl += templateOffset;
                        }

                        // templates similarity
                        int sim = maxDiff - dif;

                        if ( sim >= threshold )
                            map[y + 2, x + 2] = sim;
                    }
                }
            }

            // collect interesting points - only those points, which are local maximums
            List<TemplateMatch> matchingsList = new List<TemplateMatch>( );

            // for each row
            for ( int y = 2, maxY = mapHeight + 2; y < maxY; y++ )
            {
                // for each pixel
                for ( int x = 2, maxX = mapWidth + 2; x < maxX; x++ )
                {
                    int currentValue = map[y, x];

                    // for each windows' row
                    for ( int i = -2; ( currentValue != 0 ) && ( i <= 2 ); i++ )
                    {
                        // for each windows' pixel
                        for ( int j = -2; j <= 2; j++ )
                        {
                            if ( map[y + i, x + j] > currentValue )
                            {
                                currentValue = 0;
                                break;
                            }
                        }
                    }

                    // check if this point is really interesting
                    if ( currentValue != 0 )
                    {
                        matchingsList.Add( new TemplateMatch(
                            new Rectangle( x - 2 + startX, y - 2 + startY, templateWidth, templateHeight ),
                            (float) currentValue / maxDiff ) );
                    }
                }
            }

            // convert list to array
            TemplateMatch[] matchings = new TemplateMatch[matchingsList.Count];
            matchingsList.CopyTo( matchings );
            // sort in descending order
            Array.Sort( matchings, new MatchingsSorter( ) );

            return matchings;
        }

        // Sorter of found matchings
        private class MatchingsSorter : System.Collections.IComparer
        {
            public int Compare( Object x, Object y )
            {
                float diff = ( (TemplateMatch) y ).Similarity - ( (TemplateMatch) x ).Similarity;

                return ( diff > 0 ) ? 1 : ( diff < 0 ) ? -1 : 0;
            }
        }
    }
}
