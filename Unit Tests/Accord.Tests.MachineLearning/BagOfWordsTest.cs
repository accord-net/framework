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
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Accord.MachineLearning;
    using Accord.Math;
    using Microsoft.VisualStudio.TestTools.UnitTesting;


    [TestClass()]
    public class BagOfWordsTest
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



        string[][] texts =
        {
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas molestie malesuada nisi et placerat. Curabitur blandit porttitor suscipit. Nunc facilisis ultrices felis, vitae luctus arcu semper in. Fusce ut felis ipsum. Sed faucibus tortor ut felis placerat euismod. Vestibulum pharetra velit et dolor ornare quis malesuada leo aliquam. Aenean lobortis, tortor iaculis vestibulum dictum, tellus nisi vestibulum libero, ultricies pretium nisi ante in neque. Integer et massa lectus. Aenean ut sem quam. Mauris at nisl augue, volutpat tempus nisl. Suspendisse luctus convallis metus, vitae pretium risus pretium vitae. Duis tristique euismod aliquam".Replace(",", "").Replace(".", "").Split(' '),

            "Sed consectetur nisl et diam mattis varius. Aliquam ornare tincidunt arcu eget adipiscing. Etiam quis augue lectus, vel sollicitudin lorem. Fusce lacinia, leo non porttitor adipiscing, mauris purus lobortis ipsum, id scelerisque erat neque eget nunc. Suspendisse potenti. Etiam non urna non libero pulvinar consequat ac vitae turpis. Nam urna eros, laoreet id sagittis eu, posuere in sapien. Phasellus semper convallis faucibus. Nulla fermentum faucibus tellus in rutrum. Maecenas quis risus augue, eu gravida massa.".Replace(",", "").Replace(".", "").Split(' ')
        };

        [TestMethod()]
        public void GetFeatureVectorTest()
        {
            BagOfWords target = new BagOfWords(texts);

            string[] text = { "Lorem", "ipsum", "dolor" };

            int[] actual = target.GetFeatureVector(text);

            Assert.IsTrue(actual[0] == 1);
            Assert.IsTrue(actual[1] == 1);
            Assert.IsTrue(actual[2] == 1);

            for (int i = 3; i < actual.Length; i++)
                Assert.IsFalse(actual[i] == 1);
        }

        [TestMethod()]
        public void GetFeatureVectorTest2()
        {
            BagOfWords target = new BagOfWords(texts);

            string[] text = { "Lorem", "test", "dolor" };

            int[] actual = target.GetFeatureVector(text);

            Assert.IsTrue(actual[0] == 1);
            Assert.IsTrue(actual[1] == 0);
            Assert.IsTrue(actual[2] == 1);

            for (int i = 3; i < actual.Length; i++)
                Assert.IsFalse(actual[i] == 1);
        }

        [TestMethod()]
        public void ComputeTest()
        {
            BagOfWords target = new BagOfWords();

            target.Compute(texts);

            target.MaximumOccurance = Int16.MaxValue;

            string[] text = { "vestibulum", "vestibulum", "vestibulum" };

            int[] actual = target.GetFeatureVector(text);

            int actualIdx = 43;

            Assert.IsTrue(actual[actualIdx] == 3);

            for (int i = 0; i < actual.Length; i++)
            {
                if (i != actualIdx)
                    Assert.IsTrue(actual[i] == 0);
            }
        }

        [TestMethod()]
        public void SerializationTest()
        {
            BagOfWords target = new BagOfWords();

            target.Compute(texts);

            int[][] expected = new int[texts.Length][];
            for (int i = 0; i < expected.Length; i++)
                expected[i] = target.GetFeatureVector(texts[i]);

            MemoryStream stream = new MemoryStream();
            BinaryFormatter fmt = new BinaryFormatter();
            fmt.Serialize(stream, target);
            stream.Seek(0, SeekOrigin.Begin);
            target = (BagOfWords)fmt.Deserialize(stream);

            int[][] actual = new int[expected.Length][];
            for (int i = 0; i < actual.Length; i++)
                actual[i] = target.GetFeatureVector(texts[i]);

            Assert.IsTrue(expected.IsEqual(actual));
        }
    }
}
