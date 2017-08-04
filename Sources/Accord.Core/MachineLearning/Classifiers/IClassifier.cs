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

    /// <summary>
    ///   Common interface for classification models. Classification models
    ///   learn how to produce a class-label (or a set of class labels) <c>y</c>
    ///   from an input vector <c>x</c>.
    /// </summary>
    /// 
    public interface IClassifier
    {
        /// <summary>
        ///   Gets or sets the number of classes expected and recognized by the classifier.
        /// </summary>
        /// 
        int NumberOfClasses
        {
            get;
            set;
        }
    }

    /// <summary>
    ///   Common interface for classification models. Classification models
    ///   learn how to produce a class-label (or a set of class labels) <c>y</c>
    ///   from an input vector <c>x</c>.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    /// <typeparam name="TClasses">The data type for the class labels. Default is int.</typeparam>
    /// 
    public interface IClassifier<in TInput, TClasses> : IClassifier,
        ITransform<TInput, TClasses>
    {

        /// <summary>
        ///   Computes a class-label decision for a given <paramref name="input"/>.
        /// </summary>
        /// 
        /// <param name="input">The input vector that should be classified into
        ///   one of the <see cref="ITransform.NumberOfOutputs"/> possible classes.</param>
        /// 
        /// <returns>A class-label that best described <paramref name="input"/> according
        /// to this classifier.</returns>
        /// 
        TClasses Decide(TInput input);

        /// <summary>
        ///   Computes class-label decisions for each vector in the given <paramref name="input"/>.
        /// </summary>
        /// 
        /// <param name="input">The input vectors that should be classified into
        ///   one of the <see cref="ITransform.NumberOfOutputs"/> possible classes.</param>
        /// 
        /// <returns>The class-labels that best describe each <paramref name="input"/> 
        ///   vectors according to this classifier.</returns>
        /// 
        TClasses[] Decide(TInput[] input);

        /// <summary>
        ///   Computes class-label decisions for each vector in the given <paramref name="input"/>.
        /// </summary>
        /// 
        /// <param name="input">The input vectors that should be classified into
        ///   one of the <see cref="ITransform.NumberOfOutputs"/> possible classes.</param>
        /// <param name="result">The location where to store the class-labels.</param>
        /// 
        /// <returns>The class-labels that best describe each <paramref name="input"/> 
        ///   vectors according to this classifier.</returns>
        /// 
        TClasses[] Decide(TInput[] input, TClasses[] result);

    }

    // TODO: Incorporate this interface in the current architecture
    //       in the hope it can make learning easier.
    /*
    public interface IClassifier<in TInput, TClasses, TLearning> : 
        IClassifier<TInput, TClasses>
        where TLearning : class, new()
    {
        TLearning Fit(TInput[] input, TClasses[] outputs, double[] weights = null, TLearning teacher = null);
    }
     */
}
