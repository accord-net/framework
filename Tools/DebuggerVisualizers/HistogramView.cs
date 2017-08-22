// Accord Debugging Visualizers
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Darren Schroeder, 2017
// https://github.com/fdncred
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
// AForge debugging visualizers
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2011
// contacts@aforgenet.com
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Accord.Math;
using Accord.Controls;
using Accord.Statistics;

namespace Accord.DebuggerVisualizers
{
    public partial class HistogramView : Form
    {
        private const int HistogramHeight = 100;
        private Accord.Statistics.Visualizations.Histogram histogram;

        public HistogramView( )
        {
            InitializeComponent( );
        }

        public void SetHistogram(Accord.Statistics.Visualizations.Histogram histogram )
        {
            this.histogram = histogram;

            histogramControl.Values = histogram.Values;
            int length = histogram.Values.Length;

            Text = string.Format( "Histogram - {0} values", length );
            statsBox.Text = string.Format( "Min: {0}   Max: {1}   Mean: {2:F2}   Std.Dev.: {3:F2}",
                histogram.Min, histogram.Max, histogram.Mean, histogram.StdDev );

            // set form size
            int formWidth  = System.Math.Min( 800, length ) + 40;
            int formHeight = HistogramHeight + 120;

            this.Size = new Size( formWidth, formHeight );

            // set histogram control size
            int width = length + 2;
            int height = HistogramHeight + 2;

            int x = ( width > mainPanel.ClientSize.Width ) ? 0 : ( mainPanel.ClientSize.Width - width ) / 2;
            int y = ( height > mainPanel.ClientSize.Height ) ? 0 : ( mainPanel.ClientSize.Height - height ) / 2;

            histogramControl.SuspendLayout( );
            histogramControl.Size = new Size( width, height );
            histogramControl.Location = new System.Drawing.Point( x, y );
            histogramControl.ResumeLayout( );
        }

        // Mouse cursor's position has changed within histogram control
        private void histogramControl_PositionChanged( object sender, Accord.Controls.HistogramEventArgs e )
        {
            if ( histogram != null )
            {
                int pos = e.Position;

                if ( pos != -1 )
                {
                    textBox.Text = string.Format( "Value: {0}   Count: {1}   Percent: {2:F2}",
                        pos, histogram.Values[pos], ( (float) histogram.Values[pos] * 100 / histogram.TotalCount ) );
                }
                else
                {
                    textBox.Text = string.Empty;
                }
            }
        }

        // Selection has changed within histogram control
        private void histogramControl_SelectionChanged( object sender, Accord.Controls.HistogramEventArgs e )
        {
            if ( histogram != null )
            {
                int min = e.Min;
                int max = e.Max;
                int count = 0;

                // count pixels
                for ( int i = min; i <= max; i++ )
                {
                    count += histogram.Values[i];
                }

                textBox.Text = string.Format( "Values: {0}...{1}   Count: {2}   Percent: {3:F2}",
                    min, max, count, ( (float) count * 100 / histogram.TotalCount ) );
            }
        }

        private void logCheck_CheckedChanged( object sender, EventArgs e )
        {
            histogramControl.IsLogarithmicView = logCheck.Checked;
        }
    }
}
