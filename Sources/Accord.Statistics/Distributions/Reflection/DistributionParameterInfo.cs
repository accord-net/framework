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

namespace Accord.Statistics.Distributions.Reflection
{
    using Accord.Statistics.Distributions.Univariate;
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;
    using Accord.Compat;

    /// <summary>
    ///   Discovers the parameters of a <see cref="IDistribution">probability distribution</see>
    ///   and helps determine their range and whether then can be constructed automatically from
    ///   their indicated parameter ranges.
    /// </summary>
    /// 
    /// <seealso cref="DistributionConstructorInfo"/>
    /// 
    public class DistributionParameterInfo
    {
        /// <summary>
        ///   Gets the reflection parameter information.
        /// </summary>
        /// 
        public ParameterInfo ParameterInfo { get; private set; }

        /// <summary>
        ///   Gets the name of this parameter.
        /// </summary>
        /// 
        public string Name { get { return ParameterInfo.Name; } }

        /// <summary>
        ///   Gets the position of this parameter in the declaration of the constructor.
        /// </summary>
        /// 
        public int Position { get { return ParameterInfo.Position; } }


        /// <summary>
        ///   Gets the range of valid values for this parameter (i.e. in a <see cref="NormalDistribution"/>,
        ///   the standard deviation parameter cannot be negative).
        /// </summary>
        /// 
        public DoubleRange Range { get; private set; }

        /// <summary>
        ///   Gets the default value for this parameter (i.e. in a <see cref="NormalDistribution"/>,
        ///   the default value for the mean is 0).
        /// </summary>
        /// 
        public double DefaultValue { get; private set; }

        /// <summary>
        ///   Gets a value indicating whether it is possible to discover enough
        ///   information about this constructor such that the distribution can
        ///   be constructed using reflection.
        /// </summary>
        /// 
        /// <value><c>true</c> if this instance is buildable; otherwise, <c>false</c>.</value>
        /// 
        public bool IsBuildable { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributionParameterInfo" /> class.
        /// </summary>
        /// 
        /// <param name="parameterInfo">The parameter information.</param>
        /// 
        public DistributionParameterInfo(ParameterInfo parameterInfo)
        {
            this.ParameterInfo = parameterInfo;
            this.IsBuildable = true;

            DoubleRange range;
            if (!TryGetRange(parameterInfo, out range))
                this.IsBuildable = false;

            double value;
            if (!TryGetDefault(parameterInfo, out value))
                this.IsBuildable = false;

            this.Range = range;
            this.DefaultValue = value;
        }


        /// <summary>
        ///   Tries to get the valid range of a distribution's parameter.
        /// </summary>
        /// 
        public static bool TryGetRange(ParameterInfo parameter, out DoubleRange range)
        {
            range = new DoubleRange(0, 0);

            var attrb = parameter.GetCustomAttribute<RangeAttribute>();
            if (attrb == null)
            {
#if NETSTANDARD
                if (!parameter.ParameterType.GetTypeInfo().IsEnum)
#else
                if (!parameter.ParameterType.IsEnum)
#endif
                    return false;

                Array enumValues = Enum.GetValues(parameter.ParameterType);
                int[] intValues = new int[enumValues.Length];
                for (int i = 0; i < intValues.Length; i++)
                    intValues[i] = (int)enumValues.GetValue(i);

                Array.Sort(intValues);
                range = new DoubleRange(intValues[0], intValues[intValues.Length - 1]);
                return true;
            }

            double min = (double)Convert.ChangeType(attrb.Minimum, typeof(double));
            double max = (double)Convert.ChangeType(attrb.Maximum, typeof(double));

            range = new DoubleRange(min, max);

            return true;
        }

        /// <summary>
        ///   Tries to get the default value of a distribution's parameter.
        /// </summary>
        /// 
        public static bool TryGetDefault(ParameterInfo parameter, out double value)
        {
            var attrb = parameter.GetCustomAttribute<DefaultValueAttribute>();

            if (attrb != null)
            {
                value = (double)Convert.ChangeType(attrb.Value, typeof(double));
                return true;
            }

            DoubleRange range;
            if (!TryGetRange(parameter, out range))
            {
                value = 0;
                return false;
            }

            var a = parameter.GetCustomAttribute<RangeAttribute>();

            value = 0;

            if (a is PositiveAttribute || a is PositiveIntegerAttribute)
                value = 1;

            else if (a is NegativeAttribute || a is NegativeIntegerAttribute)
                value = -1;

            else if (a is UnitAttribute)
                value = 0.5;


            if (value < range.Min)
                value = range.Min;

            if (value > range.Max)
                value = range.Max;

            return true;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return String.Format("{0} ({1}-{2}, default: {3})", this.Name, this.Range.Min, this.Range.Max, this.DefaultValue);
        }

    }
}
