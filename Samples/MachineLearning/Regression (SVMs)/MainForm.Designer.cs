namespace Regression.SVMs
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle41 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle42 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle43 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle44 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle45 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle46 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle47 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle48 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle49 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle50 = new System.Windows.Forms.DataGridViewCellStyle();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
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
            this.graphInput = new ZedGraph.ZedGraphControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnEstimateSig = new System.Windows.Forms.Button();
            this.btnEstimateLaplacian = new System.Windows.Forms.Button();
            this.btnEstimateGaussian = new System.Windows.Forms.Button();
            this.btnEstimateC = new System.Windows.Forms.Button();
            this.btnSampleRunAnalysis = new System.Windows.Forms.Button();
            this.numT = new System.Windows.Forms.NumericUpDown();
            this.rbGaussian = new System.Windows.Forms.RadioButton();
            this.label7 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numSigB = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.numSigAlpha = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.numEpsilon = new System.Windows.Forms.NumericUpDown();
            this.numC = new System.Windows.Forms.NumericUpDown();
            this.numPolyConstant = new System.Windows.Forms.NumericUpDown();
            this.numDegree = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.rbSigmoid = new System.Windows.Forms.RadioButton();
            this.rbLaplacian = new System.Windows.Forms.RadioButton();
            this.rbPolynomial = new System.Windows.Forms.RadioButton();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.numLaplacianSigma = new System.Windows.Forms.NumericUpDown();
            this.numSigma = new System.Windows.Forms.NumericUpDown();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.zedGraphControl2 = new ZedGraph.ZedGraphControl();
            this.tabOverview = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dgvSupportVectors = new System.Windows.Forms.DataGridView();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.graphSupportVectors = new ZedGraph.ZedGraphControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgvTestingSource = new System.Windows.Forms.DataGridView();
            this.btnTestingRun = new System.Windows.Forms.Button();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.zedGraphControl1 = new ZedGraph.ZedGraphControl();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.dgvPerformance = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lbStatus = new System.Windows.Forms.ToolStripStatusLabel();
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
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSigB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSigAlpha)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEpsilon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPolyConstant)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDegree)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLaplacianSigma)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSigma)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.tabOverview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSupportVectors)).BeginInit();
            this.groupBox5.SuspendLayout();
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
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem6});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(763, 24);
            this.menuStrip1.TabIndex = 13;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.toolStripSeparator4,
            this.toolStripMenuItem5});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(37, 20);
            this.toolStripMenuItem1.Text = "&File";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem2.Image")));
            this.toolStripMenuItem2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.toolStripMenuItem2.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem2.Text = "&Open";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.MenuFileOpen_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(149, 6);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem5.Text = "E&xit";
            this.toolStripMenuItem5.Click += new System.EventHandler(this.toolStripMenuItem5_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem7});
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(44, 20);
            this.toolStripMenuItem6.Text = "&Help";
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(116, 22);
            this.toolStripMenuItem7.Text = "&About...";
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabSamples);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Controls.Add(this.tabOverview);
            this.tabControl.Controls.Add(this.tabPage4);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 24);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(763, 442);
            this.tabControl.TabIndex = 14;
            // 
            // tabSamples
            // 
            this.tabSamples.Controls.Add(this.splitContainer7);
            this.tabSamples.Location = new System.Drawing.Point(4, 22);
            this.tabSamples.Name = "tabSamples";
            this.tabSamples.Padding = new System.Windows.Forms.Padding(3);
            this.tabSamples.Size = new System.Drawing.Size(755, 416);
            this.tabSamples.TabIndex = 0;
            this.tabSamples.Text = "Samples (Input)";
            this.tabSamples.UseVisualStyleBackColor = true;
            // 
            // splitContainer7
            // 
            this.splitContainer7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer7.Location = new System.Drawing.Point(3, 3);
            this.splitContainer7.Name = "splitContainer7";
            // 
            // splitContainer7.Panel1
            // 
            this.splitContainer7.Panel1.Controls.Add(this.groupBox7);
            // 
            // splitContainer7.Panel2
            // 
            this.splitContainer7.Panel2.Controls.Add(this.groupBox15);
            this.splitContainer7.Size = new System.Drawing.Size(749, 410);
            this.splitContainer7.SplitterDistance = 240;
            this.splitContainer7.TabIndex = 9;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.dgvLearningSource);
            this.groupBox7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox7.Location = new System.Drawing.Point(0, 0);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(240, 410);
            this.groupBox7.TabIndex = 6;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Training data";
            // 
            // dgvLearningSource
            // 
            this.dgvLearningSource.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvLearningSource.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvLearningSource.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle41.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle41.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle41.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle41.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle41.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle41.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle41.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvLearningSource.DefaultCellStyle = dataGridViewCellStyle41;
            this.dgvLearningSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLearningSource.Location = new System.Drawing.Point(3, 16);
            this.dgvLearningSource.Name = "dgvLearningSource";
            this.dgvLearningSource.Size = new System.Drawing.Size(234, 391);
            this.dgvLearningSource.TabIndex = 5;
            // 
            // groupBox15
            // 
            this.groupBox15.Controls.Add(this.graphInput);
            this.groupBox15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox15.Location = new System.Drawing.Point(0, 0);
            this.groupBox15.Name = "groupBox15";
            this.groupBox15.Size = new System.Drawing.Size(505, 410);
            this.groupBox15.TabIndex = 7;
            this.groupBox15.TabStop = false;
            this.groupBox15.Text = "Scatter Plot";
            // 
            // graphInput
            // 
            this.graphInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graphInput.Location = new System.Drawing.Point(3, 16);
            this.graphInput.Name = "graphInput";
            this.graphInput.ScrollGrace = 0D;
            this.graphInput.ScrollMaxX = 0D;
            this.graphInput.ScrollMaxY = 0D;
            this.graphInput.ScrollMaxY2 = 0D;
            this.graphInput.ScrollMinX = 0D;
            this.graphInput.ScrollMinY = 0D;
            this.graphInput.ScrollMinY2 = 0D;
            this.graphInput.Size = new System.Drawing.Size(499, 391);
            this.graphInput.TabIndex = 4;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.splitContainer3);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(755, 416);
            this.tabPage2.TabIndex = 14;
            this.tabPage2.Text = "Machine Creation";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(3, 3);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.groupBox3);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.groupBox4);
            this.splitContainer3.Size = new System.Drawing.Size(749, 410);
            this.splitContainer3.SplitterDistance = 240;
            this.splitContainer3.TabIndex = 13;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnEstimateSig);
            this.groupBox3.Controls.Add(this.btnEstimateLaplacian);
            this.groupBox3.Controls.Add(this.btnEstimateGaussian);
            this.groupBox3.Controls.Add(this.btnEstimateC);
            this.groupBox3.Controls.Add(this.btnSampleRunAnalysis);
            this.groupBox3.Controls.Add(this.numT);
            this.groupBox3.Controls.Add(this.rbGaussian);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.numSigB);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.numSigAlpha);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.numEpsilon);
            this.groupBox3.Controls.Add(this.numC);
            this.groupBox3.Controls.Add(this.numPolyConstant);
            this.groupBox3.Controls.Add(this.numDegree);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.rbSigmoid);
            this.groupBox3.Controls.Add(this.rbLaplacian);
            this.groupBox3.Controls.Add(this.rbPolynomial);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.numLaplacianSigma);
            this.groupBox3.Controls.Add(this.numSigma);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(240, 410);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Settings";
            // 
            // btnEstimateSig
            // 
            this.btnEstimateSig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEstimateSig.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEstimateSig.Location = new System.Drawing.Point(181, 213);
            this.btnEstimateSig.Name = "btnEstimateSig";
            this.btnEstimateSig.Size = new System.Drawing.Size(53, 47);
            this.btnEstimateSig.TabIndex = 1;
            this.btnEstimateSig.Text = "Estimate";
            this.btnEstimateSig.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnEstimateSig.UseVisualStyleBackColor = true;
            this.btnEstimateSig.Click += new System.EventHandler(this.btnEstimateSigmoid_Click);
            // 
            // btnEstimateLaplacian
            // 
            this.btnEstimateLaplacian.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEstimateLaplacian.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEstimateLaplacian.Location = new System.Drawing.Point(180, 164);
            this.btnEstimateLaplacian.Name = "btnEstimateLaplacian";
            this.btnEstimateLaplacian.Size = new System.Drawing.Size(53, 20);
            this.btnEstimateLaplacian.TabIndex = 1;
            this.btnEstimateLaplacian.Text = "Estimate";
            this.btnEstimateLaplacian.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnEstimateLaplacian.UseVisualStyleBackColor = true;
            this.btnEstimateLaplacian.Click += new System.EventHandler(this.btnEstimateLaplacian_Click);
            // 
            // btnEstimateGaussian
            // 
            this.btnEstimateGaussian.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEstimateGaussian.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEstimateGaussian.Location = new System.Drawing.Point(184, 41);
            this.btnEstimateGaussian.Name = "btnEstimateGaussian";
            this.btnEstimateGaussian.Size = new System.Drawing.Size(53, 20);
            this.btnEstimateGaussian.TabIndex = 1;
            this.btnEstimateGaussian.Text = "Estimate";
            this.btnEstimateGaussian.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnEstimateGaussian.UseVisualStyleBackColor = true;
            this.btnEstimateGaussian.Click += new System.EventHandler(this.btnEstimateGaussian_Click);
            // 
            // btnEstimateC
            // 
            this.btnEstimateC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEstimateC.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEstimateC.Location = new System.Drawing.Point(181, 281);
            this.btnEstimateC.Name = "btnEstimateC";
            this.btnEstimateC.Size = new System.Drawing.Size(53, 20);
            this.btnEstimateC.TabIndex = 1;
            this.btnEstimateC.Text = "Estimate";
            this.btnEstimateC.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnEstimateC.UseVisualStyleBackColor = true;
            this.btnEstimateC.Click += new System.EventHandler(this.btnEstimateC_Click);
            // 
            // btnSampleRunAnalysis
            // 
            this.btnSampleRunAnalysis.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSampleRunAnalysis.Location = new System.Drawing.Point(9, 365);
            this.btnSampleRunAnalysis.Name = "btnSampleRunAnalysis";
            this.btnSampleRunAnalysis.Size = new System.Drawing.Size(228, 39);
            this.btnSampleRunAnalysis.TabIndex = 1;
            this.btnSampleRunAnalysis.Text = "Create Machine";
            this.btnSampleRunAnalysis.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnSampleRunAnalysis.UseVisualStyleBackColor = true;
            this.btnSampleRunAnalysis.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // numT
            // 
            this.numT.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numT.DecimalPlaces = 7;
            this.numT.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numT.Location = new System.Drawing.Point(94, 333);
            this.numT.Name = "numT";
            this.numT.Size = new System.Drawing.Size(139, 20);
            this.numT.TabIndex = 7;
            this.numT.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numT.Value = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            // 
            // rbGaussian
            // 
            this.rbGaussian.AutoSize = true;
            this.rbGaussian.Checked = true;
            this.rbGaussian.Location = new System.Drawing.Point(19, 19);
            this.rbGaussian.Name = "rbGaussian";
            this.rbGaussian.Size = new System.Drawing.Size(102, 17);
            this.rbGaussian.TabIndex = 6;
            this.rbGaussian.TabStop = true;
            this.rbGaussian.Text = "Gaussian Kernel";
            this.rbGaussian.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 167);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(39, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Sigma:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Sigma:";
            // 
            // numSigB
            // 
            this.numSigB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numSigB.DecimalPlaces = 6;
            this.numSigB.Location = new System.Drawing.Point(96, 240);
            this.numSigB.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numSigB.Name = "numSigB";
            this.numSigB.Size = new System.Drawing.Size(78, 20);
            this.numSigB.TabIndex = 7;
            this.numSigB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(21, 216);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(34, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Alpha";
            // 
            // numSigAlpha
            // 
            this.numSigAlpha.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numSigAlpha.DecimalPlaces = 7;
            this.numSigAlpha.Location = new System.Drawing.Point(96, 214);
            this.numSigAlpha.Name = "numSigAlpha";
            this.numSigAlpha.Size = new System.Drawing.Size(78, 20);
            this.numSigAlpha.TabIndex = 7;
            this.numSigAlpha.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(18, 119);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(52, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "Constant:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 93);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Degree:";
            // 
            // numEpsilon
            // 
            this.numEpsilon.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numEpsilon.DecimalPlaces = 7;
            this.numEpsilon.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numEpsilon.Location = new System.Drawing.Point(94, 307);
            this.numEpsilon.Name = "numEpsilon";
            this.numEpsilon.Size = new System.Drawing.Size(139, 20);
            this.numEpsilon.TabIndex = 7;
            this.numEpsilon.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numEpsilon.Value = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            // 
            // numC
            // 
            this.numC.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numC.DecimalPlaces = 7;
            this.numC.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numC.Location = new System.Drawing.Point(94, 281);
            this.numC.Name = "numC";
            this.numC.Size = new System.Drawing.Size(80, 20);
            this.numC.TabIndex = 7;
            this.numC.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numC.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numPolyConstant
            // 
            this.numPolyConstant.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numPolyConstant.DecimalPlaces = 7;
            this.numPolyConstant.Location = new System.Drawing.Point(96, 117);
            this.numPolyConstant.Name = "numPolyConstant";
            this.numPolyConstant.Size = new System.Drawing.Size(137, 20);
            this.numPolyConstant.TabIndex = 7;
            this.numPolyConstant.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numPolyConstant.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numDegree
            // 
            this.numDegree.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numDegree.Location = new System.Drawing.Point(96, 91);
            this.numDegree.Name = "numDegree";
            this.numDegree.Size = new System.Drawing.Size(137, 20);
            this.numDegree.TabIndex = 7;
            this.numDegree.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numDegree.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 242);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Constant:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 283);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Complexity";
            // 
            // rbSigmoid
            // 
            this.rbSigmoid.AutoSize = true;
            this.rbSigmoid.Location = new System.Drawing.Point(19, 191);
            this.rbSigmoid.Name = "rbSigmoid";
            this.rbSigmoid.Size = new System.Drawing.Size(95, 17);
            this.rbSigmoid.TabIndex = 6;
            this.rbSigmoid.TabStop = true;
            this.rbSigmoid.Text = "Sigmoid Kernel";
            this.rbSigmoid.UseVisualStyleBackColor = true;
            // 
            // rbLaplacian
            // 
            this.rbLaplacian.AutoSize = true;
            this.rbLaplacian.Location = new System.Drawing.Point(19, 145);
            this.rbLaplacian.Name = "rbLaplacian";
            this.rbLaplacian.Size = new System.Drawing.Size(104, 17);
            this.rbLaplacian.TabIndex = 6;
            this.rbLaplacian.TabStop = true;
            this.rbLaplacian.Text = "Laplacian Kernel";
            this.rbLaplacian.UseVisualStyleBackColor = true;
            // 
            // rbPolynomial
            // 
            this.rbPolynomial.AutoSize = true;
            this.rbPolynomial.Location = new System.Drawing.Point(19, 68);
            this.rbPolynomial.Name = "rbPolynomial";
            this.rbPolynomial.Size = new System.Drawing.Size(108, 17);
            this.rbPolynomial.TabIndex = 6;
            this.rbPolynomial.TabStop = true;
            this.rbPolynomial.Text = "Polynomial Kernel";
            this.rbPolynomial.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(20, 309);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(41, 13);
            this.label10.TabIndex = 3;
            this.label10.Text = "Epsilon";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(18, 335);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(55, 13);
            this.label9.TabIndex = 3;
            this.label9.Text = "Tolerance";
            // 
            // numLaplacianSigma
            // 
            this.numLaplacianSigma.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numLaplacianSigma.DecimalPlaces = 7;
            this.numLaplacianSigma.Location = new System.Drawing.Point(90, 165);
            this.numLaplacianSigma.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numLaplacianSigma.Name = "numLaplacianSigma";
            this.numLaplacianSigma.Size = new System.Drawing.Size(84, 20);
            this.numLaplacianSigma.TabIndex = 7;
            this.numLaplacianSigma.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numLaplacianSigma.Value = new decimal(new int[] {
            12236,
            0,
            0,
            262144});
            // 
            // numSigma
            // 
            this.numSigma.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numSigma.DecimalPlaces = 7;
            this.numSigma.Location = new System.Drawing.Point(95, 42);
            this.numSigma.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numSigma.Name = "numSigma";
            this.numSigma.Size = new System.Drawing.Size(84, 20);
            this.numSigma.TabIndex = 7;
            this.numSigma.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numSigma.Value = new decimal(new int[] {
            12236,
            0,
            0,
            262144});
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.zedGraphControl2);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Location = new System.Drawing.Point(0, 0);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(505, 410);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Visualization";
            // 
            // zedGraphControl2
            // 
            this.zedGraphControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.zedGraphControl2.Location = new System.Drawing.Point(3, 16);
            this.zedGraphControl2.Name = "zedGraphControl2";
            this.zedGraphControl2.ScrollGrace = 0D;
            this.zedGraphControl2.ScrollMaxX = 0D;
            this.zedGraphControl2.ScrollMaxY = 0D;
            this.zedGraphControl2.ScrollMaxY2 = 0D;
            this.zedGraphControl2.ScrollMinX = 0D;
            this.zedGraphControl2.ScrollMinY = 0D;
            this.zedGraphControl2.ScrollMinY2 = 0D;
            this.zedGraphControl2.Size = new System.Drawing.Size(499, 391);
            this.zedGraphControl2.TabIndex = 3;
            // 
            // tabOverview
            // 
            this.tabOverview.Controls.Add(this.splitContainer2);
            this.tabOverview.Location = new System.Drawing.Point(4, 22);
            this.tabOverview.Name = "tabOverview";
            this.tabOverview.Padding = new System.Windows.Forms.Padding(3);
            this.tabOverview.Size = new System.Drawing.Size(755, 416);
            this.tabOverview.TabIndex = 1;
            this.tabOverview.Text = "Support Vectors";
            this.tabOverview.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.groupBox5);
            this.splitContainer2.Size = new System.Drawing.Size(749, 410);
            this.splitContainer2.SplitterDistance = 240;
            this.splitContainer2.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dgvSupportVectors);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(240, 410);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Support Vectors";
            // 
            // dgvSupportVectors
            // 
            this.dgvSupportVectors.AllowUserToAddRows = false;
            this.dgvSupportVectors.AllowUserToDeleteRows = false;
            this.dgvSupportVectors.AllowUserToResizeRows = false;
            this.dgvSupportVectors.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSupportVectors.BackgroundColor = System.Drawing.Color.White;
            this.dgvSupportVectors.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle42.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle42.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle42.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle42.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle42.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle42.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle42.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSupportVectors.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle42;
            this.dgvSupportVectors.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle43.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle43.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle43.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle43.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle43.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle43.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle43.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvSupportVectors.DefaultCellStyle = dataGridViewCellStyle43;
            this.dgvSupportVectors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSupportVectors.Location = new System.Drawing.Point(3, 16);
            this.dgvSupportVectors.Name = "dgvSupportVectors";
            this.dgvSupportVectors.ReadOnly = true;
            this.dgvSupportVectors.RowHeadersVisible = false;
            dataGridViewCellStyle44.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dgvSupportVectors.RowsDefaultCellStyle = dataGridViewCellStyle44;
            this.dgvSupportVectors.Size = new System.Drawing.Size(234, 391);
            this.dgvSupportVectors.TabIndex = 1;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.graphSupportVectors);
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox5.Location = new System.Drawing.Point(0, 0);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(505, 410);
            this.groupBox5.TabIndex = 4;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Visualization";
            // 
            // graphSupportVectors
            // 
            this.graphSupportVectors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graphSupportVectors.Location = new System.Drawing.Point(3, 16);
            this.graphSupportVectors.Name = "graphSupportVectors";
            this.graphSupportVectors.ScrollGrace = 0D;
            this.graphSupportVectors.ScrollMaxX = 0D;
            this.graphSupportVectors.ScrollMaxY = 0D;
            this.graphSupportVectors.ScrollMaxY2 = 0D;
            this.graphSupportVectors.ScrollMinX = 0D;
            this.graphSupportVectors.ScrollMinY = 0D;
            this.graphSupportVectors.ScrollMinY2 = 0D;
            this.graphSupportVectors.Size = new System.Drawing.Size(499, 391);
            this.graphSupportVectors.TabIndex = 3;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.splitContainer1);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(755, 416);
            this.tabPage4.TabIndex = 12;
            this.tabPage4.Text = "Model Testing";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
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
            this.splitContainer1.Size = new System.Drawing.Size(749, 410);
            this.splitContainer1.SplitterDistance = 240;
            this.splitContainer1.TabIndex = 6;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.dgvTestingSource);
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(240, 339);
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
            dataGridViewCellStyle45.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle45.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle45.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle45.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle45.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle45.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle45.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTestingSource.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle45;
            this.dgvTestingSource.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle46.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle46.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle46.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle46.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle46.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle46.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle46.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvTestingSource.DefaultCellStyle = dataGridViewCellStyle46;
            this.dgvTestingSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTestingSource.Location = new System.Drawing.Point(3, 16);
            this.dgvTestingSource.Name = "dgvTestingSource";
            this.dgvTestingSource.ReadOnly = true;
            this.dgvTestingSource.RowHeadersVisible = false;
            dataGridViewCellStyle47.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dgvTestingSource.RowsDefaultCellStyle = dataGridViewCellStyle47;
            this.dgvTestingSource.Size = new System.Drawing.Size(234, 320);
            this.dgvTestingSource.TabIndex = 1;
            // 
            // btnTestingRun
            // 
            this.btnTestingRun.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTestingRun.Location = new System.Drawing.Point(5, 345);
            this.btnTestingRun.Name = "btnTestingRun";
            this.btnTestingRun.Size = new System.Drawing.Size(232, 60);
            this.btnTestingRun.TabIndex = 3;
            this.btnTestingRun.Text = "Run";
            this.btnTestingRun.UseVisualStyleBackColor = true;
            this.btnTestingRun.Click += new System.EventHandler(this.btnTestingRun_Click);
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.zedGraphControl1);
            this.groupBox11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox11.Location = new System.Drawing.Point(0, 0);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(505, 339);
            this.groupBox11.TabIndex = 4;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Visualization";
            // 
            // zedGraphControl1
            // 
            this.zedGraphControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.zedGraphControl1.Location = new System.Drawing.Point(3, 16);
            this.zedGraphControl1.Name = "zedGraphControl1";
            this.zedGraphControl1.ScrollGrace = 0D;
            this.zedGraphControl1.ScrollMaxX = 0D;
            this.zedGraphControl1.ScrollMaxY = 0D;
            this.zedGraphControl1.ScrollMaxY2 = 0D;
            this.zedGraphControl1.ScrollMinX = 0D;
            this.zedGraphControl1.ScrollMinY = 0D;
            this.zedGraphControl1.ScrollMinY2 = 0D;
            this.zedGraphControl1.Size = new System.Drawing.Size(499, 320);
            this.zedGraphControl1.TabIndex = 3;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.dgvPerformance);
            this.groupBox6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox6.Location = new System.Drawing.Point(0, 339);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(505, 71);
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
            dataGridViewCellStyle48.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle48.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle48.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle48.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle48.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle48.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle48.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvPerformance.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle48;
            this.dgvPerformance.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPerformance.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            dataGridViewCellStyle49.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle49.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle49.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle49.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle49.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle49.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle49.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvPerformance.DefaultCellStyle = dataGridViewCellStyle49;
            this.dgvPerformance.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPerformance.Location = new System.Drawing.Point(3, 16);
            this.dgvPerformance.Name = "dgvPerformance";
            this.dgvPerformance.ReadOnly = true;
            this.dgvPerformance.RowHeadersVisible = false;
            dataGridViewCellStyle50.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dgvPerformance.RowsDefaultCellStyle = dataGridViewCellStyle50;
            this.dgvPerformance.Size = new System.Drawing.Size(499, 52);
            this.dgvPerformance.TabIndex = 2;
            // 
            // Column1
            // 
            this.Column1.DataPropertyName = "RSquared";
            this.Column1.HeaderText = "R² (r-squared)";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Column2
            // 
            this.Column2.DataPropertyName = "Error";
            this.Column2.HeaderText = "Error sum-of-squares";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lbStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 466);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(763, 22);
            this.statusStrip1.TabIndex = 15;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lbStatus
            // 
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(147, 17);
            this.lbStatus.Text = "Click File > Open to begin!";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(763, 488);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Kernel Support Vector Machines for regression problems";
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
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSigB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSigAlpha)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEpsilon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPolyConstant)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDegree)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLaplacianSigma)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSigma)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.tabOverview.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSupportVectors)).EndInit();
            this.groupBox5.ResumeLayout(false);
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
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabSamples;
        private System.Windows.Forms.TabPage tabOverview;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgvSupportVectors;
        private ZedGraph.ZedGraphControl graphSupportVectors;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Button btnTestingRun;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dgvTestingSource;
        private System.Windows.Forms.SplitContainer splitContainer7;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.DataGridView dgvLearningSource;
        private System.Windows.Forms.GroupBox groupBox15;
        private ZedGraph.ZedGraphControl graphInput;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox11;
        private ZedGraph.ZedGraphControl zedGraphControl1;
        private System.Windows.Forms.DataGridView dgvPerformance;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnEstimateSig;
        private System.Windows.Forms.Button btnEstimateLaplacian;
        private System.Windows.Forms.Button btnEstimateGaussian;
        private System.Windows.Forms.Button btnEstimateC;
        private System.Windows.Forms.Button btnSampleRunAnalysis;
        private System.Windows.Forms.NumericUpDown numT;
        private System.Windows.Forms.RadioButton rbGaussian;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numSigB;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numSigAlpha;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numEpsilon;
        private System.Windows.Forms.NumericUpDown numC;
        private System.Windows.Forms.NumericUpDown numPolyConstant;
        private System.Windows.Forms.NumericUpDown numDegree;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rbSigmoid;
        private System.Windows.Forms.RadioButton rbLaplacian;
        private System.Windows.Forms.RadioButton rbPolynomial;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown numLaplacianSigma;
        private System.Windows.Forms.NumericUpDown numSigma;
        private System.Windows.Forms.GroupBox groupBox4;
        private ZedGraph.ZedGraphControl zedGraphControl2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lbStatus;
    }
}

