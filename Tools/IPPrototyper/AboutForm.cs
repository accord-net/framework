// Image Processing Prototyper
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2010
// andrew.kirillov@aforgenet.com
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IPPrototyper
{
    internal partial class AboutForm : Form
    {
        public AboutForm( )
        {
            InitializeComponent( );

            // initialize links
            emailLabel.Links.Add( 0, emailLabel.Text.Length, "mailto:" + emailLabel.Text );
            aforgeFrameworkLabel.Links.Add( 0, aforgeFrameworkLabel.Text.Length, aforgeFrameworkLabel.Text );
        }

        // Link clicked
        private void LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            System.Diagnostics.Process.Start( e.Link.LinkData.ToString( ) );
        }
    }
}
