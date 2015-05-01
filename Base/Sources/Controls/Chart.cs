// AForge Controls Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2011
// contacts@aforgenet.com
//

namespace AForge.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Data;
    using System.Windows.Forms;
    using AForge;

    /// <summary>
    /// Chart control.
    /// </summary>
    /// 
    /// <remarks><para>The chart control allows to display multiple charts at time
    /// of different types: dots, lines, connected dots.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create data series array
    /// double[,] testValues = new double[10, 2];
    /// // fill data series
    /// for ( int i = 0; i &lt; 10; i++ )
    /// {
    ///     testValues[i, 0] = i; // X values
    ///     testValues[i, 1] = Math.Sin( i / 18.0 * Math.PI ); // Y values
    /// }
    /// // add new data series to the chart
    /// chart.AddDataSeries( "Test", Color.DarkGreen, Chart.SeriesType.ConnectedDots, 3 );
    /// // set X range to display
    /// chart.RangeX = new AForge.Range( 0, 9 );
    /// // update the chart
    /// chart.UpdateDataSeries( "Test", testValues );
    /// </code>
    /// </remarks>
    /// 
    public class Chart : System.Windows.Forms.Control
    {
        /// <summary>
        /// Chart series type enumeration.
        /// </summary>
        public enum SeriesType
        {
            /// <summary>
            /// Line style.
            /// </summary>
            Line,
            /// <summary>
            /// Dots style.
            /// </summary>
            Dots,
            /// <summary>
            /// Connected dots style.
            /// </summary>
            ConnectedDots
        }

        // series data
        private class DataSeries
        {
            public double[,]	data = null;
            public Color		color = Color.Blue;
            public SeriesType	type = SeriesType.Line;
            public int			width = 1;
            public bool			updateYRange = true;
        }

        // data series table
        private Dictionary<string, DataSeries> seriesTable = new Dictionary<string, DataSeries>( );

        private Pen		blackPen = new Pen( Color.Black );

        private Range	rangeX = new Range( 0, 1 );
        private Range	rangeY = new Range( 0, 1 );

        /// <summary>
        /// Chart's X range.
        /// </summary>
        /// 
        /// <remarks><para>The value sets the X range of data to be displayed on the chart.</para></remarks>
        /// 
        [Browsable( false )]
        public Range RangeX
        {
            get { return rangeX; }
            set
            {
                rangeX = value;
                UpdateYRange( );
                Invalidate( );
            }
        }

        /// <summary>
        /// Chart's Y range.
        /// </summary>
        /// 
        /// <remarks>The value sets the Y range of data to be displayed on the chart.</remarks>
        ///
        [Browsable( false )]
        public Range RangeY
        {
            get { return rangeY; }
            set
            {
                rangeY = value;
                Invalidate( );
            }
        }

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Chart"/> class.
        /// </summary>
        /// 
        public Chart( )
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent( );

            // update control style
            SetStyle( ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                ControlStyles.DoubleBuffer | ControlStyles.UserPaint, true );
        }

        /// <summary>
        /// Dispose the object.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( components != null )
                    components.Dispose( );

                // free graphics resources
                blackPen.Dispose( );
            }
            base.Dispose( disposing );
        }

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent( )
        {
            this.SuspendLayout( );
            // 
            // Chart
            // 
            this.Paint += new System.Windows.Forms.PaintEventHandler( this.Chart_Paint );
            this.ResumeLayout( false );

        }
        #endregion

        // Paint the control.
        private void Chart_Paint( object sender, PaintEventArgs e )
        {
            Graphics    g = e.Graphics;
            int			clientWidth = ClientRectangle.Width;
            int			clientHeight = ClientRectangle.Height;

            // fill with white background
            Brush backgroundBrush = new SolidBrush( BackColor );
            g.FillRectangle( backgroundBrush, 0, 0, clientWidth - 1, clientHeight - 1 );
            backgroundBrush.Dispose( );

            // draw a black rectangle
            g.DrawRectangle( blackPen, 0, 0, clientWidth - 1, clientHeight - 1 );

            // set clipping rectangle
            g.SetClip( new Rectangle( 2, 2, clientWidth - 4, clientHeight - 4 ) );

            // check if there are any data series
            if ( rangeX.Length != 0 )
            {
                double xFactor = (double) ( clientWidth - 10 ) / ( rangeX.Length );
                double yFactor = (double) ( clientHeight - 10 ) / ( ( rangeY.Length != 0 ) ? rangeY.Length : 1 );

                // walk through all data series
                foreach ( KeyValuePair<string, DataSeries> kvp in seriesTable )
                {
                    DataSeries series = kvp.Value;
                    // get data of the series
                    double[,] data = series.data;

                    // check for available data
                    if ( data == null )
                        continue;

                    // check series type
                    if ( series.type == SeriesType.Dots )
                    {
                        // draw dots
                        Brush	brush = new SolidBrush( series.color );
                        int		width = series.width;
                        int		r = width >> 1;

                        // draw all points
                        for ( int i = 0, n = data.GetLength( 0 ); i < n; i++ )
                        {
                            int x = (int) ( ( data[i, 0] - rangeX.Min ) * xFactor );
                            int y = (int) ( ( data[i, 1] - rangeY.Min ) * yFactor );

                            x += 5;
                            y = clientHeight - 6 - y;

                            g.FillRectangle( brush, x - r, y - r, width, width );
                        }
                        brush.Dispose( );
                    }
                    else if ( series.type == SeriesType.ConnectedDots )
                    {
                        // draw dots connected with 1-pixel width line
                        Brush	brush = new SolidBrush( series.color );
                        Pen		pen = new Pen( series.color, 1 );
                        int		width = series.width;
                        int		r = width >> 1;

                        int x1 = (int) ( ( data[0, 0] - rangeX.Min ) * xFactor );
                        int y1 = (int) ( ( data[0, 1] - rangeY.Min ) * yFactor );

                        x1 += 5;
                        y1 = clientHeight - 6 - y1;
                        g.FillRectangle( brush, x1 - r, y1 - r, width, width );

                        // draw all lines
                        for ( int i = 1, n = data.GetLength( 0 ); i < n; i++ )
                        {
                            int x2 = (int) ( ( data[i, 0] - rangeX.Min ) * xFactor );
                            int y2 = (int) ( ( data[i, 1] - rangeY.Min ) * yFactor );

                            x2 += 5;
                            y2 = clientHeight - 6 - y2;

                            g.FillRectangle( brush, x2 - r, y2 - r, width, width );
                            g.DrawLine( pen, x1, y1, x2, y2 );

                            x1 = x2;
                            y1 = y2;
                        }

                        pen.Dispose( );
                        brush.Dispose( );
                    }
                    else if ( series.type == SeriesType.Line )
                    {
                        // draw line
                        Pen pen = new Pen( series.color, series.width );

                        int x1 = (int) ( ( data[0, 0] - rangeX.Min ) * xFactor );
                        int y1 = (int) ( ( data[0, 1] - rangeY.Min ) * yFactor );

                        x1 += 5;
                        y1 = clientHeight - 6 - y1;

                        // draw all lines
                        for ( int i = 1, n = data.GetLength( 0 ); i < n; i++ )
                        {
                            int x2 = (int) ( ( data[i, 0] - rangeX.Min ) * xFactor );
                            int y2 = (int) ( ( data[i, 1] - rangeY.Min ) * yFactor );

                            x2 += 5;
                            y2 = clientHeight - 6 - y2;

                            g.DrawLine( pen, x1, y1, x2, y2 );

                            x1 = x2;
                            y1 = y2;
                        }
                        pen.Dispose( );
                    }
                }
            }
        }

        /// <summary>
        /// Add data series to the chart.
        /// </summary>
        /// 
        /// <param name="name">Data series name.</param>
        /// <param name="color">Data series color.</param>
        /// <param name="type">Data series type.</param>
        /// <param name="width">Width (depends on the data series type, see remarks).</param>
        /// 
        /// <remarks><para>Adds new empty data series to the collection of data series. To update this
        /// series the <see cref="UpdateDataSeries"/> method should be used.</para>
        /// 
        /// <para>The meaning of the width parameter depends on the data series type:
        /// <list type="bullet">
        /// <item><b>Line</b> - width of the line;</item>
        /// <item><b>Dots</b> - size of dots (rectangular dots with specified width and the same height);</item>
        /// <item><b>Connected dots</b> - size of dots (dots are connected with one pixel width line).</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// 
        public void AddDataSeries( string name, Color color, SeriesType type, int width )
        {
            AddDataSeries( name, color, type, width, true );
        }

        /// <summary>
        /// Add data series to the chart.
        /// </summary>
        /// 
        /// <param name="name">Data series name.</param>
        /// <param name="color">Data series color.</param>
        /// <param name="type">Data series type.</param>
        /// <param name="width">Width (depends on the data series type, see remarks).</param>
        /// <param name="updateYRange">Specifies if <see cref="RangeY"/> should be updated.</param>
        /// 
        /// <remarks><para>Adds new empty data series to the collection of data series.</para>
        /// 
        /// <para>The <b>updateYRange</b> parameter specifies if the data series may affect displayable
        /// Y range. If the value is set to false, then displayable Y range is not updated, but used the
        /// range, which was specified by user (see <see cref="RangeY"/> property). In the case if the
        /// value is set to true, the displayable Y range is recalculated to fully fit the new data
        /// series.</para>
        /// </remarks>
        /// 
        public void AddDataSeries( string name, Color color, SeriesType type, int width, bool updateYRange )
        {
            // create new series definition ...
            DataSeries	series = new DataSeries( );
            // ... add fill it
            series.color = color;
            series.type = type;
            series.width = width;
            series.updateYRange = updateYRange;
            // add to series table
            seriesTable.Add( name, series );
        }

        /// <summary>
        /// Update data series on the chart.
        /// </summary>
        /// 
        /// <param name="name">Data series name to update.</param>
        /// <param name="data">Data series values.</param>
        /// 
        public void UpdateDataSeries( string name, double[,] data )
        {
            if ( !seriesTable.ContainsKey( name ) )
                throw new ArgumentException( "The chart does not contain data series with name: " + name );

            // get data series
            DataSeries	series = seriesTable[name];
            // update data
            series.data = ( data != null ) ? (double[,]) data.Clone( ) : null;

            // update Y range
            if ( series.updateYRange )
                UpdateYRange( );
            // invalidate the control
            Invalidate( );
        }

        /// <summary>
        /// Remove data series from the chart.
        /// </summary>
        /// 
        /// <param name="name">Data series name to remove.</param>
        /// 
        public void RemoveDataSeries( string name )
        {
            // remove data series from table
            seriesTable.Remove( name );
            // invalidate the control
            Invalidate( );
        }

        /// <summary>
        /// Remove all data series from the chart.
        /// </summary>
        public void RemoveAllDataSeries( )
        {
            // remove all data series from table
            seriesTable.Clear( );
            // invalidate the control
            Invalidate( );
        }

        /// <summary>
        /// Update Y range.
        /// </summary>
        private void UpdateYRange( )
        {
            float minY = float.MaxValue;
            float maxY = float.MinValue;

            // walk through all data series
            foreach ( KeyValuePair<string, DataSeries> kvp in seriesTable )
            {
                DataSeries series = kvp.Value;
                // get data of the series
                double[,] data = series.data;

                if ( ( series.updateYRange ) && ( data != null ) )
                {
                    for ( int i = 0, n = data.GetLength( 0 ); i < n; i++ )
                    {
                        if ( rangeX.IsInside( (float) data[i, 0] ) )
                        {
                            float v = (float) data[i, 1];

                            // check for max
                            if ( v > maxY )
                                maxY = v;
                            // check for min
                            if ( v < minY )
                                minY = v;
                        }
                    }
                }
            }

            // update Y range, if there are any data
            if ( ( minY != double.MaxValue ) || ( maxY != double.MinValue ) )
            {
                rangeY = new Range( minY, maxY );
            }
        }
    }
}
