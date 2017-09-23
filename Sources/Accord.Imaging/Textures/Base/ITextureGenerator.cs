// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace Accord.Imaging.Textures
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
        float[,] Generate(int width, int height);

        /// <summary>
        /// Reset generator.
        /// </summary>
        /// 
        /// <remarks>Resets the generator - resets all internal variables, regenerates
        /// internal random numbers, etc.</remarks>
        /// 
        [Obsolete("The texture generators now use a different seed at each call to Generate.")]
        void Reset();
    }
}
