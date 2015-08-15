// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2010
// contacts@aforgenet.com
//

namespace AForge.Imaging.ColorReduction
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Base class for error diffusion color dithering, where error is diffused to 
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
    /// <para>The image processing routine accepts 24/32 bpp color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create dithering routine
    /// ColorErrorDiffusionToAdjacentNeighbors dithering = new ColorErrorDiffusionToAdjacentNeighbors(
    ///     new int[3][] {
    ///         new int[2] { 5, 3 },
    ///         new int[5] { 2, 4, 5, 4, 2 },
    ///         new int[3] { 2, 3, 2 }
    ///     } );
    /// // apply the dithering routine
    /// Bitmap newImage = dithering.Apply( image );
    /// </code>
    /// </remarks>
    /// 
    public class ColorErrorDiffusionToAdjacentNeighbors : ErrorDiffusionColorDithering
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
        /// Initializes a new instance of the <see cref="ColorErrorDiffusionToAdjacentNeighbors"/> class.
        /// </summary>
        /// 
        /// <param name="coefficients">Diffusion coefficients (see <see cref="ColorErrorDiffusionToAdjacentNeighbors"/>
        /// for more information).</param>
        /// 
        public ColorErrorDiffusionToAdjacentNeighbors( int[][] coefficients )
        {
            this.coefficients = coefficients;
            CalculateCoefficientsSum( );
        }

        /// <summary>
        /// Do error diffusion.
        /// </summary>
        /// 
        /// <param name="rError">Error value of red component.</param>
        /// <param name="gError">Error value of green component.</param>
        /// <param name="bError">Error value of blue component.</param>
        /// <param name="ptr">Pointer to current processing pixel.</param>
        /// 
        /// <remarks>All parameters of the image and current processing pixel's coordinates
        /// are initialized by base class.</remarks>
        /// 
        protected override unsafe void Diffuse( int rError, int gError, int bError, byte* ptr )
        {
            int edR;	// error diffusion
            int edG;	// error diffusion
            int edB;	// error diffusion

            // do error diffusion to right-standing neighbors
            int[] coefficientsRow = coefficients[0];

            for ( int jI = 1, jP = pixelSize, jC = 0, k = coefficientsRow.Length; jC < k; jI++, jC++, jP += pixelSize )
            {
                if ( x + jI >= width )
                    break;

                edR = ptr[jP + RGB.R] + ( rError * coefficientsRow[jC] ) / coefficientsSum;
                edR = ( edR < 0 ) ? 0 : ( ( edR > 255 ) ? 255 : edR );
                ptr[jP + RGB.R] = (byte) edR;

                edG = ptr[jP + RGB.G] + ( gError * coefficientsRow[jC] ) / coefficientsSum;
                edG = ( edG < 0 ) ? 0 : ( ( edG > 255 ) ? 255 : edG );
                ptr[jP + RGB.G] = (byte) edG;

                edB = ptr[jP + RGB.B] + ( bError * coefficientsRow[jC] ) / coefficientsSum;
                edB = ( edB < 0 ) ? 0 : ( ( edB > 255 ) ? 255 : edB );
                ptr[jP + RGB.B] = (byte) edB;
            }

            // do error diffusion to bottom neigbors
            for ( int i = 1, n = coefficients.Length; i < n; i++ )
            {
                if ( y + i >= height )
                    break;

                // move pointer to next image line
                ptr += stride;

                // get coefficients of the row
                coefficientsRow = coefficients[i];

                // process the row
                for ( int jC = 0, k = coefficientsRow.Length, jI = -( k >> 1 ), jP = -( k >> 1 ) * pixelSize; jC < k; jI++, jC++, jP += pixelSize )
                {
                    if ( x + jI >= width )
                        break;
                    if ( x + jI < 0 )
                        continue;

                    edR = ptr[jP + RGB.R] + ( rError * coefficientsRow[jC] ) / coefficientsSum;
                    edR = ( edR < 0 ) ? 0 : ( ( edR > 255 ) ? 255 : edR );
                    ptr[jP + RGB.R] = (byte) edR;

                    edG = ptr[jP + RGB.G] + ( gError * coefficientsRow[jC] ) / coefficientsSum;
                    edG = ( edG < 0 ) ? 0 : ( ( edG > 255 ) ? 255 : edG );
                    ptr[jP + RGB.G] = (byte) edG;

                    edB = ptr[jP + RGB.B] + ( bError * coefficientsRow[jC] ) / coefficientsSum;
                    edB = ( edB < 0 ) ? 0 : ( ( edB > 255 ) ? 255 : edB );
                    ptr[jP + RGB.B] = (byte) edB;

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
