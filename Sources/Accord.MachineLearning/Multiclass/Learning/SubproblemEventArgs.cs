// Accord Machine Learning Library
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

namespace Accord.MachineLearning
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Accord.Compat;
    using System.Threading.Tasks;

    /// <summary>
    ///   Subproblem progress event argument.
    /// </summary>
    /// 
    public class SubproblemEventArgs : EventArgs
    {
        /// <summary>
        ///   One of the classes belonging to the subproblem.
        /// </summary>
        /// 
        public int Class1 { get; set; }

        /// <summary>
        ///  One of the classes belonging to the subproblem.
        /// </summary>
        /// 
        public int Class2 { get; set; }

        /// <summary>
        ///   Gets the progress of the overall problem,
        ///   ranging from zero up to <see cref="Maximum"/>.
        /// </summary>
        /// 
        public int Progress { get; set; }

        /// <summary>
        ///   Gets the maximum value for the current <see cref="Progress"/>.
        /// </summary>
        public int Maximum { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SubproblemEventArgs"/> class.
        /// </summary>
        /// 
        /// <param name="class1">One of the classes in the subproblem.</param>
        /// <param name="class2">The other class in the subproblem.</param>
        /// 
        public SubproblemEventArgs(int class1, int class2)
        {
            this.Class1 = class1;
            this.Class2 = class2;
        }

    }
}
