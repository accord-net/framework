// Animat sample application
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2006-2011
// contacts@aforgenet.com
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Animat
{
    public partial class CellWorld : Control
    {
        private Pen blackPen = new Pen( Color.Black );
        private Brush whiteBrush = new SolidBrush( Color.White );

        private int[,] map = null;
        private Color[] coloring = null;

        /// <summary>
        /// World's map
        /// </summary>
        /// 
        public int[,] Map
        {
            get { return map; }
            set
            {
                map = value;
                Invalidate( );
            }
        }

        /// <summary>
        /// World's coloring
        /// </summary>
        /// 
        public Color[] Coloring
        {
            get { return coloring; }
            set
            {
                coloring = value;
                Invalidate( );
            }
        }

        // Control's constructor
        public CellWorld( )
        {
            InitializeComponent( );

            // update control's style
            SetStyle( ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                ControlStyles.DoubleBuffer | ControlStyles.UserPaint, true );
        }

		// Paint the control
        protected override void OnPaint( PaintEventArgs pe )
        {
            Graphics g = pe.Graphics;
            int clientWidth = ClientRectangle.Width;
            int clientHeight = ClientRectangle.Height;

            // fill with white background
            g.FillRectangle( whiteBrush, 0, 0, clientWidth - 1, clientHeight - 1 );

            // draw a black rectangle
            g.DrawRectangle( blackPen, 0, 0, clientWidth - 1, clientHeight - 1 );

            if ( ( map != null ) && ( coloring != null ) )
            {
                int brushesCount = coloring.Length;
                int cellWidth = (int) ( clientWidth / map.GetLength( 1 ) );
                int cellHeight = (int) ( clientHeight / map.GetLength( 0 ) );

                // create brushes
                Brush[] brushes = new Brush[brushesCount];
                for ( int i = 0; i < brushesCount; i++ )
                {
                    brushes[i] = new SolidBrush( coloring[i] );
                }

                // draw the world
                for ( int i = 0, n = map.GetLength( 0 ); i < n; i++ )
                {
                    int ch = ( i < n - 1 ) ? cellHeight : clientHeight - i * cellHeight - 1;

                    for ( int j = 0, k = map.GetLength( 1 ); j < k; j++ )
                    {
                        int cw = ( j < k - 1 ) ? cellWidth : clientWidth - j * cellWidth - 1;

                        // check if we have appropriate brush
                        if ( map[i, j] < brushesCount )
                        {
                            g.FillRectangle( brushes[map[i, j]], j * cellWidth, i * cellHeight, cw, ch );
                            g.DrawRectangle( blackPen, j * cellWidth, i * cellHeight, cw, ch );
                        }
                    }
                }

                // release brushes
                for ( int i = 0; i < brushesCount; i++ )
                {
                    brushes[i].Dispose( );
                }
            }

            // Calling the base class OnPaint
            base.OnPaint( pe );
        }
    }
}
