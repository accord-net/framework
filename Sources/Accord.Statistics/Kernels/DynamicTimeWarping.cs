// Accord Statistics Library
// The Accord.NET Framework
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2013
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

namespace Accord.Statistics.Kernels
{
    using Accord.Math;
    using System;
    using System.Threading;
    using System.Collections.Generic;

    /// <summary>
    ///   Dynamic Time Warping Sequence Kernel.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Dynamic Time Warping Sequence Kernel is a sequence kernel, accepting
    ///   vector sequences of variable size as input. Despite the sequences being
    ///   variable in size, the vectors contained in such sequences should have its
    ///   size fixed and should be informed at the construction of this kernel.</para>
    /// <para>
    ///   The conversion of the DTW global distance to a dot product uses a combination
    ///   of a technique known as spherical normalization and the polynomial kernel. The
    ///   degree of the polynomial kernel and the alpha for the spherical normalization
    ///   should be given at the construction of the kernel. For more information,
    ///   please see the referenced papers shown below.</para>
    ///   
    /// <para>
    ///   The use of a <see cref="KernelFunctionCache">cache</see> is highly advisable
    ///   when using this kernel.</para>
    ///   
    /// <para>
    ///   <list type="bullet">
    ///   References:
    ///     <item><description>
    ///     V. Wan, J. Carmichael; Polynomial Dynamic Time Warping Kernel Support
    ///     Vector Machines for Dysarthric Speech Recognition with Sparse Training
    ///     Data. Interspeech'2005 - Eurospeech - 9th European Conference on Speech
    ///     Communication and Technology. Lisboa, 2005.</description></item>
    ///   </list></para>
    /// 
    /// </remarks>
    /// 
    [Serializable]
    public class DynamicTimeWarping : IKernel, ICloneable, IDisposable
    {
        private double alpha = 1.0; // spherical projection distance
        private int length = 1;     // length of the feature vectors
        private int degree = 1;     // polynomial kernel degree

        [NonSerialized]
        private ThreadLocal<Locals> locals =
            new ThreadLocal<Locals>(() => new Locals());

        /// <summary>
        ///   Gets or sets the length for the feature vectors
        ///   contained in each sequence used by the kernel.
        /// </summary>
        /// 
        public int Length
        {
            get { return length; }
            set { length = value; }
        }

        /// <summary>
        ///   Gets or sets the hypersphere ratio.
        /// </summary>
        /// 
        public double Alpha
        {
            get { return alpha; }
            set { alpha = value; }
        }

        /// <summary>
        ///   Gets or sets the polynomial degree for this kernel.
        /// </summary>
        /// 
        public int Degree
        {
            get { return degree; }
            set { degree = value; }
        }

        /// <summary>
        ///   Constructs a new Dynamic Time Warping kernel.
        /// </summary>
        /// 
        /// <param name="length">
        ///    The length of the feature vectors
        ///    contained in each sequence.
        /// </param>
        /// 
        public DynamicTimeWarping(int length)
        {
            this.length = length;
        }

        /// <summary>
        ///   Constructs a new Dynamic Time Warping kernel.
        /// </summary>
        /// 
        /// <param name="length">
        ///    The length of the feature vectors
        ///    contained in each sequence.
        /// </param>
        /// 
        /// <param name="alpha">
        ///    The hypersphere ratio. Default value is 1.
        /// </param>
        /// 
        public DynamicTimeWarping(int length, double alpha)
        {
            this.length = length;
            this.alpha = alpha;
        }

        /// <summary>
        ///   Constructs a new Dynamic Time Warping kernel.
        /// </summary>
        /// 
        /// <param name="length">
        ///    The length of the feature vectors
        ///    contained in each sequence.
        /// </param>
        /// 
        /// <param name="alpha">
        ///    The hypersphere ratio. Default value is 1.
        /// </param>
        /// 
        /// <param name="degree">
        ///    The degree of the kernel. Default value is 1 (linear kernel).
        /// </param>
        /// 
        public DynamicTimeWarping(int length, double alpha, int degree)
        {
            this.alpha = alpha;
            this.degree = degree;
            this.length = length;
        }


        /// <summary>
        ///   Dynamic Time Warping kernel function.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public double Function(double[] x, double[] y)
        {
            if (x == y) return 1.0;

            Locals m = locals.Value;

            double[] sx = snorm(x);
            double[] sy = snorm(y);

            // if (!m.vectors.TryGetValue(x, out sx))
            //     m.vectors[x] = sx = snorm(x);

            //if (!m.vectors.TryGetValue(y, out sy))
            //    m.vectors[y] = sy = snorm(y);

            // Compute the cosine of the global distance
            double distance = D(m, sx, sy);
            double cos = System.Math.Cos(distance);

            // Return cos for the linear kernel, cos^n for polynomial
            return (degree == 1) ? cos : System.Math.Pow(cos, degree);
        }


