// Accord Unit Tests
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

namespace Accord.Tests.Statistics
{
    using Accord.Statistics.Distributions.Univariate;
    using NUnit.Framework;
    using Accord.Statistics.Distributions;
    using System;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Math;
    using System.Linq;
    using System.Reflection;
    using System.Collections.Generic;
    using Accord.Statistics.Distributions.Multivariate;
    using System.ComponentModel;

    using RangeAttribute = System.ComponentModel.DataAnnotations.RangeAttribute;
    using Accord.Statistics.Distributions.Reflection;

    [TestFixture]
    public class UnivariateDistributionTest
    {

        public static IEnumerable<Type> GetDerivedConcreteTypes(Type baseType)
        {
#if NETCORE
            var distributions = Assembly
                 .GetEntryAssembly()
                 .GetReferencedAssemblies()
                 .Select(Assembly.Load)
                 .SelectMany(x => x.DefinedTypes)
                 .Where(p => baseType.IsAssignableFrom(p.AsType()) && !p.IsAbstract && !p.IsInterface)
                 .Select(p => p.AsType());
#else
            var distributions = Assembly.GetAssembly(baseType).GetTypes().Where(p =>
                baseType.IsAssignableFrom(p) && !p.IsAbstract && !p.IsInterface);
#endif
            return distributions;
        }

        private static string GetNames(Type baseType, out int count)
        {
            var distributions = GetDerivedConcreteTypes(baseType);

            var list = new List<String>();
            foreach (Type type in distributions)
                list.Add(type.Name.Replace("Distribution", ""));

            count = list.Count;

            return String.Join(", ", list.ToArray());
        }

        [Test]
        public void ParameterPropertyTest()
        {
            // Dynamically enumerate all univariate distributions and make
            // sure the distribution contains properties for every exposed
            // constructors' parameters

            var baseType = typeof(IUnivariateDistribution);

            // Get all univariate distributions in Accord.NET:
            var distributions = GetDerivedConcreteTypes(baseType);

            var check = new HashSet<Type>();

            foreach (Type type in distributions)
            {
                if (!typeof(IUnivariateDistribution).IsAssignableFrom(type))
                    continue;

                foreach (var ctor in type.GetConstructors())
                {
                    foreach (var param in ctor.GetParameters())
                    {
                        string distName = type.Name;
                        string paramName = param.Name;

                        // Search for a property with a similar name
                        var p = type.GetProperties().FirstOrDefault(
                            prop => cmp(distName, paramName, prop.Name));

                        bool found = p != null;

                        Assert.IsTrue(found);
                        check.Add(type);
                    }
                }
            }

            var checkedTypes = check.ToArray();

            Assert.IsTrue(checkedTypes.Length >= 57);
        }

