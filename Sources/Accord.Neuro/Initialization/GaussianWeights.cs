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

namespace Accord.Neuro
{
    using AForge.Math.Random;
    using AForge.Neuro;

    /// <summary>
    ///   Gaussian weight initialization.
    /// </summary>
    /// 
    public class GaussianWeights
    {
        private ActivationNetwork network;
        private GaussianGenerator random;

        /// <summary>
        ///   Gets ors sets whether the initialization
        ///   should update neurons thresholds (biases)
        /// </summary>
        /// 
        public bool UpdateThresholds { get; set; }

        /// <summary>
        ///   Constructs a new Gaussian Weight initialization.
        /// </summary>
        /// 
        /// <param name="network">The activation network whose weights will be initialized.</param>
        /// <param name="stdDev">The standard deviation to be used. Common values lie in the 0.001-
        /// 0.1 range. Default is 0.1.</param>
        /// 
        public GaussianWeights(ActivationNetwork network, double stdDev = 0.1)
        {
            this.network = network;

            this.random = new GaussianGenerator(0f, (float)stdDev, Accord.Math.Tools.Random.Next());

            this.UpdateThresholds = false;
        }

        /// <summary>
        ///   Randomizes (initializes) the weights of
        ///   the network using a Gaussian distribution.
        /// </summary>
        /// 
        public void Randomize()
        {
            foreach (ActivationLayer layer in network.Layers)
            {
                foreach (ActivationNeuron neuron in layer.Neurons)
                {
                    for (int i = 0; i < neuron.Weights.Length; i++)
                        neuron.Weights[i] = random.Next();
                    if (UpdateThresholds)
                        neuron.Threshold = random.Next();
                }
            }
        }

        /// <summary>
        ///   Randomizes (initializes) the weights of
        ///   the network using a Gaussian distribution.
        /// </summary>
        /// 
        public void Randomize(int layerIndex)
        {
            var layer = network.Layers[layerIndex] as ActivationLayer;

            foreach (ActivationNeuron neuron in layer.Neurons)
            {
                for (int i = 0; i < neuron.Weights.Length; i++)
                    neuron.Weights[i] = random.Next();
                if (UpdateThresholds)
                    neuron.Threshold = random.Next();
            }
        }

    }
}
