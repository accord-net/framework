﻿// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
// Original work copyright © Lutz Roeder, 2000
//  Adapted from Mapack for .NET, September 2000
//  Adapted from Mapack for COM and Jama routines
//  http://www.aisto.com/roeder/dotnet
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

namespace Accord.Math.Decompositions
{
    using System;
    using Accord.Math;
	using System.Diagnostics;
	using Accord.Compat;

    /// <summary>
    ///   Singular Value Decomposition for a rectangular matrix.
    /// </summary>
    ///
    /// <remarks>
    ///  <para>
    ///      For an m-by-n matrix <c>A</c> with <c>m >= n</c>, the singular value decomposition
    ///   is an m-by-n orthogonal matrix <c>U</c>, an n-by-n diagonal matrix <c>S</c>, and
    ///   an n-by-n orthogonal matrix <c>V</c> so that <c>A = U * S * V'</c>.
    ///   The singular values, <c>sigma[k] = S[k,k]</c>, are ordered so that
    ///   <c>sigma[0] >= sigma[1] >= ... >= sigma[n-1]</c>.</para>
    ///  <para>
    ///   The singular value decomposition always exists, so the constructor will
    ///   never fail. The matrix condition number and the effective numerical
    ///   rank can be computed from this decomposition.</para>
    ///  <para>
    ///   WARNING! Please be aware that if A has less rows than columns, it is better
    ///   to compute the decomposition on the transpose of A and then swap the left
    ///   and right eigenvectors. If the routine is computed on A directly, the diagonal
    ///   of singular values may contain one or more zeros. The identity A = U * S * V'
    ///   may still hold, however. To overcome this problem, pass true to the
    ///   <see cref="SingularValueDecomposition(Double[,], bool, bool, bool)">autoTranspose</see>
    ///   argument of the class constructor.</para>
    ///
    ///  <para>
    ///   This routine computes the economy decomposition of A.</para> 
    /// </remarks>
    /// 
    [Serializable]
    public sealed class SingularValueDecomposition : ICloneable, ISolverMatrixDecomposition<Double>
    {
        private Double[,] u; // left singular vectors
        private Double[,] v; // right singular vectors
        private Double[] s;  // singular values
        private int m;
        private int n;
        private bool swapped;

        private int[] si; // sorting order

        private const Double eps = 2 * Constants.DoubleEpsilon;
        private const Double tiny = Constants.DoubleSmall;

        int? rank;
		Double? determinant;
        Double? lndeterminant;
        Double? pseudoDeterminant;
        Double? lnpseudoDeterminant;

		Double[,] diagonalMatrix;

        /// <summary>
        ///   Returns the condition number <c>max(S) / min(S)</c>.
        /// </summary>
        ///
        public Double Condition
        {
            get { return s[0] / s[System.Math.Max(m, n) - 1]; }
        }

        /// <summary>
        ///   Returns the singularity threshold.
        /// </summary>
        ///
        public Double Threshold
        {
            get { return Constants.DoubleEpsilon * System.Math.Max(m, n) * s[0]; }
        }

        /// <summary>
        ///   Returns the Two norm.
        /// </summary>
        ///
        public Double TwoNorm
        {
            get { return s[0]; }
        }

        /// <summary>
        ///   Returns the effective numerical matrix rank.
        /// </summary>
        ///
        /// <value>Number of non-negligible singular values.</value>
        ///
        public int Rank
        {
            get
            {
				if (this.rank.HasValue)
					return this.rank.Value;

                Double tol = System.Math.Max(m, n) * s[0] * eps;

                int r = 0;
                for (int i = 0; i < s.Rows(); i++)
                    if (s[i] > tol) r++;

                return (int)(this.rank = r);
            }
        }

        /// <summary>
        ///   Gets whether the decomposed matrix is singular.
        /// </summary>
        ///
        public bool IsSingular
        {
            get { return Rank < Math.Max(m, n); }
        }

        /// <summary>
        ///   Gets the one-dimensional array of singular values.
        /// </summary>        
        ///
        public Double[] Diagonal
        {
            get { return this.s; }
        }

        /// <summary>
        ///  Returns the block diagonal matrix of singular values.
        /// </summary>        
        ///
        public Double[,] DiagonalMatrix
        {
            get 
			{	
				if (this.diagonalMatrix != null)
					return this.diagonalMatrix;

				return diagonalMatrix = Matrix.Diagonal(u.Columns(), v.Columns(), s);
			}
        }

        /// <summary>
        ///   Returns the V matrix of Singular Vectors.
        /// </summary>        
        ///
        public Double[,] RightSingularVectors
        {
            get { return v; }
        }

        /// <summary>
        ///   Returns the U matrix of Singular Vectors.
        /// </summary>        
        ///
        public Double[,] LeftSingularVectors
        {
            get { return u; }
        }

        /// <summary>
        ///   Returns the ordering in which the singular values have been sorted.
        /// </summary>
        ///
        public int[] Ordering
        {
            get { return si; }
        }

