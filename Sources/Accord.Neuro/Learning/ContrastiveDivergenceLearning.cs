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

namespace Accord.Neuro.Learning
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Accord.Neuro.Layers;
    using Accord.Neuro.Networks;
    using Accord.Neuro.Neurons;
    using Accord.Math;
    using AForge.Neuro.Learning;
    using Accord.Neuro.ActivationFunctions;


    /// <summary>
    ///   Contrastive Divergence learning algorithm for Restricted Boltzmann Machines.
    /// </summary>
    /// 
    public class ContrastiveDivergenceLearning : IUnsupervisedLearning, IDisposable
    {

        private double momentum = 0.9;
        private double learningRate = 0.1;
        private double decay = 0.01;
        private int steps = 1;

        private double[][] weightsGradient;
        private double[] visibleBiasGradient;
        private double[] hiddenBiasGradient;

        private double[][] weightsUpdates;
        private double[] visibleBiasUpdates;
        private double[] hiddenBiasUpdates;

        private ThreadLocal<ParallelStorage> storage;

        private int inputsCount;
        private int hiddenCount;

        private StochasticLayer hidden;
        private StochasticLayer visible;


        /// <summary>
        ///   Creates a new <see cref="ContrastiveDivergenceLearning"/> algorithm.
        /// </summary>
        /// 
        /// <param name="network">The network to be trained.</param>
        /// 
        public ContrastiveDivergenceLearning(RestrictedBoltzmannMachine network)
        {
            init(network.Hidden, network.Visible);
        }

        /// <summary>
        ///   Creates a new <see cref="ContrastiveDivergenceLearning"/> algorithm.
        /// </summary>
        /// 
        /// <param name="hidden">The hidden layer of the hidden-visible layer pair to be trained.</param>
        /// <param name="visible">The visible layer of the hidden-visible layer pair to be trained.</param>
        /// 
        public ContrastiveDivergenceLearning(StochasticLayer hidden, StochasticLayer visible)
        {
            init(hidden, visible);
        }

        private void init(StochasticLayer hidden, StochasticLayer visible)
        {
            this.hidden = hidden;
            this.visible = visible;

            inputsCount = hidden.InputsCount;
            hiddenCount = hidden.Neurons.Length;

            weightsGradient = new double[inputsCount][];
            for (int i = 0; i < weightsGradient.Length; i++)
                weightsGradient[i] = new double[hiddenCount];

            visibleBiasGradient = new double[inputsCount];
            hiddenBiasGradient = new double[hiddenCount];

            weightsUpdates = new double[inputsCount][];
            for (int i = 0; i < weightsUpdates.Length; i++)
                weightsUpdates[i] = new double[hiddenCount];

            visibleBiasUpdates = new double[inputsCount];
            hiddenBiasUpdates = new double[hiddenCount];

            storage = new ThreadLocal<ParallelStorage>(() =>
                new ParallelStorage(inputsCount, hiddenCount));
        }

        /// <summary>
        ///   Gets or sets the learning rate of the
        ///   learning algorithm. Default is 0.1.
        /// </summary>
        /// 
        public double LearningRate
        {
            get { return learningRate; }
            set { learningRate = value; }
        }

        /// <summary>
        ///   Gets or sets the momentum term of the
        ///   learning algorithm. Default is 0.9.
        /// </summary>
        /// 
        public double Momentum
        {
            get { return momentum; }
            set { momentum = value; }
        }

        /// <summary>
        ///   Gets or sets the Weight Decay constant
        ///   of the learning algorithm. Default is 0.01.
        /// </summary>
        /// 
        public double Decay
        {
            get { return decay; }
            set { decay = value; }
        }

        /// <summary>
        ///   Not supported.
        /// </summary>
        /// 
        public double Run(double[] input)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///   Runs learning epoch.
        /// </summary>
        /// 
        /// <param name="input">Array of input vectors.</param>
        /// 
        /// <returns>
        ///   Returns sum of learning errors.
        /// </returns>
        /// 
        public double RunEpoch(double[][] input)
        {

            // Initialize iteration
            for (int i = 0; i < weightsGradient.Length; i++)
                Array.Clear(weightsGradient[i], 0, weightsGradient[i].Length);
            Array.Clear(hiddenBiasGradient, 0, hiddenBiasGradient.Length);
            Array.Clear(visibleBiasGradient, 0, visibleBiasGradient.Length);


            // calculate gradient and model error
            double error = ComputeGradient(input);

            // calculate weights updates
            CalculateUpdates(input);

            // update the network
            UpdateNetwork();

            return error;
        }

        private double ComputeGradient(double[][] input)
        {
            double errors = 0;
            

#if NET35
            var partial = storage.Value.Clear();
            for (int i = 0; i < input.Length; i++)
            {
                int observationIndex = i;
#else
            Object lockObj = new Object();

            // For each training instance
            Parallel.For(0, input.Length,

#if DEBUG
				new ParallelOptions() { MaxDegreeOfParallelism = 1 },
#endif

                // Initialize
                () => storage.Value.Clear(),

                // Map
                (observationIndex, loopState, partial) =>
#endif
                {
                    var observation = input[observationIndex];

                    var probability = partial.OriginalProbability;
                    var activations = partial.OriginalActivations;
                    var reconstruction = partial.ReconstructedInput;
                    var reprobability = partial.ReconstructedProbs;

                    var weightGradient = partial.WeightGradient;
                    var hiddenGradient = partial.HiddenBiasGradient;
                    var visibleGradient = partial.VisibleBiasGradient;


                    // 1. Compute a forward pass. The network is being
                    //    driven by data, so we will gather activations
                    for (int j = 0; j < hidden.Neurons.Length; j++)
                    {
                        probability[j] = hidden.Neurons[j].Compute(observation);  // output probabilities
                        activations[j] = hidden.Neurons[j].Generate(probability[j]); // state activations
                    }

                    // 2. Reconstruct inputs from previous outputs
                    for (int j = 0; j < visible.Neurons.Length; j++)
                        reconstruction[j] = visible.Neurons[j].Compute(activations);


                    if (steps > 1)
                    {
                        // Perform Gibbs sampling
                        double[] current = probability;
                        for (int k = 0; k < steps - 1; k++)
                        {
                            for (int j = 0; j < probability.Length; j++)
                                probability[j] = hidden.Neurons[j].Compute(current);
                            for (int j = 0; j < reconstruction.Length; j++)
                                reconstruction[j] = visible.Neurons[j].Compute(probability);
                            current = reconstruction;
                        }
                    }


                    // 3. Compute outputs for the reconstruction. The network
                    //    is now being driven by reconstructions, so we should
                    //    gather the output probabilities without sampling
                    for (int j = 0; j < hidden.Neurons.Length; j++)
                        reprobability[j] = hidden.Neurons[j].Compute(reconstruction);



                    // 4.1. Compute positive associations
                    for (int k = 0; k < observation.Length; k++)
                        for (int j = 0; j < probability.Length; j++)
                            weightGradient[k][j] += observation[k] * probability[j];

                    for (int j = 0; j < hiddenGradient.Length; j++)
                        hiddenGradient[j] += probability[j];

                    for (int j = 0; j < visibleGradient.Length; j++)
                        visibleGradient[j] += observation[j];


                    // 4.2. Compute negative associations
                    for (int k = 0; k < reconstruction.Length; k++)
                        for (int j = 0; j < reprobability.Length; j++)
                            weightGradient[k][j] -= reconstruction[k] * reprobability[j];

                    for (int j = 0; j < reprobability.Length; j++)
                        hiddenGradient[j] -= reprobability[j];

                    for (int j = 0; j < reconstruction.Length; j++)
                        visibleGradient[j] -= reconstruction[j];


                    // Compute current error
                    for (int j = 0; j < observation.Length; j++)
                    {
                        double e = observation[j] - reconstruction[j];
                        partial.ErrorSumOfSquares += e * e;
                    }

#if !NET35
                    return partial; // Report partial solution
                },

                // Reduce
                (partial) =>
                {
                    lock (lockObj)
                    {
                        // Accumulate partial solutions
                        for (int i = 0; i < weightsGradient.Length; i++)
                            for (int j = 0; j < weightsGradient[i].Length; j++)
                                weightsGradient[i][j] += partial.WeightGradient[i][j];

                        for (int i = 0; i < hiddenBiasGradient.Length; i++)
                            hiddenBiasGradient[i] += partial.HiddenBiasGradient[i];

                        for (int i = 0; i < visibleBiasGradient.Length; i++)
                            visibleBiasGradient[i] += partial.VisibleBiasGradient[i];

                        errors += partial.ErrorSumOfSquares;
                    }
                });
#else
                }
            }

            weightsGradient = partial.WeightGradient;
            hiddenBiasGradient = partial.HiddenBiasGradient;
            visibleBiasGradient = partial.VisibleBiasGradient;
            errors = partial.ErrorSumOfSquares;
#endif

            return errors;
        }

        private void CalculateUpdates(double[][] input)
        {
            double rate = learningRate;

            // Assume all neurons in the layer have the same act function
            if (visible.Neurons[0].ActivationFunction is GaussianFunction)
                rate = learningRate / (100 * input.Length);
            else rate = learningRate / (input.Length);


            // 5. Compute gradient descent updates
            for (int i = 0; i < weightsGradient.Length; i++)
                for (int j = 0; j < weightsGradient[i].Length; j++)
                    weightsUpdates[i][j] = momentum * weightsUpdates[i][j]
                        + (rate * weightsGradient[i][j]);

            for (int i = 0; i < hiddenBiasUpdates.Length; i++)
                hiddenBiasUpdates[i] = momentum * hiddenBiasUpdates[i]
                        + (rate * hiddenBiasGradient[i]);

            for (int i = 0; i < visibleBiasUpdates.Length; i++)
                visibleBiasUpdates[i] = momentum * visibleBiasUpdates[i]
                        + (rate * visibleBiasGradient[i]);

            System.Diagnostics.Debug.Assert(!weightsGradient.HasNaN());
            System.Diagnostics.Debug.Assert(!visibleBiasUpdates.HasNaN());
            System.Diagnostics.Debug.Assert(!hiddenBiasUpdates.HasNaN());
        }

        private void UpdateNetwork()
        {
            // 6.1 Update hidden layer weights
            for (int i = 0; i < hidden.Neurons.Length; i++)
            {
                StochasticNeuron neuron = hidden.Neurons[i];
                for (int j = 0; j < neuron.Weights.Length; j++)
                    neuron.Weights[j] += weightsUpdates[j][i] - learningRate * decay * neuron.Weights[j];
                neuron.Threshold += hiddenBiasUpdates[i];
            }

            // 6.2 Update visible layer with reverse weights
            for (int i = 0; i < visible.Neurons.Length; i++)
                visible.Neurons[i].Threshold += visibleBiasUpdates[i];
            visible.CopyReversedWeightsFrom(hidden);
        }



        private class ParallelStorage
        {
            public double[][] WeightGradient { get; set; }
            public double[] VisibleBiasGradient { get; set; }
            public double[] HiddenBiasGradient { get; set; }

            public double[] OriginalActivations { get; set; }
            public double[] OriginalProbability { get; set; }

            public double[] ReconstructedInput { get; set; }
            public double[] ReconstructedProbs { get; set; }

            public double ErrorSumOfSquares { get; set; }

            public ParallelStorage(int inputsCount, int hiddenCount)
            {
                WeightGradient = new double[inputsCount][];
                for (int i = 0; i < WeightGradient.Length; i++)
                    WeightGradient[i] = new double[hiddenCount];

                VisibleBiasGradient = new double[inputsCount];
                HiddenBiasGradient = new double[hiddenCount];

                OriginalActivations = new double[hiddenCount];
                OriginalProbability = new double[hiddenCount];
                ReconstructedInput = new double[inputsCount];
                ReconstructedProbs = new double[hiddenCount];
            }

            public ParallelStorage Clear()
            {
                ErrorSumOfSquares = 0;
                for (int i = 0; i < WeightGradient.Length; i++)
                    Array.Clear(WeightGradient[i], 0, WeightGradient[i].Length);
                Array.Clear(VisibleBiasGradient, 0, VisibleBiasGradient.Length);
                Array.Clear(HiddenBiasGradient, 0, HiddenBiasGradient.Length);
                return this;
            }

        }


        #region IDisposable members
        /// <summary>
        ///   Performs application-defined tasks associated with 
        ///   freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///   Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// 
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged
        /// resources; <c>false</c> to release only unmanaged resources.</param>
        /// 
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (storage != null)
                {
                    storage.Dispose();
                    storage = null;
                }
            }
        }

        /// <summary>
        ///   Releases unmanaged resources and performs other cleanup operations before the
        ///   <see cref="ContrastiveDivergenceLearning"/> is reclaimed by garbage collection.
        /// </summary>
        /// 
        ~ContrastiveDivergenceLearning()
        {
            Dispose(false);
        }
        #endregion

    }
}
