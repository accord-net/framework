#include "Libbfgs.h"
#include "lbfgs.h"
#include <stdio.h>

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

    double r = Libbfgs::function(_Data);

    array<double>^ newg = Libbfgs::gradient(_Data);

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

    Libbfgs::list->Add(info);

    return 0;
}


String^ Libbfgs::Run(array<double>^ start, Function^ function, Gradient^ gradient, Param^ param)
{
    list = gcnew List<Info^>();

    double fx = 0;

    Libbfgs::function = function;
    Libbfgs::gradient = gradient;

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

