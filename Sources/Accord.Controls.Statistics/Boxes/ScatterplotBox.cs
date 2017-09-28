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
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Forms;
    using Accord.Math;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Visualizations;
    using AForge;

    /// <summary>
    ///   Scatter plot Box for quickly displaying a form with a scatter 
    ///   plot on it in the same spirit as System.Windows.Forms.MessageBox.
    /// </summary>
    /// 
    /// <example>
    /// <code>
    /// // Create unlabeled (x,y) points
    /// double[,] points = 
    /// {
    ///     { 1, 1 }, { 1, 4 },
    ///     { 4, 1 }, { 4, 4 },
    /// };
    /// 
    /// // Display them onscreen
    /// ScatterplotBox.Show(points).Hold();
    /// 
    /// // Create labels for the points
    /// int[] classes = { 0, 1, 0, 1 };
    /// 
    /// // Display it onscreen with labels
    /// ScatterplotBox.Show(points, classes).Hold();
    /// </code>
    ///   
    /// <img src="../images/visualizations/scatterplot-box-1.png"/>
    /// <img src="../images/visualizations/scatterplot-box-2.png"/>
    /// 
    /// </example>
    ///            
    public partial class ScatterplotBox : Form
    {

        private Thread formThread;

        private ScatterplotBox()
        {
            InitializeComponent();
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
        ///   Gets the <see cref="ScatterplotView"/>
        ///   control used to display data in this box.
        /// </summary>
        /// 
        /// <value>The scatter plot view control.</value>
        /// 
        public ScatterplotView ScatterplotView
        {
            get { return scatterplotView1; }
        }

        /// <summary>
        ///   Sets the size of the symbols in the scatter plot.
        /// </summary>
        /// 
        /// <param name="size">The desired symbol size.</param>
        /// 
        /// <returns>This instance, for fluent programming.</returns>
        /// 
        public ScatterplotBox SetSymbolSize(float size)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => SetSymbolSize(size)));
                return this;
            }

            scatterplotView1.SymbolSize = size;
            scatterplotView1.UpdateGraph();

            Refresh();

            return this;
        }

        /// <summary>
        ///   Sets the window title of the scatterplot box.
        ///   
        /// </summary>
        /// <param name="text">The desired title text for the window.</param>
        /// 
        /// <returns>This instance, for fluent programming.</returns>
        /// 
        public ScatterplotBox SetTitle(string text)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => SetTitle(text)));
                return this;
            }

            this.Text = text;

            return this;
        }

        /// <summary>
        ///   Sets size of the scatter plot window.
        /// </summary>
        /// 
        /// <param name="width">The desired width.</param>
        /// <param name="height">The desired height.</param>
        /// 
        /// <returns>This instance, for fluent programming.</returns>
        /// 
        public ScatterplotBox SetSize(int width, int height)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => SetSize(width, height)));
                return this;
            }

            this.Width = width;
            this.Height = height;

            Refresh();

            return this;
        }

        /// <summary>
        ///   Sets whether to show lines connecting
        ///   sequential points in the scatter plot.
        /// </summary>
        /// 
        public ScatterplotBox SetLinesVisible(bool visible)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => SetLinesVisible(visible)));
                return this;
            }

            scatterplotView1.LinesVisible = visible;
            scatterplotView1.UpdateGraph();

            Refresh();

            return this;
        }

        /// <summary>
        ///  Sets whether to remove the grace space
        ///  between the axis labels and points.
        /// </summary>
        /// 
        public ScatterplotBox SetScaleTight(bool tight)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => SetScaleTight(tight)));
                return this;
            }

            scatterplotView1.ScaleTight = tight;
            scatterplotView1.UpdateGraph();

            Refresh();

            return this;
        }


        /// <summary>
        ///   Displays a scatter plot with the specified data.
        /// </summary>
        /// 
        /// <param name="x">The x-values for the data.</param>
        /// 
        public static ScatterplotBox Show(double[] x)
        {
            return Show("Scatter Plot", x);
        }

        /// <summary>
        ///   Displays a scatter plot with the specified data.
        /// </summary>
        /// 
        /// <param name="title">The title for the plot window.</param>
        /// <param name="x">The x-values for the data.</param>
        /// <param name="y">The y-values for the data.</param>
        /// <param name="z">The corresponding labels for the (x,y) pairs.</param>
        /// 
        public static ScatterplotBox Show(string title, double[] x, double[] y, int[] z)
        {
            Scatterplot scatterplot = new Scatterplot(title);

            scatterplot.Compute(x, y, z);

            return show(scatterplot);
        }

        /// <summary>
        ///   Displays a scatter plot with the specified data.
        /// </summary>
        /// 
        /// <param name="title">The title for the plot window.</param>
        /// <param name="x">The x-values for the data.</param>
        /// <param name="y">The y-values for the data.</param>
        /// 
        public static ScatterplotBox Show(string title, double[] x, double[] y)
        {
            Scatterplot scatterplot = new Scatterplot(title);

            scatterplot.Compute(x, y);

            return show(scatterplot);
        }

        /// <summary>
        ///   Displays a scatter plot with the specified data.
        /// </summary>
        /// 
        /// <param name="title">The title for the plot window.</param>
        /// <param name="x">The x-values for the data.</param>
        /// 
        public static ScatterplotBox Show(string title, double[] x)
        {
            Scatterplot scatterplot = new Scatterplot(title);

            scatterplot.Compute(x);

            return show(scatterplot);
        }

        /// <summary>
        ///   Displays a scatter plot with the specified data.
        /// </summary>
        /// 
        /// <param name="x">A two column matrix containing the (x,y) data pairs as rows.</param>
        /// <param name="z">The corresponding labels for the (x,y) pairs.</param>
        /// 
        public static ScatterplotBox Show(double[,] x, int[] z)
        {
            return Show("Scatter Plot", x, z);
        }

        /// <summary>
        ///   Displays a scatter plot with the specified data.
        /// </summary>
        /// 
        /// <param name="x">A two column matrix containing the (x,y) data pairs as rows.</param>
        /// 
        public static ScatterplotBox Show(double[,] x)
        {
            return Show("Scatter Plot", x);
        }

        /// <summary>
        ///   Displays a scatter plot with the specified data.
        /// </summary>
        /// 
        /// <param name="title">The title for the plot window.</param>
        /// <param name="x">A two column matrix containing the (x,y) data pairs as rows.</param>
        /// <param name="z">The corresponding labels for the (x,y) pairs.</param>
        /// 
        public static ScatterplotBox Show(string title, double[,] x, int[] z)
        {
            Scatterplot scatterplot = new Scatterplot(title);

            scatterplot.Compute(x, z);

            return show(scatterplot);
        }

        /// <summary>
        ///   Displays a scatter plot with the specified data.
        /// </summary>
        /// 
        /// <param name="title">The title for the plot window.</param>
        /// <param name="x">A two column matrix containing the (x,y) data pairs as rows.</param>
        /// 
        public static ScatterplotBox Show(string title, double[,] x)
        {
            Scatterplot scatterplot = new Scatterplot(title);

            scatterplot.Compute(x);

            return show(scatterplot);
        }

        /// <summary>
        ///   Displays a scatter plot with the specified data.
        /// </summary>
        /// 
        /// <param name="x">A two column matrix containing the (x,y) data pairs as rows.</param>
        /// <param name="z">The corresponding labels for the (x,y) pairs.</param>
        /// 
        public static ScatterplotBox Show(double[][] x, int[] z)
        {
            return Show("Scatterplot", x, z);
        }


        /// <summary>
        ///   Displays a scatter plot with the specified data.
        /// </summary>
        /// 
        /// <param name="x">A two column matrix containing the (x,y) data pairs as rows.</param>
        /// 
        public static ScatterplotBox Show(double[][] x)
        {
            return Show("Scatterplot", x);
        }

        /// <summary>
        ///   Displays a scatter plot with the specified data.
        /// </summary>
        /// 
        /// <param name="title">The title for the plot window.</param>
        /// <param name="x">A two column matrix containing the (x,y) data pairs as rows.</param>
        /// 
        public static ScatterplotBox Show(string title, double[][] x)
        {
            Scatterplot scatterplot = new Scatterplot(title);

            scatterplot.Compute(x);

            return show(scatterplot);
        }

        /// <summary>
        ///   Displays a scatter plot with the specified data.
        /// </summary>
        /// 
        /// <param name="title">The title for the plot window.</param>
        /// <param name="x">A two column matrix containing the (x,y) data pairs as rows.</param>
        /// <param name="z">The corresponding labels for the (x,y) pairs.</param>
        /// 
        public static ScatterplotBox Show(string title, double[][] x, int[] z)
        {
            Scatterplot scatterplot = new Scatterplot(title);

            scatterplot.Compute(x, z);

            return show(scatterplot);
        }

        /// <summary>
        ///   Displays a scatter plot.
        /// </summary>
        /// 
        /// <param name="scatterplot">The scatter plot to show.</param>
        /// 
        public static ScatterplotBox Show(Scatterplot scatterplot)
        {
            return show(scatterplot);
        }

        /// <summary>
        ///   Displays a scatter plot.
        /// </summary>
        /// 
        /// <param name="function">The function to plot.</param>
        /// <param name="max">The maximum value for the functions argument parameter.</param>
        /// <param name="min">The minimum value for the functions argument parameter.</param>
        /// <param name="step">The step size to use during plotting.</param>
        /// 
        public static ScatterplotBox Show(Func<double, double> function, double min, double max, double step)
        {
            return show(null, function, min, max, step);
        }

        /// <summary>
        ///   Displays a scatter plot.
        /// </summary>
        /// 
        /// <param name="function">The function to plot.</param>
        /// <param name="range">The functions argument range to be plotted.</param>
        /// <param name="step">The step size to use during plotting.</param>
        /// 
        public static ScatterplotBox Show(Func<double, double> function, DoubleRange range, double step)
        {
            return show(null, function, range.Min, range.Max, step);
        }

        /// <summary>
        ///   Displays a scatter plot.
        /// </summary>
        /// 
        /// <param name="function">The function to plot.</param>
        /// <param name="range">The functions argument range to be plotted.</param>
        /// 
        public static ScatterplotBox Show(Func<double, double> function, DoubleRange range)
        {
            return show(null, function, range.Min, range.Max, null);
        }

        /// <summary>
        ///   Displays a scatter plot.
        /// </summary>
        /// 
        /// <param name="title">The title for the plot window.</param>
        /// <param name="function">The function to plot.</param>
        /// <param name="range">The functions argument range to be plotted.</param>
        /// 
        public static ScatterplotBox Show(String title, Func<double, double> function, DoubleRange range)
        {
            return show(title, function, range.Min, range.Max, null);
        }


        /// <summary>
        ///   Displays a scatter plot.
        /// </summary>
        /// 
        /// <param name="title">The title for the plot window.</param>
        /// <param name="function">The function to plot.</param>
        /// <param name="npoints">The number of points to use during plotting.</param>
        /// 
        public static ScatterplotBox Show(String title, Func<double, double> function, int npoints)
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
        public static ScatterplotBox Show(String title, Func<double, double> function, double step)
        {
            return show(title, function, null, null, step);
        }

        /// <summary>
        ///   Displays a scatter plot.
        /// </summary>
        /// 
        /// <param name="title">The title for the plot window.</param>
        /// <param name="function">The function to plot.</param>
        /// 
        public static ScatterplotBox Show(String title, Func<double, double> function)
        {
            return show(title, function, null, null, null);
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
        public static ScatterplotBox Show(String title, Func<double, double> function, double min, double max, double step)
        {
            return show(title, function, min, max, step);
        }

        private static ScatterplotBox show(String title, Func<double, double> function,
            double? min, double? max, double? step, int? npoints = null)
        {
            if (min == null || max == null)
            {
                DoubleRange range;
                if (GetRange(function, out range))
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

            Scatterplot scatterplot = new Scatterplot(title ?? "Scatter plot");

            scatterplot.Compute(input, output);

            return show(scatterplot);
        }

        private static ScatterplotBox show(Scatterplot scatterplot)
        {
            ScatterplotBox form = null;
            Thread formThread = null;

            AutoResetEvent stopWaitHandle = new AutoResetEvent(false);

            formThread = new Thread(() =>
            {
                Accord.Controls.Tools.ConfigureWindowsFormsApplication();

                // Show control in a form
                form = new ScatterplotBox();
                form.Text = scatterplot.Title;
                form.formThread = formThread;
                form.scatterplotView1.Scatterplot = scatterplot;

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

        internal static bool GetRange(Func<double, double> source, out DoubleRange range)
        {
            ParameterInfo[] parameters = source.Method.GetParameters();
            ParameterInfo first = parameters[0];

            var obj = source.Target as IUnivariateDistribution;

            if (obj != null && source.Method.Name == "ProbabilityDensityFunction")
            {
                range = obj.Support;
                return true;
            }

            RangeAttribute[] attributes =
                (RangeAttribute[])first.GetCustomAttributes(typeof(RangeAttribute), false);

            if (attributes != null && attributes.Length > 0)
            {
                range = new DoubleRange((double)attributes[0].Minimum, (double)attributes[0].Maximum);
                return true;
            }

            range = new DoubleRange();
            return false;
        }
    }
}
