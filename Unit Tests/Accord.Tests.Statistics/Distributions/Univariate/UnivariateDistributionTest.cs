// Accord Unit Tests
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

namespace Accord.Tests.Statistics
{
    using Accord.Statistics.Distributions.Univariate;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Statistics.Distributions;
    using System;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Math;
    using System.Linq;
    using System.Reflection;
    using System.Collections.Generic;
    using Accord.Statistics.Distributions.Multivariate;

    [TestClass()]
    public class UnivariateDistributionTest
    {


        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [TestMethod, Ignore]
        public void ListDistributions()
        {
            var assembly = Assembly.GetAssembly(typeof(IDistribution));

            int nuc, nud, nmc, nmd;

            string uc = GetNames(assembly, typeof(UnivariateContinuousDistribution), out nuc);
            string ud = GetNames(assembly, typeof(UnivariateDiscreteDistribution), out nud);
            string mc = GetNames(assembly, typeof(MultivariateContinuousDistribution), out nmc);
            string md = GetNames(assembly, typeof(MultivariateDiscreteDistribution), out nmd);
        }

        private static string GetNames(Assembly assembly, Type baseType, out int count)
        {
            var distributions = assembly.GetTypes().Where(p =>
                baseType.IsAssignableFrom(p) && !p.IsAbstract && !p.IsInterface);

            var list = new List<String>();
            foreach (Type type in distributions)
                list.Add(type.Name.Replace("Distribution", ""));

            count = list.Count;

            return String.Join(", ", list.ToArray());
        }

        [TestMethod()]
        public void ParameterPropertyTest()
        {
            // Dynamically enumerate all univariate distributions and make
            // sure the distribution contains properties for every exposed
            // constructors' parameters

            var baseType = typeof(IUnivariateDistribution);
            var assembly = Assembly.GetAssembly(baseType);

            // Get all univariate distributions in Accord.NET:
            var distributions = assembly.GetTypes().Where(p =>
                baseType.IsAssignableFrom(p) && !p.IsAbstract && !p.IsInterface);

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
    }
}
