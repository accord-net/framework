#include <stdio.h>
#include <string.h>

#include "Libbfgs.h"
#include "lbfgs.h"
#include "lbfgsb.h"

using namespace AccordTestsMathCpp2;
using namespace std;
using namespace System;



double computeTargetFunction(void *instance,
    const lbfgsfloatval_t *x,
    lbfgsfloatval_t *g,
    const int n,
    const lbfgsfloatval_t step)
{
    array<double>^ _Data = gcnew array<double>(n);
    System::Runtime::InteropServices::Marshal::Copy( IntPtr( ( void * ) x ), _Data, 0, n );

    double r = Wrapper::function(_Data);

    array<double>^ newg = Wrapper::gradient(_Data);

    System::Runtime::InteropServices::Marshal::Copy( newg, 0, IntPtr( ( void * ) g ), n );

    return r;
}

int showProgress(
    void *instance,
    const lbfgsfloatval_t *x,
    const lbfgsfloatval_t *g,
    const lbfgsfloatval_t fx,
    const lbfgsfloatval_t xnorm,
    const lbfgsfloatval_t gnorm,
    const lbfgsfloatval_t step,
    int n,
    int k,
    int ls
    )
{
    Info^ info = gcnew Info();

    info->x = gcnew array<double>(n);
    info->g = gcnew array<double>(n);

    if (x != NULL)
        System::Runtime::InteropServices::Marshal::Copy( IntPtr( ( void * ) x ), info->x, 0, n );

    if (g != NULL)
        System::Runtime::InteropServices::Marshal::Copy( IntPtr( ( void * ) g ), info->g, 0, n );

    info->fx = fx;
    info->xnorm = xnorm;
    info->gnorm = gnorm;
    info->step = step;
    info->n = n;
    info->k = k;
    info->ls = ls;

    Wrapper::list->Add(info);

    return 0;
}


String^ Wrapper::Libbfgs(array<double>^ start, Function^ function, Gradient^ gradient, Param^ param)
{
    list = gcnew List<Info^>();

    double fx = 0;

    Wrapper::function = function;
    Wrapper::gradient = gradient;

    lbfgs_parameter_t* param2 = new lbfgs_parameter_t();
    param2->m = param->m;
    param2->epsilon = param->epsilon;
    param2->past = param->past;
    param2->delta = param->delta;
    param2->max_iterations = param->max_iterations;
    param2->linesearch = param->linesearch;
    param2->max_linesearch = param->max_linesearch;
    param2->min_step = param->min_step;
    param2->max_step = param->max_step;
    param2->ftol = param->ftol;
    param2->wolfe = param->wolfe;
    param2->gtol = param->gtol;
    param2->xtol = param->xtol;
    param2->orthantwise_c = param->orthantwise_c;
    param2->orthantwise_start = param->orthantwise_start;
    param2->orthantwise_end = param->orthantwise_end;


    int n = start->Length;
    double* x = new double[n];
    System::Runtime::InteropServices::Marshal::Copy( start, 0, IntPtr( ( void * ) x ), n);


    int ret = lbfgs(n, x, &fx, &computeTargetFunction, &showProgress, NULL, param2);

    String^ retString = ((ReturnValue)ret).ToString();


    /* Report the result. */
    printf("L-BFGS optimization terminated with status code = %d\n", ret);
    printf("  fx = %f, x[0] = %f, x[1] = %f\n", fx, x[0], x[1]);

    System::Runtime::InteropServices::Marshal::Copy( IntPtr( ( void * ) x ), start, 0, n );

    if (ret < 0)
        fx = function(start);

    showProgress(NULL, x, NULL, fx, 0, 0, 0, 0, 0, 0);

    lbfgs_free(x);

    return retString;
}


