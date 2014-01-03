// Accord Vision Library
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

namespace Accord.Vision.Tracking
{
    using System;
    using System.Drawing;
    using AForge;
    using AForge.Imaging;
    using AForge.Math.Geometry;
    using Point = AForge.Point;
    using System.Xml.Serialization;


    /// <summary>
    ///   Axis orientation.
    /// </summary>
    /// 
    public enum AxisOrientation
    {
        /// <summary>
        ///   Horizontal axis.
        /// </summary>
        /// 
        Horizontal,

        /// <summary>
        ///   Vertical axis.
        /// </summary>
        /// 
        Vertical

    }

    /// <summary>
    ///   Tracking object to represent an object in a scene.
    /// </summary>
    /// 
    [Serializable]
    public class TrackingObject : ICloneable
    {

        [NonSerialized]
        UnmanagedImage image;

        /// <summary>
        /// Gets or sets an user-defined tag associated with this object.
        /// </summary>
        /// 
        public object Tag { get; set; }

        /// <summary>
        ///   Gets or sets the rectangle containing the object.
        /// </summary>
        /// 
        public Rectangle Rectangle { get; set; }

        /// <summary>
        ///   Gets or sets the center of gravity of the object 
        ///   relative to the original image from where it has 
        ///   been extracted.
        /// </summary>
        /// 
        public IntPoint Center { get; set; }

        /// <summary>
        ///   Gets or sets the object's extracted image.
        /// </summary>
        /// 
        [XmlIgnore]
        public UnmanagedImage Image { get { return image; } set { image = value; } }

        /// <summary>
        /// Gets a value indicating whether the object is empty.
        /// </summary>
        /// 
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        /// 
        public bool IsEmpty
        {
            get { return Rectangle.IsEmpty; }
        }

        /// <summary>
        ///   Gets the area of the object.
        /// </summary>
        /// 
        public int Area
        {
            get { return Rectangle.Width * Rectangle.Height; }
        }

        /// <summary>
        ///   Gets or sets the angle of the object.
        /// </summary>
        /// 
        public float Angle { get; set; }

        /// <summary>
        ///   Constructs a new tracking object.
        /// </summary>
        /// 
        public TrackingObject()
        {
        }

        /// <summary>
        ///   Constructs a new tracking object.
        /// </summary>
        /// 
        /// <param name="center">The center of gravity of the object.</param>
        /// 
        public TrackingObject(IntPoint center)
        {
            this.Center = center;
        }

        /// <summary>
        ///   Constructs a new tracking object.
        /// </summary>
        /// 
        /// <param name="angle">The angle of orientation for the object.</param>
        /// <param name="center">The center of gravity of the object.</param>
        /// <param name="rectangle">The rectangle containing the object.</param>
        /// 
        public TrackingObject(Rectangle rectangle, IntPoint center, float angle)
        {
            this.Rectangle = rectangle;
            this.Center = center;
            this.Angle = angle;
        }

        /// <summary>
        ///   Constructs a new tracking object.
        /// </summary>
        /// 
        /// <param name="rectangle">The rectangle containing the object.</param>
        /// <param name="angle">The angle of the object.</param>
        /// 
        public TrackingObject(Rectangle rectangle, float angle)
        {
            this.Rectangle = rectangle;
            this.Angle = angle;

            this.Center = new IntPoint(
                rectangle.X + rectangle.Width / 2,
                rectangle.Y - rectangle.Height / 2);
        }

        /// <summary>
        ///   Gets two points defining the horizontal axis of the object.
        /// </summary>
        /// 
        public LineSegment GetAxis()
        {
            return GetAxis(AxisOrientation.Horizontal);
        }

        /// <summary>
        ///   Gets two points defining the axis of the object.
        /// </summary>
        /// 
        public LineSegment GetAxis(AxisOrientation axis)
        {
            double x1, y1;
            double x2, y2;

            if (axis == AxisOrientation.Horizontal)
            {
                y1 = Math.Cos(-Angle - Math.PI) * Rectangle.Height / 2.0;
                x1 = Math.Sin(-Angle - Math.PI) * Rectangle.Width / 2.0;

                y2 = Math.Cos(-Angle) * Rectangle.Height / 2.0;
                x2 = Math.Sin(-Angle) * Rectangle.Width / 2.0;
            }
            else
            {
                y1 = Math.Cos(-(Angle + Math.PI / 2) - Math.PI) * Rectangle.Height / 2.0;
                x1 = Math.Sin(-(Angle + Math.PI / 2) - Math.PI) * Rectangle.Width / 2.0;

                y2 = Math.Cos(-(Angle + Math.PI / 2)) * Rectangle.Height / 2.0;
                x2 = Math.Sin(-(Angle + Math.PI / 2)) * Rectangle.Width / 2.0;
            }

            Point start = new Point((float)(Center.X + x1), (float)(Center.Y + y1));
            Point end = new Point((float)(Center.X + x2), (float)(Center.Y + y2));

            if (start.DistanceTo(end) == 0)
                return null;

            return new LineSegment(start, end);
        }

        /// <summary>
        ///   Resets this tracking object.
        /// </summary>
        /// 
        public void Reset()
        {
            this.Rectangle = Rectangle.Empty;
            this.Center = new IntPoint();
            this.Image = null;
            this.Angle = 0;
        }


        
        #region ICloneable Members

        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        object ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        /// <param name="excludeImage">Pass true to not include
        ///   the <see cref="Image"/> in the copy object.</param>
        /// 
        public TrackingObject Clone(bool excludeImage = true)
        {
            TrackingObject obj = new TrackingObject();
            obj.Angle = Angle;
            obj.Center = Center;
            obj.Rectangle = Rectangle;
            obj.Tag = Tag;

            if (!excludeImage)
                obj.Image = Image.Clone();

            return obj;
        }

        #endregion

    }
}
