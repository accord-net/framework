using Accord.MachineLearning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accord.MachineLearning
{
    public abstract class Pipeline<TInput, TOutput, T1, T2> : ITransform<TInput, TOutput>
        where T1 : ITransform
        where T2 : ITransform
    {

        public T1 First { get; set; }
        public T2 Second { get; set; }

        public TOutput Transform(TInput input)
        {
            return Transform(new[] { input })[0];
        }

        public TOutput[] Transform(TInput[] input)
        {
            return Transform(input, new TOutput[input.Length]);
        }

        public abstract TOutput[] Transform(TInput[] input, TOutput[] result);

        public int NumberOfInputs
        {
            get { return First.NumberOfInputs; }
        }

        public int NumberOfOutputs
        {
            get { return Second.NumberOfOutputs; }
        }
    }
}
