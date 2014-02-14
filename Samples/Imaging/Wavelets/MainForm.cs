// Accord.NET Sample Applications
// http://accord-framework.net
//
// Copyright © 2009-2014, César Souza
// All rights reserved. 3-BSD License:
//
//   Redistribution and use in source and binary forms, with or without
//   modification, are permitted provided that the following conditions are met:
//
//      * Redistributions of source code must retain the above copyright
//        notice, this list of conditions and the following disclaimer.
//
//      * Redistributions in binary form must reproduce the above copyright
//        notice, this list of conditions and the following disclaimer in the
//        documentation and/or other materials provided with the distribution.
//
//      * Neither the name of the Accord.NET Framework authors nor the
//        names of its contributors may be used to endorse or promote products
//        derived from this software without specific prior written permission.
// 
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
//  DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//  ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
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
