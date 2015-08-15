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
    using System.Diagnostics;
    using AForge.Genetic;
    using AForge.Math.Random;

    /// <summary>
    /// Neural networks' evolutionary learning algorithm, which is based on Genetic Algorithms.
    /// </summary>
    /// 
    /// <remarks><para>The class implements supervised neural network's learning algorithm,
    /// which is based on Genetic Algorithms. For the given neural network, it create a population
    /// of <see cref="DoubleArrayChromosome"/> chromosomes, which represent neural network's
    /// weights. Then, during the learning process, the genetic population evolves and weights, which
    /// are represented by the best chromosome, are set to the source neural network.</para>
    /// 
    /// <para>See <see cref="Population"/> class for additional information about genetic population
    /// and evolutionary based search.</para>
    /// 
    /// <para>Sample usage (training network to calculate XOR function):</para>
    /// <code>
    /// // initialize input and output values
    /// double[][] input = new double[4][] {
    ///     new double[] {-1,  1}, new double[] {-1, 1},
    ///     new double[] { 1, -1}, new double[] { 1, 1}
    /// };
    /// double[][] output = new double[4][] {
    ///     new double[] {-1}, new double[] { 1},
    ///     new double[] { 1}, new double[] {-1}
    /// };
    /// // create neural network
    /// ActivationNetwork   network = new ActivationNetwork(
    ///     BipolarSigmoidFunction( 2 ),
    ///     2, // two inputs in the network
    ///     2, // two neurons in the first layer
    ///     1 ); // one neuron in the second layer
    /// // create teacher
    /// EvolutionaryLearning teacher = new EvolutionaryLearning( network,
    ///     100 ); // number of chromosomes in genetic population
    /// // loop
    /// while ( !needToStop )
    /// {
    ///     // run epoch of learning procedure
    ///     double error = teacher.RunEpoch( input, output );
    ///     // check error value to see if we need to stop
    ///     // ...
    /// }
    /// 
    /// </code>
    /// </remarks>
    /// 
    /// <seealso cref="BackPropagationLearning"/>
    /// 
    public class EvolutionaryLearning : ISupervisedLearning
    {
        // designed network for training which have to matach inputs and outputs
        private ActivationNetwork network;
        // number of weight in the network to train
        private int numberOfNetworksWeights;

        // genetic population
        private Population population;
        // size of population
        private int populationSize;

        // generator for newly generated neurons
        private IRandomNumberGenerator chromosomeGenerator;
        // mutation generators
        private IRandomNumberGenerator mutationMultiplierGenerator;
        private IRandomNumberGenerator mutationAdditionGenerator;

        // selection method for chromosomes in population
        private ISelectionMethod selectionMethod;

        // crossover probability in genetic population
        private double crossOverRate;
        // mutation probability in genetic population
        private double mutationRate;
        // probability to add newly generated chromosome to population
        private double randomSelectionRate;

        /// <summary>
        /// Initializes a new instance of the <see cref="EvolutionaryLearning"/> class.
        /// </summary>
        /// 
        /// <param name="activationNetwork">Activation network to be trained.</param>
        /// <param name="populationSize">Size of genetic population.</param>
        /// <param name="chromosomeGenerator">Random numbers generator used for initialization of genetic
        /// population representing neural network's weights and thresholds (see <see cref="DoubleArrayChromosome.chromosomeGenerator"/>).</param>
        /// <param name="mutationMultiplierGenerator">Random numbers generator used to generate random
        /// factors for multiplication of network's weights and thresholds during genetic mutation
        /// (ses <see cref="DoubleArrayChromosome.mutationMultiplierGenerator"/>.)</param>
        /// <param name="mutationAdditionGenerator">Random numbers generator used to generate random
        /// values added to neural network's weights and thresholds during genetic mutation
        /// (see <see cref="DoubleArrayChromosome.mutationAdditionGenerator"/>).</param>
        /// <param name="selectionMethod">Method of selection best chromosomes in genetic population.</param>
        /// <param name="crossOverRate">Crossover rate in genetic population (see
        /// <see cref="Population.CrossoverRate"/>).</param>
        /// <param name="mutationRate">Mutation rate in genetic population (see
        /// <see cref="Population.MutationRate"/>).</param>
        /// <param name="randomSelectionRate">Rate of injection of random chromosomes during selection
        /// in genetic population (see <see cref="Population.RandomSelectionPortion"/>).</param>
        /// 
        public EvolutionaryLearning( ActivationNetwork activationNetwork, int populationSize,
            IRandomNumberGenerator chromosomeGenerator,
            IRandomNumberGenerator mutationMultiplierGenerator,
            IRandomNumberGenerator mutationAdditionGenerator,
            ISelectionMethod selectionMethod,
            double crossOverRate, double mutationRate, double randomSelectionRate )
        {
            // Check of assumptions during debugging only
            Debug.Assert( activationNetwork != null );
            Debug.Assert( populationSize > 0 );
            Debug.Assert( chromosomeGenerator != null );
            Debug.Assert( mutationMultiplierGenerator != null );
            Debug.Assert( mutationAdditionGenerator != null );
            Debug.Assert( selectionMethod != null );
            Debug.Assert( crossOverRate >= 0.0 && crossOverRate <= 1.0 );
            Debug.Assert( mutationRate >= 0.0 && crossOverRate <= 1.0 );
            Debug.Assert( randomSelectionRate >= 0.0 && randomSelectionRate <= 1.0 );

            // networks's parameters
            this.network = activationNetwork;
            this.numberOfNetworksWeights = CalculateNetworkSize( activationNetwork );

            // population parameters
            this.populationSize = populationSize;
            this.chromosomeGenerator = chromosomeGenerator;
            this.mutationMultiplierGenerator = mutationMultiplierGenerator;
            this.mutationAdditionGenerator = mutationAdditionGenerator;
            this.selectionMethod = selectionMethod;
            this.crossOverRate = crossOverRate;
            this.mutationRate = mutationRate;
            this.randomSelectionRate = randomSelectionRate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EvolutionaryLearning"/> class.
        /// </summary>
        /// 
        /// <param name="activationNetwork">Activation network to be trained.</param>
        /// <param name="populationSize">Size of genetic population.</param>
        /// 
        /// <remarks><para>This version of constructor is used to create genetic population
        /// for searching optimal neural network's weight using default set of parameters, which are:
        /// <list type="bullet">
        /// <item>Selection method - elite;</item>
        /// <item>Crossover rate - 0.75;</item>
        /// <item>Mutation rate - 0.25;</item>
        /// <item>Rate of injection of random chromosomes during selection - 0.20;</item>
        /// <item>Random numbers generator for initializing new chromosome -
        /// <c>UniformGenerator( new Range( -1, 1 ) )</c>;</item>
        /// <item>Random numbers generator used during mutation for genes' multiplication -
        /// <c>ExponentialGenerator( 1 )</c>;</item>
        /// <item>Random numbers generator used during mutation for adding random value to genes -
        /// <c>UniformGenerator( new Range( -0.5f, 0.5f ) )</c>.</item>
        /// </list></para>
        /// 
        /// <para>In order to have full control over the above default parameters, it is possible to
        /// used extended version of constructor, which allows to specify all of the parameters.</para>
        /// </remarks>
        ///
        public EvolutionaryLearning( ActivationNetwork activationNetwork, int populationSize )
        {
            // Check of assumptions during debugging only
            Debug.Assert( activationNetwork != null );
            Debug.Assert( populationSize > 0 );

            // networks's parameters
            this.network = activationNetwork;
            this.numberOfNetworksWeights = CalculateNetworkSize( activationNetwork );

            // population parameters
            this.populationSize = populationSize;
            this.chromosomeGenerator = new UniformGenerator( new Range( -1, 1 ) );
            this.mutationMultiplierGenerator = new ExponentialGenerator( 1 );
            this.mutationAdditionGenerator = new UniformGenerator( new Range( -0.5f, 0.5f ) );
            this.selectionMethod = new EliteSelection( );
            this.crossOverRate = 0.75;
            this.mutationRate = 0.25;
            this.randomSelectionRate = 0.2;
        }

        // Create and initialize genetic population
        private int CalculateNetworkSize( ActivationNetwork activationNetwork )
        {
            // caclculate total amount of weight in neural network
            int networkSize = 0;

            for ( int i = 0; i < network.Layers.Length; i++ )
            {
                Layer layer = network.Layers[i];

                for ( int j = 0; j < layer.Neurons.Length; j++ )
                {
                    // sum all weights and threshold
                    networkSize += layer.Neurons[j].Weights.Length + 1;
                }
            }

            return networkSize;
        }

        /// <summary>
        /// Runs learning iteration.
        /// </summary>
        /// 
        /// <param name="input">Input vector.</param>
        /// <param name="output">Desired output vector.</param>
        /// 
        /// <returns>Returns learning error.</returns>
        /// 
        /// <remarks><note>The method is not implemented, since evolutionary learning algorithm is global
        /// and requires all inputs/outputs in order to run its one epoch. Use <see cref="RunEpoch"/>
        /// method instead.</note></remarks>
        /// 
        /// <exception cref="NotImplementedException">The method is not implemented by design.</exception>
        /// 
        public double Run( double[] input, double[] output )
        {
            throw new NotImplementedException( "The method is not implemented by design." );
        }

        /// <summary>
        /// Runs learning epoch.
        /// </summary>
        /// 
        /// <param name="input">Array of input vectors.</param>
        /// <param name="output">Array of output vectors.</param>
        /// 
        /// <returns>Returns summary squared learning error for the entire epoch.</returns>
        /// 
        /// <remarks><para><note>While running the neural network's learning process, it is required to
        /// pass the same <paramref name="input"/> and <paramref name="output"/> values for each
        /// epoch. On the very first run of the method it will initialize evolutionary fitness
        /// function with the given input/output. So, changing input/output in middle of the learning
        /// process, will break it.</note></para></remarks>
        ///
        public double RunEpoch( double[][] input, double[][] output )
        {
            Debug.Assert( input.Length > 0 );
            Debug.Assert( output.Length > 0 );
            Debug.Assert( input.Length == output.Length );
            Debug.Assert( network.InputsCount == input.Length );

            // check if it is a first run and create population if so
            if ( population == null )
            {
                // sample chromosome
                DoubleArrayChromosome chromosomeExample = new DoubleArrayChromosome(
                    chromosomeGenerator, mutationMultiplierGenerator, mutationAdditionGenerator,
                    numberOfNetworksWeights );

                // create population ...
                population = new Population( populationSize, chromosomeExample,
                    new EvolutionaryFitness( network, input, output ), selectionMethod );
                // ... and configure it
                population.CrossoverRate = crossOverRate;
                population.MutationRate = mutationRate;
                population.RandomSelectionPortion = randomSelectionRate;
            }

            // run genetic epoch
            population.RunEpoch( );

            // get best chromosome of the population
            DoubleArrayChromosome chromosome = (DoubleArrayChromosome) population.BestChromosome;
            double[] chromosomeGenes = chromosome.Value;

            // put best chromosome's value into neural network's weights
            int v = 0;

            for ( int i = 0; i < network.Layers.Length; i++ )
            {
                Layer layer = network.Layers[i];

                for ( int j = 0; j < layer.Neurons.Length; j++ )
                {
                    ActivationNeuron neuron = layer.Neurons[j] as ActivationNeuron;

                    for ( int k = 0; k < neuron.Weights.Length; k++ )
                    {
                        neuron.Weights[k] = chromosomeGenes[v++];
                    }
                    neuron.Threshold = chromosomeGenes[v++];
                }
            }

            Debug.Assert( v == numberOfNetworksWeights );

            return 1.0 / chromosome.Fitness;
        }
    }
}
