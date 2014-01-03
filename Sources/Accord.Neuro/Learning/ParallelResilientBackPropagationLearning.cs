// Accord Neural Net Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
// cesarsouza at gmail.com
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
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
// This file is based on the original BackPropagationLearning.cs file from
// the AForge.NET Neural Net Library, part of the AForge.NET Framework. The
// AForge.NET Framework is also available under the LGPL.
//

namespace Accord.Neuro.Learning
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AForge.Neuro;
    using AForge.Neuro.Learning;

#if !NET35
    /// <summary>
    ///   Resilient Backpropagation learning algorithm.
    /// </summary>
    /// 
    /// <remarks><para>This class implements the resilient backpropagation (RProp)
    /// learning algorithm. The RProp learning algorithm is one of the fastest learning
    /// algorithms for feed-forward learning networks which use only first-order
    /// information.</para>
    /// 
    /// <para>Sample usage (training network to calculate XOR function):</para>
    /// <code>
    /// // initialize input and output values
    /// double[][] input = new double[4][] {
    ///     new double[] {0, 0}, new double[] {0, 1},
    ///     new double[] {1, 0}, new double[] {1, 1}
    /// };
    /// double[][] output = new double[4][] {
    ///     new double[] {0}, new double[] {1},
    ///     new double[] {1}, new double[] {0}
    /// };
    /// // create neural network
    /// ActivationNetwork   network = new ActivationNetwork(
    ///     SigmoidFunction( 2 ),
    ///     2, // two inputs in the network
    ///     2, // two neurons in the first layer
    ///     1 ); // one neuron in the second layer
    /// // create teacher
    /// ResilientBackpropagationLearning teacher = new ResilientBackpropagationLearning( network );
    /// // loop
    /// while ( !needToStop )
    /// {
    ///     // run epoch of learning procedure
    ///     double error = teacher.RunEpoch( input, output );
    ///     // check error value to see if we need to stop
    ///     // ...
    /// }
    /// </code>
    /// </remarks>
    /// 
    /// <seealso cref="LevenbergMarquardtLearning"/>
    /// 
    public class ParallelResilientBackpropagationLearning : ISupervisedLearning, IDisposable
    {
        private ActivationNetwork network;

        private double initialStep = 0.0125;
        private double deltaMax = 50.0;
        private double deltaMin = 1e-6;

        private double etaMinus = 0.5;
        private double etaPlus = 1.2;

        private Object lockNetwork = new Object();
        private ThreadLocal<double[][]> networkErrors;
        private ThreadLocal<double[][]> networkOutputs;

        // update values, also known as deltas
        private double[][][] weightsUpdates;
        private double[][] thresholdsUpdates;

        // current and previous gradient values
        private double[][][] weightsDerivatives;
        private double[][] thresholdsDerivatives;

        private double[][][] weightsPreviousDerivatives;
        private double[][] thresholdsPreviousDerivatives;


        /// <summary>
        ///   Gets or sets the maximum possible update step,
        ///   also referred as delta min. Default is 50.
        /// </summary>
        /// 
        public double UpdateUpperBound
        {
            get { return deltaMax; }
            set { deltaMax = value; }
        }

        /// <summary>
        ///   Gets or sets the minimum possible update step,
        ///   also referred as delta max. Default is 1e-6.
        /// </summary>
        /// 
        public double UpdateLowerBound
        {
            get { return deltaMin; }
            set { deltaMin = value; }
        }

        /// <summary>
        ///   Gets the decrease parameter, also 
        ///   referred as eta minus. Default is 0.5.
        /// </summary>
        /// 
        public double DecreaseFactor
        {
            get { return etaMinus; }
            set
            {
                if (value <= 0 || value >= 1)
                    throw new ArgumentOutOfRangeException("value", "Value should be between 0 and 1.");
                etaMinus = value;
            }
        }

        /// <summary>
        ///   Gets the increase parameter, also
        ///   referred as eta plus. Default is 1.2.
        /// </summary>
        /// 
        public double IncreaseFactor
        {
            get { return etaPlus; }
            set
            {
                if (value <= 1)
                    throw new ArgumentOutOfRangeException("value", "Value should be higher than 1.");
                etaPlus = value;
            }
        }


        /// <summary>
        ///   Initializes a new instance of the <see cref="ParallelResilientBackpropagationLearning"/> class.
        /// </summary>
        /// 
        /// <param name="network">Network to teach.</param>
        /// 
        public ParallelResilientBackpropagationLearning(ActivationNetwork network)
        {
            this.network = network;

            networkOutputs = new ThreadLocal<double[][]>(() => new double[network.Layers.Length][]);

            networkErrors = new ThreadLocal<double[][]>(() =>
            {
                var e = new double[network.Layers.Length][];
                for (int i = 0; i < e.Length; i++)
                    e[i] = new double[network.Layers[i].Neurons.Length];
                return e;
            });


            weightsDerivatives = new double[network.Layers.Length][][];
            thresholdsDerivatives = new double[network.Layers.Length][];

            weightsPreviousDerivatives = new double[network.Layers.Length][][];
            thresholdsPreviousDerivatives = new double[network.Layers.Length][];

            weightsUpdates = new double[network.Layers.Length][][];
            thresholdsUpdates = new double[network.Layers.Length][];

            // Initialize layer derivatives and updates
            for (int i = 0; i < network.Layers.Length; i++)
            {
                Layer layer = network.Layers[i];

                weightsDerivatives[i] = new double[layer.Neurons.Length][];
                weightsPreviousDerivatives[i] = new double[layer.Neurons.Length][];
                weightsUpdates[i] = new double[layer.Neurons.Length][];

                // for each neuron
                for (int j = 0; j < layer.Neurons.Length; j++)
                {
                    weightsDerivatives[i][j] = new double[layer.InputsCount];
                    weightsPreviousDerivatives[i][j] = new double[layer.InputsCount];
                    weightsUpdates[i][j] = new double[layer.InputsCount];
                }

                thresholdsDerivatives[i] = new double[layer.Neurons.Length];
                thresholdsPreviousDerivatives[i] = new double[layer.Neurons.Length];
                thresholdsUpdates[i] = new double[layer.Neurons.Length];
            }

            // Initialize steps
            Reset(initialStep);
        }



        /// <summary>
        ///   Runs learning iteration.
        /// </summary>
        /// 
        /// <param name="input">Input vector.</param>
        /// <param name="output">Desired output vector.</param>
        /// 
        /// <returns>Returns squared error (difference between current network's output and
        /// desired output) divided by 2.</returns>
        /// 
        /// <remarks><para>Runs one learning iteration and updates neuron's
        /// weights.</para></remarks>
        ///
        public double Run(double[] input, double[] output)
        {
            // Zero gradient
            ResetGradient();

            // Compute forward pass
            network.Compute(input);

            // Copy network outputs to local thread
            var networkOutputs = this.networkOutputs.Value;
            for (int j = 0; j < networkOutputs.Length; j++)
                networkOutputs[j] = network.Layers[j].Output;

            // Calculate network error
            double error = CalculateError(output);

            // Calculate weights updates
            CalculateGradient(input);

            // Update the network
            UpdateNetwork();

            return error;
        }

        /// <summary>
        ///   Runs learning epoch.
        /// </summary>
        /// 
        /// <param name="input">Array of input vectors.</param>
        /// <param name="output">Array of output vectors.</param>
        /// 
        /// <returns>Returns summary learning error for the epoch. See <see cref="Run"/>
        /// method for details about learning error calculation.</returns>
        /// 
        /// <remarks><para>The method runs one learning epoch, by calling <see cref="Run"/> method
        /// for each vector provided in the <paramref name="input"/> array.</para></remarks>
        /// 
        public double RunEpoch(double[][] input, double[][] output)
        {
            // Zero gradient
            ResetGradient();

            Object lockSum = new Object();
            double sumOfSquaredErrors = 0;


            // For all examples in batch
            Parallel.For(0, input.Length,

                // Initialize
                () => 0.0,

                // Map
                (i, loopState, partialSum) =>
                {

                    lock (lockNetwork)
                    {
                        // Compute a forward pass
                        network.Compute(input[i]);

                        // Copy network outputs to local thread
                        var networkOutputs = this.networkOutputs.Value;
                        for (int j = 0; j < networkOutputs.Length; j++)
                            networkOutputs[j] = network.Layers[j].Output;
                    }

                    // Calculate and accumulate network error
                    partialSum += CalculateError(output[i]);

                    // Calculate weights updates
                    CalculateGradient(input[i]);

                    return partialSum;
                },

                // Reduce
                (partialSum) =>
                {
                    lock (lockSum) sumOfSquaredErrors += partialSum;
                }
            );


            // Update the network
            UpdateNetwork();


            return sumOfSquaredErrors;
        }

        /// <summary>
        ///   Update network weights.
        /// </summary>
        /// 
        private void UpdateNetwork()
        {
            // For each layer of the network
            for (int i = 0; i < weightsUpdates.Length; i++)
            {
                ActivationLayer layer = this.network.Layers[i] as ActivationLayer;
                double[][] layerWeightsUpdates = weightsUpdates[i];
                double[] layerThresholdUpdates = thresholdsUpdates[i];

                double[][] layerWeightsDerivatives = weightsDerivatives[i];
                double[] layerThresholdDerivatives = thresholdsDerivatives[i];

                double[][] layerPreviousWeightsDerivatives = weightsPreviousDerivatives[i];
                double[] layerPreviousThresholdDerivatives = thresholdsPreviousDerivatives[i];

                // For each neuron in the current layer
                for (int j = 0; j < layerWeightsUpdates.Length; j++)
                {
                    ActivationNeuron neuron = layer.Neurons[j] as ActivationNeuron;

                    double[] neuronWeightUpdates = layerWeightsUpdates[j];
                    double[] neuronWeightDerivatives = layerWeightsDerivatives[j];
                    double[] neuronPreviousWeightDerivatives = layerPreviousWeightsDerivatives[j];

                    double S;

                    // For each weight in the current neuron
                    for (int k = 0; k < neuronPreviousWeightDerivatives.Length; k++)
                    {
                        S = neuronPreviousWeightDerivatives[k] * neuronWeightDerivatives[k];

                        if (S > 0.0)
                        {
                            neuronWeightUpdates[k] = Math.Min(neuronWeightUpdates[k] * etaPlus, deltaMax);
                            neuron.Weights[k] -= Math.Sign(neuronWeightDerivatives[k]) * neuronWeightUpdates[k];
                            neuronPreviousWeightDerivatives[k] = neuronWeightDerivatives[k];
                        }
                        else if (S < 0.0)
                        {
                            neuronWeightUpdates[k] = Math.Max(neuronWeightUpdates[k] * etaMinus, deltaMin);
                            neuronPreviousWeightDerivatives[k] = 0.0;
                        }
                        else
                        {
                            neuron.Weights[k] -= Math.Sign(neuronWeightDerivatives[k]) * neuronWeightUpdates[k];
                            neuronPreviousWeightDerivatives[k] = neuronWeightDerivatives[k];
                        }
                    }

                    S = layerPreviousThresholdDerivatives[j] * layerThresholdDerivatives[j];

                    if (S > 0.0)
                    {
                        layerThresholdUpdates[j] = Math.Min(layerThresholdUpdates[j] * etaPlus, deltaMax);
                        neuron.Threshold -= Math.Sign(layerThresholdDerivatives[j]) * layerThresholdUpdates[j];
                        layerPreviousThresholdDerivatives[j] = layerThresholdDerivatives[j];
                    }
                    else if (S < 0.0)
                    {
                        layerThresholdUpdates[j] = Math.Max(layerThresholdUpdates[j] * etaMinus, deltaMin);
                        layerThresholdDerivatives[j] = 0.0;
                    }
                    else
                    {
                        neuron.Threshold -= Math.Sign(layerThresholdDerivatives[j]) * layerThresholdUpdates[j];
                        layerPreviousThresholdDerivatives[j] = layerThresholdDerivatives[j];
                    }
                }
            }
        }

        /// <summary>
        ///   Compute network error for a given data set.
        /// </summary>
        /// 
        /// <param name="input">The input points.</param>
        /// <param name="output">The output points.</param>
        /// 
        /// <returns>The sum of squared errors for the data.</returns>
        /// 
        public double ComputeError(double[][] input, double[][] output)
        {
            Object lockSum = new Object();
            double sumOfSquaredErrors = 0;


            Parallel.For(0, input.Length,

                // Initialize
                () => 0.0,

                // Map
                (i, loopState, partialSum) =>
                {
                    // Compute network answer
                    double[] y = network.Compute(input[i]);

                    for (int j = 0; j < y.Length; j++)
                    {
                        double e = (y[j] - output[i][j]);
                        partialSum += e * e;
                    }

                    return partialSum;
                },

                // Reduce
                (partialSum) =>
                {
                    lock (lockSum) sumOfSquaredErrors += partialSum;
                }
            );

            return sumOfSquaredErrors / 2.0;
        }

        /// <summary>
        ///   Resets the current update steps using the given learning rate.
        /// </summary>
        /// 
        public void Reset(double rate)
        {
            Parallel.For(0, weightsUpdates.Length, i =>
            {
                for (int j = 0; j < weightsUpdates[i].Length; j++)
                    for (int k = 0; k < weightsUpdates[i][j].Length; k++)
                        weightsUpdates[i][j][k] = rate;

                for (int j = 0; j < thresholdsUpdates[i].Length; j++)
                    thresholdsUpdates[i][j] = rate;
            });
        }

        /// <summary>
        ///   Resets the gradient vector back to zero.
        /// </summary>
        /// 
        private void ResetGradient()
        {
            Parallel.For(0, weightsDerivatives.Length, i =>
            {
                for (int j = 0; j < weightsDerivatives[i].Length; j++)
                    Array.Clear(weightsDerivatives[i][j], 0, weightsDerivatives[i][j].Length);
                Array.Clear(thresholdsDerivatives[i], 0, thresholdsDerivatives[i].Length);
            });
        }

        /// <summary>
        ///   Calculates error values for all neurons of the network.
        /// </summary>
        /// 
        /// <param name="desiredOutput">Desired output vector.</param>
        /// 
        /// <returns>Returns summary squared error of the last layer divided by 2.</returns>
        /// 
        private double CalculateError(double[] desiredOutput)
        {
            double sumOfSquaredErrors = 0.0;
            int layersCount = network.Layers.Length;

            double[][] networkErrors = this.networkErrors.Value;
            double[][] networkOutputs = this.networkOutputs.Value;

            // Assume that all network neurons have the same activation function
            var function = (this.network.Layers[0].Neurons[0] as ActivationNeuron)
                .ActivationFunction;

            // 1. Calculate error values for last layer first.
            double[] layerOutputs = networkOutputs[layersCount - 1];
            double[] errors = networkErrors[layersCount - 1];

            for (int i = 0; i < errors.Length; i++)
            {
                double output = layerOutputs[i];
                double e = output - desiredOutput[i];
                errors[i] = e * function.Derivative2(output);
                sumOfSquaredErrors += e * e;
            }

            // 2. Calculate errors for all other layers
            for (int j = layersCount - 2; j >= 0; j--)
            {
                errors = networkErrors[j];
                layerOutputs = networkOutputs[j];

                ActivationLayer nextLayer = network.Layers[j + 1] as ActivationLayer;
                double[] nextErrors = networkErrors[j + 1];

                // For all neurons of this layer
                for (int i = 0; i < errors.Length; i++)
                {
                    double sum = 0.0;

                    // For all neurons of the next layer
                    for (int k = 0; k < nextErrors.Length; k++)
                        sum += nextErrors[k] * nextLayer.Neurons[k].Weights[i];

                    errors[i] = sum * function.Derivative2(layerOutputs[i]);
                }
            }

            return sumOfSquaredErrors / 2.0;
        }

        /// <summary>
        ///   Computes the gradient for a given input.
        /// </summary>
        ///
        /// <param name="input">Network's input vector.</param>
        ///
        private void CalculateGradient(double[] input)
        {
            double[][] networkErrors = this.networkErrors.Value;
            double[][] networkOutputs = this.networkOutputs.Value;

            // 1. Calculate for last layer first
            double[] errors = networkErrors[0];
            double[] outputs = networkOutputs[0];
            double[][] layerWeightsDerivatives = weightsDerivatives[0];
            double[] layerThresholdDerivatives = thresholdsDerivatives[0];

            // For each neuron of the last layer
            for (int i = 0; i < errors.Length; i++)
            {
                double[] neuronWeightDerivatives = layerWeightsDerivatives[i];

                lock (neuronWeightDerivatives)
                {
                    // For each weight in the neuron
                    for (int j = 0; j < input.Length; j++)
                        neuronWeightDerivatives[j] += errors[i] * input[j];
                    layerThresholdDerivatives[i] += errors[i];
                }
            }

            // 2. Calculate for all other layers in a chain
            for (int k = 1; k < weightsDerivatives.Length; k++)
            {
                errors = networkErrors[k];
                outputs = networkOutputs[k];

                layerWeightsDerivatives = weightsDerivatives[k];
                layerThresholdDerivatives = thresholdsDerivatives[k];

                double[] layerPrev = networkOutputs[k - 1];

                // For each neuron in the current layer
                for (int i = 0; i < layerWeightsDerivatives.Length; i++)
                {
                    double[] neuronWeightDerivatives = layerWeightsDerivatives[i];

                    lock (neuronWeightDerivatives)
                    {
                        // For each weight of the neuron
                        for (int j = 0; j < neuronWeightDerivatives.Length; j++)
                            neuronWeightDerivatives[j] += errors[i] * layerPrev[j];
                        layerThresholdDerivatives[i] += errors[i];
                    }
                }
            }
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
        ///   the <see cref="ParallelResilientBackpropagationLearning"/> is reclaimed by garbage
        ///   collection.
        /// </summary>
        /// 
        ~ParallelResilientBackpropagationLearning()
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
                if (networkErrors != null)
                {
                    networkErrors.Dispose();
                    networkErrors = null;
                }
                if (networkOutputs != null)
                {
                    networkOutputs.Dispose();
                    networkOutputs = null;
                }
            }
        }

        #endregion

    }

#else

    /// <summary>
    ///   Compatibility shim to make Accord.NET work on previous
    ///   version of the framework. This is just a wrapper around
    ///   AForge.Neuro.Learning.ResilientBackpropagationLearning.
    /// </summary>
    /// 
    public class ParallelResilientBackpropagationLearning : AForge.Neuro.Learning.ResilientBackpropagationLearning
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="ParallelResilientBackpropagationLearning"/> class.
        /// </summary>
        /// 
        public ParallelResilientBackpropagationLearning(AForge.Neuro.ActivationNetwork network)
            : base(network) { }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        /// 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "rate")]
        public void Reset(double rate)
        {
            throw new NotSupportedException();
        }
    }

#endif

}