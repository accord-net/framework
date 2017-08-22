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
    using System;

    /// <summary>
    ///   Sparse matrix representation used by
    ///   <see cref="MatReader">.MAT files</see>.
    /// </summary>
    /// 
    public class MatSparse
    {
        /// <summary>
        ///   Gets the sparse row index vector.
        /// </summary>
        /// 
        public int[] Rows { get; private set; }

        /// <summary>
        ///   Gets the sparse column index vector.
        /// </summary>
        /// 
        public int[] Columns { get; private set; }

        /// <summary>
        ///   Gets the values vector.
        /// </summary>
        /// 
        public Array Values { get; private set; }

        internal MatSparse(int[] ir, int[] ic, Array values)
        {
            Rows = ir;
            Columns = ic;
            Values = values;
        }
    }
}
