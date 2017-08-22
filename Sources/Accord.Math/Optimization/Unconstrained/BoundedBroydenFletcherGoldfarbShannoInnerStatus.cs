// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
// Copyright © Jorge Nocedal, 1990
// http://users.eecs.northwestern.edu/~nocedal/
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
    using System.ComponentModel;

    /// <summary>
    ///   Inner status of the <see cref="BoundedBroydenFletcherGoldfarbShanno"/>
    ///   optimization algorithm. This class contains implementation details that
    ///   can change at any time.
    /// </summary>
    /// 
    public class BoundedBroydenFletcherGoldfarbShannoInnerStatus
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="BoundedBroydenFletcherGoldfarbShannoInnerStatus"/> class with the inner
        ///   status values from the original FORTRAN L-BFGS implementation.
        /// </summary>
        /// 
        /// <param name="isave">The isave L-BFGS status argument.</param>
        /// <param name="dsave">The dsave L-BFGS status argument.</param>
        /// <param name="lsave">The lsave L-BFGS status argument.</param>
        /// <param name="csave">The csave L-BFGS status argument.</param>
        /// <param name="work">The work L-BFGS status argument.</param>
        /// 
        public BoundedBroydenFletcherGoldfarbShannoInnerStatus(
            int[] isave, double[] dsave, bool[] lsave, String csave, double[] work)
        {
            this.Integers = (int[])isave.Clone();
            this.Doubles = (double[])dsave.Clone();
            this.Booleans = (bool[])lsave.Clone();
            this.Strings = csave;
            this.Work = (double[])work.Clone();
        }

        /// <summary>
        ///   Gets or sets the isave status from the
        ///   original FORTRAN L-BFGS implementation.
        /// </summary>
        /// 
        public int[] Integers { get; set; }

        /// <summary>
        ///   Gets or sets the dsave status from the
        ///   original FORTRAN L-BFGS implementation.
        /// </summary>
        /// 
        public double[] Doubles { get; set; }

        /// <summary>
        ///   Gets or sets the lsave status from the
        ///   original FORTRAN L-BFGS implementation.
        /// </summary>
        /// 
        public bool[] Booleans { get; set; }

        /// <summary>
        ///   Gets or sets the csave status from the
        ///   original FORTRAN L-BFGS implementation.
        /// </summary>
        /// 
        public String Strings { get; set; }

        /// <summary>
        ///   Gets or sets the work vector from the
        ///   original FORTRAN L-BFGS implementation.
        /// </summary>
        /// 
        public double[] Work { get; set; }
    }

}
