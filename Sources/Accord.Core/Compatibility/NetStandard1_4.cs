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
    using System.Collections.Generic;
    using System.Text;
    using System.Net.Http;

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

    [Flags]
    internal enum BindingFlags
    {
        Default = 0,
        IgnoreCase = 1,
        DeclaredOnly = 2,
        Instance = 4,
        Static = 8,
        Public = 16,
        NonPublic = 32,
        FlattenHierarchy = 64,
        InvokeMethod = 256,
        CreateInstance = 512,
        GetField = 1024,
        SetField = 2048,
        GetProperty = 4096,
        SetProperty = 8192,
        PutDispProperty = 16384,
        PutRefDispProperty = 32768,
        ExactBinding = 65536,
        SuppressChangeType = 131072,
        OptionalParamBinding = 262144,
        IgnoreReturn = 16777216
    }

    internal static partial class TypeExtensions
    {
        public static bool IsInstanceOfType(this Type type, object obj)
        {
            return obj != null && type.GetTypeInfo().IsAssignableFrom(obj.GetType().GetTypeInfo());
        }

        private static bool check(MethodBase a, BindingFlags flags)
        {
            if (flags.HasFlag(BindingFlags.NonPublic) && a.IsPublic)
                return false;
            if (flags.HasFlag(BindingFlags.Public) && !a.IsPublic)
                return false;
            if (flags.HasFlag(BindingFlags.Static) && !a.IsStatic)
                return false;
            return true;
        }

        private static bool check(FieldInfo a, BindingFlags flags)
        {
            if (flags.HasFlag(BindingFlags.NonPublic) && a.IsPublic)
                return false;
            if (flags.HasFlag(BindingFlags.Public) && !a.IsPublic)
                return false;
            if (flags.HasFlag(BindingFlags.Static) && !a.IsStatic)
                return false;
            return true;
        }

        public static MethodInfo[] GetMethods(this Type type, BindingFlags flags)
        {
            var methods = new List<MethodInfo>();
            foreach (var method in type.GetTypeInfo().DeclaredMethods)
                if (check(method, flags))
                    methods.Add(method);
            return methods.ToArray();
        }

        public static FieldInfo GetField(this Type type, string name, BindingFlags flags)
        {
            var field = type.GetTypeInfo().GetDeclaredField(name);
                if (check(field, flags))
                    return field;
            return null;
        }
    }

    internal class ResolveEventArgs
    {
        public string Name { get; internal set; }
    }

    /// <summary>
    ///   Minimum SerializationBinder implementation to make Accord.NET compile under NetStandard 1.4.
    /// </summary>
    /// 
    public class SerializationBinder
    {
        /// <summary>
        ///   Minimum BindToType implementation to make Accord.NET compile under NetStandard 1.4.
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
        ///   Minimum SerializationInfo implementation to make Accord.NET compile under NetStandard 1.4.
        /// </summary>
        /// 
        public object GetValue(string name, Type t)
        {
            throw new NotSupportedException("Not supported in .NET Standard 1.4.");
        }

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

    internal class CultureInfoEx
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

    internal class OptionalFieldAttribute : Attribute { }

    internal class OnDeserializedAttribute : Attribute { }

    internal class OnDeserializingAttribute : Attribute { }

    internal class SurrogateSelectionAttribute : Attribute { }

    internal class BinaryFormatter
    {
        public object Binder { get; internal set; }

        public SurrogateSelector SurrogateSelector { get; set; }

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

    internal class SurrogateSelector
    {
    }

    internal class AppDomain
    {
        public static AppDomain CurrentDomain { get; internal set; }

        public Func<object, ResolveEventArgs, Assembly> AssemblyResolve { get; internal set; }

        internal Assembly Load(string name)
        {
            throw new NotImplementedException();
        }
    }

    internal class WebClient : IDisposable
    {
        public Dictionary<string, string> Headers = new Dictionary<string, string>();

        public WebClient()
        {

        }

        public void Dispose()
        {
        }

        public void DownloadFile(string url, string filename)
        {
            DownloadFileAsync(new Uri(url), filename).RunSynchronously();
        }

        public Task DownloadFileAsync(Uri requestUri, string filename)
        {
            using (Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
                return DownloadDataAsync(requestUri, stream);
        }

        public byte[] DownloadData(string url)
        {
            MemoryStream stream = new MemoryStream();
            DownloadDataAsync(new Uri(url), stream).RunSynchronously();
            return stream.ToArray();
        }

        public string DownloadString(string url)
        {
            byte[] bytes = DownloadData(url);
            return ASCIIEncoding.ASCII.GetString(bytes, 0, bytes.Length);
        }

        public async Task DownloadDataAsync(Uri requestUri, Stream stream)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, requestUri))
                    {
                        foreach (var h in Headers)
                            request.Headers.Add(h.Key, h.Value);

                        using (Stream contentStream = await (await httpClient.SendAsync(request)).Content.ReadAsStreamAsync())
                        {
                            await contentStream.CopyToAsync(stream);
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                throw new WebException(ex);
            }
        }
    }

    internal class WebException : HttpRequestException
    {
        public WebException(HttpRequestException ex)
            : base("HttpRequestException", ex)
        {
        }
    }
}
#endif