// AForge Neural Net Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2012
// contacts@aforgenet.com
//

namespace AForge.Neuro.Learning
{
    using System;

    /// <summary>
    /// Perceptron learning algorithm.
    /// </summary>
    /// 
    /// <remarks><para>This learning algorithm is used to train one layer neural
    /// network of <see cref="ActivationNeuron">Activation Neurons</see>
    /// with the <see cref="ThresholdFunction">Threshold</see>
    /// activation function.</para>
    /// 
    /// <para>See information about <a href="http://en.wikipedia.org/wiki/Perceptron">Perceptron</a>
    /// and its learning algorithm.</para>
    /// </remarks>
    /// 
    public class PerceptronLearning : ISupervisedLearning
    {
        // network to teach
        private ActivationNetwork network;
        // learning rate
        private double learningRate = 0.1;

        /// <summary>
        /// Learning rate, [0, 1].
        /// </summary>
        /// 
        /// <remarks><para>The value determines speed of learning.</para>
        /// 
        /// <para>Default value equals to <b>0.1</b>.</para>
        /// </remarks>
        /// 
        public double LearningRate
        {
            get { return learningRate; }
            set
            {
                learningRate = Math.Max( 0.0, Math.Min( 1.0, value ) );
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerceptronLearning"/> class.
        /// </summary>
        /// 
        /// <param name="network">Network to teach.</param>
        /// 
        /// <exception cref="ArgumentException">Invalid nuaral network. It should have one layer only.</exception>
        /// 
        public PerceptronLearning( ActivationNetwork network )
        {
            // check layers count
            if ( network.Layers.Length != 1 )
            {
                throw new ArgumentException( "Invalid nuaral network. It should have one layer only." );
            }

            this.network = network;
        }

        /// <summary>
        /// Runs learning iteration.
        /// </summary>
        /// 
        /// <param name="input">Input vector.</param>
        /// <param name="output">Desired output vector.</param>
        /// 
        /// <returns>Returns absolute error - difference between current network's output and
        /// desired output.</returns>
        /// 
        /// <remarks><para>Runs one learning iteration and updates neuron's
        /// weights in the case if neuron's output is not equal to the
        /// desired output.</para></remarks>
        /// 
        public double Run( double[] input, double[] output )
        {
            // compute output of network
            double[] networkOutput = network.Compute( input );

            // get the only layer of the network
            Layer layer = network.Layers[0];

            // summary network absolute error
            double error = 0.0;

            // check output of each neuron and update weights
            for ( int j = 0; j < layer.Neurons.Length; j++ )
            {
                double e = output[j] - networkOutput[j];

                if ( e != 0 )
                {
                    ActivationNeuron perceptron = layer.Neurons[j] as ActivationNeuron;

                    // update weights
                    for ( int i = 0; i < perceptron.Weights.Length; i++ )
                    {
                        perceptron.Weights[i] += learningRate * e * input[i];
                    }

                    // update threshold value
                    perceptron.Threshold += learningRate * e;

                    // make error to be absolute
                    error += Math.Abs( e );
                }
            }

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
            double error = 0.0;

            // run learning procedure for all samples
            for ( int i = 0, n = input.Length; i < n; i++ )
            {
                error += Run( input[i], output[i] );
            }

            // return summary error
            return error;
        }
    }
}
