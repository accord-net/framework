
using namespace System;
using namespace System::Collections::Generic;
using namespace System::Reflection;

[assembly:AssemblyKeyFileAttribute("Accord.snk")];


namespace AccordTestsMathCpp2 
{

    public enum class ReturnValue
    {
        LBFGS_SUCCESS = 0,
        LBFGS_CONVERGENCE = 0,
        LBFGS_STOP,
        LBFGS_ALREADY_MINIMIZED,
        LBFGSERR_UNKNOWNERROR = -1024,
        LBFGSERR_LOGICERROR,
        LBFGSERR_OUTOFMEMORY,
        LBFGSERR_CANCELED,
        LBFGSERR_INVALID_N,
        LBFGSERR_INVALID_N_SSE,
        LBFGSERR_INVALID_X_SSE,
        LBFGSERR_INVALID_EPSILON,
        LBFGSERR_INVALID_TESTPERIOD,
        LBFGSERR_INVALID_DELTA,
        LBFGSERR_INVALID_LINESEARCH,
        LBFGSERR_INVALID_MINSTEP,
        LBFGSERR_INVALID_MAXSTEP,
        LBFGSERR_INVALID_FTOL,
        LBFGSERR_INVALID_WOLFE,
        LBFGSERR_INVALID_GTOL,
        LBFGSERR_INVALID_XTOL,
        LBFGSERR_INVALID_MAXLINESEARCH,
        LBFGSERR_INVALID_ORTHANTWISE,
        LBFGSERR_INVALID_ORTHANTWISE_START,
        LBFGSERR_INVALID_ORTHANTWISE_END,
        LBFGSERR_OUTOFINTERVAL,
        LBFGSERR_INCORRECT_TMINMAX,
        LBFGSERR_ROUNDING_ERROR,
        LBFGSERR_MINIMUMSTEP,
        LBFGSERR_MAXIMUMSTEP,
        LBFGSERR_MAXIMUMLINESEARCH,
        LBFGSERR_MAXIMUMITERATION,
        LBFGSERR_WIDTHTOOSMALL,
        LBFGSERR_INVALIDPARAMETERS,
        LBFGSERR_INCREASEGRADIENT,
        LBFGSERR_INVALID_M,
    };

    public ref class Param
    {
    public:
        int m;
        double  epsilon;
        int past;
        double  delta;
        int max_iterations;
        int linesearch;
        int max_linesearch;
        double  min_step;
        double  max_step;
        double  ftol;
        double  wolfe;
        double  gtol;
        double  xtol;
        double  orthantwise_c;
        int orthantwise_start;
        int orthantwise_end;
    };

     public ref class Param2
    {
    public:
        int m;
        double factr;
        double pgtol;
        array<double>^ l;
        array<double>^ u;
        int max_iterations;
    };

    public ref class Info
    {
    public:
        array<double>^ x;
        array<double>^ g;
        double fx;
        double xnorm;
        double gnorm;
        double step;
        int n;
        int k;
        int ls;

        array<int>^ isave;
        array<double>^ dsave;
        array<int>^ lsave;
        //array<System::Char>^ csave;
        String^ csave;

        double Value;
        array<double>^ Gradient;
        int Iteration;
        array<double>^ Work;
    };

    public delegate double Function(array<double>^);

    public delegate array<double>^ Gradient(array<double>^);

    public ref class Wrapper
    {
    public:

        static int iteration;

        static Function^ function;
        static Gradient^ gradient;

        static String^ Libbfgs(array<double>^ start, Function^ function, 
            Gradient^ gradient, Param^ param);

        static String^ Lbfgsb3(array<double>^ start, Function^ function, 
            Gradient^ gradient, Param2^ param);

        static List<Info^>^ list;

    };

}
