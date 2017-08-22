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

namespace Accord.Statistics.Models.Fields.Learning
{
#pragma warning disable 612, 618

    using System;
    using System.ComponentModel;
    using Accord.Math.Optimization;
    using Accord.MachineLearning;
    using System.Threading;
    using Accord.Compat;

    /// <summary>
    ///   Conjugate Gradient learning algorithm for <see cref="HiddenConditionalRandomField{T}">
    ///   Hidden Conditional Hidden Fields</see>.
    /// </summary>
    /// 
    /// <example>
    /// <para>
    ///   For an example on how to learn Hidden Conditional Random Fields, please see the
    ///   <see cref="HiddenResilientGradientLearning{T}">Hidden Resilient Gradient Learning</see>
    ///   page. All learning algorithms can be utilized in a similar manner.</para>
    /// </example>
    /// 
    public class HiddenConjugateGradientLearning<T> : BaseHiddenGradientOptimizationLearning<T, ConjugateGradient>,
        ISupervisedLearning<HiddenConditionalRandomField<T>, T[], int>, IParallel,
        IHiddenConditionalRandomFieldLearning<T>, IConvergenceLearning, IDisposable
    {

        /// <summary>
        ///   Please use HasConverged instead.
        /// </summary>
        /// 
        [Obsolete("Please use HasConverged instead.")]
        public bool Converged { get { return HasConverged; } }

        int IConvergenceLearning.Iterations
        {
            get
            {
                if (Optimizer == null)
                    return 0;
                return Optimizer.Iterations;
            }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the current iteration number.
        /// </summary>
        /// <value>The current iteration.</value>
        public int CurrentIteration
        {
            get
            {
                if (Optimizer != null)
                    return Optimizer.Iterations;
                return 0;
            }
        }

        /// <summary>
        ///   Please use MaxIterations instead.
        /// </summary>
        /// 
        [Obsolete("Please use MaxIterations instead.")]
        public int Iterations
        {
            get { return MaxIterations; }
            set { MaxIterations = value; }
        }

        /// <summary>
        ///   Occurs when the current learning progress has changed.
        /// </summary>
        /// 
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;



        /// <summary>
        ///   Constructs a new Conjugate Gradient learning algorithm.
        /// </summary>
        /// 
        public HiddenConjugateGradientLearning(HiddenConditionalRandomField<T> model)
        {
            Model = model;
        }

        /// <summary>
        /// Inheritors of this class should create the optimization algorithm in this
        /// method, using the current <see cref="P:Accord.Statistics.Models.Fields.Learning.BaseHiddenGradientOptimizationLearning`2.MaxIterations" /> and <see cref="P:Accord.Statistics.Models.Fields.Learning.BaseHiddenGradientOptimizationLearning`2.Tolerance" />
        /// settings.
        /// </summary>
        /// <returns>ConjugateGradient.</returns>
        protected override ConjugateGradient CreateOptimizer()
        {
            var cg = new ConjugateGradient(Model.Function.Weights.Length)
            {
                Tolerance = Tolerance,
                MaxIterations = MaxIterations,
            };

            cg.Progress += new EventHandler<OptimizationProgressEventArgs>(progressChanged);
            return cg;
        }

        private void progressChanged(object sender, OptimizationProgressEventArgs e)
        {
            int percentage = 100;
            double ratio = e.GradientNorm / e.SolutionNorm;
            if (!Double.IsNaN(ratio))
                percentage = (int)Math.Max(0, Math.Min(100, (1.0 - ratio) * 100));

            if (ProgressChanged != null)
                ProgressChanged(this, new ProgressChangedEventArgs(percentage, e));
        }

    }
}
