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
    ///   Common interface for generative binary classifiers. A binary classifier can
    ///   predict whether or not an instance belongs to a class, while at the same time
    ///   being able to provide the probability of this sample belonging to the positive
    ///   class.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    /// 
    public interface IBinaryLikelihoodClassifier<TInput> : 
        IBinaryScoreClassifier<TInput>,
        IMulticlassOutLikelihoodClassifier<TInput, bool>,
        IMulticlassLikelihoodClassifier<TInput>
    {
        /// <summary>
        ///   Views this instance as a multi-class generative classifier,
        ///   giving access to more advanced methods, such as the prediction
        ///   of integer labels.
        /// </summary>
        /// 
        /// <returns>This instance seen as an <see cref="IMulticlassLikelihoodClassifier{TInput}"/>.</returns>
        /// 
        new IMulticlassLikelihoodClassifier<TInput> ToMulticlass();

        /// <summary>
        ///   Views this instance as a multi-class generative classifier,
        ///   giving access to more advanced methods, such as the prediction
        ///   of integer labels.
        /// </summary>
        /// 
        /// <returns>This instance seen as an <see cref="IMulticlassLikelihoodClassifier{TInput}"/>.</returns>
        /// 
        new IMulticlassLikelihoodClassifier<TInput, T> ToMulticlass<T>();

        //new IMultilabelGenerativeClassifier<TInput> ToMultilabel();
    }
   
}
