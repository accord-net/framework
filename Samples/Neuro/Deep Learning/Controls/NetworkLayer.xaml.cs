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
    using System.Windows.Controls;
    using System.Windows.Shapes;
    using AForge.Neuro;

    public partial class NetworkLayer : UserControl
    {

        private ActivationLayer layer;

        private bool inputOnly;

        public bool InputOnly
        {
            get { return inputOnly; }
            set
            {
                inputOnly = value;
                update();
            }
        }

        public ActivationLayer Layer
        {
            get { return layer; }
            set
            {
                layer = value;
                update();
            }
        }

        public int Maximum { get; set; }

        public bool Selected { get; set; }

        public int Index { get; set; }

        public int Inputs { get; set; }

        private void update()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                return;

            stackPanel1.Children.Clear();

            if (layer == null)
            {
                int count = Inputs;
                if (count > Maximum) count = Maximum;

                for (int i = 0; i < count; i++)
                    stackPanel1.Children.Add(new Polygon());

                lbCount.Content = count;
            }

            else
            {
                int count = layer.Neurons.Length;
                if (count > Maximum) count = Maximum;

                for (int i = 0; i < count; i++)
                    stackPanel1.Children.Add(new Ellipse());

                lbCount.Content = count;
            }
        }

        public NetworkLayer()
        {
            InitializeComponent();

            Maximum = 1024;
        }



     
    }
}
