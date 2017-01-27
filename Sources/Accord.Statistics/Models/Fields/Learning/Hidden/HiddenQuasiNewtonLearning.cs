// Accord Statistics Library
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

#pragma warning disable 612, 618

namespace Accord.Statistics.Models.Fields.Learning
{
    using System;
    using Accord.Math.Optimization;
    using Accord.MachineLearning;
    using System.Threading;

    /// <summary>
    ///   Quasi-Newton (L-BFGS) learning algorithm for <see cref="HiddenConditionalRandomField{T}">
    ///   Hidden Conditional Hidden Fields</see>.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the observations.</typeparam>
    /// 
    /// <example>
    /// <para>
    ///   For an example on how to learn Hidden Conditional Random Fields, please see the
    ///   <see cref="HiddenResilientGradientLearning{T}">Hidden Resilient Gradient Learning</see>
    ///   page. All learning algorithms can be utilized in a similar manner.</para>
    /// </example>
    /// 
    /// <seealso cref="HiddenResilientGradientLearning{T}"/>
    /// 
    public class HiddenQuasiNewtonLearning<T> :
        ISupervisedLearning<HiddenConditionalRandomField<T>, T[], int>,
        IHiddenConditionalRandomFieldLearning<T>,
        IDisposable
    {
        [NonSerialized]
        CancellationToken token = new CancellationToken();

        private BoundedBroydenFletcherGoldfarbShanno lbfgs;
        private ForwardBackwardGradient<T> calculator;

        /// <summary>
        ///   Gets or sets the model being trained.
        /// </summary>
        /// 
        public HiddenConditionalRandomField<T> Model { get; set; }

        /// <summary>
        /// Gets or sets a cancellation token that can be used to
        /// stop the learning algorithm while it is running.
        /// </summary>
        public CancellationToken Token
        {
            get { return token; }
            set { token = value; }
        }


        /// <summary>
        ///   Gets or sets the amount of the parameter weights
        ///   which should be included in the objective function.
        ///   Default is 0 (do not include regularization).
        /// </summary>
        /// 
        public double Regularization
        {
            get { return calculator.Regularization; }
            set { calculator.Regularization = value; }
        }

        /// <summary>
        ///   Constructs a new L-BFGS learning algorithm.
        /// </summary>
        /// 
        public HiddenQuasiNewtonLearning(HiddenConditionalRandomField<T> model)
        {
            Model = model;

            calculator = new ForwardBackwardGradient<T>(model);

            lbfgs = new BoundedBroydenFletcherGoldfarbShanno(model.Function.Weights.Length);
            lbfgs.FunctionTolerance = 1e-3;
            lbfgs.Function = calculator.Objective;
            lbfgs.Gradient = calculator.Gradient;

            for (int i = 0; i < lbfgs.UpperBounds.Length; i++)
            {
                lbfgs.UpperBounds[i] = 1e10;
                lbfgs.LowerBounds[i] = -1e100;
            }
        }

        /// <summary>
        ///   Runs the learning algorithm with the specified input
        ///   training observations and corresponding output labels.
        /// </summary>
        /// 
        /// <param name="observations">The training observations.</param>
        /// <param name="outputs">The observation's labels.</param>
        /// 
        [Obsolete("Please use Learn(x, y) instead.")]
        public double Run(T[][] observations, int[] outputs)
        {
            return run(observations, outputs);
        }

        private double run(T[][] observations, int[] outputs)
        {
            calculator.Inputs = observations;
            calculator.Outputs = outputs;

            lbfgs.Token = Token;
            lbfgs.Minimize(Model.Function.Weights);

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

        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weights">The weight of importance for each input-output pair.</param>
        /// <returns>
        /// A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.
        /// </returns>
        public HiddenConditionalRandomField<T> Learn(T[][] x, int[] y, double[] weights = null)
        {
            run(x, y);
            return Model;
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