        /// <summary>
        ///   Returns the absolute value of the matrix determinant.
        /// </summary>
        ///
        public Double AbsoluteDeterminant
        {
            get
            {
                if (!determinant.HasValue)
                {
                    Double det = 1;
                    for (int i = 0; i < s.Rows(); i++)
                        det *= s[i];
                    determinant = det;
                }

                return determinant.Value;
            }
        }

        /// <summary>
        ///   Returns the log of the absolute value for the matrix determinant.
        /// </summary>
        ///
        public Double LogDeterminant
        {
            get
            {
                if (!lndeterminant.HasValue)
                {
                    double det = 0;
                    for (int i = 0; i < s.Rows(); i++)
                        det += Math.Log((double)s[i]);
                    lndeterminant = (Double)det;
                }

                return lndeterminant.Value;
            }
        }


        /// <summary>
        ///   Returns the pseudo-determinant for the matrix.
        /// </summary>
        ///
        public Double PseudoDeterminant
        {
            get
            {
                if (!pseudoDeterminant.HasValue)
                {
                    Double det = 1;
                    for (int i = 0; i < s.Rows(); i++)
                        if (s[i] != 0) det *= s[i];
                    pseudoDeterminant = det;
                }

                return pseudoDeterminant.Value;
            }
        }

        /// <summary>
        ///   Returns the log of the pseudo-determinant for the matrix.
        /// </summary>
        ///
        public Double LogPseudoDeterminant
        {
            get
            {
                if (!lnpseudoDeterminant.HasValue)
                {
                    double det = 0;
                    for (int i = 0; i < s.Rows(); i++)
                        if (s[i] != 0) det += Math.Log((double)s[i]);
                    lnpseudoDeterminant = (Double)det;
                }

                return lnpseudoDeterminant.Value;
            }
        }


        /// <summary>
        ///   Constructs a new singular value decomposition.
        /// </summary>
        ///
        /// <param name="value">
        ///   The matrix to be decomposed.</param>
        ///
        public SingularValueDecomposition(Double[,] value)
            : this(value, true, true)
        {
        }


        /// <summary>
        ///     Constructs a new singular value decomposition.
        /// </summary>
        /// 
        /// <param name="value">
        ///   The matrix to be decomposed.</param>
        /// <param name="computeLeftSingularVectors">
        ///   Pass <see langword="true"/> if the left singular vector matrix U 
        ///   should be computed. Pass <see langword="false"/> otherwise. Default
        ///   is <see langword="true"/>.</param>
        /// <param name="computeRightSingularVectors">
        ///   Pass <see langword="true"/> if the right singular vector matrix V
        ///   should be computed. Pass <see langword="false"/> otherwise. Default
        ///   is <see langword="true"/>.</param>
        /// 
        public SingularValueDecomposition(Double[,] value,
            bool computeLeftSingularVectors, bool computeRightSingularVectors)
            : this(value, computeLeftSingularVectors, computeRightSingularVectors, false)
        {
        }

        /// <summary>
        ///   Constructs a new singular value decomposition.
        /// </summary>
        /// 
        /// <param name="value">
        ///   The matrix to be decomposed.</param>
        /// <param name="computeLeftSingularVectors">
        ///   Pass <see langword="true"/> if the left singular vector matrix U 
        ///   should be computed. Pass <see langword="false"/> otherwise. Default
        ///   is <see langword="true"/>.</param>
        /// <param name="computeRightSingularVectors">
        ///   Pass <see langword="true"/> if the right singular vector matrix V 
        ///   should be computed. Pass <see langword="false"/> otherwise. Default
        ///   is <see langword="true"/>.</param>
        /// <param name="autoTranspose">
        ///   Pass <see langword="true"/> to automatically transpose the value matrix in
        ///   case JAMA's assumptions about the dimensionality of the matrix are violated.
        ///   Pass <see langword="false"/> otherwise. Default is <see langword="false"/>.</param>
        /// 
        public SingularValueDecomposition(Double[,] value, 
            bool computeLeftSingularVectors, bool computeRightSingularVectors, bool autoTranspose)
            : this(value, computeLeftSingularVectors, computeRightSingularVectors, autoTranspose, false)
        {
        }

