// Accord Statistics Library
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

namespace Accord.Statistics.Kernels
{
    using System;

    /// <summary>
    ///   Wavelet Kernel.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In Wavelet analysis theory, one of the common goals is to express or
    ///   approximate a signal or function using a family of functions generated
    ///   by dilations and translations of a function called the mother wavelet.</para>
    /// <para>
    ///   The Wavelet kernel uses a mother wavelet function together with dilation
    ///   and translation constants to produce such representations and build a
    ///   inner product which can be used by kernel methods. The default wavelet
    ///   used by this class is the mother function <c>h(x) = cos(1.75x)*exp(-x²/2)</c>.</para>
    ///     
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Li Zhang, Weida Zhou, and Licheng Jiao; Wavelet Support Vector Machine. IEEE
    ///       Transactions on Systems, Man, and Cybernetics—Part B: Cybernetics, Vol. 34, 
    ///       No. 1, February 2004.</description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    [Serializable]
    public sealed class Wavelet : IKernel, ICloneable
    {
        // Default wavelet mother function : h(x) = cos(1.75x)*exp(-x²/2)
        private Func<double, double> h = (x => Math.Cos(1.75 * x) * Math.Exp(-(x * x) / 2.0));

        private double dilation = 1.0;
        private double translation = 0.0;
        private bool invariant     = true;

        /// <summary>
        ///   Gets or sets the Mother wavelet for this kernel.
        /// </summary>
        /// 
        public Func<double, double> Mother
        {
            get { return h; }
            set { h = value; }
        }

        /// <summary>
        ///   Gets or sets the wavelet dilation for this kernel.
        /// </summary>
        /// 
        public double Dilation
        {
            get { return dilation; }
            set { dilation = value; }
        }

        /// <summary>
        ///   Gets or sets the wavelet translation for this kernel.
        /// </summary>
        /// 
        public double Translation
        {
            get { return translation; }
            set { translation = value; }
        }

        /// <summary>
        ///   Gets or sets whether this is
        ///   an invariant Wavelet kernel.
        /// </summary>
        /// 
        public bool Invariant
        {
            get { return invariant; }
            set { invariant = value; }
        }


        /// <summary>
        ///   Constructs a new Wavelet kernel.
        /// </summary>
        /// 
        public Wavelet(bool invariant)
        {
            this.invariant = invariant;
        }

        /// <summary>
        ///   Constructs a new Wavelet kernel.
        /// </summary>
        /// 
        public Wavelet(bool invariant, double dilation)
        {
            this.invariant = invariant;
            this.dilation = dilation;
        }

        /// <summary>
        ///   Constructs a new Wavelet kernel.
        /// </summary>
        /// 
        public Wavelet(bool invariant, double dilation, Func<double, double> mother)
        {
            this.invariant = invariant;
            this.dilation = dilation;
            this.h = mother;
        }

        /// <summary>
        ///   Constructs a new Wavelet kernel.
        /// </summary>
        /// 
        public Wavelet(double translation, double dilation)
        {
            this.invariant = false;
            this.dilation = dilation;
            this.translation = translation;
        }

        /// <summary>
        ///   Constructs a new Wavelet kernel.
        /// </summary>
        /// 
        public Wavelet(double translation, double dilation, Func<double, double> mother)
        {
            this.invariant = false;
            this.dilation = dilation;
            this.translation = translation;
            this.h = mother;
        }


        /// <summary>
        ///   Wavelet kernel function.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public double Function(double[] x, double[] y)
        {
            double prod = 1.0;

            if (invariant)
            {
                for (int i = 0; i < x.Length; i++)
                {
                    prod *= (h((x[i] - translation) / dilation)) *
                            (h((y[i] - translation) / dilation));
                }
            }
            else
            {
                for (int i = 0; i < x.Length; i++)
                {
                    prod *= h((x[i] - y[i]) / dilation);
                }
            }

            return prod;
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
