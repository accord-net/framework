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
    /// Kohonen Self Organizing Map (SOM) learning algorithm.
    /// </summary>
    /// 
    /// <remarks><para>This class implements Kohonen's SOM learning algorithm and
    /// is widely used in clusterization tasks. The class allows to train
    /// <see cref="DistanceNetwork">Distance Networks</see>.</para>
    /// 
    /// <para>Sample usage (clustering RGB colors):</para>
    /// <code>
    /// // set range for randomization neurons' weights
    /// Neuron.RandRange = new Range( 0, 255 );
    /// // create network
    /// DistanceNetwork	network = new DistanceNetwork(
    ///         3, // thress inputs in the network
    ///         100 * 100 ); // 10000 neurons
    /// // create learning algorithm
    /// SOMLearning	trainer = new SOMLearning( network );
    /// // network's input
    /// double[] input = new double[3];
    /// // loop
    /// while ( !needToStop )
    /// {
    ///     input[0] = rand.Next( 256 );
    ///     input[1] = rand.Next( 256 );
    ///     input[2] = rand.Next( 256 );
    /// 
    ///     trainer.Run( input );
    /// 
    ///     // ...
    ///     // update learning rate and radius continuously,
    ///     // so networks may come steady state
    /// }
    /// </code>
    /// </remarks>
    /// 
    public class SOMLearning : IUnsupervisedLearning
    {
        // neural network to train
        private DistanceNetwork network;
        // network's dimension
        private int width;
        private int height;

        // learning rate
        private double learningRate = 0.1;
        // learning radius
        private double learningRadius = 7;

        // squared learning radius multiplied by 2 (precalculated value to speed up computations)
        private double squaredRadius2 = 2 * 7 * 7;

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
        /// Learning radius.
        /// </summary>
        /// 
        /// <remarks><para>Determines the amount of neurons to be updated around
        /// winner neuron. Neurons, which are in the circle of specified radius,
        /// are updated during the learning procedure. Neurons, which are closer
        /// to the winner neuron, get more update.</para>
        /// 
        /// <para><note>In the case if learning rate is set to 0, then only winner
        /// neuron's weights are updated.</note></para>
        /// 
        /// <para>Default value equals to <b>7</b>.</para>
        /// </remarks>
        /// 
        public double LearningRadius
        {
            get { return learningRadius; }
            set
            {
                learningRadius = Math.Max( 0, value );
                squaredRadius2 = 2 * learningRadius * learningRadius;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SOMLearning"/> class.
        /// </summary>
        /// 
        /// <param name="network">Neural network to train.</param>
        /// 
        /// <remarks><para>This constructor supposes that a square network will be passed for training -
        /// it should be possible to get square root of network's neurons amount.</para></remarks>
        /// 
        /// <exception cref="ArgumentException">Invalid network size - square network is expected.</exception>
        /// 
        public SOMLearning( DistanceNetwork network )
        {
            // network's dimension was not specified, let's try to guess
            int neuronsCount = network.Layers[0].Neurons.Length;
            int width = (int) Math.Sqrt( neuronsCount );

            if ( width * width != neuronsCount )
            {
                throw new ArgumentException( "Invalid network size." );
            }

            // ok, we got it
            this.network = network;
            this.width = width;
            this.height = width;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="SOMLearning"/> class.
        /// </summary>
        /// 
        /// <param name="network">Neural network to train.</param>
        /// <param name="width">Neural network's width.</param>
        /// <param name="height">Neural network's height.</param>
        ///
        /// <remarks>The constructor allows to pass network of arbitrary rectangular shape.
        /// The amount of neurons in the network should be equal to <b>width</b> * <b>height</b>.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">Invalid network size - network size does not correspond
        /// to specified width and height.</exception>
        /// 
        public SOMLearning( DistanceNetwork network, int width, int height )
        {
            // check network size
            if ( network.Layers[0].Neurons.Length != width * height )
            {
                throw new ArgumentException( "Invalid network size." );
            }

            this.network = network;
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// Runs learning iteration.
        /// </summary>
        /// 
        /// <param name="input">Input vector.</param>
        /// 
        /// <returns>Returns learning error - summary absolute difference between neurons' weights
        /// and appropriate inputs. The difference is measured according to the neurons
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

            // check learning radius
            if ( learningRadius == 0 )
            {
                Neuron neuron = layer.Neurons[winner];

                // update weight of the winner only
                for ( int i = 0; i < neuron.Weights.Length; i++ )
                {
                    // calculate the error
                    double e = input[i] - neuron.Weights[i];
                    error += Math.Abs( e );
                    // update weights
                    neuron.Weights[i] += e * learningRate;
                }
            }
            else
            {
                // winner's X and Y
                int wx = winner % width;
                int wy = winner / width;

                // walk through all neurons of the layer
                for ( int j = 0; j < layer.Neurons.Length; j++ )
                {
                    Neuron neuron = layer.Neurons[j];

                    int dx = ( j % width ) - wx;
                    int dy = ( j / width ) - wy;

                    // update factor ( Gaussian based )
                    double factor = Math.Exp( -(double) ( dx * dx + dy * dy ) / squaredRadius2 );

                    // update weight of the neuron
                    for ( int i = 0; i < neuron.Weights.Length; i++ )
                    {
                        // calculate the error
                        double e = ( input[i] - neuron.Weights[i] ) * factor;
                        error += Math.Abs( e );
                        // update weight
                        neuron.Weights[i] += e * learningRate;
                    }
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