        /// <summary>
        ///   Constructs a new singular value decomposition.
        /// </summary>
        /// 
        /// <param name="value">
        ///   The matrix to be decomposed.</param>
        /// <param name="computeLeftSingularVectors">
        ///   Pass <see langword="true"/> if the left singular vector matrix U 
        ///   should be computed. Pass <see langword="false"/> otherwise. Default
        ///   is <see langword="true"/>.</param>
        /// <param name="computeRightSingularVectors">
        ///   Pass <see langword="true"/> if the right singular vector matrix V 
        ///   should be computed. Pass <see langword="false"/> otherwise. Default
        ///   is <see langword="true"/>.</param>
        /// <param name="autoTranspose">
        ///   Pass <see langword="true"/> to automatically transpose the value matrix in
        ///   case JAMA's assumptions about the dimensionality of the matrix are violated.
        ///   Pass <see langword="false"/> otherwise. Default is <see langword="false"/>.</param>
        /// <param name="inPlace">
        ///   Pass <see langword="true"/> to perform the decomposition in place. The matrix
        ///   <paramref name="value"/> will be destroyed in the process, resulting in less
        ///   memory comsumption.</param>
        /// 
        public SingularValueDecomposition(Double[,] value,
           bool computeLeftSingularVectors, bool computeRightSingularVectors, bool autoTranspose, bool inPlace)
        {
            if (value == null)
                throw new ArgumentNullException("value", "Matrix cannot be null.");

            Double[,] a;
            m = value.Rows();    // rows

            if (m == 0)
              throw new ArgumentException("Matrix does not have any rows.", "value");

            n = value.Columns(); // cols

            if (n == 0)
               throw new ArgumentException("Matrix does not have any columns.", "value");
            
            if (m < n) // Check if we are violating JAMA's assumption
            {
                if (!autoTranspose) // Yes, check if we should correct it
                {
                    // Warning! This routine is not guaranteed to work when A has less rows
                    //  than columns. If this is the case, you should compute SVD on the
                    //  transpose of A and then swap the left and right eigenvectors.

                    // However, as the solution found can still be useful, the exception below
                    // will not be thrown, and only a warning will be output in the trace.

                    // throw new ArgumentException("Matrix should have more rows than columns.");

                    Trace.WriteLine("WARNING: Computing SVD on a matrix with more columns than rows.");

                    // Proceed anyway
                    a = inPlace ? value : value.Copy();
                }
                else
                {
                    // Transposing and swapping
                    a = value.Transpose(inPlace && m == n);
                    n = value.Rows();    // rows
                    m = value.Columns(); // cols
                    swapped = true;

                    bool aux = computeLeftSingularVectors;
                    computeLeftSingularVectors = computeRightSingularVectors;
                    computeRightSingularVectors = aux;
                }
            }
            else
            {
                // Input matrix is ok
                a = inPlace ? value : value.Copy();
            }


            int nu = System.Math.Min(m, n);
            int ni = System.Math.Min(m + 1, n);
            s = new Double[ni];
            u = new Double[m, nu];
            v = new Double[n, n];

            Double[] e = new Double[n];
            Double[] work = new Double[m];
            bool wantu = computeLeftSingularVectors;
            bool wantv = computeRightSingularVectors;

            // Will store ordered sequence of indices after sorting.
            si = new int[ni]; for (int i = 0; i < ni; i++) si[i] = i;


            // Reduce A to bidiagonal form, storing the diagonal elements in s and the super-diagonal elements in e.
            int nct = System.Math.Min(m - 1, n);
            int nrt = System.Math.Max(0, System.Math.Min(n - 2, m));
            int mrc = System.Math.Max(nct, nrt);

            for (int k = 0; k < mrc; k++)
            {
                if (k < nct)
                {
                    // Compute the transformation for the k-th column and place the k-th diagonal in s[k].
                    // Compute 2-norm of k-th column without under/overflow.
                    s[k] = 0;
                    for (int i = k; i < a.Rows(); i++)
                        s[k] = Accord.Math.Tools.Hypotenuse(s[k], a[i, k]);

                    if (s[k] != 0) 
                    {
                       if (a[k, k] < 0)
                          s[k] = -s[k];

                       for (int i = k; i < a.Rows(); i++) 
                          a[i, k] /= s[k];
               
                       a[k, k] += 1;
                    }

                    s[k] = -s[k];
                }

                for (int j = k+1; j < n; j++)
                {
                    if ((k < nct) & (s[k] != 0))
                    {
                        // Apply the transformation.
                        Double t = 0;
                        for (int i = k; i < a.Rows(); i++)
                          t += a[i, k] * a[i, j];

                       t = -t / a[k, k];

                       for (int i = k; i < a.Rows(); i++)
                          a[i, j] += t * a[i, k];
                     }

                     // Place the k-th row of A into e for the
                     // subsequent calculation of the row transformation.

                     e[j] = a[k, j];
                 }

                 if (wantu & (k < nct))
                 {
                    // Place the transformation in U for subsequent back
                    // multiplication.

                    for (int i = k; i < a.Rows(); i++)
                       u[i, k] = a[i, k];
                 }

                 if (k < nrt)
                 {
                    // Compute the k-th row transformation and place the
                    // k-th super-diagonal in e[k].
                    // Compute 2-norm without under/overflow.
                    e[k] = 0;
                    for (int i = k + 1; i < e.Rows(); i++)
                       e[k] = Tools.Hypotenuse(e[k], e[i]);

                    if (e[k] != 0)
                    {
                       if (e[k+1] < 0) 
                          e[k] = -e[k];

                       for (int i = k + 1; i < e.Rows(); i++) 
                          e[i] /= e[k];

                       e[k+1] += 1;
                    }

                    e[k] = -e[k];
                    if ((k + 1 < m) & (e[k] != 0))
                    {
                        // Apply the transformation.
                        for (int i = k + 1; i < work.Rows(); i++)
                            work[i] = 0;

                        for (int i = k + 1; i < a.Rows(); i++)
                            for (int j = k + 1; j < a.Columns(); j++)
                                work[i] += e[j] * a[i, j];

                       for (int j = k + 1; j < n; j++)
                       {
                          Double t = -e[j] / e[k+1];
                          for (int i = k + 1; i < work.Rows(); i++) 
                             a[i, j] += t * work[i];
                       }
                    }

                    if (wantv)
                    {
                        // Place the transformation in V for subsequent
                        // back multiplication.

                        for (int i = k + 1; i < v.Rows(); i++)
                           v[i, k] = e[i];
                    }
                }
            }

            // Set up the final bidiagonal matrix or order p.
            int p = System.Math.Min(n, m + 1);
            if (nct < n) 
                s[nct] = a[nct, nct];
            if (m < p) 
                s[p - 1] = 0;
            if (nrt + 1 < p) 
                e[nrt] = a[nrt, p - 1];
            e[p - 1] = 0;

            // If required, generate U.
            if (wantu)
            {
                for (int j = nct; j < nu; j++)
                {
                    for (int i = 0; i < u.Rows(); i++) 
                        u[i, j] = 0;

                    u[j, j] = 1;
                }

                for (int k = nct-1; k >= 0; k--)
                {
                    if (s[k] != 0)
                    {
                        for (int j = k + 1; j < nu; j++)
                        {
                            Double t = 0;
                            for (int i = k; i < u.Rows(); i++)
                                t += u[i, k] * u[i, j];

                            t = -t / u[k, k];

                            for (int i = k; i < u.Rows(); i++)
                                u[i, j] += t * u[i, k];
                        }

                        for (int i = k; i < u.Rows(); i++ )
                            u[i, k] = -u[i, k];

                        u[k, k] = 1 + u[k, k];
                        for (int i = 0; i < k - 1; i++) 
                            u[i, k] = 0;
                    }
                    else
                    {
                        for (int i = 0; i < u.Rows(); i++) 
                            u[i, k] = 0;
                        u[k, k] = 1;
                    }
                    }
            }
              

            // If required, generate V.
            if (wantv)
            {
                for (int k = n - 1; k >= 0; k--)
                {
                    if ((k < nrt) & (e[k] != 0))
                    {
                        // TODO: The following is a pseudo correction to make SVD
                        //  work on matrices with n > m (less rows than columns).

                        // For the proper correction, compute the decomposition of the
                        //  transpose of A and swap the left and right eigenvectors

                        // Original line:
                        //   for (int j = k + 1; j < nu; j++)
                        // Pseudo correction:
                        //   for (int j = k + 1; j < n; j++)

                        for (int j = k + 1; j < n; j++) // pseudo-correction
                        {
                            Double t = 0;
                            for (int i = k + 1; i < v.Rows(); i++)
                                t += v[i, k] * v[i, j];

                            t = -t / v[k+1, k];
                            for (int i = k + 1; i < v.Rows(); i++)
                                v[i, j] += t * v[i, k];
                        }
                    }

                    for (int i = 0; i < v.Rows(); i++)
                        v[i, k] = 0;
                    v[k, k] = 1;
                }
            }

            // Main iteration loop for the singular values.

            int pp = p-1;
            int iter = 0;
            Double eps = Constants.DoubleEpsilon;
            while (p > 0)
            {
                int k,kase;

                // Here is where a test for too many iterations would go.

                // This section of the program inspects for
                // negligible elements in the s and e arrays.  On
                // completion the variables kase and k are set as follows.

                // kase = 1     if s(p) and e[k-1] are negligible and k<p
                // kase = 2     if s(k) is negligible and k<p
                // kase = 3     if e[k-1] is negligible, k<p, and
                //              s(k), ..., s(p) are not negligible (qr step).
                // kase = 4     if e(p-1) is negligible (convergence).

                for (k = p - 2; k >= -1; k--)
                {
                    if (k == -1)
                        break;

                    var alpha = tiny + eps * (System.Math.Abs(s[k]) + System.Math.Abs(s[k + 1]));
                    if (System.Math.Abs(e[k]) <= alpha || Double.IsNaN(e[k]))
                    {
                        e[k] = 0;
                        break;
                    }
                }

                if (k == p-2)
                    kase = 4;

                else
                {
                    int ks;
                    for (ks = p - 1; ks >= k; ks--)
                    {
                       if (ks == k)
                          break;

                       Double t = (ks != p     ? Math.Abs(e[ks])   : (Double)0) + 
                                  (ks != k + 1 ? Math.Abs(e[ks-1]) : (Double)0);

                       if (Math.Abs(s[ks]) <= eps*t) 
                       {
                          s[ks] = 0;
                          break;
                       }
                    }

                    if (ks == k)
                       kase = 3;

                    else if (ks == p-1)
                       kase = 1;

                    else
                    {
                       kase = 2;
                       k = ks;
                    }
                 }

                 k++;

                 // Perform the task indicated by kase.
                 switch (kase)
                 {
                    // Deflate negligible s(p).
                    case 1:
                    {
                       Double f = e[p - 2];
                       e[p-2] = 0;
                       for (int j = p - 2; j >= k; j--) 
                       {
                          Double t = Tools.Hypotenuse(s[j],f);
                          Double cs = s[j] / t;
                          Double sn = f / t;
                          s[j] = t;
                          if (j != k) 
                          {
                             f = -sn * e[j - 1];
                             e[j - 1] = cs * e[j - 1];
                          }
                          if (wantv) 
                          {
                             for (int i = 0; i < v.Rows(); i++) 
                             {
                                t = cs * v[i, j] + sn * v[i, p-1];
                                v[i, p-1] = -sn * v[i, j] + cs * v[i, p-1];
                                v[i, j] = t;
                             }
                          }
                       }
                    }
                    break;

                    // Split at negligible s(k).

                    case 2:
                    {
                       Double f = e[k - 1];
                       e[k - 1] = 0;
                       for (int j = k; j < p; j++)
                       {
                          Double t = Tools.Hypotenuse(s[j], f);
                          Double cs = s[j] / t;
                          Double sn = f / t;
                          s[j] = t;
                          f = -sn * e[j];
                          e[j] = cs * e[j];
                          if (wantu) 
                          {
                                for (int i = 0; i < u.Rows(); i++) 
                                {
                                    t = cs * u[i, j] + sn * u[i, k-1];
                                    u[i, k - 1] = -sn * u[i, j] + cs * u[i, k-1];
                                    u[i, j] = t;
                                }
                          }
                       }
                    }
                    break;

                    // Perform one qr step.
                    case 3:
                        {
                           // Calculate the shift.
                           Double scale = Math.Max(Math.Max(Math.Max(Math.Max(
                                   Math.Abs(s[p-1]),Math.Abs(s[p-2])),Math.Abs(e[p-2])), 
                                   Math.Abs(s[k])),Math.Abs(e[k]));
                           Double sp = s[p-1] / scale;
                           Double spm1 = s[p-2] / scale;
                           Double epm1 = e[p-2] / scale;
                           Double sk = s[k] / scale;
                           Double ek = e[k] / scale;
                           Double b = ((spm1 + sp)*(spm1 - sp) + epm1*epm1)/2;
                           Double c = (sp*epm1)*(sp*epm1);
                           Double shift = 0;
                           if ((b != 0) || (c != 0))
                           {
                            if (b < 0)
                                shift = -(Double)System.Math.Sqrt(b * b + c);
                            else
                                shift = (Double)System.Math.Sqrt(b * b + c);
                              shift = c / (b + shift);
                           }

                           Double f = (sk + sp)*(sk - sp) + (Double)shift;
                           Double g = sk*ek;
   
                           // Chase zeros.
                           for (int j = k; j < p - 1; j++)
                           {
                              Double t = Tools.Hypotenuse(f, g);
                              Double cs = f / t;
                              Double sn = g / t;

                              if (j != k)
                                 e[j - 1] = t;

                              f = cs * s[j] + sn * e[j];
                              e[j] = cs * e[j] - sn * s[j];
                              g = sn * s[j + 1];
                              s[j+1] = cs * s[j + 1];

                              if (wantv)
                              {
                                 for (int i = 0; i < v.Rows(); i++)
                                 {
                                    t = cs * v[i, j] + sn * v[i, j + 1];
                                    v[i, j + 1] = -sn*v[i, j] + cs*v[i, j + 1];
                                    v[i, j] = t;
                                 }
                              }

                              t = Tools.Hypotenuse(f,g);
                              cs = f / t;
                              sn = g / t;
                              s[j] = t;
                              f = cs * e[j] + sn * s[j + 1];
                              s[j + 1] = -sn * e[j] + cs * s[j + 1];
                              g = sn * e[j + 1];
                              e[j + 1] = cs * e[j + 1];

                              if (wantu && (j < m - 1))
                              {
                                 for (int i = 0; i < u.Rows(); i++)
                                 {
                                    t = cs * u[i, j] + sn * u[i, j + 1];
                                    u[i, j + 1] = -sn * u[i, j] + cs * u[i, j + 1];
                                    u[i, j] = t;
                                 }
                              }
                           }

                           e[p - 2] = f;
                           iter = iter + 1;
                        }
                        break;

                    // Convergence.
                    case 4:
                        {
                            // Make the singular values positive.
                            if (s[k] <= 0)
                            {
                                s[k] = (s[k] < 0 ? -s[k] : (Double)0);

                                if (wantv)
                                {
                                    for (int i = 0; i <= pp; i++) 
                                        v[i, k] = -v[i, k];
                                }
                            }
   
                            // Order the singular values.
                            while (k < pp)
                            {
                                if (s[k] >= s[k + 1])
                                    break;

                                Double t = s[k];
                                s[k] = s[k + 1];
                                s[k+1] = t;
                                if (wantv && (k < n - 1))
                                {
                                    for (int i = 0; i < n; i++)
                                    {
                                        t = v[i, k + 1];
                                        v[i, k + 1] = v[i, k]; 
                                        v[i, k] = t;
                                    }
                                }

                                if (wantu && (k < m - 1))
                                {
                                    for (int i = 0; i < u.Rows(); i++)
                                    {
                                        t = u[i, k + 1]; 
                                        u[i, k + 1] = u[i, k]; 
                                        u[i, k] = t;
                                    }
                                }

                                k++;
                            }

                            iter = 0;
                            p--;
                        }
                        break;
                }
            }
            

            // If we are violating JAMA's assumption about 
            // the input dimension, we need to swap u and v.
            if (swapped)
            {
                var temp = this.u;
                this.u = this.v;
                this.v = temp;
            }
        }


