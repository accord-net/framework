using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accord.Math.Optimization
{
    public abstract class BaseGradientOptimizationMethod : BaseOptimizationMethod
    {

        /// <summary>
        ///   Gets or sets a function returning the gradient
        ///   vector of the function to be optimized for a
        ///   given value of its free parameters.
        /// </summary>
        /// 
        /// <value>The gradient function.</value>
        /// 
        public Func<double[], double[]> Gradient { get; set; }


        public BaseGradientOptimizationMethod(int numberOfVariables)
            : base(numberOfVariables)
        {
        }

        public BaseGradientOptimizationMethod(int numberOfVariables,
            Func<double[], double> function, Func<double[], double[]> gradient)
            : base(numberOfVariables, function)
        {
            if (gradient == null)
                throw new ArgumentNullException("gradient");

            this.Gradient = gradient;
        }

        public  override bool Maximize()
        {
            if (Gradient == null)
                throw new InvalidOperationException("gradient");

            NonlinearObjectiveFunction.CheckGradient(Gradient, Solution);

            var g = Gradient;

            Gradient = (x) => g(x).Multiply(-1);

            bool success = base.Maximize();

            Gradient = g;

            return success;
        }

        public  override bool Minimize()
        {
            if (Gradient == null)
                throw new InvalidOperationException("gradient");

            NonlinearObjectiveFunction.CheckGradient(Gradient, Solution);

            return base.Minimize();
        }


        
    }
}
