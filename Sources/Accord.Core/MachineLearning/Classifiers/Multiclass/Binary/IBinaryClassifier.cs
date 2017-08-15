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
    /// <typeparam name="TInput">The data type for the input data. Default is double[].</typeparam>
    /// 
    public interface IBinaryClassifier<in TInput> :
        IMulticlassClassifier<TInput>,
        IClassifier<TInput, bool>,
        IClassifier<TInput, int>,
        IClassifier<TInput, double>
    {
        /// <summary>
        ///   Views this instance as a multi-class classifier,
        ///   giving access to more advanced methods, such as the prediction
        ///   of integer labels.
        /// </summary>
        /// 
        /// <returns>This instance seen as an <see cref="IMulticlassClassifier{TInput}"/>.</returns>
        /// 
        IMulticlassClassifier<TInput> ToMulticlass();

        /// <summary>
        ///   Views this instance as a multi-class classifier,
        ///   giving access to more advanced methods, such as the prediction
        ///   of integer labels.
        /// </summary>
        /// 
        /// <returns>This instance seen as an <see cref="IMulticlassClassifier{TInput}"/>.</returns>
        /// 
        IMulticlassClassifier<TInput, T> ToMulticlass<T>();
    }

    /// <summary>
    ///   Common interface for classification models. Classification models
    ///   learn how to produce a class-label (or a set of class labels) <c>y</c>
    ///   from an input vector <c>x</c>.
    /// </summary>
    /// 
    public interface IBinaryClassifier :
        IBinaryClassifier<double[]>,
        IBinaryClassifier<float[]>,
        IBinaryClassifier<int[]>
    {
    }
}
