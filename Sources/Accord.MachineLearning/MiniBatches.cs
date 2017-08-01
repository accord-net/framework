// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
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

namespace Accord.MachineLearning
{
    using System;
    using System.Data;
    using Accord.Math;
    using System.Collections.Generic;
    using System.Linq;
    using System.Collections;
    using Accord.MachineLearning.Performance;

    public enum ShuffleMethod
    {
        None,
        OnlyOnce,
        EveryEpoch,
    }

    public static class MiniBatches
    {
        public static Batches<TInput, TOutput> Create<TInput, TOutput>(
            TInput[] input, TOutput[] output, double[] weights = null,
            int batchSize = 32, int maxIterations = 0, int maxEpochs = 0,
            ShuffleMethod shuffle = ShuffleMethod.EveryEpoch)
        {
            return new Batches<TInput, TOutput>(input, output, weights)
            {
                MiniBatchSize = batchSize,
                MaxIterations = maxIterations,
                MaxEpochs = maxEpochs,
                Shuffle = shuffle
            };
        }
    }

    public class Batches<TInput> : BaseBatches<DataSubset<TInput>, TInput>
    {

        public Batches(TInput[] inputs, double[] weights)
            : base(inputs, weights)
        {
        }

        protected override DataSubset<TInput> Init()
        {
            return new DataSubset<TInput>(MiniBatchSize, NumberOfSamples);
        }

    }

    public class Batches<TInput, TOutput> : BaseBatches<DataSubset<TInput, TOutput>, TInput>
    {
        public TOutput[] Outputs { get; set; }

        private TOutput[] shuffledOutputs;


        public Batches(TInput[] inputs, TOutput[] outputs, double[] weights)
            : base(inputs, weights)
        {
            if (inputs.Length != outputs.Length)
                throw new DimensionMismatchException();

            this.Outputs = outputs;
        }

        protected override DataSubset<TInput, TOutput> Init()
        {
            return new DataSubset<TInput, TOutput>(MiniBatchSize, NumberOfSamples);
        }

        protected override void PutCurrentSampleInMiniBatch(DataSubset<TInput, TOutput> batch, int positionInMiniBatch)
        {
            base.PutCurrentSampleInMiniBatch(batch, positionInMiniBatch);
            batch.Outputs[positionInMiniBatch] = shuffledOutputs[CurrentSample];
        }

        protected override void PrepareBatch(int[] idx)
        {
            base.PrepareBatch(idx);
            this.shuffledOutputs = Outputs.Get(idx);
        }
    }

    public abstract class BaseBatches<TBatch, TInput> : IEnumerable<TBatch>
        where TBatch : DataSubset<TInput>
    {

        public TInput[] Inputs { get; set; }


        public double[] Weights { get; set; }


        public int MiniBatchSize { get; set; }

        public ShuffleMethod Shuffle { get; set; }

        public int NumberOfSamples { get { return Inputs.Length; } }

        public int NumberOfMiniBatches { get; private set; }

        public int MaxIterations { get; set; }

        public int MaxEpochs { get; set; }


        public int CurrentIteration { get; set; }

        public int CurrentEpoch { get; set; }

        public int CurrentSample { get; set; }

        public int CurrentMiniBatch { get; set; }

        private TInput[] shuffledInputs;
        private double[] shuffledWeights;
        private int[] shuffledIndices;


        public BaseBatches(TInput[] inputs, double[] weights)
        {
            if (weights != null)
            {
                if (inputs.Length != weights.Length)
                    throw new DimensionMismatchException();
            }
            else
            {
                weights = Vector.Ones(inputs.Length);
            }

            this.Inputs = inputs;
            this.Weights = weights;
            this.MiniBatchSize = 32;
            this.Shuffle = ShuffleMethod.OnlyOnce;
            this.NumberOfMiniBatches = (int)Math.Ceiling(Inputs.Length / (double)MiniBatchSize);
        }


        public virtual IEnumerator<TBatch> GetEnumerator()
        {
            TBatch batch = Init();

            if (Shuffle == ShuffleMethod.None)
                PrepareBatch(Vector.Range(Inputs.Length));
            else // if (Shuffle == ShuffleMethod.OnlyOnce || ShuffleMethod.EveryEpoch)
                PrepareBatch(Vector.Sample(Inputs.Length));

            this.CurrentIteration = 0;
            this.CurrentEpoch = 0;
            this.CurrentSample = 0;
            this.CurrentMiniBatch = 0;

            while (true)
            {
                for (int i = 0; i < batch.Inputs.Length; i++)
                {
                    PutCurrentSampleInMiniBatch(batch, i);

                    CurrentSample++;

                    if (CurrentSample >= NumberOfSamples)
                    {
                        CurrentEpoch++;
                        CurrentSample = 0;

                        if (Shuffle == ShuffleMethod.EveryEpoch)
                            PrepareBatch(Vector.Sample(Inputs.Length));
                    }
                }

                batch.Index = CurrentMiniBatch;

                yield return batch;

                CurrentMiniBatch++;
                if (CurrentMiniBatch >= NumberOfMiniBatches)
                    CurrentMiniBatch = 0;

                CurrentIteration++;

                if (MaxEpochs > 0 && CurrentEpoch > MaxEpochs)
                    yield break;

                if (MaxIterations > 0 && CurrentIteration > MaxIterations)
                    yield break;
            }
        }

        protected abstract TBatch Init();

        protected virtual void PutCurrentSampleInMiniBatch(TBatch batch, int positionInMiniBatch)
        {
            batch.Inputs[positionInMiniBatch] = shuffledInputs[CurrentSample];
            batch.Weights[positionInMiniBatch] = shuffledWeights[CurrentSample];
            batch.Indices[positionInMiniBatch] = shuffledIndices[CurrentSample];
        }

        protected virtual void PrepareBatch(int[] idx)
        {
            this.shuffledInputs = Inputs.Get(idx);
            this.shuffledWeights = Weights.Get(idx);
            this.shuffledIndices = idx;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

}
