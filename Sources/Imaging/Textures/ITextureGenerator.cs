// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
//

namespace AForge.Imaging.Textures
{
    using System;

    /// <summary>
    /// Texture generator interface.
    /// </summary>
    /// 
    /// <remarks><para>Each texture generator generates a 2-D texture of the specified size and returns
    /// it as two dimensional array of intensities in the range of [0, 1] - texture's values.</para>
    /// </remarks>
    /// 
    public interface ITextureGenerator
    {
        /// <summary>
        /// Generate texture.
        /// </summary>
        /// 
        /// <param name="width">Texture's width.</param>
        /// <param name="height">Texture's height.</param>
        /// 
        /// <returns>Two dimensional array of texture's intensities.</returns>
        /// 
        /// <remarks>Generates new texture of the specified size.</remarks>
        /// 
        float[,] Generate( int width, int height );

        /// <summary>
        /// Reset generator.
        /// </summary>
        /// 
        /// <remarks>Resets the generator - resets all internal variables, regenerates
        /// internal random numbers, etc.</remarks>
        /// 
        void Reset( );
    }
}
