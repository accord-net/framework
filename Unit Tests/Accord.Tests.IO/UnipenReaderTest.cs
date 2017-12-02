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

namespace Accord.Tests.IO
{
    using System.IO;
    using Accord.IO;
    using Accord.Tests.IO.Properties;
    using NUnit.Framework;

    [TestFixture]
    public class UnipenReaderTest
    {
        public static Stream GetPendigits()
        {
            string fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "pendigits-orig.tes.Z");
            return new FileStream(fileName, FileMode.Open, FileAccess.Read);
        }


        [Test]
        public void ReadSampleTest()
        {
            var file = GetPendigits();

            var reader = new UnipenReader(file, compressed: true);

            Assert.AreEqual(new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" }, reader.Lexicon);
            Assert.AreEqual("DIGIT", reader.Hierarchy);


            string label;
            string comment;
            double[][] values = reader.Read(out label, out comment);

            Assert.AreEqual("8", label);
            Assert.AreEqual("8  6  138", comment);
        }

        [Test]
        public void ReadToEnd()
        {
            var file = GetPendigits();

            var reader = new UnipenReader(file, compressed: true);

            Assert.AreEqual(new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" }, reader.Lexicon);
            Assert.AreEqual("DIGIT", reader.Hierarchy);

            var tuple = reader.ReadToEnd();
            double[][][] x = tuple.Item1;
            string[] y = tuple.Item2;

            Assert.AreEqual(x.Length, y.Length);
            Assert.AreEqual(3498, x.Length);
        }

    }
}
