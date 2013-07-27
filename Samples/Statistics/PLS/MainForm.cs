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
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Accord.Controls;
using Accord.Math;
using Accord.Statistics.Analysis;
using Accord.Statistics.Formats;
using Accord.Statistics.Models.Regression.Linear;
using ZedGraph;

namespace PLS
{

    public partial class MainForm : System.Windows.Forms.Form
    {

        // Colors used in the pie graphics
        private readonly Color[] colors = { Color.YellowGreen, Color.DarkOliveGreen, Color.DarkKhaki, Color.Olive,
            Color.Honeydew, Color.PaleGoldenrod, Color.Indigo, Color.Olive, Color.SeaGreen };


        //     private DataTable sourceTable;
        private PartialLeastSquaresAnalysis pls;
        private DescriptiveAnalysis sda;
        private MultivariateLinearRegression regression;

        string[] inputColumnNames;
        string[] outputColumnNames;



        public MainForm()
        {
            InitializeComponent();

            dgvAnalysisSource.AutoGenerateColumns = true;
            dgvDistributionMeasures.AutoGenerateColumns = false;
            dgvWeightMatrix.AutoGenerateColumns = true;
            dgvFactors.AutoGenerateColumns = false;
            dgvProjectionComponents.AutoGenerateColumns = false;
            dgvRegressionComponents.AutoGenerateColumns = false;
            //dgvProjectionResult.AutoGenerateColumns = true;

            openFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Resources");
        }

        private void DataAnalyzer_Load(object sender, EventArgs e)
        {
            Array methods = Enum.GetValues(typeof(AnalysisMethod));
            this.tscbMethod.ComboBox.DataSource = methods;
            this.cbMethod.DataSource = methods;

            Array algorithms = Enum.GetValues(typeof(PartialLeastSquaresAlgorithm));
            this.tscbAlgorithm.ComboBox.DataSource = algorithms;
            this.cbAlgorithm.DataSource = algorithms;
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
            DataTable table = dgvAnalysisSource.DataSource as DataTable;

            // Creates a matrix from the source data table
            double[,] sourceMatrix = table.ToMatrix(out inputColumnNames);

            // Creates the Simple Descriptive Analysis of the given source
            sda = new DescriptiveAnalysis(sourceMatrix, inputColumnNames);
            sda.Compute();

            // Populates statistics overview tab with analysis data
            dgvDistributionMeasures.DataSource = sda.Measures;


            // Extract variables
            List<string> inputNames = new List<string>();
            foreach (string name in clbInput.CheckedItems)
                inputNames.Add(name);
            this.inputColumnNames = inputNames.ToArray();

            List<string> outputNames = new List<string>();
            foreach (string name in clbOutput.CheckedItems)
                outputNames.Add(name);
            this.outputColumnNames = outputNames.ToArray();

            DataTable inputTable = table.DefaultView.ToTable(false, inputColumnNames);
            DataTable outputTable = table.DefaultView.ToTable(false, outputColumnNames);

            double[,] inputs = inputTable.ToMatrix();
            double[,] outputs = outputTable.ToMatrix();



            // Creates the Partial Least Squares of the given source
            pls = new PartialLeastSquaresAnalysis(inputs, outputs,
                (AnalysisMethod)cbMethod.SelectedValue,
                (PartialLeastSquaresAlgorithm)cbAlgorithm.SelectedValue);


            // Computes the Partial Least Squares
            pls.Compute();


            // Populates components overview with analysis data
            dgvWeightMatrix.DataSource = new ArrayDataView(pls.Weights);
            dgvFactors.DataSource = pls.Factors;

            dgvAnalysisLoadingsInput.DataSource = new ArrayDataView(pls.Predictors.FactorMatrix);
            dgvAnalysisLoadingsOutput.DataSource = new ArrayDataView(pls.Dependents.FactorMatrix);

            this.regression = pls.CreateRegression();
            dgvRegressionCoefficients.DataSource = new ArrayDataView(regression.Coefficients, outputColumnNames);
            dgvRegressionIntercept.DataSource = new ArrayDataView(regression.Intercepts, outputColumnNames);

            dgvProjectionComponents.DataSource = pls.Factors;
            numComponents.Maximum = pls.Factors.Count;
            numComponents.Value = 1;

            dgvRegressionComponents.DataSource = pls.Factors;
            numComponentsRegression.Maximum = pls.Factors.Count;
            numComponentsRegression.Value = 1;

            CreateComponentCumulativeDistributionGraph(graphCurve);
            CreateComponentDistributionGraph(graphShare);

            dgvProjectionSourceX.DataSource = inputTable;
            dgvProjectionSourceY.DataSource = outputTable;

            dgvRegressionInput.DataSource = table.DefaultView.ToTable(false, inputColumnNames.Concatenate(outputColumnNames));
        }

