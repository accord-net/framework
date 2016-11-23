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

namespace Accord.Math
{

    /// <summary>
    ///   Taylor series expansions for common functions.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In mathematics, a Taylor series is a representation of a function as an infinite sum of terms 
    ///   that are calculated from the values of the function's derivatives at a single point.</para>
    ///   
    /// <para>
    ///   The concept of a Taylor series was discovered by the Scottish mathematician James Gregory and 
    ///   formally introduced by the English mathematician Brook Taylor in 1715. If the Taylor series is 
    ///   centered at zero, then that series is also called a Maclaurin series, named after the Scottish 
    ///   mathematician Colin Maclaurin, who made extensive use of this special case of Taylor series in
    ///   the 18th century.</para>
    ///   
    /// <para>
    ///   It is common practice to approximate a function by using a finite number of terms of its Taylor
    ///   series. Taylor's theorem gives quantitative estimates on the error in this approximation. Any 
    ///   finite number of initial terms of the Taylor series of a function is called a Taylor polynomial.
    ///   The Taylor series of a function is the limit of that function's Taylor polynomials, provided that
    ///   the limit exists. A function may not be equal to its Taylor series, even if its Taylor series 
    ///   converges at every point. A function that is equal to its Taylor series in an open interval (or 
    ///   a disc in the complex plane) is known as an analytic function in that interval.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Taylor_series">
    ///       Wikipedia, The Free Encyclopedia. Taylor series. Available at: 
    ///       http://en.wikipedia.org/wiki/Taylor_series </a></description></item>
    ///     <item><description><a href="http://www.haverford.edu/physics/MathAppendices/Taylor_Series.pdf">
    ///       Anne Fry, Amy Plofker, Sarah-marie Belcastro, Lyle Roelofs. A Set of Appendices on Mathematical 
    ///       Methods for Physics Students: Taylor Series Expansions and Approximations. Available at: 
    ///       http://www.haverford.edu/physics/MathAppendices/Taylor_Series.pdf </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    public static class Taylor
    {
        /// <summary>
        ///   Returns the sine of a specified angle by evaluating a Taylor series.
        /// </summary>
        /// 
        /// <param name="x">An angle, measured in radians.</param>
        /// <param name="nTerms">The number of terms to be evaluated.</param>
        /// 
        /// <returns>The sine of the angle <paramref name="x"/>.</returns>
        /// 
        public static double Sin(double x, int nTerms)
        {
            if (nTerms < 2)
                return x;

            if (nTerms == 2)
                return x - (x * x * x) / 6D;

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

        /// <summary>
        ///   Returns the cosine of a specified angle by evaluating a Taylor series.
        /// </summary>
        /// 
        /// <param name="x">An angle, measured in radians.</param>
        /// <param name="nTerms">The number of terms to be evaluated.</param>
        /// 
        /// <returns>The cosine of the angle <paramref name="x"/>.</returns>
        /// 
        public static double Cos(double x, int nTerms)
        {
            if (nTerms < 2)
                return 1;

            if (nTerms == 2)
                return 1 - (x * x) / 2D;

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

        /// <summary>
        ///   Returns the hyperbolic sine of a specified angle by evaluating a Taylor series.
        /// </summary>
        /// 
        /// <param name="x">An angle, measured in radians.</param>
        /// <param name="nTerms">The number of terms to be evaluated.</param>
        /// 
        /// <returns>The hyperbolic sine of the angle <paramref name="x"/>.</returns>
        /// 
        public static double Sinh(double x, int nTerms)
        {
            if (nTerms < 2)
                return x;

            if (nTerms == 2)
                return x + (x * x * x) / 6D;

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

        /// <summary>
        ///   Returns the hyperbolic cosine of a specified angle by evaluating a Taylor series.
        /// </summary>
        /// 
        /// <param name="x">An angle, measured in radians.</param>
        /// <param name="nTerms">The number of terms to be evaluated.</param>
        /// 
        /// <returns>The hyperbolic cosine of the angle <paramref name="x"/>.</returns>
        /// 
        public static double Cosh(double x, int nTerms)
        {
            if (nTerms < 2)
                return x;

            if (nTerms == 2)
                return 1 + (x * x) / 2D;

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

        /// <summary>
        ///   Returns e raised to the specified power by evaluating a Taylor series.
        /// </summary>
        /// 
        /// <param name="d">A number specifying a power.</param>
        /// <param name="nTerms">The number of terms to be evaluated.</param>
        /// 
        /// <returns>Euler's constant raised to the specified power <paramref name="d"/>.</returns>
        /// 
        public static double Exp(double d, int nTerms)
        {
            if (nTerms < 2)
                return 1 + d;

            if (nTerms == 2)
                return 1 + d + (d * d) / 2;

            double mult = d * d;
            double fact = 2;
            double result = 1 + d + mult / fact;

            for (int i = 3; i <= nTerms; i++)
            {
                mult *= d;
                fact *= i;
                result += mult / fact;
            }

            return result;
        }
    }
}
