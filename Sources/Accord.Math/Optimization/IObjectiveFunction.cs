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

    /// <summary>
    ///   Common interface for specifying objective functions.
    /// </summary>
    /// 
    public interface IObjectiveFunction
    {

        /// <summary>
        ///   Gets input variable's labels for the function.
        /// </summary>
        /// 
        IDictionary<string, int> Variables { get; }

        /// <summary>
        ///   Gets the index of each input variable in the function.
        /// </summary>
        /// 
        IDictionary<int, string> Indices { get; }

        /// <summary>
        ///   Gets the number of input variables for the function.
        /// </summary>
        /// 
        int NumberOfVariables { get; }

        /// <summary>
        ///   Gets the objective function.
        /// </summary>
        /// 
        Func<double[], double> Function { get; }

    }
}
