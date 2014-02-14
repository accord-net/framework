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

// Copyright © Edward Rosten, 2006-2010
//  This work is partially based on the original FAST
//  library, distributed under a 3-clause BSD license.
//  http://svr-www.eng.cam.ac.uk/~er258/work/fast.html
//
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions
// are met:
// 
// 	*Redistributions of source code must retain the above copyright
// 	 notice, this list of conditions and the following disclaimer.
// 
// 	*Redistributions in binary form must reproduce the above copyright
// 	 notice, this list of conditions and the following disclaimer in the
// 	 documentation and/or other materials provided with the distribution.
// 
// 	*Neither the name of the University of Cambridge nor the names of 
// 	 its contributors may be used to endorse or promote products derived 
// 	 from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
// EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 

namespace Accord.Imaging
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Linq;
    using Accord.Math;
    using AForge;
    using AForge.Imaging;
    using AForge.Imaging.Filters;

    /// <summary>
    ///   Features from Accelerated Segment Test (FAST) corners detector.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In the FAST corner detection algorithm, a pixel is defined as a corner
    ///   if (in a circle surrounding the pixel), N or more contiguous pixels are
    ///   all significantly brighter then or all significantly darker than the center
    ///   pixel. The ordering of questions used to classify a pixel is learned using
    ///   the ID3 algorithm.
    /// </para>
    /// 
    /// <para>This detector has been shown to exhibit a high degree of repeatability.</para>
    ///
    /// <para>
    ///   The code is roughly based on the 9 valued FAST corner detection
    ///   algorithm implementation in C by Edward Rosten, which has been
    ///   published under a 3-clause BSD license and is freely available at:
    ///   http://svr-www.eng.cam.ac.uk/~er258/work/fast.html. 
    /// </para>
    /// 
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       E. Rosten, T. Drummond. Fusing Points and Lines for High
    ///       Performance Tracking, ICCV 2005. </description></item>
    ///     <item><description>
    ///       E. Rosten, T. Drummond. Machine learning for high-speed
    ///       corner detection, ICCV 2005</description></item>
    ///   </list>
    /// </para>
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    /// 
    ///   Bitmap image = ... // Lena's famous picture
    /// 
    ///   // Create a new FAST Corners Detector
    ///   FastCornersDetector fast = new FastCornersDetector()
    ///   {
    ///       Suppress = true, // suppress non-maximum points
    ///       Threshold = 40   // less leads to more corners
    ///   };
    /// 
    ///   // Process the image looking for corners
    ///   List&lt;IntPoint> points = fast.ProcessImage(image);
    /// 
    ///   // Create a filter to mark the corners
    ///   PointsMarker marker = new PointsMarker(points);
    ///             
    ///   // Apply the corner-marking filter
    ///   Bitmap markers = marker.Apply(image);
    /// 
    ///   // Show on the screen
    ///   ImageBox.Show(markers);
    /// </code>
    /// 
    /// <para>
    ///   The resulting image is shown below:</para>
    /// 
    ///   <img src="..\images\fast.png" />
    ///   
    /// </example>
    /// 
    /// <seealso cref="SpeededUpRobustFeaturesDetector"/>
    /// <seealso cref="FastRetinaKeypointDetector"/>
    /// 
    [Serializable]
    [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode")]
    public class FastCornersDetector : ICornersDetector
    {

        private int threshold = 20;
        private bool suppress = true;
        private int[] scores;


        #region Constructors
        /// <summary>
        ///   Initializes a new instance of the <see cref="FastCornersDetector"/> class.
        /// </summary>
        /// 
        public FastCornersDetector()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="FastCornersDetector"/> class.
        /// </summary>
        /// 
        /// <param name="threshold">The suppression threshold. Decreasing this value
        ///   increases the number of points detected by the algorithm. Default is 20.</param>
        /// 
        public FastCornersDetector(int threshold)
        {
            this.threshold = threshold;
        }
        #endregion


        #region Properties
        /// <summary>
        ///   Gets or sets a value indicating whether non-maximum
        ///   points should be suppressed. Default is true.
        /// </summary>
        /// 
        /// <value><c>true</c> if non-maximum points should
        ///   be suppressed; otherwise, <c>false</c>.</value>
        ///   
        public bool Suppress
        {
            get { return suppress; }
            set { suppress = value; }
        }

        /// <summary>
        ///   Gets or sets the corner detection threshold. Increasing this value results in less corners,
        ///   whereas decreasing this value will result in more corners detected by the algorithm.
        /// </summary>
        /// 
        /// <value>The corners threshold.</value>
        /// 
        public int Threshold
        {
            get { return threshold; }
            set { threshold = value; }
        }

        /// <summary>
        ///   Gets the scores of the each corner detected in
        ///   the previous call to <see cref="ProcessImage(Bitmap)"/>.
        /// </summary>
        /// 
        /// <value>The scores of each last computed corner.</value>
        /// 
        public int[] Scores
        {
            get { return scores; }
        }
        #endregion


        /// <summary>
        ///   Process image looking for corners.
        /// </summary>
        /// 
        /// <param name="imageData">Source image data to process.</param>
        /// 
        /// <returns>Returns list of found corners (X-Y coordinates).</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">
        ///   The source image has incorrect pixel format.
        /// </exception>
        /// 
        public List<IntPoint> ProcessImage(BitmapData imageData)
        {
            return ProcessImage(new UnmanagedImage(imageData));
        }

        /// <summary>
        ///   Process image looking for corners.
        /// </summary>
        /// 
        /// <param name="image">Source image data to process.</param>
        /// 
        /// <returns>Returns list of found corners (X-Y coordinates).</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">
        ///   The source image has incorrect pixel format.
        /// </exception>
        /// 
        public List<IntPoint> ProcessImage(Bitmap image)
        {
            // check image format
            if (
                (image.PixelFormat != PixelFormat.Format8bppIndexed) &&
                (image.PixelFormat != PixelFormat.Format24bppRgb) &&
                (image.PixelFormat != PixelFormat.Format32bppRgb) &&
                (image.PixelFormat != PixelFormat.Format32bppArgb)
                )
            {
                throw new UnsupportedImageFormatException("Unsupported pixel format of the source");
            }

            // lock source image
            BitmapData imageData = image.LockBits(
                new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, image.PixelFormat);

            List<IntPoint> corners;

            try
            {
                // process the image
                corners = ProcessImage(new UnmanagedImage(imageData));
            }
            finally
            {
                // unlock image
                image.UnlockBits(imageData);
            }

            return corners;
        }

        /// <summary>
        ///   Process image looking for corners.
        /// </summary>
        /// 
        /// <param name="image">Source image data to process.</param>
        /// 
        /// <returns>Returns list of found corners (X-Y coordinates).</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">
        ///   The source image has incorrect pixel format.
        /// </exception>
        /// 
        public List<IntPoint> ProcessImage(UnmanagedImage image)
        {

            // check image format
            if (
                (image.PixelFormat != PixelFormat.Format8bppIndexed) &&
                (image.PixelFormat != PixelFormat.Format24bppRgb) &&
                (image.PixelFormat != PixelFormat.Format32bppRgb) &&
                (image.PixelFormat != PixelFormat.Format32bppArgb)
                )
            {
                throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
            }

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


            // 0. Cache pixel offsets            
            int[] offsets = makeOffsets(grayImage.Stride);


            // 1. Detect corners using the given threshold
            IntPoint[] corners = detect(grayImage, offsets);


            // 2. Compute scores for each corner
            int[] scores = new int[corners.Length];
            for (int i = 0; i < corners.Length; i++)
                scores[i] = score(grayImage, corners[i], offsets);


            if (suppress)
            {
                // 3. Perform Non-Maximum Suppression
                int[] idx = maximum(corners, scores);
                corners = corners.Submatrix(idx);
                scores = scores.Submatrix(idx);
            }

            this.scores = scores;
            return corners.ToList();
        }


        #region Private methods
        private static int[] maximum(IntPoint[] corners, int[] scores)
        {
            int n = corners.Length;

            List<int> maximum = new List<int>(n);

            if (corners.Length == 0)
                return maximum.ToArray();


            int last_row;
            int[] row_start;

            // Point above points (roughly) to the pixel above
            // the one of interest, if there is a feature there.
            int point_above = 0;
            int point_below = 0;


            // Find where each row begins (the corners are output in raster scan order).
            // A beginning of -1 signifies that there are no corners on that row.
            last_row = corners[n - 1].Y;
            row_start = new int[last_row + 1];

            for (int i = 0; i < last_row + 1; i++)
                row_start[i] = -1;

            int prev_row = -1;
            for (int i = 0; i < n; i++)
            {
                if (corners[i].Y != prev_row)
                {
                    row_start[corners[i].Y] = i;
                    prev_row = corners[i].Y;
                }
            }


            // for each detected corner
            for (int i = 0; i < n; i++)
            {
                int score = scores[i];
                IntPoint pos = corners[i];

                // Check left
                if (i > 0)
                    if (corners[i - 1].X == pos.X - 1 &&
                        corners[i - 1].Y == pos.Y && scores[i - 1] >= score)
                        continue;

                // Check right
                if (i < (n - 1))
                    if (corners[i + 1].X == pos.X + 1 &&
                        corners[i + 1].Y == pos.Y && scores[i + 1] >= score)
                        continue;

                // Check above (if there is a valid row above)
                if (pos.Y != 0 && row_start[pos.Y - 1] != -1)
                {
                    // Make sure that current point_above is one row above.
                    if (corners[point_above].Y < pos.Y - 1)
                        point_above = row_start[pos.Y - 1];

                    // Make point_above point to the first of the pixels above the current point, if it exists.*/
                    for (; corners[point_above].Y < pos.Y && corners[point_above].X < pos.X - 1; point_above++) ;

                    for (int j = point_above; corners[j].Y < pos.Y && corners[j].X <= pos.X + 1; j++)
                    {
                        int x = corners[j].X;
                        if ((x == pos.X - 1 || x == pos.X || x == pos.X + 1) && scores[j] >= score)
                            goto next_corner;
                    }

                }

                // Check below (if there is anything below)
                if (pos.Y != last_row && row_start[pos.Y + 1] != -1 && point_below < n)
                {
                    // Nothing below
                    if (corners[point_below].Y < pos.Y + 1)
                        point_below = row_start[pos.Y + 1];

                    // Make point below point to one of the pixels below the current point, if it exists.
                    for (; point_below < n && corners[point_below].Y == pos.Y + 1 && corners[point_below].X < pos.X - 1; point_below++) ;

                    for (int j = point_below; j < n && corners[j].Y == pos.Y + 1 && corners[j].X <= pos.X + 1; j++)
                    {
                        int x = corners[j].X;
                        if ((x == pos.X - 1 || x == pos.X || x == pos.X + 1) && scores[j] >= score)
                            goto next_corner;
                    }
                }

                // The current point is a local maximum.
                // Add its index to the list of indices.
                maximum.Add(i);

            next_corner:
                continue;
            }

            return maximum.ToArray();
        }

        private unsafe IntPoint[] detect(UnmanagedImage image, int[] offsets)
        {
            int width = image.Width;
            int height = image.Height;
            int stride = image.Stride;
            int offset = stride - width;
            int b = this.threshold;

            byte* src = (byte*)image.ImageData.ToPointer();
            byte* p = src + 3 * stride + 3;

            List<IntPoint> points = new List<IntPoint>(512);

            for (int y = 3; y < height - 3; y++)
            {
                for (int x = 3; x < width - 3; x++, p++)
                {

                    #region Machine Generated Code
                    int cb = *p + b;
                    int c_b = *p - b;
                    if (p[offsets[0]] > cb)
                        if (p[offsets[1]] > cb)
                            if (p[offsets[2]] > cb)
                                if (p[offsets[3]] > cb)
                                    if (p[offsets[4]] > cb)
                                        if (p[offsets[5]] > cb)
                                            if (p[offsets[6]] > cb)
                                                if (p[offsets[7]] > cb)
                                                    if (p[offsets[8]] > cb)
                                                    { }
                                                    else
                                                        if (p[offsets[15]] > cb)
                                                        { }
                                                        else
                                                            continue;
                                                else if (p[offsets[7]] < c_b)
                                                    if (p[offsets[14]] > cb)
                                                        if (p[offsets[15]] > cb)
                                                        { }
                                                        else
                                                            continue;
                                                    else if (p[offsets[14]] < c_b)
                                                        if (p[offsets[8]] < c_b)
                                                            if (p[offsets[9]] < c_b)
                                                                if (p[offsets[10]] < c_b)
                                                                    if (p[offsets[11]] < c_b)
                                                                        if (p[offsets[12]] < c_b)
                                                                            if (p[offsets[13]] < c_b)
                                                                                if (p[offsets[15]] < c_b)
                                                                                { }
                                                                                else
                                                                                    continue;
                                                                            else
                                                                                continue;
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    if (p[offsets[14]] > cb)
                                                        if (p[offsets[15]] > cb)
                                                        { }
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                            else if (p[offsets[6]] < c_b)
                                                if (p[offsets[15]] > cb)
                                                    if (p[offsets[13]] > cb)
                                                        if (p[offsets[14]] > cb)
                                                        { }
                                                        else
                                                            continue;
                                                    else if (p[offsets[13]] < c_b)
                                                        if (p[offsets[7]] < c_b)
                                                            if (p[offsets[8]] < c_b)
                                                                if (p[offsets[9]] < c_b)
                                                                    if (p[offsets[10]] < c_b)
                                                                        if (p[offsets[11]] < c_b)
                                                                            if (p[offsets[12]] < c_b)
                                                                                if (p[offsets[14]] < c_b)
                                                                                { }
                                                                                else
                                                                                    continue;
                                                                            else
                                                                                continue;
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    if (p[offsets[7]] < c_b)
                                                        if (p[offsets[8]] < c_b)
                                                            if (p[offsets[9]] < c_b)
                                                                if (p[offsets[10]] < c_b)
                                                                    if (p[offsets[11]] < c_b)
                                                                        if (p[offsets[12]] < c_b)
                                                                            if (p[offsets[13]] < c_b)
                                                                                if (p[offsets[14]] < c_b)
                                                                                { }
                                                                                else
                                                                                    continue;
                                                                            else
                                                                                continue;
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                            else
                                                if (p[offsets[13]] > cb)
                                                    if (p[offsets[14]] > cb)
                                                        if (p[offsets[15]] > cb)
                                                        { }
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else if (p[offsets[13]] < c_b)
                                                    if (p[offsets[7]] < c_b)
                                                        if (p[offsets[8]] < c_b)
                                                            if (p[offsets[9]] < c_b)
                                                                if (p[offsets[10]] < c_b)
                                                                    if (p[offsets[11]] < c_b)
                                                                        if (p[offsets[12]] < c_b)
                                                                            if (p[offsets[14]] < c_b)
                                                                                if (p[offsets[15]] < c_b)
                                                                                { }
                                                                                else
                                                                                    continue;
                                                                            else
                                                                                continue;
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                        else if (p[offsets[5]] < c_b)
                                            if (p[offsets[14]] > cb)
                                                if (p[offsets[12]] > cb)
                                                    if (p[offsets[13]] > cb)
                                                        if (p[offsets[15]] > cb)
                                                        { }
                                                        else
                                                            if (p[offsets[6]] > cb)
                                                                if (p[offsets[7]] > cb)
                                                                    if (p[offsets[8]] > cb)
                                                                        if (p[offsets[9]] > cb)
                                                                            if (p[offsets[10]] > cb)
                                                                                if (p[offsets[11]] > cb)
                                                                                { }
                                                                                else
                                                                                    continue;
                                                                            else
                                                                                continue;
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                    else
                                                        continue;
                                                else if (p[offsets[12]] < c_b)
                                                    if (p[offsets[6]] < c_b)
                                                        if (p[offsets[7]] < c_b)
                                                            if (p[offsets[8]] < c_b)
                                                                if (p[offsets[9]] < c_b)
                                                                    if (p[offsets[10]] < c_b)
                                                                        if (p[offsets[11]] < c_b)
                                                                            if (p[offsets[13]] < c_b)
                                                                            { }
                                                                            else
                                                                                continue;
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                            else if (p[offsets[14]] < c_b)
                                                if (p[offsets[7]] < c_b)
                                                    if (p[offsets[8]] < c_b)
                                                        if (p[offsets[9]] < c_b)
                                                            if (p[offsets[10]] < c_b)
                                                                if (p[offsets[11]] < c_b)
                                                                    if (p[offsets[12]] < c_b)
                                                                        if (p[offsets[13]] < c_b)
                                                                            if (p[offsets[6]] < c_b)
                                                                            { }
                                                                            else
                                                                                if (p[offsets[15]] < c_b)
                                                                                { }
                                                                                else
                                                                                    continue;
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                            else
                                                if (p[offsets[6]] < c_b)
                                                    if (p[offsets[7]] < c_b)
                                                        if (p[offsets[8]] < c_b)
                                                            if (p[offsets[9]] < c_b)
                                                                if (p[offsets[10]] < c_b)
                                                                    if (p[offsets[11]] < c_b)
                                                                        if (p[offsets[12]] < c_b)
                                                                            if (p[offsets[13]] < c_b)
                                                                            { }
                                                                            else
                                                                                continue;
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                        else
                                            if (p[offsets[12]] > cb)
                                                if (p[offsets[13]] > cb)
                                                    if (p[offsets[14]] > cb)
                                                        if (p[offsets[15]] > cb)
                                                        { }
                                                        else
                                                            if (p[offsets[6]] > cb)
                                                                if (p[offsets[7]] > cb)
                                                                    if (p[offsets[8]] > cb)
                                                                        if (p[offsets[9]] > cb)
                                                                            if (p[offsets[10]] > cb)
                                                                                if (p[offsets[11]] > cb)
                                                                                { }
                                                                                else
                                                                                    continue;
                                                                            else
                                                                                continue;
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                            else if (p[offsets[12]] < c_b)
                                                if (p[offsets[7]] < c_b)
                                                    if (p[offsets[8]] < c_b)
                                                        if (p[offsets[9]] < c_b)
                                                            if (p[offsets[10]] < c_b)
                                                                if (p[offsets[11]] < c_b)
                                                                    if (p[offsets[13]] < c_b)
                                                                        if (p[offsets[14]] < c_b)
                                                                            if (p[offsets[6]] < c_b)
                                                                            { }
                                                                            else
                                                                                if (p[offsets[15]] < c_b)
                                                                                { }
                                                                                else
                                                                                    continue;
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                            else
                                                continue;
                                    else if (p[offsets[4]] < c_b)
                                        if (p[offsets[13]] > cb)
                                            if (p[offsets[11]] > cb)
                                                if (p[offsets[12]] > cb)
                                                    if (p[offsets[14]] > cb)
                                                        if (p[offsets[15]] > cb)
                                                        { }
                                                        else
                                                            if (p[offsets[6]] > cb)
                                                                if (p[offsets[7]] > cb)
                                                                    if (p[offsets[8]] > cb)
                                                                        if (p[offsets[9]] > cb)
                                                                            if (p[offsets[10]] > cb)
                                                                            { }
                                                                            else
                                                                                continue;
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                    else
                                                        if (p[offsets[5]] > cb)
                                                            if (p[offsets[6]] > cb)
                                                                if (p[offsets[7]] > cb)
                                                                    if (p[offsets[8]] > cb)
                                                                        if (p[offsets[9]] > cb)
                                                                            if (p[offsets[10]] > cb)
                                                                            { }
                                                                            else
                                                                                continue;
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                else
                                                    continue;
                                            else if (p[offsets[11]] < c_b)
                                                if (p[offsets[5]] < c_b)
                                                    if (p[offsets[6]] < c_b)
                                                        if (p[offsets[7]] < c_b)
                                                            if (p[offsets[8]] < c_b)
                                                                if (p[offsets[9]] < c_b)
                                                                    if (p[offsets[10]] < c_b)
                                                                        if (p[offsets[12]] < c_b)
                                                                        { }
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                            else
                                                continue;
                                        else if (p[offsets[13]] < c_b)
                                            if (p[offsets[7]] < c_b)
                                                if (p[offsets[8]] < c_b)
                                                    if (p[offsets[9]] < c_b)
                                                        if (p[offsets[10]] < c_b)
                                                            if (p[offsets[11]] < c_b)
                                                                if (p[offsets[12]] < c_b)
                                                                    if (p[offsets[6]] < c_b)
                                                                        if (p[offsets[5]] < c_b)
                                                                        { }
                                                                        else
                                                                            if (p[offsets[14]] < c_b)
                                                                            { }
                                                                            else
                                                                                continue;
                                                                    else
                                                                        if (p[offsets[14]] < c_b)
                                                                            if (p[offsets[15]] < c_b)
                                                                            { }
                                                                            else
                                                                                continue;
                                                                        else
                                                                            continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                            else
                                                continue;
                                        else
                                            if (p[offsets[5]] < c_b)
                                                if (p[offsets[6]] < c_b)
                                                    if (p[offsets[7]] < c_b)
                                                        if (p[offsets[8]] < c_b)
                                                            if (p[offsets[9]] < c_b)
                                                                if (p[offsets[10]] < c_b)
                                                                    if (p[offsets[11]] < c_b)
                                                                        if (p[offsets[12]] < c_b)
                                                                        { }
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                            else
                                                continue;
                                    else
                                        if (p[offsets[11]] > cb)
                                            if (p[offsets[12]] > cb)
                                                if (p[offsets[13]] > cb)
                                                    if (p[offsets[14]] > cb)
                                                        if (p[offsets[15]] > cb)
                                                        { }
                                                        else
                                                            if (p[offsets[6]] > cb)
                                                                if (p[offsets[7]] > cb)
                                                                    if (p[offsets[8]] > cb)
                                                                        if (p[offsets[9]] > cb)
                                                                            if (p[offsets[10]] > cb)
                                                                            { }
                                                                            else
                                                                                continue;
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                    else
                                                        if (p[offsets[5]] > cb)
                                                            if (p[offsets[6]] > cb)
                                                                if (p[offsets[7]] > cb)
                                                                    if (p[offsets[8]] > cb)
                                                                        if (p[offsets[9]] > cb)
                                                                            if (p[offsets[10]] > cb)
                                                                            { }
                                                                            else
                                                                                continue;
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                else
                                                    continue;
                                            else
                                                continue;
                                        else if (p[offsets[11]] < c_b)
                                            if (p[offsets[7]] < c_b)
                                                if (p[offsets[8]] < c_b)
                                                    if (p[offsets[9]] < c_b)
                                                        if (p[offsets[10]] < c_b)
                                                            if (p[offsets[12]] < c_b)
                                                                if (p[offsets[13]] < c_b)
                                                                    if (p[offsets[6]] < c_b)
                                                                        if (p[offsets[5]] < c_b)
                                                                        { }
                                                                        else
                                                                            if (p[offsets[14]] < c_b)
                                                                            { }
                                                                            else
                                                                                continue;
                                                                    else
                                                                        if (p[offsets[14]] < c_b)
                                                                            if (p[offsets[15]] < c_b)
                                                                            { }
                                                                            else
                                                                                continue;
                                                                        else
                                                                            continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                            else
                                                continue;
                                        else
                                            continue;
                                else if (p[offsets[3]] < c_b)
                                    if (p[offsets[10]] > cb)
                                        if (p[offsets[11]] > cb)
                                            if (p[offsets[12]] > cb)
                                                if (p[offsets[13]] > cb)
                                                    if (p[offsets[14]] > cb)
                                                        if (p[offsets[15]] > cb)
                                                        { }
                                                        else
                                                            if (p[offsets[6]] > cb)
                                                                if (p[offsets[7]] > cb)
                                                                    if (p[offsets[8]] > cb)
                                                                        if (p[offsets[9]] > cb)
                                                                        { }
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                    else
                                                        if (p[offsets[5]] > cb)
                                                            if (p[offsets[6]] > cb)
                                                                if (p[offsets[7]] > cb)
                                                                    if (p[offsets[8]] > cb)
                                                                        if (p[offsets[9]] > cb)
                                                                        { }
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                else
                                                    if (p[offsets[4]] > cb)
                                                        if (p[offsets[5]] > cb)
                                                            if (p[offsets[6]] > cb)
                                                                if (p[offsets[7]] > cb)
                                                                    if (p[offsets[8]] > cb)
                                                                        if (p[offsets[9]] > cb)
                                                                        { }
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                            else
                                                continue;
                                        else
                                            continue;
                                    else if (p[offsets[10]] < c_b)
                                        if (p[offsets[7]] < c_b)
                                            if (p[offsets[8]] < c_b)
                                                if (p[offsets[9]] < c_b)
                                                    if (p[offsets[11]] < c_b)
                                                        if (p[offsets[6]] < c_b)
                                                            if (p[offsets[5]] < c_b)
                                                                if (p[offsets[4]] < c_b)
                                                                { }
                                                                else
                                                                    if (p[offsets[12]] < c_b)
                                                                        if (p[offsets[13]] < c_b)
                                                                        { }
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                            else
                                                                if (p[offsets[12]] < c_b)
                                                                    if (p[offsets[13]] < c_b)
                                                                        if (p[offsets[14]] < c_b)
                                                                        { }
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                        else
                                                            if (p[offsets[12]] < c_b)
                                                                if (p[offsets[13]] < c_b)
                                                                    if (p[offsets[14]] < c_b)
                                                                        if (p[offsets[15]] < c_b)
                                                                        { }
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                            else
                                                continue;
                                        else
                                            continue;
                                    else
                                        continue;
                                else
                                    if (p[offsets[10]] > cb)
                                        if (p[offsets[11]] > cb)
                                            if (p[offsets[12]] > cb)
                                                if (p[offsets[13]] > cb)
                                                    if (p[offsets[14]] > cb)
                                                        if (p[offsets[15]] > cb)
                                                        { }
                                                        else
                                                            if (p[offsets[6]] > cb)
                                                                if (p[offsets[7]] > cb)
                                                                    if (p[offsets[8]] > cb)
                                                                        if (p[offsets[9]] > cb)
                                                                        { }
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                    else
                                                        if (p[offsets[5]] > cb)
                                                            if (p[offsets[6]] > cb)
                                                                if (p[offsets[7]] > cb)
                                                                    if (p[offsets[8]] > cb)
                                                                        if (p[offsets[9]] > cb)
                                                                        { }
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                else
                                                    if (p[offsets[4]] > cb)
                                                        if (p[offsets[5]] > cb)
                                                            if (p[offsets[6]] > cb)
                                                                if (p[offsets[7]] > cb)
                                                                    if (p[offsets[8]] > cb)
                                                                        if (p[offsets[9]] > cb)
                                                                        { }
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                            else
                                                continue;
                                        else
                                            continue;
                                    else if (p[offsets[10]] < c_b)
                                        if (p[offsets[7]] < c_b)
                                            if (p[offsets[8]] < c_b)
                                                if (p[offsets[9]] < c_b)
                                                    if (p[offsets[11]] < c_b)
                                                        if (p[offsets[12]] < c_b)
                                                            if (p[offsets[6]] < c_b)
                                                                if (p[offsets[5]] < c_b)
                                                                    if (p[offsets[4]] < c_b)
                                                                    { }
                                                                    else
                                                                        if (p[offsets[13]] < c_b)
                                                                        { }
                                                                        else
                                                                            continue;
                                                                else
                                                                    if (p[offsets[13]] < c_b)
                                                                        if (p[offsets[14]] < c_b)
                                                                        { }
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                            else
                                                                if (p[offsets[13]] < c_b)
                                                                    if (p[offsets[14]] < c_b)
                                                                        if (p[offsets[15]] < c_b)
                                                                        { }
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                            else
                                                continue;
                                        else
                                            continue;
                                    else
                                        continue;
                            else if (p[offsets[2]] < c_b)
                                if (p[offsets[9]] > cb)
                                    if (p[offsets[10]] > cb)
                                        if (p[offsets[11]] > cb)
                                            if (p[offsets[12]] > cb)
                                                if (p[offsets[13]] > cb)
                                                    if (p[offsets[14]] > cb)
                                                        if (p[offsets[15]] > cb)
                                                        { }
                                                        else
                                                            if (p[offsets[6]] > cb)
                                                                if (p[offsets[7]] > cb)
                                                                    if (p[offsets[8]] > cb)
                                                                    { }
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                    else
                                                        if (p[offsets[5]] > cb)
                                                            if (p[offsets[6]] > cb)
                                                                if (p[offsets[7]] > cb)
                                                                    if (p[offsets[8]] > cb)
                                                                    { }
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                else
                                                    if (p[offsets[4]] > cb)
                                                        if (p[offsets[5]] > cb)
                                                            if (p[offsets[6]] > cb)
                                                                if (p[offsets[7]] > cb)
                                                                    if (p[offsets[8]] > cb)
                                                                    { }
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                            else
                                                if (p[offsets[3]] > cb)
                                                    if (p[offsets[4]] > cb)
                                                        if (p[offsets[5]] > cb)
                                                            if (p[offsets[6]] > cb)
                                                                if (p[offsets[7]] > cb)
                                                                    if (p[offsets[8]] > cb)
                                                                    { }
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                        else
                                            continue;
                                    else
                                        continue;
                                else if (p[offsets[9]] < c_b)
                                    if (p[offsets[7]] < c_b)
                                        if (p[offsets[8]] < c_b)
                                            if (p[offsets[10]] < c_b)
                                                if (p[offsets[6]] < c_b)
                                                    if (p[offsets[5]] < c_b)
                                                        if (p[offsets[4]] < c_b)
                                                            if (p[offsets[3]] < c_b)
                                                            { }
                                                            else
                                                                if (p[offsets[11]] < c_b)
                                                                    if (p[offsets[12]] < c_b)
                                                                    { }
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                        else
                                                            if (p[offsets[11]] < c_b)
                                                                if (p[offsets[12]] < c_b)
                                                                    if (p[offsets[13]] < c_b)
                                                                    { }
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                    else
                                                        if (p[offsets[11]] < c_b)
                                                            if (p[offsets[12]] < c_b)
                                                                if (p[offsets[13]] < c_b)
                                                                    if (p[offsets[14]] < c_b)
                                                                    { }
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                else
                                                    if (p[offsets[11]] < c_b)
                                                        if (p[offsets[12]] < c_b)
                                                            if (p[offsets[13]] < c_b)
                                                                if (p[offsets[14]] < c_b)
                                                                    if (p[offsets[15]] < c_b)
                                                                    { }
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                            else
                                                continue;
                                        else
                                            continue;
                                    else
                                        continue;
                                else
                                    continue;
                            else
                                if (p[offsets[9]] > cb)
                                    if (p[offsets[10]] > cb)
                                        if (p[offsets[11]] > cb)
                                            if (p[offsets[12]] > cb)
                                                if (p[offsets[13]] > cb)
                                                    if (p[offsets[14]] > cb)
                                                        if (p[offsets[15]] > cb)
                                                        { }
                                                        else
                                                            if (p[offsets[6]] > cb)
                                                                if (p[offsets[7]] > cb)
                                                                    if (p[offsets[8]] > cb)
                                                                    { }
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                    else
                                                        if (p[offsets[5]] > cb)
                                                            if (p[offsets[6]] > cb)
                                                                if (p[offsets[7]] > cb)
                                                                    if (p[offsets[8]] > cb)
                                                                    { }
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                else
                                                    if (p[offsets[4]] > cb)
                                                        if (p[offsets[5]] > cb)
                                                            if (p[offsets[6]] > cb)
                                                                if (p[offsets[7]] > cb)
                                                                    if (p[offsets[8]] > cb)
                                                                    { }
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                            else
                                                if (p[offsets[3]] > cb)
                                                    if (p[offsets[4]] > cb)
                                                        if (p[offsets[5]] > cb)
                                                            if (p[offsets[6]] > cb)
                                                                if (p[offsets[7]] > cb)
                                                                    if (p[offsets[8]] > cb)
                                                                    { }
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                        else
                                            continue;
                                    else
                                        continue;
                                else if (p[offsets[9]] < c_b)
                                    if (p[offsets[7]] < c_b)
                                        if (p[offsets[8]] < c_b)
                                            if (p[offsets[10]] < c_b)
                                                if (p[offsets[11]] < c_b)
                                                    if (p[offsets[6]] < c_b)
                                                        if (p[offsets[5]] < c_b)
                                                            if (p[offsets[4]] < c_b)
                                                                if (p[offsets[3]] < c_b)
                                                                { }
                                                                else
                                                                    if (p[offsets[12]] < c_b)
                                                                    { }
                                                                    else
                                                                        continue;
                                                            else
                                                                if (p[offsets[12]] < c_b)
                                                                    if (p[offsets[13]] < c_b)
                                                                    { }
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                        else
                                                            if (p[offsets[12]] < c_b)
                                                                if (p[offsets[13]] < c_b)
                                                                    if (p[offsets[14]] < c_b)
                                                                    { }
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                    else
                                                        if (p[offsets[12]] < c_b)
                                                            if (p[offsets[13]] < c_b)
                                                                if (p[offsets[14]] < c_b)
                                                                    if (p[offsets[15]] < c_b)
                                                                    { }
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                else
                                                    continue;
                                            else
                                                continue;
                                        else
                                            continue;
                                    else
                                        continue;
                                else
                                    continue;
                        else if (p[offsets[1]] < c_b)
                            if (p[offsets[8]] > cb)
                                if (p[offsets[9]] > cb)
                                    if (p[offsets[10]] > cb)
                                        if (p[offsets[11]] > cb)
                                            if (p[offsets[12]] > cb)
                                                if (p[offsets[13]] > cb)
                                                    if (p[offsets[14]] > cb)
                                                        if (p[offsets[15]] > cb)
                                                        { }
                                                        else
                                                            if (p[offsets[6]] > cb)
                                                                if (p[offsets[7]] > cb)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                    else
                                                        if (p[offsets[5]] > cb)
                                                            if (p[offsets[6]] > cb)
                                                                if (p[offsets[7]] > cb)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                else
                                                    if (p[offsets[4]] > cb)
                                                        if (p[offsets[5]] > cb)
                                                            if (p[offsets[6]] > cb)
                                                                if (p[offsets[7]] > cb)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                            else
                                                if (p[offsets[3]] > cb)
                                                    if (p[offsets[4]] > cb)
                                                        if (p[offsets[5]] > cb)
                                                            if (p[offsets[6]] > cb)
                                                                if (p[offsets[7]] > cb)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                        else
                                            if (p[offsets[2]] > cb)
                                                if (p[offsets[3]] > cb)
                                                    if (p[offsets[4]] > cb)
                                                        if (p[offsets[5]] > cb)
                                                            if (p[offsets[6]] > cb)
                                                                if (p[offsets[7]] > cb)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                            else
                                                continue;
                                    else
                                        continue;
                                else
                                    continue;
                            else if (p[offsets[8]] < c_b)
                                if (p[offsets[7]] < c_b)
                                    if (p[offsets[9]] < c_b)
                                        if (p[offsets[6]] < c_b)
                                            if (p[offsets[5]] < c_b)
                                                if (p[offsets[4]] < c_b)
                                                    if (p[offsets[3]] < c_b)
                                                        if (p[offsets[2]] < c_b)
                                                        { }
                                                        else
                                                            if (p[offsets[10]] < c_b)
                                                                if (p[offsets[11]] < c_b)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                    else
                                                        if (p[offsets[10]] < c_b)
                                                            if (p[offsets[11]] < c_b)
                                                                if (p[offsets[12]] < c_b)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                else
                                                    if (p[offsets[10]] < c_b)
                                                        if (p[offsets[11]] < c_b)
                                                            if (p[offsets[12]] < c_b)
                                                                if (p[offsets[13]] < c_b)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                            else
                                                if (p[offsets[10]] < c_b)
                                                    if (p[offsets[11]] < c_b)
                                                        if (p[offsets[12]] < c_b)
                                                            if (p[offsets[13]] < c_b)
                                                                if (p[offsets[14]] < c_b)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                        else
                                            if (p[offsets[10]] < c_b)
                                                if (p[offsets[11]] < c_b)
                                                    if (p[offsets[12]] < c_b)
                                                        if (p[offsets[13]] < c_b)
                                                            if (p[offsets[14]] < c_b)
                                                                if (p[offsets[15]] < c_b)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                            else
                                                continue;
                                    else
                                        continue;
                                else
                                    continue;
                            else
                                continue;
                        else
                            if (p[offsets[8]] > cb)
                                if (p[offsets[9]] > cb)
                                    if (p[offsets[10]] > cb)
                                        if (p[offsets[11]] > cb)
                                            if (p[offsets[12]] > cb)
                                                if (p[offsets[13]] > cb)
                                                    if (p[offsets[14]] > cb)
                                                        if (p[offsets[15]] > cb)
                                                        { }
                                                        else
                                                            if (p[offsets[6]] > cb)
                                                                if (p[offsets[7]] > cb)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                    else
                                                        if (p[offsets[5]] > cb)
                                                            if (p[offsets[6]] > cb)
                                                                if (p[offsets[7]] > cb)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                else
                                                    if (p[offsets[4]] > cb)
                                                        if (p[offsets[5]] > cb)
                                                            if (p[offsets[6]] > cb)
                                                                if (p[offsets[7]] > cb)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                            else
                                                if (p[offsets[3]] > cb)
                                                    if (p[offsets[4]] > cb)
                                                        if (p[offsets[5]] > cb)
                                                            if (p[offsets[6]] > cb)
                                                                if (p[offsets[7]] > cb)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                        else
                                            if (p[offsets[2]] > cb)
                                                if (p[offsets[3]] > cb)
                                                    if (p[offsets[4]] > cb)
                                                        if (p[offsets[5]] > cb)
                                                            if (p[offsets[6]] > cb)
                                                                if (p[offsets[7]] > cb)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                            else
                                                continue;
                                    else
                                        continue;
                                else
                                    continue;
                            else if (p[offsets[8]] < c_b)
                                if (p[offsets[7]] < c_b)
                                    if (p[offsets[9]] < c_b)
                                        if (p[offsets[10]] < c_b)
                                            if (p[offsets[6]] < c_b)
                                                if (p[offsets[5]] < c_b)
                                                    if (p[offsets[4]] < c_b)
                                                        if (p[offsets[3]] < c_b)
                                                            if (p[offsets[2]] < c_b)
                                                            { }
                                                            else
                                                                if (p[offsets[11]] < c_b)
                                                                { }
                                                                else
                                                                    continue;
                                                        else
                                                            if (p[offsets[11]] < c_b)
                                                                if (p[offsets[12]] < c_b)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                    else
                                                        if (p[offsets[11]] < c_b)
                                                            if (p[offsets[12]] < c_b)
                                                                if (p[offsets[13]] < c_b)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                else
                                                    if (p[offsets[11]] < c_b)
                                                        if (p[offsets[12]] < c_b)
                                                            if (p[offsets[13]] < c_b)
                                                                if (p[offsets[14]] < c_b)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                            else
                                                if (p[offsets[11]] < c_b)
                                                    if (p[offsets[12]] < c_b)
                                                        if (p[offsets[13]] < c_b)
                                                            if (p[offsets[14]] < c_b)
                                                                if (p[offsets[15]] < c_b)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                        else
                                            continue;
                                    else
                                        continue;
                                else
                                    continue;
                            else
                                continue;
                    else if (p[offsets[0]] < c_b)
                        if (p[offsets[1]] > cb)
                            if (p[offsets[8]] > cb)
                                if (p[offsets[7]] > cb)
                                    if (p[offsets[9]] > cb)
                                        if (p[offsets[6]] > cb)
                                            if (p[offsets[5]] > cb)
                                                if (p[offsets[4]] > cb)
                                                    if (p[offsets[3]] > cb)
                                                        if (p[offsets[2]] > cb)
                                                        { }
                                                        else
                                                            if (p[offsets[10]] > cb)
                                                                if (p[offsets[11]] > cb)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                    else
                                                        if (p[offsets[10]] > cb)
                                                            if (p[offsets[11]] > cb)
                                                                if (p[offsets[12]] > cb)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                else
                                                    if (p[offsets[10]] > cb)
                                                        if (p[offsets[11]] > cb)
                                                            if (p[offsets[12]] > cb)
                                                                if (p[offsets[13]] > cb)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                            else
                                                if (p[offsets[10]] > cb)
                                                    if (p[offsets[11]] > cb)
                                                        if (p[offsets[12]] > cb)
                                                            if (p[offsets[13]] > cb)
                                                                if (p[offsets[14]] > cb)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                        else
                                            if (p[offsets[10]] > cb)
                                                if (p[offsets[11]] > cb)
                                                    if (p[offsets[12]] > cb)
                                                        if (p[offsets[13]] > cb)
                                                            if (p[offsets[14]] > cb)
                                                                if (p[offsets[15]] > cb)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                            else
                                                continue;
                                    else
                                        continue;
                                else
                                    continue;
                            else if (p[offsets[8]] < c_b)
                                if (p[offsets[9]] < c_b)
                                    if (p[offsets[10]] < c_b)
                                        if (p[offsets[11]] < c_b)
                                            if (p[offsets[12]] < c_b)
                                                if (p[offsets[13]] < c_b)
                                                    if (p[offsets[14]] < c_b)
                                                        if (p[offsets[15]] < c_b)
                                                        { }
                                                        else
                                                            if (p[offsets[6]] < c_b)
                                                                if (p[offsets[7]] < c_b)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                    else
                                                        if (p[offsets[5]] < c_b)
                                                            if (p[offsets[6]] < c_b)
                                                                if (p[offsets[7]] < c_b)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                else
                                                    if (p[offsets[4]] < c_b)
                                                        if (p[offsets[5]] < c_b)
                                                            if (p[offsets[6]] < c_b)
                                                                if (p[offsets[7]] < c_b)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                            else
                                                if (p[offsets[3]] < c_b)
                                                    if (p[offsets[4]] < c_b)
                                                        if (p[offsets[5]] < c_b)
                                                            if (p[offsets[6]] < c_b)
                                                                if (p[offsets[7]] < c_b)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                        else
                                            if (p[offsets[2]] < c_b)
                                                if (p[offsets[3]] < c_b)
                                                    if (p[offsets[4]] < c_b)
                                                        if (p[offsets[5]] < c_b)
                                                            if (p[offsets[6]] < c_b)
                                                                if (p[offsets[7]] < c_b)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                            else
                                                continue;
                                    else
                                        continue;
                                else
                                    continue;
                            else
                                continue;
                        else if (p[offsets[1]] < c_b)
                            if (p[offsets[2]] > cb)
                                if (p[offsets[9]] > cb)
                                    if (p[offsets[7]] > cb)
                                        if (p[offsets[8]] > cb)
                                            if (p[offsets[10]] > cb)
                                                if (p[offsets[6]] > cb)
                                                    if (p[offsets[5]] > cb)
                                                        if (p[offsets[4]] > cb)
                                                            if (p[offsets[3]] > cb)
                                                            { }
                                                            else
                                                                if (p[offsets[11]] > cb)
                                                                    if (p[offsets[12]] > cb)
                                                                    { }
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                        else
                                                            if (p[offsets[11]] > cb)
                                                                if (p[offsets[12]] > cb)
                                                                    if (p[offsets[13]] > cb)
                                                                    { }
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                    else
                                                        if (p[offsets[11]] > cb)
                                                            if (p[offsets[12]] > cb)
                                                                if (p[offsets[13]] > cb)
                                                                    if (p[offsets[14]] > cb)
                                                                    { }
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                else
                                                    if (p[offsets[11]] > cb)
                                                        if (p[offsets[12]] > cb)
                                                            if (p[offsets[13]] > cb)
                                                                if (p[offsets[14]] > cb)
                                                                    if (p[offsets[15]] > cb)
                                                                    { }
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                            else
                                                continue;
                                        else
                                            continue;
                                    else
                                        continue;
                                else if (p[offsets[9]] < c_b)
                                    if (p[offsets[10]] < c_b)
                                        if (p[offsets[11]] < c_b)
                                            if (p[offsets[12]] < c_b)
                                                if (p[offsets[13]] < c_b)
                                                    if (p[offsets[14]] < c_b)
                                                        if (p[offsets[15]] < c_b)
                                                        { }
                                                        else
                                                            if (p[offsets[6]] < c_b)
                                                                if (p[offsets[7]] < c_b)
                                                                    if (p[offsets[8]] < c_b)
                                                                    { }
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                    else
                                                        if (p[offsets[5]] < c_b)
                                                            if (p[offsets[6]] < c_b)
                                                                if (p[offsets[7]] < c_b)
                                                                    if (p[offsets[8]] < c_b)
                                                                    { }
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                else
                                                    if (p[offsets[4]] < c_b)
                                                        if (p[offsets[5]] < c_b)
                                                            if (p[offsets[6]] < c_b)
                                                                if (p[offsets[7]] < c_b)
                                                                    if (p[offsets[8]] < c_b)
                                                                    { }
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                            else
                                                if (p[offsets[3]] < c_b)
                                                    if (p[offsets[4]] < c_b)
                                                        if (p[offsets[5]] < c_b)
                                                            if (p[offsets[6]] < c_b)
                                                                if (p[offsets[7]] < c_b)
                                                                    if (p[offsets[8]] < c_b)
                                                                    { }
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                        else
                                            continue;
                                    else
                                        continue;
                                else
                                    continue;
                            else if (p[offsets[2]] < c_b)
                                if (p[offsets[3]] > cb)
                                    if (p[offsets[10]] > cb)
                                        if (p[offsets[7]] > cb)
                                            if (p[offsets[8]] > cb)
                                                if (p[offsets[9]] > cb)
                                                    if (p[offsets[11]] > cb)
                                                        if (p[offsets[6]] > cb)
                                                            if (p[offsets[5]] > cb)
                                                                if (p[offsets[4]] > cb)
                                                                { }
                                                                else
                                                                    if (p[offsets[12]] > cb)
                                                                        if (p[offsets[13]] > cb)
                                                                        { }
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                            else
                                                                if (p[offsets[12]] > cb)
                                                                    if (p[offsets[13]] > cb)
                                                                        if (p[offsets[14]] > cb)
                                                                        { }
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                        else
                                                            if (p[offsets[12]] > cb)
                                                                if (p[offsets[13]] > cb)
                                                                    if (p[offsets[14]] > cb)
                                                                        if (p[offsets[15]] > cb)
                                                                        { }
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                            else
                                                continue;
                                        else
                                            continue;
                                    else if (p[offsets[10]] < c_b)
                                        if (p[offsets[11]] < c_b)
                                            if (p[offsets[12]] < c_b)
                                                if (p[offsets[13]] < c_b)
                                                    if (p[offsets[14]] < c_b)
                                                        if (p[offsets[15]] < c_b)
                                                        { }
                                                        else
                                                            if (p[offsets[6]] < c_b)
                                                                if (p[offsets[7]] < c_b)
                                                                    if (p[offsets[8]] < c_b)
                                                                        if (p[offsets[9]] < c_b)
                                                                        { }
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                    else
                                                        if (p[offsets[5]] < c_b)
                                                            if (p[offsets[6]] < c_b)
                                                                if (p[offsets[7]] < c_b)
                                                                    if (p[offsets[8]] < c_b)
                                                                        if (p[offsets[9]] < c_b)
                                                                        { }
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                else
                                                    if (p[offsets[4]] < c_b)
                                                        if (p[offsets[5]] < c_b)
                                                            if (p[offsets[6]] < c_b)
                                                                if (p[offsets[7]] < c_b)
                                                                    if (p[offsets[8]] < c_b)
                                                                        if (p[offsets[9]] < c_b)
                                                                        { }
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                            else
                                                continue;
                                        else
                                            continue;
                                    else
                                        continue;
                                else if (p[offsets[3]] < c_b)
                                    if (p[offsets[4]] > cb)
                                        if (p[offsets[13]] > cb)
                                            if (p[offsets[7]] > cb)
                                                if (p[offsets[8]] > cb)
                                                    if (p[offsets[9]] > cb)
                                                        if (p[offsets[10]] > cb)
                                                            if (p[offsets[11]] > cb)
                                                                if (p[offsets[12]] > cb)
                                                                    if (p[offsets[6]] > cb)
                                                                        if (p[offsets[5]] > cb)
                                                                        { }
                                                                        else
                                                                            if (p[offsets[14]] > cb)
                                                                            { }
                                                                            else
                                                                                continue;
                                                                    else
                                                                        if (p[offsets[14]] > cb)
                                                                            if (p[offsets[15]] > cb)
                                                                            { }
                                                                            else
                                                                                continue;
                                                                        else
                                                                            continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                            else
                                                continue;
                                        else if (p[offsets[13]] < c_b)
                                            if (p[offsets[11]] > cb)
                                                if (p[offsets[5]] > cb)
                                                    if (p[offsets[6]] > cb)
                                                        if (p[offsets[7]] > cb)
                                                            if (p[offsets[8]] > cb)
                                                                if (p[offsets[9]] > cb)
                                                                    if (p[offsets[10]] > cb)
                                                                        if (p[offsets[12]] > cb)
                                                                        { }
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                            else if (p[offsets[11]] < c_b)
                                                if (p[offsets[12]] < c_b)
                                                    if (p[offsets[14]] < c_b)
                                                        if (p[offsets[15]] < c_b)
                                                        { }
                                                        else
                                                            if (p[offsets[6]] < c_b)
                                                                if (p[offsets[7]] < c_b)
                                                                    if (p[offsets[8]] < c_b)
                                                                        if (p[offsets[9]] < c_b)
                                                                            if (p[offsets[10]] < c_b)
                                                                            { }
                                                                            else
                                                                                continue;
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                    else
                                                        if (p[offsets[5]] < c_b)
                                                            if (p[offsets[6]] < c_b)
                                                                if (p[offsets[7]] < c_b)
                                                                    if (p[offsets[8]] < c_b)
                                                                        if (p[offsets[9]] < c_b)
                                                                            if (p[offsets[10]] < c_b)
                                                                            { }
                                                                            else
                                                                                continue;
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                else
                                                    continue;
                                            else
                                                continue;
                                        else
                                            if (p[offsets[5]] > cb)
                                                if (p[offsets[6]] > cb)
                                                    if (p[offsets[7]] > cb)
                                                        if (p[offsets[8]] > cb)
                                                            if (p[offsets[9]] > cb)
                                                                if (p[offsets[10]] > cb)
                                                                    if (p[offsets[11]] > cb)
                                                                        if (p[offsets[12]] > cb)
                                                                        { }
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                            else
                                                continue;
                                    else if (p[offsets[4]] < c_b)
                                        if (p[offsets[5]] > cb)
                                            if (p[offsets[14]] > cb)
                                                if (p[offsets[7]] > cb)
                                                    if (p[offsets[8]] > cb)
                                                        if (p[offsets[9]] > cb)
                                                            if (p[offsets[10]] > cb)
                                                                if (p[offsets[11]] > cb)
                                                                    if (p[offsets[12]] > cb)
                                                                        if (p[offsets[13]] > cb)
                                                                            if (p[offsets[6]] > cb)
                                                                            { }
                                                                            else
                                                                                if (p[offsets[15]] > cb)
                                                                                { }
                                                                                else
                                                                                    continue;
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                            else if (p[offsets[14]] < c_b)
                                                if (p[offsets[12]] > cb)
                                                    if (p[offsets[6]] > cb)
                                                        if (p[offsets[7]] > cb)
                                                            if (p[offsets[8]] > cb)
                                                                if (p[offsets[9]] > cb)
                                                                    if (p[offsets[10]] > cb)
                                                                        if (p[offsets[11]] > cb)
                                                                            if (p[offsets[13]] > cb)
                                                                            { }
                                                                            else
                                                                                continue;
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else if (p[offsets[12]] < c_b)
                                                    if (p[offsets[13]] < c_b)
                                                        if (p[offsets[15]] < c_b)
                                                        { }
                                                        else
                                                            if (p[offsets[6]] < c_b)
                                                                if (p[offsets[7]] < c_b)
                                                                    if (p[offsets[8]] < c_b)
                                                                        if (p[offsets[9]] < c_b)
                                                                            if (p[offsets[10]] < c_b)
                                                                                if (p[offsets[11]] < c_b)
                                                                                { }
                                                                                else
                                                                                    continue;
                                                                            else
                                                                                continue;
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                            else
                                                if (p[offsets[6]] > cb)
                                                    if (p[offsets[7]] > cb)
                                                        if (p[offsets[8]] > cb)
                                                            if (p[offsets[9]] > cb)
                                                                if (p[offsets[10]] > cb)
                                                                    if (p[offsets[11]] > cb)
                                                                        if (p[offsets[12]] > cb)
                                                                            if (p[offsets[13]] > cb)
                                                                            { }
                                                                            else
                                                                                continue;
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                        else if (p[offsets[5]] < c_b)
                                            if (p[offsets[6]] > cb)
                                                if (p[offsets[15]] < c_b)
                                                    if (p[offsets[13]] > cb)
                                                        if (p[offsets[7]] > cb)
                                                            if (p[offsets[8]] > cb)
                                                                if (p[offsets[9]] > cb)
                                                                    if (p[offsets[10]] > cb)
                                                                        if (p[offsets[11]] > cb)
                                                                            if (p[offsets[12]] > cb)
                                                                                if (p[offsets[14]] > cb)
                                                                                { }
                                                                                else
                                                                                    continue;
                                                                            else
                                                                                continue;
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else if (p[offsets[13]] < c_b)
                                                        if (p[offsets[14]] < c_b)
                                                        { }
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    if (p[offsets[7]] > cb)
                                                        if (p[offsets[8]] > cb)
                                                            if (p[offsets[9]] > cb)
                                                                if (p[offsets[10]] > cb)
                                                                    if (p[offsets[11]] > cb)
                                                                        if (p[offsets[12]] > cb)
                                                                            if (p[offsets[13]] > cb)
                                                                                if (p[offsets[14]] > cb)
                                                                                { }
                                                                                else
                                                                                    continue;
                                                                            else
                                                                                continue;
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                            else if (p[offsets[6]] < c_b)
                                                if (p[offsets[7]] > cb)
                                                    if (p[offsets[14]] > cb)
                                                        if (p[offsets[8]] > cb)
                                                            if (p[offsets[9]] > cb)
                                                                if (p[offsets[10]] > cb)
                                                                    if (p[offsets[11]] > cb)
                                                                        if (p[offsets[12]] > cb)
                                                                            if (p[offsets[13]] > cb)
                                                                                if (p[offsets[15]] > cb)
                                                                                { }
                                                                                else
                                                                                    continue;
                                                                            else
                                                                                continue;
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else if (p[offsets[14]] < c_b)
                                                        if (p[offsets[15]] < c_b)
                                                        { }
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else if (p[offsets[7]] < c_b)
                                                    if (p[offsets[8]] < c_b)
                                                    { }
                                                    else
                                                        if (p[offsets[15]] < c_b)
                                                        { }
                                                        else
                                                            continue;
                                                else
                                                    if (p[offsets[14]] < c_b)
                                                        if (p[offsets[15]] < c_b)
                                                        { }
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                            else
                                                if (p[offsets[13]] > cb)
                                                    if (p[offsets[7]] > cb)
                                                        if (p[offsets[8]] > cb)
                                                            if (p[offsets[9]] > cb)
                                                                if (p[offsets[10]] > cb)
                                                                    if (p[offsets[11]] > cb)
                                                                        if (p[offsets[12]] > cb)
                                                                            if (p[offsets[14]] > cb)
                                                                                if (p[offsets[15]] > cb)
                                                                                { }
                                                                                else
                                                                                    continue;
                                                                            else
                                                                                continue;
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else if (p[offsets[13]] < c_b)
                                                    if (p[offsets[14]] < c_b)
                                                        if (p[offsets[15]] < c_b)
                                                        { }
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                        else
                                            if (p[offsets[12]] > cb)
                                                if (p[offsets[7]] > cb)
                                                    if (p[offsets[8]] > cb)
                                                        if (p[offsets[9]] > cb)
                                                            if (p[offsets[10]] > cb)
                                                                if (p[offsets[11]] > cb)
                                                                    if (p[offsets[13]] > cb)
                                                                        if (p[offsets[14]] > cb)
                                                                            if (p[offsets[6]] > cb)
                                                                            { }
                                                                            else
                                                                                if (p[offsets[15]] > cb)
                                                                                { }
                                                                                else
                                                                                    continue;
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                            else if (p[offsets[12]] < c_b)
                                                if (p[offsets[13]] < c_b)
                                                    if (p[offsets[14]] < c_b)
                                                        if (p[offsets[15]] < c_b)
                                                        { }
                                                        else
                                                            if (p[offsets[6]] < c_b)
                                                                if (p[offsets[7]] < c_b)
                                                                    if (p[offsets[8]] < c_b)
                                                                        if (p[offsets[9]] < c_b)
                                                                            if (p[offsets[10]] < c_b)
                                                                                if (p[offsets[11]] < c_b)
                                                                                { }
                                                                                else
                                                                                    continue;
                                                                            else
                                                                                continue;
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                            else
                                                continue;
                                    else
                                        if (p[offsets[11]] > cb)
                                            if (p[offsets[7]] > cb)
                                                if (p[offsets[8]] > cb)
                                                    if (p[offsets[9]] > cb)
                                                        if (p[offsets[10]] > cb)
                                                            if (p[offsets[12]] > cb)
                                                                if (p[offsets[13]] > cb)
                                                                    if (p[offsets[6]] > cb)
                                                                        if (p[offsets[5]] > cb)
                                                                        { }
                                                                        else
                                                                            if (p[offsets[14]] > cb)
                                                                            { }
                                                                            else
                                                                                continue;
                                                                    else
                                                                        if (p[offsets[14]] > cb)
                                                                            if (p[offsets[15]] > cb)
                                                                            { }
                                                                            else
                                                                                continue;
                                                                        else
                                                                            continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                            else
                                                continue;
                                        else if (p[offsets[11]] < c_b)
                                            if (p[offsets[12]] < c_b)
                                                if (p[offsets[13]] < c_b)
                                                    if (p[offsets[14]] < c_b)
                                                        if (p[offsets[15]] < c_b)
                                                        { }
                                                        else
                                                            if (p[offsets[6]] < c_b)
                                                                if (p[offsets[7]] < c_b)
                                                                    if (p[offsets[8]] < c_b)
                                                                        if (p[offsets[9]] < c_b)
                                                                            if (p[offsets[10]] < c_b)
                                                                            { }
                                                                            else
                                                                                continue;
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                    else
                                                        if (p[offsets[5]] < c_b)
                                                            if (p[offsets[6]] < c_b)
                                                                if (p[offsets[7]] < c_b)
                                                                    if (p[offsets[8]] < c_b)
                                                                        if (p[offsets[9]] < c_b)
                                                                            if (p[offsets[10]] < c_b)
                                                                            { }
                                                                            else
                                                                                continue;
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                else
                                                    continue;
                                            else
                                                continue;
                                        else
                                            continue;
                                else
                                    if (p[offsets[10]] > cb)
                                        if (p[offsets[7]] > cb)
                                            if (p[offsets[8]] > cb)
                                                if (p[offsets[9]] > cb)
                                                    if (p[offsets[11]] > cb)
                                                        if (p[offsets[12]] > cb)
                                                            if (p[offsets[6]] > cb)
                                                                if (p[offsets[5]] > cb)
                                                                    if (p[offsets[4]] > cb)
                                                                    { }
                                                                    else
                                                                        if (p[offsets[13]] > cb)
                                                                        { }
                                                                        else
                                                                            continue;
                                                                else
                                                                    if (p[offsets[13]] > cb)
                                                                        if (p[offsets[14]] > cb)
                                                                        { }
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                            else
                                                                if (p[offsets[13]] > cb)
                                                                    if (p[offsets[14]] > cb)
                                                                        if (p[offsets[15]] > cb)
                                                                        { }
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                            else
                                                continue;
                                        else
                                            continue;
                                    else if (p[offsets[10]] < c_b)
                                        if (p[offsets[11]] < c_b)
                                            if (p[offsets[12]] < c_b)
                                                if (p[offsets[13]] < c_b)
                                                    if (p[offsets[14]] < c_b)
                                                        if (p[offsets[15]] < c_b)
                                                        { }
                                                        else
                                                            if (p[offsets[6]] < c_b)
                                                                if (p[offsets[7]] < c_b)
                                                                    if (p[offsets[8]] < c_b)
                                                                        if (p[offsets[9]] < c_b)
                                                                        { }
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                    else
                                                        if (p[offsets[5]] < c_b)
                                                            if (p[offsets[6]] < c_b)
                                                                if (p[offsets[7]] < c_b)
                                                                    if (p[offsets[8]] < c_b)
                                                                        if (p[offsets[9]] < c_b)
                                                                        { }
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                else
                                                    if (p[offsets[4]] < c_b)
                                                        if (p[offsets[5]] < c_b)
                                                            if (p[offsets[6]] < c_b)
                                                                if (p[offsets[7]] < c_b)
                                                                    if (p[offsets[8]] < c_b)
                                                                        if (p[offsets[9]] < c_b)
                                                                        { }
                                                                        else
                                                                            continue;
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                            else
                                                continue;
                                        else
                                            continue;
                                    else
                                        continue;
                            else
                                if (p[offsets[9]] > cb)
                                    if (p[offsets[7]] > cb)
                                        if (p[offsets[8]] > cb)
                                            if (p[offsets[10]] > cb)
                                                if (p[offsets[11]] > cb)
                                                    if (p[offsets[6]] > cb)
                                                        if (p[offsets[5]] > cb)
                                                            if (p[offsets[4]] > cb)
                                                                if (p[offsets[3]] > cb)
                                                                { }
                                                                else
                                                                    if (p[offsets[12]] > cb)
                                                                    { }
                                                                    else
                                                                        continue;
                                                            else
                                                                if (p[offsets[12]] > cb)
                                                                    if (p[offsets[13]] > cb)
                                                                    { }
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                        else
                                                            if (p[offsets[12]] > cb)
                                                                if (p[offsets[13]] > cb)
                                                                    if (p[offsets[14]] > cb)
                                                                    { }
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                    else
                                                        if (p[offsets[12]] > cb)
                                                            if (p[offsets[13]] > cb)
                                                                if (p[offsets[14]] > cb)
                                                                    if (p[offsets[15]] > cb)
                                                                    { }
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                else
                                                    continue;
                                            else
                                                continue;
                                        else
                                            continue;
                                    else
                                        continue;
                                else if (p[offsets[9]] < c_b)
                                    if (p[offsets[10]] < c_b)
                                        if (p[offsets[11]] < c_b)
                                            if (p[offsets[12]] < c_b)
                                                if (p[offsets[13]] < c_b)
                                                    if (p[offsets[14]] < c_b)
                                                        if (p[offsets[15]] < c_b)
                                                        { }
                                                        else
                                                            if (p[offsets[6]] < c_b)
                                                                if (p[offsets[7]] < c_b)
                                                                    if (p[offsets[8]] < c_b)
                                                                    { }
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                    else
                                                        if (p[offsets[5]] < c_b)
                                                            if (p[offsets[6]] < c_b)
                                                                if (p[offsets[7]] < c_b)
                                                                    if (p[offsets[8]] < c_b)
                                                                    { }
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                else
                                                    if (p[offsets[4]] < c_b)
                                                        if (p[offsets[5]] < c_b)
                                                            if (p[offsets[6]] < c_b)
                                                                if (p[offsets[7]] < c_b)
                                                                    if (p[offsets[8]] < c_b)
                                                                    { }
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                            else
                                                if (p[offsets[3]] < c_b)
                                                    if (p[offsets[4]] < c_b)
                                                        if (p[offsets[5]] < c_b)
                                                            if (p[offsets[6]] < c_b)
                                                                if (p[offsets[7]] < c_b)
                                                                    if (p[offsets[8]] < c_b)
                                                                    { }
                                                                    else
                                                                        continue;
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                        else
                                            continue;
                                    else
                                        continue;
                                else
                                    continue;
                        else
                            if (p[offsets[8]] > cb)
                                if (p[offsets[7]] > cb)
                                    if (p[offsets[9]] > cb)
                                        if (p[offsets[10]] > cb)
                                            if (p[offsets[6]] > cb)
                                                if (p[offsets[5]] > cb)
                                                    if (p[offsets[4]] > cb)
                                                        if (p[offsets[3]] > cb)
                                                            if (p[offsets[2]] > cb)
                                                            { }
                                                            else
                                                                if (p[offsets[11]] > cb)
                                                                { }
                                                                else
                                                                    continue;
                                                        else
                                                            if (p[offsets[11]] > cb)
                                                                if (p[offsets[12]] > cb)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                    else
                                                        if (p[offsets[11]] > cb)
                                                            if (p[offsets[12]] > cb)
                                                                if (p[offsets[13]] > cb)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                else
                                                    if (p[offsets[11]] > cb)
                                                        if (p[offsets[12]] > cb)
                                                            if (p[offsets[13]] > cb)
                                                                if (p[offsets[14]] > cb)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                            else
                                                if (p[offsets[11]] > cb)
                                                    if (p[offsets[12]] > cb)
                                                        if (p[offsets[13]] > cb)
                                                            if (p[offsets[14]] > cb)
                                                                if (p[offsets[15]] > cb)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                        else
                                            continue;
                                    else
                                        continue;
                                else
                                    continue;
                            else if (p[offsets[8]] < c_b)
                                if (p[offsets[9]] < c_b)
                                    if (p[offsets[10]] < c_b)
                                        if (p[offsets[11]] < c_b)
                                            if (p[offsets[12]] < c_b)
                                                if (p[offsets[13]] < c_b)
                                                    if (p[offsets[14]] < c_b)
                                                        if (p[offsets[15]] < c_b)
                                                        { }
                                                        else
                                                            if (p[offsets[6]] < c_b)
                                                                if (p[offsets[7]] < c_b)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                    else
                                                        if (p[offsets[5]] < c_b)
                                                            if (p[offsets[6]] < c_b)
                                                                if (p[offsets[7]] < c_b)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                else
                                                    if (p[offsets[4]] < c_b)
                                                        if (p[offsets[5]] < c_b)
                                                            if (p[offsets[6]] < c_b)
                                                                if (p[offsets[7]] < c_b)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                            else
                                                if (p[offsets[3]] < c_b)
                                                    if (p[offsets[4]] < c_b)
                                                        if (p[offsets[5]] < c_b)
                                                            if (p[offsets[6]] < c_b)
                                                                if (p[offsets[7]] < c_b)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                        else
                                            if (p[offsets[2]] < c_b)
                                                if (p[offsets[3]] < c_b)
                                                    if (p[offsets[4]] < c_b)
                                                        if (p[offsets[5]] < c_b)
                                                            if (p[offsets[6]] < c_b)
                                                                if (p[offsets[7]] < c_b)
                                                                { }
                                                                else
                                                                    continue;
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                            else
                                                continue;
                                    else
                                        continue;
                                else
                                    continue;
                            else
                                continue;
                    else
                        if (p[offsets[7]] > cb)
                            if (p[offsets[8]] > cb)
                                if (p[offsets[9]] > cb)
                                    if (p[offsets[6]] > cb)
                                        if (p[offsets[5]] > cb)
                                            if (p[offsets[4]] > cb)
                                                if (p[offsets[3]] > cb)
                                                    if (p[offsets[2]] > cb)
                                                        if (p[offsets[1]] > cb)
                                                        { }
                                                        else
                                                            if (p[offsets[10]] > cb)
                                                            { }
                                                            else
                                                                continue;
                                                    else
                                                        if (p[offsets[10]] > cb)
                                                            if (p[offsets[11]] > cb)
                                                            { }
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                else
                                                    if (p[offsets[10]] > cb)
                                                        if (p[offsets[11]] > cb)
                                                            if (p[offsets[12]] > cb)
                                                            { }
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                            else
                                                if (p[offsets[10]] > cb)
                                                    if (p[offsets[11]] > cb)
                                                        if (p[offsets[12]] > cb)
                                                            if (p[offsets[13]] > cb)
                                                            { }
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                        else
                                            if (p[offsets[10]] > cb)
                                                if (p[offsets[11]] > cb)
                                                    if (p[offsets[12]] > cb)
                                                        if (p[offsets[13]] > cb)
                                                            if (p[offsets[14]] > cb)
                                                            { }
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                            else
                                                continue;
                                    else
                                        if (p[offsets[10]] > cb)
                                            if (p[offsets[11]] > cb)
                                                if (p[offsets[12]] > cb)
                                                    if (p[offsets[13]] > cb)
                                                        if (p[offsets[14]] > cb)
                                                            if (p[offsets[15]] > cb)
                                                            { }
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                            else
                                                continue;
                                        else
                                            continue;
                                else
                                    continue;
                            else
                                continue;
                        else if (p[offsets[7]] < c_b)
                            if (p[offsets[8]] < c_b)
                                if (p[offsets[9]] < c_b)
                                    if (p[offsets[6]] < c_b)
                                        if (p[offsets[5]] < c_b)
                                            if (p[offsets[4]] < c_b)
                                                if (p[offsets[3]] < c_b)
                                                    if (p[offsets[2]] < c_b)
                                                        if (p[offsets[1]] < c_b)
                                                        { }
                                                        else
                                                            if (p[offsets[10]] < c_b)
                                                            { }
                                                            else
                                                                continue;
                                                    else
                                                        if (p[offsets[10]] < c_b)
                                                            if (p[offsets[11]] < c_b)
                                                            { }
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                else
                                                    if (p[offsets[10]] < c_b)
                                                        if (p[offsets[11]] < c_b)
                                                            if (p[offsets[12]] < c_b)
                                                            { }
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                            else
                                                if (p[offsets[10]] < c_b)
                                                    if (p[offsets[11]] < c_b)
                                                        if (p[offsets[12]] < c_b)
                                                            if (p[offsets[13]] < c_b)
                                                            { }
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                        else
                                            if (p[offsets[10]] < c_b)
                                                if (p[offsets[11]] < c_b)
                                                    if (p[offsets[12]] < c_b)
                                                        if (p[offsets[13]] < c_b)
                                                            if (p[offsets[14]] < c_b)
                                                            { }
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                            else
                                                continue;
                                    else
                                        if (p[offsets[10]] < c_b)
                                            if (p[offsets[11]] < c_b)
                                                if (p[offsets[12]] < c_b)
                                                    if (p[offsets[13]] < c_b)
                                                        if (p[offsets[14]] < c_b)
                                                            if (p[offsets[15]] < c_b)
                                                            { }
                                                            else
                                                                continue;
                                                        else
                                                            continue;
                                                    else
                                                        continue;
                                                else
                                                    continue;
                                            else
                                                continue;
                                        else
                                            continue;
                                else
                                    continue;
                            else
                                continue;
                        else
                            continue;

                    #endregion

                    points.Add(new IntPoint(x, y));
                }
                p += offset + 6;
            }

            return points.ToArray();
        }

        private unsafe int score(UnmanagedImage image, IntPoint corner, int[] offsets)
        {
            byte* src = (byte*)image.ImageData.ToPointer();
            byte* p = src + corner.Y * image.Stride + corner.X;

            // Compute the score using binary search
            int bmin = this.threshold, bmax = 255;
            int b = (bmax + bmin) / 2;

            for (; ; )
            {
                int cb = *p + b;
                int c_b = *p - b;

                #region Machine generated code
                if (p[offsets[0]] > cb)
                    if (p[offsets[1]] > cb)
                        if (p[offsets[2]] > cb)
                            if (p[offsets[3]] > cb)
                                if (p[offsets[4]] > cb)
                                    if (p[offsets[5]] > cb)
                                        if (p[offsets[6]] > cb)
                                            if (p[offsets[7]] > cb)
                                                if (p[offsets[8]] > cb)
                                                    goto is_a_corner;
                                                else
                                                    if (p[offsets[15]] > cb)
                                                        goto is_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                            else if (p[offsets[7]] < c_b)
                                                if (p[offsets[14]] > cb)
                                                    if (p[offsets[15]] > cb)
                                                        goto is_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else if (p[offsets[14]] < c_b)
                                                    if (p[offsets[8]] < c_b)
                                                        if (p[offsets[9]] < c_b)
                                                            if (p[offsets[10]] < c_b)
                                                                if (p[offsets[11]] < c_b)
                                                                    if (p[offsets[12]] < c_b)
                                                                        if (p[offsets[13]] < c_b)
                                                                            if (p[offsets[15]] < c_b)
                                                                                goto is_a_corner;
                                                                            else
                                                                                goto is_not_a_corner;
                                                                        else
                                                                            goto is_not_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                if (p[offsets[14]] > cb)
                                                    if (p[offsets[15]] > cb)
                                                        goto is_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                        else if (p[offsets[6]] < c_b)
                                            if (p[offsets[15]] > cb)
                                                if (p[offsets[13]] > cb)
                                                    if (p[offsets[14]] > cb)
                                                        goto is_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else if (p[offsets[13]] < c_b)
                                                    if (p[offsets[7]] < c_b)
                                                        if (p[offsets[8]] < c_b)
                                                            if (p[offsets[9]] < c_b)
                                                                if (p[offsets[10]] < c_b)
                                                                    if (p[offsets[11]] < c_b)
                                                                        if (p[offsets[12]] < c_b)
                                                                            if (p[offsets[14]] < c_b)
                                                                                goto is_a_corner;
                                                                            else
                                                                                goto is_not_a_corner;
                                                                        else
                                                                            goto is_not_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                if (p[offsets[7]] < c_b)
                                                    if (p[offsets[8]] < c_b)
                                                        if (p[offsets[9]] < c_b)
                                                            if (p[offsets[10]] < c_b)
                                                                if (p[offsets[11]] < c_b)
                                                                    if (p[offsets[12]] < c_b)
                                                                        if (p[offsets[13]] < c_b)
                                                                            if (p[offsets[14]] < c_b)
                                                                                goto is_a_corner;
                                                                            else
                                                                                goto is_not_a_corner;
                                                                        else
                                                                            goto is_not_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                        else
                                            if (p[offsets[13]] > cb)
                                                if (p[offsets[14]] > cb)
                                                    if (p[offsets[15]] > cb)
                                                        goto is_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else if (p[offsets[13]] < c_b)
                                                if (p[offsets[7]] < c_b)
                                                    if (p[offsets[8]] < c_b)
                                                        if (p[offsets[9]] < c_b)
                                                            if (p[offsets[10]] < c_b)
                                                                if (p[offsets[11]] < c_b)
                                                                    if (p[offsets[12]] < c_b)
                                                                        if (p[offsets[14]] < c_b)
                                                                            if (p[offsets[15]] < c_b)
                                                                                goto is_a_corner;
                                                                            else
                                                                                goto is_not_a_corner;
                                                                        else
                                                                            goto is_not_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                    else if (p[offsets[5]] < c_b)
                                        if (p[offsets[14]] > cb)
                                            if (p[offsets[12]] > cb)
                                                if (p[offsets[13]] > cb)
                                                    if (p[offsets[15]] > cb)
                                                        goto is_a_corner;
                                                    else
                                                        if (p[offsets[6]] > cb)
                                                            if (p[offsets[7]] > cb)
                                                                if (p[offsets[8]] > cb)
                                                                    if (p[offsets[9]] > cb)
                                                                        if (p[offsets[10]] > cb)
                                                                            if (p[offsets[11]] > cb)
                                                                                goto is_a_corner;
                                                                            else
                                                                                goto is_not_a_corner;
                                                                        else
                                                                            goto is_not_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else if (p[offsets[12]] < c_b)
                                                if (p[offsets[6]] < c_b)
                                                    if (p[offsets[7]] < c_b)
                                                        if (p[offsets[8]] < c_b)
                                                            if (p[offsets[9]] < c_b)
                                                                if (p[offsets[10]] < c_b)
                                                                    if (p[offsets[11]] < c_b)
                                                                        if (p[offsets[13]] < c_b)
                                                                            goto is_a_corner;
                                                                        else
                                                                            goto is_not_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                        else if (p[offsets[14]] < c_b)
                                            if (p[offsets[7]] < c_b)
                                                if (p[offsets[8]] < c_b)
                                                    if (p[offsets[9]] < c_b)
                                                        if (p[offsets[10]] < c_b)
                                                            if (p[offsets[11]] < c_b)
                                                                if (p[offsets[12]] < c_b)
                                                                    if (p[offsets[13]] < c_b)
                                                                        if (p[offsets[6]] < c_b)
                                                                            goto is_a_corner;
                                                                        else
                                                                            if (p[offsets[15]] < c_b)
                                                                                goto is_a_corner;
                                                                            else
                                                                                goto is_not_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                        else
                                            if (p[offsets[6]] < c_b)
                                                if (p[offsets[7]] < c_b)
                                                    if (p[offsets[8]] < c_b)
                                                        if (p[offsets[9]] < c_b)
                                                            if (p[offsets[10]] < c_b)
                                                                if (p[offsets[11]] < c_b)
                                                                    if (p[offsets[12]] < c_b)
                                                                        if (p[offsets[13]] < c_b)
                                                                            goto is_a_corner;
                                                                        else
                                                                            goto is_not_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                    else
                                        if (p[offsets[12]] > cb)
                                            if (p[offsets[13]] > cb)
                                                if (p[offsets[14]] > cb)
                                                    if (p[offsets[15]] > cb)
                                                        goto is_a_corner;
                                                    else
                                                        if (p[offsets[6]] > cb)
                                                            if (p[offsets[7]] > cb)
                                                                if (p[offsets[8]] > cb)
                                                                    if (p[offsets[9]] > cb)
                                                                        if (p[offsets[10]] > cb)
                                                                            if (p[offsets[11]] > cb)
                                                                                goto is_a_corner;
                                                                            else
                                                                                goto is_not_a_corner;
                                                                        else
                                                                            goto is_not_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                        else if (p[offsets[12]] < c_b)
                                            if (p[offsets[7]] < c_b)
                                                if (p[offsets[8]] < c_b)
                                                    if (p[offsets[9]] < c_b)
                                                        if (p[offsets[10]] < c_b)
                                                            if (p[offsets[11]] < c_b)
                                                                if (p[offsets[13]] < c_b)
                                                                    if (p[offsets[14]] < c_b)
                                                                        if (p[offsets[6]] < c_b)
                                                                            goto is_a_corner;
                                                                        else
                                                                            if (p[offsets[15]] < c_b)
                                                                                goto is_a_corner;
                                                                            else
                                                                                goto is_not_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                        else
                                            goto is_not_a_corner;
                                else if (p[offsets[4]] < c_b)
                                    if (p[offsets[13]] > cb)
                                        if (p[offsets[11]] > cb)
                                            if (p[offsets[12]] > cb)
                                                if (p[offsets[14]] > cb)
                                                    if (p[offsets[15]] > cb)
                                                        goto is_a_corner;
                                                    else
                                                        if (p[offsets[6]] > cb)
                                                            if (p[offsets[7]] > cb)
                                                                if (p[offsets[8]] > cb)
                                                                    if (p[offsets[9]] > cb)
                                                                        if (p[offsets[10]] > cb)
                                                                            goto is_a_corner;
                                                                        else
                                                                            goto is_not_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                else
                                                    if (p[offsets[5]] > cb)
                                                        if (p[offsets[6]] > cb)
                                                            if (p[offsets[7]] > cb)
                                                                if (p[offsets[8]] > cb)
                                                                    if (p[offsets[9]] > cb)
                                                                        if (p[offsets[10]] > cb)
                                                                            goto is_a_corner;
                                                                        else
                                                                            goto is_not_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                        else if (p[offsets[11]] < c_b)
                                            if (p[offsets[5]] < c_b)
                                                if (p[offsets[6]] < c_b)
                                                    if (p[offsets[7]] < c_b)
                                                        if (p[offsets[8]] < c_b)
                                                            if (p[offsets[9]] < c_b)
                                                                if (p[offsets[10]] < c_b)
                                                                    if (p[offsets[12]] < c_b)
                                                                        goto is_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                        else
                                            goto is_not_a_corner;
                                    else if (p[offsets[13]] < c_b)
                                        if (p[offsets[7]] < c_b)
                                            if (p[offsets[8]] < c_b)
                                                if (p[offsets[9]] < c_b)
                                                    if (p[offsets[10]] < c_b)
                                                        if (p[offsets[11]] < c_b)
                                                            if (p[offsets[12]] < c_b)
                                                                if (p[offsets[6]] < c_b)
                                                                    if (p[offsets[5]] < c_b)
                                                                        goto is_a_corner;
                                                                    else
                                                                        if (p[offsets[14]] < c_b)
                                                                            goto is_a_corner;
                                                                        else
                                                                            goto is_not_a_corner;
                                                                else
                                                                    if (p[offsets[14]] < c_b)
                                                                        if (p[offsets[15]] < c_b)
                                                                            goto is_a_corner;
                                                                        else
                                                                            goto is_not_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                        else
                                            goto is_not_a_corner;
                                    else
                                        if (p[offsets[5]] < c_b)
                                            if (p[offsets[6]] < c_b)
                                                if (p[offsets[7]] < c_b)
                                                    if (p[offsets[8]] < c_b)
                                                        if (p[offsets[9]] < c_b)
                                                            if (p[offsets[10]] < c_b)
                                                                if (p[offsets[11]] < c_b)
                                                                    if (p[offsets[12]] < c_b)
                                                                        goto is_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                        else
                                            goto is_not_a_corner;
                                else
                                    if (p[offsets[11]] > cb)
                                        if (p[offsets[12]] > cb)
                                            if (p[offsets[13]] > cb)
                                                if (p[offsets[14]] > cb)
                                                    if (p[offsets[15]] > cb)
                                                        goto is_a_corner;
                                                    else
                                                        if (p[offsets[6]] > cb)
                                                            if (p[offsets[7]] > cb)
                                                                if (p[offsets[8]] > cb)
                                                                    if (p[offsets[9]] > cb)
                                                                        if (p[offsets[10]] > cb)
                                                                            goto is_a_corner;
                                                                        else
                                                                            goto is_not_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                else
                                                    if (p[offsets[5]] > cb)
                                                        if (p[offsets[6]] > cb)
                                                            if (p[offsets[7]] > cb)
                                                                if (p[offsets[8]] > cb)
                                                                    if (p[offsets[9]] > cb)
                                                                        if (p[offsets[10]] > cb)
                                                                            goto is_a_corner;
                                                                        else
                                                                            goto is_not_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                        else
                                            goto is_not_a_corner;
                                    else if (p[offsets[11]] < c_b)
                                        if (p[offsets[7]] < c_b)
                                            if (p[offsets[8]] < c_b)
                                                if (p[offsets[9]] < c_b)
                                                    if (p[offsets[10]] < c_b)
                                                        if (p[offsets[12]] < c_b)
                                                            if (p[offsets[13]] < c_b)
                                                                if (p[offsets[6]] < c_b)
                                                                    if (p[offsets[5]] < c_b)
                                                                        goto is_a_corner;
                                                                    else
                                                                        if (p[offsets[14]] < c_b)
                                                                            goto is_a_corner;
                                                                        else
                                                                            goto is_not_a_corner;
                                                                else
                                                                    if (p[offsets[14]] < c_b)
                                                                        if (p[offsets[15]] < c_b)
                                                                            goto is_a_corner;
                                                                        else
                                                                            goto is_not_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                        else
                                            goto is_not_a_corner;
                                    else
                                        goto is_not_a_corner;
                            else if (p[offsets[3]] < c_b)
                                if (p[offsets[10]] > cb)
                                    if (p[offsets[11]] > cb)
                                        if (p[offsets[12]] > cb)
                                            if (p[offsets[13]] > cb)
                                                if (p[offsets[14]] > cb)
                                                    if (p[offsets[15]] > cb)
                                                        goto is_a_corner;
                                                    else
                                                        if (p[offsets[6]] > cb)
                                                            if (p[offsets[7]] > cb)
                                                                if (p[offsets[8]] > cb)
                                                                    if (p[offsets[9]] > cb)
                                                                        goto is_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                else
                                                    if (p[offsets[5]] > cb)
                                                        if (p[offsets[6]] > cb)
                                                            if (p[offsets[7]] > cb)
                                                                if (p[offsets[8]] > cb)
                                                                    if (p[offsets[9]] > cb)
                                                                        goto is_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                            else
                                                if (p[offsets[4]] > cb)
                                                    if (p[offsets[5]] > cb)
                                                        if (p[offsets[6]] > cb)
                                                            if (p[offsets[7]] > cb)
                                                                if (p[offsets[8]] > cb)
                                                                    if (p[offsets[9]] > cb)
                                                                        goto is_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                        else
                                            goto is_not_a_corner;
                                    else
                                        goto is_not_a_corner;
                                else if (p[offsets[10]] < c_b)
                                    if (p[offsets[7]] < c_b)
                                        if (p[offsets[8]] < c_b)
                                            if (p[offsets[9]] < c_b)
                                                if (p[offsets[11]] < c_b)
                                                    if (p[offsets[6]] < c_b)
                                                        if (p[offsets[5]] < c_b)
                                                            if (p[offsets[4]] < c_b)
                                                                goto is_a_corner;
                                                            else
                                                                if (p[offsets[12]] < c_b)
                                                                    if (p[offsets[13]] < c_b)
                                                                        goto is_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                        else
                                                            if (p[offsets[12]] < c_b)
                                                                if (p[offsets[13]] < c_b)
                                                                    if (p[offsets[14]] < c_b)
                                                                        goto is_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                    else
                                                        if (p[offsets[12]] < c_b)
                                                            if (p[offsets[13]] < c_b)
                                                                if (p[offsets[14]] < c_b)
                                                                    if (p[offsets[15]] < c_b)
                                                                        goto is_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                        else
                                            goto is_not_a_corner;
                                    else
                                        goto is_not_a_corner;
                                else
                                    goto is_not_a_corner;
                            else
                                if (p[offsets[10]] > cb)
                                    if (p[offsets[11]] > cb)
                                        if (p[offsets[12]] > cb)
                                            if (p[offsets[13]] > cb)
                                                if (p[offsets[14]] > cb)
                                                    if (p[offsets[15]] > cb)
                                                        goto is_a_corner;
                                                    else
                                                        if (p[offsets[6]] > cb)
                                                            if (p[offsets[7]] > cb)
                                                                if (p[offsets[8]] > cb)
                                                                    if (p[offsets[9]] > cb)
                                                                        goto is_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                else
                                                    if (p[offsets[5]] > cb)
                                                        if (p[offsets[6]] > cb)
                                                            if (p[offsets[7]] > cb)
                                                                if (p[offsets[8]] > cb)
                                                                    if (p[offsets[9]] > cb)
                                                                        goto is_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                            else
                                                if (p[offsets[4]] > cb)
                                                    if (p[offsets[5]] > cb)
                                                        if (p[offsets[6]] > cb)
                                                            if (p[offsets[7]] > cb)
                                                                if (p[offsets[8]] > cb)
                                                                    if (p[offsets[9]] > cb)
                                                                        goto is_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                        else
                                            goto is_not_a_corner;
                                    else
                                        goto is_not_a_corner;
                                else if (p[offsets[10]] < c_b)
                                    if (p[offsets[7]] < c_b)
                                        if (p[offsets[8]] < c_b)
                                            if (p[offsets[9]] < c_b)
                                                if (p[offsets[11]] < c_b)
                                                    if (p[offsets[12]] < c_b)
                                                        if (p[offsets[6]] < c_b)
                                                            if (p[offsets[5]] < c_b)
                                                                if (p[offsets[4]] < c_b)
                                                                    goto is_a_corner;
                                                                else
                                                                    if (p[offsets[13]] < c_b)
                                                                        goto is_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                            else
                                                                if (p[offsets[13]] < c_b)
                                                                    if (p[offsets[14]] < c_b)
                                                                        goto is_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                        else
                                                            if (p[offsets[13]] < c_b)
                                                                if (p[offsets[14]] < c_b)
                                                                    if (p[offsets[15]] < c_b)
                                                                        goto is_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                        else
                                            goto is_not_a_corner;
                                    else
                                        goto is_not_a_corner;
                                else
                                    goto is_not_a_corner;
                        else if (p[offsets[2]] < c_b)
                            if (p[offsets[9]] > cb)
                                if (p[offsets[10]] > cb)
                                    if (p[offsets[11]] > cb)
                                        if (p[offsets[12]] > cb)
                                            if (p[offsets[13]] > cb)
                                                if (p[offsets[14]] > cb)
                                                    if (p[offsets[15]] > cb)
                                                        goto is_a_corner;
                                                    else
                                                        if (p[offsets[6]] > cb)
                                                            if (p[offsets[7]] > cb)
                                                                if (p[offsets[8]] > cb)
                                                                    goto is_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                else
                                                    if (p[offsets[5]] > cb)
                                                        if (p[offsets[6]] > cb)
                                                            if (p[offsets[7]] > cb)
                                                                if (p[offsets[8]] > cb)
                                                                    goto is_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                            else
                                                if (p[offsets[4]] > cb)
                                                    if (p[offsets[5]] > cb)
                                                        if (p[offsets[6]] > cb)
                                                            if (p[offsets[7]] > cb)
                                                                if (p[offsets[8]] > cb)
                                                                    goto is_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                        else
                                            if (p[offsets[3]] > cb)
                                                if (p[offsets[4]] > cb)
                                                    if (p[offsets[5]] > cb)
                                                        if (p[offsets[6]] > cb)
                                                            if (p[offsets[7]] > cb)
                                                                if (p[offsets[8]] > cb)
                                                                    goto is_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                    else
                                        goto is_not_a_corner;
                                else
                                    goto is_not_a_corner;
                            else if (p[offsets[9]] < c_b)
                                if (p[offsets[7]] < c_b)
                                    if (p[offsets[8]] < c_b)
                                        if (p[offsets[10]] < c_b)
                                            if (p[offsets[6]] < c_b)
                                                if (p[offsets[5]] < c_b)
                                                    if (p[offsets[4]] < c_b)
                                                        if (p[offsets[3]] < c_b)
                                                            goto is_a_corner;
                                                        else
                                                            if (p[offsets[11]] < c_b)
                                                                if (p[offsets[12]] < c_b)
                                                                    goto is_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                    else
                                                        if (p[offsets[11]] < c_b)
                                                            if (p[offsets[12]] < c_b)
                                                                if (p[offsets[13]] < c_b)
                                                                    goto is_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                else
                                                    if (p[offsets[11]] < c_b)
                                                        if (p[offsets[12]] < c_b)
                                                            if (p[offsets[13]] < c_b)
                                                                if (p[offsets[14]] < c_b)
                                                                    goto is_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                            else
                                                if (p[offsets[11]] < c_b)
                                                    if (p[offsets[12]] < c_b)
                                                        if (p[offsets[13]] < c_b)
                                                            if (p[offsets[14]] < c_b)
                                                                if (p[offsets[15]] < c_b)
                                                                    goto is_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                        else
                                            goto is_not_a_corner;
                                    else
                                        goto is_not_a_corner;
                                else
                                    goto is_not_a_corner;
                            else
                                goto is_not_a_corner;
                        else
                            if (p[offsets[9]] > cb)
                                if (p[offsets[10]] > cb)
                                    if (p[offsets[11]] > cb)
                                        if (p[offsets[12]] > cb)
                                            if (p[offsets[13]] > cb)
                                                if (p[offsets[14]] > cb)
                                                    if (p[offsets[15]] > cb)
                                                        goto is_a_corner;
                                                    else
                                                        if (p[offsets[6]] > cb)
                                                            if (p[offsets[7]] > cb)
                                                                if (p[offsets[8]] > cb)
                                                                    goto is_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                else
                                                    if (p[offsets[5]] > cb)
                                                        if (p[offsets[6]] > cb)
                                                            if (p[offsets[7]] > cb)
                                                                if (p[offsets[8]] > cb)
                                                                    goto is_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                            else
                                                if (p[offsets[4]] > cb)
                                                    if (p[offsets[5]] > cb)
                                                        if (p[offsets[6]] > cb)
                                                            if (p[offsets[7]] > cb)
                                                                if (p[offsets[8]] > cb)
                                                                    goto is_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                        else
                                            if (p[offsets[3]] > cb)
                                                if (p[offsets[4]] > cb)
                                                    if (p[offsets[5]] > cb)
                                                        if (p[offsets[6]] > cb)
                                                            if (p[offsets[7]] > cb)
                                                                if (p[offsets[8]] > cb)
                                                                    goto is_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                    else
                                        goto is_not_a_corner;
                                else
                                    goto is_not_a_corner;
                            else if (p[offsets[9]] < c_b)
                                if (p[offsets[7]] < c_b)
                                    if (p[offsets[8]] < c_b)
                                        if (p[offsets[10]] < c_b)
                                            if (p[offsets[11]] < c_b)
                                                if (p[offsets[6]] < c_b)
                                                    if (p[offsets[5]] < c_b)
                                                        if (p[offsets[4]] < c_b)
                                                            if (p[offsets[3]] < c_b)
                                                                goto is_a_corner;
                                                            else
                                                                if (p[offsets[12]] < c_b)
                                                                    goto is_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                        else
                                                            if (p[offsets[12]] < c_b)
                                                                if (p[offsets[13]] < c_b)
                                                                    goto is_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                    else
                                                        if (p[offsets[12]] < c_b)
                                                            if (p[offsets[13]] < c_b)
                                                                if (p[offsets[14]] < c_b)
                                                                    goto is_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                else
                                                    if (p[offsets[12]] < c_b)
                                                        if (p[offsets[13]] < c_b)
                                                            if (p[offsets[14]] < c_b)
                                                                if (p[offsets[15]] < c_b)
                                                                    goto is_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                        else
                                            goto is_not_a_corner;
                                    else
                                        goto is_not_a_corner;
                                else
                                    goto is_not_a_corner;
                            else
                                goto is_not_a_corner;
                    else if (p[offsets[1]] < c_b)
                        if (p[offsets[8]] > cb)
                            if (p[offsets[9]] > cb)
                                if (p[offsets[10]] > cb)
                                    if (p[offsets[11]] > cb)
                                        if (p[offsets[12]] > cb)
                                            if (p[offsets[13]] > cb)
                                                if (p[offsets[14]] > cb)
                                                    if (p[offsets[15]] > cb)
                                                        goto is_a_corner;
                                                    else
                                                        if (p[offsets[6]] > cb)
                                                            if (p[offsets[7]] > cb)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                else
                                                    if (p[offsets[5]] > cb)
                                                        if (p[offsets[6]] > cb)
                                                            if (p[offsets[7]] > cb)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                            else
                                                if (p[offsets[4]] > cb)
                                                    if (p[offsets[5]] > cb)
                                                        if (p[offsets[6]] > cb)
                                                            if (p[offsets[7]] > cb)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                        else
                                            if (p[offsets[3]] > cb)
                                                if (p[offsets[4]] > cb)
                                                    if (p[offsets[5]] > cb)
                                                        if (p[offsets[6]] > cb)
                                                            if (p[offsets[7]] > cb)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                    else
                                        if (p[offsets[2]] > cb)
                                            if (p[offsets[3]] > cb)
                                                if (p[offsets[4]] > cb)
                                                    if (p[offsets[5]] > cb)
                                                        if (p[offsets[6]] > cb)
                                                            if (p[offsets[7]] > cb)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                        else
                                            goto is_not_a_corner;
                                else
                                    goto is_not_a_corner;
                            else
                                goto is_not_a_corner;
                        else if (p[offsets[8]] < c_b)
                            if (p[offsets[7]] < c_b)
                                if (p[offsets[9]] < c_b)
                                    if (p[offsets[6]] < c_b)
                                        if (p[offsets[5]] < c_b)
                                            if (p[offsets[4]] < c_b)
                                                if (p[offsets[3]] < c_b)
                                                    if (p[offsets[2]] < c_b)
                                                        goto is_a_corner;
                                                    else
                                                        if (p[offsets[10]] < c_b)
                                                            if (p[offsets[11]] < c_b)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                else
                                                    if (p[offsets[10]] < c_b)
                                                        if (p[offsets[11]] < c_b)
                                                            if (p[offsets[12]] < c_b)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                            else
                                                if (p[offsets[10]] < c_b)
                                                    if (p[offsets[11]] < c_b)
                                                        if (p[offsets[12]] < c_b)
                                                            if (p[offsets[13]] < c_b)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                        else
                                            if (p[offsets[10]] < c_b)
                                                if (p[offsets[11]] < c_b)
                                                    if (p[offsets[12]] < c_b)
                                                        if (p[offsets[13]] < c_b)
                                                            if (p[offsets[14]] < c_b)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                    else
                                        if (p[offsets[10]] < c_b)
                                            if (p[offsets[11]] < c_b)
                                                if (p[offsets[12]] < c_b)
                                                    if (p[offsets[13]] < c_b)
                                                        if (p[offsets[14]] < c_b)
                                                            if (p[offsets[15]] < c_b)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                        else
                                            goto is_not_a_corner;
                                else
                                    goto is_not_a_corner;
                            else
                                goto is_not_a_corner;
                        else
                            goto is_not_a_corner;
                    else
                        if (p[offsets[8]] > cb)
                            if (p[offsets[9]] > cb)
                                if (p[offsets[10]] > cb)
                                    if (p[offsets[11]] > cb)
                                        if (p[offsets[12]] > cb)
                                            if (p[offsets[13]] > cb)
                                                if (p[offsets[14]] > cb)
                                                    if (p[offsets[15]] > cb)
                                                        goto is_a_corner;
                                                    else
                                                        if (p[offsets[6]] > cb)
                                                            if (p[offsets[7]] > cb)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                else
                                                    if (p[offsets[5]] > cb)
                                                        if (p[offsets[6]] > cb)
                                                            if (p[offsets[7]] > cb)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                            else
                                                if (p[offsets[4]] > cb)
                                                    if (p[offsets[5]] > cb)
                                                        if (p[offsets[6]] > cb)
                                                            if (p[offsets[7]] > cb)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                        else
                                            if (p[offsets[3]] > cb)
                                                if (p[offsets[4]] > cb)
                                                    if (p[offsets[5]] > cb)
                                                        if (p[offsets[6]] > cb)
                                                            if (p[offsets[7]] > cb)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                    else
                                        if (p[offsets[2]] > cb)
                                            if (p[offsets[3]] > cb)
                                                if (p[offsets[4]] > cb)
                                                    if (p[offsets[5]] > cb)
                                                        if (p[offsets[6]] > cb)
                                                            if (p[offsets[7]] > cb)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                        else
                                            goto is_not_a_corner;
                                else
                                    goto is_not_a_corner;
                            else
                                goto is_not_a_corner;
                        else if (p[offsets[8]] < c_b)
                            if (p[offsets[7]] < c_b)
                                if (p[offsets[9]] < c_b)
                                    if (p[offsets[10]] < c_b)
                                        if (p[offsets[6]] < c_b)
                                            if (p[offsets[5]] < c_b)
                                                if (p[offsets[4]] < c_b)
                                                    if (p[offsets[3]] < c_b)
                                                        if (p[offsets[2]] < c_b)
                                                            goto is_a_corner;
                                                        else
                                                            if (p[offsets[11]] < c_b)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                    else
                                                        if (p[offsets[11]] < c_b)
                                                            if (p[offsets[12]] < c_b)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                else
                                                    if (p[offsets[11]] < c_b)
                                                        if (p[offsets[12]] < c_b)
                                                            if (p[offsets[13]] < c_b)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                            else
                                                if (p[offsets[11]] < c_b)
                                                    if (p[offsets[12]] < c_b)
                                                        if (p[offsets[13]] < c_b)
                                                            if (p[offsets[14]] < c_b)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                        else
                                            if (p[offsets[11]] < c_b)
                                                if (p[offsets[12]] < c_b)
                                                    if (p[offsets[13]] < c_b)
                                                        if (p[offsets[14]] < c_b)
                                                            if (p[offsets[15]] < c_b)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                    else
                                        goto is_not_a_corner;
                                else
                                    goto is_not_a_corner;
                            else
                                goto is_not_a_corner;
                        else
                            goto is_not_a_corner;
                else if (p[offsets[0]] < c_b)
                    if (p[offsets[1]] > cb)
                        if (p[offsets[8]] > cb)
                            if (p[offsets[7]] > cb)
                                if (p[offsets[9]] > cb)
                                    if (p[offsets[6]] > cb)
                                        if (p[offsets[5]] > cb)
                                            if (p[offsets[4]] > cb)
                                                if (p[offsets[3]] > cb)
                                                    if (p[offsets[2]] > cb)
                                                        goto is_a_corner;
                                                    else
                                                        if (p[offsets[10]] > cb)
                                                            if (p[offsets[11]] > cb)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                else
                                                    if (p[offsets[10]] > cb)
                                                        if (p[offsets[11]] > cb)
                                                            if (p[offsets[12]] > cb)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                            else
                                                if (p[offsets[10]] > cb)
                                                    if (p[offsets[11]] > cb)
                                                        if (p[offsets[12]] > cb)
                                                            if (p[offsets[13]] > cb)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                        else
                                            if (p[offsets[10]] > cb)
                                                if (p[offsets[11]] > cb)
                                                    if (p[offsets[12]] > cb)
                                                        if (p[offsets[13]] > cb)
                                                            if (p[offsets[14]] > cb)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                    else
                                        if (p[offsets[10]] > cb)
                                            if (p[offsets[11]] > cb)
                                                if (p[offsets[12]] > cb)
                                                    if (p[offsets[13]] > cb)
                                                        if (p[offsets[14]] > cb)
                                                            if (p[offsets[15]] > cb)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                        else
                                            goto is_not_a_corner;
                                else
                                    goto is_not_a_corner;
                            else
                                goto is_not_a_corner;
                        else if (p[offsets[8]] < c_b)
                            if (p[offsets[9]] < c_b)
                                if (p[offsets[10]] < c_b)
                                    if (p[offsets[11]] < c_b)
                                        if (p[offsets[12]] < c_b)
                                            if (p[offsets[13]] < c_b)
                                                if (p[offsets[14]] < c_b)
                                                    if (p[offsets[15]] < c_b)
                                                        goto is_a_corner;
                                                    else
                                                        if (p[offsets[6]] < c_b)
                                                            if (p[offsets[7]] < c_b)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                else
                                                    if (p[offsets[5]] < c_b)
                                                        if (p[offsets[6]] < c_b)
                                                            if (p[offsets[7]] < c_b)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                            else
                                                if (p[offsets[4]] < c_b)
                                                    if (p[offsets[5]] < c_b)
                                                        if (p[offsets[6]] < c_b)
                                                            if (p[offsets[7]] < c_b)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                        else
                                            if (p[offsets[3]] < c_b)
                                                if (p[offsets[4]] < c_b)
                                                    if (p[offsets[5]] < c_b)
                                                        if (p[offsets[6]] < c_b)
                                                            if (p[offsets[7]] < c_b)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                    else
                                        if (p[offsets[2]] < c_b)
                                            if (p[offsets[3]] < c_b)
                                                if (p[offsets[4]] < c_b)
                                                    if (p[offsets[5]] < c_b)
                                                        if (p[offsets[6]] < c_b)
                                                            if (p[offsets[7]] < c_b)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                        else
                                            goto is_not_a_corner;
                                else
                                    goto is_not_a_corner;
                            else
                                goto is_not_a_corner;
                        else
                            goto is_not_a_corner;
                    else if (p[offsets[1]] < c_b)
                        if (p[offsets[2]] > cb)
                            if (p[offsets[9]] > cb)
                                if (p[offsets[7]] > cb)
                                    if (p[offsets[8]] > cb)
                                        if (p[offsets[10]] > cb)
                                            if (p[offsets[6]] > cb)
                                                if (p[offsets[5]] > cb)
                                                    if (p[offsets[4]] > cb)
                                                        if (p[offsets[3]] > cb)
                                                            goto is_a_corner;
                                                        else
                                                            if (p[offsets[11]] > cb)
                                                                if (p[offsets[12]] > cb)
                                                                    goto is_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                    else
                                                        if (p[offsets[11]] > cb)
                                                            if (p[offsets[12]] > cb)
                                                                if (p[offsets[13]] > cb)
                                                                    goto is_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                else
                                                    if (p[offsets[11]] > cb)
                                                        if (p[offsets[12]] > cb)
                                                            if (p[offsets[13]] > cb)
                                                                if (p[offsets[14]] > cb)
                                                                    goto is_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                            else
                                                if (p[offsets[11]] > cb)
                                                    if (p[offsets[12]] > cb)
                                                        if (p[offsets[13]] > cb)
                                                            if (p[offsets[14]] > cb)
                                                                if (p[offsets[15]] > cb)
                                                                    goto is_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                        else
                                            goto is_not_a_corner;
                                    else
                                        goto is_not_a_corner;
                                else
                                    goto is_not_a_corner;
                            else if (p[offsets[9]] < c_b)
                                if (p[offsets[10]] < c_b)
                                    if (p[offsets[11]] < c_b)
                                        if (p[offsets[12]] < c_b)
                                            if (p[offsets[13]] < c_b)
                                                if (p[offsets[14]] < c_b)
                                                    if (p[offsets[15]] < c_b)
                                                        goto is_a_corner;
                                                    else
                                                        if (p[offsets[6]] < c_b)
                                                            if (p[offsets[7]] < c_b)
                                                                if (p[offsets[8]] < c_b)
                                                                    goto is_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                else
                                                    if (p[offsets[5]] < c_b)
                                                        if (p[offsets[6]] < c_b)
                                                            if (p[offsets[7]] < c_b)
                                                                if (p[offsets[8]] < c_b)
                                                                    goto is_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                            else
                                                if (p[offsets[4]] < c_b)
                                                    if (p[offsets[5]] < c_b)
                                                        if (p[offsets[6]] < c_b)
                                                            if (p[offsets[7]] < c_b)
                                                                if (p[offsets[8]] < c_b)
                                                                    goto is_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                        else
                                            if (p[offsets[3]] < c_b)
                                                if (p[offsets[4]] < c_b)
                                                    if (p[offsets[5]] < c_b)
                                                        if (p[offsets[6]] < c_b)
                                                            if (p[offsets[7]] < c_b)
                                                                if (p[offsets[8]] < c_b)
                                                                    goto is_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                    else
                                        goto is_not_a_corner;
                                else
                                    goto is_not_a_corner;
                            else
                                goto is_not_a_corner;
                        else if (p[offsets[2]] < c_b)
                            if (p[offsets[3]] > cb)
                                if (p[offsets[10]] > cb)
                                    if (p[offsets[7]] > cb)
                                        if (p[offsets[8]] > cb)
                                            if (p[offsets[9]] > cb)
                                                if (p[offsets[11]] > cb)
                                                    if (p[offsets[6]] > cb)
                                                        if (p[offsets[5]] > cb)
                                                            if (p[offsets[4]] > cb)
                                                                goto is_a_corner;
                                                            else
                                                                if (p[offsets[12]] > cb)
                                                                    if (p[offsets[13]] > cb)
                                                                        goto is_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                        else
                                                            if (p[offsets[12]] > cb)
                                                                if (p[offsets[13]] > cb)
                                                                    if (p[offsets[14]] > cb)
                                                                        goto is_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                    else
                                                        if (p[offsets[12]] > cb)
                                                            if (p[offsets[13]] > cb)
                                                                if (p[offsets[14]] > cb)
                                                                    if (p[offsets[15]] > cb)
                                                                        goto is_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                        else
                                            goto is_not_a_corner;
                                    else
                                        goto is_not_a_corner;
                                else if (p[offsets[10]] < c_b)
                                    if (p[offsets[11]] < c_b)
                                        if (p[offsets[12]] < c_b)
                                            if (p[offsets[13]] < c_b)
                                                if (p[offsets[14]] < c_b)
                                                    if (p[offsets[15]] < c_b)
                                                        goto is_a_corner;
                                                    else
                                                        if (p[offsets[6]] < c_b)
                                                            if (p[offsets[7]] < c_b)
                                                                if (p[offsets[8]] < c_b)
                                                                    if (p[offsets[9]] < c_b)
                                                                        goto is_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                else
                                                    if (p[offsets[5]] < c_b)
                                                        if (p[offsets[6]] < c_b)
                                                            if (p[offsets[7]] < c_b)
                                                                if (p[offsets[8]] < c_b)
                                                                    if (p[offsets[9]] < c_b)
                                                                        goto is_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                            else
                                                if (p[offsets[4]] < c_b)
                                                    if (p[offsets[5]] < c_b)
                                                        if (p[offsets[6]] < c_b)
                                                            if (p[offsets[7]] < c_b)
                                                                if (p[offsets[8]] < c_b)
                                                                    if (p[offsets[9]] < c_b)
                                                                        goto is_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                        else
                                            goto is_not_a_corner;
                                    else
                                        goto is_not_a_corner;
                                else
                                    goto is_not_a_corner;
                            else if (p[offsets[3]] < c_b)
                                if (p[offsets[4]] > cb)
                                    if (p[offsets[13]] > cb)
                                        if (p[offsets[7]] > cb)
                                            if (p[offsets[8]] > cb)
                                                if (p[offsets[9]] > cb)
                                                    if (p[offsets[10]] > cb)
                                                        if (p[offsets[11]] > cb)
                                                            if (p[offsets[12]] > cb)
                                                                if (p[offsets[6]] > cb)
                                                                    if (p[offsets[5]] > cb)
                                                                        goto is_a_corner;
                                                                    else
                                                                        if (p[offsets[14]] > cb)
                                                                            goto is_a_corner;
                                                                        else
                                                                            goto is_not_a_corner;
                                                                else
                                                                    if (p[offsets[14]] > cb)
                                                                        if (p[offsets[15]] > cb)
                                                                            goto is_a_corner;
                                                                        else
                                                                            goto is_not_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                        else
                                            goto is_not_a_corner;
                                    else if (p[offsets[13]] < c_b)
                                        if (p[offsets[11]] > cb)
                                            if (p[offsets[5]] > cb)
                                                if (p[offsets[6]] > cb)
                                                    if (p[offsets[7]] > cb)
                                                        if (p[offsets[8]] > cb)
                                                            if (p[offsets[9]] > cb)
                                                                if (p[offsets[10]] > cb)
                                                                    if (p[offsets[12]] > cb)
                                                                        goto is_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                        else if (p[offsets[11]] < c_b)
                                            if (p[offsets[12]] < c_b)
                                                if (p[offsets[14]] < c_b)
                                                    if (p[offsets[15]] < c_b)
                                                        goto is_a_corner;
                                                    else
                                                        if (p[offsets[6]] < c_b)
                                                            if (p[offsets[7]] < c_b)
                                                                if (p[offsets[8]] < c_b)
                                                                    if (p[offsets[9]] < c_b)
                                                                        if (p[offsets[10]] < c_b)
                                                                            goto is_a_corner;
                                                                        else
                                                                            goto is_not_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                else
                                                    if (p[offsets[5]] < c_b)
                                                        if (p[offsets[6]] < c_b)
                                                            if (p[offsets[7]] < c_b)
                                                                if (p[offsets[8]] < c_b)
                                                                    if (p[offsets[9]] < c_b)
                                                                        if (p[offsets[10]] < c_b)
                                                                            goto is_a_corner;
                                                                        else
                                                                            goto is_not_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                        else
                                            goto is_not_a_corner;
                                    else
                                        if (p[offsets[5]] > cb)
                                            if (p[offsets[6]] > cb)
                                                if (p[offsets[7]] > cb)
                                                    if (p[offsets[8]] > cb)
                                                        if (p[offsets[9]] > cb)
                                                            if (p[offsets[10]] > cb)
                                                                if (p[offsets[11]] > cb)
                                                                    if (p[offsets[12]] > cb)
                                                                        goto is_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                        else
                                            goto is_not_a_corner;
                                else if (p[offsets[4]] < c_b)
                                    if (p[offsets[5]] > cb)
                                        if (p[offsets[14]] > cb)
                                            if (p[offsets[7]] > cb)
                                                if (p[offsets[8]] > cb)
                                                    if (p[offsets[9]] > cb)
                                                        if (p[offsets[10]] > cb)
                                                            if (p[offsets[11]] > cb)
                                                                if (p[offsets[12]] > cb)
                                                                    if (p[offsets[13]] > cb)
                                                                        if (p[offsets[6]] > cb)
                                                                            goto is_a_corner;
                                                                        else
                                                                            if (p[offsets[15]] > cb)
                                                                                goto is_a_corner;
                                                                            else
                                                                                goto is_not_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                        else if (p[offsets[14]] < c_b)
                                            if (p[offsets[12]] > cb)
                                                if (p[offsets[6]] > cb)
                                                    if (p[offsets[7]] > cb)
                                                        if (p[offsets[8]] > cb)
                                                            if (p[offsets[9]] > cb)
                                                                if (p[offsets[10]] > cb)
                                                                    if (p[offsets[11]] > cb)
                                                                        if (p[offsets[13]] > cb)
                                                                            goto is_a_corner;
                                                                        else
                                                                            goto is_not_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else if (p[offsets[12]] < c_b)
                                                if (p[offsets[13]] < c_b)
                                                    if (p[offsets[15]] < c_b)
                                                        goto is_a_corner;
                                                    else
                                                        if (p[offsets[6]] < c_b)
                                                            if (p[offsets[7]] < c_b)
                                                                if (p[offsets[8]] < c_b)
                                                                    if (p[offsets[9]] < c_b)
                                                                        if (p[offsets[10]] < c_b)
                                                                            if (p[offsets[11]] < c_b)
                                                                                goto is_a_corner;
                                                                            else
                                                                                goto is_not_a_corner;
                                                                        else
                                                                            goto is_not_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                        else
                                            if (p[offsets[6]] > cb)
                                                if (p[offsets[7]] > cb)
                                                    if (p[offsets[8]] > cb)
                                                        if (p[offsets[9]] > cb)
                                                            if (p[offsets[10]] > cb)
                                                                if (p[offsets[11]] > cb)
                                                                    if (p[offsets[12]] > cb)
                                                                        if (p[offsets[13]] > cb)
                                                                            goto is_a_corner;
                                                                        else
                                                                            goto is_not_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                    else if (p[offsets[5]] < c_b)
                                        if (p[offsets[6]] > cb)
                                            if (p[offsets[15]] < c_b)
                                                if (p[offsets[13]] > cb)
                                                    if (p[offsets[7]] > cb)
                                                        if (p[offsets[8]] > cb)
                                                            if (p[offsets[9]] > cb)
                                                                if (p[offsets[10]] > cb)
                                                                    if (p[offsets[11]] > cb)
                                                                        if (p[offsets[12]] > cb)
                                                                            if (p[offsets[14]] > cb)
                                                                                goto is_a_corner;
                                                                            else
                                                                                goto is_not_a_corner;
                                                                        else
                                                                            goto is_not_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else if (p[offsets[13]] < c_b)
                                                    if (p[offsets[14]] < c_b)
                                                        goto is_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                if (p[offsets[7]] > cb)
                                                    if (p[offsets[8]] > cb)
                                                        if (p[offsets[9]] > cb)
                                                            if (p[offsets[10]] > cb)
                                                                if (p[offsets[11]] > cb)
                                                                    if (p[offsets[12]] > cb)
                                                                        if (p[offsets[13]] > cb)
                                                                            if (p[offsets[14]] > cb)
                                                                                goto is_a_corner;
                                                                            else
                                                                                goto is_not_a_corner;
                                                                        else
                                                                            goto is_not_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                        else if (p[offsets[6]] < c_b)
                                            if (p[offsets[7]] > cb)
                                                if (p[offsets[14]] > cb)
                                                    if (p[offsets[8]] > cb)
                                                        if (p[offsets[9]] > cb)
                                                            if (p[offsets[10]] > cb)
                                                                if (p[offsets[11]] > cb)
                                                                    if (p[offsets[12]] > cb)
                                                                        if (p[offsets[13]] > cb)
                                                                            if (p[offsets[15]] > cb)
                                                                                goto is_a_corner;
                                                                            else
                                                                                goto is_not_a_corner;
                                                                        else
                                                                            goto is_not_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else if (p[offsets[14]] < c_b)
                                                    if (p[offsets[15]] < c_b)
                                                        goto is_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else if (p[offsets[7]] < c_b)
                                                if (p[offsets[8]] < c_b)
                                                    goto is_a_corner;
                                                else
                                                    if (p[offsets[15]] < c_b)
                                                        goto is_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                            else
                                                if (p[offsets[14]] < c_b)
                                                    if (p[offsets[15]] < c_b)
                                                        goto is_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                        else
                                            if (p[offsets[13]] > cb)
                                                if (p[offsets[7]] > cb)
                                                    if (p[offsets[8]] > cb)
                                                        if (p[offsets[9]] > cb)
                                                            if (p[offsets[10]] > cb)
                                                                if (p[offsets[11]] > cb)
                                                                    if (p[offsets[12]] > cb)
                                                                        if (p[offsets[14]] > cb)
                                                                            if (p[offsets[15]] > cb)
                                                                                goto is_a_corner;
                                                                            else
                                                                                goto is_not_a_corner;
                                                                        else
                                                                            goto is_not_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else if (p[offsets[13]] < c_b)
                                                if (p[offsets[14]] < c_b)
                                                    if (p[offsets[15]] < c_b)
                                                        goto is_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                    else
                                        if (p[offsets[12]] > cb)
                                            if (p[offsets[7]] > cb)
                                                if (p[offsets[8]] > cb)
                                                    if (p[offsets[9]] > cb)
                                                        if (p[offsets[10]] > cb)
                                                            if (p[offsets[11]] > cb)
                                                                if (p[offsets[13]] > cb)
                                                                    if (p[offsets[14]] > cb)
                                                                        if (p[offsets[6]] > cb)
                                                                            goto is_a_corner;
                                                                        else
                                                                            if (p[offsets[15]] > cb)
                                                                                goto is_a_corner;
                                                                            else
                                                                                goto is_not_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                        else if (p[offsets[12]] < c_b)
                                            if (p[offsets[13]] < c_b)
                                                if (p[offsets[14]] < c_b)
                                                    if (p[offsets[15]] < c_b)
                                                        goto is_a_corner;
                                                    else
                                                        if (p[offsets[6]] < c_b)
                                                            if (p[offsets[7]] < c_b)
                                                                if (p[offsets[8]] < c_b)
                                                                    if (p[offsets[9]] < c_b)
                                                                        if (p[offsets[10]] < c_b)
                                                                            if (p[offsets[11]] < c_b)
                                                                                goto is_a_corner;
                                                                            else
                                                                                goto is_not_a_corner;
                                                                        else
                                                                            goto is_not_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                        else
                                            goto is_not_a_corner;
                                else
                                    if (p[offsets[11]] > cb)
                                        if (p[offsets[7]] > cb)
                                            if (p[offsets[8]] > cb)
                                                if (p[offsets[9]] > cb)
                                                    if (p[offsets[10]] > cb)
                                                        if (p[offsets[12]] > cb)
                                                            if (p[offsets[13]] > cb)
                                                                if (p[offsets[6]] > cb)
                                                                    if (p[offsets[5]] > cb)
                                                                        goto is_a_corner;
                                                                    else
                                                                        if (p[offsets[14]] > cb)
                                                                            goto is_a_corner;
                                                                        else
                                                                            goto is_not_a_corner;
                                                                else
                                                                    if (p[offsets[14]] > cb)
                                                                        if (p[offsets[15]] > cb)
                                                                            goto is_a_corner;
                                                                        else
                                                                            goto is_not_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                        else
                                            goto is_not_a_corner;
                                    else if (p[offsets[11]] < c_b)
                                        if (p[offsets[12]] < c_b)
                                            if (p[offsets[13]] < c_b)
                                                if (p[offsets[14]] < c_b)
                                                    if (p[offsets[15]] < c_b)
                                                        goto is_a_corner;
                                                    else
                                                        if (p[offsets[6]] < c_b)
                                                            if (p[offsets[7]] < c_b)
                                                                if (p[offsets[8]] < c_b)
                                                                    if (p[offsets[9]] < c_b)
                                                                        if (p[offsets[10]] < c_b)
                                                                            goto is_a_corner;
                                                                        else
                                                                            goto is_not_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                else
                                                    if (p[offsets[5]] < c_b)
                                                        if (p[offsets[6]] < c_b)
                                                            if (p[offsets[7]] < c_b)
                                                                if (p[offsets[8]] < c_b)
                                                                    if (p[offsets[9]] < c_b)
                                                                        if (p[offsets[10]] < c_b)
                                                                            goto is_a_corner;
                                                                        else
                                                                            goto is_not_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                        else
                                            goto is_not_a_corner;
                                    else
                                        goto is_not_a_corner;
                            else
                                if (p[offsets[10]] > cb)
                                    if (p[offsets[7]] > cb)
                                        if (p[offsets[8]] > cb)
                                            if (p[offsets[9]] > cb)
                                                if (p[offsets[11]] > cb)
                                                    if (p[offsets[12]] > cb)
                                                        if (p[offsets[6]] > cb)
                                                            if (p[offsets[5]] > cb)
                                                                if (p[offsets[4]] > cb)
                                                                    goto is_a_corner;
                                                                else
                                                                    if (p[offsets[13]] > cb)
                                                                        goto is_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                            else
                                                                if (p[offsets[13]] > cb)
                                                                    if (p[offsets[14]] > cb)
                                                                        goto is_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                        else
                                                            if (p[offsets[13]] > cb)
                                                                if (p[offsets[14]] > cb)
                                                                    if (p[offsets[15]] > cb)
                                                                        goto is_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                        else
                                            goto is_not_a_corner;
                                    else
                                        goto is_not_a_corner;
                                else if (p[offsets[10]] < c_b)
                                    if (p[offsets[11]] < c_b)
                                        if (p[offsets[12]] < c_b)
                                            if (p[offsets[13]] < c_b)
                                                if (p[offsets[14]] < c_b)
                                                    if (p[offsets[15]] < c_b)
                                                        goto is_a_corner;
                                                    else
                                                        if (p[offsets[6]] < c_b)
                                                            if (p[offsets[7]] < c_b)
                                                                if (p[offsets[8]] < c_b)
                                                                    if (p[offsets[9]] < c_b)
                                                                        goto is_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                else
                                                    if (p[offsets[5]] < c_b)
                                                        if (p[offsets[6]] < c_b)
                                                            if (p[offsets[7]] < c_b)
                                                                if (p[offsets[8]] < c_b)
                                                                    if (p[offsets[9]] < c_b)
                                                                        goto is_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                            else
                                                if (p[offsets[4]] < c_b)
                                                    if (p[offsets[5]] < c_b)
                                                        if (p[offsets[6]] < c_b)
                                                            if (p[offsets[7]] < c_b)
                                                                if (p[offsets[8]] < c_b)
                                                                    if (p[offsets[9]] < c_b)
                                                                        goto is_a_corner;
                                                                    else
                                                                        goto is_not_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                        else
                                            goto is_not_a_corner;
                                    else
                                        goto is_not_a_corner;
                                else
                                    goto is_not_a_corner;
                        else
                            if (p[offsets[9]] > cb)
                                if (p[offsets[7]] > cb)
                                    if (p[offsets[8]] > cb)
                                        if (p[offsets[10]] > cb)
                                            if (p[offsets[11]] > cb)
                                                if (p[offsets[6]] > cb)
                                                    if (p[offsets[5]] > cb)
                                                        if (p[offsets[4]] > cb)
                                                            if (p[offsets[3]] > cb)
                                                                goto is_a_corner;
                                                            else
                                                                if (p[offsets[12]] > cb)
                                                                    goto is_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                        else
                                                            if (p[offsets[12]] > cb)
                                                                if (p[offsets[13]] > cb)
                                                                    goto is_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                    else
                                                        if (p[offsets[12]] > cb)
                                                            if (p[offsets[13]] > cb)
                                                                if (p[offsets[14]] > cb)
                                                                    goto is_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                else
                                                    if (p[offsets[12]] > cb)
                                                        if (p[offsets[13]] > cb)
                                                            if (p[offsets[14]] > cb)
                                                                if (p[offsets[15]] > cb)
                                                                    goto is_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                        else
                                            goto is_not_a_corner;
                                    else
                                        goto is_not_a_corner;
                                else
                                    goto is_not_a_corner;
                            else if (p[offsets[9]] < c_b)
                                if (p[offsets[10]] < c_b)
                                    if (p[offsets[11]] < c_b)
                                        if (p[offsets[12]] < c_b)
                                            if (p[offsets[13]] < c_b)
                                                if (p[offsets[14]] < c_b)
                                                    if (p[offsets[15]] < c_b)
                                                        goto is_a_corner;
                                                    else
                                                        if (p[offsets[6]] < c_b)
                                                            if (p[offsets[7]] < c_b)
                                                                if (p[offsets[8]] < c_b)
                                                                    goto is_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                else
                                                    if (p[offsets[5]] < c_b)
                                                        if (p[offsets[6]] < c_b)
                                                            if (p[offsets[7]] < c_b)
                                                                if (p[offsets[8]] < c_b)
                                                                    goto is_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                            else
                                                if (p[offsets[4]] < c_b)
                                                    if (p[offsets[5]] < c_b)
                                                        if (p[offsets[6]] < c_b)
                                                            if (p[offsets[7]] < c_b)
                                                                if (p[offsets[8]] < c_b)
                                                                    goto is_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                        else
                                            if (p[offsets[3]] < c_b)
                                                if (p[offsets[4]] < c_b)
                                                    if (p[offsets[5]] < c_b)
                                                        if (p[offsets[6]] < c_b)
                                                            if (p[offsets[7]] < c_b)
                                                                if (p[offsets[8]] < c_b)
                                                                    goto is_a_corner;
                                                                else
                                                                    goto is_not_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                    else
                                        goto is_not_a_corner;
                                else
                                    goto is_not_a_corner;
                            else
                                goto is_not_a_corner;
                    else
                        if (p[offsets[8]] > cb)
                            if (p[offsets[7]] > cb)
                                if (p[offsets[9]] > cb)
                                    if (p[offsets[10]] > cb)
                                        if (p[offsets[6]] > cb)
                                            if (p[offsets[5]] > cb)
                                                if (p[offsets[4]] > cb)
                                                    if (p[offsets[3]] > cb)
                                                        if (p[offsets[2]] > cb)
                                                            goto is_a_corner;
                                                        else
                                                            if (p[offsets[11]] > cb)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                    else
                                                        if (p[offsets[11]] > cb)
                                                            if (p[offsets[12]] > cb)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                else
                                                    if (p[offsets[11]] > cb)
                                                        if (p[offsets[12]] > cb)
                                                            if (p[offsets[13]] > cb)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                            else
                                                if (p[offsets[11]] > cb)
                                                    if (p[offsets[12]] > cb)
                                                        if (p[offsets[13]] > cb)
                                                            if (p[offsets[14]] > cb)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                        else
                                            if (p[offsets[11]] > cb)
                                                if (p[offsets[12]] > cb)
                                                    if (p[offsets[13]] > cb)
                                                        if (p[offsets[14]] > cb)
                                                            if (p[offsets[15]] > cb)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                    else
                                        goto is_not_a_corner;
                                else
                                    goto is_not_a_corner;
                            else
                                goto is_not_a_corner;
                        else if (p[offsets[8]] < c_b)
                            if (p[offsets[9]] < c_b)
                                if (p[offsets[10]] < c_b)
                                    if (p[offsets[11]] < c_b)
                                        if (p[offsets[12]] < c_b)
                                            if (p[offsets[13]] < c_b)
                                                if (p[offsets[14]] < c_b)
                                                    if (p[offsets[15]] < c_b)
                                                        goto is_a_corner;
                                                    else
                                                        if (p[offsets[6]] < c_b)
                                                            if (p[offsets[7]] < c_b)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                else
                                                    if (p[offsets[5]] < c_b)
                                                        if (p[offsets[6]] < c_b)
                                                            if (p[offsets[7]] < c_b)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                            else
                                                if (p[offsets[4]] < c_b)
                                                    if (p[offsets[5]] < c_b)
                                                        if (p[offsets[6]] < c_b)
                                                            if (p[offsets[7]] < c_b)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                        else
                                            if (p[offsets[3]] < c_b)
                                                if (p[offsets[4]] < c_b)
                                                    if (p[offsets[5]] < c_b)
                                                        if (p[offsets[6]] < c_b)
                                                            if (p[offsets[7]] < c_b)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                    else
                                        if (p[offsets[2]] < c_b)
                                            if (p[offsets[3]] < c_b)
                                                if (p[offsets[4]] < c_b)
                                                    if (p[offsets[5]] < c_b)
                                                        if (p[offsets[6]] < c_b)
                                                            if (p[offsets[7]] < c_b)
                                                                goto is_a_corner;
                                                            else
                                                                goto is_not_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                        else
                                            goto is_not_a_corner;
                                else
                                    goto is_not_a_corner;
                            else
                                goto is_not_a_corner;
                        else
                            goto is_not_a_corner;
                else
                    if (p[offsets[7]] > cb)
                        if (p[offsets[8]] > cb)
                            if (p[offsets[9]] > cb)
                                if (p[offsets[6]] > cb)
                                    if (p[offsets[5]] > cb)
                                        if (p[offsets[4]] > cb)
                                            if (p[offsets[3]] > cb)
                                                if (p[offsets[2]] > cb)
                                                    if (p[offsets[1]] > cb)
                                                        goto is_a_corner;
                                                    else
                                                        if (p[offsets[10]] > cb)
                                                            goto is_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                else
                                                    if (p[offsets[10]] > cb)
                                                        if (p[offsets[11]] > cb)
                                                            goto is_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                            else
                                                if (p[offsets[10]] > cb)
                                                    if (p[offsets[11]] > cb)
                                                        if (p[offsets[12]] > cb)
                                                            goto is_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                        else
                                            if (p[offsets[10]] > cb)
                                                if (p[offsets[11]] > cb)
                                                    if (p[offsets[12]] > cb)
                                                        if (p[offsets[13]] > cb)
                                                            goto is_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                    else
                                        if (p[offsets[10]] > cb)
                                            if (p[offsets[11]] > cb)
                                                if (p[offsets[12]] > cb)
                                                    if (p[offsets[13]] > cb)
                                                        if (p[offsets[14]] > cb)
                                                            goto is_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                        else
                                            goto is_not_a_corner;
                                else
                                    if (p[offsets[10]] > cb)
                                        if (p[offsets[11]] > cb)
                                            if (p[offsets[12]] > cb)
                                                if (p[offsets[13]] > cb)
                                                    if (p[offsets[14]] > cb)
                                                        if (p[offsets[15]] > cb)
                                                            goto is_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                        else
                                            goto is_not_a_corner;
                                    else
                                        goto is_not_a_corner;
                            else
                                goto is_not_a_corner;
                        else
                            goto is_not_a_corner;
                    else if (p[offsets[7]] < c_b)
                        if (p[offsets[8]] < c_b)
                            if (p[offsets[9]] < c_b)
                                if (p[offsets[6]] < c_b)
                                    if (p[offsets[5]] < c_b)
                                        if (p[offsets[4]] < c_b)
                                            if (p[offsets[3]] < c_b)
                                                if (p[offsets[2]] < c_b)
                                                    if (p[offsets[1]] < c_b)
                                                        goto is_a_corner;
                                                    else
                                                        if (p[offsets[10]] < c_b)
                                                            goto is_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                else
                                                    if (p[offsets[10]] < c_b)
                                                        if (p[offsets[11]] < c_b)
                                                            goto is_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                            else
                                                if (p[offsets[10]] < c_b)
                                                    if (p[offsets[11]] < c_b)
                                                        if (p[offsets[12]] < c_b)
                                                            goto is_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                        else
                                            if (p[offsets[10]] < c_b)
                                                if (p[offsets[11]] < c_b)
                                                    if (p[offsets[12]] < c_b)
                                                        if (p[offsets[13]] < c_b)
                                                            goto is_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                    else
                                        if (p[offsets[10]] < c_b)
                                            if (p[offsets[11]] < c_b)
                                                if (p[offsets[12]] < c_b)
                                                    if (p[offsets[13]] < c_b)
                                                        if (p[offsets[14]] < c_b)
                                                            goto is_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                        else
                                            goto is_not_a_corner;
                                else
                                    if (p[offsets[10]] < c_b)
                                        if (p[offsets[11]] < c_b)
                                            if (p[offsets[12]] < c_b)
                                                if (p[offsets[13]] < c_b)
                                                    if (p[offsets[14]] < c_b)
                                                        if (p[offsets[15]] < c_b)
                                                            goto is_a_corner;
                                                        else
                                                            goto is_not_a_corner;
                                                    else
                                                        goto is_not_a_corner;
                                                else
                                                    goto is_not_a_corner;
                                            else
                                                goto is_not_a_corner;
                                        else
                                            goto is_not_a_corner;
                                    else
                                        goto is_not_a_corner;
                            else
                                goto is_not_a_corner;
                        else
                            goto is_not_a_corner;
                    else
                        goto is_not_a_corner;

                #endregion

            is_a_corner:
                bmin = b;
                goto end_if;

            is_not_a_corner:
                bmax = b;
                goto end_if;

            end_if:

                if (bmin == bmax - 1 || bmin == bmax)
                    return bmin;

                b = (bmin + bmax) / 2;
            }
        }

        private static int[] makeOffsets(int stride)
        {
            int[] pixel = new int[16];
            pixel[00] = +0 + stride * +3;
            pixel[01] = +1 + stride * +3;
            pixel[02] = +2 + stride * +2;
            pixel[03] = +3 + stride * +1;
            pixel[04] = +3 + stride * +0;
            pixel[05] = +3 + stride * -1;
            pixel[06] = +2 + stride * -2;
            pixel[07] = +1 + stride * -3;
            pixel[08] = +0 + stride * -3;
            pixel[09] = -1 + stride * -3;
            pixel[10] = -2 + stride * -2;
            pixel[11] = -3 + stride * -1;
            pixel[12] = -3 + stride * +0;
            pixel[13] = -3 + stride * +1;
            pixel[14] = -2 + stride * +2;
            pixel[15] = -1 + stride * +3;
            return pixel;
        }
        #endregion

    }
}
