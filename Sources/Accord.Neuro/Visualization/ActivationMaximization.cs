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

namespace Accord.Neuro.Visualization
{
    using Accord.Math;
    using AForge.Neuro;

    /// <summary>
    ///   Activation-Maximization method for visualizing neuron's roles.
    /// </summary>
    /// 
    public class ActivationMaximization
    {
        ActivationNeuron neuron;

        /// <summary>
        ///   Initializes a new instance of the <see cref="ActivationMaximization"/> class.
        /// </summary>
        /// 
        /// <param name="neuron">The neuron to be visualized.</param>
        /// 
        public ActivationMaximization(ActivationNeuron neuron)
        {
            this.neuron = neuron;
        }

        /// <summary>
        ///   Finds the value which maximizes
        ///   the activation of this neuron.
        /// </summary>
        /// 
        public double[] Maximize()
        {
            // Initialize
            double[] value = new double[neuron.InputsCount];
            for (int i = 0; i < value.Length; i++)
                value[i] = Accord.Math.Tools.Random.NextDouble();
            
            double[] gradient = new double[neuron.InputsCount];

            for (int iter = 0; iter < 50; iter++)
            {
                // Compute the activation gradient
                double activation = neuron.Compute(value);
                for (int i = 0; i < gradient.Length; i++)
                    gradient[i] = neuron.Weights[i] * neuron.ActivationFunction.Derivative2(activation);

                // Walk against the gradient
                for (int i = 0; i < value.Length; i++)
                    value[i] -= gradient[i];

                value = value.Divide(value.Sum());
            }

            return value;
        }

    }
}
