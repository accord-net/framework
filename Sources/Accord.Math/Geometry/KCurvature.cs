// Accord Math Library
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

namespace Accord.Math.Geometry
{
    using System;
    using System.Collections.Generic;
    using AForge;

    /// <summary>
    ///   K-curvatures algorithm for local maximum contour detection.
    /// </summary>
    /// 
    public class KCurvature
    {

        /// <summary>
        ///   Gets or sets the number K of previous and posterior
        ///   points to consider when find local extremum points.
        /// </summary>
        public int K { get; set; }


        /// <summary>
        ///   Gets or sets the theta angle range (in
        ///   degrees) used to define extremum points.
        /// </summary>
        public DoubleRange Theta { get; set; }

        /// <summary>
        ///   Gets or sets the suppression radius to
        ///   use during non-minimum suppression.
        /// </summary>
        public int Suppression { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="KCurvature"/> class.
        /// </summary>
        /// <param name="k">The number K of previous and posterior
        ///   points to consider when find local extremum points.</param>
        /// <param name="theta">The theta angle range (in
        ///   degrees) used to define extremum points..</param>
        public KCurvature(int k, DoubleRange theta)
        {
            this.K = k;
            this.Theta = theta;
            this.Suppression = k;
        }

        /// <summary>
        ///   Finds local extremum points in the contour.
        /// </summary>
        /// <param name="contour">A list of <see cref="IntPoint">
        /// integer points</see> defining the contour.</param>
        /// 
        public List<IntPoint> FindPeaks(List<IntPoint> contour)
        {
            double[] map = new double[contour.Count];

            for (int i = 0; i < contour.Count; i++)
            {
                IntPoint a, b, c;

                int ai = Accord.Math.Tools.Mod(i + K, contour.Count);
                int ci = Accord.Math.Tools.Mod(i - K, contour.Count);

                a = contour[ai];
                b = contour[i];
                c = contour[ci];


                // http://stackoverflow.com/questions/3486172/angle-between-3-points/3487062#3487062
                //double angle = AForge.Math.Geometry.GeometryTools.GetAngleBetweenVectors(b, a, c);

                DoublePoint ab = new DoublePoint(b.X - a.X, b.Y - a.Y);
                DoublePoint cb = new DoublePoint(b.X - c.X, b.Y - c.Y);

                double angba = System.Math.Atan2(ab.Y, ab.X);
                double angbc = System.Math.Atan2(cb.Y, cb.X);
                double rslt = angba - angbc;

                if (rslt < 0)
                {
                    rslt = 2 * Math.PI + rslt;
                }

                double rs = (rslt * 180) / Math.PI;



                if (Theta.IsInside(rs))
                    map[i] = rs;
            }

            // Non-Minima Suppression
            int r = Suppression;
            List<IntPoint> peaks = new List<IntPoint>();
            for (int i = 0; i < map.Length; i++)
            {
                double current = map[i];
                if (current == 0) continue;

                bool isMinimum = true;

                for (int j = -r; j < r && isMinimum; j++)
                {
                    int index = Accord.Math.Tools.Mod(i + j, map.Length);

                    double candidate = map[index];

                    if (candidate == 0)
                        continue;

                    if (candidate < current)
                        isMinimum = false;
                    else map[index] = 0;
                }

                if (isMinimum)
                    peaks.Add(contour[i]);
            }

            return peaks;
        }


    }
}
