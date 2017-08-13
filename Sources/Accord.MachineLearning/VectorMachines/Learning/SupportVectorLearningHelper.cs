// Accord Machine Learning Library
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

namespace Accord.MachineLearning.VectorMachines.Learning
{
    using Accord.Math;
    using Accord.Statistics.Kernels;
    using System;
    using System.Collections;

    internal static class SupportVectorLearningHelper
    {


        public static TModel Create<TModel, TInput, TKernel>(int inputs, TKernel kernel)
            where TModel : class, ISupportVectorMachine<TInput>
            where TKernel : IKernel<TInput>
#if !NETSTANDARD1_4
            where TInput : ICloneable
#endif
        {
            TModel result = null;
            var type = typeof(TModel);
            if (type == typeof(SupportVectorMachine))
                result = new SupportVectorMachine(inputs) as TModel;
            if (type == typeof(SupportVectorMachine<IKernel>))
                result = new SupportVectorMachine<IKernel>(inputs, kernel as IKernel) as TModel;
            if (type == typeof(SupportVectorMachine<IKernel<double[]>>))
                result = new SupportVectorMachine<IKernel<double[]>>(inputs, kernel as IKernel<double[]>) as TModel;
#if !NETSTANDARD1_4
#pragma warning disable 0618
            else if (type == typeof(KernelSupportVectorMachine))
                result = new KernelSupportVectorMachine(kernel as IKernel, inputs) as TModel;
#pragma warning restore 0618
#endif
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
            if (x.Length == 0)
                throw new ArgumentException("Impossible to determine number of inputs because there are no training samples in this set.");

            var linear = kernel as ILinear<TInput>;
            if (linear != null)
                return linear.GetLength(x);

            return Accord.MachineLearning.Tools.GetNumberOfInputs(x);
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
            if (!typeof(TKernel).HasDefaultConstructor())
                throw new InvalidOperationException("Please set the kernel function before learning a model.");

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
