// Accord.NET Sample Applications
// http://accord-framework.net
//
// Copyright © 2009-2014, César Souza
// All rights reserved. 3-BSD License:
//
//   Redistribution and use in source and binary forms, with or without
//   modification, are permitted provided that the following conditions are met:
//
//      * Redistributions of source code must retain the above copyright
//        notice, this list of conditions and the following disclaimer.
//
//      * Redistributions in binary form must reproduce the above copyright
//        notice, this list of conditions and the following disclaimer in the
//        documentation and/or other materials provided with the distribution.
//
//      * Neither the name of the Accord.NET Framework authors nor the
//        names of its contributors may be used to endorse or promote products
//        derived from this software without specific prior written permission.
// 
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
//  DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//  ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;
using Accord.Controls;
using Accord.Math;
using Accord.Statistics.Analysis;
using Accord.Statistics.Formats;
using Accord.Statistics.Models.Regression.Linear;
using Components;

namespace Analysis.PLS
{

    public partial class MainForm : Form
    {

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

            openFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Resources");
        }

        private void DataAnalyzer_Load(object sender, EventArgs e)
        {
            Array methods = Enum.GetValues(typeof(AnalysisMethod));
            Array algorithms = Enum.GetValues(typeof(PartialLeastSquaresAlgorithm));

            this.cbMethod.DataSource = methods;
            this.cbAlgorithm.DataSource = algorithms;
        }


        private void btnCompute_Click(object sender, EventArgs e)
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

            distributionView.DataSource = pls.Factors;
            cumulativeView.DataSource = pls.Factors;
            dgvProjectionSourceX.DataSource = inputTable;
            dgvProjectionSourceY.DataSource = outputTable;

            dgvRegressionInput.DataSource = table.DefaultView.ToTable(false, 
                inputColumnNames.Concatenate(outputColumnNames));
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


        private void numComponentsRegression_ValueChanged(object sender, EventArgs e)
        {
            int num = (int)numComponentsRegression.Value;

            dgvRegressionComponents.ClearSelection();
            for (int i = 0; i < num && i < dgvRegressionComponents.Rows.Count; i++)
                dgvRegressionComponents.Rows[i].Selected = true;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog(this);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}