        /// <summary>
        ///   Solves a linear equation system of the form AX = B.
        /// </summary>
        /// <param name="value">Parameter B from the equation AX = B.</param>
        /// <returns>The solution X from equation AX = B.</returns>
        public Double[,] Solve(Double[,] value)
        {
            // Additionally an important property is that if there does not exists a solution
            // when the matrix A is singular but replacing 1/Li with 0 will provide a solution
            // that minimizes the residue |AX -Y|. SVD finds the least squares best compromise
            // solution of the linear equation system. Interestingly SVD can be also used in an
            // over-determined system where the number of equations exceeds that of the parameters.

            // L is a diagonal matrix with non-negative matrix elements having the same
            // dimension as A, Wi ? 0. The diagonal elements of L are the singular values of matrix A.

            Double[,] Y = value;

            // Create L*, which is a diagonal matrix with elements
            //    L*[i] = 1/L[i]  if L[i] < e, else 0, 
            // where e is the so-called singularity threshold.

            // In other words, if L[i] is zero or close to zero (smaller than e),
            // one must replace 1/L[i] with 0. The value of e depends on the precision
            // of the hardware. This method can be used to solve linear equations
            // systems even if the matrices are singular or close to singular.

            //singularity threshold
            Double e = this.Threshold;


            int scols = s.Rows();
            var Ls = new Double[scols, scols];
            for (int i = 0; i < s.Rows(); i++)
            {
                if (System.Math.Abs(s[i]) <= e)
                    Ls[i, i] = 0;
                else Ls[i, i] = 1 / s[i];
            }

            //(V x L*) x Ut x Y
            var VL = Matrix.Dot(v, Ls);

            //(V x L* x Ut) x Y
            int vrows = v.Rows();
            int urows = u.Rows();
            int ucols = u.Columns();
            var VLU = new Double[vrows, urows];
            for (int i = 0; i < vrows; i++)
            {
                for (int j = 0; j < urows; j++)
                {
                    Double sum = 0;
                    for (int k = 0; k < ucols; k++)
                        sum += VL[i, k] * u[j, k];
                    VLU[i, j] = sum;
                }
            }

            //(V x L* x Ut x Y)
            return Matrix.Dot(VLU, Y);
        }

