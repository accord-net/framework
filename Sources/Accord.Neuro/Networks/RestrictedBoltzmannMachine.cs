// Accord Neural Net Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
// cesarsouza at gmail.com
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

namespace Accord.Neuro.Networks
{
    using System;
    using Accord.Neuro.ActivationFunctions;
    using Accord.Neuro.Layers;
    using Accord.Neuro.Neurons;
    using AForge.Neuro;

    /// <summary>
    ///   Restricted Boltzmann Machine.
    /// </summary>
    /// 
    /// <example>
    ///   <code>
    ///   // Create some sample inputs and outputs. Note that the
    ///   // first four vectors belong to one class, and the other
    ///   // four belong to another (you should see that the 1s
    ///   // accumulate on the beginning for the first four vectors
    ///   // and on the end for the second four).
    ///   
    ///   double[][] inputs =
    ///   {
    ///       new double[] { 1,1,1, 0,0,0 }, // class a
    ///       new double[] { 1,0,1, 0,0,0 }, // class a
    ///       new double[] { 1,1,1, 0,0,0 }, // class a
    ///       new double[] { 0,0,1, 1,1,0 }, // class b
    ///       new double[] { 0,0,1, 1,0,0 }, // class b
    ///       new double[] { 0,0,1, 1,1,0 }, // class b
    ///   };
    ///   
    ///   double[][] outputs =
    ///   {
    ///       new double[] { 1, 0 }, // indicates the inputs at this
    ///       new double[] { 1, 0 }, // position belongs to class a
    ///       new double[] { 1, 0 },
    ///       new double[] { 0, 1 }, // indicates the inputs at this
    ///       new double[] { 0, 1 }, // position belongs to class b
    ///       new double[] { 0, 1 },
    ///   };
    ///   
    ///   // Create a Bernoulli activation function
    ///   var function = new BernoulliFunction(alpha: 0.5);
    ///   
    ///   // Create a Restricted Boltzmann Machine for 6 inputs and with 1 hidden neuron
    ///   var rbm = new RestrictedBoltzmannMachine(function, inputsCount: 6, hiddenNeurons: 2);
    ///   
    ///   // Create the learning algorithm for RBMs
    ///   var teacher = new ContrastiveDivergenceLearning(rbm)
    ///   {
    ///       Momentum = 0,
    ///       LearningRate = 0.1,
    ///       Decay = 0
    ///   };
    ///   
    ///   // learn 5000 iterations
    ///   for (int i = 0; i &lt; 5000; i++)
    ///       teacher.RunEpoch(inputs);
    ///   
    ///   // Compute the machine answers for the given inputs:
    ///   double[] a = rbm.Compute(new double[] { 1, 1, 1, 0, 0, 0 }); // { 0.99, 0.00 }
    ///   double[] b = rbm.Compute(new double[] { 0, 0, 0, 1, 1, 1 }); // { 0.00, 0.99 }
    ///   
    ///   // As we can see, the first neuron responds to vectors belonging
    ///   // to the first class, firing 0.99 when we feed vectors which 
    ///   // have 1s at the beginning. Likewise, the second neuron fires 
    ///   // when the vector belongs to the second class.
    ///   
    ///   // We can also generate input vectors given the classes:
    ///   double[] xa = rbm.GenerateInput(new double[] { 1, 0 }); // { 1, 1, 1, 0, 0, 0 }
    ///   double[] xb = rbm.GenerateInput(new double[] { 0, 1 }); // { 0, 0, 1, 1, 1, 0 }
    ///   
    ///   // As we can see, if we feed an output pattern where the first neuron
    ///   // is firing and the second isn't, the network generates an example of
    ///   // a vector belonging to the first class. The same goes for the second
    ///   // neuron and the second class.
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="Accord.Neuro.Learning.ContrastiveDivergenceLearning"/>
    /// <seealso cref="BernoulliFunction"/>
    /// 
    [Serializable]
    public class RestrictedBoltzmannMachine : ActivationNetwork
    {

        private StochasticLayer visible;
        private StochasticLayer hidden;

        /// <summary>
        ///   Gets the visible layer of the machine.
        /// </summary>
        /// 
        public StochasticLayer Visible
        {
            get { return visible; }
        }

        /// <summary>
        ///   Gets the hidden layer of the machine.
        /// </summary>
        /// 
        public StochasticLayer Hidden
        {
            get { return hidden; }
        }


        /// <summary>
        ///   Creates a new <see cref="RestrictedBoltzmannMachine"/>.
        /// </summary>
        /// 
        /// <param name="inputsCount">The number of inputs for the machine.</param>
        /// <param name="hiddenNeurons">The number of hidden neurons in the machine.</param>
        /// 
        public RestrictedBoltzmannMachine(int inputsCount, int hiddenNeurons)
            : this(new BernoulliFunction(alpha: 1), inputsCount, hiddenNeurons) { }

        /// <summary>
        ///   Creates a new <see cref="RestrictedBoltzmannMachine"/>.
        /// </summary>
        /// 
        /// <param name="hidden">The hidden layer to be added in the machine.</param>
        /// <param name="visible">The visible layer to be added in the machine.</param>
        /// 
        public RestrictedBoltzmannMachine(StochasticLayer hidden, StochasticLayer visible)
            : base(null, hidden.InputsCount, 0)
        {
            this.hidden = hidden;
            this.visible = visible;

            base.layers[0] = hidden;
        }

