using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SampleApp
{
    public partial class URLForm : Form
    {
        private string url;

        // Selected URL
        public string URL
        {
            get { return url; }
        }

        // URLs to display in combo box
        public string[] URLs
        {
            set
            {
                urlBox.Items.AddRange( value );
            }
        }

        // Description of the dialog
        public string Description
        {
            get { return descriptionLabel.Text; }
            set { descriptionLabel.Text = value; }
        }

        // Constructor
        public URLForm( )
        {
            InitializeComponent( );
        }

        // On "Ok" button clicked
        private void okButton_Click( object sender, EventArgs e )
        {
            url = urlBox.Text;

        }
    }
}