// Accord Math Library
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

namespace Accord.Math
{
    using System;
    using Accord.Compat;
    using System.Numerics;
    using Accord.Math.Transforms;

    /// <summary>
    ///   Obsolete. Please use <see cref="HilbertTransform2"/> instead.
    /// </summary>
    /// 
    [Obsolete("Please use HilbertTransform2 instead.")]
    public static class HilbertTransform
    {
        /// <summary>
        ///   Performs the Fast Hilbert Transform over a double[] array.
        /// </summary>
        /// 
        public static void FHT(double[] data, FourierTransform.Direction direction)
        {
            int N = data.Length;


            // Forward operation
            if (direction == FourierTransform.Direction.Forward)
            {
                // Copy the input to a complex array which can be processed
                //  in the complex domain by the FFT
                Complex[] cdata = new Complex[N];
                for (int i = 0; i < N; i++)
                    cdata[i] = new Complex(data[i], 0.0);

                // Perform FFT
                FourierTransform.FFT(cdata, FourierTransform.Direction.Forward);

                //double positive frequencies
                for (int i = 1; i < (N / 2); i++)
                {
                    cdata[i] *= 2.0;
                }

                // zero out negative frequencies
                //  (leaving out the dc component)
                for (int i = (N / 2) + 1; i < N; i++)
                {
                    cdata[i] = Complex.Zero;
                }

                // Reverse the FFT
                FourierTransform.FFT(cdata, FourierTransform.Direction.Backward);

                // Convert back to our initial double array
                for (int i = 0; i < N; i++)
                    data[i] = cdata[i].Imaginary;
            }

            else // Backward operation
            {
                // The inverse Hilbert can be calculated by
                //  negating the transform and reapplying the
                //  transformation.
                //
                // H^–1{h(t)} = –H{h(t)}

                FHT(data, FourierTransform.Direction.Forward);

                for (int i = 0; i < data.Length; i++)
                    data[i] = -data[i];
            }
        }


        /// <summary>
        ///   Performs the Fast Hilbert Transform over a complex[] array.
        /// </summary>
        /// 
        public static void FHT(Complex[] data, FourierTransform.Direction direction)
        {
            int N = data.Length;

            // Forward operation
            if (direction == FourierTransform.Direction.Forward)
            {
                // Makes a copy of the data so we don't lose the
                //  original information to build our final signal
                Complex[] shift = (Complex[])data.Clone();

                // Perform FFT
                FourierTransform.FFT(shift, FourierTransform.Direction.Backward);

                //double positive frequencies
                for (int i = 1; i < (N / 2); i++)
                {
                    shift[i] *= 2.0;
                }
                // zero out negative frequencies
                //  (leaving out the dc component)
                for (int i = (N / 2) + 1; i < N; i++)
                {
                    shift[i] = Complex.Zero;
                }

                // Reverse the FFT
                FourierTransform.FFT(shift, FourierTransform.Direction.Forward);

                // Put the Hilbert transform in the Imaginary part
                //  of the input signal, creating a Analytic Signal
                for (int i = 0; i < N; i++)
                    data[i] = new Complex(data[i].Real, shift[i].Imaginary);
            }

            else // Backward operation
            {
                // Just discard the imaginary part
                for (int i = 0; i < data.Length; i++)
                    data[i] = new Complex(data[i].Real, 0.0);
            }
        }

    }
}
