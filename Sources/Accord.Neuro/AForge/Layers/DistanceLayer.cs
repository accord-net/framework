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
    /// Distance layer.
    /// </summary>
    /// 
    /// <remarks>Distance layer is a layer of <see cref="DistanceNeuron">distance neurons</see>.
    /// The layer is usually a single layer of such networks as Kohonen Self
    /// Organizing Map, Elastic Net, Hamming Memory Net.</remarks>
    /// 
    [Serializable]
    public class DistanceLayer : Layer
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="DistanceLayer"/> class.
        /// </summary>
        /// 
        /// <param name="neuronsCount">Layer's neurons count.</param>
        /// <param name="inputsCount">Layer's inputs count.</param>
        /// 
        /// <remarks>The new layet is randomized (see <see cref="Neuron.Randomize"/>
        /// method) after it is created.</remarks>
        /// 
        public DistanceLayer( int neuronsCount, int inputsCount )
            : base( neuronsCount, inputsCount )
        {
            // create each neuron
            for ( int i = 0; i < neuronsCount; i++ )
                neurons[i] = new DistanceNeuron( inputsCount );
        }
    }
}
