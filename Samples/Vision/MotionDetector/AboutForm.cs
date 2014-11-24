using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MotionDetectorSample
{
    public partial class AboutForm : Form
    {
        public AboutForm( )
        {
            InitializeComponent( );

            // initialize links
            emailLabel.Links.Add( 0, emailLabel.Text.Length, "mailto:andrew.kirillov@aforgenet.com" );
            aforgeLabel.Links.Add( 0, aforgeLabel.Text.Length, "http://www.aforgenet.com/framework/" );
        }

        // Link clicked
        private void LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            System.Diagnostics.Process.Start( e.Link.LinkData.ToString( ) );
        }
    }
}