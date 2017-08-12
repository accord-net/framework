// Accord Core Library
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

#if NETSTANDARD1_4
namespace Accord.Compat
{
    using System;
    using System.Reflection;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Threading;
    using System.Globalization;
    using System.Threading.Tasks;

    internal static class DBNull
    {
        public const object Value = null;
    }

    internal static class Trace
    {
        public static void Write(string value)
        {
            Console.Write(value);
        }

        public static void WriteLine(string value)
        {
            Console.WriteLine(value);
        }

        public static void TraceWarning(string value, params object[] obj)
        {
            Console.WriteLine(String.Format(value, obj));
        }

    }

    internal class ResolveEventArgs
    {
        public string Name { get; internal set; }
    }

    /// <summary>
    ///   Minimum SerializationInfo implementation to make Accord.NET compile under NetStandard 1.4.
    /// </summary>
    /// 
    public class SerializationBinder
    {
        /// <summary>
        ///   Minimum SerializationInfo implementation to make Accord.NET compile under NetStandard 1.4.
        /// </summary>
        /// 
        public virtual Type BindToType(string assemblyName, string typeName)
        {
            throw new NotImplementedException();
        }
    }

    internal interface IDeserializationCallback
    {
        void OnDeserialization(object obj);
    }

    /// <summary>
    ///   Minimum SerializationInfo implementation to make Accord.NET compile under NetStandard 1.4.
    /// </summary>
    /// 
    public class SerializationInfo
    {
        /// <summary>
        /// Performs an implicit conversion from SerializationInfo to <see cref="System.String" />.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator string(SerializationInfo info)
        {
            return info.ToString();
        }
    }

    /// <summary>
    ///   Minimum StreamingContext implementation to make Accord.NET compile under NetStandard 1.4.
    /// </summary>
    /// 
    public class StreamingContext
    {
        /// <summary>
        /// Performs an implicit conversion from SerializationInfo to <see cref="System.String" />.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Exception(StreamingContext info)
        {
            return new Exception();
        }
    }

    /// <summary>
    ///   Minimum ReadOnlyException implementation to make Accord.NET compile under NetStandard 1.4.
    /// </summary>
    /// 
    public class ReadOnlyException : System.Exception
    {
        /// <summary>
        ///   Minimum ReadOnlyException implementation to make Accord.NET compile under NetStandard 1.4.
        /// </summary>
        /// 
        public ReadOnlyException()
        {

        }

        /// <summary>
        ///   Minimum ReadOnlyException implementation to make Accord.NET compile under NetStandard 1.4.
        /// </summary>
        /// 
        public ReadOnlyException(string str)
        {

        }
    }

    internal static class SMath
    {
        internal static int DivRem(int xNum, int yNum, out int rem)
        {
            throw new NotImplementedException();
        }

        internal static long BigMul(int numerator, int denominator)
        {
            throw new NotImplementedException();
        }
    }

    internal static class CultureInfo
    {
        public static IFormatProvider InvariantCulture = System.Globalization.CultureInfo.InvariantCulture;

        public static System.Globalization.CultureInfo CurrentCulture = System.Globalization.CultureInfo.CurrentCulture;

        public static System.Globalization.CultureInfo GetCultureInfo(string a)
        {
            return new System.Globalization.CultureInfo(a);
        }

        internal static System.Globalization.CultureInfo CreateSpecificCulture(string v)
        {
            return new System.Globalization.CultureInfo(v);
        }
    }

    internal class Encoding
    {
        public static System.Text.Encoding Default = System.Text.Encoding.UTF8;
    }

    /// <summary>
    ///   Minimum ProgressChangedEventArgs implementation to make Accord.NET compile under NetStandard 1.4.
    /// </summary>
    /// 
    public class ProgressChangedEventArgs : EventArgs
    {
        /// <summary>
        ///   Minimum ProgressChangedEventArgs implementation to make Accord.NET compile under NetStandard 1.4.
        /// </summary>
        /// 
        public ProgressChangedEventArgs()
        {
        }

        /// <summary>
        ///   Minimum ProgressChangedEventArgs implementation to make Accord.NET compile under NetStandard 1.4.
        /// </summary>
        /// 
        public ProgressChangedEventArgs(int a, int b)
        {
        }