        [Test]
        public void exhaustive_screening_test()
        {
            var passed = new HashSet<UnivariateDistributionInfo>();
            var todo = new HashSet<UnivariateDistributionInfo>(UnivariateDistributionInfo.GetUnivariateDistributions());

            // Enumerate all distributions in the framework
            foreach (var distribution in UnivariateDistributionInfo.GetUnivariateDistributions())
            {
                todo.Remove(distribution);

                Type type = distribution.DistributionType;
                string name = distribution.Name;
                bool built = false;

                if (type == typeof(GeneralContinuousDistribution)
                    || type == typeof(EmpiricalDistribution)
                    || type == typeof(Mixture<>)
                    || type == typeof(GeneralDiscreteDistribution)) // TODO: add support for vector ranges (e.g. IntegerVector(length: 3))
                    continue;

                // Enumerate all possible ways to construct each distribution
                foreach (var constructor in distribution.GetConstructors())
                {
                    if (!constructor.IsBuildable)
                        continue;

                    // Build the argument list
                    var arguments = new Dictionary<DistributionParameterInfo, object>();

                    // Select the minimum value for the parameters
                    foreach (var parameter in constructor.GetParameters())
                        arguments[parameter] = parameter.Range.Min;

                    IUnivariateDistribution dist;
                    dist = distribution.CreateInstance(arguments);

                    //// Re-build the argument list
                    //arguments.Clear();

                    //// Select the maximum value for the parameters
                    //foreach (var parameter in constructor.GetParameters())
                    //    arguments[parameter] = parameter.Range.Max;

                    //dist = distribution.Activate(arguments);


                    // Re-build the argument list
                    arguments.Clear();

                    // Select the default value for the parameters
                    foreach (var parameter in constructor.GetParameters())
                        arguments[parameter] = parameter.DefaultValue;

                    dist = distribution.CreateInstance(arguments);

                    built = true;


                    if (dist is ShapiroWilkDistribution) // tested in ShapiroWilkDistributionTest.cs
                        continue;
                    if (dist is HypergeometricDistribution) // tested in HypergeometricDistributionDistributionTest.cs
                        continue;
                    if (dist is EmpiricalHazardDistribution) // tested in EmpiricalHazardDistributionest.cs
                        continue;
                    if (dist is DegenerateDistribution) // tested in EmpiricalHazardDistributionest.cs
                        continue;

                    double icdf0 = dist.InverseDistributionFunction(0);
                    double icdf1 = dist.InverseDistributionFunction(1);


                    Assert.AreEqual(dist.Support.Min, icdf0);
                    Assert.AreEqual(dist.Support.Max, icdf1);

                    var range = dist.GetRange(1.0);
                    Assert.AreEqual(dist.Support.Min, range.Min);
                    Assert.AreEqual(dist.Support.Max, range.Max);

                    double middle = 0.5;
                    if (!(dist is WrappedCauchyDistribution ||
                          dist is SymmetricGeometricDistribution)) // exclude distributions that do not support cdf
                    {
                        double icdf05 = dist.InverseDistributionFunction(0.5);
                        Assert.AreEqual(dist.Median, icdf05, 1e-5);
                        middle = dist.Median;
                        Assert.AreEqual(middle, icdf05, 1e-5);

                        double ccdf = dist.ComplementaryDistributionFunction(middle);
                        double cdfm = dist.DistributionFunction(middle);
                        Assert.AreEqual(cdfm, 1 - ccdf, 1e-5);

                        if (dist is AndersonDarlingDistribution)
                            continue; // tested in AndersonDarlingDistributionTest.cs
                        if (dist is RademacherDistribution)
                            continue; // tested in RademacherDistributionTest.cs
                        if (dist is MannWhitneyDistribution)
                            continue; // tested in MannWhitneyDistributionTest.cs
                        if (dist is BernoulliDistribution)
                            continue; // tested in BernoulliDistributionTest.cs
                        if (dist is DegenerateDistribution)
                            continue; // tested in DegenerateDistributionTest.cs
                        if (dist is RademacherDistribution)
                            continue; // tested in RademacherDistributionTest.cs

                        double[] percentiles = Vector.Range(0.0, 1.0, stepSize: 0.1);
                        for (int i = 0; i < percentiles.Length; i++)
                        {
                            double x = percentiles[i];
                            double icdf = dist.InverseDistributionFunction(x);
                            double cdf = dist.DistributionFunction(icdf);
                            double iicdf = dist.InverseDistributionFunction(cdf);
                            double iiicdf = dist.DistributionFunction(iicdf);

                            if (distribution.IsDiscrete)
                            {
                                Assert.AreEqual(iicdf, icdf, 1e-5);
                                Assert.AreEqual(iiicdf, cdf, 1e-5);
                            }
                            else
                            {
                                Assert.AreEqual(iicdf, icdf, 1e-5);
                                Assert.AreEqual(x, cdf, 1e-5);
                                Assert.AreEqual(iiicdf, cdf, 1e-5);
                            }
                        }
                    }

                    if (!(dist is GrubbDistribution ||
                          dist is KolmogorovSmirnovDistribution)) // exclude distributions that do not support pdf
                    {
                        double pdf = dist.ProbabilityFunction(middle);
                        double lpdf = dist.LogProbabilityFunction(middle);
                        Assert.AreEqual(Math.Log(pdf), lpdf, 1e-10);
                        Assert.AreEqual(pdf, Math.Exp(lpdf), 1e-10);
                    }

                    Assert.Throws<ArgumentOutOfRangeException>(() => dist.InverseDistributionFunction(0.0 - 1e-15));
                    Assert.Throws<ArgumentOutOfRangeException>(() => dist.InverseDistributionFunction(1.0 + 1e-15));
                }

                Assert.IsTrue(built);
                passed.Add(distribution);
            }

        }

        [Test]
        public void GetFittingOptionsTest()
        {
            NormalOptions options = (NormalOptions)DistributionInfo.GetFittingOptions<NormalDistribution>();
            Assert.AreEqual(0, options.Regularization);
            Assert.IsNull(options.Postprocessing);
            Assert.AreEqual(false, options.Diagonal);
            Assert.AreEqual(false, options.Robust);
            Assert.AreEqual(false, options.Shared);
        }

        private static bool cmp(string dist, string paramName, string propName)
        {
            if (dist.Contains("AndersonDarlingDistribution"))
            {
                if (paramName == "samples") return "NumberOfSamples" == propName;
                if (paramName == "type") return "DistributionType" == propName;
            }

            if (dist.Contains("ShapiroWilkDistribution"))
            {
                if (paramName == "samples") return "NumberOfSamples" == propName;
            }

            if (dist.Contains("LogLogisticDistribution"))
            {
                if (paramName == "alpha") return "Shape" == propName;
                if (paramName == "beta") return "Scale" == propName;
            }

            if (dist.Contains("GeneralContinuousDistribution"))
                return true;

            paramName = propName.Replace("StandardDeviation", "stddev");
            paramName = propName.Replace("Variance", "var");

            return String.Compare(paramName, propName, ignoreCase: true) == 0;
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
                return false;

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
    }
}
