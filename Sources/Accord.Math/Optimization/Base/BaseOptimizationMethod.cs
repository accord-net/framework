using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accord.Math.Optimization
{
    public abstract class BaseOptimizationMethod
    {

        private double[] x;
        private double value;

        /// <summary>
        ///   Gets or sets the function to be optimized.
        /// </summary>
        /// 
        /// <value>The function to be optimized.</value>
        /// 
        public Func<double[], double> Function { get; set; }

        /// <summary>
        ///   Gets the number of variables (free parameters)
        ///   in the optimization problem.
        /// </summary>
        /// 
        /// <value>The number of parameters.</value>
        /// 
        public int NumberOfVariables { get; protected set; }

        /// <summary>
        ///   Gets the solution found, the values of the
        ///   parameters which optimizes the function.
        /// </summary>
        /// 
        public double[] Solution
        {
            get { return x; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (value.Length != NumberOfVariables)
                    throw new DimensionMismatchException("value");

                x = value;
            }
        }

        /// <summary>
        ///   Gets the output of the function at the current solution.
        /// </summary>
        /// 
        public double Value
        {
            get { return value; }
            protected set { this.value = value; }
        }

        public BaseOptimizationMethod(int numberOfVariables)
        {
            if (numberOfVariables <= 0)
                throw new ArgumentOutOfRangeException("numberOfVariables");

            this.NumberOfVariables = numberOfVariables;
            this.Solution = new double[numberOfVariables];

            for (int i = 0; i < Solution.Length; i++)
                Solution[i] = Accord.Math.Tools.Random.NextDouble() * 2 - 1;
        }

        public BaseOptimizationMethod(int numberOfVariables, Func<double[], double> function)
            : this(numberOfVariables)
        {
            if (function == null)
                throw new ArgumentNullException("function");

            this.Function = function;
        }

        public bool Maximize(double[] values)
        {
            Solution = values;
            return Maximize();
        }

        public bool Minimize(double[] values)
        {
            Solution = values;
            return Minimize();
        }

        public virtual bool Maximize()
        {
            if (Function == null)
                throw new InvalidOperationException("function");

            var f = Function;

            Function = (x) => -f(x);

            bool success = Optimize();

            Function = f;

            value = Function(Solution);

            return success;
        }

        public virtual bool Minimize()
        {
            if (Function == null)
                throw new InvalidOperationException("function");

            bool success = Optimize();

            value = Function(Solution);

            return success;
        }


        protected abstract bool Optimize();


        protected ArgumentOutOfRangeException ArgumentException(string paramName, string message, string code)
        {
            var e = new ArgumentOutOfRangeException(paramName, message);
            e.Data["Code"] = code;
            return e;
        }

        protected InvalidOperationException OperationException(string message, string code)
        {
            var e = new InvalidOperationException(message);
            e.Data["Code"] = code;
            return e;
        }
    }
}
