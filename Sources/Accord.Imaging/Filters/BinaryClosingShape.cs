// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Hashem Zawary, 2016-2017
// hashemzawary at gmail.com
// https://www.linkedin.com/in/hashem-zavvari-53b01457
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
    using System.Drawing;
    using Accord.Math;

    /// <summary>
    ///   Closing Binary Shape operator from Mathematical Morphology with 3x3 structuring element.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   Similar to the <see cref="Closing"/> filter, this filter can close (heal) a single binarized 
    ///   shape in a larger image (e.g. a jagged circle with holes) whitout changing the shape structure.</para>
    ///   
    /// <para>
    ///   The filter accepts 8 bpp grayscale images for processing.</para>
    /// </remarks>
    /// 
    /// <seealso cref="Closing"/>
    /// 
    public class ClosingBinaryShape : BaseUsingCopyPartialFilter
    {
        // private format translation dictionary
        private readonly Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        /// 
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClosingBinaryShape"/> class.
        /// </summary>
        /// 
        public ClosingBinaryShape()
        {
            // initialize format translation dictionary
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
        }


        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// <param name="destinationData">Destination image data.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        /// 
        /// <exception cref="InvalidImagePropertiesException">Processing rectangle mast be at least 4x4 in size.</exception>
        /// 
        protected override void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData, Rectangle rect)
        {
            if ((rect.Width < 4) || (rect.Height < 4))
            {
                throw new InvalidImagePropertiesException("Processing rectangle mast be at least 4x4 in size.");
            }

            List<byte[,]> patterns = Get3X3CornerPatterns();
            var hotPoints = new List<IntPoint>();

            foreach (var point in sourceData.CollectActivePixels())
            {
                byte[,] matrix3X3 = GetNeighborValues(point, 1, ref sourceData);

                foreach (byte[,] pattern in patterns)
                {
                    if (matrix3X3.IsEqual(pattern))
                        hotPoints.Add(point);
                }

            }

            var blackList = new List<int>();
            var pairPoints = new List<IntPoint>();

            for (int i = 0; i < hotPoints.Count; i++)
            {
                float distance = float.MaxValue;
                int index = -1;

                for (int j = i + 1; j < hotPoints.Count; j++)
                {
                    if (blackList.Contains(j))
                        continue;

                    bool isValidPoint = true;

                    foreach (var value in sourceData.Collect8bppPixelValues(GetLineInPoints(hotPoints[i], hotPoints[j])))
                    {
                        if (value == byte.MaxValue)
                        {
                            isValidPoint = false;
                            break;
                        }
                    }

                    if (!isValidPoint)
                        continue;

                    float d = hotPoints[i].DistanceTo(hotPoints[j]);

                    if (d > distance)
                        continue;

                    index = j;
                    distance = d;
                }

                if (index == -1)
                    continue;

                blackList.Add(index);
                pairPoints.Add(hotPoints[i]);
                pairPoints.Add(hotPoints[index]);
            }

            for (var i = 0; i < pairPoints.Count; i++)
                Drawing.Line(destinationData, pairPoints[i], pairPoints[++i], Color.White);
        }

        private static byte[,] GetNeighborValues(IntPoint subjectPoint, byte boxDistance, ref UnmanagedImage imageData)
        {
            int rows = imageData.Height;
            int cols = imageData.Width;

            int minX = subjectPoint.X - boxDistance;
            int maxX = subjectPoint.X + boxDistance;
            int minY = subjectPoint.Y - boxDistance;
            int maxY = subjectPoint.Y + boxDistance;

            var existNeighborPoints = new List<IntPoint>();

            int squreSize = 2 * boxDistance + 1;
            var result = new byte[squreSize, squreSize];

            int k = 0;
            for (var i = minX; i <= maxX; i++, k++)
            {
                var l = 0;
                for (var j = minY; j <= maxY; j++, l++)
                {
                    if (i < 0 || i >= cols || j < 0 || j >= rows) continue;

                    existNeighborPoints.Add(new IntPoint(i, j));

                    result[k, l] = byte.MaxValue;
                }
            }

            byte[] values = imageData.Collect8bppPixelValues(existNeighborPoints);

            int seqCounter = 0;

            for (int i = 0; i < squreSize; i++)
                for (int j = 0; j < squreSize; j++)
                    if (result[i, j] == byte.MaxValue)
                        result[i, j] = values[seqCounter++];

            return result;
        }

        private static List<byte[,]> Get3X3CornerPatterns()
        {
            const byte x = byte.MaxValue;

            var result = new List<byte[,]>(16)
            {
                new byte[,]
                {
                    {x, 0, 0},
                    {0, x, 0},
                    {0, 0, 0}
                },
                new byte[,]
                {
                    {0, x, 0},
                    {0, x, 0},
                    {0, 0, 0}
                },
                new byte[,]
                {
                    {0, 0, x},
                    {0, x, 0},
                    {0, 0, 0}
                },
                new byte[,]
                {
                    {0, 0, 0},
                    {0, x, x},
                    {0, 0, 0}
                },
                new byte[,]
                {
                    {0, 0, 0},
                    {0, x, 0},
                    {0, 0, x}
                },
                new byte[,]
                {
                    {0, 0, 0},
                    {0, x, 0},
                    {0, x, 0}
                },
                new byte[,]
                {
                    {0, 0, 0},
                    {0, x, 0},
                    {x, 0, 0}
                },
                new byte[,]
                {
                    {0, 0, 0},
                    {x, x, 0},
                    {0, 0, 0}
                },
                new byte[,]
                {
                    {x, x, 0},
                    {0, x, 0},
                    {0, 0, 0}
                },
                new byte[,]
                {
                    {0, x, x},
                    {0, x, 0},
                    {0, 0, 0}
                },
                new byte[,]
                {
                    {0, 0, x},
                    {0, x, x},
                    {0, 0, 0}
                },
                new byte[,]
                {
                    {0, 0, 0},
                    {0, x, x},
                    {0, 0, x}
                },
                new byte[,]
                {
                    {0, 0, 0},
                    {0, x, 0},
                    {0, x, x}
                },
                new byte[,]
                {
                    {0, 0, 0},
                    {0, x, 0},
                    {x, x, 0}
                },
                new byte[,]
                {
                    {0, 0, 0},
                    {x, x, 0},
                    {x, 0, 0}
                },
                new byte[,]
                {
                    {x, 0, 0},
                    {x, x, 0},
                    {0, 0, 0}
                }
            };

            return result;
        }

        private static List<IntPoint> GetLineInPoints(IntPoint point1, IntPoint point2)
        {
            int startX = point1.X;
            int startY = point1.Y;
            int stopX = point2.X;
            int stopY = point2.Y;

            int dx = stopX - startX;
            int dy = stopY - startY;

            var result = new List<IntPoint>();

            if (Math.Abs(dx) >= Math.Abs(dy))
            {
                // the line is more horizontal, we'll plot along the X axis
                float slope = (dx != 0) ? (float)dy / dx : 0;
                int step = (dx > 0) ? 1 : -1;

                // correct dx so last point is included as well
                dx += step;

                for (int x = 1; x < (dx - 1); x += step)
                {
                    var px = startX + x;
                    var py = (int)(startY + (slope * x));

                    result.Add(new IntPoint(px, py));
                }
            }
            else
            {
                // the line is more vertical, we'll plot along the y axis.
                float slope = (dy != 0) ? (float)dx / dy : 0;
                int step = (dy > 0) ? 1 : -1;

                // correct dy so last point is included as well
                dy += step;

                for (int y = 1; y < (dy - 1); y += step)
                {
                    var px = (int)(startX + (slope * y));
                    var py = startY + y;

                    result.Add(new IntPoint(px, py));
                }
            }

            return result;
        }

    }
}
