// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
//
// Copyright © Joan Charmant, 2008
// joan.charmant@gmail.com
//
namespace AForge.Imaging
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Block matching implementation with the exhaustive search algorithm.
    /// </summary>
    /// 
    /// <remarks><para>The class implements exhaustive search block matching algorithm
    /// (see documentation for <see cref="IBlockMatching"/> for information about
    /// block matching algorithms). Exhaustive search algorithm tests each possible
    /// location of block within search window trying to find a match with minimal
    /// difference.</para>
    /// 
    /// <para><note>Because of the exhaustive nature of the algorithm, high performance
    /// should not be expected in the case if big number of reference points is provided
    /// or big block size and search radius are specified. Minimizing theses values increases
    /// performance. But too small block size and search radius may affect quality.</note></para>
    /// 
    /// <para><note>The class processes only grayscale (8 bpp indexed) and color (24 bpp) images.</note></para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // collect reference points using corners detector (for example)
    /// SusanCornersDetector scd = new SusanCornersDetector( 30, 18 );
    /// List&lt;IntPoint&gt; points = scd.ProcessImage( sourceImage );
    /// 
    /// // create block matching algorithm's instance
    /// ExhaustiveBlockMatching bm = new ExhaustiveBlockMatching( 8, 12 );
    /// // process images searching for block matchings
    /// List&lt;BlockMatch&gt; matches = bm.ProcessImage( sourceImage, points, searchImage );
    /// 
    /// // draw displacement vectors
    /// BitmapData data = sourceImage.LockBits(
    ///     new Rectangle( 0, 0, sourceImage.Width, sourceImage.Height ),
    ///     ImageLockMode.ReadWrite, sourceImage.PixelFormat );
    /// 
    /// foreach ( BlockMatch match in matches )
    /// {
    ///     // highlight the original point in source image
    ///     Drawing.FillRectangle( data,
    ///         new Rectangle( match.SourcePoint.X - 1, match.SourcePoint.Y - 1, 3, 3 ),
    ///         Color.Yellow );
    ///     // draw line to the point in search image
    ///     Drawing.Line( data, match.SourcePoint, match.MatchPoint, Color.Red );
    /// 
    ///     // check similarity
    ///     if ( match.Similarity > 0.98f )
    ///     {
    ///         // process block with high similarity somehow special
    ///     }
    /// }
    /// 
    /// sourceImage.UnlockBits( data );
    /// </code>
    /// 
    /// <para><b>Test image 1 (source):</b></para>
    /// <img src="img/imaging/ebm_sample1.png" width="217" height="192" />
    /// <para><b>Test image 2 (search):</b></para>
    /// <img src="img/imaging/ebm_sample2.png" width="217" height="192" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/ebm_result.png" width="217" height="192" />
    /// </remarks>
    /// 
    public class ExhaustiveBlockMatching : IBlockMatching
    {
        // block size to search for
        private int blockSize = 16;
        // search radius (maximum shift from base position, in all 4 directions) 
        private int searchRadius = 12;
        // blocks' similarity threshold
        private float similarityThreshold = 0.9f;

        /// <summary>
        /// Search radius.
        /// </summary>
        /// 
        /// <remarks><para>The value specifies the shift from reference point in all
        /// four directions, used to search for the best matching block.</para>
        /// 
        /// <para>Default value is set to <b>12</b>.</para>
        /// </remarks>
        /// 
        public int SearchRadius
        {
            get { return searchRadius; }
            set { searchRadius = value; }
        }

        /// <summary>
        /// Block size to search for.
        /// </summary>
        /// 
        /// <remarks><para>The value specifies block size to search for. For each provided
        /// reference pointer, a square block of this size is taken from the source image
        /// (reference point becomes the coordinate of block's center) and the best match
        /// is searched in second image within specified <see cref="SearchRadius">search
        /// radius</see>.</para>
        /// 
        /// <para>Default value is set to <b>16</b>.</para>
        /// </remarks>
        /// 
        public int BlockSize
        {
            get { return blockSize; }
            set { blockSize = value; }
        }

        /// <summary>
        /// Similarity threshold, [0..1].
        /// </summary>
        /// 
        /// <remarks><para>The property sets the minimal acceptable similarity between blocks
        /// in source and search images. If similarity is lower than this value,
        /// then the candidate block in search image is not treated as a match for the block
        /// in source image.
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
        /// Initializes a new instance of the <see cref="ExhaustiveBlockMatching"/> class.
        /// </summary>
        public ExhaustiveBlockMatching( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExhaustiveBlockMatching"/> class.
        /// </summary>
        /// 
        /// <param name="blockSize">Block size to search for.</param>
        /// <param name="searchRadius">Search radius.</param>
        /// 
        public ExhaustiveBlockMatching( int blockSize, int searchRadius )
        {
            this.blockSize = blockSize;
            this.searchRadius = searchRadius;
        }

        /// <summary>
        /// Process images matching blocks between hem.
        /// </summary>
        /// 
        /// <param name="sourceImage">Source image with reference points.</param>
        /// <param name="coordinates">List of reference points to be matched.</param>
        /// <param name="searchImage">Image in which the reference points will be looked for.</param>
        /// 
        /// <returns>Returns list of found block matches. The list is sorted by similarity
        /// of found matches in descending order.</returns>
        /// 
        /// <exception cref="InvalidImagePropertiesException">Source and search images sizes must match.</exception>
        /// <exception cref="ArgumentException">Source images can be grayscale (8 bpp indexed) or color (24 bpp) image only.</exception>
        /// <exception cref="InvalidImagePropertiesException">Source and search images must have same pixel format.</exception>
        /// 
        public List<BlockMatch> ProcessImage( Bitmap sourceImage, List<IntPoint> coordinates, Bitmap searchImage )
        {
            // lock source image
            BitmapData sourceImageData = sourceImage.LockBits(
                new Rectangle( 0, 0, sourceImage.Width, sourceImage.Height ),
                ImageLockMode.ReadOnly, sourceImage.PixelFormat );

            BitmapData searchImageData = searchImage.LockBits(
                new Rectangle( 0, 0, searchImage.Width, searchImage.Height ),
                ImageLockMode.ReadOnly, searchImage.PixelFormat );

            List<BlockMatch> matchings;

            try
            {
                // process the image
                matchings = ProcessImage( new UnmanagedImage( sourceImageData ),
                    coordinates, new UnmanagedImage( searchImageData ) );
            }
            finally
            {
                // unlock image
                sourceImage.UnlockBits( sourceImageData );
                searchImage.UnlockBits( searchImageData );
            }

            return matchings;
        }

        /// <summary>
        /// Process images matching blocks between them.
        /// </summary>
        /// 
        /// <param name="sourceImageData">Source image with reference points.</param>
        /// <param name="coordinates">List of reference points to be matched.</param>
        /// <param name="searchImageData">Image in which the reference points will be looked for.</param>
        /// 
        /// <returns>Returns list of found block matches. The list is sorted by similarity
        /// of found matches in descending order.</returns>
        /// 
        /// <exception cref="InvalidImagePropertiesException">Source and search images sizes must match.</exception>
        /// <exception cref="UnsupportedImageFormatException">Source images can be grayscale (8 bpp indexed) or color (24 bpp) image only.</exception>
        /// <exception cref="ArgumentException">Source and search images must have same pixel format.</exception>
        /// 
        public List<BlockMatch> ProcessImage( BitmapData sourceImageData, List<IntPoint> coordinates, BitmapData searchImageData )
        {
            return ProcessImage( new UnmanagedImage( sourceImageData ), coordinates, new UnmanagedImage( searchImageData ) );
        }

        /// <summary>
        /// Process images matching blocks between them.
        /// </summary>
        /// 
        /// <param name="sourceImage">Source unmanaged image with reference points.</param>
        /// <param name="coordinates">List of reference points to be matched.</param>
        /// <param name="searchImage">Unmanaged image in which the reference points will be looked for.</param>
        /// 
        /// <returns>Returns list of found block matches. The list is sorted by similarity
        /// of found matches in descending order.</returns>
        /// 
        /// <exception cref="InvalidImagePropertiesException">Source and search images sizes must match.</exception>
        /// <exception cref="UnsupportedImageFormatException">Source images can be grayscale (8 bpp indexed) or color (24 bpp) image only.</exception>
        /// <exception cref="ArgumentException">Source and search images must have same pixel format.</exception>
        /// 
        public List<BlockMatch> ProcessImage( UnmanagedImage sourceImage, List<IntPoint> coordinates, UnmanagedImage searchImage )
        {
            // source images sizes must match.
            if ( ( sourceImage.Width != searchImage.Width ) || ( sourceImage.Height != searchImage.Height ) )
                throw new InvalidImagePropertiesException( "Source and search images sizes must match" );

            // sources images must be graysclae or color.
            if ( ( sourceImage.PixelFormat != PixelFormat.Format8bppIndexed ) && ( sourceImage.PixelFormat != PixelFormat.Format24bppRgb ) )
                throw new UnsupportedImageFormatException( "Source images can be graysclae (8 bpp indexed) or color (24 bpp) image only" );

            // source images must have the same pixel format.
            if ( sourceImage.PixelFormat != searchImage.PixelFormat )
                throw new InvalidImagePropertiesException( "Source and search images must have same pixel format" );

            int pointsCount = coordinates.Count;

            // found matches
            List<BlockMatch> matchingsList = new List<BlockMatch>( );

            // get source image size
            int width  = sourceImage.Width;
            int height = sourceImage.Height;
            int stride = sourceImage.Stride;
            int pixelSize = ( sourceImage.PixelFormat == PixelFormat.Format8bppIndexed ) ? 1 : 3;

            // pre-compute some values to avoid doing it in the loops.
            int blockRadius = blockSize / 2;
            int searchWindowSize = 2 * searchRadius;
            int blockLineSize = blockSize * pixelSize;
            int blockOffset = stride - ( blockSize * pixelSize );

            // maximum possible difference of blocks
            int maxDiff = blockSize * blockSize * pixelSize * 255;

            // integer similarity threshold
            int threshold = (int) ( similarityThreshold * maxDiff );

            // do the job
            unsafe
            {
                byte* ptrSource = (byte*) sourceImage.ImageData.ToPointer( );
                byte* ptrSearch = (byte*) searchImage.ImageData.ToPointer( );

                // for each point fed
                for ( int iPoint = 0; iPoint < pointsCount; iPoint++ )
                {
                    int refPointX = coordinates[iPoint].X;
                    int refPointY = coordinates[iPoint].Y;

                    // make sure the source block is inside the image
                    if (
                        ( ( refPointX - blockRadius < 0 ) || ( refPointX + blockRadius >= width ) ) ||
                        ( ( refPointY - blockRadius < 0 ) || ( refPointY + blockRadius >= height ) )
                        )
                    {
                        // skip point
                        continue;
                    }

                    // startting seatch point
                    int searchStartX = refPointX - blockRadius - searchRadius;
                    int searchStartY = refPointY - blockRadius - searchRadius;

                    // output match 
                    int bestMatchX = refPointX;
                    int bestMatchY = refPointY;

                    // Exhaustive Search Algorithm - we test each location within the search window
                    int minError = int.MaxValue;

                    // for each search window's row
                    for ( int searchWindowRow = 0; searchWindowRow < searchWindowSize; searchWindowRow++ )
                    {
                        if ( ( searchStartY + searchWindowRow < 0 ) || ( searchStartY + searchWindowRow + blockSize >= height ) )
                        {
                            // skip row
                            continue;
                        }

                        // for each search window's column
                        for ( int searchWindowCol = 0; searchWindowCol < searchWindowSize; searchWindowCol++ )
                        {
                            // tested block location in search image
                            int blockSearchX = searchStartX + searchWindowCol;
                            int blockSearchY = searchStartY + searchWindowRow;

                            if ( ( blockSearchX < 0 ) || ( blockSearchY + blockSize >= width ) )
                            {
                                // skip column
                                continue;
                            }

                            // get memory location of the block's upper left point in source and search images
                            byte* ptrSourceBlock = ptrSource + ( ( refPointY - blockRadius ) * stride ) + ( ( refPointX - blockRadius ) * pixelSize );
                            byte* ptrSearchBlock = ptrSearch + ( blockSearchY * stride ) + ( blockSearchX * pixelSize );

                            // navigate this block, accumulating the error
                            int error = 0;
                            for ( int blockRow = 0; blockRow < blockSize; blockRow++ )
                            {
                                for ( int blockCol = 0; blockCol < blockLineSize; blockCol++, ptrSourceBlock++, ptrSearchBlock++ )
                                {
                                    int diff = *ptrSourceBlock - *ptrSearchBlock;
                                    if ( diff > 0 )
                                    {
                                        error += diff;
                                    }
                                    else
                                    {
                                        error -= diff;
                                    }
                                }

                                // move to the next row
                                ptrSourceBlock += blockOffset;
                                ptrSearchBlock += blockOffset;
                            }

                            // check if the sum of error is mimimal
                            if ( error < minError )
                            {
                                minError = error;

                                // keep best match so far
                                bestMatchX = blockSearchX + blockRadius;
                                bestMatchY = blockSearchY + blockRadius;
                            }
                        }
                    }

                    // calculate blocks' similarity and compare it with threshold
                    int blockSimilarity = maxDiff - minError;

                    if ( blockSimilarity >= threshold )
                    {
                        matchingsList.Add( new BlockMatch(
                            new IntPoint( refPointX, refPointY ), new IntPoint( bestMatchX, bestMatchY ),
                            (float) blockSimilarity / maxDiff ) );
                    }
                }
            }

            // sort in descending order
            matchingsList.Sort( new MatchingsSorter( ) );

            return matchingsList;
        }

        // Sorter of found matchings
        private class MatchingsSorter : System.Collections.Generic.IComparer<BlockMatch>
        {
            public int Compare( BlockMatch x, BlockMatch y )
            {
                float diff = y.Similarity - x.Similarity;

                return ( diff > 0 ) ? 1 : ( diff < 0 ) ? -1 : 0;
            }
        }
    }
}
