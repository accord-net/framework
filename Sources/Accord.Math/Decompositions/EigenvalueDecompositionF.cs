// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
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

    /// <summary>
    ///     Determines the Eigenvalues and eigenvectors of a real square matrix.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     In the mathematical discipline of linear algebra, Eigendecomposition
    ///     or sometimes spectral decomposition is the factorization of a matrix
    ///     into a canonical form, whereby the matrix is represented in terms of
    ///     its Eigenvalues and eigenvectors.</para>
    ///   <para>
    ///     If <c>A</c> is symmetric, then <c>A = V * D * V'</c> and <c>A = V * V'</c>
    ///     where the eigenvalue matrix <c>D</c> is diagonal and the eigenvector matrix <c>V</c> is orthogonal.
    ///     If <c>A</c> is not symmetric, the eigenvalue matrix <c>D</c> is block diagonal
    ///     with the real eigenvalues in 1-by-1 blocks and any complex eigenvalues,
    ///     <c>lambda + i*mu</c>, in 2-by-2 blocks, <c>[lambda, mu; -mu, lambda]</c>.
    ///     The columns of <c>V</c> represent the eigenvectors in the sense that <c>A * V = V * D</c>.
    ///     The matrix V may be badly conditioned, or even singular, so the validity of the equation
    ///     <c>A = V * D * inverse(V)</c> depends upon the condition of <c>V</c>.
    ///   </para>
    /// </remarks>
    /// 
    public sealed class EigenvalueDecompositionF : ICloneable
    {
        private int n;              // matrix dimension
        private Single[] d, e;      // storage of eigenvalues.
        private Single[,] V;        // storage of eigenvectors.
        private Single[,] H;        // storage of nonsymmetric Hessenberg form.
        private Single[] ort;       // storage for nonsymmetric algorithm.
        private bool symmetric;

        /// <summary>
        ///   Construct an eigenvalue decomposition.</summary>
        /// <param name="value">
        ///   The matrix to be decomposed.</param>
        public EigenvalueDecompositionF(Single[,] value)
            : this(value, value.IsSymmetric(), false)
        {
        }

        /// <summary>
        ///   Construct an eigenvalue decomposition.</summary>
        /// <param name="value">
        ///   The matrix to be decomposed.</param>
        /// <param name="assumeSymmetric">
        ///   Defines if the matrix should be assumed as being symmetric
        ///   regardless if it is or not. Default is <see langword="false"/>.</param>
        public EigenvalueDecompositionF(Single[,] value, bool assumeSymmetric)
            : this(value, assumeSymmetric, false)
        {
        }

        /// <summary>
        ///   Construct an eigenvalue decomposition.</summary>
        /// <param name="value">
        ///   The matrix to be decomposed.</param>
        /// <param name="assumeSymmetric">
        ///   Defines if the matrix should be assumed as being symmetric
        ///   regardless if it is or not. Default is <see langword="false"/>.</param>
        /// <param name="inPlace">
        ///   Pass <see langword="true"/> to perform the decomposition in place. The matrix
        ///   <paramref name="value"/> will be destroyed in the process, resulting in less
        ///   memory consumption.</param>
        public EigenvalueDecompositionF(Single[,] value, bool assumeSymmetric, bool inPlace)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value", "Matrix cannot be null.");
            }

            if (value.GetLength(0) != value.GetLength(1))
            {
                throw new ArgumentException("Matrix is not a square matrix.", "value");
            }

            n = value.GetLength(1);
            V = new Single[n, n];
            d = new Single[n];
            e = new Single[n];


            this.symmetric = assumeSymmetric;

            if (this.symmetric)
            {
                V = inPlace ? value : (Single[,])value.Clone();

                // Tridiagonalize.
                this.tred2();

                // Diagonalize.
                this.tql2();
            }
            else
            {
                H = inPlace ? value : (Single[,])value.Clone();

                ort = new Single[n];

                // Reduce to Hessenberg form.
                this.orthes();

                // Reduce Hessenberg to real Schur form.
                this.hqr2();
            }
        }


        /// <summary>Returns the real parts of the eigenvalues.</summary>
        public Single[] RealEigenvalues
        {
            get { return this.d; }
        }

        /// <summary>Returns the imaginary parts of the eigenvalues.</summary>    
        public Single[] ImaginaryEigenvalues
        {
            get { return this.e; }
        }

        /// <summary>Returns the eigenvector matrix.</summary>
        public Single[,] Eigenvectors
        {
            get { return this.V; }
        }

        /// <summary>Returns the block diagonal eigenvalue matrix.</summary>
        public Single[,] DiagonalMatrix
        {
            get
            {
                var x = new Single[n, n];

                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                        x[i, j] = 0;

                    x[i, i] = d[i];
                    if (e[i] > 0)
                    {
                        x[i, i + 1] = e[i];
                    }
                    else if (e[i] < 0)
                    {
                        x[i, i - 1] = e[i];
                    }
                }

                return x;
            }
        }


        #region Private methods
        private void tred2()
        {
            // Symmetric Householder reduction to tridiagonal form.
            // This is derived from the Algol procedures tred2 by Bowdler, Martin, Reinsch, and Wilkinson, 
            // Handbook for Auto. Comp., Vol.ii-Linear Algebra, and the corresponding Fortran subroutine in EISPACK.
            for (int j = 0; j < n; j++)
                d[j] = V[n - 1, j];

            // Householder reduction to tridiagonal form.
            for (int i = n - 1; i > 0; i--)
            {
                // Scale to avoid under/overflow.
                Single scale = 0;
                Single h = 0;
                for (int k = 0; k < i; k++)
                    scale = scale + System.Math.Abs(d[k]);

                if (scale == 0)
                {
                    e[i] = d[i - 1];
                    for (int j = 0; j < i; j++)
                    {
                        d[j] = V[i - 1, j];
                        V[i, j] = 0;
                        V[j, i] = 0;
                    }
                }
                else
                {
                    // Generate Householder vector.
                    for (int k = 0; k < i; k++)
                    {
                        d[k] /= scale;
                        h += d[k] * d[k];
                    }

                    Single f = d[i - 1];
                    Single g = (Single)System.Math.Sqrt(h);
                    if (f > 0) g = -g;

                    e[i] = scale * g;
                    h = h - f * g;
                    d[i - 1] = f - g;
                    for (int j = 0; j < i; j++)
                        e[j] = 0;

                    // Apply similarity transformation to remaining columns.
                    for (int j = 0; j < i; j++)
                    {
                        f = d[j];
                        V[j, i] = f;
                        g = e[j] + V[j, j] * f;
                        for (int k = j + 1; k <= i - 1; k++)
                        {
                            g += V[k, j] * d[k];
                            e[k] += V[k, j] * f;
                        }
                        e[j] = g;
                    }

                    f = 0;
                    for (int j = 0; j < i; j++)
                    {
                        e[j] /= h;
                        f += e[j] * d[j];
                    }

                    Single hh = f / (h + h);
                    for (int j = 0; j < i; j++)
                        e[j] -= hh * d[j];

                    for (int j = 0; j < i; j++)
                    {
                        f = d[j];
                        g = e[j];
                        for (int k = j; k <= i - 1; k++)
                            V[k, j] -= (f * e[k] + g * d[k]);

                        d[j] = V[i - 1, j];
                        V[i, j] = 0;
                    }
                }
                d[i] = h;
            }

            // Accumulate transformations.
            for (int i = 0; i < n - 1; i++)
            {
                V[n - 1, i] = V[i, i];
                V[i, i] = 1;
                Single h = d[i + 1];
                if (h != 0)
                {
                    for (int k = 0; k <= i; k++)
                        d[k] = V[k, i + 1] / h;

                    for (int j = 0; j <= i; j++)
                    {
                        Single g = 0;
                        for (int k = 0; k <= i; k++)
                            g += V[k, i + 1] * V[k, j];
                        for (int k = 0; k <= i; k++)
                            V[k, j] -= g * d[k];
                    }
                }

                for (int k = 0; k <= i; k++)
                    V[k, i + 1] = 0;
            }

            for (int j = 0; j < n; j++)
            {
                d[j] = V[n - 1, j];
                V[n - 1, j] = 0;
            }

            V[n - 1, n - 1] = 1;
            e[0] = 0;
        }

        private void tql2()
        {
            // Symmetric tridiagonal QL algorithm.
            // This is derived from the Algol procedures tql2, by Bowdler, Martin, Reinsch, and Wilkinson, 
            // Handbook for Auto. Comp., Vol.ii-Linear Algebra, and the corresponding Fortran subroutine in EISPACK.
            for (int i = 1; i < n; i++)
                e[i - 1] = e[i];

            e[n - 1] = 0;

            Single f = 0;
            Single tst1 = 0;
            Single eps = 2 * Constants.SingleEpsilon;

            for (int l = 0; l < n; l++)
            {
                // Find small subdiagonal element.
                tst1 = System.Math.Max(tst1, System.Math.Abs(d[l]) + System.Math.Abs(e[l]));
                int m = l;
                while (m < n)
                {
                    if (System.Math.Abs(e[m]) <= eps * tst1)
                        break;
                    m++;
                }

                // If m == l, d[l] is an eigenvalue, otherwise, iterate.
                if (m > l)
                {
                    int iter = 0;
                    do
                    {
                        iter = iter + 1;  // (Could check iteration count here.)

                        // Compute implicit shift
                        Single g = d[l];
                        Single p = (d[l + 1] - g) / (2 * e[l]);
                        Single r = Accord.Math.Tools.Hypotenuse(p, 1);
                        if (p < 0)
                        {
                            r = -r;
                        }

                        d[l] = e[l] / (p + r);
                        d[l + 1] = e[l] * (p + r);
                        Single dl1 = d[l + 1];
                        Single h = g - d[l];
                        for (int i = l + 2; i < n; i++)
                        {
                            d[i] -= h;
                        }

                        f = f + h;

                        // Implicit QL transformation.
                        p = d[m];
                        Single c = 1;
                        Single c2 = c;
                        Single c3 = c;
                        Single el1 = e[l + 1];
                        Single s = 0;
                        Single s2 = 0;
                        for (int i = m - 1; i >= l; i--)
                        {
                            c3 = c2;
                            c2 = c;
                            s2 = s;
                            g = c * e[i];
                            h = c * p;
                            r = Accord.Math.Tools.Hypotenuse(p, e[i]);
                            e[i + 1] = s * r;
                            s = e[i] / r;
                            c = p / r;
                            p = c * d[i] - s * g;
                            d[i + 1] = h + s * (c * g + s * d[i]);

                            // Accumulate transformation.
                            for (int k = 0; k < n; k++)
                            {
                                h = V[k, i + 1];
                                V[k, i + 1] = s * V[k, i] + c * h;
                                V[k, i] = c * V[k, i] - s * h;
                            }
                        }

                        p = -s * s2 * c3 * el1 * e[l] / dl1;
                        e[l] = s * p;
                        d[l] = c * p;

                        // Check for convergence.
                    }
                    while (System.Math.Abs(e[l]) > eps * tst1);
                }
                d[l] = d[l] + f;
                e[l] = 0;
            }

            // Sort eigenvalues and corresponding vectors.
            for (int i = 0; i < n - 1; i++)
            {
                int k = i;
                Single p = d[i];
                for (int j = i + 1; j < n; j++)
                {
                    if (d[j] < p)
                    {
                        k = j;
                        p = d[j];
                    }
                }

                if (k != i)
                {
                    d[k] = d[i];
                    d[i] = p;
                    for (int j = 0; j < n; j++)
                    {
                        p = V[j, i];
                        V[j, i] = V[j, k];
                        V[j, k] = p;
                    }
                }
            }
        }

        private void orthes()
        {
            // Nonsymmetric reduction to Hessenberg form.
            // This is derived from the Algol procedures orthes and ortran, by Martin and Wilkinson, 
            // Handbook for Auto. Comp., Vol.ii-Linear Algebra, and the corresponding Fortran subroutines in EISPACK.
            int low = 0;
            int high = n - 1;

            for (int m = low + 1; m <= high - 1; m++)
            {
                // Scale column.

                Single scale = 0;
                for (int i = m; i <= high; i++)
                    scale = scale + System.Math.Abs(H[i, m - 1]);

                if (scale != 0)
                {
                    // Compute Householder transformation.
                    Single h = 0;
                    for (int i = high; i >= m; i--)
                    {
                        ort[i] = H[i, m - 1] / scale;
                        h += ort[i] * ort[i];
                    }

                    Single g = (Single)System.Math.Sqrt(h);
                    if (ort[m] > 0) g = -g;

                    h = h - ort[m] * g;
                    ort[m] = ort[m] - g;

                    // Apply Householder similarity transformation
                    // H = (I - u * u' / h) * H * (I - u * u') / h)
                    for (int j = m; j < n; j++)
                    {
                        Single f = 0;
                        for (int i = high; i >= m; i--)
                            f += ort[i] * H[i, j];

                        f = f / h;
                        for (int i = m; i <= high; i++)
                            H[i, j] -= f * ort[i];
                    }

                    for (int i = 0; i <= high; i++)
                    {
                        Single f = 0;
                        for (int j = high; j >= m; j--)
                            f += ort[j] * H[i, j];

                        f = f / h;
                        for (int j = m; j <= high; j++)
                            H[i, j] -= f * ort[j];
                    }

                    ort[m] = scale * ort[m];
                    H[m, m - 1] = scale * g;
                }
            }

            // Accumulate transformations (Algol's ortran).
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    V[i, j] = (i == j ? 1 : 0);

            for (int m = high - 1; m >= low + 1; m--)
            {
                if (H[m, m - 1] != 0)
                {
                    for (int i = m + 1; i <= high; i++)
                        ort[i] = H[i, m - 1];

                    for (int j = m; j <= high; j++)
                    {
                        Single g = 0;
                        for (int i = m; i <= high; i++)
                            g += ort[i] * V[i, j];

                        // Double division avoids possible underflow.
                        g = (g / ort[m]) / H[m, m - 1];
                        for (int i = m; i <= high; i++)
                            V[i, j] += g * ort[i];
                    }
                }
            }
        }

        private static void cdiv(Single xr, Single xi, Single yr, Single yi,
            out Single cdivr, out Single cdivi)
        {
            // Complex scalar division.
            Single r;
            Single d;
            if (System.Math.Abs(yr) > System.Math.Abs(yi))
            {
                r = yi / yr;
                d = yr + r * yi;
                cdivr = (xr + r * xi) / d;
                cdivi = (xi - r * xr) / d;
            }
            else
            {
                r = yr / yi;
                d = yi + r * yr;
                cdivr = (r * xr + xi) / d;
                cdivi = (r * xi - xr) / d;
            }
        }

        private void hqr2()
        {
            // Nonsymmetric reduction from Hessenberg to real Schur form.   
            // This is derived from the Algol procedure hqr2, by Martin and Wilkinson, Handbook for Auto. Comp.,
            // Vol.ii-Linear Algebra, and the corresponding  Fortran subroutine in EISPACK.
            int nn = this.n;
            int n = nn - 1;
            int low = 0;
            int high = nn - 1;
            Single eps = 2 * Constants.SingleEpsilon;
            Single exshift = 0;
            Single p = 0;
            Single q = 0;
            Single r = 0;
            Single s = 0;
            Single z = 0;
            Single t;
            Single w;
            Single x;
            Single y;

            // Store roots isolated by balanc and compute matrix norm
            Single norm = 0;
            for (int i = 0; i < nn; i++)
            {
                if (i < low | i > high)
                {
                    d[i] = H[i, i];
                    e[i] = 0;
                }

                for (int j = System.Math.Max(i - 1, 0); j < nn; j++)
                    norm = norm + System.Math.Abs(H[i, j]);
            }

            // Outer loop over eigenvalue index
            int iter = 0;
            while (n >= low)
            {
                // Look for single small sub-diagonal element
                int l = n;
                while (l > low)
                {
                    s = System.Math.Abs(H[l - 1, l - 1]) + System.Math.Abs(H[l, l]);
                    if (s == 0) s = norm;
                    if (System.Math.Abs(H[l, l - 1]) < eps * s)
                        break;

                    l--;
                }

                // Check for convergence
                if (l == n)
                {
                    // One root found
                    H[n, n] = H[n, n] + exshift;
                    d[n] = H[n, n];
                    e[n] = 0;
                    n--;
                    iter = 0;
                }
                else if (l == n - 1)
                {
                    // Two roots found
                    w = H[n, n - 1] * H[n - 1, n];
                    p = (H[n - 1, n - 1] - H[n, n]) / 2;
                    q = p * p + w;
                    z = (Single)System.Math.Sqrt(System.Math.Abs(q));
                    H[n, n] = H[n, n] + exshift;
                    H[n - 1, n - 1] = H[n - 1, n - 1] + exshift;
                    x = H[n, n];

                    if (q >= 0)
                    {
                        // Real pair
                        z = (p >= 0) ? (p + z) : (p - z);
                        d[n - 1] = x + z;
                        d[n] = d[n - 1];
                        if (z != 0)
                            d[n] = x - w / z;
                        e[n - 1] = 0;
                        e[n] = 0;
                        x = H[n, n - 1];
                        s = System.Math.Abs(x) + System.Math.Abs(z);
                        p = x / s;
                        q = z / s;
                        r = (Single)System.Math.Sqrt(p * p + q * q);
                        p = p / r;
                        q = q / r;

                        // Row modification
                        for (int j = n - 1; j < nn; j++)
                        {
                            z = H[n - 1, j];
                            H[n - 1, j] = q * z + p * H[n, j];
                            H[n, j] = q * H[n, j] - p * z;
                        }

                        // Column modification
                        for (int i = 0; i <= n; i++)
                        {
                            z = H[i, n - 1];
                            H[i, n - 1] = q * z + p * H[i, n];
                            H[i, n] = q * H[i, n] - p * z;
                        }

                        // Accumulate transformations
                        for (int i = low; i <= high; i++)
                        {
                            z = V[i, n - 1];
                            V[i, n - 1] = q * z + p * V[i, n];
                            V[i, n] = q * V[i, n] - p * z;
                        }
                    }
                    else
                    {
                        // Complex pair
                        d[n - 1] = x + p;
                        d[n] = x + p;
                        e[n - 1] = z;
                        e[n] = -z;
                    }

                    n = n - 2;
                    iter = 0;
                }
                else
                {
                    // No convergence yet     

                    // Form shift
                    x = H[n, n];
                    y = 0;
                    w = 0;
                    if (l < n)
                    {
                        y = H[n - 1, n - 1];
                        w = H[n, n - 1] * H[n - 1, n];
                    }

                    // Wilkinson's original ad hoc shift
                    if (iter == 10)
                    {
                        exshift += x;
                        for (int i = low; i <= n; i++)
                            H[i, i] -= x;

                        s = System.Math.Abs(H[n, n - 1]) + System.Math.Abs(H[n - 1, n - 2]);
                        x = y = (Single)0.75 * s;
                        w = (Single)(-0.4375) * s * s;
                    }

                    // MATLAB's new ad hoc shift
                    if (iter == 30)
                    {
                        s = (y - x) / 2;
                        s = s * s + w;
                        if (s > 0)
                        {
                            s = (Single)System.Math.Sqrt(s);
                            if (y < x) s = -s;
                            s = x - w / ((y - x) / 2 + s);
                            for (int i = low; i <= n; i++)
                                H[i, i] -= s;
                            exshift += s;
                            x = y = w = (Single)0.964;
                        }
                    }

                    iter = iter + 1;

                    // Look for two consecutive small sub-diagonal elements
                    int m = n - 2;
                    while (m >= l)
                    {
                        z = H[m, m];
                        r = x - z;
                        s = y - z;
                        p = (r * s - w) / H[m + 1, m] + H[m, m + 1];
                        q = H[m + 1, m + 1] - z - r - s;
                        r = H[m + 2, m + 1];
                        s = System.Math.Abs(p) + System.Math.Abs(q) + System.Math.Abs(r);
                        p = p / s;
                        q = q / s;
                        r = r / s;
                        if (m == l)
                            break;
                        if (System.Math.Abs(H[m, m - 1]) * (System.Math.Abs(q) + System.Math.Abs(r)) < eps * (System.Math.Abs(p) * (System.Math.Abs(H[m - 1, m - 1]) + System.Math.Abs(z) + System.Math.Abs(H[m + 1, m + 1]))))
                            break;
                        m--;
                    }

                    for (int i = m + 2; i <= n; i++)
                    {
                        H[i, i - 2] = 0;
                        if (i > m + 2)
                            H[i, i - 3] = 0;
                    }

                    // Double QR step involving rows l:n and columns m:n
                    for (int k = m; k <= n - 1; k++)
                    {
                        bool notlast = (k != n - 1);
                        if (k != m)
                        {
                            p = H[k, k - 1];
                            q = H[k + 1, k - 1];
                            r = (notlast ? H[k + 2, k - 1] : 0);
                            x = System.Math.Abs(p) + System.Math.Abs(q) + System.Math.Abs(r);
                            if (x != 0)
                            {
                                p = p / x;
                                q = q / x;
                                r = r / x;
                            }
                        }

                        if (x == 0) break;

                        s = (Single)System.Math.Sqrt(p * p + q * q + r * r);
                        if (p < 0) s = -s;

                        if (s != 0)
                        {
                            if (k != m)
                                H[k, k - 1] = -s * x;
                            else
                                if (l != m)
                                    H[k, k - 1] = -H[k, k - 1];

                            p = p + s;
                            x = p / s;
                            y = q / s;
                            z = r / s;
                            q = q / p;
                            r = r / p;

                            // Row modification
                            for (int j = k; j < nn; j++)
                            {
                                p = H[k, j] + q * H[k + 1, j];
                                if (notlast)
                                {
                                    p = p + r * H[k + 2, j];
                                    H[k + 2, j] = H[k + 2, j] - p * z;
                                }

                                H[k, j] = H[k, j] - p * x;
                                H[k + 1, j] = H[k + 1, j] - p * y;
                            }

                            // Column modification
                            for (int i = 0; i <= System.Math.Min(n, k + 3); i++)
                            {
                                p = x * H[i, k] + y * H[i, k + 1];
                                if (notlast)
                                {
                                    p = p + z * H[i, k + 2];
                                    H[i, k + 2] = H[i, k + 2] - p * r;
                                }

                                H[i, k] = H[i, k] - p;
                                H[i, k + 1] = H[i, k + 1] - p * q;
                            }

                            // Accumulate transformations
                            for (int i = low; i <= high; i++)
                            {
                                p = x * V[i, k] + y * V[i, k + 1];
                                if (notlast)
                                {
                                    p = p + z * V[i, k + 2];
                                    V[i, k + 2] = V[i, k + 2] - p * r;
                                }

                                V[i, k] = V[i, k] - p;
                                V[i, k + 1] = V[i, k + 1] - p * q;
                            }
                        }
                    }
                }
            }

            // Backsubstitute to find vectors of upper triangular form
            if (norm == 0)
            {
                return;
            }

            for (n = nn - 1; n >= 0; n--)
            {
                p = d[n];
                q = e[n];

                // Real vector
                if (q == 0)
                {
                    int l = n;
                    H[n, n] = 1;
                    for (int i = n - 1; i >= 0; i--)
                    {
                        w = H[i, i] - p;
                        r = 0;
                        for (int j = l; j <= n; j++)
                            r = r + H[i, j] * H[j, n];

                        if (e[i] < 0)
                        {
                            z = w;
                            s = r;
                        }
                        else
                        {
                            l = i;
                            if (e[i] == 0)
                            {
                                H[i, n] = (w != 0) ? (-r / w) : (-r / (eps * norm));
                            }
                            else
                            {
                                // Solve real equations
                                x = H[i, i + 1];
                                y = H[i + 1, i];
                                q = (d[i] - p) * (d[i] - p) + e[i] * e[i];
                                t = (x * s - z * r) / q;
                                H[i, n] = t;
                                H[i + 1, n] = (System.Math.Abs(x) > System.Math.Abs(z)) ? ((-r - w * t) / x) : ((-s - y * t) / z);
                            }

                            // Overflow control
                            t = System.Math.Abs(H[i, n]);
                            if ((eps * t) * t > 1)
                                for (int j = i; j <= n; j++)
                                    H[j, n] = H[j, n] / t;
                        }
                    }
                }
                else if (q < 0)
                {
                    // Complex vector
                    int l = n - 1;

                    // Last vector component imaginary so matrix is triangular
                    if (System.Math.Abs(H[n, n - 1]) > System.Math.Abs(H[n - 1, n]))
                    {
                        H[n - 1, n - 1] = q / H[n, n - 1];
                        H[n - 1, n] = -(H[n, n] - p) / H[n, n - 1];
                    }
                    else
                    {
                        cdiv(0, -H[n - 1, n], H[n - 1, n - 1] - p, q, out H[n - 1, n - 1], out H[n - 1, n]);
                    }

                    H[n, n - 1] = 0;
                    H[n, n] = 1;
                    for (int i = n - 2; i >= 0; i--)
                    {
                        Single ra, sa, vr, vi;
                        ra = 0;
                        sa = 0;
                        for (int j = l; j <= n; j++)
                        {
                            ra = ra + H[i, j] * H[j, n - 1];
                            sa = sa + H[i, j] * H[j, n];
                        }

                        w = H[i, i] - p;

                        if (e[i] < 0)
                        {
                            z = w;
                            r = ra;
                            s = sa;
                        }
                        else
                        {
                            l = i;
                            if (e[i] == 0)
                            {
                                cdiv(-ra, -sa, w, q, out H[i, n - 1], out H[i, n]);
                            }
                            else
                            {
                                // Solve complex equations
                                x = H[i, i + 1];
                                y = H[i + 1, i];
                                vr = (d[i] - p) * (d[i] - p) + e[i] * e[i] - q * q;
                                vi = (d[i] - p) * 2 * q;
                                if (vr == 0 & vi == 0)
                                    vr = eps * norm * (System.Math.Abs(w) + System.Math.Abs(q) + System.Math.Abs(x) + System.Math.Abs(y) + System.Math.Abs(z));
                                cdiv(x * r - z * ra + q * sa, x * s - z * sa - q * ra, vr, vi, out H[i, n - 1], out H[i, n]);
                                if (System.Math.Abs(x) > (System.Math.Abs(z) + System.Math.Abs(q)))
                                {
                                    H[i + 1, n - 1] = (-ra - w * H[i, n - 1] + q * H[i, n]) / x;
                                    H[i + 1, n] = (-sa - w * H[i, n] - q * H[i, n - 1]) / x;
                                }
                                else
                                {
                                    cdiv(-r - y * H[i, n - 1], -s - y * H[i, n], z, q, out H[i + 1, n - 1], out H[i + 1, n]);
                                }
                            }

                            // Overflow control
                            t = System.Math.Max(System.Math.Abs(H[i, n - 1]), System.Math.Abs(H[i, n]));
                            if ((eps * t) * t > 1)
                            {
                                for (int j = i; j <= n; j++)
                                {
                                    H[j, n - 1] = H[j, n - 1] / t;
                                    H[j, n] = H[j, n] / t;
                                }
                            }
                        }
                    }
                }
            }

            // Vectors of isolated roots
            for (int i = 0; i < nn; i++)
                if (i < low | i > high)
                    for (int j = i; j < nn; j++)
                        V[i, j] = H[i, j];

            // Back transformation to get eigenvectors of original matrix
            for (int j = nn - 1; j >= low; j--)
            {
                for (int i = low; i <= high; i++)
                {
                    z = 0;
                    for (int k = low; k <= System.Math.Min(j, high); k++)
                        z = z + V[i, k] * H[k, j];
                    V[i, j] = z;
                }
            }
        }
        #endregion



        #region ICloneable Members

        private EigenvalueDecompositionF()
        {
        }

        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            var clone = new EigenvalueDecompositionF();
            clone.d = (Single[])d.Clone();
            clone.e = (Single[])e.Clone();
            clone.H = (Single[,])H.Clone();
            clone.n = n;
            clone.ort = (Single[])ort;
            clone.symmetric = symmetric;
            clone.V = (Single[,])V.Clone();
            return clone;
        }

        #endregion

    }
}

