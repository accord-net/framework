// Accord Imaging Library
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

namespace Accord.Imaging
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using AForge;
    using AForge.Imaging;

    /// <summary>
    ///   Border following algorithm for contour extraction.
    /// </summary>
    /// 
    /// <example>
    /// <code>
    /// // Create a new border following algorithm
    /// BorderFollowing bf = new BorderFollowing();
    /// 
    /// // Get all points in the contour of the image. 
    /// List&lt;IntPoint> contour = bf.FindContour(grayscaleImage);
    /// 
    /// // Mark all points in the contour point list in blue
    /// new PointsMarker(contour, Color.Blue).ApplyInPlace(image);
    /// 
    /// // Show the result
    /// ImageBox.Show(image);
    /// </code>
    /// 
    /// <para>
    ///   The resulting image is shown below.</para>
    ///   
    /// <img src="..\images\border-following.png" /> 
    /// 
    /// </example>
    /// 
    public class BorderFollowing : IContourExtractionAlgorithm
    {

        /// <summary>
        ///   Gets or sets the pixel value threshold above which a pixel
        ///   is considered white (belonging to the object). Default is zero.
        /// </summary>
        /// 
        public byte Threshold { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="BorderFollowing"/> class.
        /// </summary>
        /// 
        public BorderFollowing()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="BorderFollowing"/> class.
        /// </summary>
        /// 
        /// <param name="threshold">The pixel value threshold above which a pixel
        /// is considered black (belonging to the object). Default is zero.</param>
        /// 
        public BorderFollowing(byte threshold)
        {
            this.Threshold = threshold;
        }


        /// <summary>
        /// Extracts the contour from a single object in a grayscale image.
        /// </summary>
        /// 
        /// <param name="image">A grayscale image.</param>
        /// <returns>A list of <see cref="IntPoint"/>s defining a contour.</returns>
        /// 
        public List<IntPoint> FindContour(Bitmap image)
        {
            // lock source bitmap data
            BitmapData srcData = image.LockBits(
                new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);

            List<IntPoint> contour = null;

            try
            {
                // extract image contour
                contour = FindContour(srcData);
            }
            finally
            {
                // unlock source image
                image.UnlockBits(srcData);
            }

            return contour;
        }

        /// <summary>
        ///   Extracts the contour from a single object in a grayscale image.
        /// </summary>
        /// 
        /// <param name="image">A grayscale image.</param>
        /// 
        /// <returns>
        /// A list of <see cref="IntPoint"/>s defining a contour.
        /// </returns>
        /// 
        public List<IntPoint> FindContour(BitmapData image)
        {
            return FindContour(new UnmanagedImage(image));
        }

        /// <summary>
        ///   Extracts the contour from a single object in a grayscale image.
        /// </summary>
        /// 
        /// <param name="image">A grayscale image.</param>
        /// <returns>A list of <see cref="IntPoint"/>s defining a contour.</returns>
        /// 
        public unsafe List<IntPoint> FindContour(UnmanagedImage image)
        {
            CheckPixelFormat(image.PixelFormat);

            int width = image.Width;
            int height = image.Height;
            int stride = image.Stride;
            int offset = image.Stride - image.Width;

            byte* src = (byte*)image.ImageData.ToPointer();


            byte* start = null;
            List<IntPoint> contour = new List<IntPoint>();

            IntPoint prevPosition = new IntPoint();

            // 1. Find the lowest point in the image 

            // The lowest point is searched first by lowest X, then lowest Y, to use
            // the same ordering of AForge.NET's GrahamConvexHull. Unfortunately, this
            // means we have to search our image by inspecting columns rather than rows.

            bool found = false;

            byte* col = src;
            for (int x = 0; x < width && !found; x++, col++)
            {
                byte* row = col;
                for (int y = 0; y < height && !found; y++, row += stride)
                {
                    if (*row > Threshold)
                    {
                        start = row;
                        prevPosition = new IntPoint(x, y);
                        contour.Add(prevPosition);
                        found = true;
                    }
                }
            }

            if (contour.Count == 0)
            {
                // Empty image
                return contour;
            }


            // 2. Beginning on the first point, starting from left
            //    neighbor and going into counter-clockwise direction,
            //    find a neighbor pixel which is black.

            int[] windowOffset =
            { 
                +1,          // 0: Right
                -stride + 1, // 1: Top-Right
                -stride,     // 2: Top
                -stride - 1, // 3: Top-Left
                -1,          // 4: Left
                +stride - 1, // 5: Bottom-Left
                +stride,     // 6: Bottom
                +stride + 1, // 7: Bottom-Right
            };

            int direction = 4; // 4: Left
            byte* current = start;
            byte* previous = null;


            do // Search until we find a dead end (or the starting pixel)
            {
                found = false;

                // Search in the neighborhood window
                for (int i = 0; i < windowOffset.Length; i++)
                {
                    // Find the next candidate neighbor point
                    IntPoint next = prevPosition + positionOffset[direction];

                    // Check if it is inside the blob area
                    if (next.X < 0 || next.X >= width ||
                        next.Y < 0 || next.Y >= height)
                    {
                        // It isn't. Change direction and continue.
                        direction = (direction + 1) % windowOffset.Length;
                        continue;
                    }

                    // Find the next candidate neighbor pixel
                    byte* neighbor = current + windowOffset[direction];

                    // Check if it is a colored pixel
                    if (*neighbor <= Threshold)
                    {
                        // It isn't. Change direction and continue.
                        direction = (direction + 1) % windowOffset.Length;
                        continue;
                    }

                    // Check if it is a previously found pixel
                    if (neighbor == previous || neighbor == start)
                    {
                        // We found a dead end.
                        found = false; break;
                    }

                    // If we reached until here, we have
                    //  found a neighboring black pixel.
                    found = true; break;
                }

                if (found)
                {
                    // Navigate to neighbor pixel
                    previous = current;
                    current = current + windowOffset[direction];

                    // Add to the contour
                    prevPosition += positionOffset[direction];
                    contour.Add(prevPosition);

                    // Continue counter-clockwise search
                    //  from the most promising direction
                    direction = nextDirection[direction];
                }

            } while (found);


            return contour;
        }


        // Check for supported pixel format
        private static void CheckPixelFormat(PixelFormat format)
        {
            // check pixel format
            if ((format != PixelFormat.Format8bppIndexed))
            {
                throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
            }
        }



        // lookup tables
        private static readonly IntPoint[] positionOffset = 
            {
                new IntPoint( 1,  0), // 0: Right
                new IntPoint( 1, -1), // 1: Top-Right
                new IntPoint( 0, -1), // 2: Top
                new IntPoint(-1, -1), // 3: Top-Left
                new IntPoint(-1,  0), // 4: Left
                new IntPoint(-1,  1), // 5: Bottom-Left
                new IntPoint( 0,  1), // 6: Bottom
                new IntPoint( 1,  1), // 7: Bottom-Right
            };

        private static readonly int[] nextDirection = 
            {
                7, // 0: Right
                7, // 1: Top-Right
                1, // 2: Top
                1, // 3: Top-Left
                3, // 4: Left
                3, // 5: Bottom-Left
                5, // 6: Bottom
                5, // 7: Bottom-Right
            };

    }
}
