// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Diego Catalano, 2014
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
// The following code has been based on the original work conducted by
// Dr. Longin Jan Latecki, with explicit permissions from the original
// author to be redistributed in the LGPL. The original work was given
// in the following publication:
//
//   L.J. Latecki and R. Lakaemper; Convexity rule for shape decomposition based
//   on discrete contour evolution. Computer Vision and Image Understanding 73 (3),
//   441-454, 1999.
//

namespace Accord.Math.Geometry
{
    using System;
    using System.Collections.Generic;
    using Accord.Math;
    using Accord.Compat;
    using System.Numerics;

    /// <summary>
    ///   Discrete Curve Evolution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Discrete Curve Evolution (DCE) algorithm can be used to simplify 
    ///   contour curves. It can preserve the outline of a shape by preserving
    ///   its most visually critical points.</para>
    ///   
    /// <para>
    ///   The implementation available in the framework has been contributed by
    ///   Diego Catalano, from the Catalano Framework for Java. The original work
    ///   has been developed by Dr. Longin Jan Latecki, and has been redistributed
    ///   under the LGPL with explicit permission from the original author, as long
    ///   as the following references are acknowledged in derived applications:</para>
    ///
    /// <para>
    ///   L.J. Latecki and R. Lakaemper; Convexity rule for shape decomposition based
    ///   on discrete contour evolution. Computer Vision and Image Understanding 73 (3),
    ///   441-454, 1999.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a 
    ///     href="http://knight.temple.edu/~lakaemper/courses/cis2168_2010FALL/assignments/assig05_folder/CVIU1999.pdf">
    ///       L.J. Latecki and R. Lakaemper; Convexity rule for shape decomposition based
    ///       on discrete contour evolution. Computer Vision and Image Understanding 73 (3),
    ///       441-454, 1999.</a></description></item>
    ///   </list>
    /// </para>   
    /// </remarks>
    /// 
    public class DiscreteCurveEvolution : IShapeOptimizer
    {

        private int vertices = 20;

        /// <summary>
        /// Gets or sets the number of vertices.
        /// </summary>
        /// 
        public int NumberOfVertices
        {
            get { return vertices; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value", "Number of vertices should be higher than zero.");
                vertices = value;
            }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="DiscreteCurveEvolution"/> class.
        /// </summary>
        /// 
        public DiscreteCurveEvolution() { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="DiscreteCurveEvolution"/> class.
        /// </summary>
        /// 
        /// <param name="vertices">Number of vertices.</param>
        /// 
        public DiscreteCurveEvolution(int vertices)
        {
            NumberOfVertices = vertices;
        }

        /// <summary>
        ///   Optimize specified shape.
        /// </summary>
        /// 
        /// <param name="shape">Shape to be optimized.</param>
        /// 
        /// <returns>
        ///   Returns final optimized shape, which may have reduced amount of points.
        /// </returns>
        /// 
        public List<IntPoint> OptimizeShape(List<IntPoint> shape)
        {
            if (vertices > shape.Count)
                throw new ArgumentException("Number of points left must be higher than number of the shape.");

            var complex = new List<Complex>();
            for (int i = 0; i < shape.Count; i++)
                complex.Add(new Complex(shape[i].X, shape[i].Y));

            for (int i = 0; i < shape.Count - vertices; i++)
            {
                double[] winkelmaass = winkel(complex);

                int index = 0;
                Matrix.Min(winkelmaass, out index);

                complex.RemoveAt(index);
            }

            var newShape = new List<IntPoint>(complex.Count);

            for (int i = 0; i < complex.Count; i++)
                newShape.Add(new IntPoint((int)complex[i].Real, (int)complex[i].Imaginary));

            return newShape;
        }

        private static double[] winkel(List<Complex> z)
        {
            int n = z.Count;
            double max = -double.MaxValue;

            double[] his = new double[n];
            for (int j = 1; j < n - 1; j++)
            {
                Complex c = z[j - 1] - z[j + 1];

                double lm = c.Magnitude;

                c = z[j] - z[j + 1];

                double lr = c.Magnitude;

                c = z[j - 1] - z[j];

                double ll = c.Magnitude;

                double alpha = Math.Acos((lr * lr + ll * ll - lm * lm) / (2 * lr * ll));

                // turning angle (0-180)
                double a = 180 - alpha * 180 / Math.PI;

                // the original relevance measure
                his[j] = a * lr * ll / (lr + ll);

                if (his[j] > max) 
                    max = his[j];

            }

            his[0] = max;
            his[n - 1] = max;

            return his;
        }

    }
}
