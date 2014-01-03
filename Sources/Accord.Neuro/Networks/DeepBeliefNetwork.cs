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
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Accord.Neuro.ActivationFunctions;
    using Accord.Neuro.Layers;
    using AForge.Neuro;
    using Accord.Neuro.Neurons;

    /// <summary>
    ///   Deep Belief Network.
    /// </summary>
    /// 
    /// <remarks>
    ///   The Deep Belief Network can be seen as a collection of stacked
    ///   <see cref="RestrictedBoltzmannMachine">Restricted Boltzmann
    ///   Machines</see> disposed as layers of a network. In turn, the
    ///   whole network can be seen as an stochastic activation network
    ///   in which the neurons activate within some given probability.
    /// </remarks>
    /// 
    [Serializable]
    public class DeepBeliefNetwork : ActivationNetwork
    {

        private List<RestrictedBoltzmannMachine> machines;

        /// <summary>
        ///   Gets the number of output neurons in the network
        ///   (the size of the computed output vectors).
        /// </summary>
        /// 
        public int OutputCount
        {
            get
            {
                if (machines.Count == 0) return 0;
                return machines[machines.Count - 1].Hidden.Neurons.Length;
            }
        }

        /// <summary>
        ///   Gets the Restricted Boltzmann Machines
        ///   on each layer of this deep network.
        /// </summary>
        /// 
        public IList<RestrictedBoltzmannMachine> Machines
        {
            get { return machines; }
        }

        /// <summary>
        ///   Creates a new <see cref="DeepBeliefNetwork"/>.
        /// </summary>
        /// 
        /// <param name="inputsCount">The number of inputs for the network.</param>
        /// <param name="hiddenNeurons">The number of hidden neurons in each layer.</param>
        /// 
        public DeepBeliefNetwork(int inputsCount, params int[] hiddenNeurons)
            : this(new BernoulliFunction(alpha: 1), inputsCount, hiddenNeurons) { }


        /// <summary>
        ///   Creates a new <see cref="DeepBeliefNetwork"/>.
        /// </summary>
        /// 
        /// <param name="function">The activation function to be used in the network neurons.</param>
        /// <param name="inputsCount">The number of inputs for the network.</param>
        /// <param name="hiddenNeurons">The number of hidden neurons in each layer.</param>
        /// 
        public DeepBeliefNetwork(IStochasticFunction function, int inputsCount, params int[] hiddenNeurons)
            : base(function, inputsCount, hiddenNeurons)
        {
            machines = new List<RestrictedBoltzmannMachine>();

            // Create first layer
            machines.Add(new RestrictedBoltzmannMachine(
                hidden: new StochasticLayer(function, hiddenNeurons[0], inputsCount),
                visible: new StochasticLayer(function, inputsCount, hiddenNeurons[0]))
            );

            // Create other layers
            for (int i = 1; i < hiddenNeurons.Length; i++)
            {
                machines.Add(new RestrictedBoltzmannMachine(
                    hidden: new StochasticLayer(function, hiddenNeurons[i], hiddenNeurons[i - 1]),
                    visible: new StochasticLayer(function, hiddenNeurons[i - 1], hiddenNeurons[i])));
            }

            // Override AForge layers
            layers = new Layer[machines.Count];
            for (int i = 0; i < layers.Length; i++)
                layers[i] = machines[i].Hidden;
        }

        /// <summary>
        ///   Creates a new <see cref="DeepBeliefNetwork"/>.
        /// </summary>
        /// 
        /// <param name="inputsCount">The number of inputs for the network.</param>
        /// <param name="layers">The layers to add to the deep network.</param>
        /// 
        public DeepBeliefNetwork(int inputsCount, params RestrictedBoltzmannMachine[] layers)
            : base(null, inputsCount, new int[layers.Length])
        {
            machines = new List<RestrictedBoltzmannMachine>(layers);

            // Override AForge layers
            base.layers = new Layer[machines.Count];
            for (int i = 0; i < layers.Length; i++)
                base.layers[i] = machines[i].Hidden;
        }

        /// <summary>
        ///   Computes the network's outputs for a given input.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// 
        /// <returns>
        ///   Returns the network's output for the given input.
        /// </returns>
        /// 
        public override double[] Compute(double[] input)
        {
            double[] output = input;

            foreach (RestrictedBoltzmannMachine layer in machines)
                output = layer.Hidden.Compute(output);

            return output;
        }

        /// <summary>
        ///   Computes the network's outputs for a given input.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// <param name="layerIndex">The index of the layer.</param>
        /// 
        /// <returns>
        ///   Returns the network's output for the given input.
        /// </returns>
        /// 
        public double[] Compute(double[] input, int layerIndex)
        {
            double[] output = input;

            for (int i = 0; i <= layerIndex; i++)
                output = machines[i].Hidden.Compute(output);

            return output;
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
            double[] input = output;

            for (int i = machines.Count - 1; i >= 0; i--)
                input = machines[i].Visible.Compute(input);

            return input;
        }

        /// <summary>
        ///   Reconstructs a input vector using the output
        ///   vector of a given layer.
        /// </summary>
        /// 
        /// <param name="output">The output vector.</param>
        /// <param name="layerIndex">The index of the layer.</param>
        /// 
        /// <returns>
        ///   Returns a probable input vector which may 
        ///   have originated the given output in the 
        ///   indicated layer.
        /// </returns>
        /// 
        public double[] Reconstruct(double[] output, int layerIndex)
        {
            double[] input = output;

            for (int i = layerIndex; i >= 0; i--)
                input = machines[i].Visible.Compute(input);

            return input;
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
            double[] output = input;

            foreach (RestrictedBoltzmannMachine layer in machines)
                output = layer.Hidden.Generate(output);

            return output;
        }

        /// <summary>
        ///   Samples an output vector from the network
        ///   given an input vector.
        /// </summary>
        /// 
        /// <param name="input">An input vector.</param>
        /// <param name="layerIndex">The index of the layer.</param>
        /// 
        /// <returns>
        ///   A possible output considering the
        ///   stochastic activations of the network.
        /// </returns>
        /// 
        public double[] GenerateOutput(double[] input, int layerIndex)
        {
            double[] output = input;

            for (int i = 0; i <= layerIndex; i++)
                output = machines[i].Hidden.Generate(output);

            return output;
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
            double[] input = output;

            for (int i = layers.Length - 1; i >= 0; i--)
                input = machines[i].Visible.Generate(input);

            return input;
        }


        /// <summary>
        ///   Inserts a new layer at the end of this network.
        /// </summary>
        /// 
        /// <param name="neurons">The number of neurons in the new layer.</param>
        /// 
        public void Push(int neurons)
        {
            Push(neurons, new BernoulliFunction(alpha: 1));
        }

        /// <summary>
        ///   Inserts a new layer at the end of this network.
        /// </summary>
        /// 
        /// <param name="neurons">The number of neurons in the new layer.</param>
        /// <param name="function">The activation function which should be used by the neurons.</param>
        /// 
        public void Push(int neurons, IStochasticFunction function)
        {
            Push(neurons, function, function);
        }

        /// <summary>
        ///   Inserts a new layer at the end of this network.
        /// </summary>
        /// 
        /// <param name="neurons">The number of neurons in the layer.</param>
        /// <param name="visibleFunction">The activation function which should be used by the visible neurons.</param>
        /// <param name="hiddenFunction">The activation function which should be used by the hidden neurons.</param>
        /// 
        public void Push(int neurons, IStochasticFunction visibleFunction, IStochasticFunction hiddenFunction)
        {
            int lastLayerNeurons;

            if (machines.Count > 0)
                lastLayerNeurons = machines[machines.Count - 1].Hidden.Neurons.Length;
            else lastLayerNeurons = inputsCount;

            machines.Add(new RestrictedBoltzmannMachine(
                hidden: new StochasticLayer(hiddenFunction, neurons, lastLayerNeurons),
                visible: new StochasticLayer(visibleFunction, lastLayerNeurons, neurons)));

            // Override AForge layers
            layers = new Layer[machines.Count];
            for (int i = 0; i < layers.Length; i++)
                layers[i] = machines[i].Hidden;
        }

        /// <summary>
        ///   Stacks a new Boltzmann Machine at the end of this network.
        /// </summary>
        /// 
        /// <param name="network">The machine to be added to the network.</param>
        /// 
        public void Push(RestrictedBoltzmannMachine network)
        {
            int lastLayerNeurons;

            if (machines.Count > 0)
                lastLayerNeurons = machines[machines.Count - 1].Hidden.Neurons.Length;
            else lastLayerNeurons = inputsCount;

            machines.Add(network);

            // Override AForge layers
            layers = new Layer[machines.Count];
            for (int i = 0; i < layers.Length; i++)
                layers[i] = machines[i].Hidden;
        }

        /// <summary>
        ///   Removes the last layer from the network.
        /// </summary>
        /// 
        public void Pop()
        {
            if (machines.Count == 0)
                return;

            machines.RemoveAt(machines.Count - 1);

            // Override AForge layers
            layers = new Layer[machines.Count];
            for (int i = 0; i < layers.Length; i++)
                layers[i] = machines[i].Hidden;
        }

        /// <summary>
        ///   Updates the weights of the visible layers by copying
        ///   the reverse of the weights in the hidden layers.
        /// </summary>
        /// 
        public void UpdateVisibleWeights()
        {
            foreach (var machine in machines)
                machine.UpdateVisibleWeights();
        }

        /// <summary>
        ///   Creates a Gaussian-Bernoulli network.
        /// </summary>
        /// 
        /// <param name="inputsCount">The number of inputs for the network.</param>
        /// <param name="hiddenNeurons">The number of hidden neurons in each layer.</param>
        /// 
        public static DeepBeliefNetwork CreateGaussianBernoulli(int inputsCount, params int[] hiddenNeurons)
        {
            DeepBeliefNetwork network = new DeepBeliefNetwork(inputsCount, hiddenNeurons);

            GaussianFunction gaussian = new GaussianFunction();
            foreach (StochasticNeuron neuron in network.machines[0].Visible.Neurons)
                neuron.ActivationFunction = gaussian;

            return network;
        }

        /// <summary>
        ///   Creates a Mixed-Bernoulli network.
        /// </summary>
        /// 
        /// <param name="visible">The <see cref="IStochasticFunction"/> to be used in the first visible layer.</param>
        /// <param name="hidden">The <see cref="IStochasticFunction"/> to be used in all other layers.</param>
        /// 
        /// <param name="inputsCount">The number of inputs for the network.</param>
        /// <param name="hiddenNeurons">The number of hidden neurons in each layer.</param>
        /// 
        public static DeepBeliefNetwork CreateMixedNetwork(IStochasticFunction visible,
            IStochasticFunction hidden, int inputsCount, params int[] hiddenNeurons)
        {
            DeepBeliefNetwork network = new DeepBeliefNetwork(hidden, inputsCount, hiddenNeurons);

            foreach (StochasticNeuron neuron in network.machines[0].Visible.Neurons)
                neuron.ActivationFunction = visible;

            return network;
        }

        /// <summary>
        ///   Saves the network to a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream to which the network is to be serialized.</param>
        /// 
        public new void Save(Stream stream)
        {
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(stream, this);
        }

        /// <summary>
        ///   Saves the network to a stream.
        /// </summary>
        /// 
        /// <param name="path">The file path to which the network is to be serialized.</param>
        /// 
        public new void Save(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                Save(fs);
            }
        }

        /// <summary>
        ///   Loads a network from a stream.
        /// </summary>
        /// 
        /// <param name="stream">The network from which the machine is to be deserialized.</param>
        /// 
        /// <returns>The deserialized network.</returns>
        /// 
        public static new DeepBeliefNetwork Load(Stream stream)
        {
            BinaryFormatter b = new BinaryFormatter();
            return (DeepBeliefNetwork)b.Deserialize(stream);
        }

        /// <summary>
        ///   Loads a network from a file.
        /// </summary>
        /// 
        /// <param name="path">The path to the file from which the network is to be deserialized.</param>
        /// 
        /// <returns>The deserialized network.</returns>
        /// 
        public static new DeepBeliefNetwork Load(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                return Load(fs);
            }
        }

    }
}
