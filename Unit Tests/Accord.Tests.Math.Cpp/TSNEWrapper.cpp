#pragma once

#include "TSNEWrapper.h"

using namespace System; 
using namespace System::Collections::Generic;
using namespace System::Reflection;
using namespace System::Runtime::InteropServices;
using namespace AccordTestsMathCpp2;

[assembly:AssemblyKeyFileAttribute("Accord.snk")];


TSNEWrapper::TSNEWrapper()
{

}

double* tomatrix(array<double, 2>^ X)
{
	int N = X->GetLength(0);
	int D = X->GetLength(1);
	double* x = new double[N*D];
	for (size_t i = 0; i < N; i++)
		for (size_t j = 0; j < D; j++)
			x[i * D + j] = X[i, j];
	return x;
}

unsigned int* tomatrix(array<unsigned int>^ X)
{
	int N = X->Length;
	unsigned int* x = (unsigned int*)calloc(X->Length, sizeof(unsigned int));
	for (size_t i = 0; i < N; i++)
		x[i] = X[i];
	return x;
}

double* tomatrix(array<double>^ X)
{
	int N = X->Length;
	double* x = (double*)calloc(X->Length, sizeof(double));
	for (size_t i = 0; i < N; i++)
		x[i] = X[i];
	return x;
}


void TSNEWrapper::run(array<double, 2>^ X, array<double, 2>^ Y, double perplexity, double theta)
{
	auto x = tomatrix(X);
	int N = X->GetLength(0);
	int D = X->GetLength(1);
	int no_dim = Y->GetLength(1);
	double* y = tomatrix(Y);

	TSNE t;
	t.run(x, N, D, y, no_dim, perplexity, theta, 0, true);

	for (size_t i = 0; i < N; i++)
		for (size_t j = 0; j < no_dim; j++)
			Y[i, j] = y[i * no_dim + j];
}


void TSNEWrapper::computeSquaredEuclideanDistance(array<double, 2>^ X, array<double, 2>^ DD)
{
	auto x = tomatrix(X);
	int N = X->GetLength(0);
	int D = X->GetLength(1);
	double* dd = new double[N * N];

	TSNE t;
	t.computeSquaredEuclideanDistance(x, N, D, dd);

	for (size_t i = 0; i < N; i++)
		for (size_t j = 0; j < N; j++)
			DD[i, j] = dd[i * N + j];
}

void TSNEWrapper::symmetrizeMatrix(array<unsigned int>^ _row_P, array<unsigned int>^ _col_P, array<double>^ _val_P, int ND)
{
	unsigned int* row = tomatrix(_row_P);
	unsigned int* col = tomatrix(_col_P);
	double* val = tomatrix(_val_P);

	TSNE t;
	t.symmetrizeMatrix(&row, &col, &val, ND);

	for (size_t i = 0; i < _row_P->Length; i++)
		_row_P[i] = row[i];

	for (size_t i = 0; i < 10; i++)
	{
		_col_P[i] = col[i];
		_val_P[i] = val[i];
	}
}

void TSNEWrapper::computeGaussianPerplexity(array<double, 2>^ X, int N, int D, array<double, 2>^ P, double perplexity)
{
	auto x = tomatrix(X);
	double *p = new double[N * N];

	TSNE t;
	t.computeGaussianPerplexity(x, N, D, p, perplexity);

	for (size_t i = 0; i < N; i++)
		for (size_t j = 0; j < N; j++)
			P[i, j] = p[i * N + j];
}

double TSNEWrapper::evaluateError(array<unsigned int>^ row_P, array<unsigned int>^ col_P, array<double>^ val_P, array<double, 2>^ Y, int N, int D, double theta)
{
	unsigned int* row = tomatrix(row_P);
	unsigned int* col = tomatrix(col_P);
	double* val = tomatrix(val_P);

	double* y = tomatrix(Y);

	TSNE t;
	return t.evaluateError(row, col, val, y, N, D, theta);
}

double TSNEWrapper::evaluateError(array<double, 2>^ P, array<double, 2>^ Y, int N, int D)
{
	double* p = tomatrix(P);
	double* y = tomatrix(Y);

	TSNE t;
	return t.evaluateError(p, y, N, D);
}

void TSNEWrapper::computeGaussianPerplexity(array<double, 2>^ X, int N, int D, array<unsigned int>^ _row_P, array<unsigned int>^ _col_P, array<double>^ _val_P, double perplexity, int K) 
{
	unsigned int* row = tomatrix(_row_P);
	unsigned int* col = tomatrix(_col_P);
	double* val = tomatrix(_val_P);
	double* x = tomatrix(X);

	TSNE t;
	t.computeGaussianPerplexity(x, N, D, &row, &col, &val, perplexity, K);

	for (size_t i = 0; i < _row_P->Length; i++)
		_row_P[i] = row[i];

	for (size_t i = 0; i < _col_P->Length; i++)
	{
		_col_P[i] = col[i];
		_val_P[i] = val[i];
	}
}

void TSNEWrapper::computeGradient(array<double, 2>^ P, array<unsigned int>^ inp_row_P, array<unsigned int>^ inp_col_P, array<double>^ inp_val_P, array<double, 2>^ Y, int N, int D, array<double, 2>^ dC, double theta)
{
	unsigned int* row = tomatrix(inp_row_P);
	unsigned int* col = tomatrix(inp_col_P);
	double* val = tomatrix(inp_val_P);
	double* p = tomatrix(P);
	double* y = tomatrix(Y);
	double* dc = tomatrix(dC);

	TSNE t;
	t.computeGradient(p, row, col, val, y, N, D, dc, theta);

	for (size_t i = 0; i < N; i++)
		for (size_t j = 0; j < D; j++)
			dC[i, j] = dc[i * D + j];
}

