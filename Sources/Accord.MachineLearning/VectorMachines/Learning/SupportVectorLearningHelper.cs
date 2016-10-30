// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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

namespace Accord.MachineLearning.VectorMachines.Learning
{
    using Accord.Statistics.Kernels;
    using System;
    using System.Collections;

    internal static class SupportVectorLearningHelper
    {


        public static TModel Create<TModel, TInput, TKernel>(int inputs, TKernel kernel)
            where TModel : class, ISupportVectorMachine<TInput>
            where TKernel : IKernel<TInput>
            where TInput : ICloneable
        {
            TModel result = null;
            var type = typeof(TModel);
            if (type == typeof(SupportVectorMachine))
                result = new SupportVectorMachine(inputs) as TModel;
            if (type == typeof(SupportVectorMachine<IKernel>))
                result = new SupportVectorMachine<IKernel>(inputs, kernel as IKernel) as TModel;
            if (type == typeof(SupportVectorMachine<IKernel<double[]>>))
                result = new SupportVectorMachine<IKernel<double[]>>(inputs, kernel as IKernel<double[]>) as TModel;
#pragma warning disable 0618
            else if (type == typeof(KernelSupportVectorMachine))
                result = new KernelSupportVectorMachine(kernel as IKernel, inputs) as TModel;
#pragma warning restore 0618
            else if (type == typeof(SupportVectorMachine<TKernel, TInput>))
                result = new SupportVectorMachine<TKernel, TInput>(inputs, kernel) as TModel;
            if (result == null)
                throw new NotSupportedException("If you are implementing your own support vector machine type, please override the Create method in your learning algorithm to instruct the framework how to instantiate a type of your new class.");
            return result;
        }



        public static TModel Create<TModel, TKernel>(int inputs, TKernel kernel)
            where TModel : class, ISupportVectorMachine<double[]>
            where TKernel : IKernel<double[]>
        {
            TModel result = Create<TModel, double[], TKernel>(inputs, kernel);

            if (result == null)
                throw new NotSupportedException("If you are implementing your own support vector machine type, please override the Create method in your learning algorithm to instruct the framework how to instantiate a type of your new class.");

            return result;
        }

        public static int GetNumberOfInputs<TKernel, TInput>(TKernel kernel, TInput[] x)
        {
            var linear = kernel as ILinear<TInput>;
            if (linear != null)
                return linear.GetLength(x);

            var first = x[0] as IList;
            if (first == null)
                return 0;

            int length = first.Count;

            for (int i = 0; i < x.Length; i++)
            {
                int c = (x[i] as IList).Count;
                if (c != length)
                    return 0;
            }

            return length;
        }

        public static void CheckArgs<TInput>(ISupportVectorMachine<TInput> machine,
            TInput[] inputs, int[] outputs)
        {
            // Initial argument checking
            if (machine == null)
                throw new ArgumentNullException("machine");

            if (inputs == null)
                throw new ArgumentNullException("inputs");

            if (outputs == null)
                throw new ArgumentNullException("outputs");

            if (inputs.Length != outputs.Length)
                throw new DimensionMismatchException("outputs",
                    "The number of input vectors and output labels does not match.");

            checkInputs(machine, inputs);

            for (int i = 0; i < outputs.Length; i++)
            {
                if (outputs[i] != 1 && outputs[i] != -1)
                {
                    throw new ArgumentOutOfRangeException("outputs",
                        "The output label at index " + i + " should be either +1 or -1.");
                }
            }
        }

        public static void CheckArgs<TInput>(ISupportVectorMachine<TInput> machine, TInput[] inputs, Array outputs)
        {
            // Initial argument checking
            if (machine == null)
                throw new ArgumentNullException("machine");

            if (inputs == null)
                throw new ArgumentNullException("inputs");

            if (outputs == null)
                throw new ArgumentNullException("outputs");

            if (inputs.Length != outputs.Length)
                throw new DimensionMismatchException("outputs",
                    "The number of input vectors and output labels does not match.");

            checkInputs(machine, inputs);
        }

        private static void checkInputs<TInput>(ISupportVectorMachine<TInput> machine, TInput[] inputs)
        {
            if (inputs.Length == 0)
                throw new ArgumentOutOfRangeException("inputs",
                    "Training algorithm needs at least one training vector.");

            if (machine.NumberOfInputs > 0)
            {
                // This machine has a fixed input vector size
                for (int i = 0; i < inputs.Length; i++)
                {
                    if (inputs[i] == null)
                    {
                        throw new ArgumentNullException("inputs",
                               "The input vector at index " + i + " is null.");
                    }

                    var xi = inputs[i] as Array;
                    if (xi != null)
                    {
                        if (xi.Length != machine.NumberOfInputs)
                        {
                            throw new DimensionMismatchException("inputs",
                                "The size of the input vector at index " + i
                                + " does not match the expected number of inputs of the machine."
                                + " All input vectors for this machine must have length " + machine.NumberOfInputs);
                        }
                    }

                    var di = inputs[i] as double[];
                    if (di != null)
                    {
                        for (int j = 0; j < di.Length; j++)
                        {
                            if (Double.IsNaN(di[j]))
                                throw new ArgumentException("The input vector at index " + i + " contains NaN values.");

                            if (Double.IsInfinity(di[j]))
                                throw new ArgumentException("The input vector at index " + i + " contains infinity values.");
                        }
                    }
                }
            }
        }


        public static TInput GetZeroWeight<TInput>(TInput[] x)
            where TInput : ICloneable
        {
            var w = (TInput)x[0].Clone();
            if (w is IList)
            {
                IList list = (IList)w;
                Array.Clear((Array)list, 0, list.Count);
            }
            return w;
        }

        public static void CheckOutput<TInput>(ISupportVectorMachine<TInput> model)
        {
            if (model.SupportVectors == null)
                throw new Exception();
            if (model.Weights == null)
                throw new Exception();
            if (model.SupportVectors.Length != model.Weights.Length)
                throw new Exception();
        }

        public static TKernel EstimateKernel<TKernel, TInput>(TKernel kernel, TInput[] x)
            where TKernel : IKernel<TInput>
        {
            var estimable = kernel as IEstimable<TInput>;
            if (estimable == null)
                throw new InvalidOperationException("Kernel type does not support estimation.");
            estimable.Estimate(x);
            return (TKernel)estimable;
        }

        public static TKernel CreateKernel<TKernel, TInput>(TInput[] x)
            where TKernel : IKernel<TInput>
        {
            var kernel = Activator.CreateInstance<TKernel>();
            var estimable = kernel as IEstimable<TInput>;
            if (estimable != null)
            {
                estimable.Estimate(x);
                return (TKernel)estimable;
            }

            return kernel;
        }
    }
}
