// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
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
    using System.Collections.Generic;
    using AForge;
    using AForge.Math.Random;


    public static partial class Matrix
    {

        #region Generic matrices
        /// <summary>
        ///   Returns a matrix with all elements set to a given value.
        /// </summary>
        /// 
        public static T[,] Create<T>(int rows, int cols, T value)
        {
            if (rows < 0)
            {
                throw new ArgumentOutOfRangeException("rows", rows,
                    "Number of rows must be a positive integer.");
            }

            if (cols < 0)
            {
                throw new ArgumentOutOfRangeException("cols", cols,
                    "Number of columns must be a positive integer.");
            }

            T[,] matrix = new T[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    matrix[i, j] = value;

            return matrix;
        }

        /// <summary>
        ///   Returns a matrix with all elements set to a given value.
        /// </summary>
        /// 
        public static T[,] Create<T>(int size, T value)
        {
            if (size < 0)
            {
                throw new ArgumentOutOfRangeException("size", size,
                "Square matrix's size must be a positive integer.");
            }

            return Create(size, size, value);
        }

        /// <summary>
        ///   Returns a new multidimensional matrix.
        /// </summary>
        /// 
        public static T[,] Create<T>(int rows, int cols)
        {
            return Create(rows, cols, default(T));
        }

        /// <summary>
        ///   Returns a new multidimensional matrix.
        /// </summary>
        /// 
        public static T[,] Create<T>(int size)
        {
            return Create(size, default(T));
        }

        /// <summary>
        ///   Returns a matrix with all elements set to a given value.
        /// </summary>
        /// 
        public static T[][] Jagged<T>(int rows, int cols, T value)
        {
            if (rows < 0)
            {
                throw new ArgumentOutOfRangeException("rows", rows,
                    "Number of rows must be a positive integer.");
            }

            if (cols < 0)
            {
                throw new ArgumentOutOfRangeException("cols", cols,
                    "Number of columns must be a positive integer.");
            }

            T[][] matrix = new T[rows][];

            for (int i = 0; i < rows; i++)
            {
                var row = matrix[i] = new T[cols];
                for (int j = 0; j < row.Length; j++)
                    row[j] = value;
            }

            return matrix;
        }

        /// <summary>
        ///   Returns a matrix with all elements set to a given value.
        /// </summary>
        /// 
        public static T[,] Jagged<T>(int size, T value)
        {
            if (size < 0)
            {
                throw new ArgumentOutOfRangeException("size", size,
                "Square matrix's size must be a positive integer.");
            }

            return Create(size, size, value);
        }

        /// <summary>
        ///   Returns a new jagged matrix.
        /// </summary>
        /// 
        public static T[][] Jagged<T>(int rows, int cols)
        {
            return Jagged(rows, cols, default(T));
        }

        /// <summary>
        ///   Returns a new jagged matrix.
        /// </summary>
        /// 
        public static T[,] Jagged<T>(int size)
        {
            return Create(size, default(T));
        }
        #endregion


        #region Diagonal matrices
        /// <summary>
        ///   Returns a square diagonal matrix of the given size.
        /// </summary>
        /// 
        public static T[,] Diagonal<T>(int size, T value)
        {
            if (size < 0)
            {
                throw new ArgumentOutOfRangeException("size", size,
                "Square matrix's size must be a positive integer.");
            }

            T[,] matrix = new T[size, size];

            for (int i = 0; i < size; i++)
                matrix[i, i] = value;

            return matrix;
        }

        /// <summary>
        ///   Returns a matrix of the given size with value on its diagonal.
        /// </summary>
        /// 
        public static T[,] Diagonal<T>(int rows, int cols, T value)
        {
            if (rows < 0)
            {
                throw new ArgumentOutOfRangeException("rows", rows,
                    "Number of rows must be a positive integer.");
            }

            if (cols < 0)
            {
                throw new ArgumentOutOfRangeException("cols", cols,
                    "Number of columns must be a positive integer.");
            }

            T[,] matrix = new T[rows, cols];

            int min = Math.Min(rows, cols);

            for (int i = 0; i < min; i++)
                matrix[i, i] = value;

            return matrix;
        }

        /// <summary>
        ///   Return a square matrix with a vector of values on its diagonal.
        /// </summary>
        /// 
        public static T[,] Diagonal<T>(T[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            T[,] matrix = new T[values.Length, values.Length];

            for (int i = 0; i < values.Length; i++)
                matrix[i, i] = values[i];

            return matrix;
        }

        /// <summary>
        ///   Return a jagged matrix with a vector of values on its diagonal.
        /// </summary>
        /// 
        // TODO: Mark as obsolete
        public static T[][] JaggedDiagonal<T>(T[] values)
        {
            return Accord.Math.Jagged.Diagonal(values);
        }

        /// <summary>
        ///   Returns a square diagonal matrix of the given size.
        /// </summary>
        /// 
        // TODO: Mark as obsolete
        public static T[][] JaggedDiagonal<T>(int size, T value)
        {
            return Accord.Math.Jagged.Diagonal(size, value);
        }

        /// <summary>
        ///   Return a square matrix with a vector of values on its diagonal.
        /// </summary>
        /// 
        public static T[,] Diagonal<T>(int size, T[] values)
        {
            return Diagonal(size, size, values);
        }

        /// <summary>
        ///   Returns a matrix with a vector of values on its diagonal.
        /// </summary>
        /// 
        public static T[,] Diagonal<T>(int rows, int cols, T[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            if (rows < 0)
            {
                throw new ArgumentOutOfRangeException("rows", rows,
                    "Number of rows must be a positive integer.");
            }

            if (cols < 0)
            {
                throw new ArgumentOutOfRangeException("cols", cols,
                    "Number of columns must be a positive integer.");
            }

            T[,] matrix = new T[rows, cols];

            for (int i = 0; i < values.Length; i++)
                matrix[i, i] = values[i];

            return matrix;
        }
        #endregion

        #region Special matrices
        /// <summary>
        ///   Returns the Identity matrix of the given size.
        /// </summary>
        /// 
        public static double[,] Identity(int size)
        {
            return Diagonal(size, 1.0);
        }

        /// <summary>
        ///   Returns the Identity matrix of the given size.
        /// </summary>
        /// 
        public static double[][] JaggedIdentity(int size)
        {
            return JaggedDiagonal(size, 1.0);
        }

        /// <summary>
        ///   Creates a jagged magic square matrix.
        /// </summary>
        /// 
        public static double[][] JaggedMagic(int size)
        {
            return Magic(size).ToArray();
        }

        /// <summary>
        ///   Creates a magic square matrix.
        /// </summary>
        /// 
        public static double[,] Magic(int size)
        {
            if (size < 3) throw new ArgumentOutOfRangeException("size", size,
                "The square size must be greater or equal to 3.");

            double[,] matrix = new double[size, size];


            // First algorithm: Odd order
            if ((size % 2) == 1)
            {
                int a = (size + 1) / 2;
                int b = (size + 1);

                for (int j = 0; j < size; j++)
                    for (int i = 0; i < size; i++)
                        matrix[i, j] = size * ((i + j + a) % size) + ((i + 2 * j + b) % size) + 1;
            }

            // Second algorithm: Even order (double)
            else if ((size % 4) == 0)
            {
                for (int j = 0; j < size; j++)
                    for (int i = 0; i < size; i++)
                        if (((i + 1) / 2) % 2 == ((j + 1) / 2) % 2)
                            matrix[i, j] = size * size - size * i - j;
                        else
                            matrix[i, j] = size * i + j + 1;
            }

            // Third algorithm: Even order (single)
            else
            {
                int n = size / 2;
                int p = (size - 2) / 4;
                double t;

                double[,] block = Matrix.Magic(n);

                for (int j = 0; j < n; j++)
                {
                    for (int i = 0; i < n; i++)
                    {
                        double e = block[i, j];
                        matrix[i, j] = e;
                        matrix[i, j + n] = e + 2 * n * n;
                        matrix[i + n, j] = e + 3 * n * n;
                        matrix[i + n, j + n] = e + n * n;
                    }
                }

                for (int i = 0; i < n; i++)
                {
                    // Swap M[i,j] and M[i+n,j]
                    for (int j = 0; j < p; j++)
                    {
                        t = matrix[i, j];
                        matrix[i, j] = matrix[i + n, j];
                        matrix[i + n, j] = t;
                    }
                    for (int j = size - p + 1; j < size; j++)
                    {
                        t = matrix[i, j];
                        matrix[i, j] = matrix[i + n, j];
                        matrix[i + n, j] = t;
                    }
                }

                // Continue swapping in the boundary
                t = matrix[p, 0];
                matrix[p, 0] = matrix[p + n, 0];
                matrix[p + n, 0] = t;

                t = matrix[p, p];
                matrix[p, p] = matrix[p + n, p];
                matrix[p + n, p] = t;
            }

            return matrix; // return the magic square.
        }

        /// <summary>
        ///   Creates a centering matrix of size <c>N x N</c> in the
        ///   form <c>(I - 1N)</c> where <c>1N</c> is a matrix with 
        ///   all elements equal to <c>1 / N</c>.
        /// </summary>
        /// 
        public static double[,] Centering(int size)
        {
            if (size < 0)
            {
                throw new ArgumentOutOfRangeException("size", size,
                "The size of the centering matrix must be a positive integer.");
            }

            double[,] C = Matrix.Create(size, -1.0 / size);

            for (int i = 0; i < size; i++)
                C[i, i] = 1.0 - 1.0 / size;

            return C;
        }
        #endregion

        #region Random matrices
        /// <summary>
        ///   Creates a rows-by-cols matrix with uniformly distributed random data.
        /// </summary>
        public static double[,] Random(int rows, int cols)
        {
            if (rows < 0)
                throw new ArgumentOutOfRangeException("rows", rows, "Number of rows must be a positive integer.");

            if (cols < 0)
                throw new ArgumentOutOfRangeException("cols", cols, "Number of columns must be a positive integer.");

            return Random(rows, cols, 0, 1);
        }

        /// <summary>
        ///   Creates a rows-by-cols matrix with uniformly distributed random data.
        /// </summary>
        /// 
        public static double[,] Random(int size, bool symmetric, double minValue, double maxValue)
        {
            if (size < 0)
                throw new ArgumentOutOfRangeException("size", size, "Size must be a positive integer.");

            double[,] matrix = new double[size, size];

            if (!symmetric)
            {
                for (int i = 0; i < size; i++)
                    for (int j = 0; j < size; j++)
                        matrix[i, j] = Accord.Math.Tools.Random.NextDouble() * (maxValue - minValue) + minValue;
            }
            else
            {
                for (int i = 0; i < size; i++)
                {
                    for (int j = i; j < size; j++)
                    {
                        matrix[i, j] = Accord.Math.Tools.Random.NextDouble() * (maxValue - minValue) + minValue;
                        matrix[j, i] = matrix[i, j];
                    }
                }
            }

            return matrix;
        }

        /// <summary>
        ///   Creates a rows-by-cols matrix with uniformly distributed random data.
        /// </summary>
        /// 
        public static double[,] Random(int rows, int cols, double minValue, double maxValue)
        {
            if (rows < 0)
                throw new ArgumentOutOfRangeException("rows", rows, "Number of rows must be a positive integer.");

            if (cols < 0)
                throw new ArgumentOutOfRangeException("cols", cols, "Number of columns must be a positive integer.");

            double[,] matrix = new double[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    matrix[i, j] = Accord.Math.Tools.Random.NextDouble() * (maxValue - minValue) + minValue;

            return matrix;
        }

        /// <summary>
        ///   Creates a rows-by-cols matrix random data drawn from a given distribution.
        /// </summary>
        /// 
        public static double[,] Random(int rows, int cols, IRandomNumberGenerator generator)
        {
            if (generator == null)
                throw new ArgumentNullException("generator");

            if (rows < 0)
                throw new ArgumentOutOfRangeException("rows", rows, "Number of rows must be a positive integer.");

            if (cols < 0)
                throw new ArgumentOutOfRangeException("cols", cols, "Number of columns must be a positive integer.");

            double[,] matrix = new double[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    matrix[i, j] = generator.Next();

            return matrix;
        }

        /// <summary>
        ///   Creates a vector with uniformly distributed random data.
        /// </summary>
        /// 
        // TODO: Mark as obsolete
        public static float[] Random(int size, float minValue, float maxValue)
        {
            return Accord.Math.Vector.Random(size, minValue, maxValue);
        }

        /// <summary>
        ///   Creates a vector with uniformly distributed random data.
        /// </summary>
        /// 
        // TODO: Mark as obsolete
        public static double[] Random(int size, double minValue, double maxValue)
        {
            return Accord.Math.Vector.Random(size, minValue, maxValue);
        }

        /// <summary>
        ///   Creates a vector with random data drawn from a given distribution.
        /// </summary>
        /// 
        // TODO: Mark as obsolete
        public static double[] Random(int size, IRandomNumberGenerator generator)
        {
            if (generator == null)
                throw new ArgumentNullException("generator");

            if (size < 0)
                throw new ArgumentOutOfRangeException("size", size, "Size must be a positive integer.");

            double[] vector = new double[size];
            for (int i = 0; i < size; i++)
                vector[i] = generator.Next();

            return vector;
        }
        #endregion


        #region Vector creation
        /// <summary>
        ///   Creates a matrix with a single row vector.
        /// </summary>
        /// 
        public static T[,] RowVector<T>(params T[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            T[,] matrix = new T[1, values.Length];
            for (int i = 0; i < values.Length; i++)
                matrix[0, i] = values[i];

            return matrix;
        }

        /// <summary>
        ///   Creates a matrix with a single column vector.
        /// </summary>
        /// 
        public static T[,] ColumnVector<T>(params T[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            T[,] matrix = new T[values.Length, 1];
            for (int i = 0; i < values.Length; i++)
                matrix[i, 0] = values[i];

            return matrix;
        }

        /// <summary>
        ///   Creates a vector with the given dimension and starting values.
        /// </summary>
        /// 
        // TODO: Mark as obsolete
        public static T[] Vector<T>(int n, T[] values)
        {
            return Accord.Math.Vector.Create(n, values);
        }

        /// <summary>
        ///   Creates a vector with the given dimension and starting values.
        /// </summary>
        /// 
        // TODO: Mark as obsolete
        public static T[] Vector<T>(int n, T value)
        {
            return Accord.Math.Vector.Create(n, value);

        }

        /// <summary>
        ///   Creates a vector with the given dimension and starting values.
        /// </summary>
        /// 
        public static double[] Vector(double a, double b, double increment = 1)
        {
            return Accord.Math.Vector.Interval(a, b, (double)increment);
        }

        /// <summary>
        ///   Creates a vector with the given dimension and starting values.
        /// </summary>
        /// 
        // TODO: Mark as obsolete
        public static int[] Vector(int a, int b, int increment = 1)
        {
            return Accord.Math.Vector.Interval(a, b, (double)increment);
        }

        /// <summary>
        ///   Creates a vector with the given dimension and starting values.
        /// </summary>
        /// 
        // TODO: Mark as obsolete
        public static double[] Vector(double a, double b, int points)
        {
            return Accord.Math.Vector.Interval(a, b, points);
        }
        #endregion

        #region Special vectors
        /// <summary>
        ///   Creates a index vector.
        /// </summary>
        /// 
        // TODO: Mark as obsolete
        public static int[] Indices(int from, int to)
        {
            return Accord.Math.Indices.Range(from, to);
        }

        /// <summary>
        ///   Creates a index vector.
        /// </summary>
        /// 
        // TODO: Mark as obsolete
        public static int[] Indices(int to)
        {
            return Accord.Math.Indices.Until(to);
        }

        /// <summary>
        ///   Gets the dimensions of an array.
        /// </summary>
        /// 
        public static int[] GetDimensions(this Array array)
        {
            int[] vector = new int[array.Rank];

            for (int i = 0; i < vector.Length; i++)
                vector[i] = array.GetUpperBound(i);

            return vector;
        }

        /// <summary>
        ///   Creates an interval vector.
        /// </summary>
        /// 
        public static int[] Interval(int from, int to)
        {
            return Accord.Math.Vector.Interval(from, to);
        }

        /// <summary>
        ///   Creates an interval vector.
        /// </summary>
        /// 
        public static double[] Interval(DoubleRange range, double stepSize)
        {
            return Interval(range.Min, range.Max, stepSize);
        }

        /// <summary>
        ///   Creates an interval vector.
        /// </summary>
        /// 
        public static double[] Interval(double from, double to, double stepSize)
        {
            return Accord.Math.Vector.Interval(from, to, stepSize);
        }

        /// <summary>
        ///   Creates an interval vector.
        /// </summary>
        /// 
        public static float[] Interval(float from, float to, double stepSize)
        {
            return Accord.Math.Vector.Interval(from, to, (float)stepSize);
        }

        /// <summary>
        ///   Creates an interval vector.
        /// </summary>
        /// 
        public static double[] Interval(DoubleRange range, int steps)
        {
            return Accord.Math.Vector.Interval(range, steps);
        }

        /// <summary>
        ///   Creates an interval vector.
        /// </summary>
        /// 
        public static double[] Interval(double from, double to, int steps)
        {
            return Accord.Math.Vector.Interval(from, to, steps);
        }

        /// <summary>
        ///   Creates a bi-dimensional mesh matrix.
        /// </summary>
        /// 
        /// <example>
        /// <code>
        /// // The Mesh method can be used to generate all
        /// // possible (x,y) pairs between two ranges. 
        /// 
        /// // We can create a grid as
        /// double[][] grid = Matrix.Mesh
        /// (
        ///     rowMin: 0, rowMax: 1, rowStepSize: 0.3,
        ///     colMin: 0, colMax: 1, colStepSize: 0.1
        /// );
        /// 
        /// // Now we can plot the points on-screen
        /// ScatterplotBox.Show("Grid (step size)", grid).Hold();
        /// </code>
        /// 
        /// <para>
        ///   The resulting image is shown below. </para>
        ///   <img src="..\images\grid-step-size.png" /> 
        /// </example>
        /// 
        public static double[][] Mesh(
            double rowMin, double rowMax, double rowStepSize,
            double colMin, double colMax, double colStepSize)
        {
            double[][] mesh = Matrix.CartesianProduct(
                Matrix.Interval(rowMin, rowMax, rowStepSize),
                Matrix.Interval(colMin, colMax, colStepSize));

            return mesh;
        }

        /// <summary>
        ///   Creates a bi-dimensional mesh matrix.
        /// </summary>
        /// 
        public static int[][] Mesh(
            int rowMin, int rowMax,
            int colMin, int colMax)
        {
            int[][] mesh = Matrix.CartesianProduct(
                Matrix.Interval(rowMin, rowMax),
                Matrix.Interval(colMin, colMax));

            return mesh;
        }

        /// <summary>
        ///   Creates a bi-dimensional mesh matrix.
        /// </summary>
        /// 
        /// <example>
        /// <code>
        /// // The Mesh method can be used to generate all
        /// // possible (x,y) pairs between two ranges. 
        /// 
        /// // We can create a grid as
        /// double[][] grid = Matrix.Mesh
        /// (
        ///     rowMin: 0, rowMax: 1, rowSteps: 10,
        ///     colMin: 0, colMax: 1, colSteps: 5
        /// );
        ///
        /// // Now we can plot the points on-screen
        /// ScatterplotBox.Show("Grid (fixed steps)", grid).Hold();
        /// </code>
        /// 
        /// <para>
        ///   The resulting image is shown below. </para>
        ///   <img src="..\images\grid-fixed-steps.png" /> 
        /// </example>
        /// 
        public static double[][] Mesh(
            double rowMin, double rowMax, int rowSteps,
            double colMin, double colMax, int colSteps)
        {
            double[][] mesh = Matrix.CartesianProduct(
                Matrix.Interval(rowMin, rowMax, rowSteps),
                Matrix.Interval(colMin, colMax, colSteps));

            return mesh;
        }

        /// <summary>
        ///   Creates a bi-dimensional mesh matrix.
        /// </summary>
        /// 
        /// <example>
        /// <code>
        /// // The Mesh method can be used to generate all
        /// // possible (x,y) pairs between two ranges. 
        /// 
        /// // We can create a grid as
        /// double[][] grid = Matrix.Mesh
        /// (
        ///     rowRange: new DoubleRange(0, 1), rowStepSize: 0.3,
        ///     colRange: new DoubleRange(0, 1), colStepSize: 0.1
        /// );
        /// 
        /// // Now we can plot the points on-screen
        /// ScatterplotBox.Show("Grid (step size)", grid).Hold();
        /// </code>
        /// 
        /// <para>
        ///   The resulting image is shown below. </para>
        ///   <img src="..\images\grid-step-size.png" /> 
        /// </example>
        /// 
        public static double[][] Mesh(
            DoubleRange rowRange, DoubleRange colRange,
            double rowStepSize, double colStepSize)
        {
            double[][] mesh = Matrix.CartesianProduct(
                Matrix.Interval(rowRange, rowStepSize),
                Matrix.Interval(colRange, colStepSize));

            return mesh;
        }

        /// <summary>
        ///   Creates a bi-dimensional mesh matrix.
        /// </summary>
        /// 
        /// <param name="x">The values to be replicated vertically.</param>
        /// <param name="y">The values to be replicated horizontally.</param>
        /// 
        /// <example>
        /// <code>
        /// // The Mesh method generates all possible (x,y) pairs
        /// // between two vector of points. For example, let's
        /// // suppose we have the values:
        /// //
        /// double[] a = { 0, 1 };
        /// double[] b = { 0, 1 };
        /// 
        /// // We can create a grid as
        /// double[][] grid = a.Mesh(b);
        /// 
        /// // the result will be:
        /// double[][] expected =
        /// {
        ///     new double[] { 0, 0 },
        ///     new double[] { 0, 1 },
        ///     new double[] { 1, 0 },
        ///     new double[] { 1, 1 },
        /// };
        /// </code>
        /// </example>
        /// 
        public static T[][] Mesh<T>(this T[] x, T[] y)
        {
            return Matrix.CartesianProduct(x, y);
        }

        /// <summary>
        ///   Generates a 2-D mesh grid from two vectors <c>a</c> and <c>b</c>,
        ///   generating two matrices <c>len(a)</c> x <c>len(b)</c> with all
        ///   all possible combinations of values between the two vectors. This
        ///   method is analogous to MATLAB/Octave's <c>meshgrid</c> function.
        /// </summary>
        ///
        /// <returns>A tuple containing two matrices: the first containing values
        /// for the x-coordinates and the second for the y-coordinates.</returns>
        /// 
        /// <example>
        /// // The MeshGrid method generates two matrices that can be
        /// // used to generate all possible (x,y) pairs between two
        /// // vector of points. For example, let's suppose we have
        /// // the values:
        /// //
        /// double[] a = { 1, 2, 3 };
        /// double[] b = { 4, 5, 6 };
        /// 
        /// // We can create a grid
        /// var grid = a.MeshGrid(b);
        /// 
        /// // get the x-axis values     //        | 1   1   1 |
        /// double[,] x = grid.Item1;    //  x =   | 2   2   2 |
        ///                              //        | 3   3   3 |
        /// 
        /// // get the y-axis values     //        | 4   5   6 |
        /// double[,] y = grid.Item2;    //  y =   | 4   5   6 |
        ///                              //        | 4   5   6 |
        /// 
        /// // we can either use those matrices separately (such as for plotting 
        /// // purposes) or we can also generate a grid of all the (x,y) pairs as
        /// //
        /// double[,][] xy = x.ApplyWithIndex((v, i, j) => new[] { x[i, j], y[i, j] });
        ///
        /// // The result will be
        /// // 
        /// //         |  (1, 4)   (1, 5)   (1, 6)  |
        /// //  xy  =  |  (2, 4)   (2, 5)   (2, 6)  |
        /// //         |  (3, 4)   (3, 5)   (3, 6)  |
        /// </example>
        ///
        public static Tuple<T[,], T[,]> MeshGrid<T>(this T[] x, T[] y)
        {
            var X = new T[x.Length, y.Length];
            var Y = new T[x.Length, y.Length];
            for (int i = 0; i < x.Length; i++)
            {
                for (int j = 0; j < y.Length; j++)
                {
                    X[i, j] = x[i];
                    Y[i, j] = y[j];
                }
            }

            return Tuple.Create(X, Y);
        }
        #endregion


        #region Combine
        /// <summary>
        ///   Combines two vectors horizontally.
        /// </summary>
        /// 
        public static T[] Concatenate<T>(this T[] a, T[] b)
        {
            T[] r = new T[a.Length + b.Length];
            for (int i = 0; i < a.Length; i++)
                r[i] = a[i];
            for (int i = 0; i < b.Length; i++)
                r[i + a.Length] = b[i];

            return r;
        }

        /// <summary>
        ///   Combines a vector and a element horizontally.
        /// </summary>
        /// 
        public static T[] Concatenate<T>(this T[] vector, T element)
        {
            T[] r = new T[vector.Length + 1];
            for (int i = 0; i < vector.Length; i++)
                r[i] = vector[i];

            r[vector.Length] = element;

            return r;
        }

        /// <summary>
        ///   Combines a vector and a element horizontally.
        /// </summary>
        /// 
        public static T[] Concatenate<T>(this T element, T[] vector)
        {
            T[] r = new T[vector.Length + 1];

            r[0] = element;

            for (int i = 0; i < vector.Length; i++)
                r[i + 1] = vector[i];

            return r;
        }

        /// <summary>
        ///   Combines a matrix and a vector horizontally.
        /// </summary>
        /// 
        public static T[,] Concatenate<T>(this T[,] matrix, T[] vector)
        {
            return matrix.InsertColumn(vector);
        }

        /// <summary>
        ///   Combines two matrices horizontally.
        /// </summary>
        /// 
        public static T[,] Concatenate<T>(this T[,] a, T[,] b)
        {
            return Concatenate(new[] { a, b });
        }

        /// <summary>
        ///   Combines two matrices horizontally.
        /// </summary>
        /// 
        public static T[][] Concatenate<T>(this T[][] a, T[][] b)
        {
            return Concatenate(new[] { a, b });
        }

        /// <summary>
        ///   Combines a matrix and a vector horizontally.
        /// </summary>
        /// 
        public static T[,] Concatenate<T>(params T[][,] matrices)
        {
            int rows = 0;
            int cols = 0;

            for (int i = 0; i < matrices.Length; i++)
            {
                cols += matrices[i].GetLength(1);
                if (matrices[i].GetLength(0) > rows)
                    rows = matrices[i].GetLength(0);
            }

            T[,] r = new T[rows, cols];


            int c = 0;
            for (int k = 0; k < matrices.Length; k++)
            {
                int currentRows = matrices[k].GetLength(0);
                int currentCols = matrices[k].GetLength(1);

                for (int j = 0; j < currentCols; j++)
                {
                    for (int i = 0; i < currentRows; i++)
                    {
                        r[i, c] = matrices[k][i, j];
                    }
                    c++;
                }
            }

            return r;
        }

        /// <summary>
        ///   Combines a matrix and a vector horizontally.
        /// </summary>
        /// 
        public static T[][] Concatenate<T>(params T[][][] matrices)
        {
            int rows = 0;
            int cols = 0;

            for (int i = 0; i < matrices.Length; i++)
            {
                cols += matrices[i][0].Length;
                if (matrices[i].Length > rows)
                    rows = matrices[i].Length;
            }

            T[][] r = new T[rows][];
            for (int i = 0; i < r.Length; i++)
                r[i] = new T[cols];


            int c = 0;
            for (int k = 0; k < matrices.Length; k++)
            {
                int currentRows = matrices[k].Length;
                int currentCols = matrices[k][0].Length;

                for (int j = 0; j < currentCols; j++)
                {
                    for (int i = 0; i < currentRows; i++)
                    {
                        r[i][c] = matrices[k][i][j];
                    }
                    c++;
                }
            }

            return r;
        }

        /// <summary>
        ///   Combine vectors horizontally.
        /// </summary>
        /// 
        public static T[] Concatenate<T>(this T[][] vectors)
        {
            int size = 0;
            for (int i = 0; i < vectors.Length; i++)
                size += vectors[i].Length;

            T[] r = new T[size];

            int c = 0;
            for (int i = 0; i < vectors.Length; i++)
                for (int j = 0; j < vectors[i].Length; j++)
                    r[c++] = vectors[i][j];

            return r;
        }

        /// <summary>
        ///   Combines vectors vertically.
        /// </summary>
        /// 
        public static T[,] Stack<T>(this T[] a, T[] b)
        {
            return Stack(new[] { a, b });
        }

        /// <summary>
        ///   Combines vectors vertically.
        /// </summary>
        /// 
        public static T[][] Stack<T>(this T[][] a, T[][] b)
        {
            return Stack(new T[][][] { a, b });
        }

        /// <summary>
        ///   Combines vectors vertically.
        /// </summary>
        /// 
        public static T[,] Stack<T>(params T[][] vectors)
        {
            return vectors.ToMatrix();
        }

        /// <summary>
        ///   Combines vectors vertically.
        /// </summary>
        /// 
        public static T[,] Stack<T>(params T[] elements)
        {
            return elements.Transpose();
        }

        /// <summary>
        ///   Combines vectors vertically.
        /// </summary>
        /// 
        public static T[,] Stack<T>(this T[] vector, T element)
        {
            return vector.Concatenate(element).Transpose();
        }

        /// <summary>
        ///   Combines matrices vertically.
        /// </summary>
        /// 
        public static T[,] Stack<T>(params T[][,] matrices)
        {
            int rows = 0;
            int cols = 0;

            for (int i = 0; i < matrices.Length; i++)
            {
                rows += matrices[i].GetLength(0);
                if (matrices[i].GetLength(1) > cols)
                    cols = matrices[i].GetLength(1);
            }

            T[,] r = new T[rows, cols];

            int c = 0;
            for (int i = 0; i < matrices.Length; i++)
            {
                for (int j = 0; j < matrices[i].GetLength(0); j++)
                {
                    for (int k = 0; k < matrices[i].GetLength(1); k++)
                        r[c, k] = matrices[i][j, k];
                    c++;
                }
            }

            return r;
        }

        /// <summary>
        ///   Combines matrices vertically.
        /// </summary>
        /// 
        public static T[,] Stack<T>(this T[,] matrix, T[] vector)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            T[,] r = new T[rows + 1, cols];

            Array.Copy(matrix, r, matrix.Length);

            for (int i = 0; i < vector.Length; i++)
                r[rows, i] = vector[i];

            return r;
        }

        /// <summary>
        ///   Combines matrices vertically.
        /// </summary>
        public static T[][] Stack<T>(params T[][][] matrices)
        {
            int rows = 0;
            int cols = 0;

            for (int i = 0; i < matrices.Length; i++)
            {
                rows += matrices[i].Length;
                if (matrices[i].Length == 0)
                    continue;

                if (matrices[i][0].Length > cols)
                    cols = matrices[i][0].Length;
            }

            T[][] r = new T[rows][];
            for (int i = 0; i < rows; i++)
                r[i] = new T[cols];

            int c = 0;
            for (int i = 0; i < matrices.Length; i++)
            {
                for (int j = 0; j < matrices[i].Length; j++)
                {
                    for (int k = 0; k < matrices[i][j].Length; k++)
                        r[c][k] = matrices[i][j][k];
                    c++;
                }
            }

            return r;
        }
        #endregion

        #region Expand


        /// <summary>
        ///   Expands a data vector given in summary form.
        /// </summary>
        /// 
        /// <param name="vector">A base vector.</param>
        /// <param name="count">An array containing by how much each line should be replicated.</param>
        /// 
        public static T[] Expand<T>(T[] vector, int[] count)
        {
            var expansion = new List<T>();
            for (int i = 0; i < count.Length; i++)
                for (int j = 0; j < count[i]; j++)
                    expansion.Add(vector[i]);

            return expansion.ToArray();
        }

        /// <summary>
        ///   Expands a data matrix given in summary form.
        /// </summary>
        /// 
        /// <param name="matrix">A base matrix.</param>
        /// <param name="count">An array containing by how much each line should be replicated.</param>
        /// 
        public static T[,] Expand<T>(T[,] matrix, int[] count)
        {
            var expansion = new List<T[]>();
            for (int i = 0; i < count.Length; i++)
                for (int j = 0; j < count[i]; j++)
                    expansion.Add(matrix.GetRow(i));

            return expansion.ToArray().ToMatrix();
        }
        #endregion

        #region Split
        /// <summary>
        ///   Splits a given vector into a smaller vectors of the given size.
        ///   This operation can be reverted using <see cref="Merge"/>.
        /// </summary>
        /// 
        /// <param name="vector">The vector to be splitted.</param>
        /// <param name="size">The size of the resulting vectors.</param>
        /// 
        /// <returns>An array of vectors containing the subdivisions of the given vector.</returns>
        /// 
        public static T[][] Split<T>(this T[] vector, int size)
        {
            int n = vector.Length / size;
            T[][] r = new T[n][];
            for (int i = 0; i < n; i++)
            {
                T[] ri = r[i] = new T[size];
                for (int j = 0; j < size; j++)
                    ri[j] = vector[j * n + i];
            }
            return r;
        }

        /// <summary>
        ///   Merges a series of vectors into a single vector. This
        ///   operation can be reverted using <see cref="Split"/>.
        /// </summary>
        /// 
        /// <param name="vectors">The vectors to be merged.</param>
        /// <param name="size">The size of the inner vectors.</param>
        /// 
        /// <returns>A single array containing the given vectors.</returns>
        /// 
        public static T[] Merge<T>(this T[][] vectors, int size)
        {
            int n = vectors.Length * size;
            T[] r = new T[n * size];

            int c = 0;
            for (int i = 0; i < vectors.Length; i++)
                for (int j = 0; j < vectors[i].Length; j++, c++)
                    r[c] = vectors[i][j];

            return r;
        }
        #endregion

        /// <summary>
        ///   Pads a matrix by filling all of its sides with zeros.
        /// </summary>
        /// 
        /// <param name="matrix">The matrix whose contents will be padded.</param>
        /// <param name="all">How many rows and columns to add at each side of the matrix.</param>
        /// 
        /// <returns>The original matrix with an extra row of zeros at the selected places.</returns>
        /// 
        public static T[,] Pad<T>(this T[,] matrix, int all)
        {
            return Pad(matrix, all, all, all, all);
        }

        /// <summary>
        ///   Pads a matrix by filling all of its sides with zeros.
        /// </summary>
        /// 
        /// <param name="matrix">The matrix whose contents will be padded.</param>
        /// <param name="rightLeft">How many columns to add at the sides of the matrix.</param>
        /// <param name="topBottom">How many rows to add at the bottom and top of the matrix.</param>
        /// 
        /// <returns>The original matrix with an extra row of zeros at the selected places.</returns>
        /// 
        public static T[,] Pad<T>(this T[,] matrix, int topBottom, int rightLeft)
        {
            return Pad(matrix, topBottom, rightLeft, topBottom, rightLeft);
        }

        /// <summary>
        ///   Pads a matrix by filling all of its sides with zeros.
        /// </summary>
        /// 
        /// <param name="matrix">The matrix whose contents will be padded.</param>
        /// <param name="bottom">How many rows to add at the bottom.</param>
        /// <param name="top">How many rows to add at the top.</param>
        /// <param name="sides">How many columns to add at the sides.</param>
        /// 
        /// <returns>The original matrix with an extra row of zeros at the selected places.</returns>
        /// 
        public static T[,] Pad<T>(this T[,] matrix, int top, int sides, int bottom)
        {
            return Pad(matrix, top, sides, bottom, sides);
        }

        /// <summary>
        ///   Pads a matrix by filling all of its sides with zeros.
        /// </summary>
        /// 
        /// <param name="matrix">The matrix whose contents will be padded.</param>
        /// <param name="bottom">How many rows to add at the bottom.</param>
        /// <param name="top">How many rows to add at the top.</param>
        /// <param name="left">How many columns to add at the left side.</param>
        /// <param name="right">How many columns to add at the right side.</param>
        /// 
        /// <returns>The original matrix with an extra row of zeros at the selected places.</returns>
        /// 
        public static T[,] Pad<T>(this T[,] matrix, int top, int right, int bottom, int left)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            T[,] r = (T[,])Array.CreateInstance(typeof(T), rows + top + bottom, cols + left + right);

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i + top, j + left] = matrix[i, j];

            return r;
        }

    }
}
