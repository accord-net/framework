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
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Accord.Compat;
    using System.Threading.Tasks;

    /// <summary>
    ///   Discovers the parameters of a <see cref="IUnivariateDistribution">univariate probability 
    ///   distribution</see> and helps determine their range and whether then can be constructed 
    ///   automatically from their indicated parameter ranges.
    /// </summary>
    /// 
    public class UnivariateDistributionInfo : DistributionInfo
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="UnivariateDistributionInfo"/> class.
        /// </summary>
        /// 
        /// <param name="type">The type for the distribution.</param>
        /// 
        public UnivariateDistributionInfo(Type type)
            : base(type)
        {
        }

        /// <summary>
        ///   Gets an array containing all univariate distributions 
        ///   by inspecting Accord.NET assemblies using reflection.
        /// </summary>
        /// 
        public static IEnumerable<UnivariateDistributionInfo> GetUnivariateDistributions(bool buildableOnly = false)
        {
            var distributions = GetDistributionsInheritingFromBaseType(typeof(IUnivariateDistribution))
                .Select(p => new UnivariateDistributionInfo(p));

            if (buildableOnly)
                distributions = distributions.Where(d => d.IsBuildable);

            return distributions;
        }

        /// <summary>
        ///   Creates a new instance of the distribution using the given arguments.
        /// </summary>
        /// 
        /// <param name="arguments">The arguments to be passed to the distribution's constructor.</param>
        /// 
        public IUnivariateDistribution CreateInstance(Dictionary<DistributionParameterInfo, object> arguments)
        {
            IUnivariateDistribution distribution = null;

            var parameters = new object[arguments.Count];
            foreach (KeyValuePair<DistributionParameterInfo, object> p in arguments)
            {
                Type paramType = p.Key.ParameterInfo.ParameterType;
#if NETSTANDARD
                if (paramType.GetTypeInfo().IsEnum)
#else
                if (paramType.IsEnum)
#endif
                {
                    int i = (int)Convert.ChangeType(p.Value, typeof(int));
                    object v = Enum.ToObject(paramType, i);
                    parameters[p.Key.Position] = v;
                }
                else
                {
                    parameters[p.Key.Position] = Convert.ChangeType(p.Value, paramType);
                }
            }

            distribution = (IUnivariateDistribution)Activator.CreateInstance(this.DistributionType, parameters);

            return distribution;
        }

        /// <summary>
        ///   Creates a new instance of the distribution using default arguments, 
        ///   if the distribution declared them using parameter attributes.
        /// </summary>
        /// 
        public static T CreateInstance<T>()
        {
            var info = new UnivariateDistributionInfo(typeof(T));

            var ctor = info.GetConstructors()
                .Where(x => x.IsBuildable)
                .OrderBy(x => x.GetParameters().Length)
                .First();

            // Build the argument list
            var arguments = new Dictionary<DistributionParameterInfo, object>();

            // Select the minimum value for the parameters
            foreach (var parameter in ctor.GetParameters())
                arguments[parameter] = parameter.DefaultValue;

            return (T)info.CreateInstance(arguments);
        }
    }

}
