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

    internal enum MatArrayType : byte
    {
        /// <summary>
        ///   Cell array
        /// </summary>
        /// 
        mxCELL_CLASS = 1,

        /// <summary>
        ///   Structure 
        /// </summary>
        /// 
        mxSTRUCT_CLASS = 2,

        /// <summary>
        ///   Object 
        /// </summary>
        /// 
        mxOBJECT_CLASS = 3,

        /// <summary>
        ///   Character array
        /// </summary>
        /// 
        mxCHAR_CLASS = 4,

        /// <summary>
        ///   Sparse array 
        /// </summary>
        /// 
        mxSPARSE_CLASS = 5,

        /// <summary>
        ///   Double precision array 
        /// </summary>
        /// 
        mxDOUBLE_CLASS = 6,

        /// <summary>
        ///   Single precision array 
        /// </summary>
        /// 
        mxSINGLE_CLASS = 7,

        /// <summary>
        ///   8-bit, signed integer 
        /// </summary>
        /// 
        mxINT8_CLASS = 8,

        /// <summary>
        ///   8-bit, unsigned integer 
        /// </summary>
        /// 
        mxUINT8_CLASS = 9,

        /// <summary>
        ///   16-bit, signed integer 
        /// </summary>
        /// 
        mxINT16_CLASS = 10,

        /// <summary>
        ///   16-bit, unsigned integer 
        /// </summary>
        /// 
        mxUINT16_CLASS = 11,

        /// <summary>
        ///   32-bit, signed integer 
        /// </summary>
        /// 
        mxINT32_CLASS = 12,

        /// <summary>
        ///   32-bit, unsigned integer 
        /// </summary>
        /// 
        mxUINT32_CLASS = 13,

        /// <summary>
        ///   64-bit, signed integer 
        /// </summary>
        /// 
        mxINT64_CLASS = 14,

        /// <summary>
        ///   64-bit, unsigned integer 
        /// </summary>
        /// 
        mxUINT64_CLASS = 15,
    }

}
