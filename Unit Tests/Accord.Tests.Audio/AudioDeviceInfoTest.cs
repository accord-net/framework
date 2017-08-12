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

using NUnit.Framework;
using Accord.DirectSound;
using System.Reflection;
using System;
using Accord.Compat;
using System.Numerics;
using System.Linq;

namespace Accord.Tests.Audio
{

    [TestFixture]
    public class AudioDeviceInfoTest
    {

        [Test]
        public void sharpdx_version_test()
        {
#if NET35
            Type type = typeof(SharpDX.Bool);
            Assembly asm = type.Assembly;
            Assert.AreEqual("v2.0.50727", asm.ImageRuntimeVersion);
#elif NET40
            Type type = typeof(SharpDX.DirectSound.CaptureBufferCapabilities);
            Assembly asm = type.Assembly;
            Assert.AreEqual("v4.0.30319", asm.ImageRuntimeVersion);
#elif NETCORE
            Type type = typeof(SharpDX.DirectSound.CaptureBufferCapabilities);
            Assembly asm = type.GetTypeInfo().Assembly;
            var targetFramework = (System.Runtime.Versioning.TargetFrameworkAttribute)asm
                .GetCustomAttributes(typeof(System.Runtime.Versioning.TargetFrameworkAttribute)).First();
            Assert.AreEqual(".NETCoreApp,Version=v1.0", targetFramework.FrameworkName);
#else
            Type type = typeof(SharpDX.DirectSound.CaptureBufferCapabilities);
            Assembly asm = type.Assembly;
            var targetFramework = (System.Runtime.Versioning.TargetFrameworkAttribute)asm
                .GetCustomAttributes(typeof(System.Runtime.Versioning.TargetFrameworkAttribute)).First();
            Assert.AreEqual(".NETFramework,Version=v4.5", targetFramework.FrameworkName);
#endif
        }

        [Test]
        public void enumerate_test()
        {
            var capture = new AudioDeviceCollection(AudioDeviceCategory.Capture);
            Assert.AreEqual(AudioDeviceCategory.Capture, capture.Category);
            Assert.IsNotNull(capture.Default);

            var output = new AudioDeviceCollection(AudioDeviceCategory.Output);
            Assert.AreEqual(AudioDeviceCategory.Output, output.Category);
            Assert.IsNotNull(output.Default);
        }

    }
}
