// AForge Neural Net Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2012
// contacts@aforgenet.com
//

namespace Accord.Neuro
{
    using System;
    using AForge;
    using Accord;
    using Accord.Math.Random;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Compat;

    /// <summary>
    /// Base neuron class.
    /// </summary>
    /// 
    /// <remarks>This is a base neuron class, which encapsulates such
    /// common properties, like neuron's input, output and weights.</remarks>
    /// 
    [Serializable]
    public abstract class Neuron
    {
        /// <summary>
        /// Neuron's inputs count.
        /// </summary>
        protected int inputsCount = 0;

        /// <summary>
        /// Neuron's weights.
        /// </summary>
        protected double[] weights = null;

        /// <summary>
        /// Neuron's output value.
        /// </summary>
        protected double output = 0;

        /// <summary>
        /// Random number generator.
        /// </summary>
        /// 
        /// <remarks>The generator is used for neuron's weights randomization.</remarks>
        /// 
        protected IRandomNumberGenerator<double> rand =
            new UniformContinuousDistribution();

        /// <summary>
        /// Random number generator.
        /// </summary>
        /// 
        /// <remarks>The property allows to initialize random generator with a custom seed. The generator is
        /// used for neuron's weights randomization.</remarks>
        /// 
        public IRandomNumberGenerator<double> RandGenerator
        {
            get { return rand; }
            set { rand = value; }
        }


        /// <summary>
        /// Neuron's inputs count.
        /// </summary>
        public int InputsCount
        {
            get { return inputsCount; }
        }

        /// <summary>
        /// Neuron's output value.
        /// </summary>
        /// 
        /// <remarks>The calculation way of neuron's output value is determined by inherited class.</remarks>
        /// 
        public double Output
        {
            get { return output; }
        }


        /// <summary>
        /// Neuron's weights.
        /// </summary>
        public double[] Weights
        {
            get { return weights; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Neuron"/> class.
        /// </summary>
        ///
        /// <param name="inputs">Neuron's inputs count.</param>
        /// 
        /// <remarks>The new neuron will be randomized (see <see cref="Randomize"/> method)
        /// after it is created.</remarks>
        ///
        protected Neuron(int inputs)
        {
            // allocate weights
            inputsCount = Math.Max(1, inputs);
            weights = new double[inputsCount];

            // randomize the neuron
            Randomize();
        }

        /// <summary>
        /// Randomize neuron.
        /// </summary>
        /// 
        /// <remarks>
        ///   Initialize neuron's weights with random values specified
        ///   by the <see cref="RandGenerator"/>.</remarks>
        /// 
        public virtual void Randomize()
        {
            rand.Generate(weights.Length, weights);
        }

        /// <summary>
        /// Computes output value of neuron.
        /// </summary>
        /// 
        /// <param name="input">Input vector.</param>
        /// 
        /// <returns>Returns neuron's output value.</returns>
        /// 
        /// <remarks>The actual neuron's output value is determined by inherited class.
        /// The output value is also stored in <see cref="Output"/> property.</remarks>
        /// 
        public abstract double Compute(double[] input);
    }
}
