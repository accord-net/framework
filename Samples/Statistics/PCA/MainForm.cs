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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using ZedGraph;

using Accord.Statistics.Analysis;
using Accord.Controls;
using Accord.Math;
using Accord.Statistics.Formats;


namespace Components
{

    public partial class MainForm : System.Windows.Forms.Form
    {

        // Colors used in the pie graphics
        private readonly Color[] colors = { Color.YellowGreen, Color.DarkOliveGreen, Color.DarkKhaki, Color.Olive,
            Color.Honeydew, Color.PaleGoldenrod, Color.Indigo, Color.Olive, Color.SeaGreen };


        private PrincipalComponentAnalysis pca;
        private DescriptiveAnalysis sda;

        string[] sourceColumnNames;



        public MainForm()
        {
            InitializeComponent();

            dgvAnalysisSource.AutoGenerateColumns = true;
            dgvDistributionMeasures.AutoGenerateColumns = false;
            dgvFeatureVectors.AutoGenerateColumns = true;
            dgvPrincipalComponents.AutoGenerateColumns = false;
            dgvProjectionComponents.AutoGenerateColumns = false;
            dgvProjectionResult.AutoGenerateColumns = true;

            openFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Resources");
        }

        private void DataAnalyzer_Load(object sender, EventArgs e)
        {
            Array methods = Enum.GetValues(typeof(AnalysisMethod));
            this.tscbMethod.ComboBox.DataSource = methods;
            this.cbMethod.DataSource = methods;
        }


        #region Buttons
        private void btnRunAnalysis_Click(object sender, EventArgs e)
        {
            if (dgvAnalysisSource.DataSource == null)
            {
                MessageBox.Show("Please load some data first.");
                return;
            }


            // Finishes and save any pending changes to the given data
            dgvAnalysisSource.EndEdit();

            // Creates a matrix from the source data table
            double[,] sourceMatrix = (dgvAnalysisSource.DataSource as DataTable).ToMatrix(out sourceColumnNames);

            // Creates the Simple Descriptive Analysis of the given source
            sda = new DescriptiveAnalysis(sourceMatrix, sourceColumnNames);

            sda.Compute();


            // Populates statistics overview tab with analysis data
            dgvStatisticCenter.DataSource = new ArrayDataView(sda.DeviationScores, sourceColumnNames);
            dgvStatisticStandard.DataSource = new ArrayDataView(sda.StandardScores, sourceColumnNames);

            dgvStatisticCovariance.DataSource = new ArrayDataView(sda.CovarianceMatrix, sourceColumnNames);
            dgvStatisticCorrelation.DataSource = new ArrayDataView(sda.CorrelationMatrix, sourceColumnNames);
            dgvDistributionMeasures.DataSource = sda.Measures;


            // Creates the Principal Component Analysis of the given source
            pca = new PrincipalComponentAnalysis(sda.Source,
                (AnalysisMethod)cbMethod.SelectedValue);


            // Compute the Principal Component Analysis
            pca.Compute();

            // Populates components overview with analysis data
            dgvFeatureVectors.DataSource = new ArrayDataView(pca.ComponentMatrix);

            dgvPrincipalComponents.DataSource = pca.Components;

            dgvProjectionComponents.DataSource = pca.Components;
            numComponents.Maximum = pca.Components.Count;
            numComponents.Value = 1;
            numThreshold.Value = (decimal)pca.Components[0].CumulativeProportion * 100;

            CreateComponentCumulativeDistributionGraph(graphCurve);
            CreateComponentDistributionGraph(graphShare);

        }

        private void btnProject_Click(object sender, EventArgs e)
        {
            if (pca == null || dgvProjectionSource.DataSource == null)
            {
                MessageBox.Show("Please compute the analysis first.");
                return;
            }

            string[] colNames;
            int components = (int)numComponents.Value;
            double[,] projectionSource = (dgvProjectionSource.DataSource as DataTable).ToMatrix(out colNames);

            // Compute the projection
            double[,] projection = pca.Transform(projectionSource, components);

            dgvProjectionResult.DataSource = new ArrayDataView(projection, GenerateComponentNames(components));
            dgvReversionSource.DataSource = dgvProjectionResult.DataSource;
        }

        private void btnRevert_Click(object sender, EventArgs e)
        {
            if (dgvReversionSource.DataSource == null)
            {
                MessageBox.Show("Please compute a projection transformation first.");
                return;
            }

            double[,] reversionSource = (double[,])(dgvReversionSource.DataSource as ArrayDataView).ArrayData;

            // Compute the projection reversion
            double[,] reversion = pca.Revert(reversionSource);

            dgvReversionResult.DataSource = new ArrayDataView(reversion, sourceColumnNames);
        }
        #endregion


