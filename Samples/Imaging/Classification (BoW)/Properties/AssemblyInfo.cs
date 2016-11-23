using System;
using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Image classification with Visual Bag-of-Words")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Accord.NET")]
[assembly: AssemblyProduct("Accord.NET Framework")]
[assembly: AssemblyCopyright("Copyright © César Souza, 2009-2013")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyDescription(
@"The Image Classification sample application shows how to perform image classification using the Bag of Visual Words (BoW) model with SURF features and the Binary Split algorithm.

The BoW model is used to transform the many SURF feature points in a image in a single, fixed-length feature vector. The feature vector is then used to train a Support Vector Machine (SVM) using a variety of kernels.")]



// This sets the default COM visibility of types in the assembly to invisible.
// If you need to expose a type to COM, use [ComVisible(true)] on that type.
[assembly: ComVisible(false)]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
[assembly: AssemblyVersion("1.0.0.0")]
