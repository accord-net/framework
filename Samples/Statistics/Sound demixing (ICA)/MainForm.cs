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

using Accord.Audio;
using Accord.Audio.Formats;
using Accord.DirectSound;
using Accord.Math;
using Accord.Statistics.Analysis;
using System;
using System.Windows.Forms;

namespace Demixing.ICA
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
