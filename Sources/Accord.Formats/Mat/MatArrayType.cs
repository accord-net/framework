﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

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