        /// <summary>
        ///   Solves a linear equation system of the form AX = B.
        /// </summary>
        /// <param name="value">Parameter B from the equation AX = B.</param>
        /// <returns>The solution X from equation AX = B.</returns>
        public Double[,] SolveTranspose(Double[,] value)
        {
            // Additionally an important property is that if there does not exists a solution
            // when the matrix A is singular but replacing 1/Li with 0 will provide a solution
            // that minimizes the residue |AX -Y|. SVD finds the least squares best compromise
            // solution of the linear equation system. Interestingly SVD can be also used in an
            // over-determined system where the number of equations exceeds that of the parameters.

            // L is a diagonal matrix with non-negative matrix elements having the same
            // dimension as A, Wi ? 0. The diagonal elements of L are the singular values of matrix A.

            Double[,] Y = value;

            // Create L*, which is a diagonal matrix with elements
            //    L*[i] = 1/L[i]  if L[i] < e, else 0, 
            // where e is the so-called singularity threshold.

            // In other words, if L[i] is zero or close to zero (smaller than e),
            // one must replace 1/L[i] with 0. The value of e depends on the precision
            // of the hardware. This method can be used to solve linear equations
            // systems even if the matrices are singular or close to singular.

            //singularity threshold
            Double e = this.Threshold;


            int scols = s.Rows();
            var Ls = new Double[scols, scols];
            for (int i = 0; i < s.Rows(); i++)
            {
                if (System.Math.Abs(s[i]) <= e)
                    Ls[i, i] = 0;
                else Ls[i, i] = 1 / s[i];
            }

            //(V x L*) x Ut x Y
            var VL = Matrix.Dot(v, Ls);

            //(V x L* x Ut) x Y
            int vrows = v.Rows();
            int urows = u.Rows();
            var VLU = new Double[vrows, scols];
            for (int i = 0; i < vrows; i++)
            {
                for (int j = 0; j < urows; j++)
                {
                    Double sum = 0;
                    for (int k = 0; k < urows; k++)
                        sum += VL[i, k] * u[j, k];
                    VLU[i, j] = sum;
                }
            }

            return Matrix.Dot(Y, VLU);
        }

