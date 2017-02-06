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

namespace Accord.Math.Converters
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    /// <summary>
    /// Converts to and from the <see cref="Rational"/> type.
    /// </summary>
    /// 
    public class RationalConverter : TypeConverter
    {
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type" /> that represents the type you want to convert from.</param>
        /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            sourceType = GetUnderlyingType(sourceType);
            return sourceType == typeof(Rational)
                || sourceType == typeof(string)
                || sourceType == typeof(int)
                || sourceType == typeof(double)
                || sourceType == typeof(long)
                || sourceType == typeof(short)
                || sourceType == typeof(uint)
                || sourceType == typeof(ulong)
                || sourceType == typeof(ushort)
                || sourceType == typeof(float)
                || sourceType == typeof(decimal)
                || sourceType == typeof(byte)
                || sourceType == typeof(sbyte)
                || sourceType == typeof(bool);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo" /> to use as the current culture.</param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert.</param>
        /// <returns>An <see cref="T:System.Object" /> that represents the converted value.</returns>
        /// <exception cref="System.ArgumentException">Inavlid value type: " + value.GetType().FullName - value</exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                return (Rational?)null;

            if (value is Rational)
                return (Rational)value;
            if (value is string)
                return Rational.Parse((string)value, culture);
            if (value is int)
                return new Rational((int)value);
            if (value is double)
                return Rational.FromDouble((double)value);
            if (value is long)
                return (Rational)(long)value;
            if (value is short)
                return (Rational)(short)value;
            if (value is uint)
                return (Rational)(uint)value;
            if (value is ulong)
                return (Rational)(ulong)value;
            if (value is ushort)
                return (Rational)(ushort)value;
            if (value is float)
                return (Rational)(float)value;
            if (value is byte)
                return (Rational)(byte)value;
            if (value is sbyte)
                return (Rational)(sbyte)value;
            if (value is bool)
                return (bool)value ? Rational.One : Rational.Zero;

            throw new ArgumentException("Inavlid value type: " + value.GetType().FullName, "value");
        }

        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="destinationType">A <see cref="T:System.Type" /> that represents the type you want to convert to.</param>
        /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            destinationType = GetUnderlyingType(destinationType);
            return destinationType == typeof(Rational)
                || destinationType == typeof(string)
                || destinationType == typeof(int)
                || destinationType == typeof(double)
                || destinationType == typeof(long)
                || destinationType == typeof(short)
                || destinationType == typeof(uint)
                || destinationType == typeof(ulong)
                || destinationType == typeof(ushort)
                || destinationType == typeof(float)
                || destinationType == typeof(decimal)
                || destinationType == typeof(byte)
                || destinationType == typeof(sbyte)
                || destinationType == typeof(bool);
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" />. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert.</param>
        /// <param name="destinationType">The <see cref="T:System.Type" /> to convert the <paramref name="value" /> parameter to.</param>
        /// <returns>An <see cref="T:System.Object" /> that represents the converted value.</returns>
        /// <exception cref="System.InvalidCastException">Cannot convert null to type " + destinationType.FullName</exception>
        /// <exception cref="System.ArgumentException">
        /// value must be a rational. - value
        /// or
        /// Inavlid destinationType: " + destinationType.FullName - destinationType
        /// </exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value == null)
            {
                if (IsNullableType(destinationType))
                    return null;

                throw new InvalidCastException("Cannot convert null to type " + destinationType.FullName);
            }
            if (!(value is Rational))
                throw new ArgumentException("value must be a rational.", "value");

            var r = (Rational)value;
            if (destinationType == typeof(Rational))
                return r;
            if (destinationType == typeof(string))
                return r.ToString(culture);
            if (destinationType == typeof(int))
                return Rational.Round(r);
            if (destinationType == typeof(double))
                return r.Value;
            if (destinationType == typeof(long))
                return (long)Rational.Round(r);
            if (destinationType == typeof(short))
                return (short)Rational.Round(r);
            if (destinationType == typeof(uint))
                return (uint)Rational.Round(r);
            if (destinationType == typeof(ulong))
                return (ulong)Rational.Round(r);
            if (destinationType == typeof(ushort))
                return (ushort)Rational.Round(r);
            if (destinationType == typeof(float))
                return (float)r;
            if (destinationType == typeof(byte))
                return (byte)Rational.Round(r);
            if (destinationType == typeof(sbyte))
                return (sbyte)Rational.Round(r);
            if (destinationType == typeof(bool))
                return !Rational.IsZero(r);

            throw new ArgumentException("Inavlid destinationType: " + destinationType.FullName, "destinationType");
        }

        private static bool IsNullableType(Type t)
        {
            return (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        private static Type GetUnderlyingType(Type t)
        {
            return IsNullableType(t) ? Nullable.GetUnderlyingType(t) : t;
        }
    }
}
