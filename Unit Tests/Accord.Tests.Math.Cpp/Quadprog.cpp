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
    System::Runtime::InteropServices::Marshal::Copy(a, 0, IntPtr( ( void * ) m ), n );
    return m;
}

int Quadprog::Compute(int variables, int constraints, 
    array<double>^ A, array<double>^ b, int eq, 
    array<double>^ Q, array<double>^ c)
{

    // Convert to pointers

    integer n = variables;
    integer q = constraints;

    doublereal* dmat = convert(Q);
    doublereal* dvec = convert(c);

    doublereal* amat = convert(A);
    doublereal* bvec = convert(b);

    doublereal* sol = new double[n];
    doublereal crval;

    integer fddmat = n;
    integer fdamat = q;

    integer iter = 0;

    integer iact = 0;
    integer nact = 0;
    integer iterl = 0;


    int r = System::Math::Min(n, q);

    doublereal* work = new double[2*n+r*(r+5)/2 + 2*q +1];
    integer ierr = 0;
    
    integer meq = eq;


    int t = qpgen2_(dmat, dvec, &fddmat, &n, sol, &crval, amat, bvec, 
        &fdamat, &q, &meq, &iact, &nact, &iter, work, &ierr);

    if (ierr != 0)
        throw gcnew Exception();

    return t;
}
