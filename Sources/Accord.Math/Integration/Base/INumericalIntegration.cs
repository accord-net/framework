// Accord Math Library
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

namespace Accord.Math.Integration
{
    using System;
    using Accord.Compat;

    /// <summary>
    ///   Common interface for numeric integration methods.
    /// </summary>
    /// 
    public interface INumericalIntegration : ICloneable
    {

        /// <summary>
        ///   Gets the numerically computed result of the 
        ///   definite integral for the specified function.
        /// </summary>
        /// 
        double Area { get; }

        /// <summary>
        ///   Computes the area of the function under the selected 
        ///   range. The computed value will be available at this 
        ///   class's <see cref="Area"/> property.
        /// </summary>
        /// 
        /// <returns>True if the integration method succeeds, false otherwise.</returns>
        /// 
        bool Compute();

    }

    /// <summary>
    ///   Common interface for numeric integration methods.
    /// </summary>
    /// 
    public interface INumericalIntegration<TCode> : INumericalIntegration
        where TCode : struct
    {
        /// <summary>
        ///   Get the exit code returned in the last call to the
        ///   <see cref="INumericalIntegration.Compute()"/> method.
        /// </summary>
        /// 
        TCode Status { get; }
    }
}
