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
    /// Elastic network learning algorithm.
    /// </summary>
    ///
    /// <remarks><para>This class implements elastic network's learning algorithm and
    /// allows to train <see cref="DistanceNetwork">Distance Networks</see>.</para>
    /// </remarks> 
    ///
    public class ElasticNetworkLearning : IUnsupervisedLearning
    {
        // neural network to train
        private DistanceNetwork network;

        // array of distances between neurons
        private double[] distance;

        // learning rate
        private double learningRate = 0.1;
        // learning radius
        private double learningRadius = 0.5;

        // squared learning radius multiplied by 2 (precalculated value to speed up computations)
        private double squaredRadius2 = 2 * 0.5 *  0.5;

        /// <summary>
        /// Learning rate, [0, 1].
        /// </summary>
        /// 
        /// <remarks><para>Determines speed of learning.</para>
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
        /// Learning radius, [0, 1].
        /// </summary>
        /// 
        /// <remarks><para>Determines the amount of neurons to be updated around
        /// winner neuron. Neurons, which are in the circle of specified radius,
        /// are updated during the learning procedure. Neurons, which are closer
        /// to the winner neuron, get more update.</para>
        /// 
        /// <para>Default value equals to <b>0.5</b>.</para>
        /// </remarks>
        /// 
        public double LearningRadius
        {
            get { return learningRadius; }
            set
            {
                learningRadius = Math.Max( 0, Math.Min( 1.0, value ) );
                squaredRadius2 = 2 * learningRadius * learningRadius;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ElasticNetworkLearning"/> class.
        /// </summary>
        /// 
        /// <param name="network">Neural network to train.</param>
        /// 
        public ElasticNetworkLearning( DistanceNetwork network )
        {
            this.network = network;

            // precalculate distances array
            int neurons = network.Layers[0].Neurons.Length;
            double deltaAlpha = Math.PI * 2.0 / neurons;
            double alpha = deltaAlpha;

            distance = new double[neurons];
            distance[0] = 0.0;

            // calculate all distance values
            for ( int i = 1; i < neurons; i++ )
            {
                double dx = 0.5 * Math.Cos( alpha ) - 0.5;
                double dy = 0.5 * Math.Sin( alpha );

                distance[i] = dx * dx + dy * dy;

                alpha += deltaAlpha;
            }
        }

        /// <summary>
        /// Runs learning iteration.
        /// </summary>
        /// 
        /// <param name="input">Input vector.</param>
        /// 
        /// <returns>Returns learning error - summary absolute difference between neurons'
        /// weights and appropriate inputs. The difference is measured according to the neurons
        /// distance to the winner neuron.</returns>
        /// 
        /// <remarks><para>The method runs one learning iterations - finds winner neuron (the neuron
        /// which has weights with values closest to the specified input vector) and updates its weight
        /// (as well as weights of neighbor neurons) in the way to decrease difference with the specified
        /// input vector.</para></remarks>
        /// 
        public double Run( double[] input )
        {
            double error = 0.0;

            // compute the network
            network.Compute( input );
            int winner = network.GetWinner( );

            // get layer of the network
            Layer layer = network.Layers[0];

            // walk through all neurons of the layer
            for ( int j = 0; j < layer.Neurons.Length; j++ )
            {
                Neuron neuron = layer.Neurons[j];

                // update factor
                double factor = Math.Exp( -distance[Math.Abs( j - winner )] / squaredRadius2 );

                // update weights of the neuron
                for ( int i = 0; i < neuron.Weights.Length; i++ )
                {
                    // calculate the error
                    double e = ( input[i] - neuron.Weights[i] ) * factor;
                    error += Math.Abs( e );
                    // update weight
                    neuron.Weights[i] += e * learningRate;
                }
            }
            return error;
        }

        /// <summary>
        /// Runs learning epoch.
        /// </summary>
        /// 
        /// <param name="input">Array of input vectors.</param>
        /// 
        /// <returns>Returns summary learning error for the epoch. See <see cref="Run"/>
        /// method for details about learning error calculation.</returns>
        /// 
        /// <remarks><para>The method runs one learning epoch, by calling <see cref="Run"/> method
        /// for each vector provided in the <paramref name="input"/> array.</para></remarks>
        /// 
        public double RunEpoch( double[][] input )
        {
            double error = 0.0;

            // walk through all training samples
            foreach ( double[] sample in input )
            {
                error += Run( sample );
            }

            // return summary error
            return error;
        }
    }
}
