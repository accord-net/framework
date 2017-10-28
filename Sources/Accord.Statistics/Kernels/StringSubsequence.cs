// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
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

namespace Accord.Statistics.Kernels
{
    using System;
    using Accord.Compat;

    /// <summary>
    ///   String Subsequence Kernel (SSK).
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This class implements the simplest, recursive version of the kernel as described
    ///   in the original paper (see below). Contributions for improving the efficiency of
    ///   this class are welcome.</para>
    /// 
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://www.jmlr.org/papers/volume2/lodhi02a/lodhi02a.pdf">
    ///       Lodhi, H., Saunders, C., Shawe-Taylor, J., Cristianini, N., and Watkins, C. (2002). 
    ///       Text classification using string kernels. Journal of Machine Learning Research, 
    ///       2(Feb), 419-444. Available on: http://www.jmlr.org/papers/volume2/lodhi02a/lodhi02a.pdf </a>
    ///       </description></item></list></para>
    /// </remarks>
    /// 
    /// <seealso cref="Gaussian"/>
    /// <seealso cref="Gaussian{TKernel}"/>
    /// 
    [Serializable]
    public sealed class StringSubsequence : KernelBase<string>, IKernel<string>, ICloneable
    {
        int k;
        double lambda;
        double normalizationPower;

        /// <summary>
        ///   Constructs a new String Kernel.
        /// </summary>
        /// 
        public StringSubsequence(int k = 3, double lambda = 0.5, double normalizationPower = 0.5)
        {
            this.k = k;
            this.lambda = lambda;
            this.normalizationPower = normalizationPower;
        }

        /// <summary>
        ///   Spherical Kernel Function
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// 
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public override double Function(string x, string y)
        {
            double xy = K(this.k, x, x.Length, y, y.Length);

            if (normalizationPower > 0)
            {
                double xx = K(this.k, x, x.Length, x, x.Length);
                double yy = K(this.k, y, y.Length, y, y.Length);
                return xy / Math.Pow(xx + yy, normalizationPower);
            }

            return xy;
        }


        private double K(int n, string s, int slen, string t, int tlen)
        {
            // k(i, s, t) = 0 if min(|s|, |t|) < i
            if (slen < n || tlen < n)
                return 0;

            // k(i, sx, t)
            char x = s[slen - 1];

            double sum = K(n, s, slen - 1, t, tlen);
            for (int j = 0; j < tlen; j++)
                if (t[j] == x)
                    sum += Kp(n - 1, s, slen - 1, t, tlen - j - 1) * (lambda * lambda);

            return sum;
        }

        private double Kp(int i, string s, int slen, string t, int tlen)
        {
            // k'(0, s, t) = 1 for all s, t
            if (i == 0)
                return 1;

            // k'(i, s, t) = 0 if min(|s|, |t|) < i
            if (slen < i || tlen < i)
                return 0;

            // k'(i, sx, t)
            char x = s[slen - 1];

            double sum = lambda * Kp(i, s, slen - 1, t, tlen);
            for (int j = 0; j < tlen; j++)
                if (t[j] == x)
                    sum += Kp(i - 1, s, slen - 1, t, tlen - j - 1) * Math.Pow(lambda, tlen - (j + 1) + 2);

            return sum;
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
    }
}
