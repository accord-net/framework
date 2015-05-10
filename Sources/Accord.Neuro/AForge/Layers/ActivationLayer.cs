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
    /// Activation layer.
    /// </summary>
    /// 
    /// <remarks>Activation layer is a layer of <see cref="ActivationNeuron">activation neurons</see>.
    /// The layer is usually used in multi-layer neural networks.</remarks>
    ///
    [Serializable]
    public class ActivationLayer : Layer
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivationLayer"/> class.
        /// </summary>
        /// 
        /// <param name="neuronsCount">Layer's neurons count.</param>
        /// <param name="inputsCount">Layer's inputs count.</param>
        /// <param name="function">Activation function of neurons of the layer.</param>
        /// 
        /// <remarks>The new layer is randomized (see <see cref="ActivationNeuron.Randomize"/>
        /// method) after it is created.</remarks>
        /// 
        public ActivationLayer( int neuronsCount, int inputsCount, IActivationFunction function )
            : base( neuronsCount, inputsCount )
        {
            // create each neuron
            for ( int i = 0; i < neurons.Length; i++ )
                neurons[i] = new ActivationNeuron( inputsCount, function );
        }

        /// <summary>
        /// Set new activation function for all neurons of the layer.
        /// </summary>
        /// 
        /// <param name="function">Activation function to set.</param>
        /// 
        /// <remarks><para>The methods sets new activation function for each neuron by setting
        /// their <see cref="ActivationNeuron.ActivationFunction"/> property.</para></remarks>
        /// 
        public void SetActivationFunction( IActivationFunction function )
        {
            for ( int i = 0; i < neurons.Length; i++ )
            {
                ( (ActivationNeuron) neurons[i] ).ActivationFunction = function;
            }
        }
    }
}
