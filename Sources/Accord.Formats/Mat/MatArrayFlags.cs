using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Accord.IO
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 8)]
    internal struct ArrayFlags
    {
        [FieldOffset(0)]
        public MatArrayType Class;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        [FieldOffset(1)]
        public ArrayFlagsType Flags;


        [FieldOffset(4)]
        public int NonZeroElements;
    }

    [Flags]
    internal enum ArrayFlagsType : byte
    {
        None = 0,
        Logical = 2,
        Global = 4,
        Complex = 8,
    }

}