        /// <summary>
        ///   Solves a linear equation system of the form AX = B.
        /// </summary>
        /// <param name="value">Parameter B from the equation AX = B.</param>
        /// <returns>The solution X from equation AX = B.</returns>
        public Double[,] SolveForDiagonal(Double[] value)
        {
            // Additionally an important property is that if there does not exists a solution
            // when the matrix A is singular but replacing 1/Li with 0 will provide a solution
            // that minimizes the residue |AX -Y|. SVD finds the least squares best compromise
            // solution of the linear equation system. Interestingly SVD can be also used in an
            // over-determined system where the number of equations exceeds that of the parameters.

            // L is a diagonal matrix with non-negative matrix elements having the same
            // dimension as A, Wi ? 0. The diagonal elements of L are the singular values of matrix A.

            Double[] Y = value;

            // Create L*, which is a diagonal matrix with elements
            //    L*[i] = 1/L[i]  if L[i] < e, else 0, 
            // where e is the so-called singularity threshold.

            // In other words, if L[i] is zero or close to zero (smaller than e),
            // one must replace 1/L[i] with 0. The value of e depends on the precision
            // of the hardware. This method can be used to solve linear equations
            // systems even if the matrices are singular or close to singular.

            //singularity threshold
            Double e = this.Threshold;


            int scols = s.Rows();
            var Ls = new Double[scols, scols];
            for (int i = 0; i < s.Rows(); i++)
            {
                if (System.Math.Abs(s[i]) <= e)
                    Ls[i, i] = 0;
                else Ls[i, i] = 1 / s[i];
            }

            //(V x L*) x Ut x Y
            Double[,] VL = Matrix.Dot(v, Ls);

            //(V x L* x Ut) x Y
            int vrows = v.Rows();
            int urows = u.Rows();
            var VLU = new Double[vrows, scols];
            for (int i = 0; i < vrows; i++)
            {
                for (int j = 0; j < urows; j++)
                {
                    Double sum = 0;
                    for (int k = 0; k < urows; k++)
                        sum += VL[i, k] * u[j, k];
                    VLU[i, j] = sum;
                }
            }

            //(V x L* x Ut x Y)
            return VLU.DotWithDiagonal(Y);
        }

