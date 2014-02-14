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
using Accord.Audio.Windows;
using Accord.DirectSound;
using AForge;
using AForge.Math;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Fourier
{
    public partial class MainForm : Form
    {
        private IAudioSource source;
        private IWindow window;

        public MainForm()
        {
            InitializeComponent();
        }


        /// <summary>
        ///   Starts capturing from the chosen audio input interface
        /// </summary>
        /// 
        void btnStart_Click(object sender, EventArgs e)
        {
            // Get the device currently selected in the combobox
            AudioDeviceInfo info = comboBox1.SelectedItem as AudioDeviceInfo;

            if (info == null)
            {
                MessageBox.Show("No audio devices available.");
                return;
            }

            // Create a new audio capture device
            source = new AudioCaptureDevice(info)
            {
                // Capture at 22050 Hz
                DesiredFrameSize = 2048,
                SampleRate = 22050
            };

            // Wire up some notification events
            source.NewFrame += source_NewFrame;
            source.AudioSourceError += source_AudioSourceError;
            

            // Start it!
            source.Start();
        }

        /// <summary>
        ///   Stops capturing
        /// </summary>
        /// 
        void btnStop_Click(object sender, EventArgs e)
        {
            // Stop capturing
            if (source != null)
                source.SignalToStop();
        }

        
        /// <summary>
        ///   This method will be called whenever there is a new audio
        ///   frame to be processed.
        /// </summary>
        /// 
        void source_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // We can start by converting the audio frame to a complex signal
            ComplexSignal signal = ComplexSignal.FromSignal(eventArgs.Signal);

            // If its needed,
            if (window != null)
            {
                // Apply the chosen audio window
                signal = window.Apply(signal, 0);
            }

            // Transform to the complex domain
            signal.ForwardFourierTransform();

            // Now we can get the power spectrum output and its
            // related frequency vector to plot our spectrometer.

            Complex[] channel = signal.GetChannel(0);
            double[] power = Accord.Audio.Tools.GetPowerSpectrum(channel);
            double[] freqv = Accord.Audio.Tools.GetFrequencyVector(signal.Length, signal.SampleRate);

            power[0] = 0; // zero DC
            float[] g = new float[power.Length];
            for (int i = 0; i < power.Length; i++)
                g[i] = (float)power[i];

            // Adjust the zoom according to the horizontal and vertical scrollbars.
            chart1.RangeX = new DoubleRange(freqv[0], freqv[freqv.Length - 1] / hScrollBar1.Value);
            chart1.RangeY = new DoubleRange(0f, Math.Pow(10, -vScrollBar1.Value));

            chart1.UpdateWaveform("fft", g);
        }

        /// <summary>
        ///   This callback will be called when there is some error with the audio 
        ///   source. It can be used to route exceptions so they don't compromise 
        ///   the audio processing pipeline.
        /// </summary>
        /// 
        void source_AudioSourceError(object sender, AudioSourceErrorEventArgs e)
        {
            throw new Exception(e.Description);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Configure the wave chart to display the FFT
            chart1.AddWaveform("fft", Color.Black, 1, false);

            // Enumerate audio devices and add all devices to combo
            AudioDeviceCollection audioDevices = new AudioDeviceCollection(AudioDeviceCategory.Capture);

            foreach (AudioDeviceInfo device in audioDevices)
                comboBox1.Items.Add(device);

            // Set a message if there is none
            if (comboBox1.Items.Count == 0)
            {
                comboBox1.Items.Add("No local capture devices");
                comboBox1.Enabled = false;
            }

            comboBox1.SelectedIndex = 0;
        }

      

        void hammingWindowToolStripMenuItemClick(object sender, EventArgs e)
        {
            window = RaisedCosineWindow.Hamming(2048);

            reset();
            hammingWindowToolStripMenuItem.Checked = true;
        }

        void hannWindowToolStripMenuItemClick(object sender, EventArgs e)
        {
            window = RaisedCosineWindow.Hann(2048);

            reset();
            hannWindowToolStripMenuItem.Checked = true;
        }

        void blackmanWindowToolStripMenuItemClick(object sender, EventArgs e)
        {
            window = new BlackmanWindow(2048);

            reset();
            blackmanWindowToolStripMenuItem.Checked = true;
        }

        private void noneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            window = null;

            reset();
            noneToolStripMenuItem.Checked = true;
        }

        private void reset()
        {
            hannWindowToolStripMenuItem.Checked = false;
            hammingWindowToolStripMenuItem.Checked = false;
            blackmanWindowToolStripMenuItem.Checked = false;
            noneToolStripMenuItem.Checked = false;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (source != null)
                source.SignalToStop();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog(this);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        
    }
}
