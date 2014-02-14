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
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Accord.Controls;
using Accord.Math;
using Accord.Statistics.Analysis;
using Accord.Statistics.Formats;
using Components;

namespace Analysis.LDA
{
    public partial class MainForm : Form
    {

        private LinearDiscriminantAnalysis lda;
        private DescriptiveAnalysis sda;

        string[] columnNames;


        public MainForm()
        {
            InitializeComponent();

            dgvAnalysisSource.AutoGenerateColumns = true;
            dgvDistributionMeasures.AutoGenerateColumns = false;
            dgvFeatureVectors.AutoGenerateColumns = true;
            dgvPrincipalComponents.AutoGenerateColumns = false;
            dgvProjectionComponents.AutoGenerateColumns = false;
            dgvReversionComponents.AutoGenerateColumns = false;
            dgvProjectionResult.AutoGenerateColumns = true;

            openFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Resources");
        }



        /// <summary>
        ///   Launched when the user clicks the File -> Open menu item.
        /// </summary>
        /// 
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
                        DataTable tableSource = db.GetWorksheet(t.Selection);
                        this.dgvAnalysisSource.DataSource = tableSource;
                        this.dgvProjectionSource.DataSource = tableSource.Copy();

                        double[,] graph = tableSource.ToMatrix(out columnNames);
                        inputScatterplot.DataSource = graph;

                        lbStatus.Text = "Now, click 'Compute analysis' to start processing the data!";
                    }
                }
            }
        }


        /// <summary>
        ///   Launched when the user clicks the "Run analysis" button.
        /// </summary>
        /// 
        private void btnCompute_Click(object sender, EventArgs e)
        {
            // Save any pending changes 
            dgvAnalysisSource.EndEdit();

            if (dgvAnalysisSource.DataSource == null)
            {
                MessageBox.Show("Please load some data using File > Open!");
                return;
            }

            // Create a matrix from the source data table
            double[,] sourceMatrix = (dgvAnalysisSource.DataSource as DataTable).ToMatrix(out columnNames);

            int rows = sourceMatrix.GetLength(0);
            int cols = sourceMatrix.GetLength(1);


            // Create and compute a new Simple Descriptive Analysis
            sda = new DescriptiveAnalysis(sourceMatrix, columnNames);

            sda.Compute();

            // Show the descriptive analysis on the screen
            dgvDistributionMeasures.DataSource = sda.Measures;


            // Get the input values (the two first columns)
            double[,] inputs = sourceMatrix.GetColumns(0, 1);

            // Get only the associated labels (last column)
            int[] outputs = sourceMatrix.GetColumn(2).ToInt32();



            // Create a Linear Discriminant Analysis for the data 
            lda = new LinearDiscriminantAnalysis(inputs, outputs);


            lda.Compute(); // Finally, compute the analysis!


            // Perform the transformation of the data using two dimensions
            double[,] result = lda.Transform(inputs, 2);

            // Create a new plot with the original Z column
            double[,] points = result.InsertColumn(sourceMatrix.GetColumn(2));


            // Create output scatter plot
            outputScatterplot.DataSource = points;

            // Create the output table
            dgvProjectionResult.DataSource = new ArrayDataView(points, columnNames);


            // Populate discriminants overview with analysis data
            dgvFeatureVectors.DataSource = new ArrayDataView(lda.DiscriminantMatrix);
            dgvScatterBetween.DataSource = new ArrayDataView(lda.ScatterBetweenClass);
            dgvScatterWithin.DataSource = new ArrayDataView(lda.ScatterWithinClass);
            dgvScatterTotal.DataSource = new ArrayDataView(lda.ScatterMatrix);
            dgvPrincipalComponents.DataSource = lda.Discriminants;
            distributionView.DataSource = lda.Discriminants;
            cumulativeView.DataSource = lda.Discriminants;

            // Populate classes information
            dgvClasses.DataSource = lda.Classes;

            lbStatus.Text = "Good! Feel free to browse the other tabs to see what has been found.";
        }

        /// <summary>
        ///   Launched when the user clicks the "Compute projection" button.
        /// </summary>
        /// 
        private void btnProject_Click(object sender, EventArgs e)
        {
            if (lda == null)
            {
                MessageBox.Show("Please run the analysis first!");
                return;
            }

            // Save any pending changes 
            dgvProjectionSource.EndEdit();

            // Creates a matrix from the source data table
            double[,] sourceMatrix = (dgvProjectionSource.DataSource as DataTable).ToMatrix(out columnNames);

            // Gets only the X and Y
            double[,] data = sourceMatrix.Submatrix(null, 0, 1);

            // Perform the transformation of the data using two components
            double[,] result = lda.Transform(data, 2);

            // Create a new plot with the original Z column
            double[,] graph = new double[sourceMatrix.GetLength(0), 3];
            for (int i = 0; i < graph.GetLength(0); i++)
            {
                graph[i, 0] = result[i, 0];
                graph[i, 1] = result[i, 1];
                graph[i, 2] = sourceMatrix[i, 2];
            }

            // Create output scatter plot
            outputScatterplot.DataSource = graph;

            // Create output table
            dgvProjectionResult.DataSource = new ArrayDataView(graph, columnNames);
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

        private void dgvClasses_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dgvClasses.CurrentRow != null)
            {
                DiscriminantAnalysisClass dclass = (DiscriminantAnalysisClass)dgvClasses.CurrentRow.DataBoundItem;

                dgvScatter.DataSource = new ArrayDataView(dclass.Scatter);
                dgvClassData.DataSource = new ArrayDataView(dclass.Subset);
            }
        }

        private void dgvClassData_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dgvClassData.CurrentCell != null && dgvClassData.DataSource != null)
            {
                int index = dgvClassData.CurrentCell.ColumnIndex;
                double[,] subset = (double[,])(dgvClassData.DataSource as ArrayDataView).ArrayData;
                dataHistogramView2.DataSource = subset.GetColumn(index);
            }
        }



        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog(this);
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            Close();
        }


    }
}
