 The Accord.NET Framework 
 http://accord-framework.net


  The Accord.NET Framework provides machine learning, mathematics, statistics,
  computer vision, computer audition, and several scientific computing related
  methods and techniques to .NET. The project extends the popular AForge.NET 
  Framework providing a more complete scientific computing environment.
 
  The GitHub repository at https://github.com/accord-net/framework is the official 
  home of the project after release 2.10 was finished. As such, new releases will 
  only be made available on this repository.

 

 Installing the framework
 ------------------------

1) Download the framework through NuGet:
   https://www.nuget.org/packages?q=accord.net


2) Follow the Getting Started Guide
   http://accord-framework.net/get-started.html
   

3) Check the sample applications and find one that is related to what you need.
   http://accord-framework.net/samples.html
   
   If you have installed the framework using the installer, the samples will be at
   
     C:\Program Files (x86)\Accord.NET\Framework\Samples
   
   You can open the Samples.sln solution on Visual Studio and check the sample 
   applications for examples. Complete documentation is also available online at
   
     http://accord-framework.net/docs/Index.html



 Building with Visual Studio
 ---------------------------

1) Clone the repository (SmartGit is the best Git tool available for Windows)
2) Open Sources/Accord.NET.sln in Visual Studio (works with Express versions)



 Building in Linux with Mono
 ---------------------------

# Install Mono
sudo apt-get install mono-complete monodevelop monodevelop-nunit

# Clone the repository
git clone https://github.com/accord-net/framework.git

# Enter the directory
cd framework

# Build the framework solution using Mono
mdtool build -c:"NET40" Sources/Accord.NET.Mono.sln