        /// <summary>
        ///   Solves a linear equation system of the form xA = b.
        /// </summary>
        /// <param name="value">The b from the equation xA = b.</param>
        ///
        /// <returns>The x from equation Ax = b.</returns>
        ///
        public Double[] SolveTranspose(Double[] value)
        {
            // Additionally an important property is that if there does not exists a solution
            // when the matrix A is singular but replacing 1/Li with 0 will provide a solution
            // that minimizes the residue |AX -Y|. SVD finds the least squares best compromise
            // solution of the linear equation system. Interestingly SVD can be also used in an
            // over-determined system where the number of equations exceeds that of the parameters.

            // L is a diagonal matrix with non-negative matrix elements having the same
            // dimension as A, Wi ? 0. The diagonal elements of L are the singular values of matrix A.

            Double[] Y = value;

            // Create L*, which is a diagonal matrix with elements
            //    L*[i] = 1/L[i]  if L[i] < e, else 0, 
            // where e is the so-called singularity threshold.

            // In other words, if L[i] is zero or close to zero (smaller than e),
            // one must replace 1/L[i] with 0. The value of e depends on the precision
            // of the hardware. This method can be used to solve linear equations
            // systems even if the matrices are singular or close to singular.

            //singularity threshold
            Double e = this.Threshold;


            int scols = s.Rows();
            var Ls = new Double[scols, scols];
            for (int i = 0; i < s.Rows(); i++)
            {
                if (System.Math.Abs(s[i]) <= e)
                    Ls[i, i] = 0;
                else Ls[i, i] = 1 / s[i];
            }

            //(V x L*) x Ut x Y
            Double[,] VL = Matrix.Dot(v, Ls);

            //(V x L* x Ut) x Y
            int vrows = v.Rows();
            int urows = u.Rows();
            var VLU = new Double[vrows, scols];
            for (int i = 0; i < vrows; i++)
            {
                for (int j = 0; j < urows; j++)
                {
                    Double sum = 0;
                    for (int k = 0; k < urows; k++)
                        sum += VL[i, k] * u[j, k];
                    VLU[i, j] = sum;
                }
            }

            return Y.Dot(VLU);
        }

