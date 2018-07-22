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
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Globalization;
    using System.Windows.Forms;
    using Accord.Math;
    using Accord.Statistics.Visualizations;
    using ZedGraph;

    /// <summary>
    ///   Histogram visualization control.
    /// </summary>
    /// 
    public partial class HistogramView : UserControl
    {

        private Histogram histogram;
        private double[] samples;

        private BarItem graphBars;

        private object dataSource;
        private String dataMember;
        private String displayMember;

        private string formatString = "N2";

        private double? binWidth;
        private int? numberOfBins;


        /// <summary>
        ///   Constructs a new instance of the HistogramView.
        /// </summary>
        /// 
        public HistogramView()
        {
            InitializeComponent();

            this.histogram = new Histogram();
            graphBars = new ZedGraph.BarItem(String.Empty);
            graphBars.Color = Color.DarkBlue;
            zedGraphControl.GraphPane.Title.FontSpec.IsBold = true;
            zedGraphControl.GraphPane.Title.FontSpec.Size = 32f;
            zedGraphControl.GraphPane.Title.IsVisible = true;
            zedGraphControl.GraphPane.XAxis.Type = AxisType.Text;
            zedGraphControl.GraphPane.XAxis.Title.IsVisible = false;
            zedGraphControl.GraphPane.XAxis.MinSpace = 0;
            zedGraphControl.GraphPane.XAxis.MajorGrid.IsVisible = false;
            zedGraphControl.GraphPane.XAxis.MinorGrid.IsVisible = false;
            zedGraphControl.GraphPane.XAxis.MajorTic.IsBetweenLabels = true;
            zedGraphControl.GraphPane.XAxis.MajorTic.IsInside = false;
            zedGraphControl.GraphPane.XAxis.MajorTic.IsOpposite = false;
            zedGraphControl.GraphPane.XAxis.MinorTic.IsAllTics = false;
            zedGraphControl.GraphPane.XAxis.Scale.FontSpec.IsBold = true;
            zedGraphControl.GraphPane.XAxis.Scale.FontSpec.IsAntiAlias = true;
            zedGraphControl.GraphPane.YAxis.MinorTic.IsAllTics = false;
            zedGraphControl.GraphPane.YAxis.MajorTic.IsOpposite = false;
            zedGraphControl.GraphPane.YAxis.Title.Text = "Frequency";
            zedGraphControl.GraphPane.YAxis.Title.FontSpec.Size = 24f;
            zedGraphControl.GraphPane.YAxis.Title.FontSpec.IsBold = true;
            zedGraphControl.GraphPane.Border.IsVisible = false;
            zedGraphControl.GraphPane.BarSettings.MinBarGap = 0;
            zedGraphControl.GraphPane.BarSettings.MinClusterGap = 0;
            zedGraphControl.GraphPane.CurveList.Add(graphBars);
        }


        /// <summary>
        ///   Gets a reference to the underlying ZedGraph
        ///   control used to draw the histogram.
        /// </summary>
        /// 
        public ZedGraphControl Graph
        {
            get { return zedGraphControl; }
        }

        /// <summary>
        ///   Gets the trackbar which controls 
        ///   the histogram bins' width.
        /// </summary>
        /// 
        public TrackBar TrackBar
        {
            get { return trackBar; }
        }

        /// <summary>
        ///   Gets or sets a fixed bin width to be used by
        ///   the histogram view. Setting this value to null
        ///   will set the histogram to the default position.
        /// </summary>
        /// 
        public double? BinWidth
        {
            get { return binWidth; }
            set
            {
                binWidth = value;

                if (!this.DesignMode)
                    onDataBind();
            }
        }

        /// <summary>
        ///   Gets or sets a fixed number of bins to be used by
        ///   the histogram view. Setting this value to null
        ///   will set the histogram to the default position.
        /// </summary>
        /// 
        public int? NumberOfBins
        {
            get { return numberOfBins; }
            set
            {
                numberOfBins = value;

                if (!this.DesignMode)
                    onDataBind();
            }
        }

        /// <summary>
        ///   Gets the underlying histogram being shown by this control.
        /// </summary>
        /// 
        public Histogram Histogram
        {
            get { return histogram; }
            set { DataSource = value; }
        }

        /// <summary>
        ///   Gets or sets a data source for this control.
        /// </summary>
        /// 
        [DefaultValue(null)]
        public object DataSource
        {
            get { return dataSource; }
            set
            {
                dataSource = value;

                if (!this.DesignMode)
                    onDataBind();
            }
        }

        /// <summary>
        ///   Gets or sets the member of the data source 
        ///   to be shown, if applicable.
        /// </summary>
        /// 
        [DefaultValue(null)]
        public string DataMember
        {
            get { return dataMember; }
            set
            {
                dataMember = value;

                if (!this.DesignMode)
                    onDataBind();
            }
        }

        /// <summary>
        ///   Gets or sets the member of the data source
        ///   to be displayed, if applicable.
        /// </summary>
        /// 
        [DefaultValue(null)]
        public string DisplayMember
        {
            get { return displayMember; }
            set
            {
                displayMember = value;

                if (!this.DesignMode)
                    onDataBind();
            }
        }

        /// <summary>
        ///   Gets or sets the format used to display
        ///   the histogram values on screen.
        /// </summary>
        /// 
        [DefaultValue("N2")]
        public string Format
        {
            get { return formatString; }
            set
            {
                formatString = value;
                if (!this.DesignMode)
                    onDataBind();
            }
        }


        /// <summary>
        ///   Forces a update of the Histogram bins.
        /// </summary>
        /// 
        public void UpdateGraph(string title = "Histogram")
        {
            graphBars.Clear();

            String[] labels = new String[histogram.Values.Length];
            for (int i = 0; i < histogram.Values.Length; i++)
            {
                graphBars.AddPoint(i, histogram.Bins[i].Value);
                labels[i] = histogram.Bins[i].Range.Min.ToString(formatString, CultureInfo.CurrentCulture) +
                    " - " + histogram.Bins[i].Range.Max.ToString(formatString, CultureInfo.CurrentCulture);
            }

            zedGraphControl.GraphPane.Title.Text = title;
            zedGraphControl.GraphPane.XAxis.Scale.TextLabels = labels;
            zedGraphControl.GraphPane.XAxis.Scale.FontSpec.Angle = 45.0f;

            zedGraphControl.AxisChange();
            zedGraphControl.Invalidate();
        }

        /// <summary>
        ///   Forces the update of the trackbar control.
        /// </summary>
        /// 
        private void UpdateTrackbar()
        {
            if (samples == null || samples.Length == 0)
            {
                trackBar.Enabled = false;
            }
            else
            {
                trackBar.Enabled = true;
                trackBar.Minimum = 1;
                trackBar.Maximum = samples.Length;
            }
        }

        /// <summary>
        ///   Resets custom settings for a fixed number of bins or bin width.
        /// </summary>
        /// 
        public void Reset()
        {
            binWidth = null;
            numberOfBins = null;
            onDataBind();
        }

        private void onDataBind()
        {
            samples = null;

            if (dataSource == null)
                return;

            Histogram source = dataSource as Histogram;

            if (source == null)
            {
                if (histogram == null)
                    histogram = new Histogram();

                if (dataSource is DataSet)
                {
                    // throw new NotSupportedException();
                }
                else if (dataSource is DataTable)
                {
                    DataTable table = dataSource as DataTable;

                    if (dataMember != null && dataMember.Length > 0)
                    {
                        if (table.Columns.Contains(dataMember))
                        {
                            DataColumn column = table.Columns[dataMember];
                            samples = Matrix.ToArray(column);
                        }
                        else
                        {
                            samples = new double[0];
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else if (dataSource is double[])
                {
                    samples = dataSource as double[];
                }
                else if (dataSource is IListSource)
                {
                    // throw new NotSupportedException();
                }
                else
                {
                    return; // invalid data source
                }

                if (binWidth != null)
                    this.histogram.Compute(samples, binWidth.Value);
                else if (numberOfBins != null)
                    this.histogram.Compute(samples, numberOfBins.Value);
                else
                    this.histogram.Compute(samples);
            }
            else
            {
                this.histogram = source;
            }

            this.UpdateTrackbar();
            this.UpdateGraph();

            if (histogram.Bins.Count > 0 &&
                histogram.Bins.Count < trackBar.Maximum)
                trackBar.Value = histogram.Bins.Count;
        }

        private void trackBar_ValueChanged(object sender, EventArgs e)
        {
            if (histogram == null)
                return;

            if (samples == null)
                return;

            histogram.Compute(samples, (int)trackBar.Value);

            this.UpdateGraph();
        }

    }
}
