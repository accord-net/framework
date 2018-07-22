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

  internal partial class BaseForm : Form
  {
    #region Public Constructors

    public BaseForm()
    {
      this.InitializeComponent();
    }

    #endregion

    #region Overridden Methods

    protected override void OnLoad(EventArgs e)
    {
      if (!this.DesignMode)
      {
        this.Font = SystemFonts.MessageBoxFont;
      }

      base.OnLoad(e);
    }

    protected override void OnShown(EventArgs e)
    {
      base.OnShown(e);

      Cursor.Current = Cursors.Default;
    }

    #endregion

    #region Protected Members

    protected string FormatPoint(Point point)
    {
      return string.Format("X:{0}, Y:{1}", point.X, point.Y);
    }

    protected string FormatRectangle(RectangleF rect)
    {
      return string.Format("X:{0}, Y:{1}, W:{2}, H:{3}", (int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
    }

    #endregion
  }
}
