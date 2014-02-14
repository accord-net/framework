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
    using System;
    using Accord.Statistics.Analysis;    
    
    [TestClass()]
    public class BowkerTestTest
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
        public void BowkerTestConstructorTest()
        {
            // Example from Bortz, Lienert and Klaus. Boehnke Verteilungsfreie Methoden in Der Biostatistik, pg 166
            // http://books.google.com.br/books?id=chxDIA-x3WIC&printsec=frontcover&source=gbs_atb#v=onepage&q&f=false

            int[,] matrix =
            {
                { 14,  7,  9 },
                {  5, 26, 19 },
                {  1,  7, 12 },
            };

            GeneralConfusionMatrix a = new GeneralConfusionMatrix(matrix);
            
            BowkerTest target = new BowkerTest(a);

            Assert.AreEqual(12.27, target.Statistic, 1e-2);
            Assert.IsFalse(Double.IsNaN(target.Statistic));
            Assert.AreEqual(3, target.DegreesOfFreedom);
        }

      
    }
}
