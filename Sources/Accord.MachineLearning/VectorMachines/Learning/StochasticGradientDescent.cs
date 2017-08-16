// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
// Copyright © Leon Bottou, 2010-2017 
// http://leon.bottou.org/projects/sgd
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
// This file is based on the original C++ implementation by Leon Bottou, licensed
// under the LGPL 2.1 license and available at http://leon.bottou.org/projects/sgd
//

namespace Accord.MachineLearning.VectorMachines.Learning
{
    using Accord.Statistics.Kernels;
    using System;
    using Accord.Math;
    using System.Diagnostics;
    using System.Threading;
    using Statistics.Models.Regression.Linear;
    using Math.Optimization;
    using System.Collections;
    using Math.Optimization.Losses;
    using Accord.Compat;

    /// <summary>
    ///   Stochastic Gradient Descent (SGD) for training linear support vector machines.
    /// </summary>
    /// 
    /// <see cref="AveragedStochasticGradientDescent"/>
    /// <see cref="SequentialMinimalOptimization{TKernel}"/>
    /// <see cref="LinearNewtonMethod"/>
    /// <see cref="LinearDualCoordinateDescent"/>
    /// 
    /// <example>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\StochasticGradientDescentTest.cs" region="doc_learn_multiclass" />
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\StochasticGradientDescentTest.cs" region="doc_learn_multilabel" />
    /// </example>
    /// 
    public class StochasticGradientDescent :
        BaseStochasticGradientDescent<SupportVectorMachine, Linear, double[], LogisticLoss>
    {
        /// <summary>
        /// Creates an instance of the model to be learned. Inheritors
        /// of this abstract class must define this method so new models
        /// can be created from the training data.
        /// </summary>
        protected override SupportVectorMachine Create(int inputs, Linear kernel)
        {
            return new SupportVectorMachine(inputs) { Kernel = kernel };
        }

        /// <summary>
        /// Inheritors should implement this function to produce a new instance
        /// with the same characteristics of the current object.
        /// </summary>
        /// 
        protected override BaseStochasticGradientDescent<SupportVectorMachine, Linear, double[], LogisticLoss> InnerClone()
        {
            return new StochasticGradientDescent();
        }
    }

    /// <summary>
    ///   Stochastic Gradient Descent (SGD) for training linear support vector machines.
    /// </summary>
    /// 
    /// <see cref="AveragedStochasticGradientDescent"/>
    /// <see cref="SequentialMinimalOptimization{TKernel}"/>
    /// <see cref="LinearNewtonMethod"/>
    /// <see cref="LinearDualCoordinateDescent"/>
    /// 
    /// <example>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\StochasticGradientDescentTest.cs" region="doc_learn_multiclass" />
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\StochasticGradientDescentTest.cs" region="doc_learn_multilabel" />
    /// </example>
    /// 
    public class StochasticGradientDescent<TKernel> :
        BaseStochasticGradientDescent<SupportVectorMachine<TKernel>, TKernel, double[], LogisticLoss>
        where TKernel : struct, ILinear
    {
        /// <summary>
        /// Creates an instance of the model to be learned. Inheritors
        /// of this abstract class must define this method so new models
        /// can be created from the training data.
        /// </summary>
        protected override SupportVectorMachine<TKernel> Create(int inputs, TKernel kernel)
        {
            return new SupportVectorMachine<TKernel>(inputs, kernel);
        }

        /// <summary>
        /// Inheritors should implement this function to produce a new instance
        /// with the same characteristics of the current object.
        /// </summary>
        /// 
        protected override BaseStochasticGradientDescent<SupportVectorMachine<TKernel>, TKernel, double[], LogisticLoss> InnerClone()
        {
            return new StochasticGradientDescent<TKernel>();
        }
    }

