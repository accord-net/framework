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
using Accord.Math;
using Accord.Imaging;
using Accord.Imaging.Filters;
using Accord;

namespace SampleApp
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

        private Accord.Controls.HuePicker huePicker;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox maxHBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox minSBox;
        private System.Windows.Forms.TextBox maxSBox;
        private Accord.Controls.ColorSlider saturationSlider;
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
        private Accord.Controls.ColorSlider luminanceSlider;
        private TableLayoutPanel tableLayoutPanel1;
        private FlowLayoutPanel flowLayoutPanel2;
        private Panel panel1;
        private FlowLayoutPanel flowLayoutPanel1;

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
            this.huePicker = new Accord.Controls.HuePicker();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.maxHBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.minHBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.saturationSlider = new Accord.Controls.ColorSlider();
            this.maxSBox = new System.Windows.Forms.TextBox();
            this.minSBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.luminanceSlider = new Accord.Controls.ColorSlider();
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.filterPreview = new IPLab.FilterPreview();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // huePicker
            // 
            this.huePicker.Location = new System.Drawing.Point(85, 73);
            this.huePicker.Name = "huePicker";
            this.huePicker.Size = new System.Drawing.Size(272, 249);
            this.huePicker.TabIndex = 0;
            this.huePicker.Type = Accord.Controls.HuePicker.HuePickerType.Range;
            this.huePicker.ValuesChanged += new System.EventHandler(this.huePicker_ValuesChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.maxHBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.minHBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.huePicker);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.MinimumSize = new System.Drawing.Size(448, 336);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(448, 336);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Hue";
            // 
            // maxHBox
            // 
            this.maxHBox.Location = new System.Drawing.Point(349, 29);
            this.maxHBox.Name = "maxHBox";
            this.maxHBox.Size = new System.Drawing.Size(80, 26);
            this.maxHBox.TabIndex = 4;
            this.maxHBox.TextChanged += new System.EventHandler(this.maxHBox_TextChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(298, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 22);
            this.label2.TabIndex = 3;
            this.label2.Text = "Max:";
            // 
            // minHBox
            // 
            this.minHBox.Location = new System.Drawing.Point(64, 29);
            this.minHBox.Name = "minHBox";
            this.minHBox.Size = new System.Drawing.Size(80, 26);
            this.minHBox.TabIndex = 2;
            this.minHBox.TextChanged += new System.EventHandler(this.minHBox_TextChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 24);
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
            this.groupBox2.Location = new System.Drawing.Point(3, 345);
            this.groupBox2.MinimumSize = new System.Drawing.Size(448, 110);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(448, 110);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Saturation";
            // 
            // saturationSlider
            // 
            this.saturationSlider.Location = new System.Drawing.Point(13, 66);
            this.saturationSlider.Name = "saturationSlider";
            this.saturationSlider.Size = new System.Drawing.Size(419, 33);
            this.saturationSlider.TabIndex = 4;
            this.saturationSlider.Type = Accord.Controls.ColorSlider.ColorSliderType.InnerGradient;
            this.saturationSlider.ValuesChanged += new System.EventHandler(this.saturationSlider_ValuesChanged);
            // 
            // maxSBox
            // 
            this.maxSBox.Location = new System.Drawing.Point(349, 29);
            this.maxSBox.Name = "maxSBox";
            this.maxSBox.Size = new System.Drawing.Size(80, 26);
            this.maxSBox.TabIndex = 3;
            this.maxSBox.TextChanged += new System.EventHandler(this.maxSBox_TextChanged);
            // 
            // minSBox
            // 
            this.minSBox.Location = new System.Drawing.Point(64, 29);
            this.minSBox.Name = "minSBox";
            this.minSBox.Size = new System.Drawing.Size(80, 26);
            this.minSBox.TabIndex = 2;
            this.minSBox.TextChanged += new System.EventHandler(this.minSBox_TextChanged);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(298, 34);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 24);
            this.label4.TabIndex = 1;
            this.label4.Text = "Max:";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(16, 34);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 23);
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
            this.groupBox3.Location = new System.Drawing.Point(3, 461);
            this.groupBox3.MinimumSize = new System.Drawing.Size(448, 110);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(448, 110);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Luminance";
            // 
            // luminanceSlider
            // 
            this.luminanceSlider.Location = new System.Drawing.Point(13, 66);
            this.luminanceSlider.Name = "luminanceSlider";
            this.luminanceSlider.Size = new System.Drawing.Size(419, 33);
            this.luminanceSlider.TabIndex = 9;
            this.luminanceSlider.Type = Accord.Controls.ColorSlider.ColorSliderType.InnerGradient;
            this.luminanceSlider.ValuesChanged += new System.EventHandler(this.luminanceSlider_ValuesChanged);
            // 
            // maxLBox
            // 
            this.maxLBox.Location = new System.Drawing.Point(349, 29);
            this.maxLBox.Name = "maxLBox";
            this.maxLBox.Size = new System.Drawing.Size(80, 26);
            this.maxLBox.TabIndex = 8;
            this.maxLBox.TextChanged += new System.EventHandler(this.maxLBox_TextChanged);
            // 
            // minLBox
            // 
            this.minLBox.Location = new System.Drawing.Point(64, 29);
            this.minLBox.Name = "minLBox";
            this.minLBox.Size = new System.Drawing.Size(80, 26);
            this.minLBox.TabIndex = 7;
            this.minLBox.TextChanged += new System.EventHandler(this.minLBox_TextChanged);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(298, 34);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 24);
            this.label5.TabIndex = 6;
            this.label5.Text = "Max:";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(16, 34);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(48, 23);
            this.label6.TabIndex = 5;
            this.label6.Text = "Min:";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.filterPreview);
            this.groupBox5.Location = new System.Drawing.Point(3, 3);
            this.groupBox5.MinimumSize = new System.Drawing.Size(272, 255);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(272, 255);
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
            this.groupBox4.Location = new System.Drawing.Point(3, 264);
            this.groupBox4.MinimumSize = new System.Drawing.Size(272, 146);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(272, 146);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Fill Color";
            // 
            // updateLCheck
            // 
            this.updateLCheck.Checked = true;
            this.updateLCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.updateLCheck.Location = new System.Drawing.Point(200, 102);
            this.updateLCheck.Name = "updateLCheck";
            this.updateLCheck.Size = new System.Drawing.Size(22, 35);
            this.updateLCheck.TabIndex = 8;
            this.updateLCheck.CheckedChanged += new System.EventHandler(this.updateLCheck_CheckedChanged);
            // 
            // fillLBox
            // 
            this.fillLBox.Location = new System.Drawing.Point(64, 102);
            this.fillLBox.Name = "fillLBox";
            this.fillLBox.Size = new System.Drawing.Size(80, 26);
            this.fillLBox.TabIndex = 7;
            this.fillLBox.TextChanged += new System.EventHandler(this.fillLBox_TextChanged);
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(16, 107);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(32, 23);
            this.label9.TabIndex = 6;
            this.label9.Text = "L:";
            // 
            // updateSCheck
            // 
            this.updateSCheck.Checked = true;
            this.updateSCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.updateSCheck.Location = new System.Drawing.Point(200, 66);
            this.updateSCheck.Name = "updateSCheck";
            this.updateSCheck.Size = new System.Drawing.Size(22, 35);
            this.updateSCheck.TabIndex = 5;
            this.updateSCheck.CheckedChanged += new System.EventHandler(this.updateSCheck_CheckedChanged);
            // 
            // fillSBox
            // 
            this.fillSBox.Location = new System.Drawing.Point(64, 66);
            this.fillSBox.Name = "fillSBox";
            this.fillSBox.Size = new System.Drawing.Size(80, 26);
            this.fillSBox.TabIndex = 4;
            this.fillSBox.TextChanged += new System.EventHandler(this.fillSBox_TextChanged);
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(16, 70);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(32, 24);
            this.label8.TabIndex = 3;
            this.label8.Text = "S:";
            // 
            // updateHCheck
            // 
            this.updateHCheck.Checked = true;
            this.updateHCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.updateHCheck.Location = new System.Drawing.Point(200, 29);
            this.updateHCheck.Name = "updateHCheck";
            this.updateHCheck.Size = new System.Drawing.Size(22, 35);
            this.updateHCheck.TabIndex = 2;
            this.updateHCheck.CheckedChanged += new System.EventHandler(this.updateHCheck_CheckedChanged);
            // 
            // fillHBox
            // 
            this.fillHBox.Location = new System.Drawing.Point(64, 29);
            this.fillHBox.Name = "fillHBox";
            this.fillHBox.Size = new System.Drawing.Size(80, 26);
            this.fillHBox.TabIndex = 1;
            this.fillHBox.TextChanged += new System.EventHandler(this.fillHBox_TextChanged);
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(16, 34);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(32, 23);
            this.label7.TabIndex = 0;
            this.label7.Text = "H:";
            // 
            // fillTypeCombo
            // 
            this.fillTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fillTypeCombo.Items.AddRange(new object[] {
            "Outside",
            "Inside"});
            this.fillTypeCombo.Location = new System.Drawing.Point(85, 8);
            this.fillTypeCombo.Name = "fillTypeCombo";
            this.fillTypeCombo.Size = new System.Drawing.Size(192, 28);
            this.fillTypeCombo.TabIndex = 10;
            this.fillTypeCombo.SelectedIndexChanged += new System.EventHandler(this.fillTypeCombo_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(5, 13);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(80, 20);
            this.label10.TabIndex = 13;
            this.label10.Text = "Fill type:";
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.okButton.Location = new System.Drawing.Point(467, 550);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(286, 34);
            this.okButton.TabIndex = 11;
            this.okButton.Text = "Close";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.okButton, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(756, 587);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.groupBox1);
            this.flowLayoutPanel1.Controls.Add(this.groupBox2);
            this.flowLayoutPanel1.Controls.Add(this.groupBox3);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.tableLayoutPanel1.SetRowSpan(this.flowLayoutPanel1, 2);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(458, 581);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.Controls.Add(this.groupBox5);
            this.flowLayoutPanel2.Controls.Add(this.groupBox4);
            this.flowLayoutPanel2.Controls.Add(this.panel1);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(467, 3);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(286, 458);
            this.flowLayoutPanel2.TabIndex = 14;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.fillTypeCombo);
            this.panel1.Location = new System.Drawing.Point(3, 416);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(280, 39);
            this.panel1.TabIndex = 15;
            // 
            // filterPreview
            // 
            this.filterPreview.Image = null;
            this.filterPreview.Location = new System.Drawing.Point(16, 22);
            this.filterPreview.Name = "filterPreview";
            this.filterPreview.Size = new System.Drawing.Size(240, 219);
            this.filterPreview.TabIndex = 0;
            this.filterPreview.TabStop = false;
            // 
            // HSLFilteringForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(756, 587);
            this.Controls.Add(this.tableLayoutPanel1);
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
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.panel1.ResumeLayout(false);
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
            Accord.Controls.ColorSlider.ColorSliderType[] types =
                new Accord.Controls.ColorSlider.ColorSliderType[]
                {
                    Accord.Controls.ColorSlider.ColorSliderType.InnerGradient,
                    Accord.Controls.ColorSlider.ColorSliderType.OuterGradient
                };
            Accord.Controls.ColorSlider.ColorSliderType type = types[fillTypeCombo.SelectedIndex];

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
