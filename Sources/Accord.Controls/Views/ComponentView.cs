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
    using System.Windows.Forms;
    using Accord.Math;
    using Accord.Statistics.Visualizations;
    using ZedGraph;
    using System.Collections.Generic;
    using Accord.Statistics.Analysis;
    using System.Linq;

    /// <summary>
    ///   Component visualization control.
    /// </summary>
    /// 
    public partial class ComponentView : UserControl
    {

        IEnumerable<IAnalysisComponent> source;
        ColorSequenceCollection colors;



        /// <summary>
        ///   Constructs a new instance of the ScatterplotView.
        /// </summary>
        /// 
        public ComponentView()
        {
            InitializeComponent();

            zedGraphControl.GraphPane.Title.Text = "Components";
            zedGraphControl.GraphPane.Title.FontSpec.Size = 24f;
            zedGraphControl.GraphPane.Title.FontSpec.Family = "Tahoma";

            zedGraphControl.GraphPane.XAxis.Title.Text = "Components";
            zedGraphControl.GraphPane.YAxis.Title.Text = "Percentage";

            zedGraphControl.BorderStyle = System.Windows.Forms.BorderStyle.None;
            zedGraphControl.GraphPane.Border.IsVisible = false;
            zedGraphControl.GraphPane.Border.Color = Color.White;
            zedGraphControl.GraphPane.Border.Width = 0;
            zedGraphControl.GraphPane.Fill = new Fill(Color.White);
            zedGraphControl.GraphPane.Chart.Fill = new Fill(Color.GhostWhite);

            zedGraphControl.GraphPane.Legend.IsVisible = false;
            zedGraphControl.GraphPane.Legend.Position = LegendPos.Right;
            zedGraphControl.GraphPane.Legend.IsShowLegendSymbols = false;

            zedGraphControl.GraphPane.XAxis.Scale.MinAuto = true;
            zedGraphControl.GraphPane.XAxis.Scale.MaxAuto = true;
            zedGraphControl.GraphPane.YAxis.Scale.MinAuto = true;
            zedGraphControl.GraphPane.YAxis.Scale.MaxAuto = true;
            zedGraphControl.GraphPane.XAxis.Scale.MagAuto = true;
            zedGraphControl.GraphPane.YAxis.Scale.MagAuto = true;


            zedGraphControl.GraphPane.Chart.Fill.Type = FillType.None;
            zedGraphControl.GraphPane.Legend.IsVisible = false;

            zedGraphControl.GraphPane.Title.FontSpec.Size = 24f;
            zedGraphControl.GraphPane.Title.FontSpec.Family = "Tahoma";

            colors = new ColorSequenceCollection();
        }



        /// <summary>
        ///   Constructs a new instance of the ScatterplotView.
        /// </summary>
        /// 
        public ComponentView(ICollection<IAnalysisComponent> components)
            : this()
        {
            this.DataSource = components;
        }


        /// <summary>
        ///   Gets the underlying scatter plot being shown by this control.
        /// </summary>
        /// 
        public IEnumerable<IAnalysisComponent> DataSource
        {
            get { return source; }
            set
            {
                if (source != value)
                {
                    source = value;
                    UpdateGraph();
                }
            }
        }



        /// <summary>
        ///   Gets a reference to the underlying ZedGraph
        ///   control used to draw the scatter plot.
        /// </summary>
        /// 
        public ZedGraphControl Graph
        {
            get { return zedGraphControl; }
        }

        /// <summary>
        ///   Gets or sets whether this control should present
        ///   the individual proportion for each component, or
        ///   the cumulative proportion in a single line curve.
        /// </summary>
        /// 
        public bool Cumulative { get; set; }



        /// <summary>
        ///   Forces a update of the scatter plot.
        /// </summary>
        /// 
        public void UpdateGraph()
        {
            GraphPane pane = zedGraphControl.GraphPane;

            pane.CurveList.Clear();

            if (Cumulative)
            {
                // Set the titles and axis labels
                pane.Title.Text = "Distribution";
                pane.XAxis.Title.IsVisible = true;
                pane.YAxis.Title.IsVisible = true;
                
                // Add points for distribution
                var list = new PointPairList();
                foreach (var component in source)
                    list.Add(component.Index, component.CumulativeProportion);

                // Add a single curve
                LineItem curve = pane.AddCurve("label", list, Color.Red, SymbolType.Circle);
                curve.Line.Width = 2.0F;
                curve.Line.IsAntiAlias = true;
                curve.Symbol.Fill = new Fill(Color.White);
                curve.Symbol.Size = 7;
            }
            else
            {
                // Set the titles and axis labels
                pane.Title.Text = "Component proportion";
                pane.XAxis.Title.IsVisible = false;
                pane.YAxis.Title.IsVisible = false;
                
                // Add pie slices for shares
                foreach (var component in source)
                {
                    int index = component.Index;
                    Color color = colors[index % colors.Count];
                    pane.AddPieSlice(component.Proportion, color, 0.1, index.ToString());
                }
            }

            // Calculate the Axis Scale
            zedGraphControl.AxisChange();
            zedGraphControl.Invalidate();
        }

    }
}
