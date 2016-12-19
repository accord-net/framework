﻿// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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
    public class RelativeParameterConvergenceTest
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



        [Test]
        public void RelativeParameterConstructorTest()
        {
            var criteria = new RelativeParameterConvergence(iterations: 0, tolerance: 0.1);

            int progress = 1;
            double[] parameters = { 12345.6, 952.12, 1925.1 };

            do
            {
                // Do some processing...


                // Update current iteration information:
                criteria.NewValues = parameters.Divide(progress++);

            } while (!criteria.HasConverged);


            // The method will converge after reaching the 
            // maximum of 11 iterations with a final value
            // of { 1234.56, 95.212, 192.51 }:

            int iterations = criteria.CurrentIteration; // 11
            var v = criteria.OldValues; // { 1234.56, 95.212, 192.51 }


            Assert.AreEqual(11, criteria.CurrentIteration);
            Assert.AreEqual(1234.56, criteria.OldValues[0]);
            Assert.AreEqual(95.212, criteria.OldValues[1]);
            Assert.AreEqual(192.51, criteria.OldValues[2]);
        }
    }
}
