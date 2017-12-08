// Accord Statistics Controls Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
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

using System.Collections;

namespace Accord.Controls
{
    using System;
    using System.Threading;
    using System.Windows.Forms;
    using Accord.Statistics.Visualizations;
    using ZedGraph;
    using System.Drawing;
    using Accord.Math;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///   Data Series Box for quickly displaying a form with a time
    ///   series plot on it in the same spirit as System.Windows.Forms.MessageBox.
    /// </summary>
    /// 
    /// <example>
    /// <code>
    /// // Create some data
    /// string[] labels = {  "1",   "2",   "3" };
    /// double[] data =   { 100.0, 150.0, 42.0 };
    /// 
    /// // Display it onscreen
    /// DataBarBox.Show(labels, data).Hold();
    /// </code>
    ///   
    /// <img src="../images/visualizations/databar-box.png"/>
    /// </example>
    /// 
    public partial class DataBarBox : Form
    {

        private Thread formThread;

        private CurveList series;

        private DataBarBox()
        {
            InitializeComponent();

            series = new CurveList();

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
        ///   Sets the window title of the data series box.
        /// </summary>
        /// 
        /// <param name="text">The desired title text for the window.</param>
        /// 
        /// <returns>This instance, for fluent programming.</returns>
        /// 
        public DataBarBox SetTitle(string text)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => SetTitle(text)));
                return this;
            }

            this.Text = text;
            zedGraphControl.GraphPane.Title.Text = text;

            return this;
        }

        /// <summary>
        ///   Sets properties for the graph being shown.
        /// </summary>
        /// 
        /// <param name="pane">The actions to be performed to the graph pane.</param>
        /// 
        public DataBarBox SetGraph(Action<GraphPane> pane)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => SetGraph(pane)));
                return this;
            }

            pane(this.zedGraphControl.GraphPane);
            zedGraphControl.GraphPane.AxisChange();

            return this;
        }


        /// <summary>
        ///   Displays a bar plot.
        /// </summary>
        /// 
        /// <param name="labels">The text labels for the bars.</param>
        /// <param name="values">The value at each bar.</param>
        /// 
        public static DataBarBox Show(IEnumerable<string> labels, params double[][] values)
        {
            return show(null, labels.ToArray(), values);
        }

        private static DataBarBox show(string title, string[] labels, double[][] series)
        {
            DataBarBox form = null;
            Thread formThread = null;

            if (title == null)
                title = "Bar chart";

            double[] x = Vector.Range(0, labels.Length).ToDouble();

            AutoResetEvent stopWaitHandle = new AutoResetEvent(false);

            formThread = new Thread(() =>
            {
                Accord.Controls.Tools.ConfigureWindowsFormsApplication();

                // Show control in a form
                form = new DataBarBox();
                form.Text = title;
                form.formThread = formThread;

                var sequence = new ColorSequenceCollection(series.Length);

                for (int i = 0; i < series.Length; i++)
                {
                    form.series.Add(new BarItem(i.ToString(), x,
                        series[i], sequence.GetColor(i)));
                }

                form.zedGraphControl.GraphPane.Title.Text = title;
                form.zedGraphControl.GraphPane.XAxis.Type = AxisType.Text;
                form.zedGraphControl.GraphPane.XAxis.Scale.TextLabels = labels;
                form.zedGraphControl.GraphPane.XAxis.Scale.MajorStep = 1;
                form.zedGraphControl.GraphPane.AxisChange();

                stopWaitHandle.Set();

                Application.Run(form);
            });

            formThread.SetApartmentState(ApartmentState.STA);

            formThread.Start();

            stopWaitHandle.WaitOne();

            return form;
        }

        /// <summary>
        ///   Holds the execution until the window has been closed.
        /// </summary>
        /// 
        public void Hold()
        {
            if (Thread.CurrentThread == formThread)
                return;

            this.SetTitle(this.Text + " [on hold]");

            formThread.Join();
        }

    }
}
