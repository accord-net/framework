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
    using Accord.Math;
    using System;

    /// <summary>
    /// Textile texture.
    /// </summary>
    /// 
    /// <remarks><para>The texture generator creates textures with effect of textile.</para>
    /// 
    /// <para>The generator is based on the <see cref="Accord.Math.PerlinNoise">Perlin noise function</see>.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create texture generator
    /// TextileTexture textureGenerator = new TextileTexture( );
    /// // generate new texture
    /// float[,] texture = textureGenerator.Generate( 320, 240 );
    /// // convert it to image to visualize
    /// Bitmap textureImage = TextureTools.ToBitmap( texture );
    /// </code>
    ///
    /// <para><b>Result image:</b></para>
    /// <img src="..\images\imaging\textile_texture.jpg" width="320" height="240" />
    /// </remarks>
    /// 
    public class TextileTexture : BaseTextureGenerator, ITextureGenerator
    {
        // Perlin noise function used for texture generation
        private PerlinNoise noise = new PerlinNoise(3, 0.65, 1.0 / 8, 1.0);

        /// <summary>
        /// Initializes a new instance of the <see cref="TextileTexture"/> class.
        /// </summary>
        /// 
        public TextileTexture()
        {
        }

        /// <summary>
        /// Generate texture.
        /// </summary>
        /// 
        /// <param name="width">Texture's width.</param>
        /// <param name="height">Texture's height.</param>
        /// 
        /// <returns>Two dimensional array of intensities.</returns>
        /// 
        /// <remarks>Generates new texture of the specified size.</remarks>
        /// 
        public override float[,] Generate(int width, int height)
        {
            var texture = new float[height, width];

            int r = Accord.Math.Random.Generator.Random.Next(5000);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float a = (float)Math.Sin(x + noise.Function2D(x + r, y + r));
                    float b = (float)Math.Sin(y + noise.Function2D(x + r, y + r));
                    texture[y, x] = Math.Max(0.0f, Math.Min(1.0f, (a + b) * 0.25f + 0.5f));

                }
            }

            return texture;
        }

    }
}
