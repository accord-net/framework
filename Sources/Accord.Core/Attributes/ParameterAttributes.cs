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
    using System.ComponentModel.DataAnnotations;
    using System;

    /// <summary>
    ///   Specifies that an argument, in a method or function,
    ///   must be greater than zero.
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class PositiveAttribute : RangeAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PositiveAttribute"/> class.
        /// </summary>
        /// 
        public PositiveAttribute() : base(double.Epsilon, double.MaxValue) { }
    }

    /// <summary>
    ///   Specifies that an argument, in a method or function,
    ///   must be lesser than zero.
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class NegativeAttribute : RangeAttribute
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="NegativeAttribute"/> class.
        /// </summary>
        /// 
        public NegativeAttribute() : base(double.MinValue, -double.Epsilon) { }
    }

    /// <summary>
    ///   Specifies that an argument, in a method or function,
    ///   must be lesser than or equal to zero.
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class NonpositiveAttribute : RangeAttribute
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="NonpositiveAttribute"/> class.
        /// </summary>
        /// 
        public NonpositiveAttribute() : base(double.MinValue, 0) { }
    }

    /// <summary>
    ///   Specifies that an argument, in a method or function,
    ///   must be greater than or equal to zero.
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class NonnegativeAttribute : RangeAttribute
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="NonnegativeAttribute"/> class.
        /// </summary>
        /// 
        public NonnegativeAttribute() : base(0, double.MaxValue) { }
    }

    /// <summary>
    ///   Specifies that an argument, in a method or function,
    ///   must be real (double).
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class RealAttribute : RangeAttribute
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="RealAttribute"/> class.
        /// </summary>
        /// 
        public RealAttribute() : base(double.MinValue, double.MaxValue) { }
    }

    /// <summary>
    ///   Specifies that an argument, in a method or function,
    ///   must be real between 0 and 1.
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class UnitAttribute : RangeAttribute
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="UnitAttribute"/> class.
        /// </summary>
        /// 
        public UnitAttribute() : base(0, 1) { }
    }

    /// <summary>
    ///   Specifies that an argument, in a method or function,
    ///   must be an integer bigger than zero.
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class PositiveIntegerAttribute : RangeAttribute
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="PositiveIntegerAttribute"/> class.
        /// </summary>
        /// 
        public PositiveIntegerAttribute() : base(1, int.MaxValue) { }
    }

    /// <summary>
    ///   Specifies that an argument, in a method or function,
    ///   must be an integer less than zero.
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class NegativeIntegerAttribute : RangeAttribute
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="NegativeIntegerAttribute"/> class.
        /// </summary>
        /// 
        public NegativeIntegerAttribute() : base(int.MinValue, -1) { }
    }

    /// <summary>
    ///   Specifies that an argument, in a method or function,
    ///   must be an integer smaller than or equal to zero.
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class NonpositiveIntegerAttribute : RangeAttribute
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="NonpositiveIntegerAttribute"/> class.
        /// </summary>
        /// 
        public NonpositiveIntegerAttribute() : base(int.MinValue, 0) { }
    }

    /// <summary>
    ///   Specifies that an argument, in a method or function,
    ///   must be an integer bigger than or equal to zero.
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class NonnegativeIntegerAttribute : RangeAttribute
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="NonnegativeIntegerAttribute"/> class.
        /// </summary>
        /// 
        public NonnegativeIntegerAttribute() : base(0, int.MaxValue) { }
    }

    /// <summary>
    ///   Specifies that an argument, in a method or function,
    ///   must be an integer.
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class IntegerAttribute : RangeAttribute
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="IntegerAttribute"/> class.
        /// </summary>
        /// 
        public IntegerAttribute() : base(int.MinValue, int.MaxValue) { }
    }
}
