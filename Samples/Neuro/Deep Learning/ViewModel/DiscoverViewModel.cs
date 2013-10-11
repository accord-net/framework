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

namespace DeepLearning.ViewModel
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Media.Imaging;
    using DeepLearning.Databases;

    /// <summary>
    ///   View-Model for the Discover tab.
    /// </summary>
    /// 
    public class DiscoverViewModel : INotifyPropertyChanged
    {

        public MainViewModel Main { get; private set; }

        public ObservableCollection<BitmapImage> Features { get; set; }

        public int LayerIndex { get; set; }

        public int MaximumIndex
        {
            get { return Main.Network.Layers.Length - 1; }
        }



        public DiscoverViewModel(MainViewModel main)
        {
            Main = main;
            Features = new ObservableCollection<BitmapImage>();

            Main.PropertyChanged += new PropertyChangedEventHandler(Main_PropertyChanged);
        }

        private void Main_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Network")
            {
                PropertyChanged(this, new PropertyChangedEventArgs("MaximumIndex"));
                LayerIndex = MaximumIndex;
            }
        }


        public void Compute()
        {
            var network = Main.Network;
            var dataset = Main.Database;

            if (LayerIndex >= network.Layers.Length)
                return;

            int outputCount = network.Layers[LayerIndex].Neurons.Length;

            Features.Clear();

            double[] output = new double[outputCount];
            for (int i = 0; i < outputCount; i++)
            {
                output[i] = 1;

                // Reconstruct
                double[] input = network.Reconstruct(output, LayerIndex);

                BitmapImage image = dataset.ToBitmap(input).ToBitmapImage();

                Features.Add(image);

                output[i] = 0;
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
