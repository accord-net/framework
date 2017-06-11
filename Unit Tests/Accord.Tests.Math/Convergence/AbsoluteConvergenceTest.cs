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

namespace Accord.Tests.Math
{
    using Accord.Math;
    using NUnit.Framework;
    using System;    
    
    [TestFixture]
    public class AbsoluteConvergenceTest
    {

        [Test]
        public void AbsoluteConvergenceConstructorTest()
        {
            var criteria = new AbsoluteConvergence(iterations: 10, tolerance: 0.1);

            int progress = 1;

            do
            {
                // Do some processing...


                // Update current iteration information:
                criteria.NewValue = 12345.6 / progress++;

            } while (!criteria.HasConverged);


            // The method will converge after reaching the 
            // maximum of 10 iterations with a final value
            // of 1371.73:

            int iterations = criteria.CurrentIteration; // 10
            double value = criteria.OldValue; // 1371.7333333


            Assert.AreEqual(10, criteria.CurrentIteration);
            Assert.AreEqual(1371.7333333333333, criteria.OldValue);
        }

        [Test]
        public void AbsoluteConvergenceConstructorTest2()
        {
            var criteria = new AbsoluteConvergence(iterations: 10, tolerance: 1e-5, startValue: 1);
            criteria.CurrentIteration = -2;
            do
            {
                criteria.NewValue /= 10.0;
            } while (!criteria.HasConverged);

            Assert.AreEqual(4, criteria.CurrentIteration);
            Assert.AreEqual(-5, Math.Log10(criteria.OldValue));
            Assert.AreEqual(-6, Math.Log10(criteria.NewValue));
        }

        [Test]
        public void AbsoluteConvergenceConstructorTest3()
        {
            var criteria = new AbsoluteConvergence(iterations: 1, tolerance: 1e-5, startValue: 1);
            criteria.CurrentIteration = -2;
            do
            {
                criteria.NewValue /= 10.0;
            } while (!criteria.HasConverged);

            Assert.AreEqual(1, criteria.CurrentIteration);
            Assert.AreEqual(-2, Math.Log10(criteria.OldValue));
            Assert.AreEqual(-3, Math.Log10(criteria.NewValue));
        }

        [Test]
        public void AbsoluteConvergenceConstructorTest4()
        {
            var criteria = new AbsoluteConvergence(iterations: 0, tolerance: 1e-5, startValue: 1);
            criteria.CurrentIteration = -2;
            do
            {
                criteria.NewValue /= 10.0;
            } while (!criteria.HasConverged);

            Assert.AreEqual(4, criteria.CurrentIteration);
            Assert.AreEqual(-5, Math.Log10(criteria.OldValue));
            Assert.AreEqual(-6, Math.Log10(criteria.NewValue));
        }

        [Test]
        public void AbsoluteConvergenceConstructorTest5()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new AbsoluteConvergence(iterations: -10, tolerance: 1e-10, startValue: 1));
        }
    }
}
