// Image Processing Prototyper
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2010-2011
// contacts@aforgenet.com
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using AForge.Math;
using AForge.Imaging;

namespace IPPrototyper
{
    // Form to display image's histogram(s) and basic statistics like mean, std.dev, etc
    internal partial class HistogramForm : Form
    {
        private ImageStatistics stats;
        private Histogram activeHistogram;

        public HistogramForm( )
        {
            InitializeComponent( );
        }

        // Set image statistics to display
        public void SetImageStatistics( ImageStatistics stats )
        {
            this.stats = stats;

            if ( stats.IsGrayscale )
            {
                activeHistogram = stats.Gray;
                histogram.Color = Color.Black;
                channelCombo.Enabled = false;

                ShowInfoForActiveHistogram( );
            }
            else
            {
                channelCombo.Enabled = true;
                channelCombo.SelectedIndex = 0;
                SelectChannel( 0 );
            }
        }

        // Show information for the currently active histogram
        private void ShowInfoForActiveHistogram( )
        {
            histogram.Values = activeHistogram.Values;
            meanLabel.Text   = activeHistogram.Mean.ToString( "F2" );
            stdDevLabel.Text = activeHistogram.StdDev.ToString( "F2" );
            medianLabel.Text = activeHistogram.Median.ToString( );
            minLabel.Text    = activeHistogram.Min.ToString( );
            maxLabel.Text    = activeHistogram.Max.ToString( );

            histogram.Values = activeHistogram.Values;
        }

        // Show histogram for the specified RGB channel
        private void SelectChannel( int c )
        {
            switch ( c )
            {
                case 0:
                    activeHistogram = stats.Red;
                    histogram.Color = Color.Red;
                    break;
                case 1:
                    activeHistogram = stats.Green;
                    histogram.Color = Color.Green;
                    break;
                case 2:
                    activeHistogram = stats.Blue;
                    histogram.Color = Color.Blue;
                    break;
            }

            ShowInfoForActiveHistogram( );
        }

        // Selection has changed in RGB channel combo
        private void channelCombo_SelectedIndexChanged( object sender, EventArgs e )
        {
            SelectChannel( channelCombo.SelectedIndex );
        }

        // Mouse cursor's position has changed within histogram control
        private void histogram_PositionChanged( object sender, AForge.Controls.HistogramEventArgs e )
        {
            int pos = e.Position;

            if ( pos != -1 )
            {
                levelLabel.Text = pos.ToString( );
                countLabel.Text = activeHistogram.Values[pos].ToString( );
                percentileLabel.Text = ( (float) activeHistogram.Values[pos] * 100 / stats.PixelsCount ).ToString( "F2" );
            }
            else
            {
                levelLabel.Text = "";
                countLabel.Text = "";
                percentileLabel.Text = "";
            }
        }

        // Selection has changed within histogram control
        private void histogram_SelectionChanged( object sender, AForge.Controls.HistogramEventArgs e )
        {
            int min = e.Min;
            int max = e.Max;
            int count = 0;

            levelLabel.Text = min.ToString( ) + "..." + max.ToString( );

            // count pixels
            for ( int i = min; i <= max; i++ )
            {
                count += activeHistogram.Values[i];
            }
            countLabel.Text = count.ToString( );
            percentileLabel.Text = ( (float) count * 100 / stats.PixelsCount ).ToString( "F2" );
        }
    }
}
