// Accord Core Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
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
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class PositiveAttribute : RealAttribute
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="PositiveAttribute"/> class.
        /// </summary>
        /// 
        public PositiveAttribute()
            : base(double.Epsilon, double.MaxValue) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="PositiveAttribute"/> class.
        /// </summary>
        /// 
        public PositiveAttribute(double minimum)
            : base(minimum, double.MaxValue) { }
    }

    /// <summary>
    ///   Specifies that an argument, in a method or function,
    ///   must be lesser than zero.
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class NegativeAttribute : RealAttribute
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="NegativeAttribute"/> class.
        /// </summary>
        /// 
        public NegativeAttribute()
            : base(double.MinValue, -double.Epsilon) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="NegativeAttribute"/> class.
        /// </summary>
        /// 
        public NegativeAttribute(double maximum)
            : base(double.MinValue, maximum) { }
    }

    /// <summary>
    ///   Specifies that an argument, in a method or function,
    ///   must be lesser than or equal to zero.
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class NonpositiveAttribute : RealAttribute
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="NonpositiveAttribute"/> class.
        /// </summary>
        /// 
        public NonpositiveAttribute()
            : base(double.MinValue, 0) { }
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
        public NonnegativeAttribute()
            : base(0, double.MaxValue) { }
    }

    /// <summary>
    ///   Specifies that an argument, in a method or function,
    ///   must be real (double).
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class RealAttribute : RangeAttribute
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="RealAttribute"/> class.
        /// </summary>
        /// 
        public RealAttribute()
            : base(double.MinValue, double.MaxValue) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="RealAttribute"/> class.
        /// </summary>
        /// 
        public RealAttribute(double minimum, double maximum)
            : base(minimum, maximum) { }

        /// <summary>
        ///   Gets the minimum allowed field value.
        /// </summary>
        /// 
        public new double Minimum { get { return (double)base.Minimum; } }

        /// <summary>
        ///   Gets the maximum allowed field value.
        /// </summary>
        /// 
        public new double Maximum { get { return (double)base.Maximum; } }
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
        public UnitAttribute()
            : base(0, 1) { }
    }

}
