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
    using Accord.Neuro;
    using Accord.Neuro.ActivationFunctions;
    using Accord.Neuro.Networks;
    using AForge.Neuro;
    using DeepLearning.Databases;

    /// <summary>
    ///   Main View-Model for the application.
    /// </summary>
    /// 
    public class MainViewModel : INotifyPropertyChanged
    {
        

        public DeepBeliefNetwork Network { get; private set; }
        public IDatabase Database { get; private set; }

        public bool CanClassify { get { return Network != null && Database != null && Network.OutputCount == Database.Classes; } }
        public bool CanGenerate { get { return Network != null && Network.Layers.Length > 0; } }


        public UseViewModel Use { get; private set; }
        public DreamViewModel Dream { get; private set; }
        public LearnViewModel Learn { get; private set; }
        public DiscoverViewModel Discover { get; private set; }


        public MainViewModel()
        {
            // Create settings for Optidigits dataset
            Network = new DeepBeliefNetwork(new BernoulliFunction(), 1024, 50, 10);

            Database = new Optdigits()
            {
                IsNormalized = false
            };

            new GaussianWeights(Network).Randomize();
            Network.UpdateVisibleWeights();
            

            Learn = new LearnViewModel(this);
            Use = new UseViewModel(this);
            Dream = new DreamViewModel(this);
            Discover = new DiscoverViewModel(this);

            NewLayerNeurons = 10;
        }

        public int NewLayerNeurons { get; set; }

        public void StackNewLayer()
        {
            if (Database.IsNormalized && Network.Layers.Length == 0)
            {
                Network.Push(NewLayerNeurons,
                    visibleFunction: new GaussianFunction(),
                    hiddenFunction: new BernoulliFunction());
            }
            else Network.Push(NewLayerNeurons, new BernoulliFunction());

            PropertyChanged(this, new PropertyChangedEventArgs("Network"));
        }

        public void RemoveLastLayer()
        {
            Network.Pop();
            PropertyChanged(this, new PropertyChangedEventArgs("Network"));
        }

        public void Save(string filename)
        {
            Network.Save(filename);
        }

        public void Load(string filename)
        {
            Network = DeepBeliefNetwork.Load(filename);
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
