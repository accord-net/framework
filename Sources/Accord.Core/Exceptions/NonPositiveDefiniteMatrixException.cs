// Accord Core Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
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

namespace Accord
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///   Non-Positive Definite Matrix Exception.
    /// </summary>
    /// 
    /// <remarks><para>The non-positive definite matrix exception is thrown in cases where a method 
    /// expects a matrix to have only positive eigenvalues, such when dealing with covariance matrices.</para>
    /// </remarks>
    /// 
    [Serializable]
    public class NonPositiveDefiniteMatrixException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NonPositiveDefiniteMatrixException"/> class.
        /// </summary>
        public NonPositiveDefiniteMatrixException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NonPositiveDefiniteMatrixException"/> class.
        /// </summary>
        /// 
        /// <param name="message">Message providing some additional information.</param>
        /// 
        public NonPositiveDefiniteMatrixException(string message) :
            base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NonPositiveDefiniteMatrixException"/> class.
        /// </summary>
        /// 
        /// <param name="message">Message providing some additional information.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        /// 
        public NonPositiveDefiniteMatrixException(string message, Exception innerException) :
            base(message, innerException) { }


        /// <summary>
        /// Initializes a new instance of the <see cref="NonPositiveDefiniteMatrixException"/> class.
        /// </summary>
        /// 
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        /// 
        protected NonPositiveDefiniteMatrixException(SerializationInfo info, StreamingContext context) :
            base(info, context) { }

    }
}
