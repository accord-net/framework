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
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Texture tools.
    /// </summary>
    /// 
    /// <remarks><para>The class represents collection of different texture tools, like
    /// converting a texture to/from grayscale image.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create texture generator
    /// WoodTexture textureGenerator = new WoodTexture( );
    /// // generate new texture
    /// float[,] texture = textureGenerator.Generate( 320, 240 );
    /// // convert it to image to visualize
    /// Bitmap textureImage = TextureTools.ToBitmap( texture );
    /// </code>
    /// </remarks>
    /// 
    public class TextureTools
    {
        // Avoid class instantiation
        private TextureTools() { }

        /// <summary>
        /// Convert texture to grayscale bitmap.
        /// </summary>
        /// 
        /// <param name="texture">Texture to convert to bitmap.</param>
        /// 
        /// <returns>Returns bitmap of the texture.</returns>
        /// 
        public static Bitmap ToBitmap(float[,] texture)
        {
            // get texture dimension
            int width = texture.GetLength(1);
            int height = texture.GetLength(0);

            // create new grawscale image
            Bitmap dstImage = Accord.Imaging.Image.CreateGrayscaleImage(width, height);

            // lock destination bitmap data
            BitmapData dstData = dstImage.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

            // do the job
            unsafe
            {
                byte* dst = (byte*)dstData.Scan0.ToPointer();
                int offset = dstData.Stride - width;

                // for each line
                for (int y = 0; y < height; y++)
                {
                    // for each pixel
                    for (int x = 0; x < width; x++, dst++)
                    {
                        *dst = (byte)(texture[y, x] * 255.0f);
                    }
                    dst += offset;
                }
            }

            // unlock destination images
            dstImage.UnlockBits(dstData);

            return dstImage;
        }

        /// <summary>
        /// Convert grayscale bitmap to texture.
        /// </summary>
        /// 
        /// <param name="image">Image to convert to texture.</param>
        /// 
        /// <returns>Returns texture as 2D float array.</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Only grayscale (8 bpp indexed images) are supported.</exception>
        /// 
        public static float[,] FromBitmap(Bitmap image)
        {
            // lock source bitmap data
            BitmapData imageData = image.LockBits(
                new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, image.PixelFormat);

            // process the image
            float[,] texture = FromBitmap(imageData);

            // unlock source image
            image.UnlockBits(imageData);

            return texture;
        }

        /// <summary>
        /// Convert grayscale bitmap to texture
        /// </summary>
        /// 
        /// <param name="imageData">Image data to convert to texture</param>
        /// 
        /// <returns>Returns texture as 2D float array.</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Only grayscale (8 bpp indexed images) are supported.</exception>
        /// 
        public static float[,] FromBitmap(BitmapData imageData)
        {
            return FromBitmap(new UnmanagedImage(imageData));
        }

        /// <summary>
        /// Convert grayscale bitmap to texture.
        /// </summary>
        /// 
        /// <param name="image">Image data to convert to texture.</param>
        /// 
        /// <returns>Returns texture as 2D float array.</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Only grayscale (8 bpp indexed images) are supported.</exception>
        /// 
        public static float[,] FromBitmap(UnmanagedImage image)
        {
            // check source image
            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
                throw new UnsupportedImageFormatException("Only grayscale (8 bpp indexed images) are supported.");

            // get source image dimension
            int width = image.Width;
            int height = image.Height;

            // create texture array
            float[,] texture = new float[height, width];

            // do the job
            unsafe
            {
                byte* src = (byte*)image.ImageData.ToPointer();
                int offset = image.Stride - width;

                // for each line
                for (int y = 0; y < height; y++)
                {
                    // for each pixel
                    for (int x = 0; x < width; x++, src++)
                    {
                        texture[y, x] = (float)*src / 255.0f;
                    }
                    src += offset;
                }
            }

            return texture;
        }
    }
}