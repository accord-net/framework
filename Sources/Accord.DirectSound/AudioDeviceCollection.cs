// Accord Audio Library
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

namespace Accord.DirectSound
{
    using System;
    using System.Collections.Generic;
    using SlimDX.DirectSound;

    /// <summary>
    ///   Audio Device Category.
    /// </summary>
    /// 
    public enum AudioDeviceCategory
    {
        /// <summary>
        ///   Capture audio device, such as a microphone or audio-in.
        /// </summary>
        /// 
        Capture,

        /// <summary>
        ///   Output audio device, such as speakers or headphone jacks.
        /// </summary>
        /// 
        Output,
    }

    /// <summary>
    ///   Audio Device Collection
    /// </summary>
    /// 
    /// <remarks>
    ///   Objects of this class may be used to iterate through available audio
    ///   devices present in the system. For example, by creating a <see cref=
    ///   "AudioDeviceCollection"/> specifying <see cref="AudioDeviceCategory.Output">
    ///   AudioDeviceCategory.Output</see> to its constructor, it will be possible
    ///   to iterate through all available output devices detected by DirectSound.
    ///   To list capture devices, use <see cref="AudioDeviceCategory.Capture">
    ///   AudioDeviceCategory.Capture</see> instead.</remarks>
    ///   
    ///  <example>
    ///    <para>The source code below shows a typical usage of AudioDeviceCollection.</para>
    ///   
    ///    <code>
    ///    // Create a new AudioDeviceCollection to list output devices:
    ///    var collection = new AudioDeviceCollection(AudioDeviceCategory.Output);
    ///   
    ///    // Print all devices available in the system
    ///    foreach (var device in collection)
    ///       Console.WriteLine(device.ToString());
    ///     
    ///    // Get the default audio device in the system
    ///    var defaultDevice = collection.Default;
    ///   </code>
    ///  </example>
    /// 
    public class AudioDeviceCollection : IEnumerable<AudioDeviceInfo>
    {

        /// <summary>
        ///   Gets the default audio device of the chosen
        ///   <see cref="Category">category</see>.
        /// </summary>
        /// 
        /// <value>The default audio device of the chosen <see cref="Category">category</see>.</value>
        /// 
        public AudioDeviceInfo Default { get; private set; }

        /// <summary>
        ///   Gets the <see cref="AudioDeviceCategory">category</see>
        ///   of the audio devices listed by this collection.
        /// </summary>
        /// 
        public AudioDeviceCategory Category { get; private set; }


        /// <summary>
        ///   Creates a <see cref="AudioDeviceCollection"/> class containing
        ///   devices of the given <see cref="AudioDeviceCategory">category</see>.
        /// </summary>
        /// 
        /// <param name="category">The category of the devices.</param>
        /// 
        public AudioDeviceCollection(AudioDeviceCategory category)
        {
            this.Category = category;

            // Set default device
            DeviceInformation info = new DeviceInformation();
            if (Category == AudioDeviceCategory.Output)
            {
                info.Description = "Primary Sound Driver";
                info.DriverGuid = Guid.Empty;
            }

            else if (Category == AudioDeviceCategory.Capture)
            {
                info.Description = "Primary Sound Capture Driver";
                info.DriverGuid = Guid.Empty;
            }

            this.Default = new AudioDeviceInfo(info);
        }

        /// <summary>
        ///   Returns an enumerator that iterates through the device collection.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that 
        ///   can be used to iterate through the collection.
        /// </returns>
        /// 
        public IEnumerator<AudioDeviceInfo> GetEnumerator()
        {
            DeviceCollection devices = (Category == AudioDeviceCategory.Capture) ?
                DirectSoundCapture.GetDevices() :
                DirectSound.GetDevices();

            foreach (DeviceInformation info in devices)
                yield return new AudioDeviceInfo(info);
        }


        /// <summary>
        ///   Returns an enumerator that iterates through the device collection.
        /// </summary>
        /// 
        /// <returns>
        ///   An <see cref="T:System.Collections.IEnumerator"/> object that 
        ///   can be used to iterate through the collection.
        /// </returns>
        /// 
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (System.Collections.IEnumerator)GetEnumerator();
        }

    }
}
