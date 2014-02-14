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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Accord.Audio;
using Accord.Audition.Beat;
using Accord.DirectSound;

namespace BeatDetector
{
    /// <summary>
    ///   Beat detector sample application.
    /// </summary>
    /// 
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

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog(this);
        }



    }
}
