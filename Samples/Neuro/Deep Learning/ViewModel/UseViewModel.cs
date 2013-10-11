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
    using System.ComponentModel;
    using System.Windows.Media.Imaging;
    using Accord.Math;
    using Accord.Neuro.Networks;
    using DeepLearning.Databases;

    /// <summary>
    ///   View-Model for the Use tab.
    /// </summary>
    /// 
    public class UseViewModel : INotifyPropertyChanged
    {

        public MainViewModel Main { get; private set; }


        public double[] UserInput { get; set; }

        public BitmapImage NetworkOutput { get; set; }

        public bool IsActive { get; set; }

        public int Classification { get; set; }


        public UseViewModel(MainViewModel main)
        {
            Main = main;

            PropertyChanged += new PropertyChangedEventHandler(UseViewModel_PropertyChanged);
        }

        private void UseViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "UserInput" && IsActive)
                Compute();
        }


        public bool CanCompute { get { return UserInput != null && Main.CanGenerate; } }

        public void Compute()
        {
            if (!CanCompute) return;

            double[] input = UserInput;
            DeepBeliefNetwork network = Main.Network;
            IDatabase database = Main.Database;

            database.Normalize(input);

            {
                double[] output = network.GenerateOutput(input);
                double[] reconstruction = network.Reconstruct(output);
                NetworkOutput = (database.ToBitmap(reconstruction).ToBitmapImage());
            }

            if (Main.CanClassify)
            {
                double[] output = network.Compute(input);
                int imax; output.Max(out imax);
                Classification = imax;
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
