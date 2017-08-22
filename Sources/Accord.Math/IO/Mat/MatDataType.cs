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

    internal enum MatDataType : short
    {
        /// <summary>
        ///   8 bit, signed
        /// </summary>
        /// 
        miINT8 = 1,

        /// <summary>
        ///   8 bit, unsigned
        ///</summary>
        /// 
        miUINT8 = 2,

        /// <summary>
        ///   16-bit, signed
        /// </summary>
        /// 
        miINT16 = 3,

        /// <summary>
        ///   16-bit, unsigned
        /// </summary>
        /// 
        miUINT16 = 4,

        /// <summary>
        ///   32-bit, signed
        /// </summary>
        /// 
        miINT32 = 5,

        /// <summary>
        ///   32-bit, unsigned
        /// </summary>
        /// 
        miUINT32 = 6,

        /// <summary>
        ///   IEEE® 754 single format
        /// </summary>
        /// 
        miSINGLE = 7,

        /// <summary>
        ///   IEEE 754 double format
        /// </summary>
        /// 
        miDOUBLE = 9,

        /// <summary>
        ///   64-bit, signed
        /// </summary>
        /// 
        miINT64 = 12,

        /// <summary>
        ///   64-bit, unsigned
        /// </summary>
        /// 
        miUINT64 = 13,

        /// <summary>
        ///   MATLAB array
        /// </summary>
        /// 
        miMATRIX = 14,

        /// <summary>
        ///   Compressed Data
        /// </summary>
        /// 
        miCOMPRESSED = 15,

        /// <summary>
        ///   Unicode UTF-8 Encoded Character Data
        /// </summary>
        /// 
        miUTF8 = 16,

        /// <summary>
        ///   Unicode UTF-16 Encoded Character Data
        /// </summary>
        /// 
        miUTF16 = 17,

        /// <summary>
        ///   Unicode UTF-32 Encoded Character Data
        /// </summary>
        /// 
        miUTF32 = 18,
    }
}