        /// <summary>
        ///   Creates a new <see cref="RestrictedBoltzmannMachine"/>.
        /// </summary>
        /// 
        /// <param name="function">The activation function to use in the network neurons.</param>
        /// <param name="inputsCount">The number of inputs for the machine.</param>
        /// <param name="hiddenNeurons">The number of hidden neurons in the machine.</param>
        /// 
        public RestrictedBoltzmannMachine(IStochasticFunction function, int inputsCount, int hiddenNeurons)
            : base(function, inputsCount, 1)
        {
            this.visible = new StochasticLayer(function, inputsCount, hiddenNeurons);
            this.hidden = new StochasticLayer(function, hiddenNeurons, inputsCount);

            base.layers[0] = hidden;
        }


        /// <summary>
        ///   Compute output vector of the network.
        /// </summary>
        /// 
        /// <param name="input">Input vector.</param>
        /// 
        /// <returns>
        ///   Returns network's output vector.
        /// </returns>
        /// 
        public override double[] Compute(double[] input)
        {
            return hidden.Compute(input);
        }

        /// <summary>
        ///   Reconstructs a input vector for a given output.
        /// </summary>
        /// 
        /// <param name="output">The output vector.</param>
        /// 
        /// <returns>
        ///   Returns a probable input vector which may 
        ///   have originated the given output.
        /// </returns>
        /// 
        public double[] Reconstruct(double[] output)
        {
            return visible.Compute(output);
        }

        /// <summary>
        ///   Samples an output vector from the network
        ///   given an input vector.
        /// </summary>
        /// 
        /// <param name="input">An input vector.</param>
        /// 
        /// <returns>
        ///   A possible output considering the
        ///   stochastic activations of the network.
        /// </returns>
        /// 
        public double[] GenerateOutput(double[] input)
        {
            return hidden.Generate(input);
        }

        /// <summary>
        ///   Samples an input vector from the network
        ///   given an output vector.
        /// </summary>
        /// 
        /// <param name="output">An output vector.</param>
        /// 
        /// <returns>
        ///   A possible reconstruction considering the
        ///   stochastic activations of the network.
        /// </returns>
        /// 
        public double[] GenerateInput(double[] output)
        {
            return visible.Generate(output);
        }


        /// <summary>
        ///   Constructs a Gaussian-Bernoulli network with 
        ///   visible Gaussian units and hidden Bernoulli units.
        /// </summary>
        /// 
        /// <param name="inputsCount">The number of inputs for the machine.</param>
        /// <param name="hiddenNeurons">The number of hidden neurons in the machine.</param>
        /// 
        /// <returns>A Gaussian-Bernoulli Restricted Boltzmann Machine</returns>
        /// 
        public static RestrictedBoltzmannMachine CreateGaussianBernoulli(int inputsCount, int hiddenNeurons)
        {
            RestrictedBoltzmannMachine network = new RestrictedBoltzmannMachine(inputsCount, hiddenNeurons);

            foreach (var neuron in network.Visible.Neurons)
                neuron.ActivationFunction = new GaussianFunction();

            return network;
        }

        /// <summary>
        ///   Creates a new <see cref="ActivationNetwork"/> from this instance.
        /// </summary>
        /// 
        /// <param name="outputs">The number of output neurons in the last layer.</param>
        /// 
        /// <returns>An <see cref="ActivationNetwork"/> containing this network.</returns>
        /// 
        public ActivationNetwork ToActivationNetwork(int outputs)
        {
            return ToActivationNetwork(new SigmoidFunction(alpha: 1), outputs);
        }

        /// <summary>
        ///   Creates a new <see cref="ActivationNetwork"/> from this instance.
        /// </summary>
        /// 
        /// <param name="outputs">The number of output neurons in the last layer.</param>
        /// <param name="function">The activation function to use in the last layer.</param>
        /// 
        /// <returns>An <see cref="ActivationNetwork"/> containing this network.</returns>
        /// 
        public ActivationNetwork ToActivationNetwork(IActivationFunction function, int outputs)
        {
            ActivationNetwork ann = new ActivationNetwork(function,
                inputsCount, hidden.Neurons.Length, outputs);

            // For each neuron
            for (int i = 0; i < hidden.Neurons.Length; i++)
            {
                ActivationNeuron aneuron = ann.Layers[0].Neurons[i] as ActivationNeuron;
                StochasticNeuron sneuron = hidden.Neurons[i];

                // For each weight
                for (int j = 0; j < sneuron.Weights.Length; j++)
                    aneuron.Weights[j] = sneuron.Weights[j];
                aneuron.Threshold = sneuron.Threshold;
                aneuron.ActivationFunction = sneuron.ActivationFunction;
            }

            return ann;
        }

        /// <summary>
        ///   Updates the weights of the visible layer by copying
        ///   the reverse of the weights in the hidden layer.
        /// </summary>
        /// 
        public void UpdateVisibleWeights()
        {
            Visible.CopyReversedWeightsFrom(Hidden);
        }
    }
}
