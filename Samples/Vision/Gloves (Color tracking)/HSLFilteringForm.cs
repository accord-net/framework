// Image Processing Lab
// http://www.aforgenet.com/projects/iplab/
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
//

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using AForge;
using AForge.Math;
using AForge.Imaging;
using AForge.Imaging.Filters;

namespace GloveTracking
{
    /// <summary>
    /// Summary description for HSLFilteringForm.
    /// </summary>
    public class HSLFilteringForm : System.Windows.Forms.Form
    {
        private HSLFiltering filter = new HSLFiltering( );
        private IntRange hue = new IntRange( 0, 359 );
        private Range saturation = new Range( 0, 1 );
        private Range luminance = new Range( 0, 1 );
        private int fillH = 0;
        private float fillS = 0, fillL = 0;

        private AForge.Controls.HuePicker huePicker;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox maxHBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox minSBox;
        private System.Windows.Forms.TextBox maxSBox;
        private AForge.Controls.ColorSlider saturationSlider;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox minLBox;
        private System.Windows.Forms.TextBox maxLBox;
        private System.Windows.Forms.GroupBox groupBox5;
        private IPLab.FilterPreview filterPreview;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox fillHBox;
        private System.Windows.Forms.CheckBox updateHCheck;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox fillSBox;
        private System.Windows.Forms.CheckBox updateSCheck;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox fillLBox;
        private System.Windows.Forms.CheckBox updateLCheck;
        private System.Windows.Forms.ComboBox fillTypeCombo;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.TextBox minHBox;
        private AForge.Controls.ColorSlider luminanceSlider;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        // Image property
        public Bitmap Image
        {
            set { filterPreview.Image = value; }
        }

        // Filter property
        public HSLFiltering Filter
        {
            get { return filter; }
            set
            {
                filter = value;
                hue = filter.Hue;
                saturation = filter.Saturation;
                luminance = filter.Luminance;

                minHBox.Text = hue.Min.ToString();
                maxHBox.Text = hue.Max.ToString();

                minSBox.Text = saturation.Min.ToString("F3");
                maxSBox.Text = saturation.Max.ToString("F3");

                minLBox.Text = luminance.Min.ToString("F3");
                maxLBox.Text = luminance.Max.ToString("F3");

                filterPreview.Filter = filter;
            }
        }

        // Constructor
        public HSLFilteringForm( )
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent( );

            //
            minHBox.Text = hue.Min.ToString( );
            maxHBox.Text = hue.Max.ToString( );
            fillHBox.Text = fillH.ToString( );

            minSBox.Text = saturation.Min.ToString( "F3" );
            maxSBox.Text = saturation.Max.ToString( "F3" );
            fillSBox.Text = fillS.ToString( "F3" );

            minLBox.Text = luminance.Min.ToString( "F3" );
            maxLBox.Text = luminance.Max.ToString( "F3" );
            fillLBox.Text = fillL.ToString( "F3" );

            fillTypeCombo.SelectedIndex = 0;

