// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace Accord.Math.Optimization
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;
    using Accord.Compat;

    /// <summary>
    ///   Nonlinear objective function.
    /// </summary>
    /// 
    /// <example>
    /// <para>
    ///   In this framework, it is possible to state a non-linear programming problem
    ///   using either symbolic processing or vector-valued functions. The following 
    ///   example demonstrates the symbolic processing case:</para>
    ///   
    /// <code source="Unit Tests\Accord.Tests.Math\Optimization\AugmentedLagrangianTest.cs" region="doc_lambda"/>
    /// 
    /// <para>
    ///   And this is the same example as before, but using standard vectors instead.</para>
    ///   
    /// <code source="Unit Tests\Accord.Tests.Math\Optimization\AugmentedLagrangianTest.cs" region="doc_vector"/>
    /// </example>
    /// 
    /// <seealso cref="NelderMead"/>
    /// <seealso cref="Cobyla"/>
    /// <seealso cref="Subplex"/>
    /// <seealso cref="AugmentedLagrangian"/>
    /// <seealso cref="BroydenFletcherGoldfarbShanno"/>
    /// 
    public class NonlinearObjectiveFunction : IObjectiveFunction
    {

        private Dictionary<string, int> variables;
        private readonly IDictionary<string, int> readOnlyVariables;

        private Dictionary<int, string> indices;
        private readonly IDictionary<int, string> readOnlyIndices;

        /// <summary>
        ///   Gets the name of each input variable.
        /// </summary>
        /// 
        public IDictionary<string, int> Variables
        {
            get { return readOnlyVariables; }
        }

        /// <summary>
        ///   Gets the index of each input variable in the function.
        /// </summary>
        /// 
        public IDictionary<int, string> Indices
        {
            get { return readOnlyIndices; }
        }

        /// <summary>
        ///   Gets the name of each input variable.
        /// </summary>
        /// 
        protected Dictionary<string, int> InnerVariables { get { return variables; } }

        /// <summary>
        ///   Gets the index of each input variable in the function.
        /// </summary>
        /// 
        protected Dictionary<int, string> InnerIndices { get { return indices; } }

        /// <summary>
        ///   Gets the objective function.
        /// </summary>
        /// 
        public Func<double[], double> Function { get; protected set; }

        /// <summary>
        ///   Gets the gradient of the <see cref="Function">objective function</see>.
        /// </summary>
        /// 
        public Func<double[], double[]> Gradient { get; protected set; }


        /// <summary>
        ///   Gets the number of input variables for the function.
        /// </summary>
        /// 
        public int NumberOfVariables { get; protected set; }


        /// <summary>
        ///   Initializes a new instance of the <see cref="NonlinearObjectiveFunction"/> class.
        /// </summary>
        /// 
        protected NonlinearObjectiveFunction()
        {
            variables = new Dictionary<string, int>();
            indices = new Dictionary<int, string>();

            readOnlyVariables = new ReadOnlyDictionary<string, int>(variables);
            readOnlyIndices = new ReadOnlyDictionary<int, string>(indices);
        }

        /// <summary>
        ///   Creates a new objective function specified through a string.
        /// </summary>
        /// 
        /// <param name="numberOfVariables">The number of parameters in the <paramref name="function"/>.</param>
        /// <param name="function">A lambda expression defining the objective
        ///   function.</param>
        /// 
        public NonlinearObjectiveFunction(int numberOfVariables, Func<double[], double> function)
            : this()
        {
            this.NumberOfVariables = numberOfVariables;
            this.Function = function;

            for (int i = 0; i < numberOfVariables; i++)
            {
                string name = "x" + i;
                variables.Add(name, i);
                indices.Add(i, name);
            }
        }

        /// <summary>
        ///   Creates a new objective function specified through a string.
        /// </summary>
        /// 
        /// <param name="numberOfVariables">The number of parameters in the <paramref name="function"/>.</param>
        /// <param name="function">A lambda expression defining the objective
        ///   function.</param>
        /// <param name="gradient">A lambda expression defining the gradient
        ///   of the <paramref name="function">objective function</paramref>.</param>
        /// 
        public NonlinearObjectiveFunction(int numberOfVariables,
            Func<double[], double> function, Func<double[], double[]> gradient)
            : this(numberOfVariables, function)
        {
            this.Gradient = gradient;
        }

#if !NET35
        /// <summary>
        ///   Creates a new objective function specified through a lambda expression.
        /// </summary>
        /// 
        /// <param name="function">A <see cref="Expression{T}"/> containing 
        ///   the function in the form of a lambda expression.</param>
        /// <param name="gradient">A <see cref="Expression{T}"/> containing 
        ///   the gradient of the <paramref name="function">objective function</paramref>.</param>
        /// 
        public NonlinearObjectiveFunction(Expression<Func<double>> function, Expression<Func<double[]>> gradient = null)
            : this()
        {
            SortedSet<string> list = new SortedSet<string>();
            ExpressionParser.Parse(list, function.Body);

            int index = 0;
            foreach (string variable in list)
            {
                variables.Add(variable, index);
                indices.Add(index, variable);
                index++;
            }

            NumberOfVariables = index;

            // Generate lambda functions
            var func = ExpressionParser.Replace(function, variables);
            var grad = ExpressionParser.Replace(gradient, variables);

            this.Function = func.Compile();
            this.Gradient = grad.Compile();
        }
#endif



        internal static void CheckGradient(Func<double[], double[]> value, double[] probe)
        {
            double[] original = (double[])probe.Clone();
            double[] result = value(probe);

            if (result == probe)
                throw new InvalidOperationException(
                    "The gradient function should not return the parameter vector.");

            if (probe.Length != result.Length)
                throw new InvalidOperationException(
                    "The gradient vector should have the same length as the number of parameters.");

            for (int i = 0; i < probe.Length; i++)
                if (!probe[i].IsEqual(original[i], 0))
                    throw new InvalidOperationException("The gradient function shouldn't modify the parameter vector.");
        }

    }
}