    /// <summary>
    ///   Stochastic Gradient Descent (SGD) for training linear support vector machines.
    /// </summary>
    /// 
    /// <see cref="AveragedStochasticGradientDescent"/>
    /// <see cref="SequentialMinimalOptimization{TKernel}"/>
    /// <see cref="LinearNewtonMethod"/>
    /// <see cref="LinearDualCoordinateDescent"/>
    /// 
    /// <example>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\StochasticGradientDescentTest.cs" region="doc_learn_multiclass" />
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\StochasticGradientDescentTest.cs" region="doc_learn_multilabel" />
    /// </example>
    /// 
    public class StochasticGradientDescent<TKernel, TInput> :
        BaseStochasticGradientDescent<SupportVectorMachine<TKernel, TInput>, TKernel, TInput, LogisticLoss>
        where TKernel : struct, ILinear<TInput>
        where TInput : ICloneable, IList
    {
        /// <summary>
        /// Creates an instance of the model to be learned. Inheritors
        /// of this abstract class must define this method so new models
        /// can be created from the training data.
        /// </summary>
        protected override SupportVectorMachine<TKernel, TInput> Create(int inputs, TKernel kernel)
        {
            return new SupportVectorMachine<TKernel, TInput>(inputs, kernel);
        }

        /// <summary>
        /// Inheritors should implement this function to produce a new instance
        /// with the same characteristics of the current object.
        /// </summary>
        /// 
        protected override BaseStochasticGradientDescent<SupportVectorMachine<TKernel, TInput>, TKernel, TInput, LogisticLoss> InnerClone()
        {
            return new StochasticGradientDescent<TKernel, TInput>();
        }
    }


    /// <summary>
    ///   Stochastic Gradient Descent (SGD) for training linear support vector machines.
    /// </summary>
    /// 
    /// <see cref="AveragedStochasticGradientDescent"/>
    /// <see cref="SequentialMinimalOptimization{TKernel}"/>
    /// <see cref="LinearNewtonMethod"/>
    /// <see cref="LinearDualCoordinateDescent"/>
    /// 
    /// <example>
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\StochasticGradientDescentTest.cs" region="doc_learn_multiclass" />
    ///   <code source="Unit Tests\Accord.Tests.MachineLearning\VectorMachines\StochasticGradientDescentTest.cs" region="doc_learn_multilabel" />
    /// </example>
    /// 
    public class StochasticGradientDescent<TKernel, TInput, TLoss> :
        BaseStochasticGradientDescent<SupportVectorMachine<TKernel, TInput>, TKernel, TInput, TLoss>
        where TKernel : struct, ILinear<TInput>
        where TInput : IList
#if !NETSTANDARD1_4
        , ICloneable
#endif
        where TLoss : struct, IDifferentiableLoss<bool, double, double>
    {
        /// <summary>
        /// Creates an instance of the model to be learned. Inheritors
        /// of this abstract class must define this method so new models
        /// can be created from the training data.
        /// </summary>
        protected override SupportVectorMachine<TKernel, TInput> Create(int inputs, TKernel kernel)
        {
            return new SupportVectorMachine<TKernel, TInput>(inputs, kernel);
        }

        /// <summary>
        /// Inheritors should implement this function to produce a new instance
        /// with the same characteristics of the current object.
        /// </summary>
        /// 
        protected override BaseStochasticGradientDescent<SupportVectorMachine<TKernel, TInput>, TKernel, TInput, TLoss> InnerClone()
        {
            return new StochasticGradientDescent<TKernel, TInput, TLoss>();
        }
    }

