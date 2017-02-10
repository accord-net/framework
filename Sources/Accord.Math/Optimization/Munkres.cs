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
//
// This class contains the original code written by Robert A. Pilgrim, originally 
// shared under the permissive MIT license. The original license text is reproduced
// below:
//  
//    The MIT License (MIT)
//  
//    Copyright (c) 2000 Robert A. Pilgrim
//                       Murray State University
//                       Dept. of Computer Science & Information Systems
//                       Murray,Kentucky
//  
//    Permission is hereby granted, free of charge, to any person obtaining a copy
//    of this software and associated documentation files (the "Software"), to deal
//    in the Software without restriction, including without limitation the rights
//    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//    copies of the Software, and to permit persons to whom the Software is
//    furnished to do so, subject to the following conditions:
//  
//    The above copyright notice and this permission notice shall be included in all
//    copies or substantial portions of the Software.
//  
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//   SOFTWARE.
//  

namespace Accord.Math.Optimization
{
    using Accord.MachineLearning;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    ///   Hungarian method for solving the assignment problem, also known
    ///   as the Kuhn–Munkres algorithm or Munkres assignment algorithm. 
    /// </summary>
    /// 
    ///   
    /// <remarks>
    /// <para>
    ///   The Hungarian method is a combinatorial optimization algorithm that solves the assignment
    ///   problem in polynomial time and which anticipated later primal-dual methods. It was developed
    ///   and published in 1955 by Harold Kuhn, who gave the name "Hungarian method" because the algorithm 
    ///   was largely based on the earlier works of two Hungarian mathematicians: Dénes Kőnig and Jenő
    ///   Egerváry.</para>
    /// <para>
    ///   James Munkres reviewed the algorithm in 1957 and observed that it is (strongly) polynomial. 
    ///   Since then the algorithm has been known also as the Kuhn–Munkres algorithm or Munkres assignment
    ///   algorithm.The time complexity of the original algorithm was O(n^4), however Edmonds and Karp, and 
    ///   independently Tomizawa noticed that it can be modified to achieve an O(n^3) running time. Ford and 
    ///   Fulkerson extended the method to general transportation problems. In 2006, it was discovered that
    ///   Carl Gustav Jacobi had solved the assignment problem in the 19th century, and the solution had been
    ///   published posthumously in 1890 in Latin.</para>
    /// 
    /// <para>
    ///   This code has been based on the original MIT-licensed code by R.A. Pilgrim,
    ///   available in http://csclab.murraystate.edu/~bob.pilgrim/445/munkres.html. </para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://csclab.murraystate.edu/~bob.pilgrim/445/munkres.html">
    ///       R. A. Pilgrim (2000). Munkres' Assignment Algorithm Modified for 
    ///       Rectangular Matrices. Available in http://csclab.murraystate.edu/~bob.pilgrim/445/munkres.html </a></description></item>
    ///     <item><description><a href="https://en.wikipedia.org/wiki/Hungarian_algorithm">
    ///       Wikipedia contributors. "Hungarian algorithm." Wikipedia, The Free Encyclopedia.
    ///       Wikipedia, The Free Encyclopedia, 23 Jan. 2016. </a></description></item>
    ///   </list>
    /// </para>   
    /// </remarks>
    /// 
    /// <example>
    /// <code source="Unit Tests\Accord.Tests.Math\Optimization\MunkresTest.cs" region="doc_example" />
    /// </example>
    /// 
    /// <seealso cref="Accord.Math.Optimization.IOptimizationMethod" />
    /// 
    public class Munkres : IOptimizationMethod
    {
        internal double[][] C;
        internal int[][] M;
        internal List<Tuple<int, int>> path;
        internal int[] RowCover;
        internal int[] ColCover;

        private int path_row_0;
        private int path_col_0;

        /// <summary>
        ///   Gets or sets the cost matrix for this assignment algorithm.
        /// </summary>
        /// 
        /// <value>The cost matrix.</value>
        /// 
        public double[][] CostMatrix { get; protected set; }

