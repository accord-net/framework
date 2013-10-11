// Accord.NET Sample Applications
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2012
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

namespace DeepLearning
{
    using System;
    using System.ComponentModel;
    using System.Windows.Media.Imaging;
    using DeepLearning.Databases;

    public enum Set { None, Training, Testing };


    /// <summary>
    ///   Database sample. This represents a 
    ///   single training or testing instance.
    /// </summary>
    /// 
    [Serializable]
    public class Sample : INotifyPropertyChanged
    {

        public IDatabase Database { get; set; }

        public Sample()
        {
        }

        public double[] Features { get; set; }

        public BitmapImage Image { get; set; }

        public BitmapImage Reconstruction { get; set; }

        public int Result { get; set; }
        public int Class { get; set; }

        public bool? Match
        {
            get
            {
                if (Result == -1) return null;
                else return Result == Class;
            }
        }



        // The PropertyChanged event doesn't needs to be explicitly raised
        // from this application. The event raising is handled automatically
        // by the NotifyPropertyWeaver VS extension using IL injection.
        //
#pragma warning disable 0067
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067
    }

}