        /// <summary>
        ///   Global distance D(X,Y) between two sequences of vectors.
        /// </summary>
        /// 
        /// <param name="locals">The current thread local storage.</param>
        /// <param name="sequence1">A sequence of vectors.</param>
        /// <param name="sequence2">A sequence of vectors.</param>
        /// 
        /// <returns>The global distance between X and Y.</returns>
        /// 
        private unsafe double D(Locals locals, double[] sequence1, double[] sequence2)
        {
            // Get the number of vectors in each sequence. The vectors
            // have been projected, so the length is augmented by one.
            int vectorSize = length + 1;
            int vectorCount1 = sequence1.Length / vectorSize;
            int vectorCount2 = sequence2.Length / vectorSize;

            // Application of the Dynamic Time Warping
            // algorithm by using dynamic programming.
            if (locals.m < vectorCount2 || locals.n < vectorCount1)
                locals.Create(vectorCount1, vectorCount2);

            double[,] DTW = locals.DTW;


            fixed (double* start1 = sequence1)
            fixed (double* start2 = sequence2)
            {
                double* vector1 = start1;

                for (int i = 0; i < vectorCount1; i++, vector1 += vectorSize)
                {
                    double* vector2 = start2;

                    for (int j = 0; j < vectorCount2; j++, vector2 += vectorSize)
                    {
                        double prod = 0; // inner product 
                        for (int k = 0; k < vectorSize; k++)
                            prod += vector1[k] * vector2[k];

                        // Return the arc-cosine of the inner product
                        double cost = Math.Acos(prod > 1 ? 1 : (prod < -1 ? -1 : prod));

                        double insertion = DTW[i, j + 1];
                        double deletion = DTW[i + 1, j];
                        double match = DTW[i, j];

                        double min = (insertion < deletion
                            ? (insertion < match ? insertion : match)
                            : (deletion < match ? deletion : match));

                        DTW[i + 1, j + 1] = cost + min;
                    }
                }
            }

            return DTW[vectorCount1, vectorCount2]; // return the minimum global distance
        }



        /// <summary>
        ///   Projects vectors from a sequence of vectors into
        ///   a hypersphere, augmenting their size in one unit
        ///   and normalizing them to be unit vectors.
        /// </summary>
        /// 
        /// <param name="input">A sequence of vectors.</param>
        /// 
        /// <returns>A sequence of vector projections.</returns>
        /// 
        private unsafe double[] snorm(double[] input)
        {
            // Get the number of vectors in the sequence
            int n = input.Length / length;

            // Create the augmented sequence projection
            double[] projection = new double[input.Length + n];

            fixed (double* source = input)
            fixed (double* result = projection)
            {
                double* src = source;
                double* dst = result;

                for (int i = 0; i < n; i++)
                {
                    double norm = alpha * alpha;

                    for (int j = 0; j < length; j++)
                        norm += src[j] * src[j];
                    norm = Math.Sqrt(norm);

                    for (int j = 0; j < length; j++, src++, dst++)
                        *dst = *src / norm;

                    *(dst++) = alpha / norm;
                }
            }

            return projection; // return the projected sequence
        }

        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public object Clone()
        {
            return MemberwiseClone();
        }


        private class Locals
        {
            public double[,] DTW;
            public int m;
            public int n;
            // Dictionary<double[], double[]> vectors;

            public Locals()
            {
                // vectors = new Dictionary<double[], double[]>();
            }

            public void Create(int n, int m)
            {
                this.n = n;
                this.m = m;
                this.DTW = new double[n + 1, m + 1];

                for (int i = 1; i <= n; i++)
                    DTW[i, 0] = double.PositiveInfinity;

                for (int i = 1; i <= m; i++)
                    DTW[0, i] = double.PositiveInfinity;
            }

        }


         /// <summary>
        ///   Performs application-defined tasks associated with freeing, 
        ///   releasing, or resetting unmanaged resources.
        /// </summary>
        /// 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///   Releases unmanaged resources and performs other cleanup operations 
        ///   before the <see cref="DynamicTimeWarping"/> is reclaimed by garbage collection.
        /// </summary>
        /// 
        ~DynamicTimeWarping()
        {
            Dispose(false);
        }

        /// <summary>
        ///   Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// 
        /// <param name="disposing"><c>true</c> to release both managed
        /// and unmanaged resources; <c>false</c> to release only unmanaged
        /// resources.</param>
        ///
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (locals != null)
                {
                    locals.Dispose();
                    locals = null;
                }
            }

        }
    }
}
