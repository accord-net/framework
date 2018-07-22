using System;
using System.Drawing;
using System.Windows.Forms;

namespace Accord.Controls.Cyotek.Demo
{
  // Cyotek ImageBox
  // Copyright (c) 2010-2015 Cyotek Ltd.
  // http://cyotek.com
  // http://cyotek.com/blog/tag/imagebox

  // Licensed under the MIT License. See license.txt for the full text.

  // If you use this control in your applications, attribution, donations or contributions are welcome.

  internal partial class DragTestForm : BaseForm
  {
    #region Instance Fields

    private RectangleF _dragItem;

    private Point _dragOffset;

    private bool _isDragging;

    #endregion

    #region Public Constructors

    public DragTestForm()
    {
      InitializeComponent();

      _dragItem = new RectangleF(32, 32, 64, 32);
    }

    #endregion

    #region Event Handlers

    private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
      AboutDialog.ShowAboutDialog();
    }

    private void closeToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void imageBox_MouseDown(object sender, MouseEventArgs e)
    {
      Point imagePoint;

      imagePoint = imageBox.PointToImage(e.Location);
      if (_dragItem.Contains(imagePoint) && e.Button == MouseButtons.Left)
      {
        _isDragging = true;
        _dragOffset = new Point(imagePoint.X - (int)_dragItem.Location.X, imagePoint.Y - (int)_dragItem.Location.Y);
      }
    }

    private void imageBox_MouseMove(object sender, MouseEventArgs e)
    {
      Point imagePoint;

      imagePoint = imageBox.PointToImage(e.Location, true);

      // set the cursor
      imageBox.Cursor = _dragItem.Contains(imagePoint) ? Cursors.SizeAll : Cursors.Default;

      // drag!
      if (_isDragging)
      {
        int x;
        int y;

        x = Math.Max(0, imagePoint.X - _dragOffset.X);
        y = Math.Max(0, imagePoint.Y - _dragOffset.Y);

        if (x + _dragItem.Width >= imageBox.VirtualSize.Width)
        {
          x = imageBox.VirtualSize.Width - (int)_dragItem.Width;
        }
        if (y + _dragItem.Height >= imageBox.VirtualSize.Height)
        {
          y = imageBox.VirtualSize.Height - (int)_dragItem.Height;
        }

        _dragItem.Location = new PointF(x, y);
        imageBox.Invalidate();
      }
    }

    private void imageBox_MouseUp(object sender, MouseEventArgs e)
    {
      _isDragging = false;
    }

    private void imageBox_Selecting(object sender, ImageBoxCancelEventArgs e)
    {
      if (_dragItem.Contains(imageBox.PointToImage(e.Location)))
      {
        e.Cancel = true;
      }
    }

    private void imageBox_SelectionRegionChanged(object sender, EventArgs e)
    {
      if (!imageBox.SelectionRegion.IsEmpty)
      {
        _dragItem = imageBox.SelectionRegion;
        imageBox.SelectionRegion = RectangleF.Empty;
      }
    }

    private void imageBox_VirtualDraw(object sender, PaintEventArgs e)
    {
      RectangleF bounds;

      // draw the virtual area
      bounds = imageBox.GetOffsetRectangle(new RectangleF(PointF.Empty, imageBox.VirtualSize));

      using (Brush brush = new SolidBrush(Color.FromArgb(128, Color.Goldenrod)))
      {
        e.Graphics.FillRectangle(brush, bounds);
      }

      e.Graphics.DrawRectangle(Pens.DarkGoldenrod, bounds.X, bounds.Y, bounds.Width, bounds.Height);

      // draw the "drag" item
      bounds = imageBox.GetOffsetRectangle(_dragItem);

      using (Brush brush = new SolidBrush(Color.FromArgb(128, Color.CornflowerBlue)))
      {
        e.Graphics.FillRectangle(brush, bounds);
      }

      e.Graphics.DrawRectangle(Pens.Blue, bounds.X, bounds.Y, bounds.Width, bounds.Height);
    }

    #endregion
  }
}
