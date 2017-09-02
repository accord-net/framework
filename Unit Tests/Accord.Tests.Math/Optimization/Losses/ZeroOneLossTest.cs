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
    using Accord.Math.Optimization.Losses;
    using NUnit.Framework;

    [TestFixture]
    public class ZeroOneLossTest
    {
        [Test]
        public void TestThatLossIsZeroForCorrectClassification()
        {
            int[] expected = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
            double loss = new ZeroOneLoss(expected).Loss(expected); 
            Assert.Zero(loss);
        }
        
        [Test]
        public void TestThatLossIsOneForTotalMissClassification()
        {
            int[] expected = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
            int[] actual =   {0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
            double loss = new ZeroOneLoss(expected).Loss(actual); 
            Assert.AreEqual(1.0, loss, 1e-10);
        }
        
        [Test]
        public void TestThatZeroOneLossReturnsNumberOfMissClassifications()
        {
            int[] expected = {1, 2, 3, 4, 5};
            int[] actual =   {0, 0, 0, 0, 0};
            double loss = new ZeroOneLoss(expected)
            {
                Mean = false
            }.Loss(actual); 
            Assert.AreEqual(5.0, loss, 1e-10);
        }
        
        [Test]
        public void TestThatZeroOneLossEncodesDoubles()
        {
            double[] expected = {0, 1.01, 0.99, 0};
            int[] actual =   {0, 1, 1, 0};
            double loss = new ZeroOneLoss(expected).Loss(actual); 
            Assert.Zero(loss);
        }
        
        [Test]
        public void TestThatZeroOneLossEncodesDoubleMatrices()
        {
            double[][] expected =
            {
                new [] {1d, 0d, 1d, 0d},
                new [] {0d, 1d, 0d, 1d}
            };
            int[] actual =   {0, 1, 0, 1};
            double loss = new ZeroOneLoss(expected).Loss(actual); 
            Assert.Zero(loss);
        }
        
        [Test]
        public void TestThatZeroOneLossNormalizesInputForBinaryClassification()
        {
            int[] expected = {-1, -1, 1, -1};
            int[] actual =   {0, 0, 1, 0};
            double loss = new ZeroOneLoss(expected).Loss(actual); 
            Assert.Zero(loss);
        }
        
        [Test]
        public void gh_824()
        {
            double loss = new ZeroOneLoss(new [] {0, 1}).Loss(new [] {0, 2}); 
            Assert.AreEqual(0.5, loss, 1e-10);
        }
    }
}