        /// <summary>
        ///   Minimum ProgressChangedEventArgs implementation to make Accord.NET compile under NetStandard 1.4.
        /// </summary>
        /// 
        public ProgressChangedEventArgs(int a, EventArgs e)
        {
        }
    }

    /// <summary>
    ///   Minimum ITypeDescriptorContext implementation to make Accord.NET compile under NetStandard 1.4.
    /// </summary>
    /// 
    public interface ITypeDescriptorContext
    {
    }

    internal class TypeConverterAttribute : Attribute
    {
        public TypeConverterAttribute(Type type) { }
    }

    internal class TypeConverter
    {
        public virtual bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            throw new NotImplementedException("This implementation is not available in .Net Standard 1.4.");
        }

        public virtual object ConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            throw new NotImplementedException("This implementation is not available in .Net Standard 1.4.");
        }

        public virtual object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            throw new NotImplementedException("This implementation is not available in .Net Standard 1.4.");
        }

        public virtual bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            throw new NotImplementedException("This implementation is not available in .Net Standard 1.4.");
        }

        public virtual object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            throw new NotImplementedException("This implementation is not available in .Net Standard 1.4.");
        }
    }

    /// <summary>
    ///   Minimum ICloneable implementation to make Accord.NET compile under NetStandard 1.4.
    /// </summary>
    /// 
    public interface ICloneable
    {
        /// <summary>
        ///   Minimum ICloneable implementation to make Accord.NET compile under NetStandard 1.4.
        /// </summary>
        /// 
        object Clone();
    }

    /// <summary>
    ///   Minimum ICloneable implementation to make Accord.NET compile under NetStandard 1.4.
    /// </summary>
    /// 
    public static class CloneExtensions
    {
        /// <summary>
        ///   Minimum ICloneable implementation to make Accord.NET compile under NetStandard 1.4.
        /// </summary>
        /// 
        public static object Clone(this object obj)
        {
            var arr = obj as Array;
            if (arr != null)
                return arr.Clone();

            var type = obj.GetType().GetTypeInfo();

            if (type.IsValueType)
                return obj;

            var clone = type.GetDeclaredMethods("Clone").Where(x => x.GetParameters().Length == 0).First();
            return clone.Invoke(obj, new object[] { });
        }
    }

    internal static class StreamExtensions
    {
        public static void Close(this Stream e)
        {
            e.Dispose();
        }

        public static void Close(this TextReader t)
        {
            t.Dispose();
        }
    }

    internal static class ManualResetEventExtensions
    {
        public static void Close(this ManualResetEvent e)
        {
            e.Dispose();
        }

        public static bool WaitOne(this ManualResetEvent e, int i, bool b)
        {
            return e.WaitOne(i);
        }
    }

    internal static class TaskExtensions
    {
        public static void Dispose(this Task e)
        {
            e.Dispose();
        }
    }

    internal class DescriptionAttribute : Attribute
    {
        public DescriptionAttribute(string description)
        {
            this.Description = description;
        }

        public string Description { get; internal set; }
    }

    internal class DisplayNameAttribute : Attribute
    {
        public DisplayNameAttribute(string category) { }
    }

    internal class BrowsableAttribute : Attribute
    {
        public BrowsableAttribute(bool value) { }
    }

    internal class CategoryAttribute : Attribute
    {
        public CategoryAttribute(string category) { }
    }



    internal class NonSerializedAttribute : Attribute { }

    internal class SerializableAttribute : Attribute { }

    internal class OnDeserializedAttribute : Attribute { }

    internal class OnDeserializingAttribute : Attribute { }

    internal class BinaryFormatter
    {
        public object Binder { get; internal set; }

        internal void Serialize<T>(GZipStream gzip, T obj)
        {
            throw new NotImplementedException();
        }

        internal void Serialize<T>(Stream stream, T obj)
        {
            throw new NotImplementedException();
        }

        internal object Deserialize(GZipStream gzip)
        {
            throw new NotImplementedException();
        }

        internal object Deserialize(Stream stream)
        {
            throw new NotImplementedException();
        }
    }

    internal class AppDomain
    {
        public static AppDomain CurrentDomain { get; internal set; }

        public Func<object, ResolveEventArgs, Assembly> AssemblyResolve { get; internal set; }
    }
}
#endif