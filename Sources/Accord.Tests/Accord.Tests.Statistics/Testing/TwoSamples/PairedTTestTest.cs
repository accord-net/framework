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

namespace Accord.Tests.Statistics
{

    using Accord.Statistics.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Statistics.Testing.Power;
    using System;
    using Accord.Math;

    [TestClass()]
    public class PairedTTestTest
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
        public void TTestConstructorTest()
        {
            // Suppose we would like to know the effect of a treatment (such
            // as a new drug) in improving the well-being of 9 patients. The
            // well-being is measured in a discrete scale, going from 0 to 10.
            //
            // To do so, we need to register the initial state of each patient
            // and then register their state after a given time under treatment.

            double[,] patients =
            {
                 //                 before      after
                 //                treatment  treatment
                 /* Patient 1.*/ {     0,         1     },
                 /* Patient 2.*/ {     6,         5     },
                 /* Patient 3.*/ {     4,         9     },
                 /* Patient 4.*/ {     8,         6     },
                 /* Patient 5.*/ {     1,         6     },
                 /* Patient 6.*/ {     6,         7     },
                 /* Patient 7.*/ {     3,         4     },
                 /* Patient 8.*/ {     8,         7     },
                 /* Patient 9.*/ {     6,         5     },
            };

            // Extract the before and after columns
            double[] before = patients.GetColumn(0);
            double[] after = patients.GetColumn(1);

            // Create the paired-sample T-test. Our research hypothesis is
            // that the treatment does improve the patient's well-being. So
            // we will be testing the hypothesis that the well-being of the
            // "before" sample, the first sample, is "smaller" in comparison
            // to the "after" treatment group.

            PairedTTest test = new PairedTTest(before, after,
                TwoSampleHypothesis.FirstValueIsSmallerThanSecond);

            bool significant = test.Significant; //    false
            double pvalue = test.PValue;         //  ~ 0.165

            Assert.IsFalse(significant);
            Assert.AreEqual(0.16500, pvalue, 1e-5);
            Assert.AreEqual(-1.03712, test.Statistic, 1e-5);
            Assert.AreEqual(-0.8888889, test.ObservedDifference,1e-6);
        }

    }
}
