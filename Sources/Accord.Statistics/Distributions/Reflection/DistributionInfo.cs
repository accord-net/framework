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
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Distributions.Univariate;
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
    ///   Discovers the parameters of a <see cref="IDistribution">probability distribution</see>
    ///   and helps determine their range and whether then can be constructed automatically from
    ///   their indicated parameter ranges.
    /// </summary>
    /// 
    public class DistributionInfo
    {
        private bool? isBuildable;

        /// <summary>
        ///   Gets the distribution's type information.
        /// </summary>
        /// 
        public Type DistributionType { get; protected set; }

        /// <summary>
        ///   Gets the name of this distribution in a more human-readable form.
        /// </summary>
        /// 
        public string Name { get; protected set; }

        /// <summary>
        ///   Gets a value indicating whether it is possible to discover enough
        ///   information about this constructor such that the distribution can
        ///   be constructed using reflection.
        /// </summary>
        /// 
        /// <value><c>true</c> if this instance is buildable; otherwise, <c>false</c>.</value>
        /// 
        public bool IsBuildable
        {
            get
            {
                if (isBuildable == null)
                    isBuildable = GetConstructors().Any(x => x.IsBuildable);
                return isBuildable.Value;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the distribution modeled by <see cref="DistributionType"/> is a discrete-valued distribution.
        ///   Discrete distributions are assumed to inherit from <see cref="UnivariateDiscreteDistribution"/> or 
        ///   <see cref="MultivariateDiscreteDistribution"/>.
        /// </summary>
        /// 
        public bool IsDiscrete
        {
            get
            {
#if NETSTANDARD
                var distributionTypeInfo = DistributionType.GetTypeInfo();
                return typeof(UnivariateDiscreteDistribution).GetTypeInfo().IsAssignableFrom(distributionTypeInfo)
                  || typeof(MultivariateDiscreteDistribution).GetTypeInfo().IsAssignableFrom(distributionTypeInfo);
#else
                return typeof(UnivariateDiscreteDistribution).IsAssignableFrom(DistributionType)
                  || typeof(MultivariateDiscreteDistribution).IsAssignableFrom(DistributionType);
#endif
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the distribution modeled by <see cref="DistributionType"/> is a continuous-valued distribution.
        ///   Discrete distributions are assumed to inherit from <see cref="UnivariateContinuousDistribution"/> or 
        ///   <see cref="MultivariateContinuousDistribution"/>.
        /// </summary>
        /// 
        public bool IsContinuous
        {
            get
            {
#if NETSTANDARD
                var distributionTypeInfo = DistributionType.GetTypeInfo();
                return typeof(UnivariateContinuousDistribution).GetTypeInfo().IsAssignableFrom(distributionTypeInfo)
                  || typeof(MultivariateContinuousDistribution).GetTypeInfo().IsAssignableFrom(distributionTypeInfo);
#else
                return typeof(UnivariateContinuousDistribution).IsAssignableFrom(DistributionType)
                  || typeof(MultivariateContinuousDistribution).IsAssignableFrom(DistributionType);
#endif
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the distribution modeled by <see cref="DistributionType"/> is univariate.
        ///   A distribution is assumed to be univariate if it implements the <see cref="IUnivariateDistribution"/> interface.
        /// </summary>
        /// 
        public bool IsUnivariate
        {
#if NETSTANDARD
            get { return typeof(IUnivariateDistribution).GetTypeInfo().IsAssignableFrom(DistributionType.GetTypeInfo()); }
#else
            get { return typeof(IUnivariateDistribution).IsAssignableFrom(DistributionType); }
#endif
        }

        /// <summary>
        ///   Gets a value indicating whether the distribution modeled by <see cref="DistributionType"/> is multivariate.
        ///   A distribution is assumed to be univariate if it implements the <see cref="IMultivariateDistribution"/> interface.
        /// </summary>
        /// 
        public bool IsMultivariate
        {
#if NETSTANDARD
            get { return typeof(IMultivariateDistribution).GetTypeInfo().IsAssignableFrom(DistributionType.GetTypeInfo()); }
#else
            get { return typeof(IMultivariateDistribution).IsAssignableFrom(DistributionType); }
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributionInfo" /> class.
        /// </summary>
        /// 
        /// <param name="type">The type for the distribution.</param>
        /// 
        public DistributionInfo(Type type)
        {
            Type baseType = typeof(IDistribution);
#if NETSTANDARD
            var typeDesc = type.GetTypeInfo();
            if (!baseType.GetTypeInfo().IsAssignableFrom(typeDesc) || typeDesc.IsAbstract || typeDesc.IsInterface)
                throw new ArgumentException("type");
#else
            if (!baseType.IsAssignableFrom(type) || type.IsAbstract || type.IsInterface)
                throw new ArgumentException("type");
#endif
            this.DistributionType = type;
            this.Name = GetDistributionName(type);
        }

        /// <summary>
        ///   Gets the public constructors of the distribution.
        /// </summary>
        /// 
        public DistributionConstructorInfo[] GetConstructors()
        {
#if NETSTANDARD
            var constructors = new List<DistributionConstructorInfo>();
            foreach (ConstructorInfo ctor in DistributionType.GetTypeInfo().DeclaredConstructors)
                constructors.Add(new DistributionConstructorInfo(ctor));
#else
            var constructors = new List<DistributionConstructorInfo>();
            foreach (ConstructorInfo ctor in DistributionType.GetConstructors())
                constructors.Add(new DistributionConstructorInfo(ctor));
#endif

            return constructors.ToArray();
        }

        /// <summary>
        ///   Gets the fitting options object that are expected by the distribution, if any. An
        ///   Accord.NET distribution object can be fitted to a set of observed values. However,
        ///   some distributions have different settings on how this fitting can be done. This
        ///   function creates an object that contains those possible settings that can be configured
        ///   for a given distribution type.
        /// </summary>
        /// 
        public IFittingOptions GetFittingOptions()
        {
            return GetFittingOptions(DistributionType);
        }

        /// <summary>
        ///   Gets the name of the distribution modeled by a given Accord.NET type. 
        ///   The name is returned in a normalized form (i.e. given a type whose name
        ///   is  NormalDistribution, the function would return "Normal").
        /// </summary>
        /// 
        public static string GetDistributionName(Type type)
        {
            // Extract the real distribution name from the class name
            string name = type.Name;

            if (name.Contains('`'))
                name = name.Remove(name.IndexOf("`"));

            // Remove the trailing "Distribution" from the name
            if (name.EndsWith("Distribution"))
                name = name.Remove(name.IndexOf("Distribution"));

            return name.Trim();
        }

        /// <summary>
        ///   Gets the fitting options object that are expected by one distribution, if any. An
        ///   Accord.NET distribution object can be fit to a set of observed values. However,
        ///   some distributions have different settings on how this fitting can be done. This
        ///   function creates an object that contains those possible settings that can be configured
        ///   for a given distribution type.
        /// </summary>
        /// 
        public static IFittingOptions GetFittingOptions(Type type)
        {
            // Try to create a fitting options object
            var fittingOptions = typeof(IFittingOptions);
#if NETSTANDARD
            var interfaces = type.GetTypeInfo().ImplementedInterfaces.Select(t => t.GetTypeInfo())
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IFittableDistribution<,>));

            var fittingOptionsInfo = fittingOptions.GetTypeInfo();
            foreach (TypeInfo i in interfaces)
                foreach (Type arg in i.GenericTypeArguments)
                    if (fittingOptionsInfo.IsAssignableFrom(arg.GetTypeInfo()) && arg != fittingOptions)
                        return (IFittingOptions)Activator.CreateInstance(arg);
#else
            var interfaces = type.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IFittableDistribution<,>));

            foreach (Type i in interfaces)
                foreach (Type arg in i.GetGenericArguments())
                    if (fittingOptions.IsAssignableFrom(arg) && arg != fittingOptions)
                        return (IFittingOptions)Activator.CreateInstance(arg);
#endif
            return null;
        }

        /// <summary>
        ///   Gets the fitting options object that are expected by one distribution, if any. An
        ///   Accord.NET distribution object can be fit to a set of observed values. However,
        ///   some distributions have different settings on how this fitting can be done. This
        ///   function creates an object that contains those possible settings that can be configured
        ///   for a given distribution type.
        /// </summary>
        /// 
        public static IFittingOptions GetFittingOptions<T>()
        {
            return GetFittingOptions(typeof(T));
        }

        internal static Type[] GetDistributionsInheritingFromBaseType(Type baseType)
        {
            // This function iterates the Accord.Statistics assembly looking for
            // classes that are concrete (not abstract) and that implement the
            // given interface. 

            // Get all univariate distributions in Accord.NET:
#if NETSTANDARD1_4
            var baseInfo = baseType.GetTypeInfo();
            var distributions = baseInfo.Assembly.DefinedTypes
                .Where(p => baseInfo.IsAssignableFrom(p) && !p.IsAbstract && !p.IsInterface)
                .Select(p => p.AsType());
#else
            var distributions = Assembly.GetAssembly(baseType).GetTypes().Where(p =>
                baseType.IsAssignableFrom(p) && !p.IsAbstract && !p.IsInterface);
#endif

            return distributions.ToArray();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return Name;
        }

    }
}
