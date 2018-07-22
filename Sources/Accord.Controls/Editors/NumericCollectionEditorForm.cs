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
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;

    /// <summary>
    ///   Numeric collection editor.
    /// </summary>
    /// 
    public partial class NumericCollectionEditorForm : Form
    {
        NumericCollectionEditor editor;
        object value;
        int lines;

        /// <summary>
        ///   Initializes a new instance of the <see cref="NumericCollectionEditorForm"/> class.
        /// </summary>
        /// 
        public NumericCollectionEditorForm()
        {
            InitializeComponent();
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="NumericCollectionEditorForm"/> class.
        /// </summary>
        /// 
        /// <param name="editor">The editor.</param>
        /// <param name="value">The value.</param>
        /// 
        public NumericCollectionEditorForm(NumericCollectionEditor editor, object value)
            : this()
        {
            this.editor = editor;
            this.value = value;
        }

        private void ObjectCollectionEditorForm_Load(object sender, EventArgs e)
        {
            foreach (object obj in editor.GetItems(value))
                textBox1.AppendText(obj.ToString() + '\n');
            lines = textBox1.Lines.Length;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            object[] values = new object[lines];
            for (int i = 0; i < textBox1.Lines.Length; i++)
            {
                string line = textBox1.Lines[i];
                values[i] = Convert.ChangeType(line, editor.CollectionItemType);
            }

            editor.SetItems(value, values);
        }
    }
}