            filterPreview.Filter = filter;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( components != null )
                {
                    components.Dispose( );
                }
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent( )
        {
            this.huePicker = new AForge.Controls.HuePicker();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.maxHBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.minHBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.saturationSlider = new AForge.Controls.ColorSlider();
            this.maxSBox = new System.Windows.Forms.TextBox();
            this.minSBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.luminanceSlider = new AForge.Controls.ColorSlider();
            this.maxLBox = new System.Windows.Forms.TextBox();
            this.minLBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.updateLCheck = new System.Windows.Forms.CheckBox();
            this.fillLBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.updateSCheck = new System.Windows.Forms.CheckBox();
            this.fillSBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.updateHCheck = new System.Windows.Forms.CheckBox();
            this.fillHBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.fillTypeCombo = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.filterPreview = new IPLab.FilterPreview();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // huePicker
            // 
            this.huePicker.Location = new System.Drawing.Point(53, 50);
            this.huePicker.Name = "huePicker";
            this.huePicker.Size = new System.Drawing.Size(170, 170);
            this.huePicker.TabIndex = 0;
            this.huePicker.Type = AForge.Controls.HuePicker.HuePickerType.Range;
            this.huePicker.ValuesChanged += new System.EventHandler(this.huePicker_ValuesChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.maxHBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.minHBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.huePicker);
            this.groupBox1.Location = new System.Drawing.Point(10, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(280, 230);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Hue";
            // 
            // maxHBox
            // 
            this.maxHBox.Location = new System.Drawing.Point(218, 20);
            this.maxHBox.Name = "maxHBox";
            this.maxHBox.Size = new System.Drawing.Size(50, 20);
            this.maxHBox.TabIndex = 4;
            this.maxHBox.TextChanged += new System.EventHandler(this.maxHBox_TextChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(186, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Max:";
            // 
            // minHBox
            // 
            this.minHBox.Location = new System.Drawing.Point(40, 20);
            this.minHBox.Name = "minHBox";
            this.minHBox.Size = new System.Drawing.Size(50, 20);
            this.minHBox.TabIndex = 2;
            this.minHBox.TextChanged += new System.EventHandler(this.minHBox_TextChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(10, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Min:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.saturationSlider);
            this.groupBox2.Controls.Add(this.maxSBox);
            this.groupBox2.Controls.Add(this.minSBox);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(10, 245);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(280, 75);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Saturation";
            // 
            // saturationSlider
            // 
            this.saturationSlider.Location = new System.Drawing.Point(8, 45);
            this.saturationSlider.Name = "saturationSlider";
            this.saturationSlider.Size = new System.Drawing.Size(262, 23);
            this.saturationSlider.TabIndex = 4;
            this.saturationSlider.Type = AForge.Controls.ColorSlider.ColorSliderType.InnerGradient;
            this.saturationSlider.ValuesChanged += new System.EventHandler(this.saturationSlider_ValuesChanged);
            // 
            // maxSBox
            // 
            this.maxSBox.Location = new System.Drawing.Point(218, 20);
            this.maxSBox.Name = "maxSBox";
            this.maxSBox.Size = new System.Drawing.Size(50, 20);
            this.maxSBox.TabIndex = 3;
            this.maxSBox.TextChanged += new System.EventHandler(this.maxSBox_TextChanged);
            // 
            // minSBox
            // 
            this.minSBox.Location = new System.Drawing.Point(40, 20);
            this.minSBox.Name = "minSBox";
            this.minSBox.Size = new System.Drawing.Size(50, 20);
            this.minSBox.TabIndex = 2;
            this.minSBox.TextChanged += new System.EventHandler(this.minSBox_TextChanged);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(186, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 17);
            this.label4.TabIndex = 1;
            this.label4.Text = "Max:";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(10, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 16);
            this.label3.TabIndex = 0;
            this.label3.Text = "Min:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.luminanceSlider);
            this.groupBox3.Controls.Add(this.maxLBox);
            this.groupBox3.Controls.Add(this.minLBox);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Location = new System.Drawing.Point(10, 325);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(280, 75);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Luminance";
            // 
            // luminanceSlider
            // 
            this.luminanceSlider.Location = new System.Drawing.Point(8, 45);
            this.luminanceSlider.Name = "luminanceSlider";
            this.luminanceSlider.Size = new System.Drawing.Size(262, 23);
            this.luminanceSlider.TabIndex = 9;
            this.luminanceSlider.Type = AForge.Controls.ColorSlider.ColorSliderType.InnerGradient;
            this.luminanceSlider.ValuesChanged += new System.EventHandler(this.luminanceSlider_ValuesChanged);
            // 
            // maxLBox
            // 
            this.maxLBox.Location = new System.Drawing.Point(218, 20);
            this.maxLBox.Name = "maxLBox";
            this.maxLBox.Size = new System.Drawing.Size(50, 20);
            this.maxLBox.TabIndex = 8;
            this.maxLBox.TextChanged += new System.EventHandler(this.maxLBox_TextChanged);
            // 
            // minLBox
            // 
            this.minLBox.Location = new System.Drawing.Point(40, 20);
            this.minLBox.Name = "minLBox";
            this.minLBox.Size = new System.Drawing.Size(50, 20);
            this.minLBox.TabIndex = 7;
            this.minLBox.TextChanged += new System.EventHandler(this.minLBox_TextChanged);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(186, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(30, 17);
            this.label5.TabIndex = 6;
            this.label5.Text = "Max:";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(10, 23);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(30, 16);
            this.label6.TabIndex = 5;
            this.label6.Text = "Min:";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.filterPreview);
            this.groupBox5.Location = new System.Drawing.Point(300, 10);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(170, 175);
            this.groupBox5.TabIndex = 4;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Preview";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.updateLCheck);
            this.groupBox4.Controls.Add(this.fillLBox);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.updateSCheck);
            this.groupBox4.Controls.Add(this.fillSBox);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.updateHCheck);
            this.groupBox4.Controls.Add(this.fillHBox);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Location = new System.Drawing.Point(300, 190);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(170, 100);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Fill Color";
            // 
            // updateLCheck
            // 
            this.updateLCheck.Checked = true;
            this.updateLCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.updateLCheck.Location = new System.Drawing.Point(125, 70);
            this.updateLCheck.Name = "updateLCheck";
            this.updateLCheck.Size = new System.Drawing.Size(14, 24);
            this.updateLCheck.TabIndex = 8;
            this.updateLCheck.CheckedChanged += new System.EventHandler(this.updateLCheck_CheckedChanged);
            // 
            // fillLBox
            // 
            this.fillLBox.Location = new System.Drawing.Point(40, 70);
            this.fillLBox.Name = "fillLBox";
            this.fillLBox.Size = new System.Drawing.Size(50, 20);
            this.fillLBox.TabIndex = 7;
            this.fillLBox.TextChanged += new System.EventHandler(this.fillLBox_TextChanged);
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(10, 73);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(20, 16);
            this.label9.TabIndex = 6;
            this.label9.Text = "L:";
            // 
            // updateSCheck
            // 
            this.updateSCheck.Checked = true;
            this.updateSCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.updateSCheck.Location = new System.Drawing.Point(125, 45);
            this.updateSCheck.Name = "updateSCheck";
            this.updateSCheck.Size = new System.Drawing.Size(14, 24);
            this.updateSCheck.TabIndex = 5;
            this.updateSCheck.CheckedChanged += new System.EventHandler(this.updateSCheck_CheckedChanged);
            // 
            // fillSBox
            // 
            this.fillSBox.Location = new System.Drawing.Point(40, 45);
            this.fillSBox.Name = "fillSBox";
            this.fillSBox.Size = new System.Drawing.Size(50, 20);
            this.fillSBox.TabIndex = 4;
            this.fillSBox.TextChanged += new System.EventHandler(this.fillSBox_TextChanged);
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(10, 48);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(20, 16);
            this.label8.TabIndex = 3;
            this.label8.Text = "S:";
            // 
            // updateHCheck
            // 
            this.updateHCheck.Checked = true;
            this.updateHCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.updateHCheck.Location = new System.Drawing.Point(125, 20);
            this.updateHCheck.Name = "updateHCheck";
            this.updateHCheck.Size = new System.Drawing.Size(14, 24);
            this.updateHCheck.TabIndex = 2;
            this.updateHCheck.CheckedChanged += new System.EventHandler(this.updateHCheck_CheckedChanged);
            // 
            // fillHBox
            // 
            this.fillHBox.Location = new System.Drawing.Point(40, 20);
            this.fillHBox.Name = "fillHBox";
            this.fillHBox.Size = new System.Drawing.Size(50, 20);
            this.fillHBox.TabIndex = 1;
            this.fillHBox.TextChanged += new System.EventHandler(this.fillHBox_TextChanged);
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(10, 23);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(20, 16);
            this.label7.TabIndex = 0;
            this.label7.Text = "H:";
            // 
            // fillTypeCombo
            // 
            this.fillTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fillTypeCombo.Items.AddRange(new object[] {
            "Outside",
            "Inside"});
            this.fillTypeCombo.Location = new System.Drawing.Point(350, 300);
            this.fillTypeCombo.Name = "fillTypeCombo";
            this.fillTypeCombo.Size = new System.Drawing.Size(120, 21);
            this.fillTypeCombo.TabIndex = 10;
            this.fillTypeCombo.SelectedIndexChanged += new System.EventHandler(this.fillTypeCombo_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(300, 303);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(50, 14);
            this.label10.TabIndex = 13;
            this.label10.Text = "Fill type:";
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.okButton.Location = new System.Drawing.Point(310, 378);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(157, 23);
            this.okButton.TabIndex = 11;
            this.okButton.Text = "Close";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // filterPreview
            // 
            this.filterPreview.Image = null;
            this.filterPreview.Location = new System.Drawing.Point(10, 15);
            this.filterPreview.Name = "filterPreview";
            this.filterPreview.Size = new System.Drawing.Size(150, 150);
            this.filterPreview.TabIndex = 0;
            this.filterPreview.TabStop = false;
            // 
            // HSLFilteringForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(479, 408);
            this.Controls.Add(this.fillTypeCombo);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HSLFilteringForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HSL Filtering";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        // Update filter
        private void UpdateFilter( )
        {
            filter.Hue = hue;
            filter.Saturation = saturation;
            filter.Luminance = luminance;
            filterPreview.RefreshFilter( );
        }

        // Min hue changed
        private void minHBox_TextChanged( object sender, System.EventArgs e )
        {
            try
            {
                huePicker.Min = hue.Min = Math.Max( 0, Math.Min( 359, int.Parse( minHBox.Text ) ) );
                UpdateFilter( );
            }
            catch ( Exception )
            {
            }
        }

        // Max hue changed
        private void maxHBox_TextChanged( object sender, System.EventArgs e )
        {
            try
            {
                huePicker.Max = hue.Max = Math.Max( 0, Math.Min( 359, int.Parse( maxHBox.Text ) ) );
                UpdateFilter( );
            }
            catch ( Exception )
            {
            }
        }

        // Min saturation changed
        private void minSBox_TextChanged( object sender, System.EventArgs e )
        {
            try
            {
                saturation.Min = float.Parse( minSBox.Text );
                saturationSlider.Min = (int) ( saturation.Min * 255 );
                UpdateFilter( );
            }
            catch ( Exception )
            {
            }
        }

        // Max saturation changed
        private void maxSBox_TextChanged( object sender, System.EventArgs e )
        {
            try
            {
                saturation.Max = float.Parse( maxSBox.Text );
                saturationSlider.Max = (int) ( saturation.Max * 255 );
                UpdateFilter( );
            }
            catch ( Exception )
            {
            }
        }

        // Min luminance changed
        private void minLBox_TextChanged( object sender, System.EventArgs e )
        {
            try
            {
                luminance.Min = float.Parse( minLBox.Text );
                luminanceSlider.Min = (int) ( luminance.Min * 255 );
                UpdateFilter( );
            }
            catch ( Exception )
            {
            }
        }

        // Max luminance changed
        private void maxLBox_TextChanged( object sender, System.EventArgs e )
        {
            try
            {
                luminance.Max = float.Parse( maxLBox.Text );
                luminanceSlider.Max = (int) ( luminance.Max * 255 );
                UpdateFilter( );
            }
            catch ( Exception )
            {
            }
        }

        // Hue picker changed
        private void huePicker_ValuesChanged( object sender, System.EventArgs e )
        {
            minHBox.Text = huePicker.Min.ToString( );
            maxHBox.Text = huePicker.Max.ToString( );
        }

        // Saturation slider changed
        private void saturationSlider_ValuesChanged( object sender, System.EventArgs e )
        {
            minSBox.Text = ( (double) saturationSlider.Min / 255 ).ToString( "F3" );
            maxSBox.Text = ( (double) saturationSlider.Max / 255 ).ToString( "F3" );
        }

        // Luminance slider changed
        private void luminanceSlider_ValuesChanged( object sender, System.EventArgs e )
        {
            minLBox.Text = ( (double) luminanceSlider.Min / 255 ).ToString( "F3" );
            maxLBox.Text = ( (double) luminanceSlider.Max / 255 ).ToString( "F3" );
        }

        // Fill hue changed
        private void fillHBox_TextChanged( object sender, System.EventArgs e )
        {
            try
            {
                fillH = int.Parse( fillHBox.Text );
                UpdateFillColor( );
            }
            catch ( Exception )
            {
            }
        }

        // Fill saturation changed
        private void fillSBox_TextChanged( object sender, System.EventArgs e )
        {
            try
            {
                fillS = float.Parse( fillSBox.Text );
                UpdateFillColor( );
            }
            catch ( Exception )
            {
            }
        }

        // Fill luminance changed
        private void fillLBox_TextChanged( object sender, System.EventArgs e )
        {
            try
            {
                fillL = float.Parse( fillLBox.Text );
                UpdateFillColor( );
            }
            catch ( Exception )
            {
            }
        }

        // Update fil color
        private void UpdateFillColor( )
        {
            int v;

            v = (int) ( fillS * 255 );
            saturationSlider.FillColor = Color.FromArgb( v, v, v );
            v = (int) ( fillL * 255 );
            luminanceSlider.FillColor = Color.FromArgb( v, v, v );


            filter.FillColor = new HSL( fillH, fillS, fillL );
            filterPreview.RefreshFilter( );
        }

        // Update Hue check clicked
        private void updateHCheck_CheckedChanged( object sender, System.EventArgs e )
        {
            filter.UpdateHue = updateHCheck.Checked;
            filterPreview.RefreshFilter( );
        }

        // Update Saturation check clicked
        private void updateSCheck_CheckedChanged( object sender, System.EventArgs e )
        {
            filter.UpdateSaturation = updateSCheck.Checked;
            filterPreview.RefreshFilter( );
        }

        // Update Luminance check clicked
        private void updateLCheck_CheckedChanged( object sender, System.EventArgs e )
        {
            filter.UpdateLuminance = updateLCheck.Checked;
            filterPreview.RefreshFilter( );
        }

        // Fill type changed
        private void fillTypeCombo_SelectedIndexChanged( object sender, System.EventArgs e )
        {
            AForge.Controls.ColorSlider.ColorSliderType[] types =
                new AForge.Controls.ColorSlider.ColorSliderType[]
                {
                    AForge.Controls.ColorSlider.ColorSliderType.InnerGradient,
                    AForge.Controls.ColorSlider.ColorSliderType.OuterGradient
                };
            AForge.Controls.ColorSlider.ColorSliderType type = types[fillTypeCombo.SelectedIndex];

            saturationSlider.Type = type;
            luminanceSlider.Type = type;

            filter.FillOutsideRange = ( fillTypeCombo.SelectedIndex == 0 );
            filterPreview.RefreshFilter( );
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
