// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2011
// contacts@aforgenet.com
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

namespace Accord.Imaging
{
    using System;
    /// <summary>
    ///   YCbCr components.
    /// </summary>
    /// 
    /// <remarks>
    ///   The class encapsulates <b>YCbCr</b> color components and can be used to implement
    ///   logic for reading, writing and converting to and from YCbCr color representations.
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   The following examples show how to convert to and from various pixel representations:</para>
    /// <code source="Unit Tests\Accord.Tests.Imaging\Colors\ColorsTest.cs" region="doc_rgb" />
    /// <code source="Unit Tests\Accord.Tests.Imaging\Colors\ColorsTest.cs" region="doc_hsl" />
    /// <code source="Unit Tests\Accord.Tests.Imaging\Colors\ColorsTest.cs" region="doc_ycbcr" />
    /// </example>
    /// 
    /// <seealso cref="HSL"/>
    /// <seealso cref="RGB"/>
    /// 
    [Serializable]
    public struct YCbCr
    {
        /// <summary>
        /// Index of <b>Y</b> component.
        /// </summary>
        public const short YIndex = 0;

        /// <summary>
        /// Index of <b>Cb</b> component.
        /// </summary>
        public const short CbIndex = 1;

        /// <summary>
        /// Index of <b>Cr</b> component.
        /// </summary>
        public const short CrIndex = 2;

        /// <summary>
        /// <b>Y</b> component.
        /// </summary>
        public float Y;

        /// <summary>
        /// <b>Cb</b> component.
        /// </summary>
        public float Cb;

        /// <summary>
        /// <b>Cr</b> component.
        /// </summary>
        public float Cr;

        /// <summary>
        /// Initializes a new instance of the <see cref="YCbCr"/> class.
        /// </summary>
        /// 
        /// <param name="y"><b>Y</b> component.</param>
        /// <param name="cb"><b>Cb</b> component.</param>
        /// <param name="cr"><b>Cr</b> component.</param>
        /// 
        public YCbCr(float y, float cb, float cr)
        {
            this.Y = Math.Max(0.0f, Math.Min(1.0f, y));
            this.Cb = Math.Max(-0.5f, Math.Min(0.5f, cb));
            this.Cr = Math.Max(-0.5f, Math.Min(0.5f, cr));
        }

        /// <summary>
        /// Convert from RGB to YCbCr color space (Rec 601-1 specification). 
        /// </summary>
        /// 
        /// <param name="rgb">Source color in <b>RGB</b> color space.</param>
        /// <param name="ycbcr">Destination color in <b>YCbCr</b> color space.</param>
        /// 
        public static void FromRGB(RGB rgb, ref YCbCr ycbcr)
        {
            float r = (float)rgb.Red / 255;
            float g = (float)rgb.Green / 255;
            float b = (float)rgb.Blue / 255;

            ycbcr.Y = (float)(0.2989 * r + 0.5866 * g + 0.1145 * b);
            ycbcr.Cb = (float)(-0.1687 * r - 0.3313 * g + 0.5000 * b);
            ycbcr.Cr = (float)(0.5000 * r - 0.4184 * g - 0.0816 * b);
        }

        /// <summary>
        /// Convert from RGB to YCbCr color space (Rec 601-1 specification).
        /// </summary>
        /// 
        /// <param name="rgb">Source color in <b>RGB</b> color space.</param>
        /// 
        /// <returns>Returns <see cref="YCbCr"/> instance, which represents converted color value.</returns>
        /// 
        public static YCbCr FromRGB(RGB rgb)
        {
            YCbCr ycbcr = new YCbCr();
            FromRGB(rgb, ref ycbcr);
            return ycbcr;
        }

        /// <summary>
        /// Convert from YCbCr to RGB color space.
        /// </summary>
        /// 
        /// <param name="ycbcr">Source color in <b>YCbCr</b> color space.</param>
        /// <param name="rgb">Destination color in <b>RGB</b> color space.</param>
        /// 
        public static void ToRGB(YCbCr ycbcr, ref RGB rgb)
        {
            // Don't worry about zeros. Compiler will remove them
            float r = Math.Max(0.0f, Math.Min(1.0f, (float)(ycbcr.Y + 0.0000 * ycbcr.Cb + 1.4022 * ycbcr.Cr)));
            float g = Math.Max(0.0f, Math.Min(1.0f, (float)(ycbcr.Y - 0.3456 * ycbcr.Cb - 0.7145 * ycbcr.Cr)));
            float b = Math.Max(0.0f, Math.Min(1.0f, (float)(ycbcr.Y + 1.7710 * ycbcr.Cb + 0.0000 * ycbcr.Cr)));

            rgb.Red = (byte)(r * 255);
            rgb.Green = (byte)(g * 255);
            rgb.Blue = (byte)(b * 255);
            rgb.Alpha = 255;
        }

        /// <summary>
        /// Convert the color to <b>RGB</b> color space.
        /// </summary>
        /// 
        /// <returns>Returns <see cref="RGB"/> instance, which represents converted color value.</returns>
        /// 
        public RGB ToRGB()
        {
            RGB rgb = new RGB();
            ToRGB(this, ref rgb);
            return rgb;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="YCbCr"/> to <see cref="RGB"/>.
        /// </summary>
        /// <param name="yCbCr">The YCbCr color.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator RGB(YCbCr yCbCr)
        {
            return yCbCr.ToRGB();
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="YCbCr"/> to <see cref="HSL"/>.
        /// </summary>
        /// <param name="yCbCr">The YCbCr color.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator HSL(YCbCr yCbCr)
        {
            return HSL.FromRGB(yCbCr.ToRGB());
        }
    }
}
