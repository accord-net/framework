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
    using System.Drawing;

    /// <summary>
    ///   RGB components.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The class encapsulates <b>RGB</b> color components and can be used to implement
    ///   logic for reading, writing and converting to and from RGB color representations.</para>
    /// <para>
    ///   <note>The <see cref="System.Drawing.Imaging.PixelFormat">PixelFormat.Format24bppRgb</see>
    ///   actually refers to a BGR pixel format.</note></para>
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
    /// <seealso cref="YCbCr"/>
    /// 
    [Serializable]
    public struct RGB
    {
        /// <summary>
        /// Index of red component.
        /// </summary>
        public const short R = 2;

        /// <summary>
        /// Index of green component.
        /// </summary>
        public const short G = 1;

        /// <summary>
        /// Index of blue component.
        /// </summary>
        public const short B = 0;

        /// <summary>
        /// Index of alpha component for ARGB images.
        /// </summary>
        public const short A = 3;

        /// <summary>
        /// Red component.
        /// </summary>
        public byte Red;

        /// <summary>
        /// Green component.
        /// </summary>
        public byte Green;

        /// <summary>
        /// Blue component.
        /// </summary>
        public byte Blue;

        /// <summary>
        /// Alpha component.
        /// </summary>
        public byte Alpha;

        /// <summary>
        /// <see cref="System.Drawing.Color">Color</see> value of the class.
        /// </summary>
        public System.Drawing.Color Color
        {
            get { return Color.FromArgb(Alpha, Red, Green, Blue); }
            set
            {
                Red = value.R;
                Green = value.G;
                Blue = value.B;
                Alpha = value.A;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RGB"/> class.
        /// </summary>
        /// 
        /// <param name="red">Red component.</param>
        /// <param name="green">Green component.</param>
        /// <param name="blue">Blue component.</param>
        /// 
        public RGB(byte red, byte green, byte blue)
        {
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
            this.Alpha = 255;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RGB"/> class.
        /// </summary>
        /// 
        /// <param name="red">Red component.</param>
        /// <param name="green">Green component.</param>
        /// <param name="blue">Blue component.</param>
        /// <param name="alpha">Alpha component.</param>
        /// 
        public RGB(byte red, byte green, byte blue, byte alpha)
        {
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
            this.Alpha = alpha;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RGB"/> class.
        /// </summary>
        /// 
        /// <param name="color">Initialize from specified <see cref="System.Drawing.Color">color.</see></param>
        /// 
        public RGB(System.Drawing.Color color)
        {
            this.Red = color.R;
            this.Green = color.G;
            this.Blue = color.B;
            this.Alpha = color.A;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="RGB"/> to <see cref="HSL"/>.
        /// </summary>
        /// <param name="rgb">The RGB color.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator HSL(RGB rgb)
        {
            return HSL.FromRGB(rgb);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="RGB"/> to <see cref="YCbCr"/>.
        /// </summary>
        /// <param name="rgb">The RGB color.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator YCbCr(RGB rgb)
        {
            return YCbCr.FromRGB(rgb);
        }
    }
}
