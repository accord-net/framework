// Time Series Prediction using Genetic Programming and Gene Expression Programming
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2006-2009
// andrew.kirillov@aforgenet.com

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace TimeSeries
{
	/// <summary>
	/// Summary description for ExSettingsDialog.
	/// </summary>
	public class ExSettingsDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox maxInitialLevelBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox maxLevelBox;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox headLengthBox;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private int maxInitialTreeLevel = 3;
		private int maxTreeLevel = 5;
		private int headLength = 20;

		// Max initial tree level property
		public int MaxInitialTreeLevel
		{
			get { return maxInitialTreeLevel; }
			set { maxInitialTreeLevel = value; }
		}
		// Max tree level property
		public int MaxTreeLevel
		{
			get { return maxTreeLevel; }
			set { maxTreeLevel = value; }
		}
		// Head length property
		public int HeadLength
		{
			get { return headLength; }
			set { headLength = value; }
		}


		// Constructor
		public ExSettingsDialog( )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.maxLevelBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.maxInitialLevelBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.headLengthBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.maxLevelBox,
																					this.label2,
																					this.maxInitialLevelBox,
																					this.label1});
			this.groupBox1.Location = new System.Drawing.Point(10, 10);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(210, 80);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "GP Settings";
			// 
			// maxLevelBox
			// 
			this.maxLevelBox.Location = new System.Drawing.Point(145, 50);
			this.maxLevelBox.Name = "maxLevelBox";
			this.maxLevelBox.Size = new System.Drawing.Size(50, 20);
			this.maxLevelBox.TabIndex = 3;
			this.maxLevelBox.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(10, 52);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 16);
			this.label2.TabIndex = 2;
			this.label2.Text = "Max tree level:";
			// 
			// maxInitialLevelBox
			// 
			this.maxInitialLevelBox.Location = new System.Drawing.Point(145, 20);
			this.maxInitialLevelBox.Name = "maxInitialLevelBox";
			this.maxInitialLevelBox.Size = new System.Drawing.Size(50, 20);
			this.maxInitialLevelBox.TabIndex = 1;
			this.maxInitialLevelBox.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(10, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(135, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Maximum initial tree level:";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.headLengthBox,
																					this.label3});
			this.groupBox2.Location = new System.Drawing.Point(10, 100);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(210, 50);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "GEP Settings";
			// 
			// headLengthBox
			// 
			this.headLengthBox.Location = new System.Drawing.Point(145, 20);
			this.headLengthBox.Name = "headLengthBox";
			this.headLengthBox.Size = new System.Drawing.Size(50, 20);
			this.headLengthBox.TabIndex = 1;
			this.headLengthBox.Text = "";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(10, 22);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 16);
			this.label3.TabIndex = 0;
			this.label3.Text = "Head length:";
			// 
			// okButton
			// 
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.okButton.Location = new System.Drawing.Point(60, 160);
			this.okButton.Name = "okButton";
			this.okButton.TabIndex = 2;
			this.okButton.Text = "&Ok";
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.cancelButton.Location = new System.Drawing.Point(145, 160);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.TabIndex = 3;
			this.cancelButton.Text = "&Cancel";
			// 
			// ExSettingsDialog
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(234, 193);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.cancelButton,
																		  this.okButton,
																		  this.groupBox2,
																		  this.groupBox1});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ExSettingsDialog";
			this.Opacity = 0.800000011920929;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Extended Settings";
			this.Load += new System.EventHandler(this.ExSettingsDialog_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		// On form load
		private void ExSettingsDialog_Load(object sender, System.EventArgs e)
		{
			maxInitialLevelBox.Text	= maxInitialTreeLevel.ToString( );
			maxLevelBox.Text		= maxTreeLevel.ToString( );
			headLengthBox.Text		= headLength.ToString( );
		}

		// On "Ok" button click
		private void okButton_Click(object sender, System.EventArgs e)
		{
			// max initial tree level
			try
			{
				maxInitialTreeLevel = Math.Max( 1, Math.Min( 7, int.Parse( maxInitialLevelBox.Text ) ) );
			}
			catch
			{
				maxInitialTreeLevel = 3;
			}
			// max tree level
			try
			{
				maxTreeLevel = Math.Max( 2, Math.Min( 9, int.Parse( maxLevelBox.Text ) ) );
			}
			catch
			{
				maxTreeLevel = 5;
			}
			// head length
			try
			{
				headLength = Math.Max( 3, Math.Min( 50, int.Parse( headLengthBox.Text ) ) );
			}
			catch
			{
				headLength = 20;
			}
		}
	}
}
