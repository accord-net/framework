// Accord Control Library
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

namespace Accord.Controls
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using AForge;
    using System.Collections;

    /// <summary>
    ///   Type converter for <see cref="DoubleRange"/>
    ///   and <see cref="IntRange"/> objects.
    /// </summary>
    /// 
    public class RangeTypeConverter : TypeConverter
    {

        /// <summary>
        ///   Returns true.
        /// </summary>
        /// 
        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        ///   Creates an instance of the type that this <see cref="T:System.ComponentModel.TypeConverter"/> is associated with, using the specified context, given a set of property values for the object.
        /// </summary>
        /// 
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="propertyValues">An <see cref="T:System.Collections.IDictionary"/> of new property values.</param>
        /// 
        /// <returns>
        /// An <see cref="T:System.Object"/> representing the given <see cref="T:System.Collections.IDictionary"/>, or null if the object cannot be created. This method always returns null.
        /// </returns>
        /// 
        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            if (propertyValues != null)
            {
                if (context.PropertyDescriptor.PropertyType == typeof(DoubleRange))
                {
                    double max = (double)propertyValues["Max"];
                    double min = (double)propertyValues["Min"];

                    return new DoubleRange(min, max);
                }
                else if (context.PropertyDescriptor.PropertyType == typeof(IntRange))
                {
                    int max = (int)propertyValues["Max"];
                    int min = (int)propertyValues["Min"];

                    return new IntRange(min, max);
                }
            }


            return base.CreateInstance(context, propertyValues);
        }

        /// <summary>
        ///   Returns true.
        /// </summary>
        /// 
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Returns a collection of properties for the type of array specified by the value parameter, using the specified context and attributes.
        /// </summary>
        /// 
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="value">An <see cref="T:System.Object"/> that specifies the type of array for which to get properties.</param>
        /// <param name="attributes">An array of type <see cref="T:System.Attribute"/> that is used as a filter.</param>
        /// 
        /// <returns>
        /// A <see cref="T:System.ComponentModel.PropertyDescriptorCollection"/> with the properties that are exposed for this data type, or null if there are no properties.
        /// </returns>
        /// 
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(value);
        }

        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// 
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type"/> that represents the type you want to convert from.</param>
        /// 
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        /// 
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(DoubleRange) ||
                sourceType == typeof(IntRange))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// 
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="destinationType">A <see cref="T:System.Type"/> that represents the type you want to convert to.</param>
        /// 
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        /// 
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(String))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// 
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"/>. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
        /// <param name="destinationType">The <see cref="T:System.Type"/> to convert the <paramref name="value"/> parameter to.</param>
        /// 
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// 
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="destinationType"/> parameter is null.
        /// </exception>
        /// 
        /// <exception cref="T:System.NotSupportedException">
        /// The conversion cannot be performed.
        /// </exception>
        /// 
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture,
            object value, Type destinationType)
        {
            if (value is DoubleRange)
            {
                DoubleRange doubleRange = (DoubleRange)value;
                return String.Format(CultureInfo.CurrentCulture,
                    "[{0}; {1}]", doubleRange.Min, doubleRange.Max);
            }

            if (value is IntRange)
            {
                IntRange intRange = (IntRange)value;
                return String.Format(CultureInfo.CurrentCulture,
                    "[{0}; {1}]", intRange.Min, intRange.Max);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        ///   Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// 
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"/> to use as the current culture.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
        /// 
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// 
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        /// 
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                return base.ConvertFrom(context, culture, value);

            String str = value as String;

            if (str == null)
                return base.ConvertFrom(context, culture, value);

            str = str.Replace("(", String.Empty);
            str = str.Replace(")", String.Empty);
            str = str.Replace("[", String.Empty);
            str = str.Replace("]", String.Empty);

            String[] parts = str.Split(';');
            string a = parts[0];
            string b = parts[1];

            if (context.PropertyDescriptor.PropertyType == typeof(IntRange))
            {
                int max = int.Parse(a);
                int min = int.Parse(b);
                return new IntRange(min, max);

            }
            else if (context.PropertyDescriptor.PropertyType == typeof(DoubleRange))
            {
                double max = double.Parse(a);
                double min = double.Parse(b);
                return new DoubleRange(min, max);
            }
            

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        ///   Assign this converter to the AForge types. This method
        ///   should be called before an AForge range type can be
        ///   bound to controls such as the PropertyGrid.
        /// </summary>
        /// 
        public static void Assign()
        {
            TypeDescriptor.AddAttributes(typeof(DoubleRange), new TypeConverterAttribute(typeof(RangeTypeConverter)));
            TypeDescriptor.AddAttributes(typeof(IntRange), new TypeConverterAttribute(typeof(RangeTypeConverter)));
        }
    }
}