        #region Menus
        private void MenuFileOpen_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string filename = openFileDialog.FileName;
                string extension = Path.GetExtension(filename);
                if (extension == ".xls" || extension == ".xlsx")
                {
                    ExcelReader db = new ExcelReader(filename, true, false);
                    TableSelectDialog t = new TableSelectDialog(db.GetWorksheetList());

                    if (t.ShowDialog(this) == DialogResult.OK)
                    {
                        this.dgvAnalysisSource.DataSource = db.GetWorksheet(t.Selection);
                        this.dgvProjectionSource.DataSource = db.GetWorksheet(t.Selection);
                    }
                }
                else if (extension == ".xml")
                {
                    DataTable dataTableAnalysisSource = new DataTable();
                    dataTableAnalysisSource.ReadXml(openFileDialog.FileName);

                    this.dgvAnalysisSource.DataSource = dataTableAnalysisSource;
                    this.dgvProjectionSource.DataSource = dataTableAnalysisSource.Clone();
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                (dgvAnalysisSource.DataSource as DataTable).WriteXml(saveFileDialog1.FileName, XmlWriteMode.WriteSchema);
            }
        }
        #endregion


        #region Graphs
        public void CreateComponentCumulativeDistributionGraph(ZedGraphControl zgc)
        {
            GraphPane myPane = zgc.GraphPane;

            myPane.CurveList.Clear();

            // Set the titles and axis labels
            myPane.Title.Text = "Component Distribution";
            myPane.Title.FontSpec.Size = 24f;
            myPane.Title.FontSpec.Family = "Tahoma";
            myPane.XAxis.Title.Text = "Components";
            myPane.YAxis.Title.Text = "Percentage";

            PointPairList list = new PointPairList();
            for (int i = 0; i < pca.Components.Count; i++)
            {
                list.Add(pca.Components[i].Index, pca.Components[i].CumulativeProportion);
            }

            // Hide the legend
            myPane.Legend.IsVisible = false;

            // Add a curve
            LineItem curve = myPane.AddCurve("label", list, Color.Red, SymbolType.Circle);
            curve.Line.Width = 2.0F;
            curve.Line.IsAntiAlias = true;
            curve.Symbol.Fill = new Fill(Color.White);
            curve.Symbol.Size = 7;

            myPane.XAxis.Scale.MinAuto = true;
            myPane.XAxis.Scale.MaxAuto = true;
            myPane.YAxis.Scale.MinAuto = true;
            myPane.YAxis.Scale.MaxAuto = true;
            myPane.XAxis.Scale.MagAuto = true;
            myPane.YAxis.Scale.MagAuto = true;


            // Calculate the Axis Scale Ranges
            zgc.AxisChange();
            zgc.Invalidate();
        }

        public void CreateComponentDistributionGraph(ZedGraphControl zgc)
        {
            GraphPane myPane = zgc.GraphPane;
            myPane.CurveList.Clear();

            // Set the GraphPane title
            myPane.Title.Text = "Component Proportion";
            myPane.Title.FontSpec.Size = 24f;
            myPane.Title.FontSpec.Family = "Tahoma";

            // Fill the pane background with a color gradient
            //  myPane.Fill = new Fill(Color.White, Color.WhiteSmoke, 45.0f);
            // No fill for the chart background
            myPane.Chart.Fill.Type = FillType.None;

            myPane.Legend.IsVisible = false;

            // Add some pie slices
            for (int i = 0; i < pca.Components.Count; i++)
            {
                myPane.AddPieSlice(pca.Components[i].Proportion, colors[i % colors.Length], 0.1, pca.Components[i].Index.ToString());
            }

            myPane.XAxis.Scale.MinAuto = true;
            myPane.XAxis.Scale.MaxAuto = true;
            myPane.YAxis.Scale.MinAuto = true;
            myPane.YAxis.Scale.MaxAuto = true;
            myPane.XAxis.Scale.MagAuto = true;
            myPane.YAxis.Scale.MagAuto = true;


            // Calculate the Axis Scale Ranges
            zgc.AxisChange();

            zgc.Invalidate();
        }
        #endregion


        #region Events
        private void numComponents_ValueChanged(object sender, EventArgs e)
        {
            if (rbComponents.Checked)
            {
                int num = (int)numComponents.Value;
                numThreshold.Value = (decimal)pca.CumulativeProportions[num - 1] * 100;

                dgvProjectionComponents.ClearSelection();
                for (int i = 0; i < num && i < dgvProjectionComponents.Rows.Count; i++)
                    dgvProjectionComponents.Rows[i].Selected = true;
            }
        }

        private void numThreshold_ValueChanged(object sender, EventArgs e)
        {
            if (rbThreshold.Checked)
            {
                int num = pca.GetNumberOfComponents((float)numThreshold.Value / 100);
                numComponents.Value = num;

                dgvProjectionComponents.ClearSelection();
                for (int i = 0; i < num && i < dgvProjectionComponents.Rows.Count; i++)
                    dgvProjectionComponents.Rows[i].Selected = true;
            }
        }

        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {
            if (dgvDistributionMeasures.CurrentRow != null)
            {
                DataGridViewRow row = (DataGridViewRow)dgvDistributionMeasures.CurrentRow;
                dataHistogramView1.DataSource =
                    ((DescriptiveMeasures)row.DataBoundItem).Samples;
            }
        }
        #endregion




        private string[] GenerateComponentNames(int number)
        {
            string[] names = new string[number];
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = "Component " + (i + 1);
            }
            return names;
        }

        private void rbComponents_CheckedChanged(object sender, EventArgs e)
        {
            numComponents.Enabled = true;
            numThreshold.Enabled = false;
        }

        private void rbThreshold_CheckedChanged(object sender, EventArgs e)
        {
            numComponents.Enabled = false;
            numThreshold.Enabled = true;
        }



    }
}