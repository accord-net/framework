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

namespace Accord.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;
    using Accord.Math;
    using ZedGraph;
    using Accord.Compat;

    /// <summary>
    ///   Data Series Box for quickly displaying a form with a time
    ///   series plot on it in the same spirit as System.Windows.Forms.MessageBox.
    /// </summary>
    /// 
    public partial class DataSeriesBox : Form
    {

        private Thread formThread;

        private CurveList series;

        private DataSeriesBox()
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
        ///   
        /// </summary>
        /// <param name="text">The desired title text for the window.</param>
        /// 
        /// <returns>This instance, for fluent programming.</returns>
        /// 
        public DataSeriesBox SetTitle(string text)
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
        public DataSeriesBox SetGraph(Action<GraphPane> pane)
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
        ///   Sets the data labels for the values being shown.
        /// </summary>
        /// 
        /// <param name="labels">The text labels.</param>
        /// <param name="size">The text size.</param>
        /// 
         public DataSeriesBox SetLabels(string[] labels, float size)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => SetLabels(labels, size)));
                return this;
            }

            var pane = zedGraphControl.GraphPane;

            LineItem lineItem = series[0] as LineItem;

            for (int i = 0; i < lineItem.Points.Count; i++)
            {
                var label = new TextObj(labels[i],
                    lineItem.Points[i].X, lineItem.Points[i].Y + 5);

                label.FontSpec.Border.IsVisible = false;
                label.FontSpec.Size = size;
                label.FontSpec.Fill.IsVisible = false;
                label.FontSpec.Angle = 45;
                pane.GraphObjList.Add(label);
            }

            lineItem.Label.IsVisible = true;
            zedGraphControl.GraphPane.AxisChange();

            return this;
        }

        /// <summary>
        ///   Displays a scatter plot with the specified data.
        /// </summary>
        /// 
        /// <param name="title">The title for the data.</param>
        /// <param name="series">The data series.</param>
        /// 
        public static DataSeriesBox Show(string title = "Time series", params IEnumerable<double>[] series)
        {
            return show(title, null, series);
        }

        /// <summary>
        ///   Displays a scatter plot with the specified data.
        /// </summary>
        /// 
        /// <param name="values">The x-values for the data series.</param>
        /// <param name="series">The data series.</param>
        /// 
        public static DataSeriesBox Show(IEnumerable<double> values, params IEnumerable<double>[] series)
        {
            return show(null, values, series);
        }

        /// <summary>
        ///   Displays a scatter plot with the specified data.
        /// </summary>
        /// 
        /// <param name="values">The x-values for the data series.</param>
        /// <param name="series">The data series.</param>
        /// 
        public static DataSeriesBox Show(IEnumerable<DateTime> values, params IEnumerable<double>[] series)
        {
            double[] xx = values.ToArray().Convert(a => (double)(new XDate(a)));
            return show(null, xx, series, true);
        }

        /// <summary>
        ///   Displays a scatter plot.
        /// </summary>
        /// 
        /// <param name="title">The title for the plot window.</param>
        /// <param name="function">The function to plot.</param>
        /// <param name="range">The functions argument range to be plotted.</param>
        /// 
        public static DataSeriesBox Show(String title, Func<double, double> function, DoubleRange range)
        {
            return show(title, function, range.Min, range.Max, null, null);
        }


        /// <summary>
        ///   Displays a scatter plot.
        /// </summary>
        /// 
        /// <param name="title">The title for the plot window.</param>
        /// <param name="function">The function to plot.</param>
        /// <param name="npoints">The number of points to use during plotting.</param>
        /// 
        public static DataSeriesBox Show(String title, Func<double, double> function, int npoints)
        {
            return show(title, function, null, null, null, npoints);
        }

        /// <summary>
        ///   Displays a scatter plot.
        /// </summary>
        /// 
        /// <param name="title">The title for the plot window.</param>
        /// <param name="function">The function to plot.</param>
        /// <param name="step">The step size to use during plotting.</param>
        /// 
        public static DataSeriesBox Show(String title, Func<double, double> function, double step)
        {
            return show(title, function, null, null, step, null);
        }

        /// <summary>
        ///   Displays a scatter plot.
        /// </summary>
        /// 
        /// <param name="title">The title for the plot window.</param>
        /// <param name="function">The function to plot.</param>
        /// 
        public static DataSeriesBox Show(String title, Func<double, double> function)
        {
            return show(title, function, null, null, null, null);
        }

        /// <summary>
        ///   Displays a scatter plot.
        /// </summary>
        /// 
        /// <param name="title">The title for the plot window.</param>
        /// <param name="function">The function to plot.</param>
        /// <param name="max">The maximum value for the functions argument parameter.</param>
        /// <param name="min">The minimum value for the functions argument parameter.</param>
        /// <param name="step">The step size to use during plotting.</param>
        /// 
        public static DataSeriesBox Show(String title, Func<double, double> function, double min, double max, double step)
        {
            return show(title, function, min, max, step, null);
        }

        private static DataSeriesBox show(String title, Func<double, double> function, double? min, double? max, double? step, int? npoints)
        {
            if (min == null || max == null)
            {
                DoubleRange range;
                if (ScatterplotBox.GetRange(function, out range))
                {
                    min = range.Min;
                    max = range.Max;
                }
                else
                {
                    min = 0;
                    max = 1;
                }
            }

            if (npoints == null)
                npoints = 1000;

            if (step == null)
                step = (max - min) / npoints;

            double[] input = Vector.Range(min.Value, max.Value, step.Value);
            double[] output = Matrix.Apply(input, function);

            return show(title, input, new[] { output });
        }

        private static DataSeriesBox show(String title, IEnumerable<double> x,
            IEnumerable<double>[] series, bool time = false)
        {
            return show(title, x.ToArray(), series.Apply(y => y.ToArray()), time);
        }

        private static DataSeriesBox show(String title, double[] x, double[][] series,
            bool time = false)
        {
            DataSeriesBox form = null;
            Thread formThread = null;

            if (title == null)
                title = "Time series";

            x = (double[])x.Clone();
            var idx = Vector.Range(0, x.Length);
            Array.Sort(x, idx);

            for (int i = 0; i < series.Length; i++)
                series[i] = series[i].Get(idx);

            AutoResetEvent stopWaitHandle = new AutoResetEvent(false);

            formThread = new Thread(() =>
            {
                Accord.Controls.Tools.ConfigureWindowsFormsApplication();

                // Show control in a form
                form = new DataSeriesBox();
                form.Text = title;
                form.formThread = formThread;

                var pane = form.zedGraphControl.GraphPane;

                if (time)
                {
                    pane.XAxis.Type = AxisType.Date;
                    pane.XAxis.Scale.MajorUnit = DateUnit.Hour;
                    pane.XAxis.Scale.Format = "T";
                }

                var sequence = new ColorSequenceCollection(series.Length);

                for (int i = 0; i < series.Length; i++)
                {
                    if (x == null)
                        x = Vector.Range(0, series[i].Length).ToDouble();

                    var lineItem = new LineItem(i.ToString(), x,
                        series[i], sequence.GetColor(i), SymbolType.None);

                    form.series.Add(lineItem);
                }

                pane.Title.Text = title;
                pane.AxisChange();

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
