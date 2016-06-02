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

    /// <summary>
    ///   Common interface for support vector machines.
    /// </summary>
    /// <summary>
    ///   Linear support vector machine.
    /// </summary>
    /// 
    [Serializable]
    [SerializationBinder(typeof(SupportVectorMachine.SupportVectorMachineBinder))]
    public class SupportVectorMachine : SupportVectorMachine<Linear>,
        IBinaryClassifier<double[]>, ISupportVectorMachine<double[]>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SupportVectorMachine"/> class.
        /// </summary>
        /// 
        /// <param name="inputs">The number of inputs for this machine.</param>
        /// 
        public SupportVectorMachine(int inputs)
            : base(inputs, new Linear())
        {
        }


        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            var clone = new SupportVectorMachine(NumberOfInputs) { Kernel = Kernel };
            clone.SupportVectors = SupportVectors.MemberwiseClone();
            clone.IsProbabilistic = IsProbabilistic;
            clone.Weights = (double[])Weights.Clone();
            clone.Threshold = Threshold;
            return clone;
        }

        #region Serialization backwards compatibility

        internal class SupportVectorMachineBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                AssemblyName name = new AssemblyName(assemblyName);

                if (name.Version < new Version(3, 1, 0))
                {
                    if (typeName == "Accord.MachineLearning.VectorMachines.SupportVectorMachine")
                        return typeof(SupportVectorMachine_2_13);
                }

                return null;
            }
        }

#pragma warning disable 0169
#pragma warning disable 0649

        [Serializable]
        internal class SupportVectorMachine_2_13 : ISerializable
        {
            public int inputCount;
            public double[][] supportVectors;
            public double[] weights;
            public double threshold;
            public ILinkFunction linkFunction;


            [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
            void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
            {
            }

            [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
            private SupportVectorMachine_2_13(SerializationInfo info, StreamingContext context)
            {
                info.GetValue("inputCount", out inputCount);
                info.GetValue("linkFunction", out linkFunction);
                info.GetValue("supportVectors", out supportVectors);
                info.GetValue("threshold", out threshold);
                info.GetValue("weights", out weights);
            }


            public static implicit operator SupportVectorMachine(SupportVectorMachine_2_13 obj)
            {
                var svm = new SupportVectorMachine(obj.inputCount)
                {
                    SupportVectors = obj.supportVectors,
                    Weights = obj.weights,
                    Threshold = obj.threshold
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

        #endregion

    }

}
