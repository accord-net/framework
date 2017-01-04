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
    using System.Runtime.InteropServices;

    /*
          
             
            0   |   1   |   2   |   3   |   4   |   5   |   6   |   7    
        +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        |                                                               |   8
        |                                                               |  16
        |                                                               |  24
        |                                                               |  32
        |                                                               |  40 
        |                                                               |  48
        |                                                               |  56
        |                         Descriptive text                      |  64
        |                            (116 bytes)                        |  72     
        |                                                               |  80
        |                                                               |  88
        |                                                               |  96
        |                                                               | 104
        |                                                               | 112
        |                               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-|
        |                               |       subsys data offset      | 120
        +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        |       subsys data offset      |  version    |   endian ind.   |
        +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

                     MAT-file header format (v5) - 128 bytes
         
        */
    [StructLayout(LayoutKind.Sequential, Size = 128)]
    internal struct MatHeader
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 124)]
        public String TextField;

        public Int32 SubsystemDataOffset;

        public Int16 Version;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
        public String Endian;
    }
}
