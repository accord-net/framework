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
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Accord.Math;
using Accord.Statistics.Analysis;
using Accord.Statistics.Formats;

namespace Survival
{
    public partial class MainForm : Form
    {

        private ProportionalHazardsAnalysis pha;
        private DataTable sourceTable;


        public MainForm()
        {
            InitializeComponent();

            dgvLogisticCoefficients.AutoGenerateColumns = false;
            dgvDistributionMeasures.AutoGenerateColumns = false;

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

                        this.cbTimeName.Items.Clear();
                        this.cbEventName.Items.Clear();
                        this.checkedListBox1.Items.Clear();
                        foreach (DataColumn col in sourceTable.Columns)
                        {
                            this.cbTimeName.Items.Add(col.ColumnName);
                            this.cbEventName.Items.Add(col.ColumnName);
                            this.checkedListBox1.Items.Add(col.ColumnName);
                        }

                        this.cbTimeName.SelectedIndex = 0;
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



            // Finishes and save any pending changes to the given data
            dgvAnalysisSource.EndEdit();
            sourceTable.AcceptChanges();

            // Gets the column of the dependent variable
            String dependentName = (string)cbTimeName.SelectedItem;
            String censorName = (string)cbEventName.SelectedItem;
            DataTable timeTable = sourceTable.DefaultView.ToTable(false, dependentName);
            DataTable censorTable = sourceTable.DefaultView.ToTable(false, censorName);

            // Gets the columns of the independent variables
            List<string> names = new List<string>();
            foreach (string name in checkedListBox1.CheckedItems)
                names.Add(name);

            String[] independentNames = names.ToArray();

            // Creates the input and output matrices from the source data table
            double[][] input;
            double[] time = timeTable.Columns[dependentName].ToArray();
            int[] censor = censorTable.Columns[censorName].ToArray().ToInt32();

            if (independentNames.Length == 0)
            {
                input = new double[time.Length][];
                for (int i = 0; i < input.Length; i++)
                    input[i] = new double[0];
            }
            else
            {
                DataTable independent = sourceTable.DefaultView.ToTable(false, independentNames);
                input = independent.ToArray();
            }


            String[] sourceColumns;
            double[,] sourceMatrix = sourceTable.ToMatrix(out sourceColumns);

            // Creates the Simple Descriptive Analysis of the given source
            DescriptiveAnalysis sda = new DescriptiveAnalysis(sourceMatrix, sourceColumns);
            sda.Compute();

            // Populates statistics overview tab with analysis data
            dgvDistributionMeasures.DataSource = sda.Measures;


            // Creates the Logistic Regression Analysis of the given source
            pha = new ProportionalHazardsAnalysis(input, time, censor,
                independentNames, dependentName, censorName);


            // Compute the Logistic Regression Analysis
            pha.Compute();

            // Populates coefficient overview with analysis data
            dgvLogisticCoefficients.DataSource = pha.Coefficients;

            // Populate details about the fitted model
            tbChiSquare.Text = pha.ChiSquare.Statistic.ToString("N5");
            tbPValue.Text = pha.ChiSquare.PValue.ToString("N5");
            checkBox1.Checked = pha.ChiSquare.Significant;
            tbDeviance.Text = pha.Deviance.ToString("N5");
            tbLogLikelihood.Text = pha.LogLikelihood.ToString("N5");


            // Populate projection source table
            string[] cols = independentNames;
            if (!independentNames.Contains(dependentName))
                cols = cols.Concatenate(dependentName);

            if (!independentNames.Contains(censorName))
                cols = cols.Concatenate(censorName);

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


            DataTable dependent = source.DefaultView.ToTable(false, pha.TimeName);

             double[][] input;
             double[] times = dependent.Columns[pha.TimeName].ToArray();
             double[] output = new double[times.Length];

            if (pha.InputNames.Length == 0)
            {
                input = new double[times.Length][];
                for (int i = 0; i < input.Length; i++)
                    input[i] = new double[0];
            }
            else
            {
                DataTable independent = sourceTable.DefaultView.ToTable(false, pha.InputNames);
                input = independent.ToArray();
            }



            for (int i = 0; i < input.Length; i++)
            {
                double[] x = input[i];

                output[i] = pha.Regression.Compute(x, times[i]);
            }


            DataTable result = source.Clone();
            for (int i = 0; i < input.Length; i++)
            {
                DataRow row = result.NewRow();
                for (int j = 0; j < pha.InputNames.Length; j++)
                {
                    row[pha.InputNames[j]] = input[i][j];
                }

                row[pha.TimeName] = times[i];
                row[pha.EventName] = output[i];

                result.Rows.Add(row);
            }

            dgvProjectionResult.DataSource = result;

        }


    }
}
