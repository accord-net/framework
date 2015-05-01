// Fyzzy Set sample application
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2011
// contacts@aforgenet.com
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using AForge.Fuzzy;
using AForge.Controls;
using AForge;

namespace FuzzySetSample
{
    public partial class Sample : Form
    {
        public Sample( )
        {
            InitializeComponent( );

            chart.RangeX = new Range( 0, 50 );
            chart.AddDataSeries( "COLD", Color.CornflowerBlue, Chart.SeriesType.Line, 3, true );
            chart.AddDataSeries( "COOL", Color.LightBlue, Chart.SeriesType.Line, 3, true );
            chart.AddDataSeries( "WARM", Color.LightCoral, Chart.SeriesType.Line, 3, true );
            chart.AddDataSeries( "HOT", Color.Firebrick, Chart.SeriesType.Line, 3, true );
        }

        // Testing basic funcionality of fuzzy sets
        private void runFuzzySetTestButton_Click( object sender, EventArgs e )
        {
            ClearDataSeries( );

            // create 2 fuzzy sets to represent the Cool and Warm temperatures
            TrapezoidalFunction function1 = new TrapezoidalFunction( 13, 18, 23, 28 );
            FuzzySet fsCool = new FuzzySet( "Cool", function1 );
            TrapezoidalFunction function2 = new TrapezoidalFunction( 23, 28, 33, 38 );
            FuzzySet fsWarm = new FuzzySet( "Warm", function2 );

            // get membership of some points to the cool fuzzy set
            double[,] coolValues = new double[20, 2];
            for ( int i = 10; i < 30; i++ )
            {
                coolValues[i - 10, 0] = i;
                coolValues[i - 10, 1] = fsCool.GetMembership( i );
            }

            // getting memberships of some points to the warm fuzzy set
            double[,] warmValues = new double[20, 2];
            for ( int i = 20; i < 40; i++ )
            {
                warmValues[i - 20, 0] = i;
                warmValues[i - 20, 1] = fsWarm.GetMembership( i );
            }

            // plot membership to a chart
            chart.UpdateDataSeries( "COOL", coolValues );
            chart.UpdateDataSeries( "WARM", warmValues );
        }


        // Testing basic funcionality of linguistic variables
        private void runLingVarTestButton_Click( object sender, EventArgs e )
        {
            ClearDataSeries( );

            // create a linguistic variable to represent temperature
            LinguisticVariable lvTemperature = new LinguisticVariable( "Temperature", 0, 80 );

            // create the linguistic labels (fuzzy sets) that compose the temperature 
            TrapezoidalFunction function1 = new TrapezoidalFunction( 10, 15, TrapezoidalFunction.EdgeType.Right );
            FuzzySet fsCold = new FuzzySet( "Cold", function1 );
            TrapezoidalFunction function2 = new TrapezoidalFunction( 10, 15, 20, 25 );
            FuzzySet fsCool = new FuzzySet( "Cool", function2 );
            TrapezoidalFunction function3 = new TrapezoidalFunction( 20, 25, 30, 35 );
            FuzzySet fsWarm = new FuzzySet( "Warm", function3 );
            TrapezoidalFunction function4 = new TrapezoidalFunction( 30, 35, TrapezoidalFunction.EdgeType.Left );
            FuzzySet fsHot = new FuzzySet( "Hot", function4 );

            // adding labels to the variable
            lvTemperature.AddLabel( fsCold );
            lvTemperature.AddLabel( fsCool );
            lvTemperature.AddLabel( fsWarm );
            lvTemperature.AddLabel( fsHot );

            // get membership of some points to the cool fuzzy set
            double[][,] chartValues = new double[4][,];
            for ( int i = 0; i < 4; i++ )
                chartValues[i] = new double[160, 2];

            // showing the shape of the linguistic variable - the shape of its labels memberships from start to end
            int j = 0;
            for ( float x = 0; x < 80; x += 0.5f, j++ )
            {
                double y1 = lvTemperature.GetLabelMembership( "Cold", x );
                double y2 = lvTemperature.GetLabelMembership( "Cool", x );
                double y3 = lvTemperature.GetLabelMembership( "Warm", x );
                double y4 = lvTemperature.GetLabelMembership( "Hot", x );

                chartValues[0][j, 0] = x;
                chartValues[0][j, 1] = y1;
                chartValues[1][j, 0] = x;
                chartValues[1][j, 1] = y2;
                chartValues[2][j, 0] = x;
                chartValues[2][j, 1] = y3;
                chartValues[3][j, 0] = x;
                chartValues[3][j, 1] = y4;
            }

            // plot membership to a chart
            chart.UpdateDataSeries( "COLD", chartValues[0] );
            chart.UpdateDataSeries( "COOL", chartValues[1] );
            chart.UpdateDataSeries( "WARM", chartValues[2] );
            chart.UpdateDataSeries( "HOT", chartValues[3] );
        }

        // Clear all data series data
        private void ClearDataSeries( )
        {
            chart.UpdateDataSeries( "COLD", null );
            chart.UpdateDataSeries( "COOL", null );
            chart.UpdateDataSeries( "WARM", null );
            chart.UpdateDataSeries( "HOT", null );
        }
    }
}