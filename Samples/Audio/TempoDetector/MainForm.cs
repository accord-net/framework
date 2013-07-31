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
using Accord.Audition.Beat;
using Accord.DirectSound;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BeatDetector
{
    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class MainForm : Form
    {
        private IAudioSource source;
        private EnergyBeatDetector detector;
        private Metronome metronome;
        private Signal current;

        private List<ComplexSignal> sample;
        private bool initializing = true;

        public MainForm()
        {
            InitializeComponent();

            metronome = new Metronome();
            metronome.SynchronizingObject = lbManualTempo;
            metronome.TempoDetected += metronome_TempoDetected;
        }

        void metronome_TempoDetected(object sender, EventArgs e)
        {
            lbManualTempo.Text = metronome.BeatsPerMinute.ToString();
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
            initializing = true;
            lbStatus.Text = "Waiting for soundcard...";
            source = new AudioCaptureDevice(info.Guid);
            source.SampleRate = 44100;
            source.DesiredFrameSize = 5000;
            source.NewFrame += source_NewFrame;


            detector = new EnergyBeatDetector(43);
            detector.Beat += new EventHandler(detector_Beat);


            sample = new List<ComplexSignal>();

            source.Start();
        }

        void detector_Beat(object sender, EventArgs e)
        {
            button5.Invoke((MethodInvoker)delegate()
                {
                    if (timer1.Enabled == false)
                    {
                        this.button5.BackColor = Color.LightGreen;
                        timer1.Start();
                    }
                    else
                    {
                        timer1.Stop();
                        timer1.Start();
                    }
                });
        }

        void source_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            current = eventArgs.Signal;


            detector.Detect(current);

            if (initializing)
            {
                initializing = false;
                lbStatus.Invoke((MethodInvoker)delegate()
                {
                    label1.Text = "Frame duration (ms): " + current.Duration;
                    lbStatus.Text = "Ready";
                });
            } 
        }


        void MainFormLoad(object sender, EventArgs e)
        {
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
            source.SignalToStop();
        }


        void MainFormFormClosed(object sender, FormClosedEventArgs e)
        {
            if (source != null)
                source.SignalToStop();
        }

        void Button3Click(object sender, EventArgs e)
        {
            sample.Clear();
            //collecting = true;
        }

        void Button4Click(object sender, EventArgs e)
        {
            metronome.Tap();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            button5.BackColor = SystemColors.Control;
        }



    }
}
