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
using System.Windows.Forms;
using Accord.Audio.Formats;
using Accord.DirectSound;
using Accord.Math;
using Accord.Statistics.Analysis;
using Accord.Audio;

namespace ICA
{
    public partial class MainForm : Form
    {

        float[][] input;
        float[][] demix;

        AudioOutputDevice output;

        IndependentComponentAnalysis ica;

        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            output = new AudioOutputDevice(Handle, 8000, 1);
            output.FramePlayingStarted += output_Started;
            output.Stopped += output_Stopped;

            input = new float[2][];
            input[0] = getSignal("Resources/mic1.wav");
            input[1] = getSignal("Resources/mic2.wav");
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            output.Stop();
        }

        private void output_Stopped(object sender, EventArgs e)
        {
            invoke(() =>
            {
                btnMic1.Enabled = btnMic2.Enabled = true;

                if (demix != null)
                    btnSource1.Enabled = btnSource2.Enabled = true;
            });
        }

        private void output_Started(object sender, PlayFrameEventArgs e)
        {
            invoke(() =>
            {
                btnMic1.Enabled = btnMic2.Enabled = false;
                btnSource1.Enabled = btnSource2.Enabled = false;
            });
        }


        private float[] getSignal(String path)
        {
            WaveDecoder decoder = new WaveDecoder(path);
            float[] samples = new float[decoder.Frames];
            decoder.Decode(samples.Length).CopyTo(samples);
            return samples;
        }


        private void btnPlayMic1_Click(object sender, EventArgs e)
        {
            output.Play(input[0]);
        }

        private void btnPlayMic2_Click(object sender, EventArgs e)
        {
            output.Play(input[1]);
        }

        private void btnPlaySource1_Click(object sender, EventArgs e)
        {
            if (demix == null || demix[0] == null)
                return;

            output.Play(demix[0]);
        }

        private void btnPlaySource2_Click(object sender, EventArgs e)
        {
            if (demix == null || demix[1] == null)
                return;

            output.Play(demix[1]);
        }


        private void btnRunAnalysis_Click(object sender, EventArgs e)
        {
            // Retrieve the input data as a double[,] matrix
            double[,] data = input.ToMatrix(true).ToDouble();


            // Create a new Independent Component Analysis
            ica = new IndependentComponentAnalysis(data,
                AnalysisMethod.Standardize) { Overwrite = true };

            // Compute the analysis
            ica.Compute();


            // Separate the input signals
            demix = ica.Separate(input);


            btnSource1.Enabled = true;
            btnSource2.Enabled = true;
        }



        private void invoke(Action action)
        {
            try
            {
                this.Invoke(action);
            }
            catch (Exception)
            {
            }
        }

    }

}
