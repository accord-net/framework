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

namespace Accord.MachineLearning.VectorMachines
{
    using System;
    using System.IO;
    using Accord.IO;
    using Accord.Math;
    using System.Runtime.Serialization.Formatters.Binary;
    using Accord.Statistics.Kernels;
    using Accord.Statistics.Links;
    using Accord.Statistics.Models.Regression;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using Accord.Compat;

    /// <summary>
    ///   Obsolete.
    /// </summary>
    /// 
    [Serializable]
    [Obsolete("Please use SupportVectorMachine<TKernel>.")]
#if !NETSTANDARD1_4
    [SerializationBinder(typeof(KernelSupportVectorMachine.KernelSupportVectorMachineBinder))]
#endif
    public class KernelSupportVectorMachine :
        SupportVectorMachine<IKernel<double[]>>,
        ISupportVectorMachine<double[]>
    {

        /// <summary>
        ///   Obsolete.
        /// </summary>
        public KernelSupportVectorMachine(IKernel<double[]> kernel, int inputs)
            : base(inputs, kernel)
        {
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        public KernelSupportVectorMachine(IKernel kernel, int inputs)
            : base(inputs, kernel)
        {
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        public new IKernel Kernel
        {
            get { return (IKernel)base.Kernel; }
            set { base.Kernel = value; }
        }



#region Serialization backwards compatibility
#if !NETSTANDARD1_4
        internal class KernelSupportVectorMachineBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                AssemblyName name = new AssemblyName(assemblyName);

                if (name.Version < new Version(3, 1, 0))
                {
                    if (typeName == "Accord.MachineLearning.VectorMachines.SupportVectorMachine")
                        return typeof(SupportVectorMachine.SupportVectorMachine_2_13);
                    else if (typeName == "Accord.MachineLearning.VectorMachines.KernelSupportVectorMachine")
                        return typeof(KernelSupportVectorMachine_2_13);
                }

                return null;
            }
        }

#pragma warning disable 0169
#pragma warning disable 0649

        [Serializable]
        internal class KernelSupportVectorMachine_2_13 : ISerializable
        {
            public int inputCount;
            public double[][] supportVectors;
            public double[] weights;
            public double threshold;
            public ILinkFunction linkFunction;
            public IKernel kernel;


            [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
            void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
            {
            }

            [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
            private KernelSupportVectorMachine_2_13(SerializationInfo info, StreamingContext context)
            {
                info.GetValue("SupportVectorMachine+inputCount", out inputCount);
                info.GetValue("SupportVectorMachine+linkFunction", out linkFunction);
                info.GetValue("SupportVectorMachine+supportVectors", out supportVectors);
                info.GetValue("SupportVectorMachine+threshold", out threshold);
                info.GetValue("SupportVectorMachine+weights", out weights);
                info.GetValue("kernel", out kernel);
            }

            public static implicit operator KernelSupportVectorMachine(KernelSupportVectorMachine_2_13 obj)
            {
                var svm = new KernelSupportVectorMachine(obj.kernel, obj.inputCount)
                {
                    SupportVectors = obj.supportVectors,
                    Weights = obj.weights,
                    Threshold = obj.threshold,
                };

                var fn = obj.linkFunction as LogLinkFunction;
                if (fn != null)
                {
                    svm.Weights.Multiply(fn.B, result: svm.Weights);
                    svm.Threshold = svm.Threshold * fn.B + fn.A;
                    svm.IsProbabilistic = true;
                }

                return svm;
            }
        }

#pragma warning restore 0169
#pragma warning restore 0649
#endif
#endregion

        /// <summary>
        ///   Obsolete.
        /// </summary>
        public override object Clone()
        {
            var clone = new KernelSupportVectorMachine(Kernel, NumberOfInputs);
            clone.SupportVectors = SupportVectors.MemberwiseClone();
            clone.IsProbabilistic = IsProbabilistic;
            clone.Weights = (double[])Weights.Clone();
            clone.Threshold = Threshold;
            return clone;
        }
    }

}
