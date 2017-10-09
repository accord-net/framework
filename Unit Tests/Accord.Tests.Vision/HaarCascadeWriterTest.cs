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

namespace Accord.Tests.Vision
{
    using Accord.Vision.Detection;
    using NUnit.Framework;
    using System.Threading;
    using Accord.Vision.Detection.Cascades;
    using System.IO;

    [TestFixture]
    public class HaarCascadeWriterTest
    {

        [Test]
        public void write_test()
        {
            string basePath = Path.Combine(NUnit.Framework.TestContext.CurrentContext.TestDirectory, "Resources");

            #region doc_write
            // Let's say we have a Haar cascade definition in OpenCV's 2.4 format:
            string inputCascadePath = Path.Combine(basePath, "haarcascade_frontalface_alt.xml");

            // We can import this cascade into the framework using:
            HaarCascade cascade = HaarCascade.FromXml(inputCascadePath);

            // Now, let's say we would like to save this cascade to C# code:
            string outputCSharpPath = Path.Combine(basePath, "FrontalFace.cs");

            // Write the "haarcascade_frontalface_alt.xml" into complete C# code:
            using (var writer = new HaarCascadeWriter(outputCSharpPath))
                writer.Write(cascade, "MyFrontalFace");

            // Now, you can import the generated FrontalFace.cs file into your application
            // and use it just as it was any other C# class. You can use it in exactly the
            // same way as the other classes in the Accord.Vision.Detection.Cascades namespace
            // such as FaceHaarCascade.cs and NoseHaarCascade.cs.
            #endregion

            string[] file = File.ReadAllLines(outputCSharpPath);
            Assert.AreEqual(2281, file.Length);
        }
    }
}
