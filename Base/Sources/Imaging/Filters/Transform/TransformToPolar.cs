// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2010
// contacts@aforgenet.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Transform rectangle image into circle (to polar coordinates).
    /// </summary>
    /// 
    /// <remarks><para>The image processing routine does transformation of the source image into
    /// circle (polar transformation). The produced effect is similar to GIMP's "Polar Coordinates"
    /// distortion filter (or its equivalent in Photoshop).
    /// </para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale and 24/32 bpp color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// TransformToPolar filter = new TransformToPolar( );
    /// filter.OffsetAngle = 0;
    /// filter.CirlceDepth = 1;
    /// filter.UseOriginalImageSize = false;
    /// filter.NewSize = new Size( 200, 200 );
    /// // apply the filter
    /// Bitmap newImage = filter.Apply( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample21.png" width="320" height="160" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/polar.png" width="200" height="200" />
    /// </remarks>
    /// 
    /// <seealso cref="TransformFromPolar"/>
    /// 
    public class TransformToPolar : BaseTransformationFilter
    {
        private const double Pi2 = Math.PI * 2;
        private const double PiHalf = Math.PI / 2;

        private double circleDepth = 1.0;

        /// <summary>
        /// Circularity coefficient of the mapping, [0, 1].
        /// </summary>
        /// 
        /// <remarks><para>The property specifies circularity coefficient of the mapping to be done.
        /// If the coefficient is set to 1, then the mapping will produce ideal circle. If the coefficient
        /// is set to 0, then the mapping will occupy entire area of the destination image (circle will
        /// be extended into direction of edges). Changing the property from 0 to 1 user may balance
        /// circularity of the produced output.
        /// </para>
        /// 
        /// <para>Default value is set to <b>1</b>.</para>
        /// </remarks>
        /// 
        public double CirlceDepth
        {
            get { return circleDepth; }
            set { circleDepth = Math.Max( 0, Math.Min( 1, value ) ); }
        }

        private double offsetAngle = 0;

        /// <summary>
        /// Offset angle used to shift mapping, [-360, 360] degrees.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies offset angle, which can be used to shift
        /// mapping in counter clockwise direction. For example, if user sets this property to 30, then
        /// start of polar mapping is shifted by 30 degrees in counter clockwise direction.</para>
        /// 
        /// <para>Default value is set to <b>0</b>.</para>
        /// </remarks>
        /// 
        public double OffsetAngle
        {
            get { return offsetAngle; }
            set { offsetAngle = Math.Max( -360, Math.Min( 360, value ) ); }
        }

        private bool mapBackwards = false;

        /// <summary>
        /// Specifies direction of mapping.
        /// </summary>
        ///
        /// <remarks><para>The property specifies direction of mapping source image's X axis. If the
        /// property is set to <see langword="false"/>, the image is mapped in clockwise direction;
        /// otherwise in counter clockwise direction.</para>
        /// 
        /// <para>Default value is set to <see langword="false"/>.</para>
        /// </remarks>
        ///
        public bool MapBackwards
        {
            get { return mapBackwards; }
            set { mapBackwards = value; }
        }

        private bool mapFromTop = true;

        /// <summary>
        /// Specifies if top of the source image should go to center or edge of the result image.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies position of the source image's top line in the destination
        /// image. If the property is set to <see langword="true"/>, then it goes to the center of the result image;
        /// otherwise it goes to the edge.</para>
        /// 
        /// <para>Default value is set to <see langword="true"/>.</para>
        /// </remarks>
        /// 
        public bool MapFromTop
        {
            get { return mapFromTop; }
            set { mapFromTop = value; }
        }

        private Color fillColor = Color.White;

        /// <summary>
        /// Fill color to use for unprocessed areas.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies fill color, which is used to fill unprocessed areas.
        /// In the case if <see cref="CirlceDepth"/> is greater than 0, then there will be some areas on
        /// the image's edge, which are not filled by the produced "circular" image, but are filled by
        /// the specified color.
        /// </para>
        /// 
        /// <para>Default value is set to <see cref="Color.White"/>.</para>
        /// </remarks>
        /// 
        public Color FillColor
        {
            get { return fillColor; }
            set { fillColor = value; }
        }

        private Size newSize = new Size( 200, 200 );
        private bool useOriginalImageSize = true;

        /// <summary>
        /// Size of destination image.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies size of result image produced by this image
        /// processing routine in the case if <see cref="UseOriginalImageSize"/> property
        /// is set to <see langword="false"/>.</para>
        /// 
        /// <para><note>Both width and height must be in the [1, 10000] range.</note></para>
        /// 
        /// <para>Default value is set to <b>200 x 200</b>.</para>
        /// </remarks>
        /// 
        public Size NewSize
        {
            get { return newSize; }
            set
            {
                newSize = new Size(
                    Math.Max( 1, Math.Min( 10000, value.Width ) ),
                    Math.Max( 1, Math.Min( 10000, value.Height ) ) );
            }
        }

        /// <summary>
        /// Use source image size for destination or not.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies if the image processing routine should create destination
        /// image of the same size as original image or of the size specified by <see cref="NewSize"/>
        /// property.</para>
        /// 
        /// <para>Default value is set to <see langword="true"/>.</para>
        /// </remarks>
        /// 
        public bool UseOriginalImageSize
        {
            get { return useOriginalImageSize; }
            set { useOriginalImageSize = value; }
        }

        // private format translation dictionary
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>( );

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        /// 
        /// <remarks><para>See <see cref="IFilterInformation.FormatTranslations"/>
        /// documentation for additional information.</para></remarks>
        /// 
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformToPolar"/> class.
        /// </summary>
        /// 
        public TransformToPolar( )
        {
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb]    = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb]    = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb]   = PixelFormat.Format32bppArgb;
            formatTranslations[PixelFormat.Format32bppPArgb]  = PixelFormat.Format32bppPArgb;
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
            return ( useOriginalImageSize ) ? new Size( sourceData.Width, sourceData.Height ) : newSize;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// <param name="destinationData">Destination image data.</param>
        /// 
        protected override unsafe void ProcessFilter( UnmanagedImage sourceData, UnmanagedImage destinationData )
        {
            int pixelSize = Bitmap.GetPixelFormatSize( destinationData.PixelFormat ) / 8;

            // get source image size
            int width  = sourceData.Width;
            int height = sourceData.Height;
            int widthM1  = width - 1;
            int heightM1 = height - 1;

            // get destination image size
            int newWidth  = destinationData.Width;
            int newHeight = destinationData.Height;

            // invert cirlce depth
            double circleDisform = 1 - circleDepth;

            // get position of center pixel
            double cx = (double) ( newWidth - 1 ) / 2;
            double cy = (double) ( newHeight - 1 ) / 2;
            double radius = ( cx < cy ) ? cx : cy;
            radius -= radius * circleDisform;

            // angle of the diagonal
            double diagonalAngle = Math.Atan2( cy, cx );

            // offset angle in radians
            double offsetAngleR = offsetAngle / 180 * Math.PI;
            if ( offsetAngleR < 0 )
            {
                offsetAngleR = Pi2 - offsetAngleR;
            }
            // add extra offset to make sure rotation starts from Y axis and Pi*2 to
            // make sure any negative angle will get positive
            offsetAngleR += PiHalf + Pi2;

            // do the job
            byte* baseSrc = (byte*) sourceData.ImageData.ToPointer( );
            byte* dst = (byte*) destinationData.ImageData.ToPointer( );

            int srcStride = sourceData.Stride;
            int dstOffset = destinationData.Stride - newWidth * pixelSize;

            // coordinates of source points
            int sx1, sy1, sx2, sy2;
            double dx1, dy1, dx2, dy2;

            // temporary pointers
            byte* p1, p2, p3, p4;

            byte fillR = fillColor.R;
            byte fillB = fillColor.B;
            byte fillG = fillColor.G;
            byte fillA = fillColor.A;
            byte fillGrey = (byte) ( 0.2125 * fillR + 0.7154 * fillG + 0.0721 * fillB );

            for ( int y = 0; y < newHeight; y++ )
            {
                double dy = y - cy;
                double dydy = dy * dy;

                for ( int x = 0; x < newWidth; x++ )
                {
                    double dx = x - cx;
                    // distance from the center
                    double distance = Math.Sqrt( dx * dx + dydy );
                    // angle of the line connecting center and the current pixel
                    double angle = Math.Atan2( dy, dx );

                    // calculate minimum angle between X axis and the
                    // line connecting center and the current pixel
                    double oxAngle = ( angle > 0 ) ? angle : -angle;
                    if ( oxAngle > PiHalf )
                    {
                        oxAngle = Math.PI - oxAngle;
                    }

                    // calculate maximm distance from center for this angle - distance to image's edge
                    double maxDistance = ( oxAngle > diagonalAngle ) ? ( cy / Math.Sin( oxAngle ) ) : ( cx / Math.Cos( oxAngle ) );

                    // calculate maximum allowed distance within wich we need to map Y axis of the source image
                    double maxAllowedDistance = radius + maxDistance * circleDisform;

                    if ( distance < maxAllowedDistance + 1 )
                    {
                        // add angle offset and make sure it is in the [0, 2PI) range
                        angle += offsetAngleR;
                        angle = angle % Pi2;

                        // calculate pixel coordinates in the source image
                        double sy = ( distance / maxAllowedDistance ) * heightM1;

                        if ( sy > heightM1 )
                        {
                            sy = heightM1;
                        }

                        if ( !mapFromTop )
                        {
                            sy = heightM1 - sy;
                        }

                        double sx = ( angle / Pi2 ) * widthM1;

                        if ( mapBackwards )
                        {
                            sx = widthM1 - sx;
                        }

                        sx1 = (int) sx;
                        sx2 = ( sx1 == widthM1 ) ? sx1 : sx1 + 1;
                        dx1 = sx - sx1;
                        dx2 = 1.0 - dx1;

                        sy1 = (int) sy;
                        sy2 = ( sy1 == heightM1 ) ? sy1 : sy1 + 1;
                        dy1 = sy - sy1;
                        dy2 = 1.0 - dy1;

                        // get four points
                        p1 = p2 = baseSrc + sy1 * srcStride;
                        p1 += sx1 * pixelSize;
                        p2 += sx2 * pixelSize;

                        p3 = p4 = baseSrc + sy2 * srcStride;
                        p3 += sx1 * pixelSize;
                        p4 += sx2 * pixelSize;

                        // interpolate using 4 points
                        for ( int i = 0; i < pixelSize; i++, dst++, p1++, p2++, p3++, p4++ )
                        {
                            *dst = (byte) (
                                dy2 * ( dx2 * ( *p1 ) + dx1 * ( *p2 ) ) +
                                dy1 * ( dx2 * ( *p3 ) + dx1 * ( *p4 ) ) );
                        }
                    }
                    else
                    {
                        // fill unprocessed pixel with default color
                        if ( pixelSize == 1 )
                        {
                            *dst = fillGrey;
                        }
                        else
                        {
                            dst[RGB.R] = fillR;
                            dst[RGB.G] = fillG;
                            dst[RGB.B] = fillB;

                            if ( pixelSize > 3 )
                            {
                                dst[RGB.A] = fillA;
                            }
                        }

                        dst += pixelSize;
                    }
                }
                dst += dstOffset;
            }
        }
    }
}
