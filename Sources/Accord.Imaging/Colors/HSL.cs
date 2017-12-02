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
    ///   HSL components.
    /// </summary>
    /// 
    /// <remarks>
    ///   The class encapsulates <b>HSL</b> color components and can be used to implement
    ///   logic for reading, writing and converting to and from HSL color representations.
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
    /// <seealso cref="RGB"/>
    /// <seealso cref="YCbCr"/>
    /// 
    [Serializable]
    public struct HSL
    {
        /// <summary>
        /// Hue component.
        /// </summary>
        /// 
        /// <remarks>Hue is measured in the range of [0, 359].</remarks>
        /// 
        public int Hue;

        /// <summary>
        /// Saturation component.
        /// </summary>
        /// 
        /// <remarks>Saturation is measured in the range of [0, 1].</remarks>
        /// 
        public float Saturation;

        /// <summary>
        /// Luminance value.
        /// </summary>
        /// 
        /// <remarks>Luminance is measured in the range of [0, 1].</remarks>
        /// 
        public float Luminance;

        /// <summary>
        /// Initializes a new instance of the <see cref="HSL"/> class.
        /// </summary>
        /// 
        /// <param name="hue">Hue component.</param>
        /// <param name="saturation">Saturation component.</param>
        /// <param name="luminance">Luminance component.</param>
        /// 
        public HSL(int hue, float saturation, float luminance)
        {
            this.Hue = hue;
            this.Saturation = saturation;
            this.Luminance = luminance;
        }

        /// <summary>
        /// Convert from RGB to HSL color space.
        /// </summary>
        /// 
        /// <param name="rgb">Source color in <b>RGB</b> color space.</param>
        /// <param name="hsl">Destination color in <b>HSL</b> color space.</param>
        /// 
        /// <remarks><para>See <a href="http://en.wikipedia.org/wiki/HSI_color_space#Conversion_from_RGB_to_HSL_or_HSV">HSL and HSV Wiki</a>
        /// for information about the algorithm to convert from RGB to HSL.</para></remarks>
        /// 
        public static void FromRGB(RGB rgb, ref HSL hsl)
        {
            float r = (rgb.Red / 255.0f);
            float g = (rgb.Green / 255.0f);
            float b = (rgb.Blue / 255.0f);

            float min = Math.Min(Math.Min(r, g), b);
            float max = Math.Max(Math.Max(r, g), b);
            float delta = max - min;

            // get luminance value
            hsl.Luminance = (max + min) / 2;

            if (delta == 0)
            {
                // gray color
                hsl.Hue = 0;
                hsl.Saturation = 0.0f;
            }
            else
            {
                // get saturation value
                hsl.Saturation = (hsl.Luminance <= 0.5) ? (delta / (max + min)) : (delta / (2 - max - min));

                // get hue value
                float hue;

                if (r == max)
                {
                    hue = ((g - b) / 6) / delta;
                }
                else if (g == max)
                {
                    hue = (1.0f / 3) + ((b - r) / 6) / delta;
                }
                else
                {
                    hue = (2.0f / 3) + ((r - g) / 6) / delta;
                }

                // correct hue if needed
                if (hue < 0)
                    hue += 1;
                if (hue > 1)
                    hue -= 1;

                hsl.Hue = (int)(hue * 360);
            }
        }

        /// <summary>
        /// Convert from RGB to HSL color space.
        /// </summary>
        /// 
        /// <param name="rgb">Source color in <b>RGB</b> color space.</param>
        /// 
        /// <returns>Returns <see cref="HSL"/> instance, which represents converted color value.</returns>
        /// 
        public static HSL FromRGB(RGB rgb)
        {
            HSL hsl = new HSL();
            FromRGB(rgb, ref hsl);
            return hsl;
        }

        /// <summary>
        /// Convert from HSL to RGB color space.
        /// </summary>
        /// 
        /// <param name="hsl">Source color in <b>HSL</b> color space.</param>
        /// <param name="rgb">Destination color in <b>RGB</b> color space.</param>
        /// 
        public static void ToRGB(HSL hsl, ref RGB rgb)
        {
            if (hsl.Saturation == 0)
            {
                // gray values
                rgb.Red = rgb.Green = rgb.Blue = (byte)(hsl.Luminance * 255);
            }
            else
            {
                float v1, v2;
                float hue = (float)hsl.Hue / 360;

                v2 = (hsl.Luminance < 0.5) ?
                    (hsl.Luminance * (1 + hsl.Saturation)) :
                    ((hsl.Luminance + hsl.Saturation) - (hsl.Luminance * hsl.Saturation));
                v1 = 2 * hsl.Luminance - v2;

                rgb.Red = (byte)(255 * Hue_2_RGB(v1, v2, hue + (1.0f / 3)));
                rgb.Green = (byte)(255 * Hue_2_RGB(v1, v2, hue));
                rgb.Blue = (byte)(255 * Hue_2_RGB(v1, v2, hue - (1.0f / 3)));
            }
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
        /// Performs an explicit conversion from <see cref="HSL"/> to <see cref="RGB"/>.
        /// </summary>
        /// <param name="hsl">The HSL color.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator RGB(HSL hsl)
        {
            return hsl.ToRGB();
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="HSL"/> to <see cref="YCbCr"/>.
        /// </summary>
        /// <param name="hsl">The HSL color.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator YCbCr(HSL hsl)
        {
            return YCbCr.FromRGB(hsl.ToRGB());
        }

        #region Private members
        // HSL to RGB helper routine
        private static float Hue_2_RGB(float v1, float v2, float vH)
        {
            if (vH < 0)
                vH += 1;
            if (vH > 1)
                vH -= 1;
            if ((6 * vH) < 1)
                return (v1 + (v2 - v1) * 6 * vH);
            if ((2 * vH) < 1)
                return v2;
            if ((3 * vH) < 2)
                return (v1 + (v2 - v1) * ((2.0f / 3) - vH) * 6);
            return v1;
        }
        #endregion
    }
}
