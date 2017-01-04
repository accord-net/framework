// Accord Core Library
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

namespace Accord
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///   Specifies that an argument, in a method or function,
    ///   must be greater than zero.
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class SourceTypeAttribute : Attribute
    {
        /// <summary>
        ///   Gets or sets the type of the source.
        /// </summary>
        /// 
        /// <value>
        ///   The type of the source.
        /// </value>
        /// 
        Type SourceType { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SourceType"/> class.
        /// </summary>
        /// 
        public SourceTypeAttribute(Type type)
        {
            this.SourceType = type;
        }

    }

}
