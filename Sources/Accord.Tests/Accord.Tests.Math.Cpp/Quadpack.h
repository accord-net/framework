using namespace System;
using namespace System::Collections::Generic;
using namespace System::Reflection;

namespace AccordTestsMathCpp2 
{
    public delegate double UFunction(double);

    public ref class Quadpack
    {
    public:
        Quadpack(void);

        static UFunction^ _function;

        static double Integrate(UFunction^ function, double a, double b);
    };
}

