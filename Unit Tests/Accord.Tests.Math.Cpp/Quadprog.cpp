#include <stdio.h>
#include <string.h>

#include "Quadprog.h"
#include "f2c.h"

using namespace std;
using namespace System;
using namespace AccordTestsMathCpp2;

extern "C"
{
	int qpgen2_(doublereal *dmat, doublereal *dvec, integer *
		fddmat, integer *n, doublereal *sol, doublereal *crval, doublereal *
		amat, doublereal *bvec, integer *fdamat, integer *q, integer *meq,
		integer *iact, integer *nact, integer *iter, doublereal *work,
		integer *ierr);
}

double* convert(array<double>^ a)
{
	int n = a->Length;
	double* m = new double[n];
	for (size_t i = 0; i < n; i++)
		m[i] = a[i];
	return m;	return m;
}

array<double>^ convert(doublereal* a, int n)
{
	array<double>^ m = gcnew array<double>(n);
	for (size_t i = 0; i < n; i++)
		m[i] = a[i];
	return m;
}

Tuple<int, array<double>^>^ Quadprog::Compute(int variables, int constraints,
	array<double>^ A, array<double>^ b, int eq,
	array<double>^ Q, array<double>^ c)
{

	// Convert to pointers

	integer n = variables;
	integer q = constraints;

	double* dmat = convert(Q);
	double* dvec = convert(c);

	double* amat = convert(A);
	double* bvec = convert(b);

	double* sol = new double[n];
	double crval;

	integer fddmat = n;
	integer fdamat = n;

	integer* iter = new integer[2];
	integer* iact = new integer[q];
	integer nact = 1;


	int r = System::Math::Min(n, q);

	double* work = new double[2 * n + r*(r + 5) / 2 + 2 * q + 100];
	integer ierr = 0;

	integer meq = eq;


	int t = qpgen2_(dmat, dvec, &fddmat, &n, sol, &crval, amat, bvec,
		&fdamat, &q, &meq, iact, &nact, iter, work, &ierr);

	if (ierr != 0)
		throw gcnew Exception();

	array<double>^ nsol = convert(sol, n);

	return gcnew Tuple<int, array<double>^>(t, nsol);
}
