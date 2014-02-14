// Accord Vision Library
// The Accord.NET Framework (LGPL)
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
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

namespace Accord.Vision.Detection
{
    using System;
    using System.Globalization;

    /// <summary>
    ///   Scalable rectangular area.
    /// </summary>
    /// 
    /// <remarks>
    ///   A rectangle which can be at any position and scale within the original image.
    /// </remarks>
    /// 
    [Serializable]
    public class HaarRectangle : ICloneable
    {

        /// <summary>
        ///   Gets or sets the x-coordinate of this Haar feature rectangle.
        /// </summary>
        /// 
        public int X { get; set; }

        /// <summary>
        ///   Gets or sets the y-coordinate of this Haar feature rectangle.
        /// </summary>
        /// 
        public int Y { get; set; }

        /// <summary>
        ///   Gets or sets the width of this Haar feature rectangle.
        /// </summary>
        /// 
        public int Width { get; set; }

        /// <summary>
        ///   Gets or sets the height of this Haar feature rectangle.
        /// </summary>
        /// 
        public int Height { get; set; }

        /// <summary>
        ///   Gets or sets the weight of this Haar feature rectangle.
        /// </summary>
        /// 
        public float Weight { get; set; }


        /// <summary>
        ///   Gets or sets the scaled x-coordinate of this Haar feature rectangle.
        /// </summary>
        /// 
        public int ScaledX { get; set; }

        /// <summary>
        ///   Gets or sets the scaled y-coordinate of this Haar feature rectangle.
        /// </summary>
        /// 
        public int ScaledY { get; set; }

        /// <summary>
        ///   Gets or sets the scaled width of this Haar feature rectangle.
        /// </summary>
        /// 
        public int ScaledWidth { get; set; }

        /// <summary>
        ///   Gets or sets the scaled height of this Haar feature rectangle.
        /// </summary>
        /// 
        public int ScaledHeight { get; set; }

        /// <summary>
        ///   Gets or sets the scaled weight of this Haar feature rectangle.
        /// </summary>
        /// 
        public float ScaledWeight { get; set; }


        /// <summary>
        ///   Constructs a new Haar-like feature rectangle.
        /// </summary>
        /// <param name="values">Values for this rectangle.</param>
        /// 
        public HaarRectangle(int[] values)
        {
            this.X = values[0];
            this.Y = values[1];
            this.Width = values[2];
            this.Height = values[3];
            this.Weight = values[4];
        }

        /// <summary>
        ///   Constructs a new Haar-like feature rectangle.
        /// </summary>
        /// 
        public HaarRectangle(int x, int y, int width, int height, float weight)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
            this.Weight = weight;
        }

        private HaarRectangle() { }

        /// <summary>
        ///   Gets the area of this rectangle.
        /// </summary>
        /// 
        public int Area
        {
            get { return ScaledWidth * ScaledHeight; }
        }

        /// <summary>
        ///   Scales the values of this rectangle.
        /// </summary>
        /// 
        public void ScaleRectangle(float value)
        {
            ScaledX = (int)(X * value);
            ScaledY = (int)(Y * value);
            ScaledWidth = (int)(Width * value);
            ScaledHeight = (int)(Height * value);
        }

        /// <summary>
        ///   Scales the weight of this rectangle.
        /// </summary>
        /// 
        public void ScaleWeight(float scale)
        {
            ScaledWeight = Weight * scale;
        }

        /// <summary>
        ///   Converts from a string representation.
        /// </summary>
        /// 
        public static HaarRectangle Parse(string s)
        {
            string[] values = s.Trim().Split(' ');

            int x = int.Parse(values[0], CultureInfo.InvariantCulture);
            int y = int.Parse(values[1], CultureInfo.InvariantCulture);
            int w = int.Parse(values[2], CultureInfo.InvariantCulture);
            int h = int.Parse(values[3], CultureInfo.InvariantCulture);
            float weight = float.Parse(values[4], CultureInfo.InvariantCulture);

            return new HaarRectangle(x, y, w, h, weight);
        }


        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public object Clone()
        {
            HaarRectangle r = new HaarRectangle();
            r.Height = Height;
            r.ScaledHeight = ScaledHeight;
            r.ScaledWeight = ScaledWeight;
            r.ScaledWidth = ScaledWidth;
            r.ScaledX = ScaledX;
            r.ScaledY = ScaledY;
            r.Weight = Weight;
            r.Width = Width;
            r.X = X;
            r.Y = Y;

            return r;
        }

    }
}
