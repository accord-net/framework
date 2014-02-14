// Accord Control Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace Accord.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Accord.Math;
    using Accord.Audio;
    using AForge;

    /// <summary>
    ///   Waveform chart control.
    /// </summary>
    /// 
    /// <remarks><para>The Waveform chart control allows to display multiple
    /// waveforms at time.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create data series array
    /// float[] testValues = new float[128];
    /// // fill data series
    /// for ( int i = 0; i &lt; 128; i++ )
    /// {
    ///     testValues[i] = Math.Sin( i / 18.0 * Math.PI );
    /// }
    /// // add new waveform to the chart
    /// chart.AddWaveform( "Test", Color.DarkGreen, 3 );
    /// // update the chart
    /// chart.UpdateWaveform( "Test", testValues );
    /// </code>
    /// </remarks>
    /// 
    public class Wavechart : System.Windows.Forms.Control
    {
        // waveform data
        private class Waveform
        {
            public float[] data = null;
            public int samples;

            public Color color = Color.Blue;
            public int width = 1;

            public bool updateYRange = true;
            
        }

        private Dictionary<string, Waveform> waveTable = new Dictionary<string, Waveform>();

        private Pen bordersPen;
        private Brush backgroundBrush;

        private DoubleRange rangeX = new DoubleRange(0, 0);
        private DoubleRange rangeY = new DoubleRange(0, 0);

        /// <summary>
        ///   Gets or sets the background color for the control.
        /// </summary>
        /// 
        /// <returns>A <see cref="T:System.Drawing.Color"/> that represents the 
        ///   background color of the control. The default is the value of the 
        ///   <see cref="P:System.Windows.Forms.Control.DefaultBackColor"/> 
        ///   property.
        /// </returns>
        /// 
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        /// 
        [DefaultValue("Color.White")]
        public override Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        /// <summary>
        ///   Chart's X range.
        /// </summary>
        /// 
        /// <remarks><para>The value sets the X range of data to be displayed on the chart.</para></remarks>
        /// 
        public DoubleRange RangeX
        {
            get { return rangeX; }
            set
            {
                rangeX = value;
                Invalidate();
            }
        }

        /// <summary>
        ///   Chart's Y range.
        /// </summary>
        /// 
        /// <remarks>The value sets the Y range of data to be displayed on the chart.</remarks>
        ///
        public DoubleRange RangeY
        {
            get { return rangeY; }
            set
            {
                rangeY = value;
                Invalidate();
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether to
        ///   create a simple wave chart only (no scaling).
        /// </summary>
        /// 
        /// <value><c>true</c> to enable simple mode; otherwise, <c>false</c>.</value>
        /// 
        public bool SimpleMode { get; set; }


        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Wavechart"/> class.
        /// </summary>
        /// 
        public Wavechart()
        {
            InitializeComponent();

            this.DoubleBuffered = true;

            this.bordersPen = new Pen(Color.Black);
            this.backgroundBrush = new SolidBrush(BackColor);
        }

        /// <summary>
        ///   Dispose the object.
        /// </summary>
        /// 
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                    components = null;
                }

                // free graphics resources
                if (bordersPen != null)
                {
                    bordersPen.Dispose();
                    bordersPen = null;
                }

                if (backgroundBrush != null)
                {
                    backgroundBrush.Dispose();
                    backgroundBrush = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Chart
            // 
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        ///   Paints the background of the control.
        /// </summary>
        /// 
        /// <param name="pevent">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains information about the control to paint.</param>
        /// 
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
        }

        /// <summary>
        ///   Paints the control.
        /// </summary>
        /// 
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            int clientWidth = ClientRectangle.Width;
            int clientHeight = ClientRectangle.Height;

            // fill with background
            g.FillRectangle(backgroundBrush, 0, 0, clientWidth - 1, clientHeight - 1);

            // draw borders
            g.DrawRectangle(bordersPen, 0, 0, clientWidth - 1, clientHeight - 1);

            if (DesignMode)
                return;

            DoubleRange rangeClientX = new DoubleRange(0, clientWidth);
            DoubleRange rangeClientY = new DoubleRange(clientHeight, 0);

            // walk through all data series
            foreach (Waveform waveform in waveTable.Values)
            {
                // get data of the waveform
                float[] data = waveform.data;

                // check for available data
                if (data == null) continue;


                using (Pen pen = new Pen(waveform.color, waveform.width))
                {
                    if (SimpleMode)
                    {
                        int blockSize = waveform.samples / clientWidth;
                        for (int x = 0; x < clientWidth; x++)
                        {
                            double max = data.RootMeanSquare(x * blockSize, blockSize);
                            int y = clientHeight / 2 + (int)(max * clientHeight);
                            g.DrawLine(pen, x, clientHeight - y, x, y);
                        }
                    }
                    else
                    {
                        int xPrev = 0;
                        int yPrev = (int)rangeY.Scale(rangeClientY, data[0]);

                        for (int x = 0; x < clientWidth; x++)
                        {
                            int index = (int)rangeClientX.Scale(rangeX, x);
                            if (index < 0 || index >= data.Length)
                                index = data.Length - 1;
                            int y = (int)rangeY.Scale(rangeClientY, data[index]);

                            g.DrawLine(pen, xPrev, yPrev, x, y);

                            xPrev = x;
                            yPrev = y;
                        }
                    }
                }
            }
        }


        /// <summary>
        ///   Raises the <see cref="E:System.Windows.Forms.Control.BackColorChanged"/> event.
        /// </summary>
        /// 
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        /// 
        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);

            Brush oldBrush = backgroundBrush;
            backgroundBrush = new SolidBrush(BackColor);
            oldBrush.Dispose();
        }

        /// <summary>
        ///   Add Waveform to the chart.
        /// </summary>
        /// 
        /// <param name="name">Waveform name.</param>
        /// <param name="color">Waveform color.</param>
        /// <param name="width">Waveform width.</param>
        /// 
        /// <remarks><para>Adds new empty waveform to the collection of waves. To update this
        /// wave the <see cref="UpdateWaveform(string, float[])"/> method should be used.</para>
        /// </remarks>
        /// 
        public void AddWaveform(string name, Color color, int width)
        {
            AddWaveform(name, color, width, true);
        }

        /// <summary>
        ///   Add Waveform to the chart.
        /// </summary>
        /// 
        /// <param name="name">Waveform name.</param>
        /// <param name="color">Waveform color.</param>
        /// <param name="width">Waveform width.</param>
        /// <param name="updateYRange">Specifies if <see cref="RangeY"/> should be updated.</param>
        /// 
        /// <remarks><para>Adds new empty waveform to the collection of waves. To update this
        /// wave the <see cref="UpdateWaveform(string, float[])"/> method should be used.</para>
        /// </remarks>
        /// 
        /// <remarks><para>Adds new empty data series to the collection of data series.</para>
        /// 
        /// <para>The <b>updateYRange</b> parameter specifies if the waveform may affect displayable
        /// Y range. If the value is set to false, then displayable Y range is not updated, but used the
        /// range, which was specified by user (see <see cref="RangeY"/> property). In the case if the
        /// value is set to true, the displayable Y range is recalculated to fully fit the new data
        /// series.</para>
        /// </remarks>
        /// 
        public void AddWaveform(string name, Color color, int width, bool updateYRange)
        {
            Waveform series = new Waveform();
            series.color = color;
            series.width = width;
            series.updateYRange = updateYRange;

            waveTable.Add(name, series);
        }

        /// <summary>
        ///   Update Waveform on the chart.
        /// </summary>
        /// 
        /// <param name="name">Data series name to update.</param>
        /// <param name="data">Data series values.</param>
        /// 
        public void UpdateWaveform(string name, float[] data)
        {
            UpdateWaveform(name, data, data.Length);
        }

        /// <summary>
        ///   Update Waveform on the chart.
        /// </summary>
        /// 
        /// <param name="name">Data series name to update.</param>
        /// <param name="data">Data series values.</param>
        /// <param name="samples">The number of samples in the <paramref name="data"/> array.</param>
        /// 
        public void UpdateWaveform(string name, float[] data, int samples)
        {
            if (samples > data.Length)
                throw new ArgumentOutOfRangeException("samples");

            // get data series
            Waveform series = waveTable[name];

            // update data
            series.data = data;
            series.samples = samples;

            // update Y range
            if (series.updateYRange)
                UpdateYRange();

            Invalidate();
        }

        /// <summary>
        ///   Remove a Waveform from the chart.
        /// </summary>
        /// 
        /// <param name="name">Waveform name to remove.</param>
        /// 
        public void RemoveWaveform(string name)
        {
            waveTable.Remove(name);

            Invalidate();
        }

        /// <summary>
        ///   Remove all waveforms from the chart.
        /// </summary>
        /// 
        public void RemoveAllWaveforms()
        {
            waveTable.Clear();

            Invalidate();
        }

        /// <summary>
        ///   Update Y range.
        /// </summary>
        /// 
        private void UpdateYRange()
        {
            float minY = float.MaxValue;
            float maxY = float.MinValue;

            // walk through all data series
            foreach (Waveform wave in waveTable.Values)
            {
                // get data of the series
                float[] data = wave.data;

                if ((wave.updateYRange) && (data != null))
                {
                    // Let the compiler perform optimizations.
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (data[i] > maxY)
                            maxY = data[i];
                        if (data[i] < minY)
                            minY = data[i];
                    }
                }
            }

            // update Y range, if there are any data
            if ((minY != float.MaxValue) || (maxY != float.MinValue))
            {
                rangeY = new DoubleRange(minY, maxY);
            }
        }
    }
}
