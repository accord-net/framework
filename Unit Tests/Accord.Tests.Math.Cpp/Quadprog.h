#pragma once

namespace AccordTestsMathCpp2 
{
    public ref class Quadprog
    {
    public:
        static int Compute(int variables, int constraints, 
            array<double>^ A, array<double>^ b, int eq, 
            array<double>^ Q, array<double>^ c);
    };
}
