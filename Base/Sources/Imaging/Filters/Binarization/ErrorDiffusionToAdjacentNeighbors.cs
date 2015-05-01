// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2008
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Base class for error diffusion dithering, where error is diffused to 
    /// adjacent neighbor pixels.
    /// </summary>
    /// 
    /// <remarks><para>The class does error diffusion to adjacent neighbor pixels
    /// using specified set of coefficients. These coefficients are represented by
    /// 2 dimensional jugged array, where first array of coefficients is for
    /// right-standing pixels, but the rest of arrays are for bottom-standing pixels.
    /// All arrays except the first one should have odd number of coefficients.</para>
    /// 
    /// <para>Suppose that error diffusion coefficients are represented by the next
    /// jugged array:</para>
    /// 
    /// <code>
    /// int[][] coefficients = new int[2][] {
    ///     new int[1] { 7 },
    ///     new int[3] { 3, 5, 1 }
    /// };
    /// </code>
    /// 
    /// <para>The above coefficients are used to diffuse error over the next neighbor
    /// pixels (<b>*</b> marks current pixel, coefficients are placed to corresponding
    /// neighbor pixels):</para>
    /// <code lang="none">
    ///     | * | 7 |
    /// | 3 | 5 | 1 |
    /// 
    /// / 16
    /// </code>
    /// 
    /// <para>The filter accepts 8 bpp grayscale images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// ErrorDiffusionToAdjacentNeighbors filter = new ErrorDiffusionToAdjacentNeighbors(
    ///     new int[3][] {
    ///         new int[2] { 5, 3 },
    ///         new int[5] { 2, 4, 5, 4, 2 },
    ///         new int[3] { 2, 3, 2 }
    ///     } );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// </remarks>
    /// 
    public class ErrorDiffusionToAdjacentNeighbors : ErrorDiffusionDithering
    {
        // diffusion coefficients
        private int[][] coefficients;
        // sum of all coefficients
        private int coefficientsSum;

        /// <summary>
        /// Diffusion coefficients.
        /// </summary>
        /// 
        /// <remarks>Set of coefficients, which are used for error diffusion to
        /// pixel's neighbors.</remarks>
        /// 
        public int[][] Coefficients
        {
            get { return coefficients; }
            set
            {
                coefficients = value;
                CalculateCoefficientsSum( );
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorDiffusionToAdjacentNeighbors"/> class.
        /// </summary>
        /// 
        /// <param name="coefficients">Diffusion coefficients.</param>
        /// 
        public ErrorDiffusionToAdjacentNeighbors( int[][] coefficients )
        {
            this.coefficients = coefficients;
            CalculateCoefficientsSum( );
        }

        /// <summary>
        /// Do error diffusion.
        /// </summary>
        /// 
        /// <param name="error">Current error value.</param>
        /// <param name="ptr">Pointer to current processing pixel.</param>
        /// 
        /// <remarks>All parameters of the image and current processing pixel's coordinates
        /// are initialized by base class.</remarks>
        /// 
        protected override unsafe void Diffuse( int error, byte* ptr )
        {
            int ed;	// error diffusion

            // do error diffusion to right-standing neighbors
            int[] coefficientsRow = coefficients[0];

            for ( int jI = 1, jC = 0, k = coefficientsRow.Length; jC < k; jI++, jC++ )
            {
                if ( x + jI >= stopX )
                    break;

                ed = ptr[jI] + ( error * coefficientsRow[jC] ) / coefficientsSum;
                ed = ( ed < 0 ) ? 0 : ( ( ed > 255 ) ? 255 : ed );
                ptr[jI] = (byte) ed;
            }

            // do error diffusion to bottom neigbors
            for ( int i = 1, n = coefficients.Length; i < n; i++ )
            {
                if ( y + i >= stopY )
                    break;

                // move pointer to next image line
                ptr += stride;

                // get coefficients of the row
                coefficientsRow = coefficients[i];

                // process the row
                for ( int jC = 0, k = coefficientsRow.Length, jI = -( k >> 1 ); jC < k; jI++, jC++ )
                {
                    if ( x + jI >= stopX )
                        break;
                    if ( x + jI < startX )
                        continue;

                    ed = ptr[jI] + ( error * coefficientsRow[jC] ) / coefficientsSum;
                    ed = ( ed < 0 ) ? 0 : ( ( ed > 255 ) ? 255 : ed );
                    ptr[jI] = (byte) ed;
                }
            }
        }

        #region Private Members

        // Calculate coefficients' sum
        private void CalculateCoefficientsSum( )
        {
            coefficientsSum = 0;

            for ( int i = 0, n = coefficients.Length; i < n; i++ )
            {
                int[] coefficientsRow = coefficients[i];

                for ( int j = 0, k = coefficientsRow.Length; j < k; j++ )
                {
                    coefficientsSum += coefficientsRow[j];
                }
            }
        }

        #endregion
    }
}
