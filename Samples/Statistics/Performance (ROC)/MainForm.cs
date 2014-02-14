using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
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
using Accord.Controls;
using Accord.Statistics.Analysis;
using Accord.Statistics.Formats;

namespace Performance.ROC
{

    public partial class MainForm : Form
    {

        private DataTable sourceTable;
        private ReceiverOperatingCharacteristic rocCurve;
        

        public MainForm()
        {
            InitializeComponent();
            
            dgvSource.AutoGenerateColumns = true;
            dgvPointDetails.AutoGenerateColumns = false;

            openFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Resources");
        }



        private void btnRunAnalysis_Click(object sender, EventArgs e)
        {
            if (sourceTable == null)
            {
                MessageBox.Show("Please load some data before attempting to plot a curve.");
                return;
            }


            // Finishes and save any pending changes to the given data
            dgvSource.EndEdit();

            // Creates a matrix from the source data table
            int n = sourceTable.Rows.Count;

            double[] realData = new double[n];
            double[] testData = new double[n];
            for (int i = 0; i < n; i++)
            {
                realData[i] = (double)sourceTable.Rows[i][0];
                testData[i] = (double)sourceTable.Rows[i][1];
            }

            // Creates the Receiver Operating Curve of the given source
            rocCurve = new ReceiverOperatingCharacteristic(realData, testData);

            // Compute the ROC curve
            if (rbNumPoints.Checked)
                rocCurve.Compute((int)numPoints.Value);
            else
                rocCurve.Compute((float)numIncrement.Value);

            scatterplotView1.Scatterplot = rocCurve.GetScatterplot(true);

            // Show point details
            dgvPointDetails.DataSource = new SortableBindingList<ReceiverOperatingCharacteristicPoint>(rocCurve.Points);

            // Show area and error
            tbArea.Text = rocCurve.Area.ToString();
            tbError.Text = rocCurve.StandardError.ToString();
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
                        this.dgvSource.DataSource = sourceTable;
                    }
                }
                else if (extension == ".xml")
                {
                    DataTable dataTableAnalysisSource = new DataTable();
                    dataTableAnalysisSource.ReadXml(openFileDialog.FileName);

                    this.sourceTable = dataTableAnalysisSource;
                    this.dgvSource.DataSource = sourceTable;
                }
            }
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