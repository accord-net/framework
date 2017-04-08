#pragma once

#include "bhtsne\tsne.h"
#include <vector>

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Reflection;
using namespace System::Runtime::InteropServices;



namespace AccordTestsMathCpp2
{


	public ref class TSNEWrapper
	{
	public:
		TSNEWrapper();

		static void run(array<double, 2>^ X, array<double, 2>^ Y, double perplexity, double theta);

		static void computeSquaredEuclideanDistance(array<double, 2>^ X, array<double, 2>^ DD);

		static void symmetrizeMatrix(array<unsigned int>^ _row_P, array<unsigned int>^ _col_P, array<double>^ _val_P, int ND);

		static void computeGaussianPerplexity(array<double, 2>^ X, int N, int D, array<double, 2>^ P, double perplexity);

		static double evaluateError(array<unsigned int>^ row_P, array<unsigned int>^ col_P, array<double>^ val_P, array<double, 2>^ Y, int N, int D, double theta);

		static double evaluateError(array<double, 2>^ P, array<double, 2>^ Y, int N, int D);

		static void computeGaussianPerplexity(array<double, 2>^ X, int N, int D, array<unsigned int>^ _row_P, array<unsigned int>^ _col_P, array<double>^ _val_P, double perplexity, int K);

		static void computeGradient(array<double, 2>^ P, array<unsigned int>^ inp_row_P, array<unsigned int>^ inp_col_P, array<double>^ inp_val_P, array<double, 2>^ Y, int N, int D, array<double, 2>^ dC, double theta);
	};

}
