// Accord Statistics Controls Library
// The Accord.NET Framework
// http://accord-framework.net
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

namespace Accord.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    /// <summary>
    ///   Color sequence for displaying categorical images.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       C.A. Glasbey, G.W.A.M. van der Heijden, V.F.K. Toh, A.J. Gray. (2007).
    ///   Color displays for categorical images, Color Research and Application, 32, 304-309
    ///   Available in: http://www.bioss.ac.uk/staff/chris/colorpaper.pdf </description></item>
    ///  </list></para>  
    /// </remarks>
    ///
    public class ColorSequenceCollection : IEnumerable<Color>
    {
        /// <summary>
        ///   Gets a list of the first 32 perceptually distinct
        ///   colors as detected in the investigation by [Glasbey et al].
        /// </summary>
        /// 
        private static readonly Color[] Colors = 
        {
            Color.FromArgb(255, 255, 255),   Color.FromArgb(  0,   0, 255),
            Color.FromArgb(255,   0,   0),   Color.FromArgb(  0, 255,   0),
            Color.FromArgb(  0,   0,  51),   Color.FromArgb(255,   0, 182),
            Color.FromArgb(  0,  83,   0),   Color.FromArgb(255, 211,   0),
            Color.FromArgb(  0, 159, 255),   Color.FromArgb(154,  77,  66),
            Color.FromArgb(  0, 255, 190),   Color.FromArgb(120,  63, 193),
            Color.FromArgb( 31, 150, 152),   Color.FromArgb(255, 172, 253),
            Color.FromArgb(177, 204, 113),   Color.FromArgb(241,   8,  92),
            Color.FromArgb(254, 143,  66),   Color.FromArgb(221,   0, 255),
            Color.FromArgb( 32,  26,   1),   Color.FromArgb(114,   0,  85),
            Color.FromArgb(118, 108, 149),   Color.FromArgb(  2, 173,  36),
            Color.FromArgb(200, 255,   0),   Color.FromArgb(136, 108,   0),
            Color.FromArgb(255, 183, 159),   Color.FromArgb(133, 133, 103),
            Color.FromArgb(161,   3,   0),   Color.FromArgb( 20, 249, 255),
            Color.FromArgb(  0,  71, 158),   Color.FromArgb(220,  94, 147),
            Color.FromArgb(147, 212, 255),   Color.FromArgb(  0,  76, 255)
        };


        private Color[] sequence;


        /// <summary>
        ///   Initializes a new instance of the <see cref="ColorSequenceCollection"/> class.
        /// </summary>
        /// 
        public ColorSequenceCollection()
            : this(Colors.Length - 1, true, false)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ColorSequenceCollection"/> class.
        /// </summary>
        /// 
        /// <param name="number">The number of colors to generate.</param>
        /// 
        public ColorSequenceCollection(int number)
            : this(number, true, false)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ColorSequenceCollection"/> class.
        /// </summary>
        /// <param name="number">The number of colors to generate.</param>
        /// <param name="skipWhite">If set to <c>true</c> white color is skipped.</param>
        /// <param name="random">If set to <c>true</c> generates a sequence of random colors.</param>
        /// 
        public ColorSequenceCollection(int number, bool skipWhite = true, bool random = false)
        {
            int start = (skipWhite) ? 1 : 0;

            if (number <= 0 || number >= Colors.Length - start + 1)
                throw new ArgumentOutOfRangeException("number", "The number of colors must be between 1 and 32.");

            sequence = new Color[number];

            if (random)
            {
                for (int i = 0; i < sequence.Length; i++)
                {
                    int j = Accord.Math.Random.Generator.Random.Next(1, Colors.Length);
                    sequence[i] = Colors[j];
                }
            }
            else
            {
                for (int i = 0; i < sequence.Length; i++)
                {
                    sequence[i] = Colors[i + 1];
                }
            }
        }

        /// <summary>
        ///   Gets the <see cref="System.Drawing.Color"/> with specified index.
        /// </summary>
        /// 
        public Color this[int i]
        {
            get
            {
                if (i < 0 || i >= Count)
                    throw new ArgumentOutOfRangeException("i",
                        "Index must be between 0 and " + Count + ".");

                return sequence[i];
            }
        }

        /// <summary>
        ///   Gets the <see cref="System.Drawing.Color"/> with specified index.
        /// </summary>
        /// 
        public Color GetColor(int i)
        {
            if (i < 0 || i >= Count)
                throw new ArgumentOutOfRangeException("i",
                    "Index must be between 0 and " + Count + ".");

            return sequence[i];
        }

        /// <summary>
        ///   Returns an enumerator that iterates through the color collection.
        /// </summary>
        /// <returns>
        ///   An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<Color> GetEnumerator()
        {
            for (int i = 0; i < sequence.Length; i++)
                yield return sequence[i];
            yield break;
        }

        /// <summary>
        ///   Returns an enumerator that iterates through the color collection.
        /// </summary>
        /// <returns>
        ///   An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///   Gets the number of colors in this sequence.
        /// </summary>
        /// 
        public int Count
        {
            get { return sequence.Length; }
        }
    }
}
