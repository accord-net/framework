// Accord Unit Tests
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

namespace Accord.Tests.MachineLearning
{
    using System;
    using System.Data;
    using Accord.MachineLearning.DecisionTrees;
    using Accord.MachineLearning.DecisionTrees.Learning;
    using Accord.MachineLearning.DecisionTrees.Rules;
    using Accord.Math;
    using Accord.Statistics.Filters;
    using Accord.Tests.MachineLearning.Properties;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class DecisionRuleTest
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



        [TestMethod()]
        public void IsInconsistentTest()
        {
            var a1 = new Antecedent(0, ComparisonKind.Equal, 1.0);
            var a2 = new Antecedent(0, ComparisonKind.Equal, 1.0);
            var a3 = new Antecedent(0, ComparisonKind.Equal, 0.0);

            var rule1 = new DecisionRule(0, a1);
            var rule2 = new DecisionRule(0, a2);
            var rule3 = new DecisionRule(0, a3);

            bool a12 = rule1.IsInconsistentWith(rule2);
            bool a13 = rule1.IsInconsistentWith(rule3);

            Assert.IsFalse(a12);
            Assert.IsFalse(a13);
        }

        [TestMethod()]
        public void AntecedentTest()
        {
            var a1 = new Antecedent(0, ComparisonKind.Equal, 1.0);
            var a2 = new Antecedent(1, ComparisonKind.Equal, 1.0);

            var a3 = new Antecedent(0, ComparisonKind.Equal, 0.0);
            var a4 = new Antecedent(0, ComparisonKind.Equal, 1.0);

            bool a12 = a1.Equals(a2);
            bool a34 = a3.Equals(a4);
            bool a14 = a1.Equals(a4);

            Assert.IsFalse(a12);
            Assert.IsFalse(a34);
            Assert.IsTrue(a14);

            int hash1 = a1.GetHashCode();
            int hash2 = a2.GetHashCode();
            int hash3 = a3.GetHashCode();
            int hash4 = a4.GetHashCode();

            Assert.AreNotEqual(hash1, hash2);
            Assert.AreNotEqual(hash3, hash4);
            Assert.AreEqual(hash1, hash4);
        }


    }
}
