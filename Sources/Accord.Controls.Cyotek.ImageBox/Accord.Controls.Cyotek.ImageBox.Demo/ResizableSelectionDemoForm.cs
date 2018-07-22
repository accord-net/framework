using System;
using System.Collections.Generic;
using System.ComponentModel;
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

  internal partial class ResizableSelectionDemoForm : BaseForm
  {
    #region Public Constructors

    public ResizableSelectionDemoForm()
    {
      InitializeComponent();
    }

    #endregion

    #region Overridden Methods

    /// <summary>
    /// Raises the <see cref="E:System.Windows.Forms.Form.Load"/> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data. </param>
    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      this.UpdateStatusBar();

      // set a default selection
      imageBox.SelectionRegion = new RectangleF(0, 0, 64, 64);

      // apply a minimum selection size for resize operations
      imageBox.MinimumSelectionSize = new Size(8, 8);

      // setup the option lists
      foreach (DragHandle handle in imageBox.DragHandles)
      {
        enabledCheckedListBox.Items.Add(handle.Anchor, handle.Enabled);
        visibleCheckedListBox.Items.Add(handle.Anchor, handle.Visible);
      }
    }

    #endregion

    #region Private Members

    private void SetStatus(string message)
    {
      statusToolStripStatusLabel.Text = message;
    }

    private void UpdateCursorPosition(Point location)
    {
      if (imageBox.IsPointInImage(location))
      {
        Point point;
        point = imageBox.PointToImage(location);
        cursorToolStripStatusLabel.Text = this.FormatPoint(point);
      }
      else
      {
        cursorToolStripStatusLabel.Text = string.Empty;
      }
    }

    private void UpdateStatusBar()
    {
      autoScrollPositionToolStripStatusLabel.Text = this.FormatPoint(imageBox.AutoScrollPosition);
      imageSizeToolStripStatusLabel.Text = this.FormatRectangle(imageBox.GetImageViewPort());
      zoomToolStripStatusLabel.Text = string.Format("{0}%", imageBox.Zoom);
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

    private void enabledCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
    {
      DragHandleAnchor anchor;

      anchor = (DragHandleAnchor)enabledCheckedListBox.Items[e.Index];

      imageBox.DragHandles[anchor].Enabled = e.NewValue == CheckState.Checked;

      imageBox.Invalidate(); // No change events on the DragHandleCollection class so need to manually refresh
    }

    private void imageBox_MouseLeave(object sender, EventArgs e)
    {
      cursorToolStripStatusLabel.Text = string.Empty;
    }

    private void imageBox_MouseMove(object sender, MouseEventArgs e)
    {
      this.UpdateCursorPosition(e.Location);
    }

    private void imageBox_Resize(object sender, EventArgs e)
    {
      this.UpdateStatusBar();
    }

    private void imageBox_Scroll(object sender, ScrollEventArgs e)
    {
      this.UpdateStatusBar();
    }

    private void imageBox_Selected(object sender, EventArgs e)
    {
      this.UpdateStatusBar();
      eventsListBox.AddEvent((Control)sender, "Selected");
    }

    private void imageBox_Selecting(object sender, ImageBoxCancelEventArgs e)
    {
      eventsListBox.AddEvent((Control)sender, "Selecting", new Dictionary<string, object>
                                                           {
                                                             {
                                                               "Location", e.Location
                                                             },
                                                             {
                                                               "Cancel", e.Cancel
                                                             }
                                                           });
    }

    private void imageBox_SelectionMoved(object sender, EventArgs e)
    {
      this.SetStatus(string.Empty);

      eventsListBox.AddEvent((Control)sender, "SelectionMoved");
    }

    private void imageBox_SelectionMoving(object sender, CancelEventArgs e)
    {
      this.SetStatus("Press escape to cancel move.");

      eventsListBox.AddEvent((Control)sender, "SelectionMoving", new Dictionary<string, object>
                                                                 {
                                                                   {
                                                                     "Cancel", e.Cancel
                                                                   }
                                                                 });
    }

    private void imageBox_SelectionRegionChanged(object sender, EventArgs e)
    {
      selectionToolStripStatusLabel.Text = this.FormatRectangle(imageBox.SelectionRegion);
    }

    private void imageBox_SelectionResized(object sender, EventArgs e)
    {
      this.SetStatus(string.Empty);

      eventsListBox.AddEvent((Control)sender, "SelectionResized");
    }

    private void imageBox_SelectionResizing(object sender, CancelEventArgs e)
    {
      this.SetStatus("Press escape to cancel resize.");

      eventsListBox.AddEvent((Control)sender, "SelectionResizing", new Dictionary<string, object>
                                                                   {
                                                                     {
                                                                       "Cancel", e.Cancel
                                                                     }
                                                                   });
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
    }

    private void imageBox_Zoomed(object sender, ImageBoxZoomEventArgs e)
    {
      this.UpdateStatusBar();
    }

    private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
    {
      imageBox.SelectAll();
    }

    private void selectNoneToolStripMenuItem_Click(object sender, EventArgs e)
    {
      imageBox.SelectNone();
    }

    private void visibleCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
    {
      DragHandleAnchor anchor;

      anchor = (DragHandleAnchor)visibleCheckedListBox.Items[e.Index];

      imageBox.DragHandles[anchor].Visible = e.NewValue == CheckState.Checked;

      imageBox.Invalidate(); // No change events on the DragHandleCollection class so need to manually refresh
    }

    #endregion
  }
}
