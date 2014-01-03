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

// Contains functions from the Cephes Math Library Release 2.8:
// June, 2000 Copyright 1984, 1987, 1988, 2000 by Stephen L. Moshier
//
// Original license is listed below:
//
//   Some software in this archive may be from the book _Methods and
// Programs for Mathematical Functions_ (Prentice-Hall or Simon & Schuster
// International, 1989) or from the Cephes Mathematical Library, a
// commercial product. In either event, it is copyrighted by the author.
// What you see here may be used freely but it comes with no support or
// guarantee.
//
//   The two known misprints in the book are repaired here in the
// source listings for the gamma function and the incomplete beta
// integral.
//
//
//   Stephen L. Moshier
//   moshier@na-net.ornl.gov
//

namespace Accord.Math
{
    using System;

    /// <summary>
    ///   Common mathematical constants.
    /// </summary>
    ///  
    /// <remarks>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Cephes Math Library, http://www.netlib.org/cephes/ </description></item>
    ///     <item><description>
    ///       http://www.johndcook.com/cpp_expm1.html </description></item>
    ///   </list>
    /// </remarks>
    /// 
    public static class Constants
    {

        /// <summary>Euler-Mascheroni constant.</summary>
        public const double EulerGamma = 0.5772156649015328606065120;

        /// <summary>Double-precision machine round-off error.</summary>
        /// <remarks>This value is actually different from Double.Epsilon.</remarks>
        public const double DoubleEpsilon = 1.11022302462515654042E-16;

        /// <summary>Single-precision machine round-off error.</summary>
        /// <remarks>This value is actually different from Single.Epsilon.</remarks>
        public const float SingleEpsilon = 1.1920929E-07f;

        /// <summary>Double-precision small value.</summary>
        public const double DoubleSmall = 1.493221789605150e-300;

        /// <summary>Single-precision small value.</summary>
        public const float SingleSmall = 1.493221789605150e-40f;

        /// <summary>Maximum log on the machine.</summary>
        public const double LogMax = 7.09782712893383996732E2;

        /// <summary>Minimum log on the machine.</summary>
        public const double LogMin = -7.451332191019412076235E2;

   

        /// <summary>Log of number pi: log(pi).</summary>
        public const double LogPI = 1.14472988584940017414;

        /// <summary>Log of two: log(2).</summary>
        public const double Log2 = 0.69314718055994530941;

        /// <summary>Log of three: log(3).</summary>
        public const double Log3 = 1.098612288668109691395;

        /// <summary>Log of square root of twice number pi: log(sqrt(2*π).</summary>
        public const double LogSqrt2PI = 0.91893853320467;

        /// <summary>Log of twice number pi: log(2*pi).</summary>
        public const double Log2PI = 1.837877066409345483556;

        /// <summary>Square root of twice number pi: sqrt(2*π).</summary>
        public const double Sqrt2PI = 2.50662827463100050242E0;

        /// <summary>Square root of half number π: sqrt(π/2).</summary>
        public const double SqrtHalfPI = 1.25331413731550025121E0;

        /// <summary>Square root of 2: sqrt(2).</summary>
        public const double Sqrt2 = 1.4142135623730950488016887;

        /// <summary>Half square root of 2: sqrt(2)/2.</summary>
        public const double Sqrt2H = 7.07106781186547524401E-1;

    }
}
