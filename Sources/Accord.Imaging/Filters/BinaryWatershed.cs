// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Diego Catalano, 2017
// diego.catalano at live.com
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

namespace Accord.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing.Imaging;
    using Accord.Imaging;
    using Accord.Imaging.Filters;
    using Accord.Math;
    using Accord.Diagnostics;
    using Accord.Imaging.Converters;

    /// <summary>
    ///   Watershed filter.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In the study of image processing, a watershed is a transformation defined on a grayscale image. 
    ///   The name refers metaphorically to a geological watershed, or drainage divide, which separates 
    ///   adjacent drainage basins. The watershed transformation treats the image it operates upon like a
    ///   topographic map, with the brightness of each point representing its height, and finds the lines 
    ///   that run along the tops of ridges.</para>
    /// <para>
    ///   There are different technical definitions of a watershed. In graphs, watershed lines may be 
    ///   defined on the nodes, on the edges, or hybrid lines on both nodes and edges. Watersheds may
    ///   also be defined in the continuous domain.[1] There are also many different algorithms to compute 
    ///   watersheds. Watershed algorithm is used in image processing primarily for segmentation purposes.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="https://en.wikipedia.org/wiki/Watershed_(image_processing)">
    ///       Wikipedia contributors. "Watershed (image processing)." Wikipedia, The Free Encyclopedia. 
    ///       Available on: https://en.wikipedia.org/wiki/Watershed_(image_processing) </a>
    ///       </description></item>
    ///    </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    ///   Bitmap input = ... 
    /// 
    ///   // Apply the transform
    ///   var dt = new BinaryWatershed();
    ///   Bitmap output = dt.Apply(input);
    /// 
    ///   // Show results on screen
    ///   ImageBox.Show("input", input);
    ///   ImageBox.Show("output", output);
    ///   
    ///   // Mark points using PointsMarker
    ///   var marker = new PointsMarker(Color.Red, 5)
    ///   {
    ///       Points = bw.MaxPoints
    ///   };
    ///   
    ///   Bitmap marked = marker.Apply(result);
    ///   ImageBox.Show("markers", marked);
    /// </code>
    /// </example>
    /// 
    public class BinaryWatershed : BaseInPlaceFilter
    {
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;

        private int[] DIR_X_OFFSET = new int[] { 0, 1, 1, 1, 0, -1, -1, -1 };
        private int[] DIR_Y_OFFSET = new int[] { -1, -1, 0, 1, 1, 1, 0, -1 };
        private int[] dirOffset;
        private const float SQRT2 = 1.4142135624f;
        private int intEncodeXMask; // needed for encoding x & y in a single int (watershed): mask for x
        private int intEncodeYMask; // needed for encoding x & y in a single int (watershed): mask for y
        private int intEncodeShift; // needed for encoding x & y in a single int (watershed): shift of y
        private List<IntPoint> maxPoints;

        private DistanceTransformMethod distance = DistanceTransformMethod.Euclidean;
        private float tolerance = 0.5f;

        /// <summary>
        ///   Gets the list of maximum points found in the image.
        /// </summary>
        /// 
        public List<IntPoint> MaxPoints
        {
            get { return maxPoints; }
        }

        /// <summary>
        ///   Gets or sets the tolerance. Default is 0.5f.
        /// </summary>
        /// 
        public float Tolerance
        {
            get { return tolerance; }
            set { tolerance = value; }
        }

        /// <summary>
        ///   Gets or sets the distance method to be used in the 
        ///   underlying <see cref="DistanceTransform"/>.
        /// </summary>
        /// 
        public DistanceTransformMethod Distance
        {
            get { return distance; }
            set { distance = value; }
        }

        /// <summary>
        ///   Format translations dictionary.
        /// </summary>
        /// 
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryWatershed"/> class.
        /// </summary>
        /// 
        public BinaryWatershed()
        {
            formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryWatershed"/> class.
        /// </summary>
        /// 
        /// <param name="tolerance">The tolerance. Default is 0.5f.</param>
        /// 
        public BinaryWatershed(float tolerance)
            : this()
        {
            this.tolerance = tolerance;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryWatershed"/> class.
        /// </summary>
        /// 
        /// <param name="tolerance">The tolerance. Default is 0.5f.</param>
        /// <param name="distance">The distance method.</param>
        /// 
        public BinaryWatershed(float tolerance, DistanceTransformMethod distance)
            : this(tolerance)
        {
            this.distance = distance;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryWatershed"/> class.
        /// </summary>
        /// 
        /// <param name="distance">The distance method.</param>
        /// 
        public BinaryWatershed(DistanceTransformMethod distance)
            : this()
        {
            this.distance = distance;
        }


        /// <summary>
        /// Processes the filter.
        /// </summary>
        /// <param name="image">The image.</param>
        protected override void ProcessFilter(UnmanagedImage image)
        {
            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
                throw new UnsupportedImageFormatException("Binary Watershed only works in grayscale (binary) images");

            int width = image.Width;
            int height = image.Height;

            DistanceTransform dt = new DistanceTransform(this.distance);
            UnmanagedImage d = dt.Apply(image);

            // Convert 2D to 1D - ImageJ Compatibility
            float[] distance1D = dt.Pixels;
            float[][] distance = Jagged.Reshape(distance1D, height, width);

            Debug.Assert(distance1D.Length == d.Size);
            Debug.Assert(distance.GetTotalLength() == d.Size);

            // Make directions offsets
            makeDirectionOffsets(width);

            byte[] back = new byte[width * height];

            // Find the maxima
            long[] maxPoints = getSortedMaxPoints(distance, distance1D, back, 0, dt.MaximumDistance, -808080.0);

            // Mark the maxima locations
            float maxSortingError = 1.1f * SQRT2 / 2f;
            this.maxPoints = analyseAndMarkMaxima(distance1D, back, maxPoints, tolerance, maxSortingError, width, height);

            // Transform em 8bit 0..255
            byte[] outImage = make8Bit(distance, back, dt.MaximumDistance, -808080.0, width, height);

            cleanupMaxima(outImage, back, maxPoints, width, height);
            watershedSegment(outImage, width, height);
            watershedPostProcess(outImage);

            unsafe
            {
                int offset = image.Offset;
                byte* dst = (byte*)image.ImageData;
                for (int y = 0, k = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++, dst++, k++)
                        *dst = outImage[k];
                    dst += offset;
                }
            }
        }

        private void makeDirectionOffsets(int width)
        {
            int shift = 0, mult = 1;
            do
            {
                shift++; mult *= 2;
            }
            while (mult < width);

            intEncodeXMask = mult - 1;
            intEncodeYMask = ~intEncodeXMask;
            intEncodeShift = shift;

            dirOffset = new int[] { -width, -width + 1, +1, +width + 1, +width, +width - 1, -1, -width - 1 };

            // dirOffset is created last, so check for it being null before makeDirectionOffsets
            // (in case we have multiple threads using the same MaximumFinder)
        }

        private long[] getSortedMaxPoints(float[][] distance, float[] distance1D, byte[] types, float globalMin, float globalMax, double threshold)
        {
            int nMax = 0;
            for (int y = 0; y < distance.Length; y++)
            {
                for (int x = 0, i = x + y * distance[0].Length; x < distance[0].Length; x++, i++)
                {
                    float v = distance[y][x];

                    float vTrue = trueEdmHeight(x, y, distance1D, distance[0].Length, distance.Length);

                    if (!(v == globalMin))
                    {
                        if (!(x == 0 || x == distance[0].Length - 1 || y == 0 || y == distance.Length - 1))
                        {
                            if (!(v < threshold))
                            {
                                bool isMax = true;

                                // check wheter we have a local maximum.
                                // Note: For an EDM, we need all maxima: those of the EDM-corrected values
                                // (needed by findMaxima) and those of the raw values (needed by cleanupMaxima) 

                                bool isInner = (y != 0 && y != distance.Length - 1) && (x != 0 && x != distance[0].Length - 1); // not necessary, but faster than isWithin

                                for (int d = 0; d < 8; d++)
                                {
                                    // compare with the 8 neighbor pixels
                                    if (isInner || isWithin(x, y, d, distance[0].Length, distance.Length))
                                    {
                                        float vNeighbor = distance[y + DIR_Y_OFFSET[d]][x + DIR_X_OFFSET[d]];
                                        float vNeighborTrue = trueEdmHeight(x + DIR_X_OFFSET[d], y + DIR_Y_OFFSET[d], distance1D, distance[0].Length, distance.Length);

                                        if (vNeighbor > v && vNeighborTrue > vTrue)
                                        {
                                            isMax = false;
                                            break;
                                        }
                                    }
                                }

                                if (isMax)
                                {
                                    types[i] = (byte)1;
                                    nMax++;
                                }
                            }
                        }
                    }
                }
            }

            // for converting float values into a 32-bit int
            double vFactor = (2e9 / (globalMax - globalMin));

            // value (int) is in the upper 32 bit, pixel offset in the lower
            long[] maxPoints = new long[nMax];

            int iMax = 0;

            for (int y = 0; y < distance.Length; y++)
            {
                // enter all maxima into an array
                for (int x = 0, pp = x + y * distance[0].Length; x < distance[0].Length; x++, pp++)
                {
                    if (types[pp] == (byte)1)
                    {
                        double fValue = trueEdmHeight(x, y, distance1D, distance[0].Length, distance.Length);
                        int iValue = (int)((fValue - globalMin) * vFactor); // 32-bit int, linear function of float value
                        maxPoints[iMax++] = (long)((long)iValue << 32 | (uint)pp);
                    }
                }
            }

            // sort the maxima by value
            Array.Sort(maxPoints);

            return maxPoints;

        }

        private List<IntPoint> analyseAndMarkMaxima(float[] edmPixels, byte[] types, long[] maxPoints, float tolerance, float maxSortingError, int width, int height)
        {
            int nMax = maxPoints.Length;
            int[] pList = new int[width * height]; // here we enter points starting from a maximum

            var xyVector = new List<IntPoint>();

            // process all maxima now, starting from the highest
            for (int iMax = nMax - 1; iMax >= 0; iMax--)
            {
                // type cast gets 32 lower bits, where pixel index is encoded
                int offset0 = unchecked((int)maxPoints[iMax]);

                if ((types[offset0] & (byte)4) != 0) // this maximum has been reached from another one, skip it
                    continue;

                // we create a list of connected points and start the list at the current maximum
                int x0 = offset0 % width;
                int y0 = offset0 / width;
                float v0 = trueEdmHeight(x0, y0, edmPixels, width, height);
                bool sortingError;
                do
                {
                    // repeat if we have encountered a sortingError
                    pList[0] = offset0;
                    types[offset0] |= ((byte)16 | (byte)2);   // mark first point as equal height (to itself) and listed
                    int listLen = 1;                          // number of elements in the list
                    int listI = 0;                            // index of current element in the list
                    sortingError = false;                     //  if sorting was inaccurate: a higher maximum was not handled so far
                    bool maxPossible = true;                  // it may be a true maximum
                    double xEqual = x0;                       // for creating a single point: determine average over the
                    double yEqual = y0;                       //   coordinates of contiguous equal-height points
                    int nEqual = 1;                           // counts xEqual/yEqual points that we use for averaging

                    do // while neighbor list is not fully processed (to listLen)
                    {
                        int offset = pList[listI];
                        int x = offset % width;
                        int y = offset / width;

                        bool isInner = (y != 0 && y != height - 1) && (x != 0 && x != width - 1); // not necessary, but faster than isWithin

                        for (int d = 0; d < 8; d++)
                        {
                            // analyze all neighbors (in 8 directions) at the same level
                            int offset2 = offset + dirOffset[d];
                            if ((isInner || isWithin(x, y, d, width, height)) && (types[offset2] & (byte)2) == 0)
                            {
                                if (edmPixels[offset2] <= 0)
                                    continue; // ignore the background (non-particles)

                                if ((types[offset2] & (byte)4) != 0)
                                {
                                    maxPossible = false; // we have reached a point processed previously, thus it is no maximum now

                                    break;
                                }

                                int x2 = x + DIR_X_OFFSET[d];
                                int y2 = y + DIR_Y_OFFSET[d];
                                float v2 = trueEdmHeight(x2, y2, edmPixels, width, height);

                                if (v2 > v0 + maxSortingError)
                                {
                                    maxPossible = false; // we have reached a higher point, thus it is no maximum
                                                         // if(x0<25&&y0<20)IJ.write("x0,y0="+x0+","+y0+":stop at higher neighbor from x,y="+x+","+y+", dir="+d+",value,value2,v2-v="+v0+","+v2+","+(v2-v0));
                                    break;
                                }
                                else if (v2 >= v0 - (float)tolerance)
                                {
                                    if (v2 > v0)
                                    {
                                        //  this point should have been treated earlier
                                        sortingError = true;
                                        offset0 = offset2;
                                        v0 = v2;
                                        x0 = x2;
                                        y0 = y2;

                                    }
                                    pList[listLen] = offset2;
                                    listLen++; // we have found a new point within the tolerance
                                    types[offset2] |= (byte)2;

                                    if (v2 == v0)
                                    {
                                        // prepare finding center of equal points (in case single point needed)
                                        types[offset2] |= (byte)16;
                                        xEqual += x2;
                                        yEqual += y2;
                                        nEqual++;
                                    }
                                }
                            } // if isWithin & not (byte)2
                        } // for directions d
                        listI++;

                    } while (listI < listLen);

                    if (sortingError)
                    {
                        // if x0,y0 was not the true maximum but we have reached a higher one
                        for (listI = 0; listI < listLen; listI++)
                            types[pList[listI]] = 0; // reset all points encountered, then retry
                    }
                    else
                    {
                        int resetMask = ~(maxPossible ? (byte)2 : ((byte)2 | (byte)16));
                        xEqual /= nEqual;
                        yEqual /= nEqual;
                        double minDist2 = 1e20;
                        int nearestI = 0;
                        for (listI = 0; listI < listLen; listI++)
                        {
                            int offset = pList[listI];
                            int x = offset % width;
                            int y = offset / width;
                            types[offset] = (byte)(types[offset] & resetMask); // reset attributes no longer needed
                            types[offset] |= (byte)4; // mark as processed

                            if (maxPossible)
                            {
                                types[offset] |= (byte)8;

                                if ((types[offset] & (byte)16) != 0)
                                {
                                    double dist2 = (xEqual - x) * (double)(xEqual - x) + (yEqual - y) * (double)(yEqual - y);

                                    if (dist2 < minDist2)
                                    {
                                        minDist2 = dist2; // this could be the best "single maximum" point
                                        nearestI = listI;
                                    }
                                }
                            }
                        } // for listI

                        if (maxPossible)
                        {
                            int offset = pList[nearestI];
                            types[offset] |= (byte)32;

                            int x = offset % width;
                            int y = offset / width;
                            xyVector.Add(new IntPoint(x, y));
                        }

                    } // if !sortingError
                } while (sortingError); // redo if we have encountered a higher maximum: handle it now.
            } // for all maxima iMax

            return xyVector;
        }

        private byte[] make8Bit(float[][] distance, byte[] types, float globalMax, double threshold, int width, int height)
        {
            threshold = 0.5;
            double minValue = 1;

            double offset = minValue - (globalMax - minValue) * (1.0 / 253 / 2 - 1e-6); // everything above minValue should become >(byte)0
            double factor = 253 / (globalMax - minValue);

            if (factor > 1)
                factor = 1;   // with EDM, no better resolution

            // convert possibly calibrated image to byte without damaging threshold (setMinAndMax would kill threshold)
            byte[] pixels = new byte[width * height];

            long v;
            for (int y = 0, i = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++, i++)
                {
                    float rawValue = distance[y][x]; // ip.getPixelValue(x, y);

                    if (rawValue < threshold)
                    {
                        pixels[i] = (byte)0;
                    }
                    else if ((types[i] & (byte)8) != 0)
                    {
                        pixels[i] = (byte)255;  // prepare watershed by setting "true" maxima+surroundings to 255
                    }
                    else
                    {
                        v = 1 + (long)Math.Round((rawValue - offset) * factor);
                        if (v < 1) pixels[i] = (byte)1;
                        else if (v <= 254) pixels[i] = (byte)(v & 255);
                        else pixels[i] = (byte)254;
                    }
                }
            }

            return pixels;
        }

        private void cleanupMaxima(byte[] pixels, byte[] types, long[] maxPoints, int width, int height)
        {
            int nMax = maxPoints.Length;
            int[] pList = new int[width * height];

            for (int iMax = nMax - 1; iMax >= 0; iMax--)
            {
                // type cast gets lower 32 bits where pixel offset is encoded
                int offset0 = unchecked((int)maxPoints[iMax]);

                if ((types[offset0] & ((byte)8 | (byte)64)) != 0)
                    continue;

                int level = pixels[offset0] & 255;
                int loLevel = level + 1;
                pList[0] = offset0;        // we start the list at the current maximum

                types[offset0] |= (byte)2; // mark first point as listed
                int listLen = 1;           // number of elements in the list
                int lastLen = 1;
                int listI = 0;             // index of current element in the list
                bool saddleFound = false;

                while (!saddleFound && loLevel > 0)
                {
                    loLevel--;
                    lastLen = listLen; // remember end of list for previous level
                    listI = 0;         // in each level, start analyzing the neighbors of all pixels

                    do // for all pixels listed so far
                    {
                        int offset = pList[listI];
                        int x = offset % width;
                        int y = offset / width;
                        bool isInner = (y != 0 && y != height - 1) && (x != 0 && x != width - 1); // not necessary, but faster than isWithin

                        for (int d = 0; d < 8; d++)
                        {
                            // analyze all neighbors (in 8 directions) at the same level
                            int offset2 = offset + dirOffset[d];
                            if ((isInner || isWithin(x, y, d, width, height)) && (types[offset2] & (byte)2) == 0)
                            {
                                if ((types[offset2] & (byte)8) != 0 || (((types[offset2] & (byte)64) != 0) && (pixels[offset2] & 255) >= loLevel))
                                {
                                    saddleFound = true; // we have reached a point touching a "true" maximum...
                                                        // if (xList[0]==122) IJ.write("saddle found at level="+loLevel+"; x,y="+xList[listI]+","+yList[listI]+", dir="+d);
                                    break;              // ...or a level not lower, but touching a "true" maximum
                                }
                                else if ((pixels[offset2] & 255) >= loLevel && (types[offset2] & (byte)64) == 0)
                                {
                                    pList[listLen] = offset2;
                                    //xList[listLen] = x+DIR_X_OFFSET[d];
                                    //yList[listLen] = x+DIR_Y_OFFSET[d];
                                    listLen++;          // we have found a new point to be processed
                                    types[offset2] |= (byte)2;
                                }
                            } // if isWithin & not (byte)2
                        } // for directions d

                        if (saddleFound)
                            break; // no reason to search any further

                        listI++;
                    } while (listI < listLen);
                } // while !levelFound && loLevel>=0

                for (listI = 0; listI < listLen; listI++) // reset attribute since we may come to this place again
                    types[pList[listI]] = (byte)(types[pList[listI]] & ~(byte)2);

                for (listI = 0; listI < lastLen; listI++)
                {
                    //for all points higher than the level of the saddle point
                    int offset = pList[listI];
                    pixels[offset] = (byte)loLevel; // set pixel value to the level of the saddle point
                    types[offset] |= (byte)64;      // mark as processed: there can't be a local maximum in this area
                }
            } // for all maxima iMax
        }

        private bool watershedSegment(byte[] pixels, int width, int height)
        {
            // Create an array with the coordinates of all points between value 1 and 254
            // This method, suggested by Stein Roervik (stein_at_kjemi-dot-unit-dot-no),
            // greatly speeds up the watershed segmentation routine.

            ImageStatistics @is = new ImageStatistics(UnmanagedImage.FromByteArray(pixels, width, height, PixelFormat.Format8bppIndexed));

            int[] histogram = @is.Gray.Values;

            int arraySize = width * height - histogram[0] - histogram[255];
            int[] coordinates = new int[arraySize];    //from pixel coordinates, low bits x, high bits y
            int highestValue = 0;
            int maxBinSize = 0;
            int offset = 0;
            int[] levelStart = new int[256];

            for (int v = 1; v < 255; v++)
            {
                levelStart[v] = offset;
                offset += histogram[v];
                if (histogram[v] > 0)
                    highestValue = v;
                if (histogram[v] > maxBinSize)
                    maxBinSize = histogram[v];
            }

            int[] levelOffset = new int[highestValue + 1];

            for (int y = 0, i = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++, i++)
                {
                    int v = pixels[i] & 255;

                    if (v > 0 && v < 255)
                    {
                        offset = levelStart[v] + levelOffset[v];
                        coordinates[offset] = x | y << intEncodeShift;
                        levelOffset[v]++;
                    }
                } // for x
            } // for y

            // Create an array of the points (pixel offsets) that we set to 255 in one pass.
            // If we remember this list we need not create a snapshot of the ImageProcessor. 
            int[] setPointList = new int[Math.Min(maxBinSize, (width * height + 2) / 3)];

            // now do the segmentation, starting at the highest level and working down.
            // At each level, dilate the particle (set pixels to 255), constrained to pixels
            // whose values are at that level and also constrained (by the fateTable)
            // to prevent features from merging.
            int[] table = makeFateTable();

            int[] directionSequence = new int[] { 7, 3, 1, 5, 0, 4, 2, 6 }; // diagonal directions first

            for (int level = highestValue; level >= 1; level--)
            {
                int remaining = histogram[level];  //number of points in the level that have not been processed
                int idle = 0;

                while (remaining > 0 && idle < 8)
                {
                    int dIndex = 0;
                    do
                    {
                        // expand each level in 8 directions
                        int n = processLevel(directionSequence[dIndex % 8], pixels, table,
                                levelStart[level], remaining, coordinates, setPointList, width, height);

                        //IJ.log("level="+level+" direction="+directionSequence[dIndex%8]+" remain="+remaining+"-"+n);

                        remaining -= n;  // number of points processed

                        if (n > 0)
                            idle = 0;    // nothing processed in this direction?

                        dIndex++;

                    } while (remaining > 0 && idle++ < 8);
                }

                // any pixels that we have not reached?
                if (remaining > 0 && level > 1)
                {
                    int nextLevel = level; // find the next level to process
                    do
                        nextLevel--;
                    while (nextLevel > 1 && histogram[nextLevel] == 0);

                    // in principle we should add all unprocessed pixels of this level to the
                    // tasklist of the next level. This would make it very slow for some images,
                    // however. Thus we only add the pixels if they are at the border (of the
                    // image or a thresholded area) and correct unprocessed pixels at the very
                    // end by CleanupExtraLines

                    if (nextLevel > 0)
                    {
                        int newNextLevelEnd = levelStart[nextLevel] + histogram[nextLevel];

                        for (int i = 0, p = levelStart[level]; i < remaining; i++, p++)
                        {
                            int xy = coordinates[p];
                            int x = xy & intEncodeXMask;
                            int y = (xy & intEncodeYMask) >> intEncodeShift;
                            int pOffset = x + y * width;
                            bool addToNext = false;

                            if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                            {
                                addToNext = true; // image border
                            }
                            else
                            {
                                for (int d = 0; d < 8; d++)
                                {
                                    if (isWithin(x, y, d, width, height) && pixels[pOffset + dirOffset[d]] == 0)
                                    {
                                        addToNext = true; // border of area below threshold
                                        break;
                                    }
                                }
                            }

                            if (addToNext)
                                coordinates[newNextLevelEnd++] = xy;
                        }
                        // tasklist for the next level to process becomes longer by this:
                        histogram[nextLevel] = newNextLevelEnd - levelStart[nextLevel];
                    }
                }
            }
            return true;
        }

        private int processLevel(int pass, byte[] pixels, int[] fateTable, int levelStart, int levelNPoints,
            int[] coordinates, int[] setPointList, int width, int height)
        {
            int xmax = width - 1;
            int ymax = height - 1;

            int nChanged = 0;
            int nUnchanged = 0;

            for (int i = 0, p = levelStart; i < levelNPoints; i++, p++)
            {
                int xy = coordinates[p];
                int x = xy & intEncodeXMask;
                int y = (xy & intEncodeYMask) >> intEncodeShift;
                int offset = x + y * width;
                int index = 0;

                // neighborhood pixel ocupation: index in fateTable
                if (y > 0 && (pixels[offset - width] & 255) == 255)
                    index ^= 1;
                if (x < xmax && y > 0 && (pixels[offset - width + 1] & 255) == 255)
                    index ^= 2;
                if (x < xmax && (pixels[offset + 1] & 255) == 255)
                    index ^= 4;
                if (x < xmax && y < ymax && (pixels[offset + width + 1] & 255) == 255)
                    index ^= 8;
                if (y < ymax && (pixels[offset + width] & 255) == 255)
                    index ^= 16;
                if (x > 0 && y < ymax && (pixels[offset + width - 1] & 255) == 255)
                    index ^= 32;
                if (x > 0 && (pixels[offset - 1] & 255) == 255)
                    index ^= 64;
                if (x > 0 && y > 0 && (pixels[offset - width - 1] & 255) == 255)
                    index ^= 128;
                int mask = 1 << pass;
                if ((fateTable[index] & mask) == mask)
                    setPointList[nChanged++] = offset; // remember to set pixel to 255
                else
                    coordinates[levelStart + (nUnchanged++)] = xy; // keep this pixel for future passes

            } // for pixel i

            for (int i = 0; i < nChanged; i++)
                pixels[setPointList[i]] = (byte)255;
            return nChanged;
        }

        private int[] makeFateTable()
        {
            var table = new int[256];
            var isSet = new bool[8];

            for (int item = 0; item < 256; item++)
            {
                // dissect into pixels
                for (int i = 0, mask = 1; i < 8; i++)
                {
                    isSet[i] = (item & mask) == mask;
                    mask *= 2;
                }

                for (int i = 0, mask = 1; i < 8; i++)
                {
                    // we dilate in the direction opposite to the direction of the existing neighbors
                    if (isSet[(i + 4) % 8]) table[item] |= mask;
                    mask *= 2;
                }

                for (int i = 0; i < 8; i += 2)
                {
                    // if side pixels are set, for counting transitions it is as good as if the adjacent edges were also set
                    if (isSet[i])
                    {
                        isSet[(i + 1) % 8] = true;
                        isSet[(i + 7) % 8] = true;
                    }
                }

                int transitions = 0;

                for (int i = 0; i < 8; i++)
                {
                    if (isSet[i] != isSet[(i + 1) % 8])
                        transitions++;
                }

                if (transitions >= 4)
                {
                    // if neighbors contain more than one region, dilation ito this pixel is forbidden
                    table[item] = 0;
                }
                else
                {
                }
            }
            return table;
        }

        private bool isWithin(int x, int y, int direction, int width, int height)
        {
            int xmax = width - 1;
            int ymax = height - 1;

            switch (direction)
            {
                case 0:
                    return (y > 0);
                case 1:
                    return (x < xmax && y > 0);
                case 2:
                    return (x < xmax);
                case 3:
                    return (x < xmax && y < ymax);
                case 4:
                    return (y < ymax);
                case 5:
                    return (x > 0 && y < ymax);
                case 6:
                    return (x > 0);
                case 7:
                    return (x > 0 && y > 0);
            }
            return false; // to make the compiler happy :-)
        }

        private float trueEdmHeight(int x, int y, float[] pixels, int width, int height)
        {
            int xmax = width - 1;
            int ymax = height - 1;
            int offset = x + y * width;
            float v = pixels[offset];

            if (x == 0 || y == 0 || x == xmax || y == ymax || v == 0)
                return v; // don't recalculate for edge pixels or background

            float trueH = v + 0.5f * SQRT2; // true height can never by higher than this
            bool ridgeOrMax = false;

            // for all directions halfway around:
            for (int d = 0; d < 4; d++)
            {
                int d2 = (d + 4) % 8; // get the opposite direction and neighbors
                float v1 = pixels[offset + dirOffset[d]];
                float v2 = pixels[offset + dirOffset[d2]];
                float h;

                if (v >= v1 && v >= v2)
                {
                    ridgeOrMax = true;
                    h = (v1 + v2) / 2;
                }
                else
                {
                    h = Math.Min(v1, v2);
                }

                h += (d % 2 == 0) ? 1 : SQRT2; // in diagonal directions, distance is sqrt2

                if (trueH > h)
                    trueH = h;
            }

            if (!ridgeOrMax)
                trueH = v;
            return trueH;
        }

        private static void watershedPostProcess(byte[] src)
        {
            for (int i = 0; i < src.Length; i++)
            {
                if ((src[i] & 255) < 255)
                    src[i] = (byte)0;
            }
        }

    }
}
