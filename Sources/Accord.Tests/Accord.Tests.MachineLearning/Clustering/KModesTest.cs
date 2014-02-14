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
    using Accord.MachineLearning;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Accord.Math;
    using Accord.Statistics;
    
    
    [TestClass()]
    public class KModesTest
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
        public void KModesConstructorTest()
        {
            Accord.Math.Tools.SetupGenerator(0);


            // Declare some observations
            int[][] observations = 
            {
                new int[] { 0, 0   }, // a
                new int[] { 0, 1   }, // a
                new int[] { 1, 1   }, // a
 
                new int[] { 5, 3   }, // b
                new int[] { 6, 8   }, // b
                new int[] { 6, 7   }, // b
                new int[] { 5, 8   }, // b

                new int[] { 12, 14 }, // c
                new int[] { 13, 14 }, // c
            };

            int[][] orig = observations.MemberwiseClone();

            // Create a new K-Modes algorithm with 3 clusters 
            KModes kmodes = new KModes(3);

            // Compute the algorithm, retrieving an integer array
            //  containing the labels for each of the observations
            int[] labels = kmodes.Compute(observations);

            // As a result, the first three observations should belong to the
            //  same cluster (thus having the same label). The same should
            //  happen to the next four observations and to the last two.

            Assert.AreEqual(labels[0], labels[1]);
            Assert.AreEqual(labels[0], labels[2]);

            Assert.AreEqual(labels[3], labels[4]);
            Assert.AreEqual(labels[3], labels[5]);
            Assert.AreEqual(labels[3], labels[6]);

            Assert.AreEqual(labels[7], labels[8]);

            Assert.AreNotEqual(labels[0], labels[3]);
            Assert.AreNotEqual(labels[0], labels[7]);
            Assert.AreNotEqual(labels[3], labels[7]);


            int[] labels2 = kmodes.Clusters.Nearest(observations);
            Assert.IsTrue(labels.IsEqual(labels2));

            // the data must not have changed!
            Assert.IsTrue(orig.IsEqual(observations));
        }

        
       
    }
}
