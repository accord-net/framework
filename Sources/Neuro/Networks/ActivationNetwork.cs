// AForge Neural Net Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2012
// contacts@aforgenet.com
//

namespace AForge.Neuro
{
    using System;

    /// <summary>
    /// Activation network.
    /// </summary>
    /// 
    /// <remarks><para>Activation network is a base for multi-layer neural network
    /// with activation functions. It consists of <see cref="ActivationLayer">activation
    /// layers</see>.</para>
    ///
    /// <para>Sample usage:</para>
    /// <code>
    /// // create activation network
    ///	ActivationNetwork network = new ActivationNetwork(
    ///		new SigmoidFunction( ), // sigmoid activation function
    ///		3,                      // 3 inputs
    ///		4, 1 );                 // 2 layers:
    ///                             // 4 neurons in the firs layer
    ///                             // 1 neuron in the second layer
    ///	</code>
    /// </remarks>
    /// 
    [Serializable]
    public class ActivationNetwork : Network
    {


        /// <summary>
        /// Initializes a new instance of the <see cref="ActivationNetwork"/> class.
        /// </summary>
        /// 
        /// <param name="function">Activation function of neurons of the network.</param>
        /// <param name="inputsCount">Network's inputs count.</param>
        /// <param name="neuronsCount">Array, which specifies the amount of neurons in
        /// each layer of the neural network.</param>
        /// 
        /// <remarks>The new network is randomized (see <see cref="ActivationNeuron.Randomize"/>
        /// method) after it is created.</remarks>
        /// 
        public ActivationNetwork( IActivationFunction function, int inputsCount, params int[] neuronsCount )
            : base( inputsCount, neuronsCount.Length )
        {
            // create each layer
            for ( int i = 0; i < layers.Length; i++ )
            {
                layers[i] = new ActivationLayer(
                    // neurons count in the layer
                    neuronsCount[i],
                    // inputs count of the layer
                    ( i == 0 ) ? inputsCount : neuronsCount[i - 1],
                    // activation function of the layer
                    function );
            }
        }

        /// <summary>
        /// Set new activation function for all neurons of the network.
        /// </summary>
        /// 
        /// <param name="function">Activation function to set.</param>
        /// 
        /// <remarks><para>The method sets new activation function for all neurons by calling
        /// <see cref="ActivationLayer.SetActivationFunction"/> method for each layer of the network.</para></remarks>
        /// 
        public void SetActivationFunction( IActivationFunction function )
        {
            for ( int i = 0; i < layers.Length; i++ )
            {
                ( (ActivationLayer) layers[i] ).SetActivationFunction( function );
            }
        }
    }
}