        /// <summary>
        /// Gets or sets the number of variables in this optimization problem
        /// (<see cref="NumberOfTasks"/> * <see cref="NumberOfWorkers"/>).
        /// </summary>
        /// 
        public int NumberOfVariables
        {
            get { return NumberOfTasks * NumberOfWorkers; }
            set { throw new InvalidOperationException(); }
        }

        /// <summary>
        ///   Gets the number of variables (free parameters)
        ///   in the optimization problem. In the assigment
        ///   problem, this gives the number of jobs (or tasks)
        ///   to be performed.
        /// </summary>
        /// 
        /// <value>The number of tasks in the assignment problem.</value>
        /// 
        public int NumberOfTasks
        {
            get; protected set;
        }

        /// <summary>
        ///   Gets or sets the number of workers in the assignment algorithm.
        ///   The workers are the entites that can be assigned jobs according
        ///   to the costs in the <see cref="CostMatrix"/>.
        /// </summary>
        /// 
        /// <value>The number of workers.</value>
        /// 
        public int NumberOfWorkers
        {
            get; protected set;
        }

        /// <summary>
        /// Gets the current solution found, the values of
        /// the parameters which optimizes the function.
        /// </summary>
        /// 
        /// <value>The solution.</value>
        /// 
        public double[] Solution { get; set; }

