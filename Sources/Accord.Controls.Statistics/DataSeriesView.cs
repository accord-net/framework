// Accord Statistics Controls Library
// The Accord.NET Framework
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2012
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
    using System.Windows.Forms;
    using Accord.Math;
    using Accord.Statistics.Visualizations;
    using ZedGraph;
    using System.Threading;
    using System.Collections.Specialized;
using System.Collections.ObjectModel;

    [Serializable]
    public class DataChartViewSeries
    {
        public Color Color { get; set; }
        public String Label { get; set; }
        public String DataPropertyName { get; set; }
    }

    /// <summary>
    ///   Scatterplot visualization control.
    /// </summary>
    /// 
    public partial class DataChartView : UserControl
    {

        private CurveList series;
        private object dataSource;

        private string title;
        private string xAxisDataMember;
        private Collection<DataChartViewSeries> seriesDataMember;


        #region Constructor

        /// <summary>
        ///   Constructs a new instance of the ScatterplotView.
        /// </summary>
        /// 
        public DataChartView(Scatterplot scatterplot)
        {
            InitializeComponent();

            series = new CurveList();
            seriesDataMember = new Collection<DataChartViewSeries>();

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
        #endregion


        #region Properties

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
                    OnDataBind();
            }
        }

        /// <summary>
        ///   Gets or sets the member of the data source 
        ///   to be shown, if applicable.
        /// </summary>
        /// 
        [DefaultValue(null)]
        public string DataMemberX
        {
            get { return xAxisDataMember; }
            set
            {
                xAxisDataMember = value;

                if (!this.DesignMode)
                    OnDataBind();
            }
        }

        /// <summary>
        ///   Gets or sets the member of the data source 
        ///   to be shown, if applicable.
        /// </summary>
        /// 
        [DefaultValue(null)]
        public Collection<DataChartViewSeries> DataMemberY
        {
            get { return seriesDataMember; }
        }

        /// <summary>
        ///   Gets a reference to the underlying ZedGraph
        ///   control used to draw the scatterplot.
        /// </summary>
        /// 
        public ZedGraphControl Graph
        {
            get { return zedGraphControl; }
        }

        #endregion


        /// <summary>
        ///   Forces a update of the scatter plot.
        /// </summary>
        /// 
        public void UpdateGraph()
        {
            zedGraphControl.GraphPane.Title.Text = scatterplot.Title;
            zedGraphControl.GraphPane.XAxis.Title.Text = Scatterplot.XAxisTitle;
            zedGraphControl.GraphPane.YAxis.Title.Text = Scatterplot.YAxisTitle;

            classes.Clear();

            if (scatterplot.Classes != null)
            {
                if (scatterplot.Classes.Count == 0)
                {
                    zedGraphControl.GraphPane.Legend.IsVisible = false;

                    // Create space for unlabelled data
                    PointPairList list = new PointPairList(scatterplot.XAxis, scatterplot.YAxis);

                    LineItem item = new LineItem(String.Empty, list, Color.Black, SymbolType.Default);

                    item.Line.IsVisible = false;
                    item.Symbol.Border.IsVisible = false;
                    item.Symbol.Fill = new Fill(Color.Black);

                    classes.Add(item);
                }
                else
                {
                    zedGraphControl.GraphPane.Legend.IsVisible = true;
                    var colors = new ColorSequenceCollection(scatterplot.Classes.Count);

                    // Create a curve item for each of the labels
                    for (int i = 0; i < scatterplot.Classes.Count; i++)
                    {
                        // retrieve the x,y pairs for the label
                        double[] x = scatterplot.Classes[i].XAxis;
                        double[] y = scatterplot.Classes[i].YAxis;
                        PointPairList list = new PointPairList(x, y);

                        LineItem item = new LineItem(scatterplot.Classes[i].Label.ToString(),
                            list, colors[i], SymbolType.Default);

                        item.Line.IsVisible = false;
                        item.Symbol.Border.IsVisible = false;
                        item.Symbol.Fill = new Fill(colors[i]);

                        classes.Add(item);
                    }
                }

                zedGraphControl.AxisChange();
                zedGraphControl.Invalidate();

                zedGraphControl.ZoomPane(zedGraphControl.GraphPane, 1.1, PointF.Empty, false);
            }
        }



        private void OnDataBind()
        {
            if (dataSource == null)
                return;

            double[] x = null;
            double[] y = null;
            int[] z = null;

            if (dataSource is DataTable)
            {
                DataTable table = dataSource as DataTable;

                if (String.IsNullOrEmpty(xAxisDataMember) &&
                    table.Columns.Contains(xAxisDataMember))
                    x = table.Columns[xAxisDataMember].ToArray();

                if (String.IsNullOrEmpty(yAxisDataMember) &&
                    table.Columns.Contains(yAxisDataMember))
                    y = table.Columns[yAxisDataMember].ToArray();

                if (String.IsNullOrEmpty(labelDataMember) &&
                    table.Columns.Contains(labelDataMember))
                    z = table.Columns[labelDataMember].ToArray().ToInt32();
            }
            else if (dataSource is double[][])
            {
                double[][] source = dataSource as double[][];

                if (source.Length > 0)
                {
                    if (source[0].Length > 0)
                        x = source.GetColumn(0);

                    if (source[0].Length > 1)
                        y = source.GetColumn(1);

                    if (source[0].Length > 2)
                        z = source.GetColumn(2).ToInt32();
                }
            }
            else if (dataSource is double[,])
            {
                double[,] source = dataSource as double[,];

                if (source.Length > 0)
                {
                    int cols = source.GetLength(1);

                    if (cols > 0)
                        x = source.GetColumn(0);

                    if (cols > 1)
                        y = source.GetColumn(1);

                    if (cols > 2)
                        z = source.GetColumn(2).ToInt32();
                }
            }
            else
            {
                return; // invalid data source
            }

            if (x != null && y == null)
                y = new double[x.Length];

            else if (y != null && x == null)
                x = new double[y.Length];

            this.scatterplot.Compute(x, y, z);

            this.UpdateGraph();
        }


    }
}
