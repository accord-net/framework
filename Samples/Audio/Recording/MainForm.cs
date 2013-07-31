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
using Accord.Audio.Formats;
using Accord.DirectSound;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Recording
{
    /// <summary>
    ///   Description of MainForm.
    /// </summary>
    /// 
    public partial class MainForm : Form
    {
        private MemoryStream stream;

        private IAudioSource source;
        private IAudioOutput output;

        private WaveEncoder encoder;
        private WaveDecoder decoder;

        private float[] current;

        private int frames;
        private int samples;
        private int duration;


        public MainForm()
        {
            InitializeComponent();

            chart.SimpleMode = true;
            chart.AddWaveform("wave", Color.Green, 1, false);

            updateButtons();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (source != null)
            {
                source.SignalToStop();
                source.WaitForStop();
            }
            if (output != null)
            {
                output.SignalToStop();
                output.WaitForStop();
            }

            updateButtons();

            Array.Clear(current, 0, current.Length);
            updateWaveform(current, current.Length);
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            stream.Seek(0, SeekOrigin.Begin);
            decoder = new WaveDecoder(stream);

            if (trackBar1.Value < decoder.Frames)
                decoder.Seek(trackBar1.Value);
            trackBar1.Maximum = decoder.Samples;

            output = new AudioOutputDevice(this.Handle, decoder.SampleRate, decoder.Channels);

            output.FramePlayingStarted += output_FramePlayingStarted;
            output.NewFrameRequested += output_NewFrameRequested;
            output.Stopped += output_PlayingFinished;

            output.Play();

            updateButtons();
        }


        // Recording related functions
        private void btnRecord_Click(object sender, EventArgs e)
        {
            // Create capture device
            source = new AudioCaptureDevice()
            {
                DesiredFrameSize = 4096,
                SampleRate = 22050,
                Format = SampleFormat.Format16Bit
            };

            source.NewFrame += source_NewFrame;
            source.AudioSourceError += source_AudioSourceError;

            // Create buffer for wavechart control
            current = new float[source.DesiredFrameSize];

            // Create stream to store file
            stream = new MemoryStream();
            encoder = new WaveEncoder(stream);

            // Start
            source.Start();
            updateButtons();
        }

        private void source_AudioSourceError(object sender, AudioSourceErrorEventArgs e)
        {
            throw new Exception(e.Description);
        }

        private void source_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // Save current frame
            eventArgs.Signal.CopyTo(current);

            // Update waveform
            updateWaveform(current, eventArgs.Signal.Length);

            // Save to memory
            encoder.Encode(eventArgs.Signal);

            // Update counters
            duration += eventArgs.Signal.Duration;
            samples += eventArgs.Signal.Samples;
            frames += eventArgs.Signal.Length;
        }



        // Playing related functions
        private void output_FramePlayingStarted(object sender, PlayFrameEventArgs e)
        {
            updateTrackbar(e.FrameIndex);

            if (e.FrameIndex + e.Count < decoder.Frames)
            {
                int previous = decoder.Position;
                decoder.Seek(e.FrameIndex);

                Signal s = decoder.Decode(e.Count);
                decoder.Seek(previous);

                updateWaveform(s.ToFloat(), s.Length);
            }
        }

        private void output_PlayingFinished(object sender, EventArgs e)
        {
            updateButtons();

            Array.Clear(current, 0, current.Length);
            updateWaveform(current, current.Length);
        }

        private void output_NewFrameRequested(object sender, NewFrameRequestedEventArgs e)
        {
            e.FrameIndex = decoder.Position;

            // Attempt to decode the requested number of frames from the stream
            Signal signal = decoder.Decode(e.Frames);

            if (signal == null)
            {
                e.Stop = true;
            }
            else
            {
                // Inform the number of frames
                // actually read from source
                e.Frames = signal.Length;

                // Copy the signal to the buffer
                signal.CopyTo(e.Buffer);
            }
        }



        private void updateWaveform(float[] samples, int length)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() =>
                {
                    chart.UpdateWaveform("wave", samples, length);
                }));
            }
            else
            {
                chart.UpdateWaveform("wave", current, length);
            }
        }

        private void updateTrackbar(int value)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() =>
                {
                    trackBar1.Value = Math.Max(trackBar1.Minimum, Math.Min(trackBar1.Maximum, value));
                }));
            }
            else
            {
                trackBar1.Value = Math.Max(trackBar1.Minimum, Math.Min(trackBar1.Maximum, value));
            }
        }

        private void updateButtons()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(updateButtons));
                return;
            }

            if (source != null && source.IsRunning)
            {
                btnBwd.Enabled = false;
                btnFwd.Enabled = false;
                btnPlay.Enabled = false;
                btnStop.Enabled = true;
                btnRecord.Enabled = false;
                trackBar1.Enabled = false;
            }
            else if (output != null && output.IsRunning)
            {
                btnBwd.Enabled = false;
                btnFwd.Enabled = false;
                btnPlay.Enabled = false;
                btnStop.Enabled = true;
                btnRecord.Enabled = false;
                trackBar1.Enabled = true;
            }
            else
            {
                btnBwd.Enabled = false;
                btnFwd.Enabled = false;
                btnPlay.Enabled = stream != null;
                btnStop.Enabled = false;
                btnRecord.Enabled = true;
                trackBar1.Enabled = decoder != null;

                trackBar1.Value = 0;
            }
        }

        private void MainFormFormClosed(object sender, FormClosedEventArgs e)
        {
            if (source != null) source.SignalToStop();
            if (output != null) output.SignalToStop();
        }

        private void saveFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Stream fileStream = saveFileDialog1.OpenFile();
            stream.WriteTo(fileStream);
            fileStream.Close();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog(this);
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            lbLength.Text = String.Format("Length: {0:00.00} sec.", duration / 1000.0);
        }


    }
}
