// Cyotek ImageBox
// Copyright (c) 2010-2017 Cyotek Ltd.
// http://cyotek.com
// http://cyotek.com/blog/tag/imagebox

// Licensed under the MIT License. See license.txt for the full text.

// If you use this control in your applications, attribution, donations or contributions are welcome.

using System;
using System.Drawing;
using System.Windows.Forms;
using Accord.Controls.Cyotek.Demo.Properties;

// demonstration was derived based on the following forum post
// https://forums.cyotek.com/imagebox/problem-when-trying-to-create-a-minimap-using-imagebox/

namespace Accord.Controls.Cyotek.Demo
{
  internal partial class MiniMapDemoForm : BaseForm
  {
    #region Fields

    private Rectangle _minimap;

    private Bitmap _thumbnailBitmap;

    #endregion

    #region Constructors

    public MiniMapDemoForm()
    {
      this.InitializeComponent();
    }

    #endregion

    #region Methods

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      zoomImageBox.Image = Resources.Sample;

      this.UpdateMiniMap();
    }

    private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
      AboutDialog.ShowAboutDialog();
    }

    private void closeToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void miniMapImageBox_Paint(object sender, PaintEventArgs e)
    {
      if (_thumbnailBitmap != null)
      {
        using (Pen pen = new Pen(Color.Aquamarine, 3))
        {
          e.Graphics.DrawImage(_thumbnailBitmap, miniMapImageBox.GetImageViewPort().
                                                                 Location);
          e.Graphics.DrawRectangle(pen, _minimap.X, _minimap.Y, _minimap.Width, _minimap.Height);
        }
      }
    }

    private void RefreshMiniMap()
    {
      Image image;

      image = zoomImageBox.Image;

      if (image != null)
      {
        Bitmap minimap;
        Size minimapSize;

        // https://github.com/cyotek/Cyotek.Windows.Forms.ImageBox/issues/27
        // I found that ImageBox can cause performance issues if you instruct it
        // to paint a large image that is zoomed out repeatly. As the image for
        // our minimap doesn't actually change or allow zooming, lets create
        // a tiny version up front. To make it easy, the "mini map" ImageBox
        // has its VirtualMode property set to True, and the SizeMode set to
        // Fit. I then set the VirtualSize to be the size if the original
        // image and it will then calculate the size I need for the thumbnail
        // which saves me some manual work. However, it does mean that I need
        // to manually paint the thumbnail

        miniMapImageBox.VirtualSize = image.Size;
        minimapSize = miniMapImageBox.GetImageViewPort().
                                      Size;
        minimap = new Bitmap(minimapSize.Width, minimapSize.Height);

        // generate the thumbnail
        using (Graphics g = Graphics.FromImage(minimap))
        {
          g.DrawImage(image, new Rectangle(Point.Empty, minimap.Size), new Rectangle(Point.Empty, image.Size), GraphicsUnit.Pixel);
        }

        // always clean up
        if (_thumbnailBitmap != null)
        {
          _thumbnailBitmap.Dispose();
          _thumbnailBitmap = null;
        }
        _thumbnailBitmap = minimap;

        // force a paint of the minimap
        this.UpdateMiniMap();
      }
    }

    private void splitContainer_SplitterMoved(object sender, SplitterEventArgs e)
    {
      this.RefreshMiniMap();
    }

    private void UpdateMiniMap()
    {
      Rectangle proposedView;
      Size viewSize;
      Point location;
      double x;
      double y;
      double w;
      double h;

      // define the initial size. We'll take the current
      // size from the source imagebox's image viewport
      viewSize = zoomImageBox.GetImageViewPort().
                              Size;
      w = viewSize.Width;
      h = viewSize.Height;

      // next we need to scale the size to match the zoomfactor of the source imagebox
      w /= zoomImageBox.ZoomFactor;
      h /= zoomImageBox.ZoomFactor;

      // next we scale the size again - this time by the zoomfactor the destination imagebox
      w *= miniMapImageBox.ZoomFactor;
      h *= miniMapImageBox.ZoomFactor;

      // with the size define, we can now turn out attention to the origin
      // first, we get the current auto scroll offsets, and reverse them
      // to give us our origin
      x = -zoomImageBox.AutoScrollPosition.X;
      y = -zoomImageBox.AutoScrollPosition.Y;

      // next, we need to scale that to account for the source imagebox zoom
      x /= zoomImageBox.ZoomFactor;
      y /= zoomImageBox.ZoomFactor;

      // as with the size, we need to scale again to account for the destination imagebox
      x *= miniMapImageBox.ZoomFactor;
      y *= miniMapImageBox.ZoomFactor;

      // and for our final action, we need to offset the origin to account
      // for where the destination imagebox is painting the output image
      location = miniMapImageBox.GetImageViewPort().
                                 Location;
      x += location.X;
      y += location.Y;

      // all done, create the final rectangle for painting
      proposedView = new Rectangle(Convert.ToInt32(x), Convert.ToInt32(y), Convert.ToInt32(w), Convert.ToInt32(h));

      // see if the final rectangle is different to the one already being used
      // to avoid painting if we don't need to
      if (proposedView != _minimap)
      {
        _minimap = proposedView;

        // force the destination to repaint to show the new rectangle
        // this has performance issues
        // https://github.com/cyotek/Cyotek.Windows.Forms.ImageBox/issues/27
        miniMapImageBox.Invalidate();

        imageViewPortToolStripStatusLabel.Text = "Image Viewport: " + zoomImageBox.GetImageViewPort().
                                                                                   ToString();
        calculatedRectangleToolStripStatusLabel.Text = "Rectangle: " + _minimap.ToString();
      }
    }

    private void zoomImageBox_ImageChanged(object sender, EventArgs e)
    {
      this.RefreshMiniMap();
    }

    private void zoomImageBox_Resize(object sender, EventArgs e)
    {
      this.RefreshMiniMap();
    }

    private void zoomImageBox_Scroll(object sender, ScrollEventArgs e)
    {
      this.UpdateMiniMap();
    }

    private void zoomImageBox_Zoomed(object sender, ImageBoxZoomEventArgs e)
    {
      this.UpdateMiniMap();
    }

    #endregion
  }
}
