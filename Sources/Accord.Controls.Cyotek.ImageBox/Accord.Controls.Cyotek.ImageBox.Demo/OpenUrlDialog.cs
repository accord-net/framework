using System;
using System.ComponentModel;
using System.Windows.Forms;

// Cyotek ImageBox
// Copyright (c) 2010-2017 Cyotek Ltd.
// http://cyotek.com
// http://cyotek.com/blog/tag/imagebox

// Licensed under the MIT License. See license.txt for the full text.

// If you use this control in your applications, attribution, donations or contributions are welcome.

namespace Accord.Controls.Cyotek.Demo
{
  internal sealed partial class OpenUrlDialog : BaseForm
  {
    #region Constructors

    public OpenUrlDialog()
    {
      this.InitializeComponent();
    }

    #endregion

    #region Properties

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Url { get; set; }

    #endregion

    #region Methods

    private void cancelButton_Click(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.Cancel;
      this.Close();
    }

    private void okButton_Click(object sender, EventArgs e)
    {
      string url;

      url = urlTextBox.Text;

      if (this.ValidateUrl(url))
      {
        this.DialogResult = DialogResult.OK;
        this.Url = url;
        this.Close();
      }
      else
      {
        this.DialogResult = DialogResult.None;
      }
    }

    private bool ValidateUrl(string url)
    {
      Uri uri;
      bool result;

      result = !string.IsNullOrEmpty(url) && Uri.TryCreate(url, UriKind.Absolute, out uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

      return result;
    }

    #endregion
  }
}
