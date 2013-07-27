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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Accord.Statistics.Analysis;
using Components;
using System.IO;

using Accord.Math;
using Accord.Controls;
using Accord.Statistics.Models.Regression.Linear;
using Accord.Statistics.Formats;

namespace Regression
{
    public partial class MainForm : Form
    {

        private LogisticRegressionAnalysis lra;
        private MultipleLinearRegressionAnalysis mlr;
        private DataTable sourceTable;


        public MainForm()
        {
            InitializeComponent();

            dgvLogisticCoefficients.AutoGenerateColumns = false;
            dgvDistributionMeasures.AutoGenerateColumns = false;
            dgvLinearCoefficients.AutoGenerateColumns = false;

            dgvLinearCoefficients.AllowNestedProperties(true);

            comboBox2.SelectedIndex = 0;

            openFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Resources");
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
                        this.sourceTable = db.GetWorksheet(t.Selection);
                        this.dgvAnalysisSource.DataSource = sourceTable;

                        this.comboBox1.Items.Clear();
                        this.checkedListBox1.Items.Clear();
                        foreach (DataColumn col in sourceTable.Columns)
                        {
                            this.comboBox1.Items.Add(col.ColumnName);
                            this.checkedListBox1.Items.Add(col.ColumnName);
                        }

                        this.comboBox1.SelectedIndex = 0;
                    }
                }
            }
        }

        private void btnSampleRunAnalysis_Click(object sender, EventArgs e)
        {
            // Check requirements
            if (sourceTable == null)
            {
                MessageBox.Show("A sample spreadsheet can be found in the " +
                    "Resources folder in the same directory as this application.",
                    "Please load some data before attempting an analysis");
                return;
            }

            if (checkedListBox1.CheckedItems.Count == 0)
            {
                MessageBox.Show("Please select the dependent input variables to be used in the regression model.",
                    "Please choose at least one input variable");
            }


            // Finishes and save any pending changes to the given data
            dgvAnalysisSource.EndEdit();
            sourceTable.AcceptChanges();

            // Gets the column of the dependent variable
            String dependentName = (string)comboBox1.SelectedItem;
            DataTable dependent = sourceTable.DefaultView.ToTable(false, dependentName);

            // Gets the columns of the independent variables
            List<string> names = new List<string>();
            foreach (string name in checkedListBox1.CheckedItems)
                names.Add(name);

            String[] independentNames = names.ToArray();
            DataTable independent = sourceTable.DefaultView.ToTable(false, independentNames);


            // Creates the input and output matrices from the source data table
            double[][] input = independent.ToArray();
            double[] output = dependent.Columns[dependentName].ToArray();

            double[,] sourceMatrix = sourceTable.ToMatrix(independentNames);

            // Creates the Simple Descriptive Analysis of the given source
            DescriptiveAnalysis sda = new DescriptiveAnalysis(sourceMatrix, independentNames);
            sda.Compute();

            // Populates statistics overview tab with analysis data
            dgvDistributionMeasures.DataSource = sda.Measures;

            // Creates the Logistic Regression Analysis of the given source
            lra = new LogisticRegressionAnalysis(input, output, independentNames, dependentName);


            // Compute the Logistic Regression Analysis
            lra.Compute();

            // Populates coefficient overview with analysis data
            dgvLogisticCoefficients.DataSource = lra.Coefficients;

            // Populate details about the fitted model
            tbChiSquare.Text = lra.ChiSquare.Statistic.ToString("N5");
            tbPValue.Text = lra.ChiSquare.PValue.ToString("N5");
            checkBox1.Checked = lra.ChiSquare.Significant;
            tbDeviance.Text = lra.Deviance.ToString("N5");
            tbLogLikelihood.Text = lra.LogLikelihood.ToString("N5");


            // Create the Multiple Linear Regression Analysis of the given source
            mlr = new MultipleLinearRegressionAnalysis(input, output, independentNames, dependentName, true);

            // Compute the Linear Regression Analysis
            mlr.Compute();

            dgvLinearCoefficients.DataSource = mlr.Coefficients;
            dgvRegressionAnova.DataSource = mlr.Table;

            tbRSquared.Text = mlr.RSquared.ToString("N5");
            tbRSquaredAdj.Text = mlr.RSquareAdjusted.ToString("N5");
            tbChiPValue.Text = mlr.ChiSquareTest.PValue.ToString("N5");
            tbFPValue.Text = mlr.FTest.PValue.ToString("N5");
            tbZPValue.Text = mlr.ZTest.PValue.ToString("N5");
            tbChiStatistic.Text = mlr.ChiSquareTest.Statistic.ToString("N5");
            tbFStatistic.Text = mlr.FTest.Statistic.ToString("N5");
            tbZStatistic.Text = mlr.ZTest.Statistic.ToString("N5");
            cbChiSignificant.Checked = mlr.ChiSquareTest.Significant;
            cbFSignificant.Checked = mlr.FTest.Significant;
            cbZSignificant.Checked = mlr.ZTest.Significant;

            // Populate projection source table
            string[] cols = independentNames;
            if (!independentNames.Contains(dependentName))
                cols = independentNames.Concatenate(dependentName);

            DataTable projSource = sourceTable.DefaultView.ToTable(false, cols);
            dgvProjectionSource.DataSource = projSource;
        }

        private void dgvDistributionMeasures_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dgvDistributionMeasures.CurrentRow != null)
            {
                DescriptiveMeasures m = dgvDistributionMeasures.CurrentRow.DataBoundItem as DescriptiveMeasures;
                dataHistogramView1.DataSource = m.Samples;
            }
        }

        private void btnShift_Click(object sender, EventArgs e)
        {
            DataTable source = dgvProjectionSource.DataSource as DataTable;


            DataTable independent = source.DefaultView.ToTable(false, lra.Inputs);
            DataTable dependent = source.DefaultView.ToTable(false, lra.Output);

            double[][] input = independent.ToArray();
            double[] output;

            if (comboBox2.SelectedItem as string == "Logistic")
            {
                output = lra.Regression.Compute(input);
            }
            else
            {
                output = mlr.Regression.Compute(input);
            }

            DataTable result = source.Clone();
            for (int i = 0; i < input.Length; i++)
            {
                DataRow row = result.NewRow();
                for (int j = 0; j < lra.Inputs.Length; j++)
                {
                    row[lra.Inputs[j]] = input[i][j];
                }
                row[lra.Output] = output[i];

                result.Rows.Add(row);
            }

            dgvProjectionResult.DataSource = result;

        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

    }
}
