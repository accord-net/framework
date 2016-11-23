#include "Quadpack.h"
#include "quadpack\qagi.h"

using namespace AccordTestsMathCpp2;

float f(float* x)
{
    double dx = *x;
    return Quadpack::_function(dx);
}

Quadpack::Quadpack(void)
{
}

double Quadpack::Integrate(UFunction^ function, double a, double b)
{
    Quadpack::_function = function;

    float result;
    float error;
    long int evaluations;
    long int errorCode;

    float bound = 0;
    long int inf = 0;

    if (Double::IsInfinity(a) && Double::IsInfinity(b))
    {
        inf = 2;
    }
    else if (Double::IsInfinity(a))
    {
        bound = b;
        inf = -1;
    }
    else if (Double::IsInfinity(b))
    {
        bound = a;
        inf = 1;
    }
    else
    {
        throw gcnew ArgumentException();
    }

    long int limit = 100;
    long int lenw = limit * 4;
    float* work = new float[lenw];
    long int* iwork = new long int[limit];
    long int last;

    float ftol = 1e-3f;
    float abstol = 0;

    int r = qagi_(&f, &bound, &inf, &abstol, &ftol, &result, &error, &evaluations,
        &errorCode, &limit, &lenw, &last, iwork, work);

    if (r != 0)
        throw gcnew Exception();

    return result;
}