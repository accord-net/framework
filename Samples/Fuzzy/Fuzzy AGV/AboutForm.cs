using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FuzzyAGV
{
    public partial class AboutForm : Form
    {
        public AboutForm( )
        {
            InitializeComponent( );

            // initialize links
            emailLabel.Links.Add( 0, emailLabel.Text.Length, "mailto:" + emailLabel.Text );
            aforgeFrameworkLabel.Links.Add( 0, aforgeFrameworkLabel.Text.Length, aforgeFrameworkLabel.Text );
        }

        private void LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            System.Diagnostics.Process.Start( e.Link.LinkData.ToString( ) );
        }
    }
}
