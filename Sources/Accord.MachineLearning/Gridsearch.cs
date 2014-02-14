// Accord Machine Learning Library
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

namespace Accord.MachineLearning
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Accord.Math;

    /// <summary>
    ///   Delegate for grid search fitting functions.
    /// </summary>
    /// 
    /// <typeparam name="TModel">The type of the model to fit.</typeparam>
    /// 
    /// <param name="parameters">The collection of parameters to be used in the fitting process.</param>
    /// <param name="error">The error (or any other performance measure) returned by the model.</param>
    /// <returns>The model fitted to the data using the given parameters.</returns>
    /// 
    public delegate TModel GridSearchFittingFunction<TModel>(GridSearchParameterCollection parameters, out double error);

    /// <summary>
    ///   Grid search procedure for automatic parameter tuning.
    /// </summary>
    /// 
    /// <remarks>
    ///   Grid Search tries to find the best combination of parameters across
    ///   a range of possible values that produces the best fit model. If there
    ///   are two parameters, each with 10 possible values, Grid Search will try
    ///   an exhaustive evaluation of the model using every combination of points,
    ///   resulting in 100 model fits.
    /// </remarks>
    /// 
    /// <typeparam name="TModel">The type of the model to be tuned.</typeparam>
    /// 
    /// <example>
    ///   How to fit a Kernel Support Vector Machine using Grid Search.
    ///   <code>
    ///   // Example binary data
    ///   double[][] inputs =
    ///   {
    ///       new double[] { -1, -1 },
    ///       new double[] { -1,  1 },
    ///       new double[] {  1, -1 },
    ///       new double[] {  1,  1 }
    ///   };
    ///
    ///   int[] xor = // xor labels
    ///   {
    ///       -1, 1, 1, -1
    ///   };
    ///
    ///   // Declare the parameters and ranges to be searched
    ///   GridSearchRange[] ranges = 
    ///   {
    ///       new GridSearchRange("complexity", new double[] { 0.00000001, 5.20, 0.30, 0.50 } ),
    ///       new GridSearchRange("degree",     new double[] { 1, 10, 2, 3, 4, 5 } ),
    ///       new GridSearchRange("constant",   new double[] { 0, 1, 2 } )
    ///   };
    ///
    ///
    ///   // Instantiate a new Grid Search algorithm for Kernel Support Vector Machines
    ///   var gridsearch = new GridSearch&lt;KernelSupportVectorMachine>(ranges);
    ///
    ///   // Set the fitting function for the algorithm
    ///   gridsearch.Fitting = delegate(GridSearchParameterCollection parameters, out double error)
    ///   {
    ///       // The parameters to be tried will be passed as a function parameter.
    ///       int degree = (int)parameters["degree"].Value;
    ///       double constant = parameters["constant"].Value;
    ///       double complexity = parameters["complexity"].Value;
    ///
    ///       // Use the parameters to build the SVM model
    ///       Polynomial kernel = new Polynomial(degree, constant);
    ///       KernelSupportVectorMachine ksvm = new KernelSupportVectorMachine(kernel, 2);
    ///
    ///       // Create a new learning algorithm for SVMs
    ///       SequentialMinimalOptimization smo = new SequentialMinimalOptimization(ksvm, inputs, xor);
    ///       smo.Complexity = complexity;
    ///
    ///       // Measure the model performance to return as an out parameter
    ///       error = smo.Run();
    ///
    ///       return ksvm; // Return the current model
    ///   };
    ///
    ///
    ///   // Declare some out variables to pass to the grid search algorithm
    ///   GridSearchParameterCollection bestParameters; double minError;
    ///
    ///   // Compute the grid search to find the best Support Vector Machine
    ///   KernelSupportVectorMachine bestModel = gridsearch.Compute(out bestParameters, out minError);
    ///   </code>
    /// </example>
    /// 
    [Serializable]
    public class GridSearch<TModel> where TModel : class
    {
        private GridSearchRangeCollection ranges;
        private GridSearchFittingFunction<TModel> fitting;

        /// <summary>
        ///   Constructs a new Grid search algorithm.
        /// </summary>
        /// 
        /// <param name="parameterRanges">The range of parameters to search.</param>
        /// 
        public GridSearch(params GridSearchRange[] parameterRanges)
        {
            this.ranges = new GridSearchRangeCollection(parameterRanges);
        }

        /// <summary>
        ///   A function that fits a model using the given parameters.
        /// </summary>
        /// 
        public GridSearchFittingFunction<TModel> Fitting
        {
            get { return fitting; }
            set { fitting = value; }
        }

        /// <summary>
        ///   The range of parameters to consider during search.
        /// </summary>
        /// 
        public GridSearchRangeCollection ParameterRanges
        {
            get { return ranges; }
        }

        /// <summary>
        ///   Searches for the best combination of parameters that results in the most accurate model.
        /// </summary>
        /// 
        /// <param name="bestParameters">The best combination of parameters found by the grid search.</param>
        /// <param name="error">The minimum error of the best model found by the grid search.</param>
        /// <returns>The best model found during the grid search.</returns>
        /// 
        public TModel Compute(out GridSearchParameterCollection bestParameters, out double error)
        {
            var result = Compute();
            bestParameters = result.Parameter;
            error = result.Error;
            return result.Model;
        }

        /// <summary>
        ///   Searches for the best combination of parameters that results in the most accurate model.
        /// </summary>
        /// 
        /// <returns>The results found during the grid search.</returns>
        /// 
        public GridSearchResult<TModel> Compute()
        {

            // Get the total number of different parameters
            var values = new GridSearchParameter[ranges.Count][];
            for (int i = 0; i < values.Length; i++)
                values[i] = ranges[i].GetParameters();


            // Generate the Cartesian product between all parameters
            GridSearchParameter[][] grid = Matrix.CartesianProduct(values);


            // Initialize the search
            var parameters = new GridSearchParameterCollection[grid.Length];
            var models = new TModel[grid.Length];
            var errors = new double[grid.Length];
            var msgs = new string[grid.Length];

            // Search the grid for the optimal parameters
            Parallel.For(0, grid.Length, i =>
            {
                // Get the current parameters for the current point
                parameters[i] = new GridSearchParameterCollection(grid[i]);

                try
                {
                    // Try to fit a model using the parameters
                    models[i] = Fitting(parameters[i], out errors[i]);
                }
                catch (ConvergenceException ex)
                {
                    errors[i] = Double.PositiveInfinity;
                    msgs[i] = ex.Message;
                }
            });


            // Select the minimum error
            int best; errors.Min(out best);

            // Return the best model found.
            return new GridSearchResult<TModel>(grid.Length,
                parameters, models, errors, best);
        }

    }

    /// <summary>
    ///   Contains results from the grid-search procedure.
    /// </summary>
    /// 
    /// <typeparam name="TModel">The type of the model to be tuned.</typeparam>
    /// 
    public class GridSearchResult<TModel> where TModel : class
    {

        private GridSearchParameterCollection[] parameters;
        private TModel[] models;
        private double[] errors;
        private int gridSize;
        private int bestIndex;

        /// <summary>
        ///   Gets all combination of parameters tried.
        /// </summary>
        /// 
        public GridSearchParameterCollection[] Parameters
        {
            get { return parameters; }
        }

        /// <summary>
        ///   Gets all models created during the search.
        /// </summary>
        /// 
        public TModel[] Models
        {
            get { return models; }
        }

        /// <summary>
        ///   Gets the error for each of the created models.
        /// </summary>
        /// 
        public double[] Errors
        {
            get { return errors; }
        }

        /// <summary>
        ///   Gets the index of the best found model
        ///   in the <see cref="Models"/> collection.
        /// </summary>
        /// 
        public int Index
        {
            get { return bestIndex; }
        }

        /// <summary>
        ///   Gets the best model found.
        /// </summary>
        /// 
        public TModel Model
        {
            get { return models[bestIndex]; }
        }

        /// <summary>
        ///   Gets the best parameter combination found.
        /// </summary>
        /// 
        public GridSearchParameterCollection Parameter
        {
            get { return parameters[bestIndex]; }
        }

        /// <summary>
        ///   Gets the minimum error found.
        /// </summary>
        /// 
        public double Error
        {
            get { return errors[bestIndex]; }
        }


        /// <summary>
        ///   Gets the size of the grid used in the grid-search.
        /// </summary>
        /// 
        public int Count
        {
            get { return gridSize; }
        }



        /// <summary>
        ///   Initializes a new instance of the <see cref="GridSearchResult&lt;TModel&gt;"/> class.
        /// </summary>
        /// 
        public GridSearchResult(int size)
        {
            gridSize = size;
            parameters = new GridSearchParameterCollection[size];
            models = new TModel[size];
            errors = new double[size];
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="GridSearchResult&lt;TModel&gt;"/> class.
        /// </summary>
        /// 
        public GridSearchResult(int size, GridSearchParameterCollection[] parameters,
            TModel[] models, double[] errors, int index)
        {
            this.gridSize = size;

            if (parameters.Length != size || models.Length != size || errors.Length != size)
                throw new DimensionMismatchException("size", "All array parameters must have the same length.");

            if (0 > index || index >= size)
                throw new ArgumentOutOfRangeException("index", "Index must be higher than 0 and less than size.");

            this.parameters = parameters;
            this.models = models;
            this.errors = errors;
            this.bestIndex = index;
        }

    }
}
