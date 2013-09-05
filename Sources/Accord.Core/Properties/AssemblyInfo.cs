using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Accord.Core")]
[assembly: AssemblyDescription("Accord.NET - Core Library")]
[assembly: AssemblyConfiguration("")]

// This sets the default COM visibility of types in the assembly to invisible.
// If you need to expose a type to COM, use [ComVisible(true)] on that type.
[assembly: ComVisible(false)]

// Mark the assembly as CLS compliant since it exposes external types
[assembly: CLSCompliant(true)]


// The following is needed to make the .NET 3.5 compatibility classes 
// visible to the other projects in the Accord.NET Framework solution.

#if NET35
[assembly: InternalsVisibleTo("Accord.Statistics, PublicKey="+
    "0024000004800000940000000602000000240000525341310004000001000100039880a76dac76"+
    "cddb9c85704c8a0e516773c28c0b202d9e0ae60b623b7bc554c7258bbf54ed6d98082964036109"+
    "d4d970132b761f5b00a83079fbff2fbea283632a420ef5280dd2c5546e3f5da776191f7076a096"+
    "6c06e7af21754fab55bdbdcddee5520632c3ebdc5908f6cdfb5b78d29123100f41faee0c29645e"+
    "42455498")]
[assembly: InternalsVisibleTo("Accord.MachineLearning, PublicKey=" +
    "0024000004800000940000000602000000240000525341310004000001000100039880a76dac76" +
    "cddb9c85704c8a0e516773c28c0b202d9e0ae60b623b7bc554c7258bbf54ed6d98082964036109" +
    "d4d970132b761f5b00a83079fbff2fbea283632a420ef5280dd2c5546e3f5da776191f7076a096" +
    "6c06e7af21754fab55bdbdcddee5520632c3ebdc5908f6cdfb5b78d29123100f41faee0c29645e" +
    "42455498")]
[assembly: InternalsVisibleTo("Accord.Math, PublicKey=" +
    "0024000004800000940000000602000000240000525341310004000001000100039880a76dac76" +
    "cddb9c85704c8a0e516773c28c0b202d9e0ae60b623b7bc554c7258bbf54ed6d98082964036109" +
    "d4d970132b761f5b00a83079fbff2fbea283632a420ef5280dd2c5546e3f5da776191f7076a096" +
    "6c06e7af21754fab55bdbdcddee5520632c3ebdc5908f6cdfb5b78d29123100f41faee0c29645e" +
    "42455498")]
[assembly: InternalsVisibleTo("Accord.Neuro, PublicKey=" +
    "0024000004800000940000000602000000240000525341310004000001000100039880a76dac76" +
    "cddb9c85704c8a0e516773c28c0b202d9e0ae60b623b7bc554c7258bbf54ed6d98082964036109" +
    "d4d970132b761f5b00a83079fbff2fbea283632a420ef5280dd2c5546e3f5da776191f7076a096" +
    "6c06e7af21754fab55bdbdcddee5520632c3ebdc5908f6cdfb5b78d29123100f41faee0c29645e" +
    "42455498")]
[assembly: InternalsVisibleTo("Accord.Imaging, PublicKey=" +
    "0024000004800000940000000602000000240000525341310004000001000100039880a76dac76" +
    "cddb9c85704c8a0e516773c28c0b202d9e0ae60b623b7bc554c7258bbf54ed6d98082964036109" +
    "d4d970132b761f5b00a83079fbff2fbea283632a420ef5280dd2c5546e3f5da776191f7076a096" +
    "6c06e7af21754fab55bdbdcddee5520632c3ebdc5908f6cdfb5b78d29123100f41faee0c29645e" +
    "42455498")]
#endif
