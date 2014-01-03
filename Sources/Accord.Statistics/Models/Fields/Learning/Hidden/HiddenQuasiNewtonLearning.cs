// Accord Statistics Library
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

namespace Accord.Statistics.Models.Fields.Learning
{
    using System;
    using Accord.Math.Optimization;

    /// <summary>
    ///   Quasi-Newton (L-BFGS) learning algorithm for <see cref="HiddenConditionalRandomField{T}">
    ///   Hidden Conditional Hidden Fields</see>.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the observations.</typeparam>
    /// 
    /// <seealso cref="HiddenResilientGradientLearning{T}"/>
    /// 
    public class HiddenQuasiNewtonLearning<T> : IHiddenConditionalRandomFieldLearning<T>,
        IDisposable
    {

        private BroydenFletcherGoldfarbShanno lbfgs;
        private ForwardBackwardGradient<T> calculator;

        /// <summary>
        ///   Gets or sets the model being trained.
        /// </summary>
        /// 
        public HiddenConditionalRandomField<T> Model { get; set; }

        /// <summary>
        ///   Constructs a new L-BFGS learning algorithm.
        /// </summary>
        /// 
        public HiddenQuasiNewtonLearning(HiddenConditionalRandomField<T> model)
        {
            Model = model;

            calculator = new ForwardBackwardGradient<T>(model);

            lbfgs = new BroydenFletcherGoldfarbShanno(model.Function.Weights.Length);
            lbfgs.Tolerance = 1e-3;
            lbfgs.Function = calculator.Objective;
            lbfgs.Gradient = calculator.Gradient;
        }

        /// <summary>
        ///   Runs the learning algorithm with the specified input
        ///   training observations and corresponding output labels.
        /// </summary>
        /// 
        /// <param name="observations">The training observations.</param>
        /// <param name="outputs">The observation's labels.</param>
        /// 
        public double Run(T[][] observations, int[] outputs)
        {
            calculator.Inputs = observations;
            calculator.Outputs = outputs;

            try
            {
                lbfgs.Minimize(Model.Function.Weights);
            }
            catch (LineSearchFailedException)
            {
                // TODO: Restructure L-BFGS to avoid exceptions.
            }

            Model.Function.Weights = lbfgs.Solution;

            // Return negative log-likelihood as error function
            return -Model.LogLikelihood(observations, outputs);
        }

        /// <summary>
        ///   Online learning is not supported.
        /// </summary>
        ///   
        public double Run(T[] observations, int output)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///   Online learning is not supported.
        /// </summary>
        ///   
        [Obsolete("Use Run(T[][], int[]) instead.")]
        public double RunEpoch(T[][] observations, int[] output)
        {
            throw new NotSupportedException();
        }


                #region IDisposable Members

        /// <summary>
        ///   Performs application-defined tasks associated with freeing,
        ///   releasing, or resetting unmanaged resources.
        /// </summary>
        /// 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///   Releases unmanaged resources and performs other cleanup operations before
        ///   the <see cref="HiddenQuasiNewtonLearning{T}"/> is reclaimed by garbage
        ///   collection.
        /// </summary>
        /// 
        ~HiddenQuasiNewtonLearning()
        {
            Dispose(false);
        }

        /// <summary>
        ///   Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// 
        /// <param name="disposing"><c>true</c> to release both managed 
        /// and unmanaged resources; <c>false</c> to release only unmanaged
        /// resources.</param>
        /// 
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (calculator != null)
                {
                    calculator.Dispose();
                    calculator = null;
                }
            }
        }
      
        #endregion

    }
}
