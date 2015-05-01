// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2008
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Base class for image rotation filters.
    /// </summary>
    /// 
    /// <remarks>The abstract class is the base class for all filters,
    /// which implement rotating algorithms.</remarks>
    /// 
    public abstract class BaseRotateFilter : BaseTransformationFilter
    {
        /// <summary>
        /// Rotation angle.
        /// </summary>
        protected double angle;

        /// <summary>
        /// Keep image size or not.
        /// </summary>
        protected bool keepSize = false;

        /// <summary>
        /// Fill color.
        /// </summary>
        protected Color fillColor = Color.FromArgb( 0, 0, 0 );

        /// <summary>
        /// Rotation angle, [0, 360].
        /// </summary>
        public double Angle
        {
            get { return angle; }
            set { angle = value % 360; }
        }

        /// <summary>
        /// Keep image size or not.
        /// </summary>
        /// 
        /// <remarks><para>The property determines if source image's size will be kept
        /// as it is or not. If the value is set to <b>false</b>, then the new image will have
        /// new dimension according to rotation angle. If the valus is set to
        /// <b>true</b>, then the new image will have the same size, which means that some parts
        /// of the image may be clipped because of rotation.</para>
        /// </remarks>
        /// 
        public bool KeepSize
        {
            get { return keepSize; }
            set { keepSize = value; }
        }

        /// <summary>
        /// Fill color.
        /// </summary>
        /// 
        /// <remarks><para>The fill color is used to fill areas of destination image,
        /// which don't have corresponsing pixels in source image.</para></remarks>
        /// 
        public Color FillColor
        {
            get { return fillColor; }
            set { fillColor = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRotateFilter"/> class.
        /// </summary>
        /// 
        /// <param name="angle">Rotation angle.</param>
        /// 
        /// <remarks><para>This constructor sets <see cref="KeepSize"/> property to <b>false</b>.
        /// </para></remarks>
        /// 
        public BaseRotateFilter( double angle )
        {
            this.angle = angle;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRotateFilter"/> class.
        /// </summary>
        /// 
        /// <param name="angle">Rotation angle.</param>
        /// <param name="keepSize">Keep image size or not.</param>
        /// 
        public BaseRotateFilter( double angle, bool keepSize )
        {
            this.angle    = angle;
            this.keepSize = keepSize;
        }

        /// <summary>
        /// Calculates new image size.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// 
        /// <returns>New image size - size of the destination image.</returns>
        /// 
        protected override System.Drawing.Size CalculateNewImageSize( UnmanagedImage sourceData )
        {
            // return same size if original image size should be kept
            if ( keepSize )
                return new Size( sourceData.Width, sourceData.Height );

            // angle's sine and cosine
            double angleRad = -angle * Math.PI / 180;
            double angleCos = Math.Cos( angleRad );
            double angleSin = Math.Sin( angleRad );

            // calculate half size
            double halfWidth  = (double) sourceData.Width / 2;
            double halfHeight = (double) sourceData.Height / 2;

            // rotate corners
            double cx1 = halfWidth * angleCos;
            double cy1 = halfWidth * angleSin;

            double cx2 = halfWidth * angleCos - halfHeight * angleSin;
            double cy2 = halfWidth * angleSin + halfHeight * angleCos;

            double cx3 = -halfHeight * angleSin;
            double cy3 =  halfHeight * angleCos;

            double cx4 = 0;
            double cy4 = 0;

            // recalculate image size
            halfWidth  = Math.Max( Math.Max( cx1, cx2 ), Math.Max( cx3, cx4 ) ) - Math.Min( Math.Min( cx1, cx2 ), Math.Min( cx3, cx4 ) );
            halfHeight = Math.Max( Math.Max( cy1, cy2 ), Math.Max( cy3, cy4 ) ) - Math.Min( Math.Min( cy1, cy2 ), Math.Min( cy3, cy4 ) );

            return new Size( (int) ( halfWidth * 2 + 0.5 ), (int) ( halfHeight * 2 + 0.5 ) );
        }
    }
}
