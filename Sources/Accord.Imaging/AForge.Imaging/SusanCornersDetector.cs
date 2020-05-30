// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
//
// Copyright © Frank Nagl, 2007
// admin@franknagl.de
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
    using System.Drawing.Imaging;
    using System.Collections.Generic;
    using Accord.Imaging.Filters;
    using Accord.Compat;

    /// <summary>
    /// Susan corners detector.
    /// </summary>
    /// 
    /// <remarks><para>The class implements Susan corners detector, which is described by
    /// S.M. Smith in: <b>S.M. Smith, "SUSAN - a new approach to low level image processing",
    /// Internal Technical Report TR95SMS1, Defense Research Agency, Chobham Lane, Chertsey,
    /// Surrey, UK, 1995</b>.</para>
    /// 
    /// <para><note>Some implementation notes:
    /// <list type="bullet">
    /// <item>Analyzing each pixel and searching for its USAN area, the 7x7 mask is used,
    /// which is comprised of 37 pixels. The mask has circle shape:
    /// <code lang="none">
    ///   xxx
    ///  xxxxx
    /// xxxxxxx
    /// xxxxxxx
    /// xxxxxxx
    ///  xxxxx
    ///   xxx
    /// </code>
    /// </item>
    /// <item>In the case if USAN's center of mass has the same coordinates as nucleus
    /// (central point), the pixel is not a corner.</item>
    /// <item>For noise suppression the 5x5 square window is used.</item></list></note></para>
    /// 
    /// <para>The class processes only grayscale 8 bpp and color 24/32 bpp images.
    /// In the case of color image, it is converted to grayscale internally using
    /// <see cref="GrayscaleBT709"/> filter.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create corners detector's instance
    /// SusanCornersDetector scd = new SusanCornersDetector( );
    /// // process image searching for corners
    /// List&lt;IntPoint&gt; corners = scd.ProcessImage( image );
    /// // process points
    /// foreach ( IntPoint corner in corners )
    /// {
    ///     // ... 
    /// }
    /// </code>
    /// </remarks>
    /// 
    /// <seealso cref="MoravecCornersDetector"/>
    /// 
    public class SusanCornersDetector : BaseCornersDetector
    {
        // brightness difference threshold
        private int differenceThreshold = 25;

        // geometrical threshold
        private int geometricalThreshold = 18;

        /// <summary>
        /// Brightness difference threshold.
        /// </summary>
        /// 
        /// <remarks><para>The brightness difference threshold controls the amount
        /// of pixels, which become part of USAN area. If difference between central
        /// pixel (nucleus) and surrounding pixel is not higher than difference threshold,
        /// then that pixel becomes part of USAN.</para>
        /// 
        /// <para>Increasing this value decreases the amount of detected corners.</para>
        /// 
        /// <para>Default value is set to <b>25</b>.</para>
        /// </remarks>
        /// 
        public int DifferenceThreshold
        {
            get { return differenceThreshold; }
            set { differenceThreshold = value; }
        }

        /// <summary>
        /// Geometrical threshold.
        /// </summary>
        /// 
        /// <remarks><para>The geometrical threshold sets the maximum number of pixels
        /// in USAN area around corner. If potential corner has USAN with more pixels, than
        /// it is not a corner.</para>
        /// 
        /// <para> Decreasing this value decreases the amount of detected corners - only sharp corners
        /// are detected. Increasing this value increases the amount of detected corners, but
        /// also increases amount of flat corners, which may be not corners at all.</para>
        /// 
        /// <para>Default value is set to <b>18</b>, which is half of maximum amount of pixels in USAN.</para>
        /// </remarks>
        /// 
        public int GeometricalThreshold
        {
            get { return geometricalThreshold; }
            set { geometricalThreshold = value; }
        }

        private static int[] rowRadius = new int[7] { 1, 2, 3, 3, 3, 2, 1 };

        /// <summary>
        /// Initializes a new instance of the <see cref="SusanCornersDetector"/> class.
        /// </summary>
        public SusanCornersDetector()
        {
            base.SupportedFormats.UnionWith(new[] { PixelFormat.Format8bppIndexed,
               PixelFormat.Format24bppRgb,
               PixelFormat.Format32bppRgb,
               PixelFormat.Format32bppArgb });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SusanCornersDetector"/> class.
        /// </summary>
        /// 
        /// <param name="differenceThreshold">Brightness difference threshold.</param>
        /// <param name="geometricalThreshold">Geometrical threshold.</param>
        /// 
        public SusanCornersDetector(int differenceThreshold, int geometricalThreshold) : this()
        {
            this.differenceThreshold = differenceThreshold;
            this.geometricalThreshold = geometricalThreshold;
        }

        /// <summary>
        /// This method should be implemented by inheriting classes to implement the
        /// actual corners detection, transforming the input image into a list of points.
        /// </summary>
        /// 
        protected override List<IntPoint> InnerProcess(UnmanagedImage image)
        {
            // get source image size
            int width = image.Width;
            int height = image.Height;

            // make sure we have grayscale image
            UnmanagedImage grayImage = null;

            if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                grayImage = image;
            }
            else
            {
                // create temporary grayscale image
                grayImage = Grayscale.CommonAlgorithms.BT709.Apply(image);
            }

            int[,] susanMap = new int[height, width];

            // do the job
            unsafe
            {
                int stride = grayImage.Stride;
                int offset = grayImage.Offset;

                byte* src = (byte*)grayImage.ImageData.ToPointer() + stride * 3 + 3;

                // for each row
                for (int y = 3, maxY = height - 3; y < maxY; y++)
                {
                    // for each pixel
                    for (int x = 3, maxX = width - 3; x < maxX; x++, src++)
                    {
                        // get value of the nucleus
                        byte nucleusValue = *src;
                        // usan - number of pixels with similar brightness
                        int usan = 0;
                        // center of gravity
                        int cx = 0, cy = 0;

                        // for each row of the mask
                        for (int i = -3; i <= 3; i++)
                        {
                            // determine row's radius
                            int r = rowRadius[i + 3];

                            // get pointer to the central pixel of the row
                            byte* ptr = (byte*)((long)src + (long)(stride * i));

                            // for each element of the mask's row
                            for (int j = -r; j <= r; j++)
                            {
                                // differenceThreshold
                                if (System.Math.Abs(nucleusValue - ptr[j]) <= differenceThreshold)
                                {
                                    usan++;

                                    cx += x + j;
                                    cy += y + i;
                                }
                            }
                        }

                        // check usan size
                        if (usan < geometricalThreshold)
                        {
                            cx /= usan;
                            cy /= usan;

                            if ((x != cx) || (y != cy))
                            {
                                // cornersList.Add( new Point( x, y ) );
                                usan = (geometricalThreshold - usan);
                            }
                            else
                            {
                                usan = 0;
                            }
                        }
                        else
                        {
                            usan = 0;
                        }

                        // usan = ( usan < geometricalThreshold ) ? ( geometricalThreshold - usan ) : 0;
                        susanMap[y, x] = usan;
                    }

                    src += 6 + offset;
                }
            }

            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                // free grayscale image
                grayImage.Dispose();
            }

            // collect interesting points - only those points, which are local maximums
            List<IntPoint> cornersList = new List<IntPoint>();

            // for each row
            for (int y = 2, maxY = height - 2; y < maxY; y++)
            {
                // for each pixel
                for (int x = 2, maxX = width - 2; x < maxX; x++)
                {
                    int currentValue = susanMap[y, x];

                    // for each windows' row
                    for (int i = -2; (currentValue != 0) && (i <= 2); i++)
                    {
                        // for each windows' pixel
                        for (int j = -2; j <= 2; j++)
                        {
                            if (susanMap[y + i, x + j] > currentValue)
                            {
                                currentValue = 0;
                                break;
                            }
                        }
                    }

                    // check if this point is really interesting
                    if (currentValue != 0)
                    {
                        cornersList.Add(new IntPoint(x, y));
                    }
                }
            }

            return cornersList;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        protected override object Clone(ISet<PixelFormat> supportedFormats)
        {
            return new SusanCornersDetector(differenceThreshold, geometricalThreshold)
            {
                SupportedFormats = supportedFormats
            };
        }
    }
}
