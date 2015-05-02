// 3D Pose Estimation (2) sample application
// AForge.NET Framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2009-2011
// contacts@aforgenet.com
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using AForge.Math;

namespace PoseEstimation
{
    public partial class MatrixControl : UserControl
    {
        private Dictionary<string, TextBox> textBoxes = new Dictionary<string, TextBox>( );

        public string Title
        {
            get { return groupBox.Text; }
            set { groupBox.Text = value; }
        }

        public MatrixControl( )
        {
            InitializeComponent( );
        }

        public void SetMatrix( Matrix4x4 matrix )
        {
            float[] array = matrix.ToArray( );

            for ( int i = 0, k = 0; i < 4; i++ )
            {
                for ( int j = 0; j < 4; j++, k++ )
                {
                    string textBoxName = string.Format( "i{0}_j{1}_Box", i, j );

                    if ( textBoxes.ContainsKey( textBoxName ) )
                    {
                        textBoxes[textBoxName].Text = FormatFloat( array[k] );
                    }
                }
            }
        }

        public void Clear( )
        {
            for ( int i = 0, k = 0; i < 4; i++ )
            {
                for ( int j = 0; j < 4; j++, k++ )
                {
                    string textBoxName = string.Format( "i{0}_j{1}_Box", i, j );

                    if ( textBoxes.ContainsKey( textBoxName ) )
                    {
                        textBoxes[textBoxName].Text = string.Empty;
                    }
                }
            }
        }

        private static string FormatFloat( float floatValue )
        {
            return String.Format( "{0:0.####}", floatValue );
        }

        private void MatrixControl_Load( object sender, EventArgs e )
        {
            textBoxes.Clear( );

            foreach ( Control c in groupBox.Controls )
            {
                if ( c is TextBox )
                {
                    textBoxes.Add( c.Name, (TextBox) c );
                }
            }
        }
    }
}
