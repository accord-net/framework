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
//
// Copyright © AJ Richardson, 2015
// https://github.com/aj-r/MathExtension
//
//    The MIT License(MIT)
//    
//    Copyright(c) 2015 AJ Richardson
//    
//    Permission is hereby granted, free of charge, to any person obtaining a copy
//    of this software and associated documentation files (the "Software"), to deal
//    in the Software without restriction, including without limitation the rights
//    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//    copies of the Software, and to permit persons to whom the Software is
//    furnished to do so, subject to the following conditions:
//    
//    The above copyright notice and this permission notice shall be included in all
//    copies or substantial portions of the Software.
//    
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//    SOFTWARE.
//

namespace Accord.Math
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using Accord.Compat;
#if NETSTANDARD1_4
    using SMath = Accord.Compat.SMath;
#else
    using SMath = System.Math;
#endif

    /// <summary>
    ///   Rational number.
    /// </summary>
    /// 
    [Serializable]
#if !NETSTANDARD1_4
    [TypeConverter(typeof(Accord.Math.Converters.RationalConverter))]
#endif
    public struct Rational : IComparable, IComparable<Rational>, IEquatable<Rational>, IFormattable
    {
        private const double DEFAULT_TOLERANCE = 1e-6;
        private const decimal DEFAULT_DECIMAL_TOLERANCE = 1e-15M;
        private const float DEFAULT_FLOAT_TOLERANCE = 1e-6f;

        #region Members

        private readonly int _numerator;
        private readonly int _denominator;

        /// <summary>
        /// Represents the number zero.
        /// </summary>
        public static readonly Rational Zero = new Rational(0, 1);

        /// <summary>
        /// Represents the number one.
        /// </summary>
        public static readonly Rational One = new Rational(1, 1);

        /// <summary>
        /// Represents the minimum finite value of a <see cref="Rational"/>.
        /// </summary>
        public static readonly Rational MinValue = new Rational(int.MinValue, 1);

        /// <summary>
        /// Represents the maximum finite value of a <see cref="Rational"/>.
        /// </summary>
        public static readonly Rational MaxValue = new Rational(int.MaxValue, 1);

        /// <summary>
        /// Represents an indeterminate value.
        /// </summary>
        public static readonly Rational Indeterminate = new Rational(0, 0);

        /// <summary>
        /// Represents positive infinity.
        /// </summary>
        public static readonly Rational PositiveInfinity = new Rational(1, 0);

        /// <summary>
        /// Represents negative infinity.
        /// </summary>
        public static readonly Rational NegativeInfinity = new Rational(-1, 0);

        /// <summary>
        /// Represents the minimum positive value of a <see cref="Rational"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This field has a value of 1 / 2,147,483,647.
        /// </para>
        /// <para>
        /// This does NOT represent the minimum possible difference between two <see cref="Rational"/> instances; some rationals may have a smaller difference.
        /// If you try to subrtact two rationals whose difference is smaller than this value, you will get unexpected results due to overflow.
        /// </para>
        /// <example>
        /// To check for this case, you can add this value to one of the rationals and compare to the other rational.
        /// <code>
        ///   if (r1 + Rational.Epsilon &gt; r2 &amp;&amp; r1 - Rational.Epsilon &lt; r2)
        ///   {
        ///     // Difference between r1 and r2 is less than Rational.Epsilon.
        ///   }
        /// </code>
        /// </example>
        /// </remarks>
        public static readonly Rational Epsilon = new Rational(1, int.MaxValue);

        #endregion

        #region Static Methods

        /// <summary>
        /// Converts the string representation of a number to its <see cref="Rational"/> representation.
        /// </summary>
        /// <param name="s">A string that represents a number.</param>
        /// <returns>The parsed <see cref="Rational"/> value.</returns>
        public static Rational Parse(string s)
        {
            return Parse(s, null);
        }

        /// <summary>
        /// Converts the string representation of a number to its <see cref="Rational"/> representation.
        /// </summary>
        /// <param name="s">A string that represents a number.</param>
        /// <param name="style">Indicates the styles that can be present when parsing a number.</param>
        /// <returns>The parsed <see cref="Rational"/> value.</returns>
        public static Rational Parse(string s, NumberStyles style)
        {
            return Parse(s, style, null);
        }

        /// <summary>
        /// Converts the string representation of a number to its <see cref="Rational"/> representation.
        /// </summary>
        /// <param name="s">A string that represents a number.</param>
        /// <param name="provider">An object that supplies culture-specific information about the format of <paramref name="s"/>.</param>
        /// <returns>The parsed <see cref="Rational"/> value.</returns>
        public static Rational Parse(string s, IFormatProvider provider)
        {
            return Parse(s, NumberStyles.Any, null);
        }

        /// <summary>
        /// Converts the string representation of a number to its <see cref="Rational"/> representation.
        /// </summary>
        /// <param name="s">A string that represents a number.</param>
        /// <param name="style">Indicates the styles that can be present when parsing a number.</param>
        /// <param name="provider">An object that supplies culture-specific information about the format of <paramref name="s"/>.</param>
        /// <returns>The parsed <see cref="Rational"/> value.</returns>
        public static Rational Parse(string s, NumberStyles style, IFormatProvider provider)
        {
            Rational result;
            TryParse(s, style, provider, true, out result);
            return result;
        }

        /// <summary>
        /// Converts the string representation of a number to its <see cref="Rational"/> representation. A return value indicates whether the conversion succeeded or failed.
        /// </summary>
        /// <param name="s">A string that represents a number.</param>
        /// <param name="result">When this method returns, contains the parsed <see cref="Rational"/> value.</param>
        /// <returns>True if the conversion succeeded; otherwise false.</returns>
        public static bool TryParse(string s, out Rational result)
        {
            return TryParse(s, null, out result);
        }

        /// <summary>
        /// Converts the string representation of a number to its <see cref="Rational"/> representation. A return value indicates whether the conversion succeeded or failed.
        /// </summary>
        /// <param name="s">A string that represents a number.</param>
        /// <param name="style">Indicates the styles that can be present when parsing a number.</param>
        /// <param name="result">When this method returns, contains the parsed <see cref="Rational"/> value.</param>
        /// <returns>True if the conversion succeeded; otherwise false.</returns>
        public static bool TryParse(string s, NumberStyles style, out Rational result)
        {
            return TryParse(s, style, null, out result);
        }

        /// <summary>
        /// Converts the string representation of a number to its <see cref="Rational"/> representation. A return value indicates whether the conversion succeeded or failed.
        /// </summary>
        /// <param name="s">A string that represents a number.</param>
        /// <param name="provider">An object that supplies culture-specific information about the format of <paramref name="s"/>.</param>
        /// <param name="result">When this method returns, contains the parsed <see cref="Rational"/> value.</param>
        /// <returns>True if the conversion succeeded; otherwise false.</returns>
        public static bool TryParse(string s, IFormatProvider provider, out Rational result)
        {
            return TryParse(s, NumberStyles.Any, provider, out result);
        }

        /// <summary>
        /// Converts the string representation of a number to its <see cref="Rational"/> representation. A return value indicates whether the conversion succeeded or failed.
        /// </summary>
        /// <param name="s">A string that represents a number.</param>
        /// <param name="style">Indicates the styles that can be present when parsing a number.</param>
        /// <param name="provider">An object that supplies culture-specific information about the format of <paramref name="s"/>.</param>
        /// <param name="result">When this method returns, contains the parsed <see cref="Rational"/> value.</param>
        /// <returns>True if the conversion succeeded; otherwise false.</returns>
        public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out Rational result)
        {
            return TryParse(s, style, provider, false, out result);
        }

        private static bool TryParse(string s, NumberStyles style, IFormatProvider provider, bool throwOnFailure, out Rational result)
        {
#if NET35
            if (string.IsNullOrEmpty(s.Trim()))
#else
            if (string.IsNullOrWhiteSpace(s))
#endif
                return ParseFailure(throwOnFailure, out result);
            var parts = s.Split('/');
            if (parts.Length > 2)
                return ParseFailure(throwOnFailure, out result);
            if (parts.Length == 1)
            {
                int n;
                if (!int.TryParse(s, style, provider, out n))
                {
                    // Maybe the number is in decimal format. Try parsing as such.
                    double d;
                    if (!double.TryParse(s, style, provider, out d))
                        return ParseFailure(throwOnFailure, out result);
                    result = FromDouble(d);
                    return true;
                }
                result = new Rational(n);
                return true;
            }

            var numeratorString = parts[0].Trim();

            var separatorIndex = numeratorString.LastIndexOf('-');
            if (separatorIndex <= 0)
                separatorIndex = numeratorString.LastIndexOf(' ');
            int integerPart = 0;
            if (separatorIndex > 0)
            {
                var integerString = numeratorString.Remove(separatorIndex);
                numeratorString = numeratorString.Substring(separatorIndex + 1);
                if (!TryParseInt(integerString, style, provider, throwOnFailure, out integerPart))
                {
                    result = Indeterminate;
                    return false;
                }
            }
            int numerator, denominator;
            if (!TryParseInt(numeratorString, style, provider, throwOnFailure, out numerator)
                || !TryParseInt(parts[1], style, provider, throwOnFailure, out denominator))
            {
                result = Indeterminate;
                return false;
            }
            if (integerPart < 0)
                numerator *= -1;

            result = new Rational(integerPart * denominator + numerator, denominator);
            return true;
        }

        private static bool ParseFailure(bool throwOnFailure, out Rational result)
        {
            if (throwOnFailure)
                throw new FormatException();
            result = Indeterminate;
            return false;
        }

        private static bool TryParseInt(string s, NumberStyles style, IFormatProvider provider, bool throwOnFailure, out int result)
        {
            if (throwOnFailure)
            {
                result = int.Parse(s, style, provider);
                return true;
            }
            return int.TryParse(s, style, provider, out result);
        }

        /// <summary>
        /// Converts a floating-point number to a rational number.
        /// </summary>
        /// <param name="value">A floating-point number to convert to a rational number.</param>
        /// <param name="tolerance">The maximum error allowed between the result and the input value.</param>
        /// <returns>A rational number.</returns>
        public static Rational FromDouble(double value, double tolerance = DEFAULT_TOLERANCE)
        {
            if (double.IsPositiveInfinity(value))
                return PositiveInfinity;
            if (double.IsNegativeInfinity(value))
                return NegativeInfinity;
            if (double.IsNaN(value))
                return Indeterminate;

            // Set numerator to 'value' for now; we will set it to the actual numerator once we know
            // what the denominator is.
            double numerator = value;

            if (value < 0)
                value *= -1;

            // Get the denominator
            double denominator = 1.0;
            double fractionPart = value - System.Math.Truncate(value);
            int n = 0;
            while (!IsInteger(fractionPart, tolerance) && n < 100)
            {
                value = 1.0 / fractionPart;
                denominator *= value;
                fractionPart = value % 1.0;
                n++;
            }

            // Get the actual numerator
            numerator *= denominator;
            return new Rational(Convert.ToInt32(numerator), Convert.ToInt32(denominator));
        }

        /// <summary>
        /// Converts a floating-point number to a rational number.
        /// </summary>
        /// <param name="value">A floating-point number to convert to a rational number.</param>
        /// <param name="tolerance">The maximum error allowed between the result and the input value.</param>
        /// <returns>A rational number.</returns>
        public static Rational FromDecimal(decimal value, decimal tolerance = DEFAULT_DECIMAL_TOLERANCE)
        {
            // Set numerator to 'value' for now; we will set it to the actual numerator once we know
            // what the denominator is.
            decimal numerator = value;

            if (value < 0)
                value *= -1;

            // Get the denominator
            decimal denominator = 1.0M;
            decimal fractionPart = value - System.Math.Truncate(value);
            int n = 0;
            while (!IsInteger(fractionPart, tolerance) && n < 100)
            {
                value = 1.0M / fractionPart;
                denominator *= value;
                fractionPart = value % 1.0M;
                n++;
            }

            // Get the actual numerator
            numerator *= denominator;
            return new Rational(Convert.ToInt32(numerator), Convert.ToInt32(denominator));
        }

        /// <summary>
        /// Converts a floating-point decimal to a rational number.
        /// </summary>
        /// <param name="value">A floating-point number to convert to a rational number.</param>
        /// <param name="maxDenominator">The maximum value that the denominator can have.</param>
        /// <param name="tolerance">
        /// The desired maximum error between the result and the input value. This is only used as an alternative end condition; 
        /// the actual error may be greater because of the limitation on the denominator value.
        /// </param>
        /// <returns>A rational number.</returns>
        public static Rational FromDoubleWithMaxDenominator(double value, int maxDenominator, double tolerance = DEFAULT_TOLERANCE)
        {
            if (double.IsPositiveInfinity(value))
                return PositiveInfinity;
            if (double.IsNegativeInfinity(value))
                return NegativeInfinity;
            if (double.IsNaN(value))
                return Indeterminate;

            if (maxDenominator < 1)
                throw new ArgumentOutOfRangeException("Maximum denominator base must be greater than or equal to 1.", "maxDenominator");

            int denominator = 0;
            // int bestDenominator = 1;
            double bestDifference = 1.0;
            double numerator;
            do
            {
                denominator++;
                numerator = value * (double)denominator;
                double difference = Math.Abs(numerator % 1.0);
                if (difference < bestDifference)
                {
                    bestDifference = difference;
                    // bestDenominator = denominator;
                }
            } while (!IsInteger(numerator, tolerance) && denominator < maxDenominator);

            return new Rational(Convert.ToInt32(numerator), denominator);
        }

        /// <summary>
        /// Returns the absolute value of a Rational.
        /// </summary>
        /// <param name="value">A Rational number.</param>
        /// <returns>Gets the absolute value of the Rational.</returns>
        public static Rational Abs(Rational value)
        {
            return new Rational(System.Math.Abs(value.Numerator), System.Math.Abs(value.Denominator));
        }

        /// <summary>
        /// Returns the smaller of two Rationals.
        /// </summary>
        /// <param name="val1">A Rational number.</param>
        /// <param name="val2">A Rational number.</param>
        /// <returns>The smaller of two Rationals.</returns>
        public static Rational Min(Rational val1, Rational val2)
        {
            return val1 <= val2 ? val1 : val2;
        }

        /// <summary>
        /// Returns the larger of two Rationals.
        /// </summary>
        /// <param name="val1">A Rational number.</param>
        /// <param name="val2">A Rational number.</param>
        /// <returns>The larger of two Rationals.</returns>
        public static Rational Max(Rational val1, Rational val2)
        {
            return val1 >= val2 ? val1 : val2;
        }

        /// <summary>
        /// Rounds a <see cref="Rational"/> to the nearest integral value.
        /// </summary>
        /// <param name="x">The number to round.</param>
        /// <returns>The rounded number.</returns>
        /// <exception cref="DivideByZeroException">The denominator of <paramref name="x"/> is zero.</exception>
        public static int Round(Rational x)
        {
            return (x.Numerator + x.Denominator / 2) / x.Denominator;
        }

        /// <summary>
        /// Returns a specified number raised to a specified power.
        /// </summary>
        /// <param name="baseValue">A Rational number.</param>
        /// <param name="exponent">A Rational number.</param>
        /// <returns>The larger of two Rationals.</returns>
        public static Rational Pow(Rational baseValue, int exponent)
        {
            var result = Rational.One;
            if (exponent < 0)
            {
                exponent *= -1;
                baseValue = baseValue.Inverse();
            }
            while (exponent > 0)
            {
                if ((exponent & 1) == 1)
                    result *= baseValue;
                exponent >>= 1;
                baseValue *= baseValue;
            }
            return result;
        }

        /// <summary>
        /// Calculates the quotient of two rational numbers and also returns the remainder in an output parameter.
        /// </summary>
        /// <param name="a">The dividend.</param>
        /// <param name="b">The divisor.</param>
        /// <param name="remainder">The remainder.</param>
        /// <returns>The quotient.</returns>
        public static int DivRem(Rational a, Rational b, out Rational remainder)
        {
            if (b._numerator == 0 || a._denominator == 0)
                throw new DivideByZeroException();
            if (b._denominator == 0)
            {
                remainder = a;
                return 0;
            }
            int xNum = a._numerator * b._denominator;
            int yNum = b._numerator * a._denominator;
            int denominator = a._denominator * b._denominator;
            int rem;
            int result = SMath.DivRem(xNum, yNum, out rem);
            remainder = Rational.Simplify(rem, denominator);
            return result;
        }

        /// <summary>
        /// Returns a value that indicates whether the specified <see cref="Rational"/> evaluates to positive or negative infinity.
        /// </summary>
        /// <param name="r">A rational number.</param>
        /// <returns>True if <paramref name="r"/> evaluates to positive or negative infinity; otherwise false.</returns>
        public static bool IsInfinity(Rational r)
        {
            return r._denominator == 0 && r._numerator != 0;
        }

        /// <summary>
        /// Returns a value that indicates whether the specified <see cref="Rational"/> evaluates to positive infinity.
        /// </summary>
        /// <param name="r">A rational number.</param>
        /// <returns>True if <paramref name="r"/> evaluates to positive infinity; otherwise false.</returns>
        public static bool IsPositiveInfinity(Rational r)
        {
            return r._denominator == 0 && r._numerator > 0;
        }

        /// <summary>
        /// Returns a value that indicates whether the specified <see cref="Rational"/> evaluates to negative infinity.
        /// </summary>
        /// <param name="r">A rational number.</param>
        /// <returns>True if <paramref name="r"/> evaluates to negative infinity; otherwise false.</returns>
        public static bool IsNegativeInfinity(Rational r)
        {
            return r._denominator == 0 && r._numerator < 0;
        }

        /// <summary>
        /// Returns a value that indicates whether the specified <see cref="Rational"/> represents an indeterminate value.
        /// </summary>
        /// <param name="r">A rational number.</param>
        /// <returns>True if <paramref name="r"/> represents an indeterminate value; otherwise false.</returns>
        public static bool IsIndeterminate(Rational r)
        {
            return r._denominator == 0 && r._numerator == 0;
        }

        /// <summary>
        /// Returns a value that indicates whether the specified <see cref="Rational"/> evaluates to zero.
        /// </summary>
        /// <param name="r">A rational number.</param>
        /// <returns>True if <paramref name="r"/> evaluates to zero; otherwise false.</returns>
        public static bool IsZero(Rational r)
        {
            return r._numerator == 0 && r._denominator != 0;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="Rational"/> instance with a denominator of 1.
        /// </summary>
        /// <param name="numerator">The numerator of the <see cref="Rational"/>.</param>
        public Rational(int numerator)
            : this(numerator, 1)
        { }

        /// <summary>
        /// Creates a new <see cref="Rational"/> instance.
        /// </summary>
        /// <param name="numerator">The numerator of the <see cref="Rational"/>.</param>
        /// <param name="denominator">The denominator of the <see cref="Rational"/>.</param>
        public Rational(int numerator, int denominator)
        {
            _numerator = numerator;
            _denominator = denominator;
        }

        /// <summary>
        /// Creates a new <see cref="Rational"/> instance equal to a floating-point value.
        /// </summary>
        /// <param name="value">A floating-point value</param>
        public Rational(double value)
        {
            this = FromDouble(value);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the numerator of the current <see cref="Rational"/> value.
        /// </summary>
        public int Numerator
        {
            get { return _numerator; }
        }

        /// <summary>
        /// Gets the numerator of the current <see cref="Rational"/> value.
        /// </summary>
        public int Denominator
        {
            get { return _denominator; }
        }

        /// <summary>
        /// Gets the floating-point equivalent of the current <see cref="Rational"/> value.
        /// </summary>
        public double Value
        {
            get
            {
                return (double)Numerator / (double)Denominator;
            }
        }

        /// <summary>
        /// Gets the inverse of the current <see cref="Rational"/> value (that is, one divided by the current value).
        /// </summary>
        /// <returns></returns>
        public Rational Inverse()
        {
            if (Numerator >= 0)
            {
                return new Rational(Denominator, Numerator);
            }
            else
            {
                return new Rational(-Denominator, -Numerator);
            }
        }

        /// <summary>
        /// Gets the negated version of the current <see cref="Rational"/> value (that is, the current value multiplied by negative one).
        /// </summary>
        /// <returns></returns>
        public Rational Negate()
        {
            return new Rational(-Numerator, Denominator);
        }

        #endregion

        #region Instance Methods

        /// <summary>
        /// Gets the simplified version of the rational number.
        /// </summary>
        public static Rational Simplify(int numerator, int denominator)
        {
            if (denominator == 0)
                return new Rational(System.Math.Sign(numerator), 0);

            var gcd = Gcd(numerator, denominator);
            // The denominator in the simplified version should always be positive,
            // so if it is negative, multiply both numbers by -1.
            if (denominator < 0)
                gcd *= -1;
            return new Rational(numerator / gcd, denominator / gcd);
        }

        /// <summary>
        /// Gets the simplified version of the rational number.
        /// </summary>
        public static Rational Simplify(long numerator, long denominator)
        {
            if (denominator == 0)
                return new Rational(Math.Sign(numerator), 0);

            var gcd = Gcd(numerator, denominator);
            if (denominator < 0)
                gcd *= -1;
            return new Rational((int)(numerator / gcd), (int)(denominator / gcd));
        }

        /// <summary>
        /// Gets the simplified version of the rational number.
        /// </summary>
        public Rational Simplify()
        {
            return Simplify(_numerator, _denominator);
        }


        /// <summary>
        /// Converts the current <see cref="Rational"/> to a double.
        /// </summary>
        /// <returns>A <see cref="Double"/>.</returns>
        public double ToDouble()
        {
            return Value;
        }

        /// <summary>
        /// Converts the Rational to a string in the form of an improper fraction.
        /// </summary>
        /// <returns>A string representaion of the rational number.</returns>
        public override string ToString()
        {
            return Numerator.ToString() + (Denominator != 1 ? "/" + Denominator.ToString() : string.Empty);
        }

        /// <summary>
        /// Converts this instance to its equivalent string representation.
        /// </summary>
        /// <param name="provider">An object that has culture-specific formatting information.</param>
        /// <returns>The string representation of the <see cref="Rational"/>.</returns>
        public string ToString(IFormatProvider provider)
        {
            return Numerator.ToString(provider) + (Denominator != 1 ? "/" + Denominator.ToString(provider) : string.Empty);
        }

        /// <summary>
        /// Converts the Rational to a string in the form of a mixed fraction.
        /// </summary>
        /// <returns>A string representaion of the rational number.</returns>
        public string ToMixedString()
        {
            return ToMixedString(" ");
        }

        /// <summary>
        /// Converts the Rational to a string in the form of a mixed fraction.
        /// </summary>
        /// <param name="numberSeparator">The separator between the number part and the fraction part</param>
        /// <returns>A string representaion of the rational number.</returns>
        public string ToMixedString(string numberSeparator)
        {
            string s = string.Empty;
            Rational x = this;
            if (x < Zero)
            {
                s += "-";
                x = x.Negate();
            }
            else if (x.Numerator < 0)
            {
                // The numerator and denominator are both negative
                x = new Rational(-x.Numerator, -x.Denominator);
            }
            bool hasIntegerPart = false;
            if (x.Numerator >= x.Denominator)
            {
                s += ((int)x).ToString();
                hasIntegerPart = true;
            }
            Rational fractionPart = x % Rational.One;
            bool hasFractionPart = fractionPart.Numerator != 0;
            if (hasFractionPart)
            {
                if (hasIntegerPart)
                    s += numberSeparator;
                s += fractionPart.ToString();
            }
            else if (!hasIntegerPart)
            {
                s = "0";
            }
            return s;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns><value>true</value> if the current object and <paramref name="obj"/> are the same type and represent the same value; otherwise, <value>false</value>.</returns>
        public override bool Equals(object obj)
        {
            return (obj is Rational) && this == (Rational)obj;
        }

        /// <summary>
        /// Indicates whether this instance and a specified <see cref="Rational"/> are strictly equal; that is, the two instances must have equal numerators and denominators.
        /// </summary>
        /// <param name="r">Another <see cref="Rational"/> to compare to.</param>
        /// <returns><value>true</value> if the current number and <paramref name="r"/> have equal numerators and denominators; otherwise, <value>false</value>.</returns>
        /// <remarks>
        /// The basic Equals implementation considers unsimplified fractions to be equal to their simplified forms; e.g. 2/4 = 1/2.
        /// This method considers those values to be different.
        /// </remarks>
        public bool StrictlyEquals(Rational r)
        {
            return Numerator == r.Numerator && Denominator == r.Denominator;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>the hash code.</returns>
        public override int GetHashCode()
        {
            // Get the hash code of the simplified Rational. Equivalent Rationals (e.g. 1/2 and 2/4) should have the same hash code.
            var simplified = Simplify();
            return FnvCombine(simplified.Numerator.GetHashCode(), simplified.Denominator.GetHashCode());
        }

        private static int FnvCombine(params int[] hashes)
        {
            unchecked // Overflow is ok
            {
                uint h = 2166136261;
                foreach (var hash in hashes)
                    h = (h * 16777619) ^ (uint)hash;
                return (int)h;
            }
        }

        #endregion

        #region Operators

        /// <summary>
        /// Gets whether two <see cref="Rational"/> values are numerically equivalent.
        /// </summary>
        /// <param name="x">A <see cref="Rational"/>.</param>
        /// <param name="y">A <see cref="Rational"/>.</param>
        /// <returns>True if the values are equivalent; otherwise false.</returns>
        public static bool operator ==(Rational x, Rational y)
        {
            return x.CompareTo(y) == 0;
        }

        /// <summary>
        /// Gets whether two <see cref="Rational"/> values are numerically not equivalent.
        /// </summary>
        /// <param name="x">A <see cref="Rational"/>.</param>
        /// <param name="y">A <see cref="Rational"/>.</param>
        /// <returns>False if the values are equivalent; otherwise true.</returns>
        public static bool operator !=(Rational x, Rational y)
        {
            return !(x == y);
        }

        /// <summary>
        /// Gets whether a <see cref="Rational"/> values is greater than another.
        /// </summary>
        /// <param name="x">A <see cref="Rational"/>.</param>
        /// <param name="y">A <see cref="Rational"/>.</param>
        /// <returns>True if <paramref name="x"/> is greater than <paramref name="y"/>; otherwise false.</returns>
        public static bool operator >(Rational x, Rational y)
        {
            return x.CompareTo(y) > 0;
        }

        /// <summary>
        /// Gets whether a <see cref="Rational"/> values is greater than or equal to another.
        /// </summary>
        /// <param name="x">A <see cref="Rational"/>.</param>
        /// <param name="y">A <see cref="Rational"/>.</param>
        /// <returns>True if <paramref name="x"/> is greater than or equal to <paramref name="y"/>; otherwise false.</returns>
        public static bool operator >=(Rational x, Rational y)
        {
            return !(x < y);
        }

        /// <summary>
        /// Gets whether a <see cref="Rational"/> values is less than another.
        /// </summary>
        /// <param name="x">A <see cref="Rational"/>.</param>
        /// <param name="y">A <see cref="Rational"/>.</param>
        /// <returns>True if <paramref name="x"/> is less than <paramref name="y"/>; otherwise false.</returns>
        public static bool operator <(Rational x, Rational y)
        {
            return x.CompareTo(y) < 0;
        }

        /// <summary>
        /// Gets whether a <see cref="Rational"/> values is less than or equal to another.
        /// </summary>
        /// <param name="x">A <see cref="Rational"/>.</param>
        /// <param name="y">A <see cref="Rational"/>.</param>
        /// <returns>True if <paramref name="x"/> is less than or equal to <paramref name="y"/>; otherwise false.</returns>
        public static bool operator <=(Rational x, Rational y)
        {
            return !(x > y);
        }

        /// <summary>
        /// Gets the <see cref="Rational"/> value.
        /// </summary>
        /// <param name="x">A <see cref="Rational"/>.</param>
        /// <returns>A <see cref="Rational"/>.</returns>
        public static Rational operator +(Rational x)
        {
            return x;
        }

        /// <summary>
        /// Gets the negated version of a <see cref="Rational"/> value.
        /// </summary>
        /// <param name="x">A <see cref="Rational"/>.</param>
        /// <returns>A <see cref="Rational"/>.</returns>
        public static Rational operator -(Rational x)
        {
            if (x.Denominator < 0)
                return new Rational(x.Numerator, -x.Denominator);
            else
                return new Rational(-x.Numerator, x.Denominator);
        }

        /// <summary>
        /// Adds two <see cref="Rational"/> values.
        /// </summary>
        /// <param name="x">A <see cref="Rational"/>.</param>
        /// <param name="y">A <see cref="Rational"/>.</param>
        /// <returns>The simplified sum of the <see cref="Rational"/> values.</returns>
        public static Rational operator +(Rational x, Rational y)
        {
            if (x.Denominator == 0)
            {
                if (y.Denominator == 0 && System.Math.Sign(x.Numerator) != System.Math.Sign(y.Numerator))
                    return Indeterminate;
                return new Rational(System.Math.Sign(x.Numerator), 0);
            }
            if (y.Denominator == 0)
            {
                if (y.Numerator == 0)
                    return Indeterminate;
                return new Rational(System.Math.Sign(y.Numerator), 0);
            }

            int denominator = Lcm(x.Denominator, y.Denominator);
            int xFactor = denominator / x.Denominator;
            int yFactor = denominator / y.Denominator;
            int numerator = x.Numerator * xFactor + y.Numerator * yFactor;
            return Rational.Simplify(numerator, denominator);
        }

        /// <summary>
        /// Subtracts two <see cref="Rational"/> values.
        /// </summary>
        /// <param name="x">A <see cref="Rational"/>.</param>
        /// <param name="y">A <see cref="Rational"/>.</param>
        /// <returns>The simplified difference of the <see cref="Rational"/> values.</returns>
        public static Rational operator -(Rational x, Rational y)
        {
            if (x.Denominator == 0)
            {
                if (y.Denominator == 0 && Math.Sign(x.Numerator) != -Math.Sign(y.Numerator))
                    return Indeterminate;
                return new Rational(Math.Sign(x.Numerator), 0);
            }
            if (y.Denominator == 0)
            {
                if (y.Numerator == 0)
                    return Indeterminate;
                return new Rational(-Math.Sign(y.Numerator), 0);
            }

            int denominator = Lcm(x.Denominator, y.Denominator);
            int xFactor = denominator / x.Denominator;
            int yFactor = denominator / y.Denominator;
            int numerator = x.Numerator * xFactor - y.Numerator * yFactor;
            return Rational.Simplify(numerator, denominator);
        }

        /// <summary>
        /// Multiplies two <see cref="Rational"/> values.
        /// </summary>
        /// <param name="x">A <see cref="Rational"/>.</param>
        /// <param name="y">A <see cref="Rational"/>.</param>
        /// <returns>The simplified product of the <see cref="Rational"/> values.</returns>
        public static Rational operator *(Rational x, Rational y)
        {
            return Rational.Simplify(x.Numerator * y.Numerator, x.Denominator * y.Denominator);
        }

        /// <summary>
        /// Divides two <see cref="Rational"/> values.
        /// </summary>
        /// <param name="x">A <see cref="Rational"/>.</param>
        /// <param name="y">A <see cref="Rational"/>.</param>
        /// <returns>The simplified dividend of the <see cref="Rational"/> values.</returns>
        public static Rational operator /(Rational x, Rational y)
        {
            return Rational.Simplify(x.Numerator * y.Denominator, x.Denominator * y.Numerator);
        }

        /// <summary>
        /// Gets the remainder that results from dividing two <see cref="Rational"/> values.
        /// </summary>
        /// <param name="x">A <see cref="Rational"/>.</param>
        /// <param name="y">A <see cref="Rational"/>.</param>
        /// <returns>The remainder that results from dividing the <see cref="Rational"/> values.</returns>
        public static Rational operator %(Rational x, Rational y)
        {
            if (y.Denominator == 0)
                return x.Denominator == 0 || y.Numerator == 0 ? Indeterminate : x.Simplify();
            if (x.Denominator == 0)
                return x.Simplify();
            if (x.Numerator == 0 || y.Numerator == 0)
                return Zero;

            long xNum = SMath.BigMul(x.Numerator, y.Denominator);
            long yNum = SMath.BigMul(y.Numerator, x.Denominator);
            int denominator = x.Denominator * y.Denominator;
            return Rational.Simplify((int)(xNum % yNum), denominator);
        }

        /// <summary>
        /// Increments a <see cref="Rational"/> value.
        /// </summary>
        /// <param name="x">A <see cref="Rational"/>.</param>
        /// <returns>The incremented <see cref="Rational"/>.</returns>
        public static Rational operator ++(Rational x)
        {
            return x + Rational.One;
        }

        /// <summary>
        /// Decrements a <see cref="Rational"/> value.
        /// </summary>
        /// <param name="x">A <see cref="Rational"/>.</param>
        /// <returns>The idecremented <see cref="Rational"/>.</returns>
        public static Rational operator --(Rational x)
        {
            return x - Rational.One;
        }

        #endregion

        #region Casts

        /// <summary>
        /// Converts the specified <see cref="Int32"/> to a <see cref="Rational"/>.
        /// </summary>
        /// <param name="x">The <see cref="Int32"/> to convert.</param>
        /// <returns>A <see cref="Rational"/>.</returns>
        public static implicit operator Rational(int x)
        {
            return new Rational(x);
        }

        /// <summary>
        /// Converts the specified <see cref="Rational"/> to a <see cref="Int32"/>.
        /// </summary>
        /// <param name="x">The <see cref="Rational"/> to convert.</param>
        /// <returns>A <see cref="Int32"/>.</returns>
        public static explicit operator int(Rational x)
        {
            return x.Numerator / x.Denominator;
        }

        /// <summary>
        /// Converts the specified <see cref="UInt32"/> to a <see cref="Rational"/>.
        /// </summary>
        /// <param name="x">The <see cref="UInt32"/> to convert.</param>
        /// <returns>A <see cref="Rational"/>.</returns>
        public static implicit operator Rational(uint x)
        {
            return new Rational(x);
        }

        /// <summary>
        /// Converts the specified <see cref="Rational"/> to a <see cref="UInt32"/>.
        /// </summary>
        /// <param name="x">The <see cref="Rational"/> to convert.</param>
        /// <returns>A <see cref="UInt32"/>.</returns>
        public static explicit operator uint(Rational x)
        {
            return (uint)(x.Numerator / x.Denominator);
        }

        /// <summary>
        /// Converts the specified <see cref="Int16"/> to a <see cref="Rational"/>.
        /// </summary>
        /// <param name="x">The <see cref="Int16"/> to convert.</param>
        /// <returns>A <see cref="Rational"/>.</returns>
        public static implicit operator Rational(short x)
        {
            return new Rational(x);
        }

        /// <summary>
        /// Converts the specified <see cref="Rational"/> to a <see cref="Int16"/>.
        /// </summary>
        /// <param name="x">The <see cref="Rational"/> to convert.</param>
        /// <returns>A <see cref="Int16"/>.</returns>
        public static explicit operator short(Rational x)
        {
            return (short)(x.Numerator / x.Denominator);
        }

        /// <summary>
        /// Converts the specified <see cref="UInt16"/> to a <see cref="Rational"/>.
        /// </summary>
        /// <param name="x">The <see cref="UInt16"/> to convert.</param>
        /// <returns>A <see cref="Rational"/>.</returns>
        public static implicit operator Rational(ushort x)
        {
            return new Rational(x);
        }

        /// <summary>
        /// Converts the specified <see cref="Rational"/> to a <see cref="UInt16"/>.
        /// </summary>
        /// <param name="x">The <see cref="Rational"/> to convert.</param>
        /// <returns>A <see cref="UInt16"/>.</returns>
        public static explicit operator ushort(Rational x)
        {
            return (ushort)(x.Numerator / x.Denominator);
        }

        /// <summary>
        /// Converts the specified <see cref="Int64"/> to a <see cref="Rational"/>.
        /// </summary>
        /// <param name="x">The <see cref="Int64"/> to convert.</param>
        /// <returns>A <see cref="Rational"/>.</returns>
        public static implicit operator Rational(long x)
        {
            return new Rational(x);
        }

        /// <summary>
        /// Converts the specified <see cref="Rational"/> to a <see cref="Int64"/>.
        /// </summary>
        /// <param name="x">The <see cref="Rational"/> to convert.</param>
        /// <returns>A <see cref="Int64"/>.</returns>
        public static explicit operator long(Rational x)
        {
            return x.Numerator / x.Denominator;
        }

        /// <summary>
        /// Converts the specified <see cref="UInt64"/> to a <see cref="Rational"/>.
        /// </summary>
        /// <param name="x">The <see cref="UInt64"/> to convert.</param>
        /// <returns>A <see cref="Rational"/>.</returns>
        public static implicit operator Rational(ulong x)
        {
            return new Rational(x);
        }

        /// <summary>
        /// Converts the specified <see cref="Rational"/> to a <see cref="UInt64"/>.
        /// </summary>
        /// <param name="x">The <see cref="Rational"/> to convert.</param>
        /// <returns>A <see cref="UInt64"/>.</returns>
        public static explicit operator ulong(Rational x)
        {
            return (ulong)(x.Numerator / x.Denominator);
        }

        /// <summary>
        /// Converts the specified <see cref="SByte"/> to a <see cref="Rational"/>.
        /// </summary>
        /// <param name="x">The <see cref="SByte"/> to convert.</param>
        /// <returns>A <see cref="Rational"/>.</returns>
        public static implicit operator Rational(sbyte x)
        {
            return new Rational(x);
        }

        /// <summary>
        /// Converts the specified <see cref="Rational"/> to a <see cref="SByte"/>.
        /// </summary>
        /// <param name="x">The <see cref="Rational"/> to convert.</param>
        /// <returns>A <see cref="SByte"/>.</returns>
        public static explicit operator sbyte(Rational x)
        {
            return (sbyte)(x.Numerator / x.Denominator);
        }

        /// <summary>
        /// Converts the specified <see cref="Byte"/> to a <see cref="Rational"/>.
        /// </summary>
        /// <param name="x">The <see cref="Byte"/> to convert.</param>
        /// <returns>A <see cref="Rational"/>.</returns>
        public static implicit operator Rational(byte x)
        {
            return new Rational(x);
        }

        /// <summary>
        /// Converts the specified <see cref="Rational"/> to a <see cref="Byte"/>.
        /// </summary>
        /// <param name="x">The <see cref="Rational"/> to convert.</param>
        /// <returns>A <see cref="Byte"/>.</returns>
        public static explicit operator byte(Rational x)
        {
            return (byte)(x.Numerator / x.Denominator);
        }

        /// <summary>
        /// Converts the specified <see cref="Single"/> to a <see cref="Rational"/>.
        /// </summary>
        /// <param name="x">The <see cref="Single"/> to convert.</param>
        /// <returns>A <see cref="Rational"/>.</returns>
        public static explicit operator Rational(float x)
        {
            // Use a much bigger tolerance for floats because there will be bigger rounding errors.
            return Rational.FromDouble(x, 1e-3);
        }

        /// <summary>
        /// Converts the specified <see cref="Rational"/> to a <see cref="Single"/>.
        /// </summary>
        /// <param name="x">The <see cref="Rational"/> to convert.</param>
        /// <returns>A <see cref="Single"/>.</returns>
        public static explicit operator float(Rational x)
        {
            return (float)x.Value;
        }

        /// <summary>
        /// Converts the specified <see cref="Double"/> to a <see cref="Rational"/>.
        /// </summary>
        /// <param name="x">The <see cref="Double"/> to convert.</param>
        /// <returns>A <see cref="Rational"/>.</returns>
        public static explicit operator Rational(double x)
        {
            return new Rational(x);
        }

        /// <summary>
        /// Converts the specified <see cref="Rational"/> to a <see cref="Double"/>.
        /// </summary>
        /// <param name="x">The <see cref="Rational"/> to convert.</param>
        /// <returns>A <see cref="Double"/>.</returns>
        public static explicit operator double(Rational x)
        {
            return x.Value;
        }

        /// <summary>
        /// Converts the specified <see cref="Decimal"/> to a <see cref="Rational"/>.
        /// </summary>
        /// <param name="x">The <see cref="Decimal"/> to convert.</param>
        /// <returns>A <see cref="Rational"/>.</returns>
        public static explicit operator Rational(decimal x)
        {
            return Rational.FromDecimal(x);
        }

        /// <summary>
        /// Converts the specified <see cref="Rational"/> to a <see cref="Decimal"/>.
        /// </summary>
        /// <param name="x">The <see cref="Rational"/> to convert.</param>
        /// <returns>A <see cref="Decimal"/>.</returns>
        public static explicit operator decimal(Rational x)
        {
            return (decimal)x.Numerator / (decimal)x.Denominator;
        }

        #endregion

        #region IComparable Members

        /// <summary>
        /// Compares this instance to another <see cref="Rational"/> and returns an indication of their relative values.
        /// </summary>
        /// <param name="obj">The value to compare to. This should be a numeric value.</param>
        /// <returns>A signed number indicating the relative values of this instance and <paramref name="obj"/>.</returns>
        public int CompareTo(object obj)
        {
            if (obj is Rational)
            {
                var r = (Rational)obj;
                return CompareTo(r);
            }
            else
            {
                var d1 = Value;
                var d2 = Convert.ToDouble(obj);
                return d1.CompareTo(d2);
            }
        }

        #endregion

        #region IComparable<Rational> Members

        /// <summary>
        /// Compares this instance to another <see cref="Rational"/> and returns an indication of their relative values.
        /// </summary>
        /// <param name="other">The <see cref="Rational"/> to compare to.</param>
        /// <returns>A signed number indicating the relative values of this instance and <paramref name="other"/>.</returns>
        public int CompareTo(Rational other)
        {
            if (Denominator == 0)
            {
                if (other.Denominator == 0)
                    return System.Math.Sign(Numerator).CompareTo(System.Math.Sign(other.Numerator));
                return Numerator == 0 ? -1 : System.Math.Sign(Numerator);
            }
            if (other.Denominator == 0)
            {
                return other.Numerator == 0 ? 1 : -System.Math.Sign(other.Numerator);
            }
            // Use BigMul to avoid losing data when multiplying large integers
            long value1 = SMath.BigMul(Numerator, other.Denominator);
            long value2 = SMath.BigMul(Denominator, other.Numerator);
            return value1.CompareTo(value2);
        }

        #endregion

        #region IFormattable Members

        /// <summary>
        /// Converts this instance to its equivalent string representation.
        /// </summary>
        /// <param name="format">The format to use for both the numerator and the denominator.</param>
        /// <param name="formatProvider">An object that has culture-specific formatting information.</param>
        /// <returns>The string representation of the <see cref="Rational"/>.</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return Numerator.ToString(format, formatProvider) + (Denominator != 1 ? "/" + Denominator.ToString(format, formatProvider) : string.Empty);
        }

        #endregion

        #region IEquatable<Rational> Members

        /// <summary>
        /// Indicates whether this instance and a specified <see cref="Rational"/> are equal.
        /// </summary>
        /// <param name="other">Another <see cref="Rational"/> to compare to.</param>
        /// <returns><value>true</value> if the current object and <paramref name="other"/> represent the same value; otherwise, <value>false</value>.</returns>
        public bool Equals(Rational other)
        {
            return this == other;
        }

        #endregion



        private static bool IsInteger(decimal x, decimal tolerance = DEFAULT_DECIMAL_TOLERANCE)
        {
            var fpart = Math.Abs(x % 1.0M);
            return fpart < tolerance || fpart > 1 - tolerance;
        }

        private static bool IsInteger(float x, float tolerance = DEFAULT_FLOAT_TOLERANCE)
        {
            var fpart = Math.Abs(x % 1.0);
            return fpart < tolerance || fpart > 1 - tolerance;
        }

        private static bool IsInteger(double x, double tolerance = DEFAULT_TOLERANCE)
        {
            var fpart = Math.Abs(x % 1.0);
            return fpart < tolerance || fpart > 1 - tolerance;
        }

        private static int Gcd(int x, int y)
        {
            x = Math.Abs(x);
            y = Math.Abs(y);
            while (y != 0)
            {
                var remainder = x % y;
                x = y;
                y = remainder;
            }
            return x;
        }

        private static long Gcd(long x, long y)
        {
            x = Math.Abs(x);
            y = Math.Abs(y);
            while (y != 0)
            {
                var remainder = x % y;
                x = y;
                y = remainder;
            }
            return x;
        }

        private static int Lcm(int x, int y)
        {
            int gcd = Gcd(x, y);
            return x / gcd * y;
        }

        private static long Lcm(long x, long y)
        {
            long gcd = Gcd(x, y);
            return x / gcd * y;
        }

    }
}