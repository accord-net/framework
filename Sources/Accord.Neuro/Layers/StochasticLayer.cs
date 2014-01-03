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

namespace Accord.Neuro.Layers
{
    using System;
    using Accord.Neuro.ActivationFunctions;
    using Accord.Neuro.Neurons;
    using AForge.Neuro;

    /// <summary>
    ///   Stochastic Activation Layer.
    /// </summary>
    /// 
    /// <remarks>
    ///   This class represents a layer of <see cref="StochasticNeuron">stochastic neurons</see>.
    /// </remarks>
    /// 
    [Serializable]
    public class StochasticLayer : ActivationLayer
    {

        private new StochasticNeuron[] neurons;
        private double[] sample;

        /// <summary>
        ///   Gets the layer's neurons.
        /// </summary>
        /// 
        public new StochasticNeuron[] Neurons
        {
            get { return neurons; }
        }

        /// <summary>
        ///   Gets the layer's sample values generated in the last
        ///   call of any of the <see cref="Generate"/> methods.
        /// </summary>
        /// 
        public double[] Sample
        {
            get { return sample; }
        }


        /// <summary>
        ///   Initializes a new instance of the <see cref="StochasticLayer"/> class.
        /// </summary>
        /// 
        /// <param name="neuronsCount">Layer's neurons count.</param>
        /// <param name="inputsCount">Layer's inputs count.</param>
        /// 
        public StochasticLayer(int neuronsCount, int inputsCount)
            : this(new BernoulliFunction(alpha: 1), neuronsCount, inputsCount) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="StochasticLayer"/> class.
        /// </summary>
        /// 
        /// <param name="function">The activation function for the neurons in the layer.</param>
        /// <param name="neuronsCount">The neurons count.</param>
        /// <param name="inputsCount">The inputs count.</param>
        /// 
        public StochasticLayer(IStochasticFunction function, int neuronsCount, int inputsCount)
            : base(neuronsCount, inputsCount, function)
        {
            neurons = new StochasticNeuron[neuronsCount];
            for (int i = 0; i < neurons.Length; i++)
                base.neurons[i] = this.neurons[i] =
                    new StochasticNeuron(inputsCount, function);
        }


        /// <summary>
        ///   Compute output vector of the layer.
        /// </summary>
        /// 
        /// <param name="input">Input vector.</param>
        /// 
        /// <returns>
        ///   Returns layer's output vector.
        /// </returns>
        /// 
        public override double[] Compute(double[] input)
        {
            double[] output = new double[neuronsCount];

            for (int i = 0; i < neurons.Length; i++)
                output[i] = neurons[i].Compute(input);

            this.output = output;

            return output;
        }

        /// <summary>
        ///   Compute probability vector of the layer.
        /// </summary>
        /// 
        /// <param name="input">Input vector.</param>
        /// 
        /// <returns>
        ///   Returns layer's probability vector.
        /// </returns>
        /// 
        public double[] Generate(double[] input)
        {
            double[] sample = new double[neuronsCount];
            double[] output = new double[neuronsCount];

            for (int i = 0; i < neurons.Length; i++)
            {
                sample[i] = neurons[i].Generate(input);
                output[i] = neurons[i].Output;
            }

            this.sample = sample;
            this.output = output;

            return sample;
        }

        /// <summary>
        ///   Copy the weights of another layer in reversed order. This
        ///   can be used to update visible layers from hidden layers and
        ///   vice-versa.
        /// </summary>
        /// 
        /// <param name="layer">The layer to copy the weights from.</param>
        /// 
        public void CopyReversedWeightsFrom(StochasticLayer layer)
        {
            int hiddenNeurons = Neurons.Length;
            int visibleNeurons = inputsCount;

            for (int i = 0; i < hiddenNeurons; i++)
                for (int j = 0; j < inputsCount; j++)
                    this.Neurons[i].Weights[j] = layer.Neurons[j].Weights[i];
        }
    }
}
