Accord.NET Framework
===================

Welcome to the Accord.NET GitHub repository. The Accord.NET project provides standard machine learning, statistics, artificial intelligence, computer vision, image processing methods to .NET, be it on Microsoft Windows, Xamarin, Unity3D, Windows Store applications or mobile.

Following a big refactoring after the merge with the AForge.NET project, the framework now offers a unified API for learning/training machine learning models that is both easy to use *and* extensible. It is based on the following pattern:

- Instantiate a desired *learning algorithm* that provides a .Learn(x, y) method;
- Call the learning algorithm's Learn(x, y) method with the data you have; 
- It will return you a trained model that you can use to Transform(), Decide(), Score(), compute Probabilities() or LogLikelihoods() for new data.

For more information, please see the getting started guide, and check the wiki. *Note: the wiki has been gradually updated/rewritten followed the recent merge/refactoring. Please do not hesitate to edit it if you would like!*
 
Installing
-------------

To install the framework in your application, please use NuGet. If you are on Visual Studio, right-click on the "References" item in your solution folder, and select "Manage NuGet Packages." Search for Accord.MachineLearning ([or equivalently, Accord.Math, Accord.Statistics or Accord.Imaging depending on your initial goal](https://www.nuget.org/packages?q=accord.net)) and select "Install."

If you would like to install the framwork on [Unity3D applications](https://unity3d.com), download the framework binaries for .NET 3.5 from the *framework releases* page.

Sample applications
-------------

The framework comes with a wide range of sample applications to help get you started quickly. If you downloaded the framework sources or cloned the repository, open the *Samples.sln* solution file in the Samples folder.

Building
-------------

#### With Visual Studio 2013
Navigate to the Sources directory, and open the *Accord.NET.sln* solution file.

#### With Visual Studio 2015
Before you can build with VS2015, you need to have VS2013 installed. Some VC++ projects still need the VS2013 VC++ toolchain. After installing either the full VS2013 or just the VS2013 VC++ tools, navigate to the Sources directory, and open the *Accord.NET.sln* solution file. However, please make sure to **not** let VS2015 auto-convert any VC++ projects to the VS2015 toolchain, otherwise the project will not build.

#### With MonoDevelop, for running on Linux

    # Install Mono
    sudo apt-get install mono-complete monodevelop monodevelop-nunit

    # Clone the repository
    git clone https://github.com/accord-net/framework.git

    # Enter the directory
    cd framework

    # Build the framework solution using Mono
    mdtool build -c:"NET40" Sources/Accord.NET.Mono.sln
    
  Contributing
-------------

If you would like to contribute, please do so by helping update the project's Wiki pages. While you can also make a donation, fill bug reports and contribute code in the form of pull requests, priority is being given to the documentation. 

Join the chat at https://gitter.im/accord-net/framework
