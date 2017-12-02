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
    ///   Common interface for multi-class models. Classification models
    ///   learn how to produce a class-label <c>y</c> from an input vector <c>x</c>.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    /// <typeparam name="TClasses">The data type for the class labels. Default is int.</typeparam>
    ///
    public interface IMulticlassClassifier<in TInput, TClasses> :
        IClassifier<TInput, TClasses>
    {
        
    }

    /// <summary>
    ///   Common interface for multi-class models. Classification models
    ///   learn how to produce a class-label <c>y</c> from an input vector <c>x</c>.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    /// 
    public interface IMulticlassClassifier<in TInput> :
        IMultilabelClassifier<TInput>,
        IMulticlassClassifier<TInput, int>,
        IMulticlassClassifier<TInput, double>
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
        new int Decide(TInput input);

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
        new int[] Decide(TInput[] input);


        /// <summary>
        ///   Views this instance as a multi-label classifier,
        ///   giving access to more advanced methods, such as the prediction
        ///   of one-hot vectors.
        /// </summary>
        /// 
        /// <returns>This instance seen as an <see cref="IMultilabelLikelihoodClassifier{TInput}"/>.</returns>
        /// 
        IMultilabelClassifier<TInput> ToMultilabel();
    }

    /// <summary>
    ///   Common interface for multi-class models. Classification models
    ///   learn how to produce a class-label <c>y</c> from an input vector <c>x</c>.
    /// </summary>
    /// 
    public interface IMulticlassClassifier :
        IMulticlassClassifier<double[]>,
        IMulticlassClassifier<int[]>,
        IMulticlassClassifier<float[]>
    {

    }

}
