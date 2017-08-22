// Accord Statistics Controls Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
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

namespace Accord.Controls
{
    using System;
    using System.Data;
    using System.Threading;
    using System.Windows.Forms;
    using System.Drawing;

    /// <summary>
    ///   Data Grid Box for quickly displaying a form with a DataGridView
    ///   on it in the same spirit as System.Windows.Forms.MessageBox.
    /// </summary>
    /// 
    /// <example>
    /// <code>
    /// // Create some data
    /// double[,] data = Matrix.Identity(5);
    ///   
    /// // Display it onscreen
    /// DataGridBox.Show(data).Hold();
    /// </code>
    ///   
    /// <img src="../images/visualizations/datagrid-box.png"/>
    /// </example>
    /// 
    public partial class DataGridBox : Form
    {

        private Thread formThread;

        private DataGridBox()
        {
            InitializeComponent();

            this.dataGridView.AllowNestedProperties(true);
        }

        /// <summary>
        ///   Gets the <see cref="DataGridView"/> control contained
        ///   in this box. As it runs on a different thread, any 
        ///   operations needs to be invoked on the control's thread.
        /// </summary>
        /// 
        public DataGridView DataGridView
        {
            get { return this.dataGridView; }
        }

        /// <summary>
        ///   Sets the cell font size.
        /// </summary>
        /// 
        public DataGridBox SetDefaultFontSize(float emSize)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => SetDefaultFontSize(emSize)));
                return this;
            }

            dataGridView.DefaultCellStyle.Font
                = new Font(dataGridView.DefaultCellStyle.Font.FontFamily, emSize);

            return this;
        }

        /// <summary>
        ///   Sets the visibility of the column headers.
        /// </summary>
        /// 
        public DataGridBox SetColumnHeadersVisible(bool visible)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => SetColumnHeadersVisible(visible)));
                return this;
            }

            dataGridView.ColumnHeadersVisible = visible;

            return this;
        }

        /// <summary>
        ///   Sets the visibility of the row headers.
        /// </summary>
        /// 
        public DataGridBox SetRowHeadersVisible(bool visible)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => SetRowHeadersVisible(visible)));
                return this;
            }

            dataGridView.RowHeadersVisible = visible;

            return this;
        }

        /// <summary>
        ///   Sets the auto-size mode for the columns.
        /// </summary>
        /// 
        public DataGridBox SetAutoSizeColumns(DataGridViewAutoSizeColumnsMode mode)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => SetAutoSizeColumns(mode)));
                return this;
            }

            dataGridView.AutoSizeColumnsMode = mode;

            return this;
        }

        /// <summary>
        ///   Sets the auto-size mode for the rows.
        /// </summary>
        /// 
        public DataGridBox SetAutoSizeRows(DataGridViewAutoSizeRowsMode mode)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => SetAutoSizeRows(mode)));
                return this;
            }

            dataGridView.AutoSizeRowsMode = mode;

            return this;
        }

        /// <summary>
        ///   Blocks the caller until the form is closed.
        /// </summary>
        /// 
        public void WaitForClose()
        {
            if (Thread.CurrentThread != formThread)
                formThread.Join();
        }

        /// <summary>
        ///   Closes the form.
        /// </summary>
        /// 
        public new void Close()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => base.Close()));
                return;
            }

            base.Close();
        }

        /// <summary>
        ///   Sets the window title of the data grid box.
        ///   
        /// </summary>
        /// <param name="text">The desired title text for the window.</param>
        /// 
        /// <returns>This instance, for fluent programming.</returns>
        /// 
        public DataGridBox SetTitle(string text)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => SetTitle(text)));
                return this;
            }

            this.Text = text;

            return this;
        }

        /// <summary>
        ///   Displays a Data Grid View with the specified data.
        /// </summary>
        /// 
        /// <param name="source">The source object to display.</param>
        /// <param name="title">The title for the data window.</param>
        /// 
        /// <returns>The Data Grid Box being shown.</returns>
        /// 
        public static DataGridBox Show(object source, string title = null)
        {
            return show(source, title);
        }

        /// <summary>
        ///   Displays a Data Grid View with the specified data.
        /// </summary>
        /// 
        /// <param name="table">The source table to display.</param>
        /// 
        /// <returns>The Data Grid Box being shown.</returns>
        /// 
        public static DataGridBox Show(DataTable table)
        {
            return show(table, table.TableName);
        }

        /// <summary>
        ///   Displays a Data Grid View with the specified data.
        /// </summary>
        /// 
        /// <param name="array">The array to be displayed.</param>
        /// <param name="colNames">A collection of column names to be displayed.</param>
        /// <param name="rowNames">A collection of row names to be displayed.</param>
        /// <param name="title">The title for the data window.</param>
        /// 
        /// <returns>The Data Grid Box being shown.</returns>
        /// 
        public static DataGridBox Show(Array array, String title = null,
            object[] rowNames = null, object[] colNames = null)
        {
            return show(new ArrayDataView(array, columnNames: colNames, rowNames: rowNames), title);
        }

        private static DataGridBox show(object source, string title)
        {
            DataGridBox form = null;
            Thread formThread = null;

            AutoResetEvent stopWaitHandle = new AutoResetEvent(false);

            formThread = new Thread(() =>
            {
                Accord.Controls.Tools.ConfigureWindowsFormsApplication();

                // Show control in a form
                form = new DataGridBox();
                form.Text = title ?? "Data Grid";
                form.formThread = formThread;
                form.dataGridView.DataSource = source;

                stopWaitHandle.Set();

                Application.Run(form);
            });

            formThread.SetApartmentState(ApartmentState.STA);

            formThread.Start();

            stopWaitHandle.WaitOne();

            return form;
        }

        /// <summary>
        ///   Holds the execution until the window has been closed.
        /// </summary>
        /// 
        public void Hold()
        {
            if (Thread.CurrentThread == formThread)
                return;

            this.SetTitle(this.Text + " [on hold]");

            formThread.Join();
        }

        private void dataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            this.contextMenuStrip1.Items.Clear();

            foreach (DataGridViewColumn column in this.dataGridView.Columns)
            {
                var item = new ToolStripMenuItem()
                {
                    Text = column.HeaderText,
                    Checked = true,
                    CheckOnClick = true,
                    Tag = column,
                };

                item.CheckedChanged += item_CheckedChanged;

                this.contextMenuStrip1.Items.Add(item);
                column.HeaderCell.ContextMenuStrip = this.contextMenuStrip1;
            }

        }

        private void item_CheckedChanged(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            DataGridViewColumn column = item.Tag as DataGridViewColumn;

            if (column == null)
                return;

            column.Visible = item.Checked;
        }

    }
}