        /// <summary>
        ///   Solves a linear equation system of the form Ax = b.
        /// </summary>
        /// <param name="value">The b from the equation Ax = b.</param>
        /// <returns>The x from equation Ax = b.</returns>
        public Double[] Solve(Double[] value)
        {
            // Additionally an important property is that if there does not exists a solution
            // when the matrix A is singular but replacing 1/Li with 0 will provide a solution
            // that minimizes the residue |AX -Y|. SVD finds the least squares best compromise
            // solution of the linear equation system. Interestingly SVD can be also used in an
            // over-determined system where the number of equations exceeds that of the parameters.

            // L is a diagonal matrix with non-negative matrix elements having the same
            // dimension as A, Wi ? 0. The diagonal elements of L are the singular values of matrix A.

            //singularity threshold
            Double e = this.Threshold;

            var Y = value;

            // Create L*, which is a diagonal matrix with elements
            //    L*i = 1/Li  if Li = e, else 0, 
            // where e is the so-called singularity threshold.

            // In other words, if Li is zero or close to zero (smaller than e),
            // one must replace 1/Li with 0. The value of e depends on the precision
            // of the hardware. This method can be used to solve linear equations
            // systems even if the matrices are singular or close to singular.


            int scols = s.Rows();

            var Ls = new Double[scols, scols];
            for (int i = 0; i < s.Rows(); i++)
            {
                if (System.Math.Abs(s[i]) <= e)
                    Ls[i, i] = 0;
                else Ls[i, i] = 1 / s[i];
            }

            //(V x L*) x Ut x Y
            var VL = Matrix.Dot(v, Ls);

            //(V x L* x Ut) x Y
            int urows = u.Rows();
            int vrows = v.Rows();
            var VLU = new Double[vrows, urows];
            for (int i = 0; i < vrows; i++)
            {
                for (int j = 0; j < urows; j++)
                {
                    Double sum = 0;
                    for (int k = 0; k < scols; k++)
                        sum += VL[i, k] * u[j, k];
                    VLU[i, j] = sum;
                }
            }

            //(V x L* x Ut x Y)
            return Matrix.Dot(VLU, Y);
        }

        /// <summary>
        ///   Computes the (pseudo-)inverse of the matrix given to the Singular value decomposition.
        /// </summary>
        ///
        public Double[,] Inverse()
        {
            Double e = this.Threshold;

            // X = V*S^-1
            int vrows = v.Rows();
            int vcols = v.Columns();
            var X = new Double[vrows, s.Rows()];
            for (int i = 0; i < vrows; i++)
            {
                for (int j = 0; j < vcols; j++)
                {
                    if (System.Math.Abs(s[j]) > e)
                        X[i, j] = v[i, j] / s[j];
                }
            }

            // Y = X*U'
            int urows = u.Rows();
            int ucols = u.Columns();
            var Y = new Double[vrows, urows];
            for (int i = 0; i < vrows; i++)
            {
                for (int j = 0; j < urows; j++)
                {
                    Double sum = 0;
                    for (int k = 0; k < ucols; k++)
                        sum += X[i, k] * u[j, k];
                    Y[i, j] = sum;
                }
            }

            return Y;
        }

        /// <summary>
        ///   Reverses the decomposition, reconstructing the original matrix <c>X</c>.
        /// </summary>
        /// 
        public Double[,] Reverse()
        {
            return LeftSingularVectors.Dot(DiagonalMatrix).DotWithTransposed(RightSingularVectors);
        }

        /// <summary>
        ///   Computes <c>(Xt * X)^1</c> (the inverse of the covariance matrix). This
        ///   matrix can be used to determine standard errors for the coefficients when
        ///   solving a linear set of equations through any of the <see cref="Solve(Double[,])"/>
        ///   methods.
        /// </summary>
        /// 
        public Double[,] GetInformationMatrix()
        {
            Double e = this.Threshold;

            // X = V*S^-1
            int vrows = v.Rows();
            int vcols = v.Columns();
            var X = new Double[vrows, s.Rows()];
            for (int i = 0; i < vrows; i++)
            {
                for (int j = 0; j < vcols; j++)
                {
                    if (System.Math.Abs(s[j]) > e)
                        X[i, j] = v[i, j] / s[j];
                }
            }

            // Y = X*V'
            var Y = new Double[vrows, vrows];
            for (int i = 0; i < vrows; i++)
            {
                for (int j = 0; j < vrows; j++)
                {
                    Double sum = 0;
                    for (int k = 0; k < vrows; k++)
                        sum += X[i, k] * v[j, k];
                    Y[i, j] = sum;
                }
            }

            return Y;
        }

        #region ICloneable Members
        private SingularValueDecomposition()
        {
        }

        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        ///
        public object Clone()
        {
            var svd = new SingularValueDecomposition();
            svd.m = m;
            svd.n = n;
            svd.s = (Double[])s.Clone();
            svd.si = (int[])si.Clone();
            svd.swapped = swapped;
            if (u != null) svd.u = (Double[,])u.MemberwiseClone();
            if (v != null) svd.v = (Double[,])v.MemberwiseClone();

            return svd;
        }

        #endregion

    }
}