        /// <summary>
        ///   Gets the output of the function at the current <see cref="Solution" />.
        /// </summary>
        /// 
        /// <value>The value.</value>
        /// 
        public double Value { get; protected set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="Munkres"/> class.
        /// </summary>
        /// 
        /// <param name="numberOfJobs">The number of jobs (tasks) that can be assigned.</param>
        /// <param name="numberOfWorkers">The number of workers that can receive an assignment.</param>
        /// 
        public Munkres(int numberOfJobs, int numberOfWorkers)
        {
            init(numberOfJobs, numberOfWorkers, Jagged.Zeros(numberOfWorkers, numberOfJobs));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Munkres"/> class.
        /// </summary>
        /// 
        /// <param name="costMatrix">The cost matrix where each row represents
        ///   a worker, each column represents a task, and each individual element
        ///   represents how much it costs for a particular worker to receive (be
        ///   assigned) a particular task.</param>
        /// 
        public Munkres(double[][] costMatrix)
        {
            int workers = costMatrix.Rows();
            int jobs = costMatrix.Columns();
            init(jobs, workers, costMatrix);
        }

        private void init(int jobs, int workers, double[][] costMatrix)
        {
            this.NumberOfTasks = jobs;
            this.NumberOfWorkers = workers;
            this.RowCover = new int[workers];
            this.ColCover = new int[jobs];
            this.CostMatrix = costMatrix;
            this.M = Jagged.CreateAs<double, int>(costMatrix);
            this.path = new List<Tuple<int, int>>();
            this.Solution = new double[jobs];
        }

        /// <summary>
        ///   Finds the minimum value of a function. The solution vector
        ///   will be made available at the <see cref="Solution" /> property.
        /// </summary>
        /// 
        /// <returns>Returns <c>true</c> if the method converged to a <see cref="Solution" />.
        /// In this case, the found value will also be available at the <see cref="Value" />
        /// property.</returns>
        /// 
        public bool Minimize()
        {
            return run(CostMatrix.Copy());
        }


        /// <summary>
        ///   Finds the maximum value of a function. The solution vector
        ///   will be made available at the <see cref="Solution" /> property.
        /// </summary>
        /// 
        /// <returns>Returns <c>true</c> if the method converged to a <see cref="Solution" />.
        /// In this case, the found value will also be available at the <see cref="Value" />
        /// property.</returns>
        /// 
        public bool Maximize()
        {
            return run(CostMatrix.Multiply(-1));
        }

        private bool run(double[][] m)
        {
            this.C = m;
            int step = 1;

            while (step > 0)
            {
                step = RunStep(step);
            }

            return true;
        }


        internal int RunStep(int step)
        {
            switch (step)
            {
                case 1:
                    return step_one();
                case 2:
                    return step_two();
                case 3:
                    return step_three();
                case 4:
                    return step_four();
                case 5:
                    return step_five();
                case 6:
                    return step_six();
                case 7:
                    return step_seven();
                default:
                    throw new InvalidOperationException();
            }
        }



        /// <summary>
        ///  For each row of the cost matrix, find the smallest element
        ///  and subtract it from every element in its row.
        /// </summary>
        /// 
        /// <returns>Go to step 2.</returns>
        /// 
        private int step_one()
        {
            for (int r = 0; r < C.Length; r++)
                C[r].Subtract(C[r].Min(), result: C[r]);

            return 2;
        }

        /// <summary>
        ///   Find a zero (Z) in the resulting matrix. If there is no starred 
        ///   zero in its row or column, star Z. Repeat for each element in the 
        ///   matrix.
        /// </summary>
        /// 
        /// <returns>Go to step 3.</returns>
        /// 
        private int step_two()
        {
            M.Clear();
            for (int r = 0; r < RowCover.Length; r++)
                for (int c = 0; c < ColCover.Length; c++)
                    if (C[r][c] == 0 && RowCover[r] == 0 && ColCover[c] == 0)
                        ColCover[c] = RowCover[r] = M[r][c] = 1;

            RowCover.Clear();
            ColCover.Clear();
            return 3;
        }

        /// <summary>
        ///   Cover each column containing a starred zero. If K columns are covered, 
        ///   the starred zeros describe a complete set of unique assignments. In this 
        ///   case, go to DONE, otherwise, go to Step 4.
        /// </summary>
        /// 
        /// <returns>If K columns are covered, returns 7. Otherwise, returns 4.</returns>
        /// 
        private int step_three()
        {
            for (int r = 0; r < M.Length; r++)
                for (int c = 0; c < M[r].Length; c++)
                    if (M[r][c] == 1)
                        ColCover[c] = 1;

            int count = 0;
            for (int c = 0; c < ColCover.Length; c++)
                if (ColCover[c] == 1)
                    count += 1;

            if (count >= ColCover.Length || count >= RowCover.Length)
                return 7; // done
            return 4;
        }


        /// <summary>
        ///   Find a noncovered zero and prime it. If there is no starred zero 
        ///   in the row containing this primed zero, Go to Step 5. Otherwise, 
        ///   cover this row and uncover the column containing the starred zero. 
        ///   Continue in this manner until there are no uncovered zeros left. 
        ///   Save the smallest uncovered value and Go to Step 6.        
        /// </summary>
        /// 
        /// <returns>Goes to step 5 or 6.</returns>
        /// 
        private int step_four()
        {
            int row = -1;
            int col = -1;

            while (true)
            {
                if (!find_a_zero(out row, out col))
                    return 6;

                M[row][col] = 2;
                if (has_star_in_row(row))
                {
                    col = find_star_in_row(row);
                    RowCover[row] = 1;
                    ColCover[col] = 0;
                }
                else
                {
                    this.path_row_0 = row;
                    this.path_col_0 = col;
                    return 5;
                }
            }
        }

        // methods to support step 4
        private bool find_a_zero(out int row, out int col)
        {
            for (int r = 0; r < RowCover.Length; r++)
            {
                for (int c = 0; c < ColCover.Length; c++)
                {
                    if (C[r][c] == 0 && RowCover[r] == 0 && ColCover[c] == 0)
                    {
                        row = r;
                        col = c;
                        return true;
                    }
                }
            }

            row = col = -1;
            return false;
        }

        private bool has_star_in_row(int row)
        {
            for (int c = 0; c < M[row].Length; c++)
                if (M[row][c] == 1)
                    return true;
            return false;
        }

        private int find_star_in_row(int row)
        {
            int col = -1;
            for (int c = 0; c < M[row].Length; c++)
                if (M[row][c] == 1)
                    col = c;
            return col;
        }




        /// <summary>
        ///   Construct a series of alternating primed and starred zeros as follows.  
        ///   Let Z0 represent the uncovered primed zero found in Step 4. Let Z1 denote 
        ///   the starred zero in the column of Z0 (if any). Let Z2 denote the primed zero 
        ///   in the row of Z1 (there will always be one). Continue until the series 
        ///   terminates at a primed zero that has no starred zero in its column.  
        ///   Unstar each starred zero of the series, star each primed zero of the series, 
        ///   erase all primes and uncover every line in the matrix.  
        /// </summary>
        /// 
        /// <returns>Return to Step 3.</returns>
        /// 
        private int step_five()
        {
            int c = -1;
            int r;
            path.Clear();
            path.Add(Tuple.Create(path_row_0, path_col_0));

            while (find_star_in_col(path[path.Count - 1].Item2, out r))
            {
                path.Add(Tuple.Create(r, path[path.Count - 1].Item2));
                find_prime_in_row(path[path.Count - 1].Item1, ref c);
                path.Add(Tuple.Create(path[path.Count - 1].Item1, c));
            }

            augment_path();
            RowCover.Clear();
            ColCover.Clear();
            erase_primes();
            return 3;
        }


        // methods to support step 5
        private bool find_star_in_col(int c, out int r)
        {
            r = -1;
            for (int i = 0; i < M.Length; i++)
                if (M[i][c] == 1)
                    r = i;
            return r >= 0;
        }

        private void find_prime_in_row(int r, ref int c)
        {
            for (int j = 0; j < M[r].Length; j++)
                if (M[r][j] == 2)
                    c = j;
        }

        private void augment_path()
        {
            for (int p = 0; p < path.Count; p++)
            {
                int i = path[p].Item1;
                int j = path[p].Item2;
                M[i][j] = (M[i][j] == 1) ? 0 : 1;
            }
        }

        private void erase_primes()
        {
            for (int r = 0; r < M.Length; r++)
                for (int c = 0; c < M[r].Length; c++)
                    if (M[r][c] == 2)
                        M[r][c] = 0;
        }


        /// <summary>
        ///   Add the value found in Step 4 to every element of each covered row, and subtract 
        ///   it from every element of each uncovered column.  
        /// </summary>
        /// 
        /// <returns>Return to step 4.</returns>
        /// 
        private int step_six()
        {
            double minval = find_smallest();
            for (int r = 0; r < RowCover.Length; r++)
            {
                for (int c = 0; c < ColCover.Length; c++)
                {
                    if (RowCover[r] == 1)
                        C[r][c] += minval;
                    if (ColCover[c] == 0)
                        C[r][c] -= minval;
                }
            }

            return 4;
        }

        //methods to support step 6
        private double find_smallest()
        {
            double minval = Double.PositiveInfinity;
            for (int r = 0; r < RowCover.Length; r++)
                for (int c = 0; c < ColCover.Length; c++)
                    if (RowCover[r] == 0 && ColCover[c] == 0)
                        if (minval > C[r][c])
                            minval = C[r][c];
            return minval;
        }

        private int step_seven()
        {
            // DONE: Assignment pairs are indicated by the positions of the starred zeros in the 
            // cost matrix.If C(i, j) is a starred zero, then the element associated with row i 
            // is assigned to the element associated with column j.
            //
            //                     (http://csclab.murraystate.edu/~bob.pilgrim/445/munkres.html)
            double value = 0;
            for (int col = 0; col < M.Columns(); col++)
            {
                int row;
                if (find_star_in_col(col, out row))
                    value += CostMatrix[row][col];
                Solution[col] = row;
            }

            Value = value;
            return -1;
        }


    }
}