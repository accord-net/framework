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

namespace Accord.Tests.MachineLearning
{
    using System;
    using NUnit.Framework;
    using Accord.MachineLearning.Text.Stemmers;

    [TestFixture]
    public class HungarianTest
    {
        [Test]
        public void Hungarian_BaseTest()
        {
            var hungarian = new HungarianStemmer();

            Assert.AreEqual("ab", hungarian.Stem("abból"));
        }

        [Test]
        public void Hungarian_DoubleAcuteTest()
        {
            var hungarian = new HungarianStemmer();

            Assert.AreEqual("bőgőz", hungarian.Stem("bőgőzik"));
        }

        [Test]
        public void Hungarian_HyphenTest()
        {
            var hungarian = new HungarianStemmer();

            Assert.AreEqual("adattárház-menedzser", hungarian.Stem("adattárház-menedzsertől"));
        }

        [Test]
        public void Hungarian_FullTest()
        {
            Tools.Test(new HungarianStemmer(), "hungarian");
        }
    }
}
