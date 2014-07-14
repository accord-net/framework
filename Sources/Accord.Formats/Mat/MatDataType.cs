using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

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
