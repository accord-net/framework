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

namespace Accord
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using Accord.Compat;

    /// <summary>
    ///   Specifies that a class can be used to decode a particular file type.
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class FormatDecoderAttribute : Attribute
    {
        /// <summary>
        ///   Gets or sets the file extension that this format decoder applies to (i.e. "wav").
        /// </summary>
        /// 
        public string Extension { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="FormatDecoderAttribute"/> class.
        /// </summary>
        /// 
        public FormatDecoderAttribute(string extension)
        {
            this.Extension = extension;
        }

        /// <summary>
        ///   Populates the dictionary with available decoders of a particular category by 
        ///   inspecting types from all referenced assemblies. Note: calling this method
        ///   will force all referenced assemblies to be loaded into the current AppDomain.
        /// </summary>
        /// 
        /// <typeparam name="T">The base type for the decoders. This should be an interface such as IImageDecoder or IAudioDecoder.</typeparam>
        /// 
        /// <param name="dictionary">The dictionary where the found decoders will be stored.</param>
        /// <param name="extension">The extension we are interested in.</param>
        /// 
        public static void PopulateDictionaryWithDecodersFromAllAssemblies<T>(Dictionary<string, Type> dictionary, string extension)
        {
            lock (dictionary)
            {
                extension = extension.ToUpperInvariant();
                if (dictionary.ContainsKey(extension))
                    return;

                var decoderTypes = new List<Tuple<Type, FormatDecoderAttribute[]>>();

#if NETSTANDARD1_4
                TypeInfo baseType = typeof(T).GetTypeInfo();

                foreach (Type t in baseType.Assembly.ExportedTypes)
                {
                    TypeInfo ti = t.GetTypeInfo();
                    var attributes = ti.GetCustomAttributes(typeof(FormatDecoderAttribute), true).ToArray();

                    if (attributes != null && attributes.Length > 0 && baseType.IsAssignableFrom(ti))
                    {
                        FormatDecoderAttribute[] at = attributes.Cast<FormatDecoderAttribute>().ToArray();
                        decoderTypes.Add(Tuple.Create(t, at));
                    }
                }
#else
                Type baseType = typeof(T);

                if (SystemTools.IsRunningOnMono())
                {
                    Console.WriteLine("FormatDecoderAttribute: Running on Mono ({0})", typeof(T));
                    foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        foreach (Type t in a.GetTypes())
                        {
                            var attributes = t.GetCustomAttributes(typeof(FormatDecoderAttribute), true);

                            if (attributes != null && attributes.Length > 0 && baseType.IsAssignableFrom(t))
                            {
                                FormatDecoderAttribute[] at = attributes.Cast<FormatDecoderAttribute>().ToArray();
                                decoderTypes.Add(Tuple.Create(t, at));
                            }
                        }
                    }
                    Console.WriteLine("FormatDecoderAttribute: Found {0} {1} decoders.", decoderTypes.Count, typeof(T));
                }
                else
                {
                    foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        foreach (AssemblyName referencedName in a.GetReferencedAssemblies())
                        {
                            Assembly referencedAssembly = Assembly.Load(referencedName);

                            foreach (Type t in referencedAssembly.GetTypes())
                            {
                                var attributes = t.GetCustomAttributes(typeof(FormatDecoderAttribute), true);

                                if (attributes != null && attributes.Length > 0 && baseType.IsAssignableFrom(t))
                                {
                                    FormatDecoderAttribute[] at = attributes.Cast<FormatDecoderAttribute>().ToArray();
                                    decoderTypes.Add(Tuple.Create(t, at));
                                }
                            }
                        }
                    }
                }
#endif

                foreach (Tuple<Type, FormatDecoderAttribute[]> pair in decoderTypes)
                {
                    foreach (FormatDecoderAttribute attr in pair.Item2)
                    {
                        extension = attr.Extension.ToUpperInvariant();
                        if (!dictionary.ContainsKey(extension))
                            dictionary.Add(extension, pair.Item1);
                    }
                }
            }
        }
    }
}
