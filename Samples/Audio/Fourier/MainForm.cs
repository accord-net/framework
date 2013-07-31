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
    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class MainForm : Form
    {
        private IAudioSource source;
        private IWindow window;

        public MainForm()
        {
            InitializeComponent();
        }


        void Button1Click(object sender, EventArgs e)
        {
            AudioDeviceInfo info = comboBox1.SelectedItem as AudioDeviceInfo;

            if (info == null)
            {
                MessageBox.Show("No audio devices available.");
                return;
            }


            source = new AudioCaptureDevice(info.Guid);
            source.DesiredFrameSize = 2048;
            source.SampleRate = 22050;
            source.NewFrame += source_NewFrame;
            source.AudioSourceError += source_AudioSourceError;

            window = RaisedCosineWindow.Hamming(source.DesiredFrameSize);

            source.Start();
        }

        void source_AudioSourceError(object sender, AudioSourceErrorEventArgs e)
        {
            throw new Exception(e.Description);
        }

        void source_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {

            ComplexSignal signal = ComplexSignal.FromSignal(eventArgs.Signal);

            if (hammingWindowToolStripMenuItem.Checked)
            {
                ComplexSignal c = window.Apply(signal, 0);
                signal = c;
            }


            signal.ForwardFourierTransform();

            Complex[] channel = signal.GetChannel(0);
            double[] power = Accord.Audio.Tools.GetPowerSpectrum(channel);
            double[] freqv = Accord.Audio.Tools.GetFrequencyVector(signal.Length, signal.SampleRate);

            power[0] = 0; // zero DC

            float[] g = new float[power.Length];

            for (int i = 0; i < power.Length; i++)
            {
                // g[i, 0] = freqv[i];
                g[i] = (float)power[i];
            }

            chart1.RangeX = new DoubleRange(freqv[0], freqv[freqv.Length - 1] / hScrollBar1.Value);
            chart1.RangeY = new DoubleRange(0f, Math.Pow(10, -vScrollBar1.Value));

            chart1.UpdateWaveform("fft", g);
        }


        void MainFormLoad(object sender, EventArgs e)
        {
            chart1.AddWaveform("fft", Color.Black, 1, false);


            // enumerate audio devices and add all devices to combo
            AudioDeviceCollection audioDevices = new AudioDeviceCollection(AudioDeviceCategory.Capture);

            foreach (AudioDeviceInfo device in audioDevices)
            {
                comboBox1.Items.Add(device);
            }

            if (comboBox1.Items.Count == 0)
            {
                comboBox1.Items.Add("No local capture devices");
                comboBox1.Enabled = false;
            }

            comboBox1.SelectedIndex = 0;
        }

        void Button2Click(object sender, EventArgs e)
        {
            if (source != null)
                source.SignalToStop();
        }

        void HammingWindowToolStripMenuItemClick(object sender, EventArgs e)
        {
            hammingWindowToolStripMenuItem.Checked = !hammingWindowToolStripMenuItem.Checked;
        }

        void MainFormFormClosed(object sender, FormClosedEventArgs e)
        {
            if (source != null)
                source.SignalToStop();
        }

    }
}
