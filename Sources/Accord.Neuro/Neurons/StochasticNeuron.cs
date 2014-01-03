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

namespace Accord.Neuro.Neurons
{
    using System;
    using Accord.Neuro.ActivationFunctions;
    using AForge.Neuro;

    /// <summary>
    ///   Stochastic Activation Neuron.
    /// </summary>
    /// 
    /// <remarks>
    ///   The Stochastic Activation Neuron is an activation neuron
    ///   which activates (returns 1) only within a given probability.
    ///   The neuron has a random component in the activation function,
    ///   and the neuron fires only if the total sum, after applied
    ///   to a logistic activation function, is greater than a randomly
    ///   sampled value.
    /// </remarks>
    /// 
    [Serializable]
    public class StochasticNeuron : ActivationNeuron
    {

        private double sample;
        private new IStochasticFunction function;

        /// <summary>
        ///   Gets the neuron sample value generated in the last
        ///   call of any of the <see cref="Generate(double[])"/> methods.
        /// </summary>
        /// 
        public double Sample
        {
            get { return sample; }
        }

        /// <summary>
        ///   Gets or sets the stochastic activation 
        ///   function for this stochastic neuron.
        /// </summary>
        /// 
        public new IStochasticFunction ActivationFunction
        {
            get { return function; }
            set { base.function = this.function =  value; }
        }


        /// <summary>
        ///   Initializes a new instance of the <see cref="StochasticNeuron"/> class.
        /// </summary>
        /// 
        /// <param name="inputs">Number of inputs for the neuron.</param>
        /// <param name="function">Activation function for the neuron.</param>
        /// 
        public StochasticNeuron(int inputs, IStochasticFunction function)
            : base(inputs, function)
        {
            this.ActivationFunction = function;
            this.Threshold = 0; // Ruslan Salakhutdinov and Geoff Hinton
                                // also start with zero thresholds
        }

        /// <summary>
        ///   Computes output value of neuron.
        /// </summary>
        /// 
        /// <param name="input">An input vector.</param>
        /// 
        /// <returns>Returns the neuron's output value for the given input.</returns>
        /// 
        public override double Compute(double[] input)
        {
            double sum = threshold;
            for (int i = 0; i < weights.Length; i++)
                sum += weights[i] * input[i];

            double output = function.Function(sum);

            this.output = output;

            return output;
        }


        /// <summary>
        ///   Samples the neuron output considering
        ///   the stochastic activation function.
        /// </summary>
        /// 
        /// <param name="input">An input vector.</param>
        /// 
        /// <returns>A possible output for the neuron drawn
        /// from the neuron's stochastic function.</returns>
        /// 
        public double Generate(double[] input)
        {
            double sum = threshold;
            for (int i = 0; i < weights.Length; i++)
                sum += weights[i] * input[i];

            double output = function.Function(sum);
            double sample = function.Generate2(output);

            this.output = output;
            this.sample = sample;

            return sample;
        }

        /// <summary>
        ///   Samples the neuron output considering
        ///   the stochastic activation function.
        /// </summary>
        /// 
        /// <param name="output">The (previously computed) neuron output.</param>
        /// 
        /// <returns>A possible output for the neuron drawn
        /// from the neuron's stochastic function.</returns>
        /// 
        public double Generate(double output)
        {
            double sample = function.Generate2(output);

            this.output = output;
            this.sample = sample;

            return sample;
        }

    }
}
