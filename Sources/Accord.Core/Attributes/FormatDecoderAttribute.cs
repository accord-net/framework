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
                if (dictionary.ContainsKey(extension))
                    return;

#if NETSTANDARD1_4
                throw new NotSupportedException("The autodiscovery of format decoders is not supported in .NET Standard 1.4. Please create a new instance of the format decoder you would like to use and use it directly instead.");
#else
                var decoderTypes = from a in AppDomain.CurrentDomain.GetAssemblies()
                                   from r in a.GetReferencedAssemblies()
                                   from t in AppDomain.CurrentDomain.Load(r).GetTypes()
                                   let attributes = t.GetCustomAttributes(typeof(FormatDecoderAttribute), true)
                                   where typeof(T).IsAssignableFrom(t)
                                   where attributes != null && attributes.Length > 0
                                   select new { Type = t, Attributes = attributes.Cast<FormatDecoderAttribute>() };

                foreach (var t in decoderTypes)
                {
                    foreach (FormatDecoderAttribute attr in t.Attributes)
                    {
                        extension = attr.Extension.ToUpperInvariant();
                        if (!dictionary.ContainsKey(extension))
                            dictionary.Add(extension, t.Type);
                    }
                }
#endif
            }
        }
    }
}
