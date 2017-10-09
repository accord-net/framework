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

#if !NETSTANDARD1_4
namespace Accord
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;
    using Accord.Compat;

    /// <summary>
    ///   Specifies a serialization surrogate to be used whenever a class is 
    ///   being deserialized by the framework. This can be used to ensure 
    ///   binary compatibility when the framework code changes.
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class SurrogateSelectorAttribute : Attribute
    {

        /// <summary>
        ///   The binder to be used for the class marked with this attribute.
        /// </summary>
        /// 
        public SurrogateSelector Selector { get; private set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SurrogateSelectorAttribute"/> class.
        /// </summary>
        /// 
        /// <param name="surrogateSelectorType">The surrogate selector to be used to deserialize objects of this type.</param>
        /// 
        public SurrogateSelectorAttribute(Type surrogateSelectorType)
        {
            Selector = (SurrogateSelector)Activator.CreateInstance(surrogateSelectorType);
        }

    }
}
#endif