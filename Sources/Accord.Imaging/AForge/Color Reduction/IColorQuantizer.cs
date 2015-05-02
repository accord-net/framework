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
    using AForge.Imaging;

    /// <summary>
    /// Interface which is implemented by different color quantization algorithms.
    /// </summary>
    /// 
    /// <remarks><para>The interface defines set of methods, which are to be implemented by different
    /// color quantization algorithms - algorithms which are aimed to provide reduced color table/palette
    /// for a color image.</para>
    /// 
    /// <para>See documentation to particular implementation of the interface for additional information
    /// about the algorithm.</para>
    /// </remarks>
    /// 
    public interface IColorQuantizer
    {
        /// <summary>
        /// Process color by a color quantization algorithm.
        /// </summary>
        /// 
        /// <param name="color">Color to process.</param>
        /// 
        /// <remarks><para>Depending on particular implementation of <see cref="IColorQuantizer"/> interface,
        /// this method may simply process the specified color or store it in internal list for
        /// later color palette calculation.</para></remarks>
        /// 
        void AddColor( Color color );

        /// <summary>
        /// Get palette of the specified size.
        /// </summary>
        /// 
        /// <param name="colorCount">Palette size to return.</param>
        /// 
        /// <returns>Returns reduced color palette for the accumulated/processed colors.</returns>
        /// 
        /// <remarks><para>The method must be called after continuously calling <see cref="AddColor"/> method and
        /// returns reduced color palette for colors accumulated/processed so far.</para></remarks>
        ///
        Color[] GetPalette( int colorCount );

        /// <summary>
        /// Clear internals of the algorithm, like accumulated color table, etc.
        /// </summary>
        /// 
        /// <remarks><para>The methods resets internal state of a color quantization algorithm returning
        /// it to initial state.</para></remarks>
        /// 
        void Clear( );
    }
}
