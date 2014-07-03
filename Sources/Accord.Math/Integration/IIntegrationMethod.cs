// Accord Math Library
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

namespace Accord.Math.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using AForge;

    public interface IUnivariateIntegration : IIntegrationMethod
    {

        Func<double, double> Function { get; set; }
        DoubleRange Range { get; set; }

    }

    public interface IIntegrationMethod : ICloneable
    {

        double Area { get; }

        bool Compute();

    }

    public interface IIntegrationMethod<TCode> : IIntegrationMethod
        where TCode : struct
    {
        /// <summary>
        ///   Get the exit code returned in the last call to the
        ///   <see cref="IIntegrationMethod.Compute()"/> method.
        /// </summary>
        /// 
        TCode Status { get; }
    }
}
