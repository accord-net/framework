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
    public class GermanTest
    {
        // Some tests are based on NLTK test cases
        // https://raw.githubusercontent.com/nltk/nltk/develop/nltk/test/unit/test_stem.py

        [Test]
        public void German_BaseTest()
        {
            GermanStemmer german = new GermanStemmer();

            Assert.AreEqual("Schrank", german.Stem("Schr\xe4nke"));
            Assert.AreEqual("kein", german.Stem("keinen"));
            Assert.AreEqual("aeternitatis", german.Stem("aeternitatis"));
            Assert.AreEqual("affar", german.Stem("affäre"));
        }

        [Test]
        public void German_PreludeTest()
        {
            GermanStemmer german = new GermanStemmer();

            Assert.AreEqual("ablasst", german.Stem("abläßt"));
        }

        [Test]
        public void German_FullTest()
        {
            Tools.Test(new GermanStemmer(), "german");
        }

    }
}
