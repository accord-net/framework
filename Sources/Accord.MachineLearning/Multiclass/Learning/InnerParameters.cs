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

namespace Accord.MachineLearning
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Accord.MachineLearning;

    /// <summary>
    ///   Parameters for learning a binary decision model. An object of this class is passed by
    ///   <see cref="OneVsRestLearning{TBinary, TModel}"/> or <see cref="OneVsOneLearning{TBinary, TModel}"/> 
    ///   to instruct how binary learning algorithms should create their binary classifiers.
    /// </summary>
    /// 
    /// <typeparam name="TBinary">The type of the binary model to be learned.</typeparam>
    /// <typeparam name="TInput">The input type for the binary classifiers.</typeparam>
    /// 
    public class InnerParameters<TBinary, TInput>
    {
        /// <summary>
        ///   Gets or sets the binary model to be learned.
        /// </summary>
        /// 
        public TBinary Model { get; private set; }

        /// <summary>
        ///   Gets or sets the input data that should be used to train the classifier.
        /// </summary>
        /// 
        public TInput[] Inputs { get; private set; }

        /// <summary>
        ///   Gets or sets the output data that should be used to train the classifier.
        /// </summary>
        /// 
        public bool[] Outputs { get; private set; }

        /// <summary>
        ///   Gets or sets the class pair that the classifier will be designated
        ///   to learn. For <see cref="OneVsRest{TModel, TInput}"/> classifiers, the first element
        ///   in the pair designates the class to be learned against all others.
        /// </summary>
        /// 
        public ClassPair Pair { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InnerParameters{TBinary, TInput}"/> class.
        /// </summary>
        /// 
        /// <param name="model">The binary model to be learned.</param>
        /// <param name="inputs">The inputs to be used.</param>
        /// <param name="outputs">The outputs to be used.</param>
        /// <param name="pair">The class labels for the problem to be learned.</param>
        /// 
        public InnerParameters(TBinary model, TInput[] inputs, bool[] outputs, ClassPair pair)
        {
            Model = model;
            Inputs = inputs;
            Outputs = outputs;
            Pair = pair;
        }
    }
}