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
    /// Distance network.
    /// </summary>
    ///
    /// <remarks>Distance network is a neural network of only one <see cref="DistanceLayer">distance
    /// layer</see>. The network is a base for such neural networks as SOM, Elastic net, etc.
    /// </remarks>
    ///
    [Serializable]
    public class DistanceNetwork : Network
    {


        /// <summary>
        /// Initializes a new instance of the <see cref="DistanceNetwork"/> class.
        /// </summary>
        /// 
        /// <param name="inputsCount">Network's inputs count.</param>
        /// <param name="neuronsCount">Network's neurons count.</param>
        /// 
        /// <remarks>The new network is randomized (see <see cref="Neuron.Randomize"/>
        /// method) after it is created.</remarks>
        /// 
        public DistanceNetwork( int inputsCount, int neuronsCount )
            : base( inputsCount, 1 )
        {
            // create layer
            layers[0] = new DistanceLayer( neuronsCount, inputsCount );
        }

        /// <summary>
        /// Get winner neuron.
        /// </summary>
        /// 
        /// <returns>Index of the winner neuron.</returns>
        /// 
        /// <remarks>The method returns index of the neuron, which weights have
        /// the minimum distance from network's input.</remarks>
        /// 
        public int GetWinner( )
        {
            // find the MIN value
            double min = output[0];
            int    minIndex = 0;

            for ( int i = 1; i < output.Length; i++ )
            {
                if ( output[i] < min )
                {
                    // found new MIN value
                    min = output[i];
                    minIndex = i;
                }
            }

            return minIndex;
        }
    }
}
