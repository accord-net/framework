using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Accord.Controls.Cyotek.Demo
{
  // Cyotek ImageBox
  // Copyright (c) 2010-2015 Cyotek Ltd.
  // http://cyotek.com
  // http://cyotek.com/blog/tag/imagebox

  // Licensed under the MIT License. See license.txt for the full text.

  // If you use this control in your applications, attribution, donations or contributions are welcome.

  internal partial class SwitchImageDuringZoomDemoForm : BaseForm
  {
    #region Instance Fields

    private int _virtualZoom;

    #endregion

    #region Public Constructors

    public SwitchImageDuringZoomDemoForm()
    {
      InitializeComponent();
    }

    #endregion

    #region Overridden Methods

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (components != null)
        {
          components.Dispose();
        }

        if (this.ImageCache != null)
        {
          foreach (Image image in this.ImageCache.Values)
          {
            image.Dispose();
          }
          this.ImageCache = null;
        }
      }
      base.Dispose(disposing);
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      LayerData = new List<MapLayerData>();

      // add some map layers for the different zoom levels
      this.AddLayer("map10", int.MinValue, 0);
      this.AddLayer("map20", 0, 2);
      this.AddLayer("map30", 2, 4);
      this.AddLayer("map40", 4, 6);
      this.AddLayer("map50", 6, 8);
      this.AddLayer("map60", 8, 9);
      this.AddLayer("map70", 9, 10);
      this.AddLayer("map80", 10, 11);
      this.AddLayer("map90", 11, 12);
      this.AddLayer("map100", 12, int.MaxValue);

      // load the lowest detail map
      imageBox.Image = this.GetMapImage("map10");

      // now zoom in a bit
      this.VirtualZoom = 5;
      this.UpdateMap();
      imageBox.Zoom = 125;
      imageBox.CenterAt(3350, 800);
    }

    #endregion

    #region Private Properties

    private IDictionary<string, Image> ImageCache { get; set; }

    private bool IsUpdatingMap { get; set; }

    private int Layer { get; set; }

    private List<MapLayerData> LayerData { get; set; }

    private bool ResetZoomOnUpdate { get; set; }

    private int VirtualZoom
    {
      get { return _virtualZoom; }
      set
      {
        _virtualZoom = value;

        this.UpdateZoomLabel();
      }
    }

    #endregion

    #region Private Members

    private void AddLayer(string name, int lowerZoom, int upperZoom)
    {
      // The larger map sizes (>map50) are 80MB, so I'm not including them in the GitHub repository.
      // Therefore, just silently skip any missing maps without raising an error

      if (File.Exists(this.GetMapFileName(name)))
      {
        MapLayerData data;

        data = new MapLayerData();
        data.Name = name;
        data.UpperZoom = upperZoom;
        data.LowerZoom = lowerZoom;

        this.LayerData.Add(data);
      }
    }

    private int FindNearestLayer(int zoom)
    {
      int result;

      result = -1;

      for (int i = 0; i < this.LayerData.Count; i++)
      {
        MapLayerData data;

        data = this.LayerData[i];

        if (zoom >= data.LowerZoom && zoom < data.UpperZoom)
        {
          result = i;
          break;
        }
      }

      return result;
    }

    private string GetMapFileName(string name)
    {
      return Path.GetFullPath(Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\maps"), Path.ChangeExtension(name, ".jpg")));
    }

    private Image GetMapImage(string name)
    {
      Image result;

      if (this.ImageCache == null)
      {
        this.ImageCache = new Dictionary<string, Image>();
      }

      if (!this.ImageCache.TryGetValue(name, out result))
      {
        this.SetStatus("Loading image...");

        result = Image.FromFile(this.GetMapFileName(name));

        this.ImageCache.Add(name, result);

        this.SetStatus(string.Empty);
      }

      this.SetMessage(string.Format("Switching to image {0}.jpg", name));

      return result;
    }

    private void SetMessage(string message)
    {
      messageLabel.Text = message;
      resetMessageTimer.Stop();
      resetMessageTimer.Start();
    }

    private void SetStatus(string message)
    {
      Cursor.Current = string.IsNullOrEmpty(message) ? Cursors.Default : Cursors.WaitCursor;

      statusToolStripStatusLabel.Text = message;
      statusToolStripStatusLabel.Owner.Refresh();
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

    private void UpdateMap()
    {
      if (!this.IsUpdatingMap)
      {
        int mapLayer;

        this.IsUpdatingMap = true;

        mapLayer = this.FindNearestLayer(this.VirtualZoom);

        if (mapLayer != -1 && mapLayer != this.Layer)
        {
          MapLayerData data;
          Image newImage;
          RectangleF currentViewport;
          RectangleF newViewport;
          Size currentSize;
          float vAspectRatio;
          float hAspectRatio;

          this.Layer = mapLayer;
          data = this.LayerData[mapLayer];
          mapNameToolStripStatusLabel.Text = data.Name;

          newImage = this.GetMapImage(data.Name);
          currentViewport = imageBox.GetSourceImageRegion();
          currentSize = imageBox.Image.Size;

          hAspectRatio = newImage.Width / (float)currentSize.Width;
          vAspectRatio = newImage.Height / (float)currentSize.Height;

          imageBox.BeginUpdate();
          imageBox.Image = newImage;
          newViewport = new RectangleF(currentViewport.X * hAspectRatio, currentViewport.Y * vAspectRatio, currentViewport.Width * hAspectRatio, currentViewport.Height * vAspectRatio);
          imageBox.ZoomToRegion(newViewport);

          if (this.ResetZoomOnUpdate)
          {
            this.ResetZoomOnUpdate = false;
            imageBox.Zoom = 100;
            imageBox.CenterToImage();
          }

          imageBox.EndUpdate();
        }

        this.IsUpdatingMap = false;
      }
    }

    private void UpdateZoomLabel()
    {
      zoomToolStripStatusLabel.Text = string.Format("{0}% [{1}]", imageBox.Zoom, this.VirtualZoom);
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

    private void imageBox_MouseLeave(object sender, EventArgs e)
    {
      cursorToolStripStatusLabel.Text = string.Empty;
    }

    private void imageBox_MouseMove(object sender, MouseEventArgs e)
    {
      this.UpdateCursorPosition(e.Location);
    }

    private void imageBox_ZoomChanged(object sender, EventArgs e)
    {
      this.UpdateZoomLabel();
    }

    private void imageBox_Zoomed(object sender, ImageBoxZoomEventArgs e)
    {
      if ((e.Source & ImageBoxActionSources.User) == ImageBoxActionSources.User)
      {
        if ((e.Actions & ImageBoxZoomActions.ActualSize) == ImageBoxZoomActions.ActualSize)
        {
          this.VirtualZoom = 0;
          this.ResetZoomOnUpdate = true;
        }
        else if ((e.Actions & ImageBoxZoomActions.ZoomIn) == ImageBoxZoomActions.ZoomIn)
        {
          this.VirtualZoom++;
        }
        else if ((e.Actions & ImageBoxZoomActions.ZoomOut) == ImageBoxZoomActions.ZoomOut)
        {
          this.VirtualZoom--;
        }

        // TODO: Currently the ZoomChanged and Zoomed events are raised after the zoom level has changed, but before any
        // actions such as modifying scrollbars occur. This means methods such as GetSourceImageRegion will return the
        // wrong X and Y values. Until this is fixed, using a timer to trigger the change.
        // However, if you had lots of map changes to make then using a timer would be a good idea regardless; for example
        // if the user rapdily zooms through the available levels, they'll have a smoother experience if you only load
        // the data once they've stopped zooming
        refreshMapTimer.Stop();
        refreshMapTimer.Start();
      }
    }

    private void refreshMapTimer_Tick(object sender, EventArgs e)
    {
      refreshMapTimer.Stop();

      this.UpdateMap();
    }

    private void resetMessageTimer_Tick(object sender, EventArgs e)
    {
      resetMessageTimer.Stop();
      messageLabel.Text = string.Empty;
    }

    #endregion

    #region Nested Types

    private struct MapLayerData
    {
      #region Public Properties

      public int LowerZoom { get; set; }

      public string Name { get; set; }

      public int UpperZoom { get; set; }

      #endregion
    }

    #endregion
  }
}
