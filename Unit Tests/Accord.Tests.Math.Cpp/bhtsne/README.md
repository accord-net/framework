
This software package contains a Barnes-Hut implementation of the t-SNE algorithm. The implementation is described in [this paper](http://lvdmaaten.github.io/publications/papers/JMLR_2014.pdf).


# Installation #

On Linux or OS X, compile the source using the following command:

```
g++ sptree.cpp tsne.cpp -o bh_tsne -O2
```

The executable will be called `bh_tsne`.

On Windows using Visual C++, do the following in your command line:

- Find the `vcvars64.bat` file in your Visual C++ installation directory. This file may be named `vcvars64.bat` or something similar. For example:

```
  // Visual Studio 12
  "C:\Program Files (x86)\Microsoft Visual Studio 12.0\VC\bin\amd64\vcvars64.bat"

  // Visual Studio 2013 Express:
  C:\VisualStudioExp2013\VC\bin\x86_amd64\vcvarsx86_amd64.bat
```

- From `cmd.exe`, go to the directory containing that .bat file and run it.

- Go to `bhtsne` directory and run:

```
  nmake -f Makefile.win all
```

The executable will be called `windows\bh_tsne.exe`.

# Usage #

The code comes with wrappers for Matlab and Python. These wrappers write your data to a file called `data.dat`, run the `bh_tsne` binary, and read the result file `result.dat` that the binary produces. There are also external wrappers available for [Torch](https://github.com/clementfarabet/manifold), [R](https://github.com/jkrijthe/Rtsne), and [Julia](https://github.com/zhmz90/BHTsne.jl). Writing your own wrapper should be straightforward; please refer to one of the existing wrappers for the format of the data and result files.

Demonstration of usage in Matlab:

```matlab
filename = websave('mnist_train.mat', 'https://github.com/awni/cs224n-pa4/blob/master/Simple_tSNE/mnist_train.mat?raw=true');
load(filename);
numDims = 2; pcaDims = 50; perplexity = 50; theta = .5; alg = 'svd';
map = fast_tsne(digits', numDims, pcaDims, perplexity, theta, alg);
gscatter(map(:,1), map(:,2), labels');
```
