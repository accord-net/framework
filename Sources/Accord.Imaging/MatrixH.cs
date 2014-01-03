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
    using System;
    using System.Drawing;

    /// <summary>
    ///   Encapsulates a 3-by-3 general transformation matrix
    ///   that represents a (possibly) non-linear transform. 
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   Linear transformations are not the only ones that can be represented by
    ///   matrices. Using homogeneous coordinates, both affine transformations and
    ///   perspective projections on R^n can be represented as linear transformations
    ///   on R^n+1 (that is, n+1-dimensional real projective space).</para>
    /// <para>
    ///   The general transformation matrix has 8 degrees of freedom, as the last
    ///   element is just a scale parameter.</para>  
    /// </remarks>
    /// 
    [Serializable]
    public class MatrixH
    {

        private float[] elements;

        /// <summary>
        ///   Creates a new projective matrix.
        /// </summary>
        /// 
        public MatrixH()
        {
            // Start as the identity matrix
            this.elements = new float[] { 1, 0, 0, 0, 1, 0, 0, 0 };
        }

        /// <summary>
        ///   Creates a new projective matrix.
        /// </summary>
        /// 
        public MatrixH(float m11, float m12, float m13,
                       float m21, float m22, float m23,
                       float m31, float m32)
        {
            this.elements = new float[8];
            this.elements[0] = m11; this.elements[1] = m12; this.elements[2] = m13;
            this.elements[3] = m21; this.elements[4] = m22; this.elements[5] = m23;
            this.elements[6] = m31; this.elements[7] = m32;
        }

        /// <summary>
        ///   Creates a new projective matrix.
        /// </summary>
        /// 
        public MatrixH(float m11, float m12, float m13,
                       float m21, float m22, float m23,
                       float m31, float m32, float m33)
            : this(m11, m12, m13, m21, m22, m23, m31, m32)
        {
            for (int i = 0; i < elements.Length; i++)
                elements[i] /= m33;
        }

        /// <summary>
        ///   Creates a new projective matrix.
        /// </summary>
        /// 
        public MatrixH(float[] elements)
        {
            if (elements.Length == 8)
            {
                this.elements = elements;
            }
            else if (elements.Length == 9)
            {
                this.elements = new float[8];
                for (int i = 0; i < elements.Length; i++)
                    this.elements[i] = (float)(elements[i] / elements[8]);
            }
            else
            {
                throw new ArgumentException("The element vector should contain either 8 or 9 members.", "elements");
            }
        }

        /// <summary>
        ///   Creates a new projective matrix.
        /// </summary>
        /// 
        public MatrixH(double[,] matrix)
        {
            if (matrix.GetLength(0) != 3 || matrix.GetLength(1) != 3)
            {
                throw new ArgumentException("The projective matrix should be a 3x3 matrix.", "matrix");
            }

            this.elements = new float[8];
            for (int i = 0, k = 0; i < 3; i++)
                for (int j = 0; j < 3 && k < 8; j++, k++)
                    this.elements[k] = (float)(matrix[i, j] / matrix[2, 2]);
        }

        /// <summary>
        ///   Creates a new projective matrix.
        /// </summary>
        /// 
        public MatrixH(float[,] matrix)
        {
            if (matrix.GetLength(0) != 3 || matrix.GetLength(1) != 3)
            {
                throw new ArgumentException("The projective matrix should be a 3x3 matrix.", "matrix");
            }

            this.elements = new float[8];
            for (int i = 0, k = 0; i < 3; i++)
                for (int j = 0; j < 3 && k < 8; j++, k++)
                    this.elements[k] = matrix[i, j] / matrix[2, 2];
        }

        /// <summary>
        ///   Gets the elements of this matrix.
        /// </summary>
        /// 
        public float[] Elements
        {
            get { return elements; }
        }

        /// <summary>
        ///   Gets the offset x
        /// </summary>
        /// 
        public float OffsetX
        {
            get { return elements[2]; }
        }

        /// <summary>
        ///   Gets the offset y
        /// </summary>
        /// 
        public float OffsetY
        {
            get { return elements[5]; }
        }

        /// <summary>
        ///   Gets whether this matrix is invertible.
        /// </summary>
        /// 
        public bool IsInvertible
        {
            get
            {
                float det =
                      elements[0] * (elements[4] - elements[5] * elements[7])
                    - elements[1] * (elements[3] - elements[5] * elements[6])
                    + elements[2] * (elements[3] * elements[7] - elements[4] * elements[6]);

                return det > 0;
            }
        }

        /// <summary>
        ///   Gets whether this is an Affine transformation matrix.
        /// </summary>
        /// 
        public bool IsAffine
        {
            get { return (elements[6] == 0 && elements[7] == 0); }
        }

        /// <summary>
        ///   Gets whether this is the identity transformation.
        /// </summary>
        /// 
        public bool IsIdentity
        {
            get
            {
                return
                    elements[0] == 1 && elements[1] == 0 && elements[2] == 0 &&
                    elements[3] == 0 && elements[4] == 1 && elements[5] == 0 &&
                    elements[6] == 0 && elements[7] == 0;
            }
        }

        /// <summary>
        ///   Resets this matrix to be the identity.
        /// </summary>
        /// 
        public void Reset()
        {
            elements[0] = 1; elements[1] = 0; elements[2] = 0;
            elements[3] = 0; elements[4] = 1; elements[5] = 0;
            elements[6] = 0; elements[7] = 0;
        }

        /// <summary>
        ///   Returns the inverse matrix, if this matrix is invertible.
        /// </summary>
        /// 
        public MatrixH Inverse()
        {
            //    m = 1 / [a(ei-fh) - b(di-fg) + c(dh-eg)]
            // 
            //                  (ei-fh)   (ch-bi)   (bf-ce)
            //  inv(A) =  m  x  (fg-di)   (ai-cg)   (cd-af)
            //                  (dh-eg)   (bg-ah)   (ae-bd)
            //

            float a = this.elements[0], b = this.elements[1], c = this.elements[2];
            float d = this.elements[3], e = this.elements[4], f = this.elements[5];
            float g = this.elements[6], h = this.elements[7];

            float m = 1f / (a * (e - f * h) - b * (d - f * g) + c * (d * h - e * g));
            float na = m * (e - f * h);
            float nb = m * (c * h - b);
            float nc = m * (b * f - c * e);
            float nd = m * (f * g - d);
            float ne = m * (a - c * g);
            float nf = m * (c * d - a * f);
            float ng = m * (d * h - e * g);
            float nh = m * (b * g - a * h);
            float nj = m * (a * e - b * d);

            return new MatrixH(na, nb, nc, nd, ne, nf, ng, nh, nj);
        }

        /// <summary>
        ///   Gets the transpose of this transformation matrix.
        /// </summary>
        /// 
        /// <returns>The transposed version of this matrix, given by <c>H'</c>.</returns>
        public MatrixH Transpose()
        {
            return new MatrixH(elements[0], elements[3], elements[6],
                               elements[1], elements[4], elements[7],
                               elements[2], elements[5]);
        }

        /// <summary>
        ///   Transforms the given points using this transformation matrix.
        /// </summary>
        /// 
        public PointH[] TransformPoints(params PointH[] points)
        {
            PointH[] r = new PointH[points.Length];

            for (int j = 0; j < points.Length; j++)
            {
                r[j].X = elements[0] * points[j].X + elements[1] * points[j].Y + elements[2] * points[j].W;
                r[j].Y = elements[3] * points[j].X + elements[4] * points[j].Y + elements[5] * points[j].W;
                r[j].W = elements[6] * points[j].X + elements[7] * points[j].Y + points[j].W;
            }

            return r;
        }

        /// <summary>
        ///   Transforms the given points using this transformation matrix.
        /// </summary>
        /// 
        public PointF[] TransformPoints(params PointF[] points)
        {
            PointF[] r = new PointF[points.Length];

            for (int j = 0; j < points.Length; j++)
            {
                float w = elements[6] * points[j].X + elements[7] * points[j].Y + 1f;
                r[j].X = (elements[0] * points[j].X + elements[1] * points[j].Y + elements[2]) / w;
                r[j].Y = (elements[3] * points[j].X + elements[4] * points[j].Y + elements[5]) / w;
            }

            return r;
        }

        /// <summary>
        ///   Multiplies this matrix, returning a new matrix as result.
        /// </summary>
        /// 
        public MatrixH Multiply(MatrixH matrix)
        {
            float na = elements[0] * matrix.elements[0] + elements[1] * matrix.elements[3] + elements[2] * matrix.elements[6];
            float nb = elements[0] * matrix.elements[1] + elements[1] * matrix.elements[4] + elements[2] * matrix.elements[7];
            float nc = elements[0] * matrix.elements[2] + elements[1] * matrix.elements[5] + elements[2];

            float nd = elements[3] * matrix.elements[0] + elements[4] * matrix.elements[3] + elements[5] * matrix.elements[6];
            float ne = elements[3] * matrix.elements[1] + elements[4] * matrix.elements[4] + elements[5] * matrix.elements[7];
            float nf = elements[3] * matrix.elements[2] + elements[4] * matrix.elements[5] + elements[5];

            float ng = elements[6] * matrix.elements[0] + elements[7] * matrix.elements[3] + matrix.elements[6];
            float nh = elements[6] * matrix.elements[1] + elements[7] * matrix.elements[4] + matrix.elements[7];
            float ni = elements[6] * matrix.elements[2] + elements[7] * matrix.elements[5] + 1f;

            return new MatrixH(na, nb, nc, nd, ne, nf, ng, nh, ni);
        }

        /// <summary>
        ///   Compares two objects for equality.
        /// </summary>
        /// 
        public override bool Equals(object obj)
        {
            MatrixH m = obj as MatrixH;

            if (obj != null) 
                return this == m;
            return false;
        }

        /// <summary>
        ///   Returns the hash code for this instance.
        /// </summary>
        /// 
        public override int GetHashCode()
        {
            return elements.GetHashCode();
        }

        /// <summary>
        ///   Double[,] conversion.
        /// </summary>
        /// 
        public static explicit operator double[,](MatrixH matrix)
        {
            return new double[,] 
            {
                { matrix.elements[0], matrix.elements[1], matrix.elements[2] },
                { matrix.elements[3], matrix.elements[4], matrix.elements[5] },
                { matrix.elements[6], matrix.elements[7], 1.0 },
            };
        }

        /// <summary>
        ///   Single[,] conversion.
        /// </summary>
        /// 
        public static explicit operator float[,](MatrixH matrix)
        {
            return new float[,] 
            {
                { matrix.elements[0], matrix.elements[1], matrix.elements[2] },
                { matrix.elements[3], matrix.elements[4], matrix.elements[5] },
                { matrix.elements[6], matrix.elements[7], 1.0f },
            };
        }

        /// <summary>
        ///   Double[,] conversion.
        /// </summary>
        /// 
        public double[,] ToDoubleArray()
        {
            return (double[,])this;
        }

        /// <summary>
        ///   Single[,] conversion.
        /// </summary>
        /// 
        public float[,] ToSingleArray()
        {
            return (float[,])this;
        }

        /// <summary>
        ///   Matrix multiplication.
        /// </summary>
        /// 
        public static MatrixH operator *(MatrixH left, MatrixH right)
        {
            return left.Multiply(right);
        }

    }
}
