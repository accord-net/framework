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
using Accord.Statistics.Filters;
using Accord.Statistics.Formats;
using Components;
using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace Tabular
{
    public partial class MainForm : Form
    {
        DataTable sourceTable;
        BindingList<FilterDescriptor> filters;


        public MainForm()
        {
            InitializeComponent();

            RangeTypeConverter.Assign();

            filters = new BindingList<FilterDescriptor>();
            filters.Add(new FilterDescriptor(null, "Original"));
            dataGridView2.AutoGenerateColumns = false;
            dataGridView2.DataSource = filters;

            openFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Resources");

        }

        #region Open / Save documents
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
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
                        this.dataGridView1.DataSource = sourceTable;
                    }
                }
            }
        }
        #endregion


        private void normToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filters.Add(new FilterDescriptor(new Normalization(), "Normalization"));
        }

        private void equalizationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filters.Add(new FilterDescriptor(new Stratification(), "Equalization"));
        }

        private void scalingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filters.Add(new FilterDescriptor(new LinearScaling(), "Scaling"));
        }

        private void projectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filters.Add(new FilterDescriptor(new Projection(), "Projection"));
        }

        private void groupingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filters.Add(new FilterDescriptor(new Grouping(), "Grouping"));
        }

        private void dataGridView2_CurrentCellChanged(object sender, EventArgs e)
        {
            apply();
        }

        private void apply()
        {
            if (dataGridView2.CurrentRow != null)
            {
                IFilter filter = (dataGridView2.CurrentRow.DataBoundItem as FilterDescriptor).Filter;
                propertyGrid1.SelectedObject = filter;

                if (filter == null)
                {
                    dataGridView1.DataSource = sourceTable;
                }
                else
                {
                    dataGridView1.DataSource = filter.Apply(dataGridView1.DataSource as DataTable);
                }
            }
        }



        private void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dataGridView1.DataSource != null && dataGridView1.CurrentCell != null)
            {
                string member = dataGridView1.CurrentCell.OwningColumn.DataPropertyName;

                histogramView1.DataSource = dataGridView1.DataSource;
                histogramView1.DataMember = member;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView2.CurrentRow != null)
            {
                IFilter filter = (dataGridView2.CurrentRow.DataBoundItem as FilterDescriptor).Filter;
                IAutoConfigurableFilter auto = filter as IAutoConfigurableFilter;

                if (auto != null)
                {
                    auto.Detect(sourceTable);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            apply();
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