String^ Wrapper::Lbfgsb3(array<double>^ start, Function^ function, Gradient^ gradient, Param2^ param)
{
    list = gcnew List<Info^>();

    Wrapper::function = function;
    Wrapper::gradient = gradient;

    /* System generated locals */
    integer i__1;
    integer iprint;

    const int nmax = 1024;
    const int mmax = 20;
    const int ws = 2*mmax*nmax + 11*mmax*mmax + 5*nmax + 8*mmax;

    static doublereal f, g[nmax];
    static integer i__;
    static doublereal l[nmax];
    static integer m, n;
    static doublereal u[nmax], x[nmax], t1, t2;
    static doublereal wa[ws];
    static integer nbd[nmax], iwa[3*nmax];
    static char task[60];
    static doublereal factr;
    static char csave[60];
    static doublereal dsave[29];
    static integer isave[44];
    static logical lsave[4];
    static doublereal pgtol;

    memset(dsave, 0, sizeof(doublereal) * 29);
    memset(isave, 0, sizeof(integer) * 44);
    memset(csave, 0, sizeof(char) * 60);
    memset(wa, 0, sizeof(doublereal) * ws);

    //iprint = 101; // print details of every iteration including x and g;
	iprint = -1;
    factr = param->factr;
    pgtol = param->pgtol;

    n = start->Length;
    m = param->m;

    i__1 = n;
    for (i__ = 1; i__ <= i__1; i__ ++) 
    {
        bool hasUpper = !Double::IsInfinity(param->u[i__ - 1]);
        bool hasLower = !Double::IsInfinity(param->l[i__ - 1]);

        if (hasUpper && hasLower)
            nbd[i__ - 1] = 2;
        else if (hasUpper)
            nbd[i__ - 1] = 3;
        else if (hasLower)
            nbd[i__ - 1] = 1;
        else nbd[i__ - 1] = 0; // unbounded

        if (hasLower)
            l[i__ - 1] = param->l[i__ - 1];
        if (hasUpper)
            u[i__ - 1] = param->u[i__ - 1];
    }

    i__1 = n;
    for (i__ = 1; i__ <= i__1; ++i__) {
        x[i__ - 1] = start[i__ - 1];
    }


    s_copy(task, "START", (ftnlen)60, (ftnlen)5);
    iteration = 0;

L111:

    iteration++;

    setulb_(&n, &m, x, l, u, nbd, &f, g, &factr, &pgtol, wa, iwa, task, &iprint, 
        csave, lsave, isave, dsave, (ftnlen)60, (ftnlen)60);

    Info^ info = gcnew Info();

    array<double>^ _Data = gcnew array<double>(n);
    System::Runtime::InteropServices::Marshal::Copy( IntPtr( ( void * ) x ), _Data, 0, n );
    double newF = Wrapper::function(_Data);
    array<double>^ newg = Wrapper::gradient(_Data);

    info->isave = gcnew array<int>(44);
    info->dsave = gcnew array<double>(29);
    info->lsave = gcnew array<int>(4);
    info->csave = gcnew String(csave);
    info->Value = newF;
    info->Gradient = (array<double>^)newg->Clone();
    info->Iteration = iteration;
    info->Work = gcnew array<double>(ws);

    System::Runtime::InteropServices::Marshal::Copy( IntPtr( ( void * ) isave ), info->isave, 0, 44 );
    System::Runtime::InteropServices::Marshal::Copy( IntPtr( ( void * ) dsave ), info->dsave, 0, 29 );
    System::Runtime::InteropServices::Marshal::Copy( IntPtr( ( void * ) lsave ), info->lsave, 0, 4 );
    System::Runtime::InteropServices::Marshal::Copy( IntPtr( ( void * ) wa ), info->Work, 0, ws );


    Wrapper::list->Add(info);

    if (s_cmp(task, "FG", (ftnlen)2, (ftnlen)2) == 0) 
    {
        f = newF;

        System::Runtime::InteropServices::Marshal::Copy( newg, 0, IntPtr( ( void * ) g ), n );

	    goto L111;
    }

    if (s_cmp(task, "NEW_X", (ftnlen)5, (ftnlen)5) == 0)
    {
	    goto L111;
    }

    //s_stop("", (ftnlen)0);

    return gcnew String(task);
}