    /// <summary>
    ///   Base class for Averaged Stochastic Gradient Descent algorithm implementations.
    /// </summary>
    /// 
    /// <typeparam name="TModel">The type of the model being learned.</typeparam>
    /// <typeparam name="TKernel">The type of the kernel function to use.</typeparam>
    /// <typeparam name="TInput">The type of the input to consider.</typeparam>
    /// <typeparam name="TLoss">The type of the loss function to use.</typeparam>
    /// 
    /// <remarks>
    ///   The <see cref="IKernel"/> and <see cref="IDifferentiableLoss{TInput, TScore, TLoss}"/>
    ///   are passed as generic parameters (constrained to be structs) because this is the only
    ///   way to force the compiler to emit a separate native code for this class whose performance
    ///   critical sections can be inlined.
    /// </remarks>
    /// 
    /// <seealso cref="StochasticGradientDescent"/>
    /// <seealso cref="AveragedStochasticGradientDescent"/>
    /// <seealso cref="Accord.MachineLearning.BinaryLearningBase{TModel, TInput}" />
    /// 
    public abstract class BaseStochasticGradientDescent<TModel, TKernel, TInput, TLoss> :
        BinaryLearningBase<TModel, TInput>, ICloneable
        where TModel : SupportVectorMachine<TKernel, TInput>
        where TKernel : struct, ILinear<TInput>
        where TInput : IList
#if !NETSTANDARD1_4
        , ICloneable
#endif
        where TLoss : struct, IDifferentiableLoss<bool, double, double>
    {

        TKernel kernel;

        double lambda = 1e-5;
        double eta0;
        double[] w;
        double wDivisor = 1;
        double wBias;
        double t;

        bool bias = true;
        bool regularizedBias = false;

        RelativeConvergence convergence = new RelativeConvergence();
        TLoss loss;

        /// <summary>
        ///   Gets or sets the kernel function use to create a 
        ///   kernel Support Vector Machine.
        /// </summary>
        /// 
        public TKernel Kernel
        {
            get { return kernel; }
            set { this.kernel = value; }
        }

        /// <summary>
        ///   Gets or sets the loss function to be used. 
        ///   Default is to use the <see cref="LogisticLoss"/>.
        /// </summary>
        /// 
        public TLoss Loss
        {
            get { return loss; }
            set { loss = value; }
        }

        /// <summary>
        ///   Gets or sets the learning rate for the SGD algorithm.
        /// </summary>
        /// 
        public double LearningRate
        {
            get { return eta0; }
            set { eta0 = value; }
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
        ///   Gets or sets the number of iterations that should be
        ///   performed by the algorithm when calling <see cref="Learn"/>.
        ///   Default is 0 (iterate until convergence).
        /// </summary>
        /// 
        public int MaxIterations
        {
            get { return convergence.MaxIterations; }
            set { convergence.MaxIterations = value; }
        }

        /// <summary>
        ///   Gets or sets the maximum relative change in the watched value
        ///   after an iteration of the algorithm used to detect convergence.
        ///   Default is 1e-5.
        /// </summary>
        /// 
        public double Tolerance
        {
            get { return convergence.Tolerance; }
            set { convergence.Tolerance = value; }
        }

        /// <summary>
        ///   Gets or sets the lambda regularization term. Default is 0.5.
        /// </summary>
        /// 
        public double Lambda
        {
            get { return lambda; }
            set { lambda = value; }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="BaseStochasticGradientDescent{TModel, TKernel, TInput, TLoss}"/> class.
        /// </summary>
        /// 
        public BaseStochasticGradientDescent()
        {
            MaxIterations = 0;
            Tolerance = 1e-5;
        }

        /// <summary>
        ///   Creates an instance of the model to be learned. Inheritors
        ///   of this abstract class must define this method so new models
        ///   can be created from the training data.
        /// </summary>
        /// 
        protected abstract TModel Create(int inputs, TKernel kernel);


        /// <summary>
        ///   Renormalize the weights
        /// </summary>
        /// 
        void renorm()
        {
            if (wDivisor != 1.0)
            {
                w.Divide(wDivisor, result: w);
                wDivisor = 1.0;
            }
        }

        /// <summary>
        ///   Compute the norm of the weights
        /// </summary>
        /// 
        double wnorm()
        {
            double norm = w.Dot(w) / wDivisor / wDivisor;
            if (regularizedBias)
                norm += wBias * wBias;
            return norm;
        }


        /// <summary>
        ///   Perform one iteration of the SGD algorithm with specified gains
        /// </summary>
        /// 
        void trainOne(TInput x, bool y, double eta)
        {
            double s = score(x);

            // update for regularization term
            wDivisor = wDivisor / (1 - eta * lambda);

            if (wDivisor > 1e5)
                renorm();

            // update for loss term
            double d = -loss.Derivative(y, s);

            if (d != 0)
                Kernel.Product(eta * d * wDivisor, x, accumulate: w);

            // same for the bias
            if (bias)
            {
                double etab = eta * 0.01;
                if (regularizedBias)
                    wBias *= (1 - etab * lambda);
                wBias += etab * d;
            }
        }

        private double score(TInput x)
        {
            return Kernel.Function(w, x) / wDivisor + wBias;
        }


        double evaluateEta(TInput[] x, bool[] y, double eta)
        {
            // take a copy of the current state
            var clone = (BaseStochasticGradientDescent<TModel, TKernel, TInput, TLoss>)this.Clone();

            for (int i = 0; i < x.Length; i++)
                clone.trainOne(x[i], y[i], eta);

            return clone.evaluateLoss(x, y);
        }

        private double evaluateLoss(TInput[] x, bool[] y)
        {
            double loss = 0;
            double cost = 0;
            for (int i = 0; i < x.Length; i++)
            {
                double s = this.score(x[i]);
                loss += this.loss.Loss(y[i], s);
            }
            loss = loss / x.Length;
            cost = loss + 0.5 * lambda * wnorm();
            return cost;
        }

        void determineEta0(TInput[] x, bool[] y)
        {
            const double factor = 2.0;

            double loEta = 1;
            double loCost = evaluateEta(x, y, loEta);

            double hiEta = loEta * factor;
            double hiCost = evaluateEta(x, y, hiEta);

            if (loCost < hiCost)
            {
                while (loCost < hiCost)
                {
                    hiEta = loEta;
                    hiCost = loCost;
                    loEta = hiEta / factor;
                    loCost = evaluateEta(x, y, loEta);
                }
            }
            else if (hiCost < loCost)
            {
                while (hiCost < loCost)
                {
                    loEta = hiEta;
                    loCost = hiCost;
                    hiEta = loEta * factor;
                    hiCost = evaluateEta(x, y, hiEta);
                }
            }

            this.eta0 = loEta;
        }


        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weights">The weight of importance for each input-output pair (if supported by the learning algorithm).</param>
        /// <returns>
        /// A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.
        /// </returns>
        public override TModel Learn(TInput[] x, bool[] y, double[] weights = null)
        {
            if (weights != null)
                throw new ArgumentException(Accord.Properties.Resources.NotSupportedWeights, "weights");

            if (Model == null)
                Model = Create(SupportVectorLearningHelper.GetNumberOfInputs(Kernel, x), Kernel);

            if (w == null)
                w = new double[Model.NumberOfInputs];

            if (eta0 == 0)
                determineEta0(x, y);

            do
            {
                for (int i = 0; i < x.Length; i++)
                {
                    trainOne(x[i], y[i], eta0 / (1 + lambda * eta0 * t));
                    t++;
                }

                convergence.NewValue = evaluateLoss(x, y);

            } while (!convergence.HasConverged);

            Model.Weights = new double[] { 1.0 };
            Model.SupportVectors = new[] { kernel.CreateVector(w) };
            Model.Threshold = wBias;

            return Model;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            var clone = InnerClone();

            clone.loss = loss;
            clone.kernel = kernel;
            clone.lambda = lambda;
            clone.eta0 = eta0;
            clone.w = w.Copy();
            clone.wDivisor = wDivisor;
            clone.wBias = wBias;
            clone.t = t;
            clone.bias = bias;
            clone.regularizedBias = regularizedBias;
            clone.Tolerance = Tolerance;
            clone.MaxIterations = MaxIterations;

            return clone;
        }

        /// <summary>
        /// Inheritors should implement this function to produce a new 
        /// instance with the same characteristics of the current object.
        /// </summary>
        /// 
        protected abstract BaseStochasticGradientDescent<TModel, TKernel, TInput, TLoss> InnerClone();
    }
}
