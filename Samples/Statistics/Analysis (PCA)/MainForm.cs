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
using ZedGraph;
using Components;

namespace Analysis.PCA
{

    public partial class MainForm : System.Windows.Forms.Form
    {

        private PrincipalComponentAnalysis pca;
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
            dgvProjectionResult.AutoGenerateColumns = true;

            openFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Resources");
        }

        protected override void OnLoad(EventArgs e)
        {
            Array methods = Enum.GetValues(typeof(AnalysisMethod));
            this.cbMethod.DataSource = methods;
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

            // Create and compute a new Simple Descriptive Analysis
            sda = new DescriptiveAnalysis(sourceMatrix, columnNames);

            sda.Compute();

            // Show the descriptive analysis on the screen
            dgvDistributionMeasures.DataSource = sda.Measures;

            // Populates statistics overview tab with analysis data
            dgvStatisticCenter.DataSource = new ArrayDataView(sda.DeviationScores, columnNames);
            dgvStatisticStandard.DataSource = new ArrayDataView(sda.StandardScores, columnNames);

            dgvStatisticCovariance.DataSource = new ArrayDataView(sda.CovarianceMatrix, columnNames);
            dgvStatisticCorrelation.DataSource = new ArrayDataView(sda.CorrelationMatrix, columnNames);


            AnalysisMethod method = (AnalysisMethod)cbMethod.SelectedValue;

            // Create the Principal Component Analysis of the data 
            pca = new PrincipalComponentAnalysis(sda.Source, method);


            pca.Compute();  // Finally, compute the analysis!


            // Populate components overview with analysis data
            dgvFeatureVectors.DataSource = new ArrayDataView(pca.ComponentMatrix);
            dgvPrincipalComponents.DataSource = pca.Components;
            dgvProjectionComponents.DataSource = pca.Components;
            distributionView.DataSource = pca.Components;
            cumulativeView.DataSource = pca.Components;

            numComponents.Maximum = pca.Components.Count;
            numComponents.Value = 1;
            numThreshold.Value = (decimal)pca.Components[0].CumulativeProportion * 100;
        }

        /// <summary>
        ///   Launched when the user clicks the "Compute projection" button.
        /// </summary>
        /// 
        private void btnProject_Click(object sender, EventArgs e)
        {
            if (pca == null || dgvProjectionSource.DataSource == null)
            {
                MessageBox.Show("Please compute the analysis first.");
                return;
            }

            // Save any pending changes 
            dgvProjectionSource.EndEdit();

            string[] colNames;
            int components = (int)numComponents.Value;
            double[,] projectionSource = (dgvProjectionSource.DataSource as DataTable).ToMatrix(out colNames);

            // Compute the projection
            double[,] projection = pca.Transform(projectionSource, components);

            dgvProjectionResult.DataSource = new ArrayDataView(projection, GenerateComponentNames(components));
            dgvReversionSource.DataSource = dgvProjectionResult.DataSource;
        }

        /// <summary>
        ///   Launched when the user clicks on the "Revert projection" button.
        /// </summary>
        /// 
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

            dgvReversionResult.DataSource = new ArrayDataView(reversion, columnNames);
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