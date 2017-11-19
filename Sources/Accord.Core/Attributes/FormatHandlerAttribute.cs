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
    using System.IO;

    /// <summary>
    ///   Specifies that a class can be used to decode a particular file type.
    /// </summary>
    /// 
    public abstract class FormatHandlerAttribute : Attribute
    {
        /// <summary>
        ///   Gets or sets the file extension that this format decoder applies to (i.e. "wav").
        /// </summary>
        /// 
        public string Extension { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="FormatHandlerAttribute"/> class.
        /// </summary>
        /// 
        public FormatHandlerAttribute(string extension)
        {
            this.Extension = extension;
        }

        /// <summary>
        ///   Extracts and normalizes a file extension in the same format that is used when 
        ///   specifying which formats are supported by each decoder (e.g. "file.wav" would
        ///   become "WAV", "image.png" would become "PNG", etc).
        /// </summary>
        /// 
        public static string GetNormalizedExtension(string fileName)
        {
            string fileExtension = Path.GetExtension(fileName).ToUpperInvariant();
            if (!String.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.Substring(1);
            return fileExtension;
        }

        /// <summary>
        ///   Finds a file handler that can process the given normalized file extension.
        /// </summary>
        /// 
        /// <typeparam name="TInterface">The type of the the handler to be found (e.g. IImageDecoder or IAudioDecoder).</typeparam>
        /// <typeparam name="TAttribute">The type of the attribute to be found (e.g. <see cref="FormatDecoderAttribute"/> or <see cref="FormatEncoderAttribute"/>).</typeparam>
        /// <param name="fileExtension">The normalized file extension (<see cref="GetNormalizedExtension(string)"/>.</param>
        /// <param name="handlerTypes">The handler types.</param>
        /// <param name="cache">The cache of already instantiated handler types.</param>
        /// 
        /// <returns>A file decoder or encoder implementing the <typeparamref name="TInterface"/> interface, or null if none have been found.</returns>
        /// 
        public static TInterface GetHandler<TInterface, TAttribute>(string fileExtension,
            Dictionary<string, Type> handlerTypes, Dictionary<string, TInterface> cache)
            where TInterface : class
            where TAttribute : FormatHandlerAttribute
        {
            TInterface handler = null;

            if (cache.TryGetValue(fileExtension, out handler))
                return handler;

            if (!handlerTypes.ContainsKey(fileExtension))
                PopulateDictionaryWithHandlersFromAllAssemblies<TInterface, TAttribute>(handlerTypes);

            Type t;
            if (handlerTypes.TryGetValue(fileExtension, out t) && t != null)
                cache[fileExtension] = handler = (TInterface)Activator.CreateInstance(t);

            return handler;
        }

        /// <summary>
        ///   Populates the dictionary with available decoders of a particular category by 
        ///   inspecting types from all referenced assemblies. Note: calling this method
        ///   will force all referenced assemblies to be loaded into the current AppDomain.
        /// </summary>
        /// 
        /// <typeparam name="TInterface">The base type for the decoders. This should be an interface such as IImageDecoder or IAudioDecoder.</typeparam>
        /// <typeparam name="TAttribute">The type of the attribute to be found (e.g. <see cref="FormatDecoderAttribute"/> or <see cref="FormatEncoderAttribute"/>).</typeparam>
        /// 
        /// <param name="dictionary">The dictionary where the found decoders will be stored.</param>
        /// 
        public static void PopulateDictionaryWithHandlersFromAllAssemblies<TInterface, TAttribute>(Dictionary<string, Type> dictionary)
            where TAttribute : FormatHandlerAttribute
        {
            lock (dictionary)
            {
                var handlerTypes = new List<Tuple<Type, TAttribute[]>>();

                Type attrType = typeof(TAttribute);
                Type baseType = typeof(TInterface);

#if NETSTANDARD1_4
                TypeInfo baseTypeInfo = typeof(TInterface).GetTypeInfo();

                foreach (Type t in baseTypeInfo.Assembly.ExportedTypes)
                {
                    TypeInfo ti = t.GetTypeInfo();
                    object[] attributes = ti.GetCustomAttributes(baseType, true).ToArray();

                    if (attributes != null && attributes.Length > 0 && baseTypeInfo.IsAssignableFrom(ti))
                    {
                        TAttribute[] at = attributes.Cast<TAttribute>().ToArray();
                        handlerTypes.Add(Tuple.Create(t, at));
                    }
                }
#else
                foreach (Assembly parent in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        foreach (Type t in parent.GetTypes())
                        {
                            object[] attributes = t.GetCustomAttributes(attrType, true);

                            if (attributes != null && attributes.Length > 0 && baseType.IsAssignableFrom(t))
                            {
                                TAttribute[] at = attributes.Cast<TAttribute>().ToArray();
                                handlerTypes.Add(Tuple.Create(t, at));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex is System.Reflection.ReflectionTypeLoadException)
                        {
                            var typeLoadException = ex as ReflectionTypeLoadException;
                            Exception[] loaderExceptions = typeLoadException.LoaderExceptions;
                            foreach (var lex in loaderExceptions)
                                throw lex;
                        }
                    }
                }
#endif
                Console.WriteLine("FormatHandlerAttribute: Found {0} {1} decoders.", handlerTypes.Count, typeof(TInterface));

                foreach (Tuple<Type, TAttribute[]> pair in handlerTypes)
                {
                    foreach (TAttribute attr in pair.Item2)
                    {
                        string extension = attr.Extension.ToUpperInvariant();
                        if (!dictionary.ContainsKey(extension))
                            dictionary.Add(extension, pair.Item1);
                    }
                }
            }
        }

    }
}