        private void btnProject_Click(object sender, EventArgs e)
        {
            if (pls == null || dgvProjectionSourceX.DataSource == null || dgvProjectionSourceY.DataSource == null)
            {
                MessageBox.Show("Please compute the analysis first.");
                return;
            }

            int components = (int)numComponents.Value;
            double[,] sourceInput = Matrix.ToMatrix(dgvProjectionSourceX.DataSource as DataTable);
            double[,] sourceOutput = Matrix.ToMatrix(dgvProjectionSourceY.DataSource as DataTable);

            double[,] input = pls.Transform(sourceInput, components);
            double[,] output = pls.TransformOutput(sourceOutput, components);

            dgvProjectionX.DataSource = new ArrayDataView(input);
            dgvProjectionY.DataSource = new ArrayDataView(output);
        }


        private void btnRegress_Click(object sender, EventArgs e)
        {
            if (pls == null || dgvRegressionInput.DataSource == null)
            {
                MessageBox.Show("Please compute the analysis first.");
                return;
            }

            int components = (int)numComponentsRegression.Value;
            regression = pls.CreateRegression(components);

            DataTable table = dgvRegressionInput.DataSource as DataTable;
            DataTable inputTable = table.DefaultView.ToTable(false, inputColumnNames);
            DataTable outputTable = table.DefaultView.ToTable(false, outputColumnNames);


            double[][] sourceInput = Matrix.ToArray(inputTable as DataTable);
            double[][] sourceOutput = Matrix.ToArray(outputTable as DataTable);

            
            double[,] result = Matrix.ToMatrix(regression.Compute(sourceInput));

            double[] rSquared = regression.CoefficientOfDetermination(sourceInput, sourceOutput, cbAdjusted.Checked);

            dgvRegressionOutput.DataSource = new ArrayDataView(result, outputColumnNames);
            dgvRSquared.DataSource = new ArrayDataView(rSquared, outputColumnNames);
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
                        DataTable table = db.GetWorksheet(t.Selection);
                        this.dgvAnalysisSource.DataSource = table;

                        clbInput.Items.Clear();
                        clbOutput.Items.Clear();

                        foreach (DataColumn col in table.Columns)
                        {
                            clbInput.Items.Add(col.ColumnName);
                            clbOutput.Items.Add(col.ColumnName);
                        }


                    }
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
            for (int i = 0; i < pls.Factors.Count; i++)
            {
                list.Add(pls.Factors[i].Index, pls.Factors[i].DependentCumulativeProportion);
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
            double cumulative = 0.0;
            for (int i = 0; i < pls.Factors.Count; i++)
            {
                cumulative = pls.Factors[i].DependentCumulativeProportion;
                myPane.AddPieSlice(pls.Factors[i].DependentProportion,
                    colors[i % colors.Length],
                    0.1, pls.Factors[i].Index.ToString());
            }

            double unexplained = 1 - cumulative;
            myPane.AddPieSlice(unexplained, Color.LightGray, 0.1, "Unexplained");

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
            int num = (int)numComponents.Value;

            dgvProjectionComponents.ClearSelection();
            for (int i = 0; i < num && i < dgvProjectionComponents.Rows.Count; i++)
                dgvProjectionComponents.Rows[i].Selected = true;

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

        private void numComponentsRegression_ValueChanged(object sender, EventArgs e)
        {
            int num = (int)numComponentsRegression.Value;

            dgvRegressionComponents.ClearSelection();
            for (int i = 0; i < num && i < dgvRegressionComponents.Rows.Count; i++)
                dgvRegressionComponents.Rows[i].Selected = true;
        }






    }
}