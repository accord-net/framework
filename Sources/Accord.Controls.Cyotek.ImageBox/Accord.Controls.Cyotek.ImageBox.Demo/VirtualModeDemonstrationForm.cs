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

  internal partial class VirtualModeDemonstrationForm : BaseForm
  {
    #region Public Constructors

    public VirtualModeDemonstrationForm()
    {
      this.InitializeComponent();
    }

    #endregion

    #region Overridden Methods

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      propertyGrid.SelectItem("VirtualSize");
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

    private void imageBox_VirtualDraw(object sender, PaintEventArgs e)
    {
      RectangleF bounds;

      bounds = imageBox.GetOffsetRectangle(new RectangleF(PointF.Empty, imageBox.VirtualSize));

      using (Brush brush = new SolidBrush(Color.FromArgb(128, Color.Goldenrod)))
      {
        e.Graphics.FillRectangle(brush, bounds);
      }

      e.Graphics.DrawRectangle(Pens.DarkGoldenrod, bounds.X, bounds.Y, bounds.Width, bounds.Height);

      using (Font font = new Font(this.Font.FontFamily, (float)(8 * imageBox.ZoomFactor)))
      {
        TextRenderer.DrawText(e.Graphics, "Use the VirtualMode and VirtualSize properties along with the VirtualDraw event to provide full control without needing a backing image.", font, new Rectangle((int)bounds.Left, (int)bounds.Top, (int)bounds.Width, (int)bounds.Height), Color.Black, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak | TextFormatFlags.WordEllipsis);
      }
    }

    #endregion
  }
}
