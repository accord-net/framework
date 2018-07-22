# Accord.NET Framework

[![DOI](https://zenodo.org/badge/3964514.svg)](https://zenodo.org/badge/latestdoi/3964514)
[![Build status](https://ci.appveyor.com/api/projects/status/ns9h9opjmu8iw3ep?svg=true)](https://ci.appveyor.com/project/cesarsouza/framework)
[![Build Status](https://travis-ci.org/accord-net/framework.svg?branch=development)](https://travis-ci.org/accord-net/framework)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Accord.svg)]()
[![License](https://img.shields.io/badge/license-LGPL--2.1-blue.svg)](LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Accord.svg)]()
[![NuGet Pre Release](https://img.shields.io/nuget/vpre/Accord.svg)]()

The Accord.NET project provides machine learning, statistics, artificial intelligence, computer vision and image processing methods to .NET. It can be used on Microsoft Windows, Xamarin, Unity3D, Windows Store applications, Linux or mobile.

After merging with the AForge.NET project, the framework now offers a unified API for learning/training machine learning models that is both easy to use *and* extensible. It is based on the following pattern:

- Choose a [learning algorithm](http://accord-framework.net/docs/html/N_Accord_MachineLearning.htm) that provides a Learn(x, y) or Learn(x) method;
- [Use the Learn(x, y)](http://accord-framework.net/docs/html/M_Accord_MachineLearning_VectorMachines_Learning_SequentialMinimalOptimization_Learn.htm) to create a [machine learning model](http://accord-framework.net/docs/html/T_Accord_MachineLearning_VectorMachines_SupportVectorMachine.htm) learned from the data; 
- Use the model's [Transform](http://accord-framework.net/docs/html/M_Accord_MachineLearning_ClassifierBase_2_Transform.htm), [Decide](http://accord-framework.net/docs/html/M_Accord_MachineLearning_ClassifierBase_2_Decide_1.htm), [Scores](http://accord-framework.net/docs/html/M_Accord_MachineLearning_BinaryScoreClassifierBase_1_Scores_3.htm), [Probabilities](http://accord-framework.net/docs/html/M_Accord_MachineLearning_BinaryLikelihoodClassifierBase_1_Probabilities.htm) or [LogLikelihoods](http://accord-framework.net/docs/html/M_Accord_MachineLearning_VectorMachines_SupportVectorMachine_2_LogLikelihood.htm) methods.

For more information, please see the [getting started guide](https://github.com/accord-net/framework/wiki/Getting-started), and check [the classfication wiki](https://github.com/accord-net/framework/wiki/Classification). *Please do not hesitate to edit the wiki if you would like!*


# Installing

To install the framework in your application, please use NuGet. If you are on Visual Studio, right-click on the "References" item in your solution folder, and select "Manage NuGet Packages." Search for Accord.MachineLearning ([or equivalently, Accord.Math, Accord.Statistics or Accord.Imaging depending on your initial goal](https://www.nuget.org/packages?q=accord.net)) and select "Install."

If you would like to install the framework on [Unity3D applications](https://unity3d.com), download the "libsonly" compressed archive from the [framework releases page](https://github.com/accord-net/framework/releases). Navigate to the Releases/Mono folder, and copy the .dll files to the Plugins folder in your Unity project. Finally, find and add the System.ComponentModel.DataAnnotations.dll assembly that should be available from your system to the Plugin folders as well.

## Sample applications

The framework comes with a wide range of sample applications to help get you started quickly. If you downloaded the framework sources or cloned the repository, open the *Samples.sln* solution file in the Samples folder.


# Building

#### With Visual Studio 2015

Please download and install the following dependencies:

- [T4 Toolbox for Visual Studio 2015](https://visualstudiogallery.msdn.microsoft.com/34b6d489-afbc-4d7b-82c3-dded2b726dbc)
- [Sandcastle Help File Builder (with VS2015 extension)](https://github.com/EWSoftware/SHFB/releases)
- [NUnit 3 Test Adapter](https://marketplace.visualstudio.com/items?itemName=NUnitDevelopers.NUnit3TestAdapter)

Then navigate to the Sources directory, and open the *Accord.NET.sln* solution file. Note: the solution includes F# unit test projects that can be disabled/unloaded from the solution in case you do not have support for F# tools in your version of Visual Studio.


#### With Visual Studio 2017

Please download and install the following dependencies:

- [T4 Toolbox for Visual Studio 2017](https://github.com/hagronnestad/T4Toolbox/releases/tag/vs2017-b1)
- [Sandcastle Help File Builder (with VS2017 extension)](https://github.com/EWSoftware/SHFB/releases)
- [NUnit 3 Test Adapter](https://marketplace.visualstudio.com/items?itemName=NUnitDevelopers.NUnit3TestAdapter)
- [Visual C++ Redistributable for Visual Studio 2015](https://www.microsoft.com/en-us/download/details.aspx?id=48145&751be11f-ede8-5a0c-058c-2ee190a24fa6) (both x64 and x86)

Then navigate to the Sources directory, and open the *Accord.NET.sln* solution file. Note: the solution includes F# unit test projects that can be disabled/unloaded from the solution in case you do not have support for F# tools in your version of Visual Studio.


#### With Mono in Linux

```bash
# Install Mono
sudo apt-get install mono-complete monodevelop monodevelop-nunit git autoconf make

# Clone the repository
git clone https://github.com/accord-net/framework.git

# Enter the directory
cd framework

# Build the framework solution using Mono
./autogen.sh
make build
make samples
make test
```

#### With Mono in OS X

```bash
# Install Mono
brew update
brew cask install mono-mdk pkg-config automake

# Clone the repository
git clone https://github.com/accord-net/framework.git

# Enter the directory
cd framework

# Set some environment variables with OSX-specific paths
export PKG_CONFIG_PATH=/Library/Frameworks/Mono.framework/Versions/Current/lib/pkgconfig/
export MONO=/Library/Frameworks/Mono.framework/Versions/Current/bin/mono
export XBUILD=/Library/Frameworks/Mono.framework/Versions/Current/bin/xbuild

# Build the framework solution using Mono
./autogen.sh
make build
make samples
make test
```

# Contributing

If you would like to contribute, please do so by helping us update the [project's Wiki pages](https://github.com/accord-net/framework/wiki). While you could also make a donation through PayPal [![Donate](https://www.paypalobjects.com/en_US/i/btn/btn_donate_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=N4Q6YQSPWN8BG), Flattr [![Flattr this git repo](http://api.flattr.com/button/flattr-badge-large.png)](https://flattr.com/submit/auto?user_id=cesarsouza&url=https://github.com/accord-net/framework&title=Accord.NET&language=&tags=github&category=software), or any of the cryptocurrencies shown below, as well as fill-in bug reports and contribute code in the form of pull requests, the most precious donation we could receive would be a bit of your time - [please take some minutes to submit us more documentation examples to our Wiki pages](https://github.com/accord-net/framework/wiki) :wink: 

Donate using cryptocurrencies:
- ```BTC: 1FC5gxLs2TsvuiHPP1tRLh5mPboQJQghvZ```
- ```ETH: 0x36FDA635Ef5773d8B376037D7BAfF22FeB987d92```
- ```LTC: LNjkZkMdSyncUvg5WnnhDNirdux4Q95gdt```

Note: all donations are 100% invested towards improving the framework, including, but not limited to, the hiring of extra developers to work on issues currently present at the project's issue tracker. If you would like to donate resources towards the development of a particular issue, please let us know!

Join the chat at https://gitter.im/accord-net/framework - but to have issues and questions answered, [post it as an issue](https://github.com/accord-net/framework/issues).


# Citing

Please cite this work as:
```bibtex
@misc{souza2014accord,
  title={The Accord.NET Framework},
  author={C{\'e}sar Souza and Andrew Kirillov and Marcos Diego Catalano and Accord.NET contributors},
  year={2014},
  doi={10.5281/zenodo.1029480},
  url={http://accord-framework.net}
}
```
[[bibtex](https://zenodo.org/record/1029481/export/hx#.We0_zCyXeUk)]
