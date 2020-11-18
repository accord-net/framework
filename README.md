# Accord.NET Framework

This project is currently archived. Please fork the project into your own GitHub account if you would like to continue its development.

# Archiving

After 14~15 years of development, the Accord.NET project has finally been archived. I would like to send a big thank you to everyone who has ever comitted, dedicated, or otherwise devoted their time and effort into making this repository better every day. What had started as a project to store knowledge in the form of algorithms and implementations had grown way beyond my expectations since I first joined university and started working on research ~15 years ago.

In the meantime, many things have happened, and the ML landscape had also greatly evolved since then.

However, I pledge you to absolutely not interpret the archiving of this project as a loss. The main goal of this project since day 1 was to crystalize the ML knowledge available at the time in the form of source code and store it under a number of compatible free software licenses. As such, if you would like to, *do not feel afraid of copy and pasting portions of this project into your own implementations*. If I (Cesar De Souza) am the solely implementor of any of the classes you would like to port, I hereby grant you an irrevocable license to do so. If I am not, and the current license of the file you would like to port does not suit your needs, I can help you contact their original developers to help you with the transition.

<pre>
"We reject kings, presidents and voting.  We
   believe in rough consensus and running code"
   -- David Clark
</pre>

All this said. This has been an amazing ride.

Thanks everyone for their ever growing support all those years.

Let's keep in touch,

Cesar


# Previous

[![DOI](https://zenodo.org/badge/3964514.svg)](https://zenodo.org/badge/latestdoi/3964514)
[![Build status](https://ci.appveyor.com/api/projects/status/ns9h9opjmu8iw3ep?svg=true)](https://ci.appveyor.com/project/cesarsouza/framework)
[![Build Status](https://travis-ci.org/accord-net/framework.svg?branch=development)](https://travis-ci.org/accord-net/framework)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Accord.svg)](https://www.nuget.org/packages/Accord/)
[![License](https://img.shields.io/badge/license-LGPL--2.1-blue.svg)](LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Accord.svg)](https://www.nuget.org/packages/Accord/)
[![NuGet Pre Release](https://img.shields.io/nuget/vpre/Accord.svg)](https://www.nuget.org/packages/Accord/)

The Accord.NET project provides machine learning, statistics, artificial intelligence, computer vision and image processing methods to .NET. It can be used on Microsoft Windows, Xamarin, Unity3D, Windows Store applications, Linux or mobile.

After merging with the AForge.NET project, the framework now offers a unified API for learning/training machine learning models that is both easy to use *and* extensible. It is based on the following pattern:

- Choose a [learning algorithm](http://accord-framework.net/docs/html/N_Accord_MachineLearning.htm) that provides a Learn(x, y) or Learn(x) method;
- [Use the Learn(x, y)](http://accord-framework.net/docs/html/M_Accord_MachineLearning_VectorMachines_Learning_SequentialMinimalOptimization_Learn.htm) to create a [machine learning model](http://accord-framework.net/docs/html/T_Accord_MachineLearning_VectorMachines_SupportVectorMachine.htm) learned from the data; 
- Use the model's [Transform](http://accord-framework.net/docs/html/M_Accord_MachineLearning_ClassifierBase_2_Transform.htm), [Decide](http://accord-framework.net/docs/html/M_Accord_MachineLearning_ClassifierBase_2_Decide_1.htm), [Scores](http://accord-framework.net/docs/html/M_Accord_MachineLearning_BinaryScoreClassifierBase_1_Scores_3.htm), [Probabilities](http://accord-framework.net/docs/html/M_Accord_MachineLearning_BinaryLikelihoodClassifierBase_1_Probabilities.htm) or [LogLikelihoods](http://accord-framework.net/docs/html/M_Accord_MachineLearning_VectorMachines_SupportVectorMachine_2_LogLikelihood.htm) methods.

For more information, please see the [getting started guide](https://github.com/accord-net/framework/wiki/Getting-started), and check [the classfication wiki](https://github.com/accord-net/framework/wiki/Classification). *Please do not hesitate to edit the wiki if you would like!*

**Update (10/05/2020):** Please see the [current status section](https://github.com/accord-net/framework#current-status) below before you start using this library in any new projects.

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

# Current status

Before you decide to use the framework for new projects, please see the following personal note below.

>
> I am writing this note to give an official status for the project.
>
> This project has certainly been the most important thing I have ever created, but I could not keep up with maintaining it as well as I wanted. This project allowed me to achieve the biggest dream I had, and that I never though I would have been able to achieve in my life, which was (some may laugh and possibly not understand - specially if you did not know where I came from): starting a new life, and a new career, here in Europe. 
> 
> For about 10 years, I had worked on this project almost every day of my life.
> 
> But with the new life, there came new steps to be climbed, and I suddently had new responsabilities and things that I absolutely needed to accomplish very well. I started a PhD and had to focus on it so I could not keep up maintaining the library for about three years. I tried to hire freelance developers to help maintain the project in the meantime I had to be absent, and it worked to some extent, but at some point, I did not have the resources to keep up with the development anymore. Eventually, I developed panic-level anxiety since I felt I had left so many people behind by not being able to keep up with the development of the project anymore. I found out that I would always _avoid opening up_ the issues page of the project, or even _checking my own personal e-mails_, just to avoid receiving new inquiries about the status of the project. 
>
> Then, a few months before my PhD defense (which happened very well, actually!), Microsoft announced that they wanted to make ML.net (which I actually fully support), the standard approach for machine learning in .NET. While this is great news (because I fully support MS giving more support for all ML practitioners out there), this eventually meant **that Accord.NET would eventually become obsolete as ML.net was on its path to become the de-facto ML library for .NET** in the foreseeing future.
> 
> I think that the reasons above would have been already enough to explain why I decided to not update Accord.NET anymore after that. However... in addition, I have to say that, as a researcher, and not solely as a developer, I have also published in, and attended to, **the most important machine learning conferences in the world to date, and under this context, I need to say that in the academia world, no one has ever heard of the framework or the project itself**. Actually, from my experience, people in those conferences can laugh or even mistreat you, **if you ever mention you have ever developed anything in C#**, specially for machine learning, as everyone [understandably] uses Python nowadays to accomplish tasks in this domain. **This happened even when those people were from Microsoft itself**. 

> I could  actually understand the reaction, as I myself only use Python to do my day-to-day work, and while I love C#/.NET, I have to say that there is nothing that could even remotely compete with Python/Pytorch at this day and age). 

> Anyways, therefore, in the past months, I have been pondering about archiving the project. To avoid that, **I am willing to make someone who would like, also an administrator of the project**.
> 
> I am also willing to change the license of any file where I am the single author (you can check the copyright headers in each file) to **MIT** so people can reuse individual pieces of code more easily. Anyone who becomes administrator is welcome to slice the parts of the project that still make sense to exist (e.g., the FFmpeg wrappers, statistical distributions, statistical tests and the simple transforms like PCA) and even start new libraries (hopefully in .NET Core) providing only them if wanted.
>
> Also, when I started this project back in 2007 (and when the original AForge library started, even way before that), there were almost no other libraries we could built upon, so we had to do start almost everything from scratch. This is not the case anymore. Any new libraries coming out of this project should **definitely reuse existing libraries for basic tasks such as matrix operations and image processing**.
> 
> Cesar De Souza  
> 10-May-2020


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
