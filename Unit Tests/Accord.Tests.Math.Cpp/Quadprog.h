#pragma once

using namespace std;
using namespace System;

namespace AccordTestsMathCpp2 
{
    public ref class Quadprog
    {
    public:
		static Tuple<int, array<double>^>^ Compute(int variables, int constraints,
            array<double>^ A, array<double>^ b, int eq, 
            array<double>^ Q, array<double>^ c);
    };
}
