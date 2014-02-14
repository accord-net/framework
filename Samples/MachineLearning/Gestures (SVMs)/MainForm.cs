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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math;
using Accord.Statistics.Kernels;
using Gestures.Native;

namespace Gestures.SVMs
{
    /// <summary>
    ///   Mouse gesture recognition sample application with Kernel Support 
    ///   Vector Machines using the Dynamic Time Warp kernel function.
    ///   
    ///   This application demonstrates how to perform sequence recognition
    ///   (gesture classification) using Support Vector Machines (SVMs).
    /// </summary>
    /// 
    public partial class MainForm : Form
    {

        private Database database;
        private MulticlassSupportVectorMachine svm;


        public MainForm()
        {
            InitializeComponent();

            database = new Database();
            gridSamples.AutoGenerateColumns = false;
            cbClasses.DataSource = database.Classes;
            gridSamples.DataSource = database.Samples;
            openDataDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Resources");
        }


        /// <summary>
        ///   Loads the stored gestures and learns a SVM using those data.
        /// </summary>
        /// 
        private void btnLearn_Click(object sender, EventArgs e)
        {
            if (gridSamples.Rows.Count == 0)
            {
                MessageBox.Show("Please load or insert some data first.");
                return;
            }

            BindingList<Sequence> samples = database.Samples;
            BindingList<String> classes = database.Classes;

            double[][] inputs = new double[samples.Count][];
            int[] outputs = new int[samples.Count];

            for (int i = 0; i < inputs.Length; i++)
            {
                inputs[i] = samples[i].Input;
                outputs[i] = samples[i].Output;
            }


            // Creates a new learning machine. Please note how the number of inputs is given
            // as zero: this means the machine will accept variable-length sequences as input.
            //
            svm = new MulticlassSupportVectorMachine(inputs: 0, 
                kernel: new DynamicTimeWarping(2), classes: classes.Count);


            // Create the learning algorithm to teach the multiple class classifier
            var teacher = new MulticlassSupportVectorLearning(svm, inputs, outputs)
            {
                // Setup the learning algorithm for each 1x1 subproblem
                Algorithm = (machine, classInputs, classOutputs, i, j) =>
                    new SequentialMinimalOptimization(machine, classInputs, classOutputs)
            };


            // Run the learning algorithm
            double error = teacher.Run();


            // Classify all training instances
            foreach (var sample in database.Samples)
            {
                sample.RecognizedAs = svm.Compute(sample.Input);
            }

            foreach (DataGridViewRow row in gridSamples.Rows)
            {
                var sample = row.DataBoundItem as Sequence;
                row.DefaultCellStyle.BackColor = (sample.RecognizedAs == sample.Output) ?
                    Color.LightGreen : Color.White;
            }
        }




        // Load and save database methods
        private void openDataStripMenuItem_Click(object sender, EventArgs e)
        {
            openDataDialog.ShowDialog();
        }

        private void saveDataStripMenuItem_Click(object sender, EventArgs e)
        {
            saveDataDialog.ShowDialog();
        }

        private void openDataDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            svm = null;

            using (var stream = openDataDialog.OpenFile())
                database.Load(stream);

            btnLearnSVM.Enabled = true;

            panelClassification.Visible = false;
            panelUserLabeling.Visible = false;
        }

        private void saveDataDialog_FileOk(object sender, CancelEventArgs e)
        {
            using (var stream = saveDataDialog.OpenFile())
                database.Save(stream);
        }

        private void btnFile_MouseDown(object sender, MouseEventArgs e)
        {
            menuFile.Show(button4, button4.PointToClient(Cursor.Position));
        }



        // Top user interaction panel box events
        private void btnYes_Click(object sender, EventArgs e)
        {
            addGesture();
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            panelClassification.Visible = false;
            panelUserLabeling.Visible = true;
        }


        // Bottom user interaction panel box events
        private void btnClear_Click(object sender, EventArgs e)
        {
            canvas.Clear();
            panelUserLabeling.Visible = false;
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            addGesture();
        }

        private void addGesture()
        {
            string selectedItem = cbClasses.SelectedItem as String;
            string classLabel = String.IsNullOrEmpty(selectedItem) ?
                cbClasses.Text : selectedItem;

            if (database.Add(canvas.GetSequence(), classLabel) != null)
            {
                canvas.Clear();

                if (database.Classes.Count >= 2 &&
                    database.SamplesPerClass() >= 3)
                    btnLearnSVM.Enabled = true;

                panelUserLabeling.Visible = false;
            }
        }


        // Canvas events
        private void inputCanvas_MouseUp(object sender, MouseEventArgs e)
        {
            double[] input = Sequence.Preprocess(canvas.GetSequence()).Merge(2);

            if (input.Length < 5)
            {
                panelUserLabeling.Visible = false;
                panelClassification.Visible = false;
                return;
            }

            if (svm == null)
            {
                panelUserLabeling.Visible = true;
                panelClassification.Visible = false;
            }

            else
            {
                int index = svm.Compute(input);

                string label = database.Classes[index];
                lbHaveYouDrawn.Text = String.Format("Have you drawn a {0}?", label);
                panelClassification.Visible = true;
                panelUserLabeling.Visible = false;
                cbClasses.SelectedItem = label;
            }
        }

        private void canvas_MouseDown(object sender, MouseEventArgs e)
        {
            lbIdle.Visible = false;
        }




        // Aero Glass settings
        //
        private void MainForm_Load(object sender, EventArgs e)
        {
            // Perform special processing to enable aero
            if (SafeNativeMethods.IsAeroEnabled)
            {
                ThemeMargins margins = new ThemeMargins();
                margins.TopHeight = canvas.Top;
                margins.LeftWidth = canvas.Left;
                margins.RightWidth = ClientRectangle.Right - gridSamples.Right;
                margins.BottomHeight = ClientRectangle.Bottom - canvas.Bottom;

                // Extend the Frame into client area
                SafeNativeMethods.ExtendAeroGlassIntoClientArea(this, margins);
            }
        }

        /// <summary>
        ///   Paints the background of the control.
        /// </summary>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            if (SafeNativeMethods.IsAeroEnabled)
            {
                // paint background black to enable include glass regions
                e.Graphics.Clear(Color.FromArgb(0, this.BackColor));
            }
        }
    }
}
