// AForge Neural Net Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © César Souza, 2009-2012
// cesarsouza at gmail.com
//
// Copyright © AForge.NET, 2005-2012
// contacts@aforgenet.com
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

namespace AForge.Neuro.Learning
{
    using System;

    /// <summary>
    /// Resilient Backpropagation learning algorithm.
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
    public class ResilientBackpropagationLearning : ISupervisedLearning
    {
        private ActivationNetwork network;

        private double learningRate = 0.0125;
        private double deltaMax = 50.0;
        private double deltaMin = 1e-6;

        private const double etaMinus = 0.5;
        private double etaPlus = 1.2;

        private double[][] neuronErrors = null;

        // update values, also known as deltas
        private double[][][] weightsUpdates = null;
        private double[][] thresholdsUpdates = null;

        // current and previous gradient values
        private double[][][] weightsDerivatives = null;
        private double[][] thresholdsDerivatives = null;

        private double[][][] weightsPreviousDerivatives = null;
        private double[][] thresholdsPreviousDerivatives = null;


        /// <summary>
        /// Learning rate.
        /// </summary>
        /// 
        /// <remarks><para>The value determines speed of learning.</para>
        /// 
        /// <para>Default value equals to <b>0.0125</b>.</para>
        /// </remarks>
        ///
        public double LearningRate
        {
            get { return learningRate; }
            set
            {
                learningRate = value;
                ResetUpdates( learningRate );
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResilientBackpropagationLearning"/> class.
        /// </summary>
        /// 
        /// <param name="network">Network to teach.</param>
        /// 
        public ResilientBackpropagationLearning( ActivationNetwork network )
        {
            this.network = network;

            int layersCount = network.Layers.Length;

            neuronErrors = new double[layersCount][];

            weightsDerivatives    = new double[layersCount][][];
            thresholdsDerivatives = new double[layersCount][];

            weightsPreviousDerivatives    = new double[layersCount][][];
            thresholdsPreviousDerivatives = new double[layersCount][];

            weightsUpdates    = new double[layersCount][][];
            thresholdsUpdates = new double[layersCount][];

            // initialize errors, derivatives and steps
            for ( int i = 0; i < network.Layers.Length; i++ )
            {
                Layer layer = network.Layers[i];
                int neuronsCount = layer.Neurons.Length;

                neuronErrors[i] = new double[neuronsCount];

                weightsDerivatives[i]         = new double[neuronsCount][];
                weightsPreviousDerivatives[i] = new double[neuronsCount][];
                weightsUpdates[i]             = new double[neuronsCount][];

                thresholdsDerivatives[i]         = new double[neuronsCount];
                thresholdsPreviousDerivatives[i] = new double[neuronsCount];
                thresholdsUpdates[i]             = new double[neuronsCount];

                // for each neuron
                for ( int j = 0; j < layer.Neurons.Length; j++ )
                {
                    weightsDerivatives[i][j]         = new double[layer.InputsCount];
                    weightsPreviousDerivatives[i][j] = new double[layer.InputsCount];
                    weightsUpdates[i][j]             = new double[layer.InputsCount];
                }
            }

            // intialize steps
            ResetUpdates( learningRate );
        }

        /// <summary>
        /// Runs learning iteration.
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
        public double Run( double[] input, double[] output )
        {
            // zero gradient
            ResetGradient( );

            // compute the network's output
            network.Compute( input );

            // calculate network error
            double error = CalculateError( output );

            // calculate weights updates
            CalculateGradient( input );

            // update the network
            UpdateNetwork( );

            // return summary error
            return error;
        }

        /// <summary>
        /// Runs learning epoch.
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
        public double RunEpoch( double[][] input, double[][] output )
        {
            // zero gradient
            ResetGradient( );

            double error = 0.0;

            // run learning procedure for all samples
            for ( int i = 0; i < input.Length; i++ )
            {
                // compute the network's output
                network.Compute( input[i] );

                // calculate network error
                error += CalculateError( output[i] );

                // calculate weights updates
                CalculateGradient( input[i] );
            }

            // update the network
            UpdateNetwork( );

            // return summary error
            return error;
        }

        /// <summary>
        /// Resets current weight and threshold derivatives.
        /// </summary>
        /// 
        private void ResetGradient( )
        {
            for ( int i = 0; i < weightsDerivatives.Length; i++ )
            {
                for ( int j = 0; j < weightsDerivatives[i].Length; j++ )
                {
                    Array.Clear( weightsDerivatives[i][j], 0, weightsDerivatives[i][j].Length );
                }
            }

            for ( int i = 0; i < thresholdsDerivatives.Length; i++ )
            {
                Array.Clear( thresholdsDerivatives[i], 0, thresholdsDerivatives[i].Length );
            }
        }

        /// <summary>
        /// Resets the current update steps using the given learning rate.
        /// </summary>
        /// 
        private void ResetUpdates( double rate )
        {
            for ( int i = 0; i < weightsUpdates.Length; i++ )
            {
                for ( int j = 0; j < weightsUpdates[i].Length; j++ )
                {
                    for ( int k = 0; k < weightsUpdates[i][j].Length; k++ )
                    {
                        weightsUpdates[i][j][k] = rate;
                    }
                }
            }

            for ( int i = 0; i < thresholdsUpdates.Length; i++ )
            {
                for ( int j = 0; j < thresholdsUpdates[i].Length; j++ )
                {
                    thresholdsUpdates[i][j] = rate;
                }
            }
        }

        /// <summary>
        /// Update network's weights.
        /// </summary>
        /// 
        private void UpdateNetwork( )
        {
            double[][] layerWeightsUpdates;
            double[] layerThresholdUpdates;
            double[] neuronWeightUpdates;

            double[][] layerWeightsDerivatives;
            double[] layerThresholdDerivatives;
            double[] neuronWeightDerivatives;

            double[][] layerPreviousWeightsDerivatives;
            double[] layerPreviousThresholdDerivatives;
            double[] neuronPreviousWeightDerivatives;

            // for each layer of the network
            for ( int i = 0; i < network.Layers.Length; i++ )
            {
                ActivationLayer layer = network.Layers[i] as ActivationLayer;

                layerWeightsUpdates = weightsUpdates[i];
                layerThresholdUpdates = thresholdsUpdates[i];

                layerWeightsDerivatives = weightsDerivatives[i];
                layerThresholdDerivatives = thresholdsDerivatives[i];

                layerPreviousWeightsDerivatives = weightsPreviousDerivatives[i];
                layerPreviousThresholdDerivatives = thresholdsPreviousDerivatives[i];

                // for each neuron of the layer
                for ( int j = 0; j < layer.Neurons.Length; j++ )
                {
                    ActivationNeuron neuron = layer.Neurons[j] as ActivationNeuron;

                    neuronWeightUpdates = layerWeightsUpdates[j];
                    neuronWeightDerivatives = layerWeightsDerivatives[j];
                    neuronPreviousWeightDerivatives = layerPreviousWeightsDerivatives[j];

                    double S = 0;

                    // for each weight of the neuron
                    for ( int k = 0; k < neuron.InputsCount; k++ )
                    {
                        S = neuronPreviousWeightDerivatives[k] * neuronWeightDerivatives[k];

                        if ( S > 0 )
                        {
                            neuronWeightUpdates[k] = Math.Min( neuronWeightUpdates[k] * etaPlus, deltaMax );
                            neuron.Weights[k] -= Math.Sign( neuronWeightDerivatives[k] ) * neuronWeightUpdates[k];
                            neuronPreviousWeightDerivatives[k] = neuronWeightDerivatives[k];
                        }
                        else if ( S < 0 )
                        {
                            neuronWeightUpdates[k] = Math.Max( neuronWeightUpdates[k] * etaMinus, deltaMin );
                            neuronPreviousWeightDerivatives[k] = 0;
                        }
                        else
                        {
                            neuron.Weights[k] -= Math.Sign( neuronWeightDerivatives[k] ) * neuronWeightUpdates[k];
                            neuronPreviousWeightDerivatives[k] = neuronWeightDerivatives[k];
                        }
                    }

                    // update treshold
                    S = layerPreviousThresholdDerivatives[j] * layerThresholdDerivatives[j];

                    if ( S > 0 )
                    {
                        layerThresholdUpdates[j] = Math.Min( layerThresholdUpdates[j] * etaPlus, deltaMax );
                        neuron.Threshold -= Math.Sign( layerThresholdDerivatives[j] ) * layerThresholdUpdates[j];
                        layerPreviousThresholdDerivatives[j] = layerThresholdDerivatives[j];
                    }
                    else if ( S < 0 )
                    {
                        layerThresholdUpdates[j] = Math.Max( layerThresholdUpdates[j] * etaMinus, deltaMin );
                        layerThresholdDerivatives[j] = 0;
                    }
                    else
                    {
                        neuron.Threshold -= Math.Sign( layerThresholdDerivatives[j] ) * layerThresholdUpdates[j];
                        layerPreviousThresholdDerivatives[j] = layerThresholdDerivatives[j];
                    }
                }
            }
        }

        /// <summary>
        /// Calculates error values for all neurons of the network.
        /// </summary>
        /// 
        /// <param name="desiredOutput">Desired output vector.</param>
        /// 
        /// <returns>Returns summary squared error of the last layer divided by 2.</returns>
        /// 
        private double CalculateError( double[] desiredOutput )
        {
            double error = 0;
            int layersCount = network.Layers.Length;

            // assume, that all neurons of the network have the same activation function
            IActivationFunction function = ( network.Layers[0].Neurons[0] as ActivationNeuron ).ActivationFunction;

            // calculate error values for the last layer first
            ActivationLayer layer = network.Layers[layersCount - 1] as ActivationLayer;
            double[] layerDerivatives = neuronErrors[layersCount - 1];

            for ( int i = 0; i < layer.Neurons.Length; i++ )
            {
                double output = layer.Neurons[i].Output;

                double e = output - desiredOutput[i];
                layerDerivatives[i] = e * function.Derivative2( output );
                error += ( e * e );
            }

            // calculate error values for other layers
            for ( int j = layersCount - 2; j >= 0; j-- )
            {
                layer = network.Layers[j] as ActivationLayer;
                layerDerivatives = neuronErrors[j];

                ActivationLayer layerNext = network.Layers[j + 1] as ActivationLayer;
                double[] nextDerivatives = neuronErrors[j + 1];

                // for all neurons of the layer
                for ( int i = 0, n = layer.Neurons.Length; i < n; i++ )
                {
                    double sum = 0.0;

                    for ( int k = 0; k < layerNext.Neurons.Length; k++ )
                    {
                        sum += nextDerivatives[k] * layerNext.Neurons[k].Weights[i];
                    }

                    layerDerivatives[i] = sum * function.Derivative2( layer.Neurons[i].Output );
                }
            }

            // return squared error of the last layer divided by 2
            return error / 2.0;
        }

        /// <summary>
        /// Calculate weights updates
        /// </summary>
        /// 
        /// <param name="input">Network's input vector.</param>
        /// 
        private void CalculateGradient( double[] input )
        {
            // 1. calculate updates for the first layer
            ActivationLayer layer = network.Layers[0] as ActivationLayer;
            double[] weightErrors = neuronErrors[0];
            double[][] layerWeightsDerivatives = weightsDerivatives[0];
            double[] layerThresholdDerivatives = thresholdsDerivatives[0];

            // So, for each neuron of the first layer:
            for ( int i = 0; i < layer.Neurons.Length; i++ )
            {
                ActivationNeuron neuron = layer.Neurons[i] as ActivationNeuron;
                double[] neuronWeightDerivatives = layerWeightsDerivatives[i];

                // for each weight of the neuron:
                for ( int j = 0; j < neuron.InputsCount; j++ )
                {
                    neuronWeightDerivatives[j] += weightErrors[i] * input[j];
                }
                layerThresholdDerivatives[i] += weightErrors[i];
            }

            // 2. for all other layers
            for ( int k = 1; k < network.Layers.Length; k++ )
            {
                layer = network.Layers[k] as ActivationLayer;
                weightErrors = neuronErrors[k];
                layerWeightsDerivatives = weightsDerivatives[k];
                layerThresholdDerivatives = thresholdsDerivatives[k];

                ActivationLayer layerPrev = network.Layers[k - 1] as ActivationLayer;

                // for each neuron of the layer
                for ( int i = 0; i < layer.Neurons.Length; i++ )
                {
                    ActivationNeuron neuron = layer.Neurons[i] as ActivationNeuron;
                    double[] neuronWeightDerivatives = layerWeightsDerivatives[i];

                    // for each weight of the neuron
                    for ( int j = 0; j < layerPrev.Neurons.Length; j++ )
                    {
                        neuronWeightDerivatives[j] += weightErrors[i] * layerPrev.Neurons[j].Output;
                    }
                    layerThresholdDerivatives[i] += weightErrors[i];
                }
            }
        }
    }
}