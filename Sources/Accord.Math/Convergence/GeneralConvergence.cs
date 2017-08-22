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

namespace Accord.Math.Convergence
{
    using System;

    /// <summary>
    ///   General convergence options.
    /// </summary>
    /// 
    public class GeneralConvergence 
    {
        private int n;
        private double ftol_rel;
        private double ftol_abs;
        private double xtol_rel;
        private double[] xtol_abs;
        private int nevals;
        private int maxeval;
        private TimeSpan maxtime;
        private DateTime start;
        private bool force_stop;

        /// <summary>
        ///   Creates a new <see cref="GeneralConvergence"/> object.
        /// </summary>
        /// 
        /// <param name="numberOfVariables">The number of variables to be tracked.</param>
        /// 
        public GeneralConvergence(int numberOfVariables)
        {
            this.n = numberOfVariables;
            this.xtol_abs = new double[n];
        }

        /// <summary>
        ///   Gets or sets the number of variables in the problem.
        /// </summary>
        /// 
        public int NumberOfVariables
        {
            get { return n; }
            set { n = value; }
        }

        /// <summary>
        ///   Gets or sets the relative function tolerance that should
        ///   be used as convergence criteria. This tracks the relative
        ///   amount that the function output changes after two consecutive
        ///   iterations. Setting this value to zero disables those checks.
        ///   Default is 0.
        /// </summary>
        /// 
        public double RelativeFunctionTolerance
        {
            get { return ftol_rel; }
            set { ftol_rel = value; }
        }

        /// <summary>
        ///   Gets or sets the absolute function tolerance that should
        ///   be used as convergence criteria. This tracks the absolute
        ///   amount that the function output changes after two consecutive
        ///   iterations. Setting this value to zero disables those checks.
        ///   Default is 0.
        /// </summary>
        /// 
        public double AbsoluteFunctionTolerance
        {
            get { return ftol_abs; }
            set { ftol_abs = value; }
        }

        /// <summary>
        ///   Gets or sets the relative parameter tolerance that should
        ///   be used as convergence criteria. This tracks the relative
        ///   amount that the model parameters changes after two consecutive
        ///   iterations. Setting this value to zero disables those checks.
        ///   Default is 0.
        /// </summary>
        /// 
        public double RelativeParameterTolerance
        {
            get { return xtol_rel; }
            set { xtol_rel = value; }
        }

        /// <summary>
        ///   Gets or sets the absolute parameter tolerance that should
        ///   be used as convergence criteria. This tracks the absolute
        ///   amount that the model parameters changes after two consecutive
        ///   iterations. Setting this value to zero disables those checks.
        ///   Default is 0.
        /// </summary>
        /// 
        public double[] AbsoluteParameterTolerance
        {
            get { return xtol_abs; }
            set { xtol_abs = value; }
        }

        /// <summary>
        ///   Gets or sets the number of function evaluations 
        ///   performed by the optimization algorithm.
        /// </summary>
        /// 
        public int Evaluations
        {
            get { return nevals; }
            set { nevals = value; }
        }

        /// <summary>
        ///   Gets or sets the maximum number of function evaluations to
        ///   be used as convergence criteria. This tracks how many times
        ///   the function to be optimized has been called, and stops the
        ///   algorithm when the number of times specified in this property
        ///   has been reached. Setting this value to zero disables this check.
        ///   Default is 0.
        /// </summary>
        /// 
        public int MaximumEvaluations
        {
            get { return maxeval; }
            set { maxeval = value; }
        }

        /// <summary>
        ///   Gets or sets the maximum amount of time that an optimization
        ///   algorithm is allowed to run. This property must be set together
        ///   with <see cref="StartTime"/> in order to function correctly. 
        ///   Setting this value to <see cref="TimeSpan.Zero"/> disables this
        ///   check. Default is <see cref="TimeSpan.Zero"/>.
        /// </summary>
        /// 
        public TimeSpan MaximumTime
        {
            get { return maxtime; }
            set { maxtime = value; }
        }

        /// <summary>
        ///   Gets or sets the time when the algorithm started running. When
        ///   time will be tracked with the <see cref="MaximumTime"/> property,
        ///   this property must also be set to a correct value.
        /// </summary>
        /// 
        public DateTime StartTime
        {
            get { return start; }
            set { start = value; }
        }

        /// <summary>
        ///   Gets or sets whether the algorithm should
        ///   be forced to terminate. Default is false.
        /// </summary>
        /// 
        public bool Cancel
        {
            get { return force_stop; }
            set { force_stop = value; }
        }

    }
}
