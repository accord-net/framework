// Accord Formats Library
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

namespace Accord.IO
{
    using Accord.Math;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;
    using System.IO.Compression;
    using System.Collections;

    /// <summary>
    ///   Lazily-loaded collection of arrays from a compressed .npz archive.
    /// </summary>
    /// 
    /// <seealso cref="NpyFormat"/>
    /// <seealso cref="NpzFormat"/>
    /// 
    public class NpzDictionary : NpzDictionary<Array>
    {
        bool jagged;

        /// <summary>
        ///   Initializes a new instance of the <see cref="NpzDictionary{T}"/> class.
        /// </summary>
        /// 
        /// <param name="stream">The stream from where the arrays should be loaded from.</param>
        /// <param name="jagged">Pass true to deserialize matrices as jagged matrices. Pass false
        ///   to deserialize them as multi-dimensional matrices.</param>
        /// 
        public NpzDictionary(Stream stream, bool jagged)
            : base(stream)
        {
            this.jagged = jagged;
        }

        /// <summary>
        ///   Loads the array from the specified stream.
        /// </summary>
        /// 
        protected override Array Load(Stream s)
        {
            if (jagged)
                return NpyFormat.LoadJagged(s);
            return NpyFormat.LoadMatrix(s);
        }

        /*
        public static bool TryRead(Stream stream, bool jagged, out NpzDictionary dictionary)
        {
            long offset = stream.Position;
            dictionary = null;

            try
            {
                dictionary = new NpzDictionary(stream, jagged);
                return true;
            }
            catch
            {
                stream.Position = offset;
            }

            return false;
        }

        public static bool TryRead<T>(Stream stream, bool jagged, out NpzDictionary<T> dictionary)
            where T : class, ICloneable, IList, ICollection, IEnumerable, IStructuralComparable, IStructuralEquatable
        {
            long offset = stream.Position;
            dictionary = null;

            try
            {
                dictionary = new NpzDictionary<T>(stream);
                return true;
            }
            catch
            {
                stream.Position = offset;
            }

            return false;
        }
        */

    }
}
