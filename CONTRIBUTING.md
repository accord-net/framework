
# Contributing

Please send pull requests against the development branch instead of master. The
master branch is reserved to contain only the latest official public release of 
the framework.


## Please stick to C# 4.0 (avoid features from C# 5, 6 and 7)

If possible, when contributing code to the framework, please avoid using C# language features from above C# 4.0. There are at least two reasons for this restriction:

 1. **Mono:** To keep compatibility with the most widespread versions of Mono (4.x). If you take a look at our Travis-CI builds, you might see that the builds are actually done and run using Mono, so using any language feature that is not supported there will cause a failing build;
 1. **Unity:** Some language features might not be accessible when targeting .NET 3.5, which for a long time has been the only .NET Framework version that could be run from inside [Unity](https://unity3d.com).

More specifically, when submitting pull-requests, please avoid using:

 * The nameof(.) operator;
 * Expression-bodied members;
 * Null-conditional operators
 * string interpolation
 * async/await *
 * value tuples

_\* Unless you can guard those sections using conditional compilation clauses (i.e. ```#if NET35```) and either exclude those code sections from the .NET 3.5 / Mono 4.0 builds or provide specific (possibly non-optimal) implementations for those platforms. Please take a look at the Accord.Compat namespace for help in filling missing functionality from higher platform versions if you run into any of those cases._
 
## Please use Visual Studio's default code formatting

If possible, please use the same code formatting style as the default format offered by Visual Studio. This is the style that Visual Studio will format your code with when pressing Ctrl+E, D while in the editor.
