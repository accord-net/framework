// Accord Statistics Controls Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
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

namespace Accord.Controls
{
    using System;
    using System.Data;
    using System.Threading;
    using System.Windows.Forms;
    using System.Drawing;
    using Accord.Audio;
    using Accord.Math;
    using AForge.Math;
    using ZedGraph;
    using Accord.Audio.Filters;

    /// <summary>
    ///   Wavechart Box.
    /// </summary>
    /// 
    public partial class WavechartBox : Form
    {

        private Thread formThread;

        private CurveList series;

        private WavechartBox()
        {
            InitializeComponent();

            series = new CurveList();

            zedGraphControl.GraphPane.Title.IsVisible = false;

            zedGraphControl.BorderStyle = System.Windows.Forms.BorderStyle.None;
            zedGraphControl.GraphPane.Border.IsVisible = false;
            zedGraphControl.GraphPane.Border.Color = Color.White;
            zedGraphControl.GraphPane.Border.Width = 0;

            // zedGraphControl.IsAntiAlias = true;
            zedGraphControl.GraphPane.Fill = new Fill(Color.White);
            zedGraphControl.GraphPane.Chart.Fill = new Fill(Color.GhostWhite);
            zedGraphControl.GraphPane.CurveList = series;

            zedGraphControl.GraphPane.Legend.IsVisible = true;
            zedGraphControl.GraphPane.Legend.Position = LegendPos.Right;
            zedGraphControl.GraphPane.Legend.IsShowLegendSymbols = false;

            zedGraphControl.GraphPane.XAxis.MajorGrid.IsVisible = true;
            zedGraphControl.GraphPane.XAxis.MinorGrid.IsVisible = false;
            zedGraphControl.GraphPane.XAxis.MajorGrid.Color = Color.LightGray;
            zedGraphControl.GraphPane.XAxis.MajorGrid.IsZeroLine = false;
            zedGraphControl.GraphPane.XAxis.Scale.MaxGrace = 0;
            zedGraphControl.GraphPane.XAxis.Scale.MinGrace = 0;

            zedGraphControl.GraphPane.YAxis.MinorGrid.IsVisible = false;
            zedGraphControl.GraphPane.YAxis.MajorGrid.IsVisible = true;
            zedGraphControl.GraphPane.YAxis.MajorGrid.Color = Color.LightGray;
            zedGraphControl.GraphPane.YAxis.MajorGrid.IsZeroLine = false;
            zedGraphControl.GraphPane.YAxis.Scale.MaxGrace = 0;
            zedGraphControl.GraphPane.YAxis.Scale.MinGrace = 0;
        }

        /// <summary>
        ///   Blocks the caller until the form is closed.
        /// </summary>
        /// 
        public void WaitForClose()
        {
            if (Thread.CurrentThread != formThread)
                formThread.Join();
        }




        /// <summary>
        ///   Displays a Wavechart with the specified signal.
        /// </summary>
        /// 
        /// <param name="signal">The signal object to display.</param>
        /// <param name="channel">The channel to be displayed.</param>
        /// <param name="nonBlocking">If set to <c>true</c>, the caller will continue
        /// executing while the form is shown on screen. If set to <c>false</c>,
        /// the caller will be blocked until the user closes the form. Default
        /// is <c>false</c>.</param>
        /// <param name="title">The title for the data window.</param>
        /// 
        /// <returns>The Data Grid Box being shown.</returns>
        /// 
        public static WavechartBox Show(Signal signal, int channel = 0,
            string title = "Wavechart", bool nonBlocking = false)
        {
            var tuple = Tuple.Create(signal, channel);
            return show(title, nonBlocking, tuple);
        }

        /// <summary>
        ///   Displays a Wavechart with the specified signal.
        /// </summary>
        /// 
        /// <param name="nonBlocking">If set to <c>true</c>, the caller will continue
        /// executing while the form is shown on screen. If set to <c>false</c>,
        /// the caller will be blocked until the user closes the form. Default
        /// is <c>false</c>.</param>
        /// 
        /// <param name="array">The signal to be displayed.</param>
        /// <param name="title">The title for the data window.</param>
        /// 
        /// <returns>The Data Grid Box being shown.</returns>
        /// 
        public static WavechartBox Show(float[] array, String title, bool nonBlocking = false)
        {
            var signal = Signal.FromArray(array, 0, SampleFormat.Format32BitIeeeFloat);
            var tuple = Tuple.Create(signal, 0);
            return show(title, nonBlocking, tuple);
        }

        private static WavechartBox show(String title, bool hold, params Tuple<Signal, int>[] series)
        {
            WavechartBox form = null;
            Thread formThread = null;

            AutoResetEvent stopWaitHandle = new AutoResetEvent(false);

            formThread = new Thread(() =>
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // Show control in a form
                form = new WavechartBox();
                form.Text = title;
                form.formThread = formThread;

                if (!String.IsNullOrEmpty(title))
                {
                    form.zedGraphControl.GraphPane.Title.IsVisible = true;
                    form.zedGraphControl.GraphPane.Title.Text = title;
                }

                var sequence = new ColorSequenceCollection(series.Length);

                for (int i = 0; i < series.Length; i++)
                {
                    var signal = series[i].Item1;
                    int channel = series[i].Item2;

                    ComplexSignal complex = signal as ComplexSignal;

                    if (complex != null && complex.Status != ComplexSignalStatus.Normal)
                    {
                        double[] spectrum = Accord.Audio.Tools
                            .GetPowerSpectrum(complex.GetChannel(channel));
                        double[] frequencies = Accord.Audio.Tools.GetFrequencyVector(signal.Length, signal.SampleRate);

                        form.series.Add(new LineItem(i.ToString(), frequencies,
                            spectrum, sequence.GetColor(i), SymbolType.None));

                        form.zedGraphControl.GraphPane.XAxis.Title.Text = "Frequency";
                        form.zedGraphControl.GraphPane.YAxis.Title.Text = "Power";
                    }
                    else
                    {
                        double[] values;
                        if (signal.Channels == 1)
                        {
                            values = signal.ToDouble();
                        }
                        else
                        {
                            ExtractChannel extract = new ExtractChannel(channel);
                            values = extract.Apply(signal).ToDouble();
                        }

                        form.series.Add(new LineItem(i.ToString(), Matrix.Indices(0, signal.Length).ToDouble(),
                            values, sequence.GetColor(i), SymbolType.None));

                        form.zedGraphControl.GraphPane.XAxis.Title.Text = "Time";
                        form.zedGraphControl.GraphPane.YAxis.Title.Text = "Amplitude";
                    }
                }

                form.zedGraphControl.GraphPane.AxisChange();

                stopWaitHandle.Set();

                Application.Run(form);
            });

            formThread.SetApartmentState(ApartmentState.STA);

            formThread.Start();

            stopWaitHandle.WaitOne();

            if (!hold)
                formThread.Join();

            return form;
        }

    }
}
