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
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using AForge.Neuro;

    public partial class NetworkDiagram : UserControl
    {

        public ActivationNetwork Network
        {
            get { return (ActivationNetwork)GetValue(NetworkProperty); }
            set
            {
                SetValue(NetworkProperty, value);
                updateNetwork();
            }
        }

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set
            {
                SetValue(SelectedIndexProperty, value);
                updateIndex();
            }
        }


        public static readonly DependencyProperty NetworkProperty =
          DependencyProperty.Register("Network", typeof(ActivationNetwork), typeof(NetworkDiagram));

        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(NetworkDiagram));


        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property.Name == "Network")
            {
                updateNetwork();
            }
            else if (e.Property.Name == "SelectedIndex")
            {
                updateIndex();
            }
        }

        private void updateIndex()
        {
            int selectedIndex = SelectedIndex;

            if (selectedIndex < 1 || selectedIndex > stackPanel1.Children.Count)
                return;

            if (selectedIndex >= stackPanel1.Children.Count)
                return;

            var element = stackPanel1.Children[selectedIndex];
            NetworkLayer current = (element as NetworkLayer);
            foreach (NetworkLayer layer in stackPanel1.Children)
                layer.BorderBrush = Brushes.Transparent;

            if (selectedIndex > 0)
                current.BorderBrush = Brushes.Black;
        }

        private void updateNetwork()
        {
            Network network = Network;

            stackPanel1.Children.Clear();

            if (network == null) return;

            // Add Input Layer
            NetworkLayer layer = new NetworkLayer();
            layer.InputOnly = true;
            layer.Inputs = Network.InputsCount;
            layer.Layer = null;
            layer.Name = "Input";
            layer.Index = 0;
            stackPanel1.Children.Add(layer);

            // Add Hidden Layers
            for (int i = 0; i < network.Layers.Length; i++)
            {
                layer = new NetworkLayer();
                layer.InputOnly = false;
                layer.Layer = network.Layers[i] as ActivationLayer;
                layer.Name = "Hidden" + i;
                layer.Index = i + 1;
                layer.MouseLeftButtonDown += new MouseButtonEventHandler(layer_MouseLeftButtonDown);
                stackPanel1.Children.Add(layer);
            }

            SelectedIndex = 1;
        }

        void layer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SelectedIndex = (sender as NetworkLayer).Index;
        }



        public NetworkDiagram()
        {
            InitializeComponent();
        }
    }
}
