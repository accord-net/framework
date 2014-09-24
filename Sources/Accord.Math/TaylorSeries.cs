// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Diego Catalano, 2014
// diego.catalano at live.com
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accord.Math
{

    /// <summary>
    /// Taylor Series Expansions.
    /// 
    /// <para>In mathematics, a Taylor series is a representation of a function as an infinite sum of terms that are calculated from
    /// the values of the function's derivatives at a single point.</para>
    /// 
    /// </summary>
    /// <remarks>
    ///   http://www.haverford.edu/physics/MathAppendices/Taylor_Series.pdf
    /// </remarks>
    public static class TaylorSeries
    {

        /// <summary>
        /// Compute Sin using Taylor Series.
        /// </summary>
        /// <param name="x">An angle, in radians.</param>
        /// <param name="nTerms">Number of terms.</param>
        /// <returns>Sin of the x.</returns>
        public static double Sin(double x, int nTerms)
        {
            if (nTerms < 2) return x;
            if (nTerms == 2)
            {
                return x - (x * x * x) / 6D;
            }
            else
            {

                double mult = x * x * x;
                double fact = 6;
                double sign = 1;
                int factS = 5;
                double result = x - mult / fact;
                for (int i = 3; i <= nTerms; i++)
                {
                    mult *= x * x;
                    fact *= factS * (factS - 1);
                    factS += 2;
                    result += sign * (mult / fact);
                    sign *= -1;
                }

                return result;
            }
        }

        /// <summary>
        /// Compute Cos using Taylor Series.
        /// </summary>
        /// <param name="x">An angle, in radians.</param>
        /// <param name="nTerms">Number of terms.</param>
        /// <returns>Cos of the x.</returns>
        public static double Cos(double x, int nTerms)
        {
            if (nTerms < 2) return 1;
            if (nTerms == 2)
            {
                return 1 - (x * x) / 2D;
            }
            else
            {

                double mult = x * x;
                double fact = 2;
                double sign = 1;
                int factS = 4;
                double result = 1 - mult / fact;
                for (int i = 3; i <= nTerms; i++)
                {
                    mult *= x * x;
                    fact *= factS * (factS - 1);
                    factS += 2;
                    result += sign * (mult / fact);
                    sign *= -1;
                }

                return result;
            }
        }

        /// <summary>
        /// Compute Hiperbolic Sin using Taylor Series.
        /// </summary>
        /// <param name="x">An angle, in radians.</param>
        /// <param name="nTerms">Number of terms.</param>
        /// <returns>Hiperbolic sin of the x.</returns>
        public static double Sinh(double x, int nTerms)
        {
            if (nTerms < 2) return x;
            if (nTerms == 2)
            {
                return x + (x * x * x) / 6D;
            }
            else
            {

                double mult = x * x * x;
                double fact = 6;
                int factS = 5;
                double result = x + mult / fact;
                for (int i = 3; i <= nTerms; i++)
                {
                    mult *= x * x;
                    fact *= factS * (factS - 1);
                    factS += 2;
                    result += mult / fact;
                }

                return result;
            }
        }

        /// <summary>
        /// Compute Hiperbolic Cos using Taylor Series.
        /// </summary>
        /// <param name="x">An angle, in radians.</param>
        /// <param name="nTerms">Number of terms.</param>
        /// <returns>Hiperbolic cos of the x.</returns>
        public static double Cosh(double x, int nTerms)
        {
            if (nTerms < 2) return x;
            if (nTerms == 2)
            {
                return 1 + (x * x) / 2D;
            }
            else
            {

                double mult = x * x;
                double fact = 2;
                int factS = 4;
                double result = 1 + mult / fact;
                for (int i = 3; i <= nTerms; i++)
                {
                    mult *= x * x;
                    fact *= factS * (factS - 1);
                    factS += 2;
                    result += mult / fact;
                }

                return result;
            }
        }

        /// <summary>
        /// Compute Exp using Taylor Series.
        /// </summary>
        /// <param name="x">An angle, in radians.</param>
        /// <param name="nTerms">Number of terms.</param>
        /// <returns>Exp of the x.</returns>
        public static double Exp(double x, int nTerms)
        {
            if (nTerms < 2) return 1 + x;
            if (nTerms == 2)
            {
                return 1 + x + (x * x) / 2;
            }
            else
            {

                double mult = x * x;
                double fact = 2;
                double result = 1 + x + mult / fact;
                for (int i = 3; i <= nTerms; i++)
                {
                    mult *= x;
                    fact *= i;
                    result += mult / fact;
                }

                return result;
            }
        }
    }
}
