/*
    The MIT License (MIT)

    Copyright (c) 2000 Robert A. Pilgrim
                       Murray State University
                       Dept. of Computer Science & Information Systems
                       Murray,Kentucky

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
   SOFTWARE.

 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munkres
{
    // This is the original code from http://csclab.murraystate.edu/~bob.pilgrim/445/munkres.html
    // with minimal modifications. The only included modifications aim to increase the debugability
    // of the original code and did not alter its behavior.

    public static class MunkresProgram
    {
        const int MAX = 35;
        public static double[,] C = new double[MAX, MAX];
        public static double[,] C_orig = new double[MAX, MAX];
        public static double[,] M = new double[MAX, MAX];
        public static int[,] path = new int[2 * MAX + 1, 2];
        public static int[] RowCover = new int[MAX];
        public static int[] ColCover = new int[MAX];
        public static int nrow;
        public static int ncol;
        public static int path_count = 0;
        public static int path_row_0;
        public static int path_col_0;
        public static int asgn = 0;
        public static int step;

        private static void resetMaskandCovers()
        {
            for (int r = 0; r < nrow; r++)
            {
                RowCover[r] = 0;
                for (int c = 0; c < ncol; c++)
                {
                    M[r, c] = 0;
                }
            }
            for (int c = 0; c < ncol; c++)
                ColCover[c] = 0;
        }

        //For each row of the cost matrix, find the smallest element and subtract
        //it from every element in its row.  When finished, Go to Step 2.
        private static void step_one(ref int step)
        {
            double min_in_row;

            for (int r = 0; r < nrow; r++)
            {
                min_in_row = C[r, 0];
                for (int c = 0; c < ncol; c++)
                    if (C[r, c] < min_in_row)
                        min_in_row = C[r, c];
                for (int c = 0; c < ncol; c++)
                    C[r, c] -= min_in_row;
            }
            step = 2;
        }

        //Find a zero (Z) in the resulting matrix.  If there is no starred 
        //zero in its row or column, star Z. Repeat for each element in the 
        //matrix. Go to Step 3.
        private static void step_two(ref int step)
        {
            for (int r = 0; r < nrow; r++)
                for (int c = 0; c < ncol; c++)
                {
                    if (C[r, c] == 0 && RowCover[r] == 0 && ColCover[c] == 0)
                    {
                        M[r, c] = 1;
                        RowCover[r] = 1;
                        ColCover[c] = 1;
                    }
                }
            for (int r = 0; r < nrow; r++)
                RowCover[r] = 0;
            for (int c = 0; c < ncol; c++)
                ColCover[c] = 0;
            step = 3;
        }

        //Cover each column containing a starred zero.  If K columns are covered, 
        //the starred zeros describe a complete set of unique assignments.  In this 
        //case, Go to DONE, otherwise, Go to Step 4.
        private static void step_three(ref int step)
        {
            int colcount;
            for (int r = 0; r < nrow; r++)
                for (int c = 0; c < ncol; c++)
                    if (M[r, c] == 1)
                        ColCover[c] = 1;

            colcount = 0;
            for (int c = 0; c < ncol; c++)
                if (ColCover[c] == 1)
                    colcount += 1;
            if (colcount >= ncol || colcount >= nrow)
                step = 7;
            else
                step = 4;
        }

        //methods to support step 4
        private static void find_a_zero(ref int row, ref int col)
        {
            int r = 0;
            int c;
            bool done;
            row = -1;
            col = -1;
            done = false;
            while (!done)
            {
                c = 0;
                while (true)
                {
                    if (C[r, c] == 0 && RowCover[r] == 0 && ColCover[c] == 0)
                    {
                        row = r;
                        col = c;
                        done = true;
                    }
                    c += 1;
                    if (c >= ncol || done)
                        break;
                }
                r += 1;
                if (r >= nrow)
                    done = true;
            }
        }

        private static bool star_in_row(int row)
        {
            bool tmp = false;
            for (int c = 0; c < ncol; c++)
                if (M[row, c] == 1)
                    tmp = true;
            return tmp;
        }

        private static void find_star_in_row(int row, ref int col)
        {
            col = -1;
            for (int c = 0; c < ncol; c++)
                if (M[row, c] == 1)
                    col = c;
        }

        //Find a noncovered zero and prime it.  If there is no starred zero 
        //in the row containing this primed zero, Go to Step 5.  Otherwise, 
        //cover this row and uncover the column containing the starred zero. 
        //Continue in this manner until there are no uncovered zeros left. 
        //Save the smallest uncovered value and Go to Step 6.
        private static void step_four(ref int step)
        {
            int row = -1;
            int col = -1;
            bool done;

            done = false;
            while (!done)
            {
                find_a_zero(ref row, ref col);
                if (row == -1)
                {
                    done = true;
                    step = 6;
                }
                else
                {
                    M[row, col] = 2;
                    if (star_in_row(row))
                    {
                        find_star_in_row(row, ref col);
                        RowCover[row] = 1;
                        ColCover[col] = 0;
                    }
                    else
                    {
                        done = true;
                        step = 5;
                        path_row_0 = row;
                        path_col_0 = col;
                    }
                }
            }
        }

        // methods to support step 5
        private static void find_star_in_col(int c, ref int r)
        {
            r = -1;
            for (int i = 0; i < nrow; i++)
                if (M[i, c] == 1)
                    r = i;
        }

        private static void find_prime_in_row(int r, ref int c)
        {
            for (int j = 0; j < ncol; j++)
                if (M[r, j] == 2)
                    c = j;
        }

        private static void augment_path()
        {
            for (int p = 0; p < path_count; p++)
                if (M[path[p, 0], path[p, 1]] == 1)
                    M[path[p, 0], path[p, 1]] = 0;
                else
                    M[path[p, 0], path[p, 1]] = 1;
        }

        private static void clear_covers()
        {
            for (int r = 0; r < nrow; r++)
                RowCover[r] = 0;
            for (int c = 0; c < ncol; c++)
                ColCover[c] = 0;
        }

        private static void erase_primes()
        {
            for (int r = 0; r < nrow; r++)
                for (int c = 0; c < ncol; c++)
                    if (M[r, c] == 2)
                        M[r, c] = 0;
        }


        //Construct a series of alternating primed and starred zeros as follows.  
        //Let Z0 represent the uncovered primed zero found in Step 4.  Let Z1 denote 
        //the starred zero in the column of Z0 (if any). Let Z2 denote the primed zero 
        //in the row of Z1 (there will always be one).  Continue until the series 
        //terminates at a primed zero that has no starred zero in its column.  
        //Unstar each starred zero of the series, star each primed zero of the series, 
        //erase all primes and uncover every line in the matrix.  Return to Step 3.
        private static void step_five(ref int step)
        {
            bool done;
            int r = -1;
            int c = -1;

            path_count = 1;
            path[path_count - 1, 0] = path_row_0;
            path[path_count - 1, 1] = path_col_0;
            done = false;
            while (!done)
            {
                find_star_in_col(path[path_count - 1, 1], ref r);
                if (r > -1)
                {
                    path_count += 1;
                    path[path_count - 1, 0] = r;
                    path[path_count - 1, 1] = path[path_count - 2, 1];

                    find_prime_in_row(path[path_count - 1, 0], ref c);
                    path_count += 1;
                    path[path_count - 1, 0] = path[path_count - 2, 0];
                    path[path_count - 1, 1] = c;
                }
                else
                    done = true;
            }
            augment_path();
            clear_covers();
            erase_primes();
            step = 3;
        }

        //methods to support step 6
        private static void find_smallest(ref double minval)
        {
            for (int r = 0; r < nrow; r++)
                for (int c = 0; c < ncol; c++)
                    if (RowCover[r] == 0 && ColCover[c] == 0)
                        if (minval > C[r, c])
                            minval = C[r, c];
        }

        //Add the value found in Step 4 to every element of each covered row, and subtract 
        //it from every element of each uncovered column.  Return to Step 4 without 
        //altering any stars, primes, or covered lines.
        private static void step_six(ref int step)
        {
            double minval = Double.MaxValue;
            find_smallest(ref minval);
            for (int r = 0; r < nrow; r++)
                for (int c = 0; c < ncol; c++)
                {
                    if (RowCover[r] == 1)
                        C[r, c] += minval;
                    if (ColCover[c] == 0)
                        C[r, c] -= minval;
                }
            step = 4;
        }

        private static void step_seven(ref int step)
        {
            Console.WriteLine("\n\n---------Run Complete----------");
        }

        public static void RunMunkres(double[,] C)
        {
            Init(C);

            bool done = false;
            while (!done)
            {
                ShowCostMatrix();
                ShowMaskMatrix();
                done = RunStep(done);
            }
        }

        public static void Init(double[,] C)
        {
            MunkresProgram.C = C;
            MunkresProgram.nrow = C.GetLength(0);
            MunkresProgram.ncol = C.GetLength(1);
            resetMaskandCovers();

            step = 1;
        }

        public static bool RunStep(bool done)
        {
            switch (step)
            {
                case 1:
                    step_one(ref step);
                    break;
                case 2:
                    step_two(ref step);
                    break;
                case 3:
                    step_three(ref step);
                    break;
                case 4:
                    step_four(ref step);
                    break;
                case 5:
                    step_five(ref step);
                    break;
                case 6:
                    step_six(ref step);
                    break;
                case 7:
                    step_seven(ref step);
                    return true;
            }

            return done;
        }

        private static void ShowCostMatrix()
        {
            Console.WriteLine("\n");
            Console.WriteLine("------------Step {0}-------------", step);
            for (int r = 0; r < nrow; r++)
            {
                Console.WriteLine();
                Console.Write("     ");
                for (int c = 0; c < ncol; c++)
                {
                    Console.Write(Convert.ToString(C[r, c]) + " ");
                }
            }
        }

        private static void ShowMaskMatrix()
        {
            Console.WriteLine();
            Console.Write("\n    ");
            for (int c = 0; c < ncol; c++)
                Console.Write(" " + Convert.ToString(ColCover[c]));
            for (int r = 0; r < nrow; r++)
            {
                Console.Write("\n  " + Convert.ToString(RowCover[r]) + "  ");
                for (int c = 0; c < ncol; c++)
                {
                    Console.Write(Convert.ToString(M[r, c]) + " ");
                }
            }
        }

    }

}
