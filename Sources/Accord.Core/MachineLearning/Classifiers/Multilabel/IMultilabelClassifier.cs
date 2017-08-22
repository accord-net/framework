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
    ///   Common interface for multi-label classifiers. A multi-label classifier can
    ///   predict the occurrence of multiple class labels at once.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    /// <typeparam name="TClasses">The data type for the class labels. Default is int[].</typeparam>
    /// 
    public interface IMultilabelClassifier<in TInput, TClasses> :
        IClassifier<TInput, TClasses>
    {
        /// <summary>
        ///   Computes class-label decisions for the given <paramref name="input"/>.
        /// </summary>
        /// 
        /// <param name="input">The input vectors that should be classified as
        ///   any of the <see cref="ITransform.NumberOfOutputs"/> possible classes.</param>
        /// <param name="result">The location where to store the class-labels.</param>
        /// 
        /// <returns>A set of class-labels that best describe the <paramref name="input"/> 
        ///   vectors according to this classifier.</returns>
        /// 
        TClasses Decide(TInput input, TClasses result);
    }

    /// <summary>
    ///   Common interface for multi-label classifiers. A multi-label classifier can
    ///   predict the occurrence of multiple class labels at once.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    /// 
    public interface IMultilabelClassifier<in TInput> :
        IMultilabelClassifier<TInput, int[]>,
        IMultilabelClassifier<TInput, bool[]>,
        IMultilabelClassifier<TInput, double[]>
    {
        ///// <summary>
        /////   Computes whether a class label applies to an <paramref name="input"/> vector.
        ///// </summary>
        ///// 
        ///// <param name="input">The input vectors that should be classified as
        /////   any of the <see cref="ITransform.NumberOfOutputs"/> possible classes.</param>
        ///// <param name="classIndex">The class label index to be tested.</param>
        ///// 
        ///// <returns>A boolean value indicating whether the given <paramref name="classIndex">
        ///// class label</paramref> applies to the <paramref name="input"/> vector.</returns>
        ///// 
        //bool Decide(TInput input, int classIndex);
    }

    /// <summary>
    ///   Common interface for multi-label classifiers. A multi-label classifier can
    ///   predict the occurrence of multiple class labels at once.
    /// </summary>
    /// 
    public interface IMultilabelClassifier :
        IMultilabelClassifier<int[]>,
        IMultilabelClassifier<float[]>,
        IMultilabelClassifier<double[]>
    {
    }
}
