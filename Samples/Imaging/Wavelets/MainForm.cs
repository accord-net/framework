// Accord.NET Sample Applications
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2013
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

using System;
using System.Drawing;
using System.Windows.Forms;
using Accord.Imaging.Filters;
using Accord.Math.Wavelets;

namespace Wavelets
{
    public partial class MainForm : Form
    {
        IWavelet wavelet;
        Bitmap lenna;
        Bitmap transformed;

        public MainForm()
        {
            InitializeComponent();

            cbWavelet.DataSource = new String[] { "Haar", "CDF" };

            lenna = Wavelets.Properties.Resources.lena512;

            // Use 16bpp for enhanced precision
            lenna = AForge.Imaging.Image.Convert8bppTo16bpp( lenna);

            pictureBox.Image = lenna;
        }


        private void btnForward_Click(object sender, EventArgs e)
        {
            if ((string)cbWavelet.SelectedItem == "Haar")
                wavelet = new Haar((int)numIterations.Value);
            else wavelet = new CDF97((int)numIterations.Value);


            // Create forward transform
            WaveletTransform wt = new WaveletTransform(wavelet);

            // Apply forward transform
            transformed = wt.Apply(lenna);

            pictureBox.Image = transformed;
        }

        private void btnBackward_Click(object sender, EventArgs e)
        {
            // Create inverse transform
            WaveletTransform wt = new WaveletTransform(wavelet, true);

            // Apply inverse transform
            pictureBox.Image = wt.Apply(transformed);
        }

        private void btnOriginal_Click(object sender, EventArgs e)
        {
            pictureBox.Image = lenna;
        }
    }
}
