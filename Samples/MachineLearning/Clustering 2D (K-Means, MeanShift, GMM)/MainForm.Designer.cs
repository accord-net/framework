namespace SampleApp
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabSamples = new System.Windows.Forms.TabPage();
            this.splitContainer7 = new System.Windows.Forms.SplitContainer();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.dgvLearningSource = new System.Windows.Forms.DataGridView();
            this.groupBox15 = new System.Windows.Forms.GroupBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbBalanced = new System.Windows.Forms.CheckBox();
            this.btnSampleRunAnalysis = new System.Windows.Forms.Button();
            this.rbKMeans = new System.Windows.Forms.RadioButton();
            this.label7 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.numRadius = new System.Windows.Forms.NumericUpDown();
            this.rbGMM = new System.Windows.Forms.RadioButton();
            this.rbMeanShift = new System.Windows.Forms.RadioButton();
            this.numGaussians = new System.Windows.Forms.NumericUpDown();
            this.numKMeans = new System.Windows.Forms.NumericUpDown();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgvTestingSource = new System.Windows.Forms.DataGridView();
            this.btnTestingRun = new System.Windows.Forms.Button();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.dgvPerformance = new System.Windows.Forms.DataGridView();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lbStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.scatterplotView1 = new Accord.Controls.ScatterplotView();
            this.scatterplotView2 = new Accord.Controls.ScatterplotView();
            this.scatterplotView3 = new Accord.Controls.ScatterplotView();
            this.menuStrip1.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabSamples.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer7)).BeginInit();
            this.splitContainer7.Panel1.SuspendLayout();
            this.splitContainer7.Panel2.SuspendLayout();
            this.splitContainer7.SuspendLayout();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLearningSource)).BeginInit();
            this.groupBox15.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRadius)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGaussians)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numKMeans)).BeginInit();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTestingSource)).BeginInit();
            this.groupBox11.SuspendLayout();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPerformance)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem6});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(9, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(1258, 35);
            this.menuStrip1.TabIndex = 13;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.toolStripSeparator3,
            this.toolStripMenuItem5});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(50, 29);
            this.toolStripMenuItem1.Text = "&File";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem2.Image")));
            this.toolStripMenuItem2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.toolStripMenuItem2.Size = new System.Drawing.Size(206, 30);
            this.toolStripMenuItem2.Text = "&Open";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.MenuFileOpen_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(203, 6);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(206, 30);
            this.toolStripMenuItem5.Text = "E&xit";
            this.toolStripMenuItem5.Click += new System.EventHandler(this.toolStripMenuItem5_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem7});
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(61, 29);
            this.toolStripMenuItem6.Text = "&Help";
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(159, 30);
            this.toolStripMenuItem7.Text = "&About...";
            this.toolStripMenuItem7.Click += new System.EventHandler(this.toolStripMenuItem7_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "examples.xls";
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabSamples);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Controls.Add(this.tabPage4);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 35);
            this.tabControl.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1258, 761);
            this.tabControl.TabIndex = 14;
            // 
            // tabSamples
            // 
            this.tabSamples.Controls.Add(this.splitContainer7);
            this.tabSamples.Location = new System.Drawing.Point(4, 29);
            this.tabSamples.Margin = new System.Windows.Forms.Padding(4);
            this.tabSamples.Name = "tabSamples";
            this.tabSamples.Padding = new System.Windows.Forms.Padding(4);
            this.tabSamples.Size = new System.Drawing.Size(1250, 728);
            this.tabSamples.TabIndex = 0;
            this.tabSamples.Text = "Samples (Input)";
            this.tabSamples.UseVisualStyleBackColor = true;
            // 
            // splitContainer7
            // 
            this.splitContainer7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer7.Location = new System.Drawing.Point(4, 4);
            this.splitContainer7.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer7.Name = "splitContainer7";
            // 
            // splitContainer7.Panel1
            // 
            this.splitContainer7.Panel1.Controls.Add(this.groupBox7);
            // 
            // splitContainer7.Panel2
            // 
            this.splitContainer7.Panel2.Controls.Add(this.groupBox15);
            this.splitContainer7.Size = new System.Drawing.Size(1242, 720);
            this.splitContainer7.SplitterDistance = 349;
            this.splitContainer7.SplitterWidth = 6;
            this.splitContainer7.TabIndex = 9;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.dgvLearningSource);
            this.groupBox7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox7.Location = new System.Drawing.Point(0, 0);
            this.groupBox7.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox7.Size = new System.Drawing.Size(349, 720);
            this.groupBox7.TabIndex = 6;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Training data";
            // 
            // dgvLearningSource
            // 
            this.dgvLearningSource.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvLearningSource.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvLearningSource.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvLearningSource.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvLearningSource.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvLearningSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLearningSource.Location = new System.Drawing.Point(4, 23);
            this.dgvLearningSource.Margin = new System.Windows.Forms.Padding(4);
            this.dgvLearningSource.Name = "dgvLearningSource";
            this.dgvLearningSource.Size = new System.Drawing.Size(341, 693);
            this.dgvLearningSource.TabIndex = 5;
            // 
            // groupBox15
            // 
            this.groupBox15.Controls.Add(this.scatterplotView1);
            this.groupBox15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox15.Location = new System.Drawing.Point(0, 0);
            this.groupBox15.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox15.Name = "groupBox15";
            this.groupBox15.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox15.Size = new System.Drawing.Size(887, 720);
            this.groupBox15.TabIndex = 7;
            this.groupBox15.TabStop = false;
            this.groupBox15.Text = "Scatter Plot";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage2.Size = new System.Drawing.Size(1250, 728);
            this.tabPage2.TabIndex = 13;
            this.tabPage2.Text = "Clustering algorithm";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.scatterplotView3);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Location = new System.Drawing.Point(402, 4);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox4.Size = new System.Drawing.Size(844, 720);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Visualization";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbBalanced);
            this.groupBox3.Controls.Add(this.btnSampleRunAnalysis);
            this.groupBox3.Controls.Add(this.rbKMeans);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.numRadius);
            this.groupBox3.Controls.Add(this.rbGMM);
            this.groupBox3.Controls.Add(this.rbMeanShift);
            this.groupBox3.Controls.Add(this.numGaussians);
            this.groupBox3.Controls.Add(this.numKMeans);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox3.Location = new System.Drawing.Point(4, 4);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox3.Size = new System.Drawing.Size(398, 720);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Settings";
            this.groupBox3.Enter += new System.EventHandler(this.groupBox3_Enter);
            // 
            // cbBalanced
            // 
            this.cbBalanced.AutoSize = true;
            this.cbBalanced.Location = new System.Drawing.Point(66, 101);
            this.cbBalanced.Name = "cbBalanced";
            this.cbBalanced.Size = new System.Drawing.Size(102, 24);
            this.cbBalanced.TabIndex = 8;
            this.cbBalanced.Text = "Balanced";
            this.cbBalanced.UseVisualStyleBackColor = true;
            // 
            // btnSampleRunAnalysis
            // 
            this.btnSampleRunAnalysis.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSampleRunAnalysis.Location = new System.Drawing.Point(9, 650);
            this.btnSampleRunAnalysis.Margin = new System.Windows.Forms.Padding(4);
            this.btnSampleRunAnalysis.Name = "btnSampleRunAnalysis";
            this.btnSampleRunAnalysis.Size = new System.Drawing.Size(380, 60);
            this.btnSampleRunAnalysis.TabIndex = 1;
            this.btnSampleRunAnalysis.Text = "Create clustering";
            this.btnSampleRunAnalysis.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnSampleRunAnalysis.UseVisualStyleBackColor = true;
            this.btnSampleRunAnalysis.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // rbKMeans
            // 
            this.rbKMeans.AutoSize = true;
            this.rbKMeans.Checked = true;
            this.rbKMeans.Location = new System.Drawing.Point(28, 28);
            this.rbKMeans.Margin = new System.Windows.Forms.Padding(4);
            this.rbKMeans.Name = "rbKMeans";
            this.rbKMeans.Size = new System.Drawing.Size(97, 24);
            this.rbKMeans.TabIndex = 6;
            this.rbKMeans.TabStop = true;
            this.rbKMeans.Text = "K-Means";
            this.rbKMeans.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(68, 276);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(85, 20);
            this.label7.TabIndex = 3;
            this.label7.Text = "Gaussians";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(68, 68);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "K:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Enabled = false;
            this.label4.Location = new System.Drawing.Point(58, 191);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 20);
            this.label4.TabIndex = 3;
            this.label4.Text = "Radius";
            // 
            // numRadius
            // 
            this.numRadius.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numRadius.DecimalPlaces = 2;
            this.numRadius.Enabled = false;
            this.numRadius.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numRadius.Location = new System.Drawing.Point(161, 188);
            this.numRadius.Margin = new System.Windows.Forms.Padding(4);
            this.numRadius.Name = "numRadius";
            this.numRadius.Size = new System.Drawing.Size(141, 26);
            this.numRadius.TabIndex = 7;
            this.numRadius.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numRadius.Value = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            // 
            // rbGMM
            // 
            this.rbGMM.AutoSize = true;
            this.rbGMM.Location = new System.Drawing.Point(28, 243);
            this.rbGMM.Margin = new System.Windows.Forms.Padding(4);
            this.rbGMM.Name = "rbGMM";
            this.rbGMM.Size = new System.Drawing.Size(157, 24);
            this.rbGMM.TabIndex = 6;
            this.rbGMM.TabStop = true;
            this.rbGMM.Text = "Gaussian Mixture";
            this.rbGMM.UseVisualStyleBackColor = true;
            // 
            // rbMeanShift
            // 
            this.rbMeanShift.AutoSize = true;
            this.rbMeanShift.Enabled = false;
            this.rbMeanShift.Location = new System.Drawing.Point(28, 153);
            this.rbMeanShift.Margin = new System.Windows.Forms.Padding(4);
            this.rbMeanShift.Name = "rbMeanShift";
            this.rbMeanShift.Size = new System.Drawing.Size(112, 24);
            this.rbMeanShift.TabIndex = 6;
            this.rbMeanShift.TabStop = true;
            this.rbMeanShift.Text = "Mean-Shift";
            this.rbMeanShift.UseVisualStyleBackColor = true;
            // 
            // numGaussians
            // 
            this.numGaussians.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numGaussians.Location = new System.Drawing.Point(161, 274);
            this.numGaussians.Margin = new System.Windows.Forms.Padding(4);
            this.numGaussians.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numGaussians.Name = "numGaussians";
            this.numGaussians.Size = new System.Drawing.Size(147, 26);
            this.numGaussians.TabIndex = 7;
            this.numGaussians.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numGaussians.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // numKMeans
            // 
            this.numKMeans.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numKMeans.Location = new System.Drawing.Point(161, 65);
            this.numKMeans.Margin = new System.Windows.Forms.Padding(4);
            this.numKMeans.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numKMeans.Name = "numKMeans";
            this.numKMeans.Size = new System.Drawing.Size(147, 26);
            this.numKMeans.TabIndex = 7;
            this.numKMeans.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numKMeans.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.splitContainer1);
            this.tabPage4.Location = new System.Drawing.Point(4, 29);
            this.tabPage4.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage4.Size = new System.Drawing.Size(1250, 728);
            this.tabPage4.TabIndex = 12;
            this.tabPage4.Text = "Model Testing";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(4, 4);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel1.Controls.Add(this.btnTestingRun);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox11);
            this.splitContainer1.Panel2.Controls.Add(this.groupBox6);
            this.splitContainer1.Size = new System.Drawing.Size(1242, 720);
            this.splitContainer1.SplitterDistance = 395;
            this.splitContainer1.SplitterWidth = 6;
            this.splitContainer1.TabIndex = 6;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dgvTestingSource);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(395, 628);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Testing data";
            // 
            // dgvTestingSource
            // 
            this.dgvTestingSource.AllowUserToAddRows = false;
            this.dgvTestingSource.AllowUserToDeleteRows = false;
            this.dgvTestingSource.AllowUserToResizeRows = false;
            this.dgvTestingSource.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTestingSource.BackgroundColor = System.Drawing.Color.White;
            this.dgvTestingSource.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTestingSource.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvTestingSource.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvTestingSource.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvTestingSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTestingSource.Location = new System.Drawing.Point(4, 23);
            this.dgvTestingSource.Margin = new System.Windows.Forms.Padding(4);
            this.dgvTestingSource.Name = "dgvTestingSource";
            this.dgvTestingSource.ReadOnly = true;
            this.dgvTestingSource.RowHeadersVisible = false;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dgvTestingSource.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvTestingSource.Size = new System.Drawing.Size(387, 601);
            this.dgvTestingSource.TabIndex = 1;
            // 
            // btnTestingRun
            // 
            this.btnTestingRun.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnTestingRun.Location = new System.Drawing.Point(0, 628);
            this.btnTestingRun.Margin = new System.Windows.Forms.Padding(4);
            this.btnTestingRun.Name = "btnTestingRun";
            this.btnTestingRun.Size = new System.Drawing.Size(395, 92);
            this.btnTestingRun.TabIndex = 3;
            this.btnTestingRun.Text = "Run";
            this.btnTestingRun.UseVisualStyleBackColor = true;
            this.btnTestingRun.Click += new System.EventHandler(this.btnTestingRun_Click);
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.scatterplotView2);
            this.groupBox11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox11.Location = new System.Drawing.Point(0, 0);
            this.groupBox11.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox11.Size = new System.Drawing.Size(841, 610);
            this.groupBox11.TabIndex = 4;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Visualization";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.dgvPerformance);
            this.groupBox6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox6.Location = new System.Drawing.Point(0, 610);
            this.groupBox6.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox6.Size = new System.Drawing.Size(841, 110);
            this.groupBox6.TabIndex = 5;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Performance Measures";
            // 
            // dgvPerformance
            // 
            this.dgvPerformance.AllowUserToAddRows = false;
            this.dgvPerformance.AllowUserToDeleteRows = false;
            this.dgvPerformance.AllowUserToResizeRows = false;
            this.dgvPerformance.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPerformance.BackgroundColor = System.Drawing.Color.White;
            this.dgvPerformance.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvPerformance.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvPerformance.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPerformance.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column3,
            this.Column4,
            this.Column5,
            this.Column6,
            this.Column7,
            this.Column8,
            this.Column10,
            this.Column9});
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvPerformance.DefaultCellStyle = dataGridViewCellStyle6;
            this.dgvPerformance.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPerformance.Location = new System.Drawing.Point(4, 23);
            this.dgvPerformance.Margin = new System.Windows.Forms.Padding(4);
            this.dgvPerformance.Name = "dgvPerformance";
            this.dgvPerformance.ReadOnly = true;
            this.dgvPerformance.RowHeadersVisible = false;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dgvPerformance.RowsDefaultCellStyle = dataGridViewCellStyle7;
            this.dgvPerformance.Size = new System.Drawing.Size(833, 83);
            this.dgvPerformance.TabIndex = 2;
            // 
            // Column3
            // 
            this.Column3.DataPropertyName = "TruePositives";
            this.Column3.HeaderText = "True Positives";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            // 
            // Column4
            // 
            this.Column4.DataPropertyName = "FalseNegatives";
            this.Column4.HeaderText = "False Negatives";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            // 
            // Column5
            // 
            this.Column5.DataPropertyName = "TrueNegatives";
            this.Column5.HeaderText = "True Negatives";
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            // 
            // Column6
            // 
            this.Column6.DataPropertyName = "FalsePositives";
            this.Column6.HeaderText = "False Positives";
            this.Column6.Name = "Column6";
            this.Column6.ReadOnly = true;
            // 
            // Column7
            // 
            this.Column7.DataPropertyName = "Sensitivity";
            this.Column7.HeaderText = "Sensitivity";
            this.Column7.Name = "Column7";
            this.Column7.ReadOnly = true;
            // 
            // Column8
            // 
            this.Column8.DataPropertyName = "Specificity";
            this.Column8.HeaderText = "Specificity";
            this.Column8.Name = "Column8";
            this.Column8.ReadOnly = true;
            // 
            // Column10
            // 
            this.Column10.DataPropertyName = "Efficiency";
            this.Column10.HeaderText = "Efficiency";
            this.Column10.Name = "Column10";
            this.Column10.ReadOnly = true;
            // 
            // Column9
            // 
            this.Column9.DataPropertyName = "Accuracy";
            this.Column9.HeaderText = "Accuracy";
            this.Column9.Name = "Column9";
            this.Column9.ReadOnly = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lbStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 796);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 21, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1258, 30);
            this.statusStrip1.TabIndex = 15;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lbStatus
            // 
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(248, 25);
            this.lbStatus.Text = "Click on File > Open to begin!";
            // 
            // scatterplotView1
            // 
            this.scatterplotView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scatterplotView1.LinesVisible = false;
            this.scatterplotView1.Location = new System.Drawing.Point(4, 23);
            this.scatterplotView1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.scatterplotView1.Name = "scatterplotView1";
            this.scatterplotView1.ScaleTight = false;
            this.scatterplotView1.Size = new System.Drawing.Size(879, 693);
            this.scatterplotView1.SymbolSize = 7F;
            this.scatterplotView1.TabIndex = 0;
            // 
            // scatterplotView2
            // 
            this.scatterplotView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scatterplotView2.LinesVisible = false;
            this.scatterplotView2.Location = new System.Drawing.Point(4, 23);
            this.scatterplotView2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.scatterplotView2.Name = "scatterplotView2";
            this.scatterplotView2.ScaleTight = false;
            this.scatterplotView2.Size = new System.Drawing.Size(833, 583);
            this.scatterplotView2.SymbolSize = 7F;
            this.scatterplotView2.TabIndex = 0;
            // 
            // scatterplotView3
            // 
            this.scatterplotView3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scatterplotView3.LinesVisible = false;
            this.scatterplotView3.Location = new System.Drawing.Point(4, 23);
            this.scatterplotView3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.scatterplotView3.Name = "scatterplotView3";
            this.scatterplotView3.ScaleTight = false;
            this.scatterplotView3.Size = new System.Drawing.Size(836, 693);
            this.scatterplotView3.SymbolSize = 7F;
            this.scatterplotView3.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1258, 826);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(1154, 788);
            this.Name = "MainForm";
            this.Text = "Clustering 2D (K-Means, MeanShift, GMM)";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabSamples.ResumeLayout(false);
            this.splitContainer7.Panel1.ResumeLayout(false);
            this.splitContainer7.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer7)).EndInit();
            this.splitContainer7.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLearningSource)).EndInit();
            this.groupBox15.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRadius)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGaussians)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numKMeans)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTestingSource)).EndInit();
            this.groupBox11.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPerformance)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabSamples;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Button btnTestingRun;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dgvTestingSource;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.SplitContainer splitContainer7;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.DataGridView dgvLearningSource;
        private System.Windows.Forms.GroupBox groupBox15;
        private System.Windows.Forms.Button btnSampleRunAnalysis;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rbKMeans;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numRadius;
        private System.Windows.Forms.RadioButton rbMeanShift;
        private System.Windows.Forms.NumericUpDown numKMeans;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.DataGridView dgvPerformance;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.RadioButton rbGMM;
        private System.Windows.Forms.NumericUpDown numGaussians;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lbStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column10;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column9;
        private System.Windows.Forms.CheckBox cbBalanced;
        private Accord.Controls.ScatterplotView scatterplotView1;
        private Accord.Controls.ScatterplotView scatterplotView2;
        private Accord.Controls.ScatterplotView scatterplotView3;
    }
}

