// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
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
    using System.Linq.Expressions;

    /// <summary>
    ///   Quadratic objective function.
    /// </summary>
    /// 
    public class NonlinearObjectiveFunction : IObjectiveFunction
    {

        private Dictionary<string, int> variables;
        private readonly ReadOnlyDictionary<string, int> readOnlyVariables;

        private Dictionary<int, string> indices;
        private ReadOnlyDictionary<int, string> readOnlyIndices;

        /// <summary>
        ///   Gets input variable's labels for the function.
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
        ///   Gets the objective function.
        /// </summary>
        /// 
        public Func<double[], double> Function { get; private set; }

        /// <summary>
        ///   Gets the gradient of the <see cref="Function">objective function</see>.
        /// </summary>
        /// 
        public Func<double[], double[]> Gradient { get; private set; }


        /// <summary>
        ///   Gets the number of input variables for the function.
        /// </summary>
        /// 
        public int NumberOfVariables { get; private set; }

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
            Func<double[], double> function,
            Func<double[], double[]> gradient = null)
        {
            this.NumberOfVariables = numberOfVariables;
            this.Function = function;
            this.Gradient = gradient;

            variables = new Dictionary<string, int>();
            readOnlyVariables = new ReadOnlyDictionary<string, int>(variables);

            indices = new Dictionary<int, string>();
            readOnlyIndices = new ReadOnlyDictionary<int, string>(indices);

            for (int i = 0; i < numberOfVariables; i++)
            {
                string name = "x" + i;
                variables.Add(name, i);
                indices.Add(i, name);
            }
        }

#if !NET35
        /// <summary>
        ///   Creates a new objective function specified through a string.
        /// </summary>
        /// 
        /// <param name="function">A <see cref="Expression{T}"/> containing 
        ///   the function in the form of a lambda expression.</param>
        /// <param name="gradient">A <see cref="Expression{T}"/> containing 
        ///   the gradient of the <paramref name="function">objective function</paramref>.</param>
        /// 
        public NonlinearObjectiveFunction(
            Expression<Func<double>> function,
            Expression<Func<double[]>> gradient = null)
        {
            variables = new Dictionary<string, int>();
            readOnlyVariables = new ReadOnlyDictionary<string, int>(variables);

            indices = new Dictionary<int, string>();
            readOnlyIndices = new ReadOnlyDictionary<int, string>(indices);

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

    }
}