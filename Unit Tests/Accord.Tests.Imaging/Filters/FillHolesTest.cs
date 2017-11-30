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

namespace Accord.Tests.Imaging
{
    using System.Drawing;
    using Accord.Imaging.Converters;
    using Accord.Imaging.Filters;
    using Accord.Math;
    using NUnit.Framework;
    using Accord.Tests.Imaging.Properties;
    using System.IO;
    using System.Threading.Tasks;
    using System;
    using Accord.Imaging;
#if NO_BITMAP
    using Resources = Accord.Tests.Imaging.Properties.Resources_Standard;
#endif

    [TestFixture]
    public class FillHolesTest
    {

        [Test, Ignore("The issue is not reproducible from NUnit (for now)")]
        public void gh408()
        {
            // https://github.com/accord-net/framework/issues/408
            string basePath = NUnit.Framework.TestContext.CurrentContext.WorkDirectory;
            string imgPath = Path.Combine(basePath, "Resources", "large8bpp.png");
            byte[] imageBytes = File.ReadAllBytes(imgPath);

            {
                using (MemoryStream memoryStream = new MemoryStream(imageBytes))
                using (Bitmap bitmap = (Bitmap)System.Drawing.Image.FromStream(memoryStream))
                using (UnmanagedImage unmanagedImage = UnmanagedImage.FromManagedImage(bitmap))
                    new FillHoles().ApplyInPlace(unmanagedImage);
            }

            ParallelLoopResult parallelLoopResult = Parallel.For(0, 1000000, 
                new ParallelOptions { MaxDegreeOfParallelism = 8 }, i =>
            {
                Console.Write(".");

                using (MemoryStream memoryStream = new MemoryStream(imageBytes))
                using (Bitmap bitmap = (Bitmap)System.Drawing.Image.FromStream(memoryStream))
                using (UnmanagedImage unmanagedImage = UnmanagedImage.FromManagedImage(bitmap))
                    new FillHoles().ApplyInPlace(unmanagedImage);
            });
        }

    }
}
