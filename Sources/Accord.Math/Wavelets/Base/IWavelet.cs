// Accord Wavelet Library
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

namespace Accord.Math.Wavelets
{
    /// <summary>
    ///   Common interface for wavelets algorithms.
    /// </summary>
    /// 
    public interface IWavelet
    {
        /// <summary>
        ///   1-D Forward Discrete Wavelet Transform.
        /// </summary>
        /// 
        void Forward(double[] data);

        /// <summary>
        ///   2-D Forward Discrete Wavelet Transform.
        /// </summary>
        /// 
        void Forward(double[,] data);

        /// <summary>
        ///   1-D Backward (Inverse) Discrete Wavelet Transform.
        /// </summary>
        /// 
        void Backward(double[] data);

        /// <summary>
        ///   2-D Backward (Inverse) Discrete Wavelet Transform.
        /// </summary>
        /// 
        void Backward(double[,] data);
    }
}
