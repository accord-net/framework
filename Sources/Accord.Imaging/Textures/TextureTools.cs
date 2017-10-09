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
    using Accord.Imaging.Converters;
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    ///   Obsolete. Please use classes from the Accord.Imaging.Converters namespace instead.
    /// </summary>
    /// 
    [Obsolete("Please use classes from the Accord.Imaging.Converters namespace instead.")]
    public static class TextureTools
    {

        /// <summary>
        ///   Obsolete. Please use the <see cref="ImageToMatrix"/> class instead. See remarks for an example.
        /// </summary>
        /// 
        /// <example>
        /// <code>
        ///   MatrixToImage i2m = new MatrixToImage();
        ///   Bitmap image;
        ///   i2m.Convert(texture, out image);
        ///   return image;
        /// </code>
        /// </example>
        /// 
        [Obsolete("Please use the MatrixToImage class instead.")]
        public static Bitmap ToBitmap(float[,] texture)
        {
            MatrixToImage i2m = new MatrixToImage();
            Bitmap image;
            i2m.Convert(texture, out image);
            return image;
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="ImageToMatrix"/> class instead. See remarks for an example.
        /// </summary>
        /// 
        /// <example>
        /// <code>
        ///   ImageToMatrix i2m = new ImageToMatrix();
        ///   float[,] texture;
        ///   i2m.Convert(image, out texture);
        ///   return texture;
        /// </code>
        /// </example>
        /// 
        [Obsolete("Please use the ImageToMatrix class instead.")]
        public static float[,] FromBitmap(Bitmap image)
        {
            ImageToMatrix i2m = new ImageToMatrix();
            float[,] texture;
            i2m.Convert(image, out texture);
            return texture;
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="ImageToMatrix"/> class instead. See remarks for an example.
        /// </summary>
        /// 
        /// <example>
        /// <code>
        ///   ImageToMatrix i2m = new ImageToMatrix();
        ///   float[,] texture;
        ///   i2m.Convert(image, out texture);
        ///   return texture;
        /// </code>
        /// </example>
        /// 
        [Obsolete("Please use the ImageToMatrix class instead.")]
        public static float[,] FromBitmap(BitmapData imageData)
        {
            return FromBitmap(new UnmanagedImage(imageData));
        }

        /// <summary>
        ///   Obsolete. Please use the <see cref="ImageToMatrix"/> class instead. See remarks for an example.
        /// </summary>
        /// 
        /// <example>
        /// <code>
        ///   ImageToMatrix i2m = new ImageToMatrix();
        ///   float[,] texture;
        ///   i2m.Convert(image, out texture);
        ///   return texture;
        /// </code>
        /// </example>
        /// 
        [Obsolete("Please use the ImageToMatrix class instead.")]
        public static float[,] FromBitmap(UnmanagedImage image)
        {
            ImageToMatrix i2m = new ImageToMatrix();
            float[,] texture;
            i2m.Convert(image, out texture);
            return texture;
        }
    }
}