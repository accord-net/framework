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

namespace Accord.Math
{
    using System;

    /// <summary>
    ///   Relative parameter change convergence criteria.
    /// </summary>
    /// 
    /// <remarks>
    ///   This class can be used to track progress and convergence
    ///   of methods which rely on the maximum relative change of
    ///   the values within a parameter vector.
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    ///   // Converge if the maximum change amongst all parameters is less than 0.1:
    ///   var criteria = new RelativeParameterConvergence(iterations: 0, tolerance: 0.1);
    /// 
    ///   int progress = 1;
    ///   double[] parameters = { 12345.6, 952.12, 1925.1 };
    ///   
    ///   do
    ///   {
    ///       // Do some processing...
    /// 
    ///       // Update current iteration information:
    ///       criteria.NewValues = parameters.Divide(progress++);
    /// 
    ///   } while (!criteria.HasConverged);
    /// 
    /// 
    ///   // The method will converge after reaching the 
    ///   // maximum of 11 iterations with a final value
    ///   // of { 1234.56, 95.212, 192.51 }:
    /// 
    ///   int iterations = criteria.CurrentIteration; // 11
    ///   var v = criteria.OldValues; // { 1234.56, 95.212, 192.51 }
    /// 
    /// </code>
    /// </example>
    /// 
    public class RelativeParameterConvergence
    {
        private double[] oldValues;
        private double[] newValues;

        private double tolerance = 0;
        private int maxIterations = 100;
        private double maxChange;


        /// <summary>
        ///   Gets or sets the maximum change in the watched value
        ///   after an iteration of the algorithm used to detect convergence.
        /// </summary>
        /// 
        public double Tolerance
        {
            get { return tolerance; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", "Tolerance should be positive.");

                tolerance = value;
            }
        }

        /// <summary>
        ///   Gets or sets the maximum number of iterations
        ///   performed by the iterative algorithm.
        /// </summary>
        /// 
        public int Iterations
        {
            get { return maxIterations; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value",
                        "The maximum number of iterations should be positive.");

                maxIterations = value;
            }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="RelativeParameterConvergence"/> class.
        /// </summary>
        /// 
        public RelativeParameterConvergence()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="RelativeParameterConvergence"/> class.
        /// </summary>
        /// 
        /// <param name="iterations">The maximum number of iterations which should be
        ///   performed by the iterative algorithm. Setting to zero indicates there
        ///   is no maximum number of iterations. Default is 0.</param>
        /// <param name="tolerance">The maximum relative change in the watched value
        ///   after an iteration of the algorithm used to detect convergence.
        ///   Default is 0.</param>
        /// 
        public RelativeParameterConvergence(int iterations, double tolerance)
        {
            this.Iterations = iterations;
            this.tolerance = tolerance;
        }

        /// <summary>
        ///   Gets the maximum relative parameter
        ///   change after the last iteration.
        /// </summary>
        /// 
        public double Delta
        {
            get { return maxChange; }
        }

        /// <summary>
        ///   Gets or sets the watched value before the iteration.
        /// </summary>
        /// 
        public double[] OldValues { get { return oldValues; } }


        /// <summary>
        ///   Gets or sets the watched value after the iteration.
        /// </summary>
        /// 
        public double[] NewValues
        {
            get { return newValues; }
            set
            {
                oldValues = newValues;
                newValues = (double[])value.Clone();
                CurrentIteration++;
            }
        }

        /// <summary>
        ///   Gets or sets the current iteration number.
        /// </summary>
        /// 
        public int CurrentIteration { get; set; }

        /// <summary>
        ///   Gets whether the algorithm has diverged.
        /// </summary>
        /// 
        public bool HasDiverged
        {
            get
            {
                for (int i = 0; i < NewValues.Length; i++)
                    if (Double.IsNaN(NewValues[i]) || Double.IsInfinity(NewValues[i]))
                        return true;
                return false;
            }
        }

        /// <summary>
        ///   Gets whether the algorithm has converged.
        /// </summary>
        /// 
        public bool HasConverged
        {
            get
            {
                if (NewValues == null && OldValues == null)
                    return true;
                if (OldValues == null)
                    return false;
                if (NewValues.Length == 0 || OldValues.Length == 0)
                    return true;

                // Check if we have reached an invalid or perfectly separable answer
                for (int i = 0; i < NewValues.Length; i++)
                    if (Double.IsNaN(NewValues[i]) || Double.IsInfinity(NewValues[i]))
                        return true;

                // Update and verify stop criteria
                if (tolerance > 0)
                {
                    // Stopping criteria is likelihood convergence
                    maxChange = Math.Abs(OldValues[0] - NewValues[0]) / Math.Abs(OldValues[0]);

                    for (int i = 1; i < OldValues.Length; i++)
                    {
                        double delta = Math.Abs(OldValues[i] - NewValues[i]) / Math.Abs(OldValues[i]);

                        if (delta > maxChange)
                            maxChange = delta;
                    }


                    if (maxChange <= tolerance)
                        return true;

                    if (maxIterations > 0)
                    {
                        // Maximum iterations should also be respected
                        if (CurrentIteration >= maxIterations)
                            return true;
                    }
                }
                else
                {
                    // Stopping criteria is number of iterations
                    if (CurrentIteration == maxIterations)
                        return true;
                }

                return false;
            }
        }


        /// <summary>
        ///   Clears this instance.
        /// </summary>
        /// 
        public void Clear()
        {
            CurrentIteration = 0;
            newValues = null;
            oldValues = null;
        }
    }
}
