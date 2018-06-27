using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Accord.Controls.Cyotek.Demo.Properties;

namespace Accord.Controls.Cyotek.Demo
{
  // Cyotek ImageBox
  // Copyright (c) 2010-2015 Cyotek Ltd.
  // http://cyotek.com
  // http://cyotek.com/blog/tag/imagebox

  // Licensed under the MIT License. See license.txt for the full text.

  // If you use this control in your applications, attribution, donations or contributions are welcome.

  internal partial class ScaledAdornmentsDemoForm : BaseForm
  {
    #region Fields

    private List<Point> _landmarks;

    private Bitmap _markerImage;

    #endregion

    #region Constructors

    public ScaledAdornmentsDemoForm()
    {
      this.InitializeComponent();
    }

    #endregion

    #region Methods

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      imageBox.ZoomToFit();

      _markerImage = Resources.MapMarker;

      _landmarks = new List<Point>();
      this.AddLandmark(new Point(467, 447));
      this.AddLandmark(new Point(662, 262));
      this.AddLandmark(new Point(779, 239));
    }

    private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
      AboutDialog.ShowAboutDialog();
    }

    private void AddLandmark(Point point)
    {
      Debug.Print("Added landmark: {0}", point);

      _landmarks.Add(new Point(point.X, point.Y));
    }

    private void closeToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void imageBox_MouseClick(object sender, MouseEventArgs e)
    {
      if (imageBox.IsPointInImage(e.Location))
      {
        // add a new landmark
        this.AddLandmark(imageBox.PointToImage(e.Location));

        // force the image to repaint
        imageBox.Invalidate();
      }
    }

    private void imageBox_MouseLeave(object sender, EventArgs e)
    {
      positionToolStripStatusLabel.Text = string.Empty;
    }

    private void imageBox_MouseMove(object sender, MouseEventArgs e)
    {
      this.UpdateCursorPosition(e.Location);
    }

    private void imageBox_Paint(object sender, PaintEventArgs e)
    {
      Graphics g;
      GraphicsState originalState;
      Size scaledSize;
      Size originalSize;
      Size drawSize;
      bool scaleAdornmentSize;

      scaleAdornmentSize = scaleAdornmentsCheckBox.Checked;

      g = e.Graphics;

      originalState = g.Save();

      // Work out the size of the marker graphic according to the current zoom level
      originalSize = _markerImage.Size;
      scaledSize = imageBox.GetScaledSize(originalSize);
      drawSize = scaleAdornmentSize ? scaledSize : originalSize;

      foreach (Point landmark in _landmarks)
      {
        Point location;

        // Work out the location of the marker graphic according to the current zoom level and scroll offset
        location = imageBox.GetOffsetPoint(landmark);

        // adjust the location so that the image is displayed above the location and centered to it
        location.Y -= drawSize.Height;
        location.X -= drawSize.Width >> 1;

        // Draw the marker
        g.InterpolationMode = InterpolationMode.NearestNeighbor;
        g.DrawImage(_markerImage, new Rectangle(location, drawSize), new Rectangle(Point.Empty, originalSize), GraphicsUnit.Pixel);
      }

      g.Restore(originalState);
    }

    private void scaleAdornmentsCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      imageBox.Invalidate();
    }

    private void UpdateCursorPosition(Point location)
    {
      Point point;

      point = imageBox.PointToImage(location);

      positionToolStripStatusLabel.Text = this.FormatPoint(point);
    }

    #endregion
  }
}
