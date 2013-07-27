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
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows.Forms;
using Accord.Controls;
using Accord.Statistics.Filters;
using Accord.Statistics.Formats;
using Components;

namespace DataProcessing
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

    }
}
