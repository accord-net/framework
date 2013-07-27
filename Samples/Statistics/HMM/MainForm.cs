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
using System.Data;
using System.IO;
using System.Windows.Forms;
using Accord.Controls;
using Accord.Statistics.Formats;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Models.Markov.Learning;
using Components;

namespace Markov
{
    public partial class MainForm : Form
    {

        HiddenMarkovClassifier hmmc;



        /// <summary>
        ///   Creates the ensemble
        /// </summary>
        private void btnCreate_Click(object sender, EventArgs e)
        {
            DataTable source = dgvSequenceSource.DataSource as DataTable;

            if (source == null)
            {
                MessageBox.Show("Please load some data by clicking 'Open' under the 'File' menu first. " +
                    "A sample dataset can be found in the folder 'Resources' contained in the same " +
                    "directory as this application.");
                return;
            }

            DataTable k = source.DefaultView.ToTable(true, "Label", "States");

            // Get the number of different classes in the data
            int classes = k.Rows.Count;

            string[] categories = new string[classes];
            int[] states = new int[classes];
            for (int i = 0; i < classes; i++)
            {
                // Gets the label name
                categories[i] = k.Rows[i]["Label"] as string;

                // Gets the number of states to recognize each label
                states[i] = int.Parse(k.Rows[i]["States"] as string);
            }


            hmmc = new HiddenMarkovClassifier(classes, states, 3, categories);

            dgvModels.DataSource = hmmc.Models;
        }


        /// <summary>
        ///   Trains the ensemble
        /// </summary>
        private void btnTrain_Click(object sender, EventArgs e)
        {
            DataTable source = dgvSequenceSource.DataSource as DataTable;
            if (source == null || hmmc == null)
            {
                MessageBox.Show("Please create a sequence classifier first.");
                return;
            }

            int rows = source.Rows.Count;

            // Gets the input sequences
            int[][] sequences = new int[rows][];
            int[] labels = new int[rows];

            for (int i = 0; i < rows; i++)
            {
                DataRow row = source.Rows[i];

                string label = row["Label"] as string;

                for (int j = 0; j < hmmc.Models.Length; j++)
                {
                    if (hmmc.Models[j].Tag.Equals(label))
                    {
                        labels[i] = j;
                        break;
                    }
                }

                sequences[i] = decode(row["Sequences"] as string);
            }


            // Grab training parameters
            int iterations = (int)numIterations.Value;
            double limit = (double)numConvergence.Value;

            if (rbStopIterations.Checked)
            {
                limit = 0;
            }
            else
            {
                iterations = 0;
            }

            // Train the ensemble

            var teacher = new HiddenMarkovClassifierLearning(hmmc, i =>
                new BaumWelchLearning(hmmc.Models[i])
                {
                        Iterations = iterations,
                        Tolerance = limit
                }
            );

            teacher.Run(sequences, labels);


            // Update the GUI
            dgvModels_CurrentCellChanged(this, EventArgs.Empty);
        }

        /// <summary>
        ///   Tests the ensemble
        /// </summary>
        private void btnTest_Click(object sender, EventArgs e)
        {
            int rows = dgvTesting.Rows.Count - 1;


            // Gets the input sequences
            int[][] sequences = new int[rows][];
            for (int i = 0; i < rows; i++)
            {
                DataGridViewRow row = dgvTesting.Rows[i];
                string sequence = row.Cells["colTestSequence"].Value as string;
                String trueLabel = row.Cells["colTestTrueClass"].Value as string;

                double likelihood;
                int label = hmmc.Compute(decode(sequence), out likelihood);
                string assignedLabel = hmmc.Models[label].Tag as string;

                row.Cells["colTestAssignedClass"].Value = assignedLabel;
                row.Cells["colTestLikelihood"].Value = likelihood;
                row.Cells["colTestMatch"].Value = trueLabel == assignedLabel;
            }
        }

        /// <summary>
        ///   Decodes a sequence in string form into is integer array form.
        ///   Example: Converts "1-2-1-3-5" into int[] {1,2,1,3,5}
        /// </summary>
        /// <returns></returns>
        private int[] decode(String sequence)
        {
            string[] elements = sequence.Split('-');
            int[] integers = new int[elements.Length];

            for (int j = 0; j < elements.Length; j++)
                integers[j] = int.Parse(elements[j]);

            return integers;
        }






        #region Form methods and events (mostly non-HMM related)
        public MainForm()
        {
            InitializeComponent();

            dgvModels.AutoGenerateColumns = false;
            dgvTesting.AutoGenerateColumns = false;

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
                        DataTable tableSource = db.GetWorksheet(t.Selection);
                        this.dgvSequenceSource.DataSource = tableSource;
                        loadTesting(tableSource);
                    }
                }
            }
        }

        private void dgvModels_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dgvModels.CurrentRow != null)
            {
                HiddenMarkovModel model = dgvModels.CurrentRow.DataBoundItem as HiddenMarkovModel;
                dgvProbabilities.DataSource = new ArrayDataView(model.Probabilities);
                dgvEmissions.DataSource = new ArrayDataView(model.Emissions);
                dgvTransitions.DataSource = new ArrayDataView(model.Transitions);
            }
        }

        private void loadTesting(DataTable table)
        {
            int rows = table.Rows.Count;

            // Gets the input sequences
            int[][] sequences = new int[rows][];
            for (int i = 0; i < rows; i++)
            {
                dgvTesting.Rows.Add(
                    table.Rows[i]["Sequences"],
                    table.Rows[i]["Label"],
                    null,
                    0,
                    false);
            }
        }
        #endregion


    }
}
