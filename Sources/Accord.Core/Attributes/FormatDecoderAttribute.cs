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
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class FormatDecoderAttribute : FormatHandlerAttribute
    {

        /// <summary>
        ///   Initializes a new instance of the <see cref="FormatDecoderAttribute"/> class.
        /// </summary>
        /// 
        public FormatDecoderAttribute(string extension)
            : base(extension)
        {
        }


        /// <summary>
        ///   Finds a decoder that can process the given normalized file extension.
        /// </summary>
        /// 
        /// <typeparam name="TDecoder">The type of the the decoder to be found (e.g. IImageDecoder or IAudioDecoder).</typeparam>
        /// <param name="fileExtension">The normalized file extension (<see cref="FormatHandlerAttribute.GetNormalizedExtension(string)"/>.</param>
        /// <param name="decoderTypes">The decoder types.</param>
        /// <param name="cache">The cache of already instantiated decoder types.</param>
        /// 
        /// <returns>A decoder implementing the <typeparamref name="TDecoder"/> interface, or null if none have been found.</returns>
        /// 
        public static TDecoder GetDecoders<TDecoder>(string fileExtension,
            Dictionary<string, Type> decoderTypes, Dictionary<string, TDecoder> cache)
            where TDecoder : class
        {
            return GetHandler<TDecoder, FormatDecoderAttribute>(fileExtension, decoderTypes, cache);
        }
    }
}
