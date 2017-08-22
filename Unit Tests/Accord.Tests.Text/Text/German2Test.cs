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
    public class German2Test
    {
        [Test]
        public void German2_BaseTest()
        {
            var german = new German2Stemmer();

            Assert.AreEqual("amtsgeheimnis", german.Stem("amtsgeheimnisse"));
        }

        [Test, Ignore("Snowball issue")]
        public void German2_FullTest()
        {
            // This test is ignored because it seems that the german2 
            // output has not been updated after the -nis change:
            // http://comments.gmane.org/gmane.comp.search.snowball/1119

            Tools.Test(new German2Stemmer(), "german2");
        }
    }
}
