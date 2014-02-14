// Accord Control Library
// The Accord.NET Framework
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

namespace Accord.Controls.Vision
{
    using System;
    using System.Drawing;

    /// <summary>
    ///   Provides data to head movement events.
    /// </summary>
    /// 
    public class HeadEventArgs : EventArgs
    {
        /// <summary>
        ///   Gets or sets the x-coordinate of the head.
        /// </summary>
        /// 
        public float X { get; set; }

        /// <summary>
        ///   Gets or sets the y-coordinate of the head.
        /// </summary>
        /// 
        /// <value>The Y.</value>
        /// 
        public float Y { get; set; }

        /// <summary>
        ///   Gets or sets the scale of the head.
        /// </summary>
        /// 
        /// <value>The scale.</value>
        /// 
        public float Scale { get; set; }

        /// <summary>
        ///   Gets or sets the tilting angle of the head.
        /// </summary>
        /// 
        /// <value>The angle.</value>
        /// 
        public float Angle { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="HeadEventArgs"/> class.
        /// </summary>
        /// 
        public HeadEventArgs(float x, float y, float angle, float scale)
        {
            this.X = x;
            this.Y = y;
            this.Angle = angle;
            this.Scale = scale;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="HeadEventArgs"/> class.
        /// </summary>
        /// 
        public HeadEventArgs(PointF position, float angle, float scale)
        {
            this.X = position.X;
            this.Y = position.Y;
            this.Angle = angle;
            this.Scale = scale;
        }

    }
}
