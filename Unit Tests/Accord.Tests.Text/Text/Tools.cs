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
    using System.IO;

    public static class Tools
    {
        public static void Test(StemmerBase stemmer, string language)
        {
            string dataPath = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "snowball"));

            string langPath = Path.Combine(dataPath, language);

            string inputFile = Path.Combine(langPath, "voc.txt");
            string outputFile = Path.Combine(langPath, "output.txt");

            string input = File.ReadAllText(inputFile);
            string output = File.ReadAllText(outputFile);

            Test(stemmer, input, output);
        }

        public static void Test(StemmerBase stemmer, string input, string output)
        {
            var newline = new[] { Environment.NewLine };
            var inputLines = input.Split(newline, StringSplitOptions.None);
            var outputLines = output.Split(newline, StringSplitOptions.None);

            for (int i = 0; i < inputLines.Length; i++)
            {
                string word = inputLines[i];
                string expected = outputLines[i];
                string actual = stemmer.Stem(word);

                Assert.AreEqual(expected, actual);
            }
        }
    }
}
