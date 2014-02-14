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
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Windows.Threading;
    using DeepLearning.Databases;

    /// <summary>
    ///   This is the View-Model for the dream tab.
    /// </summary>
    /// 
    public class DreamViewModel : INotifyPropertyChanged
    {

        public MainViewModel Main { get; private set; }


        /// <summary>
        ///   Gets the collection of dream images.
        /// </summary>
        /// 
        public ObservableCollection<Sample> Dreams { get; set; }

        /// <summary>
        ///   Gets the number of images being dreamed.
        /// </summary>
        /// 
        public int NumberOfImages { get; set; }

        /// <summary>
        ///   Gets whether the network is dreaming.
        /// </summary>
        /// 
        public bool IsDreaming { get; private set; }



        public DreamViewModel(MainViewModel main)
        {
            this.Main = main;
            this.Dreams = new ObservableCollection<Sample>();
            this.NumberOfImages = 24;
            this.Initialize();
        }


        public void Initialize()
        {
            var network = Main.Network;
            Random random = new Random();

            for (int i = 0; i < NumberOfImages; i++)
            {
                // Generate random input vectors
                Dreams.Add(new Sample());
            }
        }

        public void Randomize()
        {
            var network = Main.Network;
            var dataset = Main.Database;
            Random random = new Random();


            foreach (Sample sample in Dreams)
            {
                // Generate random input vectors
                double[] input = new double[network.InputsCount];
                for (int j = 0; j < input.Length; j++)
                    input[j] = random.NextDouble();

                sample.Features = input;
                sample.Image = dataset.ToBitmap(input).ToBitmapImage();
            }
        }

        public void Start()
        {
            if (IsDreaming)
            {
                IsDreaming = false;
                return;
            }

            var network = Main.Network;
            var dataset = Main.Database;

            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

            IsDreaming = true;
            Randomize();

            new Task(() =>
            {
                while (IsDreaming)
                {
                    Parallel.For(0, Dreams.Count, i =>
                    {
                        // Grab the current dream status
                        double[] input = Dreams[i].Features;

                        // Feed it to the network
                        double[] output = network.GenerateOutput(input);

                        // Reconstruct using within knowledge
                        input = network.Reconstruct(output);

                        dispatcher.Invoke((Action)(() =>
                        {
                            Dreams[i].Features = input;
                            Dreams[i].Image = dataset.ToBitmap(input).ToBitmapImage();
                        }), DispatcherPriority.ContextIdle);
                    });
                }
            }).Start();
        }

        public void Stop()
        {
            IsDreaming = false;
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
