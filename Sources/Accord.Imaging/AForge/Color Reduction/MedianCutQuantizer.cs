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
    using System.Collections.Generic;
    using System.Drawing;
    using AForge.Imaging;
    
    /// <summary>
    /// Median cut color quantization algorithm.
    /// </summary>
    /// 
    /// <remarks><para>The class implements <a href="http://en.wikipedia.org/wiki/Median_cut">median cut</a>
    /// <a href="http://en.wikipedia.org/wiki/Median_cut">color quantization</a> algorithm.</para>
    /// 
    /// <para>See also <see cref="ColorImageQuantizer"/> class, which may simplify processing of images.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create the color quantization algorithm
    /// IColorQuantizer quantizer = new MedianCutQuantizer( );
    /// // process colors (taken from image for example)
    /// for ( int i = 0; i &lt; pixelsToProcess; i++ )
    /// {
    ///     quantizer.AddColor( /* pixel color */ );
    /// }
    /// // get palette reduced to 16 colors
    /// Color[] palette = quantizer.GetPalette( 16 );
    /// </code>
    /// </remarks>
    /// 
    /// <seealso cref="ColorImageQuantizer"/>
    /// 
    public class MedianCutQuantizer : IColorQuantizer
    {
        private List<Color> colors = new List<Color>( );

        /// <summary>
        /// Add color to the list of processed colors.
        /// </summary>
        /// 
        /// <param name="color">Color to add to the internal list.</param>
        /// 
        /// <remarks><para>The method adds the specified color into internal list of processed colors. The list
        /// is used later by <see cref="GetPalette"/> method to build reduced color table of the specified size.</para>
        /// </remarks>
        /// 
        public void AddColor( Color color )
        {
            colors.Add( color );
        }

        /// <summary>
        /// Get paletter of the specified size.
        /// </summary>
        /// 
        /// <param name="colorCount">Palette size to get.</param>
        /// 
        /// <returns>Returns reduced palette of the specified size, which covers colors processed so far.</returns>
        /// 
        /// <remarks><para>The method must be called after continuously calling <see cref="AddColor"/> method and
        /// returns reduced color palette for colors accumulated/processed so far.</para></remarks>
        /// 
        public Color[] GetPalette( int colorCount )
        {
            List<MedianCutCube> cubes = new List<MedianCutCube>( );
            cubes.Add( new MedianCutCube( colors ) );

            // split the cube until we get required amount of colors
            SplitCubes( cubes, colorCount );

            // get the final palette
            Color[] palette = new Color[colorCount];

            for ( int i = 0; i < colorCount; i++ )
            {
                palette[i] = cubes[i].Color;
            }

            return palette;
        }

        /// <summary>
        /// Clear internal state of the color quantization algorithm by clearing the list of colors
        /// so far processed.
        /// </summary>
        /// 
        public void Clear( )
        {
            colors.Clear( );
        }

        // Split specified list of cubes into smaller cubes until the list gets the specified size
        private void SplitCubes( List<MedianCutCube> cubes, int count )
        {
            int cubeIndexToSplit = cubes.Count - 1;

            while ( cubes.Count < count )
            {
                MedianCutCube cubeToSplit = cubes[cubeIndexToSplit];
                MedianCutCube cube1, cube2;

                // find the longest color size to use for splitting
                if ( ( cubeToSplit.RedSize >= cubeToSplit.GreenSize ) && ( cubeToSplit.RedSize >= cubeToSplit.BlueSize ) )
                {
                    cubeToSplit.SplitAtMedian( RGB.R, out cube1, out cube2 );
                }
                else if ( cubeToSplit.GreenSize >= cubeToSplit.BlueSize )
                {
                    cubeToSplit.SplitAtMedian( RGB.G, out cube1, out cube2 );
                }
                else
                {
                    cubeToSplit.SplitAtMedian( RGB.B, out cube1, out cube2 );
                }

                // remove the old "big" cube
                cubes.RemoveAt( cubeIndexToSplit );
                // add two smaller cubes instead
                cubes.Insert( cubeIndexToSplit, cube1 );
                cubes.Insert( cubeIndexToSplit, cube2 );

                if ( --cubeIndexToSplit < 0 )
                {
                    cubeIndexToSplit = cubes.Count - 1;
                }
            }
        }
    }
}
