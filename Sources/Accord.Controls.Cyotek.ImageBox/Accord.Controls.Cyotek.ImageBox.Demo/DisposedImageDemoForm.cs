using System;
using System.Drawing;

namespace Accord.Controls.Cyotek.Demo
{
  // Cyotek ImageBox
  // Copyright (c) 2010-2015 Cyotek Ltd.
  // http://cyotek.com
  // http://cyotek.com/blog/tag/imagebox

  // Licensed under the MIT License. See license.txt for the full text.

  // If you use this control in your applications, attribution, donations or contributions are welcome.

  internal partial class DisposedImageDemoForm : BaseForm
  {
    #region Instance Fields

    private Image _image;

    #endregion

    #region Public Constructors

    public DisposedImageDemoForm()
    {
      InitializeComponent();
    }

    #endregion

    #region Overridden Methods

    protected override void OnLoad(EventArgs e)
    {
      _image = new Bitmap(100, 100);
      _image.Dispose();

      imageBox.Image = _image;

      base.OnLoad(e);
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

    #endregion
  }
}
