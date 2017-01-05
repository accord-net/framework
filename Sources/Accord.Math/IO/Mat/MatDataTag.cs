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
    |         data type          |          number of bytes         |
    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    |                                                               |
    ~                       variable size                           ~
    |                                                               |
    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

                 MAT-file data element format (v5)
                 8 bytes header / variable payload
         
          
     0   |   1   |   2   |   3   |   4   |   5   |   6   |   7    
    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    |   number of bytes  |   data type   |           data           |
    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

                MAT-file small data element format (v5)
                    8 bytes header + fixed payload
         
 */
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 8)]
    internal unsafe struct MatDataTag
    {
        [FieldOffset(0)]
        public MatDataType DataType;

        [FieldOffset(4)]
        public Int32 NumberOfBytes;

        [FieldOffset(2)]
        public Int16 SmallData_NumberOfBytes;

        [FieldOffset(0)]
        public MatDataType SmallData_Type;

        [FieldOffset(4)]
        public fixed byte SmallData_Value[4];

        public bool IsSmallFormat { get { return SmallData_NumberOfBytes != 0; } }

    }
}
